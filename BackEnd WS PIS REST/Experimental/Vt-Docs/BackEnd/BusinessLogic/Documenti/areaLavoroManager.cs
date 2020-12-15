using System;
using System.Data;
using log4net;

namespace BusinessLogic.Documenti
{
	/// <summary>
	/// Summary description for areaLavoroMethods.
	/// </summary>
	public class areaLavoroManager
	{
        private static ILog logger = LogManager.GetLogger(typeof(areaLavoroManager));

		public static void execAddLavoroMethod(string idProfile, string tipoProto, string idRegistro, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.fascicolazione.Fascicolo fasc)
		{
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			if (!doc.ExeAddLavoro(idProfile, tipoProto, idRegistro, infoUtente, fasc))
			{
				logger.Debug("Errore nella gestione dell'area lavoro (execAddLavoroMethod)");
				
				throw new Exception();				
			}
			#region codice originale
			/*
			DocsPaWS.Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();
			DataSet dataSet = new DataSet();
			try
			{
				db.openConnection();
				//costruzione della query
				string idPeople=infoUtente.idPeople;
				string idRuoloInUo=infoUtente.idCorrGlobali;
				System.DateTime now=System.DateTime.Now;
				CultureInfo ci = new CultureInfo("en-US"); 
				string dateString=DocsPaWS.Utils.dbControl.toDate(now.ToString("dd/MM/yyyy hh:mm:ss tt", ci ),true);
				if(infoDoc!=null)
				{
					string idProfile=infoDoc.idProfile;
                    string tipoProto=infoDoc.tipoProto;
					string queryDoc="SELECT SYSTEM_ID, CHA_TIPO_DOC FROM DPA_AREA_LAVORO WHERE ID_PEOPLE="+idPeople+" AND ID_RUOLO_IN_UO="+idRuoloInUo+" AND ID_PROFILE="+idProfile;
					db.fillTable(queryDoc,dataSet,"DOC");
					
					if(dataSet.Tables["DOC"].Rows.Count==0)
					{
						//si esegue l'inserimento
						insertString="INSERT INTO DPA_AREA_LAVORO (SYSTEM_ID,ID_PEOPLE,ID_RUOLO_IN_UO,ID_PROFILE,CHA_TIPO_DOC,DTA_INS)";
						insertString=insertString+" VALUES ('1','"+idPeople+"','"+idRuoloInUo+"','"+idProfile+"','"+tipoProto+"',"+dateString+")";
						db.executeNonQuery(insertString);
					}
					else
					{
						//si fa l'update solo se il tipo documento vecchio è grigio e quello nuovo è diverso da grigio
						if(dataSet.Tables["DOC"].Rows[0]["CHA_TIPO_DOC"].ToString().Equals("G") && !tipoProto.Equals("G"))
						{
						    string updateString="UPDATE DPA_AREA_LAVORO SET CHA_TIPO_DOC="+tipoProto+" WHERE ID_PEOPLE="+idPeople+" AND ID_RUOLO_IN_UO="+idRuoloInUo+" AND ID_PROFILE="+idProfile;
						    db.executeNonQuery(updateString);
						}
					}
				}
				if(fasc!=null)
				{
					string idProject=fasc.systemID;
					string tipoFasc=fasc.tipo;
					string queryFasc="SELECT SYSTEM_ID FROM DPA_AREA_LAVORO WHERE ID_PEOPLE="+idPeople+" AND ID_RUOLO_IN_UO="+idRuoloInUo+" AND ID_PROJECT="+idProject;
					db.fillTable(queryFasc,dataSet,"FASC");
					if(dataSet.Tables["FASC"].Rows.Count==0)
					{
						//si inserisce il nuovo dato
						insertString="INSERT INTO DPA_AREA_LAVORO (SYSTEM_ID,ID_PEOPLE,ID_RUOLO_IN_UO,ID_PROJECT,CHA_TIPO_FASC,DTA_INS)";
						insertString=insertString+" VALUES ('1','"+idPeople+"','"+idRuoloInUo+"','"+idProject+"','"+tipoFasc+"',"+dateString+")";
						db.executeNonQuery(insertString);
					}
				}
				db.closeConnection();
			}
			catch(Exception e)
			{
				db.closeConnection();
				throw e;
			}*/
#endregion
		}

