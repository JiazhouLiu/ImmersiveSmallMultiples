using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SensorInfo : MonoBehaviour {
	

//	enum Type {AC, OAPU};
//
//	class SensorReading{
//		public DateTime dt;
//		public float temp;
//		public float spHi;
//		public float spLo;
//		public float roofTemp;
//		public string status;
//		public string acUnit;
//	}

	SortedList<DateTime, SensorReading> sensorStream;

	bool init;
//	Type typeSensor;

//	private DateTime present;
//	private DateTime nextTime;
//	private int currentInd;
//	private bool pastOnly;

	public float[] tempAvg;

    public string[] tempTag;

    // Use this for initialization
    void Start () {
		
    }
    void Awake()
    {
		sensorStream = new SortedList<DateTime, SensorReading> ();
		init = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

	public void sendDataAC (DateTime dt, float temp, float spHi, float spLo, string status, string acUnit, float roofTemp)
	{
		if (!init) {
			init = true;
		}
		SensorReading sr = new SensorReading ();
		sr.dt = dt;
		sr.temp = temp;
		sr.spHi = spHi;
		sr.spLo = spLo;
		sr.status = status;
		sr.acUnit = acUnit;
		sr.roofTemp = roofTemp;

		this.sensorStream.Add (dt, sr);
	}

	private int BinarySearch(IList<DateTime> list, DateTime value)
	{
		if (list == null)
			throw new ArgumentNullException("list");
		var comp = Comparer<DateTime>.Default;
		int lo = 0, hi = list.Count - 1;
		while (lo < hi) {
			int m = (hi + lo) / 2;  // this might overflow; be careful.
			if (comp.Compare(list[m], value) < 0) lo = m + 1;
			else hi = m - 1;
		}
		if (comp.Compare(list[lo], value) < 0) lo++;
		return lo;
	}

	private int FindFirstIndexGreaterThanOrEqualTo(SortedList<DateTime, SensorReading> sortedList, DateTime key)
	{
		return BinarySearch(sortedList.Keys, key);
	}

	public void getSensorReadings(int smNumber, int dateDiff) {
		
		if (init) {
			List<int> dateIndexList = new List<int> ();
			DateTime startDT = new DateTime(2017, 1, 1, 0, 0, 0);

            int startDTIndex = FindFirstIndexGreaterThanOrEqualTo(this.sensorStream, startDT);

            dateIndexList.Add (startDTIndex);
			DateTime endDT = new DateTime (2018, 1, 1, 0, 0, 0);
            DateTime tmp = startDT;
            while (true)
            {
                DateTime nextDT = tmp.AddDays(dateDiff);
                if (DateTime.Compare(nextDT, endDT) >= 0 || dateIndexList.Count == (smNumber + 1))
                {
                    break;
                }
                dateIndexList.Add(FindFirstIndexGreaterThanOrEqualTo(this.sensorStream, nextDT));
                tmp = nextDT;
            }

            ///// diff algorithm
            //DateTime nextDT = startDT.AddDays(dateDiff);
            //int nextDTIndex = FindFirstIndexGreaterThanOrEqualTo(this.sensorStream, nextDT);
            //dateIndexList.Add(nextDTIndex);


            //for (int i = 2; i < smNumber + 1; i++) {
            //    nextDT = nextDT.AddDays(dateDiff);
            //    dateIndexList.Add(startDTIndex + i * (nextDTIndex - startDTIndex));
            //}
            ////end diff algorithm

            // testing
            //for (int r = 0; r < dateIndexList.Count; r++) {
            //    Debug.Log(dateIndexList[r]);
            //}


            float[] temperatureTotal = new float[dateIndexList.Count - 1];
			int[] temperatureCount = new int[dateIndexList.Count - 1];

            tempTag = new string[dateIndexList.Count - 1];
            for (int j = 0; j < dateIndexList.Count - 1; j++)
            {
                tempTag[j] = this.sensorStream.Values[dateIndexList[j]].dt.ToString("MMM dd") + " - " + this.sensorStream.Values[dateIndexList[j + 1]].dt.AddDays(-1).ToString("MMM dd");
                for (int i = 0; i < this.sensorStream.Count; i++) {
					if (i >= dateIndexList [j] && i < dateIndexList [j + 1]) {
						if (this.sensorStream.Values [i].temp != -1) {
							temperatureTotal[j] += this.sensorStream.Values[i].temp;
							temperatureCount[j] += 1;
						}
					}
				
				}
			}

			tempAvg = new float[dateIndexList.Count - 1];
			for (int i = 0; i < tempAvg.Length; i++) {
				tempAvg [i] = temperatureTotal [i] / temperatureCount [i];
            }
		}
    }


	public void getMonthlySensorReadings() {
		DateTime janStart = new DateTime(2017, 1, 1, 0, 0, 0);
		DateTime febStart = new DateTime(2017, 2, 1, 0, 0, 0);
		DateTime marStart = new DateTime(2017, 3, 1, 0, 0, 0);
		DateTime aprStart = new DateTime(2017, 4, 1, 0, 0, 0);
		DateTime mayStart = new DateTime(2017, 5, 1, 0, 0, 0);
		DateTime junStart = new DateTime(2017, 6, 1, 0, 0, 0);
		DateTime julStart = new DateTime(2017, 7, 1, 0, 0, 0);
		DateTime augStart = new DateTime(2017, 8, 1, 0, 0, 0);
		DateTime sepStart = new DateTime(2017, 9, 1, 0, 0, 0);
		DateTime octStart = new DateTime(2017, 10, 1, 0, 0, 0);
		DateTime novStart = new DateTime(2017, 11, 1, 0, 0, 0);
		DateTime decStart = new DateTime(2017, 12, 1, 0, 0, 0);
		DateTime newYearStart = new DateTime(2018, 1, 1, 0, 0, 0);
		if (init) {
			int janIndex = FindFirstIndexGreaterThanOrEqualTo (this.sensorStream, janStart);
			int febIndex = FindFirstIndexGreaterThanOrEqualTo (this.sensorStream, febStart);
			int marIndex = FindFirstIndexGreaterThanOrEqualTo (this.sensorStream, marStart);
			int aprIndex = FindFirstIndexGreaterThanOrEqualTo (this.sensorStream, aprStart);
			int mayIndex = FindFirstIndexGreaterThanOrEqualTo (this.sensorStream, mayStart);
			int junIndex = FindFirstIndexGreaterThanOrEqualTo (this.sensorStream, junStart);
			int julIndex = FindFirstIndexGreaterThanOrEqualTo (this.sensorStream, julStart);
			int augIndex = FindFirstIndexGreaterThanOrEqualTo (this.sensorStream, augStart);
			int sepIndex = FindFirstIndexGreaterThanOrEqualTo (this.sensorStream, sepStart);
			int octIndex = FindFirstIndexGreaterThanOrEqualTo (this.sensorStream, octStart);
			int novIndex = FindFirstIndexGreaterThanOrEqualTo (this.sensorStream, novStart);
			int decIndex = FindFirstIndexGreaterThanOrEqualTo (this.sensorStream, decStart);
			int newYearIndex = FindFirstIndexGreaterThanOrEqualTo (this.sensorStream, newYearStart);

			float[] monthlyTotal = new float[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
			int[] monthlyCount = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

			for (int i = 0; i < this.sensorStream.Count; i++) {
				if (i >= janIndex && i < febIndex) {
                    if (this.sensorStream.Values[i].temp != -1) {
                        monthlyTotal[0] += this.sensorStream.Values[i].temp;
                        monthlyCount[0] += 1;
                    }
				} else if (i >= febIndex && i < marIndex) {
                    if (this.sensorStream.Values[i].temp != -1)
                    {
                        monthlyTotal[1] += this.sensorStream.Values[i].temp;
                        monthlyCount[1] += 1;
                    }
				} else if (i >= marIndex && i < aprIndex) {
                    if (this.sensorStream.Values[i].temp != -1)
                    {
                        monthlyTotal[2] += this.sensorStream.Values[i].temp;
                        monthlyCount[2] += 1;
                    }
				} else if (i >= aprIndex && i < mayIndex) {
                    if (this.sensorStream.Values[i].temp != -1)
                    {
                        monthlyTotal[3] += this.sensorStream.Values[i].temp;
                        monthlyCount[3] += 1;
                    }
				} else if (i >= mayIndex && i < junIndex) {
                    if (this.sensorStream.Values[i].temp != -1)
                    {
                        monthlyTotal[4] += this.sensorStream.Values[i].temp;
                        monthlyCount[4] += 1;
                    }
				} else if (i >= junIndex && i < julIndex) {
                    if (this.sensorStream.Values[i].temp != -1)
                    {
                        monthlyTotal[5] += this.sensorStream.Values[i].temp;
                        monthlyCount[5] += 1;
                    }
				} else if (i >= julIndex && i < augIndex) {
                    if (this.sensorStream.Values[i].temp != -1)
                    {
                        monthlyTotal[6] += this.sensorStream.Values[i].temp;
                        monthlyCount[6] += 1;
                    }
				} else if (i >= augIndex && i < sepIndex) {
                    if (this.sensorStream.Values[i].temp != -1)
                    {
                        monthlyTotal[7] += this.sensorStream.Values[i].temp;
                        monthlyCount[7] += 1;
                    }
				} else if (i >= sepIndex && i < octIndex) {
                    if (this.sensorStream.Values[i].temp != -1)
                    {
                        monthlyTotal[8] += this.sensorStream.Values[i].temp;
                        monthlyCount[8] += 1;
                    }
				} else if (i >= octIndex && i < novIndex) {
                    if (this.sensorStream.Values[i].temp != -1)
                    {
                        monthlyTotal[9] += this.sensorStream.Values[i].temp;
                        monthlyCount[9] += 1;
                    }
				} else if (i >= novIndex && i < decIndex) {
                    if (this.sensorStream.Values[i].temp != -1)
                    {
                        monthlyTotal[10] += this.sensorStream.Values[i].temp;
                        monthlyCount[10] += 1;
                    }
				} else if (i >= decIndex && i < newYearIndex) {
                    if (this.sensorStream.Values[i].temp != -1)
                    {
                        monthlyTotal[11] += this.sensorStream.Values[i].temp;
                        monthlyCount[11] += 1;
                    }
				}
			}
			tempAvg = new float[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
			for (int i = 0; i < 12; i++) {
                tempAvg[i] = monthlyTotal [i] / monthlyCount [i];
			}

		}
	}


//	public void initS(string type){
//		if (type == "AC") {
//			typeSensor = Type.AC;
//		} else if (type == "OAPU") {
//			typeSensor = Type.OAPU;
//		}
//	}

//	public void play(DateTime present){
//		if (init) {
//			this.present = present;
//			this.currentInd = FindFirstIndexGreaterThanOrEqualTo (this.sensorStream, present) - 1;
//			if (sensorStream.Count < this.currentInd) { 
//				pastOnly = true;
//			} else {
//				nextTime = sensorStream.Keys [this.currentInd + 1];
//				pastOnly = false;
//			}
//			changeState (this.currentInd);
//		}
//	}



//	public void newTime(DateTime newDate){
//		if (init) {
//			this.present = newDate;
//			if (newDate >= nextTime) {
//				//print ("Change sensor");
//				this.currentInd++;
//				if (sensorStream.Count < this.currentInd) { 
//					pastOnly = true;
//				} else {
//					nextTime = sensorStream.Keys [this.currentInd + 1];
//					pastOnly = false;
//				}
//				changeState (this.currentInd);
//			}
//		}
//	}

//    void changeState(int currentInt)
//    {
//        if (currentInt >= 0)
//        {
//            SensorReading currentSR = this.sensorStream[this.sensorStream.Keys[currentInt]];
//            float cTemp = currentSR.temp;
//            if (cTemp != -1)
//            {
//                float cSpHi = currentSR.spHi;
//                float cSpLo = currentSR.spLo;
//                if (cSpLo == -1)
//                {
//                    cSpLo = 22;
//                }
//                if (cSpHi == -1)
//                {
//                    cSpHi = cSpLo + 2;
//                }
//                if (cTemp <= cSpHi && cTemp >= cSpLo)
//                {
//                    //this.transform.parent.gameObject.GetComponent<Renderer>().material.color = Color.green;
//                    this.transform.parent.gameObject.GetComponent<SensorScript>().changeState(2);
//
//                }
//                else if (cTemp > cSpHi)
//                {
//                    //this.transform.parent.gameObject.GetComponent<Renderer>().material.color = Color.red;
//                    this.transform.parent.gameObject.GetComponent<SensorScript>().changeState(3);
//                }
//                else
//                {
//                    //this.transform.parent.gameObject.GetComponent<Renderer>().material.color = Color.blue;
//                    this.transform.parent.gameObject.GetComponent<SensorScript>().changeState(1);
//                }
//
//            }
//            else
//            {
//                this.transform.parent.gameObject.GetComponent<Renderer>().material.color = Color.gray;
//                this.transform.parent.gameObject.GetComponent<SensorScript>().changeState(0);
//            }
//        }
//    }

//	public void printSL(){
//		if (init) {
//			//foreach (KeyValuePair<DateTime, SensorReading> de in this.sensorStream) {
//			//	print (de.Key);
//			//}
//			DateTime date1 = new DateTime(2017, 1, 1, 22, 30, 52);
//			int i = FindFirstIndexGreaterThanOrEqualTo(this.sensorStream, date1);
//			if (i >= 0) {
//				print (this.sensorStream.Keys [i]);
//			}
//		}
//	}

//    public string getType()
//    {
//        if (this.typeSensor == Type.AC)
//        {
//            return "AC";
//        }
//        else
//        {
//            return "OAPU";
//        }
//    }
    

//	public void predictionMode(){
//		if (init) {
//			for (int i = 0; i < 5; i++) {
//				//Instantiate (this.transform.parent, transform.position + new Vector3 (0, (i + 1) * 0.005f, 0), transform.rotation, this.transform);
//				GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//				cube.transform.position = transform.position + new Vector3 (0, (i + 1) * 0.0025f, 0);
//				cube.transform.rotation = transform.rotation;
//				cube.transform.parent = this.transform;
//				cube.transform.localScale = new Vector3 (0.5f, 0.3f, 0.2f);
//				changePRState (cube, this.currentInd + i + 1);
//				//print (i);
//			}
//			for (int i = 0; i > -5; i--) {
//				//Instantiate (this.transform.parent, transform.position + new Vector3 (0, (i + 1) * 0.005f, 0), transform.rotation, this.transform);
//				GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//				cube.transform.position = transform.position + new Vector3 (0, (i - 1) * 0.0025f, 0);
//				cube.transform.rotation = transform.rotation;
//				cube.transform.parent = this.transform;
//				cube.transform.localScale = new Vector3 (0.5f, 0.3f, 0.2f);
//				changePRState (cube, this.currentInd + i - 1);
//				//print (i);
//			}
//		}
//	}


//	void changePRState (GameObject cube, int currentInt)
//	{
//		if (currentInt >= 0 && currentInt<this.sensorStream.Count) {
//			SensorReading currentSR = this.sensorStream [this.sensorStream.Keys [currentInt]];
//			float cTemp = currentSR.temp;
//			if (cTemp != -1) {
//				float cSpHi = currentSR.spHi;
//				float cSpLo = currentSR.spLo;
//				if (cSpLo == -1) {
//					cSpLo = 22;
//				}
//				if (cSpHi == -1) {
//					cSpHi = cSpLo + 2;
//				}
//				if (cTemp <= cSpHi && cTemp >= cSpLo) {
//					cube.GetComponent<Renderer> ().material.color = Color.green;
//				} else {
//					cube.GetComponent<Renderer> ().material.color = Color.red;
//				}
//
//			} else {
//				cube.GetComponent<Renderer> ().material.color = Color.blue;
//			}
//		}
//	}

//    public string getState()
//    {
//        if (init)
//        {
//            if (currentInd >= 0)
//            {
//                SensorReading currentSR = this.sensorStream[this.sensorStream.Keys[currentInd]];
//                string state = currentSR.status;
//                return state;
//            }
//            else
//            {
//                return "-";
//            }
//        } else
//        {
//            return "-";
//        }
//    }
//
//    public string getTempS()
//    {
//        if (init)
//        {
//            if (currentInd >= 0)
//            {
//                SensorReading currentSR = this.sensorStream[this.sensorStream.Keys[currentInd]];
//                float temp = currentSR.temp;
//                if (temp == -1)
//                {
//                    return "-";
//                }
//                return temp + "";
//            }
//            else
//            {
//                return "-";
//            }
//        }
//        else
//        {
//            return "-";
//        }
//    }
//
//    public string getSPLoS()
//    {
//        if (init)
//        {
//            if (currentInd >= 0)
//            {
//                SensorReading currentSR = this.sensorStream[this.sensorStream.Keys[currentInd]];
//                float splo = currentSR.spLo;
//                if (splo == -1)
//                {
//                    return "-";
//                }
//                return splo + "";
//            }
//            else
//            {
//                return "-";
//            }
//        }
//        else
//        {
//            return "-";
//        }
//    }
//
//    public string getSPHiS()
//    {
//        if (init)
//        {
//            if (currentInd >= 0)
//            {
//                SensorReading currentSR = this.sensorStream[this.sensorStream.Keys[currentInd]];
//                float sphi = currentSR.spHi;
//                if (sphi == -1)
//                {
//                    return "-";
//                }
//                return sphi + "";
//            }
//            else
//            {
//                return "-";
//            }
//        }
//        else
//        {
//            return "-";
//        }
//    }
}
