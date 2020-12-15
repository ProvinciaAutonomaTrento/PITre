using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DocsPaWS.Hermes
{
    public class BandoSyncRequest
    {
        public string Mandante { get; set; }
        public int? NumeroGaraAppalto { get; set; } //questa è FK con Sap
        public string OrganizzazioneAcquisti { get; set; }
        public string GruppoAcquisti { get; set; }
        public string DescrizioneGara { get; set; }
        public DateTime DataScadenzaGara { get; set; }
        public DateTime DataEmissioneGara { get; set; }
        public DateTime DataAperturaBusteTecniche { get; set; }
        public DateTime DataRicezioneRelazioneTecnica { get; set; }
        public DateTime DataAperturaBusteEconomiche { get; set; }
        public DateTime DataPropostaAggiudicazioneTrattativa { get; set; }
        public DateTime DataAggiudicazione { get; set; }
        public DateTime DataAnnulamentoGara { get; set; }
        public int ValoreGara { get; set; }
        
    }
}