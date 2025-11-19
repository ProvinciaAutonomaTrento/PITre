// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Serilog;
using System.Collections;

namespace BusinessLogic.Documenti;

public class ProtoManager
{
    private static ILogger logger = Log.ForContext(typeof(ProtoManager));

    public delegate void AddFileSegnaturaProtocolloDelegate(DocsPaVO.documento.Allegato allegatoSegnatura, DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.InfoUtente infoUtente);


    private static void CallBack(IAsyncResult result)
    {

        var del = result.AsyncState as AddFileSegnaturaProtocolloDelegate;

        if (del != null)
            del.EndInvoke(result);
    }

    internal static void checkInputData(string idAmministrazione, DocsPaVO.documento.SchedaDocumento schedaDoc)
    {
        #region Codice Commentato
        /*DocsPaDB.Query_DocsPAWS.Documenti obj = new DocsPaDB.Query_DocsPAWS.Documenti();

			try
			{
				logger.Debug("idAmministrazione = " + objSicurezza.idAmministrazione);
			}
			catch (Exception)
			{
				TODO: throw throwException(db, null, "idAmministrazione non trovata");
			}

			if(schedaDoc.registro != null && schedaDoc.protocollo.dataProtocollazione != null)
			{
				try
				{
					queryString = "SELECT  CHA_STATO FROM DPA_EL_REGISTRI WHERE SYSTEM_ID=" + schedaDoc.registro.systemId;
					logger.Debug(queryString);
					string result;
					obj.getStatoRegistri(out result, schedaDoc.registro.systemId);

					if(!result.Equals("A"))
					{
						throw throwException(db, null, "Il registro non è aperto");
					}

				}
				catch (Exception e)
				{
					logger.Debug(e.Message);
					throw throwException(db, null, "Lo stato del registro non è corretto");
				}
			}
			if(schedaDoc.protocollo != null)
			{
				if(schedaDoc.protocollo.dataProtocollazione != null)
				{
					if(DateTime.Now < DocsPaWS.Utils.DateControl.toDate(schedaDoc.protocollo.dataProtocollazione))
					{
						throw throwException(db, null, "La data di protocollazione è successiva a quella attuale");
					}
				}

				if(schedaDoc.registro == null)
				{
					throw throwException(db, null, "Il registro è obbligatorio");
				}

				if(schedaDoc.protocollo.GetType() == typeof(DocsPaVO.documento.ProtocolloEntrata))
				{
					if (((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).mittente == null)
					{
						throw throwException(db, null, "Il mittente è obbligatorio");
					}
				}
				else if(schedaDoc.protocollo.GetType() == typeof(DocsPaVO.documento.ProtocolloUscita))
				{
					if (((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari.Count == 0)
					{
						throw throwException(db, null, "Il destinatario è obbligatorio");
					}
				}
				queryString =
					"SELECT COUNT(*) FROM DPA_EL_REGISTRI WHERE SYSTEM_ID = " +
					schedaDoc.registro.systemId + " AND DTA_ULTIMO_PROTO > " +
					DocsPaWS.Utils.dbControl.toDate(schedaDoc.protocollo.dataProtocollazione,false);
				logger.Debug(queryString);
				string res;
				string date = DocsPaWS.Utils.dbControl.toDate(schedaDoc.protocollo.dataProtocollazione,false);
				obj.getNumRegistri(out res, schedaDoc.registro.systemId, date);

				if (!res.Equals("0"))
				{
					// TODO: throw throwException(db, null, "La data di protocollazione non è valida");
				}
			}

			if (!(schedaDoc.oggetto != null && schedaDoc.oggetto.descrizione != null && !schedaDoc.oggetto.descrizione.Equals("")))
			{
				// TODO: throw throwException(db, null, "L'oggetto è obbligatorio");
			}*/
        #endregion

        DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
        doc.CheckInputData(idAmministrazione, schedaDoc);
    }

