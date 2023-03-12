using System.Collections;
using UnityEngine;

public class AiController : MonoBehaviour
{
    private GameManager _GM { get { return GameManager.GM; } }
    private CharacterController _CurrentCharacterController;
    private CharacterController _TargetCharacterController; //»грок, €вл€ющийс€ целью дл€ AI
    public bool isActive; //включаетс€ однократно после инициализации _TargetCharacterController

    private float distance;
    private bool accelerate;

    private Coroutine _MoveCoroutine; //альтернативный метод движени€ - карутин через Update

    void Start()
    {
        _CurrentCharacterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!isActive || !_CurrentCharacterController.isAlive) 
        { 
            StopAllCoroutines();
            return;
        }

        distance = Vector3.Distance(transform.position, _TargetCharacterController.transform.position);

        //MoveToUpdate();
    }
    

    public void Execute(bool aiOn)
    {
        if (aiOn)
        {
            _TargetCharacterController = _GM.enemyTarget.GetComponent<CharacterController>();

            isActive = true;
            StartCoroutine(AIRotate());
            StartCoroutine(AIMoveTo());
        }
        else
        {
            accelerate = false;
            isActive = false;
            StopAllCoroutines();
        }
        
        //_MoveCoroutine = StartCoroutine(MoveToAlt());
    }
    private IEnumerator AIRotate()
    {
        while (_CurrentCharacterController.isAlive)
        {
            transform.LookAt(_GM.enemyTarget,Vector3.up);
            yield return null;
        }
    }  

    private IEnumerator AIMoveTo()
    {
        while (_CurrentCharacterController.isAlive)
        {
            distance = Vector3.Distance(transform.position, _TargetCharacterController.transform.position);
            float accelerateValue = _CurrentCharacterController._acceleration;

            _CurrentCharacterController._animator.SetFloat(CharacterController.p_speed, _CurrentCharacterController._speedAnim * accelerateValue);

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
                transform.position += transform.forward * _CurrentCharacterController._motionSpeed * .8f * _CurrentCharacterController._acceleration * Time.deltaTime;
            }
            else
            {
                yield return new WaitForSeconds(_GM.timeWaitHit);
                if (_TargetCharacterController.isAlive && distance <= 1) StartCoroutine(AIHit());
            }
            yield return null;
        }
    }

    private IEnumerator AIHit()
    {
        _CurrentCharacterController.SetAttack(false);
        
        _CurrentCharacterController._animator.SetTrigger(CharacterController.p_hit);
        yield return new WaitUntil(() => _CurrentCharacterController._animator.GetCurrentAnimatorStateInfo(0).IsName(CharacterController.clipHitName));

        _CurrentCharacterController.SetAttack(true);

        yield return new WaitForSeconds(_CurrentCharacterController._animator.GetCurrentAnimatorStateInfo(0).length);

        if (_CurrentCharacterController.setAttack)
        {
            _CurrentCharacterController.SetAttack(false);
            yield break;
        }
    }

    //не используетс€
    #region temp
    private IEnumerator MoveToAlt()
    {
        while (_CurrentCharacterController.isAlive)
        {
            transform.position += transform.forward * _TargetCharacterController._motionSpeed * Time.deltaTime;
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
