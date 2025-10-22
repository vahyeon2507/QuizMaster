using UnityEngine;

[CreateAssetMenu(menuName = "Quiz Question", fileName = "New Question")]
public class QuestionSO : ScriptableObject
{
    [TextArea(2, 6)]
    [SerializeField] string question = "여기에 질문을 적어주세요.";
    [SerializeField] string[] answers = new string[4];
    [SerializeField] int correctAnswerIndex = 0;

    // 힌트는 문자열로 저장 (씬의 UI를 참조하지 않음)
    [TextArea(1, 3)]
    [SerializeField] string hint = "";

    public string GetQuestion()
    {
        return question;
    }

    public string GetAnswer(int i)
    {
        return answers[i];
    }

    public string GetCorrectAnswer()
    {
        return answers[correctAnswerIndex];
    }

    public int GetCorrectAnswerIndex()
    {
        return correctAnswerIndex;
    }

    public string GetHint()
    {
        return hint;
    }

    public void SetData(string q, string[] a, int correctIndex, string h)
    {
        SetData(q, a, correctIndex);
        hint = h;
    }

    public void SetData(string q, string[] a, int correctIndex)
    {
        question = q;
        answers = a;
        correctAnswerIndex = correctIndex;
    }
}
