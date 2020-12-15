using System;
using System.IO;
using System.Xml;
using System.Configuration;
using Chilkat;
using log4net;
using DocsPaUtils.Security;
using BusinessLogic.interoperabilita.Semplificata;
using System.Collections.Generic;

namespace BusinessLogic.Interoperabilità
{
    /// <summary>
    /// Summary description for InteroperabilitaInvioAnnullamento.
    /// </summary>
    public class InteroperabilitaInvioAnnullamento
    {
        private static ILog logger = LogManager.GetLogger(typeof(InteroperabilitaInvioAnnullamento));

        /// <summary>
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="reg"></param>
        public static bool sendNotificaAnnullamento(string idProfile, DocsPaVO.utente.Registro reg)
        {
            #region nuova gestione interop
            string err = "";
            bool esito = true;   //presume successo

            //todo da calcolare;
            try
            {
                // estrazione dati + controllo mittente est o int
                System.Data.DataSet dsMitt = null;
                System.Data.DataSet dsProto = null;
                DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                obj.getMittSegn(out dsMitt, idProfile);

                if ((InteroperabilitaUtils.InteropIntNoMail || InteroperabilitaSemplificataManager.IsDocumentReceivedWithIS(idProfile)) && dsMitt != null && dsMitt.Tables[0].Rows.Count > 0
                    && dsMitt.Tables["INFO_MITT"].Rows[0]["CHA_TIPO_IE"] != null
                    && !dsMitt.Tables["INFO_MITT"].Rows[0]["CHA_TIPO_IE"].Equals(System.DBNull.Value))
                {

                    obj.getDatiProtoSpedito(out dsProto, idProfile);
                    string sep = DocsPaDB.Utils.Personalization.getInstance(reg.idAmministrazione).getSepSegnatura();
                    string codAmm = DocsPaDB.Utils.Personalization.getInstance(reg.idAmministrazione).getCodiceAmministrazione();
                    string[] protoMitt = dsMitt.Tables["INFO_MITT"].Rows[0]["VAR_PROTO_IN"].ToString().Split(sep.ToCharArray(), 2);
                    string dataProtoMitt = dsMitt.Tables["INFO_MITT"].Rows[0]["DTA_PROTO_IN"].ToString();
                    string dataProto = dsProto.Tables["INFO_PROTO"].Rows[0]["DTA_PROTO"].ToString();
                    string numProto = dsProto.Tables["INFO_PROTO"].Rows[0]["NUM_PROTO"].ToString();
                    string dataAnnulla = dsProto.Tables["INFO_PROTO"].Rows[0]["DTA_ANNULLA"].ToString();
                    string motivoAnnulla = dsProto.Tables["INFO_PROTO"].Rows[0]["VAR_AUT_ANNULLA"].ToString();
                    if (protoMitt != null && dataProtoMitt != null && protoMitt.Length > 1)
                    {
                        DocsPaVO.Interoperabilita.NotificaAnnullamento not = new DocsPaVO.Interoperabilita.NotificaAnnullamento();
                        not.codAmm = codAmm;
                        not.codAmm_Mitt = codAmm; //so che viene dalla stessa amm, ma in futuro posso valutarlo nelle multiamm, dalla dpa_stato_invio ?.
                        not.codAOO = reg.codRegistro;
                        not.codAOO_Mitt = protoMitt[0];
                        not.dataRegistr_Mitt = dataProtoMitt.Substring(0, dataProtoMitt.IndexOf(" "));
                        not.dataRegistrazione = dataProto.Substring(0, dataProtoMitt.IndexOf(" ")); ;
                        not.numeroRegistr_Mitt = protoMitt[1];
                        not.numeroRegistrazione = numProto;
                        not.dataAnnullamento = dataAnnulla;
                        not.motivoAnnullamento = motivoAnnulla;

                        // Se il documento è stato ricevuto per interoperabilità semplificata, l'elaborazione 
                        // viene demandata al gestore dell'IS
                        if (InteroperabilitaSemplificataManager.IsDocumentReceivedWithIS(idProfile))
                            esito = BusinessLogic.interoperabilita.Semplificata.SimplifiedInteroperabilityRecordDroppedAndExceptionManager.SendDocumentDroppedOrExceptionProofToSender(not.motivoAnnullamento, idProfile, not.codAmm, true);
                        else
                            //ATTENZIONE SOSTITUIRE CON NUOVA PROCEDURA 
                            esito = BusinessLogic.Interoperabilità.InteroperabilitaNotificaAnnullamento.processaNotificaAnnullamento(not, out err);
                    }

                    dsMitt.Dispose();
                    dsProto.Dispose();

                }
                else
            #endregion
                {
                    esito = sendNotificaAnnulla(idProfile, reg);
                }
            }
            catch (Exception e)
            {
                esito = false;
                logger.Debug("Errore nella gestione dell'interoperabilità. (sendRicevutaRitorno)" + " ", e);
                throw e;
            }
            return esito;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="reg"></param>
        private static bool sendNotificaAnnulla(string idProfile, DocsPaVO.utente.Registro reg)
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
                basePathFiles = basePathFiles + "\\Invio_notifica_annullamento_files";
                string pathFiles = basePathFiles + "\\" + regRow["VAR_CODICE"].ToString();
                DocsPaUtils.Functions.Functions.CheckEsistenzaDirectory(pathFiles);
                // logger.Debug(""+regRow["CHA_STR_SEGNATURA"].ToString().ToCharArray().Length);

                //costruzione del file xml
                string numRegMitt = "";
                DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();

                //costruisciXml(idProfile, reg,ref numRegMitt,ref qco, regRow["CHA_STR_SEGNATURA"].ToString(),pathFiles);
                string separatore = DocsPaDB.Utils.Personalization.getInstance(reg.idAmministrazione).getSepSegnatura();
                costruisciXml(idProfile, reg, ref numRegMitt, ref qco, separatore, pathFiles);

                //invio mail
                string mailMitt = Interoperabilità.InteroperabilitaUtils.getMailAddress(qco);
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

                // copia il file di segnatura
                //System.IO.File.Copy(System.AppDomain.CurrentDomain.BaseDirectory + @"xml\segnatura.dtd",pathFiles +  "\\segnatura.dtd",true);
                creaMail(regRow["VAR_SERVER_SMTP"].ToString(), smtp_user, smtp_pwd, regRow["VAR_EMAIL_REGISTRO"].ToString(), mailMitt, numRegMitt, pathFiles, porta, regRow["CHA_SMTP_SSL"].ToString(), regRow["CHA_POP_SSL"].ToString(), regRow["CHA_SMTP_STA"].ToString());
                System.IO.File.Delete(pathFiles + "\\Annullamento.xml");
                DocsPaUtils.Functions.Functions.CancellaDirectory(pathFiles);
                DocsPaUtils.Functions.Functions.CancellaDirectory(basePathFiles);
            }
            catch (Exception e)
            {
                esito = false;
                logger.Error("Errore nella gestione dell'interoperabilità. (sendNotificaAnnullamento)", e);
                throw e;
            }
            return esito;
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

