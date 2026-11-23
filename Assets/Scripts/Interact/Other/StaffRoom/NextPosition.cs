namespace Intetract.StaffRoom.Boy
{
    using System.Collections;
    using UnityEngine;
    
    public class NextPosition : Interact
    {
        [Header("次の位置")]
        [SerializeField] Transform nextPosition;
        [SerializeField] GetKeyAndGetPicture getKeyAndGetPicture;

        Transform target;
        Collider2D localCol;

        // 初期化処理
        protected override void Initialize()
        {
            localCol = GetComponent<Collider2D>();

            localCol.enabled = false;
        }

        // 会話開始
        protected override void AcquireItem()
        {
            MissionManager.Instance.CompleteMissionAndSetNext(
                MissionNames.TalkToBoy,
                MissionNames.FindKey);
            StartCoroutine(StartTalk());
        }

        protected override IEnumerator StartTalk()
        {
            // 会話を開始する処理をここに記述
            yield return talk.Writing(GetTalkData(0));

            IsEventCompleted = true;
            getKeyAndGetPicture.CheckAllGet();
        }

        // 次の位置に移動させる
        public void SetNextPosition(GameObject target)
        {
            this.target = target.transform;
            localCol.enabled = true;
            target.transform.position = nextPosition.position;
        }

        // キャラを隠す
        public void HideCharacter()
        {
            if (target != null)
            {
                target.gameObject.SetActive(false);
                nextPosition.gameObject.SetActive(false);
            }
        }
    }
}