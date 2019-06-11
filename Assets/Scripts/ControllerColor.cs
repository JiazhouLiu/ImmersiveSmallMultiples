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
            GameObject body = this.transform.Find("body").gameObject;
            GameObject trackpad = this.transform.Find("trackpad").gameObject;
            GameObject trigger = this.transform.Find("trigger").gameObject;
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
            trigger.GetComponent<MeshRenderer>().material = red;
            trackpad.GetComponent<MeshRenderer>().material = green;

            triggerColorFlag = true;
        }
	}
}
