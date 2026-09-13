using UnityEngine;


/// <summary>
/// 通行止めやイベントをクリアしていないと進めないエリアを管理するクラス
/// </summary>
public class BlockerArea : MonoBehaviour
{
    [SerializeField] Interact[] eventObjects;
    [SerializeField] TalkData talkData;

    Collider2D localCol;

    void Awake()
    {
        localCol = GetComponent<Collider2D>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(!col.gameObject.CompareTag("Player")) return;

        Debug.Log("Player collided with BlockerArea");
        foreach (var eventObject in eventObjects)
        {
            if (!TryBlockerAreaDisabled(eventObject.IsEventCompleted))
            {
                Debug.Log("BlockerArea disabled");
                // ここには用はないtalkをする
                ApiProvider.Get<ITalk>().OnToTalk(talkData, 0f);
                return;
            }
        }
    }

    /// <summary>
    /// 通行止めが解除できるか
    /// </summary>
    /// <param name="completed">イベントが完了しているかどうか</param>
    /// <returns>通行止めが解除されたかどうか</returns>
    bool TryBlockerAreaDisabled(bool completed)
    {
        if (completed)
        {
            localCol.enabled = false;
            return true;
        }
        return false;
    }
}