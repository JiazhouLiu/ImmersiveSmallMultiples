using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class CollisionDetection : MonoBehaviour
{
    public GameObject FilterPrefab;
    GameObject TooltipPrefab;

    SmallMultiplesManagerScript smms;
    List<GameObject> sm;

    // dataset 2
    GameObject filterCoordinitor;
    GameObject axisCoordinitor;
    GameObject barValueTooltip;

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

    [HideInInspector]
    public GameObject currentCube = null;
    Transform currentCubeParent = null;

    GameObject cubeSelectionPrefab;
    public bool answerSelected = false;
    bool answerTouched = false;
    GameObject touchedAnswerGO = null;
    GameObject selectedAnswerGO = null;

    Vector2 BarYAxisIndex = new Vector2(0, 1);
    public Vector2 selectedBarYAxisIndex = new Vector2(0, 1);


    // Use this for initialization
    void Start()
    {
        
        smms = GameObject.Find("SmallMultiplesManager").GetComponent<SmallMultiplesManagerScript>();
        TooltipPrefab = smms.tooltipPrefab;
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

        filterCoordinitor = new GameObject();
        filterCoordinitor.name = "filterCoordinitor";

        axisCoordinitor = new GameObject();
        axisCoordinitor.name = "axisCoordinitor";

        barValueTooltip = (GameObject)Instantiate(TooltipPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        VRTK_ObjectTooltip ot = barValueTooltip.GetComponent<VRTK_ObjectTooltip>();
        barValueTooltip.transform.Find("TooltipCanvas").Find("UIContainer").gameObject.SetActive(false);
        barValueTooltip.SetActive(false);

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
        FilterManipulation();
        AxisManipulation();
    }

    private void OnTriggerEnter(Collider other)
    {
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
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Box")
        {
            if (touchedAxis != null) {
                touchedAxis.GetComponent<Renderer>().material.color = Color.white;
                
                
                if (other.transform.parent.parent.name.Contains("Budget")) {
                    BarYAxisIndex = new Vector2(0, 1);
                    if (touchedAxis != null) {
                        CalculateFinalBarYAxisIndex();
                        selectedBarYAxisIndex = new Vector2(0, 1);
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

            for (float i = 0; i <= 1; i = i + 0.2f)
            {
                i = Mathf.Round(i * 10f) / 10f;

                if (Mathf.Abs(filterCoordinitor.transform.localPosition.y - i) < 0.02f)
                {
                    segment = (int)(Mathf.Round(i / 0.2f * 1f) / 1f);
                }
            }

            SteamVR_TrackedController tc = transform.parent.parent.parent.parent.parent.GetComponent<SteamVR_TrackedController>();

            if (tc.triggerPressed)
            {
                if (tc.triggerTimer == 1)
                {
                    canGrab = true;
                }

                if (draggedFilter.transform.parent.parent.name.Contains("Budget"))
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
                        if (draggedFilter.transform.parent.GetChild(3).localPosition.y - (segment / 5f) < 0.08f)
                        {
                            canGrab = false;
                        }
                    }
                    else
                    {
                        if ((segment / 5f) - draggedFilter.transform.parent.GetChild(2).localPosition.y < 0.08f)
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
                        draggedFilter.transform.localPosition = new Vector3(-3, segment / 5f, 0);
                        if (draggedFilter.transform.GetSiblingIndex() == 2)
                        {
                            smms.FilterBarChartFromCollision("Country", "left", segment);
                        }
                        else
                        {
                            smms.FilterBarChartFromCollision("Country", "right", segment);

                        }
                    }
                    else if (draggedFilter.transform.parent.parent.name.Contains("Sector"))
                    {
                        draggedFilter.transform.localPosition = new Vector3(-3, segment / 5f, 0);
                        if (draggedFilter.transform.GetSiblingIndex() == 2)
                        {
                            smms.FilterBarChartFromCollision("Year", "left", segment);
                        }
                        else
                        {
                            smms.FilterBarChartFromCollision("Year", "right", segment);
                        }
                    }
                    else if(draggedFilter.transform.parent.parent.name.Contains("Budget"))
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
                                else {
                                    if (filterCoordinitor.transform.localPosition.y == 0)
                                    {
                                        GameObject leftValuePlane = draggedFilter.transform.parent.parent.parent.GetChild(2).Find("axis_mesh").Find("leftValuePlane").gameObject;

                                        SetColorForFilter(leftValuePlane.transform.GetChild(0).gameObject, 0f);
                                        leftValuePlane.transform.localPosition = new Vector3(0, filterCoordinitor.transform.localPosition.y, 0);
                                    }
                                    else
                                    {
                                        GameObject leftValuePlane = draggedFilter.transform.parent.parent.parent.GetChild(2).Find("axis_mesh").Find("leftValuePlane").gameObject;

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
                                else
                                {
                                    if (filterCoordinitor.transform.localPosition.y == 1)
                                    {
                                        GameObject rightValuePlane = draggedFilter.transform.parent.parent.parent.GetChild(2).Find("axis_mesh").Find("rightValuePlane").gameObject;

                                        SetColorForFilter(rightValuePlane.transform.GetChild(0).gameObject, 0f);
                                        rightValuePlane.transform.localPosition = new Vector3(0, filterCoordinitor.transform.localPosition.y, 0);
                                    }
                                    else
                                    {
                                        GameObject rightValuePlane = draggedFilter.transform.parent.parent.parent.GetChild(2).Find("axis_mesh").Find("rightValuePlane").gameObject;

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
                                else
                                {
                                    if (filterCoordinitor.transform.localPosition.y == 0)
                                    {
                                        GameObject leftValuePlane = draggedFilter.transform.parent.parent.parent.GetChild(2).Find("axis_mesh").Find("leftValuePlane").gameObject;

                                        SetColorForFilter(leftValuePlane.transform.GetChild(0).gameObject, 0f);
                                        leftValuePlane.transform.localPosition = new Vector3(0, filterCoordinitor.transform.localPosition.y, 0);
                                    }
                                    else
                                    {
                                        GameObject leftValuePlane = draggedFilter.transform.parent.parent.parent.GetChild(2).Find("axis_mesh").Find("leftValuePlane").gameObject;

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
                                else
                                {
                                    if (filterCoordinitor.transform.localPosition.y == 1)
                                    {
                                        GameObject rightValuePlane = draggedFilter.transform.parent.parent.parent.GetChild(2).Find("axis_mesh").Find("rightValuePlane").gameObject;

                                        SetColorForFilter(rightValuePlane.transform.GetChild(0).gameObject, 0f);
                                        rightValuePlane.transform.localPosition = new Vector3(0, filterCoordinitor.transform.localPosition.y, 0);
                                    }
                                    else
                                    {
                                        GameObject rightValuePlane = draggedFilter.transform.parent.parent.parent.GetChild(2).Find("axis_mesh").Find("rightValuePlane").gameObject;

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


            int tmp = (int)(axisCoordinitor.transform.localPosition.y / (1f / 9f)) + 1;
            axisSegment = (int)(tmp / 2f) + 1;
            if (axisCoordinitor.transform.localPosition.y == 0) {
                axisSegment = 1;
            }
            else if (axisCoordinitor.transform.localPosition.y % 0.2f == 0)
            {
                axisSegment = (int)(axisCoordinitor.transform.localPosition.y / 0.2f);
            }
            else {
                axisSegment = (int)(axisCoordinitor.transform.localPosition.y / 0.2f) + 1;
            }

            

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
            else if (touchedAxis.transform.parent.parent.name.Contains("Sector"))
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
            else if (touchedAxis.transform.parent.parent.name.Contains("Budget"))
            {
                touchedAxis.GetComponent<Renderer>().material.color = Color.green;

                float minValue = Mathf.Max(0, axisCoordinitor.transform.localPosition.y - 0.04f);
                float maxValue = Mathf.Min(1, axisCoordinitor.transform.localPosition.y + 0.04f);

                BarYAxisIndex = new Vector2(minValue, maxValue);

                // value tooltip
                barValueTooltip.SetActive(true);
                barValueTooltip.transform.SetParent(touchedAxis.transform.parent);
                if (touchedAxis.transform.parent.parent.GetSiblingIndex() == 2)
                {
                    barValueTooltip.transform.localPosition = new Vector3(-6, axisCoordinitor.transform.localPosition.y, 0);
                }
                else {
                    barValueTooltip.transform.localPosition = new Vector3(6, axisCoordinitor.transform.localPosition.y, 0);
                }
                
                barValueTooltip.transform.localEulerAngles = Vector3.zero;

                Text textFront = barValueTooltip.transform.Find("TooltipCanvas").Find("UITextFront").GetComponent<Text>();
                Text textReverse = barValueTooltip.transform.Find("TooltipCanvas").Find("UITextReverse").GetComponent<Text>();

                textFront.text =  string.Format("{0:0.##\\%}", axisCoordinitor.transform.localPosition.y * 100);
                textReverse.text = string.Format("{0:0.##\\%}", axisCoordinitor.transform.localPosition.y * 100);
                textFront.color = Color.red;
                textReverse.color = Color.red;

                CalculateFinalBarYAxisIndex();
            }
        }
        else {
            barValueTooltip.SetActive(false);
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

