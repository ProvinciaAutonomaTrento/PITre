using System;
using System.Data;
using System.Collections;
using log4net;

namespace BusinessLogic.Amministrazione
{
	/// <summary>
	/// </summary>
	public class UOManager
	{
        private static ILog logger = LogManager.GetLogger(typeof(UOManager));
		/// <summary></summary>
		/// <param name="uOrg"></param>
		/// <param name="infoUtente"></param>
		/// <returns></returns>
		public static ArrayList GetRegistriUO(DocsPaVO.utente.UnitaOrganizzativa uo) 
		{
			DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
			ArrayList result = amm.getRegistriUO(uo);

			return result;
		}

		/// <summary></summary>
		/// <param name="corr"></param>
		/// <param name="infoUtente"></param>
		/// <returns></returns>
		public static DocsPaVO.utente.UnitaOrganizzativa GetInfoUO(DocsPaVO.utente.Corrispondente corr) 
		{
			DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
			DocsPaVO.utente.UnitaOrganizzativa uo = amm.GetInfoUO((DocsPaVO.utente.UnitaOrganizzativa)corr);
			
			if (uo == null)
			{
				throw new Exception();
			}

			return uo;
		}

		/// <summary></summary>
		/// <param name="uo"></param>
		/// <param name="infoUtente"></param>
		/// <returns></returns>
		public static ArrayList GetRuoliUO(DocsPaVO.utente.UnitaOrganizzativa uo)
		{
			DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
			ArrayList result = amm.getRuoliUO(uo);

			return result;
		}

		/// <summary>
		/// Restituisce una Hashtable per una data amministrazione
		/// in cui la chiave è il codice della UO e il valore è un array
		/// di codici rubrica dei ruoli che afferiscono a questa UO
		/// </summary>
		/// <param name="id_amm"></param>
		/// <returns></returns>
		public static Hashtable GetRuoliUOSemplice (string id_amm)
		{
			DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
			return amm.GetRuoliUOSemplice (id_amm);
		}

		/// <summary>
		/// Restituisce una Hashtable per una data amministrazione
		/// in cui la chiave è il codice del ruolo e il valore è il
		/// codice rubrica dell'UO per cui questo ruolo è definito
		/// </summary>
		/// <param name="id_amm"></param>
		/// <returns></returns>
		public static Hashtable GetUORuoloSemplice (string id_amm)
		{
			DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
			return amm.GetUORuoloSemplice (id_amm);
		}

		/// <summary>
		/// Imposta livelli delle UO nella funzionalità "spostamento UO"
		/// </summary>
		/// <param name="uoDaSpostare"></param>
		/// <param name="uoPadre"></param>
		/// <returns></returns>
		public static DocsPaVO.amministrazione.EsitoOperazione ImpostaLivelloUO(DocsPaVO.amministrazione.OrgUO uoDaSpostare, DocsPaVO.amministrazione.OrgUO uoPadre)
		{
			string segno = string.Empty;
			int delta = 0;
			int rowsAffected;
			string commandText=string.Empty;
			DocsPaUtils.Query queryDef=null;
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

			int livelloUoDaSpostare = Convert.ToInt16( uoDaSpostare.Livello );
			int livelloDaAcquisire = Convert.ToInt16( uoPadre.Livello ) + 1;

			if(!livelloUoDaSpostare.Equals(livelloDaAcquisire))
			{				
				// spostamento verso l'alto
				if(livelloUoDaSpostare < livelloDaAcquisire)
				{
					segno = "+";
					delta = livelloDaAcquisire - livelloUoDaSpostare;
				}
				// spostamento verso il basso
				if(livelloUoDaSpostare > livelloDaAcquisire)
				{
					segno = "-";
					delta = livelloUoDaSpostare - livelloDaAcquisire;
				}				

				// aggiorna livello UO da spostare
				queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("U_DPACorrGlobali");			
				queryDef.setParam("param1","NUM_LIVELLO = "+Convert.ToString((Convert.ToInt16(uoPadre.Livello)+1)));
				queryDef.setParam("param2","SYSTEM_ID = "+uoDaSpostare.IDCorrGlobale);					

				commandText=queryDef.getSQL();
				logger.Debug(commandText);

				dbProvider.ExecuteNonQuery(commandText,out rowsAffected);
						
				if(rowsAffected==0)
				{
					esito.Codice = 2;	
					esito.Descrizione = "fallito aggiornamento della tabella dei corrispondenti";					
				}
				else
				{
					//re-imposta tutti i livelli delle UO figlie alla UO da spostare
					if(delta > 0)
						esito = ImpostaLivelloSottoUO(uoDaSpostare, segno, delta);							
				}
			}					

			return esito;
		}

		public static DocsPaVO.amministrazione.EsitoOperazione ImpostaLivelloSottoUO(DocsPaVO.amministrazione.OrgUO currentUO, string segno, int delta)
		{
			int rowsAffected;
			string commandText=string.Empty;
			string condizioneLivello = string.Empty;

			DocsPaUtils.Query queryDef=null;
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
			DocsPaDB.DBProvider dbProvider2=new DocsPaDB.DBProvider();
			System.Data.IDataReader reader=null;

			DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
			DocsPaVO.amministrazione.OrgUO sottoUO = null;
			
			// prende tutte le sotto UO
			queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("S_CORR_GLOB_GENERIC");			
			queryDef.setParam("param1","SYSTEM_ID, NUM_LIVELLO");
			queryDef.setParam("param2","ID_PARENT = "+currentUO.IDCorrGlobale);					

			commandText=queryDef.getSQL();
			logger.Debug(commandText);

			reader = dbProvider.ExecuteReader(commandText);

			while (reader.Read())
			{
				sottoUO = new DocsPaVO.amministrazione.OrgUO();
				sottoUO.IDCorrGlobale = reader.GetValue(reader.GetOrdinal("SYSTEM_ID")).ToString();
				sottoUO.Livello = reader.GetValue(reader.GetOrdinal("NUM_LIVELLO")).ToString();

				condizioneLivello = segno + Convert.ToString(delta);

				// aggiorna livello sotto UO
				queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("U_DPACorrGlobali");			
				queryDef.setParam("param1","NUM_LIVELLO = NUM_LIVELLO " + condizioneLivello);
				queryDef.setParam("param2","SYSTEM_ID = " + sottoUO.IDCorrGlobale);					

				commandText=queryDef.getSQL();
				logger.Debug(commandText);

				dbProvider2.ExecuteNonQuery(commandText,out rowsAffected);
						
				if(rowsAffected==0)
				{
					esito.Codice = 3;	
					esito.Descrizione = "fallito aggiornamento del livello della UO con ID: "+sottoUO.IDCorrGlobale;					
					break;
				}
				else
				{	
					esito = ImpostaLivelloSottoUO(sottoUO,segno,delta);
				}
				
			}
		
			return esito;
		}

        /// <summary></summary>
        /// <param name="uo"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static String[] getListaRuoliSoppopostiUO(DocsPaVO.utente.InfoUtente infoUtente, string system_id_uo)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            String[] result = null;
            result = amm.getListRuoliUOSottoposti(infoUtente, system_id_uo);
            return result;
        }


        public static bool IsVisibleUObyRuolo(DocsPaVO.utente.InfoUtente infoUtente, string system_id_uo, string idMiaUo)
        {
            bool result = false;
            ArrayList uoSottoposte = new ArrayList();
           
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

            uoSottoposte = amm.getListaUOSottoposte(infoUtente, system_id_uo, idMiaUo);

            if (uoSottoposte.Contains(system_id_uo))
            {
                result = true;
            }

            return result;
        }
	}
}
