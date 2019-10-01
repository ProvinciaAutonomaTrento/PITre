using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile
{
    public class ModelloSmistamento
    {
        public string IdModello
        {
            get;
            set;
        }

        public string Descrizione
        {
            get;
            set;
        }

        public bool IsDefault
        {
            get;
            set;
        }

        public SmistamentoElement RootElement
        {
            get;
            set;
        }
    }
}
