using System;
using System.Data;
using System.Xml;
using System.Globalization;
using System.Collections;
using log4net;
using System.Collections.Generic;
using DocsPaVO.Spedizione;

namespace DocsPaDB.Query_DocsPAWS
{
	/// <summary>
	/// Classe contenente tutte le query (22) di DocsPAWS > interoperabilita
	/// </summary>
	public partial class Interoperabilita : DBProvider
	{
        private ILog logger = LogManager.GetLogger(typeof(Interoperabilita));

		#region DocsPaWS.interoperabilita.InteroperabilitaControlloRicevute (2)

		/// <summary>
		/// Query per il metodo "findIdProfRicevuta"
		/// </summary>
		/// <param name="dataSet"></param>
		/// <param name="codiceAOO"></param>
		/// <param name="numeroRegistrazione"></param>
		/// <param name="anno"></param>
		public void findIdProf(out DataSet dataSet,string codiceAOO,string numeroRegistrazione,int anno)
		{	
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PROFILE__DPA_EL_REGISTRI");
			q.setParam("param1",codiceAOO);
			q.setParam("param2",numeroRegistrazione);
			q.setParam("param3",anno.ToString());
			string queryString = q.getSQL();
			ExecuteQuery(out dataSet,"ID_DOC",queryString);

            if (!(dataSet != null && dataSet.Tables["ID_DOC"] != null && dataSet.Tables["ID_DOC"].Rows.Count > 0 ))
            {
                //rifaccio il controllo con anno precedente perchè potrebbe essere che ricevo una conferma di un protocollo dell'anno
                //precedente, protocollato dal destinatario nell'anno successivo.. più di un anno di differenza sembra molto imporbabile.
                
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PROFILE__DPA_EL_REGISTRI");
                q.setParam("param1", codiceAOO);
                q.setParam("param2", numeroRegistrazione);
                q.setParam("param3", (anno - 1).ToString());
                queryString = q.getSQL();
                ExecuteQuery(out dataSet, "ID_DOC", queryString);

            }


		}

		/// <summary>
		/// Query per il metodo "updateStatoInvio"
		/// </summary>
		/// <param name="idProf"></param>
		/// <param name="codiceAOO"></param>
		/// <param name="codiceAmministrazione"></param>
		/// <param name="data"></param>
		/// <param name="numeroRegistrazione"></param>
		/// <param name="anno"></param>
		/// <param name="debug"></param>
		public bool updStatoInvio(string idProf, string codiceAOO,string codiceAmministrazione, string data, string numeroRegistrazione,int anno)
		{
			bool res=false;
            // PEC 4 Modifica Maschera Caratteri
            string statusmask = getStatusMask1(idProf, codiceAOO, codiceAmministrazione);
            if (!string.IsNullOrEmpty(statusmask))
            {
                char[] sm = statusmask.ToCharArray();
                sm[3] = 'V';
                sm[0] = 'V';
                if (sm[5] != 'X')
                    sm[5] = 'N';
                statusmask = new string(sm);
            }

			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPAStatoInvio1");
			q.setParam("param1","'"+numeroRegistrazione+"/"+codiceAOO+"/"+anno+"'");
			q.setParam("param2",DocsPaDbManagement.Functions.Functions.ToDate(data));

            // PEC 4 Modifica Maschera Caratteri
            // Inserisco il cancellamento dell'eccezione
            if (string.IsNullOrEmpty(statusmask))
                q.setParam("statusmask", ", STATUS_C_MASK = 'VVVVANN', CHA_ANNULLATO = NULL, VAR_MOTIVO_ANNULLA = NULL");
            else
                q.setParam("statusmask", ", STATUS_C_MASK = '" + statusmask + "', CHA_ANNULLATO = NULL, VAR_MOTIVO_ANNULLA = NULL");
			//DocsPaWS.Utils.dbControl.toDate(data,false));
			q.setParam("param3",idProf);
            if (codiceAOO != null)
                codiceAOO = codiceAOO.ToUpper();
			q.setParam("param4","'" + codiceAOO + "'");
            if (codiceAmministrazione != null)
                codiceAmministrazione = codiceAmministrazione.ToUpper();
			q.setParam("param5","'" + codiceAmministrazione + "'");
			string updateString = q.getSQL();
			logger.Debug(updateString);			
			res=ExecuteNonQuery(updateString);
			return res;
		}

        /// <summary>
        /// PEC 4 Modifica Maschera Caratteri
        /// Prende la status mask della riga della stato invio.
        /// </summary>
        /// <param name="idProf"></param>
        /// <param name="codiceAOO"></param>
        /// <param name="codiceAmministrazione"></param>
        /// <returns></returns>
        public string getStatusMask1(string idProf, string codiceAOO, string codiceAmministrazione, string systemidDPASI="")
        {
            string statusMask = null;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAStatoinvio");
            q.setParam("param1", "status_c_mask as mask");
            string condizione = "";
            // Se inserisco il parametro systemidDPASI sto prendendo la status mask per inserire l'eccezione...
            if (!string.IsNullOrEmpty(systemidDPASI))
            {
                condizione += " SYSTEM_ID= " + systemidDPASI;
            }
            else
            {
                if (!string.IsNullOrEmpty(idProf))
                {
                    condizione += "id_profile=" + idProf;
                }
                if (!string.IsNullOrEmpty(idProf) && !string.IsNullOrEmpty(codiceAOO))
                {
                    condizione += " AND ";
                }
                if (!string.IsNullOrEmpty(codiceAOO))
                {
                    condizione += "upper(VAR_CODICE_AOO) = '" + codiceAOO.ToUpper() + "'";
                }
                if (!string.IsNullOrEmpty(codiceAmministrazione) && (!string.IsNullOrEmpty(idProf) || !string.IsNullOrEmpty(codiceAOO)))
                {
                    condizione += " AND ";
                }
                if (!string.IsNullOrEmpty(codiceAmministrazione))
                {
                    condizione += "upper(VAR_CODICE_AMM)= '" + codiceAmministrazione.ToUpper() + "'";
                }
            }
            q.setParam("param2", condizione);
            string query = q.getSQL();


            //string query = "select status_c_mask as mask from dpa_stato_invio where id_profile=" + idProf + " AND upper(VAR_CODICE_AOO) = '" + codiceAOO.ToUpper() + "' and upper(VAR_CODICE_AMM)= '" + codiceAmministrazione.ToUpper() + "'" ; // temporanea

            logger.Debug(query);
            DataSet ds= new DataSet();
            ExecuteQuery(ds, "STAT_MASK", query);
            if (ds != null && ds.Tables["STAT_MASK"] != null && ds.Tables["STAT_MASK"].Rows.Count > 0)
            {
                statusMask = ds.Tables["STAT_MASK"].Rows[0]["mask"].ToString();
            }
            return statusMask;
        }
		#endregion

        #region DocsPaWS.interoperabilita.NotificaEccezione
        /// <summary>
        /// Query per il metodo "updateStatoEccezione identico a updateStatoInvia, cambia CHA_ANNULLA da 1 a E "
        /// </summary>
        /// <param name="codiceAOO"></param>
        /// <param name="codiceAmministrazione"></param>
        /// <param name="data"></param>
        /// <param name="numeroRegistrazione"></param>
        /// <param name="anno"></param>
        /// <param name="debug"></param>
        public bool updStatoInvioEccezione(string SystemID,  string motivo_annulla)
        {
            bool res = false;
            string statusmask= getStatusMask1("", "", "", SystemID);
            if (!string.IsNullOrEmpty(statusmask))
            {
                char[] sm = statusmask.ToCharArray();
                if (sm[5] == 'A' && sm[2]=='V')
                {
                    sm[0] = 'X';
                    sm[3] = 'N';
                    sm[4] = 'N';
                    sm[5] = 'X';
                    if (motivo_annulla.Contains("Il documento è stato ricevuto dall’Amministrazione destinataria e pertanto non occorre effettuare rispedizioni."))
                    {
                        sm[0] = 'A';
                        sm[3] = 'A';
                        sm[4] = 'A';
                    }
                    statusmask = new string(sm);
                }
            }
            DocsPaUtils.Query q;

            string condition = String.Format("SYSTEM_ID = {0}", SystemID);
            string values = string.Format("VAR_MOTIVO_ANNULLA = '{0}' ,CHA_ANNULLATO = '{1}'", motivo_annulla.Replace("'", "''"), "E");
            if (!string.IsNullOrEmpty(statusmask))
            {
                values += ", STATUS_C_MASK ='" + statusmask + "'";
            }
            else
            {
                if (motivo_annulla.Contains("Il documento è stato ricevuto dall’Amministrazione destinataria e pertanto non occorre effettuare rispedizioni."))
                {
                    values += ", STATUS_C_MASK ='AVVAAXN'";
                }
                else
                {
                    values += ", STATUS_C_MASK ='XVVNNXN'";
                }
            }
            q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPAStatoinvio");
            q.setParam("param2", condition);
            q.setParam("param1", values);

            string updateString = q.getSQL();
            logger.Debug(updateString);
            res = ExecuteNonQuery(updateString);
            return res;
        }
        #endregion

		#region DocsPaWS.interoperabilita.NotificaAnnullamento
		/// <summary>
		/// Query per il metodo "updateStatoInvio"
		/// </summary>
		/// <param name="codiceAOO"></param>
		/// <param name="codiceAmministrazione"></param>
		/// <param name="data"></param>
		/// <param name="numeroRegistrazione"></param>
		/// <param name="anno"></param>
		/// <param name="debug"></param>
		public bool updStatoInvioAnnulla(string idProfile,string codiceAOO,string codiceAmministrazione, string data, string numeroRegistrazione,int anno,string motivo_annulla, string provvedimento)
		{
			bool res=false;
            // PEC 4 Modifica Maschera Caratteri
            string statusmask = getStatusMask1(idProfile, codiceAOO, codiceAmministrazione);
            if (!string.IsNullOrEmpty(statusmask))
            {
                char[] sm = statusmask.ToCharArray();
                sm[4] = 'V';
                statusmask = new string(sm);
            }

			DocsPaUtils.Query q;

			if (motivo_annulla!=null && !motivo_annulla.Equals(""))
				motivo_annulla=motivo_annulla.Replace("'","''");
			if (provvedimento!= null && !provvedimento.Equals(""))
				provvedimento=provvedimento.Replace("'","''");

			if (idProfile != null && !idProfile.Equals(""))
			{
				q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPAStatoInvio3");
				q.setParam("param8",idProfile);
					
			}
			else
			{
				q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPAStatoInvio2");
			}
			q.setParam("param1","'"+numeroRegistrazione+"/"+codiceAOO+"/"+anno+"'");
			q.setParam("param2",DocsPaDbManagement.Functions.Functions.ToDate(data));	
			q.setParam("param3","'" + codiceAOO + "'");
			q.setParam("param4","'" + codiceAmministrazione + "'");
			q.setParam("param5","'" + motivo_annulla.Replace("'","''") + "'");
			q.setParam("param6","'1'");
			q.setParam("param7","'" + provvedimento + "'");
            // PEC 4 Modifica Maschera Caratteri
            if (string.IsNullOrEmpty(statusmask))
                q.setParam("statusmask", ", STATUS_C_MASK = 'VVVVVNN'");
            else
                q.setParam("statusmask", ", STATUS_C_MASK = '" + statusmask + "'");
			string updateString = q.getSQL();
			logger.Debug(updateString);			
			res=ExecuteNonQuery(updateString);
			return res;
		}
		#endregion

		#region DocsPaWS.interoperabilita.InteroperabilitaManagerRicevute (3)

