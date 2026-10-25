using System.Collections;
using UnityEngine;


/// <summary>
/// 通行止めやイベントをクリアしていないと進めないエリアを管理するクラス
/// </summary>
public class BlockerArea : Interact
{
    [SerializeField] protected Interact[] eventObjects;
    Collider2D localCol;


    void Awake()
    {
        localCol = GetComponent<Collider2D>();
    }

    protected virtual void OnCollisionEnter2D(Collision2D col)
    {
        if(!col.gameObject.CompareTag("Player")) return;

        if (!TryBlockerAreaDisabled())
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

    /// <summary>
    /// 通行止めが解除できるか
    /// </summary>
    /// <param name="completed">イベントが完了しているかどうか</param>
    /// <returns>通行止めが解除されたかどうか</returns>
    protected bool TryBlockerAreaDisabled()
    {
        foreach (var eventObject in eventObjects)
        {
            if (!eventObject.IsEventCompleted)
            {
                return false;
            }
        }
        return true;
    }
}