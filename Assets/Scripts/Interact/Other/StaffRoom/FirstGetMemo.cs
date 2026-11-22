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
        MissionManager.Instance.SetMission(MissionNames.FirstGetMemo);
    }

    protected override void AcquireItem()
    {
        MissionManager.Instance.CompleteMissionAndSetNext(
            MissionNames.FirstGetMemo,
            MissionNames.CheckLocker);
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