using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RouteExploration : MonoBehaviour
{
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


    // Start is called before the first frame update
    private string apiUrlRouteDetails = "http://localhost:3000/api/route/details/";
    public Text exploringRouteText;
    public Text RouteName;
    public Text RouteCity;
    public Text RouteCategory;
    void Start()
    {
        int touristId = PlayerPrefs.GetInt("Current_Logged_TouristID", -1); // -1 é o valor padrão se a chave "UserID" não existir
        Debug.Log("touristId: " + touristId);

        int choosenRoute = PlayerPrefs.GetInt("choosenRoute", -1); // -1 é o valor padrão se a chave "UserID" não existir
        Debug.Log("choosenRoute: " + choosenRoute);
        GetRouteDetails(choosenRoute);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GetRouteDetails(int id)
    {
        Debug.Log("aqui");
        StartCoroutine(GetRoutesDetailsRequest(id));
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

            }
            else
            {
                Debug.Log("No route details found in the response.");
            }
           
        }
    }
}
