using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodymary.Game
{
    public class WeaponManager: MonoBehaviour
    {
        public static WeaponManager WManager;

        public Weapon wBite;
        public Weapon wKnife;
        public Weapon wGun;
        public Weapon wGrenade;

        public Weapon CurrentMassDamageWeapon { get; private set; }

        public enum WeaponName
        {
            Бита = 1,
            Нож = 2,
            Револьвер = 3,
            Граната = 4

        }

        public enum WeaponType
        {
            Melee,
            Fire,
            Throw,
            None
        }

        public Weapon SetupWeaponByName(string name, Transform parent)
        {           
            Weapon weapon;
            switch (name.ToLower())
            {
                case "бита":
                    weapon = wBite; 
                    break;
                case "нож":
                    weapon = wKnife; 
                    break;
                case "револьвер":
                    weapon = wGun; 
                    break;
                case "граната":
                    weapon = wGrenade; 
                    break;
                default:
                    return null;
            }
            return SetupWeapon(weapon, parent);                       
        }

        public Weapon SetupWeapon(Weapon weapon, Transform parent)
        {
            if (parent.childCount > 0) Destroy(parent.GetChild(0).gameObject);
            return Instantiate(weapon, parent);
        }

        public void SetCurrentMassDamageWeapon(Weapon weapon)
        {
            CurrentMassDamageWeapon = weapon;
        }

        private void Awake()
        {
            WManager = this;
        }
    }
}

