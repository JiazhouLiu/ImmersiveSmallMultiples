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

public enum DateType{
	Weekly,     //6: 9sec; 12: 12sec; 26: 22sec; 30: 28sec; 32: 26sec; 34: 27sec ; 35: 28sec
	Fornightly, //6: 9sec; 12: 12sec; 26: 22sec
    Monthly     //6: 9sec; 12: 12sec
}

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
	public GameObject colorSchemePrefab;
    public GameObject TrajectoriesPrefab;
    public Material silhouetteShader;
    public Light haloLightPrefab;
    public TextAsset ACFile;
    public TextAsset BIMCurQsFile;
    public TextAsset BIMFltQsFile;
    public TextAsset BIMQCurQsFile;
    public TextAsset BarCurQsFile;
    public TextAsset BarFltQsFile;
    public TextAsset BarQCurQsFile;
    public TextAsset BIMCurQsTFile;
    public TextAsset BIMFltQsTFile;
    public TextAsset BIMQCurQsTFile;
    public TextAsset BarCurQsTFile;
    public TextAsset BarFltQsTFile;
    public TextAsset BarQCurQsTFile;
    public GameObject TextHolderPrefab;
    public GameObject FilterPrefab;
    public GameObject CuttingPlanePrefab;
    public GameObject cubeSelectionPrefab;
    public GameObject worldInMiniturePrefab;
    public GameObject LRLabelPrefab;

    [HideInInspector]
    public bool fixedPosition = false;
    public bool circleLayout = false;
    public bool fullCircle = false;

    public int dataset = 1;
    [Header("Experiment")]
    public int smallMultiplesNumber;
    public DateType dateType;
    

    // sensors Info List to store sensor information
    public static Dictionary<string, HashSet<SensorReading>> sensorsInfoList;

    // Small multiples game object list
	List<GameObject> dataSM;

    // shelf variables
    GameObject shelf;
    List<GameObject> taskBoards;
	GameObject colorScheme;

    // controller
    GameObject leftController;
    GameObject rightController;

	int smLimit = 0;
    int shelfRows = 3;
    int shelfItemPerRow = 0;

    // shelf variables
    float baseVPosition = 0.8f;
    float delta = 0.7f; // 0.65f
    float vDelta = 0.7f;
    float d1VerticalDiff = 0.5f;
    float d2Scale = 0.3f;
    // end shelf variables

    // string variables
    private char lineSeperater = '\n'; // It defines line seperate character
    private char fieldSeperator = ','; // It defines field seperate chracter

    string[] tempTagList;

    // task related
    [HideInInspector]
    public int taskID = 0;
    int taskNo = 4;
    string fullTaskID = "0";
    [HideInInspector]
    public string[] taskArray;
    [HideInInspector]
    public string[] trainingTaskArray;
    bool canChange = false;
    bool changed = false;
    StreamWriter writer;
    StreamWriter writerEye;
    StreamWriter writerHead;

    [HideInInspector]
    public bool startTask = false;
    // end task related

    // log related
    bool clockRotation = false;
    bool antiClockRotation = false;
    bool scaleUp = false;
    bool scaleDown = false;
      // tracking trials
    int deltaTrialNumber = 0;
    // end log related

    // shelf auto-adjustable movement
    float userHeight = 1.3f;
    bool finishInitialised = false;
    Vector3 oldSMPosition = Vector3.zero;

    // brushing for barchart
    Dictionary<string, Dictionary<Vector2, Vector3>> chessBoardPoints;

    bool[] chessBoardBrushingBool = new bool[100]; // single brushing and axis brushing
    bool[] hoveringChessBoardBrushingBool = new bool[100]; // hovering effect
    bool[] filteredChessBoardBrushingBool = new bool[100]; // y-axis filtering
    bool[] rangeSelectionChessBoardBrushingBool = new bool[100];
    bool[] hoveringRangeSelectionChessBoardBrushingBool = new bool[100];

    bool[] finalChessBoardBrushingBool = new bool[100]; // final highlight
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
    int currentCountryRightFilterPosition = 10;

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
    int currentYearRightFilterPosition = 10;

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

    // brushing for BIM
    [HideInInspector]
    public bool leftFindHighlighedSensorFromCollision = false;
    [HideInInspector]
    public bool rightFindHighlighedSensorFromCollision = false;
    GameObject leftFindHighlightedGO;
    GameObject rightFindHighlightedGO;
    GameObject leftKeepHighlightedGO;
    GameObject rightKeepHighlightedGO;
    [HideInInspector]
    public bool colorFilterOn = false;
    bool colorFilterSelected = false;
    [HideInInspector]
    public bool colorFilterActive = false;

    List<GameObject> allSensors;
    List<GameObject> finalHighlightedSensors;
    List<GameObject> highlightedSensorsFromMovingFilters;
    List<GameObject> maxZFilters;
    List<GameObject> minZFilters;
    List<GameObject> maxXFilters;
    List<GameObject> minXFilters;
    List<GameObject> maxYFilters;
    List<GameObject> minYFilters;
    float currentXMinFilterPosition = 0;
    float currentXMaxFilterPosition = 1;
    float currentZMinFilterPosition = 0;
    float currentZMaxFilterPosition = 1;
    int currentYMinFilterPosition = 0;
    int currentYMaxFilterPosition = 2;
    [HideInInspector]
    public Vector2 rightFindHoveringV2FromCollisionBIM = new Vector2(-1, -1);
    [HideInInspector]
    public Vector2 leftFindHoveringV2FromCollisionBIM = new Vector2(-1, -1);
    private Vector2 leftFindHighlighedV2FromCollisionBIM = new Vector2(-1, -1);
    private Vector2 rightFindHighlighedV2FromCollisionBIM = new Vector2(-1, -1);

    // Vector3 for rotation
    Vector3 oldV3FromLeftBtoRightB = Vector3.zero;
    float rotationDelta = 0;

    // Shelf movement
    bool controllerShelfDeltaSetup = false;
    Vector3 controllerShelfDelta = Vector3.zero;
    Vector3 oldEulerAngle = Vector3.zero;
    Vector3 oldWorldInMiniturePosition = Vector3.zero;
    Vector3 cameraForward = Vector3.zero;

    bool lastFaceToCenter = false;

    // Experiment related
    float completionTime = 60f;
    bool trainingForCombinition = true;
    bool forceStopedTFCFromManager = false;
    bool newVisTraining = false;
    [HideInInspector]
    public TrialState trialState = TrialState.PreTask;
    [HideInInspector]
    public string selectedAnswer = "";
    int trainingCounting = 0;
    int trainingCountringLeft = 0;
    [HideInInspector]
    public bool interactionTrainingNeeded = false;
    //[HideInInspector]
    public int interactionTrainingCount = 8;
    string[] interactionTrainingDesc;
    bool flyingFlag = false;
    Vector3[] taskBoardPositions = new Vector3[3];
    [HideInInspector]
    public bool confirmButtonPressed = false;
    bool answerConfirmed = false;
    bool trainingQuestionReminderOn = false;
    float SMRotationDiff = 0;
    bool answerLogged = false;
    int answerLogIncrement = 0;
    string[] QuestionIDs;
    string[] CorrectAnswers;

    //color filter for BIM
    bool colorSelected = false;
    int colorSelectedIndex = 0;

    // unclick to refresh
    bool leftClickEmptySpace = false;
    bool rightClickEmptySpace = false;

    // label issue
    List<GameObject> smToolTips;

    // highlight sm
    Vector2[] highlightedSM;
    Vector2[] highlightedTrainingSM;
    Vector2 currentHighlightedSM = Vector2.zero;

    // eyetracking
    public bool calibrationFlag = false;
    GameObject mainCamera;
    public bool afterCalibration = true;
    Vector2 rawGazePositionOnScreen = Vector2.zero;

    // pilot configuration
    //float BIMScaleDelta = 0.606f;
    //float BIMPositionYDelta = -0.27f;
    //float BarScaleDelta = 0.59f;
    //float BarPositionYDelta = -0.38f;

    // Use this for initialization
    void Start () {
        QualitySettings.vSyncCount = 1;
        userHeight = ExperimentManager.userHeight;

        interactionTrainingCount = 8;
        answerLogIncrement = 0;

        switch (ExperimentManager.ExperimentSequence)
        {
            case 1:
                if (dataset == 2 && !circleLayout)
                    newVisTraining = true;
                break;
            case 2:
                if (dataset == 1 && !circleLayout)
                    newVisTraining = true;
                break;
            case 3:
                if (dataset == 2 && !circleLayout)
                    newVisTraining = true;
                break;
            case 4:
                if (dataset == 1 && !circleLayout)
                    newVisTraining = true;
                break;
            case 5:
                if (dataset == 2 && circleLayout && fullCircle)
                    newVisTraining = true;
                break;
            case 6:
                if (dataset == 1 && circleLayout && fullCircle)
                    newVisTraining = true;
                break;
            case 7:
                if (dataset == 2 && circleLayout && fullCircle)
                    newVisTraining = true;
                break;
            case 8:
                if (dataset == 1 && circleLayout && fullCircle)
                    newVisTraining = true;
                break;
            case 9:
                if (dataset == 2 && circleLayout && !fullCircle)
                    newVisTraining = true;
                break;
            case 10:
                if (dataset == 1 && circleLayout && !fullCircle)
                    newVisTraining = true;
                break;
            case 11:
                if (dataset == 2 && circleLayout && !fullCircle)
                    newVisTraining = true;
                break;
            case 12:
                if (dataset == 1 && circleLayout && !fullCircle)
                    newVisTraining = true;
                break;
            default:
                break;
        }

        //Debug.Log(ExperimentManager.userHeight);
        //userHeight = 1.7f;
        //GameObject testingBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //testingBall.transform.position = new Vector3(0.5f, userHeight, 0.9f);
        //testingBall.transform.localScale = Vector3.one * 0.05f;

        forceStopedTFCFromManager = ExperimentManager.forceStopTrainingForCombinition;
        ExperimentManager.forceStopTrainingForCombinition = false;

        if (dataSM == null) {

            if (dataset == 1)
            {
                if (dateType == DateType.Monthly)
                {
                    smLimit = 12;
                }
                else if (dateType == DateType.Fornightly)
                {
                    smLimit = 26;
                }
                else
                {
                    smLimit = 52;
                }

                //transform.localScale = userHeight * BIMScaleDelta * Vector3.one;
                //transform.localPosition = new Vector3(transform.localPosition.x, BIMPositionYDelta * userHeight, transform.localPosition.z);

            }
            else {
                smLimit = smallMultiplesNumber;
                if (dataset == 2)
                {
                    //transform.localScale = userHeight * BarScaleDelta * Vector3.one;
                    //transform.localPosition = new Vector3(transform.localPosition.x, BarPositionYDelta *userHeight, transform.localPosition.z);

                }
            }
            transform.localScale = 0.6f * Vector3.one;
            transform.localPosition = new Vector3(transform.localPosition.x, 0.62f / 1.69f * userHeight, transform.localPosition.z);

            if (smallMultiplesNumber < 1)
			{
				Debug.Log("Please enter a valid small multiples number");
			}
			else if (smallMultiplesNumber > smLimit)
			{
				Debug.Log("More than " +  smLimit + " small multiples are not allowed in this simulation.");
			}
			else
			{
                ResetManagerPosition();
                
                chessBoardPoints = new Dictionary<string, Dictionary<Vector2, Vector3>>();
                sensorsInfoList = new Dictionary<string, HashSet<SensorReading>>();
				dataSM = new List<GameObject>();

                taskBoards = new List<GameObject>();
                taskArray = new string[taskNo];
                highlightedSM = new Vector2[taskNo];
                trainingTaskArray = new string[taskNo / 2];
                highlightedTrainingSM = new Vector2[taskNo / 2];

                rightYearFilters = new List<GameObject>();
                leftYearFilters = new List<GameObject>();
                rightCountryFilters = new List<GameObject>();
                leftCountryFilters = new List<GameObject>();
                rightValueFilters = new List<GameObject>();
                leftValueFilters = new List<GameObject>();
                rightValuePlanes = new List<GameObject>();
                leftValuePlanes = new List<GameObject>();

                maxZFilters = new List<GameObject>();
                minZFilters = new List<GameObject>();
                maxXFilters = new List<GameObject>();
                minXFilters = new List<GameObject>();
                maxYFilters = new List<GameObject>();
                minYFilters = new List<GameObject>();

                smToolTips = new List<GameObject>();

                interactionTrainingDesc = new string[8] {"Task Training: To rotate the small multiples, press and hold both <color=red>trigger</color> buttons together and then move the controller probes around the Y axis in the space.\n\n" +
                    "Press <color=green>Next</color> button on the controller to the next tutorial",
                    "Task Training: To show the color filter panel, you can hold the left controller vertically during the task. Then use the right controller probe to touch one block to see the highlight of a specific color. Press the right <color=red>trigger</color> button once on the block to keep this filter. Or press the right <color=red>trigger</color> button once in an empty space or the Reset button on the panel to reset the filter. To hide the panel, release your left hand horizontally.\n\n" +
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

                highlightedSensorsFromMovingFilters = new List<GameObject>();
                finalHighlightedSensors = new List<GameObject>();
                allSensors = new List<GameObject>();

                tempTagList = new string[smallMultiplesNumber];

                shelf = new GameObject("Shelf");
				shelf.transform.SetParent(this.transform);
				shelf.transform.localPosition = Vector3.zero;
                shelf.transform.localScale = Vector3.one;
                shelf.transform.eulerAngles = new Vector3(0, 0, 0);

                colorScheme = (GameObject)Instantiate(colorSchemePrefab, new Vector3(0, 0, 0), Quaternion.identity);
                colorScheme.transform.SetParent(shelf.transform);
                colorScheme.transform.localPosition = new Vector3(0, 0.74f, -delta / 2);
                colorScheme.transform.localScale = new Vector3(2, 2, 2);
                colorScheme.name = "Color Scheme";

                if (dataset == 2)
                {
                    colorScheme.SetActive(false);
                }
                else if(dataset == 1)
                {
                    colorScheme.transform.GetChild(1).Find("UITextFront").GetChild(1).gameObject.SetActive(false);
                    colorScheme.transform.GetChild(1).Find("UITextReverse").GetChild(1).gameObject.SetActive(false);
                }


                // calculate curvature delta

                if (dataset == 1)
                {
                    // read ac file
                    ReadACFile();
                }
                else if (dataset == 2)
                {
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
                    
                }

                // add task boards to list
                taskBoards.Add(GameObject.Find("TaskBoardLeft"));
                taskBoardPositions[0] = GameObject.Find("TaskBoardLeft").transform.position;
                taskBoards.Add(GameObject.Find("TaskBoardRight"));
                taskBoardPositions[1] = GameObject.Find("TaskBoardRight").transform.position;
                taskBoards.Add(GameObject.Find("TaskBoardTop"));
                taskBoardPositions[2] = GameObject.Find("TaskBoardTop").transform.position;

                QuestionIDs = new string[taskNo + taskNo / 2];
                CorrectAnswers = new string[taskNo + taskNo / 2];

                GetTrainingTasks();
                GetTasks();
                
                
                string path = ExperimentManager.writerFilePath;

                writer = new StreamWriter(path, true);

                string writerEyeFilePath = "Assets/ExperimentData/ExperimentLog/Participant " + ExperimentManager.ParticipantID + "/Participant_" + ExperimentManager.ParticipantID + "_EyeTrackingLog.csv";
                writerEye = new StreamWriter(writerEyeFilePath, true);

                string writerHeadFilePath = "Assets/ExperimentData/ExperimentLog/Participant " + ExperimentManager.ParticipantID + "/Participant_" + ExperimentManager.ParticipantID + "_HeadPositionLog.csv";
                writerHead = new StreamWriter(writerHeadFilePath, true);

                if (smallMultiplesNumber % shelfRows == 0)
                {
                    shelfItemPerRow = smallMultiplesNumber / shelfRows;
                }
                else
                {
                    shelfItemPerRow = (smallMultiplesNumber + shelfRows - smallMultiplesNumber % shelfRows) / shelfRows;
                }

                // create small multiples
                CreateSM();

                if (ExperimentManager.PublicTrialNumber != 0 && ExperimentManager.PublicTrialNumber % taskNo != 1) {
                    if (ExperimentManager.PublicTrialNumber % taskNo == 0)
                    {
                        taskID = taskNo - 1;
                    }
                    else {
                        taskID = ExperimentManager.PublicTrialNumber % taskNo - 1;
                    }
                   
                    deltaTrialNumber = taskID;
                    ExperimentManager.PublicTrialNumber = 0;
                }

                if (forceStopedTFCFromManager) {
                    trainingCounting = 0;
                    trainingCountringLeft = 0;
                }
                else if (ExperimentManager.comprehensiveTraining)
                {
                    trainingCounting = taskNo / 2;
                    trainingCountringLeft = taskNo / 2;
                    interactionTrainingNeeded = true;
                }
                else if (newVisTraining)
                {
                    trainingCounting = taskNo / 2;
                    trainingCountringLeft = taskNo / 2;
                    interactionTrainingNeeded = true;
                }
                else if(trainingForCombinition){
                    trainingCounting = 1;
                    trainingCountringLeft = 1;
                }

                SetupPreTaskEnvironment("none");

            }
        }
    }

    void Update(){
        if (smallMultiplesNumber <= smLimit && smallMultiplesNumber >= 1)
        {
            GetGazeInfo();
            FixedPositionCondition();

            if (writer != null) {
                if (writer.BaseStream != null)
                {
                    WritingLog();
                }
            }

            if (dataset == 2) {
                DetectBarChartInteraction();
            } else if (dataset == 1) {
                if (!colorFilterOn && !colorFilterSelected) {
                    DetectBIMInteraction();
                }
                DetectControllerVertical();
            }
            CreateRangeBrushingBox();

            FlyingFunction();
        }

        GetTrialNumber();

        if (Input.GetKeyUp(KeyCode.C)) {
            OpenPupilCamera();
        }
    }

    private void FlyingFunction() {
        if (trialState == TrialState.Answer)
        {
            if (GameObject.Find("Controller (left)") != null && GameObject.Find("Controller (right)") != null)
            {
                if (GameObject.Find("Controller (left)").GetComponent<SteamVR_TrackedController>().triggerPressed && answerConfirmed)
                {
                    flyingFlag = true;
                    if (flyingFlag)
                    {
                        StartCoroutine(PerformFlying("left"));
                        flyingFlag = false;
                    }

                }
                else if (GameObject.Find("Controller (right)").GetComponent<SteamVR_TrackedController>().triggerPressed && answerConfirmed)
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
            if (GameObject.Find("Controller (left)") != null && GameObject.Find("Controller (right)") != null)
            {
                //StartCoroutine(PerformFlying("left"));
                if (GameObject.Find("Controller (left)").GetComponent<SteamVR_TrackedController>().padPressed)
                {
                    flyingFlag = true;
                    if (flyingFlag)
                    {
                        StartCoroutine(PerformFlying("left"));
                        flyingFlag = false;
                    }

                }
                else if (GameObject.Find("Controller (right)").GetComponent<SteamVR_TrackedController>().padPressed)
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

    IEnumerator PerformFlying(string controller) {
        float i = 0;
        float rate = 1f / 1f;

        while (i < 1) {
            for (int j = 0; j < 3; j++) {
                Vector3 oldPosition = taskBoardPositions[j];
                if (controller == "left")
                {
                    taskBoards[j].transform.position = Vector3.Lerp(GameObject.Find("Controller (left)").transform.position, oldPosition, Mathf.SmoothStep(0.0f, 1.0f, i));  
                }
                else
                {
                    taskBoards[j].transform.position = Vector3.Lerp(GameObject.Find("Controller (right)").transform.position, oldPosition, Mathf.SmoothStep(0.0f, 1.0f, i));
                }
                taskBoards[j].transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 0.01f, Mathf.SmoothStep(0.0f, 1.0f, i));
            }
            i += Time.deltaTime * rate;
            yield return 0;
        }
    }

    private void ResetManagerPosition() {
        if (circleLayout)
        {
            if (fullCircle)
            {
                this.transform.position = new Vector3(0.25f, transform.position.y, 0f);
            }
            else
            {
                this.transform.position = new Vector3(0.25f, transform.position.y, -0.5f);
            }

        }
        else
        {
            this.transform.position = new Vector3(0.25f, transform.position.y, 0.8f);
        }
    }

    private string GetCorrectAnswer(int index) {
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
                if (taskNo == 6) {
                    finalString = QuestionIDs[index + 2];
                }else if(taskNo == 4)
                    finalString = QuestionIDs[index + 1];
            }
            else {
                finalString = QuestionIDs[index];
            }
        }
        else
        {
            finalString = QuestionIDs[index];
        }

        return finalString;
    }

    public void ValidateAnswer() {

        if (trialState == TrialState.Answer)
        {
            if (selectedAnswer != "")
            {
                if (!answerLogged) {
                    // write to file
                    string writerAnswerFilePath = "Assets/ExperimentData/ExperimentLog/Participant " + ExperimentManager.ParticipantID + "/Participant_" + ExperimentManager.ParticipantID + "_Answers.csv";
                    StreamWriter writerAnswer = new StreamWriter(writerAnswerFilePath, true);
                    //Debug.Log(GetCorrectAnswer(answerLogIncrement));

                    writerAnswer.Write(ExperimentManager.ParticipantID + "," + fullTaskID + "," + selectedAnswer + "," + completionTime + "," + GetCurrentDataset() + "," + GetCurrentLayout() + "," + GetCurrentTaskLevel() + "," + 
                        GetQuestionID(answerLogIncrement) + "," + GetCorrectAnswer(answerLogIncrement));
                    writerAnswer.Close();
                    answerLogged = true;
                    answerLogIncrement++;
                }
                

                if (taskID == taskNo)
                {
                    switch (ExperimentManager.ExperimentSequence)
                    {
                        case 1:
                            if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Flat")
                            {
                                //Debug.Log("End writing exp 1");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Full Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Full Circle")
                            {
                                //Debug.Log("End writing exp 2");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Half Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Half Circle")
                            {
                                //Debug.Log("End writing exp 3");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Flat")
                            {
                                //Debug.Log("End writing exp 4");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Full Circle")
                            {
                                //Debug.Log("End writing exp 5");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Half Circle")
                            {
                                taskID = -1;
                                fullTaskID = "All Finished";
                                //Debug.Log("End writing exp 6");
                                if (writer.BaseStream != null)
                                {
                                    EndWritingFile(writer);
                                }
                                UnityEditor.EditorApplication.isPlaying = false;
                            }
                            break;
                        case 2:
                            if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Flat")
                            {
                                //Debug.Log("End writing exp 1");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Full Circle")
                            {
                                //Debug.Log("End writing exp 2");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Half Circle")
                            {
                                //Debug.Log("End writing exp 3");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Flat");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Flat")
                            {
                                //Debug.Log("End writing exp 4");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Full Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Full Circle")
                            {
                                //Debug.Log("End writing exp 5");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Half Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Half Circle")
                            {
                                taskID = -1;
                                fullTaskID = "All Finished";
                                //Debug.Log("End writing exp 6");
                                if (writer.BaseStream != null)
                                {
                                    EndWritingFile(writer);
                                }
                                UnityEditor.EditorApplication.isPlaying = false;
                            }
                            break;
                        case 3:
                            if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Flat")
                            {
                                //Debug.Log("End writing exp 1");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Half Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Half Circle")
                            {
                                //Debug.Log("End writing exp 2");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Full Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Full Circle")
                            {
                                //Debug.Log("End writing exp 3");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Flat")
                            {
                                //Debug.Log("End writing exp 4");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Half Circle")
                            {
                                //Debug.log("End writing exp 5");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Full Circle")
                            {
                                taskID = -1;
                                fullTaskID = "All Finished";
                                //Debug.log("End writing exp 6");
                                if (writer.BaseStream != null)
                                {
                                    EndWritingFile(writer);
                                }
                                UnityEditor.EditorApplication.isPlaying = false;
                            }
                            break;
                        case 4:
                            if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Flat")
                            {
                                //Debug.log("End writing exp 1");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Half Circle")
                            {
                                //Debug.log("End writing exp 2");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Full Circle")
                            {
                                //Debug.log("End writing exp 3");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Flat");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Flat")
                            {
                                //Debug.log("End writing exp 4");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Half Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Half Circle")
                            {
                                //Debug.log("End writing exp 5");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Full Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Full Circle")
                            {
                                taskID = -1;
                                fullTaskID = "All Finished";
                                //Debug.log("End writing exp 6");
                                if (writer.BaseStream != null)
                                {
                                    EndWritingFile(writer);
                                }
                                UnityEditor.EditorApplication.isPlaying = false;
                            }
                            break;
                        case 5:
                            if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Full Circle")
                            {
                                //Debug.log("End writing exp 1");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Flat");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Flat")
                            {
                                //Debug.log("End writing exp 2");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Half Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Half Circle")
                            {
                                //Debug.log("End writing exp 3");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Full Circle")
                            {
                                //Debug.log("End writing exp 4");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Flat")
                            {
                                //Debug.log("End writing exp 5");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Half Circle")
                            {
                                taskID = -1;
                                fullTaskID = "All Finished";
                                //Debug.log("End writing exp 6");
                                if (writer.BaseStream != null)
                                {
                                    EndWritingFile(writer);
                                }
                                UnityEditor.EditorApplication.isPlaying = false;
                            }
                            break;
                        case 6:
                            if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Full Circle")
                            {
                                //Debug.log("End writing exp 1");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Flat")
                            {
                                //Debug.log("End writing exp 2");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Half Circle")
                            {
                                //Debug.log("End writing exp 3");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Full Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Full Circle")
                            {
                                //Debug.log("End writing exp 4");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Flat");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Flat")
                            {
                                //Debug.log("End writing exp 5");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Half Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Half Circle")
                            {
                                taskID = -1;
                                fullTaskID = "All Finished";
                                //Debug.log("End writing exp 6");
                                if (writer.BaseStream != null)
                                {
                                    EndWritingFile(writer);
                                }
                                UnityEditor.EditorApplication.isPlaying = false;
                            }
                            break;
                        case 7:
                            if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Full Circle")
                            {
                                //Debug.log("End writing exp 1");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Half Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Half Circle")
                            {
                                //Debug.log("End writing exp 2");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Flat");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Flat")
                            {
                                //Debug.log("End writing exp 3");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Full Circle")
                            {
                                //Debug.log("End writing exp 4");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Half Circle")
                            {
                                //Debug.log("End writing exp 5");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Flat")
                            {
                                taskID = -1;
                                fullTaskID = "All Finished";
                                //Debug.log("End writing exp 6");
                                if (writer.BaseStream != null)
                                {
                                    EndWritingFile(writer);
                                }
                                UnityEditor.EditorApplication.isPlaying = false;
                            }
                            break;
                        case 8:
                            if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Full Circle")
                            {
                                //Debug.log("End writing exp 1");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Half Circle")
                            {
                                //Debug.log("End writing exp 2");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Flat")
                            {
                                //Debug.log("End writing exp 3");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Full Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Full Circle")
                            {
                                //Debug.log("End writing exp 4");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Half Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Half Circle")
                            {
                                //Debug.log("End writing exp 5");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Flat");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Flat")
                            {
                                taskID = -1;
                                fullTaskID = "All Finished";
                                //Debug.log("End writing exp 6");
                                if (writer.BaseStream != null)
                                {
                                    EndWritingFile(writer);
                                }
                                UnityEditor.EditorApplication.isPlaying = false;
                            }
                            break;
                        case 9:
                            if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Half Circle")
                            {
                                //Debug.log("End writing exp 1");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Flat");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Flat")
                            {
                                //Debug.log("End writing exp 2");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Full Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Full Circle")
                            {
                                //Debug.log("End writing exp 3");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Half Circle")
                            {
                                //Debug.log("End writing exp 4");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Flat")
                            {
                                //Debug.log("End writing exp 5");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Full Circle")
                            {
                                taskID = -1;
                                fullTaskID = "All Finished";
                                //Debug.log("End writing exp 6");
                                if (writer.BaseStream != null)
                                {
                                    EndWritingFile(writer);
                                }
                                UnityEditor.EditorApplication.isPlaying = false;
                            }
                            break;
                        case 10:
                            if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Half Circle")
                            {
                                //Debug.log("End writing exp 1");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Flat")
                            {
                                //Debug.log("End writing exp 2");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Full Circle")
                            {
                                //Debug.log("End writing exp 3");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Half Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Half Circle")
                            {
                                //Debug.log("End writing exp 4");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Flat");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Flat")
                            {
                                //Debug.log("End writing exp 5");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Full Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Full Circle")
                            {
                                taskID = -1;
                                fullTaskID = "All Finished";
                                //Debug.log("End writing exp 6");
                                if (writer.BaseStream != null)
                                {
                                    EndWritingFile(writer);
                                }
                                UnityEditor.EditorApplication.isPlaying = false;
                            }
                            break;
                        case 11:
                            if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Half Circle")
                            {
                                //Debug.log("End writing exp 1");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Full Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Full Circle")
                            {
                                //Debug.log("End writing exp 2");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Flat");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Flat")
                            {
                                //Debug.log("End writing exp 3");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Half Circle")
                            {
                                //Debug.log("End writing exp 4");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Full Circle")
                            {
                                //Debug.log("End writing exp 5");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Flat")
                            {
                                taskID = -1;
                                fullTaskID = "All Finished";
                                //Debug.log("End writing exp 6");
                                if (writer.BaseStream != null)
                                {
                                    EndWritingFile(writer);
                                }
                                UnityEditor.EditorApplication.isPlaying = false;
                            }
                            break;
                        case 12: // testing sequence
                            if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Half Circle")
                            {
                                //Debug.log("End writing exp 1");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Full Circle")
                            {
                                //Debug.log("End writing exp 2");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 2 - Flat")
                            {
                                //Debug.log("End writing exp 3");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Half Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Half Circle")
                            {
                                //Debug.log("End writing exp 4");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Full Circle");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Full Circle")
                            {
                                //Debug.log("End writing exp 5");
                                EndWritingFile(writer);
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Flat");
                            }
                            else if (SceneManager.GetActiveScene().name == "SmallMultiples - DataSet 1 - Flat")
                            {
                                taskID = -1;
                                fullTaskID = "All Finished";

                                //Debug.log("End writing exp 6");
                                if (writer.BaseStream != null)
                                {
                                    EndWritingFile(writer);
                                }
                                UnityEditor.EditorApplication.isPlaying = false;
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
        else {
            interactionTrainingNeeded = false;
            if (GameObject.Find("Controller (left)") != null)
            {
                GameObject.Find("Controller (left)").transform.GetChild(5).gameObject.SetActive(false);
            }
            SetupPreTaskEnvironment("none");
        }
    }

    public void SetupPreTaskEnvironment(string controller) {
        colorSelected = false;
        colorSelectedIndex = 0;

        selectedAnswer = "";
        trainingQuestionReminderOn = false;
        if (GameObject.Find("rightCollisionDetector") != null) {
            CollisionDetection rcd = GameObject.Find("rightCollisionDetector").GetComponent<CollisionDetection>();
            rcd.answerSelected = false;
        }

        if (GameObject.Find("leftCollisionDetector") != null)
        {
            CollisionDetection lcd = GameObject.Find("leftCollisionDetector").GetComponent<CollisionDetection>();
            lcd.answerSelected = false;
        }

        for (int i = 1; i <= smallMultiplesNumber; i++) {
            Transform containerGO = GameObject.Find("Tooltip " + i).transform.Find("TooltipCanvas").GetChild(0);
            Transform frontTextGO = GameObject.Find("Tooltip " + i).transform.Find("TooltipCanvas").GetChild(1);
            Transform backTextGO = GameObject.Find("Tooltip " + i).transform.Find("TooltipCanvas").GetChild(2);

            Color originColor = new Color(227f / 255f, 227f / 255f, 227f / 255f);

            containerGO.GetComponent<Image>().color = originColor;
            frontTextGO.GetComponent<Text>().color = Color.black;
            backTextGO.GetComponent<Text>().color = Color.black;
        }

        completionTime = 60f;
        if (dataset == 1)
        {
            RefreshDataSet1();
        }
        else if (dataset == 2) {
            RefreshDataSet2();
        }
        for (int i = 0; i < smToolTips.Count; i++)
        {
            smToolTips[i].transform.SetParent(dataSM[i].transform);
        }

        GameObject leftController = GameObject.Find("Controller (left)");
        if (leftController != null)
        {
            if (dataset == 1)
            {
                leftController.transform.GetChild(6).gameObject.SetActive(false);
                GameObject monthAnswers = leftController.transform.GetChild(6).gameObject;
                Transform choicesParent = monthAnswers.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1);
                for (int i = 0; i < choicesParent.childCount; i++)
                {
                    choicesParent.GetChild(i).GetChild(0).GetComponent<Text>().color = Color.black;
                }

                leftController.transform.GetChild(5).gameObject.SetActive(false);
                GameObject sensorAnswers = leftController.transform.GetChild(5).gameObject;
                choicesParent = sensorAnswers.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1);
                for (int i = 0; i < choicesParent.childCount; i++)
                {
                    choicesParent.GetChild(i).GetChild(0).GetComponent<Text>().color = Color.black;
                }

            }
            else if (dataset == 2)
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

                leftController.transform.GetChild(6).gameObject.SetActive(false);
                GameObject chartAnswers = leftController.transform.GetChild(6).gameObject;
                choicesParent = chartAnswers.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1);
                for (int i = 0; i < choicesParent.childCount; i++)
                {
                    choicesParent.GetChild(i).GetChild(0).GetComponent<Text>().color = Color.black;
                }
            }
        }

        trialState = TrialState.PreTask;
        answerLogged = false;
        if (interactionTrainingNeeded)
        {
            if (interactionTrainingCount > 0)
            {
                //if (interactionTrainingCount == 9)
                //{
                //    scaling = true;
                //}
                //else {
                //    scaling = false;
                //}
                if (dataset == 2)
                {
                    if (interactionTrainingCount == 7)
                    {
                        interactionTrainingCount--;
                    }
                }
                if (interactionTrainingCount == 1) {
                    if (GameObject.Find("Controller (left)") != null) {
                        if (dataset == 1)
                        {
                            GameObject.Find("Controller (left)").transform.GetChild(5).gameObject.SetActive(true);
                        }
                        else if (dataset == 2) {
                            GameObject.Find("Controller (left)").transform.GetChild(4).gameObject.SetActive(true);
                        }
                    }
                }
                
                ChangeTaskText(interactionTrainingDesc[8 - interactionTrainingCount], -1);
                interactionTrainingCount--;
                //Debug.Log(interactionTrainingCount);
                if (interactionTrainingCount == 0) {
                    if (GameObject.Find("Controller (left)") != null && GameObject.Find("Controller (right)") != null)
                    {
                        GameObject.Find("Controller (left)").transform.Find("TrackPadLabel").GetComponent<TextMeshPro>().text = "Done";
                        GameObject.Find("Controller (right)").transform.Find("TrackPadLabel").GetComponent<TextMeshPro>().text = "Done";
                    }
                }
            }
            else
            {
                interactionTrainingNeeded = false;
                if (GameObject.Find("Controller (left)") != null)
                {
                    GameObject.Find("Controller (left)").transform.GetChild(5).gameObject.SetActive(false);
                }
                SetupPreTaskEnvironment("none");
            }
        }
        else {

            if (GameObject.Find("Controller (left)") != null && GameObject.Find("Controller (right)") != null)
            {
                GameObject.Find("Controller (left)").transform.Find("TrackPadLabel").GetComponent<TextMeshPro>().text = "Next";
                GameObject.Find("Controller (right)").transform.Find("TrackPadLabel").GetComponent<TextMeshPro>().text = "Next";
            }
            GameObject.Find("EnvironmentForUserStudy").transform.GetChild(3).gameObject.SetActive(true);
            //this.transform.position += Vector3.down * 10; // user study
            //scaling = true; // pilot study
            ChangeTaskText("Please stand on the floor marker.\n\n" +
                "Please press the <color=green>Next</color> button on your controller to move on.", -1);
        }
    }

    public void StartTask() {

        foreach (GameObject tooltip in smToolTips) {
            tooltip.transform.SetParent(shelf.transform);
        }

        if (trialState == TrialState.PreTask)
        {
            trialState = TrialState.OnTask;
            if (GameObject.Find("Controller (left)") != null && GameObject.Find("Controller (right)") != null) {
                GameObject.Find("Controller (left)").transform.Find("TrackPadLabel").GetComponent<TextMeshPro>().text = "Finish";
                GameObject.Find("Controller (right)").transform.Find("TrackPadLabel").GetComponent<TextMeshPro>().text = "Finish";
            }
            GameObject.Find("EnvironmentForUserStudy").transform.GetChild(3).gameObject.SetActive(false);
            //this.transform.position += Vector3.up * 10; // user study
            //scaling = false; // pilot study
            if (taskID < taskNo && taskID >= 0)
            {
                //Debug.Log("Next Task");
                
                NextTask();
            }
            for (int i = 0; i < 3; i++) {
                Transform countDownTimer = GameObject.Find("TaskBoards").transform.GetChild(i).Find("CountdownTimer");
                CountDownTimer cdt = countDownTimer.GetComponent<CountDownTimer>();
                cdt.StartTimer();
            }
        }
    }

    public void FinishOrTimeUpToAnswer() {
        if (trialState == TrialState.OnTask)
        {
            for (int i = 0; i < 3; i++)
            {
                Transform countDownTimer = GameObject.Find("TaskBoards").transform.GetChild(i).Find("CountdownTimer");
                CountDownTimer cdt = countDownTimer.GetComponent<CountDownTimer>();
                completionTime = 60f - cdt.countTimer;
                cdt.ResetTimer();
            }
            
            //Debug.log("Completion time: " + completionTime);
            MoveToAnswerState();
        }
        else {
            Debug.LogError("wrong state for timer time up!");
        }
    }

    private void MoveToAnswerState() {
        trialState = TrialState.Answer;
        GameObject leftController = GameObject.Find("Controller (left)");
        ChangeTaskText("Please choose the answer from the options attached to your left controller. And press <color=red>trigger</color> button to confirm.", -1);
        if (dataset == 1)
        {
            if (taskNo == 6)
            {
                if (taskID == 0 && trainingCounting == (taskNo / 2) && trainingCountringLeft == 0)
                {
                    if (leftController != null)
                    {
                        GameObject monthAnswers = leftController.transform.GetChild(6).gameObject;
                        monthAnswers.SetActive(true);
                    }
                }
                else if (taskID > 0 && (taskID % taskNo == 0 || taskID % taskNo == (taskNo - 1)))
                {
                    if (leftController != null)
                    {
                        GameObject monthAnswers = leftController.transform.GetChild(6).gameObject;
                        monthAnswers.SetActive(true);
                    }
                }
                else
                {
                    if (leftController != null)
                    {
                        GameObject sensorAnswers = leftController.transform.GetChild(5).gameObject;
                        sensorAnswers.SetActive(true);
                    }
                }
            }
            else if (taskNo == 4) {

                if (leftController != null)
                {
                    GameObject sensorAnswers = leftController.transform.GetChild(5).gameObject;
                    sensorAnswers.SetActive(true);
                }
            }
            
        }
        else if (dataset == 2) {

            if (taskNo == 6)
            {
                if (taskID == 0 && trainingCounting == (taskNo / 2) && trainingCountringLeft == 0) // training task 3
                {
                    if (leftController != null)
                    {
                        GameObject chartAnswers = leftController.transform.GetChild(6).gameObject; //  chart
                        chartAnswers.SetActive(true);

                    }
                }
                else if (taskID == 0 && trainingCounting == (taskNo / 2) && trainingCountringLeft == 1) // training task 2
                {
                    if (leftController != null)
                    {
                        GameObject yearAnswers = leftController.transform.GetChild(5).gameObject; // year
                        yearAnswers.SetActive(true);

                    }
                }
                else if (taskID > 0 && (taskID % (taskNo) == 0 || taskID % (taskNo) == (taskNo - 1))) // task 5, 6
                {
                    if (leftController != null)
                    {
                        GameObject chartAnswers = leftController.transform.GetChild(6).gameObject;
                        chartAnswers.SetActive(true);
                    }
                }
                else if (taskID > 0 && (taskID % (taskNo) == (taskNo - 3) || taskID % (taskNo) == (taskNo - 2))) // task 3, 4
                {
                    if (leftController != null)
                    {
                        GameObject yearAnswers = leftController.transform.GetChild(5).gameObject;
                        yearAnswers.SetActive(true);
                    }
                }
                else
                {
                    if (leftController != null)
                    {
                        GameObject countryAnswers = leftController.transform.GetChild(4).gameObject;
                        countryAnswers.SetActive(true);
                    }
                }
            }
            else if (taskNo == 4) {
                if (taskID == 0 && trainingCounting == (taskNo / 2) && trainingCountringLeft == 0) // training task 2
                {
                    if (leftController != null)
                    {
                        GameObject yearAnswers = leftController.transform.GetChild(5).gameObject; // year
                        yearAnswers.SetActive(true);

                    }
                }
                else if (taskID > 0 && (taskID % (taskNo) == (taskNo - 1) || taskID % (taskNo) == 0)) // task 3, 4
                {
                    if (leftController != null)
                    {
                        GameObject yearAnswers = leftController.transform.GetChild(5).gameObject;
                        yearAnswers.SetActive(true);
                    }
                }
                else
                {
                    if (leftController != null)
                    {
                        GameObject countryAnswers = leftController.transform.GetChild(4).gameObject;
                        countryAnswers.SetActive(true);
                    }
                }
            }   
        }
    }

    public void ShowTaskForTraining() {
        if (trialState == TrialState.Answer && taskID == 0)
        {
            trainingQuestionReminderOn = true;
            ShowTrainingTask(trainingCounting - trainingCountringLeft - 1);
        }
    }

    public string GetTaskText() {
        if (taskID == 0 && trainingCountringLeft >= 1)
        {
            return trainingTaskArray[trainingCounting - trainingCountringLeft];
        }
        else if (taskID == 0)
        {
            return taskArray[0];
        }
        else {
            return taskArray[taskID];
        }
    }

    /// <summary>
    /// Task Related
    /// </summary>
    private void NextTask() {

        if (trialState == TrialState.OnTask)
        {
            Vector2 needHighlightedSM = Vector2.zero;
            if (taskID == 0)
            {
                if (trainingCountringLeft > 0)
                {
                    if (trainingCounting == (taskNo / 2))
                    {
                        ExperimentManager.comprehensiveTraining = false;
                    }
                    ShowTrainingTask(trainingCounting - trainingCountringLeft);
                    fullTaskID = "Training";
                    trainingCountringLeft--;
                    needHighlightedSM = highlightedTrainingSM[trainingCounting - trainingCountringLeft - 1];
                }
                else
                {
                    taskID++;
                    fullTaskID = taskID.ToString();
                    deltaTrialNumber++;
                    ShowTasks();
                    needHighlightedSM = highlightedSM[taskID - 1];
                }
            }
            else {
                taskID++;
                fullTaskID = taskID.ToString();
                deltaTrialNumber++;
                ShowTasks();
                needHighlightedSM = highlightedSM[taskID - 1];
            }

            
            if (needHighlightedSM != Vector2.zero)
            {

                if (GameObject.Find("Tooltip " + needHighlightedSM.x) != null)
                {
                    Transform containerGO = GameObject.Find("Tooltip " + needHighlightedSM.x).transform.Find("TooltipCanvas").GetChild(0);
                    Transform frontTextGO = GameObject.Find("Tooltip " + needHighlightedSM.x).transform.Find("TooltipCanvas").GetChild(1);
                    Transform backTextGO = GameObject.Find("Tooltip " + needHighlightedSM.x).transform.Find("TooltipCanvas").GetChild(2);

                    containerGO.GetComponent<Image>().color = Color.red;
                    frontTextGO.GetComponent<Text>().color = Color.white;
                    backTextGO.GetComponent<Text>().color = Color.white;
                }

                if (GameObject.Find("Tooltip " + needHighlightedSM.y) != null)
                {
                    Transform containerGO = GameObject.Find("Tooltip " + needHighlightedSM.y).transform.Find("TooltipCanvas").GetChild(0);
                    Transform frontTextGO = GameObject.Find("Tooltip " + needHighlightedSM.y).transform.Find("TooltipCanvas").GetChild(1);
                    Transform backTextGO = GameObject.Find("Tooltip " + needHighlightedSM.y).transform.Find("TooltipCanvas").GetChild(2);

                    containerGO.GetComponent<Image>().color = Color.red;
                    frontTextGO.GetComponent<Text>().color = Color.white;
                    backTextGO.GetComponent<Text>().color = Color.white;
                }
            }
            currentHighlightedSM = needHighlightedSM;
        }
        else {
            Debug.Log("wrong state!" + taskID);
        } 
    }

    void OnApplicationQuit()
    {
        EndWritingFile(writer);
        //StopCoroutine(coroutine);
    }

    void EndWritingFile(StreamWriter writer) {
        if (writer != null) {
            writer.Close();
        }
        if (writerEye != null) {
            writerEye.Close();
        }
    }

    void WritingLog() {
        GameObject leftController = GameObject.Find("Controller (left)");
        GameObject rightController = GameObject.Find("Controller (right)"); 

        if (writer != null && Camera.main != null && leftController != null && rightController != null)
        {
            SteamVR_TrackedController leftControllerScript = leftController.GetComponent<SteamVR_TrackedController>();
            SteamVR_TrackedController rightControllerScript = rightController.GetComponent<SteamVR_TrackedController>();
            

            //writer.WriteLine(GetFixedTime() + "," + ExperimentManager.userHeight + "," + GetTrialNumber().ToString() + "," + ExperimentManager.ParticipantID + "," + ExperimentManager.ExperimentSequence + "," +
            //    GetCurrentDataset() + "," + GetCurrentLayout() + "," + taskID + "," + startTask + "," + VectorToString(Camera.main.transform.position) + "," + VectorToString(Camera.main.transform.eulerAngles) + "," +
            //    VectorToString(leftController.transform.position) + "," + VectorToString(leftController.transform.eulerAngles) + "," + VectorToString(rightController.transform.position) + "," +
            //    VectorToString(rightController.transform.eulerAngles) + "," + leftControllerScript.menuPressed + "," + leftControllerScript.triggerPressed + "," + leftControllerScript.gripped + "," +
            //    nextTaskBtn + "," + previousTaskBtn + "," + rightControllerScript.menuPressed + "," + rightControllerScript.triggerPressed + "," + rightControllerScript.gripped + "," + 
            //    RTTopBtn + "," + RTLeftBtn + "," + RTRightBtn + "," + RTBtmBtn + "," + GetClockRotation(leftControllerScript, rightControllerScript) + "," + GetAnticlockRotation(leftControllerScript, rightControllerScript) + "," +
            //    leftPillarToLeft + "," + leftPillarToRight + "," + 
            //    rightPillarToLeft + "," + rightPillarToRight + "," + GetTraRightTouchpadButtonIndex("top") + "," + GetTraRightTouchpadButtonIndex("bottom") + "," + GetTraRightTouchpadButtonIndex("right") + "," + 
            //    GetTraRightTouchpadButtonIndex("left") + "," + rightControllerScript.menuPressed);

            writer.WriteLine(GetFixedTime() + "," + ExperimentManager.userHeight + "," + GetTrialNumber().ToString() + "," + ExperimentManager.ParticipantID + "," + ExperimentManager.ExperimentSequence + "," +
                GetCurrentDataset() + "," + GetCurrentLayout() + "," + fullTaskID + "," + GetCurrentTaskLevel() + "," + trialState + "," + VectorToString(Camera.main.transform.position) + "," + VectorToString(Camera.main.transform.eulerAngles) + "," +
                VectorToString(leftController.transform.position) + "," + VectorToString(leftController.transform.eulerAngles) + "," + VectorToString(rightController.transform.position) + "," +
                VectorToString(rightController.transform.eulerAngles) + "," + leftControllerScript.menuPressed + "," + leftControllerScript.triggerPressed + "," + leftControllerScript.gripped + "," +
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
            writerEye.WriteLine(GetFixedTime() + "," + ExperimentManager.ParticipantID + "," + fullTaskID + "," + GetCurrentDataset() + "," + GetCurrentLayout() + "," +
            GetCurrentTaskLevel() + "," + trialState + "," + rawGazePositionOnScreen.x + "," + rawGazePositionOnScreen.y + "," + ("Small Multiples " + currentHighlightedSM.x) + "," +
            ("Small Multiples " + currentHighlightedSM.y) + "," + GetGazedSM() + "," + GetGazedWorldPosition());
            writerEye.Flush();
        }

        if (writerHead != null && Camera.main != null)
        {
            writerHead.WriteLine(GetFixedTime() + "," + ExperimentManager.ParticipantID + "," + fullTaskID + "," + GetCurrentDataset() + "," + GetCurrentLayout() + "," +
                GetCurrentTaskLevel() + "," + trialState + "," + VectorToString(transform.position) + "," + transform.localScale.x + "," + VectorToString(Camera.main.transform.position) + "," + VectorToString(Camera.main.transform.eulerAngles));
            writerHead.Flush();
        }
    }

   

    private string GetSMFilterPositions() {
        if (dataset == 1)
        {
            return minXFilters[0].transform.localPosition.y + "," + maxXFilters[0].transform.localPosition.y + "," + minYFilters[0].transform.localPosition.y + "," + maxYFilters[0].transform.localPosition.y + "," + minZFilters[0].transform.localPosition.y + "," + maxZFilters[0].transform.localPosition.y;
        }
        else if (dataset == 2)
        {
            return leftCountryFilters[0].transform.localPosition.y + "," + rightCountryFilters[0].transform.localPosition.y + "," + leftValueFilters[0].transform.localPosition.y + "," + rightValueFilters[0].transform.localPosition.y + "," + leftYearFilters[0].transform.localPosition.y + "," + rightYearFilters[0].transform.localPosition.y;
        }
        else {
            return "";
        }
    }

    // get gazed world Vector3 info
    private string GetGazedWorldPosition()
    {
        Vector3 finalPosition = Vector3.zero;

        if (PupilTools.IsConnected && PupilTools.IsGazing && afterCalibration)
        {
            Vector2 gazePointCenter = PupilData._2D.GazePosition;
            Vector3 viewportPoint = new Vector3(gazePointCenter.x, gazePointCenter.y, 1f);
            if (GameObject.Find("[CameraRig]") != null && GameObject.Find("[CameraRig]").transform.GetChild(2) != null && GameObject.Find("[CameraRig]").transform.GetChild(2).GetComponent<Camera>() != null)
            {
                Ray ray = GameObject.Find("[CameraRig]").transform.GetChild(2).GetComponent<Camera>().ViewportPointToRay(viewportPoint);
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
            if (GameObject.Find("[CameraRig]") != null && GameObject.Find("[CameraRig]").transform.GetChild(2) != null && GameObject.Find("[CameraRig]").transform.GetChild(2).GetComponent<Camera>() != null)
            {
                Ray ray = GameObject.Find("[CameraRig]").transform.GetChild(2).GetComponent<Camera>().ViewportPointToRay(viewportPoint);

                //Debug.DrawRay(ray.origin, ray.direction, Color.green);

                //RaycastHit[] hits;
                //hits = Physics.RaycastAll(ray, 100f);

                //if (hits.Length == 0)
                //{
                //    finalResult = "NA";
                //    GameObject.Find("TestingText").GetComponent<Text>().text = "I'm looking at nothing";
                //}
                //else {
                //    for (int i = 0; i < hits.Length; i++)
                //    {
                //        RaycastHit hit = hits[i];
                //        if (hit.transform.name.Contains("Small Multiples"))
                //        {
                //            finalResult = hit.transform.name;
                //            GameObject.Find("TestingText").GetComponent<Text>().text = "I'm looking at " + hit.transform.name;
                //            return finalResult;
                //        }
                //        else
                //        {
                //            GameObject.Find("TestingText").GetComponent<Text>().text = "I'm looking at " + hit.transform.name;
                //            finalResult = "NA";
                //        }
                //    }
                //}
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.name.Contains("Small Multiples"))
                    {
                        finalResult = hit.transform.name;
                        GameObject.Find("TestingText").GetComponent<Text>().text = "I'm looking at " + hit.transform.name;
                    }
                    else
                    {
                        GameObject.Find("TestingText").GetComponent<Text>().text = "I'm looking at " + hit.transform.name;
                    }
                }
                else
                {

                    finalResult = "NA";
                    GameObject.Find("TestingText").GetComponent<Text>().text = "I'm looking at nothing";
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
        //PupilSettings.Instance.currentCamera.gameObject.SetActive(false);
        //GameObject.Find("Pupil Manager").transform.GetChild(0).gameObject.SetActive(false);
        //PupilSettings.Instance.currentCamera = GameObject.Find("[CameraRig]").transform.GetChild(2).GetComponent<Camera>();
        GameObject.Find("[CameraRig]").transform.GetChild(2).gameObject.SetActive(true);
        PupilSettings.Instance.currentCamera = GameObject.Find("[CameraRig]").transform.GetChild(2).GetComponent<Camera>();

        afterCalibration = true;

        if (PupilTools.IsConnected)
        {
            PupilTools.SubscribeTo("gaze");
            PupilTools.IsGazing = true;
        }
    }

    public void OpenPupilCamera() {
        if (GameObject.Find("[CameraRig]") != null && GameObject.Find("[CameraRig]").transform.GetChild(2) != null)
        {
            GameObject.Find("[CameraRig]").transform.GetChild(2).gameObject.SetActive(false);
            //GameObject.Find("Pupil Manager").transform.GetChild(0).gameObject.SetActive(true);
            //GameObject.Find("Pupil Manager").transform.GetChild(2).gameObject.SetActive(true);
            PupilSettings.Instance.currentCamera = GameObject.Find("Pupil Manager").transform.GetChild(2).GetComponent<Camera>();
            //GameObject.Find("EnvironmentForUserStudy").transform.Find("Canvas").GetComponent<Canvas>().worldCamera = GameObject.Find("[CameraRig]").transform.GetChild(2).GetComponent<Camera>();
            afterCalibration = false;
            calibrationFlag = true;
            GameObject.Find("TestingText").GetComponent<Text>().text = "Calibrating!!!";
        }
    }

    // get Raw gaze Vector2 data
    private void GetGazeInfo() {
        GameObject filterPanel = GameObject.Find("PanelMenuForColorFilter");
        Transform gridLayoutChoices = null;
        if (filterPanel != null)
        {
            gridLayoutChoices = filterPanel.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1);
        }
        if (gridLayoutChoices != null)
        {
            if (colorSelectedIndex == 0)
            {
                for (int i = 1; i <= 9; i++)
                {
                    gridLayoutChoices.GetChild(i).GetChild(0).GetComponent<Text>().color = Color.white;
                }
            }
        }
        if (PupilTools.IsConnected && PupilTools.IsGazing && afterCalibration)
        {
            //Debug.Log(rawGazePositionOnScreen);
            Vector2 gazePointCenter = PupilData._2D.GazePosition;
            rawGazePositionOnScreen = PupilData._2D.GazePosition;
            //Vector3 viewportPoint = new Vector3(gazePointCenter.x, gazePointCenter.y, 1f);
            //if (GameObject.Find("[CameraRig]") != null && GameObject.Find("[CameraRig]").transform.GetChild(2) != null && GameObject.Find("[CameraRig]").transform.GetChild(2).GetComponent<Camera>() != null) {
            //    Ray ray = GameObject.Find("[CameraRig]").transform.GetChild(2).GetComponent<Camera>().ViewportPointToRay(viewportPoint);
            //    //Debug.DrawRay(ray.origin, ray.direction, Color.green);
            //    RaycastHit hit;
            //    if (Physics.Raycast(ray, out hit))
            //    {
            //        GameObject.Find("TestingText").GetComponent<Text>().text = "I'm looking at " + hit.transform.name;
            //    }
            //    else
            //    {
            //        GameObject.Find("TestingText").GetComponent<Text>().text = "I'm looking at nothing";
            //    }
            //}
        }
        else {
            //Debug.Log(PupilTools.IsConnected + " " + PupilTools.IsGazing + " " + afterCalibration);
        }
    }

    /*
    private string GetSMPosition() {
        string finalResult = "";
        if (dataSM.Count <= 0) {
            return ",,,,,,,,,,,,";
        }

        foreach (GameObject sm in dataSM) {
            finalResult += "(" + sm.transform.position.x + ";" + sm.transform.position.y + ";" + sm.transform.position.z + "),";
        }

        return finalResult;
    }

    private string GetSMRotation() {
        string finalResult = "";

        if (dataSM.Count <= 0)
        {
            return ",,,,,,,,,,,";
        }

        for (int i = 0; i <= 11; i++) {
            if (i != 11)
            {
                finalResult += "(" + dataSM[i].transform.eulerAngles.x + ";" + dataSM[i].transform.eulerAngles.y + ";" + dataSM[i].transform.eulerAngles.z + "),";
            }
            else {
                finalResult += "(" + dataSM[i].transform.eulerAngles.x + ";" + dataSM[i].transform.eulerAngles.y + ";" + dataSM[i].transform.eulerAngles.z + ")";
            }
        }

        return finalResult;
    }
    */

    float GetFixedTime()
    {
        float finalTime = 0;
        if (ExperimentManager.PublicTrialNumber != 0)
        {
            finalTime = ExperimentManager.lastTimePast + Time.fixedTime;
        }
        else
        {
            finalTime = Time.fixedTime;
        }
        return finalTime;
    }

    string GetCurrentTaskLevel() {
        string final = "";
        if (taskNo == 6)
        {
            if (taskID == 0 && trainingCounting == (taskNo / 2) && trainingCountringLeft == 0) // training task 3
            {
                final = "3";
            }
            else if (taskID == 0 && trainingCounting == (taskNo / 2) && trainingCountringLeft == 1) // training task 2
            {
                final = "2";
            }
            else if (taskID == 0 && trainingCounting == (taskNo / 2) && trainingCountringLeft == 2) // training task 1
            {
                final = "1";
            }
            else if (taskID > 0 && (taskID % (taskNo) == 0 || taskID % (taskNo) == (taskNo - 1))) // task 5, 6
            {
                final = "3";
            }
            else if (taskID > 0 && (taskID % (taskNo) == (taskNo - 3) || taskID % (taskNo) == (taskNo - 2))) // task 3, 4
            {
                final = "2";
            }
            else // task 1, 2
            {
                final = "1";
            }
        }
        else if (taskNo == 4) {
            if (taskID == 0 && trainingCounting == (taskNo / 2) && trainingCountringLeft == 0) // training task 2
            {
                final = "2";
            }
            else if (taskID == 0 && trainingCounting == (taskNo / 2) && trainingCountringLeft == 1) // training task 1
            {
                final = "1";
            }
            else if (taskID > 0 && (taskID % (taskNo) == 0 || taskID % (taskNo) == (taskNo - 1))) // task 3, 4
            {
                final = "2";
            }
            else // task 1, 2
            {
                final = "1";
            }
        }
        return final;
    }

    string GetCurrentLayout() {
        string layout;
        if (dataset != 3)
        {
            if (circleLayout)
            {
                if (fullCircle)
                {
                    layout = "Quarter-Circle";
                }
                else {
                    layout = "Half-Circle";
                }
            }
            else
            {
                layout = "Flat";
            }
        }
        else
        {
            layout = "N/A";
        }
        return layout;
    }

    string GetCurrentDataset() {
        string datasetCatgory = "";
        if (dataset == 1)
        {
            datasetCatgory = "BIM";
        }
        else if (dataset == 2)
        {
            datasetCatgory = "Bar";
        }
        return datasetCatgory;
    }

    string GetTrialNumber() {

        int baseDeltaTrialNumber = 0;
        int finalTrialNumber = 0;

        switch (ExperimentManager.ExperimentSequence) {
            case 1:
                if (!circleLayout && dataset == 1) // F
                {
                    baseDeltaTrialNumber = 0 * taskNo;
                }
                else if(circleLayout && fullCircle && dataset == 1) // Q
                {
                    baseDeltaTrialNumber = 1 * taskNo;
                }
                else if (circleLayout && !fullCircle && dataset == 1) // H
                {
                    baseDeltaTrialNumber = 2 * taskNo;
                }
                else if (!circleLayout && dataset == 2) // F
                {
                    baseDeltaTrialNumber = 3 * taskNo;
                }
                else if (circleLayout && fullCircle && dataset == 2) // Q
                {
                    baseDeltaTrialNumber = 4 * taskNo;
                }
                else if (circleLayout && !fullCircle && dataset == 2) // H
                {
                    baseDeltaTrialNumber = 5 * taskNo;
                }
                break;
            case 2:
                if (!circleLayout && dataset == 2) // F
                {
                    baseDeltaTrialNumber = 0 * taskNo;
                }
                else if (circleLayout && fullCircle && dataset == 2) // Q
                {
                    baseDeltaTrialNumber = 1 * taskNo;
                }
                else if (circleLayout && !fullCircle && dataset == 2) // H
                {
                    baseDeltaTrialNumber = 2 * taskNo;
                }
                else if (!circleLayout && dataset == 1) // F
                {
                    baseDeltaTrialNumber = 3 * taskNo;
                }
                else if (circleLayout && fullCircle && dataset == 1) // Q
                {
                    baseDeltaTrialNumber = 4 * taskNo;
                }
                else if (circleLayout && !fullCircle && dataset == 1) // H
                {
                    baseDeltaTrialNumber = 5 * taskNo;
                }
                break;
            case 3:
                if (!circleLayout && dataset == 1) // F
                {
                    baseDeltaTrialNumber = 0 * taskNo;
                }
                else if (circleLayout && !fullCircle && dataset == 1) // H
                {
                    baseDeltaTrialNumber = 1 * taskNo;
                }
                else if (circleLayout && fullCircle && dataset == 1) // Q
                {
                    baseDeltaTrialNumber = 2 * taskNo;
                }
                else if (!circleLayout && dataset == 2) // F
                {
                    baseDeltaTrialNumber = 3 * taskNo;
                }
                else if (circleLayout && !fullCircle && dataset == 2) // H
                {
                    baseDeltaTrialNumber = 4 * taskNo;
                }
                else if (circleLayout && fullCircle && dataset == 2) // Q
                {
                    baseDeltaTrialNumber = 5 * taskNo;
                }
                break;
            case 4:
                if (!circleLayout && dataset == 2) // F
                {
                    baseDeltaTrialNumber = 0 * taskNo;
                }
                else if (circleLayout && !fullCircle && dataset == 2) // H
                {
                    baseDeltaTrialNumber = 1 * taskNo;
                }
                else if (circleLayout && fullCircle && dataset == 2) // Q
                {
                    baseDeltaTrialNumber = 2 * taskNo;
                }
                else if (!circleLayout && dataset == 1) // F
                {
                    baseDeltaTrialNumber = 3 * taskNo;
                }
                else if (circleLayout && !fullCircle && dataset == 1) // H
                {
                    baseDeltaTrialNumber = 4 * taskNo;
                }
                else if (circleLayout && fullCircle && dataset == 1) // Q
                {
                    baseDeltaTrialNumber = 5 * taskNo;
                }
                break;
            case 5:
                if (circleLayout && fullCircle && dataset == 1) // Q
                {
                    baseDeltaTrialNumber = 0 * taskNo;
                }
                else if (!circleLayout && dataset == 1) // F
                {
                    baseDeltaTrialNumber = 1 * taskNo;
                }
                else if (circleLayout && !fullCircle && dataset == 1) // H
                {
                    baseDeltaTrialNumber = 2 * taskNo;
                }
                else if (circleLayout && fullCircle && dataset == 2) // Q
                {
                    baseDeltaTrialNumber = 3 * taskNo;
                }
                else if (!circleLayout && dataset == 2) // F
                {
                    baseDeltaTrialNumber = 4 * taskNo;
                }
                else if (circleLayout && !fullCircle && dataset == 2) // H
                {
                    baseDeltaTrialNumber = 5 * taskNo;
                }
                break;
            case 6:
                if (circleLayout && fullCircle && dataset == 2) // Q
                {
                    baseDeltaTrialNumber = 0 * taskNo;
                }
                else if (!circleLayout && dataset == 2) // F
                {
                    baseDeltaTrialNumber = 1 * taskNo;
                }
                else if (circleLayout && !fullCircle && dataset == 2) // H
                {
                    baseDeltaTrialNumber = 2 * taskNo;
                }
                else if (circleLayout && fullCircle && dataset == 1) // Q
                {
                    baseDeltaTrialNumber = 3 * taskNo;
                }
                else if (!circleLayout && dataset == 1) // F
                {
                    baseDeltaTrialNumber = 4 * taskNo;
                }
                else if (circleLayout && !fullCircle && dataset == 1) // H
                {
                    baseDeltaTrialNumber = 5 * taskNo;
                }
                break;
            case 7:
                if (circleLayout && fullCircle && dataset == 1) // Q
                {
                    baseDeltaTrialNumber = 0 * taskNo;
                }
                else if (circleLayout && !fullCircle && dataset == 1) // H
                {
                    baseDeltaTrialNumber = 1 * taskNo;
                }
                else if (!circleLayout && dataset == 1) // F
                {
                    baseDeltaTrialNumber = 2 * taskNo;
                }
                else if (circleLayout && fullCircle && dataset == 2) // Q
                {
                    baseDeltaTrialNumber = 3 * taskNo;
                }
                else if (circleLayout && !fullCircle && dataset == 2) // H
                {
                    baseDeltaTrialNumber = 4 * taskNo;
                }
                else if (!circleLayout && dataset == 2) // F
                {
                    baseDeltaTrialNumber = 5 * taskNo;
                }
                break;
            case 8:
                if (circleLayout && fullCircle && dataset == 2) // Q
                {
                    baseDeltaTrialNumber = 0 * taskNo;
                }
                else if (circleLayout && !fullCircle && dataset == 2) // H
                {
                    baseDeltaTrialNumber = 1 * taskNo;
                }
                else if (!circleLayout && dataset == 2) // F
                {
                    baseDeltaTrialNumber = 2 * taskNo;
                }
                else if (circleLayout && fullCircle && dataset == 1) // Q
                {
                    baseDeltaTrialNumber = 3 * taskNo;
                }
                else if (circleLayout && !fullCircle && dataset == 1) // H
                {
                    baseDeltaTrialNumber = 4 * taskNo;
                }
                else if (!circleLayout && dataset == 1) // F
                {
                    baseDeltaTrialNumber = 5 * taskNo;
                }
                break;
            case 9:
                if (circleLayout && !fullCircle && dataset == 1) // H
                {
                    baseDeltaTrialNumber = 0 * taskNo;
                }
                else if (!circleLayout && dataset == 1) // F
                {
                    baseDeltaTrialNumber = 1 * taskNo;
                }
                else if (circleLayout && fullCircle && dataset == 1) // Q
                {
                    baseDeltaTrialNumber = 2 * taskNo;
                }
                else if (circleLayout && !fullCircle && dataset == 2) // H
                {
                    baseDeltaTrialNumber = 3 * taskNo;
                }
                else if (!circleLayout && dataset == 2) // F
                {
                    baseDeltaTrialNumber = 4 * taskNo;
                }
                else if (circleLayout && fullCircle && dataset == 2) // Q
                {
                    baseDeltaTrialNumber = 5 * taskNo;
                }
                break;
            case 10:
                if (circleLayout && !fullCircle && dataset == 2) // H
                {
                    baseDeltaTrialNumber = 0 * taskNo;
                }
                else if (!circleLayout && dataset == 2) // F
                {
                    baseDeltaTrialNumber = 1 * taskNo;
                }
                else if (circleLayout && fullCircle && dataset == 2) // Q
                {
                    baseDeltaTrialNumber = 2 * taskNo;
                }
                else if (circleLayout && !fullCircle && dataset == 1) // H
                {
                    baseDeltaTrialNumber = 3 * taskNo;
                }
                else if (!circleLayout && dataset == 1) // F
                {
                    baseDeltaTrialNumber = 4 * taskNo;
                }
                else if (circleLayout && fullCircle && dataset == 1) // Q
                {
                    baseDeltaTrialNumber = 5 * taskNo;
                }
                break;
            case 11:
                if (circleLayout && !fullCircle && dataset == 1) // H
                {
                    baseDeltaTrialNumber = 0 * taskNo;
                }
                else if (circleLayout && fullCircle && dataset == 1) // Q
                {
                    baseDeltaTrialNumber = 1 * taskNo;
                }
                else if (!circleLayout && dataset == 1) // F
                {
                    baseDeltaTrialNumber = 2 * taskNo;
                }
                else if (circleLayout && !fullCircle && dataset == 2) // H
                {
                    baseDeltaTrialNumber = 3 * taskNo;
                }
                else if (circleLayout && fullCircle && dataset == 2) // Q
                {
                    baseDeltaTrialNumber = 4 * taskNo;
                }
                else if (!circleLayout && dataset == 2) // F
                {
                    baseDeltaTrialNumber = 5 * taskNo;
                }
                break;
            case 12:
                if (circleLayout && !fullCircle && dataset == 2) // H
                {
                    baseDeltaTrialNumber = 0 * taskNo;
                }
                else if (circleLayout && fullCircle && dataset == 2) // Q
                {
                    baseDeltaTrialNumber = 1 * taskNo;
                }
                else if (!circleLayout && dataset == 2) // F
                {
                    baseDeltaTrialNumber = 2 * taskNo;
                }
                else if (circleLayout && !fullCircle && dataset == 1) // H
                {
                    baseDeltaTrialNumber = 3 * taskNo;
                }
                else if (circleLayout && fullCircle && dataset == 1) // Q
                {
                    baseDeltaTrialNumber = 4 * taskNo;
                }
                else if (!circleLayout && dataset == 1) // F
                {
                    baseDeltaTrialNumber = 5 * taskNo;
                }
                break;
            default:
                break;
        }

        finalTrialNumber = baseDeltaTrialNumber + deltaTrialNumber;

        return finalTrialNumber.ToString();
    }

    string VectorToString(Vector3 v) {
        string text;
        text = v.x + "," + v.y + "," + v.z;
        return text;
    }

    void FixedPositionCondition() {
        if (fixedPosition)
        {
            // change controller button color
            if (GameObject.Find("Controller (left)") != null) {
                GameObject.Find("Controller (left)").transform.Find("Model").GetComponent<ControllerColor>().fixedPosition = true;
                if (interactionTrainingNeeded && interactionTrainingCount != 0) {
                    GameObject.Find("Controller (left)").transform.Find("TrackPadLabel").GetComponent<TextMeshPro>().text = "Next";
                }
            }
            if (GameObject.Find("Controller (right)") != null) {
                GameObject.Find("Controller (right)").transform.Find("Model").GetComponent<ControllerColor>().fixedPosition = true;
                if (interactionTrainingNeeded && interactionTrainingCount != 0)
                {
                    GameObject.Find("Controller (right)").transform.Find("TrackPadLabel").GetComponent<TextMeshPro>().text = "Next";
                }
            }
        }
        else
        {
            // change controller button color
            if (GameObject.Find("Controller (left)") != null)
            {
                GameObject.Find("Controller (left)").transform.Find("Model").GetComponent<ControllerColor>().fixedPosition = false;
            }
            if (GameObject.Find("Controller (right)") != null)
            {
                GameObject.Find("Controller (right)").transform.Find("Model").GetComponent<ControllerColor>().fixedPosition = false;
            }
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
                    zValue = Mathf.Sin(index * Mathf.PI / (shelfItemPerRow / 2)) * ((shelfItemPerRow - 1) * delta / (2 * Mathf.PI));
                    xValue = -Mathf.Cos(index * Mathf.PI / (shelfItemPerRow / 2)) * ((shelfItemPerRow - 1) * delta / (2 * Mathf.PI));
                }
                else
                {
                    if (index < shelfItemPerRow * 2) // second row
                    {
                        yValue = (shelfRows - 2) * vDelta;
                        zValue = Mathf.Sin((index - shelfItemPerRow) * Mathf.PI / (shelfItemPerRow / 2)) * ((shelfItemPerRow - 1) * delta / (2 * Mathf.PI));
                        xValue = -Mathf.Cos((index - shelfItemPerRow) * Mathf.PI / (shelfItemPerRow / 2)) * ((shelfItemPerRow - 1) * delta / (2 * Mathf.PI));
                    }
                    else
                    {
                        if (index < shelfItemPerRow * 3) // third row
                        {
                            yValue = (shelfRows - 3) * vDelta;
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
        if (dataset == 1) {
            for (int i = 0; i < smallMultiplesNumber; i++)
            {
                GameObject dataObj;

                dataObj = (GameObject)Instantiate(DataPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                dataObj.name = "Small Multiples " + (i + 1);

                dataObj.transform.SetParent(shelf.transform);

                dataObj.transform.localPosition = AssignSMPositionBasedOnLayout(i);
                dataObj.transform.localEulerAngles = new Vector3(0, dataObj.transform.localEulerAngles.y, 0);

                if (circleLayout) {
                    GameObject centerP1 = new GameObject();
                    centerP1.transform.SetParent(shelf.transform);
                    centerP1.transform.localPosition = Vector3.zero;

                    GameObject centerP2 = new GameObject();
                    centerP2.transform.SetParent(shelf.transform);
                    centerP2.transform.localPosition = Vector3.zero + Vector3.up * delta;

                    GameObject centerP3 = new GameObject();
                    centerP3.transform.SetParent(shelf.transform);
                    centerP3.transform.localPosition = Vector3.zero + Vector3.up * delta * 2;

                    if (i < shelfItemPerRow)
                    {
                        dataObj.transform.LookAt(centerP3.transform.position);
                    }
                    else
                    {
                        if (i < shelfItemPerRow * 2)
                        {
                            dataObj.transform.LookAt(centerP2.transform.position);
                        }
                        else
                        {
                            dataObj.transform.LookAt(centerP1.transform.position);
                        }
                    }
                    dataObj.transform.localEulerAngles += Vector3.up * 180;
                    Destroy(centerP1);
                    Destroy(centerP2);
                    Destroy(centerP3);
                }
                

                dataSM.Add(dataObj);

                dataObj.transform.localScale = Vector3.one * 0.8f;

                string tooltipText = "";

                if (dateType == DateType.Monthly)
                {
                    tooltipText = IntToMonth(i + 1);
                }
                else if (dateType == DateType.Fornightly)
                {
                    tooltipText = (i + 1) + ToOrdinal(i + 1) + " two weeks";
                }
                else
                {
                    tooltipText = (i + 1) + ToOrdinal(i + 1) + " week";
                }

                Transform tooltip = dataObj.transform.GetChild(7);
                tooltip.gameObject.name = "Tooltip " + (i + 1);
                smToolTips.Add(tooltip.gameObject);
                VRTK_ObjectTooltip ot = tooltip.GetComponent<VRTK_ObjectTooltip>();
                ot.displayText = tooltipText;

                minXFilters.Add(dataObj.transform.GetChild(1).GetChild(1).GetChild(2).gameObject);
                minXFilters.Add(dataObj.transform.GetChild(4).GetChild(1).GetChild(2).gameObject);

                maxXFilters.Add(dataObj.transform.GetChild(1).GetChild(1).GetChild(3).gameObject);
                maxXFilters.Add(dataObj.transform.GetChild(4).GetChild(1).GetChild(3).gameObject);

                minZFilters.Add(dataObj.transform.GetChild(3).GetChild(1).GetChild(2).gameObject);
                minZFilters.Add(dataObj.transform.GetChild(6).GetChild(1).GetChild(2).gameObject);

                maxZFilters.Add(dataObj.transform.GetChild(3).GetChild(1).GetChild(3).gameObject);
                maxZFilters.Add(dataObj.transform.GetChild(6).GetChild(1).GetChild(3).gameObject);

                minYFilters.Add(dataObj.transform.GetChild(2).GetChild(1).GetChild(2).gameObject);
                minYFilters.Add(dataObj.transform.GetChild(5).GetChild(1).GetChild(2).gameObject);

                maxYFilters.Add(dataObj.transform.GetChild(2).GetChild(1).GetChild(3).gameObject);
                maxYFilters.Add(dataObj.transform.GetChild(5).GetChild(1).GetChild(3).gameObject);
            }
            VRTK_ObjectTooltip ott = colorScheme.GetComponent<VRTK_ObjectTooltip>();
            ott.alwaysFaceHeadset = false;
            colorScheme.transform.localEulerAngles = new Vector3(-60f, 180, 0);

            if (circleLayout)
            {
                if (fullCircle)
                {
                    colorScheme.transform.localPosition = Vector3.zero - Vector3.up * 0.13f + Vector3.forward * 0.66f;
                }
                else
                {
                    colorScheme.transform.localPosition = Vector3.zero + Vector3.forward * 2f - Vector3.up * 0.13f;
                }
            }
            else
            {
                colorScheme.transform.localPosition = Vector3.zero + Vector3.back * 0.5f - Vector3.up * 0.13f;
            }

            foreach (GameObject go in dataSM) {
                BuildingScript bs = go.transform.GetChild(0).gameObject.GetComponent<BuildingScript>();
                if (!bs.IsExploded())
                {
                    bs.explode();
                }
            }
        }
       
        if (dataset == 2) {
            GameObject barChartManager = GameObject.Find("BarChartManagement");

            for (int i = 0; i < smallMultiplesNumber; i++)
            {
                GameObject dataObj = new GameObject();
                barChartManager.transform.GetChild(0).SetParent(dataObj.transform);
                dataObj.name = "Small Multiples " + (i + 1);
                dataObj.transform.localScale = new Vector3(d2Scale, d2Scale, d2Scale);
                dataObj.transform.GetChild(0).localPosition = new Vector3(-0.5f, 0.3f, -0.5f);
                dataObj.AddComponent<PositionLocalConstraints>();
                dataObj.AddComponent<BoxCollider>();
                dataObj.GetComponent<BoxCollider>().center = new Vector3(0,0.8f,0);
                dataObj.GetComponent<BoxCollider>().size = new Vector3(1.8f, 1.6f, 1.8f);

                //setup tooltip
                GameObject tooltip = (GameObject)Instantiate(tooltipPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                tooltip.gameObject.name = "Tooltip " + (i + 1);
                tooltip.transform.SetParent(dataObj.transform);
                tooltip.transform.localPosition = new Vector3(-1f, 0.8f, 0);
                tooltip.transform.localEulerAngles= new Vector3(0, 0, 90);

                smToolTips.Add(tooltip);

                VRTK_ObjectTooltip tt = tooltip.GetComponent<VRTK_ObjectTooltip>();
                tt.containerSize = new Vector2(300, 60);
                tt.fontSize = 24;
                tt.displayText = GetBarChartName(i + 1);
                tt.alwaysFaceHeadset = false;


                dataObj.transform.SetParent(shelf.transform);
                dataObj.transform.localPosition = AssignSMPositionBasedOnLayout(i);
                dataSM.Add(dataObj);

                GameObject leftCountryFilter = (GameObject)Instantiate(FilterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                leftCountryFilter.name = "leftCountryFilter";   
                leftCountryFilter.transform.SetParent(dataObj.transform.GetChild(0).GetChild(1).GetChild(1));
                SetColorForFilter(leftCountryFilter, 1);
                leftCountryFilter.transform.localPosition = new Vector3(-3, 0, 0);
                leftCountryFilter.transform.localScale = new Vector3(0.05f, 3, 1);
                leftCountryFilter.transform.localEulerAngles = new Vector3(0, 0, -90);
                leftCountryFilters.Add(leftCountryFilter);

                leftCountryFilter = (GameObject)Instantiate(FilterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                leftCountryFilter.name = "leftCountryFilter";
                leftCountryFilter.transform.SetParent(dataObj.transform.GetChild(0).GetChild(4).GetChild(1));
                SetColorForFilter(leftCountryFilter, 1);
                leftCountryFilter.transform.localPosition = new Vector3(-3, 0, 0);
                leftCountryFilter.transform.localScale = new Vector3(0.05f, 3, 1);
                leftCountryFilter.transform.localEulerAngles = new Vector3(0, 0, -90);
                leftCountryFilters.Add(leftCountryFilter);

                GameObject rightCountryFilter = (GameObject)Instantiate(FilterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                rightCountryFilter.name = "rightCountryFilter";
                rightCountryFilter.transform.SetParent(dataObj.transform.GetChild(0).GetChild(1).GetChild(1));               
                SetColorForFilter(rightCountryFilter, 1);
                rightCountryFilter.transform.localPosition = new Vector3(-3, 1, 0);
                rightCountryFilter.transform.localScale = new Vector3(0.05f, 3, 1);
                rightCountryFilter.transform.localEulerAngles = new Vector3(0, 0, -90);
                rightCountryFilters.Add(rightCountryFilter);

                rightCountryFilter = (GameObject)Instantiate(FilterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                rightCountryFilter.name = "rightCountryFilter";
                rightCountryFilter.transform.SetParent(dataObj.transform.GetChild(0).GetChild(4).GetChild(1));               
                SetColorForFilter(rightCountryFilter, 1);
                rightCountryFilter.transform.localPosition = new Vector3(-3, 1, 0);
                rightCountryFilter.transform.localScale = new Vector3(0.05f, 3, 1);
                rightCountryFilter.transform.localEulerAngles = new Vector3(0, 0, -90);
                rightCountryFilters.Add(rightCountryFilter);

                GameObject leftYearFilter = (GameObject)Instantiate(FilterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                leftYearFilter.name = "leftYearFilter";
                leftYearFilter.transform.SetParent(dataObj.transform.GetChild(0).GetChild(3).GetChild(1));               
                SetColorForFilter(leftYearFilter, 1);
                leftYearFilter.transform.localPosition = new Vector3(-3, 0, 0);
                leftYearFilter.transform.localScale = new Vector3(0.05f, 3, 1);
                leftYearFilter.transform.localEulerAngles = new Vector3(0, 0, -90);
                leftYearFilters.Add(leftYearFilter);

                leftYearFilter = (GameObject)Instantiate(FilterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                leftYearFilter.name = "leftYearFilter";
                leftYearFilter.transform.SetParent(dataObj.transform.GetChild(0).GetChild(6).GetChild(1));               
                SetColorForFilter(leftYearFilter, 1);
                leftYearFilter.transform.localPosition = new Vector3(-3, 0, 0);
                leftYearFilter.transform.localScale = new Vector3(0.05f, 3, 1);
                leftYearFilter.transform.localEulerAngles = new Vector3(0, 0, -90);
                leftYearFilters.Add(leftYearFilter);

                GameObject rightYearFilter = (GameObject)Instantiate(FilterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                rightYearFilter.name = "rightYearFilter";
                rightYearFilter.transform.SetParent(dataObj.transform.GetChild(0).GetChild(3).GetChild(1));
                SetColorForFilter(rightYearFilter, 1);
                rightYearFilter.transform.localPosition = new Vector3(-3, 1, 0);
                rightYearFilter.transform.localScale = new Vector3(0.05f, 3, 1);
                rightYearFilter.transform.localEulerAngles = new Vector3(0, 0, -90);
                rightYearFilters.Add(rightYearFilter);

                rightYearFilter = (GameObject)Instantiate(FilterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                rightYearFilter.name = "rightYearFilter";
                rightYearFilter.transform.SetParent(dataObj.transform.GetChild(0).GetChild(6).GetChild(1));
                SetColorForFilter(rightYearFilter, 1);
                rightYearFilter.transform.localPosition = new Vector3(-3, 1, 0);
                rightYearFilter.transform.localScale = new Vector3(0.05f, 3, 1);
                rightYearFilter.transform.localEulerAngles = new Vector3(0, 0, -90);
                rightYearFilters.Add(rightYearFilter);

                GameObject leftValueFilter = (GameObject)Instantiate(FilterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                leftValueFilter.name = "leftValueFilter";
                leftValueFilter.transform.SetParent(dataObj.transform.GetChild(0).GetChild(2).GetChild(1));
                SetColorForFilter(leftValueFilter, 1);
                leftValueFilter.transform.localPosition = new Vector3(-3, 0, 0);
                leftValueFilter.transform.localScale = new Vector3(0.05f, 3, 1);
                leftValueFilter.transform.localEulerAngles = new Vector3(0, 0, -90);
                leftValueFilters.Add(leftValueFilter);

                leftValueFilter = (GameObject)Instantiate(FilterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                leftValueFilter.name = "leftValueFilter";
                leftValueFilter.transform.SetParent(dataObj.transform.GetChild(0).GetChild(5).GetChild(1));
                SetColorForFilter(leftValueFilter, 1);
                leftValueFilter.transform.localPosition = new Vector3(3, 0, 0);
                leftValueFilter.transform.localScale = new Vector3(0.05f, 3, 1);
                leftValueFilter.transform.localEulerAngles = new Vector3(0, 0, 90);
                leftValueFilters.Add(leftValueFilter);
                GameObject tmp = new GameObject();
                tmp.transform.SetParent(leftValueFilter.transform);

                GameObject rightValueFilter = (GameObject)Instantiate(FilterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                rightValueFilter.name = "rightValueFilter";
                rightValueFilter.transform.SetParent(dataObj.transform.GetChild(0).GetChild(2).GetChild(1));
                SetColorForFilter(rightValueFilter, 1);
                rightValueFilter.transform.localPosition = new Vector3(-3, 1, 0);
                rightValueFilter.transform.localScale = new Vector3(0.05f, 3, 1);
                rightValueFilter.transform.localEulerAngles = new Vector3(0, 0, -90);
                rightValueFilters.Add(rightValueFilter);

                rightValueFilter = (GameObject)Instantiate(FilterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                rightValueFilter.name = "rightValueFilter";
                rightValueFilter.transform.SetParent(dataObj.transform.GetChild(0).GetChild(5).GetChild(1));
                SetColorForFilter(rightValueFilter, 1);
                rightValueFilter.transform.localPosition = new Vector3(3, 1, 0);
                rightValueFilter.transform.localScale = new Vector3(0.05f, 3, 1);
                rightValueFilter.transform.localEulerAngles = new Vector3(0, 0, 90);
                rightValueFilters.Add(rightValueFilter);
                GameObject tmp2 = new GameObject();
                tmp2.transform.SetParent(rightValueFilter.transform);

                GameObject leftValuePlane = (GameObject)Instantiate(CuttingPlanePrefab, new Vector3(0, 0, 0), Quaternion.identity);
                leftValuePlane.name = "leftValuePlane";
                leftValuePlane.transform.SetParent(dataObj.transform.GetChild(0).GetChild(2).GetChild(1));
                leftValuePlane.transform.localPosition = Vector3.zero;
                leftValuePlane.transform.localScale = Vector3.one;
                leftValuePlane.transform.localEulerAngles = Vector3.zero;
                SetColorForFilter(leftValuePlane.transform.GetChild(0).gameObject, 0);
                leftValuePlanes.Add(leftValuePlane);

                GameObject rightValuePlane = (GameObject)Instantiate(CuttingPlanePrefab, new Vector3(0, 0, 0), Quaternion.identity);
                rightValuePlane.name = "rightValuePlane";
                rightValuePlane.transform.SetParent(dataObj.transform.GetChild(0).GetChild(2).GetChild(1));
                SetColorForFilter(rightValuePlane.transform.GetChild(0).gameObject, 0);
                rightValuePlane.transform.localPosition = Vector3.zero;
                rightValuePlane.transform.localScale = Vector3.one;
                rightValuePlane.transform.localEulerAngles = Vector3.zero;
                rightValuePlanes.Add(rightValuePlane);
            }

            GetChessBoardDic();
        }
    }

    private void GetChessBoardDic()
    {
        if (chessBoardPoints != null && dataSM != null) {
            if (dataSM.Count > 0 && chessBoardPoints.Count != dataSM.Count)
            {
                foreach (GameObject sm in dataSM)
                {
                    Transform barChart = sm.transform.GetChild(0);
                    chessBoardPoints.Add(barChart.name, GetChessBoardPoint(barChart));
                }
            }
        }  
    }

    private Dictionary<Vector2, Vector3> GetChessBoardPoint(Transform barChart)
    {
        
        BigMesh bm = barChart.GetChild(0).GetChild(0).GetComponent<BigMesh>();

        Dictionary<Vector2, Vector3> points = new Dictionary<Vector2, Vector3>();

        float sizeDelta = 0.111f;

        for (int xDelta = 1; xDelta <= 10; xDelta++)
        {
            for (int zDelta = 1; zDelta <= 10; zDelta++)
            {
                Vector3 vertice = barChart.GetChild(0).GetChild(0).TransformPoint(bm.getBigMeshVertices()[(xDelta - 1) * 10 + (zDelta - 1)] + Vector3.up * 0.056f);
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
                        if (dataset == 1)
                            RefreshDataSet1();
                        else if (dataset == 2)
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
                        if (dataset == 1)
                            RefreshDataSet1();
                        else if (dataset == 2)
                            RefreshDataSet2();
                        rightClickEmptySpace = false;
                    }
                }
            }
        }
        
    }

    public void CheckDataset1KeepHighted(string controller, Vector3 touchPointPosition)
    {
        //Debug.Log("controller OK");

        if (!colorFilterOn)
            colorFilterSelected = false;
        else
            colorFilterOn = false;
        GameObject lController = GameObject.Find("Controller (left)");
        Transform gridLayoutChoices = null;
        if (lController != null)
        {
            gridLayoutChoices = lController.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(0).GetChild(1);
        }
        if (gridLayoutChoices != null)
        {
            for (int i = 1; i <= 9; i++)
            {
                gridLayoutChoices.GetChild(i).GetChild(0).GetComponent<Text>().color = Color.white;
            }
        }

        if (!creatingCube && !creatingWorldInMiniture)
        {
            if (controller == "left")
            {
                //rightPressedCount = 0;
                if (leftFindHighlighedAxisFromCollision) // axis brushing for BIM
                {
                    //Debug.Log("left Axis OK");
                    leftHighlighed = false;
                    rightHighlighed = false;

                    leftAxisHighlighed = true;

                    if (leftFindHighlighedV2FromCollisionBIM.x != -1) {
                        SetFilterPositionBIM(0, 1, -1, -1);
                    }
                    if (leftFindHighlighedV2FromCollisionBIM.y != -1)
                    {
                        SetFilterPositionBIM(-1, -1, 0, 1);
                    }

                    leftFindHighlighedV2FromCollisionBIM = leftFindHoveringV2FromCollisionBIM;

                    if (leftFindHighlighedV2FromCollisionBIM.x != -1)
                    {
                        if (rightFindHighlighedV2FromCollisionBIM.x != -1)
                        {
                            SetFilterPositionBIM(Mathf.Min(leftFindHighlighedV2FromCollisionBIM.x, rightFindHighlighedV2FromCollisionBIM.x) - 0.04f, 
                                Mathf.Max(leftFindHighlighedV2FromCollisionBIM.x, rightFindHighlighedV2FromCollisionBIM.x) + 0.04f, -1, -1);
                        }
                        else {
                            SetFilterPositionBIM(leftFindHighlighedV2FromCollisionBIM.x - 0.04f, leftFindHighlighedV2FromCollisionBIM.x + 0.04f, -1, -1);
                        }
                    }
                    else if (leftFindHighlighedV2FromCollisionBIM.y != -1)
                    {
                        if (rightFindHighlighedV2FromCollisionBIM.y != -1)
                        {
                            SetFilterPositionBIM(-1, -1, Mathf.Min(leftFindHighlighedV2FromCollisionBIM.y, rightFindHighlighedV2FromCollisionBIM.y) - 0.04f,
                                Mathf.Max(leftFindHighlighedV2FromCollisionBIM.y, rightFindHighlighedV2FromCollisionBIM.y) + 0.04f);
                        }
                        else {
                            SetFilterPositionBIM(-1, -1, leftFindHighlighedV2FromCollisionBIM.y - 0.04f, leftFindHighlighedV2FromCollisionBIM.y + 0.04f);
                        }
                    }
                    else
                    {
                        Debug.Log(leftFindHighlighedV2FromCollisionBIM);
                    }
                }
                else
                {
                    if (leftFindHighlighedSensorFromCollision) // single brushing for BIM
                    {
                        if (!leftHighlighed)
                        {
                            leftHighlighed = true;
                            leftKeepHighlightedGO = leftFindHighlightedGO;
                            rightHighlighed = false;
                            rightKeepHighlightedGO = null;
                        }
                        else
                        {
                            leftKeepHighlightedGO = leftFindHighlightedGO;
                        }
                        // set filters
                        Transform GOParent = leftKeepHighlightedGO.transform.parent.parent.parent.parent;
                       
                        SetFilterPositionBIM(GOParent.GetChild(1).InverseTransformPoint(leftKeepHighlightedGO.transform.position).y - 0.04f,
                            GOParent.GetChild(1).InverseTransformPoint(leftKeepHighlightedGO.transform.position).y + 0.04f,
                            GOParent.GetChild(3).InverseTransformPoint(leftKeepHighlightedGO.transform.position).y - 0.04f,
                            GOParent.GetChild(3).InverseTransformPoint(leftKeepHighlightedGO.transform.position).y + 0.04f);
                    }
                    else // empty space clicking to refresh
                    {
                        bool inSM = false;
                        foreach (GameObject go in dataSM)
                        {
                            if (go.transform.InverseTransformPoint(touchPointPosition).x >= -0.26f && go.transform.InverseTransformPoint(touchPointPosition).x <= 0.26f && 
                                go.transform.InverseTransformPoint(touchPointPosition).z >= -0.26f && go.transform.InverseTransformPoint(touchPointPosition).z <= 0.26f && 
                                go.transform.InverseTransformPoint(touchPointPosition).y >= 0 && go.transform.InverseTransformPoint(touchPointPosition).y <= 0.26f)
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
                            //    //RefreshDataSet1();
                            //}
                            //Debug.Log("l???");
                            leftClickEmptySpace = true;
                        }
                    }
                }
            }
            if (controller == "right")
            {
                //leftPressedCount = 0;
                if (rightFindHighlighedAxisFromCollision) // axis brushing for BIM
                {
                    //Debug.Log("right Axis OK");
                    leftHighlighed = false;
                    rightHighlighed = false;

                    rightAxisHighlighed = true;

                    if (rightFindHighlighedV2FromCollisionBIM.x != -1)
                    {
                        SetFilterPositionBIM(0, 1, -1, -1);
                    }
                    if (rightFindHighlighedV2FromCollisionBIM.y != -1)
                    {
                        SetFilterPositionBIM(-1, -1, 0, 1);
                    }
                    rightFindHighlighedV2FromCollisionBIM = rightFindHoveringV2FromCollisionBIM;

                    if (rightFindHighlighedV2FromCollisionBIM.x != -1)
                    {
                        if (leftFindHighlighedV2FromCollisionBIM.x != -1)
                        {
                            SetFilterPositionBIM(Mathf.Min(leftFindHighlighedV2FromCollisionBIM.x, rightFindHighlighedV2FromCollisionBIM.x) - 0.04f, 
                                Mathf.Max(leftFindHighlighedV2FromCollisionBIM.x, rightFindHighlighedV2FromCollisionBIM.x) + 0.04f, -1, -1);
                        }
                        else
                        {
                            SetFilterPositionBIM(rightFindHighlighedV2FromCollisionBIM.x - 0.04f, rightFindHighlighedV2FromCollisionBIM.x + 0.04f, -1, -1);
                        }
                    }
                    else if (rightFindHighlighedV2FromCollisionBIM.y != -1)
                    {
                        if (leftFindHighlighedV2FromCollisionBIM.y != -1)
                        {
                            SetFilterPositionBIM(-1, -1, Mathf.Min(leftFindHighlighedV2FromCollisionBIM.y, rightFindHighlighedV2FromCollisionBIM.y) - 0.04f, 
                                Mathf.Max(leftFindHighlighedV2FromCollisionBIM.y, rightFindHighlighedV2FromCollisionBIM.y) + 0.04f);
                        }
                        else
                        {
                            SetFilterPositionBIM(-1, -1, rightFindHighlighedV2FromCollisionBIM.y - 0.04f, rightFindHighlighedV2FromCollisionBIM.y + 0.04f);
                        }
                    }
                    else
                    {
                        Debug.Log(rightFindHighlighedV2FromCollisionBIM);
                    }
                }
                else
                {
                    if (rightFindHighlighedSensorFromCollision) // single brushing for BIM
                    {
                        if (!rightHighlighed)
                        {
                            rightHighlighed = true;
                            rightKeepHighlightedGO = rightFindHighlightedGO;
                            leftHighlighed = false;
                            leftKeepHighlightedGO = null;
                        }
                        else
                        {
                            rightKeepHighlightedGO = rightFindHighlightedGO;
                        }
                        // set filters
                        Transform GOParent = rightKeepHighlightedGO.transform.parent.parent.parent.parent;
                        SetFilterPositionBIM(GOParent.GetChild(1).InverseTransformPoint(rightKeepHighlightedGO.transform.position).y - 0.04f,
                            GOParent.GetChild(1).InverseTransformPoint(rightKeepHighlightedGO.transform.position).y + 0.04f,
                            GOParent.GetChild(3).InverseTransformPoint(rightKeepHighlightedGO.transform.position).y - 0.04f,
                            GOParent.GetChild(3).InverseTransformPoint(rightKeepHighlightedGO.transform.position).y + 0.04f);
                    }
                    else // empty space clicking to refresh
                    {
                        bool inSM = false;
                        foreach (GameObject go in dataSM)
                        {
                            if (go.transform.InverseTransformPoint(touchPointPosition).x >= -0.26f && go.transform.InverseTransformPoint(touchPointPosition).x <= 0.26f &&
                                go.transform.InverseTransformPoint(touchPointPosition).z >= -0.26f && go.transform.InverseTransformPoint(touchPointPosition).z <= 0.26f &&
                                go.transform.InverseTransformPoint(touchPointPosition).y >= 0 && go.transform.InverseTransformPoint(touchPointPosition).y <= 0.26f)
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
                            //    RefreshDataSet1();
                            //}
                            //Debug.Log("r???");
                            rightClickEmptySpace = true;
                        }
                    }
                }
            }
        }
    }

    private void RefreshDataSet1() {
        leftHighlighed = false;
        rightHighlighed = false;
        leftAxisHighlighed = false;
        rightAxisHighlighed = false;
        if(!colorFilterOn)
            colorSelectedIndex = 0;

        leftFindHighlighedV2FromCollisionBIM = new Vector2(-1, -1);
        rightFindHighlighedV2FromCollisionBIM = new Vector2(-1, -1);

        if (GameObject.Find("leftCollisionDetector") != null && GameObject.Find("rightCollisionDetector") != null) {
            GameObject.Find("leftCollisionDetector").GetComponent<CollisionDetection>().BIMlvlIndex = 0;
            GameObject.Find("rightCollisionDetector").GetComponent<CollisionDetection>().BIMlvlIndex = 0;
        }

        highlightedSensorsFromMovingFilters = allSensors;

        foreach (GameObject sm in dataSM)
        {
            if (sm.transform.Find("CubeSelection") != null)
            {
                Destroy(sm.transform.Find("CubeSelection").gameObject);
            }
        }

        // reset filter buttons for all axis

        foreach (GameObject filter in minXFilters)
        {
            filter.transform.localPosition = new Vector3(2, 0, 0);
        }
        currentXMinFilterPosition = 0;

        foreach (GameObject filter in maxXFilters)
        {
            filter.transform.localPosition = new Vector3(2, 1, 0);
        }
        currentXMaxFilterPosition = 1;

        foreach (GameObject filter in minYFilters)
        {
            filter.transform.localPosition = new Vector3(2, 0, 0);
        }
        currentYMinFilterPosition = 0;

        foreach (GameObject filter in maxYFilters)
        {
            filter.transform.localPosition = new Vector3(2, 1, 0);
        }
        currentYMaxFilterPosition = 2;

        foreach (GameObject filter in minZFilters)
        {
            filter.transform.localPosition = new Vector3(2, 0, 0);
        }
        currentZMinFilterPosition = 0;

        foreach (GameObject filter in maxZFilters)
        {
            filter.transform.localPosition = new Vector3(2, 1, 0);
        }
        currentZMaxFilterPosition = 1;
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
        chessBoardBrushingBool = new bool[100];
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
        GameObject leftTouchbar = null;
        if (GameObject.Find("Controller (left)") != null)
        {
            Transform leftController = GameObject.Find("Controller (left)").transform;
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

            foreach (KeyValuePair<string, Dictionary<Vector2, Vector3>> entry in chessBoardPoints)
            {
                Transform barChart = GameObject.Find(entry.Key).transform;
                if(barChart.InverseTransformPoint(leftB.position).x >= -0.06f && barChart.InverseTransformPoint(leftB.position).x <= 1.056f && barChart.InverseTransformPoint(leftB.position).z >= -0.06f && barChart.InverseTransformPoint(leftB.position).z <= 1.056f && barChart.InverseTransformPoint(leftB.position).y >= 0 && barChart.InverseTransformPoint(leftB.position).y <= 1.056f)
                //if (CheckDiff(leftB.position.x, barChart.position.x, 0.4f, false) && CheckDiff(leftB.position.z, barChart.position.z, 0.4f, false))
                {
                    foreach (KeyValuePair<Vector2, Vector3> secondEntry in entry.Value)
                    {
                        Transform yAxis = barChart.GetChild(2).GetChild(1);
                        Vector3 tilesLocalToWorld = barChart.parent.TransformPoint(secondEntry.Value);
                        Vector3 tilesLocalToWorldY = yAxis.TransformPoint(secondEntry.Value);
                        //Debug.Log((CheckDiff(leftB.position.x, tilesLocalToWorld.x, 0.02f, true)) + " " +  (CheckDiff(leftB.position.z, tilesLocalToWorld.z, 0.02f, true)) + " " +  (leftB.position.y >= barChart.position.y) + " " + (leftB.position.y <= tilesLocalToWorld.y + 0.01f));
                        if (CheckDiff(leftB.position.x, tilesLocalToWorld.x, 0.02f, true) && CheckDiff(leftB.position.z, tilesLocalToWorld.z, 0.02f, true) && leftB.position.y >= barChart.position.y && leftB.position.y <= tilesLocalToWorldY.y + 0.01f)
                        {
                            x = (int)secondEntry.Key.x;
                            z = (int)secondEntry.Key.y;
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
        GameObject rightTouchbar = null;
        if (GameObject.Find("Controller (right)") != null)
        {
            Transform rightController = GameObject.Find("Controller (right)").transform;
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

            foreach (KeyValuePair<string, Dictionary<Vector2, Vector3>> entry in chessBoardPoints)
            {
                Transform barChart = GameObject.Find(entry.Key).transform;
                

                if (barChart.InverseTransformPoint(rightB.position).x >= -0.06f && barChart.InverseTransformPoint(rightB.position).x <= 1.056f && barChart.InverseTransformPoint(rightB.position).z >= -0.06f && barChart.InverseTransformPoint(rightB.position).z <= 1.056f && barChart.InverseTransformPoint(rightB.position).y >= 0 && barChart.InverseTransformPoint(rightB.position).y <= 1.056f)
                //if (CheckDiff(rightB.position.x, barChart.position.x, 0.4f, false) && CheckDiff(rightB.position.z, barChart.position.z, 0.4f, false))
                {
                    foreach (KeyValuePair<Vector2, Vector3> secondEntry in entry.Value)
                    {
                        Transform yAxis = barChart.GetChild(2).GetChild(1);
                        Vector3 tilesLocalToWorld = barChart.parent.TransformPoint(secondEntry.Value);
                        Vector3 tilesLocalToWorldY = yAxis.TransformPoint(secondEntry.Value);
                        //Debug.Log((CheckDiff(rightB.position.x, tilesLocalToWorld.x, 0.02f, true)) + " " + (CheckDiff(rightB.position.z, tilesLocalToWorld.z, 0.02f, true)) + " " + (rightB.position.y >= barChart.position.y) + " " + (rightB.position.y <= tilesLocalToWorld.y + 0.01f));
                        if (CheckDiff(rightB.position.x, tilesLocalToWorld.x, 0.02f, true) && CheckDiff(rightB.position.z, tilesLocalToWorld.z, 0.02f, true) && rightB.position.y >= barChart.position.y && rightB.position.y <= tilesLocalToWorldY.y + 0.01f)
                        {
                            x = (int)secondEntry.Key.x;
                            z = (int)secondEntry.Key.y;
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
                    filter.transform.localPosition = new Vector3(-3, position / 10f, 0);
                }
                currentCountryLeftFilterPosition = (int)position;
                leftFindHighlighedV2FromCollision = new Vector2(currentCountryLeftFilterPosition + 1, leftFindHighlighedV2FromCollision.y);
            }
            else
            {
                foreach (GameObject filter in rightCountryFilters)
                {
                    filter.transform.localPosition = new Vector3(-3, position / 10f, 0);
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
                    filter.transform.localPosition = new Vector3(-3, position / 10f, 0);
                }
                currentYearLeftFilterPosition = (int)position;
                leftFindHighlighedV2FromCollision = new Vector2(leftFindHighlighedV2FromCollision.x, currentYearLeftFilterPosition + 1);
            }
            else
            {
                foreach (GameObject filter in rightYearFilters)
                {
                    filter.transform.localPosition = new Vector3(-3, position / 10f, 0);
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
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (chessBoardPoints[sm.transform.GetChild(0).name][new Vector2(i + 1, j + 1)].y < minFilter.localPosition.y)
                        {
                            filteredChessBoardBrushingBool[i * 10 + j] = false;
                        }
                    }
                }
            }

            if (maxFilter.localPosition.y != 1)
            {
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (chessBoardPoints[sm.transform.GetChild(0).name][new Vector2(i + 1, j + 1)].y > maxFilter.localPosition.y)
                        {
                            filteredChessBoardBrushingBool[i * 10 + j] = false;
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
        chessBoardBrushingBool = new bool[100];

        int leftCountryBrushControl = (int)input.x;
        int rightCountryBrushControl = (int)input.y;
        int leftYearBrushControl = (int)input.z;
        int rightYearBrushControl = (int)input.w;

        if (mode == "single")
        {
            if (leftYearBrushControl != 0 && leftCountryBrushControl != 0)
            {
                for (int i = 1; i <= 10; i++)
                {
                    for (int j = 1; j <= 10; j++)
                    {
                        if (i == leftCountryBrushControl && j == leftYearBrushControl)
                        {
                            chessBoardBrushingBool[(i - 1) * 10 + (j - 1)] = true;
                        }
                        else
                        {
                            chessBoardBrushingBool[(i - 1) * 10 + (j - 1)] = false;
                        }
                    }
                }
            }

            if (rightYearBrushControl != 0 && rightCountryBrushControl != 0)
            {
                for (int i = 1; i <= 10; i++)
                {
                    for (int j = 1; j <= 10; j++)
                    {
                        if (i == rightCountryBrushControl && j == rightYearBrushControl)
                        {
                            chessBoardBrushingBool[(i - 1) * 10 + (j - 1)] = true;
                        }
                        else
                        {
                            chessBoardBrushingBool[(i - 1) * 10 + (j - 1)] = false;
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
                SetFilterPosition(0, 10, 0, 10);
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
                chessBoardBrushingBool[10 * (leftCountryBrushControl - 1) + (rightYearBrushControl - 1)] = true;
                SetFilterPosition(leftCountryBrushControl - 1, leftCountryBrushControl, rightYearBrushControl - 1, rightYearBrushControl);
            }
            else if (leftYearBrushControl != 0 && leftCountryBrushControl == 0 && rightYearBrushControl == 0 && rightCountryBrushControl != 0)  // check intersection
            {
                chessBoardBrushingBool[10 * (rightCountryBrushControl - 1) + (leftYearBrushControl - 1)] = true;
                SetFilterPosition(rightCountryBrushControl - 1, rightCountryBrushControl, leftYearBrushControl - 1, leftYearBrushControl);
            }
            else if (leftYearBrushControl == 0 && leftCountryBrushControl != 0 && rightYearBrushControl == 0 && rightCountryBrushControl != 0) // check parallel
            {
                if (Math.Abs(leftCountryBrushControl - rightCountryBrushControl) != 0)
                {
                    for (int i = Math.Min(leftCountryBrushControl, rightCountryBrushControl); i <= Math.Max(leftCountryBrushControl, rightCountryBrushControl); i++)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                                chessBoardBrushingBool[(i - 1) * 10 + j] = true;
                        }
                    }
                }
                else
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        for (int j = 1; j <= 10; j++)
                        {
                            if (j == leftCountryBrushControl)
                                    chessBoardBrushingBool[(j - 1) * 10 + (i - 1)] = true;
                        }
                    }
                }
                if (leftCountryBrushControl < rightCountryBrushControl)
                {
                    SetFilterPosition(leftCountryBrushControl - 1, rightCountryBrushControl, 0, 10);
                }
                else
                {
                    SetFilterPosition(leftCountryBrushControl, rightCountryBrushControl - 1, 0, 10);
                }

            }
            else if (leftYearBrushControl != 0 && leftCountryBrushControl == 0 && rightYearBrushControl != 0 && rightCountryBrushControl == 0) // check parallel
            {
                if (Math.Abs(leftYearBrushControl - rightYearBrushControl) != 0)
                {
                    for (int i = Math.Min(leftYearBrushControl, rightYearBrushControl); i <= Math.Max(leftYearBrushControl, rightYearBrushControl); i++)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                                chessBoardBrushingBool[j * 10 + (i - 1)] = true;
                        }
                    }
                }
                else
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        for (int j = 1; j <= 10; j++)
                        {
                            if (i == leftYearBrushControl)
                                    chessBoardBrushingBool[(j - 1) * 10 + (i - 1)] = true;
                        }
                    }
                }
                if (leftYearBrushControl < rightYearBrushControl)
                {
                    SetFilterPosition(0, 10, leftYearBrushControl - 1, rightYearBrushControl);
                }
                else
                {
                    SetFilterPosition(0, 10, leftYearBrushControl, rightYearBrushControl - 1);
                }
            }
            else
            {
                if (rightCountryBrushControl == 0 && leftCountryBrushControl != 0)
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        for (int j = 1; j <= 10; j++)
                        {
                            if (i == leftCountryBrushControl)
                                    chessBoardBrushingBool[(i - 1) * 10 + (j - 1)] = true;
                        }
                    }
                    if (leftYearBrushControl == 0 && rightYearBrushControl == 0)
                    {
                        SetFilterPosition(leftCountryBrushControl - 1, leftCountryBrushControl, 0, 10);
                    }
                    else
                    {
                        SetFilterPosition(leftCountryBrushControl - 1, leftCountryBrushControl, -1, -1);
                    }
                }
                else if (leftCountryBrushControl == 0 && rightCountryBrushControl != 0)
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        for (int j = 1; j <= 10; j++)
                        {
                            if (i == rightCountryBrushControl)
                                    chessBoardBrushingBool[(i - 1) * 10 + (j - 1)] = true;
                        }
                    }
                    if (leftYearBrushControl == 0 && rightYearBrushControl == 0)
                    {
                        SetFilterPosition(rightCountryBrushControl - 1, rightCountryBrushControl, 0, 10);
                    }
                    else
                    {
                        SetFilterPosition(rightCountryBrushControl - 1, rightCountryBrushControl, -1, -1);
                    }
                }

                if (rightYearBrushControl == 0 && leftYearBrushControl != 0)
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        for (int j = 1; j <= 10; j++)
                        {
                            if (j == leftYearBrushControl)
                                    chessBoardBrushingBool[(i - 1) * 10 + (j - 1)] = true;
                        }
                    }
                    if (leftCountryBrushControl == 0 && rightCountryBrushControl == 0)
                    {
                        SetFilterPosition(0, 10, leftYearBrushControl - 1, leftYearBrushControl);
                    }
                    else
                    {
                        SetFilterPosition(-1, -1, leftYearBrushControl - 1, leftYearBrushControl);
                    }
                }
                else if (rightYearBrushControl != 0 && leftYearBrushControl == 0)
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        for (int j = 1; j <= 10; j++)
                        {
                            if (j == rightYearBrushControl)
                                    chessBoardBrushingBool[(i - 1) * 10 + (j - 1)] = true;
                        }
                    }
                    if (leftCountryBrushControl == 0 && rightCountryBrushControl == 0)
                    {
                        SetFilterPosition(0, 10, rightYearBrushControl - 1, rightYearBrushControl);
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
                    if (!chessBoardBrushingBool[i * 10 + j])
                    {
                        chessBoardBrushingBool[i * 10 + j] = true;
                    }
                    else
                    {
                        chessBoardBrushingBool[i * 10 + j] = false;
                    }
                }
            }
        }

        finalChessBoardBrushingBool = CalculateFullLogicOfBrushingAndFiltering();
        bcc.UpdateBrushing(finalChessBoardBrushingBool);
    }

    // combine all logic together
    private bool[] CalculateFullLogicOfBrushingAndFiltering() {

        bool[] finalBool = new bool[100];

        for (int i = 0; i < 100; i++) {
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
        for (int i = 0; i < 100; i++)
        {
            chessBoardBrushingBool[i] = true;
        }
    }

    private void ResetFilteredChessBoardBrushingBool() {
        //leftHighlighed = false;
        //rightHighlighed = false;
        //for (int i = 0; i < 100; i++)
        //{
        //    chessBoardBrushingBool[i] = true;
        //}
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
        if (GameObject.Find("Controller (left)") != null)
        {
            Transform leftController = GameObject.Find("Controller (left)").transform;
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
        if (GameObject.Find("Controller (right)") != null)
        {
            Transform rightController = GameObject.Find("Controller (right)").transform;
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
                GameObject.Find("leftCollisionDetector").GetComponent<CollisionDetection>().draggedFilter == null && GameObject.Find("rightCollisionDetector").GetComponent<CollisionDetection>().draggedFilter == null)
            {
                Quaternion currentRotation = Quaternion.identity;
                Vector3 currentScale = Vector3.zero;

                // middle point method

                if (touchBarMiddleSM == null)
                {
                    foreach (GameObject go in dataSM)
                    {
                        if (dataset == 1)
                        {
                            currentScale = go.transform.localScale / 4;
                            if (go.transform.InverseTransformPoint(touchBarMiddlePosition).x >= -0.26f && go.transform.InverseTransformPoint(touchBarMiddlePosition).x <= 0.26f &&
                                go.transform.InverseTransformPoint(touchBarMiddlePosition).z >= -0.26f && go.transform.InverseTransformPoint(touchBarMiddlePosition).z <= 0.26f &&
                                go.transform.InverseTransformPoint(touchBarMiddlePosition).y >= 0 && go.transform.InverseTransformPoint(touchBarMiddlePosition).y <= 0.26f)
                            {
                                touchBarMiddleSM = go.transform;
                            }
                        }
                        else {
                            if (go.transform.GetChild(0).InverseTransformPoint(touchBarMiddlePosition).x >= -0.06f && go.transform.GetChild(0).InverseTransformPoint(touchBarMiddlePosition).x <= 1.056f &&
                                go.transform.GetChild(0).InverseTransformPoint(touchBarMiddlePosition).z >= -0.06f && go.transform.GetChild(0).InverseTransformPoint(touchBarMiddlePosition).z <= 1.056f &&
                                go.transform.GetChild(0).InverseTransformPoint(touchBarMiddlePosition).y >= 0 && go.transform.GetChild(0).InverseTransformPoint(touchBarMiddlePosition).y <= 1.056f)
                            {
                                touchBarMiddleSM = go.transform;
                            }
                            currentScale = go.transform.localScale;
                        }
                        
                        currentRotation = go.transform.rotation;


                    }
                }


                if (touchBarMiddleSM != null || cubeSelectionCube != null)
                {
                    Destroy(worldInMiniture);
                    worldInMiniture = null;

                    if (!creatingCube && cubeSelectionCube == null)
                    {
                        if (dataset == 1)
                        {
                            if (touchBarMiddleSM.Find("CubeSelection") != null)
                            {
                                Destroy(touchBarMiddleSM.Find("CubeSelection").gameObject);
                            }
                        }
                        else {
                            if (touchBarMiddleSM.GetChild(0).Find("CubeSelection") != null)
                            {
                                Destroy(touchBarMiddleSM.GetChild(0).Find("CubeSelection").gameObject);
                            }
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
                            //CubeHoveringBIM(touchBarMiddleSM);
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
                            //if (!scaling)
                            //{
                                rotationDelta -= Vector3.SignedAngle(rightB.position - leftB.position, oldV3FromLeftBtoRightB, Vector3.up);
                                if (circleLayout)
                                {
                                    //GameObject tmp = new GameObject();
                                    //tmp.transform.SetParent(worldInMiniture.transform.parent);
                                    //tmp.transform.localPosition = worldInMiniture.transform.localPosition;
                                    //tmp.transform.LookAt(Camera.main.transform.position);
                                    worldInMiniture.transform.localEulerAngles = new Vector3(0, rotationDelta, 0);
                                    //Destroy(tmp);
                                }
                                else
                                {
                                    worldInMiniture.transform.rotation = currentRotation;
                                }

                                // log the rotation
                                clockRotation = false;
                                antiClockRotation = false;
                                if (Vector3.SignedAngle(rightB.position - leftB.position, oldV3FromLeftBtoRightB, Vector3.up) > 0) {
                                    antiClockRotation = true;
                                }else if (Vector3.SignedAngle(rightB.position - leftB.position, oldV3FromLeftBtoRightB, Vector3.up) < 0)
                                {
                                    clockRotation = true;
                                }

                                foreach (GameObject go in dataSM)
                                {
                                    go.transform.localEulerAngles -= Vector3.up * Vector3.SignedAngle(rightB.position - leftB.position, oldV3FromLeftBtoRightB, Vector3.up);
                                }
                                SMRotationDiff -= Vector3.SignedAngle(rightB.position - leftB.position, oldV3FromLeftBtoRightB, Vector3.up);
                                //worldInMiniture.transform.localEulerAngles = Vector3.zero;
                                //worldInMiniture.transform.localScale = currentScale;
                                //if ((Vector3.Distance(leftB.position, rightB.position) / Mathf.Sqrt(3)) <= currentScale.x)
                                //{
                                //    worldInMiniture.transform.localScale = Vector3.one * (Vector3.Distance(leftB.position, rightB.position) / Mathf.Sqrt(3));
                                //}
                                //else
                                //{
                                Color oldColor = worldInMiniture.GetComponent<MeshRenderer>().material.color;
                                oldColor.a = 1f;
                                worldInMiniture.GetComponent<MeshRenderer>().material.color = oldColor;
                                worldInMiniture.transform.localScale = currentScale / 3;
                                //}
                                worldInMiniture.transform.position = (leftB.position + rightB.position) / 2;
                            //}
                            //else {
                            //    scaleUp = false;
                            //    scaleDown = false;
                            //    foreach (GameObject go in shelfBoards)
                            //    {
                            //        go.transform.GetChild(0).GetComponent<Bezier3PointCurve>().canUpdate = true;
                            //        //go.SetActive(true);
                            //        //go.transform.GetChild(0).GetComponent<Bezier3PointCurve>().enabled = true;
                            //    }
                            //    float finalScale = 0.0f;

                            //    float newControllersDistance = Vector3.Distance(leftController.transform.position, rightController.transform.position);
                            //    float distanceDiff = newControllersDistance - oldControllersDistance;
                            //    if (distanceDiff > 0.01f)
                            //    {
                            //        if (this.transform.localScale.x <= 1.58f)
                            //        {
                            //            this.transform.localScale += Vector3.one * 0.02f;
                            //        }
                            //        finalScale = dataSM[0].transform.localScale.x;

                            //        scaleUp = true;
                            //    }
                            //    else if (distanceDiff < -0.01f)
                            //    {
                            //        if (this.transform.localScale.x >= 0.72f) {
                            //            this.transform.localScale -= Vector3.one * 0.02f;
                            //        }
                            //        finalScale = dataSM[0].transform.localScale.x;

                            //        scaleDown = true;
                            //    }

                            //    if (circleLayout)
                            //    {
                            //        //GameObject tmp = new GameObject();
                            //        //tmp.transform.SetParent(worldInMiniture.transform.parent);
                            //        //tmp.transform.localPosition = worldInMiniture.transform.localPosition;
                            //        //tmp.transform.LookAt(Camera.main.transform.position);
                            //        worldInMiniture.transform.localEulerAngles = new Vector3(0, 0, 0);
                            //        //Destroy(tmp);
                            //    }
                            //    else
                            //    {
                            //        worldInMiniture.transform.rotation = currentRotation;
                            //        //worldInMiniture.transform.localEulerAngles = new Vector3(worldInMiniture.transform.localEulerAngles.x, worldInMiniture.transform.localEulerAngles.y, worldInMiniture.transform.localEulerAngles.z);
                            //    }

                            //    Color oldColor = worldInMiniture.GetComponent<MeshRenderer>().material.color;
                            //    oldColor.a = 0.5f;
                            //    worldInMiniture.GetComponent<MeshRenderer>().material.color = oldColor;
                            //    worldInMiniture.transform.localScale = currentScale;
                            //    worldInMiniture.transform.position = (leftB.position + rightB.position) / 2;
                            //    ShelfMovement(worldInMiniture.transform);

                            //}
                            
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
                    if (dataset == 1) {
                        float minX = Mathf.Min(cubeSelectionCube.transform.parent.GetChild(1).InverseTransformPoint(cubeSelectionCube.transform.GetChild(0).TransformPoint(-0.5f, 0, -0.5f)).y, 
                            cubeSelectionCube.transform.parent.GetChild(1).InverseTransformPoint(cubeSelectionCube.transform.GetChild(0).TransformPoint(0.5f, 0, 0.5f)).y);
                        float minZ = Mathf.Min(cubeSelectionCube.transform.parent.GetChild(3).InverseTransformPoint(cubeSelectionCube.transform.GetChild(0).TransformPoint(-0.5f, 0, -0.5f)).y, 
                            cubeSelectionCube.transform.parent.GetChild(3).InverseTransformPoint(cubeSelectionCube.transform.GetChild(0).TransformPoint(0.5f, 0, 0.5f)).y);
                        float maxX = Mathf.Max(cubeSelectionCube.transform.parent.GetChild(1).InverseTransformPoint(cubeSelectionCube.transform.GetChild(0).TransformPoint(-0.5f, 0, -0.5f)).y,
                            cubeSelectionCube.transform.parent.GetChild(1).InverseTransformPoint(cubeSelectionCube.transform.GetChild(0).TransformPoint(0.5f, 0, 0.5f)).y);
                        float maxZ = Mathf.Max(cubeSelectionCube.transform.parent.GetChild(3).InverseTransformPoint(cubeSelectionCube.transform.GetChild(0).TransformPoint(-0.5f, 0, -0.5f)).y,
                            cubeSelectionCube.transform.parent.GetChild(3).InverseTransformPoint(cubeSelectionCube.transform.GetChild(0).TransformPoint(0.5f, 0, 0.5f)).y);
                        if (minX == maxX || minZ == maxZ) {
                            Debug.Log("Cube filter adjustment wrong!!!");
                        }
                        //Debug.Log(minX + " " + maxX + " / " + minZ + " " + maxZ);
                        SetFilterPositionBIM(minX, maxX, minZ, maxZ);
                    }
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

    public void CubeHoveringBIM(List<GameObject> collidedSensors) {
        //Debug.Log("HAHA");
        FindAndHighlightAllRelatedSensors(collidedSensors);
    }

    private void CubeHovering(Transform barChart) {

        for (int i = 0; i < 100; i++) {
            hoveringRangeSelectionChessBoardBrushingBool[i] = true;
        }

        List<Vector2> pointsInBox = new List<Vector2>();

        int minCountry = 10;
        int maxCountry = 1;
        int minYear = 10;
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
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (pointsInBox.Contains(new Vector2(i + 1, j + 1)))
                {
                    //hoveringRangeSelectionChessBoardBrushingBool[i * 10 + j] = true;
                }
                else
                {
                    hoveringRangeSelectionChessBoardBrushingBool[i * 10 + j] = false;
                }
            }
        }

        BarChartCreator bcc = GameObject.Find("BarChartManagement").GetComponent<BarChartCreator>();
        bcc.UpdateBrushing(hoveringRangeSelectionChessBoardBrushingBool);

    }

    private void CubeSelection()
    {
        for (int i = 0; i < 100; i++) {
            rangeSelectionChessBoardBrushingBool[i] = true;
        }

        int finalMinCountry = 1;
        int finalMaxCountry = 10;
        int finalMinYear = 1;
        int finalMaxYear = 10;
        //bool noCube = true;
        foreach (GameObject sm in dataSM) {
            Transform barChart = sm.transform.GetChild(0);
            if (barChart.Find("CubeSelection") != null)
            {
                //Debug.Log(barChart.name);
                //noCube = false;
                List<Vector2> pointsInBox = new List<Vector2>();

                int minCountry = 10;
                int maxCountry = 1;
                int minYear = 10;
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
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (rangeSelectionChessBoardBrushingBool[i * 10 + j]) {
                            if (pointsInBox.Contains(new Vector2(i + 1, j + 1)))
                            {
                                rangeSelectionChessBoardBrushingBool[i * 10 + j] = true;
                            }
                            else
                            {
                                rangeSelectionChessBoardBrushingBool[i * 10 + j] = false;
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
        //Debug.Log(noCube);
        //if (noCube)
        //{
        //    for (int i = 0; i < rangeSelectionChessBoardBrushingBool.Length; i++)
        //    {
        //        rangeSelectionChessBoardBrushingBool[i] = true;
        //    }
        //    finalMinCountry = 1;
        //    finalMaxCountry = 10;
        //    finalMinYear = 1;
        //    finalMaxYear = 10;
        //}

        triggerPressedForFilterMoving = true;

        SetFilterPosition(finalMinCountry - 1, finalMaxCountry, finalMinYear - 1, finalMaxYear);
        triggerPressedForFilterMoving = false;

        BarChartCreator bcc = GameObject.Find("BarChartManagement").GetComponent<BarChartCreator>();
        finalChessBoardBrushingBool = CalculateFullLogicOfBrushingAndFiltering();
        bcc.UpdateBrushing(finalChessBoardBrushingBool);
    }

    private bool PointInBox(Vector3 point, Transform pointParent, Transform box) {
        float xMin = 10;
        float xMax = 0;
        float yMin = 10;
        float yMax = 0;
        float zMin = 10;
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

        hoveringChessBoardBrushingBool = new bool[100];

        int leftCountryBrushControl = (int)input.x;
        int rightCountryBrushControl = (int)input.y;
        int leftYearBrushControl = (int)input.z;
        int rightYearBrushControl = (int)input.w;

        if (mode == "single")
        {
            if (leftYearBrushControl != 0 && leftCountryBrushControl != 0)
            {
                hoveringChessBoardBrushingBool[10 * (leftCountryBrushControl - 1) + (leftYearBrushControl - 1)] = true;
            }
            else if (leftYearBrushControl != 0 && leftCountryBrushControl == 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    hoveringChessBoardBrushingBool[i * 10 + (leftYearBrushControl - 1)] = true;
                }
            }
            else if (leftYearBrushControl == 0 && leftCountryBrushControl != 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    hoveringChessBoardBrushingBool[(leftCountryBrushControl - 1) * 10 + i] = true;
                }
            }

            if (rightYearBrushControl != 0 && rightCountryBrushControl != 0)
            {
                hoveringChessBoardBrushingBool[10 * (rightCountryBrushControl - 1) + (rightYearBrushControl - 1)] = true;
            }
            else if (rightYearBrushControl != 0 && rightCountryBrushControl == 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    hoveringChessBoardBrushingBool[i * 10 + (rightYearBrushControl - 1)] = true;
                }
            }
            else if (rightYearBrushControl == 0 && rightCountryBrushControl != 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    hoveringChessBoardBrushingBool[(rightCountryBrushControl - 1) * 10 + i] = true;
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
                hoveringChessBoardBrushingBool[10 * (leftCountryBrushControl - 1) + (rightYearBrushControl - 1)] = true;
            }
            else if (leftYearBrushControl != 0 && leftCountryBrushControl == 0 && rightYearBrushControl == 0 && rightCountryBrushControl != 0)
            {
                hoveringChessBoardBrushingBool[10 * (rightCountryBrushControl - 1) + (leftYearBrushControl - 1)] = true;
            }
            else if (leftYearBrushControl == 0 && leftCountryBrushControl != 0 && rightYearBrushControl == 0 && rightCountryBrushControl != 0) // check parallel
            {
                if (Math.Abs(leftCountryBrushControl - rightCountryBrushControl) != 0)
                {
                    for (int i = Math.Min(leftCountryBrushControl, rightCountryBrushControl); i <= Math.Max(leftCountryBrushControl, rightCountryBrushControl); i++)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            hoveringChessBoardBrushingBool[(i - 1) * 10 + j] = true;
                        }
                    }
                }
                else
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        for (int j = 1; j <= 10; j++)
                        {
                            if (j == leftCountryBrushControl)
                                hoveringChessBoardBrushingBool[(j - 1) * 10 + (i - 1)] = true;
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
                        for (int j = 0; j < 10; j++)
                        {
                            hoveringChessBoardBrushingBool[j * 10 + (i - 1)] = true;
                        }
                    }
                }
                else
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        for (int j = 1; j <= 10; j++)
                        {
                            if (i == leftYearBrushControl)
                                hoveringChessBoardBrushingBool[(j - 1) * 10 + (i - 1)] = true;
                        }
                    }
                }
            }
            else
            {
                if (rightCountryBrushControl == 0 && leftCountryBrushControl != 0)
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        for (int j = 1; j <= 10; j++)
                        {
                            if (i == leftCountryBrushControl)
                                hoveringChessBoardBrushingBool[(i - 1) * 10 + (j - 1)] = true;
                        }
                    }
                }
                else if (leftCountryBrushControl == 0 && rightCountryBrushControl != 0)
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        for (int j = 1; j <= 10; j++)
                        {
                            if (i == rightCountryBrushControl)
                                hoveringChessBoardBrushingBool[(i - 1) * 10 + (j - 1)] = true;
                        }
                    }
                }

                if (rightYearBrushControl == 0 && leftYearBrushControl != 0)
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        for (int j = 1; j <= 10; j++)
                        {
                            if (j == leftYearBrushControl)
                                hoveringChessBoardBrushingBool[(i - 1) * 10 + (j - 1)] = true;
                        }
                    }
                }
                else if (rightYearBrushControl != 0 && leftYearBrushControl == 0)
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        for (int j = 1; j <= 10; j++)
                        {
                            if (j == rightYearBrushControl)
                                hoveringChessBoardBrushingBool[(i - 1) * 10 + (j - 1)] = true;
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
                    filter.transform.localPosition = new Vector3(-3, leftCountry / 10f, 0);
                }
                currentCountryLeftFilterPosition = leftCountry;
            }

            if (leftYear >= 0)
            {
                foreach (GameObject filter in leftYearFilters)
                {
                    filter.transform.localPosition = new Vector3(-3, leftYear / 10f, 0);
                }
                currentYearLeftFilterPosition = leftYear;
            }

            if (rightCountry >= 0)
            {
                foreach (GameObject filter in rightCountryFilters)
                {
                    filter.transform.localPosition = new Vector3(-3, rightCountry / 10f, 0);
                }
                currentCountryRightFilterPosition = rightCountry;
            }

            if (rightYear >= 0)
            {
                foreach (GameObject filter in rightYearFilters)
                {
                    filter.transform.localPosition = new Vector3(-3, rightYear / 10f, 0);
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
            //if (!circleLayout)
            //{
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
            //}
            //else
            //{
            //    if (!controllerShelfDeltaSetup)
            //    {
            //        controllerShelfDeltaSetup = true;
            //        controllerShelfDelta = SMManagerT.position - cubeTransform.position;
            //    }

            //    Vector3 newPosition = controllerShelfDelta + cubeTransform.position;

            //    float oldYForManager = SMManagerT.position.y;
            //    float newYFormanager = newPosition.y;

            //    SMManagerT.position = new Vector3(SMManagerT.position.x, newPosition.y, SMManagerT.position.z);
            //    if (SMManagerT.position.y < -0.6f * SMManagerT.localScale.y)
            //    {
            //        SMManagerT.position = new Vector3(SMManagerT.position.x, -0.6f * SMManagerT.localScale.y, SMManagerT.position.z);
            //    }
            //    else if (SMManagerT.position.y > (ExperimentManager.userHeight - 1.1) * SMManagerT.localScale.y)
            //    {
            //        SMManagerT.position = new Vector3(SMManagerT.position.x, (ExperimentManager.userHeight - 1.1f) * SMManagerT.localScale.y, SMManagerT.position.z);
            //    }

            //    if (Mathf.Abs(newYFormanager - oldYForManager) < 0.01f) {
            //        //Debug.Log(newWorldInMiniturePosition + " " + oldWorldInMiniturePosition);
            //        float angle = Vector3.SignedAngle((oldWorldInMiniturePosition - cameraForward), (newWorldInMiniturePosition - cameraForward), Vector3.up);

            //        if (Mathf.Abs(angle) > 0.2f)
            //        {
            //            SMManagerT.eulerAngles = oldEulerAngle + Vector3.up * -angle * 4;
            //            oldEulerAngle = SMManagerT.eulerAngles;
            //        }
            //    }
                

            //    GameObject.Find("PreferableStand").transform.localEulerAngles = new Vector3(90, SMManagerT.eulerAngles.y + 90, 0);
            //}
        }
        //else
        //{
        //    if (currentCurvature > semiCircleCurvature + 10f || currentCurvature < semiCircleCurvature - 10f)
        //    {
        //        if (!controllerShelfDeltaSetup)
        //        {
        //            controllerShelfDeltaSetup = true;
        //            controllerShelfDelta = gameObject.transform.position - cubeTransform.position;
        //        }
        //        Vector3 newPosition = controllerShelfDelta + cubeTransform.position;
        //        SMManagerT.position = new Vector3(SMManagerT.position.x, newPosition.y, newPosition.z);
        //        if (SMManagerT.position.y < -0.6f)
        //        {
        //            SMManagerT.position = new Vector3(SMManagerT.position.x, -0.6f, SMManagerT.position.z);
        //        }
        //        else if (SMManagerT.position.y > (ExperimentManager.userHeight - 0.6f))
        //        {
        //            SMManagerT.position = new Vector3(SMManagerT.position.x, (ExperimentManager.userHeight - 0.6f), SMManagerT.position.z);
        //        }
        //        if (SMManagerT.position.z > 1.5f)
        //        {
        //            SMManagerT.position = new Vector3(SMManagerT.position.x, SMManagerT.position.y, 1.5f);
        //        }
        //        else if (SMManagerT.position.z < -1.5f)
        //        {
        //            SMManagerT.position = new Vector3(SMManagerT.position.x, SMManagerT.position.y, -1.5f);
        //        }
        //    }
        //    else
        //    {
        //        //float rotateAngle = 0;
        //        Vector3 newRightControllerPosition = cubeTransform.position;

        //        float angle = Vector3.SignedAngle((oldWorldInMiniturePosition - cameraForward), (newRightControllerPosition - cameraForward), Vector3.up);

        //        if (Mathf.Abs(angle) > 0.2f)
        //        {
        //            SMManagerT.eulerAngles = oldEulerAngle + Vector3.up * -angle * 4;
        //        }

        //        if (!controllerShelfDeltaSetup)
        //        {
        //            controllerShelfDeltaSetup = true;
        //            controllerShelfDelta = SMManagerT.position - cubeTransform.position;
        //        }

        //        Vector3 newPosition = controllerShelfDelta + cubeTransform.position;
        //        SMManagerT.position = new Vector3(SMManagerT.position.x, newPosition.y, SMManagerT.position.z);

        //        if (SMManagerT.position.y < -0.6f)
        //        {
        //            SMManagerT.position = new Vector3(SMManagerT.position.x, -0.6f, SMManagerT.position.z);
        //        }
        //        else if (SMManagerT.position.y > (ExperimentManager.userHeight - 1.1f))
        //        {
        //            SMManagerT.position = new Vector3(SMManagerT.position.x, (ExperimentManager.userHeight - 1.1f), SMManagerT.position.z);
        //        }

        //        GameObject.Find("PreferableStand").transform.localEulerAngles = new Vector3(90, SMManagerT.eulerAngles.y, 0);
        //    }
        //}

        oldWorldInMiniturePosition = newWorldInMiniturePosition;
    }

    private void SetColorForFilter(GameObject filter, float transparency)
    {
        Color tmpColor = filter.GetComponent<Renderer>().material.color;
        tmpColor.a = transparency;
        filter.GetComponent<Renderer>().material.color = tmpColor;
    }


    /// <summary>
    /// Dataset 1
    /// </summary>
    private void DetectBIMInteraction() {
        
        List<GameObject> highlightedHoveringSensors = new List<GameObject>();

        if (leftFindHighlighedSensorFromCollision || rightFindHighlighedSensorFromCollision)
        { // high priority to show single highlighted sensor (hovering)
            if (leftFindHighlighedSensorFromCollision)
            {
                highlightedHoveringSensors.Add(leftFindHighlightedGO);
            }
            else if (rightFindHighlighedSensorFromCollision)
            {
                highlightedHoveringSensors.Add(rightFindHighlightedGO);
            }
            else {
                Debug.Log("wrong logic for single hovering");
            }
            FindAndHighlightAllRelatedSensors(highlightedHoveringSensors);
        }
        else if (leftFindHighlighedAxisFromCollision || rightFindHighlighedAxisFromCollision)
        { // next priority to show axis highlighted sensors (hovering)
            if (leftFindHighlighedAxisFromCollision && rightFindHighlighedAxisFromCollision) {
                highlightedHoveringSensors = CalculateBIMAxisBrushing(new Vector4(leftFindHoveringV2FromCollisionBIM.x, rightFindHoveringV2FromCollisionBIM.x, leftFindHoveringV2FromCollisionBIM.y, rightFindHoveringV2FromCollisionBIM.y));
            } else if (leftFindHighlighedAxisFromCollision) {
                if (rightAxisHighlighed)
                {
                    highlightedHoveringSensors = CalculateBIMAxisBrushing(new Vector4(leftFindHoveringV2FromCollisionBIM.x, rightFindHighlighedV2FromCollisionBIM.x, leftFindHoveringV2FromCollisionBIM.y, rightFindHighlighedV2FromCollisionBIM.y));
                }
                else
                {
                    highlightedHoveringSensors = CalculateBIMAxisBrushing(new Vector4(leftFindHoveringV2FromCollisionBIM.x, -1, leftFindHoveringV2FromCollisionBIM.y, -1));
                }
            }
            else if (rightFindHighlighedAxisFromCollision)
            {
                if (leftAxisHighlighed)
                {
                    highlightedHoveringSensors = CalculateBIMAxisBrushing(new Vector4(leftFindHighlighedV2FromCollisionBIM.x, rightFindHoveringV2FromCollisionBIM.x, leftFindHighlighedV2FromCollisionBIM.y, rightFindHoveringV2FromCollisionBIM.y));
                }
                else
                {
                    highlightedHoveringSensors = CalculateBIMAxisBrushing(new Vector4(-1, rightFindHoveringV2FromCollisionBIM.x, -1, rightFindHoveringV2FromCollisionBIM.y));
                }
            }
            FindAndHighlightAllRelatedSensors(highlightedHoveringSensors);
        }
        else { // nothing for hovering, check highlighted
            CalculateFinalHighlightedSensors();
        }
    }

    private List<GameObject> CalculateBIMAxisBrushing(Vector4 v) {
       
        List<GameObject> highlightedList = new List<GameObject>();
        float minX = v.x;
        float maxX = v.y;
        float minZ = v.z;
        float maxZ = v.w;
        GameObject go = dataSM[0];
        BuildingScript bs = go.transform.GetChild(0).GetComponent<BuildingScript>();

        if (minZ == -1 && minX != -1 && maxZ != -1 && maxX == -1)  // check intersection
        {
            foreach (KeyValuePair<string, GameObject> de in bs.sensorsG)
            {
                if (currentYMinFilterPosition == 0)
                {
                    if (de.Key.Contains("ACG") && int.Parse(de.Key.Substring(4)) <= 4 && int.Parse(de.Key.Substring(4)) >= 1)
                    {
                        if (go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y <= minX + 0.04f &&
                        go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y >= minX - 0.04f &&
                        go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y <= maxZ + 0.04f &&
                        go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y >= maxZ - 0.04f)
                        {
                            highlightedList.Add(de.Value);
                        }
                    }
                }
            }

            foreach (KeyValuePair<string, GameObject> de in bs.sensorsF1)
            {
                if (currentYMaxFilterPosition == 2)
                {
                    if (de.Key.Contains("AC1") && ((int.Parse(de.Key.Substring(4)) <= 9 && int.Parse(de.Key.Substring(4)) >= 3) || int.Parse(de.Key.Substring(4)) == 12))
                    {
                        if (go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y <= minX + 0.04f &&
                        go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y >= minX - 0.04f &&
                        go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y <= maxZ + 0.04f &&
                        go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y >= maxZ - 0.04f)
                        {
                            highlightedList.Add(de.Value);
                        }
                    }
                }
            }
        }
        else if (minZ != -1 && minX == -1 && maxZ == -1 && maxX != -1)  // check intersection
        {
            foreach (KeyValuePair<string, GameObject> de in bs.sensorsG)
            {
                if (currentYMinFilterPosition == 0)
                {
                    if (de.Key.Contains("ACG") && int.Parse(de.Key.Substring(4)) <= 4 && int.Parse(de.Key.Substring(4)) >= 1)
                    {
                        if (go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y <= maxX + 0.04f &&
                        go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y >= maxX - 0.04f &&
                        go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y <= minZ + 0.04f &&
                        go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y >= minZ - 0.04f)
                        {
                            highlightedList.Add(de.Value);
                        }
                    }
                }
            }

            foreach (KeyValuePair<string, GameObject> de in bs.sensorsF1)
            {
                if (currentYMaxFilterPosition == 2)
                {
                    if (de.Key.Contains("AC1") && ((int.Parse(de.Key.Substring(4)) <= 9 && int.Parse(de.Key.Substring(4)) >= 3) || int.Parse(de.Key.Substring(4)) == 12))
                    {
                        if (go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y <= maxX + 0.04f &&
                        go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y >= maxX - 0.04f &&
                        go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y <= minZ + 0.04f &&
                        go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y >= minZ - 0.04f)
                        {
                            highlightedList.Add(de.Value);
                        }
                    }
                }
            }
        }
        else if (minZ == -1 && minX != -1 && maxZ == -1 && maxX != -1) // check parallel
        {
            if (Math.Abs(minX - maxX) != 0)
            {
                if (minX < maxX)
                {
                    foreach (KeyValuePair<string, GameObject> de in bs.sensorsG)
                    {
                        if (currentYMinFilterPosition == 0)
                        {
                            if (de.Key.Contains("ACG") && int.Parse(de.Key.Substring(4)) <= 4 && int.Parse(de.Key.Substring(4)) >= 1)
                            {
                                if (go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y <= maxX + 0.04f &&
                                go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y >= minX - 0.04f)
                                {
                                    highlightedList.Add(de.Value);
                                }
                            }
                        }
                    }

                    foreach (KeyValuePair<string, GameObject> de in bs.sensorsF1)
                    {
                        if (currentYMaxFilterPosition == 2)
                        {
                            if (de.Key.Contains("AC1") && ((int.Parse(de.Key.Substring(4)) <= 9 && int.Parse(de.Key.Substring(4)) >= 3) || int.Parse(de.Key.Substring(4)) == 12))
                            {
                                if (go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y <= maxX + 0.04f &&
                                go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y >= minX - 0.04f)
                                {
                                    highlightedList.Add(de.Value);
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (KeyValuePair<string, GameObject> de in bs.sensorsG)
                    {
                        if (currentYMinFilterPosition == 0)
                        {
                            if (de.Key.Contains("ACG") && int.Parse(de.Key.Substring(4)) <= 4 && int.Parse(de.Key.Substring(4)) >= 1)
                            {
                                if (go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y <= minX + 0.04f &&
                                go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y >= maxX - 0.04f)
                                {
                                    highlightedList.Add(de.Value);
                                }
                            }
                        }
                    }

                    foreach (KeyValuePair<string, GameObject> de in bs.sensorsF1)
                    {
                        if (currentYMaxFilterPosition == 2)
                        {
                            if (de.Key.Contains("AC1") && ((int.Parse(de.Key.Substring(4)) <= 9 && int.Parse(de.Key.Substring(4)) >= 3) || int.Parse(de.Key.Substring(4)) == 12))
                            {
                                if (go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y <= minX + 0.04f &&
                                go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y >= maxX - 0.04f)
                                {
                                    highlightedList.Add(de.Value);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<string, GameObject> de in bs.sensorsG)
                {
                    if (currentYMinFilterPosition == 0)
                    {
                        if (de.Key.Contains("ACG") && int.Parse(de.Key.Substring(4)) <= 4 && int.Parse(de.Key.Substring(4)) >= 1)
                        {
                            if (go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y <= maxX + 0.04f &&
                            go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y >= maxX - 0.04f)
                            {
                                highlightedList.Add(de.Value);
                            }
                        }
                    }
                }

                foreach (KeyValuePair<string, GameObject> de in bs.sensorsF1)
                {
                    if (currentYMaxFilterPosition == 2)
                    {
                        if (de.Key.Contains("AC1") && ((int.Parse(de.Key.Substring(4)) <= 9 && int.Parse(de.Key.Substring(4)) >= 3) || int.Parse(de.Key.Substring(4)) == 12))
                        {
                            if (go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y <= maxX + 0.04f &&
                            go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y >= maxX - 0.04f)
                            {
                                highlightedList.Add(de.Value);
                            }
                        }
                    }
                }
            }
        }
        else if (minZ != -1 && minX == -1 && maxZ != -1 && maxX == -1) // check parallel
        {
            if (Math.Abs(minZ - maxZ) != 0)
            {
                if (minZ < maxZ)
                {
                    foreach (KeyValuePair<string, GameObject> de in bs.sensorsG)
                    {
                        if (currentYMinFilterPosition == 0)
                        {
                            if (de.Key.Contains("ACG") && int.Parse(de.Key.Substring(4)) <= 4 && int.Parse(de.Key.Substring(4)) >= 1)
                            {
                                if (go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y <= maxZ + 0.04f &&
                                go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y >= minZ - 0.04f)
                                {
                                    highlightedList.Add(de.Value);
                                }
                            }
                        }
                    }

                    foreach (KeyValuePair<string, GameObject> de in bs.sensorsF1)
                    {
                        if (currentYMaxFilterPosition == 2)
                        {
                            if (de.Key.Contains("AC1") && ((int.Parse(de.Key.Substring(4)) <= 9 && int.Parse(de.Key.Substring(4)) >= 3) || int.Parse(de.Key.Substring(4)) == 12))
                            {
                                if (go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y <= maxZ + 0.04f &&
                                go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y >= minZ - 0.04f)
                                {
                                    highlightedList.Add(de.Value);
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (KeyValuePair<string, GameObject> de in bs.sensorsG)
                    {
                        if (currentYMinFilterPosition == 0)
                        {
                            if (de.Key.Contains("ACG") && int.Parse(de.Key.Substring(4)) <= 4 && int.Parse(de.Key.Substring(4)) >= 1)
                            {
                                if (go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y <= minZ + 0.04f &&
                                go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y >= maxZ - 0.04f)
                                {
                                    highlightedList.Add(de.Value);
                                }
                            }
                        }
                    }

                    foreach (KeyValuePair<string, GameObject> de in bs.sensorsF1)
                    {
                        if (currentYMaxFilterPosition == 2)
                        {
                            if (de.Key.Contains("AC1") && ((int.Parse(de.Key.Substring(4)) <= 9 && int.Parse(de.Key.Substring(4)) >= 3) || int.Parse(de.Key.Substring(4)) == 12))
                            {
                                if (go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y <= minZ + 0.04f &&
                                go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y >= maxZ - 0.04f)
                                {
                                    highlightedList.Add(de.Value);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<string, GameObject> de in bs.sensorsG)
                {
                    if (currentYMinFilterPosition == 0)
                    {
                        if (de.Key.Contains("ACG") && int.Parse(de.Key.Substring(4)) <= 4 && int.Parse(de.Key.Substring(4)) >= 1)
                        {
                            if (go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y <= maxZ + 0.04f &&
                            go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y >= maxZ - 0.04f)
                            {
                                highlightedList.Add(de.Value);
                            }
                        }
                    }
                }

                foreach (KeyValuePair<string, GameObject> de in bs.sensorsF1)
                {
                    if (currentYMaxFilterPosition == 2)
                    {
                        if (de.Key.Contains("AC1") && ((int.Parse(de.Key.Substring(4)) <= 9 && int.Parse(de.Key.Substring(4)) >= 3) || int.Parse(de.Key.Substring(4)) == 12))
                        {
                            if (go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y <= maxZ + 0.04f &&
                            go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y >= maxZ - 0.04f)
                            {
                                highlightedList.Add(de.Value);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (maxX == -1 && minX != -1)
            {
                foreach (KeyValuePair<string, GameObject> de in bs.sensorsG)
                {
                    if (currentYMinFilterPosition == 0)
                    {
                        if (de.Key.Contains("ACG") && int.Parse(de.Key.Substring(4)) <= 4 && int.Parse(de.Key.Substring(4)) >= 1)
                        {
                            if (go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y <= minX + 0.04f &&
                            go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y >= minX - 0.04f)
                            {
                                highlightedList.Add(de.Value);
                            }
                        }
                    }
                }

                foreach (KeyValuePair<string, GameObject> de in bs.sensorsF1)
                {
                    if (currentYMaxFilterPosition == 2)
                    {
                        if (de.Key.Contains("AC1") && ((int.Parse(de.Key.Substring(4)) <= 9 && int.Parse(de.Key.Substring(4)) >= 3) || int.Parse(de.Key.Substring(4)) == 12))
                        {
                            if (go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y <= minX + 0.04f &&
                            go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y >= minX - 0.04f)
                            {
                                highlightedList.Add(de.Value);
                            }
                        }
                    }
                }
            }
            else if (minX == -1 && maxX != -1)
            {
                foreach (KeyValuePair<string, GameObject> de in bs.sensorsG)
                {
                    if (currentYMinFilterPosition == 0)
                    {
                        if (de.Key.Contains("ACG") && int.Parse(de.Key.Substring(4)) <= 4 && int.Parse(de.Key.Substring(4)) >= 1)
                        {
                            if (go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y <= maxX + 0.04f &&
                            go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y >= maxX - 0.04f)
                            {
                                highlightedList.Add(de.Value);
                            }
                        }
                    }
                }

                foreach (KeyValuePair<string, GameObject> de in bs.sensorsF1)
                {
                    if (currentYMaxFilterPosition == 2)
                    {
                        if (de.Key.Contains("AC1") && ((int.Parse(de.Key.Substring(4)) <= 9 && int.Parse(de.Key.Substring(4)) >= 3) || int.Parse(de.Key.Substring(4)) == 12))
                        {
                            if (go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y <= maxX + 0.04f &&
                            go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y >= maxX - 0.04f)
                            {
                                highlightedList.Add(de.Value);
                            }
                        }
                    }
                }
            }

            if (maxZ == -1 && minZ != -1)
            {
                foreach (KeyValuePair<string, GameObject> de in bs.sensorsG)
                {
                    if (currentYMinFilterPosition == 0)
                    {
                        if (de.Key.Contains("ACG") && int.Parse(de.Key.Substring(4)) <= 4 && int.Parse(de.Key.Substring(4)) >= 1)
                        {
                            if (go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y <= minZ + 0.04f &&
                            go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y >= minZ - 0.04f)
                            {
                                highlightedList.Add(de.Value);
                            }
                        }
                    }
                }

                foreach (KeyValuePair<string, GameObject> de in bs.sensorsF1)
                {
                    if (currentYMaxFilterPosition == 2)
                    {
                        if (de.Key.Contains("AC1") && ((int.Parse(de.Key.Substring(4)) <= 9 && int.Parse(de.Key.Substring(4)) >= 3) || int.Parse(de.Key.Substring(4)) == 12))
                        {
                            if (go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y <= minZ + 0.04f &&
                            go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y >= minZ - 0.04f)
                            {
                                highlightedList.Add(de.Value);
                            }
                        }
                    }
                }
            }
            else if (maxZ != -1 && minZ == -1)
            {
                foreach (KeyValuePair<string, GameObject> de in bs.sensorsG)
                {
                    if (currentYMinFilterPosition == 0)
                    {
                        if (de.Key.Contains("ACG") && int.Parse(de.Key.Substring(4)) <= 4 && int.Parse(de.Key.Substring(4)) >= 1)
                        {
                            if (go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y <= maxZ + 0.04f &&
                            go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y >= maxZ - 0.04f)
                            {
                                highlightedList.Add(de.Value);
                            }
                        }
                    }
                }

                foreach (KeyValuePair<string, GameObject> de in bs.sensorsF1)
                {
                    if (currentYMaxFilterPosition == 2)
                    {
                        if (de.Key.Contains("AC1") && ((int.Parse(de.Key.Substring(4)) <= 9 && int.Parse(de.Key.Substring(4)) >= 3) || int.Parse(de.Key.Substring(4)) == 12))
                        {
                            if (go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y <= maxZ + 0.04f &&
                            go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y >= maxZ - 0.04f)
                            {
                                highlightedList.Add(de.Value);
                            }
                        }
                    }
                }
            }
        }
        return highlightedList;
    }

    public void RegisterCollidedSensorFromLeft(GameObject sensor)
    {
        
        if (sensor != null)
        {
            leftFindHighlighedSensorFromCollision = true;
            leftFindHighlightedGO = sensor;
        }
        else {
            leftFindHighlighedSensorFromCollision = false;
            leftFindHighlightedGO = null;
        }
    }

    public void RegisterCollidedSensorFromRight(GameObject sensor)
    {
        
        if (sensor != null)
        {
            rightFindHighlighedSensorFromCollision = true;
            rightFindHighlightedGO = sensor;
        }
        else {
            rightFindHighlighedSensorFromCollision = false;
            rightFindHighlightedGO = null;
        }
    }

    public void FilterBIMFromCollision(string axis, string fil, float position)
    {
        
        foreach (GameObject sm in dataSM)
        {
            if (sm.transform.Find("CubeSelection") != null)
            {
                Destroy(sm.transform.Find("CubeSelection").gameObject);
            }
        }
        cubeSelectionCube = null;
        touchBarMiddleSM = null;
        highlightedSensorsFromMovingFilters = allSensors;
        //Debug.Log("HAHA" + currentXMinFilterPosition + " " + currentXMaxFilterPosition + " / " + currentZMinFilterPosition + " " + currentZMaxFilterPosition);
        leftHighlighed = false;
        rightHighlighed = false;
        leftAxisHighlighed = false;
        rightAxisHighlighed = false;
        if (axis == "X")
        {
            if (fil == "min")
            {
                foreach (GameObject filter in minXFilters)
                {
                    filter.transform.localPosition = new Vector3(2, position, 0);
                }
                currentXMinFilterPosition = position;
            }
            else
            {
                foreach (GameObject filter in maxXFilters)
                {
                    filter.transform.localPosition = new Vector3(2, position, 0);
                }
                currentXMaxFilterPosition = position;
            }
        }
        else if (axis == "Z")
        {
            if (fil == "min")
            {
                foreach (GameObject filter in minZFilters)
                {
                    filter.transform.localPosition = new Vector3(2, position, 0);
                }
                currentZMinFilterPosition = position;
            }
            else
            {
                foreach (GameObject filter in maxZFilters)
                {
                    filter.transform.localPosition = new Vector3(2, position, 0);
                }
                currentZMaxFilterPosition = position;
            }
        }else if (axis == "Y")
        {
            if (fil == "min")
            {
                foreach (GameObject filter in minYFilters)
                {
                    filter.transform.localPosition = new Vector3(2, position / 2f, 0);
                }
                currentYMinFilterPosition = (int)position;
            }
            else
            {
                foreach (GameObject filter in maxYFilters)
                {
                    filter.transform.localPosition = new Vector3(2, position / 2f, 0);
                }
                currentYMaxFilterPosition = (int)position;
            }
        }
        //Debug.Log("HAHA2" + currentXMinFilterPosition + " " + currentXMaxFilterPosition + " / " + currentZMinFilterPosition + " " + currentZMaxFilterPosition);
        CalculateBIMHighlightedBasedOnFilterPosition();
    }



    private void CalculateBIMHighlightedBasedOnFilterPosition() {
        
        GameObject go = dataSM[0];
        //Debug.Log(currentXMinFilterPosition + " " + currentXMaxFilterPosition + " " + currentZMinFilterPosition + " " + currentZMaxFilterPosition);
        highlightedSensorsFromMovingFilters = new List<GameObject>();

        BuildingScript bs = go.transform.GetChild(0).GetComponent<BuildingScript>();

        foreach (KeyValuePair<string, GameObject> de in bs.sensorsG)
        {
            if (currentYMinFilterPosition == 0)
            {
                if (de.Key.Contains("ACG") && int.Parse(de.Key.Substring(4)) <= 4 && int.Parse(de.Key.Substring(4)) >= 1)
                {
                    if (go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y <= currentXMaxFilterPosition &&
                    go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y >= currentXMinFilterPosition &&
                    go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y <= currentZMaxFilterPosition &&
                    go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y >= currentZMinFilterPosition)
                    {
                        highlightedSensorsFromMovingFilters.Add(de.Value);
                    }
                }
            }
        }

        foreach (KeyValuePair<string, GameObject> de in bs.sensorsF1)
        {
            if (currentYMaxFilterPosition == 2) {
                if (de.Key.Contains("AC1") && ((int.Parse(de.Key.Substring(4)) <= 9 && int.Parse(de.Key.Substring(4)) >= 3) || int.Parse(de.Key.Substring(4)) == 12))
                {
                    if (go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y <= currentXMaxFilterPosition &&
                    go.transform.GetChild(1).InverseTransformPoint(de.Value.transform.position).y >= currentXMinFilterPosition &&
                    go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y <= currentZMaxFilterPosition &&
                    go.transform.GetChild(3).InverseTransformPoint(de.Value.transform.position).y >= currentZMinFilterPosition)
                    {
                        highlightedSensorsFromMovingFilters.Add(de.Value);
                    }
                }
            } 
        }
            
        FindAndHighlightAllRelatedSensors(highlightedSensorsFromMovingFilters);
    }

    private void CalculateFinalHighlightedSensors() {
       
        finalHighlightedSensors = new List<GameObject>();

        if (leftHighlighed || rightHighlighed) { // single brushing
            if (leftHighlighed)
            {
                finalHighlightedSensors.Add(leftKeepHighlightedGO);
            }
            else if (rightHighlighed) {
                finalHighlightedSensors.Add(rightKeepHighlightedGO);
            }
        } else if (leftAxisHighlighed || rightAxisHighlighed) { // axis brushing
            finalHighlightedSensors = CalculateBIMAxisBrushing(new Vector4(leftFindHighlighedV2FromCollisionBIM.x, rightFindHighlighedV2FromCollisionBIM.x, leftFindHighlighedV2FromCollisionBIM.y, rightFindHighlighedV2FromCollisionBIM.y));
        }
        else {
            if (allSensors.Count == 0)
            {
                BuildingScript bs = dataSM[0].transform.GetChild(0).GetComponent<BuildingScript>();
                foreach (KeyValuePair<string, GameObject> de in bs.sensorsG)
                {
                    if (de.Key.Contains("ACG") && int.Parse(de.Key.Substring(4)) <= 4 && int.Parse(de.Key.Substring(4)) >= 1)
                    {
                        allSensors.Add(de.Value);
                    }
                }

                foreach (KeyValuePair<string, GameObject> de in bs.sensorsF1)
                {
                    if (de.Key.Contains("AC1") && ((int.Parse(de.Key.Substring(4)) <= 9 && int.Parse(de.Key.Substring(4)) >= 3) || int.Parse(de.Key.Substring(4)) == 12))
                    {
                        allSensors.Add(de.Value);
                    }
                }
                highlightedSensorsFromMovingFilters = allSensors;
            }
            //Debug.Log(highlightedSensorsFromMovingFilters.Count);
            finalHighlightedSensors = highlightedSensorsFromMovingFilters;
        }

        if (finalHighlightedSensors.Count != 0) {
            FindAndHighlightAllRelatedSensors(finalHighlightedSensors);
        }   
    }

    private void DetectControllerVertical()
    {
        if (trialState == TrialState.OnTask || (trialState == TrialState.PreTask && interactionTrainingNeeded && interactionTrainingCount == 6))
        {
            GameObject leftTouchbar = null;
            if (GameObject.Find("Controller (left)") != null)
            {
                Transform leftController = GameObject.Find("Controller (left)").transform;
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

                if (leftTouchbar != null)
                {
                    Vector3 touchBarEA = leftTouchbar.transform.eulerAngles;

                    if (((touchBarEA.x >= 345 && touchBarEA.x <= 360) || (touchBarEA.x >= 0 && touchBarEA.x <= 15)) && ((touchBarEA.z >= 345 && touchBarEA.z <= 360) || (touchBarEA.z >= 0 && touchBarEA.z <= 15)))
                    {
                        GameObject.Find("Controller (left)").transform.GetChild(4).gameObject.SetActive(true);
                        colorFilterActive = true;
                    }
                    if (!(((touchBarEA.x >= 305 && touchBarEA.x <= 360) || (touchBarEA.x >= 0 && touchBarEA.x <= 55)) && ((touchBarEA.z >= 305 && touchBarEA.z <= 360) || (touchBarEA.z >= 0 && touchBarEA.z <= 55))))
                    {
                        GameObject.Find("Controller (left)").transform.GetChild(4).gameObject.SetActive(false);
                        colorFilterActive = false;
                    }
                }

            }
        }
        else {
            if (GameObject.Find("Controller (left)") != null) {
                GameObject.Find("Controller (left)").transform.GetChild(4).gameObject.SetActive(false);
            }
        }
    }

    public void ColorFilterEnterCollision() {
        ResetHighlightForBIM();
    }

    public void ColorFilterExitCollision() {
        colorFilterOn = false;
        //Debug.Log(colorFilterSelected + " " + colorSelectedIndex);
        GameObject filterPanel = GameObject.Find("PanelMenuForColorFilter");
        Transform gridLayoutChoices = null;
        if (filterPanel != null)
        {
            gridLayoutChoices = filterPanel.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1);
        }

        if (!colorFilterSelected)
        {
            if (gridLayoutChoices != null)
            {
                for (int i = 1; i <= 9; i++)
                {
                    gridLayoutChoices.GetChild(i).GetChild(0).GetComponent<Text>().color = Color.white;
                }
            }
            
            ResetHighlightForBIM();
        }
        else {
            if (gridLayoutChoices != null)
            {
                for (int i = 1; i <= 9; i++)
                {
                    if (i == colorSelectedIndex)
                    {
                        gridLayoutChoices.GetChild(10 - i).GetChild(0).GetComponent<Text>().color = Color.green;
                    }
                    else
                    {
                        gridLayoutChoices.GetChild(10 - i).GetChild(0).GetComponent<Text>().color = Color.white;
                    }
                }
            }

            if (colorSelectedIndex == 0)
            {
                HighlightSpecificColorByIndex(0);
                colorFilterOn = false;
            }
            else if (colorSelectedIndex > 0 && colorSelectedIndex <= 9)
            {
                HighlightSpecificColorByIndex(10 - colorSelectedIndex);
            }
            else
            {
                Debug.LogError("wrong colorfilterindex input from collision");
            }
            //switch (colorSelectedIndex)
            //{
            //    case 0:
            //        HighlightSpecificColor(Color.white);
            //        colorFilterOn = false;
            //        break;
            //    case 1:
            //        Color cl27 = new Color();
            //        ColorUtility.TryParseHtmlString("#d73027", out cl27);
            //        HighlightSpecificColor(cl27);
            //        break;
            //    case 2:
            //        Color cl26 = new Color();
            //        ColorUtility.TryParseHtmlString("#f46d43", out cl26);
            //        HighlightSpecificColor(cl26);
            //        break;
            //    case 3:
            //        Color cl25 = new Color();
            //        ColorUtility.TryParseHtmlString("#fdae61", out cl25);
            //        HighlightSpecificColor(cl25);
            //        break;
            //    case 4:
            //        Color cl24 = new Color();
            //        ColorUtility.TryParseHtmlString("#fee090", out cl24);
            //        HighlightSpecificColor(cl24);
            //        break;
            //    case 5:
            //        Color cl23 = new Color();
            //        ColorUtility.TryParseHtmlString("#ffffbf", out cl23);
            //        HighlightSpecificColor(cl23);
            //        break;
            //    case 6:
            //        Color cl22 = new Color();
            //        ColorUtility.TryParseHtmlString("#e0f3f8", out cl22);
            //        HighlightSpecificColor(cl22);
            //        break;
            //    case 7:
            //        Color cl21 = new Color();
            //        ColorUtility.TryParseHtmlString("#abd9e9", out cl21);
            //        HighlightSpecificColor(cl21);
            //        break;
            //    case 8:
            //        Color cl20 = new Color();
            //        ColorUtility.TryParseHtmlString("#74add1", out cl20);
            //        HighlightSpecificColor(cl20);
            //        break;
            //    case 9:
            //        Color cl19 = new Color();
            //        ColorUtility.TryParseHtmlString("#4575b4", out cl19);
            //        HighlightSpecificColor(cl19);
            //        break;
            //    default:
            //        break;
            //}
        }
    }

    public void ColorFilterForBIMSensors(int colorFilterIndex)
    {
        colorFilterOn = true;
        //Debug.Log(colorFilterIndex);
        GameObject filterPanel = GameObject.Find("PanelMenuForColorFilter");
        Transform gridLayoutChoices = null;
        if (filterPanel != null) {
            gridLayoutChoices = filterPanel.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1);
        }
        if (gridLayoutChoices != null) {
            if (colorFilterIndex != 0)
            {
                for (int i = 1; i <= 9; i++)
                {
                    if (i == colorFilterIndex)
                    {
                        gridLayoutChoices.GetChild(10 - i).GetChild(0).GetComponent<Text>().color = Color.green;
                    }
                    else {
                        gridLayoutChoices.GetChild(10 - i).GetChild(0).GetComponent<Text>().color = Color.white;
                    }
                }
            }
            else {
                for (int i = 1; i <= 9; i++) {
                    gridLayoutChoices.GetChild(i).GetChild(0).GetComponent<Text>().color = Color.white;
                }
                
            }
        }

        if (GameObject.Find("Controller (right)") != null) {
            SteamVR_TrackedController rtc = GameObject.Find("Controller (right)").GetComponent<SteamVR_TrackedController>();
            if (rtc.triggerPressed) {
                colorSelectedIndex = colorFilterIndex;
                //colorFilterOn = true;
                colorFilterSelected = true;
            }
        }
        //Debug.Log(colorFilterSelected + " " + colorSelectedIndex);
        //colorFilterOn = true;
        //ResetHighlightForBIM();
        //Debug.Log(colorFilterIndex);

        if (colorFilterIndex == 0)
        {
            HighlightSpecificColorByIndex(0);
        }
        else if (colorFilterIndex > 0 && colorFilterIndex <= 9)
        {
            HighlightSpecificColorByIndex(10 - colorFilterIndex);
        }
        else {
            Debug.LogError("wrong colorfilterindex input from collision");
        }
        /*
        switch (colorFilterIndex)
        {
            case 0:
                HighlightSpecificColor(Color.white);
                //colorSelectedIndex = 0;
                //colorFilterOn = false;
                break;
            case 1:
                Color cl27 = new Color();
                ColorUtility.TryParseHtmlString("#d73027", out cl27);
                HighlightSpecificColor(cl27);
                break;
            case 2:
                Color cl26 = new Color();
                ColorUtility.TryParseHtmlString("#f46d43", out cl26);
                HighlightSpecificColor(cl26);
                break;
            case 3:
                Color cl25 = new Color();
                ColorUtility.TryParseHtmlString("#fdae61", out cl25);
                HighlightSpecificColor(cl25);
                break;
            case 4:
                Color cl24 = new Color();
                ColorUtility.TryParseHtmlString("#fee090", out cl24);
                HighlightSpecificColor(cl24);
                break;
            case 5:
                Color cl23 = new Color();
                ColorUtility.TryParseHtmlString("#ffffbf", out cl23);
                HighlightSpecificColor(cl23);
                break;
            case 6:
                Color cl22 = new Color();
                ColorUtility.TryParseHtmlString("#e0f3f8", out cl22);
                HighlightSpecificColor(cl22);
                break;
            case 7:
                Color cl21 = new Color();
                ColorUtility.TryParseHtmlString("#abd9e9", out cl21);
                HighlightSpecificColor(cl21);
                break;
            case 8:
                Color cl20 = new Color();
                ColorUtility.TryParseHtmlString("#74add1", out cl20);
                HighlightSpecificColor(cl20);
                break;
            case 9:
                Color cl19 = new Color();
                ColorUtility.TryParseHtmlString("#4575b4", out cl19);
                HighlightSpecificColor(cl19);
                break;
            default:
                break;
        }
        */
    }

    private void HighlightSpecificColorByIndex(int index) {
        if (index == 0)
        {
            foreach (GameObject go in dataSM)
            {
                BuildingScript bs = go.transform.GetChild(0).GetComponent<BuildingScript>();
                foreach (KeyValuePair<string, GameObject> de in bs.sensorsG)
                {
                    SetColorForSensor(de.Value, 1);
                }

                foreach (KeyValuePair<string, GameObject> de in bs.sensorsF1)
                {
                    SetColorForSensor(de.Value, 1);
                }
            }
        }
        else {
            foreach (GameObject go in dataSM)
            {
                BuildingScript bs = go.transform.GetChild(0).GetComponent<BuildingScript>();
                foreach (KeyValuePair<string, GameObject> de in bs.sensorsG)
                {
                    SensorScript ss = de.Value.GetComponent<SensorScript>();

                    // Color color2 = de.Value.GetComponent<Renderer>().material.color;
                    //Debug.Log(color2.r + " " + color2.g + " " + color2.b);
                    if (ss != null) {
                        if (ss.GetSensorColorIndex() == index)
                        {
                            //Debug.Log("YES " + go.name + " " + de.Key);
                            SetColorForSensor(de.Value, 1f);
                        }
                        else
                        {
                            SetColorForSensor(de.Value, 0.1f);
                        }
                    }
                    
                }

                foreach (KeyValuePair<string, GameObject> de in bs.sensorsF1)
                {
                    SensorScript ss = de.Value.GetComponent<SensorScript>();
                    //if (CompareTwoColors(color, de.Value.GetComponent<Renderer>().material.color))
                    if (ss != null)
                    {
                        if (ss.GetSensorColorIndex() == index)
                        {
                            SetColorForSensor(de.Value, 1f);
                        }
                        else
                        {
                            SetColorForSensor(de.Value, 0.1f);
                        }

                    }
                    
                }
            }
        }
    }

    private void HighlightSpecificColor(Color color)
    {
        if (CompareTwoColors(color, Color.white))
        {
            foreach (GameObject go in dataSM)
            {
                BuildingScript bs = go.transform.GetChild(0).GetComponent<BuildingScript>();
                foreach (KeyValuePair<string, GameObject> de in bs.sensorsG)
                {
                    SetColorForSensor(de.Value, 1);
                }

                foreach (KeyValuePair<string, GameObject> de in bs.sensorsF1)
                {
                    SetColorForSensor(de.Value, 1);
                }
            }
        }
        else
        {
            foreach (GameObject go in dataSM)
            {
                BuildingScript bs = go.transform.GetChild(0).GetComponent<BuildingScript>();
                foreach (KeyValuePair<string, GameObject> de in bs.sensorsG)
                {
                    Color color2 = de.Value.GetComponent<Renderer>().material.color;
                    //Debug.Log(color2.r + " " + color2.g + " " + color2.b);
                    if (CompareTwoColors(color, de.Value.GetComponent<Renderer>().material.color))
                    {
                        //Debug.Log("YES " + go.name + " " + de.Key);
                        SetColorForSensor(de.Value, 1f);
                    }
                    else
                    {
                        SetColorForSensor(de.Value, 0.1f);
                    }
                }

                foreach (KeyValuePair<string, GameObject> de in bs.sensorsF1)
                {
                    if (CompareTwoColors(color, de.Value.GetComponent<Renderer>().material.color))
                    {
                        SetColorForSensor(de.Value, 1f);
                    }
                    else
                    {
                        SetColorForSensor(de.Value, 0.1f);
                    }
                }
            }
        }

    }

    private bool CompareTwoColors(Color a, Color b)
    {
        //Debug.Log(a.r + " " + a.g + " " + a.b + ";" + b.r + " " + b.g + " " + b.b);
        if ((int)(a.r * 100.0f) / 100.0f == (int)(b.r * 100.0f) / 100.0f && (int)(a.g * 100.0f) / 100.0f == (int)(b.g * 100.0f) / 100.0f && (int)(a.b * 100.0f) / 100.0f == (int)(b.b * 100.0f) / 100.0f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SetFilterPositionBIM(float minX, float maxX, float minZ, float maxZ)
    {
        
        if (minX >= 0)
        {
            foreach (GameObject filter in minXFilters)
            {
                filter.transform.localPosition = new Vector3(2, minX, 0);
            }
            currentXMinFilterPosition = minX;
        }

        if (minZ >= 0)
        {
            foreach (GameObject filter in minZFilters)
            {
                filter.transform.localPosition = new Vector3(2, minZ, 0);
            }
            currentZMinFilterPosition = minZ;
        }

        if (maxX >= 0)
        {
            foreach (GameObject filter in maxXFilters)
            {
                filter.transform.localPosition = new Vector3(2, maxX, 0);
            }
            currentXMaxFilterPosition = maxX;
        }

        if (maxZ >= 0)
        {
            foreach (GameObject filter in maxZFilters)
            {
                filter.transform.localPosition = new Vector3(2, maxZ, 0);
            }
            currentZMaxFilterPosition = maxZ;
        }
    }

    private void FindAndHighlightAllRelatedSensors(List<GameObject> sensors) {
       
        if (sensors.Count == 0)
        {
            foreach (GameObject go in dataSM)
            {

                BuildingScript bs = go.transform.GetChild(0).GetComponent<BuildingScript>();
                foreach (KeyValuePair<string, GameObject> de in bs.sensorsG)
                {
                    SetColorForSensor(de.Value, 0.1f);
                }

                foreach (KeyValuePair<string, GameObject> de in bs.sensorsF1)
                {

                    SetColorForSensor(de.Value, 0.1f);
                }
            }
        }
        else {
            
            List<GameObject> tmp = new List<GameObject>();
            List<GameObject> tmpAll = new List<GameObject>();

            foreach (GameObject go in dataSM)
            {

                BuildingScript bs = go.transform.GetChild(0).GetComponent<BuildingScript>();
                foreach (KeyValuePair<string, GameObject> de in bs.sensorsG)
                {
                    if (de.Key.Contains("ACG") && int.Parse(de.Key.Substring(4)) <= 4 && int.Parse(de.Key.Substring(4)) >= 1)
                    {
                        foreach (GameObject sensor in sensors)
                        {
                            if (sensor.name == de.Key)
                            {
                                tmp.Add(de.Value);
                            }
                        }
                        tmpAll.Add(de.Value);
                    }
                    
                }

                foreach (KeyValuePair<string, GameObject> de in bs.sensorsF1)
                {
                    if (de.Key.Contains("AC1") && ((int.Parse(de.Key.Substring(4)) <= 9 && int.Parse(de.Key.Substring(4)) >= 3) || int.Parse(de.Key.Substring(4)) == 12))
                    {
                        foreach (GameObject sensor in sensors)
                        {

                            if (sensor.name.Trim().Equals(de.Key.Trim()))
                            {
                                tmp.Add(de.Value);
                            }
                        }
                        tmpAll.Add(de.Value);
                    }
                    
                }
            }

            //foreach (GameObject go in tmp)
            //{
            //    SetColorForSensor(go, 1f);
            //}
            foreach (GameObject go in tmpAll) {
                if (tmp.Contains(go))
                {
                    SetColorForSensor(go, 1f);
                }
                else {
                    SetColorForSensor(go, 0.1f);
                }
            }
        }
        
    }

    private void ResetHighlightForBIM() {
        //leftPressedCount = 0;
        //rightPressedCount = 0;
        leftHighlighed = false;
        rightHighlighed = false;
        leftAxisHighlighed = false;
        rightAxisHighlighed = false;

        leftFindHighlighedV2FromCollisionBIM = new Vector2(-1, -1);
        rightFindHighlighedV2FromCollisionBIM = new Vector2(-1, -1);

        highlightedSensorsFromMovingFilters = allSensors;
        colorSelectedIndex = 0;

        foreach (GameObject sm in dataSM)
        {
            if (sm.transform.Find("CubeSelection") != null)
            {
                Destroy(sm.transform.Find("CubeSelection").gameObject);
            }
        }

        // reset filter buttons for all axis
        foreach (GameObject filter in minXFilters)
        {
            filter.transform.localPosition = new Vector3(2, 0, 0);
        }

        foreach (GameObject filter in maxXFilters)
        {
            filter.transform.localPosition = new Vector3(2, 1, 0);
        }

        foreach (GameObject filter in minYFilters)
        {
            filter.transform.localPosition = new Vector3(2, 0, 0);
        }

        foreach (GameObject filter in maxYFilters)
        {
            filter.transform.localPosition = new Vector3(2, 1, 0);
        }

        foreach (GameObject filter in minZFilters)
        {
            filter.transform.localPosition = new Vector3(2, 0, 0);
        }

        foreach (GameObject filter in maxZFilters)
        {
            filter.transform.localPosition = new Vector3(2, 1, 0);
        }
    }

    private void SetColorForSensor(GameObject go, float transparency)
    {
        Color tmpColor = go.GetComponent<Renderer>().material.color;
        tmpColor.a = transparency;
        go.GetComponent<Renderer>().material.color = tmpColor;
        //go.GetComponent<Renderer>().material.SetColor("_OutlineColor", tmpColor);
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

    private void GetTrainingTasks() {
        string[] lines = new string[(taskNo / 2 + 1)];
        if (dataset == 1)
        {
            if (!circleLayout) // F
            {
                lines = BIMFltQsTFile.text.Split(lineSeperater);
            }
            else
            {
                if (fullCircle) // Q
                {
                    lines = BIMQCurQsTFile.text.Split(lineSeperater);
                }
                else { // H
                    lines = BIMCurQsTFile.text.Split(lineSeperater);
                }
            }

        }
        else if (dataset == 2)
        {
            if (!circleLayout)
            {
                lines = BarFltQsTFile.text.Split(lineSeperater);
            }
            else
            {
                if (fullCircle) // Q
                {
                    lines = BarQCurQsTFile.text.Split(lineSeperater);
                }
                else
                { // H
                    lines = BarCurQsTFile.text.Split(lineSeperater);
                }
            }
        }
        for (int i = 0; i < (taskNo / 2); i++)
        {
            trainingTaskArray[i] = lines[i].Split('|')[0];
            highlightedTrainingSM[i] = new Vector2(int.Parse(lines[i].Split('|')[1].Split(',')[0]), int.Parse(lines[i].Split('|')[1].Split(',')[1]));
            QuestionIDs[i] = lines[i].Split('|')[3];
            CorrectAnswers[i] = lines[i].Split('|')[4];
        }
    }

    void GetTasks() {
        string[] lines = new string[taskNo + 1];
        if (dataset == 1)
        {
            if (!circleLayout) // F
            {
                lines = BIMFltQsFile.text.Split(lineSeperater);
            }
            else
            {
                if (fullCircle) // Q
                {
                    lines = BIMQCurQsFile.text.Split(lineSeperater);
                }
                else
                { // H
                    lines = BIMCurQsFile.text.Split(lineSeperater);
                }
            }

        }
        else if (dataset == 2)
        {
            if (!circleLayout) // F
            {
                lines = BarFltQsFile.text.Split(lineSeperater);
            }
            else
            {
                if (fullCircle) // Q
                {
                    lines = BarQCurQsFile.text.Split(lineSeperater);
                }
                else
                { // H
                    lines = BarCurQsFile.text.Split(lineSeperater);
                }
            }
        }

        for (int i = 0; i < taskNo; i++) {
            taskArray[i] = lines[i].Split('|')[0];
            highlightedSM[i] = new Vector2(int.Parse(lines[i].Split('|')[1].Split(',')[0]), int.Parse(lines[i].Split('|')[1].Split(',')[1]));
            QuestionIDs[i + taskNo / 2] = lines[i].Split('|')[3];
            CorrectAnswers[i + taskNo / 2] = lines[i].Split('|')[4];
        }

    }

    private void ShowTrainingTask(int index) {
        if (index >= 0) {
            ChangeTaskText(trainingTaskArray[index], 100);
        }
    }

    void ShowTasks() {
        if (taskID > 0)
        {
            ChangeTaskText(taskArray[taskID - 1], taskID);
        }
        else {
            if (taskID < 0)
            {
                ChangeTaskText("Thank you for your participation!", 0);
            }
            else {
                Debug.Log("Task ID == 0!!!");
                //if (dataset == 1)
                //{
                //    if (circleLayout)
                //    {
                //        ChangeTaskText("There will be four questions in this section. Please tell me the answer of each question when you are ready. " +
                //            "You can rotate the small multiples using <color=red>grip</color> button and change questions using <color=green>touchpad</color> buttons.", 0);
                //    }
                //    else
                //    {
                //        ChangeTaskText("There will be four questions in this section. Please tell me the answer of each question when you are ready. " +
                //            "You can rotate the small multiples using <color=red>grip</color> button and change questions using <color=green>touchpad</color> buttons.", 0);
                //    }
                //}
                //else if (dataset == 2)
                //{
                //    if (circleLayout)
                //    {
                //        ChangeTaskText("There will be four questions in this section. Please tell me the answer of each question when you are ready. " +
                //            "You can rotate the small multiples using <color=red>grip</color> button, change questions using <color=green>touchpad</color> buttons and brush any data using <color=blue>touchpad</color> buttons.", 0);
                //    }
                //    else
                //    {
                //        ChangeTaskText("There will be four questions in this section. Please tell me the answer of each question when you are ready. " +
                //            "You can rotate the small multiples using <color=red>grip</color> button, change questions using <color=green>touchpad</color> buttons and brush any data using <color=blue>touchpad</color> buttons.", 0);
                //    }
                //}
                //else if (dataset == 3)
                //{
                //    ChangeTaskText("There will be four questions in this section. Please tell me the answer of each question when you are ready. You can interact with the small multiples freely with the designed methods.", 0);
                //}
            }          
        }
    }

    // static functions

    public void ChangeTaskText(string taskText, int taskID) {

        foreach (GameObject go in taskBoards) {
            Text t = go.transform.Find("UITextFront").GetComponent<Text>();
            if (taskID > 0)
            {
                if (taskID == 100)
                {
                    if (!trainingQuestionReminderOn)
                    {
                        t.text = "Training Task " + (trainingCounting - trainingCountringLeft + 1) + ": " + taskText;
                    }
                    else {
                        t.text = "Training Task " + (trainingCounting - trainingCountringLeft) + ": " + taskText;
                    }                
                }
                else {
                    t.text = "Task " + taskID + ": " + taskText;
                }

            }
            else {
                t.text = taskText;
                t.fontSize = 10;
            }
        }
    }

    string GetTChartName(int i) {
        switch (i)
        {
            case 1:
                return "Feb 26, 3pm - 4pm";
            case 2:
                return "Feb 26, 4pm - 5pm";
            case 3:
                return "Feb 26, 5pm - 6pm";
            case 4:
                return "Feb 26, 6pm - 7pm";
            case 5:
                return "Feb 26, 7pm - 8pm";
            case 6:
                return "Feb 27, 9am - 10am";
            case 7:
                return "Feb 27, 10am - 11am";
            case 8:
                return "Feb 27, 12pm - 1pm";
            case 9:
                return "Feb 27, 1pm - 2pm";
            case 10:
                return "Feb 27, 2pm - 3pm";
            case 11:
                return "Feb 27, 3pm - 4pm";
            case 12:
                return "Feb 27, 4pm - 5pm";
            default:
                return "";
        }
    }

    string GetBarChartName(int i)
    {
        switch (i)
        {
            case 1:
                return "Agricultural Land";
            case 2:
                return "Armed Forces Personnel";
            case 3:
                return "Births Attended";
            case 4:
                return "CO2 Emissions";
            case 5:
                return "Education Aid";
            case 6:
                return "Electricity Consumption";
            case 7:
                return "Exports";
            case 8:
                return "External Debt Rates";
            case 9:
                return "Food Consumption";
            case 10:
                return "Gross Capital Formation";
            case 11:
                return "Health Expenditures";
            case 12:
                return "HIV Prevalence";
            default:
                return "";
        }
    }

    string IntToMonth(int i) {
        switch (i)
        {
            case 1:
                return "January";
            case 2:
                return "February";
            case 3:
                return "March";
            case 4:
                return "April";
            case 5:
                return "May";
            case 6:
                return "June";
            case 7:
                return "July";
            case 8:
                return "August";
            case 9:
                return "September";
            case 10:
                return "October";
            case 11:
                return "November";
            case 12:
                return "December";
            default:
                return "404 error";
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

    void ReadACFile()
    {
        string[] lines = ACFile.text.Split(lineSeperater);
        foreach (string line in lines)
        {
            string[] fields = line.Split(fieldSeperator);
            bool header = false;
            foreach (string field in fields)
            {
                if (field == "\"\"")
                {
                    header = true;
                }
            }
            if (!header)
            {
                string[] formats = {"d/MM/yyyy h:mm:ss tt", "d/MM/yyyy h:mm tt",
                    "dd/MM/yyyy hh:mm:ss tt", "d/M/yyyy h:mm:ss",
                    "d/M/yyyy hh:mm tt", "d/M/yyyy hh tt",
                    "d/M/yyyy h:mm", "d/M/yyyy h:mm",
                    "dd/MM/yyyy hh:mm", "d/M/yyyy hh:mm"};
                string name = (fields[8]).Replace("\"", "").Trim();
                DateTime dt;
                bool isSuccess = DateTime.TryParseExact(fields[1].Replace("\"", "").Trim(), formats,
                    new CultureInfo("en-US"),
                    DateTimeStyles.None,
                    out dt);
                if (isSuccess)
                {
                    float temp = -1;
                    float spHi = -1;
                    float spLo = -1;
                    string status = "Off";
                    string acUnit = "Off";
                    float roofTemp = -1;
                    if (fields[2] != "\"-\"")
                    {
                        temp = float.Parse((fields[2]).Replace("\"", "").Trim());
                    }
                    if (fields[3] != "\"-\"")
                    {
                        spHi = float.Parse((fields[3]).Replace("\"", "").Trim());
                    }
                    if (fields[4] != "\"-\"")
                    {
                        spLo = float.Parse((fields[4]).Replace("\"", "").Trim());
                    }
                    if (fields[5] != "\"-\"")
                    {
                        status = (fields[5]).Replace("\"", "").Trim();
                    }
                    if (fields[6] != "\"-\"")
                    {
                        acUnit = (fields[6]).Replace("\"", "").Trim();
                    }
                    if (fields[7] != "\"-\"")
                    {
                        roofTemp = float.Parse((fields[7]).Replace("\"", "").Trim());
                    }

					SensorReading sr = new SensorReading ();
					sr.dt = dt;
					sr.temp = temp;
					sr.spHi = spHi;
					sr.spLo = spLo;
					sr.status = status;
					sr.acUnit = acUnit;
					sr.roofTemp = roofTemp;

                    if (sensorsInfoList.ContainsKey(name))
                    {
                        sensorsInfoList[name].Add(sr);
                    }
                    else {
                        sensorsInfoList.Add(name, new HashSet<SensorReading>());
                        sensorsInfoList[name].Add(sr);
                    }
                }
            }
        }
    }
}