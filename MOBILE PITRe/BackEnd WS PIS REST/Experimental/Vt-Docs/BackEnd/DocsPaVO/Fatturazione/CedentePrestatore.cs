using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Fatturazione
{
    [Serializable()]
    public class CedentePrestatore
    {
        // Dati Anagrafici
        public string idPaese;
        public string idCodice;
        public string denominazione;

        // Sede
        public string indirizzo;
        public string numCivico;
        public string CAP;
        public string comune;
        public string provincia;
        public string nazione;

        // Iscrizione REA
        public string ufficio;
        public string numeroREA;
        public string capitaleSociale;
        public string socioUnico;
        public string statoLiquidazione;
    }
}