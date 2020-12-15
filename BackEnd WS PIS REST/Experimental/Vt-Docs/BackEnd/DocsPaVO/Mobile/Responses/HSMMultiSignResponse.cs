using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsPaVO.Mobile.Responses
{
    public class HSMMultiSignResponse
    {
        public HSMMultiSignResponseCode Code { get; set; }
        public InfoDocFirmaHSM[] infoFirma { get; set; }
        public static HSMMultiSignResponse ErrorResponse
        {
            get
            {
                HSMMultiSignResponse res = new HSMMultiSignResponse();
                res.Code = HSMMultiSignResponseCode.SYSTEM_ERROR;
                return res;
            }
        }
    }

    public enum HSMMultiSignResponseCode
    {
        OK, SYSTEM_ERROR, KO
    }

    public class InfoDocFirmaHSM
    {
        public string IdDocumento;
        public string Status;
        public string ErrorMessage;
    }
}
