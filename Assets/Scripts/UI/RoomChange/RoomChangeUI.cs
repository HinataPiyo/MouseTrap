using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public interface IRoomChangeUI
{
    void ShowPanel();
    void HidePanel();
}

public class RoomChangeUI : MonoBehaviour, IRoomChangeUI
{
    [SerializeField] VisualTreeAsset temp_Button;
    VisualElement root;
    VisualElement container;

    List<Button> buttons = new List<Button>();
    ISceneChange sceneChangeManager;
    IGameState gameState;
    IInputTrigger inputTrigger;

    void Awake()
    {
        ApiProvider.Register<IRoomChangeUI>(this);
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement.Q<VisualElement>("room-change-buttons");
        container = root.Q<VisualElement>("container");
    }

    void Start()
    {
        sceneChangeManager = ApiProvider.Get<ISceneChange>();
        gameState = ApiProvider.Get<IGameState>();
        inputTrigger = ApiProvider.Get<IInputTrigger>();

        inputTrigger.OnInputESCKeyDisabled += HidePanel;
        HidePanel();
    }

    public void ShowPanel()
    {
        gameState.ChangeState(StateType.RoomChange);
        root.style.display = DisplayStyle.Flex;
        // パネルの位置を画面の中心に持ってくる
        root.style.position = Position.Absolute;
        root.style.left = Length.Percent(50);
        root.style.top = Length.Percent(50);
        root.style.translate = new StyleTranslate(new Translate(Length.Percent(-50), Length.Percent(-50)));

        UpdateButtons();
    }

    public void HidePanel()
    {
        root.style.display = DisplayStyle.None;
    }

    /// <summary>
    /// 現在移動可能な部屋のボタンを更新する
    /// </summary>
    void UpdateButtons()
    {
        ClearButtons();
        var moveableRooms = sceneChangeManager.GetMoveableRooms();
        foreach (var room in moveableRooms)
        {
            CreateButton(room);
        }
    }

    void CreateButton(Room.Name room)
    {
        var button = temp_Button.CloneTree();
        container.Add(button);

        var uiButton = button.Q<Button>();
        uiButton.text = Room.RoomNameWithDisplayName[room];
        buttons.Add(uiButton);

        SetButtonCallback(room, uiButton);
    }

    void ClearButtons()
    {
        container.Clear();
        buttons.Clear();
    }

    public void SetButtonCallback(Room.Name room, Button button)
    {
        button.clicked += () =>
        {
            sceneChangeManager.SceneChange(room);
        };
    }
}