using System;
using System.Data;
using System.Collections;
using System.Configuration;
using System.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Generic;
using DocsPaVO.trasmissione;
using log4net;
using BusinessLogic.ProfilazioneDinamica;
using BusinessLogic.Interoperabilità;
using System.IO;
using DocsPaVO.Notification;

namespace BusinessLogic.SmistamentoDocumenti
{
	/// <summary>
	/// Summary description for Smistamento.
	/// </summary>
	public class SmistamentoManager
	{
        private static ILog logger = LogManager.GetLogger(typeof(SmistamentoManager));

		public SmistamentoManager()
		{
		}

		#region PUBLIC

		/// <summary>
		/// verifica l'esistenza delle ragioni di trasmissione: COMPETENZA e CONOSCENZA
		/// </summary>
		/// <param name="idAmm"></param>
		/// <returns>TRUE: esistono le ragioni di trasmissione per lo smistamento; FALSE: non esistono</returns>
		public static bool VerificaRagTrasmSmista(string idAmm)
		{
			bool retValue = false;
			DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti dbSmistaDoc=new DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti();
			DataSet ds = dbSmistaDoc.VerificaRagTrasmSmista(idAmm);
			if (ds!=null)
			{
				if(ds.Tables["RAG_TRASM"].Rows.Count==2)
				{
					retValue = true;
				}
			}
			return retValue;
		}

        /// <summary>
        /// Reperimento delle trasmissioni ricevute
        /// </summary>
        /// <param name="mittente"></param>
        /// <param name="filtriRicerca">Filtri per la ricerca dei documenti trasmessi</param>
        /// <returns></returns>
        public static ArrayList GetListDocumentiTrasmessi(DocsPaVO.Smistamento.MittenteSmistamento mittente, DocsPaVO.filtri.FiltroRicerca[] filtriRicerca)
        {
            DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti dbSmistaDoc = new DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti();
            DataSet ds = dbSmistaDoc.GetListDocumentiTrasmessi(mittente, filtriRicerca);
            dbSmistaDoc = null;

            return GetListDocumentiTrasmessi(ds);
        }

        /// <summary>
        /// Reperimento delle trasmissioni ricevute
        /// </summary>
        /// <param name="mittente"></param>
        /// <returns></returns>
       /// <summary>
       /// 
       /// </summary>
       /// <param name="idAmm"></param>
       /// <returns></returns>
        public static ArrayList GetRagioniTrasmissioneSmistamento(string idAmm)
        {
            ArrayList arrayRag = new ArrayList();            
            DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti dbSmistaDoc = new DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti();
            
            DataSet ds = dbSmistaDoc.VerificaRagTrasmSmista(idAmm);
            
            if (ds != null)
            {
                if (ds.Tables["RAG_TRASM"].Rows.Count > 0)
                {                    
                    foreach (DataRow r in ds.Tables["RAG_TRASM"].Rows)
                    {
                        DocsPaVO.trasmissione.RagioneTrasmissione ragione = new DocsPaVO.trasmissione.RagioneTrasmissione();
                        ragione.systemId = r["SYSTEM_ID"].ToString();
                        ragione.descrizione = r["VAR_DESC_RAGIONE"].ToString();
                        ragione.tipo = r["CHA_TIPO_RAGIONE"].ToString();
                        ragione.tipoDiritti = (DocsPaVO.trasmissione.TipoDiritto)DocsPaDB.Utils.HashTableManager.GetKeyFromValue(DocsPaVO.trasmissione.RagioneTrasmissione.tipoDirittoStringa, r["CHA_TIPO_DIRITTI"].ToString());
                        ragione.tipoDestinatario = (DocsPaVO.trasmissione.TipoGerarchia)DocsPaDB.Utils.HashTableManager.GetKeyFromValue(DocsPaVO.trasmissione.RagioneTrasmissione.tipoGerarchiaStringa, r["CHA_TIPO_DEST"].ToString());
                        ragione.risposta = r["CHA_RISPOSTA"].ToString();
                        ragione.note = r["VAR_NOTE"].ToString();
                        ragione.eredita = r["CHA_EREDITA"].ToString();
                        ragione.tipoRisposta = r["CHA_TIPO_RISPOSTA"].ToString();
                        ragione.prevedeCessione = r["CHA_CEDE_DIRITTI"].ToString();

                        arrayRag.Add(ragione);
                    }                    
                }   
            }
            return arrayRag;
        }

