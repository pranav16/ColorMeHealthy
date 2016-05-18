using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;

public class Customisation : MonoBehaviour {

	public List<GameObject> m_customisableParts;

	private int currentCount;
	private int nextCount;
	private GameObject currentActiveObject;
	private ColorObjectHandler state;
	//added because sometimes the Start gets a delayed call
	public bool isReadyToReadSavedData;

	// Use this for initialization
	void Start () {
		Analytics.CustomEvent ("CustomisationScreen",new Dictionary<string, object>());
		currentCount = 0;
		nextCount = currentCount + 1;
		currentActiveObject = m_customisableParts[0];
		state = Camera.main.GetComponent<ColorObjectHandler> ();
		isReadyToReadSavedData = true;

	}
	
	// Update is called once per frame
	void Update () {
		if (nextCount >= m_customisableParts.Count) {
			nextCount = 0;
		}
		if (state == null)
			return;
		if (Input.GetMouseButtonDown(0)&& !state.isActive()) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit)) {
				if (gameObject.name == hit.collider.name) {
					for (int i = 0; i < m_customisableParts.Count; i++) {
						m_customisableParts [i].SetActive (false);
						if (i == nextCount) {
							m_customisableParts [i].SetActive (true);
							currentActiveObject = m_customisableParts [i];
							currentCount = i;
							Debug.Log ("**"+ gameObject.name +":"+ currentCount);
							Analytics.CustomEvent("Customisation", new Dictionary<string, object>
								{
									{ "Customisation Name", gameObject.name },
									{ "id", currentCount }
								});

						}

					}
					nextCount++;
				}


			}

			}
			if(m_customisableParts.Count < 2)
				m_customisableParts [0].SetActive (true);
		
	}

	public void setCurrentColor(string name,Color color)
	{
		if (gameObject.name != name)
			return;
		
		SpriteRenderer currentSprite = currentActiveObject.GetComponent<SpriteRenderer> ();
		currentSprite.color = color;

	}
	public void setActiveState(bool value)
	{
		for (int i = 0; i < m_customisableParts.Count; i++) {
			m_customisableParts [i].SetActive (value);
		}
	}

	public void setCurrentState(int currentState,Color color)
	{
		Debug.Log ("++"+ gameObject.name +":"+ currentState);
		for (int i = 0; i < m_customisableParts.Count; i++) {
			if(i != currentState)
			m_customisableParts [i].SetActive (false);
		}
		currentCount = currentState;
		nextCount = currentCount+ 1;
		m_customisableParts[currentCount].SetActive (true);
		currentActiveObject = m_customisableParts [currentCount];
		SpriteRenderer currentSprite = currentActiveObject.GetComponent<SpriteRenderer> ();
		currentSprite.color = color ;
		isReadyToReadSavedData = false;
		Debug.Log ("--"+ gameObject.name +":"+ currentCount);
	}

	public Color getCurrentColor()
	{
		SpriteRenderer currentSprite = currentActiveObject.GetComponent<SpriteRenderer> ();

		return currentSprite.color;
	}
	public int getCurrentState()
	{

		return currentCount;
	}

}
