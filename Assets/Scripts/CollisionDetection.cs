using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollisionDetection : MonoBehaviour
{
    public GameObject FilterPrefab;

    SmallMultiplesManagerScript smms;
    List<GameObject> sm;

    // dataset 1
    Collider obj = null;
    GameObject currentLeftHighlight;
    GameObject currentRightHighlight;
    int leftGroundSensor = 0;
    int rightGroundSensor = 0;
    GameObject keepLeftHighlightGO;
    GameObject keepRightHighlightGO;

    // dataset 2
    GameObject filterCoordinitor;
    GameObject axisCoordinitor;
    //private List<GameObject> leftCountryFilters;
    //private List<GameObject> rightCountryFilters;
    //private List<GameObject> leftYearFilters;
    //private List<GameObject> rightYearFilters;
    //private List<GameObject> leftValueFilters;
    //private List<GameObject> rightValueFilters;
    //private List<GameObject> leftValuePlanes;
    //private List<GameObject> rightValuePlanes;
    [HideInInspector]
    public int touchedIndexOfColorFilter = -1;


    [HideInInspector]
    public GameObject touchedAxis = null;
    int axisSegment = 0;

    // filtering
    [HideInInspector]
    public GameObject draggedFilter = null;
    bool canGrab = false;
    int segment = 0;

    // cube selection
    [HideInInspector]
    public Vector3 controllerStartPressingPosition = Vector3.zero;
    //[HideInInspector]
    //public Vector3 rightControllerStartPressingPosition = Vector3.zero;
    //[HideInInspector]
    //public GameObject currentLeftCube = null;
    [HideInInspector]
    public GameObject currentCube = null;
    Transform currentCubeParent = null;

    GameObject cubeSelectionPrefab;
    public bool answerSelected = false;
    bool answerTouched = false;
    GameObject touchedAnswerGO = null;
    GameObject selectedAnswerGO = null;

    public int BIMlvlIndex = -1; // -1: undefined, 0:both, 1:lower, 2: upper

    Vector2 BarYAxisIndex = new Vector2(0, 1);
    public Vector2 selectedBarYAxisIndex = new Vector2(0, 1);


    // Use this for initialization
    void Start()
    {
        
        smms = GameObject.Find("SmallMultiplesManager").GetComponent<SmallMultiplesManagerScript>();
        sm = new List<GameObject>();

        cubeSelectionPrefab = smms.cubeSelectionPrefab;

        if (GameObject.Find("SmallMultiplesManager").transform.childCount > 0)
        {
            foreach (Transform t in GameObject.Find("SmallMultiplesManager").transform.GetChild(0))
            {
                if (t.name.Contains("Small Multiples"))
                {
                    sm.Add(t.gameObject);
                }
            }
        }

        //if (smms.dataset == 2)
        //{
            filterCoordinitor = new GameObject();
            filterCoordinitor.name = "filterCoordinitor";

            axisCoordinitor = new GameObject();
            axisCoordinitor.name = "axisCoordinitor";
        //}
        if (transform.parent.parent.parent.parent.parent.name == "Controller (right)")
        {
            this.name = "rightCollisionDetector";
        }
        else {
            this.name = "leftCollisionDetector";
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (smms.dataset == 1) {
            FilterManipulationD1();
            AxisManipulationD1();
        }
        if (smms.dataset == 2)
        {
            FilterManipulation();
            AxisManipulation();
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (smms.dataset == 1)
        {
            if (!smms.colorFilterOn && other.transform.parent.name.Equals("Sensor"))
            {
                SensorCollisionEnter(other);
            }
            if ((transform.parent.parent.parent.parent.parent.name == "Controller (right)" && smms.trialState == TrialState.OnTask) || (smms.trialState == TrialState.PreTask && smms.interactionTrainingCount == 6))
            {
                //if (other.transform.parent.name.Contains("PanelItem"))
                    //smms.ColorFilterEnterCollision();
            }
        }

        if (other.name == "Box")
        {
            if (touchedAxis == null)
            {
                touchedAxis = other.gameObject;
            }
            else
            {
                if (other.gameObject != touchedAxis)
                {
                    //canGrab = false;
                    touchedAxis.GetComponent<Renderer>().material.color = Color.white;
                    touchedAxis = null;
                    touchedAxis = other.gameObject;
                }
            }
        }

        if ((transform.parent.parent.parent.parent.parent.name == "Controller (right)" && smms.trialState == TrialState.Answer) || (smms.trialState == TrialState.PreTask && smms.interactionTrainingCount == 0))
        {
            AnswerCollisionEnter(other);
        }
    }

    private void ColorFilterCollisionStay(Collider other) {
        if (other.transform.parent.name.Contains("PanelItem")) {
            touchedIndexOfColorFilter = int.Parse(other.transform.parent.name[9] + "");
            //Debug.Log(touchedIndexOfColorFilter);
            if (smms.colorFilterActive) {
                smms.ColorFilterForBIMSensors(touchedIndexOfColorFilter);
            }
        }
    }

    private void AnswerCollisionEnter(Collider other)
    {
        if (other.transform.parent.name.Contains("PanelItem") && !other.transform.parent.name.Equals("PanelItemConfirm"))
        {
            //for (int i = 0; i < other.transform.parent.parent.childCount; i++) {
            //    other.transform.parent.parent.GetChild(i).GetChild(0).GetComponent<Text>().color = Color.black;
            //}
            //Transform textItem = other.transform.parent.GetChild(0);
            //textItem.GetComponent<Text>().color = Color.green;
            //Debug.Log(textItem.GetComponent<Text>().text);
            answerTouched = true;
            if (touchedAnswerGO != null) {
                touchedAnswerGO.GetComponent<Text>().color = Color.black;
            }
            touchedAnswerGO = other.transform.parent.GetChild(0).gameObject;
            touchedAnswerGO.GetComponent<Text>().color = Color.green;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (smms.dataset == 1)
        {
            if ((transform.parent.parent.parent.parent.parent.name == "Controller (right)" && smms.trialState == TrialState.OnTask) || (smms.trialState == TrialState.PreTask && smms.interactionTrainingCount == 6))
            {
                ColorFilterCollisionStay(other);
            }
        }

        if (other.name.Contains("Filter"))
        {
            if (draggedFilter == null)
            {
                draggedFilter = other.gameObject;
            }
            else
            {
                if (other.gameObject != draggedFilter)
                {
                    canGrab = false;
                    draggedFilter.GetComponent<Renderer>().material.color = new Color(147f / 255f, 1, 1);
                    draggedFilter = null;

                }
            }
        }

        if (transform.parent.parent.parent.parent.parent.name == "Controller (right)")
        {
            if (other.transform.parent.name.Contains("PanelItem"))
            {
                SteamVR_TrackedController rtc = transform.parent.parent.parent.parent.parent.GetComponent<SteamVR_TrackedController>();
                if (rtc.triggerPressed)
                {
                    SteamVR_Controller.Input((int)rtc.controllerIndex).TriggerHapticPulse(500);
                    if ((smms.trialState == TrialState.Answer) || (smms.trialState == TrialState.PreTask && smms.interactionTrainingCount == 0)) {
                        if (other.transform.parent.name.Equals("PanelItemConfirm"))
                        {
                            smms.confirmButtonPressed = true;
                            smms.ValidateAnswer();
                        }
                        else
                        {
                            Transform textItem = other.transform.parent.GetChild(0);
                            smms.selectedAnswer = textItem.GetComponent<Text>().text;
                            answerSelected = true;

                            if (selectedAnswerGO != null)
                            {
                                if (selectedAnswerGO != other.transform.parent.GetChild(0).gameObject)
                                {
                                    selectedAnswerGO.GetComponent<Text>().color = Color.black;
                                    selectedAnswerGO = other.transform.parent.GetChild(0).gameObject;
                                }
                            }
                            else {
                                selectedAnswerGO = other.transform.parent.GetChild(0).gameObject;
                            }
                        }
                    }
                }
                //else {
                //    SteamVR_Controller.Input((int)rtc.controllerIndex).TriggerHapticPulse(500);
                //}
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (smms.dataset == 1)
        {
            if (other.transform.parent.name.Equals("Sensor")) {
                if (transform.parent.parent.parent.parent.parent.name == "Controller (left)")
                {
                    if (keepLeftHighlightGO != null)
                    {
                        smms.RegisterCollidedSensorFromLeft(null);
                    }

                }
                else
                {
                    if (keepRightHighlightGO != null)
                    {
                        smms.RegisterCollidedSensorFromRight(null);
                    }
                }
            }
            
            if ((transform.parent.parent.parent.parent.parent.name == "Controller (right)" && smms.trialState == TrialState.OnTask) || (smms.trialState == TrialState.PreTask && smms.interactionTrainingCount == 6))
            {
                if (other.transform.parent.name.Contains("PanelItem"))
                {
                    if (smms.colorFilterActive)
                    {
                        smms.ColorFilterExitCollision();
                        touchedIndexOfColorFilter = -1;
                    }
                }
            }
        }

        if (other.name == "Box")
        {
            if (touchedAxis != null) {
                touchedAxis.GetComponent<Renderer>().material.color = Color.white;
                

                //Debug.Log(BIMlvlIndex);
                if (smms.dataset == 1)
                {
                    if (BIMlvlIndex == 0)
                    {
                        smms.FilterBIMFromCollision("Y", "max", 2);
                        smms.FilterBIMFromCollision("Y", "min", 0);
                    }
                    else if (BIMlvlIndex == 1)
                    {
                        smms.FilterBIMFromCollision("Y", "max", 1);
                        smms.FilterBIMFromCollision("Y", "min", 0);
                    }
                    else if (BIMlvlIndex == 2)
                    {
                        smms.FilterBIMFromCollision("Y", "max", 2);
                        smms.FilterBIMFromCollision("Y", "min", 1);
                    }
                }
                else {
                    if (other.transform.parent.parent.name.Contains("Value")) {
                        BarYAxisIndex = new Vector2(0, 1);
                        if (touchedAxis != null) {
                            CalculateFinalBarYAxisIndex();
                            selectedBarYAxisIndex = new Vector2(0, 1);
                        }
                    }
                }
                touchedAxis = null;
            }
        }

        if ((transform.parent.parent.parent.parent.parent.name == "Controller (right)" && smms.trialState == TrialState.Answer) || (smms.trialState == TrialState.PreTask && smms.interactionTrainingCount == 0))
        {
            if (other.transform.parent.name.Contains("PanelItem"))
            {
                if (answerSelected)
                {
                    if (touchedAnswerGO == selectedAnswerGO)
                    {
                        answerTouched = false;
                        touchedAnswerGO = null;
                    }
                    else
                    {
                        if (touchedAnswerGO != null && touchedAnswerGO == other.transform.parent.GetChild(0).gameObject)
                        {
                            answerTouched = false;
                            touchedAnswerGO.GetComponent<Text>().color = Color.black;
                            selectedAnswerGO.GetComponent<Text>().color = Color.green;
                            touchedAnswerGO = null;
                        }
                    }
                }
                else {
                    if (touchedAnswerGO != null && touchedAnswerGO == other.transform.parent.GetChild(0).gameObject)
                    {
                        answerTouched = false;
                        touchedAnswerGO.GetComponent<Text>().color = Color.black;
                        touchedAnswerGO = null;
                    }
                }
            }
        }

        //if (other.name == "Button")
        //{
        //    other.transform.localPosition = Vector3.Lerp(other.transform.localPosition, new Vector3(0, 0.46f, 0), Time.deltaTime * 5);
        //}

        //if (transform.parent.parent.parent.parent.parent.name == "Controller (right)" && smms.trialState == TrialState.Answer)
        //{
        //    if (other.transform.parent.name.Contains("PanelItem"))
        //    {
        //        smms.selectedAnswer = "";
        //    }
        //}
    }



    /// <summary>
    /// Dataset 1
    /// </summary>
    /// <returns></returns>
    private void SensorCollisionEnter(Collider other) {
        obj = other;
        if (other.name.Contains("ACG"))
        {
            if (int.Parse(other.name.Substring(4)) <= 4 && int.Parse(other.name.Substring(4)) >= 1)
            {
                if (transform.parent.parent.parent.parent.parent.name == "Controller (left)")
                {
                    currentLeftHighlight = other.gameObject;
                    leftGroundSensor = 1;
                }
                else
                {
                    currentRightHighlight = other.gameObject;
                    rightGroundSensor = 1;
                }
            }
        }
        else if (other.name.Contains("AC1"))
        {
            if ((int.Parse(other.name.Substring(4)) <= 9 && int.Parse(other.name.Substring(4)) >= 3) || int.Parse(other.name.Substring(4)) == 12)
            {
                if (transform.parent.parent.parent.parent.parent.name == "Controller (left)")
                {
                    currentLeftHighlight = other.gameObject;
                    leftGroundSensor = 2;
                }
                else
                {
                    currentRightHighlight = other.gameObject;
                    rightGroundSensor = 2;
                }
            }
        }
        else
        {
            if (transform.parent.parent.parent.parent.parent.name == "Controller (left)")
            {
                leftGroundSensor = 0;
            }
            else
            {
                rightGroundSensor = 0;
            }
        }

        if (transform.parent.parent.parent.parent.parent.name == "Controller (left)")
        {
            smms.RegisterCollidedSensorFromLeft(FindLeftHighlight());
            //if (currentLeftHighlight != null) {
            //    smms.RegisterCollidedSensorFromLeft(currentLeftHighlight);
            //}
            //else
            //{
            //    smms.RegisterCollidedSensorFromLeft(null);
            //}
        }
        

        if (transform.parent.parent.parent.parent.parent.name == "Controller (right)")
        {
            smms.RegisterCollidedSensorFromRight(FindRightHighlight());
            //if (currentRightHighlight != null) { 
            //    smms.RegisterCollidedSensorFromRight(currentRightHighlight);
            //}
            //else
            //{
            //    smms.RegisterCollidedSensorFromRight(null);
            //}  
        }  
    }


    private GameObject FindLeftHighlight()
    {
        if (currentLeftHighlight != null)
        {
            //foreach (GameObject go in sm)
            //{
            GameObject sensorObj = null;
            //BuildingScript bs = sm[0].transform.GetChild(0).GetComponent<BuildingScript>();
            //bs.ResetHighlightSensor();
            if (leftGroundSensor == 1)
            {
                sensorObj = sm[0].transform.GetChild(0).Find("GroundFloor").Find("Sensor").Find(currentLeftHighlight.name).gameObject;
            }
            else if (leftGroundSensor == 2)
            {
                sensorObj = sm[0].transform.GetChild(0).Find("Floor1").Find("Sensor").Find(currentLeftHighlight.name).gameObject;
            }

            if (sensorObj != null)
            {                    
                keepLeftHighlightGO = currentLeftHighlight;
            }
            else {
                keepLeftHighlightGO = null;
            }
            //}
            return keepLeftHighlightGO;
        }
        else
        {
            return null;
        }
    }

    private GameObject FindRightHighlight()
    {
        if (currentRightHighlight != null)
        {
            //foreach (GameObject go in sm)
            //{
                GameObject sensorObj = null;
                //BuildingScript bs = sm[0].transform.GetChild(0).GetComponent<BuildingScript>();
                //bs.ResetHighlightSensor();
                if (rightGroundSensor == 1)
                {
                    sensorObj = sm[0].transform.GetChild(0).Find("GroundFloor").Find("Sensor").Find(currentRightHighlight.name).gameObject;
                }
                else if (rightGroundSensor == 2)
                {
                    sensorObj = sm[0].transform.GetChild(0).Find("Floor1").Find("Sensor").Find(currentRightHighlight.name).gameObject;
                }

                if (sensorObj != null)
                {
                    keepRightHighlightGO = currentRightHighlight;
                }
                else
                {
                    keepRightHighlightGO = null;
                }
            //}
            return keepRightHighlightGO;
        }
        else
        {
            return null;
        }
    }

    
    private void FilterManipulationD1()
    {
        
        if (draggedFilter != null)
        {
            filterCoordinitor.transform.SetParent(draggedFilter.transform.parent);
            filterCoordinitor.transform.localRotation = Quaternion.identity;
            filterCoordinitor.transform.position = this.transform.position;
            filterCoordinitor.transform.localPosition = new Vector3(0, filterCoordinitor.transform.localPosition.y, 0);

            if (filterCoordinitor.transform.localPosition.y >= 1)
            {
                filterCoordinitor.transform.localPosition = new Vector3(0, 1, 0);
            }
            else if (filterCoordinitor.transform.localPosition.y <= 0)
            {
                filterCoordinitor.transform.localPosition = new Vector3(0, 0, 0);
            }

            for (float i = 0; i <= 1; i = i + 0.5f)
            {
                i = Mathf.Round(i * 10f) / 10f;

                if (Mathf.Abs(filterCoordinitor.transform.localPosition.y - i) < 0.1f)
                {
                    segment = (int)(Mathf.Round(i / 0.5f * 1f) / 1f); // 0, 1, 2 for y axis filter
                }
            }

            SteamVR_TrackedController tc = transform.parent.parent.parent.parent.parent.GetComponent<SteamVR_TrackedController>();

            if (tc.triggerPressed)
            {

                if (tc.triggerTimer == 1)
                {
                    canGrab = true;
                }

                if (draggedFilter.transform.parent.parent.name == "X axis" || draggedFilter.transform.parent.parent.name == "Z axis")
                {
                    if (draggedFilter.transform.GetSiblingIndex() == 2)
                    {
                        if (draggedFilter.transform.parent.GetChild(3).localPosition.y - filterCoordinitor.transform.localPosition.y < 0.05f)
                        {
                            canGrab = false;
                        }
                    }
                    else
                    {
                        if (filterCoordinitor.transform.localPosition.y - draggedFilter.transform.parent.GetChild(2).localPosition.y < 0.05f)
                        {
                            canGrab = false;
                        }
                    }
                }
                else
                {
                    if (draggedFilter.transform.GetSiblingIndex() == 2)
                    {
                        if (draggedFilter.transform.parent.GetChild(3).localPosition.y - (segment / 2f) < 0.08f)
                        {
                            canGrab = false;
                        }
                    }
                    else
                    {
                        if ((segment / 2f) - draggedFilter.transform.parent.GetChild(2).localPosition.y < 0.08f)
                        {
                            canGrab = false;
                        }
                    }
                }

                if (canGrab)
                {
                    if (transform.parent.parent.parent.parent.parent.name == "Controller (left)")
                    {
                        smms.leftFilterMoving = true;
                        //smms.leftPressedCount = 0;
                    }
                    else
                    {
                        smms.rightFilterMoving = true;
                        //smms.rightPressedCount = 0;
                    }
                    draggedFilter.GetComponent<Renderer>().material.color = Color.green;

                    if (draggedFilter.transform.parent.parent.name == "X axis")
                    {
                        draggedFilter.transform.localPosition = new Vector3(3, filterCoordinitor.transform.localPosition.y, 0);
                        if (draggedFilter.transform.GetSiblingIndex() == 2)
                        {
                            smms.FilterBIMFromCollision("X", "min", filterCoordinitor.transform.localPosition.y);
                        }
                        else
                        {
                            smms.FilterBIMFromCollision("X", "max", filterCoordinitor.transform.localPosition.y);

                        }
                    }
                    else if (draggedFilter.transform.parent.parent.name == "Z axis")
                    {
                        draggedFilter.transform.localPosition = new Vector3(3, filterCoordinitor.transform.localPosition.y, 0);
                        if (draggedFilter.transform.GetSiblingIndex() == 2)
                        {
                            smms.FilterBIMFromCollision("Z", "min", filterCoordinitor.transform.localPosition.y);
                        }
                        else
                        {
                            smms.FilterBIMFromCollision("Z", "max", filterCoordinitor.transform.localPosition.y);
                        }
                    }
                    else
                    {
                        BIMlvlIndex = 0;
                        if (transform.parent.parent.parent.parent.parent.name == "Controller (left)")
                        {
                            GameObject.Find("rightCollisionDetector").GetComponent<CollisionDetection>().BIMlvlIndex = BIMlvlIndex;
                        }
                        else
                        {
                            GameObject.Find("leftCollisionDetector").GetComponent<CollisionDetection>().BIMlvlIndex = BIMlvlIndex;
                        }
                    
                        draggedFilter.transform.localPosition = new Vector3(3, segment, 0);
                        if (draggedFilter.transform.GetSiblingIndex() == 2)
                        {
                            draggedFilter.transform.localPosition = new Vector3(3, segment / 2f, 0);
                            smms.FilterBIMFromCollision("Y", "min", segment);
                        }
                        else
                        {
                            draggedFilter.transform.localPosition = new Vector3(3, segment / 2f, 0);
                            smms.FilterBIMFromCollision("Y", "max", segment);
                        }
                    }
                }
            }
            else
            {
                if (draggedFilter != null)
                {
                    if (transform.parent.parent.parent.parent.parent.name == "Controller (left)")
                    {
                        smms.leftFilterMoving = false;
                    }
                    else
                    {
                        smms.rightFilterMoving = false;
                    }
                    canGrab = false;
                    draggedFilter.GetComponent<Renderer>().material.color = new Color(147f / 255f, 1, 1);
                    draggedFilter = null;
                }
            }
        }
    }

    private void AxisManipulationD1()
    {
        
        if (touchedAxis != null)
        {
            
            axisCoordinitor.transform.SetParent(touchedAxis.transform.parent);
            axisCoordinitor.transform.localRotation = Quaternion.identity;
            axisCoordinitor.transform.position = this.transform.position;
            axisCoordinitor.transform.localPosition = new Vector3(0, axisCoordinitor.transform.localPosition.y, 0);

            if (axisCoordinitor.transform.localPosition.y >= 1)
            {
                axisCoordinitor.transform.localPosition = new Vector3(0, 1, 0);
            }
            else if (axisCoordinitor.transform.localPosition.y <= 0)
            {
                axisCoordinitor.transform.localPosition = new Vector3(0, 0, 0);
            }


            //int tmp = (int)(axisCoordinitor.transform.localPosition.y / (1f / 18f)) + 1;
            //axisSegment = (int)(tmp / 2f) + 1;



            if (touchedAxis.transform.parent.parent.name == "X axis")
            {
                touchedAxis.GetComponent<Renderer>().material.color = Color.green;
                if (transform.parent.parent.parent.parent.parent.name == "Controller (left)")
                {
                    smms.leftFindHoveringV2FromCollisionBIM = new Vector2(axisCoordinitor.transform.localPosition.y, -1);
                    smms.leftFindHighlighedAxisFromCollision = true;
                    //smms.leftPressedCount = 0;
                }
                else
                {
                    smms.rightFindHoveringV2FromCollisionBIM = new Vector2(axisCoordinitor.transform.localPosition.y, -1);
                    smms.rightFindHighlighedAxisFromCollision = true;
                    //smms.rightPressedCount = 0;
                }

            }
            else if (touchedAxis.transform.parent.parent.name == "Z axis")
            {
                touchedAxis.GetComponent<Renderer>().material.color = Color.green;
                if (transform.parent.parent.parent.parent.parent.name == "Controller (left)")
                {
                    smms.leftFindHoveringV2FromCollisionBIM = new Vector2(-1, axisCoordinitor.transform.localPosition.y);
                    smms.leftFindHighlighedAxisFromCollision = true;
                    //smms.leftPressedCount = 0;
                }
                else
                {
                    smms.rightFindHoveringV2FromCollisionBIM = new Vector2(-1, axisCoordinitor.transform.localPosition.y);
                    smms.rightFindHighlighedAxisFromCollision = true;
                    //smms.rightPressedCount = 0;
                }
            }
            else if (touchedAxis.transform.parent.parent.name == "Y axis")
            {
                touchedAxis.GetComponent<Renderer>().material.color = Color.green;
                
                if (axisCoordinitor.transform.localPosition.y <= 0.5f) // ground lvl
                {
                    smms.FilterBIMFromCollision("Y", "max", 1);
                    smms.FilterBIMFromCollision("Y", "min", 0);

                    if (transform.parent.parent.parent.parent.parent.GetComponent<SteamVR_TrackedController>().triggerPressed)
                        BIMlvlIndex = 1;                
                }
                else { // first floor 
                    smms.FilterBIMFromCollision("Y", "max", 2);
                    smms.FilterBIMFromCollision("Y", "min", 1);

                    if (transform.parent.parent.parent.parent.parent.GetComponent<SteamVR_TrackedController>().triggerPressed)
                        BIMlvlIndex = 2;
                }

                if (transform.parent.parent.parent.parent.parent.name == "Controller (left)")
                {
                    //smms.leftPressedCount = 0;
                    
                    GameObject.Find("rightCollisionDetector").GetComponent<CollisionDetection>().BIMlvlIndex = BIMlvlIndex;
                }
                else
                {
                    //smms.rightPressedCount = 0;

                    GameObject.Find("leftCollisionDetector").GetComponent<CollisionDetection>().BIMlvlIndex = BIMlvlIndex;
                }
            }
        }
        else
        {
            if (transform.parent.parent.parent.parent.parent.name == "Controller (left)")
            {
                smms.leftFindHoveringV2FromCollisionBIM = new Vector2(-1, -1);
                smms.leftFindHighlighedAxisFromCollision = false;
            }
            else
            {
                smms.rightFindHoveringV2FromCollisionBIM = new Vector2(-1, -1);
                smms.rightFindHighlighedAxisFromCollision = false;
            }
        }
    }


    /// <summary>
    /// DataSet 2
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="transparency"></param>
    /// 
    private void FilterManipulation()
    {
        if (draggedFilter != null)
        {
            filterCoordinitor.transform.SetParent(draggedFilter.transform.parent);
            filterCoordinitor.transform.localRotation = Quaternion.identity;
            filterCoordinitor.transform.position = this.transform.position;
            filterCoordinitor.transform.localPosition = new Vector3(0, filterCoordinitor.transform.localPosition.y, 0);

            if (filterCoordinitor.transform.localPosition.y >= 1)
            {
                filterCoordinitor.transform.localPosition = new Vector3(0, 1, 0);
            }
            else if (filterCoordinitor.transform.localPosition.y <= 0)
            {
                filterCoordinitor.transform.localPosition = new Vector3(0, 0, 0);
            }

            for (float i = 0; i <= 1; i = i + 0.1f)
            {
                i = Mathf.Round(i * 10f) / 10f;

                if (Mathf.Abs(filterCoordinitor.transform.localPosition.y - i) < 0.02f)
                {
                    segment = (int)(Mathf.Round(i / 0.1f * 1f) / 1f);
                }
            }

            SteamVR_TrackedController tc = transform.parent.parent.parent.parent.parent.GetComponent<SteamVR_TrackedController>();

            if (tc.triggerPressed)
            {
                
                if (tc.triggerTimer == 1)
                {
                    canGrab = true;
                }

                if (draggedFilter.transform.parent.parent.name.Contains("Value"))
                {
                    if (draggedFilter.transform.GetSiblingIndex() == 2)
                    {
                        if (draggedFilter.transform.parent.GetChild(3).localPosition.y - filterCoordinitor.transform.localPosition.y < 0.05f)
                        {
                            canGrab = false;
                        }
                    }
                    else
                    {
                        if (filterCoordinitor.transform.localPosition.y - draggedFilter.transform.parent.GetChild(2).localPosition.y < 0.05f)
                        {
                            canGrab = false;
                        }
                    }
                }
                else
                {
                    if (draggedFilter.transform.GetSiblingIndex() == 2)
                    {
                        if (draggedFilter.transform.parent.GetChild(3).localPosition.y - (segment / 10f) < 0.08f)
                        {
                            canGrab = false;
                        }
                    }
                    else
                    {
                        if ((segment / 10f) - draggedFilter.transform.parent.GetChild(2).localPosition.y < 0.08f)
                        {
                            canGrab = false;
                        }
                    }
                }

                if (canGrab)
                {
                    
                    if (transform.parent.parent.parent.parent.parent.name == "Controller (left)")
                    {
                        smms.leftFilterMoving = true;
                        //smms.leftPressedCount = 0;
                    }
                    else
                    {
                        smms.rightFilterMoving = true;
                        //smms.rightPressedCount = 0;
                    }
                    draggedFilter.GetComponent<Renderer>().material.color = Color.green;
                    
                    if (draggedFilter.transform.parent.parent.name.Contains("Country"))
                    {
                        draggedFilter.transform.localPosition = new Vector3(-3, segment / 10f, 0);
                        if (draggedFilter.transform.GetSiblingIndex() == 2)
                        {
                            smms.FilterBarChartFromCollision("Country", "left", segment);
                        }
                        else
                        {
                            smms.FilterBarChartFromCollision("Country", "right", segment);

                        }
                    }
                    else if (draggedFilter.transform.parent.parent.name.Contains("Year"))
                    {
                        draggedFilter.transform.localPosition = new Vector3(-3, segment / 10f, 0);
                        if (draggedFilter.transform.GetSiblingIndex() == 2)
                        {
                            smms.FilterBarChartFromCollision("Year", "left", segment);
                        }
                        else
                        {
                            smms.FilterBarChartFromCollision("Year", "right", segment);
                        }
                    }
                    else if(draggedFilter.transform.parent.parent.name.Contains("Value"))
                    {
                        if (touchedAxis == null) {
                            selectedBarYAxisIndex = new Vector2(0, 1);
                            if (transform.parent.parent.parent.parent.parent.name == "Controller (left)")
                            {
                                GameObject.Find("rightCollisionDetector").GetComponent<CollisionDetection>().selectedBarYAxisIndex = selectedBarYAxisIndex;
                            }
                            else
                            {
                                GameObject.Find("leftCollisionDetector").GetComponent<CollisionDetection>().selectedBarYAxisIndex = selectedBarYAxisIndex;
                            }
                        }
                        

                        if (draggedFilter.transform.childCount > 0)
                        {
                            draggedFilter.transform.localPosition = new Vector3(3, filterCoordinitor.transform.localPosition.y, 0);
                            if (draggedFilter.transform.GetSiblingIndex() == 2)
                            {
                                draggedFilter.transform.parent.parent.parent.GetChild(2).GetChild(1).Find("leftValueFilter").localPosition = new Vector3(-3, filterCoordinitor.transform.localPosition.y, 0);
                                smms.FilterBarChartFromCollision("Value", "left", filterCoordinitor.transform.localPosition.y);
                                //smms.FilterValueAxisFromCollision(draggedFilter, "Min", filterCoordinitor.transform.localPosition.y);

                                if (draggedFilter.transform.parent.parent.GetSiblingIndex() == 2)
                                {
                                    if (filterCoordinitor.transform.localPosition.y == 0)
                                    {
                                        GameObject leftValuePlane = draggedFilter.transform.parent.Find("leftValuePlane").gameObject;

                                        SetColorForFilter(leftValuePlane.transform.GetChild(0).gameObject, 0f);
                                        leftValuePlane.transform.localPosition = new Vector3(0, filterCoordinitor.transform.localPosition.y, 0);
                                    }
                                    else
                                    {
                                        GameObject leftValuePlane = draggedFilter.transform.parent.Find("leftValuePlane").gameObject;

                                        SetColorForFilter(leftValuePlane.transform.GetChild(0).gameObject, 0.3f);
                                        leftValuePlane.transform.localPosition = new Vector3(0, filterCoordinitor.transform.localPosition.y, 0);
                                    }
                                }
                            }
                            else {
                                draggedFilter.transform.parent.parent.parent.GetChild(2).GetChild(1).Find("rightValueFilter").localPosition = new Vector3(-3, filterCoordinitor.transform.localPosition.y, 0);
                                smms.FilterBarChartFromCollision("Value", "right", filterCoordinitor.transform.localPosition.y);
                                //smms.FilterValueAxisFromCollision(draggedFilter, "Max", filterCoordinitor.transform.localPosition.y);

                                if (draggedFilter.transform.parent.parent.GetSiblingIndex() == 2)
                                {
                                    if (filterCoordinitor.transform.localPosition.y == 1)
                                    {
                                        GameObject rightValuePlane = draggedFilter.transform.parent.Find("rightValuePlane").gameObject;

                                        SetColorForFilter(rightValuePlane.transform.GetChild(0).gameObject, 0f);
                                        rightValuePlane.transform.localPosition = new Vector3(0, filterCoordinitor.transform.localPosition.y, 0);
                                    }
                                    else
                                    {

                                        GameObject rightValuePlane = draggedFilter.transform.parent.Find("rightValuePlane").gameObject;

                                        SetColorForFilter(rightValuePlane.transform.GetChild(0).gameObject, 0.3f);
                                        rightValuePlane.transform.localPosition = new Vector3(0, filterCoordinitor.transform.localPosition.y, 0);
                                    }
                                }
                            }

                        }
                        else
                        {
                            draggedFilter.transform.localPosition = new Vector3(-3, filterCoordinitor.transform.localPosition.y, 0);
                            if (draggedFilter.transform.GetSiblingIndex() == 2)
                            {
                                draggedFilter.transform.parent.parent.parent.GetChild(5).GetChild(1).Find("leftValueFilter").localPosition = new Vector3(3, filterCoordinitor.transform.localPosition.y, 0);
                                smms.FilterBarChartFromCollision("Value", "left", filterCoordinitor.transform.localPosition.y);
                                //smms.FilterValueAxisFromCollision(draggedFilter, "Min", filterCoordinitor.transform.localPosition.y);

                                if (draggedFilter.transform.parent.parent.GetSiblingIndex() == 2)
                                {
                                    if (filterCoordinitor.transform.localPosition.y == 0)
                                    {
                                        GameObject leftValuePlane = draggedFilter.transform.parent.Find("leftValuePlane").gameObject;

                                        SetColorForFilter(leftValuePlane.transform.GetChild(0).gameObject, 0f);
                                        leftValuePlane.transform.localPosition = new Vector3(0, filterCoordinitor.transform.localPosition.y, 0);
                                    }
                                    else
                                    {
                                        GameObject leftValuePlane = draggedFilter.transform.parent.Find("leftValuePlane").gameObject;

                                        SetColorForFilter(leftValuePlane.transform.GetChild(0).gameObject, 0.3f);
                                        leftValuePlane.transform.localPosition = new Vector3(0, filterCoordinitor.transform.localPosition.y, 0);
                                    }
                                }
                            }
                            else
                            {
                                draggedFilter.transform.parent.parent.parent.GetChild(5).GetChild(1).Find("rightValueFilter").localPosition = new Vector3(3, filterCoordinitor.transform.localPosition.y, 0);
                                smms.FilterBarChartFromCollision("Value", "right", filterCoordinitor.transform.localPosition.y);
                                //smms.FilterValueAxisFromCollision(draggedFilter, "Max", filterCoordinitor.transform.localPosition.y);
                                if (draggedFilter.transform.parent.parent.GetSiblingIndex() == 2)
                                {
                                    if (filterCoordinitor.transform.localPosition.y == 1)
                                    {
                                        GameObject rightValuePlane = draggedFilter.transform.parent.Find("rightValuePlane").gameObject;

                                        SetColorForFilter(rightValuePlane.transform.GetChild(0).gameObject, 0f);
                                        rightValuePlane.transform.localPosition = new Vector3(0, filterCoordinitor.transform.localPosition.y, 0);
                                    }
                                    else
                                    {

                                        GameObject rightValuePlane = draggedFilter.transform.parent.Find("rightValuePlane").gameObject;

                                        SetColorForFilter(rightValuePlane.transform.GetChild(0).gameObject, 0.3f);
                                        rightValuePlane.transform.localPosition = new Vector3(0, filterCoordinitor.transform.localPosition.y, 0);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (draggedFilter != null)
                {
                    if (transform.parent.parent.parent.parent.parent.name == "Controller (left)")
                    {
                        smms.leftFilterMoving = false;
                    }
                    else
                    {
                        smms.rightFilterMoving = false;
                    }
                    canGrab = false;
                    draggedFilter.GetComponent<Renderer>().material.color = new Color(147f / 255f, 1, 1);
                    draggedFilter = null;
                }
            }
        }
    }

    /// <summary>
    /// when touch axises
    /// </summary>
    private void AxisManipulation()
    {
        if (touchedAxis != null && !canGrab)
        {
            axisCoordinitor.transform.SetParent(touchedAxis.transform.parent);
            axisCoordinitor.transform.localRotation = Quaternion.identity;
            axisCoordinitor.transform.position = this.transform.position;
            axisCoordinitor.transform.localPosition = new Vector3(0, axisCoordinitor.transform.localPosition.y, 0);

            if (axisCoordinitor.transform.localPosition.y >= 1)
            {
                axisCoordinitor.transform.localPosition = new Vector3(0, 1, 0);
            }
            else if (axisCoordinitor.transform.localPosition.y <= 0)
            {
                axisCoordinitor.transform.localPosition = new Vector3(0, 0, 0);
            }


            int tmp = (int)(axisCoordinitor.transform.localPosition.y / (1f / 18f)) + 1;
            axisSegment = (int)(tmp / 2f) + 1;

            

            if (touchedAxis.transform.parent.parent.name.Contains("Country"))
            {
                touchedAxis.GetComponent<Renderer>().material.color = Color.green;
                if (transform.parent.parent.parent.parent.parent.name == "Controller (left)")
                {
                    smms.leftFindHoveringV2FromCollision = new Vector2(axisSegment, 0);
                    smms.leftFindHighlighedAxisFromCollision = true;
                    //smms.leftPressedCount = 0;
                }
                else
                {
                    smms.rightFindHoveringV2FromCollision = new Vector2(axisSegment, 0);
                    smms.rightFindHighlighedAxisFromCollision = true;
                    //smms.rightPressedCount = 0;
                }

            }
            else if (touchedAxis.transform.parent.parent.name.Contains("Year"))
            {
                touchedAxis.GetComponent<Renderer>().material.color = Color.green;
                if (transform.parent.parent.parent.parent.parent.name == "Controller (left)")
                {
                    smms.leftFindHoveringV2FromCollision = new Vector2(0, axisSegment);
                    smms.leftFindHighlighedAxisFromCollision = true;
                    //smms.leftPressedCount = 0;
                }
                else
                {
                    smms.rightFindHoveringV2FromCollision = new Vector2(0, axisSegment);
                    smms.rightFindHighlighedAxisFromCollision = true;
                    //smms.rightPressedCount = 0;
                }
            }
            else if (touchedAxis.transform.parent.parent.name.Contains("Value"))
            {
                touchedAxis.GetComponent<Renderer>().material.color = Color.green;

                float minValue = Mathf.Max(0, axisCoordinitor.transform.localPosition.y - 0.04f);
                float maxValue = Mathf.Min(1, axisCoordinitor.transform.localPosition.y + 0.04f);

                BarYAxisIndex = new Vector2(minValue, maxValue);

                CalculateFinalBarYAxisIndex();
            }
        }
        else {
            if (transform.parent.parent.parent.parent.parent.name == "Controller (left)")
            {
                smms.leftFindHoveringV2FromCollision = new Vector2(0, 0);
                smms.leftFindHighlighedAxisFromCollision = false;
            }
            else {
                smms.rightFindHoveringV2FromCollision = new Vector2(0, 0);
                smms.rightFindHighlighedAxisFromCollision = false;
            }   
        }
    }

    private void CalculateFinalBarYAxisIndex() {
        Vector2 otherControllerBarYAxisIndex = new Vector2(0, 1);
        Vector2 otherControllerSelectedBarYAxisIndex = new Vector2(0, 1);
        Vector2 finalBarYAxisIndex = new Vector2(0, 1);



        if (transform.parent.parent.parent.parent.parent.GetComponent<SteamVR_TrackedController>().triggerPressed)
        {
            selectedBarYAxisIndex = BarYAxisIndex;
        }

        if (transform.parent.parent.parent.parent.parent.name == "Controller (left)")
        {
            //smms.leftPressedCount = 0;
            smms.leftFindHighlighedAxisFromCollision = true;

            otherControllerBarYAxisIndex = GameObject.Find("rightCollisionDetector").GetComponent<CollisionDetection>().BarYAxisIndex;
            otherControllerSelectedBarYAxisIndex = GameObject.Find("rightCollisionDetector").GetComponent<CollisionDetection>().selectedBarYAxisIndex;
            //GameObject.Find("rightCollisionDetector").GetComponent<CollisionDetection>().BarYAxisIndex = BarYAxisIndex;
        }
        else
        {
            //smms.rightPressedCount = 0;
            smms.rightFindHighlighedAxisFromCollision = true;

            otherControllerBarYAxisIndex = GameObject.Find("leftCollisionDetector").GetComponent<CollisionDetection>().BarYAxisIndex;
            otherControllerSelectedBarYAxisIndex = GameObject.Find("leftCollisionDetector").GetComponent<CollisionDetection>().selectedBarYAxisIndex;
            //GameObject.Find("leftCollisionDetector").GetComponent<CollisionDetection>().BarYAxisIndex = BarYAxisIndex;
        }

        //Debug.Log(otherControllerSelectedBarYAxisIndex);

        if (otherControllerBarYAxisIndex != new Vector2(0, 1)) // both touch
        {
            finalBarYAxisIndex = new Vector2(Mathf.Min(BarYAxisIndex.x, otherControllerBarYAxisIndex.x), Mathf.Max(BarYAxisIndex.y, otherControllerBarYAxisIndex.y));
        }
        else
        {
            if (otherControllerSelectedBarYAxisIndex != new Vector2(0, 1)) 
            {
                if (BarYAxisIndex != new Vector2(0, 1)) // this touch + other selected
                {
                    finalBarYAxisIndex = new Vector2(Mathf.Min(BarYAxisIndex.x, otherControllerSelectedBarYAxisIndex.x), Mathf.Max(BarYAxisIndex.y, otherControllerSelectedBarYAxisIndex.y));
                }
                else 
                {
                    if (selectedBarYAxisIndex != new Vector2(0, 1)) // this selected + other selected
                    {
                        finalBarYAxisIndex = new Vector2(Mathf.Min(selectedBarYAxisIndex.x, otherControllerSelectedBarYAxisIndex.x), Mathf.Max(selectedBarYAxisIndex.y, otherControllerSelectedBarYAxisIndex.y));
                    }
                    else
                    { // other selected
                        finalBarYAxisIndex = otherControllerSelectedBarYAxisIndex;
                    }
                }
            }
            else {
                if (BarYAxisIndex != new Vector2(0, 1)) // this touch
                {
                    finalBarYAxisIndex = BarYAxisIndex;
                }
                else
                { // this selected
                    finalBarYAxisIndex = selectedBarYAxisIndex;
                }
            }
        }

        

        Transform barchart = touchedAxis.transform.parent.parent.parent;
        barchart.GetChild(2).GetChild(1).Find("leftValueFilter").localPosition = new Vector3(-3, finalBarYAxisIndex.x, 0);
        Transform leftValuePlane = barchart.GetChild(2).GetChild(1).Find("leftValuePlane");
        leftValuePlane.transform.localPosition = new Vector3(0, finalBarYAxisIndex.x, 0);

        barchart.GetChild(2).GetChild(1).Find("rightValueFilter").localPosition = new Vector3(-3, finalBarYAxisIndex.y, 0);
        Transform rightValuePlane = barchart.GetChild(2).GetChild(1).Find("rightValuePlane");
        rightValuePlane.transform.localPosition = new Vector3(0, finalBarYAxisIndex.y, 0);

        if (finalBarYAxisIndex.x == 0)
        {
            SetColorForFilter(leftValuePlane.GetChild(0).gameObject, 0f);
        }
        else
        {
            SetColorForFilter(leftValuePlane.transform.GetChild(0).gameObject, 0.3f);
        }

        if (finalBarYAxisIndex.y == 1)
        {
            SetColorForFilter(rightValuePlane.GetChild(0).gameObject, 0f);
        }
        else
        {
            SetColorForFilter(rightValuePlane.transform.GetChild(0).gameObject, 0.3f);
        }

        barchart.GetChild(5).GetChild(1).Find("leftValueFilter").localPosition = new Vector3(3, finalBarYAxisIndex.x, 0);
        barchart.GetChild(5).GetChild(1).Find("rightValueFilter").localPosition = new Vector3(3, finalBarYAxisIndex.y, 0);

        //Debug.Log(finalBarYAxisIndex);
        smms.FilterBarChartFromCollision("Value", "left", finalBarYAxisIndex.x);
        smms.FilterBarChartFromCollision("Value", "right", finalBarYAxisIndex.y);
        //if (selectedBarYAxisIndex != new Vector2(0, 1)) {
        //    smms.FilterBarChartFromCollision("Value", "left", finalBarYAxisIndex.x);
        //    smms.FilterBarChartFromCollision("Value", "right", finalBarYAxisIndex.y);
        //    selectedBarYAxisIndex = new Vector2(0, 1);
        //}
    }

    private void SetColorForFilter(GameObject filter, float transparency)
    {
        Color tmpColor = filter.GetComponent<Renderer>().material.color;
        tmpColor.a = transparency;
        filter.GetComponent<Renderer>().material.color = tmpColor;
    }

    private bool CheckDiff(float a, float b, float delta, bool abs)
    {
        if (abs)
        {
            if (Mathf.Abs(a - b) < delta)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (a - b < delta)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }

    private string GetCountry(int index)
    {
        switch (index)
        {
            case 0:
                return "Austria";
            case 1:
                return "France";
            case 2:
                return "Australia";
            case 3:
                return "Belgium";
            case 4:
                return "Canada";
            case 5:
                return "Netherlands";
            case 6:
                return "Italy";
            case 7:
                return "Denmark";
            case 8:
                return "United States";
            case 9:
                return "Norway";
            default:
                return "";
        }
    }

    private string GetYear(int index) {
        switch (index)
        {
            case 0:
                return "1989";
            case 1:
                return "1990";
            case 2:
                return "1991";
            case 3:
                return "1992";
            case 4:
                return "1993";
            case 5:
                return "1994";
            case 6:
                return "1995";
            case 7:
                return "1996";
            case 8:
                return "1997";
            case 9:
                return "1998";
            default:
                return "";
        }
    }
}

