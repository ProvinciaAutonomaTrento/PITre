using System;
using DocsPaVO.utente;

namespace DocsPaVO.amministrazione
{
    /// <summary>
    /// Request utilizzata dall'amministrazione per apportare modifiche ad un ruolo
    /// </summary>
    [Serializable()]
    public class SaveChangesToRoleRequest
    {
        /// <summary>
        /// Enumerazione delle fasi che compongono l'operazione di modifica di un ruolo
        /// </summary>
        [Serializable()]
        public enum SaveChangesToRolePhase
        {
            Start,
            Initialize,
            CleanToDoList,
            SaveChanges,
            ExtendVisibility,
            CalculateAtipicita,
            UpdateLists,
            UpdateTransmissionModelsAssociation,
            Finish
        }

        /// <summary>
        /// Fase della modifica da compiere
        /// </summary>
        public SaveChangesToRolePhase Phase { get; set; }

        /// <summary>
        /// Ruolo modificato
        /// </summary>
        public OrgRuolo ModifiedRole { get; set; }

        /// <summary>
        /// True se bisogna storicizzare il ruolo vecchio
        /// </summary>
        public bool Historicize { get; set; }

        /// <summary>
        /// True se bisogna aggiornare la proprietà dei modelli di trasmissione
        /// </summary>
        public bool UpdateTransModelsAssociation { get; set; }

        /// <summary>
        /// Lista degli utenti del rullo
        /// </summary>
        public OrgUtente[] Users { get; set; }

        /// <summary>
        /// Informazioni sull'utente
        /// </summary>
        public InfoUtente UserInfo { get; set; }

        /// <summary>
        /// Id del ruolo prima di essere eliminato
        /// </summary>
        public string IdOldRole { get; set; }

        /// <summary>
        /// Id corr globali del ruolo prima di essere eliminato
        /// </summary>
        public String IdCorrGlobOldRole { get; set; }

        /// <summary>
        /// Id tipo ruolo del ruolo prima di essere modificato
        /// </summary>
        public String IdOldRoleType { get; set; }

        /// <summary>
        /// Id dell'eventuale vecchia UO del ruolo
        /// </summary>
        public String IdOldUO { get; set; }

        /// <summary>
        /// Enumerazione delle possibili opzioni utilizzabili per l'estensione di
        /// visibilità ai ruoli gerarchicamente superiori
        /// </summary>
        public enum ExtendVisibilityOption
        {
            /// <summary>
            /// Non estendere
            /// </summary>
            N,
            /// <summary>
            /// Estendi tutti i diritti
            /// </summary>
            A,
            /// <summary>
            /// Estendi escludenti documenti e fascicoli a visibilità atipica
            /// </summary>
            E
        }

        /// <summary>
        /// Tipologia di estensione di visibilità da attuare
        /// </summary>
        public ExtendVisibilityOption ExtendVisibility { get; set; }

        /// <summary>
        /// Flag che indica se bisogna calcolare l'atipicità a seguito dell'estensione dei diritti
        /// </summary>
        public bool ComputeAtipicita { get; set; }

    }
}
