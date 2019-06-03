using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;


public class SmallMultiplesManagerScriptTest : MonoBehaviour {

    // prefabs
    public GameObject BuildingPrefab;
	public GameObject shelfBoardPrefab;
    public GameObject FpillarPrefab;
	public GameObject centroidPrefab;
	public GameObject curveRendererPrefab;

    // config variables
    public int smallMultiplesNumber;
    public TextAsset ACFile;

    // sensors Info List to store sensor information
	public static Dictionary<string, HashSet<SensorReading>> sensorsInfoList;

    // Small multiples game object list
	List<GameObject> buildingSM;
	static string layout = "";

    // shelf variables
    GameObject shelf;
    GameObject roofBoard;
	List<GameObject> shelfBoards;
	List<GameObject> shelfPillars;
	List<GameObject> curveRenderers;
	int shelfRows = 0;
	int shelfItemPerRow = 0;
	List<float> shelfRowY;
	float delta = 0.65f;
	Vector3 oldLeftPosition;
	Vector3 oldRightPosition;
	float currentY;

	// curve variables
	float currentCurvature;
	float currentCurveRendererZ;

	GameObject centroidGO;

    float currentZDistance;
    float currentBoardPositionZ;

    // offset variables
    //float offSetZ = 0.267f;
    //float offsetX = 0.71f;
    float curvedOffset = 0.2f;
    float linearCurvedOffsetx = 0.6f;
    float linearCurvedOffsetz = 0.2f;

    // I/O variables
    private char lineSeperater = '\n'; // It defines line seperate character
    private char fieldSeperator = ','; // It defines field seperate chracter


    // Use this for initialization
    void Start () {
		if (buildingSM == null) {
			
			sensorsInfoList = new Dictionary<string, HashSet<SensorReading>> ();
			shelfRowY = new List<float> ();
	        buildingSM = new List<GameObject> ();
			shelfBoards = new List<GameObject> ();
			shelfPillars = new List<GameObject> ();
			curveRenderers = new List<GameObject> ();

            shelf = new GameObject("Shelf");
            shelf.transform.SetParent(this.transform);
            shelf.transform.localPosition = Vector3.zero;

            // read ac file
            ReadACFile();

			CreateShelf ();
            currentZDistance = delta;
            currentBoardPositionZ = 0;

            // create building small multiples
            CreateSM();

			centroidGO = (GameObject)Instantiate(centroidPrefab, new Vector3(0, 0, 0), Quaternion.identity);
			centroidGO.name = "Centroid";
			centroidGO.transform.SetParent(shelf.transform);
			centroidGO.transform.localScale = new Vector3 (0.5f,0.5f,0.5f);
        }
	}

	void Update(){
		UpdatePillar ();
		UpdateBoards ();
        UpdateSM();
		FindCenter ();
	}



    void UpdateSM (){
		GameObject leftPillar = shelfPillars [0];
		float newLeftMostItemX = leftPillar.transform.localPosition.x + (delta / 2);
		float newTopMostItemY = shelfBoards [shelfBoards.Count - 1].transform.localPosition.y;
			
		int i = 0;
		for(int j = 0; j < shelfBoards.Count; j ++){
			int k = 0;
			while (k < shelfItemPerRow) {
				if (i >= smallMultiplesNumber) {
					break;
				}
                buildingSM[i].transform.localPosition = new Vector3(newLeftMostItemX + (k * delta), newTopMostItemY - (j * 0.4f), shelfBoards[0].transform.localPosition.z);
                k++;
				i++;
			}
		}
    }

