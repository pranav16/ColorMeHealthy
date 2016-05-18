using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

public class HistoryPage : MonoBehaviour {
	 struct MemoDescriptor
	{
		public string date;
		public string text;
		public string mood;

	};

	struct SymptomsDescriptor
	{
		public string date;
		public List<BodyPartsTable> bodyParts;

	};

	public GameObject dateListContentView;
	public Text selectedDate;
	public Dropdown monthSelected;
	public Dropdown yearsSelected;
	private Dictionary<string,MemoDescriptor> memoMap;
	private Dictionary<string,SymptomsDescriptor> SymptomsMap;
	public Text memoText;
	public GameObject symptomsContentLayer;
	public List<GameObject>smiley;
	private string month;
	private string year;
	List<string> dates;
	List<GameObject>childerenInList;
	List<GameObject>symptomsStringObjects;
	public Font userDefinedFont;
	public Image symptomImage;
	private List<BodyPartsTable> currentSymptom;
	public GameObject generalSymptomContentView;
	public GameObject dairyContentLayer;
	public GameObject dateSelectedHiglight;
	public CanvasScaler scaler;
	// Use this for initialization
	void Start () {

		memoMap = new Dictionary<string, MemoDescriptor>();
		SymptomsMap = new Dictionary<string, SymptomsDescriptor> ();
		childerenInList = new List<GameObject> ();
		symptomsStringObjects = new List<GameObject> ();
		readDairyJson ();
		readSymptomsJson ();
		dates = new List<string> ();
		fillYears ();
		year = yearsSelected.options[yearsSelected.options.Count -1].text;
		System.DateTime now = 	System.DateTime.Now;
		Debug.Log(now.Month.ToString());
		month = "0"+ now.Month.ToString();
		fillListForDate (month, year);

		monthSelected.value = now.Month -1;
		

		Analytics.CustomEvent("History Page",new Dictionary<string, object>());

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void readDairyJson()
	{
		string filePath = "Dairy.json";
		string fileName = Application.persistentDataPath + "/Color" + filePath;
		if (!System.IO.File.Exists (fileName))return;
		string rawjson = System.IO.File.ReadAllText(fileName);
		Debug.Log("is file present");
		JSONObject mainJson = new JSONObject(rawjson);
		JSONObject Entries = mainJson.GetField("Entires");
		foreach (JSONObject memo in Entries.list) {
			MemoDescriptor descriptor;
			descriptor.date = memo.GetField ("Date").str;
			descriptor.text = memo.GetField ("Text").str;
			descriptor.mood = memo.GetField ("Mood").str;
			memoMap [descriptor.date] = descriptor;
			//memoMap.Add (descriptor.date, descriptor);

		}

	}

	void readSymptomsJson()
	{
		Debug.Log("is file present");
		string filePath = "Symptoms.json";
		string fileName = Application.persistentDataPath + "/Color" + filePath;
		if (!System.IO.File.Exists (fileName))return;
		string rawjson = System.IO.File.ReadAllText(fileName);
		Debug.Log("is file present");
		JSONObject mainJson = new JSONObject(rawjson);
		JSONObject Entries = mainJson.GetField("Entires");
		foreach (JSONObject obj in Entries.list) {
			SymptomsDescriptor desciptor;
			desciptor.date = obj.GetField("Date").str;
			desciptor.bodyParts = new List<BodyPartsTable> ();

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
				desciptor.bodyParts.Add (bodyPart);
			}
		   
			SymptomsMap[desciptor.date] = desciptor;
		}


		}
	

