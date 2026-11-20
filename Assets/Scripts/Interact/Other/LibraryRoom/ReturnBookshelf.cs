using System.Collections;
using UnityEngine;

public class ReturnBookshelf : Interact
{
    [SerializeField] LastBook lastBook;
    ReturnBookshelfUIPanel panel;

    protected override void Initialize()
    {
        panel = ReturnBookshelfUIPanel.Instance;
        panel.OnBooksReturned += CompleteEvent;
    }

    protected override void OnDestroy()
    {
        if (panel != null)
            panel.OnBooksReturned -= CompleteEvent;
        base.OnDestroy();
    }

    void CompleteEvent()
    {
        IsEventCompleted = true;
    }

    // アイテム取得処理
    protected override void AcquireItem()
    {
        bool hasFour = ReturnBookshelfUIPanel.Instance.Show();
        if(hasFour && !lastBook.IsEventCompleted) StartCoroutine(StartTalk());
    }

    // 会話開始処理
    protected override IEnumerator StartTalk()
    {
        ReturnBookshelfUIPanel panel = ReturnBookshelfUIPanel.Instance;
        int sessionId = panel.OpenSessionId;
        panel.IsInteractable(false);
        yield return new WaitForSeconds(1f);
        if (!panel.IsSessionOpen(sessionId)) yield break;

        // 4冊所持していた時の会話（5冊目が足りない）
        yield return talk.Writing(GetTalkData(0));
        lastBook.ActivateLastBook();      // 5冊目を表示する
        if (!panel.IsSessionOpen(sessionId)) yield break;

        panel.IsInteractable(true);
        if (PlayerInputController.Instance.CurrentState == StateType.None)
            PlayerInputController.Instance.ChangeState(StateType.Interact);
    }
}