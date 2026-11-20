using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interact可能なオブジェクトを範囲から取得するクラス
/// </summary>
public class SearchInteractObjects : MonoBehaviour
{
    IInteract closestInteract;
    PlayerInputController inputController;
    Collider2D searchCollider;
    ContactFilter2D interactFilter;
    readonly List<Collider2D> overlappingColliders = new List<Collider2D>();

    void Start()
    {
        searchCollider = GetComponent<Collider2D>();
        if (searchCollider == null)
        {
            Debug.LogError("SearchInteractObjects requires a Collider2D.", this);
            enabled = false;
            return;
        }

        interactFilter.SetLayerMask(LayerMask.GetMask(Layer.Interactable));
        interactFilter.useTriggers = true;
        inputController = PlayerInputController.Instance;
        inputController.OnInputEKeyPressed += OnInputAction;
    }

    void OnEnable()
    {
        if (inputController != null)
            inputController.OnInputEKeyPressed += OnInputAction;
    }

    void Update()
    {
        RefreshInteractTarget();
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
            inputController.CancelCurrentOperation();
            Debug.Log("<color=red>Reset interactable object.</color>");
            return;
        }

        bool restrictions = inputController.CurrentState == StateType.Talk || inputController.CurrentState == StateType.RoomChange;
        // 行動制限中は無視
        if (restrictions) return;

        RefreshInteractTarget();

        // 必須イベントが未完了の場合は、Interactを実行しない
        if (closestInteract != null && !closestInteract.TryCompleteRequired())
        {
            Debug.Log("Required event not completed");
            return;
        }

        // Interact可能なオブジェクトを取得する
        if (closestInteract != null && closestInteract.TryInteract(out var interactData))
        {
            // InteractMarkを消す処理
            WorldCanvasController.Instance.ToggleInteractMarkVisibility(false, transform.position);
            Debug.Log($"<color=green>Interact successful.</color> InteractData: {interactData}");
        }

        closestInteract = null;
    }

    void RefreshInteractTarget()
    {
        searchCollider.Overlap(interactFilter, overlappingColliders);
        IInteract candidate = null;
        foreach (Collider2D col in overlappingColliders)
        {
            IInteract interact = col.GetComponent<IInteract>();
            if (interact == null || !interact.IsAvailable || !interact.TryCompleteRequired()) continue;

            if (interact == closestInteract)
            {
                candidate = interact;
                break;
            }
            if (candidate == null) candidate = interact;
        }

        closestInteract = candidate;
        StateType state = inputController.CurrentState;
        bool canInteract = state == StateType.None || state == StateType.Inventory || state == StateType.CharacterInfo;
        WorldCanvasController.Instance.ToggleInteractMarkVisibility(canInteract && candidate != null, transform.position);
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
        closestInteract = null;
        if (WorldCanvasController.Instance != null)
            WorldCanvasController.Instance.ToggleInteractMarkVisibility(false, transform.position);
    }
}