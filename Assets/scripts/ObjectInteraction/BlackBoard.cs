using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlackBoard : MonoBehaviour
{
    [SerializeField] TextMeshPro tmQuestion;
    [SerializeField] TextMeshPro tmAnswer1;
    [SerializeField] TextMeshPro tmAnswer2;
    [SerializeField] TextMeshPro tmAnswer3;
    [SerializeField] TextMeshPro tmAnswer4;
    [SerializeField] GameObject mark;

    TextMeshPro tmMark;
    private Question[] questions = new Question[4];

    private int questionIndex = 1;

    private string questionText;
    private string answer1;
    private string answer2;
    private string answer3;
    private string answer4;
    private string correctAnswer;

    void Awake()
    {
        ClickDelegate.Instance.OnClick += OnClick;
    }

    void Start()
    {

        questionText = "What is the world's highest mountain?";
        answer1 = "K2";
        answer2 = "Kilimanjaro";
        answer3 = "Mount Everest";
        answer4 = "Makalu";
        correctAnswer = answer3;
        questions[questionIndex] = new Question(questionText, answer1, answer2, answer3, answer4, correctAnswer);

        questionText = "Which of the following countries do not border France?";
        answer1 = "Netherlands";
        answer2 = "Germany";
        answer3 = "Spain";
        answer4 = "Italy";
        correctAnswer = answer1;
        questions[2] = new Question(questionText, answer1, answer2, answer3, answer4, correctAnswer);

        questionText = "Which is the longest river in the world?";
        answer1 = "Amazon River";
        answer2 = "Yellow River";
        answer3 = "Congo River";
        answer4 = "Nile River";
        correctAnswer = answer4;
        questions[3] = new Question(questionText, answer1, answer2, answer3, answer4, correctAnswer);


        tmQuestion.SetText(questions[questionIndex].questionText);
        tmAnswer1.SetText(questions[questionIndex].answer1);
        tmAnswer2.SetText(questions[questionIndex].answer2);
        tmAnswer3.SetText(questions[questionIndex].answer3);
        tmAnswer4.SetText(questions[questionIndex].answer4);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnClick(GameObject obj)
    {
        if (obj.tag == "BoardAnswer")
        {
            mark.gameObject.SetActive(true);
            tmMark = mark.GetComponent<TextMeshPro>();

            string playerAnswer = obj.GetComponent<TextMeshPro>().text;

            if (questionIndex < 3)
            {
                if (playerAnswer == questions[questionIndex].correctAnswer)
                {
                    tmMark.SetText("Correct! :D.");
                }
                else
                {
                    tmMark.SetText("Incorrect >:(.");
                }

                questionIndex++;

                tmQuestion.SetText(questions[questionIndex].questionText);
                tmAnswer1.SetText(questions[questionIndex].answer1);
                tmAnswer2.SetText(questions[questionIndex].answer2);
                tmAnswer3.SetText(questions[questionIndex].answer3);
                tmAnswer4.SetText(questions[questionIndex].answer4);
            }
            else
            {
                tmQuestion.enabled = false;
                tmAnswer1.enabled = false;
                tmAnswer2.enabled = false;
                tmAnswer3.enabled = false;
                tmAnswer4.enabled = false;

                tmMark.SetText("Goodbye Class!");
            }
        }
    }

    private class Question
    {
        public string questionText;
        public string answer1;
        public string answer2;
        public string answer3;
        public string answer4;
        public string correctAnswer;

        public Question(string questionText, string answer1, string answer2, string answer3, string answer4, string correctAnswer)
        {

            this.questionText = questionText;
            this.answer1 = answer1;
            this.answer2 = answer2;
            this.answer3 = answer3;
            this.answer4 = answer4;
            this.correctAnswer = correctAnswer;

        }
        private Question(string questionText)
        {
            this.questionText = questionText;
        }
    }
}
 
