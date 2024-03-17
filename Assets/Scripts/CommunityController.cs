using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CommunityController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject communityPanel;
    public GameObject communityPhotoContainer;
    private double containerHeight;
    private double photoNumber;
    private double photoHeight = 500;

    public string apiUrl = "http://localhost:3000/api/photo/community";
    public Button CommunityBtn;

    [System.Serializable]
    public class PhotoData
    {
        public int id;
        public int tourist_route_association_id;
        public string description;
        public string date;
        public string image_hash;
        public string filename;
        public string image_base64;
        public int photo_likes;
        public string email;
    }
    void Start()
    {
        int touristId = PlayerPrefs.GetInt("Current_Logged_TouristID", -1); // -1 é o valor padrão se a chave "UserID" não existir
        Debug.Log("touristId: " + touristId);
        CommunityBtn.onClick.AddListener(() => GetCommunity(touristId));
        /**
        AdjustContainerHeight();
        GameObject communityTemplate = communityPhotoContainer.transform.GetChild(0).gameObject;
        GameObject p;

        for (int i = 0; i < photoNumber; i++)
        {
            p = Instantiate(communityTemplate, communityPhotoContainer.transform);
        }
        Destroy(communityTemplate);
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetCommunity(int touristId)
    {
        Debug.Log("GetCommunity");
        string url = apiUrl;// + touristId;
        StartCoroutine(SendGetCommunityRequest(url));
    }

    IEnumerator SendGetCommunityRequest(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to get community: " + request.error);
            }
            else
            {
                Debug.Log("get fotos");
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("jsonResponse: " + jsonResponse);
                List<PhotoData> jsonArray = JsonConvert.DeserializeObject<List<PhotoData>>(jsonResponse);
                GameObject communityTemplate = communityPhotoContainer.transform.GetChild(0).gameObject;
                GameObject p;
                photoNumber = jsonArray.Count;
                AdjustContainerHeight(photoNumber);
                foreach (PhotoData item in jsonArray)
                {
                    Debug.Log("item: " + item.description);
                    p = Instantiate(communityTemplate, communityPhotoContainer.transform);
                    Image imageComponent = p.transform.GetChild(0).GetComponent<Image>();
                    LoadImageFromBase64(item.image_base64, imageComponent);
                    p.transform.GetChild(1).GetComponent<Text>().text = item.description;
                    p.transform.GetChild(5).GetComponent<Text>().text = item.email;
                    p.transform.GetChild(7).GetComponent<Text>().text = item.date;
                    p.transform.GetChild(9).GetComponent<Text>().text = item.photo_likes + "";

                }
                Destroy(communityTemplate);
                communityPanel.SetActive(true);
            }
        }
    }

    void AdjustContainerHeight(double numberElements)
    {
        double totalHeight = numberElements * photoHeight + (numberElements - 1) * 50; // Calculating total height
        RectTransform containerRectTransform = communityPhotoContainer.GetComponent<RectTransform>();
        containerRectTransform.sizeDelta = new Vector2(containerRectTransform.sizeDelta.x, (float)totalHeight);
    }

    public void LoadImageFromBase64(string base64String, Image image)
    {
        Debug.Log("LoadImageFromBase64");

        // Converte a string base64 para bytes
        byte[] imageBytes = Convert.FromBase64String(base64String);

        // Cria uma textura a partir dos bytes da imagem
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(imageBytes);

        // Define a textura no componente Image
        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }
}
