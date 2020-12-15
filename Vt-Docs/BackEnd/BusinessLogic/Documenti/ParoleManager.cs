using System;
using log4net;

namespace BusinessLogic.Documenti
{
	/// <summary>
	/// Summary description for ParoleManager.
	/// </summary>
	public class ParoleManager 
	{
        private static ILog logger = LogManager.GetLogger(typeof(ParoleManager));
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sicurezza"></param>
		/// <returns></returns>
		public static System.Collections.ArrayList getParoleChiaveMethod(string idAmministrazione) 
		{
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			System.Collections.ArrayList listaParole=new System.Collections.ArrayList();
			if (!doc.GetParole(idAmministrazione, ref listaParole))
			{
				//TODO: gestire la throw
				throw new Exception();
			}
			return listaParole;
				
			#region Codice Commentato
			/*DocsPaWS.Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();
			DataSet dataSet;
			System.Collections.ArrayList lista=new System.Collections.ArrayList();
			try 
			{
				string idAmm=sicurezza.idAmministrazione;
				string queryString="SELECT SYSTEM_ID, VAR_DESC_PAROLA FROM DPA_PAROLE WHERE ID_AMM="+idAmm + " ORDER BY VAR_DESC_PAROLA";
				db.fillTable(queryString,dataSet,"PAROLE");
				foreach(DataRow parolaRow in dataSet.Tables["PAROLE"].Rows)
				{
					DocsPaVO.documento.ParolaChiave parola=new DocsPaVO.documento.ParolaChiave();
					parola.systemId=parolaRow["SYSTEM_ID"].ToString();
					parola.descrizione=parolaRow["VAR_DESC_PAROLA"].ToString();
					parola.idAmministrazione=idAmm;
					lista.Add(parola);
				}
				listaParole=lista;
				db.closeConnection();
			}
			catch(Exception e) 
			{
				db.closeConnection();
				throw e;
			}
			return listaParole;*/
			#endregion
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parolaC"></param>
		/// <param name="infoUtente"></param>
		/// <returns></returns>
		public static DocsPaVO.documento.ParolaChiave inserisciParolaChiave(string idAmministrazione, DocsPaVO.documento.ParolaChiave parolaChiave) 
		{
			logger.Debug("inserimentoParolaChiave");
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			doc.InsertParolaChiave(idAmministrazione, ref parolaChiave);
			return parolaChiave;

			#region Codice Commentato
			/*DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
			db.openConnection();	
			string id_amministrazione;
			id_amministrazione = infoUtente.idAmministrazione;

			string numPar = checkParolaChiave(parolaC, id_amministrazione, db);
			if (!numPar.Equals("0"))
			{
				throw new Exception("Parola chiave già presente");
			}
			try 
			{
				string insertString = 
					"INSERT INTO DPA_PAROLE " +
					"(" + DocsPaWS.Utils.dbControl.getSystemIdColName() + " ID_AMM, VAR_DESC_PAROLA ) " +
					" VALUES (" + DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_PAROLE") +
					id_amministrazione + ", '" + parolaC.descrizione.Replace("'", "''") + "')";
				DocsPaDB.Query_DocsPAWS.Documenti obj = new DocsPaDB.Query_DocsPAWS.Documenti();
				string insertString = obj.insertParolaChiave(id_amministrazione, parolaC.descrizione);

				logger.Debug(insertString);
				parolaC.systemId = db.insertLocked(insertString, "DPA_PAROLE");
				db.closeConnection();
     		} 
			catch (Exception e)
			{
				logger.Debug (e.Message);				
				db.closeConnection();
				throw new Exception("F_System");
			}
			return parolaC;*/
			#endregion
		}

		#region Metodo Commentato
		/*protected static string checkParolaChiave(DocsPaVO.documento.ParolaChiave parolaC, string id_amministrazione, DocsPaWS.Utils.Database db) 
		{
			//si verifica se la parola chiave è già presente
			string selectString =
				"SELECT COUNT(*) FROM DPA_PAROLE WHERE upper(VAR_DESC_PAROLA)='"+ parolaC.descrizione.ToUpper() +"'";
			
			if (id_amministrazione != null && !id_amministrazione.Equals(""))
				selectString += " AND ID_AMM =" + id_amministrazione;
			
			logger.Debug(selectString);
			string numPar= db.executeScalar(selectString).ToString();
		}*/
		#endregion
	}
}
