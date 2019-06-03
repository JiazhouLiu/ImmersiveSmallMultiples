using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSelectionCollisionDetection : MonoBehaviour {

    List<GameObject> collidedSensors;
    SmallMultiplesManagerScript smms;

	// Use this for initialization
	void Start () {
        smms = GameObject.Find("SmallMultiplesManager").GetComponent<SmallMultiplesManagerScript>();
        collidedSensors = new List<GameObject>();
    }
	
	// Update is called once per frame
	void Update () {
        if (smms.dataset == 1) {
            smms.CubeHoveringBIM(collidedSensors);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (smms.dataset == 1)
        {
            if (other.name.Contains("ACG"))
            {
                if (int.Parse(other.name.Substring(4)) <= 4 && int.Parse(other.name.Substring(4)) >= 1)
                {
                    collidedSensors.Add(other.gameObject);
                }
            }
            else if (other.name.Contains("AC1"))
            {
                if ((int.Parse(other.name.Substring(4)) <= 9 && int.Parse(other.name.Substring(4)) >= 3) || int.Parse(other.name.Substring(4)) == 12)
                {
                    collidedSensors.Add(other.gameObject);
                }
            }
        }
        
    }

    private void OnTriggerStay(Collider other)
    {
    }

    private void OnTriggerExit(Collider other)
    {
        if (smms.dataset == 1) {
            if (collidedSensors.Contains(other.gameObject))
            {
                collidedSensors.Remove(other.gameObject);
            }
        }
            
    }
}
