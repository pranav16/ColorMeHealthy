using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ColorObjectHandler : MonoBehaviour {
	
	//public List<GameObject> m_colorObjects;
	public GameObject color;
	public List<GameObject>bodyParts;
	bool touchStarted;

	// Use this for initialization
	void Start () {
//		string filePath = "CurrentCustomization.json";
//		string fileName = Application.persistentDataPath + "/Color" + filePath;
//		System.IO.File.Delete (fileName);
		FindObjectOfType<AnalyticsSystem> ().CustomEvent("CustomisationScreen",new Dictionary<string, object>());
	}


	public bool isActive(){

		return touchStarted;
	}
	
	// Update is called once per frame
	void Update () {
		loadAndSetCustomization ();
		if (color.activeInHierarchy && !touchStarted) {
			touchStarted = true;
			Vector3 position =	Camera.main.ScreenToWorldPoint (Input.mousePosition);
			position.z = -5.0f;
			color.transform.position = position;
			color.SetActive (true);
			
		} else if (color.activeInHierarchy && touchStarted && Input.GetMouseButtonUp (0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			Color colorSelected = color.GetComponent<SpriteRenderer> ().color;
			Vector3 position = color.transform.position;
			position.z = 2.0f;
			color.transform.position = position;
			if (Physics.Raycast (ray, out hit)) {
				for (int i = 0; i < bodyParts.Count; i++) {
					
					bodyParts [i].GetComponent<Customisation> ().setCurrentColor (hit.collider.name, colorSelected);
					FindObjectOfType<AnalyticsSystem> ().CustomEvent("Customisation color selection", new Dictionary<string, object>
						{
							{ "Customisation Name", hit.collider.name },
							{ "color", color.name }
						});
				
				}
					
			}
			color.SetActive (false);
			//color = null;
			touchStarted = false;
		
		
		} else if(color.activeInHierarchy){
		
			Vector3 position =	Camera.main.ScreenToWorldPoint (Input.mousePosition);
			position.z = -7.0f;
			color.transform.position = position;
			color.SetActive (true);

		}

	}

	public void setColorObject(Button button)
	{
		color.GetComponent<SpriteRenderer>().color = button.image.color;
		Vector3 position =	Camera.main.ScreenToWorldPoint (Input.mousePosition);
		position.z = -1.0f;
		color.transform.position = position;
		//felt lazy added audio source for all color pops to this node.
		color.GetComponent<AudioSource> ().Play();
		color.SetActive(true);
	
	}

	public void saveCustomization()
	{
		string filePath = "CurrentCustomization.json";
		string fileName = Application.persistentDataPath + "/Color" + filePath;
		JSONObject json = new JSONObject ();
		JSONObject boyField = new JSONObject ();
		json.AddField ("Boy",boyField);
		for (int i = 0; i < bodyParts.Count; i++) {
			if (!bodyParts [i].gameObject.activeInHierarchy)
				continue;
			JSONObject bodyPart = new JSONObject ();
			bodyPart.AddField("currentState",bodyParts[i].GetComponent<Customisation>().getCurrentState());
			Color color = bodyParts [i].GetComponent<Customisation> ().getCurrentColor();
			bodyPart.AddField("r",color.r);
			bodyPart.AddField("g",color.g);
			bodyPart.AddField("b",color.b);
			bodyPart.AddField("a",color.a);
			boyField.AddField(bodyParts[i].name,bodyPart);
		}
		string serializedJson = json.Print();
		Debug.Log(json.Print());   
		System.IO.File.WriteAllText(fileName, serializedJson);

	}

	public void loadAndSetCustomization()
	{
		Camera.main.GetComponent<GenderSelector> ().setInitialStates();
		string filePath = "CurrentCustomization.json";
		string fileName = Application.persistentDataPath + "/Color" + filePath;
		if (!System.IO.File.Exists (fileName))return;
		string data = System.IO.File.ReadAllText(fileName);
		JSONObject json = new JSONObject (data);
		JSONObject boyField = json.GetField("Boy");
		for (int i = 0; i < bodyParts.Count; i++) {
			JSONObject bodyPart = boyField.GetField(bodyParts[i].name);
			if (bodyPart == null)
				continue;
			int currentState = (int) bodyPart.GetField("currentState").i;
			float r = bodyPart.GetField("r").f;
			float g = bodyPart.GetField("g").f;
			float b = bodyPart.GetField("b").f;
			float a = bodyPart.GetField("a").f;
			Color color = new Color (r, g, b, a);
			if(bodyParts [i].GetComponent<Customisation> ().isReadyToReadSavedData)
			bodyParts [i].GetComponent<Customisation> ().setCurrentState(currentState,color);

		}
	
	}

}
