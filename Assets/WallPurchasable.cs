using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

public class WallPurchasable : NetworkBehaviour, IInteractable
{
    public int weaponCode;
    public int cost;

    public PlayerWeapons weapons;
    public PointsCollection points;

    public void Start()
    {
        points = GameObject.FindGameObjectWithTag("NetworkFunctions").GetComponent<PointsCollection>();
    }
    public void Interact(GameObject p)
    {
        weapons = p.GetComponent<PlayerWeapons>();
        weapons.am.playsfx(weapons.am.sounds[5]);
        if (cost <= points.playerPoints[Convert.ToInt32(OwnerClientId.ToString())])
        {
            if(weaponCode == 2 || weapons.weaponOne == 0)
            {
                weapons.anim = weapons.saigaAnim;
                weapons.pistolGameobject.SetActive(false);
                weapons.saigaGameobject.SetActive(true);
                weapons.weaponOne = weaponCode;
                weapons.stats.ChangeWeaponStats(weapons.weaponOne);
                weapons.bulletsLeft = weapons.stats.magazineSize;
                weapons.bullets.text = weapons.bulletsLeft.ToString();
            }



            //ModelSwap
            



            //Buying weapons with multiple weapon slots
            /*
            if (weapons.HeldWeapon && weapons.weaponTwo == 0)
            {
                weapons.HeldWeapon = false;
                weapons.weaponTwo = weaponCode;
                weapons.stats.ChangeWeaponStats(weaponCode);
                weapons.weaponTwo = weapons.stats.magazineSize;
            }
            else if (weapons.HeldWeapon && weapons.weaponTwo < 0)
            {
                weapons.HeldWeapon = false;
                weapons.weaponOne = weaponCode;
                weapons.stats.ChangeWeaponStats(weaponCode);
                weapons.weaponTwo = weapons.stats.magazineSize;
            }
            else
            {
                weapons.HeldWeapon = true;
                weapons.weaponTwo = weaponCode;
                weapons.stats.ChangeWeaponStats(weaponCode);
                weapons.weaponOne = weapons.stats.magazineSize;
            }*/
        }
    }
}
