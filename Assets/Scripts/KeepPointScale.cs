using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepPointScale : MonoBehaviour {

	// Update is called once per frame
	void Update () {
        if (this.name.Equals("RightController")) {
            Transform bprc = this.transform.Find("[VRTK][AUTOGEN][RightController][BasePointerRenderer_ObjectInteractor_Container]");

            if (bprc != null) {
                bprc.localScale = Vector3.one;
            }
        }
        if (this.name.Equals("LeftController"))
        {
            Transform bprc = this.transform.Find("[VRTK][AUTOGEN][LeftController][BasePointerRenderer_ObjectInteractor_Container]");

            if (bprc != null)
            {
                bprc.localScale = Vector3.one;
            }
        }

    }
}
