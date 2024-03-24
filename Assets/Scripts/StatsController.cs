using Newtonsoft.Json;
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

    [System.Serializable]
    public class POIvisitedResponse
    {
        public int poi_visited;
    }

    [System.Serializable]
    public class PhotoTakenResponse
    {
        public int photo_count;
    }

    [System.Serializable]
    public class Leaderboard
    {
        public List<Tourist> leaderboard;
    }

    [System.Serializable]
    public class Tourist
    {
        public int tourist_id;
        public int photo_count;
        public int quiz_score;
        public int poi_visited;
        public string email;
    }

    public Button statsButton;
    private string quizUrl = "http://localhost:3000/api/quiz/";
    private string leaderboardUrl = "http://localhost:3000/api/photo/ranking";
    private string poi_visitedUrl = "http://localhost:3000/api/user/poi_visited/";
    private string photoTakenUrl = "http://localhost:3000/api/user/photo_count/";

    public Text score;
    public Text poiVisted;
    public Text photoTaken;

    public GameObject user1Leaderboard;
    public GameObject user2Leaderboard;
    public GameObject user3Leaderboard;

    //email leaderboard
    public Text touristEmailLeaderboard1;
    public Text touristEmailLeaderboard2;
    public Text touristEmailLeaderboard3;

    //poi leaderboard
    public Text touristPOILeaderboard1;
    public Text touristPOILeaderboard2;
    public Text touristPOILeaderboard3;

    //photo leaderboard
    public Text touristPhotoLeaderboard1;
    public Text touristPhotoLeaderboard2;
    public Text touristPhotoLeaderboard3;

    //score leaderboard
    public Text touristScoreLeaderboard1;
    public Text touristScoreLeaderboard2;
    public Text touristScoreLeaderboard3;

    // Start is called before the first frame update
    void Start()
    {
        statsButton.onClick.AddListener(GetRequestStats);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void GetRequestStats()
    {
        int touristId = PlayerPrefs.GetInt("Current_Logged_TouristID", -1);
        // Monta a URL com o ID do turista
        
        Debug.Log("get score");
        // Cria a solicitação HTTP GET

        // Envia a solicitação
        StartCoroutine(GetScoreRequest(touristId));
        StartCoroutine(GetPOIvisitedRequest(touristId));
        StartCoroutine(GetPhotoTakenRequest(touristId));

        StartCoroutine(GetLeaderBoardRequest());
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

    private IEnumerator GetPOIvisitedRequest(int touristId)
    {
        string url = poi_visitedUrl + touristId;

        UnityWebRequest request = UnityWebRequest.Get(url);

        // Envia a solicitação
        yield return request.SendWebRequest();

        // Verifica se ocorreu algum erro na solicitação
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to fetch poi visited: " + request.error);
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;

            // Converte a resposta JSON para um objeto ScoreResponse
            POIvisitedResponse response = JsonUtility.FromJson<POIvisitedResponse>(jsonResponse);

            // Verifica se a resposta não é nula e exibe o total de score
            if (response != null)
            {
                Debug.Log("Total poi visited: " + response.poi_visited);
                poiVisted.text = response.poi_visited + "";
            }
            else
            {
                Debug.LogWarning("Empty response received");
            }
        }
    }

    private IEnumerator GetPhotoTakenRequest(int touristId)
    {
        string url = photoTakenUrl + touristId;

        UnityWebRequest request = UnityWebRequest.Get(url);

        // Envia a solicitação
        yield return request.SendWebRequest();

        // Verifica se ocorreu algum erro na solicitação
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to fetch photos counts : " + request.error);
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;

            // Converte a resposta JSON para um objeto ScoreResponse
            PhotoTakenResponse response = JsonUtility.FromJson<PhotoTakenResponse>(jsonResponse);

            // Verifica se a resposta não é nula e exibe o total de score
            if (response != null)
            {
                Debug.Log("Total photo_count: " + response.photo_count);
                photoTaken.text = response.photo_count + "";
            }
            else
            {
                Debug.LogWarning("Empty response received");
            }
        }
    }

    private IEnumerator GetLeaderBoardRequest()
    {
        Debug.Log("leasderboard");
        string url = leaderboardUrl;
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
            Debug.Log("get tourist leasderboard");
            string jsonResponse = request.downloadHandler.text;
            Debug.Log("jsonResponse: " + jsonResponse);
            Leaderboard response = JsonConvert.DeserializeObject<Leaderboard>(jsonResponse);

            //List<Tourist> leaderboard = JsonConvert.DeserializeObject<List<Tourist>>(jsonResponse);

            foreach (Tourist tourist in response.leaderboard)
            {
                Debug.Log("Tourist ID: " + tourist.tourist_id);
                Debug.Log("User Email: " + tourist.email);
                Debug.Log("Photo Count: " + tourist.photo_count);
                Debug.Log("Quiz Score: " + tourist.quiz_score);
                Debug.Log("POI Visited: " + tourist.poi_visited);
            }

            if (response.leaderboard.Count == 0)
            {
                user1Leaderboard.SetActive(false);
                user2Leaderboard.SetActive(false);
                user3Leaderboard.SetActive(false);
            }

            if (response.leaderboard.Count == 1)
            {
                // Define o email do primeiro turista
                touristEmailLeaderboard1.text = response.leaderboard[0].email;
                touristPOILeaderboard1.text = response.leaderboard[0].poi_visited + "";
                touristPhotoLeaderboard1.text = response.leaderboard[0].photo_count + "";
                touristScoreLeaderboard1.text = response.leaderboard[0].quiz_score + "";

                user2Leaderboard.SetActive(false);
                user3Leaderboard.SetActive(false);
            }

            if (response.leaderboard.Count == 2)
            {
                touristEmailLeaderboard1.text = response.leaderboard[0].email;
                touristPOILeaderboard1.text = response.leaderboard[0].poi_visited + "";
                touristPhotoLeaderboard1.text = response.leaderboard[0].photo_count + "";
                touristScoreLeaderboard1.text = response.leaderboard[0].quiz_score + "";

                touristEmailLeaderboard2.text = response.leaderboard[1].email;
                touristPOILeaderboard2.text = response.leaderboard[1].poi_visited + "";
                touristPhotoLeaderboard2.text = response.leaderboard[1].photo_count + "";
                touristScoreLeaderboard2.text = response.leaderboard[1].quiz_score + "";


                user3Leaderboard.SetActive(false);
            }

            if (response.leaderboard.Count == 3)
            {
                touristEmailLeaderboard1.text = response.leaderboard[0].email;
                touristPOILeaderboard1.text = response.leaderboard[0].poi_visited + "";
                touristPhotoLeaderboard1.text = response.leaderboard[0].photo_count + "";
                touristScoreLeaderboard1.text = response.leaderboard[0].quiz_score + "";

                touristEmailLeaderboard2.text = response.leaderboard[1].email;
                touristPOILeaderboard2.text = response.leaderboard[1].poi_visited + "";
                touristPhotoLeaderboard2.text = response.leaderboard[1].photo_count + "";
                touristScoreLeaderboard2.text = response.leaderboard[1].quiz_score + "";

                touristEmailLeaderboard3.text = response.leaderboard[2].email;
                touristPOILeaderboard3.text = response.leaderboard[2].poi_visited + "";
                touristPhotoLeaderboard3.text = response.leaderboard[2].photo_count + "";
                touristScoreLeaderboard3.text = response.leaderboard[2].quiz_score + "";
            }
        }
    }
}
