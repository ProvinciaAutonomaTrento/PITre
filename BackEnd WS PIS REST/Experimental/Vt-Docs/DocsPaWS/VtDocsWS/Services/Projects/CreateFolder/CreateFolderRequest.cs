using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Projects.CreateFolder
{
    public class CreateFolderRequest : Request
    {
        /// <summary>
        /// Id del fascicolo di provenienza
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
        /// Id della cartella parent. Se omesso inserisce come sottofascicolo del fascicolo principale.
        /// Altrimenti della cartella indicata.
        /// </summary>
        [DataMember]
        public string IdParentFolder
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione della sottocartella
        /// </summary>
        [DataMember]
        public string FolderDescription
        {
            get;
            set;
        }
    }
}