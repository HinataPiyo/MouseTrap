using Player;
using UnityEngine;
using UnityEngine.UIElements;

public class InteractUIPanel : PanelView<InteractUIPanel>
{
    VisualElement object_view;
    public bool IsVisible() => isVisible;

    protected override void OnSingletonAwake()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        panel = uiDocument.rootVisualElement.Q<VisualElement>("interact-object-container");
        object_view = panel.Q<VisualElement>("image");
    }

    public void ShowInteractPanel(InteractData interactData)
    {
        SetVisible(interactData);
        object_view.style.backgroundImage = new StyleBackground(interactData.Sprite);
        PlayerInputController.Instance.ChangeState(StateType.Interact);
    }

    public void HideInteractPanel()
    {
        SetVisible(false);
    }
}