using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;
using VRTK;
using IATK;
using TMPro;
using System.IO;
using System.Linq;

public enum TrialState {
    PreTask,
    OnTask,
    Answer
}
	
public class SmallMultiplesManagerScript : MonoBehaviour {

    // prefabs
    public GameObject DataPrefab;
	public GameObject frontPillarPrefab;
	public GameObject BpillarPrefab;
	public GameObject pillarIOPrefab;
	public GameObject shelfBoardPrefab;
    public GameObject shelfBoardPiecePrefab;
	public GameObject centroidPrefab;
	public GameObject curveRendererPrefab;
    public GameObject controlBallPrefab;
    public GameObject fakeControlBallPrefab;
    public GameObject tooltipPrefab;
    public Material silhouetteShader;
    public Light haloLightPrefab;
    public TextAsset ACFile;
    public TextAsset FlatQsFile;
    public TextAsset HalfCircleQsFile;
    public TextAsset FullCircleQsFile;
    public TextAsset ShuffledOrderFile;
    public GameObject TextHolderPrefab;
    public GameObject FilterPrefab;
    public GameObject CuttingPlanePrefab;
    public GameObject cubeSelectionPrefab;
    public GameObject worldInMiniturePrefab;
    public GameObject LRLabelPrefab;
    // controller
    public Transform globalLeftController;
    public Transform globalRightController;
    public GameObject CameraRig;

    [HideInInspector]
    public bool fixedPosition = false;
    public bool circleLayout = false;
    public bool fullCircle = false;

    public int dataset = 1;
    [Header("Experiment")]
    public int smallMultiplesNumber;
    

    // Small multiples game object list
	List<GameObject> dataSM;

    // shelf variables
    GameObject shelf;
    List<GameObject> taskBoards;
	GameObject colorScheme;

    int shelfRows = 3;
    int shelfItemPerRow = 0;

    // shelf variables
    float baseVPosition = 0.8f;
    float delta = 0.8f; // 0.65f
    float vDelta = 0.7f;
    float d2Scale = 0.2f;
    // end shelf variables

    // string variables
    private char lineSeperater = '\n'; // It defines line seperate character
    private char fieldSeperator = ','; // It defines field seperate chracter

    string[] tempTagList;

    // task related
    //public int taskID = 0; // training T1 - T12, experiment 1 - 18
    int taskID = 0; // 1 - 30
    string fullTaskID = "0";
    int taskNo = 3;
    int trainingTaskNo = 2;
    int stopTaskNo = 0;

    [HideInInspector]
    public string[] taskArray;
    StreamWriter writer;
    StreamWriter writerEye;
    StreamWriter writerHead;
    StreamWriter writerAnswer;

    [HideInInspector]
    public bool startTask = false;
    // end task related

    // log related
    bool clockRotation = false;
    bool antiClockRotation = false;
    bool scaleUp = false;
    bool scaleDown = false;
    // tracking trials
    // end log related

    // shelf auto-adjustable movement
    float userHeight = 1.3f;

    // brushing for barchart
    Dictionary<string, Dictionary<Vector2, Vector3>> chessBoardPoints;

    bool[] chessBoardBrushingBool; // single brushing and axis brushing
    bool[] hoveringChessBoardBrushingBool; // hovering effect
    bool[] filteredChessBoardBrushingBool; // y-axis filtering
    bool[] rangeSelectionChessBoardBrushingBool;
    bool[] hoveringRangeSelectionChessBoardBrushingBool;

    bool[] finalChessBoardBrushingBool; // final highlight
    string currentFindHightlighted = "";

    [HideInInspector]
    public bool leftFilterMoving = false;
    [HideInInspector]
    public bool leftFindHighlighedAxisFromCollision = false;
    [HideInInspector]
    public Vector2 leftFindHighlighedV2FromCollision = new Vector2(0, 0);
    [HideInInspector]
    public Vector2 leftFindHoveringV2FromCollision = new Vector2(0, 0);

    [HideInInspector]
    public float leftYAxisPosition = 0.0f;
    [HideInInspector]
    public bool leftFindHighlighedForY = false;

    bool leftHighlighed = false;
    bool leftAxisHighlighed = false;
    bool leftFindHighlighedFromChessBoard = false;
    Vector2 leftFindHighlighedV2FromChessBoard = new Vector2(0, 0);
    Vector2 leftFindHoveringV2FromChessBoard = new Vector2(0, 0);
    bool leftHighlighedForY = false;
    int currentCountryLeftFilterPosition = 0;
    int currentCountryRightFilterPosition;

    [HideInInspector]
    public bool rightFilterMoving = false;
    [HideInInspector]
    public bool rightFindHighlighedAxisFromCollision = false;
    [HideInInspector]
    public Vector2 rightFindHighlighedV2FromCollision = new Vector2(0, 0);
    [HideInInspector]
    public Vector2 rightFindHoveringV2FromCollision = new Vector2(0, 0);
    [HideInInspector]
    public float rightYAxisPosition = 0.0f;
    [HideInInspector]
    public bool rightFindHighlighedForY = false;

    bool rightHighlighed = false;
    bool rightAxisHighlighed = false;
    bool rightFindHighlighedFromChessBoard = false;
    Vector2 rightFindHighlighedV2FromChessBoard = new Vector2(0, 0);
    Vector2 rightFindHoveringV2FromChessBoard = new Vector2(0, 0);
    bool rightHighlighedForY = false;
    [HideInInspector]
    public bool triggerPressedForFilterMoving = false;
    int currentYearLeftFilterPosition = 0;
    int currentYearRightFilterPosition;

    List<GameObject> rightYearFilters;
    List<GameObject> leftYearFilters;
    List<GameObject> rightCountryFilters;
    List<GameObject> leftCountryFilters;
    List<GameObject> rightValueFilters;
    List<GameObject> leftValueFilters;
    List<GameObject> rightValuePlanes;
    List<GameObject> leftValuePlanes;

    //[HideInInspector]
    //public GameObject cubeSelectionCube = null;
    GameObject cubeSelectionCube = null;
    Transform touchBarMiddleSM = null;

    GameObject worldInMiniture = null;
    bool creatingCube = false;
    bool creatingWorldInMiniture = false;

    // Vector3 for rotation
    Vector3 oldV3FromLeftBtoRightB = Vector3.zero;
    float rotationDelta = 0;

    // Shelf movement
    bool controllerShelfDeltaSetup = false;
    Vector3 controllerShelfDelta = Vector3.zero;
    Vector3 oldEulerAngle = Vector3.zero;
    Vector3 oldWorldInMiniturePosition = Vector3.zero;
    Vector3 cameraForward = Vector3.zero;


    // Experiment related
    float completionTime = 120f;
    bool forceStopedTFCFromManager = false;
    [HideInInspector]
    public TrialState trialState = TrialState.PreTask;
    [HideInInspector]
    public string selectedAnswer = "";
    int trainingCounting = 0;
    [HideInInspector]
    public int trainingCountingLeft = 0;
    [HideInInspector]
    public bool interactionTrainingNeeded = false;
    [HideInInspector]
    public int interactionTrainingCount = 7;
    string[] interactionTrainingDesc;
    bool flyingFlag = false;
    Vector3[] taskBoardPositions = new Vector3[3];
    [HideInInspector]
    public bool confirmButtonPressed = false;
    bool answerConfirmed = false;
    float SMRotationDiff = 0;
    bool answerLogged = false;
    int sceneCounter = 0;
    string[] QuestionIDs;
    string[] CorrectAnswers;
    float recordPastTime = 0;


    // unclick to refresh
    bool leftClickEmptySpace = false;
    bool rightClickEmptySpace = false;

    // label issue
    List<GameObject> smToolTips;

    // highlight sm
    Vector2[] highlightedSM;
    Vector2 currentHighlightedSM = Vector2.zero;

    // eyetracking
    public bool calibrationFlag = false;
    GameObject mainCamera;
    public bool afterCalibration = true;
    Vector2 rawGazePositionOnScreen = Vector2.zero;

    // optimising
    List<string> oldRelatedSensors;
    bool[] oldHighlightedbars;

    int barNum = 25;
    Dictionary<int, int[]> shuffledOrders;

    // Use this for initialization
    void Start () {
        QualitySettings.vSyncCount = 0;
        userHeight = ExperimentManager.userHeight;

        interactionTrainingCount = 7;
        sceneCounter = 0;

        //Debug.Log(ExperimentManager.userHeight);
        //userHeight = 1.7f;
        //GameObject testingBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //testingBall.transform.position = new Vector3(0.5f, userHeight, 0.9f);
        //testingBall.transform.localScale = Vector3.one * 0.05f;

        if (dataSM == null) {
            transform.localScale = 0.6f * Vector3.one;
            transform.localPosition = new Vector3(transform.localPosition.x, 0.5f / 1.7f * userHeight, transform.localPosition.z);

            if (smallMultiplesNumber < 1)
			{
				Debug.Log("Please enter a valid small multiples number");
			}
			else
			{
                ResetManagerPosition();
                
                chessBoardPoints = new Dictionary<string, Dictionary<Vector2, Vector3>>();
                shuffledOrders = new Dictionary<int, int[]>();

                dataSM = new List<GameObject>();

                taskBoards = new List<GameObject>();

                chessBoardBrushingBool = new bool[barNum]; // single brushing and axis brushing
                hoveringChessBoardBrushingBool = new bool[barNum]; // hovering effect
                filteredChessBoardBrushingBool = new bool[barNum]; // y-axis filtering
                rangeSelectionChessBoardBrushingBool = new bool[barNum];
                hoveringRangeSelectionChessBoardBrushingBool = new bool[barNum];

                finalChessBoardBrushingBool = new bool[barNum]; // final highlight
                oldHighlightedbars = new bool[barNum]; 

                currentYearRightFilterPosition = (int) Mathf.Sqrt(barNum);
                currentCountryRightFilterPosition = (int)Mathf.Sqrt(barNum);

                rightYearFilters = new List<GameObject>();
                leftYearFilters = new List<GameObject>();
                rightCountryFilters = new List<GameObject>();
                leftCountryFilters = new List<GameObject>();
                rightValueFilters = new List<GameObject>();
                leftValueFilters = new List<GameObject>();
                rightValuePlanes = new List<GameObject>();
                leftValuePlanes = new List<GameObject>();

                smToolTips = new List<GameObject>();

                interactionTrainingDesc = new string[7] {"Task Training: To rotate the small multiples, press and hold both <color=red>trigger</color> buttons together and then move the controller probes around the Y axis in the space.\n\n" +
                    "Press <color=green>Next</color> button on the controller to the next tutorial",
                    "Task Training: To brush a single data point of a small multiple, move one controller probe into a data point in one small multiple. Then hover on it to see the highlight or press <color=red>trigger</color> button while the data point is hightlighted to keep the brushing result.\n\n" +
                    "Press <color=green>Next</color> button on the controller to the next tutorial",
                    "Task Training: To refresh(reset) the brushing and filtering result, click any <color=red>trigger</color> button to an empty space after brushing. Try to perform this action when the controller probe is not inside the small multiples\n\n" +
                    "Press <color=green>Next</color> button on the controller to the next tutorial",
                    "Task Training: To brush a range of small multiples, move the controller probes to the axis bars around the small multiples to see the hightlight effect. And press <color=red>trigger</color> button while the data points are hightlighted to keep the brushing result. Using both controllers can brush a wide area.\n\n" +
                    "Press <color=green>Next</color> button on the controller to the next tutorial",
                    "Task Training: You can move the controller probes to the small <color=cyan>cones</color> on the edge of the axes. You can use any <color=red>trigger</color> button to grab it and move it to filter the visualisation.\n\n" +
                    "Press <color=green>Next</color> button on the controller to the next tutorial",
                    "Task Training: You can move two controller probes into the small multiples and then hold both <color=red>trigger</color> buttons to draw a cube to select a range of data points.\n\n" +
                    "Press <color=green>Next</color> button on the controller to the next tutorial",
                    "Task Training: The left panel attached to your left controller is the answer panel. It will appear automatically after you finish the task. To select a specific answer, press the right <color=red>trigger</color> button when the right controller probe is near the answer and the answer text becomes green. To confirm the answer and move to next task, press right <color=red>trigger</color> button again on the <color=green>Confirm</color> block.\n\n" +
                    "Press <color=green>Done</color> button on the controller or click <color=green>Confirm</color> button on the panel to finish the tutorial"};

                tempTagList = new string[smallMultiplesNumber];

                shelf = new GameObject("Shelf");
				shelf.transform.SetParent(this.transform);
				shelf.transform.localPosition = Vector3.zero;
                shelf.transform.localScale = Vector3.one;
                shelf.transform.eulerAngles = new Vector3(0, 0, 0);


                GameObject barChartManager = GameObject.Find("BarChartManagement");
                barChartManager.SetActive(true);

                // initialise bools for chessBoard
                for (int i = 0; i < finalChessBoardBrushingBool.Length; i++)
                {
                    finalChessBoardBrushingBool[i] = true;
                }

                for (int i = 0; i < chessBoardBrushingBool.Length; i++)
                {
                    chessBoardBrushingBool[i] = true;
                }

                for (int i = 0; i < filteredChessBoardBrushingBool.Length; i++)
                {
                    filteredChessBoardBrushingBool[i] = true;
                }

                for (int i = 0; i < rangeSelectionChessBoardBrushingBool.Length; i++)
                {
                    rangeSelectionChessBoardBrushingBool[i] = true;
                }

                for (int i = 0; i < hoveringRangeSelectionChessBoardBrushingBool.Length; i++)
                {
                    hoveringRangeSelectionChessBoardBrushingBool[i] = true;
                }

                // add task boards to list
                taskBoards.Add(GameObject.Find("TaskBoardLeft"));
                taskBoardPositions[0] = GameObject.Find("TaskBoardLeft").transform.position;
                taskBoards.Add(GameObject.Find("TaskBoardRight"));
                taskBoardPositions[1] = GameObject.Find("TaskBoardRight").transform.position;
                taskBoards.Add(GameObject.Find("TaskBoardTop"));
                taskBoardPositions[2] = GameObject.Find("TaskBoardTop").transform.position;

                taskArray = new string[taskNo + trainingTaskNo];
                highlightedSM = new Vector2[taskNo + trainingTaskNo];
                QuestionIDs = new string[taskNo + trainingTaskNo];
                CorrectAnswers = new string[taskNo + trainingTaskNo];

                GetTasks();
                GetShuffledOrders();

                string path = ExperimentManager.writerFilePath;

                writer = new StreamWriter(path, true);

                string writerEyeFilePath = "Assets/ExperimentData/ExperimentLog/Participant " + ExperimentManager.ParticipantID + "/Participant_" + ExperimentManager.ParticipantID + "_EyeTrackingLog.csv";
                writerEye = new StreamWriter(writerEyeFilePath, true);

                string writerHeadFilePath = "Assets/ExperimentData/ExperimentLog/Participant " + ExperimentManager.ParticipantID + "/Participant_" + ExperimentManager.ParticipantID + "_HeadPositionLog.csv";
                writerHead = new StreamWriter(writerHeadFilePath, true);

                string writerAnswerFilePath = "Assets/ExperimentData/ExperimentLog/Participant " + ExperimentManager.ParticipantID + "/Participant_" + ExperimentManager.ParticipantID + "_Answers.csv";
                writerAnswer = new StreamWriter(writerAnswerFilePath, true);

                if (smallMultiplesNumber % shelfRows == 0)
                {
                    shelfItemPerRow = smallMultiplesNumber / shelfRows;
                }
                else
                {
                    shelfItemPerRow = (smallMultiplesNumber + shelfRows - smallMultiplesNumber % shelfRows) / shelfRows;
                }     

                stopTaskNo = (ExperimentManager.sceneCounter + 1) * 5;
                // get info from scene manager
                if (ExperimentManager.sceneCounter > 5)
                {
                    Debug.LogError("Scene changing issue");
                }
                else {
                    ExperimentManager.sceneCounter += 1;
                }

                if (ExperimentManager.PublicTrialNumber != 0)
                {
                    recordPastTime = ExperimentManager.lastTimePast;
                    if (ExperimentManager.PublicTrialNumber % 5 == 1 || ExperimentManager.PublicTrialNumber % 5 == 2)
                    {
                        fullTaskID = "Training";
                    }
                    else if (ExperimentManager.PublicTrialNumber % 5 == 0)
                    {
                        fullTaskID = ExperimentManager.PublicTrialNumber / 5 * 3 - 1 + "";
                    }
                    else
                    {
                        fullTaskID = (ExperimentManager.PublicTrialNumber / 5 * 3 + ExperimentManager.PublicTrialNumber % 5 - 2) - 1 + "";
                    }
                    taskID = ExperimentManager.PublicTrialNumber - 1;
                    if (ExperimentManager.PublicTrialNumber % 5 == 0) {
                        sceneCounter = 4;
                    }else
                        sceneCounter = ExperimentManager.PublicTrialNumber % 5 - 1;
                    ExperimentManager.PublicTrialNumber = 0;
                }
                else {
                    taskID = 5 * (ExperimentManager.sceneCounter - 1);
                }
                

                if (ExperimentManager.comprehensiveTraining) {
                    interactionTrainingNeeded = true;
                    ExperimentManager.comprehensiveTraining = false;
                }
                else
                    interactionTrainingNeeded = false;
                trainingCounting = trainingTaskNo;
                trainingCountingLeft = trainingCounting - sceneCounter;
                if (trainingCountingLeft < 0)
                    trainingCountingLeft = 0;

                // create small multiples
                CreateSM();

                foreach (GameObject tooltip in smToolTips)
                {
                    tooltip.transform.SetParent(shelf.transform);
                }
                //foreach (GameObject go in dataSM) {
                //    Transform t = go.transform;
                //    SetLayer(9, t.GetChild(0).GetChild(0).GetChild(0));
                //}
                SetupPreTaskEnvironment("none");
                
            }
        }
    }

