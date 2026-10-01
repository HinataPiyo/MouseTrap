namespace Intetract.LibraryRoom
{
    using UnityEngine;

    public class GlassesBoyFirstEvent : Interact
    {
        [Header("会話の内容")]
        [SerializeField] TalkData talkData;

        ITalk talk;

        void Start()
        {
            talk = ApiProvider.Get<ITalk>();
        }

        public override bool TryInteract(out InteractData result)
        {
            if (base.TryInteract(out result))
            {
                // 会話を開始する処理をここに記述
                talk.OnToTalk(talkData, 0f);

                IsEventCompleted = true; // イベントが完了したことを設定
                return true;
            }
            return false;
        }
    }
}