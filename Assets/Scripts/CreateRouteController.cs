using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CreateRouteController : MonoBehaviour
{
    public GameObject createRoutePainel;
    public GameObject createRouteStep;
    public GameObject createPOIStep;

    public Button openCreateRoutePainel;
    public Button createRouteBtn;


    public InputField nameInputField;
    public InputField cityInputField;
    public InputField categoryInputField;


    [System.Serializable]
    public class Route
    {
        public string name;
        public string city;
        public string category;
        public int created;
    }

    [System.Serializable]
    public class CreateRouteResponse
    {
        public bool success;
        public string message;
        public int routeId;
    }
    void Start()
    {
        openCreateRoutePainel.onClick.AddListener(OpenCreateRouteFunc);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OpenCreateRouteFunc()
    {
        Debug.Log("clicou para criar route");
        createRoutePainel.SetActive(true);
        createRouteBtn.onClick.AddListener(CreateRoute);
    }
    void CreateRoute()
    {
        
        string name = nameInputField.text;
        string city = cityInputField.text;
        string category = categoryInputField.text;

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(city) || string.IsNullOrEmpty(category))
        {
            Debug.Log("campos em branco foda-se");
            return;
        }
        else
        {
            int touristId = PlayerPrefs.GetInt("Current_Logged_TouristID", -1); // -1 é o valor padrão se a chave "UserID" não existir
            Debug.Log("proceda com o create route.");
            Route route = new Route();
            route.name = name;
            route.city = city;
            route.category = category;
            route.created = touristId;
            StartCoroutine(CreateRoute(route.name, route.city, route.category, route.created));
        }
    }

    IEnumerator CreateRoute(string name, string city, string category, int created)
    {
        string uri = "http://13.60.19.19:3000/api/route/add";
        string jsonData = "{\"name\":\"" + name + "\", \"city\":\"" + city + "\", \"category\":\"" + category + "\",  \"created\":\"" + created + "\"}";

        UnityWebRequest request = new UnityWebRequest(uri, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Envia a solicitação e espera pela resposta
        yield return request.SendWebRequest();

        // Verifica se houve algum erro na resposta
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Erro ao registar: " + request.error);
        }
        else
        {
            // Analisa a resposta da API
            string responseText = request.downloadHandler.text;
            CreateRouteResponse responseData = JsonUtility.FromJson<CreateRouteResponse>(responseText);

            // Verifica se o login foi bem-sucedido
            if (responseData != null && responseData.message != null)
            {
                Debug.Log("criação de rota bem sucedida " + responseData.message + ", rota id: " + responseData.routeId);

            }
            else
            {
                Debug.LogError("Erro ao fazer login: " + responseData.message);
            }
        }
    }
}
