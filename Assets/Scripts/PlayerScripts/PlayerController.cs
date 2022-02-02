using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float playerSpeed = 2.0f;
    [SerializeField] float sprintSpeed = 5.0f;
    [SerializeField] float jumpHeight = 1.0f;
    [SerializeField] float gravityValue = -9.81f;
    [SerializeField] float rotationSpeed = 5f;

    // Camera
    [Header("Look Around")] float cinemachineTargetYaw;
    [SerializeField] float cameraRotationSensitivity = 1.0f;
    [SerializeField] float CameraAngleOverride = 0.0f;
    float cinemachineTargetPItch;
    float lookTreshold = 0.01f;
    bool rotateOnMove = true;

    // Combat
    [Header("Combat")] [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform barrelTransform;
    [SerializeField] Transform bulletParent;
    [SerializeField] float bulletMissDistance = 25f;
    bool groundedPlayer;
    bool sprint = false;
    Transform cameraTransform;
    PlayerInput playerInput;
    CharacterController controller;
    Vector3 playerVelocity;

    //controls
    InputAction moveAction;
    InputAction jumpAction;
    InputAction shootAction;
    InputAction sprintActon;
    InputAction lookAction;

    // Animator
    [Header("Animations")] [SerializeField] Animator animator;
    [SerializeField] float animationSmoothTime = 0.1f; // the lower the value, the faster is the transition
    [SerializeField] float animationPlayTransition = 0.15f;
    [SerializeField] Transform aimTarget;
    [SerializeField] float aimDistance = 1f;
    int moveXAnimationParameterId;
    int moveZAnimationParameterId;
    int jumpAnimation;

    Vector2 currentAnimationBlendVector;
    Vector2 animationVelcity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        cameraTransform = Camera.main.transform;
        // access the controls
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
        shootAction = playerInput.actions["Shoot"];
        sprintActon = playerInput.actions["Sprint"];
        Cursor.lockState = CursorLockMode.Locked;

        //Animations
        animator = GetComponent<Animator>();
        jumpAnimation = Animator.StringToHash("Pistol Jump");
        moveXAnimationParameterId = Animator.StringToHash("MoveX");
        moveZAnimationParameterId = Animator.StringToHash("MoveZ");
    }

    private void Update()
    {
        // UpdatePlayerRig();
        Move();
        // RotatePlayer();
        Jump();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void Move()
    {
        groundedPlayer = controller.isGrounded;
        // Check if the player is sprint


        float sprintInput = sprintActon.ReadValue<float>();
        float targetSpeed = sprintInput >= 1 ? targetSpeed = sprintSpeed : targetSpeed = playerSpeed;

        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }


        Vector2 input = moveAction.ReadValue<Vector2>();

        // Vector3 move = new Vector3(input.x, 0, input.y); // this is the movement, without smoothing the animations
        // - smooth Animations and movement together
        currentAnimationBlendVector = Vector2.SmoothDamp(currentAnimationBlendVector, input, ref animationVelcity, animationSmoothTime);
        Vector3 move = new Vector3(currentAnimationBlendVector.x, 0, currentAnimationBlendVector.y);


        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;
        move.y = 0f;
        controller.Move(move * Time.deltaTime * targetSpeed);

        //Blend Strafe Animations - normal
        // animator.SetFloat(moveXAnimationParameterId, input.x);
        // animator.SetFloat(moveZAnimationParameterId, input.y);

        // - smooth Animation with movement together
        animator.SetFloat(moveXAnimationParameterId, currentAnimationBlendVector.x);
        animator.SetFloat(moveZAnimationParameterId, currentAnimationBlendVector.y);

        // RotatePlayer();
        //TODO - Temp disabled Player Rotation
        // // Rotate Player towards aim/camera direction
        if (rotateOnMove)
        {
            // if (move != Vector2.zero)
            // {
            //TODO - rotate player when move
            // }
            // float targetAngle = cameraTransform.eulerAngles.y; // this returns the cameras current y rotation
            // Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
            // transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void Jump()
    {
        // Note: u have to set the Min Move Distance on the Character Controller to 0, otherwise the player will not jump
        if (jumpAction.triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            // animator.SetTrigger(jumpAnimation); // this requires a trigger in the animator GUI 
            animator.CrossFade(jumpAnimation, animationPlayTransition); // to make a smooth transition between jump and movement
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    // add shot event to player 
    private void OnEnable()
    {
        shootAction.performed += _ => ShootGun();
    }

    private void OnDisable()
    {
        shootAction.performed -= _ => ShootGun();
    }

    private void ShootGun()
    {
        RaycastHit hit;
        GameObject bullet = Instantiate(bulletPrefab, barrelTransform.position, Quaternion.identity, bulletParent);
        BulletController bulletController = bullet.GetComponent<BulletController>();
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity))
        {
            bulletController.target = hit.point;
            bulletController.hit = true;
        }
        else
        {
            bulletController.target = cameraTransform.position + cameraTransform.forward * bulletMissDistance;
            bulletController.hit = false;
        }
    }

    private void UpdatePlayerRig()
    {
        aimTarget.position = cameraTransform.position + cameraTransform.forward * aimDistance;
    }

    private void SetCursorState(bool _newState)
    {
        Cursor.lockState = _newState ? CursorLockMode.Locked : CursorLockMode.None;
    }

    // public void SetSensitivity(float _newSensitivity)
    // {
    //     sensitivity = _newSensitivity;
    // }
    private void CameraRotation()
    {

        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        if (lookInput.sqrMagnitude >= lookTreshold)
        {
            cinemachineTargetYaw += lookInput.x * Time.deltaTime * cameraRotationSensitivity;
            cinemachineTargetPItch += lookInput.y * Time.deltaTime * cameraRotationSensitivity;
        }

        cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
        cinemachineTargetPItch = ClampAngle(cinemachineTargetPItch, float.MinValue, float.MaxValue);

        cameraTransform.transform.rotation = Quaternion.Euler(cinemachineTargetPItch + CameraAngleOverride, cinemachineTargetYaw, 0.0f);

    }

    private static float ClampAngle(float _lAngle, float _lMinValue, float _lMaxValue)
    {
        if (_lAngle < -360f) _lAngle += 360f;
        if (_lAngle > 360f) _lAngle -= 360f;
        return Mathf.Clamp(_lAngle, _lMinValue, _lMaxValue);
    }

    public void SetSensitivity(float _newSensitivity)
    {
        cameraRotationSensitivity = _newSensitivity;
    }

    public void SetRotateOnMove(bool _newRotateOnMove)
    {
        rotateOnMove = _newRotateOnMove;
    }
}
