// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Serilog;

namespace BusinessLogic.Trasmissioni;

public class ModelliTrasmissioni
{
    private static ILogger logger = Log.ForContext(typeof(ModelliTrasmissioni));

    public static System.Collections.ArrayList getModelliByDdlAmmPaging(string idAmm, int nPagina, DocsPaVO.filtri.FiltroRicerca[] filtriRicerca, out int numTotPag)
    {
        numTotPag = 0;
        try
        {
            DocsPaDB.Query_DocsPAWS.ModTrasmissioni listModTrasm = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
            return listModTrasm.getModelliByDdlAmmPaging(idAmm, nPagina, filtriRicerca, out numTotPag);
        }
        catch (Exception ex)
        {
            logger.Debug("Errore in Trasmissioni - ModelliTrasmissioni - metodo: getModelliByDdlAmm", ex);
            return null;
        }
    }

    public static string getModelloSystemId()
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.ModTrasmissioni listModTrasm = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
            return listModTrasm.getModelloSystemId();

        }
        catch (Exception ex)
        {
            logger.Debug("Errore in Trasmissioni - ModelliTrasmissioni - metodo: getModelloSystemId", ex);
            return null;
        }
    }

    public static string salvaModello(DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modelloTrasmissione)
    {
        string result = string.Empty;
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaDB.Query_DocsPAWS.ModTrasmissioni modTrasm = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
                result = modTrasm.salvaModello(modelloTrasmissione);
                transactionContext.Complete();
                //if (dbProvider.DBType.ToUpper().Equals("SQL"))
                //{
                //    // metodo per il reperimento del system id del modello appena inserito in sql
                //    int sysId = modTrasm.findModelSystemId();
                //    if (sysId > 0)
                //    {
                //        modelloTrasmissione.SYSTEM_ID = sysId;
                //        modelloTrasmissione.CODICE = "MT_" + sysId.ToString();
                //        modTrasm.insertSalvaModello(modelloTrasmissione);
                //    }
                //    else
                //        modTrasm.deleteModello(modelloTrasmissione);
                //}
            }
            catch (Exception e)
            {
                logger.Debug("Errore in Trasmissioni - ModelliTrasmissioni - metodo: salvaModello", e);
            }
            return result;
        }
    }

    public static void CancellaModello(string idAmm, string idModello)
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.ModTrasmissioni delModTrasm = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
            delModTrasm.CancellaModello(idAmm, idModello);
        }
        catch (Exception ex)
        {
            logger.Debug("Errore in Trasmissioni - ModelliTrasmissioni - metodo: CancellaModello", ex);
        }

    }

    public static System.Collections.ArrayList getModelliByAmmConRicerca(string idAmm, string codiceRicerca, string tipoRicerca)
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.ModTrasmissioni listModTrasm = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
            return listModTrasm.getModelliByAmmConRicerca(idAmm, codiceRicerca, tipoRicerca);

        }
        catch (Exception ex)
        {
            logger.Debug("Errore in Trasmissioni - ModelliTrasmissioni - metodo: getModelliByAmmConRicerca", ex);
            return null;
        }
    }

    public static System.Collections.ArrayList getModelliAssDiagrammi(string idTipo, string idDiagramma, string stato, string idAmm, bool selezionati, string tipo)
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.ModTrasmissioni listModTrasm = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
            return listModTrasm.getModelliAssDiagramma(idTipo, idDiagramma, stato, idAmm, selezionati, tipo);

        }
        catch (Exception ex)
        {
            logger.Debug("Errore in Trasmissioni - ModelliTrasmissioni - metodo: getModelliAssDiagrammi", ex);
            return null;
        }
    }

    public static System.Collections.ArrayList getModelliByAmmLite(string idAmm)
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.ModTrasmissioni listModTrasm = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
            return listModTrasm.getModelliByAmmLite(idAmm);

        }
        catch (Exception ex)
        {
            logger.Debug("Errore in Trasmissioni - ModelliTrasmissioni - metodo: getModelliByAmm", ex);
            return null;
        }
    }

    public static System.Collections.ArrayList getModelliPerTrasmLite(string idAmm, DocsPaVO.utente.Registro[] registri, string idPeople, string idCorrGlobali, string idTipoDoc, string idDiagramma, string idStato, string cha_tipo_oggetto, string system_id, string idRuoloUtente, bool AllReg, string accessrights)
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.ModTrasmissioni modTrasm = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
            return modTrasm.getModelliPerTrasmLite(idAmm, registri, idPeople, idCorrGlobali, idTipoDoc, string.Empty, idDiagramma, idStato, cha_tipo_oggetto, system_id, idRuoloUtente, AllReg, accessrights);
        }
        catch (Exception e)
        {
            logger.Debug("Errore in Trasmissioni - ModelliTrasmissioni - metodo: getModelliPerTrasmLite", e);
            return null;
        }
    }

    public static DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione getModelloByIDSoloConNotifica(string idAmm, string idModello)
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.ModTrasmissioni listModTrasm = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
            return listModTrasm.getModelloByIDSoloConNotifica(idAmm, idModello);

        }
        catch (Exception ex)
        {
            logger.Debug(ex, "Errore in Trasmissioni - ModelliTrasmissioni - metodo: getModelloByID");
            return null;
        }
    }

    public static DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione getModelloByID(string idAmm, string idModello)
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.ModTrasmissioni listModTrasm = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
            return listModTrasm.getModelloByID(idAmm, idModello);

        }
        catch (Exception ex)
        {
            logger.Debug(ex, "Errore in Trasmissioni - ModelliTrasmissioni - metodo: getModelloByID");
            return null;
        }
    }

    /// <summary>
    /// GIUGNO 2008 - Adamo
    /// MODELLI DI TRASMISSIONE:
    /// Gestione della notifica trasmissione degli utenti inserito nei modelli di trasmissione
    /// </summary>
    /// <param name="objTrasm">Oggetto Modelli_Trasmissioni.ModelloTrasmissione</param>
    /// <param name="operazione">Tipo di operazione da effettuare sul db:   'GET' = reperimento dati,   'SET' = modifica dei dati </param>
    /// <returns>L'oggetto stesso passato come parametro. Oggetto NULL se ci sono state eccezioni nel metodo</returns>
    public static DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione UtentiConNotificaTrasm(DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione objModTrasm, System.Collections.ArrayList utentiDaInserire, System.Collections.ArrayList utentiDaCancellare, string operazione)
    {
        DocsPaDB.Query_DocsPAWS.ModTrasmissioni objQuery = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
        return objQuery.UtentiConNotificaTrasm(objModTrasm, utentiDaInserire, utentiDaCancellare, operazione);
    }
    public static DocsPaVO.Modelli_Trasmissioni.FindAndReplaceResponse FindAndReplaceRoleInModelliTrasmissione(DocsPaVO.Modelli_Trasmissioni.FindAndReplaceRequest request)
    {
        DocsPaVO.Modelli_Trasmissioni.FindAndReplaceResponse retVal = new DocsPaVO.Modelli_Trasmissioni.FindAndReplaceResponse();
        try
        {
            using (DocsPaDB.Query_DocsPAWS.ModTrasmissioni modTrasm = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni())
            {
                retVal = modTrasm.FindAndReplaceRuoliInModelliTrasmissione(request);
            }

            return retVal;

        }
        catch (Exception e)
        {
            logger.Debug("Errore durante l'esecuzione dell'operazione di ricerca e sostituzione ruoli nei modelli di trasmissione.");
            throw new ApplicationException("Errore durante l'esecuzione dell'operazione di ricerca e sostituzione ruoli nei modelli di trasmissione.");
        }

    }

    public static bool SalvaCessioneDirittiSuModelliTrasm(DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione objTrasm)
    {
        DocsPaDB.Query_DocsPAWS.ModTrasmissioni objQuery = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
        return objQuery.SalvaCessioneDirittiSuModelliTrasm(objTrasm);
    }
}
