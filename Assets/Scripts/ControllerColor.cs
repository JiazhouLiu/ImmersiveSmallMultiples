using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ControllerColor : MonoBehaviour {
    public GameObject touchBarPrefab;
    public Material white;
    public Material red;
    public Material orange;
    public Material yellow;
    public Material green;
    public Material sky;
    public Material blue;
    public Material purple;

    public bool fixedPosition = false;

    SmallMultiplesManagerScript smms;

    GameObject touchbar;

    private bool touchBarFlag = false;
    private bool triggerColorFlag = false;

    private void Start()
    {
        smms = GameObject.Find("SmallMultiplesManager").GetComponent<SmallMultiplesManagerScript>();
    }

    // Update is called once per frame
    void Update () {

        //if (transform.Find("tip") != null)
        //{
        //    if (transform.Find("tip").GetChild(0) != null)
        //    {
        //        if (transform.Find("tip").GetChild(0).childCount == 0)
        //        {
        //            touchbar = Instantiate(touchBarPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        //            touchbar.transform.SetParent(transform.Find("tip").GetChild(0));
        //            touchbar.transform.localPosition = new Vector3(0, -0.0172f, 0.0602f);
        //            touchbar.transform.localEulerAngles = new Vector3(90, 0, 0);

        //        }
        //    }
        //}

        if (!touchBarFlag)
        {
            if (transform.Find("tip") != null)
            {
                if (transform.Find("tip").GetChild(0) != null)
                {
                    if (transform.Find("tip").GetChild(0).childCount == 0)
                    {
                        touchbar = Instantiate(touchBarPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                        touchbar.transform.SetParent(transform.Find("tip").GetChild(0));
                        touchbar.transform.localPosition = new Vector3(0, 0, 0.1f);
                        touchbar.transform.localEulerAngles = new Vector3(90, 0, 0);

                    }
                }
            }

            if (touchbar != null)
            {
                if (this.transform.parent.name == "Controller (left)")
                {
                    touchbar.GetComponent<Renderer>().material.color = Color.blue;
                }
                else
                {
                    touchbar.GetComponent<Renderer>().material.color = Color.green;
                }
                touchBarFlag = true;
            }
        }



        if (this.transform.childCount > 0 && !triggerColorFlag)
        {
            //GameObject body = this.transform.GetChild(0).gameObject;
            //GameObject grip = this.transform.GetChild(1).gameObject;
            //GameObject menu = this.transform.GetChild(2).gameObject;
            //GameObject thumbstick = this.transform.GetChild(3).gameObject;
            //GameObject trackpad = this.transform.GetChild(5).gameObject;
            //GameObject trigger = this.transform.GetChild(6).gameObject;

            //if (fixedPosition)
            //{
            //    body.GetComponent<MeshRenderer>().material = white;
            //    grip.GetComponent<MeshRenderer>().material = white;
            //    thumbstick.GetComponent<MeshRenderer>().material = white;
            //    trigger.GetComponent<MeshRenderer>().material = red;
            //    if (this.transform.parent.name == "Controller (left)")
            //    {
            //        trackpad.GetComponent<MeshRenderer>().material = green;
            //        menu.GetComponent<MeshRenderer>().material = orange;
            //    }
            //    else
            //    {
            //        if (smms.dataset == 2)
            //        {
            //            trackpad.GetComponent<MeshRenderer>().material = white;
            //        }
            //        else
            //        {
            //            trackpad.GetComponent<MeshRenderer>().material = white;
            //        }
            //        menu.GetComponent<MeshRenderer>().material = white;
            //    }
            //}
            //else
            //{
            //    body.GetComponent<MeshRenderer>().material = white;
            //    grip.GetComponent<MeshRenderer>().material = white;
            //    thumbstick.GetComponent<MeshRenderer>().material = white;
            //    trigger.GetComponent<MeshRenderer>().material = red;

            //    if (this.transform.parent.name == "Controller (left)")
            //    {
            //        menu.GetComponent<MeshRenderer>().material = orange;
            //        trackpad.GetComponent<MeshRenderer>().material = green;
            //    }
            //    else
            //    {
            //        menu.GetComponent<MeshRenderer>().material = white;
            //        trackpad.GetComponent<MeshRenderer>().material = white;
            //    }
            //}

            GameObject body = this.transform.Find("body").gameObject;
            //GameObject lgrip = this.transform.Find("lgrip").gameObject;
            //GameObject rgrip = this.transform.Find("rgrip").gameObject;
            //GameObject menu = this.transform.Find("button").gameObject;
            ////GameObject thumbstick = this.transform.GetChild(3).gameObject;
            GameObject trackpad = this.transform.Find("trackpad").gameObject;
            GameObject trigger = this.transform.Find("trigger").gameObject;
            //GameObject sysButton = this.transform.Find("sys_button").gameObject;
            GameObject lrLabel = (GameObject)Instantiate(smms.LRLabelPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            lrLabel.transform.SetParent(body.transform);
            lrLabel.transform.localPosition = new Vector3(0, 0.006f, -0.1103f);
            lrLabel.transform.localEulerAngles = new Vector3(90, 0, 0);
            lrLabel.transform.localScale = Vector3.one * 0.01f;
            if (this.transform.parent.name == "Controller (left)")
            {
                lrLabel.GetComponent<TextMeshPro>().text = "L";
            }
            else
            {
                lrLabel.GetComponent<TextMeshPro>().text = "R";
            }

            //body.GetComponent<MeshRenderer>().material = white;
            //lgrip.GetComponent<MeshRenderer>().material = white;
            //lgrip.GetComponent<MeshRenderer>().material = white;
            //sysButton.GetComponent<MeshRenderer>().material = white;
            trigger.GetComponent<MeshRenderer>().material = red;
            trackpad.GetComponent<MeshRenderer>().material = green;
            //menu.GetComponent<MeshRenderer>().material = white;

            triggerColorFlag = true;
        }
	}
}
