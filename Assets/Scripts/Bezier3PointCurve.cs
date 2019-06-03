//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using VRTK;


//public class Bezier3PointCurve : MonoBehaviour {

//    public bool canUpdate = true;

//	public Transform point1;
//	public Transform point2;
//	public Transform point3;
//	public LineRenderer lineRenderer;
//	public int vertexCount = 50;
//    public float speed = 3f;
//    public bool angleDivision = false;
//    public bool distanceDivision = true;
//    public float diffDelta = 0.2f;
//	// circle implementation
//	public float centerZDelta = -300f;

//    float centerZDeltaDiff = 0.1f;
//    //bool curveOutOfPointFlag = false;

//    public Vector3 centerPoint;

//    List<GameObject> smallMultiples;
//	List<GameObject> currentLvlSmallMultiples;
//    int[] currentLvlSMIndex;
//    List<GameObject> tangentPerpendiculars;

//    List<Vector3> pointList;
//    Vector3[] pointListCopy;
//    List<GameObject> boardPieces;


//    bool topRow = false;

//    //bool beta;

//    //bool refreshed = false;

//    SmallMultiplesManagerScript smms;
//    int smCount;
//    //GameObject shelf;
//    float delta;
//    //bool faceToCurve;
//    //bool realFocusPoint;
//    bool needToFace = true;

//    public GameObject emptyParent;

//    bool controlBallGrabbed = false;

//    bool faceFlag = false;

//    //GameObject focus;
//    // Use this for initialization
//    void Start () {
//        smCount = smms.smallMultiplesNumber;
//        delta = 0.7f;
//        //faceToCurve = smms.faceToCurve;
//        //realFocusPoint = smms.realFocusPoint;
//        //shelf = GameObject.Find("Shelf");
//        //beta = smms.beta;

//        smallMultiples = new List<GameObject>();

//        pointList = new List<Vector3>();
//        pointListCopy = new Vector3[vertexCount + 1];

//        for (int i = 0; i < smCount; i++)
//        {
//            smallMultiples.Add(GameObject.Find("Small Multiples " + (i + 1)));
//        }



//    }

//    private void Awake()
//    {
//        //smCount = smms.smallMultiplesNumber;
//        delta = 0.7f;
//        //faceToCurve = smms.faceToCurve;
//        //realFocusPoint = smms.realFocusPoint;
//        //shelf = GameObject.Find("Shelf");
//        //beta = smms.beta;

//        //smallMultiples = new List<GameObject>();

//        //pointList = new List<Vector3>();
//        //pointListCopy = new Vector3[vertexCount + 1];

//        //for (int i = 0; i < smCount; i++)
//        //{
//        //    smallMultiples.Add(GameObject.Find("Small Multiples " + (i + 1)));
//        //}

//        tangentPerpendiculars = new List<GameObject>();
//        currentLvlSmallMultiples = new List<GameObject>();
//        pointList = new List<Vector3>();
//        boardPieces = new List<GameObject>();
//        pointListCopy = new Vector3[vertexCount + 1];
//        smms = GameObject.Find("SmallMultiplesManager").GetComponent<SmallMultiplesManagerScript>();

//        //focus = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//        //focus.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
//        //focus.name = "focus point";
//        //focus.transform.parent = this.transform;
//        //focus.SetActive(false);
//        faceFlag = false;
//    }

//    public void SetCenterZDelta(float centerZDelta) {
//        this.centerZDelta = centerZDelta;
//    }

//    public Vector3 GetCurrentMiddlePoint() {
//        return pointList[pointList.Count / 2];
//    }


//    // Update is called once per frame
//    void Update()
//    {
//        if (canUpdate) {

//            delta = smms.delta;
//            //Debug.Log(centerZDelta);
//            // reset the curve object local position
//            //transform.localPosition = Vector3.zero;
//            CheckTopRow();
//            GetCurrentRowSM(); // update current level small multiples
//            GetPointList(); //update point list
//            CalculatePointIndex(); // calculate index for current level small multiples
//            CheckFaceCurveAndGrabbed();
//            UpdateManagerUniqueCenterZDelta();

//            tangentPerpendiculars.Clear(); //clear the tp list
//        }
        

//        //Debug.Log(GameObject.Find("Small Multiples 1").transform.localEulerAngles);


//        //if (boardPieces != null && boardPieces.Count != 0 && topRow) {
//        //    GameObject leftPillar = GameObject.Find("Left Pillar");
//        //    GameObject rightPillar = GameObject.Find("Right Pillar");

//        //    if (leftPillar != null && rightPillar != null) {
//        //        leftPillar.transform.position = boardPieces[0].transform.GetChild(0).position;
//        //        rightPillar.transform.position = boardPieces[boardPieces.Count - 1].transform.GetChild(1).position;
//        //    }
//        //}

//        //test only
//        //for (int i = 0; i < currentLvlSmallMultiples.Count; i++)
//        //{
//        //    GameObject tp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//        //    tp.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
//        //    tp.name = currentLvlSmallMultiples[i].name + " tangentPerpendicular";
//        //    tp.transform.parent = shelf.transform;
//        //    tp.SetActive(false);
//        //    tangentPerpendiculars.Add(tp);
//        //}

