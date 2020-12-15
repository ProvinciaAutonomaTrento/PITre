using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Requests
{
    public class OpenCloseProjectRequest
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
        /// Codice del fascicolo
        /// </summary>
        public string Action
        {
            get;
            set;
        }
    }
}