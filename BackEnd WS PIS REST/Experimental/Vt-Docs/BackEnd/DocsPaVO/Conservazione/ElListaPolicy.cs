using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.Conservazione
{
    [Serializable()]
    public class ElListaPolicy
    {
        public Policy[] policyList;
        //public Policy[] policy = null;
        public DateTime esecuzioneQuery = new DateTime();
    }
}
