using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Procedimento.Report
{
    public class ReportProcedimentoRequest
    {
        private String _idProcedimento;
        private String _idAmm;
        private String _anno;

        public String IdProcedimento
        {
            get
            {
                return _idProcedimento;
            }
            set
            {
                _idProcedimento = value;
            }
        }

        public String IdAmm
        {
            get
            {
                return _idAmm;
            }
            set
            {
                _idAmm = value;
            }
        }

        public String Anno
        {
            get
            {
                return _anno;
            }
            set
            {
                _anno = value;
            }
        }
    }

}
