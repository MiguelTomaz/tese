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

public class RouteExploration : MonoBehaviour
{
    [SerializeField] private AREarthManager earthManager;
    public GameObject prefabLivraria;
    public GameObject prefabUni;
    public GameObject prefabClerigos;
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

    //current poi
    public Text currentPoiName;
    public Text currentPoiLat;
    public Text currentPoiLong;
    public Text currentPoiDistance;

    //next poi
    public Text nextPoiName;
    public Text nextPoiLat;
    public Text nextPoiLong;
    public Text nextPoiDistance;
    void Start()
    {
        POI_prefabs = new Dictionary<int, GameObject>();
        POI_prefabs.Add(3, prefabLivraria);
        POI_prefabs.Add(4, prefabUni);
        POI_prefabs.Add(8, prefabClerigos);

        savedLanguage = PlayerPrefs.GetString("Language", "en");
        int touristId = PlayerPrefs.GetInt("Current_Logged_TouristID", -1); // -1 é o valor padrão se a chave "UserID" não existir
        int choosenRoute = PlayerPrefs.GetInt("choosenRoute", -1); // -1 é o valor padrão se a chave "UserID" não existir
        GetRouteDetails(choosenRoute);

        openRouteExplorationDetailsPainelBtn.onClick.AddListener(openPainelRouteExploration);
        closeRouteExplorationDetailsPainelBtn.onClick.AddListener(closePainelRouteExploration);
        addOrderPoiTesteBtn.onClick.AddListener(addRoutePoiOrder);
    }

    // Update is called once per frame
    void Update()
    {
        
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

    private void PlaceObjects()
    {
        if (earthManager.EarthTrackingState == TrackingState.Tracking)
        {
            var geospatialPose = earthManager.CameraGeospatialPose;


            foreach (var obj in geospatialObjects)
            {
                var eartPosition = obj.earthPosition;
                var objAnchor = ARAnchorManagerExtensions.AddAnchor(aRAnchorManager, eartPosition.latitude, eartPosition.longitude, eartPosition.altitude, Quaternion.identity);
                var objAnchor2 = ARAnchorManagerExtensions.AddAnchor(aRAnchorManager, 41.756905, -7.462092, eartPosition.altitude, Quaternion.identity);
                Instantiate(obj.objectPrefab, objAnchor.transform);

            }
        }

        else if (earthManager.EarthTrackingState == TrackingState.None)
        {
            Debug.Log("TrackingState.None");
            Invoke("PlaceObjects", 5.0f);
        }
    }
}
