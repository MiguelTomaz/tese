using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class QuizController : MonoBehaviour
{
    public Button CreateQuizButton;
    private string quizUrl = "http://13.60.19.19:3000/api/quiz/";
    private string questionUrl = "http://13.60.19.19:3000/api/questions/";


    public GameObject quizPanel;
    public GameObject quizPortoPanel;
    public GameObject endQuizQuestionsContainer;
    public GameObject endQuizPanel;

    public Text questionNumber;
    public Text difficulty;
    public Text question;
    public Text answer1;
    public Text answer2;
    public Text answer3;
    public Text correctAnswer;
    public Text finalScore;

    public Button answer1Button;
    public Button answer2Button;
    public Button answer3Button;
    public Button answer4Button;
    public int totalScore = 0;

    private bool isGetQuestion2 = true;
    private bool listenersRemoved = false;
    private int quizId;
    // Start is called before the first frame update
    void Start()
    {
        CreateQuizButton.onClick.AddListener(CreateQuiz);
    }

    // Update is called once per frame
    void Update()
    {
        /**
        if(isGetQuestion2 == false && !listenersRemoved)
        {
            Debug.Log("remover listeners dos botoes");
            answer1Button.onClick.RemoveAllListeners();
            answer2Button.onClick.RemoveAllListeners();
            answer3Button.onClick.RemoveAllListeners();
            answer4Button.onClick.RemoveAllListeners();
            listenersRemoved = true;
        }
        */
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
                quizId = response.quizId;
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

            answer1Button.onClick.AddListener(() => {
                if (isGetQuestion2)
                {
                    StartCoroutine(GetQuestion2(questions[1].id, questions));
                }
                else
                {
                    Debug.Log("nao devia estar aqui");
                }
            });

            answer2Button.onClick.AddListener(() => {
                if (isGetQuestion2)
                {
                    StartCoroutine(GetQuestion2(questions[1].id, questions));
                }
                else
                {
                    Debug.Log("nao devia estar aqui");
                }
            });

            answer3Button.onClick.AddListener(() => {
                if (isGetQuestion2)
                {
                    StartCoroutine(GetQuestion2(questions[1].id, questions));
                }
                else
                {
                    Debug.Log("nao devia estar aqui");
                }
            });

            answer4Button.onClick.AddListener(() => {
                if (isGetQuestion2)
                {
                    StartCoroutine(GetQuestion2(questions[1].id, questions, firstQuestion.score));
                }
                else
                {
                    Debug.Log("nao devia estar aqui");
                }
            });

            quizPortoPanel.SetActive(true);
        }
    }

    private IEnumerator GetQuestion2(int questionId, List<Question> questions, int score = 0)
    {
        isGetQuestion2 = false; 
        totalScore += score;
        Debug.Log("get question 2: " + isGetQuestion2);
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
            }
            /**
            if (listenersRemoved)
            {
                answer1Button.onClick.AddListener(() => StartCoroutine(GetToQuizEnd(questions, 0)));
                answer2Button.onClick.AddListener(() => StartCoroutine(GetToQuizEnd(questions, 0)));
                answer3Button.onClick.AddListener(() => StartCoroutine(GetToQuizEnd(questions, 0)));
                answer4Button.onClick.AddListener(() => StartCoroutine(GetToQuizEnd(questions, question2.score)));
            }
            else
            {
                Debug.Log("chegou ao fim mas nao deu para colocar eventos nso botoes");
            }
            */

            answer1Button.onClick.RemoveAllListeners();
            answer2Button.onClick.RemoveAllListeners();
            answer3Button.onClick.RemoveAllListeners();
            answer4Button.onClick.RemoveAllListeners();
            /**
            answer1Button.onClick.AddListener(() => StartCoroutine(GetToQuizEnd(questions, 0)));
            answer2Button.onClick.AddListener(() => StartCoroutine(GetToQuizEnd(questions, 0)));
            answer3Button.onClick.AddListener(() => StartCoroutine(GetToQuizEnd(questions, 0)));
            answer4Button.onClick.AddListener(() => StartCoroutine(GetToQuizEnd(questions, question2.score)));
            */
            answer1Button.onClick.AddListener(() => StartCoroutine(GetQuestion3(questions[2].id, questions)));
            answer2Button.onClick.AddListener(() => StartCoroutine(GetQuestion3(questions[2].id, questions)));
            answer3Button.onClick.AddListener(() => StartCoroutine(GetQuestion3(questions[2].id, questions)));
            answer4Button.onClick.AddListener(() => StartCoroutine(GetQuestion3(questions[2].id, questions, question2.score)));
        }
    }

    private IEnumerator GetQuestion3(int questionId, List<Question> questions, int score = 0)
    {
       
        totalScore += score;
        Debug.Log("get question 3: " + totalScore);
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
            Question question3 = JsonUtility.FromJson<Question>(request.downloadHandler.text);

            // Faça algo com a pergunta, como exibir na tela
            Debug.Log("Question ID: " + question3.id + ", Question: " + question3.question_en);

            string savedLanguage = PlayerPrefs.GetString("Language", "en");
            Debug.Log("savedLanguage: " + savedLanguage);

            if (savedLanguage == "en")
            {
                Debug.Log("O idioma recuperado é ingles.");
                question.text = question3.question_en;
                answer1.text = question3.answer1_en;
                answer2.text = question3.answer2_en;
                answer3.text = question3.answer3_en;
                correctAnswer.text = question3.correct_answer_en;
                questionNumber.text = 2 + "";
                if (question3.difficulty == 1)
                {
                    difficulty.text = "easy";
                }
                else if (question3.difficulty == 2)
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

                question.text = question3.question_pt;
                answer1.text = question3.answer1_pt;
                answer2.text = question3.answer2_pt;
                answer3.text = question3.answer3_pt;
                correctAnswer.text = question3.correct_answer_pt;
                questionNumber.text = 1 + "";
                if (question3.difficulty == 1)
                {
                    difficulty.text = "fácil";
                }
                else if (question3.difficulty == 2)
                {
                    difficulty.text = "normal";
                }
                else
                {
                    difficulty.text = "difícil";
                }
            }
            
            
            answer1Button.onClick.RemoveAllListeners();
            answer2Button.onClick.RemoveAllListeners();
            answer3Button.onClick.RemoveAllListeners();
            answer4Button.onClick.RemoveAllListeners();

            answer1Button.onClick.AddListener(() => StartCoroutine(GetQuestion4(questions[3].id, questions)));
            answer2Button.onClick.AddListener(() => StartCoroutine(GetQuestion4(questions[3].id, questions)));
            answer3Button.onClick.AddListener(() => StartCoroutine(GetQuestion4(questions[3].id, questions)));
            answer4Button.onClick.AddListener(() => StartCoroutine(GetQuestion4(questions[3].id, questions, question3.score)));

        }
    }

    private IEnumerator GetQuestion4(int questionId, List<Question> questions, int score = 0)
    {

        totalScore += score;
        Debug.Log("get question 4: " + totalScore);
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
            Question question4 = JsonUtility.FromJson<Question>(request.downloadHandler.text);

            // Faça algo com a pergunta, como exibir na tela
            Debug.Log("Question ID: " + question4.id + ", Question: " + question4.question_en);

            string savedLanguage = PlayerPrefs.GetString("Language", "en");
            Debug.Log("savedLanguage: " + savedLanguage);

            if (savedLanguage == "en")
            {
                Debug.Log("O idioma recuperado é ingles.");
                question.text = question4.question_en;
                answer1.text = question4.answer1_en;
                answer2.text = question4.answer2_en;
                answer3.text = question4.answer3_en;
                correctAnswer.text = question4.correct_answer_en;
                questionNumber.text = 2 + "";
                if (question4.difficulty == 1)
                {
                    difficulty.text = "easy";
                }
                else if (question4.difficulty == 2)
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

                question.text = question4.question_pt;
                answer1.text = question4.answer1_pt;
                answer2.text = question4.answer2_pt;
                answer3.text = question4.answer3_pt;
                correctAnswer.text = question4.correct_answer_pt;
                questionNumber.text = 1 + "";
                if (question4.difficulty == 1)
                {
                    difficulty.text = "fácil";
                }
                else if (question4.difficulty == 2)
                {
                    difficulty.text = "normal";
                }
                else
                {
                    difficulty.text = "difícil";
                }
            }
            answer1Button.onClick.RemoveAllListeners();
            answer2Button.onClick.RemoveAllListeners();
            answer3Button.onClick.RemoveAllListeners();
            answer4Button.onClick.RemoveAllListeners();

            answer1Button.onClick.AddListener(() => StartCoroutine(GetQuestion5(questions[4].id, questions)));
            answer2Button.onClick.AddListener(() => StartCoroutine(GetQuestion5(questions[4].id, questions)));
            answer3Button.onClick.AddListener(() => StartCoroutine(GetQuestion5(questions[4].id, questions)));
            answer4Button.onClick.AddListener(() => StartCoroutine(GetQuestion5(questions[4].id, questions, question4.score)));
        }
    }

    private IEnumerator GetQuestion5(int questionId, List<Question> questions, int score = 0)
    {

        totalScore += score;
        Debug.Log("get question 5: " + totalScore);
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
            Question question5 = JsonUtility.FromJson<Question>(request.downloadHandler.text);

            // Faça algo com a pergunta, como exibir na tela
            Debug.Log("Question ID: " + question5.id + ", Question: " + question5.question_en);

            string savedLanguage = PlayerPrefs.GetString("Language", "en");
            Debug.Log("savedLanguage: " + savedLanguage);

            if (savedLanguage == "en")
            {
                Debug.Log("O idioma recuperado é ingles.");
                question.text = question5.question_en;
                answer1.text = question5.answer1_en;
                answer2.text = question5.answer2_en;
                answer3.text = question5.answer3_en;
                correctAnswer.text = question5.correct_answer_en;
                questionNumber.text = 2 + "";
                if (question5.difficulty == 1)
                {
                    difficulty.text = "easy";
                }
                else if (question5.difficulty == 2)
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

                question.text = question5.question_pt;
                answer1.text = question5.answer1_pt;
                answer2.text = question5.answer2_pt;
                answer3.text = question5.answer3_pt;
                correctAnswer.text = question5.correct_answer_pt;
                questionNumber.text = 1 + "";
                if (question5.difficulty == 1)
                {
                    difficulty.text = "fácil";
                }
                else if (question5.difficulty == 2)
                {
                    difficulty.text = "normal";
                }
                else
                {
                    difficulty.text = "difícil";
                }
            }
            answer1Button.onClick.RemoveAllListeners();
            answer2Button.onClick.RemoveAllListeners();
            answer3Button.onClick.RemoveAllListeners();
            answer4Button.onClick.RemoveAllListeners();

            answer1Button.onClick.AddListener(() => StartCoroutine(GetQuestion6(questions[5].id, questions)));
            answer2Button.onClick.AddListener(() => StartCoroutine(GetQuestion6(questions[5].id, questions)));
            answer3Button.onClick.AddListener(() => StartCoroutine(GetQuestion6(questions[5].id, questions)));
            answer4Button.onClick.AddListener(() => StartCoroutine(GetQuestion6(questions[5].id, questions, question5.score)));
        }
    }

    private IEnumerator GetQuestion6(int questionId, List<Question> questions, int score = 0)
    {

        totalScore += score;
        Debug.Log("get question 6: " + totalScore);
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
            Question question6 = JsonUtility.FromJson<Question>(request.downloadHandler.text);

            // Faça algo com a pergunta, como exibir na tela
            Debug.Log("Question ID: " + question6.id + ", Question: " + question6.question_en);

            string savedLanguage = PlayerPrefs.GetString("Language", "en");
            Debug.Log("savedLanguage: " + savedLanguage);

            if (savedLanguage == "en")
            {
                Debug.Log("O idioma recuperado é ingles.");
                question.text = question6.question_en;
                answer1.text = question6.answer1_en;
                answer2.text = question6.answer2_en;
                answer3.text = question6.answer3_en;
                correctAnswer.text = question6.correct_answer_en;
                questionNumber.text = 2 + "";
                if (question6.difficulty == 1)
                {
                    difficulty.text = "easy";
                }
                else if (question6.difficulty == 2)
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

                question.text = question6.question_pt;
                answer1.text = question6.answer1_pt;
                answer2.text = question6.answer2_pt;
                answer3.text = question6.answer3_pt;
                correctAnswer.text = question6.correct_answer_pt;
                questionNumber.text = 1 + "";
                if (question6.difficulty == 1)
                {
                    difficulty.text = "fácil";
                }
                else if (question6.difficulty == 2)
                {
                    difficulty.text = "normal";
                }
                else
                {
                    difficulty.text = "difícil";
                }
            }
            answer1Button.onClick.RemoveAllListeners();
            answer2Button.onClick.RemoveAllListeners();
            answer3Button.onClick.RemoveAllListeners();
            answer4Button.onClick.RemoveAllListeners();

            answer1Button.onClick.AddListener(() => StartCoroutine(GetQuestion7(questions[6].id, questions)));
            answer2Button.onClick.AddListener(() => StartCoroutine(GetQuestion7(questions[6].id, questions)));
            answer3Button.onClick.AddListener(() => StartCoroutine(GetQuestion7(questions[6].id, questions)));
            answer4Button.onClick.AddListener(() => StartCoroutine(GetQuestion7(questions[6].id, questions, question6.score)));
        }
    }

    private IEnumerator GetQuestion7(int questionId, List<Question> questions, int score = 0)
    {

        totalScore += score;
        Debug.Log("get question 7: " + totalScore);
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
            Question question7 = JsonUtility.FromJson<Question>(request.downloadHandler.text);

            // Faça algo com a pergunta, como exibir na tela
            Debug.Log("Question ID: " + question7.id + ", Question: " + question7.question_en);

            string savedLanguage = PlayerPrefs.GetString("Language", "en");
            Debug.Log("savedLanguage: " + savedLanguage);

            if (savedLanguage == "en")
            {
                Debug.Log("O idioma recuperado é ingles.");
                question.text = question7.question_en;
                answer1.text = question7.answer1_en;
                answer2.text = question7.answer2_en;
                answer3.text = question7.answer3_en;
                correctAnswer.text = question7.correct_answer_en;
                questionNumber.text = 2 + "";
                if (question7.difficulty == 1)
                {
                    difficulty.text = "easy";
                }
                else if (question7.difficulty == 2)
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

                question.text = question7.question_pt;
                answer1.text = question7.answer1_pt;
                answer2.text = question7.answer2_pt;
                answer3.text = question7.answer3_pt;
                correctAnswer.text = question7.correct_answer_pt;
                questionNumber.text = 1 + "";
                if (question7.difficulty == 1)
                {
                    difficulty.text = "fácil";
                }
                else if (question7.difficulty == 2)
                {
                    difficulty.text = "normal";
                }
                else
                {
                    difficulty.text = "difícil";
                }
            }
            answer1Button.onClick.RemoveAllListeners();
            answer2Button.onClick.RemoveAllListeners();
            answer3Button.onClick.RemoveAllListeners();
            answer4Button.onClick.RemoveAllListeners();

            answer1Button.onClick.AddListener(() => StartCoroutine(GetQuestion8(questions[7].id, questions)));
            answer2Button.onClick.AddListener(() => StartCoroutine(GetQuestion8(questions[7].id, questions)));
            answer3Button.onClick.AddListener(() => StartCoroutine(GetQuestion8(questions[7].id, questions)));
            answer4Button.onClick.AddListener(() => StartCoroutine(GetQuestion8(questions[7].id, questions, question7.score)));
        }
    }

    private IEnumerator GetQuestion8(int questionId, List<Question> questions, int score = 0)
    {

        totalScore += score;
        Debug.Log("get question 8: " + totalScore);
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
            Question question8 = JsonUtility.FromJson<Question>(request.downloadHandler.text);

            // Faça algo com a pergunta, como exibir na tela
            Debug.Log("Question ID: " + question8.id + ", Question: " + question8.question_en);

            string savedLanguage = PlayerPrefs.GetString("Language", "en");
            Debug.Log("savedLanguage: " + savedLanguage);

            if (savedLanguage == "en")
            {
                Debug.Log("O idioma recuperado é ingles.");
                question.text = question8.question_en;
                answer1.text = question8.answer1_en;
                answer2.text = question8.answer2_en;
                answer3.text = question8.answer3_en;
                correctAnswer.text = question8.correct_answer_en;
                questionNumber.text = 2 + "";
                if (question8.difficulty == 1)
                {
                    difficulty.text = "easy";
                }
                else if (question8.difficulty == 2)
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

                question.text = question8.question_pt;
                answer1.text = question8.answer1_pt;
                answer2.text = question8.answer2_pt;
                answer3.text = question8.answer3_pt;
                correctAnswer.text = question8.correct_answer_pt;
                questionNumber.text = 1 + "";
                if (question8.difficulty == 1)
                {
                    difficulty.text = "fácil";
                }
                else if (question8.difficulty == 2)
                {
                    difficulty.text = "normal";
                }
                else
                {
                    difficulty.text = "difícil";
                }
            }
            answer1Button.onClick.RemoveAllListeners();
            answer2Button.onClick.RemoveAllListeners();
            answer3Button.onClick.RemoveAllListeners();
            answer4Button.onClick.RemoveAllListeners();

            answer1Button.onClick.AddListener(() => StartCoroutine(GetQuestion9(questions[8].id, questions)));
            answer2Button.onClick.AddListener(() => StartCoroutine(GetQuestion9(questions[8].id, questions)));
            answer3Button.onClick.AddListener(() => StartCoroutine(GetQuestion9(questions[8].id, questions)));
            answer4Button.onClick.AddListener(() => StartCoroutine(GetQuestion9(questions[8].id, questions, question8.score)));
        }
    }

    private IEnumerator GetQuestion9(int questionId, List<Question> questions, int score = 0)
    {

        totalScore += score;
        Debug.Log("get question 9: " + totalScore);
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
            Question question9 = JsonUtility.FromJson<Question>(request.downloadHandler.text);

            // Faça algo com a pergunta, como exibir na tela
            Debug.Log("Question ID: " + question9.id + ", Question: " + question9.question_en);

            string savedLanguage = PlayerPrefs.GetString("Language", "en");
            Debug.Log("savedLanguage: " + savedLanguage);

            if (savedLanguage == "en")
            {
                Debug.Log("O idioma recuperado é ingles.");
                question.text = question9.question_en;
                answer1.text = question9.answer1_en;
                answer2.text = question9.answer2_en;
                answer3.text = question9.answer3_en;
                correctAnswer.text = question9.correct_answer_en;
                questionNumber.text = 2 + "";
                if (question9.difficulty == 1)
                {
                    difficulty.text = "easy";
                }
                else if (question9.difficulty == 2)
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

                question.text = question9.question_pt;
                answer1.text = question9.answer1_pt;
                answer2.text = question9.answer2_pt;
                answer3.text = question9.answer3_pt;
                correctAnswer.text = question9.correct_answer_pt;
                questionNumber.text = 1 + "";
                if (question9.difficulty == 1)
                {
                    difficulty.text = "fácil";
                }
                else if (question9.difficulty == 2)
                {
                    difficulty.text = "normal";
                }
                else
                {
                    difficulty.text = "difícil";
                }
            }
            answer1Button.onClick.RemoveAllListeners();
            answer2Button.onClick.RemoveAllListeners();
            answer3Button.onClick.RemoveAllListeners();
            answer4Button.onClick.RemoveAllListeners();

            answer1Button.onClick.AddListener(() => StartCoroutine(GetQuestion10(questions[9].id, questions)));
            answer2Button.onClick.AddListener(() => StartCoroutine(GetQuestion10(questions[9].id, questions)));
            answer3Button.onClick.AddListener(() => StartCoroutine(GetQuestion10(questions[9].id, questions)));
            answer4Button.onClick.AddListener(() => StartCoroutine(GetQuestion10(questions[9].id, questions, question9.score)));
        }
    }

    private IEnumerator GetQuestion10(int questionId, List<Question> questions, int score = 0)
    {

        totalScore += score;
        Debug.Log("get question 10: " + totalScore);
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
            Question question10 = JsonUtility.FromJson<Question>(request.downloadHandler.text);

            // Faça algo com a pergunta, como exibir na tela
            Debug.Log("Question ID: " + question10.id + ", Question: " + question10.question_en);

            string savedLanguage = PlayerPrefs.GetString("Language", "en");
            Debug.Log("savedLanguage: " + savedLanguage);

            if (savedLanguage == "en")
            {
                Debug.Log("O idioma recuperado é ingles.");
                question.text = question10.question_en;
                answer1.text = question10.answer1_en;
                answer2.text = question10.answer2_en;
                answer3.text = question10.answer3_en;
                correctAnswer.text = question10.correct_answer_en;
                questionNumber.text = 2 + "";
                if (question10.difficulty == 1)
                {
                    difficulty.text = "easy";
                }
                else if (question10.difficulty == 2)
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

                question.text = question10.question_pt;
                answer1.text = question10.answer1_pt;
                answer2.text = question10.answer2_pt;
                answer3.text = question10.answer3_pt;
                correctAnswer.text = question10.correct_answer_pt;
                questionNumber.text = 1 + "";
                if (question10.difficulty == 1)
                {
                    difficulty.text = "fácil";
                }
                else if (question10.difficulty == 2)
                {
                    difficulty.text = "normal";
                }
                else
                {
                    difficulty.text = "difícil";
                }
            }
            answer1Button.onClick.RemoveAllListeners();
            answer2Button.onClick.RemoveAllListeners();
            answer3Button.onClick.RemoveAllListeners();
            answer4Button.onClick.RemoveAllListeners();

            answer1Button.onClick.AddListener(() => StartCoroutine(GetToQuizEnd(questions, 0)));
            answer2Button.onClick.AddListener(() => StartCoroutine(GetToQuizEnd(questions, 0)));
            answer3Button.onClick.AddListener(() => StartCoroutine(GetToQuizEnd(questions, 0)));
            answer4Button.onClick.AddListener(() => StartCoroutine(GetToQuizEnd(questions, question10.score)));
        }
    }

    public IEnumerator GetToQuizEnd(List<Question> questions, int score = 0)
    {
        quizPortoPanel.SetActive(false);
        endQuizPanel.SetActive(true);
        Debug.Log("GetToQuizEnd");
        totalScore += score;
        Debug.Log("totalScore: " + totalScore);
        finalScore.text = totalScore + "";
        GameObject questionTemplate = endQuizQuestionsContainer.transform.GetChild(0).gameObject;
        GameObject container = transform.Find("EndQuizPanel/QuestionsPanel/ScrollArea/Scroll/Container").gameObject;

        GameObject q;
        string savedLanguage = PlayerPrefs.GetString("Language", "en");

        for (int i=0; i< questions.Count; i++)
        {
            q = Instantiate(questionTemplate, endQuizQuestionsContainer.transform);
            q.transform.GetChild(0).GetComponent<Text>().text = (savedLanguage == "en") ? questions[i].question_en : questions[i].question_pt;
            q.transform.GetChild(2).GetComponent<Text>().text = (savedLanguage == "en") ? questions[i].correct_answer_en : questions[i].correct_answer_pt;
        }

        Destroy(questionTemplate);
        StartCoroutine(UpdateQuizScore(quizId, totalScore));
        yield return "";
    }

    public IEnumerator UpdateQuizScore(int quizId, int score)
    {
        Debug.Log("update score");
        // Monta a URL com o ID do quiz e o score
        string url = quizUrl + "updateScore/" + quizId + "/" + score;

        // Cria a solicitação HTTP POST
        UnityWebRequest request = UnityWebRequest.Post(url, "");

        // Envia a solicitação
        yield return request.SendWebRequest();

        // Verifica se ocorreu algum erro na solicitação
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to update quiz score: " + request.error);
        }
        else
        {
            Debug.Log("Quiz score updated successfully.");
        }
    }
}
