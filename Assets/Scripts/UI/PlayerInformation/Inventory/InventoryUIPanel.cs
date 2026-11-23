using UnityEngine;
using UnityEngine.UIElements;

public class InventoryUIPanel : PanelView<InventoryUIPanel>
{
    UIDocument uiDoc;
    ScrollView container;
    Button backButton;
    [SerializeField] VisualTreeAsset temp_InventoryItem;

    const string PANEL_NAME = "InventoryPanel";
    public event System.Action<Inventory.ItemData> OnIconClicked;
    public event System.Action OnPanelClosed;
    PlayerInputController inputController;


    protected override void OnSingletonAwake()
    {
        uiDoc = GetComponent<UIDocument>();
        panel = uiDoc.rootVisualElement.Q<VisualElement>(PANEL_NAME);
        container = panel.Q<ScrollView>();
        backButton = panel.Q<Button>("BackButton");
        if (backButton == null)
        {
            Debug.LogError("The BackButton was not found in the inventory panel.", this);
            enabled = false;
            return;
        }

        container.Clear();
        backButton.clicked += HideInventoryPanel;
        backButton.clicked += () => AudioManager.Instance.PlaySE("キャンセル");
    }

    void Start()
    {
        inputController = PlayerInputController.Instance;

        inputController.OnInputTabKeyPressed += TogglePanelVisibility;
        inputController.OnInputESCKeyDisabled += HideInventoryPanel;
    }

    protected override void OnSingletonDestroy()
    {
        if (backButton != null)
            backButton.clicked -= HideInventoryPanel;

        if (inputController != null)
        {
            inputController.OnInputTabKeyPressed -= TogglePanelVisibility;
            inputController.OnInputESCKeyDisabled -= HideInventoryPanel;
        }
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
        OnPanelClosed?.Invoke();
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
        var countLabel = itemTemplate.Q<Label>("count");
        countLabel.text = "x" + data.count.ToString();
        countLabel.style.display = data.count > 1 ? DisplayStyle.Flex : DisplayStyle.None;
        itemTemplate.Q<Button>().clicked += () =>
        {
            AudioManager.Instance.PlaySE("普通のボタン");
            OnIconClicked?.Invoke(data);
        };

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