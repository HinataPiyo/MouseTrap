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

    IInventoryUIPanel inventoryUIPanel;

    IInputTrigger inputTrigger;
    IGameState gameState;

    void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement.Q<VisualElement>("root");
    }

    void Start()
    {
        inputTrigger = ApiProvider.Get<IInputTrigger>();
        inventoryUIPanel = ApiProvider.Get<IInventoryUIPanel>();
        gameState = ApiProvider.Get<IGameState>();
        inputTrigger.OnInputTabKeyPressed += TogglePlayerInformationVisibility;
        inputTrigger.OnInputESCKeyDisabled += HidePlayerInformation;
        HidePlayerInformation();
    }

    public void TogglePlayerInformationVisibility()
    {
        Debug.LogFormat("Toggling Player Information Visibility. Current display: {0}", root.style.display);
        if (root.style.display == DisplayStyle.None)
        {
            gameState.ChangeState(StateType.PlayerInfo);
            ShowPlayerInformation();
        }
        else
        {
            HidePlayerInformation();
            gameState.ChangeState(StateType.None);
        }
    }

    public void ShowPlayerInformation()
    {
        inventoryUIPanel.ShowInventoryPanel();
        root.style.display = DisplayStyle.Flex;
    }

    public void HidePlayerInformation()
    {
        inventoryUIPanel.HideInventoryPanel();
        root.style.display = DisplayStyle.None;
    }
}