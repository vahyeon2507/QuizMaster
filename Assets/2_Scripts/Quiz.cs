using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Quiz : MonoBehaviour
{
    [Header("질문")]
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] List<QuestionSO> questions = new List<QuestionSO>();
    QuestionSO currentQuestion;

    [Header("보기")]
    [SerializeField] GameObject[] answerButtons;

    [Header("버튼 색깔")]
    [SerializeField] Sprite defaultAnswerSprite;
    [SerializeField] Sprite correctAnswerSprite;

    [Header("Timer")]
    [SerializeField] UnityEngine.UI.Image timerImage;
    [SerializeField] Sprite problemTimerSprite;
    [SerializeField] Sprite solutionTimerSprite;
    Timer timer;
    bool chosenAnswer = false;

    [Header("점수")]
    [SerializeField] TextMeshProUGUI scoreText;
    ScoreKeeper scoreKeeper;

    // 로컬 점수 변수: 정답당 +5점
    private int totalScore = 0;
    private const int pointsPerCorrect = 5;

    // 힌트 사용 시 차감 값
    private const int pointsPerHint = 2;

    [Header("힌트 UI")]
    [SerializeField] TextMeshProUGUI hintTextUI;      // 힌트 텍스트 출력할 UI (인스펙터에 연결)
    [SerializeField] GameObject hintPanel;           // 힌트 전체 영역(있으면 숨김/표시로 사용)

    // 힌트 표시 제어
    private bool hintShownThisQuestion = false;

    [Header("ProgressBar")]
    [SerializeField] UnityEngine.UI.Slider progressBar;

    [SerializeField] ChatGPTClient chatGPTClient;
    [SerializeField] int qusetionCount = 3;
    [SerializeField] TextMeshProUGUI loadingText;

    bool isGenratingQuestions = false;

    void Start()
    {
        timer = FindFirstObjectByType<Timer>();
        scoreKeeper = FindFirstObjectByType<ScoreKeeper>();
        chatGPTClient.quizGenerateHandler += QuizGenratedHandler;

        // 초기 점수 표시
        totalScore = 0;
        UpdateScoreText();

        // 힌트 패널 초기 상태 숨김
        if (hintPanel != null) hintPanel.SetActive(false);
        else if (hintTextUI != null) hintTextUI.text = "";

        if (questions.Count == 0)
        {
            GeneratQuestionslfNeded();
        }
        else
        {
            intalizeProgressBar();
        }
    }

    private void GeneratQuestionslfNeded()
    {
        if (isGenratingQuestions) return;

        isGenratingQuestions = true;
        GameManager.Instance.ShowLoadingScreen();

        string topicToUse = GetTredingTopic();
        chatGPTClient.GenerateQuizQuestions(qusetionCount, topicToUse);
        Debug.Log($"GenratQuestionsIfNeeded {topicToUse}");
    }

    private string GetTredingTopic()
    {
        string[] topics = new string[] { "리그 오브 레전드", "마스터 이", "게임", "블루 아카이브", "림버스 컴퍼니", "트릭컬 리바이브", "원신", "핸리 스틱민 컬랙션", "팀 포트리스 2", "컴퓨터 과학" };
        int randomIndex = UnityEngine.Random.Range(0, topics.Length);
        return topics[randomIndex];
    }

    void QuizGenratedHandler(List<QuestionSO> generateQuestions)
    {
        Debug.Log($"QuizGenratedHandler {generateQuestions?.Count ?? 0} questions received");
        isGenratingQuestions = false;

        if (generateQuestions is null || generateQuestions.Count == 0)
        {
            Debug.LogError("문제 생성에 실패했습니다.");
            loadingText.text = "문제 생성에 실패했습니다.\n인터넷 연결을 확인하고 다시 시도하세요.";
            return;
        }

        // 간단 안전 검사: 힌트가 비어있으면 경고 로그 (그러나 진행은 함)
        foreach (var q in generateQuestions)
        {
            if (string.IsNullOrEmpty(q.GetHint()))
            {
                Debug.LogWarning($"Question '{q.GetQuestion()}' has no hint. Ensure ChatGPTClient supplies hints.");
            }
        }

        questions.AddRange(generateQuestions);
        progressBar.maxValue += generateQuestions.Count;

        GetNextQuestion();
    }

    private void intalizeProgressBar()
    {
        progressBar.maxValue = questions.Count;
        progressBar.value = 0;
    }

    private void Update()
    {
        if (timer != null)
        {
            timerImage.fillAmount = timer.fillAmount;
            if (timer.isProblemTime)
                timerImage.sprite = problemTimerSprite;
            else
                timerImage.sprite = solutionTimerSprite;
        }

        // 다음 질문 불러오기
        if (timer != null && timer.loadNextQuestion)
        {
            if (questions.Count == 0)
            {
                GeneratQuestionslfNeded();
            }
            else
            {
                GetNextQuestion();
            }
        }

        // 솔루션 타임이고 답을 선택하지 않았을 때
        if (timer != null && !timer.isProblemTime && !chosenAnswer)
        {
            DisplaySelection(-1); // -1은 틀린 답을 의미
        }
    }

    private void GetNextQuestion()
    {
        if (questions.Count <= 0)
        {
            return;
        }

        timer.loadNextQuestion = false;
        GameManager.Instance.ShowQuizSceen();
        chosenAnswer = false;
        SetButtonState(true);
        SetDefaultButtonSprites();
        GetRendomQuestion();
        OndisplayQustion();
        scoreKeeper.IncrementQuestionSeen();
        progressBar.value++;

        // 새로운 문제 들어올 때 힌트 리셋
        hintShownThisQuestion = false;
        if (hintPanel != null) hintPanel.SetActive(false);
        if (hintTextUI != null) hintTextUI.text = "";
    }

    private void GetRendomQuestion()
    {
        int randomindex = UnityEngine.Random.Range(0, questions.Count);
        currentQuestion = questions[randomindex];
        questions.RemoveAt(randomindex); // 중복 출제 방지
    }

    private void OndisplayQustion()
    {
        if (currentQuestion == null) return;

        questionText.text = currentQuestion.GetQuestion();

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.GetAnswer(i);
        }

        // 기본적으로 힌트는 숨김
        if (hintPanel != null) hintPanel.SetActive(false);
        if (hintTextUI != null) hintTextUI.text = "";
    }

    // 버튼 클릭 (정답/오답 처리)
    public void OnAnswerButtonClicked(int index)
    {
        chosenAnswer = true;

        // 1) 버튼 플래시 (정답 여부 전달)
        var flash = answerButtons[index].GetComponent<ButtonClickFlash>();
        if (flash != null)
        {
            bool isCorrect = (index == currentQuestion.GetCorrectAnswerIndex());
            flash.PlayFlash(isCorrect); // 색: 정답은 초록, 오답은 빨강
        }

        // 2) 기존 동작
        DisplaySelection(index);
        timer.CancelTimer();
        UpdateScoreText();
    }

    private void DisplaySelection(int index)
    {
        if (currentQuestion == null) return;

        if (index == currentQuestion.GetCorrectAnswerIndex())
        {
            // 정답 처리: 남은 시간에 따라 배수 계산
            float remaining = timer.GetRemainingProblemTime(); // 예: 7.2 초 남음
            float elapsed = timer.ProblemTime - remaining;     // 예: 2.8 초 경과

            int multiplier = 1;
            if (elapsed <= 3f) multiplier = 3;      // 3초 이내 정답 -> 3배
            else if (elapsed <= 6f) multiplier = 2; // 6초 이내 정답 -> 2배

            int pointsToAdd = pointsPerCorrect * multiplier;

            questionText.text = $"정답입니다! (+{pointsToAdd}점)";
            answerButtons[index].GetComponent<UnityEngine.UI.Image>().sprite = correctAnswerSprite;
            scoreKeeper.IncrementCorrectAnswers();

            // 정답당 점수 추가 (배수 적용)
            totalScore += pointsToAdd;
            UpdateScoreText();
        }
        else
        {
            questionText.text = "틀렸습니다." + currentQuestion.GetCorrectAnswer();
            // 틀렸을 때는 점수 차감 없음
        }
        SetButtonState(false);
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score : {totalScore}";
        }
    }

    // 힌트 보여주기 (수동 버튼으로만 호출)
    public void ShowHint()
    {
        // 힌트는 '풀이 중'일 때만 허용: 현재 문제 없거나 이미 정답 처리했거나 이미 힌트를 본 상태면 리턴
        if (currentQuestion == null) return;
        if (hintShownThisQuestion) return;
        if (chosenAnswer) return;

        // GPT가 생성해둔 힌트 호출
        string h = currentQuestion.GetHint();
        if (string.IsNullOrEmpty(h))
        {
            h = "힌트가 준비되어 있지 않습니다.";
        }

        // 점수 차감: -2 (단, 0 미만으로 떨어지지 않음)
        totalScore -= pointsPerHint;
        if (totalScore < 0) totalScore = 0;
        UpdateScoreText();

        if (hintTextUI != null) hintTextUI.text = $"힌트: {h}";
        if (hintPanel != null) hintPanel.SetActive(true);

        hintShownThisQuestion = true;
    }

    // 힌트 버튼에 연결
    public void OnHintButtonClicked()
    {
        ShowHint();
    }

    private void SetDefaultButtonSprites()
    {
        foreach (GameObject obj in answerButtons)
        {
            obj.GetComponent<Image>().sprite = defaultAnswerSprite;
        }
    }

    private void SetButtonState(bool state)
    {
        foreach (GameObject obj in answerButtons)
        {
            obj.GetComponent<Button>().interactable = state;
        }
    }
}
