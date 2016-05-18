using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class TabsHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit)) {
				if (hit.collider.name != "Blocker")
					handleTabs (hit.collider.name);
			}
	}
}
	public bool handleTabs(string tabs)
	{
		bool isButton = false;
		string sceenToLoad = "MainSelectionScreen";
		switch (tabs) {
		case "Main":
			sceenToLoad = "MainSelectionScreen";
			isButton = true;
			break;
		case "Dairy":
			sceenToLoad = "WriteMemoPage";
			isButton = true;
			break;
		case "Sketch":
			sceenToLoad = "PaintScreen";
			isButton = true;
			break;
		case "Personal":
			sceenToLoad = "Customization";
			isButton = true;
			break;
		case "History":
			sceenToLoad = "HistoryScene";
			isButton = true;
			break;



		}
		if (isButton)
		Camera.main.GetComponent<AudioSource> ().Play();
		if(isButton)
			SceneManager.LoadScene(sceenToLoad);
		return isButton;
	}

	public void backButtonClicked()
	{
		Camera.main.GetComponent<AudioSource> ().Play();
		SceneManager.LoadScene("MainSelectionScreen");
	}

	public void submitButtonClicked()
	{
		Camera.main.GetComponent<AudioSource> ().Play();
		SceneManager.LoadScene("MainSelectionScreen");
	}

}
