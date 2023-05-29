using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Bloodymary.Game
{
    public class AiController : MonoBehaviour
    {
        private CharacterController _CurrentCharacterController;
        private CharacterController _TargetCharacterController; //»грок, €вл€ющийс€ целью дл€ AI
        public bool isActive;

        private float distance;
        private Quaternion direction;
        private bool accelerate;        

        private Coroutine _MoveCoroutine; //альтернативный метод движени€ - карутин через Update

        private NavMeshAgent _NavMeshAgent;

        public float stopDistance = 1.2f;
        Coroutine corRot;
        Coroutine corHit;

        void Start()
        {
            _CurrentCharacterController = GetComponent<CharacterController>();
            _NavMeshAgent = GetComponent<NavMeshAgent>();
            if (_NavMeshAgent)
            {
                _NavMeshAgent.speed = _CurrentCharacterController._speedMotion;
                _NavMeshAgent.stoppingDistance = stopDistance;

            }
        }

        void Update()
        {
            if (!isActive ) return;

            distance = Vector3.Distance(transform.position, _TargetCharacterController.transform.position);
            direction = Quaternion.LookRotation((_TargetCharacterController.transform.position - transform.position).normalized);

            _NavMeshAgent.speed = distance > stopDistance ? _CurrentCharacterController._speedMotion : 0;
            _NavMeshAgent.radius = distance > stopDistance ? .4f : .2f;

        }

        private IEnumerator RotateTo()
        {
            while (true)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, direction, Time.deltaTime * 2);
                yield return null;
                corRot = null;
            }
        }

        public void Execute(bool aiOn)
        {
            if (aiOn && _CurrentCharacterController.isAlive && _CurrentCharacterController.isAI)
            {
                _CurrentCharacterController._animator.SetBool(GameDataHelper.p_hitting, false);
                _TargetCharacterController = GameManager.GManager.Player.GetComponent<CharacterController>();

                isActive = true;

                _NavMeshAgent.isStopped = false;
                StartCoroutine(AIMoveNavAgent());
            }
            else
            {
                accelerate = false;

                isActive = false;

                _CurrentCharacterController._animator.SetFloat(GameDataHelper.p_speed, 0);
                if (_NavMeshAgent.enabled)
                    _NavMeshAgent.isStopped = true;
                StopAllCoroutines();
            }
        }

        private IEnumerator AIMoveNavAgent()
        {
            float rand = Random.Range(-.3f, .3f);
            distance = Vector3.Distance(transform.position, _TargetCharacterController.transform.position);

            while (isActive)
            {                
                _CurrentCharacterController._animator.SetFloat(GameDataHelper.p_speed, _CurrentCharacterController._speedAnim
                                                                * _NavMeshAgent.velocity.normalized.magnitude);

                if (_TargetCharacterController.isAlive)
                    _NavMeshAgent.destination = _TargetCharacterController.transform.position;
                else
                {
                    _NavMeshAgent.enabled = false;
                    //_NavMeshAgent.destination = Vector3.zero;

                }
                 

                if (distance < stopDistance)
                {
                    _CurrentCharacterController._animator.SetFloat(GameDataHelper.p_speed,0);
                    if (corRot == null) { corRot = StartCoroutine(RotateTo()); }
                    yield return new WaitForSeconds(GameSettings.GSettings.timeWaitHit);
                    if (_TargetCharacterController.isAlive && corHit == null) { corHit = StartCoroutine(AIHit()); }
                }
                yield return null;
            }
        }
        
        private IEnumerator AIHit()
        {
            _CurrentCharacterController.SetAttack(false);

            _CurrentCharacterController._animator.SetTrigger(GameDataHelper.p_hit);
            yield return new WaitUntil(() => _CurrentCharacterController._animator.GetCurrentAnimatorStateInfo(0).IsName(GameDataHelper.clipHitName));
            if (!_CurrentCharacterController.isAttack) _CurrentCharacterController.SetAttack(true);
            yield return new WaitUntil(() => !_CurrentCharacterController._animator.GetCurrentAnimatorStateInfo(0).IsName(GameDataHelper.clipHitName));
            _CurrentCharacterController.SetAttack(false);
            corHit = null;
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        #region temp
        private IEnumerator AIRotate()
        {
            while (isActive)
            {
                transform.LookAt(_TargetCharacterController.transform.position, Vector3.up);

                yield return null;
            }
        }

        private IEnumerator AIMoveTo()
        {
            while (isActive)
            {
                distance = Vector3.Distance(transform.position, _TargetCharacterController.transform.position);
                float accelerateValue = _CurrentCharacterController._acceleration;
                _CurrentCharacterController._animator.SetFloat(GameDataHelper.p_speed, _CurrentCharacterController._speedAnim * accelerateValue);

                if (distance > 1.2f)
                {
                    if (!accelerate)
                    {
                        accelerate = true;
                        StartCoroutine(_CurrentCharacterController.SetAccelerate(accelerate));
                    }
                }
                else
                {
                    if (accelerate)
                    {
                        accelerate = false;
                        StartCoroutine(_CurrentCharacterController.SetAccelerate(accelerate));
                    }
                }

                if (accelerateValue > 0)
                {
                    transform.position += transform.forward * _CurrentCharacterController._speedMotion * .8f * _CurrentCharacterController._acceleration * Time.deltaTime;
                }
                else
                {
                    yield return new WaitForSeconds(GameSettings.GSettings.timeWaitHit);
                    if (_TargetCharacterController.isAlive && distance <= 1) StartCoroutine(AIHit());
                }
                yield return null;
            }
        }

        private IEnumerator MoveToAlt()
        {
            while (_CurrentCharacterController.isAlive)
            {
                transform.position += transform.forward * _TargetCharacterController._speedMotion * Time.deltaTime;
                yield return null;
            }
            _MoveCoroutine = null;
        }

        private void MoveToUpdate()
        {
            if (distance > 2)
            {
                if (_MoveCoroutine == null)
                    _MoveCoroutine = StartCoroutine(MoveToAlt());
            }
            else
            {
                if (_MoveCoroutine != null)
                {
                    StopCoroutine(_MoveCoroutine);
                    _MoveCoroutine = null;
                }
            }
        }
        #endregion
    }

}

