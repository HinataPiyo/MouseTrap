using Player;
using UnityEngine;
using UnityEngine.InputSystem;

// 基本None
public enum StateType
{ None = -1, Inventory, CharacterInfo, Movie, Talk, Interact, RoomChange }

public class PlayerInputController : SceneSingleton<PlayerInputController>
{
    CharacterControlHandler characterControlHandler;
    public StateType CurrentState { get; private set; } = StateType.None;

    public void ChangeState(StateType newState) => CurrentState = newState;

    public event System.Action OnInputEKeyPressed;
    public event System.Action OnInputTabKeyPressed;
    public event System.Action OnInputSpaceKeyPressed;
    public event System.Action OnInputESCKeyDisabled;
    public event System.Action OnInputQKeyPressed;

    protected override void OnSingletonAwake()
    {
        ChangeState(StateType.None);
    }

    void Start()
    {
        characterControlHandler = CharacterControlHandler.Instance;

        OnInputESCKeyDisabled += () => ChangeState(StateType.None);
    }

    public void Update()
    {
        Debug.Log(CurrentState);

        if (CurrentState != StateType.None
            && CurrentState != StateType.Movie
            && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            OnInputESCKeyDisabled?.Invoke();
            return;
        }

        switch (CurrentState)
        {
            case StateType.RoomChange:
                return;

            case StateType.Talk:
                if (IsActionKeyPressed())
                    OnInputSpaceKeyPressed?.Invoke();
                return;

            case StateType.Interact:
                if (IsActionKeyPressed())
                    OnInputEKeyPressed?.Invoke();
                return;

            case StateType.None:
                if (IsActionKeyPressed())
                {
                    OnInputEKeyPressed?.Invoke();
                    return;
                }
                if (Keyboard.current.tabKey.wasPressedThisFrame)
                {
                    OnInputTabKeyPressed?.Invoke();
                    return;
                }
                if (Keyboard.current.qKey.wasPressedThisFrame)
                {
                    OnInputQKeyPressed?.Invoke();
                    return;
                }
                break;

            case StateType.Inventory:
                if (Keyboard.current.tabKey.wasPressedThisFrame)
                {
                    OnInputTabKeyPressed?.Invoke();
                    return;
                }
                break;
            case StateType.CharacterInfo:
                if (Keyboard.current.qKey.wasPressedThisFrame)
                {
                    OnInputQKeyPressed?.Invoke();
                    return;
                }
                break;
        }

        // < -- キャラクター切り替え処理 -->
        if (CurrentState == StateType.None || CurrentState == StateType.Inventory)
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

    static bool IsActionKeyPressed()
    {
        return Keyboard.current.eKey.wasPressedThisFrame
            || Keyboard.current.spaceKey.wasPressedThisFrame;
    }

}