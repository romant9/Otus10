using System;
using System.Collections.Generic;
using System.Linq;
using static Bloodymary.Game.GameSettings;

namespace Bloodymary.Game
{
    [Serializable]
    public class Inventory
    {
        public Weapon[] weapons = new Weapon[3];

        private Dictionary<string, int> WeaponStaff = new Dictionary<string, int>(); //количество боеприпасов для каждого именного оружия
        private Queue<Weapon> queueWeapons = new Queue<Weapon>(); //очередь смены оружия

        public void Initialize()
        {
            WeaponStaff.Clear();
            if (weapons.First() == null)
            {
                weapons[0] = GSettings.firstWeapon;
                weapons[1] = GSettings.secondayWeapon;
            }
            
            //инициализация числа боеприпасов
            //i = 3 - предполагается, что в инвентаре только 3 оружия, но может быть и больше
            for (int i = 0; i < 3; i++)
            {
                if (weapons[i] != null)
                {
                    queueWeapons.Enqueue(weapons[i]);

                    if (weapons[i]._weaponType == WeaponManager.WeaponType.Fire)
                    {
                        WeaponStaff.Add(weapons[i]._weaponName, GSettings.bulletCount);
                    }
                    else if (weapons[i]._weaponType == WeaponManager.WeaponType.Throw)
                    {
                        WeaponStaff.Add(weapons[i]._weaponName, GSettings.grenadeCount);
                    }
                    else WeaponStaff.Add(weapons[i]._weaponName, 1); //мили оружие всегда 1
                }               
            }             
        }

        public void SetWeaponCount(Weapon weapon, int many)
        {
            if (WeaponStaff.ContainsKey(weapon._weaponName)) WeaponStaff[weapon._weaponName] = many;
        }
        public void ChangeWeaponCount(Weapon weapon, int one)
        {
            if (WeaponStaff.ContainsKey(weapon._weaponName))
                WeaponStaff[weapon._weaponName] += WeaponStaff[weapon._weaponName] > 0 ? one : -WeaponStaff[weapon._weaponName];
        }
        public int GetWeaponCount(Weapon weapon)
        {
            WeaponStaff.TryGetValue(weapon._weaponName, out int count);
            return count;
        }
        public Weapon GetNextWeapon()
        {
            var last = queueWeapons.Dequeue();

            var nextWeapon = queueWeapons.Peek();
            queueWeapons.Enqueue(last);
            return nextWeapon;
        }

    }
    
}

