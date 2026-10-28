namespace Intetract.LibraryRoom
{
    using System.Collections;
    using Player;
    using UnityEngine;

    public class GlassesBoyFirstEvent : Interact
    {
        protected override void Initialize() { }

        protected override void AcquireItem()
        {
            if(!IsEventCompleted)
                StartCoroutine(StartTalk());
            IsEventCompleted = true;
        }

        protected override IEnumerator StartTalk()
        {
            // 会話を開始する処理をここに記述
            yield return talk.Writing(GetTalkData(0), 0f);

            // 暗転した時点でキャラクターを切り替える
            yield return fadeScreen.FadeInToFadeOut(CharacterChange);
        }

        void CharacterChange()
        {
            // キャラクタを操作できるようにする
            CharacterControlHandler.Instance.
                SetCharacterControlState(Character.Name.Glasses_Boy, true);

            // プレイヤーの位置をキャラクターの位置に合わせる
            GameObject player = GameObject.FindWithTag(Layer.Player);
            player.transform.position = transform.position;

            // キャラクターの操作状態を変更する
            CharacterControlHandler.Instance.
                ChangeCharacterControlState(Character.Name.Glasses_Boy);

            Invisible();
        }

        void Invisible()
        {
            GetComponent<Collider2D>().enabled = false;
            GetComponentInChildren<SpriteRenderer>().enabled = false;
        }
    }
}