using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.areaConservazione
{
    [Serializable()]
    public class ItemsVersamento
    {
        public string idProfile;
        public string idPeople;
        public string idGruppo;
        public string idAmm;
        public string tentativiInvio;
        public string stato;
    }
}
