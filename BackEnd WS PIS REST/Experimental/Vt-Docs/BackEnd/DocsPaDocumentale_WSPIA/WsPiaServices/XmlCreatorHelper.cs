using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;


namespace DocsPaDocumentale_WSPIA
{
    public static class XmlCreatorHelper
    {
        public static string CreaXmlProtocolla(FormDatiXmlProtocolla formDatiXml)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(docNode);

            XmlNode datiInvioNode = doc.CreateElement("DatiInvio");
            doc.AppendChild(datiInvioNode);
            //---

            XmlNode categioriaProtoNode = doc.CreateElement("CategoriaProtocollo");
            categioriaProtoNode.AppendChild(doc.CreateTextNode(formDatiXml.categoriaProtocollo));
            datiInvioNode.AppendChild(categioriaProtoNode);

            XmlNode mittenteNode = doc.CreateElement("Mittente");
            datiInvioNode.AppendChild(mittenteNode);
            //---

            mittente(doc, mittenteNode, formDatiXml);

            //+++

            XmlNode listaDestNode = doc.CreateElement("ListaDestinatari");
            datiInvioNode.AppendChild(listaDestNode);
            //---

            //verifico se la lista di destinatari è vuota
            if (formDatiXml.listaDestinatari.Count == 0)
            {
                XmlNode destNode = doc.CreateElement("Destinatario");
                listaDestNode.AppendChild(destNode);
                //---
                //...
                DestinatarioXml dest = new DestinatarioXml();
                destinatario(doc, destNode, dest);
            }



                //altrimenti, se la lista dei destinatari non è vuota, stampo il contenuto nell'xml
            else
            {

                foreach (DestinatarioXml dest in formDatiXml.listaDestinatari)
                {
                    XmlNode destNode = doc.CreateElement("Destinatario");
                    listaDestNode.AppendChild(destNode);
                    //---
                    //...

                    destinatario(doc, destNode, dest);
                    //+++

                }
            }

            if (formDatiXml.listaDestinatariCC.Count > 0)
            {
                foreach (DestinatarioXml dest in formDatiXml.listaDestinatariCC)
                {
                    XmlNode destNode = doc.CreateElement("Destinatario");
                    XmlAttribute ccAttr = doc.CreateAttribute("TipoInvio");
                    ccAttr.Value = "CC";
                    destNode.Attributes.Append(ccAttr);
                    listaDestNode.AppendChild(destNode);
                    //---
                    //...

                    destinatario(doc, destNode, dest);
                    //+++

                }
            }

            XmlNode listaDocNode = doc.CreateElement("ListaDocumenti");
            datiInvioNode.AppendChild(listaDocNode);
            //---


            //verifico se la lista di documenti è vuota
            if (formDatiXml.listaDocumenti.Count == 0)
            {
                XmlNode documentoNode = doc.CreateElement("Documento");
                listaDocNode.AppendChild(documentoNode);
                //---
                //...
                DocumentoXml docXml = new DocumentoXml();
                documentoPerProtocolla(doc, documentoNode, docXml);
            }

                //altrimenti, se la lista dei documenti non è vuota, stampo il contenuto nell'xml
            else
            {

                foreach (DocumentoXml docXml in formDatiXml.listaDocumenti)
                {
                    XmlNode documentoNode = doc.CreateElement("Documento");
                    listaDocNode.AppendChild(documentoNode);
                    //---

                    documentoPerProtocolla(doc, documentoNode, docXml);
                }
            }
            /*
            XmlNode listaSoggAffNode = doc.CreateElement("ListaSoggettiAfferenti");
            documentoNode.AppendChild(listaSoggAffNode);
            //...

            XmlNode soggAfferenteNode = doc.CreateElement("SoggettoAfferente");
            listaSoggAffNode.AppendChild(soggAfferenteNode);
            //---

            soggettoAfferente(doc, soggAfferenteNode, formDatiXml);
            */

            return doc.OuterXml;
        }


