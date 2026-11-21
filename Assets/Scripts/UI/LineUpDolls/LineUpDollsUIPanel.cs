using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LineUpDollsUIPanel : PanelView<LineUpDollsUIPanel>
{
    [SerializeField] Sprite returnPositionUI;
    UIDocument uiDoc;
    VisualElement selecterPanel;

    List<Button> returnDollButtons = new List<Button>();
    List<Button> selectDollButtons = new List<Button>();

    InteractDollData[] currentDolls = new InteractDollData[5];

    [SerializeField] InteractDollData[] successDolls;

    Button backButton;

    InteractDollData selectedDoll;
    Button selecterButton;
    bool hasValidAnswer;

    PlayerInputController inputController;

    public bool IsSuccess { get; private set; } = false;
    protected override void OnSingletonAwake()
    {
        uiDoc = GetComponent<UIDocument>();
        panel = uiDoc.rootVisualElement;
        selecterPanel = panel.Q("selecter-doll-panel");
        backButton = panel.Q<Button>("back");

        returnDollButtons = panel.Q("return-doll-buttons").Query<Button>().ToList();
        selectDollButtons = panel.Q("selecter-doll-buttons").Query<Button>().ToList();

        hasValidAnswer = successDolls != null &&
                         successDolls.Length == currentDolls.Length &&
                         returnDollButtons.Count == currentDolls.Length;
        if(hasValidAnswer)
        {
            for(int i = 0; i < successDolls.Length; i++)
                if(successDolls[i] == null) hasValidAnswer = false;
        }
        if(!hasValidAnswer)
            Debug.LogError("人形の正解配置と棚の数を確認してください。", this);

        Reset();

        // 棚に戻すボタンを押下したときの処理
        for(int i = 0; i < returnDollButtons.Count; i++)
        {
            int index = i;
            returnDollButtons[i].clicked += () =>
            {
                if(!hasValidAnswer || IsSuccess || selectedDoll == null || currentDolls[index] != null)
                    return;

                currentDolls[index] = selectedDoll;
                returnDollButtons[index].style.backgroundImage = Background.FromSprite(selectedDoll.ObjectSprite);
                selecterButton.style.display = DisplayStyle.None;
                selectedDoll = null;
                selecterButton = null;

                for(int j = 0; j < currentDolls.Length; j++)
                {
                    if(currentDolls[j] == null) return;
                }

                IsSuccess = CheckSuccess();
                if(!IsSuccess)
                {
                    Reset();
                    UpdateHasDollData();
                }
            };
        }

        // 所持してる人形を選択したときの処理
        for(int i = 0; i < selectDollButtons.Count; i++)
        {
            int index = i;
            selectDollButtons[i].clicked += () =>
            {
                selectedDoll = Inventory.Instance.GetDoll(index);
                selecterButton = selectDollButtons[index];
            };
        }

        backButton.clicked += Hide;
    }

    void Start()
    {
        inputController = PlayerInputController.Instance;
        inputController.OnInputESCKeyDisabled += Hide;
        Hide();
    }

    protected override void OnSingletonDestroy()
    {
        backButton.clicked -= Hide;
        inputController.OnInputESCKeyDisabled -= Hide;
    }

    public void Show()
    {
        SetVisible(true);
        UpdateHasDollData();
    }

    public void Hide()
    {
        selectedDoll = null;
        selecterButton = null;
        SetVisible(false);
        inputController.ChangeState(StateType.None);
    }

    /// <summary>
    /// 所持してる人形UIを更新する
    /// </summary>
    void UpdateHasDollData()
    {
        InteractDollData[] hasDolls = Inventory.Instance.GetDolls();
        for(int i = 0; i < selectDollButtons.Count; i++)
        {
            if(i >= hasDolls.Length) 
            {
                selectDollButtons[i].style.backgroundImage = null;
                selectDollButtons[i].style.display = DisplayStyle.None;
                continue;
            }

            selectDollButtons[i].style.display = System.Array.IndexOf(currentDolls, hasDolls[i]) >= 0
                ? DisplayStyle.None : DisplayStyle.Flex;
            selectDollButtons[i].style.backgroundImage = Background.FromSprite(hasDolls[i].UISprite);
        }
    }

    void Reset()
    {
        selectedDoll = null;
        selecterButton = null;
        IsSuccess = false;
        System.Array.Clear(currentDolls, 0, currentDolls.Length);

        for(int i = 0; i < returnDollButtons.Count; i++)
        {
            returnDollButtons[i].style.backgroundImage = Background.FromSprite(returnPositionUI);
        }
    }

    bool CheckSuccess()
    {
        for(int i = 0; i < currentDolls.Length; i++)
            if(currentDolls[i] != successDolls[i]) return false;
        return true;
    }
}