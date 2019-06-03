using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;

public class ShelfScript : MonoBehaviour {

	List<GameObject> shelfBoards;
	List<GameObject> shelfPillars;
	List<float> shelfRowY;

	float delta = 0.65f;
	int shelfRows = 0;
	int shelfItemPerRow = 0;

	int smallMultiplesNumber = 8;

	Vector3 oldLeftPosition;
	Vector3 oldRightPosition;

	// Use this for initialization
	void Start () {
		shelfRowY = new List<float> ();
		shelfBoards = new List<GameObject> ();
		shelfPillars = new List<GameObject> ();

		createShelf ();
	}
	
	// Update is called once per frame
	void Update () {
		updatePillar ();
		updateBoards ();
	}

	void updatePillar(){
		Vector3 leftPillarPosition = shelfPillars [0].transform.localPosition;
		Vector3 rightPillarPosition = shelfPillars [1].transform.localPosition;

		float distance = rightPillarPosition.x - leftPillarPosition.x;

		if (distance < delta) {
			shelfPillars [0].transform.localPosition = oldLeftPosition;
			shelfPillars [1].transform.localPosition = oldRightPosition;
		} else {
			shelfPillars [2].transform.localPosition = new Vector3 (leftPillarPosition.x, shelfPillars [2].transform.localPosition.y, shelfPillars [2].transform.localPosition.z);
			shelfPillars [3].transform.localPosition = new Vector3 (rightPillarPosition.x, shelfPillars [3].transform.localPosition.y, shelfPillars [3].transform.localPosition.z);

			oldLeftPosition = leftPillarPosition;
			oldRightPosition = rightPillarPosition;
		}
	}

	void updateBoards (){
		GameObject leftPillar = shelfPillars [0];
		GameObject rightPillar = shelfPillars [1];

		float newPositionX = (leftPillar.transform.localPosition.x + rightPillar.transform.localPosition.x) / 2;
		float newScaleX = Mathf.Abs(rightPillar.transform.localPosition.x - leftPillar.transform.localPosition.x);

		float division = newScaleX / delta;
		int newItemPerRow = (int) division;
		Debug.Log (division + " " + newItemPerRow);
		int reminder = smallMultiplesNumber % newItemPerRow;
		int newShelfRow;
		if (reminder != 0) {
			newShelfRow = smallMultiplesNumber / newItemPerRow + 1;
		} else {
			newShelfRow = smallMultiplesNumber / newItemPerRow;
		}


		if (newShelfRow > shelfRows) {
			GameObject board = GameObject.CreatePrimitive(PrimitiveType.Cube);
			board.transform.SetParent (this.transform);
			board.transform.localScale = new Vector3 (newScaleX, 0.003f, delta);
			board.transform.localPosition = new Vector3(newPositionX, shelfBoards[shelfRows - 1].transform.localPosition.y + 0.3f, shelfBoards[shelfRows - 1].transform.localPosition.z);
			board.name = "ShelfRow " + (shelfBoards.Count + 1);
			shelfBoards.Add (board);
			shelfRows++;

			foreach (GameObject pillar in shelfPillars) {
				pillar.transform.localPosition += Vector3.up * 0.15f;
				pillar.transform.localScale += Vector3.up * 0.15f;
			}
		}

		if (newShelfRow < shelfRows) {
			GameObject lastBoard = shelfBoards [shelfBoards.Count - 1];
			Destroy (lastBoard);
			shelfBoards.RemoveAt (shelfBoards.Count - 1);
			shelfRows--;

			foreach (GameObject pillar in shelfPillars) {
				pillar.transform.localPosition -= Vector3.up * 0.15f;
				pillar.transform.localScale -= Vector3.up * 0.15f;
			}
		}

		foreach (GameObject board in shelfBoards) {
			board.transform.localPosition = new Vector3 (newPositionX,board.transform.localPosition.y, board.transform.localPosition.z);
			board.transform.localScale = new Vector3 (newScaleX, board.transform.localScale.y, board.transform.localScale.z);
		}


	}

	void createShelf(){
		float iniLeftx = - (delta * smallMultiplesNumber / 2);
		float iniRightx = delta * smallMultiplesNumber / 2;
		float iniy = 0.5f;
		float iniFrontz = - delta / 2;
		float iniBackz = delta / 2;

		GameObject pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		pillar.transform.SetParent (this.transform);
		pillar.transform.localScale = new Vector3 (0.05f, 0.5f, 0.05f);
		pillar.transform.localPosition = new Vector3 (iniLeftx, iniy, iniFrontz);
		oldLeftPosition = pillar.transform.localPosition;
		pillar.name = "LeftPillar";
		shelfPillars.Add (pillar);

		pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		pillar.transform.SetParent (this.transform);
		pillar.transform.localScale = new Vector3 (0.05f, 0.5f, 0.05f);
		pillar.transform.localPosition = new Vector3 (iniRightx, iniy, iniFrontz);	
		oldRightPosition = pillar.transform.localPosition;
		pillar.name = "RightPillar";
		shelfPillars.Add (pillar);

		pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		pillar.transform.SetParent (this.transform);
		pillar.transform.localScale = new Vector3 (0.05f, 0.5f, 0.05f);
		pillar.transform.localPosition = new Vector3 (iniLeftx, iniy, iniBackz);		
		pillar.name = "LeftPillarBack";
		shelfPillars.Add (pillar);

		pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		pillar.transform.SetParent (this.transform);
		pillar.transform.localScale = new Vector3 (0.05f, 0.5f, 0.05f);
		pillar.transform.localPosition = new Vector3 (iniRightx, iniy, iniBackz);
		pillar.name = "RightPillarBack";
		shelfPillars.Add (pillar);

		GameObject board = GameObject.CreatePrimitive(PrimitiveType.Cube);
		board.transform.SetParent (this.transform);
		board.transform.localScale = new Vector3 (delta * smallMultiplesNumber, 0.003f, delta);
		board.transform.localPosition = new Vector3(0, 0.8f, 0);
		board.name = "ShelfRow 1";
		shelfBoards.Add (board);

		shelfRows = 1;
		shelfItemPerRow = smallMultiplesNumber;
	}
}
