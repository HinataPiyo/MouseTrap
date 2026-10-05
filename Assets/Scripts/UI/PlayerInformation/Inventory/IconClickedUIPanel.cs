using UnityEngine;
using UnityEngine.UIElements;


public class IconClickedUIPanel : MonoBehaviour
{
    UIDocument uiDoc;
    VisualElement panel;
    InventoryUIPanel inventoryUIPanel;

    VisualElement icon;

    void Awake()
    {
        inventoryUIPanel = GetComponent<InventoryUIPanel>();
        uiDoc = GetComponent<UIDocument>();
        panel = uiDoc.rootVisualElement.Q<VisualElement>("IconClickedPanel");
        icon = panel.Q<VisualElement>("icon");
        HidePanel();

        // インベントリのアイコンがクリックされたときにパネルを表示するように設定
        inventoryUIPanel.OnIconClicked += (item) => ShowPanel(item);
        panel.RegisterCallback<PointerDownEvent>(evt => HidePanel());
    }

    public void ShowPanel(Inventory.ItemData item)
    {
        panel.style.display = DisplayStyle.Flex;
        icon.style.backgroundImage = new StyleBackground(item.item.Sprite);
        Debug.Log($"IconClickedUIPanel showing item: {item.item.name}");
    }

    public void HidePanel()
    {
        panel.style.display = DisplayStyle.None;
        icon.style.backgroundImage = null;
    }

}