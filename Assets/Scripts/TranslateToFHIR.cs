using UnityEngine;
using System.Collections;

public class TranslateToFHIR  {

	private const string snowMedLink = "http://snomed.info/sct";
	private TranslatorSnoMed translator;

	public bool init()
	{
		translator = new TranslatorSnoMed ();
		translator.Init();
		return true;
	}

	public void convertToFHIR(BodyPartsTable table)
	{
		if (table == null)
			return;
		foreach(symptoms s in table.getSymptoms())
			Debug.Log(LocalJsonToFHIR(table.getPartName(),s));
	}


	string LocalJsonToFHIR(string  partName ,symptoms symptom)
	{

		JSONObject FHIRJSON = new JSONObject ();
		FHIRJSON.AddField ("resourceType","Observation");
		JSONObject text = new JSONObject ();
		text.AddField ("div","human readable text");
		text.AddField ("status","generated");
		FHIRJSON.AddField ("text",text);
		JSONObject bodySite = new JSONObject ();
		JSONObject coding = new JSONObject (JSONObject.Type.ARRAY);
		bodySite.AddField ("coding", coding);
		JSONObject code = new JSONObject ();
		code.AddField ("system",snowMedLink);
		//snowmedid
		if(translator.isKeyPresent(partName))
		code.AddField ("code", translator.getSnoMedTermForUi(partName).id);
		//descrption
		code.AddField ("display",partName);
		coding.Add(code);
		FHIRJSON.AddField ("bodySite",bodySite);
		//date
		FHIRJSON.AddField ("issued", System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss+HH:mm"));
		JSONObject subject = new JSONObject ();
		//user name;
		subject.AddField("display","user name");
		FHIRJSON.AddField ("subject",subject);

		JSONObject codeK = new JSONObject();
		JSONObject codeCoding = new JSONObject(JSONObject.Type.ARRAY);
		codeK.AddField("coding",codeCoding);
		JSONObject codeTuple = new JSONObject ();
		codeTuple.AddField ("system",snowMedLink);
		//snomedid like pain etc
		if(translator.isKeyPresent(symptom.name))
		codeTuple.AddField ("code",translator.getSnoMedTermForUi(symptom.name).id);
		//descrption
		codeTuple.AddField ("display", symptom.name);
		codeCoding.Add (codeTuple);
		FHIRJSON.AddField ("code", codeK);


		JSONObject valueQuantity = new JSONObject ();
		FHIRJSON.AddField ("valueQuantity", valueQuantity);
		valueQuantity.AddField("system",snowMedLink);
		//snomed id
		valueQuantity.AddField ("code", symptomPointsToId((int)symptom.painScale));
		//descrption
		valueQuantity.AddField ("display",symptomPointsToText((int)symptom.painScale));
		string value = FHIRJSON.Print ();
		return FHIRJSON.Print ();
	}

	public string symptomPointsToText(int value)
	{
		switch (value)
		{
		case 1:
			return "Symptom mild (finding) ";
		case 2 : return "Symptom moderate (finding)";
		case 3 :return "Symptom severe (finding)";
		default :return "None" ;
		}
	}
	public long  symptomPointsToId(int value)
	{
		switch (value)
		{
		case 1:
			return 162468002 ;
		case 2 : return 162469005;
		case 3 :return 162470006;
		default :return 0 ;
		}
	}


}
