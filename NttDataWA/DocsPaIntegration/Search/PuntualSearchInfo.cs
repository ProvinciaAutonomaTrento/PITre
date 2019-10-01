using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaIntegration.Search
{
    public class PuntualSearchInfo
    {
        private string _codice;

        public PuntualSearchInfo(string codice)
        {
            this._codice = codice;
        }

        public string Codice
        {
            get
            {
                return _codice;
            }
        }
    }
}