        public static void mittente(XmlDocument doc, XmlNode mittenteNode, FormDatiXmlProtocolla formDatiXml)
        {
            XmlNode tipoMittNode = doc.CreateElement("Tipo");
            tipoMittNode.AppendChild(doc.CreateTextNode(formDatiXml.tipoMittente));
            mittenteNode.AppendChild(tipoMittNode);

            XmlNode codiceMittNode = doc.CreateElement("Codice");
            codiceMittNode.AppendChild(doc.CreateTextNode(formDatiXml.codiceMittente));
            mittenteNode.AppendChild(codiceMittNode);

            XmlNode nominativoMittNode = doc.CreateElement("Nominativo");
            nominativoMittNode.AppendChild(doc.CreateTextNode(formDatiXml.nominativoMittente));
            mittenteNode.AppendChild(nominativoMittNode);

            XmlNode emailMittNode = doc.CreateElement("EMail");
            mittenteNode.AppendChild(emailMittNode);

            XmlNode infoMittNode = doc.CreateElement("InfoCittadino");
            mittenteNode.AppendChild(infoMittNode);
            //---

            XmlNode tipoNotifMittNode = doc.CreateElement("TipoNotifica");
            infoMittNode.AppendChild(tipoNotifMittNode);

            XmlNode livelloNotifMittNode = doc.CreateElement("LivelloNotifica");
            infoMittNode.AppendChild(livelloNotifMittNode);

            XmlNode telCasaMittNode = doc.CreateElement("TelefonoCasa");
            infoMittNode.AppendChild(telCasaMittNode);

            XmlNode telCellMittNode = doc.CreateElement("TelefonoCellulare");
            infoMittNode.AppendChild(telCellMittNode);

            XmlNode telUffMittNode = doc.CreateElement("TelefonoUfficio");
            infoMittNode.AppendChild(telUffMittNode);

            XmlNode faxMittNode = doc.CreateElement("Fax");
            infoMittNode.AppendChild(faxMittNode);

            XmlNode contPrefMittNode = doc.CreateElement("ContattoPreferito");
            infoMittNode.AppendChild(contPrefMittNode);

            XmlNode oraPrefMittNode = doc.CreateElement("OrarioPreferito");
            infoMittNode.AppendChild(oraPrefMittNode);

            XmlNode indirizzoMittNode = doc.CreateElement("Indirizzo");
            infoMittNode.AppendChild(indirizzoMittNode);

            XmlNode localitaMittNode = doc.CreateElement("Localita");
            infoMittNode.AppendChild(localitaMittNode);

            XmlNode infoAggMittNode = doc.CreateElement("InfoAggiuntive");
            infoMittNode.AppendChild(infoAggMittNode);

            XmlNode edificioMittNode = doc.CreateElement("Edificio");
            infoMittNode.AppendChild(edificioMittNode);

            XmlNode noteMittNode = doc.CreateElement("Note");
            infoMittNode.AppendChild(noteMittNode);
        }


