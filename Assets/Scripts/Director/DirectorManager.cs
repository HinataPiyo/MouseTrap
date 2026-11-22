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

    public void ResetState()
    {
        GameUIHandler.Instance.Show();
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
        GameUIHandler.Instance.Hide();
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