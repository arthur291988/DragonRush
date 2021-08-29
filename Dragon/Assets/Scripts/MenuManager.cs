using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject CPUDragon;
    public GameObject PlayerDragon;
    [SerializeField]
    private List<Button> AllMenuButtons;
    public void MenuButtonPushed(int index) {
        if (index == 0) {
            PlayerDragon.SetActive(true); //player dragon shoul be enabled first
            CPUDragon.SetActive(true);
        }
        for (int i = 0; i < AllMenuButtons.Count; i++) {
            AllMenuButtons[i].gameObject.SetActive(false);
        }
    }
}
