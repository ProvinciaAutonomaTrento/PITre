using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.amministrazione;

namespace DocsPaVO.utente
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class RaggruppamentoFunzionale : Corrispondente
    {
        //public String Id { get; set; }
        public String Codice { get; set; }
        //public String Descrizione { get; set; }
        //public OrgDettagliGlobali Dettagli { get; set; }
        //public String IdAmministrazione { get; set; }
    }
}