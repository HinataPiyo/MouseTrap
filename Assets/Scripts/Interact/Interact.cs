using UnityEngine;

public class Interact : MonoBehaviour, IInteract
{
    [SerializeField] protected InteractData interactData;
    [SerializeField] Transform interactMarkPosition;
    bool hasAcquiredItem;

    public Transform GetMarkPoint() => interactMarkPosition;
    public InteractMarkType InteractMarkType => interactData.InteractMarkType;
    public bool IsAvailable => !hasAcquiredItem;

    /// <summary>
    /// Interact可能なオブジェクトを取得する処理
    /// </summary>
    /// <param name="result">取得したInteractData</param>
    /// <returns>取得に成功したかどうか</returns>
    public bool TryInteract(out InteractData result)
    {
        result = interactData;

        if (hasAcquiredItem)
        {
            return false;
        }

        hasAcquiredItem = true;
        Debug.Log($"Interact acquired: {interactData.name}");

        if (interactData.IsDisabledAfterInteract)
        {
            gameObject.SetActive(false);
        }

        return true;
    }
}