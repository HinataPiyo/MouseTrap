using UnityEngine;
using UnityEngine.UIElements;

public class TitleSceneManager : PanelView<TitleSceneManager>
{
    Button startButton;

    protected override void OnSingletonAwake()
    {
    }

    protected override void OnSingletonDestroy()
    {
    }

    void Start()
    {
        panel = GetComponent<UIDocument>().rootVisualElement;
        startButton = panel.Q<Button>("start-button");
        startButton.clicked += OnStartClicked;
    }

    void OnStartClicked()
    {
        GameSession.Reset();
        SceneChangeManager.Instance.SceneChange(Room.Name.Staffroom);
    }
}