	void deleteOldObjects()
	{
		foreach (GameObject obj in childerenInList)
			Destroy (obj);
		childerenInList.Clear();
	}
	void fillListForDate(string month,string year)
	{
		deleteOldObjects ();
		float positionY = (dateListContentView.GetComponent<RectTransform> ().rect.size.y) / 2;
		foreach (string key in memoMap.Keys) {

			MemoDescriptor desciptor = memoMap [key];
			string[] dateSplit	= desciptor.date.Split('_');
		
			if (month == dateSplit [0] && year == dateSplit [2]) {

				GameObject obj =	createButton (desciptor.date);
				positionY -= obj.GetComponent<RectTransform> ().rect.size.y;
				obj.GetComponent<RectTransform> ().anchoredPosition = new Vector2(0, positionY);
				float diffHeight = obj.GetComponent<RectTransform> ().rect.y/2;
				GameObject img = createSmiley (desciptor.mood);
				if (img) {
					img.GetComponent<RectTransform> ().anchoredPosition = new Vector2(-obj.GetComponent<RectTransform>().rect.width/2 - 15, positionY);
					float diffY = img.GetComponent<RectTransform> ().rect.size.y - obj.GetComponent<RectTransform> ().rect.size.y;
				    positionY -= diffY;
				}

			}


		}
		foreach (string key in SymptomsMap.Keys) {

			SymptomsDescriptor desciptor = SymptomsMap [key];
			if (memoMap.ContainsKey (desciptor.date))
				continue;

			string[] dateSplit	= desciptor.date.Split('_');
		
			if (month == dateSplit [0] && year == dateSplit [2]) {

				GameObject obj =	createButton (desciptor.date);
				positionY -= obj.GetComponent<RectTransform> ().rect.size.y;
				obj.GetComponent<RectTransform> ().anchoredPosition = new Vector2(0, positionY);

			}


		}
		Analytics.CustomEvent("History Page Drop down Selected", new Dictionary<string, object>
			{
				{ "Month", month },
				{ "Year", year }

			});

	}
	void clearSymptomsText()
	{
		foreach (GameObject obj in symptomsStringObjects)
			Destroy (obj);
		symptomsStringObjects.Clear ();
	}

	public void BackButtonClicked()
	{
		Camera.main.GetComponent<AudioSource> ().Play();
		SceneManager.LoadScene("MainSelectionScreen");
	}

	public void SymptomClicked(Button button)
	{
		string imageSrc = "";
		Analytics.CustomEvent("History Page Symptom Selected", new Dictionary<string, object>
			{
				{ "SymptomSlected", button.name }
			});
		foreach (BodyPartsTable tuple in currentSymptom) {
			if (button.name == tuple.getPartName ()) {
				imageSrc = tuple.getImagePath();
				break;
			}
		}
		changeImage(imageSrc);
	}

