using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaVO.documento
{
    [Serializable()]
    public class CessioneDocumento
    {
        public string idPeople;
        public string idRuolo;
        public bool docCeduto;
        public string idPeopleNewPropr;
        public string idRuoloNewPropr;
        public string userId;
    }
}
