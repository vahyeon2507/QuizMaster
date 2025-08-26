using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class Quiz : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] QuestionSO question;
    [SerializeField] GameObject[] answerButtons;
    [SerializeField] Sprite defaultAnswerSprite;
    [SerializeField] Sprite correctAnswerSprite;
    void Start()
    {
        GetNextQuestion();
    }

    private void GetNextQuestion()
    {
        SetButtonState(true);
        SetDefaultButtonSprites();
        OndisplayQustion();
    }

    

    private void OndisplayQustion()
    {
        questionText.text = question.GetQuestion();

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = question.GetAnswer(i);
        }
    }

    public void OnAnswerButtonClicked(int index)
    {
        if (index == question.GetCorrectAnswerIndex())
        {
            questionText.text = "정답입니다!";
            answerButtons[index].GetComponent<UnityEngine.UI.Image>().sprite = correctAnswerSprite; 
        }
        else
        {
            questionText.text = "틀렸습니다." + question.GetCorrectAnswer();
        }
        SetButtonState(false);
    }

    private void SetDefaultButtonSprites()
    {
        foreach (GameObject obj in answerButtons)
        {
            obj.GetComponent<UnityEngine.UI.Image>().sprite = defaultAnswerSprite;
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


