using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Modelli_Trasmissioni;

namespace DocsPaVO.Mobile
{
    public class ModelloTrasm
    {
        public int Id
        {
            get;
            set;
        }

        public string Codice
        {
            get;
            set;
        }

        public static ModelloTrasm buildInstance(ModelloTrasmissione input)
        {
            ModelloTrasm res = new ModelloTrasm();
            res.Id = input.SYSTEM_ID;
            res.Codice = input.NOME;
            return res;
        }
    }
}
