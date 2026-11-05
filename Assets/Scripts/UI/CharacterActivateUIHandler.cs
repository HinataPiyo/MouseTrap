using UnityEngine;
using UnityEngine.UIElements;
using Player;

/// <summary>
/// キャラクターアイコンの表示を管理するクラス
/// </summary>
public class CharacterActivateUIHandler : SceneSingleton<CharacterActivateUIHandler>
{
    UIDocument uiDoc;
    CharacterIcon[] icons;
    // 解放・切り替えの正本であるプレイヤーロジックを参照する
    CharacterControlHandler characterControlHandler;

    public class CharacterIcon
    {
        public Button button;
        public CharacterBaseData character;
    }

    static readonly string SelectAnimationTag = "select-icon";
    Button currentButton;

    void Start()
    {
        // ロジック側のイベントを購読し、UIと解放・切り替え状態を同期する
        characterControlHandler = CharacterControlHandler.Instance;
        if (characterControlHandler == null)
        {
            Debug.LogError("CharacterControlHandler is not available.", this);
            enabled = false;
            return;
        }

        uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null)
        {
            Debug.LogError("UIDocument is not set on CharacterActivateUIHandler.", this);
            enabled = false;
            return;
        }

        // ボタン群はVisualElementなので、親要素を取得してから配下のButtonを列挙する
        VisualElement buttonContainer = uiDoc.rootVisualElement.Q<VisualElement>("character-buttons");
        if (buttonContainer == null)
        {
            Debug.LogError("The character-buttons element was not found in the UIDocument.", this);
            enabled = false;
            return;
        }

        Button[] buttons = buttonContainer.Query<Button>().ToList().ToArray();
        icons = new CharacterIcon[buttons.Length];
        for(int i = 0; i < buttons.Length; i++)
        {
            // 未解放キャラクターも表示し、クリック可否は既存ロジックの解放判定に委ねる
            icons[i] = new CharacterIcon { button = buttons[i] };
            icons[i].button.style.display = DisplayStyle.None;
            CharacterIcon icon = icons[i];
            icon.button.clicked += () => SelectCharacter(icon);
        }

        characterControlHandler.OnCharacterUnlocked += OnCharacterUnlocked;
        characterControlHandler.OnCharacterChanged += OnCharacterChanged;

        // 未解放キャラクターも1番の番号付きアイコンで表示し、解放状態だけ操作可否に反映する
        foreach (CharacterBaseData character in characterControlHandler.GetAllCharacterData())
            ActivateIcon(character);
        ChangeCurrentCharacter(characterControlHandler.GetCharacterData(characterControlHandler.CurrentCharacter));
    }

    protected override void OnSingletonDestroy()
    {
        // シーン破棄時にイベント購読を解除し、破棄済みUIへの通知を防ぐ
        if (characterControlHandler == null) return;

        characterControlHandler.OnCharacterUnlocked -= OnCharacterUnlocked;
        characterControlHandler.OnCharacterChanged -= OnCharacterChanged;
    }

    public void ActivateIcon(CharacterBaseData character)
    {
        // 不正なキャラクターデータはアイコン表示に反映しない
        if (character == null) return;

        for(int i = 0; i < icons.Length; i++)
        {
            if (icons[i].character == character)
            {
                // 解放状態に応じて通常アイコンを更新し、ボタンの有効状態は変更しない
                UpdateIcon(icons[i]);
                return;
            }

            if(icons[i].button.style.display == DisplayStyle.None)
            {
                icons[i].button.style.display = DisplayStyle.Flex;
                icons[i].character = character;
                // データ配列順の枠を常時表示し、未解放/解放済みで0番・1番の通常アイコンを選ぶ
                UpdateIcon(icons[i]);
                break;
            }
        }
    }

    void UpdateIcon(CharacterIcon icon)
    {
        // 番号付きアイコンの0番は解放済み、1番は未解放表示に使う。SetEnabledは変更しない
        int iconIndex = characterControlHandler.IsUnlocked(icon.character.CharacterNameEnum) ? 0 : 1;
        Sprite[] numberedIcons = icon.character.CharacterIcon_Numbered;
        if (numberedIcons == null || numberedIcons.Length <= iconIndex || numberedIcons[iconIndex] == null)
        {
            Debug.LogError($"Character numbered icon {iconIndex} is not set: {icon.character.CharacterNameEnum}", this);
            return;
        }

        icon.button.style.backgroundImage = new StyleBackground(numberedIcons[iconIndex]);
    }

    public void ChangeCurrentCharacter(CharacterBaseData character)
    {
        // 切り替え先のデータが見つからない場合は現在の選択表示を維持する
        if (character == null) return;

        for(int i = 0; i < icons.Length; i++)
        {
            if(icons[i].character == character)
            {
                if(currentButton != null)
                    currentButton.RemoveFromClassList(SelectAnimationTag);

                currentButton = icons[i].button;
                icons[i].button.AddToClassList(SelectAnimationTag);
                break;
            }
        }
    }

    // 解放通知をアイコン表示へ反映する
    void OnCharacterUnlocked(Character.Name characterName)
    {
        ActivateIcon(characterControlHandler.GetCharacterData(characterName));
    }

    // キーボード操作などUI外からの切り替えも選択表示へ反映する
    void OnCharacterChanged(Character.Name characterName)
    {
        ChangeCurrentCharacter(characterControlHandler.GetCharacterData(characterName));
    }

    // ボタン操作を既存のキャラクター切り替えロジックへ委譲する
    void SelectCharacter(CharacterIcon icon)
    {
        if (icon.character != null)
            characterControlHandler.ChangeCharacterControlState(icon.character.CharacterNameEnum);
    }
}