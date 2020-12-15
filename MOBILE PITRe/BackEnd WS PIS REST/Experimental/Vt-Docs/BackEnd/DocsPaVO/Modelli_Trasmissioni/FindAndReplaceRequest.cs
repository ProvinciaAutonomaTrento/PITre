using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.utente;
using DocsPaVO.filtri;
using DocsPaVO.rubrica;

namespace DocsPaVO.Modelli_Trasmissioni
{
    [Serializable()]
    public class FindAndReplaceRequest
    {

        public FiltroRicerca[] SearchFilters { get; set; }

        public InfoUtente UserInfo { get; set; }

        public ElementoRubrica RoleToReplace { get; set; }

        public ElementoRubrica NewRole { get; set; }

        public FindAndReplaceEnum Operation { get; set; }


        public enum FindAndReplaceEnum
        {
            Find,
            Replace
        }

        public Boolean IsAdministrator { get; set; }

        public ModelloTrasmissioneSearchResultCollection Models { get; set; }

        public bool CopyNotes { get; set; }
    }
}
