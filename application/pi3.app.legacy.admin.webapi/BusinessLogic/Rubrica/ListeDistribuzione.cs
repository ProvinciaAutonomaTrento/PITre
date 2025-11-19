// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using BusinessLogic.Interoperabilita;
using Serilog;
using System.Data;

namespace BusinessLogic.Rubrica;

public class ListeDistribuzione
{
    private static ILogger logger = Log.ForContext(typeof(InteroperabilitaInvioSegnatura));

    public static DataSet getListe(string idAmm)
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
            return listeDistr.getListe(idAmm);

        }
        catch (Exception e)
        {
            logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: getListe", e);
            return null;
        }
    }

    public static string getCodiceLista(string idLista)
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
            return listeDistr.getCodiceLista(idLista);
        }
        catch (Exception e)
        {
            logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: getCorrispondentiByCodLista", e);
            return null;
        }
    }

    public static DataSet getCorrispondentiLista(string codiceLista)
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
            return listeDistr.getCorrispondentiLista(codiceLista);

        }
        catch (Exception e)
        {
            logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: getCorrispondentiLista", e);
            return null;
        }
    }

    public static void deleteLista(string codiceLista)
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
            listeDistr.deleteLista(codiceLista);

        }
        catch (Exception e)
        {
            logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: deleteLista", e);
        }
    }

    public static string getNomeLista(string codiceLista, string idAmm)
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
            return listeDistr.getNomeLista(codiceLista, idAmm);

        }
        catch (Exception e)
        {
            logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: getNomeLista", e);
            return null;
        }
    }

    public static bool isUniqueNomeLista(string nomeLista, string idAmm)
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
            return listeDistr.isUniqueNomeLista(nomeLista, idAmm);
        }
        catch (Exception e)
        {
            logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: isUniqueNomeLista", e);
            return false;
        }
    }

    public static void modificaLista(DataSet dsCorrLista, string idLista, string nomeLista, string codiceLista)
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
            listeDistr.modificaLista(dsCorrLista, idLista, nomeLista, codiceLista);

        }
        catch (Exception e)
        {
            logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: modificaLista", e);
        }
    }

    public static bool isUniqueCod(string codLista, string idAmm)
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
            return listeDistr.isUniqueCod(codLista, idAmm);
        }
        catch (Exception e)
        {
            logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: isUniqueCodLista", e);
            return false;
        }
    }

    public static void salvaLista(DataSet dsCorrLista, string nomeLista, string codiceLista, string idUtente, string idAmm)
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
            listeDistr.salvaLista(dsCorrLista, nomeLista, codiceLista, idUtente, idAmm);

        }
        catch (Exception e)
        {
            logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: salvaLista", e);
        }
    }

    public static DataSet isCorrInListaDistr(string idCorr)
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
            return listeDistr.isCorrInListaDistr(idCorr);

        }
        catch (Exception e)
        {
            logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: isCorrInListaDistr", e);
            return null;
        }
    }

}
