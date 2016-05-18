using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class StickerHomeScreenBehaviour : MonoBehaviour {


	public List<GameObject> objectsToBeDisabled;
	public GameObject mainScroller;
	public GameObject stickerObjectToInstaniate;
	private GameObject currentStickerObject;
	public GameObject Trash;
	private bool firstTouchDown;
	private bool isRePositionObject;
	private List<GameObject> stickerPlaced;
	public List<Button> stickerButtons;
	private Dictionary<string,Button> spriteNameToButtonMap;
	private int index;
	public List<GameObject> lockedSilhouette;
	public Button dailyRewardUnlocked;
	public List<Button> giftButtons;
	// Use this for initialization
	public GameObject dailyRewardPopUp;
	void Start () {
		
		firstTouchDown = false;
		spriteNameToButtonMap = new Dictionary<string, Button> ();
		stickerPlaced = new List<GameObject> ();
	
		foreach (Button btn in stickerButtons) {
			spriteNameToButtonMap [btn.image.sprite.name] = btn;

		}

	
		index = PlayerPrefs.GetInt ("stickersunlocked", 1);
		int dayOfYear = PlayerPrefs.GetInt("stickerUnlockDay",System.DateTime.Now.DayOfYear - 1);
		if (dayOfYear < System.DateTime.Now.DayOfYear) {
			index++;
			dayOfYear = System.DateTime.Now.DayOfYear;
			PlayerPrefs.SetInt ("stickersunlocked",index);
			PlayerPrefs.SetInt ("stickerUnlockDay",System.DateTime.Now.DayOfYear);
			PlayerPrefs.SetInt ("unlockedStickers", 1);
	
		}
		int unlockedSticker = 	PlayerPrefs.GetInt ("unlockedStickers", 0);
		if (unlockedSticker == 1) {
			dailyRewardPopUp.SetActive (true);
			foreach (GameObject obj in objectsToBeDisabled) {
				BoxCollider[]colliders = obj.GetComponentsInChildren<BoxCollider> ();
				foreach (BoxCollider col in colliders)
					col.enabled = false;
			}
		}
		PlayerPrefs.SetInt ("unlockedStickers", 0);
		PlayerPrefs.Save ();
		UnlockStickers (index);
		LoadScene();

	}
		

	public void UnlockStickers(int index)
	{
		for (int i = 0; i < index; i++) {
			lockedSilhouette [i].SetActive (false);
			stickerButtons [i].interactable = true;
		}
			
	}
	public void  giftClicked()
	{
		
		foreach (Button btn in giftButtons)
			btn.gameObject.SetActive (false);
		int unlockedImage = 0;
		if (index > 0)
			unlockedImage = index - 1;
		dailyRewardUnlocked.image.sprite = stickerButtons [unlockedImage].image.sprite;
	
		dailyRewardUnlocked.gameObject.SetActive (true);
	}

	// Update is called once per frame
	void Update () {

		if (currentStickerObject == null) {

			if (Input.GetMouseButtonDown (0)) {
				RaycastHit hitCheck;
				Ray rayCheck = Camera.main.ScreenPointToRay (Input.mousePosition);
				if (Physics.Raycast (rayCheck, out hitCheck)) {
					if (hitCheck.collider.name.Contains ("StickerPlacementObject") && !mainScroller.activeInHierarchy) {
						StickerTrashClicked (hitCheck.collider.gameObject);
					}
				}
			}
			return;
		}
			
		if (!firstTouchDown) {
			Vector3 touchPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			touchPosition.z = -3.0f;
			currentStickerObject.transform.position = touchPosition;

		}
		if (Input.GetMouseButton (0)) {
			firstTouchDown = true;
			Vector3 touchPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			touchPosition.z = -3.0f;
			currentStickerObject.transform.position = touchPosition;
			RaycastHit hitCheck;
			Ray rayCheck = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (rayCheck, out hitCheck)) {
				if (hitCheck.collider.name.Contains ("Trash")) {
					if(isRePositionObject){
						foreach (GameObject objs in objectsToBeDisabled) {
							BoxCollider[]colliders = objs.GetComponentsInChildren<BoxCollider> ();
							foreach (BoxCollider col in colliders)
								col.enabled = true;
						}
						foreach (GameObject placed in stickerPlaced) {
							if (currentStickerObject == placed) {
								stickerPlaced.Remove (currentStickerObject);
								break;
							}
						}
						saveScene ();
						Destroy (currentStickerObject);
						currentStickerObject = null;
						mainScroller.SetActive (false);
						disableStickers ();
						isRePositionObject = false;
						firstTouchDown = false;
						Trash.SetActive (false);

						return;
					}
					foreach (GameObject placed in stickerPlaced) {
						if (currentStickerObject == placed) {
							stickerPlaced.Remove (currentStickerObject);
							break;
						}
					}
					saveScene ();
					Destroy (currentStickerObject);

					currentStickerObject = null;
					Trash.SetActive (false);
					disableStickers ();

					firstTouchDown = false;
				}
			}
		} else if (firstTouchDown && !isRePositionObject) {
			foreach (GameObject placed in stickerPlaced) {
				if (currentStickerObject == placed) {
					stickerPlaced.Remove (currentStickerObject);
					break;
				}
			}
			stickerPlaced.Add (currentStickerObject);
			currentStickerObject = null;
			Trash.SetActive (false);
			disableStickers ();
			firstTouchDown = false;

			saveScene ();
		} else if(firstTouchDown && isRePositionObject){
		   
			foreach (GameObject objs in objectsToBeDisabled) {
				BoxCollider[]colliders = objs.GetComponentsInChildren<BoxCollider> ();
				foreach (BoxCollider col in colliders)
					col.enabled = true;
			}
			foreach (GameObject placed in stickerPlaced) {
				if (currentStickerObject == placed) {
					stickerPlaced.Remove (currentStickerObject);
					break;
				}
			}
			stickerPlaced.Add (currentStickerObject);
			saveScene ();
			currentStickerObject = null;
			mainScroller.SetActive (false);

			isRePositionObject = false;
			firstTouchDown = false;
			Trash.SetActive (false);


		}
	
	}
	public void StickerTrashClicked(GameObject obj)
	{

		foreach (GameObject objs in objectsToBeDisabled) {
			BoxCollider[]colliders = objs.GetComponentsInChildren<BoxCollider> ();
			foreach (BoxCollider col in colliders)
				col.enabled = false;
		}
		firstTouchDown = true;
		currentStickerObject = obj;
		Trash.SetActive (true);
		mainScroller.SetActive (false);
		isRePositionObject = true;
	}

	public void  closeGiftPopUp()
	{
		dailyRewardPopUp.SetActive (false);
		foreach (GameObject obj in objectsToBeDisabled) {
			BoxCollider[]colliders = obj.GetComponentsInChildren<BoxCollider> ();
			foreach (BoxCollider col in colliders)
				col.enabled = true;
		}
	}

	public void stickersButtonClicked()
	{
		
		if (mainScroller.activeInHierarchy) {
			foreach (GameObject obj in objectsToBeDisabled) {
				BoxCollider[]colliders = obj.GetComponentsInChildren<BoxCollider> ();
				foreach (BoxCollider col in colliders)
					col.enabled = true;
			}

			firstTouchDown = false;
			mainScroller.SetActive (false);

		} else {
			foreach (GameObject obj in objectsToBeDisabled) {
				 BoxCollider[]colliders = obj.GetComponentsInChildren<BoxCollider> ();
				foreach (BoxCollider col in colliders)
					col.enabled = false;
				//obj.SetActive (false);
			}


			mainScroller.SetActive (true);
		}


	}

	public void disableStickers()
	{
		foreach (GameObject obj in objectsToBeDisabled) {
			BoxCollider[]colliders = obj.GetComponentsInChildren<BoxCollider> ();
			foreach (BoxCollider col in colliders)
				col.enabled = true;
		}
		mainScroller.SetActive (false);


	}

	public void StickerClicked(Button button)
	{
		//if(Button.)

		currentStickerObject = Instantiate(stickerObjectToInstaniate);
		currentStickerObject.GetComponent<SpriteRenderer> ().sprite = button.image.sprite;
		currentStickerObject.transform.position = new Vector3 (0.0f, 0.0f, -6.0f);
		mainScroller.SetActive (false);
		Trash.SetActive (true);

	}

	public void LoadScene()
	{
		string fileName = "StickersMain.json";
		string filePath = Application.persistentDataPath + "/Color" + fileName;
		if (!System.IO.File.Exists (filePath))return;
		string data = System.IO.File.ReadAllText(filePath);
		JSONObject json = new JSONObject (data);
		JSONObject Stickers = json.GetField ("Stickers");
		foreach (JSONObject obj in Stickers.list) {
			GameObject sticker =  Instantiate(stickerObjectToInstaniate);
			sticker.transform.position = new Vector3 (obj.GetField("positionX").f,obj.GetField("positionY").f,obj.GetField("positionZ").f);

			sticker.GetComponent<SpriteRenderer> ().sprite = spriteNameToButtonMap[obj.GetField("name").str].image.sprite;
			stickerPlaced.Add (sticker);
		}

	}

	public void saveScene()
	{
		string fileName = "StickersMain.json";
		string filePath = Application.persistentDataPath + "/Color" + fileName;
		JSONObject json = new JSONObject ();
		JSONObject Stickers = new JSONObject (JSONObject.Type.ARRAY);
		json.AddField ("Stickers",Stickers);
		foreach(GameObject obj in stickerPlaced)
		{
			JSONObject node = new JSONObject ();
			node.AddField ("name",obj.GetComponent<SpriteRenderer>().sprite.name);
			node.AddField ("positionX",obj.transform.position.x);
			node.AddField ("positionY",obj.transform.position.y);
			node.AddField ("positionZ",obj.transform.position.z);
			Stickers.Add (node);

		}
		Debug.Log(json.Print ());
		System.IO.File.WriteAllText(filePath, json.Print ());
	}

}