//        //for (int i = 0; i < currentLvlSmallMultiples.Count; i++)
//        //{
//        //    if (pointListCopy != null)
//        //    {
//        //        // set local position for each game object
//        //        tangentPerpendiculars[i].transform.localPosition = currentLvlSmallMultiples[i].transform.localPosition + new Vector3(GetDirection(currentLvlSMIndex[i] / (float)vertexCount).z, GetDirection(currentLvlSMIndex[i] / (float)vertexCount).y, -GetDirection(currentLvlSMIndex[i] / (float)vertexCount).x);
//        //        // face to the tangent perpendicular object
//        //        tangentPerpendiculars[i].transform.position = tangentPerpendiculars[i].transform.position;

//        //        currentLvlSmallMultiples[i].transform.GetChild(0).gameObject.SetActive(false);
//        //        currentLvlSmallMultiples[i].transform.GetChild(1).gameObject.SetActive(false);
//        //        Debug.DrawRay((currentLvlSmallMultiples[i].transform.localPosition - GetDirection(currentLvlSMIndex[i] / (float)vertexCount) * 100f), GetDirection(currentLvlSMIndex[i] / (float)vertexCount) * 200f, Color.red);

//        //        Debug.DrawRay((currentLvlSmallMultiples[i].transform.localPosition - (tangentPerpendiculars[i].transform.localPosition - currentLvlSmallMultiples[i].transform.localPosition) * 100f), (tangentPerpendiculars[i].transform.localPosition - currentLvlSmallMultiples[i].transform.localPosition) * 200f, Color.green);
//        //    }
//        //}

//        //foreach (GameObject dp in tangentPerpendiculars)
//        //{
//        //    Destroy(dp); // destroy reference game objects
//        //}

//    }

//    void UpdateManagerUniqueCenterZDelta() {
//        if (smms != null) {
//            //if (topRow) {
//                smms.SetUniqueCenterZDelta(centerZDelta);
//            //}
//        }
//    }

//    void CheckFaceCurveAndGrabbed() {
//        if (centerZDelta <= -300)
//        {
//            smms.CanPull(false);
//        }
//        else
//        {
//            smms.CanPull(true);
//        }
//        if (centerZDelta > 0.01f)
//        {
//            smms.CanPush(false);
//        }
//        else
//        {
//            smms.CanPush(true);
//        }


//        GameObject smm = GameObject.Find("SmallMultiplesManager");
//        GameObject controlBall = smm.transform.Find("Control Ball").gameObject;
//        if (controlBall != null)
//        {
//            VRTK_InteractableObject io = controlBall.GetComponent<VRTK_InteractableObject>();
//            if (io.IsGrabbed())
//            {
//                controlBallGrabbed = true;

//                if (boardPieces != null && pointList != null)
//                {
//                    for (int i = 0; i < boardPieces.Count; i++)
//                    {
//                        boardPieces[i].transform.position = pointList[currentLvlSMIndex[i]];
//                    }
//                }

//                if (currentLvlSmallMultiples.Count > 0)
//                {
//                    for (int i = 0; i < currentLvlSmallMultiples.Count; i++)
//                    {
//                        currentLvlSmallMultiples[i].transform.position = pointList[currentLvlSMIndex[i]];
//                    }
//                }
//            }
//            else
//            {
//                if (controlBallGrabbed)
//                {
//                    MoveSmallMultiples(true);
//                    if (boardPieces != null && pointList != null)
//                    {
//                        //Debug.Log(boardPieces.Count + " " + pointList.Count + " " + currentLvlSMIndex.Length);
//                        for (int i = 0; i < boardPieces.Count; i++)
//                        {
//                            boardPieces[i].transform.position = pointList[currentLvlSMIndex[i]];
//                        }
//                    }
//                }
//                else
//                {
//                    //if (smms.dataset == 1 || smms.dataset == 2)
//                    //{
//                    //    MoveSmallMultiples(true);
//                    //    if (faceFlag == false)
//                    //    {
//                    //        FaceToCurve();
//                    //        faceFlag = true;
//                    //    }
//                    //}
//                    //else
//                    //{
//                        MoveSmallMultiples(false);
//                    //}
                    
//                }
//                controlBallGrabbed = false;
//            }
//        }

//        if (pointList.Count > 0)
//        {
//            bool moveFinished = true;
//            //Debug.Log(currentLvlSmallMultiples.Count + " " + currentLvlSMIndex.Length + " " + pointList.Count);
//            for (int i = 0; i < currentLvlSmallMultiples.Count; i++)
//            {
//                if (currentLvlSMIndex.Length >= currentLvlSmallMultiples.Count) {
//                    if (Vector3.Distance(currentLvlSmallMultiples[i].transform.position, pointList[currentLvlSMIndex[i]]) > 0.01f)
//                    {
//                        moveFinished = false;
//                        needToFace = true;
//                    }
//                }   
                
//            }

//            if (moveFinished)
//            {
//                if (needToFace)
//                {
//                    FaceToCurve();
//                    needToFace = false;
//                }
//            }
//        }
        
//    }


