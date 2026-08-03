using UnityEngine;

public interface IIntaract
{
    IntaractMarkType IntaractMarkType { get; }
    Transform GetMarkPoint();
    void Interact();
    void ShowMark();
    void HideMark();
}