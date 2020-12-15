using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Domain
{
    public class Stamp
    {
        public string StampValue
        {
            get;
            set;
        }

        /// <summary>
        /// Segnatura del protocollo
        /// </summary>
        public string SignatureValue
        {
            get;
            set;
        }

        /// <summary>
        /// Codice Amministrazione
        /// </summary>
        public string CodeAdministration
        {
            get;
            set;
        }

        /// <summary>
        /// Codice Registro
        /// </summary>
        public string CodeRegister
        {
            get;
            set;
        }

        /// <summary>
        /// Codice Unità Organizzativa
        /// </summary>
        public string CodeUO
        {
            get;
            set;
        }

        /// <summary>
        /// Anno del protocollo
        /// </summary>
        public string Year
        {
            get;
            set;
        }

        /// <summary>
        /// Data protocollazione
        /// </summary>
        public string DataProtocol
        {
            get;
            set;
        }

        /// <summary>
        /// Ora di protocollazione
        /// </summary>
        public string TimeProtocol
        {
            get;
            set;
        }

        /// <summary>
        /// Tipo di protocollo
        /// </summary>
        public string TypeProtocol
        {
            get;
            set;
        }

        /// <summary>
        /// Numero di protocollo
        /// </summary>
        public string NumberProtocol
        {
            get;
            set;
        }


        /// <summary>
        /// Codice dell'RF
        /// </summary>
        public string CodeRf
        {
            get;
            set;
        }

        /// <summary>
        ///Numero allegati
        /// </summary>
        public string NumberAtthachements
        {
            get;
            set;
        }

        /// <summary>
        ///Classificazioni
        /// </summary>
        public string Classifications
        {
            get;
            set;
        }

        /// <summary>
        ///DocNumber
        /// </summary>
        public string DocNumber
        {
            get;
            set;
        }
    }
}