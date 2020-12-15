using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Xml.Serialization;
using BusinessLogic.Documenti.DigitalSignature;
using DocsPaVO.documento;
using log4net;
using System.Data;
using BusinessLogic.Interoperabilità;
using System.Globalization;
using System.Configuration;
using DocsPaUtils.Security;
using DocsPaVO.utente;
using System.Collections;
using DocsPaVO.Spedizione;


namespace BusinessLogic.interoperabilita
{
    public class InteroperabilitaEccezioni
    {
        public string eccezione_xml = null;
        //Andrea De Marco - Variabile booleana aggiunta per gestire i controlli Bloccanti/non bloccanti
        public bool controlloBloccante = true;
        string dettagli_eccezione = string.Empty;
        //End Andrea De Marco
        public string numRegMitt = string.Empty;
        public bool documentOk = false;
        private static ILog logger = LogManager.GetLogger(typeof(InteroperabilitaEccezioni));

        public static bool processaXmlEccezioni(string path, string filename, DocsPaVO.utente.Registro reg, InfoUtente infoUtente, string mailId, Interoperabilità.CMMsg mc, DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed mailProcessed)
        {
            logger.DebugFormat("Processo eccezione.xml {0} inviato da {1} ", mailId, mc.from);

            string moreError;

            //Verifico la validità della segnatura con Xsd
            bool isSignatureValid = interoperabilita.InteroperabilitaEccezioni.isSignatureValid(System.IO.Path.Combine(path, filename));
            if (!isSignatureValid)
                throw new System.Xml.Schema.XmlSchemaException();

            XmlDocument doc = new XmlDocument();
            InteropResolver my = new InteropResolver();
            XmlTextReader xtr = new XmlTextReader(System.IO.Path.Combine(path, filename)) { Namespaces = false };
            xtr.WhitespaceHandling = WhitespaceHandling.None;
            XmlValidatingReader xvr = new XmlValidatingReader(xtr);
            xvr.ValidationType = System.Xml.ValidationType.DTD;
            xvr.EntityHandling = System.Xml.EntityHandling.ExpandCharEntities;
            xvr.XmlResolver = my;
            moreError = string.Empty;

            try
            {
                doc.Load(xvr);
            }
            catch (System.Xml.Schema.XmlSchemaException e)
            {
                logger.Error("La mail viene sospesa perche' il  file eccezione.xml non e' valido. Eccezione:" + e.Message);
                moreError = "La mail viene sospesa perche' il  file eccezione.xml non e' valido. Eccezione:" + e.Message;

                if (InteroperabilitaUtils.MailElaborata(mailId, "D"))
                {
                    logger.Debug("Sospensione eseguita");
                }
                else
                {
                    logger.Debug("Sospensione non eseguita");
                }

                return false;
            }
            catch (Exception e)
            {
                logger.Error("La mail viene sospesa. Eccezione:" + e.Message);
                moreError = "La mail viene sospesa. Eccezione:" + e.Message;

                if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
                {
                    logger.Debug("Sospensione eseguita");
                }
                else
                {
                    logger.Debug("Sospensione non eseguita");
                }

                return false;
            }
            finally
            {
                xvr.Close();
                xtr.Close();
            }
            string docNumber = null;
            string motivo = null;
            string codiceAmministrazione = string.Empty;
            string codiceAOO = string.Empty;
            string numeroRegistrazione = string.Empty;
            DateTime dataRegistrazione = DateTime.MinValue;


            try
            {
                CultureInfo ci = new CultureInfo("it-IT");
                string[] formati = { "yyyy-MM-dd" };
                try
                {
                    XmlElement elIdentificatore = (XmlElement)doc.DocumentElement.SelectSingleNode("MessaggioRicevuto/Identificatore");
                    codiceAmministrazione = elIdentificatore.SelectSingleNode("CodiceAmministrazione").InnerText.Trim();
                    codiceAOO = elIdentificatore.SelectSingleNode("CodiceAOO").InnerText.Trim();
                    numeroRegistrazione = elIdentificatore.SelectSingleNode("NumeroRegistrazione").InnerText.Trim();
                    dataRegistrazione = DateTime.ParseExact(elIdentificatore.SelectSingleNode("DataRegistrazione").InnerText.Trim(), formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                    logger.Debug("Ricerca id del profilo...");
                    docNumber = Interoperabilità.InteroperabilitaUtils.findIdProfile(codiceAOO, numeroRegistrazione, dataRegistrazione.Year);
                }
                catch
                {
                    logger.Error("Il file eccezione.xml non contiene i dati per risalire al documento, controllo nell'oggetto descrizione");
                }


                if (docNumber == null)
                {
                    try
                    {
                        XmlElement elMsgRicevuto = (XmlElement)doc.DocumentElement.SelectSingleNode("MessaggioRicevuto");
                        string DescrizioneMessaggio = elMsgRicevuto.SelectSingleNode("DescrizioneMessaggio").InnerText.Trim();

                        docNumber = BusinessLogic.Interoperabilità.InteroperabilitaRicezione.extractDocNumberFromSubject(DescrizioneMessaggio);
                    }
                    catch
                    {
                        logger.Debug("Il file eccezione.xml non contiene i dati per risalire al documento, controllo nell'oggetto descrizione");

                    }
                }

                if (docNumber != null)
                {
                    XmlElement elMotivo = (XmlElement)doc.DocumentElement.SelectSingleNode("Motivo");
                    motivo = elMotivo.InnerText.Trim();

                }



                if (docNumber == null)
                {
                    moreError = "La mail viene sospesa: il documento indicato non è stato trovato";
                    logger.Debug("La mail viene sospesa: il documento indicato non è stato trovato");
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

                AggiornaDpa_StatoInvioConEccezione(mc.from, docNumber, motivo);


            }

            catch { };

            DocsPaVO.DatiCert.Notifica notifica = BusinessLogic.interoperabilita.InteroperabilitaManager.ricercaNotifiche(docNumber).FirstOrDefault();
            
            //Creazione in DPA_NOTIFICA  
            DocsPaVO.DatiCert.TipoNotifica tiponotifica = BusinessLogic.interoperabilita.InteroperabilitaManager.ricercaTipoNotificaByCodice("eccezione");
            if (tiponotifica == null)
                BusinessLogic.interoperabilita.InteroperabilitaManager.inserimentoTipoNotifica("eccezione");

            tiponotifica = BusinessLogic.interoperabilita.InteroperabilitaManager.ricercaTipoNotificaByCodice("eccezione");

            if (notifica == null)
            {
                logger.DebugFormat("Notifica per il docnumber {0} ha risetituito null, questo protebber essere un problema",docNumber ); 
                notifica = new DocsPaVO.DatiCert.Notifica();
                notifica.zona = "+100"; //default 
            }

            notifica.docnumber = docNumber;
            notifica.oggetto = motivo;
            notifica.mittente = mailProcessed.Recipients.OfType<string>().ToList().FirstOrDefault();
            notifica.destinatario = mailProcessed.From;

            notifica.tipoDestinatario = "esterno";
            notifica.idTipoNotifica = tiponotifica.idTipoNotifica;
            notifica.msgid = mailProcessed.MailID;

            BusinessLogic.interoperabilita.InteroperabilitaManager.inserimentoNotifica(notifica, null);
            // Modifica PEC 4 requisito 2
            logger.Debug("Scrivendo nella DPA_LOG. processaXMLEccezioni riga 193");
            // Modifica dei destinatari delle modifiche, imitando il comportamento IS

            string ruoloDestinatari = infoUtente.idGruppo;
            // Recupero il ruolo che ha effettuato l'ultima spedizione PEC, dallo storico delle spedizioni. 
            ArrayList listHistorySendDoc = BusinessLogic.Spedizione.SpedizioneManager.GetElementiStoricoSpedizione(notifica.docnumber);
            if (listHistorySendDoc != null && listHistorySendDoc.Count > 0)
            {
                Object lastSendPec = (from record in listHistorySendDoc.ToArray()
                                      where ((ElStoricoSpedizioni)record).Mail.Equals(notifica.destinatario) && ((ElStoricoSpedizioni)record).Esito.Equals("Spedito")
                                      select record).ToList().OrderBy(z => ((ElStoricoSpedizioni)z).Id).LastOrDefault();
                if (lastSendPec != null)
                {
                    ruoloDestinatari = ((ElStoricoSpedizioni)lastSendPec).IdGroupSender;
                }
            }

            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, ruoloDestinatari, infoUtente.idAmministrazione, "EXCEPTION_INTEROPERABILITY_PEC", notifica.docnumber, tiponotifica.descrizioneNotifica + " è stata inviata il '" + notifica.data_ora + "'"
                            + "'. Il destinatario '" + notifica.destinatario + "' ha una mail di tipo: '" + (notifica.tipoDestinatario.ToUpper().Equals("ESTERNO") ? "MAIL NON CERTIFICATA" : notifica.tipoDestinatario) + "'."
                                + "Il codice identificatore del messaggio è: '" + notifica.identificativo + "'.", DocsPaVO.Logger.CodAzione.Esito.KO, (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null),"1");
                
            return true;
        }

        public static bool isSignatureValid(string path)
        {
            bool result = true;
        
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.XmlResolver = null;
            settings.ProhibitDtd = false;
            XmlReader xreader = XmlTextReader.Create(path, settings);
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(xreader);

                //Se è presente il namespase valido con Xsd, altrimenti con DTD
                if (string.IsNullOrEmpty(doc.DocumentElement.NamespaceURI))
                {
                    LocalDtdResolver resolver = new LocalDtdResolver();
                    // Set the validation settings.
                    settings = new XmlReaderSettings();
                    settings.ProhibitDtd = false;
                    settings.ValidationType = ValidationType.DTD;
                    settings.XmlResolver = resolver;

                    XmlReader reader = XmlReader.Create(path, settings);
                    try
                    {
                        while (reader.Read()) ;
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Errore nella validazione della segnatura " + ex.Message);
                        result = false;
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
                else
                {
                    XmlSchemaCollection myschema = new XmlSchemaCollection();
                    XmlTextReader reader = null;

                    //Creo XML reader
                    //Incapsulo XML reader dentro Validiting reader
                    reader = new XmlTextReader(path);
                    XmlValidatingReader vReader = new XmlValidatingReader(reader);
                    String filenameXsd = AppDomain.CurrentDomain.BaseDirectory + @"xml\segnatura.xsd";
                    myschema.Add(null, filenameXsd);
                    vReader.ValidationType = ValidationType.Schema;
                    vReader.Schemas.Add(myschema);

                    //Leggo il file
                    //Se ci fossero errori viene chiamato l'handler
                    try
                    {
                        while (vReader.Read()) ;
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Errore nella validazione della segnatura " + ex.Message);
                        result = false;
                    }
                    finally
                    {
                        reader.Close();
                        vReader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore nella validazione della segnatura " + ex.Message);
                result = false;
            }
            xreader.Close();
            return result;
        }

        public static bool AggiornaDpa_StatoInvioConEccezione(string email, string docNumber, string motivo)
        {
            //Metodo ha problemi .. non sempre torna quello che vogliamo.
            //Avendo il DocNumber e l'email del mittente, cerco in dpa stato invio
            //Ma prima mi serve il corrid 


          
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            //Dato un docnumber cerco tutte le entry nella  stato invio
            ArrayList idCorrList = doc.GetIdCorrInStatoInvio(docNumber);

            List<DocsPaVO.documento.ProtocolloDestinatario> pdList = new List<ProtocolloDestinatario>();

            foreach (string corrId in idCorrList)
            {
                Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(corrId);
                corr.Emails = BusinessLogic.Utenti.addressBookManager.GetMailCorrispondente(corrId);

                if ((corr.Emails == null) || (corr.Emails.Count==0))
                {
                    List<MailCorrispondente> mcl = new List<MailCorrispondente> ();
                    mcl.Add ( new MailCorrispondente { Email =corr.email});
                    corr.Emails = mcl;
                }

                foreach (MailCorrispondente mcItem in corr.Emails)
                {
                    if (mcItem.Email == email)
                    {
                        ArrayList statoInvioAL = BusinessLogic.Documenti.ProtocolloUscitaManager.aggiornamentoConferma(docNumber, corr);
                        foreach (DocsPaVO.documento.ProtocolloDestinatario p in statoInvioAL)
                            pdList.Add(p);
                    }
                }

            }

            if (pdList.Count == 1)
            {
                bool res_update = updateStatoInvioAnnulla(pdList[0].systemId, motivo);
                if (!res_update)
                {
                    logger.Debug("Errore: non e' stato eseguito l'update del profilo");
                    return false;
                }
            }
            else
            {
                logger.DebugFormat("In aggiornamento stato invio il docnumber {0} ha restituito numero {1} entry, questo è un problema", docNumber,pdList.Count());
                foreach (DocsPaVO.documento.ProtocolloDestinatario p in pdList)
                    logger.DebugFormat("protocollodestinatario {0}  pd {1} ", p.systemId, p.protocolloDestinatario);
                
                //Urca! piu di uno e mo? bho, torno false..
                return false;
            }
            return true;
        }




        public static bool GestisciDSN(
                DocsPaVO.utente.InfoUtente infoUtente,
                DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed mailProcessed,
                string docNumber,
                Interoperabilità.CMMsg mc,
                string messageId,
                string filePath,
                string nomeEmail,
                DocsPaVO.utente.Registro reg
            )
        {
            string mailBody = mc.HTMLBody;
            if (mailBody == null)
                mailBody = mc.body;

            string isDSN = mc.getHeader("IsDSN");
            if (isDSN == "TRUE") //Double Check
            {
                string errorCode = mc.getHeader("Status");
                string reason = mc.getHeader("Diagnostic");
                string descrizioneErrore = String.Format("Ricevuta di ritorno delle Mail - DSN {0} : {1}", errorCode, reason);

                string failedRecepepientsList = mc.getHeader("FailedRecepients");
                string[] failedRecepients = null;

                if (failedRecepepientsList.Contains(";"))
                {
                    failedRecepients = failedRecepepientsList.Split(';');
                }
                foreach (string recepientInfo in failedRecepients)
                {
                    if (recepientInfo == string.Empty)
                        continue;

                    string[] recinfo = recepientInfo.Split('§');
                    string recepient = recinfo[0].Trim();
                    string descrizione = string.Empty;
                    if (recinfo.Length > 1)
                        descrizione += recinfo[1] + " ";
                    if (recinfo.Length > 2)
                        descrizione += recinfo[2];

                    bool retval = AggiornaDpa_StatoInvioConEccezione(recepient, docNumber, descrizione);
                }


                #region generazione allegato
                DocsPaVO.documento.Allegato all = new DocsPaVO.documento.Allegato();
                all.descrizione = descrizioneErrore;

                all.docNumber = docNumber;
                all.fileName = nomeEmail;
                all.version = "0";
                all.numeroPagine = 1;
                DocsPaVO.documento.Allegato allIns = null;
                try
                {
                    allIns = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegatoPEC(infoUtente, all);
                    // Set del flag in CHA_ALLEGATI_ESTERNO in Versions
                    BusinessLogic.Documenti.AllegatiManager.setFlagAllegati_PEC_IS_EXT(all.versionId, all.docNumber, "P");
                        
                }
                catch (Exception ex)
                {

                    logger.ErrorFormat("Problemi nell'inserire l'allegato per la DSN del messageID {0} : {1} ", messageId, ex.Message);
                }
                #endregion

                #region inserimento eml in allegato
                string err = null;
                System.IO.FileStream fsAll = null;
                try
                {
                    DocsPaVO.documento.SchedaDocumento sd = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, docNumber);

                    DocsPaVO.documento.FileDocumento fdAll = new DocsPaVO.documento.FileDocumento();

                    fsAll = new System.IO.FileStream(filePath + "\\" + nomeEmail, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                    byte[] bufferAll = new byte[fsAll.Length];
                    fsAll.Read(bufferAll, 0, (int)fsAll.Length);
                    fdAll.content = bufferAll;
                    fdAll.length = bufferAll.Length;

                    fdAll.name = nomeEmail;
                    fdAll.bypassFileContentValidation = true;
                    DocsPaVO.documento.FileRequest fRAll = (DocsPaVO.documento.FileRequest)sd.documenti[0];
                    fRAll = (DocsPaVO.documento.FileRequest)all;
                    if (fdAll.content.Length > 0)
                    {
                        logger.Debug("controllo se esiste l'ext");
                        if (!BusinessLogic.Documenti.DocManager.esistiExt(nomeEmail))
                            BusinessLogic.Documenti.DocManager.insertExtNonGenerico(nomeEmail, "application/octet-stream");

                        if (!BusinessLogic.Documenti.FileManager.putFile(ref fRAll, fdAll, infoUtente, out err))
                        {
                            logger.Debug("errore durante la putfile");
                            //  BusinessLogic.interoperabilita.InteroperabilitaManager.deleteNotifica(daticert.docnumber);
                            throw new Exception(err);

                        }

                    }

                }
                catch (Exception ex)
                {
                    if (err == "")
                        err = string.Format("Errore nel reperimento del file allegato: {0}.  {1}", nomeEmail, ex.Message);
                    BusinessLogic.Documenti.AllegatiManager.rimuoviAllegato(all, infoUtente);
                    logger.Error(err);
                }
                finally
                {
                    logger.Debug("chiusura degli stream dell'allegato daticert");

                    if (fsAll != null)
                        fsAll.Close();
                }

                #endregion

                #region generazione notifica
                //Creazione DPA_NOTIFICA  
                DocsPaVO.DatiCert.TipoNotifica tiponotifica = BusinessLogic.interoperabilita.InteroperabilitaManager.ricercaTipoNotificaByCodice("errore");
                if (tiponotifica == null)
                    BusinessLogic.interoperabilita.InteroperabilitaManager.inserimentoTipoNotifica("errore");

                tiponotifica = BusinessLogic.interoperabilita.InteroperabilitaManager.ricercaTipoNotificaByCodice("errore");

                // DocsPaVO.DatiCert.Daticert daticertAll = new DocsPaVO.DatiCert.Daticert();
                DocsPaVO.DatiCert.Notifica notifica = BusinessLogic.interoperabilita.InteroperabilitaManager.ricercaNotifiche(docNumber).FirstOrDefault();

                if (notifica == null)
                {
                    notifica = new DocsPaVO.DatiCert.Notifica();
                    notifica.zona = "+100"; //default 
                }

                notifica.docnumber = docNumber;
                notifica.mittente = mailProcessed.From;

                notifica.tipoDestinatario = "esterno";
                notifica.idTipoNotifica = tiponotifica.idTipoNotifica;
                notifica.msgid = mailProcessed.MailID;

                notifica.oggetto = descrizioneErrore;
                notifica.destinatario = mailProcessed.Recipients.OfType<string>().ToList().FirstOrDefault();
                notifica.destinatario = notifica.destinatario.Replace("<", String.Empty).Replace(">", String.Empty);

                BusinessLogic.interoperabilita.InteroperabilitaManager.inserimentoNotifica(notifica, allIns.versionId);
                // Modifica PEC 4 requisito 2
                // Modifica dei destinatari delle modifiche, imitando il comportamento IS

                string ruoloDestinatari = infoUtente.idGruppo;
                 // Recupero il ruolo che ha effettuato l'ultima spedizione PEC, dallo storico delle spedizioni. 
                ArrayList listHistorySendDoc = BusinessLogic.Spedizione.SpedizioneManager.GetElementiStoricoSpedizione(notifica.docnumber);
                if (listHistorySendDoc != null && listHistorySendDoc.Count > 0)
                {
                    Object lastSendPec = (from record in listHistorySendDoc.ToArray()
                                         where ((ElStoricoSpedizioni)record).Mail.ToLower().Equals(notifica.destinatario.ToLower()) && ((ElStoricoSpedizioni)record).Esito.Equals("Spedito")
                                         select record).ToList().FirstOrDefault();
                    if (lastSendPec != null)
                    {
                        ruoloDestinatari = ((ElStoricoSpedizioni)lastSendPec).IdGroupSender;
                    }
                }

                logger.Debug("Scrivendo nella DPA_LOG. GestisciDSN riga411.");
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, ruoloDestinatari, infoUtente.idAmministrazione, "NO_DELIVERY_SEND_PEC", notifica.docnumber, tiponotifica.descrizioneNotifica + " è stata inviata il '" + notifica.data_ora + "'"
               + "'. Il destinatario '" + notifica.destinatario + "' ha una mail di tipo: '" + (notifica.tipoDestinatario.ToUpper().Equals("ESTERNO") ? "MAIL NON CERTIFICATA" : notifica.tipoDestinatario) + "'."
                   + "Il codice identificatore del messaggio è: '" + notifica.identificativo + "'.", DocsPaVO.Logger.CodAzione.Esito.OK, (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null),"1");
                            

                #endregion

                return true;
            }
            return false;
        }




        static t DeserializeObject<t>(String pXmlizedString)
        {
            XmlSerializer xs = new XmlSerializer(typeof(t));
            MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);

            XmlTextReader reader = new XmlTextReader(new StringReader(pXmlizedString)) { Namespaces = false, ProhibitDtd = false, XmlResolver = null};
            try
            {
                return (t)xs.Deserialize(reader);
            }
            catch (Exception e) { System.Console.WriteLine(e); return default(t); }
        }

        static Byte[] StringToUTF8ByteArray(String pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }

        //NON VALIDIAMO RISPETTO ALLA DTD MA ALL'XSD
        static String SerializeObjectOld<t>(Object pObject)
        {
            try
            {
                String XmlizedString = null;
                MemoryStream memoryStream = new MemoryStream();
                XmlSerializer xs = new XmlSerializer(typeof(t));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                xmlTextWriter.Formatting = Formatting.Indented;
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();

                xmlTextWriter.WriteStartDocument();
                xmlTextWriter.WriteDocType(pObject.GetType().Name.ToString(), null, "Segnatura.dtd", null);

                ns.Add("", "");
                xs.Serialize(xmlTextWriter, pObject, ns);


                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                memoryStream.Position = 0;
                //XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
                StreamReader sr = new StreamReader(memoryStream);
                XmlizedString = sr.ReadToEnd();
                return XmlizedString;
            }
            catch (Exception e) { logger.Error(e); return null; }
        }

        static String SerializeObject<t>(Object pObject)
        {
            try
            {
                String XmlizedString = null;
                MemoryStream memoryStream = new MemoryStream();
                XmlSerializer xs = new XmlSerializer(typeof(t));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                xmlTextWriter.Formatting = Formatting.Indented;
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();

                //xmlTextWriter.WriteStartDocument();
                //xmlTextWriter.WriteDocType(pObject.GetType().Name.ToString(), null, "Segnatura.dtd", null);

                ns.Add("xmlns", "http://www.digitPa.gov.it/protocollo/");
    
                xs.Serialize(xmlTextWriter, pObject, ns);


                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                memoryStream.Position = 0;
                //XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
                StreamReader sr = new StreamReader(memoryStream);
                XmlizedString = sr.ReadToEnd();
                return XmlizedString.Replace("xmlns:", "");
            }
            catch (Exception e) { logger.Error(e); return null; }
        }

        string generaEccezioneXml(string motivo, string descrizioneMessaggio, segnatura.Identificatore identificatore)
        {
            segnatura.NotificaEccezione notificaEccezione = new segnatura.NotificaEccezione { versione = null };
            notificaEccezione.Motivo = new segnatura.Motivo { Text = new string[] { motivo } };

            segnatura.DescrizioneMessaggio dmsg = new segnatura.DescrizioneMessaggio { Text = new String[] { descrizioneMessaggio } };

            if (identificatore == null)
                notificaEccezione.MessaggioRicevuto = new segnatura.MessaggioRicevuto { Items = new object[] { dmsg } };
            else
                notificaEccezione.MessaggioRicevuto = new segnatura.MessaggioRicevuto { Items = new object[] { identificatore } };

            return SerializeObject<segnatura.NotificaEccezione>(notificaEccezione);

        }

        public InteroperabilitaEccezioni(Interoperabilità.CMMsg messaggio)
        {
            //Andrea De Marco
            //bool prosegui = true;
            //End Andrea De Marco
            string fileSegnatura = null;

            foreach (CMAttachment a in messaggio.attachments)
            {
                if (a.name.ToLower().Equals("segnatura.xml"))
                {
                    fileSegnatura = System.Text.ASCIIEncoding.ASCII.GetString(a._data);
                }

                //Commentato: esiste il metodo isDtdValid  che si occupa di verificare se Segnatura.xml è conforme alla DTD.
                ////Andrea De Marco - Gestione Eccezioni PEC - Controllo che il file segnatura.xml sia leggibile
                //if (a.name.ToLower().Equals("segnatura.xml"))
                //{
                //    try
                //    {
                //        //string xmlSeg = System.Text.ASCIIEncoding.ASCII.GetString(a._data);

                //        //LocalDtdResolver resolver = new LocalDtdResolver();
                //        //// Set the validation settings.
                //        //XmlReaderSettings settings = new XmlReaderSettings();
                //        ////settings.DtdProcessing = DtdProcessing.Parse;  //4.0
                //        //settings.ProhibitDtd = false;
                //        //settings.ValidationType = ValidationType.DTD;
                //        //settings.XmlResolver = resolver;

                //        //MemoryStream ms = new MemoryStream(System.Text.ASCIIEncoding.ASCII.GetBytes(xmlSeg));
                //        //XmlReader reader = XmlReader.Create(ms, settings);

                //        //while (reader.Read()) ;
                //    }
                //    catch 
                //    {
                //        prosegui = false;
                //    }
                    
                //    if (prosegui)
                //        fileSegnatura = System.Text.ASCIIEncoding.ASCII.GetString(a._data);
                //    else
                //    {
                //        string testo_eccezione = "Il documento risulta essere corrotto o non leggibile.";
                //        controlloBloccante = true;
                //        eccezione_xml = generaEccezioneXml(testo_eccezione, messaggio.subject, null);
                //    }
                //}
                ////End Andrea De Marco

            }
            if (fileSegnatura != null)
            {
                InteroperabilitaEccezioniCheck(messaggio, fileSegnatura);
            }
        }

        public InteroperabilitaEccezioni(Interoperabilità.CMMsg messaggio, string xml_segnatura)
        {
            InteroperabilitaEccezioniCheck(messaggio, xml_segnatura);
        }

        void InteroperabilitaEccezioniCheck(Interoperabilità.CMMsg messaggio, string xml_segnatura)
        {
            documentOk = false;
            numRegMitt = messaggio.subject;
            string testo_eccezione = string.Empty;
            //string istruzioni_utente = "Il documento è stato registrato in ingresso dall’Amministrazione ricevente e pertanto non occorre effettuare rispedizioni.";
            string istruzioni_utente = "Il documento è stato ricevuto dall’Amministrazione destinataria e pertanto non occorre effettuare rispedizioni.";

            if (isDtdValid(xml_segnatura, messaggio))
            {   //Segnatura Valida procedo con gli altri controlli

                //dato che segnatura è stato validato, deserializzarlo non dovrebbe dare problemi.
                segnatura.Segnatura sexml = DeserializeObject<segnatura.Segnatura>(xml_segnatura);
                segnatura.Identificatore identificatore = sexml.Intestazione.Identificatore;

                numRegMitt = identificatore.NumeroRegistrazione.Text.FirstOrDefault().ToString();
                if (isStrutturaValida(messaggio, sexml))
                {
                    //PROBLEMA MACERATA(email origine mancante) - DOVREBBE ESSERE MESSO UN CONTROLLO SULL'ORIGINE / INDIRIZZO TELEMATICO -- SABRINA
                    if (!this.isMailOriginePresente(messaggio, sexml))
                    {
                        testo_eccezione = "L’Indirizzo telematico di origine contenuto nella segnatura informatica non è valorizzato.";
                        //Andrea De Marco - Gestione Eccezioni Segnatura.xml PEC - Commentare per Ripristino
                        //Set variabile booleana per consentire eseguiSenzaSegnatura per controlli non bloccanti.
                        controlloBloccante = false;
                        testo_eccezione = testo_eccezione + istruzioni_utente;
                        //End Andrea De Marco
                        eccezione_xml = generaEccezioneXml(testo_eccezione, messaggio.subject, identificatore);
                    }
                    else
                    {
                        //Struttura Valida procedo con gli altri controlli
                        if (isDestinatarioValido(messaggio, sexml))
                        {
                            //Andrea De Marco - Controllo se i campi CodAmm e CodAOO sono validi - è un controllo non bloccante
                            //if (isCodAmmValido(sexml, ref testo_eccezione))
                            //{
                            //    if (isCodAOOValido(sexml, ref testo_eccezione))
                            //    {
                            //        List<Interoperabilità.CMAttachment> attachment = new List<Interoperabilità.CMAttachment>();

                            //        /*
                            //         * //Salto il coltrollo per la "linea soft" 
                            //         * //rimuovere quando si è definito osa fare
                            //         *
                            //         * //bool corrispondenteFederato = BusinessLogic.RubricaComune.RubricaServices.isCorrispondenteFederatoByEmail(null, messaggio.from.Trim());
                            //         * //logger.InfoFormat("Il corrispondente {0} è federato? :{1}", messaggio.from, corrispondenteFederato);
                            //         */

                            //        bool corrispondenteFederato = true;

                            //        segnatura.Documento docPrincipaleSegnatura = sexml.Descrizione.Item as segnatura.Documento;
                            //        if (isDocPrincipaleSottoScritto(docPrincipaleSegnatura, corrispondenteFederato))
                            //        {
                            //            if (isDocumentoPrincipaleIntegro(messaggio, docPrincipaleSegnatura, ref  attachment))
                            //            {
                            //                if (isDocumentoLeggibile(attachment))
                            //                {
                            //                    logger.Debug("Il Documento è OK");
                            //                    documentOk = true;
                            //                }
                            //                else
                            //                {
                            //                    testo_eccezione = "Il messaggio protocollato è corrotto o uno dei documenti informatici inclusi non è leggibile.";
                            //                    eccezione_xml = generaEccezioneXml(testo_eccezione, messaggio.subject, identificatore);
                            //                }
                            //            }
                            //            else
                            //            {
                            //                testo_eccezione = "La verifica di integrità di uno dei documenti informatici sottoscritti ha dato esito negativo.";
                            //                eccezione_xml = generaEccezioneXml(testo_eccezione, messaggio.subject, identificatore);
                            //            }
                            //        }
                            //        else
                            //        {
                            //            testo_eccezione = "Il Documento principale non risulta essere sottoscritto.";
                            //            eccezione_xml = generaEccezioneXml(testo_eccezione, messaggio.subject, identificatore);
                            //        }
                            //    }
                            //    else
                            //    {
                            //        //Codice AOO vuoto o con lunghezza > 16 caratteri (da normativa sono 8)
                            //        controlloBloccante = false;
                            //        testo_eccezione = testo_eccezione + istruzioni_utente;
                            //        eccezione_xml = generaEccezioneXml(testo_eccezione, messaggio.subject, identificatore);
                            //    }
                            //}
                            //else
                            //{
                            //    //Codice Amministrazione vuoto o con lunghezza > 16 caratteri (da normativa sono 8)
                            //    controlloBloccante = false;
                            //    testo_eccezione = testo_eccezione + istruzioni_utente;
                            //    eccezione_xml = generaEccezioneXml(testo_eccezione, messaggio.subject, identificatore);
                            //}
                            //End Andrea De Marco

                            //Decommentare il seguente codice e commentare il codice sopra (Andrea De Marco) per ripristinare la situazione precedente
                            List<Interoperabilità.CMAttachment> attachment = new List<Interoperabilità.CMAttachment>();

                            /*
                             * //Salto il coltrollo per la "linea soft" 
                             * //rimuovere quando si è definito osa fare
                             *
                             * //bool corrispondenteFederato = BusinessLogic.RubricaComune.RubricaServices.isCorrispondenteFederatoByEmail(null, messaggio.from.Trim());
                             * //logger.InfoFormat("Il corrispondente {0} è federato? :{1}", messaggio.from, corrispondenteFederato);
                             */

                            bool corrispondenteFederato = true;

                            segnatura.Documento docPrincipaleSegnatura = sexml.Descrizione.Item as segnatura.Documento;
                            segnatura.DocumentoTipoRiferimento tipoRiferimento = docPrincipaleSegnatura.tipoRiferimento;
                            if (!tipoRiferimento.ToString().ToUpper().Equals("CARTACEO"))
                            {
                                if (isDocPrincipaleSottoScritto(docPrincipaleSegnatura, corrispondenteFederato))
                                {
                                    if (isDocumentoPrincipaleIntegro(messaggio, docPrincipaleSegnatura, ref  attachment))
                                    {
                                        if (isDocumentoLeggibile(attachment))
                                        {
                                            logger.Debug("Il Documento è OK");
                                            documentOk = true;
                                        }
                                        else
                                        {
                                            testo_eccezione = "Il messaggio protocollato è corrotto o uno dei documenti informatici inclusi non è leggibile.";
                                            eccezione_xml = generaEccezioneXml(testo_eccezione, messaggio.subject, identificatore);
                                        }
                                    }
                                    else
                                    {
                                        testo_eccezione = "La verifica di integrità di uno dei documenti informatici sottoscritti ha dato esito negativo.";
                                        eccezione_xml = generaEccezioneXml(testo_eccezione, messaggio.subject, identificatore);
                                    }
                                }
                                else
                                {
                                    testo_eccezione = "Il Documento principale non risulta essere sottoscritto.";
                                    eccezione_xml = generaEccezioneXml(testo_eccezione, messaggio.subject, identificatore);
                                }
                            }
                            else
                            {
                                testo_eccezione = "La descrizione del destinatario contenuta nella segnatura informatica è errata.";
                                //Andrea De Marco - Gestione Eccezioni Segnatura.xml PEC - Commentare per Ripristino
                                //Set variabile booleana per consentire eseguiSenzaSegnatura per controlli non bloccanti.
                                controlloBloccante = false;
                                testo_eccezione = testo_eccezione + istruzioni_utente;
                                //End Andrea De Marco
                                eccezione_xml = generaEccezioneXml(testo_eccezione, messaggio.subject, identificatore);
                            }
                        }
                    }
                }// gestione mail origine non presente
                else
                {
                    testo_eccezione = "La descrizione del messaggio protocollato riportata nella segnatura informatica non corrisponde alla struttura di codifica.";
                    //Andrea De Marco - Gestione Eccezioni Segnatura.xml PEC - Commentare per Ripristino
                    //Set variabile booleana per consentire eseguiSenzaSegnatura per controlli non bloccanti.
                    controlloBloccante = false;
                    testo_eccezione = testo_eccezione + istruzioni_utente;
                    //End Andrea De Marco
                    eccezione_xml = generaEccezioneXml(testo_eccezione, messaggio.subject, identificatore);
                }
            }
            else
            {
                testo_eccezione = "Il file Segnatura.xsd non rispetta il formato descritto in segnatura.dtd. ";
                //Andrea De Marco - Gestione Eccezioni Segnatura.xml PEC - Commentare per Ripristino
                //Set variabile booleana per consentire eseguiSenzaSegnatura per controlli non bloccanti.
                controlloBloccante = false;
                //validaSegnatura valid = new validaSegnatura(xml_segnatura);
                //if (valid != null && valid.ValidationErrorList.Count() > 0)
                //{
                //    foreach (string ve in valid.ValidationErrorList)
                //        dettagli_eccezione = " " + ve.ToString() + "; ";
                //}
                testo_eccezione = testo_eccezione + dettagli_eccezione + istruzioni_utente;
                //End Andrea De Marco
                eccezione_xml = generaEccezioneXml(testo_eccezione, messaggio.subject, null);
            }
            if (eccezione_xml != null && eccezione_xml.Length > 0)
                logger.DebugFormat("Generata Eccezione {0}", testo_eccezione);

        }

        /// <summary>
        /// Caso C) il formato della segnatura informatica non è conforme alla DTD di cui alla presente circolare ovvero alla sua versione più recente;
        /// </summary>
        /// <returns></returns>
        bool isDtdValid(string segnaturaXml, Interoperabilità.CMMsg messaggio)
        {
            try
            {

                validaSegnatura valid = new validaSegnatura(segnaturaXml);
                if (valid.ValidationErrorList.Count() == 0)
                {
                    //Verifica che non ci sono errori nel caricamento dell'xml
                    if (!CheckErrorInLoadXml(messaggio))
                        return false;
                    return true;
                }
                //Aggiunto per il passaggio dalla vecchia segnatura alla nuova (che prevede il CodiceRegistro)
                //if (valid.ValidationErrorList.Count() == 1 && valid.ValidationErrorList[0].Contains("CodiceRegistro"))
                //    return true;

                foreach (string ve in valid.ValidationErrorList)
                    logger.DebugFormat("Errori durante la validazione di segnatura XML {0}", ve);

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Errori durante la validazione di segnatura XML {0}", e);
                dettagli_eccezione = " " + e.Message.ToString();
            }
            
            return false;
        }

        bool CheckErrorInLoadXml(Interoperabilità.CMMsg messaggio)
        {
            bool result = true;

            try
            {
                foreach (CMAttachment a in messaggio.attachments)
                {
                    if (a.name.ToLower().Equals("segnatura.xml"))
                    {
                        // Controllo encoding
                        XmlDocument doc = new XmlDocument();
                        InteropResolver my = new InteropResolver();
                        System.IO.MemoryStream ms = new System.IO.MemoryStream(a._data);
                        XmlTextReader xtr = new XmlTextReader(ms);
                        xtr.WhitespaceHandling = WhitespaceHandling.None;
                        XmlValidatingReader xvr = new XmlValidatingReader(xtr);
                        xvr.ValidationType = System.Xml.ValidationType.Schema;
                        xvr.EntityHandling = System.Xml.EntityHandling.ExpandCharEntities;
                        xvr.XmlResolver = my;
                        try
                        {
                            doc.Load(xvr);
                        }
                        catch (System.Xml.XmlException e)
                        {
                            logger.Error("Eccezione CheckErrorInLoadXml:" + e.Message);
                            logger.Debug("Errore CheckErrorInLoadXml stackTrace : " + e.StackTrace);
                            result = false;
                        }
                        catch (Exception e)
                        {
                            logger.Error("Eccezione:" + e.Message);
                        }
                        finally
                        {
                            xvr.Close();
                            xtr.Close();
                        }
                    }
                        
                }
            }
            catch(Exception e)
            {
                logger.Error("Errore in checkEncoding: " + e.Message);
            }

            return result;
        }

        /// <summary>
        /// Caso d) nel caso di comunicazione tra diverse amministrazioni il documento principale non risulta sottoscritto;
        /// </summary>
        bool isDocPrincipaleSottoScritto(segnatura.Documento docPrincipaleSegnatura, bool isCorrispondenteFederato)
        {
            //Se non esiste, o il nome è null, non posso fare i controlli successivi e comqunue un documento principale DEVE esistere
            if (docPrincipaleSegnatura == null)
                return false;
            if (docPrincipaleSegnatura.nome == null)
                return false;
            // se il corrispondente è federato, me ne frego e vado avanti uscendo con true
            if (isCorrispondenteFederato)
                return true;
            //nel caso non fosse federato allora controllo se l'estensione è p7m
            if (docPrincipaleSegnatura.nome.ToLower().EndsWith(".p7m"))
                return true;
            //In tutti gli altri casi non gestiti torno false   
            return false;

        }

        /// <summary>
        /// Caso B) la descrizione del messaggio protocollato riportata nella segnatura informatica non corrisponde alla struttura di codifica
        /// </summary>
        bool isStrutturaValida(Interoperabilità.CMMsg messaggio, segnatura.Segnatura sexml)
        {
            bool retval = true;
            List<String> lstAllegatiSegnatura = new List<string>();
            lstAllegatiSegnatura.Add("segnatura.xml"); //Esiste per forza.. se no non staremo qui.

            if (sexml.Descrizione.Allegati != null)
            {
                foreach (object obj in sexml.Descrizione.Allegati)
                {
                    segnatura.Documento allegatoSegnatura = obj as segnatura.Documento;
                    if (allegatoSegnatura != null)
                        lstAllegatiSegnatura.Add(allegatoSegnatura.nome.ToLower());
                }
            }

            if (sexml.Descrizione.Item != null)
            {
                segnatura.Documento docPrincipaleSegnatura = sexml.Descrizione.Item as segnatura.Documento;
                if (docPrincipaleSegnatura != null && docPrincipaleSegnatura.nome != null)
                    lstAllegatiSegnatura.Add(docPrincipaleSegnatura.nome.ToLower());
            }

            if (lstAllegatiSegnatura.Count != messaggio.attachments.Count)
                return false;

            foreach (Interoperabilità.CMAttachment att in messaggio.attachments)
                if (!lstAllegatiSegnatura.Contains(att.name.ToLower()))
                    return false;

            return retval;
        }

        /// <summary>
        /// Caso E) la descrizione del destinatario contenuta nella segnatura informatica è errata;
        /// </summary>
        bool isDestinatarioValido(Interoperabilità.CMMsg messaggio, segnatura.Segnatura sexml)
        {
            List<String> ListaEmail = new List<string>();

            segnatura.Destinazione[] destinatari = sexml.Intestazione.Destinazione as segnatura.Destinazione[];
            foreach (segnatura.Destinazione destinatario in destinatari)

            //PALUMBO: modifica per consentire scarico di mail con destinatari in segnatura sprovvisti di indirizzo email     
            //ListaEmail.Add(destinatario.IndirizzoTelematico.Text.FirstOrDefault());
            {
               if (destinatario.IndirizzoTelematico != null &&
              destinatario.IndirizzoTelematico.Text != null
              && destinatario.IndirizzoTelematico.Text.FirstOrDefault() != null)
                    ListaEmail.Add(destinatario.IndirizzoTelematico.Text.FirstOrDefault());                
            }

            foreach (Interoperabilità.CMRecipient r in messaggio.recipients)
            {
                if (!ListaEmail.Contains(r.mail))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Caso A) il messaggio protocollato è corrotto o uno dei documenti informatici inclusi non è leggibile (fatto dopo il caso F)
        /// </summary>
        bool isDocumentoLeggibile(List<Interoperabilità.CMAttachment> attachments)
        {
            Sa_Utils.FileTypeFinder ff = new Sa_Utils.FileTypeFinder();
            foreach (Interoperabilità.CMAttachment att in attachments)
            {
                string fileExtension = ff.FileType(att._data).ToLower();
                string extension = System.IO.Path.GetExtension(att.name).Replace(".", string.Empty);
                bool isValid = fileExtension.ToLower().Contains(extension.ToLower());

                //Per i file Zip, il validatore non riesce a distinguere uno zip da file office.
                if (!isValid && extension.ToLower().Equals("zip") && fileExtension.ToLower().Equals("ooxml"))
                    isValid = true;

                if (!isValid)
                {
                    logger.DebugFormat("Il controllo di formato sul documento principale da trovato differenze Riscontrato {0}  Atteso {1}", fileExtension, extension);
                    return false;
                }
            }
            return true;
        }

        static byte[] readBase64(byte[] inBase64Bytes)
        {
            MemoryStream msIn = new MemoryStream(inBase64Bytes);
            MemoryStream msOut = new MemoryStream();
            StreamReader sr = new StreamReader(msIn);
            while (true)
            {
                string line = sr.ReadLine();
                if (String.IsNullOrEmpty(line))
                    break;

                byte[] temp = null;

                try
                {
                    temp = Convert.FromBase64String(line);
                }
                catch
                {
                    return null;
                }
                msOut.Write(temp, 0, temp.Length);
            }
            return msOut.ToArray();
        }

        /// <summary>
        /// Caso F) la verifica di integrità di uno dei documenti informatici ha dato esito negativo (p7m)
        /// </summary>
        bool isDocumentoPrincipaleIntegro(Interoperabilità.CMMsg messaggio, segnatura.Documento docPrincipaleSegnatura, ref List<Interoperabilità.CMAttachment> attachment)
        {
            VerifySignature verifySignature = new VerifySignature();
            string inputDirectory = verifySignature.GetPKCS7InputDirectory();

            List<string> p7m_fileNameList = new List<string>();

            foreach (Interoperabilità.CMAttachment att in messaggio.attachments)
            {
                string fileName = att.name;
                if (fileName.ToLower().Trim() == docPrincipaleSegnatura.nome.ToLower().Trim())
                {
                    Interoperabilità.CMAttachment sb_att = new Interoperabilità.CMAttachment();
                    if (Path.GetExtension(fileName).ToLower().Contains(".p7m"))
                    {
                        //E' presente un P7M, lo controlliamo
                        FileDocumento fileDoc = new FileDocumento();
                        fileDoc.name = fileName;
                        fileDoc.content = att._data;
                        int oldLen = fileDoc.content.Length;
                        try
                        {
                            if (string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_CHECK_MAIL_NOT_READ_BASE_64")) ||
                                !DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_CHECK_MAIL_NOT_READ_BASE_64").ToString().Equals("1"))
                            {
                                byte[] deb64Content = readBase64(fileDoc.content);
                                if (deb64Content != null)
                                {
                                    fileDoc.content = deb64Content;
                                    fileDoc.length = fileDoc.content.Length;
                                }
                            }
                            BusinessLogic.Documenti.FileManager.VerifyFileSignature(fileDoc, null);
                        }
                        catch (Exception e)
                        {
                            logger.ErrorFormat("Qualcosa è andato storto durante la VerifyFileSignature {0}", e.Message);
                            //Qualcosa è andato storto...
                            return false;
                        }
                        //la dimensione non è cambiata.
                        if (fileDoc.content.Length == oldLen)
                            return false;

                        sb_att.contentType = att.contentType;
                        sb_att.name = fileDoc.name;
                        sb_att._data = fileDoc.content;
                        attachment.Add(sb_att);
                    }
                    else
                    {   //Il documento non è sottoscritto, potrebbe essere un corrispondente federato.
                        //Se sono arrivato qui non ha senso veriicare se è federato o no, dato che sarebbe stato bloccato dal controllo precendete
                        attachment.Add(att);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// COD_AMM minore di 16 caratteri - la normativa prevede al più 8 caratteri;
        /// </summary>
        bool isCodAmmValido(segnatura.Segnatura sexml, ref string testoEccezione)
        {
            bool valido = true;
            
            segnatura.Identificatore identificatore = sexml.Intestazione.Identificatore as segnatura.Identificatore;

            if (!string.IsNullOrEmpty(identificatore.CodiceAmministrazione.Text[0].ToString()))
            {
                if (identificatore.CodiceAmministrazione.Text[0].ToString().Length > 16)
                {
                    valido = false;
                    testoEccezione = "Il codice amministrazione deve avere una lunghezza non superiore a 16 caratteri.";
                }
            }
            else 
            {
                valido = false;
                testoEccezione = "Il codice amministrazione deve essere popolato.";
            }

            return valido;
        }


        /// <summary>
        /// COD_AOO minore di 16 caratteri - la normativa prevede al più 8 caratteri;
        /// </summary>
        bool isCodAOOValido(segnatura.Segnatura sexml, ref string testoEccezione)
        {
            bool valido = true;

            segnatura.Identificatore identificatore = sexml.Intestazione.Identificatore as segnatura.Identificatore;

            if (!string.IsNullOrEmpty(identificatore.CodiceAOO.Text[0].ToString()))
            {
                if (identificatore.CodiceAOO.Text[0].ToString().Length > 16)
                {
                    valido = false;
                    testoEccezione = "Il codice AOO deve avere una lunghezza non superiore a 16 caratteri.";
                }
            }
            else
            {
                valido = false;
                testoEccezione = "Il codice AOO deve essere popolato.";
            }

            return valido;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="reg"></param>
        public static bool sendNotificaEccezione(DocsPaVO.utente.Registro reg, string xmlString, string numRegMitt, string mailMitt)
        {

            System.Data.DataSet ds;
            bool esito = true;  //presume successo

            try
            {
                //BusinessLogic.Interoperabilità.InteroperabilitaControlloRicevute.processaRicevutaConferma();

                DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                obj.getCampiReg(out ds, reg);

                System.Data.DataRow regRow = ds.Tables["REGISTRO"].Rows[0];
                reg.codAmministrazione = regRow["VAR_CODICE_AMM"].ToString();

                //inserimento dei files in una cartella temporanea
                string basePathFiles = ConfigurationManager.AppSettings["LOG_PATH"];
                basePathFiles = basePathFiles.Replace("%DATA", DateTime.Now.ToString("yyyyMMdd"));
                basePathFiles = basePathFiles + "\\Invio_notifica_eccezione_files";
                string pathFiles = basePathFiles + "\\" + regRow["VAR_CODICE"].ToString();
                DocsPaUtils.Functions.Functions.CheckEsistenzaDirectory(pathFiles);

                //costruisciXml(idProfile, reg, ref numRegMitt, ref qco, separatore, pathFiles);
                System.IO.File.WriteAllText(Path.Combine(pathFiles, "Eccezione.xml"), xmlString);

                //invio mail
                logger.Debug("Invio mail all'indirizzo " + mailMitt);
                string porta = null;
                if (regRow["NUM_PORTA_SMTP"] != null)
                {
                    porta = regRow["NUM_PORTA_SMTP"].ToString();
                }
                string smtp_user = (regRow["VAR_USER_SMTP"] != null && regRow["VAR_USER_SMTP"] != System.DBNull.Value) ? regRow["VAR_USER_SMTP"].ToString() : null;
                string smtp_pwd;
                if (regRow["VAR_PWD_SMTP"] != null && regRow["VAR_PWD_SMTP"] != System.DBNull.Value && regRow["VAR_USER_SMTP"] != null && regRow["VAR_USER_SMTP"] != System.DBNull.Value)
                    smtp_pwd = Crypter.Decode(regRow["VAR_PWD_SMTP"].ToString(), regRow["VAR_USER_SMTP"].ToString());
                else
                    smtp_pwd = string.Empty;

                //aggiunta la trim() per gestire la presenza di spazi bianchi nei campi VAR_USER_SMTP e VAR_PWD_SMTP
                if (smtp_user != null)
                    smtp_user = smtp_user.Trim();
                if (smtp_pwd != null)
                    smtp_pwd = smtp_pwd.Trim();

                creaMail(regRow["VAR_SERVER_SMTP"].ToString(), smtp_user, smtp_pwd, regRow["VAR_EMAIL_REGISTRO"].ToString(), mailMitt, numRegMitt, pathFiles, porta, regRow["CHA_SMTP_SSL"].ToString(), regRow["CHA_POP_SSL"].ToString(), regRow["CHA_SMTP_STA"].ToString());
                System.IO.File.Delete(pathFiles + "\\Eccezione.xml");
                DocsPaUtils.Functions.Functions.CancellaDirectory(pathFiles);
                DocsPaUtils.Functions.Functions.CancellaDirectory(basePathFiles);
            }
            catch (Exception e)
            {
                esito = false;
                logger.Error("Errore nella gestione dell'interoperabilità. (sendNotificaEccezione)", e);
                throw e;
            }
            return esito;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="mailMitt"></param>
        /// <param name="mailDest"></param>
        /// <param name="numRegMitt"></param>
        /// <param name="pathFiles"></param>
        private static void creaMail(string server, string smtp_user, string smtp_pwd, string mailMitt, string mailDest, string numRegMitt, string pathFiles, string port, string SmtpSsl, string PopSsl, string smtpSTA)
        {
            SvrPosta svr = new SvrPosta(server,
                smtp_user,
                smtp_pwd,
                port,
                Path.GetTempPath(),
               CMClientType.SMTP, SmtpSsl, PopSsl);

            try
            {
                string[] files = System.IO.Directory.GetFiles(pathFiles);
                List<CMAttachment> attachLst = new List<CMAttachment>();
                foreach (string file in files)
                {
                    CMAttachment att = new CMAttachment(Path.GetFileName(file), Interoperabilità.MimeMapper.GetMimeType (Path.GetExtension (file)), file);
                    attachLst.Add(att);
                }
                svr.connect();
                svr.sendMail(mailMitt,
                    mailDest,
                    "Ricevuta Eccezione",
                    "Ricevuta Eccezione del messaggio spedito con numero di protocollo " + numRegMitt,
                    attachLst.ToArray());
            }
            catch (Exception e)
            {
                logger.Error("Errore nella gestione dell'interoperabilità. (creaMail)", e);
                throw e;
            }
            finally
            {
                svr.disconnect();
            }
        }

        private static bool updateStatoInvioAnnulla(string systemID, string motivoAnnulla)
        {
            bool result = false;

            try
            {
                DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                result = obj.updStatoInvioEccezione(systemID, motivoAnnulla);
            }
            catch (Exception e)
            {
                logger.Error("Eccezione: " + e.Message);

                result = false;
            }

            return result;
        }


        /// <summary>
        /// CONTROLLO SULLA PRESENZA DELLA MAIL DI ORIGINE --isMailOriginePresente  //SABRI
        /// </summary>
        bool isMailOriginePresente(Interoperabilità.CMMsg messaggio, segnatura.Segnatura sexml)
        {
            bool retval = true;
            XmlDocument doc = new XmlDocument();
            string[] indTel = sexml.Intestazione.Origine.IndirizzoTelematico.Text;
            if (indTel == null || string.IsNullOrEmpty(indTel.ToString()))
                retval = false;
            /*
             XmlElement elIdentificatore = (XmlElement)doc.DocumentElement.SelectSingleNode("Intestazione/Identificatore");
             XmlElement elmailOrigine = ((XmlElement)doc.DocumentElement.SelectSingleNode("Intestazione/Origine"));
             string mailOrigine = elmailOrigine.SelectSingleNode("IndirizzoTelematico").InnerText.Trim();
             */
            return retval;
        }

    }

    public sealed class LocalDtdResolver : XmlResolver
    {
        public XmlResolver sytemResolver = null;
        public LocalDtdResolver()
        {
            if (sytemResolver == null)
            {
                sytemResolver = new XmlUrlResolver();

            }
        }

        public override System.Net.ICredentials Credentials
        {
            set { throw new NotSupportedException(); }
        }

        public override object GetEntity(Uri absoluteUri, string role, Type t)
        {
            return sytemResolver.GetEntity(absoluteUri, role, t);
        }

        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            Uri retval = null;
            String filenameDtd = AppDomain.CurrentDomain.BaseDirectory;
            filenameDtd += System.Configuration.ConfigurationManager.AppSettings.Get("DTD_SEGNATURA_PATH");

            if (relativeUri.ToLower().Contains("segnatura.dtd"))
            {
                retval = new Uri(filenameDtd);
            }
            else
            {
                retval = sytemResolver.ResolveUri(baseUri, relativeUri);
            }
            return retval;
        }
    }


    /// <summary>
    /// Caso C Controllo in validazione del file segnatura.xml contro il DTD presente nell’allegato B della circolare AIPA n.28/2001
    /// </summary>
    class validaSegnatura
    {
        public List<string> ValidationErrorList = new List<string>();
        public validaSegnatura(string directory, string file)
        {
            LocalDtdResolver resolver = new LocalDtdResolver();
            // Set the validation settings.
            XmlReaderSettings settings = new XmlReaderSettings();
            //settings.DtdProcessing = DtdProcessing.Parse;  //4.0
            settings.ProhibitDtd = false;
            settings.ValidationType = ValidationType.DTD;
            settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
            settings.XmlResolver = resolver;

            // Create the XmlReader object.
            string path = Path.Combine(directory, file);
            XmlReader reader = XmlReader.Create(path, settings);

            // Parse the file. 
            while (reader.Read()) ;
        }

        public validaSegnatura(string xmlString)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.XmlResolver = null;
            settings.ProhibitDtd = false;
            XmlReader xreader = XmlTextReader.Create(new MemoryStream(System.Text.ASCIIEncoding.ASCII.GetBytes(xmlString)), settings);
            XmlDocument doc = new XmlDocument();
            doc.Load(xreader);

            //Se è presente il namespase valido con Xsd, altrimenti con DTD
            if (string.IsNullOrEmpty(doc.DocumentElement.NamespaceURI))
            {
                LocalDtdResolver resolver = new LocalDtdResolver();
                // Set the validation settings.
                settings = new XmlReaderSettings();
                settings.ProhibitDtd = false;
                settings.ValidationType = ValidationType.DTD;
                settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
                settings.XmlResolver = resolver;

                // Create the XmlReader object.
                MemoryStream ms = new MemoryStream(System.Text.ASCIIEncoding.ASCII.GetBytes(xmlString));
                XmlReader reader = XmlReader.Create(ms, settings);

                // Parse the file. 
                while (reader.Read()) ;
            }
            else
            {
                StringReader sr = new StringReader(xmlString);
                XmlTextReader xtr = new XmlTextReader(sr);

                XmlSchemaCollection myschema = new XmlSchemaCollection();
                //Incapsulo XML reader dentro Validiting reader
                XmlValidatingReader vReader = new XmlValidatingReader(xtr);
                vReader.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
                
                String filenameXsd = AppDomain.CurrentDomain.BaseDirectory + @"xml\segnatura.xsd";

                myschema.Add(null, filenameXsd);
                vReader.ValidationType = ValidationType.Schema;
                vReader.Schemas.Add(myschema);

                //Leggo il file
                //Se ci fossero errori viene chiamato l'handler
                while (vReader.Read()) ;

                xtr.Close();
            }
        }

        // Display any validation errors.
        private void ValidationCallBack(object sender, ValidationEventArgs e)
        {
            //Console.WriteLine("Validation Error: {0}", e.Message);
            ValidationErrorList.Add(e.Message);
        }

    }
}
