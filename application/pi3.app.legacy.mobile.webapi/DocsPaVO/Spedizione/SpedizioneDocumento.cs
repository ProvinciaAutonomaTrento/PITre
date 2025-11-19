// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.Spedizione
{
    /// <summary>
    /// Configurazioni per la spedizione del documento
    /// </summary>
    [Serializable()]
    [DataContract]
    public class ConfigSpedizioneDocumento
    {
        /// <summary>
        /// Determina se è attiva per l'amministrazione la gestione della spedizione automatica dei documenti 
        /// </summary>
        [DataMember]
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
        [DataMember]
        public bool TrasmissioneAutomaticaDocumento
        {
            get;
            set;
        }


        /// <summary>
        /// Se true indicherà di generare un messaggio all’utente in fase di creazione / spedizione del documento 
        /// avvisandolo nel caso in cui non sia stato acquisito il documento elettronico
        /// </summary>
        [DataMember]
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
    [DataContract]
    public class SpedizioneDocumento
    {
        /// <summary>
        /// Identificativo univoco del documento
        /// </summary>
        [DataMember]
        public string IdDocumento { get; set; }

        /// <summary>
        /// Indica se il documento è stato spedito almeno una volta ad uno o più destinatari
        /// </summary>
        [DataMember]
        public bool Spedito { get; set; }

        /// <summary>
        /// Indica l'id del registro o dell'RF da cui spedire il documento ai destinatari esterni
        /// </summary>
        [DataMember]
        public string IdRegistroRfMittente { get; set; }

        /// <summary>
        /// Rappresenta l'indirizzo di posta associato al registro/rf selezionato da utilizzare per la spedizione
        /// </summary>
        [DataMember]
        public string mailAddress { get; set; } = string.Empty;

        /// <summary>
        /// Destinatari interni destinatari della trasmissione del documento
        /// </summary>
        [DataMember]
        [XmlArray()]
        [XmlArrayItem(typeof(DestinatarioInterno))]
        public List<DestinatarioInterno> DestinatariInterni { get; set; } = new List<DestinatarioInterno>();

        /// <summary>
        /// Destinatari interoperanti della spedizione del documento
        /// </summary>
        [DataMember]
        [XmlArray()]
        [XmlArrayItem(typeof(DestinatarioEsterno))]
        public List<DestinatarioEsterno> DestinatariEsterni { get; set; } = new List<DestinatarioEsterno>();

        //Andrea
        [DataMember]
        public List<string> listaDestinatariNonRaggiungibili { get; set; } = new List<string>();
        //End Andrea

        [DataMember]
        public FlussoAutomatico.Messaggio tipoMessaggio { get; set; } = new FlussoAutomatico.Messaggio();
    }
}
