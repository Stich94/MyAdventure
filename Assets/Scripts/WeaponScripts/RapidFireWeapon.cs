using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFireWeapon : RayCastWeapon
{
    [Header("Rapid Fire Weapon - Specific")]
    // Coroutine fireCoroutine;
    WaitForSeconds rapidFireWait;
    protected override void Awake()
    {
        base.Awake();
        rapidFireWait = new WaitForSeconds(1 / fireRate);
        shootAction.started += _ => StartRapidFiring();
        shootAction.canceled += _ => StopRapidFiring();
    }

    protected override void OnEnable()
    {
        // shootAction.started += _ => StartRapidFiring();
    }

    protected override void OnDisable()
    {
        // shootAction.canceled += _ => StopRapidFiring();
    }

    private void StartRapidFiring()
    {
        if (thirdPersonShootController.IsAiming && !isReloading)
        {
            fireRoutine = StartCoroutine(RapidFire());
        }

    }

    private void StopRapidFiring()
    {
        if (fireRoutine != null)
        {
            StopCoroutine(fireRoutine);
        }
    }

    public override void StartFiring()
    {
        base.StartFiring();
    }

    protected override void FireBullet(Vector3 _aimDirection)
    {
        isFiring = true;
        currentMagazineAmmo--;
        // muzzleFlash.Emit(1);
        Debug.Log("Pew Pew");

        Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.LookRotation(_aimDirection, Vector3.up));

        playerHudManager.UpdatePlayerAmmoUI(currentMagazineAmmo, maxMagazineAmmo);
    }

    protected IEnumerator RapidFire()
    {
        // while (true)
        // {
        //     FireBullet();
        //     yield return rapidFireWait;
        // }

        if (CanShoot() && thirdPersonShootController.IsAiming)
        {
            FireBullet(aimDir);
            while (CanShoot())
            {
                yield return rapidFireWait;
                FireBullet(aimDir);
            }
            StartCoroutine(Reload());
        }
        else
        {
            StartCoroutine(Reload());
        }
    }
}
