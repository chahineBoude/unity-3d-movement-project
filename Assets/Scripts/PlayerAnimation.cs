using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float movementBlendSpeed = 4f;
    private PlayerMovementInput _movementInput;
    private PlayerState _playerState;
    private PlayerController _playerController;

    private static int inputXHash = Animator.StringToHash("InputX");
    private static int inputYHash = Animator.StringToHash("InputY");
    private static int inputMagnitudeHash = Animator.StringToHash("InputMagnitude");
    private static int isGroundedHash = Animator.StringToHash("IsGrounded");
    private static int isFallingHash = Animator.StringToHash("IsFalling");
    private static int isJumpingHash = Animator.StringToHash("IsJumping");
    private static int rotationMismatchHash = Animator.StringToHash("RotationMismatch");
    private static int isIdlingHash = Animator.StringToHash("IsIdling");
    private static int IsRotatingToTargetHash = Animator.StringToHash("IsRotatingToTarget");
    private Vector3 _currentBlendInput = Vector3.zero;

    private void Awake()
    {
        _movementInput = GetComponent<PlayerMovementInput>();
        _playerState = GetComponent<PlayerState>();
        _playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        bool isSprinting = _playerState.CurrentPlayerState == PlayerStateEnum.Sprinting;
        bool isJumping = _playerState.CurrentPlayerState == PlayerStateEnum.Jumping;
        bool isIdling = _playerState.CurrentPlayerState == PlayerStateEnum.Idling;
        bool isRunning = _playerState.CurrentPlayerState == PlayerStateEnum.Running;
        bool isFalling = _playerState.CurrentPlayerState == PlayerStateEnum.Falling;
        bool isGrounded = _playerState.IsGroundedState();

        Vector2 inputTarget = isSprinting ? _movementInput.MovementVector * 1.5f :
                                isRunning ? _movementInput.MovementVector * 1f :
                                _movementInput.MovementVector * 0.5f;
        _currentBlendInput = Vector3.Lerp(_currentBlendInput, inputTarget, movementBlendSpeed * Time.deltaTime);
        _animator.SetFloat(inputXHash, _currentBlendInput.x);
        _animator.SetFloat(inputYHash, _currentBlendInput.y);
        _animator.SetFloat(inputMagnitudeHash, _currentBlendInput.magnitude);
        _animator.SetBool(isJumpingHash, isJumping);
        _animator.SetBool(isGroundedHash, isGrounded);
        _animator.SetBool(isFallingHash, isFalling);
        _animator.SetFloat(rotationMismatchHash, _playerController.RotationMismatch);
        _animator.SetBool(isIdlingHash, isIdling);
        _animator.SetBool(IsRotatingToTargetHash, _playerController.IsRotatingToTarget);
    }

}
