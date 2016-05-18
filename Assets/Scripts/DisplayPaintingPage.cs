using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DisplayPaintingPage : MonoBehaviour {

	public List<Image> painingImg;
	int currentImageIndex;
	int maxImageIndex;

	public List<Text> imageName;

	// Use this for initialization
	void Start () {

		setIntinalImage ();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setIntinalImage()
	{
		maxImageIndex =  currentImageIndex = PlayerPrefs.GetInt("paintingNumber",0);
		maxImageIndex -= 1;
		currentImageIndex -= 1;
		if ((maxImageIndex + 1) % 2 == 0) {
			string filePath = Application.persistentDataPath + "/Paintings" + currentImageIndex + ".png";
			imageName [1].text = PlayerPrefs.GetString (currentImageIndex - 1 + ".png", "");
			changeImage (filePath, 1);
			currentImageIndex--;
			imageName [0].text = PlayerPrefs.GetString (currentImageIndex - 1 + ".png", "");
			filePath = Application.persistentDataPath + "/Paintings" + currentImageIndex + ".png";
			changeImage (filePath, 0);
		
		} else {
			imageName [0].text = PlayerPrefs.GetString (currentImageIndex - 1 + ".png", "");
			string filePath = Application.persistentDataPath + "/Paintings" + currentImageIndex + ".png";
			changeImage (filePath, 0);

		}

	}

	public void LeftClicked()
	{
		currentImageIndex--;
		if (0 >= currentImageIndex) {
			currentImageIndex = 0;
			return;
		}
		string filePath = Application.persistentDataPath  +"/Paintings" + currentImageIndex + ".png";
		imageName[0].text = PlayerPrefs.GetString (currentImageIndex -1 + ".png", "");
		changeImage(filePath,0);
		 int index = currentImageIndex + 1;
	
		imageName[1].text = PlayerPrefs.GetString (index -1  + ".png", "");
		filePath = Application.persistentDataPath  +"/Paintings" +index  + ".png";
		changeImage(filePath,1);

	}

	public void RightClicked()
	{
		currentImageIndex++ ;
		if (maxImageIndex <= currentImageIndex) {
			currentImageIndex = maxImageIndex;
			return;
		}
		string  filePath = Application.persistentDataPath  +"/Paintings" + currentImageIndex + ".png";
		imageName[0].text = PlayerPrefs.GetString (currentImageIndex -1 + ".png", "");
		changeImage(filePath,0);
		int index = currentImageIndex  + 1;

		imageName[1].text = PlayerPrefs.GetString (index -1 + ".png", "");
		filePath = Application.persistentDataPath  +"/Paintings" +index  + ".png";
		changeImage(filePath,1);
	
	
	
	
	}

	public void changeImage(string path,int index)
	{
		if(!System.IO.File.Exists(path)) return;
		byte [] textureData = System.IO.File.ReadAllBytes (path);
		Texture2D tex = new Texture2D(Screen.width/2,Screen.height);
		tex.LoadImage (textureData);
		Sprite sprite = Sprite.Create (tex,new Rect(0,0,Screen.width/2,Screen.height),new Vector2(0.5f,0.5f));
		painingImg[index].sprite = sprite;
	}

	public void backClicked()
	{
		SceneManager.LoadScene ("PaintScreen");
	}

}