	void changeImageBasedOnFirstSymptom()
	{
		string imageSrc = "";
		foreach (BodyPartsTable tuple in currentSymptom) {

				imageSrc = tuple.getImagePath();
				break;

		}
		changeImage(imageSrc);
	}
	void buttonClicked(Button button)
	{
		clearSymptomsText ();
		memoText.text = "";
		selectedDate.text = button.name.Replace("_","/");
		Analytics.CustomEvent("History Page Date Selected", new Dictionary<string, object>
			{
				{ "Date", button.name }

			});
		dateSelectedHiglight.SetActive (true);
		dateSelectedHiglight.GetComponent<RectTransform> ().anchoredPosition = button.GetComponent<RectTransform> ().anchoredPosition;
		currentSymptom = null;
		if(memoMap.ContainsKey(button.name))
			CreateDairy (button.name);
		
		if (SymptomsMap.ContainsKey (button.name)) {
			currentSymptom = SymptomsMap[button.name].bodyParts;
			int numberofElements = SymptomsMap[button.name].bodyParts.Count - 1;
			int numberOfSymptoms = 0;
			foreach (BodyPartsTable part in SymptomsMap[button.name].bodyParts) {
				if (part.getPartName () == "MainSymptoms")
					continue;
				numberOfSymptoms += (part.getSymptoms ().Count * 3);
			}
			//numberOfSymptoms -= numberofElements;
			GameObject HeaderHeight = createButtonSymptoms ("fontHeight");
			GameObject subNodeHeight = creatDynamicText ("Pain: Medium", false);
			RectTransform transform =  symptomsContentLayer.GetComponent<RectTransform> ();
			float sizeY = HeaderHeight.GetComponent<RectTransform> ().rect.height * numberofElements + subNodeHeight.GetComponent<RectTransform> ().rect.height * numberOfSymptoms  ;
			transform.sizeDelta = new Vector2 (0, sizeY);
			float positionY = sizeY /2;
			Destroy (HeaderHeight);
			Destroy (subNodeHeight);
			if(currentSymptom == null)
			currentSymptom = SymptomsMap[button.name].bodyParts;
			foreach (BodyPartsTable tuple in SymptomsMap[button.name].bodyParts) {
				if (tuple.getPartName () == "MainSymptoms") {
					fillGeneralSymptoms (tuple);
					continue;
				}
				GameObject obj = createButtonSymptoms (tuple.getPartName ());
				positionY -= obj.GetComponent<RectTransform> ().rect.size.y;
				obj.GetComponent<RectTransform> ().anchoredPosition = new Vector2(0, positionY);
				symptomsStringObjects.Add (obj);
				foreach (symptoms s in tuple.getSymptoms()) {
					GameObject sobj = creatDynamicText(s.name,true);
					positionY -= sobj.GetComponent<RectTransform> ().rect.size.y;
					sobj.GetComponent<RectTransform> ().anchoredPosition = new Vector2(0, positionY);
					symptomsStringObjects.Add (sobj);
					GameObject pobj = creatDynamicText("How much:" + painScaleToString((int)s.painScale),false);
					positionY -= pobj.GetComponent<RectTransform> ().rect.size.y;
					pobj.GetComponent<RectTransform> ().anchoredPosition = new Vector2(0, positionY);
					symptomsStringObjects.Add (pobj);
					GameObject bobj = creatDynamicText("Bother:" + BotherScaleToString((int)s.botherScale) ,false);
					positionY -= bobj.GetComponent<RectTransform> ().rect.size.y;
					bobj.GetComponent<RectTransform> ().anchoredPosition = new Vector2(0, positionY);
					symptomsStringObjects.Add (bobj);
				}
			}
		}
		changeImageBasedOnFirstSymptom();
	}

	void fillYears()
	{
		string previousDate = "2016";
		dates.Add(previousDate);
		foreach (string key in memoMap.Keys) {

			MemoDescriptor desciptor = memoMap [key];
			if (desciptor.date == null)
				return;
			string[] dateSplit	= desciptor.date.Split('_');
			if (dateSplit [2] != previousDate) {
				previousDate = dateSplit [2];
				dates.Add (previousDate);
			}


		}
		if(dates != null)
		yearsSelected.AddOptions(dates);
	}

	public void yearChanged(int value)
	{
		year = dates[value];
		fillListForDate (month, year);
	}
	public string monthWordToNumber(string value)
	{
		switch (value) {

		case "Jan":
			return "01";
		case "Feb":
			return "02";
		case "March":
			return "03";
		case "April":
			return "04";
		case "May":
			return "05";
		case "June":
			return "06";
		case "July":
			return "07";
		case "Aug":
			return "08";
		case "Sept":
			return "09";
		case "Oct":
			return "10";
		case "Nov":
			return "11";
		case "Dec":
			return "12";
		default:
			return "01";	

		}

	}
	public void monthChanged(int value)
	{
		month = monthSelected.options[value].text;

		month = monthWordToNumber (month);
		fillListForDate (month, year);
	}
	public void CreateDairy(string date)
	{
		if (!memoMap.ContainsKey (date))return;	
		memoText.text = memoMap[date].text;
		RectTransform transform =  memoText.GetComponent<RectTransform>();
		float width = transform.rect.width;
		transform.sizeDelta = new Vector2 (width, memoText.preferredHeight);
		RectTransform dairyTransform = dairyContentLayer.GetComponent<RectTransform> ();
		Vector2 sizeOfContentLayer = new Vector2 (0.0f, memoText.preferredHeight);// - dairyTransform.rect.height);
		dairyTransform.sizeDelta = sizeOfContentLayer;
		transform.anchoredPosition = new Vector2 (0.0f,sizeOfContentLayer.y/2);
	}

