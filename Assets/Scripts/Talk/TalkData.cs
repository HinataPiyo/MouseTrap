using UnityEngine;

[CreateAssetMenu(fileName = "TalkData", menuName = "MouseTrap/TalkData")]
public class TalkData : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public Sprite character;
        public Character.Name characterName;
        public string body;
    }

    public Entry[] entries;
}