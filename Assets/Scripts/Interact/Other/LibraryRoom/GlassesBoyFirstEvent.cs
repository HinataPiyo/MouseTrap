namespace Intetract.LibraryRoom
{
    using System.Collections;
    using Player;
    using UnityEngine;

    public class GlassesBoyFirstEvent : Interact
    {
        [Header("会話の内容")]
        [SerializeField] TalkData talkData;

        TalkPanelController talk;
        CharacterControlHandler characterState;
        FadeScreen fadeScreen;

        void Start()
        {
            talk = TalkPanelController.Instance;
            characterState = CharacterControlHandler.Instance;
            fadeScreen = FadeScreen.Instance;
            fadeScreen.OnFadeInComplete += CharacterChange;
        }

        public override bool TryInteract(out InteractData result)
        {
            if (base.TryInteract(out result))
            {
                if (!IsEventCompleted)
                    StartCoroutine(StartEvent());

                IsEventCompleted = true; // イベントが完了したことを設定
                return true;
            }
            return false;
        }

        IEnumerator StartEvent()
        {
            // 会話を開始する処理をここに記述
            talk.OnToTalk(talkData, 0f);
            yield return new WaitUntil(() => !talk.IsWriting);      // 会話が終了するまで待機

            yield return fadeScreen.FadeInToFadeOut();
        }

        void CharacterChange()
        {
            // キャラクタを操作できるようにする
            characterState.SetCharacterControlState(Character.Name.Glasses_Boy, true);

            // プレイヤーの位置をキャラクターの位置に合わせる
            GameObject player = GameObject.FindWithTag(Layer.Player);
            player.transform.position = transform.position;

            // キャラクターの操作状態を変更する
            characterState.ChangeCharacterControlState(Character.Name.Glasses_Boy);
            gameObject.SetActive(false);

            if(fadeScreen != null)
            {
                fadeScreen.OnFadeInComplete -= CharacterChange;
            }
        }
    }
}