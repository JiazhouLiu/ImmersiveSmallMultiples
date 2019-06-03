using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameLabel : MonoBehaviour {

	void Update () {
		string name = this.transform.parent.name;
		GameObject nameLabel = GameObject.Find ("nameLabel" + name);
		Vector3 namePos = Camera.main.WorldToScreenPoint (this.transform.position);
		nameLabel.transform.position = namePos;

	}
}