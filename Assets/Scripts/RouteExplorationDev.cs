using Google.XR.ARCoreExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class RouteExplorationDev : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject painelRouteExplorationDev;
    public Button openPainelDev;
    public Button closePainelDev;

    public Text readyToUseText;
    public Text textLocation;
    public Text directionLocation;
    public Text cameraPoseText;
    public Text objectPoseText;
    public GameObject seta;
    public Text compassText;
    public Text compassText2;
    public GameObject arrow;


    private Vector3 cameraPoseObject;
    private Vector3 objectPoseObject;
    private Vector3 relativePositionObject;
    private Quaternion rotationObject;
    private string directionObject;

    //POI PORTO
    public GameObject prefabLivraria;
    public GameObject prefabUni;
    public GameObject prefabClerigos;
    public GameObject prefabFotografia;
    public GameObject prefabPuppet;
    public GameObject prefabPalacio;
    public GameObject prefabCityMuseum;
    public GameObject prefabRibeira;


    //POI CHAVES
    public GameObject prefabPonte;
    public GameObject prefabTermas;
    public GameObject prefabIgreja;
    public GameObject prefabCastelo;
    public GameObject prefabAguas;

    //BUTTONS OPEN POI
    //POI PORTO
    public Button ButtonLivraria;
    public Button ButtonUni;
    public Button ButtonClerigos;
    public Button ButtonFotografia;
    public Button ButtonPuppet;
    public Button ButtonPalacio;
    public Button ButtonCityMuseum;
    public Button ButtonRibeira;
    //POI CHAVES
    public Button ButtonPonte;
    public Button ButtonTermas;
    public Button ButtonIgreja;
    public Button ButtonCastelo;
    public Button ButtonAguas;

    [SerializeField] private AREarthManager earthManager;
    [SerializeField] private ARAnchorManager aRAnchorManager;
    [SerializeField] private ARCameraManager arCameraManager;

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


    [SerializeField] private List<GeospatialObject> geospatialObjects = new List<GeospatialObject>();
    public List<GameObject> poi_objects = new List<GameObject>();
    void Start()
    {
        // Ativa a bússola
        Input.location.Start();
        Input.compass.enabled = true;


        // Inicia a calibração (se necessário)
        closePainelDev.onClick.AddListener(ClosePainel);
        openPainelDev.onClick.AddListener(OpenPainel);
        // Adiciona os POIs de Porto à lista portoPOIs
        poi_objects.Add(prefabLivraria);
        poi_objects.Add(prefabUni);
        poi_objects.Add(prefabClerigos);
        poi_objects.Add(prefabFotografia);
        poi_objects.Add(prefabPuppet);
        poi_objects.Add(prefabPalacio);
        poi_objects.Add(prefabCityMuseum);
        poi_objects.Add(prefabRibeira);

        // Adiciona os POIs de Chaves à lista chavesPOIs
        poi_objects.Add(prefabPonte);
        poi_objects.Add(prefabTermas);
        poi_objects.Add(prefabIgreja);
        poi_objects.Add(prefabCastelo);
        poi_objects.Add(prefabAguas);

        EarthPosition earthPositionPoi = new EarthPosition
        {
            latitude = 41.756905,
            longitude = -7.462092,
            altitude = 433.118100
        };

        GeospatialObject geospatialObjectLivraria = new GeospatialObject
        {
            objectPrefab = prefabLivraria,
            earthPosition = earthPositionPoi
        };

        GeospatialObject geospatialObjectUni = new GeospatialObject
        {
            objectPrefab = prefabUni,
            earthPosition = earthPositionPoi
        };

        GeospatialObject geospatialObjectClerigos = new GeospatialObject
        {
            objectPrefab = prefabClerigos,
            earthPosition = earthPositionPoi
        };

        GeospatialObject geospatialObjectFotografia = new GeospatialObject
        {
            objectPrefab = prefabFotografia,
            earthPosition = earthPositionPoi
        };

        GeospatialObject geospatialObjectPuppet = new GeospatialObject
        {
            objectPrefab = prefabPuppet,
            earthPosition = earthPositionPoi
        };

        GeospatialObject geospatialObjectPalacio = new GeospatialObject
        {
            objectPrefab = prefabPalacio,
            earthPosition = earthPositionPoi
        };

        GeospatialObject geospatialObjectCityMuseum = new GeospatialObject
        {
            objectPrefab = prefabCityMuseum,
            earthPosition = earthPositionPoi
        };

        GeospatialObject geospatialObjectRibeira = new GeospatialObject
        {
            objectPrefab = prefabRibeira,
            earthPosition = earthPositionPoi
        };

        GeospatialObject geospatialObjectPonte = new GeospatialObject
        {
            objectPrefab = prefabPonte,
            earthPosition = earthPositionPoi
        };

        GeospatialObject geospatialObjectTermas = new GeospatialObject
        {
            objectPrefab = prefabTermas,
            earthPosition = earthPositionPoi
        };

        GeospatialObject geospatialObjectIgreja = new GeospatialObject
        {
            objectPrefab = prefabIgreja,
            earthPosition = earthPositionPoi
        };

        GeospatialObject geospatialObjectCastelo = new GeospatialObject
        {
            objectPrefab = prefabCastelo,
            earthPosition = earthPositionPoi
        };

        GeospatialObject geospatialObjectAguas = new GeospatialObject
        {
            objectPrefab = prefabAguas,
            earthPosition = earthPositionPoi
        };

        geospatialObjects.Add(geospatialObjectLivraria);
        
        geospatialObjects.Add(geospatialObjectUni);
        geospatialObjects.Add(geospatialObjectClerigos);
        geospatialObjects.Add(geospatialObjectFotografia);
        geospatialObjects.Add(geospatialObjectPuppet);
        geospatialObjects.Add(geospatialObjectPalacio);
        geospatialObjects.Add(geospatialObjectCityMuseum);
        geospatialObjects.Add(geospatialObjectRibeira);
        geospatialObjects.Add(geospatialObjectPonte);
        geospatialObjects.Add(geospatialObjectTermas);
        geospatialObjects.Add(geospatialObjectIgreja);
        geospatialObjects.Add(geospatialObjectCastelo);
        geospatialObjects.Add(geospatialObjectAguas);
        

        VerifyGeospatialSupport();
    }

    // Update is called once per frame
    void Update()
    {
        
        // Obtém a rotação da bússola
        float magneticNorth = Input.compass.magneticHeading;

        // Ajusta para a declinação magnética (se necessário)
        float trueNorth = magneticNorth - Input.compass.trueHeading;
        float trueNorthAarrowDirection = Input.compass.trueHeading;
        trueNorthAarrowDirection = (float)Math.Round(trueNorthAarrowDirection, 2);
        arrow.transform.rotation = Quaternion.Euler(0f, 0f, trueNorthAarrowDirection);
        // Orienta a câmera para o norte verdadeiro
        transform.rotation = Quaternion.Euler(0, trueNorth, 0);


        //compassText.text = "Norte: " + trueNorth.ToString("F2") + " graus" + ", rotation: " + transform.rotation;
        compassText.text = "norte verdadeiro: " + Input.compass.trueHeading.ToString("F2") + ", rotação seta: " + arrow.transform.rotation.eulerAngles.z;
        
        /**

        Quaternion cameraRotation = Quaternion.Euler(0, arCameraManager.transform.rotation.eulerAngles.y, 0);

        Quaternion compass = Quaternion.Euler(0, -Input.compass.magneticHeading, 0);

        Quaternion north = Quaternion.Euler(0, cameraRotation.eulerAngles.y + compass.eulerAngles.y, 0);
        transform.rotation = north;
        //compassText2.text = "norte2: " + north;


        // Obtém a rotação da câmera
        Quaternion cameraRotation2 = Camera.main.transform.rotation;

        // Obtém a rotação da bússola
        Quaternion compassRotation2 = Quaternion.Euler(0, -Input.compass.magneticHeading, 0);

        // Calcula a rotação combinada da câmera e da bússola
        Quaternion combinedRotation2 = Quaternion.Euler(0, cameraRotation2.eulerAngles.y, 0) * compassRotation2;

        // Calcula o ângulo entre a direção da câmera e o norte
        float angleToNorth2 = Quaternion.Angle(Quaternion.Euler(0, combinedRotation2.eulerAngles.y, 0), Quaternion.identity);


        // Define um limite para considerar a câmera apontando para o norte
        float threshold2 = 15f;

        // Verifica se o ângulo está dentro do limite para considerar a câmera apontando para o norte
        if (angleToNorth2 < threshold2)
        {
            compassText2.text = "Você está apontando para o norte: " + angleToNorth2;
        }
        else
        {
            compassText2.text = "Você está a " + angleToNorth2.ToString("F2") + " graus de distância do norte: ";
        }
        */
    }

    public void ClosePainel()
    {
        painelRouteExplorationDev.SetActive(false);
    }
    public void OpenPainel()
    {
        painelRouteExplorationDev.SetActive(true);
    }

    public void OpenPoiLivraria()
    {
        //POI PORTO
        prefabLivraria.SetActive(true);
        prefabUni.SetActive(false);
        prefabClerigos.SetActive(false);
        prefabFotografia.SetActive(false);
        prefabPuppet.SetActive(false);
        prefabPalacio.SetActive(false);
        prefabCityMuseum.SetActive(false);
        prefabRibeira.SetActive(false);


        //POI CHAVES
        prefabPonte.SetActive(false);
        prefabTermas.SetActive(false);
        prefabIgreja.SetActive(false);
        prefabCastelo.SetActive(false);
        prefabAguas.SetActive(false);
    }

    public void OpenPoiUni()
    {
        //POI PORTO
        prefabLivraria.SetActive(false);
        prefabUni.SetActive(true);
        prefabClerigos.SetActive(false);
        prefabFotografia.SetActive(false);
        prefabPuppet.SetActive(false);
        prefabPalacio.SetActive(false);
        prefabCityMuseum.SetActive(false);
        prefabRibeira.SetActive(false);


        //POI CHAVES
        prefabPonte.SetActive(false);
        prefabTermas.SetActive(false);
        prefabIgreja.SetActive(false);
        prefabCastelo.SetActive(false);
        prefabAguas.SetActive(false);
    }

    public void OpenPoiClerigos()
    {
        //POI PORTO
        prefabLivraria.SetActive(false);
        prefabUni.SetActive(false);
        prefabClerigos.SetActive(true);
        prefabFotografia.SetActive(false);
        prefabPuppet.SetActive(false);
        prefabPalacio.SetActive(false);
        prefabCityMuseum.SetActive(false);
        prefabRibeira.SetActive(false);


        //POI CHAVES
        prefabPonte.SetActive(false);
        prefabTermas.SetActive(false);
        prefabIgreja.SetActive(false);
        prefabCastelo.SetActive(false);
        prefabAguas.SetActive(false);
    }

    public void OpenPoiFoto()
    {
        //POI PORTO
        prefabLivraria.SetActive(false);
        prefabUni.SetActive(false);
        prefabClerigos.SetActive(false);
        prefabFotografia.SetActive(true);
        prefabPuppet.SetActive(false);
        prefabPalacio.SetActive(false);
        prefabCityMuseum.SetActive(false);
        prefabRibeira.SetActive(false);


        //POI CHAVES
        prefabPonte.SetActive(false);
        prefabTermas.SetActive(false);
        prefabIgreja.SetActive(false);
        prefabCastelo.SetActive(false);
        prefabAguas.SetActive(false);
    }

    public void OpenPoiPuppet()
    {
        //POI PORTO
        prefabLivraria.SetActive(false);
        prefabUni.SetActive(false);
        prefabClerigos.SetActive(false);
        prefabFotografia.SetActive(false);
        prefabPuppet.SetActive(true);
        prefabPalacio.SetActive(false);
        prefabCityMuseum.SetActive(false);
        prefabRibeira.SetActive(false);


        //POI CHAVES
        prefabPonte.SetActive(false);
        prefabTermas.SetActive(false);
        prefabIgreja.SetActive(false);
        prefabCastelo.SetActive(false);
        prefabAguas.SetActive(false);
    }

    public void OpenPoiPalacio()
    {
        //POI PORTO
        prefabLivraria.SetActive(false);
        prefabUni.SetActive(false);
        prefabClerigos.SetActive(false);
        prefabFotografia.SetActive(false);
        prefabPuppet.SetActive(false);
        prefabPalacio.SetActive(true);
        prefabCityMuseum.SetActive(false);
        prefabRibeira.SetActive(false);


        //POI CHAVES
        prefabPonte.SetActive(false);
        prefabTermas.SetActive(false);
        prefabIgreja.SetActive(false);
        prefabCastelo.SetActive(false);
        prefabAguas.SetActive(false);
    }

    public void OpenPoiCityMuseum()
    {
        //POI PORTO
        prefabLivraria.SetActive(false);
        prefabUni.SetActive(false);
        prefabClerigos.SetActive(false);
        prefabFotografia.SetActive(false);
        prefabPuppet.SetActive(false);
        prefabPalacio.SetActive(false);
        prefabCityMuseum.SetActive(true);
        prefabRibeira.SetActive(false);


        //POI CHAVES
        prefabPonte.SetActive(false);
        prefabTermas.SetActive(false);
        prefabIgreja.SetActive(false);
        prefabCastelo.SetActive(false);
        prefabAguas.SetActive(false);
    }

    public void OpenPoiRibeira()
    {
        //POI PORTO
        prefabLivraria.SetActive(false);
        prefabUni.SetActive(false);
        prefabClerigos.SetActive(false);
        prefabFotografia.SetActive(false);
        prefabPuppet.SetActive(false);
        prefabPalacio.SetActive(false);
        prefabCityMuseum.SetActive(false);
        prefabRibeira.SetActive(true);


        //POI CHAVES
        prefabPonte.SetActive(false);
        prefabTermas.SetActive(false);
        prefabIgreja.SetActive(false);
        prefabCastelo.SetActive(false);
        prefabAguas.SetActive(false);
    }

    public void OpenPoiPonte()
    {
        //POI PORTO
        prefabLivraria.SetActive(false);
        prefabUni.SetActive(false);
        prefabClerigos.SetActive(false);
        prefabFotografia.SetActive(false);
        prefabPuppet.SetActive(false);
        prefabPalacio.SetActive(false);
        prefabCityMuseum.SetActive(false);
        prefabRibeira.SetActive(false);


        //POI CHAVES
        prefabPonte.SetActive(true);
        prefabTermas.SetActive(false);
        prefabIgreja.SetActive(false);
        prefabCastelo.SetActive(false);
        prefabAguas.SetActive(false);
    }

    public void OpenPoiTermas()
    {
        //POI PORTO
        prefabLivraria.SetActive(false);
        prefabUni.SetActive(false);
        prefabClerigos.SetActive(false);
        prefabFotografia.SetActive(false);
        prefabPuppet.SetActive(false);
        prefabPalacio.SetActive(false);
        prefabCityMuseum.SetActive(false);
        prefabRibeira.SetActive(false);


        //POI CHAVES
        prefabPonte.SetActive(false);
        prefabTermas.SetActive(true);
        prefabIgreja.SetActive(false);
        prefabCastelo.SetActive(false);
        prefabAguas.SetActive(false);
    }

    public void OpenPoiIgreja()
    {
        //POI PORTO
        prefabLivraria.SetActive(false);
        prefabUni.SetActive(false);
        prefabClerigos.SetActive(false);
        prefabFotografia.SetActive(false);
        prefabPuppet.SetActive(false);
        prefabPalacio.SetActive(false);
        prefabCityMuseum.SetActive(false);
        prefabRibeira.SetActive(false);


        //POI CHAVES
        prefabPonte.SetActive(false);
        prefabTermas.SetActive(false);
        prefabIgreja.SetActive(true);
        prefabCastelo.SetActive(false);
        prefabAguas.SetActive(false);
    }

    public void OpenPoiCastelo()
    {
        //POI PORTO
        prefabLivraria.SetActive(false);
        prefabUni.SetActive(false);
        prefabClerigos.SetActive(false);
        prefabFotografia.SetActive(false);
        prefabPuppet.SetActive(false);
        prefabPalacio.SetActive(false);
        prefabCityMuseum.SetActive(false);
        prefabRibeira.SetActive(false);


        //POI CHAVES
        prefabPonte.SetActive(false);
        prefabTermas.SetActive(false);
        prefabIgreja.SetActive(false);
        prefabCastelo.SetActive(true);
        prefabAguas.SetActive(false);
    }

    public void OpenPoiAguas()
    {
        //POI PORTO
        prefabLivraria.SetActive(false);
        prefabUni.SetActive(false);
        prefabClerigos.SetActive(false);
        prefabFotografia.SetActive(false);
        prefabPuppet.SetActive(false);
        prefabPalacio.SetActive(false);
        prefabCityMuseum.SetActive(false);
        prefabRibeira.SetActive(false);


        //POI CHAVES
        prefabPonte.SetActive(false);
        prefabTermas.SetActive(false);
        prefabIgreja.SetActive(false);
        prefabCastelo.SetActive(false);
        prefabAguas.SetActive(true);
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
            var geospatialPose = earthManager.CameraGeospatialPose;
            

            foreach (var obj in geospatialObjects)
            {
                var earthPosition = obj.earthPosition;

                //var worldPosition = ConvertEarthPositionToWorldPosition(obj.earthPosition);


                var objAnchor = ARAnchorManagerExtensions.AddAnchor(aRAnchorManager, earthPosition.latitude, earthPosition.longitude, earthPosition.altitude, Quaternion.identity);
                //var objAnchor = ARAnchorManagerExtensions.AddAnchor(aRAnchorManager, 41.756905, -7.462092, 433.118100, Quaternion.identity);
                Instantiate(obj.objectPrefab, objAnchor.transform);
                /**
                var instantiatedObject = Instantiate(obj.objectPrefab, objAnchor.transform);
                // Calculate the object's position relative to the camera
                //var cameraPose = arCameraManager.transform.position;
                
                //var objectPose = instantiatedObject.transform.position;
                

                objectPoseObject = instantiatedObject.transform.position; // Inicializar objectPose
                objectPoseText.text = "object pose: " + objectPoseObject;
                // Calculate the object's position relative to the camera
                cameraPoseObject = arCameraManager.transform.position; // Inicializar cameraPose
                cameraPoseText.text = "camera pose: " + cameraPoseObject;
                
                relativePositionObject = objectPoseObject - cameraPoseObject; // Inicializar relativePosition

                //var relativePosition = cameraPose - objectPose;

                rotationObject = Quaternion.LookRotation(relativePositionObject);
                seta.transform.rotation = rotationObject;

                directionObject = GetDirection(relativePositionObject);
                directionLocation.text = directionObject;

                //InvokeRepeating("UpdateSetaRotation", 5f, 5f);
                var rotation = Quaternion.LookRotation(relativePositionObject);
                //seta.transform.rotation = rotation;
                

                if (Vector3.Dot(arCameraManager.transform.forward, relativePositionObject) < 0)
                {
                    textLocation.text = "objeto está frent: " + "rekative positiob = " + relativePositionObject + ", rotation = " + rotation;
                    // You can implement further actions here, like displaying a message.
                }

                else if (Vector3.Dot(arCameraManager.transform.forward, relativePositionObject) > 0)
                {
                    textLocation.text = "objeto está atrás: " + "rekative positiob = " + relativePositionObject + ", rotation = " + rotation;
                    // You can implement further actions here, like displaying a message.
                }
                */

            }

            ButtonLivraria.onClick.AddListener(OpenPoiLivraria);
           
            ButtonUni.onClick.AddListener(OpenPoiUni);
            ButtonClerigos.onClick.AddListener(OpenPoiClerigos);
            ButtonFotografia.onClick.AddListener(OpenPoiFoto);
            ButtonPuppet.onClick.AddListener(OpenPoiPuppet);
            ButtonPalacio.onClick.AddListener(OpenPoiPalacio);
            ButtonCityMuseum.onClick.AddListener(OpenPoiCityMuseum);
            ButtonRibeira.onClick.AddListener(OpenPoiRibeira);

            ButtonPonte.onClick.AddListener(OpenPoiPonte);
            ButtonTermas.onClick.AddListener(OpenPoiTermas);
            ButtonIgreja.onClick.AddListener(OpenPoiIgreja);
            ButtonCastelo.onClick.AddListener(OpenPoiCastelo);
            ButtonAguas.onClick.AddListener(OpenPoiAguas);
            
        }

        else if (earthManager.EarthTrackingState == TrackingState.None)
        {
            Debug.Log("TrackingState.None");
            Invoke("PlaceObjects", 5.0f);
        }
    }
    /**
    private Vector3 ConvertEarthPositionToWorldPosition(EarthPosition earthPosition)
    {
        // Implemente a conversão aqui
        return Vector3.zero; // Retorna um vetor de posição fictício para fins de exemplo
    }

    public void UpdateSetaRotation()
    {
        if (earthManager.EarthTrackingState == TrackingState.Tracking)
        {
            // Atualizar a posição do objeto e a posição relativa à câmera
            cameraPoseObject = arCameraManager.transform.position;
            relativePositionObject = objectPoseObject - cameraPoseObject;

            // Atualizar a rotação da seta para apontar na direção do objeto
            rotationObject = Quaternion.LookRotation(relativePositionObject);
            seta.transform.rotation = rotationObject;

            // Exibir a posição da câmera e do objeto nos textos
            cameraPoseText.text = "camera pose updated: " + cameraPoseObject;
            objectPoseText.text = "object pose updated: " + objectPoseObject;

            directionObject = GetDirection(relativePositionObject);
            directionLocation.text = "updated: " + directionObject;
        }
    }

    private string GetDirection(Vector3 relativePosition)
    {
        string direction = "";

        // Determinar a direção com base nos componentes x, y e z do vetor relativo
        if (relativePosition.z > 0)
        {
            direction += "frente";
        }
        else
        {
            direction += "atrás";
        }

        if (relativePosition.x > 0)
        {
            direction += " direita";
        }
        else if (relativePosition.x < 0)
        {
            direction += " esquerda";
        }

        if (relativePosition.y > 0)
        {
            direction += " cima";
        }
        else if (relativePosition.y < 0)
        {
            direction += " baixo";
        }

        return direction;
    }
    */
}
