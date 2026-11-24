
using UnityEngine;

public class CursorController : MonoBehaviour
{
    [Header("Cursor Settings")]
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Vector2 hotSpot = Vector2.zero;
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;

    private void Start()
    {
        SetCustomCursor();
    }

    /// <summary>
    /// カスタムカーソルを設定する
    /// </summary>
    public void SetCustomCursor()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    /// <summary>
    /// デフォルトカーソルに戻す
    /// </summary>
    public void ResetCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
