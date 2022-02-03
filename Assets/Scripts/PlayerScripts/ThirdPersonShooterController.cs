using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class ThirdPersonShooterController : MonoBehaviour
{
    //TODO - when aiming reduce movementSpeed
    [SerializeField] PlayerInput playerInput;
    [SerializeField] int priorityBoostAmount = 10;
    [SerializeField] Canvas thirdPersonCanvas;
    [SerializeField] Canvas aimCanvas;
    [SerializeField] float normalSensitivity = 5f;
    [SerializeField] float aimSensitivity = 0.5f;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] LayerMask aimColliderMask;
    [SerializeField] Transform debugTransform;
    [SerializeField] bool isAiming = false;
    [SerializeField] Camera cam;
    public bool IsAiming { get { return isAiming; } }
    InputAction aimAction;
    PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        // virtualCamera = GetComponent<CinemachineVirtualCamera>();
        aimAction = playerInput.actions["Aim"];
    }

    // add the events
    private void OnEnable()
    {
        aimAction.performed += _ => StartAim();
        aimAction.canceled += _ => CancelAim();
    }

    // unsubscribe from this events
    private void OnDisable()
    {
        aimAction.performed += _ => StartAim();
        aimAction.canceled -= _ => CancelAim();
    }

    private void StartAim()
    {
        isAiming = true;
        virtualCamera.Priority += priorityBoostAmount;
        aimCanvas.enabled = true;

        thirdPersonCanvas.enabled = false;
    }

    private void CancelAim()
    {
        isAiming = false;
        virtualCamera.Priority -= priorityBoostAmount;
        aimCanvas.enabled = false;

        thirdPersonCanvas.enabled = true;
    }

    private void Update()
    {
        // rotate the player
        Vector3 mouseWorldPosition = Vector3.zero;
        // hipoint in center of screen
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = cam.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit hit, 999f, aimColliderMask))
        {
            debugTransform.position = hit.point;
            mouseWorldPosition = hit.point;
        }
        if (isAiming)
        {
            playerController.SetSensitivity(aimSensitivity);
            playerController.SetRotateOnMove(false);
            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }
        else
        {
            playerController.SetSensitivity(normalSensitivity);
            playerController.SetRotateOnMove(true);
        }
    }

}
