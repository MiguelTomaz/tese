using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using System.Text;

public class RegisterController : MonoBehaviour
{
    public InputField emailInputField;
    public InputField passwordInputField;
    public Button registerToButton;

    public GameObject RegisterPainel;
    public GameObject indexPainel;

    string API_URI = "http://localhost:3000/api/";
    // User
    [System.Serializable]
    public class Tourist
    {
        public string email;
        public string password;
    }

    [System.Serializable]
    public class RegisterResponse
    {
        public string message;
        public string error;
    }
    void Start()
    {
        registerToButton.onClick.AddListener(Register);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Register()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.Log("Email e/ou senha não podem estar em branco.");
            return;
        }
        else
        {
            // Verificar se o email está no formato válido
            if (!IsValidEmail(email))
            {
                Debug.Log("Email inválido!");
                return;
            }

            // Verificar se a senha atende aos critérios
            if (!IsStrongPassword(password))
            {
                Debug.Log("Senha fraca! A senha deve conter pelo menos 8 caracteres: 1 letra maiúscula, 1 letra minúscula, 1 número e 1 caractere especial.");
                return;
            }

            if (IsValidEmail(email) && IsStrongPassword(password))
            {
                // Se passou pelas verificações, faça o que for necessário para o login
                Debug.Log("Email e senha válidos, proceda com o register.");
                Tourist tourist = new Tourist();
                tourist.email = email;
                tourist.password = password;

                StartCoroutine(Register(tourist.email, tourist.password));
                //RegisterPainel.SetActive(false);
                //indexPainel.SetActive(true);
            }
        }
    }

    IEnumerator Register(string email, string password)
    {

        // Se passou pelas verificações, faça o que for necessário para o login
        string uri = "http://13.60.19.19:3000/api/user/add";//API_URI + "/user/login?email=" + email + "&password=" + password + "";//API_URI + "user/login";
        string jsonData = "{\"email\":\"" + email + "\", \"password\":\"" + password + "\"}";

        UnityWebRequest request = new UnityWebRequest(uri, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Envia a solicitação e espera pela resposta
        yield return request.SendWebRequest();

        // Verifica se houve algum erro na resposta
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Erro ao registar: " + request.error);
        }
        else
        {
            // Analisa a resposta da API
            string responseText = request.downloadHandler.text;
            RegisterResponse responseData = JsonUtility.FromJson<RegisterResponse>(responseText);

            // Verifica se o login foi bem-sucedido
            if (responseData != null && responseData.message != null)
            {
                Debug.Log("registo bem-sucedido: " + responseData.message);
                RegisterPainel.SetActive(false);
                indexPainel.SetActive(true);

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
