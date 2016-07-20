using UnityEngine;
using System.Collections;

public class Eraser : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void OnTriggerEnter(Collider other) {
		if (other.tag == "MovingEraser")
			gameObject.GetComponent<SpriteRenderer> ().sprite = null; 
	
	}
}
