using System.Collections;
using UnityEngine;

public interface IInteract
{
    Transform GetMarkPoint();
    bool IsAvailable { get; }
    bool TryInteract(out InteractData interactData);
}

public abstract class Interact : MonoBehaviour, IInteract
{
    [Header("アイテム情報")]
    [SerializeField] protected InteractData interactData;

    [Header("マーク位置")]
    [SerializeField] Transform interactMarkPosition;

    protected TalkData GetTalkData(int index) => interactData.TalkData[index];
    public bool IsEventCompleted { get; protected set; } = false;   // イベントが完了したかどうかを管理するフラグ

    bool hasAcquiredItem;       // アイテムを取得したかどうかを管理するフラグ
    protected TalkPanelController talk;
    protected FadeScreen fadeScreen;

    public Transform GetMarkPoint() => interactMarkPosition;
    public bool IsAvailable => !hasAcquiredItem;

    string stateKey;

    void Start()
    {
        talk = TalkPanelController.Instance;
        fadeScreen = FadeScreen.Instance;
        stateKey = InteractStateStore.CreateKey(gameObject);

        Initialize();
        RestoreState();
    }

    // シーン破棄時に状態を保存する
    protected virtual void OnDestroy()
    {
        if (stateKey == null) return;

        InteractStateStore.Save(stateKey, new InteractStateStore.State
        {
            HasAcquiredItem = hasAcquiredItem,
            IsEventCompleted = IsEventCompleted,
            IsDeactivated = !gameObject.activeSelf
        });
    }

    void RestoreState()
    {
        if (!InteractStateStore.TryGet(stateKey, out var state)) return;

        hasAcquiredItem = state.HasAcquiredItem;
        IsEventCompleted |= state.IsEventCompleted;

        if (state.IsDeactivated)
            gameObject.SetActive(false);
    }

    // 初期化処理
    protected abstract void Initialize();

    // アイテム取得処理
    protected abstract void AcquireItem();

    // 会話開始処理
    protected abstract IEnumerator StartTalk();

    /// <summary>
    /// Interact可能なオブジェクトを取得する処理
    /// </summary>
    /// <param name="result">取得したInteractData</param>
    /// <returns>取得に成功したかどうか</returns>
    public virtual bool TryInteract(out InteractData result)
    {
        result = interactData;

        // すでにアイテムを取得している場合はInteract不可
        if (hasAcquiredItem) return false;

        // アイテムが再利用可能でない場合は取得済みフラグを立てる
        if(!interactData.IsReusable)
            hasAcquiredItem = true;

        Debug.Log($"Interact acquired: {interactData.name}");

        // Interact後にオブジェクトを無効化するかどうかの処理
        if (interactData.IsDisabledAfterInteract)
            gameObject.SetActive(false);

        //? SearchInteractObjectsの方で表示するようにしてしまっているので要検討
        if(interactData.IsShowPanel)
        {
            InteractUIPanel.Instance.ShowInteractPanel(interactData);
            Inventory.Instance.AddItem(interactData);
            Debug.Log($"<color=green>ShowInteractObjectPanel</color> Sprite: {interactData.Sprite}, Text: {interactData.Text}");
        }

        PlayerInputController.Instance.ChangeState(StateType.Interact);
        AcquireItem();

        return true;
    }
}