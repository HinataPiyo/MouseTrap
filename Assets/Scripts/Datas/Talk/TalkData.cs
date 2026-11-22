using UnityEngine;

[CreateAssetMenu(fileName = "TalkData", menuName = "MouseTrap/TalkData")]
public class TalkData : ScriptableObject
{
    [SerializeField] string tag;
    [System.Serializable]
    public class Entry
    {
        public Sprite character;
        public Character.Name characterName;
        public string body;
        [Range(0f, 1f)] public float delay = 0f;
    }

    public Entry[] entries;
    public string Tag => tag;
}