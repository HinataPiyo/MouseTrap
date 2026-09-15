namespace Intetract.StaffRoom.Boy
{
    using System.Collections;
    using UnityEngine;
    
    public class NextPosition : Interact
    {
        [Header("次の位置")]
        [SerializeField] Transform nextPosition;

        [Header("会話の内容")]
        [SerializeField] TalkData talkData_ReferenceRoom;
        [SerializeField] TalkData talkData_Sound;

        Transform target;
        Collider2D localCol;
        ITalk talk;

        void Start()
        {
            talk = ApiProvider.Get<ITalk>();
            localCol = GetComponent<Collider2D>();

            localCol.enabled = false;
        }

        public void SetNextPosition(GameObject target)
        {
            this.target = target.transform;
            localCol.enabled = true;
            target.transform.position = nextPosition.position;
        }

        public override bool TryInteract(out InteractData result)
        {
            if (base.TryInteract(out result))
            {
                StartCoroutine(TalkToReferenceRoomAndSound());
                return true;
            }
            return false;
        }

        IEnumerator TalkToReferenceRoomAndSound()
        {
            // 会話を開始する処理をここに記述
            talk.OnToTalk(talkData_ReferenceRoom, 0f);

            yield return new WaitUntil(() => !talk.IsWriting);
            ApiProvider.Get<IGameState>().ChangeState(StateType.Movie);     // 演出状態に遷移する

            // 会話が終了したら物音がする(演出)
            yield return new WaitForSeconds(1f); // 物音の演出のために1秒待機

            talk.OnToTalk(talkData_Sound, 0f);
            yield return new WaitUntil(() => !talk.IsWriting);

            // 資料室と図書室に行けるようになる
            IsEventCompleted = true;
            ApiProvider.Get<ISceneChange>().CanMoveRoom(Room.Name.Library, true);
            ApiProvider.Get<IGameState>().ChangeState(StateType.None);     // 演出が終了したら解除
        }
    }
}