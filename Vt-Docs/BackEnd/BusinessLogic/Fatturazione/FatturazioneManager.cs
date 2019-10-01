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
using System.Xml.Serialization;

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
                    
                    retVal = createXMLFattura(fattura, idFattura);
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

            using (DocsPaDB.TransactionContext trContxt = new DocsPaDB.TransactionContext())
            {
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
                    schedaDoc.oggetto = new Oggetto() { descrizione = string.Format("Fattura numero {0}", idFattura) };
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
                    string idTipologia = string.Empty;
                    try
                    {
                        schedaDoc.tipologiaAtto.systemId = (from TipologiaAtto tipo in tipologie where tipo.descrizione.ToUpper() == "FATTURA ELETTRONICA" select tipo).FirstOrDefault().systemId;
                        schedaDoc.tipologiaAtto.descrizione = (from TipologiaAtto tipo in tipologie where tipo.descrizione.ToUpper() == "FATTURA ELETTRONICA" select tipo).FirstOrDefault().descrizione;
                        idTipologia = schedaDoc.tipologiaAtto.systemId;
                    }
                    catch (Exception ex1)
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
                    if (node == null)
                    {
                        XmlNode nodoDatiTrasmissione = docXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/DatiTrasmissione", xmlnsManager);
                        XmlElement progressivoInvioEl = docXML.CreateElement("ProgressivoInvio");
                        progressivoInvioEl.InnerText = getSerialSend(schedaDoc.docNumber);
                        nodoDatiTrasmissione.InsertAfter(progressivoInvioEl, nodoDatiTrasmissione.FirstChild);
                    }
                    else
                        node.InnerText = getSerialSend(schedaDoc.docNumber);
                    // rimuovo, se non è stato impostato, il campo RiferimentoAmministrazione
                    XmlNode nodeRifAmm = docXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/RiferimentoAmministrazione", xmlnsManager);
                    if (nodeRifAmm != null && string.IsNullOrEmpty(nodeRifAmm.InnerText))
                    {
                        nodeRifAmm.ParentNode.RemoveChild(nodeRifAmm);
                    }


                    //INIZIO
                    //Elimino i nodi del template che potrebbero essere vuoti e quindi invalidare la fattura
                    XmlNode nodeNumCivico = docXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/Sede/NumeroCivico", xmlnsManager);
                    if (nodeNumCivico != null && string.IsNullOrEmpty(nodeNumCivico.InnerText))
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

                    // 8 - Segno la fattura come lavorata (per il processo automatico)
                    result = ImportFatture_Insert(idFattura.Trim(), schedaDoc.docNumber);
                    if(!result)
                    {
                        throw new Exception("** Errore inserimento fattura tra le fatture lavorate **");
                    }

                    // 9 - Imposto lo stato della fattura
                    DocsPaDB.Query_DocsPAWS.Fatturazione fatt = new DocsPaDB.Query_DocsPAWS.Fatturazione();
                    if(fatt.ImportFatture_HasAttachments(idFattura.Trim()))
                    {
                        logger.Debug("* La fattura prevede allegati - il processo proseguirà manualmente *");

                        // Imposto la fattura come da non inviare
                        result = ImportFatture_SetStatoInvio(schedaDoc.systemId, false, false);

                        // Aggiorno lo stato del diagramma
                        int idDiagramma = DiagrammiStato.DiagrammiStato.getDiagrammaAssociato(idTipologia);
                        if(idDiagramma != 0)
                        {
                            DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = DiagrammiStato.DiagrammiStato.getDiagrammaById(idDiagramma.ToString());
                            if(diagramma != null)
                            {
                                foreach(DocsPaVO.DiagrammaStato.Stato stato in diagramma.STATI)
                                {
                                    if(stato.DESCRIZIONE.ToUpper() == "INSERIMENTO ALLEGATI")
                                    {
                                        DiagrammiStato.DiagrammiStato.salvaModificaStato(schedaDoc.systemId, stato.SYSTEM_ID.ToString(), diagramma, infoUtente.idPeople, infoUtente, string.Empty);
                                        logger.Debug("Verifica trasmissioni automatiche");
                                        ArrayList modelli = DiagrammiStato.DiagrammiStato.isStatoTrasmAuto(infoUtente.idAmministrazione, stato.SYSTEM_ID.ToString(), idTipologia);
                                        if (modelli != null && modelli.Count > 0)
                                        {
                                            InfoDocumento infoDoc = new InfoDocumento();
                                            infoDoc.idProfile = schedaDoc.systemId;
                                            infoDoc.oggetto = schedaDoc.oggetto.descrizione;
                                            infoDoc.docNumber = schedaDoc.docNumber;
                                            infoDoc.tipoProto = schedaDoc.tipoProto;
                                            infoDoc.evidenza = schedaDoc.evidenza;
                                            infoDoc.dataApertura = schedaDoc.dataCreazione;
                                            infoDoc.privato = schedaDoc.privato;
                                            infoDoc.personale = schedaDoc.personale;

                                            foreach (DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione item in modelli)
                                            {

                                                if (item.SINGLE == "1")
                                                {
                                                    LibroFirma.LibroFirmaManager.effettuaTrasmissioneDocDaModello(item, stato.SYSTEM_ID.ToString(), infoDoc, infoUtente, schedaDoc);
                                                }
                                                else
                                                {
                                                    for (int k = 0; k < item.MITTENTE.Count; k++)
                                                    {
                                                        if ((item.MITTENTE[k] as DocsPaVO.Modelli_Trasmissioni.MittDest).ID_CORR_GLOBALI.ToString() == infoUtente.idCorrGlobali)
                                                        {
                                                            LibroFirma.LibroFirmaManager.effettuaTrasmissioneDocDaModello(item, stato.SYSTEM_ID.ToString(), infoDoc, infoUtente, schedaDoc);
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        logger.Debug("* La fattura non prevede allegati - il processo prosegue in automatico *");

                        // Imposto la fattura come da inviare
                        result = ImportFatture_SetStatoInvio(schedaDoc.systemId, true, false);

                        // Aggiorno lo stato del diagramma
                        int idDiagramma = DiagrammiStato.DiagrammiStato.getDiagrammaAssociato(idTipologia);
                        if(idDiagramma != 0)
                        {
                            DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = DiagrammiStato.DiagrammiStato.getDiagrammaById(idDiagramma.ToString());
                            if(diagramma != null)
                            {
                                foreach(DocsPaVO.DiagrammaStato.Stato stato in diagramma.STATI)
                                {
                                    if(stato.STATO_INIZIALE)
                                    {
                                        DiagrammiStato.DiagrammiStato.salvaModificaStato(schedaDoc.systemId, stato.SYSTEM_ID.ToString(), diagramma, infoUtente.idPeople, infoUtente, string.Empty);
                                        break;
                                    }
                                }
                            }
                        }

                    }
                    
                    trContxt.Complete();
                }
                catch (Exception ex)
                {
                    logger.Debug(ex.Message);
                    retVal = false;
                }
            }

            return retVal;
        }

        public static bool LogFattura(string numero, string dataCreazione, string fornitore, string logMessage, string idProfile)
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
            return f.LogFattura(numero, convertedDate, fornitore, logMessage, idProfile);
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
        private static FatturaElettronicaType CreateFatturaXML(FatturaPA model, string idFattura)
        {

            FatturaElettronicaType fattura = new FatturaElettronicaType();
            #region HEADER [1.x]
            // <FatturaElettronicaHeader> [1.x]
            fattura.FatturaElettronicaHeader = new FatturaElettronicaHeaderType();

            #region DATI TRASMISSIONE [1.1.x]
            // <FatturaElettronicaHeader><DatiTrasmissione><..> [1.1.x]
            fattura.FatturaElettronicaHeader.DatiTrasmissione = new DatiTrasmissioneType();
            // <FatturaElettronicaHeader><DatiTrasmissione><IdTrasmittente><..> [1.1.1.x]
            fattura.FatturaElettronicaHeader.DatiTrasmissione.IdTrasmittente = new IdFiscaleType();
            // <FatturaElettronicaHeader><DatiTrasmissione><IdTrasmittente><IdPaese> [1.1.1.1]
            fattura.FatturaElettronicaHeader.DatiTrasmissione.IdTrasmittente.IdPaese = model.trasmittenteIdPaese.Trim();
            // <FatturaElettronicaHeader><DatiTrasmissione><IdTrasmittente><IdCodice> [1.1.1.2]
            fattura.FatturaElettronicaHeader.DatiTrasmissione.IdTrasmittente.IdCodice = model.trasmittenteIdCodice.Trim();
            // <FatturaElettronicaHeader><DatiTrasmissione><ProgressivoInvio> [1.1.2]
            // Il campo ProgressivoInvio viene valorizzato dopo che la fattura viene acquisita
            // e corrisponde all'id documento in VtDocs
            // <FatturaElettronicaHeader><DatiTrasmissione><FormatoTrasmissione> [1.1.3]
            if (!string.IsNullOrEmpty(model.formatoTrasmissione) && model.formatoTrasmissione.Equals("FPA12"))
            {
                fattura.versione = FormatoTrasmissioneType.FPA12;
                fattura.FatturaElettronicaHeader.DatiTrasmissione.FormatoTrasmissione = FormatoTrasmissioneType.FPA12;
            }
            else
            {
                fattura.versione = FormatoTrasmissioneType.FPR12;
                fattura.FatturaElettronicaHeader.DatiTrasmissione.FormatoTrasmissione = FormatoTrasmissioneType.FPR12;
            }
            // <FatturaElettronicaHeader><DatiTrasmissione><CodiceDestinatario> [1.1.4]
            fattura.FatturaElettronicaHeader.DatiTrasmissione.CodiceDestinatario = model.codiceIPA.Trim();
            // <FatturaElettronicaHeader><DatiTrasmissione><ContattiTrasmittente><..> [1.1.5.x]
            if(model.trasmittenteTelefono != null && model.trasmittenteMail!= null && !string.IsNullOrEmpty(model.trasmittenteTelefono.Trim()) || !string.IsNullOrEmpty(model.trasmittenteMail.Trim()))
            {

                fattura.FatturaElettronicaHeader.DatiTrasmissione.ContattiTrasmittente = new ContattiTrasmittenteType();
                // <FatturaElettronicaHeader><DatiTrasmissione><ContattiTrasmittente><Telefono> [1.1.5.1]
                fattura.FatturaElettronicaHeader.DatiTrasmissione.ContattiTrasmittente.Telefono = model.trasmittenteTelefono.Trim();
                // <FatturaElettronicaHeader><DatiTrasmissione><ContattiTrasmittente><Email> [1.1.5.2]
                fattura.FatturaElettronicaHeader.DatiTrasmissione.ContattiTrasmittente.Email = model.trasmittenteMail.Trim();
            }
            if(model.formatoTrasmissione == "FPR12" && model.codiceIPA.Trim().Contains("0000000") && !string.IsNullOrEmpty(model.pecDestinatario.Trim()))
            {
                fattura.FatturaElettronicaHeader.DatiTrasmissione.PECDestinatario = model.pecDestinatario;
            }
            else
            {
                fattura.FatturaElettronicaHeader.DatiTrasmissione.PECDestinatario = null;
            }



            #endregion // Dati Trasmissione [1.1.x]

            #region CEDENTE PRESTATORE [1.2.x]
            // <FatturaElettronicaHeader><CedentePrestatore><..> [1.2.x]
            fattura.FatturaElettronicaHeader.CedentePrestatore = new CedentePrestatoreType();
            // <FatturaElettronicaHeader><CedentePrestatore><DatiAnagrafici><..> [1.2.1.x]
            fattura.FatturaElettronicaHeader.CedentePrestatore.DatiAnagrafici = new DatiAnagraficiCedenteType();
            // <FatturaElettronicaHeader><CedentePrestatore><DatiAnagrafici><IdFiscaleIVA><..> [1.2.1.1.x]
            fattura.FatturaElettronicaHeader.CedentePrestatore.DatiAnagrafici.IdFiscaleIVA = new IdFiscaleType();
            // <FatturaElettronicaHeader><CedentePrestatore><DatiAnagrafici><IdFiscaleIVA><IdPaese> [1.2.1.1.1]
            fattura.FatturaElettronicaHeader.CedentePrestatore.DatiAnagrafici.IdFiscaleIVA.IdPaese = model.cedente.idPaese.Trim();
            // <FatturaElettronicaHeader><CedentePrestatore><DatiAnagrafici><IdFiscaleIVA><IdCodice> [1.2.1.1.2]
            fattura.FatturaElettronicaHeader.CedentePrestatore.DatiAnagrafici.IdFiscaleIVA.IdCodice = model.cedente.idCodice.Trim();
            // <FatturaElettronicaHeader><CedentePrestatore><DatiAnagrafici><Anagrafica> [1.2.1.3.x]
            fattura.FatturaElettronicaHeader.CedentePrestatore.DatiAnagrafici.Anagrafica = new AnagraficaType();
            // <FatturaElettronicaHeader><CedentePrestatore><DatiAnagrafici><Anagrafica><Denominazione> [1.2.1.3.1]
            fattura.FatturaElettronicaHeader.CedentePrestatore.DatiAnagrafici.Anagrafica.ItemsElementName = new ItemsChoiceType[] { ItemsChoiceType.Denominazione };
            fattura.FatturaElettronicaHeader.CedentePrestatore.DatiAnagrafici.Anagrafica.Items = new string[] { model.cedente.denominazione.Trim() };
            // <FatturaElettronicaHeader><CedentePrestatore><DatiAnagrafici><Anagrafica><RegimeFiscale> [1.2.1.8]
            // ??
            // <FatturaElettronicaHeader><CedentePrestatore><Sede><..> [1.2.2.x]
            fattura.FatturaElettronicaHeader.CedentePrestatore.Sede = new IndirizzoType();
            // <FatturaElettronicaHeader><CedentePrestatore><Sede><Indirizzo> [1.2.2.1]
            fattura.FatturaElettronicaHeader.CedentePrestatore.Sede.Indirizzo = model.cedente.indirizzo.Trim();
            // <FatturaElettronicaHeader><CedentePrestatore><Sede><NumeroCivico> [1.2.2.2]
            if (model.cedente.numCivico != null && !string.IsNullOrEmpty(model.cedente.numCivico.Trim()))
                fattura.FatturaElettronicaHeader.CedentePrestatore.Sede.NumeroCivico = model.cedente.numCivico.Trim();
            // <FatturaElettronicaHeader><CedentePrestatore><Sede><CAP> [1.2.2.3]
            fattura.FatturaElettronicaHeader.CedentePrestatore.Sede.CAP = model.cedente.CAP.Trim();
            // <FatturaElettronicaHeader><CedentePrestatore><Sede><Comune> [1.2.2.4]
            fattura.FatturaElettronicaHeader.CedentePrestatore.Sede.Comune = model.cedente.comune.Trim();
            // <FatturaElettronicaHeader><CedentePrestatore><Sede><Provincia> [1.2.2.5]
            fattura.FatturaElettronicaHeader.CedentePrestatore.Sede.Provincia = model.cedente.provincia.Trim();
            // <FatturaElettronicaHeader><CedentePrestatore><Sede><Nazione> [1.2.2.6]
            fattura.FatturaElettronicaHeader.CedentePrestatore.Sede.Nazione = model.cedente.nazione.Trim();
            // <FatturaElettronicaHeader><CedentePrestatore><IscrizioneREA><..> [1.2.4.x]
            fattura.FatturaElettronicaHeader.CedentePrestatore.IscrizioneREA = new IscrizioneREAType();
            // <FatturaElettronicaHeader><CedentePrestatore><IscrizioneREA><Ufficio> [1.2.4.1]
            fattura.FatturaElettronicaHeader.CedentePrestatore.IscrizioneREA.Ufficio = model.cedente.ufficio.Trim();
            // <FatturaElettronicaHeader><CedentePrestatore><IscrizioneREA><NumeroREA> [1.2.4.2]
            fattura.FatturaElettronicaHeader.CedentePrestatore.IscrizioneREA.NumeroREA = model.cedente.numeroREA.Trim();
            // <FatturaElettronicaHeader><CedentePrestatore><IscrizioneREA><CapitaleSociale> [1.2.4.3]
            decimal _capitaleSociale = 0;
            bool _conversioneCapitaleSociale = Decimal.TryParse(model.cedente.capitaleSociale.Replace('.', ','), out _capitaleSociale);
            fattura.FatturaElettronicaHeader.CedentePrestatore.IscrizioneREA.CapitaleSocialeSpecified = _conversioneCapitaleSociale;
            fattura.FatturaElettronicaHeader.CedentePrestatore.IscrizioneREA.CapitaleSociale = _capitaleSociale;
            // <FatturaElettronicaHeader><CedentePrestatore><IscrizioneREA><SocioUnico> [1.2.4.4]
            if (model.cedente.socioUnico != null && !string.IsNullOrEmpty(model.cedente.socioUnico.Trim()))
            {
                fattura.FatturaElettronicaHeader.CedentePrestatore.IscrizioneREA.CapitaleSocialeSpecified = true;
                if (model.cedente.socioUnico.Equals("SU"))
                    fattura.FatturaElettronicaHeader.CedentePrestatore.IscrizioneREA.SocioUnico = SocioUnicoType.SU;
                else
                    fattura.FatturaElettronicaHeader.CedentePrestatore.IscrizioneREA.SocioUnico = SocioUnicoType.SM;
            }
            else
                fattura.FatturaElettronicaHeader.CedentePrestatore.IscrizioneREA.CapitaleSocialeSpecified = false;  // gia di default
            // <FatturaElettronicaHeader><CedentePrestatore><IscrizioneREA><StatoLiquidazione> [1.2.4.5]
            if (model.cedente.statoLiquidazione != null && !string.IsNullOrEmpty(model.cedente.statoLiquidazione.Trim()) && model.cedente.statoLiquidazione.Equals("LN"))
                fattura.FatturaElettronicaHeader.CedentePrestatore.IscrizioneREA.StatoLiquidazione = StatoLiquidazioneType.LN;
            else
                fattura.FatturaElettronicaHeader.CedentePrestatore.IscrizioneREA.StatoLiquidazione = StatoLiquidazioneType.LS;

            #endregion // Cedente Prestatore [1.2.x]

            #region CESSIONARIO COMMITTENTE [1.4.x]
            // <FatturaElettronicaHeader><CessionarioCommittente><..> [1.4.x]
            fattura.FatturaElettronicaHeader.CessionarioCommittente = new CessionarioCommittenteType();
            // <FatturaElettronicaHeader><CessionarioCommittente><DatiAnagrafici><..> [1.4.1.x]
            fattura.FatturaElettronicaHeader.CessionarioCommittente.DatiAnagrafici = new DatiAnagraficiCessionarioType();
            // <FatturaElettronicaHeader><CessionarioCommittente><DatiAnagrafici><IdFiscaleIVA> [1.4.1.1.x]
            if(model.cessionario.idPaese != null && model.cessionario.idCodiceI !=  null && !string.IsNullOrEmpty(model.cessionario.idPaese.Trim()) && !string.IsNullOrEmpty(model.cessionario.idCodiceI.Trim()))
            {
                fattura.FatturaElettronicaHeader.CessionarioCommittente.DatiAnagrafici.IdFiscaleIVA = new IdFiscaleType()
                {
                    IdCodice = model.cessionario.idCodiceI.Trim(),
                    IdPaese = model.cessionario.idPaese.Trim()
                };
            }
            // <FatturaElettronicaHeader><CessionarioCommittente><DatiAnagrafici><CodiceFiscale> [1.4.1.2]
            if (!string.IsNullOrEmpty(model.cessionario.idCodiceF))
            {
                fattura.FatturaElettronicaHeader.CessionarioCommittente.DatiAnagrafici.CodiceFiscale = model.cessionario.idCodiceF.Trim();
            }
            // <FatturaElettronicaHeader><CessionarioCommittente><DatiAnagrafici><Anagrafica> [1.4.1.3.x]
            fattura.FatturaElettronicaHeader.CessionarioCommittente.DatiAnagrafici.Anagrafica = new AnagraficaType();
            // <FatturaElettronicaHeader><CessionarioCommittente><DatiAnagrafici><Anagrafica><Denominazione> [1.4.1.3.1]
            fattura.FatturaElettronicaHeader.CessionarioCommittente.DatiAnagrafici.Anagrafica.ItemsElementName = new ItemsChoiceType[] { ItemsChoiceType.Denominazione };
            fattura.FatturaElettronicaHeader.CessionarioCommittente.DatiAnagrafici.Anagrafica.Items = new string[] { model.cessionario.denominazione.Trim() };
            // <FatturaElettronicaHeader><CessionarioCommittente><Sede> [1.4.2.x]
            fattura.FatturaElettronicaHeader.CessionarioCommittente.Sede = new IndirizzoType();
            // <FatturaElettronicaHeader><CessionarioCommittente><Sede><Indirizzo> [1.4.2.1]
            fattura.FatturaElettronicaHeader.CessionarioCommittente.Sede.Indirizzo = model.cessionario.indirizzo.Trim();
            // <FatturaElettronicaHeader><CessionarioCommittente><Sede><NumeroCivico> [1.4.2.2]
            if (model.cessionario.numCivico != null && !string.IsNullOrEmpty(model.cessionario.numCivico.Trim()))
                fattura.FatturaElettronicaHeader.CessionarioCommittente.Sede.NumeroCivico = model.cessionario.numCivico.Trim();
            // <FatturaElettronicaHeader><CessionarioCommittente><Sede><CAP> [1.4.2.3]
            fattura.FatturaElettronicaHeader.CessionarioCommittente.Sede.CAP = model.cessionario.CAP.Trim();
            // <FatturaElettronicaHeader><CessionarioCommittente><Sede><Comune> [1.4.2.4]
            fattura.FatturaElettronicaHeader.CessionarioCommittente.Sede.Comune = model.cessionario.comune.Trim();
            // <FatturaElettronicaHeader><CessionarioCommittente><Sede><Provincia> [1.4.2.5]
            if(model.cessionario.provincia != null && !string.IsNullOrEmpty(model.cessionario.provincia.Trim()))
                fattura.FatturaElettronicaHeader.CessionarioCommittente.Sede.Provincia = model.cessionario.provincia.Trim();
            // <FatturaElettronicaHeader><CessionarioCommittente><Sede><Nazione> [1.4.2.6]
            fattura.FatturaElettronicaHeader.CessionarioCommittente.Sede.Nazione = model.cessionario.nazione.Trim();

            #endregion // cessionario commmittente [1.4.x]

            #endregion // Header [1.x]

            #region BODY [2.x]
            // <FatturaElettronicaBody> [2.x]
            FatturaElettronicaBodyType _body = new FatturaElettronicaBodyType();

            #region DatiGenerali [2.1.x]
            // <FatturaElettronicaBody><DatiGenerali> [2.1.x]
            _body.DatiGenerali = new DatiGeneraliType();

            #region DatiGeneraliDocumento [2.1.1.x]
            // <FatturaElettronicaBody><DatiGenerali><DatiGeneraliDocumento> [2.1.1.x]
            _body.DatiGenerali.DatiGeneraliDocumento = new DatiGeneraliDocumentoType();
            // <FatturaElettronicaBody><DatiGenerali><DatiGeneraliDocumento><TipoDocumento> [2.1.1.1]
            //if(Enum.IsDefined(typeof(TipoDocumentoType), model.tipoDoc.Trim()))
            _body.DatiGenerali.DatiGeneraliDocumento.TipoDocumento = (TipoDocumentoType)Enum.Parse(typeof(TipoDocumentoType), model.tipoDoc.Trim());
            // <FatturaElettronicaBody><DatiGenerali><DatiGeneraliDocumento><Divisa> [2.1.1.2]
            _body.DatiGenerali.DatiGeneraliDocumento.Divisa = model.divisa.Trim();
            // <FatturaElettronicaBody><DatiGenerali><DatiGeneraliDocumento><Data> [2.1.1.3]
            _body.DatiGenerali.DatiGeneraliDocumento.Data = model.dataDoc;
            // <FatturaElettronicaBody><DatiGenerali><DatiGeneraliDocumento><Numero> [2.1.1.4]
            _body.DatiGenerali.DatiGeneraliDocumento.Numero = model.numeroFattura.Trim();
            // <FatturaElettronicaBody><DatiGenerali><DatiGeneraliDocumento><ImportoTotaleDocumento> [2.1.1.9]
            decimal _importoTotaleFattura = 0;
            bool _conversioneImportoTotaleFattura = Decimal.TryParse(model.importoTotaleDoc.Replace('.', ','), out _importoTotaleFattura);
            _body.DatiGenerali.DatiGeneraliDocumento.ImportoTotaleDocumento = _importoTotaleFattura;
            _body.DatiGenerali.DatiGeneraliDocumento.ImportoTotaleDocumentoSpecified = _conversioneImportoTotaleFattura;

            #endregion // DatiGeneraliDocumento [2.2.2.x]

            #region DatiOrdineAcquisto [2.1.2.x]
            // COMMENTO PERCHE' ORA E' GESTITO SU PIU' LINEE
            // <FatturaElettronicaBody><DatiGenerali><DatiOrdineAcquisto> [2.1.2.x]
            //DatiDocumentiCorrelatiType _ordineAcquisto = new DatiDocumentiCorrelatiType();
            //// <FatturaElettronicaBody><DatiGenerali><DatiOrdineAcquisto><IdDocumento> [2.1.2.2]
            //_ordineAcquisto.IdDocumento = model.idOrdineAcquisto.Trim();

            //// <FatturaElettronicaBody><DatiGenerali><DatiOrdineAcquisto><NumItem> [2.1.2.4]
            //_ordineAcquisto.NumItem = model.codiceSottoprogetto.Trim();
            //// <FatturaElettronicaBody><DatiGenerali><DatiOrdineAcquisto><CodiceCommessaConvenzione> [2.1.2.5]
            //_ordineAcquisto.CodiceCommessaConvenzione = model.codiceComponente.Trim();
            //// <FatturaElettronicaBody><DatiGenerali><DatiOrdineAcquisto><CodiceCUP> [2.1.2.6]
            //_ordineAcquisto.CodiceCUP = model.CUPOrdineAcquisto.Trim();
            //// <FatturaElettronicaBody><DatiGenerali><DatiOrdineAcquisto><CodiceCIG> [2.1.2.7]
            //_ordineAcquisto.CodiceCIG = model.CIGOrdineAcquisto.Trim();

            List<DatiDocumentiCorrelatiType> _lineeOrdineAcquisto = new List<DatiDocumentiCorrelatiType>();

            if (model.ordineAcquisto != null && model.ordineAcquisto.Count > 0)
            {
                // CASO FATTURE SOGEI
                foreach (DatiOrdineAcquisto line in model.ordineAcquisto)
                {
                    DatiDocumentiCorrelatiType _ordineAcquisto = new DatiDocumentiCorrelatiType();

                    _ordineAcquisto.NumeroLineaSAP = line.NumeroLineaSAP; // Utilizzato per i campi custom
                    _ordineAcquisto.RiferimentoNumeroLinea = line.RiferimentoNumeroLinea;
                    _ordineAcquisto.IdDocumento = line.IdDocumento;
                    _ordineAcquisto.NumItem = line.NumItem;
                    _ordineAcquisto.CodiceCommessaConvenzione = string.IsNullOrEmpty(line.CodiceCommessaConvenzione) ? null : line.CodiceCommessaConvenzione;
                    _ordineAcquisto.CodiceCUP = string.IsNullOrEmpty(line.CodiceCUP) ? null : line.CodiceCUP;
                    _ordineAcquisto.CodiceCIG = string.IsNullOrEmpty(line.CodiceCIG) ? null : line.CodiceCIG;

                    if (line.Data != null)
                        _ordineAcquisto.Data = (DateTime)line.Data;

                    _lineeOrdineAcquisto.Add(_ordineAcquisto);
                }
            }
            else
            {
                DatiDocumentiCorrelatiType _ordineAcquisto = new DatiDocumentiCorrelatiType();
                if(model.ordineAcquisto != null && model.ordineAcquisto.Count == 1)
                {
                    _ordineAcquisto.NumeroLineaSAP = ((DatiOrdineAcquisto)model.ordineAcquisto[0]).NumeroLineaSAP;
                }

                //_ordineAcquisto.IdDocumento = model.idOrdineAcquisto.Trim();
                //_ordineAcquisto.CodiceCUP = model.CUPOrdineAcquisto.Trim();
                //_ordineAcquisto.CodiceCIG = model.CIGOrdineAcquisto.Trim();

                _ordineAcquisto.IdDocumento = string.IsNullOrEmpty(model.idOrdineAcquisto) ? null : model.idOrdineAcquisto;
                _ordineAcquisto.CodiceCUP = string.IsNullOrEmpty(model.CUPOrdineAcquisto) ? null : model.CUPOrdineAcquisto;
                _ordineAcquisto.CodiceCIG = string.IsNullOrEmpty(model.CIGOrdineAcquisto) ? null : model.CIGOrdineAcquisto;

                _lineeOrdineAcquisto.Add(_ordineAcquisto);
            }

            //_body.DatiGenerali.DatiOrdineAcquisto = new DatiDocumentiCorrelatiType[] { _ordineAcquisto };
            _body.DatiGenerali.DatiOrdineAcquisto = _lineeOrdineAcquisto.ToArray<DatiDocumentiCorrelatiType>();
            #endregion //DatiOrdineAcquisto [2.1.2.x]

            #region DatiContratto [2.1.3.x]
            // <FatturaElettronicaBody><DatiGenerali><DatiContratto> [2.1.3.x]
            DatiDocumentiCorrelatiType _datiContratto = new DatiDocumentiCorrelatiType();
            // <FatturaElettronicaBody><DatiGenerali><DatiContratto><IdDocumento> [2.1.3.2]
            _datiContratto.IdDocumento = string.IsNullOrEmpty(model.idContratto) ? null : model.idContratto;
            // <FatturaElettronicaBody><DatiGenerali><DatiContratto><CodiceCommessaConvenzione> [2.1.3.5]
            _datiContratto.CodiceCommessaConvenzione = string.IsNullOrEmpty(model.dipartimentoMef) ? null : model.dipartimentoMef;
            // <FatturaElettronicaBody><DatiGenerali><DatiContratto><CodiceCUP> [2.1.3.6]
            _datiContratto.CodiceCUP = string.IsNullOrEmpty(model.CUPContratto) ? null : model.CUPContratto;
            // <FatturaElettronicaBody><DatiGenerali><DatiContratto><CodiceCIG> [2.1.3.7]
            _datiContratto.CodiceCIG = string.IsNullOrEmpty(model.CIGContratto) ? null : model.CIGContratto;

            _body.DatiGenerali.DatiContratto = new DatiDocumentiCorrelatiType[] { _datiContratto };
            #endregion // DatiContratto [2.1.3.x]

            #endregion // DatiGenerali [2.1.x]

            #region DatiBeniServizi [2.2.x]
            // <FatturaElettronicaBody><DatiBeniServizi> [2.2.x]
            _body.DatiBeniServizi = new DatiBeniServiziType();

            #region DettaglioLinee [2.2.1.x]
            // <FatturaElettronicaBody><DatiBeniServizi><DettaglioLinee> [2.2.1.x]
            List<DettaglioLineeType> _dettaglioLinee = new List<DettaglioLineeType>();

            foreach (DatiBeniServizi line in model.servizi)  // model.servizi è un ArrayList gia inizializzato
            {
                DettaglioLineeType _linea = new DettaglioLineeType();
                // Riferimento numero linea SAP per gestione campi custom
                _linea.NumeroLineaSAP = line.numeroLineaSAP.Trim();
                // <FatturaElettronicaBody><DatiBeniServizi><DettaglioLinee><NumeroLinea> [2.2.1.1]
                _linea.NumeroLinea = line.numeroLinea.Trim();
                // <FatturaElettronicaBody><DatiBeniServizi><DettaglioLinee><Descrizione> [2.2.1.4]
                _linea.Descrizione = line.descrizione.Trim();
                // <FatturaElettronicaBody><DatiBeniServizi><DettaglioLinee><Quantita> [2.2.1.5]
                Decimal _quantitaLinea = 0;
                _linea.QuantitaSpecified = Decimal.TryParse(line.quantita.Replace('.', ','), out _quantitaLinea);
                _linea.Quantita = _quantitaLinea;
                // <FatturaElettronicaBody><DatiBeniServizi><DettaglioLinee><UnitaMisura> [2.2.1.6]
                _linea.UnitaMisura = line.unitaMisura;
                // <FatturaElettronicaBody><DatiBeniServizi><DettaglioLinee><DataInizioPeriodo> [2.2.1.7]
                _linea.DataInizioPeriodoSpecified = line.dataInizioPeriodo.HasValue;
                _linea.DataInizioPeriodo = line.dataInizioPeriodo ?? DateTime.Now;
                // <FatturaElettronicaBody><DatiBeniServizi><DettaglioLinee><DataFinePeriodo> [2.2.1.8]
                _linea.DataFinePeriodoSpecified = line.dataFinePeriodo.HasValue;
                _linea.DataFinePeriodo = line.dataFinePeriodo ?? DateTime.Now;
                // <FatturaElettronicaBody><DatiBeniServizi><DettaglioLinee><PrezzoUnitario> [2.2.1.9]
                Decimal _prezzoUnitario = 0;
                Decimal.TryParse(line.prezzoUnitario.Replace('.', ','), out _prezzoUnitario); // puo non andare a buon fine e ritornare 0
                _linea.PrezzoUnitario = _prezzoUnitario;
                // <FatturaElettronicaBody><DatiBeniServizi><DettaglioLinee><PrezzoTotale> [2.2.1.11]
                Decimal _prezzoTotale = 0;
                Decimal.TryParse(line.prezzoTotale.Replace('.', ','), out _prezzoTotale);
                _linea.PrezzoTotale = _prezzoTotale;
                // <FatturaElettronicaBody><DatiBeniServizi><DettaglioLinee><AliquotaIVA> [2.2.1.12]
                Decimal _aliquotaIVA = 0;
                Decimal.TryParse(line.aliquotaIVA.Replace('.', ','), out _aliquotaIVA);
                _linea.AliquotaIVA = _aliquotaIVA;
                // <FatturaElettronicaBody><DatiBeniServizi><DettaglioLinee><Natura> [2.2.1.14]
                if (_linea.AliquotaIVA == 0)
                {
                    _linea.Natura = NaturaType.N4;
                    _linea.NaturaSpecified = true;
                }
                else
                    _linea.NaturaSpecified = false;
                // <FatturaElettronicaBody><DatiBeniServizi><DettaglioLinee><RiferimentoAmministrazione> [2.2.1.15]
                _linea.RiferimentoAmministrazione = string.IsNullOrEmpty(line.obiettivoFase) ? null : line.obiettivoFase;
                // <FatturaElettronicaBody><DatiBeniServizi><DettaglioLinee><AltriDatiGestionali> [2.2.1.16]
                List<AltriDatiGestionaliType> _altriDatiGestionali = new List<AltriDatiGestionaliType>();
                foreach (DatiBeniServizi.DatiGestionali dati in line.altriDatiGestionali)
                {
                    AltriDatiGestionaliType _datiGestionali = new AltriDatiGestionaliType();
                    // <FatturaElettronicaBody><DatiBeniServizi><DettaglioLinee><AltriDatiGestionali><TipoDato> [2.2.1.16.1]
                    _datiGestionali.TipoDato = dati.tipoDati;

                    _altriDatiGestionali.Add(_datiGestionali);
                }

                if (_altriDatiGestionali.Count > 0)
                    _linea.AltriDatiGestionali = _altriDatiGestionali.ToArray<AltriDatiGestionaliType>();
                _dettaglioLinee.Add(_linea);
            }
            if(_dettaglioLinee.Count > 0)
                _body.DatiBeniServizi.DettaglioLinee = _dettaglioLinee.ToArray<DettaglioLineeType>();
            #endregion // DettaglioLinee [2.2.1.x]

            #region DatiRiepilogo [2.2.2.x]
            // <FatturaElettronicaBody><DatiBeniServizi><DatiRiepilogo> [2.2.2.x]
            DatiRiepilogoType _datiRiepilogo = new DatiRiepilogoType();
            // <FatturaElettronicaBody><DatiBeniServizi><DatiRiepilogo><AliquotaIVA> [2.2.2.1]
            Decimal _aliquotaIvaDatiRiepilogo = 0;
            Decimal.TryParse(model.aliquotaIVA.Replace('.', ','), out _aliquotaIvaDatiRiepilogo); // puo non andare a buon fine e ritornare 0
            _datiRiepilogo.AliquotaIVA = _aliquotaIvaDatiRiepilogo;
            // <FatturaElettronicaBody><DatiBeniServizi><DatiRiepilogo><Natura> [2.2.2.2]
            if (_datiRiepilogo.AliquotaIVA == 0)
            {
                _datiRiepilogo.NaturaSpecified = true;
                _datiRiepilogo.Natura = NaturaType.N4;
            }
            else
                _datiRiepilogo.NaturaSpecified = false;
            // <FatturaElettronicaBody><DatiBeniServizi><DatiRiepilogo><ImponibileImporto> [2.2.2.5]
            Decimal _imponibileImportoDatiRiepilogo = 0;
            Decimal.TryParse(model.imponibileImporto.Replace('.', ','), out _imponibileImportoDatiRiepilogo);
            _datiRiepilogo.ImponibileImporto = _imponibileImportoDatiRiepilogo;
            // <FatturaElettronicaBody><DatiBeniServizi><DatiRiepilogo><Imposta> [2.2.2.6]
            Decimal _impostaDatiRiepilogo = 0;
            Decimal.TryParse(model.imposta.Replace('.', ','), out _impostaDatiRiepilogo);
            _datiRiepilogo.Imposta = _impostaDatiRiepilogo;
            // <FatturaElettronicaBody><DatiBeniServizi><DatiRiepilogo><EsigibilitaIVA> [2.2.2.7]
            _datiRiepilogo.EsigibilitaIVA = (EsigibilitaIVAType)Enum.Parse(typeof(EsigibilitaIVAType), model.esigibilitaIVA, true);
            _datiRiepilogo.EsigibilitaIVASpecified = true;

            _body.DatiBeniServizi.DatiRiepilogo = new DatiRiepilogoType[] { _datiRiepilogo };
            #endregion // DatiRiepilogo [2.2.2.x]

            #endregion // DatiBeniSerizi [2.2.x]

            #region DatiPagamento [2.4.x]
            // <FatturaElettronicaBody><DatiPagamento> [2.4.x]
            DatiPagamentoType _datiPagamento = new DatiPagamentoType();
            // <FatturaElettronicaBody><DatiPagamento><CondizioniPagamento> [2.4.1]
            _datiPagamento.CondizioniPagamento = (CondizioniPagamentoType)Enum.Parse(typeof(CondizioniPagamentoType), model.pagamentoCondizioni, true);
            // <FatturaElettronicaBody><DatiPagamento><DettaglioPagamento> [2.4.2.x]
            DettaglioPagamentoType _dettaglioPagamento = new DettaglioPagamentoType();
            // <FatturaElettronicaBody><DatiPagamento><DettaglioPagamento><ModalitaPagamento> [2.4.2.2]
            _dettaglioPagamento.ModalitaPagamento = (ModalitaPagamentoType)Enum.Parse(typeof(ModalitaPagamentoType), model.pagamentoModalita, true);
            // <FatturaElettronicaBody><DatiPagamento><DettaglioPagamento><DataRiferimentoTerminiPagamento> [2.4.2.3]

            // 15/02/2019 I CAMPI 2.4.2.3 E 2.4.2.4 NON SONO PIU' INSERITI NEL TRACCIATO
            _dettaglioPagamento.DataRiferimentoTerminiPagamento = model.dataRifTerminiPagamento;
            _dettaglioPagamento.DataRiferimentoTerminiPagamentoSpecified = true;

            //_dettaglioPagamento.GiorniTerminiPagamento = model.giorniTerminiPagamento; // [2.4.2.4]
            //_dettaglioPagamento.DataRiferimentoTerminiPagamentoSpecified = false;

            // [2.4.2.5]
            if(model.dataScadenzaPagamento != null)
            {
                _dettaglioPagamento.DataScadenzaPagamento = model.dataScadenzaPagamento;
                _dettaglioPagamento.DataScadenzaPagamentoSpecified = true;
            }
            
            // <FatturaElettronicaBody><DatiPagamento><DettaglioPagamento><ImportoPagamento> [2.4.2.6]
            Decimal _importoPagamento = 0;
            Decimal.TryParse(model.pagamentoImporto.Replace('.', ','), out _importoPagamento);
            _dettaglioPagamento.ImportoPagamento = _importoPagamento;
            // <FatturaElettronicaBody><DatiPagamento><DettaglioPagamento><IstitutoFinanziario> [2.4.2.12]
            _dettaglioPagamento.IstitutoFinanziario = model.istitutoFinanziario.Trim();
            // <FatturaElettronicaBody><DatiPagamento><DettaglioPagamento><IBAN> [2.4.2.13]
            _dettaglioPagamento.IBAN = model.IBAN.Trim();
            // <FatturaElettronicaBody><DatiPagamento><DettaglioPagamento><BIC> [2.4.2.13]
            _dettaglioPagamento.BIC = model.BIC.Trim();
            _datiPagamento.DettaglioPagamento = new DettaglioPagamentoType[] { _dettaglioPagamento };
            _body.DatiPagamento = new DatiPagamentoType[] { _datiPagamento };
            #endregion // DatiPagamento [2.4.x]

            fattura.FatturaElettronicaBody = new FatturaElettronicaBodyType[] { _body };
            #endregion //Body [2.x]

            logger.Debug("**** CARICAMENTO CAMPI CUSTOM");
            AddCampiCustom(fattura, idFattura);

            return fattura;


        }

        private static string createXMLFattura(FatturaPA fattura, string idFattura)
        {
            // test
            FatturaElettronicaType fatturaSerializzata = CreateFatturaXML(fattura, idFattura);
            XmlSerializer serializer = new XmlSerializer(typeof(FatturaElettronicaType));
            string xml = "";

            // Rifatto utilizzando prima un memorystream per forzare la codifica in UTF8
            using(var stream = new MemoryStream())
            {
                using(var writer = XmlWriter.Create(stream))
                {
                    try
                    {
                        serializer.Serialize(writer, fatturaSerializzata);
                        xml = Encoding.UTF8.GetString(stream.ToArray());

                        string byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                        if (xml.StartsWith(byteOrderMarkUtf8))
                        {
                            logger.Debug(">> Rimuovo byteOrderMark");
                            xml = xml.Remove(0, byteOrderMarkUtf8.Length);
                        }
                    }
                    catch(Exception ex)
                    {
                        logger.Debug(ex.Message);
                    }
                }
            }
            

            //using (var sww = new StringWriter())
            //{
            //    using (XmlWriter writer = XmlWriter.Create(sww, new XmlWriterSettings() {Encoding = Encoding.UTF8, CloseOutput = true  }))
            //    {
            //        try
            //        {
            //            serializer.Serialize(writer, fatturaSerializzata);
            //            xml = sww.ToString(); // Your XML
            //        }
            //        catch (Exception ex)
            //        {
            //            var s = ex.Message;
            //        }

            //    }
            //}
            return xml;
            //fine test

            #region OLD CODE
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
                InvoiceNamespace = System.Configuration.ConfigurationManager.AppSettings["NAMESPACE_FATTURAPA"];
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
                XmlNode codiceSottoprogetto = body.SelectSingleNode("DatiGenerali/DatiOrdineAcquisto/NumItem");
                codiceSottoprogetto.InnerText = fattura.codiceSottoprogetto;
                XmlNode codiceComponente = body.SelectSingleNode("DatiGenerali/DatiOrdineAcquisto/CodiceCommessaConvenzione");
                codiceComponente.InnerText = fattura.codiceComponente;
                XmlNode ordineCUP = body.SelectSingleNode("DatiGenerali/DatiOrdineAcquisto/CodiceCUP");
                ordineCUP.InnerText = fattura.CUPOrdineAcquisto;
                XmlNode ordineCIG = body.SelectSingleNode("DatiGenerali/DatiOrdineAcquisto/CodiceCIG");
                ordineCIG.InnerText = fattura.CIGOrdineAcquisto;
                //XmlNode codiceSipaiProgetto = body.SelectSingleNode("DatiGenerali/DatiOrdineAcquisto/IdDocumento");


                XmlNode contrattoId = body.SelectSingleNode("DatiGenerali/DatiContratto/IdDocumento");
                contrattoId.InnerText = fattura.idContratto;
                XmlNode contrattoCUP = body.SelectSingleNode("DatiGenerali/DatiContratto/CodiceCUP");
                contrattoCUP.InnerText = fattura.CUPContratto;
                XmlNode contrattoCIG = body.SelectSingleNode("DatiGenerali/DatiContratto/CodiceCIG");
                contrattoCIG.InnerText = fattura.CIGContratto;
                XmlNode dipartimentoMef = body.SelectSingleNode("DatiGenerali/DatiContratto/CodiceCommessaConvenzione");
                dipartimentoMef.InnerText = fattura.dipartimentoMef;

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
                    XmlElement dataInizioPeriodo = fatturaXML.CreateElement("DataInizioPeriodo");
                    dataInizioPeriodo.InnerText = line.dataInizioPeriodo.HasValue ? line.dataInizioPeriodo.Value.ToString("yyyy-MM-dd") : string.Empty;
                    dettaglioLinee.AppendChild(dataInizioPeriodo);
                    XmlElement dataFinePeriodo = fatturaXML.CreateElement("DataFinePeriodo");
                    dataFinePeriodo.InnerText = line.dataFinePeriodo.HasValue ? line.dataFinePeriodo.Value.ToString("yyyy-MM-dd") : string.Empty;
                    dettaglioLinee.AppendChild(dataFinePeriodo);
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

                    XmlElement riferimentoAmministrazione = fatturaXML.CreateElement("RiferimentoAmministrazione");
                    riferimentoAmministrazione.InnerText = line.obiettivoFase;
                    dettaglioLinee.AppendChild(riferimentoAmministrazione);



                    foreach (DatiBeniServizi.DatiGestionali dati in line.altriDatiGestionali)
                    {
                        XmlElement altriDatiGestionali = fatturaXML.CreateElement("AltriDatiGestionali");

                        XmlElement tipoDati = fatturaXML.CreateElement("TipoDato");
                        tipoDati.InnerText = dati.tipoDati;
                        altriDatiGestionali.AppendChild(tipoDati);

                        // questi nodi verranno creati nel front-end se presenti
                        //XmlElement nodoRiferimentoTesto = fatturaXML.CreateElement("RiferimentoTesto");
                        //altriDatiGestionali.AppendChild(nodoRiferimentoTesto);

                        //XmlElement nodoRiferimentoNumero = fatturaXML.CreateElement("RiferimentoNumero");
                        //altriDatiGestionali.AppendChild(nodoRiferimentoNumero);

                        //XmlElement nodoRiferimentoData = fatturaXML.CreateElement("RiferimentoData");
                        //altriDatiGestionali.AppendChild(nodoRiferimentoData);

                        dettaglioLinee.AppendChild(altriDatiGestionali);
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

                // rimuovo i nodi vuoti (meglio nasconderli
                //XmlNodeList emptyElements = fatturaXML.SelectNodes(@"//*[not(node())]");
                //do
                //{
                //    foreach (XmlNode node in emptyElements)
                //        node.ParentNode.RemoveChild(node);
                //    emptyElements = fatturaXML.SelectNodes(@"//*[not(node())]");
                //} while (emptyElements.Count > 0);

                // fine rimozione nodi vuoti

                result = fatturaXML.OuterXml;
                return result;
            }
            catch (Exception ex)
            {
                result = string.Empty;
                logger.Debug(ex.Message);
                return result;
            }
            #endregion
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

        public static string CheckFatturaPassiva(string numero, string data, string partitaIva)
        {
            string result = string.Empty;

            DocsPaDB.Query_DocsPAWS.Fatturazione f = new DocsPaDB.Query_DocsPAWS.Fatturazione();
            string docnumber = f.CheckFatturaPassiva(numero, data, partitaIva);

            if(string.IsNullOrEmpty(docnumber))
            {
                result = "0";
            }
            else if(docnumber == "-1")
            {
                result = "-1";
            }
            else
            {
                result = "1";
                logger.Debug("Analisi documento ID=" + docnumber);
                logger.Debug("Ricerca stato");
                DocsPaVO.DiagrammaStato.Stato stato = DiagrammiStato.DiagrammiStato.getStatoDoc(docnumber);
                if(stato != null)
                {
                    if(stato.DESCRIZIONE.ToUpper() == "RIFIUTATA")
                    {
                        result = docnumber;
                    }
                }
            }

            return result;
        }

        public static List<DocsPaVO.Fatturazione.AssociazioneFatturaPassiva> InvioNotificheGetFatture(string stato)
        {
            DocsPaDB.Query_DocsPAWS.Fatturazione fatt = new DocsPaDB.Query_DocsPAWS.Fatturazione();
            return fatt.InvioNotificheGetFatture(stato);
        }

        public static bool InvioNotificheSetStatoInvio(string docnumber)
        {
            DocsPaDB.Query_DocsPAWS.Fatturazione fatt = new DocsPaDB.Query_DocsPAWS.Fatturazione();
            return fatt.InvioNotificheSetStatoInvio(docnumber);
        }

        public static string GetIdSdiByIdFattura(string idFattura)
        {
            DocsPaDB.Query_DocsPAWS.Fatturazione fatt = new DocsPaDB.Query_DocsPAWS.Fatturazione();
            return fatt.GetIdSdiByIdFattura(idFattura);
        }

        public static string GetIdSdiByDocnumber(string docnumber)
        {
            DocsPaDB.Query_DocsPAWS.Fatturazione fatt = new DocsPaDB.Query_DocsPAWS.Fatturazione();
            return fatt.GetIdSdiByDocnumber(docnumber);
        }

        public static bool SetIdSdi(string idFattura, string idSdi, string docnumber)
        {
            DocsPaDB.Query_DocsPAWS.Fatturazione fatt = new DocsPaDB.Query_DocsPAWS.Fatturazione();
            if (string.IsNullOrEmpty(fatt.GetIdSdiByIdFattura(idFattura)))
                return fatt.InsertAssociazioneIdSdi(idFattura, idSdi, docnumber);
            else
                return fatt.UpdateAssociazioneIdSdi(idFattura, idSdi, docnumber);
        }

        public static void AddCampiCustom(FatturaElettronicaType fattura, string idFattura)
        {
            DocsPaDB.Query_DocsPAWS.Fatturazione fattDb = new DocsPaDB.Query_DocsPAWS.Fatturazione();
            List<CampoCustomFattura> list = fattDb.ImportFatture_GetCampiCustom(idFattura);

            if(list == null)
            {
                // ??
            }
            else
            {
                foreach(CampoCustomFattura item in list)
                {
                    //if(item.Tipo == TipoCampoCustomFatturaType.HEADER)
                    //{
                    //    AddCampoCustomHeader(fattura.FatturaElettronicaHeader, item.NomeCampo, item.Valore);
                    //}
                    //if(item.Tipo == TipoCampoCustomFatturaType.ITEM)
                    //{
                    //    AddCampoCustomBody(fattura.FatturaElettronicaBody[0], item.NomeCampo, item.Valore, item.NumeroLinea);
                    //}
                    if(item.NomeCampo.StartsWith("1"))
                    {
                        AddCampoCustomHeader(fattura.FatturaElettronicaHeader, item.NomeCampo, item.Valore);
                    }
                    else if(item.NomeCampo.StartsWith("2"))
                    {
                        AddCampoCustomBody(fattura.FatturaElettronicaBody[0], item.NomeCampo, item.Valore, item.NumeroLinea);
                    }
                }
            }
        }

        private static void AddCampoCustomHeader(FatturaElettronicaHeaderType header, string nomeCampo, string valoreCampo)
        {
            try
            {
                switch (nomeCampo)
                {
                    #region 1.1 DatiTrasmissione
                    case "1.1.6":
                        header.DatiTrasmissione.PECDestinatario = valoreCampo;
                        break;
                    #endregion
                    #region 1.2 CedentePrestatore
                    case "1.2.1.3.4":
                        header.CedentePrestatore.DatiAnagrafici.Anagrafica.Titolo = valoreCampo;
                        break;
                    case "1.2.1.3.5":
                        header.CedentePrestatore.DatiAnagrafici.Anagrafica.CodEORI = valoreCampo;
                        break;
                    case "1.2.1.4":
                        header.CedentePrestatore.DatiAnagrafici.AlboProfessionale = valoreCampo;
                        break;
                    case "1.2.1.5":
                        header.CedentePrestatore.DatiAnagrafici.ProvinciaAlbo = valoreCampo;
                        break;
                    case "1.2.1.6":
                        header.CedentePrestatore.DatiAnagrafici.NumeroIscrizioneAlbo = valoreCampo;
                        break;
                    case "1.2.1.7":
                        header.CedentePrestatore.DatiAnagrafici.DataIscrizioneAlbo = DateTime.Parse(valoreCampo);
                        break;
                    case "1.2.1.8":
                        //header.CedentePrestatore.DatiAnagrafici.RegimeFiscale = valoreCampo;
                        break;
                    case "1.2.3.1":
                        if (header.CedentePrestatore.StabileOrganizzazione == null)
                            header.CedentePrestatore.StabileOrganizzazione = new IndirizzoType();
                        header.CedentePrestatore.StabileOrganizzazione.Indirizzo = valoreCampo;
                        break;
                    case "1.2.3.2":
                        if (header.CedentePrestatore.StabileOrganizzazione == null)
                            header.CedentePrestatore.StabileOrganizzazione = new IndirizzoType();
                        header.CedentePrestatore.StabileOrganizzazione.NumeroCivico = valoreCampo;
                        break;
                    case "1.2.3.3":
                        if (header.CedentePrestatore.StabileOrganizzazione == null)
                            header.CedentePrestatore.StabileOrganizzazione = new IndirizzoType();
                        header.CedentePrestatore.StabileOrganizzazione.CAP = valoreCampo;
                        break;
                    case "1.2.3.4":
                        if (header.CedentePrestatore.StabileOrganizzazione == null)
                            header.CedentePrestatore.StabileOrganizzazione = new IndirizzoType();
                        header.CedentePrestatore.StabileOrganizzazione.Comune = valoreCampo;
                        break;
                    case "1.2.3.5":
                        if (header.CedentePrestatore.StabileOrganizzazione == null)
                            header.CedentePrestatore.StabileOrganizzazione = new IndirizzoType();
                        header.CedentePrestatore.StabileOrganizzazione.Provincia = valoreCampo;
                        break;
                    case "1.2.3.6":
                        if (header.CedentePrestatore.StabileOrganizzazione == null)
                            header.CedentePrestatore.StabileOrganizzazione = new IndirizzoType();
                        header.CedentePrestatore.StabileOrganizzazione.Nazione = valoreCampo;
                        break;
                    case "1.2.5.1":
                        if (header.CedentePrestatore.Contatti == null)
                            header.CedentePrestatore.Contatti = new ContattiType();
                        header.CedentePrestatore.Contatti.Telefono = valoreCampo;
                        break;
                    case "1.2.5.2":
                        if (header.CedentePrestatore.Contatti == null)
                            header.CedentePrestatore.Contatti = new ContattiType();
                        header.CedentePrestatore.Contatti.Fax = valoreCampo;
                        break;
                    case "1.2.5.3":
                        if (header.CedentePrestatore.Contatti == null)
                            header.CedentePrestatore.Contatti = new ContattiType();
                        header.CedentePrestatore.Contatti.Email = valoreCampo;
                        break;
                    case "1.2.6":
                        header.CedentePrestatore.RiferimentoAmministrazione = valoreCampo;
                        break;
                    #endregion
                    #region 1.3 RappresentanteFiscale
                    // DA FARE?
                    #endregion
                    #region 1.4 CessionarioCommittente
                    case "1.4.1.3.5":
                        header.CessionarioCommittente.DatiAnagrafici.Anagrafica.CodEORI = valoreCampo;
                        break;
                    case "1.4.3.1":
                        if (header.CessionarioCommittente.StabileOrganizzazione == null)
                            header.CessionarioCommittente.StabileOrganizzazione = new IndirizzoType();
                        header.CessionarioCommittente.StabileOrganizzazione.Indirizzo = valoreCampo;
                        break;
                    case "1.4.3.2":
                        if (header.CessionarioCommittente.StabileOrganizzazione == null)
                            header.CessionarioCommittente.StabileOrganizzazione = new IndirizzoType();
                        header.CessionarioCommittente.StabileOrganizzazione.NumeroCivico = valoreCampo;
                        break;
                    case "1.4.3.3":
                        if (header.CessionarioCommittente.StabileOrganizzazione == null)
                            header.CessionarioCommittente.StabileOrganizzazione = new IndirizzoType();
                        header.CessionarioCommittente.StabileOrganizzazione.CAP = valoreCampo;
                        break;
                    case "1.4.3.4":
                        if (header.CessionarioCommittente.StabileOrganizzazione == null)
                            header.CessionarioCommittente.StabileOrganizzazione = new IndirizzoType();
                        header.CessionarioCommittente.StabileOrganizzazione.Comune = valoreCampo;
                        break;
                    case "1.4.3.5":
                        if (header.CessionarioCommittente.StabileOrganizzazione == null)
                            header.CessionarioCommittente.StabileOrganizzazione = new IndirizzoType();
                        header.CessionarioCommittente.StabileOrganizzazione.Provincia = valoreCampo;
                        break;
                    case "1.4.3.6":
                        if (header.CessionarioCommittente.StabileOrganizzazione == null)
                            header.CessionarioCommittente.StabileOrganizzazione = new IndirizzoType();
                        header.CessionarioCommittente.StabileOrganizzazione.Nazione = valoreCampo;
                        break;
                    case "1.4.4.1.1":
                        if (header.CessionarioCommittente.RappresentanteFiscale == null)
                            header.CessionarioCommittente.RappresentanteFiscale = new RappresentanteFiscaleCessionarioType();
                        if (header.CessionarioCommittente.RappresentanteFiscale.IdFiscaleIVA == null)
                            header.CessionarioCommittente.RappresentanteFiscale.IdFiscaleIVA = new IdFiscaleType();
                        header.CessionarioCommittente.RappresentanteFiscale.IdFiscaleIVA.IdPaese = valoreCampo;
                        break;
                    case "1.4.4.1.2":
                        if (header.CessionarioCommittente.RappresentanteFiscale == null)
                            header.CessionarioCommittente.RappresentanteFiscale = new RappresentanteFiscaleCessionarioType();
                        if (header.CessionarioCommittente.RappresentanteFiscale.IdFiscaleIVA == null)
                            header.CessionarioCommittente.RappresentanteFiscale.IdFiscaleIVA = new IdFiscaleType();
                        header.CessionarioCommittente.RappresentanteFiscale.IdFiscaleIVA.IdCodice = valoreCampo;
                        break;
                    case "1.4.4.2":
                        if (header.CessionarioCommittente.RappresentanteFiscale == null)
                            header.CessionarioCommittente.RappresentanteFiscale = new RappresentanteFiscaleCessionarioType();
                        break;
                    #endregion
                    #region 1.5 TerzoIntermediarioOSoggettoEmittente
                    case "1.5.1.1.1":
                        if (header.TerzoIntermediarioOSoggettoEmittente == null)
                            header.TerzoIntermediarioOSoggettoEmittente = new TerzoIntermediarioSoggettoEmittenteType();
                        if (header.TerzoIntermediarioOSoggettoEmittente.DatiAnagrafici == null)
                            header.TerzoIntermediarioOSoggettoEmittente.DatiAnagrafici = new DatiAnagraficiTerzoIntermediarioType();
                        if (header.TerzoIntermediarioOSoggettoEmittente.DatiAnagrafici.IdFiscaleIVA == null)
                            header.TerzoIntermediarioOSoggettoEmittente.DatiAnagrafici.IdFiscaleIVA = new IdFiscaleType();
                        header.TerzoIntermediarioOSoggettoEmittente.DatiAnagrafici.IdFiscaleIVA.IdPaese = valoreCampo;
                        break;
                    case "1.5.1.1.2":
                        if (header.TerzoIntermediarioOSoggettoEmittente == null)
                            header.TerzoIntermediarioOSoggettoEmittente = new TerzoIntermediarioSoggettoEmittenteType();
                        if (header.TerzoIntermediarioOSoggettoEmittente.DatiAnagrafici == null)
                            header.TerzoIntermediarioOSoggettoEmittente.DatiAnagrafici = new DatiAnagraficiTerzoIntermediarioType();
                        if (header.TerzoIntermediarioOSoggettoEmittente.DatiAnagrafici.IdFiscaleIVA == null)
                            header.TerzoIntermediarioOSoggettoEmittente.DatiAnagrafici.IdFiscaleIVA = new IdFiscaleType();
                        header.TerzoIntermediarioOSoggettoEmittente.DatiAnagrafici.IdFiscaleIVA.IdCodice = valoreCampo;
                        break;
                    case "1.5.1.2":
                        if (header.TerzoIntermediarioOSoggettoEmittente == null)
                            header.TerzoIntermediarioOSoggettoEmittente = new TerzoIntermediarioSoggettoEmittenteType();
                        if (header.TerzoIntermediarioOSoggettoEmittente.DatiAnagrafici == null)
                            header.TerzoIntermediarioOSoggettoEmittente.DatiAnagrafici = new DatiAnagraficiTerzoIntermediarioType();
                        header.TerzoIntermediarioOSoggettoEmittente.DatiAnagrafici.CodiceFiscale = valoreCampo;
                        break;
                    case "1.5.1.3.5":
                        if (header.TerzoIntermediarioOSoggettoEmittente == null)
                            header.TerzoIntermediarioOSoggettoEmittente = new TerzoIntermediarioSoggettoEmittenteType();
                        if (header.TerzoIntermediarioOSoggettoEmittente.DatiAnagrafici == null)
                            header.TerzoIntermediarioOSoggettoEmittente.DatiAnagrafici = new DatiAnagraficiTerzoIntermediarioType();
                        if (header.TerzoIntermediarioOSoggettoEmittente.DatiAnagrafici.Anagrafica == null)
                            header.TerzoIntermediarioOSoggettoEmittente.DatiAnagrafici.Anagrafica = new AnagraficaType();
                        header.TerzoIntermediarioOSoggettoEmittente.DatiAnagrafici.Anagrafica.CodEORI = valoreCampo;
                        break;
                    #endregion
                    #region 1.6 SoggettoEmittente
                    case "1.6":
                        header.SoggettoEmittente = (SoggettoEmittenteType)Enum.Parse(typeof(SoggettoEmittenteType), valoreCampo);
                        break;
                        #endregion
                }
            }
            catch(Exception ex)
            {
                logger.ErrorFormat("Errore iserimento campo custom {0} - valore {1}", nomeCampo, valoreCampo);
                logger.Error("Eccezione: ", ex);
            }
        }

        private static void AddCampoCustomBody(FatturaElettronicaBodyType body, string nomeCampo, string valoreCampo, string numeroLinea)
        {
            DettaglioLineeType line;

            try
            {
                switch (nomeCampo)
                {
                    #region 2.1.1 DatiGeneraliDocumento
                    case "2.1.1.5.1":
                        if (body.DatiGenerali.DatiGeneraliDocumento.DatiRitenuta == null)
                            body.DatiGenerali.DatiGeneraliDocumento.DatiRitenuta = new DatiRitenutaType();
                        body.DatiGenerali.DatiGeneraliDocumento.DatiRitenuta.TipoRitenuta = (TipoRitenutaType)Enum.Parse(typeof(TipoRitenutaType), valoreCampo);
                        break;
                    case "2.1.1.5.2":
                        if (body.DatiGenerali.DatiGeneraliDocumento.DatiRitenuta == null)
                            body.DatiGenerali.DatiGeneraliDocumento.DatiRitenuta = new DatiRitenutaType();
                        body.DatiGenerali.DatiGeneraliDocumento.DatiRitenuta.ImportoRitenuta = decimal.Parse(valoreCampo);
                        break;
                    case "2.1.1.5.3":
                        if (body.DatiGenerali.DatiGeneraliDocumento.DatiRitenuta == null)
                            body.DatiGenerali.DatiGeneraliDocumento.DatiRitenuta = new DatiRitenutaType();
                        body.DatiGenerali.DatiGeneraliDocumento.DatiRitenuta.AliquotaRitenuta = decimal.Parse(valoreCampo);
                        break;
                    case "2.1.1.5.4":
                        if (body.DatiGenerali.DatiGeneraliDocumento.DatiRitenuta == null)
                            body.DatiGenerali.DatiGeneraliDocumento.DatiRitenuta = new DatiRitenutaType();
                        body.DatiGenerali.DatiGeneraliDocumento.DatiRitenuta.CausalePagamento = valoreCampo;
                        break;
                    case "2.1.1.6.1":
                        if (body.DatiGenerali.DatiGeneraliDocumento.DatiBollo == null)
                            body.DatiGenerali.DatiGeneraliDocumento.DatiBollo = new DatiBolloType();
                        body.DatiGenerali.DatiGeneraliDocumento.DatiBollo.BolloVirtuale = BolloVirtualeType.SI;                       
                        break;
                    case "2.1.1.6.2":
                        if (body.DatiGenerali.DatiGeneraliDocumento.DatiBollo == null)
                            body.DatiGenerali.DatiGeneraliDocumento.DatiBollo = new DatiBolloType();
                        decimal _importoBollo = 0.0m;
                        if (Decimal.TryParse(valoreCampo.Replace(".", ","), out _importoBollo))
                        {
                            body.DatiGenerali.DatiGeneraliDocumento.DatiBollo.ImportoBollo = _importoBollo;
                        }
                        break;
                    case "2.1.1.7.1":
                        if (body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale == null)
                        {
                            body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale = new DatiCassaPrevidenzialeType[1];
                            body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale[0] = new DatiCassaPrevidenzialeType();
                        }
                        body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale[0].TipoCassa = (TipoCassaType)Enum.Parse(typeof(TipoCassaType), valoreCampo.Trim());
                        break;
                    case "2.1.1.7.2":
                        if (body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale == null)
                        {
                            body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale = new DatiCassaPrevidenzialeType[1];
                            body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale[0] = new DatiCassaPrevidenzialeType();
                        }
                        body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale[0].AlCassa = Decimal.Parse(valoreCampo);
                        break;
                    case "2.1.1.7.3":
                        if (body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale == null)
                        {
                            body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale = new DatiCassaPrevidenzialeType[1];
                            body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale[0] = new DatiCassaPrevidenzialeType();
                        }
                        body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale[0].ImportoContributoCassa = Decimal.Parse(valoreCampo);
                        break;
                    case "2.1.1.7.4":
                        if (body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale == null)
                        {
                            body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale = new DatiCassaPrevidenzialeType[1];
                            body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale[0] = new DatiCassaPrevidenzialeType();
                        }
                        body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale[0].ImponibileCassa = Decimal.Parse(valoreCampo);
                        break;
                    case "2.1.1.7.5":
                        if (body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale == null)
                        {
                            body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale = new DatiCassaPrevidenzialeType[1];
                            body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale[0] = new DatiCassaPrevidenzialeType();
                        }
                        body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale[0].AliquotaIVA = Decimal.Parse(valoreCampo);
                        break;
                    case "2.1.1.7.6":
                        if (body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale == null)
                        {
                            body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale = new DatiCassaPrevidenzialeType[1];
                            body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale[0] = new DatiCassaPrevidenzialeType();
                        }
                        body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale[0].Ritenuta = RitenutaType.SI;
                        break;
                    case "2.1.1.7.7":
                        if (body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale == null)
                        {
                            body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale = new DatiCassaPrevidenzialeType[1];
                            body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale[0] = new DatiCassaPrevidenzialeType();
                        }
                        body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale[0].Natura = (NaturaType)Enum.Parse(typeof(NaturaType), valoreCampo.Trim());
                        break;
                    case "2.1.1.7.8":
                        if (body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale == null)
                        {
                            body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale = new DatiCassaPrevidenzialeType[1];
                            body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale[0] = new DatiCassaPrevidenzialeType();
                        }
                        body.DatiGenerali.DatiGeneraliDocumento.DatiCassaPrevidenziale[0].RiferimentoAmministrazione = valoreCampo;
                        break;
                    case "2.1.1.8.1":
                        if (body.DatiGenerali.DatiGeneraliDocumento.ScontoMaggiorazione == null)
                        {
                            body.DatiGenerali.DatiGeneraliDocumento.ScontoMaggiorazione = new ScontoMaggiorazioneType[1];
                            body.DatiGenerali.DatiGeneraliDocumento.ScontoMaggiorazione[0] = new ScontoMaggiorazioneType();
                        }
                        body.DatiGenerali.DatiGeneraliDocumento.ScontoMaggiorazione[0].Tipo = (TipoScontoMaggiorazioneType)Enum.Parse(typeof(TipoScontoMaggiorazioneType), valoreCampo.Trim());
                        break;
                    case "2.1.1.8.2":
                        if (body.DatiGenerali.DatiGeneraliDocumento.ScontoMaggiorazione == null)
                        {
                            body.DatiGenerali.DatiGeneraliDocumento.ScontoMaggiorazione = new ScontoMaggiorazioneType[1];
                            body.DatiGenerali.DatiGeneraliDocumento.ScontoMaggiorazione[0] = new ScontoMaggiorazioneType();
                        }
                        body.DatiGenerali.DatiGeneraliDocumento.ScontoMaggiorazione[0].Percentuale = Decimal.Parse(valoreCampo);
                        break;
                    case "2.1.1.8.3":
                        if (body.DatiGenerali.DatiGeneraliDocumento.ScontoMaggiorazione == null)
                        {
                            body.DatiGenerali.DatiGeneraliDocumento.ScontoMaggiorazione = new ScontoMaggiorazioneType[1];
                            body.DatiGenerali.DatiGeneraliDocumento.ScontoMaggiorazione[0] = new ScontoMaggiorazioneType();
                        }
                        body.DatiGenerali.DatiGeneraliDocumento.ScontoMaggiorazione[0].Importo = Decimal.Parse(valoreCampo);
                        break;
                    case "2.1.1.9":
                        body.DatiGenerali.DatiGeneraliDocumento.ImportoTotaleDocumento = Decimal.Parse(valoreCampo);
                        break;
                    case "2.1.1.10":
                        body.DatiGenerali.DatiGeneraliDocumento.Arrotondamento = Decimal.Parse(valoreCampo);
                        break;
                    case "2.1.1.11":
                        body.DatiGenerali.DatiGeneraliDocumento.Causale = new string[1] { valoreCampo };
                        break;
                    case "2.1.1.12":
                        body.DatiGenerali.DatiGeneraliDocumento.Art73 = (Art73Type)Enum.Parse(typeof(Art73Type), valoreCampo.Trim());
                        break;
                    #endregion
                    #region 2.1.2 DatiOrdineAcquisto
                    case "2.1.2.2":
                        if (body.DatiGenerali.DatiOrdineAcquisto.Count() == 1)
                        {
                            body.DatiGenerali.DatiOrdineAcquisto[0].IdDocumento = valoreCampo;
                        }
                        else
                        {
                            body.DatiGenerali.DatiOrdineAcquisto.Where(x => x.NumeroLineaSAP == numeroLinea.Trim()).First().IdDocumento = valoreCampo;
                        }
                        break;
                    case "2.1.2.3":
                        if (body.DatiGenerali.DatiOrdineAcquisto.Count() == 1)
                        {
                            body.DatiGenerali.DatiOrdineAcquisto[0].Data = DateTime.Parse(valoreCampo);
                            body.DatiGenerali.DatiOrdineAcquisto[0].DataSpecified = true;
                        }
                        else
                        {
                            body.DatiGenerali.DatiOrdineAcquisto.Where(x => x.NumeroLineaSAP == numeroLinea.Trim()).FirstOrDefault().Data = DateTime.Parse(valoreCampo);
                            body.DatiGenerali.DatiOrdineAcquisto.Where(x => x.NumeroLineaSAP == numeroLinea.Trim()).FirstOrDefault().DataSpecified = true;
                        }
                        break;
                    case "2.1.2.4":
                        if (body.DatiGenerali.DatiOrdineAcquisto.Count() == 1)
                        {
                            body.DatiGenerali.DatiOrdineAcquisto[0].NumItem = valoreCampo;
                        }
                        else
                        {
                            body.DatiGenerali.DatiOrdineAcquisto.Where(x => x.NumeroLineaSAP == numeroLinea.Trim()).FirstOrDefault().NumItem = valoreCampo;
                        }
                        break;
                    case "2.1.2.5":
                        if (body.DatiGenerali.DatiOrdineAcquisto.Count() == 1)
                        {
                            body.DatiGenerali.DatiOrdineAcquisto[0].CodiceCommessaConvenzione = valoreCampo;
                        }
                        else
                        {
                            body.DatiGenerali.DatiOrdineAcquisto.Where(x => x.NumeroLineaSAP == numeroLinea.Trim()).FirstOrDefault().CodiceCommessaConvenzione = valoreCampo;
                        }
                        break;
                    #endregion
                    // I blocchi 2.1.3-2.1.6 e 2.1.8 sono gestiti al momento come singole linee (non previsto quindi il valore 2.1.x.1 RiferimentoNumeroLinea
                    #region 2.1.3 DatiContratto
                    case "2.1.3.3":
                        if (body.DatiGenerali.DatiContratto == null)
                        {
                            body.DatiGenerali.DatiContratto = new DatiDocumentiCorrelatiType[1];
                            body.DatiGenerali.DatiContratto[0] = new DatiDocumentiCorrelatiType();
                        }
                        body.DatiGenerali.DatiContratto[0].Data = DateTime.Parse(valoreCampo);
                        body.DatiGenerali.DatiContratto[0].DataSpecified = true;
                        break;
                    case "2.1.3.4":
                        if (body.DatiGenerali.DatiContratto == null)
                        {
                            body.DatiGenerali.DatiContratto = new DatiDocumentiCorrelatiType[1];
                            body.DatiGenerali.DatiContratto[0] = new DatiDocumentiCorrelatiType();
                        }
                        body.DatiGenerali.DatiContratto[0].NumItem = valoreCampo;
                        break;
                    case "2.1.3.5":
                        if(body.DatiGenerali.DatiContratto == null)
                        {
                            body.DatiGenerali.DatiContratto = new DatiDocumentiCorrelatiType[1];
                            body.DatiGenerali.DatiContratto[0] = new DatiDocumentiCorrelatiType();
                        }
                        body.DatiGenerali.DatiContratto[0].CodiceCommessaConvenzione = valoreCampo;                        
                        break;
                    #endregion
                    #region 2.1.4 DatiConvenzione
                    case "2.1.4.2":
                        if (body.DatiGenerali.DatiConvenzione == null)
                        {
                            body.DatiGenerali.DatiConvenzione = new DatiDocumentiCorrelatiType[1];
                            body.DatiGenerali.DatiConvenzione[0] = new DatiDocumentiCorrelatiType();
                        }
                        body.DatiGenerali.DatiConvenzione[0].IdDocumento = valoreCampo;
                        break;
                    case "2.1.4.3":
                        if (body.DatiGenerali.DatiConvenzione == null)
                        {
                            body.DatiGenerali.DatiConvenzione = new DatiDocumentiCorrelatiType[1];
                            body.DatiGenerali.DatiConvenzione[0] = new DatiDocumentiCorrelatiType();
                        }
                        body.DatiGenerali.DatiConvenzione[0].Data = DateTime.Parse(valoreCampo);
                        body.DatiGenerali.DatiConvenzione[0].DataSpecified = true;
                        break;
                    case "2.1.4.4":
                        if (body.DatiGenerali.DatiConvenzione == null)
                        {
                            body.DatiGenerali.DatiConvenzione = new DatiDocumentiCorrelatiType[1];
                            body.DatiGenerali.DatiConvenzione[0] = new DatiDocumentiCorrelatiType();
                        }
                        body.DatiGenerali.DatiConvenzione[0].NumItem = valoreCampo;
                        break;
                    case "2.1.4.5":
                        if (body.DatiGenerali.DatiConvenzione == null)
                        {
                            body.DatiGenerali.DatiConvenzione = new DatiDocumentiCorrelatiType[1];
                            body.DatiGenerali.DatiConvenzione[0] = new DatiDocumentiCorrelatiType();
                        }
                        body.DatiGenerali.DatiConvenzione[0].CodiceCommessaConvenzione = valoreCampo;
                        break;
                    case "2.1.4.6":
                        if (body.DatiGenerali.DatiConvenzione == null)
                        {
                            body.DatiGenerali.DatiConvenzione = new DatiDocumentiCorrelatiType[1];
                            body.DatiGenerali.DatiConvenzione[0] = new DatiDocumentiCorrelatiType();
                        }
                        body.DatiGenerali.DatiConvenzione[0].CodiceCUP = valoreCampo;
                        break;
                    case "2.1.4.7":
                        if (body.DatiGenerali.DatiConvenzione == null)
                        {
                            body.DatiGenerali.DatiConvenzione = new DatiDocumentiCorrelatiType[1];
                            body.DatiGenerali.DatiConvenzione[0] = new DatiDocumentiCorrelatiType();
                        }
                        body.DatiGenerali.DatiConvenzione[0].CodiceCIG = valoreCampo;
                        break;
                    #endregion
                    #region 2.1.5 DatiRicezione
                    case "2.1.5.1":
                        if (body.DatiGenerali.DatiRicezione == null)
                        {
                            body.DatiGenerali.DatiRicezione = new DatiDocumentiCorrelatiType[1];
                            body.DatiGenerali.DatiRicezione[0] = new DatiDocumentiCorrelatiType();
                        }
                        body.DatiGenerali.DatiRicezione[0].RiferimentoNumeroLinea = valoreCampo;
                        break;
                    case "2.1.5.2":
                        if (body.DatiGenerali.DatiRicezione == null)
                        {
                            body.DatiGenerali.DatiRicezione = new DatiDocumentiCorrelatiType[1];
                            body.DatiGenerali.DatiRicezione[0] = new DatiDocumentiCorrelatiType();
                        }
                        body.DatiGenerali.DatiRicezione[0].IdDocumento = valoreCampo;
                        break;
                    case "2.1.5.3":
                        if (body.DatiGenerali.DatiRicezione == null)
                        {
                            body.DatiGenerali.DatiRicezione = new DatiDocumentiCorrelatiType[1];
                            body.DatiGenerali.DatiRicezione[0] = new DatiDocumentiCorrelatiType();
                        }
                        body.DatiGenerali.DatiRicezione[0].Data = DateTime.Parse(valoreCampo);
                        body.DatiGenerali.DatiRicezione[0].DataSpecified = true;
                        break;
                    case "2.1.5.4":
                        if (body.DatiGenerali.DatiRicezione == null)
                        {
                            body.DatiGenerali.DatiRicezione = new DatiDocumentiCorrelatiType[1];
                            body.DatiGenerali.DatiRicezione[0] = new DatiDocumentiCorrelatiType();
                        }
                        body.DatiGenerali.DatiRicezione[0].NumItem = valoreCampo;
                        break;
                    case "2.1.5.5":
                        if (body.DatiGenerali.DatiRicezione == null)
                        {
                            body.DatiGenerali.DatiRicezione = new DatiDocumentiCorrelatiType[1];
                            body.DatiGenerali.DatiRicezione[0] = new DatiDocumentiCorrelatiType();
                        }
                        body.DatiGenerali.DatiRicezione[0].CodiceCommessaConvenzione = valoreCampo;
                        break;
                    case "2.1.5.6":
                        if (body.DatiGenerali.DatiRicezione == null)
                        {
                            body.DatiGenerali.DatiRicezione = new DatiDocumentiCorrelatiType[1];
                            body.DatiGenerali.DatiRicezione[0] = new DatiDocumentiCorrelatiType();
                        }
                        body.DatiGenerali.DatiRicezione[0].CodiceCUP = valoreCampo;
                        break;
                    case "2.1.5.7":
                        if (body.DatiGenerali.DatiRicezione == null)
                        {
                            body.DatiGenerali.DatiRicezione = new DatiDocumentiCorrelatiType[1];
                            body.DatiGenerali.DatiRicezione[0] = new DatiDocumentiCorrelatiType();
                        }
                        body.DatiGenerali.DatiRicezione[0].CodiceCIG = valoreCampo;
                        break;
                    #endregion
                    #region 2.1.6 DatiFattureCollegate
                    case "2.1.6.1":
                        if (body.DatiGenerali.DatiFattureCollegate == null)
                        {
                            body.DatiGenerali.DatiFattureCollegate = new DatiDocumentiCorrelatiType[1];
                            body.DatiGenerali.DatiFattureCollegate[0] = new DatiDocumentiCorrelatiType();
                        }
                        body.DatiGenerali.DatiFattureCollegate[0].RiferimentoNumeroLinea = valoreCampo;
                        break;
                    case "2.1.6.2":
                        if (body.DatiGenerali.DatiFattureCollegate == null)
                        {
                            body.DatiGenerali.DatiFattureCollegate = new DatiDocumentiCorrelatiType[1];
                            body.DatiGenerali.DatiFattureCollegate[0] = new DatiDocumentiCorrelatiType();
                        }
                        body.DatiGenerali.DatiFattureCollegate[0].IdDocumento = valoreCampo;
                        break;
                    case "2.1.6.3":
                        if (body.DatiGenerali.DatiFattureCollegate == null)
                        {
                            body.DatiGenerali.DatiFattureCollegate = new DatiDocumentiCorrelatiType[1];
                            body.DatiGenerali.DatiFattureCollegate[0] = new DatiDocumentiCorrelatiType();
                        }
                        body.DatiGenerali.DatiFattureCollegate[0].Data = DateTime.Parse(valoreCampo);
                        body.DatiGenerali.DatiFattureCollegate[0].DataSpecified = true;
                        break;
                    case "2.1.6.4":
                        if (body.DatiGenerali.DatiFattureCollegate == null)
                        {
                            body.DatiGenerali.DatiFattureCollegate = new DatiDocumentiCorrelatiType[1];
                            body.DatiGenerali.DatiFattureCollegate[0] = new DatiDocumentiCorrelatiType();
                        }
                        body.DatiGenerali.DatiFattureCollegate[0].NumItem = valoreCampo;
                        break;
                    case "2.1.6.5":
                        if (body.DatiGenerali.DatiFattureCollegate == null)
                        {
                            body.DatiGenerali.DatiFattureCollegate = new DatiDocumentiCorrelatiType[1];
                            body.DatiGenerali.DatiFattureCollegate[0] = new DatiDocumentiCorrelatiType();
                        }
                        body.DatiGenerali.DatiFattureCollegate[0].CodiceCommessaConvenzione = valoreCampo;
                        break;
                    case "2.1.6.6":
                        if (body.DatiGenerali.DatiFattureCollegate == null)
                        {
                            body.DatiGenerali.DatiFattureCollegate = new DatiDocumentiCorrelatiType[1];
                            body.DatiGenerali.DatiFattureCollegate[0] = new DatiDocumentiCorrelatiType();
                        }
                        body.DatiGenerali.DatiFattureCollegate[0].CodiceCUP = valoreCampo;
                        break;
                    case "2.1.6.7":
                        if (body.DatiGenerali.DatiFattureCollegate == null)
                        {
                            body.DatiGenerali.DatiFattureCollegate = new DatiDocumentiCorrelatiType[1];
                            body.DatiGenerali.DatiFattureCollegate[0] = new DatiDocumentiCorrelatiType();
                        }
                        body.DatiGenerali.DatiFattureCollegate[0].CodiceCIG = valoreCampo;
                        break;
                    #endregion
                    #region 2.1.7 DatiSAL
                    case "2.1.7.1":
                        if (body.DatiGenerali.DatiSAL == null)
                        {
                            body.DatiGenerali.DatiSAL = new DatiSALType[1];
                            body.DatiGenerali.DatiSAL[0] = new DatiSALType();
                        }
                        body.DatiGenerali.DatiSAL[0].RiferimentoFase = valoreCampo;
                        break;
                    #endregion
                    #region 2.1.8 DatiDDT
                    case "2.1.8.1":
                        if (body.DatiGenerali.DatiDDT == null)
                        {
                            body.DatiGenerali.DatiDDT = new DatiDDTType[1];
                            body.DatiGenerali.DatiDDT[0] = new DatiDDTType();
                        }
                        body.DatiGenerali.DatiDDT[0].NumeroDDT = valoreCampo;
                        break;
                    case "2.1.8.2":
                        if (body.DatiGenerali.DatiDDT == null)
                        {
                            body.DatiGenerali.DatiDDT = new DatiDDTType[1];
                            body.DatiGenerali.DatiDDT[0] = new DatiDDTType();
                        }
                        body.DatiGenerali.DatiDDT[0].DataDDT = DateTime.Parse(valoreCampo);
                        break;
                    #endregion
                    #region 2.1.9 DatiTrasporto
                    case "2.1.9.1.1.1":
                        if (body.DatiGenerali.DatiTrasporto == null)
                            body.DatiGenerali.DatiTrasporto = new DatiTrasportoType();
                        if (body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore == null)
                            body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore = new DatiAnagraficiVettoreType();
                        if (body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore.IdFiscaleIVA == null)
                            body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore.IdFiscaleIVA = new IdFiscaleType();
                        body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore.IdFiscaleIVA.IdPaese = valoreCampo;
                        break;
                    case "2.1.9.1.1.2":
                        if (body.DatiGenerali.DatiTrasporto == null)
                            body.DatiGenerali.DatiTrasporto = new DatiTrasportoType();
                        if (body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore == null)
                            body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore = new DatiAnagraficiVettoreType();
                        if (body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore.IdFiscaleIVA == null)
                            body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore.IdFiscaleIVA = new IdFiscaleType();
                        body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore.IdFiscaleIVA.IdCodice = valoreCampo;
                        break;
                    case "2.1.9.1.2":
                        if (body.DatiGenerali.DatiTrasporto == null)
                            body.DatiGenerali.DatiTrasporto = new DatiTrasportoType();
                        if (body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore == null)
                            body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore = new DatiAnagraficiVettoreType();
                        body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore.CodiceFiscale = valoreCampo;
                        break;
                    case "2.1.9.1.3.1":
                        if (body.DatiGenerali.DatiTrasporto == null)
                            body.DatiGenerali.DatiTrasporto = new DatiTrasportoType();
                        if (body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore == null)
                            body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore = new DatiAnagraficiVettoreType();
                        if (body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore.Anagrafica == null)
                            body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore.Anagrafica = new AnagraficaType();
                        body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore.Anagrafica.ItemsElementName = new ItemsChoiceType[] { ItemsChoiceType.Denominazione };
                        body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore.Anagrafica.Items = new string[] { valoreCampo };
                        break;
                    case "2.1.9.1.3.2":
                        if (body.DatiGenerali.DatiTrasporto == null)
                            body.DatiGenerali.DatiTrasporto = new DatiTrasportoType();
                        if (body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore == null)
                            body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore = new DatiAnagraficiVettoreType();
                        if (body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore.Anagrafica == null)
                            body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore.Anagrafica = new AnagraficaType();
                        body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore.Anagrafica.ItemsElementName = new ItemsChoiceType[] { ItemsChoiceType.Nome };
                        body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore.Anagrafica.Items = new string[] { valoreCampo };
                        break;
                    case "2.1.9.1.3.3":
                        if (body.DatiGenerali.DatiTrasporto == null)
                            body.DatiGenerali.DatiTrasporto = new DatiTrasportoType();
                        if (body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore == null)
                            body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore = new DatiAnagraficiVettoreType();
                        if (body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore.Anagrafica == null)
                            body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore.Anagrafica = new AnagraficaType();
                        body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore.Anagrafica.ItemsElementName = new ItemsChoiceType[] { ItemsChoiceType.Cognome };
                        body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore.Anagrafica.Items = new string[] { valoreCampo };
                        break;
                    case "2.1.9.1.3.4":
                        if (body.DatiGenerali.DatiTrasporto == null)
                            body.DatiGenerali.DatiTrasporto = new DatiTrasportoType();
                        if (body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore == null)
                            body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore = new DatiAnagraficiVettoreType();
                        if (body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore.Anagrafica == null)
                            body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore.Anagrafica = new AnagraficaType();
                        body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore.Anagrafica.Titolo = valoreCampo;
                        break;
                    case "2.1.9.1.3.5":
                        if (body.DatiGenerali.DatiTrasporto == null)
                            body.DatiGenerali.DatiTrasporto = new DatiTrasportoType();
                        if (body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore == null)
                            body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore = new DatiAnagraficiVettoreType();
                        if (body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore.Anagrafica == null)
                            body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore.Anagrafica = new AnagraficaType();
                        body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore.Anagrafica.CodEORI = valoreCampo;
                        break;
                    case "2.1.9.1.4":
                        if (body.DatiGenerali.DatiTrasporto == null)
                            body.DatiGenerali.DatiTrasporto = new DatiTrasportoType();
                        if (body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore == null)
                            body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore = new DatiAnagraficiVettoreType();
                        if (body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore.Anagrafica == null)
                            body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore.Anagrafica = new AnagraficaType();
                        body.DatiGenerali.DatiTrasporto.DatiAnagraficiVettore.NumeroLicenzaGuida = valoreCampo;
                        break;
                    case "2.1.9.2":
                        if (body.DatiGenerali.DatiTrasporto == null)
                            body.DatiGenerali.DatiTrasporto = new DatiTrasportoType();
                        body.DatiGenerali.DatiTrasporto.CausaleTrasporto = valoreCampo;
                        break;
                    case "2.1.9.3":
                        if (body.DatiGenerali.DatiTrasporto == null)
                            body.DatiGenerali.DatiTrasporto = new DatiTrasportoType();
                        body.DatiGenerali.DatiTrasporto.MezzoTrasporto = valoreCampo;
                        break;
                    case "2.1.9.4":
                        if (body.DatiGenerali.DatiTrasporto == null)
                            body.DatiGenerali.DatiTrasporto = new DatiTrasportoType();
                        body.DatiGenerali.DatiTrasporto.NumeroColli = valoreCampo;
                        break;
                    case "2.1.9.5":
                        if (body.DatiGenerali.DatiTrasporto == null)
                            body.DatiGenerali.DatiTrasporto = new DatiTrasportoType();
                        body.DatiGenerali.DatiTrasporto.Descrizione = valoreCampo;
                        break;
                    case "2.1.9.6":
                        if (body.DatiGenerali.DatiTrasporto == null)
                            body.DatiGenerali.DatiTrasporto = new DatiTrasportoType();
                        body.DatiGenerali.DatiTrasporto.UnitaMisuraPeso = valoreCampo;
                        break;
                    case "2.1.9.7":
                        if (body.DatiGenerali.DatiTrasporto == null)
                            body.DatiGenerali.DatiTrasporto = new DatiTrasportoType();
                        body.DatiGenerali.DatiTrasporto.PesoLordo = Decimal.Parse(valoreCampo);
                        break;
                    case "2.1.9.8":
                        if (body.DatiGenerali.DatiTrasporto == null)
                            body.DatiGenerali.DatiTrasporto = new DatiTrasportoType();
                        body.DatiGenerali.DatiTrasporto.PesoNetto = Decimal.Parse(valoreCampo);
                        break;
                    case "2.1.9.9":
                        if (body.DatiGenerali.DatiTrasporto == null)
                            body.DatiGenerali.DatiTrasporto = new DatiTrasportoType();
                        body.DatiGenerali.DatiTrasporto.DataOraRitiro = DateTime.Parse(valoreCampo);
                        break;
                    case "2.1.9.10":
                        if (body.DatiGenerali.DatiTrasporto == null)
                            body.DatiGenerali.DatiTrasporto = new DatiTrasportoType();
                        body.DatiGenerali.DatiTrasporto.DataInizioTrasporto = DateTime.Parse(valoreCampo);
                        break;
                    case "2.1.9.11":
                        if (body.DatiGenerali.DatiTrasporto == null)
                            body.DatiGenerali.DatiTrasporto = new DatiTrasportoType();
                        body.DatiGenerali.DatiTrasporto.TipoResa = valoreCampo;
                        break;
                    case "2.1.9.12.1":
                        if (body.DatiGenerali.DatiTrasporto == null)
                            body.DatiGenerali.DatiTrasporto = new DatiTrasportoType();
                        if (body.DatiGenerali.DatiTrasporto.IndirizzoResa == null)
                            body.DatiGenerali.DatiTrasporto.IndirizzoResa = new IndirizzoType();
                        body.DatiGenerali.DatiTrasporto.IndirizzoResa.Indirizzo = valoreCampo;
                        break;
                    case "2.1.9.12.2":
                        if (body.DatiGenerali.DatiTrasporto == null)
                            body.DatiGenerali.DatiTrasporto = new DatiTrasportoType();
                        if (body.DatiGenerali.DatiTrasporto.IndirizzoResa == null)
                            body.DatiGenerali.DatiTrasporto.IndirizzoResa = new IndirizzoType();
                        body.DatiGenerali.DatiTrasporto.IndirizzoResa.NumeroCivico = valoreCampo;
                        break;
                    case "2.1.9.12.3":
                        if (body.DatiGenerali.DatiTrasporto == null)
                            body.DatiGenerali.DatiTrasporto = new DatiTrasportoType();
                        if (body.DatiGenerali.DatiTrasporto.IndirizzoResa == null)
                            body.DatiGenerali.DatiTrasporto.IndirizzoResa = new IndirizzoType();
                        body.DatiGenerali.DatiTrasporto.IndirizzoResa.CAP = valoreCampo;
                        break;
                    case "2.1.9.12.4":
                        if (body.DatiGenerali.DatiTrasporto == null)
                            body.DatiGenerali.DatiTrasporto = new DatiTrasportoType();
                        if (body.DatiGenerali.DatiTrasporto.IndirizzoResa == null)
                            body.DatiGenerali.DatiTrasporto.IndirizzoResa = new IndirizzoType();
                        body.DatiGenerali.DatiTrasporto.IndirizzoResa.Comune = valoreCampo;
                        break;
                    case "2.1.9.12.5":
                        if (body.DatiGenerali.DatiTrasporto == null)
                            body.DatiGenerali.DatiTrasporto = new DatiTrasportoType();
                        if (body.DatiGenerali.DatiTrasporto.IndirizzoResa == null)
                            body.DatiGenerali.DatiTrasporto.IndirizzoResa = new IndirizzoType();
                        body.DatiGenerali.DatiTrasporto.IndirizzoResa.Provincia = valoreCampo;
                        break;
                    case "2.1.9.12.6":
                        if (body.DatiGenerali.DatiTrasporto == null)
                            body.DatiGenerali.DatiTrasporto = new DatiTrasportoType();
                        if (body.DatiGenerali.DatiTrasporto.IndirizzoResa == null)
                            body.DatiGenerali.DatiTrasporto.IndirizzoResa = new IndirizzoType();
                        body.DatiGenerali.DatiTrasporto.IndirizzoResa.Nazione = valoreCampo;
                        break;
                    case "2.1.9.13":
                        if (body.DatiGenerali.DatiTrasporto == null)
                            body.DatiGenerali.DatiTrasporto = new DatiTrasportoType();
                        body.DatiGenerali.DatiTrasporto.DataOraConsegna = DateTime.Parse(valoreCampo);
                        break;
                    #endregion
                    #region 2.1.10 FatturaPrincipale
                    case "2.1.10.1":
                        if (body.DatiGenerali.FatturaPrincipale == null)
                            body.DatiGenerali.FatturaPrincipale = new FatturaPrincipaleType();
                        body.DatiGenerali.FatturaPrincipale.NumeroFatturaPrincipale = valoreCampo;
                        break;
                    case "2.1.10.2":
                        if (body.DatiGenerali.FatturaPrincipale == null)
                            body.DatiGenerali.FatturaPrincipale = new FatturaPrincipaleType();
                        body.DatiGenerali.FatturaPrincipale.DataFatturaPrincipale = DateTime.Parse(valoreCampo);
                        break;
                    #endregion

                    #region 2.2.1 DettaglioLinee
                    case "2.2.1.2":
                        body.DatiBeniServizi.DettaglioLinee.Where(x => x.NumeroLineaSAP == numeroLinea).FirstOrDefault().TipoCessionePrestazione = (TipoCessionePrestazioneType)Enum.Parse(typeof(TipoCessionePrestazioneType), valoreCampo);
                        body.DatiBeniServizi.DettaglioLinee.Where(x => x.NumeroLineaSAP == numeroLinea).FirstOrDefault().TipoCessionePrestazioneSpecified = true;
                        break;
                    case "2.2.1.7":
                        if (valoreCampo.Contains('.') && valoreCampo.Split('.').Count() == 3 && valoreCampo.Split('.')[2].Length == 4)
                        {
                            // Data nel formato dd.mm.yyyy
                            body.DatiBeniServizi.DettaglioLinee.Where(x => x.NumeroLineaSAP == numeroLinea).FirstOrDefault().DataInizioPeriodo = new DateTime(Int32.Parse(valoreCampo.Split('.')[2]), Int32.Parse(valoreCampo.Split('.')[1]), Int32.Parse(valoreCampo.Split('.')[0]));
                        }
                        else
                        {
                            body.DatiBeniServizi.DettaglioLinee.Where(x => x.NumeroLineaSAP == numeroLinea).FirstOrDefault().DataInizioPeriodo = DateTime.Parse(valoreCampo);
                        }
                        body.DatiBeniServizi.DettaglioLinee.Where(x => x.NumeroLineaSAP == numeroLinea).FirstOrDefault().DataInizioPeriodoSpecified = true;
                        break;
                    case "2.2.1.8":
                        if (valoreCampo.Contains('.') && valoreCampo.Split('.').Count() == 3 && valoreCampo.Split('.')[2].Length == 4)
                        {
                            // Data nel formato dd.mm.yyyy
                            body.DatiBeniServizi.DettaglioLinee.Where(x => x.NumeroLineaSAP == numeroLinea).FirstOrDefault().DataFinePeriodo = new DateTime(Int32.Parse(valoreCampo.Split('.')[2]), Int32.Parse(valoreCampo.Split('.')[1]), Int32.Parse(valoreCampo.Split('.')[0]));
                        }
                        else
                        {
                            body.DatiBeniServizi.DettaglioLinee.Where(x => x.NumeroLineaSAP == numeroLinea).FirstOrDefault().DataFinePeriodo = DateTime.Parse(valoreCampo);
                        }
                        body.DatiBeniServizi.DettaglioLinee.Where(x => x.NumeroLineaSAP == numeroLinea).FirstOrDefault().DataFinePeriodoSpecified = true;
                        break;
                    case "2.2.1.3.1":
                        line = body.DatiBeniServizi.DettaglioLinee.Where(x => x.NumeroLineaSAP == numeroLinea).FirstOrDefault();
                        if (line.CodiceArticolo == null)
                        {
                            line.CodiceArticolo = new CodiceArticoloType[1];
                            line.CodiceArticolo[0] = new CodiceArticoloType();
                        }
                        line.CodiceArticolo[0].CodiceTipo = valoreCampo;
                        break;
                    case "2.2.1.3.2":
                        line = body.DatiBeniServizi.DettaglioLinee.Where(x => x.NumeroLineaSAP == numeroLinea).FirstOrDefault();
                        if (line.CodiceArticolo == null)
                        {
                            line.CodiceArticolo = new CodiceArticoloType[1];
                            line.CodiceArticolo[0] = new CodiceArticoloType();
                        }
                        line.CodiceArticolo[0].CodiceValore = valoreCampo;
                        break;
                    case "2.2.1.10.1":
                        line = body.DatiBeniServizi.DettaglioLinee.Where(x => x.NumeroLineaSAP == numeroLinea).FirstOrDefault();
                        if (line.ScontoMaggiorazione == null)
                        {
                            line.ScontoMaggiorazione = new ScontoMaggiorazioneType[1];
                            line.ScontoMaggiorazione[0] = new ScontoMaggiorazioneType();
                        }
                        line.ScontoMaggiorazione[0].Tipo = (TipoScontoMaggiorazioneType)Enum.Parse(typeof(TipoScontoMaggiorazioneType), valoreCampo);
                        break;
                    case "2.2.1.10.2":
                        line = body.DatiBeniServizi.DettaglioLinee.Where(x => x.NumeroLineaSAP == numeroLinea).FirstOrDefault();
                        if (line.ScontoMaggiorazione == null)
                        {
                            line.ScontoMaggiorazione = new ScontoMaggiorazioneType[1];
                            line.ScontoMaggiorazione[0] = new ScontoMaggiorazioneType();
                        }
                        line.ScontoMaggiorazione[0].Percentuale = Decimal.Parse(valoreCampo);
                        break;
                    case "2.2.1.10.3":
                        line = body.DatiBeniServizi.DettaglioLinee.Where(x => x.NumeroLineaSAP == numeroLinea).FirstOrDefault();
                        if (line.ScontoMaggiorazione == null)
                        {
                            line.ScontoMaggiorazione = new ScontoMaggiorazioneType[1];
                            line.ScontoMaggiorazione[0] = new ScontoMaggiorazioneType();
                        }
                        line.ScontoMaggiorazione[0].Importo = Decimal.Parse(valoreCampo);
                        break;
                    case "2.2.1.13":
                        body.DatiBeniServizi.DettaglioLinee.Where(x => x.NumeroLineaSAP == numeroLinea).FirstOrDefault().Ritenuta = (RitenutaType)Enum.Parse(typeof(RitenutaType), valoreCampo);
                        body.DatiBeniServizi.DettaglioLinee.Where(x => x.NumeroLineaSAP == numeroLinea).FirstOrDefault().RitenutaSpecified = true;
                        break;
                    case "2.2.1.14":
                        body.DatiBeniServizi.DettaglioLinee.Where(x => x.NumeroLineaSAP == numeroLinea).FirstOrDefault().Natura = (NaturaType)Enum.Parse(typeof(NaturaType), valoreCampo);
                        body.DatiBeniServizi.DettaglioLinee.Where(x => x.NumeroLineaSAP == numeroLinea).FirstOrDefault().NaturaSpecified = true;
                        break;
                    case "2.2.1.15":
                        body.DatiBeniServizi.DettaglioLinee.Where(x => x.NumeroLineaSAP == numeroLinea).FirstOrDefault().RiferimentoAmministrazione = valoreCampo;
                        break;
                    case "2.2.1.16.1":
                        line = body.DatiBeniServizi.DettaglioLinee.Where(x => x.NumeroLineaSAP == numeroLinea).FirstOrDefault();
                        if (line.AltriDatiGestionali == null)
                        {
                            line.AltriDatiGestionali = new AltriDatiGestionaliType[1];
                            line.AltriDatiGestionali[0] = new AltriDatiGestionaliType();
                            line.AltriDatiGestionali[0].TipoDato = valoreCampo;
                        }
                        else
                        {
                            List<AltriDatiGestionaliType> list = new List<AltriDatiGestionaliType>(line.AltriDatiGestionali);
                            list.Add(new AltriDatiGestionaliType() { TipoDato = valoreCampo });
                            line.AltriDatiGestionali = list.ToArray();
                        }
                        //line.AltriDatiGestionali[0].TipoDato = valoreCampo;
                        break;
                    case "2.2.1.16.2":
                        line = body.DatiBeniServizi.DettaglioLinee.Where(x => x.NumeroLineaSAP == numeroLinea).FirstOrDefault();
                        if (line.AltriDatiGestionali == null)
                        {
                            line.AltriDatiGestionali = new AltriDatiGestionaliType[1];
                            line.AltriDatiGestionali[0] = new AltriDatiGestionaliType();
                        }
                        line.AltriDatiGestionali[0].RiferimentoTesto = valoreCampo;
                        break;
                    case "2.2.1.16.3":
                        line = body.DatiBeniServizi.DettaglioLinee.Where(x => x.NumeroLineaSAP == numeroLinea).FirstOrDefault();
                        if (line.AltriDatiGestionali == null)
                        {
                            line.AltriDatiGestionali = new AltriDatiGestionaliType[1];
                            line.AltriDatiGestionali[0] = new AltriDatiGestionaliType();
                        }
                        line.AltriDatiGestionali[0].RiferimentoNumero = valoreCampo;
                        line.AltriDatiGestionali[0].RiferimentoNumeroSpecified = true;
                        break;
                    case "2.2.1.16.4":
                        line = body.DatiBeniServizi.DettaglioLinee.Where(x => x.NumeroLineaSAP == numeroLinea).FirstOrDefault();
                        if (line.AltriDatiGestionali == null)
                        {
                            line.AltriDatiGestionali = new AltriDatiGestionaliType[1];
                            line.AltriDatiGestionali[0] = new AltriDatiGestionaliType();
                        }
                        line.AltriDatiGestionali[0].RiferimentoData = DateTime.Parse(valoreCampo);
                        line.AltriDatiGestionali[0].RiferimentoDataSpecified = true;
                        break;
                    #endregion
                    #region 2.2.2 DatiRiepilogo
                    case "2.2.2.2":
                        body.DatiBeniServizi.DatiRiepilogo[0].Natura = (NaturaType)Enum.Parse(typeof(NaturaType), valoreCampo);
                        body.DatiBeniServizi.DatiRiepilogo[0].NaturaSpecified = true;
                        break;
                    case "2.2.2.3":
                        body.DatiBeniServizi.DatiRiepilogo[0].SpeseAccessorie = Decimal.Parse(valoreCampo);
                        body.DatiBeniServizi.DatiRiepilogo[0].SpeseAccessorieSpecified = true;
                        break;
                    case "2.2.2.4":
                        body.DatiBeniServizi.DatiRiepilogo[0].Arrotondamento = Decimal.Parse(valoreCampo);
                        body.DatiBeniServizi.DatiRiepilogo[0].ArrotondamentoSpecified = true;
                        break;
                    case "2.2.2.8":
                        body.DatiBeniServizi.DatiRiepilogo[0].RiferimentoNormativo = valoreCampo;
                        break;
                    #endregion

                    #region 2.3 DatiVeicoli
                    case "2.3.1":
                        if (body.DatiVeicoli == null)
                            body.DatiVeicoli = new DatiVeicoliType();
                        body.DatiVeicoli.Data = DateTime.Parse(valoreCampo);
                        break;
                    case "2.3.2":
                        if (body.DatiVeicoli == null)
                            body.DatiVeicoli = new DatiVeicoliType();
                        body.DatiVeicoli.TotalePercorso = valoreCampo;
                        break;
                    #endregion

                    #region 2.4 DatiPagamento
                    case "2.4.2.1":
                        body.DatiPagamento[0].DettaglioPagamento[0].Beneficiario = valoreCampo;                        
                        break;
                    case "2.4.2.5":
                        body.DatiPagamento[0].DettaglioPagamento[0].DataScadenzaPagamento = DateTime.Parse(valoreCampo);
                        body.DatiPagamento[0].DettaglioPagamento[0].DataScadenzaPagamentoSpecified = true;
                        break;
                    case "2.4.2.7":
                        body.DatiPagamento[0].DettaglioPagamento[0].CodUfficioPostale = valoreCampo;
                        break;
                    case "2.4.2.8":
                        body.DatiPagamento[0].DettaglioPagamento[0].CognomeQuietanzante = valoreCampo;
                        break;
                    case "2.4.2.9":
                        body.DatiPagamento[0].DettaglioPagamento[0].NomeQuietanzante = valoreCampo;
                        break;
                    case "2.4.2.10":
                        body.DatiPagamento[0].DettaglioPagamento[0].CFQuietanzante = valoreCampo;
                        break;
                    case "2.4.2.11":
                        body.DatiPagamento[0].DettaglioPagamento[0].TitoloQuietanzante = valoreCampo;
                        break;
                    case "2.4.2.14":
                        body.DatiPagamento[0].DettaglioPagamento[0].ABI = valoreCampo;
                        break;
                    case "2.4.2.15":
                        body.DatiPagamento[0].DettaglioPagamento[0].CAB = valoreCampo;
                        break;
                    case "2.4.2.17":
                        body.DatiPagamento[0].DettaglioPagamento[0].ScontoPagamentoAnticipato = Decimal.Parse(valoreCampo);
                        body.DatiPagamento[0].DettaglioPagamento[0].ScontoPagamentoAnticipatoSpecified = true;
                        break;
                    case "2.4.2.18":
                        body.DatiPagamento[0].DettaglioPagamento[0].DataLimitePagamentoAnticipato = DateTime.Parse(valoreCampo);
                        body.DatiPagamento[0].DettaglioPagamento[0].DataLimitePagamentoAnticipatoSpecified = true;
                        break;
                    case "2.4.2.19":
                        body.DatiPagamento[0].DettaglioPagamento[0].PenalitaPagamentiRitardati = Decimal.Parse(valoreCampo);
                        body.DatiPagamento[0].DettaglioPagamento[0].PenalitaPagamentiRitardatiSpecified = true;
                        break;
                    case "2.4.2.20":
                        body.DatiPagamento[0].DettaglioPagamento[0].DataDecorrenzaPenale = DateTime.Parse(valoreCampo);
                        body.DatiPagamento[0].DettaglioPagamento[0].DataDecorrenzaPenaleSpecified = true;
                        break;
                    case "2.4.2.21":
                        body.DatiPagamento[0].DettaglioPagamento[0].CodicePagamento = valoreCampo;
                        break;
                        #endregion
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Errore iserimento campo custom {0} - valore {1}", nomeCampo, valoreCampo);
                logger.Error("Eccezione: ", ex);
            }
        }

        #region Import automatico fatture
        public static List<string> ImportFatture_GetFatture()
        {
            DocsPaDB.Query_DocsPAWS.Fatturazione fatt = new DocsPaDB.Query_DocsPAWS.Fatturazione();
            return fatt.ImportFatture_GetFatture();
        }

        public static bool ImportFatture_Insert(string idFattura, string idProfile)
        {
            DocsPaDB.Query_DocsPAWS.Fatturazione fatt = new DocsPaDB.Query_DocsPAWS.Fatturazione();
            return fatt.ImportFatture_Insert(idFattura, idProfile);
        }

        public static bool ImportFatture_SetStatoInvio(string idProfile, bool daInviare, bool setData)
        {
            DocsPaDB.Query_DocsPAWS.Fatturazione fatt = new DocsPaDB.Query_DocsPAWS.Fatturazione();
            return fatt.ImportFatture_UpdateStatoInvio(idProfile, daInviare, setData);
        }

        public static void ImportFatture_InvioASDI(InfoUtente user)
        {
            logger.Info("BEGIN");
            DocsPaDB.Query_DocsPAWS.Fatturazione fatt = new DocsPaDB.Query_DocsPAWS.Fatturazione();

            logger.Debug("Reperimento fatture da inviare a SDI");
            List<string> list = fatt.ImportFatture_GetFattureDaInviare();

            if(list != null)
            {
                if(list.Count > 0)
                {
                    logger.DebugFormat("> {0} fatture da inviare", list.Count);

                    foreach(string idProfile in list)
                    {
                        try
                        {
                            // Estrazione diagramma
                            logger.Debug("> Estrazione diagramma");
                            int idDiagramma = DiagrammiStato.DiagrammiStato.getStatoDoc(idProfile).ID_DIAGRAMMA;

                            logger.Debug("> Avvio servizio fatturazione");
                            string result = ExternalServices.ExternalServicesManager.AvviaServizioFatturazione(user, idProfile, idDiagramma);

                            if (result == "1")
                            {
                                logger.Debug("> Aggiornamento stato diagramma");
                                DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = DiagrammiStato.DiagrammiStato.getDiagrammaById(idDiagramma.ToString());
                                foreach(DocsPaVO.DiagrammaStato.Stato stato in diagramma.STATI)
                                {
                                    if(stato.DESCRIZIONE.ToUpper() == "INVIA AL SDI")
                                    {
                                        DiagrammiStato.DiagrammiStato.salvaModificaStato(idProfile, stato.SYSTEM_ID.ToString(), diagramma, user.idPeople, user, string.Empty);
                                        break;
                                    }
                                }
                                
                                logger.Debug("> Aggiornamento stato invio");
                                ImportFatture_SetStatoInvio(idProfile, false, true);
                            }
                            else
                            {
                                throw new Exception(">>>> Codice errore servizio: " + result);
                            }

                        }
                        catch(Exception ex)
                        {
                            logger.Error(">> Errore nell'invio a SDI della fattura ID=" + idProfile, ex);
                        }
                    }
                }
                else
                {
                    logger.Debug("> Nessuna fattura da inviare");
                }
            }
            
            logger.Info("END");
        }

        public static DocsPaVO.documento.FileDocumento GetPreviewPdf(byte[] content)
        {
            logger.Debug("BEGIN");
            BusinessLogic.Modelli.AsposeModelProcessor.DocModelProcessor processor = new Modelli.AsposeModelProcessor.DocModelProcessor();

            DocsPaVO.documento.FileDocumento doc = null;

            doc = new FileDocumento();
            doc.content = processor.ConvertInvoiceToPdf(content);
            doc.contentType = "application/pdf";
            doc.length = doc.content.Length;

            logger.Debug("END");
            return doc;
        }

        #endregion

    }
}
