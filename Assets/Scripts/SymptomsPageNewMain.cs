﻿// need to refactor this class
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
	public GameObject saveProgressBar;
	public Image loadDone;
	// Use this for initialization
	void Start () {
		
		firstTouches = 0;
		currentState = States.waitForFirstTouch;
		touchLocations = new List<Vector3>();
		lineGameObjects = new List<GameObject>();
		loadSymptoms ();
		if (mainSymptoms == null)
			mainSymptoms = new BodyPartsTable ();
		else
			hotLoadMainSymptoms();
		loadAndSetCustomization ();
		FindObjectOfType<AnalyticsSystem> ().CustomEvent("Fill_Symptoms",new Dictionary<string, object>());

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
		if (Input.GetMouseButtonDown (0) && loadDone.gameObject.activeInHierarchy) {
			loadDone.gameObject.SetActive (false);
			SceneManager.LoadScene ("MainSelectionScreen");
			return;
		}


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
			touchPosition.z = -9.0f;
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
	
			if (Physics.Raycast(ray, out hit))
			{
				//Debug.Log(hit.collider.name);
				//Debug.Log(hit.collider.transform.position);

				if (!ValidateBodyPart (hit.collider.name)) {
					deleteLine ();
					touchLocations.Clear();
					currentState = States.ready;
					return;
				}

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

	bool ValidateBodyPart(string bodypart)
	{
		switch (bodypart) {
		case "Head":
			return true;
		case "Chest":
			return true;
		case "RightHand":
			return true;
		case "LeftHand":
			return true;
		case "RightLeg":
			return true;
		case "LeftLeg":
			return true;
		case "Stomach":
			return true;
		case "Abdomen":
			return true;
		case "RightToe":
			return true;
		case "LeftToe":
			return true;
		case "RightPalms":
			return true;
		case "LeftPalms":
			return true;
		case "Mouth":
			return true;
		}

		return false;
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
	
		for (int i = 0;i < 8;i++)
		{
			int j = i;
			if (toggles [i].isOn) {
				symptoms symptom = new symptoms();
				symptom.name = toggles [i].GetComponentInChildren<Text> ().text;
				if (j == 3) {
					symptom.painScale = howManyTimes.value + 1;
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
		if (toggles [8].isOn) {
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
				if (questions[3].text == mainSymptoms.getSymptoms()[j].name && mainSymptoms.getSymptoms()[j].botherScale == -1.0f && mainSymptoms.getSymptoms()[j].painScale == 1.0f) {
					toggles [8].isOn = true;
				}

				if (toggles [i].GetComponentInChildren<Text> ().text == mainSymptoms.getSymptoms()[j].name) 
				{
					int k = i;

		   if (k == 3 && mainSymptoms.getSymptoms()[j].botherScale == -1.0f) {
						toggles [i].isOn = true;
						howManyTimes.value = (int)mainSymptoms.getSymptoms () [j].painScale;
						continue;
					} 
			else if(mainSymptoms.getSymptoms()[j].botherScale >= 0)
					{
					toggles [i].isOn = true;
						if (k > 3) {
							k -= 1;
						}
					howMuch [k].value = mainSymptoms.getSymptoms () [j].painScale;
					botherSome [k].value = mainSymptoms.getSymptoms ()[j].botherScale;
				}
	
			}
		if (mainSymptoms.getSymptoms () [j].name.Contains ("os_"))
					otherSymptoms.text = mainSymptoms.getSymptoms () [j].name.Replace ("os_","");
		if (mainSymptoms.getSymptoms () [j].name.Contains ("bothersome_"))
					WhatsBothering.text = mainSymptoms.getSymptoms () [j].name.Replace ("bothersome_","");
		if (mainSymptoms.getSymptoms () [j].name.Contains ("FeelingToday_"))
					FeelingToday.text = mainSymptoms.getSymptoms () [j].name.Replace ("FeelingToday_","");
		if (mainSymptoms.getSymptoms () [j].name.Contains ("Bestthing_"))
					bestThingToday.text = mainSymptoms.getSymptoms () [j].name.Replace ("Bestthing_","");
		if (mainSymptoms.getSymptoms () [j].name.Contains ("Bothersome_"))
					WhatsBothering.text = mainSymptoms.getSymptoms () [j].name.Replace ("Bothersome_","");
		}
		
		
			

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
		for (int i = 0;i < 8;i++)
		{
			int j = i;
			if (j == 3) {
				if (!toggles [i].isOn)
					continue;
				symptoms symptom = new symptoms();
				symptom.name = toggles [i].GetComponentInChildren<Text> ().text;
				symptom.painScale = howManyTimes.value + 1;
				symptom.botherScale = -1;
				table.addSymptoms (symptom);
				continue;
			}
			else if (j > 3)
				j -= 1;
			if (toggles [i].isOn) {
				symptoms symptom = new symptoms ();
				symptom.name = toggles [i].GetComponentInChildren<Text> ().text;
				symptom.botherScale = botherSome [j].value;
				symptom.painScale = howMuch [j].value;
				table.addSymptoms (symptom);
			} else {
				symptoms symptom = new symptoms ();
				symptom.name = toggles [i].GetComponentInChildren<Text> ().text;
				symptom.botherScale = -1;
				symptom.painScale = -1;
				table.addSymptoms (symptom);
			
			
			}
		}
		if (toggles [8].isOn) {
			symptoms symptom = new symptoms ();
			symptom.name = questions [3].text;
			symptom.botherScale = -1.0f;
			symptom.painScale = 1.0f;
			table.addSymptoms (symptom);
		} else {
			symptoms symptom = new symptoms ();
			symptom.name = questions [3].text;
			symptom.botherScale = -1.0f;
			symptom.painScale = -1.0f;
			table.addSymptoms (symptom);
		}
		if(otherSymptoms.text == "")
			otherSymptoms.text = "None";
			symptoms symptom1 = new symptoms();
			symptom1.name = "os_"+ otherSymptoms.text;
			table.addSymptoms (symptom1);
		
		if(WhatsBothering.text == "")
			WhatsBothering.text = "None";
			symptoms symptom2 = new symptoms();
			symptom2.name = "Bothersome_" + WhatsBothering.text;
			table.addSymptoms (symptom2);

		if(FeelingToday.text == "")
			FeelingToday.text = "None";
			symptoms symptom3 = new symptoms();
			//dirty way to make whats bothering you the most symptoms fit into our bodyparts table model
			symptom3.name = "FeelingToday_" + FeelingToday.text;
			table.addSymptoms (symptom3);
		

		if(bestThingToday.text == "")
			bestThingToday.text = "None";
			symptoms symptom4 = new symptoms();
			symptom4.name = "Bestthing_" + bestThingToday.text;
			table.addSymptoms (symptom4);

		
		finalSymptoms ["General Symptoms"] = table;
	}
	public void savedClicked()
	{
		saveProgressBar.SetActive (true);
		Invoke ("saveData", 1);
	}

	private void saveData()
	{
		createMainSymptoms();
		buildJSONFile ();


		//FindObjectOfType<PdfExporter> ().CreatePDF (finalSymptoms);
		saveProgressBar.SetActive (false);
		loadDone.gameObject.SetActive (true);
		FindObjectOfType<AnalyticsSystem> ().CustomEvent("Symptoms Main save clicked",new Dictionary<string, object>());
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

		string filePath = "Symptoms.json";
		string fileName = Application.persistentDataPath + "/Color" + filePath;
		string rawjson = System.IO.File.ReadAllText(fileName);
		//new json being build
		JSONObject mainJson = new JSONObject();
		JSONObject entries = new  JSONObject(JSONObject.Type.ARRAY);
		mainJson.AddField("Entires",entries);
		//checking if edit is happening on the same date. Change data structure if this causes performance issues.
		JSONObject oldData = new JSONObject (rawjson);
		JSONObject Entries = oldData.GetField ("Entires");
		foreach (JSONObject obj in Entries.list) {
			if (obj.HasField ("Date") && obj.GetField ("Date").str == System.DateTime.Now.ToString ("MM_dd_yyyy"))
				continue;
			
			entries.Add(obj);
		}

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
			obj.AddField ("timestamp", table.getTimeStamp ());
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
		  entries.Add(json);
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
			obj.AddField ("timestamp", table.getTimeStamp ());
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

	public void loadSymptoms()
	{
		string filePath = "Symptoms.json";
		string fileName = Application.persistentDataPath + "/Color" + filePath;
		if (!System.IO.File.Exists (fileName))
			return;
		string rawjson = System.IO.File.ReadAllText(fileName);

		JSONObject oldData = new JSONObject (rawjson);
		JSONObject Entries = oldData.GetField ("Entires");
		foreach (JSONObject obj in Entries.list) {
			if (obj.HasField ("Date") && obj.GetField ("Date").str != System.DateTime.Now.ToString ("MM_dd_yyyy"))
				continue;
			JSONObject bps =  obj.GetField ("BodyParts");
			if (bps == null)
				continue;
			foreach (string keys in bps.keys)
			{
				JSONObject bodypartJson = bps.GetField (keys);
				BodyPartsTable bodyPart = new BodyPartsTable();
				bodyPart.setPartName (keys);
				bodyPart.setImagePath (bodypartJson.GetField("imgSrc").str);
				JSONObject symptoms = bodypartJson.GetField ("symptoms");
				foreach(JSONObject sy in symptoms.list)
				{
					symptoms symptom = new symptoms ();
					symptom.name = sy.GetField("symptomname").str;
					symptom.painScale = sy.GetField("painscale").f ;
					symptom.botherScale = sy.GetField("bothersomescale").f ;
					bodyPart.addSymptoms (symptom);
				}
				Debug.Log (keys);
				if (!finalSymptoms.ContainsKey (keys))
					finalSymptoms.Add (keys, bodyPart);
				else
					finalSymptoms [keys] = bodyPart;
				if (keys.Contains ("General Symptoms")) {
					mainSymptoms = bodyPart;
				}
			}

		}

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
