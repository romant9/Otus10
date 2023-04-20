using System.Collections;
using UnityEngine;
using static Bloodymary.Game.WeaponManager;

namespace Bloodymary.Game
{
    public class Bullet : Weapon
    {
        private bool isDestroy;
        public Weapon Owner { get; protected set; } //gun

        protected override void Start()
        {
            _weaponSpeed = Owner._weaponSpeed;
            _weaponDamage = Owner._weaponDamage;
            _weaponType = WeaponType.Fire;
            WeaponOwner = Owner.WeaponOwner;

            StartCoroutine(InitBullet());
        }

        public void SetOwner(Weapon weapon)
        {
            Owner = weapon;
        }
        public IEnumerator BulletAnim()
        {
            transform.parent = null;
            Vector3 start = transform.position;
            Vector3 end = transform.position + transform.up * 10;
            float time = 0;
            float distance;

            do
            {
                if (isDestroy) yield break;

                distance = Vector3.Distance(transform.position, end);
                time += Time.deltaTime;
                transform.position = Vector3.LerpUnclamped(start, end, time * base._weaponSpeed / 4);
                yield return null;

            }
            while (distance > .1f);
            DestroyBullet();
        }
        public IEnumerator InitBullet()
        {
            float angle;
            do
            {
                angle = Vector3.Angle(Vector3.up, -transform.up);
                yield return null;
            }
            while (angle < 90);

            transform.parent = null;

            Owner.UI_Effects(WeaponType.Fire);
            StartCoroutine(BulletAnim());
        }

        public void DestroyBullet()
        {
            isDestroy = true;
            Destroy(gameObject);
        }

    }
}

