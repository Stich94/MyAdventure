using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiWeapons : MonoBehaviour
{
    [SerializeField] float inaccuracy = 0.4f;
    [SerializeField] EnemyWeapon currentWeapon;
    Animator animator;
    WeaponIK weaponIK;
    Transform currentTarget;

    public bool WeaponIsAcive { get; set; } = false;

    // shooting is handled in the AiStateMachine
    private void Start()
    {
        animator = GetComponent<Animator>();
        weaponIK = GetComponent<WeaponIK>();
    }

    public void Equip(EnemyWeapon _weapon)
    {
        currentWeapon = _weapon;
        weaponIK.SetAimTransform(currentWeapon.GetBulletOriginPosition); // raycast origin
    }

    void Update()
    {
        if (currentTarget != null && WeaponIsAcive)
        {
            // calculate the inaccuracy for the enemy
            Vector3 target = currentTarget.position;
            target += Random.insideUnitSphere * inaccuracy;
        }
    }

    public void SetTarget(Transform _target)
    {
        weaponIK.SetTargetTransform(_target);
        currentTarget = _target;
    }

    public void SetFiring(bool _canFire)
    {
        if (_canFire)
        {
            currentWeapon.StartFiring();
        }
        else
        {
            currentWeapon.StopFiring();
        }
    }


}
