using System;
using log4net;

namespace BusinessLogic.Documenti
{
	/// <summary>
	/// Summary description for CateneDocManager.
	/// </summary>
	public class CateneDocManager
	{
        private static ILog logger = LogManager.GetLogger(typeof(CateneDocManager));

		public static DocsPaVO.documento.AnelloDocumentale getCatenaDoc(string idGruppo, string idPeople, string idProfile)
		{
			logger.Debug("getCatenaDoc");
			DocsPaVO.documento.AnelloDocumentale result=new DocsPaVO.documento.AnelloDocumentale();
			//DocsPaWS.Utils.Database db= DocsPaWS.Utils.dbControl.getDatabase();
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			System.Data.DataSet dataSet;
			//bool dbOpen=false;
			try
			{
				//db.openConnection();
			    //dbOpen=true;
			   
			    //si trova la root del documento
			    string idRoot=getIdRoot(idProfile);
			   
			    //si carica in una tabella l'albero relativo a tale root
                /*string alberoString="SELECT ID_DOCUMENTO,ID_DOC_COLLEGATO FROM DPA_DOC_COLLEGAMENTI WHERE ID_ROOT="+idRoot;
			    db.fillTable(alberoString,dataSet,"ALBERO");*/
				
				doc.GetCatenaDoc(out dataSet, idRoot);

				// TODO: sostituire con l'implementazione 
				//       della gestione documentale
			    result=buildCatena(idGruppo, idPeople, idRoot, dataSet.Tables["ALBERO"]);	
			}
			catch(Exception e)
			{
			   logger.Debug(e.Message);
				/*if(dbOpen)
				{
				 db.closeConnection();
				}*/
			   logger.Debug("Errore nella gestione della catena documento (getCatenaDoc)",e);
			   throw new Exception("F_System");			   
			}
			return result;
		}

		public static string getIdRoot(string docNumber/*,DocsPaWS.Utils.Database db*/)
		{
            logger.Debug("getIdRoot");
			string idRoot = "";
			try
			{ 
				#region Codice Commentato
			    /*string idRootString="SELECT ID_ROOT FROM DPA_DOC_COLLEGAMENTI WHERE ID_DOCUMENTO="+docNumber;
				if(db.executeScalar(idRootString)!=null && !db.executeScalar(idRootString).ToString().Equals(""))
				{
					idRoot=db.executeScalar(idRootString).ToString();
				}
				else{
				    idRoot=docNumber;
				}*/
				#endregion

				DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
				doc.GetIdRoot(out idRoot, docNumber);
			}
			catch(Exception e)
			{
				logger.Debug("Errore nella gestione dell'ID root documento (getIdRoot)",e);
				throw e;				
			}
			return idRoot;
		}

		public static DocsPaVO.documento.AnelloDocumentale buildCatena(string idGruppo, string idPeople, string idNodo,System.Data.DataTable alberoTable)
		{
		  DocsPaVO.documento.AnelloDocumentale nodo=new DocsPaVO.documento.AnelloDocumentale();
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			nodo.infoDoc = doc.GetInfoDocumento(idGruppo, idPeople, idNodo, true);
		  System.Data.DataRow[] childRows=alberoTable.Select("ID_DOC_COLLEGATO="+idNodo);
			for(int i=0;i<childRows.Length;i++)
			{
			   DocsPaVO.documento.AnelloDocumentale child=buildCatena(idGruppo, idPeople, childRows[i]["ID_DOCUMENTO"].ToString(),alberoTable);
			   nodo.children.Add(child);
			}
		  return nodo;
		}

		/// <summary>
		/// UpdatePerScollegamentoDoc
		/// </summary>
		/// <param name="systemId"></param>
		/// <param name="tipoProto"></param>
		public static bool updateProfilePerScollegamentoDoc(string systemId)
		{
			logger.Debug("scollegamento del documento: updateProfilePerScollegamentoDoc");

			DocsPaDB.Query_DocsPAWS.Documenti obj = new DocsPaDB.Query_DocsPAWS.Documenti(); 
			return obj.UpdatePerScollegamentoDoc(systemId);
		
		}

        /// <summary>
        /// Reperimento oggetto "InfoDocumento" relativamente
        /// al documento mittente al documento richiesto
        /// </summary>
        /// <param name="idGruppo"></param>
        /// <param name="idPeople"></param>
        /// <param name="docNumber"></param>
        /// <param name="tipoDocumento"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.InfoDocumento GetDocumentoMittente(string idGruppo, string idPeople, string docNumber, string tipoDocumento)
        {
            DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
            return documenti.GetDocumentoMittente(idGruppo, idPeople, docNumber, tipoDocumento);
        }
	}
}
