using UnityEngine;

/// <summary>
/// 部屋移動ポイント
/// </summary>
public class RoomChangePoint : BlockerArea
{
    protected override bool ShouldRestoreDeactivatedState => false;

    RoomChangeUI roomChangeUI;

    protected override void Initialize()
    {
        roomChangeUI = RoomChangeUI.Instance;
    }

    protected override void OnCollisionEnter2D(Collision2D col)
    {
        if(DirectorManager.Instance.CurrentState != DirectorManager.State.None) return;
        
        if(col.gameObject.CompareTag("Player"))
        {
            if(!TryCompleteRequired())
            {
                StartCoroutine(StartTalk());
                return;
            }

            roomChangeUI.ShowPanel();
        }
    }
}