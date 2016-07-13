using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
public class SymptomsHistory : MonoBehaviour {

	[System.Serializable]
	public struct UILocalSymptomNode
	{
		public Image parent;
		public Image symptomImage;
		public Text partName;
		public List<Text> symptomName;
		public List<Text>painSymptom;
		public List<Text>BotherSymptom;

	};

	public Dropdown monthSelector;
	public Dropdown yearSelector;
	public Button dateSelectorReference;
	public GameObject CLayerListOfDates;
	public List<UILocalSymptomNode>symptomsNode;
	public List<Text> generalSymptomsText;
	public List<Button> listOfDates;

	private Dictionary<string,List<BodyPartsTable>> SymptomsMap;
	void Start () {
		SymptomsMap = new Dictionary<string, List<BodyPartsTable>> ();
		listOfDates = new List<Button> ();
		initialize ();
	}
	

	void Update () {
	
	}

	bool initialize()
	{
		LoadSymptomsJson();
		populateDateDropDowns();
        string currentMonth = System.DateTime.Now.Month.ToString("00");
        string currentYear = System.DateTime.Now.Year.ToString("0000");
        populateListOfDates(currentMonth, currentYear);

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
		if (SymptomsMap.ContainsKey (id)) {
			List<BodyPartsTable> tables = SymptomsMap [id];
			for (int i = 0 ;i< tables.Count ;i ++) {
				BodyPartsTable table = tables [i];
				if (table.getPartName ().Contains ("General Symptoms")) {
					populateGeneralSymptoms (table);
					continue;
				}
				symptomsNode [i].partName.text = table.getPartName ();
				symptomsNode [i].parent.gameObject.SetActive (true);
				byte [] textureData = System.IO.File.ReadAllBytes (table.getImagePath());
				Texture2D tex = new Texture2D(400,400);
				tex.LoadImage (textureData);
				Sprite sprite = Sprite.Create (tex,new Rect(0,0,400,400),new Vector2(0.5f,0.5f));
				symptomsNode [i].symptomImage.sprite = sprite;

				for (int j = 0; j < table.getSymptoms ().Count; j++) {

					symptomsNode [i].BotherSymptom [j].text = table.getSymptoms()[j].botherScale.ToString("0.0");
					symptomsNode [i].painSymptom [j].text = table.getSymptoms()[j].painScale.ToString("0.0");
					symptomsNode [i].symptomName [j].text = table.getSymptoms () [j].name;

				}

			}

			for (int k = tables.Count - 1; k < symptomsNode.Count; k++) {
				symptomsNode [k].parent.gameObject.SetActive (false);
			}



		}

	}
		
	void populateGeneralSymptoms(BodyPartsTable generalSymptoms)
	{
		
		int indexForText = 0;
		foreach (symptoms GeneralSymptom in  generalSymptoms.getSymptoms ()) {
			generalSymptomsText [indexForText].text = "";
			if (GeneralSymptom.name.Contains ("os_")) {
				generalSymptomsText [indexForText].text = "Any other symptoms? ";
				indexForText++;
				generalSymptomsText [indexForText].text = GeneralSymptom.name.Replace ("os_", "");
				indexForText++;

			} else if (GeneralSymptom.name.Contains ("Bothersome_")) {
				generalSymptomsText [indexForText].text = "What is bothering you the most? ";
				indexForText++;
				generalSymptomsText [indexForText].text = GeneralSymptom.name.Replace ("Bothersome_", "");
				indexForText++;
			}
			else if (GeneralSymptom.name.Contains ("FeelingToday_")) {
				generalSymptomsText [indexForText].text = "How are you feeling today? ";
				indexForText++;
				generalSymptomsText [indexForText].text = GeneralSymptom.name.Replace ("FeelingToday_", "");
				indexForText++;
			}
			else if (GeneralSymptom.name.Contains ("Bestthing_")) {
				generalSymptomsText [indexForText].text = "What is the best thing about today? ";
				indexForText++;
				generalSymptomsText [indexForText].text = GeneralSymptom.name.Replace ("Bestthing_", "");
				indexForText++;
			}
			else if (GeneralSymptom.botherScale >= 0) {
				generalSymptomsText [indexForText].text = GeneralSymptom.name.Replace ("?", "?  Yes");
				indexForText++;
				generalSymptomsText [indexForText].text = "Pain: " + GeneralSymptom.painScale.ToString ("0.0") + " Bother: " + GeneralSymptom.botherScale.ToString ("0.0");
				indexForText++;
			}
			else if(GeneralSymptom.name.Contains ("?") && GeneralSymptom.botherScale == -1.0f)
			{
				generalSymptomsText [indexForText].text = GeneralSymptom.name.Replace ("?", "?  Yes");
				indexForText++;
				if (GeneralSymptom.name.Contains ("thrown")) {
					generalSymptomsText [indexForText].text = "How many times ?: " + GeneralSymptom.painScale.ToString("0.0");
					indexForText++;
				}
			}

		
		}

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
			dateB.gameObject.SetActive (true);
			dateB.gameObject.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (dateSelectorReference.gameObject.GetComponent<RectTransform>().anchoredPosition.x,postionY);
			postionY -= dateSelectorReference.gameObject.GetComponent<RectTransform>().rect.height;
			listOfDates.Add (dateB);
		}
			
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




}
