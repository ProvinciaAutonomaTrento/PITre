using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO.amministrazione;
using DocsPaVO.utente;
using DocsPaUtils.LogsManagement;

namespace DocsPaDocumentale_ETDOCS.Documentale
{
    /// <summary>
    /// Gestione dell'organigramma in amministrazione
    /// per il documentale ETDOCS
    /// </summary>
    public class TitolarioManager : ITitolarioManager
    {
        /// <summary>
        /// 
        /// </summary>
        private InfoUtente _infoUtente = null;

        /// <summary>
        /// 
        /// </summary>
        public TitolarioManager(InfoUtente infoUtente)
        {
            this._infoUtente = infoUtente;
        }

        #region Public methods

        /// <summary>
        /// Attivazione di un titolario
        /// </summary>
        /// <param name="titolario"></param>
        /// <returns></returns>
        public bool AttivaTitolario(OrgTitolario titolario)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return amm.attivaTitolario(titolario);
        }

        /// <summary>
        /// Aggiornamento metadati della struttura
        /// di classificazione del titolario
        /// </summary>
        /// <param name="titolario"></param>
        /// <returns></returns>
        public bool SaveTitolario(OrgTitolario titolario)
        {
            bool saved = false;

            if (string.IsNullOrEmpty(titolario.ID))
            {
                // Inserimento
                using (DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
                {
                    saved = amm.SaveTitolario(titolario);
                }
            }
            else
            {
                // Aggiornamento
                using (DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
                {
                    saved = amm.UpdateTitolario(titolario);
                }
            }

            return saved;
        }

        /// <summary>
        /// Cancellazione metadati della struttura
        /// di classificazione del titolario
        /// </summary>
        /// <param name="titolario"></param>
        /// <returns></returns>
        public bool DeleteTitolario(OrgTitolario titolario)
        {
            bool deleted = false;

            using (DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
            {
                deleted = amm.DeleteTitolario(titolario);
            }
            return deleted;
        }

        /// <summary>
        /// Inserimento di un nuovo nodo di titolario
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <returns></returns>
        public bool SaveNodoTitolario(OrgNodoTitolario nodoTitolario)
        {
            bool saved = false;

            using (DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
            {
                if (string.IsNullOrEmpty(nodoTitolario.ID))
                {
                    EsitoOperazione esito = amm.InsertNodoTitolario(ref nodoTitolario);
                    saved = (esito.Codice == 0);
                }
                else
                    saved = amm.UpdateNodoTitolario(nodoTitolario);
            }

            return saved;
        }

        /// <summary>
        /// Cancellazione di un nodo di titolario
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <returns></returns>
        public bool DeleteNodoTitolario(OrgNodoTitolario nodoTitolario)
        {
            using (DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
                return amm.DeleteNodoTitolario(nodoTitolario);
        }

        /// <summary>
        /// Aggiornamento visibilità di un ruolo su un nodo di titolario
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <param name="ruoloTitolario"></param>
        /// <returns></returns>
        public bool SetAclRuoloNodoTitolario(OrgNodoTitolario nodoTitolario, OrgRuoloTitolario ruoloTitolario)
        {
            using (DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
                return amm.UpdateRuoloTitolario(nodoTitolario.ID, ruoloTitolario.ID, ruoloTitolario.Associato);
        }

        /// <summary>
        /// Aggiornamento visibilità di più ruoli su un titolario
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <param name="ruoliTitolario"></param>
        /// <returns></returns>
        public DocsPaVO.amministrazione.EsitoOperazione[] SetAclNodoTitolario(OrgNodoTitolario nodoTitolario, OrgRuoloTitolario[] ruoliTitolario)
        {
            List<EsitoOperazione> retValue = new List<EsitoOperazione>();

            using (DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
            {
                foreach (OrgRuoloTitolario ruolo in ruoliTitolario)
                {   
                    bool containsAssociazione = (amm.ContainsAssociazioneRuoloTitolario(nodoTitolario.ID, ruolo.ID));

                    if (ruolo.Associato && containsAssociazione)
                    {
                        // L'associazione tra ruolo e nodo titolario già esiste
                        retValue.Add(
                            new EsitoOperazione
                            {
                                Codice = Convert.ToInt32(ruolo.ID),
                                Descrizione = string.Format("Ruolo '{0}' in titolario '{1}':{2}Visibilità già presente", ruolo.Codice, nodoTitolario.Codice, Environment.NewLine)
                            });
                    }
                    else if (!ruolo.Associato && !containsAssociazione)
                    {
                        // L'associazione tra ruolo e nodo titolario è già stata rimossa
                        retValue.Add(
                            new EsitoOperazione
                            {
                                Codice = Convert.ToInt32(ruolo.ID),
                                Descrizione = string.Format("Ruolo '{0}' in titolario '{1}':{2}Visibilità già rimossa", ruolo.Codice, nodoTitolario.Codice, Environment.NewLine)
                            });
                    }
                    // Aggiornamento della visibilità di ciascun ruolo nel nodo di titolario
                    else if (!amm.UpdateRuoloTitolario(nodoTitolario.ID, ruolo.ID, ruolo.Associato))
                    {
                        // Impostazione della visibilità per il ruolo non è andata a buon fine, 
                        // nell'attributo Codice dell'oggetto "EsitoOperazione" viene impostato
                        // l'id del ruolo per cui si è verificato l'errore
                        retValue.Add(
                            new EsitoOperazione
                            {
                                Codice = Convert.ToInt32(ruolo.ID),
                                Descrizione = string.Format("Ruolo '{0}' in titolario '{1}':{2}Aggiornamento visibilità non andato a buon fine", ruolo.Codice, nodoTitolario.Codice, Environment.NewLine)
                            });
                    }
                    else
                    {
                        // Impostazione della visibilità per il ruolo completata con successo
                        string operazione = string.Empty;
                        if (ruolo.Associato)
                            operazione = "impostata";
                        else
                            operazione = "rimossa";

                        retValue.Add(
                           new EsitoOperazione
                           {
                               Codice = 0,
                               Descrizione = string.Format("Ruolo '{0}' in titolario '{1}':{2}Visibilità {3} correttamente", ruolo.Codice, nodoTitolario.Codice, Environment.NewLine, operazione)
                           });
                    }
                }
            }

            return retValue.ToArray();
        }

        #endregion

        #region Protected methods
        
        /// <summary>
        /// Credenziali dell'utente connesso a documentum
        /// </summary>
        protected InfoUtente InfoUtente
        {
            get
            {
                return this._infoUtente;
            }
        }

        #endregion
    }
}
