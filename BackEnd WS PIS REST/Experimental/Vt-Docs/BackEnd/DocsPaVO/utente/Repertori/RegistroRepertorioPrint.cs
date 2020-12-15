using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.utente.Repertori
{
    /// <summary>
    /// Questo oggetto, utlizzato dal servizio di stampa automatica,
    /// contiene le informazioni su un registro di repertorio e sul
    /// suo responsabile.
    /// </summary>
    [Serializable()]
    public class RegistroRepertorioPrint
    {
        public String CounterId { get; set; }
        public String CounterDescription { get; set; }
        public String TipologyDescription { get; set; }
        public String RegistryId { get; set; }
        public String RFId { get; set; }
        public Ruolo PrinterRole {get;set;}
        public Utente PrinterUser { get; set; }
        public DateTime NextAutomaticPrint { get; set; }
        public RegistroRepertorioSingleSettings.RepertorioState CounterState { get; set; }
        public RegistroRepertorioSingleSettings.Frequency PrintFrequency { get; set; }

    }
}
