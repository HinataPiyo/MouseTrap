namespace Interaction.StaffRoom
{
    using System.Collections;
    using UnityEngine;

    public class Picture : Interact
    {
        [SerializeField] GetKeyAndGetPicture getKeyAndGetPicture;
        protected override void Initialize()
        {
        }
        protected override void AcquireItem()
        {
            MissionManager.Instance.CompleteMissionAndSetNext(
                MissionNames.ExploreStaffroom,
                MissionNames.ChangeRoom);

            if(!IsEventCompleted)
                StartCoroutine(StartTalk());
        }
        protected override IEnumerator StartTalk()
        {
            yield return new WaitUntil(() => !InteractUIPanel.Instance.IsVisible());
            yield return talk.Writing(GetTalkData(0));
            IsEventCompleted = true;
            getKeyAndGetPicture.GetPicture();
        }
    }
}