using System.Collections;
using UnityEngine;

public class LineUpPortraitHandler : Interact
{
    [SerializeField] InteractKey keyC;
    protected override void Initialize()
    {
    }

    protected override void AcquireItem()
    {
        LineUpPortraitUIPanel.Instance.Show();
    }

    protected override bool CanInteract()
    {
        return !IsEventCompleted;
    }

    protected override IEnumerator StartTalk()
    {
        yield return null;
    }

    public void CompletedLineUp()
    {
        keyC.KeyActivate();
        IsEventCompleted = true;
    }
}