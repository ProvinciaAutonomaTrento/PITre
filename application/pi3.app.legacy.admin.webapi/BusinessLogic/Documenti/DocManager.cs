// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using Serilog;
using System.Collections;

namespace BusinessLogic.Documenti;

public class DocManager
{
    private static ILogger logger = Log.ForContext(typeof(DocManager));


    public static string VerificaDocErrati(string idAmm)
    {
        using (DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti())
            return doc.VerificaDocErrati(idAmm);
    }

    public static DocsPaVO.documento.SchedaDocumento NewSchedaDocumento(DocsPaVO.utente.InfoUtente infoUtente)
    {
        return NewSchedaDocumento(infoUtente, false);
    }

    public static DocsPaVO.documento.SchedaDocumento NewSchedaDocumento(DocsPaVO.utente.InfoUtente infoUtente, bool useSessionRepositoryContext)
    {
        DocsPaVO.documento.SchedaDocumento schedaDocumento = new DocsPaVO.documento.SchedaDocumento();

        schedaDocumento = new DocsPaVO.documento.SchedaDocumento();
        schedaDocumento.systemId = null;
        schedaDocumento.oggetto = new DocsPaVO.documento.Oggetto();
        schedaDocumento.idPeople = infoUtente.idPeople;
        schedaDocumento.userId = infoUtente.userId;

        string docType = DocsPaVO.Settings.AppSettings.Instance.DOC_TYPE;
        if (string.IsNullOrEmpty(docType))
            docType = "LETTERA"; //default
        schedaDocumento.typeId = docType;
        schedaDocumento.appId = "ACROBAT";
        schedaDocumento.privato = "0";  // doc non privato

        // Creazione di un nuovo contesto per gestire il repository temporaneo per il documento
        schedaDocumento.repositoryContext = BusinessLogic.Documenti.SessionRepositoryFileManager.NewRepository(infoUtente, useSessionRepositoryContext);

        // Creazione di un filerequest relativo alla prima versione del documento
        DocsPaVO.documento.Documento documento = new DocsPaVO.documento.Documento();
        documento.repositoryContext = schedaDocumento.repositoryContext;
        documento.applicazione = null;

        documento.autore = infoUtente.userId;
        documento.cartaceo = false;
        documento.daAggiornareFirmatari = false;
        documento.dataInserimento = DateTime.Now.ToString("dd/MM/yyyy");
        documento.descrizione = string.Empty;
        documento.docNumber = string.Empty;
        documento.docServerLoc = string.Empty;
        documento.fileName = string.Empty;
        documento.fileSize = "0";
        documento.fNversionId = string.Empty;
        documento.idPeople = infoUtente.idPeople;
        documento.msgErr = string.Empty;
        documento.path = string.Empty;
        documento.subVersion = "!";
        documento.version = "1";
        documento.versionId = string.Empty;
        documento.versionLabel = "1";
        documento.daInviare = "1";
        documento.dataArchiviazione = null;
        documento.dataArrivo = string.Empty;
        schedaDocumento.documenti = new ArrayList() { documento };

        schedaDocumento.allegati = new ArrayList();

        return schedaDocumento;
    }

