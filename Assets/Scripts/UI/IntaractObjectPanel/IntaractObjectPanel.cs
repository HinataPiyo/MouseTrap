using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;


public enum IntaractMarkType { None, Exclamation, Question }
public readonly struct ShowIntaractMark
{
    public readonly IIntaract intaract;

    public ShowIntaractMark(IIntaract intaract)
    {
        this.intaract = intaract;
    }
}

public readonly struct HideIntaractMark
{
    public readonly IIntaract intaract;

    public HideIntaractMark(IIntaract intaract)
    {
        this.intaract = intaract;
    }
}

public readonly struct ShowIntaractObjectPanel
{
    public readonly Sprite sprite;
    public readonly string text;

    public ShowIntaractObjectPanel(Sprite sprite, string text)
    {
        this.sprite = sprite;
        this.text = text;
    }
}

public readonly struct HideIntaractObjectPanel {
    
}

public readonly struct ToggleIntaractObjectPanelVisibility
{
    public readonly ShowIntaractObjectPanel show;

    public ToggleIntaractObjectPanelVisibility(ShowIntaractObjectPanel show)
    {
        this.show = show;
    }
}

public class IntaractObjectPanel : PanelView
{
    [Header("Intaract")]
    [SerializeField] Canvas canvas;
    [SerializeField] GameObject intaractMarkPrefab;

    List<CreatedMark> createdMarks = new List<CreatedMark>();

    readonly struct CreatedMark
    {
        public readonly IntaractMark mark;
        public readonly IIntaract intaract;

        public CreatedMark(IntaractMark mark, IIntaract intaract)
        {
            this.mark = mark;
            this.intaract = intaract;
        }
    }


    UIDocument uiDocument;

    VisualElement image;
    Label text;

    bool isVisible = false;

    IEventBus eventBus;

    [Inject]
    public void Construct(IEventBus eventBus)
    {
        this.eventBus = eventBus;
        eventBus.Subscribe<ToggleIntaractObjectPanelVisibility>(OnTogglePanelVisibility);

        eventBus.Subscribe<ShowIntaractObjectPanel>(ShowIntaractObjectPanel);
        eventBus.Subscribe<HideIntaractObjectPanel>(data => HideIntaractObjectPanel());

        eventBus.Subscribe<ShowIntaractMark>(data => ShowIntaractMark(data.intaract));
        eventBus.Subscribe<HideIntaractMark>(data => HideIntaractMark(data.intaract));
    }

    void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        panel = root.Q<VisualElement>("get-item-container");
        image = panel.Q<VisualElement>("image");
        text = panel.Q<Label>("text");

        HideIntaractObjectPanel();
        SetVisible(false);
    }

    /// <summary>
    /// ShowIntaractObjectPanelを受け取り、パネルを表示する
    /// </summary>
    void ShowIntaractObjectPanel(ShowIntaractObjectPanel data)
    {
        image.style.backgroundImage = data.sprite != null ? new StyleBackground(data.sprite) : new StyleBackground();
        text.text = data.text;
    }

    /// <summary>
    /// HideIntaractObjectPanelを受け取り、パネルを非表示にする
    /// </summary>
    void HideIntaractObjectPanel()
    {
        image.style.backgroundImage = new StyleBackground();
        text.text = string.Empty;
    }

    void OnTogglePanelVisibility(ToggleIntaractObjectPanelVisibility data)
    {
        isVisible = !isVisible;
        SetVisible(isVisible);

        if(isVisible)
            eventBus.Publish(data.show);
        else
            eventBus.Publish(new HideIntaractObjectPanel());
    }

    /// <summary>
    /// IntaractMarkを表示する
    /// </summary>
    void ShowIntaractMark(IIntaract intaract)
    {
        // すでに表示されている場合は何もしない
        if(createdMarks.Exists(p => p.intaract == intaract)) return;

        GameObject obj = Instantiate(intaractMarkPrefab, canvas.transform);
        IntaractMark mark = obj.GetComponent<IntaractMark>();
        mark.SetMark(intaract.IntaractMarkType, intaract.GetMarkPoint());

        createdMarks.Add(new CreatedMark(mark, intaract));
    }

    /// <summary>
    /// IntaractMarkを非表示にする
    /// </summary>
    /// <param name="position">非表示にするIntaractMarkの位置</param>
    void HideIntaractMark(IIntaract intaract)
    {
        // すでに非表示になっている場合は何もしない
        if(!createdMarks.Exists(p => p.intaract == intaract)) return;

        var createdMark = createdMarks.Find(p => p.intaract == intaract);
        if (createdMark.mark != null)
        {
            Destroy(createdMark.mark.gameObject);
            createdMarks.Remove(createdMark);
        }
    }
}