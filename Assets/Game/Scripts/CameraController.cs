using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    protected GameManager _GM { get { return GameManager.GM; } }
    public float zoom = 1;
    Transform charFirst;
    Transform charSecond;

    void Start()
    {
        charFirst = _GM._Policeman.transform;
        charSecond = _GM._Hooligan.transform;

        StartCoroutine(LookAtCharacters()); 
    }

    void Update()
    {
    }
    private IEnumerator LookAtCharacters()
    {
        while (true)
        {
            float distance = Vector3.Distance(charFirst.position, charSecond.position);
            Vector3 _targetPosition = (charFirst.position + charSecond.position) / 2;
            transform.LookAt(_targetPosition, Vector3.up);
            distance = Mathf.Clamp(distance, 3, 20);
            //transform.position = _targetPosition - transform.rotation * Vector3.forward * distance * 1.5f / zoom;
            transform.position = _targetPosition - transform.rotation * Vector3.forward * distance * 1.5f / zoom;
            transform.position = new Vector3(transform.position.x, transform.position.y, _targetPosition.z);

            yield return null;
        }
    }
}
