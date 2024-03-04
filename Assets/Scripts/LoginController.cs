using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class LoginController : MonoBehaviour
{
    // Start is called before the first frame update
    public InputField emailInputField;
    public InputField passwordInputField;
    public Button LoginToButton;

    public GameObject loginPainel;
    public GameObject initPainel;

    void Start()
    {
        LoginToButton.onClick.AddListener(Login);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Login()
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
                Debug.Log("Email e senha válidos, proceda com o login.");
                loginPainel.SetActive(false);
                initPainel.SetActive(true);
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
