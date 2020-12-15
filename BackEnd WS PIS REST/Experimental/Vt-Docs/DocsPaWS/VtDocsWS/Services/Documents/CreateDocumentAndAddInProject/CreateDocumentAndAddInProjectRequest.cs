using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.CreateDocumentAndAddInProject
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "CreateDocumentAndAddInProjectRequest"
    /// </summary>
    [DataContract]
    public class CreateDocumentAndAddInProjectRequest : Request
    {
        /// <summary>
        /// Nel caso di protocollo specificare il registro
        /// </summary>
        [DataMember]
        public string CodeRegister
        {
            get;
            set;
        }

        /// <summary>
        /// Documento che si vuole creare
        /// </summary>
        [DataMember]
        public Domain.Document Document
        {
            get;
            set;
        }

        /// <summary>
        /// Codice dell'RF in cui si vuole protocollare (opzionale)
        /// </summary>
        [DataMember]
        public string CodeRF
        {
            get;
            set;
        }

        /// <summary>
        /// Codice del fascicolo nel quale fascicolare il documento, il codice prende soltanto i fascicoli nei titolari attivi
        /// </summary>
        [DataMember]
        public string CodeProject
        {
            get;
            set;
        }

        /// <summary>
        /// Id del fascicolo nel quale fascicolare il documento
        /// </summary>
        [DataMember]
        public string IdProject
        {
            get;
            set;
        }

        /// <summary>
        /// Id del titolario
        /// </summary>
        [DataMember]
        public string ClassificationSchemeId
        {
            get;
            set;
        }
    }
}