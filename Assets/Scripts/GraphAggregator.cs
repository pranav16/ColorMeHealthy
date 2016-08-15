﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class GraphAggregator : MonoBehaviour {


	private Dictionary<string,List<BodyPartsTable>> dateToSymtpoms;
	private Dictionary<string,List<BodyPartsTable>> currentSelection;

	private string weekSelected;
	private string monthSelected;
	private string yearSelected;
    private string symptomSelected;

    private WMG_Series painSeries;
    private WMG_Series botherSeries;

    public Dropdown monthDropdown;
	public Dropdown yearDropdown;
	public Dropdown weekDropdown;
    public GameObject emptyGraphPrefab;
    public List<string> monthsLabels;
    public List<Vector2> data;

    // Use this for initialization
    void Start () {
		dateToSymtpoms = new Dictionary<string, List<BodyPartsTable>> ();
		currentSelection = new Dictionary<string, List<BodyPartsTable>> ();
		LoadSymptomsJson ();
		populateDateDropDowns ();
        initGraph();
        symptomSelected = "Pain";
        weekChanged();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void initGraph()
    {

        WMG_Axis_Graph graph = emptyGraphPrefab.GetComponent<WMG_Axis_Graph>();
        List<string> groups = new List<string>();
        groups.Add("None");
        groups.Add("Mild");
        groups.Add("Moderate");
        groups.Add("Severity");
        graph.groups.SetList(groups);
        graph.yAxis.LabelType = WMG_Axis.labelTypes.groups;
        graph.yAxis.AxisNumTicks = groups.Count;
        graph.yAxis.AxisMaxValue = groups.Count - 1;
        setUpXAxis(monthsLabels);
        painSeries = graph.addSeries();
        botherSeries = graph.addSeries();
        botherSeries.lineColor = Color.red;
        painSeries.UseXDistBetweenToSpace = true;
        botherSeries.UseXDistBetweenToSpace = true;
      
    }

    private void setUpXAxis(List<string> LabelSet)
    {
        WMG_Axis_Graph graph = emptyGraphPrefab.GetComponent<WMG_Axis_Graph>();
        graph.xAxis.AxisNumTicks = LabelSet.Count;
        graph.xAxis.AxisMaxValue = LabelSet.Count - 1;
        graph.xAxis.axisLabels.SetList(LabelSet);
    }
    private void clearGraph()
    {
        painSeries.pointValues.Clear();
        botherSeries.pointValues.Clear();
    }

    public void SymptomSelected(string symtom)
    {
        symptomSelected = symtom;
        weekChanged(); 
    }


    private void setPainSeries(List<Vector2>dataSet)
    {
        painSeries.pointValues.SetList(dataSet);
    }
    private void setBotherSeries(List<Vector2> dataSet)
    {
        botherSeries.pointValues.SetList(dataSet);
    }

    public void monthChanged ()
	{
        clearGraph();
        weekDropdown.interactable = true;
		monthSelected = monthDropdown.options [monthDropdown.value].text;
		aggregateEntriesForTheDate ();
	}

	public void yearChanged()
	{
        clearGraph();
        yearSelected = yearDropdown.options[yearDropdown.value].text;
		aggregateEntriesForTheDate ();
	}

	public void weekChanged()
	{
        clearGraph();
        weekSelected = weekDropdown.options [weekDropdown.value].text;
		aggregateEntriesForTheDate ();
        int week = int.Parse(weekSelected);
        List<Vector2> painData = new List<Vector2>();
        List<Vector2> botherData = new List<Vector2>();
        List<string> xLabels = new List<string>();
        int xValue = 0;
        foreach (string date in currentSelection.Keys)
        {
            string[] dateSplit = date.Split('_');
            int day = int.Parse(dateSplit[1]);
         
            if(day <= week * 7 && day < (week * 7) + 2)
            {
              xLabels.Add(date);
              List<BodyPartsTable> tables =  currentSelection[date];
                foreach(BodyPartsTable table in tables)
                    if(table.getPartName().Contains("General Symptoms"))
                    {
                        foreach(symptoms symtom in table.getSymptoms())
                        {
                            if (symtom.name.Contains(symptomSelected))
                            {
                                int pain = 0;
                                int bother = 0;
                                if (symtom.painScale > 0)
                                    pain = (int)symtom.painScale;
                                if (symtom.botherScale > 0)
                                    bother = (int)symtom.botherScale;
                                painData.Add(new Vector2(xValue, pain));
                                botherData.Add(new Vector2(xValue, bother));
                                
                            }
                        }
                    }
                xValue++;
            }
        }
        clearGraph();
        setUpXAxis(xLabels); 
        setBotherSeries(botherData);
        setPainSeries(painData);
	}


	private void aggregateEntriesForTheDate()
	{
		currentSelection.Clear ();
		foreach (string date in dateToSymtpoms.Keys) {
			string[] dateSplit	= date.Split ('_');
			string month = dateSplit[0];
			string year = dateSplit[2];
			if (monthSelected == month && yearSelected == year) {
				//if it hits the if part then your data set is not unique..which is not cool!
				if (currentSelection.ContainsKey (date))
					currentSelection [date] = dateToSymtpoms [date];
				else
					currentSelection.Add (date,dateToSymtpoms[date]);
			}
		}
 

	}

	bool populateDateDropDowns()
	{
		List<string> months = new List<string> ();
		List<string> years = new List<string> ();
		string previousMonth = "";
		string previousYear = "";
		foreach (string date in dateToSymtpoms.Keys) {
			string[] dateSplit	= date.Split('_');
			if (previousYear != dateSplit[2]) {
				years.Add (dateSplit[2]);
				previousYear = dateSplit[2];
			}

			if(previousMonth != dateSplit[0])
			{
				months.Add (dateSplit[0]);
				previousMonth = dateSplit[0];
			}


		}
		monthDropdown.AddOptions (months);
		yearDropdown.AddOptions (years);

		monthSelected = monthDropdown.options [monthDropdown.value].text;
		yearSelected = yearDropdown.options[yearDropdown.value].text;
		return true;
	}


	void LoadSymptomsJson()
	{

		string filePath = "Symptoms.json";
		string fileName = Application.dataPath + "/document.json";//Application.persistentDataPath + "/Color" + filePath;
		if (!System.IO.File.Exists (fileName))return;
		string rawjson = System.IO.File.ReadAllText(fileName);
		Debug.Log("is file present");
		JSONObject mainJson = new JSONObject(rawjson);
		JSONObject Entries = mainJson.GetField("Entires");
		foreach (JSONObject obj in Entries.list) {
			List<BodyPartsTable> table = new List<BodyPartsTable> ();
			if (obj.GetField ("BodyParts").list == null)
				continue;

			JSONObject bps =  obj.GetField ("BodyParts");
			if (bps == null)
				continue;
			foreach (string keys in bps.keys)
			{
				JSONObject bodypartJson = bps.GetField (keys);
				BodyPartsTable bodyPart = new BodyPartsTable();
				bodyPart.setPartName (keys);
				bodyPart.setImagePath (bodypartJson.GetField("imgSrc").str);
				bodyPart.setTimeStamp (bodypartJson.GetField("timestamp").str);
				JSONObject symptoms = bodypartJson.GetField ("symptoms");
				foreach(JSONObject sy in symptoms.list)
				{
					symptoms symptom = new symptoms ();
					symptom.name = sy.GetField("symptomname").str;
					symptom.painScale = sy.GetField("painscale").f ;
					symptom.botherScale = sy.GetField("bothersomescale").f ;
					bodyPart.addSymptoms (symptom);
				}
				table.Add (bodyPart);
			}
			if (dateToSymtpoms.ContainsKey (obj.GetField ("Date").str))
				dateToSymtpoms [obj.GetField ("Date").str] = table;
			else
				dateToSymtpoms.Add (obj.GetField ("Date").str, table);
		}

	}

}
