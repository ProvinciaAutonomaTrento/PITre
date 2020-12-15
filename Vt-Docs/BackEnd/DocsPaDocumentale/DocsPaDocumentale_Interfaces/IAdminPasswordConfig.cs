using System;
using System.Collections.Generic;
using System.Text;
using DocsPaVO.amministrazione;

namespace DocsPaDocumentale.Interfaces
{
    /// <summary>
    /// Interfaccia per la gestione avanzata delle password per il documentale
    /// </summary>
    public interface IAdminPasswordConfig
    {
        /// <summary>
        /// Indica se il documentale supporta la gestione delle configurazioni delle password
        /// </summary>
        /// <returns></returns>
        bool IsSupportedPasswordConfig();
        
        /// <summary>
        /// Aggiornamento dati relativi alle configurazioni delle password
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="configurations"></param>
        /// <returns></returns>
        bool SavePasswordConfigurations(DocsPaVO.utente.InfoUtente infoUtente, PasswordConfigurations configurations);

        /// <summary>
        /// Reperimento dati relativi alle configurazioni delle password
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        PasswordConfigurations GetPasswordConfigurations(DocsPaVO.utente.InfoUtente infoUtente, int idAmministrazione);

        /// <summary>
        /// Forza la scadenza delle password per tutti gli utenti dell'amministrazione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idAmministrazione"></param>
        void ExpireAllPassword(DocsPaVO.utente.InfoUtente infoUtente, int idAmministrazione);
    }
}