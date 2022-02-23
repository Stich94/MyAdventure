using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : RayCastWeapon
{
    WaitForSeconds reloadWait;
    WaitForSeconds nextShotWaitTime;

    [Header("Enemy - Specific")]
    [SerializeField] float accuracy;
    [SerializeField] WeaponIK weaponIK;
    [SerializeField] Vector3 targetPos; // for Debug show where the aim Target is

    float distanceToTarget;
    float accumulatedTime;
    bool canShoot = true;


    protected override void Awake()
    {
        animator = GetComponentInParent<Animator>();
        weaponIK = GetComponentInParent<WeaponIK>();
        relaodAnimID = Animator.StringToHash("Reload");

        SetWeaponStats(); // weaponstats must be in Awake, otherwise the firerate will not update
        // reloadAction.performed += _ => StartReloading();


    }

    // Subscribe to this event
    protected override void OnEnable()
    {
        // shootAction.performed += _ => StartFiring();
    }

    // unregister from this event
    protected override void OnDisable()
    {

        // shootAction.performed -= _ => StartFiring();
    }
    protected override void Update()
    {
        // aimDir = thirdPersonShootController.AimDirection;
        aimDir = weaponIK.AimDir.transform.position;
        targetPos = aimDir; // for debug
        // Debug.Log(aimDir);
    }

    public override void StartFiring()
    {
        isFiring = true;
        if (accumulatedTime > 0.0f)
        {
            accumulatedTime = 0.0f;
        }
        // recoil.Reset();

    }

    // using the StopFiring from Base class
    public override void StopFiring()
    {
        isFiring = false;
        if (fireRoutine != null)
        {
            StopCoroutine(fireRoutine);
        }
    }

    public void UpdateWeapon(float _deltaTime, Vector3 _target)
    {
        if (isFiring && !isReloading && CanShoot())
        {
            if (canShoot)
            {
                UpdateFiring(_deltaTime, _target);

            }
            else
            {
                return;
            }
        }
        else if (!CanShoot())
        {
            StartCoroutine(Reload());
            // isFiring = false;
        }

        accumulatedTime += _deltaTime;
    }

    public void UpdateFiring(float _deltatime, Vector3 _target)
    {
        float fireInterval = 1.0f / fireRate;
        while (accumulatedTime >= 0.0f)
        {
            FireBullet(aimDir);
            accumulatedTime -= fireInterval;

        }
    }

    protected override IEnumerator Reload()
    {
        if (currentMagazineAmmo == maxMagazineAmmo)
        {
            // has full ammo in magazine
            yield return null;
        }
        isReloading = true;
        animator.SetLayerWeight(1, 1f);
        animator.SetTrigger(relaodAnimID);
        canShoot = false;
        Debug.Log("Enemy Is reloading");
        yield return reloadWait;
        currentMagazineAmmo = maxMagazineAmmo;
        isReloading = false;
        animator.SetLayerWeight(1, 0f);
        animator.SetTrigger(relaodAnimID);
        canShoot = true;
        Debug.Log("Enemy finished reloading");


    }



    protected override void SetWeaponStats()
    {
        fireRate = weapon.fireRate;
        maxMagazineAmmo = weapon.magazineSize;
        currentMagazineAmmo = weapon.magazineSize;
        reloadTime = weapon.reloadTime;
        reloadWait = new WaitForSeconds(reloadTime);
        nextShotWaitTime = new WaitForSeconds(1f / fireRate);

    }

    // protected override void FireBullet(Vector3 _aimDirection)
    // {
    //     Debug.Log("Enemy is shooting");
    //     isFiring = true;
    //     currentMagazineAmmo--;


    //     Instantiate(bulletPrefab, raycastOrigin.position, transform.rotation);
    // }
    protected override void FireBullet(Vector3 _aimDirection)
    {

        isFiring = true;
        currentMagazineAmmo--;
        shootingParcticleSystem.Play();



        ray.origin = raycastOrigin.position;
        // ray.direction = raycastDestination.position - raycastOrigin.position;

        Vector3 targetDirection = (_aimDirection - raycastOrigin.position).normalized;
        Vector3 direction = GetDirection();
        Debug.DrawLine(raycastOrigin.position, targetDirection, Color.red);

        // }
        if (Physics.Raycast(bulletSpawnPoint.position, direction, out hitInfo, 50f, aimLayerMask))
        {
            Debug.DrawRay(bulletSpawnPoint.position, transform.TransformDirection(Vector3.forward) * hitInfo.distance, Color.red);

            // Debug.DrawLine(bulletSpawnPoint.position, hitInfo.point, Color.red);
            // TrailRenderer trail = Instantiate(tracerEffect, ray.origin, Quaternion.identity);
            // StartCoroutine(SpawnTrail(trail, hitInfo));

            // lastShootTime = Time.time;

            // Rigidbody rb = hitInfo.collider.gameObject.GetComponent<Rigidbody>();
            // if (rb)
            // {
            //     rb.AddForceAtPosition(ray.direction * 20, hitInfo.point, ForceMode.Impulse);
            //     // }

            //     // // Adding the weapon to our hitbox
            //     HitBox hitbox = hitInfo.collider.gameObject.GetComponent<HitBox>();
            //     if (hitbox)
            //     {
            //         hitbox.OnRaycastHit(this, ray.direction);
            //     }
            // }
            // else
            // {
            //     Debug.Log("Nothing hit");
            // }
        }
    }

    // CanShoot func used from base class
    // Relaod from base class used
    // Shoot from base class
    // HitDelay from base class



}
