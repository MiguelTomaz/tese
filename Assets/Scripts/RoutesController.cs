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
    public Button leaveRouteRateBtn;
    public Button leavePoiRateBtn;
    public Button startRouteExploration;
    public Button submitRouteRateButton;
    public Button submitPoiRateButton;


    public GameObject chooseRoutesPainel;
    public Dropdown dropdownRateRoute;
    public Dropdown dropdownRatePoi;

    private string apiUrlChooseRoutes = "http://13.60.19.19:3000/api/route/all";
    private string apiUrlRouteDetails = "http://13.60.19.19:3000/api/route/details/";
    private double routeHeight = 1100;
    private double poiHeight = 1100;

    private double routeNumber;
    private double poiNumber;
    private bool isUserCloseToPoi = false;
    private int rateRouteValue = 0;
    private int rateRouteSelectedRouteId = 0;
    private int ratePoiValue = 0;
    private int ratePoiSelectedPoiId = 0;

    public GameObject routeContainer;
    private GameObject routeTemplate;
    public GameObject poiContainer;
    private GameObject poiTemplate;
    public GameObject routeDetailsPainel;
    public GameObject routeRatePainel;
    public GameObject poiRatePainel;
    public GameObject routeSelectPainel;
    public Image imageRouteTest;
    public Image imageRouteSelected;
    public Image routeRatingDetails5;
    public Image routeRatingDetails4;
    public Image routeRatingDetails3;
    public Image routeRatingDetails2;
    public Image routeRatingDetails1;

    public Text RouteName;
    public Text RouteCity;
    public Text RouteCategory;

    public Text selectRouteExplanation;
    public Text selectRouteExplanation2;
    public Text selectRouteName;
    public Text messageSucessRateRoute;
    public Text messageErrorRateRoute;
    public Text messageSucessRatePoi;
    public Text messageErrorRatePoi;

    [System.Serializable]
    public class TouristicRoutesResponse
    {
        public bool success;
        public string message;
        public int touristicRouteId;
    }

    [System.Serializable]
    public class RateRouteResponse
    {
        public string message;
        public string operation;
        public int roundedAverageRating;
    }

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
        public string city;
        public string image;
    }

    void Start()
    {
        chooseRouteBtn.onClick.AddListener(GetRequestStats);
        leaveRouteSelectBtn.onClick.AddListener(BackFromSelectRoute);
        leaveRouteRateBtn.onClick.AddListener(BackFromRateRoute);
        leavePoiRateBtn.onClick.AddListener(BackFromRatePoi);

        dropdownRateRoute.onValueChanged.RemoveAllListeners();

        dropdownRateRoute.onValueChanged.AddListener(delegate {
            DropdownRateValueChanged(dropdownRateRoute);
        });

        dropdownRatePoi.onValueChanged.RemoveAllListeners();

        dropdownRatePoi.onValueChanged.AddListener(delegate {
            DropdownRatePoiValueChanged(dropdownRatePoi);
        });

        submitRouteRateButton.onClick.RemoveAllListeners();
        submitRouteRateButton.onClick.AddListener(SubmitRouteRate);

        submitPoiRateButton.onClick.RemoveAllListeners();
        submitPoiRateButton.onClick.AddListener(SubmitPoiRate);
    }
    void Update()
    {
    }
    public void GetRequestStats()
    {
        int touristId = PlayerPrefs.GetInt("Current_Logged_TouristID", -1); // -1 � o valor padr�o se a chave "UserID" n�o existir
        Debug.Log("get routes");

        // Envia a solicita��o
        StartCoroutine(GetRoutesRequest());
    }

    private List<GameObject> instantiatedRouteObjects = new List<GameObject>();
    private List<GameObject> instantiatedPOIObjects = new List<GameObject>();

    private IEnumerator GetRoutesRequest()
    {
        Debug.Log("routes get request");
        int touristId = PlayerPrefs.GetInt("Current_Logged_TouristID", -1);
        string url = apiUrlChooseRoutes + "/" + touristId;
        // Envia a solicita��o
        UnityWebRequest request = UnityWebRequest.Get(url);

        // Envia a solicita��o
        yield return request.SendWebRequest();

        // Verifica se ocorreu algum erro na solicita��o
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

                    Button buttonComponentRate = p.transform.GetChild(10).GetComponent<Button>();
                    buttonComponentRate.onClick.AddListener(() => { GetRouteRate(item.id); });
                    Debug.Log("rating da rota: " + item.rating);

                    if (int.TryParse(item.rating, out int ratingValue))
                    {
                        if (ratingValue == 5)
                        {
                            // Ativar a imagem no filho 5
                            p.transform.GetChild(5).gameObject.SetActive(true);
                        }
                        else if (ratingValue == 4)
                        {
                            p.transform.GetChild(6).gameObject.SetActive(true);
                        }
                        else if (ratingValue == 3)
                        {
                            p.transform.GetChild(7).gameObject.SetActive(true);
                        }
                        else if (ratingValue == 2)
                        {
                            p.transform.GetChild(8).gameObject.SetActive(true);
                        }
                        else if (ratingValue == 1)
                        {
                            p.transform.GetChild(9).gameObject.SetActive(true);
                        }
                        else
                        {

                        }
                    }
                    

                    instantiatedRouteObjects.Add(p);
                    /**
                    int id = item.id;
                    string nome = item.name;
                    string cidade = item.city;
                    string categoria = item.category;
                    string rating = item.rating;
                    string imagem = item.image;

                    // Fa�a o que precisa com os dados de cada item
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

    private void GetRouteRate(int id)
    {
        rateRouteSelectedRouteId = id;
        Debug.Log("clicou rate");
        routeRatePainel.SetActive(true);
        //StartCoroutine(GetRoutesDetailsRequest(id));
    }

    private void GetPoiRate(int id)
    {
        ratePoiSelectedPoiId = id;
        Debug.Log("clicou rate poi: " + ratePoiSelectedPoiId);
        poiRatePainel.SetActive(true);
        //StartCoroutine(GetRoutesDetailsRequest(id));
    }

    private void GetRouteDetailsSelect(int id)
    {
        StartCoroutine(GetRoutesDetailsSelectRequest(id));
    }

    private IEnumerator GetRoutesDetailsRequest(int id)
    {
        
        string url = apiUrlRouteDetails + id;
        Debug.Log("routes get details da rota " + id + ", url: " + url);
        // Envia a solicita��o
        UnityWebRequest request = UnityWebRequest.Get(url);

        // Envia a solicita��o
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
                if(response.route.rating == 5)
                {
                    routeRatingDetails5.gameObject.SetActive(true);
                    routeRatingDetails4.gameObject.SetActive(false);
                    routeRatingDetails3.gameObject.SetActive(false);
                    routeRatingDetails2.gameObject.SetActive(false);
                    routeRatingDetails1.gameObject.SetActive(false);
                }
                else if (response.route.rating == 4)
                {
                    routeRatingDetails5.gameObject.SetActive(false);
                    routeRatingDetails4.gameObject.SetActive(true);
                    routeRatingDetails3.gameObject.SetActive(false);
                    routeRatingDetails2.gameObject.SetActive(false);
                    routeRatingDetails1.gameObject.SetActive(false);
                }
                else if (response.route.rating == 3)
                {
                    routeRatingDetails5.gameObject.SetActive(false);
                    routeRatingDetails4.gameObject.SetActive(false);
                    routeRatingDetails3.gameObject.SetActive(true);
                    routeRatingDetails2.gameObject.SetActive(false);
                    routeRatingDetails1.gameObject.SetActive(false);
                }
                else if (response.route.rating == 2)
                {
                    routeRatingDetails5.gameObject.SetActive(false);
                    routeRatingDetails4.gameObject.SetActive(false);
                    routeRatingDetails3.gameObject.SetActive(false);
                    routeRatingDetails2.gameObject.SetActive(true);
                    routeRatingDetails1.gameObject.SetActive(false);
                }
                else if (response.route.rating == 1)
                {
                    routeRatingDetails5.gameObject.SetActive(false);
                    routeRatingDetails4.gameObject.SetActive(false);
                    routeRatingDetails3.gameObject.SetActive(false);
                    routeRatingDetails2.gameObject.SetActive(false);
                    routeRatingDetails1.gameObject.SetActive(true);
                }
                else
                {
                    routeRatingDetails5.gameObject.SetActive(false);
                    routeRatingDetails4.gameObject.SetActive(false);
                    routeRatingDetails3.gameObject.SetActive(false);
                    routeRatingDetails2.gameObject.SetActive(false);
                    routeRatingDetails1.gameObject.SetActive(false);
                }

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

                    Image imageComponent = p.transform.GetChild(1).GetComponent<Image>();
                    LoadImageFromBase64(poi.image, imageComponent);

                    Button buttonComponentRatePoi = p.transform.GetChild(12).GetComponent<Button>();
                    buttonComponentRatePoi.onClick.AddListener(() => { GetPoiRate(poi.id); });

                    if (poi.rating == 5)
                    {
                        // Ativar a imagem no filho 5
                        p.transform.GetChild(7).gameObject.SetActive(true);
                    }
                    else if (poi.rating == 4)
                    {
                        p.transform.GetChild(8).gameObject.SetActive(true);
                    }
                    else if (poi.rating == 3)
                    {
                        p.transform.GetChild(9).gameObject.SetActive(true);
                    }
                    else if (poi.rating == 2)
                    {
                        p.transform.GetChild(10).gameObject.SetActive(true);
                    }
                    else if (poi.rating == 1)
                    {
                        p.transform.GetChild(11).gameObject.SetActive(true);
                    }
                    else
                    {

                    }

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
        // Envia a solicita��o
        UnityWebRequest request = UnityWebRequest.Get(url);

        // Envia a solicita��o
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
            //StartCoroutine(CheckDistanceCoroutine(latitudeUser, longitudeUser, latitudePoi, longitudePoi));
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
            message = $"Para iniciar a rota tur�stica de {routeName}, � necess�rio estar na cidade de {cityName}, no ponto de interesse {poiName} que se situa aproximadamente nas coordenadas geogr�ficas [{latitudePoi} , {longitudePoi}]. Voc� pode usar um aplicativo de mapa ou dispositivo de GPS para navegar at� � localidade. Assim que chegar, a aplica��o ir� encarregar-se de o levar para o POI, de forma a poder iniciar a rota tur�stica.\n\nEspero que tenha uma excelente explora��o ao longo desta rota! Aproveite cada momento e descubra as maravilhas ao longo do caminho.";
            message2 = $"O bot�o iniciar s� ser� habilitado quando voc� estiver a 5 metros ou menos do ponto de interesse {poiName}";
        }
        selectRouteExplanation.text = message;
        selectRouteExplanation2.text = message2;
        selectRouteName.text = routeName;

        LoadImageFromBase64(routeImage, imageRouteSelected);

        startRouteExploration.onClick.AddListener(() => CreateTouristicRoute(routeId));
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
                Debug.Log("O usu�rio est� perto do ponto de interesse.");
                isUserCloseToPoi = true;
                Debug.Log("est� proximo 2");
                Image buttonImage = startRouteExploration.GetComponent<Image>();
                Color newColor = new Color(0.24f, 0.70f, 0.44f);
                buttonImage.color = newColor;
                startRouteExploration.interactable = true;
                // Fa�a o que for necess�rio quando o usu�rio estiver perto do ponto de interesse
            }
            else
            {
                isUserCloseToPoi = false;
                Debug.Log("O usu�rio est� longe do ponto de interesse.");
                Image buttonImage = startRouteExploration.GetComponent<Image>();
                Color newColor = new Color(0f, 0.35f, 0.16f);
                buttonImage.color = newColor;
                startRouteExploration.interactable = false;
            }
            yield return new WaitForSeconds(1f);
        }
    }

    // M�todo para calcular a dist�ncia entre dois pontos usando a f�rmula de haversine
    public bool IsWithinDistance(double lat1, double lon1, double lat2, double lon2, double distanceThreshold)
    {
        const double earthRadius = 6371000; // raio da Terra em metros

        // Converte graus para radianos
        double dLat = DegreesToRadians(lat2 - lat1);
        double dLon = DegreesToRadians(lon2 - lon1);

        // F�rmula de Haversine
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        // Dist�ncia em metros
        Debug.Log("distance: " + earthRadius * c);
        double distance = earthRadius * c;

        // Verifica se a dist�ncia est� dentro do limite especificado
        return distance <= distanceThreshold;
    }

    // M�todo auxiliar para converter graus em radianos
    public double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }

    public void CreateTouristicRoute(int routeId)
    {

        //PlayerPrefs.SetInt("choosenRoute", routeId);
        //PlayerPrefs.Save();
        //SceneManager.LoadScene("SampleScene");
        int touristId = PlayerPrefs.GetInt("Current_Logged_TouristID", -1);
        StartCoroutine(CreateTouristicRouteCoroutine(routeId, touristId));
    }

    private IEnumerator CreateTouristicRouteCoroutine(int route_id, int tourist_id)
    {
        string uri = "http://13.60.19.19:3000/api/route/touristic-route/add";

        WWWForm form = new WWWForm();
        form.AddField("route_id", route_id);
        form.AddField("tourist_id", tourist_id);

        Debug.Log("add touristic route com id " + route_id + ", tourist_id: " + tourist_id);

        using (UnityWebRequest www = UnityWebRequest.Post(uri, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Erro ao registar: " + www.error);
            }
            else
            {
                string responseText = www.downloadHandler.text;
                TouristicRoutesResponse responseData = JsonUtility.FromJson<TouristicRoutesResponse>(responseText);

                if (responseData != null && responseData.message != null)
                {
                    Debug.Log("cria��o de touristic route bem sucedida " + responseData.message + ", touristic route id: " + responseData.touristicRouteId);
                    PlayerPrefs.SetInt("choosenRoute", route_id);
                    PlayerPrefs.SetInt("touristic_route", responseData.touristicRouteId);
                    PlayerPrefs.Save();
                    SceneManager.LoadScene("SampleScene");

                }
                else
                {
                    Debug.LogError("Erro ao touristic route: " + responseData.message);
                }
            }
        }
    }

    public void BackFromSelectRoute()
    {
        isCheckingDistance = false;
        routeSelectPainel.SetActive(false);
    }
    public void BackFromRateRoute()
    {
        routeRatePainel.SetActive(false);
    }
    public void BackFromRatePoi()
    {
        poiRatePainel.SetActive(false);
    }
    void DropdownRateValueChanged(Dropdown change)
    {
        int selectedValue = change.value;
        rateRouteValue = selectedValue;
        Debug.Log("Item selecionado: " + rateRouteValue);
    }

    void DropdownRatePoiValueChanged(Dropdown change)
    {
        int selectedValue = change.value;
        ratePoiValue = selectedValue;
        Debug.Log("Item selecionado poi: " + ratePoiValue);
    }

    void SubmitRouteRate()
    {
        Debug.Log("clicou submit route rate: " + rateRouteValue + ", rateRouteSelectedRouteId: " + rateRouteSelectedRouteId);
        int touristId = PlayerPrefs.GetInt("Current_Logged_TouristID", -1);
        StartCoroutine(SubmitRouteRateCoroutine(rateRouteSelectedRouteId, touristId, rateRouteValue));
    }

    private IEnumerator SubmitRouteRateCoroutine(int route_id, int tourist_id, int rating)
    {
        int ratingRoute = 0;
        if(rating == 0)
        {
            ratingRoute = 5;
        }
        else if(rating == 1)
        {
            ratingRoute = 4;
        }
        else if(rating == 2)
        {
            ratingRoute = 3;
        }
        else if (rating == 3)
        {
            ratingRoute = 2;
        }
        else if (rating == 4)
        {
            ratingRoute = 1;
        }

        string uri = "http://13.60.19.19:3000/api/user/rating/route";

        WWWForm form = new WWWForm();
        form.AddField("tourist_id", tourist_id);
        form.AddField("route_id", route_id);
        form.AddField("rating", ratingRoute);

        Debug.Log("rate route com id " + route_id + ", tourist_id: " + tourist_id + ", rating: " + ratingRoute);

        using (UnityWebRequest www = UnityWebRequest.Post(uri, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Erro ao dar rate: " + www.error);
                messageErrorRateRoute.gameObject.SetActive(true);
                yield return new WaitForSeconds(3f);
                messageErrorRateRoute.gameObject.SetActive(false);
            }
            else
            {
                string responseText = www.downloadHandler.text;
                RateRouteResponse responseData = JsonUtility.FromJson<RateRouteResponse>(responseText);

                if (responseData != null && responseData.message != null)
                {
                    Debug.Log("rating bem sucedida " + responseData.message + ", rate: " + responseData.roundedAverageRating);
                    routeRatePainel.SetActive(false);
                    messageSucessRateRoute.gameObject.SetActive(true);
                    yield return new WaitForSeconds(3f);
                    messageSucessRateRoute.gameObject.SetActive(false);

                }
                else
                {
                    Debug.LogError("Erro aorating: " + responseData.message);
                    messageErrorRateRoute.gameObject.SetActive(true);
                    yield return new WaitForSeconds(3f);
                    messageErrorRateRoute.gameObject.SetActive(false);
                }
            }
        }
    }

    void SubmitPoiRate()
    {
        Debug.Log("clicou submit poi rate: " + ratePoiValue + ", ratePoiSelectedPoiId: " + ratePoiSelectedPoiId);
        int touristId = PlayerPrefs.GetInt("Current_Logged_TouristID", -1);
        StartCoroutine(SubmitPoiRateCoroutine(ratePoiSelectedPoiId, touristId, ratePoiValue));
    }

    private IEnumerator SubmitPoiRateCoroutine(int poi_id, int tourist_id, int rating)
    {
        int ratingPoi = 0;
        if (rating == 0)
        {
            ratingPoi = 5;
        }
        else if (rating == 1)
        {
            ratingPoi = 4;
        }
        else if (rating == 2)
        {
            ratingPoi = 3;
        }
        else if (rating == 3)
        {
            ratingPoi = 2;
        }
        else if (rating == 4)
        {
            ratingPoi = 1;
        }

        string uri = "http://13.60.19.19:3000/api/user/rating/poi";

        WWWForm form = new WWWForm();
        form.AddField("tourist_id", tourist_id);
        form.AddField("poi_id", poi_id);
        form.AddField("rating", ratingPoi);

        Debug.Log("rate poi com id " + poi_id + ", tourist_id: " + tourist_id + ", rating: " + ratingPoi);

        using (UnityWebRequest www = UnityWebRequest.Post(uri, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Erro ao dar rate: " + www.error);
                messageErrorRatePoi.gameObject.SetActive(true);
                yield return new WaitForSeconds(3f);
                messageErrorRatePoi.gameObject.SetActive(false);
            }
            else
            {
                string responseText = www.downloadHandler.text;
                RateRouteResponse responseData = JsonUtility.FromJson<RateRouteResponse>(responseText);

                if (responseData != null && responseData.message != null)
                {
                    Debug.Log("rating bem sucedida " + responseData.message + ", rate: " + responseData.roundedAverageRating);
                    poiRatePainel.SetActive(false);
                    messageSucessRatePoi.gameObject.SetActive(true);
                    yield return new WaitForSeconds(3f);
                    messageSucessRatePoi.gameObject.SetActive(false);

                }
                else
                {
                    Debug.LogError("Erro aorating: " + responseData.message);
                    messageErrorRatePoi.gameObject.SetActive(true);
                    yield return new WaitForSeconds(3f);
                    messageErrorRatePoi.gameObject.SetActive(false);
                }
            }
        }
    }
}
