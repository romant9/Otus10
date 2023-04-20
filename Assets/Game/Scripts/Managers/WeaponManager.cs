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
            ���� = 1,
            ��� = 2,
            ��������� = 3,
            ������� = 4

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
                case "����":
                    weapon = wBite; 
                    break;
                case "���":
                    weapon = wKnife; 
                    break;
                case "���������":
                    weapon = wGun; 
                    break;
                case "�������":
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

