using UnityEngine;

/// <summary>
/// Interact可能なオブジェクトを範囲から取得するクラス
/// </summary>
public class SearchInteractObjects : MonoBehaviour
{
    IInteract closestInteract;
    PlayerInputController inputController;

    void Start()
    {
        inputController = PlayerInputController.Instance;
        inputController.OnInputEKeyPressed += OnInputAction;
    }

    /// <summary>
    /// Eキーが押されたときに呼ばれる処理
    /// </summary>
    void OnInputAction()
    {
        // Interact状態の場合
        if (inputController.CurrentState == StateType.Interact)
        {
            ClearCurrentInteract();
            InteractUIPanel.Instance.HideInteractPanel();
            PlayerInputController.Instance.ChangeState(StateType.None);     // 行動制限を解除
            Debug.Log("<color=red>Reset interactable object.</color>");
            return;
        }
        
        bool restrictions = inputController.CurrentState == StateType.Talk || inputController.CurrentState == StateType.RoomChange;
        // 行動制限中は無視
        if(restrictions) return;

        // Interact可能なオブジェクトを取得する
        if (closestInteract != null && closestInteract.TryInteract(out var interactData))
        {
            // InteractMarkを消す処理
            WorldCanvasController.Instance.ToggleInteractMarkVisibility(false, transform.position);
            Debug.Log($"<color=green>Interact successful.</color> InteractData: {interactData}");
        }

        closestInteract = null;
    }

    /// <summary>
    /// 取得可能なInteractオブジェクトがあれば、InteractMarkを表示
    /// </summary>
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer(Layer.Interactable)) return;

        IInteract interact = col.GetComponent<IInteract>();
        if (interact == null || !interact.IsAvailable) return;

        // すでに別のInteractオブジェクトがあれば、そちらのInteractMarkを非表示にする
        if (closestInteract != null && closestInteract != interact)
        {
            WorldCanvasController.Instance.ToggleInteractMarkVisibility(false, transform.position);
            return;
        }

        closestInteract = interact;
        WorldCanvasController.Instance.ToggleInteractMarkVisibility(true, transform.position);
    }

    void OnTriggerStay2D(Collider2D col)
    {
                if (col.gameObject.layer != LayerMask.NameToLayer(Layer.Interactable)) return;

        IInteract interact = col.GetComponent<IInteract>();
        if (interact == null || !interact.IsAvailable) return;

        // すでに別のInteractオブジェクトがあれば、そちらのInteractMarkを非表示にする
        if (closestInteract != null && closestInteract != interact)
        {
            WorldCanvasController.Instance.ToggleInteractMarkVisibility(false, transform.position);
            return;
        }

        closestInteract = interact;
        WorldCanvasController.Instance.ToggleInteractMarkVisibility(true, transform.position);
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer(Layer.Interactable)) return;

        IInteract interact = col.GetComponent<IInteract>();

        // 離れたオブジェクトが現在のInteractと同じでなければ何もしない
        if (interact == null) return;
        if (closestInteract != null && closestInteract == interact)
        {
            WorldCanvasController.Instance.ToggleInteractMarkVisibility(false, transform.position);
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
            WorldCanvasController.Instance.ToggleInteractMarkVisibility(false, transform.position);
        }

        closestInteract = null;
    }

    void OnDisable()
    {
        if (inputController != null)
        {
            inputController.OnInputEKeyPressed -= OnInputAction;
        }
    }
}