//    void CheckTopRow (){
//		GameObject BuildingOne = GameObject.Find("Small Multiples 1");
//		float topY = BuildingOne.transform.position.y;

//		if (Mathf.Abs(this.transform.position.y - topY) < 0.01)
//		{
//			topRow = true;
//		}
//		else
//		{
//			topRow = false;
//		}
//	}

//    /// <summary>
//    /// calculate index for current level small multiples
//    /// use same angle division
//    /// </summary>
//    void CalculatePointIndex() {
//        if (distanceDivision) {
//            if (currentLvlSmallMultiples.Count >= 1)
//            {
//                getSMIndexByLocation();
//                currentLvlSMIndex[0] = 0;
//                currentLvlSMIndex[currentLvlSMIndex.Length - 1] = pointList.Count - 1;//

//                bool sameDistance = false;

//                while (!sameDistance)
//                {
//					float[] distance = new float[currentLvlSMIndex.Length - 1];

//					for (int i = 0; i < currentLvlSMIndex.Length - 1; i++)
//                    {
//                        distance[i] = Vector3.Distance(pointList[currentLvlSMIndex[i + 1]], pointList[currentLvlSMIndex[i]]);
//                    }

//                    // the Babylonian method
//					float[] ratio = new float[currentLvlSMIndex.Length - 1];
//                    if(ratio.Length > 0)
//                    {
//                        ratio[0] = 0;
//                    }
                    
//					for (int j = 1; j < currentLvlSMIndex.Length - 1; j++)
//                    {
//                        ratio[j] = (0.5f) * (distance[j] - distance[j - 1]) / (distance[j] + distance[j - 1]);
//                        if (ratio[j] > 0)
//                        {
//                            currentLvlSMIndex[j] = Mathf.RoundToInt(currentLvlSMIndex[j] + ratio[j] * (currentLvlSMIndex[j + 1] - currentLvlSMIndex[j]));
//                        }
//                        else if (ratio[j] < 0)
//                        {
//                            currentLvlSMIndex[j] = Mathf.RoundToInt(currentLvlSMIndex[j] + ratio[j] * (currentLvlSMIndex[j] - currentLvlSMIndex[j - 1]));
//                        }
//                        //Debug.Log(j + " " + ratio[j]);
//                    }

//					for (int i = 0; i < currentLvlSMIndex.Length - 1; i++)
//                    {
//                        distance[i] = Vector3.Distance(pointList[currentLvlSMIndex[i + 1]], pointList[currentLvlSMIndex[i]]);
//                    }
//                    if (distance.Length > 0)
//                    {
//                        float value = distance[0];

//                        bool sameFlag = true;
//                        for (int i = 1; i < distance.Length - 1; i++)
//                        {
//                            //if (beta)
//                            //{
//                            if (smms.dataset != 3)
//                            {
//                                diffDelta = 0.05f;
//                            }
//                            else {
//                                diffDelta = 0.1f;
//                            }
                           
//                            //}
//                            //else {
//                            //    diffDelta = 0.6f;
//                            //}
//                            if (Mathf.Abs(distance[i] - value) > diffDelta)
//                            {
//                                sameFlag = false;
//                            }
//                        }
//                        if (sameFlag)
//                        {
//                            sameDistance = true;
//                        }
//                    }
//                    else {
//                        //Debug.Log(distance.Length);
//                        sameDistance = true;
//                    }
//                }
//            }
//            //else if (currentLvlSmallMultiples.Count == 1)
//            //{
//            //    currentLvlSMIndex[0] = 0;
//            //}
//            //////////////// custom curve board
            
//        }
//        else if (angleDivision)
//        {
//            int currentLvlSMNo = currentLvlSmallMultiples.Count;
//            float angleDivision = 180f / (currentLvlSMNo - 1);

//            float tangentAngle = Mathf.Tan(angleDivision);
//            Vector3 localCentrePoint = new Vector3(point2.localPosition.x, point2.localPosition.y, point1.localPosition.z);
//            Vector3 globalCentrePoint = Vector3.Lerp(point1.transform.position, point3.transform.position, 0.5f);


//            if (point2.localPosition.z < 0.01f)
//            {
//                getSMIndexByLocation();
//            }
//            else {
//                currentLvlSMIndex[0] = 0;

//                for (int j = 1; j < currentLvlSMIndex.Length; j++) {
//                    float diff = 10f;
//                    for (int i = 0; i < pointList.Count; i++)
//                    {
//                        if (Mathf.Abs(Vector3.Angle(localCentrePoint - point1.localPosition, globalCentrePoint - pointList[i]) - j * angleDivision) < diff)
//                        {
//                            currentLvlSMIndex[j] = i;
//                            diff = Vector3.Angle(localCentrePoint - point1.localPosition, globalCentrePoint - pointList[i]) - j * angleDivision;
//                        }
//                    }
//                }
//            }
//        }
//        else {
//            getSMIndexByLocation();
//        }
//    }

//    void RefreshBoardPieces() {

