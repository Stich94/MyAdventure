using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : RayCastWeapon
{

    WaitForSeconds reloadWait;
    WaitForSeconds nextShotWaitTime;
    WeaponIK weaponIK;

    [Header("Enemy - Specific")]
    [SerializeField] float accuracy;


    protected override void Awake()
    {
        animator = GetComponentInParent<Animator>();
        weaponIK = GetComponent<WeaponIK>();
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
        // Debug.Log(aimDir);
    }

    public override void StartFiring()
    {
        if (CanShoot() && !isReloading)
        {
            FireBullet(weaponIK.aimDir.transform.position);
        }
        else
        {
            StartCoroutine(Reload());
        }

    }


    // using the StopFiring from Base class

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
        weapon.currentMagazineSize -= 1;
        // muzzleFlash.Emit(1);



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
                // StartCoroutine(ImpactDelay(bulletHolePrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal)));
            }
            if (hitInfo.collider.gameObject.GetComponent<IDamageAble>() != null)
            {
                //TODO - hit gets damaged
                // StartCoroutine(HitDelay(hitInfo.collider.gameObject.GetComponent<IDamageAble>()));
                BaseStats otherStats = hitInfo.collider.gameObject.GetComponent<BaseStats>();
                otherStats.TakeDamage(10f);
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
