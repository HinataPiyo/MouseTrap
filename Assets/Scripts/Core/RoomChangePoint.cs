using UnityEngine;

public class RoomChangePoint : BlockerArea
{
    IRoomChangeUI roomChangeUI;

    protected override void Start()
    {
        base.Start();
        roomChangeUI = ApiProvider.Get<IRoomChangeUI>();
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