using UnityEngine;


/// <summary>
/// 通行止めやイベントをクリアしていないと進めないエリアを管理するクラス
/// </summary>
public class BlockerArea : MonoBehaviour
{
    [SerializeField] protected Interact[] eventObjects;
    [SerializeField] TalkData talkData;

    Collider2D localCol;
    TalkPanelController talk;

    void Awake()
    {
        localCol = GetComponent<Collider2D>();
    }

    protected virtual void Start()
    {
        talk = TalkPanelController.Instance;
    }

    protected virtual void OnCollisionEnter2D(Collision2D col)
    {
        if(!col.gameObject.CompareTag("Player")) return;

        if (!TryBlockerAreaDisabled())
        {
            StartTalk(0f);
            return;
        }

        localCol.enabled = false;
    }

    protected void StartTalk(float delay)
    {
        talk.OnToTalk(talkData, delay);
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