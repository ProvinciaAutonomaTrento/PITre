using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace DocsPaVO.Plugin
{
    [Serializable()]
    public class DpaPluginHash
    {
        public string systemId;
        public string idProfile;
        public string idPeople;
        public DateTime dataElaborazione;
        public string hashFile;
    }
}
