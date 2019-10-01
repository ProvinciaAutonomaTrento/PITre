using System;
using System.Xml;
using System.Data;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using DocsPaVO.ProfilazioneDinamica;
using DocsPaVO.documento;
using log4net;
using BusinessLogic.Documenti;
using DocsPaVO.utente;
using System.Linq;
using System.Xml.Linq;
using System.Collections;
using DocsPaVO.Spedizione;
namespace BusinessLogic.Interoperabilità
{
    /// <summary>
    /// </summary>
    /// 
    public interface IInteropSchedaDocHandler
    {

        void CustomizeSchedaDocNoSegnatura(SchedaDocumento schedaDoc, CMMsg mc, string filePath);

        void CustomizeSchedaDocSegnatura(SchedaDocumento schedaDoc, CMMsg mc, string filePath);
    }

    public partial class InteroperabilitaSegnatura
    {
        private static ILog logger = LogManager.GetLogger(typeof(InteroperabilitaSegnatura));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="mailAddress"></param>
        /// <param name="filepath"></param>
        /// <param name="filename"></param>
        /// <param name="reg"></param>
        /// <param name="infoUtente"></param>
        /// <param name="ruolo"></param>
        /// <param name="mailId"></param>
        /// <param name="mailSubject"></param>
        /// <param name="logger"></param>
        ///<param name="isPec">vale 1 se la mail che conteneva la segnatura è di tipo pec vale 0 altrimenti</param>
        /// <returns></returns>
        public static bool
            eseguiSegnatura(string serverName, string mailAddress, string filepath, string filename,
            DocsPaVO.utente.Registro reg, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo,
            string mailId, bool fatturaElDaPEC, string mailSubject, string isPec, out string err, out string docnumber, string nomeMail, string dataRicezione)
        {
            System.IO.FileStream fs = null;
            System.IO.FileStream fsAll = null;
            //Andrea De Marco - Aggiunto File Stream per inserimento allegato Segnatura.xml 
            System.IO.FileStream fsAllSegnatura = null;
            //End Andrea De Marco
            err = "";
            docnumber = string.Empty;
            DocsPaVO.documento.SchedaDocumento sd = null;
            bool daAggiornareUffRef = false;
            //creazione del doc con trattamento spazi bianchi

            XmlDocument doc = new XmlDocument();
            InteropResolver my = new InteropResolver();
            //XmlTextReader xtr = new XmlTextReader(new StreamReader(filepath + "\\" + filename)) { Namespaces = false };
            XmlTextReader xtr = new XmlTextReader(filepath + "\\" + filename) { Namespaces = false };
            xtr.WhitespaceHandling = WhitespaceHandling.None;
            XmlValidatingReader xvr = new XmlValidatingReader(xtr);
            xvr.ValidationType = System.Xml.ValidationType.DTD;
            xvr.EntityHandling = System.Xml.EntityHandling.ExpandCharEntities;
            xvr.XmlResolver = my;
            bool codint1 = false;
            string mailOrigine = string.Empty;

            //RGS
            string idMessaggioFlussoRGS = string.Empty;
            string idDocumentoRispostaRGS = string.Empty;
            string idFlussoRGS = string.Empty;
            string idFascicoloFlussoRGS = string.Empty;
            string nomeRegistroRGS = string.Empty;
            string numeroRegistroRGS = string.Empty;
            string dataRegistroRGS = string.Empty;

            try
            {
                doc.Load(xvr);
            }
            catch (System.Xml.Schema.XmlSchemaException e)
            {
                //logger.addMessage("La mail viene sospesa perche' il  file segnatura.xml non e' valido. Eccezione:"+e.Message); 
                logger.Error("La mail viene sospesa perche' il  file segnatura.xml non e' valido. Eccezione:" + e.Message);

                //********************************
                //** ATTENZIONE !!!!  ************
                //** NON RIMUOVERE (CODINTEROP2)**
                //** dal msg d'errore.          **
                //********************************
                err = "( CODINTEROP2 ) La mail viene sospesa perche' il  file segnatura.xml non e' valido. Eccezione:" + e.Message;

                if (InteroperabilitaUtils.MailElaborata(mailId, "D"))
                {
                    //logger.addMessage("Sospensione eseguita");
                    logger.Debug("Sospensione eseguita");
                }
                else
                {
                    //logger.addMessage("Sospensione non eseguita");
                    logger.Debug("Sospensione non eseguita");
                };
                return false;
            }
            catch (System.Xml.XmlException e)
            {
                logger.Error("La mail viene sospesa perche' il  file segnatura.xml non e' valido. Eccezione:" + e.Message);
                err = "( CODINTEROP2 ) La mail viene sospesa perche' il  file segnatura.xml non e' valido. Eccezione:" + e.Message;
                return false;
            }
            catch (Exception e)
            {
                //				logger.addMessage("La mail viene sospesa. Eccezione:"+e.Message); 
                logger.Error("La mail viene sospesa. Eccezione:" + e.Message);
                err = "La mail viene sospesa. Eccezione:" + e.Message;
                if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
                {
                    //					logger.addMessage("Sospensione eseguita");
                    logger.Debug("Sospensione eseguita");
                }
                else
                {
                    //					logger.addMessage("Sospensione non eseguita");
                    logger.Debug("Sospensione non eseguita");
                };

                return false;
            }
            finally
            {
                xvr.Close();
                xtr.Close();
            }
            try
            {
                XmlElement elIdentificatore = (XmlElement)doc.DocumentElement.SelectSingleNode("Intestazione/Identificatore");
                XmlElement elmailOrigine = ((XmlElement)doc.DocumentElement.SelectSingleNode("Intestazione/Origine"));
                mailOrigine = elmailOrigine.SelectSingleNode("IndirizzoTelematico").InnerText.Trim();
                string codiceAmministrazione = elIdentificatore.SelectSingleNode("CodiceAmministrazione").InnerText.Trim();
                //Andrea De Marco - Il campo di DB ha una lunghezza massima di 16 caratteri - CodiceAmministrazione non può essere più lungo di questo valore
                if (codiceAmministrazione.Length > 16) 
                {
                    codiceAmministrazione = codiceAmministrazione.Substring(0, 16);
                }
                //End Andrea De Marco
                string codiceAOO = elIdentificatore.SelectSingleNode("CodiceAOO").InnerText.Trim();
                //Andrea De Marco - Il campo di DB ha una lunghezza massima di 16 caratteri - CodiceAOO non può essere più lungo di questo valore
                if (codiceAOO.Length > 16)
                {
                    codiceAOO = codiceAOO.Substring(0, 16);
                }
                //End Andrea De Marco
                string numeroRegistrazione = elIdentificatore.SelectSingleNode("NumeroRegistrazione").InnerText.Trim();
                string dataRegistrazione = DocsPaUtils.Functions.Functions.CheckData(elIdentificatore.SelectSingleNode("DataRegistrazione").InnerText.Trim());
                bool confermaRic = false;

                if (dataRegistrazione == null)
                {
                    //					logger.addMessage("La mail viene sospesa. La data registrazione ha un formato non corretto"); 
                    logger.Debug("La mail viene sospesa. La data registrazione ha un formato non corretto");
                    err = "La mail viene sospesa. La data registrazione ha un formato non corretto";

                    if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
                    {
                        //logger.addMessage("Sospensione eseguita");
                        logger.Debug("Sospensione eseguita");
                    }
                    else
                    {
                        //logger.addMessage("Sospensione non eseguita");
                        logger.Debug("Sospensione non eseguita");
                    };
                    return false;
                }
                //				logger.addMessage("Data registrazione: "+dataRegistrazione);
                logger.Debug("Data registrazione: " + dataRegistrazione);

                string oggetto = doc.DocumentElement.SelectSingleNode("Intestazione/Oggetto").InnerText.Trim();

                //MITTENTE
                string rows = "";
                XmlElement elOrigine = (XmlElement)doc.DocumentElement.SelectSingleNode("Intestazione/Origine");
                XmlElement elMittente = (XmlElement)elOrigine.SelectSingleNode("Mittente");
                DocsPaVO.addressbook.TipoUtente tipoMittente;
                if (codiceAmministrazione.Equals(reg.codAmministrazione))
                {
                    tipoMittente = DocsPaVO.addressbook.TipoUtente.INTERNO;
                }
                else
                {
                    tipoMittente = DocsPaVO.addressbook.TipoUtente.ESTERNO;
                }
                DocsPaVO.utente.Corrispondente mittente = null;

                if (System.Configuration.ConfigurationManager.AppSettings["RICERCA_COD"] != null
                    && System.Configuration.ConfigurationManager.AppSettings["RICERCA_COD"].Trim().Equals("1"))
                    mittente = getMittente(elMittente, mailOrigine, codiceAmministrazione, tipoMittente, reg);
                else
                    mittente = getMittente(elMittente, mailOrigine, codiceAmministrazione, codiceAOO, tipoMittente, reg, infoUtente, out rows);


                if (mittente == null)
                {
                    //se il mittente è interno, allora ci si blocca!!!!!
                    if (tipoMittente == DocsPaVO.addressbook.TipoUtente.INTERNO)
                    {
                        //						logger.addMessage("La mail viene sospesa: mittente non trovato");
                        logger.Debug("La mail viene sospesa: mittente non trovato, perchè il tipo mittente è interno");
                        err = "La mail viene sospesa: il Mittente non risulta di tipo esterno.";
                        if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
                        {
                            //							logger.addMessage("Sospensione eseguita");
                            logger.Debug("Sospensione eseguita");
                        }
                        else
                        {
                            //							logger.addMessage("Sospensione non eseguita");
                            logger.Debug("Sospensione non eseguita");
                        };
                        return false;
                    }
                    else
                    {
                        mittente = addNewCorrispondente(elOrigine, elIdentificatore, reg);
                        //DocsPaDB.Query_DocsPAWS.Utenti users = new DocsPaDB.Query_DocsPAWS.Utenti();
                        //users.resetCorrVarInsertIterop(mittente.systemId, "3");
                        //AGGIORNAMENTO CANALE PREFERENZIALE
                        updateCanalePref(mittente);
                        //inserisco la mail associata al corrispondente esterno in DPA_MAIL_CORR_ESTERNI
                        List<DocsPaVO.utente.MailCorrispondente> casella = new List<DocsPaVO.utente.MailCorrispondente>();
                        casella.Add(new DocsPaVO.utente.MailCorrispondente(){
                                                                                Email = mittente.email, 
                                                                                Note = "",
                                                                                Principale = "1"});
                        BusinessLogic.Utenti.addressBookManager.InsertMailCorrispondente(casella, mittente.systemId);
                    }
                }
                else
                {
                    //					logger.addMessage("Mittente trovato"+mittente.GetType());
                    logger.Debug("Mittente trovato" + mittente.GetType());

                    /* 12/03/2018: non aggiorno la mail impostandola come preferità poichè ogni volta che si riutilizza questo corrispondende
                     * va a riaggiornare i dati rispetto a quelli di RC, reimpostando la vacchia mail come quella preferita ottendo come effotto continue
                     * storicizzazioni (INC000001044145 Comune di Rovereto)
                    //AGGIORNAMENTO DELLA MAIL E DELL'INTEROPERABILITA' DELLA UO DEL MITTENTE
                    string mailMittente = elOrigine.SelectSingleNode("IndirizzoTelematico").InnerText.Trim();
                    updateUOMittente(mittente, mailMittente);
                    //AGGIORNAMENTO CANALE PREFERENZIALE
                    updateCanalePref(mittente);
                    //DocsPaDB.Query_DocsPAWS.Utenti users = new DocsPaDB.Query_DocsPAWS.Utenti();
                    //if(rows.Equals("1"))
                    //    users.resetCorrVarInsertIterop(mittente.systemId, "1");
                    //else if(rows!="0" && rows!="1")
                    //    users.resetCorrVarInsertIterop(mittente.systemId, "2");
                    */
                }

                //DESTINATARI
                string infoDestinatari = getInfoDestinatari((XmlElement)doc.DocumentElement.SelectSingleNode("Intestazione"), ref confermaRic, mailAddress);
                //				logger.addMessage("Conferma ricezione: "+confermaRic);
                logger.Debug("Conferma ricezione: " + confermaRic);

                //DESCRIZIONE E DOCUMENTI
                //SA E SF se tipoRiferimento manca il default è MIME, in questo caso deve esserci il nome del file 
                //bisogna gestire il caso di tipoRiferimento='CARTACEO'
                bool tipoRiferimentoMIME = false;
                bool tipoRiferimentoCARTACEO = false;

                //CONTESTO PROCEDURALE
                #region Contesto procedurale RGS
                XmlElement riferimenti = (XmlElement)doc.DocumentElement.SelectSingleNode("Riferimenti");
                if (riferimenti != null)
                { 
                    //seleziono, se esiste, il contesto procedurale per il flusso RGS
                    XmlNodeList contestoProceduraleNode = riferimenti.SelectNodes("ContestoProcedurale");
                    if (contestoProceduraleNode != null && contestoProceduraleNode.Count > 0)
                    {
                        List<string> tipiCont = BusinessLogic.FlussoAutomatico.FlussoAutomaticoManager.GetTipiContestoProcedurale();
                        foreach (XmlElement contestoProcedurale in contestoProceduraleNode)
                        {
                            if (contestoProcedurale.SelectSingleNode("TipoContestoProcedurale") != null && !string.IsNullOrEmpty(contestoProcedurale.SelectSingleNode("TipoContestoProcedurale").InnerText))
                            {
                                string tipoContesto = (from tipo in tipiCont where tipo.Equals(contestoProcedurale.SelectSingleNode("TipoContestoProcedurale").InnerText) select tipo).FirstOrDefault();
                                if(!string.IsNullOrEmpty(tipoContesto))
                                {
                                    idMessaggioFlussoRGS = contestoProcedurale.SelectSingleNode("Identificativo").InnerText;
                                    idFlussoRGS = contestoProcedurale.Attributes["id"].Value;
                                    DocsPaVO.FlussoAutomatico.Flusso flusso = BusinessLogic.FlussoAutomatico.FlussoAutomaticoManager.GetFlussoInizioRichiesta(idFlussoRGS); 
                                    idDocumentoRispostaRGS = flusso.INFO_DOCUMENTO.ID_PROFILE;

                                    XmlElement metaDatiInterni = (XmlElement)contestoProcedurale.SelectSingleNode("PiuInfo/MetadatiInterni");
                                    XmlCDataSection cDataNode = (XmlCDataSection)(contestoProcedurale.SelectSingleNode("PiuInfo/MetadatiInterni").ChildNodes[0]);
                                    XmlDocument xmlDocCdata = new XmlDocument();
                                    xmlDocCdata.LoadXml(cDataNode.Value);
                                    XmlElement cDataEl = (XmlElement)xmlDocCdata.SelectSingleNode("TIPOLOGIA");
                                    XmlNodeList cDataNodes = cDataEl.SelectNodes("MetadatoAssociato");

                                    foreach (XmlElement metaDati in cDataNodes)
                                    {
                                        switch (metaDati.SelectSingleNode("Codice").InnerText)
                                        { 
                                            case "NOME_REGISTRO":
                                                nomeRegistroRGS = metaDati.SelectSingleNode("Valore").InnerText;
                                                break;
                                            case "NUMERO_REGISTRO":
                                                numeroRegistroRGS = metaDati.SelectSingleNode("Valore").InnerText;
                                                break;
                                            case "DATA_REGISTRO":
                                                dataRegistroRGS = metaDati.SelectSingleNode("Valore").InnerText;
                                                break;
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }

                }

                #endregion

                //DOCUMENTO PRINCIPALE
                string docPrincipaleName = "";
                XmlElement descrizione = (XmlElement)doc.DocumentElement.SelectSingleNode("Descrizione");
                if (descrizione.SelectSingleNode("TestoDelMessaggio") != null)
                {
                    docPrincipaleName = "body.html";
                }
                else
                {
                    //Verifichiamo il tipo di riferimento SAB
                    if ((descrizione.SelectSingleNode("Documento").Attributes["tipoRiferimento"] != null
                         && descrizione.SelectSingleNode("Documento").Attributes["tipoRiferimento"].Value.Equals("MIME"))
                        || descrizione.SelectSingleNode("Documento").Attributes["tipoRiferimento"] == null)
                        tipoRiferimentoMIME = true;
                    else if (descrizione.SelectSingleNode("Documento").Attributes["tipoRiferimento"] != null
                         && descrizione.SelectSingleNode("Documento").Attributes["tipoRiferimento"].Value.ToUpper().Equals("CARTACEO"))
                        tipoRiferimentoCARTACEO = true;

                    if (!tipoRiferimentoMIME && !tipoRiferimentoCARTACEO)
                        //commento SAB if (!descrizione.SelectSingleNode("Documento").Attributes["tipoRiferimento"].Value.Equals("MIME"))
                        if (!descrizione.SelectSingleNode("Documento").Attributes["tipoRiferimento"].Value.Equals("MIME"))
                    {
                        //						logger.addMessage("La mail viene sospesa. Il documento principale non ha MIME come tipo riferimento"); 
                        logger.Debug("La mail viene sospesa. Il documento principale non ha un tipo riferimento valido");
                        err = "La mail viene sospesa. Il documento principale non ha un tipo riferimento valido";
                        if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
                        {
                            //							logger.addMessage("Sospensione eseguita");
                            logger.Debug("Sospensione eseguita");
                        }
                        else
                        {
                            //							logger.addMessage("Sospensione non eseguita");
                            logger.Debug("Sospensione non eseguita");
                        };
                        return false;
                    }
                    if (tipoRiferimentoMIME && (descrizione.SelectSingleNode("Documento").Attributes["nome"] == null))
                    {
                        //						logger.addMessage("La mail viene sospesa. Non e' presente il nome del documento principale"); 
                        logger.Debug("La mail viene sospesa. Non e' presente il nome del documento principale");
                        err = "La mail viene sospesa. Non e' presente il nome del documento principale";
                        if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
                        {
                            //							logger.addMessage("Sospensione eseguita");
                            logger.Debug("Sospensione eseguita");
                        }
                        else
                        {
                            //							logger.addMessage("Sospensione non eseguita");
                            logger.Debug("Sospensione non eseguita");
                        };
                        return false;
                    }
                    if (tipoRiferimentoMIME)
                        docPrincipaleName = descrizione.SelectSingleNode("Documento").Attributes["nome"].Value;
                }

                //CONTROLLO DI CONSISTENZA DEI NOMI DEI DOCUMENTI
                //Documento principale
                if (docPrincipaleName != null && !docPrincipaleName.Equals(""))
                {
                    if (!System.IO.File.Exists(filepath + "\\" + docPrincipaleName))
                    {
                        //					logger.addMessage("La mail viene sospesa. Il documento principale indicato non e' presente"); 
                        logger.Debug("La mail viene sospesa. Il documento principale indicato non e' presente");
                        err = "La mail viene sospesa. Il documento principale indicato non e' presente";
                        if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
                        {
                            //						logger.addMessage("Sospensione eseguita");
                            logger.Debug("Sospensione eseguita");
                        }
                        else
                        {
                            //						logger.addMessage("Sospensione non eseguita");
                            logger.Debug("Sospensione non eseguita");
                        };
                        return false;
                    }
                    if (getApp(docPrincipaleName) == null)
                    {
                        //					logger.addMessage("La mail viene sospesa. Il formato file del documento principale non e' gestito"); 
                        logger.Debug("La mail viene sospesa. Il formato file del documento principale non e' gestito");
                        err = "La mail viene sospesa. Il formato file del documento principale non e' gestito";
                        if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
                        {
                            //						logger.addMessage("Sospensione eseguita");
                            logger.Debug("Sospensione eseguita");
                        }
                        else
                        {
                            //						logger.addMessage("Sospensione non eseguita");
                            logger.Debug("Sospensione non eseguita");
                        };
                        return false;
                    }
                }
                //Allegati
                //XmlElement allegati=(XmlElement)  doc.DocumentElement.SelectSingleNode("Descrizione/Allegati");
                XmlNodeList documenti = doc.DocumentElement.SelectNodes("Descrizione/Allegati/Documento[@tipoRiferimento='MIME']");
                for (int ind = 0; ind < documenti.Count; ind++)
                {
                    // SA E SF Se tipoRiferimento è null o è valorizzato con "MIME", si deve procedere con l'analisi 
                    if (((XmlElement)documenti[ind]).Attributes["tipoRiferimento"] == null ||
                        ((XmlElement)documenti[ind]).Attributes["tipoRiferimento"].Value.ToUpper() == "MIME")
                    {
                        //si verifica se e' specificato il nome dell'allegato
                        if (((XmlElement)documenti[ind]).Attributes["nome"] == null)
                        {
                            //						logger.addMessage("La mail viene sospesa. Non e' presente il nome dell'allegato "+ind); 
                            logger.Debug("La mail viene sospesa. Non e' presente il nome dell'allegato " + ind);
                            err = "La mail viene sospesa. Non e' presente il nome dell'allegato " + ind;
                            if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
                            {
                                //							logger.addMessage("Sospensione eseguita");
                                logger.Debug("Sospensione eseguita");
                            }
                            else
                            {
                                //							logger.addMessage("Sospensione non eseguita");
                                logger.Debug("Sospensione non eseguita");
                            };
                            return false;
                        }
                        string nome = ((XmlElement)documenti[ind]).Attributes["nome"].Value;
                        if (!System.IO.File.Exists(filepath + "\\" + nome))
                        {
                            //						logger.addMessage("La mail viene sospesa. Il documento "+nome+" non e' presente"); 
                            logger.Debug("La mail viene sospesa. Il documento " + nome + " non e' presente");
                            err = "La mail viene sospesa. Il documento " + nome + " non e' presente";

                            if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
                            {
                                //							logger.addMessage("Sospensione eseguita");
                                logger.Debug("Sospensione eseguita");
                            }
                            else
                            {
                                //							logger.addMessage("Sospensione non eseguita");
                                logger.Debug("Sospensione non eseguita");
                            };
                            return false;
                        };
                        if (getApp(nome) == null)
                        {
                            //						logger.addMessage("La mail viene sospesa. Il documento "+nome+" ha un formato non gestito"); 
                            logger.Debug("La mail viene sospesa. Il documento " + nome + " ha un formato non gestito");
                            err = "La mail viene sospesa. Il documento " + nome + " ha un formato non gestito";
                            if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
                            {
                                //							logger.addMessage("Sospensione eseguita");
                                logger.Debug("Sospensione eseguita");
                            }
                            else
                            {
                                //							logger.addMessage("Sospensione non eseguita");
                                logger.Debug("Sospensione non eseguita");
                            };
                            return false;
                        }
                    }
                }

               

                //fatti i controlli, si procede con la protocollazione
                //				logger.addMessage("Inserimento documento principale "+docPrincipaleName);
                logger.Debug("Inserimento documento principale " + docPrincipaleName);

                sd = new DocsPaVO.documento.SchedaDocumento();

                if (!string.IsNullOrEmpty(docPrincipaleName))
                    sd.appId = getApp(docPrincipaleName).application;
                sd.idPeople = infoUtente.idPeople;
                sd.userId = infoUtente.userId;

                string valorechiave = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_NOTE_IN_SEGNATURA");
                if (!string.IsNullOrEmpty(valorechiave) && valorechiave.Equals("1"))
                {
                    //NOTE (Augusto 30/08/2011)
                    XmlElement noteDescrizione = (XmlElement)doc.DocumentElement.SelectSingleNode("Descrizione/Note");

                    if (noteDescrizione != null && !string.IsNullOrEmpty(noteDescrizione.InnerText))
                    {
                        sd.noteDocumento = new List<DocsPaVO.Note.InfoNota>();
                        sd.noteDocumento.Add(
                            new DocsPaVO.Note.InfoNota 
                                { 
                                    DaInserire = true,
                                    TipoVisibilita = DocsPaVO.Note.TipiVisibilitaNotaEnum.Tutti
                                }
                        );
                    }
                }

                //sd.note=infoDestinatari; 
                DocsPaVO.documento.Oggetto ogg = new DocsPaVO.documento.Oggetto();
                ogg.descrizione = mailSubject;
                sd.oggetto = ogg;
                sd.predisponiProtocollazione = true;

                //old 28/04/08    sd.registro = reg;
                sd.registro = CaricaRegistroInScheda(reg);

                sd.tipoProto = "A";
                // sd.typeId = "MAIL";
                sd.typeId = "INTEROPERABILITA";
                sd.mezzoSpedizione = BusinessLogic.Documenti.InfoDocManager.getIdMezzoSpedizioneByDesc("INTEROPERABILITA");
                sd.descMezzoSpedizione = "INTEROPERABILITA";
                sd.interop = "S";
                //aggiunta protocollo entrata
                DocsPaVO.documento.ProtocolloEntrata protEntr = new DocsPaVO.documento.ProtocolloEntrata();
                protEntr.mittente = mittente;
                protEntr.dataProtocolloMittente = dataRegistrazione;
                protEntr.descrizioneProtocolloMittente = codiceAOO + DocsPaDB.Utils.Personalization.getInstance(reg.idAmministrazione).getSepSegnatura() + numeroRegistrazione;  //METTERE CARATTERE DELL'AMMINISTRAZIONE
                if (confermaRic)
                {
                    protEntr.invioConferma = "1";
                }
                sd.protocollo = protEntr;

                //dati utente/ruolo/Uo del creatore.
                sd.protocollatore = new DocsPaVO.documento.Protocollatore(infoUtente, ruolo);

                //Riferimenti Mittente
                sd.riferimentoMittente = getRiferimentiMittente(doc);
                // Per gestione pendenti tramite PEC
                if (InteroperabilitaUtils.MantieniMailRicevutePendenti(reg.systemId, mailAddress))
                {
                    sd.privato = "1";
                }

                if (!string.IsNullOrEmpty(idDocumentoRispostaRGS))
                {
                    sd.rispostaDocumento = new InfoDocumento();
                    sd.rispostaDocumento.idProfile = idDocumentoRispostaRGS;
                }

                try
                {
                    sd = BusinessLogic.Documenti.DocSave.addDocGrigia(sd, infoUtente, ruolo);
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOADDDOCGRIGIA", sd.systemId, string.Format("{0} {1}", "N.ro Doc.: ", sd.docNumber), DocsPaVO.Logger.CodAzione.Esito.OK);
                }
                catch (Exception excp)
                {
                    logger.Error("Errore nella creazione del documento . " + excp.Message);
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOADDDOCGRIGIA", "", string.Format("{0} {1}", "N.ro Doc.: ", ""), DocsPaVO.Logger.CodAzione.Esito.KO);
                }

                //				logger.addMessage("Salvataggio doc...");
                logger.Debug("Salvataggio doc...");
                //modifica
                sd.documento_da_pec = isPec;
                //fine modifica
                ((DocsPaVO.documento.Documento)sd.documenti[0]).dataArrivo = dataRicezione;
                sd = BusinessLogic.Documenti.DocSave.save(infoUtente, sd, false, out daAggiornareUffRef, ruolo);
                logger.Debug("Salvataggio eseguito");
                if (sd.tipoProto != "G")
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOSAVEDOCUMENTO", sd.systemId, string.Format("{0} {1}", "Aggiornamento Protocollo Numero ", sd.protocollo.segnatura), DocsPaVO.Logger.CodAzione.Esito.OK);
                else
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOSAVEDOCUMENTO", sd.systemId, string.Format("{0} {1}", "Aggiornamento Documento Numero ", sd.docNumber), DocsPaVO.Logger.CodAzione.Esito.OK);

                // Associazione del canale di spedizione interoperabilità al documento
                ProtoManager.collegaMezzoSpedizioneDocumento(infoUtente, sd.mezzoSpedizione, sd.systemId);

                //MULTI CASELLA: associazione documento mailAddress(necessario per l'invio della conferma di ricezione/ annullamento) 
                if (BusinessLogic.interoperabilita.InteroperabilitaManager.InsertAssDocAddress(sd.systemId, reg.systemId, mailAddress))
                    logger.Debug("associazione documento mail address correttamente eseguita in DPA_ASS_DOC_MAIL_INTEROP");
                else
                    logger.Debug("errore nell'associazione documento mail address in DPA_ASS_DOC_MAIL_INTEROP");
                try
                {
                    if (!tipoRiferimentoCARTACEO || !string.IsNullOrEmpty(docPrincipaleName))
                    {
                        DocsPaVO.documento.FileDocumento fd = new DocsPaVO.documento.FileDocumento();
                        fs = new System.IO.FileStream(filepath + "\\" + docPrincipaleName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, (int)fs.Length);
                        fs.Close();
                        fd.content = buffer;
                        fd.length = buffer.Length;
                        fd.name = docPrincipaleName;

                        DocsPaVO.documento.FileRequest fRSch = (DocsPaVO.documento.FileRequest)sd.documenti[0];

                        // inserito per permettere la memorizzazione corretta del path nella components 
                        // in caso di ricezione per interoperabilità e PitreDualFileWritingMode=false
                        fRSch.fileName = docPrincipaleName;

                        if (fd.content.Length > 0)
                        {
                            //OLD :  BusinessLogic.Documenti.FileManager.putFile((DocsPaVO.documento.FileRequest)sd.documenti[0], fd, infoUtente);
                            if (!BusinessLogic.Documenti.FileManager.putFile(ref fRSch, fd, infoUtente, out err))
                                throw new Exception(err);
                            else
                            {
                                logger.Debug("Documento principale inserito");
                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOPUTFILE", fRSch.docNumber, string.Format("{0} {1}", "Acquisito documento N.ro:", fRSch.docNumber), DocsPaVO.Logger.CodAzione.Esito.OK);
                                fRSch.fileName = docPrincipaleName;
                                InteroperabilitaUtils.MatchTSR(filepath, fRSch, fd, infoUtente);
                                XmlParsing.XmlParserManager.parseExtraXmlfiles(sd, docPrincipaleName, fd.content, infoUtente, ruolo);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (err == "")
                        err = "Errore nel reperimento del file principale";
                    //se il putFile va in errore devo rimuovere il profile (predisposto appena inserito)
                    BusinessLogic.Documenti.DocManager.ExecRimuoviSchedaMethod(infoUtente, sd);
                    logger.Error("Eseguita rimozione profilo");
                    logger.Error(err);
                    throw ex;
                }
                //ricerca degli allegati
                logger.Debug("Inserimento degli allegati");

                for (int i = 0; i < documenti.Count; i++)
                {
                    //estrazione dati dell'allegato
                    XmlElement documentoAllegato = (XmlElement)documenti[i];
                    //SA e SF controllo inserito per gestire solo i file di tipo MIME
                    if (((XmlElement)documentoAllegato).Attributes["tipoRiferimento"] == null ||
                        ((XmlElement)documentoAllegato).Attributes["tipoRiferimento"].Value.ToUpper() == "MIME")
                    {
                        string nomeAllegato = documentoAllegato.Attributes["nome"].Value;

                        if (InteroperabilitaUtils.FindTSRMatch(filepath, nomeAllegato))
                            continue; //il TSR fa parte di un o dei doc, salto l'acquisizione e l'aggiunta dell'allegato

                        XmlElement numPagine = (XmlElement)documentoAllegato.SelectSingleNode("NumeroPagine");
                        XmlElement titoloDoc = (XmlElement)documentoAllegato.SelectSingleNode("TitoloDocumento");
                        //					logger.addMessage("Inserimento allegato "+nomeAllegato);
                        logger.Debug("Inserimento allegato " + nomeAllegato);

                        DocsPaVO.documento.Allegato all = new DocsPaVO.documento.Allegato();
                        //					logger.addMessage("docnumber="+sd.docNumber);
                        logger.Debug("docnumber=" + sd.docNumber);

                        all.docNumber = sd.docNumber;
                        //all.applicazione=getApp(nomeAllegato,logger);
                        all.fileName = getFileName(nomeAllegato);
                        
                        all.descrizione = "allegato " + i;
                        if (!String.IsNullOrEmpty (all.fileName))
                            all.descrizione = all.fileName;

                        all.version = "0";
                        //numero pagine
                        if (numPagine != null && !numPagine.InnerText.Trim().Equals(""))
                        {
                            all.numeroPagine = Int32.Parse(numPagine.InnerText);
                        }
                        //descrizione allegato
                        if (titoloDoc != null && !titoloDoc.InnerText.Trim().Equals(""))
                        {
                            all.descrizione = titoloDoc.InnerText;
                        }

                        DocsPaVO.documento.Allegato res = null;
                        try
                        {
                            res = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, all);
                            if (res != null)
                            {
                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOAGGIUNGIALLEGATO", all.docNumber, string.Format("{0}{1}{2}{3}", "Aggiunto al N.ro Doc.: ", all.docNumber, " il N.ro Allegato: ", res.versionLabel), DocsPaVO.Logger.CodAzione.Esito.OK);
                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCNEWALLEGATO", all.docNumber, string.Format("{0}{1}{2}{3}", "Aggiunto al N.ro Doc.: ", all.docNumber, " il N.ro Allegato: ", res.versionLabel), DocsPaVO.Logger.CodAzione.Esito.OK);
                            }
                        }
                        catch (Exception e)
                        {
                            err = "errore nel metodo aggiungiAllegato per l'allegato n. " + Convert.ToString(i + 1);
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOAGGIUNGIALLEGATO", all.docNumber, string.Format("{0}{1}{2}", "Errore in aggiunta al N.ro Doc.: ", all.docNumber, " il Allegato: "), DocsPaVO.Logger.CodAzione.Esito.KO);
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCNEWALLEGATO", all.docNumber, string.Format("{0}{1}{2}", "Errore in aggiunta al N.ro Doc.: ", all.docNumber, " il Allegato: "), DocsPaVO.Logger.CodAzione.Esito.KO);
                            logger.Error(err);
                            throw e;
                        }


                        logger.Debug("Allegato id=" + all.versionId);
                        logger.Debug("Allegato version label=" + all.versionLabel);
                        logger.Debug("Inserimento nel filesystem");

                        try
                        {
                            DocsPaVO.documento.FileDocumento fdAll = new DocsPaVO.documento.FileDocumento();
                            fsAll = new System.IO.FileStream(filepath + "\\" + nomeAllegato, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                            byte[] bufferAll = new byte[fsAll.Length];
                            fsAll.Read(bufferAll, 0, (int)fsAll.Length);
                            fdAll.content = bufferAll;
                            fdAll.length = bufferAll.Length;
                            fdAll.name = nomeAllegato;
                            DocsPaVO.documento.FileRequest fRAll = (DocsPaVO.documento.FileRequest)sd.documenti[0];
                            fRAll = (DocsPaVO.documento.FileRequest)all;

                            // inserito per permettere la memorizzazione corretta del path nella components 
                            // in caso di ricezione per interoperabilità e PitreDualFileWritingMode=false
                            fRAll.fileName = nomeAllegato;

                            if (fdAll.content.Length > 0)
                            {
                                //OLD:  BusinessLogic.Documenti.FileManager.putFile((DocsPaVO.documento.FileRequest)all, fdAll, infoUtente);
                                if (!BusinessLogic.Documenti.FileManager.putFile(ref fRAll, fdAll, infoUtente, out err))
                                    throw new Exception(err);
                                else
                                {
                                    logger.Debug("Allegato " + i + " inserito");
                                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOPUTFILE", fRAll.docNumber, string.Format("{0} {1}", "Acquisito documento N.ro:", fRAll.docNumber), DocsPaVO.Logger.CodAzione.Esito.OK);
                                    fRAll.fileName = nomeAllegato;
                                    InteroperabilitaUtils.MatchTSR(filepath, fRAll, fdAll, infoUtente);
                                    XmlParsing.XmlParserManager.parseExtraXmlfiles(sd, nomeAllegato, fdAll.content, infoUtente, ruolo);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (err == "")
                                err = "Errore nel reperimento del file allegato";
                            //se il putFile va in errore devo rimuovere il profile (predisposto appena inserito)
                            BusinessLogic.Documenti.DocManager.ExecRimuoviSchedaMethod(infoUtente, sd);
                            logger.Debug("Eseguita rimozione profilo");
                            logger.Error(err);
                            throw ex;
                        }
                        finally
                        {

                            fsAll.Close();
                        }
                    }
                }


                //Andrea De Marco - Inserimento segnatura.xml come allegato - MEV Gestione Eccezioni PEC
                //Commentare per ripristinare la situazione precedente
                logger.Debug("Inserimento in allegati di segnatura.xml");

                
                    //estrazione dati dell'allegato
                    string nomeAllegato_Segnatura = "segnatura.xml";

                    logger.Debug("Inserimento allegato " + nomeAllegato_Segnatura);

                    DocsPaVO.documento.Allegato all_Segnatura = new DocsPaVO.documento.Allegato();
                    all_Segnatura.descrizione = "allegato segnatura.xml";

                    logger.Debug("docnumber=" + sd.docNumber);

                    all_Segnatura.docNumber = sd.docNumber;
                    //all.applicazione=getApp(nomeAllegato,logger);
                    all_Segnatura.fileName = getFileName(nomeAllegato_Segnatura);
                    all_Segnatura.version = "0";

                    DocsPaVO.documento.Allegato res2 = null;
                    try
                    {
                        res2 = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, all_Segnatura);
                        if (res2 != null)
                        {
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOAGGIUNGIALLEGATO", all_Segnatura.docNumber, string.Format("{0}{1}{2}{3}", "Aggiunto al N.ro Doc.: ", all_Segnatura.docNumber, " il N.ro Allegato: ", res2.versionLabel), DocsPaVO.Logger.CodAzione.Esito.OK);
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCNEWALLEGATO", all_Segnatura.docNumber, string.Format("{0}{1}{2}{3}", "Aggiunto al N.ro Doc.: ", all_Segnatura.docNumber, " il N.ro Allegato: ", res2.versionLabel), DocsPaVO.Logger.CodAzione.Esito.OK);
                        }

                    }
                    catch (Exception e)
                    {
                        err = "errore nel metodo aggiungiAllegato per l'allegato segnatura.xml";
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOAGGIUNGIALLEGATO", all_Segnatura.docNumber, string.Format("{0}{1}", "Errore in inserimento allegato al N.ro Doc.: ", all_Segnatura.docNumber), DocsPaVO.Logger.CodAzione.Esito.KO);
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCNEWALLEGATO", all_Segnatura.docNumber, string.Format("{0}{1}", "Errore in inserimento allegato al N.ro Doc.: ", all_Segnatura.docNumber), DocsPaVO.Logger.CodAzione.Esito.KO);
                        logger.Error(err);
                        throw e;
                    }


                    logger.Debug("Allegato id=" + all_Segnatura.versionId);
                    logger.Debug("Allegato version label=" + all_Segnatura.versionLabel);
                    logger.Debug("Inserimento nel filesystem");

                    try
                    {
                        DocsPaVO.documento.FileDocumento fdAllSegnatura = new DocsPaVO.documento.FileDocumento();
                        fsAllSegnatura = new System.IO.FileStream(filepath + "\\" + nomeAllegato_Segnatura, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                        byte[] bufferAllSegnatura = new byte[fsAllSegnatura.Length];
                        fsAllSegnatura.Read(bufferAllSegnatura, 0, (int)fsAllSegnatura.Length);
                        fdAllSegnatura.content = bufferAllSegnatura;
                        fdAllSegnatura.length = bufferAllSegnatura.Length;
                        fdAllSegnatura.name = nomeAllegato_Segnatura;
                        DocsPaVO.documento.FileRequest fRAllSegnatura = (DocsPaVO.documento.FileRequest)sd.documenti[0];
                        fRAllSegnatura = (DocsPaVO.documento.FileRequest)all_Segnatura;

                        // inserito per permettere la memorizzazione corretta del path nella components 
                        // in caso di ricezione per interoperabilità e PitreDualFileWritingMode=false
                        fRAllSegnatura.fileName = nomeAllegato_Segnatura;

                        if (fdAllSegnatura.content.Length > 0)
                        {
                            //OLD:  BusinessLogic.Documenti.FileManager.putFile((DocsPaVO.documento.FileRequest)all, fdAll, infoUtente);
                            if (!BusinessLogic.Documenti.FileManager.putFile(ref fRAllSegnatura, fdAllSegnatura, infoUtente, out err))
                                throw new Exception(err);
                            else
                            {
                                logger.Debug("Allegato segnatura.xml inserito");
                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOPUTFILE", fRAllSegnatura.docNumber, string.Format("{0} {1}", "Acquisito documento N.ro:", fRAllSegnatura.docNumber), DocsPaVO.Logger.CodAzione.Esito.OK);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (err == "")
                            err = "Errore nel reperimento del file allegato";
                        //se il putFile va in errore devo rimuovere il profile (predisposto appena inserito)
                        BusinessLogic.Documenti.DocManager.ExecRimuoviSchedaMethod(infoUtente, sd);
                        logger.Debug("Eseguita rimozione profilo");
                        logger.Error(err);
                        throw ex;
                    }
                    finally
                    {

                        fsAllSegnatura.Close();
                    }
                
                //End Andrea De Marco


                ///modifica per il salvataggio della mail

                //string valore = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "");

                if (!string.IsNullOrEmpty(nomeMail))
                {
                    logger.Debug("Inserimento allegato " + nomeMail);
                    DocsPaVO.documento.Allegato all = new DocsPaVO.documento.Allegato();
                    all.descrizione = "E-Mail Ricevuta";
                    logger.Debug("docnumber=" + sd.docNumber);
                    all.docNumber = sd.docNumber;
                    all.fileName = getFileName(nomeMail);
                    all.version = "0";
                    all.numeroPagine = 0;

                    DocsPaVO.documento.Allegato res3 = null;
                    try
                    {
                        res3 = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, all);
                        if (res3 != null)
                        {
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOAGGIUNGIALLEGATO", all.docNumber, string.Format("{0}{1}{2}{3}", "Aggiunto al N.ro Doc.: ", all.docNumber, " il N.ro Allegato: ", res3.versionLabel), DocsPaVO.Logger.CodAzione.Esito.OK);
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCNEWALLEGATO", all.docNumber, string.Format("{0}{1}{2}{3}", "Aggiunto al N.ro Doc.: ", all.docNumber, " il N.ro Allegato: ", res3.versionLabel), DocsPaVO.Logger.CodAzione.Esito.OK);
                        }
                    }
                    catch (Exception e)
                    {
                        err = "errore nel metodo aggiungiAllegato per il salvataggio della mail";
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOAGGIUNGIALLEGATO", all_Segnatura.docNumber, string.Format("{0}{1}", "Errore in inserimento allegato al N.ro Doc.: ", all.docNumber), DocsPaVO.Logger.CodAzione.Esito.KO);
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCNEWALLEGATO", all_Segnatura.docNumber, string.Format("{0}{1}", "Errore in inserimento allegato al N.ro Doc.: ", all.docNumber), DocsPaVO.Logger.CodAzione.Esito.KO);
                        logger.Error(err);
                        throw e;
                    }


                    logger.Debug("Allegato id=" + all.versionId);
                    logger.Debug("Allegato version label=" + all.versionLabel);
                    logger.Debug("Inserimento nel filesystem");

                    try
                    {
                        DocsPaVO.documento.FileDocumento fdAll = new DocsPaVO.documento.FileDocumento();
                        fsAll = new System.IO.FileStream(filepath + "\\" + nomeMail, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                        byte[] bufferAll = new byte[fsAll.Length];
                        fsAll.Read(bufferAll, 0, (int)fsAll.Length);
                        fdAll.content = bufferAll;
                        fdAll.length = bufferAll.Length;
                        fdAll.name = nomeMail;
                        DocsPaVO.documento.FileRequest fRAll = (DocsPaVO.documento.FileRequest)sd.documenti[0];
                        fRAll = (DocsPaVO.documento.FileRequest)all;
                        if (fdAll.content.Length > 0)
                        {
                            if (!BusinessLogic.Documenti.FileManager.putFile(ref fRAll, fdAll, infoUtente, out err))
                                throw new Exception(err);
                            else
                            {
                                logger.Debug("Salvataggio della mail inserito");
                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOPUTFILE", fRAll.docNumber, string.Format("{0} {1}", "Acquisito documento N.ro:", fRAll.docNumber), DocsPaVO.Logger.CodAzione.Esito.OK);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (err == "")
                            err = "Errore nel reperimento del file allegato";
                        BusinessLogic.Documenti.DocManager.ExecRimuoviSchedaMethod(infoUtente, sd);
                        logger.Debug("Eseguita rimozione profilo");
                        logger.Error(err);
                        throw ex;
                    }
                    finally
                    {

                        fsAll.Close();
                    }

                }
                //// fine modifica per il salvataggio

                // Per gestione pendenti tramite PEC
                if (true)//(string.IsNullOrEmpty(sd.privato)||!string.IsNullOrEmpty(sd.privato) && sd.privato != "1")
                {
                    //TRASMISSIONE   
                    //				logger.addMessage("Esegui trasmissione...");
                    logger.Debug("Esegui trasmissione...");

                    //eseguiTrasmissione(infoUtente.idPeople,serverName, sd,infoDestinatari,reg,ruolo);


                    //eseguiTrasmissione(infoUtente.idPeople,serverName,sd,null,reg,ruolo);
                    try
                    {
                        if (!string.IsNullOrEmpty(infoDestinatari) && infoDestinatari.Length > 248)
                            infoDestinatari = infoDestinatari.Substring(0, 248);
                        eseguiTrasmissione(infoUtente.idPeople, serverName, sd, infoDestinatari, reg, ruolo, infoUtente.dst, mailAddress, infoUtente, null);

                        // S. Furnari - 16/01/2013 - Sviluppo trasmissione documento ricevuto per interoperabilità interna
                        // solo a ruoli nella UO destinataria e non a tutta la AOO
                        //eseguiTrasmissione(infoUtente.idPeople, serverName, sd, infoDestinatari, reg, ruolo, infoUtente.dst, mailAddress);
                       //modifica furnari non so se và bene ---  eseguiTrasmissione(infoUtente.idPeople, serverName, sd, infoDestinatari, reg, ruolo, infoUtente.dst, mailAddress, null);

                        if (!checkExecTrasm(sd.systemId, "INTEROPERABILITA"))
                        {
                            codint1 = true;
                            err = "CODINTEROP1 Trasmissione per INTEROPERABILITA del documento con id:" + sd.docNumber + " non effettuata. Il documento può essere trovato tra i predisposti.";
                            throw new Exception("(CODINTEROP1) Trasmissione per INTEROPERABILITA del documento con id:" + sd.docNumber + "non effettuata!");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (sd != null)
                            docnumber = sd.docNumber;
                        if (InteroperabilitaUtils.MailElaborata(mailId, "E", reg.systemId, docnumber, mailOrigine))
                            logger.Debug("La mail è stata elaborata");
                        else
                            logger.Debug("La mail non è stata elaborata");

                        if (sd != null && sd.systemId != null && sd.systemId != "" && !codint1)
                        {
                            BusinessLogic.Documenti.DocManager.ExecRimuoviSchedaMethod(infoUtente, sd);
                            logger.Debug("Eseguita rimozione profilo");
                        }
                        logger.Error(err);
                        throw ex;
                    }
                }
                else
                {
                    logger.Debug("PEC4R1: Mail ricevute come pendenti. Provo a non eseguire la trasmissione.");
                }

                //chiusura canali
                xvr.Close();
                xtr.Close();
                if (fatturaElDaPEC)
                    BusinessLogic.Amministrazione.SistemiEsterni.FattElDaPEC(sd.docNumber, infoUtente);


                if (sd != null)
                {
                    docnumber = sd.docNumber;
                    try
                    {
                        //Inseirsco il flusso RGS
                        if (!string.IsNullOrEmpty(idMessaggioFlussoRGS))
                        {
                            DocsPaVO.FlussoAutomatico.InfoDocumentoFlusso infoDoc = new DocsPaVO.FlussoAutomatico.InfoDocumentoFlusso() { ID_PROFILE = sd.docNumber, NOME_REGISTRO_IN = nomeRegistroRGS, NUMERO_REGISTRO_IN = numeroRegistroRGS, DATA_REGISTRO_IN = dataRegistroRGS };
                            DocsPaVO.FlussoAutomatico.Messaggio messaggio = new DocsPaVO.FlussoAutomatico.Messaggio() { ID = idMessaggioFlussoRGS };
                            DocsPaVO.FlussoAutomatico.Flusso flusso = new DocsPaVO.FlussoAutomatico.Flusso() { ID_PROCESSO = idFlussoRGS, MESSAGGIO = messaggio, INFO_DOCUMENTO = infoDoc };

                            BusinessLogic.FlussoAutomatico.FlussoAutomaticoManager.InsertFlussoProcedurale(flusso);

                            //Estraggo il fascicolo in cui fascicolare il predisposto in arrivo
                            DocsPaVO.fascicolazione.Fascicolo fascicolo = BusinessLogic.FlussoAutomatico.FlussoAutomaticoManager.GetFasicoloByIdProfile(infoUtente, idDocumentoRispostaRGS);
                            if (fascicolo != null)
                            {
                                string msg = string.Empty;
                                if (fascicolo.folderSelezionato != null)
                                {
                                    BusinessLogic.Fascicoli.FolderManager.addDocFolder(infoUtente, sd.docNumber, fascicolo.folderSelezionato.systemID, false, out msg);
                                }
                                else
                                {
                                    BusinessLogic.Fascicoli.FascicoloManager.addDocFascicolo(infoUtente, sd.docNumber, fascicolo.systemID, true, out msg);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Errore in inserimento flusso automatico " + ex.Message);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                //				logger.addMessage("La mail viene sospesa. Eccezione: "+e.ToString()); 
                logger.Debug("La mail viene sospesa. Eccezione: " + e.Message.ToString());
                logger.Error("Errore nell'interoperabilità. La mail viene sospesa.", e);
                if (err == "")
                    err = "Errore nell'interoperabilità. La mail viene sospesa." + e.Message.ToString();
                if (err.Contains("CODINTEROP1"))
                {
                    if (sd != null)
                    {
                        docnumber = sd.docNumber;
                        if(fatturaElDaPEC)
                            BusinessLogic.Amministrazione.SistemiEsterni.FattElDaPEC(sd.docNumber, infoUtente);
                    }
                    if (InteroperabilitaUtils.MailElaborata(mailId, "E", reg.systemId, docnumber, mailOrigine))
                        logger.Debug("La mail è stata elaborata");
                    else
                        logger.Debug("La mail non è stata elaborata");
                }
                else
                {
                    if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
                        logger.Debug("Sospensione eseguita, errore: " + err);
                    else
                        logger.Debug("Sospensione non eseguita, errore " + err);

                }
                xvr.Close();
                xtr.Close();
                if (fsAll != null) fsAll.Close();
                if (fs != null) fs.Close();
                return false;
            }
        }

        /// <summary>
        /// Serve per la corretta valorizzazione del docnumber, deve essere numerico, non contenre spazi e caratteri alfabetici
        /// </summary>
        /// <param name="docnumber">docnumber da pulire</param>
        /// <returns>valore restituito</returns>
        public static string cleanDocNumber(string docnumber)
        {
            long convertedDocNumber = 0;
            bool isNum = long.TryParse(docnumber.Trim(), out convertedDocNumber);
            if (isNum)
                return convertedDocNumber.ToString();
            else
                logger.ErrorFormat("Errore nella formattazione del documber {0}: " + docnumber);

            return null;
        }

        public static bool controllaRiferimentoDocNumberDatiCert(string filepath,string filename)
        {

            DocsPaVO.DatiCert.Daticert daticert = new DocsPaVO.DatiCert.Daticert();
            XmlDocument Xmlfile = new XmlDocument();
            InteropResolver my = new InteropResolver();
            XmlTextReader xtr = new XmlTextReader(filepath + "\\" + filename);
            xtr.WhitespaceHandling = WhitespaceHandling.None;
            XmlValidatingReader xvr = new XmlValidatingReader(xtr);
            xvr.ValidationType = System.Xml.ValidationType.DTD;
            xvr.EntityHandling = System.Xml.EntityHandling.ExpandCharEntities;
            xvr.XmlResolver = my;
            XmlElement nodeRoot = Xmlfile.DocumentElement;
            try
            {
                Xmlfile.Load(xvr);
            }
            catch (System.Xml.Schema.XmlSchemaException e)
            {
                return false;
            }

            try
            {
                XmlNodeList xmlIntestazione = nodeRoot.SelectNodes("intestazione");
                int indice = 0;
                int massimo = xmlIntestazione.Item(0).ChildNodes.Count - 1;
                daticert.mittente = xmlIntestazione.Item(0).ChildNodes[indice].InnerText;
                indice++;


                if (xmlIntestazione.Item(0).ChildNodes[indice].Name.ToLower().Equals("oggetto".ToLower()))
                {
                    daticert.oggetto = xmlIntestazione.Item(0).ChildNodes[indice].InnerText.Replace("'", "'''");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore durante la lettura del file daticert.xml" + ex.Message);
            }

            try
            {
                string[] split = daticert.oggetto.Split('#');
                if (split.Length == 2)
                // nell'oggetto è presente solo un #
                {
                    daticert.oggetto = split[0];
                    daticert.docnumber = split[1];
                }
                // nell'oggetto sono presenti più #, in questo caso il docunumber lo trovo in posizione length-2
                else if (split.Length > 2)
                {
                    daticert.oggetto = split[0];
                    daticert.docnumber = split[split.Length - 2];
                }
                daticert.docnumber = cleanDocNumber(daticert.docnumber);
            }
            catch (Exception da)
            {
                logger.Error("errore nella ricerca del docnumer: " + da.Message);
            }


            if (String.IsNullOrEmpty(daticert.docnumber))
                return false;
            else
                return true;
        }

        public static bool leggiFileDatiCert(string filepath,
                           string filename, out string err, out string docnumber, List<string> documenti,
            DocsPaVO.utente.InfoUtente infoUtente, string mailId)
        {
            bool retval = false;

            err = string.Empty;
            docnumber = string.Empty;

            if (!File.Exists(filepath + "\\" + filename))
                return retval;


            #region datiCert
            DocsPaVO.DatiCert.Daticert daticert = new DocsPaVO.DatiCert.Daticert();
            err = string.Empty;
            XmlDocument Xmlfile = new XmlDocument();
            InteropResolver my = new InteropResolver();
            XmlTextReader xtr = new XmlTextReader(filepath + "\\" + filename);
            xtr.WhitespaceHandling = WhitespaceHandling.None;
            XmlValidatingReader xvr = new XmlValidatingReader(xtr);
            xvr.ValidationType = System.Xml.ValidationType.DTD;
            xvr.EntityHandling = System.Xml.EntityHandling.ExpandCharEntities;
            xvr.XmlResolver = my;

            try
            {

                Xmlfile.Load(xvr);
            }
            catch (System.Xml.Schema.XmlSchemaException e)
            {
                //logger.addMessage("La mail viene sospesa perche' il  file segnatura.xml non e' valido. Eccezione:"+e.Message); 
                logger.Error("La mail viene sospesa perche' il  file daticert.xml non e' valido. Eccezione:" + e.Message);

                err = "La mail viene sospesa perche' il  file daticert.xml non e' valido";

                if (InteroperabilitaUtils.MailElaborata(mailId, "D"))
                    logger.Debug("Sospensione eseguita");
                else
                    logger.Debug("Sospensione non eseguita");

                return false;
            }
            catch (Exception e)
            {
                xvr.Close();
                xtr.Close();
                logger.Error("Errore durante la lettura del file daticert.xml" + e.Message);
                err = "Errore durante la lettura del file daticert";
            }
            #region boby

            XmlElement nodeRoot = Xmlfile.DocumentElement;
            try
            {
                //puo assuemre uno dei seguenti valori accettazione |non-accettazione |presa-in-carico |
                //avvenuta-consegna |posta-certificata |errore-consegna |preavviso-errore-consegna |rilevazione-virus
                daticert.tipoRicevutaIntestazione = nodeRoot.GetAttributeNode("tipo").Value;
                //puo assumere uno dei seguenti valori nessuno |no-dest |no-dominio |virus |altro
                daticert.erroreRicevuta = nodeRoot.GetAttributeNode("errore").Value;
            }
            catch (Exception ex)
            {
                xvr.Close();
                xtr.Close();
                logger.Error("Errore durante la lettura del file daticert.xml" + ex.Message);
                err = "Errore durante la lettura del file daticert";
            }
            #endregion

            #region intestazione

            try
            {
                XmlNodeList xmlIntestazione = nodeRoot.SelectNodes("intestazione");
                int indice = 0;
                int massimo = xmlIntestazione.Item(0).ChildNodes.Count - 1;
                daticert.mittente = xmlIntestazione.Item(0).ChildNodes[indice].InnerText;
                indice++;
                //puo assumere uno dei seguenti valori certificato | esterno
                bool ok = false;
                List<string> destinatario = new List<string>();
                List<string> tipoDestinatario = new List<string>();
                while (!ok)
                {
                    if (xmlIntestazione.Item(0).ChildNodes[indice].Name.ToLower().Equals("destinatari".ToLower()))
                    {
                        tipoDestinatario.Add(xmlIntestazione.Item(0).ChildNodes[indice].Attributes.GetNamedItem("tipo").Value);
                        destinatario.Add(xmlIntestazione.Item(0).ChildNodes[indice].InnerText);
                        indice++;
                    }
                    else
                        break;
                }

                daticert.destinatarioLst = destinatario.ToArray();
                daticert.tipoDestinatarioLst = tipoDestinatario.ToArray();


                List<string> risposte = new List<string>();
                while (indice < massimo)
                {
                    if (xmlIntestazione.Item(0).ChildNodes[indice].Name.ToLower().Equals("risposte".ToLower()))
                    {
                        risposte.Add(xmlIntestazione.Item(0).ChildNodes[indice].InnerText);
                        indice++;
                    }
                }
                daticert.risposteLst = risposte.ToArray();


                if (xmlIntestazione.Item(0).ChildNodes[indice].Name.ToLower().Equals("oggetto".ToLower()))
                {
                    daticert.oggetto = xmlIntestazione.Item(0).ChildNodes[indice].InnerText.Replace("'", "'''");
                }
            #endregion

                #region dati

                XmlNodeList xmlDati = nodeRoot.SelectNodes("dati");
                indice = 0;
                massimo = xmlDati.Item(0).ChildNodes.Count;

                if (indice < massimo)
                {
                    if (xmlDati.Item(0).ChildNodes[indice].Name.ToLower().Equals("gestore-emittente".ToLower()))
                    {
                        daticert.gestioneEmittente = xmlDati.Item(0).ChildNodes[indice].InnerText;
                        indice++;
                    }
                }

                if (indice < massimo)
                {
                    if (xmlDati.Item(0).ChildNodes[indice].Name.ToLower().Equals("data".ToLower()))
                    {
                        daticert.zona = xmlDati.Item(0).ChildNodes[indice].Attributes.GetNamedItem("zona").Value;
                        daticert.giorno = xmlDati.Item(0).ChildNodes[indice].SelectNodes("giorno").Item(0).InnerText;
                        daticert.ora = xmlDati.Item(0).ChildNodes[indice].SelectNodes("ora").Item(0).InnerText;
                        indice++;
                    }
                }

                if (indice < massimo)
                {
                    if (xmlDati.Item(0).ChildNodes[indice].Name.ToLower().Equals("identificativo".ToLower()))
                    {
                        daticert.identificativo = xmlDati.Item(0).ChildNodes[indice].InnerText;
                        indice++;
                    }
                }
                if (indice < massimo)
                {
                    if (xmlDati.Item(0).ChildNodes[indice].Name.ToLower().Equals("msgid".ToLower()))
                    {
                        daticert.msgid = xmlDati.Item(0).ChildNodes[indice].InnerText;
                        indice++;
                    }
                }

                if (indice < massimo)
                {
                    if (xmlDati.Item(0).ChildNodes[indice].Name.ToLower().Equals("ricevuta".ToLower()))
                    {
                        daticert.ricevuta = xmlDati.Item(0).ChildNodes[indice].InnerText;
                        //puo assumere uno dei seguenti valori completa |breve |sintetica 
                        daticert.tipoRicevuta = xmlDati.Item(0).ChildNodes[indice].Attributes.GetNamedItem("tipo").Value;
                        indice++;
                    }

                }


                if (indice < massimo)
                {
                    if (xmlDati.Item(0).ChildNodes[indice].Name.ToLower().Equals("consegna".ToLower()))
                    {
                        daticert.consegna = xmlDati.Item(0).ChildNodes[indice].InnerText;
                        indice++;
                    }
                }

                if (indice < massimo)
                {
                    if (xmlDati.Item(0).ChildNodes[indice].Name.ToLower().Equals("ricezione".ToLower()))
                    {
                        daticert.ricezione = xmlDati.Item(0).ChildNodes[indice].InnerText;
                        indice++;
                    }
                }

                if (indice < massimo)
                {
                    if (xmlDati.Item(0).ChildNodes[indice].Name.ToLower().Equals("errore-esteso".ToLower()))
                    {
                        daticert.errore_esteso = xmlDati.Item(0).ChildNodes[indice].InnerText;
                        indice++;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore durante la lettura del file daticert.xml" + ex.Message);
                err = "Errore durante la lettuar del file daticert";
            }
                #endregion
            finally
            {
                xvr.Close();
                xtr.Close();
            }
            #endregion
            if (!string.IsNullOrEmpty(err))
                return retval;



            #region allegato
            string IdAllegato = string.Empty;
            logger.Debug("Inserimento degli allegati");
            try
            {
                string[] split = daticert.oggetto.Split('#');
                if (split.Length == 2)
                // nell'oggetto è presente solo un #
                {
                    daticert.oggetto = split[0];
                    daticert.docnumber = split[1];
                }
                // nell'oggetto sono presenti più #, in questo caso il docunumber lo trovo in posizione length-2
                else if (split.Length > 2)
                {
                    daticert.oggetto = split[0];
                    daticert.docnumber = split[split.Length - 2];
                }
                daticert.docnumber = cleanDocNumber(daticert.docnumber);
            }
            catch (Exception da)
            {
                logger.Error("errore nella ricerca del docnumer: " + da.Message);
            }

            if (string.IsNullOrEmpty(daticert.docnumber))
            {
                err = "La ricevuta non viene salvata perche manca il riferimento al documento";
                if (daticert.tipoRicevutaIntestazione.Equals("posta-certificata"))
                    err = "Impossibile scaricare una mail inoltrata";
                return retval;
            }


            DocsPaVO.DatiCert.TipoNotifica tiponotifica = BusinessLogic.interoperabilita.InteroperabilitaManager.ricercaTipoNotificaByCodice(daticert.tipoRicevutaIntestazione);

            string systemIdTipoNotifica = string.Empty;
            if (tiponotifica != null &&
                !string.IsNullOrEmpty(tiponotifica.idTipoNotifica))
                systemIdTipoNotifica = tiponotifica.idTipoNotifica;


            logger.Debug("inizio Allegato");
            if (!string.IsNullOrEmpty(daticert.docnumber))
            {
                string destinatario = (daticert.destinatarioLst != null && daticert.destinatarioLst.Count() > 0) ? daticert.destinatarioLst[0] : string.Empty;
                if (!BusinessLogic.interoperabilita.InteroperabilitaManager.verificaPresenzaNotifica(daticert, systemIdTipoNotifica))
                {
                    logger.Debug("Ci sono: " + documenti.Count + " Allegati");
                    for (int i = 0; i < documenti.Count; i++)
                    {
                        DocsPaVO.documento.SchedaDocumento sd = null;
                        string nomeAllegato = string.Empty;
                        DocsPaVO.documento.Allegato all = null;
                        try
                        {
                            //estrazione dati dell'allegato
                            nomeAllegato = documenti[i];
                            sd = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, daticert.docnumber);

                            all = new DocsPaVO.documento.Allegato();
                            all.docNumber = daticert.docnumber;
                            all.fileName = getFileName(nomeAllegato);
                            all.version = "0";
                            all.numeroPagine = 1;
                            all.TypeAttachment = 2;
                            //descrizione allegato
                            all.descrizione = "Ricevuta di ritorno delle Mail di tipo PEC - " + daticert.tipoRicevutaIntestazione + " - " + destinatario;
                        }
                        catch (Exception allllll)
                        {
                            logger.Error("errore nella creazioen del tipo allegato:" + allllll.Message);
                        }
                        logger.Debug("inizio salvataggio Allegato");
                        System.IO.FileStream fsAll = null;
                        DocsPaVO.documento.FileDocumento fdAll = null;
                        try
                        {
                            fdAll = new DocsPaVO.documento.FileDocumento();
                            fsAll = new System.IO.FileStream(filepath + "\\" + nomeAllegato, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                            byte[] bufferAll = new byte[fsAll.Length];
                            fsAll.Read(bufferAll, 0, (int)fsAll.Length);
                            fdAll.content = bufferAll;
                            fdAll.length = bufferAll.Length;
                            fdAll.name = nomeAllegato;
                        }
                        catch (Exception e)
                        {
                            logger.Debug("chiusura degli streem dell'allegato daticert");
                            xvr.Close();
                            xtr.Close();
                            if (fsAll != null)
                                fsAll.Close();
                            err = "Errore nel reperimento del file per l'allegato n. " + Convert.ToString(i + 1);
                            logger.Error(err);
                            throw e;
                        }
                        if (fdAll.content.Length > 0)
                        {
                            try
                            {
                                //questo metodo è stato eliminato perchè non permette di aggiungere un allegato associato a notifica pec in caso di documento consolidato
                                BusinessLogic.Documenti.AllegatiManager.aggiungiAllegatoPEC(infoUtente, all);
                                //BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, all);
                                IdAllegato = all.versionId; // necessario per associare la notifica al suo allegato
                                // Set del flag in CHA_ALLEGATI_ESTERNO in Versions
                                BusinessLogic.Documenti.AllegatiManager.setFlagAllegati_PEC_IS_EXT(all.versionId, all.docNumber, "P");
                            }
                            catch (Exception e)
                            {
                                logger.Debug("chiusura degli streem dell'allegato daticert");
                                xvr.Close();
                                xtr.Close();
                                if (fsAll != null)
                                    fsAll.Close();
                                err = "errore nel metodo aggiungiAllegato per l'allegato n. " + Convert.ToString(i + 1);
                                logger.Error(err);
                                throw e;
                            }


                            logger.Debug("Allegato id=" + all.versionId);
                            logger.Debug("Allegato version label=" + all.versionLabel);
                            logger.Debug("Inserimento nel filesystem");


                            try
                            {
                                DocsPaVO.documento.FileRequest fRAll = (DocsPaVO.documento.FileRequest)sd.documenti[0];
                                fRAll = (DocsPaVO.documento.FileRequest)all;
                                logger.Debug("controllo se esiste l'ext");
                                if (!BusinessLogic.Documenti.DocManager.esistiExt(nomeAllegato))
                                    BusinessLogic.Documenti.DocManager.insertExtNonGenerico(nomeAllegato, "application/octet-stream");

                                if (!BusinessLogic.Documenti.FileManager.putFile(ref fRAll, fdAll, infoUtente, out err))
                                {
                                    logger.Debug("errore durante la putfile");
                                    //  BusinessLogic.interoperabilita.InteroperabilitaManager.deleteNotifica(daticert.docnumber);
                                    throw new Exception(err);
                                }
                                else
                                    logger.Debug("Allegato " + i + " inserito");
                                retval = true;
                            }
                            catch (Exception ex)
                            {
                                if (err == "")
                                    err = "Errore nel reperimento del file allegato: " + nomeAllegato + ".";
                                BusinessLogic.Documenti.AllegatiManager.rimuoviAllegato(all, infoUtente);
                                logger.Error(err);
                            }
                            finally
                            {
                                logger.Debug("chiusura degli streem dell'allegato daticert");
                                xvr.Close();
                                xtr.Close();
                                if (fsAll != null)
                                    fsAll.Close();
                            }
                        }
                        else
                        {
                            logger.Debug("chiusura degli streem dell'allegato daticert");
                            xvr.Close();
                            xtr.Close();
                            if (fsAll != null)
                                fsAll.Close();
                            err = "Errore nel reperimento del file per l'allegato n. " + Convert.ToString(i + 1);
                            logger.Error(err);
                        }
                        if (!string.IsNullOrEmpty(err))
                        {
                            retval = false;
                            break;
                        }
                    }
                }
                else
                    retval = true;
            }
            #endregion


            #region inserimento dati
            if (!string.IsNullOrEmpty(err))
            {
                return retval;
            }



            if (string.IsNullOrEmpty(systemIdTipoNotifica))
            {
                BusinessLogic.interoperabilita.InteroperabilitaManager.inserimentoTipoNotifica(daticert.tipoRicevutaIntestazione);
                tiponotifica = BusinessLogic.interoperabilita.InteroperabilitaManager.ricercaTipoNotificaByCodice(daticert.tipoRicevutaIntestazione);
                if (tiponotifica != null &&
               !string.IsNullOrEmpty(tiponotifica.idTipoNotifica))
                    systemIdTipoNotifica = tiponotifica.idTipoNotifica;
                else
                {
                    err = "Errore durante il salvataggio delle informazioni relative al tipo della ricevuta. La Mail e i suoi allegati risultano salvata";
                    return retval;
                }
            }

            if (string.IsNullOrEmpty(systemIdTipoNotifica))
            {
                err = "Errore durante il salvataggio delle informazioni relative al daticert. La Mail e i suoi allegati risultano salvata";
                return retval;
            }

            for (int i = 0; i < daticert.destinatarioLst.Length; i++)
            {
                if (!BusinessLogic.interoperabilita.InteroperabilitaManager.inserimentoNotifica(daticert, systemIdTipoNotifica, i, IdAllegato))
                {
                    BusinessLogic.interoperabilita.InteroperabilitaManager.deleteNotifica(daticert.docnumber);
                    err = "Errore nel file Daticert.xml.";
                    return false;
                }
                else
                {
                    // Modifica PEC 4 requisito 2
                    if (tiponotifica.codiceNotifica == "errore" || tiponotifica.codiceNotifica == "DNS" || tiponotifica.codiceNotifica == "non-accettazione" || tiponotifica.codiceNotifica == "errore-consegna" || tiponotifica.codiceNotifica=="preavviso-errore-consegna")
                    {
                        logger.Debug("Scrivendo nella DPA_LOG. leggiFileDatiCert riga1261.");
                        DocsPaVO.DatiCert.Notifica not1 = new DocsPaVO.DatiCert.Notifica();
                        not1 = (DocsPaVO.DatiCert.Notifica)daticert;
                        // Modifica dei destinatari delle modifiche, imitando il comportamento IS

                        string ruoloDestinatari = infoUtente.idGruppo;
                        // Recupero il ruolo che ha effettuato l'ultima spedizione PEC, dallo storico delle spedizioni. 
                        ArrayList listHistorySendDoc = BusinessLogic.Spedizione.SpedizioneManager.GetElementiStoricoSpedizione(not1.docnumber);
                        if (listHistorySendDoc != null && listHistorySendDoc.Count > 0)
                        {
                            Object lastSendPec = (from record in listHistorySendDoc.ToArray()
                                                  where ((ElStoricoSpedizioni)record).Mail.ToLower().Equals(not1.destinatario.ToLower()) && ((ElStoricoSpedizioni)record).Esito.Equals("Spedito")
                                                  select record).ToList().FirstOrDefault();
                            if (lastSendPec != null)
                            {
                                ruoloDestinatari = ((ElStoricoSpedizioni)lastSendPec).IdGroupSender;
                            }
                        }
                        if (retval)
                        {
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, ruoloDestinatari, infoUtente.idAmministrazione, "NO_DELIVERY_SEND_PEC", not1.docnumber, tiponotifica.descrizioneNotifica + " è stata inviata il '" + not1.data_ora + "'"
                                + "'. Il destinatario '" + not1.destinatario + "' ha una mail di tipo: '" + (not1.tipoDestinatario.ToUpper().Equals("ESTERNO") ? "MAIL NON CERTIFICATA" : not1.tipoDestinatario) + "'."
                                    + "Il codice identificatore del messaggio è: '" + not1.identificativo + "'.", DocsPaVO.Logger.CodAzione.Esito.OK, (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "1");
                        }                        
                    }
                }

            }

            if (daticert.risposteLst.Length > 1)
            {
                DocsPaVO.DatiCert.Notifica[] notifica = BusinessLogic.interoperabilita.InteroperabilitaManager.ricercaNotifiche(daticert.docnumber);


                daticert.risposteLst[0].Remove(0);
                for (int i = 0; i < daticert.risposteLst.Length; i++)
                    for (int j = 0; j < notifica.Length; j++)
                    {
                        bool inserimento = false;
                        notifica[i].risposte = daticert.risposteLst[i];
                        inserimento = BusinessLogic.interoperabilita.InteroperabilitaManager.inserimentoNotifica(notifica[i],IdAllegato);
                        if (!inserimento)
                            err = "Errore durante l'inserimento delle notifiche.";
                        else
                        {
                            // Modifica PEC 4 requisito 2
                            if (tiponotifica.codiceNotifica == "errore" || tiponotifica.codiceNotifica == "DNS" || tiponotifica.codiceNotifica == "non-accettazione" || tiponotifica.codiceNotifica == "errore-consegna" || tiponotifica.codiceNotifica=="preavviso-errore-consegna")
                            {
                                logger.Debug("Scrivendo nella DPA_LOG. leggiFileDatiCert riga1292.");
                                // Modifica dei destinatari delle modifiche, imitando il comportamento IS

                                string ruoloDestinatari = infoUtente.idGruppo;
                                // Recupero il ruolo che ha effettuato l'ultima spedizione PEC, dallo storico delle spedizioni. 
                                ArrayList listHistorySendDoc = BusinessLogic.Spedizione.SpedizioneManager.GetElementiStoricoSpedizione(notifica[i].docnumber);
                                if (listHistorySendDoc != null && listHistorySendDoc.Count > 0)
                                {
                                    Object lastSendPec = (from record in listHistorySendDoc.ToArray()
                                                          where ((ElStoricoSpedizioni)record).Mail.ToLower().Equals(notifica[i].destinatario.ToLower()) && ((ElStoricoSpedizioni)record).Esito.Equals("Spedito")
                                                          select record).ToList().FirstOrDefault();
                                    if (lastSendPec != null)
                                    {
                                        ruoloDestinatari = ((ElStoricoSpedizioni)lastSendPec).IdGroupSender;
                                    }
                                }
                                if (retval)
                                {
                                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, ruoloDestinatari, infoUtente.idAmministrazione, "NO_DELIVERY_SEND_PEC", notifica[i].docnumber, tiponotifica.descrizioneNotifica + " è stata inviata il '" + notifica[i].data_ora + "'"
                                   + "'. Il destinatario '" + notifica[i].destinatario + "' ha una mail di tipo: '" + (notifica[i].tipoDestinatario.ToUpper().Equals("ESTERNO") ? "MAIL NON CERTIFICATA" : notifica[i].tipoDestinatario) + "'."
                                       + "Il codice identificatore del messaggio è: '" + notifica[i].identificativo + "'.", DocsPaVO.Logger.CodAzione.Esito.OK, (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "1");
                                }
                            }
                        }
                    }
            }
            #endregion
            if(daticert != null)
                docnumber = daticert.docnumber;
            return retval;
        }

        


        public static bool gestioneDatiCert(string filepath,
                      string filename, out string err, List<string> documenti,
                      DocsPaVO.utente.InfoUtente infoUtente, string mailId)
        {
            bool retval = false;

            err = string.Empty;

            if (!File.Exists(filepath + "\\" + filename))
                return retval;

            DocsPaVO.DatiCert.Daticert daticert = InteroperabilitaSegnatura.letturaDaticert(filepath, filename, mailId, out err);
            if (!string.IsNullOrEmpty(err))
                return retval;

            InteroperabilitaSegnatura.allegatiDatiCert(daticert, filepath, documenti, infoUtente, out err);
            if (!string.IsNullOrEmpty(err))
                return retval;

            retval = InteroperabilitaSegnatura.inserimentoDatiCert(daticert, out err);
            if (!string.IsNullOrEmpty(err))
                retval = false;

            return retval;
        }


        public static bool ricevuteSenzaDatiCert(string subject, string CodicetipoRicevuta, string filepath,
                      out string err, List<string> documenti,
                      DocsPaVO.utente.InfoUtente infoUtente, string mailId)
        {
            bool retval = false;
            err = string.Empty;
            DocsPaVO.DatiCert.Daticert daticert = new DocsPaVO.DatiCert.Daticert();
            daticert.oggetto = subject;
            daticert.tipoRicevutaIntestazione = CodicetipoRicevuta;
            try
            {
                string[] split = daticert.oggetto.Split('#');
                if (split.Length == 2)
                // nell'oggetto è presente solo un #
                {
                    daticert.oggetto = split[0];
                    daticert.docnumber = split[1];
                }
                // nell'oggetto sono presenti più #, in questo caso il docunumber lo trovo in posizione length-2
                else if (split.Length > 2)
                {
                    daticert.oggetto = split[0];
                    daticert.docnumber = split[split.Length - 2];
                }
                daticert.docnumber = cleanDocNumber(daticert.docnumber);
            }
            catch (Exception da)
            {
                logger.Error("errore nella ricerca del docnumer: " + da.Message);
            }

            if (string.IsNullOrEmpty(daticert.docnumber))
            {
                err = "La ricevuta non viene salvata perche manca il riferimento al documento";
                return retval;
            }


            retval = InteroperabilitaSegnatura.allegatiDatiCert(daticert, filepath, documenti, infoUtente, out err);

            if (retval)
            {
                retval = InteroperabilitaSegnatura.inserimentoDatiCert(daticert, out err);
            }

            return retval;
        }

        public static DocsPaVO.DatiCert.Daticert letturaDaticert(string filepath, string filename, string mailId, out string err)
        {

            DocsPaVO.DatiCert.Daticert daticert = new DocsPaVO.DatiCert.Daticert();
            err = string.Empty;
            XmlDocument Xmlfile = new XmlDocument();
            InteropResolver my = new InteropResolver();
            XmlTextReader xtr = new XmlTextReader(filepath + "\\" + filename);
            xtr.WhitespaceHandling = WhitespaceHandling.None;
            XmlValidatingReader xvr = new XmlValidatingReader(xtr);
            //xvr.ValidationType = System.Xml.ValidationType.DTD;
            //xvr.EntityHandling = System.Xml.EntityHandling.ExpandCharEntities;
            //xvr.XmlResolver = my;

            try
            {

                Xmlfile.Load(xvr);
            }
            catch (System.Xml.Schema.XmlSchemaException e)
            {
                //logger.addMessage("La mail viene sospesa perche' il  file segnatura.xml non e' valido. Eccezione:"+e.Message); 
                logger.Error("La mail viene sospesa perche' il  file daticert.xml non e' valido. Eccezione:" + e.Message);

                err = "La mail viene sospesa perche' il  file daticert.xml non e' valido";

                if (InteroperabilitaUtils.MailElaborata(mailId, "D"))
                    logger.Debug("Sospensione eseguita");
                else
                    logger.Debug("Sospensione non eseguita");

                return null;
            }

            #region boby

            XmlElement nodeRoot = Xmlfile.DocumentElement;
            //puo assuemre uno dei seguenti valori accettazione |non-accettazione |presa-in-carico |
            //avvenuta-consegna |posta-certificata |errore-consegna |preavviso-errore-consegna |rilevazione-virus
            daticert.tipoRicevutaIntestazione = nodeRoot.GetAttributeNode("tipo").Value;
            //puo assumere uno dei seguenti valori nessuno |no-dest |no-dominio |virus |altro
            daticert.erroreRicevuta = nodeRoot.GetAttributeNode("errore").Value;

            #endregion

            #region intestazione

            try
            {
                XmlNodeList xmlIntestazione = nodeRoot.SelectNodes("intestazione");
                int indice = 0;
                int massimo = xmlIntestazione.Item(0).ChildNodes.Count - 1;
                daticert.mittente = xmlIntestazione.Item(0).ChildNodes[indice].InnerText;
                indice++;
                //puo assumere uno dei seguenti valori certificato | esterno
                bool ok = false;
                List<string> destinatario = new List<string>();
                List<string> tipoDestinatario = new List<string>();
                while (!ok)
                {
                    if (xmlIntestazione.Item(0).ChildNodes[indice].Name.ToLower().Equals("destinatari".ToLower()))
                    {
                        tipoDestinatario.Add(xmlIntestazione.Item(0).ChildNodes[indice].Attributes.GetNamedItem("tipo").Value);
                        destinatario.Add(xmlIntestazione.Item(0).ChildNodes[indice].InnerText);
                        indice++;
                    }
                    else
                        break;
                }

                daticert.destinatarioLst = destinatario.ToArray();
                daticert.tipoDestinatarioLst = tipoDestinatario.ToArray();


                List<string> risposte = new List<string>();
                while (indice < massimo)
                {
                    if (xmlIntestazione.Item(0).ChildNodes[indice].Name.ToLower().Equals("risposte".ToLower()))
                    {
                        risposte.Add(xmlIntestazione.Item(0).ChildNodes[indice].InnerText);
                        indice++;
                    }
                }
                daticert.risposteLst = risposte.ToArray();


                if (xmlIntestazione.Item(0).ChildNodes[indice].Name.ToLower().Equals("oggetto".ToLower()))
                {
                    daticert.oggetto = xmlIntestazione.Item(0).ChildNodes[indice].InnerText;
                }
            #endregion

                #region dati

                XmlNodeList xmlDati = nodeRoot.SelectNodes("dati");
                indice = 0;
                massimo = xmlDati.Item(0).ChildNodes.Count;

                if (indice < massimo)
                {
                    if (xmlDati.Item(0).ChildNodes[indice].Name.ToLower().Equals("gestore-emittente".ToLower()))
                    {
                        daticert.gestioneEmittente = xmlDati.Item(0).ChildNodes[indice].InnerText;
                        indice++;
                    }
                }

                if (indice < massimo)
                {
                    if (xmlDati.Item(0).ChildNodes[indice].Name.ToLower().Equals("data".ToLower()))
                    {
                        daticert.zona = xmlDati.Item(0).ChildNodes[indice].Attributes.GetNamedItem("zona").Value;
                        daticert.giorno = xmlDati.Item(0).ChildNodes[indice].SelectNodes("giorno").Item(0).InnerText;
                        daticert.ora = xmlDati.Item(0).ChildNodes[indice].SelectNodes("ora").Item(0).InnerText;
                        indice++;
                    }
                }

                if (indice < massimo)
                {
                    if (xmlDati.Item(0).ChildNodes[indice].Name.ToLower().Equals("identificativo".ToLower()))
                    {
                        daticert.identificativo = xmlDati.Item(0).ChildNodes[indice].InnerText;
                        indice++;
                    }
                }
                if (indice < massimo)
                {
                    if (xmlDati.Item(0).ChildNodes[indice].Name.ToLower().Equals("msgid".ToLower()))
                    {
                        daticert.msgid = xmlDati.Item(0).ChildNodes[indice].InnerText;
                        indice++;
                    }
                }

                if (indice < massimo)
                {
                    if (xmlDati.Item(0).ChildNodes[indice].Name.ToLower().Equals("ricevuta".ToLower()))
                    {
                        daticert.ricevuta = xmlDati.Item(0).ChildNodes[indice].InnerText;
                        //puo assumere uno dei seguenti valori completa |breve |sintetica 
                        daticert.tipoRicevuta = xmlDati.Item(0).ChildNodes[indice].Attributes.GetNamedItem("tipo").Value;
                        indice++;
                    }

                }


                if (indice < massimo)
                {
                    if (xmlDati.Item(0).ChildNodes[indice].Name.ToLower().Equals("consegna".ToLower()))
                    {
                        daticert.consegna = xmlDati.Item(0).ChildNodes[indice].InnerText;
                        indice++;
                    }
                }

                if (indice < massimo)
                {
                    if (xmlDati.Item(0).ChildNodes[indice].Name.ToLower().Equals("ricezione".ToLower()))
                    {
                        daticert.ricezione = xmlDati.Item(0).ChildNodes[indice].InnerText;
                        indice++;
                    }
                }

                if (indice < massimo)
                {
                    if (xmlDati.Item(0).ChildNodes[indice].Name.ToLower().Equals("errore-esteso".ToLower()))
                    {
                        daticert.errore_esteso = xmlDati.Item(0).ChildNodes[indice].InnerText;
                        indice++;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore durante la lettura del file daticert.xml" + ex.Message);
                err = "Errore durante la lettuar del file daticert";
                daticert = null;
            }
                #endregion

            xvr.Close();
            xtr.Close();

            return daticert;
        }

        public static bool allegatiDatiCert(DocsPaVO.DatiCert.Daticert daticert, string filepath, List<string> documenti, DocsPaVO.utente.InfoUtente infoUtente, out string err)
        {
            bool retval = false;
            err = string.Empty;
            logger.Debug("Inserimento degli allegati");
            try
            {
                string[] split = daticert.oggetto.Split('#');
                if (split.Length == 2)
                // nell'oggetto è presente solo un #
                {
                    daticert.oggetto = split[0];
                    daticert.docnumber = split[1];
                }
                // nell'oggetto sono presenti più #, in questo caso il docunumber lo trovo in posizione length-2
                else if (split.Length > 2)
                {
                    daticert.oggetto = split[0];
                    daticert.docnumber = split[split.Length - 2];
                }
                daticert.docnumber = cleanDocNumber(daticert.docnumber);
            }
            catch (Exception da)
            {
                logger.Error("errore nella ricerca del docnumer: " + da.Message);
            }

            if (string.IsNullOrEmpty(daticert.docnumber))
            {
                err = "La ricevuta non viene salvata perche manca il riferimento al documento";
                return retval;
            }

            logger.Debug("inizio Allegato");
            if (!string.IsNullOrEmpty(daticert.docnumber))
            {
                logger.Debug("Ci sono: " + documenti.Count + " Allegati");
                for (int i = 0; i < documenti.Count; i++)
                {
                    DocsPaVO.documento.SchedaDocumento sd = null;
                    string nomeAllegato = string.Empty;
                    DocsPaVO.documento.Allegato all = null;
                    try
                    {
                        //estrazione dati dell'allegato
                        nomeAllegato = documenti[i];
                        sd = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, daticert.docnumber);

                        all = new DocsPaVO.documento.Allegato();
                        all.docNumber = daticert.docnumber;
                        all.fileName = getFileName(nomeAllegato);
                        all.version = "0";
                        all.numeroPagine = 1;
                        all.TypeAttachment = 2;
                        //descrizione allegato
                        all.descrizione = "Ricevuta di ritorno delle Mail di tipo PEC - " + daticert.tipoRicevutaIntestazione;
                    }
                    catch (Exception allllll)
                    {
                        logger.Error("errore nella creazioen del tipo allegato:" + allllll.Message);
                    }
                    logger.Debug("inizio salvataggio Allegato");

                    try
                    {
                        BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, all);
                        // Set del flag in CHA_ALLEGATI_ESTERNO in Versions
                        BusinessLogic.Documenti.AllegatiManager.setFlagAllegati_PEC_IS_EXT(all.versionId, all.docNumber, "P");
                        
                    }
                    catch (Exception e)
                    {
                        err = "errore nel metodo aggiungiAllegato per l'allegato n. " + Convert.ToString(i + 1);
                        logger.Error(err);
                        throw e;
                    }


                    logger.Debug("Allegato id=" + all.versionId);
                    logger.Debug("Allegato version label=" + all.versionLabel);
                    logger.Debug("Inserimento nel filesystem");

                    System.IO.FileStream fsAll = null;
                    try
                    {
                        DocsPaVO.documento.FileDocumento fdAll = new DocsPaVO.documento.FileDocumento();
                        fsAll = new System.IO.FileStream(filepath + "\\" + nomeAllegato, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                        byte[] bufferAll = new byte[fsAll.Length];
                        fsAll.Read(bufferAll, 0, (int)fsAll.Length);
                        fdAll.content = bufferAll;
                        fdAll.length = bufferAll.Length;
                        fdAll.name = nomeAllegato;
                        DocsPaVO.documento.FileRequest fRAll = (DocsPaVO.documento.FileRequest)sd.documenti[0];
                        fRAll = (DocsPaVO.documento.FileRequest)all;
                        if (fdAll.content.Length > 0)
                        {
                            logger.Debug("controllo se esiste l'ext");
                            if (!BusinessLogic.Documenti.DocManager.esistiExt(nomeAllegato))
                                BusinessLogic.Documenti.DocManager.insertExtNonGenerico(nomeAllegato, "application/octet-stream");

                            if (!BusinessLogic.Documenti.FileManager.putFile(ref fRAll, fdAll, infoUtente, out err))
                            {
                                logger.Debug("errore durante la putfile");
                                //  BusinessLogic.interoperabilita.InteroperabilitaManager.deleteNotifica(daticert.docnumber);
                                throw new Exception(err);
                            }
                            else
                                logger.Debug("Allegato " + i + " inserito");
                        }
                        retval = true;
                    }
                    catch (Exception ex)
                    {
                        if (err == "")
                            err = "Errore nel reperimento del file allegato: " + nomeAllegato + ".";
                        BusinessLogic.Documenti.AllegatiManager.rimuoviAllegato(all, infoUtente);
                        logger.Error(err + " ;" + ex.Message);
                    }
                    finally
                    {
                        logger.Debug("chiusura degli streem dell'allegato daticert");
                        if (fsAll != null)
                            fsAll.Close();
                    }

                    if (!string.IsNullOrEmpty(err))
                        break;
                }
            }

            return retval;
        }

        public static bool inserimentoDatiCert(DocsPaVO.DatiCert.Daticert daticert, out string err)
        {
            bool retval = false;
            err = string.Empty;
            DocsPaVO.DatiCert.TipoNotifica tiponotifica = BusinessLogic.interoperabilita.InteroperabilitaManager.ricercaTipoNotificaByCodice(daticert.tipoRicevutaIntestazione);

            string systemIdTipoNotifica = string.Empty;
            if (tiponotifica != null &&
                !string.IsNullOrEmpty(tiponotifica.idTipoNotifica))
                systemIdTipoNotifica = tiponotifica.idTipoNotifica;

            if (string.IsNullOrEmpty(systemIdTipoNotifica))
            {
                BusinessLogic.interoperabilita.InteroperabilitaManager.inserimentoTipoNotifica(daticert.tipoRicevutaIntestazione);
                tiponotifica = BusinessLogic.interoperabilita.InteroperabilitaManager.ricercaTipoNotificaByCodice(daticert.tipoRicevutaIntestazione);
                if (tiponotifica != null &&
               !string.IsNullOrEmpty(tiponotifica.idTipoNotifica))
                    systemIdTipoNotifica = tiponotifica.idTipoNotifica;
                else
                {
                    err = "Errore durante il salvataggio delle informazioni relative al tipo della ricevuta. La Mail e i suoi allegati risultano salvata";
                    return retval;
                }
            }

            if (string.IsNullOrEmpty(systemIdTipoNotifica))
            {
                err = "Errore durante il salvataggio delle informazioni relative al daticert. La Mail e i suoi allegati risultano salvata";
                return retval;
            }

            if (daticert.destinatarioLst != null)
            {
                for (int i = 0; i < daticert.destinatarioLst.Length; i++)
                {
                    if (!BusinessLogic.interoperabilita.InteroperabilitaManager.inserimentoNotifica(daticert, systemIdTipoNotifica, i,null))
                    {
                        BusinessLogic.interoperabilita.InteroperabilitaManager.deleteNotifica(daticert.docnumber);
                        err = "Errore nel file Daticert.xml.";
                        return false;
                    }
                }
            }
            else
            {
                daticert.destinatarioLst = new string[0];
                if (!BusinessLogic.interoperabilita.InteroperabilitaManager.inserimentoNotifica(daticert, systemIdTipoNotifica, 0,null))
                {
                    BusinessLogic.interoperabilita.InteroperabilitaManager.deleteNotifica(daticert.docnumber);
                    err = "Errore nel file Daticert.xml.";
                    return false;
                }
            }

            if (daticert.risposteLst.Length > 1)
            {
                DocsPaVO.DatiCert.Notifica[] notifica = BusinessLogic.interoperabilita.InteroperabilitaManager.ricercaNotifiche(daticert.docnumber);


                daticert.risposteLst[0].Remove(0);
                for (int i = 0; i < daticert.risposteLst.Length; i++)
                    for (int j = 0; j < notifica.Length; j++)
                    {
                        bool inserimento = false;
                        notifica[i].risposte = daticert.risposteLst[i];
                        inserimento = BusinessLogic.interoperabilita.InteroperabilitaManager.inserimentoNotifica(notifica[i],null);
                        if (!inserimento)
                            err = "Errore durante l'inserimento delle notifiche.";
                    }
            }
            return retval;
        }

        /// <summary>
        /// questo metodo esegue la procedura dell'interoperabilità a partire da un oggetto schedaDocuemento, un registro.
        /// la schedadocumento sarebbe quella di un documente protocollato in uscita e spedito alla casella del registro  (parametro)
        /// come paramento. l'infoUtente è quella del utente e ruolo che crea il documento predisposto alla protocollazione che viene creato a partire dalla schedaDocumento (paramento).
        /// il mittente del documento predisposto alla protocollazione è il ruolo che ha creato la schedadocumento (parametro) del protocollato in uscita e spedito alla casella del registro.
        /// il parametro di uscita err è una stringa che riporta il docnumber se la procedura ha avuto successo, altrimenti da un messaggio di errore e il docnumber se è stato creato il documento predisposto  
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="reg"></param>
        /// <param name="infoUtente"></param>
        /// <param name="ruolo"></param>
        /// <param name="sch"></param>
        /// <param name="err"></param>
        /// <returns></returns>

        #region old eseguiSegnaturaNoMail
        /* old 
        public static bool eseguiSegnaturaNoMail(string serverName,DocsPaVO.utente.Registro reg, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo,DocsPaVO.documento.SchedaDocumento  sch,out string err)
		{
			err=string.Empty;
			System.IO.FileStream fs=null;
			System.IO.FileStream fsAll=null;
			DocsPaVO.documento.SchedaDocumento sd=null;
			bool daAggiornareUffRef = false;
			string filepath="";
			string docPrincipaleName="";
			try
			{
				//se arriva sch con solo system_id e docnumber la ricerco
				if(sch!=null && sch.protocollo==null)
				{
					sch=BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, sch.systemId, sch.docNumber);
				}

				
				sd=new DocsPaVO.documento.SchedaDocumento();
				if(sch.documenti!=null && sch.documenti[0]!=null &&
					Int32.Parse(((DocsPaVO.documento.FileRequest)sch.documenti[0]).fileSize)>0)
				{
					string n=((DocsPaVO.documento.FileRequest)sch.documenti[0]).fileName;
					sd.appId=getApp(n).application;
					//apro subito il file per vedere se ci sono problemi nell'apertura dello stream
					try
					{
						filepath=((DocsPaVO.documento.FileRequest)sch.documenti[0]).docServerLoc+((DocsPaVO.documento.FileRequest)sch.documenti[0]).path;
						docPrincipaleName=((DocsPaVO.documento.FileRequest)sch.documenti[0]).fileName;
						DocsPaVO.documento.FileDocumento fd=new DocsPaVO.documento.FileDocumento();
                        if (filepath != null && filepath != "")
                        {
                            fs = new System.IO.FileStream(filepath + "\\" + docPrincipaleName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
                            logger.Debug("controllo del file: " + filepath + "\\" + docPrincipaleName + " effettuata con successo");
					
                        }
                        else
                        {
                            fs = new System.IO.FileStream(docPrincipaleName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
                            logger.Debug("controllo del file: " + docPrincipaleName + " effettuata con successo");
					
                        }
                    }
					catch (Exception ex)
					{
						
						err=ex.Message ;
						logger.Debug(err);
						throw ex;
						//se non si riesce a prener eil file per problemi vari continuo comunque per arrivare a eseguiTrasmissione
					}
					finally
					{
						fs.Flush();
						fs.Close();
						fs=null;
					}

					//allegati
					for(int i=0;sch.allegati!=null && i<sch.allegati.Count;i++)
					{
						DocsPaVO.documento.Allegato documentoAllegato=(DocsPaVO.documento.Allegato) sch.allegati[i];
						filepath=documentoAllegato.docServerLoc+documentoAllegato.path;
						if(Int32.Parse(documentoAllegato.fileSize)>0)
						{
						
							try
							{
                                if (filepath != null && filepath != "")
                                {
                                    fsAll = new System.IO.FileStream(filepath + "\\" + documentoAllegato.fileName, System.IO.FileMode.Open);
                                    logger.Debug("controllo del file: " + filepath + "\\" + documentoAllegato.fileName + " effettuata con successo");
                                }
                                else
                                {
                                    fsAll = new System.IO.FileStream(documentoAllegato.fileName, System.IO.FileMode.Open);
                                    logger.Debug("controllo del file: "+ documentoAllegato.fileName + " effettuata con successo");
                                }
								
							}
							catch (Exception ex)
							{ 
								err=ex.Message ;
								logger.Debug(err);
								throw ex;
							}
							finally
							{
								fsAll.Flush();
								fsAll.Close();
								fsAll=null;
							
							}
						}
					}

				}
				else
					sd.appId="ACROBAT";

				sd.idPeople=infoUtente.idPeople;
				sd.userId = infoUtente.userId;
				//sd.note=infoDestinatari; 
				
				sd.oggetto=sch.oggetto; 
				sd.predisponiProtocollazione=true;
				sd.registro=reg;
				sd.tipoProto="A";
				sd.typeId="MAIL";
				sd.interop="I";
				//aggiunta protocollo entrata
				DocsPaVO.documento.ProtocolloEntrata protEntr=new DocsPaVO.documento.ProtocolloEntrata();
				DocsPaVO.utente.Ruolo ruo=BusinessLogic.Utenti.UserManager.getRuolo(sch.protocollatore.ruolo_idCorrGlobali);
				protEntr.mittente=ruo;
				protEntr.dataProtocolloMittente=sch.protocollo.dataProtocollazione;
				// reg.codRegistro + ...
				protEntr.invioConferma="1";protEntr.descrizioneProtocolloMittente=sch.registro.codRegistro + DocsPaDB.Utils.Personalization.getInstance(reg.idAmministrazione).getSepSegnatura()+sch.protocollo.numero;  //METTERE CARATTERE DELL'AMMINISTRAZIONE
				
				
				sd.protocollo=protEntr;

				//dati utente/ruolo/Uo del creatore.
				sd.protocollatore=new DocsPaVO.documento.Protocollatore(infoUtente,ruolo);
				
				sd=BusinessLogic.Documenti.DocSave.addDocGrigia(sd,infoUtente,ruolo);
				
				logger.Debug("Salvataggio doc...");

				sd=BusinessLogic.Documenti.DocSave.save(infoUtente, sd, false, out daAggiornareUffRef);
			
				logger.Debug("Salvataggio eseguito");
				
				if(sch.documenti!=null && sch.documenti[0]!=null &&
					Int32.Parse(((DocsPaVO.documento.FileRequest)sch.documenti[0]).fileSize)>0)
				{
					try
					{
						filepath=((DocsPaVO.documento.FileRequest)sch.documenti[0]).docServerLoc+((DocsPaVO.documento.FileRequest)sch.documenti[0]).path;
						docPrincipaleName=((DocsPaVO.documento.FileRequest)sch.documenti[0]).fileName;
						DocsPaVO.documento.FileDocumento fd=new DocsPaVO.documento.FileDocumento();
						//fs=new System.IO.FileStream(filepath+"\\"+docPrincipaleName,System.IO.FileMode.Open,System.IO.FileAccess.Read,System.IO.FileShare.Read);
                        if (filepath != null && filepath != "") //in ETDOCS è vuoto
                        {
                            fs = new System.IO.FileStream(filepath + "\\" + docPrincipaleName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);

                        }
                        else
                        {
                            fs = new System.IO.FileStream(docPrincipaleName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);

                        }
                        byte[] bufferDoc = new byte[fs.Length];
						fs.Read(bufferDoc,0,(int)fs.Length);
						fd.content=bufferDoc;
						fd.length=bufferDoc.Length;
						fd.name=docPrincipaleName;
						BusinessLogic.Documenti.FileManager.putFile((DocsPaVO.documento.FileRequest)sd.documenti[0],fd,infoUtente);
						logger.Debug("file principale inserito");
					}
					catch (Exception ex)
					{
						err=err="Errore nel reperimento del file principale "+filepath+"\\"+docPrincipaleName+" eccezione: "+ex.Message ;
						logger.Debug(err);
						throw ex;
						//se non si riesce a prener eil file per problemi vari continuo comunque per arrivare a eseguiTrasmissione
					}
					finally
					{
						fs.Flush();
						fs.Close();
					}

					
				}
				

				//ricerca degli allegati
				//				logger.addMessage("Inserimento degli allegati");
				logger.Debug("Inserimento degli allegati");

				for(int i=0;sch.allegati!=null && i<sch.allegati.Count;i++)
				{
					//estrazione dati dell'allegato
					DocsPaVO.documento.Allegato documentoAllegato=(DocsPaVO.documento.Allegato) sch.allegati[i];
					filepath=documentoAllegato.docServerLoc+documentoAllegato.path;
					string nomeAllegato=documentoAllegato.fileName;
					string numPagine=documentoAllegato.numeroPagine.ToString();
					string titoloDoc=documentoAllegato.descrizione;
					//					logger.addMessage("Inserimento allegato "+nomeAllegato);
					logger.Debug("Inserimento allegato "+nomeAllegato);

					DocsPaVO.documento.Allegato all=new DocsPaVO.documento.Allegato();
					all.descrizione="allegato "+i;
					//					logger.addMessage("docnumber="+sd.docNumber);
					logger.Debug("docnumber="+sd.docNumber);

					all.docNumber=sd.docNumber;
					//all.applicazione=getApp(nomeAllegato,logger);
					all.fileName=getFileName(nomeAllegato);
					all.version="0";
					//numero pagine
					if(numPagine!=null && !numPagine.Trim().Equals(""))
					{
						all.numeroPagine=Int32.Parse(numPagine);
					}
					//descrizione allegato
					if(titoloDoc!=null && !titoloDoc.Trim().Equals(""))
					{
						all.descrizione=titoloDoc;
					}


					BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente,all);

					#region Codice Commentato
					//					logger.addMessage("Allegato id="+all.versionId);
					//					logger.addMessage("Allegato version label="+all.versionLabel);
					//					logger.addMessage("Inserimento nel filesystem");
					#endregion 

					logger.Debug("Allegato id="+all.versionId);
					logger.Debug("Allegato version label="+all.versionLabel);
					logger.Debug("Inserimento nel filesystem");
					
					DocsPaVO.documento.FileDocumento fdAll=new DocsPaVO.documento.FileDocumento();
					
					if(Int32.Parse(documentoAllegato.fileSize)>0)
					{
						try
						{
							//fsAll=new System.IO.FileStream(filepath+"\\"+nomeAllegato,System.IO.FileMode.Open);
                            if (filepath != null && filepath != "")
                            {
                                fsAll = new System.IO.FileStream(filepath + "\\" + nomeAllegato, System.IO.FileMode.Open);

                            }
                            else
                            {
                                fsAll = new System.IO.FileStream(nomeAllegato, System.IO.FileMode.Open);

                            }
                            byte[] bufferAll = new byte[fsAll.Length];
							fsAll.Read(bufferAll,0,(int)fsAll.Length);
							fdAll.content=bufferAll;
							fdAll.length=bufferAll.Length;
							fdAll.name=nomeAllegato;
													
				
							BusinessLogic.Documenti.FileManager.putFile((DocsPaVO.documento.FileRequest)all,fdAll,infoUtente);
							//					logger.addMessage("Allegato "+i+" inserito");
							logger.Debug("Allegato "+i+" inserito");
					
						}
						catch (Exception ex)
						{ 
							err="Errore nel reperimento dell'allegato "+filepath+"\\"+nomeAllegato+" eccezione: "+ex.Message ;
							logger.Debug(err);//se non si riesce a prener eil file per problemi vari continuo comunque per arrivare a eseguiTrasmissione
							throw ex;
						}
						finally
						{
							fsAll.Flush();
							fsAll.Close();
							
						}
					}
				}

				//TRASMISSIONE   
				//				logger.addMessage("Esegui trasmissione...");
				logger.Debug("Esegui trasmissione...");
				
				//eseguiTrasmissione(infoUtente.idPeople,serverName, sd,infoDestinatari,reg,ruolo);
				eseguiTrasmissione(infoUtente.idPeople,serverName, sd,"INTEROPERABILITA",reg,ruolo,infoUtente.dst);
				
				

				err=err+" "+sd.docNumber;
				return true;
			}
			catch (Exception ex) 
			{
				if(sd!=null && sd.docNumber!=null)
				{
					err="errore "+err+" Docnumber predisposto: "+sd.docNumber+" "+ex.Message.ToString();
				}
				else
					err="errore "+err+" "+ex.Message.ToString();
				logger.Debug(err);
			
				return false;	
			}
		}*/

        #endregion

        /// <summary>
        /// IACOZZILLI 21/06/2012
        /// NOTA: Ho dovuto aggiungere un param,etro in chiamata:
        /// infoUtenteFisico , mi serve per l'invio dell'XML a presidenza per i doc UBR
        /// Nella 312 me lo portavo, devo portarlo anche ora.
        /// </summary>
        /// <param name="infoUtenteFisico"></param>
        /// <param name="serverName"></param>
        /// <param name="reg"></param>
        /// <param name="infoUtente"></param>
        /// <param name="ruolo"></param>
        /// <param name="sch"></param>
        /// <param name="err"></param>
        /// <param name="dia"></param>
        /// <param name="mailAddress"></param>

        /// /// <param name="recipient">Destinatario della spedizione. Viene utilizzato se è attivo la funzionalità di trasmissione selettiva, per determinare i corrispondente cui effettuare la trasmissione</param>
        /// <returns></returns>
        public static bool eseguiSegnaturaNoMail(DocsPaVO.utente.InfoUtente infoUtenteFisico, string serverName, DocsPaVO.utente.Registro reg, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.documento.SchedaDocumento sch, out string err, out DocsPaVO.Interoperabilita.DatiInteropAutomatica dia, string mailAddress, Corrispondente recipient)
        //public static bool eseguiSegnaturaNoMail(DocsPaVO.utente.InfoUtente infoUtenteFisico, string serverName, DocsPaVO.utente.Registro reg, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.documento.SchedaDocumento sch, out string err, out DocsPaVO.Interoperabilita.DatiInteropAutomatica dia, string mailAddress)
        {
            err = string.Empty;
            System.IO.FileStream fs = null;
            System.IO.FileStream fsAll = null;
            DocsPaVO.documento.SchedaDocumento sd = null;
            bool daAggiornareUffRef = false;
            string filepath = "";
            string docPrincipaleName = "";
            dia = null;
            DocsPaVO.documento.FileDocumento fd = null;
            try
            {
                //se arriva sch con solo system_id e docnumber la ricerco
                if (sch != null && sch.protocollo == null)
                {
                    sch = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, sch.systemId, sch.docNumber);
                }


                sd = new DocsPaVO.documento.SchedaDocumento();
                if (sch.documenti != null && sch.documenti[0] != null &&
                    Int32.Parse(((DocsPaVO.documento.FileRequest)sch.documenti[0]).fileSize) > 0)
                {
                    sd.appId = sch.appId;

                }
                else
                    sd.appId = "ACROBAT";

                if (sd.appId == null)
                    sd.appId = "ACROBAT";

                sd.idPeople = infoUtente.idPeople;
                sd.userId = infoUtente.userId;
                //sd.note=infoDestinatari; 

                string statoReg = BusinessLogic.Utenti.RegistriManager.getStatoRegistro(reg);

                sd.oggetto = sch.oggetto;
                if (reg.autoInterop != null && reg.autoInterop.Equals("2") && statoReg == "V")
                {
                    //registro AUTOMATICO: creo direttamente un protocollo sulla AOO destinataria
                    //solo se il registro di destinazione è in verde, altrim creo il predisposto
                    sd.predisponiProtocollazione = false;

                }
                else
                {
                    //registro SEMIAUTOMATICO O MANUALE: comportamento rimane inalterato
                    sd.predisponiProtocollazione = true;
                }


                sd.registro = reg;
                sd.tipoProto = "A";
                sd.typeId = "INTEROPERABILITA";
                sd.interop = "I";
                sd.descMezzoSpedizione = "INTEROPERABILITA";
                sd.mezzoSpedizione = BusinessLogic.Documenti.InfoDocManager.getIdMezzoSpedizioneByDesc("INTEROPERABILITA");
                //aggiunta protocollo entrata

                DocsPaVO.documento.ProtocolloEntrata protEntr = new DocsPaVO.documento.ProtocolloEntrata();
                if (((DocsPaVO.documento.ProtocolloUscita)(sch.protocollo)).mittente != null)
                {
                    DocsPaVO.utente.Corrispondente corr = ((DocsPaVO.documento.ProtocolloUscita)(sch.protocollo)).mittente;
                    protEntr.mittente = corr;
                }
                else
                {
                    DocsPaVO.utente.Ruolo corr = BusinessLogic.Utenti.UserManager.getRuolo(sch.protocollatore.ruolo_idCorrGlobali);
                    protEntr.mittente = corr;
                }
                protEntr.dataProtocolloMittente = sch.protocollo.dataProtocollazione;
                // reg.codRegistro + ...
                protEntr.invioConferma = "1";
                protEntr.descrizioneProtocolloMittente = sch.registro.codRegistro + DocsPaDB.Utils.Personalization.getInstance(reg.idAmministrazione).getSepSegnatura() + sch.protocollo.numero;  //METTERE CARATTERE DELL'AMMINISTRAZIONE


                sd.protocollo = protEntr;

                //dati utente/ruolo/Uo del creatore.
                sd.protocollatore = new DocsPaVO.documento.Protocollatore(infoUtente, ruolo);

                try
                {
                    sd = BusinessLogic.Documenti.DocSave.addDocGrigia(sd, infoUtente, ruolo);
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOADDDOCGRIGIA", sd.systemId, string.Format("{0} {1}", "N.ro Doc.: ", sd.docNumber), DocsPaVO.Logger.CodAzione.Esito.OK);
                }
                catch (Exception excp)
                {
                    logger.Error("Errore nella creazione del documento . " + excp.Message);
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOADDDOCGRIGIA", "", string.Format("{0} {1}", "N.ro Doc.: ", ""), DocsPaVO.Logger.CodAzione.Esito.KO);
                }

                logger.Debug("Salvataggio doc...");


                if (DocsPaUtils.Configuration.CustomConfigurationBaseManager.isEnableRiferimentiMittente())
                    sd.riferimentoMittente = sch.riferimentoMittente;

                if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.CustomConfigurationBaseManager.isEnableProtocolloTitolario()))
                    sd.riferimentoMittente = sch.protocolloTitolario;

                sd = BusinessLogic.Documenti.DocSave.save(infoUtente, sd, false, out daAggiornareUffRef, ruolo);
                if (sd.tipoProto != "G")
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOSAVEDOCUMENTO", sd.systemId, string.Format("{0} {1}", "Aggiornamento Protocollo Numero ", sd.protocollo.segnatura), DocsPaVO.Logger.CodAzione.Esito.OK);
                else
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOSAVEDOCUMENTO", sd.systemId, string.Format("{0} {1}", "Aggiornamento Documento Numero ", sd.docNumber), DocsPaVO.Logger.CodAzione.Esito.OK);

                logger.Debug("Salvataggio eseguito");
                //}
                DocsPaVO.documento.FileDocumento fdNew = null;
                if (sch.documenti != null && sch.documenti[0] != null &&
                    Int32.Parse(((DocsPaVO.documento.FileRequest)sch.documenti[0]).fileSize) > 0)
                {
                    try
                    {
                        fd = BusinessLogic.Documenti.FileManager.getFileFirmato((DocsPaVO.documento.FileRequest)sch.documenti[0], infoUtente, false);
                        if (fd == null)
                            throw new Exception("Errore nel reperimento del file principale.");

                        //copio in un nuovo filerequest perchè putfile lo vuole senza
                        fdNew = new DocsPaVO.documento.FileDocumento();
                        fdNew.content = fd.content;
                        fdNew.length = fd.length;
                        fdNew.name = fd.name;
                        fdNew.fullName = fd.fullName;
                        fdNew.contentType = fd.contentType;
                        DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest)sd.documenti[0];

                        if (!BusinessLogic.Documenti.FileManager.putFile(ref fr, fdNew, infoUtente, out err))
                            throw new Exception(err);
                        else
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOPUTFILE", fr.docNumber, string.Format("{0} {1}", "Acquisito documento N.ro:", fr.docNumber), DocsPaVO.Logger.CodAzione.Esito.OK);

                        logger.Debug("file principale inserito");
                    }
                    catch (Exception ex)
                    {
                        err = "Errore nel reperimento del file principale : " + ex.Message;
                        if (sd != null && sd.systemId != null && sd.systemId != "")
                        {
                            //se il putFile va in errore devo rimuovere il profile (predisposto appena inserito)
                            BusinessLogic.Documenti.DocManager.ExecRimuoviSchedaMethod(infoUtente, sd);
                            logger.Debug("Eseguita rimozione profilo");
                        }
                        logger.Error(err);
                        throw ex;
                        //se non si riesce a prener eil file per problemi vari continuo comunque per arrivare a eseguiTrasmissione
                    }
                }

                //ricerca degli allegati
                //				logger.addMessage("Inserimento degli allegati");
                logger.Debug("Inserimento degli allegati");
                DocsPaDB.Query_DocsPAWS.Documenti docWs = new DocsPaDB.Query_DocsPAWS.Documenti();
                for (int i = 0; sch.allegati != null && i < sch.allegati.Count; i++)
                {
                    //estrazione dati dell'allegato
                    DocsPaVO.documento.Allegato documentoAllegato = (DocsPaVO.documento.Allegato)sch.allegati[i];

                    //Salto il tipo allegato Derivato essendo lui estratto dal docprinc o da un altro allegato
                    if (docWs.GetTipologiaAllegato(documentoAllegato.versionId) == "D")
                        continue;

                    filepath = documentoAllegato.docServerLoc + documentoAllegato.path;
                    string nomeAllegato = documentoAllegato.fileName;
                    string numPagine = documentoAllegato.numeroPagine.ToString();
                    string titoloDoc = documentoAllegato.descrizione;
                    //					logger.addMessage("Inserimento allegato "+nomeAllegato);
                    logger.Debug("Inserimento allegato " + nomeAllegato);

                    DocsPaVO.documento.Allegato all = new DocsPaVO.documento.Allegato();
                    //					logger.addMessage("docnumber="+sd.docNumber);
                    logger.Debug("docnumber=" + sd.docNumber);

                    all.docNumber = sd.docNumber;
                    //all.applicazione=getApp(nomeAllegato,logger);
                    all.fileName = getFileName(nomeAllegato);
                    all.version = "0";

                    all.descrizione = "allegato " + i;
                    if (!String.IsNullOrEmpty(all.fileName))
                        all.descrizione = all.fileName;
                    
                    //numero pagine
                    if (numPagine != null && !numPagine.Trim().Equals(""))
                    {
                        all.numeroPagine = Int32.Parse(numPagine);
                    }
                    //descrizione allegato
                    if (titoloDoc != null && !titoloDoc.Trim().Equals(""))
                    {
                        all.descrizione = titoloDoc;
                    }

                    DocsPaVO.documento.Allegato res = null;
                    try
                    {
                        res = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, all);
                        if (res != null)
                        {
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOAGGIUNGIALLEGATO", all.docNumber, string.Format("{0}{1}{2}{3}", "Aggiunto al N.ro Doc.: ", all.docNumber, " il N.ro Allegato: ", res.versionLabel), DocsPaVO.Logger.CodAzione.Esito.OK);
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCNEWALLEGATO", all.docNumber, string.Format("{0}{1}{2}{3}", "Aggiunto al N.ro Doc.: ", all.docNumber, " il N.ro Allegato: ", res.versionLabel), DocsPaVO.Logger.CodAzione.Esito.OK);
                        }
                    }
                    catch (Exception e)
                    {
                        err = "errore nel metodo aggiungiAllegato per l'allegato n. " + Convert.ToString(i + 1);
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOAGGIUNGIALLEGATO", all.docNumber, string.Format("{0}{1}", "Errore in inserimento allegato al N.ro Doc.: ", all.docNumber), DocsPaVO.Logger.CodAzione.Esito.KO);
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCNEWALLEGATO", all.docNumber, string.Format("{0}{1}", "Errore in inserimento allegato al N.ro Doc.: ", all.docNumber), DocsPaVO.Logger.CodAzione.Esito.KO);
                        logger.Error(err);
                        throw e;
                    }


                    #region Codice Commentato
                    //					logger.addMessage("Allegato id="+all.versionId);
                    //					logger.addMessage("Allegato version label="+all.versionLabel);
                    //					logger.addMessage("Inserimento nel filesystem");
                    #endregion

                    logger.Debug("Allegato id=" + all.versionId);
                    logger.Debug("Allegato version label=" + all.versionLabel);
                    logger.Debug("Inserimento nel filesystem");

                    DocsPaVO.documento.FileDocumento fdAllNew = new DocsPaVO.documento.FileDocumento();
                    DocsPaVO.documento.FileDocumento fdAll = null;
                    if (Int32.Parse(documentoAllegato.fileSize) > 0)
                    {
                        try
                        {
                            fdAll = BusinessLogic.Documenti.FileManager.getFileFirmato((DocsPaVO.documento.FileRequest)sch.allegati[i], infoUtente, false);
                            if (fdAll == null)
                                throw new Exception("Errore nel reperimento dell'allegato numero" + i.ToString());
                            fdAllNew.content = fdAll.content;
                            fdAllNew.length = fdAll.length;
                            fdAllNew.name = fdAll.name;
                            fdAllNew.fullName = fdAll.fullName;
                            if (!String.IsNullOrEmpty (fdAll.nomeOriginale))
                                fdAllNew.nomeOriginale = fdAll.nomeOriginale;
                            fdAllNew.contentType = fdAll.contentType;
                            DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest)all;
                            if (!BusinessLogic.Documenti.FileManager.putFile(ref fr, fdAllNew, infoUtente, out err))
                                throw new Exception(err);
                            else
                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOPUTFILE", fr.docNumber, string.Format("{0} {1}", "Acquisito documento N.ro:", fr.docNumber), DocsPaVO.Logger.CodAzione.Esito.OK);

                            logger.Debug("Allegato " + i + " inserito");

                        }
                        catch (Exception ex)
                        {
                            err = "Errore nel reperimento dell'allegato numero " + i.ToString() + " : " + ex.Message;

                            if (sd != null && sd.systemId != null && sd.systemId != "")
                            {
                                //se il putFile va in errore devo rimuovere il profile (predisposto appena inserito)
                                BusinessLogic.Documenti.DocManager.ExecRimuoviSchedaMethod(infoUtente, sd);

                                logger.Debug("Eseguita rimozione profilo");
                            }
                            logger.Error(err);//se non si riesce a prener eil file per problemi vari continuo comunque per arrivare a eseguiTrasmissione
                            throw ex;
                        }
                    }
                }

                //***********************************************************************************************************//
                /*  MODIFICA Giordano Iacozzilli Data: 27/04/2012                                                            */
                //***********************************************************************************************************// 
                //Modifca relativa all'invio in allegato diun File XML in caso di Interop Semplificata //
                interoperabilità.InteroperabilitaSendXmlInAttach _intXsendXML = new interoperabilità.InteroperabilitaSendXmlInAttach();
                bool _esito = _intXsendXML.ManageAttachXML(sch, sd, infoUtenteFisico.idGruppo, infoUtente);
                // la bool esito al momento non la uso, devo capire se metterla o meno nel log applicativo.
                //***********************************************************************************************************//
                /*  FINE: MODIFICA Giordano Iacozzilli Data: 27/04/2012                                                      */
                //***********************************************************************************************************// 


                //nuova gestione interoperabilità AUTOMATICA

                if ((reg.autoInterop != null && !reg.autoInterop.Equals("2"))
                    || (reg.autoInterop != null && reg.autoInterop.Equals("2") && !statoReg.Equals("V")))
                {
                    //TRASMISSIONE PER INTEROPERABILITA
                    logger.Debug("Esegui trasmissione...");
                    try
                    {
                        //eseguiTrasmissione(infoUtente.idPeople,serverName, sd,infoDestinatari,reg,ruolo);
                        // correzione furnari eseguiTrasmissione(infoUtente.idPeople, serverName, sd, "INTEROPERABILITA", reg, ruolo, infoUtente.dst, mailAddress, infoUtente);

                        // S. Furnari - 16/01/2013 - Sviluppo trasmissione documento solo a ruoli nella UO destinataria della spedizione e non a tutta la AOO
                        //eseguiTrasmissione(infoUtente.idPeople, serverName, sd, "INTEROPERABILITA", reg, ruolo, infoUtente.dst, mailAddress);
                        eseguiTrasmissione(infoUtente.idPeople, serverName, sd, "INTEROPERABILITA", reg, ruolo, infoUtente.dst, mailAddress, infoUtente,recipient);


                        if (!checkExecTrasm(sd.systemId, "INTEROPERABILITA"))
                        {
                            //codint1 = true;
                            //err = "CODINTEROP1 Trasmissione per INTEROPERABILITA del documento con id:" + sd.docNumber + " non effettuata. Il documento può essere trovato tra i predisposti.";
                            //throw new Exception("(CODINTEROP1) Trasmissione per INTEROPERABILITA del documento con id:" + sd.docNumber + "non effettuata!");
                            // S. Furnari - 09/01/2013 - Quando viene catturata l'eccezione scatenata di seguito, viene cancellata la scheda
                            // documento, quindi non la si può trovare con una ricerca documenti predisposti.
                            //err = "CODINTEROP1 Trasmissione per INTEROPERABILITA del documento con id:" + sd.docNumber + " non effettuata. Il documento può essere trovato tra i predisposti.";
                            err = "CODINTEROP1 Trasmissione del documento non riuscita.";
                            throw new Exception("(CODINTEROP1) Trasmissione per INTEROPERABILITA del documento con id:" + sd.docNumber + "non effettuata!");
     
                        }
                    }
                    catch (Exception ex)
                    {
                        if (sd != null && sd.systemId != null && sd.systemId != "")
                        {
                            BusinessLogic.Documenti.DocManager.ExecRimuoviSchedaMethod(infoUtente, sd);
                            logger.Debug("Eseguita rimozione profilo");
                        }
                        logger.Error(err);
                        throw ex;
                    }
                }
                //luluciani: viene fatto alla fine, per  poter gestire il rollback manuale.

                if ((reg.autoInterop != null && reg.autoInterop.Equals("2")) &&
                    (statoReg == "V"))
                {
                    //registro AUTOMATICO: creo direttamente un protocollo sulla AOO destinataria
                    logger.Debug("Protocollazione documento in caso di registro AUTOMATICO");
                    //dafault OK
                    DocsPaVO.documento.ResultProtocollazione resultProtocollazione = DocsPaVO.documento.ResultProtocollazione.OK;
                    //

                    sd = BusinessLogic.Documenti.ProtoManager.protocolla(sd, ruolo, infoUtente, out resultProtocollazione);
                    string VarDescOggetto = string.Empty;
                    if (sd != null)
                    {
                        if (sd.protocollo != null)
                            VarDescOggetto = string.Format("{0}{1} / {2}{3}", "N.ro Doc.: ", sd.docNumber, "Segnatura: ", sd.protocollo.segnatura);
                        else
                            VarDescOggetto = string.Format("{0}{1}", "N.ro Doc.: ", sd.docNumber);

                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOPROTOCOLLA", sd.systemId, VarDescOggetto, DocsPaVO.Logger.CodAzione.Esito.OK);
                    }

                    // Popolamento oggetto DIA (Dati Interop Automatica)
                    dia = new DocsPaVO.Interoperabilita.DatiInteropAutomatica();
                    //esito protocollazione su registro automatico
                    dia.esitoProtocollazione = resultProtocollazione;
                    //ruolo e infoUtente gestore del registro
                    dia.ruolo = ruolo;
                    dia.infoUtente = infoUtente;

                    //systemId protocollo in arrivo appena creato su registro automatico
                    if (sd != null && sd.systemId != null)
                        dia.schedaDoc = sd;

                    //registro automatico
                    dia.registro = reg;
                }

                return true;
            }
            catch (Exception ex)
            {
                if (sd != null && sd.docNumber != null)
                {
                    //err="errore "+err+" Docnumber predisposto: "+sd.docNumber+" "+ex.Message.ToString();
                    err = "errore " + err + "  " + ex.Message.ToString();
                }
                else
                    err = "errore " + err + " " + ex.Message.ToString();
                logger.Error(err);

                return false;
            }
        }
       
        public static bool checkExecTrasm(string sysid, string tipoTrasm)
        {
            return BusinessLogic.Trasmissioni.ExecTrasmManager.checkExecTrasm(sysid, tipoTrasm);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="filepath"></param>
        /// <param name="reg"></param>
        /// <param name="infoUtente"></param>
        /// <param name="ruolo"></param>
        /// <param name="mailId"></param>
        /// <param name="mc"></param>
        /// <param name="logger"></param>
        /// <param name="isPec">valorizzato a 1 se il documento proviene da una mail certificata 0 se se il documento proviene da una mail normale
        /// </param>
        /// parametro Opzionale eccSegnatura
        /// <returns></returns>
        public static bool eseguiSenzaSegnatura(string serverName, string filepath, DocsPaVO.utente.Registro reg,
            DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, string mailId, bool fatturaElDaPEC,
            Interoperabilità.CMMsg mc, string isPec, out string err, out string docnumber, string nomeMail, string dataRicezione, string mailAddress, bool eccSegnatura = false)
        {
            System.IO.FileStream fs = null;
            bool codint1 = false;
            System.IO.FileStream fsAll = null;
            bool daAggiornareUffRef = false;
            DocsPaVO.documento.SchedaDocumento sd = null;
            err = string.Empty;
            docnumber = string.Empty;
            string messaggioRC = string.Empty;
            try
            {
                string docPrincipaleName = "body.html";

                //CONTROLLI NEL FORMATO DEI FILES
                //Documento principale
                if (getApp(docPrincipaleName) == null)
                {
                    logger.Debug("La mail viene sospesa. Il documento principale ha un formato non gestito");
                    err = "La mail viene sospesa. Il documento principale ha un formato non gestito";
                    if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
                    {
                        logger.Debug("Sospensione eseguita");
                    }
                    else
                    {
                        logger.Debug("Sospensione non eseguita");
                    };
                    return false;
                }
                //Attachment
                for (int ind = 0; ind < mc.attachments.Count; ind++)
                {

                    if (getApp(mc.attachments[ind].name) == null)
                    {
                        //						logger.addMessage("La mail viene sospesa. Il documento "+mc.Attachments[ind].Name+" ha un formato non gestito"); 
                        logger.Debug("La mail viene sospesa. Il documento " + mc.attachments[ind].name + " ha un formato non gestito");
                        err = "La mail viene sospesa. Il documento " + mc.attachments[ind].name + " ha un formato non gestito";
                        if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
                        {
                            //							logger.addMessage("Sospensione eseguita");
                            logger.Debug("Sospensione eseguita");
                        }
                        else
                        {
                            //							logger.addMessage("Sospensione non eseguita");
                            logger.Debug("Sospensione non eseguita");
                        }

                        return false;
                    }
                }

                //mittente 
                //DocsPaVO.utente.UnitaOrganizzativa mittente = null; //= new DocsPaVO.utente.UnitaOrganizzativa();
                DocsPaVO.utente.Corrispondente mittente = null;//corrispondente = null;
                //corrispondente 
                logger.Debug("mc.from = " + mc.from);
                DocsPaVO.utente.Corrispondente mittente_appoggio = null;
                // Il mittente va ricercato in tutti i registri associati al ruolo
                Registro[] registriRuolo = (Registro[])(Utenti.RegistriManager.getListaRegistriRfRuolo(infoUtente.idCorrGlobali, null, null).ToArray(typeof(Registro)));
                foreach (var registro in registriRuolo)
                {
                    mittente = BusinessLogic.Utenti.UserManager.getCorrispondenteByEmail(mc.from, infoUtente, registro, out messaggioRC);
                    if(mittente != null)
                    {
                        if (!mittente.codiceRubrica.Contains("@"))
                            break;
                        else
                            mittente_appoggio = mittente;
                    }
                }
                if (mittente == null)
                    mittente = mittente_appoggio;
                string checkInteroperante = "";
                string checkSameMail = "";
                
                if (mittente == null)
                {
                    mittente = new DocsPaVO.utente.UnitaOrganizzativa();
                    mittente.descrizione = mc.from;
                    mittente.tipoIE = "E";
                    mittente.tipoCorrispondente = "S";
                    mittente.codiceRubrica = mc.from;
                    mittente.codiceAOO = reg.codice;
                    //Modifica 20.7.12, Ferlito: la seguente linea di codice è stata messa a commento in quanto
                    //il mittente della spedizione veniva inserito in rubrica con il cod_amm del destinatario 
                    //mittente.codiceAmm = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(reg.idAmministrazione).Codice;
                    mittente.email = mc.from;
                    string idMezzoSpedizione = BusinessLogic.Documenti.InfoDocManager.getIdMezzoSpedizioneByDesc("MAIL");
                    mittente.canalePref = BusinessLogic.Documenti.InfoDocManager.getCanaleBySystemId(idMezzoSpedizione);
                    mittente.cognome = mc.from;
                    mittente.idAmministrazione = reg.idAmministrazione;
                    mittente.idRegistro = reg.systemId;
                    ////Check Mittente Interoperante
                    //checkInteroperante = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "CHECK_MITT_INTEROPERANTE");
                    //checkSameMail = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "CHECK_MAILBOX_INTEROPERANTE");
                    //if (checkInteroperante.Equals("1") || checkSameMail.Equals("1"))
                    //{
                    //    mittente.note = "#0";
                    //}
                    
                    //                mittente = BusinessLogic.Utenti.addressBookManager.insertCorrispondente(mittente, null);
                }
                //logger.addMessage("Inserimento documento principale");
                logger.Debug("Inserimento documento principale");
               
                sd = new DocsPaVO.documento.SchedaDocumento();
                sd.appId = getApp(docPrincipaleName).application;
                sd.idPeople = infoUtente.idPeople;
                sd.userId = infoUtente.userId;
                DocsPaVO.documento.Oggetto ogg = new DocsPaVO.documento.Oggetto();
                ogg.descrizione = mc.subject;
                ogg = (DocsPaVO.documento.Oggetto)DocsPaUtils.Functions.Functions.XML_Serialization_Deserialization_By_Encode(ogg, typeof(DocsPaVO.documento.Oggetto), null, System.Text.Encoding.UTF8);
                sd.oggetto = ogg;
                sd.tipoProto = "G";
                sd.typeId = "MAIL";
                sd.mezzoSpedizione = BusinessLogic.Documenti.InfoDocManager.getIdMezzoSpedizioneByDesc("MAIL");
                sd.descMezzoSpedizione = "MAIL";
                // Per gestione pendenti tramite PEC
                if (InteroperabilitaUtils.MantieniMailRicevutePendenti(reg.systemId, mailAddress))
                {
                    sd.privato = "1";
                }
                logger.Debug("Salvataggio doc...");
                try
                {
                    sd = BusinessLogic.Documenti.DocSave.addDocGrigia(sd, infoUtente, ruolo);
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOADDDOCGRIGIA", sd.systemId, string.Format("{0} {1}", "N.ro Doc.: ", sd.docNumber), DocsPaVO.Logger.CodAzione.Esito.OK);
                }
                catch (Exception ex)
                {
                    err = "Errore durante la creazione del documento.";
                    logger.Error("Errore durante la creazione del documento. " + ex.Message);
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOADDDOCGRIGIA", sd.systemId, string.Format("{0} {1}", "N.ro Doc.: ", sd.docNumber), DocsPaVO.Logger.CodAzione.Esito.KO);
                    throw new Exception();

                }
                //salvo l'rf che ho utilizzato per interrogare la casella istituzionale
                if (BusinessLogic.interoperabilita.InteroperabilitaManager.InsertAssDocAddress(sd.systemId, reg.systemId, mailAddress))
                    logger.Debug("associazione documento mail address correttamente eseguita in DPA_ASS_DOC_MAIL_INTEROP");
                else
                    logger.Debug("errore nell'associazione documento mail address in DPA_ASS_DOC_MAIL_INTEROP");

                sd.predisponiProtocollazione = true;


                //28/04/08
                //sd.registro = reg;

                sd.registro = CaricaRegistroInScheda(reg);

                sd.tipoProto = "A";
                sd.typeId = "MAIL";
                //Andrea De Marco - Gestione Eccezioni PEC - Se si è verificata un'eccezione in Segnatura.xml imposto sd.interop = E
                //Per ripristino commentare De Marco e decommentare il codice sottostante
                //il parametro eccSegnatura è opzionale
                if(eccSegnatura)
                {
                    sd.interop = "E";
                    sd.predisponiProtocollazione = true;
                }
                else
                {
                    sd.interop = "P";
                }
                //End Andrea De Marco
                //sd.interop = "P";

                DocsPaVO.documento.ProtocolloEntrata protEntr = new DocsPaVO.documento.ProtocolloEntrata();
                protEntr.mittente = mittente;



                //Controllo se nell'headers esiste la chiave "utenteDocspa"
                //In questo caso risolvo tramite email il mittete e lo imposto come mittente intermedio
                //del protocollo in ingresso
                System.Collections.Hashtable headers = mc.headers;
                if (headers["utenteDocspa"] != null)
                {
                    logger.Debug("Risoluzione mittente intermedio ...");
                    System.Collections.IDictionaryEnumerator enumerator = headers.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Key.ToString() == "utenteDocspa")
                        {
                            DocsPaVO.utente.Corrispondente mittenteIntermedio = BusinessLogic.Utenti.UserManager.getUtenteByEmail(reg.idAmministrazione, enumerator.Value.ToString());
                            if (mittenteIntermedio != null && mittenteIntermedio.systemId != null)
                                protEntr.mittenteIntermedio = mittenteIntermedio;
                        }
                    }
                    headers.Remove("utenteDocspa");
                    logger.Debug("Risoluzione mittente intermedio eseguita ...");
                }



                sd.protocollo = protEntr;

                sd.protocollo.invioConferma = "0";


                //				logger.addMessage("Salvataggio doc...");


                //Controllo che il file allegato sia unico e sia una form pdf da processare
                bool processForm = false;

                //LULUCIANI+ FERRO X unitn
                if (mc.attachments.Count == 1)
                {
                    //mc.attachments[0].name = mc.attachments[0].name.ToUpper().Replace(".P DF", ".PDF");
                    string attachmentName = mc.attachments[0].name;
                    if (attachmentName.ToUpper().EndsWith(".P DF"))
                    {   // Zanotti dice che riceve file con estensioni errate, 
                        // ho migliorato la gestione del file lasciando invariato il nomefile,e unendo solo l'estensione
                        string ext = Path.GetExtension(attachmentName);
                        mc.attachments[0].name = attachmentName.Replace(ext, String.Empty) + ext.Replace(" ", String.Empty);
                     }
                }
                //Fine 
                if (mc.attachments.Count == 1 && System.IO.Path.GetExtension(mc.attachments[0].name).ToUpper() == ".PDF")
                {
                    logger.Debug("Recupero il fileDocumento");
                    //Recupero il fileDocumento
                    DocsPaVO.documento.FileDocumento fdAll = new DocsPaVO.documento.FileDocumento();
                    fsAll = new System.IO.FileStream(filepath + "\\" + mc.attachments[0].name, System.IO.FileMode.Open);
                    byte[] bufferAll = new byte[fsAll.Length];
                    fsAll.Read(bufferAll, 0, (int)fsAll.Length);
                    fdAll.content = bufferAll;
                    fdAll.length = bufferAll.Length;
                    fdAll.name = mc.attachments[0].name;
                    fsAll.Close();

                    logger.Debug("Inizio processo form pdf");
                    //Controllo che il file pdf sia una form che sappiamo processare
                    DocsPaVO.LiveCycle.ProcessFormInput processFormInput = new DocsPaVO.LiveCycle.ProcessFormInput();
                    processFormInput.schedaDocumentoInput = sd;
                    processFormInput.fileDocumentoInput = fdAll;
                    DocsPaVO.LiveCycle.ProcessFormOutput processFormOutput = BusinessLogic.LiveCycle.LiveCycle.processFormPdf(infoUtente, processFormInput);

                    if (processFormOutput != null && processFormOutput.schedaDocumentoOutput != null)
                    {
                        sd = processFormOutput.schedaDocumentoOutput;
                        docPrincipaleName = mc.attachments[0].name;
                        mc.attachments.Clear();
                        processForm = true;
                    }
                    logger.Debug("Fine processo form pdf");
                }

                //salvo mittente dopo cambio descrizione se c'è modulo PDF.
                mittente = ((DocsPaVO.documento.ProtocolloEntrata)sd.protocollo).mittente;

                try
                {
                    string[] splitted = messaggioRC.Split('#');
                    if (splitted.Length > 1)
                    {
                        messaggioRC = splitted[0];

                    }
                    if (mittente != null
                        && string.IsNullOrEmpty(mittente.systemId)
                        && !messaggioRC.Equals("RC"))
                    {
                        //inserico il corrispondente in rubrica comune
                        mittente = BusinessLogic.Utenti.addressBookManager.insertCorrispondente(mittente, null);
                        //inserisc ola mail associata al corrispondente esterno in DPA_MAIL_CORR_ESTERNI
                        List<DocsPaVO.utente.MailCorrispondente> casella = new List<DocsPaVO.utente.MailCorrispondente>();
                        casella.Add(new DocsPaVO.utente.MailCorrispondente()
                        {
                            Email = mittente.email,
                            Note = "",
                            Principale = "1"
                        });
                        BusinessLogic.Utenti.addressBookManager.InsertMailCorrispondente(casella, mittente.systemId);
                    }
                    //else
                    //{
                    //    if (splitted.Length > 1)
                    //    {
                    //        if (splitted[1].Equals("2"))
                    //        {
                    //            DocsPaDB.Query_DocsPAWS.Utenti users = new DocsPaDB.Query_DocsPAWS.Utenti();
                    //            users.resetCorrVarInsertIterop(mittente.systemId, "2");
                    //        }
                    //        else if (splitted[1].Equals("1"))
                    //        {
                    //            DocsPaDB.Query_DocsPAWS.Utenti users = new DocsPaDB.Query_DocsPAWS.Utenti();
                    //            users.resetCorrVarInsertIterop(mittente.systemId, "1");
                    //        }
                    //    }
                    //}
                }
                catch (Exception errmes)
                {
                    logger.Error("errore durante l'inserimento di un corrispondente. il corrispondente esisite già. - errore:" + errmes.Message + " stacktrace: " + errmes.StackTrace);
                    //err = "(CODINTEROP3)";
                }
                // luluciani  19/09/2012 è sbagliato, perchè il mezzo di spedizione è la MAIL , siamo in eseguisenzasegnatura!! E' arrivata una MAIL!!!
                //if (mittente != null && mittente.canalePref != null && !string.IsNullOrEmpty(mittente.canalePref.systemId))
                //{
                //    sd.mezzoSpedizione = mittente.canalePref.systemId;
                //    sd.descMezzoSpedizione = mittente.canalePref.descrizione;
                //}
                // fine  luluciani 
                try
                {
                    sd.documento_da_pec = isPec;
                    ((DocsPaVO.documento.Documento)sd.documenti[0]).dataArrivo = dataRicezione;
                    sd = BusinessLogic.Documenti.DocSave.save(infoUtente, sd, false, out daAggiornareUffRef, ruolo);
                    if (sd.tipoProto != "G")
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOSAVEDOCUMENTO", sd.systemId, string.Format("{0} {1}", "Aggiornamento Protocollo Numero ", sd.protocollo.segnatura), DocsPaVO.Logger.CodAzione.Esito.OK);
                    else
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOSAVEDOCUMENTO", sd.systemId, string.Format("{0} {1}", "Aggiornamento Documento Numero ", sd.docNumber), DocsPaVO.Logger.CodAzione.Esito.OK);
                }
                catch (Exception ex)
                {
                    err = "Errore durante il salvataggio del documento." + ex.Message;
                    logger.Error("Errore durante il salvataggio del documento. " + ex.Message);

                    throw new Exception(err);

                }

                logger.Debug("Salvataggio eseguito");

                //Controllo che sia una form pdf processata
                if (processForm)
                {
                    processForm = false;
                    logger.Debug("Trasmissione con il modello associato al template");
                    //Eventualmente effetuo la trasmissione usando il modello associato al template
                    if (sd != null && sd.template != null && sd.template.CODICE_MODELLO_TRASM != null)
                    {
                        DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione trasmModel = Trasmissioni.ModelliTrasmissioni.getModelloByID(infoUtente.idAmministrazione, sd.template.CODICE_MODELLO_TRASM.Replace("MT_", ""));
                        BusinessLogic.Trasmissioni.TrasmManager.TransmissionExecuteDocTransmFromModelCode(infoUtente, serverName, sd, sd.template.CODICE_MODELLO_TRASM, ruolo, out trasmModel);
                    }
                    logger.Debug("Classifica con il codice associato al template");
                    //Eventualmente effetuo la classifica usando il codice di classifica associato al template
                    if (sd != null && sd.template != null && sd.template.CODICE_CLASSIFICA != null)
                    {
                        if (reg != null && reg.chaRF == "1" && reg.idAOOCollegata != null)
                        {
                            DocsPaVO.utente.Registro registro = Utenti.RegistriManager.getRegistro(reg.idAOOCollegata);
                            System.Collections.ArrayList fascicoli = Fascicoli.FascicoloManager.getListaFascicoliDaCodice(infoUtente, sd.template.CODICE_CLASSIFICA, registro, false, false, "I");
                            if (fascicoli != null && fascicoli.Count == 1)
                            {
                                string msg;
                                bool result = BusinessLogic.Fascicoli.FascicoloManager.addDocFascicolo(infoUtente, sd.systemId, ((DocsPaVO.fascicolazione.Fascicolo)fascicoli[0]).systemID, true, out msg);
                                if (result)
                                {
                                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "FASCICOLOADDDOC", ((DocsPaVO.fascicolazione.Fascicolo)fascicoli[0]).systemID, "Inserimento doc " + sd.systemId + " in fascicolo: " + ((DocsPaVO.fascicolazione.Fascicolo)fascicoli[0]).codice, DocsPaVO.Logger.CodAzione.Esito.OK);
                                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCADDINFASC", sd.systemId, "Inserimento doc " + sd.systemId + " in fascicolo: " + ((DocsPaVO.fascicolazione.Fascicolo)fascicoli[0]).codice, DocsPaVO.Logger.CodAzione.Esito.OK);
                                }
                            }
                        }
                        else
                        {
                            System.Collections.ArrayList fascicoli = Fascicoli.FascicoloManager.getListaFascicoliDaCodice(infoUtente, sd.template.CODICE_CLASSIFICA, reg, false, false, "I");
                            if (fascicoli != null && fascicoli.Count == 1)
                            {
                                string msg;
                                bool result2 = BusinessLogic.Fascicoli.FascicoloManager.addDocFascicolo(infoUtente, sd.systemId, ((DocsPaVO.fascicolazione.Fascicolo)fascicoli[0]).systemID, true, out msg);
                                if (result2)
                                {
                                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "FASCICOLOADDDOC", ((DocsPaVO.fascicolazione.Fascicolo)fascicoli[0]).systemID, "Inserimento doc " + sd.systemId + " in fascicolo: " + ((DocsPaVO.fascicolazione.Fascicolo)fascicoli[0]).codice, DocsPaVO.Logger.CodAzione.Esito.OK);
                                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCADDINFASC", sd.systemId, "Inserimento doc " + sd.systemId + " in fascicolo: " + ((DocsPaVO.fascicolazione.Fascicolo)fascicoli[0]).codice, DocsPaVO.Logger.CodAzione.Esito.OK);
                                }
                            }
                        }
                    }
                }
                // luluciani  19/09/2012 è sbagliato, perchè il mezzo di spedizione è la MAIL , siamo in eseguisenzasegnatura!! E' arrivata una MAIL!!!
                //if (mittente != null && mittente.canalePref != null && !string.IsNullOrEmpty(mittente.canalePref.systemId))
                //{
                //    sd.mezzoSpedizione = mittente.canalePref.systemId;
                //    sd.descMezzoSpedizione = mittente.canalePref.descrizione;

                //}
                // fine  luluciani 


                logger.Debug("Salvataggio eseguito");

                //Controllo che sia una form pdf processata
                if (processForm)
                {
                    processForm = false;
                    logger.Debug("Trasmissione con il modello associato al template");
                    //Eventualmente effetuo la trasmissione usando il modello associato al template
                    if (sd != null && sd.template != null && sd.template.CODICE_MODELLO_TRASM != null)
                    {
                        DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione trasmModel = Trasmissioni.ModelliTrasmissioni.getModelloByID(infoUtente.idAmministrazione, sd.template.CODICE_MODELLO_TRASM.Replace("MT_", ""));
                        if(BusinessLogic.Trasmissioni.TrasmManager.TransmissionExecuteDocTransmFromModelCode(infoUtente, serverName, sd, sd.template.CODICE_MODELLO_TRASM, ruolo, out trasmModel))
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, ruolo.idGruppo, infoUtente.idAmministrazione, "DOCUMENTOTRASMESSO", sd.docNumber, "Effettuata trasmissione del documento " + sd.docNumber + " con modello " + sd.template.CODICE_MODELLO_TRASM, DocsPaVO.Logger.CodAzione.Esito.OK, null);
                    }
                    logger.Debug("Classifica con il codice associato al template");
                    //Eventualmente effetuo la classifica usando il codice di classifica associato al template
                    if (sd != null && sd.template != null && sd.template.CODICE_CLASSIFICA != null)
                    {
                        if (reg != null && reg.chaRF == "1" && reg.idAOOCollegata != null)
                        {
                            DocsPaVO.utente.Registro registro = Utenti.RegistriManager.getRegistro(reg.idAOOCollegata);
                            System.Collections.ArrayList fascicoli = Fascicoli.FascicoloManager.getListaFascicoliDaCodice(infoUtente, sd.template.CODICE_CLASSIFICA, registro, false, false, "I");
                            if (fascicoli != null && fascicoli.Count == 1)
                            {
                                string msg;
                                bool result3 = BusinessLogic.Fascicoli.FascicoloManager.addDocFascicolo(infoUtente, sd.systemId, ((DocsPaVO.fascicolazione.Fascicolo)fascicoli[0]).systemID, true, out msg);
                                if (result3)
                                {
                                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "FASCICOLOADDDOC", ((DocsPaVO.fascicolazione.Fascicolo)fascicoli[0]).systemID, "Inserimento doc " + sd.systemId + " in fascicolo: " + ((DocsPaVO.fascicolazione.Fascicolo)fascicoli[0]).codice, DocsPaVO.Logger.CodAzione.Esito.OK);
                                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCADDINFASC", sd.systemId, "Inserimento doc " + sd.systemId + " in fascicolo: " + ((DocsPaVO.fascicolazione.Fascicolo)fascicoli[0]).codice, DocsPaVO.Logger.CodAzione.Esito.OK);
                                }
                            }
                        }
                        else
                        {
                            System.Collections.ArrayList fascicoli = Fascicoli.FascicoloManager.getListaFascicoliDaCodice(infoUtente, sd.template.CODICE_CLASSIFICA, reg, false, false, "I");
                            if (fascicoli != null && fascicoli.Count == 1)
                            {
                                string msg;
                                bool result4 = BusinessLogic.Fascicoli.FascicoloManager.addDocFascicolo(infoUtente, sd.systemId, ((DocsPaVO.fascicolazione.Fascicolo)fascicoli[0]).systemID, true, out msg);
                                if (result4)
                                {
                                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "FASCICOLOADDDOC", ((DocsPaVO.fascicolazione.Fascicolo)fascicoli[0]).systemID, "Inserimento doc " + sd.systemId + " in fascicolo: " + ((DocsPaVO.fascicolazione.Fascicolo)fascicoli[0]).codice, DocsPaVO.Logger.CodAzione.Esito.OK);
                                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCADDINFASC", sd.systemId, "Inserimento doc " + sd.systemId + " in fascicolo: " + ((DocsPaVO.fascicolazione.Fascicolo)fascicoli[0]).codice, DocsPaVO.Logger.CodAzione.Esito.OK);
                                }
                            }
                        }
                    }
                }
                // luluciani  19/09/2012 è sbagliato, perchè il mezzo di spedizione è la MAIL , siamo in eseguisenzasegnatura!! E' arrivata una MAIL!!!
                //if (mittente != null && mittente.canalePref != null && !string.IsNullOrEmpty(mittente.canalePref.systemId))
                //{
                //    sd.mezzoSpedizione = mittente.canalePref.systemId;
                //    sd.descMezzoSpedizione = mittente.canalePref.descrizione;
                //}
                // fine luluciani
                if(!string.IsNullOrEmpty(sd.interop) && sd.interop.Equals("E"))
                    sd.predisponiProtocollazione = true;
                else
                    sd.predisponiProtocollazione = false;
                
                //sd = BusinessLogic.Documenti.DocSave.save(infoUtente, sd, false, out daAggiornareUffRef, ruolo);

                DocsPaVO.utente.InfoUtente info = new DocsPaVO.utente.InfoUtente();
                info.idAmministrazione = sd.registro.idAmministrazione;
                //if (mittente != null && mittente.canalePref != null && !string.IsNullOrEmpty(mittente.canalePref.systemId))
                //{
                //    BusinessLogic.Documenti.ProtoManager.collegaMezzoSpedizioneDocumento(info, mittente.canalePref.systemId, sd.systemId);
                //}
                //logger.Debug("eseguito insert mezzo sped");

                if (sd != null && sd.mezzoSpedizione != null && !string.IsNullOrEmpty(sd.mezzoSpedizione))
                {
                    BusinessLogic.Documenti.ProtoManager.collegaMezzoSpedizioneDocumento(info, sd.mezzoSpedizione, sd.systemId);
                }
                logger.Debug("eseguito insert mezzo sped");


                DocsPaVO.documento.FileDocumento fd = new DocsPaVO.documento.FileDocumento();
                fs = new System.IO.FileStream(filepath + "\\" + docPrincipaleName, System.IO.FileMode.Open);
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
                fs.Close();
                fd.content = buffer;
                fd.length = buffer.Length;
                fd.name = docPrincipaleName;

                DocsPaVO.documento.FileRequest frSch = (DocsPaVO.documento.FileRequest)sd.documenti[0];

                // inserito per permettere la memorizzazione corretta del path nella components 
                // in caso di ricezione per interoperabilità e PitreDualFileWritingMode=false
                frSch.fileName = docPrincipaleName;

                if (!BusinessLogic.Documenti.FileManager.putFile(ref frSch, fd, infoUtente, out err))
                    throw new Exception(err);
                //				logger.addMessage("Documento principale inserito");
                else
                {
                    logger.Debug("Documento principale inserito");
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOPUTFILE", frSch.docNumber, string.Format("{0} {1}", "Acquisito documento N.ro:", frSch.docNumber), DocsPaVO.Logger.CodAzione.Esito.OK);
                    frSch.fileName = docPrincipaleName;
                    InteroperabilitaUtils.MatchTSR(filepath,frSch, fd,infoUtente);
                    XmlParsing.XmlParserManager.parseExtraXmlfiles(sd, docPrincipaleName, fd.content, infoUtente, ruolo);
                }

                //ricerca degli allegati
                //				logger.addMessage("Inserimento degli allegati");
                logger.Debug("Inserimento degli allegati");

                for (int i = 0; i < mc.attachments.Count; i++)
                {
                    //estrazione dati dell'allegato
                    string nomeAllegato = mc.attachments[i].name;
                    string nomeAllegatoAlternativo = mc.attachments[i].alterntiveName;
                    //					logger.addMessage("Inserimento allegato "+nomeAllegato);
                    logger.Debug("Inserimento allegato " + nomeAllegato);

                    if (InteroperabilitaUtils.FindTSRMatch(filepath, (nomeAllegatoAlternativo ?? nomeAllegato)))
                        continue; //il TSR fa parte di un o dei doc, salto l'acquisizione e l'aggiunta dell'allegato


                    DocsPaVO.documento.Allegato all = new DocsPaVO.documento.Allegato();

                    //all.applicazione=getApp(nomeAllegato,logger);
                    all.docNumber = sd.docNumber;
                    all.fileName = getFileName(nomeAllegato);
                    all.version = "0";

                    all.descrizione = "allegato " + i;
                    if (!String.IsNullOrEmpty(all.fileName))
                        all.descrizione = all.fileName;
                   
                    //Andrea De Marco - Inserimento dell'allegato segnatura come allegato Utente (il cliente avrebbe preferito PEC ma si è scelto per il momento di inserirlo come allegato PEC)
                    //Andrea De Marco - Osserva che i metodi AggiungiAllegato e AggiungiAllegatoPEC fanno la stessa cosa.
                    if (eccSegnatura && all.fileName.ToLower().Equals("segnatura.xml") || all.fileName.ToLower().Equals("segnatura"))
                    {
                        try
                        {
                            //Aggiunge la descrizione allegato segnatura.xml all'allegato
                            all.descrizione = "allegato segnatura.xml";
                            //Così aggiunge allegato utente
                            DocsPaVO.documento.Allegato res = null;
                            res = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegatoPEC(infoUtente, all);
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOAGGIUNGIALLEGATO", all.docNumber, string.Format("{0}{1}{2}{3}", "Aggiunto al N.ro Doc.: ", all.docNumber, " il N.ro Allegato PEC: ", res.versionLabel), DocsPaVO.Logger.CodAzione.Esito.OK);
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCNEWALLEGATO", all.docNumber, string.Format("{0}{1}{2}{3}", "Aggiunto al N.ro Doc.: ", all.docNumber, " il N.ro Allegato PEC: ", res.versionLabel), DocsPaVO.Logger.CodAzione.Esito.OK);
                        }
                        catch (Exception e)
                        {
                            err = "errore nel metodo aggiungiAllegato per l'allegato n. " + Convert.ToString(i + 1);
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOAGGIUNGIALLEGATO", all.docNumber, string.Format("{0}{1}", "Errore in inserimento allegato al N.ro Doc.: ", all.docNumber), DocsPaVO.Logger.CodAzione.Esito.KO);
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCNEWALLEGATO", all.docNumber, string.Format("{0}{1}", "Errore in inserimento allegato al N.ro Doc.: ", all.docNumber), DocsPaVO.Logger.CodAzione.Esito.KO);
                            logger.Error(err);
                            throw e;
                        }
                    }
                    else
                    {
                        try
                        {
                            DocsPaVO.documento.Allegato res = null;
                            res = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, all);
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOAGGIUNGIALLEGATO", all.docNumber, string.Format("{0}{1}{2}{3}", "Aggiunto al N.ro Doc.: ", all.docNumber, " il N.ro Allegato: ", res.versionLabel), DocsPaVO.Logger.CodAzione.Esito.OK);
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCNEWALLEGATO", all.docNumber, string.Format("{0}{1}{2}{3}", "Aggiunto al N.ro Doc.: ", all.docNumber, " il N.ro Allegato: ", res.versionLabel), DocsPaVO.Logger.CodAzione.Esito.OK);
                        }
                        catch (Exception e)
                        {
                            err = "errore nel metodo aggiungiAllegato per l'allegato n. " + Convert.ToString(i + 1);
                            logger.Error(err);
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOAGGIUNGIALLEGATO", all.docNumber, string.Format("{0}{1}", "Errore in inserimento allegato al N.ro Doc.: ", all.docNumber), DocsPaVO.Logger.CodAzione.Esito.KO);
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCNEWALLEGATO", all.docNumber, string.Format("{0}{1}", "Errore in inserimento allegato al N.ro Doc.: ", all.docNumber), DocsPaVO.Logger.CodAzione.Esito.KO);
                            throw e;
                        }
                    }
                    //End Andrea De Marco - Per ripristino, togliere l'if e il suo corpo; togliere l'else e lasciare intatto il suo corpo. 

                    if (eccSegnatura && mc.attachments[i].name.ToLower().Equals("segnatura.xml") || mc.attachments[i].name.ToLower().Equals("segnatura"))
                    {
                        System.IO.FileStream fsAllSeg = null;

                        logger.Debug("Allegato id=" + all.versionId);
                        logger.Debug("Allegato version label=" + all.versionLabel);
                        logger.Debug("Inserimento nel filesystem");

                        DocsPaVO.documento.FileDocumento fdAllSeg = new DocsPaVO.documento.FileDocumento();

                        fsAllSeg = new System.IO.FileStream(filepath + "\\" + nomeAllegato, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                        byte[] bufferAllSeg = new byte[fsAllSeg.Length];
                        fsAllSeg.Read(bufferAllSeg, 0, (int)fsAllSeg.Length);
                        fdAllSeg.content = bufferAllSeg;
                        fdAllSeg.length = bufferAllSeg.Length;
                        fdAllSeg.name = nomeAllegato;
                        DocsPaVO.documento.FileRequest fRAllSeg = (DocsPaVO.documento.FileRequest)all;

                        // inserito per permettere la memorizzazione corretta del path nella components 
                        // in caso di ricezione per interoperabilità e PitreDualFileWritingMode=false
                        fRAllSeg.fileName = nomeAllegato;

                        if (!BusinessLogic.Documenti.FileManager.putFile(ref fRAllSeg, fdAllSeg, infoUtente, out err))
                            throw new Exception(err);
                        else
                        {
                            logger.Debug("Allegato " + i + " inserito");
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOPUTFILE", fRAllSeg.docNumber, string.Format("{0} {1}", "Acquisito documento N.ro:", fRAllSeg.docNumber), DocsPaVO.Logger.CodAzione.Esito.OK);
                        }

                        fsAllSeg.Close();
                    }
                    else
                    {

                        logger.Debug("Allegato id=" + all.versionId);
                        logger.Debug("Allegato version label=" + all.versionLabel);
                        logger.Debug("Inserimento nel filesystem");

                        DocsPaVO.documento.FileDocumento fdAll = new DocsPaVO.documento.FileDocumento();
                        fsAll = new System.IO.FileStream(filepath + "\\" + (nomeAllegatoAlternativo ?? nomeAllegato), System.IO.FileMode.Open);
                        byte[] bufferAll = new byte[fsAll.Length];
                        fsAll.Read(bufferAll, 0, (int)fsAll.Length);
                        fdAll.content = bufferAll;
                        fdAll.length = bufferAll.Length;
                        fdAll.name = nomeAllegato;
                        DocsPaVO.documento.FileRequest fRAll = (DocsPaVO.documento.FileRequest)all;

                        // inserito per permettere la memorizzazione corretta del path nella components 
                        // in caso di ricezione per interoperabilità e PitreDualFileWritingMode=false
                        fRAll.fileName = nomeAllegato;

                        if (!BusinessLogic.Documenti.FileManager.putFile(ref fRAll, fdAll, infoUtente, out err))
                            throw new Exception(err);
                        else
                        {
                            logger.Debug("Allegato " + i + " inserito");
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOPUTFILE", fRAll.docNumber, string.Format("{0} {1}", "Acquisito documento N.ro:", fRAll.docNumber), DocsPaVO.Logger.CodAzione.Esito.OK);
                            fRAll.fileName = nomeAllegato;
                            InteroperabilitaUtils.MatchTSR(filepath, fRAll, fdAll, infoUtente);
                            XmlParsing.XmlParserManager.parseExtraXmlfiles(sd, nomeAllegato, fdAll.content, infoUtente, ruolo);
                            
                        }

                        fsAll.Close();
                    }
                }


                ///salvataggio della mail
                if (!string.IsNullOrEmpty(nomeMail))
                {
                    logger.Debug("Inserimento allegato " + nomeMail);

                    DocsPaVO.documento.Allegato all = new DocsPaVO.documento.Allegato();
                    all.descrizione = "E-Mail Ricevuta";
                    all.docNumber = sd.docNumber;
                    all.fileName = getFileName(nomeMail);
                    all.version = "0";

                    DocsPaVO.documento.Allegato res = null;
                    try
                    {
                        BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, all);
                        if (res != null)
                        {
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOAGGIUNGIALLEGATO", all.docNumber, string.Format("{0}{1}{2}{3}", "Aggiunto al N.ro Doc.: ", all.docNumber, " il N.ro Allegato: ", res.versionLabel), DocsPaVO.Logger.CodAzione.Esito.OK);
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCNEWALLEGATO", all.docNumber, string.Format("{0}{1}{2}{3}", "Aggiunto al N.ro Doc.: ", all.docNumber, " il N.ro Allegato: ", res.versionLabel), DocsPaVO.Logger.CodAzione.Esito.OK);
                        }
                    }
                    catch (Exception e)
                    {
                        err = "errore nel metodo aggiungiAllegato per il salvatggio della E-Mail spedita";
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOAGGIUNGIALLEGATO", all.docNumber, string.Format("{0}{1}", "Errore in inserimento allegato al N.ro Doc.: ", all.docNumber), DocsPaVO.Logger.CodAzione.Esito.KO);
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCNEWALLEGATO", all.docNumber, string.Format("{0}{1}", "Errore in inserimento allegato al N.ro Doc.: ", all.docNumber), DocsPaVO.Logger.CodAzione.Esito.KO);
                        logger.Error(err);
                        throw e;
                    }

                    logger.Debug("Allegato id=" + all.versionId);
                    logger.Debug("Allegato version label=" + all.versionLabel);
                    logger.Debug("Inserimento nel filesystem");

                    DocsPaVO.documento.FileDocumento fdAll = new DocsPaVO.documento.FileDocumento();
                    fsAll = new System.IO.FileStream(filepath + "\\" + nomeMail, System.IO.FileMode.Open);
                    byte[] bufferAll = new byte[fsAll.Length];
                    fsAll.Read(bufferAll, 0, (int)fsAll.Length);
                    fdAll.content = bufferAll;
                    fdAll.length = bufferAll.Length;
                    fdAll.name = nomeMail;
                    DocsPaVO.documento.FileRequest fRAll = (DocsPaVO.documento.FileRequest)all;
                    if (!BusinessLogic.Documenti.FileManager.putFile(ref fRAll, fdAll, infoUtente, out err))
                        throw new Exception(err);
                    else
                    {
                        logger.Debug("E-Mail salavata correttamente");
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOPUTFILE", fRAll.docNumber, string.Format("{0} {1}", "Acquisito documento N.ro:", fRAll.docNumber), DocsPaVO.Logger.CodAzione.Esito.OK);
                    }

                    fsAll.Close();
                }

                ///fine salvataggio della mail

                // Per gestione pendenti tramite PEC

                if (true)//(string.IsNullOrEmpty(sd.privato)||!string.IsNullOrEmpty(sd.privato) && sd.privato != "1")
                {
                    //TRASMISSIONE 
                    //				logger.addMessage("Preparazione trasmissione...");
                    logger.Debug("Preparazione trasmissione...");

                    //eseguiTrasmissione(infoUtente.idPeople,serverName,sd,null,reg,ruolo);

                    try
                    {
                        //eseguiTrasmissione(infoUtente.idPeople,serverName, sd,infoDestinatari,reg,ruolo);
                        //eseguiTrasmissione(infoUtente.idPeople, serverName, sd, "INTEROPERABILITA", reg, ruolo, infoUtente.dst);                 
                        //commentata furnari -- eseguiTrasmissione(infoUtente.idPeople, serverName, sd, null, reg, ruolo, infoUtente.dst, mailAddress, infoUtente);

                        // S. Furnari - 16/01/2013 - Sviluppo trasmissione documento ricevuto per interoperabilità interna
                        // solo a ruoli nella UO destinataria e non a tutta la AOO
                        //eseguiTrasmissione(infoUtente.idPeople, serverName, sd, null, reg, ruolo, infoUtente.dst, mailAddress);
                        eseguiTrasmissione(infoUtente.idPeople, serverName, sd, null, reg, ruolo, infoUtente.dst, mailAddress, infoUtente, null);

                        if (!checkExecTrasm(sd.systemId, "INTEROPERABILITA"))
                        {
                            codint1 = true;
                            err = "CODINTEROP1 Trasmissione per INTEROPERABILITA del documento con id:" + sd.docNumber + " non effettuata. Il documento può essere trovato tra i predisposti.";
                            throw new Exception("(CODINTEROP1) Trasmissione per INTEROPERABILITA del documento con id:" + sd.docNumber + "non effettuata!");
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(err);
                        throw ex;
                    }
                }
                else
                {
                    logger.Debug("PEC4R1: Mail ricevute come pendenti. Provo a non eseguire la trasmissione. Senza Segnatura");
                }
                if(fatturaElDaPEC)
                    BusinessLogic.Amministrazione.SistemiEsterni.FattElDaPEC(sd.docNumber, infoUtente);

                if (sd != null)
                    docnumber = sd.docNumber;
                return true;
            }
            catch (Exception e)
            {
                //				logger.addMessage("La mail viene sospesa. Eccezione: "+e.ToString()); 
                logger.Error("La mail viene sospesa. Eccezione: " + e.ToString());

                if (err.Contains("CODINTEROP1"))
                {
                    if (sd != null)
                        docnumber = sd.docNumber;
                    if (InteroperabilitaUtils.MailElaborata(mailId, "E", reg.systemId, docnumber, mc.from))
                        logger.Debug("La mail è stata elaborata");
                    else
                        logger.Debug("La mail non è stata elaborata");
                }
                else
                {

                    if (string.IsNullOrEmpty(err))
                        err = "errore durante la creazione del documento";//e.Message+" "+e.StackTrace;
                    if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
                        logger.Debug("Sospensione eseguita, errore: " + err);
                    else
                        logger.Debug("Sospensione non eseguita, errore " + err);

                    logger.Debug("errore :" + e.Message + " " + e.StackTrace);

                }
                if (fs != null) fs.Close();
                if (fsAll != null) fsAll.Close();
                if (sd != null && !codint1)
                {
                    DocsPaDB.Query_DocsPAWS.Interoperabilita doc= new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                    doc.CestinaPredisposto(sd.docNumber);
                    //BusinessLogic.Documenti.DocManager.ExecRimuoviSchedaMethod(infoUtente, sd);
                    logger.Debug("Eseguita rimozione profilo");
                    logger.Debug(err);
                }

                return false;
            }
        }


        //public static bool eseguiSenzaSegnaturaProtocollazione(string serverName, string filepath, DocsPaVO.utente.Registro reg, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, string mailId, Interoperabilità.CMMsg mc, out string err)
        //{
        //    System.IO.FileStream fs = null;
        //    System.IO.FileStream fsAll = null;
        //    bool daAggiornareUffRef = false;
        //    DocsPaVO.documento.SchedaDocumento sd = null;
        //    err = string.Empty;
        //    bool codint1 = false;
        //    try
        //    {
        //        string docPrincipaleName = "body.html";

        //        //CONTROLLI NEL FORMATO DEI FILES
        //        //Documento principale
        //        if (getApp(docPrincipaleName) == null)
        //        {
        //            logger.Debug("La mail viene sospesa. Il documento principale ha un formato non gestito");
        //            err = "La mail viene sospesa. Il documento principale ha un formato non gestito";
        //            if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
        //            {
        //                logger.Debug("Sospensione eseguita");
        //            }
        //            else
        //            {
        //                logger.Debug("Sospensione non eseguita");
        //            };
        //            return false;
        //        }
        //        //Attachment
        //        for (int ind = 0; ind < mc.attachments.Count; ind++)
        //        {

        //            if (getApp(mc.attachments[ind].name) == null)
        //            {
        //                //						logger.addMessage("La mail viene sospesa. Il documento "+mc.Attachments[ind].Name+" ha un formato non gestito"); 
        //                logger.Debug("La mail viene sospesa. Il documento " + mc.attachments[ind].name + " ha un formato non gestito");
        //                err = "La mail viene sospesa. Il documento " + mc.attachments[ind].name + " ha un formato non gestito";
        //                if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
        //                {
        //                    //							logger.addMessage("Sospensione eseguita");
        //                    logger.Debug("Sospensione eseguita");
        //                }
        //                else
        //                {
        //                    //							logger.addMessage("Sospensione non eseguita");
        //                    logger.Debug("Sospensione non eseguita");
        //                }

        //                return false;
        //            }
        //        }

        //        //mittente 
        //        DocsPaVO.utente.Corrispondente mittente = null;//corrispondente = null;
        //        //corrispondente 
        //        mittente = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(mc.from, infoUtente);
        //        if (mittente == null)
        //        {
        //            mittente = new DocsPaVO.utente.UnitaOrganizzativa();
        //            mittente.descrizione = mc.from;
        //            mittente.tipoIE = "E";
        //            mittente.tipoCorrispondente = "S";
        //            mittente.codiceRubrica = mc.from;
        //            mittente.codiceAOO = reg.codice;
        //            mittente.codiceAmm = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(reg.idAmministrazione).Codice;
        //            mittente.email = mc.from;
        //            string idMezzoSpedizione = BusinessLogic.Documenti.InfoDocManager.getIdMezzoSpedizioneByDesc("MAIL");
        //            mittente.canalePref = BusinessLogic.Documenti.InfoDocManager.getCanaleBySystemId(idMezzoSpedizione);
        //            mittente.cognome = mc.from;
        //            mittente.idAmministrazione = reg.idAmministrazione;

        //        }

        //        //logger.addMessage("Inserimento documento principale");
        //        logger.Debug("Inserimento documento principale");

        //        sd = new DocsPaVO.documento.SchedaDocumento();
        //        sd.appId = getApp(docPrincipaleName).application;
        //        sd.idPeople = infoUtente.idPeople;
        //        sd.userId = infoUtente.userId;
        //        DocsPaVO.documento.Oggetto ogg = new DocsPaVO.documento.Oggetto();
        //        ogg.descrizione = mc.subject;
        //        sd.oggetto = ogg;

        //        sd.tipoProto = "G";
        //        sd.typeId = "MAIL";
        //        logger.Debug("Salvataggio doc...");
        //        try
        //        {
        //            sd = BusinessLogic.Documenti.DocSave.addDocGrigia(sd, infoUtente, ruolo);
        //        }
        //        catch (Exception ex)
        //        {
        //            err = "Errore durante la creazione del documento.";
        //            logger.Debug("Errore durante la creazione del documento. " + ex.Message);

        //            throw new Exception();

        //        }

        //        sd.predisponiProtocollazione = false;


        //        //28/04/08
        //        //sd.registro = reg;

        //        sd.registro = CaricaRegistroInScheda(reg);

        //        sd.tipoProto = "A";
        //        sd.typeId = "MAIL";
        //        sd.interop = "P";
        //        DocsPaVO.documento.ProtocolloEntrata protEntr = new DocsPaVO.documento.ProtocolloEntrata();
        //        protEntr.mittente = mittente;



        //        //Controllo se nell'headers esiste la chiave "utenteDocspa"
        //        //In questo caso risolvo tramite email il mittete e lo imposto come mittente intermedio
        //        //del protocollo in ingresso
        //        System.Collections.Hashtable headers = mc.headers;
        //        if (headers["utenteDocspa"] != null)
        //        {
        //            logger.Debug("Risoluzione mittente intermedio ...");
        //            System.Collections.IDictionaryEnumerator enumerator = headers.GetEnumerator();
        //            while (enumerator.MoveNext())
        //            {
        //                if (enumerator.Key.ToString() == "utenteDocspa")
        //                {
        //                    DocsPaVO.utente.Corrispondente mittenteIntermedio = BusinessLogic.Utenti.UserManager.getUtenteByEmail(reg.idAmministrazione, enumerator.Value.ToString());
        //                    if (mittenteIntermedio != null && mittenteIntermedio.systemId != null)
        //                        protEntr.mittenteIntermedio = mittenteIntermedio;
        //                }
        //            }
        //            headers.Remove("utenteDocspa");
        //            logger.Debug("Risoluzione mittente intermedio eseguita ...");
        //        }



        //        sd.protocollo = protEntr;

        //        sd.protocollo.invioConferma = "0";


        //        //				logger.addMessage("Salvataggio doc...");


        //        //Controllo che il file allegato sia unico e sia una form pdf da processare
        //        if (mc.attachments.Count == 1 && System.IO.Path.GetExtension(mc.attachments[0].name).ToUpper() == ".PDF")
        //        {
        //            //Recupero il fileDocumento
        //            DocsPaVO.documento.FileDocumento fdAll = new DocsPaVO.documento.FileDocumento();
        //            fsAll = new System.IO.FileStream(filepath + "\\" + mc.attachments[0].name, System.IO.FileMode.Open);
        //            byte[] bufferAll = new byte[fsAll.Length];
        //            fsAll.Read(bufferAll, 0, (int)fsAll.Length);
        //            fdAll.content = bufferAll;
        //            fdAll.length = bufferAll.Length;
        //            fdAll.name = mc.attachments[0].name;
        //            fsAll.Close();

        //            //Controllo che il file pdf sia una form che sappiamo processare
        //            DocsPaVO.LiveCycle.ProcessFormInput processFormInput = new DocsPaVO.LiveCycle.ProcessFormInput();
        //            processFormInput.schedaDocumentoInput = sd;
        //            processFormInput.fileDocumentoInput = fdAll;
        //            DocsPaVO.LiveCycle.ProcessFormOutput processFormOutput = BusinessLogic.LiveCycle.LiveCycle.processFormPdf(infoUtente, processFormInput);

        //            if (processFormOutput != null && processFormOutput.schedaDocumentoOutput != null)
        //            {
        //                sd = processFormOutput.schedaDocumentoOutput;
        //                docPrincipaleName = mc.attachments[0].name;
        //                mc.attachments.Clear();
        //            }
        //        }

        //        //salvo mittente dopo cambio descrizione se c'è modulo PDF.
        //        mittente = ((DocsPaVO.documento.ProtocolloEntrata)sd.protocollo).mittente;
        //        if (mittente != null && string.IsNullOrEmpty(mittente.systemId))
        //            mittente = BusinessLogic.Utenti.addressBookManager.insertCorrispondente(mittente, null);

        //        if (mittente != null && mittente.canalePref != null && !string.IsNullOrEmpty(mittente.canalePref.systemId))
        //        {
        //            sd.mezzoSpedizione = mittente.canalePref.systemId;
        //            sd.descMezzoSpedizione = mittente.canalePref.descrizione;
        //        }

        //        try
        //        {
        //            sd = BusinessLogic.Documenti.DocSave.save(infoUtente, sd, false, out daAggiornareUffRef, ruolo);
        //        }
        //        catch (Exception ex)
        //        {
        //            err = "Errore durante il salvataggio del documento.";
        //            logger.Debug("Errore durante il salvataggio del documento. " + ex.Message);

        //            throw new Exception(err);

        //        }

        //        logger.Debug("Salvataggio eseguito");

        //        if (mittente != null && mittente.canalePref != null && !string.IsNullOrEmpty(mittente.canalePref.systemId))
        //        {
        //            sd.mezzoSpedizione = mittente.canalePref.systemId;
        //            sd.descMezzoSpedizione = mittente.canalePref.descrizione;
        //        }

        //        sd.predisponiProtocollazione = false;
        //        // sd = BusinessLogic.Documenti.DocSave.save(infoUtente, sd, false, out daAggiornareUffRef, ruolo);

        //        DocsPaVO.utente.InfoUtente info = new DocsPaVO.utente.InfoUtente();
        //        info.idAmministrazione = sd.registro.idAmministrazione;

        //        if (mittente != null && mittente.canalePref != null && !string.IsNullOrEmpty(mittente.canalePref.systemId))
        //        {
        //            BusinessLogic.Documenti.ProtoManager.collegaMezzoSpedizioneDocumento(info, mittente.canalePref.systemId, sd.systemId);
        //        }

        //        logger.Debug("eseguito insert mezzo sped");

        //        DocsPaVO.documento.FileDocumento fd = new DocsPaVO.documento.FileDocumento();
        //        fs = new System.IO.FileStream(filepath + "\\" + docPrincipaleName, System.IO.FileMode.Open);
        //        byte[] buffer = new byte[fs.Length];
        //        fs.Read(buffer, 0, (int)fs.Length);
        //        fd.content = buffer;
        //        fd.length = buffer.Length;
        //        fd.name = docPrincipaleName;
        //        DocsPaVO.documento.FileRequest frSch = (DocsPaVO.documento.FileRequest)sd.documenti[0];
        //        if (!BusinessLogic.Documenti.FileManager.putFile(ref frSch, fd, infoUtente, out err))
        //            throw new Exception(err);
        //        //				logger.addMessage("Documento principale inserito");
        //        else
        //            logger.Debug("Documento principale inserito");

        //        //ricerca degli allegati
        //        //				logger.addMessage("Inserimento degli allegati");
        //        logger.Debug("Inserimento degli allegati");

        //        for (int i = 0; i < mc.attachments.Count; i++)
        //        {
        //            //estrazione dati dell'allegato
        //            string nomeAllegato = mc.attachments[i].name;
        //            //					logger.addMessage("Inserimento allegato "+nomeAllegato);
        //            logger.Debug("Inserimento allegato " + nomeAllegato);

        //            DocsPaVO.documento.Allegato all = new DocsPaVO.documento.Allegato();
        //            all.descrizione = "allegato " + i;
        //            //all.applicazione=getApp(nomeAllegato,logger);
        //            all.docNumber = sd.docNumber;
        //            all.fileName = getFileName(nomeAllegato);
        //            all.version = "0";
        //            try
        //            {
        //                BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, all);
        //            }
        //            catch (Exception e)
        //            {
        //                err = "errore nel metodo aggiungiAllegato per l'allegato n. " + Convert.ToString(i + 1);
        //                logger.Debug(err);
        //                throw e;
        //            }

        //            #region Codice Commentato
        //            //					logger.addMessage("Allegato id="+all.versionId);
        //            //					logger.addMessage("Allegato version label="+all.versionLabel);
        //            //					logger.addMessage("Inserimento nel filesystem");
        //            #endregion

        //            logger.Debug("Allegato id=" + all.versionId);
        //            logger.Debug("Allegato version label=" + all.versionLabel);
        //            logger.Debug("Inserimento nel filesystem");

        //            DocsPaVO.documento.FileDocumento fdAll = new DocsPaVO.documento.FileDocumento();
        //            fsAll = new System.IO.FileStream(filepath + "\\" + nomeAllegato, System.IO.FileMode.Open);
        //            byte[] bufferAll = new byte[fsAll.Length];
        //            fsAll.Read(bufferAll, 0, (int)fsAll.Length);
        //            fdAll.content = bufferAll;
        //            fdAll.length = bufferAll.Length;
        //            fdAll.name = nomeAllegato;
        //            DocsPaVO.documento.FileRequest fRAll = (DocsPaVO.documento.FileRequest)all;
        //            if (!BusinessLogic.Documenti.FileManager.putFile(ref fRAll, fdAll, infoUtente, out err))
        //                throw new Exception(err);
        //            else
        //                logger.Debug("Allegato " + i + " inserito");

        //            fsAll.Close();
        //        }

        //        //TRASMISSIONE 
        //        //				logger.addMessage("Preparazione trasmissione...");
        //        logger.Debug("Preparazione trasmissione...");

        //        //eseguiTrasmissione(infoUtente.idPeople,serverName,sd,null,reg,ruolo);

        //        try
        //        {
        //            //eseguiTrasmissione(infoUtente.idPeople,serverName, sd,infoDestinatari,reg,ruolo);
        //            //eseguiTrasmissione(infoUtente.idPeople, serverName, sd, "INTEROPERABILITA", reg, ruolo, infoUtente.dst);
        //            eseguiTrasmissione(infoUtente.idPeople, serverName, sd, null, reg, ruolo, infoUtente.dst);

        //            if (!checkExecTrasm(sd.systemId, "INTEROPERABILITA"))
        //            {
        //                codint1 = true;
        //                err = "CODINTEROP1 Trasmissione per INTEROPERABILITA del documento con id:" + sd.docNumber + " non effettuata. Il documento può essere trovato tra i predisposti.";
        //                throw new Exception("(CODINTEROP1) Trasmissione per INTEROPERABILITA del documento con id:" + sd.docNumber + "non effettuata!");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            logger.Debug(err);
        //            throw ex;
        //        }


        //        fs.Close();
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        //				logger.addMessage("La mail viene sospesa. Eccezione: "+e.ToString()); 
        //        logger.Debug("La mail viene sospesa. Eccezione: " + e.ToString());

        //        if (err.Contains("CODINTEROP1"))
        //        {
        //            if (InteroperabilitaUtils.MailElaborata(mailId, "E", reg.systemId))
        //                logger.Debug("La mail è stata elaborata");
        //            else
        //                logger.Debug("La mail non è stata elaborata");
        //        }
        //        else
        //        {

        //            if (string.IsNullOrEmpty(err))
        //                err = "errore durante la creazione del documento";//e.Message+" "+e.StackTrace;
        //            if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
        //                logger.Debug("Sospensione eseguita, errore: " + err);
        //            else
        //                logger.Debug("Sospensione non eseguita, errore " + err);

        //            logger.Debug("errore :" + e.Message + " " + e.StackTrace);

        //        }
        //        if (fs != null) fs.Close();
        //        if (fsAll != null) fsAll.Close();
        //        if (sd != null && !codint1)
        //        {
        //            BusinessLogic.Documenti.DocManager.ExecRimuoviSchedaMethod(infoUtente, sd);
        //            logger.Debug("Eseguita rimozione profilo");
        //            logger.Debug(err);
        //        }


        //        return false;
        //    }
        //}

        public static bool eseguiSenzaSegnaturaProtocollazione(string serverName, string filepath, DocsPaVO.utente.Registro reg, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, IInteropSchedaDocHandler handler, string mailId, Interoperabilità.CMMsg mc, string isPec, out string err, out string docnumber, string nomeMail, string dataRicezione, string mailAddress)
        {
            System.IO.FileStream fs = null;
            System.IO.FileStream fsAll = null;
            bool daAggiornareUffRef = false;
            DocsPaVO.documento.SchedaDocumento sd = null;
            err = string.Empty;
            docnumber = string.Empty;
            bool codint1 = false;
            try
            {
                string docPrincipaleName = "body.html";

                //CONTROLLI NEL FORMATO DEI FILES
                //Documento principale
                if (getApp(docPrincipaleName) == null)
                {
                    logger.Debug("La mail viene sospesa. Il documento principale ha un formato non gestito");
                    err = "La mail viene sospesa. Il documento principale ha un formato non gestito";
                    if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
                    {
                        logger.Debug("Sospensione eseguita");
                    }
                    else
                    {
                        logger.Debug("Sospensione non eseguita");
                    };
                    return false;
                }
                //Attachment
                for (int ind = 0; ind < mc.attachments.Count; ind++)
                {

                    if (getApp(mc.attachments[ind].name) == null)
                    {
                        //						logger.addMessage("La mail viene sospesa. Il documento "+mc.Attachments[ind].Name+" ha un formato non gestito"); 
                        logger.Debug("La mail viene sospesa. Il documento " + mc.attachments[ind].name + " ha un formato non gestito");
                        err = "La mail viene sospesa. Il documento " + mc.attachments[ind].name + " ha un formato non gestito";
                        if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
                        {
                            //							logger.addMessage("Sospensione eseguita");
                            logger.Debug("Sospensione eseguita");
                        }
                        else
                        {
                            //							logger.addMessage("Sospensione non eseguita");
                            logger.Debug("Sospensione non eseguita");
                        }

                        return false;
                    }
                }

                //mittente 
                DocsPaVO.utente.Corrispondente mittente = null;//corrispondente = null;
                //corrispondente 
                mittente = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(mc.from, infoUtente);
                string check_mitt_interop = "";
                if (mittente == null)
                {
                    mittente = new DocsPaVO.utente.UnitaOrganizzativa();
                    mittente.descrizione = mc.from;
                    mittente.tipoIE = "E";
                    mittente.tipoCorrispondente = "S";
                    mittente.codiceRubrica = mc.from;
                    mittente.codiceAOO = reg.codice;
                    mittente.codiceAmm = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(reg.idAmministrazione).Codice;
                    mittente.email = mc.from;
                    string idMezzoSpedizione = BusinessLogic.Documenti.InfoDocManager.getIdMezzoSpedizioneByDesc("MAIL");
                    mittente.canalePref = BusinessLogic.Documenti.InfoDocManager.getCanaleBySystemId(idMezzoSpedizione);
                    mittente.cognome = mc.from;
                    mittente.idAmministrazione = reg.idAmministrazione;
                    check_mitt_interop = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "CHECK_MITT_INTEROPERANTE");
                    if (check_mitt_interop.Equals("1"))
                        mittente.idRegistro = reg.systemId;
                }

                //logger.addMessage("Inserimento documento principale");
                logger.Debug("Inserimento documento principale");

                sd = new DocsPaVO.documento.SchedaDocumento();
                sd.appId = getApp(docPrincipaleName).application;
                sd.idPeople = infoUtente.idPeople;
                sd.userId = infoUtente.userId;
                if (handler != null)
                {
                    logger.Debug("richiamo customize handler");
                    handler.CustomizeSchedaDocNoSegnatura(sd, mc, filepath);
                }

                DocsPaVO.documento.Oggetto ogg = new DocsPaVO.documento.Oggetto();
                ogg.descrizione = mc.subject;
                sd.oggetto = ogg;

                sd.tipoProto = "G";
                sd.typeId = "MAIL";
                logger.Debug("Salvataggio doc...");
                try
                {
                    sd = BusinessLogic.Documenti.DocSave.addDocGrigia(sd, infoUtente, ruolo);
                }
                catch (Exception ex)
                {
                    err = "Errore durante la creazione del documento.";
                    logger.Error("Errore durante la creazione del documento. " + ex.Message);

                    throw new Exception();

                }

                sd.predisponiProtocollazione = false;


                //28/04/08
                //sd.registro = reg;

                sd.registro = CaricaRegistroInScheda(reg);

                sd.tipoProto = "A";
                sd.typeId = "MAIL";
                sd.interop = "P";
                DocsPaVO.documento.ProtocolloEntrata protEntr = new DocsPaVO.documento.ProtocolloEntrata();
                protEntr.mittente = mittente;



                //Controllo se nell'headers esiste la chiave "utenteDocspa"
                //In questo caso risolvo tramite email il mittete e lo imposto come mittente intermedio
                //del protocollo in ingresso
                System.Collections.Hashtable headers = mc.headers;
                if (headers["utenteDocspa"] != null)
                {
                    logger.Debug("Risoluzione mittente intermedio ...");
                    System.Collections.IDictionaryEnumerator enumerator = headers.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Key.ToString() == "utenteDocspa")
                        {
                            DocsPaVO.utente.Corrispondente mittenteIntermedio = BusinessLogic.Utenti.UserManager.getUtenteByEmail(reg.idAmministrazione, enumerator.Value.ToString());
                            if (mittenteIntermedio != null && mittenteIntermedio.systemId != null)
                                protEntr.mittenteIntermedio = mittenteIntermedio;
                        }
                    }
                    headers.Remove("utenteDocspa");
                    logger.Debug("Risoluzione mittente intermedio eseguita ...");
                }



                sd.protocollo = protEntr;

                sd.protocollo.invioConferma = "0";


                //				logger.addMessage("Salvataggio doc...");


                //Controllo che il file allegato sia unico e sia una form pdf da processare
                if (mc.attachments.Count == 1 && System.IO.Path.GetExtension(mc.attachments[0].name).ToUpper() == ".PDF")
                {
                    //Recupero il fileDocumento
                    DocsPaVO.documento.FileDocumento fdAll = new DocsPaVO.documento.FileDocumento();
                    fsAll = new System.IO.FileStream(filepath + "\\" + mc.attachments[0].name, System.IO.FileMode.Open);
                    byte[] bufferAll = new byte[fsAll.Length];
                    fsAll.Read(bufferAll, 0, (int)fsAll.Length);
                    fdAll.content = bufferAll;
                    fdAll.length = bufferAll.Length;
                    fdAll.name = mc.attachments[0].name;
                    fsAll.Close();

                    //Controllo che il file pdf sia una form che sappiamo processare
                    DocsPaVO.LiveCycle.ProcessFormInput processFormInput = new DocsPaVO.LiveCycle.ProcessFormInput();
                    processFormInput.schedaDocumentoInput = sd;
                    processFormInput.fileDocumentoInput = fdAll;
                    DocsPaVO.LiveCycle.ProcessFormOutput processFormOutput = BusinessLogic.LiveCycle.LiveCycle.processFormPdf(infoUtente, processFormInput);

                    if (processFormOutput != null && processFormOutput.schedaDocumentoOutput != null)
                    {
                        sd = processFormOutput.schedaDocumentoOutput;
                        docPrincipaleName = mc.attachments[0].name;
                        mc.attachments.Clear();
                    }
                }

                //salvo mittente dopo cambio descrizione se c'è modulo PDF.
                mittente = ((DocsPaVO.documento.ProtocolloEntrata)sd.protocollo).mittente;
                if (mittente != null && string.IsNullOrEmpty(mittente.systemId))
                {
                    mittente = BusinessLogic.Utenti.addressBookManager.insertCorrispondente(mittente, null);
                    //inserisco la mail associata al corrispondente esterno in DPA_MAIL_CORR_ESTERNI
                    List<DocsPaVO.utente.MailCorrispondente> casella = new List<DocsPaVO.utente.MailCorrispondente>();
                    casella.Add(new DocsPaVO.utente.MailCorrispondente()
                    {
                        Email = mittente.email,
                        Note = "",
                        Principale = "1"
                    });
                    BusinessLogic.Utenti.addressBookManager.InsertMailCorrispondente(casella, mittente.systemId);
                }

                if (mittente != null && mittente.canalePref != null && !string.IsNullOrEmpty(mittente.canalePref.systemId))
                {
                    sd.mezzoSpedizione = mittente.canalePref.systemId;
                    sd.descMezzoSpedizione = mittente.canalePref.descrizione;
                }

                try
                {
                    ((DocsPaVO.documento.Documento)sd.documenti[0]).dataArrivo = dataRicezione;
                    sd = BusinessLogic.Documenti.DocSave.save(infoUtente, sd, false, out daAggiornareUffRef, ruolo);
                }
                catch (Exception ex)
                {
                    err = "Errore durante il salvataggio del documento.";
                    logger.Error("Errore durante il salvataggio del documento. " + ex.Message);

                    throw new Exception(err);

                }

                logger.Debug("Salvataggio eseguito");

                if (mittente != null && mittente.canalePref != null && !string.IsNullOrEmpty(mittente.canalePref.systemId))
                {
                    sd.mezzoSpedizione = mittente.canalePref.systemId;
                    sd.descMezzoSpedizione = mittente.canalePref.descrizione;
                }

                sd.predisponiProtocollazione = false;
                // sd = BusinessLogic.Documenti.DocSave.save(infoUtente, sd, false, out daAggiornareUffRef, ruolo);

                DocsPaVO.utente.InfoUtente info = new DocsPaVO.utente.InfoUtente();
                info.idAmministrazione = sd.registro.idAmministrazione;

                if (mittente != null && mittente.canalePref != null && !string.IsNullOrEmpty(mittente.canalePref.systemId))
                {
                    BusinessLogic.Documenti.ProtoManager.collegaMezzoSpedizioneDocumento(info, mittente.canalePref.systemId, sd.systemId);
                }

                logger.Debug("eseguito insert mezzo sped");

                DocsPaVO.documento.FileDocumento fd = new DocsPaVO.documento.FileDocumento();
                fs = new System.IO.FileStream(filepath + "\\" + docPrincipaleName, System.IO.FileMode.Open);
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
                fs.Close();
                fd.content = buffer;
                fd.length = buffer.Length;
                fd.name = docPrincipaleName;
                DocsPaVO.documento.FileRequest frSch = (DocsPaVO.documento.FileRequest)sd.documenti[0];

                // inserito per permettere la memorizzazione corretta del path nella components 
                // in caso di ricezione per interoperabilità e PitreDualFileWritingMode=false
                frSch.fileName = docPrincipaleName;

                if (!BusinessLogic.Documenti.FileManager.putFile(ref frSch, fd, infoUtente, out err))
                    throw new Exception(err);
                //				logger.addMessage("Documento principale inserito");
                else
                {
                    logger.Debug("Documento principale inserito");
                    frSch.fileName = docPrincipaleName;
                    InteroperabilitaUtils.MatchTSR(filepath, frSch, fd, infoUtente);
                    XmlParsing.XmlParserManager.parseExtraXmlfiles(sd, docPrincipaleName, fd.content, infoUtente, ruolo);
                }
                //ricerca degli allegati
                //				logger.addMessage("Inserimento degli allegati");
                logger.Debug("Inserimento degli allegati");

                for (int i = 0; i < mc.attachments.Count; i++)
                {
                    //estrazione dati dell'allegato
                    string nomeAllegato = mc.attachments[i].name;
                    string nomeAllegatoAlternativo = mc.attachments[i].alterntiveName;
                    //					logger.addMessage("Inserimento allegato "+nomeAllegato);
                    logger.Debug("Inserimento allegato " + nomeAllegato);

                    if (InteroperabilitaUtils.FindTSRMatch(filepath, (nomeAllegatoAlternativo ?? nomeAllegato)))
                        continue; //il TSR fa parte di un o dei doc, salto l'acquisizione e l'aggiunta dell'allegato


                    DocsPaVO.documento.Allegato all = new DocsPaVO.documento.Allegato();

                    //all.applicazione=getApp(nomeAllegato,logger);
                    all.docNumber = sd.docNumber;
                    all.fileName = getFileName(nomeAllegato);
                    all.version = "0";
                    
                    all.descrizione = "allegato " + i;
                    if (!String.IsNullOrEmpty(all.fileName))
                        all.descrizione = all.fileName;

                    try
                    {
                        BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, all);
                    }
                    catch (Exception e)
                    {
                        err = "errore nel metodo aggiungiAllegato per l'allegato n. " + Convert.ToString(i + 1);
                        logger.Error(err);
                        throw e;
                    }

                    #region Codice Commentato
                    //					logger.addMessage("Allegato id="+all.versionId);
                    //					logger.addMessage("Allegato version label="+all.versionLabel);
                    //					logger.addMessage("Inserimento nel filesystem");
                    #endregion

                    logger.Debug("Allegato id=" + all.versionId);
                    logger.Debug("Allegato version label=" + all.versionLabel);
                    logger.Debug("Inserimento nel filesystem");

                    DocsPaVO.documento.FileDocumento fdAll = new DocsPaVO.documento.FileDocumento();
                    fsAll = new System.IO.FileStream(filepath + "\\" + (nomeAllegatoAlternativo ?? nomeAllegato), System.IO.FileMode.Open);
                    byte[] bufferAll = new byte[fsAll.Length];
                    fsAll.Read(bufferAll, 0, (int)fsAll.Length);
                    fdAll.content = bufferAll;
                    fdAll.length = bufferAll.Length;
                    fdAll.name = nomeAllegato;
                    DocsPaVO.documento.FileRequest fRAll = (DocsPaVO.documento.FileRequest)all;

                    // inserito per permettere la memorizzazione corretta del path nella components 
                    // in caso di ricezione per interoperabilità e PitreDualFileWritingMode=false
                    fRAll.fileName = nomeAllegato;

                    if (!BusinessLogic.Documenti.FileManager.putFile(ref fRAll, fdAll, infoUtente, out err))
                        throw new Exception(err);
                    else
                    {
                        logger.Debug("Allegato " + i + " inserito");
                        fRAll.fileName = nomeAllegato;
                        InteroperabilitaUtils.MatchTSR(filepath, fRAll, fdAll, infoUtente);
                        XmlParsing.XmlParserManager.parseExtraXmlfiles(sd, nomeAllegato, fdAll.content, infoUtente, ruolo);
                    }
                    fsAll.Close();
                }

                ///salvataggio della mail
                if (!string.IsNullOrEmpty(nomeMail))
                {
                    logger.Debug("Inserimento allegato " + nomeMail);

                    DocsPaVO.documento.Allegato all = new DocsPaVO.documento.Allegato();
                    all.descrizione = "E-Mail Ricevuta";
                    all.docNumber = sd.docNumber;
                    all.fileName = getFileName(nomeMail);
                    all.version = "0";
                    try
                    {
                        BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, all);
                    }
                    catch (Exception e)
                    {
                        err = "errore nel metodo aggiungiAllegato per il salvatggio della E-Mail spedita";
                        logger.Error(err);
                        throw e;
                    }

                    logger.Debug("Allegato id=" + all.versionId);
                    logger.Debug("Allegato version label=" + all.versionLabel);
                    logger.Debug("Inserimento nel filesystem");

                    DocsPaVO.documento.FileDocumento fdAll = new DocsPaVO.documento.FileDocumento();
                    fsAll = new System.IO.FileStream(filepath + "\\" + nomeMail, System.IO.FileMode.Open);
                    byte[] bufferAll = new byte[fsAll.Length];
                    fsAll.Read(bufferAll, 0, (int)fsAll.Length);
                    fdAll.content = bufferAll;
                    fdAll.length = bufferAll.Length;
                    fdAll.name = nomeMail;
                    DocsPaVO.documento.FileRequest fRAll = (DocsPaVO.documento.FileRequest)all;
                    if (!BusinessLogic.Documenti.FileManager.putFile(ref fRAll, fdAll, infoUtente, out err))
                        throw new Exception(err);
                    else
                        logger.Debug("E-Mail salavata correttamente");


                    fsAll.Close();

                }

                ///fine salvataggio della mail
                //TRASMISSIONE 
                //				logger.addMessage("Preparazione trasmissione...");
                logger.Debug("Preparazione trasmissione...");

                //eseguiTrasmissione(infoUtente.idPeople,serverName,sd,null,reg,ruolo);

                try
                {
                    DocsPaVO.documento.ResultProtocollazione resultProtocollazione;
                    sd = BusinessLogic.Documenti.ProtoManager.protocolla(sd, ruolo, infoUtente, out resultProtocollazione);
                    //systemId protocollo in arrivo appena creato su registro automatico
                    if (resultProtocollazione != DocsPaVO.documento.ResultProtocollazione.OK)
                    {
                        err = "Errore durante la protocollazione automatica batch da interoperabilità del doc id: " + sd.systemId + "Errore " + resultProtocollazione.ToString();
                        logger.Debug(err);
                        throw new Exception("Errore durante la protocollazione automatica batch da interoperabilità del doc id: " + "Errore " + resultProtocollazione.ToString());
                    }
                    //modifica furnari eseguiTrasmissione(infoUtente.idPeople, serverName, sd, null, reg, ruolo, infoUtente.dst, mailAddress, infoUtente);

                    // S. Furnari - 16/01/2013 - Sviluppo trasmissione documento ricevuto per interoperabilità interna
                    // solo a ruoli nella UO destinataria e non a tutta la AOO
                    //eseguiTrasmissione(infoUtente.idPeople, serverName, sd, null, reg, ruolo, infoUtente.dst, mailAddress);
                    eseguiTrasmissione(infoUtente.idPeople, serverName, sd, null, reg, ruolo, infoUtente.dst, mailAddress, infoUtente,null);

                    if (!checkExecTrasm(sd.systemId, "INTEROPERABILITA"))
                    {
                        codint1 = true;
                        err = "CODINTEROP1 Trasmissione per INTEROPERABILITA del documento con id:" + sd.docNumber + " non effettuata. Il documento può essere trovato tra i predisposti.";
                        throw new Exception("(CODINTEROP1) Trasmissione per INTEROPERABILITA del documento con id:" + sd.docNumber + "non effettuata!");
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(err);
                    throw ex;
                }
                if (sd != null)
                    docnumber = sd.docNumber;
                return true;
            }
            catch (Exception e)
            {
                //				logger.addMessage("La mail viene sospesa. Eccezione: "+e.ToString()); 
                logger.Error("La mail viene sospesa. Eccezione: " + e.ToString());

                if (err.Contains("CODINTEROP1"))
                {
                    if (sd != null)
                        docnumber = sd.docNumber;
                    if (InteroperabilitaUtils.MailElaborata(mailId, "E", reg.systemId, docnumber, mc.from))
                        logger.Debug("La mail è stata elaborata");
                    else
                        logger.Debug("La mail non è stata elaborata");
                }
                else
                {

                    if (string.IsNullOrEmpty(err))
                        err = "errore durante la creazione del documento";//e.Message+" "+e.StackTrace;
                    if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
                        logger.Debug("Sospensione eseguita, errore: " + err);
                    else
                        logger.Debug("Sospensione non eseguita, errore " + err);

                    logger.Debug("errore :" + e.Message + " " + e.StackTrace);

                }
                if (fs != null) fs.Close();
                if (fsAll != null) fsAll.Close();
                if (sd != null && !codint1)
                {
                    BusinessLogic.Documenti.DocManager.ExecRimuoviSchedaMethod(infoUtente, sd);
                    logger.Debug("Eseguita rimozione profilo");
                    logger.Debug(err);
                }


                return false;
            }
        }





        /// <summary>
        /// Ritorna il registro sul quale verrà predisposto il documento ricevuto per Interoperabilità
        /// a seconda se si sta effettuando il CheckCasella di un Registro o di un RF.
        /// 1. caso di RF: si predisponde il documento sul registro relativo alla AOO Collegata all'RF;
        /// 2. caso di REGISTRO: si predisponde il documento sul registro sul quale si sta effettuando il check;
        /// </summary>
        /// <param name="registro"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Registro CaricaRegistroInScheda(DocsPaVO.utente.Registro registro)
        {
            //dafault: registro sul quale si effettua il Check
            DocsPaVO.utente.Registro reg = registro;

            if (registro.chaRF != null && registro.chaRF == "1")
            {
                //se è un RF allora ricerco il registro relativo alla Aoo Collegata
                reg = BusinessLogic.Utenti.RegistriManager.getRegistro(registro.idAOOCollegata);
            }

            return reg;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elMittente"></param>
        /// <param name="codiceAmm"></param>
        /// <param name="tipoUtente"></param>
        /// <param name="reg"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        private static DocsPaVO.utente.Corrispondente getMittente(XmlElement elMittente, string mailAddress, string codiceAmm, DocsPaVO.addressbook.TipoUtente tipoUtente, DocsPaVO.utente.Registro reg)
        {
            //costruzione oggetto corrispondente	
            DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
            qco.tipoUtente = tipoUtente;
            qco.idAmministrazione = reg.idAmministrazione;
            System.Collections.ArrayList registri = new System.Collections.ArrayList();
            registri.Add(reg.systemId);
            logger.Debug("getMittente id_Registro " + reg.systemId);
            qco.idRegistri = registri;
            //luluciani 3.10.10
            qco.fineValidita = true;
            /****************/
            //è presente una UO ? 
            XmlElement elUO = (XmlElement)elMittente.SelectSingleNode("descendant::UnitaOrganizzativa");


            if (elUO != null && !elUO.SelectSingleNode("Denominazione").InnerText.Trim().Equals(""))
            {



                logger.Debug("caso UO, ricavo il Ruolo");
                XmlNode nodo = elMittente.SelectSingleNode(".//Ruolo");
                if (nodo != null)
                {
                    qco.descrizioneUO = nodo.PreviousSibling.InnerText.Trim();
                    qco.email = mailAddress;
                }
                else
                {
                    qco.descrizioneUO = elUO.SelectSingleNode("Denominazione").InnerText.Trim();
                    qco.email = mailAddress;
                }
            }
            /***********/

            //è presente il riferimento ad un ruolo?
            XmlElement elRuolo = (XmlElement)elMittente.SelectSingleNode("descendant::Ruolo");

            if (elRuolo != null && !elRuolo.SelectSingleNode("Denominazione").InnerText.Trim().Equals(""))
            {
                logger.Debug("caso Ruolo");
                qco.codiceUO = codiceAmm;
                qco.descrizioneRuolo = elRuolo.SelectSingleNode("Denominazione").InnerText.Trim();
                XmlElement elPersona = (XmlElement)elRuolo.SelectSingleNode("Persona");
                if (elPersona != null)
                {
                    if (elPersona.SelectSingleNode("Cognome") != null && !elPersona.SelectSingleNode("Cognome").InnerText.Trim().Equals(""))
                    {
                        qco.cognomeUtente = elPersona.SelectSingleNode("Cognome").InnerText.Trim();
                    }
                    else if (!elPersona.SelectSingleNode("Denominazione").InnerText.Trim().Equals(""))
                    {
                        qco.cognomeUtente = elPersona.SelectSingleNode("Denominazione").InnerText.Trim();
                    }
                }
            }
            else
            {
                //				logger.addMessage("caso UO");
                logger.Debug("caso UO");

                //è presente il riferimento ad una UO?
                XmlNode elLastUO = elMittente.SelectSingleNode("descendant::UnitaOrganizzativa[count(child::UnitaOrganizzativa)=0]");
                if (elLastUO != null && !elLastUO.SelectSingleNode("Denominazione").InnerText.Trim().Equals(""))
                {
                    qco.descrizioneUO = elLastUO.SelectSingleNode("Denominazione").InnerText.Trim();
                    //					logger.addMessage("qui:"+qco.descrizioneUO+qco.codiceUO);
                    logger.Debug("qui:" + qco.descrizioneUO + qco.codiceUO);

                    qco.email = mailAddress;
                }
                else
                {
                    //si usa sempre il codice amministrazione
                    qco.codiceUO = codiceAmm;
                }
            }

            //chiamata metodo addressboook
            System.Collections.ArrayList risultatiRicerca;
            if (tipoUtente == DocsPaVO.addressbook.TipoUtente.ESTERNO)
            {
                if (Interoperabilità.InteroperabilitaUtils.Cfg_InteroperSegnaturaSoloUO)
                {
                    //allora ricerco solo UO
                    qco.descrizioneRuolo = null;
                    qco.cognomeUtente = null;
                    qco.nomeUtente = null;
                }
                logger.Debug("listaCorrispondentiEstMethod");

                if (string.IsNullOrEmpty(qco.email))
                    qco.email = mailAddress;

                risultatiRicerca = BusinessLogic.Utenti.addressBookManager.listaCorrispondentiEstMethod(qco);

            }
            else
            {
                logger.Debug("listaCorrispondentiIntMethod");
                risultatiRicerca = BusinessLogic.Utenti.addressBookManager.listaCorrispondentiIntMethod(qco);
                //risultatiRicerca=BusinessLogic.Utenti.addressBookManager.listaCorrispondentiIntInteropMethod(qco);
            }
            //			logger.addMessage("Risultati ricerca: "+risultatiRicerca.Count);
            logger.Debug("Risultati ricerca: " + risultatiRicerca.Count);

            for (int i = 0; i < risultatiRicerca.Count; i++)
            {
                if (Interoperabilità.InteroperabilitaUtils.Cfg_InteroperSegnaturaSoloUO)
                    if (tipoUtente == DocsPaVO.addressbook.TipoUtente.ESTERNO)
                        return (DocsPaVO.utente.Corrispondente)risultatiRicerca[0];
                    else if (isCorrectMittente((DocsPaVO.utente.Corrispondente)risultatiRicerca[i], elMittente))
                        return (DocsPaVO.utente.Corrispondente)risultatiRicerca[i];
            }
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="elMittente"></param>
        /// <param name="mailAddress"></param>
        /// <param name="codiceAmm"></param>
        /// <param name="tipoUtente"></param>
        /// <param name="reg"></param>
        /// <param name="infoutente"></param>
        /// <param name="rows">out rows</param>
        /// <returns></returns>
        private static DocsPaVO.utente.Corrispondente getMittente(XmlElement elMittente, string mailAddress, string codiceAmm, string codiceAOO, DocsPaVO.addressbook.TipoUtente tipoUtente, DocsPaVO.utente.Registro reg, DocsPaVO.utente.InfoUtente infoutente, out string rows)
        {
            rows = "";
            //costruzione oggetto corrispondente	
            DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
            qco.tipoUtente = tipoUtente;
            qco.idAmministrazione = reg.idAmministrazione;
            System.Collections.ArrayList registri = new System.Collections.ArrayList();
            registri.Add(reg.systemId);
            logger.Debug("getMittente id_Registro " + reg.systemId);
            qco.idRegistri = registri;
            //luluciani 3.10.10
            qco.fineValidita = true;
            /****************/
            //è presente una UO ? 
            XmlElement elUO = (XmlElement)elMittente.SelectSingleNode("descendant::UnitaOrganizzativa");


            if (elUO != null && !elUO.SelectSingleNode("Denominazione").InnerText.Trim().Equals(""))
            {



                logger.Debug("caso UO, ricavo il Ruolo");
                XmlNode nodo = elMittente.SelectSingleNode(".//Ruolo");
                if (nodo != null)
                {
                    qco.descrizioneUO = nodo.PreviousSibling.InnerText.Trim();
                    qco.email = mailAddress;
                }
                else
                {
                    qco.descrizioneUO = elUO.SelectSingleNode("Denominazione").InnerText.Trim();
                    qco.email = mailAddress;
                }
            }
            /***********/

            //è presente il riferimento ad un ruolo?
            XmlElement elRuolo = (XmlElement)elMittente.SelectSingleNode("descendant::Ruolo");

            if (elRuolo != null && !elRuolo.SelectSingleNode("Denominazione").InnerText.Trim().Equals(""))
            {
                logger.Debug("caso Ruolo");
                qco.codiceUO = codiceAmm;
                qco.descrizioneRuolo = elRuolo.SelectSingleNode("Denominazione").InnerText.Trim();
                XmlElement elPersona = (XmlElement)elRuolo.SelectSingleNode("Persona");
                if (elPersona != null)
                {
                    if (elPersona.SelectSingleNode("Cognome") != null && !elPersona.SelectSingleNode("Cognome").InnerText.Trim().Equals(""))
                    {
                        qco.cognomeUtente = elPersona.SelectSingleNode("Cognome").InnerText.Trim();
                    }
                    else if (!elPersona.SelectSingleNode("Denominazione").InnerText.Trim().Equals(""))
                    {
                        qco.cognomeUtente = elPersona.SelectSingleNode("Denominazione").InnerText.Trim();
                    }
                }
            }
            else
            {
                //				logger.addMessage("caso UO");
                logger.Debug("caso UO");

                //è presente il riferimento ad una UO?
                XmlNode elLastUO = elMittente.SelectSingleNode("descendant::UnitaOrganizzativa[count(child::UnitaOrganizzativa)=0]");
                if (elLastUO != null && !elLastUO.SelectSingleNode("Denominazione").InnerText.Trim().Equals(""))
                {
                    qco.descrizioneUO = elLastUO.SelectSingleNode("Denominazione").InnerText.Trim();
                    //					logger.addMessage("qui:"+qco.descrizioneUO+qco.codiceUO);
                    logger.Debug("qui:" + qco.descrizioneUO + qco.codiceUO);

                    qco.email = mailAddress;
                }
                else
                {
                    //si usa sempre il codice amministrazione
                    qco.codiceUO = codiceAmm;
                }
            }

            XmlElement elAMM = (XmlElement)elMittente.SelectSingleNode("Amministrazione");
            string Amministrazione = string.Empty;
            if (elAMM != null && !elAMM.SelectSingleNode("Denominazione").InnerText.Trim().Equals(""))
                Amministrazione = elAMM.SelectSingleNode("Denominazione").InnerText.Trim();


            //chiamata metodo addressboook
            System.Collections.ArrayList risultatiRicerca;
            string descrizone = qco.descrizioneUO;
            if (tipoUtente == DocsPaVO.addressbook.TipoUtente.ESTERNO)
            {
                if (Interoperabilità.InteroperabilitaUtils.Cfg_InteroperSegnaturaSoloUO)
                {
                    //allora ricerco solo UO
                    qco.descrizioneRuolo = null;
                    qco.cognomeUtente = null;
                    qco.nomeUtente = null;
                }
                else
                {
                    if (!string.IsNullOrEmpty(qco.descrizioneUO))
                        descrizone = qco.descrizioneUO;
                    else
                        if (!string.IsNullOrEmpty(qco.descrizioneRuolo))
                            descrizone = qco.descrizioneRuolo;
                        else
                            if (!string.IsNullOrEmpty(qco.cognomeUtente) && !string.IsNullOrEmpty(qco.nomeUtente))
                                descrizone = qco.cognomeUtente + " " + qco.nomeUtente;
                }
                logger.Debug("listaCorrispondentiEstMethod");
                if (string.IsNullOrEmpty(qco.email))
                    qco.email = mailAddress;

                //Caso particolare, segnatura xml senza unità organizzativa (enac)
                if (string.IsNullOrEmpty(descrizone))
                {
                    if (!string.IsNullOrEmpty(Amministrazione))
                    {
                        descrizone = Amministrazione;
                    }
                    else
                    {
                        logger.Error("ATTENZIONE CASO NON GESTITO, RISOLVERE!!!");
                    }
                }
                //LULUCIANI Settembre 2011 OLD PRIMA DELLA 3.16.x   DocsPaVO.utente.Corrispondente corrispondente = BusinessLogic.Utenti.UserManager.getCorrispondenteByEmailAndDescrizione(qco.email, infoutente, reg, descrizone);
                DocsPaVO.utente.Corrispondente corrispondente = null;
                string valorechiave = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_RIC_MITT_INTEROP_BY_MAIL_DESC");
                
                
                if (!string.IsNullOrEmpty(valorechiave) && valorechiave.Equals("1"))
                {
                    //RICERCA SOLO PER MAIL and like %DESC%
                    corrispondente = BusinessLogic.Utenti.UserManager.getCorrispondenteByEmailAndDescrizione(qco.email, infoutente, reg, descrizone, out rows, codiceAmm);

                }
                else //RICERCA SOLO PER MAIL
                {
                    //corrispondente = BusinessLogic.Utenti.UserManager.getCorrispondenteByEmail(qco.email, infoutente, reg);
                    corrispondente = BusinessLogic.Utenti.UserManager.getCorrispondenteByEmailCodiceAmmCodiceAoo(qco.email, infoutente, reg, codiceAmm, codiceAOO);

                    //Se non trovo niente nel registro selezionato, cerco in tutte le rubriche visibili al ruolo
                    if (corrispondente == null || corrispondente.codiceRubrica.Contains("@") || (corrispondente.codiceRubrica.Length > 8 && corrispondente.codiceRubrica.Substring(0, 8).ToUpper().Equals("INTEROP_")))
                    {
                        Registro[] registriRuolo = (Registro[])(Utenti.RegistriManager.getListaRegistriRfRuolo(infoutente.idCorrGlobali, null, null).ToArray(typeof(Registro)));
                        DocsPaVO.utente.Corrispondente corrispondenteTemp = null;
                        foreach (var registro in registriRuolo)
                        {
                            if (!registro.systemId.Equals(reg.systemId))
                            {
                                corrispondenteTemp = BusinessLogic.Utenti.UserManager.getCorrispondenteByEmailCodiceAmmCodiceAoo(qco.email, infoutente, registro, codiceAmm, codiceAOO);
                                if (corrispondenteTemp != null && (!corrispondenteTemp.codiceRubrica.Contains("@") && (corrispondenteTemp.codiceRubrica.Length < 8 || !corrispondenteTemp.codiceRubrica.Substring(0, 8).ToUpper().Equals("INTEROP_"))))
                                {
                                    corrispondente = corrispondenteTemp;
                                    break;
                                }
                            }
                        }
                    }
                }

                //FINE MODIFICA LULUCIANI
                if (corrispondente != null)
                {
                    if (!string.IsNullOrEmpty(codiceAmm))
                        corrispondente.codDescAmministrizazione = codiceAmm + "-";

                    if (!string.IsNullOrEmpty(Amministrazione))
                        corrispondente.codDescAmministrizazione = Amministrazione + "-";
                    risultatiRicerca = new System.Collections.ArrayList() { corrispondente };
                }
                else
                    risultatiRicerca = new System.Collections.ArrayList();
            }
            else
            {
                logger.Debug("listaCorrispondentiIntMethod");
                risultatiRicerca = BusinessLogic.Utenti.addressBookManager.listaCorrispondentiIntMethod(qco);
                //risultatiRicerca=BusinessLogic.Utenti.addressBookManager.listaCorrispondentiIntInteropMethod(qco);
            }
            //			logger.addMessage("Risultati ricerca: "+risultatiRicerca.Count);
            logger.Debug("Risultati ricerca: " + risultatiRicerca.Count);

            for (int i = 0; i < risultatiRicerca.Count; i++)
            {
                if (Interoperabilità.InteroperabilitaUtils.Cfg_InteroperSegnaturaSoloUO)
                    if (tipoUtente == DocsPaVO.addressbook.TipoUtente.ESTERNO)
                        return (DocsPaVO.utente.Corrispondente)risultatiRicerca[0];
                    else if (isCorrectMittente((DocsPaVO.utente.Corrispondente)risultatiRicerca[i], elMittente))
                        return (DocsPaVO.utente.Corrispondente)risultatiRicerca[i];
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mittente"></param>
        /// <param name="elMittente"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        private static bool isCorrectMittente(DocsPaVO.utente.Corrispondente mittente, XmlElement elMittente)
        {
            //			logger.addMessage("Controllo correttezza mittente");
            logger.Debug("Controllo correttezza mittente");

            System.Collections.ArrayList uoGerarchia = new System.Collections.ArrayList();
            DocsPaVO.utente.UnitaOrganizzativa uo = null;
            if (mittente.GetType().Equals(typeof(DocsPaVO.utente.Ruolo)))
            {
                uo = ((DocsPaVO.utente.Ruolo)mittente).uo;
            }
            else
                if (mittente.GetType().Equals(typeof(DocsPaVO.utente.Utente)))
                {
                    uo = ((DocsPaVO.utente.Ruolo)((DocsPaVO.utente.Utente)mittente).ruoli[0]).uo;
                }
                else
                    if (mittente.GetType().Equals(typeof(DocsPaVO.utente.UnitaOrganizzativa)))
                    {
                        uo = (DocsPaVO.utente.UnitaOrganizzativa)mittente;
                    }
            while (uo != null)
            {
                uoGerarchia.Add(uo.descrizione);
                uo = uo.parent;
            }
            //ora si fa il matching
            string strAmm = ((string)uoGerarchia[uoGerarchia.Count - 1]).ToUpper().Trim();
            string strAmmXml = elMittente.SelectSingleNode("Amministrazione/Denominazione").InnerText.ToString().ToUpper().Trim();
            //			logger.addMessage(strAmm+" "+strAmmXml);
            logger.Debug(strAmm + " " + strAmmXml);

            if (!strAmm.Equals(strAmmXml)) return false;
            string strUo;
            string strUoXml;
            XmlElement elUo = (XmlElement)elMittente.SelectSingleNode("Amministrazione/UnitaOrganizzativa");
            for (int i = uoGerarchia.Count - 1; i >= 0; i--)
            {
                strUo = ((string)uoGerarchia[i]).ToUpper().Trim();
                //se la catena nell'XML finisce prima...
                if (elUo == null) return false;
                strUoXml = elUo.SelectSingleNode("Denominazione").InnerText.Trim().ToUpper();
                //				logger.addMessage(strUo+" "+strUoXml);
                logger.Debug(strUo + " " + strUoXml);

                //verifica delle uo tra il file xml e l'organigramma
                if (!strUo.Equals(strUoXml)) return false;
                //si sposta nella catena dell'XML
                elUo = (XmlElement)elUo.SelectSingleNode("UnitaOrganizzativa");
            }
            //se la catena nell'XML continua..
            if (elUo != null && !elUo.SelectSingleNode("Denominazione").InnerText.Trim().Equals("")) return false;
            return true;
        }

        //metodo per aggiundere in rubrica un nuovo corrispondente
        /// <summary>
        /// 
        /// </summary>
        /// <param name="elOrigine"></param>
        /// <param name="elIdentificatore"></param>
        /// <param name="reg"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        private static DocsPaVO.utente.UnitaOrganizzativa addNewCorrispondente(XmlElement elOrigine, XmlElement elIdentificatore, DocsPaVO.utente.Registro reg)
        {
            /*Emanuela: questo metodo è stato modificato affinchè inserisca solamente, l'unita organizzativa valida, ovvero che contenga
             * il tag DENOMINAZIONE popolato, più interna nell'xml.
             * Nel caso non esistano UO valide, viene inserita come UO l'amministrazione.
             * Prima veniva inserita una UO per ogni Unità organizzativa contenuta nell'XML
             * Di seguito c'è il vecchio metodo rinominato con addNewCorrispondente
             * */
            string tempCodRubr = "";
            try
            {
                //query di insert
                System.Collections.ArrayList insertList = new System.Collections.ArrayList();
                //parametri comuni
                string codiceAOO = elIdentificatore.SelectSingleNode("CodiceAOO").InnerText.Trim();
                string codiceAmm = elIdentificatore.SelectSingleNode("CodiceAmministrazione").InnerText.Trim();
                string tipoIE = "E";
                string tipoCorr = "S";
                string mailMitt = elOrigine.SelectSingleNode("IndirizzoTelematico").InnerText;
                string idParent = "0";
                bool transBegun = false;
                string codEdesc = string.Empty;
                string descAmm = string.Empty;
                DocsPaDB.Query_DocsPAWS.Utenti ut = new DocsPaDB.Query_DocsPAWS.Utenti();

                DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();

                int level = 0;
                XmlElement elUO = (XmlElement)elOrigine.SelectSingleNode("Mittente/Amministrazione/UnitaOrganizzativa");
                XmlElement uoTemp = null;
                while (elUO != null)
                {
                    if (!elUO.SelectSingleNode("Denominazione").InnerText.Trim().Equals(""))
                        uoTemp = elUO;
                    level = level + 1;
                    elUO = (XmlElement)elUO.SelectSingleNode("UnitaOrganizzativa");
                }
                elUO = uoTemp;

                if (elUO == null)
                {
                    string descrizioneAmm = elOrigine.SelectSingleNode("Mittente/Amministrazione/Denominazione").InnerText.Trim();
                    string descrInterAmm = null;
                    if (descrizioneAmm.Length >= 7)
                    {
                        descrInterAmm = descrizioneAmm.Substring(0, 7);
                    }
                    else
                    {
                        descrInterAmm = descrizioneAmm;
                    }

                    transBegun = true;
                    //modofica
                    if (!string.IsNullOrEmpty(codiceAmm))
                        codEdesc = codiceAmm + " - ";

                    if (!string.IsNullOrEmpty(descrizioneAmm))
                        codEdesc = codEdesc + descrizioneAmm + " - ";

                    string sysId = string.Empty;
                    //fine modifica
                    if (string.IsNullOrEmpty(codEdesc))
                        sysId = obj.addNewCorr(elOrigine,/*db,*/tipoIE, reg, mailMitt, descrizioneAmm, codiceAmm, tipoCorr, codiceAOO, descrInterAmm);
                    else
                        sysId = obj.addNewCorrByCodEDesc(elOrigine,/*db,*/tipoIE, reg, mailMitt, descrizioneAmm, codiceAmm, tipoCorr, codiceAOO, descrInterAmm, codEdesc);

                    string codRubricaAmm = DocsPaDB.Utils.Personalization.getInstance(reg.idAmministrazione).getInteropPrefix() + sysId;

                    //Emanuela 14/04/2014: se la descrizione nella segnatura non è specificata, mette nel campo descrizione nella dpa_corr_globali
                    //il codice rubrica(per impedire che venga inserito un valore vuoto)
                    string valorechiave = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_RIC_MITT_INTEROP_BY_MAIL_DESC");
                    bool updateDescCorr = false;
                    if (descrizioneAmm.Equals(string.Empty) && (valorechiave != null && valorechiave.Equals("0")))
                        updateDescCorr = true;
                    obj.setCodRub(codRubricaAmm, sysId, updateDescCorr);

                    //si inserisce il dettaglio su dpa_dett_globali
                    //e il canale preferenziale sulla dpa_t_canale_corr
                    ut.CompletaInserimentoCorrispPerInterop(sysId);

                    logger.Debug("Amministrazione " + descrizioneAmm + " da inserire");

                    //si setta l'id parent
                    idParent = sysId;
                    //si setta il codice rubrica temporaneo
                    tempCodRubr = DocsPaDB.Utils.Personalization.getInstance(reg.idAmministrazione).getInteropPrefix() + sysId;
                    //si risparmiano ulteriori select: non sono necessarie, le UO bisogna metterle da zero!
                    
                }
                else
                {
                    string sysId = string.Empty;
                    string descrizioneUO = "";
                    descrizioneUO = elUO.SelectSingleNode("Denominazione").InnerText.Trim();
                    //modifica gennaro
                    if (string.IsNullOrEmpty(descAmm))
                        descAmm = descrizioneUO;
                    //fine modifica gennaro
                
                    //si aggiunge nuova uo
                    string descrInterUO = null;
                    if (descrizioneUO.Length >= 7)
                    {
                        descrInterUO = descrizioneUO.Substring(0, 7);
                    }
                    else
                    {
                        descrInterUO = descrizioneUO;
                    }

                    if (!transBegun)
                    {
                        transBegun = true;
                    }

                    //modofica
                    if (!string.IsNullOrEmpty(codiceAmm))
                        codEdesc = codiceAmm + " - ";

                    if (!string.IsNullOrEmpty(descAmm))
                        codEdesc = codEdesc + descAmm + " - ";
                    sysId = string.Empty;
                    //fine modifica
                    if (string.IsNullOrEmpty(codEdesc))
                        sysId = obj.addNewUO(elUO,/*db,*/level, tipoIE, reg, mailMitt, descrizioneUO, idParent, codiceAmm, tipoCorr, codiceAOO, descrInterUO);
                    else
                        sysId = obj.addNewUOByCodEDesc(elUO,/*db,*/level, tipoIE, reg, mailMitt, descrizioneUO, idParent, codiceAmm, tipoCorr, codiceAOO, descrInterUO, codEdesc);

                    //si inserisce il codice rubrica
                    string codRubricaUO = "INTEROP_" + sysId;
                    tempCodRubr = codRubricaUO;
                    
                    obj.updCodRubr(codRubricaUO, sysId);

             
                    //si inserisce il dettaglio su dpa_dett_globali
                    //e il canale preferenziale sulla dpa_t_canale_corr
                    ut.CompletaInserimentoCorrispPerInterop(sysId);

                    logger.Debug("Unita' organizzativa " + descrizioneUO + " da inserire");

                    //si setta l'id parent estraendo system_id
                    idParent = sysId;
                    //si risparmiano ulteriori select: non sono necessarie, le UO bisogna metterle da zero!
                }

                if (transBegun)
                {
                    logger.Debug("Inserimenti eseguiti");
                }

                logger.Debug("Ricerca della UO con codRubrica=" + tempCodRubr);

                DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.ESTERNO;
                qco.codiceRubrica = tempCodRubr;

                qco.idAmministrazione = reg.idAmministrazione;
                System.Collections.ArrayList idReg = new System.Collections.ArrayList();
                idReg.Add(reg.systemId);
                qco.idRegistri = idReg;
                System.Collections.ArrayList resQuery = BusinessLogic.Utenti.addressBookManager.listaCorrispondentiEstMethod(qco);
                logger.Debug("Num ris:" + resQuery.Count);

                if (!string.IsNullOrEmpty(codiceAmm))
                    ((DocsPaVO.utente.UnitaOrganizzativa)resQuery[0]).codDescAmministrizazione = codiceAmm + " - ";

                if (!string.IsNullOrEmpty(descAmm))
                    ((DocsPaVO.utente.UnitaOrganizzativa)resQuery[0]).codDescAmministrizazione = descAmm + " - ";

                return ((DocsPaVO.utente.UnitaOrganizzativa)resQuery[0]);
            }
            catch (Exception e)
            {
                logger.Error("Errore nella gestione dell'interoperabilità. (addNewCorrispondente)", e);
                throw e;
            }
        }



        //metodo per aggiundere in rubrica un nuovo corrispondente
        /// <summary>
        /// 
        /// </summary>
        /// <param name="elOrigine"></param>
        /// <param name="elIdentificatore"></param>
        /// <param name="reg"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        private static DocsPaVO.utente.UnitaOrganizzativa addNewCorrispondente_old(XmlElement elOrigine, XmlElement elIdentificatore, DocsPaVO.utente.Registro reg)
        {
            //DocsPa_V15_Utils.DBAgent db = new DocsPa_V15_Utils.DBAgent();
            //DataSet dataSet=new DataSet();	
            DataSet dataSet;
            string tempCodRubr = "";
            try
            {
                //db.openConnection();
                //query di insert
                System.Collections.ArrayList insertList = new System.Collections.ArrayList();
                //parametri comuni
                string codiceAOO = elIdentificatore.SelectSingleNode("CodiceAOO").InnerText.Trim();
                string codiceAmm = elIdentificatore.SelectSingleNode("CodiceAmministrazione").InnerText.Trim();
                string tipoIE = "E";
                string tipoCorr = "S";
                string mailMitt = elOrigine.SelectSingleNode("IndirizzoTelematico").InnerText;
                //string dataInizio=DocsPaDbManagement.Functions.Functions.ToDate(System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", new CultureInfo("en-US") ),true);
                string idParent;
                bool noSelect = false;
                bool noInsert = false;
                bool transBegun = false;
                string codEdesc = string.Empty;
                DocsPaDB.Query_DocsPAWS.Utenti ut = new DocsPaDB.Query_DocsPAWS.Utenti();
                //ricerca amministrazione 
                #region Codice Commentato
                /*
				string ammString="SELECT SYSTEM_ID, VAR_COD_RUBRICA FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_IE='E' AND VAR_CODICE='"+codiceAmm+"' AND ID_AMM="+reg.idAmministrazione+" AND ID_REGISTRO="+reg.systemId;
				logger.Debug(ammString);
				db.fillTable(ammString,dataSet,"AMMINISTRAZIONE");
				*/
                #endregion

                DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                //obj.getCodRubr(out dataSet, codiceAmm, reg);

                ////si controlla se è stata trovata un'amministrazione e se il codice di questa non è nullo
                //if (dataSet.Tables["AMMINISTRAZIONE"].Rows.Count > 0 && !codiceAmm.Equals(""))
                //{
                //    idParent = dataSet.Tables["AMMINISTRAZIONE"].Rows[0]["SYSTEM_ID"].ToString();
                //    tempCodRubr = dataSet.Tables["AMMINISTRAZIONE"].Rows[0]["VAR_COD_RUBRICA"].ToString();
                //    noSelect = false;
                //}
                //else
                //{
                //si aggiunge una nuova amministrazione
                string descrizioneAmm = elOrigine.SelectSingleNode("Mittente/Amministrazione/Denominazione").InnerText.Trim();
                //string codiceAmm=elIdentificatore.SelectSingleNode("CodiceAmministrazione").InnerText;
                string descrInterAmm = null;
                if (descrizioneAmm.Length >= 7)
                {
                    descrInterAmm = descrizioneAmm.Substring(0, 7);
                }
                else
                {
                    descrInterAmm = descrizioneAmm;
                }

                transBegun = true;
                //string sysId =db.insertLocked(insertAmm,"DPA_CORR_GLOBALI");
                //modofica
                if (!string.IsNullOrEmpty(codiceAmm))
                    codEdesc = codiceAmm + " - ";

                if (!string.IsNullOrEmpty(descrizioneAmm))
                    codEdesc = codEdesc + descrizioneAmm + " - ";

                string sysId = string.Empty;
                //fine modifica
                if (string.IsNullOrEmpty(codEdesc))
                    sysId = obj.addNewCorr(elOrigine,/*db,*/tipoIE, reg, mailMitt, descrizioneAmm, codiceAmm, tipoCorr, codiceAOO, descrInterAmm);
                else
                    sysId = obj.addNewCorrByCodEDesc(elOrigine,/*db,*/tipoIE, reg, mailMitt, descrizioneAmm, codiceAmm, tipoCorr, codiceAOO, descrInterAmm, codEdesc);

                string codRubricaAmm = DocsPaDB.Utils.Personalization.getInstance(reg.idAmministrazione).getInteropPrefix() + sysId;
                /*
                string updateCodRubricaAmm="UPDATE DPA_CORR_GLOBALI SET VAR_COD_RUBRICA='"+codRubricaAmm+"' WHERE SYSTEM_ID="+sysId;
                db.executeNonQuery(updateCodRubricaAmm);
                */
                obj.setCodRub(codRubricaAmm, sysId, false);




                //si inserisce il dettaglio su dpa_dett_globali
                //e il canale preferenziale sulla dpa_t_canale_corr
                ut.CompletaInserimentoCorrispPerInterop(sysId);

                //					DocsPaDB.Query_DocsPAWS.Amministrazione amm = DocsPaDB.Query_DocsPAWS.Amministrazione();


                //TODO: addDettcorr addCanalicorr

                //					logger.addMessage("Amministrazione "+descrizioneAmm+" da inserire");
                logger.Debug("Amministrazione " + descrizioneAmm + " da inserire");

                //si setta l'id parent
                idParent = sysId;
                //si setta il codice rubrica temporaneo
                tempCodRubr = DocsPaDB.Utils.Personalization.getInstance(reg.idAmministrazione).getInteropPrefix() + sysId;
                //si risparmiano ulteriori select: non sono necessarie, le UO bisogna metterle da zero!
                noSelect = true;
                //}

                //inserimento UO
                XmlElement elUO = (XmlElement)elOrigine.SelectSingleNode("Mittente/Amministrazione/UnitaOrganizzativa");
                string descrizioneUO = "";
                int level = 0;
                string descAmm = string.Empty;
                while (elUO != null && !elUO.SelectSingleNode("Denominazione").InnerText.Trim().Equals(""))
                {
                    descrizioneUO = elUO.SelectSingleNode("Denominazione").InnerText.Trim();
                    //modifica gennaro
                    if (string.IsNullOrEmpty(descAmm))
                        descAmm = descrizioneUO;
                    //fine modifica gennaro
                    level = level + 1;
                    if (noSelect == false)
                    {
                        #region Codice Commentato
                        //controlliamo se la UO esiste
                        /*
                        string uoString="SELECT SYSTEM_ID, VAR_COD_RUBRICA FROM DPA_CORR_GLOBALI WHERE VAR_DESC_CORR='"+descrizioneUO+"' AND ID_AMM="+reg.idAmministrazione+" AND CHA_TIPO_IE='E' AND CHA_TIPO_URP='U' AND ID_PARENT="+idParent;
                        logger.Debug(uoString);
                        db.fillTable(uoString,dataSet,"UO");
                        */
                        #endregion

                        obj.getExistUo(out dataSet, descrizioneUO, reg, idParent);

                        if (dataSet.Tables["UO"].Rows.Count > 0)
                        {
                            //si setta idParent
                            idParent = dataSet.Tables["UO"].Rows[0]["SYSTEM_ID"].ToString();
                            //si setta il cod rubrica temporaneo
                            tempCodRubr = dataSet.Tables["UO"].Rows[0]["VAR_COD_RUBRICA"].ToString();

                            //resettiamo la tabella delle UO
                            dataSet.Tables["UO"].Reset();
                            //si risparmia l'inserimento
                            noInsert = true;
                        }
                        else
                        {
                            noInsert = false;
                        }
                    }
                    if (noInsert == false)
                    {
                        //si aggiunge nuova uo
                        string descrInterUO = null;
                        if (descrizioneUO.Length >= 7)
                        {
                            descrInterUO = descrizioneUO.Substring(0, 7);
                        }
                        else
                        {
                            descrInterUO = descrizioneUO;
                        }

                        if (!transBegun)
                        {
                            //db.beginTransaction();
                            transBegun = true;
                        }
                        //string sysId=db.insertLocked(insertUO,"DPA_CORR_GLOBALI");

                        //modofica
                        if (!string.IsNullOrEmpty(codiceAmm))
                            codEdesc = codiceAmm + " - ";

                        if (!string.IsNullOrEmpty(descAmm))
                            codEdesc = codEdesc + descAmm + " - ";
                        sysId = string.Empty;
                        //fine modifica
                        if (string.IsNullOrEmpty(codEdesc))
                            sysId = obj.addNewUO(elUO,/*db,*/level, tipoIE, reg, mailMitt, descrizioneUO, idParent, codiceAmm, tipoCorr, codiceAOO, descrInterUO);
                        else
                            sysId = obj.addNewUOByCodEDesc(elUO,/*db,*/level, tipoIE, reg, mailMitt, descrizioneUO, idParent, codiceAmm, tipoCorr, codiceAOO, descrInterUO, codEdesc);

                        //si inserisce il codice rubrica
                        string codRubricaUO = "INTEROP_" + sysId;
                        tempCodRubr = codRubricaUO;

                        #region Codice Commentato
                        /*
						string updateCodRubricaUO="UPDATE DPA_CORR_GLOBALI SET VAR_COD_RUBRICA='"+codRubricaUO+"' WHERE SYSTEM_ID="+sysId;
						db.executeNonQuery(updateCodRubricaUO);
						*/
                        #endregion

                        obj.updCodRubr(codRubricaUO, sysId);

                        //si inserisce il dettaglio su dpa_dett_globali
                        //e il canale preferenziale sulla dpa_t_canale_corr
                        ut.CompletaInserimentoCorrispPerInterop(sysId);

                        //TODO: addDettcorr addCanalicorr

                        //						logger.addMessage("Unita' organizzativa "+descrizioneUO+" da inserire");
                        logger.Debug("Unita' organizzativa " + descrizioneUO + " da inserire");

                        //si setta l'id parent estraendo system_id
                        idParent = sysId;
                        //si risparmiano ulteriori select: non sono necessarie, le UO bisogna metterle da zero!
                        noSelect = true;
                    }

                    //si punta l'altra UO nell'XML
                    elUO = (XmlElement)elUO.SelectSingleNode("UnitaOrganizzativa");
                }

                if (transBegun)
                {
                    //db.commitTransaction();
                    //					logger.addMessage("Inserimenti eseguiti");
                    logger.Debug("Inserimenti eseguiti");
                }

                //si restituisce la UO cercandola nell'addressbook
                //				logger.addMessage("Ricerca della UO con codRubrica="+tempCodRubr);
                logger.Debug("Ricerca della UO con codRubrica=" + tempCodRubr);

                DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.ESTERNO;
                qco.codiceRubrica = tempCodRubr;
                qco.idAmministrazione = reg.idAmministrazione;
                System.Collections.ArrayList idReg = new System.Collections.ArrayList();
                idReg.Add(reg.systemId);
                qco.idRegistri = idReg;
                System.Collections.ArrayList resQuery = BusinessLogic.Utenti.addressBookManager.listaCorrispondentiEstMethod(qco);
                //				logger.addMessage("Num ris:"+resQuery.Count);
                logger.Debug("Num ris:" + resQuery.Count);

                if (!string.IsNullOrEmpty(codiceAmm))
                    ((DocsPaVO.utente.UnitaOrganizzativa)resQuery[0]).codDescAmministrizazione = codiceAmm + " - ";

                if (!string.IsNullOrEmpty(descAmm))
                    ((DocsPaVO.utente.UnitaOrganizzativa)resQuery[0]).codDescAmministrizazione = descAmm + " - ";

                return ((DocsPaVO.utente.UnitaOrganizzativa)resQuery[0]);
            }
            catch (Exception e)
            {
                //db.rollbackTransaction();
                logger.Error("Errore nella gestione dell'interoperabilità. (addNewCorrispondente)", e);
                throw e;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elUO"></param>
        /// <returns></returns>
        private static string getPA(XmlElement elUO)
        {
            XmlNode elUoInt = elUO.SelectSingleNode("UnitaOrganizzativa");
            if (elUoInt == null)
            {
                //non ci sono ruoli interni, non ci sono unità interne. 
                return "1";
            }
            else
            {
                XmlNode elDenUo = elUoInt.SelectSingleNode("Denominazione");
                if (elDenUo.InnerText.Trim().Equals(""))
                {
                    return "1";
                }
                else
                {
                    return "0";
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elUO"></param>
        /// <returns></returns>
        private static bool isLastUO(XmlElement elUO)
        {
            XmlNode elUoInt = elUO.SelectSingleNode("UnitaOrganizzativa");
            if (elUoInt == null)
            {
                //non ci sono unità interne. 
                return true;
            }
            else
            {
                XmlNode elDenUo = elUoInt.SelectSingleNode("Denominazione");
                if (elDenUo.InnerText.Trim().Equals(""))
                {
                    return true;
                }
                else
                {
                    return false;
                };
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elIntestazione"></param>
        /// <param name="confermaRic"></param>
        /// <param name="mailAddress"></param>
        /// <returns></returns>
        private static string getInfoDestinatari(XmlElement elIntestazione, ref bool confermaRic, string mailAddress)
        {
            XmlNodeList destinazioni = elIntestazione.SelectNodes("Destinazione");
            logger.Debug("destinazioni:" + destinazioni);
            string result = "";
            for (int i = 0; i < destinazioni.Count; i++)
            {
                XmlElement destinazione = (XmlElement)destinazioni.Item(i);
                //controllo sulla conferma ricezione
                // logger.Debug("destinazione (i):" + destinazione);
                if (destinazione.SelectSingleNode("IndirizzoTelematico").InnerText.ToUpper().Trim().Equals(mailAddress.ToUpper()))
                {
                    if (destinazione.Attributes["confermaRicezione"] != null)
                    {
                        string conferma = destinazione.Attributes["confermaRicezione"].Value;
                        //  logger.Debug("conferma (i):" + conferma);
                        if (conferma != null && conferma.ToUpper().Equals("SI")) confermaRic = true;
                    }
                }
                result = result + "DESTINAZIONE " + (i + 1) + " ";
                result = result + "Indirizzo: " + destinazione.SelectSingleNode("IndirizzoTelematico").InnerText.ToUpper().Trim() + " ";
                // logger.Debug("Indirizzo (i):" + result);
                XmlNodeList destinatari = destinazione.SelectNodes("Destinatario");
                if (destinatari.Count > 0)
                {
                    result = result + "Destinatari: ";
                }
                for (int j = 0; j < destinatari.Count; j++)
                {
                    XmlElement destinatario = (XmlElement)destinatari.Item(j);
                    // logger.Debug("destinatario (i):" + destinatario);
                    if (destinatario.SelectSingleNode("Amministrazione") != null)
                    {
                        result = result + destinatario.SelectSingleNode("Amministrazione/Denominazione").InnerText.Trim() + " ";
                        //    logger.Debug("Amministrazione (i):" + result);
                    }
                    if (destinatario.SelectSingleNode("Denominazione") != null)
                    {

                        result = result + destinatario.SelectSingleNode("Denominazione").InnerText.Trim() + " ";
                        // logger.Debug("Denominazione (i):" + result);
                    }
                    XmlNodeList persone = destinatario.SelectNodes("Persona");
                    for (int k = 0; k < persone.Count; k++)
                    {
                        if (persone.Item(k).SelectSingleNode("Cognome") != null && !persone.Item(k).SelectSingleNode("Cognome").Equals(""))
                            result = result + persone.Item(k).SelectSingleNode("Cognome").InnerText.Trim() + " ";
                        else if (persone.Item(k).SelectSingleNode("Denominazione") != null)
                            result = result + persone.Item(k).SelectSingleNode("Denominazione").InnerText.Trim() + " ";
                        //   logger.Debug("persone (i):" + result);
                    }
                };
                result = result + "/ ";
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.Applicazione getApp(string filename)
        {
            logger.Debug("getApp");

            char[] dot = { '.' };
            string[] parts = filename.Split(dot);
            string suffix = parts[parts.Length - 1];
            if (suffix.ToUpper().Equals("P7M"))
            {
                logger.Debug("File p7m");
                //suffix=filename.Substring(filename.IndexOf(".")+1);
            }

            logger.Debug("Suffisso:" + suffix);
            DocsPaVO.documento.Applicazione res = null;
            try
            {
                System.Collections.ArrayList apps = BusinessLogic.Documenti.FileManager.getApplicazioni(suffix);
                //					logger.addMessage("App:"+apps.Count);
                logger.Debug("App:" + apps.Count);




                if (apps.Count > 0)
                {
                    res = (DocsPaVO.documento.Applicazione)apps[0];
                    //						logger.addMessage(res.estensione);
                    logger.Debug(res.estensione);
                }
                return res;
            }
            catch (Exception e)
            {
                logger.Debug(e.ToString());
                logger.Error("Errore nella gestione dell'interoperabilità. (getApp)", e);
                throw e;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string getFileName(string fileName)
        {
            logger.Debug("getFileName");
            string res = null;
            if (fileName.Substring(fileName.LastIndexOf(".") + 1).ToUpper().Equals("P7M"))
            {
                res = fileName.Substring(0, fileName.LastIndexOf("."));
            }
            else
            {
                res = fileName;
            }
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="sd"></param>
        /// <param name="noteGenerali"></param>
        /// <param name="reg"></param>
        /// <param name="infoUtente"></param>
        /// <param name="ruolo"></param>
        /// <param name="logger"></param>
        private static void eseguiTrasmissione(string idPeople, string serverName, DocsPaVO.documento.SchedaDocumento sd, string noteGenerali, DocsPaVO.utente.Registro reg, DocsPaVO.utente.Ruolo ruolo)
        {
            //DocsPa_V15_Utils.DBAgent db=new DocsPa_V15_Utils.DBAgent();
            try
            {
                DocsPaVO.trasmissione.Trasmissione trasm = new DocsPaVO.trasmissione.Trasmissione();
                trasm.ruolo = ruolo;
                //db.openConnection();
                trasm.utente = BusinessLogic.Utenti.UserManager.getUtente(idPeople);
                //db.closeConnection();
                trasm.noteGenerali = noteGenerali;
                DocsPaVO.documento.InfoDocumento infoDoc = new DocsPaVO.documento.InfoDocumento();
                infoDoc.idProfile = sd.systemId;
                infoDoc.docNumber = sd.docNumber;
                infoDoc.oggetto = sd.oggetto.descrizione;
                infoDoc.tipoProto = "A";
                infoDoc.idRegistro = reg.systemId;
                trasm.infoDocumento = infoDoc;
                //costruzione singole trasmissioni
                DocsPaVO.trasmissione.RagioneTrasmissione ragione = getRagioneTrasm(ruolo.idAmministrazione, "I");
                //				logger.addMessage("RAGIONE :"+ragione.tipo+" "+ragione.tipoDestinatario);
                logger.Debug("RAGIONE :" + ragione.tipo + " " + ragione.tipoDestinatario);

                System.Collections.ArrayList ruoliDest = getRuoliDestTrasm(reg,string.Empty);
                System.Collections.ArrayList trasmissioniSing = new System.Collections.ArrayList();
                if (ruoliDest.Count > 0)
                {
                    for (int i = 0; i < ruoliDest.Count; i++)
                    {
                        //						logger.addMessage("Aggiunta trasmissione singola");
                        logger.Debug("Aggiunta trasmissione singola");

                        DocsPaVO.trasmissione.TrasmissioneSingola trSing = new DocsPaVO.trasmissione.TrasmissioneSingola();
                        trSing.ragione = ragione;
                        trSing.corrispondenteInterno = (DocsPaVO.utente.Ruolo)ruoliDest[i];
                        trSing.tipoTrasm = "S";
                        trSing.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;
                        //ricerca degli utenti del ruolo
                        System.Collections.ArrayList utenti = new System.Collections.ArrayList();
                        //						logger.addMessage("Ricerca utenti del ruolo per codice e registro");
                        logger.Debug("Ricerca utenti del ruolo per codice e registro");

                        DocsPaVO.addressbook.QueryCorrispondente qc = new DocsPaVO.addressbook.QueryCorrispondente();
                        qc.codiceRubrica = ((DocsPaVO.utente.Ruolo)ruoliDest[i]).codiceRubrica;
                        System.Collections.ArrayList registri = new System.Collections.ArrayList();
                        registri.Add(reg.systemId);
                        qc.idRegistri = registri;
                        qc.idAmministrazione = reg.idAmministrazione;
                        qc.getChildren = true;
                        utenti = BusinessLogic.Utenti.addressBookManager.listaCorrispondentiIntMethod(qc);
                        System.Collections.ArrayList trasmissioniUt = new System.Collections.ArrayList();
                        for (int k = 0; k < utenti.Count; k++)
                        {
                            //							logger.addMessage("aggiunta trasmissione utente");
                            logger.Debug("aggiunta trasmissione utente");

                            DocsPaVO.trasmissione.TrasmissioneUtente trUt = new DocsPaVO.trasmissione.TrasmissioneUtente();
                            trUt.utente = (DocsPaVO.utente.Utente)utenti[k];
                            trasmissioniUt.Add(trUt);
                        }
                        trSing.trasmissioneUtente = trasmissioniUt;
                        trasmissioniSing.Add(trSing);
                    }
                    trasm.trasmissioniSingole = trasmissioniSing;
                    //BusinessLogic.Trasmissioni.TrasmManager.saveTrasmMethod(trasm);
                    ////					logger.addMessage("Trasmissione salvata");
                    //logger.Debug("Trasmissione salvata");

                    //BusinessLogic.Trasmissioni.ExecTrasmManager.executeTrasmMethod(serverName, trasm);
                    ////					logger.addMessage("Trasmissione eseguita");
                    //logger.Debug("Trasmissione eseguita");
                    BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(serverName, trasm);
                    logger.Debug("Trasmissione salvata ed eseguita");
                }
                else
                {
                    //					logger.addMessage("Trasmissione non eseguita per mancanza di ruoli destinatari");
                    logger.Debug("Trasmissione non eseguita per mancanza di ruoli destinatari");
                }
            }
            catch (Exception e)
            {
                //db.closeConnection();
                logger.Error("Errore nella gestione dell'interoperabilità. (eseguiTrasmissione)", e);
                throw e;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="sd"></param>
        /// <param name="noteGenerali"></param>
        /// <param name="reg"></param>
        /// <param name="infoUtente"></param>
        /// <param name="ruolo"></param>
        /// <param name="logger"></param>
        /// <param name="dst"></param>
        private static void eseguiTrasmissione(string idPeople, string serverName, DocsPaVO.documento.SchedaDocumento sd, string noteGenerali, DocsPaVO.utente.Registro reg, DocsPaVO.utente.Ruolo ruolo, string dst, string mailAddress, InfoUtente infoUtente = null , Corrispondente recipient = null)
        {
            //DocsPa_V15_Utils.DBAgent db=new DocsPa_V15_Utils.DBAgent();
            try
            {
                // Per gestione pendenti tramite PEC
                bool MailPendente = InteroperabilitaUtils.MantieniMailRicevutePendenti(reg.systemId, mailAddress);
                    
                DocsPaVO.trasmissione.Trasmissione trasm = new DocsPaVO.trasmissione.Trasmissione();
                trasm.ruolo = ruolo;
                //db.openConnection();
                trasm.utente = BusinessLogic.Utenti.UserManager.getUtente(idPeople);
                trasm.utente.dst = dst;//aggiunto dst il 27/10/2005 per errore in HM
                //db.closeConnection();
                trasm.noteGenerali = noteGenerali;
                DocsPaVO.documento.InfoDocumento infoDoc = new DocsPaVO.documento.InfoDocumento();
                infoDoc.idProfile = sd.systemId;
                infoDoc.docNumber = sd.docNumber;
                infoDoc.oggetto = sd.oggetto.descrizione;
                infoDoc.tipoProto = "A";
                infoDoc.idRegistro = reg.systemId;
                trasm.infoDocumento = infoDoc;
                //costruzione singole trasmissioni
                DocsPaVO.trasmissione.RagioneTrasmissione ragione = getRagioneTrasm(ruolo.idAmministrazione, "I");
                //				logger.addMessage("RAGIONE :"+ragione.tipo+" "+ragione.tipoDestinatario);
                logger.Debug("RAGIONE :" + ragione.tipo + " " + ragione.tipoDestinatario);

                // S. Furnari - 16/01/2013 - Sviluppo trasmissione documento ricevuto per interop interno solo a ruoli nella UO
                // destinataria della spedizione e non a tutta la AOO.
                //System.Collections.ArrayList ruoliDest = getRuoliDestTrasm(reg, mailAddress);
                System.Collections.ArrayList ruoliDest = null;
                // Se il destinatario è interno, è attiva l'interoperabilità interna, ed è attiva la funzionalità
                // di tramimssione solo ai ruoli nella UO destinataria della spedizione, devono essere ricercati
                // solo i ruoli in "recipient" che, nel caso in cui il destinatario originale sia un ruolo, sarà valorizzata
                // con la UO in cui è definito il ruolo.
                if (recipient != null && InteroperabilitaSegnatura.IsEnabledSelectiveTransmission(recipient.idAmministrazione))
                    ruoliDest = new System.Collections.ArrayList(GetRecipients(reg, recipient.systemId));
                else
                    ruoliDest = getRuoliDestTrasm(reg, mailAddress);

               //commentato furnari System.Collections.ArrayList ruoliDest = getRuoliDestTrasm(reg, mailAddress);
               
                //if (MailPendente)
                //{
                //    bool found = false;
                //    foreach (DocsPaVO.utente.Ruolo r in ruoliDest)
                //    {                        
                //        if (r.systemId == ruolo.systemId)
                //        {
                //            found = true;
                //        }

                //    }
                //    ruoliDest.Clear();
                //    if (found) ruoliDest.Add(ruolo);
                //}
                System.Collections.ArrayList trasmissioniSing = new System.Collections.ArrayList();
                if (ruoliDest.Count > 0)
                {
                    for (int i = 0; i < ruoliDest.Count; i++)
                    {
                        //						logger.addMessage("Aggiunta trasmissione singola");
                        logger.Debug("Aggiunta trasmissione singola");

                        DocsPaVO.trasmissione.TrasmissioneSingola trSing = new DocsPaVO.trasmissione.TrasmissioneSingola();
                        trSing.ragione = ragione;
                        trSing.corrispondenteInterno = (DocsPaVO.utente.Ruolo)ruoliDest[i];
                        // S. Furnari - 18/01/2013 - Se recipient è valorizzato e quindi la spedizione è per interop interna, 
                        // il tipo di trasmissione è determinato dal valore della chiave TRASM_UNO_TUTTI_INTEROP
                        //trSing.tipoTrasm =  "S";
                        trSing.tipoTrasm = recipient != null ? InteroperabilitaSegnatura.GetInteropTrasmType(recipient.idAmministrazione) : "S";
    
                        //furnari trSing.tipoTrasm = "S";
                        trSing.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;
                        if (MailPendente)
                        {
                            trSing.ragione.eredita = "0";
                        }

                        //ricerca degli utenti del ruolo
                        System.Collections.ArrayList utenti = new System.Collections.ArrayList();
                        //						logger.addMessage("Ricerca utenti del ruolo per codice e registro");
                        logger.Debug("Ricerca utenti del ruolo per codice e registro");

                        DocsPaVO.addressbook.QueryCorrispondente qc = new DocsPaVO.addressbook.QueryCorrispondente();
                        qc.codiceRubrica = ((DocsPaVO.utente.Ruolo)ruoliDest[i]).codiceRubrica;
                        System.Collections.ArrayList registri = new System.Collections.ArrayList();
                        registri.Add(reg.systemId);
                        qc.idRegistri = registri;
                        qc.idAmministrazione = reg.idAmministrazione;
                        qc.getChildren = true;

                        //LULUCIANI 28/07/2009 
                        qc.fineValidita = true;

                        utenti = BusinessLogic.Utenti.addressBookManager.listaCorrispondentiIntMethod(qc);
                        System.Collections.ArrayList trasmissioniUt = new System.Collections.ArrayList();
                        for (int k = 0; k < utenti.Count; k++)
                        {
                            //							logger.addMessage("aggiunta trasmissione utente");
                            logger.Debug("aggiunta trasmissione utente");

                            DocsPaVO.trasmissione.TrasmissioneUtente trUt = new DocsPaVO.trasmissione.TrasmissioneUtente();
                            trUt.utente = (DocsPaVO.utente.Utente)utenti[k];
                            trasmissioniUt.Add(trUt);
                        }
                        trSing.trasmissioneUtente = trasmissioniUt;
                        trasmissioniSing.Add(trSing);
                    }
                    trasm.trasmissioniSingole = trasmissioniSing;
                    //BusinessLogic.Trasmissioni.TrasmManager.saveTrasmMethod(trasm);
                    ////					logger.addMessage("Trasmissione salvata");
                    //logger.Debug(" ");

                    //BusinessLogic.Trasmissioni.ExecTrasmManager.executeTrasmMethod(serverName, trasm);
                    ////					logger.addMessage("Trasmissione eseguita");
                    //logger.Debug("Trasmissione eseguita");
                    DocsPaVO.trasmissione.Trasmissione result = null;
                    string desc = string.Empty;
                    string method;
                    result = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(serverName, trasm);
                    logger.Debug("Trasmissione salvata ed eseguita");

                    if (result != null)
                    {
                    // LOG per documento
                        foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in result.trasmissioniSingole)
                        {
                            method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                            if (result.infoDocumento.segnatura == null)
                                desc = "Trasmesso Documento ID: " + result.infoDocumento.docNumber.ToString();
                            else
                                desc = "Trasmesso Documento ID: " + result.infoDocumento.segnatura.ToString();
                            BusinessLogic.UserLog.UserLog.WriteLog(result.utente.userId, result.utente.idPeople, result.ruolo.idGruppo, result.utente.idAmministrazione, method, result.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK,
                                (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "1", single.systemId);
                        }
                    }
                }
                else
                {
                    //					logger.addMessage("Trasmissione non eseguita per mancanza di ruoli destinatari");
                    logger.Debug("Trasmissione non eseguita per mancanza di ruoli destinatari");
                }
            }
            catch (Exception e)
            {
                //db.closeConnection();
                logger.Error("Errore nella gestione dell'interoperabilità. (eseguiTrasmissione)", e);
                throw e;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DocsPaVO.trasmissione.RagioneTrasmissione getRagioneTrasm(string idAmm, String tipoRagione)
        {
            logger.Debug("getRagioneTrasm");
            //DocsPa_V15_Utils.Database db=DocsPa_V15_Utils.dbControl.getDatabase();
            //DataSet ds=new DataSet();
            DataSet ds;
            DocsPaVO.trasmissione.RagioneTrasmissione rt = new DocsPaVO.trasmissione.RagioneTrasmissione();
            try
            {
                #region Codice Commentato
                /*
				string queryString="SELECT * FROM DPA_RAGIONE_TRASM WHERE CHA_TIPO_RAGIONE='N' AND CHA_TIPO_DIRITTI='W'";
				db.fillTable(queryString,ds,"RAGIONE");
				*/
                #endregion

                DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                obj.getRagTrasm(out ds, idAmm, tipoRagione);

                DataRow ragione = ds.Tables["RAGIONE"].Rows[0];
                rt.descrizione = ragione["VAR_DESC_RAGIONE"].ToString();
                rt.risposta = ragione["CHA_RISPOSTA"].ToString();
                rt.systemId = ragione["SYSTEM_ID"].ToString();
                rt.tipo = "N";
                rt.tipoDestinatario = (DocsPaVO.trasmissione.TipoGerarchia)DocsPaDB.Utils.HashTableManager.GetKeyFromValue(DocsPaVO.trasmissione.RagioneTrasmissione.tipoGerarchiaStringa, ragione["CHA_TIPO_DEST"].ToString());
                rt.tipoDiritti = DocsPaVO.trasmissione.TipoDiritto.WRITE;
                rt.eredita = ragione["CHA_EREDITA"].ToString();
            }
            catch (Exception e)
            {
                logger.Debug(e.ToString());
                //db.closeConnection();
                logger.Error("Errore nella gestione dell'interoperabilità. (getRagioneTrasm)", e);
                throw e;
            }
            return rt;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        private static System.Collections.ArrayList getRuoliDestTrasm(DocsPaVO.utente.Registro reg, string mailAddress)
        {
            logger.Debug("getRuoliDestTrasm");
            //DocsPa_V15_Utils.Database db=DocsPa_V15_Utils.dbControl.getDatabase();
            DataSet ds = new DataSet();
            System.Collections.ArrayList ruoliDestTrasm = new System.Collections.ArrayList();
            try
            {
                #region Codice Commentato
                //db.openConnection();
                /*
                string queryString="SELECT A.SYSTEM_ID, A.VAR_CODICE, A.VAR_COD_RUBRICA, D.VAR_DESC_RUOLO, A.ID_GRUPPO, D.NUM_LIVELLO, E.VAR_DESC_CORR FROM DPA_CORR_GLOBALI A, DPA_TIPO_F_RUOLO B,DPA_TIPO_FUNZIONE C,DPA_TIPO_RUOLO D,DPA_CORR_GLOBALI E, DPA_L_RUOLO_REG F  WHERE A.CHA_TIPO_URP='R' AND F.ID_REGISTRO="+reg.systemId+" AND B.ID_RUOLO_IN_UO=A.SYSTEM_ID AND B.ID_TIPO_FUNZ=C.SYSTEM_ID AND C.VAR_COD_TIPO='PRAU' AND D.SYSTEM_ID=A.ID_TIPO_RUOLO AND E.SYSTEM_ID=A.ID_UO AND F.ID_RUOLO_IN_UO=A.SYSTEM_ID";
                db.fillTable(queryString,ds,"RUOLI");
                */
                #endregion

                DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                obj.getCorrRuoloFun(out ds, reg,mailAddress);

                for (int i = 0; i < ds.Tables["RUOLI"].Rows.Count; i++)
                {
                    DataRow ruoloRow = ds.Tables["RUOLI"].Rows[i];
                    DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                    ruolo.systemId = ruoloRow["SYSTEM_ID"].ToString();
                    ruolo.codiceCorrispondente = ruoloRow["VAR_CODICE"].ToString();
                    ruolo.codiceRubrica = ruoloRow["VAR_COD_RUBRICA"].ToString();
                    ruolo.descrizione = ruoloRow["VAR_DESC_RUOLO"].ToString() + " " + ruoloRow["VAR_DESC_CORR"].ToString();
                    ruolo.livello = ruoloRow["NUM_LIVELLO"].ToString();
                    ruolo.idGruppo = ruoloRow["ID_GRUPPO"].ToString();
                    DocsPaVO.utente.UnitaOrganizzativa uoDest = new DocsPaVO.utente.UnitaOrganizzativa();
                    ruolo.uo = uoDest;
                    ruolo.uo.systemId = ruoloRow["ID_UO"].ToString();
                    ruolo.tipoCorrispondente = "R";
                    ruoliDestTrasm.Add(ruolo);
                }
            }
            catch (Exception e)
            {
                logger.Debug(e.ToString());
                //db.closeConnection();
                logger.Error("Errore nella gestione dell'interoperabilità. (getRuoliDestTrasm)", e);
                throw e;
            }
            return ruoliDestTrasm;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mittente"></param>
        /// <param name="mailMittente"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        private static bool updateUOMittente(DocsPaVO.utente.Corrispondente mittente, string mailMittente)
        {
            //			logger.addMessage("updateUOMittente");
            logger.Debug("updateUOMittente");

            //DocsPa_V15_Utils.Database db=DocsPa_V15_Utils.dbControl.getDatabase();
            //si trova la UO del mittente
            try
            {
                //db.openConnection();
                DocsPaVO.utente.UnitaOrganizzativa uoMitt = new DocsPaVO.utente.UnitaOrganizzativa();
                if (mittente.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa)) uoMitt = (DocsPaVO.utente.UnitaOrganizzativa)mittente;
                if (mittente.GetType() == typeof(DocsPaVO.utente.Ruolo)) uoMitt = ((DocsPaVO.utente.Ruolo)mittente).uo;
                if (mittente.GetType() == typeof(DocsPaVO.utente.Utente)) uoMitt = ((DocsPaVO.utente.Ruolo)((DocsPaVO.utente.Utente)mittente).ruoli[0]).uo;

                #region Codice Commentato
                /*string updateString="UPDATE DPA_CORR_GLOBALI SET VAR_EMAIL='"+mailMittente+"', CHA_PA='1' WHERE SYSTEM_ID="+uoMitt.systemId;
				logger.addMessage(updateString);
				db.executeNonQuery(updateString);
				*/
                #endregion

                DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                obj.updUOMitt(uoMitt, mailMittente);

                return true;
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
                //db.closeConnection();
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mittente"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        private static bool updateCanalePref(DocsPaVO.utente.Corrispondente mittente)
        {
            logger.Debug("updateCanalePref");
            //DocsPa_V15_Utils.DBAgent db=new DocsPa_V15_Utils.DBAgent();
            //System.Data.DataSet ds=new System.Data.DataSet();
            System.Data.DataSet ds = new DataSet();
            try
            {
                //db.openConnection();
                DocsPaVO.utente.UnitaOrganizzativa uoMitt = new DocsPaVO.utente.UnitaOrganizzativa();
                if (mittente.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa)) uoMitt = (DocsPaVO.utente.UnitaOrganizzativa)mittente;
                if (mittente.GetType() == typeof(DocsPaVO.utente.Ruolo)) uoMitt = ((DocsPaVO.utente.Ruolo)mittente).uo;
                if (mittente.GetType() == typeof(DocsPaVO.utente.Utente)) uoMitt = ((DocsPaVO.utente.Ruolo)((DocsPaVO.utente.Utente)mittente).ruoli[0]).uo;
                DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                obj.updCanalPref(ds, uoMitt/*,db*/);

                return true;
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());

                //db.rollbackTransaction();
                //db.closeConnection();
                return false;
            }
        }

        private static string getRiferimentiMittente(XmlDocument doc)
        {
            XmlNodeList ContestiProcedurali = doc.DocumentElement.SelectNodes("Riferimenti/ContestoProcedurale");
            string riferimentoMittente = string.Empty;
            //Verifico che tipo di contesto procedurale esaminare
            for (int i = 0; i < ContestiProcedurali.Count; i++)
            {
                //Contesto Procedurale Carabilnieri
                if (ContestiProcedurali[i].SelectSingleNode("TipoContestoProcedurale").InnerText == "Protocollo Arma")
                {
                    if (!string.IsNullOrEmpty(ContestiProcedurali[i].SelectSingleNode("Identificativo").InnerText))
                    {
                        riferimentoMittente = ContestiProcedurali[i].SelectSingleNode("Identificativo").InnerText + "$CC";
                    }

                }
                //Contesto Procedurale Generale
                if (ContestiProcedurali[i].SelectSingleNode("TipoContestoProcedurale").InnerText == "Codice Classifica")
                {
                    if (!string.IsNullOrEmpty(ContestiProcedurali[i].SelectSingleNode("Identificativo").InnerText))
                    {
                        riferimentoMittente = ContestiProcedurali[i].SelectSingleNode("Identificativo").InnerText;
                    }
                }
            }
            return riferimentoMittente;
        }


        public static
     bool eseguiSegnaturaProtocollazione(string serverName, string mailAddress, string filepath, string filename, DocsPaVO.utente.Registro reg, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, IInteropSchedaDocHandler handler, string mailId, CMMsg mail, string isPec, out string err, out string docnumber, string nomeMail, string dataRicezione)
        {
            System.IO.FileStream fs = null;
            System.IO.FileStream fsAll = null;
            DocsPaVO.documento.SchedaDocumento sd = null;
            err = "";
            docnumber = string.Empty;
            bool daAggiornareUffRef = false;
            //creazione del doc con trattamento spazi bianchi
            XmlDocument doc = new XmlDocument();
            InteropResolver my = new InteropResolver();
            XmlTextReader xtr = new XmlTextReader(filepath + "\\" + filename);
            xtr.WhitespaceHandling = WhitespaceHandling.None;
            XmlValidatingReader xvr = new XmlValidatingReader(xtr);
            xvr.ValidationType = System.Xml.ValidationType.DTD;
            xvr.EntityHandling = System.Xml.EntityHandling.ExpandCharEntities;
            xvr.XmlResolver = my;
            bool codint1 = false;
            try
            {

                doc.Load(xvr);
            }
            catch (System.Xml.Schema.XmlSchemaException e)
            {
                //logger.addMessage("La mail viene sospesa perche' il  file segnatura.xml non e' valido. Eccezione:"+e.Message); 
                logger.Error("La mail viene sospesa perche' il  file segnatura.xml non e' valido. Eccezione:" + e.Message);

                //********************************
                //** ATTENZIONE !!!!  ************
                //** NON RIMUOVERE (CODINTEROP2)**
                //** dal msg d'errore.          **
                //********************************
                err = "( CODINTEROP2 ) La mail viene sospesa perche' il  file segnatura.xml non e' valido. Eccezione:" + e.Message;

                if (InteroperabilitaUtils.MailElaborata(mailId, "D"))
                {
                    //logger.addMessage("Sospensione eseguita");
                    logger.Debug("Sospensione eseguita");
                }
                else
                {
                    //logger.addMessage("Sospensione non eseguita");
                    logger.Debug("Sospensione non eseguita");
                };
                return false;
            }
            catch (Exception e)
            {
                //				logger.addMessage("La mail viene sospesa. Eccezione:"+e.Message); 
                logger.Error("La mail viene sospesa. Eccezione:" + e.Message);
                err = "La mail viene sospesa. Eccezione:" + e.Message;
                if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
                {
                    //					logger.addMessage("Sospensione eseguita");
                    logger.Debug("Sospensione eseguita");
                }
                else
                {
                    //					logger.addMessage("Sospensione non eseguita");
                    logger.Debug("Sospensione non eseguita");
                };

                return false;
            }
            finally
            {
                xvr.Close();
                xtr.Close();
            }
            try
            {
                XmlElement elIdentificatore = (XmlElement)doc.DocumentElement.SelectSingleNode("Intestazione/Identificatore");
                XmlElement elmailOrigine = ((XmlElement)doc.DocumentElement.SelectSingleNode("Intestazione/Origine"));
                string mailOrigine = elmailOrigine.SelectSingleNode("IndirizzoTelematico").InnerText.Trim();
                string codiceAmministrazione = elIdentificatore.SelectSingleNode("CodiceAmministrazione").InnerText.Trim();
                //Andrea De Marco - Il campo di DB ha una lunghezza massima di 16 caratteri - CodiceAmministrazione non può essere più lungo di questo valore
                if (codiceAmministrazione.Length > 16)
                {
                    codiceAmministrazione = codiceAmministrazione.Substring(0, 16);
                }
                //End Andrea De Marco
                string codiceAOO = elIdentificatore.SelectSingleNode("CodiceAOO").InnerText.Trim();
                //Andrea De Marco - Il campo di DB ha una lunghezza massima di 16 caratteri - CodiceAOO non può essere più lungo di questo valore
                if (codiceAOO.Length > 16)
                {
                    codiceAOO = codiceAOO.Substring(0, 16);
                }
                //End Andrea De Marco
                string numeroRegistrazione = elIdentificatore.SelectSingleNode("NumeroRegistrazione").InnerText.Trim();
                string dataRegistrazione = DocsPaUtils.Functions.Functions.CheckData(elIdentificatore.SelectSingleNode("DataRegistrazione").InnerText.Trim());
                bool confermaRic = false;

                if (dataRegistrazione == null)
                {
                    //					logger.addMessage("La mail viene sospesa. La data registrazione ha un formato non corretto"); 
                    logger.Debug("La mail viene sospesa. La data registrazione ha un formato non corretto");
                    err = "La mail viene sospesa. La data registrazione ha un formato non corretto";

                    if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
                    {
                        //logger.addMessage("Sospensione eseguita");
                        logger.Debug("Sospensione eseguita");
                    }
                    else
                    {
                        //logger.addMessage("Sospensione non eseguita");
                        logger.Debug("Sospensione non eseguita");
                    };
                    return false;
                }
                //				logger.addMessage("Data registrazione: "+dataRegistrazione);
                logger.Debug("Data registrazione: " + dataRegistrazione);

                string oggetto = doc.DocumentElement.SelectSingleNode("Intestazione/Oggetto").InnerText.Trim();

                //MITTENTE
                string rows = "";
                XmlElement elOrigine = (XmlElement)doc.DocumentElement.SelectSingleNode("Intestazione/Origine");
                XmlElement elMittente = (XmlElement)elOrigine.SelectSingleNode("Mittente");
                DocsPaVO.addressbook.TipoUtente tipoMittente;
                if (codiceAmministrazione.Equals(reg.codAmministrazione))
                {
                    tipoMittente = DocsPaVO.addressbook.TipoUtente.INTERNO;
                }
                else
                {
                    tipoMittente = DocsPaVO.addressbook.TipoUtente.ESTERNO;
                }
                DocsPaVO.utente.Corrispondente mittente = null;

                if (System.Configuration.ConfigurationManager.AppSettings["RICERCA_COD"] != null
                    && System.Configuration.ConfigurationManager.AppSettings["RICERCA_COD"].Trim().Equals("1"))
                    mittente = getMittente(elMittente, mailOrigine, codiceAmministrazione, tipoMittente, reg);
                else
                    mittente = getMittente(elMittente, mailOrigine, codiceAmministrazione, codiceAOO, tipoMittente, reg, infoUtente, out rows);


                if (mittente == null)
                {
                    //se il mittente è interno, allora ci si blocca!!!!!
                    if (tipoMittente == DocsPaVO.addressbook.TipoUtente.INTERNO)
                    {
                        //						logger.addMessage("La mail viene sospesa: mittente non trovato");
                        logger.Debug("La mail viene sospesa: mittente non trovato, perchè il tipo mittente è interno");
                        err = "La mail viene sospesa: il Mittente non risulta di tipo esterno.";
                        if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
                        {
                            //							logger.addMessage("Sospensione eseguita");
                            logger.Debug("Sospensione eseguita");
                        }
                        else
                        {
                            //							logger.addMessage("Sospensione non eseguita");
                            logger.Debug("Sospensione non eseguita");
                        };
                        return false;
                    }
                    else
                    {
                        mittente = addNewCorrispondente(elOrigine, elIdentificatore, reg);
                        //AGGIORNAMENTO CANALE PREFERENZIALE
                        updateCanalePref(mittente);
                        //inserisco la mail associata al corrispondente esterno in DPA_MAIL_CORR_ESTERNI
                        List<DocsPaVO.utente.MailCorrispondente> casella = new List<DocsPaVO.utente.MailCorrispondente>();
                        casella.Add(new DocsPaVO.utente.MailCorrispondente()
                        {
                            Email = mittente.email,
                            Note = "",
                            Principale = "1"
                        });
                        BusinessLogic.Utenti.addressBookManager.InsertMailCorrispondente(casella, mittente.systemId);
                    }
                }
                else
                {
                    //					logger.addMessage("Mittente trovato"+mittente.GetType());
                    logger.Debug("Mittente trovato" + mittente.GetType());

                    //AGGIORNAMENTO DELLA MAIL E DELL'INTEROPERABILITA' DELLA UO DEL MITTENTE
                    string mailMittente = elOrigine.SelectSingleNode("IndirizzoTelematico").InnerText.Trim();
                    updateUOMittente(mittente, mailMittente);
                    //AGGIORNAMENTO CANALE PREFERENZIALE
                    updateCanalePref(mittente);
                }

                //DESTINATARI
                string infoDestinatari = getInfoDestinatari((XmlElement)doc.DocumentElement.SelectSingleNode("Intestazione"), ref confermaRic, mailAddress);
                //				logger.addMessage("Conferma ricezione: "+confermaRic);
                logger.Debug("Conferma ricezione: " + confermaRic);

                //DESCRIZIONE E DOCUMENTI
                //SA E SF se tipoRiferimento manca il default è MIME, in questo caso deve esserci il nome del file 
                //bisogna gestire il caso di tipoRiferimento='CARTACEO'
                bool tipoRiferimentoMIME = false;
                bool tipoRiferimentoCARTACEO = false;
                //DOCUMENTO PRINCIPALE
                string docPrincipaleName = "";
                XmlElement descrizione = (XmlElement)doc.DocumentElement.SelectSingleNode("Descrizione");
                if (descrizione.SelectSingleNode("TestoDelMessaggio") != null)
                {
                    docPrincipaleName = "body.rtf";
                }
                else
                {
                    //Verifichiamo il tipo di riferimento SAB
                    if ((descrizione.SelectSingleNode("Documento").Attributes["tipoRiferimento"] != null
                         && descrizione.SelectSingleNode("Documento").Attributes["tipoRiferimento"].Value.Equals("MIME"))
                        || descrizione.SelectSingleNode("Documento").Attributes["tipoRiferimento"] == null)
                        tipoRiferimentoMIME = true;
                    else if (descrizione.SelectSingleNode("Documento").Attributes["tipoRiferimento"] != null
                         && descrizione.SelectSingleNode("Documento").Attributes["tipoRiferimento"].Value.ToUpper().Equals("CARTACEO"))
                        tipoRiferimentoCARTACEO = true;

                    if (!tipoRiferimentoMIME && !tipoRiferimentoCARTACEO)
                    //commento SAB if (!descrizione.SelectSingleNode("Documento").Attributes["tipoRiferimento"].Value.Equals("MIME"))
                    {
                        //						logger.addMessage("La mail viene sospesa. Il documento principale non ha MIME come tipo riferimento"); 
                        logger.Debug("La mail viene sospesa. Il documento principale non ha un tipo riferimento valido");
                        err = "La mail viene sospesa. Il documento principale non ha un tipo riferimento valido";
                        if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
                        {
                            //							logger.addMessage("Sospensione eseguita");
                            logger.Debug("Sospensione eseguita");
                        }
                        else
                        {
                            //							logger.addMessage("Sospensione non eseguita");
                            logger.Debug("Sospensione non eseguita");
                        };
                        return false;
                    }
                    if (tipoRiferimentoMIME && (descrizione.SelectSingleNode("Documento").Attributes["nome"] == null))
                    {
                        //						logger.addMessage("La mail viene sospesa. Non e' presente il nome del documento principale"); 
                        logger.Debug("La mail viene sospesa. Non e' presente il nome del documento principale");
                        err = "La mail viene sospesa. Non e' presente il nome del documento principale";
                        if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
                        {
                            //							logger.addMessage("Sospensione eseguita");
                            logger.Debug("Sospensione eseguita");
                        }
                        else
                        {
                            //							logger.addMessage("Sospensione non eseguita");
                            logger.Debug("Sospensione non eseguita");
                        };
                        return false;
                    }
                    if (tipoRiferimentoMIME)
                        docPrincipaleName = descrizione.SelectSingleNode("Documento").Attributes["nome"].Value;
                }

                //CONTROLLO DI CONSISTENZA DEI NOMI DEI DOCUMENTI
                //Documento principale
                if (!System.IO.File.Exists(filepath + "\\" + docPrincipaleName))
                {
                    //					logger.addMessage("La mail viene sospesa. Il documento principale indicato non e' presente"); 
                    logger.Debug("La mail viene sospesa. Il documento principale indicato non e' presente");
                    err = "La mail viene sospesa. Il documento principale indicato non e' presente";
                    if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
                    {
                        //						logger.addMessage("Sospensione eseguita");
                        logger.Debug("Sospensione eseguita");
                    }
                    else
                    {
                        //						logger.addMessage("Sospensione non eseguita");
                        logger.Debug("Sospensione non eseguita");
                    };
                    return false;
                }
                if (getApp(docPrincipaleName) == null)
                {
                    //					logger.addMessage("La mail viene sospesa. Il formato file del documento principale non e' gestito"); 
                    logger.Debug("La mail viene sospesa. Il formato file del documento principale non e' gestito");
                    err = "La mail viene sospesa. Il formato file del documento principale non e' gestito";
                    if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
                    {
                        //						logger.addMessage("Sospensione eseguita");
                        logger.Debug("Sospensione eseguita");
                    }
                    else
                    {
                        //						logger.addMessage("Sospensione non eseguita");
                        logger.Debug("Sospensione non eseguita");
                    };
                    return false;
                }
                //Allegati
                //XmlElement allegati=(XmlElement)  doc.DocumentElement.SelectSingleNode("Descrizione/Allegati");
                XmlNodeList documenti = doc.DocumentElement.SelectNodes("Descrizione/Allegati/Documento[@tipoRiferimento='MIME']");
                for (int ind = 0; ind < documenti.Count; ind++)
                {
                    // SA E SF Se tipoRiferimento è null o è valorizzato con "MIME", si deve procedere con l'analisi 
                    if (((XmlElement)documenti[ind]).Attributes["tipoRiferimento"] == null ||
                        ((XmlElement)documenti[ind]).Attributes["tipoRiferimento"].Value.ToUpper() == "MIME")
                    {
                        //si verifica se e' specificato il nome dell'allegato
                        if (((XmlElement)documenti[ind]).Attributes["nome"] == null)
                        {
                            //						logger.addMessage("La mail viene sospesa. Non e' presente il nome dell'allegato "+ind); 
                            logger.Debug("La mail viene sospesa. Non e' presente il nome dell'allegato " + ind);
                            err = "La mail viene sospesa. Non e' presente il nome dell'allegato " + ind;
                            if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
                            {
                                //							logger.addMessage("Sospensione eseguita");
                                logger.Debug("Sospensione eseguita");
                            }
                            else
                            {
                                //							logger.addMessage("Sospensione non eseguita");
                                logger.Debug("Sospensione non eseguita");
                            };
                            return false;
                        }
                        string nome = ((XmlElement)documenti[ind]).Attributes["nome"].Value;
                        if (!System.IO.File.Exists(filepath + "\\" + nome))
                        {
                            //						logger.addMessage("La mail viene sospesa. Il documento "+nome+" non e' presente"); 
                            logger.Debug("La mail viene sospesa. Il documento " + nome + " non e' presente");
                            err = "La mail viene sospesa. Il documento " + nome + " non e' presente";

                            if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
                            {
                                //							logger.addMessage("Sospensione eseguita");
                                logger.Debug("Sospensione eseguita");
                            }
                            else
                            {
                                //							logger.addMessage("Sospensione non eseguita");
                                logger.Debug("Sospensione non eseguita");
                            };
                            return false;
                        };
                        if (getApp(nome) == null)
                        {
                            //						logger.addMessage("La mail viene sospesa. Il documento "+nome+" ha un formato non gestito"); 
                            logger.Debug("La mail viene sospesa. Il documento " + nome + " ha un formato non gestito");
                            err = "La mail viene sospesa. Il documento " + nome + " ha un formato non gestito";
                            if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
                            {
                                //							logger.addMessage("Sospensione eseguita");
                                logger.Debug("Sospensione eseguita");
                            }
                            else
                            {
                                //							logger.addMessage("Sospensione non eseguita");
                                logger.Debug("Sospensione non eseguita");
                            };
                            return false;
                        }
                    }
                }

                //fatti i controlli, si procede con la protocollazione
                //				logger.addMessage("Inserimento documento principale "+docPrincipaleName);
                logger.Debug("Inserimento documento principale " + docPrincipaleName);

                sd = new DocsPaVO.documento.SchedaDocumento();
                sd.appId = getApp(docPrincipaleName).application;
                sd.idPeople = infoUtente.idPeople;
                sd.userId = infoUtente.userId;

                //sd.note=infoDestinatari; 
                DocsPaVO.documento.Oggetto ogg = new DocsPaVO.documento.Oggetto();
                ogg.descrizione = mail.subject;
                sd.oggetto = ogg;
                sd.predisponiProtocollazione = false;

                //old 28/04/08    sd.registro = reg;
                sd.registro = CaricaRegistroInScheda(reg);

                sd.tipoProto = "A";
                // sd.typeId = "MAIL";
                sd.typeId = "INTEROPERABILITA";
                sd.mezzoSpedizione = BusinessLogic.Documenti.InfoDocManager.getIdMezzoSpedizioneByDesc("INTEROPERABILITA");
                sd.descMezzoSpedizione = "INTEROPERABILITA";
                sd.interop = "S";
                //aggiunta protocollo entrata
                DocsPaVO.documento.ProtocolloEntrata protEntr = new DocsPaVO.documento.ProtocolloEntrata();
                protEntr.mittente = mittente;
                protEntr.dataProtocolloMittente = dataRegistrazione;
                protEntr.descrizioneProtocolloMittente = codiceAOO + DocsPaDB.Utils.Personalization.getInstance(reg.idAmministrazione).getSepSegnatura() + numeroRegistrazione;  //METTERE CARATTERE DELL'AMMINISTRAZIONE
                if (confermaRic)
                {
                    protEntr.invioConferma = "1";
                }
                sd.protocollo = protEntr;

                //dati utente/ruolo/Uo del creatore.
                sd.protocollatore = new DocsPaVO.documento.Protocollatore(infoUtente, ruolo);

                //Riferimenti Mittente
                sd.riferimentoMittente = getRiferimentiMittente(doc);
                //customize delegate
                if (handler != null)
                {
                    logger.Debug("Richiamo handler custom");
                    handler.CustomizeSchedaDocSegnatura(sd, mail, filepath);
                }
                sd = BusinessLogic.Documenti.DocSave.addDocGrigia(sd, infoUtente, ruolo);
                //				logger.addMessage("Salvataggio doc...");
                logger.Debug("Salvataggio doc...");
                //modifica
                sd.documento_da_pec = isPec;
                //fine modifica
                ((DocsPaVO.documento.Documento)sd.documenti[0]).dataArrivo = dataRicezione;
                sd = BusinessLogic.Documenti.DocSave.save(infoUtente, sd, false, out daAggiornareUffRef, ruolo);
                //				logger.addMessage("Salvataggio eseguito");
                logger.Debug("Salvataggio eseguito");
                if (BusinessLogic.interoperabilita.InteroperabilitaManager.InsertAssDocAddress(sd.systemId, reg.systemId, mailAddress))
                    logger.Debug("associazione documento mail address correttamente eseguita in DPA_ASS_DOC_MAIL_INTEROP");
                else
                    logger.Debug("errore nell'associazione documento mail address in DPA_ASS_DOC_MAIL_INTEROP");
                try
                {
                    DocsPaVO.documento.FileDocumento fd = new DocsPaVO.documento.FileDocumento();
                    fs = new System.IO.FileStream(filepath + "\\" + docPrincipaleName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, (int)fs.Length);
                    fs.Close();
                    fd.content = buffer;
                    fd.length = buffer.Length;
                    fd.name = docPrincipaleName;
                    DocsPaVO.documento.FileRequest fRSch = (DocsPaVO.documento.FileRequest)sd.documenti[0];

                    // inserito per permettere la memorizzazione corretta del path nella components 
                    // in caso di ricezione per interoperabilità e PitreDualFileWritingMode=false
                    fRSch.fileName = docPrincipaleName;

                    if (fd.content.Length > 0)
                    {
                        //OLD :  BusinessLogic.Documenti.FileManager.putFile((DocsPaVO.documento.FileRequest)sd.documenti[0], fd, infoUtente);
                        if (!BusinessLogic.Documenti.FileManager.putFile(ref fRSch, fd, infoUtente, out err))
                            throw new Exception(err);
                        else
                        {
                            InteroperabilitaUtils.MatchTSR(filepath, fRSch, fd, infoUtente);
                            XmlParsing.XmlParserManager.parseExtraXmlfiles(sd, docPrincipaleName, fd.content, infoUtente, ruolo);
                            fRSch.fileName = docPrincipaleName;
                            logger.Debug("Documento principale inserito");
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (err == "")
                        err = "Errore nel reperimento del file principale";
                    //se il putFile va in errore devo rimuovere il profile (predisposto appena inserito)
                    BusinessLogic.Documenti.DocManager.ExecRimuoviSchedaMethod(infoUtente, sd);
                    logger.Debug("Eseguita rimozione profilo");
                    logger.Error(err);
                    throw ex;
                }
                //ricerca degli allegati
                logger.Debug("Inserimento degli allegati");

                for (int i = 0; i < documenti.Count; i++)
                {
                    //estrazione dati dell'allegato
                    XmlElement documentoAllegato = (XmlElement)documenti[i];
                    //SA e SF controllo inserito per gestire solo i file di tipo MIME
                    if (((XmlElement)documentoAllegato).Attributes["tipoRiferimento"] == null ||
                        ((XmlElement)documentoAllegato).Attributes["tipoRiferimento"].Value.ToUpper() == "MIME")
                    {
                        string nomeAllegato = documentoAllegato.Attributes["nome"].Value;

                        if (InteroperabilitaUtils.FindTSRMatch(filepath, nomeAllegato))
                            continue; //il TSR fa parte di un o dei doc, salto l'acquisizione e l'aggiunta dell'allegato

                        XmlElement numPagine = (XmlElement)documentoAllegato.SelectSingleNode("NumeroPagine");
                        XmlElement titoloDoc = (XmlElement)documentoAllegato.SelectSingleNode("TitoloDocumento");
                        //					logger.addMessage("Inserimento allegato "+nomeAllegato);
                        logger.Debug("Inserimento allegato " + nomeAllegato);

                        DocsPaVO.documento.Allegato all = new DocsPaVO.documento.Allegato();
                        //					logger.addMessage("docnumber="+sd.docNumber);
                        logger.Debug("docnumber=" + sd.docNumber);

                        all.docNumber = sd.docNumber;
                        //all.applicazione=getApp(nomeAllegato,logger);
                        all.fileName = getFileName(nomeAllegato);
                        all.version = "0";
                        //numero pagine
                        if (numPagine != null && !numPagine.InnerText.Trim().Equals(""))
                        {
                            all.numeroPagine = Int32.Parse(numPagine.InnerText);
                        }
                        //descrizione allegato
                        all.descrizione = "allegato " + i;
                        if (!String.IsNullOrEmpty(all.fileName))
                            all.descrizione = all.fileName;
                        
                        if (titoloDoc != null && !titoloDoc.InnerText.Trim().Equals(""))
                        {
                            all.descrizione = titoloDoc.InnerText;
                        }

                        try
                        {
                            BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, all);
                        }
                        catch (Exception e)
                        {
                            err = "errore nel metodo aggiungiAllegato per l'allegato n. " + Convert.ToString(i + 1);
                            logger.Error(err);
                            throw e;
                        }


                        logger.Debug("Allegato id=" + all.versionId);
                        logger.Debug("Allegato version label=" + all.versionLabel);
                        logger.Debug("Inserimento nel filesystem");

                        try
                        {
                            DocsPaVO.documento.FileDocumento fdAll = new DocsPaVO.documento.FileDocumento();
                            fsAll = new System.IO.FileStream(filepath + "\\" + nomeAllegato, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                            byte[] bufferAll = new byte[fsAll.Length];
                            fsAll.Read(bufferAll, 0, (int)fsAll.Length);
                            fdAll.content = bufferAll;
                            fdAll.length = bufferAll.Length;
                            fdAll.name = nomeAllegato;
                            DocsPaVO.documento.FileRequest fRAll = (DocsPaVO.documento.FileRequest)sd.documenti[0];
                            fRAll = (DocsPaVO.documento.FileRequest)all;

                            // inserito per permettere la memorizzazione corretta del path nella components 
                            // in caso di ricezione per interoperabilità e PitreDualFileWritingMode=false
                            fRAll.fileName = nomeAllegato;

                            if (fdAll.content.Length > 0)
                            {
                                if (!BusinessLogic.Documenti.FileManager.putFile(ref fRAll, fdAll, infoUtente, out err))
                                    throw new Exception(err);
                                else
                                {
                                    logger.Debug("Allegato " + i + " inserito");
                                    fRAll.fileName = nomeAllegato;
                                    InteroperabilitaUtils.MatchTSR(filepath, fRAll, fdAll, infoUtente);
                                    XmlParsing.XmlParserManager.parseExtraXmlfiles(sd, nomeAllegato, fdAll.content, infoUtente, ruolo);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (err == "")
                                err = "Errore nel reperimento del file allegato";
                            //se il putFile va in errore devo rimuovere il profile (predisposto appena inserito)
                            BusinessLogic.Documenti.DocManager.ExecRimuoviSchedaMethod(infoUtente, sd);
                            logger.Debug("Eseguita rimozione profilo");
                            logger.Error(err);
                            throw ex;
                        }
                        finally
                        {

                            fsAll.Close();
                        }
                    }
                }

                ///modifica per il salvataggio della mail

                //string valore = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "");

                if (!string.IsNullOrEmpty(nomeMail))
                {
                    logger.Debug("Inserimento allegato " + nomeMail);
                    DocsPaVO.documento.Allegato all = new DocsPaVO.documento.Allegato();
                    all.descrizione = "E-Mail Ricevuta";
                    logger.Debug("docnumber=" + sd.docNumber);
                    all.docNumber = sd.docNumber;
                    all.fileName = getFileName(nomeMail);
                    all.version = "0";
                    all.numeroPagine = 0;
                    try
                    {
                        BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, all);
                    }
                    catch (Exception e)
                    {
                        err = "errore nel metodo aggiungiAllegato per il salvataggio della mail";
                        logger.Error(err);
                        throw e;
                    }


                    logger.Debug("Allegato id=" + all.versionId);
                    logger.Debug("Allegato version label=" + all.versionLabel);
                    logger.Debug("Inserimento nel filesystem");

                    try
                    {
                        DocsPaVO.documento.FileDocumento fdAll = new DocsPaVO.documento.FileDocumento();
                        fsAll = new System.IO.FileStream(filepath + "\\" + nomeMail, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                        byte[] bufferAll = new byte[fsAll.Length];
                        fsAll.Read(bufferAll, 0, (int)fsAll.Length);
                        fdAll.content = bufferAll;
                        fdAll.length = bufferAll.Length;
                        fdAll.name = nomeMail;
                        DocsPaVO.documento.FileRequest fRAll = (DocsPaVO.documento.FileRequest)sd.documenti[0];
                        fRAll = (DocsPaVO.documento.FileRequest)all;
                        if (fdAll.content.Length > 0)
                        {
                            if (!BusinessLogic.Documenti.FileManager.putFile(ref fRAll, fdAll, infoUtente, out err))
                                throw new Exception(err);
                            else
                                logger.Debug("Salvataggio della mail inserito");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (err == "")
                            err = "Errore nel reperimento del file allegato";
                        BusinessLogic.Documenti.DocManager.ExecRimuoviSchedaMethod(infoUtente, sd);
                        logger.Debug("Eseguita rimozione profilo");
                        logger.Error(err);
                        throw ex;
                    }
                    finally
                    {

                        fsAll.Close();
                    }

                }
                //// fine modifica per il salvataggio

                //TRASMISSIONE   
                logger.Debug("Esegui trasmissione...");

                try
                {
                    DocsPaVO.documento.ResultProtocollazione resultProtocollazione = DocsPaVO.documento.ResultProtocollazione.OK;
                    sd = BusinessLogic.Documenti.ProtoManager.protocolla(sd, ruolo, infoUtente, out resultProtocollazione);
                    if (resultProtocollazione != DocsPaVO.documento.ResultProtocollazione.OK)
                    {
                        err = "Errore durante la protocollazione automatica batch da interoperabilità del doc id: " + sd.systemId + "Errore " + resultProtocollazione.ToString();
                        logger.Debug(err);
                        throw new Exception("Errore durante la protocollazione automatica batch da interoperabilità del doc id: " + "Errore " + resultProtocollazione.ToString());
                    }

                    if (!string.IsNullOrEmpty(infoDestinatari) && infoDestinatari.Length > 248)
                        infoDestinatari = infoDestinatari.Substring(0, 248);
                    // S. Furnari - 16/01/2013 - Sviluppo trasmissione documento ricevuto per interoperabilità interna
                    // solo a ruoli nella UO destinataria e non a tutta la AOO
                    //eseguiTrasmissione(infoUtente.idPeople, serverName, sd, infoDestinatari, reg, ruolo, infoUtente.dst, mailAddress);
                    eseguiTrasmissione(infoUtente.idPeople, serverName, sd, infoDestinatari, reg, ruolo, infoUtente.dst, mailAddress, infoUtente, null);

                    //commento furnari eseguiTrasmissione(infoUtente.idPeople, serverName, sd, infoDestinatari, reg, ruolo, infoUtente.dst, mailAddress, infoUtente);

                    if (!checkExecTrasm(sd.systemId, "INTEROPERABILITA"))
                    {
                        codint1 = true;
                        err = "CODINTEROP1 Trasmissione per INTEROPERABILITA del documento con id:" + sd.docNumber + " non effettuata. Il documento può essere trovato tra i predisposti.";
                        throw new Exception("(CODINTEROP1) Trasmissione per INTEROPERABILITA del documento con id:" + sd.docNumber + "non effettuata!");
                    }
                }
                catch (Exception ex)
                {
                    if (sd != null)
                        docnumber = sd.docNumber;
                    if (InteroperabilitaUtils.MailElaborata(mailId, "E", reg.systemId, docnumber, mail.from))
                        logger.Debug("La mail è stata elaborata");
                    else
                        logger.Debug("La mail non è stata elaborata");

                    if (sd != null && sd.systemId != null && sd.systemId != "" && !codint1)
                    {
                        BusinessLogic.Documenti.DocManager.ExecRimuoviSchedaMethod(infoUtente, sd);
                        logger.Debug("Eseguita rimozione profilo");
                    }
                    logger.Error(err);
                    throw ex;
                }
                //modifica di luca
                logger.Debug("Protocollazione documento in caso di registro AUTOMATICO");

                //dafault OK


                //chiusura canali
                xvr.Close();
                xtr.Close();
                if (sd != null)
                    docnumber = sd.docNumber;
                return true;
            }
            catch (Exception e)
            {
                //				logger.addMessage("La mail viene sospesa. Eccezione: "+e.ToString()); 
                logger.Error("La mail viene sospesa. Eccezione: " + e.Message.ToString());
                logger.Debug("Errone nell'interoperabilità. La mail viene sospesa.", e);
                if (err == "")
                    err = "Errone nell'interoperabilità. La mail viene sospesa." + e.Message.ToString();
                if (err.Contains("CODINTEROP1"))
                {
                    if (sd != null)
                        docnumber = sd.docNumber;
                    if (InteroperabilitaUtils.MailElaborata(mailId, "E", reg.systemId, docnumber, mail.from))
                        logger.Debug("La mail è stata elaborata");
                    else
                        logger.Debug("La mail non è stata elaborata");
                }
                else
                {
                    if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
                        logger.Debug("Sospensione eseguita, errore: " + err);
                    else
                        logger.Debug("Sospensione non eseguita, errore " + err);

                }
                xvr.Close();
                xtr.Close();
                if (fsAll != null) fsAll.Close();
                if (fs != null) fs.Close();
                return false;
            }
        }






        //public static bool eseguiSegnaturaProtocollazione(string serverName, string mailAddress, string filepath, string filename, DocsPaVO.utente.Registro reg, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, string mailId, string mailSubject, string isPec, out string err)
        //{
        //    System.IO.FileStream fs = null;
        //    System.IO.FileStream fsAll = null;
        //    err = "";
        //    bool daAggiornareUffRef = false;
        //    //creazione del doc con trattamento spazi bianchi
        //    XmlDocument doc = new XmlDocument();
        //    InteropResolver my = new InteropResolver();
        //    XmlTextReader xtr = new XmlTextReader(filepath + "\\" + filename);
        //    xtr.WhitespaceHandling = WhitespaceHandling.None;
        //    XmlValidatingReader xvr = new XmlValidatingReader(xtr);
        //    xvr.ValidationType = System.Xml.ValidationType.DTD;
        //    xvr.EntityHandling = System.Xml.EntityHandling.ExpandCharEntities;
        //    xvr.XmlResolver = my;
        //    bool codint1 = false;
        //    try
        //    {
        //        doc.Load(xvr);
        //    }
        //    catch (System.Xml.Schema.XmlSchemaException e)
        //    {
        //        //logger.addMessage("La mail viene sospesa perche' il  file segnatura.xml non e' valido. Eccezione:"+e.Message); 
        //        logger.Debug("La mail viene sospesa perche' il  file segnatura.xml non e' valido. Eccezione:" + e.Message);

        //        //********************************
        //        //** ATTENZIONE !!!!  ************
        //        //** NON RIMUOVERE (CODINTEROP2)**
        //        //** dal msg d'errore.          **
        //        //********************************
        //        err = "( CODINTEROP2 ) La mail viene sospesa perche' il  file segnatura.xml non e' valido. Eccezione:" + e.Message;

        //        if (InteroperabilitaUtils.MailElaborata(mailId, "D"))
        //        {
        //            //logger.addMessage("Sospensione eseguita");
        //            logger.Debug("Sospensione eseguita");
        //        }
        //        else
        //        {
        //            //logger.addMessage("Sospensione non eseguita");
        //            logger.Debug("Sospensione non eseguita");
        //        };
        //        return false;
        //    }
        //    catch (Exception e)
        //    {
        //        //				logger.addMessage("La mail viene sospesa. Eccezione:"+e.Message); 
        //        logger.Debug("La mail viene sospesa. Eccezione:" + e.Message);
        //        err = "La mail viene sospesa. Eccezione:" + e.Message;
        //        if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
        //        {
        //            //					logger.addMessage("Sospensione eseguita");
        //            logger.Debug("Sospensione eseguita");
        //        }
        //        else
        //        {
        //            //					logger.addMessage("Sospensione non eseguita");
        //            logger.Debug("Sospensione non eseguita");
        //        };

        //        return false;
        //    }
        //    finally
        //    {
        //        xvr.Close();
        //        xtr.Close();
        //    }
        //    try
        //    {
        //        XmlElement elIdentificatore = (XmlElement)doc.DocumentElement.SelectSingleNode("Intestazione/Identificatore");
        //        XmlElement elmailOrigine = ((XmlElement)doc.DocumentElement.SelectSingleNode("Intestazione/Origine"));
        //        string mailOrigine = elmailOrigine.SelectSingleNode("IndirizzoTelematico").InnerText.Trim();
        //        string codiceAmministrazione = elIdentificatore.SelectSingleNode("CodiceAmministrazione").InnerText.Trim();
        //        string codiceAOO = elIdentificatore.SelectSingleNode("CodiceAOO").InnerText.Trim();
        //        string numeroRegistrazione = elIdentificatore.SelectSingleNode("NumeroRegistrazione").InnerText.Trim();
        //        string dataRegistrazione = DocsPaUtils.Functions.Functions.CheckData(elIdentificatore.SelectSingleNode("DataRegistrazione").InnerText.Trim());
        //        bool confermaRic = false;

        //        if (dataRegistrazione == null)
        //        {
        //            //					logger.addMessage("La mail viene sospesa. La data registrazione ha un formato non corretto"); 
        //            logger.Debug("La mail viene sospesa. La data registrazione ha un formato non corretto");
        //            err = "La mail viene sospesa. La data registrazione ha un formato non corretto";

        //            if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
        //            {
        //                //						logger.addMessage("Sospensione eseguita");
        //                logger.Debug("Sospensione eseguita");
        //            }
        //            else
        //            {
        //                //						logger.addMessage("Sospensione non eseguita");
        //                logger.Debug("Sospensione non eseguita");
        //            };
        //            return false;
        //        }
        //        //				logger.addMessage("Data registrazione: "+dataRegistrazione);
        //        logger.Debug("Data registrazione: " + dataRegistrazione);

        //        string oggetto = doc.DocumentElement.SelectSingleNode("Intestazione/Oggetto").InnerText.Trim();

        //        //MITTENTE
        //        XmlElement elOrigine = (XmlElement)doc.DocumentElement.SelectSingleNode("Intestazione/Origine");
        //        XmlElement elMittente = (XmlElement)elOrigine.SelectSingleNode("Mittente");
        //        DocsPaVO.addressbook.TipoUtente tipoMittente;
        //        if (codiceAmministrazione.Equals(reg.codAmministrazione))
        //        {
        //            tipoMittente = DocsPaVO.addressbook.TipoUtente.INTERNO;
        //        }
        //        else
        //        {
        //            tipoMittente = DocsPaVO.addressbook.TipoUtente.ESTERNO;
        //        }
        //        DocsPaVO.utente.Corrispondente mittente = getMittente(elMittente, mailOrigine, codiceAmministrazione, tipoMittente, reg);
        //        if (mittente == null)
        //        {
        //            //se il mittente è interno, allora ci si blocca!!!!!
        //            if (tipoMittente == DocsPaVO.addressbook.TipoUtente.INTERNO)
        //            {
        //                //						logger.addMessage("La mail viene sospesa: mittente non trovato");
        //                logger.Debug("La mail viene sospesa: mittente non trovato");
        //                err = "La mail viene sospesa: mittente non trovato nella rubrica.";
        //                if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
        //                {
        //                    //							logger.addMessage("Sospensione eseguita");
        //                    logger.Debug("Sospensione eseguita");
        //                }
        //                else
        //                {
        //                    //							logger.addMessage("Sospensione non eseguita");
        //                    logger.Debug("Sospensione non eseguita");
        //                };
        //                return false;
        //            }
        //            else
        //            {
        //                mittente = addNewCorrispondente(elOrigine, elIdentificatore, reg);
        //                //AGGIORNAMENTO CANALE PREFERENZIALE
        //                updateCanalePref(mittente);
        //            }
        //        }
        //        else
        //        {
        //            //					logger.addMessage("Mittente trovato"+mittente.GetType());
        //            logger.Debug("Mittente trovato" + mittente.GetType());

        //            //AGGIORNAMENTO DELLA MAIL E DELL'INTEROPERABILITA' DELLA UO DEL MITTENTE
        //            string mailMittente = elOrigine.SelectSingleNode("IndirizzoTelematico").InnerText.Trim();
        //            updateUOMittente(mittente, mailMittente);
        //            //AGGIORNAMENTO CANALE PREFERENZIALE
        //            updateCanalePref(mittente);
        //        }

        //        //DESTINATARI
        //        string infoDestinatari = getInfoDestinatari((XmlElement)doc.DocumentElement.SelectSingleNode("Intestazione"), ref confermaRic, mailAddress);
        //        //				logger.addMessage("Conferma ricezione: "+confermaRic);
        //        logger.Debug("Conferma ricezione: " + confermaRic);

        //        //DESCRIZIONE E DOCUMENTI
        //        //DOCUMENTO PRINCIPALE
        //        string docPrincipaleName = "";
        //        XmlElement descrizione = (XmlElement)doc.DocumentElement.SelectSingleNode("Descrizione");
        //        if (descrizione.SelectSingleNode("TestoDelMessaggio") != null)
        //        {
        //            docPrincipaleName = "body.rtf";
        //        }
        //        else
        //        {
        //            //si verifica se il tipo riferimento è MIME e se è presente il nome del documento
        //            if (!descrizione.SelectSingleNode("Documento").Attributes["tipoRiferimento"].Value.Equals("MIME"))
        //            {
        //                //						logger.addMessage("La mail viene sospesa. Il documento principale non ha MIME come tipo riferimento"); 
        //                logger.Debug("La mail viene sospesa. Il documento principale non ha MIME come tipo riferimento");
        //                err = "La mail viene sospesa. Il documento principale non ha MIME come tipo riferimento";
        //                if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
        //                {
        //                    //							logger.addMessage("Sospensione eseguita");
        //                    logger.Debug("Sospensione eseguita");
        //                }
        //                else
        //                {
        //                    //							logger.addMessage("Sospensione non eseguita");
        //                    logger.Debug("Sospensione non eseguita");
        //                };
        //                return false;
        //            }
        //            if (descrizione.SelectSingleNode("Documento").Attributes["nome"] == null)
        //            {
        //                //						logger.addMessage("La mail viene sospesa. Non e' presente il nome del documento principale"); 
        //                logger.Debug("La mail viene sospesa. Non e' presente il nome del documento principale");
        //                err = "La mail viene sospesa. Non e' presente il nome del documento principale";
        //                if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
        //                {
        //                    //							logger.addMessage("Sospensione eseguita");
        //                    logger.Debug("Sospensione eseguita");
        //                }
        //                else
        //                {
        //                    //							logger.addMessage("Sospensione non eseguita");
        //                    logger.Debug("Sospensione non eseguita");
        //                };
        //                return false;
        //            }
        //            docPrincipaleName = descrizione.SelectSingleNode("Documento").Attributes["nome"].Value;
        //        }

        //        //CONTROLLO DI CONSISTENZA DEI NOMI DEI DOCUMENTI
        //        //Documento principale
        //        if (!System.IO.File.Exists(filepath + "\\" + docPrincipaleName))
        //        {
        //            //					logger.addMessage("La mail viene sospesa. Il documento principale indicato non e' presente"); 
        //            logger.Debug("La mail viene sospesa. Il documento principale indicato non e' presente");
        //            err = "La mail viene sospesa. Il documento principale indicato non e' presente";
        //            if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
        //            {
        //                //						logger.addMessage("Sospensione eseguita");
        //                logger.Debug("Sospensione eseguita");
        //            }
        //            else
        //            {
        //                //						logger.addMessage("Sospensione non eseguita");
        //                logger.Debug("Sospensione non eseguita");
        //            };
        //            return false;
        //        };
        //        if (getApp(docPrincipaleName) == null)
        //        {
        //            //					logger.addMessage("La mail viene sospesa. Il formato file del documento principale non e' gestito"); 
        //            logger.Debug("La mail viene sospesa. Il formato file del documento principale non e' gestito");
        //            err = "La mail viene sospesa. Il formato file del documento principale non e' gestito";
        //            if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
        //            {
        //                //						logger.addMessage("Sospensione eseguita");
        //                logger.Debug("Sospensione eseguita");
        //            }
        //            else
        //            {
        //                //						logger.addMessage("Sospensione non eseguita");
        //                logger.Debug("Sospensione non eseguita");
        //            };
        //            return false;
        //        }
        //        //Allegati
        //        //XmlElement allegati=(XmlElement)  doc.DocumentElement.SelectSingleNode("Descrizione/Allegati");
        //        XmlNodeList documenti = doc.DocumentElement.SelectNodes("Descrizione/Allegati/Documento[@tipoRiferimento='MIME']");
        //        for (int ind = 0; ind < documenti.Count; ind++)
        //        {
        //            //si verifica se e' specificato il nome dell'allegato
        //            if (((XmlElement)documenti[ind]).Attributes["nome"] == null)
        //            {
        //                //						logger.addMessage("La mail viene sospesa. Non e' presente il nome dell'allegato "+ind); 
        //                logger.Debug("La mail viene sospesa. Non e' presente il nome dell'allegato " + ind);
        //                err = "La mail viene sospesa. Non e' presente il nome dell'allegato " + ind;
        //                if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
        //                {
        //                    //							logger.addMessage("Sospensione eseguita");
        //                    logger.Debug("Sospensione eseguita");
        //                }
        //                else
        //                {
        //                    //							logger.addMessage("Sospensione non eseguita");
        //                    logger.Debug("Sospensione non eseguita");
        //                };
        //                return false;
        //            }
        //            string nome = ((XmlElement)documenti[ind]).Attributes["nome"].Value;
        //            if (!System.IO.File.Exists(filepath + "\\" + nome))
        //            {
        //                //						logger.addMessage("La mail viene sospesa. Il documento "+nome+" non e' presente"); 
        //                logger.Debug("La mail viene sospesa. Il documento " + nome + " non e' presente");
        //                err = "La mail viene sospesa. Il documento " + nome + " non e' presente";

        //                if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
        //                {
        //                    //							logger.addMessage("Sospensione eseguita");
        //                    logger.Debug("Sospensione eseguita");
        //                }
        //                else
        //                {
        //                    //							logger.addMessage("Sospensione non eseguita");
        //                    logger.Debug("Sospensione non eseguita");
        //                };
        //                return false;
        //            };
        //            if (getApp(nome) == null)
        //            {
        //                //						logger.addMessage("La mail viene sospesa. Il documento "+nome+" ha un formato non gestito"); 
        //                logger.Debug("La mail viene sospesa. Il documento " + nome + " ha un formato non gestito");
        //                err = "La mail viene sospesa. Il documento " + nome + " ha un formato non gestito";
        //                if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
        //                {
        //                    //							logger.addMessage("Sospensione eseguita");
        //                    logger.Debug("Sospensione eseguita");
        //                }
        //                else
        //                {
        //                    //							logger.addMessage("Sospensione non eseguita");
        //                    logger.Debug("Sospensione non eseguita");
        //                };
        //                return false;
        //            }
        //        }

        //        //fatti i controlli, si procede con la protocollazione
        //        //				logger.addMessage("Inserimento documento principale "+docPrincipaleName);
        //        logger.Debug("Inserimento documento principale " + docPrincipaleName);

        //        DocsPaVO.documento.SchedaDocumento sd = new DocsPaVO.documento.SchedaDocumento();
        //        sd.appId = getApp(docPrincipaleName).application;
        //        sd.idPeople = infoUtente.idPeople;
        //        sd.userId = infoUtente.userId;
        //        //sd.note=infoDestinatari; 
        //        DocsPaVO.documento.Oggetto ogg = new DocsPaVO.documento.Oggetto();
        //        ogg.descrizione = mailSubject;
        //        sd.oggetto = ogg;
        //        sd.predisponiProtocollazione = false;//true;

        //        //old 28/04/08    sd.registro = reg;
        //        sd.registro = CaricaRegistroInScheda(reg);

        //        sd.tipoProto = "A";
        //        // sd.typeId = "MAIL";
        //        sd.typeId = "INTEROPERABILITA";
        //        sd.mezzoSpedizione = BusinessLogic.Documenti.InfoDocManager.getIdMezzoSpedizioneByDesc("INTEROPERABILITA");
        //        sd.descMezzoSpedizione = "INTEROPERABILITA";
        //        sd.interop = "S";
        //        //aggiunta protocollo entrata
        //        DocsPaVO.documento.ProtocolloEntrata protEntr = new DocsPaVO.documento.ProtocolloEntrata();
        //        protEntr.mittente = mittente;
        //        protEntr.dataProtocolloMittente = dataRegistrazione;
        //        protEntr.descrizioneProtocolloMittente = codiceAOO + DocsPaDB.Utils.Personalization.getInstance(reg.idAmministrazione).getSepSegnatura() + numeroRegistrazione;  //METTERE CARATTERE DELL'AMMINISTRAZIONE
        //        if (confermaRic)
        //        {
        //            protEntr.invioConferma = "1";
        //        }
        //        sd.protocollo = protEntr;

        //        //dati utente/ruolo/Uo del creatore.
        //        sd.protocollatore = new DocsPaVO.documento.Protocollatore(infoUtente, ruolo);

        //        //Riferimenti Mittente
        //        sd.riferimentoMittente = getRiferimentiMittente(doc);

        //        sd = BusinessLogic.Documenti.DocSave.addDocGrigia(sd, infoUtente, ruolo);
        //        //				logger.addMessage("Salvataggio doc...");
        //        logger.Debug("Salvataggio doc...");

        //        sd = BusinessLogic.Documenti.DocSave.save(infoUtente, sd, false, out daAggiornareUffRef, ruolo);
        //        //				logger.addMessage("Salvataggio eseguito");
        //        logger.Debug("Salvataggio eseguito");

        //        try
        //        {
        //            DocsPaVO.documento.FileDocumento fd = new DocsPaVO.documento.FileDocumento();
        //            fs = new System.IO.FileStream(filepath + "\\" + docPrincipaleName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
        //            byte[] buffer = new byte[fs.Length];
        //            fs.Read(buffer, 0, (int)fs.Length);
        //            fd.content = buffer;
        //            fd.length = buffer.Length;
        //            fd.name = docPrincipaleName;
        //            DocsPaVO.documento.FileRequest fRSch = (DocsPaVO.documento.FileRequest)sd.documenti[0];
        //            if (fd.content.Length > 0)
        //            {
        //                //OLD :  BusinessLogic.Documenti.FileManager.putFile((DocsPaVO.documento.FileRequest)sd.documenti[0], fd, infoUtente);
        //                if (!BusinessLogic.Documenti.FileManager.putFile(ref fRSch, fd, infoUtente, out err))
        //                    throw new Exception(err);
        //                else
        //                    logger.Debug("Documento principale inserito");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            if (err == "")
        //                err = "Errore nel reperimento del file principale";
        //            //se il putFile va in errore devo rimuovere il profile (predisposto appena inserito)
        //            BusinessLogic.Documenti.DocManager.ExecRimuoviSchedaMethod(infoUtente, sd);
        //            logger.Debug("Eseguita rimozione profilo");
        //            logger.Debug(err);
        //            throw ex;
        //        }
        //        //ricerca degli allegati
        //        logger.Debug("Inserimento degli allegati");

        //        for (int i = 0; i < documenti.Count; i++)
        //        {
        //            //estrazione dati dell'allegato
        //            XmlElement documentoAllegato = (XmlElement)documenti[i];
        //            string nomeAllegato = documentoAllegato.Attributes["nome"].Value;
        //            XmlElement numPagine = (XmlElement)documentoAllegato.SelectSingleNode("NumeroPagine");
        //            XmlElement titoloDoc = (XmlElement)documentoAllegato.SelectSingleNode("TitoloDocumento");
        //            //					logger.addMessage("Inserimento allegato "+nomeAllegato);
        //            logger.Debug("Inserimento allegato " + nomeAllegato);

        //            DocsPaVO.documento.Allegato all = new DocsPaVO.documento.Allegato();
        //            all.descrizione = "allegato " + i;
        //            //					logger.addMessage("docnumber="+sd.docNumber);
        //            logger.Debug("docnumber=" + sd.docNumber);

        //            all.docNumber = sd.docNumber;
        //            //all.applicazione=getApp(nomeAllegato,logger);
        //            all.fileName = getFileName(nomeAllegato);
        //            all.version = "0";
        //            //numero pagine
        //            if (numPagine != null && !numPagine.InnerText.Trim().Equals(""))
        //            {
        //                all.numeroPagine = Int32.Parse(numPagine.InnerText);
        //            }
        //            //descrizione allegato
        //            if (titoloDoc != null && !titoloDoc.InnerText.Trim().Equals(""))
        //            {
        //                all.descrizione = titoloDoc.InnerText;
        //            }

        //            try
        //            {
        //                BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, all);
        //            }
        //            catch (Exception e)
        //            {
        //                err = "errore nel metodo aggiungiAllegato per l'allegato n. " + Convert.ToString(i + 1);
        //                logger.Debug(err);
        //                throw e;
        //            }


        //            #region Codice Commentato
        //            //					logger.addMessage("Allegato id="+all.versionId);
        //            //					logger.addMessage("Allegato version label="+all.versionLabel);
        //            //					logger.addMessage("Inserimento nel filesystem");
        //            #endregion

        //            logger.Debug("Allegato id=" + all.versionId);
        //            logger.Debug("Allegato version label=" + all.versionLabel);
        //            logger.Debug("Inserimento nel filesystem");

        //            try
        //            {
        //                DocsPaVO.documento.FileDocumento fdAll = new DocsPaVO.documento.FileDocumento();
        //                fsAll = new System.IO.FileStream(filepath + "\\" + nomeAllegato, System.IO.FileMode.Open, System.IO.FileAccess.Read);
        //                byte[] bufferAll = new byte[fsAll.Length];
        //                fsAll.Read(bufferAll, 0, (int)fsAll.Length);
        //                fdAll.content = bufferAll;
        //                fdAll.length = bufferAll.Length;
        //                fdAll.name = nomeAllegato;
        //                DocsPaVO.documento.FileRequest fRAll = (DocsPaVO.documento.FileRequest)sd.documenti[0];
        //                fRAll = (DocsPaVO.documento.FileRequest)all;
        //                if (fdAll.content.Length > 0)
        //                {
        //                    if (!BusinessLogic.Documenti.FileManager.putFile(ref fRAll, fdAll, infoUtente, out err))
        //                        throw new Exception(err);
        //                    else
        //                        logger.Debug("Allegato " + i + " inserito");
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                if (err == "")
        //                    err = "Errore nel reperimento del file allegato";
        //                //se il putFile va in errore devo rimuovere il profile (predisposto appena inserito)
        //                BusinessLogic.Documenti.DocManager.ExecRimuoviSchedaMethod(infoUtente, sd);
        //                logger.Debug("Eseguita rimozione profilo");
        //                logger.Debug(err);
        //                throw ex;
        //            }
        //            finally
        //            {

        //                fsAll.Close();
        //            }
        //        }

        //        //TRASMISSIONE   
        //        //				logger.addMessage("Esegui trasmissione...");
        //        logger.Debug("Esegui trasmissione...");

        //        //eseguiTrasmissione(infoUtente.idPeople,serverName, sd,infoDestinatari,reg,ruolo);


        //        //eseguiTrasmissione(infoUtente.idPeople,serverName,sd,null,reg,ruolo);
        //        try
        //        {
        //            if (!string.IsNullOrEmpty(infoDestinatari) && infoDestinatari.Length > 248)
        //                infoDestinatari = infoDestinatari.Substring(0, 248);
        //            eseguiTrasmissione(infoUtente.idPeople, serverName, sd, infoDestinatari, reg, ruolo, infoUtente.dst);

        //            if (!checkExecTrasm(sd.systemId, "INTEROPERABILITA"))
        //            {
        //                codint1 = true;
        //                err = "CODINTEROP1 Trasmissione per INTEROPERABILITA del documento con id:" + sd.docNumber + " non effettuata. Il documento può essere trovato tra i predisposti.";
        //                throw new Exception("(CODINTEROP1) Trasmissione per INTEROPERABILITA del documento con id:" + sd.docNumber + "non effettuata!");
        //            }
        //        }
        //        catch (Exception ex)
        //        {

        //            if (InteroperabilitaUtils.MailElaborata(mailId, "E", reg.systemId))
        //                logger.Debug("La mail è stata elaborata");
        //            else
        //                logger.Debug("La mail non è stata elaborata");

        //            if (sd != null && sd.systemId != null && sd.systemId != "" && !codint1)
        //            {
        //                BusinessLogic.Documenti.DocManager.ExecRimuoviSchedaMethod(infoUtente, sd);
        //                logger.Debug("Eseguita rimozione profilo");
        //            }
        //            logger.Debug(err);
        //            throw ex;
        //        }
        //        //modifica di luca
        //        logger.Debug("Protocollazione documento in caso di registro AUTOMATICO");

        //        //dafault OK

        //        DocsPaVO.documento.ResultProtocollazione resultProtocollazione = DocsPaVO.documento.ResultProtocollazione.OK;
        //        sd = BusinessLogic.Documenti.ProtoManager.protocolla(sd, ruolo, infoUtente, out resultProtocollazione);
        //        if (resultProtocollazione != DocsPaVO.documento.ResultProtocollazione.OK)
        //        {
        //            err = "Errore durante la protocollazione automatica batch da interoperabilità del doc id: " + sd.systemId + "Errore " + resultProtocollazione.ToString();
        //            logger.Debug(err);
        //            throw new Exception("Errore durante la protocollazione automatica batch da interoperabilità del doc id: " + "Errore " + resultProtocollazione.ToString());
        //        }
        //        //chiusura canali
        //        fs.Close();
        //        xvr.Close();
        //        xtr.Close();
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        //				logger.addMessage("La mail viene sospesa. Eccezione: "+e.ToString()); 
        //        logger.Debug("La mail viene sospesa. Eccezione: " + e.Message.ToString());
        //        logger.Debug("Errone nell'interoperabilità. La mail viene sospesa.", e);
        //        if (err == "")
        //            err = "Errone nell'interoperabilità. La mail viene sospesa." + e.Message.ToString();

        //        if (err.Contains("CODINTEROP1"))
        //        {
        //            if (InteroperabilitaUtils.MailElaborata(mailId, "E", reg.systemId))
        //                logger.Debug("La mail è stata elaborata");
        //            else
        //                logger.Debug("La mail non è stata elaborata");
        //        }
        //        else
        //        {
        //            if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
        //                logger.Debug("Sospensione eseguita, errore: " + err);
        //            else
        //                logger.Debug("Sospensione non eseguita, errore " + err);

        //        }

        //        xvr.Close();
        //        xtr.Close();
        //        if (fsAll != null) fsAll.Close();
        //        if (fs != null) fs.Close();
        //        return false;
        //    }
        //}


        #region old protocollazione

        //       public static bool eseguiSenzaSegnaturaProtocollazione(string serverName, string filepath, DocsPaVO.utente.Registro reg, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, string mailId, Interoperabilità.CMMsg mc, out string err)
        //       {
        //           System.IO.FileStream fs = null;
        //           System.IO.FileStream fsAll = null;
        //           bool daAggiornareUffRef = false;
        //           DocsPaVO.documento.SchedaDocumento sd = null;
        //           err = string.Empty;
        //           bool codint1 = false;
        //           try
        //           {
        //               string docPrincipaleName = "body.html";

        //               //CONTROLLI NEL FORMATO DEI FILES
        //               //Documento principale
        //               if (getApp(docPrincipaleName) == null)
        //               {
        //                   logger.Debug("La mail viene sospesa. Il documento principale ha un formato non gestito");
        //                   err = "La mail viene sospesa. Il documento principale ha un formato non gestito";
        //                   if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
        //                   {
        //                       logger.Debug("Sospensione eseguita");
        //                   }
        //                   else
        //                   {
        //                       logger.Debug("Sospensione non eseguita");
        //                   };
        //                   return false;
        //               }
        //               //Attachment
        //               for (int ind = 0; ind < mc.attachments.Count; ind++)
        //               {

        //                   if (getApp(mc.attachments[ind].name) == null)
        //                   {
        //                       //						logger.addMessage("La mail viene sospesa. Il documento "+mc.Attachments[ind].Name+" ha un formato non gestito"); 
        //                       logger.Debug("La mail viene sospesa. Il documento " + mc.attachments[ind].name + " ha un formato non gestito");
        //                       err = "La mail viene sospesa. Il documento " + mc.attachments[ind].name + " ha un formato non gestito";
        //                       if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
        //                       {
        //                           //							logger.addMessage("Sospensione eseguita");
        //                           logger.Debug("Sospensione eseguita");
        //                       }
        //                       else
        //                       {
        //                           //							logger.addMessage("Sospensione non eseguita");
        //                           logger.Debug("Sospensione non eseguita");
        //                       }

        //                       return false;
        //                   }
        //               }

        //               //mittente 
        //               DocsPaVO.utente.UnitaOrganizzativa mittente = new DocsPaVO.utente.UnitaOrganizzativa();
        //               mittente = (DocsPaVO.utente.UnitaOrganizzativa)BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(mc.from, infoUtente);
        //               if (mittente == null)
        //               {
        //                   mittente = new DocsPaVO.utente.UnitaOrganizzativa();
        //                   mittente.descrizione = mc.from;
        //                   mittente.tipoIE = "E";
        //                   mittente.tipoCorrispondente = "S";
        //                   mittente.codiceRubrica = mc.from;
        //                   mittente.codiceAOO = reg.codice;
        //                   mittente.codiceAmm = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(reg.idAmministrazione).Codice;
        //                   mittente.email = mc.from;
        //                   string idMezzoSpedizione = BusinessLogic.Documenti.InfoDocManager.getIdMezzoSpedizioneByDesc("MAIL");
        //                   mittente.canalePref = BusinessLogic.Documenti.InfoDocManager.getCanaleBySystemId(idMezzoSpedizione);
        //                   mittente.cognome = mc.from;
        //                   mittente.idAmministrazione = reg.idAmministrazione;
        //                   mittente = (DocsPaVO.utente.UnitaOrganizzativa)BusinessLogic.Utenti.addressBookManager.insertCorrispondente(mittente, null);
        //               }

        //               //logger.addMessage("Inserimento documento principale");
        //               logger.Debug("Inserimento documento principale");

        //               sd = new DocsPaVO.documento.SchedaDocumento();
        //               sd.appId = getApp(docPrincipaleName).application;
        //               sd.idPeople = infoUtente.idPeople;
        //               sd.userId = infoUtente.userId;
        //               DocsPaVO.documento.Oggetto ogg = new DocsPaVO.documento.Oggetto();
        //               ogg.descrizione = mc.subject;
        //               sd.oggetto = ogg;
        //               sd.predisponiProtocollazione = false;//true;


        //               //28/04/08
        //               //sd.registro = reg;

        //               sd.registro = CaricaRegistroInScheda(reg);

        //               sd.tipoProto = "A";
        //               sd.typeId = "MAIL";
        //               sd.interop = "P";
        //               DocsPaVO.documento.ProtocolloEntrata protEntr = new DocsPaVO.documento.ProtocolloEntrata();
        //               protEntr.mittente = mittente;


        //               sd.mezzoSpedizione = mittente.canalePref.systemId;
        //               sd.descMezzoSpedizione = mittente.canalePref.descrizione;

        //               //Controllo se nell'headers esiste la chiave "utenteDocspa"
        //               //In questo caso risolvo tramite email il mittete e lo imposto come mittente intermedio
        //               //del protocollo in ingresso
        //               System.Collections.Hashtable headers = mc.headers;
        //               if (headers["utenteDocspa"] != null)
        //               {
        //                   logger.Debug("Risoluzione mittente intermedio ...");
        //                   System.Collections.IDictionaryEnumerator enumerator = headers.GetEnumerator();
        //                   while (enumerator.MoveNext())
        //                   {
        //                       if (enumerator.Key.ToString() == "utenteDocspa")
        //                       {
        //                           DocsPaVO.utente.Corrispondente mittenteIntermedio = BusinessLogic.Utenti.UserManager.getUtenteByEmail(reg.idAmministrazione, enumerator.Value.ToString());
        //                           if (mittenteIntermedio != null && mittenteIntermedio.systemId != null)
        //                               protEntr.mittenteIntermedio = mittenteIntermedio;
        //                       }
        //                   }
        //                   headers.Remove("utenteDocspa");
        //                   logger.Debug("Risoluzione mittente intermedio eseguita ...");
        //               }

        //               sd.protocollo = protEntr;

        //               sd.protocollo.invioConferma = "0";

        //               sd = BusinessLogic.Documenti.DocSave.addDocGrigia(sd, infoUtente, ruolo);
        //               //				logger.addMessage("Salvataggio doc...");
        //               logger.Debug("Salvataggio doc...");

        //               //Controllo che il file allegato sia unico e sia una form pdf da processare
        //               if (mc.attachments.Count == 1 && System.IO.Path.GetExtension(mc.attachments[0].name).ToUpper() == ".PDF")
        //               {
        //                   //Recupero il fileDocumento
        //                   DocsPaVO.documento.FileDocumento fdAll = new DocsPaVO.documento.FileDocumento();
        //                   fsAll = new System.IO.FileStream(filepath + "\\" + mc.attachments[0].name, System.IO.FileMode.Open);
        //                   byte[] bufferAll = new byte[fsAll.Length];
        //                   fsAll.Read(bufferAll, 0, (int)fsAll.Length);
        //                   fdAll.content = bufferAll;
        //                   fdAll.length = bufferAll.Length;
        //                   fdAll.name = mc.attachments[0].name;
        //                   fsAll.Close();

        //                   //Controllo che il file pdf sia una form che sappiamo processare
        //                   DocsPaVO.LiveCycle.ProcessFormInput processFormInput = new DocsPaVO.LiveCycle.ProcessFormInput();
        //                   processFormInput.schedaDocumentoInput = sd;
        //                   processFormInput.fileDocumentoInput = fdAll;
        //                   DocsPaVO.LiveCycle.ProcessFormOutput processFormOutput = BusinessLogic.LiveCycle.LiveCycle.processFormPdf(infoUtente, processFormInput);

        //                   if (processFormOutput != null && processFormOutput.schedaDocumentoOutput != null)
        //                   {
        //                       sd = processFormOutput.schedaDocumentoOutput;
        //                       docPrincipaleName = mc.attachments[0].name;
        //                       mc.attachments.Clear();
        //                   }                    
        //               }

        //               sd = BusinessLogic.Documenti.DocSave.save(infoUtente, sd, false, out daAggiornareUffRef, ruolo);
        //               //				logger.addMessage("Salvataggio eseguito");
        //               logger.Debug("Salvataggio eseguito");

        //               DocsPaVO.documento.FileDocumento fd = new DocsPaVO.documento.FileDocumento();
        //               fs = new System.IO.FileStream(filepath + "\\" + docPrincipaleName, System.IO.FileMode.Open);
        //               byte[] buffer = new byte[fs.Length];
        //               fs.Read(buffer, 0, (int)fs.Length);
        //               fd.content = buffer;
        //               fd.length = buffer.Length;
        //               fd.name = docPrincipaleName;
        //               DocsPaVO.documento.FileRequest frSch = (DocsPaVO.documento.FileRequest)sd.documenti[0];
        //               if (!BusinessLogic.Documenti.FileManager.putFile(ref frSch, fd, infoUtente, out err))
        //                   throw new Exception(err);
        //               //				logger.addMessage("Documento principale inserito");
        //               else
        //                   logger.Debug("Documento principale inserito");

        //               //ricerca degli allegati
        //               //				logger.addMessage("Inserimento degli allegati");
        //               logger.Debug("Inserimento degli allegati");

        //               for (int i = 0; i < mc.attachments.Count; i++)
        //               {
        //                   //estrazione dati dell'allegato
        //                   string nomeAllegato = mc.attachments[i].name;
        //                   //					logger.addMessage("Inserimento allegato "+nomeAllegato);
        //                   logger.Debug("Inserimento allegato " + nomeAllegato);

        //                   DocsPaVO.documento.Allegato all = new DocsPaVO.documento.Allegato();
        //                   all.descrizione = "allegato " + i;
        //                   //all.applicazione=getApp(nomeAllegato,logger);
        //                   all.docNumber = sd.docNumber;
        //                   all.fileName = getFileName(nomeAllegato);
        //                   all.version = "0";
        //                   try
        //                   {
        //                       BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, all);
        //                   }
        //                   catch (Exception e)
        //                   {
        //                       err = "errore nel metodo aggiungiAllegato per l'allegato n. " + Convert.ToString(i + 1);
        //                       logger.Debug(err);
        //                       throw e;
        //                   }

        //                   #region Codice Commentato
        //                   //					logger.addMessage("Allegato id="+all.versionId);
        //                   //					logger.addMessage("Allegato version label="+all.versionLabel);
        //                   //					logger.addMessage("Inserimento nel filesystem");
        //                   #endregion

        //                   logger.Debug("Allegato id=" + all.versionId);
        //                   logger.Debug("Allegato version label=" + all.versionLabel);
        //                   logger.Debug("Inserimento nel filesystem");

        //                   DocsPaVO.documento.FileDocumento fdAll = new DocsPaVO.documento.FileDocumento();
        //                   fsAll = new System.IO.FileStream(filepath + "\\" + nomeAllegato, System.IO.FileMode.Open);
        //                   byte[] bufferAll = new byte[fsAll.Length];
        //                   fsAll.Read(bufferAll, 0, (int)fsAll.Length);
        //                   fdAll.content = bufferAll;
        //                   fdAll.length = bufferAll.Length;
        //                   fdAll.name = nomeAllegato;
        //                   DocsPaVO.documento.FileRequest fRAll = (DocsPaVO.documento.FileRequest)all;
        //                   if (!BusinessLogic.Documenti.FileManager.putFile(ref fRAll, fdAll, infoUtente, out err))
        //                       throw new Exception(err);
        //                   else
        //                       logger.Debug("Allegato " + i + " inserito");

        //                   fsAll.Close();
        //               }

        //               //TRASMISSIONE 
        //               //				logger.addMessage("Preparazione trasmissione...");
        //               logger.Debug("Preparazione trasmissione...");

        //               //eseguiTrasmissione(infoUtente.idPeople,serverName,sd,null,reg,ruolo);
        //               try
        //               {
        //                   //eseguiTrasmissione(infoUtente.idPeople,serverName, sd,infoDestinatari,reg,ruolo);
        //                   //eseguiTrasmissione(infoUtente.idPeople, serverName, sd, "INTEROPERABILITA", reg, ruolo, infoUtente.dst);
        //                   eseguiTrasmissione(infoUtente.idPeople, serverName, sd, null, reg, ruolo, infoUtente.dst);

        //                   if (!checkExecTrasm(sd.systemId, "INTEROPERABILITA"))
        //                   {
        //                       codint1 = true;
        //                       err = "CODINTEROP1 Trasmissione per INTEROPERABILITA del documento con id:" + sd.docNumber + " non effettuata. Il documento può essere trovato tra i predisposti.";
        //                       throw new Exception("(CODINTEROP1) Trasmissione per INTEROPERABILITA del documento con id:" + sd.docNumber + "non effettuata!");
        //                   }
        //               }
        //               catch (Exception ex)
        //               {
        //                   logger.Debug(err);
        //                   throw ex;
        //               }

        //               //nuovo
        //               logger.Debug("Protocollazione documento in caso di registro AUTOMATICO");
        //               //dafault OK
        //               DocsPaVO.documento.ResultProtocollazione resultProtocollazione = DocsPaVO.documento.ResultProtocollazione.OK;
        //               //

        //               sd = BusinessLogic.Documenti.ProtoManager.protocolla(sd, ruolo, infoUtente, out resultProtocollazione);
        //               //systemId protocollo in arrivo appena creato su registro automatico
        //               if (resultProtocollazione != DocsPaVO.documento.ResultProtocollazione.OK)
        //               {
        //                   err = "Errore durante la protocollazione automatica batch da interoperabilità del doc id: " + sd.systemId + "Errore " + resultProtocollazione.ToString();
        //                   logger.Debug(err);
        //                   throw new Exception("Errore durante la protocollazione automatica batch da interoperabilità del doc id: " + "Errore " + resultProtocollazione.ToString());
        //               }
        //               //
        //fs.Close();
        //               return true;
        //           }
        //           catch (Exception e)
        //           {
        //               //				logger.addMessage("La mail viene sospesa. Eccezione: "+e.ToString()); 
        //               logger.Debug("La mail viene sospesa. Eccezione: " + e.ToString());

        //               if (err.Contains("CODINTEROP1"))
        //               {
        //                   if (InteroperabilitaUtils.MailElaborata(mailId, "E", reg.systemId))
        //                       logger.Debug("La mail è stata elaborata");
        //                   else
        //                       logger.Debug("La mail non è stata elaborata");
        //               }
        //               else
        //               {
        //                   if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
        //                       logger.Debug("Sospensione eseguita, errore: " + err);
        //                   else
        //                       logger.Debug("Sospensione non eseguita, errore " + err);

        //               }                
        //               if (fs != null) fs.Close();
        //               if (fsAll != null) fsAll.Close();
        //               if (sd != null && !codint1)
        //               {
        //                   BusinessLogic.Documenti.DocManager.ExecRimuoviSchedaMethod(infoUtente, sd);
        //                   logger.Debug("Eseguita rimozione profilo");
        //                   logger.Debug(err);
        //               }


        //               return false;
        //           }
        //       }


        //public static bool eseguiSegnaturaProtocollazione(string serverName, string mailAddress, string filepath, string filename, DocsPaVO.utente.Registro reg, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, string mailId, string mailSubject, out string err)
        //{
        //    System.IO.FileStream fs = null;
        //    System.IO.FileStream fsAll = null;
        //    err = "";
        //    bool daAggiornareUffRef = false;
        //    //creazione del doc con trattamento spazi bianchi
        //    XmlDocument doc = new XmlDocument();
        //    InteropResolver my = new InteropResolver();
        //    XmlTextReader xtr = new XmlTextReader(filepath + "\\" + filename);
        //    xtr.WhitespaceHandling = WhitespaceHandling.None;
        //    XmlValidatingReader xvr = new XmlValidatingReader(xtr);
        //    xvr.ValidationType = System.Xml.ValidationType.DTD;
        //    xvr.EntityHandling = System.Xml.EntityHandling.ExpandCharEntities;
        //    xvr.XmlResolver = my;
        //    bool codint1= false;
        //    try
        //    {
        //        doc.Load(xvr);
        //    }
        //    catch (System.Xml.Schema.XmlSchemaException e)
        //    {
        //        //logger.addMessage("La mail viene sospesa perche' il  file segnatura.xml non e' valido. Eccezione:"+e.Message); 
        //        logger.Debug("La mail viene sospesa perche' il  file segnatura.xml non e' valido. Eccezione:" + e.Message);
        //        //********************************
        //        //** ATTENZIONE !!!!  ************
        //        //** NON RIMUOVERE (CODINTEROP2)**
        //        //** dal msg d'errore.          **
        //        //********************************
        //        err = "( CODINTEROP2 ) La mail viene sospesa perche' il  file segnatura.xml non e' valido. Eccezione:" + e.Message;
        //        if (InteroperabilitaUtils.MailElaborata(mailId, "D"))
        //        {
        //            //logger.addMessage("Sospensione eseguita");
        //            logger.Debug("Sospensione eseguita");
        //        }
        //        else
        //        {
        //            //logger.addMessage("Sospensione non eseguita");
        //            logger.Debug("Sospensione non eseguita");
        //        };
        //        return false;
        //    }
        //    catch (Exception e)
        //    {
        //        //				logger.addMessage("La mail viene sospesa. Eccezione:"+e.Message); 
        //        logger.Debug("La mail viene sospesa. Eccezione:" + e.Message);
        //        err = "La mail viene sospesa. Eccezione:" + e.Message;
        //        if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
        //        {
        //            //					logger.addMessage("Sospensione eseguita");
        //            logger.Debug("Sospensione eseguita");
        //        }
        //        else
        //        {
        //            //					logger.addMessage("Sospensione non eseguita");
        //            logger.Debug("Sospensione non eseguita");
        //        };

        //        return false;
        //    }
        //    finally
        //    {
        //        xvr.Close();
        //        xtr.Close();
        //    }
        //    try
        //    {
        //        XmlElement elIdentificatore = (XmlElement)doc.DocumentElement.SelectSingleNode("Intestazione/Identificatore");
        //        XmlElement elmailOrigine = ((XmlElement)doc.DocumentElement.SelectSingleNode("Intestazione/Origine"));
        //        string mailOrigine = elmailOrigine.SelectSingleNode("IndirizzoTelematico").InnerText.Trim();
        //        string codiceAmministrazione = elIdentificatore.SelectSingleNode("CodiceAmministrazione").InnerText.Trim();
        //        string codiceAOO = elIdentificatore.SelectSingleNode("CodiceAOO").InnerText.Trim();
        //        string numeroRegistrazione = elIdentificatore.SelectSingleNode("NumeroRegistrazione").InnerText.Trim();
        //        string dataRegistrazione = DocsPaUtils.Functions.Functions.CheckData(elIdentificatore.SelectSingleNode("DataRegistrazione").InnerText.Trim());
        //        bool confermaRic = false;

        //        if (dataRegistrazione == null)
        //        {
        //            //					logger.addMessage("La mail viene sospesa. La data registrazione ha un formato non corretto"); 
        //            logger.Debug("La mail viene sospesa. La data registrazione ha un formato non corretto");
        //            err = "La mail viene sospesa. La data registrazione ha un formato non corretto";
        //            if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
        //            {
        //                //						logger.addMessage("Sospensione eseguita");
        //                logger.Debug("Sospensione eseguita");
        //            }
        //            else
        //            {
        //                //						logger.addMessage("Sospensione non eseguita");
        //                logger.Debug("Sospensione non eseguita");
        //            };
        //            return false;
        //        }
        //        //				logger.addMessage("Data registrazione: "+dataRegistrazione);
        //        logger.Debug("Data registrazione: " + dataRegistrazione);

        //        string oggetto = doc.DocumentElement.SelectSingleNode("Intestazione/Oggetto").InnerText.Trim();

        //        //MITTENTE
        //        XmlElement elOrigine = (XmlElement)doc.DocumentElement.SelectSingleNode("Intestazione/Origine");
        //        XmlElement elMittente = (XmlElement)elOrigine.SelectSingleNode("Mittente");
        //        DocsPaVO.addressbook.TipoUtente tipoMittente;
        //        if (codiceAmministrazione.Equals(reg.codAmministrazione))
        //        {
        //            tipoMittente = DocsPaVO.addressbook.TipoUtente.INTERNO;
        //        }
        //        else
        //        {
        //            tipoMittente = DocsPaVO.addressbook.TipoUtente.ESTERNO;
        //        }
        //        DocsPaVO.utente.Corrispondente mittente = getMittente(elMittente, mailOrigine, codiceAmministrazione, tipoMittente, reg);
        //        if (mittente == null)
        //        {
        //            //se il mittente è interno, allora ci si blocca!!!!!
        //            if (tipoMittente == DocsPaVO.addressbook.TipoUtente.INTERNO)
        //            {
        //                //						logger.addMessage("La mail viene sospesa: mittente non trovato");
        //                logger.Debug("La mail viene sospesa: mittente non trovato");
        //                err = "La mail viene sospesa: mittente non trovato nella rubrica.";
        //                if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
        //                {
        //                    //							logger.addMessage("Sospensione eseguita");
        //                    logger.Debug("Sospensione eseguita");
        //                }
        //                else
        //                {
        //                    //							logger.addMessage("Sospensione non eseguita");
        //                    logger.Debug("Sospensione non eseguita");
        //                };
        //                return false;
        //            }
        //            else
        //            {
        //                mittente = addNewCorrispondente(elOrigine, elIdentificatore, reg);
        //                //AGGIORNAMENTO CANALE PREFERENZIALE
        //                updateCanalePref(mittente);
        //            }
        //        }
        //        else
        //        {
        //            //					logger.addMessage("Mittente trovato"+mittente.GetType());
        //            logger.Debug("Mittente trovato" + mittente.GetType());

        //            //AGGIORNAMENTO DELLA MAIL E DELL'INTEROPERABILITA' DELLA UO DEL MITTENTE
        //            string mailMittente = elOrigine.SelectSingleNode("IndirizzoTelematico").InnerText.Trim();
        //            updateUOMittente(mittente, mailMittente);
        //            //AGGIORNAMENTO CANALE PREFERENZIALE
        //            updateCanalePref(mittente);
        //        }

        //        //DESTINATARI
        //        string infoDestinatari = getInfoDestinatari((XmlElement)doc.DocumentElement.SelectSingleNode("Intestazione"), ref confermaRic, mailAddress);
        //        //				logger.addMessage("Conferma ricezione: "+confermaRic);
        //        logger.Debug("Conferma ricezione: " + confermaRic);

        //        //DESCRIZIONE E DOCUMENTI
        //        //DOCUMENTO PRINCIPALE
        //        string docPrincipaleName = "";
        //        XmlElement descrizione = (XmlElement)doc.DocumentElement.SelectSingleNode("Descrizione");
        //        if (descrizione.SelectSingleNode("TestoDelMessaggio") != null)
        //        {
        //            docPrincipaleName = "body.rtf";
        //        }
        //        else
        //        {
        //            //si verifica se il tipo riferimento è MIME e se è presente il nome del documento
        //            if (!descrizione.SelectSingleNode("Documento").Attributes["tipoRiferimento"].Value.Equals("MIME"))
        //            {
        //                //						logger.addMessage("La mail viene sospesa. Il documento principale non ha MIME come tipo riferimento"); 
        //                logger.Debug("La mail viene sospesa. Il documento principale non ha MIME come tipo riferimento");
        //                err = "La mail viene sospesa. Il documento principale non ha MIME come tipo riferimento";
        //                if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
        //                {
        //                    //							logger.addMessage("Sospensione eseguita");
        //                    logger.Debug("Sospensione eseguita");
        //                }
        //                else
        //                {
        //                    //							logger.addMessage("Sospensione non eseguita");
        //                    logger.Debug("Sospensione non eseguita");
        //                };
        //                return false;
        //            }
        //            if (descrizione.SelectSingleNode("Documento").Attributes["nome"] == null)
        //            {
        //                //						logger.addMessage("La mail viene sospesa. Non e' presente il nome del documento principale"); 
        //                logger.Debug("La mail viene sospesa. Non e' presente il nome del documento principale");
        //                err = "La mail viene sospesa. Non e' presente il nome del documento principale";
        //                if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
        //                {
        //                    //							logger.addMessage("Sospensione eseguita");
        //                    logger.Debug("Sospensione eseguita");
        //                }
        //                else
        //                {
        //                    //							logger.addMessage("Sospensione non eseguita");
        //                    logger.Debug("Sospensione non eseguita");
        //                };
        //                return false;
        //            }
        //            docPrincipaleName = descrizione.SelectSingleNode("Documento").Attributes["nome"].Value;
        //        }

        //        //CONTROLLO DI CONSISTENZA DEI NOMI DEI DOCUMENTI
        //        //Documento principale
        //        if (!System.IO.File.Exists(filepath + "\\" + docPrincipaleName))
        //        {
        //            //					logger.addMessage("La mail viene sospesa. Il documento principale indicato non e' presente"); 
        //            logger.Debug("La mail viene sospesa. Il documento principale indicato non e' presente");
        //            err = "La mail viene sospesa. Il documento principale indicato non e' presente";
        //            if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
        //            {
        //                //						logger.addMessage("Sospensione eseguita");
        //                logger.Debug("Sospensione eseguita");
        //            }
        //            else
        //            {
        //                //						logger.addMessage("Sospensione non eseguita");
        //                logger.Debug("Sospensione non eseguita");
        //            };
        //            return false;
        //        };
        //        if (getApp(docPrincipaleName) == null)
        //        {
        //            //					logger.addMessage("La mail viene sospesa. Il formato file del documento principale non e' gestito"); 
        //            logger.Debug("La mail viene sospesa. Il formato file del documento principale non e' gestito");
        //            err = "La mail viene sospesa. Il formato file del documento principale non e' gestito";
        //            if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
        //            {
        //                //						logger.addMessage("Sospensione eseguita");
        //                logger.Debug("Sospensione eseguita");
        //            }
        //            else
        //            {
        //                //						logger.addMessage("Sospensione non eseguita");
        //                logger.Debug("Sospensione non eseguita");
        //            };
        //            return false;
        //        }
        //        //Allegati
        //        //XmlElement allegati=(XmlElement)  doc.DocumentElement.SelectSingleNode("Descrizione/Allegati");
        //        XmlNodeList documenti = doc.DocumentElement.SelectNodes("Descrizione/Allegati/Documento[@tipoRiferimento='MIME']");
        //        for (int ind = 0; ind < documenti.Count; ind++)
        //        {
        //            //si verifica se e' specificato il nome dell'allegato
        //            if (((XmlElement)documenti[ind]).Attributes["nome"] == null)
        //            {
        //                //						logger.addMessage("La mail viene sospesa. Non e' presente il nome dell'allegato "+ind); 
        //                logger.Debug("La mail viene sospesa. Non e' presente il nome dell'allegato " + ind);
        //                err = "La mail viene sospesa. Non e' presente il nome dell'allegato " + ind;
        //                if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
        //                {
        //                    //							logger.addMessage("Sospensione eseguita");
        //                    logger.Debug("Sospensione eseguita");
        //                }
        //                else
        //                {
        //                    //							logger.addMessage("Sospensione non eseguita");
        //                    logger.Debug("Sospensione non eseguita");
        //                };
        //                return false;
        //            }
        //            string nome = ((XmlElement)documenti[ind]).Attributes["nome"].Value;
        //            if (!System.IO.File.Exists(filepath + "\\" + nome))
        //            {
        //                //						logger.addMessage("La mail viene sospesa. Il documento "+nome+" non e' presente"); 
        //                logger.Debug("La mail viene sospesa. Il documento " + nome + " non e' presente");
        //                err = "La mail viene sospesa. Il documento " + nome + " non e' presente";

        //                if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
        //                {
        //                    //							logger.addMessage("Sospensione eseguita");
        //                    logger.Debug("Sospensione eseguita");
        //                }
        //                else
        //                {
        //                    //							logger.addMessage("Sospensione non eseguita");
        //                    logger.Debug("Sospensione non eseguita");
        //                };
        //                return false;
        //            };
        //            if (getApp(nome) == null)
        //            {
        //                //						logger.addMessage("La mail viene sospesa. Il documento "+nome+" ha un formato non gestito"); 
        //                logger.Debug("La mail viene sospesa. Il documento " + nome + " ha un formato non gestito");
        //                err = "La mail viene sospesa. Il documento " + nome + " ha un formato non gestito";
        //                if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
        //                {
        //                    //							logger.addMessage("Sospensione eseguita");
        //                    logger.Debug("Sospensione eseguita");
        //                }
        //                else
        //                {
        //                    //							logger.addMessage("Sospensione non eseguita");
        //                    logger.Debug("Sospensione non eseguita");
        //                };
        //                return false;
        //            }
        //        }

        //        //fatti i controlli, si procede con la protocollazione
        //        //				logger.addMessage("Inserimento documento principale "+docPrincipaleName);
        //        logger.Debug("Inserimento documento principale " + docPrincipaleName);

        //        DocsPaVO.documento.SchedaDocumento sd = new DocsPaVO.documento.SchedaDocumento();
        //        sd.appId = getApp(docPrincipaleName).application;
        //        sd.idPeople = infoUtente.idPeople;
        //        sd.userId = infoUtente.userId;
        //        //sd.note=infoDestinatari; 
        //        DocsPaVO.documento.Oggetto ogg = new DocsPaVO.documento.Oggetto();
        //        ogg.descrizione = mailSubject;
        //        sd.oggetto = ogg;
        //        sd.predisponiProtocollazione = false;//true;

        //        //old 28/04/08    sd.registro = reg;
        //        sd.registro = CaricaRegistroInScheda(reg);

        //        sd.tipoProto = "A";
        //        // sd.typeId = "MAIL";
        //        sd.typeId = "INTEROPERABILITA";
        //        sd.mezzoSpedizione = BusinessLogic.Documenti.InfoDocManager.getIdMezzoSpedizioneByDesc("INTEROPERABILITA");
        //        sd.descMezzoSpedizione = "INTEROPERABILITA";
        //        sd.interop = "S";
        //        //aggiunta protocollo entrata
        //        DocsPaVO.documento.ProtocolloEntrata protEntr = new DocsPaVO.documento.ProtocolloEntrata();
        //        protEntr.mittente = mittente;
        //        protEntr.dataProtocolloMittente = dataRegistrazione;
        //        protEntr.descrizioneProtocolloMittente = codiceAOO + DocsPaDB.Utils.Personalization.getInstance(reg.idAmministrazione).getSepSegnatura() + numeroRegistrazione;  //METTERE CARATTERE DELL'AMMINISTRAZIONE
        //        if (confermaRic)
        //        {
        //            protEntr.invioConferma = "1";
        //        }
        //        sd.protocollo = protEntr;

        //        //dati utente/ruolo/Uo del creatore.
        //        sd.protocollatore = new DocsPaVO.documento.Protocollatore(infoUtente, ruolo);

        //        sd = BusinessLogic.Documenti.DocSave.addDocGrigia(sd, infoUtente, ruolo);
        //        //				logger.addMessage("Salvataggio doc...");
        //        logger.Debug("Salvataggio doc...");

        //        sd = BusinessLogic.Documenti.DocSave.save(infoUtente, sd, false, out daAggiornareUffRef, ruolo);
        //        //				logger.addMessage("Salvataggio eseguito");
        //        logger.Debug("Salvataggio eseguito");

        //        try
        //        {
        //            DocsPaVO.documento.FileDocumento fd = new DocsPaVO.documento.FileDocumento();
        //            fs = new System.IO.FileStream(filepath + "\\" + docPrincipaleName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
        //            byte[] buffer = new byte[fs.Length];
        //            fs.Read(buffer, 0, (int)fs.Length);
        //            fd.content = buffer;
        //            fd.length = buffer.Length;
        //            fd.name = docPrincipaleName;
        //            DocsPaVO.documento.FileRequest fRSch = (DocsPaVO.documento.FileRequest)sd.documenti[0];
        //            if (fd.content.Length > 0)
        //            {
        //                //OLD :  BusinessLogic.Documenti.FileManager.putFile((DocsPaVO.documento.FileRequest)sd.documenti[0], fd, infoUtente);
        //                if (!BusinessLogic.Documenti.FileManager.putFile(ref fRSch, fd, infoUtente, out err))
        //                    throw new Exception(err);
        //                else
        //                    logger.Debug("Documento principale inserito");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            if (err == "")
        //                err = "Errore nel reperimento del file principale";
        //            //se il putFile va in errore devo rimuovere il profile (predisposto appena inserito)
        //            BusinessLogic.Documenti.DocManager.ExecRimuoviSchedaMethod(infoUtente, sd);
        //            logger.Debug("Eseguita rimozione profilo");
        //            logger.Debug(err);
        //            throw ex;
        //        }
        //        //ricerca degli allegati
        //        logger.Debug("Inserimento degli allegati");

        //        for (int i = 0; i < documenti.Count; i++)
        //        {
        //            //estrazione dati dell'allegato
        //            XmlElement documentoAllegato = (XmlElement)documenti[i];
        //            string nomeAllegato = documentoAllegato.Attributes["nome"].Value;
        //            XmlElement numPagine = (XmlElement)documentoAllegato.SelectSingleNode("NumeroPagine");
        //            XmlElement titoloDoc = (XmlElement)documentoAllegato.SelectSingleNode("TitoloDocumento");
        //            //					logger.addMessage("Inserimento allegato "+nomeAllegato);
        //            logger.Debug("Inserimento allegato " + nomeAllegato);

        //            DocsPaVO.documento.Allegato all = new DocsPaVO.documento.Allegato();
        //            all.descrizione = "allegato " + i;
        //            //					logger.addMessage("docnumber="+sd.docNumber);
        //            logger.Debug("docnumber=" + sd.docNumber);

        //            all.docNumber = sd.docNumber;
        //            //all.applicazione=getApp(nomeAllegato,logger);
        //            all.fileName = getFileName(nomeAllegato);
        //            all.version = "0";
        //            //numero pagine
        //            if (numPagine != null && !numPagine.InnerText.Trim().Equals(""))
        //            {
        //                all.numeroPagine = Int32.Parse(numPagine.InnerText);
        //            }
        //            //descrizione allegato
        //            if (titoloDoc != null && !titoloDoc.InnerText.Trim().Equals(""))
        //            {
        //                all.descrizione = titoloDoc.InnerText;
        //            }

        //            try
        //            {
        //                BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, all);
        //            }
        //            catch (Exception e)
        //            {
        //                err = "errore nel metodo aggiungiAllegato per l'allegato n. " + Convert.ToString(i + 1);
        //                logger.Debug(err);
        //                throw e;
        //            }


        //            #region Codice Commentato
        //            //					logger.addMessage("Allegato id="+all.versionId);
        //            //					logger.addMessage("Allegato version label="+all.versionLabel);
        //            //					logger.addMessage("Inserimento nel filesystem");
        //            #endregion

        //            logger.Debug("Allegato id=" + all.versionId);
        //            logger.Debug("Allegato version label=" + all.versionLabel);
        //            logger.Debug("Inserimento nel filesystem");

        //            try
        //            {
        //                DocsPaVO.documento.FileDocumento fdAll = new DocsPaVO.documento.FileDocumento();
        //                fsAll = new System.IO.FileStream(filepath + "\\" + nomeAllegato, System.IO.FileMode.Open, System.IO.FileAccess.Read);
        //                byte[] bufferAll = new byte[fsAll.Length];
        //                fsAll.Read(bufferAll, 0, (int)fsAll.Length);
        //                fdAll.content = bufferAll;
        //                fdAll.length = bufferAll.Length;
        //                fdAll.name = nomeAllegato;
        //                DocsPaVO.documento.FileRequest fRAll = (DocsPaVO.documento.FileRequest)sd.documenti[0];
        //                fRAll = (DocsPaVO.documento.FileRequest)all;
        //                if (fdAll.content.Length > 0)
        //                {
        //                    //OLD:  BusinessLogic.Documenti.FileManager.putFile((DocsPaVO.documento.FileRequest)all, fdAll, infoUtente);
        //                    if (!BusinessLogic.Documenti.FileManager.putFile(ref fRAll, fdAll, infoUtente, out err))
        //                        throw new Exception(err);
        //                    else
        //                        logger.Debug("Allegato " + i + " inserito");
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                if (err == "")
        //                    err = "Errore nel reperimento del file allegato";
        //                //se il putFile va in errore devo rimuovere il profile (predisposto appena inserito)
        //                BusinessLogic.Documenti.DocManager.ExecRimuoviSchedaMethod(infoUtente, sd);
        //                logger.Debug("Eseguita rimozione profilo");
        //                logger.Debug(err);
        //                throw ex;
        //            }
        //            finally
        //            {

        //                fsAll.Close();
        //            }
        //        }

        //        //TRASMISSIONE   
        //        //				logger.addMessage("Esegui trasmissione...");
        //        logger.Debug("Esegui trasmissione...");

        //        //eseguiTrasmissione(infoUtente.idPeople,serverName, sd,infoDestinatari,reg,ruolo);


        //        //eseguiTrasmissione(infoUtente.idPeople,serverName,sd,null,reg,ruolo);
        //        try
        //        {
        //            if (!string.IsNullOrEmpty(infoDestinatari) && infoDestinatari.Length > 248)
        //                infoDestinatari = infoDestinatari.Substring(0, 248);
        //            eseguiTrasmissione(infoUtente.idPeople, serverName, sd, infoDestinatari, reg, ruolo, infoUtente.dst);

        //            if (!checkExecTrasm(sd.systemId, "INTEROPERABILITA"))
        //            {
        //                codint1 = true;
        //                err = "CODINTEROP1 Trasmissione per INTEROPERABILITA del documento con id:" + sd.docNumber + " non effettuata. Il documento può essere trovato tra i predisposti.";
        //                throw new Exception("(CODINTEROP1) Trasmissione per INTEROPERABILITA del documento con id:" + sd.docNumber + "non effettuata!");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //                if (InteroperabilitaUtils.MailElaborata(mailId, "E", reg.systemId))
        //                    logger.Debug("La mail è stata elaborata");
        //                else
        //                    logger.Debug("La mail non è stata elaborata");

        //            if (sd != null && sd.systemId != null && sd.systemId != "" && !codint1)
        //            {
        //                BusinessLogic.Documenti.DocManager.ExecRimuoviSchedaMethod(infoUtente, sd);
        //                logger.Debug("Eseguita rimozione profilo");
        //            }
        //            logger.Debug(err);
        //            throw ex;
        //        }
        //        //modifica di luca
        //        logger.Debug("Protocollazione documento in caso di registro AUTOMATICO");

        //        //dafault OK

        //        DocsPaVO.documento.ResultProtocollazione resultProtocollazione = DocsPaVO.documento.ResultProtocollazione.OK;
        //        sd = BusinessLogic.Documenti.ProtoManager.protocolla(sd, ruolo, infoUtente, out resultProtocollazione);
        //        if (resultProtocollazione != DocsPaVO.documento.ResultProtocollazione.OK)
        //        {
        //            err = "Errore durante la protocollazione automatica batch da interoperabilità del doc id: " + sd.systemId + "Errore " + resultProtocollazione.ToString();
        //            logger.Debug(err);
        //            throw new Exception("Errore durante la protocollazione automatica batch da interoperabilità del doc id: " + "Errore " + resultProtocollazione.ToString());
        //        }
        //        //chiusura canali
        //        fs.Close();
        //        xvr.Close();
        //        xtr.Close();
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        //				logger.addMessage("La mail viene sospesa. Eccezione: "+e.ToString()); 
        //        logger.Debug("La mail viene sospesa. Eccezione: " + e.Message.ToString());
        //        logger.Debug("Errone nell'interoperabilità. La mail viene sospesa.", e);
        //        if (err == "")
        //            err = "Errone nell'interoperabilità. La mail viene sospesa." + e.Message.ToString();

        //        if (err.Contains("CODINTEROP1"))
        //        {
        //            if (InteroperabilitaUtils.MailElaborata(mailId, "E", reg.systemId))
        //                logger.Debug("La mail è stata elaborata");
        //            else
        //                logger.Debug("La mail non è stata elaborata");
        //        }
        //        else
        //        {
        //            if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
        //                logger.Debug("Sospensione eseguita, errore: " + err);
        //            else
        //                logger.Debug("Sospensione non eseguita, errore " + err);

        //        }

        //        xvr.Close();
        //        xtr.Close();
        //        if (fsAll != null) fsAll.Close();
        //        if (fs != null) fs.Close();
        //        return false;
        //    }
        //}
        #endregion
    }
}
