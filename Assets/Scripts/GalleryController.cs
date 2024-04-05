using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GalleryController : MonoBehaviour
{
    // Start is called before the first frame update
    public string apiUrl = "http://localhost:3000/api/photo/gallery/";
    public Button GalleryBtn;
    public Button backBtn;
    public Image photoImage;
    public Text photoDescription;
    public GameObject galleryPanel;
    public GameObject galleryPhotoContainer;
    private GameObject photoTemplate;

    void Start()
    {
        int touristId = PlayerPrefs.GetInt("Current_Logged_TouristID", -1); // -1 é o valor padrão se a chave "UserID" não existir
        GalleryBtn.onClick.AddListener(() => GetGallery(touristId));
        backBtn.onClick.AddListener(LeaveGallery);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [System.Serializable]
    public class GalleryResponse
    {
        public List<PhotoData> gallery;
    }

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
    }

    public void GetGallery(int touristId)
    {
        Debug.Log("GetGallery");
        Debug.Log("touristId: " + touristId);
        string url = apiUrl + touristId;
        StartCoroutine(SendGetGalleryRequest(apiUrl));
    }

    private List<GameObject> instantiatedPhotoObjects = new List<GameObject>();

    IEnumerator SendGetGalleryRequest(string urlApi)
    {
        int touristId = PlayerPrefs.GetInt("Current_Logged_TouristID", -1);
        var url = urlApi + touristId;
        ClearInstantiatedPhotoObjects();
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to get gallery: " + request.error);
            }
            else
            {
                Debug.Log("get fotos");
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("jsonResponse: " + jsonResponse);
                /**
                List<PhotoData> gallery = JsonUtility.FromJson<List<PhotoData>>(jsonResponse);
                Debug.Log("gallery: " + gallery);
                LoadImageFromBase64(gallery[0].image_base64, photoImage);
                photoDescription.text = gallery[0].description;
                // Handle the gallery data here
                foreach (var photo in gallery)
                {

                    Debug.Log("Description: " + photo.description);
                    // Add more fields as needed
                }
                */
                List<Dictionary<string, object>> photoList = new List<Dictionary<string, object>>();

                // Parse do JSON response para uma lista de dicionários
                List<object> jsonArray = JsonConvert.DeserializeObject<List<object>>(jsonResponse);
                photoTemplate = galleryPhotoContainer.transform.GetChild(0).gameObject;
                GameObject picture = transform.Find("GalleryPanel/PicturePanel/ScrollArea/Scroll/Container").gameObject;
                GameObject p;
                // Percorre cada item do JSON response
               
                List<PhotoData> jsonArray2 = JsonConvert.DeserializeObject<List<PhotoData>>(jsonResponse);

                foreach (PhotoData item in jsonArray2)
                {
                    Debug.Log("item3: " + item.description);
                    p = Instantiate(photoTemplate, galleryPhotoContainer.transform);
                    Image imageComponent = p.transform.GetChild(0).GetComponent<Image>();
                    LoadImageFromBase64(item.image_base64, imageComponent);
                    p.transform.GetChild(1).GetComponent<Text>().text = item.description;
                    instantiatedPhotoObjects.Add(p);
                }
                //Destroy(photoTemplate);
                photoTemplate.SetActive(false);
                if(jsonArray.Count > 0)
                {
                    string firstItemJson = jsonArray[0].ToString();
                    PhotoData firstPhotoData = JsonConvert.DeserializeObject<PhotoData>(firstItemJson);
                    //photoDescription.text = firstPhotoData.description;
                    //LoadImageFromBase64(firstPhotoData.image_base64, photoImage);


                    // Inicializa a lista de PhotoData
                    List<PhotoData> gallery = new List<PhotoData>();

                    // Percorre cada dicionário na lista de dicionários
                    foreach (Dictionary<string, object> photoDict in photoList)
                    {
                        // Cria um novo objeto PhotoData e atribui os valores do dicionário a ele
                        PhotoData photo = new PhotoData
                        {
                            id = (int)photoDict["id"],
                            tourist_route_association_id = (int)photoDict["tourist_route_association_id"],
                            description = (string)photoDict["description"],
                            date = (string)photoDict["date"],
                            image_hash = (string)photoDict["image_hash"],
                            filename = (string)photoDict["filename"],
                            image_base64 = (string)photoDict["image_base64"]
                        };

                        // Adiciona o objeto PhotoData à lista de galeria
                        gallery.Add(photo);
                    }

                }

                galleryPanel.SetActive(true);
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
    public void LeaveGallery()
    {
        photoTemplate.SetActive(true);
        ClearInstantiatedPhotoObjects();
        Debug.Log("leave gallery");
        galleryPanel.SetActive(false);
    }
}