		/// <summary>
		/// Reperimento delle trasmissioni ricevute
		/// </summary>
		/// <param name="systemIDRuolo"></param>
		/// <param name="idPeople"></param>
		/// <returns></returns>
		public static ArrayList GetListDocumentiTrasmessi(DocsPaVO.Smistamento.MittenteSmistamento mittente)
		{
            DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti dbSmistaDoc = new DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti();
            DataSet ds = dbSmistaDoc.GetListDocumentiTrasmessi(mittente);
            dbSmistaDoc = null;
            
            return GetListDocumentiTrasmessi(ds);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idDocumento"></param>
        /// <param name="infoUtente"></param>
        /// <param name="content"></param>
        /// <param name="convertPdfInline"></param>
        /// <param name="isConverted"></param>
        /// <returns></returns>
        public static DocsPaVO.Smistamento.DocumentoSmistamento GetDocumentoSmistamento(string idDocumento, DocsPaVO.utente.InfoUtente infoUtente, bool content, bool convertPdfInline, out bool isConverted)
        {
            isConverted = false;

            DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti dbSmistaDoc=new DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti();
			DataSet ds=dbSmistaDoc.GetDocumentoSmistamento(idDocumento);
			dbSmistaDoc=null;

			DocsPaVO.Smistamento.DocumentoSmistamento retValue=null;

			if (ds.Tables.Count>0 && ds.Tables["DOCUMENTI"].Rows.Count>0)
			{
				DataRow rowDocumento=ds.Tables["DOCUMENTI"].Rows[0];

				DataTable tableMittDest=null;

				if (rowDocumento["TipoDocumento"].ToString().Equals("A") ||  
					rowDocumento["TipoDocumento"].ToString().Equals("P") || 
					rowDocumento["TipoDocumento"].ToString().Equals("I"))
					tableMittDest=ds.Tables["MITT_DEST"];

				retValue=CreateDocumentoTrasmesso(rowDocumento,tableMittDest);

				// Reperimento dati documento (oggetto "FileDocument")
				// e degli allegati (arraylist di oggetti "FileRequest")
				if (retValue!=null && ds.Tables["COMPONENTS"].Rows.Count>0)
				{
                    retValue.ImmagineDocumento = GetFileDocument(ds.Tables["COMPONENTS"].Rows[0], infoUtente, content, convertPdfInline, out isConverted);
				}
			}

			return retValue;
        }

		/// <summary>
		/// Reperimento dati del documento corrente
		/// </summary>
		/// <param name="idDocumento"></param>
		/// <param name="infoUtente"></param>
		/// <param name="visualizzaImmagineDocumento"></param>
		/// <returns></returns>
		public static DocsPaVO.Smistamento.DocumentoSmistamento GetDocumentoSmistamento(string idDocumento,DocsPaVO.utente.InfoUtente infoUtente,bool content)
		{
            bool isConverted;
            return GetDocumentoSmistamento(idDocumento, infoUtente, content, false, out isConverted);
		}

		/// <summary>
		/// Reperimento oggetto "DocsPaVO.SmistamentoDocumenti.UnitaOrganizzativa" 
		/// contenente i ruoli gerarchicamente inferiori definiti nell'ambito
        /// della propria UO o della UO corrente (se siamo in smista_naviga_UO)
		/// </summary>
        /// <param name="IDUO">system_id della UO (dpa_corr_globali)</param>
        /// <param name="mittente">Oggetto mittente</param>
        /// <param name="isCurrentUO">True se deve reperire la UO corrente (navigazione UO in smistamento), altimenti False (default)</param>
		/// <returns></returns>
        public static DocsPaVO.Smistamento.UOSmistamento GetUOAppartenenza(string idUnitaOrganizzativa, DocsPaVO.Smistamento.MittenteSmistamento mittente, bool isCurrentUO)
		{
			DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti dbSmistaDoc=new DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti();
            DataSet ds = dbSmistaDoc.GetUOAppartenenza(idUnitaOrganizzativa, mittente.RegistriAppartenenza, mittente.IDAmministrazione, mittente.LivelloRuolo, isCurrentUO);
			dbSmistaDoc=null;

            if (ds.Tables["UO"].Rows.Count > 0)
            {
                ArrayList list = CreateUOSmistamento(ds, mittente.IDPeople, mittente.IDCorrGlobaleRuolo);
                if (list.Count > 0)
                    return (DocsPaVO.Smistamento.UOSmistamento)list[0];
                else
                    return null;
            }
            else
                return null;
		}

        /// <summary>
        /// Restituisce la lista degli id_corr_globali all'interno della UO di Appartenenza, per cui il documento è stato già trasmesso
        /// </summary>
        /// <param name="idCorrGlobali"></param>
        /// <param name="docnumber"></param>
        /// <param name="mittente"></param>
        /// <returns></returns>
        public static string[] GetIdDestinatariTrasmDocInUo(string idUo, string docnumber)
        {
            DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti dbSmistaDoc = new DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti();
            return dbSmistaDoc.GetIdDestinatariTrasmDocInUo(idUo, docnumber);
        }

        /// <summary>
        /// Reperimento array di oggetti "DocsPaVO.SmistamentoDocumenti.UnitaOrganizzativa" 
        /// contenente i ruoli gerarchicamente inferiori definiti nell'ambito
        /// delle UO direttamente inferiori
        /// </summary>
        /// <param name="IDUO"></param>
        /// <returns></returns>
        public static ArrayList GetUOInferiori(string idUOAppartenenza, DocsPaVO.Smistamento.MittenteSmistamento mittente)
		{
			ArrayList UoList = null;

			DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti dbSmistaDoc=new DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti();
			DataSet ds=dbSmistaDoc.GetUOInferiori(idUOAppartenenza, mittente.RegistriAppartenenza, mittente.IDAmministrazione, mittente.LivelloRuolo);
			dbSmistaDoc=null;

			if(ds.Tables[0].Rows.Count > 0)
			{
				UoList = CreateUOSmistamento(ds,mittente.IDPeople,mittente.IDCorrGlobaleRuolo);
			}
			
			return UoList;			
		}

		
		/// <summary>
		/// Reperimento delle UO (i cui riferimento sono persistiti nella
		/// tabella "DPA_UO_SMISTAMENTO") cui è possibile smistare il documento
		/// </summary>
		/// <param name="idRegistro"></param>
		/// <param name="mittente"></param>
		/// <returns></returns>
		public static ArrayList GetListUOSmistamento(string idRegistro,DocsPaVO.Smistamento.MittenteSmistamento mittente)
		{
			ArrayList retValue=new ArrayList();

			DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti dbSmistaDoc=new DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti();
			DataSet ds=dbSmistaDoc.GetUOSmistamento(idRegistro, mittente.LivelloRuolo);
			dbSmistaDoc=null;

			if(ds.Tables["UO"].Rows.Count > 0)
				retValue=CreateUOSmistamento(ds,mittente.IDPeople,mittente.IDCorrGlobaleRuolo);
			
			return retValue;			
		}

		

		/// <summary>
		/// Invio del documento trasmesso ai destinatari selezionati
		/// </summary>
		/// <param name="documentoTrasmesso"></param>
		/// <param name="uoAppartenenza"></param>
		/// <param name="uoInferiori"></param>
		/// <param name="alertMessage"></param>
		/// <returns></returns>
		public static ArrayList SmistaDocumento(
									DocsPaVO.Smistamento.MittenteSmistamento mittente,
									DocsPaVO.utente.InfoUtente infoUtente,
									DocsPaVO.Smistamento.DocumentoSmistamento documentoTrasmesso,
									DocsPaVO.Smistamento.DatiTrasmissioneDocumento datiDocumentoTrasmesso,	
									DocsPaVO.Smistamento.UOSmistamento uoAppartenenza,
									DocsPaVO.Smistamento.UOSmistamento[] uoInferiori,
                                    string httpFullPath)
        {
           
            ArrayList retValue = new ArrayList();

            // Verifica che, tra le UO destinatarie, sia selezionato almeno un destinatario per lo smistamento del documento
            bool almostOneChecked = AlmostOneCheckedInUO(uoAppartenenza);

            if (almostOneChecked)
                retValue = SmistaDocumentoRic(mittente, infoUtente, documentoTrasmesso, datiDocumentoTrasmesso, uoAppartenenza, uoInferiori, httpFullPath, retValue);
            else
                retValue.Add(
                    new DocsPaVO.Smistamento.EsitoSmistamentoDocumento
                    {
                        CodiceEsitoSmistamento = -1,
                        DescrizioneEsitoSmistamento = "Nessun destinatario selezionato per lo smistamento del documento"
                    });

            return retValue;
       }

        private static ArrayList SmistaDocumentoRic(
									DocsPaVO.Smistamento.MittenteSmistamento mittente,
									DocsPaVO.utente.InfoUtente infoUtente,
									DocsPaVO.Smistamento.DocumentoSmistamento documentoTrasmesso,
									DocsPaVO.Smistamento.DatiTrasmissioneDocumento datiDocumentoTrasmesso,	
									DocsPaVO.Smistamento.UOSmistamento uoAppartenenza,
									DocsPaVO.Smistamento.UOSmistamento[] uoInferiori,
                                    string httpFullPath, ArrayList retValue)
        {
            DocsPaVO.trasmissione.RagioneTrasmissione ragioneSmistamento;
            DocsPaVO.trasmissione.RagioneTrasmissione ragCompetenza;
            DocsPaVO.trasmissione.RagioneTrasmissione ragConoscenza;

            string accessRights = string.Empty;
            bool modelloNoNotify = uoAppartenenza.modelloNoNotify;

            Hashtable destinatariMail = new Hashtable();
            Hashtable destinatariMailRuolo = new Hashtable();

            DocsPaVO.Smistamento.EsitoSmistamentoDocumento esitoSmistamento = null;

            DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti smistaDoc = new DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti();

            DocsPaDocumentale.Interfaces.IAclEventListener eventsNotification = new DocsPaDocumentale.Documentale.AclEventListener(infoUtente);

            //DocsPaVO.trasmissione.RagioneTrasmissione ragCompetenza = Trasmissioni.RagioniManager.getRagioneByCodice(infoUtente.idAmministrazione, "COMPETENZA");
            //DocsPaVO.trasmissione.RagioneTrasmissione ragConoscenza = Trasmissioni.RagioniManager.getRagioneByCodice(infoUtente.idAmministrazione, "CONOSCENZA");

            ArrayList listaRagSmista;
            listaRagSmista = GetAllRagioniTrasmissioneSmistamento(infoUtente.idAmministrazione);//GetRagioniTrasmissioneSmistamento(infoUtente.idAmministrazione);

            RagioneTrasmissione[] trasmRags = (RagioneTrasmissione[])listaRagSmista.ToArray(typeof(RagioneTrasmissione));
            
            DocsPaVO.amministrazione.InfoAmministrazione amm = Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoUtente.idAmministrazione);

            if (listaRagSmista != null)//&& listaRagSmista.Count.Equals(2))
            {
                ragCompetenza = trasmRags.Where(e => e.systemId == amm.IDRagioneCompetenza).FirstOrDefault(); //GetRagioneSelezionata("COMPETENZA", listaRagSmista);//(DocsPaVO.trasmissione.RagioneTrasmissione)listaRagSmista[0];
                ragConoscenza = trasmRags.Where(e => e.systemId == amm.IDRagioneConoscenza).FirstOrDefault(); //GetRagioneSelezionata("CONOSCENZA", listaRagSmista);//(DocsPaVO.trasmissione.RagioneTrasmissione)listaRagSmista[1];
            }
            else
            {
                esitoSmistamento = new DocsPaVO.Smistamento.EsitoSmistamentoDocumento();
                esitoSmistamento.CodiceEsitoSmistamento = 199;
                esitoSmistamento.DescrizioneEsitoSmistamento = "ragioni di trasmissioni per lo smistamento non impostate correttamente";
                retValue.Add(esitoSmistamento);
                esitoSmistamento = null;
                return retValue;
            }


            // --------------------------------------- UO appartenenza -----------------------------------------------
            if (uoAppartenenza != null)
            {
                if (uoAppartenenza.FlagCompetenza || uoAppartenenza.FlagConoscenza)
                {
                    DocsPaVO.Smistamento.RuoloSmistamento[] ruoli = (DocsPaVO.Smistamento.RuoloSmistamento[])
                                    uoAppartenenza.Ruoli.ToArray(typeof(DocsPaVO.Smistamento.RuoloSmistamento));

                    int countComp = ruoli.Count(e => e.FlagCompetenza == true);
                    int countCC = ruoli.Count(e => e.FlagConoscenza == true);

                    if (countComp == 0 && countCC == 0)
                    {
                        foreach (DocsPaVO.Smistamento.RuoloSmistamento item in ruoli.Where(e => e.RuoloRiferimento).ToArray())
                        {
                            item.FlagCompetenza = uoAppartenenza.FlagCompetenza;
                            item.FlagConoscenza = uoAppartenenza.FlagConoscenza;
                        }
                    }
                }

                foreach (DocsPaVO.Smistamento.RuoloSmistamento ruolo in uoAppartenenza.Ruoli)
                {   
                    // è stato selezionato un ruolo oppure se è stato selezionato un modello di trasmissione rapida
                    //che prevede la trasmissione a UO (ruoli di riferimento) o a ruolo 
                    if (ruolo.FlagCompetenza || ruolo.FlagConoscenza || !string.IsNullOrEmpty(ruolo.ragioneTrasmRapida) || !string.IsNullOrEmpty(uoAppartenenza.ragioneTrasmRapida))
                    {
                        logger.Debug("Smistamento a Ruolo in UO di appartenenza: " + ruolo.Descrizione);

                        if (AccessToExecute(documentoTrasmesso.IDRegistro, ruolo.Registri))
                        {
                            if (ruolo.FlagCompetenza)
                                ragioneSmistamento = ragCompetenza;
                            else
                                ragioneSmistamento = ragConoscenza;
                            if (!ruolo.FlagCompetenza && !ruolo.FlagConoscenza)
                            {
                                if (!string.IsNullOrEmpty(ruolo.ragioneTrasmRapida))
                                {
                                    ragioneSmistamento = GetRagioneSelezionata(ruolo.ragioneTrasmRapida, listaRagSmista);
                                }
                                else
                                {
                                    ragioneSmistamento = GetRagioneSelezionata(uoAppartenenza.ragioneTrasmRapida, listaRagSmista);
                                    ruolo.ragioneTrasmRapida = uoAppartenenza.ragioneTrasmRapida;
                                }
                            }

                            esitoSmistamento = smistaDoc.SmistaDocumentoRuolo(infoUtente,
                                                                                mittente,
                                                                                documentoTrasmesso,
                                                                                datiDocumentoTrasmesso,
                                                                                ruolo,
                                                                                ragioneSmistamento, modelloNoNotify);

                            // se lo smistamento del documento è andato a buon fine...
                            if (esitoSmistamento.CodiceEsitoSmistamento.Equals(0))
                            {
                                accessRights = SetAccessRights(ragioneSmistamento);

                                DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti dbSmistaDoc = new DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti();

                                //DataSet ds = dbSmistaDoc.
                                bool eredita = dbSmistaDoc.IsRagioneEreditaByTrasmSingola(datiDocumentoTrasmesso.IDTrasmissioneSingola);
                                if (
                                    (ragioneSmistamento.tipo.Equals("W") && !IsEnabledVisibPosticipataInTrasmConWF()
                                    //||
                                    //  (!ragioneSmistamento.tipo.Equals("W") && ragioneSmistamento.eredita.Equals("1"))
                                    && ragioneSmistamento.eredita.Equals("1"))
                                    )
                                {
                                    // ...aggiunge la visibilità ai ruoli superiori gerarchici                                    
                                    smistaDoc.SetVisibilitaRuoliSup(mittente, datiDocumentoTrasmesso, ruolo, accessRights);
                                }

                                DocsPaDB.Query_DocsPAWS.Trasmissione trasmDb = new DocsPaDB.Query_DocsPAWS.Trasmissione();

                                string tipoTrasmSing = trasmDb.getTipoTrasmSing(datiDocumentoTrasmesso.IDTrasmissioneSingola);

                                //PACCHETTO ZANOTTI
                                if (tipoTrasmSing.Equals("R") && datiDocumentoTrasmesso.TrasmissioneConWorkflow && IsEnabledVisibPosticipataInTrasmConWF() && eredita)
                                {  
                                    DocsPaDB.Query_DocsPAWS.Utenti dbUser = new DocsPaDB.Query_DocsPAWS.Utenti();
                                    DocsPaVO.utente.Ruolo tempRuolo = dbUser.GetRuoloByIdGruppo(mittente.IDGroup);
                                    DocsPaVO.Smistamento.RuoloSmistamento newRuolo = new DocsPaVO.Smistamento.RuoloSmistamento();
                                    newRuolo.Codice = tempRuolo.codiceRubrica;
                                    newRuolo.ID = tempRuolo.systemId;
                                    newRuolo.Registri = tempRuolo.registri;
                                    DocsPaVO.Smistamento.MittenteSmistamento newMittente = dbSmistaDoc.getMittenteSmistamentoByIdTrasm(datiDocumentoTrasmesso.IDTrasmissione);
                                    smistaDoc.SetVisibilitaRuoliSup(newMittente, datiDocumentoTrasmesso, newRuolo, accessRights);
                                }

                                // Notifica al documentale dell'avvenuta impostazione di visibilità
                                // del documento a seguito dello smistamento
                                eventsNotification.SmistamentoDocumentoCompletatoEventHandler(mittente, documentoTrasmesso, ruolo, accessRights);

                                foreach (DocsPaVO.Smistamento.UtenteSmistamento utente in ruolo.Utenti)
                                {
                                    if (utente.FlagCompetenza || utente.FlagConoscenza)
                                    {
                                        // ...imposta le mail destinatari
                                        // FillDestinatariMail(destinatariMail, utente, ragCompetenza, ragConoscenza);
                                        //se la chiave non è abilitata popolo un hashtable differente contenente l'id degli utenti del ruolo e spedisco la mail subito
                                        //inserendo nel link l'id del ruolo. Altrimenti invio mail come prima: unico hashtable contenente le mail di TUTTI gli utenti con cui verrà
                                        //effettuato un invio massivo, senza l'info dell'id gruppo.
                                        if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_SMISTA_MAIL_SENZA_ID_RUOLO"))
                                            && DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_SMISTA_MAIL_SENZA_ID_RUOLO").Equals("1"))
                                        {
                                            FillDestinatariMail(destinatariMail, utente, ragioneSmistamento);
                                        }
                                        else
                                        {
                                            FillDestinatariMail(destinatariMailRuolo, utente, ragioneSmistamento);
                                        }
                                    }
                                    else
                                    {
                                        //se sto facendo una trasmissione da modello 
                                        if (!string.IsNullOrEmpty(ruolo.ragioneTrasmRapida) || !string.IsNullOrEmpty(uoAppartenenza.ragioneTrasmRapida))
                                        {
                                            //FillDestinatariMail(destinatariMail, utente, ragCompetenza, ragConoscenza);
                                            if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_SMISTA_MAIL_SENZA_ID_RUOLO"))
                                           && DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_SMISTA_MAIL_SENZA_ID_RUOLO").Equals("1"))
                                            {
                                                FillDestinatariMail(destinatariMail, utente, ragioneSmistamento);
                                            }
                                            else
                                            {
                                                FillDestinatariMail(destinatariMailRuolo, utente, ragioneSmistamento);
                                            }
                                        }
                                    }
                                }
                                //Nel caso di smistamento a ruolo: invio subito la notifica via mail agli utenti del ruolo per non perndere, nel link,
                                // l'informazione dell'id del ruolo
                                if (destinatariMailRuolo != null && destinatariMailRuolo.Count > 0)
                                {
                                    DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                                    DocsPaVO.utente.Ruolo role = utenti.getRuoloById(ruolo.ID);
                                    string idGruppo = string.Empty;
                                    if (role != null && !string.IsNullOrEmpty(role.idGruppo))
                                        idGruppo = role.idGruppo;
                                    string destinatari = GetDestinatariMail(destinatariMailRuolo, ragioneSmistamento.descrizione);
                                    if (!destinatari.Equals(string.Empty))
                                    {
                                        logger.Debug("Invio email per " + ragioneSmistamento.descrizione + " a: " + destinatari);
                                        SendMail(mittente, infoUtente, destinatari, ragioneSmistamento.descrizione, documentoTrasmesso, false, ragioneSmistamento.testoMsgNotificaDoc, true, httpFullPath, datiDocumentoTrasmesso, idGruppo);
                                    }

                                    destinatari = GetDestinatariMail(destinatariMailRuolo, ragioneSmistamento.descrizione + "_CON_ALLEGATI");
                                    if (!destinatari.Equals(string.Empty))
                                    {
                                        logger.Debug("Invio email per " + ragioneSmistamento.descrizione + " con ALLEGATI a: " + destinatari);
                                        SendMail(mittente, infoUtente, destinatari, ragioneSmistamento.descrizione, documentoTrasmesso, true, ragioneSmistamento.testoMsgNotificaDoc, true, httpFullPath, datiDocumentoTrasmesso, idGruppo);
                                    }

                                    destinatari = GetDestinatariMail(destinatariMailRuolo, ragioneSmistamento.descrizione + "_SOLO_ALLEGATI");
                                    if (!destinatari.Equals(string.Empty))
                                    {
                                        logger.Debug("Invio email per " + ragioneSmistamento.descrizione + " ma solo ALLEGATI a: " + destinatari);
                                        SendMail(mittente, infoUtente, destinatari, ragioneSmistamento.descrizione, documentoTrasmesso, true, ragioneSmistamento.testoMsgNotificaDoc, false, httpFullPath, datiDocumentoTrasmesso, idGruppo);
                                    }
                                }
                            }
                            destinatariMailRuolo.Clear();
                            retValue.Add(esitoSmistamento);
                            esitoSmistamento = null;
                        }
                        else
                        {
                            esitoSmistamento = new DocsPaVO.Smistamento.EsitoSmistamentoDocumento();
                            esitoSmistamento.CodiceEsitoSmistamento = 99;
                            esitoSmistamento.DenominazioneDestinatario = ruolo.Descrizione;
                            esitoSmistamento.DescrizioneEsitoSmistamento = "impossibile trasmettere su registro differente";
                            retValue.Add(esitoSmistamento);
                            esitoSmistamento = null;
                        }
                    }
                    else
                    {
                        // cerca tra gli utenti del ruolo...
                        foreach (DocsPaVO.Smistamento.UtenteSmistamento utente in ruolo.Utenti)
                        {
                            if (utente.FlagCompetenza || utente.FlagConoscenza || !string.IsNullOrEmpty(utente.ragioneTrasmRapida))
                            {
                                if (utente.FlagCompetenza)
                                    ragioneSmistamento = ragCompetenza;
                                else
                                    ragioneSmistamento = ragConoscenza;

                                if (!string.IsNullOrEmpty(utente.ragioneTrasmRapida))
                                    ragioneSmistamento = GetRagioneSelezionata(utente.ragioneTrasmRapida, listaRagSmista);

                                logger.Debug("Smistamento a Utente: " + utente.EMail + " del Ruolo: " + ruolo.Descrizione + " in UO di appartenenza");
                                esitoSmistamento = smistaDoc.SmistaDocumento(infoUtente,
                                                                            mittente,
                                                                            documentoTrasmesso,
                                                                            datiDocumentoTrasmesso,
                                                                            utente,
                                                                            ragioneSmistamento,
                                                                            false);

                                // se lo smistamento del documento è andato a buon fine...
                                if (esitoSmistamento.CodiceEsitoSmistamento.Equals(0))
                                {
                                    accessRights = SetAccessRights(ragioneSmistamento);

                                    if (
                                        (ragioneSmistamento.tipo.Equals("W") && !IsEnabledVisibPosticipataInTrasmConWF()) ||
                                        (!ragioneSmistamento.tipo.Equals("W") && ragioneSmistamento.eredita.Equals("1")) )
                                    {
                                        // ...aggiunge la visibilità ai ruoli superiori gerarchici 
                                        if(!string.IsNullOrEmpty(ruolo.ID)) //nelle trasmissioni a singolo UTENTE, il ruolo.ID è null
                                            // perchè non c'è alcun ruolo, ma c'è solo un utente. E' giusto così,  infatti,  quando la trasmissione è a utente, non deve essere calcolata alcuna gerarchia dei superiori.
                                        smistaDoc.SetVisibilitaRuoliSup(mittente, datiDocumentoTrasmesso, ruolo, accessRights);
                                    }
                                    DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti dbSmistaDoc = new DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti();
                                    DocsPaDB.Query_DocsPAWS.Trasmissione trasmDb = new DocsPaDB.Query_DocsPAWS.Trasmissione();
                                    bool eredita = dbSmistaDoc.IsRagioneEreditaByTrasmSingola(datiDocumentoTrasmesso.IDTrasmissioneSingola);
                                    string tipoTrasmSing = trasmDb.getTipoTrasmSing(datiDocumentoTrasmesso.IDTrasmissioneSingola);

                                    //PACCHETTO ZANOTTI
                                    if (tipoTrasmSing.Equals("R") && datiDocumentoTrasmesso.TrasmissioneConWorkflow && IsEnabledVisibPosticipataInTrasmConWF() && eredita)
                                    {
                                        DocsPaDB.Query_DocsPAWS.Utenti dbUser = new DocsPaDB.Query_DocsPAWS.Utenti();
                                        DocsPaVO.utente.Ruolo tempRuolo = dbUser.GetRuoloByIdGruppo(mittente.IDGroup);
                                        DocsPaVO.Smistamento.RuoloSmistamento newRuolo = new DocsPaVO.Smistamento.RuoloSmistamento();
                                        newRuolo.Codice = tempRuolo.codiceRubrica;
                                        newRuolo.ID = tempRuolo.systemId;
                                        newRuolo.Registri = tempRuolo.registri;
                                        DocsPaVO.Smistamento.MittenteSmistamento newMittente = dbSmistaDoc.getMittenteSmistamentoByIdTrasm(datiDocumentoTrasmesso.IDTrasmissione);
                                        smistaDoc.SetVisibilitaRuoliSup(newMittente, datiDocumentoTrasmesso, newRuolo, accessRights);
                                    }

                                    // Notifica al documentale dell'avvenuta impostazione di visibilità
                                    // del documento a seguito dello smistamento
                                    eventsNotification.SmistamentoDocumentoCompletatoEventHandler(mittente, documentoTrasmesso, ruolo, accessRights);

                                    // ...imposta le mail destinatari
                                    // FillDestinatariMail(destinatariMail, utente, ragCompetenza, ragConoscenza);
                                    FillDestinatariMail(destinatariMail, utente, ragioneSmistamento);
                                }

                                retValue.Add(esitoSmistamento);
                                esitoSmistamento = null;
                            }
                        }
                    }
                }
            }
            
            foreach (DocsPaVO.trasmissione.RagioneTrasmissione ragione in listaRagSmista)
            {
                string destinatari = GetDestinatariMail(destinatariMail, ragione.descrizione);
                if (!destinatari.Equals(string.Empty))
                {
                    logger.Debug("Invio email per " + ragione.descrizione + " a: " + destinatari);
                    SendMail(mittente, infoUtente, destinatari, ragione.descrizione, documentoTrasmesso, false, ragione.testoMsgNotificaDoc, true, httpFullPath, datiDocumentoTrasmesso);
                }

                // invio email per competenza con allegato ai destinatari
                destinatari = GetDestinatariMail(destinatariMail, ragione.descrizione + "_CON_ALLEGATI");
                if (!destinatari.Equals(string.Empty))
                {
                    logger.Debug("Invio email per " + ragione.descrizione + " con ALLEGATI a: " + destinatari);
                    SendMail(mittente, infoUtente, destinatari, ragione.descrizione, documentoTrasmesso, true, ragione.testoMsgNotificaDoc, true, httpFullPath, datiDocumentoTrasmesso);
                }

                destinatari = GetDestinatariMail(destinatariMail, ragione.descrizione + "_SOLO_ALLEGATI");
                if (!destinatari.Equals(string.Empty))
                {
                    logger.Debug("Invio email per " + ragione.descrizione + " ma solo ALLEGATI a: " + destinatari);
                    SendMail(mittente, infoUtente, destinatari, ragione.descrizione, documentoTrasmesso, true, ragione.testoMsgNotificaDoc, false, httpFullPath, datiDocumentoTrasmesso);
                }

            }
            destinatariMail.Clear();
            destinatariMail = null;

            if (uoAppartenenza.UoInferiori != null && uoAppartenenza.UoInferiori.Count > 0)
            {
                foreach (DocsPaVO.Smistamento.UOSmistamento uoInf in uoAppartenenza.UoInferiori)
                {
                    retValue = SmistaDocumentoRic(mittente,
                                                infoUtente,
                                                documentoTrasmesso,
                                                datiDocumentoTrasmesso,
                                                uoInf,
                                                uoInferiori,
                                                httpFullPath, retValue);
                }
            }

            if (uoAppartenenza.UoSmistaTrasAutomatica != null && uoAppartenenza.UoSmistaTrasAutomatica.Count > 0)
            {
                foreach (DocsPaVO.Smistamento.UOSmistamento uoTrasmAutomatica in uoAppartenenza.UoSmistaTrasAutomatica)
                {
                    # region andrea commento
                    //Andrea
                      //if(uoTrasmAutomatica.Ruoli!=null && uoTrasmAutomatica.Ruoli.Count>0)
                      //{
                      //    foreach (DocsPaVO.Smistamento.RuoloSmistamento ruolo in uoTrasmAutomatica.Ruoli)
                      //    {
                      //        if (ruolo != null && ruolo.Utenti.Count>0)
                      //        {
                      //            foreach (DocsPaVO.Smistamento.UtenteSmistamento utente in ruolo.Utenti) 
                      //            {
                      //                if (utente != null)
                      //                {
                      //                    retValue = SmistaDocumentoRic(mittente,
                      //                                  infoUtente,
                      //                                  documentoTrasmesso,
                      //                                  datiDocumentoTrasmesso,
                      //                                  uoTrasmAutomatica,
                      //                                  null,
                      //                                  httpFullPath, retValue);
                      //                }
                      //            }
                      //        }
                      //        else
                      //        {
                                  
                      //            esitoSmistamento = new DocsPaVO.Smistamento.EsitoSmistamentoDocumento();
                      //            esitoSmistamento.CodiceEsitoSmistamento = 999;
                      //            esitoSmistamento.DescrizioneEsitoSmistamento = "Trasmissione: "
                      //                                                           + uoTrasmAutomatica.Descrizione.ToString()
                      //                                                           + " per il destinatario: "
                      //                                                           + ruolo.Descrizione.ToString()
                      //                                                           + " non è andata a buon fine";

                      //            retValue.Add(esitoSmistamento);
                      //            esitoSmistamento = null;
                                  
                      //        } 
                      //     }
                      //  }
                    //  End Andrea
                    # endregion 
                    uoTrasmAutomatica.modelloNoNotify = uoAppartenenza.modelloNoNotify;

                    //Prima della Modifica
                    retValue = SmistaDocumentoRic(mittente,
                                                infoUtente,
                                                documentoTrasmesso,
                                                datiDocumentoTrasmesso,
                                                uoTrasmAutomatica,
                                                null,
                                                httpFullPath, retValue);
                }
            }

            return retValue;
        }

