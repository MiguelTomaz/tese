using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoutesController : MonoBehaviour
{
    // Start is called before the first frame update
    public Button chooseRouteBtn;
    public Button leaveChooseRouteBtn;
    public Button leaveRouteDetailsBtn;
    public Button leaveRouteSelectBtn;
    public Button startRouteExploration;

    public GameObject chooseRoutesPainel;

    private string apiUrlChooseRoutes = "http://localhost:3000/api/route/all";
    private string apiUrlRouteDetails = "http://localhost:3000/api/route/details/";
    private double routeHeight = 800;
    private double poiHeight = 1100;

    private double routeNumber;
    private double poiNumber;
    private bool isUserCloseToPoi = false;

    public GameObject routeContainer;
    private GameObject routeTemplate;
    public GameObject poiContainer;
    private GameObject poiTemplate;
    public GameObject routeDetailsPainel;
    public GameObject routeSelectPainel;
    public Image imageRouteTest;
    public Image imageRouteSelected;

    public Text RouteName;
    public Text RouteCity;
    public Text RouteCategory;

    public Text selectRouteExplanation;
    public Text selectRouteExplanation2;
    public Text selectRouteName;


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

    [System.Serializable]
    public class RouteDetailsResponse
    {
        [JsonProperty("route")]
        public RouteDetails route;
        [JsonProperty("poiList")]
        public List<PointOfInterest> poiList;
    }

    [System.Serializable]
    public class RouteDetails
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
        public int rating;
        [JsonProperty("image")]
        public string image;
    }

    [System.Serializable]
    public class PointOfInterest
    {
        public int id;
        public int route_id;
        public string name;
        public string description;
        public int order_in_route;
        public string category;
        public double latitude;
        public double longitude;
        public double altitude;
        public string creator_name;
        public string architectural_style;
        public string website;
        public string opening_hours;
        public int rating;
    }

    void Start()
    {
        chooseRouteBtn.onClick.AddListener(GetRequestStats);
        leaveRouteSelectBtn.onClick.AddListener(BackFromSelectRoute);

    }
    public void GetRequestStats()
    {
        int touristId = PlayerPrefs.GetInt("Current_Logged_TouristID", -1); // -1 é o valor padrão se a chave "UserID" não existir
        Debug.Log("get routes");

        // Envia a solicitação
        StartCoroutine(GetRoutesRequest());
    }

    private List<GameObject> instantiatedRouteObjects = new List<GameObject>();
    private List<GameObject> instantiatedPOIObjects = new List<GameObject>();

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
            //Debug.Log("jsonResponse: " + jsonResponse);

            Routes routesObject = JsonConvert.DeserializeObject<Routes>(jsonResponse);

            // Acessar o primeiro item da matriz de rotas
            Route primeiroItem = routesObject.routes[0][0];
            //Debug.Log("primeiro item name: " + primeiroItem.name);
            //LoadImageFromBase64(primeiroItem.image, imageRouteTest);

            routeTemplate = routeContainer.transform.GetChild(0).gameObject;
            leaveChooseRouteBtn.onClick.AddListener(BackFromChooseRoute);
            Debug.Log("routeTemplate1: " + routeTemplate);
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

                    Button buttonComponentSelect = p.transform.GetChild(3).GetComponent<Button>();
                    buttonComponentSelect.onClick.AddListener(() => { GetRouteDetailsSelect(item.id); });

                    Button buttonComponent = p.transform.GetChild(4).GetComponent<Button>();
                    buttonComponent.onClick.AddListener(() => { GetRouteDetails(item.id); });


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
            Debug.Log("routeTemplate2: " + routeTemplate);
            chooseRoutesPainel.SetActive(true);
        }
    }

    private void GetRouteDetails(int id)
    {
        StartCoroutine(GetRoutesDetailsRequest(id));
    }

    private void GetRouteDetailsSelect(int id)
    {
        StartCoroutine(GetRoutesDetailsSelectRequest(id));
    }

    private IEnumerator GetRoutesDetailsRequest(int id)
    {
        
        string url = apiUrlRouteDetails + id;
        Debug.Log("routes get details da rota " + id + ", url: " + url);
        // Envia a solicitação
        UnityWebRequest request = UnityWebRequest.Get(url);

        // Envia a solicitação
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to fetch routes: " + request.error);
        }
        else
        {
            Debug.Log("get routes details");
            string jsonResponse = request.downloadHandler.text;
            //Debug.Log("jsonResponse: " + jsonResponse);
            /**
            List<RouteDetails> jsonArray = JsonConvert.DeserializeObject<List<RouteDetails>>(jsonResponse);
            foreach (RouteDetails item in jsonArray)
            {
                Debug.Log("item3: " + item.name);
            }
            */
            RouteDetailsResponse response = JsonUtility.FromJson<RouteDetailsResponse>(jsonResponse);

            if (response != null)
            {
                
                Debug.Log("ID da Rota: " + response.route.id);
                Debug.Log("Nome da Rota: " + response.route.name);

                RouteName.text = response.route.name;
                //poiDescription = response.route.d
                RouteCity.text = response.route.city;
                RouteCategory.text = response.route.category;

                poiTemplate = poiContainer.transform.GetChild(0).gameObject;
                leaveRouteDetailsBtn.onClick.AddListener(BackFromRouteDetails);
                GameObject p;
                foreach (var poi in response.poiList)
                {
                    Debug.Log("ID do poi: " + poi.id);
                    Debug.Log("Nome do poi: " + poi.name);
                    p = Instantiate(poiTemplate, poiContainer.transform);
                    poiNumber = response.poiList.Count;
                    AdjustContainerPOIHeight(poiNumber);
                    p.transform.GetChild(0).GetComponent<Text>().text = poi.name;
                    p.transform.GetChild(2).GetComponent<Text>().text = poi.description;
                    p.transform.GetChild(4).GetComponent<Text>().text = poi.creator_name;
                    p.transform.GetChild(6).GetComponent<Text>().text = poi.architectural_style;

                    instantiatedPOIObjects.Add(p);
                }
                poiTemplate.SetActive(false);
            }
            else
            {
                Debug.Log("No route details found in the response.");
            }
            routeDetailsPainel.SetActive(true);
        }
    }

    private IEnumerator GetRoutesDetailsSelectRequest(int id)
    {
        string url = apiUrlRouteDetails + id;
        Debug.Log("routes get details da rota " + id + ", url: " + url);
        // Envia a solicitação
        UnityWebRequest request = UnityWebRequest.Get(url);

        // Envia a solicitação
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to fetch routes: " + request.error);
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;
            RouteDetailsResponse response = JsonUtility.FromJson<RouteDetailsResponse>(jsonResponse);

            if (response != null)
            {
                int routeId = response.route.id;
                string routeName = response.route.name;
                string routeCity = response.route.city;
                string firstPoiName = response.poiList[0].name;
                double firstPoiLatitude = response.poiList[0].latitude;
                double firstPoiLongitude = response.poiList[0].longitude;

                SelectRoute(routeId, routeName, routeCity, firstPoiName, firstPoiLatitude, firstPoiLongitude, response.route.image);
            }
            else
            {
                Debug.Log("No route details found in the response.");
            }
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
    private void ClearInstantiatedRoutesObjects()
    {
        foreach (GameObject obj in instantiatedRouteObjects)
        {
            Destroy(obj);
        }
        instantiatedRouteObjects.Clear();
    }
    public void BackFromRouteDetails()
    {
        Debug.Log("LEAVE route details");
        poiTemplate.SetActive(true);
        routeDetailsPainel.SetActive(false);
        ClearInstantiatedPOIObjects();
    }

    public void BackFromChooseRoute()
    {
        Debug.Log("LEAVE route");
        Debug.Log("routeTemplate: " + routeTemplate);
        routeTemplate.SetActive(true);
        chooseRoutesPainel.SetActive(false);
        ClearInstantiatedRoutesObjects();
    }

    private bool isCheckingDistance = false;

    public void SelectRoute(int routeId, string routeName, string cityName, string poiName, double latitudePoi, double longitudePoi, string routeImage)
    {

        string latitudeString = PlayerPrefs.GetString("UserLatitude");
        string longitudeString = PlayerPrefs.GetString("UserLongitude");

        Debug.Log("latitude poi: " + latitudePoi);
        Debug.Log("longitude poi: " + longitudePoi);

        double latitudeUser = 41.756947;
        double longitudeUser = -7.4620061;

        if (!string.IsNullOrEmpty(latitudeString))
        {
            latitudeUser = double.Parse(latitudeString);
        }

        if (!string.IsNullOrEmpty(longitudeString))
        {
            longitudeUser = double.Parse(longitudeString);
        }

        if (!isCheckingDistance)
        {
            StartCoroutine(CheckDistanceCoroutine(latitudeUser, longitudeUser, latitudePoi, longitudePoi));
        }

        //bool isUserCloseToPoi = IsWithinDistance(latitudeUser, longitudeUser, latitudePoi, longitudePoi, 5);

        string message = "";
        string message2 = "";
        string savedLanguage = PlayerPrefs.GetString("Language", "en");
        if (savedLanguage == "en")
        {
            message = $"To start the {routeName} tourist route, it is necessary to be in the city of {cityName}, at the point of interest {poiName} which is located approximately at the geographic coordinates [{latitudePoi}, {longitudePoi}]. You can use a map app or GPS device to navigate to the location. Once you arrive, the application will take you to the POI, so you can start the tourist route.\n\n I hope you have a great exploration along this route! Enjoy every moment and discover wonders along the way.";
            message2 = $"The start button will only be enabled when you are 5 meters or less from the point of interest {poiName}";
        }
        else
        {
            message = $"Para iniciar a rota turística de {routeName}, é necessário estar na cidade de {cityName}, no ponto de interesse {poiName} que se situa aproximadamente nas coordenadas geográficas [{latitudePoi} , {longitudePoi}]. Você pode usar um aplicativo de mapa ou dispositivo de GPS para navegar até à localidade. Assim que chegar, a aplicação irá encarregar-se de o levar para o POI, de forma a poder iniciar a rota turística.\n\nEspero que tenha uma excelente exploração ao longo desta rota! Aproveite cada momento e descubra as maravilhas ao longo do caminho.";
            message2 = $"O botão iniciar só será habilitado quando você estiver a 5 metros ou menos do ponto de interesse {poiName}";
        }
        selectRouteExplanation.text = message;
        selectRouteExplanation2.text = message2;
        selectRouteName.text = routeName;

        LoadImageFromBase64(routeImage, imageRouteSelected);

        startRouteExploration.onClick.AddListener(() => ExploreRoute(routeId));
        routeSelectPainel.SetActive(true);
    }

    private IEnumerator CheckDistanceCoroutine(double latitudeUser, double longitudeUser, double latitudePoi, double longitudePoi)
    {
        isCheckingDistance = true;
        while (isCheckingDistance)
        {
            bool isUserClose = IsWithinDistance(latitudeUser, longitudeUser, latitudePoi, longitudePoi, 5);
            if (isUserClose)
            {
                Debug.Log("O usuário está perto do ponto de interesse.");
                isUserCloseToPoi = true;
                Debug.Log("está proximo 2");
                Image buttonImage = startRouteExploration.GetComponent<Image>();
                Color newColor = new Color(0.24f, 0.70f, 0.44f);
                buttonImage.color = newColor;
                startRouteExploration.interactable = true;
                // Faça o que for necessário quando o usuário estiver perto do ponto de interesse
            }
            else
            {
                isUserCloseToPoi = false;
                Debug.Log("O usuário está longe do ponto de interesse.");
                Image buttonImage = startRouteExploration.GetComponent<Image>();
                Color newColor = new Color(0f, 0.35f, 0.16f);
                buttonImage.color = newColor;
                startRouteExploration.interactable = false;
            }
            yield return new WaitForSeconds(1f);
        }
    }

    // Método para calcular a distância entre dois pontos usando a fórmula de haversine
    public bool IsWithinDistance(double lat1, double lon1, double lat2, double lon2, double distanceThreshold)
    {
        const double earthRadius = 6371000; // raio da Terra em metros

        // Converte graus para radianos
        double dLat = DegreesToRadians(lat2 - lat1);
        double dLon = DegreesToRadians(lon2 - lon1);

        // Fórmula de Haversine
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        // Distância em metros
        double distance = earthRadius * c;

        // Verifica se a distância está dentro do limite especificado
        return distance <= distanceThreshold;
    }

    // Método auxiliar para converter graus em radianos
    public double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }

    public void ExploreRoute(int routeId)
    {
        PlayerPrefs.SetInt("choosenRoute", routeId);
        PlayerPrefs.Save();
        SceneManager.LoadScene("SampleScene");
    }

    public void BackFromSelectRoute()
    {
        isCheckingDistance = false;
        routeSelectPainel.SetActive(false);
    }
}
