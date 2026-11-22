using UnityEngine;
using UnityEngine.Playables;

public class DirectorManager : SceneSingleton<DirectorManager>
{
    PlayableDirector director;
    string stateKey;
    PlayableAsset activeAsset;
    public enum State { None, Play, Pause }
    public State CurrentState { get; private set; } = State.None;
    protected override void OnSingletonAwake()
    {
        director = GetComponentInChildren<PlayableDirector>();
        stateKey = InteractStateStore.CreateKey(gameObject);
        director.stopped += OnDirectorStopped;
    }

    protected override void OnSingletonDestroy()
    {
        director.stopped -= OnDirectorStopped;
    }

    void Start()
    {
        SetPlay();
    }

    void Update()
    {
        if (CurrentState == State.Play && director.time >= director.duration)
            ResetState();
    }

    void OnDirectorStopped(PlayableDirector stoppedDirector)
    {
        if (GameUIHandler.Instance != null)
            ResetState();
        else
            CurrentState = State.None;
    }

    public void SetState(State state)
    {
        CurrentState = state;
        GameUIHandler.Instance.SetDirectorActive(state != State.None);
    }

    public void ResetState()
    {
        SetState(State.None);
    }

    public void SetPlay()
    {
        if(director.playableAsset == null) return;
        if (CurrentState == State.None || activeAsset != director.playableAsset)
        {
            if (!GameSession.PlayedDirectors.Add((stateKey, director.playableAsset))) return;
            activeAsset = director.playableAsset;
        }
        SetState(State.Play);
        director.Play();
    }

    public void SetPause()
    {
        SetState(State.Pause);
        director.Pause();
    }

    public void SetAsset(PlayableAsset asset)
    {
        director.playableAsset = asset;
        SetPlay();
    }

}