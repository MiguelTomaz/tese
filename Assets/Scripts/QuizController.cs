using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class QuizController : MonoBehaviour
{
    public Button CreateQuizButton;
    private string quizUrl = "http://localhost:3000/api/quiz/";
    private string questionUrl = "http://localhost:3000/api/questions/";


    public GameObject quizPanel;
    public GameObject quizPortoPanel;
    public Text questionNumber;
    public Text difficulty;
    public Text question;
    public Text answer1;
    public Text answer2;
    public Text answer3;
    public Text correctAnswer;

    public Button answer1Button;
    public Button answer2Button;
    public Button answer3Button;
    public Button answer4Button;
    public int totalScore = 0;

    // Start is called before the first frame update
    void Start()
    {
        CreateQuizButton.onClick.AddListener(CreateQuiz);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public class QuizCreationResponse
    {
        public string message;
        public string error;
        public int quizId;
    }

    [System.Serializable]
    public class Question
    {
        public int id;
        public string question_en;
        public string question_pt;
        public string answer1_en;
        public string answer2_en;
        public string answer3_en;
        public string correct_answer_en;
        public string answer1_pt;
        public string answer2_pt;
        public string answer3_pt;
        public string correct_answer_pt;
        public int score;
        public int difficulty;
    }

    [System.Serializable]
    public class QuestionsId
    {
        public int question1Id;
        public int question2Id;
        public int question3Id;
        public int question4Id;
    }

    [System.Serializable]
    private class QuestionListWrapper
    {
        public List<Question> questions;
    }

    public void CreateQuiz()
    {
        int touristId = PlayerPrefs.GetInt("Current_Logged_TouristID", -1); // -1 é o valor padrão se a chave "UserID" não existir

        StartCoroutine(SendCreateQuizRequest(touristId));
    }

    private IEnumerator SendCreateQuizRequest(int touristId)
    {
        // Monta a URL com o id do turista
        string url = quizUrl + "create/" + touristId;

        // Cria a solicitação HTTP POST
        UnityWebRequest request = UnityWebRequest.Post(url, "");

        // Envia a solicitação
        yield return request.SendWebRequest();

        // Verifica se ocorreu algum erro na solicitação
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to create quiz: " + request.error);
        }
        else
        {
            QuizCreationResponse response = JsonUtility.FromJson<QuizCreationResponse>(request.downloadHandler.text);

            if (!string.IsNullOrEmpty(response.error))
            {
                Debug.LogError("Failed to create quiz: " + response.error);
            }
            else
            {
                Debug.Log("Quiz created successfully with id: " + response.quizId);

                StartCoroutine(GetQuizQuestions(response.quizId));
            }
        }
    }

    private IEnumerator GetQuizQuestions(int quizId)
    {
        // Monta a URL com o id do quiz
        string url = quizUrl + "questions/" + quizId;

        // Cria a solicitação HTTP GET
        UnityWebRequest request = UnityWebRequest.Get(url);

        // Envia a solicitação
        yield return request.SendWebRequest();

        // Verifica se ocorreu algum erro na solicitação
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to fetch questions: " + request.error);
        }
        else
        {
            Debug.Log("get questions from the quiz");
            // Desserializa a resposta da solicitação para uma lista de perguntas
            string jsonResponse = request.downloadHandler.text;
            List<Question> questions = JsonUtility.FromJson<QuestionListWrapper>("{\"questions\":" + jsonResponse + "}").questions;

            // Faça algo com as perguntas, como exibir ou processar
            foreach (Question question in questions)
            {
                Debug.Log("Question ID: " + question.id + ", Question: " + question.question_en);
            }

            QuestionsId questionsId = new QuestionsId();
            questionsId.question1Id = questions.Count > 0 ? questions[0].id : -1;
            questionsId.question2Id = questions.Count > 1 ? questions[1].id : -1;
            questionsId.question3Id = questions.Count > 2 ? questions[2].id : -1;
            questionsId.question4Id = questions.Count > 3 ? questions[3].id : -1;


            string savedLanguage = PlayerPrefs.GetString("Language", "en");
            Debug.Log("savedLanguage: " + savedLanguage);
            Question firstQuestion = questions[0];

            if (savedLanguage == "en")
            {
                Debug.Log("O idioma recuperado é ingles.");
                question.text = firstQuestion.question_en;
                answer1.text = firstQuestion.answer1_en;
                answer2.text = firstQuestion.answer2_en;
                answer3.text = firstQuestion.answer3_en;
                correctAnswer.text = firstQuestion.correct_answer_en;
                questionNumber.text = 1 + "";
                if (firstQuestion.difficulty == 1)
                {
                    difficulty.text = "easy";
                }
                else if (firstQuestion.difficulty == 2)
                {
                    difficulty.text = "normal";
                }
                else
                {
                    difficulty.text = "hard";
                }
            }
            else
            {
                // Faça algo se o idioma for português
                Debug.Log("O idioma recuperado é português.");

                question.text = firstQuestion.question_pt;
                answer1.text = firstQuestion.answer1_pt;
                answer2.text = firstQuestion.answer2_pt;
                answer3.text = firstQuestion.answer3_pt;
                correctAnswer.text = firstQuestion.correct_answer_pt;
                questionNumber.text = 1 + "";
                if (firstQuestion.difficulty == 1)
                {
                    difficulty.text = "fácil";
                }
                else if (firstQuestion.difficulty == 2)
                {
                    difficulty.text = "normal";
                }
                else
                {
                    difficulty.text = "difícil";
                }
            }

            Debug.Log("botoes");
            answer1Button.onClick.AddListener(() => StartCoroutine(GetQuestion2(questions[1].id)));
            answer2Button.onClick.AddListener(() => StartCoroutine(GetQuestion2(questions[1].id)));
            answer3Button.onClick.AddListener(() => StartCoroutine(GetQuestion2(questions[1].id)));
            answer4Button.onClick.AddListener(() => StartCoroutine(GetQuestion2(questions[1].id, firstQuestion.score)));

            quizPortoPanel.SetActive(true);
        }
    }

    private IEnumerator GetQuestion2(int questionId, int score = 0)
    {
        totalScore += score;
        Debug.Log("get question 2");
        // Monta a URL com o ID da pergunta
        string url = questionUrl + questionId;

        // Cria a solicitação HTTP GET
        UnityWebRequest request = UnityWebRequest.Get(url);

        // Envia a solicitação
        yield return request.SendWebRequest();

        // Verifica se ocorreu algum erro na solicitação
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to fetch question: " + request.error);
        }
        else
        {
            // Parse a resposta JSON para a classe Question
            Question question2 = JsonUtility.FromJson<Question>(request.downloadHandler.text);

            // Faça algo com a pergunta, como exibir na tela
            Debug.Log("Question ID: " + question2.id + ", Question: " + question2.question_en);

            string savedLanguage = PlayerPrefs.GetString("Language", "en");
            Debug.Log("savedLanguage: " + savedLanguage);

            if (savedLanguage == "en")
            {
                Debug.Log("O idioma recuperado é ingles.");
                question.text = question2.question_en;
                answer1.text = question2.answer1_en;
                answer2.text = question2.answer2_en;
                answer3.text = question2.answer3_en;
                correctAnswer.text = question2.correct_answer_en;
                questionNumber.text = 2 + "";
                if (question2.difficulty == 1)
                {
                    difficulty.text = "easy";
                }
                else if (question2.difficulty == 2)
                {
                    difficulty.text = "normal";
                }
                else
                {
                    difficulty.text = "hard";
                }
            }
            else
            {
                // Faça algo se o idioma for português
                Debug.Log("O idioma recuperado é português.");

                question.text = question2.question_pt;
                answer1.text = question2.answer1_pt;
                answer2.text = question2.answer2_pt;
                answer3.text = question2.answer3_pt;
                correctAnswer.text = question2.correct_answer_pt;
                questionNumber.text = 1 + "";
                if (question2.difficulty == 1)
                {
                    difficulty.text = "fácil";
                }
                else if (question2.difficulty == 2)
                {
                    difficulty.text = "normal";
                }
                else
                {
                    difficulty.text = "difícil";
                }

                //answer1Button.onClick.AddListener(() => StartCoroutine(GetQuestion3(question2.id)));
                //answer2Button.onClick.AddListener(() => StartCoroutine(GetQuestion3(question2.id)));
                //answer3Button.onClick.AddListener(() => StartCoroutine(GetQuestion3(question2.id)));
                //answer4Button.onClick.AddListener(() => StartCoroutine(GetQuestion3(questions[1].id, firstQuestion.score)));
            }
        }
    }
}
