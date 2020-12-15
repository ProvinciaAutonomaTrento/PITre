using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Requests
{
    public class GetTransmModelsRequest
    {
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// Registri sui quali sono disponibili i modelli
        /// </summary>
        public Domain.Register[] Registers
        {
            get;
            set;
        }
    }
}