    private void ResetManagerPosition()
    {
        if (circleLayout)
        {
            if (fullCircle)
            {
                this.transform.position = new Vector3(0, transform.position.y, 0f);
            }
            else
            {
                this.transform.position = new Vector3(0, transform.position.y, -0.6f);
            }

        }
        else
        {
            this.transform.position = new Vector3(0, transform.position.y, 0.8f);
        }
    }

    void Update(){
        if (smallMultiplesNumber >= 1)
        {
            GetGazeInfo();
            FixedPositionCondition();

            if (writer != null)
            {
                if (writer.BaseStream != null)
                {
                    WritingLog();
                }
            }

            DetectBarChartInteraction();

            CreateRangeBrushingBox();

            FlyingFunction();
        }

        if (Input.GetKeyUp(KeyCode.C)) {
            OpenPupilCamera();
        }
    }

    /// <summary>
    /// Experiment process : Pre-task
    /// </summary>
    public void SetupPreTaskEnvironment(string controller)
    {
        trialState = TrialState.PreTask;

        completionTime = 120f;

        // reset tooltip highlighting
        for (int i = 1; i <= smallMultiplesNumber; i++)
        {
            Transform containerGO = GameObject.Find("Tooltip " + i).transform.Find("TooltipCanvas").GetChild(0);
            Transform frontTextGO = GameObject.Find("Tooltip " + i).transform.Find("TooltipCanvas").GetChild(1);
            Transform backTextGO = GameObject.Find("Tooltip " + i).transform.Find("TooltipCanvas").GetChild(2);

            Color originColor = new Color(227f / 255f, 227f / 255f, 227f / 255f);

            containerGO.GetComponent<Image>().color = originColor;
            frontTextGO.GetComponent<Text>().color = Color.black;
            backTextGO.GetComponent<Text>().color = Color.black;
        }

        // reset brushing
        RefreshDataSet2();

        // reset answer section
        GameObject leftController = globalLeftController.gameObject;
        if (leftController != null)
        {
            leftController.transform.GetChild(4).gameObject.SetActive(false);
            GameObject countryAnswers = leftController.transform.GetChild(4).gameObject;
            Transform choicesParent = countryAnswers.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1);
            for (int i = 0; i < choicesParent.childCount; i++)
            {
                choicesParent.GetChild(i).GetChild(0).GetComponent<Text>().color = Color.black;
            }

            leftController.transform.GetChild(5).gameObject.SetActive(false);
            GameObject yearAnswers = leftController.transform.GetChild(5).gameObject;
            choicesParent = yearAnswers.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1);
            for (int i = 0; i < choicesParent.childCount; i++)
            {
                choicesParent.GetChild(i).GetChild(0).GetComponent<Text>().color = Color.black;
            }

