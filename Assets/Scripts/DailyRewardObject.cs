using UnityEngine;
using System.Collections;


public class DailyRewardObject : MonoBehaviour {

	public GameObject lockedImage;
	public GameObject unlockedImage;
	// Use this for initialization
	void Start () {
	
	}

	public void setLockedImage(bool value)
	{
		lockedImage.SetActive(value);
	}
	public void setUnLockedImage(bool value)
	{
		unlockedImage.SetActive(value);
	}

}
