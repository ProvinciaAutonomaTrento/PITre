using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain
{
    /// <summary>
    /// Corrispondente
    /// </summary>
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class Correspondent
    {
        /// <summary>
        /// System id del corrispondente
        /// </summary>
        [DataMember]
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione del corrispondente
        /// </summary>
        [DataMember]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Codice rubrica
        /// </summary>
        [DataMember]
        public string Code
        {
            get;
            set;
        }

        /// <summary>
        /// Indirizzo
        /// </summary>
        [DataMember]
        public string Address
        {
            get;
            set;
        }

        /// <summary>
        /// Cap
        /// </summary>
        [DataMember]
        public string Cap
        {
            get;
            set;
        }

        /// <summary>
        /// Citta
        /// </summary>
        [DataMember]
        public string City
        {
            get;
            set;
        }

        /// <summary>
        /// Provincia
        /// </summary>
        [DataMember]
        public string Province
        {
            get;
            set;
        }

        /// <summary>
        /// Località
        /// </summary>
        [DataMember]
        public string Location
        {
            get;
            set;
        }

        /// <summary>
        /// Nazione
        /// </summary>
        [DataMember]
        public string Nation
        {
            get;
            set;
        }

        /// <summary>
        /// Telefono
        /// </summary>
        [DataMember]
        public string PhoneNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Fax
        /// </summary>
        [DataMember]
        public string Fax
        {
            get;
            set;
        }

        /// <summary>
        /// Codice Fiscale
        /// </summary>
        [DataMember]
        public string NationalIdentificationNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Partita.iVA
        /// </summary>
        [DataMember]
        public string VatNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Email
        /// </summary>
        [DataMember]
        public string Email
        {
            get;
            set;
        }

        /// <summary>
        /// Codice AOO
        /// </summary>
        [DataMember]
        public string AOOCode
        {
            get;
            set;
        }

        /// <summary>
        /// Codice amministrazione
        /// </summary>
        [DataMember]
        public string AdmCode
        {
            get;
            set;
        }

        /// <summary>
        /// Note
        /// </summary>
        [DataMember]
        public string Note
        {
            get;
            set;
        }

        /// <summary>
        /// Tipo di corrispondente: I Interno / E Esterno
        /// </summary>
        [DataMember]
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// Codice del registro o dell'RF
        /// </summary>
        [DataMember]
        public string CodeRegisterOrRF
        {
            get;
            set;
        }

        /// <summary>
        /// Tipo del corrispondente: U UO/R Roles/P People/O Occasional/L List/F RF
        /// </summary>
        [DataMember]
        public string CorrespondentType
        {
            get;
            set;
        }

        /// <summary>
        /// Canale preferenziale
        /// </summary>
        [DataMember]
        public string PreferredChannel
        {
            get;
            set;
        }

        /// <summary>
        /// Nome
        /// </summary>
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Cognome
        /// </summary>
        [DataMember]
        public string Surname
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se il corrispondente è memorizzato nella rubrica comune
        /// </summary>
        [DataMember]
        public bool IsCommonAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Codice IPA
        /// </summary>
        //[DataMember]
        //public string IpaCode
        //{
        //    get;
        //    set;
        //}
    }
}