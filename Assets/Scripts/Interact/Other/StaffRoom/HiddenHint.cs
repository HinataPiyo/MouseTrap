namespace Intetract.StaffRoom
{
    using System.Collections;

    public class HiddenHint : Interact
    {
        protected override void Initialize() { }

        protected override IEnumerator StartTalk()
        {
            yield return talk.Writing(GetTalkData(0));
            yield break;
        }

        protected override void AcquireItem()
        {
            StartCoroutine(StartTalk());
            IsEventCompleted = true;
        }

    }
}