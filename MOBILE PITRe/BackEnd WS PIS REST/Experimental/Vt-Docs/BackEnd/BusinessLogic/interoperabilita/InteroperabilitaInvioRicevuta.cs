using System;
using System.IO;
using System.Xml;
using System.Configuration;
using System.Globalization;
using Chilkat;
using System.Data;
using DocsPaUtils.Security;
using log4net;
using System.Collections.Generic;

namespace BusinessLogic.Interoperabilità
{
    /// <summary>
    /// </summary>
    public class InteroperabilitaInvioRicevuta
    {
        private static ILog logger = LogManager.GetLogger(typeof(InteroperabilitaInvioRicevuta));
        /// <summary>
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="reg"></param>
        public static void sendRicevutaRitorno(DocsPaVO.documento.SchedaDocumento schedaDoc, DocsPaVO.utente.Registro reg, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoUtente, out string messaggioErrore)
        {
            messaggioErrore = string.Empty;
            #region nuova gestione interop
            string err = "";


            //todo da calcolare;
            try
            {
                // estrazione dati + controllo mittente est o int
                System.Data.DataSet dsMitt = null;
                System.Data.DataSet dsProto = null;
                DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                obj.getMittSegn(out dsMitt, schedaDoc.systemId);






                if (InteroperabilitaUtils.InteropIntNoMail && schedaDoc.interop != null && schedaDoc.interop == "I" && dsMitt != null && dsMitt.Tables[0].Rows.Count > 0
                    && dsMitt.Tables["INFO_MITT"].Rows[0]["CHA_TIPO_IE"] != null
                    && !dsMitt.Tables["INFO_MITT"].Rows[0]["CHA_TIPO_IE"].Equals(System.DBNull.Value))
                {

                    obj.getDatiProtoSpedito(out dsProto, schedaDoc.systemId);
                    string sep = DocsPaDB.Utils.Personalization.getInstance(reg.idAmministrazione).getSepSegnatura();
                    string codAmm = DocsPaDB.Utils.Personalization.getInstance(reg.idAmministrazione).getCodiceAmministrazione();
                    //string[] protoMitt=dsMitt.Tables["INFO_MITT"].Rows[0]["VAR_PROTO_IN"].ToString().Split(sep.ToCharArray(),2);
                    int index = dsMitt.Tables["INFO_MITT"].Rows[0]["VAR_PROTO_IN"].ToString().LastIndexOf(sep);
                    string[] protoMitt = new String[2];
                    protoMitt[0] = dsMitt.Tables["INFO_MITT"].Rows[0]["VAR_PROTO_IN"].ToString().Substring(0, index);
                    protoMitt[1] = dsMitt.Tables["INFO_MITT"].Rows[0]["VAR_PROTO_IN"].ToString().Substring(index + 1); ;
                    string dataProtoMitt = dsMitt.Tables["INFO_MITT"].Rows[0]["DTA_PROTO_IN"].ToString();
                    string dataProto = dsProto.Tables["INFO_PROTO"].Rows[0]["DTA_PROTO"].ToString();
                    string numProto = dsProto.Tables["INFO_PROTO"].Rows[0]["NUM_PROTO"].ToString();
                    if (protoMitt != null && dataProtoMitt != null && protoMitt[0] != null && protoMitt[1] != null)
                    {
                        DocsPaVO.Interoperabilita.RicevutaRitorno ric = new DocsPaVO.Interoperabilita.RicevutaRitorno();
                        ric.codAmm = codAmm;
                        ric.codAmm_Mitt = codAmm; //so che viene dalla stessa amm, ma in futuro posso valutarlo nelle multiamm, dalla dpa_stato_invio ?.
                        ric.codAOO = reg.codRegistro;
                        ric.codAOO_Mitt = protoMitt[0];
                        ric.dataRegistr_Mitt = dataProtoMitt.Substring(0, dataProtoMitt.IndexOf(" "));
                        ric.dataRegistrazione = dataProto.Substring(0, dataProtoMitt.IndexOf(" ")); ;
                        ric.numeroRegistr_Mitt = protoMitt[1];
                        ric.numeroRegistrazione = numProto;
                        BusinessLogic.Interoperabilità.InteroperabilitaControlloRicevute.processaRicevutaConferma(ric, out err);

                        if (reg.autoInterop != null && reg.autoInterop != "0")
                        {
                            DocsPaVO.documento.SchedaDocumento schedaDocUscita = new DocsPaVO.documento.SchedaDocumento();

                            //ricavo il protocollo in uscita a partire dal protocollo
                            //predisposto in arrivo
                            schedaDocUscita = getDocumentoInUscitaByDocPredisposto(infoUtente, schedaDoc);

                            //
                            bool verificaRagioni;
                            string message = "";
                            BusinessLogic.trasmissioni.TrasmProtoIntManager.TrasmissioneProtocolloAutomatico(schedaDoc, schedaDoc.registro.systemId, schedaDocUscita, ruolo, infoUtente, infoUtente.urlWA, false, out verificaRagioni, out message);
                        }

                    }

                    dsMitt.Dispose();
                    dsProto.Dispose();

                }
                else
            #endregion
                {

                    sendRicRitorno(schedaDoc.systemId, reg, schedaDoc,out messaggioErrore);


                }


            }
            catch (Exception e)
            {
                if(string.IsNullOrEmpty(messaggioErrore))
                    messaggioErrore = "errore nell' invio della ricevuta";
                logger.Error("Errore nella gestione dell'interoperabilità. (sendRicevutaRitorno)" + " ", e);
                throw e;
            }
        }

