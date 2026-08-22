using UnityEngine;
using UnityEngine.InputSystem;

public interface IInputTrigger
{
    public event System.Action OnInputEKeyPressed;
    public event System.Action OnInputTabKeyPressed;
}

public class PlayerInputController : MonoBehaviour, IInputTrigger
{
    public event System.Action OnInputEKeyPressed;
    public event System.Action OnInputTabKeyPressed;

    void Awake()
    {
        ApiProvider.Register<IInputTrigger>(this);
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