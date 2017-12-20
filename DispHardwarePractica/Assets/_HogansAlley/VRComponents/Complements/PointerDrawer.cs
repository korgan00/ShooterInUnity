using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawRadar : MonoBehaviour {

    public float thetaScale = 0.01f;
    public float radius = 3f;
    private int size;
    private LineRenderer lineDrawer;
    private float theta = 0f;

    void Start() {
        lineDrawer = GetComponent<LineRenderer>();
    }

    void Update() {
        theta = 0f;
        size = (int)((1f / thetaScale) + 1f);
        lineDrawer.positionCount = size;

        for (int i = 0; i < size; i++) {
            theta += (2.0f * Mathf.PI * thetaScale);
            float x = radius * Mathf.Cos(theta);
            float y = radius * Mathf.Sin(theta);
            lineDrawer.SetPosition(i, new Vector3(x, y, 0));
        }
    }

}