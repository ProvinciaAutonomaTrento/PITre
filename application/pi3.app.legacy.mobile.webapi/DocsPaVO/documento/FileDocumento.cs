// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DocsPaVO.documento 
{
    /// <summary>
    /// </summary>
    [XmlInclude(typeof(BigFileDocumento))]
    [XmlInclude(typeof(VerifySignatureResult))]
    [Serializable()]
    [DataContract]
    public class FileDocumento 
	{
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string path { get; set; }
        [DataMember]
        public string fullName { get; set; }
        [DataMember]
        public byte [] content { get; set; }
        [DataMember]
        public int length { get; set; }
        [DataMember]
        public string contentType { get; set; }
        [DataMember]
        public string estensioneFile { get; set; }
        [DataMember]
        public string nomeOriginale { get; set; }
        [DataMember]
        public labelPdf LabelPdf { get; set; } = new labelPdf();
        // Metadati riguardanti la firma digitale del documento
        [DataMember]
        public VerifySignatureResult signatureResult { get; set; }
        //messaggio di errore.
        [DataMember]
        public string msgErr { get; set; } = string.Empty;

        /// <summary>
        /// Se true, il documento ha un originale cartaceo
        /// </summary>
        [DataMember]
        public bool cartaceo { get; set; } = false;

        /// <summary>
        /// Metadati riguardanti la marca temporale
        /// </summary>
        [DataMember]
        public areaConservazione.OutputResponseMarca timestampResult { get; set; }

        /// <summary>
        /// Se true, indica se evitare o meno il controllo relativo alla validazione del contenuto del file rispetto alla sua estenzione
        /// </summary>
        [DataMember]
        public bool bypassFileContentValidation { get; set; } = false;
	}

    
    [Serializable()]
    [DataContract]
    public class BigFileDocumento : FileDocumento
    {
        [DataMember]
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
