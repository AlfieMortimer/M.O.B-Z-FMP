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

        if (cost <= points.playerPoints[Convert.ToInt32(OwnerClientId.ToString())])
        {

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
            }
        }
    }
}
