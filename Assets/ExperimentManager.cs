using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using UnityEngine.UI;

public class ExperimentManager : MonoBehaviour {

    public int ExperimentID;
    public int TrialNumber;

    public static int ExperimentSequence;
    public static int ParticipantID;
    public static string CurrentDateTime;
    public static int PublicTrialNumber;
    public static string writerFilePath;
    public static float lastTimePast;
    public static float userHeight;
    public static bool comprehensiveTraining = true;
    public static bool forceStopTrainingForCombinition = false;
    public static int sceneCounter = 0;

    private bool leftTriggerButtonPass = false;
    private bool rightTriggerButtonPass = false;
    private bool leftGripButtonPass = false;
    private bool rightGripButtonPass = false;
    private bool leftMenuButtonPass = false;
    private bool rightMenuButtonPass = false;
    private bool leftTouchPadButtonPass = false;
    private bool rightTouchPadButtonPass = false;
    private int state = 1;
    private List<GameObject> taskBoards;
    private int taskNo = 4;

    private string leftController = "Controller (left)";
    private string rightController = "Controller (right)";

    // Use this for initialization
    void Start () {
        
        // add task boards to list
        taskBoards = new List<GameObject>();
        taskBoards.Add(GameObject.Find("TaskBoardLeft"));
        taskBoards.Add(GameObject.Find("TaskBoardRight"));
        taskBoards.Add(GameObject.Find("TaskBoardTop"));

        PublicTrialNumber = TrialNumber;

        if (ExperimentID > 0)
        {
            ParticipantID = ExperimentID;

            switch (ExperimentID % 6)
            {
                case 1:
                    ExperimentSequence = 1;
                    break;
                case 2:
                    ExperimentSequence = 2;
                    break;
                case 3:
                    ExperimentSequence = 3;
                    break;
                case 4:
                    ExperimentSequence = 4;
                    break;
                case 5:
                    ExperimentSequence = 5;
                    break;
                case 0:
                    ExperimentSequence = 6;
                    break;
                default:
                    break;
            }
        }
        else { // testing stream
            ExperimentSequence = 1;
        }

        
        if (TrialNumber == 0)
        {
            CurrentDateTime = GetDateTimeString();
            writerFilePath = "Assets/ExperimentData/ExperimentLog/Participant " + ParticipantID + "/Participant_" + ParticipantID + "_RawData.csv";
            //string logFileHeader = "TimeSinceStart,UserHeight,TrialID,ParticipantID,ExperimentSequence,Dataset,Layout,TaskID,TaskStarted,CameraPosition.x,CameraPosition.y,CameraPosition.z,CameraEulerAngles.x," +
            //    "CameraEulerAngles.y,CameraEulerAngles.z,LeftControllerPosition.x,LeftControllerPosition.y,LeftControllerPosition.z," +
            //    "LeftControllerEulerAngles.x,LeftControllerEulerAngles.y,LeftControllerEulerAngles.z,RightControllerPosition.x,RightControllerPosition.y,RightControllerPosition.z," +
            //    "RightControllerEulerAngles.x,RightControllerEulerAngles.y,RightControllerEulerAngles.z,LeftMenuButtonPressed," +
            //    "LeftTriggerButtonPressed,LeftGripButtonPressed,LeftPadTopPressed,LeftPadBottomPressed,RightMenuButtonPressed," +
            //    "RightTriggerButtonPressed,RightGripButtonPressed,RightPadTopPressed,RightPadLeftPressed,RightPadRightPressed,RightPadBottomPressed," +
            //    "RotatingClockwise,RotatingAnticlockwise," +
            //    "LeftPillarToLeft,LeftPillarToRight,RightPillarToLeft,RightPillarToRight,(Shortcut)IncreasingCurvature,(Shortcut)DecreasingCurvature,(Shortcut)IncreasingRowNumber," +
            //    "(Shortcut)DecreasingRowNumber,TogglingSMOrientation";

            string logFileHeader = "TimeSinceStart,UserHeight,TrialID,ParticipantID,ExperimentSequence,Dataset,Layout,TaskID,QuestionType,TrialState,CameraPosition.x,CameraPosition.y,CameraPosition.z,CameraEulerAngles.x," +
                "CameraEulerAngles.y,CameraEulerAngles.z,LeftControllerPosition.x,LeftControllerPosition.y,LeftControllerPosition.z," +
                "LeftControllerEulerAngles.x,LeftControllerEulerAngles.y,LeftControllerEulerAngles.z,RightControllerPosition.x,RightControllerPosition.y,RightControllerPosition.z," +
                "RightControllerEulerAngles.x,RightControllerEulerAngles.y,RightControllerEulerAngles.z,LeftMenuButtonPressed," +
                "LeftTriggerButtonPressed,LeftGripButtonPressed,LeftPadPressed,RightMenuButtonPressed," +
                "RightTriggerButtonPressed,RightGripButtonPressed,RightPadPressed,RotatingClockwise,RotatingAnticlockwise," +
                "ScaleUp,ScaleDown,SMManagerPosition.x,SMManagerPosition.y,SMManagerPosition.z,SMManagerScale,SMRotationDiff,FilterXMin,FilterXMax,FilterYMin,FilterYMax,FilterZMin,FilterZMax," +
                "RawGazeFromPupil2D.x,RawGazeFromPupil2D.y,SMGazed,GazePositionAfterCalculation.x,GazePositionAfterCalculation.y,GazePositionAfterCalculation.z";

            StreamWriter writer = new StreamWriter(writerFilePath, false);
            writer.WriteLine(logFileHeader);
            writer.Close();

            string writerAnswerFilePath = "Assets/ExperimentData/ExperimentLog/Participant " + ParticipantID + "/Participant_" + ParticipantID + "_Answers.csv";
            writer = new StreamWriter(writerAnswerFilePath, false);
            writer.WriteLine("UserID,TrialID,TaskID,Answer,CompletionTime,Dataset,Layout,QuestionType,QuestionID,CorrectAnswer");
            writer.Close();

            string writerEyeFilePath = "Assets/ExperimentData/ExperimentLog/Participant " + ParticipantID + "/Participant_" + ParticipantID + "_EyeTrackingLog.csv";
            writer = new StreamWriter(writerEyeFilePath, false);
            writer.WriteLine("TimeSinceStart,UserID,TrialID,TaskID,Dataset,Layout,QuestionType,TrialState,RawGazeFromPupil2D.x,RawGazeFromPupil2D.y,SMHighlighted1,SMHighlighted2,SMGazed,GazePositionAfterCalculation.x,GazePositionAfterCalculation.y,GazePositionAfterCalculation.z");
            writer.Close();

            string writerHeadFilePath = "Assets/ExperimentData/ExperimentLog/Participant " + ParticipantID + "/Participant_" + ParticipantID + "_HeadPositionLog.csv";
            writer = new StreamWriter(writerHeadFilePath, false);
            writer.WriteLine("TimeSinceStart,UserID,TrialID,TaskID,Dataset,Layout,QuestionType,TrialState,SMManagerPosition.x,SMManagerPosition.y,SMManagerPosition.z,SMManagerScale,CameraPosition.x,CameraPosition.y,CameraPosition.z,CameraEulerAngles.x,CameraEulerAngles.y,CameraEulerAngles.z");
            writer.Close();
        }
        else {
            string lastFileName = "";

            string folderPath = "Assets/ExperimentData/ExperimentLog/Participant " + ParticipantID + "/";
            DirectoryInfo info = new DirectoryInfo(folderPath);
            FileInfo[] fileInfo = info.GetFiles();
            foreach (FileInfo file in fileInfo)
            {
                if (file.Name.Contains("Participant_" + ParticipantID + "_RawData.csv") && !file.Name.Contains("meta"))
                {
                    lastFileName = file.Name;
                }
            }
            if (lastFileName == "")
            {
                Debug.LogError("No previous file found!");
            }
            else {
                writerFilePath = "Assets/ExperimentData/ExperimentLog/Participant " + ParticipantID + "/" + lastFileName;
                //Debug.Log(File.ReadAllLines(writerFilePath).Length);
                string lastLine = File.ReadAllLines(writerFilePath)[File.ReadAllLines(writerFilePath).Length - 1];
                float lastTime = float.Parse(lastLine.Split(',')[0]);

                float lastUserHeight = float.Parse(lastLine.Split(',')[1]);

                // stop start training and stop combinition training
                comprehensiveTraining = false;

                lastTimePast = lastTime;
                userHeight = lastUserHeight;

                switch (ExperimentSequence)
                {
                    case 1:
                        if ((TrialNumber % 15 >= 1) && (TrialNumber % 15 <= 5)) {
                            if (TrialNumber > 15)
                            {
                                sceneCounter = 3;
                            }
                            else {
                                sceneCounter = 0;
                            }
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                        }
                        else if ((TrialNumber % 15 >= 6) && (TrialNumber % 15 <= 10))
                        {
                            if (TrialNumber > 15)
                            {
                                sceneCounter = 4;
                            }
                            else
                            {
                                sceneCounter = 1;
                            }
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                        }
                        else if (((TrialNumber % 15 >= 11) && (TrialNumber % 15 < 15)) || TrialNumber % 15 == 0)
                        {
                            if (TrialNumber > 15)
                            {
                                sceneCounter = 5;
                            }
                            else
                            {
                                sceneCounter = 2;
                            }
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                        }
                        break;
                    case 2:
                        if ((TrialNumber % 15 >= 1) && (TrialNumber % 15 <= 5))
                        {
                            if (TrialNumber > 15)
                            {
                                sceneCounter = 3;
                            }
                            else
                            {
                                sceneCounter = 0;
                            }
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                        }
                        else if ((TrialNumber % 15 >= 6) && (TrialNumber % 15 <= 10))
                        {
                            if (TrialNumber > 15)
                            {
                                sceneCounter = 4;
                            }
                            else
                            {
                                sceneCounter = 1;
                            }
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                        }
                        else if (((TrialNumber % 15 >= 11) && (TrialNumber % 15 < 15)) || TrialNumber % 15 == 0)
                        {
                            if (TrialNumber > 15)
                            {
                                sceneCounter = 5;
                            }
                            else
                            {
                                sceneCounter = 2;
                            }
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                        }
                        break;
                    case 3:
                        if ((TrialNumber % 15 >= 1) && (TrialNumber % 15 <= 5))
                        {
                            if (TrialNumber > 15)
                            {
                                sceneCounter = 3;
                            }
                            else
                            {
                                sceneCounter = 0;
                            }
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                        }
                        else if ((TrialNumber % 15 >= 6) && (TrialNumber % 15 <= 10))
                        {
                            if (TrialNumber > 15)
                            {
                                sceneCounter = 4;
                            }
                            else
                            {
                                sceneCounter = 1;
                            }
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                        }
                        else if (((TrialNumber % 15 >= 11) && (TrialNumber % 15 < 15)) || TrialNumber % 15 == 0)
                        {
                            if (TrialNumber > 15)
                            {
                                sceneCounter = 5;
                            }
                            else
                            {
                                sceneCounter = 2;
                            }
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                        }
                        break;
                    case 4:
                        if ((TrialNumber % 15 >= 1) && (TrialNumber % 15 <= 5))
                        {
                            if (TrialNumber > 15)
                            {
                                sceneCounter = 3;
                            }
                            else
                            {
                                sceneCounter = 0;
                            }
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                        }
                        else if ((TrialNumber % 15 >= 6) && (TrialNumber % 15 <= 10))
                        {
                            if (TrialNumber > 15)
                            {
                                sceneCounter = 4;
                            }
                            else
                            {
                                sceneCounter = 1;
                            }
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                        }
                        else if (((TrialNumber % 15 >= 11) && (TrialNumber % 15 < 15)) || TrialNumber % 15 == 0)
                        {
                            if (TrialNumber > 15)
                            {
                                sceneCounter = 5;
                            }
                            else
                            {
                                sceneCounter = 2;
                            }
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                        }
                        break;
                    case 5:
                        if ((TrialNumber % 15 >= 1) && (TrialNumber % 15 <= 5))
                        {
                            if (TrialNumber > 15)
                            {
                                sceneCounter = 3;
                            }
                            else
                            {
                                sceneCounter = 0;
                            }
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                        }
                        else if ((TrialNumber % 15 >= 6) && (TrialNumber % 15 <= 10))
                        {
                            if (TrialNumber > 15)
                            {
                                sceneCounter = 4;
                            }
                            else
                            {
                                sceneCounter = 1;
                            }
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                        }
                        else if (((TrialNumber % 15 >= 11) && (TrialNumber % 15 < 15)) || TrialNumber % 15 == 0)
                        {
                            if (TrialNumber > 15)
                            {
                                sceneCounter = 5;
                            }
                            else
                            {
                                sceneCounter = 2;
                            }
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                        }
                        break;
                    case 6:
                        if ((TrialNumber % 15 >= 1) && (TrialNumber % 15 <= 5))
                        {
                            if (TrialNumber > 15)
                            {
                                sceneCounter = 3;
                            }
                            else
                            {
                                sceneCounter = 0;
                            }
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                        }
                        else if ((TrialNumber % 15 >= 6) && (TrialNumber % 15 <= 10))
                        {
                            if (TrialNumber > 15)
                            {
                                sceneCounter = 4;
                            }
                            else
                            {
                                sceneCounter = 1;
                            }
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                        }
                        else if (((TrialNumber % 15 >= 11) && (TrialNumber % 15 < 15)) || TrialNumber % 15 == 0)
                        {
                            if (TrialNumber > 15)
                            {
                                sceneCounter = 5;
                            }
                            else
                            {
                                sceneCounter = 2;
                            }
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "StartScene") {
            GameObject leftControllerGO = GameObject.Find(leftController);
            GameObject rightControllerGO = GameObject.Find(rightController);

            if (Camera.main != null && rightControllerGO != null && leftControllerGO != null) {
                if (state == 1)
                {
                    WriteToBoard("Please stay behind the <color=yellow>yellow line</color> during the whole experiment. At the beginning, you will have a training session.\n\nNow, please press the <color=red>trigger</color> button on your left (L) hand to continue.");
                    SteamVR_TrackedControllerForStartScene leftControllerScript = leftControllerGO.GetComponent<SteamVR_TrackedControllerForStartScene>();
                    if (leftControllerScript.triggerPressed)
                    {
                        state = 2;
                    }
                }
                else if (state == 2)
                {
                    SteamVR_TrackedControllerForStartScene rightControllerScript = rightControllerGO.GetComponent<SteamVR_TrackedControllerForStartScene>();
                    WriteToBoard("Before each task, you have free time to get familiar with small multiples data. Then, you have free time to read the question. After reading, you will be navigated to work on the questions. Please finish as quickly as possible. " +
                        "When you get the result, press the <color=green>Finish</color> button on the controller to start answering the question.\n\nNow, please stand still and press the <color=red>trigger</color> button on your right (R) hand to continue.");
                    if (rightControllerScript.triggerPressed)
                    {
                        userHeight = Camera.main.transform.position.y;

                        switch (ExperimentSequence)
                        {
                            case 1:
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                                break;
                            case 2:
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                                break;
                            case 3:
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                                break;
                            case 4:
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                                break;
                            case 5:
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                                break;
                            case 6:
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                                break; 
                            default:
                                break;
                        }
                    }
                }
                else {
                    state = 1;
                } 
            }   
        }
    }

    // utilities
    private void WriteToBoard(string text) {
        if (taskBoards.Count > 0) {
            foreach (GameObject go in taskBoards)
            {
                Text t = go.transform.Find("UITextFront").GetComponent<Text>();
                t.fontSize = 10;
                t.text = text;
            }
        }
        
    }

    string GetDateTimeString() {
        return DateTime.Now.Month.ToString("D2") +  DateTime.Now.Day.ToString("D2") + "-" + DateTime.Now.Hour.ToString("D2") + DateTime.Now.Minute.ToString("D2") + DateTime.Now.Second.ToString("D2");
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
