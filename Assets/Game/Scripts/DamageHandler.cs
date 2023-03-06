using System.Collections;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    private AnimationManager owner;
    private void Start()
    {
        owner = transform.parent.GetComponent<AnimationManager>();
    }
    public void DamageEvent()
    {
        StartCoroutine(SetDamageInFrame());
    }
    private IEnumerator SetDamageInFrame()
    {
        owner.SetDamage = true;
        yield return null;
        owner.SetDamage = false;
    }

}
