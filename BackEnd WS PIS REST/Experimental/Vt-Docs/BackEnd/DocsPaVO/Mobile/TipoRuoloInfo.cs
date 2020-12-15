using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.amministrazione;

namespace DocsPaVO.Mobile
{
    public class TipoRuoloInfo
    {
        public string Descrizione
        {
            get; 
            set; 
        }

        public string Codice
        {
            get; 
            set;
        }

        public string Id
        {
            get; 
            set;
        }

        public static TipoRuoloInfo buildInstance(OrgTipoRuolo input)
        {
            TipoRuoloInfo res = new TipoRuoloInfo();
            res.Descrizione = input.Descrizione;
            res.Id = input.IDTipoRuolo;
            res.Codice = input.Codice;
            return res;
        }
    }
}
