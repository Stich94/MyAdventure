using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class RayCastWeapon : MonoBehaviour
{
    [SerializeField] protected PlayerInput playerInput;
    [SerializeField] protected ThirdPersonShooterController thirdPersonShootController;
    [SerializeField] protected WeapnScriptable weapon;
    [SerializeField] protected Vector3 bulletSpreadVariance = new Vector3(0.5f, 0.3f, 0.1f);
    [SerializeField] protected float shootDelay = 0.5f;
    float weaponDamage = 0f;
    public float GetWeaponDamage => weaponDamage;
    public string WeaponTitle { get { return weapon.weaponName; } }
    public int WeaponId { get { return weapon.weaponId; } }
    [SerializeField] protected Transform raycastOrigin;
    [SerializeField] protected Transform raycastDestination;
    public Transform RayCastDestination { get { return raycastDestination; } set { raycastDestination = value; } }
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Transform bulletSpawnPoint;
    public Transform GetBulletOriginPosition { get { return raycastOrigin; } }
    [SerializeField] protected string enemyTag = "Enemy";
    [SerializeField] protected String environmentTag = "Environment";
    [SerializeField] protected LayerMask targetLayer;
    [SerializeField] protected LayerMask aimLayerMask;
    [SerializeField] protected bool isFiring = false;
    [SerializeField] protected bool isReloading = false; // just for Debug
    public bool IsReloading { get { return isReloading; } }


    [Header("VFX")]
    [SerializeField] protected ParticleSystem muzzleFlash;
    [SerializeField] protected GameObject bulletHolePrefab;
    [SerializeField] protected ParticleSystem shootingParcticleSystem;
    [SerializeField] protected ParticleSystem impactParticleSystem;
    [SerializeField] protected TrailRenderer tracerEffect;
    [SerializeField] protected GameObject magazine;


    // Animator for reload Animation
    [SerializeField] protected Animator animator;
    protected int relaodAnimID;
    protected Ray ray;
    protected RaycastHit hitInfo;
    protected InputAction shootAction;
    protected InputAction reloadAction;

    // get Weaponspecs from the Scriptable


    [Header("Weapon Stats")]
    [SerializeField] protected int currentMagazineAmmo;
    [SerializeField] protected float fireRate = 5f;
    protected int maxMagazineAmmo;
    // public int GetCurrentAmmo => weapon.currentMagazineSize;
    // public int GetMaxMagazineSize => weapon.magazineSize;
    protected float reloadTime;
    WaitForSeconds reloadWait;
    WaitForSeconds nextShotWaitTime;
    protected Coroutine fireRoutine;
    protected Coroutine reloadRoutine;

    protected Vector3 aimDir;
    protected ThirdPersonMovementController playerController;

    protected UIManager playerHudManager;

    protected WeaponRecoil recoil;

    public WeaponRecoil Recoil { get { return recoil; } set { recoil = value; } }

    protected float lastShootTime;

    protected bool addBulletSpread = true;


    protected virtual void Awake()
    {
        recoil = GetComponent<WeaponRecoil>();
        playerController = GetComponent<ThirdPersonMovementController>();
        animator = GetComponentInParent<Animator>();
        relaodAnimID = Animator.StringToHash("Reload");
        playerInput = GetComponentInParent<PlayerInput>();
        thirdPersonShootController = gameObject.GetComponentInParent<ThirdPersonShooterController>();
        shootAction = playerInput.actions["Shoot"];
        reloadAction = playerInput.actions["Reload"];
        SetWeaponStats(); // weaponstats must be in Awake, otherwise the firerate will not update
        reloadAction.performed += _ => StartReloading();
        // nextShotWaitTime = new WaitForSeconds(1 / fireRate);
        // shootAction.started += _ => StartFiring();
        // shootAction.canceled += _ => StopFiring();

    }

    // Subscribe to this event
    protected virtual void OnEnable()
    {
        shootAction.performed += _ => StartFiring();
    }

    // unregister from this event
    protected virtual void OnDisable()
    {

        shootAction.performed -= _ => StartFiring();
    }
    protected virtual void Update()
    {
        aimDir = thirdPersonShootController.AimDirection;
        // Debug.Log(aimDir);
    }

    protected void StartReloading()
    {
        if (!isReloading)
        {
            // animator.SetBool(relaodAnimID, true);
            animator?.SetTrigger("Reload");
            StartCoroutine(Reload());
            // animator.SetBool(relaodAnimID, false);
        }
    }


    public virtual void StartFiring()
    {
        // Debug.Log(shootAction.ReadValue<float>());
        if (thirdPersonShootController.IsAiming)
        {
            if (CanShoot() && !isReloading && weapon.currentMagazineSize > 0)
            {
                if (lastShootTime + shootDelay < Time.time)
                {
                    FireBullet(aimDir);
                }
            }
            else
            {
                StartCoroutine(Reload());
            }
        }

    }
    public virtual void StopFiring()
    {
        if (fireRoutine != null)
        {
            StopCoroutine(fireRoutine);
        }
    }
    protected virtual void SetWeaponStats()
    {
        weaponDamage = weapon.damage;
        fireRate = weapon.fireRate;
        maxMagazineAmmo = weapon.magazineSize;
        currentMagazineAmmo = weapon.magazineSize;
        reloadTime = weapon.reloadTime;
        reloadWait = new WaitForSeconds(reloadTime);
        nextShotWaitTime = new WaitForSeconds(1f / fireRate);
    }

    protected virtual void FireBullet(Vector3 _aimDirection)
    {

        isFiring = true;
        currentMagazineAmmo--;
        weapon.currentMagazineSize -= 1;
        // muzzleFlash.Emit(1);
        shootingParcticleSystem.Play();

        // Debug.Log("Pew Pew");

        // Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.LookRotation(_aimDirection, Vector3.up));

        playerHudManager?.UpdateWeaponAmmoUI();


        ray.origin = raycastOrigin.position;
        ray.direction = raycastDestination.position - raycastOrigin.position;

        Vector3 direction = GetDirection();
        // pew pew effect
        // TrailRenderer tracer = Instantiate(tracerEffect, ray.origin, Quaternion.identity);
        // tracer.AddPosition(ray.origin);

        // if (Physics.Raycast(ray, out hitInfo, 50f, aimLayerMask))
        // {

        //     TrailRenderer trail = Instantiate(tracerEffect, ray.origin, Quaternion.identity);
        //     StartCoroutine(SpawnTrail(trail, hitInfo));

        //     lastShootTime = Time.time;

        //     if (hitInfo.collider.CompareTag(environmentTag))
        //     {
        //         // impact effect
        //         // Instantiate(bulletHolePrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
        //     }
        //     if (hitInfo.collider.gameObject.GetComponent<IDamageAble>() != null)
        //     {
        //         //TODO - hit gets damaged
        //         // StartCoroutine(HitDelay(hitInfo.collider.gameObject.GetComponent<IDamageAble>()));
        //         BaseStats otherStats = hitInfo.collider.gameObject.GetComponent<BaseStats>();
        //         otherStats.TakeDamage(10f, ray.direction);
        //         // take Damage
        //     }
        //     // tracer.transform.position = hitInfo.point;

        //     // Add a Force to the Rigidbody
        //     Rigidbody rb = hitInfo.collider.gameObject.GetComponent<Rigidbody>();
        //     if (rb)
        //     {
        //         rb.AddForceAtPosition(ray.direction * 20, hitInfo.point, ForceMode.Impulse);
        //     }

        //     // Adding the weapon to our hitbox
        //     HitBox hitbox = hitInfo.collider.gameObject.GetComponent<HitBox>();
        //     if (hitbox)
        //     {
        //         hitbox.OnRaycastHit(this, ray.direction);
        //     }

        // }
        if (Physics.Raycast(bulletSpawnPoint.position, direction, out hitInfo, 50f, aimLayerMask))
        {

            TrailRenderer trail = Instantiate(tracerEffect, ray.origin, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, hitInfo));

            lastShootTime = Time.time;

            // if (hitInfo.collider.CompareTag(environmentTag))
            // {
            //     // impact effect
            //     // Instantiate(bulletHolePrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            // }
            // if (hitInfo.collider.gameObject.GetComponent<IDamageAble>() != null)
            // {
            //     //TODO - hit gets damaged
            //     // StartCoroutine(HitDelay(hitInfo.collider.gameObject.GetComponent<IDamageAble>()));
            //     BaseStats otherStats = hitInfo.collider.gameObject.GetComponent<BaseStats>();
            //     otherStats.TakeDamage(10f, ray.direction);
            //     // take Damage
            // }
            // // tracer.transform.position = hitInfo.point;

            // // Add a Force to the Rigidbody
            Rigidbody rb = hitInfo.collider.gameObject.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.AddForceAtPosition(ray.direction * 20, hitInfo.point, ForceMode.Impulse);
                // }

                // // Adding the weapon to our hitbox
                HitBox hitbox = hitInfo.collider.gameObject.GetComponent<HitBox>();
                if (hitbox)
                {
                    hitbox.OnRaycastHit(this, ray.direction);
                }
            }
            else
            {
                Debug.Log("Nothing hit");
            }
            recoil.GenerateRecoil();
        }
    }
    // protected void FireBullet()
    // {
    //     Vector3 velocity = (raycastDestination.position - raycastOrigin.position).normalized; // bullet speed required
    //     Projectile projectile = projectilePrefab.Init(raycastOrigin.position, velocity);
    //     projectiles.Add(projectile);
    // }


    protected bool CanShoot()
    {
        bool hasEnoughAmmo = currentMagazineAmmo > 0;
        return hasEnoughAmmo;
    }


    protected virtual IEnumerator Reload()
    {
        if (currentMagazineAmmo == maxMagazineAmmo)
        {
            // has full ammo in magazine
            yield return null;
        }
        isReloading = true;
        animator.SetLayerWeight(2, 1f);
        animator.SetTrigger(relaodAnimID);

        Debug.Log("Is reloading");
        yield return reloadWait;
        currentMagazineAmmo = maxMagazineAmmo;
        weapon.currentMagazineSize = weapon.magazineSize;
        isReloading = false;

        Debug.Log("Finished reloading");
        playerHudManager?.UpdateWeaponAmmoUI();
        animator.SetLayerWeight(2, 0f);
    }


    protected IEnumerator Shoot()
    {
        if (CanShoot())
        {
            FireBullet(aimDir);
            while (CanShoot())
            {
                yield return nextShotWaitTime;
                FireBullet(aimDir);
            }
            StartCoroutine(Reload());
        }
        else
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator HitDelay(IDamageAble _target)
    {
        yield return new WaitForSeconds(0.5f);
        _target.TakeDamage(2.5f);
    }

    private IEnumerator ImpactDelay(GameObject _bulletHolePrefab, Vector3 _hitPos, Quaternion _rotaion)
    {
        yield return new WaitForSeconds(0.5f);
        Instantiate(_bulletHolePrefab, _hitPos, _rotaion);
    }

    public void GetReferences()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        thirdPersonShootController = gameObject.GetComponentInParent<ThirdPersonShooterController>();
        shootAction = playerInput.actions["Shoot"];
        reloadAction = playerInput.actions["Reload"];
    }


    // void OnDestroy()
    // {
    //     shootAction.performed -= _ => StartFiring();
    //     reloadAction.performed -= _ => StartReloading();
    // }

    public WeapnScriptable GetWeaponData()
    {
        return this.weapon;
    }

    protected Vector3 GetDirection()
    {
        Vector3 direction = transform.forward;

        if (addBulletSpread)
        {
            float randomX = UnityEngine.Random.Range(-bulletSpreadVariance.x, bulletSpreadVariance.x);
            float randomY = UnityEngine.Random.Range(-bulletSpreadVariance.y, bulletSpreadVariance.y);
            float randomZ = UnityEngine.Random.Range(-bulletSpreadVariance.z, bulletSpreadVariance.z);

            direction += new Vector3(randomX, randomY, randomZ);

            direction.Normalize();
        }

        return direction;
    }

    protected IEnumerator SpawnTrail(TrailRenderer _trail, RaycastHit _hit)
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
            otherStats.TakeDamage(10f, ray.direction);
        }

        Destroy(_trail.gameObject, _trail.time);
    }
}