using UnityEngine;
using UnityEngine.UIElements;

public class CharacterInformationUIPanel : PanelView
{
    UIDocument uiDoc;

    void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
        panel = uiDoc.rootVisualElement.Q<VisualElement>("CharacterInformation");

        SetVisible(false);
    }

    void Start()
    {
        PlayerInputController.Instance.OnInputQKeyPressed += ToggleCharacterInformationPanel;
        PlayerInputController.Instance.OnInputESCKeyDisabled += HideCharacterInformationPanel;
    }

    void ToggleCharacterInformationPanel()
    {
        if (panel.style.display == DisplayStyle.None)
            ShowCharacterInformationPanel();
        else
            HideCharacterInformationPanel();
    }

    void HideCharacterInformationPanel()
    {
        SetVisible(false);
        PlayerInputController.Instance.ChangeState(StateType.None);
    }

    void ShowCharacterInformationPanel()
    {
        SetVisible(true);
        PlayerInputController.Instance.ChangeState(StateType.CharacterInfo);
    }
}