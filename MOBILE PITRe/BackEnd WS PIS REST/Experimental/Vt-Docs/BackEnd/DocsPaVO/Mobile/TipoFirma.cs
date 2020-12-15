using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsPaVO.Mobile
{
    public enum TipoFirma
    {
        NESSUNA_FIRMA = 0, //'N',
        PADES = 1,//'P',
        CADES = 2,//'C',
        ELETTORNICA = 3,//'E',
        TSD = 4,//'T',
        XADES = 5, //'X',
        PADES_ELETTORNICA = 6, //'PE',
        CADES_ELETTORNICA = 7, //'CE',
        TSD_ELETTORNICA = 8,//'TE',
        XADES_ELETTORNICA = 9//'XE'
    }
}
