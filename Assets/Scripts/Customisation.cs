using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Customisation : MonoBehaviour {

	public List<Sprite> m_customisableParts;

	private int currentCount;
	private int nextCount;
	private SpriteRenderer render;
	private ColorObjectHandler state;
	//added because sometimes the Start gets a delayed call
	public bool isReadyToReadSavedData;

	// Use this for initialization
	void Start () {

	
		currentCount = 0;
		nextCount = currentCount + 1;
		render = GetComponent<SpriteRenderer>();
		render.sprite = m_customisableParts[0];
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
						
						if (i == nextCount) {
							render.sprite = m_customisableParts[i];
							currentCount = i;
							Debug.Log ("**"+ gameObject.name +":"+ currentCount);
							FindObjectOfType<AnalyticsSystem> ().CustomEvent("Customisation", new Dictionary<string, object>
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
			render.sprite = m_customisableParts[0];
		
	}

	public void setCurrentColor(string name,Color color)
	{
		if (gameObject.name != name)
			return;
		render.color = color;

	}
	public void setActiveState(bool value)
	{
		for (int i = 0; i < m_customisableParts.Count; i++) {
			//m_customisableParts [i].SetActive (value);

		}
	}

	public void setCurrentState(int currentState,Color color)
	{
		Debug.Log ("++"+ gameObject.name +":"+ currentState);
		currentCount = currentState;
		nextCount = currentCount+ 1;
		render.sprite = m_customisableParts[currentCount];

		render.color = color ;
		isReadyToReadSavedData = false;
		Debug.Log ("--"+ gameObject.name +":"+ currentCount);
	}

	public Color getCurrentColor()
	{
		return render.color;
	}
	public int getCurrentState()
	{

		return currentCount;
	}

}
