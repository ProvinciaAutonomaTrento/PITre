using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rubrica.WinClient
{
    public class TipoCorrispondente
    {
        public String Descrizione { get; set; }

        public Rubrica.ClientProxy.RubricaServices.Tipo Value { get; set; }
    }
}
