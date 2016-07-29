using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class HelpScreen : MonoBehaviour {

	public List<Sprite> frames;
	public List<Sprite> textDescription;
	private int index;
	public Image currentFrame;
	public Image textFrame;

	// Use this for initialization
	void Start () {
		StartCoroutine ("timer");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void rightClicked()
	{

		index++;
		if (index >= frames.Count)
			index = 0;
		currentFrame.sprite = frames [index];
		textFrame.sprite = textDescription [index];
	}


	public void leftClicked()
	{
		index--;
		if (index < 0)
			index = frames.Count - 1;
		currentFrame.sprite = frames [index];
		textFrame.sprite = textDescription [index];
	}

	IEnumerator FadeOut() {
		
		for (float f = 1f; f >= 0; f -= 0.1f) {
			Color c = currentFrame.gameObject.GetComponent<CanvasRenderer> ().GetColor (); //.color;
			c.a = f;
			currentFrame.gameObject.GetComponent<CanvasRenderer> ().SetColor (c);
			textFrame.gameObject.GetComponent<CanvasRenderer> ().SetColor (c);
			yield return new WaitForSeconds(.1f);

		}
		rightClicked ();
		StartCoroutine ("FadeIn");

	}
	IEnumerator FadeIn()
	{
		for (float f = .1f; f <= 1f; f += 0.1f) {
			Color c = currentFrame.gameObject.GetComponent<CanvasRenderer> ().GetColor (); //.color;
			c.a = f;
			currentFrame.gameObject.GetComponent<CanvasRenderer> ().SetColor (c);
			textFrame.gameObject.GetComponent<CanvasRenderer> ().SetColor (c);
			yield return new WaitForSeconds(.1f);

		}

	}


	IEnumerator timer() {
		for (int i = 0 ;i < frames.Count; i++ ) {
			StartCoroutine ("FadeOut");
			yield return new WaitForSeconds(6.0f);
		}

		int isFtue = PlayerPrefs.GetInt ("isFTUE",1);
			if (isFtue == 1) {
				SceneManager.LoadScene ("MainSelectionScreen");
				PlayerPrefs.SetInt ("isFTUE",0);
				PlayerPrefs.Save ();
			} 
		else
			StartCoroutine ("timer");	
			
	}

	public void backButtonClicked()
	{
		int isFtue = PlayerPrefs.GetInt ("isFTUE",1);
		if (isFtue == 1) {
			SceneManager.LoadScene ("MainSelectionScreen");
			PlayerPrefs.SetInt ("isFTUE",0);
			PlayerPrefs.Save ();
		} 
		else
		SceneManager.LoadScene("TitleScreen");
	}


}
