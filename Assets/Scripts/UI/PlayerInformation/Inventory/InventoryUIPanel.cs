using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using VContainer;

public interface IInventoryUIPanel
{
    void TogglePanelVisibility();
    void ShowInventoryPanel();
    void HideInventoryPanel();
}

public class InventoryUIPanel : PanelView, IInventoryUIPanel
{
    UIDocument uiDoc;
    VisualElement container;
    [SerializeField] VisualTreeAsset temp_InventoryItem;

    IInventoryService inventoryService;

    const string PANEL_NAME = "InventoryPanel";

    [Inject]
    public void Construct(IInventoryService inventoryService)
    {
        this.inventoryService = inventoryService;
    }


    void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
        panel = uiDoc.rootVisualElement.Q<VisualElement>(PANEL_NAME);
        container = panel.Q<VisualElement>("root");

        container.Clear();
    }

    public void TogglePanelVisibility()
    {
        SetVisible(!isVisible);
        Debug.Log($"InventoryUIPanel visibility toggled. Now visible: {isVisible}");

        if (isVisible)
        {
            ShowInventoryPanel();
        }
        else
        {
            HideInventoryPanel();
        }
    }

    public void ShowInventoryPanel()
    {
        RefreshInventoryElements();
        panel.style.display = DisplayStyle.Flex;
        isVisible = true;
    }

    public void HideInventoryPanel()
    {
        panel.style.display = DisplayStyle.None;
        isVisible = false;
    }

    /// <summary>
    /// UIにアイテムを追加する
    /// </summary>
    /// <param name="itemData">追加するアイテムのデータ</param>
    void CreateUI(Inventory.ItemData data)
    {
        VisualElement itemTemplate = temp_InventoryItem.CloneTree();

        itemTemplate.Q<VisualElement>("icon").style.backgroundImage = new StyleBackground(data.item.Sprite);
        itemTemplate.Q<Label>("name").text = data.item.Text;
        itemTemplate.Q<Label>("count").text = "x" + data.count.ToString();

        container.Add(itemTemplate);
    }

    void RefreshInventoryElements()
    {
        container.Clear();
        foreach (var itemData in inventoryService.GetItems())
        {
            CreateUI(itemData);
        }
    }
}