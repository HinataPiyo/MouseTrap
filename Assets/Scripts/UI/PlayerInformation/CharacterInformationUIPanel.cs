using UnityEngine;
using UnityEngine.UIElements;
using Player;

public class CharacterInformationUIPanel : PanelView<CharacterInformationUIPanel>
{
    UIDocument uiDoc;
    VisualElement selectCharacterImage;
    VisualElement specialSkillIcon;
    Label specialSkillDescription;
    ScrollView iconList;
    CharacterIcon[] icons;
    CharacterControlHandler characterControlHandler;
    PlayerInputController inputController;

    protected override void OnSingletonAwake()
    {
        // パネルのUI参照を初期化し、必須UIが欠けている場合は明示的に停止する
        uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null)
        {
            Debug.LogError("UIDocument is not set on CharacterInformationUIPanel.", this);
            enabled = false;
            return;
        }

        panel = uiDoc.rootVisualElement.Q<VisualElement>("CharacterInformation");
        if (panel == null)
        {
            Debug.LogError("The CharacterInformation element was not found in the UIDocument.", this);
            enabled = false;
            return;
        }

        SetVisible(false);
    }

    void Start()
    {
        // キャラクター切り替えロジックを取得し、UI初期化より先に参照可能か確認する
        characterControlHandler = CharacterControlHandler.Instance;
        if (characterControlHandler == null)
        {
            Debug.LogError("CharacterControlHandler is not available.", this);
            enabled = false;
            return;
        }

        VisualElement characterView = panel.Q<VisualElement>("select-character-view");
        VisualElement descriptionView = panel.Q<VisualElement>("special-skill-description");
        // character-list-container自体はVisualElementで、その内側にScrollViewがある
        VisualElement iconListContainer = panel.Q<VisualElement>("character-list-container");
        iconList = iconListContainer != null ? iconListContainer.Q<ScrollView>() : null;
        if (characterView == null || descriptionView == null || iconList == null)
        {
            Debug.LogError("Character information UI elements are missing from the panel.", this);
            enabled = false;
            return;
        }

        // UXMLの実際の要素名に合わせ、詳細表示用の要素を取得する
        selectCharacterImage = characterView.Q<VisualElement>("image");
        specialSkillIcon = characterView.Q<VisualElement>("special-skill-icon");
        specialSkillDescription = descriptionView.Q<Label>();
        Button[] buttons = iconList.Query<Button>().ToList().ToArray();
        CharacterBaseData[] characterDatas = characterControlHandler.CharacterDatas;
        if (selectCharacterImage == null || specialSkillIcon == null || specialSkillDescription == null)
        {
            Debug.LogError("Character information detail elements are missing from the panel.", this);
            enabled = false;
            return;
        }
        if (characterDatas == null || buttons.Length < characterDatas.Length)
        {
            Debug.LogError("Character data or character icon buttons are not configured correctly.", this);
            enabled = false;
            return;
        }

        // UXMLのアイコン枠をキャラクターデータ順に割り当て、選択をロジックへ委譲する
        icons = new CharacterIcon[characterDatas.Length];
        for (int i = 0; i < characterDatas.Length; i++)
        {
            if (characterDatas[i] == null)
            {
                Debug.LogError($"Character data at index {i} is not set.", this);
                enabled = false;
                return;
            }

            icons[i] = new CharacterIcon { button = buttons[i], character = characterDatas[i] };
            CharacterIcon icon = icons[i];
            icon.button.clicked += () => SelectCharacter(icon);
            UpdateIcon(icon);
        }

        // キャラクター数より余分なUXMLアイコン枠は表示しない
        for (int i = characterDatas.Length; i < buttons.Length; i++)
            buttons[i].style.display = DisplayStyle.None;

        // 解放・切り替え・入力イベントを購読して、UIとロジックの状態を同期する
        characterControlHandler.OnCharacterUnlocked += OnCharacterUnlocked;
        characterControlHandler.OnCharacterChanged += OnCharacterChanged;
        inputController = PlayerInputController.Instance;
        if (inputController == null)
        {
            Debug.LogError("PlayerInputController is not available.", this);
            enabled = false;
            return;
        }

        inputController.OnInputQKeyPressed += ToggleCharacterInformationPanel;
        inputController.OnInputESCKeyDisabled += HideCharacterInformationPanel;
        OnCharacterChanged(characterControlHandler.CurrentCharacter);
    }

    protected override void OnSingletonDestroy()
    {
        // シーン破棄時にイベント購読を解除し、破棄済みUIへの通知を防ぐ
        if (characterControlHandler != null)
        {
            characterControlHandler.OnCharacterUnlocked -= OnCharacterUnlocked;
            characterControlHandler.OnCharacterChanged -= OnCharacterChanged;
        }

        if (inputController != null)
        {
            inputController.OnInputQKeyPressed -= ToggleCharacterInformationPanel;
            inputController.OnInputESCKeyDisabled -= HideCharacterInformationPanel;
        }
    }

    void UpdateIcon(CharacterIcon icon)
    {
        // 解放済みは0番、未解放は1番の番号付きアイコンで状態を示す
        int iconIndex = characterControlHandler.IsUnlocked(icon.character.CharacterNameEnum) ? 0 : 1;
        Sprite[] numberedIcons = icon.character.CharacterIcon_Numbered;
        if (numberedIcons == null || numberedIcons.Length <= iconIndex || numberedIcons[iconIndex] == null)
        {
            Debug.LogError($"Character numbered icon {iconIndex} is not set: {icon.character.CharacterNameEnum}", this);
            return;
        }

        icon.button.style.backgroundImage = new StyleBackground(numberedIcons[iconIndex]);
        icon.button.style.display = DisplayStyle.Flex;
    }

    void SelectCharacter(CharacterIcon icon)
    {
        // アイコンクリックでは操作キャラクターを変更せず、パネル内の情報表示だけ切り替える
        ShowCharacterInformation(icon.character);
    }

    void OnCharacterUnlocked(Character.Name characterName)
    {
        // 解放通知を該当キャラクターのアイコンへ反映する
        foreach (CharacterIcon icon in icons)
        {
            if (icon.character.CharacterNameEnum == characterName)
            {
                UpdateIcon(icon);
                break;
            }
        }
    }

    void OnCharacterChanged(Character.Name characterName)
    {
        // パネル外で操作キャラクターが変わった場合も、詳細情報を現在のキャラクターに同期する
        CharacterBaseData character = characterControlHandler.GetCharacterData(characterName);
        ShowCharacterInformation(character);
    }

    void ShowCharacterInformation(CharacterBaseData character)
    {
        // 選択中アイコンと立ち絵・特技情報を更新する。操作キャラクターの状態は変更しない
        if (character == null) return;

        foreach (CharacterIcon icon in icons)
        {
            if (icon.character == character)
                icon.button.AddToClassList("select-icon");
            else
                icon.button.RemoveFromClassList("select-icon");
        }

        selectCharacterImage.style.backgroundImage = new StyleBackground(character.CharacterStandingSprite);
        if (character.SpecialSkillIcon != null)
            specialSkillIcon.style.backgroundImage = new StyleBackground(character.SpecialSkillIcon);
        specialSkillDescription.text = character.SpecialSkillDescription ?? string.Empty;
    }

    void ToggleCharacterInformationPanel()
    {
        if (!isVisible)
            ShowCharacterInformationPanel();
        else
            HideCharacterInformationPanel();
    }

    void HideCharacterInformationPanel()
    {
        // パネルを閉じたとき入力状態も通常操作へ戻す
        SetVisible(false);
        inputController.ChangeState(StateType.None);
    }

    void ShowCharacterInformationPanel()
    {
        // 表示時に最新のロジック状態を再反映してから入力状態を変更する
        OnCharacterChanged(characterControlHandler.CurrentCharacter);
        SetVisible(true);
        inputController.ChangeState(StateType.CharacterInfo);
    }
}