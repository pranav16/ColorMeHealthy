using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using sharpPDF;
using sharpPDF.Enumerators;
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class PdfExporter : MonoBehaviour {

	// Use this for initialization
	string		attacName;
	PdfExporter instance;
	TranslatorSnoMed translateToSnowMed;
	// Use this for initialization
	void Start () {
		if (instance == null) {
			instance = this;
			instance.init ();
		} else if (instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad (gameObject);
	}

	public void init()
	{
		attacName = Application.persistentDataPath + "/ColorMeHealthy.pdf";
		translateToSnowMed = new TranslatorSnoMed ();
		translateToSnowMed.Init ();
	}

	// Update is called once per frame
	public void CreatePDF (Dictionary<string,BodyPartsTable>finalSymptoms) {

		if (System.IO.File.Exists (attacName))
			System.IO.File.Delete (attacName);
		pdfDocument myDoc = new pdfDocument("ColorMeHealthy","Me", false);
		foreach (string key in finalSymptoms.Keys) {
		pdfPage myFirstPage = myDoc.addPage();
			string header = key;
			if (translateToSnowMed.isKeyPresent (key))
				header = translateToSnowMed.getSnoMedTermForUi (key).snoMedTerm;

			myFirstPage.addText(header,10,730,predefinedFont.csHelveticaOblique,30,new pdfColor(predefinedColor.csDarkRed));
			//format general symptoms in its very own special way
			if (key.Contains("General Symptoms")) {
				int height = 700;
				int fontsize = 20;
				BodyPartsTable bodyParts = finalSymptoms [key];
				foreach (symptoms symptom in  bodyParts.getSymptoms ()) {
					if (symptom.name.Contains ("os_")) {
						myFirstPage.addText ("Any other symptoms?", 0, height, predefinedFont.csCourier, fontsize,new pdfColor(predefinedColor.csDarkRed));
						height -= fontsize;
						myFirstPage.addText (symptom.name.Replace ("os_", ""), 0, height, predefinedFont.csCourier, fontsize);
						height -= fontsize;
					} else if (symptom.name.Contains ("Bothersome_")) {
						myFirstPage.addText ("What is bothering you the most?", 0,height, predefinedFont.csCourier, fontsize,new pdfColor(predefinedColor.csDarkRed));
						height -= fontsize;
						myFirstPage.addText (symptom.name.Replace ("Bothersome_", ""), 0,height, predefinedFont.csCourier, fontsize);
						height -= fontsize;
					} else if (symptom.name.Contains ("FeelingToday_")) {
						myFirstPage.addText ("How are you feeling today?", 0, height, predefinedFont.csCourier, fontsize,new pdfColor(predefinedColor.csDarkRed));
						height -= fontsize;
						myFirstPage.addText (symptom.name.Replace ("FeelingToday_", ""), 0, height, predefinedFont.csCourier, fontsize);
						height -= fontsize;
					} else if (symptom.name.Contains ("Bestthing_")) {
						myFirstPage.addText ("What is the best thing about today?", 0, height, predefinedFont.csCourier, fontsize,new pdfColor(predefinedColor.csDarkRed));
						height -= fontsize;
						myFirstPage.addText (symptom.name.Replace ("Bestthing_", ""), 0, height, predefinedFont.csCourier, fontsize);
						height -= fontsize;
					} else if (symptom.botherScale >= 0) {
						myFirstPage.addText (symptom.name.Replace ("?", "?  Yes"), 0, height, predefinedFont.csCourier, fontsize,new pdfColor(predefinedColor.csDarkRed));
						height -= fontsize;
						myFirstPage.addText ("Symptom Severity (attribute): " + symptomPointsToText((int)symptom.painScale), 0, height, predefinedFont.csCourier, fontsize);
						height -= fontsize;
						myFirstPage.addText ("Distress (finding): " + symptomPointsToText((int)symptom.botherScale), 0, height, predefinedFont.csCourier, fontsize);
						height -= fontsize;
					} else if (symptom.name.Contains ("?") && symptom.botherScale == -1.0f) {
						myFirstPage.addText (symptom.name.Replace ("?", "?  Yes"), 0, height, predefinedFont.csCourier, fontsize,new pdfColor(predefinedColor.csDarkRed));
						height -= fontsize;
						if (symptom.name.Contains ("throw")) {
							myFirstPage.addText ("How many times ?: " + (symptom.painScale + 1).ToString("0.0"), 0, height, predefinedFont.csCourier, fontsize);
							height -= fontsize;
						}
					}
				}
				continue;
			}
			/*Table's creation*/
			pdfTable myTable = new pdfTable ();
			//Set table's border
			myTable.borderSize = 1;
			myTable.borderColor = new pdfColor (predefinedColor.csDarkBlue);

			/*Add Columns to a grid*/

	
			myTable.tableHeader.addColumn (new pdfTableColumn ("Symptom", predefinedAlignment.csCenter, 200));
			myTable.tableHeader.addColumn (new pdfTableColumn ("Symptom Severity (attribute)", predefinedAlignment.csLeft, 150));
			myTable.tableHeader.addColumn (new pdfTableColumn ("Distress (finding)", predefinedAlignment.csLeft, 150));

	

			BodyPartsTable table = finalSymptoms [key];
			foreach (symptoms symptom in  table.getSymptoms ()) {
				pdfTableRow myRow = myTable.createRow ();
				string symptomName = symptom.name;
				if (translateToSnowMed.isKeyPresent (symptomName))
					symptomName = translateToSnowMed.getSnoMedTermForUi(symptomName).snoMedTerm;
					myRow [0].columnValue = symptomName;
				myRow [1].columnValue = symptomPointsToText((int)symptom.painScale);
				myRow [2].columnValue = symptomPointsToText((int)symptom.botherScale);

				myTable.addRow (myRow);
			}

			if (System.IO.File.Exists (table.getImagePath ())) {
				byte [] textureData = System.IO.File.ReadAllBytes (table.getImagePath ());
				myFirstPage.addImage (textureData, 0,0,400,400);
			}
			/*Set Header's Style*/
			myTable.tableHeaderStyle = new pdfTableRowStyle (predefinedFont.csCourierBoldOblique, 12, new pdfColor (predefinedColor.csBlack), new pdfColor (predefinedColor.csLightRed));
			/*Set Row's Style*/
			myTable.rowStyle = new pdfTableRowStyle (predefinedFont.csCourier, 8, new pdfColor (predefinedColor.csBlack), new pdfColor (predefinedColor.csWhite));
			/*Set Alternate Row's Style*/
			myTable.alternateRowStyle = new pdfTableRowStyle (predefinedFont.csCourier, 8, new pdfColor (predefinedColor.csBlack), new pdfColor (predefinedColor.csLightRed));
			/*Set Cellpadding*/
			myTable.cellpadding = 10;
			/*Put the table on the page object*/
			myFirstPage.addTable (myTable, 5, 700);
		}

		//yield return StartCoroutine ( myFirstPage.newAddImage (  "FILE://picture1.jpg",2,100 ) );

		myDoc.createPDF(attacName);
		mail ();
		//myTable = null;
	}

	public string symptomPointsToText(int value)
	{
		switch (value)
		{
		case 1:
			return "Symptom mild (finding) ";
		case 2 : return "Symptom moderate (finding)";
		case 3 :return "Symptom severe (finding)";
		default :return "None" ;
		}
	}
		
	public void mail()
	{

		MailMessage mail = new MailMessage();

		mail.From = new MailAddress("thecolormehealthyapp@gmail.com");
		//mail.To.Add("mitchell.eastwold@hotmail.com");
		mail.To.Add("thecolormehealthyapp@gmail.com");
		mail.Subject = "Color Me Healthy -- Report";
		mail.Body = "Attached your patient's report";
		mail.Attachments.Add(new Attachment(attacName));
		SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
		smtpServer.Port = 587;
		smtpServer.Credentials = new System.Net.NetworkCredential("thecolormehealthyapp@gmail.com", "Gapplab1") as ICredentialsByHost;
		smtpServer.EnableSsl = true;
		ServicePointManager.ServerCertificateValidationCallback = 
			delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) 
		{ return true; };
		smtpServer.Send(mail);
		Debug.Log("success");
	}


}
