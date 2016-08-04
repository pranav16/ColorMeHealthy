using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ContiniousAudioScript : MonoBehaviour {
	public static ContiniousAudioScript instance = null;
	public Sprite onImage;
	public Sprite offImage;
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
		int childCount = 	transform.childCount;
		Debug.Log ("childCount" + childCount);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void audioToggleClicked(Button button)
	{
		if (GetComponent<AudioSource> ().isPlaying) {
			GetComponent<AudioSource> ().Pause ();
			button.image.overrideSprite = offImage;
			//button.GetComponent<Image> ().sprite = offImage;
		} else {
			GetComponent<AudioSource> ().Play();
			button.image.overrideSprite = onImage;
		}

	}

}
