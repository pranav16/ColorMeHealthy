using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class SpeechBubble : MonoBehaviour {

	public GameObject speechBubble;
	public GameObject currentSpeechText;
	public List<Sprite>speechText;


	// Use this for initialization
	void Start () {
	
		initialize ();
	}

	void initialize()
	{

		if (FindObjectOfType<AnalyticsSystem> ().getCounterValue ("Fill_Symptoms") > 0) {

			int index = Random.Range (1, speechText.Count);
			//just in case you have one element in the list
			if (index < speechText.Count)
				currentSpeechText.GetComponent<SpriteRenderer>().sprite = speechText[index];
		}
		Invoke ("startScaleDown",4);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void startScaleDown()
	{
		StartCoroutine ("scaleSpeechBoxDown",4);
	}

	IEnumerator scaleSpeechBoxDown(float time)
	{

		Vector3 originalScale = speechBubble.transform.localScale;
		Vector3 destinationScale = new Vector3(0.0f, 0.0f, 0.0f);

		float originalTime = time;

		while(time > 0.0f)
		{
			time -= Time.deltaTime;
			speechBubble.transform.localScale = Vector3.Lerp(destinationScale,originalScale,time/originalTime);

			yield return null;
		} 

	}
	IEnumerator scaleSpeechBoxUp(float time)
	{
		Vector3 originalScale = speechBubble.transform.localScale;
		Vector3 destinationScale = new Vector3 (1.7f, 1.3f, 0.0f);

		float originalTime = time;

		while (time > 0.0f) {
			time -= Time.deltaTime;
			speechBubble.transform.localScale = Vector3.Lerp (destinationScale, originalScale, time / originalTime);

			yield return null;
		} 
	}
}