    public static DocsPaVO.documento.SchedaDocumento protocolla(DocsPaVO.documento.SchedaDocumento schedaDoc, DocsPaVO.utente.Ruolo objRuolo, DocsPaVO.utente.InfoUtente objSicurezza, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione, bool protocollazioneAutomatica = false)
    {
        logger.Information("BEGIN");
        //controllo se ruolo abilitato sul registro.
        bool associato = false;
        //controllo se il Flag WSPIA è attivo.
        string flagWspia = "1";

        //SOSTITUISCO IL CARATTERE SPECIALE
        if (schedaDoc.oggetto.descrizione.Contains("–"))
            schedaDoc.oggetto.descrizione = schedaDoc.oggetto.descrizione.Replace("–", "-");

        for (int i = 0; i < objRuolo.registri.Count; i++)
        {
            DocsPaVO.utente.Registro reg = (DocsPaVO.utente.Registro)objRuolo.registri[i];
            if (reg.systemId == schedaDoc.registro.systemId)
            {
                associato = true;
                if (reg.FlagWspia == "1")
                    flagWspia = "1";
                else
                    flagWspia = "0";

                break;
            }
        }

        if (!associato)
            throw new Exception("il ruolo " + objRuolo.descrizione + " non è associato al registro" + schedaDoc.registro.descrizione);

        //Verifico se il documento è in libro firma e in caso se il passo in attesa è quello di protocollazione ed il titolare è l'utente che sta effettuando la protocollazione
        #region CHECK_LIBRO_FIRMA
        if (!string.IsNullOrEmpty(schedaDoc.systemId) && (LibroFirma.LibroFirmaManager.IsDocInLibroFirma(schedaDoc.systemId)))
        {
            //Controllo che il documento non sia da repertoriare, non posso protocollare e repertoriare insieme, quindi anche se era prevista la repertoriazione, lancio eccezione
            bool daRepertoriare = false;
            if (schedaDoc.template != null && !string.IsNullOrEmpty(schedaDoc.template.ID_TIPO_ATTO)
                && schedaDoc.template.ELENCO_OGGETTI != null && schedaDoc.template.ELENCO_OGGETTI.Count > 0)
            {
                DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg = (from o in schedaDoc.template.ELENCO_OGGETTI.Cast<DocsPaVO.ProfilazioneDinamica.OggettoCustom>()
                                                                   where o.TIPO.DESCRIZIONE_TIPO.Equals("Contatore") && o.REPERTORIO.Equals("1")
                                                                   && o.CONTATORE_DA_FAR_SCATTARE && string.IsNullOrEmpty(o.VALORE_DATABASE)
                                                                   select o).FirstOrDefault();
                if (ogg != null)
                    daRepertoriare = true;
            }

            if (!LibroFirma.LibroFirmaManager.IsTitolarePassoInAttesa(schedaDoc.systemId, objSicurezza, DocsPaVO.LibroFirma.Azione.RECORD_PREDISPOSED) || daRepertoriare)
            {
                risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.DOCUMENTO_IN_LIBRO_FIRMA_PASSO_NON_ATTESO;
                throw new Exception();
            }
        }
        #endregion

        #region Controlli su Mancanza Mitt/Dest
        //controllo se presente Mitt/Dest dati obligatori.
        if (schedaDoc != null && schedaDoc.protocollo != null)
        {
            if (schedaDoc.tipoProto == "A")
            {
                DocsPaVO.documento.ProtocolloEntrata schEnt = (DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo;
                if (!(schEnt.mittente != null
                    && schEnt.mittente.descrizione != null
                    && !string.IsNullOrEmpty(schEnt.mittente.descrizione.Trim())))
                {
                    risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.MITTENTE_MANCANTE;
                    throw new Exception();
                }
            }
            else if (schedaDoc.tipoProto == "P")
            {
                DocsPaVO.documento.ProtocolloUscita schUsc = (DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo;
                if (!(schUsc.destinatari != null
                    && schUsc.destinatari.Count > 0
                    && schUsc.destinatari[0] != null
                    && !String.IsNullOrEmpty(((DocsPaVO.utente.Corrispondente)schUsc.destinatari[0]).descrizione.Trim())
                    && ((DocsPaVO.utente.Corrispondente)schUsc.destinatari[0]).systemId != "0"
                    ))
                {
                    risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.DESTINATARIO_MANCANTE;
                    throw new Exception();
                }

            }
            else if (schedaDoc.tipoProto == "I")
            {
                DocsPaVO.documento.ProtocolloInterno schInt = (DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo;
                if (!(schInt.mittente != null
                    && schInt.mittente.descrizione != null
                    && !string.IsNullOrEmpty(schInt.mittente.descrizione.Trim())))
                {
                    risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.MITTENTE_MANCANTE;
                    throw new Exception();
                }

                if (!
                    (schInt.destinatari != null
                    && schInt.destinatari.Count > 0
                    && schInt.destinatari[0] != null
                    && ((DocsPaVO.utente.Corrispondente)schInt.destinatari[0]).descrizione != null
                    && !string.IsNullOrEmpty(((DocsPaVO.utente.Corrispondente)schInt.destinatari[0]).descrizione.Trim())
                    && ((DocsPaVO.utente.Corrispondente)schInt.destinatari[0]).systemId != "0"))
                {
                    risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.DESTINATARIO_MANCANTE;
                    throw new Exception();
                }


            }




        }
        #endregion

        // Avvio del contesto transazionale
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            //Import pregressi
            if (!schedaDoc.pregresso)
            {
                schedaDoc = getDataProtocollo(schedaDoc);
            }
            schedaDoc.predisponiProtocollazione = false;
            DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

            if (DocsPaDB.Query_Utils.Utils.getStatoRegistro(schedaDoc.registro) != "G")
            {
                schedaDoc.oraCreazione = obj.SelectDBTime();
            }
            risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.OK;

            //add massimo digregorio carica dati protocollatore in schedaProtocollo
            schedaDoc = getDatiProtocollatore(schedaDoc, objRuolo, objSicurezza);
            //DATI CREATORE
            schedaDoc = getDatiCreatore(schedaDoc, objRuolo, objSicurezza);

            if (!string.IsNullOrEmpty(schedaDoc.systemId))
            {
                if (!doc.CheckProto(schedaDoc))
                {
                    risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.DOCUMENTO_GIA_PROTOCOLLATO;
                    throw new Exception();
                }

                schedaDoc = protocollaDocProntoProtocollazione(objSicurezza, objRuolo, schedaDoc);
            }
            else
            {
                DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(objSicurezza, flagWspia);

                List<DocsPaVO.documento.FileRequest> versions = new List<DocsPaVO.documento.FileRequest>();


                if (schedaDoc.documenti != null && schedaDoc.documenti.Count > 0)
                {
                    versions = new List<DocsPaVO.documento.FileRequest>(
                        (DocsPaVO.documento.FileRequest[])
                            schedaDoc.documenti.ToArray(typeof(DocsPaVO.documento.FileRequest)));

                    // Ordinamento versioni
                    versions.Sort(
                            delegate (DocsPaVO.documento.FileRequest x, DocsPaVO.documento.FileRequest y)
                            {
                                int versionX, versionY;
                                Int32.TryParse(x.version, out versionX);
                                Int32.TryParse(y.version, out versionY);

                                return (versionX.CompareTo(versionY));
                            }
                        );
                }

                DocsPaVO.utente.Ruolo[] ruoliSuperiori;

                if (!documentManager.CreateProtocollo(schedaDoc, objRuolo, out risultatoProtocollazione, out ruoliSuperiori))
                {
                    throw new Exception();
                }
                else
                {
                    // Notifica creazione del protocollo
                    DocsPaDocumentale.Interfaces.IAclEventListener eventsNotification = new DocsPaDocumentale.Documentale.AclEventListener(objSicurezza);

                    eventsNotification.DocumentoCreatoEventHandler(schedaDoc, objRuolo, ruoliSuperiori);

                    // Sincronizzazione repository
                    if (schedaDoc.repositoryContext != null)
                    {
                        SessionRepositoryFileManager fileManager = SessionRepositoryFileManager.GetFileManager(schedaDoc.repositoryContext);

                        // In fase di inserimento di un repository temporaneo,
                        // possono essere creati:
                        // - la prima versione del documento e, qualora sia stato acquisito un file
                        //   e firmato, anche la seconda versione firmata del documento
                        // - la prima versione di n allegati
                        // La prima versione del documento è creata automaticamente con la creazione del documento stesso.
                        // In caso di seconda versione firmata del documento, è necessario procedre alla creazione.
                        foreach (DocsPaVO.documento.FileRequest v in versions)
                        {
                            int version;
                            Int32.TryParse(v.version, out version);

                            DocsPaVO.documento.FileRequest savedVersion = null;

                            if (version > 1)
                            {
                                // Seconda versione firmata del documento,
                                // impostazione dell'id del documento di appartenenza
                                v.docNumber = schedaDoc.docNumber;

                                // Inserimento delle versioni del documento,
                                // acquisite oltre alla versione principale
                                if (!documentManager.AddVersion(v, false))
                                    throw new ApplicationException(string.Format("Errore nella creazione della versione {0} del documento con id {1}", version, schedaDoc.systemId));

                                savedVersion = SessionRepositorySyncronizer.CopyToRepository(fileManager, v);

                                // Inserimento della nuova versione come primo elemento della lista documenti
                                schedaDoc.documenti.Insert(0, savedVersion);
                            }
                            else
                            {
                                // La versione principale del documento è già stata creata al momento dell'inserimento,
                                // pertanto è necessario copiare solamente il file acquisito nel repository
                                savedVersion = SessionRepositorySyncronizer.CopyToRepository(fileManager, (DocsPaVO.documento.FileRequest)schedaDoc.documenti[0]);

                                // Aggiornamento istanza documento principale
                                schedaDoc.documenti[0] = savedVersion;
                            }
                        }

                        if (schedaDoc.allegati != null && schedaDoc.allegati.Count > 0)
                        {
                            // Gli allegati e le rispettive versioni andranno create manualmente
                            foreach (DocsPaVO.documento.Allegato allegato in schedaDoc.allegati)
                            {
                                string oldVersionLabel = allegato.versionLabel;

                                // Impostazione del docnumber del documento principale
                                // cui sarà associato l'allegato
                                allegato.docNumber = schedaDoc.docNumber;

                                if (!documentManager.AddAttachment(allegato, "N"))
                                    throw new ApplicationException(string.Format("Errore nella creazione dell'allegato {0} del documento con id {1}", allegato.position, schedaDoc.systemId));

                                allegato.versionLabel = oldVersionLabel;
                            }

                            DocsPaVO.documento.FileRequest[] allegati = (DocsPaVO.documento.FileRequest[])schedaDoc.allegati.ToArray(typeof(DocsPaVO.documento.Allegato));

                            if (allegati.Length > 0)
                                schedaDoc.allegati = new ArrayList(SessionRepositorySyncronizer.CopyToRepository(fileManager, allegati));
                        }

                        // Se è presente un repository temporaneo, viene effettuato l'inserimento del file nel repository del documentale
                        // Imposta il repository come scaduto
                        fileManager.Delete();

                        schedaDoc.repositoryContext = null;
                    }
                }
            }

            //Richiamo il metodo per il calcolo della atipicità del documento
            DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
            schedaDoc.InfoAtipicita = documentale.CalcolaAtipicita(objSicurezza, schedaDoc.docNumber, DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.DOCUMENTO);

            if (risultatoProtocollazione == DocsPaVO.documento.ResultProtocollazione.OK)
            {
                // Impostazione della transazione come completata
                transactionContext.Complete();

                //2021: Effettuo la fascicolazione del protocollo (spostato da FE a BE)
                if (schedaDoc.fascicolo != null && !string.IsNullOrEmpty(schedaDoc.fascicolo.systemID))
                {
                    try
                    {
                        bool result = true;
                        string msg = string.Empty;
                        if (schedaDoc.fascicolo.folderSelezionato != null)
                        {
                            result = BusinessLogic.Fascicoli.FolderManager.addDocFolder(objSicurezza, schedaDoc.docNumber, schedaDoc.fascicolo.folderSelezionato.systemID, false, out msg);
                        }
                        else
                        {
                            result = BusinessLogic.Fascicoli.FascicoloManager.addDocFascicolo(objSicurezza, schedaDoc.docNumber, schedaDoc.fascicolo.systemID, true, out msg);
                        }
                        if (result)
                            schedaDoc.fascicolato = "1";
                        else
                        {
                            risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.ERRORE_DURANTE_LA_FASCICOLAZIONE;
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Error("Errore durantente la fascicolazione del protocollo " + e.Message);
                        risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.ERRORE_DURANTE_LA_FASCICOLAZIONE;
                    }
                }

            }
        }
        //Allegato 6 AGID 2021: Creo l'allegato contenente il file di segnatura.xml per i protocolli in partenza
        if (risultatoProtocollazione == DocsPaVO.documento.ResultProtocollazione.OK && schedaDoc.tipoProto.Equals("P"))
        {
            string creaAllegatoSegnaturaXml = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_SEGNATURA_PROTO_ALLEGATO6");
            if (!string.IsNullOrEmpty(creaAllegatoSegnaturaXml) && creaAllegatoSegnaturaXml.Equals("1"))
            {
                DocsPaVO.documento.Allegato allegatoSegnatura = CreaAllegatoSegnaturaProtocollo(schedaDoc.docNumber, objSicurezza);
                //Se non è protocollazione automatica, ad esempio da libro firma, procedo in maniera asincrona, altrimenti sincrona
                if (!protocollazioneAutomatica)
                {
                    //Creo la segnatura in modalità asincrona
                    AsyncCallback callback = new AsyncCallback(CallBack);
                    AddFileSegnaturaProtocolloDelegate addFileSegnaturaProtocollo = new AddFileSegnaturaProtocolloDelegate(AddFileSegnaturaProtocollo);
                    addFileSegnaturaProtocollo.BeginInvoke(allegatoSegnatura, schedaDoc, objSicurezza, callback, addFileSegnaturaProtocollo);
                }
                else
                {
                    AddFileSegnaturaProtocollo(allegatoSegnatura, schedaDoc, objSicurezza);
                }
            }
        }
        logger.Information("END");

        return schedaDoc;
    }

    internal static DocsPaVO.documento.SchedaDocumento getDataProtocollo(DocsPaVO.documento.SchedaDocumento schedaDoc)
    {

        string data = schedaDoc.protocollo.dataProtocollazione;

        if (data != null)
        {

            data = data.Trim();

        }



        // utilizzo la data di apertura del registro solo se non è settata una data di apertura



        bool protocollazioneLibera = false;

        if (!string.IsNullOrEmpty(DocsPaVO.Settings.AppSettings.Instance.PROTOCOLLAZIONE_LIBERA) &&

            DocsPaVO.Settings.AppSettings.Instance.PROTOCOLLAZIONE_LIBERA.ToUpper().Equals("TRUE"))

            protocollazioneLibera = bool.Parse(DocsPaVO.Settings.AppSettings.Instance.PROTOCOLLAZIONE_LIBERA.ToLower());


        /* MEV 3765 Gestione selettiva integrazione WSPIA
             * Modifica MCaropreso:
             * Effettua controllo sul flag associato al registro
             */
        bool flagWSPIA = false;
        if (schedaDoc != null && schedaDoc.registro.FlagWspia != null &&
            schedaDoc.registro.FlagWspia.Equals("1")
            ) flagWSPIA = true;
        else
            flagWSPIA = false;

        protocollazioneLibera = protocollazioneLibera && flagWSPIA;


        if (!protocollazioneLibera)
        {

            if (!(data != null && !data.Equals("")))
            {

                if (schedaDoc.registro.dataApertura.IndexOf(" ") > 0)

                    schedaDoc.protocollo.dataProtocollazione = schedaDoc.registro.dataApertura.Substring(0, schedaDoc.registro.dataApertura.IndexOf(" "));

                else

                    schedaDoc.protocollo.dataProtocollazione = schedaDoc.registro.dataApertura;

                data = schedaDoc.protocollo.dataProtocollazione.Trim();

            }
            schedaDoc.protocollo.anno = schedaDoc.registro.dataApertura.Substring(schedaDoc.registro.dataApertura.LastIndexOf("/") + 1);

        }



        //protocollazioneLibera
        else
            if (string.IsNullOrEmpty(schedaDoc.protocollo.anno))
        {
            schedaDoc.protocollo.anno = schedaDoc.registro.dataApertura.Substring(schedaDoc.registro.dataApertura.LastIndexOf("/") + 1);
        }

        return schedaDoc;

    }

    /// <summary>
    /// add massimo digregorio gestione protocollatore
    /// </summary>
    /// <param name="schedaDoc"></param>
    /// <param name="objRuolo"></param>
    /// <param name="objUtente"></param>
    /// <returns></returns>
    internal static DocsPaVO.documento.SchedaDocumento getDatiProtocollatore(DocsPaVO.documento.SchedaDocumento schedaDoc, DocsPaVO.utente.Ruolo objRuolo, DocsPaVO.utente.InfoUtente objUtente)
    {
        if (schedaDoc.protocollatore == null || schedaDoc.protocollatore.utente_idPeople.Equals(String.Empty))
        {
            schedaDoc.protocollatore = new DocsPaVO.documento.Protocollatore(objUtente, objRuolo);
        }
        return schedaDoc;
    }

    internal static DocsPaVO.documento.SchedaDocumento getDatiCreatore(DocsPaVO.documento.SchedaDocumento schedaDoc, DocsPaVO.utente.Ruolo objRuolo, DocsPaVO.utente.InfoUtente objUtente)
    {
        if (schedaDoc.creatoreDocumento == null || schedaDoc.creatoreDocumento.idPeople.Equals(String.Empty))
        {
            schedaDoc.creatoreDocumento = new DocsPaVO.documento.CreatoreDocumento(objUtente, objRuolo);
        }
        return schedaDoc;
    }

    internal static DocsPaVO.documento.SchedaDocumento protocollaDocProntoProtocollazione(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo objRuolo, DocsPaVO.documento.SchedaDocumento schedaDoc)
    {
        string flag = "1";

        for (int i = 0; i < objRuolo.registri.Count; i++)
        {
            DocsPaVO.utente.Registro reg = new DocsPaVO.utente.Registro();
            reg = (DocsPaVO.utente.Registro)objRuolo.registri[i];
            if (schedaDoc.registro.codRegistro == reg.codRegistro)
            {
                flag = reg.FlagWspia;
                break;
            }
            ///MODIFICA AFIORDI

        }

        DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente, flag);

        DocsPaVO.documento.ResultProtocollazione result;
        documentManager.ProtocollaDocumentoPredisposto(schedaDoc, objRuolo, out result);

        // Se il risultato della protocollazione è positivo, il documento è stato ricevuto per
        // inteoperabilità semplificata, viene inviata al mittente la ricevuta di conferma di
        // ricezione
        if (schedaDoc.typeId == BusinessLogic.Interoperabilita.Semplificata.InteroperabilitaSemplificataManager.InteroperabilityCode)
            BusinessLogic.Interoperabilita.Semplificata.SimplifiedInteroperabilityProtoManager.SendDocumentReceivedProofToSender(
                schedaDoc.systemId,
                infoUtente,
                schedaDoc.registro.systemId);

        if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_PORTALE_RICEVUTA_PROTO")) && DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_PORTALE_RICEVUTA_PROTO").Equals("1"))
        {
            if (schedaDoc.tipoProto == "A")
            {
                if (((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).mittente.canalePref != null && ((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).mittente.canalePref.descrizione.ToUpper() == "PORTALE")
                {
                    // Genero la ricevuta di protocollazione e la inserisco come nuovo allegato
                    DocsPaVO.documento.FileDocumento ricevuta = BusinessLogic.Modelli.StampaRicevutaProtocolloPdf.Create(infoUtente, schedaDoc.docNumber);

                    if (ricevuta != null && ricevuta.content != null && ricevuta.content.Length > 0)
                    {
                        ricevuta.name = "Ricevuta di protocollazione.pdf";
                        ricevuta.fullName = ricevuta.name;
                        ricevuta.nomeOriginale = ricevuta.name;
                        ricevuta.contentType = "application/pdf";
                        ricevuta.estensioneFile = "pdf";

                        DocsPaVO.documento.Allegato allRicevuta = new DocsPaVO.documento.Allegato();
                        allRicevuta.docNumber = schedaDoc.docNumber;
                        allRicevuta.descrizione = "Ricevuta di protocollazione";
                        allRicevuta.fileName = ricevuta.nomeOriginale;
                        allRicevuta.version = "0";
                        allRicevuta.numeroPagine = 1;

                        DocsPaVO.documento.Allegato allResult = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, allRicevuta);
                        if (allResult != null)
                        {
                            BusinessLogic.Documenti.FileManager.putFile(allResult, ricevuta, infoUtente);
                        }
                    }

                    // Imposto la data di apertura del procedimento
                    string nomeCampoAvvio = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "BE_PORTALE_CAMPO_AVVIO_PROC");
                    if (string.IsNullOrEmpty(nomeCampoAvvio))
                        nomeCampoAvvio = "Data avvio procedimento";

                    ArrayList listaFasc = BusinessLogic.Fascicoli.FascicoloManager.getFascicoliDaDoc(infoUtente, schedaDoc.docNumber);
                    if (listaFasc != null && listaFasc.Count > 0)
                    {
                        foreach (DocsPaVO.fascicolazione.Fascicolo f in listaFasc)
                        {
                            bool toUpdate = false;
                            f.template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascDettagli(f.systemID);
                            if (f.template != null && f.template.ELENCO_OGGETTI != null)
                            {
                                foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in f.template.ELENCO_OGGETTI)
                                {
                                    if (ogg.DESCRIZIONE.ToUpper() == nomeCampoAvvio.ToUpper())
                                    {
                                        ogg.VALORE_DATABASE = DateTime.Now.ToString("dd/MM/yyyy");
                                        if (f.stato == "C")
                                        {
                                            f.stato = "A";
                                            f.apertura = DateTime.Now.ToString("dd/MM/yyyy");
                                            f.chiusura = string.Empty;
                                            f.chiudeFascicolo = null;
                                            toUpdate = true;
                                        }
                                    }
                                }
                            }
                            if (toUpdate)
                            {
                                BusinessLogic.Fascicoli.FascicoloManager.setFascicolo(infoUtente, f);
                            }
                        }
                    }
                }
            }
        }

