using UnityEngine;

[DefaultExecutionOrder(-1)]
public class PlayerController : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] CharacterController _characterController;
    [SerializeField] Camera _camera;
    public float RotationMismatch { get; private set; } = 0f;
    public bool IsRotatingToTarget { get; private set; } = false;

    [Header("Movement Settings")]
    public float runAcceleration = 0.25f;
    public float runSpeed = 4f;
    public float drag = 0.1f;
    public float sprintAcceleration = 0.5f;
    public float sprintSpeed = 7f;
    public float jumpSpeed = 1f;
    public float gravity = 25f;

    [Header("Animation Settings")]
    public float playerRotationSpeed = 10f;
    public float rotateToTargetTime = 0.25f;

    [Header("Camera Settings")]
    public float lookSensitivityH = 0.1f;
    public float lookSensitivityV = 0.1f;
    public float lookLimitV = 80f;

    private PlayerMovementInput _playerMovementInput;
    private PlayerState _playerState;
    private Vector2 _cameraRotation = Vector2.zero;
    public Vector2 _playerTargetRotation = Vector2.zero;
    private float verticalVelocity = 0f;
    private float _rotatingToTargetTimer = 0f;

    private void Awake()
    {
        _playerMovementInput = GetComponent<PlayerMovementInput>();
        _playerState = GetComponent<PlayerState>();
    }

    private void Update()
    {
        UpdateMovementState();
        HandleVerticalMovement();
        HandleLateralMovement();
    }

    private void UpdateMovementState()
    {
        bool isMovementPressed = _playerMovementInput.MovementVector != Vector2.zero;
        bool isMovingLaterally = IsMovingLaterally();
        bool isSprinting = _playerMovementInput.SprintToggleOn && isMovingLaterally;
        bool isGrounded = IsGrounded();

        PlayerStateEnum lateralState = isSprinting ? PlayerStateEnum.Sprinting :
                                        isMovingLaterally || isMovementPressed ? PlayerStateEnum.Running : PlayerStateEnum.Idling;
        _playerState.SetCurrentPlayerState(lateralState);

        if (!isGrounded && _characterController.velocity.y > 0f)
        {
            _playerState.SetCurrentPlayerState(PlayerStateEnum.Jumping);
        }
        else if (!isGrounded && _characterController.velocity.y < 0f)
        {
            _playerState.SetCurrentPlayerState(PlayerStateEnum.Falling);
        }

    }

    private void HandleVerticalMovement()
    {
        bool isGrounded = _playerState.IsGroundedState();
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = 0f;
        }
        verticalVelocity -= gravity * Time.deltaTime;

        if (_playerMovementInput.JumpPressed && isGrounded)
        {
            verticalVelocity += Mathf.Sqrt(jumpSpeed * 3 * gravity);
        }

    }

    private void HandleLateralMovement()
    {
        bool isSprinting = _playerState.CurrentPlayerState == PlayerStateEnum.Sprinting;
        bool isGrounded = _playerState.IsGroundedState();

        float lateralAcceleration = isSprinting ? sprintAcceleration : runAcceleration;
        float lateralSpeed = isSprinting ? sprintSpeed : runSpeed;

        Vector3 cameraForward = new Vector3(_camera.transform.forward.x, 0f, _camera.transform.forward.z).normalized;
        Vector3 cameraRight = new Vector3(_camera.transform.right.x, 0f, _camera.transform.right.z).normalized;

        Vector3 moveDir = cameraRight * _playerMovementInput.MovementVector.x + cameraForward * _playerMovementInput.MovementVector.y;
        Vector3 movementDelta = moveDir * lateralAcceleration * Time.deltaTime;
        Vector3 newVelocity = _characterController.velocity + movementDelta;

        Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;

        newVelocity = newVelocity.magnitude > drag * Time.deltaTime ? newVelocity - currentDrag : Vector3.zero;
        newVelocity = Vector3.ClampMagnitude(newVelocity, lateralSpeed);
        newVelocity.y = verticalVelocity;

        _characterController.Move(newVelocity * Time.deltaTime);
    }

    private bool IsMovingLaterally()
    {
        Vector3 lateralVelocity = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.z);
        return lateralVelocity.magnitude > 0.01;
    }

    private bool IsGrounded()
    {
        return _characterController.isGrounded;
    }

    private void LateUpdate()
    {
        UpdateCameraRotation();
    }

    public void UpdateCameraRotation()
    {
        _cameraRotation.x += lookSensitivityH * _playerMovementInput.LookVector.x;
        _cameraRotation.y = Mathf.Clamp(_cameraRotation.y - lookSensitivityV * _playerMovementInput.LookVector.y, -lookLimitV, lookLimitV);

        _playerTargetRotation.x += transform.eulerAngles.x + lookSensitivityH * _playerMovementInput.LookVector.x;
        bool isIdling = _playerState.CurrentPlayerState == PlayerStateEnum.Idling;
        IsRotatingToTarget = _rotatingToTargetTimer > 0;
        if (!isIdling || Mathf.Abs(RotationMismatch) > 90f || IsRotatingToTarget)
        {
            Quaternion targetRotationX = Quaternion.Euler(0f, _playerTargetRotation.x, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotationX, playerRotationSpeed * Time.deltaTime);
            if (Mathf.Abs(RotationMismatch) > 90f)
            {
                _rotatingToTargetTimer = rotateToTargetTime;
            }
            _rotatingToTargetTimer -= Time.deltaTime;
        }
        _camera.transform.rotation = Quaternion.Euler(_cameraRotation.y, _cameraRotation.x, 0f);
    }

}
