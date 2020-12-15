using System;

namespace DocsPaVO.documento
{
    /// <summary>
    /// Questa classe fornisce informazioni di base relative ad un documento
    /// </summary>
    [Serializable()]
    public class BaseInfoDoc
    {
        /// <summary>
        /// Il nome del documento (equivale al docNumber per i non protocollati
        /// e alla segnatura per i protocollati)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indica se il file per questo documento è stato acquisito
        /// </summary>
        public bool HaveFile { get; set; }

        /// <summary>
        /// La dimensione del file
        /// </summary>
        public int FileSize { get; set; }

        /// <summary>
        /// La descrizione/oggetto del documento
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Il numero di versione del documento caricato
        /// </summary>
        public int VersionNumber { get; set; }

        /// <summary>
        /// Valore booleano utilizzato per indicare se il documento
        /// è un protocollo
        /// </summary>
        public bool IsProto { get; set; }

        /// <summary>
        /// Il path del documento
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Il nome del file
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Il nome del file originale
        /// </summary>
        public string OriginalFileName { get; set; }

        /// <summary>
        /// L'id della versione
        /// </summary>
        public string VersionId { get; set; }

        /// <summary>
        /// Indica se il documento è firmato
        /// </summary>
        public string Firmato { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string VersionLabel { get; set; }

        /// <summary>
        /// Il numero del documento
        /// </summary>
        public string DocNumber { get; set; }

        /// <summary>
        /// System id del documento
        /// </summary>
        public string IdProfile { get; set; }

        /// <summary>
        /// Valore booleano che indica se queste informazioni fanno riferimento 
        /// ad un allegato
        /// </summary>
        public bool IsAttachment { get; set; }

        /// <summary>
        /// L'allegato è un allegato di PEC
        /// </summary>
        public bool IsPecAttachment { get; set; }

    }
}
