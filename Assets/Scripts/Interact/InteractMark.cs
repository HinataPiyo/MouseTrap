using UnityEngine;

/// <summary>
/// WorldCanvasに表示するInteractMarkのPrefabを管理するクラス
/// </summary>
public class InteractMark : MonoBehaviour
{
    public void SetVisibility(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }
}