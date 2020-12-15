using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaIntegration.Operation;
using DocsPaIntegration.ObjectTypes.Attributes;

namespace DocsPaIntegrationImpl.VO
{
    public class TestOutput : OperationBean
    {
        [IntegrationNumberTypeAttribute("OutputParam1", true)]
        public int Param1
        {
            get;
            set;
        }
    }
}
