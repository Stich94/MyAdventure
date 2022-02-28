using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class RapidFireWeapon : RayCastWeapon
{
    [Header("Rapid Fire Weapon - Speific")]

    [SerializeField] float cameraShakeAmount = 0.5f;
    CinemachineImpulseSource source;
    // Coroutine fireCoroutine;
    WaitForSeconds rapidFireWait;
    protected override void Awake()
    {
        base.Awake();
        source = GetComponent<CinemachineImpulseSource>();
        rapidFireWait = new WaitForSeconds(1 / fireRate);
        // shootAction.started += _ => StartRapidFiring();
        // shootAction.canceled += _ => StopRapidFiring();
    }

    protected override void OnEnable()
    {
        // shootAction.started += _ => StartRapidFiring();
        shootAction.started += _ => StartRapidFiring();
        shootAction.canceled += _ => StopRapidFiring();
    }

    protected override void OnDisable()
    {
        // shootAction.canceled += _ => StopRapidFiring();
    }


    private void StartRapidFiring()
    {
        if (thirdPersonShootController.IsAiming && !isReloading && weapon.currentMagazineSize > 0)
        {
            fireRoutine = StartCoroutine(RapidFire());
        }

    }

    public void StopRapidFiring()
    {
        if (fireRoutine != null)
        {
            StopCoroutine(fireRoutine);
        }
    }

    public override void StartFiring()
    {
        // if (thirdPersonShootController.IsAiming && !isReloading)
        // {
        //     fireRoutine = StartCoroutine(RapidFire());
        // }
    }

    // protected override void FireBullet(Vector3 _aimDirection)
    // {
    //     isFiring = true;
    //     currentMagazineAmmo--;
    //     weapon.currentMagazineSize--;
    //     // muzzleFlash.Emit(1);
    //     Debug.Log("Pew Pew");

    //     Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.LookRotation(_aimDirection, Vector3.up));

    //     playerHudManager?.UpdateWeaponAmmoUI();

    //     // source.GenerateImpulse(cameraShakeAmount);
    //     recoil.GenerateRecoil();


    // }

    protected IEnumerator RapidFire()
    {
        // while (true)
        // {
        //     FireBullet();
        //     yield return rapidFireWait;
        // }

        if (CanShoot() && thirdPersonShootController.IsAiming)
        {
            // FireBullet(aimDir);
            while (CanShoot())
            {
                yield return rapidFireWait;
                FireBullet(aimDir);
                Soundmanager.PlaySound(Soundmanager.Sound.PlayerRapidFireShoot);
            }
            StartCoroutine(Reload());
        }
        else
        {
            StartCoroutine(Reload());
        }
    }
}
