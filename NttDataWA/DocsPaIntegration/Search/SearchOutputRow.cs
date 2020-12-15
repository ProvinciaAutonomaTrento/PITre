using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaIntegration.Search
{
    [Serializable]
    public class SearchOutputRow
    {
        public string Codice
        {
            get;
            set;
        }

        public string Descrizione
        {
            get;
            set;
        }
    }
}
