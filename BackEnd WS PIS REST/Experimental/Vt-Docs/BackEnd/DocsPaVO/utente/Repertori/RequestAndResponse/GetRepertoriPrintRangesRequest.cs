using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.utente.Repertori.RequestAndResponse
{
    [Serializable()]
    public class GetRepertoriPrintRangesRequest
    {
        public string CounterId { get; set; }

        public string RegistryId { get; set; }

        public string RfId { get; set; }
    }
}
