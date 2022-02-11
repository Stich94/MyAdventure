using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;

public class ThirdPersonShooterController : MonoBehaviour
{
    //TODO - when aiming reduce movementSpeed
    [SerializeField] PlayerInput playerInput;
    [SerializeField] float aimMovementSpeed = 2f;
    [SerializeField] int priorityBoostAmount = 10;
    [SerializeField] float normalSensitivity = 5f;
    [SerializeField] float aimSensitivity = 0.5f;
    [SerializeField] CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] LayerMask aimColliderMask = new LayerMask();
    [SerializeField] Transform DebugTransform;
    [SerializeField] bool isAiming = false;
    [SerializeField] Camera cam;
    public bool IsAiming { get { return isAiming; } }
    InputAction aimAction;
    InputAction shootAction;
    ThirdPersonMovementController playerController;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawnPoint;
    Vector3 mouseWorldPosition;
    Transform hitTransform;
    Animator animator;

    [Header("Rig Settings")]
    [SerializeField] Rig aimRigLayer;
    [SerializeField] float aimRigWeight = 0f;
    [SerializeField] float aimDuration = 0.3f;

    [Header("Current Active Player Weapon")]
    [SerializeField] RayCastWeapon activeWeapon;


    void Awake()
    {
        playerController = GetComponent<ThirdPersonMovementController>();
        aimAction = playerInput.actions["Aim"];
        shootAction = playerInput.actions["Shoot"];
        animator = GetComponent<Animator>();
    }

    // add the events
    void OnEnable()
    {
        aimAction.performed += _ => StartAim();
        shootAction.performed += _ => ShootGun();
        aimAction.canceled += _ => CancelAim();
    }

    // unsubscribe from this events
    void OnDisable()
    {
        aimAction.performed -= _ => StartAim();
        shootAction.performed -= _ => ShootGun();
        aimAction.canceled -= _ => CancelAim();
    }

    void StartAim()
    {
        isAiming = true;
        aimVirtualCamera.gameObject.SetActive(true);
        aimVirtualCamera.Priority += priorityBoostAmount;

        playerController.SetSensitivity(aimSensitivity);
        aimRigWeight = 1f;

    }

    void CancelAim()
    {
        isAiming = false;
        aimVirtualCamera.Priority -= priorityBoostAmount;
        aimVirtualCamera.gameObject.SetActive(false);

        playerController.SetSensitivity(normalSensitivity);
        aimRigWeight = 0f;

    }

    void Update()
    {
        UpdatePlayerRotation();
        UpdateAimRig();
    }

    void UpdatePlayerRotation()
    {
        mouseWorldPosition = Vector3.zero;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        hitTransform = null;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderMask))
        {
            DebugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
            hitTransform = raycastHit.transform;
        }

        if (isAiming) // this must be calculated in update - event does not work for it
        {
            playerController.SetRotateOnMove(false);
            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - playerController.transform.position).normalized;

            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));


            // // rotate the player towards aimDirection
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }
        else
        {
            playerController.SetRotateOnMove(true);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));


        }
    }

    void ShootGun()
    {

        // instantiate real bullets
        if (isAiming)
        {
            Debug.Log("is shooting");
            Vector3 aimDir = (mouseWorldPosition - bulletSpawnPoint.position).normalized;
            Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.LookRotation(aimDir, Vector3.up));
        }



        // // Raycast Hit Method
        if (hitTransform != null)
        {
            if (hitTransform.GetComponent<IDamageAble>() != null)
            {
                // hit target
                // Instantiate vfxHit green
                // Instantiate(vfxHit, transform.position, Quaternion.identity);
            }
            else
            {
                // hit something else
                // instantiate vfx Hit red
            }
        }

    }

    void UpdateAimRig()
    {
        aimRigLayer.weight = Mathf.Lerp(aimRigLayer.weight, aimRigWeight, Time.deltaTime * 20f);
    }




}

