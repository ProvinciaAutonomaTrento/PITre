using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaIntegration.Operation
{
    public class OperationInfo
    {
        public string Name
        {
            get;
            set;
        }

        public List<OperationParam> Input
        {
            get;
            set;
        }

        public List<OperationParam> Output
        {
            get;
            set;
        }
    }
}
