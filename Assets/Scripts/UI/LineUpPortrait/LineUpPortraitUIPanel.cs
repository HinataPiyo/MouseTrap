using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LineUpPortraitUIPanel : PanelView<LineUpPortraitUIPanel>
{
    [SerializeField] Sprite returnPositionUI;
    [SerializeField] PortaitInfo[] portaitInfos;
    VisualElement selecterPortaitPanel;
    Button backButton;

    List<Button> returnButtons = new List<Button>();
    List<Button> selectButtons = new List<Button>();

    PlayerInputController inputController;
    Character.PortraitOrder[] portraitOrders = new Character.PortraitOrder[5];
    static readonly Character.PortraitOrder[] successPortrait = new Character.PortraitOrder[]
    {
        Character.PortraitOrder.Teacher,
        Character.PortraitOrder.GlassesBoy,
        Character.PortraitOrder.BandageBoy,
        Character.PortraitOrder.DollGirl,
        Character.PortraitOrder.ReturnedBoy
    };

    [System.Serializable]
    public class PortaitInfo
    {
        public Character.PortraitOrder order;
        public Sprite image;
    }

    int selectReturnIndex;

    protected override void OnSingletonAwake()
    {
        panel = GetComponent<UIDocument>().rootVisualElement;
        returnButtons = panel.Q("return-portrait-buttons").Query<Button>().ToList();
        selectButtons = panel.Q("selecter-portrait-buttons").Query<Button>().ToList();
        backButton = panel.Q<Button>("back");

        selecterPortaitPanel = panel.Q("selecter-portrait-panel");

        // 肖像画を設置する箇所を選択する
        for(int i = 0; i < returnButtons.Count; i++)
        {
            int index = i;
            returnButtons[i].clicked += () =>
            {
                selectReturnIndex = index;
                SelecterPanelShow();
            };
        }

        // 配置する肖像画を選ぶボタンに設定する
        for(int i = 0; i < selectButtons.Count; i++)
        {
            int index = i;
            selectButtons[i].clicked += () =>
            {
                SelecterPanelHide();
                SetPortrait(portaitInfos[index]);
                selectButtons[index].style.display = DisplayStyle.None;
            };

            selectButtons[i].style.backgroundImage = Background.FromSprite(portaitInfos[i].image);
            selectButtons[i].style.display = DisplayStyle.Flex;
        }

        backButton.clicked += SelecterPanelHide;
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
        SetVisible(true);
        Reset();
    }

    public void Hide()
    {
        SetVisible(false);
    }

    void SelecterPanelHide()
    {
        selecterPortaitPanel.style.display = DisplayStyle.None;
    }

    void SelecterPanelShow()
    {
        selecterPortaitPanel.style.display = DisplayStyle.Flex;
    }

    /// <summary>
    /// 空の場所に肖像画を置く
    /// </summary>
    void SetPortrait(PortaitInfo info)
    {
        Button selectReturnButton = returnButtons[selectReturnIndex];
        selectReturnButton.style.backgroundImage = Background.FromSprite(info.image);
        // 設置した肖像画を記録する
        portraitOrders[selectReturnIndex] = info.order;
        selectReturnButton.pickingMode = PickingMode.Ignore;        // クリック無効にする
    }

    void Reset()
    {
        for(int i = 0; i < returnButtons.Count; i++)
        {
            returnButtons[i].style.backgroundImage = Background.FromSprite(returnPositionUI);
            returnButtons[i].pickingMode = PickingMode.Position;
        }

        for(int i = 0; i < selectButtons.Count; i++)
        {
            selectButtons[i].style.display = DisplayStyle.Flex;
        }

        portraitOrders = new Character.PortraitOrder[5];
        SelecterPanelHide();
    }
}