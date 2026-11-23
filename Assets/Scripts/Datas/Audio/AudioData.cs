using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "MouseTrap/AudioData")]
public class AudioData : ScriptableObject
{
    [SerializeField] Clip[] clips;

    public AudioClip GetClip(string tag)
    {
        foreach(var c in clips)
            if(c.tag == tag) return c.clip;
        return null;
    }
    
    [System.Serializable]
    public class Clip
    {
        public string tag;
        public AudioClip clip;
    }
}