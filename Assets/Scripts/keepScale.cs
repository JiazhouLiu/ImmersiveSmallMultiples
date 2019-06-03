using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keepScale : MonoBehaviour {


    //GameObject MultipleManager;
    SmallMultiplesManagerScript mrs;

    private void Start()
    {
        //MultipleManager = GameObject.Find("SmallMultiplesManager");
        mrs = GameObject.Find("SmallMultiplesManager").GetComponent<SmallMultiplesManagerScript>();
    }

    // Update is called once per frame
    void Update () {

        if (!this.name.Equals("RadialMenu"))
        {
            if (this.transform.parent.gameObject.name.Equals("LeftController"))
            {
                this.transform.parent.localScale = Vector3.one;

                //if (!mrs.indirectTouch)
                //{
                //    this.transform.parent.GetChild(1).localScale = new Vector3(0.002f, 0.002f, 0.002f);
                //}

                //this.transform.GetChild(0).gameObject.SetActive(true);
                //this.transform.GetChild(1).gameObject.SetActive(true);
                //this.transform.GetChild(2).gameObject.SetActive(true);
                //this.transform.GetChild(4).gameObject.SetActive(true);


                //if (mrs.indirectTouch)
                //{
                //    this.transform.parent.localScale = Vector3.one;
                //}
                //else
                //{
                //    this.transform.parent.GetChild(1).localScale = Vector3.one;
                //}
                //if (!mrs.indirectTouch)
                //{
                //    ////this.transform.parent.GetChild(1).GetChild(1).localScale = new Vector3(0.002f, 0.002f, 0.002f);
                //}


            }
            if (this.transform.parent.gameObject.name.Equals("RightController"))
            {

                this.transform.parent.localScale = Vector3.one;

                //if (!mrs.indirectTouch)
                //{
                //    //this.transform.parent.GetChild(2).GetChild(1).localScale = new Vector3(0.002f, 0.002f, 0.002f);
                //}
                //this.transform.GetChild(0).gameObject.SetActive(true);
                //this.transform.GetChild(1).gameObject.SetActive(true);
                //this.transform.GetChild(2).gameObject.SetActive(true);
                //this.transform.GetChild(4).gameObject.SetActive(true);


                //if (mrs.indirectTouch)
                //{
                //    this.transform.parent.GetChild(1).localScale = Vector3.one;
                //}
                //else
                //{
                //    this.transform.parent.GetChild(2).localScale = Vector3.one;
                //}

            }
        }

        //if (this.transform.parent.localScale.x != 1)
        //{
        //    Vector3 parentScale = this.transform.parent.localScale;
        //    if (this.transform.parent.name.Equals("LeftController")) {
        //        Debug.Log(this.transform.parent.localScale.x);
        //    }
            
        //    this.transform.localScale = new Vector3(1 / parentScale.x, 1 / parentScale.y, 1 / parentScale.z);

        //    if (this.name.Equals("RadialMenu"))
        //    {
                
        //        //this.transform.localScale = new Vector3(1 / parentScale.x, 1 / parentScale.y, 1 / parentScale.z);
        //        RectTransform rt = transform.GetChild(0).gameObject.GetComponent<RectTransform>();
        //        rt.localScale = new Vector3(1 / parentScale.x * 0.0002f, 1 / parentScale.y * 0.0002f, 1 / parentScale.z * 0.0002f);
        //        //rt.localPosition = new Vector3(0, -0.05f, -0.01f);
        //    }
        //}
        //else
        //{
        //    //this.transform.localScale = new Vector3(1, 1, 1);
        //    if (this.name.Equals("RadialMenu"))
        //    {
        //        RectTransform rt = transform.GetChild(0).gameObject.GetComponent<RectTransform>();
        //        rt.localPosition = new Vector3(0, 0, 0);
        //    }
        //}
        
	}
}
