using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using static Bloodymary.Game.GameManager;
using static Bloodymary.Game.GameSettings;
using static Bloodymary.Game.WeaponManager;

namespace Bloodymary.Game
{
    public abstract class CharacterController : MonoBehaviour
    {
        public Animator _animator;
        public СharacterUIData _PlayerUIData; //локальный UI над персонажем
        public СharacterUIData CurrentUIData { get; private set; } //текущий UI - либо локальный для AI, либо глобальный в Screen Space
        public CharacterController character { get; private set; }
        public CharacterType _characterType;// { get; set; }

        public ControllerTheme _currentControlTheme { get; protected set; }

        public float _speedAnim = 1; //скорость анимации
        public float _speedMotion = 1; //скорость движения и вращения
        public float transitionTimeUp = .5f; //время ускорения
        public float transitionTimeDown = .5f; //время замедления

        public float _speedMult { get; private set; } //множитель скорости для режима Shift

        //acceleration
        public float _acceleration { get; private set; } //ускорение 0..1
        private float accel_Forward;
        private float accel_Back;
        private float accel_Left;
        private float accel_Right;
        float savedAcceleration;
        //

        public Inventory _Inventory;

        protected Collider selfWeaponCollider; //свое оружие игрока, которое исключается из OnTriggerXXX
        protected NavMeshAgent _agent;
        protected Rigidbody _rigidbody;
        protected PhysicalJump _physicalJump;
        protected SimpleMovement _simpleMovement;
        //protected bool isJump;
        public bool isReadyToMove { get; private set; }

        protected Weapon currentWeapon;
        private Weapon enemyWeapon;
        public Transform _weaponPivot;

        public Health health;

        public bool isAttack { get; private set; } //true, когда срабатывает анимация атаки
        public bool isAlive { get; private set; }
        public bool isAI { get; private set; }

        private bool isGrounded;
        public LayerMask whatIsGround;

        public Action GetGrenadeDamage;
        private float throwMult = 1;

        private bool Initialized;

        private int damageCount; //damage recieve

        private EffectRouter _effects;


        private float Acceleration()
        {
            var accelerationArray = new float[4] { accel_Forward , accel_Back, accel_Left, accel_Right };
            float maxValue = accelerationArray.Max();
            return maxValue;
        }

        public static bool ExtTriggerCondition(WeaponType weaponType, bool isPlayer, out bool isFire)
        {
            isFire = weaponType == WeaponType.Fire;
            bool condition = isFire ? (isPlayer ? false : true) : true;
            return condition;
        }

        protected virtual void Start()
        {
            character = this;

            isAlive = true;

            _agent = GetComponent<NavMeshAgent>();
            _rigidbody = GetComponent<Rigidbody>();
            _effects = GetComponent<EffectRouter>();
            _physicalJump = GetComponent<PhysicalJump>();
            _simpleMovement = GetComponent<SimpleMovement>();

            _Inventory.Initialize();
            currentWeapon = WManager.SetupWeapon(_Inventory.weapons[0], _weaponPivot);

            GetGrenadeDamage += OnDamageRecieveGrenade;
            health.OnDeath += Death;

            GManager.Characters.Add(character);
            CharacterPreInit();
        }

        public void CharacterPreInit()
        {
            CharacterInit();
            InitUIData();
            health.MaxHealth = GSettings.health;
            health.InitHealth(CurrentUIData.HealthSlider);
            Initialized = true;
        }

