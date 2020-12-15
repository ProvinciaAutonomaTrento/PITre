using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento
{
    /// <summary>
    /// </summary>
    [XmlInclude(typeof(VerifySignatureResult))]
    [Serializable()]
    public class FileDocumentoAnteprima : FileDocumento
    {
        public int firstPg;
        public int lastPg;
        public int numPg;
        public int totalPg;
        public bool inFileSystem;

        public void Import(FileDocumento fd)
        {
            this.cartaceo = fd.cartaceo;
            this.name = fd.name;
		    this.path = fd.path;
		    this.fullName = fd.fullName;
		    this.content = fd.content;
		    this.length = fd.length;
		    this.contentType = fd.contentType;
		    this.estensioneFile = fd.estensioneFile;
            this.nomeOriginale = fd.nomeOriginale;
		    this.LabelPdf = fd.LabelPdf;
		    this.signatureResult = fd.signatureResult;
            this.msgErr  = fd.msgErr;
            this.timestampResult = fd.timestampResult;
            this.bypassFileContentValidation = fd.bypassFileContentValidation;

        }
    }
}
