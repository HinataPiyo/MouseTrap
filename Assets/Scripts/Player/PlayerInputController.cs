using UnityEngine.InputSystem;
using VContainer.Unity;
using UnityEngine;
using VContainer;


public interface IInputTrigger
{
    public event System.Action OnInputEKeyPressed;
}

public class PlayerInputController : ITickable, IInputTrigger
{
    public event System.Action OnInputEKeyPressed;
    
    public void Tick()
    {
        if(Keyboard.current.eKey.wasPressedThisFrame)
        {
            OnInputEKeyPressed?.Invoke();
        }
    }

}