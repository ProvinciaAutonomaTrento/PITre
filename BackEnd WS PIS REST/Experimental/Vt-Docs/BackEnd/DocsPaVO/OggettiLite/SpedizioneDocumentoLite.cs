using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using DocsPaVO.Spedizione;


namespace DocsPaVO.OggettiLite
{

    /// <summary>
    /// Rappresenta l'astrazione di una spedizione di un documento verso un destinatario
    /// </summary>
    [Serializable()]
    [XmlInclude(typeof(DestinatarioEsternoLite))]
    [XmlInclude(typeof(DestinatarioInternoLite))]
    public abstract class DestinatarioLite
    {
        /// <summary>
        /// Identificativo univoco del destinatario
        /// </summary>
        public string Id;

        /// <summary>
        /// Data e ora dell'ultima spedizione / trasmissione
        /// </summary>
        /// <remarks>
        /// Se non valorizzata, al destinatario non è mai stato spedito il documento
        /// </remarks>
        public string DataUltimaSpedizione;

        /// <summary>
        /// Indica l'indirizzo email di spedizione del documento
        /// </summary>
        public string Email;

        /// <summary>
        /// Indica se includere il soggetto tra i destinatari
        /// in fase di spedizione di un documento
        /// </summary>
        public bool IncludiInSpedizione;

        /// <summary>
        /// Stato della spedizione di un documento al destinatario
        /// </summary>
        public StatoSpedizioneDocumento StatoSpedizione = new StatoSpedizioneDocumento();
    }

    /// <summary>
    /// Rappresenta un destinatario interno cui il documento verrà trasmesso
    /// </summary>
    [Serializable()]
    public class DestinatarioInternoLite : DestinatarioLite
    {
        /// <summary>
        /// Dati del corrispondente destinatario della spedizione
        /// </summary>
        public DocsPaVO.OggettiLite.CorrispondenteLite DatiDestinatario;

        /// <summary>
        /// Corrispondente disabilitato alla ricezione delle trasmissioni 
        /// </summary>
        public bool DisabledTrasm = false;
    }

    /// <summary>
    /// Rappresenta un destinatario esterno interoperante cui viene spedito il documento
    /// </summary>
    [Serializable()]
    public class DestinatarioEsternoLite : DestinatarioLite 
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Interoperante = false;

        /// <summary>
        /// Destinatari della spedizione per interoperabilità
        /// </summary>
        [XmlArray()]
        [XmlArrayItem(typeof(DocsPaVO.OggettiLite.CorrispondenteLite))]
        public List<DocsPaVO.OggettiLite.CorrispondenteLite> DatiDestinatari = new List<DocsPaVO.OggettiLite.CorrispondenteLite>();
    }

    /// <summary>
    /// Rappresenta il documento oggetto della spedizione
    /// </summary>
    [Serializable()]
    public class SpedizioneDocumentoLite
    {
        /// <summary>
        /// Identificativo univoco del documento
        /// </summary>
        public string IdDocumento;

        /// <summary>
        /// Indica se il documento è stato spedito almeno una volta ad uno o più destinatari
        /// </summary>
        public bool Spedito;

        /// <summary>
        /// Indica l'id del registro o dell'RF da cui spedire il documento ai destinatari esterni
        /// </summary>
        public string IdRegistroRfMittente;

        /// <summary>
        /// Rappresenta l'indirizzo di posta associato al registro/rf selezionato da utilizzare per la spedizione
        /// </summary>
        public string mailAddress = string.Empty;

        /// <summary>
        /// Destinatari interni destinatari della trasmissione del documento
        /// </summary>
        [XmlArray()]
        [XmlArrayItem(typeof(DestinatarioInternoLite))]
        public List<DestinatarioInternoLite> DestinatariInterni = new List<DestinatarioInternoLite>();

        /// <summary>
        /// Destinatari interoperanti della spedizione del documento
        /// </summary>
        [XmlArray()]
        [XmlArrayItem(typeof(DestinatarioEsternoLite))]
        public List<DestinatarioEsternoLite> DestinatariEsterni = new List<DestinatarioEsternoLite>();

        //Andrea
        public List<string> listaDestinatariNonRaggiungibili = new List<string>();
        //End Andrea
    }


}