        public void InitUIData()
        {
            _PlayerUIData.gameObject.SetActive(false);

            if (!GManager.AIOn)
            {
                CurrentUIData = _characterType == CharacterType._Policeman ? GManager._PolicemanUI : GManager._HooliganUI;
                CurrentUIData.gameObject.SetActive(true);
            }
            else
            {
                CurrentUIData = transform == GManager.Player ?
                    _characterType == CharacterType._Policeman ?
                    GManager._PolicemanUI : GManager._HooliganUI : _PlayerUIData;

                CurrentUIData.gameObject.SetActive(true);

                if (GManager.MyCharacterIs == CharacterType._Policeman)
                    GManager._HooliganUI.gameObject.SetActive(false);
                else
                    GManager._PolicemanUI.gameObject.SetActive(false);
            }

            if (!isAI)
            {
                GManager.SetPlayerUIData(CurrentUIData);

            }
        }
        public virtual void CharacterInit() //сброс и назначение ролей персонажам. Игрок или AI
        {
            _acceleration = 0;
            isAI = false;

            _agent.enabled = false;
            _rigidbody.isKinematic = false;

            if (GManager.AIOn)
            {
                if (_characterType == GManager.MyCharacterIs)
                {
                    _currentControlTheme = GSettings.GetControlTheme(false);
                    GManager.SetPlayer(this.transform);
                }
                else
                {
                    isAI = true;
                    _agent.enabled = true;
                    _rigidbody.isKinematic = true;
                }
            }
            else
            {
                if (_characterType == GManager.MyCharacterIs)
                {
                    _currentControlTheme = GSettings.GetControlTheme(false);
                    GManager.SetPlayer(this.transform);
                }
                else
                {
                    _currentControlTheme = GSettings.GetControlTheme(true);                   
                }
            }
            StartCoroutine(ExecuteNextFrame());
        }
        private IEnumerator ExecuteNextFrame()
        {
            yield return new WaitForEndOfFrame(); //ждем инициализации GManager.Player
            GetComponent<AiController>().Execute(GManager.AIOn);
        }

        public virtual void OnTriggerEnter(Collider otherWeapon)
        {
            if (otherWeapon.CompareTag("Weapon"))
            {
                enemyWeapon = otherWeapon.GetComponent<Weapon>();
                if (enemyWeapon != currentWeapon && enemyWeapon.WeaponOwner != this)
                {
                    //bool condition = ExtTriggerCondition(enemyWeapon._weaponType, _characterType == CharacterType._Policeman, out bool isFire);
                    bool isFire = enemyWeapon._weaponType == WeaponType.Fire;
                    //сильно запутанно, но в целом: игрок принимает урон от ИИ, а ИИ только от игрока
                    bool condition = !GManager.AIOn || !(enemyWeapon.WeaponOwner.transform != GManager.Player && this.transform != GManager.Player);
                    if (condition)
                    {
                        if (isAlive)
                        {
                            if (isFire)
                            {
                                damageCount++;
                                OnDamageRecieve(otherWeapon); //fire damage
                                if (otherWeapon.GetComponent<Bullet>())
                                {
                                    otherWeapon.GetComponent<Bullet>().DestroyBullet();
                                }
                            }
                            else
                            {
                                if (enemyWeapon.WeaponOwner.isAttack)
                                {
                                    damageCount++;
                                    enemyWeapon.WeaponOwner.SetAttack(false);

                                    OnDamageRecieve(otherWeapon); //melee damage
                                }
                            }
                        }
                    }
                }
            }
        }
        public virtual void OnTriggerExit(Collider otherWeapon)
        {
            if (enemyWeapon)
            {
                enemyWeapon = null;
            }
        }
        void OnDamageRecieve(Collider wCollider)
        {
            Weapon weapon = wCollider.GetComponent<Weapon>();

            if (weapon != null)
            {
                IsDamage(weapon);

                Vector3 effectPos = GetComponent<Collider>().ClosestPoint(wCollider.transform.position);
                effectPos += transform.forward * .25f;
                Quaternion effectRot = Quaternion.LookRotation((weapon.WeaponOwner.transform.position - transform.position).normalized);

                _effects.ExecuteParticlesEffect(effectPos, effectRot);

            }
        }

        void OnDamageRecieveGrenade()
        {
            Weapon weapon = WManager.CurrentMassDamageWeapon;

            if (weapon != null)
            {
                _animator.SetTrigger(GameDataHelper.p_grenadeDamage);
                IsDamage(weapon);

            }
        }
        void IsDamage(Weapon weapon)
        {
            string content = $"{weapon.WeaponOwner.CurrentUIData.PlayerName} damage To {CurrentUIData.PlayerName} by {weapon._weaponName}";
            GManager.GameLog.ShowInfo(false, content);
            Debug.Log(content);

            if (weapon._weaponType == WeaponType.Melee)
            {
                StartCoroutine(PlaySoundEffects(weapon));
            }
            else
            {
                _effects.PlaySound("GetDamage");
            }

            SetHealth(weapon._weaponDamage);
        }
        IEnumerator PlaySoundEffects(Weapon weapon)
        {
            weapon._audioSource.Play();
            yield return new WaitForSeconds(.25f);
            _effects.PlaySound("GetDamage");
            yield return null;
        }
        public void SetHealth(float damageValue)
        {
            health.SetHealth(damageValue);
            
        }

