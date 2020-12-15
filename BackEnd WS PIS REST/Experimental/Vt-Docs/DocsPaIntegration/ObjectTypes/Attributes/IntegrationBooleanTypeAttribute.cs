using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaIntegration.ObjectTypes.Attributes
{
    public class IntegrationBooleanTypeAttribute : IntegrationObjectTypeAttribute
    {
        public IntegrationBooleanTypeAttribute(string name, bool mandatory) : base(name,mandatory)
        {

        }

        public override ObjectType Type
        {
            get {
                return ObjectType.BOOLEAN;
            }
        }

        public override object GetValue(string value)
        {
            try
            {
                return Boolean.Parse(value);
            }
            catch (Exception e)
            {
                throw new ObjectValidationException("Il valore " + value + " non è valido");
            }
        }
    }
}
