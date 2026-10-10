using UnityEngine;
using UnityEngine.UIElements;

public class InteractUIPanel : PanelView
{
    public static InteractUIPanel Instance { get; private set; }
    VisualElement object_view;
    void Awake()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        panel = uiDocument.rootVisualElement.Q<VisualElement>("interact-object-container");
        object_view = panel.Q<VisualElement>("image");
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetInstance()
    {
        Instance = null;
    }

    void OnEnable()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ShowInteractPanel(InteractData interactData)
    {
        SetVisible(interactData);
        object_view.style.backgroundImage = new StyleBackground(interactData.Sprite);
    }

    public void HideInteractPanel()
    {
        SetVisible(false);
    }
}