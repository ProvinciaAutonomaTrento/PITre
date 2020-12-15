using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.documento;

namespace DocsPaVO.utente.Repertori.RequestAndResponse
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class GeneratePrintRepertorioResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public SchedaDocumento Document { get; set; }
    }
}
