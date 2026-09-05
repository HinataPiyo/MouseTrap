using Player;
using UnityEngine;
using UnityEngine.InputSystem;

// 基本None
public enum StateType
{ None = -1, Talk, Interact }

public interface IInputTrigger
{
    public event System.Action OnInputEKeyPressed;
    public event System.Action OnInputTabKeyPressed;
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

    void Awake()
    {
        ApiProvider.Register<IInputTrigger>(this);
        ApiProvider.Register<IGameState>(this);
        
        ChangeState(StateType.None);
    }

    void Start()
    {
        characterControlHandler = ApiProvider.Get<ICharacterState>();
    }

    public void Update()
    {
        // 取得処理
        if(Keyboard.current.eKey.wasPressedThisFrame)
        {
            OnInputEKeyPressed?.Invoke();
        }

        if(CurrentState == StateType.Talk || CurrentState == StateType.Interact)
        {
            return;
        }

        // インフォメーション表示処理
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            // Tabキーが押されたときの処理
            OnInputTabKeyPressed?.Invoke();
        }

        // < -- キャラクター切り替え処理 -->
        if(Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            characterControlHandler.ChangeCharacterControlState(Character.Name.Girl);
        }
        else if(Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            characterControlHandler.ChangeCharacterControlState(Character.Name.Boy);
        }
        else if(Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            characterControlHandler.ChangeCharacterControlState(Character.Name.Glasses_Boy);
        }
        else if(Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            characterControlHandler.ChangeCharacterControlState(Character.Name.Delinquent_Boy);
        }
        else if(Keyboard.current.digit5Key.wasPressedThisFrame)
        {
            characterControlHandler.ChangeCharacterControlState(Character.Name.Girl_TwinTail);
        }
    }

}