using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 眼鏡少年が最初の本をゲットしたのち散らばる本を管理する
/// </summary>
public class ScatteredBooksHandler : Interact
{
    [SerializeField] List<GameObject> books = new List<GameObject>();
    [SerializeField] GameObject lastBook;

    protected override void Initialize() { }
    protected override void AcquireItem() { }
    protected override IEnumerator StartTalk() { yield return null; }

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
            ActivateLastBook();
            IsEventCompleted = true;
        }
    }

    /// <summary>
    /// 本を全部表示する
    /// </summary>
    public void ActivateBooks()
    {
        foreach (var book in books)
            book.SetActive(true);
    }

    /// <summary>
    /// 最後の本を表示する
    /// </summary>
    public void ActivateLastBook()
    {
        lastBook.SetActive(true);
    }
}