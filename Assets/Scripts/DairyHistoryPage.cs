using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DairyHistoryPage : MonoBehaviour {

	public Dropdown monthSelector;
	public Dropdown yearSelector;
	public Button dateSelectorReference;
	public GameObject CLayerListOfDates;
	public Text dairyEntry;
	private Dictionary<string ,string> dateToMemo;
	public List<Button> listOfDates;

	// Use this for initialization
	void Start () {
		dateToMemo = new Dictionary<string, string> ();
		listOfDates = new List<Button> ();	
		readDairyJson ();
		populateDateDropDowns ();
        string currentMonth = System.DateTime.Now.Month.ToString("00");
        string currentYear = System.DateTime.Now.Year.ToString("0000");
        populateListOfDates(currentMonth,currentYear);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
	bool populateDateDropDowns()
	{
		List<string> months = new List<string> ();
		List<string> years = new List<string> ();
		string previousMonth = "";
		string previousYear = "";
		foreach (string date in dateToMemo.Keys) {
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
	public void yearValueChanged(int value)
	{
		string month = monthSelector.options [monthSelector.value].text;
		string year = yearSelector.options [value].text;
		populateListOfDates (month, year);
	}
	void populateListOfDates(string month,string year)
	{
		foreach (Button btn in listOfDates)
			GameObject.Destroy (btn.gameObject);
		listOfDates.Clear ();
		List<string> dateEntries = new List<string> ();
		foreach (string keys in dateToMemo.Keys) {
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

	public void dateSelected(Button button)
	{
		string id = button.GetComponentInChildren<Text> ().text.Replace ("/","_");
		dairyEntry.text = dateToMemo [id];

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
			string  date = memo.GetField ("Date").str;
			string  text = memo.GetField ("Text").str;
			dateToMemo [date] = text;
			//memoMap.Add (descriptor.date, descriptor);

		}

	}


}
