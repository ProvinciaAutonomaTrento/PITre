using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Signature.VerifySignature
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "VerifySignatureResponse"
    /// </summary>
    [DataContract]
    public class VerifySignatureResponse : Response
    {

        //TO DO: Capire quali parametri debbano essere ritornati nella response per la firma.
        /*
         * il valore di ritorno di questa response è FileDocumento (quello che ritorna il metodo VerificaValiditaFirma)
         */

        /// <summary>
        /// File del documento richiesto con in più i dati di firma
        /// </summary>
        [DataMember]
        public Domain.FileDoc.FileDoc FileDoc
        {
            get;
            set;
        }

    }
}