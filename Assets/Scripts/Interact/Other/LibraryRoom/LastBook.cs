using System.Collections;
using UnityEngine;

public class LastBook : Interact
{
    protected override void Initialize()
    {
    }

    protected override void AcquireItem()
    {
        IsEventCompleted = true;
        gameObject.SetActive(false);
    }

    protected override IEnumerator StartTalk()
    {
        yield return null;
    }

    /// <summary>
    /// 最後の本を表示する
    /// </summary>
    public void ActivateLastBook()
    {
        gameObject.SetActive(true);
    }
}