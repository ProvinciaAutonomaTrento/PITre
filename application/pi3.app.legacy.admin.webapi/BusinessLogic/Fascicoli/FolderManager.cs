// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Serilog;
using System.Collections;

namespace BusinessLogic.Fascicoli;

public class FolderManager
{
    private static ILogger logger = Log.ForContext(typeof(FolderManager));

    /// <summary>
    /// </summary>
    /// <param name="objFascicolo"></param>
    /// <param name="infoUtente"></param>
    /// <param name="debug"></param>
    /// <returns></returns>
    public static DocsPaVO.fascicolazione.Folder getFolder(string idPeople, string idGruppo, DocsPaVO.fascicolazione.Fascicolo objFascicolo)
    {
        //return getFolder(objFascicolo.systemID, infoUtente);
        DocsPaVO.fascicolazione.Folder folderObject = new DocsPaVO.fascicolazione.Folder();
        DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

        folderObject = fascicoli.GetFolder(idPeople, idGruppo, objFascicolo.systemID);

        if (folderObject == null)
        {
            logger.Debug("Errore nella gestione dei fascicoli. (getFolder)");
            throw new Exception("F_System");
        }

        fascicoli.Dispose();

        return folderObject;
    }

    public static ArrayList getIdFolderDoc(string idFascicolo, ArrayList regs)
    {

        //GetIdFolderDoc
        DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
        ArrayList lista = new ArrayList();

        lista = fascicoli.GetIdFolderDoc(idFascicolo, regs);

        if (lista == null)
        {
            logger.Debug("Errore nella gestione dei fascicoli. (getIdFolder)");
            throw new Exception();
        }

        fascicoli.Dispose();

        return lista;
    }

    /// <summary>
    /// Creazione di un array di oggetti "Folder" che
    /// contengono il documento fornito.
    /// </summary>
    public static ArrayList GetFoldersDocument(string systemIdDocument)
    {
        DocsPaDB.Query_DocsPAWS.Fascicoli queryFasc = new DocsPaDB.Query_DocsPAWS.Fascicoli();
        return queryFasc.GetFoldersDocument(systemIdDocument);
    }

