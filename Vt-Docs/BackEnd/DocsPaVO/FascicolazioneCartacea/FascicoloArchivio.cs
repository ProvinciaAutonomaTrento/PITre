using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaVO.FascicolazioneCartacea
{
    /// <summary>
    /// Rappresenta i dati generali di un fascicolo in archivio
    /// </summary>
    [Serializable()]
    public class FascicoloArchivio
    {
        /// <summary>
        /// Dati del fascicolo
        /// </summary>
        public int IdFascicolo = 0;
        public string CodiceFascicolo = string.Empty;
        public string DescrizioneFascicolo = string.Empty;

        /// <summary>
        /// Tipologia del fascicolo: generale o procedimentale
        /// </summary>
        public string TipoFascicolo = string.Empty;

        /// <summary>
        /// Indica se il per il fascicolo vi è
        /// un corrispondente cartaceo in archivio
        /// </summary>
        public bool Cartaceo = false;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Concat("Fascicolo ", this.CodiceFascicolo);
        }

        public FascicoloArchivio()
        { }

        public FascicoloArchivio(int idFascicolo, string codiceFascicolo, string descrizioneFascicolo)
        {
            this.IdFascicolo = idFascicolo;
            this.CodiceFascicolo = codiceFascicolo;
            this.DescrizioneFascicolo = descrizioneFascicolo;
        }

        public FascicoloArchivio(int idFascicolo, string codiceFascicolo, string descrizioneFascicolo, bool cartaceo) : this(idFascicolo, codiceFascicolo, descrizioneFascicolo)
        {
            this.Cartaceo = cartaceo;
        }
    }
}
