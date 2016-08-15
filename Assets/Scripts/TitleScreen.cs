using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using UnityEngine.SocialPlatforms;
public class TitleScreen : MonoBehaviour {

	int numberOfClicks;
	public Text debugText;
	// Use this for initialization
	void Start () {
		#if UNITY_EDITOR
	 	freshBuild ();
		#endif
	
	}
	
	// Update is called once per frame
	void Update () {
	
	
	}

	public void startClicked()
	{
		int isFtue = PlayerPrefs.GetInt ("isFTUE",1);
		if (isFtue == 1) {
			SceneManager.LoadScene ("HelpScreen");
				return;
		} 
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


	public void login()
	{

	}
}
