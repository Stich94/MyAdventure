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
    [SerializeField] Canvas thirdPersonCanvas;
    [SerializeField] Canvas aimCanvas;
    [SerializeField] float normalSensitivity = 5f;
    [SerializeField] float aimSensitivity = 0.5f;
    [SerializeField] CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] LayerMask aimColliderMask;
    [SerializeField] bool isAiming = false;
    [SerializeField] Camera cam;
    public bool IsAiming { get { return isAiming; } }
    InputAction aimAction;
    InputAction shootAction;
    PlayerController playerController;
    [SerializeField] Transform bulletPrefab;
    [SerializeField] Transform bulletSpawnPoint;
    Vector3 mouseWorldPosition;
    Transform hitTransform;
    Animator animator;

    [Header("Rig Settings")]

    [SerializeField] Rig bodyRigLayer;
    [SerializeField] float bodyRigValue = 0f;
    [SerializeField] float bodyduration = 0.3f; // how long it takes before we aim
    [SerializeField] Rig aimRigLayer;
    [SerializeField] float aimRigValue = 0f;
    [SerializeField] float aimDuration = 0.3f;

    [Header("Current Active Player Weapon")]
    [SerializeField] RayCastWeapon activeWeapon;


    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        // virtualCamera = GetComponent<CinemachineVirtualCamera>();
        aimAction = playerInput.actions["Aim"];
        // shootAction = playerInput.actions["Shoot"];
        animator = GetComponent<Animator>();
    }

    // add the events
    private void OnEnable()
    {
        aimAction.performed += _ => StartAim();
        // shootAction.performed += _ => ShootGun();
        aimAction.canceled += _ => CancelAim();
    }

    // unsubscribe from this events
    private void OnDisable()
    {
        aimAction.performed += _ => StartAim();
        // shootAction.performed -= _ => ShootGun();
        aimAction.canceled -= _ => CancelAim();
    }

    private void StartAim()
    {
        isAiming = true;
        aimVirtualCamera.gameObject.SetActive(true);
        aimVirtualCamera.Priority += priorityBoostAmount;
        // aimCanvas.enabled = true;

        // thirdPersonCanvas.enabled = false;
    }

    private void CancelAim()
    {
        isAiming = false;
        aimVirtualCamera.Priority -= priorityBoostAmount;
        aimVirtualCamera.gameObject.SetActive(false);
        // aimCanvas.enabled = false;

        // thirdPersonCanvas.enabled = true;
    }

    private void Update()
    {
        // rotate the player
        // mouseWorldPosition = Vector3.zero;
        // // hipoint in center of screen
        // Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        // Ray ray = cam.ScreenPointToRay(screenCenterPoint);
        // hitTransform = null;
        // if (Physics.Raycast(ray, out RaycastHit hit, 999f, aimColliderMask))
        // {
        //     mouseWorldPosition = hit.point;
        //     hitTransform = hit.transform;
        // }
        if (isAiming)
        {
            aimRigLayer.weight += Time.deltaTime / aimDuration;
            bodyRigLayer.weight = 1f;
            playerController.TargetSpeed = aimMovementSpeed;
            playerController.SetSensitivity(aimSensitivity);
            playerController.SetRotateOnMove(false);
            // Vector3 worldAimTarget = mouseWorldPosition;
            // worldAimTarget.y = transform.position.y;
            // Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
            // transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));

        }
        else
        {
            aimRigLayer.weight -= Time.deltaTime / aimDuration;
            bodyRigLayer.weight = 0f;
            playerController.SetSensitivity(normalSensitivity);
            playerController.SetRotateOnMove(true);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
        }
    }

    private void ShootGun()
    {

        Debug.Log("is shooting");
        // // Raycast Hit Method
        // if (hitTransform != null)
        // {
        //     if (hitTransform.GetComponent<IDamageAble>() != null)
        //     {
        //         // hit target
        //         // Instantiate vfxHit green
        //     }
        //     else
        //     {
        //         // hit something else
        //         // instantiate vfx Hit red
        //     }
        // }
        // // instantiate real projectiles
        // Vector3 aimDir = (mouseWorldPosition - bulletSpawnPoint.position).normalized;
        // Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.LookRotation(aimDir, Vector3.up)); // was Vector3.up - before change
        // // Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        // #region - shoot with raycast - disabled
        //     RaycastHit hit;
        //     GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        //     BulletController bulletController = bullet.GetComponent<BulletController>();
        //     if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity))
        //     {
        //         bulletController.target = hit.point;
        //         bulletController.hit = true;
        //     }
        //     else
        //     {
        //         bulletController.target = cameraTransform.position + cameraTransform.forward * bulletMissDistance;
        //         bulletController.hit = false;
        //     }
        // }
        // #endregion

    }
}

