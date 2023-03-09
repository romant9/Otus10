using System.Collections;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    private CharacterController Owner;
    private void Start()
    {
        Owner = transform.parent.GetComponent<CharacterController>();
    }
    //public void DamageEvent()
    //{
    //    StartCoroutine(SetDamageInFrame());
    //}
    //private IEnumerator SetDamageInFrame()
    //{
    //    Owner.SetDamage = true;
    //    yield return null;
    //    Owner.SetDamage = false;
    //}

}
