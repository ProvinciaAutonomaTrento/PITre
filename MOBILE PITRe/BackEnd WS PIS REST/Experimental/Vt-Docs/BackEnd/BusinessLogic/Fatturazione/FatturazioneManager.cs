using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using DocsPaVO.documento;
using DocsPaVO.Fatturazione;
using DocsPaVO.utente;
using log4net;

namespace BusinessLogic.Fatturazione
{
    public class FatturazioneManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(BusinessLogic.Fatturazione.FatturazioneManager));
        private static string InvoiceNamespace = System.Configuration.ConfigurationManager.AppSettings["NAMESPACE_FATTURAPA"];

        public enum TrasmissioneFattureRicevutaType
        {
            UNKNOWN,
            RicevutaConsegna,
            NotificaMancataConsegna,
            NotificaScarto,
            NotificaEsito,
            NotificaDecorrenzaTermini,
            AttestazioneTrasmissioneFattura
        }

        public static string GetXMLTemplateFattura(string idAmm)
        {
            string retVal = string.Empty;
            try
            {
                DocsPaDB.Query_DocsPAWS.Fatturazione fatturazione = new DocsPaDB.Query_DocsPAWS.Fatturazione();
                retVal = fatturazione.getXmlTemplate(idAmm);
            }
            catch (Exception ex)
            {
            }

            return retVal;
        }

        public static string GetFattura(string idAmm, string idFattura)
        {
            string retVal = string.Empty;
            try
            {
                DocsPaDB.Query_DocsPAWS.Fatturazione fatturazione = new DocsPaDB.Query_DocsPAWS.Fatturazione();

                FatturaPA fattura = fatturazione.getFattura(idAmm, idFattura);

                if (fattura != null)
                {
                    retVal = createXMLFattura(fattura);
                }
                else
                {
                    retVal = "NotFound";
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                retVal = "KO";
            }
            return retVal;
        }

        public static bool SendFattura(string fattura, DocsPaVO.utente.InfoUtente infoUtente, string idGruppo)
        {
            bool retVal = false;
            string error = string.Empty;

            try
            {
                DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(idGruppo);

                // 1 - Costruisco il documento XML
                //string decl = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
                //string pi = "<?xml-stylesheet type=\"text/xsl\" href=\"http://localhost/nttdatawa/importDati/fatturapa_v1.0.xsl\"?>";
                //fattura = fattura.Replace(decl, decl + "\n" + pi);
                XmlDocument docXML = new XmlDocument();
                docXML.LoadXml(fattura);
                XmlNamespaceManager xmlnsManager = new XmlNamespaceManager(docXML.NameTable);
                //xmlnsManager.AddNamespace("p", "http://www.fatturapa.gov.it/sdi/fatturapa/v1.0");
                //xmlnsManager.AddNamespace("p", "http://www.fatturapa.gov.it/sdi/fatturapa/v1.1");
                xmlnsManager.AddNamespace("p", InvoiceNamespace);

                string idFattura = docXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaBody/DatiGenerali/DatiGeneraliDocumento/Numero", xmlnsManager).InnerText;
                string nomefile = docXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/DatiTrasmissione/IdTrasmittente/IdPaese", xmlnsManager).InnerText +
                                  docXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/DatiTrasmissione/IdTrasmittente/IdCodice", xmlnsManager).InnerText;
                
                // 2 - Creo una nuova SchedaDocumento e personalizzo i parametri
                SchedaDocumento schedaDoc = BusinessLogic.Documenti.DocManager.NewSchedaDocumento(infoUtente);
                schedaDoc.oggetto = new Oggetto() { descrizione = string.Format("Fattura numero {0}",idFattura) };
                schedaDoc.protocollo = null;
                schedaDoc.tipoProto = "G";
                schedaDoc.predisponiProtocollazione = false;
                schedaDoc.registro = null;
                schedaDoc.mezzoSpedizione = "0";
                schedaDoc.descMezzoSpedizione = "";
                //schedaDoc.idPeople = infoUtente.idPeople;
                //schedaDoc.userId = infoUtente.userId;
                ArrayList tipologie = getTipologie(infoUtente.idAmministrazione);
                schedaDoc.tipologiaAtto = new TipologiaAtto();
                try
                {
                    schedaDoc.tipologiaAtto.systemId = (from TipologiaAtto tipo in tipologie where tipo.descrizione.ToUpper() == "FATTURA ELETTRONICA" select tipo).FirstOrDefault().systemId;
                    schedaDoc.tipologiaAtto.descrizione = (from TipologiaAtto tipo in tipologie where tipo.descrizione.ToUpper() == "FATTURA ELETTRONICA" select tipo).FirstOrDefault().descrizione;
                }
                catch(Exception ex1)
                {
                    throw new Exception("Impossibile proseguire, tipologia atto FATTURA ELETTRONICA mancante");
                }

                // 2b - Configuro i campi della tipologia Fattura Elettronica
                DocsPaVO.ProfilazioneDinamica.Templates template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(schedaDoc.tipologiaAtto.systemId);
                if (template == null)
                {
                    throw new Exception("Errore nel reperimento della tipologia Fattura Elettronica");
                }
                foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in template.ELENCO_OGGETTI)
                {
                    XmlNode nodeFatt = null;
                    switch (oggettoCustom.DESCRIZIONE)
                    {
                        case "NUMERO FATTURA":
                            nodeFatt = docXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaBody/DatiGenerali/DatiGeneraliDocumento/Numero", xmlnsManager);
                            break;
                        case "DENOMINAZIONE CP":
                            nodeFatt = docXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/DatiAnagrafici/Anagrafica/Denominazione", xmlnsManager);
                            break;
                        case "NOME CP":
                            nodeFatt = docXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/DatiAnagrafici/Anagrafica/Nome", xmlnsManager);
                            break;
                        case "COGNOME CP":
                            nodeFatt = docXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/DatiAnagrafici/Anagrafica/Cognome", xmlnsManager);
                            break;
                        case "DATA":
                            nodeFatt = docXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaBody/DatiGenerali/DatiGeneraliDocumento/Data", xmlnsManager);
                            break;
                        case "TIPODOCUMENTO":
                            nodeFatt = docXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaBody/DatiGenerali/DatiGeneraliDocumento/TipoDocumento", xmlnsManager);
                            break;
                        case "CODICEFISCALE CP":
                            nodeFatt = docXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/DatiAnagrafici/CodiceFiscale", xmlnsManager);
                            break;
                        case "PARTITAIVA CP":
                            nodeFatt = docXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/DatiAnagrafici/IdFiscaleIVA/IdCodice", xmlnsManager);
                            break;
                        case "DENOMINAZIONE CC":
                            nodeFatt = docXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/DatiAnagrafici/Anagrafica/Denominazione", xmlnsManager);
                            break;
                        case "NOME CC":
                            nodeFatt = docXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/DatiAnagrafici/Anagrafica/Nome", xmlnsManager);
                            break;
                        case "COGNOME CC":
                            nodeFatt = docXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/DatiAnagrafici/Anagrafica/Cognome", xmlnsManager);
                            break;
                        case "CODICEFISCALE CC":
                            nodeFatt = docXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/DatiAnagrafici/CodiceFiscale", xmlnsManager);
                            break;
                        case "PARTITAIVA CC":
                            nodeFatt = docXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/DatiAnagrafici/IdFiscaleIVA/IdCodice", xmlnsManager);
                            break;
                        default:
                            break;
                    }
                    if (nodeFatt != null)
                    {
                        oggettoCustom.VALORE_DATABASE = nodeFatt.InnerText;
                    }

                    // Devo far scattare il contatore di repertorio
                    if (oggettoCustom.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("CONTATORE") && oggettoCustom.REPERTORIO.Equals("1"))
                    {
                        oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                    }
                }
                schedaDoc.template = template;
                

                // 3 - Aggiungo doc grigio
                schedaDoc = BusinessLogic.Documenti.DocSave.addDocGrigia(schedaDoc, infoUtente, ruolo);

                // 4 - Modifico il documento XML
                // valorizzo il campo ProgressivoInvio
                XmlNode node = docXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/DatiTrasmissione/ProgressivoInvio", xmlnsManager);
                node.InnerText = getSerialSend(schedaDoc.docNumber);
                // rimuovo, se non è stato impostato, il campo RiferimentoAmministrazione
                XmlNode nodeRifAmm = docXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/RiferimentoAmministrazione", xmlnsManager);
                if (string.IsNullOrEmpty(nodeRifAmm.InnerText))
                {
                    nodeRifAmm.ParentNode.RemoveChild(nodeRifAmm);
                }


                //INIZIO
                //Elimino i nodi del template che potrebbero essere vuoti e quindi invalidare la fattura
                XmlNode nodeNumCivico = docXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/Sede/NumeroCivico", xmlnsManager);
                if (nodeNumCivico!=null && string.IsNullOrEmpty(nodeNumCivico.InnerText))
                {
                    nodeNumCivico.ParentNode.RemoveChild(nodeNumCivico);
                }

                XmlNode nodeBic = docXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaBody/DatiPagamento/DettaglioPagamento/BIC", xmlnsManager);
                if (nodeBic != null && string.IsNullOrEmpty(nodeBic.InnerText))
                {
                    nodeBic.ParentNode.RemoveChild(nodeBic);
                }
                //FINE


                // 5 - Produco un array di byte a partire dal documento XML
                //Byte[] bytes = System.Text.Encoding.UTF8.GetBytes(docXML.OuterXml);
                MemoryStream ms = new MemoryStream();
                docXML.Save(ms);
                Byte[] bytes = ms.ToArray();

                // 6 - Creo il FileDocumento contenente l'array di byte del documento XML
                DocsPaVO.documento.FileDocumento fileDoc = new FileDocumento();
                fileDoc.cartaceo = false;
                fileDoc.content = bytes;
                fileDoc.contentType = "text/xml";
                fileDoc.estensioneFile = "xml";
                fileDoc.fullName = nomefile + "_" + ".xml";
                fileDoc.length = bytes.Length;
                fileDoc.name = nomefile + "_" + ".xml";
                fileDoc.nomeOriginale = nomefile + "_" + ".xml";
                fileDoc.path = "";

                // 7 - Aggiungo il documento XML come doc principale
                DocsPaVO.documento.FileRequest fileReq = (DocsPaVO.documento.FileRequest)schedaDoc.documenti[0];
                bool result = BusinessLogic.Documenti.FileManager.putFile(ref fileReq, fileDoc, infoUtente, out error);
                if (!result)
                {
                    throw new Exception(error);
                }
                else
                {
                    Documenti.areaLavoroManager.execAddLavoroMethod(schedaDoc.systemId, schedaDoc.tipoProto, (schedaDoc.registro != null ? schedaDoc.registro.systemId : ""), infoUtente, null);
                    retVal = true;
                }
                

            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                retVal = false;
            }

            return retVal;
        }

        public static bool LogFattura(string numero, string dataCreazione, string fornitore, string logMessage)
        {
            string convertedDate = string.Empty;

            try
            {
                // Provo la conversione della data dal formato YYYYMMDD al formato DD/MM/YYYY
                convertedDate = string.Format("{0}/{1}/{2}", dataCreazione.Substring(6, 2), dataCreazione.Substring(4, 2), dataCreazione.Substring(0, 4));
                
            }
            catch (Exception ex)
            {
                convertedDate = dataCreazione;
            }

            DocsPaDB.Query_DocsPAWS.Fatturazione f = new DocsPaDB.Query_DocsPAWS.Fatturazione();
            return f.LogFattura(numero, convertedDate, fornitore, logMessage);
        }

        public static string CheckNumFattura(string numero, string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Fatturazione f = new DocsPaDB.Query_DocsPAWS.Fatturazione();
            return f.CheckNumFattura(numero, idAmm);
        }

        public static string GetIdDocFatturaFromNomefile(string nomeFile)
        {
            DocsPaDB.Query_DocsPAWS.Fatturazione f = new DocsPaDB.Query_DocsPAWS.Fatturazione();
            return f.GetIdDocFatturaFromNomefile(nomeFile);
        }

        public static bool LogFatturaFirmata(string docNumber, string nomeFatt, string nomeAllegato, string logMessage)
        {
            DocsPaDB.Query_DocsPAWS.Fatturazione f = new DocsPaDB.Query_DocsPAWS.Fatturazione();
            return f.LogFatturaFirmata(docNumber, nomeFatt, nomeAllegato, logMessage);
        }

        private static string getSerialSend(string docNumb)
        {
            int value = Convert.ToInt32(docNumb);
            string result = string.Empty;
            char[] baseChars = new char[] { '0','1','2','3','4','5','6','7','8','9',
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x'};
            int targetBase = baseChars.Length;

            do
            {
                result = baseChars[value % targetBase] + result;
                value = value / targetBase;
            }
            while (value > 0);

            return result;
        }

        private static string createXMLFattura(FatturaPA fattura)
        {
            string result = string.Empty;

            try
            {

                // path del template
                //string appPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                //string pathTemplate = System.IO.Path.Combine(appPath, "xml/FatturaElettronica.xml");

                // caricamento template
                XmlDocument fatturaXML = new XmlDocument();
                fatturaXML.LoadXml(fattura.templateXML);
                //fatturaXML.Load(pathTemplate);

                XmlNamespaceManager xmlnsManager = new XmlNamespaceManager(fatturaXML.NameTable);
                //xmlnsManager.AddNamespace("p", "http://www.fatturapa.gov.it/sdi/fatturapa/v1.0");
                //xmlnsManager.AddNamespace("p", "http://www.fatturapa.gov.it/sdi/fatturapa/v1.1");
                xmlnsManager.AddNamespace("p", InvoiceNamespace);

                #region HEADER

                XmlNode header = fatturaXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader", xmlnsManager);

                #region DatiTrasmissione

                XmlNode trasmIdPaese = header.SelectSingleNode("DatiTrasmissione/IdTrasmittente/IdPaese");
                trasmIdPaese.InnerText = fattura.trasmittenteIdPaese;
                XmlNode trasmIdCodice = header.SelectSingleNode("DatiTrasmissione/IdTrasmittente/IdCodice");
                trasmIdCodice.InnerText = fattura.trasmittenteIdCodice;
                // Il campo ProgressivoInvio viene valorizzato dopo che la fattura viene acquisita
                // e corrisponde all'id documento in VtDocs
                XmlNode trasmFormato = header.SelectSingleNode("DatiTrasmissione/FormatoTrasmissione");
                trasmFormato.InnerText = fattura.formatoTrasmissione;
                XmlNode trasmCodDestinatario = header.SelectSingleNode("DatiTrasmissione/CodiceDestinatario");
                trasmCodDestinatario.InnerText = fattura.codiceIPA;

                // ContattiTrasmittente
                XmlNode contattiTel = header.SelectSingleNode("DatiTrasmissione/ContattiTrasmittente/Telefono");
                contattiTel.InnerText = fattura.trasmittenteTelefono;
                XmlNode contattiMail = header.SelectSingleNode("DatiTrasmissione/ContattiTrasmittente/Email");
                contattiMail.InnerText = fattura.trasmittenteMail;

                #endregion

                #region CedentePrestatore

                XmlNode cedente = header.SelectSingleNode("CedentePrestatore");

                XmlNode cedIdPaese = cedente.SelectSingleNode("DatiAnagrafici/IdFiscaleIVA/IdPaese");
                cedIdPaese.InnerText = fattura.cedente.idPaese;
                XmlNode cedIdCodice = cedente.SelectSingleNode("DatiAnagrafici/IdFiscaleIVA/IdCodice");
                cedIdCodice.InnerText = fattura.cedente.idCodice;
                XmlNode cedDenominazione = cedente.SelectSingleNode("DatiAnagrafici/Anagrafica/Denominazione");
                cedDenominazione.InnerText = fattura.cedente.denominazione;
                // RegimeFiscale
                XmlNode cedSedeIndirizzo = cedente.SelectSingleNode("Sede/Indirizzo");
                cedSedeIndirizzo.InnerText = fattura.cedente.indirizzo;
                XmlNode cedSedeNumCivico = cedente.SelectSingleNode("Sede/NumeroCivico");
                if (string.IsNullOrEmpty(fattura.cedente.numCivico))
                    cedSedeNumCivico.ParentNode.RemoveChild(cedSedeNumCivico);
                else
                    cedSedeNumCivico.InnerText = fattura.cedente.numCivico;
                XmlNode cedSedeCAP = cedente.SelectSingleNode("Sede/CAP");
                cedSedeCAP.InnerText = fattura.cedente.CAP;
                XmlNode cedSedeComune = cedente.SelectSingleNode("Sede/Comune");
                cedSedeComune.InnerText = fattura.cedente.comune;
                XmlNode cedSedeProvincia = cedente.SelectSingleNode("Sede/Provincia");
                cedSedeProvincia.InnerText = fattura.cedente.provincia;
                XmlNode cedSedeNazione = cedente.SelectSingleNode("Sede/Nazione");
                cedSedeNazione.InnerText = fattura.cedente.nazione;
                XmlNode cedUfficio = cedente.SelectSingleNode("IscrizioneREA/Ufficio");
                cedUfficio.InnerText = fattura.cedente.ufficio;
                XmlNode cedNumeroREA = cedente.SelectSingleNode("IscrizioneREA/NumeroREA");
                cedNumeroREA.InnerText = fattura.cedente.numeroREA;
                XmlNode cedCapitaleSociale = cedente.SelectSingleNode("IscrizioneREA/CapitaleSociale");
                cedCapitaleSociale.InnerText = fattura.cedente.capitaleSociale;
                XmlNode cedSocioUnico = cedente.SelectSingleNode("IscrizioneREA/SocioUnico");
                cedSocioUnico.InnerText = fattura.cedente.socioUnico;
                XmlNode cedStatoLiquidazione = cedente.SelectSingleNode("IscrizioneREA/StatoLiquidazione");
                cedStatoLiquidazione.InnerText = fattura.cedente.statoLiquidazione;

                #endregion

                #region CessionarioCommittente

                XmlNode cessionario = header.SelectSingleNode("CessionarioCommittente");

                if (!string.IsNullOrEmpty(fattura.cessionario.idCodiceI.Trim()))
                {
                    XmlNode cessIdPaese = cessionario.SelectSingleNode("DatiAnagrafici/IdFiscaleIVA/IdPaese");
                    cessIdPaese.InnerText = fattura.cessionario.idPaese;
                    XmlNode cessIdCodiceI = cessionario.SelectSingleNode("DatiAnagrafici/IdFiscaleIVA/IdCodice");
                    cessIdCodiceI.InnerText = fattura.cessionario.idCodiceI;
                }
                else
                {
                    XmlNode cessAnagrafica = cessionario.SelectSingleNode("DatiAnagrafici");
                    XmlNode cessIdCodiceI = cessionario.SelectSingleNode("DatiAnagrafici/IdFiscaleIVA");
                    cessIdCodiceI.RemoveAll();
                    //cessAnagrafica.RemoveChild(cessIdCodiceI);

                    XmlElement cessIdCodiceF = fatturaXML.CreateElement("CodiceFiscale");
                    cessIdCodiceF.InnerText = fattura.cessionario.idCodiceF;
                    cessAnagrafica.ReplaceChild(cessIdCodiceF, cessIdCodiceI);
                    //cessAnagrafica.AppendChild(cessIdCodiceF);
                }

                XmlNode cessDenominazione = cessionario.SelectSingleNode("DatiAnagrafici/Anagrafica/Denominazione");
                cessDenominazione.InnerText = fattura.cessionario.denominazione;
                // RegimeFiscale
                XmlNode cessSedeIndirizzo = cessionario.SelectSingleNode("Sede/Indirizzo");
                cessSedeIndirizzo.InnerText = fattura.cessionario.indirizzo;
                XmlNode cessSedeNumCivico = cessionario.SelectSingleNode("Sede/NumeroCivico");
                if (string.IsNullOrEmpty(fattura.cessionario.numCivico))
                    cessSedeNumCivico.ParentNode.RemoveChild(cessSedeNumCivico);
                else
                    cessSedeNumCivico.InnerText = fattura.cessionario.numCivico;
                XmlNode cessSedeCAP = cessionario.SelectSingleNode("Sede/CAP");
                cessSedeCAP.InnerText = fattura.cessionario.CAP;
                XmlNode cessSedeComune = cessionario.SelectSingleNode("Sede/Comune");
                cessSedeComune.InnerText = fattura.cessionario.comune;
                XmlNode cessSedeProvincia = cessionario.SelectSingleNode("Sede/Provincia");
                cessSedeProvincia.InnerText = fattura.cessionario.provincia;
                XmlNode cessSedeNazione = cessionario.SelectSingleNode("Sede/Nazione");
                cessSedeNazione.InnerText = fattura.cessionario.nazione;

                #endregion

                #endregion

                #region BODY

                XmlNode body = fatturaXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaBody", xmlnsManager);

                #region DatiGenerali

                XmlNode tipoDoc = body.SelectSingleNode("DatiGenerali/DatiGeneraliDocumento/TipoDocumento");
                tipoDoc.InnerText = fattura.tipoDoc;
                XmlNode divisa = body.SelectSingleNode("DatiGenerali/DatiGeneraliDocumento/Divisa");
                divisa.InnerText = fattura.divisa;
                XmlNode dataFattura = body.SelectSingleNode("DatiGenerali/DatiGeneraliDocumento/Data");
                dataFattura.InnerText = (fattura.dataDoc).ToString("yyyy-MM-dd");
                XmlNode numFattura = body.SelectSingleNode("DatiGenerali/DatiGeneraliDocumento/Numero");
                numFattura.InnerText = fattura.numeroFattura;
                XmlNode importoTotaleDoc = body.SelectSingleNode("DatiGenerali/DatiGeneraliDocumento/ImportoTotaleDocumento");
                if (string.IsNullOrEmpty(fattura.importoTotaleDoc))
                    importoTotaleDoc.ParentNode.RemoveChild(importoTotaleDoc);
                else
                    importoTotaleDoc.InnerXml = fattura.importoTotaleDoc;

                XmlNode ordineId = body.SelectSingleNode("DatiGenerali/DatiOrdineAcquisto/IdDocumento");
                ordineId.InnerText = fattura.idOrdineAcquisto;
                XmlNode ordineCUP = body.SelectSingleNode("DatiGenerali/DatiOrdineAcquisto/CodiceCUP");
                ordineCUP.InnerText = fattura.CUPOrdineAcquisto;
                XmlNode ordineCIG = body.SelectSingleNode("DatiGenerali/DatiOrdineAcquisto/CodiceCIG");
                ordineCIG.InnerText = fattura.CIGOrdineAcquisto;

                XmlNode contrattoId = body.SelectSingleNode("DatiGenerali/DatiContratto/IdDocumento");
                contrattoId.InnerText = fattura.idContratto;
                XmlNode contrattoCUP = body.SelectSingleNode("DatiGenerali/DatiContratto/CodiceCUP");
                contrattoCUP.InnerText = fattura.CUPContratto;
                XmlNode contrattoCIG = body.SelectSingleNode("DatiGenerali/DatiContratto/CodiceCIG");
                contrattoCIG.InnerText = fattura.CIGContratto;

                #endregion

                #region DatiBeniServizi

                XmlNode serviziNode = body.SelectSingleNode("DatiBeniServizi");

                #region DettaglioLinee

                foreach (DatiBeniServizi line in fattura.servizi)
                {
                    XmlElement dettaglioLinee = fatturaXML.CreateElement("DettaglioLinee");

                    XmlElement numLinea = fatturaXML.CreateElement("NumeroLinea");
                    numLinea.InnerText = line.numeroLinea;
                    dettaglioLinee.AppendChild(numLinea);
                    XmlElement descrizione = fatturaXML.CreateElement("Descrizione");
                    descrizione.InnerText = line.descrizione;
                    dettaglioLinee.AppendChild(descrizione);
                    XmlElement quantita = fatturaXML.CreateElement("Quantita");
                    quantita.InnerText = line.quantita;
                    dettaglioLinee.AppendChild(quantita);
                    XmlElement unitaMisura = fatturaXML.CreateElement("UnitaMisura");
                    unitaMisura.InnerText = line.unitaMisura;
                    dettaglioLinee.AppendChild(unitaMisura);
                    XmlElement prezzoUnitario = fatturaXML.CreateElement("PrezzoUnitario");
                    prezzoUnitario.InnerText = line.prezzoUnitario;
                    dettaglioLinee.AppendChild(prezzoUnitario);
                    XmlElement prezzoTotale = fatturaXML.CreateElement("PrezzoTotale");
                    prezzoTotale.InnerText = line.prezzoTotale;
                    dettaglioLinee.AppendChild(prezzoTotale);
                    XmlElement aliquotaIVA = fatturaXML.CreateElement("AliquotaIVA");
                    string aliquota = (string.IsNullOrEmpty(line.aliquotaIVA) ? "0.00" : line.aliquotaIVA);
                    aliquotaIVA.InnerText = aliquota;
                    dettaglioLinee.AppendChild(aliquotaIVA);
                    if(aliquota == "0.00") {
                        XmlElement natura = fatturaXML.CreateElement("Natura");
                        natura.InnerText = "N4";
                        dettaglioLinee.AppendChild(natura);
                    }

                    serviziNode.AppendChild(dettaglioLinee);
                }

                #endregion

                #region DatiRiepilogo

                XmlElement datiRiepilogo = fatturaXML.CreateElement("DatiRiepilogo");
                XmlElement riepilogoAliquotaIVA = fatturaXML.CreateElement("AliquotaIVA");
                riepilogoAliquotaIVA.InnerText = fattura.aliquotaIVA;
                datiRiepilogo.AppendChild(riepilogoAliquotaIVA);
                if (fattura.aliquotaIVA.Trim() == "0.00")
                {
                    XmlElement natura = fatturaXML.CreateElement("Natura");
                    natura.InnerText = "N4";
                    datiRiepilogo.AppendChild(natura);
                }
                XmlElement riepilogoImponibile = fatturaXML.CreateElement("ImponibileImporto");
                riepilogoImponibile.InnerText = fattura.imponibileImporto;
                datiRiepilogo.AppendChild(riepilogoImponibile);
                XmlElement riepilogoImposta = fatturaXML.CreateElement("Imposta");
                riepilogoImposta.InnerText = fattura.imposta;
                datiRiepilogo.AppendChild(riepilogoImposta);
                XmlElement riepilogoEsigibilitaIVA = fatturaXML.CreateElement("EsigibilitaIVA");
                riepilogoEsigibilitaIVA.InnerText = fattura.esigibilitaIVA;
                datiRiepilogo.AppendChild(riepilogoEsigibilitaIVA);

                serviziNode.AppendChild(datiRiepilogo);

                #endregion

                #endregion

                #region DatiPagamento

                // CondizioniPagamento
                XmlNode condizioniPagamento = body.SelectSingleNode("DatiPagamento/CondizioniPagamento");
                condizioniPagamento.InnerText = fattura.pagamentoCondizioni;
                XmlNode modalitaPagamento = body.SelectSingleNode("DatiPagamento/DettaglioPagamento/ModalitaPagamento");
                modalitaPagamento.InnerText = fattura.pagamentoModalita;
                XmlNode importoPagamento = body.SelectSingleNode("DatiPagamento/DettaglioPagamento/ImportoPagamento");
                importoPagamento.InnerText = fattura.pagamentoImporto;
                XmlNode dataRifTerminiPagamento = body.SelectSingleNode("DatiPagamento/DettaglioPagamento/DataRiferimentoTerminiPagamento");
                dataRifTerminiPagamento.InnerText = (fattura.dataRifTerminiPagamento).ToString("yyyy-MM-dd");
                XmlNode giorniTerminiPagamento = body.SelectSingleNode("DatiPagamento/DettaglioPagamento/GiorniTerminiPagamento");
                giorniTerminiPagamento.InnerText = fattura.giorniTerminiPagamento;
                XmlNode istitutoFinanziario = body.SelectSingleNode("DatiPagamento/DettaglioPagamento/IstitutoFinanziario");
                istitutoFinanziario.InnerText = fattura.istitutoFinanziario;
                XmlNode IBAN = body.SelectSingleNode("DatiPagamento/DettaglioPagamento/IBAN");
                IBAN.InnerText = fattura.IBAN;
                XmlNode BIC = body.SelectSingleNode("DatiPagamento/DettaglioPagamento/BIC");
                BIC.InnerText = fattura.BIC;

                #endregion

                #endregion

                result = fatturaXML.OuterXml;
                return result;
            }
            catch (Exception ex)
            {
                result = string.Empty;
                logger.Debug(ex.Message);
                return result;
            }
        }

        private static ArrayList getTipologie(string idAmm)
        {

            ArrayList result = new ArrayList();

            try
            {
                result = BusinessLogic.Documenti.DocManager.getTipologiaAtto(idAmm);
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                result = null;
            }

            return result;
        }

        public static bool PutProfileFattura(string docnumber, string identificativo_Invio, string diagramId)
        {
            bool retVal = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.Fatturazione fatturazione = new DocsPaDB.Query_DocsPAWS.Fatturazione();

                retVal = fatturazione.InsertProfileFattura(docnumber, identificativo_Invio, diagramId);
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                retVal = false;
            }
            return retVal;
        }

        public static bool Add_Notifica_SDI(DocsPaVO.utente.InfoUtente infoUtente, string documentNumber, string fileName, byte[] fileRicevuta, TrasmissioneFattureRicevutaType comunicationType, DocsPaVO.DiagrammaStato.DiagrammaStato diagramma)
        {
            bool retVal = false;
            string descrizioneAllegato = "UNKNOW";
            string systemId_Stato = "0";

            /* VALORI PRESENTI AL MOMENTO DELLO SVILUPPO NEL DATABASE NTTDOCSTEST SU SERVER 10.168.1.89
            1	In redazione
            2	Genera versione con allegati
            3	Invia al SDI
                4	Notifica di scarto
                5	Notifica di mancata consegna
                6	Ricevuta di consegna
                7	Notifica esito Committente
                8	Notifica decorrenza termini
            */
            try
            {
                switch (comunicationType)
                {
                    case TrasmissioneFattureRicevutaType.RicevutaConsegna:
                        descrizioneAllegato = "Ricevuta di consegna";
                        systemId_Stato = "6";
                        break;
                    case TrasmissioneFattureRicevutaType.NotificaScarto:
                        descrizioneAllegato = "Notifica di scarto";
                        systemId_Stato = "4";
                        break;
                    case TrasmissioneFattureRicevutaType.NotificaMancataConsegna:
                        descrizioneAllegato = "Notifica di mancata consegna";
                        systemId_Stato = "5";
                        break;
                    case TrasmissioneFattureRicevutaType.NotificaEsito:
                        descrizioneAllegato = "Esito consegna";
                        systemId_Stato = "7";
                        break;
                    case TrasmissioneFattureRicevutaType.NotificaDecorrenzaTermini:
                        descrizioneAllegato = "Notifica della decorrenza dei termini";
                        systemId_Stato = "8";
                        break;
                    case TrasmissioneFattureRicevutaType.AttestazioneTrasmissioneFattura:
                        descrizioneAllegato = "Attestazionr trasmissione fattura";
                        systemId_Stato = "7";
                        break;
                }

                DocsPaVO.documento.FileRequest allegato = new DocsPaVO.documento.Allegato
                {
                    docNumber = documentNumber,
                    descrizione = descrizioneAllegato
                };

                allegato.fileName = fileName;

                allegato = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, ((DocsPaVO.documento.Allegato)allegato));

                //definisco l'allegato come esterno
                BusinessLogic.Documenti.AllegatiManager.setFlagAllegatiEsterni(allegato.versionId, allegato.docNumber);

                DocsPaVO.documento.FileDocumento fileDocumento = new DocsPaVO.documento.FileDocumento
                {
                    name = fileName,
                    fullName = fileName,
                    content = fileRicevuta,
                    contentType = BusinessLogic.Documenti.FileManager.getContentType(fileName),
                    length = fileRicevuta.Length,
                    bypassFileContentValidation = true
                };


                string erroreMessage;
                if (BusinessLogic.Documenti.FileManager.putFile(ref allegato, fileDocumento, infoUtente, out erroreMessage))
                {
                    BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStato(documentNumber, systemId_Stato, diagramma, infoUtente.idPeople, infoUtente, "");
                    retVal = true;
                }
                else
                {
                    throw new Exception(erroreMessage);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                retVal = false;
            }

            return retVal;
        }

        public static DocsPaVO.utente.Utente GetUtenteOwner(string id_sdi, out DocsPaVO.utente.Ruolo ruolo, out string docNumber, out DocsPaVO.DiagrammaStato.DiagrammaStato diagramma)
        {
            DocsPaVO.utente.Utente utente = null;

            try
            {
                DocsPaDB.Query_DocsPAWS.Fatturazione fatturazione = new DocsPaDB.Query_DocsPAWS.Fatturazione();
                string[] idPeople_idRuolo_idDoc_idDiagramma = fatturazione.GetInfoFatturaSDI(id_sdi);

                utente = BusinessLogic.Utenti.UserManager.getUtente(idPeople_idRuolo_idDoc_idDiagramma[0]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuolo(idPeople_idRuolo_idDoc_idDiagramma[1]);
                docNumber = idPeople_idRuolo_idDoc_idDiagramma[2];
                diagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaById(idPeople_idRuolo_idDoc_idDiagramma[3]);

            }
            catch (Exception ex)
            {
                ruolo = null; diagramma = null; docNumber = "";
                logger.Debug(ex.Message);
            }

            return utente;
        }

        public static bool IsFatturaElettronica(string docNumber, string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Fatturazione fatturazione = new DocsPaDB.Query_DocsPAWS.Fatturazione();
            return fatturazione.IsFatturaElettronica(docNumber, idAmm);
        }

        public static FileRequest AddAllegatoToXMLFattura(string docNumber, DocsPaVO.documento.FileDocumento allegato, string descrizioneAllegato ,DocsPaVO.utente.InfoUtente infoUtente)
        {
            SchedaDocumento documento = Documenti.DocManager.getDettaglioNoSecurity(infoUtente, docNumber);
            DocsPaVO.documento.FileRequest fileReq = (DocsPaVO.documento.FileRequest)documento.documenti[0];

            string encodedFile = Convert.ToBase64String(allegato.content);

            if (string.IsNullOrEmpty(allegato.estensioneFile))
                allegato.estensioneFile = Documenti.FileManager.getExtFileFromPath(allegato.fullName);

            Byte[] xmlNewVersion = addAllegatoToXML(fileReq.fileName, allegato.estensioneFile, allegato.name, descrizioneAllegato, encodedFile);

            DocsPaVO.documento.FileRequest fileRequest = new FileRequest();
            fileRequest.cartaceo = false;
            fileRequest.daAggiornareFirmatari = false;
            fileRequest.descrizione = fileReq.descrizione;
            fileRequest.docNumber = fileReq.docNumber;

            fileRequest = BusinessLogic.Documenti.VersioniManager.addVersion(fileRequest, infoUtente, false);
            DocsPaVO.documento.FileDocumento newFileVersion = new FileDocumento();

            newFileVersion.cartaceo = false;
            newFileVersion.content = xmlNewVersion;
            newFileVersion.contentType = "text/xml";
            newFileVersion.estensioneFile = "xml";
            newFileVersion.fullName = fileReq.fileName;
            newFileVersion.length = xmlNewVersion.Length;
            newFileVersion.name = fileReq.fileName;
            newFileVersion.nomeOriginale = fileRequest.fileName;
            newFileVersion.path = "";

            return Documenti.FileManager.putFile(fileRequest, newFileVersion, infoUtente, false);

        }

        private static Byte[] addAllegatoToXML(string filename, string estensione, string nomeAllegato, string descrizioneAllegato, string allegatoB64)
        {
            byte[] retValue;
            XmlDocument fatturaXML = new XmlDocument();
            string xmlText = File.ReadAllText(filename);

            try
            {
                XmlNamespaceManager xmlnsManager = new XmlNamespaceManager(fatturaXML.NameTable);

                //xmlnsManager.AddNamespace("p", "http://www.fatturapa.gov.it/sdi/fatturapa/v1.0");
                //xmlnsManager.AddNamespace("p", "http://www.fatturapa.gov.it/sdi/fatturapa/v1.1");
                xmlnsManager.AddNamespace("p", InvoiceNamespace);

                fatturaXML.LoadXml(xmlText);

                XmlNode body = fatturaXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaBody", xmlnsManager);

                XmlNode nodeAllegato = fatturaXML.CreateElement("Allegati");

                XmlElement nodeNomeAllegato = fatturaXML.CreateElement("NomeAttachment");
                nodeNomeAllegato.InnerText = nomeAllegato;
                nodeAllegato.AppendChild(nodeNomeAllegato);

                if (!string.IsNullOrEmpty(estensione))
                {
                    XmlElement formatoFile = fatturaXML.CreateElement("FormatoAttachment");
                    formatoFile.InnerText = estensione;
                    nodeAllegato.AppendChild(formatoFile);
                }

                if (!string.IsNullOrEmpty(descrizioneAllegato))
                {
                    XmlElement descrizione = fatturaXML.CreateElement("DescrizioneAttachment");
                    descrizione.InnerText = descrizioneAllegato;
                    nodeAllegato.AppendChild(descrizione);
                }

                XmlElement nodeContentAllegato = fatturaXML.CreateElement("Attachment");
                nodeContentAllegato.InnerText = allegatoB64;
                nodeAllegato.AppendChild(nodeContentAllegato);

                body.AppendChild(nodeAllegato);

                MemoryStream ms = new MemoryStream();
                fatturaXML.Save(ms);
                retValue = ms.ToArray();
            }
            catch (Exception ex)
            {
                retValue = File.ReadAllBytes(filename);
            }

            return retValue;
        
        }

        public static bool CheckIdMailFattura(string messageId)
        {
            DocsPaDB.Query_DocsPAWS.Fatturazione fatt = new DocsPaDB.Query_DocsPAWS.Fatturazione();
            return fatt.CheckIdRicevutaFattura(messageId);
        }

        public static bool InsertIdMailFattura(string messageId)
        {
            DocsPaDB.Query_DocsPAWS.Fatturazione fatt = new DocsPaDB.Query_DocsPAWS.Fatturazione();
            return fatt.InsertIdRicevutaFattura(messageId);
        }

    }
}
