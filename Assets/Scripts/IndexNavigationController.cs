using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IndexNavigationController : MonoBehaviour
{
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
    #endregion

    #region init
    //init
    #region paineis
    public GameObject quizPainel;
    public GameObject comPainel;
    public GameObject homePainel;
    public GameObject statsPainel;
    public GameObject optionsPainel;
    #endregion
    
    #region navbar
    public Button quizNavButton;
    public Button comNavButton;
    public Button homeNavButton;
    public Button statsNavButton;
    public Button optionsNavButton;

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

    private void Start()
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
        comPainel.SetActive(true);

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

}
