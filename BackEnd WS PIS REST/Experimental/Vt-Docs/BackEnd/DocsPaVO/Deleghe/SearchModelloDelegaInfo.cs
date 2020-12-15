using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Deleghe
{
    public class SearchModelloDelegaInfo
    {
        public string Nome
        {
            get;
            set;
        }

        public string NomeDelegato
        {
            get;
            set;
        }

        public string IdRuoloDelegante
        {
            get;
            set;
        }

        public DateTime DataInizio
        {
            get;
            set;
        }

        public StatoModelloDelega StatoModelloDelega
        {
            get;
            set;
        }

        public Boolean StatoModelloDelegaSpec
        {
            get;
            set;
        }
    }
}
