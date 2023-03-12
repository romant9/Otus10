using System.Collections;
using UnityEngine;
using static GameManager;

public class PolicemanController : CharacterController
{
    protected static int p_shoot = Animator.StringToHash("IsShoot");
    protected static int p_shooting = Animator.StringToHash("IsShooting");

    protected static string clipShootName = "m_pistol_shoot";

    public override void CharacterInit()
    {
        _characterType = CharacterType._Policeman;
        SetAI(this._characterType);
    }

    public override void Shoot()
    {
        if (Input.GetKeyDown(_shootKey))
        {
            StartCoroutine(ShootBullet());
        }
        _animator.SetBool(p_shooting, Input.GetKey(_shootKey));
    }

    private IEnumerator ShootBullet()
    {
        InitShoot();

        _animator.SetTrigger(p_shoot);

        yield return new WaitUntil(() => _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == clipShootName);
        if (Input.GetKey(_shootKey))
        {
            while (Input.GetKey(_shootKey))
            {
                yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length / 2);
                InitShoot();
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);            
        }
    }
    private void InitShoot()
    {
        Transform bullet = Instantiate(_GM._bullet.gameObject, selfWeapon.transform).transform;
        bullet.RotateAround(bullet.localPosition, bullet.right, 90);
        bullet.localPosition = new Vector3(-.12f, 0, .25f);

        var bulletFly = bullet.GetComponent<BulletFly>();
        StartCoroutine(bulletFly.BulletInit(_GM._bullet._weaponDamage));
    }
}