//        foreach (GameObject go in boardPieces)
//        {
//            Destroy(go);
//        }
//        GameObject tmp = emptyParent;
//        Destroy(tmp);
//        boardPieces = new List<GameObject>();
//        emptyParent = new GameObject();
//        emptyParent.transform.SetParent(this.transform.parent.parent);
//        emptyParent.transform.position = this.transform.parent.position;
//        emptyParent.transform.localRotation = Quaternion.identity;
//        emptyParent.transform.localScale = Vector3.one;
//        emptyParent.name = this.transform.parent.name + " Pieces";

//        if (boardPieces.Count == 0 || boardPieces.Count != currentLvlSMIndex.Length)
//        {
//            for (int h = 0; h < currentLvlSMIndex.Length; h++)
//            {
//                GameObject shelfBoardPiecePrefab = smms.shelfBoardPiecePrefab;
//                GameObject board = (GameObject)Instantiate(shelfBoardPiecePrefab, new Vector3(0, 0, 0), Quaternion.identity);
//                board.transform.SetParent(emptyParent.transform);
//                Vector3 parentScale = this.transform.parent.localScale;
//                board.transform.localScale = new Vector3(delta * 1.05f, 0.003f, delta * 1.05f);
//                if (!float.IsNaN(pointList[currentLvlSMIndex[h]].x)) {
//                    board.transform.position = pointList[currentLvlSMIndex[h]];
//                }
                
//                board.name = (h + 1) + " board";
//                boardPieces.Add(board);
//            }
//            this.transform.parent.GetComponent<MeshRenderer>().enabled = false;
//        }
//        if (boardPieces.Count == 1)
//        {
//            boardPieces[0].transform.localRotation = Quaternion.identity;
//        }
//        else {
//            for (int i = 0; i < boardPieces.Count; i++)
//            {
//                if (!float.IsNaN(pointList[currentLvlSMIndex[i]].x)) {
//                    boardPieces[i].transform.position = pointList[currentLvlSMIndex[i]];
//                    Vector3 finalPos = centerPoint;
//                    Vector3 offset = boardPieces[i].transform.position - finalPos;
//                    boardPieces[i].transform.LookAt(boardPieces[i].transform.position + offset);
//                }
                
//            }
//        }
        
  
//        //refreshed = true;
//    }

//    void getSMIndexByLocation() {

//        if (currentLvlSmallMultiples.Count >= 1)
//        {
//            //for (int i = 0; i < currentLvlSmallMultiples.Count; i++)
//			for (int i = 0; i < currentLvlSMIndex.Length; i++) // changed
//            {
//                int currentPointIndex;
//				if (topRow)
//                {
//                    if (currentLvlSMIndex.Length == 1)
//                    {
//                        currentPointIndex = 0;
//                    }
//                    else {
//                        currentPointIndex = (int)pointList.Count / (currentLvlSMIndex.Length - 1) * i;
//                    }
                    
//                }
//                else
//                {
//                    if ((int)(this.transform.parent.localScale.x / delta) > 1)
//                    {
//                        currentPointIndex = (pointList.Count / ((int)(this.transform.parent.localScale.x / delta) - 1) * i);
//                    }
//                    else {
//                        currentPointIndex = 0;
//                    }
                    
//                }

//                if (currentPointIndex > (pointList.Count - 1))
//                {
//                    currentLvlSMIndex[i] = pointList.Count - 1;
//                }
//                else
//                {
//                    currentLvlSMIndex[i] = currentPointIndex;
//                }
//            }
//        }
//        //else if (currentLvlSmallMultiples.Count == 1)
//        //{
//        //    currentLvlSMIndex[0] = 0;
//        //}
//    }

//    /// <summary>
//    /// function for move small multiples positioni with animation
//    /// </summary>
//    void MoveSmallMultiples(bool released) {
//        for (int i = 0; i < currentLvlSMIndex.Length; i++)
//        {
//            if (released)
//            {
//                currentLvlSmallMultiples[i].transform.position = pointList[currentLvlSMIndex[i]];
//            }
//            else {
//                if (!float.IsNaN(pointList[currentLvlSMIndex[i]].x)) {

//                    if (smms.dataset == 1 || smms.dataset == 2) {
//                        speed = 8;
//                    }
//                    currentLvlSmallMultiples[i].transform.position = Vector3.Lerp(currentLvlSmallMultiples[i].transform.position, pointList[currentLvlSMIndex[i]], Time.deltaTime * speed);
//                }
                
//            }
//            //currentLvlSmallMultiples[i].transform.position = Vector3.MoveTowards(currentLvlSmallMultiples[i].transform.position, pointList[currentLvlSMIndex[i]], Time.deltaTime * speed);

//            if (smms.dataset == 3) {
//                currentLvlSmallMultiples[i].transform.eulerAngles = new Vector3(0, currentLvlSmallMultiples[i].transform.eulerAngles.y, 0);
//            }
            
//        }
//    }

