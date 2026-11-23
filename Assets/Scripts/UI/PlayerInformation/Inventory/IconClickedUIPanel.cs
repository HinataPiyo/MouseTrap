using UnityEngine;
using UnityEngine.UIElements;


public class IconClickedUIPanel : MonoBehaviour
{
    UIDocument uiDoc;
    VisualElement panel;
    InventoryUIPanel inventoryUIPanel;

    Button useButton;
    Label description;

    VisualElement icon;
    InteractData selectedData;

    void Awake()
    {
        inventoryUIPanel = GetComponent<InventoryUIPanel>();
        uiDoc = GetComponent<UIDocument>();
        panel = uiDoc.rootVisualElement.Q<VisualElement>("IconClickedPanel");
        icon = panel.Q<VisualElement>("icon");
        useButton = panel.Q<Button>("use-button");
        description = panel.Q<Label>("description-text");
        HidePanel();

        // インベントリのアイコンがクリックされたときにパネルを表示するように設定
        inventoryUIPanel.OnIconClicked += ShowPanel;
        inventoryUIPanel.OnPanelClosed += HidePanel;
        panel.RegisterCallback<PointerDownEvent>(evt =>
        {
            if (evt.target is VisualElement target && (target == useButton || useButton.Contains(target)))
                return;

            HidePanel();
        });

        useButton.clicked += () =>
        {
            // 選択したものが破れたメモなら
            if(selectedData is InteractTornNotesData)
            {
                TornNotesUIPanel.Instance.Show();
            }
        };
    }

    void OnDestroy()
    {
        if (inventoryUIPanel == null) return;

        inventoryUIPanel.OnIconClicked -= ShowPanel;
        inventoryUIPanel.OnPanelClosed -= HidePanel;
    }

    public void ShowPanel(Inventory.ItemData item)
    {
        panel.style.display = DisplayStyle.Flex;
        icon.style.backgroundImage = new StyleBackground(item.item.Sprite);
        selectedData = item.item;
        useButton.style.display = selectedData.IsShowUseButton ? DisplayStyle.Flex : DisplayStyle.None;
        description.text = selectedData.Text;
        Debug.Log($"IconClickedUIPanel showing item: {item.item.name}");
    }

    public void HidePanel()
    {
        panel.style.display = DisplayStyle.None;
        icon.style.backgroundImage = null;
        selectedData = null;
        if(TornNotesUIPanel.Instance != null) TornNotesUIPanel.Instance.Hide();
    }

}