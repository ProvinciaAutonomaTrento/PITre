using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaIntegration.ObjectTypes.Attributes
{
    public class IntegrationStringTypeAttribute : IntegrationObjectTypeAttribute
    {
        public IntegrationStringTypeAttribute(string name, bool mandatory) : base(name,mandatory)
        {

        }

        public override ObjectType Type
        {
            get {
                return ObjectType.STRING;
            }
        }

        public override object GetValue(string value)
        {
            return value;
        }
    }
}
