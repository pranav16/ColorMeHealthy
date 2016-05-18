using UnityEngine;
using System.Collections;

public class ContiniousAudioScript : MonoBehaviour {
	public static ContiniousAudioScript instance = null;
	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
			
		DontDestroyOnLoad (gameObject);
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
