namespace Interaction.StaffRoom
{
    using System.Collections;
    using UnityEngine;

    public class KeyBox : Interact
    {
        protected override bool ShouldRestoreDeactivatedState => false;

        [SerializeField] GetKeyAndGetPicture getKeyAndGetPicture;
        protected override void Initialize()
        {
        }

        protected override void AcquireItem()
        {
            MissionManager.Instance.CompleteMissionAndSetNext(
                MissionNames.FindKey,
                MissionNames.ExploreStaffroom);

            if(!IsEventCompleted)
                StartCoroutine(StartTalk());
        }

        protected override IEnumerator StartTalk()
        {
            yield return new WaitUntil(() => !InteractUIPanel.Instance.IsVisible());
            yield return talk.Writing(GetTalkData(0));
            IsEventCompleted = true;
            getKeyAndGetPicture.GetKey();
        }
        
    }

}