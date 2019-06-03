using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bezier3PointCurveMesh : MonoBehaviour {

	public Transform point1;
	public Transform point2;
	public Transform point3;
	public float meshWidth;
	public int vertexCount = 50;
	
	Mesh mesh;

	
	void Awake(){
		mesh = GetComponent<MeshFilter>().mesh;
	}
	
	// Use this for initialization
	void Start () {
        DrawMesh(CalculateCurvePoint());
	}
	
	// Update is called once per frame
	void Update () {
        DrawMesh(CalculateCurvePoint());
	}

    private void DrawMesh(Vector3[] curveVerts)
    {
        Vector3[] verticies = new Vector3[curveVerts.Length];

        for (int i = 0; i < verticies.Length; i++)
        {
            verticies[i] = curveVerts[i];
        }

        int[] triangles = new int[((curveVerts.Length / 2) - 1) * 6];

        //Works on linear patterns tn = bn+c
        int position = 6;
        for (int i = 0; i < (triangles.Length / 6); i++)
        {
            triangles[i * position] = 2 * i;
            triangles[i * position + 3] = 2 * i;

            triangles[i * position + 1] = 2 * i + 3;
            triangles[i * position + 4] = (2 * i + 3) - 1;

            triangles[i * position + 2] = 2 * i + 1;
            triangles[i * position + 5] = (2 * i + 1) + 2;
        }


        mesh.Clear();
        mesh.vertices = verticies;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    void MakeMesh(Vector3[] curveVerts){
		mesh.Clear();
		Vector3[] vertices = new Vector3[(vertexCount + 1) * 2];
		int[] triangles = new int[vertexCount * 6 * 2];
		
		for(int i = 0; i <= vertexCount; i++){
			// set vertices
			vertices[i * 2] = new Vector3(meshWidth * 0.5f, curveVerts[i].y, curveVerts[i].x);
			vertices[i * 2 + 1] = new Vector3(meshWidth * -0.5f, curveVerts[i].y, curveVerts[i].x);
			
			// set triangles
			if(i != vertexCount){
				triangles[i * 12] = i * 2;
				triangles[i * 12 + 1] = triangles[i * 12 + 4] = i * 2 + 1;
				triangles[i * 12 + 2] = triangles[i * 12 + 3] = (i + 1) * 2;
				triangles[i * 12 + 5] = (i + 1) * 2 + 1;
				
				triangles[i * 12 + 6] = i * 2;
				triangles[i * 12 + 7] = triangles[i * 12 + 10] = (i + 1) * 2;
				triangles[i * 12 + 8] = triangles[i * 12 + 9] = i * 2 + 1;
				triangles[i * 12 + 11] = (i + 1) * 2 + 1;
			}
			
			mesh.vertices = vertices;
			mesh.triangles = triangles;
		}
	}
	
	Vector3[] CalculateCurvePoint(){
		var pointList = new List<Vector3> ();

		for (float ratio = 0; ratio <= 1; ratio += 1.0f / vertexCount) {
			var tangentLineVertex1 = Vector3.Lerp(point1.position, point2.position, ratio); 
			var tangentLineVertex2 = Vector3.Lerp(point2.position, point3.position, ratio);
			var bezierPoint = Vector3.Lerp (tangentLineVertex1, tangentLineVertex2, ratio);
			pointList.Add (bezierPoint);
		}
		
		return pointList.ToArray ();
	}
}
