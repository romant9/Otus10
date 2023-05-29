using System.Collections;
using System.Linq;
using UnityEngine;

namespace Bloodymary.Game
{
    public class PhysicalJump : MonoBehaviour
    {
        private Rigidbody rb;
        private CharacterController character;

        public float pushTime = 0.5f;
        //public float jumpHeight = 5;
        public float jumpForce = 50;
        public float downForce = 50;

        float jumpTime;
        public bool isJumping;// { get; private set; }
        bool jumpCancelled;
        bool isIteraction;
        Vector3 attractDirection;
        Collision currentCollision;
        bool isCollided;

        private KeyCode jumpKey;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            character = GetComponent<CharacterController>();
            StartCoroutine(CorWaitInit());
        }

        IEnumerator CorWaitInit()
        {
            yield return new WaitForEndOfFrame();
            jumpKey = character._currentControlTheme._jumpKey;
        }

        private void Update()
        {
            if (!character.isReadyToMove) return;

            if (!isJumping && Input.GetKeyDown(jumpKey))
            {
                //Jump(new Vector3(0, jumpForce * character._speedMult, 0));
                Jump(new Vector3(rb.velocity.x, jumpForce * character._speedMult / 10, rb.velocity.z));
            }

            if (isJumping)
            {
                jumpTime += Time.deltaTime;
                if (Input.GetKeyUp(jumpKey) || jumpTime > pushTime)
                {
                    jumpCancelled = true;
                }
            }
        }

        private void Jump(Vector3 jumpDirection)
        {
            //jumpForce = Mathf.Sqrt(jumpHeight * -2 * (Physics.gravity.y * rb.drag));
            rb.velocity = jumpDirection;
            //rb.AddForce(jumpDirection, ForceMode.Impulse);
            isJumping = true;
            jumpCancelled = false;
            jumpTime = 0;
        }
        private void FixedUpdate()
        {
            if (character.isReadyToMove && jumpCancelled && rb.velocity.y > 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            }
        }
        private void OnCollisionStay(Collision collision)
        {
            Vector3 point = collision.contacts.FirstOrDefault().point;
            Vector3 direction = point - new Vector3(transform.position.x, point.y, transform.position.z);
            Debug.DrawLine(new Vector3(transform.position.x, point.y, transform.position.z), point, Color.blue);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == 6)
            {
                currentCollision = collision;
                EnterCollision(collision);
            }
        }
        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.layer == 6 && collision != currentCollision)
            {
                isCollided = false;
                StopAllCoroutines();
            }
        }

        void EnterCollision(Collision collision)
        {
            Vector3 point = collision.contacts.FirstOrDefault().point;
            Vector3 direction = point - new Vector3(transform.position.x, point.y, transform.position.z);
            float dirLength = direction.magnitude;
            Debug.DrawLine(new Vector3(transform.position.x, point.y, transform.position.z), point, Color.blue);

            if (dirLength > .2f)
            {
                isCollided = true;

                StopAllCoroutines();
                if (isJumping)
                {
                    StartCoroutine(WallJumpCor(direction.normalized, .5f));
                }
                else
                {
                    StartCoroutine(WaitForJumpCor(collision));
                }
            }
            else
            {
                isJumping = false;
            }
        }
        private IEnumerator WaitForJumpCor(Collision collision)
        {
            while (isCollided)
            {
                if (Input.GetKeyDown(jumpKey))
                {
                    yield return new WaitUntil(() => Input.GetKeyUp(jumpKey));
                    EnterCollision(collision);
                    yield break;
                }
                yield return null;
            }
        }
        private IEnumerator WallJumpCor(Vector3 direction, float waitTime)
        {
            float time = 0;
            while (time < waitTime)
            {
                time += Time.deltaTime;
                if (Input.GetKeyDown(jumpKey))
                {
                    Jump((-direction + Vector3.up) * jumpForce * character._speedMult / 10);
                }
                yield return null;
                isJumping = false;
            }
        }
    }
}
