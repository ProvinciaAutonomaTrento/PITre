using System;
using System.Collections;
using log4net;

namespace BusinessLogic.Report
{
	/// <summary>
	/// </summary>
	public class Corrispondenti 
	{
        private static ILog logger = LogManager.GetLogger(typeof(Corrispondenti));
		/// <summary></summary>
		/// <param name="qco"></param>
		/// <returns></returns>
		public static DocsPaVO.documento.FileDocumento stampa(DocsPaVO.addressbook.QueryCorrispondente qco) 
		{
			//string path = ReportUtils.getPathName() + "\\corrispondenti\\";			
			string path = AppDomain.CurrentDomain.BaseDirectory + "report\\corrispondenti\\";

			//string path = DocsPaUtils.Functions.Functions.GetReportsPath() + "\\corrispondenti\\";			
			DocsPaVO.documento.FileDocumento result = new DocsPaVO.documento.FileDocumento();
			
			try 
			{
				ArrayList report = new ArrayList();
				string tmpStr = "";

				#region Testata
				tmpStr = ReportUtils.stringFile(path + "Header.txt");

				// Registri
				string registro = "COMUNI A TUTTI";
				if(qco.idRegistri.Count > 0) 
				{
					//db.openConnection();
					registro = BusinessLogic.Utenti.RegistriManager.getRegistro((string)qco.idRegistri[0]).descrizione;				
					//db.closeConnection();
				}
				tmpStr = tmpStr.Replace("XREGISTRO", registro);

				// Tipo corrispondente
				string tipo = "INTERNI/ESTERNI";
				if(qco.tipoUtente==DocsPaVO.addressbook.TipoUtente.INTERNO)
					tipo = "INTERNI";
				else if(qco.tipoUtente==DocsPaVO.addressbook.TipoUtente.ESTERNO)
					tipo = "ESTERNI";

				tmpStr = tmpStr.Replace("XFILTRO", tipo);
				ReportUtils.addStringToReport(tmpStr, ref report);
				#endregion Testata

				#region Tabella
				//testata
				tmpStr = ReportUtils.stringFile(path + "TableHead.txt");
				ReportUtils.addStringToReport(tmpStr, ref report);

				//righe
				string rowString = ReportUtils.stringFile(path + "Row.txt");
				qco.getChildren = false;

				// UO
				qco.descrizioneUO = "";
				addRows(BusinessLogic.Utenti.addressBookManager.getListaCorrispondenti(qco), rowString, ref report);
				qco.descrizioneUO = null;
				
				// Ruoli
				qco.descrizioneRuolo = "";
				addRows(BusinessLogic.Utenti.addressBookManager.getListaCorrispondenti(qco), rowString, ref report);
				qco.descrizioneRuolo = null;

				// Utenti
				qco.cognomeUtente = "";
				ArrayList lista = BusinessLogic.Utenti.addressBookManager.getListaCorrispondenti(qco);
				if(qco.tipoUtente!=DocsPaVO.addressbook.TipoUtente.INTERNO) {		
					ArrayList listaSciolti = BusinessLogic.Utenti.addressBookManager.listaCorrEstSciolti(qco);
					for (int i=0; i < listaSciolti.Count; i++)
						lista.Add(listaSciolti[i]);
				}
				addRows(lista, rowString, ref report);
												
				#endregion Tabella

				#region Bottom
				tmpStr = ReportUtils.stringFile(path + "Bottom.txt");
				ReportUtils.addStringToReport(tmpStr, ref report);
				#endregion Bottom

				result.content = (byte[]) report.ToArray(typeof(byte));
				result.length = result.content.Length;
				result.contentType = "application/msword";
				result.name = "report.rtf";
			}
			catch(Exception e) 
			{
				logger.Debug("Errore nella gestione di Report (stampa)",e);
				throw e;
			}
			return result;
		}

		/// <summary></summary>
		/// <param name="lista"></param>
		/// <param name="rowString"></param>
		/// <param name="report"></param>
		private static void addRows(ArrayList lista, string rowString, ref ArrayList report) 
		{		
			for (int i=0;i<lista.Count;i++)
				addRowInTable((DocsPaVO.utente.Corrispondente)lista[i], rowString, ref report);


//			Hashtable htInseriti = new Hashtable();	
//			
//			for (int i=0; i < lista.Count; i++) {	
//				string idCorrispondente = ((DocsPaVO.utente.Corrispondente)lista[i]).systemId;
//				if(!htInseriti.ContainsKey(idCorrispondente))	
//					htInseriti.Add(idCorrispondente, lista[i]);
//			}
//			lista = new ArrayList(htInseriti.Values);
//			for (int i=0; i < lista.Count; i++) 
//				addRowInTable((DocsPaVO.utente.Corrispondente)lista[i], rowString, ref report);
		}

		/// <summary></summary>
		/// <param name="corr"></param>
		/// <param name="rowString"></param>
		/// <param name="report"></param>
		private static void addRowInTable(DocsPaVO.utente.Corrispondente corr, string rowString, ref ArrayList report) 
		{		
			string tmpStr = rowString;

			// tipo
			string tipo = "";
			if(corr.GetType().Equals(typeof(DocsPaVO.utente.Utente)))
				tipo = "Utente";
			else if(corr.GetType().Equals(typeof(DocsPaVO.utente.Ruolo)))
				tipo = "Ruolo"; 
			else if(corr.GetType().Equals(typeof(DocsPaVO.utente.UnitaOrganizzativa)))
				tipo = "UO"; 
			tmpStr = tmpStr.Replace("XTIPO", tipo);

			// codice
			tmpStr = tmpStr.Replace("XCODICE", corr.codiceRubrica);

			// descrizione
			tmpStr = tmpStr.Replace("XDESCRIZIONE", corr.descrizione);

			// email
			string email = "";
			if(corr.email != null)
				email = corr.email;
			tmpStr = tmpStr.Replace("XEMAIL", email);

			// tipo I/E
			tmpStr = tmpStr.Replace("XIE", corr.tipoIE);
					
			ReportUtils.addStringToReport(tmpStr, ref report);
		}
	}
}
