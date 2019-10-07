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
    [SerializeField] int questionsWrongAllowed;

    [HideInInspector] TextMeshPro[] answerObjects;

    Color32 unselected = new Color32(255, 255, 255, 255);
    Color32 highlighted = new Color32(102, 152, 88, 255);

    TextMeshPro tmMark;
    private Question[] questions = new Question[6];

    private int questionIndex = 0;

    private int questionsWrong = 0;

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
        questions[1] = new Question(questionText, answer1, answer2, answer3, answer4, correctAnswer);

        questionText = "Which is the longest river in the world?";
        answer1 = "Amazon River";
        answer2 = "Yellow River";
        answer3 = "Congo River";
        answer4 = "Nile River";
        correctAnswer = answer4;
        questions[2] = new Question(questionText, answer1, answer2, answer3, answer4, correctAnswer);

        questionText = "What's the term for comparing one thing to another?";
        answer1 = "Synonym";
        answer2 = "Antonym";
        answer3 = "Simile";
        answer4 = "Alliteration";
        correctAnswer = answer3;
        questions[3] = new Question(questionText, answer1, answer2, answer3, answer4, correctAnswer);

        questionText = "In which continent are the Atlas mountains?";
        answer1 = "Africa";
        answer2 = "South America";
        answer3 = "North America";
        answer4 = "Asia";
        correctAnswer = answer1;
        questions[4] = new Question(questionText, answer1, answer2, answer3, answer4, correctAnswer);

        questionText = "Which types of buildings did Henry VIII destroy?";
        answer1 = "Monasteries";
        answer2 = "Palaces";
        answer3 = "Castles";
        answer4 = "Prisons";
        correctAnswer = answer1;
        questions[5] = new Question(questionText, answer1, answer2, answer3, answer4, correctAnswer);


        tmQuestion.SetText(questions[questionIndex].questionText);
        tmAnswer1.SetText(questions[questionIndex].answer1);
        tmAnswer2.SetText(questions[questionIndex].answer2);
        tmAnswer3.SetText(questions[questionIndex].answer3);
        tmAnswer4.SetText(questions[questionIndex].answer4);

        answerObjects = new TextMeshPro[] { tmAnswer1, tmAnswer2, tmAnswer3, tmAnswer4 };
    }

    // Update is called once per frame
    void Update()
    {

        foreach (var answer in answerObjects)
        {

            var answerColour = answer.GetComponent<TextMeshPro>(); 

            if (answerColour.color == highlighted)
            {
                answerColour.color = unselected;
            }
        }

        if (Camera.main &&
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit) &&
                string.Equals(hit.transform.gameObject.tag, "BoardAnswer"))
        {
            var hitKey = hit.transform.gameObject;

            hitKey.GetComponent<TextMeshPro>().color = highlighted;

        } 
    }

    void OnClick(GameObject obj)
    {
        if (obj.tag == "BoardAnswer")
        {
            mark.gameObject.SetActive(true);
            tmMark = mark.GetComponent<TextMeshPro>();

            string playerAnswer = obj.GetComponent<TextMeshPro>().text;

            if (questionIndex < 5)
            {
                if (playerAnswer == questions[questionIndex].correctAnswer)
                {
                    tmMark.SetText("Correct!");
                    Debug.Log(questionsWrong);
                }
                else
                {
                    tmMark.SetText("Incorrect.");
                    questionsWrong++;
                    Debug.Log(questionsWrong);
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

                if (questionsWrong >= questionsWrongAllowed)
                {
                    questionIndex = 0;
                    questionsWrong = 0;
                    tmQuestion.SetText(questions[questionIndex].questionText);
                    tmAnswer1.SetText(questions[questionIndex].answer1);
                    tmAnswer2.SetText(questions[questionIndex].answer2);
                    tmAnswer3.SetText(questions[questionIndex].answer3);
                    tmAnswer4.SetText(questions[questionIndex].answer4);

                    tmMark.SetText("Try Again.");
                } else
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
 
