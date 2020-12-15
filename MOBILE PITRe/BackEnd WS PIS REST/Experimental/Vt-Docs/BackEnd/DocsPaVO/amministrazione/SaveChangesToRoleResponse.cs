using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.amministrazione
{
    /// <summary>
    /// Response relativa all'azione di salvataggio di modifiche ad un ruolo
    /// </summary>
    [Serializable()]
    public class SaveChangesToRoleResponse
    {
        public SaveChangesToRoleResponse() { }

        public SaveChangesToRoleResponse(SaveChangesToRoleRequest request)
        {
            this.Users = request.Users;
            this.ModifiedRole = request.ModifiedRole;
            this.Result = new EsitoOperazione();
            this.IdOldRole = request.IdOldRole;
            this.IdCorrGlobOldRole = request.IdCorrGlobOldRole;
            this.ExtendVisibility = request.ExtendVisibility;
            this.ComputeAtipicita = request.ComputeAtipicita;
            this.IdOldRoleType = request.IdOldRoleType;
            this.IdOldUO = request.IdOldUO;
        }

        /// <summary>
        /// Esito dell'operazione
        /// </summary>
        public EsitoOperazione Result { get; set; }

        /// <summary>
        /// Prossima fase da compiere
        /// </summary>
        public SaveChangesToRoleRequest.SaveChangesToRolePhase NextPhase { get; set; }

        /// <summary>
        /// Lista degli utenti del ruolo
        /// </summary>
        public OrgUtente[] Users { get; set; }

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
        /// Id della eventuale vecchia UO del ruolo
        /// </summary>
        public String IdOldUO { get; set; }

        /// <summary>
        /// Ruolo modificato da salvare
        /// </summary>
        public OrgRuolo ModifiedRole { get; set; }

        /// <summary>
        /// Messaggio da visualizzare all'amministratore. Se Valorizzato, verrò visualizzata
        /// una richiesta di conferma all'amministratore
        /// </summary>
        public String MessageToShowToAdministrator { get; set; }

        /// <summary>
        /// Opzione di estensione della visibilità da attuare
        /// </summary>
        public SaveChangesToRoleRequest.ExtendVisibilityOption ExtendVisibility { get; set; }

        /// <summary>
        /// Flag che indica se bisogna calcolare l'atipicità a seguito dell'estensione dei diritti
        /// </summary>
        public bool ComputeAtipicita { get; set; }
    }
}
