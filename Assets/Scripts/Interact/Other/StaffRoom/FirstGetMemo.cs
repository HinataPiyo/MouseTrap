using System.Collections;
using UnityEngine;

public class FirstGetMemo : Interact
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(base.TryInteract(out InteractData result))
            {
                StartCoroutine(StartTalk());
            }
        }
    }
    protected override void Initialize()
    {
        if (string.IsNullOrEmpty(GameSession.CurrentMissionTitle))
            MissionManager.Instance.SetMission(MissionNames.CheckLocker);
    }

    protected override void AcquireItem()
    {
    }

    protected override IEnumerator StartTalk()
    {
        yield return new WaitUntil(() => !InteractUIPanel.Instance.IsVisible());
        yield return talk.Writing(GetTalkData(0));

        IsEventCompleted = true;
        gameObject.SetActive(false);
        DirectorManager.Instance.SetPlay();
    }
}