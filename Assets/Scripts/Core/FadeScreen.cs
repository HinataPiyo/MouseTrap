using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 画面のフェードを行う。
/// 暗転中に行いたい処理は、イベント購読ではなく <see cref="FadeInToFadeOut"/> の引数で渡す。
/// </summary>
public class FadeScreen : SceneSingleton<FadeScreen>
{
    [SerializeField] CanvasGroup canvasGroup;

    const float WAIT_BLACK_SCREEN_DURATION = 1f;
    readonly WaitForSeconds waitBlackScreenDuration = new WaitForSeconds(WAIT_BLACK_SCREEN_DURATION);

    protected override void OnSingletonAwake()
    {
        // 部屋移動直後は暗転した状態から始める
        if (GameSession.FadeOutPending)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
    }

    void Start()
    {
        if (!GameSession.FadeOutPending) return;

        GameSession.FadeOutPending = false;
        StartCoroutine(FadeOutOnSceneStart());
    }

    IEnumerator FadeOutOnSceneStart()
    {
        PlayerInputController input = PlayerInputController.Instance;
        input.ChangeState(StateType.Movie);

        yield return FadeOutCoroutine();

        if (input.CurrentState == StateType.Movie)
            input.ChangeState(StateType.None);
    }

    public IEnumerator FadeInCoroutine(float duration = 1f)
    {
        yield return FadeCanvasGroup(canvasGroup.alpha, 1f, duration);
    }

    public IEnumerator FadeOutCoroutine(float duration = 1f)
    {
        yield return FadeCanvasGroup(canvasGroup.alpha, 0f, duration);
    }

    /// <summary>
    /// 暗転して、完全に暗くなった時点で onBlackScreen を呼び、待機後にフェードアウトする
    /// </summary>
    /// <param name="onBlackScreen">画面が真っ暗になった瞬間に一度だけ呼ばれる処理</param>
    public IEnumerator FadeInToFadeOut(Action onBlackScreen = null, float duration = 1f)
    {
        yield return FadeInCoroutine(duration);
        onBlackScreen?.Invoke();
        yield return waitBlackScreenDuration;
        yield return FadeOutCoroutine(duration);
    }

    IEnumerator FadeCanvasGroup(float start, float end, float duration)
    {
        canvasGroup.blocksRaycasts = true;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = end;
        canvasGroup.blocksRaycasts = end > 0f;
    }
}
