namespace Intetract.StaffRoom.Boy
{
    using System.Collections;
    using UnityEngine;
    
    public class NextPosition : Interact
    {
        [Header("次の位置")]
        [SerializeField] Transform nextPosition;

        Transform target;
        Collider2D localCol;

        // 初期化処理
        protected override void Initialize()
        {
            localCol = GetComponent<Collider2D>();

            localCol.enabled = false;
            IsEventCompleted = true;        //! テスト
        }

        // 会話開始
        protected override void AcquireItem()
        {
            StartCoroutine(StartTalk());
        }

        protected override IEnumerator StartTalk()
        {
            // 会話を開始する処理をここに記述
            yield return talk.Writing(GetTalkData(0));
            PlayerInputController.Instance.ChangeState(StateType.Movie);     // 演出状態に遷移する

            // 会話が終了したら物音がする(演出)
            yield return new WaitForSeconds(1f); // 物音の演出のために1秒待機

            yield return talk.Writing(GetTalkData(1));

            yield return FadeScreen.Instance.FadeInToFadeOut(HideCharacter);

            // キャラクターの操作を有効にする
            Player.CharacterControlHandler.Instance.SetCharacterControlState(Character.Name.Boy, true);

            // 資料室と図書室に行けるようになる
            IsEventCompleted = true;
            SceneChangeManager.Instance.CanMoveRoom(Room.Name.Library, true);
            PlayerInputController.Instance.ChangeState(StateType.None);     // 演出が終了したら解除
        }

        // 次の位置に移動させる
        public void SetNextPosition(GameObject target)
        {
            this.target = target.transform;
            localCol.enabled = true;
            target.transform.position = nextPosition.position;
        }

        // キャラを隠す
        void HideCharacter()
        {
            if (target != null)
                target.gameObject.SetActive(false);
        }
    }
}