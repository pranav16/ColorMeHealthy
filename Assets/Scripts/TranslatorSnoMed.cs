using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct SnoMedData
{
	public string snoMedTerm;
	public long id;
}

public class TranslatorSnoMed : MonoBehaviour {



	Dictionary<string,SnoMedData>UiToSnoMedTerm;
	private string filePath;


	public bool Init()
	{
		UiToSnoMedTerm = new Dictionary<string, SnoMedData> ();
		filePath = Application.dataPath + "/SnoMedData.json";
		loadTable ();
		return true;
	}
	

	private void loadTable()
	{
		if (!System.IO.File.Exists (filePath))
			return;
		string fileData = System.IO.File.ReadAllText (filePath);
		JSONObject MainObject = new JSONObject(fileData);
		JSONObject dataField = MainObject.GetField ("Data");
		foreach (JSONObject SnoMedDataJson in dataField.list) {
			SnoMedData sno = new SnoMedData();
			sno.id = SnoMedDataJson.GetField ("id").i;
			sno.snoMedTerm = SnoMedDataJson.GetField ("term").str;
			UiToSnoMedTerm.Add (SnoMedDataJson.GetField ("key").str, sno);
		}
			
	}
	public bool isKeyPresent(string key)
	{
		return UiToSnoMedTerm.ContainsKey (key);
	}

	public SnoMedData getSnoMedTermForUi(string key)
	{
		return UiToSnoMedTerm[key];
	}



}
