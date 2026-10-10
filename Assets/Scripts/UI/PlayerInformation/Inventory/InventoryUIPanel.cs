using UnityEngine;
using UnityEngine.UIElements;

public class InventoryUIPanel : PanelView
{
    UIDocument uiDoc;
    ScrollView container;
    [SerializeField] VisualTreeAsset temp_InventoryItem;

    const string PANEL_NAME = "InventoryPanel";
    public event System.Action<Inventory.ItemData> OnIconClicked;
    PlayerInputController inputController;


    void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
        panel = uiDoc.rootVisualElement.Q<VisualElement>(PANEL_NAME);
        container = panel.Q<ScrollView>();

        container.Clear();
    }

    void Start()
    {
        inputController = PlayerInputController.Instance;

        inputController.OnInputTabKeyPressed += TogglePanelVisibility;
        inputController.OnInputESCKeyDisabled += HideInventoryPanel;
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
        PlayerInputController.Instance.ChangeState(StateType.Inventory);
    }

    public void HideInventoryPanel()
    {
        panel.style.display = DisplayStyle.None;
        isVisible = false;
        PlayerInputController.Instance.ChangeState(StateType.None);
    }

    /// <summary>
    /// UIにアイテムを追加する
    /// </summary>
    /// <param name="itemData">追加するアイテムのデータ</param>
    void CreateUI(Inventory.ItemData data)
    {
        VisualElement itemTemplate = temp_InventoryItem.CloneTree();

        itemTemplate.Q<VisualElement>("icon").style.backgroundImage = new StyleBackground(data.item.Sprite);
        itemTemplate.Q<Label>("count").text = "x" + data.count.ToString();
        itemTemplate.Q<Button>().clicked += () => OnIconClicked?.Invoke(data);

        container.Add(itemTemplate);
    }

    void RefreshInventoryElements()
    {
        container.Clear();
        foreach (var itemData in Inventory.Instance.GetItems())
        {
            CreateUI(itemData);
        }
    }
}