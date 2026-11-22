namespace Intetract.StaffRoom.Boy
{
    using System.Collections;
    using UnityEngine;

    public class LockerIntetract : Interact
    {
        [Header("表示するキャラクター")]
        [SerializeField] GameObject subCharacter;

        [Header("プレイヤーの位置")]
        [SerializeField] Transform playerSetPoint;

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
            MissionManager.Instance.CompleteMissionAndSetNext(
                MissionNames.CheckLocker,
                MissionNames.FindKey);
            StartCoroutine(StartTalk());
        }

        protected override IEnumerator StartTalk()
        {
            PlayerInputController.Instance.ChangeState(StateType.Movie);     // 演出状態に遷移する
            yield return fadeScreen.FadeInCoroutine(0.2f);
            subCharacter.SetActive(true);       // 少年を表示
            GameObject player = GameObject.FindWithTag("Player");
            player.transform.position = playerSetPoint.position;

            yield return fadeScreen.FadeOutCoroutine(0.2f);

            yield return talk.Writing(GetTalkData(0), 1f);

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