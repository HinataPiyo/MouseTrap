using UnityEngine;

public class RoomChangePoint : BlockerArea
{
    RoomChangeUI roomChangeUI;

    protected override void Start()
    {
        base.Start();
        roomChangeUI = RoomChangeUI.Instance;
    }

    protected override void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            if(!TryBlockerAreaDisabled())
            {
                StartTalk(0f);
                return;
            }

            roomChangeUI.ShowPanel();
        }
    }
}