using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSwitchController : MonoBehaviour
{
    public CPUDragonController CPUDragon;
    public PlayerDragonController PlayerDragon;

    public static bool isPlayerTurn;

    // Start is called before the first frame update
    void Start()
    {
        //randomly define who's the first turn is
        if (Random.Range(0, 2) > 0) isPlayerTurn = false;
        else isPlayerTurn = true;
    }


}
