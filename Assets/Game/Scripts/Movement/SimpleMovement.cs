using System.Collections;
using System.Linq;
using UnityEngine;
using static Bloodymary.Game.GameManager;


namespace Bloodymary.Game
{
    public class SimpleMovement : MonoBehaviour
    {
        private Rigidbody rb;
        private CharacterController character;
        private Animator _animator;
        private CapsuleCollider _collider;

        private KeyCode _moveForwardKey;
        private KeyCode _moveBackKey;
        private KeyCode _moveLeftKey;
        private KeyCode _moveRightKey;

        //acceleration
        private float accel_Forward;
        private float accel_Back;
        private float accel_Left;
        private float accel_Right;
        //

        public float _speedAnim = 1; //скорость анимации
        public float _speedMotion = 1; //скорость движения и вращения
        public float transitionTimeUp = .5f; //время ускорения
        public float transitionTimeDown = .5f; //время замедления

        public float _speedMult { get; private set; } //множитель скорости для режима Shift

        private bool isGrounded;
        public LayerMask whatIsGround;


        private float Acceleration()
        {
            var accelerationArray = new float[4] { accel_Forward, accel_Back, accel_Left, accel_Right };
            float maxValue = accelerationArray.Max();
            return maxValue;
        }

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            character = GetComponent<CharacterController>();
            _animator = character._animator;
            _collider = GetComponent<CapsuleCollider>();
            StartCoroutine(CorWaitInit());
        }
        private IEnumerator CorWaitInit()
        {
            yield return new WaitForEndOfFrame();

            _moveForwardKey = character._currentControlTheme._moveKey;
            _moveBackKey = character._currentControlTheme._moveBackKey;
            _moveLeftKey = character._currentControlTheme._rotateLeftKey;
            _moveRightKey = character._currentControlTheme._rotateRightKey;
        }

        void Update()
        {
            if (!character.isReadyToMove) return;

            isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.3f, whatIsGround);
            MoveFree();

        }

        public void MoveFree()
        {
            _speedMult = character._speedMult;

            _animator.SetFloat(GameDataHelper.p_speed, isGrounded ? _speedAnim * _speedMult * Acceleration() : 0);

            if (Input.GetKey(_moveForwardKey))
            {
                accel_Forward = Mathf.Clamp(accel_Forward + Time.deltaTime * 1 / transitionTimeUp, 0f, 1f);
            }
            else accel_Forward = Mathf.Clamp(accel_Forward - Time.deltaTime * 1 / transitionTimeDown, 0f, 1f);

            if (Input.GetKey(_moveBackKey))
            {
                accel_Back = Mathf.Clamp(accel_Back + Time.deltaTime * 1 / transitionTimeUp, 0f, 1f);
            }
            else accel_Back = Mathf.Clamp(accel_Back - Time.deltaTime * 1 / transitionTimeDown, 0f, 1f);

            if (Input.GetKey(_moveLeftKey))
            {
                accel_Left = Mathf.Clamp(accel_Left + Time.deltaTime * 1 / transitionTimeUp, 0f, 1f);
            }
            else accel_Left = Mathf.Clamp(accel_Left - Time.deltaTime * 1 / transitionTimeDown, 0f, 1f);

            if (Input.GetKey(_moveRightKey))
            {
                accel_Right = Mathf.Clamp(accel_Right + Time.deltaTime * 1 / transitionTimeUp, 0f, 1f);
            }
            else accel_Right = Mathf.Clamp(accel_Right - Time.deltaTime * 1 / transitionTimeDown, 0f, 1f);


            if (accel_Forward > 0) transform.position += transform.forward * _speedMotion * _speedMult * accel_Forward * Time.deltaTime;
            if (accel_Back > 0) transform.position -= transform.forward * _speedMotion * _speedMult * accel_Back * Time.deltaTime;
            if (accel_Left > 0) transform.position -= transform.right * _speedMotion * _speedMult * accel_Left * Time.deltaTime;
            if (accel_Right > 0) transform.position += transform.right * _speedMotion * _speedMult * accel_Right * Time.deltaTime;

            if (!Input.GetKey(KeyCode.C)) transform.rotation = Quaternion.LookRotation(MoveDirection());

        }

        public Vector3 MoveDirection()
        {
            Vector3 direction = GManager._camera.transform.forward;
            direction.y = 0;
            direction.Normalize();

            return direction;
        }

    }
}

