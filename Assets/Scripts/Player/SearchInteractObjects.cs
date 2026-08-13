using UnityEngine;
using VContainer;

/// <summary>
/// Interact可能なオブジェクトを範囲から取得するクラス
/// </summary>
public class SearchInteractObjects : MonoBehaviour
{
    IInteract closestInteract;
    IInputTrigger inputTrigger;
    IEventBus eventBus;

    [Inject]
    public void Construct(IInputTrigger inputTrigger, IEventBus eventBus)
    {
        this.inputTrigger = inputTrigger;
        this.eventBus = eventBus;
    }

    void OnEnable()
    {
        inputTrigger.OnInputEKeyPressed += OnInputAction;
    }

    /// <summary>
    /// Eキーが押されたときに呼ばれる処理
    /// </summary>
    void OnInputAction()
    {
        // 取得可能なInteractオブジェクトがなければ何もしない
        if (closestInteract == null || !closestInteract.IsAvailable)
        {
            ClearCurrentInteract();
            eventBus.Publish(new Player.OnChangePlayerStop(false));
            return;
        }

        // Interact可能なオブジェクトを取得する
        if (closestInteract.TryInteract(out var interactData))
        {
            eventBus.Publish(new ToggleInteractObjectPanelVisibility(interactData.Sprite, interactData.Text));
            eventBus.Publish(new OnItemChanged(new Inventory.ItemData { item = interactData, count = 1 }, true));
            eventBus.Publish(new Player.OnChangePlayerStop(true));
        }
    }

    /// <summary>
    /// コライダーに入ったときに呼ばれる処理
    /// 取得可能なInteractオブジェクトがあれば、InteractMarkを表示
    /// </summary>
    /// <param name="col"></param>
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer(Layer.Interactable)) return;

        IInteract interact = col.GetComponent<IInteract>();
        if (interact == null || !interact.IsAvailable) return;

        // すでに別のInteractオブジェクトがあれば、そちらのInteractMarkを非表示にする
        if (closestInteract != null && closestInteract != interact)
        {
            eventBus.Publish(new ToggleMarkVisibility(closestInteract, false));
        }

        closestInteract = interact;
        eventBus.Publish(new ToggleMarkVisibility(closestInteract, true));
    }

    /// <summary>
    /// コライダーから離れたときに呼ばれる処理
    /// InteractMarkを非表示にする
    /// </summary>
    /// <param name="col"></param>
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer(Layer.Interactable)) return;

        IInteract interact = col.GetComponent<IInteract>();

        // 離れたオブジェクトが現在のInteractと同じでなければ何もしない
        if (interact == null) return;

        eventBus.Publish(new ToggleMarkVisibility(interact, false));
        closestInteract = null;
    }

    /// <summary>
    /// 現在のInteractをクリアする
    /// クリアする際に、InteractMarkの表示を消す処理も
    /// </summary>
    void ClearCurrentInteract()
    {
        if (closestInteract != null)
        {
            eventBus.Publish(new ToggleMarkVisibility(closestInteract, false));
        }

        eventBus.Publish(new ToggleInteractObjectPanelVisibility(null, string.Empty));
        closestInteract = null;
    }

    void OnDisable()
    {
        inputTrigger.OnInputEKeyPressed -= OnInputAction;
    }
}