    public static DocsPaVO.documento.SchedaDocumento getDettaglioNoSecurityByIDVecchioDoc(DocsPaVO.utente.InfoUtente infoUtente, string id_vecchio_doc)
    {
        DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
        DocsPaVO.documento.SchedaDocumento schedaDoc = new DocsPaVO.documento.SchedaDocumento();
        schedaDoc = doc.GetDettaglioNoSecurityByIDVecchioDoc(infoUtente, id_vecchio_doc, id_vecchio_doc);

        if (schedaDoc == null)
        {
            throw new ApplicationException(string.Format("Documento con ID '{0}' non trovato", id_vecchio_doc));
        }
        else
        {
            DocsPaDocumentale.Documentale.UserManager userManager = new DocsPaDocumentale.Documentale.UserManager();

            // Reperimento del token di autenticazione per il superutente del documentale
            string superUserToken = userManager.GetSuperUserAuthenticationToken();
            string oldToken = infoUtente.dst;
            infoUtente.dst = superUserToken;
            try
            {
                // Reperimento informazioni se il documento è in stato checkout
                schedaDoc.checkOutStatus = BusinessLogic.CheckInOut.CheckInOutServices.GetCheckOutStatus(schedaDoc.systemId, schedaDoc.docNumber, infoUtente);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Ripristino token di autenticazione
                infoUtente.dst = oldToken;

            }
        }

        try
        {
            DocsPaDocumentale.Documentale.DocumentManager docManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);
            DocsPaVO.documento.Documento docPrinc = (schedaDoc.documenti[0] as DocsPaVO.documento.Documento);
            string ext = docManager.GetFileExtension(schedaDoc.docNumber, docPrinc.version);
            DocsPaVO.documento.Applicazione app = new DocsPaVO.documento.Applicazione();
            app.estensione = ext;
            docPrinc.applicazione = app;
            schedaDoc.documenti[0] = docPrinc;

        }
        catch (Exception) { }