    /// <summary>
    /// add doc to fascicolo
    /// </summary>
    /// <param name="idPeople"></param>
    /// <param name="idGruppo"></param>
    /// <param name="idProfile"></param>
    /// <param name="idFolder"></param>
    public static bool addDocFolder(DocsPaVO.utente.InfoUtente infoUtente, string idProfile, string idFolder, bool fascRapida, out string msg)
    {
        logger.Information("BEGIN");
        bool result = true;
        msg = string.Empty;

        // Creazione contesto transazionale
        //QUI
        //DocsPaVO.Validations.ValidationResultInfo result = null;
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            //Verifico se il documento è in libro firma e se, nel passo in attesa, è prevista la fascicolazione del documento
            #region CHECK_LIBRO_FIRMA
            /*if (LibroFirma.LibroFirmaManager.IsDocInLibroFirma(idProfile))
            {
                if (!LibroFirma.LibroFirmaManager.IsTitolarePassoInAttesa(idProfile, infoUtente, DocsPaVO.LibroFirma.Azione.DOC_ADD_INFASC))
                {
                    msg = DocsPaVO.documento.ResultFascicolazione.DOCUMENTO_IN_LIBRO_FIRMA_PASSO_NON_ATTESO.ToString();
                    logger.Debug("Non è possibile procedere con la fascicolazione poichè il documento è in Libro Firma");
                    return false;
                }
            }
            */
            #endregion
            if (BusinessLogic.CheckInOut.CheckInOutServices.IsCheckedOut(idProfile, idProfile, infoUtente))
            {
                msg = string.Format("Il documento con ID {0} risulta bloccato e non può essere inserito nel folder con ID {1}", idProfile, idFolder);
                result = false;
            }
            else
            {
                // Verifica se il titolario in cui si cerca di inserire il documento è chiuso o meno
                if (canAddDocFolder(infoUtente, idProfile, idFolder, out msg))
                {
                    DocsPaDocumentale.Documentale.ProjectManager projectManager = new DocsPaDocumentale.Documentale.ProjectManager(infoUtente);
                    result = projectManager.AddDocumentInFolder(idProfile, idFolder);

                    if (!result)
                        throw new ApplicationException(string.Format("Errore nell'inserimento del documento con id {0} nel folder con id {1}", idProfile, idFolder));

                    //AS400:
                    AS400.AS400.setAs400InFolder(idProfile, infoUtente, idFolder, DocsPaAS400.Constants.CREATE_MODIFY_OPERATION);

                    //Richiamo il metodo per il calcolo della atipicità del documento
                    DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
                    documentale.CalcolaAtipicita(infoUtente, idProfile, DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.DOCUMENTO);

                    //if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_PORTALE_NOTIFICHE")) && DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_PORTALE_NOTIFICHE") == "1")
                    //{
                    //    // E' attiva la gestione delle notifiche di visualizzazione del portale
                    //    if (Procedimenti.ProcedimentiManager.IsFolderInProceeding(idFolder))
                    //    {
                    //        DocsPaVO.Procedimento.Procedimento procedimento = Procedimenti.ProcedimentiManager.GetProcedimentoByIdFolder(idFolder);
                    //        Procedimenti.ProcedimentiManager.InsertDoc(procedimento.Id, idProfile, procedimento.Autore, false, procedimento.IdEsterno, false);
                    //    }                           
                    //}

                    transactionContext.Complete();
                }
                else
                {
                    logger.Debug(string.Format("Errore nell'inserimento del documento con ID '{0}' nel folder con ID '{1}'", idProfile, idFolder));
                    throw new ApplicationException(msg);
                }
            }
            transactionContext.Complete();
            logger.Information("END");
            return result;
        }

    }

    /// <summary>
    /// Verifica se un documento può essere inserito in un folder (record di tipo "C")
    /// </summary>
    /// <param name="infoUtente"></param>
    /// <param name="idProfile"></param>
    /// <param name="idFolder"></param>
    /// <param name="errorMssage">
    /// Errore di validazione riscontrato
    /// </param>
    /// <returns></returns>
    public static bool canAddDocFolder(DocsPaVO.utente.InfoUtente infoUtente, string idProfile, string idFolder, out string errorMssage)
    {
        bool retValue = false;
        errorMssage = string.Empty;

        using (DocsPaDB.Query_DocsPAWS.Fascicoli dbFascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli())
        {
            if (!dbFascicoli.IsDocumentoClassificatoInFolder(idProfile, idFolder))
            {
                if (dbFascicoli.isFascicoloGenerale(infoUtente, idFolder))
                {
                    // Classificazione su fascicolo generale
                    if (dbFascicoli.isTitolarioChiuso(infoUtente, idFolder))
                    {
                        // Il titolario è chiuso, impossibile classificare
                        retValue = false;
                        errorMssage = "Impossibile classificare il documento su un titolo non aperto";
                    }
                    else
                        retValue = true;
                }
                else
                {
                    // Fascicolazione documento su procedimentale

                    // Verifica che lo stato del fascicolo non sia chiuso
                    if (!dbFascicoli.isFascicoloProcedimentaleAperto(infoUtente, idFolder))
                    {
                        retValue = false;
                        errorMssage = "Impossibile fascicolare il documento su un procedimentale chiuso";
                    }
                    else
                        retValue = true;
                }
            }
            else
            {
                retValue = false;
                errorMssage = "Il documento risulta già classificato nel fascicolo richiesto";
            }
        }

        return retValue;
    }

    public static DocsPaVO.fascicolazione.Folder getFolder(string idPeople, string idGruppo, string idFascicolo)
    {
        #region Codice Commentato
        //			logger.Debug("getFolder");
        //			DocsPaWS.Utils.Database database = DocsPaWS.Utils.dbControl.getDatabase();			
        //			DocsPaVO.fascicolazione.Folder folderObject= new DocsPaVO.fascicolazione.Folder();
        //			try {
        //				database.openConnection();
        //				DataSet dataSet= new DataSet();
        //				
        //				string commandString1=
        //					" SELECT DISTINCT A.* FROM PROJECT A, SECURITY B " +
        //					" WHERE A.SYSTEM_ID=B.THING AND A.ID_FASCICOLO=" + idFascicolo +
        //					" AND (B.PERSONORGROUP=" + infoUtente.idPeople + " OR B.PERSONORGROUP=" + infoUtente.idGruppo + ") AND B.ACCESSRIGHTS > 0";
        //					
        //				logger.Debug(commandString1);
        //				
        //				database.fillTable(commandString1,dataSet,"FOLDER");
        //
        //				DataRow[] folderRootRows = dataSet.Tables["FOLDER"].Select("ID_PARENT=" + idFascicolo);
        //				if (folderRootRows.Length > 0) 
        //					folderObject = getFolderData(folderRootRows[0],dataSet.Tables["FOLDER"]);
        //				
        //			} catch (Exception e) {
        //				logger.Debug (e.Message);				
        //				
        //				database.closeConnection();
        //				throw new Exception("F_System");
        //			}
        //			return folderObject;
        #endregion

        DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
        DocsPaVO.fascicolazione.Folder folderObject = new DocsPaVO.fascicolazione.Folder();
        folderObject = fascicoli.GetFolder(idPeople, idGruppo, idFascicolo);
        fascicoli.Dispose();

        return folderObject;
    }

    /// <summary>
    /// Creazione di un array di oggetti "Folder" che
    /// contengono il documento fornito.
    /// </summary>
    public static ArrayList GetFoldersDocument(string systemIdDocument, string systemIdFascicolo)
    {
        DocsPaDB.Query_DocsPAWS.Fascicoli queryFasc = new DocsPaDB.Query_DocsPAWS.Fascicoli();
        return queryFasc.GetFoldersDocument(systemIdDocument, systemIdFascicolo);
    }

}