        private static DocsPaVO.documento.SchedaDocumento getDocumentoInUscitaByDocPredisposto(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDoc)
        {
            logger.Debug("getDocumentoInUscitaByDocPredisposto");
            DataSet ds;
            DocsPaVO.documento.SchedaDocumento sd = new DocsPaVO.documento.SchedaDocumento();
            try
            {
                DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                string sep = DocsPaDB.Utils.Personalization.getInstance(schedaDoc.registro.idAmministrazione).getSepSegnatura();
                string[] protoMitt = ((DocsPaVO.documento.ProtocolloEntrata)(schedaDoc.protocollo)).descrizioneProtocolloMittente.Split(sep.ToCharArray(), 2);
                CultureInfo ci = new CultureInfo("it-IT");
                string[] formati = { "dd/MM/yyyy", "yyyy-MM-dd", "DD/MM/YYYY hh:mm:ss", "DD/MM/YYYY hh.mm.ss", "DD/MM/YYYY HH.mm.ss", "DD/MM/YYYY HH:mm:ss" };

                DateTime dataRegistrazione = DateTime.ParseExact(((DocsPaVO.documento.ProtocolloEntrata)(schedaDoc.protocollo)).dataProtocolloMittente, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);


                if (protoMitt != null && protoMitt.Length > 1)
                {
                    string regProto = protoMitt[0];
                    string numProto = protoMitt[1];
                    int anno = dataRegistrazione.Year;

                    DocsPaDB.Query_DocsPAWS.Interoperabilita interop = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                    DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

                    interop.getIdProtocolloUscitaOriginario(out ds, numProto, regProto, anno);

                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow row = ds.Tables["PROTOUSCITA"].Rows[0];
                        string docNumber = row["DOCNUMBER"].ToString();

                        if (docNumber != null && docNumber != "")
                        {
                            sd = doc.GetSchedaDocumentoByID(infoUtente, docNumber);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());

                logger.Error("Errore nella gestione dell'interoperabilità. (getDocumentoInUscitaByDocPredisposto", e);
                throw e;
            }
            return sd;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="reg"></param>
        private static void sendRicRitorno(string idProfile, DocsPaVO.utente.Registro reg, DocsPaVO.documento.SchedaDocumento schedaDoc, out string messaggioErrore)
        {

            System.Data.DataSet ds;
            messaggioErrore = string.Empty;
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
                basePathFiles = basePathFiles + "\\Invio_ricevuta_files\\" + Guid.NewGuid().ToString();
                string pathFiles = basePathFiles + "\\" + regRow["VAR_CODICE"].ToString();
                DocsPaUtils.Functions.Functions.CheckEsistenzaDirectory(pathFiles);
                // logger.Debug(""+regRow["CHA_STR_SEGNATURA"].ToString().ToCharArray().Length);

                //costruzione del file xml
                string numRegMitt = "";
                DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();

                //costruisciXml(idProfile, reg,ref numRegMitt,ref qco, regRow["CHA_STR_SEGNATURA"].ToString(),pathFiles);
                string separatore = DocsPaDB.Utils.Personalization.getInstance(reg.idAmministrazione).getSepSegnatura();
                string mailMitt = string.Empty;
                if (schedaDoc.typeId.ToUpper().Trim() == "INTEROPERABILITA")
                {
                    costruisciXml(idProfile, reg, ref numRegMitt, ref qco, separatore, pathFiles);
                }
                else
                {
                    //costruzione dell'oggetto per la richiesta mittente
                    System.Data.DataSet ds1;
                    DocsPaDB.Query_DocsPAWS.Interoperabilita obj1 = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                    obj1.getMittSegn(out ds1, idProfile);

                    System.Data.DataRow mittRow = ds1.Tables["INFO_MITT"].Rows[0];

                    qco.codiceRubrica = mittRow["VAR_COD_RUBRICA"].ToString();
                    qco.idAmministrazione = mittRow["ID_AMM"].ToString();
                    qco.systemId = mittRow["SYSTEM_ID"].ToString();
                    if (mittRow["CHA_TIPO_IE"].ToString().Equals("I"))
                    {
                        qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
                    }
                    else
                    {
                        qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.ESTERNO;
                    }
                    qco.getChildren = false;
                }

                //invio mail
                mailMitt = getMailAddress(qco, schedaDoc.interop);
                if (string.IsNullOrEmpty(mailMitt))
                {
                    messaggioErrore = "Il Mittente non ha associata una mail.";
                    logger.Error("Il Mittente non ha associata una mail.");
                    throw new ApplicationException("Il Mittente non ha associata una mail.");
                }
                logger.Debug("Invio mail all'indirizzo " + mailMitt);
                string porta = null;
                if (regRow["NUM_PORTA_SMTP"] != null)
                {
                    porta = regRow["NUM_PORTA_SMTP"].ToString();
                }
                string smtp_user = (regRow["VAR_USER_SMTP"] != null && regRow["VAR_USER_SMTP"] != System.DBNull.Value) ? regRow["VAR_USER_SMTP"].ToString() : null;
                string smtp_pwd = (regRow["VAR_PWD_SMTP"] != null && regRow["VAR_PWD_SMTP"] != System.DBNull.Value) ? regRow["VAR_PWD_SMTP"].ToString() : null;

                //aggiunta la trim() per gestire la presenza di spazi bianchi nei campi VAR_USER_SMTP e VAR_PWD_SMTP
                if (smtp_user != null)
                    smtp_user = smtp_user.Trim();
                if (smtp_pwd != null)
                    smtp_pwd = smtp_pwd.Trim();

                // copia il file di segnatura
                //System.IO.File.Copy(System.AppDomain.CurrentDomain.BaseDirectory + @"xml\segnatura.dtd",pathFiles +  "\\segnatura.dtd",true);              

                string mailBody = getMailBody(idProfile, reg, separatore, schedaDoc);
                string subject_appo = "Conferma ricezione ''" + schedaDoc.oggetto.descrizione + "'' del " + System.Convert.ToDateTime(schedaDoc.dataCreazione).ToString("dd-MM-yyyy");
                string subject = subject_appo; //= subject_appo.Substring(0, 256);
                if (subject_appo.Length > 256)
                    subject = subject_appo.Substring(0, 256);
                
                //aggiunta del docnumber all'oggetto delal mail per la gestione delle ricevute pec
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["GESTIONE_RICEVUTE_PEC"]) &&
                    bool.Parse(ConfigurationManager.AppSettings["GESTIONE_RICEVUTE_PEC"]))
                    subject += "#"+schedaDoc.docNumber.ToString()+"#";
                //Crypter.Decode(smtp_pwd, smtp_user)
                creaMail(regRow["VAR_SERVER_SMTP"].ToString(), smtp_user, Crypter.Decode(smtp_pwd, smtp_user), regRow["VAR_EMAIL_REGISTRO"].ToString(), mailMitt, numRegMitt, pathFiles, porta, regRow["CHA_SMTP_SSL"].ToString(), regRow["CHA_POP_SSL"].ToString(), regRow["CHA_SMTP_STA"].ToString(), mailBody, subject);

                //old: System.IO.File.Delete(pathFiles+"\\confermaRicezione.xml");
                System.IO.File.Delete(pathFiles + "\\conferma.xml");
                //System.IO.File.Delete(pathFiles+"\\segnatura.dtd");
                DocsPaUtils.Functions.Functions.CancellaDirectory(pathFiles);
                DocsPaUtils.Functions.Functions.CancellaDirectory(basePathFiles);
            }
            catch (Exception e)
            {
                if(string.IsNullOrEmpty(messaggioErrore))
                    messaggioErrore = "Errore durante l'invio della ricevuta";
                logger.Error("Errore nella gestione dell'interoperabilità. (sendRicevutaRitorno)", e);
                throw e;
            }
        }
        /// <summary></summary>
        /// <param name="idProfile"></param>
        /// <param name="reg"></param>
        /// <param name="numRegMitt"></param>
        /// <param name="qco"></param>
        /// <param name="separatore"></param>
        /// <param name="filePath"></param>
        private static void costruisciXml(string idProfile, DocsPaVO.utente.Registro reg, ref string numRegMitt, ref DocsPaVO.addressbook.QueryCorrispondente qco, string separatore, string filePath)
        {
            System.Data.DataSet ds;
            DocsPaVO.utente.Registro registro = null;
            try
            {
                logger.Debug("costruisciXml");
                logger.Debug("" + separatore);

                #region Codice Commentato
                //query per recuperare le info sul mittente della segnatura
                /*
                string queryMittString="SELECT A.VAR_PROTO_IN, A.DTA_PROTO_IN, B.VAR_CODICE_AMM, B.CHA_TIPO_IE, B.VAR_COD_RUBRICA, B.ID_AMM FROM PROFILE A, DPA_CORR_GLOBALI B, DPA_DOC_ARRIVO_PAR C WHERE A.SYSTEM_ID="+idProfile;
                queryMittString=queryMittString+" AND B.SYSTEM_ID=C.ID_MITT_DEST AND C.ID_PROFILE=A.SYSTEM_ID";
                logger.Debug(queryMittString);
                db.fillTable(queryMittString,ds,"INFO_MITT");
                */
                #endregion

                DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                obj.getMittSegn(out ds, idProfile);

                System.Data.DataRow mittRow = ds.Tables["INFO_MITT"].Rows[0];

                char[] separator = { separatore.ToCharArray()[0] };

                //si trova il codice amministrazione del mittente
                string codiceAmmMittString = "";
                if (mittRow["CHA_TIPO_IE"].ToString().Equals("I"))
                {
                    //mittente interno
                    codiceAmmMittString = reg.codAmministrazione;
                }
                else
                {
                    //mittente esterno
                    codiceAmmMittString = mittRow["VAR_CODICE_AMM"].ToString();
                }
                string protoIn = mittRow["VAR_PROTO_IN"].ToString();
                logger.Debug(protoIn + " " + protoIn.Split(separator).Length);
                string codiceAOOMittString = "";
                string numeroRegMittString = "";
                if (!string.IsNullOrEmpty(protoIn))
                {
                    codiceAOOMittString = protoIn.Split(separator)[0];
                    if (protoIn.Contains(separatore))
                    {
                        numeroRegMittString = protoIn.Split(separator)[1];
                    }
                }
                numRegMitt = numeroRegMittString;

                //costruzione dell'oggetto per la richiesta mittente
                qco.codiceRubrica = mittRow["VAR_COD_RUBRICA"].ToString();
                qco.idAmministrazione = mittRow["ID_AMM"].ToString();
                if (mittRow["CHA_TIPO_IE"].ToString().Equals("I"))
                {
                    qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
                }
                else
                {
                    qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.ESTERNO;
                }
                qco.getChildren = false;

                string dataRegMittString = "";
                if (mittRow["DTA_PROTO_IN"] != null && !mittRow["DTA_PROTO_IN"].ToString().Equals(""))
                {
                    dataRegMittString = System.Convert.ToDateTime(mittRow["DTA_PROTO_IN"]).ToString("yyyy-MM-dd");
                }

                #region Codice Commentato
                //query per recuperare le info per l'identificatore
                /*
                string queryProtoString="SELECT DTA_PROTO, NUM_PROTO FROM PROFILE WHERE SYSTEM_ID="+idProfile;
                logger.Debug(queryProtoString);
                db.fillTable(queryProtoString,ds,"INFO_PROTO");
                */
                #endregion

                obj.getIdent(out ds, idProfile);

                System.Data.DataRow protoRow = ds.Tables["INFO_PROTO"].Rows[0];
                string numeroRegString = protoRow["NUM_PROTO"].ToString();
                string dataRegString = "";
                if (protoRow["DTA_PROTO"] != null && !protoRow["DTA_PROTO"].ToString().Equals(""))
                {
                    dataRegString = System.Convert.ToDateTime(protoRow["DTA_PROTO"]).ToString("yyyy-MM-dd");
                }
                //costruzione del file xml
                logger.Debug("Costruzione file xml");
                XmlDocument xdoc = new XmlDocument();
                //impostazione
                xdoc.XmlResolver = null;
                XmlDeclaration dec = xdoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                xdoc.AppendChild(dec);

                //NON VALIDIAMO PIù CON LA DTD MA CON L'XSD
                //XmlDocumentType dtd = xdoc.CreateDocumentType("ConfermaRicezione", null, "Segnatura.dtd", null);
                //xdoc.AppendChild(dtd);
                //logger.Debug("dtd impostato");

                //creazione della root
                XmlElement root = xdoc.CreateElement("ConfermaRicezione");
                root.SetAttribute("xmlns", "http://www.digitPa.gov.it/protocollo/");  
                xdoc.AppendChild(root);

                //creazione dell'identificatore
                XmlElement identificatore = xdoc.CreateElement("Identificatore");
                root.AppendChild(identificatore);

                XmlElement codiceAmm = xdoc.CreateElement("CodiceAmministrazione");
                codiceAmm.InnerText = reg.codAmministrazione;
                identificatore.AppendChild(codiceAmm);

                XmlElement codiceAOO = xdoc.CreateElement("CodiceAOO");

                //se è un RF, io devo comunque inviare la ricevuta dall'AOO.
                if (reg.chaRF != null && reg.chaRF == "1")
                {
                    if (!string.IsNullOrEmpty(reg.idAOOCollegata))
                    {
                        registro = BusinessLogic.Utenti.RegistriManager.getRegistro(reg.idAOOCollegata);

                    }
                    else throw new Exception("Errore in costruiscixml conferma, l'RF " + reg.descrizione + " non ha una AOO collegata.");
                }
                if (registro != null)
                    reg = registro;


                codiceAOO.InnerText = reg.codRegistro;
                identificatore.AppendChild(codiceAOO);

                XmlElement numeroReg = xdoc.CreateElement("NumeroRegistrazione");
                numeroReg.InnerText = numeroRegString;
                identificatore.AppendChild(numeroReg);

                XmlElement dataReg = xdoc.CreateElement("DataRegistrazione");
                dataReg.InnerText = dataRegString;
                identificatore.AppendChild(dataReg);


                //creazione dell'identificatore mittente;
                XmlElement messRicevuto = xdoc.CreateElement("MessaggioRicevuto");
                XmlElement identificatoreMitt = xdoc.CreateElement("Identificatore");

                XmlElement codiceAmmMitt = xdoc.CreateElement("CodiceAmministrazione");
                codiceAmmMitt.InnerText = codiceAmmMittString;
                identificatoreMitt.AppendChild(codiceAmmMitt);

                XmlElement codiceAOOMitt = xdoc.CreateElement("CodiceAOO");
                codiceAOOMitt.InnerText = codiceAOOMittString;
                identificatoreMitt.AppendChild(codiceAOOMitt);

                XmlElement numeroRegMitt = xdoc.CreateElement("NumeroRegistrazione");
                numeroRegMitt.InnerText = numeroRegMittString;
                identificatoreMitt.AppendChild(numeroRegMitt);

                XmlElement dataRegMitt = xdoc.CreateElement("DataRegistrazione");
                dataRegMitt.InnerText = dataRegMittString;
                identificatoreMitt.AppendChild(dataRegMitt);

                messRicevuto.AppendChild(identificatoreMitt);
                root.AppendChild(messRicevuto);

                //salvataggio file
                System.IO.FileStream fs = new System.IO.FileStream(filePath + "\\conferma.xml", System.IO.FileMode.Create);
                xdoc.Save(fs);
                fs.Close();
            }
            catch (Exception e)
            {
                logger.Error("Errore nella gestione dell'interoperabilità. (costruisciXml)", e);
                throw e;
            }
        }

