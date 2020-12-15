using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Interoperabilita.Semplificata
{
    /// <summary>
    /// Impostazioni relative all'interoperabilità semplificata
    /// specifica per un registro o RF
    /// </summary>
    [Serializable]
    public class InteroperabilitySettings
    {
        /// <summary>
        /// Id del registro o RF cui si riferiscono le impostazioni
        /// </summary>
        public String RegistryId { get; set; }

        /// <summary>
        ///  Id del ruolo utilizzato per la creazione del documento
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Id dell'utente utilizzato per la creazione del documento
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Flag utilizzato per indicare se per questo registro / RF è attiva l'interoperabilità
        /// semplificata
        /// </summary>
        public bool IsEnabledInteroperability { get; set; }

        /// <summary>
        /// Flag utilizzato per indicare se i documenti ricevuti per questo registro / RF 
        /// devo essere mantenuti pendenti
        /// </summary>
        public bool KeepPrivate { get; set; }

        /// <summary>
        /// Modalità di gestione dei documenti ricevuti per interoperabilità semplificata per questo registro / RF
        /// </summary>
        public ManagementType ManagementMode { get; set; }

    }

    /// <summary>
    /// Enumerazione delle tipologie di gestione dei documenti ricevuti per interoperabilità semplificata
    /// </summary>
    public enum ManagementType
    {
        /// <summary>
        /// Manuale
        /// </summary>
        M,
        /// <summary>
        /// Automatica
        /// </summary>
        A
    }
}
