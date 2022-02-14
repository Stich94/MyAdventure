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
    public string WeaponTitle { get { return weapon.weaponName; } }
    public int WeaponId { get { return weapon.weaponId; } }
    [SerializeField] protected Transform raycastOrigin;
    [SerializeField] protected Transform raycastDestination;
    public Transform RayCastDestination { get { return raycastDestination; } set { raycastDestination = value; } }
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Transform bulletSpawnPoint;
    public Transform GetBulletOriginPosition { get { return bulletSpawnPoint; } }

    [SerializeField] String environmentTag = "Environment";
    [SerializeField] protected LayerMask targetLayer;
    [SerializeField] protected LayerMask aimLayerMask;
    [SerializeField] protected bool isFiring = false;
    [SerializeField] protected bool isReloading = false; // just for Debug


    [Header("VFX")]
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject bulletHolePrefab;
    [SerializeField] TrailRenderer tracerEffect;
    protected Ray ray;
    protected RaycastHit hitInfo;
    protected InputAction shootAction;
    protected InputAction reloadAction;

    // get Weaponspecs from the Scriptable
    protected int maxMagazineAmmo;
    [Header("Weapon Stats")]
    [SerializeField] protected int currentMagazineAmmo;
    [SerializeField] protected float fireRate = 5f;
    protected float reloadTime;
    WaitForSeconds reloadWait;
    WaitForSeconds nextShotWaitTime;
    protected Coroutine fireRoutine;
    protected Coroutine reloadRoutine;

    protected Vector3 aimDir;

    [SerializeField] protected UIManager playerHudManager;

    protected virtual void Awake()
    {
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
        // shootAction.canceled += _ => StopFiring();


        shootAction.performed -= _ => StartFiring();
    }
    protected void Update()
    {
        aimDir = thirdPersonShootController.AimDirection;
        // Debug.Log(aimDir);
    }

    protected void StartReloading()
    {
        if (!isReloading)
        {
            StartCoroutine(Reload());

        }
    }


    public virtual void StartFiring()
    {
        // Debug.Log(shootAction.ReadValue<float>());
        if (thirdPersonShootController.IsAiming)
        {
            if (CanShoot() && !isReloading)
            {
                FireBullet(aimDir);

            }
            else
            {
                StartCoroutine(Reload());
            }
        }

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

    protected virtual void FireBullet(Vector3 _aimDirection)
    {

        isFiring = true;
        currentMagazineAmmo--;
        // muzzleFlash.Emit(1);
        Debug.Log("Pew Pew");

        // Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.LookRotation(_aimDirection, Vector3.up));

        // playerHudManager.UpdatePlayerAmmoUI(currentMagazineAmmo, maxMagazineAmmo);

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


    protected IEnumerator Reload()
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
        isReloading = false;
        Debug.Log("Finished reloading");
        // playerHudManager.UpdatePlayerAmmoUI(currentMagazineAmmo, maxMagazineAmmo);

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





}
