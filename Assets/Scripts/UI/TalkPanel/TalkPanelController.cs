using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class TalkPanelController : PanelView<TalkPanelController>
{
    [SerializeField] float writingSpeed = 0.05f;
    TalkData currentTalkData;

    PlayerInputController gameState;
    PlayerInputController inputTrigger;

    Label characterName;
    Label body;
    VisualElement characterImage;
    VisualElement arrow;

    WaitForSeconds writingWait;

    bool isWriting = false;
    bool isSkipping = false;
    bool isWaitingForAdvance = false;
    bool isAdvanceRequested = false;
    int talkSessionId;

    public bool IsWriting => isWriting || currentTalkData != null;


    protected override void OnSingletonAwake()
    {
        panel = GetComponent<UIDocument>().rootVisualElement.Q("root");
        characterName = panel.Q<Label>("name");
        body = panel.Q<Label>("body");
        characterImage = panel.Q<VisualElement>("left-character");
        arrow = panel.Q<VisualElement>("arrow");
        writingWait = new WaitForSeconds(writingSpeed);

        SetVisible(false);
    }

    void Start()
    {
        gameState = PlayerInputController.Instance;
        inputTrigger = PlayerInputController.Instance;
        inputTrigger.OnInputSpaceKeyPressed += OnClick;
        inputTrigger.OnInputESCKeyDisabled += CloseTalkPanel;
    }

    void OnClick()
    {
        if(currentTalkData == null) return;

        if (isWriting)
        {
            isSkipping = true;
            return;
        }

        if (isWaitingForAdvance)
            isAdvanceRequested = true;
    }

    protected override void OnSingletonDestroy()
    {
        // Startの前に破棄された場合はnull
        if (inputTrigger != null)
        {
            inputTrigger.OnInputSpaceKeyPressed -= OnClick;
            inputTrigger.OnInputESCKeyDisabled -= CloseTalkPanel;
        }
    }

    void CloseTalkPanel()
    {
        talkSessionId++;
        currentTalkData = null;
        isWriting = false;
        isSkipping = false;
        isWaitingForAdvance = false;
        isAdvanceRequested = false;
        SetVisible(false);
        gameState.ChangeState(StateType.None);
    }

    void OnTalkEnd()
    {
        gameState.ChangeState(StateType.None);
    }

    public void BackgroundHide()
    {
        Color color = new Color(0,0,0,0f);
        panel.style.backgroundColor = color;
    }

    public void BackgroundShow()
    {
        Color color = new Color(0,0,0,0.5f);
        panel.style.backgroundColor = color;
    }

    public IEnumerator Writing(TalkData data, float delay = 0f, bool background = true)
    {
        int sessionId = ++talkSessionId;
        currentTalkData = data;
        gameState.ChangeState(StateType.Talk);
        isSkipping = false;
        isWaitingForAdvance = false;
        isAdvanceRequested = false;

        yield return new WaitForSeconds(delay);
        if (sessionId != talkSessionId) yield break;

        SetVisible(true);
        if(background) BackgroundShow();
        else BackgroundHide();

        for (int i = 0; i < data.entries.Length; i++)
        {
            var entry = data.entries[i];
            yield return new WaitForSeconds(entry.delay);
            isWriting = true;

            characterImage.style.backgroundImage = entry.character != null ? new StyleBackground(entry.character) : null;
            characterName.text = Character.GetName(entry.characterName);
            arrow.style.display = DisplayStyle.None;
            body.text = "";

            foreach (char c in entry.body)
            {
                body.text += c;

                // スキップ中の場合はループを抜ける
                if (isSkipping)
                {
                    isSkipping = false;
                    break;
                }

                yield return writingWait;
                if (sessionId != talkSessionId) yield break;
            }

            body.text = entry.body;
            arrow.style.display = DisplayStyle.Flex;
            isWriting = false;
            isWaitingForAdvance = true;
            yield return new WaitUntil(() => sessionId != talkSessionId || isAdvanceRequested);
            if (sessionId != talkSessionId) yield break;
            isWaitingForAdvance = false;
            isAdvanceRequested = false;
        }
        currentTalkData = null;
        SetVisible(false);

        OnTalkEnd();        // 会話終了時の処理を呼び出す
    }
}