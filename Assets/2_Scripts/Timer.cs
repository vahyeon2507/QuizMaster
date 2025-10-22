using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Timer : MonoBehaviour
{
    [Header("시간 설정")]
    [SerializeField] float problemTime = 10f; // 문제 푸는 시간
    [SerializeField] float solutionTime = 3f; // 정답, 오답 표기시간
    float time = 0f;

    [HideInInspector] public bool isProblemTime = true;
    [HideInInspector] public float fillAmount;
    [HideInInspector] public bool loadNextQuestion;

    [Header("타이머 이미지 (색상 제어)")]
    [SerializeField] Image timerImage; // 인스펙터에 연결
    [SerializeField] Color normalColor = new Color(0.2f, 0.9f, 0.3f, 1f); // 기본(초록계)
    [SerializeField] Color warnColor = new Color(1f, 0.85f, 0.2f, 1f); // 3초 경과 후 (노랑)
    [SerializeField] Color dangerColor = new Color(1f, 0.35f, 0.35f, 1f); // 6초 경과 후 (빨강)
    [SerializeField] Color solutionColor = new Color(0.8f, 0.8f, 0.8f, 1f); // 솔루션 타임 색

    [Header("마지막 카운트다운 UI (3,2,1)")]
    [SerializeField] TextMeshProUGUI lastSecondsText;   // 표시할 텍스트 (필수는 아님)
    [SerializeField] GameObject lastSecondsPanel;      // 텍스트 감춤/표시용 패널(옵션)
    [Tooltip("몇 초 이하일 때 마지막 카운트다운을 보여줄지 (기본: 3초)")]
    [SerializeField] float lastSecondsDisplayThreshold = 3f;

    [Header("펄스 애니메이션 설정")]
    [SerializeField] float pulseScale = 1.25f;
    [SerializeField] float pulseInDuration = 0.08f;
    [SerializeField] float pulseOutDuration = 0.12f;

    // 외부에서 현재 문제시간을 참고할 수 있게 (Quiz에서 배수 계산용)
    public float ProblemTime => problemTime;

    // 내부 상태
    int lastDisplayedSecond = -1;
    Coroutine pulseRoutine;

    private void Start()
    {
        time = problemTime;
        loadNextQuestion = true;

        // 기본 색/패널 상태
        if (timerImage != null)
        {
            timerImage.color = normalColor;
        }
        HideLastSecondsImmediate();
    }

    private void Update()
    {
        TimerCountDown();
        UpdateFillAmount();
        UpdateTimerColor();
        UpdateLastSecondsDisplay();
    }

    private void UpdateFillAmount()
    {
        if (isProblemTime)
            fillAmount = Mathf.Clamp01(time / problemTime);
        else
            fillAmount = Mathf.Clamp01(time / solutionTime);
    }

    private void TimerCountDown()
    {
        time -= Time.deltaTime;
        if (time <= 0f)
        {
            if (isProblemTime)
            {
                isProblemTime = false;
                time = solutionTime;
            }
            else
            {
                isProblemTime = true;
                time = problemTime;
                loadNextQuestion = true;
            }

            // 상태 전환 시 카운트다운 숨김
            HideLastSecondsImmediate();
            lastDisplayedSecond = -1;
        }
    }

    private void UpdateTimerColor()
    {
        if (timerImage == null) return;

        if (!isProblemTime)
        {
            timerImage.color = solutionColor;
            return;
        }

        float elapsed = problemTime - Mathf.Clamp(time, 0f, problemTime);

        if (elapsed >= 6f)
        {
            timerImage.color = dangerColor; // 6초 경과 → 빨강
        }
        else if (elapsed >= 3f)
        {
            timerImage.color = warnColor; // 3초 경과 → 노랑
        }
        else
        {
            timerImage.color = normalColor; // 초기(녹색)
        }
    }

    // 마지막 3초(또는 threshold 값) 동안 3,2,1 텍스트를 갱신 표시
    private void UpdateLastSecondsDisplay()
    {
        if (lastSecondsText == null && lastSecondsPanel == null) return; // UI 없으면 무시

        if (!isProblemTime)
        {
            // 솔루션 타임이면 숨김
            if (lastDisplayedSecond != -1) HideLastSecondsImmediate();
            lastDisplayedSecond = -1;
            return;
        }

        float remaining = Mathf.Clamp(time, 0f, problemTime);

        if (remaining > 0f && remaining <= lastSecondsDisplayThreshold)
        {
            // 예: remaining=2.9 -> Ceil -> 3
            int toShow = Mathf.CeilToInt(remaining);

            if (toShow != lastDisplayedSecond)
            {
                lastDisplayedSecond = toShow;
                ShowLastSecond(toShow);
            }
        }
        else
        {
            // 아직 threshold보다 많음 -> 숨김
            if (lastDisplayedSecond != -1) HideLastSecondsImmediate();
            lastDisplayedSecond = -1;
        }
    }

    void ShowLastSecond(int sec)
    {
        if (lastSecondsPanel != null) lastSecondsPanel.SetActive(true);
        if (lastSecondsText != null) lastSecondsText.text = sec.ToString();

        // 펄스 애니메이션 재생
        if (lastSecondsText != null)
        {
            if (pulseRoutine != null) StopCoroutine(pulseRoutine);
            pulseRoutine = StartCoroutine(PulseRoutine(lastSecondsText.rectTransform));
        }
    }

    void HideLastSecondsImmediate()
    {
        if (lastSecondsPanel != null) lastSecondsPanel.SetActive(false);
        if (lastSecondsText != null) lastSecondsText.text = "";
        if (pulseRoutine != null) { StopCoroutine(pulseRoutine); pulseRoutine = null; }
    }

    IEnumerator PulseRoutine(RectTransform rt)
    {
        if (rt == null) yield break;

        Vector3 original = rt.localScale;
        Vector3 target = original * pulseScale;

        // in
        float t = 0f;
        while (t < pulseInDuration)
        {
            t += Time.unscaledDeltaTime;
            rt.localScale = Vector3.Lerp(original, target, Mathf.SmoothStep(0f, 1f, t / pulseInDuration));
            yield return null;
        }
        rt.localScale = target;

        // out
        t = 0f;
        while (t < pulseOutDuration)
        {
            t += Time.unscaledDeltaTime;
            rt.localScale = Vector3.Lerp(target, original, Mathf.SmoothStep(0f, 1f, t / pulseOutDuration));
            yield return null;
        }
        rt.localScale = original;
        pulseRoutine = null;
    }

    // 외부에서 문제 진행중 남은 시간(초)을 얻는 용도
    // 만약 현재 솔루션 타임이면 0 반환
    public float GetRemainingProblemTime()
    {
        return isProblemTime ? Mathf.Max(0f, time) : 0f;
    }

    public void CancelTimer()
    {
        // 외부 호출(문제 정답/오답) 시 타이머 즉시 0으로 두어 솔루션 타임으로 넘어가도록 함
        time = 0f;

        // 힌트/카운트다운 숨김
        HideLastSecondsImmediate();
        lastDisplayedSecond = -1;
    }
}
