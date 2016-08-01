using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
public class TitleScreen : MonoBehaviour {

	int numberOfClicks;
	public Text debugText;
	// Use this for initialization
	void Start () {
	 	//freshBuild ();
		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
			// require access to a player's Google+ social graph (usually not needed)
			.RequireGooglePlus()
			.Build();

		PlayGamesPlatform.InitializeInstance(config);
		// recommended for debugging:
		PlayGamesPlatform.DebugLogEnabled = true;
		// Activate the Google Play Games platform
		PlayGamesPlatform.Activate();

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
		Social.localUser.Authenticate((bool success) => {
			if(success)
			{
				Debug.Log(Social.localUser.id);
				debugText.text = Social.localUser.id;

			}
			else
			{
				Debug.Log("failed");
				debugText.text = "fail";
			}
		});
	}
}
