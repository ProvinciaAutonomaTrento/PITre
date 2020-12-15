using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain.FileDoc
{
    /*
     * Dati del titolare del certificato
     */
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class SubjectInfo
    {
        /// <summary>
        /// CommonName
        /// </summary>
        [DataMember]
        public string CommonName
        {
            get;
            set;
        }

        /// <summary>
        /// Identificativo del titolare presso il certificatore,
        /// per la normativa 2005 è "dnQualifier"
        /// </summary>
        [DataMember]
        public string CertId
        {
            get;
            set;
        }

        /// <summary>
        /// CodiceFiscale
        /// </summary>
        [DataMember]
        public string CodiceFiscale
        {
            get;
            set;
        }

        /// <summary>
        /// Data di nascita,
        /// solo per la normativa 2000
        /// </summary>
        [DataMember]
        public string DataDiNascita
        {
            get;
            set;
        }

        /// <summary>
        /// Ruolo del titolare
        /// </summary>
        [DataMember]
        public string Ruolo
        {
            get;
            set;
        }

        /// <summary>
        /// Cognome
        /// </summary>
        [DataMember]
        public string Cognome
        {
            get;
            set;
        }

        /// <summary>
        /// Nome
        /// </summary>
        [DataMember]
        public string Nome
        {
            get;
            set;
        }

        /// <summary>
        /// Country
        /// </summary>
        [DataMember]
        public string Country
        {
            get;
            set;
        }

        /// <summary>
        /// Serial Number,
        /// solo per la normativa 2005: comprende il codice fiscale
        /// </summary>
        [DataMember]
        public string SerialNumber
        {
            get;
            set;
        }
        
    }
}