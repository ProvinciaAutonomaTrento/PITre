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
    public class AddAssociazioneQualificaRequest : Request
    {
        /// <summary>
        /// 
        /// </summary>
        public AddAssociazioneQualificaRequest()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public List<string> CodiciQualifiche
        {
            get;
            set;
        }

        /// <summary>
        /// Determina se sollevare o meno l'eccezione se l'associazione qualifica esiste già
        /// </summary>
        public bool ThrowIfAssociazioneExist
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int IdAmministrazione
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int IdUO
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int IdGruppo
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int IdUtente
        {
            get;
            set;
        }
    }
}
