using System.Collections;
using Player;
using UnityEngine;

/// <summary>
/// 椅子をジャンプするためのInteractクラス
/// </summary>
public class JumpChair : Interact
{
    [SerializeField] Transform nextPosition;
    [SerializeField] Transform backPosition;
    protected override void Initialize()
    {
    }

    protected override void AcquireItem()
    {
        StartCoroutine(StartTalk());
    }

    protected override bool CanInteract()
    {
        // 不良少年でないとジャンプできない
        return CharacterControlHandler.Instance.CurrentCharacter == Character.Name.Delinquent_Boy;
    }

    protected override IEnumerator StartTalk()
    {
        yield return fadeScreen.FadeInCoroutine(0.2f);
        GameObject player = GameObject.FindWithTag("Player");
        // 椅子の中心から見て右か左かで位置を決める
        if(transform.position.x < player.transform.position.x)      // 右にいる場合
            player.transform.position = nextPosition.position;
        else
            player.transform.position = backPosition.position;      // 左にいる場合

        yield return fadeScreen.FadeOutCoroutine(0.2f);

        PlayerInputController.Instance.ChangeState(StateType.None);
    }
}