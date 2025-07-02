using UnityEngine;

public class PlayerState : MonoBehaviour
{
    [field: SerializeField] public PlayerStateEnum CurrentPlayerState { get; private set; } = PlayerStateEnum.Idling;

    public void SetCurrentPlayerState(PlayerStateEnum playerStateEnum) => CurrentPlayerState = playerStateEnum;

    public bool IsGroundedState()
    {
        return CurrentPlayerState == PlayerStateEnum.Idling
            || CurrentPlayerState == PlayerStateEnum.Walking
            || CurrentPlayerState == PlayerStateEnum.Running
            || CurrentPlayerState == PlayerStateEnum.Sprinting;
    }

}
public enum PlayerStateEnum
{
    Idling = 0,
    Walking = 1,
    Running = 2,
    Sprinting = 3,
    Jumping = 4,
    Falling = 5,
    Strafing = 6
}