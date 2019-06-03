using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using System.IO;
using UnityEngine.UI;
using UnityEngine.XR.WSA.Input;

public class BuildingScript : MonoBehaviour
{


    GestureRecognizer gr = null;

    public Camera mainCamera;

    public GameObject viveCamera;

    public GameObject Ground;
    public GameObject Floor1;
    public GameObject Roof;


    public TextAsset ACFile;
    public TextAsset OAPUFile;

    public Mesh cylinderMesh;


    GameObject wallIntF1_1;
    GameObject wallIntF1_2;
    GameObject wallIntG;


    GameObject wallExtF1;
    GameObject wallExtG;

    bool movingCamera = false;
    Vector3 targetTransCam;
    Vector3 currentTransCam;
    Vector3 whereToLook;

    List<GameObject> IntWallList;
    List<GameObject> ExtWallList;

    public Dictionary<string, GameObject> sensorsF1;
    public Dictionary<string, GameObject> sensorsG;

    private bool play;
    private DateTime present;

    private float nexTime;
    private float delta = 5;

    private bool isExploded;
    private bool isMagnified;


    public Text simulationText;

    private Vector3 realCentre;

    private int explosionIndex = 0;

    // brushing use
    GameObject leftHighlightedSensor;
    GameObject rightHighlightedSensor;

    // Use this for initialization
    void Start()
    {
        Floor1.transform.localPosition += Vector3.up * 3;
        play = false;
        isExploded = false;
        isMagnified = false;

        realCentre = this.GetComponent<BoxCollider>().center;

        this.GetComponent<Collider>().enabled = false;

        IntWallList = new List<GameObject>();
        ExtWallList = new List<GameObject>();
        sensorsF1 = new Dictionary<string, GameObject>();
        sensorsG = new Dictionary<string, GameObject>();

        GameObject childFloor = Floor1.transform.Find("Floor").gameObject;
        GameObject childChildFloor = childFloor.transform.Find("floor").gameObject;
        childChildFloor.GetComponent<Renderer>().material.color = new Color(217f / 255f, 217f / 255f, 217f / 255f);

        GameObject childFloorGround = Ground.transform.Find("Plane001").gameObject;
        childFloorGround.GetComponent<Renderer>().material.color = new Color(82f / 255f, 82f / 255f, 82f / 255f);

        wallIntF1_1 = Floor1.transform.Find("wallInt1").gameObject.transform.Find("Internal Wall 1").gameObject;
        wallIntF1_2 = Floor1.transform.Find("wallInt2").gameObject.transform.Find("Internal Wall 2").gameObject;
        wallIntG = Ground.transform.Find("wallInt").gameObject.transform.Find("Internal walls").gameObject;

        IntWallList.Add(wallIntF1_1);
        IntWallList.Add(wallIntF1_2);
        IntWallList.Add(wallIntG);

        wallExtF1 = Floor1.transform.Find("extWall").gameObject.transform.Find("External Wall").gameObject;
        wallExtG = Ground.transform.Find("extWall").gameObject.transform.Find("External walls").gameObject;

        ExtWallList.Add(wallExtF1);
        ExtWallList.Add(wallExtG);

        Color darkGray = new Color();
        ColorUtility.TryParseHtmlString("#525252", out darkGray);

        Color lightGray = new Color();
        ColorUtility.TryParseHtmlString("#969696", out lightGray);

        Color blueSensor = new Color();
        ColorUtility.TryParseHtmlString("#1d91c0", out blueSensor);

        foreach (GameObject wall in IntWallList)
        {
            wall.GetComponent<Renderer>().material.color = lightGray;
        }

        foreach (GameObject wall in ExtWallList)
        {
            wall.GetComponent<Renderer>().material.color = lightGray;
        }

        sensorsG = getSensorG();
        sensorsF1 = getSensorF1();

        //Put down all the sensors
        foreach (KeyValuePair<string, GameObject> de in sensorsG)
        {
            de.Value.transform.position = new Vector3(de.Value.transform.position.x, Ground.transform.Find("Plane001").position.y, de.Value.transform.position.z);

            if (de.Value.GetComponent<SensorScript>() == null)
            {
                de.Value.transform.localPosition = new Vector3(de.Value.transform.localPosition.x, de.Value.transform.localPosition.y - 1, de.Value.transform.localPosition.z);
                de.Value.SetActive(false);
            }
        }
        foreach (KeyValuePair<string, GameObject> de in sensorsF1)
        {
            de.Value.transform.position = new Vector3(de.Value.transform.position.x, Floor1.transform.Find("Floor").position.y, de.Value.transform.position.z);

            if (de.Value.GetComponent<SensorScript>() == null)
            {
                de.Value.transform.localPosition = new Vector3(de.Value.transform.localPosition.x, de.Value.transform.localPosition.y - 1, de.Value.transform.localPosition.z);
                de.Value.SetActive(false);
            }
        }

        // find vive camera
        viveCamera = GameObject.Find("Camera (eye)");
    }

    

