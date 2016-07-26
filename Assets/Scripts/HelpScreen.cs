using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class HelpScreen : MonoBehaviour {

	public List<Sprite> frames;
	public List<Sprite> textDescription;
	private int index;
	public Image currentFrame;
	public Image textFrame;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void rightClicked()
	{
		index++;
		if (index > frames.Count)
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
}
