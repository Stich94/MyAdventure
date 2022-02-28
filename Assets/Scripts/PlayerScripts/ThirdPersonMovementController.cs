using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonMovementController : MonoBehaviour
{
    [SerializeField] float playerSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] bool isSprinting = false;
    public bool IsSprinting => isSprinting;
    [SerializeField] float playerRotationSmoothTime;
    [SerializeField] float speedChangeRate;
    [SerializeField] float jumpHeight;
    [SerializeField] float gravity;
    [SerializeField] float jumptimeOut;
    [SerializeField] float fallTimeOut;
    [SerializeField] bool grounded;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundedOffset = -0.14f;
    [SerializeField] float groundedRadius = 0.28f;
    static Transform playerPos;
    public Transform GetPlayerPos { get; }

    bool canJump = true;
    bool rotateOnMove = true;


    [Header("Cinemachine Settings")]
    [SerializeField] GameObject cinemachineCameraTarget;
    [SerializeField] float sensitivity = 1f;


    float TopClamp = 70.0f;
    float BottomClamp = -30.0f;
    float CameraAngleOverride = 0.0f;
    bool LockCameraPosition = false;

    // cinemachine
    float cinemachineTargetYaw;
    float cinemachineTargetPitch;

    // player
    float speed;
    float animationBlend;
    float targetRotation = 0.0f;
    float rotationVelocity;
    float verticalVelocity;
    float terminalVelocity = 53.0f;
    const float Treshhold = 0.01f;

    // timeout deltatime
    float jumpTimeoutDelta;
    float fallTimeoutDelta;

    //controls
    Vector3 playerVelocity;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction sprintActon;
    InputAction lookAction;
    CharacterController controller;
    // StarterAssetsInputs _input;
    Camera mainCamera;
    PlayerInput playerInput;


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

    // animation IDs
    int animSpeedID;
    int animGroundID;
    int animJumpID;
    int animFreeFallID;
    int animMotionSpeedID;


    void Awake()
    {
        controller = GetComponent<CharacterController>();
        // aimController = GetComponent<ThirdPersonShooterController>();
        playerInput = GetComponent<PlayerInput>();
        mainCamera = Camera.main;

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

        // reset our timeouts on start
        jumpTimeoutDelta = jumptimeOut;
        fallTimeoutDelta = fallTimeOut;
        Soundmanager.Initialize();
    }

    void Start()
    {
        SetAnimationIDs();
    }


    void Update()
    {
        Jump();
        GroundCheck();
        Move();
    }


    void LateUpdate()
    {
        CameraRotation();
    }


    void SetAnimationIDs()
    {
        animSpeedID = Animator.StringToHash("Speed");
        animGroundID = Animator.StringToHash("Grounded");
        animJumpID = Animator.StringToHash("Jump");
        animFreeFallID = Animator.StringToHash("FreeFall");
        animMotionSpeedID = Animator.StringToHash("MotionSpeed");
    }


    void GroundCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
        grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayer, QueryTriggerInteraction.Ignore);

        animator.SetBool(animGroundID, grounded);
    }

    void Jump()
    {
        if (grounded)
        {
            canJump = true;
            fallTimeoutDelta = fallTimeOut;

            animator.SetBool(animJumpID, false);
            animator.SetBool(animFreeFallID, false);

            if (verticalVelocity < 0.0f)
            {
                verticalVelocity = -2f;
            }

            // jump 
            if (jumpAction.triggered && jumpTimeoutDelta <= 0.0f)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

                animator.SetBool(animJumpID, true);
            }

            // jump time out
            if (jumpTimeoutDelta >= 0.0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            jumpTimeoutDelta = jumptimeOut;

            // fall timeout
            if (fallTimeoutDelta >= 0.0f)
            {
                fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                animator.SetBool(animFreeFallID, true);
            }

            // if we are not grounded, do not jump
            canJump = false;
        }

        // apply gravity
        if (verticalVelocity < terminalVelocity)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    private void Move()
    {

        float sprintInput = sprintActon.ReadValue<float>();
        float targetSpeed = sprintInput >= 1 ? targetSpeed = sprintSpeed : targetSpeed = playerSpeed;

        // check if the player is sprinting
        isSprinting = sprintInput >= 1 ? true : false;

        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        if (moveInput == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = 1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {

            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);

            // round speed to 3 decimal places
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else
        {
            speed = targetSpeed;
        }
        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);

        // normalise input direction
        Vector3 inputDirection = new Vector3(moveInput.x, 0.0f, moveInput.y).normalized;

        // handle target rotation
        if (moveInput != Vector2.zero)
        {
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, playerRotationSmoothTime);

            // rotate to face input direction relative to camera position
            // if it is false - player is not aiming
            if (rotateOnMove)
            {
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }


        Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

        // move the player
        controller.Move(targetDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);

        // update animator if using character
        animator.SetFloat(animSpeedID, animationBlend);
        animator.SetFloat(animMotionSpeedID, inputMagnitude);
        if (speed >= 0.05f)
        {
            // TODO - SoundManger disabled temp
            // Soundmanager.PlaySound(Soundmanager.Sound.PlayerMove, this.transform.position);
        }

        playerPos = this.transform;
    }


    void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        if (lookInput.sqrMagnitude >= Treshhold && !LockCameraPosition)
        {
            cinemachineTargetYaw += lookInput.x * Time.deltaTime;
            cinemachineTargetPitch += lookInput.y * Time.deltaTime;
        }

        // clamp our rotations so our values are limited 360 degrees
        cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        cinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch + CameraAngleOverride, cinemachineTargetYaw, 0.0f);
    }

    static float ClampAngle(float _lAngle, float _lfMin, float _lfMax)
    {
        if (_lAngle < -360f) _lAngle += 360f;
        if (_lAngle > 360f) _lAngle -= 360f;
        return Mathf.Clamp(_lAngle, _lfMin, _lfMax);
    }


    public void SetSensitivity(float _newSensitivity)
    {
        sensitivity = _newSensitivity;
    }

    public void SetRotateOnMove(bool _newRotateOnMove)
    {
        rotateOnMove = _newRotateOnMove;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }


}
