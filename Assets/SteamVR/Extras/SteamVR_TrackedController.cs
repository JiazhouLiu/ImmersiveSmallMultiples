//======= Copyright (c) Valve Corporation, All rights reserved. ===============
using UnityEngine;
using Valve.VR;
using VRTK;
using TMPro;

public struct ClickedEventArgs
{
    public uint controllerIndex;
    public uint flags;
    public float padX, padY;
}

public delegate void ClickedEventHandler(object sender, ClickedEventArgs e);

public class SteamVR_TrackedController : MonoBehaviour
{
    public uint controllerIndex;
    public VRControllerState_t controllerState;
    public bool triggerPressed = false;
    public bool steamPressed = false;
    public bool menuPressed = false;
    public bool padPressed = false;
    public bool padTouched = false;
    public bool gripped = false;

    public bool controlBallTouched = false;

    public GameObject building;
    //BuildingScript buildS;
    int smallMultipleNumbers;
    SmallMultiplesManagerScript mrs;
    // variable to store the last y value;
    float z = 0.0f;
    float zBeta = 0.0f;

    bool indirectTouched = false;

    //float uniqueCenterZDelta = -100;

    bool RotateGripped = false;
    //float oldFacingValue = 0f;
    Vector3 oldLeftControllerPosition = Vector3.zero;

    public bool confirmed = false;
    public bool readQuestion = false;

    // shelf movement
    
    //Vector3 controllerShelfDelta = Vector3.zero;
    //Vector3 oldRightControllerPosition = Vector3.zero;
    //float semiCircleCurvature = 146f;
    //Vector3 oldEulerAngle = Vector3.zero;

    public bool clockwiseRotation = false;
    public bool antiClockwiseRotation = false;

    public event ClickedEventHandler MenuButtonClicked;
    public event ClickedEventHandler MenuButtonUnclicked;
    public event ClickedEventHandler TriggerClicked;
    public event ClickedEventHandler TriggerUnclicked;
    public event ClickedEventHandler SteamClicked;
    public event ClickedEventHandler PadClicked;
    public event ClickedEventHandler PadUnclicked;
    public event ClickedEventHandler PadTouched;
    public event ClickedEventHandler PadUntouched;
    public event ClickedEventHandler Gripped;
    public event ClickedEventHandler Ungripped;

    GameObject MultipleManager;

    public bool bothGripFlag = false;
    // leftController grip to rotate
    public bool canRotate = true;
    // rightController grip to move
    public bool canMove = true;

    // dataset2 highlighted
    bool keepHighlighted = false;

    float currentRotation = 0.0f;

    public GameObject touchedFilter = null;
    public int triggerTimer = 0;

    bool flag = false;


    // Use this for initialization
    protected virtual void Start()
    {
        MultipleManager = GameObject.Find("SmallMultiplesManager");
        mrs = GameObject.Find("SmallMultiplesManager").GetComponent<SmallMultiplesManagerScript>();

        smallMultipleNumbers = mrs.smallMultiplesNumber;

        if (this.GetComponent<SteamVR_TrackedObject>() == null)
        {
            gameObject.AddComponent<SteamVR_TrackedObject>();
        }

        if (controllerIndex != 0)
        {
            this.GetComponent<SteamVR_TrackedObject>().index = (SteamVR_TrackedObject.EIndex)controllerIndex;
            if (this.GetComponent<SteamVR_RenderModel>() != null)
            {
                this.GetComponent<SteamVR_RenderModel>().index = (SteamVR_TrackedObject.EIndex)controllerIndex;
            }
        }
        else
        {
            controllerIndex = (uint)this.GetComponent<SteamVR_TrackedObject>().index;
        }
    }

    public void SetDeviceIndex(int index)
    {
        this.controllerIndex = (uint)index;
    }

