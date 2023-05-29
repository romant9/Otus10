using System.Collections;
using UnityEngine;
using static Bloodymary.Game.GameSettings;


namespace Bloodymary.Game
{
    public class PolicemanController : CharacterController
    {
        protected override void Start()
        {
            _characterType = CharacterType._Policeman;
            base.Start();
        }
        public override void CharacterInit()
        {
            _characterType = CharacterType._Policeman;
            _currentControlTheme = GSettings.LeftPlayerTheme;

            base.CharacterInit();
        }

        public override void Shoot(bool setAttack)
        {
            if (setAttack)
            {               
                StartCoroutine(ShootBullet());
            }
            _animator.SetBool(GameDataHelper.p_shooting, Input.GetKey(_currentControlTheme._attackKey));
        }

        private IEnumerator ShootBullet()
        {
            currentWeapon.InitShoot();
            _Inventory.ChangeWeaponCount(currentWeapon, -1);
            CurrentUIData.ammunitionCount.text = _Inventory.GetWeaponCount(currentWeapon).ToString();
            _animator.SetTrigger(GameDataHelper.p_shoot);

            yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).IsName(GameDataHelper.clipIdleName));
            
            while (Input.GetKey(_currentControlTheme._attackKey))
            {
                currentWeapon.InitShoot();
                _Inventory.ChangeWeaponCount(currentWeapon, -1);
                CurrentUIData.ammunitionCount.text = _Inventory.GetWeaponCount(currentWeapon).ToString();
                yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length / 2);
            }

            if (_Inventory.GetWeaponCount(currentWeapon) < 1) CheckAmmunition();
        }
    }
}

