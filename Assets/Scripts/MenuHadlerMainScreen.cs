//Writtern by Pranav S Nayak
//nayak.pranav@gmail.com
//loads player customiaztion
//makes char click traversal to symptoms page
//works on plant behavior

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


public class MenuHadlerMainScreen : MonoBehaviour {


	public List<GameObject>bodyParts;
	public GameObject waterCan;


	// Use this for initialization
	void Start () {
		//PlayerPrefs.DeleteAll ();
		FindObjectOfType<AnalyticsSystem> ().CustomEvent("Main  Menu",new Dictionary<string, object>());
		
		loadAndSetCustomization ();
		//int dayOfYear = PlayerPrefs.GetInt("dayofyeardailyreward",System.DateTime.Now.DayOfYear - 1 );
		int currentDayOfYear = System.DateTime.Now.DayOfYear;
	

	}
	
	// Update is called once per frame
	void Update () {
		loadAndSetCustomization ();
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast (ray, out hit)) { 
				if (hit.collider.name == "Present1"|| hit.collider.name == "Present2" || hit.collider.name == "Present3") {
					PlayerPrefs.SetInt ("dayofyeardailyreward",System.DateTime.Now.DayOfYear);
					int random = Random.Range (1, 3);
					int unlockedStickerCount = PlayerPrefs.GetInt ("unlockedStickerCount", -1);
					int unlockedAcessCount = PlayerPrefs.GetInt ("unlockedAcessCount", -1);
					if (random == 1 && unlockedStickerCount < 9) {
						
						PlayerPrefs.SetInt ("unlockedStickerCount", unlockedStickerCount + 1);
					} else if(unlockedAcessCount < 5)  {
						
						PlayerPrefs.SetInt ("unlockedAcessCount", unlockedAcessCount + 1);
					}
				
					PlayerPrefs.Save ();
					SceneManager.LoadScene ("DailyRewardsScreen");
					return;
				}
				else if (hit.collider.name == "Girl") {
					SceneManager.LoadScene ("SymptomsPageNew");
					return;
				} else if (hit.collider.name == "watering_can") {
				}
			}
                
    }
	}





	public void loadAndSetCustomization()
	{
		Camera.main.GetComponent<GenderSelector> ().setInitialStates();
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