	void UpdateBoards (){
		GameObject leftPillar = shelfPillars [0];
		GameObject rightPillar = shelfPillars [1];

		float newPositionX = (leftPillar.transform.localPosition.x + rightPillar.transform.localPosition.x) / 2;
		float newScaleX = Mathf.Abs(rightPillar.transform.localPosition.x - leftPillar.transform.localPosition.x);
        float newScaleZ = shelfBoards[0].transform.localScale.z;
		Quaternion newRotation = shelfBoards [0].transform.localRotation;

		float division = newScaleX / delta;
		int newShelfItemPerRow = (int) division;
        if (newShelfItemPerRow != shelfItemPerRow) {
            shelfItemPerRow = newShelfItemPerRow;
        }
		int reminder = smallMultiplesNumber % shelfItemPerRow;
		int newShelfRow;
		if (reminder != 0) {
			newShelfRow = smallMultiplesNumber / shelfItemPerRow + 1;
		} else {
			newShelfRow = smallMultiplesNumber / shelfItemPerRow;
		}
        while (newShelfRow != shelfRows) {
            if (newShelfRow > shelfRows && newShelfRow <= smallMultiplesNumber)
            {
                GameObject board = (GameObject)Instantiate(shelfBoardPrefab, new Vector3(0,0,0), newRotation);
                board.transform.SetParent(shelf.transform);
                board.transform.localScale = new Vector3(newScaleX, 0.003f, newScaleZ);
                board.transform.localRotation = newRotation;
                board.transform.localPosition = new Vector3(newPositionX, shelfBoards[shelfRows - 1].transform.localPosition.y + 0.4f, shelfBoards[shelfRows - 1].transform.localPosition.z);
                board.name = "ShelfRow " + (shelfBoards.Count + 1);
                shelfBoards.Add(board);
                shelfRows++;

				GameObject curveRenderer = (GameObject)Instantiate(curveRendererPrefab, new Vector3(0, 0, 0), Quaternion.identity);
				curveRenderer.transform.SetParent (board.transform);
				curveRenderer.name = "ShelfRow " + shelfBoards.Count + " curve renderer";
				curveRenderer.transform.localPosition = new Vector3(0, 0, 0);
				curveRenderer.transform.localScale = new Vector3 (1,1,1);
				curveRenderers.Add (curveRenderer);

                // increase pillar height
                foreach (GameObject pillar in shelfPillars)
                {
                    pillar.transform.localPosition += Vector3.up * 0.2f;
                    pillar.transform.localScale += Vector3.up * 0.2f;
                }
                currentY += 0.2f;

                // update roof board
                roofBoard.transform.localPosition = new Vector3(newPositionX, roofBoard.transform.localPosition.y + 0.4f, shelfBoards[shelfRows - 1].transform.localPosition.z);
                roofBoard.transform.localScale = new Vector3(newScaleX, 0.003f, newScaleZ);

            }

            if (newShelfRow < shelfRows && newShelfRow > 0)
            {
                GameObject lastBoard = shelfBoards[shelfBoards.Count - 1];
                Destroy(lastBoard);
                shelfBoards.RemoveAt(shelfBoards.Count - 1);
                shelfRows--;
				curveRenderers.RemoveAt (shelfBoards.Count);
                foreach (GameObject pillar in shelfPillars)
                {
                    pillar.transform.localPosition -= Vector3.up * 0.2f;
                    pillar.transform.localScale -= Vector3.up * 0.2f;
                }
                currentY -= 0.2f;
                // update roof board
                roofBoard.transform.localPosition = new Vector3(newPositionX, roofBoard.transform.localPosition.y - 0.4f, shelfBoards[shelfRows - 1].transform.localPosition.z);
                roofBoard.transform.localScale = new Vector3(newScaleX, 0.003f, newScaleZ);

            }
        }
		

		foreach (GameObject board in shelfBoards) {
			board.transform.localPosition = new Vector3 (newPositionX,board.transform.localPosition.y, currentBoardPositionZ);
			board.transform.localScale = new Vector3 (newScaleX, board.transform.localScale.y, board.transform.localScale.z);

			Transform renderer = board.transform.GetChild (0);
			renderer.localPosition = new Vector3 (renderer.localPosition.x, renderer.localPosition.y, currentCurveRendererZ);;

			Transform point2 = board.transform.GetChild(0).Find("Point2");
			point2.localPosition = new Vector3(point2.localPosition.x, point2.localPosition.y, currentCurvature);
		}
        roofBoard.transform.localPosition = new Vector3(newPositionX, roofBoard.transform.localPosition.y, currentBoardPositionZ);
        roofBoard.transform.localScale = new Vector3(newScaleX, roofBoard.transform.localScale.y, roofBoard.transform.localScale.z);
    }

