using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public Sprite isLikedButtonSprite;
    public Sprite dontLikedButtonSprite;
    private double containerHeight;
    private double photoNumber;
    private double photoHeight = 500;

    private string apiUrl = "http://13.60.19.19:3000/api/photo/community";
    private string checkIsLikedUrl = "http://13.60.19.19:3000/api/photo/community/like/check";
    private string likedUrl = "http://13.60.19.19:3000/api/photo/community/like";
    private GameObject communityTemplate;
    public Button leaveCommunityBtn;

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
        leaveCommunityBtn.onClick.AddListener(LeaveCommunity);
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
    private List<GameObject> instantiatedPhotoObjects = new List<GameObject>();

    IEnumerator SendGetCommunityRequest(string url)
    {
        ClearInstantiatedPhotoObjects();

        int touristId = PlayerPrefs.GetInt("Current_Logged_TouristID", -1);
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
                communityTemplate = communityPhotoContainer.transform.GetChild(0).gameObject;
                GameObject p;
                photoNumber = jsonArray.Count;
                AdjustContainerHeight(photoNumber);
                foreach (PhotoData item in jsonArray)
                {
                    Debug.Log("item: " + item.id);
                    p = Instantiate(communityTemplate, communityPhotoContainer.transform);
                    Image imageComponent = p.transform.GetChild(0).GetComponent<Image>();
                    LoadImageFromBase64(item.image_base64, imageComponent);
                    p.transform.GetChild(1).GetComponent<Text>().text = item.description;
                    p.transform.GetChild(5).GetComponent<Text>().text = item.email;
                    p.transform.GetChild(7).GetComponent<Text>().text = item.date;
                    p.transform.GetChild(9).GetComponent<Text>().text = item.photo_likes + "";
                    CheckLike(checkIsLikedUrl, item.id, touristId, p.transform.GetChild(10).GetComponent<Button>());
                    Button likeButton = p.transform.GetChild(10).GetComponent<Button>();
                    Text likeText = p.transform.GetChild(9).GetComponent<Text>();
                    likeButton.onClick.AddListener(() => Like(item.id, touristId, likeButton, likeText));
                    instantiatedPhotoObjects.Add(p);


                }
                //Destroy(communityTemplate);
                communityTemplate.SetActive(false);
                communityPanel.SetActive(true);
            }
        }
    }

    private void ClearInstantiatedPhotoObjects()
    {
        foreach (GameObject obj in instantiatedPhotoObjects)
        {
            Destroy(obj);
        }
        instantiatedPhotoObjects.Clear();
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

    void CheckLike(string url, int photoId, int touristId, Button buttonLike)
    {
        WWWForm form = new WWWForm();
        form.AddField("touristId", touristId);
        form.AddField("photoId", photoId);

        // Enviando a requisição
        StartCoroutine(SendRequestCheckLike(url, form, buttonLike, photoId));
    }

    IEnumerator SendRequestCheckLike(string url, WWWForm form, Button buttonLike, int photoId)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erro");
            }
            else
            {
                Debug.Log("check like da foto: " + photoId);
                string jsonResponse = www.downloadHandler.text;
                Debug.Log("jsonResponse: " + jsonResponse);
                JObject jsonObject = JObject.Parse(jsonResponse);
                bool liked = (bool)jsonObject["liked"];
                if (liked)
                {
                    //likeTextComponent.text = "deu like";
                    Image buttonImage = buttonLike.GetComponent<Image>();
                    Debug.Log("deu like");
                    buttonImage.sprite = isLikedButtonSprite;
                }
                /**
                bool liked = JsonConvert.DeserializeObject<bool>(jsonResponse);
                if (liked)
                {
                    likeTextComponent.text = "deu like";
                }
                */
            }
        }
    }

    void Like(int photoId, int touristId, Button likeButton, Text likeNumber)
    {
        // Chame o método para dar like na foto
        WWWForm form = new WWWForm();
        form.AddField("touristId", touristId);
        form.AddField("photoId", photoId);
        Debug.Log("like da foto " + photoId + " cujos likes atuais sao: " + likeNumber);
        // Enviando a requisição
        StartCoroutine(SendRequestLike(likedUrl, form, photoId, likeButton, likeNumber));
    }

    IEnumerator SendRequestLike(string url, WWWForm form, int photoId, Button buttonLike, Text likeNumber)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erro");
            }
            else
            {
                Debug.Log("like da foto: " + photoId);
                string jsonResponse = www.downloadHandler.text;
                Debug.Log("jsonResponse: " + jsonResponse);
                JObject jsonObject = JObject.Parse(jsonResponse);
                bool liked = (bool)jsonObject["like"];
                int likesNumber = (int)jsonObject["likes"];
                likeNumber.text = likesNumber + "";
                if (liked)
                {
                    //likeTextComponent.text = "deu like";
                    Image buttonImage = buttonLike.GetComponent<Image>();
                    Debug.Log("deu like");
                    buttonImage.sprite = isLikedButtonSprite;
                }
                else
                {
                    Image buttonImage = buttonLike.GetComponent<Image>();
                    Debug.Log("retirou like");
                    buttonImage.sprite = dontLikedButtonSprite;
                }
                /**
                bool liked = JsonConvert.DeserializeObject<bool>(jsonResponse);
                if (liked)
                {
                    likeTextComponent.text = "deu like";
                }
                */
            }
        }
    }

    public void LeaveCommunity()
    {
        Debug.Log("LEAVE COMMUNITY");
        communityTemplate.SetActive(true);
        ClearInstantiatedPhotoObjects();
        Debug.Log("leave community");
    }
}
