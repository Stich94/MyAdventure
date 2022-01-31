
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class SwitchVCam : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] int priorityBoostAmount = 10;
    [SerializeField] Canvas thirdPersonCanvas;
    [SerializeField] Canvas aimCanvas;

    CinemachineVirtualCamera virtualCamera;
    InputAction aimAction;

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
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
        virtualCamera.Priority += priorityBoostAmount;
        aimCanvas.enabled = true;
        thirdPersonCanvas.enabled = false;
    }

    private void CancelAim()
    {
        virtualCamera.Priority -= priorityBoostAmount;
        aimCanvas.enabled = false;
        thirdPersonCanvas.enabled = true;
    }
}
