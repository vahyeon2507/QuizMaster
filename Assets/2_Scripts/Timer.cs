using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] float problemTime = 10f; // 문제 푸는 시간
    [SerializeField] float solutionTime = 3f; // 정답, 오답 표기시간
    float time = 0f;

    [HideInInspector] public bool isProblemTime = true;
    [HideInInspector] public float fillAmount;
    private void Start()
    {
        time = problemTime;
    }
    private void Update()
    {
        TimerCountDown();
        UpdateFillAmount();

    }

    private void UpdateFillAmount()
    {
        if (isProblemTime)
            fillAmount = time / problemTime;
        else
            fillAmount = time / solutionTime;
    }

    private void TimerCountDown()
    {
        Debug.Log("Time remaining " + time);
        time -= Time.deltaTime;
        if (time <= 0)
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
            }
        }
    }
}


