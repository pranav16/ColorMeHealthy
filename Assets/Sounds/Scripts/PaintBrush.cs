using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PaintBrush : MonoBehaviour {

    enum States { touchbegin, touchend, drawImage, ready,earserOn, waitForFirstTouch };

    // Use this for initialization
    private States currentState;
    private List<Vector3> touchLocations;
    private GameObject touchSprite;
    public List<GameObject> colourPallet; 
    private List<GameObject> lineGameObjects;
	public  List<GameObject> platteImages;
	public List<GameObject> blurryLineObjects;
	private bool isSmoothLineOn;
	public GameObject movingEraserBar;
	public List<GameObject>scaleHighlights;
	public GameObject earse;
	public GameObject quitPopUp;
	public InputField nameOfArt;


    void Start()
    {
		if(SceneManager.GetActiveScene ().name == "PaintScreen")
			FindObjectOfType<AnalyticsSystem> ().CustomEvent("Paint Screen",new Dictionary<string, object>());
        currentState = States.waitForFirstTouch;
        touchLocations = new List<Vector3>();
        lineGameObjects = new List<GameObject>();
		blurryLineObjects = new List<GameObject> ();
	
        touchSprite = colourPallet[0];
		if (platteImages.Count > 0) {
			platteImages[0].SetActive (false);
			platteImages[1].SetActive (true);
		}
			

		isSmoothLineOn = false;
	
    }



	public bool isEraserOn()
	{
	
		return currentState == States.earserOn ? true : false;
	}

    // Update is called once per frame
    void Update()
    {
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit)) {
			    
				if (paintSelected (hit.collider.name)) {
					currentState = States.waitForFirstTouch;
					splatOn (hit.collider.name);
				} else if (hit.collider.name == "eraser") {
					earserClicked ();
				} else if (hit.collider.name == "Color_Box") {
					isSmoothLineOn = !isSmoothLineOn;
				} else if (hit.collider.name == "Scale1") {
					scaleHighlights [0].SetActive (true);
					scaleHighlights [1].SetActive (false);
					scaleHighlights [2].SetActive (false);
					setScale (hit.collider.gameObject.transform.localScale);
				}
				else if(hit.collider.name == "Scale2")
				{
					scaleHighlights [0].SetActive (false);
					scaleHighlights [1].SetActive (true);
					scaleHighlights [2].SetActive (false);
					setScale (hit.collider.gameObject.transform.localScale);
				}
				else if(hit.collider.name == "Scale3")
				{
					scaleHighlights [0].SetActive (false);
					scaleHighlights [1].SetActive (false);
					scaleHighlights [2].SetActive (true);
					setScale (hit.collider.gameObject.transform.localScale);
				}
				if (hit.collider.name.Contains("Blocker")) {
				}


				return;	
			}
				
		}
		RaycastHit hitCheck;
		Ray rayCheck = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast (rayCheck, out hitCheck)) {
			if (hitCheck.collider.name.Contains("Blocker") || hitCheck.collider.name == "Color_Box" || hitCheck.collider.name == "Scale1" || hitCheck.collider.name == "Scale2" || hitCheck.collider.name == "Scale3") {
				Debug.Log (hitCheck.collider.name);
				return;
			} else {
			
				//Debug.Log (hitCheck.collider.name);
			}
		}


		if (currentState == States.earserOn) {
			Vector3 touchPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			touchPosition.z = -1.0f;
			movingEraserBar.SetActive (true);
			movingEraserBar.transform.position = touchPosition;
			earse.transform.localScale = new Vector3 (1.5f, 1.5f, 1.0f);
			for (int i = 0; i < platteImages.Count; i++) {
				if (i % 2 == 0)
					platteImages [i].SetActive (true);
				else
					platteImages [i].SetActive (false);
			
			}


		} else {
			
			movingEraserBar.SetActive (false);
			if(earse)
			earse.transform.localScale = new Vector3 (1f, 1f, 1f);
		}

        if (Input.GetMouseButton(0) && currentState == States.waitForFirstTouch)
        {
            currentState = States.touchbegin;
            return;
        }
        else if (Input.GetMouseButton(0) && (currentState == States.ready || currentState == States.touchbegin))
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
            touchPosition.z = -8.0f;
            touchLocations.Add(touchPosition);
            currentState = States.touchbegin;
            GameObject lineDrawn = Instantiate(touchSprite, touchPosition, Quaternion.identity) as GameObject;
			blurryLineObjects.Add(lineDrawn);
        }
        else if (currentState == States.touchbegin)
        {
            currentState = States.touchend;
        }
        else if (currentState == States.touchend)
        {
			//deleteLine (blurryLineObjects);
			List<Vector3> smoothPoints;
			if (isSmoothLineOn) {
			 
				 smoothPoints = MakeSmoothCurve (touchLocations, 10.0f);
				//StartCoroutine("MakeSmoothCurve");



			} else {
				smoothPoints = touchLocations;	
			}
			for (int i = 0; i < smoothPoints.Count; i++) {
				//GameObject lineDrawn = Instantiate (touchSprite,smoothPoints[i], Quaternion.identity) as GameObject;
				//lineGameObjects.Add (lineDrawn);

			}
			touchLocations.Clear ();	
            currentState = States.ready;
		
			 
        }

    }
	public void earseAllClicked()
	{
		quitPopUp.SetActive (true);
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
		lineGameObjects.Clear ();
		colourPallet [1].GetComponent<AudioSource> ().Play ();
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
		colourPallet [2].GetComponent<AudioSource> ().Play ();
		int currentArtNumber = PlayerPrefs.GetInt("paintingNumber",0);
		string filePath = Application.persistentDataPath  +"/Paintings" + currentArtNumber + ".png";
		takeScreenShot (filePath);
	}




	public void takeScreenShot(string filePath)
	{

		RenderTexture shot = new RenderTexture(Screen.width,Screen.height,24);
		//filePath = Application.dataPath + "/0.png";
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
		SceneManager.LoadScene ("MainSelectionScreen");

	}
	public void earserClicked()
	{
		currentState = States.earserOn;
		colourPallet [1].GetComponent<AudioSource> ().Play ();

		FindObjectOfType<AnalyticsSystem> ().CustomEvent("Paint Screen Earser Clicked",new Dictionary<string, object>());
	}

	public bool checkForPaintTiles(string collider)
	{
		bool isPaint = false;
		switch(collider)
		{
		case "TouchSprite(Clone)":
			isPaint = true;
					break;
		case "BlackSprite(Clone)":
			isPaint = true;
			break;	
		case "BrownSpite(Clone)":
			isPaint = true;
			break;
		case "PinkSprite(Clone)":
			isPaint = true;
			break;
		case "BlueSprite(Clone)":
			isPaint = true;
			break;
		case "YellowSprite(Clone)":
			isPaint = true;
			break;
		case "GreenSprite(Clone)":
			isPaint = true;
			break;
		case "PurpleSprite(Clone)":
			isPaint = true;
			break;
		case "WhiteSprite(Clone)":
			isPaint = true;
			break;

		}

		return isPaint;
	}

	public void setScale(Vector3 scale)
	{
		touchSprite.transform.localScale = scale;
	}

	public bool paintSelected(string collider)
    {
     
		bool isButton = false;
		Vector3 scale = touchSprite.transform.localScale;
		switch(collider)
        {
			case "Red":
				touchSprite = colourPallet [0];
				isButton = true;
                    break;
			case "Purple":
                touchSprite = colourPallet[1];
			    isButton = true;
                break;
			case "Pink":
                touchSprite = colourPallet[2];
			    isButton = true;
                break;
            case "Brown":
                touchSprite = colourPallet[3];
			    isButton = true;
                break;
            case "Black":
                touchSprite = colourPallet[4];
			    isButton = true;
                break;
			case "Yellow":
				touchSprite = colourPallet[5];
				isButton = true;
				break;
			case "Green":
				touchSprite = colourPallet[6];
				isButton = true;
				break;
			case "Blue":
				touchSprite = colourPallet[7];
				isButton = true;
				break;
			case "White":
				touchSprite = colourPallet[8];
				isButton = true;
				break;
        } 
		if (isButton) {
			
			colourPallet [0].GetComponent<AudioSource> ().Play ();
			FindObjectOfType<AnalyticsSystem> ().CustomEvent("Paint Selected", new Dictionary<string, object>
				{
					{ "Paint Selected", collider }

				});
			touchSprite.transform.localScale = scale;
		}
		return isButton;

    }
	public void splatOn(string collider)
	{
		int index = 0;

		switch(collider)
		{
		case "Red":
			index = 0;
			break;
		case "Purple":
			index = 2;

			break;
		case "Pink":
			index = 4;

			break;
		case "Brown":
			index = 6;

			break;
		case "Black":
			index = 8;

			break;
		case "Yellow":
			index = 10;

			break;
		case "Green":
			index = 12;

			break;
		case "Blue":
			index = 14;

			break;
		case "White":
			index = 16;

			break;
		} 

		for (int i = 0; i < platteImages.Count; i++) {
		
			platteImages [i].SetActive (false);
			if (i == index + 1 || i % 2 == 0) {
				platteImages [i].SetActive (true);
			} 
			if (i == index) {
				platteImages [i].SetActive (false);
			}
		

		}
		currentState = States.ready;

	}
	public static List<Vector3> MakeSmoothCurve(List<Vector3> arrayToCurve,float smoothness){

		List<Vector3> points;
		List<Vector3> curvedPoints = new List<Vector3>();
		int pointsLength = 0;
		int curvedLength = 0;

		if(smoothness < 1.0f) smoothness = 1.0f;

		pointsLength = arrayToCurve.Count;

		curvedLength = (pointsLength*Mathf.RoundToInt(smoothness))-1;
		curvedPoints = new List<Vector3>(curvedLength);

		float t = 0.0f;
		for(int pointInTimeOnCurve = 0;pointInTimeOnCurve < curvedLength+1;pointInTimeOnCurve++){
			t = Mathf.InverseLerp(0,curvedLength,pointInTimeOnCurve);

			points = new List<Vector3>(arrayToCurve);

			for(int j = pointsLength-1; j > 0; j--){
				for (int i = 0; i < j; i++){
					points[i] = (1-t)*points[i] + t*points[i+1];
				}
			}

			curvedPoints.Add(points[0]);
		}

		return curvedPoints;
	}
		
}
