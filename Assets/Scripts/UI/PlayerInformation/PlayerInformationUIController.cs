using UnityEngine.UIElements;
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

    void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement.Q<VisualElement>("root");
        HidePlayerInformation();
    }

    public void TogglePlayerInformationVisibility()
    {
        if (root.style.display == DisplayStyle.None)
        {
            ShowPlayerInformation();
        }
        else
        {
            HidePlayerInformation();
        }
    }

    public void ShowPlayerInformation()
    {
        inventoryUIPanel.ShowInventoryPanel();
        root.style.display = DisplayStyle.Flex;
    }

    public void HidePlayerInformation()
    {
        root.style.display = DisplayStyle.None;
    }
}