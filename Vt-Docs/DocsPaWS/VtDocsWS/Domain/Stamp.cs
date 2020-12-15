using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain
{
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class Stamp
    {
        /// <summary>
        /// Timbro del protocollo
        /// </summary>
        [DataMember]
        public string StampValue
        {
            get;
            set;
        }

        /// <summary>
        /// Segnatura del protocollo
        /// </summary>
        [DataMember]
        public string SignatureValue
        {
            get;
            set;
        }

        /// <summary>
        /// Codice Amministrazione
        /// </summary>
        [DataMember]
        public string CodeAdministration
        {
            get;
            set;
        }

        /// <summary>
        /// Codice Registro
        /// </summary>
        [DataMember]
        public string CodeRegister
        {
            get;
            set;
        }

        /// <summary>
        /// Codice Unità Organizzativa
        /// </summary>
        [DataMember]
        public string CodeUO
        {
            get;
            set;
        }

        /// <summary>
        /// Anno del protocollo
        /// </summary>
        [DataMember]
        public string Year
        {
            get;
            set;
        }

        /// <summary>
        /// Data protocollazione
        /// </summary>
        [DataMember]
        public string DataProtocol
        {
            get;
            set;
        }

        /// <summary>
        /// Ora di protocollazione
        /// </summary>
        [DataMember]
        public string TimeProtocol
        {
            get;
            set;
        }

        /// <summary>
        /// Tipo di protocollo
        /// </summary>
        [DataMember]
        public string TypeProtocol
        {
            get;
            set;
        }

        /// <summary>
        /// Numero di protocollo
        /// </summary>
        [DataMember]
        public string NumberProtocol
        {
            get;
            set;
        }


        /// <summary>
        /// Codice dell'RF
        /// </summary>
        [DataMember]
        public string CodeRf
        {
            get;
            set;
        }

        /// <summary>
        ///Numero allegati
        /// </summary>
        [DataMember]
        public string NumberAtthachements
        {
            get;
            set;
        }

        /// <summary>
        ///Classificazioni
        /// </summary>
        [DataMember]
        public string Classifications
        {
            get;
            set;
        }

        /// <summary>
        ///DocNumber
        /// </summary>
        [DataMember]
        public string DocNumber
        {
            get;
            set;
        }
    }
}