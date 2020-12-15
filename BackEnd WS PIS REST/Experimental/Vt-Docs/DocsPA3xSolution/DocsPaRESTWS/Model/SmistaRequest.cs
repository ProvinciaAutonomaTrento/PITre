using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaRESTWS.Model
{
    public class SmistaRequest
    {
        public string IdTrasmissione
        {
            get;
            set;
        }

        public string IdTrasmissioneUtente
        {
            get;
            set;
        }

        public bool HasWorkflow { get; set; }
        public string IdEvento { get; set; }

        //public string NoteAccettazione{get;set;}
        

        //public string Action
        //{
        //    get;
        //    set;
        //}

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

        public string IdModelloTrasm
        {
            get;
            set;
        }

        public string NoteTrasm { get; set; }
        
        public string Path
        {
            get;
            set;
        }

    }
}