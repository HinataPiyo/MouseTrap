namespace Intetract.StaffRoom.Boy
{
    using System.Collections;
    using UnityEngine;
    
    public class LockerIntetract : Interact
    {
        [Header("表示するキャラクター")]
        [SerializeField] GameObject subCharacter;

        [Header("会話終了後待機する次の位置")]
        [SerializeField] NextPosition nextPosition;

        protected override void Initialize()
        {
            HideSubCharacter();
            fadeScreen.OnFadeInComplete += SetNextPosition;        // フェードイン完了時にサブキャラクターの位置を次の位置に設定する
        }

        void OnDestroy()
        {
            fadeScreen.OnFadeInComplete -= SetNextPosition;
        }

        /// <summary>
        /// アイテム取得処理
        /// </summary>
        protected override void AcquireItem()
        {
            ShowSubCharacter();
            IsEventCompleted = true;
        }

        /// <summary>
        /// サブキャラクターを表示させる処理
        /// </summary>
        void ShowSubCharacter()
        {
            StartCoroutine(StartTalk());
        }

        protected override IEnumerator StartTalk()
        {
            subCharacter.SetActive(true);       // 少年を表示

            yield return talk.Writing(GetTalkData(0), 1f);

            //* プレイヤーの状態を変更する
            PlayerInputController.Instance.ChangeState(StateType.Movie);     // 演出状態に遷移する

            yield return fadeScreen.FadeInToFadeOut();              // FadeInToOutが終わるのを待つ
        }

        /// <summary>
        /// 少年を非表示にしフェード終了後の登録済みの処理を解除する
        /// </summary>
        void HideSubCharacter()
        {
            subCharacter.SetActive(false);
            fadeScreen.OnFadeInComplete -= SetNextPosition;
        }

        /// <summary>
        /// サブキャラの位置を次の位置に設定する
        /// </summary>
        void SetNextPosition()
        {
            nextPosition.SetNextPosition(subCharacter);
            PlayerInputController.Instance.ChangeState(StateType.None);     // 演出が終了したら解除
        }
    }
}