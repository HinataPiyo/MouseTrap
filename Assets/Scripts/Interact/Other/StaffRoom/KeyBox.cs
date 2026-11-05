namespace Interaction.StaffRoom
{
    using System.Collections;
    using UnityEngine;

    public class KeyBox : Interact
    {
        protected override void Initialize()
        {
        }

        protected override void AcquireItem()
        {
            MissionManager.Instance.CompleteMissionAndSetNext(
                MissionNames.FindKey,
                MissionNames.TalkToBoy);
        }

        protected override IEnumerator StartTalk()
        {
            yield return null;
        }
        
    }

}