		/// <summary>
		/// Query #1 per il metodo "costruisciXml"
		/// </summary>
		/// <param name="dataSet"></param>
		/// <param name="idProfile"></param>
		/// <param name="debug"></param>
		public void getMittSegn(out DataSet dataSet,string idProfile)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__PROFILE__DOC_ARRIVO_PAR");
			q.setParam("param1",idProfile);
			string queryMittString = q.getSQL();
			logger.Debug(queryMittString);			
			ExecuteQuery(out dataSet,"INFO_MITT",queryMittString);
		}
		public void getDatiProtoSpedito(out DataSet dataSet,string idProfile)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_PROFILE_DATI_PROTO");
			q.setParam("param1",idProfile);
			string queryString = q.getSQL();
			logger.Debug(queryString);			
			ExecuteQuery(out dataSet,"INFO_PROTO",queryString);
		}


		/// <summary>
		/// Query #2 per il metodo "costruisciXml"
		/// </summary>
		/// <param name="ds"></param>
		/// <param name="idProfile"></param>
		/// <param name="debug"></param>
		public void getIdent(out DataSet ds,string idProfile)
		{			
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Profile");
			q.setParam("param1",idProfile);
			string queryProtoString = q.getSQL();
			logger.Debug(queryProtoString);
			ExecuteQuery(out ds,"INFO_PROTO",queryProtoString);
		}

		#endregion

		#region DocsPaWS.interoperabilita.InteroperabilitaInvioSegnatura (2)

		/// <summary>
		/// Query per il metodo "interopInviaMethod"
		/// </summary>
		/// <param name="ds"></param>
		/// <param name="reg"></param>
		/* NON PIU' NECESSARIO CON L'AGGIUNTA DEL MULTICASELLA
         * public void getDatiReg(out DataSet ds,DocsPaVO.utente.Registro reg)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_EL_REGISTRI__AMMINISTRA3");
			q.setParam("param1",reg.systemId);			
			string queryRegString = q.getSQL();						
			ExecuteQuery(out ds,"REGISTRO",queryRegString);
		}*/

		/// <summary>
		/// Query per il metodo "isMailPreferred"
		/// </summary>
		/// <param name="ds"></param>
		/// <param name="corr"></param>
		public void getDatiCan(out DataSet ds,DocsPaVO.utente.Corrispondente corr)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_T_CANALE_CORR__DOCUMENTTYPES2");
			q.setParam("param1",corr.systemId);			
			string queryString = q.getSQL();
            logger.Debug(queryString);			
		 	ExecuteQuery(out ds,"CANALE",queryString);
		}

        public void setRicevutaPec(string idRegistro, DocsPaVO.amministrazione.CasellaRegistro[] casella)
        {
            new Amministrazione().UpdateMailRegistro(idRegistro, casella);
        }
		#endregion

		#region DocsPaWS.interoperabilita.InteroperabilitaRicezione (3)

		/// <summary>
		/// Query per il metodo "getRuoloReg"
		/// </summary>
		/// <param name="ds"></param>
		/// <param name="reg"></param>
		public void getRuoReg(out DataSet ds,DocsPaVO.utente.Registro reg)
		{
			logger.Debug("start > getRuoReg");
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__TIPO_RUOLO__L_RUOLO_REG");
			q.setParam("param1",reg.systemId);			
			string queryString = q.getSQL();
			logger.Debug(queryString);			
			ExecuteQuery(out ds,"RUOLO",queryString);
		}

		/// <summary>
		/// Query per il metodo "getUtenteReg"
		/// </summary>
		/// <param name="ds"></param>
		/// <param name="ruolo"></param>
		public void getUtReg(out DataSet ds,DocsPaVO.utente.Ruolo ruolo)
		{
			logger.Debug("start > getUtReg");
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PEOPLE__PEOPLEGROUPS");
			q.setParam("param1",ruolo.idGruppo);			
			string queryString = q.getSQL();
			logger.Debug(queryString);			
			ExecuteQuery(out ds,"UTENTE",queryString);
		}

        public string isEnabledSaveMail(string idReg)
        {
            string result = string.Empty;

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_INTEROP_ENABLED_SAVE_MAIL");
                q.setParam("idReg", idReg);
                string commandText = q.getSQL();
                logger.Debug(commandText);
                string field = string.Empty;
                if (!ExecuteScalar(out field, commandText))
                {
                    throw new Exception(LastExceptionMessage);
                }
                result = field;

            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                result = string.Empty;
            }

            return result;

        }

		#endregion

		#region DocsPaWS.interoperabilita.InteroperabilitaSegnatura (10)

		/// <summary>
		/// Query #1 per il metodo "addNewCorrispondente"
		/// </summary>
		/// <param name="ds"></param>
		/// <param name="codiceAmm"></param>
		/// <param name="reg"></param>
		/// <param name="debug"></param>
		public void getCodRubr(out DataSet ds,string codiceAmm,DocsPaVO.utente.Registro reg)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlob8");
			q.setParam("param1",codiceAmm);	
			q.setParam("param2",reg.idAmministrazione);	
			q.setParam("param3",reg.systemId);	
			string queryString = q.getSQL();	
			logger.Debug(queryString);		
			ExecuteQuery(out ds,"AMMINISTRAZIONE",queryString);
		}

		/// <summary>
		/// Query #2 per il metodo "addNewCorrispondente"
		/// </summary>
		/// <param name="elOrigine"></param>
		/// <param name="db"></param>
		/// <param name="tipoIE"></param>
		/// <param name="reg"></param>
		/// <param name="mailMitt"></param>
		/// <param name="descrizioneAmm"></param>
		/// <param name="dataInizio"></param>
		/// <param name="codiceAmm"></param>
		/// <param name="tipoCorr"></param>
		/// <param name="codiceAOO"></param>
		/// <param name="descrInterAmm"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		public string addNewCorr(XmlElement elOrigine,/*DocsPaWS.Utils.Database db,*/string tipoIE,DocsPaVO.utente.Registro reg,string mailMitt,string descrizioneAmm, string codiceAmm,string tipoCorr,string codiceAOO,string descrInterAmm)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPACorrGlobali4");
			string dataInizio = DocsPaDbManagement.Functions.Functions.ToDate(System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", new CultureInfo("en-US") ));
			
			q.setParam("param1",DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
			//DocsPaWS.Utils.dbControl.GetSystemIdColName());	
			if(isLastUO((XmlElement) elOrigine.SelectSingleNode("Mittente/Amministrazione")))
			{				
				q.setParam("param2",",VAR_EMAIL");	
				q.setParam("param14",",'"+mailMitt+"'");
			}			
			q.setParam("param3",DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_CORR_GLOBALI"));
			//DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_CORR_GLOBALI"));	
			q.setParam("param4","'"+tipoIE+"',");
			q.setParam("param5",reg.systemId+",");
			q.setParam("param6",reg.idAmministrazione+",");
			q.setParam("param7","'"+descrizioneAmm.Replace("'","''")+"',");
			q.setParam("param8",dataInizio+",");
			q.setParam("param9","'"+codiceAmm+"',");
			q.setParam("param10","'"+tipoCorr+"',");
			q.setParam("param11","'"+getPA((XmlElement) elOrigine.SelectSingleNode("Mittente/Amministrazione"))+"',");
			q.setParam("param12","'"+codiceAOO+"',");
			q.setParam("param13","'INTEROP_"+descrInterAmm.Replace("'","''")+"'");
			
			string insertAmm = q.getSQL();
			logger.Debug(insertAmm);
			string sysId;
			InsertLocked(out sysId,insertAmm,"DPA_CORR_GLOBALI");
			return sysId;
		}

        /// <summary>
        /// Query #2 per il metodo "addNewCorrispondente"
        /// </summary>
        /// <param name="elOrigine"></param>
        /// <param name="db"></param>
        /// <param name="tipoIE"></param>
        /// <param name="reg"></param>
        /// <param name="mailMitt"></param>
        /// <param name="descrizioneAmm"></param>
        /// <param name="dataInizio"></param>
        /// <param name="codiceAmm"></param>
        /// <param name="tipoCorr"></param>
        /// <param name="codiceAOO"></param>
        /// <param name="descrInterAmm"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public string addNewCorrByCodEDesc(XmlElement elOrigine,/*DocsPaWS.Utils.Database db,*/string tipoIE, DocsPaVO.utente.Registro reg, string mailMitt, string descrizioneAmm, string codiceAmm, string tipoCorr, string codiceAOO, string descrInterAmm, string codEdesc)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPACorrGlobali4");
            string dataInizio = DocsPaDbManagement.Functions.Functions.ToDate(System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", new CultureInfo("en-US")));

            q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
            //DocsPaWS.Utils.dbControl.GetSystemIdColName());	
            if (isLastUO((XmlElement)elOrigine.SelectSingleNode("Mittente/Amministrazione")))
            {
                q.setParam("param2", ",VAR_EMAIL , COD_DESC_INTEROP");
                q.setParam("param14", ",'" + mailMitt + "'" + ",'" + DocsPaUtils.Functions.Functions.ReplaceApexes(codEdesc) + "'");
            }
            else
            {
                q.setParam("param2", ",COD_DESC_INTEROP ");
                q.setParam("param14", ",'" + DocsPaUtils.Functions.Functions.ReplaceApexes(codEdesc) + "'");
            }

            q.setParam("param3", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_CORR_GLOBALI"));
            //DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_CORR_GLOBALI"));	
            q.setParam("param4", "'" + tipoIE + "',");
            q.setParam("param5", reg.systemId + ",");
            q.setParam("param6", reg.idAmministrazione + ",");
            q.setParam("param7", "'" + descrizioneAmm.Replace("'", "''") + "',");
            q.setParam("param8", dataInizio + ",");
            q.setParam("param9", "'" + codiceAmm + "',");
            q.setParam("param10", "'" + tipoCorr + "',");
            q.setParam("param11", "'" + getPA((XmlElement)elOrigine.SelectSingleNode("Mittente/Amministrazione")) + "',");
            q.setParam("param12", "'" + codiceAOO + "',");
            q.setParam("param13", "'INTEROP_" + descrInterAmm.Replace("'", "''") + "'");

            string insertAmm = q.getSQL();
            logger.Debug(insertAmm);
            string sysId;
            InsertLocked(out sysId, insertAmm, "DPA_CORR_GLOBALI");
            return sysId;
        }

		/// <summary>
		/// Query #3 per il metodo "addNewCorrispondente"
		/// </summary>
		/// <param name="codRubricaAmm"></param>
		/// <param name="sysId"></param>
		public void setCodRub(string codRubricaAmm,string sysId, bool updateDescCorr)
		{
            DocsPaUtils.Query q = null;
            if(updateDescCorr)
                q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPACorrGlobDescCorrAndCod");
            else
                q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPACorrGlob5");
			q.setParam("param1",codRubricaAmm);	
			q.setParam("param2",sysId);	
			string updateCodRubricaAmm = q.getSQL();
			ExecuteNonQuery(updateCodRubricaAmm);
		}

		/// <summary>
		/// Query #4 per il metodo "addNewCorrispondente"
		/// </summary>
		/// <param name="ds"></param>
		/// <param name="descrizioneUO"></param>
		/// <param name="reg"></param>
		/// <param name="debug"></param>
		public void getExistUo(out DataSet ds,string descrizioneUO,DocsPaVO.utente.Registro reg,string idParent)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlob9");
			q.setParam("param1",descrizioneUO.Replace("'","''"));	
			q.setParam("param2",reg.idAmministrazione);	
			q.setParam("param3",idParent);	
			string queryString = q.getSQL();	
			logger.Debug(queryString);		
			ExecuteQuery(out ds,"UO",queryString);
		}

		/// <summary>
		/// Query #5 per il metodo "addNewCorrispondente"
		/// </summary>
		/// <param name="elUO"></param>
		/// <param name="db"></param>
		/// <param name="level"></param>
		/// <param name="tipoIE"></param>
		/// <param name="reg"></param>
		/// <param name="mailMitt"></param>
		/// <param name="descrizioneUO"></param>
		/// <param name="dataInizio"></param>
		/// <param name="idParent"></param>
		/// <param name="codiceAmm"></param>
		/// <param name="tipoCorr"></param>
		/// <param name="codiceAOO"></param>
		/// <param name="descrInterUO"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		public string addNewUO(XmlElement elUO,/*DocsPaWS.Utils.Database db,*/int level,string tipoIE,DocsPaVO.utente.Registro reg,string mailMitt,string descrizioneUO,string idParent,string codiceAmm,string tipoCorr,string codiceAOO,string descrInterUO)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPACorrGlobali5");
			string dataInizio = DocsPaDbManagement.Functions.Functions.ToDate(System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", new CultureInfo("en-US") ));

			q.setParam("param1",DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
			//DocsPaWS.Utils.dbControl.GetSystemIdColName());	
			if(isLastUO(elUO))
			{				
				q.setParam("param2",",VAR_EMAIL,VAR_DESC_CORR_OLD");
                q.setParam("param16", ",'" + mailMitt + "','" + descrizioneUO.Replace("'", "''") + "'");
			}			
			q.setParam("param3",DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_CORR_GLOBALI"));
			//DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_CORR_GLOBALI"));	
			q.setParam("param4",level.ToString()+",");
			q.setParam("param5","'"+tipoIE+"',");
			q.setParam("param6",reg.systemId+",");
			q.setParam("param7",reg.idAmministrazione+",");
			q.setParam("param8","'"+codiceAmm+"',");
			q.setParam("param9","'"+descrizioneUO.Replace("'","''")+"',");
			q.setParam("param10",dataInizio+",");
			q.setParam("param11",idParent+",");
			q.setParam("param12","'"+tipoCorr+"',");
			q.setParam("param13","'"+getPA(elUO)+"',");
			q.setParam("param14","'"+codiceAOO+"',");
			q.setParam("param15","'INTEROP_"+descrInterUO.Replace("'","''")+"'");				

			string insertUO = q.getSQL();
			logger.Debug(insertUO);
			string sysId;
			InsertLocked (out sysId,insertUO,"DPA_CORR_GLOBALI");
			return sysId;
		}



        /// <summary>
        /// Query #5 per il metodo "addNewCorrispondente"
        /// </summary>
        /// <param name="elUO"></param>
        /// <param name="db"></param>
        /// <param name="level"></param>
        /// <param name="tipoIE"></param>
        /// <param name="reg"></param>
        /// <param name="mailMitt"></param>
        /// <param name="descrizioneUO"></param>
        /// <param name="dataInizio"></param>
        /// <param name="idParent"></param>
        /// <param name="codiceAmm"></param>
        /// <param name="tipoCorr"></param>
        /// <param name="codiceAOO"></param>
        /// <param name="descrInterUO"></param>
        /// <param name="debug"></param>
        /// <param name="codEdesc"></param>
        /// <returns></returns>
        public string addNewUOByCodEDesc(XmlElement elUO,/*DocsPaWS.Utils.Database db,*/int level, string tipoIE, DocsPaVO.utente.Registro reg, string mailMitt, string descrizioneUO, string idParent, string codiceAmm, string tipoCorr, string codiceAOO, string descrInterUO, string codEdesc)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPACorrGlobali5");
            string dataInizio = DocsPaDbManagement.Functions.Functions.ToDate(System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", new CultureInfo("en-US")));

            q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
            //DocsPaWS.Utils.dbControl.GetSystemIdColName());	
            if (isLastUO(elUO))
            {
                q.setParam("param2", ",VAR_EMAIL, COD_DESC_INTEROP,VAR_DESC_CORR_OLD");
                q.setParam("param16", ",'" + mailMitt + "'" + ",'" + DocsPaUtils.Functions.Functions.ReplaceApexes(codEdesc) + "','" + DocsPaUtils.Functions.Functions.ReplaceApexes(descrizioneUO) + "'");
            }
            else
            {
                q.setParam("param2", ",COD_DESC_INTEROP");
                q.setParam("param16", ",'" + DocsPaUtils.Functions.Functions.ReplaceApexes(codEdesc) + "'");
            }

            q.setParam("param3", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_CORR_GLOBALI"));
            //DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_CORR_GLOBALI"));	
            q.setParam("param4", level.ToString() + ",");
            q.setParam("param5", "'" + tipoIE + "',");
            q.setParam("param6", reg.systemId + ",");
            q.setParam("param7", reg.idAmministrazione + ",");
            q.setParam("param8", "'" + codiceAmm + "',");
            q.setParam("param9", "'" + descrizioneUO.Replace("'", "''") + "',");
            q.setParam("param10", dataInizio + ",");
            q.setParam("param11", idParent + ",");
            q.setParam("param12", "'" + tipoCorr + "',");
            q.setParam("param13", "'" + getPA(elUO) + "',");
            q.setParam("param14", "'" + codiceAOO + "',");
            q.setParam("param15", "'INTEROP_" + descrInterUO.Replace("'", "''") + "'");


            string insertUO = q.getSQL();
            logger.Debug(insertUO);
            string sysId;
            InsertLocked(out sysId, insertUO, "DPA_CORR_GLOBALI");
            return sysId;
        }


		/// <summary>
		/// Query #6 per il metodo "addNewCorrispondente"
		/// </summary>
		/// <param name="codRubricaUO"></param>
		/// <param name="sysId"></param>
		public void updCodRubr(string codRubricaUO,string sysId)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPACorrGlob6");
			q.setParam("param1",codRubricaUO);	
			q.setParam("param2",sysId);	
			string updateCodRubricaUO = q.getSQL();
			ExecuteNonQuery(updateCodRubricaUO);
		}


		/// <summary>
		/// Query per il metodo "getRagioneTrasm"
		/// </summary>
		/// <param name="ds"></param>
		public void getRagTrasm(out DataSet ds,string idAmm, String tipoRagione)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPARagTrasm2");			
			q.setParam("param1",idAmm);
            q.setParam("param2", tipoRagione);
			string queryString = q.getSQL();		
			ExecuteQuery(out ds,"RAGIONE",queryString);
		}

        /// <summary>
        /// Ricava i destinatari del protocollo in partenza dal quale è stato originato il predisposto
        /// in arrivo durante il processo di interoperabilità interna (interop_int_no_mail = "1")
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="idAmm"></param>
        public void getIdProtocolloUscitaOriginario(out DataSet ds, string numProto, string codRegistro, int anno)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("SELECT_ID_PROTOCOLLO_USCITA_ORIGINARIO");
            q.setParam("param1", numProto);
            q.setParam("param2", codRegistro);
            q.setParam("param3", anno.ToString());
            string queryString = q.getSQL();
            ExecuteQuery(out ds, "PROTOUSCITA", queryString);
        }

		/// <summary>
		/// Query per il metodo "getRuoliDestTrasm"
		/// </summary>
		/// <param name="ds"></param>
		/// <param name="reg"></param>
		public void getCorrRuoloFun(out DataSet ds,DocsPaVO.utente.Registro reg, string mailAddress)
		{

            //DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__TIPO_F_RUOLO__TIPO_FUNZIONE__TIPO_RUOLO__CORR_GLOBALI__L_RUOLO_REG_2");;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_RUOLI_VIS_CONSULTA_PEC"); ;
            q.setParam("idRegistro",reg.systemId);
            if (string.IsNullOrEmpty(mailAddress) &&
                System.Configuration.ConfigurationManager.AppSettings["INTEROP_INT_NO_MAIL"] != null &&
                System.Configuration.ConfigurationManager.AppSettings["INTEROP_INT_NO_MAIL"].ToString() != "0")
                q.setParam("email", " AND ( V.var_email_registro='' OR V.var_email_registro is null ) ");
            else if(!string.IsNullOrEmpty(mailAddress))
                q.setParam("email", " AND (V.var_email_registro='" + mailAddress + "' OR V.var_email_registro is null)");
               // q.setParam("email", " AND V.var_email_registro = '" + mailAddress + "' "); SAB la query non restituiva risultati per oracle
            /*
            //modifica
            string prau = "PRAU";
            if(reg.chaRF.Equals("1"))
                prau = prau+"_RF";
            q.setParam("prau",prau);
			// fine modifica
            */
            string queryString = q.getSQL();
            logger.Debug("query per ruoli per notifica interop: " + queryString);
            
			ExecuteQuery(out ds,"RUOLI",queryString);


            
		}

		/// <summary>
		/// Query per il metodo "updateUOMittente"
		/// </summary>
		/// <param name="uoMitt"></param>
		/// <param name="mailMittente"></param>
		public void updUOMitt(DocsPaVO.utente.UnitaOrganizzativa uoMitt,string mailMittente)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPACorrGlob7");
			q.setParam("param1",mailMittente);	
			q.setParam("param2",uoMitt.systemId);			
			string queryString = q.getSQL();	
			logger.Debug(queryString);
			ExecuteNonQuery(queryString);
		}

		/// <summary>
		/// Query per il metodo "updateCanalePref"
		/// </summary>
		/// <param name="ds"></param>
		/// <param name="uoMitt"></param>
		/// <param name="debug"></param>
		/// <param name="db"></param>
		public void updCanalPref(DataSet ds,DocsPaVO.utente.UnitaOrganizzativa uoMitt/*,DocsPaWS.Utils.Database db*/)
		{
            //Nuova Gestione
            //Si cercano le system_id dei possibili tipi di canale - mail o interop
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DocumentTypes2");
            string queryString = q.getSQL();
            logger.Debug(queryString);
            ExecuteQuery(ds, "TIPO_CAN", queryString);

            string idTipoCanale = string.Empty;
            string mailId = string.Empty;
            if (ds.Tables["TIPO_CAN"].Rows.Count != 0)
            {
                mailId = ds.Tables["TIPO_CAN"].Rows[0]["SYSTEM_ID"].ToString();
                if (ds.Tables["TIPO_CAN"].Rows.Count == 1)
                    idTipoCanale = " AND ID_DOCUMENTTYPE = " + ds.Tables["TIPO_CAN"].Rows[0]["SYSTEM_ID"].ToString();
                if (ds.Tables["TIPO_CAN"].Rows.Count == 2)
                    idTipoCanale = " AND (ID_DOCUMENTTYPE = " + ds.Tables["TIPO_CAN"].Rows[0]["SYSTEM_ID"].ToString() + " OR ID_DOCUMENTTYPE = " + ds.Tables["TIPO_CAN"].Rows[1]["SYSTEM_ID"].ToString() + ")";
            }

            //Si verifica se è già associato un tipo canale al corrispondente
            if (!string.IsNullOrEmpty(idTipoCanale))
            {
                DocsPaUtils.Query q2 = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPATCanaleCorr2");
                q2.setParam("param1", uoMitt.systemId);
                q2.setParam("param2", idTipoCanale);
                string queryString2 = q2.getSQL();
                logger.Debug(queryString2);
                ExecuteQuery(ds, "CANALI", queryString2);
            }

            //Se il corrispondente non ha un tipo canale lo associamo
            if (ds.Tables["CANALI"].Rows.Count == 0 && !string.IsNullOrEmpty(mailId))
            {
                DocsPaUtils.Query q4 = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPATCanCorr2");
                q4.setParam("param10", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                q4.setParam("param1", uoMitt.systemId);
                q4.setParam("param2", mailId);
                q4.setParam("param20", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(""));
                string queryString4 = q4.getSQL();
                logger.Debug(queryString4);
                ExecuteLockedNonQuery(queryString4);
            }

            //Vecchia gestione
	        /*
			//si trova l'id del canale mail
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DocumentTypes2");
			string queryString = q.getSQL();	
			logger.Debug(queryString);
			ExecuteQuery(ds,"TIPO_CAN",queryString);

			string mailId=ds.Tables["TIPO_CAN"].Rows[0]["SYSTEM_ID"].ToString();

			//si trova il canale mail per il corrispondente
			DocsPaUtils.Query q2 = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPATCanaleCorr2");
			q2.setParam("param1",uoMitt.systemId);	
			q2.setParam("param2",mailId);
			string queryString2 = q2.getSQL();
			logger.Debug(queryString2);
			ExecuteQuery(ds,"CANALI",queryString2);

			//db.beginTransaction();
			BeginTransaction();
			DocsPaUtils.Query q3 = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPATCanCorr2");
			q3.setParam("param1",uoMitt.systemId);
			string queryString3 = q3.getSQL();
			logger.Debug(queryString3);
			ExecuteLockedNonQuery(queryString3);

			if(ds.Tables["CANALI"].Rows.Count>0)
			{
				DocsPaUtils.Query q4 = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPATCanCorr3");
				q4.setParam("param1",uoMitt.systemId);
				q4.setParam("param2",mailId);
				string queryString4 = q4.getSQL();
				logger.Debug(queryString4);
				ExecuteLockedNonQuery(queryString4);
			}
			else
			{
				DocsPaUtils.Query q4 = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPATCanCorr2");
				q4.setParam("param10",DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
				q4.setParam("param1",uoMitt.systemId);
				q4.setParam("param2",mailId);
				q4.setParam("param20",DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(""));
				string queryString4 = q4.getSQL();
				logger.Debug(queryString4);
				ExecuteLockedNonQuery(queryString4);
			}

			//db.commitTransaction();
			CommitTransaction();
            */
		}

		#endregion

		#region DocsPaWS.interoperabilita.InteroperabilitaUtils (2)

		/// <summary>
		/// Query per il metodo "checkId"
		/// </summary>
		/// <param name="dataSet"></param>
		/// <param name="messageId"></param>
		/// <param name="debug"></param>
		public void getMailElab(out DataSet dataSet, string messageId,string id_registro)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAMailElab");
			q.setParam("param1","'" + messageId.Replace("'","''") + "'");
            q.setParam("idRegistro", id_registro);
			string queryString = q.getSQL();	
			logger.Debug(queryString);		
			ExecuteQuery(out dataSet,"MAIL",queryString);
		}

		/// <summary>
		/// Query per il metodo "mailElaborata"
		/// </summary>
		/// <param name="idMessage"></param>
		/// <param name="ragione"></param>
		/// <param name="debug"></param>
		public void insMailElab(string idMessage, string ragione)
		{
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPAMailElab");
			q.setParam("param1",DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
			//DocsPaWS.Utils.dbControl.GetSystemIdColName());	
			q.setParam("param2",DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_MAIL_ELABORATE"));
			//DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_MAIL_ELABORATE"));
			q.setParam("param3","'" + idMessage.Replace("'","''") + "',");
			q.setParam("param4","'"+ragione+"',");
			q.setParam("param5",DocsPaDbManagement.Functions.Functions.ToDate(System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", new CultureInfo("en-US") )));
		    //DocsPaWS.Utils.dbControl.toDate(System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", new CultureInfo("en-US") ),true));
            q.setParam("id_registro", ",null");
            q.setParam("docnumber", ", null");
            string insertString = q.getSQL();	
			logger.Debug(insertString);
			ExecuteNonQuery(insertString);			
		}

        public void insMailElab(string idMessage, string ragione, string id_registro, string docnumber, string email)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPAMailElab");
            q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
            q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_MAIL_ELABORATE"));
            q.setParam("param3", "'" + idMessage.Replace("'", "''") + "',");
            q.setParam("param4", "'" + ragione + "',");
            q.setParam("param5", DocsPaDbManagement.Functions.Functions.ToDate(System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", new CultureInfo("en-US")))+",");
            q.setParam("id_registro",id_registro);
            q.setParam("docnumber", string.IsNullOrEmpty(docnumber) ? ", null" : ", " + docnumber);
            q.setParam("email", string.IsNullOrEmpty(email) ? ", 'null'" : ", '" + email + "'");

            string insertString = q.getSQL();
            logger.Debug(insertString);
            ExecuteNonQuery(insertString);
        }

        public void CestinaPredisposto(string docnumber)
        {
            try
            {
                if (!string.IsNullOrEmpty(docnumber))
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROFILE_CHA_IN_CESTINO_DOC");

                    //Metto in cestino il documento principale
                    q.setParam("docnumber", "SYSTEM_ID =" + docnumber);
                    string queryString = q.getSQL();
                    logger.Debug(queryString);

                    if (ExecuteNonQuery(queryString))
                    {
                        //Metto in cestino tutti i suoi allegati
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROFILE_CHA_IN_CESTINO_DOC");
                        q.setParam("docnumber", "ID_DOCUMENTO_PRINCIPALE =" + docnumber);
                        queryString = q.getSQL();
                        ExecuteNonQuery(queryString);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("CestinaPredisposto: Errore durante la rimozione del predisposto");
            }
            
        }

        /// <summary>
        /// Restituisce i parametri della mail da utilizzare in ricezione/spedizione
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="reg"></param>
        public void getCampiReg(out DataSet ds, DocsPaVO.utente.Registro reg)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Interoperabilita > getCampiReg");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_PARAM_MAIL_REGISTRO_SEND");
            q.setParam("systemId", reg.systemId);
            q.setParam("casella", "'" + reg.email + "'");
            string queryString = q.getSQL();
            logger.Debug(queryString);
            ExecuteQuery(out ds, "REGISTRO", queryString);
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Interoperabilita > getCampiReg");
        }

        /// <summary>
        /// Restituisce il valore di VAR_SOLO_MAIL_PEC, VAR_MAIL_RIC_PENDENTI per una determinata casella di un determinato registro.
        /// A seconda di come sono settati i valori, le mail ricevute possono essere settate private.
        /// Per gestione pendenti tramite PEC
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="idRegistro"></param>
        /// <param name="casella"></param>
        public void getVarMailRicPendenti(out DataSet ds, string idRegistro, string casella)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Interoperabilita > getVarMailRicPendenti");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_MAIL_REGISTRO_VAR_MAIL_RIC_PENDENTE");
            q.setParam("idregistro", idRegistro);
            q.setParam("casella", "'" + casella + "'");
            string queryString = q.getSQL();
            logger.Debug(queryString);
            ExecuteQuery(out ds, "REGISTRO", queryString);
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Interoperabilita > getVarMailRicPendenti");
            
        }

        /// <summary>
        /// Controlla se un documento è stato ricevuto tramite una casella configurata per mantenere le mail ricevute come pendenti.
        /// Serve per il frontend.
        /// Per gestione pendenti tramite PEC
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="idDocument"></param>
        public bool getDocPECPendente(string idDocument)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Interoperabilita > getDocPECPendente");
            DataSet ds = new DataSet();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DOC_PEC_PENDENTE");
            q.setParam("idDocument", idDocument);
            string queryString = q.getSQL();
            logger.Debug(queryString);
            ExecuteQuery(out ds, "DOCUMENTO", queryString);
            bool retval = false;
            if (ds.Tables["DOCUMENTO"].Rows.Count > 0)
            {
                DataRow regRow = ds.Tables["DOCUMENTO"].Rows[0];
                if (regRow["VAR_SOLO_MAIL_PEC"].ToString() != "1")
                {
                    if (regRow["VAR_MAIL_RIC_PENDENTE"].ToString() == "1") retval = true;
                }
            }
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Interoperabilita > getDocPECPendente");
            return retval;
        }


		#endregion

        #region Spedizioni
        public ArrayList GetSpedizioni(string idProfile)
        {
            logger.Debug("getSpedizioni");
            ArrayList lista = new ArrayList();
            try
            {
                DataSet dataSet;
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_STATO_INVIO_ELENCO");
                q.setParam("param1", idProfile); 
                string queryString = q.getSQL();
                logger.Debug(queryString);
                ExecuteQuery(out dataSet, "SPEDIZIONI", queryString);
                foreach (DataRow dataRow in dataSet.Tables["SPEDIZIONI"].Rows)
                {
                    lista.Add(GetSpedizioni(dataRow));
                }
                dataSet.Dispose();
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                logger.Error("Errore nella gestione dei documenti (Query - GetSpedizioni)", e);
                throw new Exception("F_System");
            }
            return lista;
        }

        private DocsPaVO.StatoInvio.StatoInvio GetSpedizioni(DataRow dataRow)
        {
            DocsPaVO.StatoInvio.StatoInvio statoInvio = new DocsPaVO.StatoInvio.StatoInvio();
            statoInvio.indirizzo = dataRow["VAR_INDIRIZZO"].ToString();
            statoInvio.codiceAmm = dataRow["VAR_CODICE_AMM"].ToString();
            statoInvio.codiceAOO = dataRow["VAR_CODICE_AOO"].ToString();
            statoInvio.dataSpedizione = dataRow["DTA_SPEDIZIONE"].ToString();
            statoInvio.destinatario = dataRow["VAR_DESC_CORR"].ToString();
            //statoInvio.idCorrispondente = dataRow["ID_CORR_GLOBALE"].ToString();
            return statoInvio;
        }

        /// <summary>
        /// Reperimento della data dell'ultima trasmissione effettuata per il documento
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idProfile"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public DateTime GetDataUltimaSpedizioneEffettuata(DocsPaVO.utente.InfoUtente infoUtente, string idProfile, string idDest)
        {
            DateTime date = DateTime.MinValue;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_DATA_ULTIMA_SPEDIZIONE_DOCUMENTO_EFFETTUATA");
            queryDef.setParam("idProfile", idProfile);
            queryDef.setParam("destinatario", idDest);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DBProvider dbProvider = new DBProvider())
            {
                string field;
                dbProvider.ExecuteScalar(out field, commandText);
                DateTime.TryParse(field, out date);
            }

            return date;
        }

        public bool DocumentAlreadySent_Opt(string idDocument)
        {
            bool retval = false;
            DataSet ds = new DataSet();
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_DOCUMENTALREADY_SENT_OPT");
            queryDef.setParam("param1", idDocument);
            string commandText = queryDef.getSQL();
            logger.Debug(commandText);
            using (DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteQuery(out ds, "SPEDS", commandText);
                if (ds.Tables["SPEDS"].Rows.Count > 0)
                    retval = true;
            }

            return retval;
        }

        /// <summary>
        /// PEC 4 - requisito 5 - storico spedizioni
        /// Inserimento in storico spedizioni.
        /// </summary>
        /// <param name="idDocumento"></param>
        /// <param name="Corrispondente"></param>
        /// <param name="esito"></param>
        /// <param name="mezzo"></param>
        public bool InsertInStoricoSpedizioni(string idDocumento, string Corrispondente, string esito, string mailAddress, string idGruppo, string descCanPref, string mail_mittente, string idRegistroMailMittente)
        {
            try
            {
                bool retval = false;
                logger.DebugFormat("Insert in storico spedizioni, documento {0}, idCorrispondente {1}, esito {2}", idDocumento, Corrispondente, esito);
                // Il mezzo è da ricavare nella DPA_DOC_ARRIVO_PAR con corrispondente e iddocumento.
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_SEND_STO");
                queryDef.setParam("idProfile", idDocumento);
                queryDef.setParam("idCorrispondente", Corrispondente);
                queryDef.setParam("esito", "'" + esito.Replace("'", "''") + "'");
                // Modifica per l'inserimento nello storico. Utilizzo la stessa condizione della spedizione se un destinatario è IS
                if (string.IsNullOrEmpty(mailAddress) || (!string.IsNullOrEmpty(descCanPref) && descCanPref.Equals("Interoperabilità PITRE")))
                {
                    queryDef.setParam("mail", "N.A.");
                    queryDef.setParam("mail_mittente", "N.A.");
                    queryDef.setParam("id_reg_mail_mittente", "null");
                }
                else
                {
                    queryDef.setParam("mail", mailAddress);
                    queryDef.setParam("mail_mittente", mail_mittente);
                    if (string.IsNullOrEmpty(idRegistroMailMittente))
                    {
                        queryDef.setParam("id_reg_mail_mittente", "NULL");
                    }
                    else
                    {
                        queryDef.setParam("id_reg_mail_mittente", idRegistroMailMittente);
                    }
                }
                // Fix: prendo l'id_documenttype dalla tabella stessa cercando per descrizione, e non dalla DPA_DOC_ARRIVO_PAR
                // che non cambia quando cambio il mezzo di spedizione di un destinatario
                queryDef.setParam("canalePref", descCanPref);
                
                // PEC 4 requisito 4 - report spedizioni
                // Serve per i documenti spediti dal ruolo
                queryDef.setParam("idGruppoMittente", idGruppo);
                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                using (DBProvider dbProvider = new DBProvider())
                {

                    retval = dbProvider.ExecuteNonQuery(commandText);

                }
                return retval;
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Errore durante nell'inserimento dello storico. Messaggio: {0}", e.Message);
                logger.ErrorFormat("Stacktrace: {0}", e.StackTrace);
                return false;
            }

        }

        public ArrayList GetSendStoricoByIdDocument(string idDocument)
        {
            logger.DebugFormat("Query in storico spedizioni per il documento {0}", idDocument);
            ArrayList lista = new ArrayList();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_SEND_STO_BY_IDDOCUMENT");
                q.setParam("idDocument", idDocument);
                DataSet ds;
                string commandText = q.getSQL();
                logger.Debug(commandText);
                ExecuteQuery(out ds, "SPEDIZIONI", commandText);
                foreach (DataRow dataRow in ds.Tables["SPEDIZIONI"].Rows)
                {
                    lista.Add(GetElementiStoricoSpedizione(dataRow));
                }
                ds.Dispose();

                
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Errore durante nella query dello storico. Messaggio: {0}", e.Message);
                logger.ErrorFormat("Stacktrace: {0}", e.StackTrace);
            }
            return lista;

        }

        private DocsPaVO.Spedizione.ElStoricoSpedizioni GetElementiStoricoSpedizione(DataRow dataRow)
        {
            DocsPaVO.Spedizione.ElStoricoSpedizioni ess = new DocsPaVO.Spedizione.ElStoricoSpedizioni();

            ess.idDocument = dataRow["ID_DOCUMENT"].ToString();
            ess.OggettoDocumento = dataRow["OGGETTO_DOCUMENTO"].ToString();
            ess.Mezzo = dataRow["MEZZO"].ToString();
            ess.Esito = dataRow["ESITO"].ToString();
            ess.Corrispondente = dataRow["CORRISPONDENTE"].ToString();
            //ess.DataSpedizione = dataRow["DTA_SPEDIZIONE"].ToString();
            ess.Mail = dataRow["MAIL"].ToString();
            ess.Mail_mittente = (dataRow["MAIL_MITTENTE"] != null ? dataRow["MAIL_MITTENTE"].ToString() : "");
            DateTime date = DateTime.MinValue;
            DateTime.TryParse(dataRow["DTA_SPEDIZIONE"].ToString(), out date);
            ess.DataSpedizione = date.ToString("dd/MM/yyyy HH:mm:ss");
            ess.Id = (dataRow["ID"] != null ? dataRow["ID"].ToString() : "");
            ess.IdGroupSender = (dataRow["ID_GROUP_SENDER"] != null ? dataRow["ID_GROUP_SENDER"].ToString() : "");
            return ess;
        }

        public bool UpdateStoricoSpedizione(string idDoc, string Corrispondente, string idRuolo, string idRegMittente, string esito, bool errore)
        {
            try
            {
                bool retval = false;
                string commandText = "";
                if (!errore)
                {
                    logger.DebugFormat("Update storico spedizioni, documento {0}, idCorrispondente {1}, esito {2}", idDoc, Corrispondente, esito);
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_SEND_STO");
                    queryDef.setParam("idProfile", idDoc);
                    queryDef.setParam("idCorrispondente", Corrispondente);
                    queryDef.setParam("esito", "'" + esito.Replace("'", "''") + "'");
                    queryDef.setParam("idRuolo", idRuolo);
                    //queryDef.setParam("idRegMittente", idRegMittente);
                    commandText = queryDef.getSQL();
                }
                else
                {
                    logger.DebugFormat("Update storico spedizioni, errore generale, documento {0}, esito {2}", idDoc, esito);
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_SEND_STO_ERRORE");
                    queryDef.setParam("idProfile", idDoc);
                    queryDef.setParam("esito", "'" + esito.Replace("'", "''") + "'");
                    queryDef.setParam("idRuolo", idRuolo);
                    //queryDef.setParam("idRegMittente", idRegMittente);
                    commandText = queryDef.getSQL();
                }
                logger.Debug(commandText);

                using (DBProvider dbProvider = new DBProvider())
                {

                    retval = dbProvider.ExecuteNonQuery(commandText);

                }
                return retval;
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Errore durante nell'update dello storico. Messaggio: {0}", e.Message);
                logger.ErrorFormat("Stacktrace: {0}", e.StackTrace);
                return false;
            }
        }

        #endregion

        #region Metodi privati

        private static bool isLastUO(XmlElement elUO)
		{
			XmlNode elUoInt=elUO.SelectSingleNode("UnitaOrganizzativa");
			if(elUoInt==null)
			{
				//non ci sono unità interne. 
				return true;
			}
			else
			{
				XmlNode elDenUo=elUoInt.SelectSingleNode("Denominazione");
				if(elDenUo.InnerText.Trim().Equals(""))
				{
					return true;
				}
				else
				{
					return false;	
				};
			}		
		}

		private static string getPA(XmlElement elUO)
		{
			XmlNode elUoInt=elUO.SelectSingleNode("UnitaOrganizzativa");
			if(elUoInt==null)
			{
				//non ci sono ruoli interni, non ci sono unità interne. 
				return "1";
			}
			else
			{
				XmlNode elDenUo=elUoInt.SelectSingleNode("Denominazione");
				if(elDenUo.InnerText.Trim().Equals(""))
				{
					return "1";
				}
				else
				{
					return "0";	
				};
			}			
		}
		#endregion

        #region check mailbox manager

        /// <summary>
        /// Aggiunge un nuovo record nella DPA_CHECK_MAILBOX
        /// </summary>
        /// <param name="idJob"></param>
        /// <param name="idUser"></param>
        /// <param name="idRole"></param>
        /// <param name="idReg"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public void CreateCheckMailbox(string idJob, string idUser, string idRole, string idReg, string email, out string id)
        {
            logger.Debug("start > InsertCheckmailBox");
            BeginTransaction();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_CHECK_MAILBOX");
            q.setParam("idJob", idJob);
            q.setParam("idUser", idUser);
            q.setParam("idRole", idRole);
            q.setParam("idReg", idReg);
            q.setParam("mail", email);
            string queryString = q.getSQL();
            logger.Debug(queryString);
            try
            {
                if (ExecuteNonQuery(queryString))
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_CHECK_MAILBOX_ID");
                    q.setParam("idJob", idJob);
                    q.setParam("idUser", idUser);
                    q.setParam("idRole", idRole);
                    q.setParam("idReg", idReg);
                    q.setParam("mail", email);
                    queryString = q.getSQL();
                    logger.Debug(queryString);
                    if (ExecuteScalar(out id, queryString) && !string.IsNullOrEmpty(id))
                    {
                        CommitTransaction();
                    }
                    else
                    {
                        RollbackTransaction();
                        id = "0";
                    }
                }
                else
                {
                    RollbackTransaction();
                    id = "0";
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                logger.Error("Errore nella gestione della mailbox (Query - CreateCheckMailbox)", e);
                RollbackTransaction();
                id = "0";
            }
        }

        /// <summary>
        /// Invocato al termine dell'interrogazione della casella(imposta il campo concluded a 1)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ConcludedCheckMailbox(string id)
        {
            int res;
            logger.Debug("start > CompleteCheckMailBox");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_CHECK_MAILBOX");
            q.setParam("id", id);
            q.setParam("param1", "CONCLUDED = '1', DTA_CONCLUDED = " + DocsPaDbManagement.Functions.Functions.GetDate(true));
            string queryString = q.getSQL();
            logger.Debug(queryString);
            try
            {
                //ExecuteScalar(out res, queryString);
                //if (Convert.ToInt32(res) > 0)
                //{
                //    return true;
                //}
                //else
                //    return false;
                ExecuteNonQuery(queryString, out res);
                if (res > 0)
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                logger.Error("Errore nella gestione della mailbox (Query - ConcludedCheckMailbox)", e);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mailUserId"></param>
        /// <param name="errorMessage"></param>
        /// <param name="mailServer"></param>
        /// <returns></returns>
        public bool UpdateInfoCheckMailbox(string id, string mailUserId, string errorMessage, string mailServer)
        {
            int res;
            logger.Debug("start > UpdateInfoCheckMailbox");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_CHECK_MAILBOX");
            q.setParam("id", id);
            q.setParam("param1", "MAILUSERID = '" + mailUserId + "', ERRORMESSAGE = '" + errorMessage.Replace("'", "''") + "', MAILSERVER = '" + mailServer + "'");
            string queryString = q.getSQL();
            logger.Debug(queryString);
            try
            {
                //ExecuteScalar(out res, queryString);
                //if (Convert.ToInt32(res) > 0)
                //{
                //    return true;
                //}
                //else
                //    return false;
                ExecuteNonQuery(queryString, out res);
                if (res > 0)
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                logger.Error("Errore nella gestione della mailbox (Query - UpdateInfoCheckMailbox)", e);
                return false;
            }
        }

        /// <summary>
        /// inserisce il numero totale di mail da elaborare
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool MailTotalCheckMailbox(string id, string total)
        {
            string res;
            logger.Debug("start > CompleteCheckMailBox");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_CHECK_MAILBOX");
            q.setParam("id", id);
            q.setParam("param1", "TOTAL = " + total);
            string queryString = q.getSQL();
            logger.Debug(queryString);
            try
            {
                ExecuteScalar(out res, queryString);
                if (Convert.ToInt32(res) > 0)
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                logger.Error("Errore nella gestione della mailbox (Query - MailTotalCheckMailbox)", e);
                return false;
            }
        }

        /// <summary>
        ///aggiorna il valore mail processate durante l'interrogazione della casella
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool MailProcessedCheckMailbox(string id, string processed)
        {
            logger.Debug("start > CompleteCheckMailBox");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_CHECK_MAILBOX");
            q.setParam("id", id);
            q.setParam("param1", "ELABORATE = " + processed);
            string queryString = q.getSQL();
            logger.Debug(queryString);
            try
            {
                if (ExecuteNonQuery(queryString))
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                logger.Error("Errore nella gestione della mailbox (Query - MailProcessedCheckMailbox)", e);
                return false;
            }
        }

        /// <summary>
        /// Aggiunge ile informazioni sul
        /// </summary>
        /// <param name="mailprocessed"></param>
        /// <returns></returns>
        public bool CreateCheckMailboxReport(string idCheckMailbox, DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed mailprocessed)
        {
            logger.Debug("start > CreateCheckMailboxReport");
            mailprocessed = (DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed)DocsPaUtils.Functions.Functions.XML_Serialization_Deserialization_By_Encode(mailprocessed, typeof(DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed), null, System.Text.Encoding.UTF8);
            string processedType;
            string receipt;
            switch (mailprocessed.ProcessedType)
            { 
                case DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.ConfirmReception:
                    processedType = "ConfirmReception";
                    break;
                case DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.DatiCert:
                    processedType = "DatiCert";
                    break;
                case DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.Eccezione:
                    processedType = "Eccezione";
                    break;
                case DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.NonPEC:
                    processedType = "NonPEC";
                    break;
                case DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.NotifyCancellation:
                    processedType = "NotifyCancellation";
                    break;
                case DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.Pec:
                    processedType = "Pec";
                    break;
                case DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.Signature:
                    processedType = "Signature";
                    break;
                default:
                    processedType = string.Empty;
                    break;
            }

            switch (mailprocessed.PecXRicevuta)
            { 
                case DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.Delivery_Status_Notification:
                    receipt = "Delivery_Status_Notification";
                    break;
                case DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.From_Non_PEC:
                    receipt = "From_Non_PEC";
                    break;
                case DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Accept_Notify:
                    receipt = "PEC_Accept_Notify";
                    break;
                case DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Alert_Virus:
                    receipt = "PEC_Alert_Virus";
                    break;
                case DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Contain_Virus:
                    receipt = "PEC_Contain_Virus";
                    break;
                case DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Delivered:
                    receipt = "PEC_Delivered";
                    break;
                case DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Delivered_Notify:
                    receipt = "PEC_Delivered_Notify";
                    break;
                case DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Delivered_Notify_Short:
                    receipt = "PEC_Delivered_Notify_Short";
                    break;
                case DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Error:
                    receipt = "PEC_Error";
                    break;
                case DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Error_Delivered_Notify_By_Virus:
                    receipt = "PEC_Error_Delivered_Notify_By_Virus";
                    break;
                case DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Error_Preavviso_Delivered_Notify:
                    receipt = "PEC_Error_Preavviso_Delivered_Notify";
                    break;
                case DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Mancata_Consegna:
                    receipt = "PEC_Mancata_Consegna";
                    break;
                case DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_NO_XRicevuta:
                    receipt = "PEC_NO_XRicevuta";
                    break;
                case DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Non_Accept_Notify:
                    receipt = "PEC_Non_Accept_Notify";
                    break;
                case DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Presa_In_Carico:
                    receipt = "PEC_Presa_In_Carico";
                    break;
                case DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.unknown:
                    receipt = "unknown";
                    break;
                default:
                    receipt = string.Empty;
                    break;
            }

            BeginTransaction();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_REPORT_MAILBOX");
            string values = "";
            values = DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_REPORT_MAILBOX") + " " +
                idCheckMailbox + ", '" +
                mailprocessed.MailID.ToString().Replace("'", "''") + "', '" +
                processedType.Replace("'", "''") + "', '" +
                 receipt.Replace("'", "''") + "'," + DocsPaDbManagement.Functions.Functions.ToDate(mailprocessed.Date.ToShortDateString() + " " + mailprocessed.Date.ToShortTimeString())
                  + " , '" +
                mailprocessed.From.Replace("'", "''") + "', '" +
                mailprocessed.ErrorMessage.Replace("'","''") + "', " + 
                mailprocessed.CountAttatchments +
                ", '" + mailprocessed.Subject.Replace("'", "''") + "'";
            q.setParam("param1", values);
            string queryString = q.getSQL();
            logger.Debug(queryString);
            try
            {
                if(ExecuteNonQuery(queryString))
                {
                    CommitTransaction();
                    return true;
                }
                else
                {
                    RollbackTransaction();
                    return false;
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                logger.Error("Errore nella gestione della mailbox (Query - CreateCheckMailboxReport)", e);
                RollbackTransaction();
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void CreateJobs(out string id)
        {
            logger.Debug("start > CreateJobs");
            BeginTransaction();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_JOBS");
            string queryString = q.getSQL();
            logger.Debug(queryString);
            try
            {
                if (ExecuteNonQuery(queryString))
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_JOBS");
                    queryString = q.getSQL();
                    logger.Debug(queryString);
                    if (ExecuteScalar(out id, queryString) && !string.IsNullOrEmpty(id))
                    {
                        CommitTransaction();
                    }
                    else
                    {
                        RollbackTransaction();
                        id = "0";
                    }
                }
                else
                {
                    RollbackTransaction();
                    id = "0";
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                logger.Error("Errore nella gestione del job (Query - CreateJobs)", e);
                RollbackTransaction();
                id = "0";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listEmails"></param>
        /// <returns></returns>
        public List<DocsPaVO.Interoperabilita.InfoCheckMailbox> InfoCheckMailbox(List<string> listEmails)
        {
            try
            {
                logger.Debug("start > InfoCheckMailbox");
                List<DocsPaVO.Interoperabilita.InfoCheckMailbox> listResult = new List<DocsPaVO.Interoperabilita.InfoCheckMailbox>();
                string where = string.Empty;
                string or = " OR ";
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_CHECK_MAILBOX_WHERE_CONDITION");
                DataSet ds = new DataSet();
                if (listEmails.Count > 0)
                {
                    where += "MAIL = '" + listEmails[0] + "' ";
                    for (int i = 1; i < listEmails.Count; i++)
                    {
                        where += or + "MAIL = '" + listEmails[i] + "'";
                    }
                    q.setParam("whereCondition", where);
                    q.setParam("fields", "ID, IDUSER, IDROLE, IDREG, MAIL, ELABORATE, TOTAL, CONCLUDED");
                    string queryString = q.getSQL();
                    ExecuteQuery(ds, "CheckMailbox",  queryString);
                    if (ds.Tables["CheckMailbox"] != null && ds.Tables["CheckMailbox"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["Checkmailbox"].Rows)
                        {
                            DocsPaVO.Interoperabilita.InfoCheckMailbox checkMailbox = new DocsPaVO.Interoperabilita.InfoCheckMailbox();
                            checkMailbox.IdCheckMailbox = row["ID"].ToString();
                            checkMailbox.UserID = row["IDUSER"].ToString();
                            checkMailbox.RoleID = row["IDROLE"].ToString();
                            checkMailbox.RegisterID = row["IDREG"].ToString();
                            checkMailbox.Mail = row["MAIL"].ToString();
                            checkMailbox.Elaborate = Convert.ToInt32(row["ELABORATE"].ToString());
                            checkMailbox.Total = Convert.ToInt32(row["TOTAL"].ToString());
                            checkMailbox.Concluded = row["CONCLUDED"].ToString();
                            listResult.Add(checkMailbox);
                        }
                    }
                    logger.Debug(queryString);
                }

                return listResult;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                logger.Error("Errore nella gestione della mailbox (Query - InfoCheckMailbox)", e);
                return new List<DocsPaVO.Interoperabilita.InfoCheckMailbox>();
            }
        }

        /// <summary>
        /// Q1uery eseguita dopo la consultazione del report
        /// Elimina il record con ID = idCheckMailbox dalla DPA_CHECK_MAILBOX ed in cascata i record 
        /// della DPA_REPORT_MAILBOX in relazione con DPA_CHECK_MAILBOX
        /// </summary>
        /// <param name="idCheckMailbox"></param>
        /// <returns></returns>
        public bool RemoveReportMailbox(string idCheckMailbox)
        {
            bool res = false;
            try
            {
                logger.Debug("start > RemoveReportMailbox");
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_CHECK_MAILBOX");
                q.setParam("idCheckMailbox", idCheckMailbox);
                string query = q.getSQL();
                res = ExecuteNonQuery(query);
                return res;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                logger.Error("Errore nella gestione della mailbox (Query - RemoveReportMailbox)", e);
                return false;
            }
        }

        /// <summary>
        /// Restituisce il record della DPA_CHECK_MAILBOX e quelli associati ad esso in DPA_REPORT_MAILBOX
        /// </summary>
        /// <param name="idCheckMailbox"></param>
        /// <returns></returns>
        public DocsPaVO.Interoperabilita.MailAccountCheckResponse InfoReportMailbox(string idCheckMailbox)
        {
            try
            {
                logger.Debug("start > InfoReportMailbox");
                DocsPaVO.Interoperabilita.MailAccountCheckResponse mailAccountResponse = new DocsPaVO.Interoperabilita.MailAccountCheckResponse();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_CHECK_MAILBOX");
                DataSet ds = new DataSet();
                q.setParam("id", idCheckMailbox);
                string queryString = q.getSQL();
                ExecuteQuery(ds, "CheckMailbox", queryString);
                if (ds.Tables["CheckMailbox"] != null && ds.Tables["CheckMailbox"].Rows.Count > 0)
                {
                    DataRow row = ds.Tables["CheckMailbox"].Rows[0];
                    mailAccountResponse.Registro = row["registerCode"].ToString();
                    mailAccountResponse.MailAddress = row["mail"].ToString();
                    mailAccountResponse.MailServer = row["mailserver"].ToString();
                    mailAccountResponse.MailUserID = row["mailuserid"].ToString();
                    mailAccountResponse.ErrorMessage = row["errormessage"].ToString();
                    mailAccountResponse.DtaConcluded = Convert.ToDateTime(row["DTA_CONCLUDED"].ToString());

                    ds = new DataSet();
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_REPORT_MAILBOX");
                    q.setParam("id", idCheckMailbox);
                    queryString = q.getSQL();
                    ExecuteQuery(ds, "CheckMailbox", queryString);
                    if (ds.Tables["CheckMailbox"] != null && ds.Tables["CheckMailbox"].Rows.Count > 0)
                    {
                        foreach (DataRow r in ds.Tables["CheckMailbox"].Rows)
                        {
                            DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed mailprocessed =
                                new DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed();
                            mailprocessed.Subject = r["subject"].ToString();
                            mailprocessed.Date = Convert.ToDateTime(r["date_mail"].ToString());
                            mailprocessed.MailID = r["mailid"].ToString();
                            mailprocessed.From = r["from_mail"].ToString();
                            mailprocessed.CountAttatchments = Convert.ToInt32(r["count_attachments"].ToString());
                            mailprocessed.ErrorMessage = r["error"].ToString();
                            switch (r["type"].ToString())
                            {
                                case "ConfirmReception":
                                    mailprocessed.ProcessedType = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.ConfirmReception;
                                    break;
                                case "DatiCert":
                                    mailprocessed.ProcessedType = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.DatiCert;
                                    break;
                                case "Eccezione":
                                    mailprocessed.ProcessedType = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.Eccezione;
                                    break;
                                case "NonPEC":
                                    mailprocessed.ProcessedType = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.NonPEC;
                                    break;
                                case "NotifyCancellation":
                                    mailprocessed.ProcessedType = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.NotifyCancellation;
                                    break;
                                case "Pec":
                                    mailprocessed.ProcessedType = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.Pec;
                                    break;
                                case "Signature":
                                    mailprocessed.ProcessedType = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.Signature;
                                    break;
                            }
                            switch (r["receipt"].ToString())
                            {
                                case "Delivery_Status_Notification":
                                    mailprocessed.PecXRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.Delivery_Status_Notification;
                                    break;
                                case "From_Non_PEC":
                                    mailprocessed.PecXRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.From_Non_PEC;
                                    break;
                                case "PEC_Accept_Notify":
                                    mailprocessed.PecXRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Accept_Notify;
                                    break;
                                case "PEC_Alert_Virus":
                                    mailprocessed.PecXRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Alert_Virus;
                                    break;
                                case "PEC_Contain_Virus":
                                    mailprocessed.PecXRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Contain_Virus;
                                    break;
                                case "PEC_Delivered":
                                    mailprocessed.PecXRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Delivered;
                                    break;
                                case "PEC_Delivered_Notify":
                                    mailprocessed.PecXRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Delivered_Notify;
                                    break;
                                case "PEC_Delivered_Notify_Short":
                                    mailprocessed.PecXRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Delivered_Notify_Short;
                                    break;
                                case "PEC_Error":
                                    mailprocessed.PecXRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Error;
                                    break;
                                case "PEC_Error_Delivered_Notify_By_Virus":
                                    mailprocessed.PecXRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Error_Delivered_Notify_By_Virus;
                                    break;
                                case "PEC_Error_Preavviso_Delivered_Notify":
                                    mailprocessed.PecXRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Error_Preavviso_Delivered_Notify;
                                    break;
                                case "PEC_Mancata_Consegna":
                                    mailprocessed.PecXRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Mancata_Consegna;
                                    break;
                                case "PEC_NO_XRicevuta":
                                    mailprocessed.PecXRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_NO_XRicevuta;
                                    break;
                                case "PEC_Non_Accept_Notify":
                                    mailprocessed.PecXRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Non_Accept_Notify;
                                    break;
                                case "PEC_Presa_In_Carico":
                                    mailprocessed.PecXRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Presa_In_Carico;
                                    break;
                                case "unknown":
                                    mailprocessed.PecXRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.unknown;
                                    break;
                            }
                            mailAccountResponse.MailProcessedList.Add(mailprocessed);
                        }
                    }
                }
                return mailAccountResponse;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                logger.Error("Errore nella gestione della mailbox (Query - InfoReportMailbox)", e);
                return null;
            }
        }

       

        #region Report Spedizioni

        /// <summary>
        /// Lista Report delle spedizioni
        /// </summary>
        /// <param name="filters">Filtri delle spedizioni</param>
        /// <returns></returns>
        public List<InfoDocumentoSpedito> GetReportSpedizioni(FiltriReportSpedizioni filters, string idPeople, string idGruppo)
        {
            try
            {

                logger.Debug("START : DocsPaDB > Query_DocsPAWS > Interoperabilita > getReportSpedizioni");
                // recupero elenco documenti spediti
                DataSet dsDocSpediti = new DataSet();
                DocsPaUtils.Query qDocSpediti = DocsPaUtils.InitQuery.getInstance().getQuery("GET_REPORT_SPEDIZIONI_MASTER");
                qDocSpediti.setParam("filters", GetReportSpedizioniQueryFilters(filters,idGruppo,true));
                qDocSpediti.setParam("filtersEsito", GetReportSpedizioniQueryFiltersEsito(filters));
                // Controllo della visibilità nella query
                qDocSpediti.setParam("idPeople", idPeople);
                qDocSpediti.setParam("idGruppo",idGruppo);
                string qSDocSpediti = qDocSpediti.getSQL();
                logger.Debug(qSDocSpediti);
                ExecuteQuery(out dsDocSpediti, "DocSpediti", qSDocSpediti);

                //recupero elenco spedizioni
                DataSet dsSpedizioni = new DataSet();
                DocsPaUtils.Query qSpedizioni = DocsPaUtils.InitQuery.getInstance().getQuery("GET_REPORT_SPEDIZIONI_DETAILS");
                qSpedizioni.setParam("filters", GetReportSpedizioniQueryFilters(filters,idGruppo,false));
                string qSSpedizioni = qSpedizioni.getSQL();
                logger.Debug(qSSpedizioni);
                ExecuteQuery(out dsSpedizioni, "Spedizioni", qSSpedizioni);

                //mapping dati 
                List<InfoDocumentoSpedito> listDocSpediti = new List<InfoDocumentoSpedito>();
                if (dsDocSpediti.Tables["DocSpediti"].Rows.Count > 0)
                {
                    foreach (DataRow rowDoc in dsDocSpediti.Tables["DocSpediti"].Rows)
                    {
                        InfoDocumentoSpedito docSpedito = new InfoDocumentoSpedito();
                        List<InfoSpedizione> listSpedizioni = new List<InfoSpedizione>();
                        //add info documento
                        docSpedito.InfoSpedizione = InfoDocumentoSpedito.TipoInfoSpedizione.Effettuato;// default value alert azione
                        //docSpedito.IDDocumento = rowDoc["IDDOCUMENT"].ToString();
                        foreach (DataRow rowSpedizioni in dsSpedizioni.Tables["Spedizioni"].Rows)
                        {
                            if (rowDoc["IDDOCUMENT"].ToString().Equals(rowSpedizioni["IDDOCUMENT"].ToString()))
                            {
                                docSpedito.IDDocumento = rowSpedizioni["IDDOCUMENT"].ToString();
                                docSpedito.Protocollo = rowSpedizioni["PROTOCOLLO"].ToString();
                                docSpedito.DescrizioneDocumento = rowSpedizioni["OGGETTO"].ToString();
                                // add info spedizione
                                InfoSpedizione spedizione = new InfoSpedizione();
                                spedizione.MezzoSpedizione = rowSpedizioni["MEZZO"].ToString();
                                spedizione.NominativoDestinatario = rowSpedizioni["DESTINATARIO"].ToString();
                                spedizione.EMailDestinatario = rowSpedizioni["MAIL"].ToString();

                                spedizione.EMailMittente = (rowSpedizioni["MAIL_MITTENTE"] != null ? rowSpedizioni["MAIL_MITTENTE"].ToString() : "");
                                
                                if (spedizione.MezzoSpedizione == "Interoperabilità PITRE" || spedizione.EMailDestinatario.Contains("InteroperabilityService.svc"))
                                {
                                    spedizione.EMailDestinatario = "N.A.";
                                    spedizione.EMailMittente = "N.A.";
                                }

                                if (string.IsNullOrEmpty(rowSpedizioni["TIPO_DEST"].ToString()))
                                    spedizione.TipoDestinatario = "";
                                else
                                {
                                    if (rowSpedizioni["TIPO_DEST"].ToString() == "D" || rowSpedizioni["TIPO_DEST"].ToString() == "F" || rowSpedizioni["TIPO_DEST"].ToString() == "L")
                                        spedizione.TipoDestinatario = "D";
                                    else if (rowSpedizioni["TIPO_DEST"].ToString() == "C")
                                        spedizione.TipoDestinatario = "CC";
                                    else spedizione.TipoDestinatario = "";
                                }
                                spedizione.DataSpedizione = rowSpedizioni["DATASPEDIZIONE"].ToString();
                                if (!string.IsNullOrEmpty(rowSpedizioni["MASK"].ToString()))
                                {
                                    spedizione.Azione_Info = GetStatoAzione(rowSpedizioni["MASK"].ToString().Substring(0, 1));
                                    // set alert 
                                    if (spedizione.Azione_Info == InfoSpedizione.TipologiaAzione.Attendere && docSpedito.InfoSpedizione != InfoDocumentoSpedito.TipoInfoSpedizione.Errore) docSpedito.InfoSpedizione = InfoDocumentoSpedito.TipoInfoSpedizione.Warning;
                                    if (spedizione.Azione_Info == InfoSpedizione.TipologiaAzione.Rispedire) docSpedito.InfoSpedizione = InfoDocumentoSpedito.TipoInfoSpedizione.Errore;
                                    spedizione.TipoRicevuta_Accettazione = GetStatoRicevuta(rowSpedizioni["MASK"].ToString().Substring(1, 1));
                                    spedizione.TipoRicevuta_Consegna = GetStatoRicevuta(rowSpedizioni["MASK"].ToString().Substring(2, 1));
                                    spedizione.TipoRicevuta_Conferma = GetStatoRicevuta(rowSpedizioni["MASK"].ToString().Substring(3, 1));
                                    spedizione.TipoRicevuta_Annullamento = GetStatoRicevuta(rowSpedizioni["MASK"].ToString().Substring(4, 1));
                                    spedizione.TipoRicevuta_Eccezione = GetStatoRicevuta(rowSpedizioni["MASK"].ToString().Substring(5, 1));
                                }

                                spedizione.ProtocolloDestinatario = (rowSpedizioni["PROTO_DEST"] != null ? rowSpedizioni["PROTO_DEST"].ToString() : string.Empty);
                                spedizione.DataProtDest = (rowSpedizioni["DATA_PROTO_DEST"] != null ? rowSpedizioni["DATA_PROTO_DEST"].ToString() : string.Empty);
                                spedizione.MotivoAnnullEccezione = (rowSpedizioni["MOTIVO_ANNULLA"] != null ? rowSpedizioni["MOTIVO_ANNULLA"].ToString() : string.Empty);
                                
                                // Problema associazione valori sbagliati Statusmask
                                if (spedizione.MezzoSpedizione != "INTEROPERABILITA" && spedizione.MezzoSpedizione != "Interoperabilità PITRE")
                                {
                                    spedizione.TipoRicevuta_Annullamento = InfoSpedizione.TipologiaStatoRicevuta.AttendereCausaMezzo;
                                    spedizione.TipoRicevuta_Conferma = InfoSpedizione.TipologiaStatoRicevuta.AttendereCausaMezzo;

                                }
                                else
                                {
                                    if ((string.IsNullOrEmpty(spedizione.DataProtDest) || string.IsNullOrEmpty(spedizione.ProtocolloDestinatario))
                                        && (spedizione.Azione_Info == InfoSpedizione.TipologiaAzione.OK || spedizione.TipoRicevuta_Conferma == InfoSpedizione.TipologiaStatoRicevuta.OK))
                                    {
                                        spedizione.Azione_Info = InfoSpedizione.TipologiaAzione.Attendere;
                                        spedizione.TipoRicevuta_Conferma = InfoSpedizione.TipologiaStatoRicevuta.Attendere;
                                        spedizione.TipoRicevuta_Annullamento = InfoSpedizione.TipologiaStatoRicevuta.Attendere;
                                    }
                                }

                                listSpedizioni.Add(spedizione);
                            }
                        }
                        docSpedito.Spedizioni = listSpedizioni;
                        listDocSpediti.Add(docSpedito);
                    }
                }

                logger.Debug("END : DocsPaDB > Query_DocsPAWS > Interoperabilita > getReportSpedizioni");
                return listDocSpediti;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private DocsPaVO.Spedizione.InfoSpedizione.TipologiaStatoRicevuta GetStatoRicevuta(string charMask)
        {
            /*
             * Spiegazione funzionamento campo status_c_mask
             * Nella tabella dpa_stato_invio è presente un campo di 7 caratteri denominato status_c_mask. I caratteri indicano
             * Essi possono assumere i valori A (Attendere), V (OK), X (KO), N(Non prevista)
             * 1 - Esito generale
             * Indica se una spedizione ha raggiunto l'esito finale, se è in stato di attesa o se ha avuto un errore. Per le spedizioni via mail
             * basta l'accettazione per esito positivo, per le mail certificate serve la consegna, per i corrispondenti interoperabili serve la 
             * conferma. Viene controllata dal metodo GetStatoAzione sottostante.
             * 2 - Ricevuta di accettazione
             * Mail inviata dal provider nel caso di accettazione. Prevista per mail, PEC, PEC Interoperante
             * 3 - Ricevuta di consegna
             * Ricevuta che indica la consegna al destinatario. Prevista per PEC, PEC Int., Int.Semplificata.
             * 4 - Ricevuta di conferma
             * Indica se il destinatario ha protocollato il documento spedito. Prevista per PEC Int. e Int. Semplificata
             * 5 - Ricevuta di annullamento
             * Nel caso il documento protocollato venga annullato
             * 6 - Eccezione
             * Nel caso ci siano degli errori per i destinatari interoperanti
             * 7 - Con errori
             * Questo riguarda gli errori generici come DNS etc.
             */

            switch (charMask)
            {
                case "A": return DocsPaVO.Spedizione.InfoSpedizione.TipologiaStatoRicevuta.Attendere;
                case "V": return DocsPaVO.Spedizione.InfoSpedizione.TipologiaStatoRicevuta.OK;
                case "X": return DocsPaVO.Spedizione.InfoSpedizione.TipologiaStatoRicevuta.KO;
                case "N": return DocsPaVO.Spedizione.InfoSpedizione.TipologiaStatoRicevuta.AttendereCausaMezzo;
                default: return InfoSpedizione.TipologiaStatoRicevuta.Nessuna;
            }
        }

        private DocsPaVO.Spedizione.InfoSpedizione.TipologiaAzione GetStatoAzione(string charMask)
        {
            switch (charMask)
            {
                case "A": return InfoSpedizione.TipologiaAzione.Attendere;
                case "V": return InfoSpedizione.TipologiaAzione.OK;
                case "X": return InfoSpedizione.TipologiaAzione.Rispedire;
                default: return InfoSpedizione.TipologiaAzione.Nessuna;
            }
        }

        private string GetReportSpedizioniQueryFilters(FiltriReportSpedizioni filters,string idGruppo, bool allReceivers)
        {
            // METODI PER ORACLE - sono da fare per sql
            string resultQueryFilters = string.Empty;
            string resultQueryFiltersTipoRicevuta = string.Empty;

            if (string.IsNullOrEmpty(filters.IdDocumento))
            {
                // filtri data 
                switch (filters.FiltroData)
                {
                    case FiltriReportSpedizioni.TipoFiltroData.Intervallo:
                    case FiltriReportSpedizioni.TipoFiltroData.MeseCorrente:
                    case FiltriReportSpedizioni.TipoFiltroData.SettimanaCorrente:
                        if (dbType.ToUpper() == "ORACLE")
                        {
                            resultQueryFilters += string.Format("AND TRUNC(A.DTA_SPEDIZIONE) >= to_date('{0:dd-MM-yyyy}','dd-mm-yyyy') AND  TRUNC(A.DTA_SPEDIZIONE) <= to_date('{1:dd-MM-yyyy}','dd-mm-yyyy')", filters.DataDa, filters.DataA);
                        }
                        else if (dbType.ToUpper() == "SQL")
                        {
                            resultQueryFilters += string.Format("AND A.DTA_SPEDIZIONE >= {0} AND  A.DTA_SPEDIZIONE <= {1}", DocsPaDbManagement.Functions.Functions.ToDateBetween(filters.DataDa.ToShortDateString(), true), DocsPaDbManagement.Functions.Functions.ToDateBetween(filters.DataA.ToShortDateString(), false));
                                            
                        }
                        break;
                    case FiltriReportSpedizioni.TipoFiltroData.Oggi:
                    case FiltriReportSpedizioni.TipoFiltroData.ValoreSingolo:
                        if (dbType.ToUpper() == "ORACLE")
                        {
                            resultQueryFilters += string.Format(" AND TRUNC(A.DTA_SPEDIZIONE) = to_date('{0:dd-MM-yyyy}','dd-mm-yyyy')", filters.DataDa, filters.DataA);
                        }
                        else if (dbType.ToUpper() == "SQL")
                        {
                            resultQueryFilters += string.Format("AND A.DTA_SPEDIZIONE >= {0} AND  A.DTA_SPEDIZIONE <= {1}", DocsPaDbManagement.Functions.Functions.ToDateBetween(filters.DataDa.ToShortDateString(), true), DocsPaDbManagement.Functions.Functions.ToDateBetween(filters.DataDa.ToShortDateString(), false));
                            
                        }
                            break;
                }

                //filtro ruolo
                if (filters.VisibilitaDoc == FiltriReportSpedizioni.TipoVisibilitaDocumenti.AllDocByRuolo)
                    resultQueryFilters += " AND a.ID_PROFILE IN (SELECT ID_PROFILE FROM DPA_SEND_STO k WHERE k.ID_GROUP_SENDER=" + idGruppo + " and k.esito='Spedito' and k.dta_spedizione>=a.dta_spedizione and k.id_profile=a.id_profile and k.id_corr_globale=a.id_corr_globale)";

                
            }

            // Modifica per il requisito 4.
            // Devono essere visualizzati tutti i destinatari per documento, anche per calcolare l'esito corretto.

            if (allReceivers)
            {
                // filtro registro e mail mittente
                if (!string.IsNullOrEmpty(filters.IdRegMailMittente) && !string.IsNullOrEmpty(filters.MailMittente))
                {
                    resultQueryFilters += " AND a.ID_PROFILE IN (SELECT ID_PROFILE FROM DPA_SEND_STO k WHERE k.MAIL_MITTENTE='" + filters.MailMittente + "' and k.id_reg_mail_mittente=" + filters.IdRegMailMittente + " and k.esito='Spedito' and k.dta_spedizione>=a.dta_spedizione and k.id_profile=a.id_profile and k.id_corr_globale=a.id_corr_globale)";

                }

                if (dbType.ToUpper() == "ORACLE")
                {
                    //filtro tipo ricevuta - Avvenuta Accettazione
                    if (filters.TipoRicevuta_Accettazione)
                        resultQueryFiltersTipoRicevuta += " SUBSTR(A.STATUS_C_MASK,2,1) = 'V' OR";

                    //filtro tipo ricevuta - Mancata Accettazione
                    if (filters.TipoRicevuta_MancataAccettazione)
                        resultQueryFiltersTipoRicevuta += " SUBSTR(A.STATUS_C_MASK,2,1) = 'X' OR";

                    //filters tipo ricevuta - Avvenuta Consegna
                    if (filters.TipoRicevuta_AvvenutaConsegna)
                        resultQueryFiltersTipoRicevuta += " SUBSTR(A.STATUS_C_MASK,3,1) = 'V' OR";

                    //filters tipo ricevuta - Mancata Consegna
                    if (filters.TipoRicevuta_MancataConsegna)
                        resultQueryFiltersTipoRicevuta += " SUBSTR(A.STATUS_C_MASK,3,1) = 'X' OR";

                    //filters tipo ricevuta - Conferma Ricezione
                    if (filters.TipoRicevuta_ConfermaRicezione)
                        resultQueryFiltersTipoRicevuta += " SUBSTR(A.STATUS_C_MASK,4,1) = 'V' OR";

                    //filters tipo ricevuta - Annullamento
                    if (filters.TipoRicevuta_AnnullamentoProtocollazione)
                        resultQueryFiltersTipoRicevuta += " SUBSTR(A.STATUS_C_MASK,5,1) = 'V' OR";

                    //filters tipo ricevuta - Con Errori
                    if (filters.TipoRicevuta_ConErrori)
                        resultQueryFiltersTipoRicevuta += " SUBSTR(A.STATUS_C_MASK,7,1) = 'V' OR";

                    //filters tipo ricevuta - Eccezione
                    if (filters.TipoRicevuta_Eccezione)
                        resultQueryFiltersTipoRicevuta += " SUBSTR(A.STATUS_C_MASK,6,1) = 'X' OR";
                }
                else if (dbType.ToUpper()=="SQL")
                {
                    //filtro tipo ricevuta - Avvenuta Accettazione
                    if (filters.TipoRicevuta_Accettazione)
                        resultQueryFiltersTipoRicevuta += " SUBSTRING(A.STATUS_C_MASK,2,1) = 'V' OR";

                    //filtro tipo ricevuta - Mancata Accettazione
                    if (filters.TipoRicevuta_MancataAccettazione)
                        resultQueryFiltersTipoRicevuta += " SUBSTRING(A.STATUS_C_MASK,2,1) = 'X' OR";

                    //filters tipo ricevuta - Avvenuta Consegna
                    if (filters.TipoRicevuta_AvvenutaConsegna)
                        resultQueryFiltersTipoRicevuta += " SUBSTRING(A.STATUS_C_MASK,3,1) = 'V' OR";

                    //filters tipo ricevuta - Mancata Consegna
                    if (filters.TipoRicevuta_MancataConsegna)
                        resultQueryFiltersTipoRicevuta += " SUBSTRING(A.STATUS_C_MASK,3,1) = 'X' OR";

                    //filters tipo ricevuta - Conferma Ricezione
                    if (filters.TipoRicevuta_ConfermaRicezione)
                        resultQueryFiltersTipoRicevuta += " SUBSTRING(A.STATUS_C_MASK,4,1) = 'V' OR";

                    //filters tipo ricevuta - Annullamento
                    if (filters.TipoRicevuta_AnnullamentoProtocollazione)
                        resultQueryFiltersTipoRicevuta += " SUBSTRING(A.STATUS_C_MASK,5,1) = 'V' OR";

                    //filters tipo ricevuta - Con Errori
                    if (filters.TipoRicevuta_ConErrori)
                        resultQueryFiltersTipoRicevuta += " SUBSTRING(A.STATUS_C_MASK,7,1) = 'V' OR";

                    //filters tipo ricevuta - Eccezione
                    if (filters.TipoRicevuta_Eccezione)
                        resultQueryFiltersTipoRicevuta += " SUBSTRING(A.STATUS_C_MASK,6,1) = 'X' OR";
                }
            }
            else
            {
                if (dbType.ToUpper() == "ORACLE")
                {
                    //filtro tipo ricevuta - Avvenuta Accettazione
                    if (filters.TipoRicevuta_Accettazione)
                        resultQueryFiltersTipoRicevuta += " EXISTS( select 'X' from DPA_STATO_INVIO z1 where z1.id_profile=a.id_profile and SUBSTR(z1.STATUS_C_MASK,2,1) = 'V') OR";

                    //filtro tipo ricevuta - Mancata Accettazione
                    if (filters.TipoRicevuta_MancataAccettazione)
                        resultQueryFiltersTipoRicevuta += " EXISTS( select 'X' from DPA_STATO_INVIO z1 where z1.id_profile=a.id_profile and SUBSTR(z1.STATUS_C_MASK,2,1) = 'X') OR";

                    //filters tipo ricevuta - Avvenuta Consegna
                    if (filters.TipoRicevuta_AvvenutaConsegna)
                        resultQueryFiltersTipoRicevuta += " EXISTS( select 'X' from DPA_STATO_INVIO z1 where z1.id_profile=a.id_profile and SUBSTR(z1.STATUS_C_MASK,3,1) = 'V') OR";

                    //filters tipo ricevuta - Mancata Consegna
                    if (filters.TipoRicevuta_MancataConsegna)
                        resultQueryFiltersTipoRicevuta += " EXISTS( select 'X' from DPA_STATO_INVIO z1 where z1.id_profile=a.id_profile and SUBSTR(z1.STATUS_C_MASK,3,1) = 'X') OR";

                    //filters tipo ricevuta - Conferma Ricezione
                    if (filters.TipoRicevuta_ConfermaRicezione)
                        resultQueryFiltersTipoRicevuta += " EXISTS( select 'X' from DPA_STATO_INVIO z1 where z1.id_profile=a.id_profile and SUBSTR(z1.STATUS_C_MASK,4,1) = 'V') OR";

                    //filters tipo ricevuta - Annullamento
                    if (filters.TipoRicevuta_AnnullamentoProtocollazione)
                        resultQueryFiltersTipoRicevuta += " EXISTS( select 'X' from DPA_STATO_INVIO z1 where z1.id_profile=a.id_profile and SUBSTR(z1.STATUS_C_MASK,5,1) = 'V') OR";

                    //filters tipo ricevuta - Eccezione
                    if (filters.TipoRicevuta_Eccezione)
                        resultQueryFiltersTipoRicevuta += " EXISTS( select 'X' from DPA_STATO_INVIO z1 where z1.id_profile=a.id_profile and SUBSTR(z1.STATUS_C_MASK,6,1) = 'X') OR";

                    //filters tipo ricevuta - Con Errori
                    if (filters.TipoRicevuta_ConErrori)
                        resultQueryFiltersTipoRicevuta += " EXISTS( select 'X' from DPA_STATO_INVIO z1 where z1.id_profile=a.id_profile and SUBSTR(z1.STATUS_C_MASK,7,1) = 'V') OR";
                }
                else if (dbType.ToUpper() == "SQL")
                {
                    //filtro tipo ricevuta - Avvenuta Accettazione
                    if (filters.TipoRicevuta_Accettazione)
                        resultQueryFiltersTipoRicevuta += " EXISTS( select 'X' from DPA_STATO_INVIO z1 where z1.id_profile=a.id_profile and SUBSTRING(z1.STATUS_C_MASK,2,1) = 'V') OR";

                    //filtro tipo ricevuta - Mancata Accettazione
                    if (filters.TipoRicevuta_MancataAccettazione)
                        resultQueryFiltersTipoRicevuta += " EXISTS( select 'X' from DPA_STATO_INVIO z1 where z1.id_profile=a.id_profile and SUBSTRING(z1.STATUS_C_MASK,2,1) = 'X') OR";

                    //filters tipo ricevuta - Avvenuta Consegna
                    if (filters.TipoRicevuta_AvvenutaConsegna)
                        resultQueryFiltersTipoRicevuta += " EXISTS( select 'X' from DPA_STATO_INVIO z1 where z1.id_profile=a.id_profile and SUBSTRING(z1.STATUS_C_MASK,3,1) = 'V') OR";

                    //filters tipo ricevuta - Mancata Consegna
                    if (filters.TipoRicevuta_MancataConsegna)
                        resultQueryFiltersTipoRicevuta += " EXISTS( select 'X' from DPA_STATO_INVIO z1 where z1.id_profile=a.id_profile and SUBSTRING(z1.STATUS_C_MASK,3,1) = 'X') OR";

                    //filters tipo ricevuta - Conferma Ricezione
                    if (filters.TipoRicevuta_ConfermaRicezione)
                        resultQueryFiltersTipoRicevuta += " EXISTS( select 'X' from DPA_STATO_INVIO z1 where z1.id_profile=a.id_profile and SUBSTRING(z1.STATUS_C_MASK,4,1) = 'V') OR";

                    //filters tipo ricevuta - Annullamento
                    if (filters.TipoRicevuta_AnnullamentoProtocollazione)
                        resultQueryFiltersTipoRicevuta += " EXISTS( select 'X' from DPA_STATO_INVIO z1 where z1.id_profile=a.id_profile and SUBSTRING(z1.STATUS_C_MASK,5,1) = 'V') OR";

                    //filters tipo ricevuta - Eccezione
                    if (filters.TipoRicevuta_Eccezione)
                        resultQueryFiltersTipoRicevuta += " EXISTS( select 'X' from DPA_STATO_INVIO z1 where z1.id_profile=a.id_profile and SUBSTRING(z1.STATUS_C_MASK,6,1) = 'X') OR";

                    //filters tipo ricevuta - Con Errori
                    if (filters.TipoRicevuta_ConErrori)
                        resultQueryFiltersTipoRicevuta += " EXISTS( select 'X' from DPA_STATO_INVIO z1 where z1.id_profile=a.id_profile and SUBSTRING(z1.STATUS_C_MASK,7,1) = 'V') OR";
                
                }
                }

                if ((filters.TipoRicevuta_Accettazione) ||
                    (filters.TipoRicevuta_MancataAccettazione) ||
                    (filters.TipoRicevuta_AvvenutaConsegna) ||
                    (filters.TipoRicevuta_MancataConsegna) ||
                    (filters.TipoRicevuta_ConfermaRicezione) ||
                    (filters.TipoRicevuta_AnnullamentoProtocollazione) ||
                    (filters.TipoRicevuta_ConErrori) ||
                    (filters.TipoRicevuta_Eccezione)) resultQueryFilters += string.Format(" AND ({0})", resultQueryFiltersTipoRicevuta.Substring(0, resultQueryFiltersTipoRicevuta.Length - 3));
            
            return resultQueryFilters;
        }

        private string GetReportSpedizioniQueryFiltersEsito(FiltriReportSpedizioni filters)
        {
            string retval = "";
            string retvaltemp = "";

            //filters tipo ricevuta - Esito Complessivo OK
            if (filters.TipoRicevuta_EsitoOK)
                retvaltemp += " (select count(1) from dpa_stato_invio z where a.id_profile= z.id_profile)=(select count(1) from dpa_stato_invio z where a.id_profile= z.id_profile and z.status_c_mask like 'V%') OR";

            //filters tipo ricevuta - Esito Complessivo Attendere
            if (filters.TipoRicevuta_EsitoAttesa)
                retvaltemp += " ( (select count(1) from dpa_stato_invio z where a.id_profile= z.id_profile and z.status_c_mask like 'A%')>0 AND (select count(1) from dpa_stato_invio z where a.id_profile= z.id_profile and z.status_c_mask like 'X%')=0) OR";

            //filters tipo ricevuta - Esito Complessivo Rispedire
            if (filters.TipoRicevuta_EsitoKO)
                retvaltemp += " (select count(1) from dpa_stato_invio z where a.id_profile= z.id_profile and z.status_c_mask like 'X%')>0 OR";
            if ((filters.TipoRicevuta_EsitoOK) ||
                (filters.TipoRicevuta_EsitoAttesa) ||
                (filters.TipoRicevuta_EsitoKO)) retval += string.Format(" AND ({0})", retvaltemp.Substring(0, retvaltemp.Length - 3));

            return retval;
        }


        /// <summary>
        /// Spedizioni per documento
        /// </summary>
        /// <param name="IdDocumento">Identificativo Documento</param>
        /// <returns></returns>
        public List<InfoDocumentoSpedito> GetReportSpedizioniDocumento(FiltriReportSpedizioni filters)
        {
            List<InfoDocumentoSpedito> docSpediti = new List<InfoDocumentoSpedito>();
            InfoDocumentoSpedito docSpedito = new InfoDocumentoSpedito();
            bool isFirstRow = true;
            try
            {
                logger.Debug("START : DocsPaDB > Query_DocsPAWS > Interoperabilita > getReportSpedizioniDocumento");
                //recupero elenco spedizioni
                DataSet dsSpedizioni = new DataSet();
                DocsPaUtils.Query qSpedizioni = DocsPaUtils.InitQuery.getInstance().getQuery("GET_REPORT_SPEDIZIONI_BY_DOCUMENTO");
                qSpedizioni.setParam("filters", GetReportSpedizioniQueryFilters(filters, null, true));
                qSpedizioni.setParam("iddocumento", filters.IdDocumento);
                string qSSpedizioni = qSpedizioni.getSQL();
                logger.Debug(qSSpedizioni);
                ExecuteQuery(out dsSpedizioni, "Spedizioni", qSSpedizioni);

                //mapping dati 
                if (dsSpedizioni.Tables["Spedizioni"].Rows.Count > 0)
                {
                    List<InfoSpedizione> listSpedizioni = new List<InfoSpedizione>();
                    //add info documento
                    docSpedito.InfoSpedizione = InfoDocumentoSpedito.TipoInfoSpedizione.Effettuato;// default value alert azione

                    foreach (DataRow rowSpedizioni in dsSpedizioni.Tables["Spedizioni"].Rows)
                    {
                        // add info master documento 
                        if (isFirstRow)
                        {
                            docSpedito.IDDocumento = rowSpedizioni["IDDOCUMENT"].ToString();
                            docSpedito.Protocollo = rowSpedizioni["PROTOCOLLO"].ToString();
                            docSpedito.DescrizioneDocumento = rowSpedizioni["OGGETTO"].ToString();
                            isFirstRow = false;
                        }
                        // add info detail spedizione
                        InfoSpedizione spedizione = new InfoSpedizione();
                        spedizione.MezzoSpedizione = rowSpedizioni["MEZZO"].ToString();
                        spedizione.NominativoDestinatario = rowSpedizioni["DESTINATARIO"].ToString();
                        spedizione.EMailDestinatario = rowSpedizioni["MAIL"].ToString();

                        spedizione.EMailMittente = (rowSpedizioni["MAIL_MITTENTE"] != null ? rowSpedizioni["MAIL_MITTENTE"].ToString() : "");
                                

                        if (spedizione.MezzoSpedizione == "Interoperabilità PITRE" || spedizione.EMailDestinatario.Contains("InteroperabilityService.svc"))
                        {
                            spedizione.EMailDestinatario = "N.A.";
                            spedizione.EMailMittente = "N.A.";
                        }
                        if (string.IsNullOrEmpty(rowSpedizioni["TIPO_DEST"].ToString()))
                            spedizione.TipoDestinatario = "";
                        else
                        {
                            if (rowSpedizioni["TIPO_DEST"].ToString() == "D" || rowSpedizioni["TIPO_DEST"].ToString() == "F" || rowSpedizioni["TIPO_DEST"].ToString() == "L")
                                spedizione.TipoDestinatario = "D";
                            else if (rowSpedizioni["TIPO_DEST"].ToString() == "C")
                                spedizione.TipoDestinatario = "CC";
                            else spedizione.TipoDestinatario = "";
                        }
                        spedizione.DataSpedizione = rowSpedizioni["DATASPEDIZIONE"].ToString();
                        if (!string.IsNullOrEmpty(rowSpedizioni["MASK"].ToString()))
                        {
                            //set azione/info
                            spedizione.Azione_Info = GetStatoAzione(rowSpedizioni["MASK"].ToString().Substring(0, 1));
                            // set alert 
                            if (spedizione.Azione_Info == InfoSpedizione.TipologiaAzione.Attendere && docSpedito.InfoSpedizione != InfoDocumentoSpedito.TipoInfoSpedizione.Errore) docSpedito.InfoSpedizione = InfoDocumentoSpedito.TipoInfoSpedizione.Warning;
                            if (spedizione.Azione_Info == InfoSpedizione.TipologiaAzione.Rispedire) docSpedito.InfoSpedizione = InfoDocumentoSpedito.TipoInfoSpedizione.Errore;
                            spedizione.TipoRicevuta_Accettazione = GetStatoRicevuta(rowSpedizioni["MASK"].ToString().Substring(1, 1));
                            spedizione.TipoRicevuta_Consegna = GetStatoRicevuta(rowSpedizioni["MASK"].ToString().Substring(2, 1));
                            spedizione.TipoRicevuta_Conferma = GetStatoRicevuta(rowSpedizioni["MASK"].ToString().Substring(3, 1));
                            spedizione.TipoRicevuta_Annullamento = GetStatoRicevuta(rowSpedizioni["MASK"].ToString().Substring(4, 1));
                            spedizione.TipoRicevuta_Eccezione = GetStatoRicevuta(rowSpedizioni["MASK"].ToString().Substring(5, 1));
                        }
                        spedizione.ProtocolloDestinatario = (rowSpedizioni["PROTO_DEST"] != null ? rowSpedizioni["PROTO_DEST"].ToString() : string.Empty);
                        spedizione.DataProtDest = (rowSpedizioni["DATA_PROTO_DEST"] != null ? rowSpedizioni["DATA_PROTO_DEST"].ToString() : string.Empty);
                        spedizione.MotivoAnnullEccezione = (rowSpedizioni["MOTIVO_ANNULLA"] != null ? rowSpedizioni["MOTIVO_ANNULLA"].ToString() : string.Empty);

                        if (spedizione.MezzoSpedizione != "INTEROPERABILITA" && spedizione.MezzoSpedizione != "Interoperabilità PITRE")
                        {
                            spedizione.TipoRicevuta_Annullamento = InfoSpedizione.TipologiaStatoRicevuta.AttendereCausaMezzo;
                            spedizione.TipoRicevuta_Conferma = InfoSpedizione.TipologiaStatoRicevuta.AttendereCausaMezzo;

                        }
                        else
                        {
                            if ((string.IsNullOrEmpty(spedizione.DataProtDest) || string.IsNullOrEmpty(spedizione.ProtocolloDestinatario))
                                && (spedizione.Azione_Info == InfoSpedizione.TipologiaAzione.OK || spedizione.TipoRicevuta_Conferma == InfoSpedizione.TipologiaStatoRicevuta.OK))
                            {
                                spedizione.Azione_Info = InfoSpedizione.TipologiaAzione.Attendere;
                                spedizione.TipoRicevuta_Conferma = InfoSpedizione.TipologiaStatoRicevuta.Attendere;
                                spedizione.TipoRicevuta_Annullamento = InfoSpedizione.TipologiaStatoRicevuta.Attendere;
                            }
                        }
                                
                        listSpedizioni.Add(spedizione);
                    }
                    docSpedito.Spedizioni = listSpedizioni;
                    docSpediti.Add(docSpedito);
                }

                logger.Debug("END : DocsPaDB > Query_DocsPAWS > Interoperabilita > getReportSpedizioniDocumento");
                return docSpediti;
            }
            catch (Exception ex) { throw ex; }
        }
        #endregion




        #endregion

        /// <summary>
        /// Query per il metodo "checkIdIMAP"
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="messageId"></param>
        /// <param name="id_registro"></param>
        public void getMailElabIMAP(out DataSet dataSet, string messageId, string id_registro)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAMailElabIMAP");
            q.setParam("param1", "'" + messageId.Replace("'", "''") + "'");
            q.setParam("idRegistro", id_registro);
            string queryString = q.getSQL();
            logger.Debug(queryString);
            ExecuteQuery(out dataSet, "MAIL", queryString);
        }


        /// <summary>
        /// metodo per l'aggiornamento del campo CHA_RAGIONE_ELAB della tabella DPA_MAIL_ELABORATE
        /// </summary>
        /// <param name="chaRagioneElab"></param>
        /// <param name="sysId"></param>
        public void setMailElabIMAP(string chaRagioneElab, string sysId)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPAMailElab");
            q.setParam("param1", chaRagioneElab);
            q.setParam("param2", sysId);
            string updateChaRagioneElab = q.getSQL();
            logger.Debug("IMAP_ELAB_SPOSTATA, query: " + updateChaRagioneElab);
            ExecuteNonQuery(updateChaRagioneElab);
        }
    }
}
