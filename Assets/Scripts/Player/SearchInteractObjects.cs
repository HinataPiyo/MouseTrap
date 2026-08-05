using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;


/// <summary>
/// Interact可能なオブジェクトを範囲から取得するクラス
/// </summary>
public class SearchInteractObjects : MonoBehaviour
{
    IInteract closestInteract;

    [Inject]
    IInputTrigger inputTrigger;

    void OnEnable()
    {
        // InputTriggerのイベントに登録(EKey押下で発火)
        inputTrigger.OnInputEKeyPressed += OnInputAction;
    }

    /// <summary>
    /// Eキーが押されたときに呼ばれる処理
    /// </summary>
    void OnInputAction()
    {
        if (closestInteract != null) closestInteract.ApplyInteract();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.layer != LayerMask.NameToLayer(Layer.Interactable)) return;

        IInteract interact = col.GetComponent<IInteract>();
        if (interact != null)
        {
            closestInteract = interact;
            closestInteract.ShowMark();
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.gameObject.layer != LayerMask.NameToLayer(Layer.Interactable)) return;

        if (closestInteract != null)
        {
            closestInteract.HideMark();
            closestInteract = null;
        }
    }

    void OnDisable()
    {
        inputTrigger.OnInputEKeyPressed -= OnInputAction;
    }
}