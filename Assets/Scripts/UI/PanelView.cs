using UnityEngine;
using UnityEngine.UIElements;

public abstract class PanelView<TSelf> : SceneSingleton<TSelf> where TSelf : PanelView<TSelf>
{
    protected VisualElement panel;
    protected bool isVisible = false;

    public void SetVisible(bool visible)
    {
        panel.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        isVisible = visible;
    }
}