            //leftController.transform.GetChild(6).gameObject.SetActive(false);
            //GameObject chartAnswers = leftController.transform.GetChild(6).gameObject;
            //choicesParent = chartAnswers.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1);
            //for (int i = 0; i < choicesParent.childCount; i++)
            //{
            //    choicesParent.GetChild(i).GetChild(0).GetComponent<Text>().color = Color.black;
            //}
        }

        selectedAnswer = "";
        answerLogged = false;

        if (GameObject.Find("rightCollisionDetector") != null)
        {
            CollisionDetection rcd = GameObject.Find("rightCollisionDetector").GetComponent<CollisionDetection>();
            rcd.answerSelected = false;
        }

        if (GameObject.Find("leftCollisionDetector") != null)
        {
            CollisionDetection lcd = GameObject.Find("leftCollisionDetector").GetComponent<CollisionDetection>();
            lcd.answerSelected = false;
        }

        
        // check interaction training
        if (interactionTrainingNeeded)
        {
            if (interactionTrainingCount > 0)
            {
                if (interactionTrainingCount == 1) // answer training
                {
                    if (globalLeftController != null)
                    {
                        globalLeftController.GetChild(4).gameObject.SetActive(true);
                    }
                }

                ChangeTaskText(interactionTrainingDesc[7 - interactionTrainingCount], -1);
                interactionTrainingCount--;

                if (interactionTrainingCount == 0)
                {
                    if (globalLeftController != null && globalRightController != null)
                    {
                        globalLeftController.Find("TrackPadLabel").GetComponent<TextMeshPro>().text = "Done";
                        globalRightController.Find("TrackPadLabel").GetComponent<TextMeshPro>().text = "Done";
                    }
                }
            }
            else
            {
                interactionTrainingNeeded = false;
                if (globalLeftController != null)
                {
                    globalLeftController.GetChild(4).gameObject.SetActive(false);
                }
                SetupPreTaskEnvironment("none");
            }
        }
        else
        {
            // increase task ID***
            taskID++;
            fullTaskID = TaskIDToFullTaskID(taskID);
            ShuffleSMOrder();
            if (globalLeftController != null && globalRightController != null)
            {
                globalLeftController.Find("TrackPadLabel").GetComponent<TextMeshPro>().text = "Next";
                globalRightController.Find("TrackPadLabel").GetComponent<TextMeshPro>().text = "Next";
            }
            GameObject.Find("EnvironmentForUserStudy").transform.GetChild(3).gameObject.SetActive(true);
            ChangeTaskText("Please stand on the floor marker.\n\n" +
                "Please press the <color=green>Next</color> button on your controller to move on.", -1);
        }
    }

    /// <summary>
    /// Experiment process : On-Task
    /// </summary>
    public void StartTask()
    {
        trialState = TrialState.OnTask;

        // change trackpad label
        if (globalLeftController != null && globalRightController != null)
        {
            globalLeftController.Find("TrackPadLabel").GetComponent<TextMeshPro>().text = "Finish";
            globalRightController.Find("TrackPadLabel").GetComponent<TextMeshPro>().text = "Finish";
        }
        GameObject.Find("EnvironmentForUserStudy").transform.GetChild(3).gameObject.SetActive(false);

        // start timer
        for (int i = 0; i < 3; i++)
        {
            Transform countDownTimer = GameObject.Find("TaskBoards").transform.GetChild(i).Find("CountdownTimer");
            CountDownTimer cdt = countDownTimer.GetComponent<CountDownTimer>();
            cdt.StartTimer();
        }

        // show task and visualisation
        ShowCurrentTask();
    }

    private void ShowCurrentTask()
    {
        ChangeTaskText(taskArray[sceneCounter], sceneCounter + 1);

        Vector2 needHighlightedSM = Vector2.zero;

        // get highlighted SM from saved array
        needHighlightedSM = highlightedSM[sceneCounter];
        if (needHighlightedSM != Vector2.zero)
        {
            if (GameObject.Find("Tooltip " + needHighlightedSM.x) != null)
            {
                Transform containerGO = GameObject.Find("Tooltip " + needHighlightedSM.x).transform.Find("TooltipCanvas").GetChild(0);
                Transform frontTextGO = GameObject.Find("Tooltip " + needHighlightedSM.x).transform.Find("TooltipCanvas").GetChild(1);
                Transform backTextGO = GameObject.Find("Tooltip " + needHighlightedSM.x).transform.Find("TooltipCanvas").GetChild(2);

                containerGO.GetComponent<Image>().color = Color.green;
                frontTextGO.GetComponent<Text>().color = Color.red;
                backTextGO.GetComponent<Text>().color = Color.red;
            }

            if (GameObject.Find("Tooltip " + needHighlightedSM.y) != null)
            {
                Transform containerGO = GameObject.Find("Tooltip " + needHighlightedSM.y).transform.Find("TooltipCanvas").GetChild(0);
                Transform frontTextGO = GameObject.Find("Tooltip " + needHighlightedSM.y).transform.Find("TooltipCanvas").GetChild(1);
                Transform backTextGO = GameObject.Find("Tooltip " + needHighlightedSM.y).transform.Find("TooltipCanvas").GetChild(2);

                containerGO.GetComponent<Image>().color = Color.green;
                frontTextGO.GetComponent<Text>().color = Color.red;
                backTextGO.GetComponent<Text>().color = Color.red;
            }
        }
        currentHighlightedSM = needHighlightedSM;
    }

    /// <summary>
    /// Experiment process : Answer
    /// </summary>
    public void FinishOrTimeUpToAnswer()
    {
        trialState = TrialState.Answer;

        // stop and record time
        for (int i = 0; i < 3; i++)
        {
            Transform countDownTimer = GameObject.Find("TaskBoards").transform.GetChild(i).Find("CountdownTimer");
            CountDownTimer cdt = countDownTimer.GetComponent<CountDownTimer>();
            completionTime = 120f - cdt.countTimer;
            cdt.ResetTimer();
        }
            
        ChangeTaskText("Please choose the answer from the options attached to your left controller. And press <color=red>trigger</color> button to confirm.", -1);

        if (transform.localPosition.y > -50f)
            transform.localPosition -= Vector3.up * 100;
        // change answer panel
        Transform leftController = globalLeftController;
        if (ExperimentManager.sceneCounter > 3) // trending questions
        {
            if (leftController != null)
            {
                GameObject yearAnswers = leftController.GetChild(5).gameObject;
                yearAnswers.SetActive(true);
                SetupAnswerPanelYearList(int.Parse(GetQuestionID(sceneCounter)));
            }
        }
        else
        {
            if (leftController != null)
            {
                GameObject countryAnswers = leftController.GetChild(4).gameObject;
                countryAnswers.SetActive(true);
            }
        }
    }

    // post answer state
    public void ValidateAnswer() {
        if (trialState == TrialState.Answer)
        {
            if (selectedAnswer != "")
            {
                if (!answerLogged) {
                    // write to file
                    writerAnswer.WriteLine(ExperimentManager.ParticipantID + "," + taskID.ToString() + "," + fullTaskID + "," + selectedAnswer + "," + 
                        completionTime + "," + GetCurrentDataset() + "," + GetCurrentLayout() + "," + GetCurrentTaskLevel() + "," + 
                        GetQuestionID(sceneCounter) + "," + GetCorrectAnswer(sceneCounter));
                    writerAnswer.Flush();
                    answerLogged = true;
                    //trainingCountingLeft--;
                }

                if (transform.localPosition.y < -50f)
                    transform.localPosition += Vector3.up * 100;

                if (trainingCountingLeft > 0)
                {
                    //Debug.Log(selectedAnswer + " " + GetCorrectAnswer(sceneCounter));
                    if (selectedAnswer.Trim() == GetCorrectAnswer(sceneCounter).Trim())
                    {
                        trainingCountingLeft--;
                        sceneCounter++;
                        SetupPreTaskEnvironment("none");
                    }
                    else {
                        ChangeTaskText("<color=red>Wrong. </color>Please do it again.\n" + taskArray[sceneCounter], sceneCounter);
                    }
                }
                else {
                    trainingCountingLeft--;
                    if (sceneCounter < 5) {
                        sceneCounter++;
                    }
                    
                    if (sceneCounter == 5)
                    {
                        switch (ExperimentManager.ExperimentSequence)
                        {
                            case 1:
                                if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Flat")
                                {
                                    EndWritingFile();
                                    SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                                }
                                else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Full Circle")
                                {
                                    EndWritingFile();
                                    SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                                }
                                else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Half Circle")
                                {
                                    if (ExperimentManager.sceneCounter > 3)
                                    {
                                        taskID = -1;
                                        fullTaskID = "All Finished";
                                        EndWritingFile();
                                        UnityEditor.EditorApplication.isPlaying = false;
                                    }
                                    else
                                    {
                                        EndWritingFile();
                                        SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                                    }
                                }
                                break;
                            case 2:
                                if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Flat")
                                {
                                    EndWritingFile();
                                    SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                                }
                                else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Half Circle")
                                {
                                    EndWritingFile();
                                    SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                                }
                                else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Full Circle")
                                {
                                    if (ExperimentManager.sceneCounter > 3)
                                    {
                                        taskID = -1;
                                        fullTaskID = "All Finished";
                                        EndWritingFile();
                                        UnityEditor.EditorApplication.isPlaying = false;
                                    }
                                    else
                                    {
                                        EndWritingFile();
                                        SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                                    }
                                }
                                break;
                            case 3:
                                if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Half Circle")
                                {
                                    EndWritingFile();
                                    SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                                }
                                else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Flat")
                                {
                                    EndWritingFile();
                                    SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                                }
                                else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Full Circle")
                                {
                                    if (ExperimentManager.sceneCounter > 3)
                                    {
                                        taskID = -1;
                                        fullTaskID = "All Finished";
                                        EndWritingFile();
                                        UnityEditor.EditorApplication.isPlaying = false;
                                    }
                                    else
                                    {
                                        EndWritingFile();
                                        SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                                    }
                                }
                                break;
                            case 4:
                                if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Half Circle")
                                {
                                    EndWritingFile();
                                    SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                                }
                                else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Full Circle")
                                {
                                    EndWritingFile();
                                    SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                                }
                                else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Flat")
                                {
                                    if (ExperimentManager.sceneCounter > 3)
                                    {
                                        taskID = -1;
                                        fullTaskID = "All Finished";
                                        EndWritingFile();
                                        UnityEditor.EditorApplication.isPlaying = false;
                                    }
                                    else
                                    {
                                        EndWritingFile();
                                        SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                                    }
                                }
                                break;
                            case 5:
                                if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Full Circle")
                                {
                                    EndWritingFile();
                                    SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                                }
                                else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Flat")
                                {
                                    EndWritingFile();
                                    SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                                }
                                else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Half Circle")
                                {
                                    if (ExperimentManager.sceneCounter > 3)
                                    {
                                        taskID = -1;
                                        fullTaskID = "All Finished";
                                        EndWritingFile();
                                        UnityEditor.EditorApplication.isPlaying = false;
                                    }
                                    else
                                    {
                                        EndWritingFile();
                                        SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                                    }
                                }
                                break;
                            case 6:
                                if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Full Circle")
                                {
                                    EndWritingFile();
                                    SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                                }
                                else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Half Circle")
                                {
                                    EndWritingFile();
                                    SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                                }
                                else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Flat")
                                {
                                    if (ExperimentManager.sceneCounter > 3)
                                    {
                                        taskID = -1;
                                        fullTaskID = "All Finished";
                                        EndWritingFile();
                                        UnityEditor.EditorApplication.isPlaying = false;
                                    }
                                    else
                                    {
                                        EndWritingFile();
                                        SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        SetupPreTaskEnvironment("none");
                    }
                } 
            }
        }
        else {
            interactionTrainingNeeded = false;
            if (globalLeftController != null)
            {
                globalLeftController.GetChild(5).gameObject.SetActive(false);
            }
            SetupPreTaskEnvironment("none");
        }
    }

    // show question after timeout
    public void ShowTaskForTraining() {
        if (trialState == TrialState.Answer && (taskID == ((ExperimentManager.sceneCounter - 1) * 5 + 1) || taskID == ((ExperimentManager.sceneCounter - 1) * 5 + 2)))
        {
            ChangeTaskText(taskArray[sceneCounter], sceneCounter);
        }
    }

    public string GetTaskText() {

        return taskArray[sceneCounter];
    }

    // close functions
    void OnApplicationQuit()
    {
        EndWritingFile();
    }

    void EndWritingFile() {
        if (writer != null)
            if (writer.BaseStream != null)
                writer.Close();
        if (writerEye != null)
            if (writerEye.BaseStream != null)
                writerEye.Close();
        if (writerHead != null)
            if (writerHead.BaseStream != null)
                writerHead.Close();
        if (writerAnswer != null)
            if (writerAnswer.BaseStream != null)
                writerAnswer.Close();
    }

    // write log functions
    void WritingLog() {
        Transform leftController = globalLeftController;
        Transform rightController = globalRightController; 

        if (writer != null && Camera.main != null && leftController != null && rightController != null)
        {
            SteamVR_TrackedController leftControllerScript = leftController.GetComponent<SteamVR_TrackedController>();
            SteamVR_TrackedController rightControllerScript = rightController.GetComponent<SteamVR_TrackedController>();

            writer.WriteLine(GetFixedTime() + "," + ExperimentManager.userHeight + "," + taskID.ToString() + "," + ExperimentManager.ParticipantID + "," + ExperimentManager.ExperimentSequence + "," +
                GetCurrentDataset() + "," + GetCurrentLayout() + "," + fullTaskID + "," + GetCurrentTaskLevel() + "," + trialState + "," + VectorToString(Camera.main.transform.position) + "," + VectorToString(Camera.main.transform.eulerAngles) + "," +
                VectorToString(leftController.position) + "," + VectorToString(leftController.eulerAngles) + "," + VectorToString(rightController.position) + "," +
                VectorToString(rightController.eulerAngles) + "," + leftControllerScript.menuPressed + "," + leftControllerScript.triggerPressed + "," + leftControllerScript.gripped + "," +
                leftControllerScript.padPressed + "," + rightControllerScript.menuPressed + "," + rightControllerScript.triggerPressed + "," + rightControllerScript.gripped + "," +
                rightControllerScript.padPressed + "," + clockRotation + "," + antiClockRotation + "," + scaleUp + "," + scaleDown + "," + VectorToString(transform.position) + "," + transform.localScale.x + "," + SMRotationDiff + "," +
                GetSMFilterPositions() + "," + rawGazePositionOnScreen.x + "," + rawGazePositionOnScreen.y + "," + GetGazedSM() + "," + GetGazedWorldPosition());
            writer.Flush();
        }
        else {
            //Debug.Log("Camera or Controller not attached!");
        }

        if (writerEye != null)
        {
            writerEye.WriteLine(GetFixedTime() + "," + ExperimentManager.ParticipantID + "," + taskID.ToString() + "," + fullTaskID + "," + GetCurrentDataset() + "," + GetCurrentLayout() + "," +
            GetCurrentTaskLevel() + "," + trialState + "," + rawGazePositionOnScreen.x + "," + rawGazePositionOnScreen.y + "," + ("Small Multiples " + currentHighlightedSM.x) + "," +
            ("Small Multiples " + currentHighlightedSM.y) + "," + GetGazedSM() + "," + GetGazedWorldPosition());
            writerEye.Flush();
        }

        if (writerHead != null && Camera.main != null)
        {
            writerHead.WriteLine(GetFixedTime() + "," + ExperimentManager.ParticipantID + "," + taskID.ToString() + "," + fullTaskID + "," + GetCurrentDataset() + "," + GetCurrentLayout() + "," +
                GetCurrentTaskLevel() + "," + trialState + "," + VectorToString(transform.position) + "," + transform.localScale.x + "," + VectorToString(Camera.main.transform.position) + "," + VectorToString(Camera.main.transform.eulerAngles));
            writerHead.Flush();
        }
    }

   

    private string GetSMFilterPositions() {
        return leftCountryFilters[0].transform.localPosition.y + "," + rightCountryFilters[0].transform.localPosition.y + "," + 
            leftValueFilters[0].transform.localPosition.y + "," + rightValueFilters[0].transform.localPosition.y + "," + 
            leftYearFilters[0].transform.localPosition.y + "," + rightYearFilters[0].transform.localPosition.y;
    }

    // get gazed world Vector3 info
    private string GetGazedWorldPosition()
    {
        Vector3 finalPosition = Vector3.zero;

        if (PupilTools.IsConnected && PupilTools.IsGazing && afterCalibration)
        {
            Vector2 gazePointCenter = PupilData._2D.GazePosition;
            Vector3 viewportPoint = new Vector3(gazePointCenter.x, gazePointCenter.y, 1f);
            if (CameraRig != null && CameraRig.transform.GetChild(2) != null && CameraRig.transform.GetChild(2).GetComponent<Camera>() != null)
            {
                Ray ray = CameraRig.transform.GetChild(2).GetComponent<Camera>().ViewportPointToRay(viewportPoint);
                RaycastHit hit;

                // see this for ray stop point
                if (Physics.Raycast(ray, out hit))
                {
                    finalPosition = hit.point;
                }
                else
                {
                    finalPosition = ray.origin + ray.direction * 50f;
                }
            }
        }
        return finalPosition.x + "," + finalPosition.y + "," + finalPosition.z;
    }

    // get gazed SM info
    private string GetGazedSM()
    {
        string finalResult = "NA";
        if (PupilTools.IsConnected && PupilTools.IsGazing && afterCalibration)
        {
            //Debug.Log(rawGazePositionOnScreen);
            Vector2 gazePointCenter = PupilData._2D.GazePosition;
            rawGazePositionOnScreen = PupilData._2D.GazePosition;
            Vector3 viewportPoint = new Vector3(gazePointCenter.x, gazePointCenter.y, 1f);
            if (CameraRig != null && CameraRig.transform.GetChild(2) != null && CameraRig.transform.GetChild(2).GetComponent<Camera>() != null)
            {
                Ray ray = CameraRig.transform.GetChild(2).GetComponent<Camera>().ViewportPointToRay(viewportPoint);

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.name.Contains("Small Multiples"))
                    {
                        finalResult = hit.transform.name;
                        //if (GameObject.Find("TestingText") != null)
                        //    GameObject.Find("TestingText").GetComponent<Text>().text = "I'm looking at " + hit.transform.name;        
                    }
                    else
                    {
                        //if (GameObject.Find("TestingText") != null)
                        //    GameObject.Find("TestingText").GetComponent<Text>().text = "I'm looking at " + hit.transform.name;
                    }
                }
                else
                {
                    finalResult = "NA";
                    //if (GameObject.Find("TestingText") != null)
                    //    GameObject.Find("TestingText").GetComponent<Text>().text = "I'm looking at nothing";
                }
            }
        }
        else {
            //Debug.Log(PupilTools.IsConnected + " " +  PupilTools.IsGazing + " " + afterCalibration);
        }
        return finalResult;
    }

    public void OpenMainCamera()
    {
        CameraRig.transform.GetChild(2).gameObject.SetActive(true);
        PupilSettings.Instance.currentCamera = CameraRig.transform.GetChild(2).GetComponent<Camera>();

        afterCalibration = true;

        if (PupilTools.IsConnected)
        {
            PupilTools.SubscribeTo("gaze");
            PupilTools.IsGazing = true;
        }
    }

    public void OpenPupilCamera() {
        if (CameraRig != null && CameraRig.transform.GetChild(2) != null)
        {
            CameraRig.transform.GetChild(2).gameObject.SetActive(false);
            PupilSettings.Instance.currentCamera = GameObject.Find("Pupil Manager").transform.GetChild(2).GetComponent<Camera>();

            afterCalibration = false;
            calibrationFlag = true;
            //if (GameObject.Find("TestingText") != null)
            //    GameObject.Find("TestingText").GetComponent<Text>().text = "Calibrating!!!";
        }
    }

    // get Raw gaze Vector2 data
    private void GetGazeInfo() {
        if (PupilTools.IsConnected && PupilTools.IsGazing && afterCalibration)
        {
            //Debug.Log(rawGazePositionOnScreen);
            Vector2 gazePointCenter = PupilData._2D.GazePosition;
            rawGazePositionOnScreen = PupilData._2D.GazePosition;
        }
    }

    // get log info
    float GetFixedTime()
    {
        //float finalTime = 0;
        //if (ExperimentManager.PublicTrialNumber != 0)
        //{
        //    finalTime = pasttim + Time.fixedTime;
        //}
        //else
        //{
        //    finalTime = Time.fixedTime;
        //}

        return recordPastTime + Time.fixedTime;
    }

    string GetCurrentTaskLevel() {
        if (ExperimentManager.sceneCounter > 3)
            return "Trending";
        else
            return "Long Distance";
    }

    string GetCurrentLayout() {
        if (circleLayout)
        {
            if (fullCircle)
                return "Full-Circle";
            else
                return "Half-Circle";
        }
        else
            return "Flat";
    }

    string GetCurrentDataset() {
        return "Bar";
    }

    string VectorToString(Vector3 v) {
        string text;
        text = v.x + "," + v.y + "," + v.z;
        return text;
    }

    void FixedPositionCondition() {
        // change controller button color
        if (globalLeftController != null)
        {
            if (interactionTrainingNeeded && interactionTrainingCount != 0)
                if (globalLeftController.Find("TrackPadLabel").GetComponent<TextMeshPro>().text != "Next")
                    globalLeftController.Find("TrackPadLabel").GetComponent<TextMeshPro>().text = "Next";
        }
        if (globalRightController != null)
        {
            if (interactionTrainingNeeded && interactionTrainingCount != 0)
                if (globalRightController.Find("TrackPadLabel").GetComponent<TextMeshPro>().text != "Next")
                    globalRightController.Find("TrackPadLabel").GetComponent<TextMeshPro>().text = "Next";
        }

    }

    public void AssignTempTag( string[] tempTagList) {
        this.tempTagList = tempTagList;
    }

    /// <summary>
    /// draw flat, half circle and full circle
    /// </summary>
    private Vector3 AssignSMPositionBasedOnLayout(int index) {
        float xValue = 0;
        float yValue = 0;
        float zValue = 0;

        if (!circleLayout) // flat
        {
            zValue = 0;
            if (index < shelfItemPerRow) // top row
            {
                yValue = (shelfRows - 1) * vDelta;
                if (shelfItemPerRow % 2 == 0) // even number
                {
                    xValue = (index - (shelfItemPerRow / 2 - 0.5f)) * delta;
                }
                else { // odd number
                    // TODO
                }
            }
            else {
                if (index < shelfItemPerRow * 2) // second row
                {
                    yValue = (shelfRows - 2) * vDelta;
                    if (shelfItemPerRow % 2 == 0) // even number
                    {
                        xValue = (index - shelfItemPerRow - (shelfItemPerRow / 2 - 0.5f)) * delta;
                    }
                    else
                    { // odd number
                      // TODO
                    }
                }
                else {
                    if (index < shelfItemPerRow * 3) // third row
                    {
                        yValue = (shelfRows - 3) * vDelta;
                        if (shelfItemPerRow % 2 == 0) // even number
                        {
                            xValue = (index - (2 * shelfItemPerRow) - (shelfItemPerRow / 2 - 0.5f)) * delta;
                        }
                        else
                        { // odd number
                          // TODO
                        }
                    }
                    else // fourth row
                    {
                        // TODO
                    }
                }
            }
        }
        else {
            if (!fullCircle) // half circle
            {
                if (index < shelfItemPerRow) // top row
                {
                    yValue = (shelfRows - 1) * vDelta;
                    zValue = Mathf.Sin(index * Mathf.PI / (shelfItemPerRow - 1)) * ((shelfItemPerRow - 1) * delta / Mathf.PI);
                    xValue = -Mathf.Cos(index * Mathf.PI / (shelfItemPerRow - 1)) * ((shelfItemPerRow - 1) * delta / Mathf.PI);
                }
                else
                {
                    if (index < shelfItemPerRow * 2) // second row
                    {
                        yValue = (shelfRows - 2) * vDelta;
                        zValue = Mathf.Sin((index - shelfItemPerRow) * Mathf.PI / (shelfItemPerRow - 1)) * ((shelfItemPerRow - 1) * delta / Mathf.PI);
                        xValue = -Mathf.Cos((index - shelfItemPerRow) * Mathf.PI / (shelfItemPerRow - 1)) * ((shelfItemPerRow - 1) * delta / Mathf.PI);
                    }
                    else
                    {
                        if (index < shelfItemPerRow * 3) // third row
                        {
                            yValue = (shelfRows - 3) * vDelta;
                            zValue = Mathf.Sin((index - 2 * shelfItemPerRow) * Mathf.PI / (shelfItemPerRow - 1)) * ((shelfItemPerRow - 1) * delta / Mathf.PI);
                            xValue = -Mathf.Cos((index - 2 * shelfItemPerRow) * Mathf.PI / (shelfItemPerRow - 1)) * ((shelfItemPerRow - 1) * delta / Mathf.PI);
                        }
                        else // fourth row
                        {
                            // TODO
                        }
                    }
                }
            }
            else { // full circle
                if (index < shelfItemPerRow) // top row
                {
                    yValue = (shelfRows - 1) * vDelta;
                    //yValue = (shelfRows - 2) * vDelta + (shelfItemPerRow - index) * vDelta / shelfItemPerRow;
                    zValue = Mathf.Sin(index * Mathf.PI / (shelfItemPerRow / 2)) * ((shelfItemPerRow - 1) * delta / (2 * Mathf.PI));
                    xValue = -Mathf.Cos(index * Mathf.PI / (shelfItemPerRow / 2)) * ((shelfItemPerRow - 1) * delta / (2 * Mathf.PI));
                }
                else
                {
                    if (index < shelfItemPerRow * 2) // second row
                    {
                        yValue = (shelfRows - 2) * vDelta;
                        //yValue = (shelfRows - 3) * vDelta + (2 * shelfItemPerRow - index) * vDelta / shelfItemPerRow;
                        zValue = Mathf.Sin((index - shelfItemPerRow) * Mathf.PI / (shelfItemPerRow / 2)) * ((shelfItemPerRow - 1) * delta / (2 * Mathf.PI));
                        xValue = -Mathf.Cos((index - shelfItemPerRow) * Mathf.PI / (shelfItemPerRow / 2)) * ((shelfItemPerRow - 1) * delta / (2 * Mathf.PI));
                    }
                    else
                    {
                        if (index < shelfItemPerRow * 3) // third row
                        {
                            yValue = (shelfRows - 3) * vDelta;
                            //yValue = (shelfRows - 4) * vDelta + (3 * shelfItemPerRow - index) * vDelta / shelfItemPerRow;
                            zValue = Mathf.Sin((index - 2 * shelfItemPerRow) * Mathf.PI / (shelfItemPerRow / 2)) * ((shelfItemPerRow - 1) * delta / (2 * Mathf.PI));
                            xValue = -Mathf.Cos((index - 2 * shelfItemPerRow) * Mathf.PI / (shelfItemPerRow / 2)) * ((shelfItemPerRow - 1) * delta / (2 * Mathf.PI));
                        }
                        else // fourth row
                        {
                            // TODO
                        }
                    }
                }
            }
        }
        return new Vector3(xValue, yValue, zValue);
    }


    // create objects
    void CreateSM()
    {
        GameObject barChartManager = GameObject.Find("BarChartManagement");

        for (int i = 0; i < smallMultiplesNumber; i++)
        {
            GameObject dataObj = new GameObject();
            dataObj.name = "Small Multiples " + (i + 1);
            barChartManager.transform.GetChild(0).SetParent(dataObj.transform);
            dataObj.transform.GetChild(0).localPosition = new Vector3(-0.5f, 0.3f, -0.5f);
            dataObj.transform.localScale = new Vector3(d2Scale, d2Scale, d2Scale);
            dataObj.AddComponent<PositionLocalConstraints>();
            dataObj.AddComponent<BoxCollider>();
            dataObj.GetComponent<BoxCollider>().center = new Vector3(0,0.8f,0);
            dataObj.GetComponent<BoxCollider>().size = new Vector3(1.8f, 1.6f, 1.8f);

            //setup tooltip
            GameObject tooltip = (GameObject)Instantiate(tooltipPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            tooltip.gameObject.name = "Tooltip " + (i + 1);
            tooltip.transform.SetParent(dataObj.transform);
            tooltip.transform.localPosition = new Vector3(-1f, 0.8f, -0.5f);
            tooltip.transform.localEulerAngles= new Vector3(0, 0, 90);

            smToolTips.Add(tooltip);

            Transform tmpTrans = dataObj.transform.GetChild(0);
            tmpTrans.SetParent(shelf.transform);
            tmpTrans.SetParent(dataObj.transform);

            VRTK_ObjectTooltip tt = tooltip.GetComponent<VRTK_ObjectTooltip>();
            tt.containerSize = new Vector2(200, 60);
            tt.fontSize = 24;
            //tt.displayText = GetBarChartName(i + 1);
            tt.displayText = (i + 1980) + "";
            tt.alwaysFaceHeadset = false;


            dataObj.transform.SetParent(shelf.transform);
            dataObj.transform.localPosition = AssignSMPositionBasedOnLayout(i);
            dataObj.transform.localEulerAngles = new Vector3(0, dataObj.transform.localEulerAngles.y, 0);

            if (circleLayout)
            {
                GameObject center = new GameObject();
                center.transform.SetParent(shelf.transform);
                center.transform.localPosition = dataObj.transform.localPosition;
                center.transform.localPosition = new Vector3(0, center.transform.localPosition.y, 0);

                dataObj.transform.LookAt(center.transform.position);

                dataObj.transform.localEulerAngles += Vector3.up * 180;
                Destroy(center);
            }

            dataSM.Add(dataObj);

            GameObject leftCountryFilter = (GameObject)Instantiate(FilterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            leftCountryFilter.name = "leftCountryFilter";   
            leftCountryFilter.transform.SetParent(dataObj.transform.GetChild(1).GetChild(1).GetChild(1));
            SetColorForFilter(leftCountryFilter, 1);
            leftCountryFilter.transform.localPosition = new Vector3(-3, 0, 0);
            leftCountryFilter.transform.localScale = new Vector3(0.05f, 3, 1);
            leftCountryFilter.transform.localEulerAngles = new Vector3(0, 0, -90);
            leftCountryFilters.Add(leftCountryFilter);

            leftCountryFilter = (GameObject)Instantiate(FilterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            leftCountryFilter.name = "leftCountryFilter";
            leftCountryFilter.transform.SetParent(dataObj.transform.GetChild(1).GetChild(4).GetChild(1));
            SetColorForFilter(leftCountryFilter, 1);
            leftCountryFilter.transform.localPosition = new Vector3(-3, 0, 0);
            leftCountryFilter.transform.localScale = new Vector3(0.05f, 3, 1);
            leftCountryFilter.transform.localEulerAngles = new Vector3(0, 0, -90);
            leftCountryFilters.Add(leftCountryFilter);

            GameObject rightCountryFilter = (GameObject)Instantiate(FilterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            rightCountryFilter.name = "rightCountryFilter";
            rightCountryFilter.transform.SetParent(dataObj.transform.GetChild(1).GetChild(1).GetChild(1));               
            SetColorForFilter(rightCountryFilter, 1);
            rightCountryFilter.transform.localPosition = new Vector3(-3, 1, 0);
            rightCountryFilter.transform.localScale = new Vector3(0.05f, 3, 1);
            rightCountryFilter.transform.localEulerAngles = new Vector3(0, 0, -90);
            rightCountryFilters.Add(rightCountryFilter);

            rightCountryFilter = (GameObject)Instantiate(FilterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            rightCountryFilter.name = "rightCountryFilter";
            rightCountryFilter.transform.SetParent(dataObj.transform.GetChild(1).GetChild(4).GetChild(1));               
            SetColorForFilter(rightCountryFilter, 1);
            rightCountryFilter.transform.localPosition = new Vector3(-3, 1, 0);
            rightCountryFilter.transform.localScale = new Vector3(0.05f, 3, 1);
            rightCountryFilter.transform.localEulerAngles = new Vector3(0, 0, -90);
            rightCountryFilters.Add(rightCountryFilter);

            GameObject leftYearFilter = (GameObject)Instantiate(FilterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            leftYearFilter.name = "leftYearFilter";
            leftYearFilter.transform.SetParent(dataObj.transform.GetChild(1).GetChild(3).GetChild(1));               
            SetColorForFilter(leftYearFilter, 1);
            leftYearFilter.transform.localPosition = new Vector3(-3, 0, 0);
            leftYearFilter.transform.localScale = new Vector3(0.05f, 3, 1);
            leftYearFilter.transform.localEulerAngles = new Vector3(0, 0, -90);
            leftYearFilters.Add(leftYearFilter);

            leftYearFilter = (GameObject)Instantiate(FilterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            leftYearFilter.name = "leftYearFilter";
            leftYearFilter.transform.SetParent(dataObj.transform.GetChild(1).GetChild(6).GetChild(1));               
            SetColorForFilter(leftYearFilter, 1);
            leftYearFilter.transform.localPosition = new Vector3(-3, 0, 0);
            leftYearFilter.transform.localScale = new Vector3(0.05f, 3, 1);
            leftYearFilter.transform.localEulerAngles = new Vector3(0, 0, -90);
            leftYearFilters.Add(leftYearFilter);

            GameObject rightYearFilter = (GameObject)Instantiate(FilterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            rightYearFilter.name = "rightYearFilter";
            rightYearFilter.transform.SetParent(dataObj.transform.GetChild(1).GetChild(3).GetChild(1));
            SetColorForFilter(rightYearFilter, 1);
            rightYearFilter.transform.localPosition = new Vector3(-3, 1, 0);
            rightYearFilter.transform.localScale = new Vector3(0.05f, 3, 1);
            rightYearFilter.transform.localEulerAngles = new Vector3(0, 0, -90);
            rightYearFilters.Add(rightYearFilter);

            rightYearFilter = (GameObject)Instantiate(FilterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            rightYearFilter.name = "rightYearFilter";
            rightYearFilter.transform.SetParent(dataObj.transform.GetChild(1).GetChild(6).GetChild(1));
            SetColorForFilter(rightYearFilter, 1);
            rightYearFilter.transform.localPosition = new Vector3(-3, 1, 0);
            rightYearFilter.transform.localScale = new Vector3(0.05f, 3, 1);
            rightYearFilter.transform.localEulerAngles = new Vector3(0, 0, -90);
            rightYearFilters.Add(rightYearFilter);

            GameObject leftValueFilter = (GameObject)Instantiate(FilterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            leftValueFilter.name = "leftValueFilter";
            leftValueFilter.transform.SetParent(dataObj.transform.GetChild(1).GetChild(2).GetChild(1));
            SetColorForFilter(leftValueFilter, 1);
            leftValueFilter.transform.localPosition = new Vector3(-3, 0, 0);
            leftValueFilter.transform.localScale = new Vector3(0.05f, 3, 1);
            leftValueFilter.transform.localEulerAngles = new Vector3(0, 0, -90);
            leftValueFilters.Add(leftValueFilter);

            leftValueFilter = (GameObject)Instantiate(FilterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            leftValueFilter.name = "leftValueFilter";
            leftValueFilter.transform.SetParent(dataObj.transform.GetChild(1).GetChild(5).GetChild(1));
            SetColorForFilter(leftValueFilter, 1);
            leftValueFilter.transform.localPosition = new Vector3(3, 0, 0);
            leftValueFilter.transform.localScale = new Vector3(0.05f, 3, 1);
            leftValueFilter.transform.localEulerAngles = new Vector3(0, 0, 90);
            leftValueFilters.Add(leftValueFilter);
            GameObject tmp = new GameObject();
            tmp.transform.SetParent(leftValueFilter.transform);

            GameObject rightValueFilter = (GameObject)Instantiate(FilterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            rightValueFilter.name = "rightValueFilter";
            rightValueFilter.transform.SetParent(dataObj.transform.GetChild(1).GetChild(2).GetChild(1));
            SetColorForFilter(rightValueFilter, 1);
            rightValueFilter.transform.localPosition = new Vector3(-3, 1, 0);
            rightValueFilter.transform.localScale = new Vector3(0.05f, 3, 1);
            rightValueFilter.transform.localEulerAngles = new Vector3(0, 0, -90);
            rightValueFilters.Add(rightValueFilter);

            rightValueFilter = (GameObject)Instantiate(FilterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            rightValueFilter.name = "rightValueFilter";
            rightValueFilter.transform.SetParent(dataObj.transform.GetChild(1).GetChild(5).GetChild(1));
            SetColorForFilter(rightValueFilter, 1);
            rightValueFilter.transform.localPosition = new Vector3(3, 1, 0);
            rightValueFilter.transform.localScale = new Vector3(0.05f, 3, 1);
            rightValueFilter.transform.localEulerAngles = new Vector3(0, 0, 90);
            rightValueFilters.Add(rightValueFilter);
            GameObject tmp2 = new GameObject();
            tmp2.transform.SetParent(rightValueFilter.transform);

            GameObject leftValuePlane = (GameObject)Instantiate(CuttingPlanePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            leftValuePlane.name = "leftValuePlane";
            leftValuePlane.transform.SetParent(dataObj.transform.GetChild(1).GetChild(2).GetChild(1));
            leftValuePlane.transform.localPosition = Vector3.zero;
            leftValuePlane.transform.localScale = Vector3.one;
            leftValuePlane.transform.localEulerAngles = Vector3.zero;
            SetColorForFilter(leftValuePlane.transform.GetChild(0).gameObject, 0);
            leftValuePlanes.Add(leftValuePlane);

            GameObject rightValuePlane = (GameObject)Instantiate(CuttingPlanePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            rightValuePlane.name = "rightValuePlane";
            rightValuePlane.transform.SetParent(dataObj.transform.GetChild(1).GetChild(2).GetChild(1));
            SetColorForFilter(rightValuePlane.transform.GetChild(0).gameObject, 0);
            rightValuePlane.transform.localPosition = Vector3.zero;
            rightValuePlane.transform.localScale = Vector3.one;
            rightValuePlane.transform.localEulerAngles = Vector3.zero;
            rightValuePlanes.Add(rightValuePlane);
        }
        GetChessBoardDic();
    }

    private void GetChessBoardDic()
    {
        if (chessBoardPoints != null && dataSM != null) {
            if (dataSM.Count > 0 && chessBoardPoints.Count != dataSM.Count)
            {
                foreach (GameObject sm in dataSM)
                {
                    Transform barChart = sm.transform.GetChild(1);
                    chessBoardPoints.Add(barChart.name, GetChessBoardPoint(barChart));
                }
            }
        }  
    }

    private Dictionary<Vector2, Vector3> GetChessBoardPoint(Transform barChart)
    {
        
        BigMesh bm = barChart.GetChild(0).GetChild(0).GetComponent<BigMesh>();

        Dictionary<Vector2, Vector3> points = new Dictionary<Vector2, Vector3>();
        float sizeDelta = 0.25f;
        for (int xDelta = 1; xDelta <= (int)Mathf.Sqrt(barNum); xDelta++)
        {
            for (int zDelta = 1; zDelta <= (int)Mathf.Sqrt(barNum); zDelta++)
            {
                Vector3 vertice = barChart.GetChild(0).GetChild(0).TransformPoint(bm.getBigMeshVertices()[(xDelta - 1) * 5 + (zDelta - 1)] + Vector3.up * 0.125f);
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Transform yAxisParent = barChart.GetChild(2);
                sphere.transform.SetParent(yAxisParent);
                sphere.transform.position = vertice;
                sphere.transform.localScale = Vector3.one * 0.01f;

                // get correct scale
                points.Add(new Vector2(xDelta, zDelta), new Vector3(barChart.localPosition.x + sizeDelta * (xDelta - 1), sphere.transform.localPosition.y, barChart.localPosition.z + sizeDelta * (zDelta - 1)));
                Destroy(sphere);
            }
        }
        return points;
    }

    public void ControllerTriggerReleased(string controller) {
        if (!creatingCube) {
            if (controller == "left")
            {
                if (leftClickEmptySpace)
                {
                    if (!rightClickEmptySpace)
                    {
                        RefreshDataSet2();
                        leftClickEmptySpace = false;
                    }
                }
            }
            else
            {
                if (rightClickEmptySpace)
                {
                    if (!leftClickEmptySpace)
                    {
                        RefreshDataSet2();
                        rightClickEmptySpace = false;
                    }
                }
            }
        }
        
    }


    public void CheckDataset2KeepHighted(string controller, Vector3 touchPointPosition) {

        if (!creatingCube && !creatingWorldInMiniture)
        {
            if (controller == "left")
            {
                //rightPressedCount = 0;
                if (leftFindHighlighedAxisFromCollision)
                {
                    ResetRangeSelectionChessBoardBrushingBool();

                    leftHighlighed = false;
                    rightHighlighed = false;

                    leftAxisHighlighed = true;
                    leftFindHighlighedV2FromCollision = leftFindHoveringV2FromCollision;

                    triggerPressedForFilterMoving = true;
                    if (rightAxisHighlighed)
                    {
                        CalculateChessBoardBool(new Vector4(leftFindHighlighedV2FromCollision.x, rightFindHighlighedV2FromCollision.x, leftFindHighlighedV2FromCollision.y, rightFindHighlighedV2FromCollision.y), "axis");
                    }
                    else
                    {
                        CalculateChessBoardBool(new Vector4(leftFindHighlighedV2FromCollision.x, 0, leftFindHighlighedV2FromCollision.y, 0), "axis");
                    }
                    triggerPressedForFilterMoving = false;
                }
                else
                {
                    if (currentFindHightlighted == "left")
                    {
                        leftAxisHighlighed = false;
                        ResetRangeSelectionChessBoardBrushingBool();

                        if (rightHighlighed)
                            rightHighlighed = false;
                        leftHighlighed = true;

                        leftFindHighlighedV2FromChessBoard = leftFindHoveringV2FromChessBoard;
                        if (leftFindHighlighedV2FromChessBoard != Vector2.zero)
                        {
                            triggerPressedForFilterMoving = true;
                            CalculateChessBoardBool(new Vector4(leftFindHighlighedV2FromChessBoard.x, 0, leftFindHighlighedV2FromChessBoard.y, 0), "single");
                            triggerPressedForFilterMoving = false;
                        }
                    }
                    else
                    {
                        bool inSM = false;
                        foreach (GameObject go in dataSM)
                        {
                            if (go.transform.GetChild(0).InverseTransformPoint(touchPointPosition).x >= -0.06f && go.transform.GetChild(0).InverseTransformPoint(touchPointPosition).x <= 1.056f && go.transform.GetChild(0).InverseTransformPoint(touchPointPosition).z >= -0.06f && go.transform.GetChild(0).InverseTransformPoint(touchPointPosition).z <= 1.056f && go.transform.GetChild(0).InverseTransformPoint(touchPointPosition).y >= 0 && go.transform.GetChild(0).InverseTransformPoint(touchPointPosition).y <= 1.056f)
                            {
                                inSM = true;
                            }
                        }

                        if (!inSM)
                        {
                            //leftPressedCount++;

                            //if (leftPressedCount >= 2)
                            //{
                            //    leftPressedCount = 0;
                            //    RefreshDataSet2();
                            //}
                            leftClickEmptySpace = true;
                        }
                    }
                }
            }
            if (controller == "right")
            {
                //leftPressedCount = 0;
                if (rightFindHighlighedAxisFromCollision)
                {
                    ResetRangeSelectionChessBoardBrushingBool();
                    leftHighlighed = false;
                    rightHighlighed = false;
                    rightAxisHighlighed = true;

                    rightFindHighlighedV2FromCollision = rightFindHoveringV2FromCollision;
                    triggerPressedForFilterMoving = true;
                    if (leftAxisHighlighed)
                    {
                        CalculateChessBoardBool(new Vector4(leftFindHighlighedV2FromCollision.x, rightFindHighlighedV2FromCollision.x, leftFindHighlighedV2FromCollision.y, rightFindHighlighedV2FromCollision.y), "axis");
                    }
                    else
                    {
                        CalculateChessBoardBool(new Vector4(0, rightFindHighlighedV2FromCollision.x, 0, rightFindHighlighedV2FromCollision.y), "axis");
                    }
                    triggerPressedForFilterMoving = false;
                }
                else
                {
                    if (currentFindHightlighted == "right")
                    {
                        ResetRangeSelectionChessBoardBrushingBool();

                        rightAxisHighlighed = false;
                        if (leftHighlighed && !leftAxisHighlighed)
                            leftHighlighed = false;
                        rightHighlighed = true;

                        rightFindHighlighedV2FromChessBoard = rightFindHoveringV2FromChessBoard;
                        if (rightFindHighlighedV2FromChessBoard != Vector2.zero)
                        {
                            triggerPressedForFilterMoving = true;
                            CalculateChessBoardBool(new Vector4(0, rightFindHighlighedV2FromChessBoard.x, 0, rightFindHighlighedV2FromChessBoard.y), "single");
                            triggerPressedForFilterMoving = false;
                        }
                    }
                    else
                    {
                        bool inSM = false;
                        foreach (GameObject go in dataSM)
                        {
                            if (go.transform.GetChild(0).InverseTransformPoint(touchPointPosition).x >= -0.06f && go.transform.GetChild(0).InverseTransformPoint(touchPointPosition).x <= 1.056f && go.transform.GetChild(0).InverseTransformPoint(touchPointPosition).z >= -0.06f && go.transform.GetChild(0).InverseTransformPoint(touchPointPosition).z <= 1.056f && go.transform.GetChild(0).InverseTransformPoint(touchPointPosition).y >= 0 && go.transform.GetChild(0).InverseTransformPoint(touchPointPosition).y <= 1.056f)
                            {
                                inSM = true;
                            }
                        }

                        if (!inSM)
                        {
                            //rightPressedCount++;
                            //if (rightPressedCount >= 2)
                            //{
                            //    rightPressedCount = 0;
                            //    RefreshDataSet2();
                            //}
                            rightClickEmptySpace = true;
                        }
                    }
                }
            }
        }
    }

    private void RefreshDataSet2() {

        leftHighlighed = false;
        rightHighlighed = false;
        leftAxisHighlighed = false;
        rightAxisHighlighed = false;

        if (GameObject.Find("leftCollisionDetector") != null && GameObject.Find("rightCollisionDetector") != null)
        {
            GameObject.Find("leftCollisionDetector").GetComponent<CollisionDetection>().selectedBarYAxisIndex = new Vector2(0, 1);
            GameObject.Find("rightCollisionDetector").GetComponent<CollisionDetection>().selectedBarYAxisIndex = new Vector2(0, 1);
        }

        // reset brushing Bool
        chessBoardBrushingBool = new bool[barNum];
        for (int i = 0; i < filteredChessBoardBrushingBool.Length; i++)
        {
            filteredChessBoardBrushingBool[i] = true;
        }
        for (int i = 0; i < rangeSelectionChessBoardBrushingBool.Length; i++)
        {
            rangeSelectionChessBoardBrushingBool[i] = true;
        }
        foreach (GameObject sm in dataSM)
        {
            if (dataset == 1)
            {
                if (sm.transform.Find("CubeSelection") != null)
                {
                    Destroy(sm.transform.Find("CubeSelection").gameObject);
                }
            }
            else
            {
                if (sm.transform.GetChild(0).Find("CubeSelection") != null)
                {
                    Destroy(sm.transform.GetChild(0).Find("CubeSelection").gameObject);
                }
            }
        }

        // reset filter buttons for y axis
        foreach (GameObject filter1 in leftValueFilters)
        {
            if (filter1.transform.childCount > 0)
            {
                filter1.transform.localPosition = new Vector3(3, 0, 0);
            }
            else
            {
                filter1.transform.localPosition = new Vector3(-3, 0, 0);
            }
        }

        foreach (GameObject filter2 in rightValueFilters)
        {
            if (filter2.transform.childCount > 0)
            {
                filter2.transform.localPosition = new Vector3(3, 1, 0);
            }
            else
            {
                filter2.transform.localPosition = new Vector3(-3, 1, 0);
            }
        }

        foreach (GameObject plane1 in leftValuePlanes)
        {
            plane1.transform.localPosition = new Vector3(0, 0, 0);
            SetColorForFilter(plane1.transform.GetChild(0).gameObject, 0f);
        }

        foreach (GameObject plane2 in rightValuePlanes)
        {
            plane2.transform.localPosition = new Vector3(0, 1, 0);
            SetColorForFilter(plane2.transform.GetChild(0).gameObject, 0f);
        }

        // reset filter buttons for x, z axes
        triggerPressedForFilterMoving = true;
        CalculateChessBoardBool(new Vector4(0, 0, 0, 0), "single");
        triggerPressedForFilterMoving = false;
        //}
    }


    // single brushing variable assignment
    private void DetectLeftTouchBarInteractionForBarChart()
    {
        bool insideSM = false;

        GameObject leftTouchbar = null;
        if (globalLeftController != null)
        {
            Transform leftController = globalLeftController;
            if (leftController.GetChild(0) != null)
            {
                Transform lModel = leftController.GetChild(0);
                if (lModel.Find("tip") != null)
                {
                    if (lModel.Find("tip").GetChild(0) != null)
                    {
                        Transform lAttach = lModel.Find("tip").GetChild(0);
                        if (lAttach.childCount > 0)
                        {
                            leftTouchbar = lAttach.GetChild(0).gameObject;
                        }
                    }
                }
            }
        }

        if (leftTouchbar != null)
        {
            int x = 0;
            int z = 0;
            Transform leftB = leftTouchbar.transform.GetChild(0);
            if (!circleLayout) {
                if (leftB.position.z > 0.6f) {
                    insideSM = true;
                }
                else {
                    insideSM = false;
                }
            } else if (fullCircle) {
                if (Vector3.Distance(Vector3.zero, new Vector3(leftB.position.x, 0, leftB.position.z)) > 0.6f)
                {
                    insideSM = true;
                }
                else
                {
                    insideSM = false;
                }
            } else {
                if (Vector3.Distance(new Vector3(0, 0, -0.6f), new Vector3(leftB.position.x, 0, leftB.position.z)) > 1.4f)
                {
                    insideSM = true;
                }
                else
                {
                    insideSM = false;
                }
            }


            if (insideSM) {
                foreach (KeyValuePair<string, Dictionary<Vector2, Vector3>> entry in chessBoardPoints)
                {
                    Transform barChart = GameObject.Find(entry.Key).transform;
                    if (barChart.InverseTransformPoint(leftB.position).x >= -0.1f && barChart.InverseTransformPoint(leftB.position).x <= 1.25f && barChart.InverseTransformPoint(leftB.position).z >= -0.1f && barChart.InverseTransformPoint(leftB.position).z <= 1.25f && barChart.InverseTransformPoint(leftB.position).y >= 0 && barChart.InverseTransformPoint(leftB.position).y <= 1.25f)
                    //if (CheckDiff(leftB.position.x, barChart.position.x, 0.4f, false) && CheckDiff(leftB.position.z, barChart.position.z, 0.4f, false))
                    {
                        foreach (KeyValuePair<Vector2, Vector3> secondEntry in entry.Value)
                        {
                            Transform yAxis = barChart.GetChild(2).GetChild(1);
                            Vector3 tilesLocalToWorld = barChart.parent.TransformPoint(secondEntry.Value);
                            Vector3 tilesLocalToWorldY = yAxis.TransformPoint(secondEntry.Value);
                            //Debug.Log((CheckDiff(leftB.position.x, tilesLocalToWorld.x, 0.02f, true)) + " " +  (CheckDiff(leftB.position.z, tilesLocalToWorld.z, 0.02f, true)) + " " +  (leftB.position.y >= barChart.position.y) + " " + (leftB.position.y <= tilesLocalToWorld.y + 0.01f));
                            if (CheckDiff(leftB.position.x, tilesLocalToWorld.x, 0.03f, true) && CheckDiff(leftB.position.z, tilesLocalToWorld.z, 0.03f, true) && leftB.position.y >= barChart.position.y && leftB.position.y <= tilesLocalToWorldY.y + 0.01f)
                            {
                                x = (int)secondEntry.Key.x;
                                z = (int)secondEntry.Key.y;
                            }
                        }
                    }
                }
            }
            
            if (x != 0 || z != 0)
            {
                leftFindHighlighedFromChessBoard = true;
                leftFindHoveringV2FromChessBoard = new Vector2(x, z);
                if (currentFindHightlighted == "")
                {
                    currentFindHightlighted = "left";
                }
            }
            else if (x == 0 && z == 0)
            {
                leftFindHighlighedFromChessBoard = false;
                leftFindHoveringV2FromChessBoard = new Vector2(0, 0);
                if (currentFindHightlighted == "left")
                    currentFindHightlighted = "";
            }          
        }
    }

    private void DetectRightTouchBarInteractionForBarChart()
    {
        bool insideSM = false;

        GameObject rightTouchbar = null;
        if (globalRightController != null)
        {
            Transform rightController = globalRightController;
            if (rightController.GetChild(0) != null)
            {
                Transform rModel = rightController.GetChild(0);
                if (rModel.Find("tip") != null)
                {
                    if (rModel.Find("tip").GetChild(0) != null)
                    {
                        Transform rAttach = rModel.Find("tip").GetChild(0);
                        if (rAttach.childCount > 0)
                        {
                            rightTouchbar = rAttach.GetChild(0).gameObject;
                        }
                    }
                }
            }
        }

        if (rightTouchbar != null)
        {
            int x = 0;
            int z = 0;
            Transform rightB = rightTouchbar.transform.GetChild(0);
            if (!circleLayout)
            {
                if (rightB.position.z > 0.6f)
                {
                    insideSM = true;
                }
                else
                {
                    insideSM = false;
                }
            }
            else if (fullCircle)
            {
                if (Vector3.Distance(Vector3.zero, new Vector3(rightB.position.x, 0, rightB.position.z)) > 0.6f)
                {
                    insideSM = true;
                }
                else
                {
                    insideSM = false;
                }
            }
            else
            {
                if (Vector3.Distance(new Vector3(0, 0, -0.6f), new Vector3(rightB.position.x, 0, rightB.position.z)) > 1.4f)
                {
                    insideSM = true;
                }
                else
                {
                    insideSM = false;
                }
            }

            if (insideSM) {
                foreach (KeyValuePair<string, Dictionary<Vector2, Vector3>> entry in chessBoardPoints)
                {
                    Transform barChart = GameObject.Find(entry.Key).transform;


                    if (barChart.InverseTransformPoint(rightB.position).x >= -0.1f && barChart.InverseTransformPoint(rightB.position).x <= 1.25f && barChart.InverseTransformPoint(rightB.position).z >= -0.1f && barChart.InverseTransformPoint(rightB.position).z <= 1.25f && barChart.InverseTransformPoint(rightB.position).y >= 0 && barChart.InverseTransformPoint(rightB.position).y <= 1.25f)
                    //if (CheckDiff(rightB.position.x, barChart.position.x, 0.4f, false) && CheckDiff(rightB.position.z, barChart.position.z, 0.4f, false))
                    {
                        foreach (KeyValuePair<Vector2, Vector3> secondEntry in entry.Value)
                        {
                            Transform yAxis = barChart.GetChild(2).GetChild(1);
                            Vector3 tilesLocalToWorld = barChart.parent.TransformPoint(secondEntry.Value);
                            Vector3 tilesLocalToWorldY = yAxis.TransformPoint(secondEntry.Value);
                            //Debug.Log((CheckDiff(rightB.position.x, tilesLocalToWorld.x, 0.02f, true)) + " " + (CheckDiff(rightB.position.z, tilesLocalToWorld.z, 0.02f, true)) + " " + (rightB.position.y >= barChart.position.y) + " " + (rightB.position.y <= tilesLocalToWorld.y + 0.01f));
                            if (CheckDiff(rightB.position.x, tilesLocalToWorld.x, 0.03f, true) && CheckDiff(rightB.position.z, tilesLocalToWorld.z, 0.03f, true) && rightB.position.y >= barChart.position.y && rightB.position.y <= tilesLocalToWorldY.y + 0.01f)
                            {
                                x = (int)secondEntry.Key.x;
                                z = (int)secondEntry.Key.y;
                            }

                        }
                    }
                }
            }
            

            if (x != 0 || z != 0)
            {
                rightFindHighlighedFromChessBoard = true;
                rightFindHoveringV2FromChessBoard = new Vector2(x, z);
                if (currentFindHightlighted == "") {
                    currentFindHightlighted = "right";
                }
            }
            else if (x == 0 && z == 0)
            {
                rightFindHighlighedFromChessBoard = false;
                rightFindHoveringV2FromChessBoard = new Vector2(0, 0);
                if(currentFindHightlighted == "right")
                    currentFindHightlighted = "";
            }
        }
    }

    private void DetectBarChartInteraction()
    {
        if (!creatingCube && !creatingWorldInMiniture)
        {
            DetectLeftTouchBarInteractionForBarChart();
            DetectRightTouchBarInteractionForBarChart();

            // Axis hovering detection
            if (!leftFindHighlighedFromChessBoard && !rightFindHighlighedFromChessBoard && (leftFindHighlighedAxisFromCollision || rightFindHighlighedAxisFromCollision))
            {
                if (leftFindHighlighedAxisFromCollision && rightFindHighlighedAxisFromCollision)
                {

                    CalculateHoveringChessBoardBool(new Vector4(leftFindHoveringV2FromCollision.x, rightFindHoveringV2FromCollision.x, leftFindHoveringV2FromCollision.y, rightFindHoveringV2FromCollision.y), "axis");
                }
                else
                {
                    if (leftFindHighlighedAxisFromCollision)
                    {
                        // highlight left controller touched axis
                        if (rightAxisHighlighed)
                        {
                            CalculateHoveringChessBoardBool(new Vector4(leftFindHoveringV2FromCollision.x, rightFindHighlighedV2FromCollision.x, leftFindHoveringV2FromCollision.y, rightFindHighlighedV2FromCollision.y), "axis");
                        }
                        else
                        {
                            CalculateHoveringChessBoardBool(new Vector4(leftFindHoveringV2FromCollision.x, 0, leftFindHoveringV2FromCollision.y, 0), "axis");
                        }
                    }
                    else if (rightFindHighlighedAxisFromCollision)
                    {
                        // highlight right controller touched axis
                        if (leftAxisHighlighed)
                        {
                            CalculateHoveringChessBoardBool(new Vector4(leftFindHighlighedV2FromCollision.x, rightFindHoveringV2FromCollision.x, leftFindHighlighedV2FromCollision.y, rightFindHoveringV2FromCollision.y), "axis");
                        }
                        else
                        {
                            CalculateHoveringChessBoardBool(new Vector4(0, rightFindHoveringV2FromCollision.x, 0, rightFindHoveringV2FromCollision.y), "axis");
                        }
                    }
                }
            }
            // single hovering detection
            else if (!leftFindHighlighedAxisFromCollision && !rightFindHighlighedAxisFromCollision && (leftFindHighlighedFromChessBoard || rightFindHighlighedFromChessBoard))
            {
                if (rightFindHighlighedFromChessBoard && leftFindHighlighedFromChessBoard)
                {
                    if (currentFindHightlighted == "right")
                    {
                        CalculateHoveringChessBoardBool(new Vector4(0, rightFindHoveringV2FromChessBoard.x, 0, rightFindHoveringV2FromChessBoard.y), "single");
                    }
                    else if (currentFindHightlighted == "left")
                    {
                        CalculateHoveringChessBoardBool(new Vector4(leftFindHoveringV2FromChessBoard.x, 0, leftFindHoveringV2FromChessBoard.y, 0), "single");
                    }
                }
                else if (rightFindHighlighedFromChessBoard)
                {
                    CalculateHoveringChessBoardBool(new Vector4(0, rightFindHoveringV2FromChessBoard.x, 0, rightFindHoveringV2FromChessBoard.y), "single");
                }
                else if (leftFindHighlighedFromChessBoard)
                {
                    CalculateHoveringChessBoardBool(new Vector4(leftFindHoveringV2FromChessBoard.x, 0, leftFindHoveringV2FromChessBoard.y, 0), "single");
                }
            }
            else if (!leftFindHighlighedAxisFromCollision && !rightFindHighlighedAxisFromCollision && !leftFindHighlighedFromChessBoard && !rightFindHighlighedFromChessBoard)
            {
                if (!leftFilterMoving && !rightFilterMoving)
                {
                    BarChartCreator bcc = GameObject.Find("BarChartManagement").GetComponent<BarChartCreator>();
                    bcc.UpdateBrushing(finalChessBoardBrushingBool);
                }
            }
        }
        else
        {
            //leftPressedCount = 0;
            //rightPressedCount = 0;
        }
    }


    // filter variables assignment
    public void FilterBarChartFromCollision(string axis, string controller, float position)
    {

        if (axis == "Country")
        {
            if (controller == "left")
            {
                foreach (GameObject filter in leftCountryFilters)
                {
                    filter.transform.localPosition = new Vector3(-3, position / Mathf.Sqrt(barNum), 0);
                }
                currentCountryLeftFilterPosition = (int)position;
                leftFindHighlighedV2FromCollision = new Vector2(currentCountryLeftFilterPosition + 1, leftFindHighlighedV2FromCollision.y);
            }
            else
            {
                foreach (GameObject filter in rightCountryFilters)
                {
                    filter.transform.localPosition = new Vector3(-3, position / Mathf.Sqrt(barNum), 0);
                }
                currentCountryRightFilterPosition = (int)position;
                rightFindHighlighedV2FromCollision = new Vector2(currentCountryRightFilterPosition, rightFindHighlighedV2FromCollision.y);
            }
        }
        else if (axis == "Year")
        {
            if (controller == "left")
            {
                foreach (GameObject filter in leftYearFilters)
                {
                    filter.transform.localPosition = new Vector3(-3, position / Mathf.Sqrt(barNum), 0);
                }
                currentYearLeftFilterPosition = (int)position;
                leftFindHighlighedV2FromCollision = new Vector2(leftFindHighlighedV2FromCollision.x, currentYearLeftFilterPosition + 1);
            }
            else
            {
                foreach (GameObject filter in rightYearFilters)
                {
                    filter.transform.localPosition = new Vector3(-3, position / Mathf.Sqrt(barNum), 0);
                }
                currentYearRightFilterPosition = (int)position;
                rightFindHighlighedV2FromCollision = new Vector2(rightFindHighlighedV2FromCollision.x, currentYearRightFilterPosition);
            }
        }


        if (axis == "Value")
        {
            CalculateYAxisChessBoardBoolSeperately();
        }
        else {
            Vector4 filterPositions = new Vector4(currentCountryLeftFilterPosition, currentCountryRightFilterPosition, currentYearLeftFilterPosition, currentYearRightFilterPosition);
            CalculateChessBoardBool(filterPositions, "filter");
        }
        
    }

    private void CalculateYAxisChessBoardBoolSeperately() {

        // apply for value filter

        for (int i = 0; i < filteredChessBoardBrushingBool.Length; i++)
        {
            filteredChessBoardBrushingBool[i] = true;
        }

        //filteredChessBoardBrushingBool = chessBoardBrushingBool;
        foreach (GameObject sm in dataSM)
        {
            Transform minFilter = sm.transform.GetChild(0).GetChild(2).GetChild(1).GetChild(2);
            Transform maxFilter = sm.transform.GetChild(0).GetChild(2).GetChild(1).GetChild(3);

            //Debug.Log(minFilter.localPosition.y + " " + maxFilter.localPosition.y);
            if (minFilter.localPosition.y != 0)
            {
                for (int i = 0; i < (int)Mathf.Sqrt(barNum); i++)
                {
                    for (int j = 0; j < (int)Mathf.Sqrt(barNum); j++)
                    {
                        if (chessBoardPoints[sm.transform.GetChild(0).name][new Vector2(i + 1, j + 1)].y < minFilter.localPosition.y)
                        {
                            filteredChessBoardBrushingBool[i * (int)Mathf.Sqrt(barNum) + j] = false;
                        }
                    }
                }
            }

            if (maxFilter.localPosition.y != 1)
            {
                for (int i = 0; i < (int)Mathf.Sqrt(barNum); i++)
                {
                    for (int j = 0; j < (int)Mathf.Sqrt(barNum); j++)
                    {
                        if (chessBoardPoints[sm.transform.GetChild(0).name][new Vector2(i + 1, j + 1)].y > maxFilter.localPosition.y)
                        {
                            filteredChessBoardBrushingBool[i * (int)Mathf.Sqrt(barNum) + j] = false;
                        }
                    }
                }
            }
        }

        BarChartCreator bcc = GameObject.Find("BarChartManagement").GetComponent<BarChartCreator>();
        finalChessBoardBrushingBool = CalculateFullLogicOfBrushingAndFiltering();
        bcc.UpdateBrushing(finalChessBoardBrushingBool);
    }

    // after assign values then calculate chess board bool for coloring
    private void CalculateChessBoardBool(Vector4 input, string mode) {

        BarChartCreator bcc = GameObject.Find("BarChartManagement").GetComponent<BarChartCreator>();
        chessBoardBrushingBool = new bool[barNum];

        int leftCountryBrushControl = (int)input.x;
        int rightCountryBrushControl = (int)input.y;
        int leftYearBrushControl = (int)input.z;
        int rightYearBrushControl = (int)input.w;

        if (mode == "single")
        {
            if (leftYearBrushControl != 0 && leftCountryBrushControl != 0)
            {
                for (int i = 1; i <= (int)Mathf.Sqrt(barNum); i++)
                {
                    for (int j = 1; j <= (int)Mathf.Sqrt(barNum); j++)
                    {
                        if (i == leftCountryBrushControl && j == leftYearBrushControl)
                        {
                            chessBoardBrushingBool[(i - 1) * (int)Mathf.Sqrt(barNum) + (j - 1)] = true;
                        }
                        else
                        {
                            chessBoardBrushingBool[(i - 1) * (int)Mathf.Sqrt(barNum) + (j - 1)] = false;
                        }
                    }
                }
            }

            if (rightYearBrushControl != 0 && rightCountryBrushControl != 0)
            {
                for (int i = 1; i <= (int)Mathf.Sqrt(barNum); i++)
                {
                    for (int j = 1; j <= (int)Mathf.Sqrt(barNum); j++)
                    {
                        if (i == rightCountryBrushControl && j == rightYearBrushControl)
                        {
                            chessBoardBrushingBool[(i - 1) * (int)Mathf.Sqrt(barNum) + (j - 1)] = true;
                        }
                        else
                        {
                            chessBoardBrushingBool[(i - 1) * (int)Mathf.Sqrt(barNum) + (j - 1)] = false;
                        }
                    }
                }
            }

            // check all false
            bool allFalse = true;
            foreach (bool tmp in chessBoardBrushingBool)
            {
                if (tmp)
                    allFalse = false;
            }


            if (allFalse)
            {
                for (int i = 0; i < chessBoardBrushingBool.Length; i++)
                {
                    chessBoardBrushingBool[i] = true;
                }
                triggerPressedForFilterMoving = true;
                SetFilterPosition(0, (int)Mathf.Sqrt(barNum), 0, (int)Mathf.Sqrt(barNum));
                triggerPressedForFilterMoving = false;
            }
            else
            {

                if (leftCountryBrushControl == 0 && leftYearBrushControl == 0)
                {
                    SetFilterPosition(rightCountryBrushControl - 1, rightCountryBrushControl, rightYearBrushControl - 1, rightYearBrushControl);
                }
                else
                {
                    SetFilterPosition(leftCountryBrushControl - 1, leftCountryBrushControl, leftYearBrushControl - 1, leftYearBrushControl);
                }

            }
        }
        else if (mode == "axis")
        {
            if (leftYearBrushControl == 0 && leftCountryBrushControl != 0 && rightYearBrushControl != 0 && rightCountryBrushControl == 0)  // check intersection
            {
                chessBoardBrushingBool[(int)Mathf.Sqrt(barNum) * (leftCountryBrushControl - 1) + (rightYearBrushControl - 1)] = true;
                SetFilterPosition(leftCountryBrushControl - 1, leftCountryBrushControl, rightYearBrushControl - 1, rightYearBrushControl);
            }
            else if (leftYearBrushControl != 0 && leftCountryBrushControl == 0 && rightYearBrushControl == 0 && rightCountryBrushControl != 0)  // check intersection
            {
                chessBoardBrushingBool[(int)Mathf.Sqrt(barNum) * (rightCountryBrushControl - 1) + (leftYearBrushControl - 1)] = true;
                SetFilterPosition(rightCountryBrushControl - 1, rightCountryBrushControl, leftYearBrushControl - 1, leftYearBrushControl);
            }
            else if (leftYearBrushControl == 0 && leftCountryBrushControl != 0 && rightYearBrushControl == 0 && rightCountryBrushControl != 0) // check parallel
            {
                if (Math.Abs(leftCountryBrushControl - rightCountryBrushControl) != 0)
                {
                    for (int i = Math.Min(leftCountryBrushControl, rightCountryBrushControl); i <= Math.Max(leftCountryBrushControl, rightCountryBrushControl); i++)
                    {
                        for (int j = 0; j < (int)Mathf.Sqrt(barNum); j++)
                        {
                                chessBoardBrushingBool[(i - 1) * (int)Mathf.Sqrt(barNum) + j] = true;
                        }
                    }
                }
                else
                {
                    for (int i = 1; i <= (int)Mathf.Sqrt(barNum); i++)
                    {
                        for (int j = 1; j <= (int)Mathf.Sqrt(barNum); j++)
                        {
                            if (j == leftCountryBrushControl)
                                    chessBoardBrushingBool[(j - 1) * (int)Mathf.Sqrt(barNum) + (i - 1)] = true;
                        }
                    }
                }
                if (leftCountryBrushControl < rightCountryBrushControl)
                {
                    SetFilterPosition(leftCountryBrushControl - 1, rightCountryBrushControl, 0, (int)Mathf.Sqrt(barNum));
                }
                else
                {
                    SetFilterPosition(leftCountryBrushControl, rightCountryBrushControl - 1, 0, (int)Mathf.Sqrt(barNum));
                }

            }
            else if (leftYearBrushControl != 0 && leftCountryBrushControl == 0 && rightYearBrushControl != 0 && rightCountryBrushControl == 0) // check parallel
            {
                if (Math.Abs(leftYearBrushControl - rightYearBrushControl) != 0)
                {
                    for (int i = Math.Min(leftYearBrushControl, rightYearBrushControl); i <= Math.Max(leftYearBrushControl, rightYearBrushControl); i++)
                    {
                        for (int j = 0; j < (int)Mathf.Sqrt(barNum); j++)
                        {
                                chessBoardBrushingBool[j * (int)Mathf.Sqrt(barNum) + (i - 1)] = true;
                        }
                    }
                }
                else
                {
                    for (int i = 1; i <= (int)Mathf.Sqrt(barNum); i++)
                    {
                        for (int j = 1; j <= (int)Mathf.Sqrt(barNum); j++)
                        {
                            if (i == leftYearBrushControl)
                                    chessBoardBrushingBool[(j - 1) * (int)Mathf.Sqrt(barNum) + (i - 1)] = true;
                        }
                    }
                }
                if (leftYearBrushControl < rightYearBrushControl)
                {
                    SetFilterPosition(0, (int)Mathf.Sqrt(barNum), leftYearBrushControl - 1, rightYearBrushControl);
                }
                else
                {
                    SetFilterPosition(0, (int)Mathf.Sqrt(barNum), leftYearBrushControl, rightYearBrushControl - 1);
                }
            }
            else
            {
                if (rightCountryBrushControl == 0 && leftCountryBrushControl != 0)
                {
                    for (int i = 1; i <= (int)Mathf.Sqrt(barNum); i++)
                    {
                        for (int j = 1; j <= (int)Mathf.Sqrt(barNum); j++)
                        {
                            if (i == leftCountryBrushControl)
                                    chessBoardBrushingBool[(i - 1) * (int)Mathf.Sqrt(barNum) + (j - 1)] = true;
                        }
                    }
                    if (leftYearBrushControl == 0 && rightYearBrushControl == 0)
                    {
                        SetFilterPosition(leftCountryBrushControl - 1, leftCountryBrushControl, 0, (int)Mathf.Sqrt(barNum));
                    }
                    else
                    {
                        SetFilterPosition(leftCountryBrushControl - 1, leftCountryBrushControl, -1, -1);
                    }
                }
                else if (leftCountryBrushControl == 0 && rightCountryBrushControl != 0)
                {
                    for (int i = 1; i <= (int)Mathf.Sqrt(barNum); i++)
                    {
                        for (int j = 1; j <= (int)Mathf.Sqrt(barNum); j++)
                        {
                            if (i == rightCountryBrushControl)
                                    chessBoardBrushingBool[(i - 1) * (int)Mathf.Sqrt(barNum) + (j - 1)] = true;
                        }
                    }
                    if (leftYearBrushControl == 0 && rightYearBrushControl == 0)
                    {
                        SetFilterPosition(rightCountryBrushControl - 1, rightCountryBrushControl, 0, (int)Mathf.Sqrt(barNum));
                    }
                    else
                    {
                        SetFilterPosition(rightCountryBrushControl - 1, rightCountryBrushControl, -1, -1);
                    }
                }

                if (rightYearBrushControl == 0 && leftYearBrushControl != 0)
                {
                    for (int i = 1; i <= (int)Mathf.Sqrt(barNum); i++)
                    {
                        for (int j = 1; j <= (int)Mathf.Sqrt(barNum); j++)
                        {
                            if (j == leftYearBrushControl)
                                    chessBoardBrushingBool[(i - 1) * (int)Mathf.Sqrt(barNum) + (j - 1)] = true;
                        }
                    }
                    if (leftCountryBrushControl == 0 && rightCountryBrushControl == 0)
                    {
                        SetFilterPosition(0, (int)Mathf.Sqrt(barNum), leftYearBrushControl - 1, leftYearBrushControl);
                    }
                    else
                    {
                        SetFilterPosition(-1, -1, leftYearBrushControl - 1, leftYearBrushControl);
                    }
                }
                else if (rightYearBrushControl != 0 && leftYearBrushControl == 0)
                {
                    for (int i = 1; i <= (int)Mathf.Sqrt(barNum); i++)
                    {
                        for (int j = 1; j <= (int)Mathf.Sqrt(barNum); j++)
                        {
                            if (j == rightYearBrushControl)
                                    chessBoardBrushingBool[(i - 1) * (int)Mathf.Sqrt(barNum) + (j - 1)] = true;
                        }
                    }
                    if (leftCountryBrushControl == 0 && rightCountryBrushControl == 0)
                    {
                        SetFilterPosition(0, (int)Mathf.Sqrt(barNum), rightYearBrushControl - 1, rightYearBrushControl);
                    }
                    else
                    {
                        SetFilterPosition(-1, -1, rightYearBrushControl - 1, rightYearBrushControl);
                    }
                }

            }
        }
        else if (mode == "filter")
        {
            for (int i = leftCountryBrushControl; i < rightCountryBrushControl; i++)
            {
                for (int j = leftYearBrushControl; j < rightYearBrushControl; j++)
                {
                    if (!chessBoardBrushingBool[i * (int)Mathf.Sqrt(barNum) + j])
                    {
                        chessBoardBrushingBool[i * (int)Mathf.Sqrt(barNum) + j] = true;
                    }
                    else
                    {
                        chessBoardBrushingBool[i * (int)Mathf.Sqrt(barNum) + j] = false;
                    }
                }
            }
        }

        finalChessBoardBrushingBool = CalculateFullLogicOfBrushingAndFiltering();
        bcc.UpdateBrushing(finalChessBoardBrushingBool);
    }

    // combine all logic together
    private bool[] CalculateFullLogicOfBrushingAndFiltering() {

        bool[] finalBool = new bool[barNum];

        for (int i = 0; i < barNum; i++) {
            if (chessBoardBrushingBool[i] && filteredChessBoardBrushingBool[i] && rangeSelectionChessBoardBrushingBool[i])
            {
                finalBool[i] = true;
            }
            else {
                finalBool[i] = false;
            }
        }

        return finalBool;
    }


    private void ResetChessBoardBrushingBool() {
        leftHighlighed = false;
        rightHighlighed = false;
        for (int i = 0; i < barNum; i++)
        {
            chessBoardBrushingBool[i] = true;
        }
    }


    private void ResetRangeSelectionChessBoardBrushingBool() {
        for (int i = 0; i < rangeSelectionChessBoardBrushingBool.Length; i++)
        {
            rangeSelectionChessBoardBrushingBool[i] = true;
        }
        foreach (GameObject sm in dataSM)
        {
            if (dataset == 1)
            {
                if (sm.transform.Find("CubeSelection") != null)
                {
                    Destroy(sm.transform.Find("CubeSelection").gameObject);
                }
            }
            else
            {
                if (sm.transform.GetChild(0).Find("CubeSelection") != null)
                {
                    Destroy(sm.transform.GetChild(0).Find("CubeSelection").gameObject);
                }
            }
        }
    }


    // range selection control
    private void CreateRangeBrushingBox()
    {
        SteamVR_TrackedController ltc = null;
        SteamVR_TrackedController rtc = null;

        GameObject leftTouchbar = null;
        if (globalLeftController != null)
        {
            Transform leftController = globalLeftController;
            ltc = leftController.GetComponent<SteamVR_TrackedController>();
            if (leftController.GetChild(0) != null)
            {
                Transform lModel = leftController.GetChild(0);
                if (lModel.Find("tip") != null)
                {
                    if (lModel.Find("tip").GetChild(0) != null)
                    {
                        Transform lAttach = lModel.Find("tip").GetChild(0);
                        if (lAttach.childCount > 0)
                        {
                            leftTouchbar = lAttach.GetChild(0).gameObject;
                        }
                    }
                }
            }
        }

        GameObject rightTouchbar = null;
        if (globalRightController != null)
        {
            Transform rightController = globalRightController;
            rtc = rightController.GetComponent<SteamVR_TrackedController>();
            if (rightController.GetChild(0) != null)
            {
                Transform rModel = rightController.GetChild(0);
                if (rModel.Find("tip") != null)
                {
                    if (rModel.Find("tip").GetChild(0) != null)
                    {
                        Transform rAttach = rModel.Find("tip").GetChild(0);
                        if (rAttach.childCount > 0)
                        {
                            rightTouchbar = rAttach.GetChild(0).gameObject;
                        }
                    }
                }
            }
        }
        
        if (leftTouchbar != null && rightTouchbar != null && ltc != null && rtc != null)
        {
            Transform leftB = leftTouchbar.transform.GetChild(0);
            Transform rightB = rightTouchbar.transform.GetChild(0);

            Vector3 touchBarMiddlePosition = (leftB.position + rightB.position) / 2;
            if (ltc.triggerPressed && rtc.triggerPressed && !leftFindHighlighedAxisFromCollision && !rightFindHighlighedAxisFromCollision &&
                leftB.GetComponent<CollisionDetection>().draggedFilter == null && rightB.GetComponent<CollisionDetection>().draggedFilter == null)
            {
                Quaternion currentRotation = Quaternion.identity;
                Vector3 currentScale = Vector3.zero;

                // middle point method

                if (touchBarMiddleSM == null)
                {
                    foreach (GameObject go in dataSM)
                    {

                        if (go.transform.GetChild(0).InverseTransformPoint(touchBarMiddlePosition).x >= -0.06f && go.transform.GetChild(0).InverseTransformPoint(touchBarMiddlePosition).x <= 1.056f &&
                            go.transform.GetChild(0).InverseTransformPoint(touchBarMiddlePosition).z >= -0.06f && go.transform.GetChild(0).InverseTransformPoint(touchBarMiddlePosition).z <= 1.056f &&
                            go.transform.GetChild(0).InverseTransformPoint(touchBarMiddlePosition).y >= 0 && go.transform.GetChild(0).InverseTransformPoint(touchBarMiddlePosition).y <= 1.056f)
                        {
                            touchBarMiddleSM = go.transform;
                        }
                        currentScale = go.transform.localScale;
                        
                        currentRotation = go.transform.rotation;
                    }
                }


                if (touchBarMiddleSM != null || cubeSelectionCube != null)
                {
                    Destroy(worldInMiniture);
                    worldInMiniture = null;

                    if (!creatingCube && cubeSelectionCube == null)
                    {

                        if (touchBarMiddleSM.GetChild(0).Find("CubeSelection") != null)
                        {
                            Destroy(touchBarMiddleSM.GetChild(0).Find("CubeSelection").gameObject);
                        }
                        
                        creatingCube = true;

                        cubeSelectionCube = (GameObject)Instantiate(cubeSelectionPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                        cubeSelectionCube.name = "CubeSelection";
                        cubeSelectionCube.transform.localScale = Vector3.zero;
                        cubeSelectionCube.transform.localEulerAngles = Vector3.zero;
                        if (dataset == 1)
                        {
                            cubeSelectionCube.transform.SetParent(touchBarMiddleSM);
                        }
                        else {
                            cubeSelectionCube.transform.SetParent(touchBarMiddleSM.GetChild(0));
                            ResetChessBoardBrushingBool();
                        }         
                    }

                    if (cubeSelectionCube != null && creatingCube)
                    {
                        cubeSelectionCube.transform.localEulerAngles = Vector3.zero;
                        cubeSelectionCube.transform.position = leftB.position;
                        if (dataset == 1)
                        {
                            cubeSelectionCube.transform.localScale = touchBarMiddleSM.InverseTransformPoint(rightB.position) - touchBarMiddleSM.InverseTransformPoint(leftB.position);
                        }
                        else {
                            cubeSelectionCube.transform.localScale = touchBarMiddleSM.GetChild(0).InverseTransformPoint(rightB.position) - touchBarMiddleSM.GetChild(0).InverseTransformPoint(leftB.position);
                            CubeHovering(touchBarMiddleSM.GetChild(0));
                        }
                    }
                }
                else
                {
                    if (!leftFindHighlighedAxisFromCollision && !rightFindHighlighedAxisFromCollision) {

                        if (worldInMiniture == null && !creatingWorldInMiniture)
                        {
                            creatingWorldInMiniture = true;
                            worldInMiniture = (GameObject)Instantiate(worldInMiniturePrefab, new Vector3(0, 0, 0), Quaternion.identity);
                            worldInMiniture.name = "World in Miniture";
                            worldInMiniture.transform.localScale = Vector3.zero;
                            worldInMiniture.transform.SetParent(shelf.transform);
                            worldInMiniture.transform.position = (leftB.position + rightB.position) / 2;

                            oldEulerAngle = transform.eulerAngles;
                            oldWorldInMiniturePosition = worldInMiniture.transform.position;
                            
                        }

                        if (worldInMiniture != null && creatingWorldInMiniture)
                        {
                            rotationDelta -= Vector3.SignedAngle(rightB.position - leftB.position, oldV3FromLeftBtoRightB, Vector3.up);
                            if (circleLayout)
                            {
                                worldInMiniture.transform.LookAt(Camera.main.transform.position);
                                worldInMiniture.transform.localEulerAngles = new Vector3(0, worldInMiniture.transform.localEulerAngles.y + 180 + rotationDelta, 0);
                            }
                            else
                            {
                                worldInMiniture.transform.rotation = currentRotation;
                            }

                            // log the rotation
                            clockRotation = false;
                            antiClockRotation = false;
                            if (Vector3.SignedAngle(rightB.position - leftB.position, oldV3FromLeftBtoRightB, Vector3.up) > 0)
                            {
                                antiClockRotation = true;
                            }
                            else if (Vector3.SignedAngle(rightB.position - leftB.position, oldV3FromLeftBtoRightB, Vector3.up) < 0)
                            {
                                clockRotation = true;
                            }

                            foreach (GameObject go in dataSM)
                            {
                                go.transform.localEulerAngles -= Vector3.up * Vector3.SignedAngle(rightB.position - leftB.position, oldV3FromLeftBtoRightB, Vector3.up);
                            }
                            SMRotationDiff -= Vector3.SignedAngle(rightB.position - leftB.position, oldV3FromLeftBtoRightB, Vector3.up);
                            Color oldColor = worldInMiniture.GetComponent<MeshRenderer>().material.color;
                            oldColor.a = 1f;
                            worldInMiniture.GetComponent<MeshRenderer>().material.color = oldColor;
                            worldInMiniture.transform.localScale = currentScale / 2;
                            worldInMiniture.transform.position = (leftB.position + rightB.position) / 2;
                        }
                    }
                    
                }
            }
            else if (!ltc.triggerPressed && !rtc.triggerPressed)
            {

                clockRotation = false;
                antiClockRotation = false;
                scaleUp = false;
                scaleDown = false;
                
                controllerShelfDeltaSetup = false;
                if (cubeSelectionCube != null)
                {
                    
                    //Debug.Log(touchBarMiddleSM == null);
                    bool vertexInSM = false;
                    bool middleInSM = false;

                    if (dataset == 1)
                    {
                        foreach (Vector3 v in cubeSelectionCube.transform.GetChild(0).GetComponent<MeshFilter>().mesh.vertices)
                        {
                            Vector3 chessBoardLocalVFromCubeVertices = touchBarMiddleSM.InverseTransformPoint(cubeSelectionCube.transform.GetChild(0).TransformPoint(v));
   
                            if (chessBoardLocalVFromCubeVertices.x >= -0.26f && chessBoardLocalVFromCubeVertices.x <= 0.26f &&
                               chessBoardLocalVFromCubeVertices.z >= -0.26f && chessBoardLocalVFromCubeVertices.z <= 0.26f &&
                               chessBoardLocalVFromCubeVertices.y >= 0 && chessBoardLocalVFromCubeVertices.y <= 0.26f)
                            {
                                vertexInSM = true;
                            }
                        }

                        if (touchBarMiddleSM.InverseTransformPoint(touchBarMiddlePosition).x >= -0.26f && touchBarMiddleSM.InverseTransformPoint(touchBarMiddlePosition).x <= 0.26f &&
                                touchBarMiddleSM.InverseTransformPoint(touchBarMiddlePosition).z >= -0.26f && touchBarMiddleSM.InverseTransformPoint(touchBarMiddlePosition).z <= 0.26f &&
                                touchBarMiddleSM.InverseTransformPoint(touchBarMiddlePosition).y >= 0 && touchBarMiddleSM.InverseTransformPoint(touchBarMiddlePosition).y <= 0.26f)
                        {
                            middleInSM = true;
                        }
                    }
                    else {
                        foreach (Vector3 v in cubeSelectionCube.transform.GetChild(0).GetComponent<MeshFilter>().mesh.vertices)
                        {
                            Vector3 chessBoardLocalVFromCubeVertices = touchBarMiddleSM.GetChild(0).InverseTransformPoint(cubeSelectionCube.transform.GetChild(0).TransformPoint(v));
                            if (chessBoardLocalVFromCubeVertices.x >= -0.06f && chessBoardLocalVFromCubeVertices.x <= 1.056f &&
                               chessBoardLocalVFromCubeVertices.z >= -0.06f && chessBoardLocalVFromCubeVertices.z <= 1.056f &&
                               chessBoardLocalVFromCubeVertices.y >= 0 && chessBoardLocalVFromCubeVertices.y <= 1.056f)
                            {
                                vertexInSM = true;
                            }
                        }

                        if (touchBarMiddleSM.GetChild(0).InverseTransformPoint(touchBarMiddlePosition).x >= -0.06f && touchBarMiddleSM.GetChild(0).InverseTransformPoint(touchBarMiddlePosition).x <= 1.056f &&
                                touchBarMiddleSM.GetChild(0).InverseTransformPoint(touchBarMiddlePosition).z >= -0.06f && touchBarMiddleSM.GetChild(0).InverseTransformPoint(touchBarMiddlePosition).z <= 1.056f &&
                                touchBarMiddleSM.GetChild(0).InverseTransformPoint(touchBarMiddlePosition).y >= 0 && touchBarMiddleSM.GetChild(0).InverseTransformPoint(touchBarMiddlePosition).y <= 1.056f)
                        {
                            middleInSM = true;
                        }
                    }

                    // change filter for cube selection for dataset 1
                    cubeSelectionCube.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
                    if (!vertexInSM && !middleInSM)
                    {
                        //GameObject removedCube = touchBarMiddleSM.Find("CubeSelection").gameObject;
                        DestroyImmediate(cubeSelectionCube);
                        //Destroy(cubeSelectionCube);
                    }
                    touchBarMiddleSM = null;
                    
                    cubeSelectionCube = null;
                }

                if (creatingCube)
                {
                    creatingCube = false;
                    if (dataset == 1)
                    {
                    }
                    else {
                        CubeSelection();
                    }
                    
                }
                if (creatingWorldInMiniture) {
                    creatingWorldInMiniture = false;
                }

                if (worldInMiniture != null)
                {
                    Destroy(worldInMiniture);
                    worldInMiniture = null;
                }
            }
            oldV3FromLeftBtoRightB = rightB.position - leftB.position;
            //oldControllersDistance = Vector3.Distance(leftController.transform.position, rightController.transform.position);
            if (leftClickEmptySpace && rightClickEmptySpace)
            {
                leftClickEmptySpace = false;
                rightClickEmptySpace = false;
            }
        }

    }


    private void CubeHovering(Transform barChart) {

        for (int i = 0; i < barNum; i++) {
            hoveringRangeSelectionChessBoardBrushingBool[i] = true;
        }

        List<Vector2> pointsInBox = new List<Vector2>();

        int minCountry = (int)Mathf.Sqrt(barNum);
        int maxCountry = 1;
        int minYear = (int)Mathf.Sqrt(barNum);
        int maxYear = 1;
        float minValue = 1;
        float maxValue = 0;


        foreach (KeyValuePair<Vector2, Vector3> entry in chessBoardPoints[barChart.name])
        {
            if (PointInBox(entry.Value, GameObject.Find(barChart.name).transform, barChart.Find("CubeSelection")))
            {
                pointsInBox.Add(entry.Key);
                Vector3 adjustedPoint = entry.Value - GameObject.Find(barChart.name).transform.localPosition;
                if (entry.Key.x < minCountry)
                    minCountry = (int)entry.Key.x;
                if (entry.Key.x > maxCountry)
                    maxCountry = (int)entry.Key.x;
                if (entry.Key.y < minYear)
                    minYear = (int)entry.Key.y;
                if (entry.Key.y > maxYear)
                    maxYear = (int)entry.Key.y;
                if (adjustedPoint.y < minValue)
                    minValue = adjustedPoint.y;
                if (adjustedPoint.y > maxValue)
                    maxValue = adjustedPoint.y;
            }
        }
        //Debug.Log(minCountry + " " + maxCountry + " " + minYear + " " + maxYear);
        for (int i = 0; i < (int)Mathf.Sqrt(barNum); i++)
        {
            for (int j = 0; j < (int)Mathf.Sqrt(barNum); j++)
            {
                if (pointsInBox.Contains(new Vector2(i + 1, j + 1)))
                {
                    //hoveringRangeSelectionChessBoardBrushingBool[i * 10 + j] = true;
                }
                else
                {
                    hoveringRangeSelectionChessBoardBrushingBool[i * (int)Mathf.Sqrt(barNum) + j] = false;
                }
            }
        }

        BarChartCreator bcc = GameObject.Find("BarChartManagement").GetComponent<BarChartCreator>();
        bcc.UpdateBrushing(hoveringRangeSelectionChessBoardBrushingBool);

    }

    private void CubeSelection()
    {
        for (int i = 0; i < barNum; i++) {
            rangeSelectionChessBoardBrushingBool[i] = true;
        }

        int finalMinCountry = 1;
        int finalMaxCountry = (int)Mathf.Sqrt(barNum);
        int finalMinYear = 1;
        int finalMaxYear = (int)Mathf.Sqrt(barNum);
        //bool noCube = true;
        foreach (GameObject sm in dataSM) {
            Transform barChart = sm.transform.GetChild(0);
            if (barChart.Find("CubeSelection") != null)
            {
                //Debug.Log(barChart.name);
                //noCube = false;
                List<Vector2> pointsInBox = new List<Vector2>();

                int minCountry = (int)Mathf.Sqrt(barNum);
                int maxCountry = 1;
                int minYear = (int)Mathf.Sqrt(barNum);
                int maxYear = 1;
                float minValue = 1;
                float maxValue = 0;


                foreach (KeyValuePair<Vector2, Vector3> entry in chessBoardPoints[barChart.name])
                {
                    if (PointInBox(entry.Value, GameObject.Find(barChart.name).transform, barChart.Find("CubeSelection")))
                    {
                        pointsInBox.Add(entry.Key);
                        Vector3 adjustedPoint = entry.Value - GameObject.Find(barChart.name).transform.localPosition;
                        if (entry.Key.x < minCountry)
                            minCountry = (int)entry.Key.x;
                        if (entry.Key.x > maxCountry)
                            maxCountry = (int)entry.Key.x;
                        if (entry.Key.y < minYear)
                            minYear = (int)entry.Key.y;
                        if (entry.Key.y > maxYear)
                            maxYear = (int)entry.Key.y;
                        if (adjustedPoint.y < minValue)
                            minValue = adjustedPoint.y;
                        if (adjustedPoint.y > maxValue)
                            maxValue = adjustedPoint.y;
                    }
                }
                //Debug.Log(minCountry + " " + maxCountry + " " + minYear + " " + maxYear);
                for (int i = 0; i < (int)Mathf.Sqrt(barNum); i++)
                {
                    for (int j = 0; j < (int)Mathf.Sqrt(barNum); j++)
                    {
                        if (rangeSelectionChessBoardBrushingBool[i * (int)Mathf.Sqrt(barNum) + j]) {
                            if (pointsInBox.Contains(new Vector2(i + 1, j + 1)))
                            {
                                rangeSelectionChessBoardBrushingBool[i * (int)Mathf.Sqrt(barNum) + j] = true;
                            }
                            else
                            {
                                rangeSelectionChessBoardBrushingBool[i * (int)Mathf.Sqrt(barNum) + j] = false;
                            }
                        }
                        
                    }
                }

                if (minCountry > finalMinCountry)
                    finalMinCountry = minCountry;
                if (maxCountry < finalMaxCountry)
                    finalMaxCountry = maxCountry;
                if (minYear > finalMinYear)
                    finalMinYear = minYear;
                if (maxYear < finalMaxYear)
                    finalMaxYear = maxYear;
            }
        }

        triggerPressedForFilterMoving = true;

        SetFilterPosition(finalMinCountry - 1, finalMaxCountry, finalMinYear - 1, finalMaxYear);
        triggerPressedForFilterMoving = false;

        BarChartCreator bcc = GameObject.Find("BarChartManagement").GetComponent<BarChartCreator>();
        finalChessBoardBrushingBool = CalculateFullLogicOfBrushingAndFiltering();
        bcc.UpdateBrushing(finalChessBoardBrushingBool);     
    }

    private bool PointInBox(Vector3 point, Transform pointParent, Transform box) {
        float xMin = (int)Mathf.Sqrt(barNum);
        float xMax = 0;
        float yMin = (int)Mathf.Sqrt(barNum);
        float yMax = 0;
        float zMin = (int)Mathf.Sqrt(barNum);
        float zMax = 0;

        foreach (Vector3 v in box.transform.GetChild(0).GetComponent<MeshFilter>().mesh.vertices) {
            Vector3 chessBoardLocalVFromCubeVertices = pointParent.InverseTransformPoint(box.transform.GetChild(0).TransformPoint(v));
            

            if (chessBoardLocalVFromCubeVertices.x < xMin) {
                xMin = chessBoardLocalVFromCubeVertices.x;
            }

            if (chessBoardLocalVFromCubeVertices.x > xMax)
            {
                xMax = chessBoardLocalVFromCubeVertices.x;
            }

            if (chessBoardLocalVFromCubeVertices.y < yMin)
            {
                yMin = chessBoardLocalVFromCubeVertices.y;
            }

            if (chessBoardLocalVFromCubeVertices.y > yMax)
            {
                yMax = chessBoardLocalVFromCubeVertices.y;
            }

            if (chessBoardLocalVFromCubeVertices.z < zMin)
            {
                zMin = chessBoardLocalVFromCubeVertices.z;
            }

            if (chessBoardLocalVFromCubeVertices.z > zMax)
            {
                zMax = chessBoardLocalVFromCubeVertices.z;
            }
        }

        Transform yAxis = pointParent.GetChild(2).GetChild(1);
        Vector3 adjustedPoint = point - pointParent.localPosition;
        Vector3 adjustedPointY = pointParent.InverseTransformPoint(yAxis.TransformPoint(point));

        //Vector3 adjustedPoint = pointParent.InverseTransformPoint(pointParent.parent.TransformPoint(point));

        //Debug.Log(adjustedPoint + " " + xMin + " " + xMax + " " + yMin + " " + yMax + " " + zMin + " " + zMax);

        if (adjustedPoint.x < xMax && adjustedPoint.x > xMin && adjustedPointY.y < yMax && adjustedPointY.y > yMin && adjustedPoint.z < zMax && adjustedPoint.z > zMin)
        {
            //Debug.Log(adjustedPoint + " " + xMin + " " + xMax + " " + yMin + " " + yMax + " " + zMin + " " + zMax);
            return true;
        }
        else {
            return false;
        }
            
    }

    private void CalculateHoveringChessBoardBool(Vector4 input, string mode)
    {
        BarChartCreator bcc = GameObject.Find("BarChartManagement").GetComponent<BarChartCreator>();

        hoveringChessBoardBrushingBool = new bool[barNum];

        int leftCountryBrushControl = (int)input.x;
        int rightCountryBrushControl = (int)input.y;
        int leftYearBrushControl = (int)input.z;
        int rightYearBrushControl = (int)input.w;

        if (mode == "single")
        {
            if (leftYearBrushControl != 0 && leftCountryBrushControl != 0)
            {
                hoveringChessBoardBrushingBool[(int)Mathf.Sqrt(barNum) * (leftCountryBrushControl - 1) + (leftYearBrushControl - 1)] = true;
            }
            else if (leftYearBrushControl != 0 && leftCountryBrushControl == 0)
            {
                for (int i = 0; i < (int)Mathf.Sqrt(barNum); i++)
                {
                    hoveringChessBoardBrushingBool[i * (int)Mathf.Sqrt(barNum) + (leftYearBrushControl - 1)] = true;
                }
            }
            else if (leftYearBrushControl == 0 && leftCountryBrushControl != 0)
            {
                for (int i = 0; i < (int)Mathf.Sqrt(barNum); i++)
                {
                    hoveringChessBoardBrushingBool[(leftCountryBrushControl - 1) * (int)Mathf.Sqrt(barNum) + i] = true;
                }
            }

            if (rightYearBrushControl != 0 && rightCountryBrushControl != 0)
            {
                hoveringChessBoardBrushingBool[(int)Mathf.Sqrt(barNum) * (rightCountryBrushControl - 1) + (rightYearBrushControl - 1)] = true;
            }
            else if (rightYearBrushControl != 0 && rightCountryBrushControl == 0)
            {
                for (int i = 0; i < (int)Mathf.Sqrt(barNum); i++)
                {
                    hoveringChessBoardBrushingBool[i * (int)Mathf.Sqrt(barNum) + (rightYearBrushControl - 1)] = true;
                }
            }
            else if (rightYearBrushControl == 0 && rightCountryBrushControl != 0)
            {
                for (int i = 0; i < (int)Mathf.Sqrt(barNum); i++)
                {
                    hoveringChessBoardBrushingBool[(rightCountryBrushControl - 1) * (int)Mathf.Sqrt(barNum) + i] = true;
                }
            }

            // check all false
            bool allFalse = true;
            foreach (bool tmp in hoveringChessBoardBrushingBool)
            {
                if (tmp)
                    allFalse = false;
            }


            if (allFalse)
            {
                for (int i = 0; i < hoveringChessBoardBrushingBool.Length; i++)
                {
                    hoveringChessBoardBrushingBool[i] = true;
                }
            }
        }
        else if (mode == "axis")
        {
            // check intersection
            if (leftYearBrushControl == 0 && leftCountryBrushControl != 0 && rightYearBrushControl != 0 && rightCountryBrushControl == 0)
            {
                hoveringChessBoardBrushingBool[(int)Mathf.Sqrt(barNum) * (leftCountryBrushControl - 1) + (rightYearBrushControl - 1)] = true;
            }
            else if (leftYearBrushControl != 0 && leftCountryBrushControl == 0 && rightYearBrushControl == 0 && rightCountryBrushControl != 0)
            {
                hoveringChessBoardBrushingBool[(int)Mathf.Sqrt(barNum) * (rightCountryBrushControl - 1) + (leftYearBrushControl - 1)] = true;
            }
            else if (leftYearBrushControl == 0 && leftCountryBrushControl != 0 && rightYearBrushControl == 0 && rightCountryBrushControl != 0) // check parallel
            {
                if (Math.Abs(leftCountryBrushControl - rightCountryBrushControl) != 0)
                {
                    for (int i = Math.Min(leftCountryBrushControl, rightCountryBrushControl); i <= Math.Max(leftCountryBrushControl, rightCountryBrushControl); i++)
                    {
                        for (int j = 0; j < (int)Mathf.Sqrt(barNum); j++)
                        {
                            hoveringChessBoardBrushingBool[(i - 1) * (int)Mathf.Sqrt(barNum) + j] = true;
                        }
                    }
                }
                else
                {
                    for (int i = 1; i <= (int)Mathf.Sqrt(barNum); i++)
                    {
                        for (int j = 1; j <= (int)Mathf.Sqrt(barNum); j++)
                        {
                            if (j == leftCountryBrushControl)
                                hoveringChessBoardBrushingBool[(j - 1) * (int)Mathf.Sqrt(barNum) + (i - 1)] = true;
                        }
                    }
                }
            }
            else if (leftYearBrushControl != 0 && leftCountryBrushControl == 0 && rightYearBrushControl != 0 && rightCountryBrushControl == 0) // check parallel
            {
                if (Math.Abs(leftYearBrushControl - rightYearBrushControl) != 0)
                {
                    for (int i = Math.Min(leftYearBrushControl, rightYearBrushControl); i <= Math.Max(leftYearBrushControl, rightYearBrushControl); i++)
                    {
                        for (int j = 0; j < (int)Mathf.Sqrt(barNum); j++)
                        {
                            hoveringChessBoardBrushingBool[j * (int)Mathf.Sqrt(barNum) + (i - 1)] = true;
                        }
                    }
                }
                else
                {
                    for (int i = 1; i <= (int)Mathf.Sqrt(barNum); i++)
                    {
                        for (int j = 1; j <= (int)Mathf.Sqrt(barNum); j++)
                        {
                            if (i == leftYearBrushControl)
                                hoveringChessBoardBrushingBool[(j - 1) * (int)Mathf.Sqrt(barNum) + (i - 1)] = true;
                        }
                    }
                }
            }
            else
            {
                if (rightCountryBrushControl == 0 && leftCountryBrushControl != 0)
                {
                    for (int i = 1; i <= (int)Mathf.Sqrt(barNum); i++)
                    {
                        for (int j = 1; j <= (int)Mathf.Sqrt(barNum); j++)
                        {
                            if (i == leftCountryBrushControl)
                                hoveringChessBoardBrushingBool[(i - 1) * (int)Mathf.Sqrt(barNum) + (j - 1)] = true;
                        }
                    }
                }
                else if (leftCountryBrushControl == 0 && rightCountryBrushControl != 0)
                {
                    for (int i = 1; i <= (int)Mathf.Sqrt(barNum); i++)
                    {
                        for (int j = 1; j <= (int)Mathf.Sqrt(barNum); j++)
                        {
                            if (i == rightCountryBrushControl)
                                hoveringChessBoardBrushingBool[(i - 1) * (int)Mathf.Sqrt(barNum) + (j - 1)] = true;
                        }
                    }
                }

                if (rightYearBrushControl == 0 && leftYearBrushControl != 0)
                {
                    for (int i = 1; i <= (int)Mathf.Sqrt(barNum); i++)
                    {
                        for (int j = 1; j <= (int)Mathf.Sqrt(barNum); j++)
                        {
                            if (j == leftYearBrushControl)
                                hoveringChessBoardBrushingBool[(i - 1) * (int)Mathf.Sqrt(barNum) + (j - 1)] = true;
                        }
                    }
                }
                else if (rightYearBrushControl != 0 && leftYearBrushControl == 0)
                {
                    for (int i = 1; i <= (int)Mathf.Sqrt(barNum); i++)
                    {
                        for (int j = 1; j <= (int)Mathf.Sqrt(barNum); j++)
                        {
                            if (j == rightYearBrushControl)
                                hoveringChessBoardBrushingBool[(i - 1) * (int)Mathf.Sqrt(barNum) + (j - 1)] = true;
                        }
                    }
                }
            }
        }
        bcc.UpdateBrushing(hoveringChessBoardBrushingBool);
    }

    private void SetFilterPosition(int leftCountry, int rightCountry, int leftYear, int rightYear)
    {
        if (triggerPressedForFilterMoving)
        {
            if (leftCountry >= 0)
            {
                foreach (GameObject filter in leftCountryFilters)
                {
                    filter.transform.localPosition = new Vector3(-3, leftCountry / Mathf.Sqrt(barNum), 0);
                }
                currentCountryLeftFilterPosition = leftCountry;
            }

            if (leftYear >= 0)
            {
                foreach (GameObject filter in leftYearFilters)
                {
                    filter.transform.localPosition = new Vector3(-3, leftYear / Mathf.Sqrt(barNum), 0);
                }
                currentYearLeftFilterPosition = leftYear;
            }

            if (rightCountry >= 0)
            {
                foreach (GameObject filter in rightCountryFilters)
                {
                    filter.transform.localPosition = new Vector3(-3, rightCountry / Mathf.Sqrt(barNum), 0);
                }
                currentCountryRightFilterPosition = rightCountry;
            }

            if (rightYear >= 0)
            {
                foreach (GameObject filter in rightYearFilters)
                {
                    filter.transform.localPosition = new Vector3(-3, rightYear / Mathf.Sqrt(barNum), 0);
                }
                currentYearRightFilterPosition = rightYear;
            }
        }

    }

    //shelf movement control
    private void ShelfMovement(Transform cubeTransform)
    {
        cameraForward = Camera.main.transform.localPosition + Camera.main.transform.forward * 2f;
        cameraForward = new Vector3(cameraForward.x, ExperimentManager.userHeight, cameraForward.z);
        Transform SMManagerT = transform;
        Vector3 newWorldInMiniturePosition = cubeTransform.position;

        if (dataset == 1 || dataset == 2)
        {
            if (!controllerShelfDeltaSetup)
            {
                controllerShelfDeltaSetup = true;
                controllerShelfDelta = SMManagerT.position - cubeTransform.position;
            }

            Vector3 newPosition = controllerShelfDelta + cubeTransform.position;
            SMManagerT.position = new Vector3(newPosition.x, newPosition.y, SMManagerT.position.z);

            if (SMManagerT.position.y < -0.6f * SMManagerT.localScale.y)
            {
                SMManagerT.position = new Vector3(SMManagerT.position.x, -0.6f * SMManagerT.localScale.y, SMManagerT.position.z);
            }
            else if (SMManagerT.position.y > (ExperimentManager.userHeight - 0.6f) * SMManagerT.localScale.y)
            {
                SMManagerT.position = new Vector3(SMManagerT.position.x, (ExperimentManager.userHeight - 0.6f) * SMManagerT.localScale.y, SMManagerT.position.z);
            }


            if (SMManagerT.position.x > 1.5f)
            {
                SMManagerT.position = new Vector3(1.5f, SMManagerT.position.y, SMManagerT.position.z);
            }
            else if (SMManagerT.position.x < -1.5f)
            {
                SMManagerT.position = new Vector3(-1.5f, SMManagerT.position.y, SMManagerT.position.z);
            }
        }

        oldWorldInMiniturePosition = newWorldInMiniturePosition;
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
        else {
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

    void GetTasks() {
        string[] lines = new string[taskNo];

        if (!circleLayout) // F
        {
            lines = FlatQsFile.text.Split(lineSeperater);
        }
        else
        {
            if (fullCircle) // C
            {
                lines = FullCircleQsFile.text.Split(lineSeperater);
            }
            else  // H
            { 
                lines = HalfCircleQsFile.text.Split(lineSeperater);
            }
        }

        int startPosition = 0;
        if (ExperimentManager.sceneCounter > 2) {
            startPosition = 5;
        }

        for (int i = 0; i < 5; i++) {
            taskArray[i] = lines[startPosition + i].Split('|')[0];
            highlightedSM[i] = new Vector2(int.Parse(lines[startPosition + i].Split('|')[1].Split(',')[0]), int.Parse(lines[startPosition + i].Split('|')[1].Split(',')[1]));
            QuestionIDs[i] = lines[startPosition + i].Split('|')[3];
            CorrectAnswers[i] = lines[startPosition + i].Split('|')[4];
        }
    }

    private void GetShuffledOrders()
    {
        string[] lines = new string[30];
        lines = ShuffledOrderFile.text.Split(lineSeperater);

        for (int i = 0; i < 30; i++)
        {
            shuffledOrders.Add(i + 1, lines[i].Split(',').Select(n => int.Parse(n)).ToArray());
        }

    }

    private void ShuffleSMOrder() {

        for (int i = 0; i < smallMultiplesNumber; i++) {
            GameObject.Find("BarCharts-" + i).transform.SetParent(GameObject.Find("Small Multiples " + shuffledOrders[int.Parse(QuestionIDs[sceneCounter])][i]).transform);
            GameObject.Find("BarCharts-" + i).transform.localPosition = new Vector3(-0.5f, 0.3f, -0.5f);
            GameObject.Find("BarCharts-" + i).transform.localEulerAngles = Vector3.zero;
        }
    }


    private string GetCorrectAnswer(int index)
    {
        string finalString = "";

        if (trainingCounting == 1)
        {
            if (index > 0)
            {
                if (taskNo == 6)
                {
                    finalString = CorrectAnswers[index + 2].Replace("\n", String.Empty);
                }
                else if (taskNo == 4)
                    finalString = CorrectAnswers[index + 1].Replace("\n", String.Empty);
            }
            else
            {
                finalString = CorrectAnswers[index].Replace("\n", String.Empty);
            }
        }
        else
        {
            finalString = CorrectAnswers[index].Replace("\n", String.Empty);
        }

        return finalString;
    }

    private string GetQuestionID(int index)
    {
        string finalString = "";
        //Debug.Log(index + " " + QuestionIDs.Length);
        if (trainingCounting == 1)
        {
            if (index > 0)
            {
                if (taskNo == 6)
                {
                    finalString = QuestionIDs[index + 2];
                }
                else if (taskNo == 4)
                    finalString = QuestionIDs[index + 1];
            }
            else
            {
                finalString = QuestionIDs[index];
            }
        }
        else
        {
            //Debug.Log(index + " " + QuestionIDs.Length);
            finalString = QuestionIDs[index];
        }

        return finalString;
    }

    private string TaskIDToFullTaskID(int taskID) {
        if (taskID % 5 == 1 || taskID % 5 == 2)
        {
            return "Training";
        }
        else if (taskID % 5 == 0)
        {
            return taskID / 5 * 3 + "";
        }
        else
        {
            return (taskID / 5 * 3 + taskID % 5 - 2) + "";
        }
    }

    void SetLayer(int newLayer, Transform trans)
    {
        trans.gameObject.layer = newLayer;
        foreach (Transform child in trans)
        {
            child.gameObject.layer = newLayer;
            if (child.childCount > 0)
            {
                SetLayer(newLayer, child.transform);
            }
        }
    }

    // static functions

    private void FlyingFunction()
    {
        if (trialState == TrialState.Answer)
        {
            if (globalLeftController != null && globalRightController != null)
            {
                if (globalLeftController.GetComponent<SteamVR_TrackedController>().triggerPressed && answerConfirmed)
                {
                    flyingFlag = true;
                    if (flyingFlag)
                    {
                        StartCoroutine(PerformFlying("left"));
                        flyingFlag = false;
                    }

                }
                else if (globalRightController.GetComponent<SteamVR_TrackedController>().triggerPressed && answerConfirmed)
                {
                    flyingFlag = true;
                    if (flyingFlag)
                    {
                        StartCoroutine(PerformFlying("right"));
                        flyingFlag = false;
                    }
                }
            }
        }
        else
        {
            if (globalLeftController != null && globalRightController != null)
            {
                //StartCoroutine(PerformFlying("left"));
                if (globalLeftController.GetComponent<SteamVR_TrackedController>().padPressed)
                {
                    flyingFlag = true;
                    if (flyingFlag)
                    {
                        StartCoroutine(PerformFlying("left"));
                        flyingFlag = false;
                    }

                }
                else if (globalRightController.GetComponent<SteamVR_TrackedController>().padPressed)
                {
                    flyingFlag = true;
                    if (flyingFlag)
                    {
                        StartCoroutine(PerformFlying("right"));
                        flyingFlag = false;
                    }
                }
            }
        }
    }

    IEnumerator PerformFlying(string controller)
    {
        float i = 0;
        float rate = 1f / 1f;

        while (i < 1)
        {
            for (int j = 0; j < 3; j++)
            {
                Vector3 oldPosition = taskBoardPositions[j];
                if (controller == "left")
                {
                    taskBoards[j].transform.position = Vector3.Lerp(globalLeftController.position, oldPosition, Mathf.SmoothStep(0.0f, 1.0f, i));
                }
                else
                {
                    taskBoards[j].transform.position = Vector3.Lerp(globalRightController.position, oldPosition, Mathf.SmoothStep(0.0f, 1.0f, i));
                }
                taskBoards[j].transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 0.01f, Mathf.SmoothStep(0.0f, 1.0f, i));
            }
            i += Time.deltaTime * rate;
            yield return 0;
        }
    }

    public void ChangeTaskText(string taskText, int taskID) {

        foreach (GameObject go in taskBoards) {
            Text t = go.transform.Find("UITextFront").GetComponent<Text>();
            if (taskID > 0)
            {
                if (taskID == 1 || taskID == 2)
                {
                     t.text = "Training Task " + taskID + ": \n" + taskText;            
                }
                else {
                    t.text = "Task " + (taskID - 2) + ":\n" + taskText;
                }
            }
            else {
                t.text = taskText;
                t.fontSize = 10;
            }
        }
    }

	public static string ToOrdinal(int value)
	{
		// Start with the most common extension.
		string extension = "th";

		// Examine the last 2 digits.
		int last_digits = value % 100;

		// If the last digits are 11, 12, or 13, use th. Otherwise:
		if (last_digits < 11 || last_digits > 13)
		{
			// Check the last digit.
			switch (last_digits % 10)
			{
			case 1:
				extension = "st";
				break;
			case 2:
				extension = "nd";
				break;
			case 3:
				extension = "rd";
				break;
			}
		}

		return extension;
	}

    private void SetupAnswerPanelYearList(int questionID) {
        string[] answerArray = new string[5];
        switch (questionID) {
            case 16:
                answerArray = new string[] { "2000", "2001", "2002", "2003", "2004" };
                break;
            case 17:
                answerArray = new string[] { "2005", "2006", "2007", "2008", "2009" };
                break;
            case 18:
                answerArray = new string[] { "2010", "2011", "2012", "2013", "2014" };
                break;
            case 19:
                answerArray = new string[] { "1985", "1986", "1987", "1988", "1989" };
                break;
            case 20:
                answerArray = new string[] { "1995", "1996", "1997", "1998", "1999" };
                break;
            case 21:
                answerArray = new string[] { "2010", "2011", "2012", "2013", "2014" };
                break;
            case 22:
                answerArray = new string[] { "2011", "2012", "2013", "2014", "2015" };
                break;
            case 23:
                answerArray = new string[] { "1980", "1981", "1982", "1983", "1984" };
                break;
            case 24:
                answerArray = new string[] { "1980", "1981", "1982", "1983", "1984" };
                break;
            case 25:
                answerArray = new string[] { "2000", "2001", "2002", "2003", "2004" };
                break;
            case 26:
                answerArray = new string[] { "1990", "1991", "1992", "1993", "1994" };
                break;
            case 27:
                answerArray = new string[] { "1985", "1986", "1987", "1988", "1989" };
                break;
            case 28:
                answerArray = new string[] { "2010", "2011", "2012", "2013", "2014" };
                break;
            case 29:
                answerArray = new string[] { "1995", "1996", "1997", "1998", "1999" };
                break;
            case 30:
                answerArray = new string[] { "2005", "2006", "2007", "2008", "2009" };
                break;
            default:
                break;
        }
        Transform gridChoices = GameObject.Find("PanelMenuForYears").transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1);
        for (int i = 0; i < 5; i++) {
            gridChoices.GetChild(i).GetChild(0).GetComponent<Text>().text = answerArray[i];
        }
    }

	public List<GameObject> GetSMList(){
		return this.dataSM;
	}

    public int GetRows() {
        return shelfRows;
    }

    public int GetItemPerRow() {
        return shelfItemPerRow;
    }

    //Two non-parallel lines which may or may not touch each other have a point on each line which are closest
    //to each other. This function finds those two points. If the lines are not parallel, the function 
    //outputs true, otherwise false.
    public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {

        closestPointLine1 = Vector3.zero;
        closestPointLine2 = Vector3.zero;

        float a = Vector3.Dot(lineVec1, lineVec1);
        float b = Vector3.Dot(lineVec1, lineVec2);
        float e = Vector3.Dot(lineVec2, lineVec2);

        float d = a * e - b * b;

        //lines are not parallel
        if (d != 0.0f)
        {

            Vector3 r = linePoint1 - linePoint2;
            float c = Vector3.Dot(lineVec1, r);
            float f = Vector3.Dot(lineVec2, r);

            float s = (b * f - c * e) / d;
            float t = (a * f - c * b) / d;

            closestPointLine1 = linePoint1 + lineVec1 * s;
            closestPointLine2 = linePoint2 + lineVec2 * t;

            return true;
        }

        else
        {
            return false;
        }
    }
}