        return schedaDoc;
    }

    public static bool isEnabledConversionePdfServer()
    {
        bool retValue;
        bool.TryParse(DocsPaVO.Settings.AppSettings.Instance.CONVERSIONE_PDF_LATO_SERVER, out retValue);
        return retValue;
    }

    public static bool isDocInConversionePdf(string idProfile)
    {
        using (DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti())
            return doc.isDocInConversionePdf(idProfile);
    }

    public static DocsPaVO.documento.SchedaDocumento getDettaglio(DocsPaVO.utente.InfoUtente infoUtente, string idProfile, string docNumber)
    {
        logger.Information("BEGIN");
        DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
        DocsPaVO.documento.SchedaDocumento schedaDoc = new DocsPaVO.documento.SchedaDocumento();
        schedaDoc = doc.GetDettaglio(infoUtente, idProfile, docNumber, true);

        if (schedaDoc == null)
        {
            throw new Exception();
        }
        else
        {
            // Reperimento informazioni se il documento è in stato checkout,
            // solo per i documenti non di tipo stampa registro
            schedaDoc.checkOutStatus = BusinessLogic.CheckInOut.CheckInOutServices.GetCheckOutStatus(schedaDoc.systemId, schedaDoc.docNumber, infoUtente);
        }

        try
        {
            DocsPaDocumentale.Documentale.DocumentManager docManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);
            DocsPaVO.documento.Documento docPrinc = (schedaDoc.documenti[0] as DocsPaVO.documento.Documento);
            string ext = docManager.GetFileExtension(schedaDoc.docNumber, docPrinc.version);
            DocsPaVO.documento.Applicazione app = new DocsPaVO.documento.Applicazione();
            app.estensione = ext;
            docPrinc.applicazione = app;
            schedaDoc.documenti[0] = docPrinc;
        }
        catch (Exception)
        {
        }
        logger.Information("END");
        return schedaDoc;
    }

    public static string checkdocInCestino(string docNumber)
    {
        string rtn = string.Empty;
        using (DocsPaDB.DBProvider db = new DocsPaDB.DBProvider())
        {
            if (docNumber != null && !docNumber.Equals(""))
            {
                string cmd = "select " + DocsPaDbManagement.Functions.Functions.getNVL("cha_in_cestino", "0") + " from profile where docnumber=" + docNumber;
                using (System.Data.IDataReader dr = db.ExecuteReader(cmd))
                {
                    while (dr.Read())
                    {
                        if (!dr.IsDBNull(0))
                        {
                            rtn = dr.GetValue(0).ToString();
                        }

                    }

                }
            }
        }
        return rtn;

    }

    public static string GetTipoDocumento(string docNumber)
    {
        string tipoDocumento;

        DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

        DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_TIPO_DOCUMENTO_FROM_DOCNUMBER");

        queryDef.setParam("docNumber", docNumber);

        string commandText = queryDef.getSQL();
        logger.Debug(commandText);

        if (!dbProvider.ExecuteScalar(out tipoDocumento, commandText))
        {
            logger.Debug("Errore nel reperimento del tipo documento, QUERY " + commandText);
            throw new ApplicationException("Errore nel reperimento del tipo documento");
        }

        return tipoDocumento;
    }

    public static DocsPaVO.documento.SchedaDocumento getDettaglioPerNotificaAllegati(DocsPaVO.utente.InfoUtente infoUtente, string idProfile, string docNumber)
    {
        DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
        DocsPaVO.documento.SchedaDocumento schedaDoc = new DocsPaVO.documento.SchedaDocumento();
        schedaDoc = doc.GetDettaglio(infoUtente, idProfile, docNumber, false);

        if (schedaDoc == null)
        {
            throw new Exception();
        }

        return schedaDoc;
    }

    public static DocsPaVO.documento.SchedaDocumento getDettaglioNoSecurity(DocsPaVO.utente.InfoUtente infoUtente, string docNumber)
    {
        DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
        DocsPaVO.documento.SchedaDocumento schedaDoc = new DocsPaVO.documento.SchedaDocumento();
        schedaDoc = doc.GetDettaglioNoSecurity(infoUtente, docNumber, docNumber);

        if (schedaDoc == null)
        {
            throw new ApplicationException(string.Format("Documento con ID '{0}' non trovato", docNumber));
        }
        else
        {
            DocsPaDocumentale.Documentale.UserManager userManager = new DocsPaDocumentale.Documentale.UserManager();

            // Reperimento del token di autenticazione per il superutente del documentale
            string superUserToken = userManager.GetSuperUserAuthenticationToken();
            string oldToken = infoUtente.dst;
            infoUtente.dst = superUserToken;

            try
            {
                // Reperimento informazioni se il documento è in stato checkout
                schedaDoc.checkOutStatus = BusinessLogic.CheckInOut.CheckInOutServices.GetCheckOutStatus(schedaDoc.systemId, schedaDoc.docNumber, infoUtente);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Ripristino token di autenticazione
                infoUtente.dst = oldToken;
            }
        }

        try
        {
            DocsPaDocumentale.Documentale.DocumentManager docManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);
            DocsPaVO.documento.Documento docPrinc = (schedaDoc.documenti[0] as DocsPaVO.documento.Documento);
            string ext = docManager.GetFileExtension(schedaDoc.docNumber, docPrinc.version);
            DocsPaVO.documento.Applicazione app = new DocsPaVO.documento.Applicazione();
            app.estensione = ext;
            docPrinc.applicazione = app;
            schedaDoc.documenti[0] = docPrinc;
        }
        catch (Exception) { }

        return schedaDoc;
    }

    public static DocsPaVO.documento.Documento[] GetVersionsMainDocument(InfoUtente infoUser, string docNumber)
    {
        using (DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti())
        {
            return doc.GetVersionsMainDoc(infoUser, docNumber).ToArray();
        }
    }

    /// <summary>
    /// ritorna una schedadocumento ma non viene fatto set datavista, altrimenti in Interoperabilita non si ha todolist, e senza i diritti altrimenti non può inviare la mail in caso di cedi diritti
    /// </summary>
    /// 
    public static DocsPaVO.documento.SchedaDocumento getDettaglioPerNotificaAllegatiNoSecurity(DocsPaVO.utente.InfoUtente infoUtente, string idProfile, string docNumber)
    {
        DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
        DocsPaVO.documento.SchedaDocumento schedaDoc = new DocsPaVO.documento.SchedaDocumento();
        schedaDoc = doc.GetDettaglioNoSecurity(infoUtente, idProfile, docNumber);

        if (schedaDoc == null)
        {
            throw new Exception();
        }

        return schedaDoc;
    }

    public static bool SetDataFirmaDocumento(string docnumber, string versionId)
    {
        DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
        return doc.SetDataFirmaDocumento(docnumber, versionId);
    }

    public static string GetSegnaturaRepertorio(string docnumber, string codiceAmm, bool getInHtmlVersion, out String dataAnnullamento)
    {
        using (DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti())
        {
            return doc.GetSegnaturaRepertorio(docnumber, codiceAmm, getInHtmlVersion, out dataAnnullamento);
        }
    }

    public static string ExecRimuoviSchedaMethod(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDoc)
    {
        DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);

        if (documentManager.Remove(new DocsPaVO.documento.InfoDocumento(schedaDoc)))
            return "Del";
        else
            return string.Format("Errore nella cancellazione del documento con docNumber {0}", schedaDoc.docNumber);
    }

    public static ArrayList getTipologiaAtto(string idAmministrazione)
    {
        DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
        return doc.GetTipoAtto(idAmministrazione);
    }

    public static void cambiaDirittiDocumenti(int accessRight, string idDocumento)
    {
        DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
        doc.cambiaDirittiDocumenti(accessRight, idDocumento);
    }

    public static string GetLabelTipoDocumento(string typeId)
    {
        DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
        return doc.GetLabelTipoDocumento(typeId);
    }

    /// <summary>
    /// Verifica se il Documento con idDocumento è fascicolato nel fascicolo con ID_FASCICOLO = idFascicolo
    /// </summary>
    /// <param name="idDocumento">id del documento che si vuole verificare</param>
    /// <param name="idFascicolo">id fascicolo della project</param>
    /// <returns></returns>
    public static bool GetSeDocFascicolato(string idDocumento, string idFascicolo)
    {
        bool result = false;
        DocsPaDB.Query_DocsPAWS.Documenti documentiDb = new DocsPaDB.Query_DocsPAWS.Documenti();
        if (!string.IsNullOrEmpty(idDocumento) && !string.IsNullOrEmpty(idFascicolo))
        {
            result = documentiDb.GetSeDocFascicolato(idDocumento, idFascicolo);
        }
        return result;
    }

    /// <summary>
    /// Reperimento scheda documento a partire dal solo "docNumber".
    /// NB: Viene ignorata la gestione della visibilità. 
    /// </summary>
    /// <param name="idPeople"></param>
    /// <param name="idProfile"></param>
    /// <param name="docNumber"></param>
    /// <returns></returns>
    public static DocsPaVO.documento.SchedaDocumento getDettaglioNoSecurityByNumProtoEIDRegistro(DocsPaVO.utente.InfoUtente infoUtente, string numProto, string idRegistro)
    {
        DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
        DocsPaVO.documento.SchedaDocumento schedaDoc = new DocsPaVO.documento.SchedaDocumento();

        schedaDoc = doc.GetDettaglioNoSecurityByNumProtoEIDRegistro(infoUtente, numProto, idRegistro);

        if (schedaDoc == null || string.IsNullOrEmpty(schedaDoc.systemId))
        {
            schedaDoc = null;
        }
        else
        {
            DocsPaDocumentale.Documentale.UserManager userManager = new DocsPaDocumentale.Documentale.UserManager();

            // Reperimento del token di autenticazione per il superutente del documentale
            string superUserToken = userManager.GetSuperUserAuthenticationToken();
            string oldToken = infoUtente.dst;
            infoUtente.dst = superUserToken;

            try
            {
                // Reperimento informazioni se il documento è in stato checkout
                schedaDoc.checkOutStatus = BusinessLogic.CheckInOut.CheckInOutServices.GetCheckOutStatus(schedaDoc.systemId, schedaDoc.docNumber, infoUtente);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Ripristino token di autenticazione
                infoUtente.dst = oldToken;
            }

            DocsPaDocumentale.Documentale.DocumentManager docManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);
            DocsPaVO.documento.Documento docPrinc = (schedaDoc.documenti[0] as DocsPaVO.documento.Documento);
            string ext = docManager.GetFileExtension(schedaDoc.docNumber, docPrinc.version);
            DocsPaVO.documento.Applicazione app = new DocsPaVO.documento.Applicazione();
            app.estensione = ext;
            docPrinc.applicazione = app;
            schedaDoc.documenti[0] = docPrinc;

        }



        return schedaDoc;
    }

    public static DocsPaVO.documento.InfoDocumento GetInfoDocumento(DocsPaVO.utente.InfoUtente infoUtente, string idProfile, string docNumber, bool fetchCorrispondenti)
    {
        DocsPaDB.Query_DocsPAWS.Documenti documentiDb = new DocsPaDB.Query_DocsPAWS.Documenti();

        return documentiDb.GetInfoDocumento(infoUtente.idGruppo, infoUtente.idPeople, idProfile, fetchCorrispondenti);
    }

    public static int VerificaACL(string tipoObj, string idObj, DocsPaVO.utente.InfoUtente infoUtente, out string errorMessage)
    {
        int result = -1;
        DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
        result = doc.VerificaACL(tipoObj, idObj, infoUtente.idPeople, infoUtente.idGruppo, out errorMessage);
        return result;
    }

    public static DocsPaVO.documento.InfoDocumento getInfoDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento)
    {
        DocsPaVO.documento.InfoDocumento infoDoc = new DocsPaVO.documento.InfoDocumento();
        infoDoc.idProfile = schedaDocumento.systemId;
        infoDoc.oggetto = schedaDocumento.oggetto.descrizione;
        infoDoc.docNumber = schedaDocumento.docNumber;
        infoDoc.tipoProto = schedaDocumento.tipoProto;
        infoDoc.evidenza = schedaDocumento.evidenza;

        if (schedaDocumento.registro != null)
        {
            infoDoc.codRegistro = schedaDocumento.registro.codRegistro;
            infoDoc.idRegistro = schedaDocumento.registro.systemId;
        }

        if (schedaDocumento.protocollo != null)
        {
            infoDoc.numProt = schedaDocumento.protocollo.numero;
            infoDoc.daProtocollare = schedaDocumento.protocollo.daProtocollare;
            infoDoc.dataApertura = schedaDocumento.protocollo.dataProtocollazione;
            infoDoc.segnatura = schedaDocumento.protocollo.segnatura;

            if (schedaDocumento.protocollo.GetType().Equals(typeof(DocsPaVO.documento.ProtocolloEntrata)))
            {
                infoDoc.mittDest = new System.Collections.ArrayList();
                DocsPaVO.documento.ProtocolloEntrata pe = (DocsPaVO.documento.ProtocolloEntrata)schedaDocumento.protocollo;
                infoDoc.mittDest.Add(pe.mittente.descrizione);
            }
            else if (schedaDocumento.protocollo.GetType().Equals(typeof(DocsPaVO.documento.ProtocolloUscita)))
            {
                DocsPaVO.documento.ProtocolloUscita pu = (DocsPaVO.documento.ProtocolloUscita)schedaDocumento.protocollo;
                infoDoc.mittDest = new System.Collections.ArrayList();
                for (int i = 0; i < pu.destinatari.Count; i++)
                    infoDoc.mittDest.Add(((DocsPaVO.utente.Corrispondente)pu.destinatari[i]).descrizione);
            }

        }
        else
        {
            infoDoc.dataApertura = schedaDocumento.dataCreazione;
        }

        return infoDoc;
    }

    public static void delDocRichiestaConversionePdf(string idProfile)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                doc.delDocRichiestaConversionePdf(idProfile);
                transactionContext.Complete();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in Documenti  - metodo: delDocRichiestaConversionePdf", e);
            }
        }
    }

}
