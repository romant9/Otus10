using UnityEngine;

namespace Bloodymary.Game
{
    public class LookAtCamera : MonoBehaviour
    {
        Camera cam;
        Collider character;
        public Vector3 offset;
        float offsetY;

        void Start()
        {
            cam = Camera.main;
            character = transform.parent.GetComponent<Collider>();
            offsetY = character && character.GetComponent<CharacterController>() ? character.bounds.size.y + .2f : 0;
            transform.localPosition = offset + Vector3.up * offsetY;
        }

        void Update()
        {
            if (!cam) return;
            transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
        }
    }
}

