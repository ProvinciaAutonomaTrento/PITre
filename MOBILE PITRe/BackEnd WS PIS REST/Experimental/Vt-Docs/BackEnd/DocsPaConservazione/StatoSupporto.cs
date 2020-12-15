using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaConservazione
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public sealed class StatoSupporto
    {
        /// <summary>
        /// 
        /// </summary>
        public string Codice
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Descrizione
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public static StatoSupporto[] Stati
        {
            get
            {
                List<StatoSupporto> stati = new List<StatoSupporto>();
                
                stati.Add(new StatoSupporto { Codice = StatoSupporto.DA_PRODURRE, Descrizione = "Da produrre" });
                stati.Add(new StatoSupporto { Codice = StatoSupporto.DA_REGISTRARE, Descrizione = "Da registrare" });
                stati.Add(new StatoSupporto { Codice = StatoSupporto.REGISTRATO, Descrizione = "Registrato" });
                stati.Add(new StatoSupporto { Codice = StatoSupporto.DA_VERIFICARE, Descrizione = "Da verificare" });
                stati.Add(new StatoSupporto { Codice = StatoSupporto.IN_VERIFICA, Descrizione = "In verifica" });
                stati.Add(new StatoSupporto { Codice = StatoSupporto.VERIFICATO, Descrizione = "Verificato" });
                stati.Add(new StatoSupporto { Codice = StatoSupporto.DANNEGGIATO, Descrizione = "Danneggiato" });

                return stati.ToArray();
            }
        }

        /// <summary>
        /// Supporto in stato "DaProdurre"
        /// </summary>
        public const string DA_PRODURRE = "L";

        /// <summary>
        // Supporto rimovibile in stato "DaRegistrare"
        /// </summary>
        public const string DA_REGISTRARE = "G";

        /// <summary>
        /// Supporto rimovibile in stato "Registrato"
        /// </summary>
        public const string REGISTRATO = "R";

        /// <summary>
        /// Supporto in stato "DaVerificare"
        /// </summary>
        public const string DA_VERIFICARE = "A";

        /// <summary>
        /// Supporto in stato "InVerifica"
        /// </summary>
        public const string IN_VERIFICA = "I";

        /// <summary>
        /// Supporto in stato "Verificato"
        /// </summary>
        public const string VERIFICATO = "V";

        /// <summary>
        /// Supporto in stato "Danneggiato"
        /// </summary>
        public const string DANNEGGIATO = "D";
    }
}
