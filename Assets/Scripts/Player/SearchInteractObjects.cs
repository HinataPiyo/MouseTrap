using UnityEngine;

/// <summary>
/// Interact可能なオブジェクトを範囲から取得するクラス
/// </summary>
public class SearchInteractObjects : MonoBehaviour
{
    IInteract closestInteract;
    IInputTrigger inputTrigger;
    IInteractObjectPanel interactObjectPanel;
    IInventoryService inventory;
    IGameState gameState;

    void Start()
    {
        inputTrigger = ApiProvider.Get<IInputTrigger>();
        interactObjectPanel = ApiProvider.Get<IInteractObjectPanel>();
        gameState = ApiProvider.Get<IGameState>();
        inventory = ApiProvider.Get<IInventoryService>();
        inputTrigger.OnInputEKeyPressed += OnInputAction;
    }

    /// <summary>
    /// Eキーが押されたときに呼ばれる処理
    /// </summary>
    void OnInputAction()
    {
        if (gameState.CurrentState != StateType.None)
        {
            // Interact状態の場合
            if (gameState.CurrentState == StateType.Interact)
            {
                Debug.Log("<color=red>No interactable object found.</color>");
                ClearCurrentInteract();
                interactObjectPanel.TogglePanelVisibility(null, string.Empty);
                return;
            }
        }


        // Interact可能なオブジェクトを取得する
        if (closestInteract != null && closestInteract.TryInteract(out var interactData))
        {
            // 取得後にオブジェクトを破棄する場合は、InteractMarkの表示を消す
            if (interactData.IsShowPanel)
            {
                interactObjectPanel.TogglePanelVisibility(interactData.Sprite, interactData.Text);
                inventory.AddItem(interactData);
                Debug.Log($"<color=green>ShowInteractObjectPanel</color> Sprite: {interactData.Sprite}, Text: {interactData.Text}");
            }
            else
            {
                interactObjectPanel.TogglePanelVisibility(null, string.Empty);
            }

            interactData.OnInteractAction?.Invoke();
            interactObjectPanel.ToggleMarkVisibility(closestInteract, false);
            Debug.Log($"<color=green>Interact successful.</color> InteractData: {interactData}");
        }

        closestInteract = null;
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
            interactObjectPanel.ToggleMarkVisibility(closestInteract, false);
            return;
        }

        closestInteract = interact;
        interactObjectPanel.ToggleMarkVisibility(closestInteract, true);
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
        if (closestInteract != null && closestInteract == interact)
        {
            interactObjectPanel.ToggleMarkVisibility(closestInteract, false);
        }
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
            interactObjectPanel.ToggleMarkVisibility(closestInteract, false);
        }

        closestInteract = null;
    }

    void OnDisable()
    {
        inputTrigger.OnInputEKeyPressed -= OnInputAction;
    }
}