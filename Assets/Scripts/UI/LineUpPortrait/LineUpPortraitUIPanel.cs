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
    Character.PortraitOrder[] portraitOrders;
    bool[] hasPortraits;
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
        portraitOrders = new Character.PortraitOrder[returnButtons.Count];
        hasPortraits = new bool[returnButtons.Count];
        backButton = panel.Q<Button>("back");

        selecterPortaitPanel = panel.Q("selecter-portrait-panel");

        // 肖像画を設置する箇所を選択する
        for (int i = 0; i < returnButtons.Count; i++)
        {
            int index = i;
            returnButtons[i].clicked += () =>
            {

                // 既に肖像画が置かれている場合は戻す
                if (hasPortraits[index])
                {
                    int selectorIndex = -1;
                    if (portaitInfos != null)
                    {
                        for (int j = 0; j < portaitInfos.Length; j++)
                        {
                            if (portaitInfos[j] != null && portaitInfos[j].order == portraitOrders[index])
                            {
                                selectorIndex = j;
                                break;
                            }
                        }
                    }

                    if (selectorIndex < 0 || selectorIndex >= selectButtons.Count)
                    {
                        Debug.LogError("Could not find the placed portrait in the selector.", this);
                        return;
                    }

                    selectButtons[selectorIndex].style.display = DisplayStyle.Flex;
                    returnButtons[index].style.backgroundImage = Background.FromSprite(returnPositionUI);
                    portraitOrders[index] = default(Character.PortraitOrder);
                    hasPortraits[index] = false;
                }

                selectReturnIndex = index;
                SelecterPanelShow();
            };
        }

        // 配置する肖像画を選ぶボタンに設定する
        for (int i = 0; i < selectButtons.Count; i++)
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
        if (inputController != null)
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
    /// 肖像画を配置する
    /// </summary>
    void SetPortrait(PortaitInfo info)
    {
        Button selectReturnButton = returnButtons[selectReturnIndex];
        selectReturnButton.style.backgroundImage = Background.FromSprite(info.image);
        // 設置した肖像画を記録する
        portraitOrders[selectReturnIndex] = info.order;
        hasPortraits[selectReturnIndex] = true;
    }

    void Reset()
    {
        for (int i = 0; i < returnButtons.Count; i++)
        {
            returnButtons[i].style.backgroundImage = Background.FromSprite(returnPositionUI);
            returnButtons[i].pickingMode = PickingMode.Position;
            hasPortraits[i] = false;
        }

        for (int i = 0; i < selectButtons.Count; i++)
        {
            selectButtons[i].style.display = DisplayStyle.Flex;
        }

        System.Array.Clear(portraitOrders, 0, portraitOrders.Length);
        SelecterPanelHide();
    }
}