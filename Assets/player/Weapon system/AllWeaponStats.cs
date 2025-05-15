using UnityEngine;

[CreateAssetMenu(fileName = "WeaponStats", menuName = "WeaponStats")]

public class AllWeaponStats : ScriptableObject
{
    //stats base
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;

    public void ChangeWeaponStats(int weapon)
    {
        //I HATE THIS BUT HOW THE **** DO I DO IT OTHERWISE
        if(weapon == 0)
        {
            PistolSwitch();
        }
        else if(weapon == 1)
        {
            ShotgunSwitch();
        }
        else if(weapon == 2)
        {
            RifleSwitch();
        }
        else if(weapon == 3)
        {
            empty();
        }
    }

    public void empty()
    {
        damage = 0;
        timeBetweenShooting = 0f;
        spread = 0;
        range = 100;
        reloadTime = 0;
        magazineSize = 0;
        bulletsPerTap = 0;
        allowButtonHold = false;
    }

    public void PistolSwitch()
    {
        damage = 2;
        timeBetweenShooting = .05f;
        spread = 0;
        range = 100;
        reloadTime = 2;
        magazineSize = 8;
        bulletsPerTap = 1;
        allowButtonHold = false;
    }

    public void ShotgunSwitch()
    {
        damage = 6;
        timeBetweenShooting = 1f;
        spread = 2;
        range = 100;
        reloadTime = 2;
        magazineSize = 6;
        bulletsPerTap = 8;
        allowButtonHold = false;
    }

    public void RifleSwitch()
    {
        damage = 10;
        timeBetweenShooting = .1f;
        spread = 0;
        range = 100;
        reloadTime = 2f;
        magazineSize = 45;
        bulletsPerTap = 1;
        allowButtonHold = true;
    }
}
