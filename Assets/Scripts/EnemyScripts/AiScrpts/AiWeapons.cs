using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiWeapons : MonoBehaviour
{
    RayCastWeapon currentWeapon;
    Animator animator;
    WeaponIK weaponIK;
    Transform currentTarget;

    private void Start()
    {
        animator = GetComponent<Animator>();
        weaponIK = GetComponent<WeaponIK>();
    }

    public void Equip(RayCastWeapon _weapon)
    {
        currentWeapon = _weapon;
        weaponIK.SetAimTransform(currentWeapon.GetBulletOriginPosition);
    }

    public void SetTarget(Transform _target)
    {
        weaponIK.SetTargetTransform(_target);
        currentTarget = _target;
    }
}
