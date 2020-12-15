using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using log4net;
using BusinessLogic.interoperabilita.Semplificata;

namespace BusinessLogic.Spedizione
{
    /// <summary>
    /// Classe per la gestione delle spedizioni dei documenti
    /// </summary>
    public sealed class SpedizioneManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(SpedizioneManager));

        #region Public methods

        /// <summary>
        /// Reperimento delle informazioni riguardo la modalità di spedizione del documento nell'ambito dell'amministrazione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static DocsPaVO.Spedizione.ConfigSpedizioneDocumento GetConfigSpedizioneDocumento(DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.amministrazione.InfoAmministrazione amministrazione = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoUtente.idAmministrazione);

            return amministrazione.SpedizioneDocumenti;
        }

        /// <summary>
        /// Reperimento delle informazioni relative alle spedizioni effettuate per un documento
        /// </summary>
        /// <param name="infoUtente">Utente che richiede le informazioni</param>
        /// <param name="documento">Documento oggetto della spedizione</param>
        /// <returns>
        /// Informazioni relative ai destinatari del documento
        /// </returns>
        public static DocsPaVO.Spedizione.SpedizioneDocumento GetSpedizioneDocumento(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento documento)
        {
            DocsPaVO.Spedizione.SpedizioneDocumento infoSpedizioni = null;

            if (documento != null && !string.IsNullOrEmpty(documento.systemId))
            {
                try
                {
                    CheckProtocolloUscita(documento);

                    infoSpedizioni = new DocsPaVO.Spedizione.SpedizioneDocumento();
                    infoSpedizioni.IdDocumento = documento.systemId;

                    // Per impostazione predefinita, viene impostato il registro del protocollo del documento
                    infoSpedizioni.IdRegistroRfMittente = documento.registro.systemId;

                    // Reperimento del protocollo in uscita
                    DocsPaVO.documento.ProtocolloUscita protocolloUscita = (DocsPaVO.documento.ProtocolloUscita)documento.protocollo;

                    // Reperimento dei destinari del documento
                    FetchDestinatari(infoSpedizioni, infoUtente, documento);

                    SetStatoSpedito(infoSpedizioni);

                }
                catch (Exception ex)
                {
                    infoSpedizioni = null;
                    logger.Debug(new ApplicationException("Errore nel reperimento delle informazioni sulle spedizioni del documento", ex));
                }
            }
            else
            {
                infoSpedizioni = null;

            }

            return infoSpedizioni;
        }


        public static bool DocumentAlreadySent_Opt(string idDocument)
        {
            bool retval = false;
            DocsPaDB.Query_DocsPAWS.Interoperabilita dbInt = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
            retval = dbInt.DocumentAlreadySent_Opt(idDocument);
            return retval;
        }

        /// <summary>
        /// Task di spedizione di un documento a tutti i possibili destinatari
        /// </summary>
        /// <param name="infoUtente">Utente che richiede le informazioni</param>
        /// <param name="documento">Documento oggetto della spedizione</param>
        /// <returns>
        /// Informazioni relative ai destinatari del documento successivamente alla spedizione
        /// </returns>
        public static DocsPaVO.Spedizione.SpedizioneDocumento SpedisciDocumento(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento documento)
        {
            DocsPaVO.Spedizione.SpedizioneDocumento infoSpedizione = GetSpedizioneDocumento(infoUtente, documento);

            //*******************************************************
            // Giordano Iacozzilli 21/09/2012 
            // Ripristino della sola trasmissione in automatico ai 
            // destinatari interni nei protocolli in uscita
            //*******************************************************

            //OLD CODE:
            //DocsPaVO.amministrazione.InfoAmministrazione _infamm = new DocsPaVO.amministrazione.InfoAmministrazione();
            //// Include tutti i destinatari del documento nella spedizione
            //foreach (DocsPaVO.Spedizione.DestinatarioEsterno dest in infoSpedizione.DestinatariEsterni)
            //{
            //    if (dest.Interoperante)
            //        dest.IncludiInSpedizione = true;
            //}
            //foreach (DocsPaVO.Spedizione.Destinatario dest in infoSpedizione.DestinatariInterni)
            //    dest.IncludiInSpedizione = true;
            //  return SpedisciDocumento(infoUtente, documento, infoSpedizione);

            //NEW CODE:
            DocsPaVO.amministrazione.InfoAmministrazione _infamm = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoUtente.idAmministrazione);
            
            //In base alla valorizzazione dei flag (spedizioneAutomatica e trasmissioneAutomatica) procedo con la valorizzazione 
            //o meno delle liste dei destinatari:

            //I flag True entrambi
            if (_infamm.SpedizioneDocumenti.SpedizioneAutomaticaDocumento && _infamm.SpedizioneDocumenti.TrasmissioneAutomaticaDocumento)
            {
                // Include tutti i destinatari del documento nella spedizione
                foreach (DocsPaVO.Spedizione.DestinatarioEsterno dest in infoSpedizione.DestinatariEsterni)
                {
                    if (dest.Interoperante)
                        dest.IncludiInSpedizione = true;
                }
                foreach (DocsPaVO.Spedizione.Destinatario dest in infoSpedizione.DestinatariInterni)
                    dest.IncludiInSpedizione = true;
            }

            //Attivo solo spedizioni automatico
            if (_infamm.SpedizioneDocumenti.SpedizioneAutomaticaDocumento && !_infamm.SpedizioneDocumenti.TrasmissioneAutomaticaDocumento)
            {
                // Include tutti i destinatari del documento nella spedizione
                foreach (DocsPaVO.Spedizione.DestinatarioEsterno dest in infoSpedizione.DestinatariEsterni)
                {
                    if (dest.Interoperante)
                        dest.IncludiInSpedizione = true;
                }
                foreach (DocsPaVO.Spedizione.Destinatario dest in infoSpedizione.DestinatariInterni)
                    dest.IncludiInSpedizione = false;
            }

            //Attivo solo la trasmissione
            if (!_infamm.SpedizioneDocumenti.SpedizioneAutomaticaDocumento && _infamm.SpedizioneDocumenti.TrasmissioneAutomaticaDocumento)
            {
                // Include tutti i destinatari del documento nella spedizione
                foreach (DocsPaVO.Spedizione.DestinatarioEsterno dest in infoSpedizione.DestinatariEsterni)
                {
                    if (dest.Interoperante)
                        dest.IncludiInSpedizione = false;
                }
                foreach (DocsPaVO.Spedizione.Destinatario dest in infoSpedizione.DestinatariInterni)
                    dest.IncludiInSpedizione = true;
            }

            return SpedisciDocumento(infoUtente, documento, infoSpedizione);
        }

        /// <summary>
        /// Task di spedizione di un documento
        /// </summary>
        /// <param name="infoUtente">Utente che richiede le informazioni</param>
        /// <param name="documento">Documento oggetto della spedizione</param>
        /// <param name="infoSpedizioni">
        /// Metadati delle spedizioni effettuate per il documento
        /// </param>
        /// <returns>
        /// Informazioni relative ai destinatari del documento successivamente alla spedizione
        /// </returns>
        public static DocsPaVO.Spedizione.SpedizioneDocumento SpedisciDocumento(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento documento, DocsPaVO.Spedizione.SpedizioneDocumento infoSpedizioni)
        {
            ArrayList inclusiInSpedizione = new ArrayList();
            try
            {
                CheckProtocolloUscita(documento);

                //Verifico se il documento è in libro firma e in caso se il passo in attesa è quello di spedizione ed il titolare è l'utente che sta effettuando la spedizione
                #region CHECK_LIBRO_FIRMA
                if ((LibroFirma.LibroFirmaManager.IsDocInLibroFirma(documento.systemId)))
                {
                    if (!LibroFirma.LibroFirmaManager.IsTitolarePassoInAttesa(documento.systemId, infoUtente, DocsPaVO.LibroFirma.Azione.DOCUMENTOSPEDISCI))
                    {
                        throw new Exception("Non è possibile procedere con la spedizione poichè è attivo un processo di firma per il documento.");
                    }
                }
                #endregion

                //Aggiorno le versioni del documento principale da DB perchè in alcuni casi quello inviato dal FE non è aggiornato(caso di firma documento e poi spedisci INC000001057841)
                documento.documenti = new ArrayList();
                documento.documenti.AddRange(BusinessLogic.Documenti.DocManager.GetVersionsMainDocument(infoUtente, documento.docNumber));

                // PEC 4 - requisito 5 - storico spedizioni
                // Dato che alla fine i destinatari perdono l'includiInSpedizione, mi salvo i loro ID in un arraylist.
                // Modifica per un errore della notifica di mancata consegna IS
                foreach (DocsPaVO.Spedizione.DestinatarioEsterno corr in infoSpedizioni.DestinatariEsterni)
                {
                    if (corr.IncludiInSpedizione)
                    {
                        DocsPaDB.Query_DocsPAWS.Interoperabilita interop = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                        // Modifica per Occasionali.
                        //string canalePref = (corr.DatiDestinatari[0].canalePref != null ? corr.DatiDestinatari[0].canalePref.descrizione : "MAIL");
                        DocsPaVO.utente.Canale canalePref = corr.DatiDestinatari[0].canalePref;
                        interop.InsertInStoricoSpedizioni(
                            documento.systemId,
                            corr.Id,
                            "In fase di spedizione",
                            corr.Email,
                            infoUtente.idGruppo,
                            canalePref,
                            (!string.IsNullOrEmpty(infoSpedizioni.mailAddress) ? infoSpedizioni.mailAddress : ""),
                            (!string.IsNullOrEmpty(infoSpedizioni.IdRegistroRfMittente) ? infoSpedizioni.IdRegistroRfMittente : "")
                            );
                    }
                }


                // Reperimento del protocollo in uscita
                DocsPaVO.documento.ProtocolloUscita protocolloUscita = (DocsPaVO.documento.ProtocolloUscita)documento.protocollo;

                // Registro o RF mittente della spedizione per interoperabilità
                DocsPaVO.utente.Registro registroRfMittente = null;

                if (!string.IsNullOrEmpty(infoSpedizioni.IdRegistroRfMittente))
                    registroRfMittente = BusinessLogic.Utenti.RegistriManager.getRegistro(infoSpedizioni.IdRegistroRfMittente);
                // Prelevamento di tutti i corrispondenti a cui bisogna spedire per interoperabilità semplificata (se attiva)
                IEnumerable<DocsPaVO.Spedizione.DestinatarioEsterno> destIS = new List<DocsPaVO.Spedizione.DestinatarioEsterno>();
                if (InteroperabilitaSemplificataManager.IsEnabledSimplifiedInteroperability(infoUtente.idAmministrazione))
                {
                    destIS =
                        infoSpedizioni.DestinatariEsterni.Where(
                            d => d.IncludiInSpedizione &&
                                d.Interoperante &&
                                d.DatiDestinatari.Where(d1 => d1.canalePref != null &&
                                    d1.canalePref.typeId.Equals(InteroperabilitaSemplificataManager.InteroperabilityCode) &&
                                    d1.Url != null && d1.Url.Count > 0 &&
                                    !String.IsNullOrEmpty(d1.Url[0].Url) &&
                                    Uri.IsWellFormedUriString(d1.Url[0].Url, UriKind.Absolute)).Count() > 0);
                    if (destIS.Count() > 0)
                    {
                        foreach (DocsPaVO.Spedizione.DestinatarioEsterno dtemp in destIS)
                        {
                            inclusiInSpedizione.Add(dtemp.Id);
                        }

                        // Spedizione per IS
                        SpedisciPerInteroperabilitaSemplificata(infoUtente, documento, destIS);
                    }
                }

                //spedisco ai destinatari interoperanti esterni(PEC - IS)
                if (registroRfMittente != null)
                    registroRfMittente.email = infoSpedizioni.mailAddress;
                foreach (DocsPaVO.Spedizione.DestinatarioEsterno destinatario in infoSpedizioni.DestinatariEsterni.Where(e => e.IncludiInSpedizione))
                {
                    // Se il destinatario non è già stato spedito per IS si procede con la spedizione classica
                    if (!destIS.Contains(destinatario))
                    {
                        inclusiInSpedizione.Add(destinatario.Id);
                        if (destinatario.Interoperante)
                        {
                            if (registroRfMittente != null)
                            {
                                // Il destinatario è esterno all'AOO su cui è stato protocollato il documento,
                                // pertanto il documento viene inviato al destinatario per interoperabilità
                                // modifica per l'email dell corrispondente occasionale 
                                if ((!string.IsNullOrEmpty(destinatario.DatiDestinatari[0].tipoCorrispondente)
                                    && destinatario.DatiDestinatari[0].tipoCorrispondente == "O") || destinatario.DatiDestinatari[0].tipoIE.Equals("E"))
                                    SpedisciPerInteroperabilita(infoUtente, documento, registroRfMittente, destinatario);

                            }
                            else
                            {
                                destinatario.StatoSpedizione = GetStatoErroreInSpedizione("Impossibile effettuare la spedizione. E' necessario selezionare il registro/rf del mittente");
                            }
                        }
                        else
                            destinatario.StatoSpedizione = GetStatoErroreInSpedizione("Impossibile spedire il documento ad un destinatario non interoperante");
                    }
                }
                //spedisco ai destinatari interoperanti interni
                if (System.Configuration.ConfigurationManager.AppSettings["INTEROP_INT_NO_MAIL"] != null &&
                            System.Configuration.ConfigurationManager.AppSettings["INTEROP_INT_NO_MAIL"].ToString() != "0")
                {
                    System.Collections.Generic.List<string> aoo = new List<string>();
                    foreach (DocsPaVO.Spedizione.DestinatarioEsterno destinatario in infoSpedizioni.DestinatariEsterni.
                        Where(e => e.DatiDestinatari[0].tipoIE != null && e.DatiDestinatari[0].tipoIE.Equals("I") && e.IncludiInSpedizione))
                    {
                        inclusiInSpedizione.Add(destinatario.Id);
                        if (!aoo.Contains(destinatario.DatiDestinatari[0].codiceAOO))
                        {
                            if (registroRfMittente != null)
                            {
                                SpedisciPerInteroperabilita(infoUtente, documento, registroRfMittente, destinatario);
                                if (destinatario.StatoSpedizione.Stato == DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.Spedito)
                                    aoo.Add(destinatario.DatiDestinatari[0].codiceAOO);
                            }
                            else
                            {
                                destinatario.StatoSpedizione = GetStatoErroreInSpedizione("Impossibile effettuare la spedizione. E' necessario selezionare il registro/rf del mittente");
                            }
                        }
                        else
                        {
                            foreach (DocsPaVO.Spedizione.DestinatarioEsterno dest in infoSpedizioni.DestinatariEsterni.
                                Where(e => e.DatiDestinatari[0].tipoIE.Equals("I")))
                            {
                                if (dest.DatiDestinatari[0].codiceAOO.Equals(destinatario.DatiDestinatari[0].codiceAOO))
                                {
                                    destinatario.StatoSpedizione = dest.StatoSpedizione;
                                    destinatario.DataUltimaSpedizione = dest.DataUltimaSpedizione;
                                    destinatario.IncludiInSpedizione = false;
                                    break;
                                }
                            }
                        }
                    }
                }
                //spedisco ai destinatari interni(Intra AOO)
                foreach (DocsPaVO.Spedizione.DestinatarioInterno destinatario in infoSpedizioni.DestinatariInterni.Where(e => e.IncludiInSpedizione && !e.DisabledTrasm))
                {
                    // Il destinatario è interno, cioè ha visibilità sull'AOO sui cui è stato procollato il documento,
                    // pertanto viene effettuata una nuova trasmissione

                    //Andrea - Aggiunto parametro infospedizione per gestire Alert - lista destinatari non raggiungibili.
                    SpedisciPerTrasmissione(infoUtente, documento, destinatario, infoSpedizioni);
                    //End Andrea
                }

                SetStatoSpedito(infoSpedizioni);
            }
            catch (Exception ex)
            {
                infoSpedizioni.Spedito = false;
                DocsPaDB.Query_DocsPAWS.Interoperabilita interop = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                interop.UpdateStoricoSpedizione(documento.systemId, "", infoUtente.idGruppo,
                    (!string.IsNullOrEmpty(infoSpedizioni.IdRegistroRfMittente) ? infoSpedizioni.IdRegistroRfMittente : ""),
                    "Errore nella spedizione del documento", true);

                logger.Debug(new ApplicationException(string.Format("Errore nella spedizione del documento con id {0}", documento.systemId), ex));
            }

            try
            {
                if (infoSpedizioni.DestinatariInterni != null)
                    infoSpedizioni.DestinatariInterni = new List<DocsPaVO.Spedizione.DestinatarioInterno>(infoSpedizioni.DestinatariInterni.OrderBy(destInt => destInt.StatoSpedizione.Descrizione));

                if (infoSpedizioni.DestinatariEsterni != null)
                    infoSpedizioni.DestinatariEsterni = new List<DocsPaVO.Spedizione.DestinatarioEsterno>(infoSpedizioni.DestinatariEsterni.OrderBy(destEst => destEst.StatoSpedizione.Descrizione));
            }
            catch (Exception ex1)
            {
                // in caso di eccezione i destinatari vengono restituiti non ordinati
                logger.Debug(new ApplicationException("Errore nell'ordinamento dei destinatari", ex1));
            }
            // PEC 4 - requisito 5 - storico spedizioni

            foreach (DocsPaVO.Spedizione.DestinatarioEsterno corr in infoSpedizioni.DestinatariEsterni)
            {
                if (inclusiInSpedizione.Contains(corr.Id))
                {
                    DocsPaDB.Query_DocsPAWS.Interoperabilita interop = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                    // Modifica per Occasionali.
                    //string canalePref = (corr.DatiDestinatari[0].canalePref != null ? corr.DatiDestinatari[0].canalePref.descrizione : "MAIL");
                    //interop.InsertInStoricoSpedizioni(
                    //    documento.systemId,
                    //    corr.Id,
                    //    corr.StatoSpedizione.Descrizione,
                    //    corr.Email,
                    //    infoUtente.idGruppo,
                    //    canalePref,
                    //    (!string.IsNullOrEmpty(infoSpedizioni.mailAddress) ? infoSpedizioni.mailAddress : ""),
                    //    (!string.IsNullOrEmpty(infoSpedizioni.IdRegistroRfMittente) ? infoSpedizioni.IdRegistroRfMittente : "")
                    //    );

                    // Modifica da inserimento a update
                    interop.UpdateStoricoSpedizione(
                        documento.systemId,
                        corr.Id,
                        infoUtente.idGruppo,
                        (!string.IsNullOrEmpty(infoSpedizioni.IdRegistroRfMittente) ? infoSpedizioni.IdRegistroRfMittente : ""),
                        corr.StatoSpedizione.Descrizione,
                        false);
                       
                }
            }
            return infoSpedizioni;
        }

        #endregion
        #region INVIO MAIL AL SUPPORT PER SPEDIZIONE MASSIVA

        public static void SendMailSupportForDocumentDelivery(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Utente infoUtente, DocsPaVO.utente.Ruolo ruolo)
        {
            try
            {
                string bodyMail = string.Empty;
                string subject = "Richiesta di spedizione massiva";
                string emailSender = string.Empty;
                string emailRecipients = string.Empty;

                //Estraggo l'email in amministrazione da cui spedire
                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                emailSender = amm.GetEmailAddress(infoUtente.idAmministrazione);

                //Estraggo gli indirizzi email dei destinatari
                if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_EMAIL_ADDRESS_SUPPORT")) &&
                    !DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_EMAIL_ADDRESS_SUPPORT").ToString().Equals("0"))
                {
                    emailRecipients = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_EMAIL_ADDRESS_SUPPORT").ToString();
                }
                bodyMail = CreateBodyMessage(schedaDocumento, infoUtente, ruolo);
                if(!string.IsNullOrEmpty(emailSender) && !string.IsNullOrEmpty(emailRecipients))
                    BusinessLogic.Interoperabilità.Notifica.notificaByMail(emailRecipients, emailSender, subject, bodyMail, string.Empty, infoUtente.idAmministrazione, null);

            }
            catch (Exception e)
            {
                logger.Error("Errore nel metodo SendMailSupportForDocumentDelivery " + e.Message);
            }
        }


        private static string CreateBodyMessage(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Utente infoUtente, DocsPaVO.utente.Ruolo ruolo)
        {
            string bodyMail = "<font face='Arial'>";

            DocsPaVO.amministrazione.InfoAmministrazione infoAmm = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoUtente.idAmministrazione);

            bodyMail += "Amministrazione: <B>" + infoAmm.Codice + " - " + infoAmm.Descrizione + "</B><br />";
            bodyMail += "Utente: <B>" + infoUtente.cognome + " " + infoUtente.nome + " (" + ruolo.descrizione + ")" + "</B><br />";
            bodyMail += "Id documento: <B>" + schedaDocumento.docNumber + "</B><br />";
            bodyMail += "Oggetto del documento: <B>" + schedaDocumento.oggetto.descrizione + "</B><br />";
            bodyMail += "Data spedizione: <B>" + System.DateTime.Now + "</B><br />";

            return bodyMail;
        }
        #endregion

        #region Private members

        /// <summary>
        /// 
        /// </summary>
        private const string ERROR_DESTINATARIO_OCCASIONALE = "Impossibile spedire il documento ad un occasionale";

        /// <summary>
        /// 
        /// </summary>
        private SpedizioneManager()
        { }

        /// <summary>
        /// Caricamento dei destinatari del documento suddivisi per interoperanti e non interoperanti
        /// </summary>
        /// <param name="infoSpedizioni"></param>
        /// <param name="infoUtente"></param>
        /// <param name="documento"></param>
        /// <returns></returns>
        private static void FetchDestinatari(DocsPaVO.Spedizione.SpedizioneDocumento infoSpedizioni, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento documento)
        {
            // Caricamento dei destinari esterni del documento
            FetchDestinatariEsterni(infoSpedizioni, infoUtente, documento);

            // Caricamento dei destinatari interni del documento
            FetchDestinatariInterni(infoSpedizioni, infoUtente, documento);
        }

        /// <summary>
        /// Determina se il corrispondente è un interno
        /// </summary>
        /// <param name="destinatario"></param>
        /// <returns></returns>
        private static bool IsDestinatarioInterno(DocsPaVO.utente.Corrispondente destinatario)
        {

            return (!string.IsNullOrEmpty(destinatario.tipoIE) && destinatario.tipoIE == "I");
        }

        /// <summary>
        /// Determina se il corrispondente è un interno
        /// </summary>
        /// <param name="destinatario"></param>
        /// <returns></returns>
        private static bool IsDestinatarioInterno(DocsPaVO.utente.Corrispondente destinatario, string id_registro_Documento, DocsPaVO.utente.InfoUtente infoUtente)
        {

            if (!string.IsNullOrEmpty(destinatario.tipoIE) && destinatario.tipoIE == "I")
            {
                DocsPaDB.Query_DocsPAWS.Rubrica rub = new DocsPaDB.Query_DocsPAWS.Rubrica(infoUtente);
                return rub.IsDestinatarioInterno(destinatario, id_registro_Documento);
            }

            return false;
        }


        /// <summary>
        /// Caricamento dei destinatari esterni
        /// </summary>
        /// <param name="infoSpedizioni"></param>
        /// <param name="infoUtente"></param>
        /// <param name="documento"></param>
        private static void FetchDestinatariEsterni(DocsPaVO.Spedizione.SpedizioneDocumento infoSpedizioni, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento documento)
        {
            DocsPaVO.documento.ProtocolloUscita protocolloUscita = (DocsPaVO.documento.ProtocolloUscita)documento.protocollo;

            DocsPaVO.Interoperabilita.SendDocumentResponse response = new DocsPaVO.Interoperabilita.SendDocumentResponse();

            ArrayList destinatariProtocollo = new ArrayList();
            destinatariProtocollo.AddRange(protocolloUscita.destinatari);
            destinatariProtocollo.AddRange(protocolloUscita.destinatariConoscenza);
            Hashtable table = BusinessLogic.Interoperabilità.InteroperabilitaInvioSegnatura.dividiDestinatari(destinatariProtocollo, documento.registro.codRegistro, documento.registro.email, response, infoUtente.idAmministrazione);

            List<DocsPaVO.utente.Corrispondente> corrispondentiInteroperanti = new List<DocsPaVO.utente.Corrispondente>();

            foreach (DictionaryEntry entry in table)
            {
                // Impostazione dell'email come identificativo univoco del destinatario
                //destinatario.Id = destinatario.Email;
                ArrayList listDestinatari = (ArrayList)entry.Value;
                for (int i = 0; i < listDestinatari.Count; i++)
                {
                    /* corrispondentiInteroperanti.AddRange((DocsPaVO.utente.Corrispondente[])listDestinatari.ToArray(typeof(DocsPaVO.utente.Corrispondente)));
                     destinatario.DatiDestinatari.AddRange((DocsPaVO.utente.Corrispondente[])listDestinatari.ToArray(typeof(DocsPaVO.utente.Corrispondente)));
                     destinatario.DataUltimaSpedizione = GetDataUltimaSpedizione(infoUtente, documento, destinatario.Email);
                     
                     // Determina se il documento è stato spedito almeno una volta al destinatario
                     bool spedito = !string.IsNullOrEmpty(destinatario.DataUltimaSpedizione);

                     // Nel caso non sia stato spedito, viene forzata l'impostazione di spedizione
                     //destinatario.IncludiInSpedizione = !spedito;
                     destinatario.IncludiInSpedizione = spedito;

                     if (spedito)
                         destinatario.StatoSpedizione = GetStatoSpedizione(DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.Spedito);
                     else
                         destinatario.StatoSpedizione = GetStatoSpedizione(DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.DaSpedire);

                     infoSpedizioni.DestinatariEsterni.Add(destinatario);*/
                    DocsPaVO.Spedizione.DestinatarioEsterno destinatario = new DocsPaVO.Spedizione.DestinatarioEsterno();
                    destinatario.Interoperante = true;
                    destinatario.Email = entry.Key.ToString();
                    corrispondentiInteroperanti.Add((DocsPaVO.utente.Corrispondente)listDestinatari[i]);
                    destinatario.DatiDestinatari.Add((DocsPaVO.utente.Corrispondente)listDestinatari[i]);
                    //se il corrispondente è interoperabilità passo l'url come indirizzo
                    if ((listDestinatari[i] as DocsPaVO.utente.Corrispondente).canalePref != null &&
                        (listDestinatari[i] as DocsPaVO.utente.Corrispondente).canalePref.typeId.Equals(InteroperabilitaSemplificataManager.InteroperabilityCode) &&
                        (listDestinatari[i] as DocsPaVO.utente.Corrispondente).Url != null &&
                        (listDestinatari[i] as DocsPaVO.utente.Corrispondente).Url.Count > 0)
                    {
                        destinatario.DataUltimaSpedizione = GetDataUltimaSpedizione(infoUtente, documento, (listDestinatari[i] as DocsPaVO.utente.Corrispondente));
                    }
                    else
                    {
                        destinatario.DataUltimaSpedizione = GetDataUltimaSpedizione(infoUtente, documento, listDestinatari[i] as DocsPaVO.utente.Corrispondente);
                    }
                    // Determina se il documento è stato spedito almeno una volta al destinatario
                    bool spedito = !string.IsNullOrEmpty(destinatario.DataUltimaSpedizione);
                    destinatario.IncludiInSpedizione = !spedito;
                    if (spedito)
                    {
                        //MEV 2020: Se è stato spedito ma è presente mancata consegna devo rispedire di nuovo
                        DocsPaDB.Query_DocsPAWS.Interoperabilita interop = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                        DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum stato = interop.GetStatoInvio(infoUtente, documento.systemId, (listDestinatari[i] as DocsPaVO.utente.Corrispondente).systemId);
                        destinatario.StatoSpedizione = GetStatoSpedizione(stato);
                        if (stato == DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.DaRispedire)
                            destinatario.IncludiInSpedizione = true;
                        //destinatario.StatoSpedizione = GetStatoSpedizione(DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.Spedito);                       
                    }
                    else
                        destinatario.StatoSpedizione = GetStatoSpedizione(DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.DaSpedire);
                    destinatario.Id = ((DocsPaVO.utente.Corrispondente)listDestinatari[i]).systemId;

                    //Verifico se è interoperante RGS
                    if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "FE_ENABLE_FLUSSO_AUTOMATICO")) &&
                    DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "FE_ENABLE_FLUSSO_AUTOMATICO").ToString().Equals("1"))
                    {
                        destinatario.InteroperanteRGS = BusinessLogic.FlussoAutomatico.FlussoAutomaticoManager.CheckIsInteroperanteRGS(destinatario.Id);
                    }
                    infoSpedizioni.DestinatariEsterni.Add(destinatario);
                }
            }
            //recupero le caselle di posta associate ai corrispondenti esterni interoperanti 
            AssociaCaselleDestinatariEsterni(infoSpedizioni.DestinatariEsterni);
            ArrayList corrispondenti = new ArrayList();
            corrispondenti.AddRange(protocolloUscita.destinatari);
            corrispondenti.AddRange(protocolloUscita.destinatariConoscenza);

            // Caricamento dei destinatari esterni non interoperanti
            foreach (DocsPaVO.utente.Corrispondente item in corrispondenti)
            {

                if (!IsDestinatarioInterno(item, documento.registro.systemId, infoUtente))
                {
                    // Determina se il corrispondente esterno è già stato inserito tra gli interoperanti
                    if (corrispondentiInteroperanti.Count(e => e.systemId == item.systemId) == 0)
                    {
                        DocsPaVO.Spedizione.DestinatarioEsterno destinatario = new DocsPaVO.Spedizione.DestinatarioEsterno
                        {
                            Id = item.systemId,
                            IncludiInSpedizione = false,
                            Interoperante = false,
                            Email = item.email,
                            StatoSpedizione = GetStatoDestinatarioEsternoNonInteroperante(item) // Reperimento stato di spedizione verso il destinatario classificato come non interoperante
                        };

                        destinatario.DatiDestinatari.Add(item);
                        infoSpedizioni.DestinatariEsterni.Add(destinatario);
                    }
                }
            }

            if (infoSpedizioni.DestinatariEsterni != null)
                infoSpedizioni.DestinatariEsterni = new List<DocsPaVO.Spedizione.DestinatarioEsterno>(infoSpedizioni.DestinatariEsterni.OrderBy(destEst => destEst.StatoSpedizione.Descrizione));
        }

        private static void AssociaCaselleDestinatariEsterni(List<DocsPaVO.Spedizione.DestinatarioEsterno> destinatari)
        {
            foreach (DocsPaVO.Spedizione.DestinatarioEsterno d in destinatari)
            {
                List<DocsPaVO.utente.Corrispondente> listCorr = d.DatiDestinatari;
                foreach (DocsPaVO.utente.Corrispondente c in listCorr)
                {
                    if (c.canalePref != null && (c.canalePref.descrizione.ToUpper().Equals("INTEROPERABILITA") || c.canalePref.descrizione.ToUpper().Equals("MAIL") || c.canalePref.descrizione.ToUpper().Equals("PORTALE")))
                    {
                        c.Emails = BusinessLogic.Utenti.addressBookManager.GetMailCorrispondente(c.systemId);
                    }
                }
            }
            return;
        }
        /// <summary>
        /// Caricamento dei destinatari interni
        /// </summary>
        /// <param name="infoSpedizioni"></param>
        /// <param name="infoUtente"></param>
        /// <param name="documento"></param>
        private static void FetchDestinatariInterni(DocsPaVO.Spedizione.SpedizioneDocumento infoSpedizioni, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento documento)
        {
            DocsPaVO.documento.ProtocolloUscita protocolloUscita = (DocsPaVO.documento.ProtocolloUscita)documento.protocollo;

            ArrayList destinatari = new ArrayList();
            destinatari.AddRange(protocolloUscita.destinatari);
            destinatari.AddRange(protocolloUscita.destinatariConoscenza);

            foreach (DocsPaVO.utente.Corrispondente destinatario in destinatari)
            {
                //if (IsDestinatarioInterno(destinatario))
                if (IsDestinatarioInterno(destinatario, documento.registro.systemId, infoUtente))
                {
                    // Benché interno, determina se il corrispondente non appartenga ad un'altra AOO
                    // verificando se è considerato tra i destinatari esterni
                    if (infoSpedizioni.DestinatariEsterni.Count(e => e.DatiDestinatari.Any(d => d.systemId == destinatario.systemId)) == 0)
                    {
                        DocsPaVO.Spedizione.DestinatarioInterno item = new DocsPaVO.Spedizione.DestinatarioInterno();

                        // Impostazione dell'id del corrispondente come identificativo univoco del destinatario
                        item.Id = destinatario.systemId;
                        item.Email = destinatario.email;
                        item.DatiDestinatario = destinatario;
                        item.DataUltimaSpedizione = GetDataUltimaTrasmissione(infoUtente, documento, destinatario);

                        // Determina se il documento è stato spedito almeno una volta al destinatario
                        bool spedito = !string.IsNullOrEmpty(item.DataUltimaSpedizione);

                        // Nel caso non sia stato spedito, viene forzata l'impostazione di spedizione
                        item.IncludiInSpedizione = !spedito;

                        if (IsDestinatarioOccasionale(destinatario))
                            item.StatoSpedizione = GetStatoErroreInTrasmissione(ERROR_DESTINATARIO_OCCASIONALE);
                        else if (spedito)
                            item.StatoSpedizione = GetStatoTrasmissione(DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.Spedito);
                        else
                            item.StatoSpedizione = GetStatoTrasmissione(DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.DaSpedire);

                        //In funzione dell'abilitazione alle trasmissioni vengono impostate le proprietà
                        if (destinatario.disabledTrasm)
                        {
                            item.DisabledTrasm = destinatario.disabledTrasm;
                            item.IncludiInSpedizione = !destinatario.disabledTrasm;
                            item.StatoSpedizione = GetStatoTrasmissione(DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.DisabilitatoTrasmissioni);
                        }


                        infoSpedizioni.DestinatariInterni.Add(item);
                    }
                }
            }

            if (infoSpedizioni.DestinatariInterni != null)
                infoSpedizioni.DestinatariInterni = new List<DocsPaVO.Spedizione.DestinatarioInterno>(infoSpedizioni.DestinatariInterni.OrderBy(destInt => destInt.StatoSpedizione.Descrizione));
        }

        /// <summary>
        /// Verifica se il destinatario è occasionale
        /// </summary>
        /// <param name="destinatario"></param>
        /// <returns></returns>
        private static bool IsDestinatarioOccasionale(DocsPaVO.utente.Corrispondente destinatario)
        {
            return (destinatario.tipoCorrispondente == "O");
        }

        /// <summary>
        /// Determina se la ragione trasmissione (automatica da amministrazione)
        /// utilizzata per le trasmissioni comporta l'estensione della visibilità
        /// ad utenti e ruoli gerarchicamente superiori
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        private static bool IsRagioneTrasmissioneConEstensioneVisibilita(DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaDB.Query_DocsPAWS.Trasmissione trasm = new DocsPaDB.Query_DocsPAWS.Trasmissione();

            return trasm.GetEstensioneVisibilita(infoUtente.idAmministrazione, "Null");
        }

        /// <summary>
        /// Creazione dell'oggetto StatoSpedizioneDocumento relativo alle trasmissioni per destinatari interni
        /// </summary>
        /// <param name="stato"></param>
        /// <returns></returns>
        private static DocsPaVO.Spedizione.StatoSpedizioneDocumento GetStatoTrasmissione(DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum stato)
        {
            DocsPaVO.Spedizione.StatoSpedizioneDocumento statoSpedizione = new DocsPaVO.Spedizione.StatoSpedizioneDocumento();

            statoSpedizione.Stato = stato;
            if (stato == DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.DaSpedire)
                statoSpedizione.Descrizione = "Da trasmettere";
            else if (stato == DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.Spedito)
                statoSpedizione.Descrizione = "Trasmesso";
            else if (stato == DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.ErroreInSpedizione)
                statoSpedizione.Descrizione = "Errore nella trasmissione";
            else if (stato == DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.DisabilitatoTrasmissioni)
                statoSpedizione.Descrizione = "Non abilitato alla ricezione delle trasmissioni";
            return statoSpedizione;
        }

        /// <summary>
        /// Creazione dell'oggetto StatoSpedizioneDocumento con stato in errore in trasmissione per destinatari interni
        /// </summary>
        /// <param name="errore"></param>
        /// <returns></returns>
        private static DocsPaVO.Spedizione.StatoSpedizioneDocumento GetStatoErroreInTrasmissione(string errore)
        {
            DocsPaVO.Spedizione.StatoSpedizioneDocumento statoSpedizione = GetStatoTrasmissione(DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.ErroreInSpedizione);
            statoSpedizione.Descrizione = errore;
            return statoSpedizione;
        }

        /// <summary>
        /// Creazione dell'oggetto StatoSpedizioneDocumento
        /// </summary>
        /// <param name="stato"></param>
        /// <returns></returns>
        private static DocsPaVO.Spedizione.StatoSpedizioneDocumento GetStatoSpedizione(DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum stato)
        {
            DocsPaVO.Spedizione.StatoSpedizioneDocumento statoSpedizione = new DocsPaVO.Spedizione.StatoSpedizioneDocumento();

            statoSpedizione.Stato = stato;
            if (stato == DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.DaSpedire)
                statoSpedizione.Descrizione = "Da spedire";
            else if (stato == DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.Spedito)
                statoSpedizione.Descrizione = "Spedito";
            else if (stato == DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.ErroreInSpedizione)
                statoSpedizione.Descrizione = "Errore nella spedizione";
            else if (stato == DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.DaRispedire)
                statoSpedizione.Descrizione = "Da rispedire";

            return statoSpedizione;
        }

        /// <summary>
        /// Creazione dell'oggetto StatoSpedizioneDocumento con stato in errore in spedizione
        /// </summary>
        /// <param name="errore"></param>
        /// <returns></returns>
        private static DocsPaVO.Spedizione.StatoSpedizioneDocumento GetStatoErroreInSpedizione(string errore)
        {
            DocsPaVO.Spedizione.StatoSpedizioneDocumento statoSpedizione = GetStatoSpedizione(DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.ErroreInSpedizione);
            statoSpedizione.Descrizione = errore;
            return statoSpedizione;
        }

        /// <summary>
        /// Creazione dell'oggetto StatoSpedizioneDocumento con stato in errore in spedizione per un destinatario non interoperante
        /// </summary>
        /// <param name="destinatario"></param>
        /// <returns></returns>
        private static DocsPaVO.Spedizione.StatoSpedizioneDocumento GetStatoDestinatarioEsternoNonInteroperante(DocsPaVO.utente.Corrispondente destinatario)
        {
            DocsPaVO.Spedizione.StatoSpedizioneDocumento statoSpedizione = GetStatoSpedizione(DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.ErroreInSpedizione);

            if (IsDestinatarioOccasionale(destinatario))
            {
                statoSpedizione.Descrizione = "Impossibile spedire il documento ad un destinatario occasionale";
            }
            else if (string.IsNullOrEmpty(destinatario.email))
            {
                statoSpedizione.Descrizione = "Impossibile spedire il documento al destinatario per mancanza mail";
            }
            else if (destinatario.canalePref == null ||
                        (destinatario.canalePref != null &&
                         destinatario.canalePref.descrizione != "MAIL" &&
                         destinatario.canalePref.descrizione != "INTEROPERABILITA"))
            {
                // La mail non e' canale preferenziale della UO. Destinatario, quindi per noi non interoperante " + i + " scartato
                statoSpedizione.Descrizione = "La mail non è canale preferenziale della UO, destinatario non interoperante";
            }

            return statoSpedizione;
        }

        /// <summary>
        /// Reperimento della data e ora dell'ultima spedizione del documento per interoperabilità con destinatario il corrispondente fornito
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="documento"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        private static string GetDataUltimaSpedizione(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento documento, DocsPaVO.utente.Corrispondente dest)
        {
            // Se il documento è stato spedito per interoperabilità,
            // l'informazione relativa alla data di spedizione è nella DPA_STATO_INVIO,
            // altrimenti l'informazione viene reperita dalla DPA_TRASMISSIONE
            string dataSpedizione = string.Empty;

            // Documento spedito al corrispondente per interoperabilità
            using (DocsPaDB.Query_DocsPAWS.Interoperabilita interopDb = new DocsPaDB.Query_DocsPAWS.Interoperabilita())
            {
                // Reperimento della data di ultima spedizione del documento
                DateTime data = interopDb.GetDataUltimaSpedizioneEffettuata(infoUtente, documento.systemId, dest.systemId);

                if (data == DateTime.MinValue)
                    // Il documento risulta non spedito oppure l'ultima spedizione effettuata per 
                    // non è andata a buon fine
                    dataSpedizione = string.Empty;
                else
                    dataSpedizione = data.ToString("dd/MM/yyyy HH:mm:ss");
            }

            return dataSpedizione;
        }

        /// <summary>
        /// Reperimento della data e ora dell'ultima spedizione del documento con destinatario il corrispondente fornito
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="documento"></param>
        /// <param name="destinatario"></param>
        /// <returns></returns>
        private static string GetDataUltimaTrasmissione(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento documento, DocsPaVO.utente.Corrispondente destinatario)
        {
            string idDestinatario = string.Empty;

            if (destinatario is DocsPaVO.utente.UnitaOrganizzativa)
            {
                // Quando il destinatario di una trasmissione è una UO,
                // la trasmissione in realtà viene effettuata ai ruoli di riferimento dello stesso.
                // Pertanto nella ricerca della data ultima trasmissione, 
                // sarà reperita quella relativa alla trasmissione
                // inoltrata agli utenti del primo ruolo di riferimento trovato nell'UO.
                foreach (DocsPaVO.amministrazione.OrgRuolo ruolo in BusinessLogic.Amministrazione.OrganigrammaManager.GetListRuoliUo(destinatario.systemId))
                {
                    if (ruolo.DiRiferimento == "1")
                    {
                        idDestinatario = ruolo.IDCorrGlobale;
                        break;
                    }
                }
            }
            else
                // Il destinatario è un utente o un ruolo
                idDestinatario = destinatario.systemId;

            if (string.IsNullOrEmpty(idDestinatario))
                return string.Empty;
            // Documento spedito al corrispondente per trasmissione
            using (DocsPaDB.Query_DocsPAWS.Trasmissione trasmissioneDb = new DocsPaDB.Query_DocsPAWS.Trasmissione())
                return trasmissioneDb.GetDataUltimaTrasmissioneEffettuata(infoUtente, "D", documento.systemId, idDestinatario);
        }

        /// <summary>
        /// Determina se il destinatario è presente tra i destinatari principali o in conoscenza per il documento
        /// </summary>
        /// <param name="documento"></param>
        /// <param name="idDestinatario"></param>
        /// <returns></returns>
        private static bool IsDestinatarioPrincipale(DocsPaVO.documento.SchedaDocumento documento, string idDestinatario)
        {
            DocsPaVO.documento.ProtocolloUscita protocolloUscita = (DocsPaVO.documento.ProtocolloUscita)documento.protocollo;

            return ((DocsPaVO.utente.Corrispondente[])protocolloUscita.destinatari.ToArray(typeof(DocsPaVO.utente.Corrispondente))).Count(e => e.systemId == idDestinatario) > 0;
        }

        /// <summary>
        /// Impostazione dello stato di spedizione del documento
        /// </summary>
        /// <param name="infoSpedizioni"></param>
        /// <remarks>
        /// Il documento sarà in stato spedito se è stato inviato almeno una volta ad un destinatario
        /// </remarks>
        private static void SetStatoSpedito(DocsPaVO.Spedizione.SpedizioneDocumento infoSpedizioni)
        {
            // Determina se il documento è stato spedito almeno una volta ad uno o più destinatari
            infoSpedizioni.Spedito =
                (infoSpedizioni.DestinatariEsterni.Count(e => !string.IsNullOrEmpty(e.DataUltimaSpedizione)) +
                infoSpedizioni.DestinatariInterni.Count(e => !string.IsNullOrEmpty(e.DataUltimaSpedizione)) > 0 +
                infoSpedizioni.DestinatariInterni.Count(e => e.DisabledTrasm));
        }

        /// <summary>
        /// Spedizione del documento ad un destinatario interno per trasmissione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="documento"></param>
        /// <param name="destinatario"></param>
        /// <returns></returns>
        //Andrea - aggiunto paramentro infoSpedizioni
        private static void SpedisciPerTrasmissione(
                        DocsPaVO.utente.InfoUtente infoUtente,
                        DocsPaVO.documento.SchedaDocumento documento,
                        DocsPaVO.Spedizione.DestinatarioInterno destinatario,
                        DocsPaVO.Spedizione.SpedizioneDocumento infoSpedizioni)
        {
            try
            {
                if (IsDestinatarioOccasionale(destinatario.DatiDestinatario))
                {
                    destinatario.StatoSpedizione = GetStatoErroreInSpedizione(ERROR_DESTINATARIO_OCCASIONALE);
                }
                else
                {
                    // Il destinatario è interno, cioè ha visibilità sull'AOO sui cui è stato procollato il documento,
                    // pertanto viene effettuata una nuova trasmissione
                    DocsPaVO.trasmissione.Trasmissione trasmissione = new DocsPaVO.trasmissione.Trasmissione();
                    trasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;

                    trasmissione.ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(infoUtente.idCorrGlobali);
                    DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtente(infoUtente.idPeople);
                    trasmissione.infoDocumento = new DocsPaVO.documento.InfoDocumento(documento);
                    trasmissione.utente = utente;

                    // Per reperire la ragione che la nuova trasmissione deve avere, 
                    // è necessario verificare le impostazioni dell'amministrazione relative
                    // alle ragioni trasmissioni in competenza e conoscenza
                    string tipoDestinatario = string.Empty;
                    if (IsDestinatarioPrincipale(documento, destinatario.DatiDestinatario.systemId))
                        tipoDestinatario = "TO";
                    else
                        tipoDestinatario = "CC";

                    DocsPaVO.trasmissione.RagioneTrasmissione ragioneTrasmissione = BusinessLogic.Trasmissioni.RagioniManager.GetRagione(tipoDestinatario, infoUtente.idAmministrazione);

                    if (ragioneTrasmissione.eredita == "1" && documento.eredita == "0")
                        ragioneTrasmissione.eredita = "0";

                    trasmissione = BusinessLogic.Trasmissioni.TrasmManager.addTrasmissioneSingola(trasmissione, destinatario.DatiDestinatario, ragioneTrasmissione, string.Empty, "S");
                    if (infoUtente.delegato != null)
                        trasmissione.delegato = ((DocsPaVO.utente.InfoUtente)(infoUtente.delegato)).idPeople;

                    trasmissione = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(infoUtente.urlWA, trasmissione);

                    // LOG per documento
                    if (trasmissione.infoDocumento != null && !string.IsNullOrEmpty(trasmissione.infoDocumento.idProfile))
                    {
                        foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in trasmissione.trasmissioniSingole)
                        {
                            string method = string.Empty, desc = string.Empty;
                            method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                            if (trasmissione.infoDocumento.segnatura == null)
                                desc = "Trasmesso Documento ID: " + trasmissione.infoDocumento.docNumber.ToString();
                            else
                                desc = "Trasmesso Documento ID: " + trasmissione.infoDocumento.segnatura.ToString();
                            UserLog.UserLog.WriteLog(trasmissione.utente.userId, trasmissione.utente.idPeople,
                                trasmissione.ruolo.idGruppo, trasmissione.utente.idAmministrazione, method,
                                trasmissione.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK,
                                (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "1", single.systemId);
                        }
                    }


                    string dataSpedizione = GetDataUltimaTrasmissione(infoUtente, documento, destinatario.DatiDestinatario);

                    DateTime result;
                    if (DateTime.TryParse(dataSpedizione, out result))
                    {
                        destinatario.DataUltimaSpedizione = dataSpedizione;
                        destinatario.StatoSpedizione = GetStatoTrasmissione(DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.Spedito);
                        destinatario.IncludiInSpedizione = false;
                    }
                    else
                    {
                        destinatario.StatoSpedizione = GetStatoTrasmissione(DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.ErroreInSpedizione);
                        destinatario.IncludiInSpedizione = true;
                    }
                    //Andrea
                    foreach (string s in trasmissione.listaDestinatariNonRaggiungibili)
                    {
                        infoSpedizioni.listaDestinatariNonRaggiungibili.Add(s);
                    }
                    //infoSpedizioni.listaDestinatariNonRaggiungibili = trasmissione.listaDestinatariNonRaggiungibili;
                    //End Andrea
                }
            }
            catch (Exception ex)
            {
                //string errore = string.Format("Errore nella spedizione del documento al destinatario: {0}", ex.Message);
                string errore = string.Format("Errore nella spedizione del documento al destinatario");
                logger.Debug(errore, ex);

                // Errore nell'invio del documento per trasmissione
                destinatario.StatoSpedizione.Stato = DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.ErroreInSpedizione;
                destinatario.StatoSpedizione.Descrizione = errore;
            }

        }

        /// <summary>
        /// Spedizione del documento ad un destinatario esterno per interoperabilità
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="documento"></param>
        /// <param name="registroRfMittente">
        /// Indica il registro o l'RF mittente della spedizione per interoperabilità
        /// </param>
        /// <param name="destinatario"></param>
        private static void SpedisciPerInteroperabilita(
                DocsPaVO.utente.InfoUtente infoUtente,
                DocsPaVO.documento.SchedaDocumento documento,
                DocsPaVO.utente.Registro registroRfMittente,
                DocsPaVO.Spedizione.DestinatarioEsterno destinatario)
        {
            try
            {
                // Spedizione per interoperabilità
                DocsPaVO.Interoperabilita.SendDocumentResponse interopResponse = BusinessLogic.Documenti.ProtocolloUscitaManager.spedisci(
                                documento,
                                registroRfMittente,
                                destinatario.Email,
                                new ArrayList((from c in destinatario.DatiDestinatari select c).ToArray()),
                                infoUtente,
                                true);

                // Reperimento dell'esito della spedizione al destinatario
                DocsPaVO.Interoperabilita.SendDocumentResponse.SendDocumentMailResponse mailResponse;
                // Se in destinatari.DatiDestinatari ci sono corrispondenti con email valorizzata come url,
                // il documento è stato spedito per IS, quindi il corrispondente viene ricercato per
                // url e non per email. Questo metodo andrebbe ripulito insieme al dividiDestinari.
                if (destinatario.DatiDestinatari.Count(d => !String.IsNullOrEmpty(d.email) && Uri.IsWellFormedUriString(d.email, UriKind.Absolute)) > 0)
                    mailResponse = interopResponse.GetDocumentResponseByMail(destinatario.DatiDestinatari[0].email);
                else
                    mailResponse = interopResponse.GetDocumentResponseByMail(destinatario.Email);

                // Mapping del risultato della spedizione dagli oggetti dell'interoperabilità
                // agli oggetti della spedizione unificata
                destinatario.StatoSpedizione.Stato = (mailResponse.SendSucceded ?
                    DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.Spedito :
                    DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.ErroreInSpedizione);

                if (destinatario.StatoSpedizione.Stato != DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.Spedito)
                {
                    destinatario.DataUltimaSpedizione = string.Empty;
                    destinatario.StatoSpedizione = GetStatoErroreInSpedizione(mailResponse.SendErrorMessage);
                    destinatario.IncludiInSpedizione = true;
                }
                else
                {
                    // Reperimento della data dell'ultima spedizione del documento per interoperabilità
                    destinatario.DataUltimaSpedizione = GetDataUltimaSpedizione(infoUtente, documento, destinatario.DatiDestinatari[0]);
                    destinatario.StatoSpedizione = GetStatoSpedizione(DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.Spedito);
                    destinatario.IncludiInSpedizione = false;

                    //Se il destinatario è interoperante RGS, inserisco nel flusso procedurale.
                    if (destinatario.InteroperanteRGS && documento.spedizioneDocumento.tipoMessaggio != null && !string.IsNullOrEmpty(documento.spedizioneDocumento.tipoMessaggio.ID))
                    {
                        string idProcesso = BusinessLogic.FlussoAutomatico.FlussoAutomaticoManager.GetIdProcessoFlusso(documento, documento.spedizioneDocumento.tipoMessaggio);

                         //Nel caso in cui non lo fosse, devo fascicolare il documento nello stesso fascicolo del documento padre
                        if (!documento.spedizioneDocumento.tipoMessaggio.INIZIALE)
                        {
                            //Estraggo il fascicolo in cui fascicolare il predisposto in arrivo
                            DocsPaVO.FlussoAutomatico.Flusso richiestaIniziale = BusinessLogic.FlussoAutomatico.FlussoAutomaticoManager.GetFlussoInizioRichiesta(idProcesso);
                            DocsPaVO.fascicolazione.Fascicolo fascicolo = BusinessLogic.FlussoAutomatico.FlussoAutomaticoManager.GetFasicoloByIdProfile(infoUtente, richiestaIniziale.INFO_DOCUMENTO.ID_PROFILE);
                            if (fascicolo != null)
                            {
                                string msg = string.Empty;
                                try
                                {
                                    if (fascicolo.folderSelezionato != null)
                                    {
                                        BusinessLogic.Fascicoli.FolderManager.addDocFolder(infoUtente, documento.docNumber, fascicolo.folderSelezionato.systemID, false, out msg);
                                    }
                                    else
                                    {
                                        BusinessLogic.Fascicoli.FascicoloManager.addDocFascicolo(infoUtente, documento.docNumber, fascicolo.systemID, true, out msg);
                                    }
                                }
                                catch (Exception e)
                                {
                                    logger.Debug("Non è stato possibile fascicolare il documento: " + msg);
                                }
                            }
                        }

                        DocsPaVO.FlussoAutomatico.InfoDocumentoFlusso infoDoc = new DocsPaVO.FlussoAutomatico.InfoDocumentoFlusso() {  ID_PROFILE = documento.docNumber };
                        DocsPaVO.FlussoAutomatico.Flusso flusso = new DocsPaVO.FlussoAutomatico.Flusso() { ID_PROCESSO = idProcesso, MESSAGGIO = documento.spedizioneDocumento.tipoMessaggio, INFO_DOCUMENTO = infoDoc};
                        BusinessLogic.FlussoAutomatico.FlussoAutomaticoManager.InsertFlussoProcedurale(flusso);
                    }

                }
            }
            catch (Exception ex)
            {
                //string errore = string.Format("Errore nella spedizione del documento al destinatario: {0}", ex.Message);
                string errore = string.Format("Errore nella spedizione del documento al destinatario");
                logger.Debug(errore, ex);

                destinatario.StatoSpedizione.Stato = DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.ErroreInSpedizione;
                destinatario.StatoSpedizione.Descrizione = errore;
            }
        }

        /// <summary>
        /// Verifica se il documento fornito per la spedizione è un protocollo in uscita valido
        /// </summary>
        /// <param name="documento"></param>
        private static void CheckProtocolloUscita(DocsPaVO.documento.SchedaDocumento documento)
        {
            if (string.IsNullOrEmpty(documento.systemId))
                throw new ApplicationException("Documento non salvato");

            DocsPaVO.documento.ProtocolloUscita protocolloUscita = documento.protocollo as DocsPaVO.documento.ProtocolloUscita;

            if (protocolloUscita == null || (protocolloUscita != null && string.IsNullOrEmpty(protocolloUscita.numero)))
                throw new ApplicationException(string.Format("Documento con id {0} non protocollato in uscita", documento.systemId));
        }

        /// <summary>
        /// Metodo per la gestione della spedizione di un documento per interoperabilità semplificata
        /// </summary>
        /// <param name="infoUtente">Informazioni sull'utente che sta effettuando la spedizione</param>
        /// <param name="documento">Documento da spedire</param>
        /// <param name="destinatario">Destinatario cui inviare la spedizione</param>
        private static void SpedisciPerInteroperabilitaSemplificata(
                DocsPaVO.utente.InfoUtente infoUtente,
                DocsPaVO.documento.SchedaDocumento documento,
                IEnumerable<DocsPaVO.Spedizione.DestinatarioEsterno> destinatario)
        {
            try
            {
                // Spedizione per interoperabilità
                DocsPaVO.Interoperabilita.SendDocumentResponse interopResponse = BusinessLogic.Documenti.ProtocolloUscitaManager.SpedisciPerInteroperabilitaSemplificata(
                                documento,
                                infoUtente,
                                destinatario);

                foreach (DocsPaVO.Spedizione.DestinatarioEsterno de in destinatario)
                {
                    foreach (DocsPaVO.utente.Corrispondente c in de.DatiDestinatari)
                    {
                        // Reperimento dell'esito della spedizione al destinatario
                        DocsPaVO.Interoperabilita.SendDocumentResponse.SendDocumentMailResponse mailResponse =
                            interopResponse.GetDocumentResponseByMail(c.email);

                        // Mapping del risultato della spedizione dagli oggetti dell'interoperabilità
                        // agli oggetti della spedizione unificata
                        de.StatoSpedizione.Stato = (mailResponse.SendSucceded ?
                            DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.Spedito :
                            DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.ErroreInSpedizione);

                        if (de.StatoSpedizione.Stato != DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.Spedito)
                        {
                            de.DataUltimaSpedizione = string.Empty;
                            de.StatoSpedizione = GetStatoErroreInSpedizione(mailResponse.SendErrorMessage);
                            de.IncludiInSpedizione = true;
                        }
                        else
                        {
                            // Reperimento della data dell'ultima spedizione del documento per interoperabilità
                            de.DataUltimaSpedizione = GetDataUltimaSpedizione(infoUtente, documento, c);
                            de.StatoSpedizione = GetStatoSpedizione(DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.Spedito);
                            de.IncludiInSpedizione = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //string errore = string.Format("Errore nella spedizione del documento al destinatario: {0}", ex.Message);
                string errore = string.Format("Errore nella spedizione del documento al destinatario");
                logger.Debug(errore, ex);

                foreach (DocsPaVO.Spedizione.DestinatarioEsterno de in destinatario)
                {
                    de.StatoSpedizione.Stato = DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.ErroreInSpedizione;
                    de.StatoSpedizione.Descrizione = errore;
                }

            }
        }

        /// <summary>
        /// PEC 4 - requisito 5 - storico spedizioni
        /// </summary>
        /// <param name="idDocumento"></param>
        /// <returns></returns>
        public static ArrayList GetElementiStoricoSpedizione(string idDocumento)
        {
            DocsPaDB.Query_DocsPAWS.Interoperabilita interop = new DocsPaDB.Query_DocsPAWS.Interoperabilita();

            return interop.GetSendStoricoByIdDocument(idDocumento);
        }



        /// <summary>
        /// Lista Report delle spedizioni
        /// </summary>
        public static List<DocsPaVO.Spedizione.InfoDocumentoSpedito> GetReportSpedizioni(DocsPaVO.Spedizione.FiltriReportSpedizioni filters, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaDB.Query_DocsPAWS.Interoperabilita interop = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
             if (string.IsNullOrEmpty(filters.IdDocumento))
            return interop.GetReportSpedizioni(filters,infoUtente.idPeople, infoUtente.idGruppo);
             return interop.GetReportSpedizioniDocumento(filters);
        }

        /// <summary>
        /// Lista Report delle spedizioni
        /// </summary>
        public static List<DocsPaVO.Spedizione.InfoDocumentoSpedito> GetReportSpedizioniDocumenti(DocsPaVO.Spedizione.FiltriReportSpedizioni filters, List<string> idDocumenti, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaDB.Query_DocsPAWS.Interoperabilita interop = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
            if (idDocumenti == null || idDocumenti.Count == 0)
                return null;
            return interop.GetReportSpedizioniDocumenti(filters, idDocumenti.Distinct().ToList(), infoUtente.idGruppo);
        }


        #endregion
    }
}
