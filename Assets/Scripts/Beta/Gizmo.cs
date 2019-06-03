using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gizmo : MonoBehaviour {

	public float gizmoSize = .75f;
	public Color gizmoColor = Color.yellow;

	void OnDrawGizmos()
	{
		// Draw a yellow sphere at the transform's position
		Gizmos.color = gizmoColor;
		Gizmos.DrawSphere(transform.position, gizmoSize);
	}
}
