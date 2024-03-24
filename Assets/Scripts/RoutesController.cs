using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RoutesController : MonoBehaviour
{
    // Start is called before the first frame update
    public Button chooseRouteBtn;
    public Button leaveChooseRouteBtn;
    public GameObject chooseRoutesPainel;

    private string apiUrlChooseRoutes = "http://localhost:3000/api/route/all";
    private double routeHeight = 800;
    private double routeNumber;

    public GameObject routeContainer;
    private GameObject routeTemplate;
    public Image imageRouteTest;

    [System.Serializable]
    public class Route
    {
        [JsonProperty("id")]
        public int id;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("city")]
        public string city;
        [JsonProperty("category")]
        public string category;
        [JsonProperty("rating")]
        public string rating;
        [JsonProperty("image")]
        public string image;
    }

    [System.Serializable]
    public class Routes
    {
        [JsonProperty("routes")]
        public Route[][] routes { get; set; }
    }


    void Start()
    {
        chooseRouteBtn.onClick.AddListener(GetRequestStats);

        //leaveChooseRouteBtn.onClick.AddListener(LeaveChooseRoute);
    }
    public void GetRequestStats()
    {
        int touristId = PlayerPrefs.GetInt("Current_Logged_TouristID", -1); // -1 é o valor padrão se a chave "UserID" não existir
        Debug.Log("get routes");

        // Envia a solicitação
        StartCoroutine(GetRoutesRequest());
    }

    private List<GameObject> instantiatedRouteObjects = new List<GameObject>();

    private IEnumerator GetRoutesRequest()
    {
        Debug.Log("routes get request");
        string url = apiUrlChooseRoutes;
        // Envia a solicitação
        UnityWebRequest request = UnityWebRequest.Get(url);

        // Envia a solicitação
        yield return request.SendWebRequest();

        // Verifica se ocorreu algum erro na solicitação
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to fetch routes: " + request.error);
        }
        else
        {
            Debug.Log("get routes");
            string jsonResponse = request.downloadHandler.text;
            Debug.Log("jsonResponse: " + jsonResponse);

            Routes routesObject = JsonConvert.DeserializeObject<Routes>(jsonResponse);

            // Acessar o primeiro item da matriz de rotas
            Route primeiroItem = routesObject.routes[0][0];
            Debug.Log("primeiro item name: " + primeiroItem.name);
            //LoadImageFromBase64(primeiroItem.image, imageRouteTest);

            routeTemplate = routeContainer.transform.GetChild(0).gameObject;
            GameObject p;
            
            foreach (Route[] rota in routesObject.routes)
            {
                foreach (Route item in rota)
                {
                    routeNumber = rota.Length;
                    AdjustContainerHeight(routeNumber);
                    p = Instantiate(routeTemplate, routeContainer.transform);
                    Image imageComponent = p.transform.GetChild(0).GetComponent<Image>();
                    LoadImageFromBase64(item.image, imageComponent);
                    // Acessar os membros de Route para cada item
                    p.transform.GetChild(1).GetComponent<Text>().text = item.name;
                    instantiatedRouteObjects.Add(p);
                    /**
                    int id = item.id;
                    string nome = item.name;
                    string cidade = item.city;
                    string categoria = item.category;
                    string rating = item.rating;
                    string imagem = item.image;

                    // Faça o que precisa com os dados de cada item
                    Debug.Log($"ID: {id}, Nome: {nome}, Cidade: {cidade}, Categoria: {categoria}, Rating: {rating}, Imagem: {imagem}");*/
                }
            }
            routeTemplate.SetActive(false);
            chooseRoutesPainel.SetActive(true);
        }
    }

    public void LoadImageFromBase64(string base64String, Image image)
    {
        Debug.Log("LoadImageFromBase64");
        //Debug.Log("base64String: " + base64String);

        // Converte a string base64 para bytes
        byte[] imageBytes = Convert.FromBase64String(base64String);

        // Cria uma textura a partir dos bytes da imagem
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(imageBytes);

        // Define a textura no componente Image
        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }

    void AdjustContainerHeight(double numberElements)
    {
        double totalHeight = numberElements * routeHeight + (numberElements - 1) * 50; // Calculating total height
        RectTransform containerRectTransform = routeContainer.GetComponent<RectTransform>();
        containerRectTransform.sizeDelta = new Vector2(containerRectTransform.sizeDelta.x, (float)totalHeight);
    }
}
