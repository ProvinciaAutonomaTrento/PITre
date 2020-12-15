using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.utente.Repertori.RequestAndResponse
{
    [Serializable()]
    public class GetRepertorioStateResponse
    {
        public RegistroRepertorioSingleSettings.RepertorioState State { get; set; }
    }
}
