using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Sanita.VO
{
    public class StatusTemplateVO
    {
        public string IdTemplate
        {
            get;
            set;
        }

        public string StatusTemplateFieldName
        {
            get;
            set;
        }

        public string StatusProcessedValue
        {
            get;
            set;
        }

        public string StatusNotProcessedValue
        {
            get;
            set;
        }

    }
}