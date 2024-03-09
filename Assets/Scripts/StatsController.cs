using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class StatsController : MonoBehaviour
{
    [System.Serializable]
    public class ScoreResponse
    {
        public int totalScore;
    }
    public Button statsButton;
    private string quizUrl = "http://localhost:3000/api/quiz/";
    public Text score;
    // Start is called before the first frame update
    void Start()
    {
        statsButton.onClick.AddListener(GetTotalScoreByTouristId);

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void GetTotalScoreByTouristId()
    {
        int touristId = PlayerPrefs.GetInt("Current_Logged_TouristID", -1);
        // Monta a URL com o ID do turista
        
        Debug.Log("get score");
        // Cria a solicitação HTTP GET

        // Envia a solicitação
        StartCoroutine(GetScoreRequest(touristId));
    }

    private IEnumerator GetScoreRequest(int touristId)
    {
        string url = quizUrl + "score/user/" + touristId;
        // Envia a solicitação
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
            string jsonResponse = request.downloadHandler.text;

            // Converte a resposta JSON para um objeto ScoreResponse
            ScoreResponse response = JsonUtility.FromJson<ScoreResponse>(jsonResponse);

            // Verifica se a resposta não é nula e exibe o total de score
            if (response != null)
            {
                Debug.Log("Total Score: " + response.totalScore);
                score.text = response.totalScore + "";
            }
            else
            {
                Debug.LogWarning("Empty response received");
            }
        }
    }
}
