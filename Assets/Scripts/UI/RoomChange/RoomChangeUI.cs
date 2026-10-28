using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UIElements;

public class RoomChangeUI : SceneSingleton<RoomChangeUI>
{
    public enum StatType { None = -1, Next, Back }
    [System.Serializable]
    public class StatSprite
    {
        public bool isInteractive;
        public StatType statType;
        public Sprite sprite;
    }

    [SerializeField] VisualTreeAsset temp_Button;
    VisualElement root;
    VisualElement container;

    [SerializeField] StatSprite[] statSprites;      // 配列内の各StatSpriteは、ボタンの状態に対応するスプライトとインタラクティブ状態を保持する

    Button nextButton;
    Button backButton;
    Room.Name nextRoom = Room.Name.None;
    Room.Name backRoom = Room.Name.None;

    SceneChangeManager sceneChangeManager;
    PlayerInputController gameState;
    PlayerInputController inputTrigger;

    protected override void OnSingletonAwake()
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement.Q<VisualElement>("room-change-buttons");
        container = root.Q<VisualElement>("container");
        nextButton = root.Q<Button>("next-button");
        backButton = root.Q<Button>("back-button");
    }

    void Start()
    {
        sceneChangeManager = SceneChangeManager.Instance;
        gameState = PlayerInputController.Instance;
        inputTrigger = PlayerInputController.Instance;

        // クリック処理は一度だけ登録し、移動先は表示のたびに更新する
        nextButton.clicked += () => sceneChangeManager.SceneChange(nextRoom);
        backButton.clicked += () => sceneChangeManager.SceneChange(backRoom);

        inputTrigger.OnInputESCKeyDisabled += HidePanel;
        HidePanel();
    }

    public void ShowPanel()
    {
        gameState.ChangeState(StateType.RoomChange);
        root.style.display = DisplayStyle.Flex;

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
        nextRoom = sceneChangeManager.GetNextMoveableRoom();
        backRoom = sceneChangeManager.GetBackMoveableRoom();

        UpdateButtonState(nextRoom, nextButton, StatType.Next);
        UpdateButtonState(backRoom, backButton, StatType.Back);
    }
    
    void UpdateButtonState(Room.Name moveableRoom, Button button, StatType statType)
    {
        bool isInteractive = moveableRoom != Room.Name.None;

        foreach (var statSprite in statSprites)
        {
            if (statSprite.statType != statType || statSprite.isInteractive != isInteractive) continue;

            button.style.backgroundImage = new StyleBackground(statSprite.sprite);
            button.pickingMode = isInteractive ? PickingMode.Position : PickingMode.Ignore;
            Debug.Log($"Button: {button.name}, MoveableRoom: {moveableRoom}, StatType: {statType}");
            break;
        }
    }
}