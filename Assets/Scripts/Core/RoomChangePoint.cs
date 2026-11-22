using UnityEngine;

/// <summary>
/// 部屋移動ポイント
/// </summary>
public class RoomChangePoint : BlockerArea
{
    RoomChangeUI roomChangeUI;
    Collider2D col;

    void Start()
    {
        roomChangeUI = RoomChangeUI.Instance;
        col = GetComponent<Collider2D>();
        col.enabled = false;
    }

    void Update()
    {
        if(!col.enabled && TryCompleteRequired())
            col.enabled = true;
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