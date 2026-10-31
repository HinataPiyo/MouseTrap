using System.Collections;
using UnityEngine;

/// <summary>
/// ここでは、Playerが眼鏡少年に話をかけた後に最初の本を取得する際の処理を扱う
/// </summary>
public class FirstConectingBook : Interact
{
    [SerializeField] GameObject[] books;

    protected override void Initialize() { }
    protected override void AcquireItem()
    {
        if(!IsEventCompleted)
            StartCoroutine(StartTalk());
    }

    protected override IEnumerator StartTalk()
    {
        yield return new WaitUntil(() => !InteractUIPanel.Instance.IsVisible());
        yield return fadeScreen.FadeInCoroutine();
        HideBook();
        yield return fadeScreen.FadeOutCoroutine();
        yield return talk.Writing(GetTalkData(0));


        IsEventCompleted = true;
        yield return null;
    }

    /// <summary>
    /// 眼鏡少年をかこっている本を隠す
    /// </summary>
    public void HideBook()
    {
        foreach(var book in books)
            book.SetActive(false);
        
        Collider2D[] col = GetComponents<Collider2D>();
        foreach(var c in col)
            c.enabled = false;
    }

    void Update()
    {
        
    }
}
