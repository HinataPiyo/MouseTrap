using UnityEngine;
using UnityEngine.UIElements;

public class UpperButtonsHandler : MonoBehaviour
{
    Button inventoryButton;
    Button characterButton;

    void Start()
    {
        UIDocument uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null)
        {
            Debug.LogError("UIDocument is not set on UpperButtonsHandler.", this);
            enabled = false;
            return;
        }

        VisualElement root = uiDoc.rootVisualElement;
        inventoryButton = root.Q<Button>("inventory-backpack-button");
        characterButton = root.Q<Button>("character-button");
        if (inventoryButton == null || characterButton == null)
        {
            Debug.LogError("Inventory or character button was not found in the UIDocument.", this);
            enabled = false;
            return;
        }

        inventoryButton.clicked += OpenInventory;
        characterButton.clicked += OpenCharacterInformation;
    }

    void OnDestroy()
    {
        if (inventoryButton != null)
            inventoryButton.clicked -= OpenInventory;
        if (characterButton != null)
            characterButton.clicked -= OpenCharacterInformation;
    }

    void OpenInventory()
    {
        if (InventoryUIPanel.Instance == null)
        {
            Debug.LogError("InventoryUIPanel is not available.", this);
            return;
        }

        InventoryUIPanel.Instance.ShowInventoryPanel();
    }

    void OpenCharacterInformation()
    {
        if (CharacterInformationUIPanel.Instance == null)
        {
            Debug.LogError("CharacterInformationUIPanel is not available.", this);
            return;
        }

        CharacterInformationUIPanel.Instance.ShowCharacterInformationPanel();
    }
}