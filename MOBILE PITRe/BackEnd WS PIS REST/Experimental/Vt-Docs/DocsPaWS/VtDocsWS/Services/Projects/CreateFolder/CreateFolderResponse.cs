using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Projects.CreateFolder
{
    public class CreateFolderResponse: Response
    {
        /// <summary>
        /// Stringa del risultato. Per non ritornare una risposta vuota.
        /// </summary>
        [DataMember]
        public string Result
        {
            get;
            set;
        }

        /// <summary>
        /// La cartella creata
        /// </summary>
        [DataMember]
        public Domain.Folder Folder
        {
            get;
            set;
        }
    }
}