using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField] BaseStats health;

    public BaseStats Health { get { return health; } set { health = value; } }


    public void OnRaycastHit(RayCastWeapon _weapon, Vector3 _direction)
    {
        health.TakeDamage(_weapon.GetWeaponDamage, _direction);
    }
}
