using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DocsPaVO.Conservazione.PARER.Report
{
    public class ReportSingolaAmmResponse
    {
        private DataSet _ds;
        private String _nome_amm;
        private String _mail_resp;

        public DataSet Dataset
        {
            get
            {
                return _ds;
            }
            set
            {
                _ds = value;
            }
        }

        public String MailResponsabile
        {
            get
            {
                return _mail_resp;
            }
            set
            {
                _mail_resp = value;
            }
        }
    }
}