    public Dictionary<string, GameObject> getSensorG()
    {

        Color lightGray = new Color();
        ColorUtility.TryParseHtmlString("#969696", out lightGray);

        Dictionary<string, GameObject> temp = new Dictionary<string, GameObject>();

        foreach (Transform child in Ground.transform.Find("Sensor"))
        {
            temp.Add(child.gameObject.name.Trim(), child.gameObject);
            child.gameObject.GetComponent<Renderer>().material.color = lightGray;

            child.GetComponent<MeshFilter>().mesh = cylinderMesh;
            if (child.GetComponent<Collider>() != null)
            {
                child.GetComponent<Collider>().enabled = false;
            }

            //child.gameObject.AddComponent(typeof(CapsuleCollider));

            child.localScale = new Vector3(0.6f, 0.1f, 0.6f);
            child.localPosition = new Vector3(child.localPosition.x, child.localPosition.y - 1f, child.localPosition.z);
            child.eulerAngles = new Vector3(0, 0, 0);
            if (child.name.Contains("ACG."))
            {
                if (int.Parse(child.name.Substring(4)) <= 4 && int.Parse(child.name.Substring(4)) >= 1)
                {
                    child.gameObject.AddComponent(typeof(CapsuleCollider));
                    child.GetComponent<CapsuleCollider>().radius = 1;
                    child.GetComponent<CapsuleCollider>().height = 3;
                }
                else
                {
                    //if (child.GetComponent<Collider>() != null)
                    //{
                    //    child.GetComponent<Collider>().enabled = false;
                    //}
                }
            }
        }



        return temp;
    }

    public Dictionary<string, GameObject> getSensorF1()
    {
        Color lightGray = new Color();
        ColorUtility.TryParseHtmlString("#969696", out lightGray);

        Dictionary<string, GameObject> temp = new Dictionary<string, GameObject>();
        foreach (Transform child in Floor1.transform.Find("Sensor"))
        {
            temp.Add(child.gameObject.name.Trim(), child.gameObject);
            child.gameObject.GetComponent<Renderer>().material.color = lightGray;

            child.GetComponent<MeshFilter>().mesh = cylinderMesh;
            if (child.GetComponent<Collider>() != null)
            {
                child.GetComponent<Collider>().enabled = false;
            }
            //child.gameObject.AddComponent(typeof(CapsuleCollider));

            child.localScale = new Vector3(0.6f, 0.1f, 0.6f);
            child.localPosition = new Vector3(child.localPosition.x, child.localPosition.y - 1f, child.localPosition.z);
            child.eulerAngles = new Vector3(0, 0, 0);
            if (child.name.Contains("AC1."))
            {
                if ((int.Parse(child.name.Substring(4)) <= 9 && int.Parse(child.name.Substring(4)) >= 3) || int.Parse(child.name.Substring(4)) == 12)
                {
                    child.gameObject.AddComponent(typeof(CapsuleCollider));
                    child.GetComponent<CapsuleCollider>().radius = 1;
                    child.GetComponent<CapsuleCollider>().height = 3;
                }
                else
                {
                    //if (child.GetComponent<Collider>() != null)
                    //{
                    //    child.GetComponent<Collider>().enabled = false;
                    //}
                }
            }

        }
        return temp;
    }

    //public void HighlightSensor(List<GameObject> sensors)
    //{
    //    List<GameObject> highlightedSensors = new List<GameObject>();
    //    foreach (GameObject sensor in sensors) {
    //        foreach (Transform child in Ground.transform.Find("Sensor"))
    //        {
    //            SetColorForGO(child.gameObject, 0.1f);
    //            if (child.name == sensor.name)
    //            {
    //                highlightedSensors.Add(child.gameObject);
    //            }
    //        }

    //        foreach (Transform child in Floor1.transform.Find("Sensor"))
    //        {
    //            SetColorForGO(child.gameObject, 0.1f);
    //            if (child.name == sensor.name)
    //            {
    //                highlightedSensors.Add(child.gameObject);
    //            }
    //        }
    //    }

    //    foreach (GameObject sensor in highlightedSensors) {
    //        SetColorForGO(sensor, 1f);
    //    }
  
