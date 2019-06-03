using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatRuler : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.localPosition = new Vector3(0,0,0.2f);
        this.transform.localRotation = Quaternion.identity;
    }
}
