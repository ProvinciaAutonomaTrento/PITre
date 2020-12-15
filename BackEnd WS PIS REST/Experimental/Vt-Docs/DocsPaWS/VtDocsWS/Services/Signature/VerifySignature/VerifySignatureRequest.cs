using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Signature.VerifySignature
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "VerifySignatureRequest"
    /// </summary>
    [DataContract]
    public class VerifySignatureRequest : Request
    {

        //TO DO: Capire quali parametri debbano essere passati alla requets per la firma.

        /// <summary>
        /// Id del documento di cui si desidera verificare la firma
        /// </summary>
        //[DataMember]
        //public string IdDocument
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// Version del documento di cui si desidera verificare la firma
        /// </summary>
        //[DataMember]
        //public string Version
        //{
        //    get;
        //    set;
        //}

        /*
         * I parametri dovrebbero essere i seguenti: Numero Documento e Versione; 
         * Andrebbe fatta la getFile (quella che fa il servizio getFileDocumentById) e con l'oggetto restituito andare a fare la logica
         * che abbiamo in VerificaValiditaFirma del .asmx (cioè prima getFile e poi VerifyFileSignature)
         */

        /// <summary>
        /// DocNumber del documento
        /// </summary>
        [DataMember]
        public string IdDocument
        {
            get;
            set;
        }

        /// <summary>
        /// Id della versione, opzionale, se vuoto prende l'ultima versione
        /// </summary>
        [DataMember]
        public string VersionId
        {
            get;
            set;
        }
    }
}