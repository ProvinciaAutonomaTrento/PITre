using System;
using System.Collections;
using log4net;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Documenti
{
	#region Classe Commentata
	/*public class StatoInvio 
	{

		public string idRegistro = "";
		public string idCorrispondente = "";
		public string idProfile = "";
		public string typeId = "";
		public string idCanale = "";
		public string idDocArrivoPar = "";
		public string indirizzo = "";
		public string cap = "";
		public string citta = "";
		public string provincia = "";
		public string interop = "";
		public string serverSMTP = "";
		public string portaSMTP = "";
		public string dataSpedizione;
		public string tipoCanale = "";
		public string codiceAOO = "";
		public string codiceAmm = "";


		public StatoInvio() {}
	}*/
	#endregion

	/// <summary>
	/// 
	/// </summary>
	public class ProtocolloUscitaManager 
	{
        private static ILog logger = LogManager.GetLogger(typeof(ProtocolloUscitaManager));

		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="corrispondente"></param>
		/// <param name="idDocArrivoPar"></param>
		/// <param name="idRegistro"></param>
		/// <param name="idProfile"></param>
		
		public static void addStatoInvio(DocsPaVO.utente.Corrispondente corrispondente, string idDocArrivoPar, string idRegistro, string idProfile, string mail) 
		{
			#region Codice Commentato
			/*ArrayList idCorrispondenti = getIdCorrispondentiGruppo(corrispondente.systemId);
			DocsPaVO.StatoInvio.StatoInvio statoInvio;
			for (int i = 0; i < idCorrispondenti.Count; i++) 
			{
				statoInvio = new DocsPaVO.StatoInvio.StatoInvio();
				statoInvio.idDocArrivoPar = idDocArrivoPar;
				statoInvio.idRegistro = idRegistro;
				statoInvio.idProfile = idProfile;
				statoInvio.idCorrispondente = (string) idCorrispondenti[i];
				//modifica per gestione - tipo spedizione  -- ATTENZIONE SE SI TRATTA DI UN GRUPPO NON HA MOLTO SENSO UN TIPO SPEDIZIONE VALIDO PER TUTTI I MEMBRI DEL GRUPPO
				//statoInvio = getDocumentType(db, statoInvio);
		
				if (corrispondente.canalePref == null)
				{
					statoInvio = getDocumentType(statoInvio);
				}
				else
				{
						statoInvio.typeId = corrispondente.canalePref.systemId;
				}
				if (statoInvio.typeId != null && !statoInvio.typeId.Equals("")) 
				{
					statoInvio = getDatiCorrispondente(statoInvio);
					statoInvio = getDatiCanale(statoInvio);
					statoInvio = getDettagliCorrispondente(statoInvio);
				}
				string insertString = 
					"INSERT INTO DPA_STATO_INVIO (" + DocsPa_V15_Utils.dbControl.getSystemIdColName() +
					"ID_CORR_GLOBALE, ID_PROFILE, ID_DOC_ARRIVO_PAR, ID_CANALE, " +
					"VAR_INDIRIZZO, VAR_CAP, VAR_CITTA, CHA_INTEROP, VAR_PROVINCIA, " +
					"ID_DOCUMENTTYPE, VAR_SERVER_SMTP, NUM_PORTA_SMTP, VAR_CODICE_AOO, VAR_CODICE_AMM) VALUES (" + 
					DocsPa_V15_Utils.dbControl.getSystemIdNextVal("DPA_STATO_INVIO") +
					statoInvio.idCorrispondente + "," + statoInvio.idProfile + "," + 
					statoInvio.idDocArrivoPar + ",'" + statoInvio.idCanale + "','" +
					statoInvio.indirizzo.Replace("'","''") + "','" + statoInvio.cap + "','" + 
					statoInvio.citta.Replace("'","''") + "','" + statoInvio.interop + "','" + 
					statoInvio.provincia.Replace("'","''") + "','" + statoInvio.typeId + "','" + 
					statoInvio.serverSMTP.Replace("'","''") + "','" + statoInvio.portaSMTP + "','" + 
					statoInvio.codiceAOO + "','" + statoInvio.codiceAmm + "')";
				logger.Debug(insertString);
				db.insertLocked(insertString, "DPA_STATO_INVIO");*/
			#endregion

			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			doc.InsertStatoInvio(corrispondente, idDocArrivoPar, idRegistro, idProfile, mail);	
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="statoInvio"></param>
		/// <returns></returns>
		public static DocsPaVO.StatoInvio.StatoInvio getDatiCorrispondente(DocsPaVO.StatoInvio.StatoInvio statoInvio) 
		{
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			doc.GetDatiCorrispondente(ref statoInvio);
			return statoInvio;

			#region Codice Commentato
			/*string queryString =
				"SELECT VAR_EMAIL, VAR_SMTP, NUM_PORTA_SMTP, CHA_PA, VAR_CODICE_AMM, VAR_CODICE_AOO " +
				"FROM DPA_CORR_GLOBALI WHERE SYSTEM_ID=" + statoInvio.idCorrispondente;
			logger.Debug(queryString);
			IDataReader dr = db.executeReader(queryString);
			if (dr.Read()) 
			{
				if(dr.GetValue(0) != null)
				{
					statoInvio.indirizzo = dr.GetValue(0).ToString();
				}
				if(dr.GetValue(1) != null)
				{
					statoInvio.serverSMTP = dr.GetValue(1).ToString();
				}
				if(dr.GetValue(2) != null)
				{
					statoInvio.portaSMTP = dr.GetValue(2).ToString();
				}
				if(dr.GetValue(3) != null)
				{
					statoInvio.interop = dr.GetValue(3).ToString();
				}
				if(dr.GetValue(4) != null) 
				{
					statoInvio.codiceAmm = dr.GetValue(4).ToString();
				}
				if(dr.GetValue(5) != null)
				{
					statoInvio.codiceAOO = dr.GetValue(5).ToString();
				}
			}	
			dr.Close();
			return statoInvio;*/
			#endregion
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="statoInvio"></param>
		/// <returns></returns>
		private static DocsPaVO.StatoInvio.StatoInvio getDettagliCorrispondente(DocsPaVO.StatoInvio.StatoInvio statoInvio) 
		{
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			return doc.GetDettagliCorrispondente(statoInvio);

			#region Codice Commentato
			/*if (statoInvio.tipoCanale.Equals("E"))
			{
				return statoInvio;
			}
			string queryString =
				"SELECT  VAR_FAX, VAR_INDIRIZZO, VAR_CAP, VAR_PROVINCIA, VAR_CITTA " +
				"FROM DPA_DETT_GLOBALI WHERE ID_CORR_GLOBALI= " + statoInvio.idCorrispondente;
			logger.Debug(queryString);
			IDataReader dr = db.executeReader(queryString);
			if (dr.Read()) 
			{
				if(statoInvio.tipoCanale.Equals("F")) 
				{
					statoInvio.indirizzo = dr.GetValue(0).ToString();
				}
				else 
				{
					statoInvio.indirizzo = dr.GetValue(1).ToString();
					statoInvio.cap = dr.GetValue(2).ToString();
					statoInvio.provincia = dr.GetValue(3).ToString();
					statoInvio.citta = dr.GetValue(4).ToString();
				}
			}		
			dr.Close();
			return statoInvio;*/
			#endregion
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="statoInvio"></param>
		/// <returns></returns>
		private static DocsPaVO.StatoInvio.StatoInvio getDatiCanale(DocsPaVO.StatoInvio.StatoInvio statoInvio) 
		{
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			doc.GetDatiCanale(ref statoInvio);
			return statoInvio;

			#region Codice Commentato
			/*string queryString =
				"SELECT  A.VAR_SERVER_SMTP, A.NUM_PORTA_SMTP, C.CHA_TIPO_CANALE, A.SYSTEM_ID " +
				"FROM DPA_CANALI_REG B, DPA_CANALI A, DOCUMENTTYPES C " +
				"WHERE B.ID_CANALE = A.SYSTEM_ID AND B.ID_DOCUMENTTYPE = C.SYSTEM_ID " +
				"AND B.ID_REGISTRO=" + statoInvio.idRegistro +
				" AND B.ID_DOCUMENTTYPE = " + statoInvio.typeId;
			logger.Debug(queryString);
			IDataReader dr = db.executeReader(queryString);
			if (dr.Read()) 
			{
				if(dr.GetValue(0) != null) 
				{
					statoInvio.serverSMTP = dr.GetValue(0).ToString();
					if(dr.GetValue(1) != null)
					{
						statoInvio.portaSMTP = dr.GetValue(1).ToString();
					}
					else
					{
						statoInvio.portaSMTP = "";
					}
				}
				statoInvio.tipoCanale = dr.GetValue(2).ToString();
				statoInvio.idCanale = dr.GetValue(3).ToString();
			}			
			dr.Close();
			return statoInvio;*/
			#endregion
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="statoInvio"></param>
		/// <returns></returns>
		private static DocsPaVO.StatoInvio.StatoInvio getDocumentType(DocsPaVO.StatoInvio.StatoInvio statoInvio) 
		{
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			doc.GetDocumentType(ref statoInvio);
			return statoInvio;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="idCorrispondente"></param>
		/// <returns></returns>
		private static ArrayList getIdCorrispondentiGruppo(string idCorrispondente) 
		{
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			return doc.GetIdCorrispondentiGruppo(idCorrispondente);
				
			#region Codice Commentato
			/*ArrayList idCorrispondenti = new ArrayList();
			  string queryString =
				"SELECT ID_COMP_GRUPPO FROM DPA_CORR_GRUPPO WHERE ID_GRUPPO="+idCorrispondente;
			logger.Debug(queryString);
			IDataReader dr = db.executeReader(queryString);
			while (dr.Read()) 
			{
				idCorrispondenti.Add(dr.GetValue(0).ToString());
			}
			dr.Close();
			
			if (idCorrispondenti.Count == 0)
			{
				idCorrispondenti.Add(idCorrispondente);
			}
			return idCorrispondenti;*/
			#endregion
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="corrispondente"></param>
		/// <returns></returns>
		private static ArrayList getCorrispondentiGruppo(DocsPaVO.utente.Corrispondente corrispondente) 
		{
			ArrayList list = new ArrayList();
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			list = doc.GetCorrispondentiGruppo(corrispondente);
			if (list == null)
			{
				//TODO: gestire la throw
				throw new Exception();
			}
			return list;
			
			#region Codice Commentato
			/*ArrayList corrispondenti = new ArrayList();
			string queryString = 
				"SELECT A.SYSTEM_ID, A.VAR_COD_RUBRICA, A.ID_AMM, " +
				"A.CHA_TIPO_IE, A.CHA_TIPO_CORR, B.CHA_TIPO_MITT_DEST, A.VAR_DESC_CORR " +
				"FROM DPA_CORR_GLOBALI A, DPA_DOC_ARRIVO_PAR B, DPA_CORR_GRUPPO C " +
				"WHERE A.SYSTEM_ID=B.ID_MITT_DEST AND A.SYSTEM_ID=C.ID_COMP_GRUPPO AND C.ID_GRUPPO=" + corrispondente.systemId;
				
			logger.Debug(queryString);
			IDataReader dr = db.executeReader(queryString);
			while (dr.Read()) 
			{
				corrispondenti.Add(DocManager.getCorrispondente(dr));
			}
			dr.Close();
			
			if (corrispondenti.Count == 0)
			{
				corrispondenti.Add(corrispondente);
			}
			return corrispondenti;*/
			#endregion
		}

        /// <summary>
        /// Spedizione del documento per interoperabilità
        /// </summary>
        /// <param name="schedaDoc"></param>
        /// <param name="registroMittente">
        /// Indica il registro (o l'RF) mittente del documento per interoperabilità.
        /// <remarks>
        /// Se null, il mittente sarà la mail del registro su cui è protocollato il documento
        /// </remarks>
        /// </param>
        /// <param name="mailAddress"></param>
        /// <param name="destinatari"></param>
        /// <param name="infoUtente"></param>
        /// <param name="confermaRic"></param>
        /// <returns></returns>
        public static DocsPaVO.Interoperabilita.SendDocumentResponse spedisci(
            DocsPaVO.documento.SchedaDocumento schedaDoc,
            DocsPaVO.utente.Registro registroMittente,
            string mailAddress,
            ArrayList destinatari,
            DocsPaVO.utente.InfoUtente infoUtente,
            bool confermaRic)
        {
            // Oggetto contenente l'esito della spedizione del documento ai destinatari.
            // Per ognuno di essi, nella tabella "DPA_STATO_INVIO",
            // deve essere impostata correttamente la data di spedizione; per gli 
            // altri viene impostata a null;
            DocsPaVO.Interoperabilita.SendDocumentResponse sendDocumentResponse = null;

            //se c'è il batch_interoperabilità non bisogna mandare risultati a video

            try
            {
                DocsPaVO.utente.Corrispondente mittente = BusinessLogic.Utenti.UserManager.getCorrispondente(infoUtente.idCorrGlobali, false);

                DocsPaVO.utente.Ruolo ruoloMitt = (DocsPaVO.utente.Ruolo)mittente;
                mittente.descrizione = ruoloMitt.descrizione;

                // Invio mail a tutti i destinatari del documento
                sendDocumentResponse = BusinessLogic.Interoperabilità.InteroperabilitaInvioSegnatura.SendDocument(schedaDoc, registroMittente, mailAddress, destinatari, infoUtente, confermaRic);
            }
            catch (Exception e)
            {
                logger.Debug("Errore nell'invio mail", e);

                throw new Exception("Si è verificato un errore nell'invio dell'email:\\n\\n" + e.Message);
            }

            try
            {
                InsertStatoInvioDestinatari(schedaDoc, destinatari, sendDocumentResponse);

                //SOLO nel caso di interoperabilità interna abilitata
                if (System.Configuration.ConfigurationManager.AppSettings["INTEROP_INT_NO_MAIL"] != null &&
                            System.Configuration.ConfigurationManager.AppSettings["INTEROP_INT_NO_MAIL"].ToString() != "0")
                {
                    foreach (DocsPaVO.Interoperabilita.SendDocumentResponse.SendDocumentMailResponse sdml in sendDocumentResponse.SendDocumentMailResponseList)
                    {
                        if (sdml.datiInteropAutomatica != null)
                        {
                            string erroreMessage = string.Empty;
                            Interoperabilità.InteroperabilitaInvioRicevuta.sendRicevutaRitorno(sdml.datiInteropAutomatica.schedaDoc, sdml.datiInteropAutomatica.registro, sdml.datiInteropAutomatica.ruolo, sdml.datiInteropAutomatica.infoUtente,out erroreMessage);
                            //
                            bool verificaRagioni;
                            string message = "";
                            //BUG INPS: doppia trasmis
                            //BusinessLogic.trasmissioni.TrasmProtoIntManager.TrasmissioneProtocolloAutomatico(sdml.datiInteropAutomatica.schedaDoc, sdml.datiInteropAutomatica.registro.systemId, schedaDoc, sdml.datiInteropAutomatica.ruolo, sdml.datiInteropAutomatica.infoUtente, infoUtente.urlWA, false, out verificaRagioni, out message);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella gestione della spedizione", e);

                throw new Exception("Errore nella gestione della spedizione:\\n\\n" + e.Message);
            }

            return sendDocumentResponse;
        }

        /// <summary>
        /// Metodo per la spedizione di un documento per interoperabilità semplificata
        /// </summary>
        /// <param name="schedaDoc">Documento da spedire</param>
        /// <param name="infoUtente">Informazioni sull'utente che sta effettuando la spedizione</param>
        /// <param name="receivers">Destinatari cui indirizzare la spedizione</param>
        /// <returns>Risultato della spedizione</returns>
        public static DocsPaVO.Interoperabilita.SendDocumentResponse SpedisciPerInteroperabilitaSemplificata(
            DocsPaVO.documento.SchedaDocumento schedaDoc,
            DocsPaVO.utente.InfoUtente infoUtente,
            IEnumerable<DocsPaVO.Spedizione.DestinatarioEsterno> receivers)
        {
            // Oggetto contenente l'esito della spedizione del documento ai destinatari.
            // Per ognuno di essi, nella tabella "DPA_STATO_INVIO",
            // deve essere impostata correttamente la data di spedizione; per gli 
            // altri viene impostata a null;
            DocsPaVO.Interoperabilita.SendDocumentResponse sendDocumentResponse = null;
            //try
            //{
            //    InsertStatoInvioISPreliminare(schedaDoc, receivers);
            //}
            //catch (Exception e1)
            //{
            //    logger.Error("Errore nell'inserimento preliminare dei destinatari nella DPA_STATO_INVIO", e1);
            //}

            try
            {
                DocsPaVO.utente.Corrispondente mittente = BusinessLogic.Utenti.UserManager.getCorrispondente(infoUtente.idCorrGlobali, false);

                DocsPaVO.utente.Ruolo ruoloMitt = (DocsPaVO.utente.Ruolo)mittente;
                mittente.descrizione = ruoloMitt.descrizione;

                // Invio del documento a tutti i destinatari
                sendDocumentResponse = BusinessLogic.Interoperabilità.InteroperabilitaInvioSegnatura.SendDocumentIS(schedaDoc, infoUtente, receivers);
            }
            catch (Exception e)
            {
                logger.Error("Errore nella richiesta di interoperabilità semplificata", e);

                throw new Exception("Si è verificato un errore durante l'invio della richiesta di interoperabilità: " + e.Message);
            }

            //try
            //{
            //    List<DocsPaVO.utente.Corrispondente> c = new List<DocsPaVO.utente.Corrispondente>();
            //    foreach (DocsPaVO.Spedizione.DestinatarioEsterno de in receivers)
            //        c.AddRange(de.DatiDestinatari);

            //    InsertStatoInvioDestinatari(schedaDoc, new ArrayList(c), sendDocumentResponse);

            //    BusinessLogic.interoperabilita.Semplificata.InteroperabilitaSemplificataManager.IS_statusMaskUpdater(schedaDoc.systemId);
            //}
            //catch (Exception e)
            //{
            //    logger.Error("Errore nella gestione della spedizione", e);

            //    throw new Exception("Errore nella gestione della spedizione:\\n\\n" + e.Message);
            //}

            return sendDocumentResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedaDoc"></param>
        /// <param name="mailAddress"></param>
        /// <param name="destinatari"></param>
        /// <param name="infoUtente"></param>
        /// <param name="confermaRic"></param>
        /// <returns></returns>
        public static DocsPaVO.Interoperabilita.SendDocumentResponse spedisci(
                    DocsPaVO.documento.SchedaDocumento schedaDoc,
                    string mailAddress,
                    ArrayList destinatari,
                    DocsPaVO.utente.InfoUtente infoUtente,
                    bool confermaRic)
        {
            return spedisci(schedaDoc, null, mailAddress, destinatari, infoUtente, confermaRic);
        }

		/// <summary>
		/// Invio del documento ad un singolo indirizzo mail,
		/// cui possono corrispondere 1 o più destinatari
		/// </summary>
		/// <param name="schedaDoc"></param>
		/// <param name="mailAddress"></param>
		/// <param name="infoUtente"></param>
		/// <param name="confermaRic"></param>
		/// <returns></returns>
		public static DocsPaVO.Interoperabilita.SendDocumentResponse spedisci(
            DocsPaVO.documento.SchedaDocumento schedaDoc,
            string mailAddress,
            DocsPaVO.utente.InfoUtente infoUtente,
            bool confermaRic)
		{
            ArrayList listDestinatari = new ArrayList();

            FillListDestinatari(listDestinatari, mailAddress, (((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari));
            FillListDestinatari(listDestinatari, mailAddress, (((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza));

            return spedisci(schedaDoc, mailAddress, listDestinatari, infoUtente, confermaRic);
		}

		private static void FillListDestinatari(ArrayList listToFill,string mailAddress,ArrayList destinatari)
		{
			if (destinatari!=null && listToFill!=null)
			{
				foreach (DocsPaVO.utente.Corrispondente corrispondente in destinatari)
                //luluciani://if (mailAddress.Equals(corrispondente.email))
                    if (Interoperabilità.InteroperabilitaUtils.InteropIntNoMail)
                    {
                        if (corrispondente.tipoIE != null)
                        {
                            if (corrispondente.tipoIE.Equals("I"))
                            {
                                if (GetCorrispondenteInterop(mailAddress,corrispondente))
                                    listToFill.Add(corrispondente);
                            }
                            else if (corrispondente.tipoIE.Equals("E"))
                            {
					if (mailAddress.Equals(corrispondente.email))
						listToFill.Add(corrispondente);

                            }
                        }
                        else // è un occasionale con mail quindi spedisco
                        //anche se interopNomail  abilitata (ab. ANAS 17/04/2008)
                        {
                            if (mailAddress.Equals(corrispondente.email))
                                listToFill.Add(corrispondente);
                        }

                    }
                    else
                    {
                        if (mailAddress.Equals(corrispondente.email))
                            listToFill.Add(corrispondente);

                    }
			}
		}
        /// <summary>
        /// restituisce true, se il corrisposondente ha valorizzato codiceAOO / mailaddress 
        /// considerando i vari casi tra uo/ruolo/utente
        /// </summary>
        /// <param name="mailAddress"></param>
        /// <param name="corrispondente"></param>
        /// <returns></returns>
        private static bool GetCorrispondenteInterop(string mailAddress,DocsPaVO.utente.Corrispondente corrispondente)
        {
            bool rtn = false;
            if (corrispondente.GetType().Equals(typeof(DocsPaVO.utente.UnitaOrganizzativa)))
            {
                if (corrispondente.codiceAOO != null && mailAddress.Equals(corrispondente.codiceAOO))
                    rtn = true;
                
            }
            if (corrispondente.GetType().Equals(typeof(DocsPaVO.utente.Ruolo)))
            {
                DocsPaVO.utente.Ruolo ruo = ((DocsPaVO.utente.Ruolo)corrispondente);
                if (ruo.uo.codiceAOO != null && mailAddress.Equals(ruo.uo.codiceAOO))
                    rtn = true;
            }
            if (corrispondente.GetType().Equals(typeof(DocsPaVO.utente.Utente)))
            {
                DocsPaVO.utente.Utente ut = ((DocsPaVO.utente.Utente)corrispondente);
                if (ut.ruoli.Count > 0)
                {
                    DocsPaVO.utente.Ruolo ruo = ((DocsPaVO.utente.Ruolo)ut.ruoli[0]);
                    if (ruo.uo.codiceAOO != null && mailAddress.Equals(ruo.uo.codiceAOO))
                        rtn = true;
                }
            }
            return rtn;
        }
    

        private static void FillListDestinatari(ArrayList listToFill, ArrayList destinatari)
        {
            if (destinatari != null && listToFill != null)
            {
                foreach (DocsPaVO.utente.Corrispondente corrispondente in destinatari)
                            listToFill.Add(corrispondente);
            }
        }

		/// <summary>
		/// Invio del documento a tutti i destinatari del documento
		/// </summary>
		/// <param name="schedaDoc"></param>
		/// <param name="infoUtente"></param>
		/// <param name="confermaRic"></param>
		public static DocsPaVO.Interoperabilita.SendDocumentResponse spedisci(DocsPaVO.documento.SchedaDocumento schedaDoc,DocsPaVO.utente.InfoUtente infoUtente,bool confermaRic) 
		{
			// Oggetto contenente l'esito della spedizione del documento ai destinatari.
			// Per ognuno di essi, nella tabella "DPA_STATO_INVIO",
			// deve essere impostata correttamente la data di spedizione; per gli 
			// altri viene impostata a null;
			DocsPaVO.Interoperabilita.SendDocumentResponse sendDocumentResponse=null;
 
			try
			{
				DocsPaVO.utente.Corrispondente mittente=BusinessLogic.Utenti.UserManager.getCorrispondente(infoUtente.idCorrGlobali,false);

				DocsPaVO.utente.Ruolo ruoloMitt=(DocsPaVO.utente.Ruolo) mittente;
				mittente.descrizione = ruoloMitt.descrizione;
                DocsPaVO.Interoperabilita.DatiInteropAutomatica dia = null;
				// Invio mail a tutti i destinatari del documento
                sendDocumentResponse = BusinessLogic.Interoperabilità.InteroperabilitaInvioSegnatura.SendDocument(schedaDoc, schedaDoc.registro, infoUtente, confermaRic, out dia);
			}
			catch (Exception e) 
			{
				logger.Debug("Errore nell'invio mail",e);
				
				throw new Exception("Si è verificato un errore nell'invio dell'email:\\n\\n" + e.Message);
			}

            try
            {
                DocsPaVO.documento.ProtocolloUscita pu = (DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo;

                // destinatari
                InsertStatoInvioDestinatari(schedaDoc, pu.destinatari, sendDocumentResponse);

                // destinatari per conoscenza 
                InsertStatoInvioDestinatari(schedaDoc, pu.destinatariConoscenza, sendDocumentResponse);

                //SOLO nel caso di interoperabilità interna abilitata
                if (System.Configuration.ConfigurationManager.AppSettings["INTEROP_INT_NO_MAIL"] != null &&
                            System.Configuration.ConfigurationManager.AppSettings["INTEROP_INT_NO_MAIL"].ToString() != "0")
                {
                    foreach (DocsPaVO.Interoperabilita.SendDocumentResponse.SendDocumentMailResponse sdml in sendDocumentResponse.SendDocumentMailResponseList)
                    {
                        if (sdml.datiInteropAutomatica != null)
                        {
                            string erroreMessage = string.Empty;
                            Interoperabilità.InteroperabilitaInvioRicevuta.sendRicevutaRitorno(sdml.datiInteropAutomatica.schedaDoc, sdml.datiInteropAutomatica.registro, sdml.datiInteropAutomatica.ruolo, sdml.datiInteropAutomatica.infoUtente,out erroreMessage);
                            //
                            bool verificaRagioni;
                            string message = "";
                            // OLD: MAC_INPS 3749 if (schedaDoc.protocollo.daProtocollare != null && schedaDoc.protocollo.daProtocollare.Equals("1") && reg.autoInterop != null && reg.autoInterop != "0")
                           // BusinessLogic.trasmissioni.TrasmProtoIntManager.TrasmissioneProtocolloAutomatico(sdml.datiInteropAutomatica.schedaDoc,sdml.datiInteropAutomatica.registro.systemId, schedaDoc, sdml.datiInteropAutomatica.ruolo, sdml.datiInteropAutomatica.infoUtente, infoUtente.urlWA, false, out verificaRagioni, out message);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella gestione della spedizione", e);

                throw new Exception("Errore nella gestione della spedizione:\\n\\n" + e.Message);
            }
			
			
			return sendDocumentResponse;
		}

        
		public static void InsertStatoInvioDestinatari(DocsPaVO.documento.SchedaDocumento schedaDocumento,ArrayList listDestinatari,DocsPaVO.Interoperabilita.SendDocumentResponse sendDocumentResponse)
		{
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

			foreach (DocsPaVO.utente.Corrispondente corrispondente in listDestinatari)
			{
				if(schedaDocumento.protocollo.GetType() == typeof(DocsPaVO.documento.ProtocolloUscita))
				{
					string idDocArrivoPar=doc.GetIdDocArrivoPar(schedaDocumento.systemId,corrispondente,"'D','C','F'");
					
					string mailCorrispondente=corrispondente.email;
					if  (mailCorrispondente==null)
						mailCorrispondente=string.Empty;

					// Reperimento esito invio della mail al destinatario corrente
                    DocsPaVO.Interoperabilita.SendDocumentResponse.SendDocumentMailResponse mailResponse = sendDocumentResponse.FindDocumentResponse(corrispondente);
                    //if(corrispondente.tipoCorrispondente!="O")
						doc.InsertStatoInvio(corrispondente,idDocArrivoPar,schedaDocumento.registro.systemId,schedaDocumento.systemId, (mailResponse==null || (mailResponse != null && !mailResponse.SendSucceded && !string.IsNullOrEmpty(mailResponse.SendErrorMessage))), mailResponse.MailAddress);

                }
			}
            
            
		}

        /// <summary>
        /// Dato un problema con il report spedizioni per l'aggiornamento di un numero elevato di corrispondenti
        /// si fa un inserimento preliminare nella DPA_STATO_INVIO
        /// </summary>
        /// <param name="schedaDoc"></param>
        /// <param name="receivers"></param>
        public static void InsertStatoInvioISPreliminare(DocsPaVO.documento.SchedaDocumento schedaDoc,
            IEnumerable<DocsPaVO.Spedizione.DestinatarioEsterno> receivers)
        {
            List<DocsPaVO.utente.Corrispondente> c = new List<DocsPaVO.utente.Corrispondente>();
            foreach (DocsPaVO.Spedizione.DestinatarioEsterno de in receivers)
                c.AddRange(de.DatiDestinatari);
            ArrayList destinatari = new ArrayList(c);
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

            foreach (DocsPaVO.utente.Corrispondente corrispondente in destinatari)
            {
                if (schedaDoc.protocollo.GetType() == typeof(DocsPaVO.documento.ProtocolloUscita))
                {
                    string idDocArrivoPar = doc.GetIdDocArrivoPar(schedaDoc.systemId, corrispondente, "'D','C','F'");
                    doc.InsertStatoInvio(corrispondente, idDocArrivoPar, schedaDoc.registro.systemId, schedaDoc.systemId,"");

                }
            }
        }

		public static bool VerificaSpedizione(string idProfile)
		{
			bool result=false;
			try
			{
				DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
				result = doc.verificaSpedizione(idProfile);
			}
			catch
			{
				logger.Debug("Errore nella verifica della spedizione");
				throw new Exception();
			}
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="idCorrispondente"></param>
		/// <returns></returns>
		private static string getCondIdGruppo(string idCorrispondente) 
		{
			ArrayList lista = getIdCorrispondentiGruppo(idCorrispondente);
			if (lista.Count == 0)
				return "";
			string inStr = " AND ID_CORR_GLOBALE IN ("+ (string)lista[0];
			for (int i=1; i< lista.Count; i++) 
				inStr += ","+ (string)lista[i];
			inStr += ")";
			return inStr;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="infoDoc"></param>
		/// <param name="corr"></param>
		/// <returns></returns>
		public static ArrayList aggiornamentoConferma(string idProfile, DocsPaVO.utente.Corrispondente corr) 
		{
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			return doc.AggiornamentoConferma(idProfile, corr);

			#region Codice Commentato
			/*if (infoDoc == null || infoDoc.idProfile == null)
				return null;
			ArrayList lista = new ArrayList();
			
			DocsPa_V15_Utils.Database db = DocsPa_V15_Utils.dbControl.getDatabase();
			try
			{
				db.openConnection();
				string queryString =
					"SELECT SYSTEM_ID,VAR_CODICE_AOO,VAR_CODICE_AMM,VAR_PROTO_DEST," +
					DocsPa_V15_Utils.dbControl.toChar("DTA_PROTO_DEST",false) + " AS DTA_PROTO_DEST, " +
					"ID_CORR_GLOBALE, ID_DOCUMENTTYPE FROM DPA_STATO_INVIO " +
					"WHERE ID_PROFILE=" + infoDoc.idProfile; 
				string queryString ="";
				if(!corr.GetType().Equals(typeof(DocsPaVO.utente.Gruppo)))
					queryString += " AND ID_CORR_GLOBALE=" + corr.systemId;
				else
					queryString += getCondIdGruppo(db, corr.systemId);
				DataSet dataSet;
				logger.Debug(queryString);
				db.fillTable(queryString, dataSet, "STATO_INVIO");
				
				foreach (DataRow dr in dataSet.Tables["STATO_INVIO"].Rows)
				{
					DocsPaVO.documento.ProtocolloDestinatario pd = new DocsPaVO.documento.ProtocolloDestinatario();
					pd.systemId = dr["SYSTEM_ID"].ToString();
					pd.codiceAOO = dr["VAR_CODICE_AOO"].ToString();
					pd.codiceAmm = dr["VAR_CODICE_AMM"].ToString();
					pd.protocolloDestinatario = dr["VAR_PROTO_DEST"].ToString();
					pd.dataProtocolloDestinatario = dr["DTA_PROTO_DEST"].ToString();
					if(dr["ID_CORR_GLOBALE"].ToString().Equals(corr.systemId))
						pd.descrizioneCorr = corr.descrizione;
					else
						pd.descrizioneCorr = UserManager.getCorrispondente(db, dr["ID_CORR_GLOBALE"].ToString()).descrizione;
					string idDocumentType;
					if (dr["ID_DOCUMENTTYPE"] != null)
					{
						idDocumentType = dr["ID_DOCUMENTTYPE"].ToString();
						pd.documentType = getTipoDocumento(idDocumentType,db);
					}
					lista.Add(pd);
				}

				db.closeConnection();
			} 
			catch (Exception e) 
			{
				logger.Debug (e.Message);				
				db.closeConnection();
				throw new Exception("F_System");
			}
			return lista;*/
			#endregion
		}

		//nuovo 30/07/2003 gestione tipo spedizione
		/// <summary>
		/// </summary>
		/// <param name="idDocumentType"></param>
		/// <returns></returns>
		private static string getTipoDocumento(string idDocumentType)
		{
			#region Codice Commentato
			/*if (idDocumentType == null || idDocumentType.Equals(""))
				return "";
			string queryString = "SELECT DESCRIPTION FROM DOCUMENTTYPES WHERE SYSTEM_ID = " + idDocumentType;
			logger.Debug(queryString);
			try 
			{
					object descr = db.executeScalar(queryString);
					if (descr != null)
						return db.executeScalar(queryString).ToString();
					else
						return "";
			} 
			catch (Exception) 
			{
				return "";
			}*/
			#endregion

			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			return doc.GetTipoDocumento(idDocumentType);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cor"></param>
		/// <param name="idProfile"></param>
		/// <returns></returns>
		internal static DocsPaVO.utente.Corrispondente getDatiSpedizione(DocsPaVO.utente.Corrispondente cor, string idProfile)
		{
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			doc.GetDatiSpedizione(ref cor, idProfile);
			return cor;

			#region Codice Commentato
			/*ArrayList lista = new ArrayList();
			string idDocumentType = null;
			string queryString="";
			DocsPa_V15_Utils.Database db = DocsPa_V15_Utils.dbControl.getDatabase();
			try 
			{
				db.openConnection();
				string queryString =
					"SELECT ID_DOCUMENTTYPE FROM DPA_STATO_INVIO " +
					"WHERE ID_PROFILE=" + idProfile; 

				if(!cor.GetType().Equals(typeof(DocsPaVO.utente.Gruppo)))
					queryString += " AND ID_CORR_GLOBALE=" + cor.systemId;
				else
					queryString += getCondIdGruppo(db, cor.systemId);

				logger.Debug(queryString);
				DataSet dataSet;
				db.fillTable(queryString, dataSet, "STATO_INVIO");
				
				foreach (DataRow dr in dataSet.Tables["STATO_INVIO"].Rows) 
				{
					idDocumentType = dr["ID_DOCUMENTTYPE"].ToString();
				}
				db.closeConnection();
			} 
			catch (Exception e) 
			{
				logger.Debug (e.Message);				
				db.closeConnection();
				throw new Exception("F_System");
			}
			if (idDocumentType!=null && !idDocumentType.Equals(""))
			{
				cor.canalePref = new DocsPaVO.utente.Canale();
				cor.canalePref.systemId = idDocumentType;
			}
			return cor;*/
			#endregion
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="corr"></param>
		/// <param name="idProfile"></param>
		internal static void updateTipoSpedizione(DocsPaVO.utente.Corrispondente corr, string idProfile)
		{
			#region Codice Commentato
			/*if (corr != null && corr.canalePref != null) 
			{
				string typeId;
				if (corr.canalePref.systemId == null)
					typeId = "NULL";
				else
					typeId = corr.canalePref.systemId;
				string updateString =
					"UPDATE DPA_STATO_INVIO SET " +
					"ID_DOCUMENTTYPE = "  + typeId + 
					" WHERE ID_PROFILE=" + idProfile + " AND ID_CORR_GLOBALE = " + corr.systemId;
				
				logger.Debug(updateString);
				db.executeLocked(updateString);
			}*/
			#endregion

			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			doc.UpdateTipoSpedizione(corr, idProfile);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="idProfile"></param>
		/// <returns></returns>
		internal static ArrayList getIdCorrInStatoInvio(string idProfile) 
		{
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			return doc.GetIdCorrInStatoInvio(idProfile);

			#region Codice Commentato
			/*logger.Debug("get IdCorr in DPA_STATO_INVIO");
			ArrayList listaIdCorr = new ArrayList();
			//ricerca dei Server Posta
			string queryString = 
				"SELECT ID_CORR_GLOBALE " +
				"FROM DPA_STATO_INVIO " +
				"WHERE ID_PROFILE = " + idProfile;
			
			logger.Debug(queryString);
			IDataReader dr = db.executeReader(queryString);
			while(dr.Read())
			{
				listaIdCorr.Add(dr.GetValue(0).ToString());
			}
			dr.Close();
			return listaIdCorr;*/
			#endregion
		}

        //GESTIONE SPEDIZIONI
        public static ArrayList getSpedizioni(string idProfile)
        {
            DocsPaDB.Query_DocsPAWS.Interoperabilita interop = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
            return interop.GetSpedizioni(idProfile);

        }

	}
}
