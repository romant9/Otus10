using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    private AnimationManager root;
    private void Start()
    {
        root = transform.parent.GetComponent<AnimationManager>();
    }
    public void DamageEvent()
    {
        if (root.enemy.currentWeapon != null)
        {
            Debug.Log("Milee damage To  " + root.enemy.name);
            root.enemy.SetHealth(root.enemy);
        }
    }


}
