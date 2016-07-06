using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class TitleScreen : MonoBehaviour {

	int numberOfClicks;
	// Use this for initialization
	void Start () {
	// freshBuild ();
	}
	
	// Update is called once per frame
	void Update () {
	
	
	}

	public void startClicked()
	{
		SceneManager.LoadScene("MainSelectionScreen");
	}

	public void helpClicked()
	{
		SceneManager.LoadScene("HelpScreen");
	}

	public void freshBuild()
	{
		PlayerPrefs.DeleteAll ();
		System.IO.File.Delete ( Application.persistentDataPath + "/Color" + "StickersMain.json");
		System.IO.File.Delete ( Application.persistentDataPath + "/Color" + "Symptoms.json");
		System.IO.File.Delete ( Application.persistentDataPath + "/Color" + "CurrentCustomization.json");
		System.IO.File.Delete (Application.persistentDataPath + "/Color" + "QuestSaveData.json");
		System.IO.File.Delete ( Application.persistentDataPath + "/Color" + "Dairy.json");
		System.IO.File.Delete ( Application.persistentDataPath + "/Color" + "Personal.json");
		System.IO.File.Delete ( Application.persistentDataPath +"/color" + "Analytics.json");




	}
}
