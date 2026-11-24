using System.Collections;
using UnityEngine;

public class InteractKey : Interact
{
    protected override void Initialize()
    {
        gameObject.SetActive(false);
    }

    protected override void AcquireItem()
    {
    }

    protected override IEnumerator StartTalk()
    {
        yield return null;
    }

    public void KeyActivate()
    {
        gameObject.SetActive(true);
    }
}