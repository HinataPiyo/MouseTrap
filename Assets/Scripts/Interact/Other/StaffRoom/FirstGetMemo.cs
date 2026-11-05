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
        Debug.Log("<color=green>Start talk</color>");
        yield return new WaitUntil(() => !InteractUIPanel.Instance.IsVisible());
        Debug.Log("<color=green>Panel hidden</color>");
        yield return talk.Writing(GetTalkData(0));

        IsEventCompleted = true;
        gameObject.SetActive(false);
    }
}