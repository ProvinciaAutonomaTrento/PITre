using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaRESTWS.Model
{
    public class EseguiTrasmDirettaRequest
    {
        public string IdDoc
        {
            get;
            set;
        }

        public string IdFasc
        {
            get;
            set;
        }

        public string IdDestinatario
        {
            get;
            set;
        }

        public string CodiceDestinatario { get; set; }

        public bool Notify { get; set; }

        public string TipoTrasmissione { get; set; }

        public string Ragione { get; set; }

        public string Note
        {
            get;
            set;
        }

        public string Path
        {
            get;
            set;
        }
    }
}