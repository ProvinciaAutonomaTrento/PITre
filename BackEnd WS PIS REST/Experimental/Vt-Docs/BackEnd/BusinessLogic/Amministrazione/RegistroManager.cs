using System;
using System.Data;
using System.Collections;
using DocsPaVO.amministrazione;
using DocsPaVO.Validations;
using System.Xml;
using System.IO;
using DocsPaUtils.Security;
using log4net;
using DocsPaVO.utente;

namespace BusinessLogic.Amministrazione
{
	/// <summary>
	/// Gestione registro in amministrazione
	/// </summary>
	public sealed class RegistroManager
	{
        private static ILog logger = LogManager.GetLogger(typeof(RegistroManager));
		private RegistroManager()
		{
		}

		#region Public methods

	    /// <summary>
         /// Reperimento registri/RF in amministrazione
	    /// </summary>
	    /// <param name="codiceAmministrazione"></param>
	    /// <param name="chaRF">0, se vogLio solo i registri, 1 se voglio solo gli RF, "" se voglio entrambi</param>
	    /// <returns></returns>
		public static ArrayList GetRegistri(string codiceAmministrazione,string chaRF)
		{
			ArrayList registri=new ArrayList();
			
			DocsPaDB.Query_DocsPAWS.Amministrazione dbAmministrazione=new DocsPaDB.Query_DocsPAWS.Amministrazione();
			DataSet ds=dbAmministrazione.GetDsRegistriAmministrazione(codiceAmministrazione,chaRF);

			if (ds.Tables.Count>0)
			{
				DataTable dtRegistri=ds.Tables["REGISTRI"];
				
				foreach (DataRow rowRegistro in dtRegistri.Rows)
					registri.Add(GetRegistro(rowRegistro));
			}

			return registri;
		}

		/// <summary>
		/// Reperimento di un registro in amministrazione
		/// </summary>
		/// <param name="idRegistro"></param>
		/// <returns></returns>
		public static OrgRegistro GetRegistro(string idRegistro)
		{
			OrgRegistro registro=null;

			DocsPaDB.Query_DocsPAWS.Amministrazione dbAmministrazione=new DocsPaDB.Query_DocsPAWS.Amministrazione();
			DataSet ds=dbAmministrazione.GetDsRegistro(idRegistro);

			if (ds.Tables.Count>0)
			{
				DataTable dtRegistri=ds.Tables["REGISTRI"];

				if (dtRegistri.Rows.Count==1)
					registro=GetRegistro(dtRegistri.Rows[0]);
			}

			return registro;
		}

		/// <summary>
		/// Inserimento di un registro in amministrazione
		/// </summary>
		/// <param name="registro"></param>
		/// <returns></returns>
		public static ValidationResultInfo InsertRegistro(OrgRegistro registro)
		{
			ValidationResultInfo result=CanInsertRegistro(registro);

			if (result.Value)
			{
				DocsPaDB.Query_DocsPAWS.Amministrazione dbAmministrazione=new DocsPaDB.Query_DocsPAWS.Amministrazione();
				result.Value=dbAmministrazione.InsertRegistro(registro);

				if (!result.Value)
				{
					// Errore nell'inserimento del registro
					BrokenRule brokenRule=new BrokenRule();
					brokenRule.ID="DB_ERROR";
					brokenRule.Description="Si è verificato un errore in inserimento del registro";
					result.BrokenRules.Add(brokenRule);
				}
			}

			return result;
		}

        /// <summary>
        /// Verifica dell'esistenza di documenti predisposti alla protocollazione in un dato registro
        /// </summary>
        /// <param name="registro"></param>
        /// <returns></returns>
        public static bool AmmPredispostiInRegistro(OrgRegistro registro)
        {
            bool result = false;
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmministrazione = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            result = dbAmministrazione.AmmPredispostiInRegistro(registro);
            return result;
        }

		/// <summary>
		/// Aggiornamento di un registro in amministrazione
		/// </summary>
		/// <param name="registro"></param>
		/// <returns></returns>
		public static ValidationResultInfo UpdateRegistro(OrgRegistro registro)
		{
			// Validazione dati del registro
			ValidationResultInfo result=CanUpdateRegistro(registro);

			if (result.Value)
			{
				DocsPaDB.Query_DocsPAWS.Amministrazione dbAmministrazione=new DocsPaDB.Query_DocsPAWS.Amministrazione();
				result.Value=dbAmministrazione.UpdateRegistro(registro);

				if (!result.Value)
				{
					// Errore nell'aggiornamento del registro
					BrokenRule brokenRule=new BrokenRule();
					brokenRule.ID="DB_ERROR";
					brokenRule.Description="Si è verificato un errore in aggiornamento del registro";
					result.BrokenRules.Add(brokenRule);
				}
			}

			return result;
		}

