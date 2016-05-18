//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//
//public class GameManager : MonoBehaviour {
//
//    enum States { Init, FileLoading, Ready, WaitForSymptomSelction, Done }
//    private States state;
//    private List<BodyPartsTable> bodyPartsTable;
//  
//	// Use this for initialization
//	void Start () {
//        state = States.Init;
//        bodyPartsTable = new List<BodyPartsTable>();
//	}
//    public bool isReady()
//    {
//        return state == States.Ready ? true : false;
//    }
//    
//    public bool isWaitingOnSymptoms()
//    {
//        return state == States.WaitForSymptomSelction ? true : false;
//    }
//    public void Ready()
//    {
//        state = States.Ready;
//    }
//	
//	// Update is called once per frame
//	void Update () {
//        if (state == States.Init)
//        {
//            state = States.FileLoading;
//            loadBodyPartsTable();
//        }   
//	
//	}
//    public void waitForSymptomSelection()
//    {
//        state = States.WaitForSymptomSelction;
//    }
//
//    public List<BodyPartsTable> getBodyPartTable()
//    {
//        return bodyPartsTable;
//    }
//    void loadBodyPartsTable()
//    {
//       
//		TextAsset file = Resources.Load("BodyPartsSymptomsTable") as TextAsset;
//		string text = file.text; 
//		string json = text;
//        JSONObject jsonObject = new JSONObject(json);
//        JSONObject bodyParts = jsonObject.GetField("BodyParts");
//        addBodyPartToList(bodyParts,"RightHand");
//        addBodyPartToList(bodyParts, "LeftHand");
//        addBodyPartToList(bodyParts, "RightLeg");
//        addBodyPartToList(bodyParts, "LeftLeg");
//        addBodyPartToList(bodyParts, "Abdomen");
//        addBodyPartToList(bodyParts, "Chest");
//        addBodyPartToList(bodyParts, "Stomach");
//        addBodyPartToList(bodyParts, "Head");
//        state = States.Ready;
//    }
//
//    void addBodyPartToList(JSONObject bodyParts, string bodyPart)
//    {
//        Debug.Log("Bodypart" + bodyPart);
//        BodyPartsTable rightHand = new BodyPartsTable();
//        rightHand.setPartName(bodyPart);
//        JSONObject symptoms = bodyParts.GetField(bodyPart);
//        foreach (JSONObject j in symptoms.list)
//        {
//            rightHand.addSymptoms(j.str);
//        }
//        bodyPartsTable.Add(rightHand);
//    }
//
//    }
