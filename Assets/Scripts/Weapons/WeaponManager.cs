using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static List<Weapon> weapons;

    void Awake()
    {
        weapons = new List<Weapon>();
    }

    public static void SetDefaultPosition()
    {
        for(int i = 0; i < weapons.Count; i++)
        {
            Weapon weapon = weapons[i];
            if(weapon == null)
            {
                weapons.Remove(weapon);
                i--;
            }
            else
            {
                weapon.SetDefaultPosition();
            }
        }
    }

    public static void ResetAllWeapons(Weapon exclude = null)
    {
        for(int i = 0; i < weapons.Count; i++)
        {
            Weapon weapon = weapons[i];
            if(weapon == null)
            {
                weapons.Remove(weapon);
                i--;
            }
            else
            {
                weapon.gameObject.SetActive(true);
                if(weapon != exclude)
                {
                    weapon.ResetWeapon();
                }
            }
        }
    }
}
