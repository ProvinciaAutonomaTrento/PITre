using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using DocsPaVO.utente;
using DocsPaDB.Query_DocsPAWS;
using System.Data;

namespace BusinessLogic.Interoperabilità
{
    public partial class InteroperabilitaSegnatura
    {
        /// <summary>
        /// Codice della chiave per l'abilitazione/disabilitazione della funzionalità
        /// </summary>
        public const String INTEROP_INTERNA_TRASMISSIONE_SELETTIVA = "INTEROP_INT_TRASM_SELETTIVA";

        /// <summary>
        /// Metodo utilizzato per verificare se, per l'amministrazione corrente è attiva la funzionalità di 
        /// trasmissione selettiva
        /// </summary>
        /// <param name="adminId">Id dell'amministrazione</param>
        /// <returns>Flag che indica lo stato di attivazione della funzionalità</returns>
        public static bool IsEnabledSelectiveTransmission(String adminId)
        {
            string enabled = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(adminId, INTEROP_INTERNA_TRASMISSIONE_SELETTIVA);
            if (String.IsNullOrEmpty(enabled))
                enabled = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", INTEROP_INTERNA_TRASMISSIONE_SELETTIVA);

            return enabled == "1";

        }

        /// <summary>
        /// Questo metodo restituisce la lista dei corrispondenti destinatari della trasmissione.
        /// Nel caso in cui il destinatario sia una UO, restituisce la lista dei ruoli, 
        /// definiti nella UO abilitati alla ricezione di trasmissioni.
        /// Nel caso in cui il destinatario sia un ruolo, restituisce la lista dei ruoli,
        /// definiti nella uo di appartenenza del ruolo, abilitati alla ricezione di trasmissioni.
        /// </summary>
        /// <param name="reg">AOO destinataria della spedizione</param>
        /// <param name="uoId">Identificativo della UO destinataria della spedizione</param>
        /// <returns>Lista dei ruoli cui trasmettere il documento</returns>
        private static List<Ruolo> GetRecipients(Registro reg, String uoId)
        {
            List<Ruolo> retVal = new List<Ruolo>();
            try
            {
         
                Interoperabilita obj = new Interoperabilita();

                DataSet ds = new DataSet();
                obj.GetSelectiveRecipients(out ds, reg, uoId);

                for (int i = 0; i < ds.Tables["RUOLI"].Rows.Count; i++)
                {
                    DataRow ruoloRow = ds.Tables["RUOLI"].Rows[i];
                    DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                    ruolo.systemId = ruoloRow["SYSTEM_ID"].ToString();
                    ruolo.codiceCorrispondente = ruoloRow["VAR_CODICE"].ToString();
                    ruolo.codiceRubrica = ruoloRow["VAR_COD_RUBRICA"].ToString();
                    ruolo.descrizione = ruoloRow["VAR_DESC_RUOLO"].ToString() + " " + ruoloRow["VAR_DESC_CORR"].ToString();
                    ruolo.livello = ruoloRow["NUM_LIVELLO"].ToString();
                    ruolo.idGruppo = ruoloRow["ID_GRUPPO"].ToString();
                    DocsPaVO.utente.UnitaOrganizzativa uoDest = new DocsPaVO.utente.UnitaOrganizzativa();
                    ruolo.uo = uoDest;
                    ruolo.uo.systemId = ruoloRow["ID_UO"].ToString();
                    ruolo.tipoCorrispondente = "R";
                    retVal.Add(ruolo);
                }
            }
            catch (Exception e)
            {
                logger.Debug(e.ToString());
                //db.closeConnection();
                logger.Debug("Errore nella gestione dell'interoperabilità. (GetRecipients)", e);
                throw e;
            }

            return retVal;
        } 

    }
}
