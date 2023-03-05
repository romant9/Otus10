using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public abstract class AnimationManager : MonoBehaviour
{
    protected static int p_speed = Animator.StringToHash("Speed");
    protected static int p_hit = Animator.StringToHash("IsHit");
    protected static int p_hitting = Animator.StringToHash("IsHitting");
    protected static int p_die = Animator.StringToHash("IsDie");

    public Animator _animator;
    public float _speedAnim; //�������� ��������
    protected float _speedMult; //��������� �������� ��� ������ Shift
    protected float _acceleration; //��������� 0..1
    public float _motionSpeed = 1; //�������� �������� � ��������
    public float transitionTimeUp; //����� ���������
    public float transitionTimeDown; //����� ����������

    public KeyCode _turboKey;
    public KeyCode _moveKey;
    public KeyCode _rotateLeftKey;
    public KeyCode _rotateRightKey;
    public KeyCode _shootKey;
    public KeyCode _hitKey;

    public Collider selfWeapon; //���� ������ ������, ������� ����������� �� OnTriggerXXX
    public Collider currentWeapon { get; private set; } //������� ������ �����, ������������ � OnTriggerXXX
    //ToDo: �� �������, ��� ��������� ���������� DamageEvent �� �������� � ������ �����, ������� ��������� ����
    public AnimationManager enemy; //����

    public Slider hpSlider; //������� ��������
    public float MaxHealth = 10; //�������� ������
    public float damageValue = 1; //�������� ����� �� ������

    protected bool oneShootDamage; //������� ��� ��������� ���������� ����� �� �������� ����� OnTriggerXXX
    bool lastShoot = true; //����������� �������

    private float _health;


    //������, ��� ������� �� AnyState � ������
    public static bool ExtTriggerCondition(Collider col, bool isPolice)
    {
        bool condition = isPolice ? !col.name.ToLower().Contains("bullet") : col.name.ToLower().Contains("bullet");
        return condition;
    }


    public void Start()
    {
        _health = MaxHealth;
    }

    public virtual void OnTriggerStay(Collider otherWeapon)
    {
        if (otherWeapon != selfWeapon && otherWeapon.tag == "Weapon")
        {
            if (ExtTriggerCondition(otherWeapon, name == "Police"))
            {
                //�� �������, ��� ��������� ���������� ������� �� ��������� ������������
                if (oneShootDamage != lastShoot)
                {
                    oneShootDamage = lastShoot;
                    ShootRecieve();
                }
            }
            currentWeapon = otherWeapon;

        }
    }
    public virtual void OnTriggerExit(Collider otherWeapon)
    {
        currentWeapon = null;
        oneShootDamage = !lastShoot;
    }
    public void SetHealth(AnimationManager enemy)
    {
        enemy._health -= damageValue;
        enemy.hpSlider.value = MaxHealth - _health;
        if (enemy._health <= 0) enemy._animator.SetBool(p_die, true);
    }

    void Update()
    {

        _speedMult = Input.GetKey(_turboKey) ? 2 : 1;

        Move();
        Rotate();
        Shoot();
        Hit();

        _animator.SetFloat(p_speed, _speedAnim * _speedMult * _acceleration);

    }
    public virtual void Shoot()
    {
    }
    public virtual void ShootRecieve()
    {
    }
    public virtual void Hit()
    {
        if (Input.GetKeyDown(_hitKey))
        {
            _animator.SetTrigger(p_hit);
        }
        _animator.SetBool(p_hitting, Input.GetKey(_hitKey));

        _animator.transform.localRotation = Quaternion.identity; //������, ������ �������� ���� ��� ����������� � Idle

    }

    public void Move()
    {
        if (Input.GetKeyDown(_moveKey))
        {
            StopAllCoroutines();
            StartCoroutine(SetAccelerate(true));
        }
        if (Input.GetKeyUp(_moveKey))
        {
            StopAllCoroutines();
            StartCoroutine(SetAccelerate(false));
        }

        transform.position += transform.forward * _motionSpeed * _speedMult * _acceleration * Time.deltaTime;
    }
    public void Rotate()
    {
        if (Input.GetKey(_rotateLeftKey))
        {
            transform.Rotate(transform.up, -100 * _motionSpeed * _speedMult * Time.deltaTime);
        }
        if (Input.GetKey(_rotateRightKey))
        {
            transform.Rotate(transform.up, 100 * _motionSpeed * _speedMult * Time.deltaTime);
        }
    }
    protected IEnumerator SetAccelerate(bool increase)
    {
        float time = 0;
        float start = _acceleration;
        float end = increase ? 1 : 0;
        while (increase ? _acceleration < 1 : _acceleration > 0)
        {
            time += Time.deltaTime;
            _acceleration = Mathf.Lerp(start, end, time / (increase ? transitionTimeUp : transitionTimeDown));
            yield return null;
        }
        _acceleration = end;
    }

}
