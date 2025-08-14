using UnityEngine;

[CreateAssetMenu(menuName = "Quiz Qusetion", fileName = "New Qusetion")]
public class QuestionSO : ScriptableObject
{
    [TextArea(2, 6)] 
    [SerializeField] string question= "여기에 질문을 적어주세요.";
}