	void UpdatePillar(){
		Vector3 leftPillarPosition = shelfPillars [0].transform.localPosition;
		Vector3 rightPillarPosition = shelfPillars [1].transform.localPosition;
		//oldPillarY = shelfPillars [2].transform.localPosition.y;
		float distance = rightPillarPosition.x - leftPillarPosition.x;

        if (distance < delta)
        {
            if (oldLeftPosition != leftPillarPosition)
            {
                shelfPillars[0].transform.localPosition = rightPillarPosition - Vector3.right * delta;
            }
            else if (oldRightPosition != rightPillarPosition)
            {
                shelfPillars[1].transform.localPosition = leftPillarPosition + Vector3.right * delta;
            }
            else
            {
                shelfPillars[0].transform.localPosition = rightPillarPosition - Vector3.right * delta;
                Debug.Log("Both Moved!!!!");
            }
            GameObject leftController = GameObject.Find("Controller (left)");
            SteamVR_TrackedController ltc = leftController.GetComponent<SteamVR_TrackedController>();
            SteamVR_Controller.Input((int)ltc.controllerIndex).TriggerHapticPulse(500);
            GameObject rightController = GameObject.Find("Controller (right)");
            SteamVR_TrackedController rtc = rightController.GetComponent<SteamVR_TrackedController>();
            SteamVR_Controller.Input((int)rtc.controllerIndex).TriggerHapticPulse(500);
        }
        else if (distance > smallMultiplesNumber * delta * 1.5f) {
            shelfPillars[0].transform.localPosition = oldLeftPosition;
            shelfPillars[1].transform.localPosition = oldRightPosition;
        }
		oldLeftPosition = shelfPillars[0].transform.localPosition;
		oldRightPosition = shelfPillars[1].transform.localPosition;

        foreach (GameObject pillar in shelfPillars) {
            pillar.transform.localPosition = new Vector3(pillar.transform.localPosition.x, currentY, pillar.transform.localPosition.z);
        }
    }

