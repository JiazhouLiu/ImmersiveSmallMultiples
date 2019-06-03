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
        
        if (ExperimentID > 0) {
            ParticipantID = ExperimentID;

            switch (ExperimentID % 12)
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
                case 6:
                    ExperimentSequence = 6;
                    break;
                case 7:
                    ExperimentSequence = 7;
                    break;
                case 8:
                    ExperimentSequence = 8;
                    break;
                case 9:
                    ExperimentSequence = 9;
                    break;
                case 10:
                    ExperimentSequence = 10;
                    break;
                case 11:
                    ExperimentSequence = 11;
                    break;
                case 0:
                    ExperimentSequence = 12;
                    break;
                default:
                    break;
            }
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

            string logFileHeader = "TimeSinceStart,UserHeight,TrialID,ParticipantID,ExperimentSequence,Dataset,Layout,TaskID,TaskLevel,TrialState,CameraPosition.x,CameraPosition.y,CameraPosition.z,CameraEulerAngles.x," +
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
            writer.WriteLine("UserID,Task,Answer,CompletionTime,Dataset,Layout,QuestionLevel,QuestionID,CorrectAnswer");
            writer.Close();

            string writerEyeFilePath = "Assets/ExperimentData/ExperimentLog/Participant " + ParticipantID + "/Participant_" + ParticipantID + "_EyeTrackingLog.csv";
            writer = new StreamWriter(writerEyeFilePath, false);
            writer.WriteLine("TimeSinceStart,UserID,Task,Dataset,Layout,QuestionLevel,TrialState,RawGazeFromPupil2D.x,RawGazeFromPupil2D.y,SMHighlighted1,SMHighlighted2,SMGazed,GazePositionAfterCalculation.x,GazePositionAfterCalculation.y,GazePositionAfterCalculation.z");
            writer.Close();

            string writerHeadFilePath = "Assets/ExperimentData/ExperimentLog/Participant " + ParticipantID + "/Participant_" + ParticipantID + "_HeadPositionLog.csv";
            writer = new StreamWriter(writerHeadFilePath, false);
            writer.WriteLine("TimeSinceStart,UserID,Task,Dataset,Layout,QuestionLevel,TrialState,SMManagerPosition.x,SMManagerPosition.y,SMManagerPosition.z,SMManagerScale,CameraPosition.x,CameraPosition.y,CameraPosition.z,CameraEulerAngles.x,CameraEulerAngles.y,CameraEulerAngles.z");
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
                forceStopTrainingForCombinition = true;

                lastTimePast = lastTime;
                userHeight = lastUserHeight;

                switch (ExperimentSequence)
                {
                    case 1: 
                        if (TrialNumber >= (0 * taskNo + 1) && TrialNumber <= (1 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Flat");
                        }
                        else if (TrialNumber >= (1 * taskNo + 1) && TrialNumber <= (2 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Full Circle");
                        }
                        else if (TrialNumber >= (2 * taskNo + 1) && TrialNumber <= (3 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Half Circle");
                        }
                        if (TrialNumber >= (3 * taskNo + 1) && TrialNumber <= (4 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                        }
                        else if (TrialNumber >= (4 * taskNo + 1) && TrialNumber <= (5 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                        }
                        else if (TrialNumber >= (5 * taskNo + 1) && TrialNumber <= (6 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                        }
                        break;
                    case 2:
                        if (TrialNumber >= (0 * taskNo + 1) && TrialNumber <= (1 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                        }
                        else if (TrialNumber >= (1 * taskNo + 1) && TrialNumber <= (2 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                        }
                        else if (TrialNumber >= (2 * taskNo + 1) && TrialNumber <= (3 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                        }
                        if (TrialNumber >= (3 * taskNo + 1) && TrialNumber <= (4 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Flat");
                        }
                        else if (TrialNumber >= (4 * taskNo + 1) && TrialNumber <= (5 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Full Circle");
                        }
                        else if (TrialNumber >= (5 * taskNo + 1) && TrialNumber <= (6 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Half Circle");
                        }
                        break;
                    case 3:
                        if (TrialNumber >= (0 * taskNo + 1) && TrialNumber <= (1 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Flat");
                        }
                        else if (TrialNumber >= (1 * taskNo + 1) && TrialNumber <= (2 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Half Circle");
                        }
                        else if (TrialNumber >= (2 * taskNo + 1) && TrialNumber <= (3 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Full Circle");
                        }
                        if (TrialNumber >= (3 * taskNo + 1) && TrialNumber <= (4 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                        }
                        else if (TrialNumber >= (4 * taskNo + 1) && TrialNumber <= (5 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                        }
                        else if (TrialNumber >= (5 * taskNo + 1) && TrialNumber <= (6 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                        }
                        break;
                    case 4:
                        if (TrialNumber >= (0 * taskNo + 1) && TrialNumber <= (1 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                        }
                        else if (TrialNumber >= (1 * taskNo + 1) && TrialNumber <= (2 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                        }
                        else if (TrialNumber >= (2 * taskNo + 1) && TrialNumber <= (3 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                        }
                        if (TrialNumber >= (3 * taskNo + 1) && TrialNumber <= (4 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Flat");
                        }
                        else if (TrialNumber >= (4 * taskNo + 1) && TrialNumber <= (5 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Half Circle");
                        }
                        else if (TrialNumber >= (5 * taskNo + 1) && TrialNumber <= (6 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Full Circle");
                        }
                        break;
                    case 5:
                        if (TrialNumber >= (0 * taskNo + 1) && TrialNumber <= (1 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Full Circle");
                        }
                        else if (TrialNumber >= (1 * taskNo + 1) && TrialNumber <= (2 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Flat");
                        }
                        else if (TrialNumber >= (2 * taskNo + 1) && TrialNumber <= (3 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Half Circle");
                        }
                        if (TrialNumber >= (3 * taskNo + 1) && TrialNumber <= (4 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                        }
                        else if (TrialNumber >= (4 * taskNo + 1) && TrialNumber <= (5 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                        }
                        else if (TrialNumber >= (5 * taskNo + 1) && TrialNumber <= (6 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                        }
                        break;
                    case 6:
                        if (TrialNumber >= (0 * taskNo + 1) && TrialNumber <= (1 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                        }
                        else if (TrialNumber >= (1 * taskNo + 1) && TrialNumber <= (2 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                        }
                        else if (TrialNumber >= (2 * taskNo + 1) && TrialNumber <= (3 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                        }
                        if (TrialNumber >= (3 * taskNo + 1) && TrialNumber <= (4 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Full Circle");
                        }
                        else if (TrialNumber >= (4 * taskNo + 1) && TrialNumber <= (5 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Flat");
                        }
                        else if (TrialNumber >= (5 * taskNo + 1) && TrialNumber <= (6 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Half Circle");
                        }
                        break;
                    case 7:
                        if (TrialNumber >= (0 * taskNo + 1) && TrialNumber <= (1 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Full Circle");
                        }
                        else if (TrialNumber >= (1 * taskNo + 1) && TrialNumber <= (2 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Half Circle");
                        }
                        else if (TrialNumber >= (2 * taskNo + 1) && TrialNumber <= (3 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Flat");
                        }
                        if (TrialNumber >= (3 * taskNo + 1) && TrialNumber <= (4 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                        }
                        else if (TrialNumber >= (4 * taskNo + 1) && TrialNumber <= (5 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                        }
                        else if (TrialNumber >= (5 * taskNo + 1) && TrialNumber <= (6 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                        }
                        break;
                    case 8:
                        if (TrialNumber >= (0 * taskNo + 1) && TrialNumber <= (1 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                        }
                        else if (TrialNumber >= (1 * taskNo + 1) && TrialNumber <= (2 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                        }
                        else if (TrialNumber >= (2 * taskNo + 1) && TrialNumber <= (3 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                        }
                        if (TrialNumber >= (3 * taskNo + 1) && TrialNumber <= (4 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Full Circle");
                        }
                        else if (TrialNumber >= (4 * taskNo + 1) && TrialNumber <= (5 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Half Circle");
                        }
                        else if (TrialNumber >= (5 * taskNo + 1) && TrialNumber <= (6 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Flat");
                        }
                        break;
                    case 9:
                        if (TrialNumber >= (0 * taskNo + 1) && TrialNumber <= (1 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Half Circle");
                        }
                        else if (TrialNumber >= (1 * taskNo + 1) && TrialNumber <= (2 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Flat");
                        }
                        else if (TrialNumber >= (2 * taskNo + 1) && TrialNumber <= (3 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Full Circle");
                        }
                        if (TrialNumber >= (3 * taskNo + 1) && TrialNumber <= (4 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                        }
                        else if (TrialNumber >= (4 * taskNo + 1) && TrialNumber <= (5 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                        }
                        else if (TrialNumber >= (5 * taskNo + 1) && TrialNumber <= (6 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                        }
                        break;
                    case 10:
                        if (TrialNumber >= (0 * taskNo + 1) && TrialNumber <= (1 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                        }
                        else if (TrialNumber >= (1 * taskNo + 1) && TrialNumber <= (2 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                        }
                        else if (TrialNumber >= (2 * taskNo + 1) && TrialNumber <= (3 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                        }
                        if (TrialNumber >= (3 * taskNo + 1) && TrialNumber <= (4 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Half Circle");
                        }
                        else if (TrialNumber >= (4 * taskNo + 1) && TrialNumber <= (5 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Flat");
                        }
                        else if (TrialNumber >= (5 * taskNo + 1) && TrialNumber <= (6 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Full Circle");
                        }
                        break;
                    case 11:
                        if (TrialNumber >= (0 * taskNo + 1) && TrialNumber <= (1 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Half Circle");
                        }
                        else if (TrialNumber >= (1 * taskNo + 1) && TrialNumber <= (2 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Full Circle");
                        }
                        else if (TrialNumber >= (2 * taskNo + 1) && TrialNumber <= (3 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Flat");
                        }
                        if (TrialNumber >= (3 * taskNo + 1) && TrialNumber <= (4 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                        }
                        else if (TrialNumber >= (4 * taskNo + 1) && TrialNumber <= (5 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                        }
                        else if (TrialNumber >= (5 * taskNo + 1) && TrialNumber <= (6 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                        }
                        break;
                    case 12:
                        if (TrialNumber >= (0 * taskNo + 1) && TrialNumber <= (1 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                        }
                        else if (TrialNumber >= (1 * taskNo + 1) && TrialNumber <= (2 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                        }
                        else if (TrialNumber >= (2 * taskNo + 1) && TrialNumber <= (3 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                        }
                        if (TrialNumber >= (3 * taskNo + 1) && TrialNumber <= (4 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Half Circle");
                        }
                        else if (TrialNumber >= (4 * taskNo + 1) && TrialNumber <= (5 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Full Circle");
                        }
                        else if (TrialNumber >= (5 * taskNo + 1) && TrialNumber <= (6 * taskNo))
                        {
                            SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Flat");
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
                    WriteToBoard("Before each task, you have free time to get familiar with small multiples data. The data and order of each small multiple will NOT change during the same visualisation experiment. Then, you have free time to read the question. After reading, you have a maximum of 60 seconds to finish the question." +
                        "Or, you can press the <color=green>Finish</color> button on the controller to start answering the question.\n\nNow, please stand still and press the <color=red>trigger</color> button on your right (R) hand to continue.");
                    if (rightControllerScript.triggerPressed)
                    {
                        userHeight = Camera.main.transform.position.y;

                        switch (ExperimentSequence)
                        {
                            case 1:
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Flat");
                                break;
                            case 2:
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                                break;
                            case 3:
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Flat");
                                break;
                            case 4:
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Flat");
                                break;
                            case 5:
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Full Circle");
                                break;
                            case 6:
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                                break;
                            case 7:
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Full Circle");
                                break;
                            case 8:
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Full Circle");
                                break;
                            case 9:
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Half Circle");
                                break;
                            case 10:
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
                                break;
                            case 11:
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Half Circle");
                                break;
                            case 12:
                                SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Half Circle");
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

    //public void ChangeState(GameObject controller, string button) {
    //    switch (state) {
    //        case 0:
    //            // load scene based on sequence
    //            if (ExperimentSequence <= 4 && ExperimentSequence > 0 && ParticipantID > 0)
    //            {
    //                switch (ExperimentSequence)
    //                {
    //                    case 1:
    //                        SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1 - Curved");
    //                        break;
    //                    case 2:
    //                        SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 1");
    //                        break;
    //                    case 3:
    //                        SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2 - Curved");
    //                        break;
    //                    case 4:
    //                        SceneManager.LoadScene(sceneName: "SmallMultiples - DataSet 2");
    //                        break;
    //                    default:
    //                        break;
    //                }
    //            }
    //            else {
    //                Debug.Log("Starting loading scene failed!");
    //            }
    //            break;
    //        case 1:
    //            // start position tut
    //            WriteToBoard("start position tut");
    //            if (button == "trigger") {
    //                state++;
    //                ChangeState(null, "");
    //            }
    //            break;
    //        case 2:
    //            // trigger button tut
    //            WriteToBoard("trigger button tut");
    //            if (controller != null) {
    //                if (controller.name == leftController && button == "trigger")
    //                {
    //                    SteamVR_TrackedControllerForStartScene tc = controller.GetComponent<SteamVR_TrackedControllerForStartScene>();
    //                    SteamVR_Controller.Input((int)tc.controllerIndex).TriggerHapticPulse(1000);
    //                    leftTriggerButtonPass = true;
    //                }
    //                else if (controller.name == rightController && button == "trigger")
    //                {
    //                    SteamVR_TrackedControllerForStartScene tc = controller.GetComponent<SteamVR_TrackedControllerForStartScene>();
    //                    SteamVR_Controller.Input((int)tc.controllerIndex).TriggerHapticPulse(1000);
    //                    rightTriggerButtonPass = true;
    //                }
    //            }
                
    //            break;
    //        case 3:
    //            // grip button tut
    //            WriteToBoard("grip button tut");
    //            if (controller != null)
    //            {
    //                if (controller.name == leftController && button == "grip")
    //                {
    //                    SteamVR_TrackedControllerForStartScene tc = controller.GetComponent<SteamVR_TrackedControllerForStartScene>();
    //                    SteamVR_Controller.Input((int)tc.controllerIndex).TriggerHapticPulse(1000);
    //                    leftGripButtonPass = true;
    //                }
    //                else if (controller.name == rightController && button == "grip")
    //                {
    //                    SteamVR_TrackedControllerForStartScene tc = controller.GetComponent<SteamVR_TrackedControllerForStartScene>();
    //                    SteamVR_Controller.Input((int)tc.controllerIndex).TriggerHapticPulse(1000);
    //                    rightGripButtonPass = true;
    //                }
    //            }
    //            break;
    //        case 4:
    //            // menu button tut
    //            WriteToBoard("menu button tut");
    //            if (controller != null)
    //            {
    //                if (controller.name == leftController && button == "menu")
    //                {
    //                    SteamVR_TrackedControllerForStartScene tc = controller.GetComponent<SteamVR_TrackedControllerForStartScene>();
    //                    SteamVR_Controller.Input((int)tc.controllerIndex).TriggerHapticPulse(1000);
    //                    leftMenuButtonPass = true;
    //                }
    //                else if (controller.name == rightController && button == "menu")
    //                {
    //                    SteamVR_TrackedControllerForStartScene tc = controller.GetComponent<SteamVR_TrackedControllerForStartScene>();
    //                    SteamVR_Controller.Input((int)tc.controllerIndex).TriggerHapticPulse(1000);
    //                    rightMenuButtonPass = true;
    //                }
    //            }
    //            break;
    //        case 5:
    //            // touchpad button tut
    //            WriteToBoard("touchpad button tut");
    //            if (controller != null)
    //            {
    //                if (controller.name == leftController && button == "TouchPad")
    //                {
    //                    SteamVR_TrackedControllerForStartScene tc = controller.GetComponent<SteamVR_TrackedControllerForStartScene>();
    //                    SteamVR_Controller.Input((int)tc.controllerIndex).TriggerHapticPulse(1000);
    //                    leftTouchPadButtonPass = true;
    //                }
    //                else if (controller.name == rightController && button == "TouchPad")
    //                {
    //                    SteamVR_TrackedControllerForStartScene tc = controller.GetComponent<SteamVR_TrackedControllerForStartScene>();
    //                    SteamVR_Controller.Input((int)tc.controllerIndex).TriggerHapticPulse(1000);
    //                    rightTouchPadButtonPass = true;
    //                }
    //            }
    //            break;
    //        case 6:
    //            // stand still and press trigger button to record height
    //            WriteToBoard("stand still and press trigger button to record height");
    //            userHeight = Camera.main.transform.position.y;
    //            //Debug.Log(Camera.main.transform.position.y);
    //            state = 0;
    //            break;
    //        default:
    //            break;
    //    }
    //}


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
