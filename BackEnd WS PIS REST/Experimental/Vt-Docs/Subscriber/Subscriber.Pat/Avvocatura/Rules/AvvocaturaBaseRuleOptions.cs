using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.Pat.Avvocatura.Rules
{
    /// <summary>
    /// Opzioni configurabili per tutte le regole dell'avvocatura
    /// </summary>
    [Serializable()]
    public class AvvocaturaBaseRuleOptions
    {
        /// <summary>
        /// Opzione contenente il nome del campo del profilo cui si applica la regola
        /// </summary>
        /// <remarks>
        /// Obbilgatoria
        /// </remarks>
        private const string OPTION_FIELD = "Campo";

        /// <summary>
        /// Opzione contenente il numero di giorni o mesi di decorrenza per il calcolo della scadenza
        /// </summary>
        /// <remarks>
        /// Opzionale
        /// </remarks>
        private const string OPTION_DECORRENZA = "Decorrenza";

        /// <summary>
        /// Opzione contenente il tipo di decorrenza per il calcolo della scadenza, ovvero giorni o mesi
        /// </summary>
        /// <remarks>
        /// Opzionale
        /// </remarks>
        private const string OPTION_TIPO_DECORRENZA = "Tipo decorrenza";

        /// <summary>
        /// Opzione contenente il numero di giorni o mesi di decorrenza per il calcolo dell'alert solitamente antecedente la scadenza
        /// </summary>
        /// <remarks>
        /// Opzionale
        /// </remarks>
        private const string OPTION_GIORNI_DECORRENZA_AVVISO = "Giorni decorrenza avviso";

        /// <summary>
        /// Opzione contenente la descrizione dell'avviso visualizzato nell'alert
        /// </summary>
        private const string OPTION_DESCRIZIONE_AVVISO = "Descrizione avviso";

        /// <summary>
        /// Opzione contenente l'ora di inizio dell'evento
        /// </summary>
        /// <remarks>
        /// Opzionale
        /// </remarks>
        private const string OPTION_ORA_INIZIO = "Ora inizio";

        /// <summary>
        /// Opzione contenente la durata in ore dell'evento
        /// </summary>
        /// <remarks>
        /// Opzionale
        /// </remarks>        
        private const string OPTION_DURATA_ORE = "Durata ore";

        /// <summary>
        /// 
        /// </summary>
        public enum TipoDecorrenzaEnum
        {
            Giorni,
            Mesi,
        }

        /// <summary>
        /// 
        /// </summary>
        public AvvocaturaBaseRuleOptions()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rule"></param>
        public AvvocaturaBaseRuleOptions(BaseRuleInfo rule)
        {
            this.Campo = rule.GetOptionByName(OPTION_FIELD, true);

            int decorrenza;
            Int32.TryParse(rule.GetOptionByName(OPTION_DECORRENZA, false), out decorrenza);
            this.Decorrenza = decorrenza;

            string tipoDecorrenza = rule.GetOptionByName(OPTION_TIPO_DECORRENZA, false);

            if (!string.IsNullOrEmpty(tipoDecorrenza))
            {
                this.TipoDecorrenza = (TipoDecorrenzaEnum) Enum.Parse(typeof(TipoDecorrenzaEnum), tipoDecorrenza, true);
            }

            int giorniDecorrenzaAvviso;
            Int32.TryParse(rule.GetOptionByName(OPTION_GIORNI_DECORRENZA_AVVISO, false), out giorniDecorrenzaAvviso);
            this.GiorniDecorrenzaAvviso = giorniDecorrenzaAvviso;

            if (giorniDecorrenzaAvviso != 0)
            {
                this.DescrizioneDecorrenzaAvviso = rule.GetOptionByName(OPTION_DESCRIZIONE_AVVISO, false);
            }

            int oraInizio;
            Int32.TryParse(rule.GetOptionByName(OPTION_ORA_INIZIO, false), out oraInizio);
            this.OraInizio = oraInizio;

            int durataOre;
            Int32.TryParse(rule.GetOptionByName(OPTION_DURATA_ORE, false), out durataOre);
            this.OreDurataEvento = durataOre;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Campo
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public TipoDecorrenzaEnum TipoDecorrenza
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Decorrenza
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int GiorniDecorrenzaAvviso
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string DescrizioneDecorrenzaAvviso
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int OraInizio
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int OreDurataEvento
        {
            get;
            set;
        }
    }
}
