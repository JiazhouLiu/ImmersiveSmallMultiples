using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using VRTK;
using TMPro;

public class SensorScript : MonoBehaviour
{
    private Material silhouetteShader;

    //    public GameObject tag;
    //    public Text textSensor;
    //	  public Camera cam;
    //    TextMesh nameT;
    //    TextMesh temp;
    //    TextMesh state;
    //    TextMesh sp;
    //	  GameObject tempBlue;
    //	  GameObject tempRed;
    //	  GameObject tempGreen;
    //	  int stateTemp;

    private GameObject ToolTipPrefab;
    private GameObject TextPrefab;

    private SensorInfo si;
    private BuildingScript bs;

    private GameObject tooltip;
    private GameObject textTooltip;

    private bool hasInfo = false;
    private bool initiated = false;

    public int sensorColorIndex = 0;

    // Use this for initialization
    void Start()
    {
        

        SmallMultiplesManagerScript smms = GameObject.Find("SmallMultiplesManager").GetComponent<SmallMultiplesManagerScript>();
        ToolTipPrefab = smms.tooltipPrefab;
        TextPrefab = smms.TextHolderPrefab;
        silhouetteShader = smms.silhouetteShader;

        bs = this.transform.parent.parent.parent.GetComponent<BuildingScript>();

        si = transform.Find("sensorInfo").gameObject.GetComponent<SensorInfo>();
        ReadDataFromManager();
        if (hasInfo)
        {
            //si.getMonthlySensorReadings ();
            if (smms.dateType == DateType.Monthly)
            {
                si.getMonthlySensorReadings();
            }
            if (smms.dateType == DateType.Fornightly)
            {
                si.getSensorReadings(smms.smallMultiplesNumber, 14);
            }
            if (smms.dateType == DateType.Weekly)
            {
                si.getSensorReadings(smms.smallMultiplesNumber, 7);
            }
            ChangeColor();
            SendTempTag();
        }
        else
        {
            Color lightGray = new Color();
            ColorUtility.TryParseHtmlString("#969696", out lightGray);
            this.GetComponent<Renderer>().material.color = lightGray;
            this.gameObject.SetActive(false);
            this.transform.localScale = new Vector3(0.6f, 0.1f, 0.6f);
            this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y - 1, this.transform.localPosition.z);
            //Debug.Log(this.transform.localPosition);

        }
        transform.localScale *= 2f;
        transform.localPosition += Vector3.up * transform.localScale.y;
        transform.localEulerAngles = Vector3.zero;
    }

    private void Awake()
    {
        if (hasInfo)
        {
            ChangeColor();
        }
    }

    public int GetSensorColorIndex() {
        return this.sensorColorIndex;
    }

    void SendTempTag()
    {
        if (si.tempTag != null)
        {
            SmallMultiplesManagerScript smms = GameObject.Find("SmallMultiplesManager").GetComponent<SmallMultiplesManagerScript>();
            smms.AssignTempTag(si.tempTag);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (hasInfo && initiated)
        {
            Transform textT = textTooltip.transform;
            Transform headset = VRTK_DeviceFinder.HeadsetTransform();
            textT.LookAt(headset);
            textT.localEulerAngles = new Vector3(0, textT.localEulerAngles.y, textT.localEulerAngles.z);
            if (bs.IsExploded())
            {
                BuildingScript bs = this.transform.parent.parent.parent.gameObject.GetComponent<BuildingScript>();
                //if (bs.IsMagnified ()) {
                //tooltip.SetActive (true);
                //} else {
                //tooltip.SetActive (false);
                //}




                //Camera realCamera = Camera.main;
                //if (realCamera != null)
                //{
                //    Vector3 viewPos = realCamera.WorldToViewportPoint(this.transform.position);
                //    if (viewPos.x > 0 && viewPos.y > 0 && viewPos.z > 0)
                //    {
                //        tooltip.SetActive(true);
                //    }
                //    else {
                //        tooltip.SetActive(false);
                //    }
                //}
            }
            else
            {
                //tooltip.SetActive(false);
            }


        }
    }

    void ReadDataFromManager()
    {
        HashSet<SensorReading> sensorHS;
        if (SmallMultiplesManagerScript.sensorsInfoList.TryGetValue(this.name.Trim(), out sensorHS))
        {
            hasInfo = true;
            foreach (SensorReading sr in sensorHS)
            {
                si.sendDataAC(sr.dt, sr.temp, sr.spHi, sr.spLo, sr.status, sr.acUnit, sr.roofTemp);
            }
        }
    }

    // calculate average temperature and change sensor coloring
    public void ChangeColor()
    {
        int buildingNumber = Int32.Parse(this.transform.parent.parent.parent.parent.name.Remove(0, 15));
        this.GetComponent<Renderer>().material = silhouetteShader;
        if (si.tempAvg.Length > 0)
        { // if has sensor information for the current sensor
            float tempValue = si.tempAvg[buildingNumber - 1];

            //tooltip = (GameObject)Instantiate(ToolTipPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            //tooltip.transform.SetParent(this.transform);

            //tooltip.name = this.name.Trim() + " tooltip";
            //VRTK_ObjectTooltip ot = tooltip.GetComponent<VRTK_ObjectTooltip>();
            //ot.displayText = this.name.Trim();

            if (tempValue >= 27)
            {
                Color cl27 = new Color();
                ColorUtility.TryParseHtmlString("#d73027", out cl27); //d73027
                this.GetComponent<Renderer>().material.color = cl27;
                this.GetComponent<Renderer>().material.SetColor("_OutlineColor", cl27);

                this.transform.localScale = new Vector3(0.6f, 1f, 0.6f);
                //this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y - 0.1f, this.transform.localPosition.z);
                //tooltip.transform.localPosition = new Vector3(0, 1.6f, 0);
                sensorColorIndex = 9;

            }
            else if (tempValue >= 26 & tempValue < 27)
            {
                Color cl26 = new Color();
                ColorUtility.TryParseHtmlString("#f46d43", out cl26); //f46d43
                this.GetComponent<Renderer>().material.color = cl26;
                this.GetComponent<Renderer>().material.SetColor("_OutlineColor", cl26);

                this.transform.localScale = new Vector3(0.6f, 0.9f, 0.6f);
                //this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y - 0.2f, this.transform.localPosition.z);
                //tooltip.transform.localPosition = new Vector3(0, 1.8f, 0);
                sensorColorIndex = 8;
            }
            else if (tempValue >= 25 & tempValue < 26)
            {
                Color cl25 = new Color();
                ColorUtility.TryParseHtmlString("#fdae61", out cl25); //ffc800
                this.GetComponent<Renderer>().material.color = cl25;
                this.GetComponent<Renderer>().material.SetColor("_OutlineColor", cl25);


                this.transform.localScale = new Vector3(0.6f, 0.8f, 0.6f);
                //this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y - 0.3f, this.transform.localPosition.z);
                //tooltip.transform.localPosition = new Vector3(0, 2.0f, 0);
                sensorColorIndex = 7;
            }
            else if (tempValue >= 24 & tempValue < 25)
            {
                Color cl24 = new Color();
                ColorUtility.TryParseHtmlString("#fee090", out cl24); //ffffbf
                this.GetComponent<Renderer>().material.color = cl24;
                this.GetComponent<Renderer>().material.SetColor("_OutlineColor", cl24);

                this.transform.localScale = new Vector3(0.6f, 0.7f, 0.6f);
                //this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y - 0.4f, this.transform.localPosition.z);
                //tooltip.transform.localPosition = new Vector3(0, 2.2f, 0);
                sensorColorIndex = 6;
            }
            else if (tempValue >= 23 & tempValue < 24)
            {
                Color cl23 = new Color();
                ColorUtility.TryParseHtmlString("#ffffbf", out cl23); //8aff00
                this.GetComponent<Renderer>().material.color = cl23;
                this.GetComponent<Renderer>().material.SetColor("_OutlineColor", cl23);

                this.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                //this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y - 0.5f, this.transform.localPosition.z);
                //tooltip.transform.localPosition = new Vector3(0, 2.4f, 0);
                sensorColorIndex = 5;
            }
            else if (tempValue >= 22 & tempValue < 23)
            {
                Color cl22 = new Color();
                ColorUtility.TryParseHtmlString("#e0f3f8", out cl22); //1a9850
                this.GetComponent<Renderer>().material.color = cl22;
                this.GetComponent<Renderer>().material.SetColor("_OutlineColor", cl22);

                this.transform.localScale = new Vector3(0.6f, 0.5f, 0.6f);
                //this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y - 0.6f, this.transform.localPosition.z);
                //tooltip.transform.localPosition = new Vector3(0, 2.6f, 0);
                sensorColorIndex = 4;
            }
            else if (tempValue >= 21 & tempValue < 22)
            {
                Color cl21 = new Color();
                ColorUtility.TryParseHtmlString("#abd9e9", out cl21); //00fff4
                this.GetComponent<Renderer>().material.color = cl21;
                this.GetComponent<Renderer>().material.SetColor("_OutlineColor", cl21);

                this.transform.localScale = new Vector3(0.6f, 0.4f, 0.6f);
                //this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y - 0.7f, this.transform.localPosition.z);
                //tooltip.transform.localPosition = new Vector3(0, 2.8f, 0);
                sensorColorIndex = 3;
            }
            else if (tempValue >= 20 & tempValue < 21)
            {
                Color cl20 = new Color();
                ColorUtility.TryParseHtmlString("#74add1", out cl20); //0064ff
                this.GetComponent<Renderer>().material.color = cl20;
                this.GetComponent<Renderer>().material.SetColor("_OutlineColor", cl20);

                this.transform.localScale = new Vector3(0.6f, 0.3f, 0.6f);
                //this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y - 0.8f, this.transform.localPosition.z);
                //tooltip.transform.localPosition = new Vector3(0, 3f, 0);
                sensorColorIndex = 2;
            }
            else
            {
                Color cl19 = new Color();
                ColorUtility.TryParseHtmlString("#4575b4", out cl19); //542788
                this.GetComponent<Renderer>().material.color = cl19;
                this.GetComponent<Renderer>().material.SetColor("_OutlineColor", cl19);

                this.transform.localScale = new Vector3(0.6f, 0.2f, 0.6f);
                //this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y - 0.9f, this.transform.localPosition.z);
                //tooltip.transform.localPosition = new Vector3(0, 3.2f, 0);
                sensorColorIndex = 1;
            }

            //tooltip.transform.localScale = new Vector3(30, 20f / this.transform.localScale.y, 30);
            //this.GetComponent<Renderer>().material.shader = silhouetteShader;
            //this.GetComponent<Renderer>().material.SetFloat("_Outline", 0.0001f);

            
            //this.GetComponent<Renderer>().material.SetFloat("_Outline", 0.0001f);
        }
        else
        { // no info snesor
            Color lightGray = new Color();
            ColorUtility.TryParseHtmlString("#969696", out lightGray);
            //			Color blueSensor = new Color ();
            //			ColorUtility.TryParseHtmlString ("#1d91c0", out blueSensor);
            this.GetComponent<Renderer>().material.color = lightGray;

            this.transform.localScale = new Vector3(0.6f, 0.1f, 0.6f);
            this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y + 1f, this.transform.localPosition.z);
            //Debug.Log(this.transform.localPosition);
        }
        initiated = true;

        textTooltip = (GameObject)Instantiate(TextPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        textTooltip.transform.SetParent(this.transform);
        textTooltip.transform.localPosition = Vector3.zero;
        textTooltip.transform.localScale = new Vector3(0.5f / this.transform.localScale.x, 0.5f / this.transform.localScale.y, 0.5f / this.transform.localScale.z);

        GameObject textT = textTooltip.transform.GetChild(0).gameObject;
        textT.transform.localPosition = new Vector3(0, 0, 1);
        TextMeshPro tm = textT.GetComponent<TextMeshPro>();

        if (name[2] == 'G')
        {
            tm.text = intToChar(int.Parse(name.Trim().Substring(4)));
        }
        else if (name[2] == '1') {
            if (int.Parse(name.Trim().Substring(4)) != 12)
            {
                tm.text = intToChar(int.Parse(name.Trim().Substring(4)) + 2);
            }
            else {
                tm.text = intToChar(int.Parse(name.Trim().Substring(4)));
            }
            
        }
        
        tm.color = Color.white;


        if (this.transform.localScale.y == 0.2f || this.transform.localScale.y == 0.3f)
        {
            textT.transform.localPosition = new Vector3(0, 0.4f, 1);
        }
    }

    private string intToChar(int num) {
        switch (num) {
            case 1:
                return "A";
            case 2:
                return "B";
            case 3:
                return "C";
            case 4:
                return "D";
            case 5:
                return "E";
            case 6:
                return "F";
            case 7:
                return "G";
            case 8:
                return "H";
            case 9:
                return "I";
            case 10:
                return "J";
            case 11:
                return "K";
            case 12:
                return "L";
            default:
                return "";
        }
    }


    //    public void OnFocusEnter()
    //    {
    //        //tag.transform.position = transform.position + new Vector3(0,0.05f,0);
    //        //var n = cam.transform.position - tag.transform.position;
    //        //tag.transform.rotation = Quaternion.LookRotation(n)*Quaternion.Euler(0,180,0);
    //        SensorInfo si = transform.Find("sensorInfo").gameObject.GetComponent<SensorInfo>();
    //        //nameT.text = this.name;
    //        //state.text = "State: "+si.getState();
    //        //temp.text = "Temp: "+si.getTempS();
    //        //sp.text = "SP: " + si.getSPLoS() + "-" + si.getSPHiS();
    //        string entireText = this.name+"\n"+ "State: " + si.getState() + "\n" + "Temp: " + si.getTempS() + "\n" + "SP: " + si.getSPLoS() + "-" + si.getSPHiS();
    //        textSensor.text = entireText;
    //        //tag.SetActive(true); 
    //
    //    }
    //
    //    public void OnFocusExit()
    //    {
    //        //tag.SetActive(false);
    //        textSensor.text = "";
    //    }
    //


    //    // Use this for initialization
    //    void Start () {
    //        //nameT = tag.transform.Find("name").gameObject.GetComponent<TextMesh>();
    //        //temp = tag.transform.Find("temp").gameObject.GetComponent<TextMesh>();
    //        //state = tag.transform.Find("state").gameObject.GetComponent<TextMesh>();
    //        //sp = tag.transform.Find("sp").gameObject.GetComponent<TextMesh>();
    //        si = transform.Find("sensorInfo").gameObject.GetComponent<SensorInfo>();
    //        if (si.getType() == "AC")
    //        {
    //            /*GameObject tempBlue = Resources.Load("logoBlue") as GameObject;
    //            if(tempBlue == null)
    //            {
    //                print("TempBlue is Null");
    //            }
    //            GameObject tempLogo = Instantiate(tempBlue);
    //            tempLogo.transform.position = transform.position;
    //            tempLogo.transform.parent = transform;
    //            tempLogo.transform.Rotate(new Vector3(90f,0f,0f));
    //            tempLogo.transform.localScale = new Vector3(0.7f,0.05f,0.7f);            //tempLogo.transform.position
    //            tempLogo.transform.Translate(new Vector3(0f,0f,-0.03f));*/
    //            //initTempLogo(ref tempBlue, "logoBlue");
    //            //initTempLogo(ref tempRed, "logoRed");
    //            //initTempLogo(ref tempGreen, "logoGreen");
    //            stateTemp = 0;
    //            //tempGreen.SetActive(true);
    //        }
    //    }

    //    void initTempLogo(ref GameObject temp, string name)
    //    {
    //        /*GameObject tempP = Resources.Load(name) as GameObject;
    //        temp = Instantiate(tempP);
    //        temp.transform.position = transform.position;
    //        temp.transform.parent = transform;
    //        temp.transform.Rotate(new Vector3(90f, 0f, 0f));
    //        temp.transform.localScale = new Vector3(0.7f, 0.05f, 0.7f);            //tempLogo.transform.position
    //        temp.transform.Translate(new Vector3(0f, 0f, -0.03f));
    //        temp.SetActive(false);*/
    //    }

    //    public void changeState(int newStateTemp)
    //    {
    //        /*if (newStateTemp != this.stateTemp)
    //        {
    //            this.stateTemp = newStateTemp;
    //            switch (this.stateTemp)
    //            {
    //                case 0:
    //                    tempBlue.SetActive(false);
    //                    tempRed.SetActive(false);
    //                    tempGreen.SetActive(false);
    //                    break;
    //                case 1:
    //                    tempBlue.SetActive(true);
    //                    tempRed.SetActive(false);
    //                    tempGreen.SetActive(false);
    //                    break;
    //                case 2:
    //                    tempBlue.SetActive(false);
    //                    tempRed.SetActive(false);
    //                    tempGreen.SetActive(true);
    //                    break;
    //                case 3:
    //                    tempBlue.SetActive(false);
    //                    tempRed.SetActive(true);
    //                    tempGreen.SetActive(false);
    //                    break;
    //            }
    //        }*/
    //    }
}
