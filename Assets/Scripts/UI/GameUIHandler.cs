using UnityEngine;
using UnityEngine.UIElements;

public class GameUIHandler : PanelView<GameUIHandler>
{
    bool isTalkActive;
    bool isDirectorActive;
    bool isGimmickUIPanelActive;

    protected override void OnSingletonAwake()
    {
        panel = GetComponent<UIDocument>().rootVisualElement;
        UpdateVisibility();
    }

    protected override void OnSingletonDestroy()
    {
    }

    public void SetGimmickActive(bool active)
    {
        isGimmickUIPanelActive = active;
        UpdateVisibility();
    }

    public void SetTalkActive(bool active)
    {
        isTalkActive = active;
        UpdateVisibility();
    }

    public void SetDirectorActive(bool active)
    {
        isDirectorActive = active;
        UpdateVisibility();
    }

    void UpdateVisibility() => SetVisible(!isTalkActive && !isDirectorActive && !isGimmickUIPanelActive);
}