using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Sanita.Responses
{
    public class ImportAttestatiResponse
    {
        public ImportAttestatiResponseCode Code
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

    public enum ImportAttestatiResponseCode
    {
        OK,KO
    }
}
