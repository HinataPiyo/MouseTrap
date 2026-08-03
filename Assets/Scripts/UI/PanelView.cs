using UnityEngine;
using UnityEngine.UIElements;

public class PanelView : MonoBehaviour
{
    protected VisualElement panel;

    public void SetVisible(bool visible)
    {
        panel.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }
}