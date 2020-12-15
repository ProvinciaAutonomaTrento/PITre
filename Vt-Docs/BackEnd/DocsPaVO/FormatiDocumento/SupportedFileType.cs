using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaVO.FormatiDocumento
{
    /// <summary>
    /// Classe che definisce una tipologia di file supportato in docspa
    /// </summary>
    [Serializable()]
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
        public int SystemId = 0;

        /// <summary>
        /// Id dell'amministrazione
        /// </summary>
        public int IdAmministrazione = 0;

        /// <summary>
        /// Codice dell'amministrazione di appartenenza
        /// </summary>
        public string CodiceAmministrazione = string.Empty;

        /// <summary>
        /// Nome dell'applicazione proprietaria del file
        /// </summary>
        public string Description = string.Empty;

        ///// <summary>
        ///// MimeType del file
        ///// </summary>
        //public string MimeType = string.Empty;

        /// <summary>
        /// Estensione del file
        /// </summary>
        public string FileExtension = string.Empty;

        /// <summary>
        /// Dimensione massima del file acquisibile dal tipo di file
        /// </summary>
        public int MaxFileSize = 0;

        /// <summary>
        /// Modalità con cui si avvisa l'utente
        /// in merito alla dimensione massima del file 
        /// </summary>
        public MaxFileSizeAlertModeEnum MaxFileSizeAlertMode = MaxFileSizeAlertModeEnum.None;

        /// <summary>
        /// Indica se, per il tipo di file, è presente un modello di file predefinito
        /// </summary>
        public bool ContainsFileModel = false;

        /// <summary>
        /// Bytearray, contenuto binario del file modello predefinito per il tipo di documento
        /// </summary>
        public byte[] ModelFileContent = null;

        /// <summary>
        /// Tipo documento cui viene applicato il formato
        /// </summary>
        public DocumentTypeEnum DocumentType = DocumentTypeEnum.All;

        /// <summary>
        /// Indica se il tipo di file è utilizzato dall'amministrazione
        /// </summary>
        public bool FileTypeUsed = false;

        /// <summary>
        /// Indica se il tipo di file è ammesso alla firma
        /// </summary>
        public bool FileTypeSignature = false;

        /// <summary>
        /// Indica se il tipo di file è ammesso alla conservazione
        /// </summary>
        public bool FileTypePreservation = false;

        /// <summary>
        /// Indica se il tipo di file deve essere validato
        /// </summary>
        public bool FileTypeValidation= false;

        /// <summary>
        /// Indica se il file è convertibile
        /// </summary>
        public bool FileTypeConvertible = true;
    }

    /// <summary>
    /// Indica le modalità con cui si avvisa l'utente
    /// nel caso in cui si acquisisce un file di dimensione
    /// maggiore rispetto a quella massima prestabilita
    /// </summary>
    public enum MaxFileSizeAlertModeEnum
    {
        None,       // Nessun messaggio viene inviato all'utente,
                    // anche se il file è di dimensione maggiore rispetto a quella prestabilita
        
        Warning,    // Viene inviato un messaggio di warning all'utente,
                    // il file può comunque essere acquisito lo stesso
        
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
