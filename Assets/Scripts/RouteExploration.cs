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

public class RouteExploration : MonoBehaviour
{
    [SerializeField] private AREarthManager earthManager;
    public GameObject prefabLivraria;
    public GameObject prefabUni;
    public GameObject prefabClerigos;

    public GameObject arrowNorth;
    public GameObject arrowPoi;
    public TextMeshProUGUI northText;
    public TextMeshProUGUI poiDistanceText;

    public TextMeshProUGUI coordinatesLatPoiTeste;
    public TextMeshProUGUI coordinatesLongPoiTeste;

    private bool canStartExploration = false;

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
    private bool isEplorationBegin = false;
    private string apiUrlRouteDetails = "http://13.60.19.19:3000/api/route/details/";
    private List<PointOfInterest> poiListWithOrder = new List<PointOfInterest>();
    private string savedLanguage = "";
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
        // Ativa a bússola
        Input.location.Start();
        Input.compass.enabled = true;

        POI_prefabs = new Dictionary<int, GameObject>();
        POI_prefabs.Add(3, prefabLivraria);
        POI_prefabs.Add(4, prefabUni);
        POI_prefabs.Add(5, prefabClerigos);

        savedLanguage = PlayerPrefs.GetString("Language", "en");
        int touristId = PlayerPrefs.GetInt("Current_Logged_TouristID", -1); // -1 é o valor padrão se a chave "UserID" não existir
        int choosenRoute = PlayerPrefs.GetInt("choosenRoute", -1); // -1 é o valor padrão se a chave "UserID" não existir
        GetRouteDetails(choosenRoute);

        openRouteExplorationDetailsPainelBtn.onClick.AddListener(openPainelRouteExploration);
        closeRouteExplorationDetailsPainelBtn.onClick.AddListener(closePainelRouteExploration);
        addOrderPoiTesteBtn.onClick.AddListener(addRoutePoiOrder);
        VerifyGeospatialSupport();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(canStartExploration)
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
            currentPoiDistance.text = currentDistance + " m";

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
        openRouteExplorationDetailsPainelBtn.gameObject.SetActive(false);
        StartCoroutine(GetRoutesDetailsRequest(id));
    }

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
                        string message = "";
                        if (savedLanguage == "en")
                        {
                            message = "Your distance to the current POI: " + currentDistance.ToString("F2") + " m";
                        }
                        else
                        {
                            message = "A tua distancia ao POI atual: " + currentDistance.ToString("F2") + " m";
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
                        if (savedLanguage == "en")
                        {
                            message = "Your distance to the next POI: " + nextDistance.ToString("F2") + " m";
                        }
                        else
                        {
                            message = "A tua distancia ao POI seguinte: " + nextDistance.ToString("F2") + " m";
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
}
