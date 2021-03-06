﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class SymptomsHistory : MonoBehaviour {

	[System.Serializable]
	public struct UILocalSymptomNode
	{
		public Image parent;
		public Image symptomImage;
		public Text partName;
		public Text time;
		public List<Text> symptomName;
		public List<Text>painSymptom;
		public List<Text>BotherSymptom;
		public List<GameObject> symptomCell;
	};

	public GameObject localSymptomsNode;
	public Dropdown monthSelector;
	public Dropdown yearSelector;
	public Button dateSelectorReference;
	public GameObject CLayerListOfDates;
	public GameObject CLayerSymptoms;
	public GameObject GeneralSymptomNode;
	public List<UILocalSymptomNode>symptomsNode;
	public List<Text> generalSymptomsText;
	public List<Button> listOfDates;
	public GameObject sendMailProgressBar;
	public InputField emailId;
	private string email;
	public GameObject SentImage;
	public static Dictionary<string,BodyPartsTable>currentSymptoms = new Dictionary<string, BodyPartsTable>();
	private Dictionary<string,List<BodyPartsTable>> SymptomsMap;
	public CanvasScaler scaler;
	void Start () {
		SymptomsMap = new Dictionary<string, List<BodyPartsTable>> ();
		currentSymptoms = new Dictionary<string, BodyPartsTable> ();
		listOfDates = new List<Button> ();
		initialize ();
	}
	

	void Update () {

		if(Input.GetMouseButtonDown(0))
			SentImage.SetActive (false);
	
		for (int i = 0; i < localSymptomsNode.gameObject.transform.childCount; i++)
			Debug.Log ("child name : " + localSymptomsNode.gameObject.transform.GetChild (i).name);
	}

	bool initialize()
	{
		LoadSymptomsJson();
		populateDateDropDowns();
        string currentMonth = System.DateTime.Now.Month.ToString("00");
        string currentYear = System.DateTime.Now.Year.ToString("0000");
        populateListOfDates(currentMonth, currentYear);
		email = PlayerPrefs.GetString ("Email","");
		emailId.text = email;
		FindObjectOfType<PdfExporter> ().setEmailAddress (email);
        return true;
	}

	bool populateDateDropDowns()
	{
		List<string> months = new List<string> ();
		List<string> years = new List<string> ();
		string previousMonth = "";
		string previousYear = "";
		foreach (string date in SymptomsMap.Keys) {
			string[] dateSplit	= date.Split('_');
			if (previousYear != dateSplit[2]) {
				years.Add (dateSplit[2]);
				previousYear = dateSplit[2];
			}

			if(previousMonth != dateSplit[0])
			{
				months.Add (dateSplit[0]);
				previousMonth = dateSplit[0];
			}


		}
		monthSelector.AddOptions (months);
		yearSelector.AddOptions (years);
		return true;
	}

	public void onEmailChange(string email)
	{
		email = emailId.text;
		FindObjectOfType<PdfExporter> ().setEmailAddress (email);
		PlayerPrefs.SetString ("Email",email);
		PlayerPrefs.Save ();
	}

	public  void monthValueChanged(int value)
	{
		string month = monthSelector.options [value].text;
		string year = yearSelector.options [yearSelector.value].text;
		populateListOfDates (month, year);


	}
	public  void yearValueChanged(int value)
	{
		string month = monthSelector.options [monthSelector.value].text;
		string year = yearSelector.options [value].text;
		populateListOfDates (month, year);
	}
	public void dateSelected(Button button)
	{
		string id = button.GetComponentInChildren<Text> ().text.Replace ("/", "_");
		populateSymptoms (id);
	}
	void populateSymptoms(string id)
	{
		for (int k = 0; k < symptomsNode.Count; k++) {
			symptomsNode [k].parent.gameObject.SetActive (false);
		}
		currentSymptoms.Clear ();
		if (SymptomsMap.ContainsKey (id)) {
			List<BodyPartsTable> tables = SymptomsMap [id];

			for (int i = 0, k = 0; i < tables.Count; i++ ,k++) {
				BodyPartsTable table = tables [i];
				if (table.getPartName ().Contains ("General Symptoms")) {
					currentSymptoms.Add (table.getPartName (), table);
					populateGeneralSymptoms (table);
					k = i - 1;
					continue;
				}

				symptomsNode [k].partName.text = table.getPartName ();
				symptomsNode [k].time.text = table.getTimeStamp ();
				currentSymptoms.Add (table.getPartName (), table);
				symptomsNode [k].parent.gameObject.SetActive (true);
				byte[] textureData = System.IO.File.ReadAllBytes (table.getImagePath ());
				Texture2D tex = new Texture2D (350, 350);
				tex.LoadImage (textureData);
				Sprite sprite = Sprite.Create (tex, new Rect (100, 100, 250, 250), new Vector2 (0.5f, 0.5f));
				symptomsNode [k].symptomImage.sprite = sprite;

				for (int j = 0; j < table.getSymptoms ().Count; j++) {

					symptomsNode [k].BotherSymptom [j].text = symptomPointsToText ((int)table.getSymptoms () [j].botherScale);
					symptomsNode [k].painSymptom [j].text = symptomPointsToText ((int)table.getSymptoms () [j].painScale);
					symptomsNode [k].symptomName [j].text = table.getSymptoms () [j].name;

				}

				for (int j = table.getSymptoms ().Count; j < symptomsNode [k].symptomCell.Count; j++)
					symptomsNode [k].symptomCell [j].SetActive (false);
				
			}

			
		}

		resizeTheScrollView ();
	}


	void resizeTheScrollView()
	{
		int count = 0;
		foreach (UILocalSymptomNode node in symptomsNode)
			if (node.parent.gameObject.activeInHierarchy)
				count++;

		float sizeOfScroll = count * symptomsNode [0].parent.gameObject.GetComponent<RectTransform> ().rect.height + GeneralSymptomNode.GetComponent<RectTransform>().rect.height;
		CLayerSymptoms.GetComponent<RectTransform> ().sizeDelta = new Vector2(0,sizeOfScroll);
		float positionY = sizeOfScroll / 2 - GeneralSymptomNode.GetComponent<RectTransform>().rect.height/2;
		GeneralSymptomNode.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, positionY);
		positionY -= (GeneralSymptomNode.GetComponent<RectTransform> ().rect.height/2 + symptomsNode [0].parent.gameObject.GetComponent<RectTransform> ().rect.height/2);

		for (int i = 0; i <= count; i++) {

			symptomsNode [i].parent.gameObject.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, positionY);
			positionY -= symptomsNode [i].parent.gameObject.GetComponent<RectTransform> ().rect.height;
		
		}
			

	}
		
	void populateGeneralSymptoms(BodyPartsTable generalSymptoms)
	{
		
		int indexForText = 0;
		foreach (symptoms GeneralSymptom in  generalSymptoms.getSymptoms ()) {
			generalSymptomsText [indexForText].text = "";
			if (GeneralSymptom.name.Contains ("os_")) {
				generalSymptomsText [indexForText].text = "Did anything else make you feel sick today? ";
				indexForText++;
				generalSymptomsText [indexForText].text = GeneralSymptom.name.Replace ("os_", "");
				indexForText++;

			} else if (GeneralSymptom.name.Contains ("Bothersome_")) {
				generalSymptomsText [indexForText].text = "What is bothering you the most? ";
				indexForText++;
				generalSymptomsText [indexForText].text = GeneralSymptom.name.Replace ("Bothersome_", "");
				indexForText++;
			} else if (GeneralSymptom.name.Contains ("FeelingToday_")) {
				generalSymptomsText [indexForText].text = "How are you feeling today? ";
				indexForText++;
				generalSymptomsText [indexForText].text = GeneralSymptom.name.Replace ("FeelingToday_", "");
				indexForText++;
			} else if (GeneralSymptom.name.Contains ("Bestthing_")) {
				generalSymptomsText [indexForText].text = "What is the best thing about today? ";
				indexForText++;
				generalSymptomsText [indexForText].text = GeneralSymptom.name.Replace ("Bestthing_", "");
				indexForText++;
			} else if (GeneralSymptom.botherScale >= 0) {
				generalSymptomsText [indexForText].text = GeneralSymptom.name.Replace ("?", "?  Yes");
				indexForText++;
				generalSymptomsText [indexForText].text = "Severity: " + symptomPointsToText((int)GeneralSymptom.painScale) + " Bother: " + symptomPointsToText((int)GeneralSymptom.botherScale);
				indexForText++;
			 } 
			else if(GeneralSymptom.botherScale == -1.0f && GeneralSymptom.painScale == -1.0f && GeneralSymptom.name.Contains("school"))
			{
				generalSymptomsText [indexForText].text = GeneralSymptom.name;
				indexForText++;
				generalSymptomsText [indexForText].text = "No";
				indexForText++;
			}
			else if (GeneralSymptom.botherScale == -1.0f && GeneralSymptom.painScale == -1.0f) {
				generalSymptomsText [indexForText].text = GeneralSymptom.name.Replace ("?", "?  Yes");
				indexForText++;
				generalSymptomsText [indexForText].text = "Severity: " + 0.0f + " Bother: " + 0.0f;
				indexForText++;
			}
			else if(GeneralSymptom.name.Contains ("?") && GeneralSymptom.botherScale == -1.0f)
			{
				generalSymptomsText [indexForText].text = GeneralSymptom.name.Replace ("?", "");
				indexForText++;
				generalSymptomsText [indexForText].text = "Yes";
				indexForText++;
				if (GeneralSymptom.name.Contains ("throw")) {
					generalSymptomsText [indexForText].text = "How many times ?" ;
					indexForText++;
					generalSymptomsText [indexForText].text =  GeneralSymptom.painScale.ToString("0");
					indexForText++;
				}
			}

		
		}
		for (int i = indexForText; i < generalSymptomsText.Count; i++)
			generalSymptomsText [i].text = "";

	}

	void populateListOfDates(string month,string year)
	{

		foreach (Button btn in listOfDates)
			GameObject.Destroy (btn.gameObject);
		listOfDates.Clear ();
		List<string> dateEntries = new List<string> ();
		foreach (string keys in SymptomsMap.Keys) {
			string[] dateSplit	= keys.Split ('_');
			if (year == dateSplit [2] && month == dateSplit [0]) 
			{
				dateEntries.Add (keys.Replace ("_", "/"));

			}
		}
		 
		CLayerListOfDates.GetComponent<RectTransform>().sizeDelta = new Vector2(0,dateEntries.Count * dateSelectorReference.gameObject.GetComponent<RectTransform>().rect.width);
		float postionY = CLayerListOfDates.GetComponent<RectTransform>().rect.height/2 - dateSelectorReference.gameObject.GetComponent<RectTransform>().rect.height;
		foreach (string date in dateEntries) {
			
			Button dateB = GameObject.Instantiate (dateSelectorReference) as Button;
			dateB.GetComponentInChildren<Text> ().text = date;
			dateB.gameObject.transform.SetParent (CLayerListOfDates.transform);
			dateB.transform.localScale = Vector3.one;
			dateB.gameObject.SetActive (true);
			dateB.gameObject.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (dateSelectorReference.gameObject.GetComponent<RectTransform>().anchoredPosition.x,postionY);
			postionY -= dateSelectorReference.gameObject.GetComponent<RectTransform>().rect.height;
			listOfDates.Add (dateB);
		}
			
	}

	public void goToTrends()
	{
		SceneManager.LoadScene ("Graphing");
	}

	public void createPDFReport()
	{
		if (currentSymptoms.Keys.Count <= 0)
			return;
		sendMailProgressBar.SetActive (true);
		FindObjectOfType<PdfExporter> ().CreatePDF (currentSymptoms);
		sendMailProgressBar.SetActive (false);
		SentImage.SetActive (true);
	}
		
	void LoadSymptomsJson()
	{
		
		string filePath = "Symptoms.json";
		string fileName = Application.persistentDataPath + "/Color" + filePath;
		if (!System.IO.File.Exists (fileName))return;
		string rawjson = System.IO.File.ReadAllText(fileName);
		Debug.Log("is file present");
		JSONObject mainJson = new JSONObject(rawjson);
		JSONObject Entries = mainJson.GetField("Entires");
		foreach (JSONObject obj in Entries.list) {
			List<BodyPartsTable> table = new List<BodyPartsTable> ();
			if (obj.GetField ("BodyParts").list == null)
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
				bodyPart.setTimeStamp (bodypartJson.GetField("timestamp").str);
				JSONObject symptoms = bodypartJson.GetField ("symptoms");
				foreach(JSONObject sy in symptoms.list)
				{
					symptoms symptom = new symptoms ();
					symptom.name = sy.GetField("symptomname").str;
					symptom.painScale = sy.GetField("painscale").f ;
					symptom.botherScale = sy.GetField("bothersomescale").f ;
					bodyPart.addSymptoms (symptom);
				}
				table.Add (bodyPart);
			}
			if (SymptomsMap.ContainsKey (obj.GetField ("Date").str))
				SymptomsMap [obj.GetField ("Date").str] = table;
			else
			SymptomsMap.Add (obj.GetField ("Date").str, table);
		}
			
	}

	public string symptomPointsToText(int value)
	{
		switch (value)
		{
		case 1:
			return "Mild";
		case 2 : return "Moderate";
		case 3 :return "Severe";
		default :return "None" ;
		}
	}



}
