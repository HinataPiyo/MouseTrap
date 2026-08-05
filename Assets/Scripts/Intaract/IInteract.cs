using UnityEngine;

public interface IInteract
{
    InteractMarkType InteractMarkType { get; }
    Transform GetMarkPoint();
    void ApplyInteract();
    void ShowMark();
    void HideMark();
}