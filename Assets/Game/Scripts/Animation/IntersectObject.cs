using UnityEngine;

namespace Bloodymary.Game
{
    public class IntersectObject : MonoBehaviour
    {
        public bool IsIntersect { get; private set; }
        public ButtonActionSystem system;
        public Collider currentCollider { get; private set; }

        void Start()
        {
        }        

        private void OnTriggerEnter(Collider other)
        {
            var character = other.GetComponent<CharacterController>();
            if (character && !character.isAI)
            {
                IsIntersect = true;
                currentCollider = character.GetComponent<Collider>();

                if (!system) system = currentCollider.GetComponent<ButtonActionSystem>();

                if (system) system.ActionBase.Invoke(IsIntersect);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (currentCollider && other == currentCollider)
            {
                IsIntersect = false;
                if (system) system.ActionBase.Invoke(IsIntersect);
            }
            
        }
    }
}

