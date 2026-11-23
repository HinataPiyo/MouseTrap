using System.Collections;
using Intetract.StaffRoom.Boy;
using Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class GetKeyAndGetPicture : Interact
{
    [Header("カギと写真を取得したのち再生（図書室から音が聞こえる）")]
    [SerializeField] PlayableAsset asset;
    [SerializeField] NextPosition nextPos;
    bool gotKey;
    bool gotPicture;
    protected override void Initialize()
    {
        gotKey = MissionManager.Instance.IsMissionCompleted(MissionNames.FindKey);
        gotPicture = MissionManager.Instance.IsMissionCompleted(MissionNames.ExploreStaffroom);
    }

    protected override void AcquireItem()
    {
    }

    protected override IEnumerator StartTalk()
    {
        return null;
    }

    public void GetKey()
    {
        gotKey = true;
        CheckAllGet();
    }
    public void GetPicture()
    {
        gotPicture = true;
        CheckAllGet();
    }

    /// <summary>
    /// カギと写真を両方取得したか確認する
    /// </summary>
    public void CheckAllGet()
    {
        if (gotKey && gotPicture && nextPos.IsEventCompleted)
        {
            if(!IsEventCompleted)
            {
                // 再生開始
                DirectorManager.Instance.SetAsset(asset);
                nextPos.HideCharacter();
                IsEventCompleted = true;
            }
        }
    }

    /// <summary>
    /// 図書室に行けるようにする
    /// Signalで呼ばれる
    /// </summary>
    public void CanGoToLibraryScene()
    {
        CharacterControlHandler.Instance.SetCharacterControlState(Character.Name.Boy, true);
        SceneChangeManager.Instance.CanMoveRoom(Room.Name.Library, true);
    }
}