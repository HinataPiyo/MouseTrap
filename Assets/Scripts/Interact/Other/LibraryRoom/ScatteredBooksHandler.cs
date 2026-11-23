using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// 眼鏡少年が最初の本をゲットしたのち散らばる本を管理する
/// 最後の一冊は最初に拾った本を配置、または足りないことに気づいたときに現れる
/// </summary>
public class ScatteredBooksHandler : Interact
{
    [SerializeField] List<GameObject> books = new List<GameObject>();

    protected override void Initialize() { }
    protected override void AcquireItem() { }
    protected override IEnumerator StartTalk()
    {
        PlayerInputController.Instance.ChangeState(StateType.Talk);
        yield return new WaitForSeconds(1f);
        yield return talk.Writing(GetTalkData(0));

        PlayerInputController.Instance.ChangeState(StateType.Interact);
        yield return new WaitUntil(() => !InteractUIPanel.Instance.IsVisible());

        PlayerInputController.Instance.ChangeState(StateType.None);
    }

    /// <summary>
    /// 本を取ったときに呼ぶ
    /// 本を全部取ったか確認
    /// </summary>
    public void DeactivateBooks(GameObject book)
    {
        books.Remove(book);
        book.SetActive(false);
        CheckGetAllBooks();
    }

    /// <summary>
    /// 本が全部取られたか確認する
    /// </summary>
    void CheckGetAllBooks()
    {
        if (books.Count == 0)
        {
            IsEventCompleted = true;
            MissionManager.Instance.CompleteMission(MissionNames.CollectScatteredBooks);
            StartCoroutine(StartTalk());
        }
    }

    /// <summary>
    /// 本を全部表示する
    /// </summary>
    public void ActivateBooks()
    {
        foreach (var book in books)
            book.SetActive(true);

        MissionManager.Instance.SetMission(MissionNames.CollectScatteredBooks);
    }
}