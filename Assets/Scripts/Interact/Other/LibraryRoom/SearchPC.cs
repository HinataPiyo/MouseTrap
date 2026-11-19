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
    }

    protected override IEnumerator StartTalk() { yield return null; }
}