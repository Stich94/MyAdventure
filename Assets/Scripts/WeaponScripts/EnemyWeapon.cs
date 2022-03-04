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

        if (Physics.Raycast(bulletSpawnPoint.position, direction, out hitInfo, 50f, aimLayerMask))
        {
            Debug.DrawRay(bulletSpawnPoint.position, transform.TransformDirection(_aimDirection) * hitInfo.distance, Color.red);
            // Debug.DrawRay(bulletSpawnPoint.position, transform.TransformDirection(Vector3.forward) * hitInfo.distance, Color.red);

            TrailRenderer trail = Instantiate(tracerEffect, ray.origin, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, hitInfo));

            Debug.Log("enemy ray hit: " + hitInfo);
        }

        Soundmanager.PlaySound(Soundmanager.Sound.PlayerRapidFireShoot, transform.position);
    }
    protected override IEnumerator SpawnTrail(TrailRenderer _trail, RaycastHit _hit)
    {
        float time = 0;
        Vector3 startPosition = _trail.transform.position;

        while (time < 1)
        {
            _trail.transform.position = Vector3.Lerp(startPosition, _hit.point, time);

            time += Time.deltaTime / _trail.time;

            yield return null;
        }
        _trail.transform.position = _hit.point;
        if (_hit.collider.gameObject.CompareTag(environmentTag))
        {

            Instantiate(impactParticleSystem, _hit.point, Quaternion.LookRotation(_hit.normal));
        }
        if (_hit.collider.gameObject.GetComponent<IDamageAble>() != null)
        {
            //TODO - hit gets damaged
            BaseStats otherStats = hitInfo.collider.gameObject.GetComponent<BaseStats>();
            // otherStats.TakeDamage(10f, ray.direction);
            otherStats.TakeDamage(10f);
        }

        Destroy(_trail.gameObject, _trail.time);
    }

    // CanShoot func used from base class
    // Relaod from base class used
    // Shoot from base class
    // HitDelay from base class



}
