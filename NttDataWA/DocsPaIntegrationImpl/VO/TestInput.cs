using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaIntegration.ObjectTypes.Attributes;
using DocsPaIntegration.Operation;

namespace DocsPaIntegrationImpl.VO
{
    public class TestInput : OperationBean
    {
        [IntegrationNumberTypeAttribute("Param1",true)]
        public int Param1
        {
            get;
            set;
        }

        [IntegrationStringTypeAttribute("Param2", false)]
        public string Param2
        {
            get;
            set;
        }
    }
}
