using UnityEngine.InputSystem;
using VContainer.Unity;
using UnityEngine;
using VContainer;

public class PlayerInputController : ITickable
{
    IEventBus eventBus;

    [Inject]
    public void Construct(IEventBus eventBus)
    {
        this.eventBus = eventBus;
    }

    public void Tick()
    {
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            //! テスト
            eventBus.Publish(new ToggleTakeObjectPanelVisibility(new ShowTakeObjectPanel(null, "テスト")));
        }
    }

}