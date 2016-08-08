using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class GraphAggregator : MonoBehaviour {


	private Dictionary<string,List<BodyPartsTable>> dateToSymtpoms;
	private Dictionary<string,List<BodyPartsTable>> currentSelection;

	private string weekSelected;
	private string monthSelected;
	private string yearSelected;

	public Dropdown monthDropdown;
	public Dropdown yearDropdown;
	public Dropdown weekDropdown;

	// Use this for initialization
	void Start () {
		dateToSymtpoms = new Dictionary<string, List<BodyPartsTable>> ();
		currentSelection = new Dictionary<string, List<BodyPartsTable>> ();
		LoadSymptomsJson ();
		populateDateDropDowns ();

	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void monthChanged ()
	{
		monthSelected = monthDropdown.options [monthDropdown.value].text;
		aggregateEntriesForTheDate ();
	}

	public void yearChanged()
	{
		yearSelected = yearDropdown.options[yearDropdown.value].text;
		aggregateEntriesForTheDate ();
	}

	public void weekChanged()
	{
		weekSelected = weekDropdown.options [weekDropdown.value].text;
		aggregateEntriesForTheDate ();
	}


	private void aggregateEntriesForTheDate()
	{
		currentSelection.Clear ();
		foreach (string date in dateToSymtpoms.Keys) {
			string[] dateSplit	= date.Split ('_');
			string month = dateSplit[0];
			string year = dateSplit[2];
			if (monthSelected == month && yearSelected == year) {
				//if it hits the if part then your data set is not unique..which is not cool!
				if (currentSelection.ContainsKey (date))
					currentSelection [date] = dateToSymtpoms [date];
				else
					currentSelection.Add (date,dateToSymtpoms[date]);
			}
		}

	}

	bool populateDateDropDowns()
	{
		List<string> months = new List<string> ();
		List<string> years = new List<string> ();
		string previousMonth = "";
		string previousYear = "";
		foreach (string date in dateToSymtpoms.Keys) {
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
		monthDropdown.AddOptions (months);
		yearDropdown.AddOptions (years);
		monthSelected = monthDropdown.options [monthDropdown.value].text;
		yearSelected = yearDropdown.options[yearDropdown.value].text;
		return true;
	}


	void LoadSymptomsJson()
	{

		string filePath = "Symptoms.json";
		string fileName = Application.dataPath + "/document.json";//Application.persistentDataPath + "/Color" + filePath;
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
			if (dateToSymtpoms.ContainsKey (obj.GetField ("Date").str))
				dateToSymtpoms [obj.GetField ("Date").str] = table;
			else
				dateToSymtpoms.Add (obj.GetField ("Date").str, table);
		}

	}

}
