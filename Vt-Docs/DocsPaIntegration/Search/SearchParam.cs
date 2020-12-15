using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaIntegration.Search
{
    public class SearchParam
    {
        private string _id;
        private string _name;
        private bool _isKey;

        public SearchParam(string id, string name, bool isKey)
        {
            this._id = id;
            this._name = name;
            this._isKey = isKey;
        }

        public string Id
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
        }

        public bool IsKey
        {
            get { return _isKey; }
        }
    }
}
