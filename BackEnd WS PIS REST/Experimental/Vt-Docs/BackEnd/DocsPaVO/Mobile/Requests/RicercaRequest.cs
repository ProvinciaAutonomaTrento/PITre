using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Requests
{
    public class RicercaRequest
    {
        public UserInfo UserInfo
        {
            get;
            set;
        }

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

        public string IdGruppo
        {
            get;
            set;
        }

        public string IdCorrGlobali
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
    }
}
