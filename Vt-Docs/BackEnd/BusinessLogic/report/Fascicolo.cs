using System;
using System.IO;
using System.Configuration;
using System.Collections;
using StampaPDF;
using BusinessLogic.Report;
using DocsPaVO.documento;

namespace BusinessLogic.Report
{
	public class FascicoloReport
	{
		public static DocsPaVO.documento.FileDocumento StampaFascetta(DocsPaVO.fascicolazione.Fascicolo fascicolo) 
		{
			return CreateFileDocumentoRTF(GetReportData(fascicolo.codice,fascicolo.descrizione));
		}

		private static Byte[] GetReportData(string codice,string descrizione)
		{
			string templatePath = AppDomain.CurrentDomain.BaseDirectory + @"report/fascicolo/Fascetta.txt";
			string report=ReportUtils.stringFile(templatePath);
			codice=codice.Replace("\\","\\\\");
			report = report.Replace("XCODICE", codice);
			descrizione=descrizione.Replace("\\","\\\\");
			report = report.Replace("XDESCRIZIONE", descrizione);
			return ReportUtils.toByteArray(report);
		}

		private static FileDocumento CreateFileDocumentoRTF(Byte[] fileContent)
		{
			FileDocumento fileDoc=new DocsPaVO.documento.FileDocumento();
			fileDoc.name = "StampaFascetteFascicolo.RTF";
			fileDoc.path = "";
			fileDoc.fullName = '\u005C'.ToString() + fileDoc.name;
			fileDoc.length = (int) fileContent.Length;
			fileDoc.content = fileContent;		
			fileDoc.contentType ="application/rtf";
			return fileDoc;
		}
	}
}
