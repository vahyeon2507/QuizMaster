using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scale")]
    public float hoverScale = 1.08f;
    public float scaleDuration = 0.12f;

    [Header("Color")]
    public bool changeColor = true;
    public Color normalColor = Color.white;
    public Color hoverColor = new Color(1f, 1f, 1f, 0.9f); // 살짝 밝게
    public bool changeTextColor = false;
    public Color normalTextColor = Color.white;
    public Color hoverTextColor = Color.yellow;

    [Header("Optional Sound")]
    public AudioClip hoverSound;
    [Range(0f, 1f)] public float hoverVolume = 0.6f;
    public bool playOnEnterOnly = true;

    // internals
    RectTransform rect;
    Coroutine scaleRoutine;
    Image targetImage;
    TextMeshProUGUI targetText;
    AudioSource audioSource;
    Button button;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        button = GetComponent<Button>();
        targetImage = GetComponent<Image>();
        targetText = GetComponentInChildren<TextMeshProUGUI>();

        // audio: 자동으로 붙이기 (없다면)
        if (hoverSound != null)
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.clip = hoverSound;
            audioSource.volume = hoverVolume;
        }

        // 초기 색 세팅
        if (targetImage != null) targetImage.color = normalColor;
        if (targetText != null) targetText.color = normalTextColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button != null && !button.interactable) return; // 비활성 상태면 반응 금지

        // scale up
        if (scaleRoutine != null) StopCoroutine(scaleRoutine);
        scaleRoutine = StartCoroutine(ScaleTo(Vector3.one * hoverScale, scaleDuration));

        // color change
        if (changeColor && targetImage != null) targetImage.color = hoverColor;
        if (changeTextColor && targetText != null) targetText.color = hoverTextColor;

        // sound
        if (hoverSound != null && audioSource != null)
        {
            if (!playOnEnterOnly || (playOnEnterOnly && !audioSource.isPlaying))
                audioSource.PlayOneShot(hoverSound, hoverVolume);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (scaleRoutine != null) StopCoroutine(scaleRoutine);
        scaleRoutine = StartCoroutine(ScaleTo(Vector3.one, scaleDuration));

        // revert color
        if (changeColor && targetImage != null) targetImage.color = normalColor;
        if (changeTextColor && targetText != null) targetText.color = normalTextColor;
    }

    IEnumerator ScaleTo(Vector3 target, float duration)
    {
        Vector3 start = rect.localScale;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            rect.localScale = Vector3.Lerp(start, target, Mathf.SmoothStep(0f, 1f, t / duration));
            yield return null;
        }
        rect.localScale = target;
    }
}