	GameObject createButton(string value)
	{
		GameObject obj = new GameObject ();
		obj.transform.SetParent (dateListContentView.gameObject.transform);
    	obj.AddComponent<CanvasRenderer>();
		RectTransform rectTransform =  obj.AddComponent<RectTransform> ();
    	Button button = obj.AddComponent<Button> ();
		Text text = obj.AddComponent<Text> ();
		text.text = value.Replace("_","/");
		text.font = Font.CreateDynamicFontFromOSFont ("Arial", 25);//userDefinedFont;
		//text.fontStyle = FontStyle.Bold;
		text.fontSize = 25;
		text.color = Color.black;
		text.alignment = TextAnchor.MiddleCenter;
		button.name = value;
		button.onClick.AddListener(() =>buttonClicked(button) );
		rectTransform.sizeDelta = new Vector2(text.preferredWidth,text.preferredHeight);
		childerenInList.Add (obj);
		return obj;
	}

	GameObject createButtonSymptoms(string value)
	{
		GameObject obj = new GameObject ();
		obj.transform.SetParent (symptomsContentLayer.gameObject.transform);
		obj.AddComponent<CanvasRenderer>();
		RectTransform rectTransform =  obj.AddComponent<RectTransform> ();
		Button button = obj.AddComponent<Button> ();
		Text text = obj.AddComponent<Text> ();
		text.text = value;
		text.font = Font.CreateDynamicFontFromOSFont ("Arial", 15);
		text.fontStyle = FontStyle.Bold;
		text.fontSize = 25;
		text.color = Color.black;
		button.name = value;
		button.onClick.AddListener(() =>SymptomClicked(button) );
		rectTransform.sizeDelta = new Vector2(text.preferredWidth,text.preferredHeight);
		childerenInList.Add (obj);
		return obj;
	}
	GameObject createSmiley(string value)
	{
		if (value == "")
			return null;
		GameObject obj = new GameObject ();
		obj.transform.SetParent (dateListContentView.gameObject.transform);
		obj.AddComponent<CanvasRenderer>();
		RectTransform rectTransform =  obj.AddComponent<RectTransform> ();
		Image img = obj.AddComponent<Image> ();
		foreach(GameObject sm in smiley)
		{
			if(sm.name == value)
			{
				img.sprite = sm.GetComponent<SpriteRenderer>().sprite;
				break;
			}
		}
		rectTransform.sizeDelta = new Vector2(30,30);
		childerenInList.Add (obj);
		return obj;
	}
	public GameObject creatDynamicText(string value,bool header)
	{
		GameObject obj = new GameObject ();
		obj.transform.SetParent (symptomsContentLayer.gameObject.transform);
		obj.AddComponent<CanvasRenderer>();
		RectTransform rectTransform =  obj.AddComponent<RectTransform> ();
		Text text = obj.AddComponent<Text> ();
		text.font = Font.CreateDynamicFontFromOSFont ("Arial", 30);
		text.text = value;
		//text.fontStyle = FontStyle.Bold;
		text.alignment = TextAnchor.MiddleCenter;
		text.color = Color.black;
		text.fontSize = 20;
		if (header) {
			text.fontStyle = FontStyle.Bold;
			text.fontSize = 20;

		}
		text.horizontalOverflow = HorizontalWrapMode.Wrap;
		text.verticalOverflow = VerticalWrapMode.Overflow;
		float sizeX = symptomsContentLayer.GetComponent<RectTransform> ().rect.width - 20;
	    float sizeY = 50.0f;
		sizeX *= scaler.scaleFactor;
		sizeY *= scaler.scaleFactor;
		rectTransform.sizeDelta = new Vector2 (sizeX, sizeY);
		return obj;
	}
	public GameObject creatDynamicTextGS(string value,bool header)
	{
		
		GameObject obj = new GameObject ();
		obj.transform.SetParent (generalSymptomContentView.gameObject.transform);
		obj.AddComponent<CanvasRenderer> ();
		RectTransform rectTransform = obj.AddComponent<RectTransform> ();
		Text text = obj.AddComponent<Text> ();
		text.font = Font.CreateDynamicFontFromOSFont ("Arial", 15);
		text.text = value;
		//text.fontStyle = FontStyle.Bold;
		text.color = Color.black;
		text.fontSize = 25;
		text.alignment = TextAnchor.MiddleCenter;
		if (header) {
			text.fontStyle = FontStyle.Bold;
		}
		text.horizontalOverflow = HorizontalWrapMode.Wrap;
		text.verticalOverflow = VerticalWrapMode.Overflow;
		float sizeX = generalSymptomContentView.GetComponent<RectTransform> ().rect.width - 20;
		Rect test = generalSymptomContentView.GetComponent<RectTransform> ().rect;
		float sizeY = 80.0f;
		sizeX *= scaler.scaleFactor;
		sizeY *= scaler.scaleFactor;
		rectTransform.sizeDelta = new Vector2 (sizeX, sizeY);
		return obj;
	}

