using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Deleghe;
using System.Globalization;
using DocsPaVO.utente;

namespace DocsPaVO.Mobile
{
    public class Delega : DelegaInfo
    {

        public DateTime DataDecorrenza
        {
            get; 
            set;
        }

        public DateTime DataScadenza
        {
            get; 
            set; 
        }

        public string InEsercizio
        {
            get; 
            set; 
        }

        public StatoDelega Stato
        {
            get; 
            set; 
        }

        public string DelegaStato
        {
            get;
            set;
        }

        public string Delegato
        {
            get; 
            set; 
        }

        public string IdDelegato
        {
            get; 
            set; 
        }

        public string RuoloDelegato
        {
            get; 
            set;
        }

        public string RuoloDelegante
        {
            get;
            set;
        }

        public string IdRuoloDelegato { get; set; }

        public static Delega buildInstance(InfoDelega input, Ruolo ruoloDelegato, Ruolo ruoloDelegante)
        {
            Delega res = new Delega();
            res.DataDecorrenza = toDate(input.dataDecorrenza);
            if (res.DataDecorrenza.CompareTo(DateTime.Now) > 0) { res.Stato = StatoDelega.IMPOSTATA; res.DelegaStato = "IMPOSTATA"; }
            
            if (!string.IsNullOrEmpty(input.dataScadenza))
            {
                res.DataScadenza = toDate(input.dataScadenza);          
                if (res.DataScadenza.CompareTo(DateTime.Now) < 0) { res.Stato = StatoDelega.SCADUTA; res.DelegaStato = "SCADUTA"; }
            }

            if (res.DataDecorrenza.CompareTo(DateTime.Now) < 0 && (string.IsNullOrEmpty(input.dataScadenza) || res.DataScadenza.CompareTo(DateTime.Now) > 0))
            {
                res.Stato = StatoDelega.ATTIVA; 
                res.DelegaStato = "ATTIVA";
            }
            res.Id = input.id_delega;
            res.Delegato = input.cod_utente_delegato;
            res.Delegante = input.cod_utente_delegante;
            res.CodiceDelegante = input.codiceDelegante;
            res.IdDelegato = input.id_utente_delegato;
            res.IdRuoloDelegato = input.id_ruolo_delegato;
            
            if (ruoloDelegato != null)
            {
                res.RuoloDelegato = ruoloDelegato.descrizione;
            }
            else
            {
                res.RuoloDelegato = "TUTTI";
            }

            if (ruoloDelegante != null)
            {
                res.RuoloDelegante = ruoloDelegante.descrizione;
            }
            else
            {
                res.RuoloDelegante = "TUTTI";
            }
            res.IdRuoloDelegante = input.id_ruolo_delegante;
            res.InEsercizio = input.inEsercizio;
            return res;
        }

        public InfoDelega InfoDelega{
            get
            {
                InfoDelega res = new InfoDelega();
                if (DataDecorrenza != null)
                {
                    res.dataDecorrenza = DataDecorrenza.ToString("dd/MM/yyyy HH.mm.ss");
                }
                else
                {
                    res.dataDecorrenza = string.Empty;
                }
                if (DataScadenza != null)
                {
                    res.dataScadenza = DataScadenza.ToString("dd/MM/yyyy HH.mm.ss");
                }
                else
                {
                    res.dataScadenza = string.Empty;
                }
                res.cod_utente_delegato = Delegato;
                res.cod_ruolo_delegato = RuoloDelegato;
                res.id_utente_delegato = IdDelegato;
                res.id_ruolo_delegante = IdRuoloDelegante;
                res.cod_utente_delegante = RuoloDelegante;
                res.id_delega = Id;
                return res;
            }
        }

        private static DateTime toDate(string date)
        {
            string[] formats = {"dd/MM/yyyy",
                                "dd/MM/yyyy HH:mm:ss",
								"dd/MM/yyyy h:mm:ss",
								"dd/MM/yyyy h.mm.ss",
								"dd/MM/yyyy HH.mm.ss"};
            return DateTime.ParseExact(date, formats, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces);
        }
    }

    public enum TipoDelega
    {
        ASSEGNATA, RICEVUTA, ESERCIZIO, TUTTI
    }

    public enum StatoDelega
    {
        ATTIVA,IMPOSTATA,SCADUTA, TUTTI
    }

}