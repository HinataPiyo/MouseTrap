using UnityEngine;
using VContainer;

public class Interact : MonoBehaviour, IInteract
{
    [SerializeField] InteractData interactData;
    [SerializeField] Transform interactMarkPosition;
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
        eventBus.Publish(new ToggleInteractObjectPanelVisibility(new ShowInteractObjectPanel(interactData.Sprite, interactData.Text))); 
    }

    public void ShowMark()
    {
        eventBus.Publish(new ShowInteractMark(this));
    }

    public void HideMark()
    {
        eventBus.Publish(new HideInteractMark(this));
    }
}