    public virtual void OnTriggerClicked(ClickedEventArgs e)
    {
        if (TriggerClicked != null)
            TriggerClicked(this, e);
        
        CollisionDetection cd = transform.GetChild(0).Find("tip").GetChild(0).GetChild(0).GetChild(0).GetComponent<CollisionDetection>();
        triggerTimer = 1;
        if (mrs.dataset == 2)
        {
            GameObject touchedFilter = cd.draggedFilter;
            GameObject touchedAxis = cd.touchedAxis;
            if (this.name == ("Controller (left)"))
            {
                if (touchedFilter == null || (touchedFilter != null && touchedAxis != null)) {
                    if (touchedAxis != null)
                    {
                        if (!touchedAxis.transform.parent.parent.name.Contains("Budget"))
                        {
                            mrs.CheckDataset2KeepHighted("left", transform.GetChild(0).Find("tip").GetChild(0).GetChild(0).GetChild(0).position);
                        }
                    }
                    else
                    {
                        mrs.CheckDataset2KeepHighted("left", transform.GetChild(0).Find("tip").GetChild(0).GetChild(0).GetChild(0).position);
                    }
                }
            }
            else
            {
                //Debug.Log(touchedAxis);
                if (touchedFilter == null || (touchedFilter != null && touchedAxis != null)) {
                    if (touchedAxis != null)
                    {
                        if (!touchedAxis.transform.parent.parent.name.Contains("Budget"))
                        {
                            mrs.CheckDataset2KeepHighted("right", transform.GetChild(0).Find("tip").GetChild(0).GetChild(0).GetChild(0).position);
                        }
                    }
                    else {
                        mrs.CheckDataset2KeepHighted("right", transform.GetChild(0).Find("tip").GetChild(0).GetChild(0).GetChild(0).position);
                    }
                }
            }
            //mrs.triggerPressedForFilterMoving = true;
        }
    }

    public virtual void OnTriggerUnclicked(ClickedEventArgs e)
    {
        if (TriggerUnclicked != null)
            TriggerUnclicked(this, e);
        triggerTimer = 0;
        if (mrs.dataset == 2)
        {
            //mrs.triggerPressedForFilterMoving = false;
        }
        flag = false;
        CollisionDetection cd = transform.GetChild(0).Find("tip").GetChild(0).GetChild(0).GetChild(0).GetComponent<CollisionDetection>();
        cd.controllerStartPressingPosition = Vector3.zero;
        cd.currentCube = null;

        if (this.name == ("Controller (left)"))
        {
            mrs.ControllerTriggerReleased("left");
        }
        else {
            mrs.ControllerTriggerReleased("right");
        }
    }

    void RecentrePosition(float distance)
    {
        GameObject viveCamera = GameObject.Find("Camera (eye)");

        Transform shelf = MultipleManager.transform.Find("Shelf");
        //mrs.MoveShelfToCenter();

        shelf.transform.position = viveCamera.transform.position + (viveCamera.transform.forward * distance) - (viveCamera.transform.up * 0.4f);

        Vector3 camPos = viveCamera.transform.position;
        Vector3 finalPos = new Vector3(camPos.x, shelf.transform.position.y, camPos.z);
        Vector3 offset = shelf.transform.position - finalPos;
        shelf.transform.LookAt(shelf.transform.position + offset);
    }


    public virtual void OnMenuClicked(ClickedEventArgs e)
    {
        if (MenuButtonClicked != null)
            MenuButtonClicked(this, e);

        mrs.ShowTaskForTraining();
        //if (this.name.Equals("Controller (left)"))
        //{
        //    if (!mrs.startTask)
        //    {
        //        mrs.startTask = true;
        //    }
        //    else
        //    {
        //        mrs.startTask = false;
        //    }
        //}
        //if (!mrs.hidePillarsAndBoards)
        //{

        //    if (this.name.Equals("Controller (right)"))
        //    {
        //        mrs.ToggleFaceCurve();
        //    }
        //}
        //if (mrs.scaling)
        //{
        //    mrs.scaling = false;
        //}
        //else {
        //    mrs.scaling = true;
        //}
    }

