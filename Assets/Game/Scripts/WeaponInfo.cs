using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInfo : MonoBehaviour
{
    public AnimationManager WOwner { get; protected set; }
    public enum WeaponType
    {
        Melee,
        Fire       
    }
    public WeaponType type;
    public float wDamage;
    public string wName;

    protected void Start()
    {
        WOwner = GetComponentInParent<AnimationManager>();
    }
}
