using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectChanger : MonoBehaviour {

	public List<Sprite> objects;
	private int currentCount;
	public bool hasDailyAssociatedObject;
	public GameObject dailyAssociatedObject;
	private bool canChange;
	private int dayOfYear;
	public AudioSource audioEffect;
	private SpriteRenderer render;
	// Use this for initialization
	void Start () {
		
		render = GetComponent<SpriteRenderer> ();
		canChange = true;
		if (hasDailyAssociatedObject) {
			dayOfYear = PlayerPrefs.GetInt(gameObject.name+"doy",System.DateTime.Now.DayOfYear - 1 );
			int currentDayOfYear = System.DateTime.Now.DayOfYear;
			if(dayOfYear == currentCount + 1)
			{
				canChange = true;
			}
			else if (currentDayOfYear > dayOfYear) {
				canChange = true;
				//objects [0].SetActive (false);
			} else
				canChange = false;
		
		}
		currentCount = PlayerPrefs.GetInt(gameObject.name,0);

		for (int i = 0; i < objects.Count; i++) {
			if (i == currentCount) {
				render.sprite =  objects [i];
			}
		
		}

	
	}
	
	// Update is called once per frame
	void Update () {
		if (currentCount >= objects.Count) {
			currentCount = 0;

		}

		if (Input.GetMouseButtonDown(0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit)) {
				if (gameObject.name == hit.collider.name && canChange) {
					currentCount++;
					if (currentCount >= objects.Count) {
						currentCount = 0;

					}
					for (int i = 0; i < objects.Count; i++) {
						if (i == currentCount) {
							
							render.sprite =  objects [i];
							PlayerPrefs.SetInt(gameObject.name,currentCount);
							if (hasDailyAssociatedObject) {
								PlayerPrefs.SetInt(gameObject.name+"doy", System.DateTime.Now.DayOfYear);
								dailyAssociatedObject.SetActive (true);
								Invoke ("deleteAssociatedObject", 2);
								canChange = false;
								FindObjectOfType<AnalyticsSystem> ().CustomEvent("Water_Plant",new Dictionary<string, object>());
								FindObjectOfType<StickerHomeScreenBehaviour> ().setProgressionInGame ();

							}
							if(audioEffect && !audioEffect.isPlaying)
							{
								audioEffect.Play();
							}
							PlayerPrefs.Save ();
						}

					}


				}

			}
		}
	}

	private void deleteAssociatedObject()
	{
		dailyAssociatedObject.SetActive (false);
	}
}
