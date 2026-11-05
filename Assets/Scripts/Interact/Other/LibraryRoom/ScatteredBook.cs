using System.Collections;
using UnityEngine;

public class ScatteredBook : Interact
{
    [SerializeField] ScatteredBooksHandler handler;
    protected override void Initialize() { }
    protected override void AcquireItem()
    {
        // 本を取ったのち非アクティブにする
        handler.DeactivateBooks(gameObject);
        IsEventCompleted = true;
    }
    
    protected override IEnumerator StartTalk()
    {
        yield return null;
    }
}