using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Requests
{
    public class CreateFolderRequest
    {
        /// <summary>
        /// Id del fascicolo di provenienza
        /// </summary>
        public string IdProject
        {
            get;
            set;
        }

        /// <summary>
        /// Id del titolario
        /// </summary>
        public string ClassificationSchemeId
        {
            get;
            set;
        }

        /// <summary>
        /// Codice del fascicolo
        /// </summary>
        public string CodeProject
        {
            get;
            set;
        }

        /// <summary>
        /// Id della cartella parent. Se omesso inserisce come sottofascicolo del fascicolo principale.
        /// Altrimenti della cartella indicata.
        /// </summary>
        public string IdParentFolder
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione della sottocartella
        /// </summary>
        public string FolderDescription
        {
            get;
            set;
        }
    }
}