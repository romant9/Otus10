using System.Collections;
using UnityEngine;

namespace Bloodymary.Game
{
    public class ElevatorSystem : ButtonActionSystem
    {

        public Animator _animator;
        public Transform elevatorButton;
        public GameObject help;
        public Color activeColor;
        public Color passiveColor;

        private IntersectObject intersectElevator; // платформа

        private bool stayNear;
        private bool IsIntersect;

        protected override void Start()
        {
            base.Start();
            intersectElevator = _animator.GetComponent<IntersectObject>();
        }

        void Update()
        {
            if (IsIntersect)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    StartCoroutine(KeyDown(true));
                    _animator.SetBool("stayNear", stayNear);
                    _animator.SetTrigger("move");
                    stayNear = !stayNear;

                    intersectElevator.system = this;
                    if (intersectElevator.IsIntersect)
                    {
                        StartCoroutine(CharacterLink());
                    }
                }

            }
        }

        protected override void ActionMethod(bool intersect)
        {
            IsIntersect = intersect;
            help.SetActive(IsIntersect);


        }

        IEnumerator KeyDown(bool isDown)
        {
            float timeElapsed = 0;
            float duration = .5f;
            Vector3 startPosition = elevatorButton.localPosition;
            Vector3 targetPosition = new Vector3(elevatorButton.localPosition.x, isDown ? .04f : 0.14f, elevatorButton.localPosition.z);
            while (timeElapsed < duration)
            {
                elevatorButton.localPosition = Vector3.Lerp(startPosition, targetPosition, timeElapsed / duration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            elevatorButton.localPosition = targetPosition;
            if (isDown == true)
            {
                yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
                StartCoroutine(KeyDown(false));
            }
        }
        IEnumerator CharacterLink()
        {
            float t = 0;
            while (t < _animator.GetCurrentAnimatorStateInfo(0).length)
            {
                t += Time.deltaTime;
                intersectElevator.currentCollider.transform.position = intersectElevator.transform.position + new Vector3(0,.1f,0);
                yield return null;
            }

            //while (intersectElevator.IsIntersect)
            //{
            //    Debug.Log(intersectElevator.currentCollider.name);
            //    intersectElevator.currentCollider.transform.position = intersectElevator.transform.position;
            //    yield return null;
            //}
        }
    }
}

