using UnityEngine;
using UnityEngine.UIElements;

public class PanelView : MonoBehaviour
{
    protected VisualElement panel;
    protected bool isVisible = false;

    public void SetVisible(bool visible)
    {
        panel.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        isVisible = visible;
    }
}