        return schedaDoc;
    }

    private static DocsPaVO.documento.Allegato CreaAllegatoSegnaturaProtocollo(string docnumber, DocsPaVO.utente.InfoUtente infoUtente)
    {
        DocsPaVO.documento.Allegato allegatoSegnatura = null;
        try
        {

            allegatoSegnatura = new DocsPaVO.documento.Allegato();
            allegatoSegnatura.docNumber = docnumber;
            allegatoSegnatura.version = "0";

            allegatoSegnatura.descrizione = "allegato segnatura.xml";
            allegatoSegnatura.TypeAttachment = 6;

            DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);
            if (!documentManager.AddAttachment(allegatoSegnatura, "N"))
                throw new Exception("Errore nell'inserimento dell'allegato nel documentale.");

            BusinessLogic.Documenti.AllegatiManager.setFlagAllegati_PEC_IS_EXT(allegatoSegnatura.versionId, allegatoSegnatura.docNumber, "S");
        }
        catch (Exception e)
        {
            logger.Error("Errore durante la creazione dell'allegato di Segnatura: " + e.Message);

            //Inserisco nella Tabella di log di errori di creazione segnatura
            DocsPaDB.Query_DocsPAWS.Documenti db = new DocsPaDB.Query_DocsPAWS.Documenti();
            db.InsertLogErrorSegnaturaProtocollo(docnumber, e.Message);
            allegatoSegnatura = null;
        }

        return allegatoSegnatura;
    }

    private static void AddFileSegnaturaProtocollo(DocsPaVO.documento.Allegato allegatoSegnatura, DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.InfoUtente infoUtente)
    {
        string err = string.Empty;
        try
        {
            DocsPaVO.utente.Corrispondente mittente = null;
            if (((DocsPaVO.documento.ProtocolloUscita)(schedaDocumento.protocollo)).mittente != null)
                mittente = ((DocsPaVO.documento.ProtocolloUscita)(schedaDocumento.protocollo)).mittente;
            else
                mittente = BusinessLogic.Utenti.UserManager.getCorrispondente(infoUtente.idCorrGlobali, false);

            DocsPaVO.documento.ResultSigilloElettronico resultSigillo = DocsPaVO.documento.ResultSigilloElettronico.OK;
            byte[] segnaturaProtocollo = BusinessLogic.Interoperabilita.InteroperabilitaInvioSegnatura.CreaSegnaturaProtocollo(mittente, infoUtente, schedaDocumento.registro, schedaDocumento.registro, schedaDocumento.docNumber, true, out resultSigillo);

            DocsPaVO.documento.FileDocumento fdAll = new DocsPaVO.documento.FileDocumento();
            fdAll.content = segnaturaProtocollo;
            fdAll.length = segnaturaProtocollo.Length;
            fdAll.name = "segnatura.xml";
            DocsPaVO.documento.FileRequest fRAll = (DocsPaVO.documento.FileRequest)allegatoSegnatura;
            fRAll.fileName = "segnatura.xml";
            if (!BusinessLogic.Documenti.FileManager.putFile(ref fRAll, fdAll, infoUtente, out err))
                throw new Exception(err);
        }
        catch (Exception e)
        {
            logger.Error("Errore durante la creazione del file di Segnatura: " + e.Message);

            //Inserisco nella Tabella di log di errori di creazione segnatura
            DocsPaDB.Query_DocsPAWS.Documenti db = new DocsPaDB.Query_DocsPAWS.Documenti();
            db.InsertLogErrorSegnaturaProtocollo(schedaDocumento.docNumber, e.Message);
        }
    }

    public static bool DeleteProtocollo(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDocumento)
    {
        bool success = false;
        using (DocsPaDB.Query_DocsPAWS.Documenti docs = new DocsPaDB.Query_DocsPAWS.Documenti())
        {
            success = docs.DeleteProtocollo(infoUtente, schedaDocumento);
        }
        return success;
    }

    public static System.Collections.ArrayList getListaOggetti(DocsPaVO.documento.QueryOggetto objQueryOggetto)
    {
        #region Codice Commentato
        /*DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
			db.openConnection();

			// Creo l'oggetto che dovrà essere popolato dalla funzione
			System.Collections.ArrayList listaObj = new System.Collections.ArrayList();

			// Query sul database
			string queryString =
				"SELECT SYSTEM_ID, VAR_DESC_OGGETTO FROM DPA_OGGETTARIO " +
				"WHERE CHA_OCCASIONALE='0' ";

			//condizioni sul registro
			for(int i=0;i<objQueryOggetto.idRegistri.Count;i++)
			{
				if(i==0)
					queryString=queryString+" AND (";
				queryString=queryString+"ID_REGISTRO='" + objQueryOggetto.idRegistri[i].ToString() +"' ";
				if(i<objQueryOggetto.idRegistri.Count-1)
					queryString=queryString+" OR ";
				else
					queryString=queryString+" OR ID_REGISTRO IS NULL) ";
			}
			queryString=queryString+" AND ID_AMM='" + objQueryOggetto.idAmministrazione + "'";
			if (objQueryOggetto.queryDescrizione != null && !objQueryOggetto.queryDescrizione.Equals(""))
				queryString = queryString + " AND VAR_DESC_OGGETTO LIKE '%" + objQueryOggetto.queryDescrizione.Replace("'","''") + "%'";

			queryString += " ORDER BY VAR_DESC_OGGETTO";
			logger.Debug("Ricerca oggetti: " + queryString);
			DataSet dataSet = new DataSet();
			db.fillTable(queryString, dataSet, "DPA_OGGETTARIO");

			//creazione della lista oggetti
			foreach(DataRow dataRow in dataSet.Tables["DPA_OGGETTARIO"].Rows) {
				listaObj.Add(new DocsPaVO.documento.Oggetto(dataRow["SYSTEM_ID"].ToString(),dataRow["VAR_DESC_OGGETTO"].ToString()));
			}
			dataSet.Dispose();
			db.closeConnection();*/
        #endregion

        DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
        System.Collections.ArrayList list = doc.GetListaOggetti(objQueryOggetto);
        return list;
    }

}
