using UnityEngine;

public class AudioManager : SceneSingleton<AudioManager>
{
    [SerializeField] AudioSource seSource;
    [SerializeField] AudioData seData;

    protected override void OnSingletonAwake()
    {
    }

    protected override void OnSingletonDestroy()
    {
    }

    public void PlaySE(string tag)
    {
        AudioClip clip = seData.GetClip(tag);
        if(clip == null)
        {
            Debug.LogWarning("Clip not found");
            return;
        }

        seSource.PlayOneShot(clip);
    }

}