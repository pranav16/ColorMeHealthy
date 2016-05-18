using UnityEngine;
using System.Collections;

public class Graph : MonoBehaviour {

	public GameObject point1, point2, arrow;
	// Use this for initialization
	void Start () {
	
	}

	
	// Update is called once per frame
	void Update () {
	 
		orientBetweenTwoPoints (arrow, point1.transform.position, point2.transform.position);
	}

	void orientBetweenTwoPoints(GameObject objectToOrient,Vector3 pointA ,Vector3 pointB)
	{
		Vector3 midPoint = (pointA + pointB) / 2;
		objectToOrient.transform.position = midPoint;
		objectToOrient.transform.LookAt(pointB);
		Quaternion roation = new Quaternion(objectToOrient.transform.rotation.x ,0,objectToOrient.transform.rotation.z,1);
		objectToOrient.transform.rotation = roation;
	}
}
