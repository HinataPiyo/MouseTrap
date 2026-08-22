using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public readonly struct ToTalk
{
    public readonly TalkData talkData;

    public ToTalk(TalkData talkData)
    {
        this.talkData = talkData;
    }
}

public class TalkPanelController : MonoBehaviour
{
    UIDocument uiDocument;
    [SerializeField] TalkData testData;
    [SerializeField] float writingSpeed = 0.05f;

    VisualElement root;

    Label characterName;
    Label body;
    VisualElement characterImage;
    VisualElement arrow;

    WaitForSeconds writingWait;

    Coroutine writingCoroutine;

    bool isWriting = false;
    bool isSkipping = false;


    void Awake()
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

    void Update()
    {
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if(writingCoroutine == null)
            {
                writingCoroutine = StartCoroutine(Writing());
            }
            else if(isWriting)
            {
                isSkipping = true;
            }
        }
    }

    void OnToTalk(ToTalk toTalk)
    {
        testData = toTalk.talkData;
        writingCoroutine = StartCoroutine(Writing());
    }

    IEnumerator Writing()
    {
        root.style.display = DisplayStyle.Flex;

        for (int i = 0; i < testData.entries.Length; i++)
        {
            var entry = testData.entries[i];
            isWriting = true;

            characterImage.style.backgroundImage = new StyleBackground(entry.character);
            characterName.text = Character.GetName(entry.characterName);
            arrow.style.display = DisplayStyle.None;
            body.text = "";

            foreach (char c in entry.body)
            {
                body.text += c;

                // スキップ中の場合はループを抜ける
                if(isSkipping)
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
        root.style.display = DisplayStyle.None;
    }
}