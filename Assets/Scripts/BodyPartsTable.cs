using System.Collections.Generic;
using System.Collections;

public struct symptoms
{
	public string name;
	public float painScale;
	public float botherScale;


}

public class BodyPartsTable  {

    private string partName;
	private string imageFilePath;
	List<symptoms> symptoms;
	private string timeStamp;

     public BodyPartsTable()
    {
		timeStamp = string.Format ("{0:HH:mm }", System.DateTime.Now);// System.DateTime.Now.TimeOfDay.ToString();
		symptoms = new List<symptoms>();

    }
    public void addSymptoms(symptoms symptom)
    {
        symptoms.Add(symptom);
    }

	public List<symptoms> getSymptoms()
    {
        return symptoms;
    }

    public void setPartName(string name)
    {
        partName = name;
    }

    public string getPartName()
    {
        return partName;
    }
	public void setImagePath(string name)
	{
		imageFilePath = name;
	}

	public string getImagePath()
	{
		return imageFilePath;
	}
	public string getTimeStamp()
	{
		return timeStamp;
	}
	public void setTimeStamp(string time)
	{
		 timeStamp = time;
	}

}