    public virtual void OnMenuUnclicked(ClickedEventArgs e)
    {
        if (MenuButtonUnclicked != null)
            MenuButtonUnclicked(this, e);
    }

    public virtual void OnSteamClicked(ClickedEventArgs e)
    {
        if (SteamClicked != null)
            SteamClicked(this, e);
    }

    public virtual void OnPadClicked(ClickedEventArgs e)
    {
        if (PadClicked != null)
            PadClicked(this, e);
        if (Camera.main != null) {
            if (mrs.trialState == TrialState.PreTask)
            {
                if (mrs.interactionTrainingNeeded)
                {
                    if (GameObject.Find("Controller (left)") != null && GameObject.Find("Controller (right)") != null)
                    {
                        GameObject.Find("Controller (left)").transform.Find("TrackPadLabel").GetComponent<TextMeshPro>().text = "Next";
                        GameObject.Find("Controller (right)").transform.Find("TrackPadLabel").GetComponent<TextMeshPro>().text = "Next";
                    }
                    if (name == "Controller (left)")
                    {
                        mrs.SetupPreTaskEnvironment("left");
                    }
                    else
                    {
                        mrs.SetupPreTaskEnvironment("right");
                    }
                }
                else
                {
                    if (!confirmed)
                    {
                        if (!mrs.calibrationFlag)
                            mrs.OpenPupilCamera();
                        if (mrs.trainingCountingLeft > 0)
                        {
                            mrs.ChangeTaskText("The next question is a <color=red>training</color> question. You have multiple chances to answer this question until you get the correct answer.\n\n" +
                                "Press <color=green>Read</color> to read the question.\n\n", -1);
                        }
                        else {
                            mrs.ChangeTaskText("The next question is an <color=red>experiment</color> question. Please solve the question as quickly as possible.\n\n" +
                                "Press <color=green>Read</color> to read the question.\n\n", -1);
                        }
                       // mrs.ChangeTaskText("Are you sure you want to start now? Please solve the question as quickly as possible. \n\nPress <color=green>Read</color> to read the question.\n\n", -1);

                        if (GameObject.Find("Controller (left)") != null && GameObject.Find("Controller (right)") != null)
                        {
                            GameObject.Find("Controller (left)").transform.Find("TrackPadLabel").GetComponent<TextMeshPro>().text = "Read";
                            GameObject.Find("Controller (right)").transform.Find("TrackPadLabel").GetComponent<TextMeshPro>().text = "Read";
                        }

                        confirmed = true;
                        if (this.name == ("Controller (left)") && GameObject.Find("Controller (right)") != null)
                        {
                            GameObject.Find("Controller (right)").GetComponent<SteamVR_TrackedController>().confirmed = true;
                        }
                        else if (this.name == ("Controller (right)") && GameObject.Find("Controller (left)") != null)
                        {
                            GameObject.Find("Controller (left)").GetComponent<SteamVR_TrackedController>().confirmed = true;
                        }
                    }
                    else if (!readQuestion)
                    {
                        mrs.ChangeTaskText(mrs.GetTaskText() + "\n\nPress <color=green>Start</color> to solve the question.\n\n", -1);
                        if (MultipleManager.transform.localPosition.y > -50f)
                            MultipleManager.transform.localPosition -= Vector3.up * 100;
                        if (GameObject.Find("Controller (left)") != null && GameObject.Find("Controller (right)") != null)
                        {
                            GameObject.Find("Controller (left)").transform.Find("TrackPadLabel").GetComponent<TextMeshPro>().text = "Start";
                            GameObject.Find("Controller (right)").transform.Find("TrackPadLabel").GetComponent<TextMeshPro>().text = "Start";
                        }
                        readQuestion = true;
                        if (this.name == ("Controller (left)") && GameObject.Find("Controller (right)") != null)
                        {
                            GameObject.Find("Controller (right)").GetComponent<SteamVR_TrackedController>().readQuestion = true;
                        }
                        else if (this.name == ("Controller (right)") && GameObject.Find("Controller (left)") != null)
                        {
                            GameObject.Find("Controller (left)").GetComponent<SteamVR_TrackedController>().readQuestion = true;
                        }
                    }
                    else
                    {
                        if (MultipleManager.transform.localPosition.y < -50f)
                            MultipleManager.transform.localPosition += Vector3.up * 100;
                        confirmed = false;
                        readQuestion = false;
                        if (this.name == ("Controller (left)") && GameObject.Find("Controller (right)") != null)
                        {
                            GameObject.Find("Controller (right)").GetComponent<SteamVR_TrackedController>().confirmed = false;
                            GameObject.Find("Controller (right)").GetComponent<SteamVR_TrackedController>().readQuestion = false;
                        }
                        else if (this.name == ("Controller (right)") && GameObject.Find("Controller (left)") != null)
                        {
                            GameObject.Find("Controller (left)").GetComponent<SteamVR_TrackedController>().confirmed = false;
                            GameObject.Find("Controller (left)").GetComponent<SteamVR_TrackedController>().readQuestion = false;
                        }

                        mrs.StartTask();
                    }
                }
            }
            else if (mrs.trialState == TrialState.OnTask)
            {
                mrs.FinishOrTimeUpToAnswer();
            }
        }
        
    }

