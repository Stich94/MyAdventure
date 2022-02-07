using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class RayCastWeapon : MonoBehaviour
{
    [SerializeField] Transform raycastOrigin;
    [SerializeField] Transform raycastDestination;
    [SerializeField] ParticleSystem hitEffect;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] LayerMask deculLayer;
    PlayerControls playerControlInstance;
    InputAction shootAction;
    [SerializeField] PlayerInput playerInput;

    [SerializeField] WeapnScriptable weapon;
    [SerializeField] bool isFiring = false;
    [SerializeField] Transform muzzleFlashPos;
    [SerializeField] ParticleSystem muzzleFlash;

    ThirdPersonShooterController thirdPersonController;
    Ray ray;
    RaycastHit hitInfo;

    Vector3 startPoint;
    Vector3 destination;

    private void Awake()
    {

        shootAction = playerInput.actions["Shoot"];
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

    private void Shoot(InputAction.CallbackContext obj)
    {
        Debug.Log("Shooting, Pew Pew");
    }

    private void ShootGun()
    {
        Debug.Log("Pew Pew");
        ParticleSystem go = Instantiate(muzzleFlash, muzzleFlashPos.position, Quaternion.identity);
        go.Emit(1);
    }

    private void Update()
    {

        ray.origin = raycastOrigin.position;
        startPoint = ray.origin;
        ray.direction = raycastDestination.position - raycastOrigin.position;
        destination = ray.direction;
    }

    private void StartFiring(Vector3 _origin, Vector3 _destination)
    {
        Debug.Log("Pew Pew");

        if (Physics.Raycast(ray, out hitInfo, deculLayer))
        {
            Debug.DrawLine(ray.origin, hitInfo.point, Color.red, 1.0f);
            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(1); // how many particles we want to spawn

        }

    }
}
