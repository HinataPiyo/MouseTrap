using UnityEngine;
using UnityEngine.UIElements;

public class EndingSceneManager : PanelView<EndingSceneManager>
{
    protected override void OnSingletonAwake()
    {
        panel = GetComponent<UIDocument>().rootVisualElement;
        Button gotoTitle = panel.Q<Button>();
        gotoTitle.clicked += () =>
        {
            GoToTitle();
        };
    }

    protected override void OnSingletonDestroy()
    {
    }

    void GoToTitle()
    {
        SceneChangeManager.Instance.SceneChange(Room.Name.Title);
    }

}