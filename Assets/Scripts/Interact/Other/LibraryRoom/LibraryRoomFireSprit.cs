using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 火の粉とのイベント
/// </summary>
public class LibraryRoomFireSprit : Interact
{
    [SerializeField] Match match;
    protected override void Initialize()
    {
        
    }

    protected override void AcquireItem()
    {
        StartCoroutine(StartTalk());
    }

    protected override IEnumerator StartTalk()
    {
        // まだmatchを所持していなかった場合
        if(!match.IsEventCompleted)
        {
            yield return talk.Writing(GetTalkData(1));
            yield break;
        }

        yield return talk.Writing(GetTalkData(0));

        SceneChangeManager.Instance.CanMoveRoom(Room.Name.Classroom, true);     // 教室に移動可能にする
        IsEventCompleted = true;
        yield return null;
    }
}