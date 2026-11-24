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
            AudioManager.Instance.PlaySE("普通のボタン");
            if (selectedData != null && selectedData.ArrangementRecipe != null)
            {
                ArrangementUIPanel.Instance.Show(selectedData.ArrangementRecipe, selectedData);
            }
            else if (selectedData is InteractArrangementData arrangement)
            {
                ArrangementUIPanel.Instance.Show(arrangement, selectedData);
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
        useButton.style.display = selectedData.IsShowUseButton || selectedData.ArrangementRecipe != null
            ? DisplayStyle.Flex : DisplayStyle.None;
        description.text = selectedData.Text;
        Debug.Log($"IconClickedUIPanel showing item: {item.item.name}");
    }

    public void HidePanel()
    {
        panel.style.display = DisplayStyle.None;
        icon.style.backgroundImage = null;
        selectedData = null;
        if (ArrangementUIPanel.Instance != null) ArrangementUIPanel.Instance.Hide();
    }

}