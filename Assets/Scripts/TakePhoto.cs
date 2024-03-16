using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.UI;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using System.Security.Cryptography;
using UnityEngine.Networking;

public class TakePhoto : MonoBehaviour
{
    public Button takePhotoButton;
    public Button savePhotoButton;
    // Start is called before the first frame update
    void Start()
    {
        takePhotoButton.onClick.AddListener(takePhoto);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void takePhoto()
    {
        StartCoroutine(Photo());
    }

    public IEnumerator Photo()
    {
        Debug.Log("StartCoroutine");
        yield return new WaitForEndOfFrame();
        Camera camera = Camera.main;
        int width = Screen.width;
        int height = Screen.height;
        RenderTexture rt = new RenderTexture(width, height, 24);
        camera.targetTexture = rt;
        // The Render Texture in RenderTexture.active is the one
        // that will be read by ReadPixels.
        var currentRT = RenderTexture.active;
        RenderTexture.active = rt;

        // Render the camera's view.
        camera.Render();

        // Make a new texture and read the active Render Texture into it.
        Texture2D image = new Texture2D(width, height);
        image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        image.Apply();
        Debug.Log("image: " + image);
        camera.targetTexture = null;
        // Replace the original active Render Texture.
        RenderTexture.active = currentRT;

        //savePhoto(image, rt);
        //savePhotoButton.onClick.AddListener(() => savePhoto(image, rt));

        byte[] bytes = image.EncodeToPNG();
        Debug.Log("bytes: " + bytes);
        string base64String = Convert.ToBase64String(bytes);
        System.IO.File.WriteAllText("base64.txt", base64String);
        string filePathBase64 = @"D:\uni\tese\rep\base64\base64.txt"; // Caminho completo do arquivo de texto
        System.IO.File.WriteAllText(filePathBase64, base64String);
        string filename = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        Debug.Log("filename: " + filename);
        string imageHash = CalculateImageHash(image);
        Debug.Log("imageHash: " + imageHash);
        //string filePath = Application.persistentDataPath + "/" + filename;
        //string directoryName = "PhotosTaken";
        
        //string directoryPath = @"D:\uni\tese\rep\photos_teste";

        // Combine o caminho do diretório com o caminho do arquivo.
        //string filePath = Path.Combine("Assets", directoryPath, DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png");
        //Debug.Log("filePath: " + filePath);
        //System.IO.File.WriteAllBytes(filePath, bytes);

        int touristRouteId = 2;
        string description = "Descrição da foto";
        DateTime date = DateTime.Now;

        

        UploadPhoto(touristRouteId, description, date, imageHash, filename, base64String);


        Destroy(rt);
        Destroy(image);

    }
    
    public void UploadPhoto(int touristRouteAssociationId, string description, DateTime date, string imageHash, string filename, string base64String)
    {
        string url = "http://localhost:3000/api/photo/upload";

        // Convertendo a imagem para bytes
        Debug.Log("UploadPhoto: ");
        // Criando o objeto de dados a serem enviados
        WWWForm form = new WWWForm();
        form.AddField("tourist_route_association_id", touristRouteAssociationId.ToString());
        form.AddField("description", description);
        form.AddField("date", date.ToString("yyyy-MM-ddTHH:mm:ss"));
        form.AddField("image_hash", imageHash);
        form.AddField("filename", filename);
        form.AddField("image_base64", base64String);

        // Enviando a requisição
        StartCoroutine(SendRequest(url, form));
    }

    IEnumerator SendRequest(string url, WWWForm form)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erro ao enviar a foto: " + www.error);
            }
            else
            {
                Debug.Log("Foto enviada com sucesso!");
            }
        }
    }
    

    string CalculateImageHash(Texture2D image)
    {
        byte[] imageBytes = image.EncodeToPNG();
        string hash;

        using (MD5 md5 = MD5.Create())
        {
            byte[] hashBytes = md5.ComputeHash(imageBytes);
            hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }

        return hash;
    }

    /**
    public void savePhoto(Texture2D image, RenderTexture rt)
    {
        byte[] bytes = image.EncodeToPNG();
        Debug.Log("bytes: " + bytes);
        string filename = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        Debug.Log("filename: " + filename);

        //string filePath = Application.persistentDataPath + "/" + filename;
        //string directoryName = "PhotosTaken";
        string directoryPath = @"D:\uni\tese\rep\photos_teste";

        // Combine o caminho do diretório com o caminho do arquivo.
        string filePath = Path.Combine("Assets", directoryPath, DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png");
        Debug.Log("filePath: " + filePath);
        System.IO.File.WriteAllBytes(filePath, bytes);
        Destroy(rt);
        Destroy(image);
    }
    */
}
