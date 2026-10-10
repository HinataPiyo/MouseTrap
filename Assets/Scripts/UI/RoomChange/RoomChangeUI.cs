using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UIElements;

public class RoomChangeUI : MonoBehaviour
{
    public enum StatType { None = -1, Next, Back }
    [System.Serializable]
    public class StatSprite
    {
        public bool isInteractive;
        public StatType statType;
        public Sprite sprite;
    }

    public static RoomChangeUI Instance { get; private set; }

    [SerializeField] VisualTreeAsset temp_Button;
    VisualElement root;
    VisualElement container;

    [SerializeField] StatSprite[] statSprites;      // 配列内の各StatSpriteは、ボタンの状態に対応するスプライトとインタラクティブ状態を保持する

    Button nextButton;
    Button backButton;

    SceneChangeManager sceneChangeManager;
    PlayerInputController gameState;
    PlayerInputController inputTrigger;

    void Awake()
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement.Q<VisualElement>("room-change-buttons");
        container = root.Q<VisualElement>("container");
        nextButton = root.Q<Button>("next-button");
        backButton = root.Q<Button>("back-button");
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetInstance()
    {
        Instance = null;
    }

    void OnEnable()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        sceneChangeManager = SceneChangeManager.Instance;
        gameState = PlayerInputController.Instance;
        inputTrigger = PlayerInputController.Instance;

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
        Room.Name nextMoveableRoom = sceneChangeManager.GetNextMoveableRoom();
        Room.Name backMoveableRoom = sceneChangeManager.GetBackMoveableRoom();

        SetButtonCallback(nextMoveableRoom, nextButton);
        SetButtonCallback(backMoveableRoom, backButton);

        UpdateButtonState(nextMoveableRoom, nextButton, StatType.Next);
        UpdateButtonState(backMoveableRoom, backButton, StatType.Back);
    }

    public void SetButtonCallback(Room.Name room, Button button)
    {
        button.clicked += () =>
        {
            sceneChangeManager.SceneChange(room);
        };
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