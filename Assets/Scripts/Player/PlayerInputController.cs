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

    public void Update()
    {
        if(Keyboard.current.eKey.wasPressedThisFrame)
        {
            OnInputEKeyPressed?.Invoke();
        }
        else if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            // Tabキーが押されたときの処理
            OnInputTabKeyPressed?.Invoke();
        }
    }

}