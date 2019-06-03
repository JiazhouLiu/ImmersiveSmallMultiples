using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ReadACData : MonoBehaviour {

	/*List<ACData> acDataList = new List<ACData>();
	public string[] monthlyTempAvg = new string[12];
	bool tempTextFlag = false;
	public GameObject prefab;
	public GameObject light;
	public TextAsset ACFile;

	// Use this for initialization
	void Start () {
		// read ac file

		string[] data = ACFile.text.Split (new char[] { '\n' });

		// split rows
		for (int i = 1; i < data.Length; i++) {
			string[] row = data [i].Split (new char[] { ',' });

			if (row[8].Replace("\"", "").Trim().Equals("ACG.1")){
				ACData d = new ACData ();
				d.id = row [0].Replace("\"", "");
				d.DateTime = row [1].Replace("\"", "");
				d.Temp = row [2].Replace("\"", "");
				d.SPHi = row [3].Replace("\"", "");
				d.SPLo = row [4].Replace("\"", "");
				d.Status = row [5].Replace("\"", "");
				d.ACUnitOCC = row [6].Replace("\"", "");
				d.RoofTemp = row [7].Replace("\"", "");
				d.Name = row [8].Replace("\"", "");

				acDataList.Add (d);
			}

		}

		// remove 2018 record
		//acDataList.RemoveAt(0);

		// Lists to store temp values
		List<float> JanTemp = new List<float>();
		List<float> FebTemp = new List<float>();
		List<float> MarTemp = new List<float>();
		List<float> AprTemp = new List<float>();
		List<float> MayTemp = new List<float>();
		List<float> JunTemp = new List<float>();
		List<float> JulTemp = new List<float>();
		List<float> AugTemp = new List<float>();
		List<float> SepTemp = new List<float>();
		List<float> OctTemp = new List<float>();
		List<float> NovTemp = new List<float>();
		List<float> DecTemp = new List<float>();

		// add values to lists
		foreach (ACData d in acDataList) {
			string[] date = d.DateTime.Split (new char[] { ' ' });
			if (!d.Temp.Equals ("-")) {
				//Debug.Log (date [0].Split (new char[] { '/' }));
				switch (date [0].Split (new char[] { '/' })[1])
				{
				case "01":
					float temp1;
					float.TryParse( d.Temp.Replace(" ", ""), out temp1 );
					//Debug.Log (d.id + " " + temp);
					JanTemp.Add(temp1);
					break;
				case "02":
					float temp2;
					float.TryParse( d.Temp.Replace(" ", ""), out temp2 );
					//Debug.Log (d.id + " " + temp);
					FebTemp.Add(temp2);
					break;
				case "03":
					float temp3;
					float.TryParse( d.Temp.Replace(" ", ""), out temp3 );
					//Debug.Log (d.id + " " + temp);
					MarTemp.Add(temp3);
					break;
				case "04":
					float temp4;
					float.TryParse( d.Temp.Replace(" ", ""), out temp4 );
					//Debug.Log (d.id + " " + temp);
					AprTemp.Add(temp4);
					break;
				case "05":
					float temp5;
					float.TryParse( d.Temp.Replace(" ", ""), out temp5 );
					//Debug.Log (d.id + " " + temp);
					MayTemp.Add(temp5);
					break;
				case "06":
					float temp6;
					float.TryParse( d.Temp.Replace(" ", ""), out temp6 );
					//Debug.Log (d.id + " " + temp);
					JunTemp.Add(temp6);
					break;
				case "07":
					float temp7;
					float.TryParse( d.Temp.Replace(" ", ""), out temp7 );
					//Debug.Log (d.id + " " + temp);
					JulTemp.Add(temp7);
					break;
				case "08":
					float temp8;
					float.TryParse( d.Temp.Replace(" ", ""), out temp8 );
					//Debug.Log (d.id + " " + temp);
					AugTemp.Add(temp8);
					break;
				case "09":
					float temp9;
					float.TryParse( d.Temp.Replace(" ", ""), out temp9 );
					//Debug.Log (d.id + " " + temp);
					SepTemp.Add(temp9);
					break;
				case "10":
					float temp10;
					float.TryParse( d.Temp.Replace(" ", ""), out temp10 );
					//Debug.Log (d.id + " " + temp);
					OctTemp.Add(temp10);
					break;
				case "11":
					float temp11;
					float.TryParse( d.Temp.Replace(" ", ""), out temp11 );
					//Debug.Log (d.id + " " + temp);
					NovTemp.Add(temp11);
					break;
				case "12":
					float temp12;
					float.TryParse( d.Temp.Replace(" ", ""), out temp12 );
					//Debug.Log (d.id + " " + temp);
					DecTemp.Add(temp12);
					break;
				}
			}
		}

		// Get Jan avg
		float JanTotal = 0;
		foreach (float temp in JanTemp) {
			JanTotal += temp;
		}
		float JanAvg = JanTotal / JanTemp.Count;
		monthlyTempAvg [0] = "Jan: " + JanAvg.ToString("0.00") + " °C";

		//Get Feb avg
		float FebTotal = 0;
		foreach (float temp in FebTemp) {
			FebTotal += temp;
		}
		float FebAvg = FebTotal / FebTemp.Count;
		monthlyTempAvg [1] = "Feb: " + FebAvg.ToString("0.00") + " °C";

		//Get march avg
		float MarTotal = 0;
		foreach (float temp in MarTemp) {
			MarTotal += temp;
		}
		float MarAvg = MarTotal / MarTemp.Count;
		monthlyTempAvg [2] = "Mar: " + MarAvg.ToString("0.00") + " °C";

		//Get Apr
		float AprTotal = 0;
		foreach (float temp in AprTemp) {
			AprTotal += temp;
		}
		float AprAvg = AprTotal / AprTemp.Count;
		monthlyTempAvg [3] = "Apr: " + AprAvg.ToString("0.00") + " °C";

		//Get May
		float MayTotal = 0;
		foreach (float temp in MayTemp) {
			MayTotal += temp;
		}
		float MayAvg = MayTotal / MayTemp.Count;
		monthlyTempAvg [4] = "May: " + MayAvg.ToString("0.00") + " °C";

		//Get Jun
		float JunTotal = 0;
		foreach (float temp in JunTemp) {
			JunTotal += temp;
		}
		float JunAvg = JunTotal / JunTemp.Count;
		monthlyTempAvg [5] = "Jun: " + JunAvg.ToString("0.00") + " °C";

		//Get Jul
		float JulTotal = 0;
		foreach (float temp in JulTemp) {
			JulTotal += temp;
		}
		float JulAvg = JulTotal / JulTemp.Count;
		monthlyTempAvg [6] = "Jul: " + JulAvg.ToString("0.00") + " °C";

		//Get Aug
		float AugTotal = 0;
		foreach (float temp in AugTemp) {
			AugTotal += temp;
		}
		float AugAvg = AugTotal / AugTemp.Count;
		monthlyTempAvg [7] = "Aug: " + AugAvg.ToString("0.00") + " °C";

		//Get Sep
		float SepTotal = 0;
		foreach (float temp in SepTemp) {
			SepTotal += temp;
		}
		float SepAvg = SepTotal / SepTemp.Count;
		monthlyTempAvg [8] = "Sep: " + SepAvg.ToString("0.00") + " °C";

		//Get Oct
		float OctTotal = 0;
		foreach (float temp in OctTemp) {
			OctTotal += temp;
		}
		float OctAvg = OctTotal / OctTemp.Count;
		monthlyTempAvg [9] = "Oct: " + OctAvg.ToString("0.00") + " °C";

		//Get Nov
		float NovTotal = 0;
		foreach (float temp in NovTemp) {
			NovTotal += temp;
		}
		float NovAvg = NovTotal / NovTemp.Count;
		monthlyTempAvg [10] = "Nov: " + NovAvg.ToString("0.00") + " °C";

		//Get Dec
		float DecTotal = 0;
		foreach (float temp in DecTemp) {
			DecTotal += temp;
		}
		float DecAvg = DecTotal / DecTemp.Count;
		monthlyTempAvg [11] = "Dec: " + DecAvg.ToString("0.00") + " °C";

		// test avg values
//		Debug.Log (monthlyTempAvg [0]);
//		Debug.Log (monthlyTempAvg [1]);
//		Debug.Log (monthlyTempAvg [2]);
//		Debug.Log (monthlyTempAvg [3]);
//		Debug.Log (monthlyTempAvg [4]);
//		Debug.Log (monthlyTempAvg [5]);
//		Debug.Log (monthlyTempAvg [6]);
//		Debug.Log (monthlyTempAvg [7]);
//		Debug.Log (monthlyTempAvg [8]);
//		Debug.Log (monthlyTempAvg [9]);
//		Debug.Log (monthlyTempAvg [10]);
//		Debug.Log (monthlyTempAvg [11]);
		//Debug.Log(this.name);
		// setup text label for temp value

	}

	// Update is called once per frame
	void Update () {

	}

	public void onClickMonthlyView () {

		if (!tempTextFlag) {
			GameObject canvasObj = GameObject.Find ("Canvas");


			for (int i = 1; i <= 12; i++) {
				GameObject UItextGO = new GameObject ("nameLabel" + i);
				UItextGO.transform.SetParent (canvasObj.transform);

				RectTransform trans = UItextGO.AddComponent<RectTransform> ();
				trans.sizeDelta = new Vector2 (150, 100);
				trans.anchoredPosition = new Vector2 (0, 0);

				Text text = UItextGO.AddComponent<Text> ();
				text.font = Resources.GetBuiltinResource (typeof(Font), "Arial.ttf") as Font;
				text.color = Color.black;

				text.fontSize = 20;
				text.alignment = TextAnchor.UpperCenter;
				text.text = monthlyTempAvg[i-1];

				switch (i)
				{
				case 1:
					GameObject buildingObj1 = (GameObject)Instantiate (prefab, new Vector3(-15,10,0), Quaternion.identity);
					buildingObj1.name = i + "";
					break;
				case 2:
					GameObject buildingObj2 = (GameObject)Instantiate (prefab, new Vector3(-5,10,0), Quaternion.identity);
					buildingObj2.name = i + "";
					break;
				case 3:
					GameObject buildingObj3 = (GameObject)Instantiate (prefab, new Vector3(5,10,0), Quaternion.identity);
					buildingObj3.name = i + "";
					break;
				case 4:
					GameObject buildingObj4 = (GameObject)Instantiate (prefab, new Vector3(15,10,0), Quaternion.identity);
					buildingObj4.name = i + "";
					break;
				case 5:
					GameObject buildingObj5 = (GameObject)Instantiate (prefab, new Vector3(-15,0,0), Quaternion.identity);
					buildingObj5.name = i + "";
					break;
				case 6:
					GameObject buildingObj6 = (GameObject)Instantiate (prefab, new Vector3(-5,0,0), Quaternion.identity);
					buildingObj6.name = i + "";
					break;
				case 7:
					GameObject buildingObj7 = (GameObject)Instantiate (prefab, new Vector3(5,0,0), Quaternion.identity);
					buildingObj7.name = i + "";
					break;
				case 8:
					GameObject buildingObj8 = (GameObject)Instantiate (prefab, new Vector3(15,0,0), Quaternion.identity);
					buildingObj8.name = i + "";
					break;
				case 9:
					GameObject buildingObj9 = (GameObject)Instantiate (prefab, new Vector3(-15,-10,0), Quaternion.identity);
					buildingObj9.name = i + "";
					break;
				case 10:
					GameObject buildingObj10 = (GameObject)Instantiate (prefab, new Vector3(-5,-10,0), Quaternion.identity);
					buildingObj10.name = i + "";
					break;
				case 11:
					GameObject buildingObj11 = (GameObject)Instantiate (prefab, new Vector3(5,-10,0), Quaternion.identity);
					buildingObj11.name = i + "";
					break;
				case 12:
					GameObject buildingObj12 = (GameObject)Instantiate (prefab, new Vector3(15,-10,0), Quaternion.identity);
					buildingObj12.name = i + "";
					break;
				}

			}
			tempTextFlag = true;
		}
	}*/
}
