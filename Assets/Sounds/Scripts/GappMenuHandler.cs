//using UnityEngine;
//using System.Collections;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;
//using System.Collections.Generic;
//using System.Linq;
//
//
//public class GappMenuHandler : MonoBehaviour {
//
//
//   public GameObject touchHandler;
//   public Canvas OnScreenUi;
//   public Canvas FinalSubmit;
//   public List<Button> options;
//   private TouchHandler handler;
//   private GameManager manager;
//   private string localPart;
//   private List<string> localSymptoms;
//   private List<BodyPartsTable> FinalSymptoms;
//   public List<GameObject>bodyParts;
//   private List<GameObject> selectedText;
//   JSONObject json;
//   public List<Toggle> toggles;
//   public Font font;
//   public GameObject canvasKey;
//   //public Text canvasStartMarker;
//	private float postionY;
//    void Start () {
//
//		handler = Camera.main.GetComponent<TouchHandler>();
//        json = new JSONObject(JSONObject.Type.OBJECT);
//        manager = Camera.main.GetComponent<GameManager>();
//        localSymptoms = new List<string>();
//        FinalSymptoms = new List<BodyPartsTable>();
//		selectedText = new List<GameObject> ();
//        OnScreenUi.enabled = false;
//		loadAndSetCustomization ();
//	
//    }
//
//	// Update is called once per frame
//	void Update () {
//      
//        if(manager.isWaitingOnSymptoms())
//        {
//			
//            populateOptionsForBodyPart(handler.getCurrentBodyPart());
//            OnScreenUi.enabled = true;
//        }
//
//       
//    }
//	public void clearLocalSymptoms()
//	{
//		localSymptoms.Clear();
//		clearToggle ();
//	}
//    void populateOptionsForBodyPart(string bodyPartName)
//    {
//        List<BodyPartsTable> Table = manager.getBodyPartTable();
//        BodyPartsTable bodyPart = null;
//        for (int i = 0; i< Table.Count;i++)
//        {
//           
//            if(Table[i].getPartName() == bodyPartName)
//            {
//                bodyPart = Table[i];
//                break;
//            }
//        }
//        localPart = bodyPartName;
//        for(int i = 0; i < options.Count && i < bodyPart.getSymptoms().Count;i++)
//        {
//            options[i].GetComponentInChildren<Text>().text = bodyPart.getSymptoms()[i];
//        }
//    }
//    public void editJSONFile()
//    {
//        Debug.Log("is file present");
//        string filePath = "Symptoms.json";
//        string fileName = Application.persistentDataPath + "/Color" + filePath;
//        string rawjson = System.IO.File.ReadAllText(fileName);
//        Debug.Log("is file present");
//        JSONObject mainJson = new JSONObject(rawjson);
//        JSONObject Entries = mainJson.GetField("Entires");
//        JSONObject json = new JSONObject();
//        json.AddField("Date", System.DateTime.Now.ToString("MM_dd_yyyy"));
//        JSONObject BodyParts = new JSONObject();
//        json.AddField("BodyParts", BodyParts);
//        for (int i = 0; i < FinalSymptoms.Count; i++)
//        {
//            JSONObject arr = new JSONObject(JSONObject.Type.ARRAY);
//            BodyParts.AddField(FinalSymptoms[i].getPartName(), arr);
//            for (int j = 0; j < FinalSymptoms[i].getSymptoms().Count; j++)
//                arr.Add(FinalSymptoms[i].getSymptoms()[j]);
//
//        }
//        Entries.Add(json);
//        string serializedJson = mainJson.Print();
//        Debug.Log(mainJson.Print());
//        System.IO.File.WriteAllText(fileName, serializedJson);
//      
//    }
//
//
//    public void buildJSONFile()
//    {
//       string filePath = "Symptoms.json";
//       string fileName = Application.persistentDataPath + "/Color" + filePath;
//       if( System.IO.File.Exists(fileName))
//        {
//            editJSONFile();
//            return;
//        }
//        JSONObject mainJson = new JSONObject();
//        JSONObject entries = new  JSONObject(JSONObject.Type.ARRAY);
//        mainJson.AddField("Entires",entries);
//        JSONObject json = new JSONObject();
//        json.AddField("Date", System.DateTime.Now.ToString("MM_dd_yyyy"));
//        JSONObject BodyParts = new JSONObject();
//        json.AddField("BodyParts", BodyParts);
//        for (int i = 0; i < FinalSymptoms.Count; i++)
//        {
//            JSONObject arr = new JSONObject(JSONObject.Type.ARRAY);
//            BodyParts.AddField(FinalSymptoms[i].getPartName(), arr);
//            for (int j = 0; j < FinalSymptoms[i].getSymptoms().Count; j++)
//                arr.Add(FinalSymptoms[i].getSymptoms()[j]);
//
//        }
//        entries.Add(json);
//        string serializedJson = mainJson.Print();
//        Debug.Log(mainJson.Print());   
//        System.IO.File.WriteAllText(fileName, serializedJson);
//       
//
//    }
//
//    public void optionClicked(Button button)
//    {
//		bool currentState = button.GetComponentInChildren<Toggle> ().isOn;
//		currentState = !currentState;
//		button.GetComponentInChildren<Toggle> ().isOn = currentState;
//		if (currentState)
//			localSymptoms.Add (button.GetComponentInChildren<Text> ().text);
//		else {
//			localSymptoms.Remove (button.GetComponentInChildren<Text> ().text);
//		}
//		submitClicked ();
//	
//    }
//
//
//
//	public void updateText()
//	{
//		
//		for (int i = 0; i < selectedText.Count; i++) {
//			Destroy (selectedText[i].gameObject);
//			postionY = 0.0f;
//		}
//		if (FinalSymptoms.Count <= 0)
//			return;
//		int numberofElements = FinalSymptoms.Count;
//		int numberOfSymptoms = 0;
//		foreach (BodyPartsTable part in FinalSymptoms) {
//			numberOfSymptoms += part.getSymptoms ().Count;
//		}
//		int textCount = 0;
//		float spacing = 0.0f;
//		foreach (BodyPartsTable part in FinalSymptoms) {
//			GameObject text = creatDynamicText(part.getPartName (),true);
//			RectTransform t = text.GetComponent<RectTransform> ();
//			RectTransform containerRectTransform = canvasKey.gameObject.GetComponent<RectTransform> ();
//			//calculate the width and height of each child item.
//			if (textCount == 0) {
//				float scrollHeight = t.rect.height * numberofElements + t.rect.height * 0.5f * numberOfSymptoms;
//				containerRectTransform.sizeDelta = new Vector2(t.rect.width,scrollHeight);
//				postionY = -t.rect.height/2;
//				spacing = t.rect.height / 2;
//			}
//			text.transform.localPosition = new Vector3 (t.rect.width* 0.6f,postionY ,0.0f);
//			postionY -= spacing;
//			selectedText.Add (text);
//			textCount++;
//			foreach (string symptom in part.getSymptoms()) {
//				GameObject symptomText = creatDynamicText(symptom,false);
//				RectTransform ts = symptomText.GetComponent<RectTransform> ();
//				symptomText.transform.localPosition = new Vector3 (ts.rect.width* 0.6f,postionY ,0.0f);
//				postionY -= spacing;
//				selectedText.Add (symptomText);
//				textCount++;
//			}
//		}
//	
//
//	
//	}
//
//	public GameObject creatDynamicText(string value,bool header)
//	{
//		GameObject obj = new GameObject ();
//		obj.transform.SetParent (canvasKey.gameObject.transform);
//		obj.AddComponent<CanvasRenderer>();
//		RectTransform rectTransform =  obj.AddComponent<RectTransform> ();
//		Text text = obj.AddComponent<Text> ();
//		text.font = font;
//		text.text = value;
//		text.fontStyle = FontStyle.Bold;
//		text.color = Color.black;
//		text.fontSize = 30;
//		if (header) {
//			text.fontStyle = FontStyle.Bold;
//			text.fontSize = 40;
//
//		}
//		rectTransform.sizeDelta = new Vector2(text.preferredWidth,text.preferredHeight);
//
//
//
//		return obj;
//	}
//
//
//    public void submitClicked()
//    {
//		if (localPart == "")
//			return;
//		foreach (BodyPartsTable table in FinalSymptoms) {
//			if (table.getPartName () == localPart) {
//			 FinalSymptoms.Remove(table);
//				break;
//			}
//		}
//        BodyPartsTable bodyPart = new BodyPartsTable();
//        bodyPart.setPartName(localPart);
//        for(int i = 0; i < localSymptoms.Count;i++)
//        {
//            bodyPart.addSymptoms(localSymptoms[i]);
//        }
//
//        FinalSymptoms.Add(bodyPart);
//
//        //OnScreenUi.enabled = false;
//		Camera.main.GetComponent<TouchHandler> ().invokeDeletion();
//        manager.Ready();
//		updateText();
//		//clearToggle();
//    }
//    public void finalSubmitClicked()
//    {
//        buildJSONFile();
//        SceneManager.LoadScene("MainSelectionScreen");
//    }
//
//	public void clearToggle()
//	{
//		foreach (Toggle toggle in toggles)
//			toggle.isOn = false;
//
//	}
//    public void clearClicked()
//    {
//		localSymptoms.Clear();
//		FinalSymptoms.Clear ();
//		localPart = "";
//		//OnScreenUi.enabled = false;
//		Camera.main.GetComponent<TouchHandler> ().invokeDeletion();
//		manager.Ready();
//		updateText ();
//		clearToggle();
//    }
//
//    public void backButtonClicked()
//    {
//        SceneManager.LoadScene("MainSelectionScreen");
//    }
//	public void loadAndSetCustomization()
//	{
//		string filePath = "CurrentCustomization.json";
//		string fileName = Application.persistentDataPath + "/Color" + filePath;
//		string data = System.IO.File.ReadAllText(fileName);
//		JSONObject json = new JSONObject (data);
//		JSONObject boyField = json.GetField("Boy");
//		for (int i = 0; i < bodyParts.Count; i++) {
//			JSONObject bodyPart = boyField.GetField(bodyParts[i].name);
//
//			int currentState = (int) bodyPart.GetField("currentState").i;
//
//			float r = bodyPart.GetField("r").f;
//			float g = bodyPart.GetField("g").f;
//			float b = bodyPart.GetField("b").f;
//			float a = bodyPart.GetField("a").f;
//			Color color = new Color (r, g, b, a);
//			bodyParts [i].GetComponent<Customisation>().setCurrentState(currentState,color);
//
//		}
//
//	}
//}
