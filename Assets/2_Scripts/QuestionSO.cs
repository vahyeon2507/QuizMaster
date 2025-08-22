using UnityEngine;

[CreateAssetMenu(menuName = "Quiz Qusetion", fileName = "New Qusetion")]
public class QuestionSO : ScriptableObject
{
    [TextArea(2, 6)] 
    [SerializeField] string question= "여기에 질문을 적어주세요.";
    [SerializeField] string[] answers  = new string[4];
    [SerializeField] int correctAnswerIndex;

    public string GetQuestion()
        {
        return question;
    }

    public string[] GetAnswers()
    {
        return answers;
    }

    public string GetCorrectAnswer()
    {
        return answers[correctAnswerIndex];
    }

    public int GetCorrectAnswerIndex()
    {
        return correctAnswerIndex;
    }

}
