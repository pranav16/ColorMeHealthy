using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class TitleScreen : MonoBehaviour {

	int numberOfClicks;
	// Use this for initialization
	void Start () {
		//PlayerPrefs.DeleteAll ();
	}
	
	// Update is called once per frame
	void Update () {
	
	
	}

	public void startClicked()
	{
		SceneManager.LoadScene("MainSelectionScreen");
	}
}
