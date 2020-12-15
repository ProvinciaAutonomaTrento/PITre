using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.areaConservazione;

namespace DocsPa_I_TSAuthority
{
    /// <summary>
    /// 
    /// </summary>
    public interface I_TSR_Request
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="TimeStampQuery"></param>
        /// <returns></returns>
        OutputResponseMarca getTimeStamp(InputMarca TimeStampQuery);
    }
}
