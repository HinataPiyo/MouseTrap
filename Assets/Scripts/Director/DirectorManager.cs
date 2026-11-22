using UnityEngine;
using UnityEngine.Playables;

public class DirectorManager : SceneSingleton<DirectorManager>
{
    PlayableDirector director;
    public enum State { None, Play, Pause }
    public State CurrentState { get; private set; } = State.None;
    protected override void OnSingletonAwake()
    {
        director = GetComponentInChildren<PlayableDirector>();
    }

    protected override void OnSingletonDestroy()
    {
    }

    void Start()
    {
        SetPlay();
    }

    public void SetState(State state)
    {
        CurrentState = state;
    }

    public void ResetState() => SetState(State.None);

    public void SetPlay()
    {
        SetState(State.Play);
        director.Play();
    }

    public void SetPause()
    {
        SetState(State.Pause);
        director.Pause();
    }
}