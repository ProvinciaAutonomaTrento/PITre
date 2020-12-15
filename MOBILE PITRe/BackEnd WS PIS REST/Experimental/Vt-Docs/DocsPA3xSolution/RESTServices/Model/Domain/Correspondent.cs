using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Domain
{
    public class Correspondent
    {
        /// <summary>
        /// System id del corrispondente
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione del corrispondente
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Codice rubrica
        /// </summary>
        public string Code
        {
            get;
            set;
        }

        /// <summary>
        /// Indirizzo
        /// </summary>
        public string Address
        {
            get;
            set;
        }

        /// <summary>
        /// Cap
        /// </summary>
        public string Cap
        {
            get;
            set;
        }

        /// <summary>
        /// Citta
        /// </summary>
        public string City
        {
            get;
            set;
        }

        /// <summary>
        /// Provincia
        /// </summary>
        public string Province
        {
            get;
            set;
        }

        /// <summary>
        /// Località
        /// </summary>
        public string Location
        {
            get;
            set;
        }

        /// <summary>
        /// Nazione
        /// </summary>
        public string Nation
        {
            get;
            set;
        }

        /// <summary>
        /// Telefono
        /// </summary>
        public string PhoneNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Telefono
        /// </summary>
        public string PhoneNumber2
        {
            get;
            set;
        }

        /// <summary>
        /// Fax
        /// </summary>
        public string Fax
        {
            get;
            set;
        }

        /// <summary>
        /// Codice Fiscale
        /// </summary>
        public string NationalIdentificationNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Partita.iVA
        /// </summary>
        public string VatNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Email
        /// </summary>
        public string Email
        {
            get;
            set;
        }

        /// <summary>
        /// Altre email
        /// </summary>
        public List<string> OtherEmails
        {
            get;
            set;
        }

        /// <summary>
        /// Codice AOO
        /// </summary>
        public string AOOCode
        {
            get;
            set;
        }

        /// <summary>
        /// Codice amministrazione
        /// </summary>
        public string AdmCode
        {
            get;
            set;
        }

        /// <summary>
        /// Note
        /// </summary>
        public string Note
        {
            get;
            set;
        }

        /// <summary>
        /// Tipo di corrispondente: I Interno / E Esterno
        /// </summary>
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// Codice del registro o dell'RF
        /// </summary>
        public string CodeRegisterOrRF
        {
            get;
            set;
        }

        /// <summary>
        /// Tipo del corrispondente: U UO/R Roles/P People/O Occasional/L List/F RF
        /// </summary>
        public string CorrespondentType
        {
            get;
            set;
        }

        /// <summary>
        /// Canale preferenziale
        /// </summary>
        public string PreferredChannel
        {
            get;
            set;
        }

        /// <summary>
        /// Nome
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Cognome
        /// </summary>
        public string Surname
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se il corrispondente è memorizzato nella rubrica comune
        /// </summary>
        public bool IsCommonAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Codice IPA
        /// </summary>
        //        //public string IpaCode
        //{
        //    get;
        //    set;
        //}
    }
}