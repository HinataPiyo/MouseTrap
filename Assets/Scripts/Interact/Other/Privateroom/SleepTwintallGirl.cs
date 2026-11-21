using System.Collections;
using Player;
using Unity.VisualScripting;
using UnityEngine;

public class SleepTwintallGirl : Interact
{
    [Header("起きてるツインテール少女")]
    [SerializeField] GameObject getUp_TwinTallGirl;

    [Header("ツインテール少女を起こしたときのPlayerの位置")]
    [SerializeField] Transform nextPlayerPosition;

    protected override void Initialize()
    {
    }

    protected override void AcquireItem()
    {
        StartCoroutine(StartTalk());
    }

    protected override IEnumerator StartTalk()
    {
        yield return new WaitForSeconds(1f);

        yield return talk.Writing(GetTalkData(0));      // 起こす会話

        yield return fadeScreen.FadeInCoroutine();

        // ツインテール少女を起こす処理
        getUp_TwinTallGirl.SetActive(true);
        GameObject player = GameObject.FindWithTag("Player");
        player.transform.position = nextPlayerPosition.position;

        // 自身を非表示にする
        GetComponent<Collider2D>().enabled = false;
        GetComponentInChildren<SpriteRenderer>().enabled = false;

        yield return fadeScreen.FadeOutCoroutine();

        yield return talk.Writing(GetTalkData(1));  // 起きた後の会話

        yield return fadeScreen.FadeInCoroutine();

        // 起こした少女を非表示にしPlayer操作に切り替える
        getUp_TwinTallGirl.SetActive(false);

        // キャラクターをツインテール少女にする
        CharacterControlHandler.Instance.SetCharacterControlState(Character.Name.Girl_TwinTail, true);
        CharacterControlHandler.Instance.ChangeCharacterControlState(Character.Name.Girl_TwinTail);

        yield return fadeScreen.FadeOutCoroutine();

        IsEventCompleted = true;


        yield return null;
    }
}