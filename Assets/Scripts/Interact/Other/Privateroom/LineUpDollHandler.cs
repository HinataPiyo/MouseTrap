using System.Collections;
using UnityEngine;

public class LineUpDollHandler : Interact
{
    protected override void Initialize()
    {
    }

    protected override void AcquireItem()
    {
        LineUpDollsUIPanel.Instance.Show();
    }

    protected override IEnumerator StartTalk()
    {
        yield return null;
    }
}