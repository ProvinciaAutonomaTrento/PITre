using System;
using System.Collections.Generic;
using DocsPaVO.utente;
using DocsPaVO.Report;

namespace DocsPaVO.Modelli_Trasmissioni
{
    /// <summary>
    /// Questa classe rappresenta una versione leggera del modello di trasmissione utilizzata
    /// per la ricerca dei modelli di trasmissione.
    /// </summary>
    [Serializable()]
    public class ModelloTrasmissioneSearchResult
    {
        public enum ModelloTrasmissioneSearchResultSynthetic
        {
            OK,
            KO
        }

        /// <summary>
        /// Costruttore no args richiesto per serializzazione oggetto
        /// </summary>
        public ModelloTrasmissioneSearchResult() { }

        /// <summary>
        /// Inizializzazione di un risultato della ricerca del modello di trasmissione
        /// </summary>
        /// <param name="modelId">Id del modello</param>
        /// <param name="description">Descrizione del modello</param>
        public ModelloTrasmissioneSearchResult(String modelId, String description)
        {
            this.IdModello = modelId;
            this.DescrizioneModello = description;
            this.Destinatari = new List<DestinatarioLite>();
            this.Mittenti = new List<MittenteLite>();
            this.CodiceModello = String.Format("MT_{0}", modelId);
 
        }

        /// <summary>
        /// Identificativo del modello
        /// </summary>
        public String IdModello { get; set; }

        /// <summary>
        /// Codice del modello
        /// </summary>
        [PropertyToExport(Name="Codice Modello", Type=typeof(String))]
        public String CodiceModello { get; set; }

        /// <summary>
        /// Descrizione del modello di trasmissione
        /// </summary>
        [PropertyToExport(Name="Descrizione modello", Type=typeof(String))]
        public String DescrizioneModello { get; set; }

        /// <summary>
        /// Lista dei mittenti
        /// </summary>
        public List<MittenteLite> Mittenti { get; set; }

        /// <summary>
        /// Lista dei destinatari
        /// </summary>
        public List<DestinatarioLite> Destinatari { get; set; }

        /// <summary>
        /// Messaggio 
        /// </summary>
        [PropertyToExport(Name="Messaggio", Type=typeof(String))]
        public String Message { get; set; }

        /// <summary>
        /// Esito sintetico dell'operazione
        /// </summary>
        [PropertyToExport(Name="Esito", Type=typeof(String))]
        public ModelloTrasmissioneSearchResultSynthetic SyntheticResult { get; set; }
       
    }

    /// <summary>
    /// Destinatario di un modello di trasmissione
    /// </summary>
    [Serializable()]
    public class DestinatarioLite
    {
        /// <summary>
        /// Tipo di trasmissione
        /// </summary>
        public String TipoTrasmissione { get; set; }

        /// <summary>
        /// Id del destinatario della trasmissione
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// Ruolo destinatario della trasmissione
        /// </summary>
        public Ruolo Corrispondente { get; set; }

    }

    /// <summary>
    /// Mittente di un modello di trasmissione
    /// </summary>
    [Serializable()]
    public class MittenteLite
    {
        /// <summary>
        /// Id del mittente
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// Livello occupato dal corrispondente all'interno dell'organigramma
        /// </summary>
        public String Livel { get; set; }

    }

}
