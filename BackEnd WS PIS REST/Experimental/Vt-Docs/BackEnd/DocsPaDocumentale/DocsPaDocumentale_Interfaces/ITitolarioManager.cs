using System;
using System.Collections.Generic;
using System.Text;
using DocsPaVO.amministrazione;

namespace DocsPaDocumentale.Interfaces
{
    /// <summary>
    /// Interfaccia per la gestione dei ruoli di titolario in amministrazione
    /// per il documentale
    /// </summary>
    public interface ITitolarioManager
    {
        /// <summary>
        /// Attivazione di un titolario
        /// </summary>
        /// <param name="titolario"></param>
        /// <returns></returns>
        bool AttivaTitolario(OrgTitolario titolario);
        
        /// <summary>
        /// Aggiornamento metadati della struttura
        /// di classificazione del titolario
        /// </summary>
        /// <param name="titolario"></param>
        /// <returns></returns>
        bool SaveTitolario(OrgTitolario titolario);

        /// <summary>
        /// Cancellazione struttura di classificazione
        /// </summary>
        /// <param name="titolario"></param>
        /// <returns></returns>
        bool DeleteTitolario(OrgTitolario titolario);

        /// <summary>
        /// Aggiornamento metadati del nodo di titolario
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <returns></returns>
        bool SaveNodoTitolario(OrgNodoTitolario nodoTitolario);

        /// <summary>
        /// Eliminazione di un nodo di titolario
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <returns></returns>
        bool DeleteNodoTitolario(OrgNodoTitolario nodoTitolario);

        /// <summary>
        /// Aggiornamento visibilità di un ruolo su un titolario
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <param name="ruoloTitolario"></param>
        /// <returns></returns>
        bool SetAclRuoloNodoTitolario(OrgNodoTitolario nodoTitolario, OrgRuoloTitolario ruoloTitolario);

        /// <summary>
        /// Aggiornamento visibilità di più ruoli su un titolario
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <param name="ruoliTitolario"></param>
        /// <returns>Riporta l'esito dell'operazione effettuata</returns>
        DocsPaVO.amministrazione.EsitoOperazione[] SetAclNodoTitolario(OrgNodoTitolario nodoTitolario, OrgRuoloTitolario[] ruoliTitolario);
    }
}
