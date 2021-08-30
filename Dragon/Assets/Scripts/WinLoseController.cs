using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinLoseController : MonoBehaviour
{
    [SerializeField]
    private Button WinLoseButton;
    [SerializeField]
    private Text WinLoseButtonText;
    private MenuManager menuManagerObject;

    private void Start()
    {
        menuManagerObject = FindObjectOfType<MenuManager>();
    }

    public void DetermineTheWinner(bool theWinnerIsPlayer) {
        if (theWinnerIsPlayer) {
            WinLoseButtonText.text = "YouWin";
            WinLoseButton.GetComponent<Image>().color = Color.green;
        }
        else
        {
            WinLoseButtonText.text = "YouLose";
            //WinLoseButtonText.SetText("YouLose");
            WinLoseButton.GetComponent<Image>().color = Color.red;
        }
        WinLoseButton.gameObject.SetActive(true);
    }

    public void WinLoseButtonPushed() {
        menuManagerObject.MenuPanelActivation();
        WinLoseButton.gameObject.SetActive(false);
    }
}
