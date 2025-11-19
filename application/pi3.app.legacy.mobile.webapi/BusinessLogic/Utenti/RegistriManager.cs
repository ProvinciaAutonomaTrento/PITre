// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.utente;
using Serilog;
using System.Collections;
using System.Data;
using System.Globalization;

namespace BusinessLogic.Utenti;

public class RegistriManager
{
    private static ILogger logger = Log.ForContext(typeof(RegistriManager));

    public static DocsPaVO.utente.Registro getRegistroByCodAOO(string codAOO, string idAmministrazione)
    {
        DocsPaVO.utente.Registro reg = null;
        if (!(codAOO != null && !codAOO.Equals("")))
        {
            return reg;
        }
        DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
        utenti.GetRegistroByCodAOO(codAOO, idAmministrazione, ref reg);


        return reg;
    }

    public static DocsPaVO.utente.Registro getRegistroByCode(string codiceRegistro)
    {
        DocsPaVO.utente.Registro reg = null;

        if (!string.IsNullOrEmpty(codiceRegistro))
        {
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            utenti.GetRegistroByCodice(codiceRegistro, ref reg);
        }

        return reg;
    }

    public static ArrayList getRegistriRuolo(string idRuolo)
    {
        DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

        return utenti.GetRegistriRuolo(idRuolo);
    }

    public static DocsPaVO.utente.Registro getRegistro(string idRegistro)
    {
        DocsPaVO.utente.Registro reg = null;

        if (!(idRegistro != null && !idRegistro.Equals("")))
        {
            return reg;
        }

        DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
        utenti.GetRegistro(idRegistro, ref reg);

        return reg;
    }

    public static bool existRf(string idAmministazione)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                bool result = amm.existRf(idAmministazione);
                transactionContext.Complete();
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in RegistriManager  - metodo: existRf", e);
                return false;
            }
        }
    }

    public static ArrayList getListaRegistriRfRuolo(string idRuolo, string all, string idAooColl)
    {
        DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

        return utenti.GetListaRegistriRfRuolo(idRuolo, all, idAooColl);

    }

    /// <summary>
    /// Ritorna uno dei tre possibili stati del registro 
    /// (V - aperto, R - chiuso, G - giallo)
    /// </summary>
    /// <param name="registro"></param>
    /// <returns></returns>
    public static string getStatoRegistro(DocsPaVO.utente.Registro registro)
    {
        // R = Rosso -  CHIUSO
        // V = Verde -  APERTO
        // G = Giallo - APERTO IN GIALLO

        string dataApertura = registro.dataApertura;

        if (!dataApertura.Equals(""))
        {

            DateTime dt_cor = DateTime.Now;

            CultureInfo ci = new CultureInfo("it-IT");

            string[] formati = { "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy H.mm.ss", "dd/MM/yyyy" };

            DateTime d_ap = DateTime.ParseExact(dataApertura, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
            //aggiungo un giorno per fare il confronto con now (che comprende anche minuti e secondi)
            d_ap = d_ap.AddDays(1);

            string mydate = dt_cor.ToString(ci);

            //DateTime dt = DateTime.ParseExact(mydate,formati,ci.DateTimeFormat,DateTimeStyles.AllowWhiteSpaces);


            if (registro.stato.Equals("A"))
            {
                if (dt_cor.CompareTo(d_ap) > 0)
                {
                    //data odierna maggiore della data di apertura del registro
                    return "G";
                }
                else
                    return "V";
            }
        }
        return "R";

    }

    /// <summary>
    /// Metodo per il reperimento dei dettaglio di un Raggruppamento Funzionale. 
    /// </summary>
    /// <param name="idRf">Id dell'RF da caricare</param>
    /// <returns>Dettaglio del raggruppamento funzionale</returns>
    public static RaggruppamentoFunzionale GetRaggruppamentoFunzionaleRC(String idRf)
    {
        RaggruppamentoFunzionale rf = new RaggruppamentoFunzionale();

        using (DocsPaDB.Query_DocsPAWS.RF rfDb = new DocsPaDB.Query_DocsPAWS.RF())
        {
            rf = rfDb.GetRaggruppamentoFunzionaleRC(idRf);
        }

        return rf;

    }

    public static bool enabledRF(string idAmministazione)
    {
        if (DocsPaVO.Settings.AppSettings.Instance.ENABLE_RF != null &&
         DocsPaVO.Settings.AppSettings.Instance.ENABLE_RF != "0")
            return true;
        else return false;

    }

    public static string getIdRegistro(string codiceAmm, string codiceReg)
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.Model model = new DocsPaDB.Query_DocsPAWS.Model();
            string idAmm = model.getIdAmmByCod(codiceAmm);
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAElRegistri");
            q.setParam("param1", " SYSTEM_ID ");
            q.setParam("param2", " ID_AMM = " + idAmm + " AND upper(VAR_CODICE) = '" + codiceReg.ToUpper() + "'");
            string sql = q.getSQL();
            logger.Debug(sql);
            DataSet ds = new DataSet();
            dbProvider.ExecuteQuery(ds, sql);
            if (ds.Tables[0].Rows.Count != 0)
                return ds.Tables[0].Rows[0]["SYSTEM_ID"].ToString();
            else
                return string.Empty;

        }
        catch (Exception ex)
        {
            logger.Debug("Errore durante getIdRegistro.", ex);
            return string.Empty;
        }
    }

    public static bool SaveRaggruppamentoFunzionaleRC(RaggruppamentoFunzionale rf, String idRf)
    {
        bool retVal = false;

        using (DocsPaDB.Query_DocsPAWS.RF rfDb = new DocsPaDB.Query_DocsPAWS.RF())
        {
            retVal = rfDb.SaveRaggruppamentoFunzionaleCorrGlobali(rf, idRf);
        }


        return retVal;
    }

    public static Hashtable GetRegistriByRuolo(string id_amm)
    {
        DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
        return amm.GetRegistriByRuolo(id_amm);
    }

}
