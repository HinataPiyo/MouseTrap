using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;


public enum InteractMarkType { None, Exclamation, Question }
public readonly struct ShowInteractMark
{
    public readonly IInteract interact;

    public ShowInteractMark(IInteract interact)
    {
        this.interact = interact;
    }
}

public readonly struct HideInteractMark
{
    public readonly IInteract interact;

    public HideInteractMark(IInteract interact)
    {
        this.interact = interact;
    }
}

public readonly struct ShowInteractObjectPanel
{
    public readonly Sprite sprite;
    public readonly string text;

    public ShowInteractObjectPanel(Sprite sprite, string text)
    {
        this.sprite = sprite;
        this.text = text;
    }
}

public readonly struct HideInteractObjectPanel {
    
}

public readonly struct ToggleInteractObjectPanelVisibility
{
    public readonly ShowInteractObjectPanel show;

    public ToggleInteractObjectPanelVisibility(ShowInteractObjectPanel show)
    {
        this.show = show;
    }
}

public class InteractObjectPanel : PanelView
{
    [Header("Interact")]
    [SerializeField] Canvas canvas;
    [SerializeField] GameObject interactMarkPrefab;

    List<CreatedMark> createdMarks = new List<CreatedMark>();

    readonly struct CreatedMark
    {
        public readonly InteractMark mark;
        public readonly IInteract interact;

        public CreatedMark(InteractMark mark, IInteract interact)
        {
            this.mark = mark;
            this.interact = interact;
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
        eventBus.Subscribe<ToggleInteractObjectPanelVisibility>(OnTogglePanelVisibility);

        eventBus.Subscribe<ShowInteractObjectPanel>(ShowInteractObjectPanel);
        eventBus.Subscribe<HideInteractObjectPanel>(data => HideInteractObjectPanel());

        eventBus.Subscribe<ShowInteractMark>(data => ShowInteractMark(data.interact));
        eventBus.Subscribe<HideInteractMark>(data => HideInteractMark(data.interact));
    }

    void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        panel = root.Q<VisualElement>("interact-object-container");
        image = panel.Q<VisualElement>("image");
        text = panel.Q<Label>("text");

        HideInteractObjectPanel();
        SetVisible(false);
    }

    /// <summary>
    /// ShowInteractObjectPanelを受け取り、パネルを表示する
    /// </summary>
    void ShowInteractObjectPanel(ShowInteractObjectPanel data)
    {
        image.style.backgroundImage = data.sprite != null ? new StyleBackground(data.sprite) : new StyleBackground();
        text.text = data.text;
    }

    /// <summary>
    /// HideInteractObjectPanelを受け取り、パネルを非表示にする
    /// </summary>
    void HideInteractObjectPanel()
    {
        image.style.backgroundImage = new StyleBackground();
        text.text = string.Empty;
    }

    void OnTogglePanelVisibility(ToggleInteractObjectPanelVisibility data)
    {
        isVisible = !isVisible;
        SetVisible(isVisible);

        if(isVisible)
            eventBus.Publish(data.show);
        else
            eventBus.Publish(new HideInteractObjectPanel());
    }

    /// <summary>
    /// InteractMarkを表示する
    /// </summary>
    void ShowInteractMark(IInteract interact)
    {
        // すでに表示されている場合は何もしない
        if(createdMarks.Exists(p => p.interact == interact)) return;

        GameObject obj = Instantiate(interactMarkPrefab, canvas.transform);
        InteractMark mark = obj.GetComponent<InteractMark>();
        mark.SetMark(interact.InteractMarkType, interact.GetMarkPoint());

        createdMarks.Add(new CreatedMark(mark, interact));
    }

    /// <summary>
    /// InteractMarkを非表示にする
    /// </summary>
    /// <param name="interact">非表示にするInteractMarkの対象</param>
    void HideInteractMark(IInteract interact)
    {
        // すでに非表示になっている場合は何もしない
        if(!createdMarks.Exists(p => p.interact == interact)) return;

        var createdMark = createdMarks.Find(p => p.interact == interact);
        if (createdMark.mark != null)
        {
            Destroy(createdMark.mark.gameObject);
            createdMarks.Remove(createdMark);
        }
    }
}