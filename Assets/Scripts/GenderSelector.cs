using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;
public class GenderSelector : MonoBehaviour {

	public List<GameObject> boyParts;
	 
	public List<GameObject> girlParts;
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
		for (int i = 0; i < boyParts.Count; i++) {
			if(gender == 0)
			{
				boyParts [i].SetActive (true);
				girlParts [i].SetActive (false);
			}
			else
			{
				boyParts [i].SetActive (false);
				girlParts [i].SetActive (true);
			}
		}
//		if (gender == 1)
//			girlParts [5].SetActive (true);
//		else if (gender == 0)
//			girlParts[5].SetActive (false);	
	}
	private void loadCurrentBg()
	{
		int index = PlayerPrefs.GetInt ("currentBackgroundImage", 0);
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
		for (int i = 0; i < boyParts.Count; i++) {
			boyParts [i].SetActive (true);
			girlParts [i].SetActive (false);
		}
	//	girlParts [5].SetActive (false);
		Gender gender = Gender.Male;
		Analytics.SetUserGender(gender);
		PlayerPrefs.SetInt("Gender",0);
		PlayerPrefs.Save ();
	}

	public void girlPartsSelected()
	{
		Camera.main.GetComponent<AudioSource> ().Play();
		//by design number of componets for girl == boys so we'll just use a single loop 
		for (int i = 0; i < boyParts.Count; i++) {
			boyParts [i].SetActive (false);
			girlParts [i].SetActive (true);
		}
	//	girlParts [5].SetActive (true);
		Gender gender = Gender.Female;
		Analytics.SetUserGender(gender);
		PlayerPrefs.SetInt("Gender",1);
		PlayerPrefs.Save ();
	}
}
