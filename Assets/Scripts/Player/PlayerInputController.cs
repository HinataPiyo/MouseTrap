using UnityEngine.InputSystem;
using VContainer.Unity;

public interface IInputTrigger
{
    public event System.Action OnInputEKeyPressed;
    public event System.Action OnInputTabKeyPressed;
}

public class PlayerInputController : ITickable, IInputTrigger
{
    public event System.Action OnInputEKeyPressed;
    public event System.Action OnInputTabKeyPressed;
    public void Tick()
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