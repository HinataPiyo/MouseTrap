using System.Collections;


/// <summary>
/// 本棚の横のところにジャンルを確認できる箇所があるのでそのインタラクトクラス
/// </summary>
public class BookshelfGenre : Interact
{
    protected override void Initialize() { }
    protected override void AcquireItem()
    {
        StartCoroutine(StartTalk());
    }

    protected override IEnumerator StartTalk()
    {
        yield return talk.Writing(GetTalkData(0));

        if(!IsEventCompleted)
        {
            // 初回の会話
            yield return talk.Writing(GetTalkData(1));
        }

        IsEventCompleted = true;

        yield break;
    }
}