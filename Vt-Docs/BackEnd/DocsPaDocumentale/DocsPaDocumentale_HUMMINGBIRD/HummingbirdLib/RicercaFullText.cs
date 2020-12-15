using System;
using System.Collections;
using DocsPaVO.utente;
using log4net;

namespace DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib
{
	/// <summary>
	/// Classe per la gestione delle ricerche estesa
	/// nella libreria Hummingbird.
	/// Estende la classe "Ricerca" (che gestisce gli accessi
	/// a basso livello all'oggetto PCDSearch della libreria Hummingbird)
	/// fornendo il servizio per la ricerca estesa.
	/// </summary>
	public class RicercaFullText
	{
        private ILog logger = LogManager.GetLogger(typeof(RicercaFullText));
		private const int PAGE_SIZE=20;
		private const string FULL_TEXT_MAX_ROWS="FULL_TEXT_MAX_ROWS";

		#region Costruttori

		public RicercaFullText()
		{
		}

		#endregion

		/// <summary>
		/// Reperimento limite massimo di record che è possibile estrarre dal documentale
		/// </summary>
		/// <returns></returns>
		public int GetMaxRows()
		{
			int retValue=0;

			string configValue=System.Configuration.ConfigurationManager.AppSettings[FULL_TEXT_MAX_ROWS];
			
			if (configValue!=null && configValue!=string.Empty)
			{
				try
				{
					retValue=Convert.ToInt32(configValue);
				}
				catch
				{
				}
			}

			if (retValue<0)
				retValue=0;

			return retValue;
		}

		/// <summary>
		/// Avvio ricerca fulltext nel documentale hummingbird
		/// </summary>
		/// <param name="infoUtente"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		private string[] SearchHummingbird(InfoUtente infoUtente,
										   DocsPaVO.ricerche.FullTextSearchContext context)
		{
			string[] retValue=new string[0];

			//Creiamo l'oggetto per la ricerca
			PCDCLIENTLib.PCDSearch objSearch = new PCDCLIENTLib.PCDSearchClass();

			// Impostazione DST e libreria, indispensabili per la connessione ad Hummingbird
			objSearch.SetDST(infoUtente.dst);
			objSearch.AddSearchLib(this.GetLibreria(infoUtente.idAmministrazione)); 
			// Aggiunta clausola from
			objSearch.SetSearchObject("DEF_PROF");

			//Aggiungiamo la clausola where
			objSearch.AddSearchCriteria("FULLTEXT_CONTENT_PROFILE",context.TextToSearch);

			//Aggiungiamo la clausola where
			objSearch.AddReturnProperty("DOCNUM");
			objSearch.AddReturnProperty("DOCNAME");
			objSearch.AddReturnProperty("SYSTEM_ID");
			objSearch.AddOrderByProperty("DOCNUM",0);
			
			int maxRows=this.GetMaxRows();
			if (maxRows>0)
				objSearch.SetMaxRows(maxRows);

			//Eseguiamo la ricerca
			objSearch.Execute();

			int numDocs=0;

			if(objSearch.ErrNumber != 0)
			{

			}
			else
			{
				numDocs=objSearch.GetRowsFound();

				if(numDocs != 0)
					// Reperimento chiavi di ricerca
					retValue=this.GetSearchDocNumbers(objSearch);
			}

			objSearch.ReleaseResults();
			objSearch=null;

			return retValue;
		}

		/// <summary>
		/// Ricerca fulltext
		/// </summary>
		/// <param name="infoUtente"></param>
		/// <param name="context">Informazioni di contesto per la ricerca</param>
		/// <returns></returns>
		public ArrayList RicercaFT(InfoUtente infoUtente,ref DocsPaVO.ricerche.FullTextSearchContext context)
		{
			string[] resultList=null;

			if (context.SearchResultList!=null && context.SearchResultList.Length>0)
			{
				// Ricerca già effettuata, reperimento dall'oggetto di contesto
				// dei risultati precedenti evitando così una dispendiosa 
				// chiamata al sistema documentale
				resultList=context.SearchResultList;
			}
			else
			{
				// Prima ricerca, chiamata al documentale
				resultList=this.SearchHummingbird(infoUtente,context);
				
				context.SearchResultList=resultList;
			}

			if (resultList.Length>0)
				// Estrazione dei soli id relativi alla pagina richiesta
				resultList=this.ExtractPageDocNumbers(resultList,context.RequestedPageNumber);

			// Calcolo numero pagine e record estratti
			int numDocs=context.SearchResultList.Length;
			int numTotPage=(numDocs / PAGE_SIZE);

			if(numTotPage * PAGE_SIZE < numDocs)
				numTotPage++;

			context.TotalPageNumber=numTotPage;
			context.TotalRecordCount=numDocs;

			// Reperimento dettagli sui documenti
			return this.GetListaDocumenti(resultList,infoUtente);
		}

