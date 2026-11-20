using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 検索用PC用のパネルを表示する
/// </summary>
public partial class SearchPCUIPanel : PanelView<SearchPCUIPanel>
{
    UIDocument uiDoc;

    bool isPasswordComplete;        // PC画面表示の際初期状態ではパスワード入力画面となる

    VisualElement passwordField;
    Label passwordLabel;
    Label errorLabel;

    Button clearButton;
    Button excuteButton;
    Button[] numberButtons = new Button[10];
    List<int> inputNumber = new List<int>();

    const int MaxInput = 3;
    static readonly int Pasword = 105;

    PlayerInputController inputController;

    protected override void OnSingletonAwake()
    {
        uiDoc = GetComponent<UIDocument>();
        passwordField = uiDoc.rootVisualElement.Q("password-panel");
        passwordLabel = uiDoc.rootVisualElement.Q<Label>("input-value");
        clearButton = uiDoc.rootVisualElement.Q<Button>("clear");
        excuteButton = uiDoc.rootVisualElement.Q<Button>("excute");
        errorLabel = uiDoc.rootVisualElement.Q<Label>("error");
        List<Button> buttons = uiDoc.rootVisualElement.Q<VisualElement>("number-buttons").Query<Button>().ToList();

        int index = 0;
        for (int i = 0; i < numberButtons.Length; i++)
        {
            index++;
            numberButtons[i] = buttons[i];

            if(index == 10) index = 0;
            
            buttons[i].text = index.ToString();
            RegisterPasswordButtons(buttons[i], index);
        }
        clearButton.clicked += ClearInput;
        excuteButton.clicked += Excute;

        SearchPanelInit();

        Hide();
    }

    protected override void OnSingletonDestroy()
    {
        if(inputController != null) inputController.OnInputESCKeyDisabled -= Hide;
    }

    void Start()
    {
        inputController = PlayerInputController.Instance;
        inputController.OnInputESCKeyDisabled += Hide;
    }

    public void Show()
    {
        uiDoc.rootVisualElement.style.display = DisplayStyle.Flex;

        // パスワード完了なら検索パネルへ
        if(isPasswordComplete) BackToGenrePanel();
        else
        {
            searchPanel.style.display = DisplayStyle.None;
            passwordField.style.display = DisplayStyle.Flex;
        }

        ClearInput();
    }

    public void Hide()
    {
        uiDoc.rootVisualElement.style.display = DisplayStyle.None;
    }

    /// <summary>
    /// ボタンを登録する
    /// </summary>
    /// <param name="num">ボタンの番号</param>
    void RegisterPasswordButtons(Button btn,int num)
    {
        btn.clicked += () =>
        {
            if(inputNumber.Count < MaxInput) inputNumber.Add(num);
            passwordLabel.text = string.Join("", inputNumber);
        };
    }

    /// <summary>
    /// 入力をクリアする処理
    /// </summary>
    void ClearInput()
    {
        inputNumber.Clear();
        passwordLabel.text = "";
        errorLabel.style.display = DisplayStyle.None;
    }

    /// <summary>
    /// 確定ボタンを押下したときの処理
    /// </summary>
    void Excute()
    {
        // 入力が足りない場合は弾く
        if(inputNumber.Count < MaxInput)
        {
            errorLabel.style.display = DisplayStyle.Flex;
            return;
        }


        int input = int.Parse(string.Join("", inputNumber));        // 入力値を取得
        // 正しいか確認
        if(input == Pasword)
        {
            isPasswordComplete = true;
            passwordField.style.display = DisplayStyle.None;
            BackToGenrePanel();
        }
        else errorLabel.style.display = DisplayStyle.Flex;
    }
}