using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Google.XR.ARCoreExtensions;
using System;
using UnityEngine.UI;

public class VPSManager : MonoBehaviour
{
    [SerializeField] private AREarthManager earthManager;
    public GameObject panel;
    private bool instantiate = false;

    private Transform objAnchorTeste;


    [Serializable]
    public struct GeospatialObject
    {
        public GameObject objectPrefab;
        public GameObject objectPrefab2;
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

    public GameObject prefabTeste;

    public Text readyToUseText;
    public Text text2;

    //public Text text3;
    // Start is called before the first frame update
    void Start()
    {
        VerifyGeospatialSupport();
    }
    private void Update()
    {
        if(instantiate)
        {
            Vector3 position = objAnchorTeste.position;

            // Converte as coordenadas em uma string formatada para exibição
            string coordinates = string.Format("X: {0}, Y: {1}, Z: {2}", position.x, position.y, position.z);
            text2.text = "look at: " + coordinates;
            prefabTeste.transform.LookAt(objAnchorTeste);
        }
        else
        {
            text2.text = "look at é falso";
        }
    }

    private void VerifyGeospatialSupport()
    {
        var result = earthManager.IsGeospatialModeSupported(GeospatialMode.Enabled);
        switch(result)
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
        if(earthManager.EarthTrackingState == TrackingState.Tracking)
        {
            var geospatialPose = earthManager.CameraGeospatialPose;


            foreach(var obj in geospatialObjects)
            {
                var eartPosition = obj.earthPosition;
                var objAnchor = ARAnchorManagerExtensions.AddAnchor(aRAnchorManager, eartPosition.latitude, eartPosition.longitude, eartPosition.altitude, Quaternion.identity);
                var objAnchor2 = ARAnchorManagerExtensions.AddAnchor(aRAnchorManager, 41.756905, -7.462092, eartPosition.altitude, Quaternion.identity);
                objAnchorTeste = objAnchor.transform;
                //text2.text = "objAnchor: " + objAnchor;
                Instantiate(obj.objectPrefab, objAnchor.transform);
                instantiate = true;

            }
        }

        else if(earthManager.EarthTrackingState == TrackingState.None)
        {
            Debug.Log("TrackingState.None");
            readyToUseText.text = "TrackingState.None";
            Invoke("PlaceObjects", 5.0f);
        }
    }

    

   
}
