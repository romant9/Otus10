using System.Collections;
using UnityEngine;
using static Bloodymary.Game.GameManager; //GManager

public class CameraController : MonoBehaviour
{
    public float zoom = 1;
    Transform charFirst;
    Transform charSecond;


    private void Start()
    {
        charFirst = GManager._PolicemanUI.Player.transform;
        charSecond = GManager._HooliganUI.Player.transform;
    }
    private void OnEnable()
    {
        StartCoroutine(LookAtCharacters());
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }

    void Update()
    {
    }
    private IEnumerator LookAtCharacters()
    {
        yield return new WaitForEndOfFrame();
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
