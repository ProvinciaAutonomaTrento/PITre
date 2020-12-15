using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaIntegration.Search
{
    [Serializable]
    public class SearchOutput
    {
        private List<SearchOutputRow> _rows;
        private int _numTotResult;

        public SearchOutput(List<SearchOutputRow> rows, int numTotResults)
        {
            this._rows = rows;
            this._numTotResult = numTotResults;
        }

        public int NumTotResults
        {
            get
            {
                return _numTotResult;
            }
        }

        public List<SearchOutputRow> Rows
        {
            get
            {
                return _rows;
            }
        }

    }
}
