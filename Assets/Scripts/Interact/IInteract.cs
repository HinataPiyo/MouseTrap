using UnityEngine;

public interface IInteract
{
    InteractMarkType InteractMarkType { get; }
    Transform GetMarkPoint();
    bool IsAvailable { get; }
    bool TryInteract(out InteractData interactData);
}