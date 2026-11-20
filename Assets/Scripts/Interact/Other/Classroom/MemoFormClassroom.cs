using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// 教室に存在するメモ
/// </summary>
public class MemoFormClassroom : Interact
{
    [SerializeField] CinemachineCamera cam;
    [SerializeField] GameObject player;
    protected override void Initialize()
    {
        gameObject.SetActive(false);
    }

    protected override void AcquireItem()
    {
        if(!IsEventCompleted)
            IsEventCompleted = true;
    }

    protected override bool CanInteract()
    {
        return ReturnTableUIPanel.Instance.IsSuccess;
    }

    protected override IEnumerator StartTalk()
    {
        yield return null;
    }

    public void ActivateMemo(Vector2 nextPosition)
    {
        gameObject.SetActive(true);
        player.SetActive(true);
        player.transform.position = nextPosition;
        cam.Lens.OrthographicSize = Room.DefaultCameraSize;
        cam.Target.TrackingTarget = player.transform;
        PlayerInputController.Instance.ChangeState(StateType.None);

        //TODO ここでツインテールの女の子を探す会話を行う

        SceneChangeManager.Instance.CanMoveRoom(Room.Name.Privateroom, true);
    }
}