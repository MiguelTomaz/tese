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

    private void Start()
    {
        languageDropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(languageDropdown, languageDropdown2);
        });
    }
    private void Awake()
    {
        LocalizationManager.Read();
        //LocalizationManager.Language = "Portuguese";
        if (languageDropdown != null && languageDropdown.value != 0)
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
    }

    private void OnDropdownValueChanged(int index)
    {
        if (index == 0)
        {
            Debug.Log("mudou pra ingles");
            LocalizationManager.Language = "English";
        }
        else
        {
            Debug.Log("mudou pra pt");
            LocalizationManager.Language = "Portuguese";
        }
    }

    void DropdownValueChanged(Dropdown change, Dropdown targetDropdown)
    {
        // Define o valor do dropdown 2 para ser o mesmo que o dropdown 1
        targetDropdown.value = change.value;
    }
}
