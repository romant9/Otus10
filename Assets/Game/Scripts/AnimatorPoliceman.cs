using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorPoliceman : AnimationManager
{
    protected static int p_shoot = Animator.StringToHash("IsShoot");
    protected static int p_shooting = Animator.StringToHash("IsShooting");
    public GameObject _bullet;
    public float bulletSpeed = 1.0f;

    public override void Shoot()
    {
        if (Input.GetKeyDown(_shootKey))
        {
            _animator.SetTrigger(p_shoot);
            ShootBullet(); //ToDo: Ќе успел сделать вариант дл€ очереди выстрелов
        }
        _animator.SetBool(p_shooting, Input.GetKey(_shootKey));
    }

    private void ShootBullet()
    {
        Transform bullet = Instantiate(_bullet, selfWeapon.transform).transform;
        bullet.RotateAround(bullet.localPosition, bullet.right, 90);
        bullet.localPosition = new Vector3(-.12f, 0, .25f);

        var bulletFly = bullet.GetComponent<BulletFly>();
        StartCoroutine(bulletFly.BulletInit(bulletSpeed));
    }
}
