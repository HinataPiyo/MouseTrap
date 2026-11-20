using System.Collections;
using UnityEngine;

public class ReturnBookshelf : Interact
{
    protected override void Initialize()
    {
    }

    // アイテム取得処理
    protected override void AcquireItem()
    {
        ReturnBookshelfUIPanel.Instance.Show();
    }

    // 会話開始処理
    protected override IEnumerator StartTalk()
    {
        yield return null;
    }
}