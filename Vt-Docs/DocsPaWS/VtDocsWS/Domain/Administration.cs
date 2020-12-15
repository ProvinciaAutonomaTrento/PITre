using System.Runtime.Serialization;

namespace VtDocsWS.Domain
{
    /// <summary>
    /// Amministrazione
    /// </summary>
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class Administration
    {
        /// <summary>
        /// Id dell'amministrazione
        /// </summary>
        [DataMember]
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Codice dell'amministrazione
        /// </summary>
        [DataMember]
        public string Code
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione dell'amministrazione
        /// </summary>
        [DataMember]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Libreria dell'amministrazione
        /// </summary>
        [DataMember]
        public string Library
        {
            get;
            set;
        }

        /// <summary>
        /// Email dell'amministrazione
        /// </summary>
        [DataMember]
        public string Email
        {
            get;
            set;
        }
    }
}
