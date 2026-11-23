using System.Collections;
using UnityEngine;

public class Match : Interact
{
    
    protected override void Initialize()
    {
        gameObject.SetActive(false);
    }

    protected override void AcquireItem()
    {
        IsEventCompleted = true;
    }

    protected override bool CanInteract()
    {
        return false;       // シグナルでのみ取得可能
    }

    protected override IEnumerator StartTalk()
    {
        yield return null;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void GetMatch()
    {
        InteractUIPanel.Instance.ShowInteractPanel(interactData);
        Inventory.Instance.AddItem(interactData);
        gameObject.SetActive(false);
    }
}