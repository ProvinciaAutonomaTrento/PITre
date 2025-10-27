// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DocsPaVO.FormatiDocumento
{
    /// <summary>
    /// Classe che definisce una tipologia di file supportato in docspa
    /// </summary>
    [Serializable()]
    [DataContract]
    public class SupportedFileType
    {
        /// <summary>
        /// 
        /// </summary>
        public SupportedFileType()
        { }

        /// <summary>
        /// SystemId del file
        /// </summary>
        [DataMember]
        public int SystemId { get; set; } = 0;

        /// <summary>
        /// Id dell'amministrazione
        /// </summary>
        [DataMember]
        public int IdAmministrazione { get; set; } = 0;

        /// <summary>
        /// Codice dell'amministrazione di appartenenza
        /// </summary>
        [DataMember]
        public string CodiceAmministrazione { get; set; } = string.Empty;

        /// <summary>
        /// Nome dell'applicazione proprietaria del file
        /// </summary>
        [DataMember]
        public string Description { get; set; } = string.Empty;

        ///// <summary>
        ///// MimeType del file
        ///// </summary>
        //public string MimeType = string.Empty;

        /// <summary>
        /// Estensione del file
        /// </summary>
        [DataMember]
        public string FileExtension { get; set; } = string.Empty;

        /// <summary>
        /// Dimensione massima del file acquisibile dal tipo di file
        /// </summary>
        [DataMember]
        public int MaxFileSize { get; set; } = 0;

        /// <summary>
        /// Modalit� con cui si avvisa l'utente
        /// in merito alla dimensione massima del file 
        /// </summary>
        [DataMember]
        public MaxFileSizeAlertModeEnum MaxFileSizeAlertMode { get; set; } = MaxFileSizeAlertModeEnum.None;

        /// <summary>
        /// Indica se, per il tipo di file, � presente un modello di file predefinito
        /// </summary>
        [DataMember]
        public bool ContainsFileModel { get; set; } = false;

        /// <summary>
        /// Bytearray, contenuto binario del file modello predefinito per il tipo di documento
        /// </summary>
        [DataMember]
        public byte[] ModelFileContent { get; set; } = null;

        /// <summary>
        /// Tipo documento cui viene applicato il formato
        /// </summary>
        [DataMember]
        public DocumentTypeEnum DocumentType { get; set; } = DocumentTypeEnum.All;

        /// <summary>
        /// Indica se il tipo di file � utilizzato dall'amministrazione
        /// </summary>
        [DataMember]
        public bool FileTypeUsed { get; set; } = false;

        /// <summary>
        /// Indica se il tipo di file � ammesso alla firma
        /// </summary>
        [DataMember]
        public bool FileTypeSignature { get; set; } = false;

        /// <summary>
        /// Indica se il tipo di file � ammesso alla conservazione
        /// </summary>
        [DataMember]
        public bool FileTypePreservation { get; set; } = false;

        /// <summary>
        /// Indica se il tipo di file deve essere validato
        /// </summary>
        [DataMember]
        public bool FileTypeValidation { get; set; } = false;

        /// <summary>
        /// Indica se il file � convertibile
        /// </summary>
        [DataMember]
        public bool FileTypeConvertible { get; set; } = true;
    }

    /// <summary>
    /// Indica le modalit� con cui si avvisa l'utente
    /// nel caso in cui si acquisisce un file di dimensione
    /// maggiore rispetto a quella massima prestabilita
    /// </summary>
    public enum MaxFileSizeAlertModeEnum
    {
        None,       // Nessun messaggio viene inviato all'utente,
                    // anche se il file � di dimensione maggiore rispetto a quella prestabilita
        
        Warning,    // Viene inviato un messaggio di warning all'utente,
                    // il file pu� comunque essere acquisito lo stesso
        
        Error,      // Viene inviato un messaggio di errore all'utente
    }

    /// <summary>
    /// Tipologie di documenti cui viene applicato il formato
    /// </summary>
    public enum DocumentTypeEnum
    {
        All,        // Formato documento applicabile a tutti i tipi di documenti

        Grigio,     // Applicabile solo ai documenti grigi

        Protocollo,  // Applicabile solo ai protocolli
    }
}
