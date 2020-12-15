using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.amministrazione
{
    /// <summary>
    /// Rappresenta un documento in stato finale
    /// </summary>
    [Serializable()]
    public class DocumentoStatoFinale
    {
        public string IdDocumento { get; set; }
        public string DocName { get; set; }
        public string Oggetto { get; set; }
        public string MittDest { get; set; }
        public string IdTipologia { get; set; }
        public string Tipologia { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class ModificaAclDocumentoStatoFinale
    {
        public string IdDocumento { get; set; }
        public string IdRuolo { get; set; }
        public string Azione { get; set; }
    }
}
