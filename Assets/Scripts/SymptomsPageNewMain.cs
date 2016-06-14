// need to refactor this class
// the genral symtops was a late addition and done a bit hackily.
//never cleaned it up. should be simple though :|

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SymptomsPageNewMain : MonoBehaviour {

	enum States {touchbegin,touchend,drawImage,ready,waitForFirstTouch};
	private States currentState;
	private List<Vector3> touchLocations;
	public  GameObject touchSprite;
	private string bodyPartSelected;
	private List<GameObject> lineGameObjects;
	public static Dictionary<string,BodyPartsTable>finalSymptoms = new Dictionary<string, BodyPartsTable>();
	public List<Toggle> toggles;
	public InputField otherSymptoms;
	public InputField WhatsBothering;
	public InputField FeelingToday;
	public InputField bestThingToday;
	public List<Text> questions;
	public List<GameObject>bodyParts;
	public static BodyPartsTable mainSymptoms;
	public List<Slider> howMuch;
	public List<Slider>botherSome;
	public Dropdown howManyTimes;
	private int firstTouches;

	// Use this for initialization
	void Start () {
		
		firstTouches = 0;
		currentState = States.waitForFirstTouch;
		touchLocations = new List<Vector3>();
		lineGameObjects = new List<GameObject>();
		if (mainSymptoms == null)
			mainSymptoms = new BodyPartsTable ();
		else
			hotLoadMainSymptoms();
		loadAndSetCustomization ();
		FindObjectOfType<AnalyticsSystem> ().CustomEvent("Fill_Symptoms",new Dictionary<string, object>());
 		
//		string filePath = "Symptoms.json";
//		string fileName = Application.persistentDataPath + "/Color" + filePath;
//		System.IO.File.Delete (fileName);

	}
	bool checkForInteractableObjects(string name)
	{
		bool isInteractableObject = false;
		switch (name) {
		case "WindowCollider":
			isInteractableObject = true;
			break;
		case "WindowCollider (1)":
			isInteractableObject = true;
			break;
		case "WindowCollider (2)":
			isInteractableObject = true;
			break;
		case "WindowCollider (4)":
			isInteractableObject = true;
			break;
		case "WindowCollider (5)":
			isInteractableObject = true;
			break;


		}
		return isInteractableObject;
	}
	// Update is called once per frame
	void Update () {
		loadAndSetCustomization ();
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit)) {
				if (checkForInteractableObjects(hit.collider.name)|| hit.collider.name.Contains("Blocker")) {
					return;	
				}
			}
		}
		RaycastHit hitCheck;
		Ray rayCheck = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast (rayCheck, out hitCheck)) {
			if (checkForInteractableObjects(hitCheck.collider.name) || hitCheck.collider.name.Contains("Blocker")) {
				return;	
			}
		}

		if(Input.GetMouseButton(0) && currentState == States.waitForFirstTouch && firstTouches < 2)
		{
			firstTouches++;
			//currentState = States.touchbegin;
			return;
		}
		else if(Input.GetMouseButton(0) && currentState == States.waitForFirstTouch && firstTouches >= 2)
		{
			currentState = States.touchbegin;
			return;
		}
		else if (Input.GetMouseButton(0) && (currentState == States.ready || currentState == States.touchbegin))
		{
			RaycastHit checkForBlocker;
			Ray checkForBlockerRay = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (checkForBlockerRay, out checkForBlocker)) {
				if (checkForBlocker.collider.name.Contains("Blocker")) {
					currentState = States.touchend;
					return;
				}
			}
			Vector3 touchPosition =  Camera.main.ScreenToWorldPoint(Input.mousePosition);
			touchPosition.z = -8.0f;
			touchLocations.Add(touchPosition);
			currentState = States.touchbegin;
			GameObject lineDrawn = Instantiate(touchSprite, touchPosition, Quaternion.identity) as GameObject;
			lineGameObjects.Add(lineDrawn);
		}
		else if(currentState == States.touchbegin)
		{
			currentState = States.touchend;
		}
		else if(currentState == States.touchend)
		{
			if (touchLocations.Count <= 0) {
				currentState = States.ready;
				return;
			}
			Vector3 meanPosition = new Vector3();
			for(int i = 0;i < touchLocations.Count;i ++)
			{
				meanPosition.x += touchLocations[i].x;
				meanPosition.y += touchLocations[i].y;
			}
			meanPosition.x /= touchLocations.Count;
			meanPosition.y /= touchLocations.Count;
			meanPosition.z = 1000.0f;
			Debug.Log("centre of points: " + meanPosition);
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(meanPosition));
			deleteLine ();
			List<Vector3>smoothPoints =  MakeSmoothCurve (touchLocations, 10.0f);
			if (smoothPoints == null)
				return;
			for (int i = 0; i < smoothPoints.Count; i++) {
				GameObject lineDrawn = Instantiate (touchSprite, smoothPoints[i], Quaternion.identity) as GameObject;
				lineGameObjects.Add (lineDrawn);
			}
			if (Physics.Raycast(ray, out hit))
			{
				Debug.Log(hit.collider.name);
				Debug.Log(hit.collider.transform.position);
				bodyPartSelected = hit.collider.name;
				SymptomsPageNewLocal.bodyPartSelected = bodyPartSelected;
				FindObjectOfType<AnalyticsSystem> ().CustomEvent("Symptoms Main Page body part selected", new Dictionary<string, object>
					{
						{ "body part", bodyPartSelected },

					});
				retainMainSymptoms ();
				SceneManager.LoadScene ("SymptomsLocal");
			}

			touchLocations.Clear();
			currentState = States.ready;

		}
	}
	public void invokeDeletion()
	{
		deleteLine();
	}
	public string getCurrentBodyPart()
	{
		return bodyPartSelected;
	}

	public void onToggleChange(Toggle toggle)
	{
		toggle.isOn = !toggle.isOn;
	}

	private void deleteLine()
	{
		for(int i = 0; i < lineGameObjects.Count; i++)
		{
			Destroy(lineGameObjects[i]);
		}
		lineGameObjects.Clear();
	}
	public static List<Vector3> MakeSmoothCurve(List<Vector3> arrayToCurve,float smoothness){

		List<Vector3> points;
		List<Vector3> curvedPoints = new List<Vector3>();
		int pointsLength = 0;
		int curvedLength = 0;

		if(smoothness < 1.0f) smoothness = 1.0f;

		pointsLength = arrayToCurve.Count;
		if (arrayToCurve.Count < 1)
			return null;

		curvedLength = (pointsLength*Mathf.RoundToInt(smoothness))-1;
		curvedPoints = new List<Vector3>(curvedLength);

		float t = 0.0f;
		for(int pointInTimeOnCurve = 0;pointInTimeOnCurve < curvedLength+1;pointInTimeOnCurve++){
			t = Mathf.InverseLerp(0,curvedLength,pointInTimeOnCurve);

			points = new List<Vector3>(arrayToCurve);

			for(int j = pointsLength-1; j > 0; j--){
				for (int i = 0; i < j; i++){
					points[i] = (1-t)*points[i] + t*points[i+1];
				}
			}

			curvedPoints.Add(points[0]);
		}

		return curvedPoints;
	}

	public void  retainMainSymptoms()
	{		
		mainSymptoms.setPartName ("General Symptoms");
	
		for (int i = 0;i < 7;i++)
		{
			int j = i;
			if (toggles [i].isOn) {
				symptoms symptom = new symptoms();
				symptom.name = toggles [i].GetComponentInChildren<Text> ().text;
				if (j == 1) {
					symptom.painScale = howManyTimes.value;
					symptom.botherScale = -1.0f;
					mainSymptoms.addSymptoms (symptom);
					continue;
				} else if (j > 1) {
					j -= 1;
				}
				if (howMuch.Count > j)
					symptom.painScale = howMuch [j].value;
				if (botherSome.Count > j)
					symptom.botherScale = botherSome [j].value;
				mainSymptoms.addSymptoms (symptom);
			}
		}
		if (toggles [7].isOn) {
			symptoms symptom = new symptoms();
			symptom.name =  questions[3].text;
			symptom.botherScale = -1.0f;
			mainSymptoms.addSymptoms (symptom);
		}

		if (otherSymptoms.text != "") {
			symptoms symptom = new symptoms();
			//dirty way to make other symptoms fit into our bodyparts table model
			symptom.name = "os_" + otherSymptoms.text;
			mainSymptoms.addSymptoms (symptom);
		}
		if (WhatsBothering.text != "") {
			symptoms symptom = new symptoms();
			//dirty way to make whats bothering you the most symptoms fit into our bodyparts table model
			symptom.name = "bothersome_" + WhatsBothering.text;
			mainSymptoms.addSymptoms (symptom);
		}
		if (FeelingToday.text != "") {
			symptoms symptom = new symptoms();
			//dirty way to make whats bothering you the most symptoms fit into our bodyparts table model
			symptom.name = "FeelingToday_" + FeelingToday.text;
			mainSymptoms.addSymptoms (symptom);
		}
		if (bestThingToday.text != "") {
			symptoms symptom = new symptoms();
			symptom.name = "Bestthing_" + FeelingToday.text;
			mainSymptoms.addSymptoms (symptom);
		
		}
	

	}
	public void hotLoadMainSymptoms()
	{
		mainSymptoms.setPartName ("General Symptoms");
		for (int i = 0;i < 8;i++)
		{
			for (int j = 0; j < mainSymptoms.getSymptoms ().Count; j++) {
				if (i == 7 && questions[2].text == mainSymptoms.getSymptoms()[j].name) {
					toggles [7].isOn = true;
				}

				if (toggles [i].GetComponentInChildren<Text> ().text == mainSymptoms.getSymptoms()[j].name) 
				{
					int k = i;
					if (k == 1) {
						toggles [i].isOn = true;
						howManyTimes.value = (int)mainSymptoms.getSymptoms () [j].painScale;
						continue;
					} else if (k > 1) {
						k -= 1;
					}
					toggles [i].isOn = true;
					howMuch [k].value = mainSymptoms.getSymptoms () [j].painScale;
					botherSome [k].value = mainSymptoms.getSymptoms ()[j].botherScale;
				}
			
				if (mainSymptoms.getSymptoms () [j].name.Contains ("os_"))
					otherSymptoms.text = mainSymptoms.getSymptoms () [j].name.Replace ("os_","");
				if (mainSymptoms.getSymptoms () [j].name.Contains ("bothersome_"))
					WhatsBothering.text = mainSymptoms.getSymptoms () [j].name.Replace ("bothersome_","");
				if (mainSymptoms.getSymptoms () [j].name.Contains ("FeelingToday_"))
					FeelingToday.text = mainSymptoms.getSymptoms () [j].name.Replace ("FeelingToday_","");
				if (mainSymptoms.getSymptoms () [j].name.Contains ("Bestthing_"))
					bestThingToday.text = mainSymptoms.getSymptoms () [j].name.Replace ("Bestthing_","");
					
			}

		}

	}

	public void ToggleValueChanged(int value)
	{
		return;
		int index = 0;
		if (value > 1)
			index = value - 1;
		if (toggles[value].isOn) {
			botherSome [index].interactable = true;
			howMuch [index].interactable = true;
		} else {
			botherSome [index].interactable = false;
			howMuch [index].interactable = false;
			botherSome [index].value = botherSome [index].minValue;
			howMuch [index].value = howMuch [index].minValue;
		}

	}

	public void SliderValueChanged(int value)
	{
		int index = value;
	
		toggles [index].isOn = true;

	}

	public void createMainSymptoms()
	{
		BodyPartsTable table = new BodyPartsTable ();
		table.setPartName ("General Symptoms");
		for (int i = 0;i < 7;i++)
		{
			int j = i;
			if (j == 1) {
				if (!toggles [i].isOn)
					continue;
				symptoms symptom = new symptoms();
				symptom.name = toggles [i].GetComponentInChildren<Text> ().text;
				symptom.painScale = howManyTimes.value;
				symptom.botherScale = -1;
				table.addSymptoms (symptom);
				continue;
			}
			else if (j > 1)
				j -= 1;
			if (toggles [i].isOn) {
				symptoms symptom = new symptoms();
				symptom.name = toggles [i].GetComponentInChildren<Text> ().text;
				symptom.botherScale = botherSome [j].value;
				symptom.painScale = howMuch [j].value;
				table.addSymptoms (symptom);
			}
		}
		if (toggles [7].isOn) {
			symptoms symptom = new symptoms();
			symptom.name =  questions[3].text;
			symptom.botherScale = -1.0f;
			table.addSymptoms (symptom);
		}
		if (otherSymptoms.text != "") {
			symptoms symptom = new symptoms();
			symptom.name = "os_"+ otherSymptoms.text;
			table.addSymptoms (symptom);
		}
		if (WhatsBothering.text != "") {
			symptoms symptom = new symptoms();
			symptom.name = "Bothersome_" + WhatsBothering.text;
			table.addSymptoms (symptom);
		}
		if (FeelingToday.text != "") {
			symptoms symptom = new symptoms();
			//dirty way to make whats bothering you the most symptoms fit into our bodyparts table model
			symptom.name = "FeelingToday_" + FeelingToday.text;
			table.addSymptoms (symptom);
		}
		if (bestThingToday.text != "") {
			symptoms symptom = new symptoms();
			symptom.name = "Bestthing_" + FeelingToday.text;
			table.addSymptoms (symptom);

		}
		finalSymptoms ["General Symptoms"] = table;
	}
	public void savedClicked()
	{
		createMainSymptoms();
		buildJSONFile ();
		PdfExporter ex= FindObjectOfType<PdfExporter> ();
		FindObjectOfType<PdfExporter> ().CreatePDF (finalSymptoms);
		//FindObjectOfType<AnalyticsSystem> ().CustomEvent("Symptoms Main save clicked",new Dictionary<string, object>());

	}

	void OnDestroy() {

		//savedClicked ();
	}


	public void backButtonClicked()
	{
		Camera.main.GetComponent<AudioSource> ().Play();
	
		SceneManager.LoadScene("MainSelectionScreen");
	}

	public void editJSONFile()
	{
		Dictionary<string ,object> analytics = new Dictionary<string, object> ();
		Debug.Log("is file present");
		string filePath = "Symptoms.json";
		string fileName = Application.persistentDataPath + "/Color" + filePath;
		string rawjson = System.IO.File.ReadAllText(fileName);
		Debug.Log("is file present");
		JSONObject mainJson = new JSONObject(rawjson);
		JSONObject Entries = mainJson.GetField("Entires");
		JSONObject json = new JSONObject();
		json.AddField("Date", System.DateTime.Now.ToString("MM_dd_yyyy"));
		JSONObject BodyParts = new JSONObject();
	    json.AddField("BodyParts", BodyParts);
		foreach (string key in finalSymptoms.Keys) {
			BodyPartsTable table = finalSymptoms [key];
			JSONObject obj = new JSONObject ();
			JSONObject arr = new JSONObject (JSONObject.Type.ARRAY);
			obj.AddField ("name",table.getPartName());
			analytics ["Body Part"] = table.getPartName ();
			obj.AddField ("symptoms",arr);
			obj.AddField ("imgSrc", table.getImagePath ());
			foreach (symptoms symptom in table.getSymptoms()) {
				JSONObject sy = new JSONObject ();
				sy.AddField ("symptomname", symptom.name);
				analytics ["Symptom Name"] = symptom.name;
				sy.AddField ("painscale", symptom.painScale);
				analytics ["Pain Scale"] = symptom.painScale;
				sy.AddField ("bothersomescale", symptom.botherScale);
				analytics ["bother Scale"] = symptom.botherScale;
				arr.Add (sy);

			}
			BodyParts.AddField(table.getPartName (), obj);
		}
		//FindObjectOfType<AnalyticsSystem> ().CustomEvent ("Sysmtoms Page Final Symptom",analytics);
		  Entries.Add(json);
		  string serializedJson = mainJson.Print();
		  Debug.Log(mainJson.Print());
		  System.IO.File.WriteAllText(fileName, serializedJson);
	}


	public void buildJSONFile()
	 {
	       string filePath = "Symptoms.json";
	       string fileName = Application.persistentDataPath + "/Color" + filePath;
	       if( System.IO.File.Exists(fileName))
	        {
	            editJSONFile();
	            return;
	        }
			Dictionary<string ,object> analytics = new Dictionary<string, object> ();
	        JSONObject mainJson = new JSONObject();
	        JSONObject entries = new  JSONObject(JSONObject.Type.ARRAY);
	        mainJson.AddField("Entires",entries);
	        JSONObject json = new JSONObject();
	        json.AddField("Date", System.DateTime.Now.ToString("MM_dd_yyyy"));
	        JSONObject BodyParts = new JSONObject();
	        json.AddField("BodyParts", BodyParts);
		   foreach (string key in finalSymptoms.Keys) {
			BodyPartsTable table = finalSymptoms [key];
		
			JSONObject obj = new JSONObject ();
			JSONObject arr = new JSONObject (JSONObject.Type.ARRAY);
			obj.AddField ("name",table.getPartName());
			analytics ["Body Part"] = table.getPartName ();
			obj.AddField ("symptoms",arr);
			obj.AddField ("imgSrc", table.getImagePath ());
			foreach (symptoms symptom in table.getSymptoms()) {
				JSONObject sy = new JSONObject ();
				sy.AddField ("symptomname", symptom.name);
				analytics ["Symptom Name"] = symptom.name;
				sy.AddField ("painscale", symptom.painScale);
				analytics ["Pain Scale"] = symptom.painScale;
				sy.AddField ("bothersomescale", symptom.botherScale);
				analytics ["bother Scale"] = symptom.botherScale;
				arr.Add (sy);

			}
			BodyParts.AddField (table.getPartName (), obj);
		}
	        entries.Add(json);
	        string serializedJson = mainJson.Print();
	        Debug.Log(mainJson.Print());   
	        System.IO.File.WriteAllText(fileName, serializedJson);
	       
	
	    
	}

	public void EarseClicked()
	{
		foreach (Toggle toggle in toggles) {
			toggle.isOn = false;
		}
		foreach (Slider slide in howMuch)
			slide.value = slide.minValue;
		foreach (Slider slide in botherSome)
			slide.value = slide.minValue;
		otherSymptoms.text = "";
		finalSymptoms.Clear();
		questions [0].gameObject.GetComponent<AudioSource> ().Play ();
		FindObjectOfType<AnalyticsSystem> ().CustomEvent ("Symptoms Main Earse Clicked",new Dictionary<string, object>()); 


	}
	public void loadAndSetCustomization()
	{
		FindObjectOfType<GenderSelector> ().setInitialStates();

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
