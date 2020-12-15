using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NttDataWA.Utils
{
    public class FileToSign
    {
        public FileToSign(String fileExtension, String signed, String signType = TipoFirma.NESSUNA_FIRMA)
        {
            this.fileExtension = fileExtension;
            this.signed = signed;
            this.signType = signType;
        }

        public String fileExtension
        {
            get;
            set;
        }

        public String signed
        {
            get;
            set;
        }

        public String signType
        {
            get;
            set;
        }
    }
}