	public void fillGeneralSymptoms(BodyPartsTable table)
	{
		
		int numberofElements = 1;
		int numberOfSymptoms = table.getSymptoms ().Count + 20 ;

		//numberOfSymptoms -= numberofElements;
		GameObject HeaderHeight = creatDynamicTextGS("Main Symptoms",true);
		GameObject subNodeHeight = creatDynamicTextGS("Pain Medium ?", false);
		RectTransform transform =  generalSymptomContentView.GetComponent<RectTransform> ();
		float sizeY = HeaderHeight.GetComponent<RectTransform> ().rect.height * numberofElements + subNodeHeight.GetComponent<RectTransform> ().rect.height * numberOfSymptoms;
		transform.sizeDelta = new Vector2 (0, sizeY);

		float positionY = sizeY /2;

		Destroy (HeaderHeight);
		Destroy (subNodeHeight);


		GameObject obj = creatDynamicTextGS (table.getPartName(),true);
		positionY -= (obj.GetComponent<RectTransform> ().rect.size.y);
		obj.GetComponent<RectTransform> ().anchoredPosition = new Vector2(0, positionY);
		symptomsStringObjects.Add (obj);

		foreach (symptoms s in table.getSymptoms()) {
			//string text = "";
			if (s.name.Contains ("os_")) {
				GameObject sobj2 = creatDynamicTextGS ("Any other symptoms?", true);
				positionY -= (sobj2.GetComponent<RectTransform> ().rect.size.y );
				sobj2.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, positionY);
				symptomsStringObjects.Add (sobj2);
				GameObject sobj1 = creatDynamicTextGS (s.name.Replace ("os_", ""), false);
				positionY -= (sobj1.GetComponent<RectTransform> ().rect.size.y );
				sobj1.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, positionY);
				symptomsStringObjects.Add (sobj1);
				continue;
			}
			if (s.name.Contains ("Bothersome_")) {
				GameObject sobj2 = creatDynamicTextGS ("What is bothering you the most?", true);
				positionY -= (sobj2.GetComponent<RectTransform> ().rect.size.y );
				sobj2.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, positionY);
				symptomsStringObjects.Add (sobj2);
				GameObject sobj1 = creatDynamicTextGS (s.name.Replace ("Bothersome_", ""), false);
				positionY -= (sobj1.GetComponent<RectTransform> ().rect.size.y );
				sobj1.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, positionY);
				symptomsStringObjects.Add (sobj1);
				continue;
			}
			if (s.name.Contains ("FeelingToday_")) {
				GameObject sobj2 = creatDynamicTextGS ("How are you feeling today?", true);
				positionY -= (sobj2.GetComponent<RectTransform> ().rect.size.y);
				sobj2.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, positionY);
				symptomsStringObjects.Add (sobj2);
				GameObject sobj1 = creatDynamicTextGS (s.name.Replace ("FeelingToday_", ""), false);
				positionY -= (sobj1.GetComponent<RectTransform> ().rect.size.y);
				sobj1.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, positionY);
				symptomsStringObjects.Add (sobj1);
				continue;
			}
			if (s.name.Contains ("Bestthing_")) {
				GameObject sobj2 = creatDynamicTextGS ("What is the best thing about today?", true);
				positionY -= (sobj2.GetComponent<RectTransform> ().rect.size.y);
				sobj2.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, positionY);
				symptomsStringObjects.Add (sobj2);
				GameObject sobj1 = creatDynamicTextGS (s.name.Replace ("Bestthing_", ""), false);
				positionY -= (sobj1.GetComponent<RectTransform> ().rect.size.y );
				sobj1.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, positionY);
				symptomsStringObjects.Add (sobj1);
				continue;
			}
			if (s.botherScale >= 0) {
				GameObject sobj1 = creatDynamicTextGS (s.name.Replace ("?", "? Yes"), true);
				positionY -= (sobj1.GetComponent<RectTransform> ().rect.size.y );
				sobj1.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, positionY);
				GameObject spain = creatDynamicTextGS ("How much: " + painScaleToString ((int)s.painScale), false);
				positionY -= (spain.GetComponent<RectTransform> ().rect.size.y);
				spain.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, positionY);
				GameObject sbothersome = creatDynamicTextGS ("Bother: " + BotherScaleToString ((int)s.botherScale), false);
				positionY -= (sbothersome.GetComponent<RectTransform> ().rect.size.y);
				sbothersome.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, positionY);
				symptomsStringObjects.Add (sobj1);
				symptomsStringObjects.Add (spain);
				symptomsStringObjects.Add (sbothersome);
				continue;
			} else {
				if (s.name.Contains ("?") && s.botherScale != -1.0f)
					continue;
				GameObject sobj = creatDynamicTextGS (s.name.Replace("?","? Yes"), true);
				positionY -= (sobj.GetComponent<RectTransform> ().rect.size.y);
				sobj.GetComponent<RectTransform> ().anchoredPosition = new Vector2(0, positionY);
				//float x = sobj.GetComponent<RectTransform> ().rect.size.x;
				symptomsStringObjects.Add (sobj);
				if (s.name.Contains ("throw")) {
					float times = s.painScale + 1;
					GameObject Times = creatDynamicTextGS ("How many times have you thrown up?" + times.ToString("0"), true);
					positionY -= (Times.GetComponent<RectTransform> ().rect.size.y);
					Times.GetComponent<RectTransform> ().anchoredPosition = new Vector2(0, positionY);
					//float x = sobj.GetComponent<RectTransform> ().rect.size.x;
					symptomsStringObjects.Add (Times);
				}
					
			}

	
		}


	}

	public string painScaleToString(int scale)
	{
		switch (scale) {
		case 0:
			return "Little";

		case 1:
			return "Medium";

		case 2:
			 return "A Lot";
		}
		return "";
	}

	public string BotherScaleToString(int scale)
	{
		switch (scale) {
		case 0:
			return "None";

		case 1:
			return "Little";

		case 2:
			return "Medium";
		case 3:
			return "A Lot";
		}
		return "";
	}

	public void changeImage(string path)
	{
		if(!System.IO.File.Exists(path)) return;
		byte [] textureData = System.IO.File.ReadAllBytes (path);
		Texture2D tex = new Texture2D(Screen.width/2,Screen.height);
		tex.LoadImage (textureData);
		Sprite sprite = Sprite.Create (tex,new Rect(0,0,Screen.width/2,Screen.height),new Vector2(0.5f,0.5f));
		symptomImage.sprite = sprite;
	}


}
