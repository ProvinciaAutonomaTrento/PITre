using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.InsertDocumento
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta forniti al servizio di "InsertIstanza"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/InsertDocumentoRequest")]
    public class InsertDocumentoRequest : Request
    {
        ///// <summary>
        ///// Identificativo dell'istanza di conservazione in cui inserire il documento
        ///// </summary>
        //public string IdIstanza
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// Indica se inserire o meno il documento nell'area di conservazione contestualmente alla creazione
        /// </summary>
        public bool AddInIstanzaConservazione
        {
            get;
            set;
        }

        /// <summary>
        /// Dati del documento da inserire in conservazione
        /// </summary>
        public Dominio.Documento Documento
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se prevedere o meno la validazione bloccante del file acquisito
        /// </summary>
        public bool ValidaContenutoFile
        {
            get;
            set;
        }
    }
}