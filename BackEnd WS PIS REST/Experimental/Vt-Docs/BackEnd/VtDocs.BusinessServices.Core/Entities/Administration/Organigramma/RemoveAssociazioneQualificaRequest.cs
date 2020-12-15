using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Administration.Organigramma
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class RemoveAssociazioneQualificaRequest : Request
    {
        /// <summary>
        /// Id dell'associazione qualifica da rimuovere
        /// </summary>
        public int IdPeopleGroups
        {
            get;
            set;
        }



        /// <summary>
        /// 
        /// </summary>
        public string CodiceQualifica
        {
            get;
            set;
        }

        /// <summary>
        /// Se true, indica di ricercare gli utenti con la qualifica richiesta
        /// per tutte le UO sottostanti
        /// </summary>
        public bool Recursive
        {
            get;
            set;
        }

        /// <summary>
        /// Solo se la ricerca è ricorsiva, definisce la qualifica discriminante per reperire le UO sottostanti
        /// </summary>
        public string CodiceQualificaUODiscriminante
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string IdAmministrazione
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string IdUO
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string IdRuolo
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string IdUtente
        {
            get;
            set;
        }
    }
}
