using System.Collections;
using UnityEngine;

public class SafePassword : Interact
{
    [SerializeField] InteractData key;
    protected override void Initialize()
    {
    }

    protected override void AcquireItem()
    {
        SafePasswordUIPanel.Instance.Show();
    }

    protected override bool CanInteract()
    {
        return !SafePasswordUIPanel.Instance.IsSuccess && !IsEventCompleted;
    }

    public void PasswordSuccess()
    {
        StartCoroutine(StartTalk());
        IsEventCompleted = true;
    }

    protected override IEnumerator StartTalk()
    {
        InteractUIPanel.Instance.ShowInteractPanel(key);
        Inventory.Instance.AddItem(key);

        yield return null;
    }
}