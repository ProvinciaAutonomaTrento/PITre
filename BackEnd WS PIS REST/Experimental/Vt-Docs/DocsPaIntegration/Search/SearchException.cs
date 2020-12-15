using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaIntegration.Search
{
    public class SearchException : Exception
    {
        private  SearchExceptionCode _code;
        private string _message;

        public SearchException(SearchExceptionCode code)
        {
            this._code = code;
        }

        public SearchException(SearchExceptionCode code,string message)
        {
            this._code = code;
            this._message = message;
        }

        public SearchExceptionCode Code
        {
            get
            {
                return _code;
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }
        }
    }

    public enum SearchExceptionCode
    {
        SERVER_ERROR,SERVICE_UNAVAILABLE
    }
}
