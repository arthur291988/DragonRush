
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject CPUDragon;
    public GameObject PlayerDragon;
    [SerializeField]
    private Image MenuPanel;

    public RawImage playerDragonsLifeBarObject;
    public RawImage CPUDragonsLifeBarObject;


    public void MenuButtonPushed(int index) {
        if (index == 0) {
            PlayerDragon.SetActive(true); //player dragon shoul be enabled first
            CPUDragon.SetActive(true);
        }
        MenuPanel.gameObject.SetActive(false);
    }

    public void MenuPanelActivation()
    {
        PlayerDragon.SetActive(false);
        playerDragonsLifeBarObject.gameObject.SetActive(false);
        CPUDragon.SetActive(false);
        CPUDragonsLifeBarObject.gameObject.SetActive(false);
        MenuPanel.gameObject.SetActive(true);
    }
}
