using System.Collections;
using UnityEngine;


/// <summary>
/// Playble用の会話制御クラス
/// </summary>
public class TalkSignalController : MonoBehaviour
{
    [SerializeField] TalkDatabase talkDatabase;

    /// <summary>
    /// 会話を表示する
    /// </summary>
    public void ShowTalk(string tag)
    {
        StartCoroutine(RestartDirector(tag, true));
    }

    public void ShowTalk_HideBackground(string tag)
    {
        StartCoroutine(RestartDirector(tag, false));
    }

    IEnumerator RestartDirector(string tag, bool background)
    {
        TalkData talk = talkDatabase.GetTalkData(tag);
        Debug.Log($"TalkData: {tag} が選択されました");
        yield return TalkPanelController.Instance.Writing(talk, 0, background);
        DirectorManager.Instance.SetPlay();
    }
}