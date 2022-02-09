using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class RayCastWeapon : MonoBehaviour
{
    [SerializeField] protected PlayerInput playerInput;
    [SerializeField] protected WeapnScriptable weapon;
    [SerializeField] protected Transform raycastOrigin;
    [SerializeField] protected Transform raycastDestination;
    [SerializeField] String environmentTag = "Environment";
    [SerializeField] LayerMask targetLayer;
    [SerializeField] LayerMask aimLayerMask;
    [SerializeField] bool isFiring = false;

    [Header("VFX")]
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject bulletHolePrefab;
    [SerializeField] TrailRenderer tracerEffect;
    Ray ray;
    RaycastHit hitInfo;
    protected InputAction shootAction;

    // get Weaponspecs from the Scriptable
    protected float maxMagazineAmmo;
    protected float currentMagazineAmmo;
    protected float fireRate = 5f;
    protected float reloadTime;
    WaitForSeconds reloadWait;
    WaitForSeconds nextShotWaitTime;
    Coroutine fireRoutine;
    protected virtual void Awake()
    {
        shootAction = playerInput.actions["Shoot"];
        SetWeaponStats();
        // nextShotWaitTime = new WaitForSeconds(1 / fireRate);
        // shootAction.started += _ => StartFiring();
        // shootAction.canceled += _ => StopFiring();
    }

    // Subscribe to this event
    protected virtual void OnEnable()
    {
        // playerControlInstance.Player.Shoot.performed += Shoot;
        // playerControlInstance.Player.Enable();
        // shootAction.performed += _ => ShootGun();
        shootAction.performed += _ => StartFiring();
    }

    // unregister from this event
    protected virtual void OnDisable()
    {
        // playerControlInstance.Player.Shoot.performed -= Shoot;
        // playerControlInstance.Player.Disable();
        // shootAction.performed -= _ => ShootGun();
        // shootAction.performed -= _ => StartFiring();
        shootAction.canceled += _ => StopFiring();
    }

    protected virtual void StartFiring()
    {
        Debug.Log(shootAction.ReadValue<float>());

        // FireBullet();
        fireRoutine = StartCoroutine(Shoot());
        // StartCoroutine(Shoot());

    }
    protected virtual void StopFiring()
    {
        if (fireRoutine != null)
        {
            StopCoroutine(fireRoutine);
        }
    }
    protected virtual void SetWeaponStats()
    {
        fireRate = weapon.fireRate;
        maxMagazineAmmo = weapon.magazineSize;
        currentMagazineAmmo = weapon.magazineSize;
        reloadTime = weapon.reloadTime;
        reloadWait = new WaitForSeconds(reloadTime);
        nextShotWaitTime = new WaitForSeconds(1f / fireRate);
    }

    protected virtual void FireBullet()
    {
        isFiring = true;
        currentMagazineAmmo--;
        muzzleFlash.Emit(1);
        Debug.Log("Pew Pew");
        ray.origin = raycastOrigin.position;
        ray.direction = raycastDestination.position - raycastOrigin.position;

        // pew pew effect
        TrailRenderer tracer = Instantiate(tracerEffect, ray.origin, Quaternion.identity);
        tracer.AddPosition(ray.origin);

        if (Physics.Raycast(ray, out hitInfo, 50f, aimLayerMask))
        {
            // Debug.Log("Ray hit: " + hitInfo.collider.tag);

            // Debug.DrawLine(ray.origin, hitInfo.point, Color.red, 1.0f);
            if (hitInfo.collider.CompareTag(environmentTag))
            {
                // impact effect
                Instantiate(bulletHolePrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            }
            if (hitInfo.collider.gameObject.GetComponent<IDamageAble>() != null)
            {
                //TODO - hit gets damaged
                // take Damage
            }

            tracer.transform.position = hitInfo.point;
        }
        else
        {
            Debug.Log("Nothing hit");
        }
    }
    protected bool CanShoot()
    {
        bool hasEnoughAmmo = currentMagazineAmmo > 0;
        return hasEnoughAmmo;
    }


    protected IEnumerator Reload()
    {
        if (currentMagazineAmmo == maxMagazineAmmo)
        {
            // has full ammo in magazine
            yield return null;
        }

        Debug.Log("Is reloading");
        yield return reloadWait;
        currentMagazineAmmo = maxMagazineAmmo;
        Debug.Log("Finished reloading");
    }


    protected IEnumerator Shoot()
    {
        if (CanShoot())
        {
            FireBullet();
            while (CanShoot())
            {
                yield return nextShotWaitTime;
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
