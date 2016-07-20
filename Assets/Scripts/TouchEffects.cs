using UnityEngine;
using System.Collections;

public class TouchEffects : MonoBehaviour
{

	static TouchEffects instance = null;

	// Use this for initialization
	void Awake()
	{
		if (instance == null) {
			instance = this;
		} else if (instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad (gameObject);
	}

	void Start () {

	}

	// Update is called once per frame
	void Update ()
	{

	

		if (Input.GetMouseButtonDown (0)) {
			playAnimation ();
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			transform.position = new Vector3 (mousePosition.x, mousePosition.y, -6.0f);
		}
	
	
	}

	void playAnimation ()
	{
		Debug.Log ("playanimation");
		int random = Random.Range (1, 4);
		Debug.Log (random);
		GetComponent<Animator> ().SetTrigger ("Effect" + random);
		GetComponent<SpriteRenderer> ().color = setRandomColor ();
	}


	Color setRandomColor ()
	{
		int random = Random.Range (0, 7);
		switch (random) {
		case 0:
			return Color.black;
		case 1:
			return Color.red;
		case 2:
			return Color.green;
		case 3:
			return Color.grey;
		case 4:
			return Color.cyan;
		case 5:
			return Color.magenta;
		case 6:
			return Color.blue;
		default:
			return Color.blue;
		}
	}

}
