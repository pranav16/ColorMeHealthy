using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using System.Collections.Generic;
public class HandleNotes : MonoBehaviour {

   private string memoText;
    // Use this for initialization
    InputField memoBox;
	private string smileySelected;
	public GameObject selectedImage;
	public AudioSource memoAudioSource;
	public List<Button> smileys;

	private List<string> dropText;
	public List<Toggle> taskMarker;
	public TextAsset fixedDailyGoalsText;
	public List<Dropdown> FixedDailyGoals;
	public List<InputField> dailyGoals;
	public List<int> indexesForFixedGoals;

	void Start () {
		Analytics.CustomEvent ("Dairy",new Dictionary<string, object>());
         memoBox = GetComponent<InputField>();
		dropText = new List<string> ();
		//string  fileName = Application.persistentDataPath + "/Color" + "Dairy.json";
		//System.IO.File.Delete (fileName);
		populateDowndowns ();
		loadDiary ();
	
	

	}
	
	// Update is called once per frame
	void Update () {
	
	

	}
	public void unlockStickersForTaskCompletion()
	{
		int dayOfYear = PlayerPrefs.GetInt ("DailyTaskDate", System.DateTime.Now.DayOfYear - 1);
		if (dayOfYear < System.DateTime.Now.DayOfYear) {
			int stickerUnlocked = PlayerPrefs.GetInt ("stickersunlocked", 1);
			stickerUnlocked++;
			PlayerPrefs.SetInt ("DailyTaskDate", System.DateTime.Now.DayOfYear);
			PlayerPrefs.SetInt ("stickersunlocked", stickerUnlocked);
			PlayerPrefs.SetInt ("unlockedStickers", 1);
			PlayerPrefs.Save ();

		}
	}
    public void onTextChanged()
    {
        memoText = memoBox.text;
        Debug.Log("text typed: " + memoText);
	
     
    }
	void populateDowndowns()
	{
		string fixedDailyGoalsJson = fixedDailyGoalsText.text;
		JSONObject json = new JSONObject (fixedDailyGoalsJson);
		JSONObject fields = json.GetField ("Entries");
		foreach (JSONObject obj in fields.list) {
			dropText.Add (obj.str);
		}
		int currentMarker = 0;
		for(int i = 0 ;i <FixedDailyGoals.Count;i++)
		{
			
			FixedDailyGoals[i].AddOptions(dropText.GetRange(currentMarker,indexesForFixedGoals[i]));
			currentMarker = indexesForFixedGoals [i];
		}


	}
    public void onButtonClicked()
    {
		memoAudioSource.Play ();
		//Camera.main.GetComponent<AudioSource> ().Play();
        Debug.Log("on button clicked");
        string  fileName = Application.persistentDataPath + "/Color" + "Dairy.json";
        if (System.IO.File.Exists(fileName))
        {
            editJson();
            
        }
        else
        {
            buildJson();
        }
		int isTaskDone = 0;
		for (int i = 0; i < taskMarker.Count; i++) {
			if (taskMarker [i].isOn)
			{
			isTaskDone++;
			}
		}
		if (isTaskDone >= 4)
			unlockStickersForTaskCompletion ();
		if(isTaskDone >= 6)
			unlockStickersForTaskCompletion ();

		Analytics.CustomEvent("Dairy Used", new Dictionary<string, object>
			{
				{ "Input Size", memoBox.text.Length}
			});
        memoBox.text = "";
       // SceneManager.LoadScene("MainSelectionScreen");


    }


	void OnDestroy() {

		onButtonClicked ();
	}

    void buildJson()
    {
        string fileName = Application.persistentDataPath + "/Color" + "Dairy.json";
        JSONObject mainJson = new JSONObject();
        JSONObject entries = new JSONObject(JSONObject.Type.ARRAY);
        mainJson.AddField("Entires", entries);
        JSONObject data = new JSONObject();
        data.AddField("Date", System.DateTime.Now.ToString("MM_dd_yyyy"));
        data.AddField("Text", memoBox.text);
		data.AddField ("Mood", smileySelected);
	
		for (int i = 0; i < dailyGoals.Count; i++) {
			data.AddField ("Goal" + i, dailyGoals [i].text);

		}
		for (int i = 0; i < FixedDailyGoals.Count; i++) {
			data.AddField ("FixedGoal" + i, FixedDailyGoals [i].value);

		}
		for (int i = 0; i < taskMarker.Count; i++) {
			data.AddField("TaskMarker" + i,taskMarker[i].isOn);
		}


        entries.Add(data);
        System.IO.File.WriteAllText(fileName, mainJson.Print());
    }

