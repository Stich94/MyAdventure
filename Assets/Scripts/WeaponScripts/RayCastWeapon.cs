using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class RayCastWeapon : MonoBehaviour
{
    [SerializeField] WeapnScriptable weapon;
    [SerializeField] Transform raycastOrigin;
    [SerializeField] Transform raycastDestination;
    [SerializeField] String environmentTag = "Environment";
    [SerializeField] LayerMask targetLayer;
    [SerializeField] LayerMask aimLayerMask;
    PlayerControls playerControlInstance;
    InputAction shootAction;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] bool isFiring = false;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject bulletHolePrefab;
    [SerializeField] TrailRenderer tracerEffect;

    ThirdPersonShooterController thirdPersonController;
    Ray ray;
    RaycastHit hitInfo;

    Vector3 startPoint;
    Vector3 destination;

    [SerializeField] float fireRate;

    float accumulatedTime;

    private void Awake()
    {

        shootAction = playerInput.actions["Shoot"];
        fireRate = weapon.fireRate;

    }

    // Subscribe to this event
    private void OnEnable()
    {
        // playerControlInstance.Player.Shoot.performed += Shoot;
        // playerControlInstance.Player.Enable();
        // shootAction.performed += _ => ShootGun();
        shootAction.performed += _ => StartFiring(startPoint, destination);

    }


    // unregister from this event
    private void OnDisable()
    {
        // playerControlInstance.Player.Shoot.performed -= Shoot;
        // playerControlInstance.Player.Disable();
        // shootAction.performed -= _ => ShootGun();
        shootAction.performed -= _ => StartFiring(startPoint, destination);
    }

    // private void Shoot(InputAction.CallbackContext obj)
    // {
    //     Debug.Log("Shooting, Pew Pew");
    // }

    private void Update()
    {

        Debug.Log(shootAction.ReadValue<float>());
        // ray.origin = raycastOrigin.position;
        // startPoint = ray.origin;
        // ray.direction = raycastDestination.position - raycastOrigin.position;
        // destination = ray.direction;

    }

    private void StartFiring(Vector3 _origin, Vector3 _destination)
    {
        Debug.Log(shootAction.ReadValue<float>());
        isFiring = true;
        accumulatedTime = 0.0f;
        FireBullet();
    }

    private void FireBullet()
    {
        muzzleFlash.Emit(1);
        Debug.Log("Pew Pew");
        ray.origin = raycastOrigin.position;
        ray.direction = raycastDestination.position - raycastOrigin.position;

        TrailRenderer tracer = Instantiate(tracerEffect, ray.origin, Quaternion.identity);
        tracer.AddPosition(ray.origin);

        if (Physics.Raycast(ray, out hitInfo, 50f, aimLayerMask))
        {
            Debug.Log("Ray hit: " + hitInfo.collider.tag);
            Debug.DrawLine(ray.origin, hitInfo.point, Color.red, 1.0f);
            // hitEffect.transform.position = hitInfo.point;
            // hitEffect.transform.forward = hitInfo.normal; // align the axis
            if (hitInfo.collider.CompareTag(environmentTag))
            {
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

    public void UpdateFiring(float _deltaTime)
    {
        accumulatedTime += _deltaTime;
        float fireInterval = 1.0f / fireRate;
        while (accumulatedTime >= 0.0f)
        {
            FireBullet();
            accumulatedTime -= fireInterval;
        }
    }
}
