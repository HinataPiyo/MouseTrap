using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

/// <summary>
/// 教室に存在する鍵を取得するクラス
/// </summary>
public class KeyFormClassroom : Interact
{
    [SerializeField] GameObject default_table_Group;
    [SerializeField] GameObject changed_table_group;

    [SerializeField] CinemachineCamera cam;
    [SerializeField] float camOrthographicSize = 3f;
    protected override void Initialize()
    {
    }

    protected override void AcquireItem()
    {
        if(!IsEventCompleted)
            StartCoroutine(StartTalk());
    }

    protected override IEnumerator StartTalk()
    {
        PlayerInputController.Instance.ChangeState(StateType.Movie);
        yield return new WaitForSeconds(1f);
        yield return talk.Writing(GetTalkData(0));
        GameObject player = GameObject.FindWithTag("Player");

        yield return fadeScreen.FadeInCoroutine();

        // UIと操作切替
        InteractUIPanel.Instance.HideInteractPanel();
        PlayerInputController.Instance.ChangeState(StateType.Movie);

        // tableを全て非表示にする
        player.SetActive(false);
        changed_table_group.SetActive(false);

        // カメラ表示を切り替え
        cam.Lens.OrthographicSize = camOrthographicSize;
        cam.Target.TrackingTarget = changed_table_group.transform;

        yield return fadeScreen.FadeOutCoroutine();

        yield return new WaitForSeconds(1f);

        ReturnTableUIPanel.Instance.Show();

        IsEventCompleted = true;
        yield return null;
    }
}