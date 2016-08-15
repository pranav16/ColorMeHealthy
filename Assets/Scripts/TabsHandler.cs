using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class TabsHandler : MonoBehaviour
{
	public GameObject GiftTab;
	public GameObject StatusTab;
	public List<Sprite>giftTabsIcons;
	public List<Sprite>statusTabIcons;
	public GameObject dialog;
	public bool showDialog;
	private string screenToLoad;
	// Use this for initialization
	void Start ()
	{
		initializeTabs ();
	}

	int progressionInGame()
	{
		int count = 0;
		if (FindObjectOfType<AnalyticsSystem> ().getCounterValue ("Saved_Picture") > 0)
			count ++;
		if (  FindObjectOfType<AnalyticsSystem> ().getCounterValue ("Water_Plant") > 0)
			count ++;
		if (FindObjectOfType<AnalyticsSystem> ().getCounterValue ("Completed_Goal") > 3)
			count++;
		if ( FindObjectOfType<AnalyticsSystem> ().getCounterValue ("Write_Dairy") > 0)
		count ++;
		if ( FindObjectOfType<AnalyticsSystem> ().getCounterValue ("Fill_Symptoms") > 0)
			count ++;

		return count;
	}

	void initializeTabs()
	{
		if (GiftTab == null || StatusTab == null)
			return;
		int status = progressionInGame();
		GiftTab.GetComponent<SpriteRenderer> ().sprite = giftTabsIcons [status];
		StatusTab.GetComponent<SpriteRenderer> ().sprite = statusTabIcons [status];
	
	}


	// Update is called once per frame
	void Update ()
	{
	
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit)) {
				if (hit.collider.name != "Blocker")
					handleTabs (hit.collider.name);
			}
		}
	}

	public bool handleTabs (string tabs)
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
		case "GiftTab":
			{
				GiftTab.SetActive (!GiftTab.activeInHierarchy);
				StatusTab.SetActive (!StatusTab.activeInHierarchy);
				break;
			}



		}

		if (isButton)
			Camera.main.GetComponent<AudioSource> ().Play ();
		if (isButton && !showDialog) {
			SceneManager.LoadScene (sceenToLoad);
		} else if(isButton){
			dialog.SetActive (true);
			screenToLoad = sceenToLoad;
		
		}
			
		return isButton;
	}

	public void backButtonClicked ()
	{
		Camera.main.GetComponent<AudioSource> ().Play ();
		SceneManager.LoadScene ("MainSelectionScreen");
	}

	public void submitButtonClicked ()
	{
		Camera.main.GetComponent<AudioSource> ().Play ();
		SceneManager.LoadScene ("MainSelectionScreen");
	}
	public void TitleScreenClicked()
	{
		SceneManager.LoadScene("TitleScreen");
	}

	public void PopUpNoClicked()
	{
		dialog.SetActive (false);

	}
	public void YesClicked()
	{

		dialog.SetActive (false);
		SceneManager.LoadScene (screenToLoad);
	}

}
