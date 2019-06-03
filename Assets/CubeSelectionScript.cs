using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSelectionScript : MonoBehaviour {
    public GameObject cubeSelectionPrefab;

    [HideInInspector]
    public Vector3 leftControllerStartPressingPosition = Vector3.zero;
    [HideInInspector]
    public Vector3 rightControllerStartPressingPosition = Vector3.zero;
    [HideInInspector]
    public GameObject currentLeftCube = null;
    [HideInInspector]
    public GameObject currentRightCube = null;

    Transform leftController;
    Transform rightController;

    

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {

        if (GameObject.Find("Controller (left)") != null) {
            if (GameObject.Find("Controller (left)").transform.GetChild(0) != null) {
                if (GameObject.Find("Controller (left)").transform.GetChild(0).Find("tip") != null)
                {
                    Transform tip = GameObject.Find("Controller (left)").transform.GetChild(0).Find("tip");
                    if (tip.GetChild(0) != null)
                    {
                        Transform attach = tip.GetChild(0);
                        if (attach.GetChild(0) != null)
                        {
                            Transform touchBar = attach.GetChild(0);
                            if (touchBar.GetChild(0) != null)
                                leftController = touchBar.GetChild(0);
                        }
                    }
                }
            }
        }

        if (GameObject.Find("Controller (right)") != null)
        {
            if (GameObject.Find("Controller (right)").transform.GetChild(0) != null)
            {
                if (GameObject.Find("Controller (right)").transform.GetChild(0).Find("tip") != null)
                {
                    Transform tip = GameObject.Find("Controller (right)").transform.GetChild(0).Find("tip");
                    if (tip.GetChild(0) != null)
                    {
                        Transform attach = tip.GetChild(0);
                        if (attach.GetChild(0) != null)
                        {
                            Transform touchBar = attach.GetChild(0);
                            if (touchBar.GetChild(0) != null)
                                rightController = touchBar.GetChild(0);
                        }
                    }
                }
            }
        }

        if (leftControllerStartPressingPosition != Vector3.zero) {
            if (currentLeftCube != null) {
                float x = Vector3.Distance(leftController.position, leftControllerStartPressingPosition) / Mathf.Sqrt(3);
                currentLeftCube.transform.localScale = Vector3.one * x;
                currentLeftCube.transform.position = leftControllerStartPressingPosition;

            }
        }

        if (rightControllerStartPressingPosition != Vector3.zero)
        {
            if (currentRightCube != null)
            {
                //float x = Vector3.Distance(rightController.position, rightControllerStartPressingPosition) / Mathf.Sqrt(3);


                //Vector3 adjustedV = new Vector3(rightController.position.x - x, rightController.position.y - x, rightController.position.z);
                //currentRightCube.transform.rotation = Quaternion.LookRotation(adjustedV - rightControllerStartPressingPosition);
                currentRightCube.transform.rotation = Quaternion.LookRotation(rightController.position - rightControllerStartPressingPosition);
                currentRightCube.transform.localEulerAngles = new Vector3(0, currentRightCube.transform.localEulerAngles.y + 45, 0);
                //float angle = Vector3.SignedAngle((rightControllerStartPressingPosition - Vector3.one * rightControllerStartPressingPosition.y), (rightController.position - rightControllerStartPressingPosition), Vector3.up);
                //currentRightCube.transform.localEulerAngles = new Vector3(0, angle, 0);

                //currentRightCube.transform.localScale = Vector3.one * Vector3.Distance(rightController.position, rightControllerStartPressingPosition) / Mathf.Sqrt(3);
                float horizontalDistance = Vector3.Distance(rightController.position, new Vector3(rightControllerStartPressingPosition.x, rightController.position.y, rightControllerStartPressingPosition.z));
                currentRightCube.transform.localScale = Vector3.one * Mathf.Min(Mathf.Abs(rightController.position.y - rightControllerStartPressingPosition.y), horizontalDistance / Mathf.Sqrt(2));
                currentRightCube.transform.position = (rightControllerStartPressingPosition + rightController.position) / 2;

            }
        }
    }

    public GameObject CreateCube() {
        GameObject cube = (GameObject)Instantiate(cubeSelectionPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        cube.transform.localScale = Vector3.zero;

        return cube;
    }
}
