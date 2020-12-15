using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Requests
{
    public class ExecuteTransmPrjModelRequest
    {
        /// <summary>
        /// I del modello di trasmissione
        /// </summary>
        public string IdModel
        {
            get;
            set;
        }

        /// <summary>
        /// Id del fascciolo
        /// </summary>
        public string IdProject
        {
            get;
            set;
        }
    }
}