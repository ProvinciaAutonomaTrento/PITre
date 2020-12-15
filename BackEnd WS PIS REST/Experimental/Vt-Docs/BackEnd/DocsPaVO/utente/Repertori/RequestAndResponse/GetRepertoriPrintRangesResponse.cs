using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.utente.Repertori.RequestAndResponse
{
    [Serializable()]
    public class GetRepertoriPrintRangesResponse
    {
        public List<RepertorioPrintRange> Ranges { get; set; }
    }
}
