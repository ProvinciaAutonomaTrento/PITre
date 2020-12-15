using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaIntegration.ObjectTypes;

namespace DocsPaIntegration.Operation
{
    public class OperationParam
    {
        public string Name
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }

        public ObjectType Type
        {
            get;
            set;
        }
    }
}
