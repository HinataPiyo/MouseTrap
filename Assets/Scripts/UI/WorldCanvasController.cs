using UnityEngine;

public class WorldCanvasController : SceneSingleton<WorldCanvasController>
{
    [SerializeField] InteractMark interactMark;
    static readonly Vector2 InteractMarkOffsetY = new Vector2(0.025f, 0.5f);
    Canvas worldCanvas;
    Transform interactMarkTarget;

    protected override void OnSingletonAwake()
    {
        worldCanvas = GetComponent<Canvas>();
        if (worldCanvas == null || worldCanvas.worldCamera == null || interactMark == null)
        {
            Debug.LogError("WorldCanvasController requires a Canvas with a camera and an InteractMark.", this);
            enabled = false;
            return;
        }

        interactMark.SetVisibility(false);
    }

    void OnEnable()
    {
        Canvas.preWillRenderCanvases += UpdateInteractMarkPosition;
    }

    void OnDisable()
    {
        Canvas.preWillRenderCanvases -= UpdateInteractMarkPosition;
    }

    /// <summary>
    /// InteractMarkの表示・非表示を切り替える
    /// </summary>
    /// <param name="isVisible">表示するかどうか</param>
    /// <param name="target">表示位置の基準</param>
    public void ToggleInteractMarkVisibility(bool isVisible, Transform target)
    {
        if (!enabled) return;

        interactMarkTarget = isVisible ? target : null;
        interactMark.SetVisibility(isVisible);
    }

    /// <summary>
    /// InteractMarkの位置を更新する
    /// </summary>
    void UpdateInteractMarkPosition()
    {
        if (interactMarkTarget == null) return;

        Vector3 position = interactMarkTarget.position + (Vector3)InteractMarkOffsetY;
        Vector2 screenPosition = worldCanvas.worldCamera.WorldToScreenPoint(position);
        if (!RectTransformUtility.ScreenPointToWorldPointInRectangle(
            (RectTransform)worldCanvas.transform, screenPosition, worldCanvas.worldCamera, out Vector3 canvasPosition))
        {
            Debug.LogError("Could not project the InteractMark onto the Canvas.", this);
            interactMarkTarget = null;
            interactMark.SetVisibility(false);
            enabled = false;
            return;
        }

        interactMark.transform.position = canvasPosition;
    }
}