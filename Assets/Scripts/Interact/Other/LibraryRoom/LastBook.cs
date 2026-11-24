using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class LastBook : Interact
{
    [Header("追いかけっこする演出")]
    [SerializeField] PlayableAsset chase;
    bool canGet;
    protected override void Initialize()
    {
        gameObject.SetActive(false);
    }

    protected override void AcquireItem()
    {
        StartCoroutine(StartTalk());        // 火のコについての会話を開始
    }

    protected override IEnumerator StartTalk()
    {
        yield return new WaitUntil(() => !InteractUIPanel.Instance.IsVisible());

        yield return talk.Writing(GetTalkData(0));
        PlayerInputController.Instance.ChangeState(StateType.Movie);

        yield return fadeScreen.FadeInToFadeOut();

        AudioManager.Instance.PlaySE("火のコ登場");
        yield return new WaitForSeconds(0.5f);
        yield return talk.Writing(GetTalkData(1));

        PlayerInputController.Instance.ChangeState(StateType.Movie);
        yield return fadeScreen.FadeInToFadeOut();

        AudioManager.Instance.PlaySE("教室のテーブル引きずる音");
        yield return new WaitForSeconds(2f);
        yield return talk.Writing(GetTalkData(2));

        IsEventCompleted = true;
        MissionManager.Instance.CompleteMissionAndSetNext(
            MissionNames.FindeBook,
            MissionNames.ReturnBooks);
    }

    protected override bool CanInteract()
    {
        if(DirectorManager.Instance.CurrentState != DirectorManager.State.None) return false;

        if(canGet)
        {
            return true;
        }
        else
        {
            //* 追いかけっこ開始
            DirectorManager.Instance.SetAsset(chase);
            // 別の部屋へ移動不可
            RoomChangeUI.Instance.BlockChange();
            return false;
        }

    }

    public void StopChase()
    {
        GetComponentInChildren<SpriteRenderer>().sortingOrder = 0;
        canGet = true;
    }

    /// <summary>
    /// 最後の本を表示する
    /// </summary>
    public void ActivateLastBook()
    {
        gameObject.SetActive(true);
    }
}