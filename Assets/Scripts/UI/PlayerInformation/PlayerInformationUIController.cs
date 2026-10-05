using UnityEngine.UIElements;
using UnityEngine;

public interface IPlayerInformationUIController {
    void TogglePlayerInformationVisibility();
    void ShowPlayerInformation();
    void HidePlayerInformation();
}


public class PlayerInformationUIController : PanelView, IPlayerInformationUIController
{
    UIDocument uiDoc;
    VisualElement root;

    PlayerInputController inputController;

    void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement.Q<VisualElement>("root");
    }

    void Start()
    {
        inputController = PlayerInputController.Instance;
        inputController.OnInputTabKeyPressed += TogglePlayerInformationVisibility;
        inputController.OnInputESCKeyDisabled += HidePlayerInformation;
        HidePlayerInformation();
    }

    public void TogglePlayerInformationVisibility()
    {
        Debug.LogFormat("Toggling Player Information Visibility. Current display: {0}", root.style.display);
        if (root.style.display == DisplayStyle.None)
        {
            inputController.ChangeState(StateType.PlayerInfo);
            ShowPlayerInformation();
        }
        else
        {
            HidePlayerInformation();
            inputController.ChangeState(StateType.None);
        }
    }

    public void ShowPlayerInformation()
    {
        InventoryUIPanel.Instance.ShowInventoryPanel();
        root.style.display = DisplayStyle.Flex;
    }

    public void HidePlayerInformation()
    {
        InventoryUIPanel.Instance.HideInventoryPanel();
        root.style.display = DisplayStyle.None;
    }
}