using DocsPaVO.Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaRESTWS.Model
{
    public class RicercaRequest
    {
        public string IdRicercaSalvata
        {
            get;
            set;
        }

        public RicercaSalvataType TypeRicercaSalvata
        {
            get;
            set;
        }

        public string TipoRicercaSalvata { get; set; }

        public string Text
        {
            get;
            set;
        }

        public RicercaType TypeRicerca
        {
            get;
            set;
        }

        public string TipoRicerca { get; set; }

        public string ParentFolderId
        {
            get;
            set;
        }

        public string FascId
        {
            get;
            set;
        }

        public bool EnableProfilazione
        {
            get;
            set;
        }

        public bool EnableUfficioRef
        {
            get;
            set;
        }

        public int RequestedPage
        {
            get;
            set;
        }

        public int PageSize
        {
            get;
            set;
        }

        public string IdDocumento { get; set; }
        public string NumProto { get; set; }

        public string DataDa { get; set; }

        public string DataA { get; set; }

        public string DataProtoDa { get; set; }
        public string DataProtoA { get; set; }
    }
}