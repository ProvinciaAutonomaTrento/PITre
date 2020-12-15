// TODO: Controllare tutti i metodi di DB

using System;
using System.Data;
using System.Globalization;
using log4net;

namespace DocsPaDB.Query_Utils
{
	/// <summary>
	/// Classe contenente tutte le query (13) di Utils
	/// </summary>
	public class Utils : DBProvider
	{
        private static ILog logger = LogManager.GetLogger(typeof(Utils));

		#region le query in Utils.Gerarchia (5)
		
		/// <summary>
		/// Query per il metodo "getGerarchiaSup"
		/// </summary>
		/// <param name="tipoOggetto"></param>
		/// <param name="idRegistro"></param>
		/// <param name="idNodoTitolario"></param>
		/// <param name="ruolo"></param>
		/// <param name="idParentUO"></param>
		public void selCorGlTipRuoSup(out DataSet dataSet,DocsPaVO.trasmissione.TipoOggetto tipoOggetto,string idRegistro,string idNodoTitolario,DocsPaVO.utente.Ruolo ruolo,System.Collections.ArrayList idParentUO)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__TIPO_RUOLO2");
			string queryString="";
			
			queryString = getQueryAut(tipoOggetto,idRegistro,idNodoTitolario,ruolo,idParentUO,DocsPaVO.trasmissione.TipoGerarchia.SUPERIORE.ToString());
			
			#region commenti
//			if(tipoOggetto==DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
//			{
//				if(idRegistro!=null && idRegistro!="")
//					queryString=queryString+",DPA_L_RUOLO_REG C";					
//			}
//			else
//			{
//				queryString=queryString+",SECURITY C";				
//				if(idRegistro!=null)
//					queryString=queryString+",DPA_L_RUOLO_REG D";					
//			}
//			queryString=queryString+" WHERE";
//			if(ruolo.idAmministrazione!=null && !ruolo.idAmministrazione.ToString().Equals(""))
//				queryString=queryString+" A.ID_AMM="+ruolo.idAmministrazione+" AND";
//				
//			if(idParentUO.Count!=0)
//			{
//				queryString=queryString+" A.ID_UO IN (";
//				for(int i=0;i<idParentUO.Count;i++)
//				{
//					queryString=queryString+idParentUO[i].ToString();
//					if(i<idParentUO.Count-1)
//						queryString=queryString+",";
//				}
//				queryString=queryString+") AND";
//			}
//		  
//			queryString=queryString+" B.NUM_LIVELLO<"+ruolo.livello+" AND B.SYSTEM_ID=A.ID_TIPO_RUOLO";
//			if(tipoOggetto==DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
//			{
//				if(idRegistro!=null && idRegistro != "")
//				{
//					queryString=queryString+" AND C.ID_RUOLO_IN_UO=A.SYSTEM_ID";
//					queryString=queryString+" AND C.ID_REGISTRO="+idRegistro+"";
//				}
//			}
//			else
//			{
//				queryString=queryString+" AND C.PERSONORGROUP=A.ID_GRUPPO AND C.THING="+idNodoTitolario+" AND C.ACCESSRIGHTS > 0";
//				if(idRegistro!=null && idRegistro != "")
//				{
//					queryString=queryString+" AND D.ID_RUOLO_IN_UO=A.SYSTEM_ID"; 
//					queryString=queryString+" AND D.ID_REGISTRO="+idRegistro+"";
//				}
//			}
			#endregion commenti

            q.setParam("param1", queryString);
            q.setParam("hint", "");

			string myString = q.getSQL();
            logger.Debug(myString);
			this.ExecuteQuery (out dataSet,"RUOLI",myString);
		}


		/// <summary>
		/// Query per il metodo "getGerarchiaSup"
		/// </summary>
		/// <param name="tipoOggetto"></param>
		/// <param name="idRegistro"></param>
		/// <param name="idNodoTitolario"></param>
		/// <param name="ruolo"></param>
		/// <param name="idParentUO"></param>
		public void selCorGlTipRuoSup(out DataSet dataSet,DocsPaVO.trasmissione.TipoOggetto tipoOggetto,string idRegistro,string idNodoTitolario,DocsPaVO.utente.Ruolo ruolo,System.Collections.ArrayList idParentUO, DocsPaDB.DBProvider dbProvider)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__TIPO_RUOLO2");
            string queryString = "";
            /// string hint = "/*+ index (A) index (B) */";
            string hint = "/*+ ";

			queryString = getQueryAut(tipoOggetto,idRegistro,idNodoTitolario,ruolo,idParentUO,DocsPaVO.trasmissione.TipoGerarchia.SUPERIORE.ToString());

            q.setParam("param1", queryString);
            if (queryString.ToLower().IndexOf("security c") > 0)
                /// hint = "/*+index(C) index (A) index (B) */";
                hint += "index(C) ";

            if (queryString.ToLower().IndexOf("dpa_l_ruolo_reg d") > 0)
                hint += "index(D) ";

            hint += "index(A) index(B)";
            hint += "*/";

            q.setParam("hint", hint);

