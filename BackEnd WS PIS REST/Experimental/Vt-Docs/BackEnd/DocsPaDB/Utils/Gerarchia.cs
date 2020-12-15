using System;
using System.Data;
using System.Collections;
using log4net;

namespace DocsPaDB.Utils
{

	public class Gerarchia 
	{
        private ILog logger = LogManager.GetLogger(typeof(Gerarchia));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruolo"></param>
        /// <param name="idRegistro"></param>
        /// <param name="idNodoTitolario"></param>
        /// <param name="tipoOggetto"></param>
        /// <returns> ritorna un arraylist e in caso di eccezione null</returns>
        //ricerca dei ruoli autorizzati di livello superiore
        public System.Collections.ArrayList getGerarchiaSup(DocsPaVO.utente.Ruolo ruolo, string idRegistro, string idNodoTitolario, DocsPaVO.trasmissione.TipoOggetto tipoOggetto)
        {
            logger.Debug("getGerarchiaSup");
            System.Collections.ArrayList listaRuoli = new System.Collections.ArrayList();
            try
            {
                DataSet dataSet;
                //Costruzione della query
                //si estraggono i codici delle UO parent del ruolo
                System.Collections.ArrayList idParentUO = new System.Collections.ArrayList();
                DocsPaVO.utente.UnitaOrganizzativa currentUO;
                currentUO = ruolo.uo;

                DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
                idParentUO = getParentUO(ruolo);
                
                DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();
                obj.selCorGlTipRuoSup(out dataSet, tipoOggetto, idRegistro, idNodoTitolario, ruolo, idParentUO);

                foreach (DataRow ruoloRow in dataSet.Tables["RUOLI"].Rows)
                {
                    listaRuoli.Add(this.GetRuolo(ruoloRow));
                }
            }
            catch (Exception e)
            {
                // TODO: Utilizzare il progetto DocsPaDbManagement
                //database.closeConnection();
                //throw e;
                string exMessage = e.Message;
                listaRuoli = null;
            }
            return listaRuoli;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ruolo"></param>
		/// <param name="idRegistro"></param>
		/// <param name="idNodoTitolario"></param>
		/// <param name="tipoOggetto"></param>
		/// <returns> ritorna un arraylist e in caso di eccezione null</returns>
		
		//ricerca dei ruoli autorizzati di livello superiore
		public System.Collections.ArrayList getGerarchiaSup(DocsPaVO.utente.Ruolo ruolo, string idRegistro, string idNodoTitolario, DocsPaVO.trasmissione.TipoOggetto tipoOggetto,DocsPaDB.DBProvider dbProvider)
		{
			logger.Debug("getGerarchiaSup");
			System.Collections.ArrayList listaRuoli= new System.Collections.ArrayList();
			try
			{
				DataSet dataSet;	
				
				//si estraggono i codici delle UO parent del ruolo
				System.Collections.ArrayList idParentUO=new System.Collections.ArrayList();
				DocsPaVO.utente.UnitaOrganizzativa currentUO;
				currentUO=ruolo.uo;
				
				DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();

				idParentUO = getParentUO(ruolo,dbProvider);
					
				DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();				
				obj.selCorGlTipRuoSup(out dataSet,tipoOggetto,idRegistro,idNodoTitolario,ruolo,idParentUO,dbProvider);

				foreach(DataRow ruoloRow in dataSet.Tables["RUOLI"].Rows)
				{
					DocsPaVO.utente.Ruolo ruoloSup=new DocsPaVO.utente.Ruolo();
					ruoloSup.systemId= ruoloRow["SYSTEM_ID"].ToString();
					ruoloSup.descrizione=ruoloRow["VAR_DESC_RUOLO"].ToString();
                    ruoloSup.codice = ruoloRow["VAR_CODICE"].ToString();
                    ruoloSup.codiceCorrispondente = ruoloSup.codice;
					ruoloSup.codiceRubrica=ruoloRow["VAR_COD_RUBRICA"].ToString();
					ruoloSup.uo=getParents(ruoloRow["ID_UO"].ToString(),ruolo);
					ruoloSup.idGruppo=ruoloRow["ID_GRUPPO"].ToString();
					listaRuoli.Add(ruoloSup);
				}
			
			}
			catch(Exception e)
			{
			
				string exMessage = e.Message;
				listaRuoli = null ;
			}
			return listaRuoli;
		}

		//ricerca dei ruoli autorizzati di pari livello
		public System.Collections.ArrayList getGerarchiaPariLiv(DocsPaVO.utente.Ruolo ruolo, string idRegistro, string idNodoTitolario, DocsPaVO.trasmissione.TipoOggetto tipoOggetto)
		{
			logger.Debug("getGerarchiaPariLiv");
			System.Collections.ArrayList listaRuoli= new System.Collections.ArrayList();
			try
			{
				DataSet dataSet= new DataSet();	
				//Costruzione della query
				//si estraggono i codici delle UO di pari livello del ruolo
				System.Collections.ArrayList idPariUO=new System.Collections.ArrayList();
				DocsPaVO.utente.UnitaOrganizzativa currentUO;
				currentUO=ruolo.uo;
				
				idPariUO = getPariUO(ruolo);
				//si aggiunge la systemId della nostra UO
				idPariUO.Add(ruolo.uo.systemId);
				logger.Debug(idPariUO.Count.ToString());

				DocsPaDB.Query_Utils.Utils obj2 = new DocsPaDB.Query_Utils.Utils();				
				obj2.selCorGlTipRuoPari(out dataSet,tipoOggetto,idRegistro,idNodoTitolario,ruolo,idPariUO);

				foreach(DataRow ruoloRow in dataSet.Tables["RUOLI"].Rows)
				{
                    listaRuoli.Add(this.GetRuolo(ruoloRow));
				}
			}
			catch(Exception e)
			{
				// TODO: Utilizzare il progetto DocsPaDbManagement
				//database.closeConnection();
				//throw e;
				string exMessage = e.Message;
				listaRuoli = null ;
			}
			return listaRuoli;
		}

		//ricerca dei ruoli autorizzati di livello inferiore
		public System.Collections.ArrayList getGerarchiaInf(DocsPaVO.utente.Ruolo ruolo, string idRegistro, string idNodoTitolario, DocsPaVO.trasmissione.TipoOggetto tipoOggetto)
		{
			System.Collections.ArrayList listaRuoli= new System.Collections.ArrayList();

			//DocsPaWS.Utils.Database database=DocsPaWS.Utils.dbControl.getDatabase();
			try
			{
				// TODO: Utilizzare il progetto DocsPaDbManagement
				//database.openConnection();

				DataSet dataSet= new DataSet();						
	
//				//si estraggono le uo di livello superiore, possibili figlie della uo del ruolo
//				DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();	
//				obj.getGerInf(dataSet,ruolo);
//				//si inseriscono in un array list i system_id delle UO gerarchicamente inferiori
//				System.Collections.ArrayList childrenUO= getChildrenUO(ruolo.uo.systemId,dataSet.Tables["UO"]);

				System.Collections.ArrayList childrenUO= getChildrenUO(ruolo);
				//si aggiunge la systemId della nostra UO
				childrenUO.Add(ruolo.uo.systemId);
				logger.Debug(childrenUO.Count.ToString());
				DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();	
				obj.selCorGlTipRuoInf(dataSet,tipoOggetto,idRegistro,idNodoTitolario,ruolo, childrenUO);

				foreach(DataRow ruoloRow in dataSet.Tables["RUOLI"].Rows)
				{
                    listaRuoli.Add(this.GetRuolo(ruoloRow));
				}
			}
			catch(Exception)
			{
				// TODO: Utilizzare il progetto DocsPaDbManagement
				//database.closeConnection();
				listaRuoli = null; 
			}
			return listaRuoli;
		}

		
		//ricerca di tutti i ruoli autorizzati
		public System.Collections.ArrayList getRuoliAut(DocsPaVO.utente.Ruolo ruolo, string idRegistro, string idNodoTitolario, DocsPaVO.trasmissione.TipoOggetto tipoOggetto)
		{
			System.Collections.ArrayList listaRuoli= new System.Collections.ArrayList();

			//DocsPaWS.Utils.Database database=DocsPaWS.Utils.dbControl.getDatabase();
			try
			{
				// TODO: Utilizzare il progetto DocsPaDbManagement
				//database.openConnection();

				DataSet dataSet;	
				DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();	
				obj.getRuoAut(out dataSet,ruolo,idRegistro,idNodoTitolario,tipoOggetto);

				foreach(DataRow ruoloRow in dataSet.Tables["RUOLI"].Rows)
				{
                    listaRuoli.Add(this.GetRuolo(ruoloRow));
				}
			}
			catch(Exception)
			{
				// TODO: Utilizzare il progetto DocsPaDbManagement
				//database.closeConnection();
				listaRuoli = null; 
			}
			return listaRuoli;
		}

		public static DocsPaVO.utente.UnitaOrganizzativa getParents(string id_parent, DocsPaVO.utente.Ruolo ruoloInit)
		{
			DocsPaVO.utente.UnitaOrganizzativa result=ruoloInit.uo;
			if(result==null) return null;
			
			while(result.systemId != null && result.systemId.Equals(id_parent))
			{
				result=result.parent;
				if(result==null) return null;
			}

			return result;
		} 

		
		//METODI PER LA RICERCA DELLE UO GERARCHICAMENTE VALIDE
		private static System.Collections.ArrayList getChildrenUO(string idUO, DataTable table)
		{
			DataRow[] childrenUO=table.Select("ID_PARENT="+idUO);
			System.Collections.ArrayList lista=new System.Collections.ArrayList();
			for(int i=0;i<childrenUO.Length;i++)
			{
				lista.Add(childrenUO[i]["SYSTEM_ID"].ToString());
				System.Collections.ArrayList lista2=getChildrenUO(childrenUO[i]["SYSTEM_ID"].ToString(),table);
				for(int j=0;j<lista2.Count;j++)
				{
					lista.Add(lista2[j]);
				}			
			}
			return lista;
		}
		public System.Collections.ArrayList getChildrenUO(DocsPaVO.utente.Ruolo ruolo)
		{
			System.Collections.ArrayList lista=new System.Collections.ArrayList();
			try 
			{
				//si estraggono le uo di livello superiore, possibili figlie della uo del ruolo
				DataSet dataSet = new DataSet();
				DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();	
				obj.getGerInf(dataSet,ruolo);
				DataTable table = dataSet.Tables["UO"];
				lista = getChildrenUO(ruolo.uo.systemId, table);
			}			
			catch(Exception e)
			{
				// TODO: Utilizzare il progetto DocsPaDbManagement
				//database.closeConnection();
				//throw e;
				string exMessage = e.Message;
				lista = null;
			}
			return lista;
		}
		public System.Collections.ArrayList getChildrenUO(DocsPaVO.utente.UnitaOrganizzativa uo)
		{
			System.Collections.ArrayList lista=new System.Collections.ArrayList();
			try 
			{
				//si estraggono le uo di livello superiore, possibili figlie della uo del ruolo
				DataSet dataSet = new DataSet();
				DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();	
				obj.getGerInf(dataSet,uo);
				DataTable table = dataSet.Tables["UO"];
				lista = getChildrenUO(uo.systemId, table);
				
			}			
			catch(Exception e)
			{
				// TODO: Utilizzare il progetto DocsPaDbManagement
				//database.closeConnection();
				//throw e;
				string exMessage = e.Message;
				lista = null;
			}
			return lista;
		}
		/// <summary>
		/// usato nel tool di amministrazione per ricercare le UO di livello inferiore a quella 
		/// fornita in input
		/// </summary>
		/// <param name="idUo">systemId della Uo</param>
		/// <param name="livelloUo">livello della Uo</param>
		/// <returns></returns>
		public System.Collections.ArrayList getChildrenUO(string idUo, string livelloUo)
		{
			System.Collections.ArrayList lista=new System.Collections.ArrayList();
			try 
			{
				//si estraggono le uo di livello superiore, possibili figlie della uo del ruolo
				DataSet dataSet = new DataSet();
				DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();	
				obj.getGerInf(dataSet,livelloUo);
				DataTable table = dataSet.Tables["UO"];
				lista = getChildrenUO(idUo, table);
				
			}			
			catch(Exception e)
			{
				// TODO: Utilizzare il progetto DocsPaDbManagement
				//database.closeConnection();
				//throw e;
				string exMessage = e.Message;
				lista = null;
			}
			return lista;
		}
		public System.Collections.ArrayList getPariUO(DocsPaVO.utente.Ruolo ruolo)
		{
			System.Collections.ArrayList lista=new System.Collections.ArrayList();
			try 
			{
				DataSet dataSet= new DataSet();	
				//Costruzione della query
				//si estraggono i codici delle UO di pari livello del ruolo
				DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();	
				obj.getGerPariLiv(dataSet,ruolo);

				DataTable table = dataSet.Tables["UO"];
				foreach(DataRow ruoloRow in table.Rows)
				{
					lista.Add(ruoloRow["SYSTEM_ID"].ToString());
				}
			}
			catch(Exception e)
			{
				// TODO: Utilizzare il progetto DocsPaDbManagement
				//database.closeConnection();
				//throw e;
				string exMessage = e.Message;
				lista = null;
			}
			return lista;
		}

		public System.Collections.ArrayList getParentUO(DocsPaVO.utente.Ruolo ruolo)
		{
			System.Collections.ArrayList lista=new System.Collections.ArrayList();
			DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
			string idCurrentUO = null;
			if(ruolo.uo!=null)
			{
				idCurrentUO = ruolo.uo.systemId ;
			}
			while(idCurrentUO != null)
			{
				lista.Add(idCurrentUO);
				idCurrentUO = amministrazioneXml.GetUOParent(idCurrentUO);
			}
			return lista;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ruolo"></param>
		/// <param name="dbProvider"></param>
		/// <returns></returns>
		public System.Collections.ArrayList getParentUO(DocsPaVO.utente.Ruolo ruolo,DBProvider dbProvider)
		{
			System.Collections.ArrayList lista=new System.Collections.ArrayList();
			DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
			string idCurrentUO = null;
			if(ruolo.uo!=null)
			{
				idCurrentUO = ruolo.uo.systemId ;
			}
			while(idCurrentUO != null)
			{
				lista.Add(idCurrentUO);
				idCurrentUO = amministrazioneXml.GetUOParent(idCurrentUO,dbProvider);
			}
			return lista;
		}

        /// <summary>
        /// Creazione di un oggetto Ruolo
        /// </summary>
        /// <param name="ruoloRow"></param>
        /// <returns></returns>
        protected virtual DocsPaVO.utente.Ruolo GetRuolo(DataRow ruoloRow)
        {
            DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
            ruolo.systemId = ruoloRow["SYSTEM_ID"].ToString();
            ruolo.descrizione = ruoloRow["VAR_DESC_RUOLO"].ToString();
            ruolo.codiceCorrispondente = ruoloRow["VAR_CODICE"].ToString();
            ruolo.codiceRubrica = ruoloRow["VAR_COD_RUBRICA"].ToString();
            ruolo.uo = getParents(ruoloRow["ID_UO"].ToString(), ruolo);
            ruolo.idGruppo = ruoloRow["ID_GRUPPO"].ToString();
            return ruolo;
        }
	}
}
