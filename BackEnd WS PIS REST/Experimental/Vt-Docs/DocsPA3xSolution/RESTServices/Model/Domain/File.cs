using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Domain
{
    public class File
    {
        /// <summary>
        /// System id del documento
        /// </summary>
        public string Id
        {
            get;
            set;
        }
        /// <summary>
        /// Descrizione del file
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Contenuto del file
        /// </summary>
        public byte[] Content
        {
            get;
            set;
        }

        /// <summary>
        /// Mime del file
        /// </summary>
        public string MimeType
        {
            get;
            set;
        }

        /// <summary>
        /// Id della versione del file
        /// </summary>
        public string VersionId
        {
            get;
            set;
        }

        /// <summary>
        /// Nome del file
        /// </summary>
        public string Name
        {
            get;
            set;
        }
    }
}