//	public void ShelfPushed(){
//        if (smms.fixedPositionCurved && !smms.quarterCurved)
//        {
//            centerZDelta = -0.1f;
//        }
//        else if (smms.fixedPositionCurved && smms.quarterCurved) {
//            centerZDelta = -1.05f;
//        }
//        else
//        {
//            //Debug.Log(centerZDelta);
//            if (centerZDelta <= -12 && centerZDelta >= -80)
//            {
//                centerZDelta += centerZDeltaDiff * 20;
//            }
//            else
//            {
//                if (centerZDelta < -0.1f)
//                {
//                    centerZDelta += centerZDeltaDiff;
//                }
//                else if (centerZDelta == -0.1f)
//                {
//                    centerZDelta = 0.01f;
//                }
//                if (centerZDelta < -80 && centerZDelta >= -300)
//                {
//                    centerZDelta += centerZDeltaDiff * 200;
//                }
//            }
//        }
//        UpdateManagerUniqueCenterZDelta();

//    }

//	public void ShelfPulled(){
//        if (smms.fixedPositionCurved)
//        {
//            centerZDelta = -300;
//        }
//        //Debug.Log(centerZDelta);
//        if (centerZDelta == 0.01f) {
//            centerZDelta = -0.1f;
//        }
//        if (centerZDelta <= -10 && centerZDelta > -80)
//        {
//            centerZDelta -= centerZDeltaDiff * 20;
//        }
//        else if (centerZDelta <= -80 && centerZDelta >= -280) {
//            centerZDelta -= centerZDeltaDiff * 200;
//        }
//        else
//        {
//            if (centerZDelta < -280 && centerZDelta > -300) {
//                centerZDelta = -300;
//            }
//            //if (centerZDelta <= -100 && centerZDelta > -200)
//            //{
//            //    centerZDelta -= centerZDeltaDiff * 40;
//            //}
//            //else {
//            //if (centerZDelta > -10)
//            //{
//                centerZDelta -= centerZDeltaDiff;
//           //}

//            //}

//        }
//        UpdateManagerUniqueCenterZDelta();

//    }

//    public void BoardPieceToNormal() {
//        foreach (GameObject go in boardPieces) {
//            go.transform.localRotation = Quaternion.identity;
//        }
//    }

//    /// <summary>
//    /// Get the total points list of bezier curve 
//    /// </summary>
//    void GetPointList() {
//        pointList.Clear(); // refresh point list

//        //float dividence = this.transform.parent.localScale.z * this.transform.parent.parent.parent.localScale.z;
//        //////////////
//        //if (beta)
//        //{
//        //Debug.Log(centerZDelta);
//        Vector3 parentScale = this.transform.parent.localScale * this.transform.parent.parent.parent.localScale.z * this.transform.parent.parent.localScale.z;
        
//            float scaleZ = 1 / parentScale.z;
//            float scaledZ = -(this.transform.parent.localScale.z - delta) * scaleZ / 2;

//            float localCenterZ = centerZDelta / parentScale.z;
//        //Debug.Log(centerZDelta + " " + localCenterZ);
//            Vector3 point1V = new Vector3(-0.5f + delta/2/parentScale.x , 0, scaledZ);
//            Vector3 point3V = new Vector3(0.5f - delta /2/ parentScale.x, 0, scaledZ);

//            Vector3 globalCenterPoint = transform.TransformPoint(new Vector3(0,0,localCenterZ));
//            centerPoint = globalCenterPoint;
//            //Debug.Log(point1V);
//            Vector3 globalfakePoint1 = transform.TransformPoint(point1V);
//            Vector3 globalfakePoint3 = transform.TransformPoint(point3V);
            
//            float arcLength = Vector3.Angle(globalCenterPoint - globalfakePoint1, globalCenterPoint - globalfakePoint3) * Mathf.Deg2Rad * Vector3.Distance(globalCenterPoint, globalfakePoint1);
//            //Debug.Log("before " + arcLength + " " + (currentLvlSMIndex.Length - 1) * delta  + " " + centerZDelta);
//            Vector3 oldPoint1V = point1V;

//            while (Mathf.Abs(arcLength - (currentLvlSMIndex.Length - 1) * delta) > 0.2f)
//            {
//                point1V += Vector3.right * 0.01f;
                
//                if (point1V.x > -0.4f)
//                {

//                    break;
//                }
//                globalfakePoint1 = transform.TransformPoint(point1V);

//                point3V -= Vector3.right * 0.01f;
//                globalfakePoint3 = transform.TransformPoint(point3V);

//                arcLength = Vector3.Angle(globalCenterPoint - globalfakePoint1, globalCenterPoint - globalfakePoint3) * Mathf.Deg2Rad * Vector3.Distance(globalCenterPoint, globalfakePoint1);
                
//                oldPoint1V = point1V;
//            }


//            float radius = Vector3.Distance(globalfakePoint1, globalCenterPoint);
        
//        float localRadiusZ = radius / parentScale.z;
//            //Debug.Log(localCenterZ + " " + localRadiusZ);
//            //Debug.Log(transform.TransformPoint(new Vector3(0,0,1)));
//            //Debug.Log("after " + arcLength + " " + (currentLvlSMIndex.Length - 1) * delta + " " + radius + " " + centerZDelta);