        #region Metodo Commentato
        /*private static void creaMail(string username,string password,string server,string port,string mailMitt,string mailDest,string numRegMitt,string pathFiles){
			WinToolZone.CSLMail.SMTP smtp=new WinToolZone.CSLMail.SMTP();
			try
			{
				smtp.Username=username; 
				smtp.Password=password;
				smtp.Authentication=WinToolZone.CSLMail.SMTP.SMTPAuthenticationType.LOGIN;
				smtp.From=mailMitt;
				smtp.To=mailDest;
				smtp.MailType=WinToolZone.CSLMail.SMTP.MailEncodingType.HTML;
				smtp.Subject="Conferma ricezione";
				//body della mail
				string bodyMail="Conferma di ricezione del messaggio con numero di protocollo "+numRegMitt;
				smtp.Message=bodyMail;
				smtp.SMTPServer=server;
				if(port!=null)
				{
					smtp.SMTPPort=(short) Int32.Parse(port);
				}
				string[] files=System.IO.Directory.GetFiles(pathFiles);
				for(int i=0;i<files.Length;i++)
				{
					if(smtp.AddAttachment(files[i]))
					{
						logger.Debug("Attachment "+files[i]+" inserito");
					}
					else
					{
						logger.Debug("Attachment "+files[i]+"non inserito");
						throw new Exception();
					};
				}
				if(!smtp.SendMail())
				{ 
					logger.Debug("Invio mail non eseguito"+smtp.ErrorDescription);
					throw new Exception();
				}
			}
			catch(Exception e)
			{
               throw e;
			}
		}*/
        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="mailMitt"></param>
        /// <param name="mailDest"></param>
        /// <param name="numRegMitt"></param>
        /// <param name="pathFiles"></param>
        private static void creaMail(string server, string smtp_user, string smtp_pwd, string mailMitt, string mailDest, string numRegMitt, string pathFiles, string port, string SmtpSsl, string PopSsl, string smtpSTA, string body, string subject)
        {
            SvrPosta svr = new SvrPosta(server,
                smtp_user,
                smtp_pwd,
                port,
                Path.GetTempPath(),
                CMClientType.SMTP, SmtpSsl, PopSsl, smtpSTA);

           

            try
            {
                string[] files = System.IO.Directory.GetFiles(pathFiles);
                List<CMAttachment> attachLst = new List<CMAttachment>();
                foreach (string file in files)
                {
                    CMAttachment att = new CMAttachment(Path.GetFileName(file), Interoperabilità.MimeMapper.GetMimeType(Path.GetExtension(file)), file);
                    attachLst.Add(att);
                }
                svr.connect();
                svr.sendMail(mailMitt,
                    mailDest,
                    subject,
                    body,//"Conferma di ricezione del messaggio con numero di protocollo " + numRegMitt,
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

        /// <summary></summary>
        /// <param name="qco"></param>
        /// <returns></returns>
        private static string getMailAddress(DocsPaVO.addressbook.QueryCorrispondente qco, string interop)
        {
            logger.Debug("getMailAddress");
            string mailMitt = "";
            System.Collections.ArrayList mittArr = new System.Collections.ArrayList();
            if (qco.tipoUtente == DocsPaVO.addressbook.TipoUtente.ESTERNO)
            {
                //Chiamato altro metodo perchè quello in uso mette in join con una tabella non popolata DPA_RUOE_UTENTI, e quindi in alcuni casi non funziona
                //mittArr = BusinessLogic.Utenti.addressBookManager.listaCorrispondentiEstMethod(qco);
                DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                mittArr = utenti.ListaCorrispondentiEst2(qco);
               
            }
            else
            {
                mittArr = BusinessLogic.Utenti.addressBookManager.listaCorrispondentiIntMethod(qco);
            }
            if (mittArr.Count == 0)
            {
                mittArr = BusinessLogic.Utenti.addressBookManager.listaCorrispondentiOccMethod(qco);
                if (mittArr.Count > 0)
                    return mittArr[0].ToString();
            }
            if (mittArr.Count > 0)
            {
                foreach (DocsPaVO.utente.Corrispondente c in mittArr)
                {
                    if (c.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
                    {
                        logger.Debug("UO");
                        if (string.IsNullOrEmpty(interop) || interop.Equals("I"))
                            mailMitt = ((DocsPaVO.utente.UnitaOrganizzativa)mittArr[0]).email;
                        else if (interop.Equals("P") && (!string.IsNullOrEmpty(c.email)))
                        {
                            mailMitt = c.email;
                            break;
                        }
                        else if (!string.IsNullOrEmpty(c.codiceAmm) && (!string.IsNullOrEmpty(c.codiceAOO)) && (!string.IsNullOrEmpty(c.email)))
                        {
                            mailMitt = c.email;
                            break;
                        }
                    }
                    if (c.GetType() == typeof(DocsPaVO.utente.Ruolo))
                    {
                        logger.Debug("Ruolo");
                        if (string.IsNullOrEmpty(interop) || interop.Equals("I"))
                            mailMitt = ((DocsPaVO.utente.Ruolo)mittArr[0]).uo.email;
                        else if (interop.Equals("P") && (!string.IsNullOrEmpty(c.email)))
                        {
                            mailMitt = c.email;
                            break;
                        }
                        else if(!string.IsNullOrEmpty(c.codiceAmm) && (!string.IsNullOrEmpty(c.codiceAOO)) && (!string.IsNullOrEmpty(c.email)))
                        {
                            mailMitt = c.email;
                            break;
                         }
                    }
                    if (c.GetType() == typeof(DocsPaVO.utente.Utente))
                    {
                        logger.Debug("Utente");
                        if (string.IsNullOrEmpty(interop) || interop.Equals("I"))
                            mailMitt = ((DocsPaVO.utente.Ruolo)((DocsPaVO.utente.Utente)mittArr[0]).ruoli[0]).uo.email;
                        else if (interop.Equals("P") && (!string.IsNullOrEmpty(c.email)))
                        {
                            mailMitt = c.email;
                            break;
                        }
                        else if(!string.IsNullOrEmpty(c.codiceAmm) && (!string.IsNullOrEmpty(c.codiceAOO)) && (!string.IsNullOrEmpty(c.email)))
                        {
                            mailMitt = c.email;
                            break;
                        }
                    }
                }
            }
            logger.Debug(mailMitt);
            return mailMitt;
        }

     


        private static string getMailBody(string idProfile, DocsPaVO.utente.Registro reg, string separatore, DocsPaVO.documento.SchedaDocumento schedaDoc)
        {
            System.Data.DataSet ds;
            string mailBody = string.Empty;
            DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
            try
            {
                obj.getMittSegn(out ds, idProfile);

                System.Data.DataRow mittRow = ds.Tables["INFO_MITT"].Rows[0];

                char[] separator = { separatore.ToCharArray()[0] };

                //si trova il codice amministrazione del mittente
                string codiceAmmMittString = "";
                if (mittRow["CHA_TIPO_IE"].ToString().Equals("I"))
                {
                    //mittente interno
                    codiceAmmMittString = reg.codAmministrazione;
                }
                else
                {
                    //mittente esterno
                    codiceAmmMittString = mittRow["VAR_CODICE_AMM"].ToString();
                }
                string codiceAOOMittString = "";
                string numeroRegMittString = "";
                string protoIn = mittRow["VAR_PROTO_IN"].ToString();
                if (!string.IsNullOrEmpty(protoIn))
                {
                    codiceAOOMittString = protoIn.Split(separator)[0];
                    if (protoIn.Contains(separatore))
                        numeroRegMittString = protoIn.Split(separator)[1];
                }

                mailBody += "Conferma di ricezione ''" + schedaDoc.oggetto.descrizione + "'' del " + System.Convert.ToDateTime(schedaDoc.dataCreazione).ToString("dd-MM-yyyy") + "<br>";
                string dataRegMittString = "";

                if (mittRow["DTA_PROTO_IN"] != null && !mittRow["DTA_PROTO_IN"].ToString().Equals(""))
                {
                    dataRegMittString = System.Convert.ToDateTime(mittRow["DTA_PROTO_IN"]).ToString("yyyy-MM-dd");
                }


                obj.getIdent(out ds, idProfile);

                System.Data.DataRow protoRow = ds.Tables["INFO_PROTO"].Rows[0];
                string numeroRegString = protoRow["NUM_PROTO"].ToString();
                string dataRegString = "";
                if (protoRow["DTA_PROTO"] != null && !protoRow["DTA_PROTO"].ToString().Equals(""))
                {
                    dataRegString = System.Convert.ToDateTime(protoRow["DTA_PROTO"]).ToString("yyyy-MM-dd");
                }



                DocsPaVO.utente.Corrispondente mittente = ((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).mittente;

                mailBody += "<br>Segnatura: " + ((DocsPaVO.documento.Protocollo)schedaDoc.protocollo).segnatura + "<br>Codice Amministrazione: " + reg.codAmministrazione + "<br>Codice AOO: " +
                    reg.codRegistro + "<br>Numero Protocollo: " + numeroRegString + "<br>Data Protocollo: " + dataRegString +
                    //"<br><br>Protocollo Mittente" + "<br>Codice Amministrazione: " + codiceAmmMittString + "<br>Codice AOO: " + codiceAOOMittString +
                    // "<br>Numero Protocollo: " + numeroRegMittString + "<br>Data Protocollo: " + dataRegMittString +
                    "<br>Mittente: " + mittente.descrizione;

            }
            catch (Exception e)
            {
                logger.Error("Errore nella gestione dell'interoperabilità. (getMailBody)", e);
                mailBody = "";
            }
            return mailBody;
        }

    }
}


