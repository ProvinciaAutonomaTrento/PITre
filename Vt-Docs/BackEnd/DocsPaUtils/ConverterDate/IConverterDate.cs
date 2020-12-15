using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaUtils.ConverterDate
{
    public interface IConverterDate
    {
        DateTime getDateTime(string datetime);
    }
}