        public static void destinatario(XmlDocument doc, XmlNode destinatarioNode, DestinatarioXml dest)
        {
            XmlNode tipoDestNode = doc.CreateElement("Tipo");
            tipoDestNode.AppendChild(doc.CreateTextNode(dest.tipoDestinatario));
            destinatarioNode.AppendChild(tipoDestNode);

            XmlNode codiceDestNode = doc.CreateElement("Codice");
            codiceDestNode.AppendChild(doc.CreateTextNode(dest.codiceDestinatario));
            destinatarioNode.AppendChild(codiceDestNode);

            XmlNode nominativoDestNode = doc.CreateElement("Nominativo");
            nominativoDestNode.AppendChild(doc.CreateTextNode(dest.nominativoDestinatario));
            destinatarioNode.AppendChild(nominativoDestNode);

            XmlNode emailDestNode = doc.CreateElement("EMail");
            destinatarioNode.AppendChild(emailDestNode);

            XmlNode infoDestNode = doc.CreateElement("InfoCittadino");
            destinatarioNode.AppendChild(infoDestNode);
            //---

            XmlNode tipoNotifDestNode = doc.CreateElement("TipoNotifica");
            infoDestNode.AppendChild(tipoNotifDestNode);

            XmlNode livelloNotifDestNode = doc.CreateElement("LivelloNotifica");
            infoDestNode.AppendChild(livelloNotifDestNode);

            XmlNode telCasaDestNode = doc.CreateElement("TelefonoCasa");
            infoDestNode.AppendChild(telCasaDestNode);

            XmlNode telCellDestNode = doc.CreateElement("TelefonoCellulare");
            infoDestNode.AppendChild(telCellDestNode);

            XmlNode telUffDestNode = doc.CreateElement("TelefonoUfficio");
            infoDestNode.AppendChild(telUffDestNode);

            XmlNode faxDestNode = doc.CreateElement("Fax");
            infoDestNode.AppendChild(faxDestNode);

            XmlNode contPrefDestNode = doc.CreateElement("ContattoPreferito");
            infoDestNode.AppendChild(contPrefDestNode);

            XmlNode oraPrefDestNode = doc.CreateElement("OrarioPreferito");
            infoDestNode.AppendChild(oraPrefDestNode);

            XmlNode indirizzoDestNode = doc.CreateElement("Indirizzo");
            infoDestNode.AppendChild(indirizzoDestNode);

            XmlNode localitaDestNode = doc.CreateElement("Localita");
            infoDestNode.AppendChild(localitaDestNode);

            XmlNode infoAggDestNode = doc.CreateElement("InfoAggiuntive");
            infoDestNode.AppendChild(infoAggDestNode);

            XmlNode edificioDestNode = doc.CreateElement("Edificio");
            infoDestNode.AppendChild(edificioDestNode);

            XmlNode noteDestNode = doc.CreateElement("Note");
            infoDestNode.AppendChild(noteDestNode);
        }


        public static void soggettoAfferente(XmlDocument doc, XmlNode soggAfferenteNode, FormDatiXmlProtocolla formDatiXml)
        {
            XmlNode tipoDestNode = doc.CreateElement("Tipo");
            soggAfferenteNode.AppendChild(tipoDestNode);

            XmlNode codiceDestNode = doc.CreateElement("Codice");
            soggAfferenteNode.AppendChild(codiceDestNode);

            XmlNode nominativoDestNode = doc.CreateElement("Nominativo");
            soggAfferenteNode.AppendChild(nominativoDestNode);

            XmlNode emailDestNode = doc.CreateElement("EMail");
            soggAfferenteNode.AppendChild(emailDestNode);

            XmlNode infoDestNode = doc.CreateElement("InfoCittadino");
            soggAfferenteNode.AppendChild(infoDestNode);
            //---

            XmlNode tipoNotifDestNode = doc.CreateElement("TipoNotifica");
            infoDestNode.AppendChild(tipoNotifDestNode);

            XmlNode livelloNotifDestNode = doc.CreateElement("LivelloNotifica");
            infoDestNode.AppendChild(livelloNotifDestNode);

            XmlNode telCasaDestNode = doc.CreateElement("TelefonoCasa");
            infoDestNode.AppendChild(telCasaDestNode);

            XmlNode telCellDestNode = doc.CreateElement("TelefonoCellulare");
            infoDestNode.AppendChild(telCellDestNode);

            XmlNode telUffDestNode = doc.CreateElement("TelefonoUfficio");
            infoDestNode.AppendChild(telUffDestNode);

            XmlNode faxDestNode = doc.CreateElement("Fax");
            infoDestNode.AppendChild(faxDestNode);

            XmlNode contPrefDestNode = doc.CreateElement("ContattoPreferito");
            infoDestNode.AppendChild(contPrefDestNode);

            XmlNode oraPrefDestNode = doc.CreateElement("OrarioPreferito");
            infoDestNode.AppendChild(oraPrefDestNode);

            XmlNode indirizzoDestNode = doc.CreateElement("Indirizzo");
            infoDestNode.AppendChild(indirizzoDestNode);

            XmlNode localitaDestNode = doc.CreateElement("Localita");
            infoDestNode.AppendChild(localitaDestNode);

            XmlNode infoAggDestNode = doc.CreateElement("InfoAggiuntive");
            infoDestNode.AppendChild(infoAggDestNode);

            XmlNode edificioDestNode = doc.CreateElement("Edificio");
            infoDestNode.AppendChild(edificioDestNode);

            XmlNode noteDestNode = doc.CreateElement("Note");
            infoDestNode.AppendChild(noteDestNode);
        }


