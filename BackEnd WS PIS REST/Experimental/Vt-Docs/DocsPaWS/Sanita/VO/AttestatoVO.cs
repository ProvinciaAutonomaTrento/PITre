using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaWS.Sanita.VO
{
    public class AttestatoVO
    {
        public string Oggetto
        {
            get;
            set;
        }

        public string IdTemplate
        {
            get;
            set;
        }

        public byte[] Content
        {
            get;
            set;
        }

        public string FileName
        {
            get;
            set;
        }
    }
}
