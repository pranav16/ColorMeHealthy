using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class TitleScreen : MonoBehaviour {

	int numberOfClicks;
	// Use this for initialization
	void Start () {
		numberOfClicks = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetMouseButtonDown (0)) {
			numberOfClicks++;
			if(numberOfClicks == 2)
				SceneManager.LoadScene("MainSelectionScreen");
		
		}
	}
}
