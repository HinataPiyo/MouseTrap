using UnityEngine;
using VContainer;

public class Intaract : MonoBehaviour, IIntaract
{
    [SerializeField] IntaractData intaractData;
    [SerializeField] Transform intaractMarkPosition;
    public Transform GetMarkPoint() => intaractMarkPosition;
    public IntaractMarkType IntaractMarkType => intaractData.IntaractMarkType;

    [Inject]
    IEventBus eventBus;

    /// <summary>
    /// Intaractオブジェクトを取得した際にPanelに表示する処理
    /// </summary>
    public void Interact()
    {
        Debug.Log($"Intaract: {intaractData.name}");
        eventBus.Publish(new ToggleIntaractObjectPanelVisibility(new ShowIntaractObjectPanel(intaractData.Sprite, intaractData.Text)));
    }

    public void ShowMark()
    {
        eventBus.Publish(new ShowIntaractMark(this));
    }

    public void HideMark()
    {
        eventBus.Publish(new HideIntaractMark(this));
    }
}