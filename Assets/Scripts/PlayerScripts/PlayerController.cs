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
    [SerializeField] float targetRotation = 0.0f;
    [SerializeField] float RotationSmoothTime = 0.12f;
    [SerializeField] float SpeedChangeRate = 10.0f;

    [SerializeField] ThirdPersonShooterController aimController;
    float rotationVelocity;
    float targetSpeed;

    public float TargetSpeed { get { return targetSpeed; } set { targetSpeed = value; } }
    float speed;

    // Camera
    [Header("Camera")] float cinemachineTargetYaw;
    [SerializeField] GameObject cinemaCameraTarget;
    [SerializeField] float cameraRotationSensitivity = 1.0f;
    [SerializeField] float CameraAngleOverride = 0.0f;
    [SerializeField] float bottomClamp = -30f;
    [SerializeField] float topClamp = 70f;
    float cinemachineTargetPItch;
    float lookTreshold = 0.01f;
    bool rotateOnMove = true;
    bool groundedPlayer;
    bool sprint = false;

    Transform cameraTransform;

    //controls
    PlayerInput playerInput;
    CharacterController controller;
    Vector3 playerVelocity;
    InputAction moveAction;
    InputAction jumpAction;
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
    float animationBlend;
    Vector2 currentAnimationBlendVector;
    Vector2 animationVelcity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        aimController = GetComponent<ThirdPersonShooterController>();
        playerInput = GetComponent<PlayerInput>();
        cameraTransform = Camera.main.transform;
        // access the controls
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
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
        // NewMove();
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
        if (aimController.IsAiming)
        {
            targetSpeed = 2f;
        }

        Vector2 input = moveAction.ReadValue<Vector2>();
        if (input == Vector2.zero) targetSpeed = 0.0f;

        // Vector3 move = new Vector3(input.x, 0, input.y); // this is the movement, without smoothing the animations
        // - smooth Animations and movement together
        currentAnimationBlendVector = Vector2.SmoothDamp(currentAnimationBlendVector, input, ref animationVelcity, animationSmoothTime);
        Vector3 move = new Vector3(currentAnimationBlendVector.x, 0, currentAnimationBlendVector.y);


        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;
        move.y = 0f;

        Vector3 inputDirection = new Vector3(input.x, 0, input.y).normalized;

        // Player Rotation
        if (input != Vector2.zero)
        {
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, RotationSmoothTime);

            // rotate only when moving
            if (rotateOnMove)
            {
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                // Debug.Log("rotation value" + rotation);

            }
        }

        // Move Player always on Global Coordinates
        Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

        controller.Move(targetDirection.normalized * (targetSpeed * Time.deltaTime) + new Vector3(0.0f, playerVelocity.y, 0.0f) * Time.deltaTime);

        // - smooth Animation with movement together
        animator.SetFloat(moveXAnimationParameterId, currentAnimationBlendVector.x);
        animator.SetFloat(moveZAnimationParameterId, currentAnimationBlendVector.y);

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
        cinemachineTargetPItch = ClampAngle(cinemachineTargetPItch, bottomClamp, topClamp);

        cinemaCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPItch + CameraAngleOverride, cinemachineTargetYaw, 0.0f);
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
