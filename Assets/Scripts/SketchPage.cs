using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SketchPage : MonoBehaviour {

	enum States { touchbegin, touchend, drawImage, ready,earserOn, waitForFirstTouch };

	// Use this for initialization
	private States currentState;
	private List<Vector3> touchLocations;
	public GameObject touchSprite;

	private List<GameObject> blurryLineObjects;
	public GameObject quitPopUp;
	public InputField nameOfArt;
	public List<Button>colourPallet;
	public Slider colorSlider;
	public Slider scaleSlider;
	public Slider shadeSlider;
	private Vector3 originalBrushSize;
	public List<Image> shades;
	private Color colorSelected;
	private List<List<GameObject>>undoStack;
	private int stackIndex;
	public GameObject savedShield;
	//private List<GameObject> undoStack;


	void Start()
	{
		FindObjectOfType<AnalyticsSystem> ().CustomEvent("Sketch Page",new Dictionary<string, object>());
		currentState = States.waitForFirstTouch;
		touchLocations = new List<Vector3>();
		undoStack = new List<List<GameObject>> ();
		blurryLineObjects = new List<GameObject>(); 
		stackIndex = -1;
		Color color = colourPallet [0].image.color;
		touchSprite.GetComponent<SpriteRenderer>().color = color;
		colorSelected = color;
		originalBrushSize = touchSprite.transform.localScale;
		fillShadeChart ();
	}



	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButtonDown (0) && savedShield.activeInHierarchy) {
			savedShield.SetActive (false);
			SceneManager.LoadScene ("MainSelectionScreen");
			return;
		}


		if (Input.mousePosition.x > Screen.width/2) {
			return;
		}

		if (Input.GetMouseButton (0) && currentState == States.waitForFirstTouch) {
			currentState = States.touchbegin;
			return;
		} else if (Input.GetMouseButton(0) && currentState == States.touchbegin) {
			currentState = States.ready;
			stackIndex++;
			undoStack.Add (new List<GameObject> ());
		}
		else if (Input.GetMouseButton(0) && currentState == States.ready)
		{
			RaycastHit checkForBlocker;
			Ray checkForBlockerRay = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (checkForBlockerRay, out checkForBlocker)) {
				if (checkForBlocker.collider.name.Contains("Blocker")) {
					currentState = States.touchend;
					return;
				}
			}
			Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			touchPosition.z = -1.0f;
			touchLocations.Add(touchPosition);
			GameObject lineDrawn = Instantiate(touchSprite, touchPosition, Quaternion.identity) as GameObject;
			blurryLineObjects.Add(lineDrawn);
			undoStack[stackIndex].Add (lineDrawn);

		}
		else if (Input.GetMouseButton(0) == false && currentState == States.ready)
		{
			currentState = States.touchend;
		}
		else if (currentState == States.touchend)
		{
			touchLocations.Clear ();	
			currentState = States.touchbegin;


		}

	}

	public void quitPopUpYesClicked()
	{
		deleteLines ();
		quitPopUp.SetActive (false);
	}

	public void quitPopUpNoClicked()
	{
		quitPopUp.SetActive (false);
	}

	public void deleteLines()
	{
		for (int i = 0; i < blurryLineObjects.Count; i++) {
			Destroy (blurryLineObjects [i]);

		}
		stackIndex = -1;
		undoStack.Clear ();
	

	}

	private void deleteLine(List<GameObject> objs)
	{
		for(int i = 0; i < objs.Count; i++)
		{
			Destroy(objs[i]);
		}
		objs.Clear();

	}


	public void pastWorkClicked()
	{
		SceneManager.LoadScene ("DairyRead");
	}

	public void save()
	{
		//colourPallet [2].GetComponent<AudioSource> ().Play ();
		FindObjectOfType<AnalyticsSystem> ().CustomEvent("Saved_Picture",new Dictionary<string, object>());
		int currentArtNumber = PlayerPrefs.GetInt("paintingNumber",0);
		string filePath = Application.persistentDataPath  +"/Paintings" + currentArtNumber + ".png";
		takeScreenShot (filePath);
		savedShield.SetActive (true);
		//SceneManager.LoadScene ("MainSelectionScreen");
	}

	public void undoClicked()
	{
		for (int i = 0; i < undoStack[stackIndex].Count; i++) {
			blurryLineObjects.Remove (undoStack[stackIndex] [i]);
			Destroy (undoStack[stackIndex] [i]);


		}
		stackIndex--;
		if (stackIndex < -1)
			stackIndex = -1;

	}
	public void backButtonClicked()
	{

		SceneManager.LoadScene ("MainSelectionScreen");
	}

	public void earseAllClicked()
	{
		quitPopUp.SetActive (true);
	}


	public void takeScreenShot(string filePath)
	{

		RenderTexture shot = new RenderTexture(Screen.width,Screen.height,24);
		RenderTexture currentShot = Camera.main.targetTexture;
		Camera.main.targetTexture = shot;
		Camera.main.Render();
		RenderTexture.active = shot;
		Texture2D tex = new Texture2D(Screen.width/2,Screen.height , TextureFormat.RGB24, false);
		tex.ReadPixels(new Rect(0 ,0 ,Screen.width/2, Screen.height ),0,0);
		tex.Apply();
		Camera.main.targetTexture = currentShot;
		System.IO.File.WriteAllBytes(filePath,tex.EncodeToPNG());
		int currentArtNumber = PlayerPrefs.GetInt("paintingNumber",0);
		PlayerPrefs.SetInt ("paintingNumber", currentArtNumber + 1);
		PlayerPrefs.SetString(currentArtNumber -1 +".png",nameOfArt.text);
		PlayerPrefs.Save ();


	}
	public void earserClicked()
	{
		currentState = States.earserOn;
		colourPallet [1].GetComponent<AudioSource> ().Play ();

		FindObjectOfType<AnalyticsSystem> ().CustomEvent("Paint Screen Earser Clicked",new Dictionary<string, object>());
	}
		
	public void setScale(Vector3 scale)
	{
		touchSprite.transform.localScale = scale;
	}

	public void colorSliderValueChanged()
	{
		int index = (int)colorSlider.value;
		Color color = colourPallet [index].image.color;
		touchSprite.GetComponent<SpriteRenderer>().color = color;
		colorSelected = color;
		fillShadeChart ();
		shadeSelector ();
	}

	public void scaleSliderValueChanged()
	{
		Vector3 Newscale = originalBrushSize * scaleSlider.value;
		touchSprite.transform.localScale = Newscale;
	}

	public void shadeSelector()
	{
		if (shadeSlider.value > 3) {
			float factor = shadeSlider.value - 3;
			Color color = new Color (colorSelected.r * 0.5f / factor , colorSelected.g * 0.5f / factor, colorSelected.b * 0.5f / factor, 255.0f );
			touchSprite.GetComponent<SpriteRenderer> ().color = color;
		
		} else if(shadeSlider.value == 3)
		{
			touchSprite.GetComponent<SpriteRenderer> ().color = colorSelected;
		}else {
			float factor = shadeSlider.value;
			Color color = new Color (colorSelected.r + (1 - colorSelected.r) * 0.5f / factor ,colorSelected.g + (1 - colorSelected.g) * 0.5f /  factor,colorSelected.b  + (1 - colorSelected.b )* 0.5f / factor,255.0f);
			touchSprite.GetComponent<SpriteRenderer> ().color = color;
		}
	}

	public void fillShadeChart()
	{
		for (int i = 0; i < shades.Count; i++) {
			if (i > 2) {
				float factor = i - 2;
				Color color = new Color (colorSelected.r * 0.5f / factor , colorSelected.g * 0.5f / factor, colorSelected.b * 0.5f / factor, 255.0f );
				shades[i].color = color;

			} else if(i == 2)
			{
				shades[i].color = colorSelected;
			}else {
				float factor = i + 1;
				Color color = new Color (colorSelected.r + (1 - colorSelected.r) * 0.5f / factor ,colorSelected.g + (1 - colorSelected.g) * 0.5f /  factor,colorSelected.b  + (1 - colorSelected.b )* 0.5f / factor,255.0f);
				shades[i].color = color;
			}
		}
	}

}
