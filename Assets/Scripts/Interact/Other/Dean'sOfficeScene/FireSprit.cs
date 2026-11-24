namespace DeansOffice
{
    using System.Collections;
    using UnityEngine;
    
    public class FireSprit : Interact
    {
        [SerializeField] InteractData candleLampData;
        protected override void Initialize()
        {
        }

        protected override void AcquireItem()
        {
            Inventory.Instance.RemoveItem(candleLampData);
            Inventory.Instance.AddItem(interactData);
            InteractUIPanel.Instance.ShowInteractPanel(interactData);
        }

        protected override bool CanInteract()
        {
            // しゅしょくランプを所持しているか否か
            return Inventory.Instance.HasItem(candleLampData);
        }

        protected override IEnumerator StartTalk()
        {
            yield return null;
        }
    }
}