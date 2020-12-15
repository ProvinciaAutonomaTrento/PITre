using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaIntegration.ObjectTypes.Attributes
{
    public class IntegrationNumberTypeAttribute : IntegrationObjectTypeAttribute
    {
        public IntegrationNumberTypeAttribute(string name, bool mandatory) : base(name,mandatory)
        {

        }

        public override ObjectType Type
        {
            get {
                return ObjectType.NUMBER;
            }
        }

        public override object GetValue(string value)
        {
            try
            {
                return Int32.Parse(value);
            }
            catch (Exception e)
            {
                throw new ObjectValidationException("Il valore " + value + " non è valido");
            }
        }
    }
}
