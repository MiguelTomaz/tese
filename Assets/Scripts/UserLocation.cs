using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;

public class UserLocation : MonoBehaviour
{
    private float timeSinceLastUpdate = 0f;
    private float updateInterval = 5f; // Intervalo desejado em segundos
    public TextMeshProUGUI userLocationText;

    private double lat;
    private double lon;

    private double latitudeObjectHouse = 41.756947;
    private double longitudeObjectHouse = -7.4620061;
    public GameObject cilindroUI;

    void Start()
    {
        StartCoroutine(GPSUserLocation());
    }

    // Update is called once per frame
    void Update()
    {
        // Atualiza o tempo decorrido

    }

    IEnumerator GPSUserLocation()
    {

        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            Permission.RequestUserPermission(Permission.CoarseLocation);
        }

        if (!Input.location.isEnabledByUser)
        {
            //Debug.Log("location is not Enabled By User");
            userLocationText.text = "location is not Enabled By User";

            //start map with default coordinates
            //Debug.Log("start map with default coordinates");

            yield break;
        }

        //start service
        Input.location.Start();
        userLocationText.text = "loading map...please wait";
        //wait until service initialize
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        //service didnt init at 20s
        if (maxWait < 1)
        {
            //Debug.Log("erro maxWaint < 1");
            userLocationText.text = "erro maxWaint < 1";
            yield break;
        }

        //conection failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            //Debug.Log("unnable to determine device location");
            userLocationText.text = "unnable to determine device location";

            //start map with default coordinates
            //Debug.Log("start map with default coordinates");
            yield break;
        }
        else
        {
            //Debug.Log("running");
            InvokeRepeating("UpdateGPSData", 0.5f, 1f);
            //access granted
        }
    }

    private void UpdateGPSData()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            //Debug.Log("running");
            lon = Input.location.lastData.longitude;
            lat = Input.location.lastData.latitude;

            userLocationText.text = "lon: " + lon + ", lat: " + lat;
            PlayerPrefs.SetString("UserLatitude", lat.ToString());
            PlayerPrefs.SetString("UserLongitude", lon.ToString());
            PlayerPrefs.Save();
            CalcularRotacao();

            //start map with the user coordinates
            //Debug.Log("start map with user coordinates");
        }

        else
        {
            //Debug.Log("stopped");
        }
    }

    void CalcularRotacao()
    {
        double deltaLon = longitudeObjectHouse - lon;
        double deltaLat = latitudeObjectHouse - lat;

        // Calcula o ângulo em radianos usando a função Atan2
        float angleRad = Mathf.Atan2((float)deltaLon, (float)deltaLat);

        // Converte o ângulo para graus e adiciona 180 para que a seta aponte na direção certa
        float angleDegrees = angleRad * Mathf.Rad2Deg + 180f;

        // Define a rotação Z da imagem2Dseta
        cilindroUI.transform.rotation = Quaternion.Euler(0f, 0f, angleDegrees);
    }
}
