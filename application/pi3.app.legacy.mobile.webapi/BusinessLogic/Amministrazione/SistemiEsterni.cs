// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Serilog;
using System.Collections;

namespace BusinessLogic.Amministrazione
{
    public class SistemiEsterni
    {
        private static ILogger logger = Serilog.Log.ForContext(typeof(SistemiEsterni));

        public static ArrayList getSistemiEsterni(string idAmm)
        {
            ArrayList retval = null;
            DocsPaDB.Query_DocsPAWS.Amministrazione DBAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

            retval = DBAmm.getSistemiEsterni(idAmm);

            return retval;
        }

        public static string ctrlInserimentoSistemaEsterno(string idAmm, string codUtente, string codRuolo)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione DBamm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return DBamm.ctrlInserimentoSistemaEsterno(idAmm, codUtente, codRuolo);
        }

        public static DocsPaVO.utente.UnitaOrganizzativa getHubSistemaEsterno(string codice, string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione DBamm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DocsPaVO.utente.UnitaOrganizzativa retval = DBamm.getUOByCodAndIdAmm(codice, idAmm);
            if (retval != null && string.IsNullOrEmpty(retval.systemId))
                retval = null;
            return retval;
        }

        public static bool setVisibilityHubSysExt(string idHub)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione DBamm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return DBamm.setVisibilityHubSysExt(idHub);
        }

        public static DocsPaVO.utente.TipoRuolo getTipoRuoloByCode(string idAmm, string codice)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione DBAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return DBAmm.getTipoRuoloByCode(codice, idAmm);
        }

        public static bool InsSysExtAfterAssoc(DocsPaVO.utente.InfoUtente infut, string idAmm, string codUtente, string codRuolo, string descrizione)
        {
            bool retval = false;
            DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtente(codUtente, idAmm);
            DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloByCodice(codRuolo);

            BusinessLogic.Amministrazione.OrganigrammaManager.AmmInsUtenteInRuolo(infut, utente.idPeople, ruolo.idGruppo);
            DocsPaDB.Query_DocsPAWS.Amministrazione DBAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            retval = DBAmm.insNuovoSistemaEsterno(codRuolo, codUtente, ruolo.systemId, idAmm, descrizione);

            if (retval)
            {
                retval = DBAmm.setSystemUser(utente.idPeople);
            }
            return retval;
        }

        public static bool ModificaDescTokenSistemaEsterno(string descrizione, string token, string idSysExt)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione DBAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return DBAmm.ModificaDescTokenSistemaEsterno(descrizione, token, idSysExt);
        }

        public static bool deleteExtSys(DocsPaVO.amministrazione.SistemaEsterno sysExt, DocsPaVO.utente.InfoUtente infoUt)
        {
            bool retval = false;

            DocsPaDB.Query_DocsPAWS.Amministrazione DBAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DocsPaDB.Query_DocsPAWS.Utenti DbUt = new DocsPaDB.Query_DocsPAWS.Utenti();
            DocsPaVO.utente.Amministrazione infoAmm = DBAmm.getInfoAmministrazione(sysExt.idAmministrazione);
            DocsPaVO.utente.Utente utenteNorm = DbUt.getUtenteByCodice(sysExt.UserIdAssociato, infoAmm.codice);
            DocsPaVO.amministrazione.OrgUtente utenteOrg = new DocsPaVO.amministrazione.OrgUtente();
            utenteOrg.IDPeople = utenteNorm.idPeople;
            utenteOrg.UserId = utenteNorm.userId;

            DocsPaVO.amministrazione.EsitoOperazione esito1 = OrganigrammaManager.AmmDisabilitaUtente(infoUt, utenteNorm.idPeople);
            retval = (esito1.Codice == 1 ? false : true);
            // Per eliminare utente e ruolo non vanno presi i metodi in amministrazione, ma quelli in organigramma...
            // Imitare il comportamento di gestioneUtenti.
            // 1. Dissociare utente da ruoli
            // 2. Eliminare utente
            // 3. Eliminare ruolo
            //retval = DBAmm.AmmEliminaUtente(utenteOrg);
            //// eliminazione ruolo (dopo utente)
            logger.Debug("Codice esito cancellazione utente: " + esito1.Codice);
            if (!string.IsNullOrEmpty(esito1.Descrizione))
            {
                logger.Debug(esito1.Descrizione);
            }
            if (retval)
            {
                DocsPaVO.amministrazione.OrgRuolo ruolo = DBAmm.GetRole(sysExt.idRuoloAssociato);
                //string delRuoloResult = DBAmm.AmmEliminaRuolo(ruolo);
                DocsPaVO.amministrazione.EsitoOperazione esito2 = OrganigrammaManager.AmmEliminaRuolo(infoUt, ruolo);
                logger.Debug("Codice esito cancellazione ruolo: " + esito2.Codice);
                if (!string.IsNullOrEmpty(esito2.Descrizione))
                {
                    logger.Debug(esito2.Descrizione);
                }
                retval = DBAmm.delExtSys(sysExt);
            }
            return retval;
        }

        public static bool ModificaMetodiPermessiSistemaEsterno(string metodi, string idSysExt)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione DBAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return DBAmm.ModificaMetodiPermessiSistemaEsterno(metodi, idSysExt);
        }

        public static ArrayList getPISMethods()
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione DBAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

            return DBAmm.getPISMethods();
        }
    }
}
