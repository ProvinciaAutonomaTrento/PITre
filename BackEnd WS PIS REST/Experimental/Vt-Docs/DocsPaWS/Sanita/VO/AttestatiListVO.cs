using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaWS.Sanita.VO
{
    public class AttestatiListVO
    {
        public string DocNumber
        {
            get;
            set;
        }

        public StatusTemplateVO StatusTemplate
        {
            get;
            set;
        }

        public List<AttestatoVO> Attestati
        {
            get;
            set;
        }
    }
}