    public virtual void OnPadUnclicked(ClickedEventArgs e)
    {
        if (PadUnclicked != null)
            PadUnclicked(this, e);
        //if (!mrs.hidePillarsAndBoards)
        //{
        //    if (this.name.Equals("Controller (right)"))
        //    {
        //        mrs.faceToCurve = true;
        //        mrs.ToggleFaceCurve();
        //    }
        //}
    }

    public virtual void OnPadTouched(ClickedEventArgs e)
    {
        if (PadTouched != null)
            PadTouched(this, e);
    }

    public virtual void OnPadUntouched(ClickedEventArgs e)
    {
        if (PadUntouched != null)
            PadUntouched(this, e);
    }

    public virtual void OnGripped(ClickedEventArgs e)
    {
        if (Gripped != null)
            Gripped(this, e);

        //Transform SMManagerT = GameObject.Find("SmallMultiplesManager").transform;

        //if (this.name.Equals("Controller (left)"))
        //{
        //    if (canRotate)
        //    {
        //        //z = transform.localPosition.x;
        //        //zBeta = transform.localPosition.z;
        //        oldEulerAngle = SMManagerT.eulerAngles;
        //        oldLeftControllerPosition = transform.position;
        //    }
        //}
        //else {
        //    if (canMove) {
        //        oldEulerAngle = SMManagerT.eulerAngles;
        //        oldRightControllerPosition = transform.position;
        //    }
        //}
    }

    public virtual void OnUngripped(ClickedEventArgs e)
    {
        if (Ungripped != null)
            Ungripped(this, e);

        //if (this.name.Equals("Controller (right)"))
        //{
            
        //}

    }

    //void TouchNearestObject()
    //{

    //    GameObject nearestObject = mrs.CalculateNearestTouchPoint(this.transform);

    //    if (nearestObject != null)
    //    {
    //        VRTK_InteractTouch IT = this.transform.GetChild(1).GetComponent<VRTK_InteractTouch>();
    //        IT.ForceTouch(nearestObject);
    //    }
    //}

    

