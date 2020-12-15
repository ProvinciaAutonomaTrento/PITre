using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaDocumentale_WSPIA
{
   public class FormDatiXmlAllegato
    {
       public string categoriaProtocollo = string.Empty;

       public string tipoMittente = string.Empty;

       public string nominativoMittente = string.Empty;

       //Laura 10 Dicembre

       public List<DestinatarioXml> listaDestinatari = new List<DestinatarioXml>();

       public List<DestinatarioXml> listaDestinatariCC = new List<DestinatarioXml>();

       //Laura 10 Dicembre

       public string nomeFile = string.Empty;

       public string riservato = string.Empty;

       public string flagProtocollo = string.Empty;

       public string classifica = string.Empty;

       public string oggetto = string.Empty;


       //Laura 7 febbraio 2013
       public string codiceMittente = string.Empty;
       
    }
}
