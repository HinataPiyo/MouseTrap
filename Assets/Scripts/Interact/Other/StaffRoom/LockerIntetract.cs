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

            // 暗転した時点でサブキャラクターの位置を次の位置に設定する
            yield return fadeScreen.FadeInToFadeOut(SetNextPosition);
        }

        /// <summary>
        /// 少年を非表示にする
        /// </summary>
        void HideSubCharacter()
        {
            subCharacter.SetActive(false);
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