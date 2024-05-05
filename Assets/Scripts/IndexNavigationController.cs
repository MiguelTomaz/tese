using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IndexNavigationController : MonoBehaviour
{
    private bool isLocated = false;
    public double northDirrectionTeste = 0;
    public Button goToDevModeAR;
    public GameObject arrowNorth;
    public GameObject arrowPoint;
    public Text compassText;
    public Text compassPointText;
    public Text coordinatesX_Y;
    #region buttons index
    //buttons index
    public Button LoginButton;
    public Button RegisterButton;
    public Button AboutUsButton;
    public Button ExitButton;
    #endregion

    #region buttons login
    //buttons login
    public Button LoginToButton;
    public Button backLogin;
    #endregion

    #region buttons register
    //buttons register
    public Button RegisterToButton;
    public Button backRegister;
    #endregion

    #region paineis
    //painels
    public GameObject indexPainel;
    public GameObject loginPainel;
    public GameObject registerPainel;
    public GameObject aboutUsPainel;
    public GameObject initPainel;

    #endregion

    #region init
    //init
    #region paineis
    public GameObject quizPainel;
    public GameObject comPainel;
    public GameObject homePainel;
    public GameObject statsPainel;
    public GameObject optionsPainel;
    public GameObject portoQuiz;
    public GameObject endQuiz;
    #endregion

    #region navbar
    public Button quizNavButton;
    public Button comNavButton;
    public Button homeNavButton;
    public Button statsNavButton;
    public Button optionsNavButton;
    public Button exitEndQuizButton;

    #endregion

    #region quiz
    public Button backFromQuiz;
    #endregion
    #region com
    public Button backFromCom;
    #endregion

    #region stats
    public Button backFromStats;
    #endregion

    #region settings
    public Button backFromSettings;
    #endregion


    #endregion

    public Button LogoutButton;

    private void Start()
    {
        // Ativa a bússola
        Input.location.Start();
        Input.compass.enabled = true;


        if (PlayerPrefs.GetInt("AfterExploration", 0) == 1)
        {
            // Ativa o GameObject
            indexPainel.SetActive(false);
            loginPainel.SetActive(false);
            registerPainel.SetActive(false);
            initPainel.SetActive(true);
            PlayerPrefs.SetInt("AfterExploration", 0);
        }
        else
        {
            LoginButton.onClick.AddListener(GoToLoginPainel);
            RegisterButton.onClick.AddListener(GoToRegisterPainel);
            backLogin.onClick.AddListener(GoBackFromLogin);
            backRegister.onClick.AddListener(GoBackFromRegister);

            quizNavButton.onClick.AddListener(openQuizPainel);
            comNavButton.onClick.AddListener(openComPainel);
            homeNavButton.onClick.AddListener(openHomePainel);
            statsNavButton.onClick.AddListener(openStatsPainel);
            optionsNavButton.onClick.AddListener(openOptionsPainel);

            backFromQuiz.onClick.AddListener(BackNavBar);
            backFromCom.onClick.AddListener(BackNavBar);
            backFromStats.onClick.AddListener(BackNavBar);
            backFromSettings.onClick.AddListener(BackNavBar);

            LogoutButton.onClick.AddListener(Logout);
            exitEndQuizButton.onClick.AddListener(ExitEndQuiz);

            goToDevModeAR.onClick.AddListener(ExploreAR_DevMode);
        }
    }

    private void Update()
    {

        string latitudeString = PlayerPrefs.GetString("UserLatitude");
        string longitudeString = PlayerPrefs.GetString("UserLongitude");



        var x1 = -7.4622632;
        var y1 = 41.7568188;
        /**
        if (!string.IsNullOrEmpty(latitudeString))
        {
            isLocated = true;
            x1 = double.Parse(latitudeString);
        }

        if (!string.IsNullOrEmpty(longitudeString))
        {
            y1 = double.Parse(longitudeString);
        }
        */

        //latitudeUserText.text = "lat: " + latitudeUser;
        //longitudeUserText.text = "lat: " + longitudeUser;
        coordinatesX_Y.text = "lat:" + x1 + ", long: " + y1 + ", isLocated: " + isLocated;


        var x2 = -7.46212;
        var y2 = 41.756975;

        double delta_x = x2 - x1;
        double delta_y = y2 - y1;

        double[] vector_AB = { delta_x, delta_y };

        double angle_with_y_axis = Math.Atan2(delta_y, delta_x);

        // Converte o ângulo para graus
        double angle_degrees = angle_with_y_axis * (180 / Math.PI);

        Console.WriteLine("Vetor AB: ({0}, {1})", vector_AB[0], vector_AB[1]);
        Debug.Log("angle_degrees: " + angle_degrees);

        // Obtém a rotação da bússola
        float magneticNorth = Input.compass.magneticHeading;

        // Ajusta para a declinação magnética (se necessário)
        float trueNorth = magneticNorth - Input.compass.trueHeading;
        float trueNorthAarrowDirection = Input.compass.trueHeading;
        trueNorthAarrowDirection = (float)Math.Round(trueNorthAarrowDirection, 2);

        arrowNorth.transform.rotation = Quaternion.Euler(0f, 0f, trueNorthAarrowDirection);

        //arrowNorth.transform.rotation = Quaternion.Euler(0f, 0f, (float)northDirrectionTeste);

        double angle_degrees_Y = 90 - angle_degrees;
        double anglePoint = trueNorthAarrowDirection - angle_degrees_Y;
        double anglePointTeste = northDirrectionTeste - angle_degrees_Y;
        arrowPoint.transform.rotation = Quaternion.Euler(0f, 0f, (float)anglePoint);
        // Orienta a câmera para o norte verdadeiro
        transform.rotation = Quaternion.Euler(0, trueNorth, 0);


        //compassText.text = "Norte: " + trueNorth.ToString("F2") + " graus" + ", rotation: " + transform.rotation;
        compassText.text = "norte verdadeiro: " + Input.compass.trueHeading.ToString("F2") + ", rotação seta: " + arrowNorth.transform.rotation.eulerAngles.z;

        compassPointText.text = "angulo seta point: " + anglePoint;

    }
    public void GoToLoginPainel()
    {
        indexPainel.SetActive(false);
        loginPainel.SetActive(true);
    }
    public void GoToRegisterPainel()
    {
        indexPainel.SetActive(false);
        registerPainel.SetActive(true);
    }
    public void GoBackFromLogin()
    {
        loginPainel.SetActive(false);
        indexPainel.SetActive(true);
    }

    public void GoBackFromRegister()
    {
        registerPainel.SetActive(false);
        indexPainel.SetActive(true);
    }
    public void openQuizPainel()
    {
        quizPainel.SetActive(true);

        homePainel.SetActive(false);
        comPainel.SetActive(false);
        optionsPainel.SetActive(false);
        statsPainel.SetActive(false);
    }
    public void openComPainel()
    {
        //comPainel.SetActive(true);

        homePainel.SetActive(false);
        quizPainel.SetActive(false);
        optionsPainel.SetActive(false);
        statsPainel.SetActive(false);
    }
    public void openHomePainel()
    {
        homePainel.SetActive(true);

        comPainel.SetActive(false);
        quizPainel.SetActive(false);
        optionsPainel.SetActive(false);
        statsPainel.SetActive(false);
    }
    public void openStatsPainel()
    {
        statsPainel.SetActive(true);

        comPainel.SetActive(false);
        quizPainel.SetActive(false);
        optionsPainel.SetActive(false);
        homePainel.SetActive(false);
    }
    public void openOptionsPainel()
    {
        optionsPainel.SetActive(true);

        comPainel.SetActive(false);
        quizPainel.SetActive(false);
        statsPainel.SetActive(false);
        homePainel.SetActive(false);
    }
    public void BackNavBar()
    {
        homePainel.SetActive(true);

        optionsPainel.SetActive(false);
        comPainel.SetActive(false);
        quizPainel.SetActive(false);
        statsPainel.SetActive(false);
    }

    public void Logout()
    {
        PlayerPrefs.DeleteAll();
        optionsPainel.SetActive(false);
        homePainel.SetActive(true);
        initPainel.SetActive(false);
        indexPainel.SetActive(true);
    }

    public void ExitEndQuiz()
    {
        portoQuiz.SetActive(false);
        endQuiz.SetActive(false);
        quizPainel.SetActive(true);
    }

    public void ExploreAR_DevMode()
    {
        SceneManager.LoadScene("DevMode");
    }

}
