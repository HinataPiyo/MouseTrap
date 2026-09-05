using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum InteractMarkType { None, Exclamation, Question }

public interface IInteractObjectPanel
{
    void TogglePanelVisibility(Sprite sprite, string text);
    void ToggleMarkVisibility(IInteract interact, bool isVisible);
}

public class InteractObjectPanel : PanelView, IInteractObjectPanel
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

    IGameState gameState;

    UIDocument uiDocument;

    VisualElement image;
    Label text;

    void Awake()
    {
        ApiProvider.Register<IInteractObjectPanel>(this);

        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        panel = root.Q<VisualElement>("interact-object-container");
        image = panel.Q<VisualElement>("image");
        text = panel.Q<Label>("text");
    }

    void Start()
    {
        gameState = ApiProvider.Get<IGameState>();

        TogglePanelVisibility(null, string.Empty);
        SetVisible(false);
    }

    /// <summary>
    /// ShowInteractObjectPanelを受け取り、パネルを表示する
    /// </summary>
    void ShowInteractObjectPanel(Sprite sprite, string _text)
    {
        image.style.backgroundImage = sprite != null ? new StyleBackground(sprite) : new StyleBackground();
        text.text = _text;
    }

    /// <summary>
    /// HideInteractObjectPanelを受け取り、パネルを非表示にする
    /// </summary>
    void HideInteractObjectPanel()
    {
        image.style.backgroundImage = new StyleBackground();
        text.text = string.Empty;
    }

    public void TogglePanelVisibility(Sprite sprite, string text)
    {
        if (sprite == null && string.IsNullOrEmpty(text))
        {
            HideInteractObjectPanel();
            SetVisible(false);
            gameState.ChangeState(StateType.None);
            return;
        }

        ShowInteractObjectPanel(sprite, text);
        SetVisible(true);
        gameState.ChangeState(StateType.Interact);
    }

    public void ToggleMarkVisibility(IInteract interact, bool isVisible)
    {
        if(interact == null)
        {
            HideInteractMark(interact);
            return;
        }

        if (isVisible)
        {
            ShowInteractMark(interact);
        }
        else
        {
            HideInteractMark(interact);
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