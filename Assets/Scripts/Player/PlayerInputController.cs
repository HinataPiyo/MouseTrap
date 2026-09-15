using Player;
using UnityEngine;
using UnityEngine.InputSystem;

// 基本None
public enum StateType
{ None = -1, PlayerInfo, Movie, Talk, Interact, RoomChange }

public interface IInputTrigger
{
    public event System.Action OnInputEKeyPressed;
    public event System.Action OnInputTabKeyPressed;
    public event System.Action OnInputSpaceKeyPressed;
    public event System.Action OnInputESCKeyDisabled;
}

public interface IGameState
{
    public StateType CurrentState { get; }
    public void ChangeState(StateType newState);
}

public class PlayerInputController : MonoBehaviour, IInputTrigger, IGameState
{
    ICharacterState characterControlHandler;
    public StateType CurrentState { get; private set; } = StateType.None;

    public void ChangeState(StateType newState) => CurrentState = newState;

    public event System.Action OnInputEKeyPressed;
    public event System.Action OnInputTabKeyPressed;
    public event System.Action OnInputSpaceKeyPressed;
    public event System.Action OnInputESCKeyDisabled;

    void Awake()
    {
        ApiProvider.Register<IInputTrigger>(this);
        ApiProvider.Register<IGameState>(this);

        ChangeState(StateType.None);
    }

    void Start()
    {
        characterControlHandler = ApiProvider.Get<ICharacterState>();

        OnInputESCKeyDisabled += () => ChangeState(StateType.None);
    }

    public void Update()
    {
        Debug.Log(CurrentState);

        switch (CurrentState)
        {
            case StateType.RoomChange:
                if (Keyboard.current.escapeKey.wasPressedThisFrame)
                    OnInputESCKeyDisabled?.Invoke();
                return;

            case StateType.Talk:
                if (Keyboard.current.spaceKey.wasPressedThisFrame)
                    OnInputSpaceKeyPressed?.Invoke();
                return;

            case StateType.Interact:
                if (Keyboard.current.eKey.wasPressedThisFrame)
                    OnInputEKeyPressed?.Invoke();
                return;

            case StateType.None:
                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    OnInputEKeyPressed?.Invoke();
                    return;
                }
                if (Keyboard.current.tabKey.wasPressedThisFrame)
                {
                    OnInputTabKeyPressed?.Invoke();
                    return;
                }
                break;

            case StateType.PlayerInfo:
                if (Keyboard.current.escapeKey.wasPressedThisFrame)
                    OnInputESCKeyDisabled?.Invoke();
                if (Keyboard.current.tabKey.wasPressedThisFrame)
                {
                    OnInputTabKeyPressed?.Invoke();
                    return;
                }
                break;
        }

        // < -- キャラクター切り替え処理 -->
        if (CurrentState == StateType.None || CurrentState == StateType.PlayerInfo)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                characterControlHandler.ChangeCharacterControlState(Character.Name.Girl);
            }
            else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                characterControlHandler.ChangeCharacterControlState(Character.Name.Boy);
            }
            else if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                characterControlHandler.ChangeCharacterControlState(Character.Name.Glasses_Boy);
            }
            else if (Keyboard.current.digit4Key.wasPressedThisFrame)
            {
                characterControlHandler.ChangeCharacterControlState(Character.Name.Delinquent_Boy);
            }
            else if (Keyboard.current.digit5Key.wasPressedThisFrame)
            {
                characterControlHandler.ChangeCharacterControlState(Character.Name.Girl_TwinTail);
            }
        }
    }

}