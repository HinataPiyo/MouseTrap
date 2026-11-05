using UnityEngine.UIElements;

public class InteractUIPanel : PanelView<InteractUIPanel>
{
    VisualElement object_view;
    Label itemName_view;
    PlayerInputController inputController;
    public bool IsVisible() => isVisible;

    protected override void OnSingletonAwake()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        panel = uiDocument.rootVisualElement.Q<VisualElement>("interact-object-container");
        object_view = panel.Q<VisualElement>("image");
        itemName_view = panel.Q<Label>("item-name");
    }

    void Start()
    {
        inputController = PlayerInputController.Instance;
        inputController.OnInputESCKeyDisabled += HideInteractPanel;
    }

    protected override void OnSingletonDestroy()
    {
        if (inputController != null)
            inputController.OnInputESCKeyDisabled -= HideInteractPanel;
    }

    public void ShowInteractPanel(InteractData interactData)
    {
        SetVisible(interactData);

        object_view.style.backgroundImage = new StyleBackground(interactData.Sprite);
        itemName_view.text = interactData.Text;
        PlayerInputController.Instance.ChangeState(StateType.Interact);
    }

    public void HideInteractPanel()
    {
        SetVisible(false);
    }
}