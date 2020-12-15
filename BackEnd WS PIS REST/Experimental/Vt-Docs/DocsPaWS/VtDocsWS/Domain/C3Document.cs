using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain
{
    /// <summary>
    /// Documento
    /// </summary>
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class C3Document
    {
        /// <summary>
        /// System id del documento
        /// </summary>
        [DataMember]
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        ///Docnumber del documento
        /// </summary>
        [DataMember]
        public string DocNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Valore dell'oggetto del documento
        /// </summary>
        [DataMember]
        public string Object
        {
            get;
            set;
        }

        /// <summary>
        /// Data di creazione del documento
        /// </summary>
        [DataMember]
        public string CreationDate
        {
            get;
            set;
        }

        /// <summary>
        /// Tipo del documento A/P/I/G
        /// </summary>
        [DataMember]
        public string DocumentType
        {
            get;
            set;
        }

        ///// <summary>
        ///// Indica se il documento è privato 
        ///// </summary>
        //[DataMember]
        //public bool PrivateDocument
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// Indica se il documento è personale 
        ///// </summary>
        //[DataMember]
        //public bool PersonalDocument
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// Indica se il documento è un allegato
        ///// </summary>
        //[DataMember]
        //public bool IsAttachments
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// Indica se il documento è un predispoto
        ///// </summary>
        //[DataMember]
        //public bool Predisposed
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// Segnatura del protocollo
        /// </summary>
        [DataMember]
        public string Signature
        {
            get;
            set;
        }


        ///// <summary>
        ///// Indica se il protocollo è stato annullato
        ///// </summary>
        //[DataMember]
        //public bool Annulled
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// Indica la data di annullamento.
        /// </summary>
        //[DataMember]
        //public string AnnulmentDate
        //{
        //    get;
        //    set;
        //}


        ///// <summary>
        ///// Indica la descrizione del mezzo di spedizione
        ///// </summary>
        //[DataMember]
        //public string MeansOfSending
        //{
        //    get;
        //    set;
        //}


        ///// <summary>
        ///// Indica se il documento è nel cestino
        ///// </summary>
        //[DataMember]
        //public bool InBasket
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// Indica la data di protocollazione
        /// </summary>
        [DataMember]
        public string ProtocolDate
        {
            get;
            set;
        }

        ///// <summary>
        ///// Indica, se il documento è consolidato, lo stato
        ///// </summary>
        //[DataMember]
        //public string ConsolidationState
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// Indica il protocollo mittente nel caso di un protocollo in arrivo
        ///// </summary>
        //[DataMember]
        //public string ProtocolSender
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// Indica la data del protocollo mittente
        ///// </summary>
        //[DataMember]
        //public string DataProtocolSender
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// Indica la data di arrivo nel caso di un protocollo in arrivo
        ///// </summary>
        //[DataMember]
        //public string ArrivalDate
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// Mittente nel caso di protocollo
        /// </summary>
        [DataMember]
        public Domain.Correspondent Sender
        {
            get;
            set;
        }

        /// <summary>
        /// Destinatari del protocollo
        /// </summary>
        [DataMember]
        public Domain.Correspondent[] Recipients
        {
            get;
            set;
        }

        /// <summary>
        /// Destinatari in conoscenza
        /// </summary>
        [DataMember]
        public Correspondent[] RecipientsCC
        {
            get;
            set;
        }

        /// <summary>
        /// Mittenti mnultipli
        /// </summary>
        [DataMember]
        public Correspondent[] MultipleSenders
        {
            get;
            set;
        }

        /// <summary>
        /// Template associato al documento
        /// </summary>
        [DataMember]
        public Template Template
        {
            get;
            set;
        }

        ///// <summary>
        ///// Note del documentos
        ///// </summary>
        //[DataMember]
        //public Note[] Note
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// Documento principale
        /// </summary>
        [DataMember]
        public C3File MainDocument
        {
            get;
            set;
        }

        /// <summary>
        /// Allegati del documento
        /// </summary>
        [DataMember]
        public C3File[] Attachments
        {
            get;
            set;
        }

        /// <summary>
        /// Nel caso di protocollo registro in cui è protocollato
        /// </summary>
        [DataMember]
        public Register Register
        {
            get;
            set;
        }

        ///// <summary>
        ///// Nel caso il documento sia un allegato è l'id del documento al quale è associato l'allegato
        ///// </summary>
        //[DataMember]
        //public string IdParent
        //{
        //    get;
        //    set;
        //}
        /// <summary>
        /// Autore del documento
        /// </summary>
        [DataMember]
        public string Author { get; set; }

        /// <summary>
        /// Ruolo Autore del documento
        /// </summary>
        [DataMember]
        public string AuthorRole { get; set; }

        [DataMember]
        public string AuthorId { get; set; }

        [DataMember]
        public string AuthorRoleId { get; set; }

        /// <summary>
        /// UO Autrice del documento
        /// </summary>
        [DataMember]
        public string AuthorUO { get; set; }

        [DataMember]
        public string ProtocolNumber { get; set; }

        [DataMember]
        public string ProtocolYear { get; set; }
    }
}