//            point1.localPosition = transform.InverseTransformPoint(globalfakePoint1);
//        //Debug.Log("globalfakePoint1: " + globalfakePoint1 + " globalCenterPoint: " + globalCenterPoint);
//        point2.localPosition = new Vector3(0, 0, localRadiusZ + localCenterZ);
//            point3.localPosition = transform.InverseTransformPoint(globalfakePoint3);
//            //Debug.Log(point1.localPosition.x + " " + (localCenterZ + Mathf.Sqrt(localRadiusZ * localRadiusZ - point1.localPosition.x * point1.localPosition.x)));
//            //Debug.Log((point2.localPosition.z - point1.localPosition.z) * parentScale.z);
//            for (float ratio = 0; ratio <= 1; ratio += 1.0f / vertexCount)
//            {
//                //float x = point1.position.x + 2 * Mathf.Abs(point1.localPosition.x) * ratio;
//                float x = point1.localPosition.x + 2 * Mathf.Abs(point1.localPosition.x) * ratio;
//                float globalZ = centerZDelta + Mathf.Sqrt(radius * radius - x * parentScale.x * x * parentScale.x);
//                //float z = localCenterZ + Mathf.Sqrt(localRadiusZ * localRadiusZ - x * x);
//                float z = globalZ / parentScale.z;
//                //if (ratio == 0) {
//                //    //Debug.Log(z - point1.localPosition.z);
//                //    if (z - point1.localPosition.z > 0.1f)
//                //    {
//                //        curveOutOfPointFlag = true;
//                //    }
//                //    else {
//                //        curveOutOfPointFlag = false;
//                //    }
//                //}
//                Vector3 point = transform.TransformPoint(new Vector3(x, 0, z));
//                pointList.Add(point);
//            }


//            // copy point list for longer existance
//            for (int k = 0; k < pointList.Count; k++)
//            {
//                pointListCopy[k] = pointList[k];
//            }
//            // create line for the points as bezier curve (hidden in the UI)
//            lineRenderer.positionCount = pointList.Count;
//            lineRenderer.SetPositions(pointList.ToArray());
//        //}
//        //else {
//        //    GameObject leftPillar = GameObject.Find("Left Pillar");
//        //    GameObject rightPillar = GameObject.Find("Right Pillar");

//        //    Vector3 currentLeft = leftPillar.transform.GetChild(0).position;
//        //    Vector3 currentRight = rightPillar.transform.GetChild(0).position;

//        //    pointList.Clear(); // refresh point list

//        //    // recalculate point1, point3 position
//        //    if (currentLvlSmallMultiples.Count > 0)
//        //    {
//        //        if (topRow) // top curve has full objects located
//        //        {
//        //            this.name = "Top Curve Renderer";
//        //            GameObject firstB = currentLvlSmallMultiples[0];
//        //            GameObject lastB = currentLvlSmallMultiples[currentLvlSmallMultiples.Count - 1];



//        //            this.point1.localPosition = new Vector3(this.point1.localPosition.x, 0, 0);
//        //            this.point3.localPosition = new Vector3(this.point3.localPosition.x, 0, 0);


//        //            this.point1.position = new Vector3(currentLeft.x, this.point1.position.y, currentLeft.z);
//        //            this.point3.position = new Vector3(currentRight.x, this.point1.position.y, currentRight.z);

//        //        }
//        //        else // ohter rows have slots but need to align with the top row
//        //        {
//        //            this.name = this.transform.parent.name + " Curve Renderer";
//        //            this.point1.localPosition = new Vector3(this.point1.localPosition.x, 0, 0);
//        //            this.point3.localPosition = new Vector3(this.point3.localPosition.x, 0, 0);


//        //            this.point1.position = new Vector3(currentLeft.x, this.point1.position.y, currentLeft.z);
//        //            this.point3.position = new Vector3(currentRight.x, this.point3.position.y, currentRight.z);

//        //        }
//        //    }



//        //    // generate bezier point curve points
//        //    for (float ratio = 0; ratio <= 1; ratio += 1.0f / vertexCount)
//        //    {
//        //        var tangentLineVertex1 = Vector3.Lerp(point1.position, point2.position, ratio);
//        //        var tangentLineVertex2 = Vector3.Lerp(point2.position, point3.position, ratio);
//        //        var bezierPoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);
//        //        pointList.Add(bezierPoint);
//        //    }
//        //    // copy point list for longer existance
//        //    for (int k = 0; k < pointList.Count; k++)
//        //    {
//        //        pointListCopy[k] = pointList[k];
//        //    }

//        //    // create line for the points as bezier curve (hidden in the UI)
//        //    lineRenderer.positionCount = pointList.Count;
//        //    lineRenderer.SetPositions(pointList.ToArray());
//        //}


//    }

//    /// <summary>
//    /// Get small multiples object in current Rows
//    /// </summary>
//    void GetCurrentRowSM() {

//        currentLvlSmallMultiples.Clear();

//        // variables from manager script
//        int rowIndex;
//        int totalRows = smms.GetRows();
//        int itemPerRow = smms.GetItemPerRow();
//        smCount = smms.smallMultiplesNumber;

//        // copy current small multiple list
//        List<GameObject> sm = new List<GameObject>();
//        for (int l = 0; l < smCount; l++)
//        {
//            sm.Add(GameObject.Find("Small Multiples " + (l + 1)));
//        }
        