        public static void documentoPerProtocolla(XmlDocument doc, XmlNode documentoNode, DocumentoXml formDatiXml)
        {

            XmlNode riservatoNode = doc.CreateElement("Riservato");
            riservatoNode.AppendChild(doc.CreateTextNode(formDatiXml.riservato));
            documentoNode.AppendChild(riservatoNode);

            XmlNode flagProtoNode = doc.CreateElement("FlagProtocollo");
            flagProtoNode.AppendChild(doc.CreateTextNode(formDatiXml.flagProtocollo));
            documentoNode.AppendChild(flagProtoNode);

            XmlNode classificaNode = doc.CreateElement("Classifica");
            classificaNode.AppendChild(doc.CreateTextNode(formDatiXml.classifica));
            documentoNode.AppendChild(classificaNode);

            XmlNode oggettoNode = doc.CreateElement("Oggetto");
            oggettoNode.AppendChild(doc.CreateTextNode(formDatiXml.oggetto));
            documentoNode.AppendChild(oggettoNode);

            XmlNode protoMittNode = doc.CreateElement("ProtocolloMittente");
            XmlAttribute dataAttr = doc.CreateAttribute("Data");
            XmlAttribute protoAttr = doc.CreateAttribute("Protocollo");
            documentoNode.AppendChild(protoMittNode);

            XmlNode protoInizNode = doc.CreateElement("ProtocolloIniziale");
            XmlAttribute codAmmAttr = doc.CreateAttribute("CodAmm");
            XmlAttribute codAOOAttr = doc.CreateAttribute("codAOO");
            XmlAttribute progrAttr = doc.CreateAttribute("Progr");
            XmlAttribute dataPrAttr = doc.CreateAttribute("Data");
            documentoNode.AppendChild(protoInizNode);

            XmlNode dataArrivoNode = doc.CreateElement("DataArrivo");
            documentoNode.AppendChild(dataArrivoNode);
        }

        /*
        public static void documentoPerProtoConImmagine(XmlDocument doc, XmlNode documentoNode, FormDatiXmlProtocolla formDatiXml)
        {
          
            XmlNode riservatoNode = doc.CreateElement("Riservato");
            documentoNode.AppendChild(riservatoNode);

            XmlNode flagProtoNode = doc.CreateElement("FlagProtocollo");
            documentoNode.AppendChild(flagProtoNode);

            XmlNode classificaNode = doc.CreateElement("Classifica");
            documentoNode.AppendChild(classificaNode);

            XmlNode oggettoNode = doc.CreateElement("Oggetto");
            documentoNode.AppendChild(oggettoNode);

            XmlNode protoMittNode = doc.CreateElement("ProtocolloMittente");
            XmlAttribute dataAttr = doc.CreateAttribute("Data");
            XmlAttribute protoAttr = doc.CreateAttribute("Protocollo");
            documentoNode.AppendChild(protoMittNode);

            XmlNode protoInizNode = doc.CreateElement("ProtocolloIniziale");
            XmlAttribute codAmmAttr = doc.CreateAttribute("CodAmm");
            XmlAttribute codAOOAttr = doc.CreateAttribute("codAOO");
            XmlAttribute progrAttr = doc.CreateAttribute("Progr");
            XmlAttribute dataPrAttr = doc.CreateAttribute("Data");
            documentoNode.AppendChild(protoInizNode);

            XmlNode dataArrivoNode = doc.CreateElement("DataArrivo");
            documentoNode.AppendChild(dataArrivoNode);
        }
        */

        public static string creaXmlAssociaImmagine(FormDatiXmlAssociaImg formDatiXmlAssImg)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(docNode);

            XmlNode datiInvioNode = doc.CreateElement("DatiInvio");
            doc.AppendChild(datiInvioNode);
            //---

            XmlNode listaDocNode = doc.CreateElement("ListaDocumenti");
            datiInvioNode.AppendChild(listaDocNode);
            //---

