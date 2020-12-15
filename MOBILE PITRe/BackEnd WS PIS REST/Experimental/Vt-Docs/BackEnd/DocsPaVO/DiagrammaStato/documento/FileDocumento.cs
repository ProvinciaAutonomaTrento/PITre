using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento 
{
	/// <summary>
	/// </summary>
	[XmlInclude(typeof(VerifySignatureResult))]
    [Serializable()]
	public class FileDocumento 
	{
		public string name;
		public string path;
		public string fullName;
		public byte [] content;
		public int length;
		public string contentType;
		public string estensioneFile;
		public labelPdf LabelPdf = new labelPdf();
		// Metadati riguardanti la firma digitale del documento
		public VerifySignatureResult signatureResult;
        //messaggio di errore.
        public string msgErr = string.Empty;

        /// <summary>
        /// Se true, il documento ha un originale cartaceo
        /// </summary>
        public bool cartaceo = false;

        /// <summary>
        /// Metadati riguardanti la marca temporale
        /// </summary>
        public areaConservazione.OutputResponseMarca timestampResult;

        /// <summary>
        /// Se true, indica se evitare o meno il controllo relativo alla validazione del contenuto del file rispetto alla sua estenzione
        /// </summary>
        public bool bypassFileContentValidation = false;
	}
}