		/// <summary>
		/// </summary>
		/// <param name="utente"></param>
		/// <param name="ruolo"></param>
		/// <param name="tipoObj"></param>
		/// <param name="tipoDoc"></param>
		/// <param name="tipoFasc"></param>
		/// <returns></returns>
		public static DocsPaVO.areaLavoro.AreaLavoro getAreaLavoro(DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.areaLavoro.TipoOggetto tipoObj, DocsPaVO.areaLavoro.TipoDocumento tipoDoc, DocsPaVO.areaLavoro.TipoFascicolo tipoFasc, string idRegistro, bool enableUffRef)
		{
			//DocsPaWS.Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();
			DataSet dataSet;
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti(); 
			DocsPaVO.areaLavoro.AreaLavoro al=new DocsPaVO.areaLavoro.AreaLavoro();
			try
			{
				//db.openConnection();
				string idPeople=utente.idPeople;
				string idRuoloInUo=ruolo.systemId;
				DocsPaVO.areaLavoro.QueryAreaLavoro qal = new DocsPaVO.areaLavoro.QueryAreaLavoro();
				//logger.Debug("prova:"+DocsPaVO.areaLavoro.QueryAreaLavoro.tipoDocString[tipoDoc]);
				//costruzione della query
				doc.GetAreaLavoro(out dataSet, idPeople, idRuoloInUo, tipoObj, tipoDoc, tipoFasc, idRegistro);
				//doc.GetAreaLavoroPaging(out dataSet, idPeople, idRuoloInUo, tipoObj, tipoDoc, tipoFasc,numPage,out numTotPage);
				#region codice originale
				/*string queryString="SELECT * FROM DPA_AREA_LAVORO WHERE ID_PEOPLE='"+idPeople+"' AND ID_RUOLO_IN_UO='"+idRuoloInUo+"'";
				if(tipoObj==DocsPaVO.areaLavoro.TipoOggetto.DOCUMENTO)
				{
					queryString=queryString+" AND ID_PROFILE IS NOT NULL";
					if(tipoDoc!=DocsPaVO.areaLavoro.TipoDocumento.TUTTI){
					  queryString=queryString+" AND CHA_TIPO_DOC='"+DocsPaVO.areaLavoro.QueryAreaLavoro.tipoDocString[tipoDoc]+ "'";
					}
				}
				else
				{
					queryString=queryString+" AND ID_PROJECT IS NOT NULL";
					if(tipoFasc!=DocsPaVO.areaLavoro.TipoFascicolo.TUTTI)
					{
						queryString=queryString+" AND CHA_TIPO_FASC='"+DocsPaVO.areaLavoro.QueryAreaLavoro.tipoFascString[tipoFasc]+"'";
					}
				}
				logger.Debug(queryString);
				db.fillTable(queryString,dataSet,"OGGETTI");
				*/
				#endregion
				//si riempie la lista 
				if(tipoObj==DocsPaVO.areaLavoro.TipoOggetto.DOCUMENTO)
				{
					logger.Debug("lista documenti");
					foreach(DataRow dr in dataSet.Tables["OGGETTI"].Rows)
					{
						DocsPaVO.documento.InfoDocumento id=new DocsPaVO.documento.InfoDocumento();
						id.idProfile=dr["ID_PROFILE"].ToString();
						id.tipoProto=dr["CHA_TIPO_DOC"].ToString();
						//DocsPaVO.utente.InfoUtente infoUtente=new DocsPaVO.utente.InfoUtente(utente,ruolo);
						
						/* mod sab
						DocsPaVO.documento.SchedaDocumento sd=DocManager.getDettaglio(id,sic);
						//DocsPaVO.documento.Documento doc=(DocsPaVO.documento.Documento) sd.documenti[0];
						DocsPaVO.documento.InfoDocumento infoDoc=new DocsPaVO.documento.InfoDocumento(sd);
						*/
						//nuovo
						DocsPaDB.Query_DocsPAWS.Documenti documento = new DocsPaDB.Query_DocsPAWS.Documenti();
						DocsPaVO.documento.InfoDocumento infoDoc;
						infoDoc = documento.GetInfoDocumento(ruolo.idGruppo, utente.idPeople, dr["ID_PROFILE"].ToString(), true);
						//fine nuovo
						if (infoDoc != null)
						{
							infoDoc.tipoProto=dr["CHA_TIPO_DOC"].ToString();
							al.lista.Add(infoDoc);	
						}
					}
				}
				else
				{
					logger.Debug("lista fascicoli");
					foreach(DataRow dr in dataSet.Tables["OGGETTI"].Rows)
					{
                       DocsPaVO.fascicolazione.InfoFascicolo infoFasc=new DocsPaVO.fascicolazione.InfoFascicolo();
					   infoFasc.idFascicolo=dr["ID_PROJECT"].ToString();
						DocsPaVO.utente.InfoUtente sic=new DocsPaVO.utente.InfoUtente(utente,ruolo);
                       //Federica 5 ott
                        DocsPaVO.fascicolazione.Fascicolo fasc = BusinessLogic.Fascicoli.FascicoloManager.getDettaglio(sic, infoFasc, enableUffRef);
					   
						if(fasc == null)
						{
							logger.Debug("Errore nella gestione dell'area lavoro (getAreaLavoro)");
							//TODO: gestire la throw
							throw new Exception();							
						}
						
						fasc.tipo=dr["CHA_TIPO_FASC"].ToString();
					   al.lista.Add(fasc);
					}
				}
//				db.closeConnection();
				return al;

			}
			catch(Exception e)
			{
				logger.Debug("Errore nella gestione dell'area lavoro (getAreaLavoro)",e);
				//db.closeConnection();
				throw e;				
			}
				   
		}

