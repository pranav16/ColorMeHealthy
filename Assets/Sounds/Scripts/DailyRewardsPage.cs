using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class DailyRewardsPage : MonoBehaviour {


	public List<GameObject> stickerObjects;
	public List<GameObject> acessoryObjects;

	// Use this for initialization
	void Start () {
	 
		int unlockedStickerCount = PlayerPrefs.GetInt ("unlockedStickerCount", 0);
	
		for (int i = 0 ;i<= unlockedStickerCount && i < stickerObjects.Count;i++) {
			GameObject obj = stickerObjects [i];
			obj.GetComponent<DailyRewardObject> ().setLockedImage (false);
			obj.GetComponent<DailyRewardObject> ().setUnLockedImage (true);

		}
		int unlockedAcessCount = PlayerPrefs.GetInt ("unlockedAcessCount", 0);

		for (int i = 0 ;i<= unlockedAcessCount && i < acessoryObjects.Count;i++) {
			GameObject obj = acessoryObjects [i];
			obj.GetComponent<DailyRewardObject> ().setLockedImage (false);
			obj.GetComponent<DailyRewardObject> ().setUnLockedImage (true);

		}


	}

	public void backClicked()
	{
		SceneManager.LoadScene ("MainSelectionScreen");
	}
	

}
