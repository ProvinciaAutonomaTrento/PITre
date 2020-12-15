using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.Spedizione
{
    /// <summary>
    /// Configurazioni per la spedizione del documento
    /// </summary>
    [Serializable()]
    public class ConfigSpedizioneDocumento
    {
        /// <summary>
        /// Determina se è attiva per l'amministrazione la gestione della spedizione automatica dei documenti 
        /// </summary>
        public bool SpedizioneAutomaticaDocumento
        {
            get;
            set;
        }

        //*******************************************************
        // Giordano Iacozzilli 20/09/2012 
        // Ripristino della sola trasmissione in automatico ai 
        // destinatari interni nei protocolli in uscita
        //*******************************************************
        public bool TrasmissioneAutomaticaDocumento
        {
            get;
            set;
        }


        /// <summary>
        /// Se true indicherà di generare un messaggio all’utente in fase di creazione / spedizione del documento 
        /// avvisandolo nel caso in cui non sia stato acquisito il documento elettronico
        /// </summary>
        public bool AvvisaSuSpedizioneDocumento
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Possibili stati di una spedizione documento
    /// </summary>
    public enum StatiSpedizioneDocumentoEnum
    {
        DisabilitatoTrasmissioni, 
        DaSpedire,
        Spedito,
        ErroreInSpedizione,
        DaRispedire
    }

    /// <summary>
    /// Esito della spedizione di un documento ad un destinatario
    /// </summary>
    [Serializable()]
    public class StatoSpedizioneDocumento
    {
        /// <summary>
        /// Stato della spedizione
        /// </summary>
        public StatiSpedizioneDocumentoEnum Stato = StatiSpedizioneDocumentoEnum.DaSpedire;

        /// <summary>
        /// Riporta l'eventuale descrizione dello stato della spedizione
        /// </summary>
        public string Descrizione;
    }

    /// <summary>
    /// Rappresenta l'astrazione di una spedizione di un documento verso un destinatario
    /// </summary>
    [Serializable()]
    [XmlInclude(typeof(DestinatarioEsterno))]
    [XmlInclude(typeof(DestinatarioInterno))]
    public abstract class Destinatario
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
    public class DestinatarioInterno : Destinatario
    {
        /// <summary>
        /// Dati del corrispondente destinatario della spedizione
        /// </summary>
        public DocsPaVO.utente.Corrispondente DatiDestinatario;

        /// <summary>
        /// Corrispondente disabilitato alla ricezione delle trasmissioni 
        /// </summary>
        public bool DisabledTrasm = false;
    }

    /// <summary>
    /// Rappresenta un destinatario esterno interoperante cui viene spedito il documento
    /// </summary>
    [Serializable()]
    public class DestinatarioEsterno : Destinatario 
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Interoperante = false;

        public bool InteroperanteRGS = false;
        /// <summary>
        /// Destinatari della spedizione per interoperabilità
        /// </summary>
        [XmlArray()]
        [XmlArrayItem(typeof(DocsPaVO.utente.Corrispondente))]
        public List<DocsPaVO.utente.Corrispondente> DatiDestinatari = new List<DocsPaVO.utente.Corrispondente>();
    }

    /// <summary>
    /// Rappresenta il documento oggetto della spedizione
    /// </summary>
    [Serializable()]
    public class SpedizioneDocumento
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
        [XmlArrayItem(typeof(DestinatarioInterno))]
        public List<DestinatarioInterno> DestinatariInterni = new List<DestinatarioInterno>();

        /// <summary>
        /// Destinatari interoperanti della spedizione del documento
        /// </summary>
        [XmlArray()]
        [XmlArrayItem(typeof(DestinatarioEsterno))]
        public List<DestinatarioEsterno> DestinatariEsterni = new List<DestinatarioEsterno>();

        //Andrea
        public List<string> listaDestinatariNonRaggiungibili = new List<string>();
        //End Andrea

        public FlussoAutomatico.Messaggio tipoMessaggio = new FlussoAutomatico.Messaggio();
    }
}
