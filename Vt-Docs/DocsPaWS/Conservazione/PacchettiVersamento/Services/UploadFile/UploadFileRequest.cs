//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.UploadFile
//{
//    /// <summary>
//    /// Oggetto contenente i dati di richiesta forniti al servizio di "UploadFile"
//    /// </summary>
//    [Serializable()]
//    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/UploadFileRequest")]
//    public class UploadFileRequest : Request
//    {
//        /// <summary>
//        /// Identificativo del documento cui associare il file
//        /// </summary>
//        public string IdDocumento
//        {
//            get;
//            set;
//        }

//        /// <summary>
//        /// Indica se prevedere o meno la validazione bloccante del file acquisito
//        /// </summary>
//        public bool ValidaContenutoFile
//        {
//            get;
//            set;
//        }

//        /// <summary>
//        /// Nome del file
//        /// </summary>
//        public string Nome
//        {
//            get;
//            set;
//        }

//        /// <summary>
//        /// Contenuto binario del file
//        /// </summary>
//        public byte[] Contenuto
//        {
//            get;
//            set;
//        }

//        /// <summary>
//        /// Tipo del file
//        /// </summary>
//        public string TipoMime
//        {
//            get;
//            set;
//        }
//    }
//}