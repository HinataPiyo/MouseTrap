using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;


/// <summary>
/// Intaract可能なオブジェクトを範囲から取得するクラス
/// </summary>
public class SearchIntaractObjects : MonoBehaviour
{
    IIntaract closestIntaract;

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
        if (closestIntaract != null) closestIntaract.Interact();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.layer != LayerMask.NameToLayer(Layer.Intaractable)) return;

        IIntaract intaract = col.GetComponent<IIntaract>();
        if (intaract != null)
        {
            closestIntaract = intaract;
            closestIntaract.ShowMark();
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.gameObject.layer != LayerMask.NameToLayer(Layer.Intaractable)) return;

        if (closestIntaract != null)
        {
            closestIntaract.HideMark();
            closestIntaract = null;
        }
    }

    void OnDisable()
    {
        inputTrigger.OnInputEKeyPressed -= OnInputAction;
    }
}