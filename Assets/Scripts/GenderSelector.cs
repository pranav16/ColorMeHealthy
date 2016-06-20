using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;
public class GenderSelector : MonoBehaviour {

	public GameObject boy;
	 
	public GameObject girl;
	public GameObject bg;
	public List<Sprite> listOfBgs;

	// Use this for initialization
	void Start () {

		loadCurrentBg ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setInitialStates()
	{
		int gender =  PlayerPrefs.GetInt("Gender",0);

			if(gender == 0)
			{
				boy.SetActive (true);
				girl.SetActive (false);
			}
			else
			{
				boy.SetActive (false);
				girl.SetActive (true);
			}

//		if (gender == 1)
//			girlParts [5].SetActive (true);
//		else if (gender == 0)
//			girlParts[5].SetActive (false);	
	}
	private void loadCurrentBg()
	{
		int index = PlayerPrefs.GetInt ("currentBackgroundImage", 0);
		if(bg)
		bg.GetComponent<SpriteRenderer> ().sprite = listOfBgs [index];
	}

	public void changeBgImage()
	{
		int index = PlayerPrefs.GetInt ("currentBackgroundImage", 0);
		index++;
		if (index >= listOfBgs.Count)
			index = 0;
		bg.GetComponent<SpriteRenderer> ().sprite = listOfBgs [index];
		PlayerPrefs.SetInt ("currentBackgroundImage", index);
		PlayerPrefs.Save ();
	}

	public void boyPartsSelected()
	{
		//by design number of componets for girl == boys so we'll just use a single loop 
		Camera.main.GetComponent<AudioSource> ().Play();

		boy.SetActive (true);
		girl.SetActive (false);

		//Gender gender = Gender.Male;
		//Analytics.SetUserGender(gender);
		PlayerPrefs.SetInt("Gender",0);
		PlayerPrefs.Save ();
	}

	public void girlPartsSelected()
	{
		Camera.main.GetComponent<AudioSource> ().Play();
		//by design number of componets for girl == boys so we'll just use a single loop 
		boy.SetActive (false);
		girl.SetActive (true);
	//	girlParts [5].SetActive (true);
		//Gender gender = Gender.Female;
		//Analytics.SetUserGender(gender);
		PlayerPrefs.SetInt("Gender",1);
		PlayerPrefs.Save ();
	}
}
