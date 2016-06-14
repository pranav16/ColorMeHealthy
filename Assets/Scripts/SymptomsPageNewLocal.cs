using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;


public class SymptomsPageNewLocal : MonoBehaviour {

	public static string bodyPartSelected;
	private static List<BodyPartsTable> listOfSymptoms = new List<BodyPartsTable>();
	private BodyPartsTable selectedSymptom;
	public List<Text> symptomsHeader;
	private Dictionary<string ,symptoms> selectedSymptoms;
	public SpriteRenderer partsRender;
	public List<GameObject> symptomNodes;
	public Text bodyPartSelectedText;
	public List<Slider>howmuch;
	public List<Slider>bothersome;
	public List<Toggle>toggles;
	// Use this for initialization
	enum states {Ready,LoadFile,Updatevalues,Done};
	states state;
	void Start () {
		FindObjectOfType<AnalyticsSystem> ().CustomEvent("Symptoms Local Page", new Dictionary<string, object>
			{
				{ "body part", bodyPartSelected },

			});
		selectedSymptom = new BodyPartsTable ();
		selectedSymptoms = new Dictionary<string, symptoms> ();
		Debug.Log (bodyPartSelected);
		selectedSymptom.setPartName (bodyPartSelected);
		state = states.Ready;
		if (listOfSymptoms.Count > 0)
			state = states.Updatevalues;

		if (state == states.Ready)
			loadBodyPartsTable ();
		else if (state == states.Updatevalues)
			updateUi ();
			

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	string spriteNameToBodyPartSelected()
	{
		string filename = "";
		switch (bodyPartSelected) {
		case "Head":
			filename = "Symptom_Boy_Head";
			bodyPartSelectedText.text = "Head";
			break;
		case "Chest":
			filename = "Symptom_Chest";
			bodyPartSelectedText.text = "Chest";
			break;
		case "RightHand":
			filename = "Symptom_Arm";
			bodyPartSelectedText.text = "Right Arm";
			break;
		case "LeftHand":
			filename = "Left_Arm";
			bodyPartSelectedText.text = "Left Arm";
			break;
		case "RightLeg":
			filename = "Symptom_Leg";
			bodyPartSelectedText.text = "Right Leg";
			break;
		case "LeftLeg":
			filename = "Left_Leg";
			bodyPartSelectedText.text = "Left Leg";
			break;
		case "Stomach":
			filename = "Symptom_Stomach";
			bodyPartSelectedText.text = "Stomach";
			break;
		case "Abdomen":
			filename = "Symptom_Stomach";
			bodyPartSelectedText.text = "Abdomen";
			break;
		case "RightToe":
			filename = "Symptom_RFoot";
			bodyPartSelectedText.text = "Right foot";
			break;
		case "LeftToe":
			filename = "Symptom_LFoot";
			bodyPartSelectedText.text = "Left foot";
			break;
		case "RightPalms":
			filename = "Symptom_RHand";
			bodyPartSelectedText.text = "Right Hand";
			break;
		case "LeftPalms":
			filename = "Symptom_LHand";
			bodyPartSelectedText.text = "Left Hand";
			break;
		case "Mouth":
			filename = "Mouth";
			bodyPartSelectedText.text = "Mouth";
			break;
		

		}
		return filename;
	}

	void updateUi()
	{
		BodyPartsTable bodyPartToDisplay = new BodyPartsTable();
		foreach (BodyPartsTable table in listOfSymptoms) {
			if (table.getPartName () == bodyPartSelected) {
				bodyPartToDisplay = table;
				break;
			}
				
		}
		List<string> headers = new List<string> ();
		foreach(symptoms s in bodyPartToDisplay.getSymptoms())
		{
			headers.Add(s.name);
		}
		setSymptomsHeader (headers);
		partsRender.sprite = Resources.Load<Sprite> (spriteNameToBodyPartSelected())  as Sprite;
 		state = states.Done;

	}

	void setSymptomsHeader(List<string> headers)
	{
		for (int i = 0; i < headers.Count && i < symptomsHeader.Count; i++)
			symptomsHeader [i].text = headers [i];

		for (int i = headers.Count; i < symptomNodes.Count; i++)
			symptomNodes [i].SetActive (false);
	}

	public void valueChangedSymptom(Toggle toggle)
	{
		return;
		bool value = toggle.isOn;
		int index = int.Parse(toggle.name);
		if (value) {
			howmuch [index].interactable = true;
			bothersome[index].interactable = true;
			symptoms symptom = new symptoms();
			if (selectedSymptoms.ContainsKey (symptomsHeader [index].text))
				symptom = selectedSymptoms [symptomsHeader [index].text];
			symptom.name = symptomsHeader [index].text;
			symptom.painScale = howmuch [index].value;
			symptom.botherScale = bothersome [index].value;

			selectedSymptoms [symptomsHeader [index].text] = symptom;
		}
		else {
			howmuch [index].interactable = false;
			bothersome[index].interactable = false;
			howmuch [index].value = howmuch [index].minValue;
			bothersome [index].value = bothersome [index].minValue;
		}
	
			
	}

	public void painChange(Slider slide)
	{
		int index = int.Parse(slide.name);
		symptoms symptom = new symptoms();
		if (selectedSymptoms.ContainsKey (symptomsHeader [index].text))
			symptom = selectedSymptoms [symptomsHeader [index].text];
		symptom.name = symptomsHeader [index].text;
		symptom.painScale = slide.value;
		selectedSymptoms [symptomsHeader [index].text] = symptom;
		toggles [index].isOn = true;
	}


	public void bothersomeChange(Slider slide)
	{
		int index = int.Parse(slide.name);
		symptoms symptom = new symptoms();
		if (selectedSymptoms.ContainsKey (symptomsHeader [index].text))
			symptom = selectedSymptoms [symptomsHeader [index].text];
		symptom.name = symptomsHeader [index].text;
		symptom.botherScale = slide.value;
		selectedSymptoms [symptomsHeader [index].text] = symptom;
		toggles [index].isOn = true;
	}



	public void backButtonClicked()
	{
		if(SymptomsPageNewMain.finalSymptoms.ContainsKey(bodyPartSelected))
		{
			SymptomsPageNewMain.finalSymptoms.Remove (bodyPartSelected);
		}
		Camera.main.GetComponent<AudioSource> ().Play();
		SceneManager.LoadScene("SymptomsPageNew");
	}

	public void submitButtonClicked()
	{
		//BodyPartsTable table = new BodyPartsTable ();
		Dictionary<string,object> analytics =	new Dictionary<string, object> ();
		int imageFileNumber = PlayerPrefs.GetInt(bodyPartSelected,0);
		string filePath = Application.persistentDataPath +"/Images"+ bodyPartSelected+"_"+imageFileNumber+".jpg";
		takeScreenShot (filePath);
		imageFileNumber++;
		PlayerPrefs.SetInt(bodyPartSelected, imageFileNumber);
		PlayerPrefs.Save();
		selectedSymptom.setPartName (bodyPartSelectedText.text);
		selectedSymptom.setImagePath (filePath);
		analytics ["Body Part Selected"] = bodyPartSelected;
		foreach (string key in selectedSymptoms.Keys) {
			if(selectedSymptoms[key].name != "")
			{
			selectedSymptom.addSymptoms (selectedSymptoms [key]);
			analytics ["symptom name"] = selectedSymptoms [key].name;
			analytics ["pain"] = " " +selectedSymptoms[key].painScale;
			analytics ["botherscale"] = " " +selectedSymptoms[key].botherScale;
			}
		}

		FindObjectOfType<AnalyticsSystem> ().CustomEvent ("Symptoms Local Body part", analytics);
		SymptomsPageNewMain.finalSymptoms [bodyPartSelected] = selectedSymptom;
		symptomNodes [0].GetComponent<AudioSource> ().Play ();
		SceneManager.LoadScene("SymptomsPageNew");
	}


	public void takeScreenShot(string filePath)
	{
		//filePath = Application.dataPath + "/picture1.jpg";
		RenderTexture shot = new RenderTexture(Screen.width,Screen.height,24);
		Camera.main.targetTexture = shot;
		Camera.main.Render();
		RenderTexture.active = shot;
		Texture2D tex = new Texture2D(Screen.width/2,Screen.height , TextureFormat.RGB24, false);
		tex.ReadPixels(new Rect(0 ,0 ,Screen.width/2, Screen.height),0,0);
		int width = 400;
		int height = 400;
		Rect texR = new Rect(0,0,width,height);
		_gpu_scale(tex,width,height,FilterMode.Trilinear);

		// Update new texture
		tex.Resize(width, height);
		tex.ReadPixels(texR,0,0,true);
		tex.Apply(true);  
		//Texture2D tex = new Texture2D(Screen.width,Screen.height , TextureFormat.RGB24, false);
		//tex.ReadPixels(new Rect(0,0 ,Screen.width,Screen.height),0,0);
		tex.Apply();
	
		System.IO.File.WriteAllBytes(filePath,tex.EncodeToJPG());

	
	}
	static void _gpu_scale(Texture2D src, int width, int height, FilterMode fmode)
	{
		//We need the source texture in VRAM because we render with it
		src.filterMode = fmode;
		src.Apply(true);       

		//Using RTT for best quality and performance. Thanks, Unity 5
		RenderTexture rtt = new RenderTexture(width, height, 32);

		//Set the RTT in order to render to it
		Graphics.SetRenderTarget(rtt);

		//Setup 2D matrix in range 0..1, so nobody needs to care about sized
		GL.LoadPixelMatrix(0,1,1,0);

		//Then clear & draw the texture to fill the entire RTT.
		GL.Clear(true,true,new Color(0,0,0,0));
		Graphics.DrawTexture(new Rect(0,0,1,1),src);
	}

	void loadBodyPartsTable()
	{

		TextAsset file = Resources.Load("BodyPartsSymptomsTable") as TextAsset;
		string text = file.text; 
		string json = text;
		JSONObject jsonObject = new JSONObject(json);
		JSONObject bodyParts = jsonObject.GetField("BodyParts");
		addBodyPartToList(bodyParts,"RightHand");
		addBodyPartToList(bodyParts, "LeftHand");
		addBodyPartToList(bodyParts, "RightLeg");
		addBodyPartToList(bodyParts, "LeftLeg");
		addBodyPartToList(bodyParts, "Abdomen");
		addBodyPartToList(bodyParts, "Chest");
		addBodyPartToList(bodyParts, "Stomach");
		addBodyPartToList(bodyParts, "Head");
		addBodyPartToList(bodyParts, "RightToe");
		addBodyPartToList(bodyParts, "LeftToe");
		addBodyPartToList(bodyParts, "RightPalms");
		addBodyPartToList(bodyParts, "LeftPalms");
		addBodyPartToList(bodyParts, "Mouth");
		state = states.Updatevalues;
		updateUi ();
	

	}
	void addBodyPartToList(JSONObject bodyParts, string bodyPart)
	{
		
		Debug.Log("Bodypart" + bodyPart);
		BodyPartsTable rightHand = new BodyPartsTable();
		rightHand.setPartName(bodyPart);
		JSONObject symptoms = bodyParts.GetField(bodyPart);
		foreach (JSONObject j in symptoms.list)
		{    
			symptoms s;
			s.name = j.str;
			s.botherScale = 0;
			s.painScale = 0;
		
			rightHand.addSymptoms(s);
		}
		listOfSymptoms.Add(rightHand);
	}

}
