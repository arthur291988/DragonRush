using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class TrajectoryRenderer : MonoBehaviour
{
    private LineRenderer LineRenderer;


    // Start is called before the first frame update
    void Start()
    {
        LineRenderer = GetComponent<LineRenderer>();
    }

    public void ShowTrajectoryOfPlayerDragon(Vector3 originCoordinates, Vector3 speedRush) {
        Vector3[] pointOfLine = new Vector3[50];

        LineRenderer.positionCount = pointOfLine.Length;
        for (int i=0; i < pointOfLine.Length;i++) {
            float time = i * 0.1f;
            pointOfLine[i] = originCoordinates + speedRush * time + Physics.gravity * time * time / 2f;
        }
        LineRenderer.SetPositions(pointOfLine);
    }

    public void TurnOnOffLineOfTrajectory(bool turnOn) {
        LineRenderer.enabled = turnOn;
    }
}
