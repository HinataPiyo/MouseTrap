using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TalkPanelController : SceneSingleton<TalkPanelController>
{
    UIDocument uiDocument;
    [SerializeField] float writingSpeed = 0.05f;
    TalkData currentTalkData;

    PlayerInputController gameState;
    PlayerInputController inputTrigger;


    VisualElement root;

    Label characterName;
    Label body;
    VisualElement characterImage;
    VisualElement arrow;

    WaitForSeconds writingWait;

    bool isWriting = false;
    bool isSkipping = false;

    public bool IsWriting => isWriting || currentTalkData != null;


    protected override void OnSingletonAwake()
    {
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;
        characterName = root.Q<Label>("name");
        body = root.Q<Label>("body");
        characterImage = root.Q<VisualElement>("left-character");
        arrow = root.Q<VisualElement>("arrow");
        writingWait = new WaitForSeconds(writingSpeed);

        root.style.display = DisplayStyle.None;
    }

    void Start()
    {
        gameState = PlayerInputController.Instance;
        inputTrigger = PlayerInputController.Instance;
        inputTrigger.OnInputSpaceKeyPressed += OnClick;
    }

    void OnClick()
    {
        if(currentTalkData == null) return;

        if (isWriting)
        {
            isSkipping = true;
        }
    }

    protected override void OnSingletonDestroy()
    {
        // Startの前に破棄された場合はnull
        if (inputTrigger != null)
            inputTrigger.OnInputSpaceKeyPressed -= OnClick;
    }

    void OnTalkEnd()
    {
        gameState.ChangeState(StateType.None);
    }

    public IEnumerator Writing(TalkData data, float delay = 0f)
    {
        currentTalkData = data;
        gameState.ChangeState(StateType.Talk);
        isSkipping = false;

        yield return new WaitForSeconds(delay);

        root.style.display = DisplayStyle.Flex;

        for (int i = 0; i < currentTalkData.entries.Length; i++)
        {
            var entry = currentTalkData.entries[i];
            isWriting = true;

            characterImage.style.backgroundImage = new StyleBackground(entry.character);
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
            }

            body.text = entry.body;
            arrow.style.display = DisplayStyle.Flex;
            isWriting = false;
            yield return new WaitUntil(() => Keyboard.current.spaceKey.wasPressedThisFrame);
        }
        currentTalkData = null;
        root.style.display = DisplayStyle.None;

        OnTalkEnd();        // 会話終了時の処理を呼び出す
    }
}