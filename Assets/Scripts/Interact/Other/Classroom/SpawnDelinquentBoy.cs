using System.Collections;
using Player;
using UnityEngine;

public class SpawnDelinquentBoy : Interact
{
    [SerializeField] GameObject delinquentBoy;
    [Header("不良少年(0)とPlayer(1)の位置を設定")]
    [SerializeField] Transform[] setPosition;
    protected override void Initialize()
    {
        delinquentBoy.SetActive(false);
    }

    protected override void AcquireItem()
    {
        if(!IsEventCompleted)
            StartCoroutine(StartTalk());
    }

    /// <summary>
    /// 不良少年との会話
    /// 会話前に位置を設定する
    /// </summary>
    protected override IEnumerator StartTalk()
    {
        yield return fadeScreen.FadeInCoroutine();
        delinquentBoy.SetActive(true);
        delinquentBoy.transform.position = setPosition[0].position;
        GameObject player = GameObject.FindWithTag("Player");
        player.transform.position = setPosition[1].position;
        yield return fadeScreen.FadeOutCoroutine();

        yield return new WaitForSeconds(1f);
        yield return talk.Writing(GetTalkData(0));

        yield return fadeScreen.FadeInCoroutine();
        delinquentBoy.SetActive(false);

        // 操作するキャラを変更する
        CharacterControlHandler.Instance.SetCharacterControlState(Character.Name.Delinquent_Boy, true);
        CharacterControlHandler.Instance.ChangeCharacterControlState(Character.Name.Delinquent_Boy);

        yield return fadeScreen.FadeOutCoroutine();

        IsEventCompleted = true;
        
        yield return null;
    }
}