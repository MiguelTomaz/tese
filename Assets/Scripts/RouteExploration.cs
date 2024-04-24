using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Google.XR.ARCoreExtensions;
using TMPro;
using System.Security.Cryptography;
using UnityEngine.SceneManagement;

public class RouteExploration : MonoBehaviour
{
    [SerializeField] private AREarthManager earthManager;
    public GameObject prefabLivraria;
    public GameObject prefabUni;
    public GameObject prefabClerigos;
    public GameObject addDescriptionPhotoPainel;
    public GameObject routeDetailsPainel;
    public GameObject currentPoiDetailsPainel;
    public GameObject livrariaPoiDetails;

    public GameObject arrowNorth;
    public GameObject arrowPoi;
    public TextMeshProUGUI northText;
    public TextMeshProUGUI poiDistanceText;
    public TMP_InputField photoDescriptionInputField;
    public TextMeshProUGUI poiDistancePointerText;
    public TextMeshProUGUI coordinatesLatPoiTeste;
    public TextMeshProUGUI coordinatesLongPoiTeste;
    public TextMeshProUGUI clickPhotoTest;

    public Text photoAddedSuccessMessage;
    public Text photoAddedErrorMesage;
    public Text currentPoiDetailsName;
    public GameObject poiContainerRouteDetails;
    private GameObject poiTemplateRouteDetails;

    private bool canStartExploration = false;
    private double poiHeight = 1100;
    private double poiNumber;

    public Dictionary<int, GameObject> POI_prefabs;

    [Serializable]
    public struct GeospatialObject
    {
        public GameObject objectPrefab;
        public EarthPosition earthPosition;
    }

    [Serializable]
    public struct EarthPosition
    {
        public double latitude;
        public double longitude;
        public double altitude;
    }

    [SerializeField] private ARAnchorManager aRAnchorManager;

