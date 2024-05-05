using Newtonsoft.Json;
using System;
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
    public Button finishCreationRoute;
    public Button cancelCreationRoute;

    public InputField nameInputField;
    public InputField cityInputField;
    public InputField categoryInputField;
    private GameObject poiTemplate;
    public GameObject poiContainer;
    private double poiHeight = 1000;
    private double poiNumber;
    private int orderCount = 0;


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

    [System.Serializable]
    public class PointOfInterest
    {
        [JsonProperty("id")]
        public int id;
        [JsonProperty("route_id")]
        public int route_id;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("description")]
        public string description;
        [JsonProperty("order_in_route")]
        public int order_in_route;
        [JsonProperty("category")]
        public string category;
        [JsonProperty("latitude")]
        public double latitude;
        [JsonProperty("longitude")]
        public double longitude;
        [JsonProperty("altitude")]
        public double altitude;
        [JsonProperty("creator_name")]
        public string creator_name;
        [JsonProperty("architectural_style")]
        public string architectural_style;
        [JsonProperty("website")]
        public string website;
        [JsonProperty("opening_hours")]
        public string opening_hours;
        [JsonProperty("rating")]
        public int rating;
        [JsonProperty("city")]
        public string city;
        [JsonProperty("image")]
        public string image;
    }

    [System.Serializable]
    public class PoiList
    {
        public List<PointOfInterest> poi_list;
    }

    [System.Serializable]
    public class PoiListResponse
    {
        public List<PointOfInterest> PoiList { get; set; }
    }

    [System.Serializable]
    public class ApiResponse
    {
        public List<PoiListResponse> ListApi { get; set; }
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

        createRouteBtn.onClick.RemoveListener(CreateRoute);
        cancelCreationRoute.onClick.RemoveListener(CancelCreateRouteFunc);


        createRouteBtn.onClick.AddListener(CreateRoute);
        cancelCreationRoute.onClick.AddListener(CancelCreateRouteFunc);
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
        Debug.Log("jsonData create route: " + jsonData);
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
                GoToAddPoi(responseData.routeId, created);

            }
            else
            {
                Debug.LogError("Erro ao fazer login: " + responseData.message);
            }
        }
    }

    void GoToAddPoi(int routeId, int created)
    {
        StartCoroutine(GetPoiListRequest(routeId, created));
    }

    private List<GameObject> instantiatedPOIObjects = new List<GameObject>();

    private IEnumerator GetPoiListRequest(int routeId, int created)
    {
        string url = "http://13.60.19.19:3000/api/route/poi/all";
        Debug.Log("get poi list");
        // Envia a solicitação
        UnityWebRequest request = UnityWebRequest.Get(url);

        // Envia a solicitação
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to fetch poi list: " + request.error);
        }
        else
        {
            Debug.Log("get poi list");
            string jsonResponse = request.downloadHandler.text;
            Debug.Log("jsonResponse: " + jsonResponse);
            PoiList response = JsonUtility.FromJson<PoiList>(jsonResponse);

            if (response != null)
            {
                if (response != null && response.poi_list != null)
                {
                    poiTemplate = poiContainer.transform.GetChild(0).gameObject;
                    GameObject p;
                    foreach (PointOfInterest poi in response.poi_list)
                    {
                        Debug.Log("name poi: " + poi.name);
                        p = Instantiate(poiTemplate, poiContainer.transform);
                        p.transform.GetChild(0).GetComponent<Text>().text = poi.name;
                        Image imageComponent = p.transform.GetChild(1).GetComponent<Image>();
                        if(poi.image != null)
                        {
                            LoadImageFromBase64(poi.image, imageComponent);
                        }
                        p.transform.GetChild(2).GetComponent<Text>().text = poi.description;
                        p.transform.GetChild(3).GetComponent<Text>().text = poi.city;
                        p.transform.GetChild(5).GetComponent<Text>().text = poi.latitude + "";
                        p.transform.GetChild(7).GetComponent<Text>().text = poi.longitude + "";

                        Button buttonComponentSelect = p.transform.GetChild(8).GetComponent<Button>();
                        buttonComponentSelect.onClick.AddListener(() => { addPoiToRoute(buttonComponentSelect, routeId, orderCount, poi.id); });

                        poiNumber = response.poi_list.Count;
                        AdjustContainerPOIHeight(poiNumber);
                        instantiatedPOIObjects.Add(p);
                    }
                    poiTemplate.SetActive(false);
                    createRouteStep.SetActive(false);
                    createPOIStep.SetActive(true);
                    finishCreationRoute.onClick.AddListener(FinishCreateRouteFunc);
                }
                else
                {
                    Debug.Log("Failed to parse JSON.");
                }

            }
            else
            {
                Debug.Log("No poi list found in the response.");
            }
        }
    }

    void addPoiToRoute(Button buttonComponentSelect, int routeId, int order_in_route, int id)
    {
        orderCount++;
        //buttonComponentSelect.interactable = false;
        //Debug.Log("poi " + id + "add com order: " + order_in_route);
        StartCoroutine(AddPoiToRouteRequest(buttonComponentSelect, routeId, orderCount, id));
    }

    private IEnumerator AddPoiToRouteRequest(Button buttonComponentSelect, int routeId, int orderCount, int id)
    {
        int routeIdTeste = -1;
        string uri = "http://13.60.19.19:3000/api/route/addPoiToRoute";
        string jsonData = "{\"route_id\":\"" + routeIdTeste + "\", \"order_in_route\":\"" + orderCount + "\", \"id\":\"" + id + "\"}";

        WWWForm form = new WWWForm();
        form.AddField("route_id", routeId);
        form.AddField("order_in_route", orderCount);
        form.AddField("id", id);

        Debug.Log("add poi com route_id: " + routeId + ", order: " + orderCount + ", e id= " + id);

        using (UnityWebRequest www = UnityWebRequest.Post(uri, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Erro ao registar: " + www.error);
                Debug.Log("nao add, ordem mantem-se: " + orderCount);
            }
            else
            {
                //orderCount++;
                buttonComponentSelect.interactable = false;
            }
        }
        /**
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
            Debug.Log("Erro ao registar: " + request.error);
            Debug.Log("nao add, ordem mantem-se: " + orderCount);
        }
        else
        {
            orderCount++;
            buttonComponentSelect.interactable = false;
        }
        */
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

    void AdjustContainerPOIHeight(double numberElements)
    {
        double totalHeight = numberElements * poiHeight + (numberElements - 1) * 50; // Calculating total height
        RectTransform containerRectTransform = poiContainer.GetComponent<RectTransform>();
        containerRectTransform.sizeDelta = new Vector2(containerRectTransform.sizeDelta.x, (float)totalHeight);
    }
    private void ClearInstantiatedPOIObjects()
    {
        foreach (GameObject obj in instantiatedPOIObjects)
        {
            Destroy(obj);
        }
        instantiatedPOIObjects.Clear();
    }

    public void FinishCreateRouteFunc()
    {
        ClearInstantiatedPOIObjects();
        poiTemplate.SetActive(true);
        createPOIStep.SetActive(false);
        createRouteStep.SetActive(true);
        createRoutePainel.SetActive(false);
    }

    public void CancelCreateRouteFunc()
    {
        createRoutePainel.SetActive(false);
    }
}
