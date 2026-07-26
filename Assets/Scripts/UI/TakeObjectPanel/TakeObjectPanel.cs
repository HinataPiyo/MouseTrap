using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public readonly struct ShowTakeObjectPanel
{
    public readonly Sprite sprite;
    public readonly string text;

    public ShowTakeObjectPanel(Sprite sprite, string text)
    {
        this.sprite = sprite;
        this.text = text;
    }
}

public readonly struct HideTakeObjectPanel {
    
}

public readonly struct ToggleTakeObjectPanelVisibility
{
    public readonly ShowTakeObjectPanel show;

    public ToggleTakeObjectPanelVisibility(ShowTakeObjectPanel show)
    {
        this.show = show;
    }
}

public class TakeObjectPanel : PanelView
{
    UIDocument uiDocument;

    VisualElement image;
    Label text;

    bool isVisible = false;

    IEventBus eventBus;

    [Inject]
    public void Construct(IEventBus eventBus)
    {
        this.eventBus = eventBus;
        eventBus.Subscribe<ToggleTakeObjectPanelVisibility>(OnTogglePanelVisibility);

        eventBus.Subscribe<ShowTakeObjectPanel>(ShowTakeObjectPanel);
        eventBus.Subscribe<HideTakeObjectPanel>(data => HideTakeObjectPanel());
    }

    void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        panel = root.Q<VisualElement>("get-item-container");
        image = panel.Q<VisualElement>("image");
        text = panel.Q<Label>("text");

        HideTakeObjectPanel();
        SetVisible(false);
    }

    /// <summary>
    /// ShowTakeObjectPanelを受け取り、パネルを表示する
    /// </summary>
    void ShowTakeObjectPanel(ShowTakeObjectPanel data)
    {
        image.style.backgroundImage = data.sprite != null ? new StyleBackground(data.sprite) : new StyleBackground();
        text.text = data.text;
    }

    /// <summary>
    /// HideTakeObjectPanelを受け取り、パネルを非表示にする
    /// </summary>
    void HideTakeObjectPanel()
    {
        image.style.backgroundImage = new StyleBackground();
        text.text = string.Empty;
    }

    void OnTogglePanelVisibility(ToggleTakeObjectPanelVisibility data)
    {
        isVisible = !isVisible;
        SetVisible(isVisible);

        if(isVisible)
            eventBus.Publish(data.show);
        else
            eventBus.Publish(new HideTakeObjectPanel());
    }
}