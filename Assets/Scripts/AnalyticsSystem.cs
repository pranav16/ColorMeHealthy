﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;

public class AnalyticsSystem : MonoBehaviour
{

	private AnalyticsSystem instance;
	private Dictionary<string,int> Counters;

	// Use this for initialization
	void Start ()
	{
		if (instance == null) {
			instance = this;
			instance.init ();
		} else if (instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad (gameObject);
	}

	private bool init ()
	{
		Counters = new Dictionary<string, int> ();
		loadAnalytics ();
		return true;
	}

	public void UpdateCounter (string trackingID, int count = 1)
	{
		if (Counters.ContainsKey (trackingID))
			Counters [trackingID] += count;
		else
			Counters.Add (trackingID, count);
		FindObjectOfType<QuestSystem> ().updateCounters (trackingID);
	}

	public int getCounterValue (string trackingID)
	{
		if (Counters.ContainsKey (trackingID))
			return Counters [trackingID];
		return -1;
	}

	public void CustomEvent (string trackingId, Dictionary<string,object> data)
	{
		if (data.ContainsKey ("Count")) {
			UpdateCounter (trackingId, System.Convert.ToInt32 (data ["Count"]));
		} else
			UpdateCounter (trackingId);
		Analytics.CustomEvent (trackingId, data);
	}

	public void loadAnalytics ()
	{
		if (!System.IO.File.Exists (Application.persistentDataPath +"/color" +"Analytics.json"))
			return;
		string jsonText = System.IO.File.ReadAllText (Application.persistentDataPath +"/color" + "Analytics.json");
		JSONObject json = new JSONObject (jsonText);
		if (!json.HasField (System.DateTime.Now.DayOfYear.ToString ()))
			return;
		
		JSONObject body = json.GetField (System.DateTime.Now.DayOfYear.ToString ());
		foreach (JSONObject obj in body.list) {
			string name = obj.GetField ("name").str;
			int value = (int)obj.GetField ("value").i;
			Counters.Add (name, value);
		}

	}

	public void saveAnalytics ()
	{
		JSONObject json = new JSONObject ();
		JSONObject body = new JSONObject (JSONObject.Type.ARRAY);
		json.AddField (System.DateTime.Now.DayOfYear.ToString (), body);
		foreach (string key in Counters.Keys) {
			JSONObject obj = new JSONObject ();
			obj.AddField ("name", key);
			obj.AddField ("value", Counters [key]);
			body.Add (obj);
		}
		System.IO.File.WriteAllText (Application.persistentDataPath +"/color" + "Analytics.json", json.Print ());

	}

	void OnApplicationQuit ()
	{
		saveAnalytics ();
	}
}
