using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LineUpDollsUIPanel : PanelView<LineUpDollsUIPanel>
{
    [SerializeField] Sprite returnPositionUI;
    [SerializeField] LineUpDollHandler handler;
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

        // 正解配置が有効か確認
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
                if(!hasValidAnswer || IsSuccess)
                    return;

                bool returnedDoll = false;

                // 既に人形が置かれている場合は戻す
                if(currentDolls[index] != null)
                {
                    InteractDollData[] dolls = Inventory.Instance.GetDolls();
                    int returnedDollIndex = System.Array.IndexOf(dolls, currentDolls[index]);
                    if(returnedDollIndex < 0 || returnedDollIndex >= selectDollButtons.Count)
                    {
                        Debug.LogError("Could not find the placed doll in the inventory.", this);
                        return;
                    }

                    selectDollButtons[returnedDollIndex].style.display = DisplayStyle.Flex;
                    returnDollButtons[index].style.backgroundImage = Background.FromSprite(returnPositionUI);
                    currentDolls[index] = null;
                    returnedDoll = true;
                }

                if(selectedDoll == null || selecterButton == null)
                {
                    if (returnedDoll)
                        AudioManager.Instance.PlaySE("キャンセル");
                    return;
                }

                currentDolls[index] = selectedDoll;
                returnDollButtons[index].style.backgroundImage = Background.FromSprite(selectedDoll.ObjectSprite);
                selecterButton.style.display = DisplayStyle.None;
                selectedDoll = null;
                selecterButton = null;
                AudioManager.Instance.PlaySE("置く");

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
                else
                {
                    // 所持中の人形をインベントリから消す
                    Inventory.Instance.RemoveAllDoll();

                    // 人形並び替え完了後の処理
                    handler.CompletedEvent();
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
                AudioManager.Instance.PlaySE("アイテム選択");
            };
        }

        backButton.clicked += Hide;
        backButton.clicked += () => AudioManager.Instance.PlaySE("キャンセル");
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
        GameUIHandler.Instance.SetGimmickActive(true);
        UpdateHasDollData();
    }

    public void Hide()
    {
        selectedDoll = null;
        selecterButton = null;
        SetVisible(false);
        GameUIHandler.Instance.SetGimmickActive(false);
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