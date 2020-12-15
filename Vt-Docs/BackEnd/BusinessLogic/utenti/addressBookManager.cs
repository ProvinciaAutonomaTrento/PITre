using System;
using System.Web;
using System.Data;
using System.Configuration;
using System.Collections;
using DocsPaVO.addressbook;
using System.Globalization;
using log4net;


namespace BusinessLogic.Utenti
{
    /// <summary>
    /// </summary>
    public class addressBookManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(addressBookManager));
        /// <summary>
        /// </summary>
        /// <param name="queryCorrispondente"></param>
        /// <returns></returns>
        public static ArrayList getListaCorrispondenti(DocsPaVO.addressbook.QueryCorrispondente queryCorrispondente)
        {
            ArrayList objListaCorrispondenti = null;
            //metodo per ottenere la lista
            if (queryCorrispondente.codiceGruppo != null || queryCorrispondente.descrizioneGruppo != null)
            {
                //ricerca per campi riguardanti il gruppo
                //commentato perchè non esiste la gestione dei gruppi 07/03/2005
                //objListaCorrispondenti= addressBookManager.getCorrGruppiMethod(queryCorrispondente);
            }
            else
            {
                //ricerca per campi riguardanti UO, ruolo o utente
                if (queryCorrispondente.tipoUtente == DocsPaVO.addressbook.TipoUtente.ESTERNO)
                {
                    objListaCorrispondenti = addressBookManager.listaCorrEstSciolti(queryCorrispondente);
                }
                if (queryCorrispondente.tipoUtente == DocsPaVO.addressbook.TipoUtente.INTERNO)
                {
                    objListaCorrispondenti = addressBookManager.listaCorrispondentiIntMethod(queryCorrispondente);
                }
                if (queryCorrispondente.tipoUtente == DocsPaVO.addressbook.TipoUtente.GLOBALE)
                {
                    ArrayList objListaCorrispondentiInt = addressBookManager.listaCorrispondentiIntMethod(queryCorrispondente);
                    logger.Debug("Corr int:" + objListaCorrispondentiInt.Count);
                    if ((queryCorrispondente.codiceRubrica != null && queryCorrispondente.codiceRubrica != "") && objListaCorrispondentiInt.Count > 0)
                    {
                        objListaCorrispondenti = objListaCorrispondentiInt;
                    }
                    else
                    {
                        objListaCorrispondenti = addressBookManager.listaCorrEstSciolti(queryCorrispondente);
                        logger.Debug("Corr est:" + objListaCorrispondenti.Count);
                        for (int i = 0; i < objListaCorrispondentiInt.Count; i++)
                        {
                            objListaCorrispondenti.Add(objListaCorrispondentiInt[i]);
                        }
                    }
                }
                //se il codice rubrica non è nullo, ricerca anche dei gruppi
                //commentata perchè non esiste la gestione dei gruppi 07/03/2005
                /*if(queryCorrispondente.codiceRubrica!=null) 
                {
                    logger.Debug("Ricerca nei gruppi");
                    ArrayList objListaCorrGruppo=addressBookManager.getCorrGruppiMethod(queryCorrispondente);
                    for(int i=0;i<objListaCorrGruppo.Count;i++) 
                    {
                        objListaCorrispondenti.Add(objListaCorrGruppo[i]);
                    }
                }*/
            }
            return objListaCorrispondenti;
        }

        #region corrispondenti interni autorizzati protocollo interno -- elisa
        /// <summary>
        /// </summary>
        /// <param name="queryCorrispondente"></param>
        /// <returns></returns>
        public static ArrayList getListaCorrispondentiAutProtInt(DocsPaVO.addressbook.QueryCorrispondente queryCorrispondente)
        {
            ArrayList objListaCorrispondenti = null;

            if (queryCorrispondente.tipoUtente == DocsPaVO.addressbook.TipoUtente.INTERNO)
            {
                objListaCorrispondenti = addressBookManager.listaCorrispondentiInt_Aut_Method(queryCorrispondente);
            }

            return objListaCorrispondenti;
        }


        /// <summary>
        /// </summary>
        /// <param name="qco"></param>
        /// <returns></returns>
        public static ArrayList listaCorrispondentiInt_Aut_Method(DocsPaVO.addressbook.QueryCorrispondente qco)
        {
            ArrayList list = new ArrayList();
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            DocsPaVO.trasmissione.RagioneTrasmissione ragTrasm = null;
            ragTrasm = BusinessLogic.Trasmissioni.RagioniManager.GetRagione("TO", qco.idAmministrazione);
            list = utenti.ListaCorrispondentiInt_Aut(qco, ragTrasm);
            if (list == null)
            {
                logger.Debug("Errore nella gestione degli utenti (listaCorrispondentiInt_Aut_Method)");
                throw new Exception();
            }
            return list;

        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qco"></param>
        /// <returns></returns>
        public static ArrayList listaCorrispondentiEstMethod(DocsPaVO.addressbook.QueryCorrispondente qco)
        {
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            return utenti.ListaCorrispondentiEst(qco);

            #region Codice Commentato
            /*logger.Debug("listaCorrispondentiEstMethod");
			DocsPa_V15_Utils.Database database = DocsPaWS.Utils.dbControl.getDatabase();
			database.openConnection();
			DataSet dataSet= new DataSet();
			DocsPaVO.addressbook.QueryCorrispondente objQueryCorrispondente=correctApiciQuery(qco);
			ArrayList listaCorr=new  ArrayList();
			try
			{
				//costruzione della query in base alla richiesta ricevuta
				string commandString1="";
				string commandString_begin="";
				string commandString_UO="";
				string commandString_ruolo="";
				string commandString_utente="";
             
				string generalCondition="((ID_AMM IS NULL) OR (ID_REGISTRO IS NULL AND ID_AMM='"+objQueryCorrispondente.idAmministrazione+"')";
				generalCondition=generalCondition+registriCondition(objQueryCorrispondente,null)+") ";
				
				
				if(objQueryCorrispondente.codiceRubrica!=null)
				{
					string generalConditionCR=" ((ID_AMM IS NULL) OR (ID_AMM='"+objQueryCorrispondente.idAmministrazione+"'))";
					
					string parentString="SELECT * FROM DPA_CORR_GLOBALI A WHERE VAR_COD_RUBRICA='"+objQueryCorrispondente.codiceRubrica+"' and cha_tipo_ie='E' AND CHA_TIPO_CORR='S' AND "+generalConditionCR;
					
					database.fillTable(parentString,dataSet,"PARENT");
					    
					//nel caso non esiste oggetto parent con tale codice rubrica
					if(dataSet.Tables["PARENT"].Rows.Count==0) return listaCorr;
					
					if(objQueryCorrispondente.getChildren==false)
					{
						if(dataSet.Tables["PARENT"].Rows[0]["CHA_TIPO_URP"].Equals("P"))
						{
							commandString1="SELECT A.SYSTEM_ID, A.VAR_COGNOME, A.VAR_NOME, A.VAR_DESC_CORR, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.CHA_DETTAGLI, A.CHA_TIPO_URP, A.ID_REGISTRO, A.VAR_EMAIL, A.VAR_SMTP, A.NUM_PORTA_SMTP, C.SYSTEM_ID AS RUOLO_SYSTEM_ID, C.VAR_DESC_CORR AS RUOLO_DESC, C.VAR_CODICE AS RUOLO_CODICE, C.ID_UO AS RUOLO_ID_UO, C.VAR_COD_RUBRICA AS RUOLO_COD_RUBRICA, C.CHA_DETTAGLI AS RUOLO_DETTAGLI  FROM  DPA_CORR_GLOBALI A,DPA_RUOE_UTENTE B, DPA_CORR_GLOBALI C WHERE B.ID_UTENTE_EST=A.SYSTEM_ID AND C.SYSTEM_ID=ID_RUOE AND A.VAR_COD_RUBRICA='"+objQueryCorrispondente.codiceRubrica+"'";
							commandString1=commandString1+" AND ((A.ID_AMM IS NULL) OR (A.ID_AMM='"+objQueryCorrispondente.idAmministrazione+"')) AND A.CHA_TIPO_CORR='S'";
							
						}
						else
						{
							commandString1=parentString;
						}
					}
					else
					{
						DataRow parentRow=dataSet.Tables["PARENT"].Rows[0];
						
						//se il parent è un utente si restituisce la lista vuota
						if(parentRow["CHA_TIPO_URP"].ToString().Equals("P")) return listaCorr;

						//se il parent è una UO
						if(parentRow["CHA_TIPO_URP"].ToString().Equals("U"))
						{
							commandString1="SELECT SYSTEM_ID, ID_REGISTRO, VAR_EMAIL, CHA_PA, VAR_DESC_CORR, VAR_CODICE, VAR_COD_RUBRICA, VAR_CODICE_AMM, VAR_CODICE_ISTAT, VAR_CODICE_AOO, CHA_TIPO_URP, ID_UO, ID_PARENT, NUM_LIVELLO, CHA_DETTAGLI, VAR_SMTP, NUM_PORTA_SMTP FROM DPA_CORR_GLOBALI A WHERE (ID_UO='"+parentRow["SYSTEM_ID"].ToString()+"' OR ID_PARENT='"+parentRow["SYSTEM_ID"].ToString()+"') and cha_tipo_ie='E'";
						};

						//se il parent è un ruolo
						if(parentRow["CHA_TIPO_URP"].ToString().Equals("R"))
						{
							commandString1="SELECT A.SYSTEM_ID, A.ID_REGISTRO, A.VAR_COGNOME, A.VAR_NOME, A.VAR_DESC_CORR, A.VAR_EMAIL, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.CHA_DETTAGLI, A.CHA_TIPO_URP, A.VAR_SMTP, A.NUM_PORTA_SMTP, C.SYSTEM_ID AS RUOLO_SYSTEM_ID, C.VAR_DESC_CORR AS RUOLO_DESC, C.VAR_CODICE AS RUOLO_CODICE, C.VAR_COD_RUBRICA AS RUOLO_COD_RUBRICA , C.CHA_DETTAGLI AS RUOLO_DETTAGLI, C.ID_UO AS RUOLO_ID_UO FROM  DPA_CORR_GLOBALI A,DPA_RUOE_UTENTE B, DPA_CORR_GLOBALI C WHERE A.SYSTEM_ID=B.ID_UTENTE_EST AND C.SYSTEM_ID=B.ID_RUOE AND B.ID_RUOE='"+parentRow["SYSTEM_ID"].ToString()+"' and a.cha_tipo_ie='E'";
						};
					}

				}
				else
				{
					//la query viene fatta in base all'UO, al ruolo e all'utente
					commandString_begin="SELECT * FROM DPA_CORR_GLOBALI A WHERE "+generalCondition;
						
					//se la UO non è nulla
					if(objQueryCorrispondente.codiceUO!=null)
					{
						commandString_UO=commandString_begin+" AND VAR_CODICE='"+objQueryCorrispondente.codiceUO+"' AND CHA_TIPO_URP='U' AND CHA_TIPO_IE='E' AND CHA_TIPO_CORR='S'";
					};
					if(objQueryCorrispondente.descrizioneUO!=null)
					{
						commandString_UO=commandString_begin+" AND CHA_TIPO_URP='U' AND CHA_TIPO_IE='E' AND CHA_TIPO_CORR='S' AND (";
						char[] separator=ConfigurationManager.AppSettings["separator"].ToCharArray();
						string[] uo_list=objQueryCorrispondente.descrizioneUO.Split(separator);
						for(int i=0;i<uo_list.Length;i++)
						{
							commandString_UO=commandString_UO+"UPPER(VAR_DESC_CORR) LIKE UPPER('%"+uo_list[i].Replace(" ","%")+"%')";
							if(i<uo_list.Length-1){commandString_UO= commandString_UO+" OR ";}
						};
						commandString_UO=commandString_UO+")";                            
					}
					commandString1=commandString_UO;
					    
					//se il ruolo non è nullo
					if(objQueryCorrispondente.isRuoloDefined())
					{
						commandString_ruolo=commandString_begin+" AND VAR_DESC_CORR LIKE '%"+objQueryCorrispondente.descrizioneRuolo+"%' AND CHA_TIPO_URP='R' AND CHA_TIPO_IE='E' ";
						commandString1=commandString_ruolo;
					}

					//se l'utente non è nullo
					if(objQueryCorrispondente.isUtenteDefined())
					{
						string generalCondition_utente="((A.ID_AMM IS NULL) OR (A.ID_REGISTRO IS NULL AND A.ID_AMM='"+objQueryCorrispondente.idAmministrazione+"')"+registriCondition(objQueryCorrispondente,"A")+")";
						
						string commandString_utente_begin="SELECT A.SYSTEM_ID, A.VAR_NOME, A.VAR_COGNOME, A.VAR_DESC_CORR, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.CHA_DETTAGLI, A.CHA_TIPO_URP, A.ID_REGISTRO, A.VAR_EMAIL, A.VAR_SMTP, A.NUM_PORTA_SMTP, C.SYSTEM_ID AS RUOLO_SYSTEM_ID, C.VAR_DESC_CORR AS RUOLO_DESC, C.VAR_CODICE AS RUOLO_CODICE, C.ID_UO AS RUOLO_ID_UO, C.VAR_COD_RUBRICA AS RUOLO_COD_RUBRICA, C.CHA_DETTAGLI AS RUOLO_DETTAGLI  FROM  DPA_CORR_GLOBALI A,DPA_RUOE_UTENTE B, DPA_CORR_GLOBALI C WHERE "+generalCondition_utente+" AND B.ID_UTENTE_EST=A.SYSTEM_ID AND C.SYSTEM_ID=ID_RUOE";
						if(objQueryCorrispondente.nomeUtente!=null)
						{
							commandString_utente=commandString_utente_begin+" AND A.VAR_NOME LIKE '"+objQueryCorrispondente.nomeUtente+"%' AND A.CHA_TIPO_URP='P' AND A.CHA_TIPO_IE='E' AND A.CHA_TIPO_CORR='S'";
						}
						else
						{
							commandString_utente=commandString_utente_begin+" AND A.VAR_COGNOME LIKE '"+objQueryCorrispondente.cognomeUtente+"%' AND A.CHA_TIPO_URP='P' AND A.CHA_TIPO_IE='E' AND A.CHA_TIPO_CORR='S'";
						}

						//controllo se la query ha condizioni sui ruoli
						if(objQueryCorrispondente.isRuoloDefined())
						{
						
							commandString_utente=commandString_utente+" AND C.VAR_DESC_CORR LIKE '%"+objQueryCorrispondente.descrizioneRuolo+"%'";
						}
						commandString1=commandString_utente;

					}
				
				}

				//Fabio
				if(objQueryCorrispondente.fineValidita)
					commandString1 += " AND A.DTA_FINE IS NULL";

				logger.Debug(commandString1);
				
				database.fillTable(commandString1,dataSet,"CORRISPONDENTI");
				
				ArrayList list=new ArrayList();
				
				//riempimento dell'oggetto finale
				foreach(DataRow corrRow in dataSet.Tables["CORRISPONDENTI"].Rows)
				{
					//ricerca delle UO parent: viene riempita una tabella ottimizzata
					/*string commandString2="";
					if(corrRow["CHA_TIPO_URP"].ToString().Equals("U"))
					{
						commandString2="SELECT SYSTEM_ID, ID_REGISTRO, VAR_EMAIL, CHA_PA, VAR_DESC_CORR, NUM_LIVELLO, CHA_DETTAGLI, VAR_CODICE, VAR_COD_RUBRICA, VAR_CODICE_AMM, VAR_CODICE_ISTAT, VAR_CODICE_AOO, VAR_SMTP, NUM_PORTA_SMTP, ID_PARENT FROM DPA_CORR_GLOBALI A WHERE CHA_TIPO_URP='U' AND CHA_TIPO_IE='E' AND NUM_LIVELLO <="+corrRow["NUM_LIVELLO"].ToString();
					};
					if(corrRow["CHA_TIPO_URP"].ToString().Equals("R"))
					{
						commandString2="SELECT A.SYSTEM_ID, A.ID_REGISTRO, A.VAR_EMAIL, A.CHA_PA, A.VAR_DESC_CORR, A.NUM_LIVELLO, A.CHA_DETTAGLI, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_CODICE_AMM, A.VAR_CODICE_ISTAT, A.VAR_CODICE_AOO, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_PARENT FROM DPA_CORR_GLOBALI A, DPA_CORR_GLOBALI B WHERE A.CHA_TIPO_URP='U' AND A.CHA_TIPO_IE='E' AND B.SYSTEM_ID='"+corrRow["ID_UO"].ToString()+"' AND A.NUM_LIVELLO <= B.NUM_LIVELLO";
					};
					if(corrRow["CHA_TIPO_URP"].ToString().Equals("P"))
					{
						commandString2="SELECT A.SYSTEM_ID, A.ID_REGISTRO, A.VAR_EMAIL, A.CHA_PA, A.VAR_DESC_CORR, A.NUM_LIVELLO, A.CHA_DETTAGLI, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_CODICE_AMM, A.VAR_CODICE_ISTAT, A.VAR_CODICE_AOO, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_PARENT FROM DPA_CORR_GLOBALI A, DPA_CORR_GLOBALI B WHERE A.CHA_TIPO_URP='U' AND A.CHA_TIPO_IE='E' AND B.SYSTEM_ID='"+corrRow["RUOLO_ID_UO"].ToString()+"' AND A.NUM_LIVELLO <= B.NUM_LIVELLO";
					};
					logger.Debug(commandString2);
					
					database.fillTable(commandString2,dataSet,"UO");

					obj.ricercaUOParent(dataSet,corrRow);

					//l'oggetto viene riempito in base alla sua tipologia
					if(corrRow["CHA_TIPO_URP"].ToString().Equals("U"))
					{
						DocsPaVO.utente.UnitaOrganizzativa corrispondenteUO= new DocsPaVO.utente.UnitaOrganizzativa();
						corrispondenteUO.systemId= corrRow["SYSTEM_ID"].ToString();
						corrispondenteUO.descrizione=corrRow["VAR_DESC_CORR"].ToString();
						corrispondenteUO.codiceCorrispondente=corrRow["VAR_CODICE"].ToString();
						corrispondenteUO.codiceRubrica=corrRow["VAR_COD_RUBRICA"].ToString();
						if(corrRow["ID_REGISTRO"]!=null)
						{
							corrispondenteUO.idRegistro=corrRow["ID_REGISTRO"].ToString();
						}
						corrispondenteUO.email=corrRow["VAR_EMAIL"].ToString();
						corrispondenteUO.interoperante=fromCharToBool(corrRow["CHA_PA"].ToString());
						corrispondenteUO.dettagli=fromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
						DocsPaVO.utente.ServerPosta sp=new DocsPaVO.utente.ServerPosta();
						sp.serverSMTP=corrRow["VAR_SMTP"].ToString();
						sp.portaSMTP=corrRow["NUM_PORTA_SMTP"].ToString();
						corrispondenteUO.serverPosta=sp;
						corrispondenteUO.codiceAOO=corrRow["VAR_CODICE_AOO"].ToString();
						corrispondenteUO.codiceAmm=corrRow["VAR_CODICE_AMM"].ToString();
						corrispondenteUO.codiceIstat=corrRow["VAR_CODICE_ISTAT"].ToString();
						corrispondenteUO.livello=corrRow["NUM_LIVELLO"].ToString();
						corrispondenteUO.tipoIE="E";
						//qui si ritrova la parentela
						logger.Debug("IDPAR: "+corrRow["ID_PARENT"].ToString());
						if(!corrRow["ID_PARENT"].ToString().Equals("0")) 
						{
							corrispondenteUO.parent=getParents(corrRow["ID_PARENT"].ToString(),dataSet.Tables["UO"]);
						}
						list.Add(corrispondenteUO);
					}
					if(corrRow["CHA_TIPO_URP"].ToString().Equals("R"))
					{
						DocsPaVO.utente.Ruolo corrispondenteRuolo= new DocsPaVO.utente.Ruolo();
						corrispondenteRuolo.systemId= corrRow["SYSTEM_ID"].ToString();
						corrispondenteRuolo.descrizione=corrRow["VAR_DESC_CORR"].ToString();
						corrispondenteRuolo.codiceCorrispondente=corrRow["VAR_CODICE"].ToString();
						corrispondenteRuolo.codiceRubrica=corrRow["VAR_COD_RUBRICA"].ToString();
						corrispondenteRuolo.dettagli=fromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
						DocsPaVO.utente.ServerPosta sp=new DocsPaVO.utente.ServerPosta();
						sp.serverSMTP=corrRow["VAR_SMTP"].ToString();
						sp.portaSMTP=corrRow["NUM_PORTA_SMTP"].ToString();
						corrispondenteRuolo.serverPosta=sp;
						corrispondenteRuolo.tipoIE="E";
						if(corrRow["ID_REGISTRO"]!=null)
						{
							corrispondenteRuolo.idRegistro=corrRow["ID_REGISTRO"].ToString();
						}
						//qui si ritrova la parentela (con filtro o no)
						if(objQueryCorrispondente.isUODefined())
						{
							if((objQueryCorrispondente.descrizioneUO!=null && hasDefinedUo(objQueryCorrispondente.descrizioneUO,1,corrRow["ID_UO"].ToString(),dataSet.Tables["UO"]))||(objQueryCorrispondente.codiceUO!=null && hasDefinedUo(objQueryCorrispondente.codiceUO,2,corrRow["ID_UO"].ToString(),dataSet.Tables["UO"])))
							{
								corrispondenteRuolo.uo=getParents(corrRow["ID_UO"].ToString(),dataSet.Tables["UO"]);
								list.Add(corrispondenteRuolo);
							}
						}
						else
						{
							corrispondenteRuolo.uo=getParents(corrRow["ID_UO"].ToString(),dataSet.Tables["UO"]);
							list.Add(corrispondenteRuolo);
						}
					}
					if(corrRow["CHA_TIPO_URP"].ToString().Equals("P"))
					{
						DocsPaVO.utente.Utente corrispondenteUtente= new DocsPaVO.utente.Utente();
						corrispondenteUtente.systemId= corrRow["SYSTEM_ID"].ToString();
						corrispondenteUtente.descrizione=corrRow["VAR_COGNOME"].ToString()+" "+corrRow["VAR_NOME"].ToString();
						corrispondenteUtente.codiceCorrispondente=corrRow["VAR_CODICE"].ToString();
						corrispondenteUtente.codiceRubrica=corrRow["VAR_COD_RUBRICA"].ToString();
						corrispondenteUtente.dettagli=fromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
						DocsPaVO.utente.ServerPosta sp=new DocsPaVO.utente.ServerPosta();
						sp.serverSMTP=corrRow["VAR_SMTP"].ToString();
						sp.portaSMTP=corrRow["NUM_PORTA_SMTP"].ToString();
						corrispondenteUtente.serverPosta=sp;
						corrispondenteUtente.tipoIE="E";
						if(corrRow["ID_REGISTRO"]!=null)
						{
							corrispondenteUtente.idRegistro=corrRow["ID_REGISTRO"].ToString();
						}
						corrispondenteUtente.email=corrRow["VAR_EMAIL"].ToString();
						//si trova il ruolo corrispondente
						logger.Debug("riempimento ruolo");
						DocsPaVO.utente.Ruolo ruoloUtente=new DocsPaVO.utente.Ruolo();
						ruoloUtente.systemId=corrRow["RUOLO_SYSTEM_ID"].ToString();
						ruoloUtente.descrizione=corrRow["RUOLO_DESC"].ToString();
						ruoloUtente.codiceCorrispondente=corrRow["RUOLO_CODICE"].ToString();
						ruoloUtente.codiceRubrica=corrRow["RUOLO_COD_RUBRICA"].ToString();
						ruoloUtente.dettagli=fromCharToBool(corrRow["RUOLO_DETTAGLI"].ToString());
						//qui si trova la parentela (con filtro o no)
						logger.Debug("Riempimento UO");
						if(objQueryCorrispondente.isUODefined())
						{
							if((objQueryCorrispondente.descrizioneUO!=null && hasDefinedUo(objQueryCorrispondente.descrizioneUO,1,corrRow["RUOLO_ID_UO"].ToString(),dataSet.Tables["UO"]))||(objQueryCorrispondente.codiceUO!=null && hasDefinedUo(objQueryCorrispondente.codiceUO,2,corrRow["RUOLO_ID_UO"].ToString(),dataSet.Tables["UO"])))
							{
								ruoloUtente.uo=getParents(corrRow["RUOLO_ID_UO"].ToString(),dataSet.Tables["UO"]);
						
								ArrayList ruoli=new ArrayList();
								ruoli.Add(ruoloUtente);
								corrispondenteUtente.ruoli=ruoli;
								list.Add(corrispondenteUtente);
							}
						}
						else
						{
							ruoloUtente.uo=getParents(corrRow["RUOLO_ID_UO"].ToString(),dataSet.Tables["UO"]);
						
							ArrayList ruoli=new ArrayList();
							ruoli.Add(ruoloUtente);
							corrispondenteUtente.ruoli=ruoli;
							list.Add(corrispondenteUtente);
						}	
						
					}
				}
				listaCorr=list;
				
				database.closeConnection();
			 
			}
			catch(Exception e)
			{
				logger.Debug("Eccezione: "+e); 
				
				database.closeConnection();
				throw e;}
			return listaCorr;*/
            #endregion
        }

        public static ArrayList listaCorrispondentiOccMethod(DocsPaVO.addressbook.QueryCorrispondente qco)
        {
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            return utenti.ListaCorrispondentiOcc(qco);

        }

        //METODO PER GLI UTENTI INTERNI
        /// <summary>
        /// </summary>
        /// <param name="qco"></param>
        /// <returns></returns>
        public static ArrayList listaCorrispondentiIntMethod(DocsPaVO.addressbook.QueryCorrispondente qco)
        {
            ArrayList list = new ArrayList();
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            list = utenti.ListaCorrispondentiInt(qco);
            if (list == null)
            {
                logger.Debug("Errore nella gestione degli utenti (listaCorrispondentiIntMethod)");
                throw new Exception();
            }
            return list;

            #region Codice Commentato
            /*logger.Debug("listaCorrispondentiIntMethod");
			DocsPa_V15_Utils.Database database=DocsPaWS.Utils.dbControl.getDatabase();
			
			database.openConnection();
			DataSet dataSet= new DataSet();
			DocsPaVO.addressbook.QueryCorrispondente objQueryCorrispondente=correctApiciQuery(qco);
			ArrayList listaCorr=new  ArrayList();
			try
			{
				//costruzione della query in base alla richiesta ricevuta
				string commandString1="";
				string commandString_begin="";
				string commandString_UO="";
				string commandString_ruolo="";
				string commandString_utente="";
             
				string generalCondition="((ID_AMM IS NULL) OR (ID_REGISTRO IS NULL AND ID_AMM='"+objQueryCorrispondente.idAmministrazione+"')"+registriCondition(objQueryCorrispondente,null)+")";
				if(objQueryCorrispondente.codiceRubrica!=null)
				{
					string generalConditionCR=" ((ID_AMM IS NULL) OR (ID_AMM='"+objQueryCorrispondente.idAmministrazione+"'))";
					string parentString="SELECT * FROM DPA_CORR_GLOBALI A WHERE VAR_COD_RUBRICA='"+objQueryCorrispondente.codiceRubrica+"' and cha_tipo_ie='I' AND CHA_TIPO_CORR='S' AND "+generalConditionCR;
					logger.Debug(parentString);
					
					database.fillTable(parentString,dataSet,"PARENT");
					    
					//nel caso non esiste oggetto parent con tale codice rubrica
					if(dataSet.Tables["PARENT"].Rows != null && dataSet.Tables["PARENT"].Rows.Count==0) return listaCorr;
					logger.Debug("Parent rows > 0");
					if(objQueryCorrispondente.getChildren==false)
					{
						if(dataSet.Tables["PARENT"].Rows[0]["CHA_TIPO_URP"].Equals("P"))
						{
							commandString1="SELECT A.SYSTEM_ID,A.ID_PEOPLE,A.ID_REGISTRO, A.ID_AMM, E.VAR_NOME, E.VAR_COGNOME, E.EMAIL_ADDRESS, E.CHA_NOTIFICA, E.VAR_TELEFONO, A.VAR_DESC_CORR, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.CHA_DETTAGLI, A.CHA_TIPO_URP, A.VAR_SMTP, A.NUM_PORTA_SMTP, C.SYSTEM_ID AS RUOLO_SYSTEM_ID, D.VAR_DESC_RUOLO AS RUOLO_DESC, C.VAR_CODICE AS RUOLO_CODICE, C.ID_UO AS RUOLO_ID_UO, C.VAR_COD_RUBRICA AS RUOLO_COD_RUBRICA, C.CHA_DETTAGLI AS RUOLO_DETTAGLI  FROM  DPA_CORR_GLOBALI A, PEOPLEGROUPS B, DPA_CORR_GLOBALI C, PEOPLE E, DPA_TIPO_RUOLO D WHERE B.PEOPLE_SYSTEM_ID=A.ID_PEOPLE AND C.ID_GRUPPO=B.GROUPS_SYSTEM_ID AND A.VAR_COD_RUBRICA='"+objQueryCorrispondente.codiceRubrica+"' AND D.SYSTEM_ID=C.ID_TIPO_RUOLO AND E.SYSTEM_ID=A.ID_PEOPLE ";
							logger.Debug(commandString1);
							commandString1=commandString1+" AND ((A.ID_AMM IS NULL) OR (A.ID_AMM='"+objQueryCorrispondente.idAmministrazione+"')) AND A.CHA_TIPO_CORR='S'";
							logger.Debug(commandString1);
						}
						else if(dataSet.Tables["PARENT"].Rows[0]["CHA_TIPO_URP"].Equals("R"))
						{
							commandString1="SELECT A.SYSTEM_ID, A.ID_REGISTRO, A.ID_AMM, A.VAR_DESC_CORR, B.VAR_DESC_RUOLO, B.NUM_LIVELLO AS RUOLO_LIVELLO, A.ID_GRUPPO, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.CHA_TIPO_URP, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_UO, A.ID_PARENT, A.NUM_LIVELLO, A.CHA_DETTAGLI FROM DPA_CORR_GLOBALI A, DPA_TIPO_RUOLO B WHERE A.ID_TIPO_RUOLO=B.SYSTEM_ID AND A.VAR_COD_RUBRICA='"+objQueryCorrispondente.codiceRubrica+"'";
							logger.Debug(commandString1);
							commandString1=commandString1+" AND ((A.ID_AMM IS NULL) OR (A.ID_AMM='"+objQueryCorrispondente.idAmministrazione+"')) AND A.CHA_TIPO_CORR='S'";
							logger.Debug(commandString1);
							logger.Debug(commandString1);
						}
						else
						{
							commandString1=parentString;
						}
					}
					else
					{
						DataRow parentRow=dataSet.Tables["PARENT"].Rows[0];
						
						//se il parent è un utente si restituisce la lista vuota
						if(parentRow["CHA_TIPO_URP"].ToString().Equals("P")) return listaCorr;

						//se il parent è una UO
						if(parentRow["CHA_TIPO_URP"].ToString().Equals("U"))
						{
							commandString1="SELECT A.SYSTEM_ID, A.VAR_DESC_CORR, A.ID_REGISTRO, A.ID_AMM, A.VAR_EMAIL, A.CHA_PA, B.VAR_DESC_RUOLO, B.NUM_LIVELLO AS RUOLO_LIVELLO, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_CODICE_AMM, A.VAR_CODICE_ISTAT, A.VAR_CODICE_AOO, A.CHA_TIPO_URP, A.ID_UO, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_PARENT, A.NUM_LIVELLO, A.ID_GRUPPO, A.CHA_DETTAGLI FROM DPA_CORR_GLOBALI A, DPA_TIPO_RUOLO B WHERE (A.ID_TIPO_RUOLO=B.SYSTEM_ID OR A.ID_TIPO_RUOLO IS NULL) AND (A.ID_UO='"+parentRow["SYSTEM_ID"].ToString()+"' OR A.ID_PARENT='"+parentRow["SYSTEM_ID"].ToString()+"') and a.cha_tipo_ie='I' AND A.CHA_TIPO_CORR='S'";
						};

						//se il parent è un ruolo
						if(parentRow["CHA_TIPO_URP"].ToString().Equals("R"))
						{
							commandString1="SELECT A.SYSTEM_ID, A.ID_PEOPLE, A.ID_REGISTRO, A.ID_AMM, E.VAR_COGNOME, E.VAR_NOME, E.EMAIL_ADDRESS, E.CHA_NOTIFICA, E.VAR_TELEFONO, A.VAR_DESC_CORR, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.CHA_DETTAGLI, A.CHA_TIPO_URP, A.VAR_SMTP, A.NUM_PORTA_SMTP, C.SYSTEM_ID AS RUOLO_SYSTEM_ID, D.VAR_DESC_RUOLO AS RUOLO_DESC, C.VAR_CODICE AS RUOLO_CODICE, C.VAR_COD_RUBRICA AS RUOLO_COD_RUBRICA , C.CHA_DETTAGLI AS RUOLO_DETTAGLI, C.ID_UO AS RUOLO_ID_UO FROM  DPA_CORR_GLOBALI A,PEOPLEGROUPS B, DPA_CORR_GLOBALI C, DPA_TIPO_RUOLO D, PEOPLE E WHERE A.ID_PEOPLE=B.PEOPLE_SYSTEM_ID AND C.ID_GRUPPO=B.GROUPS_SYSTEM_ID AND B.GROUPS_SYSTEM_ID='"+parentRow["ID_GRUPPO"].ToString()+"' AND D.SYSTEM_ID=C.ID_TIPO_RUOLO AND E.SYSTEM_ID=A.ID_PEOPLE and a.cha_tipo_ie='I' AND A.CHA_TIPO_CORR='S'";
						}
					}

					logger.Debug(commandString1);
				}
				else
				{
					//la query viene fatta in base all'UO, al ruolo e all'utente
					commandString_begin="SELECT * FROM DPA_CORR_GLOBALI A WHERE "+generalCondition;
						
					//se la UO non è nulla
					if(objQueryCorrispondente.codiceUO!=null)
					{
						commandString_UO=commandString_begin+" AND VAR_CODICE='"+objQueryCorrispondente.codiceUO+"' AND CHA_TIPO_URP='U' AND CHA_TIPO_IE='I' AND CHA_TIPO_CORR='S'";
					};
					if(objQueryCorrispondente.descrizioneUO!=null)
					{
						commandString_UO=commandString_begin+" AND CHA_TIPO_URP='U' AND CHA_TIPO_IE='I' AND CHA_TIPO_CORR='S' AND (";
						char[] separator=ConfigurationManager.AppSettings["separator"].ToCharArray();
						string[] uo_list=objQueryCorrispondente.descrizioneUO.Split(separator);
						for(int i=0;i<uo_list.Length;i++)
						{
							commandString_UO=commandString_UO+"UPPER(VAR_DESC_CORR) LIKE UPPER('%"+uo_list[i].Replace(" ","%")+"%')";
							if(i<uo_list.Length-1){commandString_UO= commandString_UO+" OR ";}
						};
						commandString_UO=commandString_UO+")";                            
					}
					commandString1=commandString_UO;
					    
					//se il ruolo non è nullo
					if(objQueryCorrispondente.isRuoloDefined())
					{
						commandString_ruolo="SELECT A.*, B.VAR_DESC_RUOLO, B.NUM_LIVELLO AS RUOLO_LIVELLO FROM DPA_CORR_GLOBALI A, DPA_TIPO_RUOLO B WHERE A.ID_TIPO_RUOLO=B.SYSTEM_ID AND B.VAR_DESC_RUOLO LIKE '%"+objQueryCorrispondente.descrizioneRuolo+"%' AND CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' ";
						commandString1=commandString_ruolo;
					}

					//se l'utente non è nullo
					if(objQueryCorrispondente.isUtenteDefined())
					{
						string generalCondition_utente="((A.ID_AMM IS NULL) OR (A.ID_REGISTRO IS NULL AND A.ID_AMM='"+objQueryCorrispondente.idAmministrazione+"')"+registriCondition(objQueryCorrispondente,"A")+") ";
						string commandString_utente_begin="SELECT A.SYSTEM_ID, A.ID_PEOPLE, A.ID_REGISTRO, A.ID_AMM, E.VAR_NOME, E.VAR_COGNOME, E.EMAIL_ADDRESS, E.CHA_NOTIFICA, E.VAR_TELEFONO, A.VAR_DESC_CORR, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.CHA_DETTAGLI, A.CHA_TIPO_URP, A.VAR_SMTP, A.NUM_PORTA_SMTP, C.SYSTEM_ID AS RUOLO_SYSTEM_ID, D.VAR_DESC_RUOLO AS RUOLO_DESC, C.VAR_CODICE AS RUOLO_CODICE, C.ID_UO AS RUOLO_ID_UO, C.VAR_COD_RUBRICA AS RUOLO_COD_RUBRICA, C.CHA_DETTAGLI AS RUOLO_DETTAGLI  FROM  DPA_CORR_GLOBALI A,PEOPLEGROUPS B, DPA_CORR_GLOBALI C, DPA_TIPO_RUOLO D, PEOPLE E WHERE "+generalCondition_utente+" AND B.PEOPLE_SYSTEM_ID=A.ID_PEOPLE AND C.ID_GRUPPO=B.GROUPS_SYSTEM_ID AND D.SYSTEM_ID=C.ID_TIPO_RUOLO AND E.SYSTEM_ID=A.ID_PEOPLE ";
						if(objQueryCorrispondente.nomeUtente!=null)
						{
							commandString_utente=commandString_utente_begin+" AND E.VAR_NOME LIKE '"+objQueryCorrispondente.nomeUtente+"%' AND A.CHA_TIPO_URP='P' AND A.CHA_TIPO_IE='I' AND A.CHA_TIPO_CORR='S'";
						}
						else
						{
							commandString_utente=commandString_utente_begin+" AND E.VAR_COGNOME LIKE '"+objQueryCorrispondente.cognomeUtente+"%' AND A.CHA_TIPO_URP='P' AND A.CHA_TIPO_IE='I' AND A.CHA_TIPO_CORR='S'";
						}

						//controllo se la query ha condizioni sui ruoli
						if(objQueryCorrispondente.isRuoloDefined())
						{
						
							commandString_utente=commandString_utente+" AND D.VAR_DESC_RUOLO LIKE '%"+objQueryCorrispondente.descrizioneRuolo+"%'";
						}
						commandString1=commandString_utente;

					}
				
				}
				//Fabio
				if(objQueryCorrispondente.fineValidita)
					commandString1 += " AND A.DTA_FINE IS NULL";
				commandString1 = DocsPaWS.Utils.dbControl.selectTop(commandString1);
				logger.Debug(commandString1);
				
				database.fillTable(commandString1,dataSet,"CORRISPONDENTI");
				ArrayList list=new ArrayList();
				//logger.Debug(commandString1);
				/*DocsPaVO.utente.UnitaOrganizzativa prova=new DocsPaVO.utente.UnitaOrganizzativa();
				prova.descrizione=commandString1;
				list.Add(prova);*/
            //riempimento dell'oggetto finale
            /*foreach(DataRow corrRow in dataSet.Tables["CORRISPONDENTI"].Rows)
                {
                    //ricerca delle UO parent: viene riempita una tabella ottimizzata
                    string commandString2="";
                    if(corrRow["CHA_TIPO_URP"].ToString().Equals("U"))
                    {
                        commandString2="SELECT SYSTEM_ID, VAR_EMAIL, CHA_PA, ID_REGISTRO, VAR_DESC_CORR, NUM_LIVELLO, CHA_DETTAGLI, VAR_CODICE, VAR_COD_RUBRICA, VAR_CODICE_AMM, VAR_CODICE_ISTAT, VAR_CODICE_AOO, VAR_SMTP, NUM_PORTA_SMTP, ID_PARENT FROM DPA_CORR_GLOBALI A WHERE CHA_TIPO_URP='U' AND CHA_TIPO_IE='I' AND NUM_LIVELLO <="+corrRow["NUM_LIVELLO"].ToString();
                    };
                    if(corrRow["CHA_TIPO_URP"].ToString().Equals("R"))
                    {
                        commandString2="SELECT A.SYSTEM_ID, A.VAR_EMAIL, A.CHA_PA, A.ID_REGISTRO, A.VAR_DESC_CORR, A.NUM_LIVELLO, A.CHA_DETTAGLI, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_CODICE_AMM, A.VAR_CODICE_ISTAT, A.VAR_CODICE_AOO, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_PARENT FROM DPA_CORR_GLOBALI A, DPA_CORR_GLOBALI B WHERE A.CHA_TIPO_URP='U' AND A.CHA_TIPO_IE='I' AND B.SYSTEM_ID='"+corrRow["ID_UO"].ToString()+"' AND A.NUM_LIVELLO <= B.NUM_LIVELLO";
                    };
                    if(corrRow["CHA_TIPO_URP"].ToString().Equals("P"))
                    {
                        commandString2="SELECT A.SYSTEM_ID, A.VAR_EMAIL, A.CHA_PA, A.ID_REGISTRO, A.VAR_DESC_CORR, A.NUM_LIVELLO, A.CHA_DETTAGLI, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_CODICE_AMM, A.VAR_CODICE_ISTAT, A.VAR_CODICE_AOO, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_PARENT FROM DPA_CORR_GLOBALI A, DPA_CORR_GLOBALI B WHERE A.CHA_TIPO_URP='U' AND A.CHA_TIPO_IE='I' AND B.SYSTEM_ID='"+corrRow["RUOLO_ID_UO"].ToString()+"' AND A.NUM_LIVELLO <= B.NUM_LIVELLO";
                    };
                    logger.Debug(commandString2);
                    database.fillTable(commandString2,dataSet,"UO");

                    obj.ricercaUOParentInt(out dataSet,corrRow);

                    //this.ExecuteQuery(out dataSet,"UO",commandString2);

                    //l'oggetto viene riempito in base alla sua tipologia
                    if(corrRow["CHA_TIPO_URP"].ToString().Equals("U"))
                    {
                                            //si verifica se è replicata nella lista
                        int replies=0;
                        for(int i=0;i<list.Count;i++)
                        {
                            if(((DocsPaVO.utente.Corrispondente)list[i]).systemId.Equals(corrRow["SYSTEM_ID"].ToString())) replies=replies+1;
                        }
                        if(replies==0)
                        {
                            DocsPaVO.utente.UnitaOrganizzativa corrispondenteUO= new DocsPaVO.utente.UnitaOrganizzativa();
                            corrispondenteUO.systemId= corrRow["SYSTEM_ID"].ToString();
                            corrispondenteUO.descrizione=corrRow["VAR_DESC_CORR"].ToString();
                            corrispondenteUO.codiceCorrispondente=corrRow["VAR_CODICE"].ToString();
                            corrispondenteUO.codiceRubrica=corrRow["VAR_COD_RUBRICA"].ToString();
                            if(corrRow["ID_REGISTRO"]!=null)
                            {
                                corrispondenteUO.idRegistro=corrRow["ID_REGISTRO"].ToString();
                            }
                            corrispondenteUO.email=corrRow["VAR_EMAIL"].ToString();
                            corrispondenteUO.interoperante=fromCharToBool(corrRow["CHA_PA"].ToString());
                            corrispondenteUO.dettagli=fromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                            corrispondenteUO.livello=corrRow["NUM_LIVELLO"].ToString();
                            DocsPaVO.utente.ServerPosta sp=new DocsPaVO.utente.ServerPosta();
                            sp.serverSMTP=corrRow["VAR_SMTP"].ToString();
                            sp.portaSMTP=corrRow["NUM_PORTA_SMTP"].ToString();
                            corrispondenteUO.serverPosta=sp;
                            corrispondenteUO.idAmministrazione=corrRow["ID_AMM"].ToString();
                            corrispondenteUO.codiceAOO=corrRow["VAR_CODICE_AOO"].ToString();
                            corrispondenteUO.codiceAmm=corrRow["VAR_CODICE_AMM"].ToString();
                            corrispondenteUO.codiceIstat=corrRow["VAR_CODICE_ISTAT"].ToString();
                            corrispondenteUO.tipoIE="I";
                            //qui si ritrova la parentela
                            if(!corrRow["ID_PARENT"].ToString().Equals("0")) 
                            {
                                corrispondenteUO.parent=getParents(corrRow["ID_PARENT"].ToString(),dataSet.Tables["UO"]);
                            }
                            list.Add(corrispondenteUO);
                        }
                    }
                    if(corrRow["CHA_TIPO_URP"].ToString().Equals("R"))
                    {
                        DocsPaVO.utente.Ruolo corrispondenteRuolo= new DocsPaVO.utente.Ruolo();
                        corrispondenteRuolo.systemId= corrRow["SYSTEM_ID"].ToString();
                        corrispondenteRuolo.descrizione=corrRow["VAR_DESC_RUOLO"].ToString();
                        corrispondenteRuolo.codiceCorrispondente=corrRow["VAR_CODICE"].ToString();
                        corrispondenteRuolo.codiceRubrica=corrRow["VAR_COD_RUBRICA"].ToString();
                        corrispondenteRuolo.idAmministrazione=corrRow["ID_AMM"].ToString();
                        corrispondenteRuolo.livello=corrRow["RUOLO_LIVELLO"].ToString();
                        corrispondenteRuolo.idGruppo=corrRow["ID_GRUPPO"].ToString();
                        DocsPaVO.utente.ServerPosta sp=new DocsPaVO.utente.ServerPosta();
                        sp.serverSMTP=corrRow["VAR_SMTP"].ToString();
                        sp.portaSMTP=corrRow["NUM_PORTA_SMTP"].ToString();
                        corrispondenteRuolo.serverPosta=sp;
                        corrispondenteRuolo.tipoIE="I";
                        if(corrRow["ID_REGISTRO"]!=null)
                        {
                            corrispondenteRuolo.idRegistro=corrRow["ID_REGISTRO"].ToString();
                        }
                        corrispondenteRuolo.dettagli=fromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                        //qui si ritrova la parentela (con filtro o no)
                        if(objQueryCorrispondente.isUODefined())
                        {
                            if((objQueryCorrispondente.descrizioneUO!=null && hasDefinedUo(objQueryCorrispondente.descrizioneUO,1,corrRow["ID_UO"].ToString(),dataSet.Tables["UO"]))||(objQueryCorrispondente.codiceUO!=null && hasDefinedUo(objQueryCorrispondente.codiceUO,2,corrRow["ID_UO"].ToString(),dataSet.Tables["UO"])))
                            {
                                corrispondenteRuolo.uo=getParents(corrRow["ID_UO"].ToString(),dataSet.Tables["UO"]);
                                list.Add(corrispondenteRuolo);
                            }
                        }
                        else
                        {
                            corrispondenteRuolo.uo=getParents(corrRow["ID_UO"].ToString(),dataSet.Tables["UO"]);
                            list.Add(corrispondenteRuolo);
                        }
                    }
                    if(corrRow["CHA_TIPO_URP"].ToString().Equals("P"))
                    {
                        DocsPaVO.utente.Utente corrispondenteUtente= new DocsPaVO.utente.Utente();
                        corrispondenteUtente.systemId= corrRow["SYSTEM_ID"].ToString();
                        corrispondenteUtente.idPeople= corrRow["ID_PEOPLE"].ToString();
                        corrispondenteUtente.descrizione=corrRow["VAR_COGNOME"].ToString()+" "+corrRow["VAR_NOME"].ToString();
                        corrispondenteUtente.codiceCorrispondente=corrRow["VAR_CODICE"].ToString();
                        corrispondenteUtente.codiceRubrica=corrRow["VAR_COD_RUBRICA"].ToString();
                        corrispondenteUtente.idAmministrazione=corrRow["ID_AMM"].ToString();
                        if(corrRow["ID_REGISTRO"]!=null)
                        {
                            corrispondenteUtente.idRegistro=corrRow["ID_REGISTRO"].ToString();
                        }
                        corrispondenteUtente.dettagli=fromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                        DocsPaVO.utente.ServerPosta sp=new DocsPaVO.utente.ServerPosta();
                        sp.serverSMTP=corrRow["VAR_SMTP"].ToString();
                        sp.portaSMTP=corrRow["NUM_PORTA_SMTP"].ToString();
                        corrispondenteUtente.serverPosta=sp;
                        corrispondenteUtente.email=corrRow["EMAIL_ADDRESS"].ToString();
                        corrispondenteUtente.notifica=corrRow["CHA_NOTIFICA"].ToString();
                        corrispondenteUtente.telefono=corrRow["VAR_TELEFONO"].ToString();
                        corrispondenteUtente.tipoIE="I";
                        //si trova il ruolo corrispondente
                        DocsPaVO.utente.Ruolo ruoloUtente=new DocsPaVO.utente.Ruolo();
                        ruoloUtente.systemId=corrRow["RUOLO_SYSTEM_ID"].ToString();
                        ruoloUtente.descrizione=corrRow["RUOLO_DESC"].ToString();
                        ruoloUtente.codiceCorrispondente=corrRow["RUOLO_CODICE"].ToString();
                        ruoloUtente.codiceRubrica=corrRow["RUOLO_COD_RUBRICA"].ToString();
                        ruoloUtente.dettagli=fromCharToBool(corrRow["RUOLO_DETTAGLI"].ToString());
                        //qui si trova la parentela (con filtro o no)
                        if(objQueryCorrispondente.isUODefined())
                        {
                            if((objQueryCorrispondente.descrizioneUO!=null && hasDefinedUo(objQueryCorrispondente.descrizioneUO,1,corrRow["RUOLO_ID_UO"].ToString(),dataSet.Tables["UO"]))||(objQueryCorrispondente.codiceUO!=null && hasDefinedUo(objQueryCorrispondente.codiceUO,2,corrRow["RUOLO_ID_UO"].ToString(),dataSet.Tables["UO"])))
                            {
                                ruoloUtente.uo=getParents(corrRow["RUOLO_ID_UO"].ToString(),dataSet.Tables["UO"]);
						
                                ArrayList ruoli=new ArrayList();
                                ruoli.Add(ruoloUtente);
                                corrispondenteUtente.ruoli=ruoli;
                                list.Add(corrispondenteUtente);
                            }
                        }
                        else
                        {
                            ruoloUtente.uo=getParents(corrRow["RUOLO_ID_UO"].ToString(),dataSet.Tables["UO"]);
						
                            ArrayList ruoli=new ArrayList();
                            ruoli.Add(ruoloUtente);
                            corrispondenteUtente.ruoli=ruoli;
                            list.Add(corrispondenteUtente);
                        }	
						
                    }
                }
                listaCorr=list;
				
                database.closeConnection();
			 
            }
            catch(Exception e)
            {
                logger.Debug("error: listaCorrispondentiIntMethod");
				
                database.closeConnection();
                throw e;}
            return listaCorr;*/
            #endregion
        }


        //METODO PER GLI UTENTI INTERNI UTILIZZATO NELLA VERIFICA CASELLA ISTITUZIONALE PER IL
        //REPERIMENTO DEL MITTENTE DEL PROTOCOLLO IN INGRESSO, RICEVUTO PER INTEROPERABILITA'

        public static ArrayList listaCorrispondentiIntInteropMethod(DocsPaVO.addressbook.QueryCorrispondente qco)
        {
            ArrayList list = new ArrayList();
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            list = utenti.ListaCorrispondentiIntInterop(qco);
            if (list == null)
            {
                logger.Debug("Errore nella gestione degli utenti (listaCorrispondentiIntInteropMethod)");
                throw new Exception();
            }
            return list;

        }

        //METODO PER LA RICERCA DEI CORRISPONDENTI INTERNI CON ALTRE AUTORIZZAZIONI
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objQueryCorrAutorizzato"></param>
        /// <returns></returns>
        public static ArrayList listaCorrispondentiAutMethod(DocsPaVO.addressbook.QueryCorrispondenteAutorizzato objQueryCorrAutorizzato)
        {
            DocsPaVO.utente.Ruolo ruolo = objQueryCorrAutorizzato.ruolo;
            DocsPaVO.addressbook.QueryCorrispondente objQueryCorrispondente = objQueryCorrAutorizzato.queryCorrispondente;
            //ricerca di UO per la trasmissione ad UO - TODO: aggiungere filtro sul livello della UO
            if ((objQueryCorrispondente.codiceUO != null && !objQueryCorrispondente.codiceUO.Equals("")) || (objQueryCorrispondente.descrizioneUO != null && !objQueryCorrispondente.descrizioneUO.Equals("")))
                return getUOAutorizzate(ruolo, objQueryCorrAutorizzato);

            DocsPaVO.trasmissione.RagioneTrasmissione ragione = objQueryCorrAutorizzato.ragione;
            string idRegistro = objQueryCorrAutorizzato.idRegistro;
            string idNodoTitolario = objQueryCorrAutorizzato.idNodoTitolario;
            DocsPaVO.trasmissione.TipoOggetto tipoOggetto = objQueryCorrAutorizzato.tipoOggetto;

            //oggetto di ritorno
            ArrayList listaCorrAut = new ArrayList();

            //ricerca di tutti i ruoli autorizzati
            ArrayList ruoliAut = new ArrayList();
            switch (ragione.tipoDestinatario)
            {
                case DocsPaVO.trasmissione.TipoGerarchia.TUTTI:
                    {//tutti i destinatari
                        logger.Debug("TUTTI idReg=" + idRegistro);
                        DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();
                        ruoliAut = gerarchia.getRuoliAut(ruolo, idRegistro, idNodoTitolario, tipoOggetto);
                        break;
                    }
                case DocsPaVO.trasmissione.TipoGerarchia.SUPERIORE:
                    {
                        //solo i superiori
                        logger.Debug("SUP");
                        DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();
                        ruoliAut = gerarchia.getGerarchiaSup(ruolo, idRegistro, idNodoTitolario, tipoOggetto);
                        break;
                    }
                case DocsPaVO.trasmissione.TipoGerarchia.INFERIORE:
                    {
                        //solo gli inferiori
                        logger.Debug("INF");
                        DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();
                        ruoliAut = gerarchia.getGerarchiaInf(ruolo, idRegistro, idNodoTitolario, tipoOggetto);
                        break;
                    }
                case DocsPaVO.trasmissione.TipoGerarchia.PARILIVELLO:
                    {
                        //solo i pari livello (UO dello stesso livello (per ora anche di alberi diversi) e tipoRuolo dello stesso livello)
                        logger.Debug("PARI_LIVELLO");
                        DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();
                        ruoliAut = gerarchia.getGerarchiaPariLiv(ruolo, idRegistro, idNodoTitolario, tipoOggetto);
                        break;
                    }
                default:
                    {
                        logger.Debug("ERRATA TIPO GERARCHIA");
                        return null;
                    }
            }
            if (ruoliAut == null)
            {
                logger.Debug("Errore nella gestione degli utenti (listaCorrispondentiAutMethod)");
                throw new Exception("Errore in : listaCorrispondentiAutMethod");
            }

            //			if (ruoliAut!= null && ruoliAut.Count <1)
            //			{
            //				return listaCorrAut;
            //			}


            //18/10/2005 elisa 
            //se è una trasmissione
            //visualizza solo i ruoli associati al registro corrente
            ArrayList listaCorr = new ArrayList();
            if (objQueryCorrAutorizzato.isProtoInterno)
            {
                listaCorr = listaCorrispondentiInt_Aut_Method(objQueryCorrispondente);
            }
            else
            {
                listaCorr = listaCorrispondentiIntMethod(objQueryCorrispondente);
            }
            logger.Debug("QUI SI ENTRA " + listaCorr.Count + " " + ruoliAut.Count);


            for (int i = 0; i < listaCorr.Count; i++)
            {
                //se il corrispondente è un ruolo
                if (listaCorr[i].GetType().Equals(typeof(DocsPaVO.utente.Ruolo)))
                {
                    DocsPaVO.utente.Ruolo currRuolo = (DocsPaVO.utente.Ruolo)listaCorr[i];
                    for (int j = 0; j < ruoliAut.Count; j++)
                    {
                        DocsPaVO.utente.Ruolo currRuoloAut = (DocsPaVO.utente.Ruolo)ruoliAut[j];
                        if (currRuolo.systemId.Equals(currRuoloAut.systemId))
                        {
                            logger.Debug("ruolo aggiunto:" + currRuolo.systemId);
                            listaCorrAut.Add(currRuolo);
                        }
                    }
                }

                //se il corrispondente è un utente
                if (listaCorr[i].GetType().Equals(typeof(DocsPaVO.utente.Utente)))
                {
                    logger.Debug("UTENTE...");
                    DocsPaVO.utente.Utente currUtente = (DocsPaVO.utente.Utente)listaCorr[i];
                    DocsPaVO.utente.Ruolo currRuolo = (DocsPaVO.utente.Ruolo)currUtente.ruoli[0];
                    logger.Debug("RUOLO: " + currRuolo.systemId);
                    for (int j = 0; j < ruoliAut.Count; j++)
                    {
                        DocsPaVO.utente.Ruolo currRuoloAut = (DocsPaVO.utente.Ruolo)ruoliAut[j];
                        if (currRuolo.systemId.Equals(currRuoloAut.systemId))
                        {
                            listaCorrAut.Add(currUtente);
                        }
                    }
                }

            }
            return listaCorrAut;
        }


        //METODO PER LA RICERCA DELLE UO INTERNE e GERARCHICAMENTE VALIDE 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruolo"></param>
        /// <param name="objQueryCorrAutorizzato"></param>
        /// <returns></returns>
        public static System.Collections.ArrayList getUOAutorizzate(DocsPaVO.utente.Ruolo ruolo, DocsPaVO.addressbook.QueryCorrispondenteAutorizzato objQueryCorrAutorizzato)
        {
            //ricerca di tutti i ruoli autorizzati
            ArrayList UOAut = new ArrayList();
            ArrayList listaUO = new ArrayList(); //per la gerarchia
            switch (objQueryCorrAutorizzato.ragione.tipoDestinatario)
            {
                case DocsPaVO.trasmissione.TipoGerarchia.INFERIORE:
                    {//le UO gerarchicamente inferiori
                        DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();
                        listaUO = gerarchia.getChildrenUO(ruolo);
                        listaUO.Add(ruolo.uo.systemId);
                        break;
                    }
                case DocsPaVO.trasmissione.TipoGerarchia.SUPERIORE:
                    {//le UO gerarchicamente superiori
                        DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();
                        listaUO = gerarchia.getParentUO(ruolo);
                        listaUO.Add(ruolo.uo.systemId);
                        break;
                    }
                case DocsPaVO.trasmissione.TipoGerarchia.PARILIVELLO:
                    {//le UO gerarchicamente di pari livello
                        DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();
                        listaUO = gerarchia.getPariUO(ruolo);
                        listaUO.Add(ruolo.uo.systemId);
                        break;
                    }
            }
            //cerca le uo che soddisfano i criteri di ricerca e sono gerarchicamente valide
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            //17/10/2005 elisa
            //se è una trasmissione per protocollo interno
            //visualizza solo le UO i cui ruoli sono associati al registro corrente
            if (objQueryCorrAutorizzato.isProtoInterno)
            {
                UOAut = utenti.ListaUOAutInterne(objQueryCorrAutorizzato.queryCorrispondente, listaUO);
            }
            else
            {
                UOAut = utenti.ListaUOAut(objQueryCorrAutorizzato.queryCorrispondente, listaUO);
            }

            return UOAut;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="qco"></param>
        /// <returns></returns>
        internal static ArrayList listaUtentiScioltiMethod(DocsPaVO.addressbook.QueryCorrispondente qco)
        {
            System.Collections.ArrayList listaCorr = new System.Collections.ArrayList();
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            listaCorr = utenti.ListaUtentiSciolti(qco);
            if (listaCorr == null)
            {
                logger.Debug("Errore nella gestione degli utenti (listaUtentiScioltiMethod)");
                throw new Exception();
            }
            return listaCorr;

            #region Codice Commentato
            /*
			DocsPa_V15_Utils.Database database=DocsPaWS.Utils.dbControl.getDatabase();
			System.Data.DataSet dataSet=new System.Data.DataSet();
			System.Collections.ArrayList listaCorr=new System.Collections.ArrayList();
			DocsPaVO.addressbook.QueryCorrispondente objQueryCorrispondente=correctApiciQuery(qco);
			try
			{
				if(objQueryCorrispondente.isUODefined()||objQueryCorrispondente.isRuoloDefined()) return listaCorr;
				string generalCondition="((ID_AMM IS NULL) OR (ID_REGISTRO IS NULL AND ID_AMM='"+objQueryCorrispondente.idAmministrazione+"')";
				generalCondition=generalCondition+registriCondition(objQueryCorrispondente,null)+") ";
				if(objQueryCorrispondente.codiceRubrica!=null)
				{
					string generalConditionCR=" ((ID_AMM IS NULL) OR (ID_AMM='"+objQueryCorrispondente.idAmministrazione+"'))";

					string parentString="SELECT * FROM DPA_CORR_GLOBALI A WHERE VAR_COD_RUBRICA='"+objQueryCorrispondente.codiceRubrica+"' and cha_tipo_ie='E' AND CHA_TIPO_CORR='S' AND "+generalConditionCR;
					
					// TODO: Utilizzare il progetto DocsPaDbManagement
					database.fillTable(parentString,dataSet,"PARENT");
					    
					//nel caso non esiste oggetto parent con tale codice rubrica
					if(dataSet.Tables["PARENT"].Rows.Count==0) return listaCorr;
					
					if(objQueryCorrispondente.getChildren==false)
					{
						if(dataSet.Tables["PARENT"].Rows[0]["CHA_TIPO_URP"].Equals("P"))
						{
							//è un utente:
							commandString1="SELECT A.SYSTEM_ID, A.VAR_COGNOME, A.VAR_NOME, A.VAR_DESC_CORR, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.CHA_DETTAGLI, A.CHA_TIPO_URP, A.ID_REGISTRO, A.VAR_EMAIL, A.VAR_SMTP, A.NUM_PORTA_SMTP FROM  DPA_CORR_GLOBALI A WHERE A.VAR_COD_RUBRICA='"+objQueryCorrispondente.codiceRubrica+"'";
							commandString1=commandString1+" AND ((A.ID_AMM IS NULL) OR (A.ID_AMM='"+objQueryCorrispondente.idAmministrazione+"')) AND A.CHA_TIPO_CORR='S'";
						}
						else
						{
							return listaCorr;
						}
					}
					else
					{
						//non ci possono essere figli
						return listaCorr;
					}

				}
				else
				{
					//la query viene fatta in base all'UO, al ruolo e all'utente
					string commandString_begin="SELECT * FROM DPA_CORR_GLOBALI A WHERE "+generalCondition;
					string commandString_utente=null;

					string generalCondition_utente="((A.ID_AMM IS NULL) OR (A.ID_REGISTRO IS NULL AND A.ID_AMM='"+objQueryCorrispondente.idAmministrazione+"')"+registriCondition(objQueryCorrispondente,"A")+")";

					string commandString_utente_begin="SELECT A.SYSTEM_ID, A.VAR_NOME, A.VAR_COGNOME, A.VAR_DESC_CORR, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.CHA_DETTAGLI, A.CHA_TIPO_URP, A.ID_REGISTRO, A.VAR_EMAIL, A.VAR_SMTP, A.NUM_PORTA_SMTP FROM  DPA_CORR_GLOBALI A WHERE "+generalCondition_utente;
					if(objQueryCorrispondente.nomeUtente!=null)
					{
						commandString_utente=commandString_utente_begin+" AND A.VAR_NOME LIKE '"+objQueryCorrispondente.nomeUtente+"%' AND A.CHA_TIPO_URP='P' AND A.CHA_TIPO_IE='E' AND A.CHA_TIPO_CORR='S'";
					}
					else
					{
						commandString_utente=commandString_utente_begin+" AND A.VAR_COGNOME LIKE '"+objQueryCorrispondente.cognomeUtente+"%' AND A.CHA_TIPO_URP='P' AND A.CHA_TIPO_IE='E' AND A.CHA_TIPO_CORR='S'";
					}

					commandString1=commandString_utente;

				}
				
				//Fabio
				if(objQueryCorrispondente.fineValidita)
					commandString1 += " AND A.DTA_FINE IS NULL";

				logger.Debug(commandString1);
				
				// TODO: Utilizzare il progetto DocsPaDbManagement
				database.fillTable(commandString1,dataSet,"CORRISPONDENTI");

				

				foreach(DataRow corrRow in dataSet.Tables["CORRISPONDENTI"].Rows)
				{
					if(corrRow["CHA_TIPO_URP"].ToString().Equals("P"))
					{
						DocsPaVO.utente.Utente corrispondenteUtente=new DocsPaVO.utente.Utente();
						corrispondenteUtente.systemId= corrRow["SYSTEM_ID"].ToString();
						corrispondenteUtente.descrizione=corrRow["VAR_COGNOME"].ToString()+" "+corrRow["VAR_NOME"].ToString();
						corrispondenteUtente.codiceCorrispondente=corrRow["VAR_CODICE"].ToString();
						corrispondenteUtente.codiceRubrica=corrRow["VAR_COD_RUBRICA"].ToString();
						corrispondenteUtente.dettagli=fromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
						DocsPaVO.utente.ServerPosta sp=new DocsPaVO.utente.ServerPosta();
						sp.serverSMTP=corrRow["VAR_SMTP"].ToString();
						sp.portaSMTP=corrRow["NUM_PORTA_SMTP"].ToString();
						corrispondenteUtente.serverPosta=sp;
						corrispondenteUtente.tipoIE="E";
						if(corrRow["ID_REGISTRO"]!=null)
						{
							corrispondenteUtente.idRegistro=corrRow["ID_REGISTRO"].ToString();
						}
						corrispondenteUtente.email=corrRow["VAR_EMAIL"].ToString();
						listaCorr.Add(corrispondenteUtente);
					}
				}
			}
			catch(Exception e)
			{
				logger.Debug("error: listaUtentiScioltiMethod");
				
				// TODO: Utilizzare il progetto DocsPaDbManagement
				database.closeConnection();
				throw e;
			}
			return listaCorr;*/
            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qco"></param>
        /// <returns></returns>
        public static ArrayList listaCorrEstSciolti(DocsPaVO.addressbook.QueryCorrispondente qco)
        {
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            ArrayList lista = utenti.ListaCorrEstSciolti(qco);
            if (lista == null)
            {
                logger.Debug("Errore nella gestione degli utenti (listaCorrEstSciolti)");
                throw new Exception();
            }
            return lista;

            #region Codice Commentato
            /*logger.Debug("listaCorrEstSciolti");
			ArrayList listaCorr = listaCorrispondentiEstMethod(qco);
			ArrayList listaSciolti = listaUtentiScioltiMethod(qco);
			ArrayList temp = new ArrayList();
			for(int i=0;i<listaSciolti.Count;i++)
			{
				DocsPaVO.utente.Utente ut=(DocsPaVO.utente.Utente) listaSciolti[i];
				logger.Debug("Utente sciolto: "+ut.systemId);
				bool isInEst=false;
				for(int k=0;k<listaCorr.Count;k++)
				{
					if(ut.systemId.Equals(((DocsPaVO.utente.Corrispondente)listaCorr[k]).systemId)) isInEst=true;
				}
				if(!isInEst) temp.Add(ut);
			}
			for(int j=0;j<temp.Count;j++)
			{
				logger.Debug("Aggiunto utente sciolto");
				listaCorr.Add(temp[j]);
			}
			return listaCorr;*/
            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objQueryCorrispondente"></param>
        /// <returns></returns>
        public static ArrayList getRootUO(DocsPaVO.addressbook.QueryCorrispondente objQueryCorrispondente)
        {
            //DocsPa_V15_Utils.Database database=DocsPaWS.Utils.dbControl.getDatabase();
            System.Data.DataSet ds = new System.Data.DataSet();
            System.Collections.ArrayList listUO = new System.Collections.ArrayList();
            try
            {
                #region Codice Commentato
                // TODO: Utilizzare il progetto DocsPaDbManagement
                //database.openConnection();
                /*string generalCondition="((ID_AMM IS NULL) OR (ID_REGISTRO IS NULL AND ID_AMM='"+objQueryCorrispondente.idAmministrazione+"')";
                generalCondition=generalCondition+registriCondition(objQueryCorrispondente,null)+") ";
                string queryUO="SELECT * FROM DPA_CORR_GLOBALI A WHERE A.DTA_FINE IS NULL AND CHA_TIPO_URP='U' AND NUM_LIVELLO='0' ";
                if(objQueryCorrispondente.tipoUtente==DocsPaVO.addressbook.TipoUtente.ESTERNO)
                {
                    queryUO=queryUO+" AND CHA_TIPO_IE='E'";
                };
                if(objQueryCorrispondente.tipoUtente==DocsPaVO.addressbook.TipoUtente.INTERNO)
                {
                    queryUO=queryUO+" AND CHA_TIPO_IE='I'";
                };
                queryUO=queryUO+" AND "+generalCondition;
                logger.Debug(queryUO);
				
                // TODO: Utilizzare il progetto DocsPaDbManagement
                database.fillTable(queryUO,ds,"UO");*/
                #endregion

                DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                utenti.GetRootUO(out ds, objQueryCorrispondente);

                foreach (DataRow dr in ds.Tables["UO"].Rows)
                {
                    DocsPaVO.utente.UnitaOrganizzativa uo = new DocsPaVO.utente.UnitaOrganizzativa();
                    uo.systemId = dr["SYSTEM_ID"].ToString();
                    uo.descrizione = dr["VAR_DESC_CORR"].ToString();
                    uo.codiceCorrispondente = dr["VAR_CODICE"].ToString();
                    uo.codiceRubrica = dr["VAR_COD_RUBRICA"].ToString();
                    if (dr["ID_REGISTRO"] != null)
                    {
                        uo.idRegistro = dr["ID_REGISTRO"].ToString();
                    };
                    if (dr["ID_AMM"] != null)
                    {
                        uo.idAmministrazione = dr["ID_AMM"].ToString();
                    }
                    uo.email = dr["VAR_EMAIL"].ToString();
                    uo.interoperante = fromCharToBool(dr["CHA_PA"].ToString());
                    uo.dettagli = fromCharToBool(dr["CHA_DETTAGLI"].ToString());
                    DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                    sp.serverSMTP = dr["VAR_SMTP"].ToString();
                    sp.portaSMTP = dr["NUM_PORTA_SMTP"].ToString();
                    uo.serverPosta = sp;
                    uo.livello = dr["NUM_LIVELLO"].ToString();
                    listUO.Add(uo);
                }
                // TODO: Utilizzare il progetto DocsPaDbManagement
                //database.closeConnection();			    
            }
            catch (Exception e)
            {
                // TODO: Utilizzare il progetto DocsPaDbManagement
                //database.closeConnection();
                logger.Debug("Errore nella gestione degli utenti (getRootUO)", e);
                throw e;
            }
            return listUO;
        }

        internal static ArrayList getCorrGruppiMethod(DocsPaVO.addressbook.QueryCorrispondente qco)
        {
            ArrayList result = new ArrayList();
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            result = utenti.GetCorrGruppi(qco);
            if (result == null)
            {
                logger.Debug("Errore nella gestione degli utenti (getCorrGruppoMethod)");
                throw new Exception();
            }
            return result;

            #region Codice Commentato
            /*logger.Debug("getCorrGruppiMethod");
			DocsPa_V15_Utils.Database database=DocsPaWS.Utils.dbControl.getDatabase();
			DocsPaVO.addressbook.QueryCorrispondente objQuery=correctApiciQuery(qco);
			ArrayList result=new ArrayList();
			System.Collections.ArrayList lista=new System.Collections.ArrayList();
			
			// TODO: Utilizzare il progetto DocsPaDbManagement
			database.openConnection();
			DataSet dataSet= new DataSet();
			try
			{
				if(objQuery.codiceRubrica!=null)
				{
					if(objQuery.getChildren)
					{
						queryString= getQueryChildrenGruppo(objQuery.codiceRubrica,database);
					}
					else
					{
						queryString="SELECT SYSTEM_ID, ID_REGISTRO, VAR_EMAIL, CHA_PA, VAR_DESC_CORR, VAR_CODICE, VAR_COD_RUBRICA, VAR_SMTP, NUM_PORTA_SMTP, ID_AMM FROM DPA_CORR_GLOBALI A WHERE CHA_TIPO_CORR='G' AND VAR_COD_RUBRICA='"+objQuery.codiceRubrica+"'";
					}
				}
				else
				{
					queryString="SELECT SYSTEM_ID, ID_REGISTRO, VAR_EMAIL, NUM_LIVELLO, CHA_PA, VAR_DESC_CORR, VAR_CODICE, VAR_COD_RUBRICA, VAR_SMTP, NUM_PORTA_SMTP, ID_AMM FROM DPA_CORR_GLOBALI A WHERE CHA_TIPO_CORR='G' ";
					if(objQuery.codiceGruppo != null)
					{
						queryString=queryString+" AND VAR_CODICE='"+objQuery.codiceGruppo+"'";
					}
					else
					{
						queryString=queryString+" AND VAR_DESC_CORR LIKE '%"+objQuery.descrizioneGruppo+"%'";
					};	
					if(objQuery.tipoUtente==DocsPaVO.addressbook.TipoUtente.ESTERNO)
					{
						queryString=queryString+" AND CHA_TIPO_IE='E'";
					};
					if(objQuery.tipoUtente==DocsPaVO.addressbook.TipoUtente.INTERNO)
					{
						queryString=queryString+" AND CHA_TIPO_IE='I'";
					};	
				}
				logger.Debug(queryString);
				
				// TODO: Utilizzare il progetto DocsPaDbManagement
				database.fillTable(queryString,dataSet,"CORR_GRUPPI");
				
				
				if(objQuery.codiceRubrica!=null && objQuery.getChildren)
				{
					//nel caso in cui si ottengono gli appartenenti al gruppo, che sono utente o UO
					foreach(DataRow dr in dataSet.Tables["CORR_GRUPPI"].Rows)
					{
						if(dr["CHA_TIPO_URP"].Equals("U"))
						{
							DocsPaVO.utente.UnitaOrganizzativa uo=new DocsPaVO.utente.UnitaOrganizzativa();
							uo.codiceCorrispondente=dr["VAR_CODICE"].ToString();
							uo.codiceRubrica=dr["VAR_COD_RUBRICA"].ToString();
							uo.descrizione=dr["VAR_DESC_CORR"].ToString();
							uo.idAmministrazione=dr["ID_AMM"].ToString();
							if(dr["ID_REGISTRO"]!=null)
							{
								uo.idRegistro=dr["ID_REGISTRO"].ToString();
							}
							uo.email=dr["VAR_EMAIL"].ToString();
							uo.interoperante=fromCharToBool(dr["CHA_PA"].ToString());
							uo.systemId=dr["SYSTEM_ID"].ToString();
							DocsPaVO.utente.ServerPosta sp=new DocsPaVO.utente.ServerPosta();
							sp.serverSMTP=dr["VAR_SMTP"].ToString();
							sp.portaSMTP=dr["NUM_PORTA_SMTP"].ToString();
							uo.serverPosta=sp;
							uo.livello=dr["NUM_LIVELLO"].ToString();
							lista.Add(uo);
						}
						if(dr["CHA_TIPO_URP"].Equals("P"))
						{
							DocsPaVO.utente.Utente ut=new DocsPaVO.utente.Utente();
							if(dr["CHA_TIPO_IE"].Equals("E"))
							{
								ut.systemId= dr["SYSTEM_ID"].ToString();
								ut.descrizione=dr["VAR_COGNOME"].ToString()+" "+dr["VAR_NOME"].ToString();
								ut.codiceCorrispondente=dr["VAR_CODICE"].ToString();
								ut.codiceRubrica=dr["VAR_COD_RUBRICA"].ToString();
								ut.dettagli=fromCharToBool(dr["CHA_DETTAGLI"].ToString());
								if(dr["ID_REGISTRO"]!=null)
								{
									ut.idRegistro=dr["ID_REGISTRO"].ToString();
								}
								ut.email=dr["VAR_EMAIL"].ToString();
							}
							else
							{
								ut.systemId= dr["SYSTEM_ID"].ToString();
								ut.idPeople= dr["ID_PEOPLE"].ToString();
								if(dr["ID_REGISTRO"]!=null)
								{
									ut.idRegistro= dr["ID_REGISTRO"].ToString();
								}
								ut.descrizione=dr["VAR_COGNOME"].ToString()+" "+dr["VAR_NOME"].ToString();
								ut.codiceCorrispondente=dr["VAR_CODICE"].ToString();
								ut.codiceRubrica=dr["VAR_COD_RUBRICA"].ToString();
								ut.dettagli=fromCharToBool(dr["CHA_DETTAGLI"].ToString());
								ut.email=dr["EMAIL_ADDRESS"].ToString();
								ut.notifica=dr["CHA_NOTIFICA"].ToString();
								ut.telefono=dr["VAR_TELEFONO"].ToString();
							}
							lista.Add(ut);
						}
					}
				}
				else
				{
					foreach(DataRow dr in dataSet.Tables["CORR_GRUPPI"].Rows)
					{
						DocsPaVO.utente.Gruppo gr=new DocsPaVO.utente.Gruppo();
						gr.codiceCorrispondente=dr["VAR_CODICE"].ToString();
						gr.codiceRubrica=dr["VAR_COD_RUBRICA"].ToString();
						gr.descrizione=dr["VAR_DESC_CORR"].ToString();
						gr.idAmministrazione=dr["ID_AMM"].ToString();
						gr.systemId=dr["SYSTEM_ID"].ToString();
						if(dr["ID_REGISTRO"]!=null)
						{
							gr.idRegistro=dr["ID_REGISTRO"].ToString();
						}
						lista.Add(gr);
					}
				}
				result=lista;
				
				// TODO: Utilizzare il progetto DocsPaDbManagement
				database.closeConnection();
			}
			catch(Exception e)
			{
				logger.Debug("eccezione: "+e);
				
				// TODO: Utilizzare il progetto DocsPaDbManagement
				database.closeConnection();
				throw e;
			}
			return result;*/
            #endregion
        }

        #region Metodo Commentato
        /*private static string getQueryChildrenGruppo(string codiceRubrica, DocsPa_V15_Utils.Database database) 
		{
			string queryString="";
			try
			{
				DataSet dataSet=new DataSet();
				string parentString="SELECT CHA_TIPO_URP, CHA_TIPO_IE FROM DPA_CORR_GLOBALI A WHERE VAR_COD_RUBRICA='"+codiceRubrica+"' AND CHA_TIPO_CORR='G'"; 
				
				// TODO: Utilizzare il progetto DocsPaDbManagement
				database.fillTable(parentString, dataSet, "PARENT");
				if(dataSet.Tables["PARENT"].Rows.Count==0) return parentString;
				if(dataSet.Tables["PARENT"].Rows[0]["CHA_TIPO_URP"].ToString().Equals("P"))
				{
					//il gruppo contiene utenti
					if(dataSet.Tables["PARENT"].Rows[0]["CHA_TIPO_IE"].ToString().Equals("E"))
					{
						queryString="SELECT A.SYSTEM_ID, A.CHA_TIPO_IE, A.ID_REGISTRO, A.VAR_EMAIL, A.VAR_COGNOME, A.VAR_NOME, A.VAR_DESC_CORR, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.CHA_DETTAGLI, A.CHA_TIPO_URP FROM  DPA_CORR_GLOBALI A,DPA_CORR_GRUPPO B, DPA_CORR_GLOBALI C ";
						queryString=queryString+" WHERE A.SYSTEM_ID=B.ID_COMP_GRUPPO AND C.SYSTEM_ID=B.ID_GRUPPO AND C.VAR_COD_RUBRICA='"+codiceRubrica+"'";
					}																												
					else
					{
						queryString="SELECT A.SYSTEM_ID, A.ID_PEOPLE, A.ID_REGISTRO, A.CHA_TIPO_IE, E.VAR_COGNOME, E.VAR_NOME, E.EMAIL_ADDRESS, E.CHA_NOTIFICA, E.VAR_TELEFONO, A.VAR_DESC_CORR, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.CHA_DETTAGLI, A.CHA_TIPO_URP";
						queryString=queryString+" FROM DPA_CORR_GLOBALI A , DPA_CORR_GLOBALI C, PEOPLE E, DPA_CORR_GRUPPO F WHERE A.ID_PEOPLE = E.SYSTEM_ID AND  A.SYSTEM_ID = F.ID_COMP_GRUPPO";
						queryString=queryString+" AND F.ID_GRUPPO = C.SYSTEM_ID AND C.CHA_TIPO_CORR = 'G' AND C.VAR_COD_RUBRICA = '"+codiceRubrica+"'";
					}
				}
				else
				{
					//il gruppo contiene UO
					queryString="SELECT A.SYSTEM_ID, A.CHA_TIPO_URP, A.ID_REGISTRO, A.NUM_LIVELLO, A.VAR_EMAIL, A.CHA_PA, A.VAR_DESC_CORR, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_AMM FROM DPA_CORR_GLOBALI A, DPA_CORR_GRUPPO B, DPA_CORR_GLOBALI C";
					queryString=queryString+" WHERE A.SYSTEM_ID=B.ID_COMP_GRUPPO AND C.SYSTEM_ID=B.ID_GRUPPO AND C.VAR_COD_RUBRICA='"+codiceRubrica+"' AND C.CHA_TIPO_CORR='G'";
				}
			}
			catch(Exception e){throw e;};
			return queryString;
		}*/
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objQueryCorrispondente"></param>
        /// <returns></returns>
        public static DocsPaVO.addressbook.DettagliCorrispondente dettagliCorrispondenteMethod(DocsPaVO.addressbook.QueryCorrispondente objQueryCorrispondente)
        {
            //DocsPa_V15_Utils.Database database=DocsPaWS.Utils.dbControl.getDatabase();

            // TODO: Utilizzare il progetto DocsPaDbManagement
            //database.openConnection();
            DataSet dataSet = new DataSet();

            DocsPaVO.addressbook.DettagliCorrispondente dettagliCorr = new DocsPaVO.addressbook.DettagliCorrispondente();
            try
            {
                logger.Debug("dettagliCorrispondenteMethod");

                #region Codice Commentato
                /*string commandString1="SELECT * FROM DPA_DETT_GLOBALI WHERE ID_CORR_GLOBALI="+objQueryCorrispondente.systemId;
				logger.Debug(commandString1);
				
				// TODO: Utilizzare il progetto DocsPaDbManagement
				database.fillTable(commandString1,dataSet,"DETTAGLI");*/
                #endregion

                DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                utenti.DettagliCorrispondenti(out dataSet, objQueryCorrispondente);

                //				dettagliCorr.Corrispondente.AddCorrispondenteRow("", "", "", "", "", "", "", "", "", "");

                if (dataSet.Tables["DETTAGLI"].Rows.Count > 0)
                {
                    DataRow dettagliRow = dataSet.Tables["DETTAGLI"].Rows[0];

                    dettagliCorr.Corrispondente.AddCorrispondenteRow(dettagliRow["VAR_INDIRIZZO"].ToString(),
                                                                     dettagliRow["VAR_CITTA"].ToString(),
                                                                     dettagliRow["VAR_CAP"].ToString(),
                                                                     dettagliRow["VAR_PROVINCIA"].ToString(),
                                                                     dettagliRow["VAR_NAZIONE"].ToString(),
                                                                     dettagliRow["VAR_TELEFONO"].ToString(),
                                                                     dettagliRow["VAR_TELEFONO2"].ToString(),
                                                                     dettagliRow["VAR_FAX"].ToString(),
                                                                     dettagliRow["VAR_COD_FISC"].ToString(),
                                                                     dettagliRow["VAR_NOTE"].ToString(),
                                                                     dettagliRow["VAR_LOCALITA"].ToString(),
                                                                     dettagliRow["VAR_LUOGO_NASCITA"].ToString(),
                                                                     dettagliRow["DTA_NASCITA"].ToString(),
                                                                     dettagliRow["VAR_TITOLO"].ToString(),
                                                                     dettagliRow["VAR_COD_PI"].ToString());

                }
                else
                {
                    dettagliCorr.Corrispondente.AddCorrispondenteRow("", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella gestione degli utenti (dettagliCorrispondenteMethod)", e);
                throw e;
            }
            return dettagliCorr;
        }

        //METODO PER L'INSERIMENTO IN RUBRICA DI UN CORRISPONDENTE
        public static DocsPaVO.utente.Corrispondente insertCorrispondente(DocsPaVO.utente.Corrispondente corr, DocsPaVO.utente.Corrispondente parent)
        {
            DocsPaVO.utente.Corrispondente risIns = null;

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

                risIns = new DocsPaVO.utente.Corrispondente();

                //Emanuela :aggiungo nel backend i controlli per codice fiscale e partita iva


                string messaggio = string.Empty;
                if (corr.dettagli && corr.tipoCorrispondente != null && corr.tipoCorrispondente.ToUpper() != "R")
                {
                    DocsPaVO.addressbook.DettagliCorrispondente dettagliCorrispondente = new DocsPaVO.addressbook.DettagliCorrispondente();
                    if (corr.tipoCorrispondente.ToUpper() == "U")
                        DocsPaUtils.Data.TypedDataSetManager.MakeTyped(((DocsPaVO.utente.UnitaOrganizzativa)corr).info, dettagliCorrispondente.Corrispondente.DataSet);
                    else if (corr.tipoCorrispondente.ToUpper() == "P")
                        DocsPaUtils.Data.TypedDataSetManager.MakeTyped(((DocsPaVO.utente.Utente)corr).info, dettagliCorrispondente.Corrispondente.DataSet);
                    else if (corr.tipoCorrispondente.ToUpper() == "F")
                        DocsPaUtils.Data.TypedDataSetManager.MakeTyped(((DocsPaVO.utente.RaggruppamentoFunzionale)corr).info, dettagliCorrispondente.Corrispondente.DataSet);

                    validaCampi(dettagliCorrispondente, ref messaggio, corr.tipoCorrispondente);
                    if (!string.IsNullOrEmpty(messaggio))
                    {
                        risIns.errore = messaggio;
                        logger.Debug("Errore nella gestione degli utenti (insertCorrispondente)");
                        logger.Debug(risIns.errore.ToString());
                        return risIns;
                    }
                }

                //Fine codice Emanuela   

                if (!utenti.InsertCorrispondente(corr, parent, corr.idAmministrazione, ref risIns))
                {
                    logger.Debug("Errore nella gestione degli utenti (insertCorrispondente)");
                    logger.Debug(risIns.errore.ToString());
                }
                else
                {
                    if (risIns != null && !string.IsNullOrEmpty(risIns.systemId) && corr.interoperanteRGS)
                        BusinessLogic.FlussoAutomatico.FlussoAutomaticoManager.InsertCorrispondenteRGS(risIns.systemId, true);
                    
                    transactionContext.Complete();
                }
            }

            return risIns;
        }

        //Emanuela: METODO PER IL CONTROLLO SULLA VALIDITà DEI CAMPI DEL CORRISPONDENTE
        private static void validaCampi(DocsPaVO.addressbook.DettagliCorrispondente dettagliCorrispondente, ref string messaggio, string tipoCorr)
        {
            if (tipoCorr.ToUpper() == "U")
            {
                if ((dettagliCorrispondente.Corrispondente[0].codiceFiscale != null && !dettagliCorrispondente.Corrispondente[0].codiceFiscale.Equals("")) && ((dettagliCorrispondente.Corrispondente[0].codiceFiscale.Length == 11 && BusinessLogic.Rubrica.DPA3_RubricaSearchAgent.CheckVatNumber(dettagliCorrispondente.Corrispondente[0].codiceFiscale) != 0) || (dettagliCorrispondente.Corrispondente[0].codiceFiscale.Length == 16 && BusinessLogic.Rubrica.DPA3_RubricaSearchAgent.CheckTaxCode(dettagliCorrispondente.Corrispondente[0].codiceFiscale) != 0) || (dettagliCorrispondente.Corrispondente[0].codiceFiscale.Length != 11 && dettagliCorrispondente.Corrispondente[0].codiceFiscale.Length != 16)))
                {
                    messaggio = "Attenzione, il campo CODICE FISCALE non è valido";
                    return;
                }
            }
            else
            {
                if (dettagliCorrispondente.Corrispondente[0].codiceFiscale != null && !dettagliCorrispondente.Corrispondente[0].codiceFiscale.Equals("") && BusinessLogic.Rubrica.DPA3_RubricaSearchAgent.CheckTaxCode(dettagliCorrispondente.Corrispondente[0].codiceFiscale) != 0)
                {
                    messaggio = "Attenzione, il campo CODICE FISCALE non è valido";
                    return;
                }
            }

            if (dettagliCorrispondente.Corrispondente[0].partitaIva != null && !dettagliCorrispondente.Corrispondente[0].partitaIva.Equals("") && BusinessLogic.Rubrica.DPA3_RubricaSearchAgent.CheckVatNumber(dettagliCorrispondente.Corrispondente[0].partitaIva) != 0)
            {
                messaggio = "Attenzione, il campo PARTITA IVA non è valido";
                return;
            }
        }

        //METODO PER LA RICERCA DEI RUOLI SUPERIORI AD UN RUOLO APPARTENENTI ALLA STESSA UO
        /// <summary>
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public static System.Collections.ArrayList getRuoliSuperioriInUO(DocsPaVO.utente.Ruolo ruolo)
        {
            logger.Debug("getRuoliSuperioriInUO");
            System.Collections.ArrayList ruoli = new System.Collections.ArrayList();
            //DocsPa_V15_Utils.Database database=DocsPaWS.Utils.dbControl.getDatabase();
            DataSet dataSet = new DataSet();
            bool openDb = false;
            try
            {
                // TODO: Utilizzare il progetto DocsPaDbManagement
                //database.openConnection();
                openDb = true;

                #region Codice Commentato
                /*string queryString="SELECT A.SYSTEM_ID, A.VAR_COD_RUBRICA, A.ID_UO, A.ID_GRUPPO, A.VAR_CODICE, A.ID_PARENT, B.VAR_DESC_RUOLO, B.NUM_LIVELLO FROM DPA_CORR_GLOBALI A, DPA_TIPO_RUOLO B WHERE CHA_TIPO_URP='R' AND ID_UO="+ruolo.uo.systemId+" AND B.NUM_LIVELLO < "+ruolo.livello+" AND B.SYSTEM_ID=A.ID_TIPO_RUOLO";
				logger.Debug(queryString);
				
				// TODO: Utilizzare il progetto DocsPaDbManagement
				database.fillTable(queryString,dataSet,"RUOLI_SUP");*/
                #endregion

                DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                utenti.GetRuoliSuperiori(out dataSet, ruolo);

                if (dataSet.Tables["RUOLI_SUP"] != null)
                {
                    for (int i = 0; i < dataSet.Tables["RUOLI_SUP"].Rows.Count; i++)
                    {
                        DocsPaVO.utente.Ruolo ruoloSup = new DocsPaVO.utente.Ruolo();
                        System.Data.DataRow ruoloRow = dataSet.Tables["RUOLI_SUP"].Rows[i];
                        ruoloSup.codiceRubrica = ruoloRow["VAR_COD_RUBRICA"].ToString();

                        //MODIFICATO PER LA RISOLUZIONE DEL BUG 1116
                        //ruoloSup.descrizione=ruoloRow["VAR_DESC_RUOLO"].ToString();
                        ruoloSup.descrizione = ruoloRow["VAR_DESC_CORR"].ToString();
                        //
                        ruoloSup.systemId = ruoloRow["SYSTEM_ID"].ToString();
                        ruoloSup.livello = ruoloRow["NUM_LIV_B"].ToString();
                        ruoloSup.idGruppo = ruoloRow["ID_GRUPPO"].ToString();
                        ruoloSup.codiceCorrispondente = ruoloRow["VAR_CODICE"].ToString();
                        ruoloSup.uo = DocsPaDB.Utils.Gerarchia.getParents(ruoloRow["ID_PARENT"].ToString(), ruolo);
                        ruoli.Add(ruoloSup);
                    }
                }
                // TODO: Utilizzare il progetto DocsPaDbManagement
                //database.closeConnection();
                openDb = false;
                return ruoli;
            }
            catch (Exception e)
            {
                if (openDb)
                {
                    // TODO: Utilizzare il progetto DocsPaDbManagement
                    //database.closeConnection();
                }
                logger.Debug(e.Message);
                logger.Debug("Errore nella gestione degli utenti (getRuoliSuperioriInUO)", e);
                throw e;
            }
        }

        //METODO PER OTTENERE LA LISTA DEI CANALI
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static ArrayList getCanaliMethod()
        {
            logger.Debug("getCanaliMethod");
            System.Collections.ArrayList canali = new System.Collections.ArrayList();
            System.Data.DataSet dataSet = new System.Data.DataSet();

            try
            {
                #region Codice Commentato
                /*string queryCanaliString="SELECT SYSTEM_ID,TYPE_ID,DESCRIPTION FROM DOCUMENTTYPES";
				
				// TODO: Utilizzare il progetto DocsPaDbManagement
				database.fillTable(queryCanaliString,dataSet,"CANALI");*/
                #endregion

                DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                utenti.GetCanali(out dataSet);

                for (int i = 0; i < dataSet.Tables["CANALI"].Rows.Count; i++)
                {
                    System.Data.DataRow canaleRow = dataSet.Tables["CANALI"].Rows[i];
                    DocsPaVO.utente.Canale canale = new DocsPaVO.utente.Canale();
                    canale.systemId = canaleRow["SYSTEM_ID"].ToString();
                    canale.typeId = canaleRow["TYPE_ID"].ToString();
                    canale.descrizione = canaleRow["DESCRIPTION"].ToString();
                    canali.Add(canale);
                }
                return canali;
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                logger.Debug("Errore nella gestione degli utenti (getCanaliMethod)", e);
                throw e;
            }
        }

        /// <summary></summary>
        /// <param name="uo"></param>
        /// <returns>ruoli appartenenti alla UO - TODO: ruoli autorizzati </returns>
        public static ArrayList getRuoliRiferimentoEsterni(DocsPaVO.addressbook.QueryCorrispondenteAutorizzato queryCorr, DocsPaVO.utente.UnitaOrganizzativa uo)
        {
            logger.Debug("getRuoliRiferimentoEsterni");
            System.Collections.ArrayList ruoli = new System.Collections.ArrayList();
            DataSet dataSet = new DataSet();
            bool openDb = false;
            try
            {
                // TODO: Utilizzare il progetto DocsPaDbManagement
                //database.openConnection();
                openDb = true;

                DocsPaDB.Query_Utils.Utils utils = new DocsPaDB.Query_Utils.Utils();
                utils.GetRuoliRiferimentoEsterni(out dataSet, queryCorr, uo);

                if (dataSet.Tables["RUOLI_RIF_EST"] != null)
                {
                    for (int i = 0; i < dataSet.Tables["RUOLI_RIF_EST"].Rows.Count; i++)
                    {
                        DocsPaVO.utente.Ruolo ruoloRif = new DocsPaVO.utente.Ruolo();
                        System.Data.DataRow ruoloRow = dataSet.Tables["RUOLI_RIF_EST"].Rows[i];
                        ruoloRif.codiceRubrica = ruoloRow["VAR_COD_RUBRICA"].ToString();
                        ruoloRif.descrizione = ruoloRow["VAR_DESC_RUOLO"].ToString();
                        ruoloRif.systemId = ruoloRow["SYSTEM_ID"].ToString();
                        ruoloRif.livello = ruoloRow["NUM_LIVELLO"].ToString();
                        ruoloRif.idGruppo = ruoloRow["ID_GRUPPO"].ToString();
                        ruoloRif.codiceCorrispondente = ruoloRow["VAR_CODICE"].ToString();
                        ruoloRif.uo = uo;

                        ruoli.Add(ruoloRif);
                    }
                }
                // TODO: Utilizzare il progetto DocsPaDbManagement
                //database.closeConnection();
                openDb = false;
                return ruoli;
            }
            catch (Exception e)
            {
                if (openDb)
                {
                    // TODO: Utilizzare il progetto DocsPaDbManagement
                    //database.closeConnection();
                }
                logger.Debug(e.Message);
                logger.Debug("Errore nella gestione degli utenti (getRuoliSuperioriInUO)", e);
                throw e;
            }

        }

        /// <summary></summary>
        /// <param name="uo"></param>
        /// <returns>ruoli appartenenti alla UO - TODO: ruoli autorizzati </returns>
        public static ArrayList getRuoliRiferimentoAutorizzati(DocsPaVO.addressbook.QueryCorrispondenteAutorizzato queryCorr, DocsPaVO.utente.UnitaOrganizzativa uo)
        {
            logger.Debug("getRuoliRiferimentoAutorizzati");
            System.Collections.ArrayList ruoli = new System.Collections.ArrayList();
            DataSet dataSet = new DataSet();
            bool openDb = false;
            try
            {
                // TODO: Utilizzare il progetto DocsPaDbManagement
                //database.openConnection();
                openDb = true;

                DocsPaDB.Query_Utils.Utils utils = new DocsPaDB.Query_Utils.Utils();
                utils.GetRuoliRiferimentoAutorizzati(out dataSet, queryCorr, uo);

                if (dataSet.Tables["RUOLI_RIF"] != null)
                {
                    for (int i = 0; i < dataSet.Tables["RUOLI_RIF"].Rows.Count; i++)
                    {
                        DocsPaVO.utente.Ruolo ruoloRif = new DocsPaVO.utente.Ruolo();
                        System.Data.DataRow ruoloRow = dataSet.Tables["RUOLI_RIF"].Rows[i];
                        ruoloRif.codiceRubrica = ruoloRow["VAR_COD_RUBRICA"].ToString();
                        ruoloRif.descrizione = ruoloRow["VAR_DESC_RUOLO"].ToString();
                        ruoloRif.systemId = ruoloRow["SYSTEM_ID"].ToString();
                        ruoloRif.livello = ruoloRow["NUM_LIVELLO"].ToString();
                        ruoloRif.idGruppo = ruoloRow["ID_GRUPPO"].ToString();
                        ruoloRif.codiceCorrispondente = ruoloRow["VAR_CODICE"].ToString();
                        ruoloRif.idAmministrazione = uo.idAmministrazione;
                        ruoloRif.uo = uo;

                        ruoli.Add(ruoloRif);
                    }
                }
                // TODO: Utilizzare il progetto DocsPaDbManagement
                //database.closeConnection();
                openDb = false;
                return ruoli;
            }
            catch (Exception e)
            {
                if (openDb)
                {
                    // TODO: Utilizzare il progetto DocsPaDbManagement
                    //database.closeConnection();
                }
                logger.Debug(e.Message);
                logger.Debug("Errore nella gestione degli utenti (getRuoliSuperioriInUO)", e);
                throw e;
            }

        }


        #region Metodi Commentati
        /*private static DocsPaVO.utente.UnitaOrganizzativa getParents(string idParent, DataTable dt)
		{
			logger.Debug("getParents");
			DataRow parentRow=dt.Select("SYSTEM_ID='"+idParent+"'")[0];
			DocsPaVO.utente.UnitaOrganizzativa parent=new DocsPaVO.utente.UnitaOrganizzativa();
			parent.systemId=parentRow["SYSTEM_ID"].ToString();
			parent.descrizione=parentRow["VAR_DESC_CORR"].ToString();
			parent.codiceCorrispondente=parentRow["VAR_CODICE"].ToString();
			parent.codiceRubrica=parentRow["VAR_COD_RUBRICA"].ToString();
			parent.livello=parentRow["NUM_LIVELLO"].ToString();
			parent.codiceAOO=parentRow["VAR_CODICE_AOO"].ToString();
			parent.codiceAmm=parentRow["VAR_CODICE_AMM"].ToString();
			parent.codiceIstat=parentRow["VAR_CODICE_ISTAT"].ToString();
			parent.dettagli=fromCharToBool(parentRow["CHA_DETTAGLI"].ToString());
			DocsPaVO.utente.ServerPosta sp=new DocsPaVO.utente.ServerPosta();
			sp.serverSMTP=parentRow["VAR_SMTP"].ToString();
			sp.portaSMTP=parentRow["NUM_PORTA_SMTP"].ToString();
			parent.serverPosta=sp;
			if(parentRow["ID_REGISTRO"]!=null)
			{
				parent.idRegistro=parentRow["ID_REGISTRO"].ToString();
			}
			parent.email=parentRow["VAR_EMAIL"].ToString();
			parent.interoperante=fromCharToBool(parentRow["CHA_PA"].ToString());
			logger.Debug("Parent: "+parentRow["VAR_DESC_CORR"].ToString()+" "+parentRow["ID_PARENT"].ToString());
			if(!parentRow["ID_PARENT"].ToString().Equals("0"))
			{
				parent.parent=getParents(parentRow["ID_PARENT"].ToString(),dt);
			}
			return parent;
		}*/

        /*private static bool hasDefinedUo(string val, int type, string idParent, DataTable dt)
        {
            bool ret=false;
            DataRow parentRow=dt.Select("SYSTEM_ID='"+idParent+"'")[0];
            if((type==1 &&  stringContain(val, parentRow["VAR_DESC_CORR"].ToString()) )||(type==2 &&  parentRow["VAR_CODICE"].ToString().Equals(val)))
            {
                return true;
            }
            else
            {
                if(!parentRow["ID_PARENT"].ToString().Equals("0"))
                {
                    ret=hasDefinedUo(val, type, parentRow["ID_PARENT"].ToString(),dt);
                }
            }
            return ret;
        }*/
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objQueryCorrispondente"></param>
        /// <param name="nameTable"></param>
        /// <returns></returns>
        private static string registriCondition(QueryCorrispondente objQueryCorrispondente, string nameTable)
        {
            string condition = "";
            string prefix = "";
            if (nameTable != null) prefix = nameTable + ".";
            if (objQueryCorrispondente.idRegistri != null && objQueryCorrispondente.idRegistri.Count > 0)
            {
                condition = condition + " OR ((";
                for (int i = 0; i < objQueryCorrispondente.idRegistri.Count; i++)
                {
                    condition = condition + prefix + "ID_REGISTRO='" + objQueryCorrispondente.idRegistri[i] + "'";
                    if (i < objQueryCorrispondente.idRegistri.Count - 1) condition = condition + " OR ";
                }
                condition = condition + ") AND " + prefix + "ID_AMM='" + objQueryCorrispondente.idAmministrazione + "')";
            }
            return condition;
        }

        #region Metodo Commentato
        /*private static bool stringContain(string query, string obj)
		{
			char[] separator_or=ConfigurationManager.AppSettings["separator"].ToCharArray();
			char[] separator_and={' '};
			string query_up=query.ToUpper();
			string obj_up=obj.ToUpper();
			string[] query_array_or=query_up.Split(separator_or);
			for(int i=0;i<query_array_or.Length;i++)
			{
				string[] query_array_and=query_array_or[i].Split(separator_and);
				int ind_true=0;
				for(int j=0;j<query_array_and.Length;j++)
				{
					if(obj_up.IndexOf(query_array_and[j])>-1)
					{
						ind_true=ind_true+1;
					}
					
				}
				if(ind_true==query_array_and.Length) return true;
			};
			return false;
		}*/
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static bool fromCharToBool(string str)
        {
            if (str.Equals("1"))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        #region Metodi Commentati
        /*private static void isCodRubricaPresente(string codRubrica,string idAmm,DocsPa_V15_Utils.Database database, System.Data.DataSet dataSet)
		{
			logger.Debug("isCodRubricaPresente");
			string codiceString="SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI A WHERE VAR_COD_RUBRICA='"+codRubrica+"' AND (ID_AMM="+idAmm+" OR ID_AMM IS NULL)";
			logger.Debug(codiceString);
			
			// TODO: Utilizzare il progetto DocsPaDbManagement
			database.fillTable(codiceString,dataSet,"CODICE");
			if(dataSet.Tables["CODICE"].Rows.Count>0)
			{
				throw new Exception("Codice già presente in rubrica");
			}
		}*/

        /*public static void isCodeCorrect(string codRubrica)
        {
            logger.Debug("isCodeCorrect");
            char[] separator={';'};
            String[] prefissi=System.Configuration.ConfigurationManager.AppSettings["prefissiRubricaRiservati"].Split(separator);
            for(int i=0;i<prefissi.Length;i++)
            {
                if(codRubrica.ToUpper().StartsWith(prefissi[i]))
                {
                    logger.Debug("Il codice rubrica inserito inizia per "+prefissi[i]);
                    throw new Exception("Il codice rubrica inserito non può iniziare per "+prefissi[i]);
                }
            }
        }*/

        /*public static string updateCodRubrica(string systemId,string prefix,string idAmm,DocsPa_V15_Utils.Database database)
        {
            logger.Debug("updateCorrRubrica");
            string res=null;
            try
            {
                string updateRuoloQuery="UPDATE DPA_CORR_GLOBALI SET VAR_COD_RUBRICA='"+prefix+systemId+"' WHERE SYSTEM_ID="+systemId+
                    " AND NOT EXISTS (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI A WHERE ID_AMM="+idAmm+" AND VAR_COD_RUBRICA='"+prefix+systemId+"')";
                logger.Debug(updateRuoloQuery);
				
                // TODO: Utilizzare il progetto DocsPaDbManagement
                if(resultUpdate==false){
                    throw new Exception("Update del codice rubrica non effettuato");
                }
                res=prefix+systemId;
            }
            catch(Exception e){
               throw e;
            }
            return res;
        }*/

        /*public static void insertDettagli(string systemId,DocsPaVO.addressbook.DettagliCorrispondente dettagli,DocsPa_V15_Utils.Database database)
        {
            logger.Debug("insertDettagli");
            try
            {
                string insertDettagliQuery="INSERT INTO DPA_DETT_GLOBALI (ID_CORR_GLOBALI,VAR_INDIRIZZO,VAR_CAP,VAR_CITTA,VAR_PROVINCIA,VAR_NAZIONE,VAR_TELEFONO,VAR_TELEFONO2,VAR_FAX,VAR_COD_FIS,VAR_NOTE) VALUES"+
                    " ("+systemId+",'"+correctApici(dettagli.indirizzo)+"','"+correctApici(dettagli.cap)+"','"+correctApici(dettagli.citta)+"','"+correctApici(dettagli.provincia)+"','"+correctApici(dettagli.nazione)+"','"+
                    correctApici(dettagli.telefono)+"','"+correctApici(dettagli.telefono2)+"','"+correctApici(dettagli.fax)+"','"+correctApici(dettagli.codiceFiscale)+"','"+correctApici(dettagli.note)+"')";
                    logger.Debug(insertDettagliQuery);
                // TODO: Utilizzare il progetto DocsPaDbManagement
                database.executeNonQuery(insertDettagliQuery);
            }
            catch(Exception e)
            {
                throw e;
            }
        }*/
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="canalePref"></param>
        private static void insertCanalePref(string sysId, DocsPaVO.utente.Canale canalePref/*,DocsPa_V15_Utils.DBAgent db*/)
        {
            logger.Debug("insertCanalePref");
            try
            {
                DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                utenti.InsertCanalePref(sysId, canalePref);
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella gestione degli utenti (insertCanalePref)", e);
                throw e;
            }
        }

        #region Metodi Commentati
        /*private static string correctApici(string str){
			if(str!=null)
			{
			   return str.Replace("'","''");
			}
			else
			{
			   return str;
			}
		}*/

        /*private static DocsPaVO.addressbook.QueryCorrispondente correctApiciQuery(DocsPaVO.addressbook.QueryCorrispondente qco)
        {
            DocsPaVO.addressbook.QueryCorrispondente res=new DocsPaVO.addressbook.QueryCorrispondente();
            res.codiceGruppo=correctApici(qco.codiceGruppo);
            res.codiceRubrica=correctApici(qco.codiceRubrica);
            res.codiceRuolo=correctApici(qco.codiceRuolo);
            res.codiceUO=correctApici(qco.codiceUO);
            res.cognomeUtente=correctApici(qco.cognomeUtente);
            res.descrizioneGruppo=correctApici(qco.descrizioneGruppo);
            res.descrizioneRuolo=correctApici(qco.descrizioneRuolo);
            res.descrizioneUO=correctApici(qco.descrizioneUO);
            res.fineValidita=qco.fineValidita;
            res.getChildren=qco.getChildren;
            res.idAmministrazione=qco.idAmministrazione;
            res.idRegistri=qco.idRegistri;
            res.nomeUtente=correctApici(qco.nomeUtente);
            res.systemId=qco.systemId;
            res.tipoUtente=qco.tipoUtente;
            return res;
        }*/
        #endregion

        public static bool isCorrispondenteValido(string idCorrispondente)
        {
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            return utenti.isCorrispondenteValido(idCorrispondente);
        }

        //elisa ruoli utente interno
        /// <summary>
        /// 
        /// </summary>
        /// <param name="codice rubrica"></param>
        /// <returns></returns>
        public static DataSet getRuoliUtenteInt(string codiceRubrica)
        {
            DataSet ds = new DataSet();
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            ds = utenti.GetRuoliUtenteInterno(codiceRubrica);
            if (ds == null)
            {
                logger.Debug("Errore nella gestione degli utenti (GetRuoliUtenteInterno)");
                throw new Exception();
            }

            return ds;
        }

        public static ArrayList GetRuoliUtente(string id_amm, string cod_rubrica)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                return amm.GetRuoliUtente(id_amm, cod_rubrica);
            }
            catch
            {
                logger.Debug("Errore nella gestione degli utenti (GetRuoliUtente)");
                throw new Exception();
            }
        }


        public static bool VerificaAutorizzazioneRuolo(string idRuolo, string idregistro)
        {
            bool result = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.Utenti ut = new DocsPaDB.Query_DocsPAWS.Utenti();
                result = ut.VerificaRuoloAut(idRuolo, idregistro);
            }
            catch
            {
                logger.Debug("Errore nella verifica del ruolo autorizzato (VerificaAutorizzazioneRuolo)");
                throw new Exception();
            }
            return result;
        }

        public static ArrayList GetParentUo(string idUo, string livelloUO)
        {
            try
            {
                DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();
                return gerarchia.getChildrenUO(idUo, livelloUO);
            }
            catch
            {
                logger.Debug("Errore nel reperimento dei figli di una Uo: metodo getChildrenUO");
                throw new Exception();
            }
        }

        #region Multi Casella corrispondenti esterni
        public static System.Collections.Generic.List<DocsPaVO.utente.MailCorrispondente> GetMailCorrispondente(string idCorrispondente)
        {
            DocsPaDB.Query_DocsPAWS.Utenti ut = new DocsPaDB.Query_DocsPAWS.Utenti();
            DataSet ds = new System.Data.DataSet();
            System.Collections.Generic.List<DocsPaVO.utente.MailCorrispondente> listMailCorr = new System.Collections.Generic.List<DocsPaVO.utente.MailCorrispondente>();
            try
            {
                ds = ut.GetMailCorr(idCorrispondente);
                if (ds != null && ds.Tables["CASELLE_CORRISPONDENTE"].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables["CASELLE_CORRISPONDENTE"].Rows)
                    {
                        DocsPaVO.utente.MailCorrispondente mailCorr = new DocsPaVO.utente.MailCorrispondente();
                        mailCorr.systemId = row["SystemId"].ToString();
                        mailCorr.Email = row["Email"].ToString();
                        mailCorr.Principale = row["Principale"].ToString();
                        mailCorr.Note = row["Note"].ToString();
                        listMailCorr.Add(mailCorr);
                    }
                }
                return listMailCorr;
            }
            catch (Exception e)
            {
                return new System.Collections.Generic.List<DocsPaVO.utente.MailCorrispondente>();
            }
        }

        public static System.Collections.Generic.List<DocsPaVO.utente.MailCorrispondente> GetMailsAllCorrProto(int idDoc)
        {
            System.Collections.Generic.List<DocsPaVO.utente.MailCorrispondente> listMailAllCorrProto =
                new System.Collections.Generic.List<DocsPaVO.utente.MailCorrispondente>();
            DocsPaDB.Query_DocsPAWS.Utenti ut = new DocsPaDB.Query_DocsPAWS.Utenti();
            DataSet ds = new System.Data.DataSet();
            try
            {
                ds = ut.GetMailsAllCorrProto(idDoc);
                if (ds != null && ds.Tables["CASELLE_CORRISPONDENTE"].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables["CASELLE_CORRISPONDENTE"].Rows)
                    {
                        listMailAllCorrProto.Add(new DocsPaVO.utente.MailCorrispondente()
                        {
                            systemId = row["idCorr"].ToString(),
                            Email = row["email"].ToString(),
                            Note = row["note"].ToString(),
                            Principale = row["principale"].ToString()
                        });
                    }
                }
                return listMailAllCorrProto;
            }
            catch (Exception e)
            {
                return new System.Collections.Generic.List<DocsPaVO.utente.MailCorrispondente>();
            }
        }

        public static bool InsertMailCorrispondente(System.Collections.Generic.List<DocsPaVO.utente.MailCorrispondente> listCaselle, string idCorrispondente)
        {
            bool result = false;
            DocsPaDB.Query_DocsPAWS.Utenti ut = new DocsPaDB.Query_DocsPAWS.Utenti();
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                result = ut.InsertMailCorr(listCaselle, idCorrispondente);
                if(result)
                    transactionContext.Complete();
            }
            return result;
        }

        public static bool DeleteMailCorrispondente(string idCorrispondente)
        {
            DocsPaDB.Query_DocsPAWS.Utenti ut = new DocsPaDB.Query_DocsPAWS.Utenti();
            return ut.DeleteMailCorr(idCorrispondente);
        }
        #endregion

        /// <summary>
        /// Metodo per il recupero del codice di un corrispondente nella sua ultima versione
        /// </summary>
        /// <param name="code">Codice del corrispondente</param>
        /// <returns>Codice associato al corrispondente nella sua ultima versione</returns>
        public static String GetActualCorrCode(String code)
        {
            using (DocsPaDB.Query_DocsPAWS.Utenti dbUser = new DocsPaDB.Query_DocsPAWS.Utenti())
            {
                return dbUser.GetActualCorrCode(code);
            }
        }

        //METODO PER LA VERIFICA DI UN CORRISPONDENTE IN RUBRICA
        public static bool VerificaInserimentoCorrispondente(DocsPaVO.utente.Corrispondente corr, DocsPaVO.utente.Corrispondente parent)
        {
            bool result = true;

            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

            if (utenti.CodiceRubricaPresente(corr.codiceRubrica, corr.tipoCorrispondente, corr.idAmministrazione, corr.idRegistro, corr.inRubricaComune))
            {
                result = false;
            }

            return result;
        }


        public static DocsPaVO.utente.Corrispondente GetDettaglioCorrispondente(string idCorrispondente)
        {
            DocsPaVO.utente.Corrispondente result = null;
            
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

            result = utenti.GetDettaglioCorrispondente(idCorrispondente);

            return result;
        }

        public static bool IsCodRubricaPresente(string codRubrica, string tipoCorr, string idAmm, string idReg, bool inRubricaComune)
        {
            bool retVal = false;
            try
            {
                using (DocsPaDB.Query_DocsPAWS.Utenti users = new DocsPaDB.Query_DocsPAWS.Utenti())
                {
                    DataSet dataSet = new DataSet();
                    users.IsCodRubricaPresente(codRubrica, tipoCorr, idAmm, idReg, inRubricaComune, out dataSet);
                }
            }
            catch (Exception e)
            {
                retVal = true;
            }

            return retVal;
        }

    }
}
