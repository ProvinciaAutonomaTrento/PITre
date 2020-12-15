using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Fatturazione
{
    public class CampoCustomFattura
    {
        public TipoCampoCustomFatturaType Tipo { get; set; }

        public string NomeCampo { get; set; }

        public string NumeroLinea { get; set; }

        public string Valore { get; set; }
    }

    public enum TipoCampoCustomFatturaType
    {
        ITEM,
        HEADER
    }
}
