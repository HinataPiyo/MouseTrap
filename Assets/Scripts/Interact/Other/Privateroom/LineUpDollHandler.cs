using System.Collections;
using UnityEngine;

public class LineUpDollHandler : Interact
{
    [Header("補聴器")]
    [SerializeField] InteractData hearingAid;
    LineUpDollsUIPanel panel;
    protected override void Initialize()
    {
        panel = LineUpDollsUIPanel.Instance;
    }

    protected override void AcquireItem()
    {
        panel.Show();
    }

    /// <summary>
    /// 並び替え終了判定を行っているところから呼び出す
    /// </summary>
    public void CompletedEvent()
    {
        StartCoroutine(StartTalk());
    }

    protected override IEnumerator StartTalk()
    {
        yield return fadeScreen.FadeInCoroutine();
        panel.Hide();
        yield return fadeScreen.FadeOutCoroutine();

        yield return talk.Writing(GetTalkData(0));      // 人形並び替え終了後に会話start

        // ここで特殊な補聴器をゲットする
        // InteractしたときのPanelを表示する
        InteractUIPanel.Instance.ShowInteractPanel(hearingAid);
        Inventory.Instance.AddItem(hearingAid);

        // 取得画面が閉じるまで待機
        yield return new WaitUntil(() => !InteractUIPanel.Instance.IsVisible());
        yield return talk.Writing(GetTalkData(1));      // 次の部屋へ移動する会話

        SceneChangeManager.Instance.CanMoveRoom(Room.Name.DeansOfficeScene, true);
        yield return fadeScreen.FadeInToFadeOut();
    }
}