using UnityEngine;
using TMPro;
using Unity.Netcode;
using System.Collections.Generic;
using System.Collections;
using System;

public class PlayerWeapons : NetworkBehaviour
{
    //Keybindings
    public KeyCode shoot, reload, swapWeaponUp, swapWeaponDown;

    //Equipped Weapon
    public int selectedWeapon;

    public List<string> weapons = new List<string>();

    public int weaponOne = 0;
    int weaponAmmoOne = -5;
    public int WeaponReserveOne;

    public int weaponTwo;
    int weaponAmmoTwo = -5;
    public int weaponReserveTwo;

    public bool HeldWeapon;

    //Gun stats
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    //bools
    [SerializeField]
    bool shooting, readyToShoot, reloading;

    //references
    public Camera cam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask Enemy;
    public TextMeshProUGUI bullets;
    
    Animator anim;
    NetworkObject nO;
    public AllWeaponStats stats;


    //BulletFX
    public Transform bulletSpawn;
    public TrailRenderer BulletTrail;
    public ParticleSystem impact;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
        anim = GetComponentInChildren<Animator>();
        nO = GetComponent<NetworkObject>();
    }

    private void Start()
    {
        stats.ChangeWeaponStats(weaponOne);
    }
    private void Update()
    {
        MyInput();
    }

    private void MyInput()
    {
        if (nO.IsOwner)
        {

            if (stats.allowButtonHold)
            {
                shooting = Input.GetKey(shoot);
            }
            else
            {
                shooting = Input.GetKeyDown(shoot);
            }

            if (Input.GetKeyDown(reload) && bulletsLeft < stats.magazineSize && !reloading)
            {
                ReloadFunc();

            }

            if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
            {
                bulletsShot = stats.bulletsPerTap;
                ShootFunc();
            }
            if (Input.GetKeyDown(swapWeaponUp) || Input.GetKeyDown(swapWeaponDown))
            {
                changeWeapon();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(cam.transform.position, cam.transform.forward, Color.red, 1);
    }
    private void ShootFunc()
    {
        readyToShoot = false;
        bool enemydamage = false;
        Debug.Log("Shot Bullet");

        //spread
        float x = UnityEngine.Random.Range(-stats.spread, stats.spread);
        float y = UnityEngine.Random.Range(-stats.spread, stats.spread);

        //Calculate direction with spread
        Vector3 direction = cam.transform.forward + new Vector3(x, y, 0);

        anim.Play("Shot");

        //FX
        TrailRenderer trail = Instantiate(BulletTrail, bulletSpawn.position, Quaternion.identity);

        //Raycast
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out rayHit, stats.range, Enemy))
        {
            Debug.Log(rayHit.collider.name);

            StartCoroutine(SpawnTrail(trail, rayHit));

            if (rayHit.collider.CompareTag("Enemy") && enemydamage == false)
            {
                Debug.Log("You have hit an enemy");
                enemydamage = true;
                rayHit.collider.GetComponent<BasicEnemyStats>().TakeDamageRpc(Convert.ToInt32(nO.OwnerClientId.ToString()), stats.damage);
                UpdatePointsUI();
            }
        }

        bulletsLeft--;
        bullets.text = bulletsLeft.ToString();
        Invoke("ResetShot", stats.timeBetweenShooting);

        if (bulletsLeft <= 0)
        {
            ReloadFunc();
        }


    }
    private void UpdatePointsUI()
    {

    }
    private void ResetShot()
    {
        readyToShoot = true;
    }
    private void ReloadFunc()
    {
        reloading = true;

        if (bulletsLeft <= 0)
        {
            anim.Play("Reload_Empty");
        }
        else
        {
            anim.Play("Reload_bullets");
        }

        Invoke("ReloadFinished", stats.reloadTime);
    }
    private void ReloadFinished()
    {
        reloading = false;
        bulletsLeft = stats.magazineSize;
        bullets.text = stats.magazineSize.ToString();
    }

    //I will have to RPC these at some point to make sure the outside model is synched across clients.
    private void changeWeapon()
    {
        HeldWeapon = !HeldWeapon;
        if (HeldWeapon)
        {
            Debug.Log("Weapon 1 equipped");
            Debug.Log("Weapon 2 has " + weaponAmmoTwo + " bullets left");
            stats.ChangeWeaponStats(weaponOne);
            weaponAmmoTwo = bulletsLeft;
            if (weaponAmmoOne <= -1)
            {
                bulletsLeft = stats.magazineSize;
                bullets.text = bulletsLeft.ToString();

            }
            else
            {
                bulletsLeft = weaponAmmoOne;
                bullets.text = bulletsLeft.ToString();

            }
        }
        else
        {
            stats.ChangeWeaponStats(weaponTwo);
            Debug.Log("Weapon 2 equipped");
            Debug.Log("Weapon 2 has " + weaponAmmoTwo + " bullets left");
            weaponAmmoOne = bulletsLeft;
            if (weaponAmmoTwo <= -1)
            {
                bulletsLeft = stats.magazineSize;
                bullets.text = bulletsLeft.ToString();
            }
            else
            {
                bulletsLeft = weaponAmmoTwo;
                bullets.text = bulletsLeft.ToString();

            }
        }
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while (time < 1)
        {

            trail.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }

        trail.transform.position = hit.point;
        Destroy(trail.gameObject, trail.time);
    }
}
