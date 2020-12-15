using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Dummy
{
    public class DummyHandler
    {
        private static Dictionary<string, DummyElement> _elements;

        static DummyHandler()
        {
             _elements= new Dictionary<string, DummyElement>();
             for (int i = 0; i < 100; i++)
             {
                 _elements.Add("COD_"+i,new DummyElement("COD_"+i,"DESCRIZIONE_"+i));
             }
        }

        public static SearchOutputWS Search(SearchInfoWS searchInfo)
        {
            if (string.IsNullOrEmpty(searchInfo.Codice) && string.IsNullOrEmpty(searchInfo.Descrizione))
            {
                SearchOutputWS res1 = new SearchOutputWS();
                res1.Code = SearchOutputCode.KO;
                res1.ErrorMessage = "Inserire un campo di ricerca valido";
                return res1;
            }
            IEnumerable<DummyElement> temp=null;
            if(string.IsNullOrEmpty(searchInfo.Codice) && !string.IsNullOrEmpty(searchInfo.Descrizione)){
                temp=_elements.Values.Where(e => e.Descrizione.ToUpper().Contains(searchInfo.Descrizione.ToUpper()));
            }
            if (!string.IsNullOrEmpty(searchInfo.Codice) && string.IsNullOrEmpty(searchInfo.Descrizione))
            {
                temp=_elements.Values.Where(e => e.Codice.ToUpper().Contains(searchInfo.Codice.ToUpper()));
            }
            if (!string.IsNullOrEmpty(searchInfo.Codice) && !string.IsNullOrEmpty(searchInfo.Descrizione))
            {
                temp = _elements.Values.Where(e => e.Codice.ToUpper().Contains(searchInfo.Codice.ToUpper()) && e.Descrizione.ToUpper().Contains(searchInfo.Descrizione.ToUpper()));
            }
            List<DummyElement> tempList = temp.ToList();
            SearchOutputWS res = new SearchOutputWS();
            res.Rows = new List<SearchOutputRowWS>();
            int begin = getBegin(searchInfo.RequestedPage, searchInfo.PageSize, tempList.Count);
            int end = getEnd(searchInfo.RequestedPage, searchInfo.PageSize, tempList.Count);
            for(int i = begin; i < end; i++){
                DummyElement el = tempList[i];
                SearchOutputRowWS row=new SearchOutputRowWS();
                row.Codice = el.Codice;
                row.Descrizione = el.Descrizione;
                res.Rows.Add(row);
            }
            res.NumTotResults = tempList.Count;
            res.Code = SearchOutputCode.OK;
            return res;
        }

        private static int getBegin(int requestedPage,int pageSize,int numTotResults)
        {
            int temp = (requestedPage - 1) * pageSize;
            if (temp > numTotResults - 1) return 0;
            return temp;
        }

        private static int getEnd(int requestedPage, int pageSize, int numTotResults)
        {
            if (getBegin(requestedPage, pageSize, numTotResults) > numTotResults - 1) return 0;
            int temp = getBegin(requestedPage, pageSize, numTotResults) + pageSize;
            return Math.Min(numTotResults, temp);
        }

        public static PuntualSearchOutputWS PuntualSearch(PuntualSearchInfoWS searchInfo)
        {
            PuntualSearchOutputWS res = new PuntualSearchOutputWS();
            if (string.IsNullOrEmpty(searchInfo.Codice.ToUpper()))
            {
                res.Code = SearchOutputCode.KO;
                res.ErrorMessage = "Il campo di input non ha un valore valido";
                return res;
            }
            if (_elements.ContainsKey(searchInfo.Codice))
            {
                DummyElement temp = _elements[searchInfo.Codice];
                SearchOutputRowWS row = new SearchOutputRowWS();
                row.Codice = temp.Codice;
                row.Descrizione = temp.Descrizione;
                res.Row = row;
            }
            res.Code = SearchOutputCode.OK;
            return res;
        }
    }

    public class DummyElement
    {
        private string _codice;
        private string _descrizione;

        public DummyElement(string codice, string descrizione)
        {
            this._codice = codice;
            this._descrizione = descrizione;
        }

        public string Codice
        {
            get
            {
                return _codice;
            }
        }

        public string Descrizione
        {
            get
            {
                return _descrizione;
            }
        }
    }
}
