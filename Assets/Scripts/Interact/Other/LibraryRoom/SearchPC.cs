using System.Collections;
using UnityEngine;

/// <summary>
/// 破れたメモ紙を並べ替えパスワードを獲得しPCに入力する
/// 解除されたら散らばった本の配置を確認できる画面になる
/// これから散らばった本を配置しに行く（散らばった本を4冊回収していたら配置可能の状態にする）
/// </summary>
public class SearchPC : Interact
{
    protected override void Initialize() { }
    protected override void AcquireItem()
    {
        SearchPCUIPanel.Instance.Show();
        if(!IsEventCompleted) StartCoroutine(StartTalk());
    }

    protected override IEnumerator StartTalk()
    {
        PlayerInputController.Instance.ChangeState(StateType.Talk);
        yield return new WaitForSeconds(1f);
        yield return talk.Writing(GetTalkData(0));
        if (SearchPCUIPanel.Instance.IsVisible())
            PlayerInputController.Instance.ChangeState(StateType.Interact);
        IsEventCompleted = true;
    }
}