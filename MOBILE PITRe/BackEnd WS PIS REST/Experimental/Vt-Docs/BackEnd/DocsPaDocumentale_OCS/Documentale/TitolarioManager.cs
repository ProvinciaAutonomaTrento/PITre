using System;
using System.Collections.Generic;
using System.Text;
using DocsPaVO.amministrazione;
using DocsPaDocumentale.Interfaces;
using DocsPaVO;
using DocsPaVO.utente;
using DocsPaUtils.LogsManagement;

namespace DocsPaDocumentale_OCS.Documentale
{
    public class TitolarioManager : ITitolarioManager
    {
        #region Ctros, variables, constants

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// 
        private InfoUtente _infoUtente = null;

        public TitolarioManager(InfoUtente infoUtente)
        {
            this._infoUtente = infoUtente;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Attivazione di un titolario
        /// </summary>
        /// <param name="titolario"></param>
        /// <returns></returns>
        public bool AttivaTitolario(OrgTitolario titolario)
        {
            bool retValue = true;
            // ... non fa nulla 
            return retValue;
        }

        /// <summary>
        /// Inserimento o aggiornamento dei metadati generali relativi 
        /// all’intera struttura di classificazione dei documenti
        /// </summary>
        /// <param name="titolario"></param>
        /// <returns></returns>
        public bool SaveTitolario(OrgTitolario titolario)
        {
            bool retValue = true;
            // ... non fa nulla
            return retValue;
        }

        /// <summary>
        /// Cancellazione dei metadati generali relativi 
        /// all’intera struttura di classificazione dei documenti
        /// </summary>
        /// <param name="titolario"></param>
        /// <returns></returns>
        public bool DeleteTitolario(OrgTitolario titolario)
        {
            bool retValue = true;
            // ... non fa nulla
            return retValue;
        }

        /// <summary>
        /// Aggiornamento metadati del nodo di titolario
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <returns></returns>
        public bool SaveNodoTitolario(OrgNodoTitolario nodoTitolario)
        {
            bool retValue = true;
            // ... non fa nulla
            return retValue;
        }

        /// <summary>
        /// Eliminazione di un nodo di titolario
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <returns></returns>
        public bool DeleteNodoTitolario(OrgNodoTitolario nodoTitolario)
        {
            bool retValue = true;
            // ... non fa nulla
            return retValue;
        }

        /// <summary>
        /// Impostazione / rimozione della visibilità di un singolo nodo di titolario per un ruolo
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <param name="ruoloTitolario">
        /// Ruolo cui deve essere associata / rimossa la visibilità verso il nodo titolario
        /// </param>
        /// <returns></returns>
        public bool SetAclRuoloNodoTitolario(OrgNodoTitolario nodoTitolario, OrgRuoloTitolario ruoloTitolario)
        {
            bool retValue = true;
            // ... non fa nulla
            return retValue;
        }

        /// <summary>
        /// Aggiornamento visibilità di più ruoli su un titolario
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <param name="ruoliTitolario"></param>
        /// <returns></returns>
        public DocsPaVO.amministrazione.EsitoOperazione[] SetAclNodoTitolario(DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario, DocsPaVO.amministrazione.OrgRuoloTitolario[] ruoliTitolario)
        {
            throw new InvalidOperationException("Operazione 'SetAclNodoTitolario' non implementata per il documentale");
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Creazione delle ACL per il nodo titolario
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <returns>ACL creata, ma ancora non associata al nodo titolario</returns>

        /// <summary>
        /// Rimozione ACL per il nodo di titolario
        /// </summary>
        /// <param name="nodoTitolario"></param>

        /// <summary>
        /// 
        /// </summary>
        protected InfoUtente InfoUtente
        {
            get
            {
                return this._infoUtente;
            }
            set
            {
                this._infoUtente = value;
            }
        }

        #endregion
    }
}
