using UnityEngine.UIElements;

public class InteractUIPanel : PanelView<InteractUIPanel>
{
    VisualElement object_view;
    Label itemName_view;
    PlayerInputController inputController;
    public bool IsVisible() => isVisible;

    protected override void OnSingletonAwake()
    {
        panel = GetComponent<UIDocument>().rootVisualElement;
        object_view = panel.Q<VisualElement>("image");
        itemName_view = panel.Q<Label>("item-name");

        SetVisible(false);
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

        AudioManager.Instance.PlaySE("アイテム獲得");
    }

    public void HideInteractPanel()
    {
        SetVisible(false);
        PlayerInputController.Instance.ChangeState(StateType.None);
    }
}