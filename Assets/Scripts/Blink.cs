using UnityEngine;
using System.Collections;

public class Blink : MonoBehaviour {

	public float Speed;
	public float maxDurationForBlink;
	public string triggerKeyForAnimation;
	private float currentTimeElapsed;

	// Use this for initialization
	void Start () {
		currentTimeElapsed = 0;
	}
	
	// Update is called once per frame
	void Update () {

		currentTimeElapsed += Time.deltaTime * Speed;
	
		if (currentTimeElapsed >= maxDurationForBlink) 
		{
			//Debug.Log (currentTimeElapsed);
			GetComponent<Animator> ().SetTrigger (triggerKeyForAnimation);
			currentTimeElapsed = 0;
			Debug.Log ("Blink");
		}

	
	}
}
