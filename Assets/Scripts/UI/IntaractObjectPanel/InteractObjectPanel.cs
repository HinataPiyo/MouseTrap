using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;


public enum InteractMarkType { None, Exclamation, Question }

public readonly struct ToggleMarkVisibility
{
    public readonly IInteract interact;
    public readonly bool isVisible;

    public ToggleMarkVisibility(IInteract interact, bool isVisible)
    {
        this.interact = interact;
        this.isVisible = isVisible;
    }
}


public readonly struct ToggleInteractObjectPanelVisibility
{
    public readonly Sprite sprite;
    public readonly string text;

    public ToggleInteractObjectPanelVisibility(Sprite sprite, string text)
    {
        this.sprite = sprite;
        this.text = text;
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

    [Inject]
    public void Construct(IEventBus eventBus)
    {
        eventBus.Subscribe<ToggleInteractObjectPanelVisibility>(OnTogglePanelVisibility);
        eventBus.Subscribe<ToggleMarkVisibility>(OnToggleMarkVisibility);
    }

    void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        panel = root.Q<VisualElement>("interact-object-container");
        image = panel.Q<VisualElement>("image");
        text = panel.Q<Label>("text");

        OnTogglePanelVisibility(new ToggleInteractObjectPanelVisibility(null, string.Empty));
        SetVisible(false);
    }

    /// <summary>
    /// ShowInteractObjectPanelを受け取り、パネルを表示する
    /// </summary>
    void ShowInteractObjectPanel(ToggleInteractObjectPanelVisibility data)
    {
        image.style.backgroundImage = data.sprite != null ? new StyleBackground(data.sprite) : new StyleBackground();
        text.text = data.text;
    }

    /// <summary>
    /// HideInteractObjectPanelを受け取り、パネルを非表示にする
    /// </summary>
    void HideInteractObjectPanel(ToggleInteractObjectPanelVisibility data)
    {
        image.style.backgroundImage = new StyleBackground();
        text.text = string.Empty;
    }

    void OnTogglePanelVisibility(ToggleInteractObjectPanelVisibility data)
    {
        if (!isVisible)
        {
            ShowInteractObjectPanel(data);
            SetVisible(true);
        }
        else
        {
            HideInteractObjectPanel(data);
            SetVisible(false);
        }
    }

    void OnToggleMarkVisibility(ToggleMarkVisibility data)
    {
        if (data.isVisible)
        {
            ShowInteractMark(data.interact);
        }
        else
        {
            HideInteractMark(data.interact);
        }
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