			string myString = q.getSQL();
            logger.Debug(myString);
			dbProvider.ExecuteQuery (out dataSet,"RUOLI",myString);
		}

		/// <summary>
		/// Query #0 per il metodo "getGerarchiaInf"
		/// </summary>
		/// <param name="dataSet"></param>
		/// <param name="ruolo"></param>
		public void getGerInf(DataSet dataSet,DocsPaVO.utente.UnitaOrganizzativa uo)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlob10");
			q.setParam("param1", uo.livello);
			string myString = q.getSQL();
			this.ExecuteQuery (dataSet,"UO",myString);
		}

		/// <summary>
		/// Query #0 per il metodo "getGerarchiaInf"
		/// </summary>
		/// <param name="dataSet"></param>
		/// <param name="ruolo"></param>
		public void getGerInf(DataSet dataSet,string livelloUo)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlob10");
			q.setParam("param1", livelloUo);
			string myString = q.getSQL();
			this.ExecuteQuery (dataSet,"UO",myString);
		}

		/// <summary>
		/// Query #1 per il metodo "getGerarchiaInf"
		/// </summary>
		/// <param name="dataSet"></param>
		/// <param name="ruolo"></param>
		public void getGerInf(DataSet dataSet,DocsPaVO.utente.Ruolo ruolo)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlob10");
			q.setParam("param1", ruolo.uo.livello);
			string myString = q.getSQL();
			this.ExecuteQuery (dataSet,"UO",myString);
		}

		/// <summary>
		/// Query #2 per il metodo "getGerarchiaInf"
		/// </summary>
		/// <param name="dataSet"></param>
		/// <param name="tipoOggetto"></param>
		/// <param name="idRegistro"></param>
		/// <param name="idNodoTitolario"></param>
		/// <param name="ruolo"></param>
		public void selCorGlTipRuoInf(DataSet dataSet,DocsPaVO.trasmissione.TipoOggetto tipoOggetto,string idRegistro,string idNodoTitolario,DocsPaVO.utente.Ruolo ruolo, System.Collections.ArrayList childrenUO)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__TIPO_RUOLO3");
			string queryString="";

			queryString = getQueryAut(tipoOggetto,idRegistro,idNodoTitolario,ruolo,childrenUO,DocsPaVO.trasmissione.TipoGerarchia.INFERIORE.ToString());
			#region commenti
