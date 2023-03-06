using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;

public abstract class AnimationManager : MonoBehaviour
{
    protected static int p_speed = Animator.StringToHash("Speed");
    protected static int p_hit = Animator.StringToHash("IsHit");
    protected static int p_hitting = Animator.StringToHash("IsHitting");
    protected static int p_die = Animator.StringToHash("IsDie");

    public Animator _animator;
    public float _speedAnim; //скорость анимации
    protected float _speedMult; //множитель скорости для режима Shift
    protected float _acceleration; //ускорение 0..1
    public float _motionSpeed = 1; //скорость движения и вращения
    public float transitionTimeUp; //время ускорения
    public float transitionTimeDown; //время замедления

    public KeyCode _turboKey;
    public KeyCode _moveKey;
    public KeyCode _rotateLeftKey;
    public KeyCode _rotateRightKey;
    public KeyCode _shootKey;
    public KeyCode _hitKey;

    public Collider selfWeapon; //свое оружие игрока, которое исключается из OnTriggerXXX
    public Collider enemyWeapon { get; private set; } //текущее оружие врага, обнаруженное в OnTriggerXXX
    //ToDo: не понимаю, как нормально передавать DamageEvent от анимации к любому врагу, который принимает урон
    public AnimationManager enemy; //враг

    public Slider hpSlider; //Слайдер здоровья
    public float MaxHealth = 10; //Здоровье игрока
    public float damageValue = 1; //Величина урона по игроку

    protected bool oneShootDamage; //костыль для обработки одиночного урона от выстрела через OnTriggerXXX
    bool nextShoot = true; //продолжение костыля

    private float _health;
    public Action<Collider> RecieveDamageAction;
    [HideInInspector]
    public bool SetDamage; //true, когда срабатывает Event от Animator


    //Вопрос, как перейти из AnyState к смерти
    public static bool ExtTriggerCondition(WeaponInfo.WeaponType type, bool isPolice)
    {
        bool condition = type == WeaponInfo.WeaponType.Fire ? (isPolice ? false : true) : true;
        return condition;
    }

    public void Start()
    {
        _health = MaxHealth;
        RecieveDamageAction += OnDamageRecieve;
    }

    public virtual void OnTriggerStay(Collider otherWeapon)
    {
        if (otherWeapon != selfWeapon && otherWeapon.tag == "Weapon")
        {
            var type = otherWeapon.GetComponent<WeaponInfo>().type;
            if (ExtTriggerCondition(type, name == "Police")) //если пуля, то на полицейского не действует, а для хулигана всегда true
            {
                if (enemy.SetDamage)
                {                   
                    RecieveDamageAction.Invoke(otherWeapon); //melee damage
                }
                else
                {
                    //не понимаю, как поставить нормальный триггер на одиночное срабатывание
                    if (oneShootDamage != nextShoot && type == WeaponInfo.WeaponType.Fire)
                    {
                        RecieveDamageAction.Invoke(otherWeapon); //fire damage
                        oneShootDamage = nextShoot;
                    }
                }               
            }
            enemyWeapon = otherWeapon;
        }
    }
    
    void OnDamageRecieve(Collider weapon)
    {
        WeaponInfo weaponInfo = weapon.GetComponent<WeaponInfo>();
        if (weaponInfo != null)
        {
            Debug.Log($"{weaponInfo.WOwner.gameObject.name} damage To {name} by {weaponInfo.wName}");
            SetHealth(weaponInfo.wDamage);
        }
    }

    public virtual void OnTriggerExit(Collider otherWeapon)
    {
        enemyWeapon = null;
        oneShootDamage = !nextShoot;
    }
    public void SetHealth(float damageValue)
    {
        _health -= damageValue;
        hpSlider.value = MaxHealth - _health;
        if (_health <= 0)
        {
            _animator.SetBool(p_die, true);
            Debug.Log(name + "  is die");
        }
    }
    
    //public void SetHealth(AnimationManager enemy, float damageValue)
    //{
    //    enemy._health -= damageValue;
    //    enemy.hpSlider.value = enemy.MaxHealth - enemy._health;
    //    if (enemy._health <= 0) 
    //    {
    //        enemy._animator.SetBool(p_die, true);
    //        Debug.Log(enemy.name + "  is die");
    //    }
    //}

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

    public virtual void Hit()
    {
        if (Input.GetKeyDown(_hitKey))
        {
            _animator.SetTrigger(p_hit);
        }
        _animator.SetBool(p_hitting, Input.GetKey(_hitKey));

        _animator.transform.localEulerAngles = Vector3.zero; //Вопрос, почему меняется угол при возвращении к Idle

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
