using UnityEngine;
using System.Collections;
public class FadeScreen : MonoBehaviour
{
    public static FadeScreen Instance { get; private set; }
    [SerializeField] CanvasGroup canvasGroup;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetInstance()
    {
        Instance = null;
    }

    public event System.Action OnFadeInComplete;
    public event System.Action OnFadeOutComplete;

    const float WAIT_BLACK_SCREEN_DURATION = 1f;
    WaitForSeconds waitBlackScreenDuration = new WaitForSeconds(WAIT_BLACK_SCREEN_DURATION);

    void OnEnable()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    void FadeIn(float duration = 1f)
    {
        StartCoroutine(FadeInCoroutine(duration));
    }

    void FadeOut(float duration = 1f)
    {
        StartCoroutine(FadeOutCoroutine(duration));
    }

    public IEnumerator FadeInCoroutine(float duration = 1f)
    {
        yield return FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 1, duration);
    }

    public IEnumerator FadeOutCoroutine(float duration = 1f)
    {
        yield return FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 0, duration);
    }

    public void FadeInToFadeOut(float duration = 1f)
    {
        StartCoroutine(FadeInThenOut(duration));
    }

    private IEnumerator FadeInThenOut(float duration)
    {
        FadeIn(duration);
        yield return new WaitUntil(() => canvasGroup.alpha == 1);
        yield return waitBlackScreenDuration;
        FadeOut(duration);
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        cg.blocksRaycasts = true;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }

        cg.alpha = end;

        if (end == 1)
        {
            OnFadeInComplete?.Invoke();
        }
        else if (end == 0)
        {
            OnFadeOutComplete?.Invoke();
        }
        cg.blocksRaycasts = false;
    }
}