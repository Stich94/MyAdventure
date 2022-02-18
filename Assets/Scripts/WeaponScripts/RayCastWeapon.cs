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

    [SerializeField] protected String environmentTag = "Environment";
    [SerializeField] protected LayerMask targetLayer;
    [SerializeField] protected LayerMask aimLayerMask;
    [SerializeField] protected bool isFiring = false;
    [SerializeField] protected bool isReloading = false; // just for Debug


    [Header("VFX")]
    [SerializeField] protected ParticleSystem muzzleFlash;
    [SerializeField] protected GameObject bulletHolePrefab;
    [SerializeField] protected TrailRenderer tracerEffect;


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
                FireBullet(aimDir);
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
        Debug.Log("Pew Pew");

        // Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.LookRotation(_aimDirection, Vector3.up));

        playerHudManager?.UpdateWeaponAmmoUI();


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
                otherStats.TakeDamage(10f, ray.direction);
                // take Damage
            }
            tracer.transform.position = hitInfo.point;

            // Add a Force to the Rigidbody
            Rigidbody rb = hitInfo.collider.gameObject.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.AddForceAtPosition(ray.direction * 20, hitInfo.point, ForceMode.Impulse);
            }

            // Adding the weapon to our hitbox
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


        Debug.Log("Is reloading");
        yield return reloadWait;
        currentMagazineAmmo = maxMagazineAmmo;
        weapon.currentMagazineSize = weapon.magazineSize;
        isReloading = false;
        Debug.Log("Finished reloading");
        playerHudManager?.UpdateWeaponAmmoUI();

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
}