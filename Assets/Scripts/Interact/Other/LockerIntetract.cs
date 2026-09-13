namespace Intetract.StaffRoom.Boy
{
    using System.Collections;
    using UnityEngine;
    
    public class LockerIntetract : Interact
    {
        [Header("少年と話す内容")]
        [SerializeField] TalkData talkData;

        [Header("表示するキャラクター")]
        [SerializeField] GameObject subCharacter;

        [Header("会話終了後待機する次の位置")]
        [SerializeField] NextPosition nextPosition;

        ITalk talk;
        IFadeScreen fadeScreen;

        void Start()
        {
            talk = ApiProvider.Get<ITalk>();
            HideSubCharacter();

            fadeScreen = ApiProvider.Get<IFadeScreen>();
            fadeScreen.OnFadeInComplete += SetNextPosition;        // フェードイン完了時にサブキャラクターの位置を次の位置に設定する
        }

        void OnDestroy()
        {
            fadeScreen.OnFadeInComplete -= SetNextPosition;
        }

        public override bool TryInteract(out InteractData result)
        {
            if (base.TryInteract(out result))
            {
                ShowSubCharacter();
                IsEventCompleted = true; // イベントが完了したことを設定
                return true;
            }
            return false;
        }

        /// <summary>
        /// サブキャラクターを表示させる処理
        /// </summary>
        public void ShowSubCharacter()
        {
            StartCoroutine(TalkToBoy());
        }

        public IEnumerator TalkToBoy()
        {
            subCharacter.SetActive(true);
            //! ここではキャラクターの操作を有効にしない
            // characterState.SetCharacterControlState(Character.Name.Boy, true);
            talk.OnToTalk(talkData, 1f);

            yield return new WaitUntil(() => talk.IsWriting == false);

            fadeScreen.FadeInToFadeOut();
        }

        void HideSubCharacter()
        {
            subCharacter.SetActive(false);
            if (fadeScreen != null)
            {
                fadeScreen.OnFadeInComplete -= SetNextPosition;
            }
        }

        void SetNextPosition()
        {
            nextPosition.SetNextPosition(subCharacter);
        }
    }
}