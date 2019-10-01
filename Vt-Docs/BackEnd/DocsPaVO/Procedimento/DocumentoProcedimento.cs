using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Procedimento
{
    public class DocumentoProcedimento
    {
        private String _id;
        private String _dataVisualizzazione;

        public String Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public String DataVisualizzazione
        {
            get
            {
                return _dataVisualizzazione;
            }
            set
            {
                _dataVisualizzazione = value;
            }
        }
    }
}