//        // refer row index
//        if (this.transform.parent.gameObject.name.Equals("Bottom Board"))
//        {
//            rowIndex = 0;
//        }
//        else {
//            string rowNo = this.transform.parent.name.Split(' ')[1];
//            rowIndex = int.Parse(rowNo) - 1;
//        }
        
//        // if in current row then add into current level sm list
//        int i = totalRows - 1;
//        int k = 0;
//        while (i >= 0) {
//            for (int j = 0; j < itemPerRow; j++)
//            {
//                if (rowIndex == i) {
//                    currentLvlSmallMultiples.Add(sm[k]);
//                }
//                k++;
//                if (k == sm.Count)
//                {
//                    break;
//                }
//            }
//            i--;
//        }
//        // initiate new list for index
//		if (topRow) {
//			currentLvlSMIndex = new int[currentLvlSmallMultiples.Count];
//		} else {
//			if ((int)(this.transform.parent.localScale.x / delta) < 1) {
//				currentLvlSMIndex = new int[1];
//			} else {
//				currentLvlSMIndex = new int[(int)(this.transform.parent.localScale.x / delta) ];
//			}

//		}
        
//    }

//    private void FaceToCurveOnly() {
//        if (smms.GetRows() != smCount)
//        {
//            for (int i = 0; i < currentLvlSmallMultiples.Count; i++)
//            {
//                if (pointListCopy != null)
//                {
//                    Vector3 finalPos = centerPoint;
//                    Vector3 offset = currentLvlSmallMultiples[i].transform.position - finalPos;
//                    currentLvlSmallMultiples[i].transform.LookAt(currentLvlSmallMultiples[i].transform.position + offset);
//                }
//                else
//                {
//                    Debug.Log("BUGBUGBUG");
//                }
//            }
//        }
//        else
//        { // if each row has only one object
//            if (currentLvlSmallMultiples.Count == 1)
//            {
//                currentLvlSmallMultiples[0].transform.localRotation = Quaternion.identity;
//            }
//        }
//    }

//    /// <summary>
//    /// Face to the curve function, can be called in other script
//    /// </summary>
//    public void FaceToCurve() {


//        //this.transform.localPosition = Vector3.zero;
//        GetCurrentRowSM(); // get current level small multiples objects
//        GetPointList(); // get bezier points list
//        CalculatePointIndex(); // calculate index for each small multiples
//        RefreshBoardPieces();
//        //tangentPerpendiculars.Clear(); //clear the tp list

//        //// reference variables
//        //shelf = GameObject.Find("Shelf");

//        //// create tangent perpendicular object
//        //for (int i = 0; i < currentLvlSmallMultiples.Count; i++)
//        //{
//        //    GameObject tp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//        //    tp.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
//        //    tp.name = currentLvlSmallMultiples[i].name + " tangentPerpendicular";
//        //    tp.transform.parent = shelf.transform;
//        //    tp.SetActive(false);
//        //    tangentPerpendiculars.Add(tp);
//        //}


//        //Vector3 focusP = CalculateParabola();
//        //if (focus != null && !float.IsInfinity(focusP.z) && !float.IsNaN(focusP.x))
//        //{
//        //    focus.transform.localPosition = focusP;
//        //}
//        //focus.transform.position = Camera.main.transform.position;


//        // set tangent perpendiculars for small multiples
//        if (smms.GetRows() != smCount)
//        {
//            for (int i = 0; i < currentLvlSmallMultiples.Count; i++)
//            {
//                if (pointListCopy != null)
//                {
//                    //// set local position for each game object
//                    //tangentPerpendiculars[i].transform.localPosition = currentLvlSmallMultiples[i].transform.localPosition + new Vector3(GetDirection(currentLvlSMIndex[i] / (float)vertexCount).z, GetDirection(currentLvlSMIndex[i] / (float)vertexCount).y, -GetDirection(currentLvlSMIndex[i] / (float)vertexCount).x);
//                    //// face to the tangent perpendicular object
//                    //tangentPerpendiculars[i].transform.position = tangentPerpendiculars[i].transform.position;

//                    //Debug.DrawRay (currentLvlSmallMultiples[i].transform.position, tangentPerpendiculars[i].transform.position - currentLvlSmallMultiples[i].transform.position, Color.red, 20);

//                    //Vector3 finalPos = tangentPerpendiculars[i].transform.position;
//                    //Vector3 offset = currentLvlSmallMultiples[i].transform.position - finalPos;
//                    //currentLvlSmallMultiples[i].transform.LookAt(currentLvlSmallMultiples[i].transform.position + offset);

//                    //if (!beta)
//                    //{
//                    //    //if (point2.localPosition.z > 0.001)
//                    //    //{
//                    //    //    Vector3 finalPos = focus.transform.position;
//                    //    //    Vector3 offset = currentLvlSmallMultiples[i].transform.position - finalPos;
//                    //    //    currentLvlSmallMultiples[i].transform.LookAt(currentLvlSmallMultiples[i].transform.position + offset);
//                    //    //}
//                    //    //else
//                    //    //{
//                    //    //    currentLvlSmallMultiples[i].transform.localRotation = Quaternion.identity;
//                    //    //}
//                    //}
//                    //else {
//                    //Debug.Log(centerPoint);
//                    Vector3 finalPos = centerPoint;
//                        Vector3 offset = currentLvlSmallMultiples[i].transform.position - finalPos;
//                        currentLvlSmallMultiples[i].transform.LookAt(currentLvlSmallMultiples[i].transform.position + offset);
//                    //}
//                }
//                else {
//                    Debug.Log("BUGBUGBUG");
//                }
//            }
//        }
//        else { // if each row has only one object
//            if (currentLvlSmallMultiples.Count == 1) {
//                currentLvlSmallMultiples[0].transform.localRotation = Quaternion.identity;
//            }
//        }
//        //foreach (GameObject dp in tangentPerpendiculars)
//        //{
//        //    Destroy(dp); // destroy reference game objects
//        //}
//    }

