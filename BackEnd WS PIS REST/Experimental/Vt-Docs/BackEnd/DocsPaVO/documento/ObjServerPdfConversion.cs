using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Collections;

namespace DocsPaVO.documento
{
    [Serializable()]
    public class ObjServerPdfConversion
    {
        public byte[] content = null;
        public string fileName  = string.Empty;
        public string idProfile = string.Empty;
        public string docNumber = string.Empty;       
    }   
}
