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
        if (CanShoot() && !isReloading)
        {
            Debug.Log("Enemy is shooting");
            FireBullet(aimDir);
        }
        else
        {
            StartCoroutine(Reload());
            Debug.Log("Enemy is reloading");
        }

    }

    // using the StopFiring from Base class
    public override void StopFiring()
    {
        if (fireRoutine != null)
        {
            StopCoroutine(fireRoutine);
        }
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

        isFiring = true;
        currentMagazineAmmo--;
        // weapon.currentMagazineSize -= 1;
        // muzzleFlash.Emit(1);



        ray.origin = raycastOrigin.position;
        // ray.direction = raycastDestination.position - raycastOrigin.position;
        ray.direction = weaponIK.AimDir.transform.position - raycastOrigin.position;

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
                // StartCoroutine(ImpactDelay(bulletHolePrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal)));
            }
            if (hitInfo.collider.gameObject.GetComponent<IDamageAble>() != null)
            {
                //TODO - hit gets damaged
                // StartCoroutine(HitDelay(hitInfo.collider.gameObject.GetComponent<IDamageAble>()));
                BaseStats otherStats = hitInfo.collider.gameObject.GetComponent<BaseStats>();
                otherStats.TakeDamage(0f);
                // take Damage
            }

            tracer.transform.position = hitInfo.point;
        }
        else
        {
            Debug.Log("Nothing hit");
        }
    }

    // CanShoot func used from base class
    // Relaod from base class used
    // Shoot from base class
    // HitDelay from base class


}
