using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UIElements;

public class SafePasswordUIPanel : PanelView<SafePasswordUIPanel>
{
    [SerializeField] SafePassword safePassword;
    Button backButton;
    Label inputValue;
    List<int> inputNumber = new List<int>();
    static readonly string SuccessPassword = "17324";
    static readonly int MaxInput = 5;       // 5桁まで可能
    PlayerInputController inputController;

    public bool IsSuccess { get; private set; } = false;

    protected override void OnSingletonAwake()
    {
        panel = GetComponent<UIDocument>().rootVisualElement;
        List<Button> numberButtons = new List<Button>();
        numberButtons = panel.Q("number-buttons").Query<Button>().ToList();
        backButton = panel.Q<Button>("back");
        inputValue = panel.Q<Label>("input-value");
        backButton.clicked += () => Hide();

        for(int i = 0; i < numberButtons.Count; i++)
        {
            int index = i;
            numberButtons[index].clicked += () =>
            {
                if(inputNumber.Count < MaxInput) inputNumber.Add((index + 1) % 10);
                inputValue.text = string.Join("", inputNumber);

                if(inputNumber.Count < MaxInput) return;

                if(inputValue.text == SuccessPassword)
                {
                    IsSuccess = true;
                    safePassword.PasswordSuccess();
                    Hide();        // 金庫が解除できた場合
                    return;
                }

                Reset();
            };
        }
    }

    void Start()
    {
        inputController = PlayerInputController.Instance;
        inputController.OnInputESCKeyDisabled += Hide;
        Hide();
    }

    protected override void OnSingletonDestroy()
    {
        if(inputController != null)
            inputController.OnInputESCKeyDisabled -= Hide;
    }

    public void Show()
    {
        if(IsSuccess) return;
        SetVisible(true);
        Reset();
    }

    public void Hide()
    {
        SetVisible(false);
    }

    void Reset()
    {
        inputNumber.Clear();
        inputValue.text = "";
    }
}