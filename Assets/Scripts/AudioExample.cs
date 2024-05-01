using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioExample : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource source;
    public AudioClip clipPonte;
    public AudioClip clipTermas;
    public AudioClip clipIgreja;
    public AudioClip clipCastelo;
    public AudioClip clipCaldas;
    public AudioClip clipFimChaves;
    public Button audioBook;
    private bool isPlaying = false;
    private int routePoiCurrentOrder;
    private int orderCount = 0;
    void Start()
    {
        routePoiCurrentOrder = PlayerPrefs.GetInt("RoutePoiCurrentOrder", 0);
        audioBook.onClick.RemoveAllListeners();
        audioBook.onClick.AddListener(ToggleAudio);
        orderCount = routePoiCurrentOrder;
    }

    // Update is called once per frame
    void Update()
    {
        routePoiCurrentOrder = PlayerPrefs.GetInt("RoutePoiCurrentOrder", 0);
    }
    void ToggleAudio()
    {
        Debug.Log("routePoiCurrentOrder: " + routePoiCurrentOrder);
        if (!isPlaying)
        {
            if (routePoiCurrentOrder == 0)
            {
                source.PlayOneShot(clipPonte);
            }
            else if(routePoiCurrentOrder == 1)
            {
                source.PlayOneShot(clipTermas);
            }
            else if (routePoiCurrentOrder == 2)
            {
                source.PlayOneShot(clipIgreja);
            }
            else if (routePoiCurrentOrder == 3)
            {
                source.PlayOneShot(clipCastelo);
            }
            else if (routePoiCurrentOrder == 4)
            {
                source.PlayOneShot(clipCaldas);
            }
            else
            {
                source.PlayOneShot(clipFimChaves);
            }
            isPlaying = true;
        }
        else
        {
            source.Stop();
            isPlaying = false;
        }
        
        
    }
}
