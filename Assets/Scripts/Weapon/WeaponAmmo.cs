using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAmmo : MonoBehaviour
{
    private Weapon weapon;

    private readonly string WEAPON_AMMO_SAVELOAD = "Weapon_";

    private void Awake()
    {
        weapon = GetComponent<Weapon>();
        RefillAmmo();
    }

    public void ConsumeAmmo()
    {
        if (weapon.UseMagazine)
        {
            weapon.CurrentAmmo -= 1;
        }
    }

    // checks for ammo
    public bool CanUseWeapon()
    {
        if (weapon.CurrentAmmo > 0)
        {
            return true;
        }

        return false;
    }

    public void RefillAmmo()
    {
        if (weapon.UseMagazine)
        {
            weapon.CurrentAmmo = LoadAmmo();
        }
    }

    public void SaveAmmo() {
        PlayerPrefs.SetInt(WEAPON_AMMO_SAVELOAD + weapon.WeaponName, weapon.CurrentAmmo);
    }

    public int LoadAmmo() {
        // returns the saved ammo or the magazine size if the 1st argument (the string) is null
        return PlayerPrefs.GetInt(WEAPON_AMMO_SAVELOAD + weapon.WeaponName, weapon.MagazineSize);
    }
}
