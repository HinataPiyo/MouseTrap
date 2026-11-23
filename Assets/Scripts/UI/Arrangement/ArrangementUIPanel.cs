using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ArrangementUIPanel : PanelView<ArrangementUIPanel>
{
    [SerializeField] Sprite returnPositionUI;

    List<Button> elementButtons;
    List<Button> positionButtons;
    Button backButton;
    InteractArrangementData currentItem;
    int[] placedElements;
    int selectedElement = -1;
    PlayerInputController inputController;

    protected override void OnSingletonAwake()
    {
        panel = GetComponent<UIDocument>().rootVisualElement.Q("ArrangementPanel");
        backButton = panel.Q<Button>("back");
        elementButtons = panel.Q("element-buttons").Query<Button>().ToList();
        positionButtons = panel.Q("position-buttons").Query<Button>().ToList();

        for (int i = 0; i < elementButtons.Count; i++)
        {
            int index = i;
            elementButtons[i].clicked += () => selectedElement = index;
        }

        for (int i = 0; i < positionButtons.Count; i++)
        {
            int index = i;
            positionButtons[i].clicked += () => PlaceElement(index);
        }

        backButton.clicked += Hide;
        Hide();
    }

    void Start()
    {
        inputController = PlayerInputController.Instance;
        inputController.OnInputESCKeyDisabled += Hide;
    }

    protected override void OnSingletonDestroy()
    {
        if (inputController != null)
            inputController.OnInputESCKeyDisabled -= Hide;
        if (backButton != null)
            backButton.clicked -= Hide;
    }

    public void Show(InteractArrangementData item)
    {
        if (item == null)
        {
            Debug.LogError("Cannot open the arrangement panel without an item.", this);
            return;
        }

        if (!item.TryValidate(out string error))
        {
            Debug.LogError($"Invalid arrangement item '{item.name}': {error}", item);
            return;
        }

        if (elementButtons.Count < item.Elements.Length || positionButtons.Count < item.Elements.Length)
        {
            Debug.LogError("The arrangement panel does not have enough buttons.", this);
            return;
        }

        currentItem = item;
        placedElements = new int[item.Elements.Length];
        ResetArrangement();
        SetVisible(true);
    }

    public void Hide()
    {
        currentItem = null;
        selectedElement = -1;
        SetVisible(false);
    }

    void ResetArrangement()
    {
        selectedElement = -1;
        for (int i = 0; i < placedElements.Length; i++)
            placedElements[i] = -1;

        for (int i = 0; i < positionButtons.Count; i++)
        {
            Button button = positionButtons[i];
            button.style.display = i < placedElements.Length ? DisplayStyle.Flex : DisplayStyle.None;
            button.style.width = placedElements.Length == InteractArrangementData.MaxElements ? 100 : 135;
            button.style.backgroundImage = new StyleBackground(returnPositionUI);
            button.iconImage = null;
            button.text = "";
        }

        for (int i = 0; i < elementButtons.Count; i++)
        {
            Button button = elementButtons[i];
            bool visible = i < placedElements.Length;
            button.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            button.style.width = placedElements.Length == InteractArrangementData.MaxElements ? 100 : 120;
            if (visible)
                button.style.backgroundImage = new StyleBackground(currentItem.Elements[i].Sprite);
        }
    }

    void PlaceElement(int position)
    {
        if (currentItem == null || selectedElement < 0 || position >= placedElements.Length ||
            placedElements[position] >= 0)
            return;

        placedElements[position] = selectedElement;
        positionButtons[position].iconImage = Background.FromSprite(currentItem.Elements[selectedElement].Sprite);
        elementButtons[selectedElement].style.display = DisplayStyle.None;
        selectedElement = -1;

        for (int i = 0; i < placedElements.Length; i++)
        {
            if (placedElements[i] < 0) return;
        }

        if (currentItem.RequireOrder)
        {
            for (int i = 0; i < placedElements.Length; i++)
            {
                if (currentItem.Elements[placedElements[i]].CorrectPosition != i + 1)
                {
                    ResetArrangement();
                    return;
                }
            }
        }

        if (!Inventory.Instance.HasItem(currentItem))
        {
            Debug.LogError($"Arrangement item '{currentItem.name}' is no longer in the inventory.", currentItem);
            Hide();
            return;
        }

        InteractData completedItem = currentItem.CompletedItem;
        Inventory.Instance.RemoveItem(currentItem);
        InventoryUIPanel.Instance.HideInventoryPanel();
        InteractUIPanel.Instance.ShowInteractPanel(completedItem);
        Inventory.Instance.AddItem(completedItem);
    }
}