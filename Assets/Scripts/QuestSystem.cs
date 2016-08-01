using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct QuestData
{
	public string name;
	public string trackerId;
	public float questId;
	public bool  isDaily;
	public float duration;
	public float totalQuantity;
	public float currentCounter;
	public float previousUpdate;
	public string rewardId;
	public float rewardQuantity;
};

public class QuestSystem : MonoBehaviour
{
	public static QuestSystem instance = null;
	List<QuestData> currentQuests;

	void Start ()
	{
		string fileName = Application.persistentDataPath + "/Color" + "QuestSaveData.json";
		System.IO.File.Delete (fileName);
		if (instance == null) {
			instance = this;
			instance.init ();
		} else if (instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad (gameObject);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public bool init ()
	{
		currentQuests = new List<QuestData> ();
		loadBaseQuestDataTable ();
		loadSavedContex ();
		return true;
	}

	public  void updateCounters (string trackingId)
	{
		for (int i = 0; i < currentQuests.Count; i++) {
			QuestData quest = currentQuests [i];
			if (quest.trackerId == trackingId) {
				int currentDay = System.DateTime.Now.DayOfYear;
				if (quest.isDaily) {

					if (currentDay == quest.previousUpdate + 1) {
						quest.previousUpdate = currentDay;
						quest.currentCounter++;

					} else if (currentDay > quest.previousUpdate) {
						quest.previousUpdate = currentDay;
						quest.currentCounter = 1;
					}
						
				} else {
					quest.previousUpdate = currentDay;
					quest.currentCounter++;
				}

				if (quest.totalQuantity <= quest.currentCounter) {
					if (quest.rewardId == "Stickers") {
						int stickersUnlocked =  PlayerPrefs.GetInt ("stickersunlocked", 1);
						stickersUnlocked += (int)quest.rewardQuantity;
						PlayerPrefs.SetInt ("stickersunlocked",stickersUnlocked );
						PlayerPrefs.Save();
						quest.rewardQuantity = 0;
						quest.trackerId = "Completed";
					}
				
				}
			
			}
			currentQuests [i] = quest;
		
		}

	}

	public void loadBaseQuestDataTable ()
	{
		TextAsset file = Resources.Load ("QuestData") as TextAsset;
		string json = file.text;
		JSONObject jsonObject = new JSONObject (json);
		foreach (JSONObject obj in jsonObject.list) {
			QuestData quest = new QuestData ();
			quest.name = obj.GetField ("Name").str;
			quest.trackerId = obj.GetField ("Id").str;
			quest.questId = obj.GetField ("QuestID").f;
			quest.isDaily = obj.GetField ("IsDaily").b;
			quest.duration = obj.GetField ("Duration").f;
			quest.totalQuantity = obj.GetField ("TotalQuantity").f;
			quest.currentCounter = obj.GetField ("CurrentQuantity").f;
			quest.previousUpdate = obj.GetField ("PreviousUpdate").f;
			quest.rewardId = obj.GetField ("RewardId").str;
			quest.rewardQuantity = obj.GetField ("RewardQuantity").f;
			currentQuests.Add (quest);

		}
			
	}

	public void loadSavedContex ()
	{
		string fileName = Application.persistentDataPath + "/Color" + "QuestSaveData";
		if (System.IO.File.Exists (fileName)) {
			string jsonText = System.IO.File.ReadAllText (fileName);
			JSONObject jsonObject = new JSONObject (jsonText);
			foreach (JSONObject obj in jsonObject.list) {
				float id = obj.GetField ("QuestID").f;
				for (int i = 0; i < currentQuests.Count; i++) {
					QuestData quest = currentQuests [i];
					if (id == quest.questId) {
						quest.currentCounter = obj.GetField ("CurrentQuantity").f;
						quest.previousUpdate = obj.GetField ("PreviousUpdate").f;
						quest.trackerId = obj.GetField ("Id").str;
					}
					currentQuests [i] = quest;
				}

			}

		}
	}

	public void saveCurrentContext ()
	{
		JSONObject entries = new  JSONObject (JSONObject.Type.ARRAY);
		foreach (QuestData quest in currentQuests) {
			JSONObject pData = new JSONObject ();
			pData.AddField ("Name", quest.name);
			pData.AddField ("Id", quest.trackerId);
			pData.AddField ("QuestID", quest.questId);
			pData.AddField ("IsDaily", quest.isDaily);
			pData.AddField ("Duration", quest.duration);
			pData.AddField ("TotalQuantity", quest.totalQuantity);
			pData.AddField ("CurrentQuantity", quest.currentCounter);
			pData.AddField ("PreviousUpdate", quest.previousUpdate);
			pData.AddField ("RewardId", quest.rewardId);
			pData.AddField ("RewardQuantity", quest.rewardQuantity);
			entries.Add (pData);
		}
		string fileName = Application.persistentDataPath + "/Color" + "QuestSaveData.json";
		System.IO.File.WriteAllText (fileName,entries.Print());
	}

	void OnApplicationQuit ()
	{
		saveCurrentContext ();
	}
	void OnApplicationPause(bool paused)
	{
		if (paused) {
			saveCurrentContext();
		}
	}

}
