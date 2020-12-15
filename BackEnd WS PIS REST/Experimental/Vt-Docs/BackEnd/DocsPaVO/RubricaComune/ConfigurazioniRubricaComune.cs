using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaVO.RubricaComune
{
    /// <summary>
    /// Oggetto che mantiene le configurazioni docspa
    /// per la gestione del sistema esterno rubrica comune
    /// </summary>
    [Serializable()]
    public class ConfigurazioniRubricaComune
    {
        /// <summary>
        /// Flag, indica se la gestione è abilitata o meno
        /// per l'utente o per il contesto corrente
        /// </summary>
        public bool GestioneAbilitata;

        /// <summary>
        /// Flag, indica se la gestione della rubrica comune 
        /// tool di amministrazione è abilitata o meno per il contesto corrente
        /// </summary>
        public bool GestioneAmministrazioneAbilitata;

        /// <summary>
        /// BaseUrl in cui sono disponibili i servizi web
        /// esposti dal sistema rubrica comune
        /// </summary>
        public string ServiceRoot;

        /// <summary>
        /// 
        /// </summary>
        public string SuperUserId;

        /// <summary>
        /// 
        /// </summary>
        public string SuperUserPwd;
    }
}