    //}

    
    public void ResetHighlightSensor()
    {
        foreach (Transform child in Ground.transform.Find("Sensor"))
        {
            SetColorForGO(child.gameObject, 1);
        }
        foreach (Transform child in Floor1.transform.Find("Sensor"))
        {
            SetColorForGO(child.gameObject, 1);
        }
    }

    private void SetColorForGO(GameObject go, float transparency)
    {
        Color tmpColor = go.GetComponent<Renderer>().material.color;
        tmpColor.a = transparency;
        go.GetComponent<Renderer>().material.color = tmpColor;
        go.GetComponent<Renderer>().material.SetColor("_OutlineColor", tmpColor);
    }


    void Awake()
    {
        present = new DateTime(2017, 3, 6, 6, 30, 00);
    }

    private void Tap(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        this.explode();
    }

    public bool getPlay()
    {
        return play;
    }

    public DateTime getDT()
    {
        return this.present;
    }

    public Vector3 getCentreCoordinates()
    {
        return transform.TransformPoint(this.realCentre);
    }

    void to2DView()
    {

        Roof.SetActive(false);
        foreach (GameObject wall in IntWallList)
        {
            wall.transform.localScale = new Vector3(1f, 1f, 0.1f);
        }

        foreach (GameObject wall in ExtWallList)
        {
            wall.transform.localScale = new Vector3(1f, 1f, 0.1f);
        }



        GameObject childFloor = Floor1.transform.Find("Floor").gameObject;
        GameObject childChildFloor = childFloor.transform.Find("floor").gameObject;

        GameObject childFloorGround = Ground.transform.Find("Plane001").gameObject;

        Vector3 posF1 = childChildFloor.transform.position;
        Vector3 posG = childFloorGround.transform.position;

        Vector3 targPos = posG + new Vector3(0, 0, 20);

        Vector3 targetTrans = targPos - posF1;

        Vector3 midCD = posG + (posG + targPos) / 2;
        Vector3 result = midCD + new Vector3(0, 40, 0);

        whereToLook = midCD;
        movingCamera = true;
        targetTransCam = result;
        currentTransCam = new Vector3(0, 0, 0);
    }

    private void GetWalls() {
        IntWallList = new List<GameObject>();
        ExtWallList = new List<GameObject>();

        GameObject childFloor = Floor1.transform.Find("Floor").gameObject;
        GameObject childChildFloor = childFloor.transform.Find("floor").gameObject;
        childChildFloor.GetComponent<Renderer>().material.color = new Color(217f / 255f, 217f / 255f, 217f / 255f);

        GameObject childFloorGround = Ground.transform.Find("Plane001").gameObject;
        childFloorGround.GetComponent<Renderer>().material.color = new Color(82f / 255f, 82f / 255f, 82f / 255f);

        wallIntF1_1 = Floor1.transform.Find("wallInt1").gameObject.transform.Find("Internal Wall 1").gameObject;
        wallIntF1_2 = Floor1.transform.Find("wallInt2").gameObject.transform.Find("Internal Wall 2").gameObject;
        wallIntG = Ground.transform.Find("wallInt").gameObject.transform.Find("Internal walls").gameObject;

        IntWallList.Add(wallIntF1_1);
        IntWallList.Add(wallIntF1_2);
        IntWallList.Add(wallIntG);

        wallExtF1 = Floor1.transform.Find("extWall").gameObject.transform.Find("External Wall").gameObject;
        wallExtG = Ground.transform.Find("extWall").gameObject.transform.Find("External walls").gameObject;

        ExtWallList.Add(wallExtF1);
        ExtWallList.Add(wallExtG);

        Color darkGray = new Color();
        ColorUtility.TryParseHtmlString("#525252", out darkGray);

        Color lightGray = new Color();
        ColorUtility.TryParseHtmlString("#969696", out lightGray);


        foreach (GameObject wall in IntWallList)
        {
            wall.GetComponent<Renderer>().material.color = lightGray;
        }

        foreach (GameObject wall in ExtWallList)
        {
            wall.GetComponent<Renderer>().material.color = lightGray;
        }
    }

