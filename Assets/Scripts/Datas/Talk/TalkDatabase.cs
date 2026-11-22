using UnityEngine;

[CreateAssetMenu(fileName = "TalkDatabase", menuName = "MouseTrap/Database/TalkDatabase")]
public class TalkDatabase : ScriptableObject
{
    [SerializeField] TalkData[] talks;

    /// <summary>
    /// 会話データを取得する
    /// </summary>
    public TalkData GetTalkData(string tag)
    {
        foreach (var t in talks)
            if (t.Tag == tag) return t;
        return null;
    }
}