		/// <summary>
		/// Cancellazione di un registro in amministrazione
		/// </summary>
		/// <param name="registro"></param>
		/// <returns></returns>
		public static ValidationResultInfo DeleteRegistro(OrgRegistro registro)
		{
			ValidationResultInfo result=CanDeleteRegistro(registro);

			if (result.Value)
			{
				DocsPaDB.Query_DocsPAWS.Amministrazione dbAmministrazione=new DocsPaDB.Query_DocsPAWS.Amministrazione();
				result.Value=dbAmministrazione.DeleteRegistro(registro);

				if (!result.Value)
				{
					// Errore nella cancellazione del registro
					BrokenRule brokenRule=new BrokenRule();
					brokenRule.ID="DB_ERROR";
					brokenRule.Description="Si è verificato un errore in cancellazione del registro";
					result.BrokenRules.Add(brokenRule);
				}
			}

			return result;
		}

		/// <summary>
		/// Verifica se un registro può essere eliminato
		/// </summary>
		/// <param name="registro"></param>
		/// <returns></returns>
		public static ValidationResultInfo CanDeleteRegistro(OrgRegistro registro)
		{
			ValidationResultInfo retValue=IsValidRequiredFieldsRegistro(DBActionTypeRegistroEnum.DeleteMode,registro);

			if (retValue.Value)
			{
                DocsPaDB.Query_DocsPAWS.Amministrazione dbAmministrazione = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                if (registro.chaRF == "0")
                {
                    // Verifica presenza documenti protocollati (error)

                    if (dbAmministrazione.GetCountProtocolliRegistro(registro.IDRegistro) > 0)
                    {
                        retValue.Value = false;
                        BrokenRule brokenRule = new BrokenRule();
                        brokenRule.ID = "CONTAIN_DOCUMENTS";
                        brokenRule.Description = "Nel registro è presente almeno un documento";
                        retValue.BrokenRules.Add(brokenRule);
                    }

                    // Verifica presenza fascicoli (error)
                    if (dbAmministrazione.GetCountFascicoliRegistro(registro.IDRegistro) > 0)
                    {
                        retValue.Value = false;
                        BrokenRule brokenRule = new BrokenRule();
                        brokenRule.ID = "CONTAIN_FOLDERS";
                        brokenRule.Description = "Nel registro è presente almeno un fascicolo";
                        retValue.BrokenRules.Add(brokenRule);
                    }

                    // Verifica dipendenza del registro con almeno un ruolo (warning)
                    if (dbAmministrazione.GetCountRuoliRegistro(registro.IDRegistro) > 0)
                    {
                        BrokenRule brokenRule = new BrokenRule();
                        brokenRule.ID = "CONTAIN_ROLES";
                        brokenRule.Level = BrokenRule.BrokenRuleLevelEnum.Warning;

                        brokenRule.Description = "Almeno un ruolo risulta collegato al registro";
                        //if (registro.chaRF != null && registro.chaRF == "1")
                        //    brokenRule.Description = "Almeno un ruolo risulta collegato all'RF";

                        retValue.BrokenRules.Add(brokenRule);
                    }

                    //Verifica se ci sono degli RF associati
                    if (dbAmministrazione.GetCountRfAssociati(registro.IDRegistro) > 0)
                    {
                        BrokenRule brokenRule = new BrokenRule();
                        brokenRule.ID = "CONTAIN_RF";
                        //brokenRule.Level = BrokenRule.BrokenRuleLevelEnum.Error;

                        brokenRule.Description = "Almeno un RF risulta collegato al registro";

                        retValue.BrokenRules.Add(brokenRule);
                    }
                }
                else
                {

                    // Verifica dipendenza del registro con almeno un ruolo (warning)
                    if (dbAmministrazione.GetCountRuoliRegistro(registro.IDRegistro) > 0)
                    {
                        BrokenRule brokenRule = new BrokenRule();
                        brokenRule.ID = "CONTAIN_ROLES";
                        
                        brokenRule.Description = "Almeno un ruolo risulta collegato all'RF";

                        retValue.BrokenRules.Add(brokenRule);
                    }

                    // Verifica che non ci siano oggetti associati all'RF
                    if (dbAmministrazione.GetCountOggettiAssociati(registro.IDRegistro) > 0)
                    {
                          retValue.Value = false;
                          BrokenRule brokenRule = new BrokenRule();
                          brokenRule.ID = "CONTAIN_OGGETTI";
                          brokenRule.Description = "Almeno un oggetto risulta collegato all'RF";
                          retValue.BrokenRules.Add(brokenRule);
                    }

                    // Verifica che non ci siano corrispondenti associati all'RF
                    if (dbAmministrazione.GetCountCorrispondentiAssociati(registro.IDRegistro) > 0)
                    {
                          retValue.Value = false;
                          BrokenRule brokenRule = new BrokenRule();
                          brokenRule.ID = "CONTAIN_CORRISPONDENTI";
                          brokenRule.Description = "Almeno un corrispondente risulta collegato all'RF";
                          retValue.BrokenRules.Add(brokenRule);
                    }
                
                    
                }
			}

			return retValue;
		}

