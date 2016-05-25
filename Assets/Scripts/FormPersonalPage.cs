using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

public class FormPersonalPage : MonoBehaviour {

    public InputField name;
	public InputField color;
	public InputField schoolName;
	public InputField height;
	public InputField wight;
	public InputField animal;
	public InputField game;
	public InputField thingThatAnnoysMe;
	public InputField food;
	public InputField favoriteThingToDo;


	// Use this for initialization
	void Start () {
		

		loadDataBackUp ();	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void changeInNumericalFields()
	{
	
	}



	void loadDataBackUp()
	{
		string filePath = "Personal.json";
		string fileName = Application.persistentDataPath + "/Color" + filePath;
		if (System.IO.File.Exists (fileName)) {
		string rawjson = System.IO.File.ReadAllText(fileName);
			JSONObject json = new JSONObject (rawjson);
			if(	System.DateTime.Now.ToString("MM_dd_yyyy") != json.GetField("date").str)
				return;
			name.text =  json.GetField("name").str;
			color.text = json.GetField("color").str;
			schoolName.text = json.GetField("schoolname").str;
			height.text = json.GetField("height").str;
			wight.text = json.GetField("weight").str;
			animal.text = json.GetField("animal").str;
			game.text = json.GetField("game").str;
			thingThatAnnoysMe.text= json.GetField("annoy").str;
			food.text= json.GetField("food").str;
			favoriteThingToDo.text= json.GetField("thing").str;
		}

	}


	public void backClicked()
	{

		JSONObject json = new JSONObject ();
		json.AddField("name",name.text);
		json.AddField("color",color.text);
		json.AddField("schoolname",schoolName.text);
		json.AddField("height",height.text);
		json.AddField("weight",wight.text);
		json.AddField("animal",animal.text);
		json.AddField("game",	game.text);
	    json.AddField("annoy",thingThatAnnoysMe.text);
		json.AddField("food",food.text);
		json.AddField("thing",favoriteThingToDo.text);
		json.AddField("date",System.DateTime.Now.ToString("MM_dd_yyyy"));
		string filePath = "Personal.json";
		string fileName = Application.persistentDataPath + "/Color" + filePath;
		string serializedJson = json.Print();
		Debug.Log(json.Print());   
		System.IO.File.WriteAllText(fileName, serializedJson);
		Camera.main.GetComponent<ColorObjectHandler> ().saveCustomization ();
		Analytics.CustomEvent("Customisation Form", new Dictionary<string, object>
			{
				{ "name", name.text },
				{ "color", color.text },
				{ "animal", animal.text }
			});

		//SceneManager.LoadScene ("MainSelectionScreen");
	}

}
