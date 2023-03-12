using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static GameManager;
using static WeaponInfo;

public abstract class CharacterController : MonoBehaviour
{
    protected GameManager _GM { get { return GameManager.GM; }}
    public static int p_speed = Animator.StringToHash("Speed");
    public static int p_hit = Animator.StringToHash("IsHit");
    public static int p_hitting = Animator.StringToHash("IsHitting");
    public static int p_die = Animator.StringToHash("IsDie");

    public static string clipHitName = "m_melee_combat_attack_A";

    public Animator _animator;
    protected CharacterType _characterType;
    public float _speedAnim; //скорость анимации
    protected float _speedMult; //множитель скорости для режима Shift
    public float _acceleration { get; private set; } //ускорение 0..1
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
    private WeaponInfo enemyWeaponInfo;
    public Slider hpSlider; //Слайдер здоровья
    public float MaxHealth = 10; //Здоровье игрока

    private float _health;
    private Text _healthIndicator;
    public Action<Collider> RecieveDamageAction; //в принципе можно обойтись без Action
    
    public bool setAttack { get; private set; } //true, когда срабатывает анимация атаки
    public bool isAlive { get; private set; }

    protected AiController AI;

    public static bool ExtTriggerCondition(WeaponType weaponType, bool isPolice, out bool isFire)
    {
        isFire = weaponType == WeaponType.Fire;
        bool condition = isFire ? (isPolice ? false : true) : true;
        return condition;
    }

    public void Start()
    {
        _health = MaxHealth;
        RecieveDamageAction += OnDamageRecieve;
        hpSlider.maxValue = MaxHealth;
        _healthIndicator = hpSlider.transform.Find("HP").GetComponent<Text>();
        _healthIndicator.text = MaxHealth.ToString();
        isAlive = true;
        CharacterInit();
    }
    public virtual void CharacterInit()
    {
    }
    private IEnumerator ExecuteNextFrame()
    {
        yield return new WaitForEndOfFrame();
        AI.Execute(_GM.AIOn);
    }
    protected void SetAI(CharacterType _characterType)
    {
        this.AI = null;
        _acceleration = 0;

        if (_characterType != GM.MyCharacterIs)
        {
            this.AI = GetComponent<AiController>();
            StartCoroutine(ExecuteNextFrame());

        }
        else
        {
            GM.enemyTarget = this.transform;
        }
    }
    public virtual void OnTriggerEnter(Collider otherWeapon)
    {
        if (otherWeapon != selfWeapon && otherWeapon.tag == "Weapon")
        {
            enemyWeaponInfo = otherWeapon.GetComponent<WeaponInfo>();
            bool condition = ExtTriggerCondition(enemyWeaponInfo._weaponType, _characterType == CharacterType._Policeman, out bool isFire);
            if (condition) //если пуля, то на полицейского не действует, а для хулигана всегда true
            {
                if (isAlive)
                {
                    if (isFire)
                    {
                        RecieveDamageAction.Invoke(otherWeapon); //fire damage
                    }
                    else
                    {
                        if (enemyWeaponInfo.WeaponOwner.setAttack)
                        {
                            enemyWeaponInfo.WeaponOwner.setAttack = false;
                            RecieveDamageAction.Invoke(otherWeapon); //melee damage
                        }
                    }
                }                                    
            }           
        }
    }
    public virtual void OnTriggerExit(Collider otherWeapon)
    {
        if (enemyWeaponInfo)
        {
            enemyWeaponInfo = null;
        }
    }
    void OnDamageRecieve(Collider weapon)
    {
        WeaponInfo weaponInfo = weapon.GetComponent<WeaponInfo>();
        if (weaponInfo != null)
        {
            Debug.Log($"{weaponInfo.WeaponOwner.gameObject.name} damage To {name} by {weaponInfo._weaponName}");
            SetHealth(weaponInfo._weaponDamage);
        }
    }
    
    public void SetHealth(float damageValue)
    {
        _health -= damageValue;
        hpSlider.value = MaxHealth - _health;
        _healthIndicator.text = _health.ToString();
        if (_health <= 0)
        {
            _animator.SetBool(p_die, true);
            isAlive = false;
            Debug.Log(name + "  is die");
        }
    }

    void Update()
    {
        if (AI == null || !AI.isActive)
        {
            _speedMult = Input.GetKey(_turboKey) ? 2 : 1;

            Move();
            Rotate();
            Shoot();
            Hit();

            _animator.SetFloat(p_speed, _speedAnim * _speedMult * _acceleration);
        }
    }
    public virtual void Shoot()
    {
    }

    public virtual void Hit()
    {
        if (Input.GetKeyDown(_hitKey))
        {
            setAttack = false;

            StartCoroutine(_Hit());
        }       
        _animator.SetBool(p_hitting, Input.GetKey(_hitKey));

        _animator.transform.localEulerAngles = Vector3.zero; //Вопрос, почему меняется угол при возвращении к Idle

    }
    private IEnumerator _Hit()
    {
        _animator.SetTrigger(p_hit);
        //yield return new WaitUntil(() => _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == clipHitName);
        yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).IsName(clipHitName));

        setAttack = true;
        if (Input.GetKey(_hitKey))
        {
            while (Input.GetKey(_hitKey))
            {
                setAttack = true;

                yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length / 2);
                if (setAttack)
                {
                    setAttack = false;
                }
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
            if (setAttack)
            {
                setAttack = false;
                yield break;
            }
        }
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
    public IEnumerator SetAccelerate(bool increase)
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
    public void SetAttack(bool attack)
    {
        setAttack = attack;
    }
}
