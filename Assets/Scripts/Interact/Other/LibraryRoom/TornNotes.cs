using System.Collections;
using UnityEngine;


/// <summary>
/// 破れたメモ紙の取得管理
/// </summary>
public class TornNotes : Interact
{
    protected override void Initialize() { }
    protected override void AcquireItem()
    {
        IsEventCompleted = true;
    }
    protected override IEnumerator StartTalk() { yield return null; }
}