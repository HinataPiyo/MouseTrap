using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// 破れたメモを並べ替えUIを管理するクラス
/// </summary>
public class TornNotesUIPanel : PanelView<TornNotesUIPanel>
{
    UIDocument uiDoc;

    [SerializeField] Sprite returnPositionUI;

    List<Button> tornSelecterButtons = new List<Button>();
    List<Button> tornReturnButtons = new List<Button>();

    InteractData selectedTornNote;
    Button selectButton;
    Button backButton;

    InteractData[] tornNotes;       // 並べたメモ

    [Header("成功の並び")]
    [SerializeField] InteractData[] successLineNotes;

    [Header("成功のメモ(完成版)")]
    [SerializeField] InteractData successNote;

    PlayerInputController inputController;

    protected override void OnSingletonAwake()
    {
        uiDoc = GetComponent<UIDocument>();
        panel = uiDoc.rootVisualElement.Q("TornNotesPanel");
        backButton = panel.Q<Button>("back");
        tornSelecterButtons = panel.Q("torn-selecter-buttons").Query<Button>().ToList();
        tornReturnButtons = panel.Q("torn-return-button-panel").Query<Button>().ToList();
        tornNotes = new InteractData[tornReturnButtons.Count];

        for (int i = 0; i < tornSelecterButtons.Count; i++)
        {
            int index = i;
            // 下端のボタンを押下したときの処理
            tornSelecterButtons[i].clicked += () =>
            {
                selectedTornNote = Inventory.Instance.GetTornNotes()?.TornNotes[index];
                selectButton = tornSelecterButtons[index];
            };
        }

        // 上端のボタンを押下したときの処理
        for (int i = 0; i < tornReturnButtons.Count; i++)
        {
            int index = i;
            tornReturnButtons[i].clicked += () =>
            {
                if (selectedTornNote == null || tornNotes[index] != null) return;

                Button returnButton = tornReturnButtons[index];
                returnButton.iconImage = Background.FromSprite(selectedTornNote.Sprite);
                returnButton.text = "";
                selectButton.style.display = DisplayStyle.None;     // 下端のボタンは消す
                tornNotes[index] = selectedTornNote;        // 配列にセット
                selectedTornNote = null;
                selectButton = null;

                bool success = CheckSuccess();
                // 完成版メモを取得する
                if(success)
                {
                    Inventory.Instance.DeleteTornNotes();       // 破れたメモを消す
                    InventoryUIPanel.Instance.HideInventoryPanel(); // インベントリ画面を閉じる

                    // InteractしたときのPanelを表示する
                    InteractUIPanel.Instance.ShowInteractPanel(successNote);
                    Inventory.Instance.AddItem(successNote);
                }
                else
                {
                    // 並べたメモが正解でない場合の処理（全て並べてしまったときの処理）
                    //* System.Array.TrueForAllとは 配列の全要素が条件を満たすか確認するメソッド
                    if (System.Array.TrueForAll(tornNotes, note => note != null))
                    {
                        Reset();
                    }
                }
            };
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
        if(inputController != null)
            inputController.OnInputESCKeyDisabled -= Hide;
    }

    public void Show()
    {
        SetVisible(true);
        Reset();
    }

    public void Hide()
    {
        Reset();
        SetVisible(false);
    }

    void Reset()
    {
        System.Array.Clear(tornNotes, 0, tornNotes.Length);
        selectedTornNote = null;
        selectButton = null;

        if(Inventory.Instance == null) return;

        for (int i = 0; i < tornReturnButtons.Count; i++)
        {
            tornReturnButtons[i].style.backgroundImage = new StyleBackground(returnPositionUI);
            tornReturnButtons[i].iconImage = null;
            tornReturnButtons[i].text = "";
        }

        for (int i = 0; i < tornSelecterButtons.Count; i++)
        {
            InteractTornNotesData torn = Inventory.Instance.GetTornNotes();     // 破れたメモ取得

            if(torn == null || i >= torn.TornNotes.Length)
            {
                tornSelecterButtons[i].style.display = DisplayStyle.None;
                continue;
            }

            // ボタンの数と一致しない場合はerror処理
            if(torn.TornNotes.Length > tornSelecterButtons.Count)
            {
                Debug.LogError("Too many torn notes");
                return;
            }

            tornSelecterButtons[i].style.display = DisplayStyle.Flex;
            tornSelecterButtons[i].style.backgroundImage = new StyleBackground(torn.TornNotes[i].Sprite);
        }
    }

    bool CheckSuccess()
    {
        if (successLineNotes == null || successLineNotes.Length != tornNotes.Length)
        {
            Debug.LogError("The number of correct notes must match the number of placement buttons.", this);
            return false;
        }

        // 並べたメモが正解しているか確認
        for (int i = 0; i < tornNotes.Length; i++)
        {
            if(tornNotes[i] == null || tornNotes[i] != successLineNotes[i]) return false;
        }

        return true;
    }
}