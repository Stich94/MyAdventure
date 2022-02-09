using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFireWeapon : RayCastWeapon
{
    [Header("Rapid Fire Weapon - Specific")]
    Coroutine fireCoroutine;
    WaitForSeconds rapidFireWait;
    protected override void Awake()
    {
        base.Awake();
        rapidFireWait = new WaitForSeconds(1 / fireRate);
        // shootAction.started += _ => StartFiring();
        // shootAction.canceled += _ => StopFiring();
    }

    protected override void OnEnable()
    {
        shootAction.performed += _ => StartRapidFiring();
    }

    protected override void OnDisable()
    {
        shootAction.canceled += _ => StopRapidFiring();
    }

    private void StartRapidFiring()
    {
        fireCoroutine = StartCoroutine(RapidFire());
    }

    private void StopRapidFiring()
    {
        if (fireCoroutine != null)
        {
            StopCoroutine(fireCoroutine);
        }
    }

    protected IEnumerator RapidFire()
    {
        // while (true)
        // {
        //     FireBullet();
        //     yield return rapidFireWait;
        // }

        if (CanShoot())
        {
            FireBullet();
            while (CanShoot())
            {
                yield return rapidFireWait;
                FireBullet();
            }
            StartCoroutine(Reload());
        }
        else
        {
            StartCoroutine(Reload());
        }
    }
}