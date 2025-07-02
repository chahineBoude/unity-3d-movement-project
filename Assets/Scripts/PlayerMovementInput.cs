using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-2)]
public class PlayerMovementInput : MonoBehaviour, PlayerInputActions.IMovementActions
{
    public PlayerInputActions PlayerInputActions { get; private set; }
    public Vector2 MovementVector { get; private set; }
    public Vector2 LookVector { get; private set; }

    [SerializeField] private bool holdToSprint = true;

    public bool SprintToggleOn { get; private set; }

    public bool JumpPressed { get; private set; }

    private void OnEnable()
    {
        PlayerInputActions = new PlayerInputActions();
        PlayerInputActions.Enable();
        PlayerInputActions.Movement.Enable();
        PlayerInputActions.Movement.SetCallbacks(this);
    }

    private void OnDisable()
    {
        PlayerInputActions.Movement.Disable();
        PlayerInputActions.Movement.RemoveCallbacks(this);
    }

    private void LateUpdate()
    {
        JumpPressed = false;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MovementVector = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        LookVector = context.ReadValue<Vector2>();
    }

    public void OnSprintToggle(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SprintToggleOn = holdToSprint || !SprintToggleOn;
        }
        else if (context.canceled)
        {
            SprintToggleOn = !holdToSprint && SprintToggleOn;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        JumpPressed = true;
    }
}
