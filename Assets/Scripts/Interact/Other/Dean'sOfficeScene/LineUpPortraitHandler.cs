using System.Collections;
using UnityEngine;

public class LineUpPortraitHandler : Interact
{
    protected override void Initialize()
    {
    }

    protected override void AcquireItem()
    {
        LineUpPortraitUIPanel.Instance.Show();
    }

    protected override IEnumerator StartTalk()
    {
        yield return null;
    }
}