    void CreateShelf(){
		float iniLeftx = - (delta * smallMultiplesNumber / 2);
		float iniRightx = delta * smallMultiplesNumber / 2;
		float iniy = 1;

		GameObject pillar = (GameObject)Instantiate(FpillarPrefab, new Vector3(0, 0, 0), Quaternion.identity);
		pillar.transform.SetParent (shelf.transform);
		pillar.transform.localScale = new Vector3 (0.05f, 0.2f, 0.05f);
		pillar.transform.localPosition = new Vector3 (iniLeftx, iniy, 0);
		oldLeftPosition = pillar.transform.localPosition;
        pillar.name = "Left Pillar";
        shelfPillars.Add (pillar);

		pillar = (GameObject)Instantiate(FpillarPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        pillar.transform.SetParent (shelf.transform);
		pillar.transform.localScale = new Vector3 (0.05f, 0.2f, 0.05f);
		pillar.transform.localPosition = new Vector3 (iniRightx, iniy, 0);
        oldRightPosition = pillar.transform.localPosition;
		pillar.name = "Right Pillar";
		shelfPillars.Add (pillar);

        currentY = iniy;

        GameObject board = (GameObject)Instantiate(shelfBoardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
		board.transform.SetParent (shelf.transform);
		board.transform.localScale = new Vector3 (delta * smallMultiplesNumber, 0.003f, delta);
		board.transform.localPosition = new Vector3(0, 0.8f, 0);
		board.name = "Bottom Board";
		shelfBoards.Add (board);

		GameObject curveRenderer = (GameObject)Instantiate(curveRendererPrefab, new Vector3(0, 0, 0), Quaternion.identity);
		curveRenderer.transform.SetParent (board.transform);
		curveRenderer.transform.localPosition = new Vector3(0, 0, 0);
		curveRenderer.transform.localScale = new Vector3 (1,1,1);
		curveRenderer.name = "Bottom Curve Renderer";
		curveRenderers.Add (curveRenderer);

        roofBoard = (GameObject)Instantiate(shelfBoardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        roofBoard.transform.SetParent(shelf.transform);
        roofBoard.transform.localScale = new Vector3(delta * smallMultiplesNumber, 0.003f, delta);
        roofBoard.transform.localPosition = new Vector3(0, 1.2f, 0);
        roofBoard.name = "Roof Board";

        shelfRows = 1;
		shelfItemPerRow = smallMultiplesNumber;
	}

	void CreateSM(){
        GameObject bottomBoard = shelfBoards[0];
        float iniLeftEdge = shelfPillars[0].transform.localPosition.x;
		float iniLeftMostItemX = iniLeftEdge + (delta / 2);
		float iniItemY = bottomBoard.transform.localPosition.y;


		if (smallMultiplesNumber < 1) {
			Debug.Log ("Please enter a valid small multiples number");
		} else if (smallMultiplesNumber > 12) {
			Debug.Log ("More than 12 small multiples are not allowed in this simulation.");
		} else {
			for (int i = 0; i < smallMultiplesNumber; i++)
			{
				GameObject buildingObj = (GameObject)Instantiate(BuildingPrefab, new Vector3(0,0,0), Quaternion.identity);
				buildingObj.name = "Building " + (i + 1);
				buildingObj.transform.SetParent(shelf.transform);
				buildingObj.transform.localPosition = new Vector3(iniLeftMostItemX + (i * delta), iniItemY, 0);
				buildingSM.Add(buildingObj);
			}
		}
	}

    void FindCenter()
    {
        Vector3 centroid = Vector3.zero;
        if (shelf.transform.childCount > 0)
        {
            Transform[] transforms;
            transforms = shelf.GetComponentsInChildren<Transform>();
            foreach (Transform t in transforms)
            {
                centroid += t.position;
            }
            centroid /= transforms.Length;
            centroidGO.transform.position = centroid;
        }
    }

    public void MoveShelfToCenter()
    {
        Vector3 centrePosition = centroidGO.transform.position;
        GameObject tmp = new GameObject();
        tmp.transform.SetParent(this.transform);
        tmp.transform.localPosition = shelf.transform.localPosition;
        int childrenLength = shelf.transform.childCount;
        for (int i = 0; i < childrenLength; i++)
        {
            shelf.transform.GetChild(0).SetParent(tmp.transform);

        }
        shelf.transform.position = centrePosition;
        for (int i = 0; i < childrenLength; i++)
        {
            tmp.transform.GetChild(0).SetParent(shelf.transform);
        }
        
        currentY = shelfPillars[0].transform.localPosition.y;

        Destroy(tmp);
    }

    public void PushShelf() {
        int rowLimit;
        if (smallMultiplesNumber % 2 == 0)
        {
            rowLimit = smallMultiplesNumber / 2;
        }
        else {
            rowLimit = smallMultiplesNumber / 2 + 1;
        }

        currentZDistance += 0.01f;

        if (Mathf.Abs(currentZDistance) <= (rowLimit * delta)) {
            if (Mathf.Abs(currentZDistance) >= delta)
            {
                if (currentZDistance >= 0)
                {
					ExpandShelf();
                }
                else
                {
					//ShrinkShelf();
					currentZDistance -= 0.01f;
                }
            }
        } 
        else {
            currentZDistance -= 0.01f;
        }
    }

    public void PullShelf() {
        int rowLimit;
        if (smallMultiplesNumber % 2 == 0)
        {
            rowLimit = smallMultiplesNumber / 2;
        }
        else
        {
            rowLimit = smallMultiplesNumber / 2 + 1;
        }

        currentZDistance -= 0.01f;

        if (Mathf.Abs(currentZDistance) <= (rowLimit * delta))
        {
			if (Mathf.Abs (currentZDistance) >= delta) {
				if (currentZDistance >= 0) {
					ShrinkShelf ();
				} else {
					//ExpandShelf();
				}
			} else {
				currentZDistance += 0.01f;
			}
        }
        else
        {
            currentZDistance += 0.01f;
        }
    }

    void ExpandShelf() {
        Transform leftTransform = shelfPillars[2].transform;
        PositionLocalConstraints plcl = shelfPillars[2].gameObject.GetComponent<PositionLocalConstraints>();

        Transform rightTransform = shelfPillars[3].transform;
        PositionLocalConstraints plcr = shelfPillars[3].gameObject.GetComponent<PositionLocalConstraints>();

        plcl.UpdateZ(leftTransform.localPosition.z + 0.01f);
        plcr.UpdateZ(rightTransform.localPosition.z + 0.01f);

        currentBoardPositionZ += 0.005f;
		currentCurvature += 0.01f;
		currentCurveRendererZ -= 0.0025f;

        foreach (GameObject board in shelfBoards) {
            Transform boardTransform = board.transform;
            boardTransform.localScale += Vector3.forward * 0.01f;
        }

        Transform roofBoardTran = roofBoard.transform;
        roofBoardTran.localScale += Vector3.forward * 0.01f;
    }

    void ShrinkShelf() {
        Transform leftTransform = shelfPillars[2].transform;
        PositionLocalConstraints plcl = shelfPillars[2].gameObject.GetComponent<PositionLocalConstraints>();

        Transform rightTransform = shelfPillars[3].transform;
        PositionLocalConstraints plcr = shelfPillars[3].gameObject.GetComponent<PositionLocalConstraints>();

        plcl.UpdateZ(leftTransform.localPosition.z - 0.01f);
        plcr.UpdateZ(rightTransform.localPosition.z - 0.01f);

        currentBoardPositionZ -= 0.005f;
		currentCurvature -= 0.01f;
		currentCurveRendererZ += 0.0025f;

        foreach (GameObject board in shelfBoards)
        {
            Transform boardTransform = board.transform;
            boardTransform.localScale -= Vector3.forward * 0.01f;
        }

        Transform roofBoardTran = roofBoard.transform;
        roofBoardTran.localScale -= Vector3.forward * 0.01f;
    }

	public Vector3 FindLeftPoint(){
		return new Vector3 (shelfPillars[0].transform.localPosition.x, shelfBoards[0].transform.localPosition.y, shelfPillars[0].transform.localPosition.z + delta / 2);
	}

	public Vector3 FindRightPoint(){
		return new Vector3 (shelfPillars[1].transform.localPosition.x, shelfBoards[0].transform.localPosition.y, shelfPillars[1].transform.localPosition.z + delta / 2);
	}

	public List<GameObject> GetSMList(){
		return this.buildingSM;
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

    //// create building prefabs
    //void CreateBuildingPrefabs()
    //{
    //    if (smallMultiplesNumber < 1)
    //    {
    //        Debug.Log("Please enter a valid small multiples number");
    //    }
    //    else if (smallMultiplesNumber > 12)
    //    {
    //        Debug.Log("More than 12 small multiples are not allowed in this simulation.");
    //    }
    //    else if (smallMultiplesNumber < 4)
    //    {
    //        shelfRows = 1;
    //        shelfItemPerRow = smallMultiplesNumber;
    //        shelfRowY.Add(0);
    //        for (int i = 0; i < smallMultiplesNumber; i++)
    //        {
    //            float xC = 0.3f - 0.3f * smallMultiplesNumber + 0.6f * i;
    //            GameObject buildingObj = (GameObject)Instantiate(BuildingPrefab, new Vector3(xC, 0, 0), Quaternion.identity);
    //            buildingObj.name = (i + 1) + "";
    //            buildingObj.transform.SetParent(this.transform);
    //            buildingObj.transform.localPosition = new Vector3(xC, 0, 0);
    //            buildingSM.Add(buildingObj);
    //        }

    //    }
    //    else if (smallMultiplesNumber == 4)
    //    {
    //        shelfRows = 2;
    //        shelfItemPerRow = 2;
    //        for (int j = 0; j < 2; j++)
    //        {
    //            float yC = 0.15f - 0.3f * j;
    //            shelfRowY.Add(yC);
    //            for (int i = 0; i < 2; i++)
    //            {
    //                float xC = -0.3f + 0.6f * i;
    //                GameObject buildingObj = (GameObject)Instantiate(BuildingPrefab, new Vector3(xC, yC, 0), Quaternion.identity);
    //                buildingObj.name = 2 * j + i + 1 + "";
    //                buildingObj.transform.SetParent(this.transform);
    //                buildingObj.transform.localPosition = new Vector3(xC, yC, 0);
    //                buildingSM.Add(buildingObj);
    //            }
    //        }

    //    }
    //    else
    //    {
    //        int numRows = 1;
    //        int count = 0;
    //        if (smallMultiplesNumber % 4 == 0)
    //        {
    //            numRows = smallMultiplesNumber / 4;
    //        }
    //        else
    //        {
    //            numRows = smallMultiplesNumber / 4 + 1;
    //        }

    //        shelfRows = numRows;
    //        shelfItemPerRow = 4;
    //        for (int j = 0; j < numRows; j++)
    //        {
    //            float yC = -0.15f + 0.15f * numRows - 0.3f * j;
    //            shelfRowY.Add(yC);
    //            for (int i = 0; i < 4; i++)
    //            {
    //                float xC = -0.9f + 0.6f * i;
    //                GameObject buildingObj = (GameObject)Instantiate(BuildingPrefab, new Vector3(xC, yC, 0), Quaternion.identity);
    //                buildingObj.name = 4 * j + i + 1 + "";
    //                buildingObj.transform.SetParent(this.transform);
    //                buildingObj.transform.localPosition = new Vector3(xC, yC, 0);
    //                buildingSM.Add(buildingObj);



    //                count++;
    //                if (count >= smallMultiplesNumber)
    //                {
    //                    break;
    //                }
    //            }
    //        }
    //    }
    //}

    //void ChangeLayout(string layout)
    //{
    //    if (buildingSM != null)
    //    {
    //        GameObject viveCamera = GameObject.Find("Camera (eye)");
    //        // reset rotation
    //        this.transform.rotation = Quaternion.identity;
    //        foreach (GameObject go in buildingSM)
    //        {
    //            go.transform.rotation = Quaternion.identity;
    //        }
    //        if (layout.Equals("2Dmatrix"))
    //        {
    //            if (smallMultiplesNumber < 4)
    //            {
    //                for (int i = 0; i < smallMultiplesNumber; i++)
    //                {
    //                    float xC = 0.3f - 0.3f * smallMultiplesNumber + 0.6f * i;
    //                    GameObject buildingObj = buildingSM[i];
    //                    buildingObj.transform.localPosition = new Vector3(xC, 0, 0);
    //                }
    //            }
    //            else if (smallMultiplesNumber == 4)
    //            {
    //                for (int j = 0; j < 2; j++)
    //                {
    //                    float yC = 0.15f - 0.3f * j;
    //                    for (int i = 0; i < 2; i++)
    //                    {
    //                        float xC = -0.3f + 0.6f * i;
    //                        GameObject buildingObj = buildingSM[2 * j + i];
    //                        buildingObj.transform.localPosition = new Vector3(xC, yC, 0);
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                int numRows = 1;
    //                int count = 0;
    //                if (smallMultiplesNumber % 4 == 0)
    //                {
    //                    numRows = smallMultiplesNumber / 4;
    //                }
    //                else
    //                {
    //                    numRows = smallMultiplesNumber / 4 + 1;
    //                }
    //                for (int j = 0; j < numRows; j++)
    //                {
    //                    float yC = -0.15f + 0.15f * numRows - 0.3f * j;
    //                    for (int i = 0; i < 4; i++)
    //                    {
    //                        float xC = -0.9f + 0.6f * i;
    //                        GameObject buildingObj = buildingSM[4 * j + i];
    //                        buildingObj.transform.localPosition = new Vector3(xC, yC, 0);
    //                        count++;
    //                        if (count >= smallMultiplesNumber)
    //                        {
    //                            break;
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        else if (layout.Equals("2Dlinear"))
    //        {
    //            for (int i = 0; i < smallMultiplesNumber; i++)
    //            {
    //                float xC = 0.3f - 0.3f * smallMultiplesNumber + 0.6f * i;
    //                GameObject buildingObj = buildingSM[i];
    //                buildingObj.transform.localPosition = new Vector3(xC, 0, 0);
    //            }
    //        }
    //        else if (layout.Equals("2DCmatrix"))
    //        {
    //            if (smallMultiplesNumber < 4)
    //            {
    //                for (int i = 0; i < smallMultiplesNumber; i++)
    //                {
    //                    float xC = 0.3f - 0.3f * smallMultiplesNumber + 0.6f * i;
    //                    GameObject buildingObj = buildingSM[i];
    //                    if (smallMultiplesNumber == 3)
    //                    {
    //                        if (i == 1)
    //                        {
    //                            buildingObj.transform.localPosition = new Vector3(xC, 0, 0.17f);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        buildingObj.transform.localPosition = new Vector3(xC, 0, 0);
    //                    }
    //                }
    //            }
    //            else if (smallMultiplesNumber == 4)
    //            {
    //                for (int j = 0; j < 2; j++)
    //                {
    //                    float yC = 0.15f - 0.3f * j;
    //                    for (int i = 0; i < 2; i++)
    //                    {
    //                        float xC = -0.3f + 0.6f * i;
    //                        GameObject buildingObj = buildingSM[2 * j + i];
    //                        buildingObj.transform.localPosition = new Vector3(xC, yC, 0);
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                int numRows = 1;
    //                int count = 0;
    //                if (smallMultiplesNumber % 4 == 0)
    //                {
    //                    numRows = smallMultiplesNumber / 4;
    //                }
    //                else
    //                {
    //                    numRows = smallMultiplesNumber / 4 + 1;
    //                }
    //                for (int j = 0; j < numRows; j++)
    //                {
    //                    float yC = -0.15f + 0.15f * numRows - 0.3f * j;
    //                    for (int i = 0; i < 4; i++)
    //                    {
    //                        float xC = -0.9f + 0.6f * i;
    //                        GameObject buildingObj = buildingSM[4 * j + i];
    //                        if ((i % 4 == 1) || (i % 4 == 2))
    //                        {
    //                            buildingObj.transform.localPosition = new Vector3(xC, yC, 0.3f);
    //                        }
    //                        else
    //                        {
    //                            buildingObj.transform.localPosition = new Vector3(xC, yC, 0);
    //                        }
    //                        count++;
    //                        if (count >= smallMultiplesNumber)
    //                        {
    //                            break;
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        else if (layout.Equals("2DClinear"))
    //        {
    //            float xC1 = 0.0f;
    //            float xC2 = 0.0f;
    //            GameObject buildingObj;

    //            if (smallMultiplesNumber % 2 == 0)
    //            {
    //                if (smallMultiplesNumber != 2)
    //                {
    //                    xC1 = 0.3f - 0.3f * smallMultiplesNumber + 0.6f * (smallMultiplesNumber / 2 - 1);
    //                    xC2 = 0.3f - 0.3f * smallMultiplesNumber + 0.6f * (smallMultiplesNumber / 2);
    //                    Vector3 pos1 = new Vector3(xC1, 0, 0);
    //                    buildingSM[smallMultiplesNumber / 2 - 1].transform.localPosition = pos1;
    //                    Vector3 pos2 = new Vector3(xC2, 0, 0);
    //                    buildingSM[smallMultiplesNumber / 2].transform.localPosition = pos2;
    //                    for (int i = smallMultiplesNumber / 2 - 2; i >= 0; i--)
    //                    {
    //                        buildingObj = buildingSM[i];
    //                        buildingObj.transform.localPosition = new Vector3(pos1.x - linearCurvedOffsetx * (smallMultiplesNumber / 2 - 1 - i), pos1.y, pos1.z - linearCurvedOffsetz * (smallMultiplesNumber / 2 - 1 - i));
    //                    }
    //                    for (int i = smallMultiplesNumber / 2 + 1; i < smallMultiplesNumber; i++)
    //                    {
    //                        buildingObj = buildingSM[i];
    //                        buildingObj.transform.localPosition = new Vector3(pos2.x + linearCurvedOffsetx * (i - smallMultiplesNumber / 2), pos2.y, pos2.z - linearCurvedOffsetz * (i - smallMultiplesNumber / 2));
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                if (smallMultiplesNumber != 1)
    //                {
    //                    xC1 = 0.3f - 0.3f * smallMultiplesNumber + 0.6f * (smallMultiplesNumber / 2);
    //                    Vector3 pos1 = new Vector3(xC1, 0, 0);
    //                    buildingSM[smallMultiplesNumber / 2].transform.localPosition = pos1;
    //                    for (int i = smallMultiplesNumber / 2 - 1; i >= 0; i--)
    //                    {
    //                        buildingObj = buildingSM[i];
    //                        buildingObj.transform.localPosition = new Vector3(pos1.x - linearCurvedOffsetx * (smallMultiplesNumber / 2 - i), pos1.y, pos1.z - linearCurvedOffsetz * (smallMultiplesNumber / 2 - i));
    //                    }
    //                    for (int i = smallMultiplesNumber / 2 + 1; i < smallMultiplesNumber; i++)
    //                    {
    //                        buildingObj = buildingSM[i];
    //                        buildingObj.transform.localPosition = new Vector3(pos1.x + linearCurvedOffsetx * (i - smallMultiplesNumber / 2), pos1.y, pos1.z - linearCurvedOffsetz * (i - smallMultiplesNumber / 2));
    //                    }
    //                }
    //            }
    //        }

    //        this.transform.position = viveCamera.transform.position + (viveCamera.transform.forward * 1f) - (viveCamera.transform.up * 0.2f);
    //        Vector3 camPos = viveCamera.transform.position;
    //        Vector3 finalPos = new Vector3(camPos.x, this.transform.position.y, camPos.z);
    //        Vector3 offset = this.transform.position - finalPos;
    //        this.transform.LookAt(this.transform.position + offset);
    //    }
    //    else
    //    {
    //        Debug.Log("Building list is empty!");
    //    }
    //}

    //public void Select2Dmatrix()
    //{
    //    if (!layout.Equals("2Dmatrix"))
    //    {
    //        this.ChangeLayout("2Dmatrix");
    //        layout = "2Dmatrix";
    //    }
    //}

    //public void Select2Dlinear()
    //{
    //    if (!layout.Equals("2Dlinear"))
    //    {
    //        this.ChangeLayout("2Dlinear");
    //        layout = "2Dlinear";
    //    }
    //}

    //public void Select2DCmatrix()
    //{
    //    if (!layout.Equals("2DCmatrix"))
    //    {
    //        this.ChangeLayout("2DCmatrix");
    //        layout = "2DCmatrix";
    //    }
    //}

    //public void Select2DClinear()
    //{
    //    if (!layout.Equals("2DClinear"))
    //    {
    //        this.ChangeLayout("2DClinear");
    //        layout = "2DClinear";
    //    }
    //}

}
