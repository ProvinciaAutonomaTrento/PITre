using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DocsPaIntegration.ObjectTypes.Attributes
{
    public class IntegrationUrlTypeAttribute : IntegrationObjectTypeAttribute
    {
        public IntegrationUrlTypeAttribute(string name, bool mandatory) : base(name,mandatory)
        {

        }

        public override ObjectType Type
        {
            get {
                return ObjectType.URL;
            }
        }

        public override object GetValue(string value)
        {
                string pattern = @"^(http|https|ftp)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$";
                Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                if (!reg.IsMatch(value)) throw new ObjectValidationException("Il valore " + value + " non è un URL valido");
                return value;
        }
    }
}
