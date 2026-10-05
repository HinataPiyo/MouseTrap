namespace Intetract.StaffRoom
{
    using UnityEngine;
    
    public class HiddenHit : Interact
    {
        [Header("閲覧する時の会話内容")]
        [SerializeField] TalkData talkData_HiddenHit;

        public override bool TryInteract(out InteractData result)
        {
            if (base.TryInteract(out result))
            {
                // 会話を開始する処理をここに記述
                TalkPanelController.Instance.OnToTalk(talkData_HiddenHit, 0f);

                IsEventCompleted = true; // イベントが完了したことを設定
                return true;
            }
            return false;
        }
    }
}