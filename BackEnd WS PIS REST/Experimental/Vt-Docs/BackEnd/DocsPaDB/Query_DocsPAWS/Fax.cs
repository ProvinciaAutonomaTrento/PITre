using System;
using System.Data;
using log4net;

namespace DocsPaDB.Query_DocsPAWS
{
	/// <summary>
	/// Classe contenente tutte le query (7) di DocsPAWS > fax
	/// </summary>
	public class Fax : DBProvider
	{
        private ILog logger = LogManager.GetLogger(typeof(Fax));
		#region DocsPaWS.fax.FaxInvio (4)

		/// <summary>
		/// Query #1 per il metodo "FaxInvioMethod"
		/// </summary>
		/// <param name="dataSet"></param>
		/// <param name="reg"></param>
		/// <param name="debug"></param>
		public void getDataReg(out DataSet dataSet,DocsPaVO.utente.Registro reg)
		{			
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_EL_REGISTRI__AMMINISTRA");
			q.setParam("param1",reg.systemId);
			string queryString = q.getSQL();
			this.ExecuteQuery(out dataSet,"REGISTRO",queryString);			
		}

		/// <summary>
		/// Query #2 per il metodo "FaxInvioMethod"
		/// </summary>
		/// <param name="infoUtente"></param>
		/// <param name="debug"></param>
		public string getFaxUsrLogin(string idCorrGlobali)
		{			
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlob5");
			q.setParam("param1",idCorrGlobali);
			string queryString = q.getSQL();
		    logger.Debug(queryString);
			string faxUserLogin;
			this.ExecuteScalar(out faxUserLogin,queryString);
			return faxUserLogin;
		}

		/// <summary>
		/// Query per il metodo "isFaxPreferred"
		/// </summary>
		/// <param name="dataSet"></param>
		/// <param name="corr"></param>
		public void getFax(out DataSet dataSet,DocsPaVO.utente.Corrispondente corr)
		{			
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_T_CANALE_CORR__DOCUMENTTYPES");
			q.setParam("param1",corr.systemId);
			string queryString = q.getSQL();			
			this.ExecuteQuery(out dataSet,"CANALE",queryString);	
		}

		/// <summary>
		/// Query per il metodo "getNumFaxCorr"
		/// </summary>
		/// <param name="systemId"></param>
		/// <param name="db"></param>		
		/// <returns></returns>
		public string getNumFax(string systemId/*,DocsPaWS.Utils.Database db */)
		{			
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPADettGlob3");
			q.setParam("param1",systemId);
			string queryString = q.getSQL();
			string res;
			this.ExecuteScalar(out res,queryString);
			return res;
		}

		#endregion

		#region DocsPaWS.fax.FaxManager (3)

		/// <summary>
		/// Query per il metodo "processaCaselleFaxUO"
		/// </summary>
		/// <param name="dataSet"></param>
		/// <param name="ruolo"></param>
		/// <param name="debug"></param>
		public void processFaxUO(out DataSet dataSet, DocsPaVO.utente.Ruolo ruolo)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlob6");
			q.setParam("param1",ruolo.uo.systemId);
			string queryString = q.getSQL();
			logger.Debug(queryString);		        
			this.ExecuteQuery(out dataSet,"UTENTI_FAX",queryString);
		}

		/// <summary>
		/// Query per il metodo "getUserLogin"
		/// </summary>
		/// <param name="db"></param>
		/// <param name="infoUtente"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		public string getFaxUserLogin(string idPeople)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlob7");
			q.setParam("param1",idPeople);
			string queryString = q.getSQL();
			logger.Debug(queryString);
			string userLogin;
			this.ExecuteScalar(out userLogin,queryString);
			return userLogin;
		}

		/// <summary>
		/// Query per il metodo "getRagioneTrasm"
		/// </summary>
		/// <param name="ds"></param>
		public void getRagTrasm(out DataSet ds)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPARagTrasm");			
			string queryString = q.getSQL();
			this.ExecuteQuery(out ds,"RAGIONE",queryString);			
		}

		#endregion
	}
}
