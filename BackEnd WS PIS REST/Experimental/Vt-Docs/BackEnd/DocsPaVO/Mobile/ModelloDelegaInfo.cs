using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Deleghe;

namespace DocsPaVO.Mobile
{
    public class ModelloDelegaInfo
    {
        public string Id
        {
            get;
            set;
        }

        public string Nome
        {
            get;
            set;
        }

        public string DescrUtenteDelegato
        {
            get;
            set;
        }

        public string DescrRuoloDelegante
        {
            get;
            set;
        }
        
        public DateTime DataInizioDelega
        {
            get;
            set;
        }

        public DateTime DataFineDelega
        {
            get;
            set;
        }

        public static ModelloDelegaInfo buildInstance(ModelloDelega input){
           ModelloDelegaInfo res=new ModelloDelegaInfo();
           res.Id = input.Id;
           res.Nome = input.Nome;
           res.DataInizioDelega = input.DataInizioDelega;
           res.DataFineDelega = input.DataFineDelega;
           if (input.DescrRuoloDelegante.Length >0)
               res.DescrRuoloDelegante = input.DescrRuoloDelegante;
           else
               res.DescrRuoloDelegante = "Tutti";

           res.DescrUtenteDelegato =  input.DescrUtenteDelegato;
           return res;
        }
    }
}