		/// <summary>
		/// Verifica vincoli in inserimento di un registro
		/// </summary>
		/// <param name="registro"></param>
		/// <returns></returns>
		public static ValidationResultInfo CanInsertRegistro(OrgRegistro registro)
		{
			ValidationResultInfo retValue=IsValidRequiredFieldsRegistro(DBActionTypeRegistroEnum.InsertMode,registro);

			// Verifica presenza codice univoco
			if (retValue.Value)
			{
				DocsPaDB.Query_DocsPAWS.Amministrazione dbAmministrazione=new DocsPaDB.Query_DocsPAWS.Amministrazione();
				retValue.Value=dbAmministrazione.CheckUniqueCodiceRegistro(registro.Codice,registro.CodiceAmministrazione);

				if (!retValue.Value)
				{
					BrokenRule brokenRule=new BrokenRule();
                    if (registro.chaRF != null && registro.chaRF != string.Empty)
                    {
                        if (registro.chaRF == "0")
                        {
                            brokenRule.ID = "CODICE_REGISTRO";
                            brokenRule.Description = "Codice registro già presente";
                        }
                        else
                        {
                            brokenRule.ID = "CODICE_RF";
                            brokenRule.Description = "Codice RF già presente";
                        }
                    }
					retValue.BrokenRules.Add(brokenRule);
				}
			}

			return retValue;
		}

