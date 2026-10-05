using UnityEngine;

public class WorldCanvasController : MonoBehaviour
{
    public static WorldCanvasController Instance { get; private set; }
    [SerializeField] InteractMark interactMark;
    static readonly Vector2 InteractMarkOffsetY = new Vector2(0.025f, 0.5f);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetInstance()
    {
        Instance = null;
    }

    void OnEnable()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// InteractMarkの表示・非表示を切り替える
    /// </summary>
    /// <param name="isVisible">表示するかどうか</param>
    /// <param name="position">表示する位置</param>
    public void ToggleInteractMarkVisibility(bool isVisible, Vector2 position)
    {
        interactMark.SetVisibility(isVisible);
        if (isVisible)
        {
            interactMark.transform.position = position + InteractMarkOffsetY;
        }
    }
}