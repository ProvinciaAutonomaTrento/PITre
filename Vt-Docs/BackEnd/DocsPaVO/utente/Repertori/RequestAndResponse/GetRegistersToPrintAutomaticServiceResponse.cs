using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.utente.Repertori.RequestAndResponse
{
    [Serializable()]
    public class GetRegistersToPrintAutomaticServiceResponse
    {
        public List<RegistroRepertorioPrint> RegistersToPrint { get; set; } 
    }
}