//			if(tipoOggetto==DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
//			{
//				if(idRegistro!=null)
//				{
//					queryString=queryString+", DPA_L_RUOLO_REG C";
//				}
//				
//			}
//			else
//			{
//				queryString=queryString+", SECURITY C";
//				if(idRegistro!=null)
//				{
//					queryString=queryString+", DPA_L_RUOLO_REG D";
//				}
//			}
//			queryString=queryString+" WHERE";
//			if(ruolo.idAmministrazione!=null && !ruolo.idAmministrazione.ToString().Equals(""))
//			{
//				queryString=queryString+" A.ID_AMM="+ruolo.idAmministrazione+" AND";
//			}
//			if(childrenUO.Count!=0)
//			{
//				queryString=queryString+" A.ID_UO IN (";
//				for(int i=0;i<childrenUO.Count;i++)
//				{
//					queryString=queryString+childrenUO[i].ToString();
//					if(i<childrenUO.Count-1)
//					{
//						queryString=queryString+",";
//					}
//				}
//				queryString=queryString+") AND";
//			}
//			queryString=queryString+" B.NUM_LIVELLO>"+ruolo.livello+" AND B.SYSTEM_ID=A.ID_TIPO_RUOLO ";
//			if(tipoOggetto==DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
//			{
//				if(idRegistro!=null)
//				{
//					queryString=queryString+" AND C.ID_RUOLO_IN_UO=A.SYSTEM_ID";
//					queryString=queryString+" AND C.ID_REGISTRO='"+idRegistro+"'";
//				}
//			}
//			else
//			{
//				queryString=queryString+"AND C.PERSONORGROUP=A.ID_GRUPPO AND C.THING='"+idNodoTitolario+"'";
//				if(idRegistro!=null)
//				{
//					queryString=queryString+" AND D.ID_RUOLO_IN_UO=A.SYSTEM_ID"; 
//					queryString=queryString+" AND D.ID_REGISTRO='"+idRegistro+"'";
//				}
//			}
			#endregion commenti

			q.setParam("param1",queryString);
			string myString = q.getSQL();
			
			//logger.Debug(myString);			
			this.ExecuteQuery (dataSet,"RUOLI",myString);
		}

		/// <summary>
		/// Query per il metodo "getRuoliAut"
		/// </summary>
		/// <param name="ruolo"></param>
		/// <param name="idRegistro"></param>
		/// <param name="idNodoTitolario"></param>
		/// <param name="tipoOggetto"></param>
		public void getRuoAut(out DataSet dataSet, DocsPaVO.utente.Ruolo ruolo, string idRegistro, string idNodoTitolario, DocsPaVO.trasmissione.TipoOggetto tipoOggetto)
		{
			string queryString="";
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__TIPO_RUOLO4");

			if(tipoOggetto==DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
			{
				if(idRegistro!=null && idRegistro!="" && !isFiltroAooEnabled())
				{
					queryString=queryString+", DPA_L_RUOLO_REG C";
				}
				
			}
			else
			{
                //Emanuela 07/04/2014: nel caso di trasmissione massiva di fascicoli, idNodoTitolario è vuoto e la query non fornisce alcun
                //risulato.
                if (!string.IsNullOrEmpty(idNodoTitolario))
                {
                    queryString = queryString + ", SECURITY C";
                }
                if (idRegistro != null && idRegistro != "" && !isFiltroAooEnabled())
				{
					queryString=queryString+", DPA_L_RUOLO_REG D";
				}
			}
			queryString=queryString+" WHERE";
			if(ruolo != null && ruolo.idAmministrazione!=null && !ruolo.idAmministrazione.ToString().Equals(""))
			{
				queryString=queryString+" A.ID_AMM="+ruolo.idAmministrazione+" AND";
			}
			queryString=queryString+" B.SYSTEM_ID=A.ID_TIPO_RUOLO ";
			if(tipoOggetto==DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
			{
                if (idRegistro != null && idRegistro != "" && !isFiltroAooEnabled())
				{
					queryString=queryString+" AND C.ID_RUOLO_IN_UO=A.SYSTEM_ID";
					queryString=queryString+" AND C.ID_REGISTRO='"+idRegistro+"'";
				}
			}
			else
			{
                //Emanuela 07/04/2014: nel caso di trasmissione massiva di fascicoli, idNodoTitolario è vuoto e la query non fornisce alcun
                //risulato.
                if (!string.IsNullOrEmpty(idNodoTitolario))
                {
                    queryString = queryString + "AND C.PERSONORGROUP=A.ID_GRUPPO AND C.THING='" + idNodoTitolario + "'";
                }
                if (idRegistro != null && idRegistro != "" && !isFiltroAooEnabled())
				{
					queryString=queryString+" AND D.ID_RUOLO_IN_UO=A.SYSTEM_ID"; 
					queryString=queryString+" AND D.ID_REGISTRO='"+idRegistro+"'";
				}
			}

			q.setParam("param1",queryString);
			string myString = q.getSQL();
            logger.Debug("getRuoAut - S_J_DPA_CORR_GLOBALI__TIPO_RUOLO4 - " + myString);	
			this.ExecuteQuery (out dataSet,"RUOLI",myString);
		}


		/// <summary>
		/// Query per il metodo "getGerarchiaPariLiv"
		/// </summary>
		/// <param name="dataSet"></param>
		/// <param name="ruolo"></param>
		public void getGerPariLiv(DataSet dataSet,DocsPaVO.utente.Ruolo ruolo)
		{
			string idParent = "0";
			if (ruolo.uo.parent!=null && ruolo.uo.parent.systemId !=null && !ruolo.uo.parent.systemId.Equals(""))
				idParent=ruolo.uo.parent.systemId;
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlob12");
			q.setParam("param1", ruolo.uo.livello);
			q.setParam("param2", idParent);
			string myString = q.getSQL();
			this.ExecuteQuery (dataSet,"UO",myString);
		}

		/// <summary>
		/// Query per il metodo "getGerarchiaPariLiv"
		/// </summary>
		/// <param name="tipoOggetto"></param>
		/// <param name="idRegistro"></param>
		/// <param name="idNodoTitolario"></param>
		/// <param name="ruolo"></param>
		/// <param name="idPariUO"></param>
		public void selCorGlTipRuoPari(out DataSet dataSet,DocsPaVO.trasmissione.TipoOggetto tipoOggetto,string idRegistro,string idNodoTitolario,DocsPaVO.utente.Ruolo ruolo,System.Collections.ArrayList idPariUO)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__TIPO_RUOLO2");
			string queryString="";
			queryString = getQueryAut(tipoOggetto,idRegistro,idNodoTitolario,ruolo,idPariUO,DocsPaVO.trasmissione.TipoGerarchia.PARILIVELLO.ToString());

			#region commenti
//			if(tipoOggetto==DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
//			{
//				if(idRegistro!=null && idRegistro!="")
//					queryString=queryString+",DPA_L_RUOLO_REG C";					
//			}
//			else
//			{
//				queryString=queryString+",SECURITY C";				
//				if(idRegistro!=null)
//					queryString=queryString+",DPA_L_RUOLO_REG D";					
//			}
//			queryString=queryString+" WHERE";
//			if(ruolo.idAmministrazione!=null && !ruolo.idAmministrazione.ToString().Equals(""))
//				queryString=queryString+" A.ID_AMM="+ruolo.idAmministrazione+" AND";
//				
//			if(idPariUO.Count!=0)
//			{
//				queryString=queryString+" A.ID_UO IN (";
//				for(int i=0;i<idParentUO.Count;i++)
//				{
//					queryString=queryString+idParentUO[i].ToString();
//					if(i<idParentUO.Count-1)
//						queryString=queryString+",";
//				}
//				queryString=queryString+") AND";
//			}
//		  
//			queryString=queryString+" B.NUM_LIVELLO="+ruolo.livello+" AND B.SYSTEM_ID=A.ID_TIPO_RUOLO";
//			if(tipoOggetto==DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
//			{
//				if(idRegistro!=null && idRegistro != "")
//				{
//					queryString=queryString+" AND C.ID_RUOLO_IN_UO=A.SYSTEM_ID";
//					queryString=queryString+" AND C.ID_REGISTRO="+idRegistro+"";
//				}
//			}
//			else
//			{
//				queryString=queryString+" AND C.PERSONORGROUP=A.ID_GRUPPO AND C.THING="+idNodoTitolario+" AND C.ACCESSRIGHTS > 0";
//				if(idRegistro!=null && idRegistro != "")
//				{
//					queryString=queryString+" AND D.ID_RUOLO_IN_UO=A.SYSTEM_ID"; 
//					queryString=queryString+" AND D.ID_REGISTRO="+idRegistro+"";
//				}
//			}
			#endregion commenti

            q.setParam("param1", queryString);
            q.setParam("hint", "");
			string myString = q.getSQL();
			this.ExecuteQuery (out dataSet,"RUOLI",myString);
		}

		#region getQueryAut OLD
		//		private string getQueryAut(DocsPaVO.trasmissione.TipoOggetto tipoOggetto,string idRegistro,string idNodoTitolario,DocsPaVO.utente.Ruolo ruolo,System.Collections.ArrayList listaIdUO, string tipoGerarchia)
		//		{
		//			string queryString="";
		//			
		//			if(tipoOggetto==DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
		//			{
		//				if(idRegistro!=null && idRegistro!="")
		//					queryString=queryString+",DPA_L_RUOLO_REG C";					
		//			}
		//			else
		//			{
		//				queryString=queryString+",SECURITY C";				
		//				if(idRegistro!=null)
		//					queryString=queryString+",DPA_L_RUOLO_REG D";					
		//			}
		//			queryString=queryString+" WHERE";
		//			if(ruolo.idAmministrazione!=null && !ruolo.idAmministrazione.ToString().Equals(""))
		//				queryString=queryString+" A.ID_AMM="+ruolo.idAmministrazione+" AND";
		//				
		//			if(listaIdUO != null && listaIdUO.Count>0)
		//			{
		//				queryString=queryString+" A.ID_UO IN (";
		//				for(int i=0;i<listaIdUO.Count;i++)
		//				{
		//					queryString=queryString+listaIdUO[i].ToString();
		//					if(i<listaIdUO.Count-1)
		//						queryString=queryString+",";
		//				}
		//				queryString=queryString+") AND";
		//			}
		//			string segn = "";
		//			if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.SUPERIORE.ToString()))
		//				segn="<";
		//			else if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.INFERIORE.ToString()))
		//				segn=">";
		//			else if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.PARILIVELLO.ToString()))
		//				segn="=";
		//			if (!segn.Equals(""))
		//				queryString=queryString+" B.NUM_LIVELLO "+segn+ruolo.livello + " AND";
		//			queryString=queryString+" B.SYSTEM_ID=A.ID_TIPO_RUOLO";
		//			if(tipoOggetto==DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
		//			{
		//				if(idRegistro!=null && idRegistro != "")
		//				{
		//					queryString=queryString+" AND C.ID_RUOLO_IN_UO=A.SYSTEM_ID";
		//					queryString=queryString+" AND C.ID_REGISTRO="+idRegistro+"";
		//				}
		//			}
		//			else
		//			{
		//				queryString=queryString+" AND C.PERSONORGROUP=A.ID_GRUPPO AND C.THING="+idNodoTitolario+" AND C.ACCESSRIGHTS > 0";
		//				if(idRegistro!=null && idRegistro != "")
		//				{
		//					queryString=queryString+" AND D.ID_RUOLO_IN_UO=A.SYSTEM_ID"; 
		//					queryString=queryString+" AND D.ID_REGISTRO="+idRegistro+"";
		//				}
		//			}
		//			return queryString;
		//		}
		#endregion

		private string getQueryAut(DocsPaVO.trasmissione.TipoOggetto tipoOggetto,string idRegistro,string idNodoTitolario,DocsPaVO.utente.Ruolo ruolo,System.Collections.ArrayList listaIdUO, string tipoGerarchia)
		{
			string queryString="";
			
			if(tipoOggetto==DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
			{

				if(idRegistro!=null && idRegistro!="" && !isFiltroAooEnabled())
					queryString=queryString+",DPA_L_RUOLO_REG C";					
			}
			else
			{
				queryString=queryString+",SECURITY C";				
				if(idRegistro!=null)
					queryString=queryString+",DPA_L_RUOLO_REG D";					
			}
			queryString=queryString+" WHERE";
			if(ruolo.idAmministrazione!=null && !ruolo.idAmministrazione.ToString().Equals(""))
				queryString=queryString+" A.ID_AMM="+ruolo.idAmministrazione+" AND";
				
			//se la chiave sul web.config è assente oppure è impostata a "0"
			if (System.Configuration.ConfigurationManager.AppSettings["EST_VIS_SUP_PARI_LIV"]== null || System.Configuration.ConfigurationManager.AppSettings["EST_VIS_SUP_PARI_LIV"].Equals("0"))
			{
				if(listaIdUO != null && listaIdUO.Count>0)
				{
					queryString=queryString+" A.ID_UO IN (";
					for(int i=0;i<listaIdUO.Count;i++)
					{
						queryString=queryString+listaIdUO[i].ToString();
						if(i<listaIdUO.Count-1)
							queryString=queryString+",";
					}
					queryString=queryString+") AND";
				}
				string segn = "";
				if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.SUPERIORE.ToString()))
					segn="<";
				else if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.INFERIORE.ToString()))
					segn=">";
				else if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.PARILIVELLO.ToString()))
					segn="=";
				if (!segn.Equals(""))
					queryString=queryString+" B.NUM_LIVELLO "+segn+ruolo.livello + " AND";
				queryString=queryString+" B.SYSTEM_ID=A.ID_TIPO_RUOLO";
			}
			else
			{
				/* caso nuovo per ANAS -- si estende la visibilità a tutti i ruoli dello stesso livello 
				 * apparteneti a UO gerarchicamente superiori */
				string segn = "";
				if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.SUPERIORE.ToString()))
				{
					if(listaIdUO != null && listaIdUO.Count>1)
					{
						queryString=queryString+" ((A.ID_UO IN (";
						for(int i=0;i<listaIdUO.Count;i++)
						{
							if (ruolo.uo.systemId != listaIdUO[i].ToString())
							{
								queryString=queryString+listaIdUO[i].ToString();
								if(i<listaIdUO.Count-1)
									queryString=queryString+",";
							}
						}
						if(queryString.Substring(queryString.Length-1,1).Equals(","))
						{
							queryString=queryString.Substring(0,queryString.Length-1);
						}
						queryString=queryString+") AND B.NUM_LIVELLO <= " + ruolo.livello + ") OR (A.ID_UO = " + ruolo.uo.systemId + " AND ";
					}
					else
					{
						//caso in cui la UO non ha UO gerarchicamente superiori
						queryString=queryString+" A.ID_UO = " + ruolo.uo.systemId + " AND ";
					}
				}
				else if(tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.INFERIORE.ToString()))
				{
					//risoluzione bug 1586 - per ora riportato per i sottoposti.
					if(listaIdUO != null && listaIdUO.Count>1) 
					{
						queryString=queryString+" ((A.ID_UO IN (";
						for(int i=0;i<listaIdUO.Count;i++)
						{
							if (ruolo.uo.systemId != listaIdUO[i].ToString())
							{
								queryString=queryString+listaIdUO[i].ToString();

								if(i<listaIdUO.Count-1)
								{
									if(i%998==0 && i > 0)
									{
										queryString = queryString + ") OR A.ID_UO IN (";
									}
									else
									{
										queryString=queryString+",";
									}
								}
								else
								{
									queryString=queryString + " )";
								}
							}

						}

						if(queryString.Substring(queryString.Length-1,1).Equals(","))
						{
							queryString=queryString.Substring(0,queryString.Length-1);
						}
						queryString=queryString+") AND B.NUM_LIVELLO >= " + ruolo.livello + ") OR (A.ID_UO = " + ruolo.uo.systemId + " AND ";
					}
					else
					{
						//caso in cui la UO non ha UO gerarchicamente superiori
						queryString=queryString+" A.ID_UO = " + ruolo.uo.systemId + " AND ";
					}
				}
				if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.SUPERIORE.ToString()))
					segn="<";
				else if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.INFERIORE.ToString()))
					segn=">";
				else if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.PARILIVELLO.ToString()))
					segn="=";
				if (!segn.Equals(""))
					queryString=queryString+" B.NUM_LIVELLO "+segn+ruolo.livello;

				if ((tipoGerarchia.ToString()==DocsPaVO.trasmissione.TipoGerarchia.SUPERIORE.ToString() || tipoGerarchia.ToString()==DocsPaVO.trasmissione.TipoGerarchia.INFERIORE.ToString()) && (listaIdUO!=null && listaIdUO.Count>1) )
				{
					if((listaIdUO != null) && listaIdUO.Count > 1)
					{
						queryString += " )) AND";
					}
					else
					{
						queryString += " AND";
					}
				}
				else 
				{
					if((listaIdUO != null) && listaIdUO.Count > 0)
					{
						queryString= queryString + " AND";
					}
				}
				
				queryString = queryString + " B.SYSTEM_ID=A.ID_TIPO_RUOLO ";
			}
			if(tipoOggetto==DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
			{
				if(idRegistro!=null && idRegistro != "" && !isFiltroAooEnabled())
				{
					queryString=queryString+" AND C.ID_RUOLO_IN_UO=A.SYSTEM_ID";
					queryString=queryString+" AND C.ID_REGISTRO="+idRegistro+"";
				}
			}
			else
			{
				//nel caso di una sottocartella THING contiene la system_id del fascicolo a cui è associata,
				//mentre nel caso di un fascicolo contiene la system_id del nodo di titolario a cui è associata
				queryString=queryString+" AND C.PERSONORGROUP=A.ID_GRUPPO AND C.THING="+idNodoTitolario+" AND C.ACCESSRIGHTS > 0";
				if(idRegistro!=null && idRegistro != "")
				{
					queryString=queryString+" AND D.ID_RUOLO_IN_UO=A.SYSTEM_ID"; 
					queryString=queryString+" AND D.ID_REGISTRO="+idRegistro+"";
				}
			}
			return queryString;
		}
