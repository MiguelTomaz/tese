using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using System.Text;

public class LoginController : MonoBehaviour
{
    // Start is called before the first frame update
    public InputField emailInputField;
    public InputField passwordInputField;
    public Button LoginToButton;

    public GameObject loginPainel;
    public GameObject initPainel;

    string API_URI = "http://localhost:3000/api/";
    // User
    [System.Serializable]
    public class Tourist
    {
        public string email;
        public string password;
    }

    [System.Serializable]
    public class LoginResponse
    {
        public string message;
        public string error;
        public int touristId;
    }

    void Start()
    {
        LoginToButton.onClick.AddListener(LoginFunction);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoginFunction()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.Log("Email e/ou senha não podem estar em branco.");
        }
        else
        {
            
            if (!IsValidEmail(email))
            {
                Debug.Log("Email inválido!");
            }

            // Verificar se a senha atende aos critérios
            if (!IsStrongPassword(password))
            {
                Debug.Log("Senha fraca! A senha deve conter pelo menos 8 caracteres: 1 letra maiúscula, 1 letra minúscula, 1 número e 1 caractere especial.");
            }

            if (IsValidEmail(email) && IsStrongPassword(password))
            {
                Tourist tourist = new Tourist();
                tourist.email = email;
                tourist.password = password;

                string jsonData = JsonUtility.ToJson(tourist);
                StartCoroutine(Login(tourist.email, tourist.password));
            }

        }
    }
    IEnumerator Login(string email, string password)
    {

        // Se passou pelas verificações, faça o que for necessário para o login
        Debug.Log("Email e senha válidos, proceda com o login.");
        string email2 = "teste@teste.teste";
        string password2 = "123";
        string uri = "http://localhost:3000/api/user/login";//API_URI + "/user/login?email=" + email + "&password=" + password + "";//API_URI + "user/login";
        string jsonData = "{\"email\":\"" + email + "\", \"password\":\"" + password + "\"}";

        UnityWebRequest request = new UnityWebRequest(uri, "GET");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Envia a solicitação e espera pela resposta
        yield return request.SendWebRequest();

        // Verifica se houve algum erro na resposta
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Erro ao fazer login: " + request.error);
        }
        else
        {
            // Analisa a resposta da API
            string responseText = request.downloadHandler.text;
            LoginResponse responseData = JsonUtility.FromJson<LoginResponse>(responseText);

            // Verifica se o login foi bem-sucedido
            if (responseData != null && responseData.message != null)
            {
                int touristId = responseData.touristId;
                Debug.Log("Login bem-sucedido: " + responseData.message);
                Debug.Log("ID do turista: " + touristId);
                PlayerPrefs.SetInt("Current_Logged_TouristID", touristId);

                int userId = PlayerPrefs.GetInt("Current_Logged_TouristID", -1); // -1 é o valor padrão se a chave "UserID" não existir

                if (userId != -1)
                {
                    // O ID do turista foi encontrado, faça o que for necessário com ele
                    Debug.Log("ID do turista recuperado: " + userId);
                }
                else
                {
                    // Não foi encontrado um ID de turista armazenado
                    Debug.LogWarning("Nenhum ID de turista encontrado.");
                }

                loginPainel.SetActive(false);
                initPainel.SetActive(true);
            }
            else if (responseData != null && responseData.error != null)
            {
                Debug.LogError("Erro ao fazer login: " + responseData.error);
            }
        }

    }


    bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    bool IsStrongPassword(string password)
    {
        if (password == "123")
        {
            return true;
        }
        // Pelo menos 1 letra maiúscula, 1 letra minúscula, 1 número e 1 caractere especial
        return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
    }
}