            try
            {
                logger.Debug("costruisciXml");
                logger.Debug("" + separatore);

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
                string codiceAOOMittString = protoIn.Split(separator)[0];
                string numeroRegMittString = (protoIn.Split(separator).Length > 1 ? protoIn.Split(separator)[1] : string.Empty);
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

                string dataRegMittString = (!string.IsNullOrEmpty(mittRow["DTA_PROTO_IN"].ToString()) ? System.Convert.ToDateTime(mittRow["DTA_PROTO_IN"]).ToString("yyyy-MM-dd") : string.Empty);

                obj.getIdent(out ds, idProfile);

                System.Data.DataRow protoRow = ds.Tables["INFO_PROTO"].Rows[0];
                string numeroRegString = protoRow["NUM_PROTO"].ToString();
                string dataRegString = "";
                if (protoRow["DTA_PROTO"] != null && !protoRow["DTA_PROTO"].ToString().Equals(""))
                {
                    dataRegString = System.Convert.ToDateTime(protoRow["DTA_PROTO"]).ToString("yyyy-MM-dd");
                }
                string dataAnnullamento = "";
                if (protoRow["DTA_ANNULLA"] != null && !protoRow["DTA_ANNULLA"].ToString().Equals(""))
                {
                    dataAnnullamento = System.Convert.ToDateTime(protoRow["DTA_ANNULLA"]).ToString("yyyy-MM-dd");
                }
                string motivoAnnullamento = protoRow["VAR_AUT_ANNULLA"].ToString();

                //costruzione del file xml
                logger.Debug("Costruzione file xml");
                XmlDocument xdoc = new XmlDocument();
                //impostazione
                xdoc.XmlResolver = null;
                XmlDeclaration dec = xdoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                xdoc.AppendChild(dec);
                //XmlDocumentType dtd = xdoc.CreateDocumentType("AnnullamentoProtocollazione", null, "Segnatura.dtd", null);
                //xdoc.AppendChild(dtd);
                //logger.Debug("dtd impostato");
                //creazione della root
                XmlElement root = xdoc.CreateElement("AnnullamentoProtocollazione");
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
                DocsPaVO.utente.Registro registro = new DocsPaVO.utente.Registro();
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

                //creazione motivo annullamento
                XmlElement motivoAnnulla = xdoc.CreateElement("Motivo");
                motivoAnnulla.InnerText = motivoAnnullamento;
                root.AppendChild(motivoAnnulla);

                //creazione provvedimento annullamento
                XmlElement provvedimento = xdoc.CreateElement("Provvedimento");
                provvedimento.InnerText = "";
                root.AppendChild(provvedimento);

                /* NON E' RICHIESTO
                               //creazione dell'identificatore mittente;
                               XmlElement messRicevuto=xdoc.CreateElement("MessaggioRicevuto");
                               XmlElement identificatoreMitt=xdoc.CreateElement("Identificatore");
				
                               XmlElement codiceAmmMitt=xdoc.CreateElement("CodiceAmministrazione");
                               codiceAmmMitt.InnerText=codiceAmmMittString;
                               identificatoreMitt.AppendChild(codiceAmmMitt);


                               XmlElement codiceAOOMitt=xdoc.CreateElement("CodiceAOO");
                               codiceAOOMitt.InnerText=codiceAOOMittString;
                               identificatoreMitt.AppendChild(codiceAOOMitt);

                               XmlElement numeroRegMitt=xdoc.CreateElement("NumeroRegistrazione");
                               numeroRegMitt.InnerText=numeroRegMittString;
                               identificatoreMitt.AppendChild(numeroRegMitt);

                               XmlElement dataRegMitt=xdoc.CreateElement("DataRegistrazione");
                               dataRegMitt.InnerText=dataRegMittString;
                               identificatoreMitt.AppendChild(dataRegMitt);

                               messRicevuto.AppendChild(identificatoreMitt);
                               root.AppendChild(messRicevuto);
               */
                //salvataggio file
                System.IO.FileStream fs = new System.IO.FileStream(filePath + "\\Annullamento.xml", System.IO.FileMode.Create);
                xdoc.Save(fs);
                fs.Close();
            }
            catch (Exception e)
            {
                logger.Error("Errore nella gestione dell'interoperabilità. (costruisciXml)", e);
                throw e;
            }
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
                    CMAttachment att = new CMAttachment(Path.GetFileName(file), Interoperabilità.MimeMapper.GetMimeType(Path.GetExtension(file)), file);
                    attachLst.Add(att);
                }
                svr.connect();
                svr.sendMail(mailMitt,
                    mailDest,
                    "Notifica Annullamento",
                    "Notifica Annullamento del messaggio spedito con numero di protocollo " + numRegMitt,
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

       
    }
}

