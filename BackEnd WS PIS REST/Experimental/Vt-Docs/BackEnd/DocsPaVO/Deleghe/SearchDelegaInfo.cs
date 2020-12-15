using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Deleghe
{
    public class SearchDelegaInfo
    {
        public string TipoDelega
        {
            get;
            set;
        }

        public string StatoDelega
        {
            get;
            set;
        }

        public string NomeDelegato
        {
            get;
            set;
        }

        public string NomeDelegante
        {
            get;
            set;
        }

        public string IdRuoloDelegante
        {
            get;
            set;
        }

        public DateTime DataDecorrenza
        {
            get;
            set;
        }

        public DateTime DataScadenza
        {
            get;
            set;
        }
    }
}
