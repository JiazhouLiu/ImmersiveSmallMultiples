using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountDownTimer : MonoBehaviour {

    private bool startCount = false;
    public float countTimer = 60.0f;
    SmallMultiplesManagerScript smms = null;
	// Use this for initialization
	void Start () {
        
        if (GameObject.Find("SmallMultiplesManager") != null) {
            smms = GameObject.Find("SmallMultiplesManager").GetComponent<SmallMultiplesManagerScript>();
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (countTimer >= 0 && startCount)
        {
            countTimer -= Time.deltaTime;

            TextMeshProUGUI tmp = transform.GetChild(0).Find("CountText").GetComponent<TextMeshProUGUI>();
            tmp.text = countTimer.ToString("F0") + " s";
            if (countTimer < 10.5f)
            {
                tmp.color = Color.red;
            }
            else
            {
                tmp.color = Color.white;
            }
        }
        if (countTimer < 0.5f && smms != null) {
            smms.FinishOrTimeUpToAnswer();
        }
    }

    public void StartTimer() {
        startCount = true;
    }

    public void ResetTimer() {
        countTimer = 60f;
        TextMeshProUGUI tmp = transform.GetChild(0).Find("CountText").GetComponent<TextMeshProUGUI>();
        tmp.text = "60 s";
        tmp.color = Color.white;
        startCount = false;
    }
}