		#region Paging
		/// <summary>
		/// </summary>
		/// <param name="utente"></param>
		/// <param name="ruolo"></param>
		/// <param name="tipoObj"></param>
		/// <param name="tipoDoc"></param>
		/// <param name="tipoFasc"></param>
		/// <param name="numPage"></param>
		/// <param name="numTotPage"></param>
		/// <param name="nRec"></param>
		/// <returns></returns>
        public static DocsPaVO.areaLavoro.AreaLavoro getAreaLavoroPaging(DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.areaLavoro.TipoOggetto tipoObj, DocsPaVO.areaLavoro.TipoDocumento tipoDoc, DocsPaVO.areaLavoro.TipoFascicolo tipoFasc, string idRegistro, bool enableUffRef, string chaDaProto, int numPage, out int numTotPage, out int nRec, DocsPaVO.filtri.FiltroRicerca[][] query = null)
        {
            //DocsPaWS.Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();
            DataSet dataSet;
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            DocsPaVO.areaLavoro.AreaLavoro al = new DocsPaVO.areaLavoro.AreaLavoro();
            try
            {
                //db.openConnection();
                string idPeople = utente.idPeople;
                string idRuoloInUo = ruolo.systemId;
                //DocsPaVO.areaLavoro.QueryAreaLavoro qal = new DocsPaVO.areaLavoro.QueryAreaLavoro();

                //costruzione della query
                //doc.GetAreaLavoro(out dataSet, idPeople, idRuoloInUo, tipoObj, tipoDoc, tipoFasc);
                doc.GetAreaLavoroPaging(out dataSet, idPeople, idRuoloInUo, tipoObj, tipoDoc, tipoFasc, idRegistro, chaDaProto, numPage, out numTotPage, out nRec, query);
                #region codice originale
                /*string queryString="SELECT * FROM DPA_AREA_LAVORO WHERE ID_PEOPLE='"+idPeople+"' AND ID_RUOLO_IN_UO='"+idRuoloInUo+"'";
				if(tipoObj==DocsPaVO.areaLavoro.TipoOggetto.DOCUMENTO)
				{
					queryString=queryString+" AND ID_PROFILE IS NOT NULL";
					if(tipoDoc!=DocsPaVO.areaLavoro.TipoDocumento.TUTTI){
					  queryString=queryString+" AND CHA_TIPO_DOC='"+DocsPaVO.areaLavoro.QueryAreaLavoro.tipoDocString[tipoDoc]+ "'";
					}
				}
				else
				{
					queryString=queryString+" AND ID_PROJECT IS NOT NULL";
					if(tipoFasc!=DocsPaVO.areaLavoro.TipoFascicolo.TUTTI)
					{
						queryString=queryString+" AND CHA_TIPO_FASC='"+DocsPaVO.areaLavoro.QueryAreaLavoro.tipoFascString[tipoFasc]+"'";
					}
				}
				logger.Debug(queryString);
				db.fillTable(queryString,dataSet,"OGGETTI");
				*/
                #endregion
                //si riempie la lista 
                if (tipoObj == DocsPaVO.areaLavoro.TipoOggetto.DOCUMENTO)
                {
                    logger.Debug("lista documenti");
                    foreach (DataRow dr in dataSet.Tables["PagingTable"].Rows)
                    {
                        DocsPaVO.documento.InfoDocumento id = new DocsPaVO.documento.InfoDocumento();
                        id.idProfile = dr["ID_PROFILE"].ToString();
                        id.tipoProto = dr["CHA_TIPO_DOC"].ToString();
                        id.docNumber = dr["DOCNUMBER"].ToString();
                        //DocsPaVO.utente.InfoUtente infoUtente=new DocsPaVO.utente.InfoUtente(utente,ruolo);

                        /* mod sab
                        DocsPaVO.documento.SchedaDocumento sd=DocManager.getDettaglio(id,sic);
                        //DocsPaVO.documento.Documento doc=(DocsPaVO.documento.Documento) sd.documenti[0];
                        DocsPaVO.documento.InfoDocumento infoDoc=new DocsPaVO.documento.InfoDocumento(sd);
                        */
                        //nuovo
                        DocsPaDB.Query_DocsPAWS.Documenti documento = new DocsPaDB.Query_DocsPAWS.Documenti();
                        DocsPaVO.documento.InfoDocumento infoDoc;
                        infoDoc = documento.GetInfoDocumento(ruolo.idGruppo, utente.idPeople, id.docNumber, true);
                        //fine nuovo
                        if (infoDoc != null)
                        {
                            infoDoc.tipoProto = dr["CHA_TIPO_DOC"].ToString();
                            al.lista.Add(infoDoc);
                        }
                    }
                }
                else
                {
                    logger.Debug("lista fascicoli");
                    foreach (DataRow dr in dataSet.Tables["DPA_AREA_LAVORO"].Rows)
                    {
                        DocsPaVO.fascicolazione.InfoFascicolo infoFasc = new DocsPaVO.fascicolazione.InfoFascicolo();
                        infoFasc.idFascicolo = dr["ID_PROJECT"].ToString();
                        DocsPaVO.utente.InfoUtente sic = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                        //Federica 5 ott 2005
                        DocsPaVO.fascicolazione.Fascicolo fasc = BusinessLogic.Fascicoli.FascicoloManager.getDettaglio(sic, infoFasc, enableUffRef);

                        if (fasc == null)
                        {
                            logger.Debug("Errore nella gestione dell'area lavoro (getAreaLavoroPaging)");
                            //TODO: gestire la throw
                            throw new Exception();
                        }

                        fasc.tipo = dr["CHA_TIPO_FASC"].ToString();
                        al.lista.Add(fasc);
                    }
                }
                //				db.closeConnection();
                return al;

            }
            catch (Exception e)
            {
                logger.Debug("Errore nella gestione dell'area lavoro (getAreaLavoroPaging)", e);
                //db.closeConnection();
                throw e;
            }

        }