#region GetRuoliRiferimentoAutorizzati_OLD
//		public void GetRuoliRiferimentoAutorizzati(out DataSet dataSet,DocsPaVO.addressbook.QueryCorrispondenteAutorizzato qc, DocsPaVO.utente.UnitaOrganizzativa uo)
//		{
//			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__TIPO_RUOLO3");
//			string idUO = uo.systemId;
//			string strWhere=""; 
//			//condizione sulle autorizzazioni (registro, nodo di titolario)
//			//condizione sulla gerarchia
//			strWhere = getQueryAut(qc.tipoOggetto,qc.idRegistro,qc.idNodoTitolario,qc.ruolo,null,qc.ragione.tipoDestinatario.ToString());
//			strWhere +=" AND A.CHA_TIPO_URP='R' AND A.ID_UO="+idUO+" AND A.CHA_RIFERIMENTO = '1' AND A.CHA_TIPO_IE = 'I'";
//			if (qc.queryCorrispondente!=null && qc.queryCorrispondente.fineValidita)
//				strWhere +=" AND A.DTA_FINE IS NULL";
//			q.setParam("param1", strWhere);
//			string sql = q.getSQL();
//			logger.Debug(sql);
//			
//			this.ExecuteQuery(out dataSet,"RUOLI_RIF",sql);
//		}
		#endregion

		
		public void GetRuoliRiferimentoAutorizzati(out DataSet dataSet,DocsPaVO.addressbook.QueryCorrispondenteAutorizzato qc, DocsPaVO.utente.UnitaOrganizzativa uo)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__TIPO_RUOLO3");
			string idUO = uo.systemId;
			string strWhere=""; 
			//condizione sulle autorizzazioni (registro, nodo di titolario)
			//condizione sulla gerarchia
			//elisa e sabri
			System.Collections.ArrayList ListaUO = new System.Collections.ArrayList();
			if(qc.ragione.tipoDestinatario.ToString()==DocsPaVO.trasmissione.TipoGerarchia.SUPERIORE.ToString())
			{
				DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();
				ListaUO=gerarchia.getParentUO(qc.ruolo);
				ListaUO.Add(qc.ruolo.uo.systemId);
			}
			else if(qc.ragione.tipoDestinatario.ToString()==DocsPaVO.trasmissione.TipoGerarchia.INFERIORE.ToString())
			{
				DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();
				ListaUO=gerarchia.getChildrenUO(qc.ruolo);
				ListaUO.Add(qc.ruolo.uo.systemId);
			}
			else if(qc.ragione.tipoDestinatario.ToString() == DocsPaVO.trasmissione.TipoGerarchia.PARILIVELLO.ToString())
			{
				DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();
				ListaUO=gerarchia.getPariUO(qc.ruolo);
				ListaUO.Add(qc.ruolo.uo.systemId);
			}
			else
			{
				//				DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();
				ListaUO=null;
				//				ListaUO.Add(qc.ruolo.uo.systemId);
			}
			
			
			//			strWhere = getQueryAut(qc.tipoOggetto,qc.idRegistro,qc.idNodoTitolario,qc.ruolo,null,qc.ragione.tipoDestinatario.ToString());
			strWhere = getQueryAut(qc.tipoOggetto,qc.idRegistro,qc.idNodoTitolario,qc.ruolo,ListaUO,qc.ragione.tipoDestinatario.ToString());
			strWhere +=" AND A.CHA_TIPO_URP='R' AND A.ID_UO="+idUO+" AND A.CHA_RIFERIMENTO = '1' AND A.CHA_TIPO_IE = 'I'";
			if (qc.queryCorrispondente!=null && qc.queryCorrispondente.fineValidita)
				strWhere +=" AND A.DTA_FINE IS NULL";
			q.setParam("param1", strWhere);
			string sql = q.getSQL();
            logger.Debug(sql);
			this.ExecuteQuery(out dataSet,"RUOLI_RIF",sql);
		}

        public void GetRuoliRiferimentoEsterni(out DataSet dataSet, DocsPaVO.addressbook.QueryCorrispondenteAutorizzato qc, DocsPaVO.utente.UnitaOrganizzativa uo)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__TIPO_RUOLO3");
            string idUO = uo.systemId;
            string strWhere = "";
            //condizione sulle autorizzazioni (registro, nodo di titolario)
            //condizione sulla gerarchia
            //elisa e sabri
            System.Collections.ArrayList ListaUO = new System.Collections.ArrayList();
            if (qc.ragione.tipoDestinatario.ToString() == DocsPaVO.trasmissione.TipoGerarchia.SUPERIORE.ToString())
            {
                DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();
                ListaUO = gerarchia.getParentUO(qc.ruolo);
                ListaUO.Add(qc.ruolo.uo.systemId);
            }
            else if (qc.ragione.tipoDestinatario.ToString() == DocsPaVO.trasmissione.TipoGerarchia.INFERIORE.ToString())
            {
                DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();
                ListaUO = gerarchia.getChildrenUO(qc.ruolo);
                ListaUO.Add(qc.ruolo.uo.systemId);
            }
            else if (qc.ragione.tipoDestinatario.ToString() == DocsPaVO.trasmissione.TipoGerarchia.PARILIVELLO.ToString())
            {
                DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();
                ListaUO = gerarchia.getPariUO(qc.ruolo);
                ListaUO.Add(qc.ruolo.uo.systemId);
            }
            else
            {
                //				DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();
                ListaUO = null;
                //				ListaUO.Add(qc.ruolo.uo.systemId);
            }


            //			strWhere = getQueryAut(qc.tipoOggetto,qc.idRegistro,qc.idNodoTitolario,qc.ruolo,null,qc.ragione.tipoDestinatario.ToString());
            strWhere = getQueryAut(qc.tipoOggetto, null, qc.idNodoTitolario, qc.ruolo, ListaUO, qc.ragione.tipoDestinatario.ToString());
            strWhere += " AND A.CHA_TIPO_URP='R' AND A.ID_UO=" + idUO + " AND A.CHA_TIPO_IE = 'I'";
            if (qc.queryCorrispondente != null && qc.queryCorrispondente.fineValidita)
                strWhere += " AND A.DTA_FINE IS NULL";
            q.setParam("param1", strWhere);
            string sql = q.getSQL();
            logger.Debug(sql);
            this.ExecuteQuery(out dataSet, "RUOLI_RIF_EST", sql);
        }
		#endregion

		#region le query in Utils.Notifica (1)
		
		/// <summary>
		/// Query per il metodo "notificaByMail"
		/// </summary>
		/// <param name="dataSet"></param>
		/// <param name="idAmm"></param>
		public void getSmtp(out DataSet dataSet, string idAmm)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAAmministra2");
		   q.setParam("param1", "VAR_SMTP, VAR_USER_SMTP, VAR_PWD_SMTP,NUM_PORTA_SMTP, CHA_SMTP_SSL, CHA_SMTP_STA");
		q.setParam("param2",idAmm);
			string myString = q.getSQL();
            logger.Debug(myString);
			this.ExecuteQuery (out dataSet,"SERVER",myString);
		}

		#endregion

		#region le query in Utils.Personalization (7)

		/// <summary>
		/// Query per il metodo "getSeparatorTable"
		/// </summary>
		/// <param name="dataSet"></param>
		/// <param name="idAmm"></param>
		public void getSepar(out DataSet dataSet, string idAmm)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAAmministra2");
			q.setParam("param1","CHA_SEPARATORE");
			q.setParam("param2",idAmm);
			string myString = q.getSQL();
			this.ExecuteQuery (out dataSet,"SEPARATORE",myString);
		}

		/// <summary>
		/// Query per il metodo "setSepFascicolo"
		/// </summary>
		/// <param name="idAmm"></param>
		/// <returns></returns>
		public string getSeparFasc(string idAmm)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAAmministra2");
			q.setParam("param1","CHA_STR_FISSA");
			q.setParam("param2",idAmm);
			string myString = q.getSQL();
			string res;
			this.ExecuteScalar (out res,myString);
			return res;
		}

		/// <summary>
		/// Query per il metodo "setSepSegnatura"
		/// </summary>
		/// <param name="idAmm"></param>
		/// <returns></returns>
		public string getSepSeg(string idAmm)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAAmministra2");
			q.setParam("param1","CHA_STR_SEGNATURA");
			q.setParam("param2",idAmm);
			string myString = q.getSQL();
			string res;
			this.ExecuteScalar (out res,myString);
			return res;
		}

		/// <summary>
		/// Query per il metodo "setFormatFascicolo"
		/// </summary>
		/// <param name="dataSet"></param>
		/// <param name="idAmm"></param>
		public void getFormatFasc(out DataSet dataSet, string idAmm)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAFormattaFasc");
			q.setParam("param1",idAmm);
			string myString = q.getSQL();
			this.ExecuteQuery(out dataSet, myString);
		}

		/// <summary>
		/// Query per il metodo "setFormatSegnatura"
		/// </summary>
		/// <param name="dataSet"></param>
		/// <param name="idAmm"></param>
		public void getFormatSegn(out DataSet dataSet, string idAmm)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAFormattaSegn");
			q.setParam("param1",idAmm);
			string myString = q.getSQL();
			this.ExecuteQuery(out dataSet, myString);
		}

		/// <summary>
		/// Query per il metodo "setLibrary"
		/// </summary>
		/// <param name="idAmm"></param>
		/// <returns></returns>
		public string getLib(string idAmm)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAAmministra2");
			q.setParam("param1","VAR_LIBRERIA");
			q.setParam("param2",idAmm);
			string myString = q.getSQL();
			string res;
			this.ExecuteScalar (out res,myString);
			return res;
		}

		/// <summary>
		/// Query per il metodo "setCodiceAmministrazione"
		/// </summary>
		/// <param name="idAmm"></param>
		/// <returns></returns>
		public string getCodAmm(string idAmm)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAAmministra2");
			q.setParam("param1","VAR_CODICE_AMM");
			q.setParam("param2",idAmm);
			string myString = q.getSQL();
			string res;
			this.ExecuteScalar (out res,myString);
			return res;
		}

        public string getDescAmm(string idAmm) 
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAAmministra2");
            q.setParam("param1", "VAR_DESC_AMM");
            q.setParam("param2", idAmm);
            string myString = q.getSQL();
            string res;
            this.ExecuteScalar(out res, myString);
            return res;
        }

		#endregion

		#region le query in Utils.Security (1)

		/// <summary>
		/// Query per il metodo "isRuoloAutorizzato"
		/// </summary>
		/// <param name="dataSet"></param>
		/// <param name="nomeFunzione"></param>
		/// <param name="idRuolo"></param>
		public void getFunzTipRuolo(out DataSet dataSet,string nomeFunzione,string idRuolo)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_FUNZIONI__TIPO_FUNZIONE__TIPO_F_RUOLO");
			q.setParam("param1",nomeFunzione);
			q.setParam("param2",idRuolo);
			string myString = q.getSQL();
			this.ExecuteQuery (out dataSet,"AUTORIZZAZIONI",myString);
		}

		#endregion

		public string SelectDBTime()
		{
			string dataTimeDb = "";
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DB_TIME");
			string myString = q.getSQL();
			this.ExecuteScalar(out dataTimeDb,myString);
			return dataTimeDb;
		}

        public DateTime SelectDBDateTime()
        {
            string dataTimeDb = "";
            DateTime dbDateTime = DateTime.Now;
            string sql = string.Empty;
            string dbmms = System.Configuration.ConfigurationManager.AppSettings["dbType"].ToUpper();
            string[] dbFormats = { "dd/MM/yyyy HH:mm:ss", "DD/MM/YYYY HH24:MI:SS"};
            CultureInfo ci = new CultureInfo("it-IT");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DB_DATE_TIME");

            if(dbmms.Equals("ORACLE"))
                q.setParam("format", dbFormats[1]);
            else
                q.setParam("format", dbFormats[0]);

            sql = q.getSQL();
            this.ExecuteScalar(out dataTimeDb, sql);
            if (!String.IsNullOrEmpty(dataTimeDb))
                dbDateTime = DateTime.ParseExact(dataTimeDb, "dd/MM/yyyy HH:mm:ss", ci);
            return dbDateTime;
        }

        public string GetDBDate(bool time)
        {
            string dataTimeDb = "";
            string[] dbFormats;
            string sql = string.Empty;
            string dbmms = System.Configuration.ConfigurationManager.AppSettings["dbType"].ToUpper();
            if(time)
                dbFormats = new string[2]{ "dd/MM/yyyy HH:mm:ss", "DD/MM/YYYY HH24:MI:SS" };
            else
                dbFormats = new string[2] { "dd/MM/yyyy", "DD/MM/YYYY" };
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DB_DATE_TIME");

            if(dbmms.Equals("ORACLE"))
                q.setParam("format", dbFormats[1]);
            else
                q.setParam("format", dbFormats[0]);

            sql = q.getSQL();
            this.ExecuteScalar(out dataTimeDb, sql);
            return dataTimeDb;
        }

		public static string getStatoRegistro(DocsPaVO.utente.Registro reg) 
		{
			// R = Rosso -  CHIUSO
			// V = Verde -  APERTO
			// G = Giallo - APERTO IN GIALLO
			
			string dataApertura=reg.dataApertura;

			if (!dataApertura.Equals("")) 
			{
				
				DateTime dt_cor = DateTime.Now;
			
				CultureInfo ci = new CultureInfo("it-IT");
			
				string[] formati={"dd/MM/yyyy HH.mm.ss","dd/MM/yyyy H.mm.ss", "dd/MM/yyyy"};

				DateTime d_ap = DateTime.ParseExact(dataApertura,formati,ci.DateTimeFormat,DateTimeStyles.AllowWhiteSpaces);
				//aggiungo un giorno per fare il confronto con now (che comprende anche minuti e secondi)
				d_ap = d_ap.AddDays(1);
		
				string mydate = dt_cor.ToString(ci);

				//DateTime dt = DateTime.ParseExact(mydate,formati,ci.DateTimeFormat,DateTimeStyles.AllowWhiteSpaces);

				
					if (dt_cor.CompareTo(d_ap)>0) 
					{
						//data odierna maggiore della data di apertura del registro
						return  "G";
					} 
					else
						return "V";
				
			}
			return "R";	

		}

        public bool isFiltroAooEnabled()
        {
            bool filtroAOO = false;
            if (System.Configuration.ConfigurationManager.AppSettings["NO_FILTRO_AOO"] != null && System.Configuration.ConfigurationManager.AppSettings["NO_FILTRO_AOO"] == "1")
                filtroAOO = true;
            return filtroAOO;
        }

        public string countTipoFromTipologia(string tipoOggetto, string tipologiaDoc)
        {
            string ris = string.Empty;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("CDC_COUNT_TIPO_OGG_FROM_TIPOLOGIA");
            q.setParam("oggetto", tipoOggetto);
            q.setParam("tipologia", tipologiaDoc);
            string myString = q.getSQL();
            this.ExecuteScalar(out ris, myString);
            return ris;
        }

        public string countTipoFromTipologiaFasc(string tipoOggetto, string idTipoFasc)
        {
            string ris = string.Empty;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("CDC_COUNT_TIPO_OGG_FROM_TIPOLOGIA_FASC");
            q.setParam("oggetto", tipoOggetto);
            q.setParam("tipologia", idTipoFasc);
            string myString = q.getSQL();
            this.ExecuteScalar(out ris, myString);
            return ris;
        }

        public string countDeferimenti(string from, string to, string ufficio, string idTipoAtto)
        {
            string ris = string.Empty;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("CDC_COUNT_TOTALI_DEFERIMENTI");
            q.setParam("dataFrom", from);
            q.setParam("dataTo", to);
            if (!string.IsNullOrEmpty(ufficio))
                ufficio = "and e.personorgroup in (" + ufficio + ")";
            q.setParam("filterUfficio", ufficio);
            q.setParam("idTipoAtto", idTipoAtto);
            string myString = q.getSQL();
            this.ExecuteScalar(out ris, myString);
            return ris;
        }

        public string countDecretiEsaminati(string from, string to, string ufficio)
        {
            string ris = string.Empty;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("CDC_COUNT_TOTALI_DECRETI_ESAMINATI");
            q.setParam("dataFrom", from);
            q.setParam("dataTo", to);
            if (!string.IsNullOrEmpty(ufficio))
                ufficio = "and e.personorgroup in (" + ufficio + ")";
            q.setParam("filterUfficio", ufficio);
            string myString = q.getSQL();
            this.ExecuteScalar(out ris, myString);
            return ris;
        }

        public DataSet getRegistriByTipologia(string desc_tipologia)
        {
            DataSet ris = new DataSet();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("CDC_GET_REGISTRI_BY_TIPOLOGIA");
            q.setParam("DESC_TIPOLOGIA", desc_tipologia);
            string myString = q.getSQL();
            this.ExecuteQuery(out ris, myString);
            return ris;
        }

        public DataSet getRuoliByRegistro(string idRegistro)
        {
            DataSet ris = new DataSet();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("CDC_GET_RUOLI_BY_REGISTRO");
            q.setParam("idRegistro", idRegistro);
            string myString = q.getSQL();
            this.ExecuteQuery(out ris, myString);
            return ris;
        }

        public DataSet foudKeyWord(string word,string idRegistro)
        {
            DataSet ris = new DataSet();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("CDC_PAROLE_CHIAVE");
            q.setParam("WORD", word);
            q.setParam("ID_REG",idRegistro);
            string myString = q.getSQL();
            this.ExecuteQuery(out ris, myString);
            return ris;
        }

	}
}
