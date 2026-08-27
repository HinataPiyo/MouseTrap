namespace Intetract
{
    using UnityEngine;
    
    public class LockerIntetract : Interact
    {
        [Header("少年と話す内容")]
        [SerializeField] TalkData talkData;

        [Header("表示するキャラクター")]
        [SerializeField] GameObject subCharacter;

        ITalk talk;

        void Start()
        {
            talk = ApiProvider.Get<ITalk>();
            subCharacter.SetActive(false);
        }

        public override bool TryInteract(out InteractData result)
        {
            if (base.TryInteract(out result))
            {
                ShowSubCharacter();
                return true;
            }
            return false;
        }

        /// <summary>
        /// サブキャラクターを表示させる処理
        /// </summary>
        public void ShowSubCharacter()
        {
            subCharacter.SetActive(true);

            talk.OnToTalk(talkData, 1f);
        }
    }
}