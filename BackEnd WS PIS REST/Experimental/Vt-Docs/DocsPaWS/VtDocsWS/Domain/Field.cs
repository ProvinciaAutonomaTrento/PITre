using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain
{
    /// <summary>
    /// Campo della tipologia
    /// </summary>
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class Field
    {
        /// <summary>
        /// System id del campo profilato
        /// </summary>
        [DataMember]
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione del campo profilato
        /// </summary>
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se il campo è obbligatorio oppure no
        /// </summary>
        [DataMember]
        public bool Required
        {
            get;
            set;
        }

        /// <summary>
        /// Valore del campo profilato
        /// </summary>
        [DataMember]
        public string Value
        {
            get;
            set;
        }

        ///// <summary>
        ///// Valori multiplo nel caso delle caselle di selezione
        ///// </summary>
        //[DataMember]
        //public List<string> MultipleChoiceList
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// Valori multiplo nel caso delle caselle di selezione
        /// </summary>
        [DataMember]
        public string[] MultipleChoice
        {
            get;
            set;
        }

        /// <summary>
        /// Tipologia del campo profilato
        /// </summary>
        [DataMember]
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// Se true indica al contatore che deve scattare
        /// </summary>
        [DataMember]
        public bool CounterToTrigger
        {
            get;
            set;
        }

        /// <summary>
        /// Id del registro o dell'RF del contatore da far scattare
        /// </summary>
        [DataMember]
        public string CodeRegisterOrRF
        {
            get;
            set;
        }

        /// <summary>
        /// Diritti che il ruolo dell'utente connesso ha sul campo.
        /// </summary>
        [DataMember]
        public string Rights
        {
            get;
            set;
        }
    }
}