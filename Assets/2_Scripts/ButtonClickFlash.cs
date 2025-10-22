using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 버튼 클릭 시 색/스케일로 번쩍이는 효과를 줌.
/// Inspector에서 색, 지속시간, 스케일 등을 조절할 수 있음.
/// Quiz 스크립트에서 PlayFlash(bool? isCorrect) 로 호출 가능.
/// If isCorrect == true => correctColor, false => wrongColor, null => neutral flash.
/// </summary>
[RequireComponent(typeof(Image))]
public class ButtonClickFlash : MonoBehaviour, IPointerClickHandler
{
    [Header("Target")]
    public Image targetImage; // 없으면 자동으로 할당
    Color originalColor;

    [Header("Flash Colors")]
    public Color neutralFlashColor = new Color(1f, 1f, 1f, 0.9f);
    public Color correctColor = new Color(0.2f, 1f, 0.3f, 1f);
    public Color wrongColor = new Color(1f, 0.4f, 0.4f, 1f);

    [Header("Timing")]
    public float flashInDuration = 0.08f;
    public float flashOutDuration = 0.12f;

    [Header("Scale (optional)")]
    public bool useScale = true;
    public float scaleAmount = 1.06f;
    public float scaleDuration = 0.12f;

    // internals
    Vector3 originalScale;
    Coroutine flashRoutine;

    void Awake()
    {
        if (targetImage == null) targetImage = GetComponent<Image>();
        originalColor = targetImage != null ? targetImage.color : Color.white;
        originalScale = transform.localScale;
    }

    // 자동으로 클릭 시에도 효과 재생(선택사항)
    public void OnPointerClick(PointerEventData eventData)
    {
        // 기본: 중립 플래시 (null)
        PlayFlash(null);
    }

    /// <summary>
    /// 호출용: isCorrect == true/false/null
    /// </summary>
    public void PlayFlash(bool? isCorrect)
    {
        if (targetImage == null) return;
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine(isCorrect));
    }

    IEnumerator FlashRoutine(bool? isCorrect)
    {
        Color from = targetImage.color;
        Color to;
        if (isCorrect == true) to = correctColor;
        else if (isCorrect == false) to = wrongColor;
        else to = neutralFlashColor;

        // in
        float t = 0f;
        if (useScale) StartCoroutine(ScaleTo(originalScale * scaleAmount, scaleDuration * 0.6f));
        while (t < flashInDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / flashInDuration);
            targetImage.color = Color.Lerp(from, to, Mathf.SmoothStep(0f, 1f, p));
            yield return null;
        }

        // out
        t = 0f;
        while (t < flashOutDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / flashOutDuration);
            targetImage.color = Color.Lerp(to, originalColor, Mathf.SmoothStep(0f, 1f, p));
            yield return null;
        }
        targetImage.color = originalColor;

        // revert scale
        if (useScale) StartCoroutine(ScaleTo(originalScale, scaleDuration * 0.6f));

        flashRoutine = null;
    }

    IEnumerator ScaleTo(Vector3 target, float duration)
    {
        Vector3 start = transform.localScale;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            transform.localScale = Vector3.Lerp(start, target, Mathf.SmoothStep(0f, 1f, t / duration));
            yield return null;
        }
        transform.localScale = target;
    }
}