		/// <summary>
		/// Verifica se un registro può essere aggiornato
		/// </summary>
		/// <param name="registro"></param>
		/// <returns></returns>
		public static ValidationResultInfo CanUpdateRegistro(OrgRegistro registro)
		{
			// Validazione dati obbligatori in fase di aggiornamento
			ValidationResultInfo retValue=IsValidRequiredFieldsRegistro(DBActionTypeRegistroEnum.UpdateMode,registro);

			if (retValue.Value)
			{
				// Verifica che non sia stato modificato il codice del registro
				DocsPaDB.Query_DocsPAWS.Amministrazione dbAmministrazione=new DocsPaDB.Query_DocsPAWS.Amministrazione();
				string codiceRegistro=dbAmministrazione.GetCodiceRegistro(registro.IDRegistro);

				retValue.Value=(registro.Codice.Equals(codiceRegistro));

				if (!retValue.Value)
				{
					BrokenRule brokenRule=new BrokenRule();
					brokenRule.ID="CODICE_REGISTRO";
					brokenRule.Description="Il codice registro non può essere modificato";
					retValue.BrokenRules.Add(brokenRule);
				}
			}

			return retValue;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="idRegistro"></param>
		/// <returns></returns>
		public static string GetXMLUOSmistamento(string idRegistro)
		{
			string result = null;			

			try
			{				
				System.Data.DataSet ds;				
				DocsPaDB.Query_DocsPAWS.Amministrazione manager = new DocsPaDB.Query_DocsPAWS.Amministrazione();
				ds = manager.GetXMLUOSmistamento(idRegistro);

				if(ds != null && ds.Tables["UOSmista"].Rows.Count > 0) 
					result = ds.GetXml();								
			}
			catch
			{
				result = null;
			}
			return result; 
		}

		public static bool SetXMLUOSmistamento(string streamXml, string idRegistro)
		{
			bool result = true;

			try
			{						
				DocsPaDB.Query_DocsPAWS.Amministrazione manager = new DocsPaDB.Query_DocsPAWS.Amministrazione();
			
				// xml passato in stream
				XmlDocument xmlDocStream = new XmlDocument();
				xmlDocStream.LoadXml(streamXml);

				XmlNode lista = xmlDocStream.SelectSingleNode("NEWDATASET");
				if(lista.ChildNodes.Count > 0)
				{										
					foreach (XmlNode nodo in lista.ChildNodes)
					{
						if(nodo.ChildNodes[3].InnerText == "1")
						{
							if(!manager.ExistUOSmista(nodo.ChildNodes[0].InnerText,idRegistro))
								manager.InsertUoSmista(nodo.ChildNodes[0].InnerText,idRegistro);
						}
						else
						{
							manager.DeleteUoSmista(nodo.ChildNodes[0].InnerText,idRegistro);
						}						
					}					
				}				
			}
			catch
			{				
				return false;
			}

			return result;
		}

        public static Registro[] GetRfByIdAmm(int idAmministrazione, string tipo)
        {
            Registro[] result = null;
            DocsPaDB.Query_DocsPAWS.Amministrazione manager = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            result = manager.GetRfByIdAmm(idAmministrazione, tipo);
            return result;
        }

		#endregion

		

		#region Private methods
		
		private static OrgRegistro GetRegistro(DataRow rowRegistro)
		{
			OrgRegistro registro=new OrgRegistro();
			
			registro.IDRegistro=rowRegistro["ID_REGISTRO"].ToString();
			registro.Codice=rowRegistro["CODICE"].ToString();
			registro.Descrizione=rowRegistro["DESCRIZIONE"].ToString();
			registro.IDAmministrazione=rowRegistro["ID_AMMINISTRAZIONE"].ToString();
			registro.CodiceAmministrazione=rowRegistro["CODICE_AMMINISTRAZIONE"].ToString();
            registro.Stato = rowRegistro["STATO"].ToString();
            
            //Andrea De Marco - popolamento flag_pregressi e Anno_pregressi
            if (rowRegistro.Table.Columns.Contains("VAR_PREG"))
            {
                if (rowRegistro["VAR_PREG"].ToString() == null || !rowRegistro["VAR_PREG"].ToString().Equals("1"))
                {
                    registro.flag_pregresso = false;
                    registro.anno_pregresso = string.Empty;
                }
                else
                {
                    registro.flag_pregresso = true;
                    registro.anno_pregresso = rowRegistro["ANNO_PREG"].ToString();
                }
            }
            //End Andrea De Marco
            
            if (rowRegistro["INVIO_RICEVUTA_MANUALE"] != null)
            {
                //if (rowRegistro["INVIO_RICEVUTA_MANUALE"].ToString().Equals("1"))
                registro.invioRicevutaManuale = rowRegistro["INVIO_RICEVUTA_MANUALE"].ToString();
            }
            
            if (registro.Stato.Equals("S"))
                registro.Sospeso = true;
            if (rowRegistro["APERTURA_AUTOMATICA"] != null && rowRegistro["APERTURA_AUTOMATICA"].ToString() != string.Empty)
            {
                char aperturaAutomatica = Convert.ToChar(rowRegistro["APERTURA_AUTOMATICA"].ToString());

                registro.AperturaAutomatica = (aperturaAutomatica != '0');
            }
            else
            {
                //caso degli RF
                registro.AperturaAutomatica = false;
            }
            registro.ID_PEOPLE_AOO = rowRegistro["ID_PEOPLE_AOO"].ToString();
            registro.ID_RUOLO_AOO = rowRegistro["ID_RUOLO_AOO"].ToString();
			registro.Mail.Email=rowRegistro["EMAIL"].ToString();
			registro.Mail.UserID=rowRegistro["USER_EMAIL"].ToString();
            registro.Mail.Password = Crypter.Decode(rowRegistro["PWD_EMAIL"].ToString(), registro.Mail.UserID);
			registro.Mail.ServerSMTP=rowRegistro["SERVER_SMTP"].ToString();
            if (rowRegistro["CHA_SMTP_SSL"] != DBNull.Value)
             registro.Mail.SMTPssl = rowRegistro["CHA_SMTP_SSL"].ToString();
            if (rowRegistro["CHA_POP_SSL"] != DBNull.Value)
             registro.Mail.POPssl = rowRegistro["CHA_POP_SSL"].ToString();
			if (rowRegistro["PORTA_SMTP"]!=DBNull.Value)
				registro.Mail.PortaSMTP=Convert.ToInt32(rowRegistro["PORTA_SMTP"]);
            if (rowRegistro["CHA_SMTP_STA"] != DBNull.Value)
                registro.Mail.SMTPsslSTA = rowRegistro["CHA_SMTP_STA"].ToString();

			registro.Mail.ServerPOP=rowRegistro["SERVER_POP"].ToString();
			
			if (rowRegistro["PORTA_POP"]!=DBNull.Value)
				registro.Mail.PortaPOP=Convert.ToInt32(rowRegistro["PORTA_POP"]);

			registro.Mail.UserSMTP=rowRegistro["USER_SMTP"].ToString();
            registro.Mail.PasswordSMTP = Crypter.Decode(rowRegistro["PWD_SMTP"].ToString(), registro.Mail.UserSMTP);
             
         //modifica
            registro.Mail.inbox = rowRegistro["VAR_INBOX_IMAP"].ToString();
            registro.Mail.serverImap = rowRegistro["VAR_SERVER_IMAP"].ToString();
            if (rowRegistro["NUM_PORTA_IMAP"] != DBNull.Value)
                registro.Mail.portaIMAP = Convert.ToInt32(rowRegistro["NUM_PORTA_IMAP"]);
            registro.Mail.tipoPosta =  rowRegistro["VAR_TIPO_CONNESSIONE"].ToString();
            registro.Mail.mailElaborate = rowRegistro["VAR_BOX_MAIL_ELABORATE"].ToString();
            registro.Mail.mailNonElaborate = rowRegistro["VAR_MAIL_NON_ELABORATE"].ToString();
            registro.Mail.IMAPssl = rowRegistro["CHA_IMAP_SSL"].ToString();         
            //fine modifica
            //MODIFICA del 10/07/2009"
            registro.Mail.soloMailPec = rowRegistro["VAR_SOLO_MAIL_PEC"].ToString();
            //fine modifica
            //Modifica del 6/6/11
            if (rowRegistro.Table.Columns.Contains("CHA_RICEVUTA_PEC"))
                if (rowRegistro["CHA_RICEVUTA_PEC"] != DBNull.Value)
                    registro.Mail.pecTipoRicevuta = rowRegistro["CHA_RICEVUTA_PEC"].ToString();

            // Per gestione pendenti tramite PEC
            registro.Mail.MailRicevutePendenti = rowRegistro["VAR_MAIL_RIC_PENDENTE"].ToString();
  
            if (rowRegistro["CHA_AUTO_INTEROP"] != DBNull.Value)
            {
                registro.autoInterop = rowRegistro["CHA_AUTO_INTEROP"].ToString();
            }
            if (rowRegistro["CHA_RF"] != DBNull.Value)
            {
                registro.chaRF = rowRegistro["CHA_RF"].ToString();
            }
            if (rowRegistro["ID_AOO_COLLEGATA"] != DBNull.Value)
            {
                registro.idAOOCollegata = rowRegistro["ID_AOO_COLLEGATA"].ToString();
            }
            if (rowRegistro["CHA_DISABILITATO"] != DBNull.Value)
            {
                registro.rfDisabled = rowRegistro["CHA_DISABILITATO"].ToString();
                registro.Sospeso = true;
            }

            if (rowRegistro["DIRITTO_RUOLO_AOO"] != DBNull.Value)
            {
               registro.Diritto_Ruolo_AOO = rowRegistro["DIRITTO_RUOLO_AOO"].ToString();
            }
            
            if (rowRegistro["ID_RUOLO_RESP"] != DBNull.Value)
                registro.idRuoloResp = rowRegistro["ID_RUOLO_RESP"].ToString();
            else
                registro.idRuoloResp = string.Empty;

			return registro;
		}

		private enum DBActionTypeRegistroEnum
		{
			InsertMode,
			UpdateMode,
			DeleteMode
		}

		/// <summary>
		/// Verifica presenza dati obbligatori del registro
		/// </summary>
		/// <param name="actionType"></param>
		/// <param name="nodoTitolario"></param>
		/// <param name="errorDescription"></param>
		/// <returns></returns>
		private static ValidationResultInfo IsValidRequiredFieldsRegistro(
									DBActionTypeRegistroEnum actionType,
									DocsPaVO.amministrazione.OrgRegistro registro)
		{
			ValidationResultInfo retValue=new ValidationResultInfo();
			BrokenRule brokenRule=null;
			
			if (actionType!=DBActionTypeRegistroEnum.InsertMode &&
				(registro.IDRegistro==null || 
				registro.IDRegistro==string.Empty || 
				registro.IDRegistro=="0"))
			{
				retValue.Value=false;
				brokenRule=new BrokenRule();
                if (registro.chaRF != null && registro.chaRF.Equals("1"))
                {
                    brokenRule.ID = "ID_RF";
                    brokenRule.Description = "ID raggruppamento funzionale mancante";
                }
                else
                {
                    brokenRule.ID = "ID_REGISTRO";
                    brokenRule.Description = "ID registro mancante";
                }
				retValue.BrokenRules.Add(brokenRule);
			}

            if (actionType == DBActionTypeRegistroEnum.InsertMode ||
                actionType == DBActionTypeRegistroEnum.UpdateMode)
            {
                if (registro.Codice == null || registro.Codice == string.Empty)
                {
                    retValue.Value = false;
                    brokenRule = new BrokenRule();

                    if (registro.chaRF != null && registro.chaRF.Equals("1"))
                    {
                        brokenRule.ID = "CODICE_RF";
                        brokenRule.Description = "Codice raggruppamento funzionale mancante";
                    }
                    else
                    {

                        brokenRule.ID = "CODICE_REGISTRO";
                        brokenRule.Description = "Codice registro mancante";
                    }
                    retValue.BrokenRules.Add(brokenRule);
                }

                if (registro.Descrizione == null || registro.Descrizione == string.Empty)
                {
                    retValue.Value = false;
                    brokenRule = new BrokenRule();
                    if (registro.chaRF != null && registro.chaRF.Equals("1"))
                    {
                        brokenRule.ID = "DESCRIZIONE_RF";
                        brokenRule.Description = "Descrizione raggruppamento funzionale mancante";
                    }
                    else
                    {
                        brokenRule.ID = "DESCRIZIONE_REGISTRO";
                        brokenRule.Description = "Descrizione registro mancante";
                    }
                    retValue.BrokenRules.Add(brokenRule);
                }

                // se è un RF vedo che sia inserita obbligatoriamente la AOO collegata
                if ((registro.chaRF != null && registro.chaRF == "1") && (registro.idAOOCollegata == null || registro.idAOOCollegata == string.Empty))
                {
                    retValue.Value = false;
                    brokenRule = new BrokenRule();

                    brokenRule.ID = "AOO_COLLEGATA_RF";
                    brokenRule.Description = "AOO collegata al raggruppamento funzionale mancante";

                    retValue.BrokenRules.Add(brokenRule);
                }
            }

			// Validazione indirizzo email
			if (actionType!=DBActionTypeRegistroEnum.DeleteMode && 
				registro.Mail.Email!=null && registro.Mail.Email!=string.Empty &&
				!IsValidEmail(registro.Mail.Email))
			{
				retValue.Value=false;
				retValue.BrokenRules.Add(new BrokenRule("MAIL_REGISTRO","Indirizzo email non valido"));
			}

            retValue.Value = (retValue.BrokenRules.Count == 0);

			return retValue;
		}

		/// <summary>
		/// Validazione formale indirizzo email
		/// </summary>
		/// <param name="mail"></param>
		/// <returns></returns>
		public static bool IsValidEmail(string mail)
		{	
			return System.Text.RegularExpressions.Regex.IsMatch(mail, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"); 
		}

		#endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registro"></param>
        /// <param name="modelContent"></param>
        /// <returns></returns>
        public static bool SaveModelloStampaRicevuta(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.amministrazione.OrgRegistro registro, byte[] modelContent, bool modelPdf)
        {
            bool retValue = true;

            try
            {
                FileInfo fileInfo = new FileInfo(GetPathModelloStampaRicevuta(registro, (modelPdf ? "pdf" : "rtf")));

                if (!fileInfo.Directory.Exists)
                    fileInfo.Directory.Create();

                File.WriteAllBytes(fileInfo.FullName, modelContent);
                
                retValue = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retValue = false;
            }

            return retValue;
        }

        //public static bool SaveModelloStampaRicevutaPDF(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.amministrazione.OrgRegistro registro, byte[] modelContent)
        //{
        //    bool retValue = true;

        //    try
        //    {
        //        FileInfo fileInfo = new FileInfo(GetPathModelloStampaRicevutaPDF(registro));

        //        if (!fileInfo.Directory.Exists)
        //            fileInfo.Directory.Create();

        //        File.WriteAllBytes(fileInfo.FullName, modelContent);

        //        retValue = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        DocsPaUtils.LogsManagement.logger.Debug(ex);
        //        retValue = false;
        //    }

        //    return retValue;
        //}

        /// <summary>
        /// Rimozione del modello utilizzato per la stampa della ricevuta
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="registro"></param>
        /// <returns></returns>
        public static bool DeleteModelloStampaRicevuta(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.amministrazione.OrgRegistro registro)
        {
            bool retValue = false;

            try
            {
                string path = GetPathModelloStampaRicevuta(registro);

                if (File.Exists(path))
                {
                    File.Delete(path);

                    retValue = true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retValue = false;
            }

            return retValue;
        }

        /// <summary>
        /// Verifica se il modello per la stampa della ricevuta esiste o meno
        /// </summary>
        /// <param name="registro"></param>
        /// <returns></returns>
        public static bool ContainsModelloStampaRicevuta(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.amministrazione.OrgRegistro registro)
        {
            string tipo_ricevuta_proto = string.Empty;
            string extensionFile = string.Empty;
            ArrayList keyConfig = DocsPaUtils.Configuration.ChiaviConfigManager.GetChiaviConfig(registro.IDAmministrazione);
            for (int i = 0; i < keyConfig.Count; i++)
            {
                if (((ChiaveConfigurazione)keyConfig[i]).Codice.Equals("FE_RICEVUTA_PROTOCOLLO_PDF"))
                {
                    tipo_ricevuta_proto = ((ChiaveConfigurazione)keyConfig[i]).Valore;
                    break;
                }
            }
            if (!String.IsNullOrEmpty(tipo_ricevuta_proto) && tipo_ricevuta_proto.Equals("1"))
                extensionFile = "pdf";
            else
                extensionFile = "";
            return File.Exists(GetPathModelloStampaRicevuta(registro, extensionFile));
        }

        /// <summary>
        /// Reperimento del path del modello del registro
        /// </summary>
        /// <param name="registro"></param>
        /// <returns></returns>
        public static string GetPathModelloStampaRicevuta(DocsPaVO.amministrazione.OrgRegistro registro)
        {
            return GetPathModelloStampaRicevuta(registro, "rtf");
        }
        
        /// <summary>
        /// Reperimento del path del modello del registro
        /// </summary>
        /// <param name="registro"></param>
        /// <param name="templatePdf"></param>
        /// <returns></returns>
        public static string GetPathModelloStampaRicevuta(DocsPaVO.amministrazione.OrgRegistro registro, string modelExtension)
        {
            return string.Concat(ModelsRootPath, string.Format(@"\Modelli\{0}\Ricevute\{1}\Ricevuta.{2}", registro.CodiceAmministrazione, registro.Codice, modelExtension));
        }

        /// <summary>
        /// Reperimento del percorso radice dei modelli
        /// </summary>
        /// <returns></returns>
        private static string ModelsRootPath
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["MODELS_ROOT_PATH"];
            }
        }

        public static string getCodRFFromSysIdCorrGlob(string systemId)
        {
            string codiceRF = string.Empty;
            try
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione dbAmministrazione = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                codiceRF = dbAmministrazione.getCodRFFromSysIdCorrGlob(systemId);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return codiceRF;
        }

        #region Multi Caselle Pec
        /// <summary>
        /// Reperimento elenco caselle di posta di un registro/rf in amministrazione
        /// </summary>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        public static CasellaRegistro[] GetMailRegistro(string idRegistro)
        {
            CasellaRegistro [] mail_registri = null;

            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmministrazione = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DataSet ds = dbAmministrazione.GetDsMailRegistro(idRegistro);
            if (ds.Tables.Count > 0)
            {
                int index = 0;
                mail_registri = new CasellaRegistro[ds.Tables["MAIL_REGISTRI"].Rows.Count];
                foreach (DataRow row in ds.Tables["MAIL_REGISTRI"].Rows)
                {
                    mail_registri[index] = GetMail(row);
                    ++index;
                }
            }

            return mail_registri;
        }

        private static CasellaRegistro GetMail(DataRow rowMailRegistro)
        {
            CasellaRegistro mail = new CasellaRegistro();
            mail.IdRegistro = rowMailRegistro["ID_REGISTRO"].ToString();
            mail.EmailRegistro = rowMailRegistro["VAR_EMAIL_REGISTRO"].ToString();
            mail.UserMail = rowMailRegistro["VAR_USER_MAIL"].ToString();
            mail.PwdMail = Crypter.Decode(rowMailRegistro["VAR_PWD_MAIL"].ToString(), mail.UserMail);
            mail.ServerSMTP = rowMailRegistro["VAR_SERVER_SMTP"].ToString();
            if (rowMailRegistro["CHA_SMTP_SSL"] != DBNull.Value)
                mail.SmtpSSL = rowMailRegistro["CHA_SMTP_SSL"].ToString();
            if(rowMailRegistro["CHA_POP_SSL"] != DBNull.Value)
                mail.PopSSL = rowMailRegistro["CHA_POP_SSL"].ToString();
            if (rowMailRegistro["NUM_PORTA_SMTP"] != DBNull.Value)
                mail.PortaSMTP = Convert.ToInt32(rowMailRegistro["NUM_PORTA_SMTP"]);
            if (rowMailRegistro["CHA_SMTP_STA"] != DBNull.Value)
                mail.SmtpSta = rowMailRegistro["CHA_SMTP_STA"].ToString();
            mail.ServerPOP = rowMailRegistro["VAR_SERVER_POP"].ToString();
            if(rowMailRegistro["NUM_PORTA_POP"] != DBNull.Value)
                mail.PortaPOP = Convert.ToInt32(rowMailRegistro["NUM_PORTA_POP"]);
            mail.UserSMTP = rowMailRegistro["VAR_USER_SMTP"].ToString();
            mail.PwdSMTP = Crypter.Decode(rowMailRegistro["VAR_PWD_SMTP"].ToString(), mail.UserSMTP);
            mail.IboxIMAP = rowMailRegistro["VAR_INBOX_IMAP"].ToString();
            mail.ServerIMAP = rowMailRegistro["VAR_SERVER_IMAP"].ToString();
            if(rowMailRegistro["NUM_PORTA_IMAP"] != DBNull.Value)
                mail.PortaIMAP = Convert.ToInt32(rowMailRegistro["NUM_PORTA_IMAP"]);
            mail.TipoConnessione = rowMailRegistro["VAR_TIPO_CONNESSIONE"].ToString();
            mail.BoxMailElaborate = rowMailRegistro["VAR_BOX_MAIL_ELABORATE"].ToString();
            mail.MailNonElaborate = rowMailRegistro["VAR_MAIL_NON_ELABORATE"].ToString();
            mail.ImapSSL = rowMailRegistro["CHA_IMAP_SSL"].ToString();
            mail.SoloMailPEC = rowMailRegistro["VAR_SOLO_MAIL_PEC"].ToString();
            if (rowMailRegistro["CHA_RICEVUTA_PEC"] != DBNull.Value)
                mail.RicevutaPEC = rowMailRegistro["CHA_RICEVUTA_PEC"].ToString();
            mail.Principale = rowMailRegistro["VAR_PRINCIPALE"].ToString();
            mail.Note = rowMailRegistro["VAR_NOTE"].ToString();
            // Per gestione pendenti tramite PEC
            mail.MailRicevutePendenti = rowMailRegistro["VAR_MAIL_RIC_PENDENTE"].ToString();
            return mail;
        }

        /// <summary>
        /// Aggiornamento delle caselle di posta di un registro in amministrazione
        /// </summary>
        /// <param name="registro"></param>
        /// <returns></returns>
        public static ValidationResultInfo UpdateMailRegistro(string idRegistro, CasellaRegistro[] caselle)
        {
                ValidationResultInfo result = new ValidationResultInfo();
                DocsPaDB.Query_DocsPAWS.Amministrazione dbAmministrazione = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                result.Value = dbAmministrazione.UpdateMailRegistro(idRegistro, caselle);
                if (!result.Value)
                {
                    // Errore nell'aggiornamento delle caselle di posta associate al registro
                    BrokenRule brokenRule = new BrokenRule();
                    brokenRule.ID = "DB_ERROR";
                    brokenRule.Description = "Si è verificato un errore in aggiornamento caselle di posta del registro";
                    result.BrokenRules.Add(brokenRule);
                }
                return result;
        }

        public static ValidationResultInfo InsertMailRegistro(string idRegistro, CasellaRegistro[] caselle, bool insertInteropInt)
        {
            ValidationResultInfo result = new ValidationResultInfo();
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmministrazione = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            result.Value = dbAmministrazione.InsertMailRegistro(idRegistro, caselle, insertInteropInt);
            if (!result.Value)
            {
                // Errore nell'aggiornamento delle caselle di posta associate al registro
                BrokenRule brokenRule = new BrokenRule();
                brokenRule.ID = "DB_ERROR";
                brokenRule.Description = "Si è verificato un errore in inserimento caselle di posta del registro";
                result.BrokenRules.Add(brokenRule);
            }
            return result;
        }

        public static ValidationResultInfo DeleteMailRegistro(string idRegistro, string casella)
        {
            ValidationResultInfo result = new ValidationResultInfo();
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmministrazione = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            result.Value = dbAmministrazione.DeleteMailRegistro(idRegistro, casella);
            if (!result.Value)
            {
                // Errore nell'eliminazione delle caselle di posta associate al registro
                BrokenRule brokenRule = new BrokenRule();
                brokenRule.ID = "DB_ERROR";
                brokenRule.Description = "Si è verificato un errore durante l'eliminazione delle caselle di posta associate al registro";
                result.BrokenRules.Add(brokenRule);
            }
            return result;
        }

        /// <summary>
        /// Restituisce la casella di posta di un registro/rf impostata in amministrazione come principale
        /// </summary>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        public static string GetMailPrincipaleRegistro(string idRegistro)
        { 
            string casellaPrincipale = string.Empty;
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmministrazione = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            casellaPrincipale = dbAmministrazione.GetMailPrincipaleReg(idRegistro);
            return casellaPrincipale;
        }

        /// <summary>
        /// Inserimernto diritti di visibilità del ruolo di un registro/RF sulle caselle di posta associate
        /// </summary>
        /// <param name="idRegistro"></param>
        /// <param name="idAooInUO"></param>
        /// <returns></returns>
        public static ValidationResultInfo InsertRightRuoloMailRegistro(System.Collections.Generic.List<RightRuoloMailRegistro> rightRuoloMailRegistro)
        {
            ValidationResultInfo result = new ValidationResultInfo();
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmministrazione = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            result.Value = dbAmministrazione.InsertRigthRuoloMailRegistro(rightRuoloMailRegistro);
            if (!result.Value)
            {
                // Errore nell'inserimento dei diritti
                BrokenRule brokenRule = new BrokenRule();
                brokenRule.ID = "DB_ERROR";
                brokenRule.Description = "Si è verificato un errore durante l'inserimento dei diritti di visibilità dei ruoli del registro/RF sulle caselle di posta";
                result.BrokenRules.Add(brokenRule);
            }
            return result;
        }

        /// <summary>
        /// Eliminazione diritti di visibilità del ruolo di un registro/RF sulle caselle di posta associate
        /// </summary>
        /// <param name="idRegistro"></param>
        /// <param name="idAooInUO"></param>
        /// <returns></returns>
        public static ValidationResultInfo DeleteRightRuoloMailRegistro(string idRegistro, string idRuoloInUO, string indirizzoEmail)
        {
            ValidationResultInfo result = new ValidationResultInfo();
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmministrazione = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            result.Value = dbAmministrazione.DeleteRigthRuoloMailRegistro(idRegistro, idRuoloInUO, indirizzoEmail);
            if (!result.Value)
            {
                // Errore nell'eliminazione dei diritti
                BrokenRule brokenRule = new BrokenRule();
                brokenRule.ID = "DB_ERROR";
                brokenRule.Description = "Si è verificato un errore durante l'eliminazione dei diritti di visibilità dei ruoli del registro/RF sulle caselle di posta";
                result.BrokenRules.Add(brokenRule);
            }
            return result;
        }

        /// <summary>
        /// Restituisce le info di visibilità del ruolo sulle caselle di postasul Eliminazione diritti di visibilità del ruolo di un registro/RF sulle caselle di posta associate
        /// </summary>
        /// <param name="idRegistro"></param>
        /// <param name="idAooInUO"></param>
        /// <returns></returns>
        public static DataSet GetRightRuoloMailRegistro(string idRegistro, string idRuoloInUO)
        {
            RightRuoloMailRegistro[] visRuolo = null;
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmministrazione = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return dbAmministrazione.GetRigthRuoloMailRegistro(idRegistro, idRuoloInUO);
        }

        public static ValidationResultInfo UpdateRightRuoloMailRegistro(RightRuoloMailRegistro[] visRuolo)
        {
            ValidationResultInfo result = new ValidationResultInfo();
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmministrazione = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            result.Value = dbAmministrazione.UpdateRightRuoloMailRegistro(visRuolo);
            if (!result.Value)
            {
                BrokenRule brokenRule = new BrokenRule();
                brokenRule.ID = "DB_ERROR";
                brokenRule.Description = "Si è verificato un errore in aggiornamento visibilità ruolo sulle caselle di posta associate al registro";
                result.BrokenRules.Add(brokenRule);
            }
            return result;
        }

        #endregion
    }
}