    public void removeRoof()
    {
        GetWalls();
        Roof.SetActive(false);
        if (IntWallList != null) {
            foreach (GameObject wall in IntWallList)
            {
                wall.transform.localScale = new Vector3(1f, 1f, 0.1f);
            }
        }

        if (ExtWallList != null)
        {
            foreach (GameObject wall in ExtWallList)
            {
                wall.transform.localScale = new Vector3(1f, 1f, 0.1f);
            }
        }

        GameObject childFloor = Floor1.transform.Find("Floor").gameObject;
        GameObject childChildFloor = childFloor.transform.Find("floor").gameObject;

        GameObject childFloorGround = Ground.transform.Find("Plane001").gameObject;

        Vector3 posF1 = childChildFloor.transform.position;
        Vector3 posG = childFloorGround.transform.position;

        Vector3 transFloor1 = new Vector3(0, 0.1f, 0);

        Vector3 posFinalF1 = posF1 + transFloor1;

        isExploded = true;
    }

    public void putBackRoof()
    {
        Roof.SetActive(true);
        foreach (GameObject wall in IntWallList)
        {
            wall.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        foreach (GameObject wall in ExtWallList)
        {
            wall.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        GameObject childFloor = Floor1.transform.Find("Floor").gameObject;
        GameObject childChildFloor = childFloor.transform.Find("floor").gameObject;

        GameObject childFloorGround = Ground.transform.Find("Plane001").gameObject;

        Vector3 posF1 = childChildFloor.transform.position;
        Vector3 posG = childFloorGround.transform.position;

        Vector3 transFloor1 = new Vector3(0, -0.1f, 0);

        Vector3 posFinalF1 = posF1 + transFloor1;

        isExploded = false;
    }

    public void explode()
    {

        if (this.isExploded)
        {
            putBackRoof();
        }
        else
        {
            removeRoof();
        }
    }

    public bool IsExploded()
    {
        return isExploded;
    }

    public void SetMagnify(bool magnify)
    {
        this.isMagnified = magnify;
    }


    public bool IsMagnified()
    {
        return isMagnified;
    }


    public void ChangeExplosion()
    {

        if (explosionIndex == 0)
        {
            explosionIndex++;
            removeRoof();
        }
        else if (explosionIndex == 1)
        {
            explosionIndex++;
            putBackRoof();
            SensorOnly();
        }
        else if (explosionIndex == 2)
        {
            explosionIndex = 0;
            ResetBuildingFromSensorOnly();
        }
    }

    public int CurrentExplosion()
    {
        return explosionIndex;
    }

    void SensorOnly()
    {
        Color lightGray = new Color();
        ColorUtility.TryParseHtmlString("#969696", out lightGray);

        Roof.SetActive(false);
        foreach (GameObject wall in IntWallList)
        {
            wall.SetActive(false);
        }

        foreach (GameObject wall in ExtWallList)
        {
            wall.SetActive(false);
        }

        GameObject childFloor = Floor1.transform.Find("Floor").gameObject;
        childFloor.SetActive(false);

        GameObject block = Floor1.transform.Find("Block:block 131").gameObject;
        block.SetActive(false);

        GameObject childFloorGround = Ground.transform.Find("Plane001").gameObject;
        childFloorGround.SetActive(false);
        if (sensorsG != null && sensorsF1 != null)
        {
            foreach (KeyValuePair<string, GameObject> de in sensorsG)
            {
                if (de.Value.GetComponent<Renderer>().material.color == lightGray)
                {
                    de.Value.SetActive(false);
                }
            }

            foreach (KeyValuePair<string, GameObject> de in sensorsF1)
            {
                if (de.Value.GetComponent<Renderer>().material.color == lightGray)
                {
                    de.Value.SetActive(false);
                }
            }
        }


        isExploded = true;
    }

    void ResetBuildingFromSensorOnly()
    {
        Color lightGray = new Color();
        ColorUtility.TryParseHtmlString("#969696", out lightGray);

        Roof.SetActive(true);
        foreach (GameObject wall in IntWallList)
        {
            wall.SetActive(true);
        }

        foreach (GameObject wall in ExtWallList)
        {
            wall.SetActive(true);
        }

        GameObject childFloor = Floor1.transform.Find("Floor").gameObject;
        childFloor.SetActive(true);

        GameObject block = Floor1.transform.Find("Block:block 131").gameObject;
        block.SetActive(true);

        GameObject childFloorGround = Ground.transform.Find("Plane001").gameObject;
        childFloorGround.SetActive(true);

        if (sensorsG != null)
        {
            foreach (KeyValuePair<string, GameObject> de in sensorsG)
            {
                if (de.Value.GetComponent<Renderer>().material.color == lightGray)
                {
                    de.Value.SetActive(true);
                }
            }
        }

        if (sensorsF1 != null)
        {
            foreach (KeyValuePair<string, GameObject> de in sensorsF1)
            {
                if (de.Value.GetComponent<Renderer>().material.color == lightGray)
                {
                    de.Value.SetActive(true);
                }
            }
        }


        isExploded = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
