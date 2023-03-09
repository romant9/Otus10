using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInfo : MonoBehaviour
{
    public CharacterController WeaponOwner { get; protected set; }
    public enum WeaponType
    {
        Melee,
        Fire,
        None
    }
    public WeaponType _weaponType;
    
    public float _weaponDamage;
    public string _weaponName;
    [HideInInspector]
    public bool _isDamaging;

    [ShowIf("_weaponType", WeaponType.Fire)]
    public float _weaponSpeed;

    protected void Start()
    {
        WeaponOwner = GetComponentInParent<CharacterController>();
    }

    
}



