using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ButtonClick : MonoBehaviour
{
    // Start is called before the first frame update
    public Text text3;
    public TextMeshProUGUI text;
    public GameObject viewMore;
    //public Button botao;

    void Start()
    {
        text3 = GameObject.Find("texttest").GetComponent<Text>();
        //Button botaoComponent = botao.GetComponent<Button>();
        /**
        if (botaoComponent != null)
        {
            // Adiciona um ouvinte para o evento de clique do botão
            botaoComponent.onClick.AddListener(Click);
        }
        else
        {
            text.text = "button é null";
        }
        */
        //botao.onClick.AddListener(Click);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Click()
    {
        viewMore.SetActive(true);
    }
}
