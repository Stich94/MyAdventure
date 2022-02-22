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
        canShoot = false;
        Debug.Log("Is reloading");
        yield return reloadWait;
        currentMagazineAmmo = maxMagazineAmmo;
        isReloading = false;
        canShoot = true;
        Debug.Log("Finished reloading");


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

    protected override void FireBullet(Vector3 _aimDirection)
    {
        Debug.Log("Enemy is shooting");
        isFiring = true;
        currentMagazineAmmo--;


        Instantiate(bulletPrefab, raycastOrigin.position, transform.rotation);


    }

    // CanShoot func used from base class
    // Relaod from base class used
    // Shoot from base class
    // HitDelay from base class



}
