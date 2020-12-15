using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.amministrazione
{
    [Serializable()]
    public class VisualizzaStatoDoc
    {
        #region Info documento
        public string idDocumento;
        public string segnatura;
        public string utenteProtocollatore;
        public string ruoloProtocollatore;
        public string uoProtocollatore;
        public string descrizioneTipologia;
        #endregion
        public List<string> trasmissioniDocumento;
        public bool spedizioniDocumento;
        public List<string> fascicoliDocumento;
    }
}
