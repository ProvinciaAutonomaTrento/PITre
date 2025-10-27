// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Serilog;
using System.Globalization;

namespace BusinessLogic.Interoperabilita;

public class InteroperabilitaControlloRicevute
{
    private static ILogger logger = Log.ForContext(typeof(InteroperabilitaControlloRicevute));

    public static bool processaRicevutaConferma(DocsPaVO.Interoperabilita.RicevutaRitorno ricevuta, out string message)
    {
        message = string.Empty;
        try
        {
            CultureInfo ci = new CultureInfo("it-IT");
            string[] formati = { "dd/MM/yyyy", "yyyy-MM-dd", "DD/MM/YYYY hh:mm:ss", "DD/MM/YYYY hh.mm.ss", "DD/MM/YYYY HH.mm.ss", "DD/MM/YYYY HH:mm:ss" };

            string codiceAmministrazione = ricevuta.codAmm;
            string codiceAOO = ricevuta.codAOO;
            string numeroRegistrazione = ricevuta.numeroRegistrazione;
            DateTime dataRegistrazione = DateTime.ParseExact(ricevuta.dataRegistrazione, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);

            //info sul messaggio
            string codiceAmministrazioneMitt = ricevuta.codAmm_Mitt;
            string codiceAOOMitt = ricevuta.codAOO_Mitt;
            string numeroRegistrazioneMitt = ricevuta.numeroRegistr_Mitt;
            DateTime dataRegistrazioneMitt = DateTime.ParseExact(ricevuta.dataRegistr_Mitt, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);

            //si trova il numero del documento
            logger.Debug("Ricerca id del profilo...");

            string idProf = Interoperabilita.InteroperabilitaUtils.findIdProfile(codiceAOOMitt, numeroRegistrazioneMitt, dataRegistrazioneMitt.Year);
            logger.Debug("idProfile=" + idProf);

            if (idProf == null)
            {
                logger.Debug("Documento mittente non trovato");
                message = "Documento mittente non trovato";
                return false;
            }

            //si esegue l'update della tabella stato invio
            if (codiceAOO != null && !codiceAOO.Equals("") && codiceAmministrazione != null && !codiceAmministrazione.Equals(""))
            {
                logger.Debug("Update della tabella stato invio: idProfile=" + idProf + ", CodiceAOO=" + codiceAOO + ", CodiceAmm=" + codiceAmministrazione + ", data=" + dataRegistrazione.ToString("dd/MM/yyyy"));

                bool res_update = updateStatoInvio(idProf, codiceAOO, codiceAmministrazione, dataRegistrazione.ToString("dd/MM/yyyy"), numeroRegistrazione, dataRegistrazione.Year);
                if (!res_update)
                {
                    logger.Debug("Errore: non e' stato eseguito l'update del profilo");
                    message = "Si è verificato un errore nell'aggiornamento della ricevuta di ritorno";
                    return false;
                }
            }
            else
            {
                logger.Debug("L'update della tabella profile non può essere eseguito: codiceAOO o codiceAmministrazione nullo");
                message = "Si è verificato un errore: dati mancanti ";
                return false;
            }

            return true;
        }
        catch (Exception e)
        {
            logger.Error("Si è verificato un problema nella ricevuta di ritorno. Eccezione: " + e.ToString());
            return false;
        }
    }

    private static bool updateStatoInvio(string idProf, string codiceAOO, string codiceAmministrazione, string data, string numeroRegistrazione, int anno)
    {
        bool result = false;

        try
        {
            #region Codice Commentato
            /*
				string updateString="UPDATE DPA_STATO_INVIO SET ";
				updateString=updateString+"VAR_PROTO_DEST='"+numeroRegistrazione+"/"+codiceAOO+"/"+anno+"',";
            updateString=updateString+"DTA_PROTO_DEST="+DocsPaWS.Utils.dbControl.toDate(data,false);
	
				updateString=updateString+" WHERE ID_PROFILE="+idProf+" AND VAR_CODICE_AOO='"+codiceAOO+"' AND VAR_CODICE_AMM='"+codiceAmministrazione+"'";
				logger.Debug(updateString);
				db.executeNonQuery(updateString);
				*/
            #endregion

            DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
            result = obj.updStatoInvio(idProf, codiceAOO, codiceAmministrazione, data, numeroRegistrazione, anno);
        }
        catch (Exception e)
        {
            logger.Error("Eccezione: " + e.Message);

            result = false;
        }

        return result;
    }
}
