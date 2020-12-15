using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaIntegration.Search
{
    [Serializable]
    public class SearchInfo
    {

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

        public int Begin
        {
            get
            {
                int temp = (RequestedPage - 1) * PageSize;
                return temp;
            }
        }

        public int End
        {
            get
            {
                int temp = Begin+ PageSize;
                return temp;
            }
        }

        public string Descrizione
        {
            get;
            set;
        }

        public string Codice
        {
            get;
            set;
        }

        public bool PagingRequired
        {
            get
            {
                return PageSize > 0;
            }
        }

        public Dictionary<object, object> OtherParam
        {
            get;
            set;
        }
    }
}
