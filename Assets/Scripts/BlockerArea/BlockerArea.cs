using System.Collections;
using UnityEngine;


/// <summary>
/// 通行止めやイベントをクリアしていないと進めないエリアを管理するクラス
/// </summary>
public class BlockerArea : Interact
{
    Collider2D localCol;


    void Awake()
    {
        localCol = GetComponent<Collider2D>();
    }

    protected virtual void OnCollisionEnter2D(Collision2D col)
    {
        if(!col.gameObject.CompareTag("Player")) return;

        if (!TryCompleteRequired())
        {
            StartCoroutine(StartTalk());
            return;
        }

        localCol.enabled = false;
    }

    protected override void Initialize() { }
    protected override void AcquireItem() { }

    protected override IEnumerator StartTalk()
    {
        talk.Writing(GetTalkData(0));
        yield return null;
    }
}