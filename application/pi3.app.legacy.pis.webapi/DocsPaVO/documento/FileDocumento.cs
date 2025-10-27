// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento 
{
    /// <summary>
    /// </summary>
    [XmlInclude(typeof(BigFileDocumento))]
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
        public string nomeOriginale;
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

    
    [Serializable()]
    public class BigFileDocumento : FileDocumento
    {
        public string BigFilePath { get; set; }

        public BigFileDocumento() { } // creato perch� la serializzazione soap XML necessita di un costruttore senza parametri

        public BigFileDocumento(FileDocumento documento, string filePath)
        {
            this.name = documento.name;
            this.path = documento.path;
            this.fullName = documento.fullName;
            this.content = documento.content;
            this.length = documento.length;
            this.contentType = documento.contentType;
            this.estensioneFile = documento.estensioneFile;
            this.nomeOriginale = documento.nomeOriginale;
            this.LabelPdf = documento.LabelPdf;
            this.signatureResult = documento.signatureResult;
            this.msgErr = documento.msgErr;
            this.cartaceo = documento.cartaceo;
            this.timestampResult = documento.timestampResult;
            this.bypassFileContentValidation = documento.bypassFileContentValidation;

            this.BigFilePath = filePath;
        }
    }

}
