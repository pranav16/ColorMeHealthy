//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine.UI;
//
//public class TouchHandler : MonoBehaviour {
//
//    enum States {touchbegin,touchend,drawImage,ready,waitForFirstTouch};
//
//    // Use this for initialization
//    private States currentState;
//    private List<Vector3> touchLocations;
//    public  GameObject touchSprite;
//    private string bodyPartSelected;
//
//    private List<GameObject> lineGameObjects;
//   // private List<string> bodyPartsSelected;
//    GameManager manager;
//
//	void Start () {
//
//        currentState = States.waitForFirstTouch;
//        touchLocations = new List<Vector3>();
//        lineGameObjects = new List<GameObject>();
//       // bodyPartsSelected = new List<string>();
//        manager = Camera.main.GetComponent<GameManager>();
//        
//    }
//	bool checkForInteractableObjects(string name)
//	{
//		bool isInteractableObject = false;
//		switch (name) {
//		case "WindowCollider":
//				isInteractableObject = true;
//				break;
//		case "WindowCollider (1)":
//			isInteractableObject = true;
//			break;
//		case "WindowCollider (2)":
//			isInteractableObject = true;
//			break;
//		case "WindowCollider (4)":
//			isInteractableObject = true;
//			break;
//		case "WindowCollider (5)":
//			isInteractableObject = true;
//			break;
//
//
//		}
//		return isInteractableObject;
//	}
//	// Update is called once per frame
//	void Update () {
//
//        //if (!manager.isReady())
//           // return;
//		if (Input.GetMouseButtonDown (0)) {
//			RaycastHit hit;
//			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
//			if (Physics.Raycast (ray, out hit)) {
//
//				if (checkForInteractableObjects(hit.collider.name)) {
//					return;	
//				}
//
//			}
//
//		}
//		RaycastHit hitCheck;
//		Ray rayCheck = Camera.main.ScreenPointToRay (Input.mousePosition);
//		if (Physics.Raycast (rayCheck, out hitCheck)) {
//			if (checkForInteractableObjects(hitCheck.collider.name)) {
//				return;	
//			}
//		}
//
//       if(Input.GetMouseButton(0) && currentState == States.waitForFirstTouch)
//        {
//            currentState = States.touchbegin;
//            return;
//        }
//      else if (Input.GetMouseButton(0) && (currentState == States.ready || currentState == States.touchbegin))
//        {
//            Vector3 touchPosition =  Camera.main.ScreenToWorldPoint(Input.mousePosition);
//            touchPosition.z = -6.0f;
//            touchLocations.Add(touchPosition);
//            currentState = States.touchbegin;
//             GameObject lineDrawn = Instantiate(touchSprite, touchPosition, Quaternion.identity) as GameObject;
//            lineGameObjects.Add(lineDrawn);
//        }
//        else if(currentState == States.touchbegin)
//        {
//            currentState = States.touchend;
//        }
//        else if(currentState == States.touchend)
//        {
//            Vector3 meanPosition = new Vector3();
//            for(int i = 0;i < touchLocations.Count;i ++)
//            {
//                meanPosition.x += touchLocations[i].x;
//                meanPosition.y += touchLocations[i].y;
//            }
//            meanPosition.x /= touchLocations.Count;
//            meanPosition.y /= touchLocations.Count;
//            meanPosition.z = 1000.0f;
//            Debug.Log("centre of points: " + meanPosition);
//            RaycastHit hit;
//            Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(meanPosition));
//			deleteLine ();
//			List<Vector3>smoothPoints =  MakeSmoothCurve (touchLocations, 10.0f);
//			for (int i = 0; i < smoothPoints.Count; i++) {
//				GameObject lineDrawn = Instantiate (touchSprite, smoothPoints[i], Quaternion.identity) as GameObject;
//				lineGameObjects.Add (lineDrawn);
//			}
//            if (Physics.Raycast(ray, out hit))
//            {
//				Debug.Log(hit.collider.name);
//                Debug.Log(hit.collider.transform.position);
//                bodyPartSelected = hit.collider.name;
//               // bodyPartsSelected.Add(hit.collider.name);
//				Camera.main.GetComponent<GappMenuHandler>().clearLocalSymptoms();
//                manager.waitForSymptomSelection();
//            }
//           
//            touchLocations.Clear();
//            currentState = States.ready;
//            
//        }
//
//    }
//	public void invokeDeletion()
//	{
//		deleteLine();
//	}
//    public string getCurrentBodyPart()
//    {
//        return bodyPartSelected;
//    }
//
//  //  public List<string> getBodyPartsSelected()
// //   {
// //       return bodyPartsSelected;
////    }
//    private void deleteLine()
//    {
//        for(int i = 0; i < lineGameObjects.Count; i++)
//        {
//            Destroy(lineGameObjects[i]);
//        }
//        lineGameObjects.Clear();
//    }
//
//	public static List<Vector3> MakeSmoothCurve(List<Vector3> arrayToCurve,float smoothness){
//
//		List<Vector3> points;
//		List<Vector3> curvedPoints = new List<Vector3>();
//		int pointsLength = 0;
//		int curvedLength = 0;
//
//		if(smoothness < 1.0f) smoothness = 1.0f;
//
//		pointsLength = arrayToCurve.Count;
//
//		curvedLength = (pointsLength*Mathf.RoundToInt(smoothness))-1;
//		curvedPoints = new List<Vector3>(curvedLength);
//
//		float t = 0.0f;
//		for(int pointInTimeOnCurve = 0;pointInTimeOnCurve < curvedLength+1;pointInTimeOnCurve++){
//			t = Mathf.InverseLerp(0,curvedLength,pointInTimeOnCurve);
//
//			points = new List<Vector3>(arrayToCurve);
//
//			for(int j = pointsLength-1; j > 0; j--){
//				for (int i = 0; i < j; i++){
//					points[i] = (1-t)*points[i] + t*points[i+1];
//				}
//			}
//
//			curvedPoints.Add(points[0]);
//		}
//
//		return curvedPoints;
//	}
//
//
//
//}
