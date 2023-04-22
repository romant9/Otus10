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
        private bool accelerate;        

        private Coroutine _MoveCoroutine; //альтернативный метод движени€ - карутин через Update

        private NavMeshAgent _NavMeshAgent;

        public float stopDistance = 1.2f;
        
        
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
        }

        public void Execute(bool aiOn)
        {
            if (aiOn && _CurrentCharacterController.isAlive && _CurrentCharacterController.isAI)
            {
                //_CurrentCharacterController.StopAttack();
                _CurrentCharacterController._animator.SetBool(GameDataHelper.p_hitting, false);
                _TargetCharacterController = GameManager.GManager.Player.GetComponent<CharacterController>();

                isActive = true;
                //StartCoroutine(AIRotate());
                _NavMeshAgent.isStopped = false;
                StartCoroutine(AIMoveNavAgent());
            }
            else
            {
                accelerate = false;
                isActive = false;
                //StartCoroutine(_CurrentCharacterController.SetAccelerate(accelerate));
                _CurrentCharacterController._animator.SetFloat(GameDataHelper.p_speed, 0);
                _NavMeshAgent.isStopped = true;
                StopAllCoroutines();
            }
        }

        private IEnumerator AIMoveNavAgent()
        {
            while (isActive)
            {
                distance = Vector3.Distance(transform.position, _TargetCharacterController.transform.position);
                _NavMeshAgent.destination = _TargetCharacterController.transform.position;
                _CurrentCharacterController._animator.SetFloat(GameDataHelper.p_speed, _CurrentCharacterController._speedAnim 
                                                                * _NavMeshAgent.velocity.normalized.magnitude);

                if (distance < Random.Range(-.5f,.5f) + stopDistance)
                { 
                    yield return new WaitForSeconds(GameSettings.GSettings.timeWaitHit);
                    if (_TargetCharacterController.isAlive) StartCoroutine(AIHit());
                }
                yield return null;
            }
        }
        
        private IEnumerator AIHit()
        {
            _CurrentCharacterController.SetAttack(false);

            _CurrentCharacterController._animator.SetTrigger(GameDataHelper.p_hit);
            yield return new WaitUntil(() => _CurrentCharacterController._animator.GetCurrentAnimatorStateInfo(0).IsName(GameDataHelper.clipHitName));

            _CurrentCharacterController.SetAttack(true);

            yield return new WaitForSeconds(_CurrentCharacterController._animator.GetCurrentAnimatorStateInfo(0).length);

            if (_CurrentCharacterController.setAttack)
            {
                _CurrentCharacterController.SetAttack(false);
                yield break;
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }


        //не используетс€
        //private void OnTriggerStay(Collider collision)
        //{
        //    if (isActive && collision.gameObject.CompareTag("Player"))
        //    {
        //        _NavMeshAgent.speed = 0;
        //    }

        //}

        //private void OnTriggerExit(Collider collision)
        //{
        //    if (isActive && collision.gameObject.CompareTag("Player"))
        //    {
        //        _NavMeshAgent.speed = _CurrentCharacterController._speedMotion;
        //    }
        //}
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

                    //_NavMeshAgent.destination = _TargetCharacterController.transform.position;
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

    public static class AiUtils
    {
        public enum CollisionIndicatorType
        {
            Left,
            Right,
            Forward
        }
    }
}