		/// <summary>
		/// Ricerca fulltext su Hummingbird
		/// </summary>
		/// <param name="testo">testo da ricercare, sia nell'oggetto, sia nel contenuto dei documenti</param>
		/// <param name="libreria">libreria sulla quale eseguire la ricerca</param>
		/// <param name="dst">dst per la connessione alla libreria specificata</param>
		/// <param name="infoUtente">oggetto di contesto utente</param>
		/// <returns>ArrayList contenente le informazioni dei documenti di interesse</returns>
		public ArrayList RicercaFT(string testo, 
									string idReg,
									InfoUtente infoUtente,
									int numPage,
									out int numTotPage,
									out int nRec)
		{
			logger.Debug("RicercaFT");

			numTotPage=0;
			nRec=0;

			//creiamo la stringa da usare per la ricerca delle info dei docs
			//che conterrà tutti i system_id recuperati dalla ricerca fulltext
			ArrayList resultSet = new ArrayList();

			//controllo sulla stringa
			testo = testo.Replace("'","''");

			//Creiamo l'oggetto per la ricerca
			PCDCLIENTLib.PCDSearch objSearch = new PCDCLIENTLib.PCDSearchClass();

			//impostiamo DST e libreria, indispensabili per la connessione ad Hummingbird
			objSearch.SetDST(infoUtente.dst);
			objSearch.AddSearchLib(this.GetLibreria(infoUtente.idAmministrazione)); // ESTRARRE LA LIBRERIA
			//Aggiungiamo la clausola from
			objSearch.SetSearchObject("DEF_PROF");

			//Aggiungiamo la clausola where
			objSearch.AddSearchCriteria("FULLTEXT_CONTENT_PROFILE",testo);

			//Aggiungiamo la clausola where
			objSearch.AddReturnProperty("DOCNUM");
			objSearch.AddReturnProperty("DOCNAME");
			objSearch.AddReturnProperty("SYSTEM_ID");
			objSearch.AddOrderByProperty("DOCNUM",1);

			objSearch.SetMaxRows(100);

			//Eseguiamo la ricerca
			objSearch.Execute();

			int numDocs=0;

			//Controlliamo eventuali errori
			if(objSearch.ErrNumber != 0)
			{
				//errore
			}
			else
			{
				numDocs=objSearch.GetRowsFound();

				if(numDocs != 0)
				{	
					// Reperimento chiavi di ricerca
					string[] docNumbers=this.GetSearchDocNumbers(objSearch);

					if (docNumbers.Length>0)
						// Estrazione dei soli id relativi alla pagina richiesta
						docNumbers=this.ExtractPageDocNumbers(docNumbers,numPage);

					// Reperimento dettagli sui documenti
					resultSet=this.GetListaDocumenti(docNumbers,infoUtente);
				}
			}

			objSearch.ReleaseResults();
			objSearch=null;
			
			numTotPage=(numDocs / PAGE_SIZE);

			if(numTotPage * PAGE_SIZE < numDocs)
				numTotPage++;

			nRec=numDocs;

			return resultSet;
		}

		public bool SetProfileFT(string docnumber,InfoUtente infoUtente)
		{
			logger.Debug("SetProfileFT");

			PCDCLIENTLib.PCDDocObject docObj = new PCDCLIENTLib.PCDDocObject();

			// impostazione del dst
			docObj.SetDST(infoUtente.dst);

			//Impostazione della form di interesse
			docObj.SetObjectType("DEF_PROF");

			//Impostiazione della libreria
			docObj.SetProperty("%TARGET_LIBRARY",this.GetLibreria(infoUtente.idAmministrazione));

			docObj.SetProperty("%OBJECT_IDENTIFIER", docnumber);

			//Impostazione del campo fulltext per abilitare l'indicizzazione
			docObj.SetProperty("FULLTEXT","Y");

			//Esecuzione dell'update
			int errorNumber = docObj.Update();

			if(errorNumber == 0)
			{
				return true;
			}
			else
			{
				logger.Debug("ERRORE: " + docObj.ErrDescription);
				return true; //NON BLOCCA L'ACQUISIZIONE SE NON FUNZIONA L'INDICIZZAZIONE LO SEGNALO SOLAMENTE.
			}
		}
 

		private ArrayList GetListaDocumenti(string[] docNumbers,InfoUtente infoUtente)
		{
			DocsPaDB.Query_DocsPAWS.RicercaFullText ricercaFT=new DocsPaDB.Query_DocsPAWS.RicercaFullText();
			return ricercaFT.GetDocumenti(docNumbers,infoUtente);
		}

		private string GetLibreria(string idAmministrazione)
		{
			DocsPaDB.Query_DocsPAWS.RicercaFullText ricercaFT=new DocsPaDB.Query_DocsPAWS.RicercaFullText();
			return ricercaFT.GetLibreria(idAmministrazione);
		}

		/// <summary>
		/// Creazione di un array di stringhe contenente tutti i systemid 
		/// estratti dalla ricerca fulltext
		/// </summary>
		/// <param name="objSearch"></param>
		/// <param name="requestedPage"></param>
		/// <param name="numDocs"></param>
		/// <returns></returns>
		private string[] GetSearchDocNumbers(PCDCLIENTLib.PCDSearch objSearch)
		{
			ArrayList arrayList=new ArrayList();

			int numDocs=objSearch.GetRowsFound();

			if (numDocs>0)
			{
				for (int i=0;i<numDocs;i++)
				{
					if (objSearch.NextRow()>0)
						arrayList.Add(objSearch.GetPropertyValue("SYSTEM_ID").ToString());
					else
						break;
				}
			}

			return (string[]) arrayList.ToArray(typeof(string));
		}

		private string[] ExtractPageDocNumbers(string[] docNumbers,int requestedPage)
		{
			ArrayList retValue=new ArrayList();

			int startRow=(requestedPage * PAGE_SIZE) - PAGE_SIZE;
			for (int i=startRow; i<(startRow + PAGE_SIZE); i++)
			{
				if (docNumbers.Length<=i)
					break;

				retValue.Add(docNumbers[i]);
			}

			return (string[]) retValue.ToArray(typeof(string));
		}
	}
}
