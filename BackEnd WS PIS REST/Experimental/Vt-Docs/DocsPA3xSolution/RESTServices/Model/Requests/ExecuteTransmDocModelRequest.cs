using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Requests
{
    public class ExecuteTransmDocModelRequest
    {
        public string IdModel
        {
            get;
            set;
        }

        /// <summary>
        /// Id del documento
        /// </summary>
        public string DocumentId
        {
            get;
            set;
        }
    }
}