	void loadDiary()
	{
		string filePath = "Dairy.json";
		string fileName = Application.persistentDataPath + "/Color" + filePath;
		if (!System.IO.File.Exists (fileName))
			return;
		string rawjson = System.IO.File.ReadAllText(fileName);
		JSONObject mainJson = new JSONObject(rawjson);
		JSONObject entries = mainJson.GetField("Entires");
		foreach (JSONObject obj in entries.list) {
			if (obj.GetField ("Date").str == System.DateTime.Now.ToString ("MM_dd_yyyy")) {
				memoBox.text = obj.GetField ("Text").str;
				foreach (Button btn in smileys) {
					if (btn.image.name == obj.GetField ("Mood").str)
					 	selectedImageSetter (btn.transform.position);	
				}
			   
			}
			for (int i = 0; i < FixedDailyGoals.Count; i++) {
				FixedDailyGoals[i].value = (int)obj.GetField("FixedGoal" + i).i;
			}
			for (int i = 0; i < taskMarker.Count; i++) {
				taskMarker[i].isOn = obj.GetField("TaskMarker" + i).b;
			}
			for (int i = 0; i < dailyGoals.Count; i++) {
				dailyGoals[i].text = obj.GetField ("Goal" + i).str;

			}
		}
	}

    void editJson()
    {
        string filePath = "Dairy.json";
        string fileName = Application.persistentDataPath + "/Color" + filePath;
        string rawjson = System.IO.File.ReadAllText(fileName);
        Debug.Log("is file present");
        JSONObject mainJson = new JSONObject(rawjson);
        JSONObject Entries = mainJson.GetField("Entires");
		foreach (JSONObject obj in Entries.list) {
			if (obj.GetField ("Date").str == System.DateTime.Now.ToString ("MM_dd_yyyy")) {
				obj.GetField ("Text").str = memoBox.text;
				obj.GetField ("Mood").str = smileySelected;
				Debug.Log (mainJson.Print ());
				//System.IO.File.WriteAllText (fileName, mainJson.Print ());
				//return;
			}
		}
        JSONObject data = new JSONObject();
        data.AddField("Date", System.DateTime.Now.ToString("MM_dd_yyyy"));
        data.AddField("Text", memoBox.text);
		data.AddField ("Mood", smileySelected);
		for (int i = 0; i < dailyGoals.Count; i++) {
			data.AddField ("Goal" + i, dailyGoals [i].text);

		}
		for (int i = 0; i < FixedDailyGoals.Count; i++) {
			data.AddField ("FixedGoal" + i, FixedDailyGoals [i].value);

		}
		for (int i = 0; i < taskMarker.Count; i++) {
			data.AddField("TaskMarker" + i,taskMarker[i].isOn);
		}

        Entries.Add(data);
        System.IO.File.WriteAllText(fileName, mainJson.Print());
    }

     public void backButtonClicked()
    {
		memoBox.text = memoText = "";
		selectedImage.SetActive (false);
		smileySelected = "";
		return;
        //SceneManager.LoadScene("MainSelectionScreen");
		//Camera.main.GetComponent<AudioSource> ().Play();
    }

	public void smileyClicked(Button button)
	{
	
		smileySelected = button.name;
		Analytics.CustomEvent("Smiley Used", new Dictionary<string, object>
			{
				{ "Simley Name", button.name}
			});
		selectedImageSetter (button.transform.position);
		Camera.main.GetComponent<AudioSource> ().Play();
	}

	void selectedImageSetter(Vector3 position)
	{
		selectedImage.SetActive (true);
		selectedImage.transform.position = position;
			

	}

}