    private void SMRotation() {

        Vector3 cameraForward = Camera.main.transform.localPosition + Camera.main.transform.forward * 2f;
        cameraForward = new Vector3(cameraForward.x, ExperimentManager.userHeight, cameraForward.z);

        Vector3 newLeftControllerPosition = transform.position;
        float angle = Vector3.SignedAngle((oldLeftControllerPosition - cameraForward), (newLeftControllerPosition - cameraForward), Vector3.up);

        if (Mathf.Abs(angle) > 0.2f)
        {
            for (int i = 1; i <= smallMultipleNumbers; i++)
            {
                GameObject building = GameObject.Find("Small Multiples " + i);

                int dataSet = mrs.dataset;
                Vector3 realCentre;

                if (dataSet == 1)
                {
                    //building.transform.RotateAround(Vector3.up, transform.rotation.y * Time.deltaTime);
                    BuildingScript buildS = building.transform.GetChild(1).gameObject.GetComponent<BuildingScript>();
                    realCentre = buildS.getCentreCoordinates();
                }
                else
                {
                    realCentre = building.transform.position;
                }

                building.transform.RotateAround(realCentre, Vector3.up, angle * 10);
            }
        }
        currentRotation = GameObject.Find("Small Multiples 1").transform.localEulerAngles.y;
        oldLeftControllerPosition = transform.position;

        //// rotate with the controller
        //GameObject swipeTooltip = this.transform.Find("SwipeToolTip").gameObject;
        //GameObject mainCamera = GameObject.Find("Camera (eye)");
        //float facing = 0;

        //facing = mainCamera.transform.rotation.eulerAngles.y;

        ////Debug.Log(facing);
        //RotateGripped = true;

        //float lastZ = z;
        //float lastZBeta = zBeta;

        //z = transform.localPosition.x;
        //zBeta = transform.localPosition.z;
        //swipeTooltip.SetActive(true);

        //float diff = z - lastZ;
        //float diffBeta = zBeta - lastZBeta;
        //for (int i = 1; i <= smallMultipleNumbers; i++)
        //{
        //    GameObject building = GameObject.Find("Small Multiples " + i);

        //    int dataSet = mrs.dataset;
        //    Vector3 realCentre;

        //    if (dataSet == 1)
        //    {
        //        //building.transform.RotateAround(Vector3.up, transform.rotation.y * Time.deltaTime);
        //        BuildingScript buildS = building.transform.GetChild(0).gameObject.GetComponent<BuildingScript>();
        //        realCentre = buildS.getCentreCoordinates();
        //    }
        //    else
        //    {
        //        realCentre = building.transform.position;
        //    }


        //    float finalDiff = 0.0f;
        //    if (Mathf.Abs(diff) > Mathf.Abs(diffBeta))
        //    {
        //        finalDiff = diff;
        //    }
        //    else
        //    {
        //        finalDiff = -diffBeta;
        //    }
        //    if (Mathf.Abs(finalDiff) < 0.001f)
        //    {
        //        finalDiff = 0;
        //    }
        //    if (Mathf.Abs(finalDiff) > 0.002f)
        //    {
        //        if (facing >= 0 && facing < 180)
        //        {
        //            building.transform.RotateAround(realCentre, building.transform.up, -finalDiff * 1000);
        //            if (finalDiff > 0)
        //            {
        //                antiClockwiseRotation = true;
        //                clockwiseRotation = false;
        //            }
        //            else if (finalDiff < 0)
        //            {
        //                clockwiseRotation = true;
        //                antiClockwiseRotation = false;
        //            }

        //        }
        //        else
        //        {
        //            building.transform.RotateAround(realCentre, building.transform.up, finalDiff * 1000);
        //            if (finalDiff > 0)
        //            {
        //                clockwiseRotation = true;
        //                antiClockwiseRotation = false;
        //            }
        //            else if (finalDiff < 0)
        //            {
        //                antiClockwiseRotation = true;
        //                clockwiseRotation = false;
        //            }
        //        }
        //    }

        //    oldFacingValue = facing;
        //    currentRotation = GameObject.Find("Small Multiples 1").transform.localEulerAngles.y;
        //}

    }