    [SerializeField] private List<GeospatialObject> geospatialObjects = new List<GeospatialObject>();

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
        public int Order;
    }


    // Start is called before the first frame update
    public GameObject routeExplorationDetailsPainel;
    public Button addOrderPoiTesteBtn;
    public Button openRouteExplorationDetailsPainelBtn;
    public Button closeRouteExplorationDetailsPainelBtn;
    public Button savePhotoButton;
    public Button takePhotoButton;
    public Button closeTakePhotoButton;
    public Button openRouteDetailsButton;
    public Button closeRouteDetailsButton;
    public Button openCurrentPoiDetails;
    public Button closeCurrentPoiDetails;
    public Button leaveExplorationButton;
    private bool isEplorationBegin = false;
    private string apiUrlRouteDetails = "http://13.60.19.19:3000/api/route/details/";
    private List<PointOfInterest> poiListWithOrder = new List<PointOfInterest>();
    private string savedLanguage = "";
    private int touristRouteId;
    private int choosenRoute;

    private int routePoiCurrentOrder = 0;
    public Text exploringRouteText;
    public Text RouteName;
    public Text RouteCity;
    public Text RouteCategory;
    public Text latitudeUserText;
    public Text longitudeUserText;
    public Text readyToUseText;

    //current poi
    public Text currentPoiName;
    public Text currentPoiLat;
    public Text currentPoiLong;
    private double currentPoiLatitude = 0;
    private double currentPoiLongitude = 0;
    public Text currentPoiDistance;

    //next poi
    public Text nextPoiName;
    public Text nextPoiLat;
    public Text nextPoiLong;
    public Text nextPoiDistance;
    void Start()
    {
        photoAddedSuccessMessage.gameObject.SetActive(false);
        photoAddedErrorMesage.gameObject.SetActive(false);
        currentPoiDetailsPainel.SetActive(false);

        addDescriptionPhotoPainel.SetActive(false);
        // Ativa a bússola
        Input.location.Start();
        Input.compass.enabled = true;

        POI_prefabs = new Dictionary<int, GameObject>();
        POI_prefabs.Add(3, prefabLivraria);
        POI_prefabs.Add(4, prefabUni);
        POI_prefabs.Add(5, prefabClerigos);

        savedLanguage = PlayerPrefs.GetString("Language", "en");
        int touristId = PlayerPrefs.GetInt("Current_Logged_TouristID", -1); // -1 é o valor padrão se a chave "UserID" não existir
        choosenRoute = PlayerPrefs.GetInt("choosenRoute", -1); // -1 é o valor padrão se a chave "UserID" não existir

        touristRouteId = PlayerPrefs.GetInt("touristic_route", -1);

        GetRouteDetails(choosenRoute);

        openRouteExplorationDetailsPainelBtn.onClick.AddListener(openPainelRouteExploration);
        closeRouteExplorationDetailsPainelBtn.onClick.AddListener(closePainelRouteExploration);
        addOrderPoiTesteBtn.onClick.AddListener(addRoutePoiOrder);
        VerifyGeospatialSupport();

        takePhotoButton.onClick.RemoveListener(TakePhoto);
        takePhotoButton.onClick.AddListener(TakePhoto);

        closeTakePhotoButton.onClick.RemoveListener(closeTakePhoto);
        closeTakePhotoButton.onClick.AddListener(closeTakePhoto);

        openRouteDetailsButton.onClick.AddListener(openRouteDetailsPainel);
        closeRouteDetailsButton.onClick.AddListener(closeRouteDetailsPainel);

        openCurrentPoiDetails.onClick.AddListener(openCurrentPoiDetailsPainel);
        closeCurrentPoiDetails.onClick.AddListener(closeCurrentPoiDetailsPainel);

        leaveExplorationButton.onClick.AddListener(LeaveExploration);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var poi in poiListWithOrder)
        {
            if (poi.Order == routePoiCurrentOrder)
            {
                if (poi.id == 3) //liv teste
                {
                    livrariaPoiDetails.SetActive(true);
                }
                currentPoiDetailsName.text = poi.name;
            }

        }

        if (canStartExploration)
        {
            // Obtém a rotação da bússola
            float magneticNorth = Input.compass.magneticHeading;

            // Ajusta para a declinação magnética (se necessário)
            float trueNorth = magneticNorth - Input.compass.trueHeading;
            float trueNorthAarrowDirection = Input.compass.trueHeading;
            trueNorthAarrowDirection = (float)Math.Round(trueNorthAarrowDirection, 2);
            arrowNorth.transform.rotation = Quaternion.Euler(0f, 0f, trueNorthAarrowDirection);
            // Orienta a câmera para o norte verdadeiro
            transform.rotation = Quaternion.Euler(0, trueNorth, 0);


            string latitudeString = PlayerPrefs.GetString("UserLatitude");
            string longitudeString = PlayerPrefs.GetString("UserLongitude");

            var x1 = -7.4622632;
            var y1 = 41.7568188;

            if (!string.IsNullOrEmpty(longitudeString))
            {
                x1 = double.Parse(longitudeString);
            }

            if (!string.IsNullOrEmpty(latitudeString))
            {
                y1 = double.Parse(latitudeString);
            }

            var x2 = -7.462006;
            var y2 = 41.756947;

            if (currentPoiLongitude != 0)
            {
                x2 = currentPoiLongitude;
                coordinatesLongPoiTeste.text = "long poi: " + currentPoiLongitude;

            }

            if (currentPoiLatitude != 0)
            {
                y2 = currentPoiLatitude;
                coordinatesLatPoiTeste.text = "lat poi: " + currentPoiLatitude;
            }

            double currentDistance = CalculateDistance(y1, x1, y2, x2);
            int roundedDistance = (int)Math.Floor(currentDistance);
            poiDistancePointerText.text = roundedDistance + " m";

            double delta_x = x2 - x1;
            double delta_y = y2 - y1;

            double[] vector_AB = { delta_x, delta_y };

            double angle_with_x_axis = Math.Atan2(delta_y, delta_x);

            // Converte o ângulo para graus
            double angle_degrees = angle_with_x_axis * (180 / Math.PI);

            //Console.WriteLine("Vetor AB: ({0}, {1})", vector_AB[0], vector_AB[1]);
            Debug.Log("angle_degrees: " + angle_degrees);

            double angle_degrees_Y = 90 - angle_degrees;
            double anglePoint = trueNorthAarrowDirection - angle_degrees_Y;

            arrowPoi.transform.rotation = Quaternion.Euler(0f, 0f, (float)anglePoint);


            if (trueNorthAarrowDirection >= 340 || trueNorthAarrowDirection <= 20)
            {
                northText.text = "N";
            }
            else if (trueNorthAarrowDirection > 20 && trueNorthAarrowDirection <= 60)
            {
                northText.text = "NE";
            }
            else if (trueNorthAarrowDirection > 60 && trueNorthAarrowDirection <= 110)
            {
                northText.text = "E";
            }
            else if (trueNorthAarrowDirection > 110 && trueNorthAarrowDirection <= 140)
            {
                northText.text = "SE";
            }
            else if (trueNorthAarrowDirection > 140 && trueNorthAarrowDirection <= 200)
            {
                northText.text = "S";
            }
            else if (trueNorthAarrowDirection > 200 && trueNorthAarrowDirection <= 240)
            {
                northText.text = "SW";
            }
            else if (trueNorthAarrowDirection > 240 && trueNorthAarrowDirection <= 290)
            {
                northText.text = "W";
            }
            else if (trueNorthAarrowDirection > 290 && trueNorthAarrowDirection < 340)
            {
                northText.text = "NW";
            }
        }
        else
        {
            coordinatesLongPoiTeste.text = "error";
        }
        
    }

    private void GetRouteDetails(int id)
    {
        if(choosenRoute == -1)
        {
            choosenRoute = 2;
        }
        openRouteExplorationDetailsPainelBtn.gameObject.SetActive(false);
        StartCoroutine(GetRoutesDetailsRequest(id));
    }

    private List<GameObject> instantiatedPOIObjects = new List<GameObject>();
    private IEnumerator GetRoutesDetailsRequest(int id)
    {

        string url = apiUrlRouteDetails + id;
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
            RouteDetailsResponse response = JsonUtility.FromJson<RouteDetailsResponse>(jsonResponse);

            if (response != null)
            {
                Debug.Log("route: " + choosenRoute);
                Debug.Log("route name " + response.route.name);
                RouteName.text = response.route.name;
                RouteCity.text = response.route.city;
                RouteCategory.text = response.route.category;

                poiTemplateRouteDetails = poiContainerRouteDetails.transform.GetChild(0).gameObject;
                GameObject p;
                foreach (var poi in response.poiList)
                {
                    Debug.Log("ID do poi: " + poi.id);
                    Debug.Log("Nome do poi: " + poi.name);
                    p = Instantiate(poiTemplateRouteDetails, poiContainerRouteDetails.transform);
                    poiNumber = response.poiList.Count;
                    AdjustContainerPOIHeight(poiNumber);
                    p.transform.GetChild(0).GetComponent<Text>().text = poi.name;
                    p.transform.GetChild(2).GetComponent<Text>().text = poi.description;

                    instantiatedPOIObjects.Add(p);
                }
                poiTemplateRouteDetails.SetActive(false);

                string exploringRoute = "";
                string savedLanguage = PlayerPrefs.GetString("Language", "en");
                if (savedLanguage == "en")
                {
                    exploringRoute = $"Exploring {response.route.name}";
                }
                else
                {
                    exploringRoute = $"Explorando {response.route.name}";
                }
                exploringRouteText.text = exploringRoute;

                string latitudeString = PlayerPrefs.GetString("UserLatitude");
                string longitudeString = PlayerPrefs.GetString("UserLongitude");


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

                latitudeUserText.text = "lat: " + latitudeUser;
                longitudeUserText.text = "lat: " + longitudeUser;

                if(isEplorationBegin == false)
                {
                    Debug.Log("isEplorationBegin == falase");
                    int order = 0;
                    foreach (var poi in response.poiList)
                    {
                        Debug.Log("poiListWithOrder: " + poi.name);
                        poi.Order = order;
                        poiListWithOrder.Add(poi);
                        order++;
                    }
                    foreach (PointOfInterest poi in poiListWithOrder)
                    {
                        EarthPosition earthPosition = new EarthPosition
                        {
                            latitude = poi.latitude,
                            longitude = poi.longitude,
                            altitude = poi.altitude
                        };

                        if (POI_prefabs.TryGetValue(poi.id, out GameObject prefab))
                        {
                            GeospatialObject geospatialObject = new GeospatialObject
                            {
                                objectPrefab = prefab,
                                earthPosition = earthPosition
                            };

                            geospatialObjects.Add(geospatialObject);
                        }

                    }
                }
                foreach (var poi in poiListWithOrder)
                {
                    Debug.Log("poi order: " + poi.Order + ", current order: " + routePoiCurrentOrder);
                    if(poi.Order == routePoiCurrentOrder)
                    {
                        currentPoiName.text = poi.name;
                        currentPoiLat.text = "lat: " + poi.latitude;
                        currentPoiLong.text = "long: " +poi.longitude;

                        currentPoiLatitude = poi.latitude;
                        currentPoiLongitude = poi.longitude;

                        double currentDistance = CalculateDistance(latitudeUser, longitudeUser, poi.latitude, poi.longitude);
                        int roundedDistance = (int)Math.Floor(currentDistance);
                        string message = "";
                        if (savedLanguage == "en")
                        {
                            message = "Your distance to the current POI: " + roundedDistance + " m";
                        }
                        else
                        {
                            message = "A tua distancia ao POI atual: " + roundedDistance + " m";
                        }
                        Debug.Log("distancia current: " + currentDistance);
                        currentPoiDistance.text = message;

                    }
                    if(poi.Order == routePoiCurrentOrder + 1)
                    {
                        nextPoiName.text = poi.name;
                        nextPoiLat.text = "lat: " + poi.latitude;
                        nextPoiLong.text = "long: " + poi.longitude;
                        string message = "";
                        double nextDistance = CalculateDistance(latitudeUser, longitudeUser, poi.latitude, poi.longitude);
                        int roundedDistance = (int)Math.Floor(nextDistance);
                        if (savedLanguage == "en")
                        {
                            message = "Your distance to the next POI: " + roundedDistance + " m";
                        }
                        else
                        {
                            message = "A tua distancia ao POI seguinte: " + roundedDistance + " m";
                        }
                        nextPoiDistance.text = message;
                    }
                    else
                    {
                        nextPoiName.text = "no more POI";
                        nextPoiLat.text = "";
                        nextPoiLong.text = "";
                    }
                }
            }
            else
            {
                Debug.Log("No route details found in the response.");
            }
           
        }
    }

    public void closePainelRouteExploration()
    {
        openRouteExplorationDetailsPainelBtn.gameObject.SetActive(true);
        routeExplorationDetailsPainel.SetActive(false);
    }

    public void openPainelRouteExploration()
    {
        isEplorationBegin = true;
        Debug.Log("routePoiCurrentOrder: " + routePoiCurrentOrder);
        int choosenRoute = PlayerPrefs.GetInt("choosenRoute", -1); // -1 é o valor padrão se a chave "UserID" não existir
        GetRouteDetails(choosenRoute);
        routeExplorationDetailsPainel.SetActive(true);
    }
    public void addRoutePoiOrder()
    {
        Debug.Log("click add poi current order. before: " + routePoiCurrentOrder);
        routePoiCurrentOrder = routePoiCurrentOrder + 1;
        Debug.Log("after click add poi current order. after: " + routePoiCurrentOrder);
    }

    public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
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
        return distance;
    }

    public double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }

    private void VerifyGeospatialSupport()
    {
        var result = earthManager.IsGeospatialModeSupported(GeospatialMode.Enabled);
        switch (result)
        {
            case FeatureSupported.Supported:
                Debug.Log("ready to use VPS");
                readyToUseText.text = "ready to use VPS";
                PlaceObjects();
                break;

            case FeatureSupported.Unknown:
                Debug.Log("unknown");
                readyToUseText.text = "unknown";
                Invoke("VerifyGeospatialSupport", 5.0f);
                break;

            case FeatureSupported.Unsupported:
                Debug.Log("vps unsuported");
                readyToUseText.text = "vps unsuported";
                break;
        }
    }

    private void PlaceObjects()
    {
        if (earthManager.EarthTrackingState == TrackingState.Tracking)
        {
            readyToUseText.text = "";
            var geospatialPose = earthManager.CameraGeospatialPose;


            foreach (var obj in geospatialObjects)
            {
                var eartPosition = obj.earthPosition;
                var objAnchor = ARAnchorManagerExtensions.AddAnchor(aRAnchorManager, eartPosition.latitude, eartPosition.longitude, eartPosition.altitude, Quaternion.identity);
                //var objAnchor2 = ARAnchorManagerExtensions.AddAnchor(aRAnchorManager, 41.756905, -7.462092, eartPosition.altitude, Quaternion.identity);
                Instantiate(obj.objectPrefab, objAnchor.transform);
                canStartExploration = true;
            }
        }

        else if (earthManager.EarthTrackingState == TrackingState.None)
        {
            Debug.Log("TrackingState.None");
            Invoke("PlaceObjects", 5.0f);
        }
    }

    public void TakePhoto()
    {
        clickPhotoTest.text = "clicou photo";
        
        StartCoroutine(Photo());
        addDescriptionPhotoPainel.SetActive(true);
    }

    public IEnumerator Photo()
    {
        clickPhotoTest.text = "clicou photo 1";
        Debug.Log("StartCoroutine");
        yield return new WaitForEndOfFrame();
        Camera camera = Camera.main;
        int width = Screen.width;
        int height = Screen.height;

        clickPhotoTest.text = "clicou photo 2";

        RenderTexture rt = new RenderTexture(width, height, 24);
        camera.targetTexture = rt;
        // The Render Texture in RenderTexture.active is the one
        // that will be read by ReadPixels.
        var currentRT = RenderTexture.active;
        RenderTexture.active = rt;

        // Render the camera's view.
        camera.Render();
        clickPhotoTest.text = "clicou photo 3";

        // Make a new texture and read the active Render Texture into it.
        Texture2D image = new Texture2D(width, height);
        image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        image.Apply();
        Debug.Log("image: " + image);
        camera.targetTexture = null;
        // Replace the original active Render Texture.
        RenderTexture.active = currentRT;

 
        clickPhotoTest.text = "clicou photo 4";
        byte[] bytes = image.EncodeToPNG();
        Debug.Log("bytes: " + bytes);
        string base64String = Convert.ToBase64String(bytes);
        
        string filename = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        Debug.Log("filename: " + filename);
        string imageHash = CalculateImageHash(image);
        Debug.Log("imageHash: " + imageHash);
       

        clickPhotoTest.text = "clicou photo 5";
        savePhotoButton.onClick.RemoveAllListeners();
        savePhotoButton.onClick.AddListener(() => { SubmitPhotoWithDescription(imageHash, filename, base64String); });

        Destroy(rt);
        Destroy(image);

    }

    public void SubmitPhotoWithDescription(string imageHash, string filename, string base64String)
    {
        clickPhotoTest.text = "clicou photo 6";
        //send this to save descriptino painel button
        int touristicRouteId = touristRouteId;
        if(touristicRouteId == -1)
        {
            touristicRouteId = 11;
        }
        string description = photoDescriptionInputField.text;
        if(string.IsNullOrEmpty(description))
        {
            description = "isNull";
        }
        DateTime date = DateTime.Now;

        /**
        if (!string.IsNullOrEmpty(description))
        {
            
        }
        else
        {
            Image buttonImage = savePhotoButton.GetComponent<Image>();
            Color newColor = new Color(0f, 0.35f, 0.16f);
            buttonImage.color = newColor;
            //savePhotoButton.interactable = false;
        }
        */
        clickPhotoTest.text = "clicou photo 7";
        UploadPhoto(touristicRouteId, description, date, imageHash, filename, base64String);
        addDescriptionPhotoPainel.SetActive(false);
    }

    string CalculateImageHash(Texture2D image)
    {
        byte[] imageBytes = image.EncodeToPNG();
        string hash;

        using (MD5 md5 = MD5.Create())
        {
            byte[] hashBytes = md5.ComputeHash(imageBytes);
            hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }

        return hash;
    }

    public void UploadPhoto(int touristRouteAssociationId, string description, DateTime date, string imageHash, string filename, string base64String)
    {
        clickPhotoTest.text = "clicou photo 8";
        string url = "http://13.60.19.19:3000/api/photo/upload";

        // Convertendo a imagem para bytes
        Debug.Log("UploadPhoto: ");
        // Criando o objeto de dados a serem enviados
        WWWForm form = new WWWForm();
        form.AddField("tourist_route_association_id", touristRouteAssociationId.ToString());
        form.AddField("description", description);
        form.AddField("date", date.ToString("yyyy-MM-ddTHH:mm:ss"));
        form.AddField("image_hash", imageHash);
        form.AddField("filename", filename);
        form.AddField("image_base64", base64String);

        clickPhotoTest.text = "clicou photo 9";

        // Enviando a requisição
        StartCoroutine(SendRequestPhoto(url, form));
    }

    IEnumerator SendRequestPhoto(string url, WWWForm form)
    {
        clickPhotoTest.text = "clicou photo 10";
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erro ao enviar a foto: " + www.error);
                clickPhotoTest.text = "clicou photo 11";

                photoAddedErrorMesage.gameObject.SetActive(true);
                yield return new WaitForSeconds(3f); // Exibir por 3 segundos
                photoAddedErrorMesage.gameObject.SetActive(false);
            }
            else
            {
                clickPhotoTest.text = "foto enviada";
                Debug.Log("Foto enviada com sucesso!");

                photoAddedSuccessMessage.gameObject.SetActive(true);
                yield return new WaitForSeconds(3f); // Exibir por 3 segundos
                photoAddedSuccessMessage.gameObject.SetActive(false);
            }
        }
    }
    
    void closeTakePhoto()
    {
        addDescriptionPhotoPainel.SetActive(false);
    }

    void openRouteDetailsPainel()
    {
        routeDetailsPainel.SetActive(true);
    }

    void closeRouteDetailsPainel()
    {
        routeDetailsPainel.SetActive(false);
    }

    void openCurrentPoiDetailsPainel()
    {
        currentPoiDetailsPainel.SetActive(true);
    }

    void closeCurrentPoiDetailsPainel()
    {
        currentPoiDetailsPainel.SetActive(false);
    }

    void AdjustContainerPOIHeight(double numberElements)
    {
        double totalHeight = numberElements * poiHeight + (numberElements - 1) * 50; // Calculating total height
        RectTransform containerRectTransform = poiContainerRouteDetails.GetComponent<RectTransform>();
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
    void LeaveExploration()
    {
        ClearInstantiatedPOIObjects();
        PlayerPrefs.SetInt("AfterExploration", 1);
        PlayerPrefs.Save(); // Salva as alterações
        SceneManager.LoadScene("Index");
    }
}
