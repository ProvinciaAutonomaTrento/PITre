using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Domain
{
    public class Document
    {
        /// <summary>
        /// System id del documento
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        ///Docnumber del documento
        /// </summary>
        public string DocNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Valore dell'oggetto del documento
        /// </summary>
        public string Object
        {
            get;
            set;
        }

        /// <summary>
        /// Data di creazione del documento
        /// </summary>
        public string CreationDate
        {
            get;
            set;
        }

        /// <summary>
        /// Tipo del documento A/P/I/G
        /// </summary>
        public string DocumentType
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se il documento è privato 
        /// </summary>
        public bool PrivateDocument
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se il documento è personale 
        /// </summary>
        public bool PersonalDocument
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se il documento è un allegato
        /// </summary>
        public bool IsAttachments
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se il documento è un predispoto
        /// </summary>
        public bool Predisposed
        {
            get;
            set;
        }

        /// <summary>
        /// Segnatura del protocollo
        /// </summary>
        public string Signature
        {
            get;
            set;
        }


        /// <summary>
        /// Indica se il protocollo è stato annullato
        /// </summary>
        public bool Annulled
        {
            get;
            set;
        }

        /// <summary>
        /// Indica la data di annullamento.
        /// </summary>
        //        //public string AnnulmentDate
        //{
        //    get;
        //    set;
        //}


        /// <summary>
        /// Indica la descrizione del mezzo di spedizione
        /// </summary>
        public string MeansOfSending
        {
            get;
            set;
        }


        /// <summary>
        /// Indica se il documento è nel cestino
        /// </summary>
        public bool InBasket
        {
            get;
            set;
        }

        /// <summary>
        /// Indica la data di protocollazione
        /// </summary>
        public string ProtocolDate
        {
            get;
            set;
        }

        public string ProtocolNumber { get; set; }
        public string ProtocolYear { get; set; }

        /// <summary>
        /// Indica, se il documento è consolidato, lo stato
        /// </summary>
        public string ConsolidationState
        {
            get;
            set;
        }

        /// <summary>
        /// Indica il protocollo mittente nel caso di un protocollo in arrivo
        /// </summary>
        public string ProtocolSender
        {
            get;
            set;
        }

        /// <summary>
        /// Indica la data del protocollo mittente
        /// </summary>
        public string DataProtocolSender
        {
            get;
            set;
        }

        /// <summary>
        /// Indica la data di arrivo nel caso di un protocollo in arrivo
        /// </summary>
        public string ArrivalDate
        {
            get;
            set;
        }

        /// <summary>
        /// Mittente nel caso di protocollo
        /// </summary>
        public Domain.Correspondent Sender
        {
            get;
            set;
        }

        /// <summary>
        /// Destinatari del protocollo
        /// </summary>
        public Domain.Correspondent[] Recipients
        {
            get;
            set;
        }

        /// <summary>
        /// Destinatari in conoscenza
        /// </summary>
        public Correspondent[] RecipientsCC
        {
            get;
            set;
        }

        /// <summary>
        /// Mittenti mnultipli
        /// </summary>
        public Correspondent[] MultipleSenders
        {
            get;
            set;
        }

        /// <summary>
        /// Template associato al documento
        /// </summary>
        public Template Template
        {
            get;
            set;
        }

        /// <summary>
        /// Note del documentos
        /// </summary>
        public Note[] Note
        {
            get;
            set;
        }

        /// <summary>
        /// Documento principale
        /// </summary>
        public File MainDocument
        {
            get;
            set;
        }

        /// <summary>
        /// Allegati del documento
        /// </summary>
        public File[] Attachments
        {
            get;
            set;
        }

        /// <summary>
        /// Nel caso di protocollo registro in cui è protocollato
        /// </summary>
        public Register Register
        {
            get;
            set;
        }

        /// <summary>
        /// Nel caso il documento sia un allegato è l'id del documento al quale è associato l'allegato
        /// </summary>
        public string IdParent
        {
            get;
            set;
        }

        public LinkedDocument ParentDocument { get; set; }

        public LinkedDocument[] LinkedDocuments { get; set; }
    }
}