    // Update is called once per frame
    protected virtual void Update()
    {

        if (triggerPressed) {
            triggerTimer++;
        }
        //Debug.Log(triggerTimer);

        //if (triggerTimer > 5 && !flag) {
        //    CollisionDetection cd = transform.GetChild(0).Find("tip").GetChild(0).GetChild(0).GetChild(0).GetComponent<CollisionDetection>();

        //    GameObject touchedFilter = cd.draggedFilter;
        //    GameObject toucheAxis = cd.touchedAxis;

        //    if (touchedFilter == null)
        //    {
        //        cd.controllerStartPressingPosition = transform.GetChild(0).Find("tip").GetChild(0).GetChild(0).GetChild(0).position;
        //        cd.currentCube = cd.CreateCube();
        //        flag = true;
        //        mrs.creatingCube = true;
        //    }
        //}
        //else
        //{
        //    mrs.creatingCube = false;
        //}


        // left controller rotation
        //if (this.name.Equals("Controller (left)"))
        //{
        //    if (gripped && bothGripFlag)
        //    {
        //        canRotate = false;
        //    }
        //    else if (gripped && !bothGripFlag)
        //    {
        //        canRotate = true;
        //    }

        //    if (gripped && canRotate)
        //    {
        //        SMRotation();
        //    }

        //    if (!gripped)
        //    {
        //        for (int i = 1; i <= smallMultipleNumbers; i++)
        //        {
        //            GameObject building = GameObject.Find("Small Multiples " + i);
        //            if (mrs.fixedPositionCurved)
        //            {
        //                //if (mrs.curveCenterPoint != Vector3.zero) {
        //                //    Vector3 finalPos = mrs.curveCenterPoint;
        //                //    Vector3 offset = building.transform.position - finalPos;
        //                //    building.transform.LookAt(building.transform.position + offset);
        //                //}
        //            }
        //            else {
        //                building.transform.localEulerAngles = new Vector3(0, currentRotation, 0);
        //            }

        //        }
        //        GameObject swipeTooltip = this.transform.Find("SwipeToolTip").gameObject;
        //        swipeTooltip.SetActive(false);

        //        RotateGripped = false;
        //        antiClockwiseRotation = false;
        //        clockwiseRotation = false;
        //    }

        //}
        //else
        //if (this.name.Equals("Controller (right)"))
        //{
        //    if (gripped && bothGripFlag)
        //    {
        //        canMove = false;
        //    }
        //    else if (gripped && !bothGripFlag)
        //    {
        //        canMove = true;
        //    }


        //    if (gripped && canMove) {
        //        ShelfMovement();
        //    }
        //}


        //this.uniqueCenterZDelta = mrs.uniqueCenterZDelta;

        // indirect touch find nearest point
        //if (!mrs.hidePillarsAndBoards)
        //{
        //    if (mrs.indirectTouch)
        //    {
        //        TouchNearestObject();
        //        indirectTouched = true;
        //    }
        //    else
        //    {
        //        if (indirectTouched)
        //        {
        //            VRTK_InteractTouch IT = this.transform.GetChild(1).GetComponent<VRTK_InteractTouch>();
        //            IT.ForceStopTouching();
        //            indirectTouched = false;
        //        }

        //    }
        //}

        //// reset facing
        //if (padPressed && !mrs.hidePillarsAndBoards)
        //{
        //    if (this.name.Equals("Controller (right)"))
        //    {
        //        mrs.faceToCurve = true;
        //        mrs.ToggleFaceCurve();
        //    }
        //}

        var system = OpenVR.System;
        if (system != null && system.GetControllerState(controllerIndex, ref controllerState, (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(VRControllerState_t))))
        {
            ulong trigger = controllerState.ulButtonPressed & (1UL << ((int)EVRButtonId.k_EButton_SteamVR_Trigger));
            if (trigger > 0L && !triggerPressed)
            {
                triggerPressed = true;
                ClickedEventArgs e;
                e.controllerIndex = controllerIndex;
                e.flags = (uint)controllerState.ulButtonPressed;
                e.padX = controllerState.rAxis0.x;
                e.padY = controllerState.rAxis0.y;
                OnTriggerClicked(e);

            }
            else if (trigger == 0L && triggerPressed)
            {
                triggerPressed = false;
                ClickedEventArgs e;
                e.controllerIndex = controllerIndex;
                e.flags = (uint)controllerState.ulButtonPressed;
                e.padX = controllerState.rAxis0.x;
                e.padY = controllerState.rAxis0.y;
                OnTriggerUnclicked(e);
            }

            ulong grip = controllerState.ulButtonPressed & (1UL << ((int)EVRButtonId.k_EButton_Grip));
            if (grip > 0L && !gripped)
            {
                gripped = true;
                ClickedEventArgs e;
                e.controllerIndex = controllerIndex;
                e.flags = (uint)controllerState.ulButtonPressed;
                e.padX = controllerState.rAxis0.x;
                e.padY = controllerState.rAxis0.y;
                OnGripped(e);

            }
            else if (grip == 0L && gripped)
            {
                gripped = false;
                ClickedEventArgs e;
                e.controllerIndex = controllerIndex;
                e.flags = (uint)controllerState.ulButtonPressed;
                e.padX = controllerState.rAxis0.x;
                e.padY = controllerState.rAxis0.y;
                OnUngripped(e);
            }

            ulong pad = controllerState.ulButtonPressed & (1UL << ((int)EVRButtonId.k_EButton_SteamVR_Touchpad));
            if (pad > 0L && !padPressed)
            {
                padPressed = true;
                ClickedEventArgs e;
                e.controllerIndex = controllerIndex;
                e.flags = (uint)controllerState.ulButtonPressed;
                e.padX = controllerState.rAxis0.x;
                e.padY = controllerState.rAxis0.y;
                OnPadClicked(e);
            }
            else if (pad == 0L && padPressed)
            {
                padPressed = false;
                ClickedEventArgs e;
                e.controllerIndex = controllerIndex;
                e.flags = (uint)controllerState.ulButtonPressed;
                e.padX = controllerState.rAxis0.x;
                e.padY = controllerState.rAxis0.y;
                OnPadUnclicked(e);
            }

            ulong menu = controllerState.ulButtonPressed & (1UL << ((int)EVRButtonId.k_EButton_ApplicationMenu));
            if (menu > 0L && !menuPressed)
            {
                menuPressed = true;
                ClickedEventArgs e;
                e.controllerIndex = controllerIndex;
                e.flags = (uint)controllerState.ulButtonPressed;
                e.padX = controllerState.rAxis0.x;
                e.padY = controllerState.rAxis0.y;
                OnMenuClicked(e);
            }
            else if (menu == 0L && menuPressed)
            {
                menuPressed = false;
                ClickedEventArgs e;
                e.controllerIndex = controllerIndex;
                e.flags = (uint)controllerState.ulButtonPressed;
                e.padX = controllerState.rAxis0.x;
                e.padY = controllerState.rAxis0.y;
                OnMenuUnclicked(e);
            }

            pad = controllerState.ulButtonTouched & (1UL << ((int)EVRButtonId.k_EButton_SteamVR_Touchpad));
            if (pad > 0L && !padTouched)
            {
                padTouched = true;
                ClickedEventArgs e;
                e.controllerIndex = controllerIndex;
                e.flags = (uint)controllerState.ulButtonPressed;
                e.padX = controllerState.rAxis0.x;
                e.padY = controllerState.rAxis0.y;
                OnPadTouched(e);

            }
            else if (pad == 0L && padTouched)
            {
                padTouched = false;
                ClickedEventArgs e;
                e.controllerIndex = controllerIndex;
                e.flags = (uint)controllerState.ulButtonPressed;
                e.padX = controllerState.rAxis0.x;
                e.padY = controllerState.rAxis0.y;
                OnPadUntouched(e);
            }
        }
    }
}
