using Assets.SimpleLocalization.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiLanguage : MonoBehaviour
{
    // Start is called before the first frame update
    public Dropdown languageDropdown;
    public Dropdown languageDropdown2;
    public Dropdown languageDropdownExplorationScene;

    private void Start()
    {
        if(languageDropdown != null && languageDropdown2 != null)
        {
            languageDropdown.onValueChanged.AddListener(delegate {
                DropdownValueChanged(languageDropdown, languageDropdown2);
            });
        }
        
    }
    private void Awake()
    {
        LocalizationManager.Read();
        //LocalizationManager.Language = "Portuguese";
        if (languageDropdown != null && languageDropdown.value != 0)
        {
            LocalizationManager.Language = "English";
        }

        if (languageDropdownExplorationScene != null && languageDropdownExplorationScene.value != 0)
        {
            LocalizationManager.Language = "English";
        }

        // Adicionar um listener para o evento de mudança de valor da dropdown
        if (languageDropdown != null)
        {
            Debug.Log("mudar lingua");
            languageDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }
        if (languageDropdown2 != null)
        {
            Debug.Log("mudar lingua");
            languageDropdown2.onValueChanged.AddListener(OnDropdownValueChanged);
        }

        if (languageDropdownExplorationScene != null)
        {
            Debug.Log("mudar lingua exploration");
            languageDropdownExplorationScene.onValueChanged.AddListener(OnDropdownValueChanged);
        }
    }

    private void OnDropdownValueChanged(int index)
    {
        string languageCode;
        if (index == 0)
        {
            languageCode = "en";
            Debug.Log("mudou pra ingles");
            LocalizationManager.Language = "English";
            PlayerPrefs.SetString("Language", languageCode);
        }
        else
        {
            languageCode = "pt";
            Debug.Log("mudou pra pt");
            LocalizationManager.Language = "Portuguese";
            PlayerPrefs.SetString("Language", languageCode);
        }
    }

    void DropdownValueChanged(Dropdown change, Dropdown targetDropdown)
    {
        // Define o valor do dropdown 2 para ser o mesmo que o dropdown 1
        targetDropdown.value = change.value;
    }
}
