using UnityEngine;
using VContainer;

public class Interact : MonoBehaviour, IInteract
{
    [SerializeField] InteractData interactData;
    [SerializeField] Transform interactMarkPosition;
    bool hasAcquiredItem;

    public Transform GetMarkPoint() => interactMarkPosition;
    public InteractMarkType InteractMarkType => interactData.InteractMarkType;

    [Inject]
    IEventBus eventBus;

    /// <summary>
    /// Interactオブジェクトを取得した際にPanelに表示する処理
    /// </summary>
    public void ApplyInteract()
    {
        Debug.Log($"Interact: {interactData.name}");
        eventBus.Publish(new ToggleInteractObjectPanelVisibility(interactData.Sprite, interactData.Text));

        if (!hasAcquiredItem)
        {
            eventBus.Publish(new OnItemChanged(new Inventory.ItemData { item = interactData, count = 1 }, true));
            eventBus.Publish(new Player.OnChangePlayerStop(true));
            hasAcquiredItem = true;
        }
        else
        {
            eventBus.Publish(new Player.OnChangePlayerStop(false));
        }
    }

    public void ShowMark()
    {
        eventBus.Publish(new ToggleMarkVisibility(this, true));
    }

    public void HideMark()
    {
        eventBus.Publish(new ToggleMarkVisibility(this, false));
    }
}