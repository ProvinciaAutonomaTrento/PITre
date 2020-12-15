using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.documento
{
    [Serializable()]
    public class Tab
    {
        public string TransmissionsNumber;

        public string ClassificationsNumber;

        public bool DeletedSecurity;
    }
}
