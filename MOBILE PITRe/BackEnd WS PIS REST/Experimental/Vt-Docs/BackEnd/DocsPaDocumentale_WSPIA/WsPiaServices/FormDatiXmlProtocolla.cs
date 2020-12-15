using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaDocumentale_WSPIA
{
    public class FormDatiXmlProtocolla
    {
        public List<DestinatarioXml> listaDestinatari = new List<DestinatarioXml>();

        public List<DestinatarioXml> listaDestinatariCC = new List<DestinatarioXml>();

        public List<DocumentoXml> listaDocumenti = new List<DocumentoXml>();

        public string categoriaProtocollo
        {
            get;
            set;
        }

        public string tipoMittente
        {
            get;
            set;

        }


        public string classifica
        {
            get;
            set;
        }

        public string nominativoMittente
        {
            get;
            set;
        }

        public string codiceMittente
        {
            get;
            set;
        }

        //altre proprietà mancanti
    }
}
