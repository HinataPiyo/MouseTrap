using System.Collections;
using UnityEngine;

public class FirstGetMemo : Interact
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(base.TryInteract(out InteractData result))
            {
                StartCoroutine(StartTalk());
            }
        }
    }
    protected override void Initialize() { }
    protected override void AcquireItem() { }

    protected override IEnumerator StartTalk()
    {
        // if(interactData.IsShowPanel)
        //     InteractUIPanel.Instance.ShowInteractPanel(interactData);

        // ToDo パネル非表示後下の処理を行いたいのだがうまくいっていない
        yield return new WaitUntil(() => !InteractUIPanel.Instance.IsVisible());
        yield return talk.Writing(GetTalkData(0));

        if(interactData.IsDisabledAfterInteract)
            gameObject.SetActive(false);
    }
}