            XmlNode documentoNode = doc.CreateElement("Documento");
            XmlAttribute nomeFileDocAttr = doc.CreateAttribute("NomeFile");            
            nomeFileDocAttr.Value = formDatiXmlAssImg.nomeFile;
            documentoNode.Attributes.Append(nomeFileDocAttr);

            //Laura 7 febbraio 2013
            XmlAttribute versionamentoAttr = doc.CreateAttribute("Versionamento");
            versionamentoAttr.Value = formDatiXmlAssImg.versionamento;
            documentoNode.Attributes.Append(versionamentoAttr);
            
            listaDocNode.AppendChild(documentoNode);



            return doc.OuterXml;
        }

        /*
       
        public static void creaXmlProtocollaConImmagine(FormDatiXmlProtocolla formDatiXml)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(docNode);

            XmlNode datiInvioNode = doc.CreateElement("DatiInvio");
            doc.AppendChild(datiInvioNode);
            //---

            XmlNode categioriaProtoNode = doc.CreateElement("CategoriaProtocollo");
            categioriaProtoNode.AppendChild(doc.CreateTextNode(formDatiXml.categoriaProtocollo));
            datiInvioNode.AppendChild(categioriaProtoNode);

            XmlNode mittenteNode = doc.CreateElement("Mittente");
            datiInvioNode.AppendChild(mittenteNode);
            //---

            mittente(doc, mittenteNode, formDatiXml);

            //+++

            XmlNode listaDestNode = doc.CreateElement("ListaDestinatari");
            datiInvioNode.AppendChild(listaDestNode);
            //---
            //...

            XmlNode destinatarioNode = doc.CreateElement("Destinatario");
            listaDestNode.AppendChild(destinatarioNode);

            XmlNode tipoDestNode = doc.CreateElement("Tipo");
            destinatarioNode.AppendChild(tipoDestNode);

            XmlNode codiceDestNode = doc.CreateElement("Codice");
            destinatarioNode.AppendChild(codiceDestNode);

            XmlNode nominativoDestNode = doc.CreateElement("Nominativo");
            destinatarioNode.AppendChild(nominativoDestNode);

            XmlNode listaDocNode = doc.CreateElement("ListaDocumenti");
            datiInvioNode.AppendChild(listaDocNode);

            XmlNode documentoNode = doc.CreateElement("Documento");
            XmlAttribute nomeDocAttr = doc.CreateAttribute("NomeFile");
            documentoNode.Attributes.Append(nomeDocAttr);
            listaDocNode.AppendChild(documentoNode);
            //---
            //...

            documentoPerProtoConImmagine(doc, documentoNode, formDatiXml);

            XmlNode listaSoggAffNode = doc.CreateElement("ListaSoggettiAfferenti");
            documentoNode.AppendChild(listaSoggAffNode);
            //...

            XmlNode soggAfferenteNode = doc.CreateElement("soggettoAfferente");
            listaSoggAffNode.AppendChild(soggAfferenteNode);
            //---

            soggettoAfferente(doc, soggAfferenteNode, formDatiXml);
            //+++

            XmlNode listaAllegatiNode = doc.CreateElement("ListaAllegati");
            documentoNode.AppendChild(listaAllegatiNode);

            XmlNode documentoAllNode = doc.CreateElement("Documento");
            XmlAttribute docAllnomeAttr = doc.CreateAttribute("NomeFile");
            documentoAllNode.Attributes.Append(docAllnomeAttr);
            listaAllegatiNode.AppendChild(documentoAllNode);

            XmlNode oggettoDocAllNode = doc.CreateElement("Oggetto");
            documentoAllNode.AppendChild(oggettoDocAllNode);



        }

         */


        public static string creaXmlPerAssociaAllegato(FormDatiXmlAllegato formDatiXml)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(docNode);

            XmlNode datiInvioNode = doc.CreateElement("DatiInvio");
            doc.AppendChild(datiInvioNode);
            //---

            XmlNode categioriaProtoNode = doc.CreateElement("CategoriaProtocollo");
            categioriaProtoNode.AppendChild(doc.CreateTextNode(formDatiXml.categoriaProtocollo));
            datiInvioNode.AppendChild(categioriaProtoNode);

            XmlNode mittenteNode = doc.CreateElement("Mittente");
            datiInvioNode.AppendChild(mittenteNode);
            //---

            XmlNode tipoMittNode = doc.CreateElement("Tipo");
            tipoMittNode.AppendChild(doc.CreateTextNode(formDatiXml.tipoMittente));
            mittenteNode.AppendChild(tipoMittNode);

            XmlNode codiceMittNode = doc.CreateElement("Codice");

            //laura 7 febbraio 2013
            codiceMittNode.AppendChild(doc.CreateTextNode(formDatiXml.codiceMittente));

            mittenteNode.AppendChild(codiceMittNode);

            XmlNode nominativoMittNode = doc.CreateElement("Nominativo");
            nominativoMittNode.AppendChild(doc.CreateTextNode(formDatiXml.nominativoMittente));
            mittenteNode.AppendChild(nominativoMittNode);

            XmlNode listaDocNode = doc.CreateElement("ListaDocumenti");
            datiInvioNode.AppendChild(listaDocNode);
            //---



            //Laura 31 Ottobre

            if(formDatiXml.categoriaProtocollo == "U"){


            XmlNode listaDestNode = doc.CreateElement("ListaDestinatari");
            datiInvioNode.AppendChild(listaDestNode);


            if (formDatiXml.listaDestinatari.Count == 0)
            {
                XmlNode destNode = doc.CreateElement("Destinatario");
                listaDestNode.AppendChild(destNode);
                DestinatarioXml dest = new DestinatarioXml();
                destinatario(doc, destNode, dest);
            }



                //altrimenti, se la lista dei destinatari non è vuota, stampo il contenuto nell'xml
            else
            {

                foreach (DestinatarioXml dest in formDatiXml.listaDestinatari)
                {
                    XmlNode destNode = doc.CreateElement("Destinatario");
                    listaDestNode.AppendChild(destNode);                    

                    destinatario(doc, destNode, dest);
                    

                }
            }


            if (formDatiXml.listaDestinatariCC.Count > 0)
            {
                foreach (DestinatarioXml dest in formDatiXml.listaDestinatariCC)
                {
                    XmlNode destNode = doc.CreateElement("Destinatario");
                    XmlAttribute ccAttr = doc.CreateAttribute("TipoInvio");
                    ccAttr.Value = "CC";
                    destNode.Attributes.Append(ccAttr);
                    listaDestNode.AppendChild(destNode);                   
                    destinatario(doc, destNode, dest);
                    //+++

                }
            }

        }

            //Laura 31 Ottobre

            XmlNode documentoNode = doc.CreateElement("Documento");
            XmlAttribute nomeFileDocAttr = doc.CreateAttribute("NomeFile");
            nomeFileDocAttr.Value = formDatiXml.nomeFile;
            documentoNode.Attributes.Append(nomeFileDocAttr);
            listaDocNode.AppendChild(documentoNode);

            XmlNode riservatoNode = doc.CreateElement("Riservato");
            riservatoNode.AppendChild(doc.CreateTextNode(formDatiXml.riservato));
            documentoNode.AppendChild(riservatoNode);

            XmlNode flagProtoNode = doc.CreateElement("FlagProtocollo");
            flagProtoNode.AppendChild(doc.CreateTextNode(formDatiXml.flagProtocollo));
            documentoNode.AppendChild(flagProtoNode);

            XmlNode classificaNode = doc.CreateElement("Classifica");
            classificaNode.AppendChild(doc.CreateTextNode(formDatiXml.classifica));
            documentoNode.AppendChild(classificaNode);

            XmlNode oggettoNode = doc.CreateElement("Oggetto");
            oggettoNode.AppendChild(doc.CreateTextNode(formDatiXml.oggetto));
            documentoNode.AppendChild(oggettoNode);

            XmlNode dataArrivoNode = doc.CreateElement("DataArrivo");
            documentoNode.AppendChild(dataArrivoNode);

            return doc.OuterXml;
        }
    }
}
