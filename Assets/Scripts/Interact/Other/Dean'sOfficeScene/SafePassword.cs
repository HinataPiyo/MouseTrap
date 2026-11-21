using System.Collections;
using UnityEngine;

public class SafePassword : Interact
{
    [SerializeField] InteractMemoData memo;
    protected override void Initialize()
    {
    }

    protected override void AcquireItem()
    {
        SafePasswordUIPanel.Instance.Show();
    }

    protected override bool CanInteract()
    {
        return !SafePasswordUIPanel.Instance.IsSuccess;
    }

    public void PasswordSuccess()
    {
        StartCoroutine(StartTalk());
    }

    protected override IEnumerator StartTalk()
    {
        InteractUIPanel.Instance.ShowInteractPanel(memo);
        Inventory.Instance.AddItem(memo);

        yield return null;
    }
}