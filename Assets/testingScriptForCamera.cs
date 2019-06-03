using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testingScriptForCamera : MonoBehaviour {

    GameObject testingBall;
	// Use this for initialization
	void Start () {
        testingBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        testingBall.transform.localScale = Vector3.one * 0.1f;

        testingBall.GetComponent<Renderer>().material.color = Color.red;
    }

    // Update is called once per frame
    void Update () {
        Vector3 cameraF = Camera.main.transform.localPosition + Camera.main.transform.forward * 2f;
        testingBall.transform.SetParent(Camera.main.transform.parent);
        testingBall.transform.position = cameraF;
        testingBall.transform.position = new Vector3(testingBall.transform.position.x, 1, testingBall.transform.position.z);
    }
}
