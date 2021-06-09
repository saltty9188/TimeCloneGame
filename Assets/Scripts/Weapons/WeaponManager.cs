using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The WeaponManager class is responsible for managing all of the <see cref="Weapons">Weapons</see> in a scene.
/// </summary>
public class WeaponManager : MonoBehaviour
{
    #region  Public fields
    /// <summary>
    /// Static list of all the <see cref="Weapon">Weapons</see> in the scene.
    /// </summary>
    public static List<Weapon> Weapons;
    #endregion 

    void Awake()
    {
        Weapons = new List<Weapon>();
    }

    /// <summary>
    /// Sets the initial position of every Weapon in the scene.
    /// </summary>
    /// <seealso cref="Weapon.SetInitalPosition"/>
    public static void SetDefaultPosition()
    {
        for(int i = 0; i < Weapons.Count; i++)
        {
            Weapon weapon = Weapons[i];
            if(weapon == null)
            {
                Weapons.Remove(weapon);
                i--;
            }
            else
            {
                weapon.SetInitalPosition();
            }
        }
    }

    /// <summary>
    /// Resets the position of every Weapon in the scene.
    /// </summary>
    /// <seealso cref="Weapon.ResetWeapon"/>
    /// <param name="exclude"></param>
    public static void ResetAllWeapons()
    {
        for(int i = 0; i < Weapons.Count; i++)
        {
            Weapon weapon = Weapons[i];
            if(weapon == null)
            {
                Weapons.Remove(weapon);
                i--;
            }
            else
            {
                weapon.gameObject.SetActive(true);
                weapon.ResetWeapon();
            }
        }
    }
}
