using UnityEngine;
using UnityEngine.UIElements;

public class GameUIHandler : PanelView<GameUIHandler>
{
    protected override void OnSingletonAwake()
    {
        panel = GetComponent<UIDocument>().rootVisualElement;
    }

    protected override void OnSingletonDestroy()
    {
    }

    public void Show()
    {
        panel.style.display = DisplayStyle.Flex;
    }
    
    public void Hide()
    {
        panel.style.display = DisplayStyle.None;
    }
}