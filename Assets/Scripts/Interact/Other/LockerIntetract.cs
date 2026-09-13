namespace Intetract
{
    using System.Collections;
    using UnityEngine;
    
    public class LockerIntetract : Interact
    {
        [Header("少年と話す内容")]
        [SerializeField] TalkData talkData;

        [Header("表示するキャラクター")]
        [SerializeField] GameObject subCharacter;

        ITalk talk;
        IFadeScreen fadeScreen;
        Player.ICharacterState characterState;

        void Start()
        {
            talk = ApiProvider.Get<ITalk>();
            characterState = ApiProvider.Get<Player.ICharacterState>();
            HideSubCharacter();

            fadeScreen = ApiProvider.Get<IFadeScreen>();
            fadeScreen.OnFadeInComplete += HideSubCharacter;        // フェードイン完了時にサブキャラクターを非表示にする
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
            characterState.SetCharacterControlState(Character.Name.Boy, true);
            talk.OnToTalk(talkData, 1f);

            yield return new WaitUntil(() => talk.IsWriting == false);

            fadeScreen.FadeInToFadeOut();
        }

        void HideSubCharacter()
        {
            subCharacter.SetActive(false);
        }
    }
}