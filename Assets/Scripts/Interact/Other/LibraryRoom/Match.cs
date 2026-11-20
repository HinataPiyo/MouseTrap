using System.Collections;
using UnityEngine;

public class Match : Interact
{
    protected override void Initialize()
    {
    }

    protected override void AcquireItem()
    {
        IsEventCompleted = true;
    }

    protected override IEnumerator StartTalk()
    {
        yield return null;
    }
}