using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class testingScript : MonoBehaviour {

    public GameObject testGO;
    public GameObject text;

    GameObject leftController;
    bool calibrationFlag = false;
    GameObject mainCamera;
    bool afterCalibration = false;


    // Use this for initialization
    void Start()
    {
    }


    public void OpenMainCamera() {
        GameObject.Find("[CameraRig]").transform.GetChild(2).gameObject.SetActive(true);
        afterCalibration = true;
        
        if (PupilTools.IsConnected)
        {
            PupilTools.SubscribeTo("gaze");
            PupilTools.IsGazing = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        leftController = GameObject.Find("Controller (left)");
        mainCamera = GameObject.Find("Camera (eye)");

        if (leftController != null) {
            SteamVR_TrackedControllerOrigin ltc = leftController.GetComponent<SteamVR_TrackedControllerOrigin>();
            if (ltc.padPressed) {
                if(!calibrationFlag)
                    calibrationFlag = true;
            }
        }

        if (calibrationFlag) {
            calibrationFlag = false;
            if (GameObject.Find("Pupil Manager") != null) {
                if (mainCamera != null) {
                    mainCamera.SetActive(false);
                    GameObject.Find("Pupil Manager").transform.GetChild(2).gameObject.SetActive(true);
                    afterCalibration = false;
                }
            }
            
        }
        if (PupilTools.IsConnected && PupilTools.IsGazing && afterCalibration)
        {
            Vector2 gazePointCenter = PupilData._2D.GazePosition;

            Vector3 viewportPoint = new Vector3(gazePointCenter.x, gazePointCenter.y, 1f);
            Ray ray = Camera.main.ViewportPointToRay(viewportPoint);
            Debug.DrawRay(ray.origin, ray.direction, Color.green);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                text.GetComponent<Text>().text = "I'm looking at " + hit.transform.name;
            }
            else {
                text.GetComponent<Text>().text = "I'm looking at nothing!";
            }
            // see this for ray stop point
            //if (Physics.Raycast(ray, out hit))
            //{
            //    heading.SetPosition(1, hit.point);
            //}
            //else
            //{
            //    heading.SetPosition(1, ray.origin + ray.direction * 50f);
            //}
        }
    }

    
}
