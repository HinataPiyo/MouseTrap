using UnityEngine;

/// <summary>
/// 部屋移動ポイント
/// </summary>
public class RoomChangePoint : BlockerArea
{
    RoomChangeUI roomChangeUI;

    void Start()
    {
        roomChangeUI = RoomChangeUI.Instance;
    }

    protected override void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            if(!TryBlockerAreaDisabled())
            {
                StartCoroutine(StartTalk());
                return;
            }

            roomChangeUI.ShowPanel();
        }
    }
}