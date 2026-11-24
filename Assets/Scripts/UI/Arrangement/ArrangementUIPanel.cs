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
        SetVisible(false);
        backButton = panel.Q<Button>("back");
        elementButtons = panel.Q("element-buttons").Query<Button>().ToList();
        positionButtons = panel.Q("position-buttons").Query<Button>().ToList();

        for (int i = 0; i < elementButtons.Count; i++)
        {
            int index = i;
            elementButtons[i].clicked += () =>
            {
                selectedElement = index;
                AudioManager.Instance.PlaySE("アイテム選択");
            };
        }

        for (int i = 0; i < positionButtons.Count; i++)
        {
            int index = i;
            positionButtons[i].clicked += () => PlaceElement(index);
        }

        backButton.clicked += Hide;
        backButton.clicked += () => AudioManager.Instance.PlaySE("キャンセル");
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

    public void Show(InteractArrangementData item, InteractData source)
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

        if (source == null || !Inventory.Instance.HasItem(source) ||
            (item.UseIngredients ? !item.ContainsIngredient(source) : source != item))
        {
            Debug.LogError($"Cannot open arrangement '{item.name}' from an item that is not an owned ingredient.", item);
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
        GameUIHandler.Instance.SetGimmickActive(true);
    }

    public void Hide()
    {
        currentItem = null;
        selectedElement = -1;
        SetVisible(false);
        GameUIHandler.Instance.SetGimmickActive(false);
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
            bool visible = i < placedElements.Length && IsElementAvailable(i);
            button.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            button.style.width = placedElements.Length == InteractArrangementData.MaxElements ? 100 : 120;
            button.SetEnabled(visible);
            if (visible)
            {
                button.style.backgroundImage = new StyleBackground(currentItem.Elements[i].Ingredient.Sprite);
                button.text = "";
            }
        }
    }

    bool IsElementAvailable(int index)
    {
        if (!currentItem.UseIngredients) return true;

        InteractData ingredient = currentItem.Elements[index].Ingredient;
        int requiredCount = 0;
        for (int i = 0; i <= index; i++)
            if (currentItem.Elements[i].Ingredient == ingredient) requiredCount++;

        return Inventory.Instance.GetItemCount(ingredient) >= requiredCount;
    }

    void PlaceElement(int position)
    {
        if (currentItem == null || selectedElement < 0 || position >= placedElements.Length ||
            placedElements[position] >= 0)
            return;

        if (!IsElementAvailable(selectedElement))
        {
            Debug.LogError($"An ingredient for arrangement '{currentItem.name}' is no longer in the inventory.", currentItem);
            Hide();
            return;
        }

        placedElements[position] = selectedElement;
        positionButtons[position].iconImage = Background.FromSprite(currentItem.Elements[selectedElement].Ingredient.Sprite);
        elementButtons[selectedElement].style.display = DisplayStyle.None;
        selectedElement = -1;
        AudioManager.Instance.PlaySE("置く");

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

        bool hasRequiredItems = true;
        if (currentItem.UseIngredients)
        {
            for (int i = 0; i < currentItem.Elements.Length; i++)
                hasRequiredItems &= IsElementAvailable(i);
        }
        else
        {
            hasRequiredItems = Inventory.Instance.HasItem(currentItem);
        }

        if (!hasRequiredItems)
        {
            Debug.LogError($"Required items for arrangement '{currentItem.name}' are no longer in the inventory.", currentItem);
            Hide();
            return;
        }

        InteractData completedItem = currentItem.CompletedItem;
        if (currentItem.UseIngredients)
        {
            foreach (InteractArrangementData.Element element in currentItem.Elements)
                Inventory.Instance.RemoveItem(element.Ingredient);
        }
        else
        {
            Inventory.Instance.RemoveItem(currentItem);
        }
        InventoryUIPanel.Instance.HideInventoryPanel();
        InteractUIPanel.Instance.ShowInteractPanel(completedItem);
        Inventory.Instance.AddItem(completedItem);
    }
}