        /// <summary>
        /// Rifiuta il documento ricevuto
        /// </summary>
        /// <param name="notaRifiuto"></param>
        /// <param name="IDTrasmUtente"></param>
        /// <param name="idTrasmissione"></param>
        /// <param name="idPeople"></param>
        /// <param name="ruolo"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static bool RifiutaDocumento(string notaRifiuto, string IDTrasmUtente, string idTrasmissione, string idPeople, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool rtn = true;
            string mode;
            string idObj;
            string msg;
            const string ACCETTATADOC = "ACCETTATA_DOCUMENTO";
            const string ACCETTATAFASC = "ACCETTATA_FASCICOLO";
            const string RIFIUTATADOC = "RIFIUTATA_DOCUMENTO";
            const string RIFIUTATAFASC = "RIFIUTATA_FASCICOLO";

            string errore = string.Empty;
            DocsPaVO.trasmissione.TrasmissioneUtente objTrasmutente = null;

            try
            {
                // Verifica lo stato del documento, ovvero che non sia già stato accettato / rifiutato
                DocsPaVO.trasmissione.StatoTrasmissioneUtente statoTrasmissione = Trasmissioni.QueryTrasmManager.getStatoTrasmissioneUtente(infoUtente, IDTrasmUtente);

                if (!statoTrasmissione.Accettata && !statoTrasmissione.Rifiutata)
                {
                    objTrasmutente = new DocsPaVO.trasmissione.TrasmissioneUtente();
                    objTrasmutente.noteRifiuto = notaRifiuto;
                    objTrasmutente.systemId = IDTrasmUtente;
                    objTrasmutente.tipoRisposta = DocsPaVO.trasmissione.TipoRisposta.RIFIUTO;
                    objTrasmutente.utente = Utenti.UserManager.getUtente(idPeople);

                    rtn = Trasmissioni.ExecTrasmManager.executeAccRifMethod(objTrasmutente, idTrasmissione, ruolo, infoUtente, out errore, out mode, out idObj);
                    if (rtn && !string.IsNullOrEmpty(mode))
                    {
                        DocsPaVO.trasmissione.TrasmissioneSingola sing = BusinessLogic.Trasmissioni.ExecTrasmManager.getTrasmSingola(objTrasmutente);
                        switch (mode)
                        {
                            case ACCETTATADOC:
                                msg = "Accettazione della trasmissione. Id documento: " + idObj;
                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, ruolo.idGruppo, infoUtente.idAmministrazione, "ACCEPTTRASMDOCUMENT", idObj, msg, DocsPaVO.Logger.CodAzione.Esito.OK, null, "1", sing.systemId);
                                break;
                            case ACCETTATAFASC:
                                msg = "Accettazione della trasmissione. Id fascicolo: " + idObj;
                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, ruolo.idGruppo, infoUtente.idAmministrazione, "ACCEPTTRASMFOLDER", idObj, msg, DocsPaVO.Logger.CodAzione.Esito.OK, null, "1", sing.systemId);
                                break;
                            case RIFIUTATAFASC:
                                msg = "Rifiuto della trasmissione. Id fascicolo: " + idObj;
                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, ruolo.idGruppo, infoUtente.idAmministrazione, "REJECTTRASMFOLDER", idObj, msg, DocsPaVO.Logger.CodAzione.Esito.OK, null, "1", sing.systemId);
                                break;
                            case RIFIUTATADOC:
                                msg = "Rifiuto della trasmissione. Id documento: " + idObj;
                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, ruolo.idGruppo, infoUtente.idAmministrazione, "REJECTTRASMDOCUMENT", idObj, msg, DocsPaVO.Logger.CodAzione.Esito.OK, null, "1", sing.systemId);
                                break;
                        }
                    }
                }
                else
                {
                    rtn = false;
                    logger.Debug(string.Format("RifiutaDocumentoSmistamento: Il documento non può essere rifiutato in quanto la trasmissione utente risulta in stato '{0}'", (statoTrasmissione.Accettata ? "Accettata" : "Rifiutata")));
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in RifiutaDocumento: ", ex);
                rtn = false;
            }
            return rtn;
        }

        public static bool ScartaDocumento(DocsPaVO.utente.InfoUtente infoUtente, string idOggetto, string tipoOggetto, string idTrasmissione, string idTrasmSingola, DocsPaVO.Smistamento.MittenteSmistamento mittente, DocsPaVO.Smistamento.RuoloSmistamento ruolo)
        {
            bool esito = false;

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                string idRagione = string.Empty;
                string accessRights = string.Empty;
                DocsPaVO.trasmissione.RagioneTrasmissione ragione = null;
                DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti dbSmistaDoc = new DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti();
                esito = dbSmistaDoc.ScartaDocumento(infoUtente, idOggetto, tipoOggetto, idTrasmissione);
                DocsPaVO.Smistamento.DocumentoSmistamento docsmistato = BusinessLogic.SmistamentoDocumenti.SmistamentoManager.GetDocumentoSmistamento(idOggetto, infoUtente, false);

                if (esito)
                {

                    //calcolo ragione trasmissione ricevuta su cui ho fatto visto.

                    string q = "SELECT ID_RAGIONE FROM DPA_TRASM_SINGOLA WHERE SYSTEM_ID=" + idTrasmSingola;
                    using (DocsPaDB.DBProvider db = new DocsPaDB.DBProvider())
                    {
                        using (IDataReader dr = db.ExecuteReader(q))
                        {
                            if (dr.Read())
                            {
                                idRagione = dr.GetValue(0).ToString();
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(idRagione))
                    {
                        ragione = BusinessLogic.Trasmissioni.RagioniManager.getRagione(idRagione);

                        DocsPaDB.Query_DocsPAWS.Trasmissione trasmDb = new DocsPaDB.Query_DocsPAWS.Trasmissione();

                        string tipoTrasmSing = trasmDb.getTipoTrasmSing(idTrasmSingola);

                        if (tipoTrasmSing.Equals("R") && ragione != null && ((ragione.tipo.Equals("W") && IsEnabledVisibPosticipataInTrasmConWF()) ||
                            (!ragione.tipo.Equals("W") && ragione.eredita.Equals("1")))
                            )
                        {
                            //calcolo accessRights
                            accessRights = dbSmistaDoc.GetAccessRightSmistamentoOriginali(ragione).ToString();
                            // aggiunge la visibilià ai ruoli superiori gerarchici

                            esito = dbSmistaDoc.SetVisibilitaRuoliSup(mittente, docsmistato, ruolo, accessRights);
                            //}

                            //// Notifica al documentale dell'avvenuta impostazione di visibilità
                            //// del documento a seguito dello smistamento
                            DocsPaDocumentale.Interfaces.IAclEventListener eventsNotification = new DocsPaDocumentale.Documentale.AclEventListener(infoUtente);
                            eventsNotification.SmistamentoDocumentoCompletatoEventHandler(mittente, docsmistato, ruolo, accessRights);
                        }
                    }
                }

                if (esito)
                    transactionContext.Complete();
            }

            return esito;
        }

        /// <summary>
        /// Verifica se, nella gerarchia delle UO destinatarie dello smistamento, almeno un destinatario è stato selezionato
        /// </summary>
        /// <param name="uo"></param>
        /// <returns></returns>
        private static bool AlmostOneCheckedInUO(params DocsPaVO.Smistamento.UOSmistamento[] uo)
        {
            bool almostOne = false;

            foreach (DocsPaVO.Smistamento.UOSmistamento item in uo)
            {
                if (almostOne)
                    break;

                if (item.FlagCompetenza || item.FlagConoscenza)
                {
                    almostOne = true;
                    break;
                }
                else
                {
                    // Verifica se almeno un elemento nei ruoli dell'UO è selezionato
                    almostOne = AlmostOneCheckedInRuoli((DocsPaVO.Smistamento.RuoloSmistamento[])item.Ruoli.ToArray(typeof(DocsPaVO.Smistamento.RuoloSmistamento)));

                    if (!almostOne)
                    {
                        foreach (DocsPaVO.Smistamento.UOSmistamento uoInferiore in (DocsPaVO.Smistamento.UOSmistamento[])item.UoInferiori.ToArray(typeof(DocsPaVO.Smistamento.UOSmistamento)))
                        {
                            almostOne = AlmostOneCheckedInUO(uoInferiore);

                            if (almostOne)
                                break;
                        }
                    }
                    else
                        break;
                }

                if (!almostOne &&
                    item.UoSmistaTrasAutomatica != null && item.UoSmistaTrasAutomatica.Count > 0)
                {
                    // Se ancora alcun elemento risulta selezionato per lo smistamento,
                    // verifica lo stato di selezione delle uo selezionate per la trasmissione automatica
                    almostOne = true;
                    break;
                }
            }

            return almostOne;
        }

        /// <summary>
        /// Verifica che almeno un ruolo o utenti in esso contenuti siano stati selezionati per lo smistamento
        /// </summary>
        /// <param name="ruoli"></param>
        /// <returns></returns>
        private static bool AlmostOneCheckedInRuoli(params DocsPaVO.Smistamento.RuoloSmistamento[] ruoli)
        {
            bool almostOne = false;

            foreach (DocsPaVO.Smistamento.RuoloSmistamento ruolo in ruoli)
            {
                if (ruolo.FlagCompetenza || ruolo.FlagConoscenza)
                {
                    almostOne = true;
                    break;
                }
                else
                {
                    // Ricerca almeno un utente del ruolo selezionato per lo smistamento
                    almostOne = AlmostOneUtenteChecked((DocsPaVO.Smistamento.UtenteSmistamento[]) ruolo.Utenti.ToArray(typeof(DocsPaVO.Smistamento.UtenteSmistamento)));

                    if (almostOne)
                        break;
                }
            }

            return almostOne;
        }

        /// <summary>
        /// Verifica che almeno un utente sia selezionato per lo smistamento
        /// </summary>
        /// <param name="utenti"></param>
        /// <returns></returns>
        private static bool AlmostOneUtenteChecked(params DocsPaVO.Smistamento.UtenteSmistamento[] utenti)
        {
            int countComp = utenti.Count(e => e.FlagCompetenza == true);
            int countCC = utenti.Count(e => e.FlagConoscenza == true);
            
            return (countComp > 0 || countCC > 0);
        }

		/// <summary>
		/// Invio del documento alle uo destinatarie
		/// </summary>
		/// <param name="mittente"></param>
		/// <param name="infoUtente"></param>
		/// <param name="documentoTrasmesso">dati del documento da smistare</param>
		/// <param name="uoDestinatarie"></param>
		/// <returns></returns>
		public static ArrayList SmistaDocumento(
					DocsPaVO.Smistamento.MittenteSmistamento mittente,
					DocsPaVO.utente.InfoUtente infoUtente,
					DocsPaVO.Smistamento.DocumentoSmistamento documentoSmistamento,
					DocsPaVO.Smistamento.UOSmistamento[] uoDestinatarie,
                    string httpFullPath)
		{
            ArrayList retValue = new ArrayList();

            // Verifica che, tra le UO destinatarie, sia selezionato almeno un destinatario per lo smistamento del documento
            if (!AlmostOneCheckedInUO(uoDestinatarie))
            {

                retValue.Add(
                    new DocsPaVO.Smistamento.EsitoSmistamentoDocumento
                    {
                        CodiceEsitoSmistamento = -1,
                        DescrizioneEsitoSmistamento = "Nessun destinatario selezionato per lo smistamento del documento"
                    });
            }
            else
            {
                DocsPaVO.trasmissione.RagioneTrasmissione ragioneSmistamento;
                DocsPaVO.trasmissione.RagioneTrasmissione ragCompetenza;
                DocsPaVO.trasmissione.RagioneTrasmissione ragConoscenza;

                Hashtable destinatariMail = new Hashtable();
                DocsPaVO.Smistamento.EsitoSmistamentoDocumento esitoSmistamento = null;
                DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti smistaDoc = new DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti();

                DocsPaDocumentale.Interfaces.IAclEventListener eventsNotification = new DocsPaDocumentale.Documentale.AclEventListener(infoUtente);


                //DocsPaVO.trasmissione.RagioneTrasmissione ragCompetenza = Trasmissioni.RagioniManager.getRagioneByCodice(infoUtente.idAmministrazione, "COMPETENZA");
                //DocsPaVO.trasmissione.RagioneTrasmissione ragConoscenza = Trasmissioni.RagioniManager.getRagioneByCodice(infoUtente.idAmministrazione, "CONOSCENZA");

                ArrayList listaRagSmista;
                listaRagSmista = GetRagioniTrasmissioneSmistamento(infoUtente.idAmministrazione);
                if (listaRagSmista != null && listaRagSmista.Count.Equals(2))
                {
                    ragCompetenza = (DocsPaVO.trasmissione.RagioneTrasmissione)listaRagSmista[0];
                    ragConoscenza = (DocsPaVO.trasmissione.RagioneTrasmissione)listaRagSmista[1];
                }
                else
                {
                    esitoSmistamento = new DocsPaVO.Smistamento.EsitoSmistamentoDocumento();
                    esitoSmistamento.CodiceEsitoSmistamento = 199;
                    esitoSmistamento.DescrizioneEsitoSmistamento = "ragioni di trasmissioni per lo smistamento non impostate correttamente";
                    retValue.Add(esitoSmistamento);
                    esitoSmistamento = null;
                    return retValue;
                }

                foreach (DocsPaVO.Smistamento.UOSmistamento uo in uoDestinatarie)
                {
                    foreach (DocsPaVO.Smistamento.RuoloSmistamento ruolo in uo.Ruoli)
                    {
                        // Smistamento del documento solamente ai ruoli di riferimento
                        if (ruolo.RuoloRiferimento)
                        {
                            if (ruolo.FlagCompetenza || ruolo.FlagConoscenza)
                            {
                                logger.Debug("Smistamento a Ruolo di riferimento: " + ruolo.Descrizione + " della UO: " + uo.Descrizione);

                                if (ruolo.FlagCompetenza)
                                    ragioneSmistamento = ragCompetenza;
                                else
                                    ragioneSmistamento = ragConoscenza;

                                // Smistamento del documento al ruolo di riferimento
                                esitoSmistamento = smistaDoc.SmistaDocumentoRuolo(mittente, documentoSmistamento, ruolo, ragioneSmistamento);

                                // Se lo smistamento del documento è andato a buon fine
                                if (esitoSmistamento.CodiceEsitoSmistamento.Equals(0))
                                {
                                    string accessRights = SetAccessRights(ragioneSmistamento);

                                    if (
                                        (ragioneSmistamento.tipo.Equals("W") && !IsEnabledVisibPosticipataInTrasmConWF()) ||
                                        (!ragioneSmistamento.tipo.Equals("W") && ragioneSmistamento.eredita.Equals("1"))
                                        )
                                    {
                                        // aggiunge la visibilità ai ruoli superiori gerarchici
                                        smistaDoc.SetVisibilitaRuoliSup(mittente, documentoSmistamento, ruolo, accessRights);
                                    }

                                    // Notifica al documentale dell'avvenuta impostazione di visibilità
                                    // del documento a seguito dello smistamento
                                    eventsNotification.SmistamentoDocumentoCompletatoEventHandler(mittente, documentoSmistamento, ruolo, accessRights);

                                    foreach (DocsPaVO.Smistamento.UtenteSmistamento utente in ruolo.Utenti)
                                    {
                                        if (utente.FlagCompetenza || utente.FlagConoscenza)
                                        {
                                            // impostazione delle mail destinatari
                                            FillDestinatariMail(destinatariMail, utente, ragCompetenza, ragConoscenza);
                                        }
                                    }
                                }
                                retValue.Add(esitoSmistamento);
                                esitoSmistamento = null;
                            }
                        }
                    }
                }

                // Invio mail ai destinatari del documento
                string destinatari = GetDestinatariMail(destinatariMail, "COMPETENZA");
                if (!destinatari.Equals(string.Empty))
                {
                    logger.Debug("Invio email per COMPETENZA a: " + destinatari);
                    SendMail(mittente, infoUtente, destinatari, "COMPETENZA", documentoSmistamento, false, ragCompetenza.testoMsgNotificaDoc, true, httpFullPath, null);
                }

                // invio email per competenza con allegato ai destinatari
                destinatari = GetDestinatariMail(destinatariMail, "COMPETENZA_CON_ALLEGATI");
                if (!destinatari.Equals(string.Empty))
                {
                    logger.Debug("Invio email per COMPETENZA con ALLEGATI a: " + destinatari);
                    SendMail(mittente, infoUtente, destinatari, "COMPETENZA", documentoSmistamento, true, ragCompetenza.testoMsgNotificaDoc, true, httpFullPath, null);
                }

                destinatari = GetDestinatariMail(destinatariMail, "COMPETENZA_SOLO_ALLEGATI");
                if (!destinatari.Equals(string.Empty))
                {
                    logger.Debug("Invio email per COMPETENZA ma solo ALLEGATI a: " + destinatari);
                    SendMail(mittente, infoUtente, destinatari, "COMPETENZA", documentoSmistamento, true, ragCompetenza.testoMsgNotificaDoc, false, httpFullPath, null);
                }

                // invio email per conoscenza ai destinatari
                destinatari = GetDestinatariMail(destinatariMail, "CC");
                if (!destinatari.Equals(string.Empty))
                {
                    logger.Debug("Invio email per CONOSCENZA a: " + destinatari);
                    SendMail(mittente, infoUtente, destinatari, "CONOSCENZA", documentoSmistamento, false, ragConoscenza.testoMsgNotificaDoc, true, httpFullPath, null);
                }

                // invio email per conoscenza ai destinatari con allegato
                destinatari = GetDestinatariMail(destinatariMail, "CC_CON_ALLEGATI");
                if (!destinatari.Equals(string.Empty))
                {
                    logger.Debug("Invio email per CONOSCENZA con ALLEGATI a: " + destinatari);
                    SendMail(mittente, infoUtente, destinatari, "CONOSCENZA", documentoSmistamento, true, ragConoscenza.testoMsgNotificaDoc, true, httpFullPath, null);
                }

                //SOLO allegati per conoscenza
                destinatari = GetDestinatariMail(destinatariMail, "CC_SOLO_ALLEGATI");
                if (!destinatari.Equals(string.Empty))
                {
                    logger.Debug("Invio email per CONOSCENZA solo ALLEGATI a: " + destinatari);
                    SendMail(mittente, infoUtente, destinatari, "CONOSCENZA", documentoSmistamento, true, ragConoscenza.testoMsgNotificaDoc, false, httpFullPath, null);
                }

                destinatariMail.Clear();
                destinatariMail = null;
            }

			return retValue;
		}

        public static bool ExistUOInf(string idUO, DocsPaVO.Smistamento.MittenteSmistamento mittente)
        {
            DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti dbSmistaDoc=new DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti();
            return dbSmistaDoc.ExistUOInf(idUO, mittente.RegistriAppartenenza);
        }

		#endregion

		#region PRIVATE

		private static string SetAccessRights(DocsPaVO.trasmissione.RagioneTrasmissione ragione)
		{
			string retValue = string.Empty;

            DocsPaVO.HMDiritti.HMdiritti HMD = new DocsPaVO.HMDiritti.HMdiritti(); 

            switch (ragione.tipoDiritti)
            {
                case DocsPaVO.trasmissione.TipoDiritto.READ:
                    retValue = HMD.HMdiritti_Read.ToString();
                    break;
                case DocsPaVO.trasmissione.TipoDiritto.WRITE:
                    retValue = HMD.HMdiritti_Write.ToString();
                    break;
                default:
                    retValue = HMD.HMdiritti_Write.ToString();
                    break;
            }

			return retValue;
		}

        private static string SetAccessRights(bool FlagComp, bool FlagCC)
        {
            string retValue = string.Empty;

            if (FlagComp) retValue = "63";
            if (FlagCC) retValue = "45";
            
            return retValue;
        }

		private static DocsPaVO.Smistamento.DocumentoSmistamento CreateDocumentoTrasmesso(DataRow rowDocumentoTrasmesso,DataTable tableMittDest)
		{
			DocsPaVO.Smistamento.DocumentoSmistamento retValue=new DocsPaVO.Smistamento.DocumentoSmistamento();
			
			retValue.IDDocumento=rowDocumentoTrasmesso["IDDocumento"].ToString();
			retValue.TipoDocumento=rowDocumentoTrasmesso["TipoDocumento"].ToString();
			retValue.Oggetto=rowDocumentoTrasmesso["Oggetto"].ToString();
			retValue.Segnatura=rowDocumentoTrasmesso["Segnatura"].ToString();
            retValue.IDRegistro = rowDocumentoTrasmesso["IDRegistro"].ToString();
			retValue.DataCreazione=rowDocumentoTrasmesso["DataCreazione"].ToString();
			retValue.Versioni=rowDocumentoTrasmesso["Versioni"].ToString();
			retValue.Allegati=rowDocumentoTrasmesso["Allegati"].ToString();
            retValue.DocNumber = rowDocumentoTrasmesso["DocNumber"].ToString();
            retValue.TipologyDescription = ProfilazioneDocumenti.GetTipologyDescriptionByIdProfile(retValue.IDDocumento);

			// se documento protocollato, reperimento del
			// mittente e del / dei destinatari
			if (tableMittDest!=null)
			{
				DataRow[] rows=tableMittDest.Select("TIPO = 'M'");

				if (rows.Length==1)
					retValue.MittenteDocumento=rows[0]["VAR_DESC_CORR"].ToString();

				rows=null;

				rows=tableMittDest.Select("TIPO = 'D'");

				foreach (DataRow rowDestinatario in rows)
					retValue.DestinatariDocumento.Add(rowDestinatario["VAR_DESC_CORR"].ToString());

				rows=null;
			}
			return retValue;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataRowFileDocument"></param>
        /// <param name="infoUtente"></param>
        /// <param name="content"></param>
        /// <param name="converPdfInline"></param>
        /// <param name="isConverted"></param>
        /// <returns></returns>
        private static DocsPaVO.documento.FileDocumento GetFileDocument(DataRow dataRowFileDocument, DocsPaVO.utente.InfoUtente infoUtente, bool content, bool converPdfInline, out bool isConverted)
        {
            isConverted = false;            
            DocsPaVO.documento.FileDocumento retValue = null;

            // Se il file è stato acquisito (esistenza del path)
            if (dataRowFileDocument["PATH"] != DBNull.Value)
            {
                DocsPaVO.documento.FileRequest fileRequest = new DocsPaVO.documento.FileRequest();

                fileRequest.docNumber = dataRowFileDocument["DOCNUMBER"].ToString();
                fileRequest.versionId = dataRowFileDocument["VERSION_ID"].ToString();
                fileRequest.fileName = dataRowFileDocument["PATH"].ToString();
                fileRequest.docServerLoc = System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"];
                fileRequest.firmato = dataRowFileDocument.Table.Columns.Contains("CHA_FIRMATO") && dataRowFileDocument["CHA_FIRMATO"] != null ? dataRowFileDocument["CHA_FIRMATO"].ToString() : "0";
                try
                {
                    if (content)
                        retValue = Documenti.FileManager.getFile(fileRequest, infoUtente, true, converPdfInline, out isConverted);
                    else
                        retValue = Documenti.FileManager.getInfoFile(fileRequest, infoUtente);
                    if (fileRequest.firmato.Equals("1") && retValue != null)
                        retValue.signatureResult = new DocsPaVO.documento.VerifySignatureResult();
                }
                catch
                {
                }
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataRowFileDocument"></param>
        /// <param name="infoUtente"></param>
        /// <param name="content"></param>
        /// <returns></returns>
		private static DocsPaVO.documento.FileDocumento GetFileDocument(DataRow dataRowFileDocument,DocsPaVO.utente.InfoUtente infoUtente,bool content)
		{
            bool isConverted;
            return GetFileDocument(dataRowFileDocument, infoUtente, content, false, out isConverted);
		}

		private static string GetFileExtension(string fileName)
		{
			System.IO.FileInfo fileInfo=new System.IO.FileInfo(fileName);
			return fileInfo.Extension;
		}

        // CH inserito questo  metodo per consentire l'invio della mail di notifica con allegati
        /// <summary>
        /// Estrae il Documento Principale
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="schedaDoc"></param>
        /// <param name="path"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        private static bool estrazioneDocPrincipale(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDoc, string path,out Dictionary<string, string> CoppiaNomeFileENomeOriginale)
        {
            System.IO.FileStream fs = null;
            System.IO.FileStream fsAll = null;
            CoppiaNomeFileENomeOriginale = new Dictionary<string, string>();
            try
            {
                //estrazione documento principale
                DocsPaVO.documento.Documento doc = (DocsPaVO.documento.Documento)schedaDoc.documenti[0];

                //modifica bug estensione file
                char[] dot = { '.' };
                string[] parts = doc.fileName.Split(dot);
                string suffix = parts[parts.Length - 1];
                string docPrincipaleName = "Documento_principale." + suffix;
                //fine modifica

                //string docPrincipaleName="Documento_principale."+doc.fileName.Substring(doc.fileName.IndexOf(".")+1);
                fs = new System.IO.FileStream(path + "\\" + docPrincipaleName, System.IO.FileMode.Create);
                string library = DocsPaDB.Utils.Personalization.getInstance(infoUtente.idAmministrazione).getLibrary();

                //byte[] content=DocsPaWS.interoperabilità.InteroperabilitaInvioSegnatura.getDocument(infoUtente,doc.docNumber,doc.version,doc.versionId,doc.versionLabel,logger);
                DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);
                byte[] content = documentManager.GetFile(doc.docNumber, doc.version, doc.versionId, doc.versionLabel);
                
                if (content == null)
                {
                    logger.Debug("Errore nella gestione delle trasmissioni (estrazioneDocPrincipale| principale)");
                    throw new Exception();
                }
                
         
                string NomeOriginale=BusinessLogic.Documenti.FileManager.getOriginalFileName(infoUtente,doc);

                if (String.IsNullOrEmpty(NomeOriginale))
                    NomeOriginale = docPrincipaleName;

                CoppiaNomeFileENomeOriginale.Add(String.Format(path + "\\" + docPrincipaleName).ToLowerInvariant(), NomeOriginale);

                fs.Write(content, 0, content.Length);
                fs.Close();
                fs = null;

                // estrazione degli allegati
                foreach (DocsPaVO.documento.Allegato allegato in schedaDoc.allegati)
                {
                    fs = new System.IO.FileStream(path + @"\" + Path.GetFileName(allegato.fileName), System.IO.FileMode.Create);

                    content = documentManager.GetFile(allegato.docNumber, allegato.version, allegato.versionId, allegato.versionLabel);

                    if (content == null)
                    {
                        logger.Debug("Errore nella gestione delle trasmissioni (estrazioneDocPrincipale| allegati)");
                        throw new Exception();
                    }

                    NomeOriginale = BusinessLogic.Documenti.FileManager.getOriginalFileName(infoUtente, allegato);

                    if (String.IsNullOrEmpty(NomeOriginale))
                        NomeOriginale = Path.GetFileName(allegato.fileName);

                    CoppiaNomeFileENomeOriginale.Add(String.Format (path + @"\" + Path.GetFileName(allegato.fileName)).ToLowerInvariant(), NomeOriginale);

                    fs.Write(content, 0, content.Length);
                    fs.Close();
                    fs = null;
                }

                return true;
            }
            catch (Exception e)
            {
                //logger.addMessage("Estrazione del file non eseguita.Eccezione: "+e.ToString());
                logger.Debug("Estrazione del file non eseguita.Eccezione: " + e.ToString());
                if (fs != null) fs.Close();
                if (fsAll != null) fsAll.Close();
                return false;
            }
        }

        // CH inserito questo  metodo per consentire l'invio della mail di notifica con allegati. Questo metodo ha sostituito il precedente GetAttachments
        /// <summary>
        /// Creazione di un array di oggetti "CMAttachment":
        /// rappresentano il documento principale e gli allegati
        /// che vengono inviati per mail
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        private static CMAttachment[] GetAttachments(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.InfoUtente infoUtente)
        {
            System.Int64 fileSize = System.Int64.Parse(((DocsPaVO.documento.Documento)schedaDocumento.documenti[0]).fileSize);
            string pathFiles = string.Empty;
            Dictionary<string, string> CoppiaNomeFileENomeOriginale=null;
            if (fileSize > 0)
            {
                string codiceRegistro = "";
                if (schedaDocumento.registro != null)
                {
                    DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione =
                        new DocsPaDB.Query_DocsPAWS.Trasmissione();
                    codiceRegistro = "\\" + trasmissione.GetRegistryCode(schedaDocumento.registro.systemId);
                    trasmissione.Dispose();
                }

                //creazione del logger
                string basePathLogger = ConfigurationManager.AppSettings["LOG_PATH"];
                basePathLogger = basePathLogger.Replace("%DATA", DateTime.Now.ToString("yyyyMMdd"));
                basePathLogger = basePathLogger + "\\Fax";
                DocsPaUtils.Functions.Functions.CheckEsistenzaDirectory(basePathLogger + codiceRegistro);
                string pathLogger = basePathLogger + codiceRegistro + "\\invio";

                // inserimento dei file in una cartella temporanei
                string basePathFiles = ConfigurationManager.AppSettings["LOG_PATH"];
                basePathFiles = basePathFiles.Replace("%DATA", DateTime.Now.ToString("yyyyMMdd"));
                basePathFiles = basePathFiles + "\\Invio_files";
                pathFiles = basePathFiles + codiceRegistro;
                DocsPaUtils.Functions.Functions.CheckEsistenzaDirectory(pathFiles);
                logger.Debug("Estrazione documento principale da inviare");

                // estrazione del documento principale e degli allegati nella cartella "pathFiles"
                
                estrazioneDocPrincipale(infoUtente, schedaDocumento, pathFiles,out CoppiaNomeFileENomeOriginale);
            }

            ArrayList list = new ArrayList();

            if (!pathFiles.Equals(string.Empty))
            {
                string[] files = System.IO.Directory.GetFiles(pathFiles);

                if (CoppiaNomeFileENomeOriginale != null)
                {
                    foreach (string fileAttPath in CoppiaNomeFileENomeOriginale.Keys)
                    {
                        CMAttachment att = new CMAttachment(Path.GetFileName(fileAttPath), Interoperabilità.MimeMapper.GetMimeType(Path.GetExtension(fileAttPath)), fileAttPath);
                        if (CoppiaNomeFileENomeOriginale.ContainsKey(fileAttPath.ToLowerInvariant()))
                            att.name = CoppiaNomeFileENomeOriginale[fileAttPath.ToLowerInvariant()];

                        list.Add(att);
                    }
                }
                else
                {   //fallback
                    foreach (string fileAttPath in files)
                    {
                        CMAttachment att = new CMAttachment(Path.GetFileName(fileAttPath), Interoperabilità.MimeMapper.GetMimeType(Path.GetExtension(fileAttPath)), fileAttPath);
                        list.Add(att);
                    }
                }
            }

            CMAttachment[] retValue = new CMAttachment[list.Count];
            list.CopyTo(retValue);
            list = null;
            return retValue;
        }




        // COMMENTATO DA CH 06/09
        /*
		/// <summary>
		/// Caricamento allagati del documento
		/// </summary>
		/// <param name="documentoSmistamento"></param>
        
		 private static Interoperabilità.CMAttachment[] GetAttachments
										(DocsPaVO.Smistamento.DocumentoSmistamento documentoSmistamento,
										 DocsPaVO.utente.InfoUtente infoUtente)
		{
			 
			ArrayList list=new ArrayList();

			if (documentoSmistamento.ImmagineDocumento!=null)
			{
				list.Add(
					new BusinessLogic.Interoperabilità.CMAttachment
					("Documento_principale" + GetFileExtension(documentoSmistamento.ImmagineDocumento.fullName),
					 documentoSmistamento.ImmagineDocumento.contentType,
                    //documentoSmistamento.ImmagineDocumento.content));
                documentoSmistamento.ImmagineDocumento.fullName)); 
			}

			ArrayList allegatiDocumento=BusinessLogic.Documenti.AllegatiManager.getAllegati(documentoSmistamento.IDDocumento, string.Empty);

			for (int i=0;i<allegatiDocumento.Count;i++)
			{	
				DocsPaVO.documento.FileDocumento fileAllegato=
					BusinessLogic.Documenti.FileManager.getFile
						((DocsPaVO.documento.FileRequest) allegatiDocumento[i],infoUtente);

				list.Add(
					new BusinessLogic.Interoperabilità.CMAttachment
							("Allegato_" + (i + 1).ToString() + GetFileExtension(fileAllegato.fullName),
							 fileAllegato.contentType,
							 fileAllegato.content));

				fileAllegato=null;
			}

			BusinessLogic.Interoperabilità.CMAttachment[] retValue=
				new BusinessLogic.Interoperabilità.CMAttachment[list.Count];
			list.CopyTo(retValue);
			list=null;

			return retValue;
		}
         * 
         * */

        private static ArrayList CreateUOSmistamento(DataSet ds, string idPeople, string idRuolo)
		{
			ArrayList retValue=new ArrayList();

			DocsPaVO.Smistamento.UOSmistamento uo=null;
			DocsPaVO.Smistamento.RuoloSmistamento ruolo=null;
			DocsPaVO.Smistamento.UtenteSmistamento utente=null;

			foreach (DataRow rowUO in ds.Tables["UO"].Rows)
			{
				uo=new DocsPaVO.Smistamento.UOSmistamento();
				uo.ID=rowUO["ID"].ToString();
				uo.Codice=rowUO["CODICE_UO"].ToString();
				uo.Descrizione=rowUO["DESCRIZIONE_UO"].ToString();
				
				foreach (DataRow rowRuolo in rowUO.GetChildRows("UO_RUOLI"))
				{
                    // Verifica se il ruolo esistano o meno degli utenti
                    // Benché di riferimento, il ruolo non deve essere incluso
                    if (rowRuolo.GetChildRows("RUOLI_UTENTI").Length > 0)
                    {
                        ruolo = new DocsPaVO.Smistamento.RuoloSmistamento();
                        ruolo.ID = rowRuolo["ID"].ToString();
                        ruolo.Codice = rowRuolo["CODICE_RUOLO"].ToString();
                        ruolo.Descrizione = rowRuolo["DESCRIZIONE_RUOLO"].ToString();
                        ruolo.RuoloRiferimento = (rowRuolo["RUOLO_RIFERIMENTO"].ToString().Equals("1"));
                        ruolo.Registri = GetListaRegistriRuolo(rowRuolo["ID"].ToString());
                        //indica se il ruolo è superiore a quello che sta smistando
                        //è popolato solamente per la Uo di Appartenenza
                        if (ds.Tables["RUOLI"].Columns["GERARCHIA"] != null)
                            ruolo.Gerarchia = rowRuolo["GERARCHIA"].ToString();
                        if (rowRuolo.Table.Columns.Contains("CHA_DISABLED_TRASM") && rowRuolo["CHA_DISABLED_TRASM"] != null && rowRuolo["CHA_DISABLED_TRASM"].ToString() == "1")
                        {
                            ruolo.disabledTrasm = true;
                        }

                        uo.Ruoli.Add(ruolo);

                        foreach (DataRow rowUtente in rowRuolo.GetChildRows("RUOLI_UTENTI"))
                        {
                            // viene escluso l'utente corrente (idPeople) considerando 
                            // l'utente all'interno del ruolo corrente
                            if ((!rowUtente["ID"].ToString().Equals(idPeople)) ||
                                (rowUtente["ID"].ToString().Equals(idPeople) && !ruolo.ID.Equals(idRuolo)))
                            {

                                utente = new DocsPaVO.Smistamento.UtenteSmistamento();
                                utente.ID = rowUtente["ID"].ToString();
                                utente.IDCorrGlobali = rowUtente["ID_CORR_GLOBALI"].ToString();
                                utente.UserID = rowUtente["CODICE_UTENTE"].ToString();
                                utente.Denominazione = rowUtente["DESCRIZIONE_UTENTE"].ToString();
                                utente.TipoNotificaSmistamento = GetTipoNotificaSmistamento(rowUtente);
                                utente.EMail = rowUtente["EMAIL_UTENTE"].ToString();

                                ruolo.Utenti.Add(utente);
                                utente = null;
                            }
                        }

                        ruolo = null;
                    }
				}

				retValue.Add(uo);
			}

			return retValue;
		}

		private static DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum GetTipoNotificaSmistamento(DataRow rowUtente)
		{
			DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum retValue=DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum.NoMail;
			
			string tipoNotifica=rowUtente["CHA_NOTIFICA"].ToString() +
								rowUtente["CHA_NOTIFICA_CON_ALLEGATO"].ToString();

			if (tipoNotifica.Equals(""))
				retValue=DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum.NoMail;
			else if (tipoNotifica.Equals("E") || tipoNotifica.Equals("E0"))
				retValue=DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum.Mail;
			else if (tipoNotifica.Equals("E1"))
				retValue=DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum.MailConAllegati;
            else if (tipoNotifica.Equals("1"))
                retValue = DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum.SoloAllegati;

			return retValue;
		}

		private static void FillDestinatariMail(Hashtable destinatariHashTable,
												DocsPaVO.Smistamento.UtenteSmistamento utente, DocsPaVO.trasmissione.RagioneTrasmissione ragComp, DocsPaVO.trasmissione.RagioneTrasmissione ragCon)
		{
			DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum tipoNotifica=utente.TipoNotificaSmistamento;

            bool notificaRagComp = (ragComp != null && ragComp.notifica != null && ragComp.notifica != "");
            bool notificaRagCon = (ragCon != null && ragCon.notifica != null && ragCon.notifica != "");


			string key=string.Empty;

          
                if (utente.FlagCompetenza &&
                    tipoNotifica.Equals(DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum.Mail) && !notificaRagComp)
                    key = "COMPETENZA";

                else if (utente.FlagCompetenza &&
                    tipoNotifica.Equals(DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum.MailConAllegati) && !notificaRagComp)
                    key = "COMPETENZA_CON_ALLEGATI";

                else if (utente.FlagCompetenza &&
                    tipoNotifica.Equals(DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum.SoloAllegati) && !notificaRagComp)
                    key = "COMPETENZA_SOLO_ALLEGATI";

                else if (utente.FlagCompetenza && notificaRagComp)
                {
                   if(ragComp.notifica.Equals("EA"))
                   {
                        key = "COMPETENZA_SOLO_ALLEGATI";
                   }
                   else if (ragComp.notifica.Equals("ED"))
                   {
                        key = "COMPETENZA_CON_ALLEGATI";
                   }
                   else if (ragComp.notifica.Equals("E"))
                   {
                        key = "COMPETENZA";
                   }
                }
           
                if (utente.FlagConoscenza &&
                    tipoNotifica.Equals(DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum.Mail) && !notificaRagCon)
                    key = "CC";

                else if (utente.FlagConoscenza &&
                    tipoNotifica.Equals(DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum.MailConAllegati) && !notificaRagCon)
                    key = "CC_CON_ALLEGATI";

                else if (utente.FlagConoscenza &&
               tipoNotifica.Equals(DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum.SoloAllegati) && !notificaRagCon)
                    key = "CC_SOLO_ALLEGATI";


                else if (utente.FlagConoscenza && notificaRagCon)
                {
                    if (ragCon.notifica.Equals("EA"))
                    {
                        key = "CC_SOLO_ALLEGATI";
                    }
                    else if (ragCon.notifica.Equals("ED"))
                    {
                        key = "CC_CON_ALLEGATI";
                    }
                    else if (ragCon.notifica.Equals("E"))
                    {
                        key = "CC";
                    }
                }
            
           

			if (!key.Equals(string.Empty))
			{
				string destinatari=string.Empty;

				if (!destinatariHashTable.ContainsKey(key))
					destinatariHashTable.Add(key,string.Empty);

				destinatari=(string) destinatariHashTable[key];
                if (!destinatari.Equals(string.Empty) && utente.EMail != null && utente.EMail != string.Empty)
					destinatari += ", ";

                if (utente.EMail != null && utente.EMail != string.Empty)
				    destinatari += utente.EMail;

				destinatariHashTable[key]=destinatari;		
			}
		}

		private static string GetDestinatariMail(Hashtable destinatariMail,string key)
		{
			string retValue=string.Empty;

			if (destinatariMail!=null && destinatariMail.ContainsKey(key))
				retValue=(string) destinatariMail[key];

			return retValue;
		}

		private static bool SendMail(DocsPaVO.Smistamento.MittenteSmistamento mittente,
									 DocsPaVO.utente.InfoUtente infoUtente,
									 string to,
									 string ragione,
									 DocsPaVO.Smistamento.DocumentoSmistamento documentoTrasmesso,
									 bool withAttach,
                                     string withText,
                                     bool withLink,
                                     string httpFullPath,
                                    DocsPaVO.Smistamento.DatiTrasmissioneDocumento datiTrasmissioneDocumento,
                                    string idGruppo = ""
                                    //string notaIndividuale
            )
		{
			//string subject="Trasmissione documento";


            string defaultSubject = string.Empty;
            string subject = string.Empty;
            if (System.Configuration.ConfigurationManager.AppSettings["APPLICATION_NAME"] != null &&
                    System.Configuration.ConfigurationManager.AppSettings["APPLICATION_NAME"].ToString().Trim() != "")
            {
                defaultSubject = System.Configuration.ConfigurationManager.AppSettings["APPLICATION_NAME"].ToString();
            }

            //new : defaultSubject andrà letto da una chiave di web.config

            if (defaultSubject.Trim() != "")
            {
                subject = defaultSubject.Trim() + " - Trasmissione";

            }
            else
            {
                subject = "Trasmissione documento";
            }
            
            subject += " : ";
            if (documentoTrasmesso != null)
            {

                if (documentoTrasmesso.Oggetto.Length > 172)
                {
                    subject += documentoTrasmesso.Oggetto.Substring(0, 171) + " ...";
                }
                else
                {
                    subject += documentoTrasmesso.Oggetto;
                }
            }
            else
            {
                if (documentoTrasmesso.Oggetto.Length > 172)
                {
                    subject += documentoTrasmesso.Oggetto.Substring(0, 171) + " ...";
                }
                else
                {
                    subject += documentoTrasmesso.Oggetto;
                }
            }
         
            
            string body = "<font face= 'Arial'>";
            /*
            * 13-10-2014: la parte di codice seguente gestisce singolarmente le mail degli utenti per aggiungere ulteriori informazioni
            * legate al singolo utente
            if (!string.IsNullOrEmpty(withText)) // se la ragione prevede il testo della mail
            {
                body = body + withText.Replace("\n", "<BR>") +"<BR><BR>" +
                " - RAGIONE: " + ragione + "<BR> - OGGETTO: " + documentoTrasmesso.Oggetto + "<BR>";
                if (!string.IsNullOrEmpty(documentoTrasmesso.MittenteDocumento))
                    body += "<BR> - MITTENTE: " + documentoTrasmesso.MittenteDocumento + "<BR>";

                if (documentoTrasmesso.Segnatura != null && documentoTrasmesso.Segnatura != "")
                {

                    body = body + " - SEGNATURA: " + documentoTrasmesso.Segnatura + "<BR>";
                }
                else
                {
                    if (documentoTrasmesso.IDDocumento != null && documentoTrasmesso.IDDocumento != "")
                        body = body + " - ID: " + documentoTrasmesso.IDDocumento + "<BR>";
                }
                 

                body = body.Replace("RAG_TRASM", "<B>" + ragione + "</B>");

                body = body.Replace("DEST_TRASM", "<B>" + destTrasm + "</B>");

               if (datiTrasmissioneDocumento != null && !string.IsNullOrEmpty(datiTrasmissioneDocumento.NoteGenerali))
                {
                    body = body.Replace("NOTE_GEN", "<B>" + datiTrasmissioneDocumento.NoteGenerali + "</B>");
                }
                else
                {
                    body = body.Replace("NOTE_GEN", "---------");
                }

               if (!string.IsNullOrEmpty(notaIndividuale))
                {
                    body = body.Replace("NOTE_IND", "<B>" + notaIndividuale + "</B>");
                }
                else
                {
                    body = body.Replace("NOTE_IND", "---------");
                }

               if (documentoTrasmesso.Segnatura != null && documentoTrasmesso.Segnatura != "") // solo per i protocolli metto la segnatura
                {
                    body = body.Replace("SEGN_ID_DOC", "<B>" + documentoTrasmesso.Segnatura + "</B>");
                }
                else
                {
                    body = body.Replace("SEGN_ID_DOC", "<B>" + documentoTrasmesso.DocNumber + "</B>");
                }


                try
                {
                    if (documentoTrasmesso.TipoDocumento == "A")
                    {
                        if (string.IsNullOrEmpty(datiTrasmissioneDocumento.MittenteTrasmissione))
                            body = body.Replace("MITT_PROTO", "---------");
                        else
                            body = body.Replace("MITT_PROTO", string.Format("<B>{0}</B>", datiTrasmissioneDocumento.MittenteTrasmissione));
                    }
                    else
                        body = body.Replace("MITT_PROTO", "---------");
                }
                catch (Exception e) { logger.Debug(e.Message); }

                body = body.Replace("OGG_DOC", "<B>" + documentoTrasmesso.Oggetto + "</B>");
              
            }
            */
            body = body + "Le è stato smistato il seguente documento: <BR><BR>" +
                " - RAGIONE: " + ragione + " <BR> - NOTA GENERALE:  " + (datiTrasmissioneDocumento == null || string.IsNullOrEmpty(datiTrasmissioneDocumento.NoteGenerali) ? "---------" : datiTrasmissioneDocumento.NoteGenerali)
            + "<BR> - OGGETTO: " + documentoTrasmesso.Oggetto + "<BR>";
            if (!string.IsNullOrEmpty(documentoTrasmesso.MittenteDocumento))
                body += "<BR> - MITTENTE: " + documentoTrasmesso.MittenteDocumento + "<BR>";

            if (documentoTrasmesso.Segnatura != null && documentoTrasmesso.Segnatura != "")
            {

                body = body + " - SEGNATURA: " + documentoTrasmesso.Segnatura + "<BR>";
            }
            else
            {
                if (documentoTrasmesso.IDDocumento != null && documentoTrasmesso.IDDocumento != "")
                    body = body + " - ID: " + documentoTrasmesso.IDDocumento + "<BR>";
            }
            
	
            Interoperabilità.CMAttachment[] attachments=null;
            if (withAttach)
            {
                if (withAttach)
                {
                    //Modifica CH per consentire il reperimento dei file da allegare a mail di notifica
                    //attachments=GetAttachments(documentoTrasmesso,infoUtente);
                    DocsPaVO.documento.SchedaDocumento schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglioPerNotificaAllegati(infoUtente, documentoTrasmesso.IDDocumento, documentoTrasmesso.DocNumber);
                    attachments = GetAttachments(schedaDocumento, infoUtente);
                }
                //attachments=GetAttachments(documentoTrasmesso,infoUtente);

            }

            if (withLink)
            {
                body = body + "<br>Link alla scheda di dettaglio del documento:<br>";
                body = body + "<a href='" + httpFullPath + "/VisualizzaOggetto.aspx?idAmministrazione=" + infoUtente.idAmministrazione;
                if (!string.IsNullOrEmpty(idGruppo))
                    body = body + "&groupId=" + idGruppo; 
                body = body + "&tipoOggetto=D&idObj=" + documentoTrasmesso.DocNumber + "&tipoProto=" + documentoTrasmesso.TipoDocumento + "'>link</a>";
            }
            body = body + "</font>";
            string emailMittente = "";
            emailMittente = checkEmail(mittente);
            if (emailMittente != "")
                mittente.EMail = emailMittente;
			return Interoperabilità.Notifica.notificaByMail(to, mittente.EMail, subject, body, string.Empty, mittente.IDAmministrazione, attachments);
		}

        private static string checkEmail(DocsPaVO.Smistamento.MittenteSmistamento mittente)
        {
            string indirizzoMitt = "";
            DocsPaDB.Query_DocsPAWS.Utenti utente = new DocsPaDB.Query_DocsPAWS.Utenti();
            string fromEmail = utente.GetFromEmailUtente(mittente.IDPeople);
            
            if (fromEmail != null && !fromEmail.Equals(""))
            {
                indirizzoMitt = fromEmail;
            }
            else
            {
                //Ricerca se esiste l'email from notifica dell'amministrazione
                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                string fromEmailAmministra = amm.GetEmailAddress(mittente.IDAmministrazione);
                if (fromEmailAmministra != null && !fromEmailAmministra.Equals(""))
                {
                    indirizzoMitt = fromEmailAmministra;
                }
                else
                {
                    if (mittente.EMail != null && !mittente.EMail.Equals(""))
                    {
                        indirizzoMitt = mittente.EMail;
                    }
                    else // se l'utente mittente  non ha l'email viene presa dal web.config del WS
                    {
                        if (ConfigurationManager.AppSettings["mittenteNotificaTrasmissione"] != null)
                        {
                            indirizzoMitt = ConfigurationManager.AppSettings["mittenteNotificaTrasmissione"];
                        }
                    }
                }
            }
            return indirizzoMitt;
        }

        public static ArrayList GetListaRegistriRuolo(string idRuolo)
        {
            ArrayList lista = new ArrayList();
            DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti smistaDoc=new DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti();
            lista = smistaDoc.GetRegistriRuolo(idRuolo);
            return lista;
        }

        private static bool AccessToExecute(string idRegistroDocumento, ArrayList listaRegistriRuolo)
        {
            bool access = (idRegistroDocumento.Equals(null)||idRegistroDocumento.Equals(string.Empty)
                || idRegistroDocumento.Equals("0"));
            
            if (listaRegistriRuolo.Count > 0)
            {
                if (idRegistroDocumento != null && idRegistroDocumento != string.Empty)
                {
                    foreach (string idRR in listaRegistriRuolo)
                    {
                        if (idRegistroDocumento.Equals(idRR))
                            return true;
                    }
                }
            }

            if (System.Configuration.ConfigurationManager.AppSettings["NO_FILTRO_AOO"] != null && System.Configuration.ConfigurationManager.AppSettings["NO_FILTRO_AOO"] == "1")
            {
                access = true;
            }

            return access;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dsDocumentiTrasmessi"></param>
        /// <returns></returns>
        private static ArrayList GetListDocumentiTrasmessi(DataSet dsDocumentiTrasmessi)
        {
            DocsPaVO.Smistamento.DatiTrasmissioneDocumento datiTrasmissioneDocumento = null;

            ArrayList retValue = new ArrayList();
            Hashtable htTrasmissioni = new Hashtable();
            DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti smistaDoc = new DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti();
            if (dsDocumentiTrasmessi.Tables.Count > 0)
            {
                foreach (DataRow row in dsDocumentiTrasmessi.Tables["DOCUMENTI_TRASMESSI"].Rows)
                {
                    datiTrasmissioneDocumento = new DocsPaVO.Smistamento.DatiTrasmissioneDocumento();

                    datiTrasmissioneDocumento.IDDocumento = row["SYSTEM_ID"].ToString();
                    datiTrasmissioneDocumento.IDTrasmissione = row["ID_TRASMISSIONE"].ToString();
                    if (!htTrasmissioni.ContainsKey(datiTrasmissioneDocumento.IDTrasmissione))
                    {
                        try
                        {
                            htTrasmissioni.Add(datiTrasmissioneDocumento.IDTrasmissione, null);
                        }
                        catch { };

                        datiTrasmissioneDocumento.TrasmissioneConWorkflow = (row["TIPO_RAGIONE"].ToString().Equals("W"));

                        datiTrasmissioneDocumento.IDTrasmissioneSingola = row["ID_TRASMISSIONE_SINGOLA"].ToString();
                        datiTrasmissioneDocumento.IDTrasmissioneUtente = row["ID_TRASMISSIONE_UTENTE"].ToString();
                        datiTrasmissioneDocumento.NoteGenerali = row["NOTE_GENERALI"].ToString();
                        datiTrasmissioneDocumento.NoteIndividualiTrasmSingola = row["NOTE_INDIVIDUALI"].ToString();
                        datiTrasmissioneDocumento.DescRagioneTrasmissione = row["RAGIONE_TRASM"].ToString();
                        datiTrasmissioneDocumento.MittenteTrasmissione = row["MITT_TRASM"].ToString() + " (" + row["RUOLO_MITT"].ToString() + ")";
                        //OLD
                        //datiTrasmissioneDocumento.NoteIndividualiTrasmSingola = smistaDoc.GetNoteIndividuali(datiTrasmissioneDocumento.IDTrasmissioneSingola);
                       
                        retValue.Add(datiTrasmissioneDocumento);
                    }
                    datiTrasmissioneDocumento = null;
                }
            }

            return retValue;
        }

        /// <summary>
        /// Verifica se il sistema è impostato ad avere la gestione della visibilità posticipata ai superiori gerarchici
        /// nelle trasmisissioni con workflow
        /// </summary>
        /// <returns></returns>
        private static bool IsEnabledVisibPosticipataInTrasmConWF()
        {
            return (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["VISIB_POST_TRASM_WF"]) && System.Configuration.ConfigurationManager.AppSettings["VISIB_POST_TRASM_WF"].Equals("1"));
        }
		#endregion

        public static void GetDatiAggUtente(ref DocsPaVO.Smistamento.UtenteSmistamento utSmistamento, string idUtente)
        {
            DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti smistaDoc = new DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti();
            DataSet ds = smistaDoc.getDatiAggUtente(ref utSmistamento, idUtente);
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow rowUtente in ds.Tables["UTENTI"].Rows)
                {
                    utSmistamento.IDCorrGlobali = rowUtente["ID_CORR_GLOBALI"].ToString();
                    utSmistamento.ID = rowUtente["ID_PEOPLE"].ToString();
                    utSmistamento.EMail = rowUtente["EMAIL_UTENTE"].ToString();
                    utSmistamento.TipoNotificaSmistamento = GetTipoNotificaSmistamento(rowUtente);
                }
            }
        }

        public static ArrayList GetUtentiSmistamentoRuolo(string id, string queryParam)
        {
            ArrayList utentiRuolo = new ArrayList();
            DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti smistaDoc = new DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti();
            DataSet ds = smistaDoc.getUtentiRuolo(id, queryParam);
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow rowUtente in ds.Tables["UTENTI"].Rows)
                {
                    DocsPaVO.Smistamento.UtenteSmistamento utSmistamento = new DocsPaVO.Smistamento.UtenteSmistamento();
                    utSmistamento.ID = rowUtente["ID"].ToString();
                    utSmistamento.IDCorrGlobali = rowUtente["ID_CORR_GLOBALI"].ToString();
                    utSmistamento.UserID = rowUtente["CODICE_UTENTE"].ToString();
                    utSmistamento.Denominazione = rowUtente["DESCRIZIONE_UTENTE"].ToString();
                    utSmistamento.EMail = rowUtente["EMAIL_UTENTE"].ToString();
                    utSmistamento.TipoNotificaSmistamento = GetTipoNotificaSmistamento(rowUtente);
                    utentiRuolo.Add(utSmistamento);
                }
            }
            return utentiRuolo;
        }

        public static ArrayList GetRuoliUoSmista(string idUo)
        {
            ArrayList ruoliUo = new ArrayList();
            DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti smistaDoc = new DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti();
            DataSet ds = smistaDoc.getRuoliUo(idUo);
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow rowRuolo in ds.Tables["RUOLI"].Rows)
                {
                    if (rowRuolo["CHA_RIFERIMENTO"].ToString() == "1")
                    {
                        DocsPaVO.Smistamento.RuoloSmistamento ruoloSmista = new DocsPaVO.Smistamento.RuoloSmistamento();
                        ruoloSmista.ID = rowRuolo["ID"].ToString();
                        ruoloSmista.Codice = rowRuolo["CODICE"].ToString();
                        ruoloSmista.Descrizione = rowRuolo["DESCRIZIONE"].ToString();
                        ruoloSmista.Registri = GetListaRegistriRuolo(ruoloSmista.ID);
                        ruoloSmista.Utenti = GetUtentiSmistamentoRuolo(ruoloSmista.ID, "R");
                        ruoliUo.Add(ruoloSmista);
                    }
                }
            }
            return ruoliUo;
        }

        public static ArrayList GetAllRagioniTrasmissioneSmistamento(string idAmm)
        {
            ArrayList arrayRag = new ArrayList();
            DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti dbSmistaDoc = new DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti();

            DataSet ds = dbSmistaDoc.getAllRagioniTrasmSmista(idAmm);

            if (ds != null)
            {
                if (ds.Tables["RAG_TRASM"].Rows.Count > 0)
                {
                    foreach (DataRow r in ds.Tables["RAG_TRASM"].Rows)
                    {
                        DocsPaVO.trasmissione.RagioneTrasmissione ragione = new DocsPaVO.trasmissione.RagioneTrasmissione();
                        ragione.systemId = r["SYSTEM_ID"].ToString();
                        ragione.descrizione = r["VAR_DESC_RAGIONE"].ToString();
                        ragione.tipo = r["CHA_TIPO_RAGIONE"].ToString();
                        ragione.tipoDiritti = (DocsPaVO.trasmissione.TipoDiritto)DocsPaDB.Utils.HashTableManager.GetKeyFromValue(DocsPaVO.trasmissione.RagioneTrasmissione.tipoDirittoStringa, r["CHA_TIPO_DIRITTI"].ToString());
                        ragione.tipoDestinatario = (DocsPaVO.trasmissione.TipoGerarchia)DocsPaDB.Utils.HashTableManager.GetKeyFromValue(DocsPaVO.trasmissione.RagioneTrasmissione.tipoGerarchiaStringa, r["CHA_TIPO_DEST"].ToString());
                        ragione.risposta = r["CHA_RISPOSTA"].ToString();
                        ragione.note = r["VAR_NOTE"].ToString();
                        ragione.eredita = r["CHA_EREDITA"].ToString();
                        ragione.tipoRisposta = r["CHA_TIPO_RISPOSTA"].ToString();
                        ragione.prevedeCessione = r["CHA_CEDE_DIRITTI"].ToString();
                        ragione.notifica = r["VAR_NOTIFICA_TRASM"].ToString();
                        ragione.testoMsgNotificaDoc = r["VAR_TESTO_MSG_NOTIFICA_DOC"].ToString();
                        ragione.testoMsgNotificaFasc = r["VAR_TESTO_MSG_NOTIFICA_FASC"].ToString();
                        arrayRag.Add(ragione);
                    }
                }
            }
            return arrayRag;
        }

        private static DocsPaVO.trasmissione.RagioneTrasmissione GetRagioneSelezionata(string descRagione, ArrayList listaRagioni)
        {
            DocsPaVO.trasmissione.RagioneTrasmissione result = null;
            foreach (DocsPaVO.trasmissione.RagioneTrasmissione ragione in listaRagioni)
            {
                if (ragione.descrizione.ToUpper().Equals(descRagione.ToUpper()))
                {
                    result = ragione;
                    break;
                }
            }
            return result;
        }


        private static void FillDestinatariMail(Hashtable destinatariHashTable,
                                                DocsPaVO.Smistamento.UtenteSmistamento utente, DocsPaVO.trasmissione.RagioneTrasmissione ragione)
        {
            DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum tipoNotifica = utente.TipoNotificaSmistamento;               

            string key = string.Empty;


            if (string.IsNullOrEmpty(ragione.notifica))
            {
                switch (tipoNotifica)
                {
                    case DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum.Mail:
                        key = ragione.descrizione.Trim().ToUpper();
                        break;
                    case DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum.MailConAllegati:
                        key = ragione.descrizione.Trim().ToUpper() + "_CON_ALLEGATI";                        
                        break;
                    case DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum.SoloAllegati:
                        key = ragione.descrizione.Trim().ToUpper() + "_SOLO_ALLEGATI";
                        break;
                }
            }
            else
            {
                if (ragione.notifica.Equals("EA"))
                {
                    key = ragione.descrizione.Trim().ToUpper() + "_SOLO_ALLEGATI";
                }
                else if (ragione.notifica.Equals("ED"))
                {
                    key = ragione.descrizione.Trim().ToUpper() + "_CON_ALLEGATI";
                }
                else if (ragione.notifica.Equals("E"))
                {
                    key = ragione.descrizione.Trim().ToUpper();
                }
            }

            if (!key.Equals(string.Empty))
            {
                string destinatari = string.Empty;

                if (!destinatariHashTable.ContainsKey(key))
                    destinatariHashTable.Add(key, string.Empty);

                destinatari = (string)destinatariHashTable[key];
                if (!destinatari.Equals(string.Empty) && utente.EMail != null && utente.EMail != string.Empty)
                    destinatari += ", ";

                if (utente.EMail != null && utente.EMail != string.Empty)
                    destinatari += utente.EMail;

                destinatariHashTable[key] = destinatari;
            }
        }

        /// <summary>
        /// Reperimento delle trasmissioni ricevute
        /// </summary>
        /// <param name="systemIDRuolo"></param>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        public static ArrayList GetListDocumentiTrasmessiNotifyFilters(DocsPaVO.Smistamento.MittenteSmistamento mittente, DocsPaVO.filtri.FiltroRicerca[] filtriRicerca)
        {
            //Lnr 22/05/2013 (ora prende i records dalla tab Notify
            DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti dbSmistaDoc = new DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti();
            DataSet ds = dbSmistaDoc.GetListDocumentiTrasmessiNotifyFilters(mittente, filtriRicerca);
            dbSmistaDoc = null;

            return GetListDocumentiTrasmessiNotify(ds, mittente);
        }


        /// <summary>
        /// Reperimento delle trasmissioni ricevute
        /// </summary>
        /// <param name="systemIDRuolo"></param>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        public static ArrayList GetListDocumentiTrasmessiNotify(List<Notification> notifications, DocsPaVO.Smistamento.MittenteSmistamento mittente)
        {
            DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti dbSmistaDoc = new DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti();
            DataSet ds = dbSmistaDoc.GetListDocumentiTrasmessiNotify(notifications,mittente);
            return GetListDocumentiTrasmessiNotify(ds, mittente);
        }

        private static ArrayList GetListDocumentiTrasmessiNotify(DataSet dsDocumentiTrasmessiNotify, DocsPaVO.Smistamento.MittenteSmistamento mittente)
        {
            DocsPaVO.Smistamento.DatiTrasmissioneDocumento datiTrasmissioneDocumento = null;
            string errorMessage = string.Empty;
            DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente() { idPeople = mittente.IDPeople, idGruppo = mittente.IDGroup};
            ArrayList retValue = new ArrayList();
            Hashtable htTrasmissioni = new Hashtable();
            DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti smistaDoc = new DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti();
            if (dsDocumentiTrasmessiNotify.Tables.Count > 0)
            {
                foreach (DataRow row in dsDocumentiTrasmessiNotify.Tables["DOCUMENTI_TRASMESSI"].Rows)
                {
                    datiTrasmissioneDocumento = new DocsPaVO.Smistamento.DatiTrasmissioneDocumento();

                    datiTrasmissioneDocumento.IDDocumento = row["ID_DOCUMENTO"].ToString();
                    datiTrasmissioneDocumento.IDTrasmissione = row["ID_TRASMISSIONE"].ToString();
                    if (!htTrasmissioni.ContainsKey(datiTrasmissioneDocumento.IDTrasmissione))
                    {
                        try
                        {
                            htTrasmissioni.Add(datiTrasmissioneDocumento.IDTrasmissione, null);
                        }
                        catch { };

                        datiTrasmissioneDocumento.TrasmissioneConWorkflow = (row["TIPO_RAGIONE"].ToString().Equals("W"));
                        datiTrasmissioneDocumento.IDTrasmissioneSingola = row["ID_TRASMISSIONE_SINGOLA"].ToString();
                        datiTrasmissioneDocumento.IDTrasmissioneUtente = row["ID_TRASMISSIONE_UTENTE"].ToString();
                        datiTrasmissioneDocumento.NoteGenerali = row["NOTE_GENERALI"].ToString();
                        datiTrasmissioneDocumento.NoteIndividualiTrasmSingola = row["NOTE_INDIVIDUALI"].ToString();
                        datiTrasmissioneDocumento.DescRagioneTrasmissione = row["RAGIONE_TRASM"].ToString();
                        string people = row["PEOPLE"].ToString();
                        string role = row["ROLE"].ToString();
                        datiTrasmissioneDocumento.MittenteTrasmissione = people + " (" + role + ")";
                        if (Documenti.DocManager.VerificaACL("D", datiTrasmissioneDocumento.IDDocumento, infoUtente, out errorMessage) != 0)
                        {
                            retValue.Add(datiTrasmissioneDocumento);
                        }
                    }
                    datiTrasmissioneDocumento = null;
                }
            }

            return retValue;
        }


	}
}
