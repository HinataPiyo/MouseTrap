using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

interface ITalk
{
    void OnToTalk(TalkData data, float delay);
    bool IsWriting { get; }
}

public class TalkPanelController : MonoBehaviour, ITalk
{
    UIDocument uiDocument;
    [SerializeField] float writingSpeed = 0.05f;
    TalkData currentTalkData;

    IGameState gameState;
    IInputTrigger inputTrigger;


    VisualElement root;

    Label characterName;
    Label body;
    VisualElement characterImage;
    VisualElement arrow;

    WaitForSeconds writingWait;

    Coroutine writingCoroutine;

    bool isWriting = false;
    bool isSkipping = false;

    public bool IsWriting => isWriting || currentTalkData != null;


    void Awake()
    {
        ApiProvider.Register<ITalk>(this);

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
        gameState = ApiProvider.Get<IGameState>();
        inputTrigger = ApiProvider.Get<IInputTrigger>();
        inputTrigger.OnInputSpaceKeyPressed += OnClick;
    }

    void OnClick()
    {
        if(currentTalkData == null) return;

        if (isWriting && writingCoroutine != null)
        {
            isSkipping = true;
        }
    }

    void OnDestroy()
    {
        inputTrigger.OnInputSpaceKeyPressed -= OnClick;
    }

    /// <summary>
    /// 会話を開始する
    /// </summary>
    /// <param name="data">会話データ</param>
    /// <param name="delay">会話開始までの遅延時間</param>
    public void OnToTalk(TalkData data, float delay = 0f)
    {
        currentTalkData = data;
        // キャラの操作をできなくする
        gameState.ChangeState(StateType.Talk);

        writingCoroutine = StartCoroutine(Writing(delay));
    }

    void OnTalkEnd()
    {
        gameState.ChangeState(StateType.None);
    }

    IEnumerator Writing(float delay = 0f)
    {
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

        writingCoroutine = null;
        currentTalkData = null;
        root.style.display = DisplayStyle.None;

        OnTalkEnd();        // 会話終了時の処理を呼び出す
    }
}