//    /// <summary>
//    /// Bezier curve function
//    /// </summary>
//    /// <param name="p0"> first point in the bezier curve </param>
//    /// <param name="p1"> second point in the bezier curve </param>
//    /// <param name="p2"> third point in the bezier curve </param>
//    /// <param name="t"> the point index for derivative solution </param>
//    /// <returns></returns>
//    public static Vector3 GetBezierCurveValue(Vector3 p0, Vector3 p1, Vector3 p2, float t)
//    {
//        return
//            (1f - t) * (1f - t) * p0 + 2 * (1f - t) * t * p1 +
//            t * t * p2;
//    }


//    /// <summary>
//    /// Calculate the first derivative for bezier curve based on three points
//    /// </summary>
//    /// <param name="p0"> first point in the bezier curve </param>
//    /// <param name="p1"> second point in the bezier curve </param>
//    /// <param name="p2"> third point in the bezier curve </param>
//    /// <param name="t"> the point index for derivative solution </param>
//    /// <returns></returns>
//    public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t)
//    {
//        return
//            2f * (1f - t) * (p1 - p0) +
//            2f * t * (p2 - p1);
//    }

//    /// <summary>
//    /// Get the current velocity of tangent at a certain point
//    /// </summary>
//    /// <param name="t"> the index for the point </param>
//    /// <returns></returns>
//    public Vector3 GetVelocity(float t)
//    {
        
//        return GetFirstDerivative(point1.localPosition, point2.localPosition, point3.localPosition, t);
//    }

//    /// <summary>
//    /// Get the normalized direction of velocity
//    /// </summary>
//    /// <param name="t"> the index for the point </param>
//    /// <returns></returns>
//    public Vector3 GetDirection(float t)
//    {
//        return GetVelocity(t).normalized;
//    }

//    Vector3 CalculateParabola() {
////        float x1 = point1.localPosition.x;
////        float x2 = point2.localPosition.x;
////        float x3 = point3.localPosition.x;
////        float z1 = point1.localPosition.z;
////        float z2 = point2.localPosition.z;
////        float z3 = point3.localPosition.z;
////        float a = (z1 - (2 * z2) + z3) / ((x3 - x1) * (x3 - x1));
////        float b = 2 * (x3 * (z2 - z1) + x1 * (z2 - z3)) / ((x3 - x1) * (x3 - x1));
////        float c = z1 - a * x1 * x1 - b * x1;
////        float fx = -b / (2 * a);
////        float fz = (4 * a * c - b * b + 1) / (4 * a);
////
////        float deltaZ = fz;
////        if (fz < -1)
////        {
////            deltaZ = fz * 20;
////        }
////        else if(fz >= -1 && fz < 0) {
////            deltaZ = fz * 40;
////        }
////        else {
////            deltaZ = fz * 1f;
////        }
////        //Debug.Log(a + " " + b + " " + c);
////        if (realFocusPoint) {
////            return new Vector3(fx, 0, fz);
////        }
////        return new Vector3(fx, 0, deltaZ);

//        /// new algorithm
//		float x1 =  transform.InverseTransformPoint(pointList[0]).x;
//		float z1 =  transform.InverseTransformPoint (pointList [0]).z;
//		float z2 =  transform.InverseTransformPoint(pointList[pointList.Count / 2]).z;
//		float y2 = z2 - z1;
//		float x3 =  transform.InverseTransformPoint(pointList[pointList.Count - 1]).x;
//		float z3 =  transform.InverseTransformPoint(pointList[pointList.Count - 1]).z;
//		//Debug.Log (transform.InverseTransformPoint(pointList[0]) + " " + transform.InverseTransformPoint(pointList[pointList.Count / 2]) + " " + transform.InverseTransformPoint(pointList[pointList.Count - 1]));


//		float deltaX = this.transform.parent.localScale.x;
//		float deltaZ = this.transform.parent.localScale.z;

//		//x1 = x1 / deltaX;
//		//x3 = x3 / deltaX;
//		//y2 = y2 / deltaZ;

//		float c = y2;
//		float a = y2 / (x1 * x3);
//		float b = -(x1 + x3) * y2 / (x1 * x3);
//		//Debug.Log (a + " " + b + " " + c);
//		float fx = -b / (2 * a);
//		float fz = (4 * a * c - b * b + 1) / (4 * a);

//		fz = fz + z1;
//		//Debug.Log ("fx: " + fx + " fz: " + fz);
//		return new Vector3(fx, 0, fz);
//    }
//}