        void Update()
        {
            //костыль с компенсацией поворота персонажа при переходе к idle 
            _animator.transform.localEulerAngles = Vector3.zero;

            if (!isAlive || isAI || !Initialized) 
            {
                isReadyToMove = false; 
                return;
            }
            else isReadyToMove = true;

            isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.3f, whatIsGround);
            _speedMult = Input.GetKey(_currentControlTheme._turboKey) ? 2 : 1;

            if ((int)GManager._camera.gameCameraType != 2 && GManager.Player == this.transform)
            {
                //MoveFree();
                _simpleMovement.enabled = true;
            }
            else
            {
                _simpleMovement.enabled = false;

                Move();
                Rotate();

                _animator.SetFloat(GameDataHelper.p_speed, isGrounded ? _speedAnim * _speedMult * _acceleration : 0);
                //_animator.SetFloat(GameDataHelper.p_speed, !_physicalJump.isJumping ? _speedAnim * _speedMult * _acceleration : 0);
            }

            SwitchWeapon();

            Attack(currentWeapon._weaponType);
        }

        public virtual void Attack(WeaponType weaponType)
        {
            bool setAttack;
            if (GManager.Player == this.transform)
            {
                bool isOverCanvas = EventSystem.current.IsPointerOverGameObject();
                setAttack = Input.GetKeyDown(_currentControlTheme._attackKey) || (Input.GetMouseButtonDown(0) && !isOverCanvas);
            }
            else
            {
                setAttack = Input.GetKeyDown(_currentControlTheme._attackKey);
            }

            switch (weaponType)
            {
                case WeaponType.Melee: { Hit(setAttack); break; }
                case WeaponType.Fire: { Shoot(setAttack); break; }
                case WeaponType.Throw: { Throw(setAttack); break; }
                    default: { return; }
            }
        }

        public virtual void Shoot(bool setAttack)
        {
        }

        public void SwitchWeapon()
        {
            if (Input.GetKeyDown(_currentControlTheme._switchWeapon))
            {                
                currentWeapon = _Inventory.GetNextWeapon();
                CheckAmmunition(); //после выбора оружия проверка на наличие боеприпасов
                string content = $"{CurrentUIData.PlayerName} выбрал {currentWeapon._weaponName}";
                GManager.GameLog.ShowInfo(false, content);
                Debug.Log(content);
            }  

        }

