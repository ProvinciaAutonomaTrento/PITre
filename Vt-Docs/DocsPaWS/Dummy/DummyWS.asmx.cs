using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace DocsPaWS.Dummy
{
    /// <summary>
    /// Summary description for DummyWS
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/",Name="ExternalWS")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class DummyWS : System.Web.Services.WebService
    {

        [WebMethod]
        public PuntualSearchOutputWS PuntualSearch(PuntualSearchInfoWS searchInfo)
        {
            return DummyHandler.PuntualSearch(searchInfo);
        }

        [WebMethod]
        public SearchOutputWS Search(SearchInfoWS searchInfo)
        {
            return DummyHandler.Search(searchInfo);
        }

    }

    [Serializable]
    public class SearchInfoWS
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

        public int PageSize
        {
            get;
            set;
        }

        public int RequestedPage
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
                int temp = Begin + PageSize;
                return temp;
            }
        }
    }

    [Serializable]
    public class SearchOutputWS
    {

        public int NumTotResults
        {
            get;set;
        }

        public List<SearchOutputRowWS> Rows
        {
            get;set;
        }

        public SearchOutputCode Code
        {
            get;
            set;
        }

        public string ErrorMessage
        {
            get;
            set;
        }

    }

    [Serializable]
    public class PuntualSearchOutputWS
    {


        public SearchOutputRowWS Row
        {
            get;
            set;
        }

        public SearchOutputCode Code
        {
            get;
            set;
        }

        public string ErrorMessage
        {
            get;
            set;
        }

    }


    [Serializable]
    public class SearchOutputRowWS
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

    [Serializable]
    public class PuntualSearchInfoWS
    {

        public string Codice
        {
            get;
            set;
        }
    }

    [Serializable]
    public enum SearchOutputCode
    {
        OK,KO
    }


}
