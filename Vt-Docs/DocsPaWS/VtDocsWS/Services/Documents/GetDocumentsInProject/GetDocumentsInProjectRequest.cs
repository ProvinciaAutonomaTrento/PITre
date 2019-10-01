using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.GetDocumentsInProject
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "GetDocumentsInProjectRequest"
    /// </summary>
    [DataContract]
    public class GetDocumentsInProjectRequest : Request
    {
        /// <summary>
        /// Codice del fascicolo
        /// </summary>
        [DataMember]
        public string CodeProject
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

        /// <summary>
        /// Pagina che si desidera visualizzare
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public int? PageNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Quanti elementi sono presenti nella pagina se la ricerca è paginata
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public int? ElementsInPage
        {
            get;
            set;
        }

        /// <summary>
        /// System id del fascicolo
        /// </summary>
        [DataMember]
        public string IdProject
        {
            get;
            set;
        }

        /// <summary>
        /// Se true la relativa response conterrà il numero totale di documenti ottenuti nella ricerca
        /// </summary>
        [DataMember]
        public bool GetTotalDocumentsNumber
        {
            get;
            set;
        }

    }
}