		#endregion

		/// <summary>
		/// </summary>
		/// <param name="infoUtente"></param>
		/// <param name="infoDoc"></param>
		/// <param name="fasc"></param>
		public static void cancellaAreaLavoro(string idPeople, string idRuoloInUo, string idProfile, DocsPaVO.fascicolazione.Fascicolo fasc)
		{
			logger.Debug("cancellaAreaLavoro");
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti(); 
			/*attenzione: aggiunto controllo su fasc.systemID, in alcuni casi è null quindi si va in 
			 * errore */
			/*if (!doc.DeleteAreaLavoro(fasc.systemID, infoDoc, infoUtente))
			{
				throw new Exception();
			}*/
			bool result;

			if (fasc == null)
			{
				result = doc.DeleteAreaLavoro(idPeople, idRuoloInUo, null, idProfile);
			}
			else
			{
				result = doc.DeleteAreaLavoro(idPeople, idRuoloInUo, fasc.systemID, idProfile);
			}
			if (!result)
			{
				//TODO : gestire la throw
				throw new Exception();
			}

			#region codice originale
			/*DocsPaWS.Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();
			bool dbOpen=false;
			try
			{
				db.openConnection();
				dbOpen=true;
				//costruzione della query
				string idPeople=infoUtente.idPeople;
				string idRuoloInUo=infoUtente.idCorrGlobali;
				string deleteString="DELETE FROM DPA_AREA_LAVORO WHERE ID_PEOPLE="+idPeople+" AND ID_RUOLO_IN_UO="+idRuoloInUo;
				if(infoDoc!=null)
				{
                   deleteString=deleteString+" AND ID_PROFILE="+infoDoc.idProfile;
				}
				else{
				   deleteString=deleteString+" AND ID_PROJECT="+fasc.systemID;
				}
				db.executeNonQuery(deleteString);
				db.closeConnection();
			}
			catch(Exception e){
				if(dbOpen){
				   db.closeConnection();
				}
				throw e;
			}*/
			#endregion
		}

        public static void execAddLavoroRoleMethod(string idProfile, string tipoProto, string idRegistro, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.fascicolazione.Fascicolo fasc)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            if (!doc.ExeAddLavoroRole(idProfile, tipoProto, idRegistro, infoUtente, fasc))
            {
                logger.Debug("Errore nella gestione dell'area lavoro ruolo (execAddLavoroRoleMethod)");

                throw new Exception();
            }
        }

	}
}