        public virtual void Hit(bool setAttack)
        {
            if (setAttack)
            {
                isAttack = false;

                StartCoroutine(CorHit());
            }
            _animator.SetBool(GameDataHelper.p_hitting, Input.GetKey(_currentControlTheme._attackKey));
        }
        public virtual void Throw(bool setAttack)
        {
            //пробел - throwCycle + бросок
            //G - замах, пробел - бросок
            //throwMult = 1;

            if (Input.GetKeyDown(_currentControlTheme._throwRange))
            {
                CurrentUIData.ThrowSlider.gameObject.SetActive(true);
                CurrentUIData.ThrowSlider.maxValue = 3;
                StartCoroutine(_ThrowReady());
            }

            if (setAttack)
            {              
                _Inventory.ChangeWeaponCount(currentWeapon, -1);
                CurrentUIData.grenadeCount.text = _Inventory.GetWeaponCount(currentWeapon).ToString();
                StartCoroutine(IThrowCycle());                
            }           
        }
        private IEnumerator _ThrowReady()
        {
            float t = 0;
            throwMult = 1;

            while (Input.GetKey(_currentControlTheme._throwRange))
            {
                t += Time.deltaTime;
                throwMult = Mathf.PingPong(t * 3, 3);
                CurrentUIData.ThrowSlider.value = throwMult;
                _animator.SetBool(GameDataHelper.p_throwReady, true);
                yield return null;

            }
            CurrentUIData.ThrowSlider.gameObject.SetActive(false);
            _animator.SetBool(GameDataHelper.p_throwReady, false);

        }
        private IEnumerator IThrowCycle()
        {
            _animator.SetBool(GameDataHelper.p_throwCycle, true);
            if (!Input.GetKey(_currentControlTheme._throwRange))  _animator.SetTrigger(GameDataHelper.p_throw);
            yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).IsName(GameDataHelper.clipIdleName));
            currentWeapon.InitThrow(throwMult);
            yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).IsName(GameDataHelper.clipIdleName));
            _animator.SetBool(GameDataHelper.p_throwCycle, false);

            CheckAmmunition();
        }
        
        public void CheckAmmunition()
        {
            if (_Inventory.GetWeaponCount(currentWeapon) < 1)
            {
                currentWeapon = _Inventory.GetNextWeapon();
                CheckAmmunition();
            }
            currentWeapon = WManager.SetupWeaponByName(currentWeapon._weaponName, _weaponPivot);
        }
        private IEnumerator CorHit()
        {
            _animator.SetTrigger(GameDataHelper.p_hit);
            //yield return new WaitUntil(() => _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == clipHitName); 
            //один из вариантов
            yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).IsName(GameDataHelper.clipHitName));

            isAttack = true;
            
            while (Input.GetKey(_currentControlTheme._attackKey))
            {               
                isAttack = true;
                yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length / 2);
                isAttack = false;
            }
            yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
            
            isAttack = false;           
        }

        public void Move()
        {
            if (Input.GetKeyDown(_currentControlTheme._moveKey))
            {
                StopAllCoroutines();
                StartCoroutine(SetAccelerate(true));
            }
            if (Input.GetKeyUp(_currentControlTheme._moveKey))
            {
                StopAllCoroutines();
                StartCoroutine(SetAccelerate(false));
            }

            transform.position += transform.forward * _speedMotion * _speedMult * _acceleration * Time.deltaTime;
        }
       
        public void Rotate()
        {           
            if (Input.GetKey(_currentControlTheme._rotateLeftKey))
            {
                transform.Rotate(transform.up, -100 * _speedMotion * _speedMult * Time.deltaTime);
            }
            if (Input.GetKey(_currentControlTheme._rotateRightKey))
            {
                transform.Rotate(transform.up, 100 * _speedMotion * _speedMult * Time.deltaTime);
            }
        }
        private void Jump()
        {
            //if (Input.GetKeyDown(_currentControlTheme._jumpKey) && !isJump)
            //{
            //    StartCoroutine(CorJump());
            //}
        }

        private IEnumerator CorJump()
        {
            //var forwardForce = _acceleration > 0 ? transform.forward * 15 : Vector3.zero;
            //_rigidbody.AddForce((transform.up * 20 + forwardForce) * _agent.height * _speedMult, ForceMode.Impulse);
            //savedAcceleration = _acceleration;
            //_acceleration = 0;
            yield return new WaitForSeconds(.1f);
            //isJump = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            //if (collision.gameObject.layer == 6 && isJump)
            //{

            //    if (Input.GetKey(_currentControlTheme._moveKey)) _acceleration = savedAcceleration;
            //    isJump = false;
            //}
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
            isAttack = attack;
        }
        public void Death()
        {
            _animator.SetBool(GameDataHelper.p_die, true);
            _effects.PlaySound("Die");

            isAlive = false;
            if (isAI) GetComponent<AiController>().Execute(GManager.AIOn);
            else 
            {
                //GManager.AIOn = false;
                GManager.PreInit();
            }
            string content = CurrentUIData.PlayerName + " is died";
            GManager.GameLog.ShowInfo(false, content);
            Debug.Log(content);

            health.OnDeath -= Death;
            GetGrenadeDamage -= OnDamageRecieveGrenade;

            GManager.Characters.Remove(character);

            if (GManager.Player == transform) 
            { 
                GManager.SetPlayer(null);
                GManager.isPlayedDied = true;
            }          

            GManager.CheckVictory();

            StartCoroutine(DestroyCharacter());
        }

        private IEnumerator DestroyCharacter()
        {
            yield return new WaitForSeconds(5);
            Destroy(this.gameObject);
        }

        private void OnDestroy()
        {
            if (GManager.Characters.Contains(character))
                GManager.Characters.Remove(character);
        }


    }
}

