using System;
using System.Data;
using System.Globalization;
using System.Configuration;
using log4net;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Linq;

namespace BusinessLogic.Interoperabilità
{
	/// <summary>
	/// Summary description for InteroperabilitaUtils.
	/// </summary>
	public class InteroperabilitaUtils
    {
        private static ILog logger = LogManager.GetLogger(typeof(InteroperabilitaUtils));
		/// <summary></summary>
		/// <param name="messageId"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		public static bool CheckId(string messageId, string userId, string id_registro)
		{
			if(messageId==null||messageId.Equals("")) 
			{
				return false;			
			}

			DataSet dataSet;
			
			try
			{
				#region Codice Commentato
				//db.openConnection();
				/*
				string checkString="SELECT * FROM DPA_MAIL_ELABORATE WHERE VAR_MESSAGE='"+messageId+"'";
				db.fillTable(checkString,dataSet,"MAIL");
				*/
				#endregion 

				DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();				
				obj.getMailElab(out dataSet,messageId,id_registro );

				if(dataSet.Tables["MAIL"].Rows.Count>0)
				{
					return false;
				}

				return true;
			}
			catch(Exception e)
			{
                logger.Error(e.ToString());

				return false;
			}
		}	

		/// <summary>
		/// </summary>
		/// <param name="idMessage"></param>
		/// <param name="ragione"></param>
		/// <returns></returns>
        //modifica
        public static bool MailElaborata(string idMessage, string ragione, string id_registro, string docnumber, string email)
		{
			try
			{ 
				logger.Debug("mailElaborata");
				DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();				
			    obj.insMailElab(idMessage,ragione,id_registro, docnumber, email);
                 logger.Debug("elaborazione della mail con id: " + idMessage+ " avvenuta con successo");
            	return true;
			}
			catch(Exception e)
			{
                logger.Error("elaborazione della mail con id: " + idMessage + " ha gerato il seguente errore" + e.Message);

				return false;
			}
		}
        //fine modifica

        public static bool MailElaborata(string idMessage, string ragione)
        {
            try
            {
                logger.Debug("mailElaborata");
  
                DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                obj.insMailElab(idMessage, ragione);
                logger.Debug("elaborazione della mail con id: " + idMessage+ " avvenuta con successo");
                return true;
            }
            catch (Exception e)
            {
                logger.Error("elaborazione della mail con id: " + idMessage + " ha gerato il seguente errore" + e.Message);

                return false;
            }
        }


		// Nuove proprietà per interrogazione chiavi di configurazione<

        public static bool Cfg_EliminazioneMailElaborate(DocsPaVO.utente.InfoUtente infoUtente)
        {
            string valoreChiaveDB = string.Empty;
            DocsPaVO.amministrazione.ConfigRepository chiaviAmm = DocsPaUtils.Configuration.InitConfigurationKeys.getInstance(infoUtente.idAmministrazione);
            if (chiaviAmm != null && chiaviAmm.ContainsKey("BE_ELIMINA_MAIL_ELABORATE"))
                valoreChiaveDB = chiaviAmm["BE_ELIMINA_MAIL_ELABORATE"].ToString();

            return (valoreChiaveDB != null && valoreChiaveDB == "1") ? true : false;

        }
		/// <summary>
        /// modalità interoperabilità ricerca mittente in segnatura solo UO
        /// </summary>
        public static bool Cfg_InteroperSegnaturaSoloUO
        {
            get
            {
                string eme = ConfigurationManager.AppSettings["INTEROP_SEGNATURA_SOLO_UO"];
                return (eme != null && eme == "1") ? true : false;
            }
        }
		/// <summary>
		/// True, se è settata l'interoperabilità per gli Interni senza mail
		/// </summary>
		public static bool InteropIntNoMail 
		{
			get 
			{
				string eme = ConfigurationManager.AppSettings["INTEROP_INT_NO_MAIL"];
				return (eme != null && eme == "1") ? true : false;
			}
		}

        /// <summary>
        /// True, se è settata l'interoperabilità semplificata
        /// </summary>
        public static bool InteropSemp
        {
            get
            {
                string eme = ConfigurationManager.AppSettings["INTEROP_SEMP"];
                return (eme != null && eme == "1") ? true : false;
            }
        }

		public static int Cfg_NumeroTentativiLogServerPosta 
		{
			get 
			{
				string eme = ConfigurationManager.AppSettings["NUM_TENTATIVI_CONN_SVR_POSTA"];
				return (eme != null && eme != "") ? Int32.Parse(eme) : 1;
			}
		}
		public static bool Cfg_BacthInteroperabilita 
		{
			get 
			{
				string eme = ConfigurationManager.AppSettings["BATCH_INTEROP"];
				return (eme != null && eme == "1") ? true : false;
			}
		}
		public static bool Cfg_ElaborazionePostaOrdinaria
		{
			get 
			{
				string eme = ConfigurationManager.AppSettings["ELABORA_MAIL_ORDINARIE"];
				return (eme != null && eme == "1") ? true : false;
			}
		}

		public static bool Cfg_SmtpOverSsl
		{
			get 
			{
				string eme = ConfigurationManager.AppSettings["SMTP_SSL"];
				return (eme != null && eme == "1") ? true : false;
			}
		}

		public static bool Cfg_Pop3OverSsl
		{
			get 
			{
				string eme = ConfigurationManager.AppSettings["POP3_SSL"];
				return (eme != null && eme == "1") ? true : false;
			}
		}

        public static bool InteropInternaEnabled
        {
            get
            {
                bool supported = false;
                if (ConfigurationManager.AppSettings["INTEROP_INT_NO_MAIL"] != null && ConfigurationManager.AppSettings["INTEROP_INT_NO_MAIL"].Equals("1"))
                {
                    supported = true;
                }
                return supported;
            }
        }


     
        /// <summary></summary>
        /// <param name="qco"></param>
        /// <returns></returns>
        public static string getMailAddress(DocsPaVO.addressbook.QueryCorrispondente qco)
        {
            logger.Debug("getMailAddress");
            string mailMitt = "";
            System.Collections.ArrayList mittArr = new System.Collections.ArrayList();
            if (qco.tipoUtente == DocsPaVO.addressbook.TipoUtente.ESTERNO)
            {
                mittArr = BusinessLogic.Utenti.addressBookManager.listaCorrispondentiEstMethod(qco);
            }
            else
            {
                mittArr = BusinessLogic.Utenti.addressBookManager.listaCorrispondentiIntMethod(qco);
            }
            if (mittArr[0].GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
            {
                logger.Debug("UO");
                mailMitt = ((DocsPaVO.utente.UnitaOrganizzativa)mittArr[0]).email;
            }
            if (mittArr[0].GetType() == typeof(DocsPaVO.utente.Ruolo))
            {
                logger.Debug("Ruolo");
                mailMitt = ((DocsPaVO.utente.Ruolo)mittArr[0]).uo.email;
            }
            if (mittArr[0].GetType() == typeof(DocsPaVO.utente.Utente))
            {
                logger.Debug("Utente");
                mailMitt = ((DocsPaVO.utente.Ruolo)((DocsPaVO.utente.Utente)mittArr[0]).ruoli[0]).uo.email;
            }
            logger.Debug(mailMitt);
            return mailMitt;
        }


        /// <summary>
        /// Trova un ID Documento (profile) da Codice AOO, Protocollo e anno
        /// </summary>
        /// <param name="codiceAOO"></param>
        /// <param name="numeroRegistrazione"></param>
        /// <param name="anno"></param>
        /// <returns></returns>
        public static string findIdProfile(string codiceAOO, string numeroRegistrazione, int anno)
        {
            DataSet ds;
            try
            {
                DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                obj.findIdProf(out ds, codiceAOO, numeroRegistrazione, anno);


                if (ds.Tables["ID_DOC"].Rows.Count == 0)
                {
                    return null;
                }
                else
                {
                    return ds.Tables["ID_DOC"].Rows[0]["SYSTEM_ID"].ToString();
                }
            }
            catch (Exception e)
            {
                logger.Error("Eccezione: " + e.Message);
                return null;
            }
        }


        public static  List<string> getCorrIdListByEmail(string email, string idRegistro)
        {
            List<string> retval = new List<string>();
            DataSet ds = BusinessLogic.Utenti.UserManager.GetCorrByEmail(email, idRegistro);

            string sys = string.Empty;
            if (ds.Tables.Count == 1)
                foreach (DataRow s in ds.Tables[0].Rows)
                    retval.Add(s["SYSTEM_ID"].ToString());

            


            return retval;
        }

        /// <summary>
        /// 1- Attiva multi casella RF/REGISTRI, destinatari esterni
        /// Se questa chiave è a 0, non sarà possibile :
        /// FrontEnd: inserire più di una casella mail per i corrispondenti esterni nuovi o in modifica. 
        /// Amm.ne: non sarà possibile inserire più di una casella mail su Registri /RF.
        /// </summary>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public static bool MultiMailEnabled(string idAmm)
        {
            bool enabled = false;
            DocsPaVO.amministrazione.ConfigRepository chiaviAmm = DocsPaUtils.Configuration.InitConfigurationKeys.getInstance(idAmm);
            if (chiaviAmm != null && chiaviAmm.ContainsKey("FE_ATTIVA_GESTIONE_MULTIMAIL"))
                enabled = (!string.IsNullOrEmpty(chiaviAmm["FE_ATTIVA_GESTIONE_MULTIMAIL"].ToString()) &&
                    chiaviAmm["FE_ATTIVA_GESTIONE_MULTIMAIL"].ToString().Equals("1")) ? true : false;
            return enabled;
        }

        /// <summary>
        /// Metodo che controlla se la mail è configurata per mantenere le mail ricevute come pendenti.
        /// Per gestione pendenti tramite PEC
        /// </summary>
        /// <param name="idregistro"></param>
        /// <param name="casella"></param>
        /// <returns></returns>
        public static bool MantieniMailRicevutePendenti(string idregistro, string casella)
        {
            bool retval = false;
            DataSet ds = new DataSet();
            DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();

                    obj.getVarMailRicPendenti(out ds, idregistro,casella);

                    if (ds.Tables["REGISTRO"].Rows.Count > 0)
                    {
                        DataRow regRow = ds.Tables["REGISTRO"].Rows[0];
                        if (regRow["VAR_SOLO_MAIL_PEC"].ToString() != "1")
                        {
                            if (regRow["VAR_MAIL_RIC_PENDENTE"].ToString() == "1") retval = true;
                        }
                    }

            return retval;
        }

        /// <summary>
        /// Controlla se un documento è stato ricevuto tramite una casella configurata per mantenere le mail ricevute come pendenti.
        /// Serve per il frontend.
        /// Per gestione pendenti tramite PEC
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        public static bool isDocPecPendente(string idDocument)
        {
            bool retval = false;
            DataSet ds = new DataSet();
            DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();

            retval= obj.getDocPECPendente(idDocument);

            //if (ds.Tables["REGISTRO"].Rows.Count > 0)
            //{
            //    DataRow regRow = ds.Tables["DOCUMENTO"].Rows[0];
            //    if (regRow["VAR_SOLO_MAIL_PEC"].ToString() != "1")
            //    {
            //        if (regRow["VAR_MAIL_RIC_PENDENTE"].ToString() == "1") retval = true;
            //    }
            //}
            return retval;
        }


        /// <summary>
        /// Dato un filerequest estrae la marca associata se esiste.
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        public static byte[] GetTSRForDocument(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.FileRequest fileRequest)
        {
            byte[] retval = null;
            try
            {
                ArrayList tsrAL = BusinessLogic.Documenti.TimestampManager.getTimestampsDoc(infoUtente, fileRequest);
                foreach (DocsPaVO.documento.TimestampDoc ts in tsrAL)
                {
                    retval = Convert.FromBase64String(ts.TSR_FILE);
                    break;
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Errore Ricavando il TS {0} {1}", e.Message, e.StackTrace);
            }
            return retval;
        }

        /// <summary>
        /// Prova a tovare un match con il TSR verso i file nella cartella 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="tsrFileName"></param>
        /// <returns>In caso di associabilità torna true, altrimenti da false</returns>
        public static bool FindTSRMatch(string path, string tsrFileName)
        {
            bool retval = false;
            try
            {
                if (!Path.GetExtension(tsrFileName).ToLowerInvariant().Contains("tsr"))
                {
                    return false;
                }
                byte[] tsrFile = File.ReadAllBytes(path + "\\" + tsrFileName);

                BusinessLogic.Documenti.DigitalSignature.VerifyTimeStamp vts = new Documenti.DigitalSignature.VerifyTimeStamp();
                string[] Files = Directory.GetFiles(path);

                foreach (string file in Files)
                {
                    if (!Path.GetExtension(file).ToLowerInvariant().Contains("tsr"))
                    {
                        byte[] FileTocheck = File.ReadAllBytes(file);
                        retval = vts.machTSR(tsrFile, FileTocheck);
                        if (retval)
                            return retval;
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Errore Cercando il TSR {0} : {1}", e.Message, e.StackTrace);
            }
            return retval;
        }

        public static bool MatchTSR(byte[] tsrFile, byte[] content)
        {
            try
            {
                BusinessLogic.Documenti.DigitalSignature.VerifyTimeStamp vts = new Documenti.DigitalSignature.VerifyTimeStamp();
                if (vts.machTSR(tsrFile, content))
                {
                    DocsPaVO.areaConservazione.OutputResponseMarca resultMarca = vts.Verify(content, tsrFile);
                    if (resultMarca.esito == "OK")
                        return true;
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Errore verificando il TSR {0} {1}", e.Message, e.StackTrace);
            }
            return false;
        }

        /// <summary>
        /// Associa un TSR a un documento ricevuto
        /// </summary>
        /// <param name="path">path dove sono presenti i file ricevuti in mail</param>
        /// <param name="fr">filerequest</param>
        /// <param name="fd">filedocumento</param>
        /// <param name="infoUtente">infoutente</param>
        /// <returns>False in caso di fallimento</returns>
        public static bool MatchTSR(string path, DocsPaVO.documento.FileRequest fr, DocsPaVO.documento.FileDocumento fd, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool retval = false;
            string[] tsrFiles = Directory.GetFiles(path, "*.tsr");
            //se non ci sono TSR
            if (tsrFiles.Length==0)
                return false;

            if (Path.GetExtension(fr.fileName).ToLowerInvariant() == ".tsr")
            {
                //logger.DebugFormat ("Il file è un TSR, non associo un TSR a un TSR");
                return false;
            }
            
            BusinessLogic.Documenti.DigitalSignature.VerifyTimeStamp vts = new Documenti.DigitalSignature.VerifyTimeStamp();
            string fdFileName = path + "\\" + fr.fileName;
            foreach (string file in tsrFiles)
            {
               // byte[] tsrFile = File.ReadAllBytes(file);

                byte[] tsrFile ;
                if (fdFileName.ToLowerInvariant().Equals(file.ToLowerInvariant()))
                {
                    tsrFile = fd.content;
                }
                else
                {

                    System.IO.FileStream fsAll = new System.IO.FileStream(file, System.IO.FileMode.Open, FileAccess.Read, FileShare.Read);
                    tsrFile = new byte[fsAll.Length];
                    fsAll.Read(tsrFile, 0, (int)fsAll.Length);
                    fsAll.Close();
                }
                try
                {
                    if (vts.machTSR(tsrFile, fd.content))
                    {
                        logger.DebugFormat("Provo TSR {0}", file);
                        DocsPaVO.areaConservazione.OutputResponseMarca resultMarca = vts.Verify(fd.content, tsrFile);
                        if (resultMarca.esito == "OK")
                        {
                            DocsPaDB.Query_DocsPAWS.TimestampDoc timestampDoc = new DocsPaDB.Query_DocsPAWS.TimestampDoc();
                            timestampDoc.saveTSR(infoUtente, resultMarca, fr);
                            logger.DebugFormat("Associato TSR al documento {0} {1}", fr.docNumber, fr.versionId);
                            retval = true;
                        }
                    }
                }
                catch (Exception e1)
                {
                    logger.ErrorFormat("Errore Cercando di associare il TSR {0} : {1}", e1.Message, e1.StackTrace);
                }
            }
            return retval;
        }

        
    }
    #region mimemammper

    public class MimeMapper
    {
        private static IDictionary<string, string> _mappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {

        #region Big freaking list of mime types
        // combination of values from Windows 7 Registry and 
        // from C:\Windows\System32\inetsrv\config\applicationHost.config
        // some added, including .7z and .dat
        {".doc", "application/msword"},
        {".xls", "application/vnd.ms-excel"},
        {".ppt", "application/vnd.ms-powerpoint"},
        {".txt", "text/plain"},
        {".bin", "application/octet-stream"},

        {".323", "text/h323"},
        {".3g2", "video/3gpp2"},
        {".3gp", "video/3gpp"},
        {".3gp2", "video/3gpp2"},
        {".3gpp", "video/3gpp"},
        {".7z", "application/x-7z-compressed"},
        {".aa", "audio/audible"},
        {".AAC", "audio/aac"},
        {".aaf", "application/octet-stream"},
        {".aax", "audio/vnd.audible.aax"},
        {".ac3", "audio/ac3"},
        {".aca", "application/octet-stream"},
        {".accda", "application/msaccess.addin"},
        {".accdb", "application/msaccess"},
        {".accdc", "application/msaccess.cab"},
        {".accde", "application/msaccess"},
        {".accdr", "application/msaccess.runtime"},
        {".accdt", "application/msaccess"},
        {".accdw", "application/msaccess.webapplication"},
        {".accft", "application/msaccess.ftemplate"},
        {".acx", "application/internet-property-stream"},
        {".AddIn", "text/xml"},
        {".ade", "application/msaccess"},
        {".adobebridge", "application/x-bridge-url"},
        {".adp", "application/msaccess"},
        {".ADT", "audio/vnd.dlna.adts"},
        {".ADTS", "audio/aac"},
        {".afm", "application/octet-stream"},
        {".ai", "application/postscript"},
        {".aif", "audio/x-aiff"},
        {".aifc", "audio/aiff"},
        {".aiff", "audio/aiff"},
        {".air", "application/vnd.adobe.air-application-installer-package+zip"},
        {".amc", "application/x-mpeg"},
        {".application", "application/x-ms-application"},
        {".art", "image/x-jg"},
        {".asa", "application/xml"},
        {".asax", "application/xml"},
        {".ascx", "application/xml"},
        {".asd", "application/octet-stream"},
        {".asf", "video/x-ms-asf"},
        {".ashx", "application/xml"},
        {".asi", "application/octet-stream"},
        {".asm", "text/plain"},
        {".asmx", "application/xml"},
        {".aspx", "application/xml"},
        {".asr", "video/x-ms-asf"},
        {".asx", "video/x-ms-asf"},
        {".atom", "application/atom+xml"},
        {".au", "audio/basic"},
        {".avi", "video/x-msvideo"},
        {".axs", "application/olescript"},
        {".bas", "text/plain"},
        {".bcpio", "application/x-bcpio"},
        {".bmp", "image/bmp"},
        {".c", "text/plain"},
        {".cab", "application/octet-stream"},
        {".caf", "audio/x-caf"},
        {".calx", "application/vnd.ms-office.calx"},
        {".cat", "application/vnd.ms-pki.seccat"},
        {".cc", "text/plain"},
        {".cd", "text/plain"},
        {".cdda", "audio/aiff"},
        {".cdf", "application/x-cdf"},
        {".cer", "application/x-x509-ca-cert"},
        {".chm", "application/octet-stream"},
        {".class", "application/x-java-applet"},
        {".clp", "application/x-msclip"},
        {".cmx", "image/x-cmx"},
        {".cnf", "text/plain"},
        {".cod", "image/cis-cod"},
        {".config", "application/xml"},
        {".contact", "text/x-ms-contact"},
        {".coverage", "application/xml"},
        {".cpio", "application/x-cpio"},
        {".cpp", "text/plain"},
        {".crd", "application/x-mscardfile"},
        {".crl", "application/pkix-crl"},
        {".crt", "application/x-x509-ca-cert"},
        {".cs", "text/plain"},
        {".csdproj", "text/plain"},
        {".csh", "application/x-csh"},
        {".csproj", "text/plain"},
        {".css", "text/css"},
        {".csv", "text/csv"},
        {".cur", "application/octet-stream"},
        {".cxx", "text/plain"},
        {".dat", "application/octet-stream"},
        {".datasource", "application/xml"},
        {".dbproj", "text/plain"},
        {".dcr", "application/x-director"},
        {".def", "text/plain"},
        {".deploy", "application/octet-stream"},
        {".der", "application/x-x509-ca-cert"},
        {".dgml", "application/xml"},
        {".dib", "image/bmp"},
        {".dif", "video/x-dv"},
        {".dir", "application/x-director"},
        {".disco", "text/xml"},
        {".dll", "application/x-msdownload"},
        {".dll.config", "text/xml"},
        {".dlm", "text/dlm"},
        {".docm", "application/vnd.ms-word.document.macroEnabled.12"},
        {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
        {".dot", "application/msword"},
        {".dotm", "application/vnd.ms-word.template.macroEnabled.12"},
        {".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
        {".dsp", "application/octet-stream"},
        {".dsw", "text/plain"},
        {".dtd", "text/xml"},
        {".dtsConfig", "text/xml"},
        {".dv", "video/x-dv"},
        {".dvi", "application/x-dvi"},
        {".dwf", "drawing/x-dwf"},
        {".dwp", "application/octet-stream"},
        {".dxr", "application/x-director"},
        {".eml", "message/rfc822"},
        {".emz", "application/octet-stream"},
        {".eot", "application/octet-stream"},
        {".eps", "application/postscript"},
        {".etl", "application/etl"},
        {".etx", "text/x-setext"},
        {".evy", "application/envoy"},
        {".exe", "application/octet-stream"},
        {".exe.config", "text/xml"},
        {".fdf", "application/vnd.fdf"},
        {".fif", "application/fractals"},
        {".filters", "Application/xml"},
        {".fla", "application/octet-stream"},
        {".flr", "x-world/x-vrml"},
        {".flv", "video/x-flv"},
        {".fsscript", "application/fsharp-script"},
        {".fsx", "application/fsharp-script"},
        {".generictest", "application/xml"},
        {".gif", "image/gif"},
        {".group", "text/x-ms-group"},
        {".gsm", "audio/x-gsm"},
        {".gtar", "application/x-gtar"},
        {".gz", "application/x-gzip"},
        {".h", "text/plain"},
        {".hdf", "application/x-hdf"},
        {".hdml", "text/x-hdml"},
        {".hhc", "application/x-oleobject"},
        {".hhk", "application/octet-stream"},
        {".hhp", "application/octet-stream"},
        {".hlp", "application/winhlp"},
        {".hpp", "text/plain"},
        {".hqx", "application/mac-binhex40"},
        {".hta", "application/hta"},
        {".htc", "text/x-component"},
        {".htm", "text/html"},
        {".html", "text/html"},
        {".htt", "text/webviewhtml"},
        {".hxa", "application/xml"},
        {".hxc", "application/xml"},
        {".hxd", "application/octet-stream"},
        {".hxe", "application/xml"},
        {".hxf", "application/xml"},
        {".hxh", "application/octet-stream"},
        {".hxi", "application/octet-stream"},
        {".hxk", "application/xml"},
        {".hxq", "application/octet-stream"},
        {".hxr", "application/octet-stream"},
        {".hxs", "application/octet-stream"},
        {".hxt", "text/html"},
        {".hxv", "application/xml"},
        {".hxw", "application/octet-stream"},
        {".hxx", "text/plain"},
        {".i", "text/plain"},
        {".ico", "image/x-icon"},
        {".ics", "application/octet-stream"},
        {".idl", "text/plain"},
        {".ief", "image/ief"},
        {".iii", "application/x-iphone"},
        {".inc", "text/plain"},
        {".inf", "application/octet-stream"},
        {".inl", "text/plain"},
        {".ins", "application/x-internet-signup"},
        {".ipa", "application/x-itunes-ipa"},
        {".ipg", "application/x-itunes-ipg"},
        {".ipproj", "text/plain"},
        {".ipsw", "application/x-itunes-ipsw"},
        {".iqy", "text/x-ms-iqy"},
        {".isp", "application/x-internet-signup"},
        {".ite", "application/x-itunes-ite"},
        {".itlp", "application/x-itunes-itlp"},
        {".itms", "application/x-itunes-itms"},
        {".itpc", "application/x-itunes-itpc"},
        {".IVF", "video/x-ivf"},
        {".jar", "application/java-archive"},
        {".java", "application/octet-stream"},
        {".jck", "application/liquidmotion"},
        {".jcz", "application/liquidmotion"},
        {".jfif", "image/pjpeg"},
        {".jnlp", "application/x-java-jnlp-file"},
        {".jpb", "application/octet-stream"},
        {".jpg", "image/jpeg"},
        {".jpeg", "image/jpeg"},
        {".jpe", "image/jpeg"},
        {".js", "application/x-javascript"},
        {".jsx", "text/jscript"},
        {".jsxbin", "text/plain"},
        {".latex", "application/x-latex"},
        {".library-ms", "application/windows-library+xml"},
        {".lit", "application/x-ms-reader"},
        {".loadtest", "application/xml"},
        {".lpk", "application/octet-stream"},
        {".lsf", "video/x-la-asf"},
        {".lst", "text/plain"},
        {".lsx", "video/x-la-asf"},
        {".lzh", "application/octet-stream"},
        {".m13", "application/x-msmediaview"},
        {".m14", "application/x-msmediaview"},
        {".m1v", "video/mpeg"},
        {".m2t", "video/vnd.dlna.mpeg-tts"},
        {".m2ts", "video/vnd.dlna.mpeg-tts"},
        {".m2v", "video/mpeg"},
        {".m3u", "audio/x-mpegurl"},
        {".m3u8", "audio/x-mpegurl"},
        {".m4a", "audio/m4a"},
        {".m4b", "audio/m4b"},
        {".m4p", "audio/m4p"},
        {".m4r", "audio/x-m4r"},
        {".m4v", "video/x-m4v"},
        {".mac", "image/x-macpaint"},
        {".mak", "text/plain"},
        {".man", "application/x-troff-man"},
        {".manifest", "application/x-ms-manifest"},
        {".map", "text/plain"},
        {".master", "application/xml"},
        {".mda", "application/msaccess"},
        {".mdb", "application/x-msaccess"},
        {".mde", "application/msaccess"},
        {".mdp", "application/octet-stream"},
        {".me", "application/x-troff-me"},
        {".mfp", "application/x-shockwave-flash"},
        {".mht", "message/rfc822"},
        {".mhtml", "message/rfc822"},
        {".mid", "audio/mid"},
        {".midi", "audio/mid"},
        {".mix", "application/octet-stream"},
        {".mk", "text/plain"},
        {".mmf", "application/x-smaf"},
        {".mno", "text/xml"},
        {".mny", "application/x-msmoney"},
        {".mod", "video/mpeg"},
        {".mov", "video/quicktime"},
        {".movie", "video/x-sgi-movie"},
        {".mp2", "video/mpeg"},
        {".mp2v", "video/mpeg"},
        {".mp3", "audio/mpeg"},
        {".mp4", "video/mp4"},
        {".mp4v", "video/mp4"},
        {".mpa", "video/mpeg"},
        {".mpe", "video/mpeg"},
        {".mpeg", "video/mpeg"},
        {".mpf", "application/vnd.ms-mediapackage"},
        {".mpg", "video/mpeg"},
        {".mpp", "application/vnd.ms-project"},
        {".mpv2", "video/mpeg"},
        {".mqv", "video/quicktime"},
        {".ms", "application/x-troff-ms"},
        {".msi", "application/octet-stream"},
        {".mso", "application/octet-stream"},
        {".mts", "video/vnd.dlna.mpeg-tts"},
        {".mtx", "application/xml"},
        {".mvb", "application/x-msmediaview"},
        {".mvc", "application/x-miva-compiled"},
        {".mxp", "application/x-mmxp"},
        {".nc", "application/x-netcdf"},
        {".nsc", "video/x-ms-asf"},
        {".nws", "message/rfc822"},
        {".ocx", "application/octet-stream"},
        {".oda", "application/oda"},
        {".odc", "text/x-ms-odc"},
        {".odh", "text/plain"},
        {".odl", "text/plain"},
        {".odp", "application/vnd.oasis.opendocument.presentation"},
        {".ods", "application/oleobject"},
        {".odt", "application/vnd.oasis.opendocument.text"},
        {".one", "application/onenote"},
        {".onea", "application/onenote"},
        {".onepkg", "application/onenote"},
        {".onetmp", "application/onenote"},
        {".onetoc", "application/onenote"},
        {".onetoc2", "application/onenote"},
        {".orderedtest", "application/xml"},
        {".osdx", "application/opensearchdescription+xml"},
        {".p10", "application/pkcs10"},
        {".p12", "application/x-pkcs12"},
        {".p7b", "application/x-pkcs7-certificates"},
        {".p7c", "application/pkcs7-mime"},
        {".p7m", "application/pkcs7-mime"},
        {".p7r", "application/x-pkcs7-certreqresp"},
        {".p7s", "application/pkcs7-signature"},
        {".pbm", "image/x-portable-bitmap"},
        {".pcast", "application/x-podcast"},
        {".pct", "image/pict"},
        {".pcx", "application/octet-stream"},
        {".pcz", "application/octet-stream"},
        {".pdf", "application/pdf"},
        {".pfb", "application/octet-stream"},
        {".pfm", "application/octet-stream"},
        {".pfx", "application/x-pkcs12"},
        {".pgm", "image/x-portable-graymap"},
        {".pic", "image/pict"},
        {".pict", "image/pict"},
        {".pkgdef", "text/plain"},
        {".pkgundef", "text/plain"},
        {".pko", "application/vnd.ms-pki.pko"},
        {".pls", "audio/scpls"},
        {".pma", "application/x-perfmon"},
        {".pmc", "application/x-perfmon"},
        {".pml", "application/x-perfmon"},
        {".pmr", "application/x-perfmon"},
        {".pmw", "application/x-perfmon"},
        {".png", "image/png"},
        {".pnm", "image/x-portable-anymap"},
        {".pnt", "image/x-macpaint"},
        {".pntg", "image/x-macpaint"},
        {".pnz", "image/png"},
        {".pot", "application/vnd.ms-powerpoint"},
        {".potm", "application/vnd.ms-powerpoint.template.macroEnabled.12"},
        {".potx", "application/vnd.openxmlformats-officedocument.presentationml.template"},
        {".ppa", "application/vnd.ms-powerpoint"},
        {".ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12"},
        {".ppm", "image/x-portable-pixmap"},
        {".pps", "application/vnd.ms-powerpoint"},
        {".ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
        {".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
        {".pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
        {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
        {".prf", "application/pics-rules"},
        {".prm", "application/octet-stream"},
        {".prx", "application/octet-stream"},
        {".ps", "application/postscript"},
        {".psc1", "application/PowerShell"},
        {".psd", "application/octet-stream"},
        {".psess", "application/xml"},
        {".psm", "application/octet-stream"},
        {".psp", "application/octet-stream"},
        {".pub", "application/x-mspublisher"},
        {".pwz", "application/vnd.ms-powerpoint"},
        {".qht", "text/x-html-insertion"},
        {".qhtm", "text/x-html-insertion"},
        {".qt", "video/quicktime"},
        {".qti", "image/x-quicktime"},
        {".qtif", "image/x-quicktime"},
        {".qtl", "application/x-quicktimeplayer"},
        {".qxd", "application/octet-stream"},
        {".ra", "audio/x-pn-realaudio"},
        {".ram", "audio/x-pn-realaudio"},
        {".rar", "application/octet-stream"},
        {".ras", "image/x-cmu-raster"},
        {".rat", "application/rat-file"},
        {".rc", "text/plain"},
        {".rc2", "text/plain"},
        {".rct", "text/plain"},
        {".rdlc", "application/xml"},
        {".resx", "application/xml"},
        {".rf", "image/vnd.rn-realflash"},
        {".rgb", "image/x-rgb"},
        {".rgs", "text/plain"},
        {".rm", "application/vnd.rn-realmedia"},
        {".rmi", "audio/mid"},
        {".rmp", "application/vnd.rn-rn_music_package"},
        {".roff", "application/x-troff"},
        {".rpm", "audio/x-pn-realaudio-plugin"},
        {".rqy", "text/x-ms-rqy"},
        {".rtf", "application/rtf"},
        {".rtx", "text/richtext"},
        {".ruleset", "application/xml"},
        {".s", "text/plain"},
        {".safariextz", "application/x-safari-safariextz"},
        {".scd", "application/x-msschedule"},
        {".sct", "text/scriptlet"},
        {".sd2", "audio/x-sd2"},
        {".sdp", "application/sdp"},
        {".sea", "application/octet-stream"},
        {".searchConnector-ms", "application/windows-search-connector+xml"},
        {".setpay", "application/set-payment-initiation"},
        {".setreg", "application/set-registration-initiation"},
        {".settings", "application/xml"},
        {".sgimb", "application/x-sgimb"},
        {".sgml", "text/sgml"},
        {".sh", "application/x-sh"},
        {".shar", "application/x-shar"},
        {".shtml", "text/html"},
        {".sit", "application/x-stuffit"},
        {".sitemap", "application/xml"},
        {".skin", "application/xml"},
        {".sldm", "application/vnd.ms-powerpoint.slide.macroEnabled.12"},
        {".sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide"},
        {".slk", "application/vnd.ms-excel"},
        {".sln", "text/plain"},
        {".slupkg-ms", "application/x-ms-license"},
        {".smd", "audio/x-smd"},
        {".smi", "application/octet-stream"},
        {".smx", "audio/x-smd"},
        {".smz", "audio/x-smd"},
        {".snd", "audio/basic"},
        {".snippet", "application/xml"},
        {".snp", "application/octet-stream"},
        {".sol", "text/plain"},
        {".sor", "text/plain"},
        {".spc", "application/x-pkcs7-certificates"},
        {".spl", "application/futuresplash"},
        {".src", "application/x-wais-source"},
        {".srf", "text/plain"},
        {".SSISDeploymentManifest", "text/xml"},
        {".ssm", "application/streamingmedia"},
        {".sst", "application/vnd.ms-pki.certstore"},
        {".stl", "application/vnd.ms-pki.stl"},
        {".sv4cpio", "application/x-sv4cpio"},
        {".sv4crc", "application/x-sv4crc"},
        {".svc", "application/xml"},
        {".swf", "application/x-shockwave-flash"},
        {".t", "application/x-troff"},
        {".tar", "application/x-tar"},
        {".tcl", "application/x-tcl"},
        {".testrunconfig", "application/xml"},
        {".testsettings", "application/xml"},
        {".tex", "application/x-tex"},
        {".texi", "application/x-texinfo"},
        {".texinfo", "application/x-texinfo"},
        {".tgz", "application/x-compressed"},
        {".thmx", "application/vnd.ms-officetheme"},
        {".thn", "application/octet-stream"},
        {".tif", "image/tiff"},
        {".tiff", "image/tiff"},
        {".tlh", "text/plain"},
        {".tli", "text/plain"},
        {".toc", "application/octet-stream"},
        {".tr", "application/x-troff"},
        {".trm", "application/x-msterminal"},
        {".trx", "application/xml"},
        {".ts", "video/vnd.dlna.mpeg-tts"},
        {".tsv", "text/tab-separated-values"},
        {".ttf", "application/octet-stream"},
        {".tts", "video/vnd.dlna.mpeg-tts"},
        {".u32", "application/octet-stream"},
        {".uls", "text/iuls"},
        {".user", "text/plain"},
        {".ustar", "application/x-ustar"},
        {".vb", "text/plain"},
        {".vbdproj", "text/plain"},
        {".vbk", "video/mpeg"},
        {".vbproj", "text/plain"},
        {".vbs", "text/vbscript"},
        {".vcf", "text/x-vcard"},
        {".vcproj", "Application/xml"},
        {".vcs", "text/plain"},
        {".vcxproj", "Application/xml"},
        {".vddproj", "text/plain"},
        {".vdp", "text/plain"},
        {".vdproj", "text/plain"},
        {".vdx", "application/vnd.ms-visio.viewer"},
        {".vml", "text/xml"},
        {".vscontent", "application/xml"},
        {".vsct", "text/xml"},
        {".vsd", "application/vnd.visio"},
        {".vsi", "application/ms-vsi"},
        {".vsix", "application/vsix"},
        {".vsixlangpack", "text/xml"},
        {".vsixmanifest", "text/xml"},
        {".vsmdi", "application/xml"},
        {".vspscc", "text/plain"},
        {".vss", "application/vnd.visio"},
        {".vsscc", "text/plain"},
        {".vssettings", "text/xml"},
        {".vssscc", "text/plain"},
        {".vst", "application/vnd.visio"},
        {".vstemplate", "text/xml"},
        {".vsto", "application/x-ms-vsto"},
        {".vsw", "application/vnd.visio"},
        {".vsx", "application/vnd.visio"},
        {".vtx", "application/vnd.visio"},
        {".wav", "audio/wav"},
        {".wave", "audio/wav"},
        {".wax", "audio/x-ms-wax"},
        {".wbk", "application/msword"},
        {".wbmp", "image/vnd.wap.wbmp"},
        {".wcm", "application/vnd.ms-works"},
        {".wdb", "application/vnd.ms-works"},
        {".wdp", "image/vnd.ms-photo"},
        {".webarchive", "application/x-safari-webarchive"},
        {".webtest", "application/xml"},
        {".wiq", "application/xml"},
        {".wiz", "application/msword"},
        {".wks", "application/vnd.ms-works"},
        {".WLMP", "application/wlmoviemaker"},
        {".wlpginstall", "application/x-wlpg-detect"},
        {".wlpginstall3", "application/x-wlpg3-detect"},
        {".wm", "video/x-ms-wm"},
        {".wma", "audio/x-ms-wma"},
        {".wmd", "application/x-ms-wmd"},
        {".wmf", "application/x-msmetafile"},
        {".wml", "text/vnd.wap.wml"},
        {".wmlc", "application/vnd.wap.wmlc"},
        {".wmls", "text/vnd.wap.wmlscript"},
        {".wmlsc", "application/vnd.wap.wmlscriptc"},
        {".wmp", "video/x-ms-wmp"},
        {".wmv", "video/x-ms-wmv"},
        {".wmx", "video/x-ms-wmx"},
        {".wmz", "application/x-ms-wmz"},
        {".wpl", "application/vnd.ms-wpl"},
        {".wps", "application/vnd.ms-works"},
        {".wri", "application/x-mswrite"},
        {".wrl", "x-world/x-vrml"},
        {".wrz", "x-world/x-vrml"},
        {".wsc", "text/scriptlet"},
        {".wsdl", "text/xml"},
        {".wvx", "video/x-ms-wvx"},
        {".x", "application/directx"},
        {".xaf", "x-world/x-vrml"},
        {".xaml", "application/xaml+xml"},
        {".xap", "application/x-silverlight-app"},
        {".xbap", "application/x-ms-xbap"},
        {".xbm", "image/x-xbitmap"},
        {".xdr", "text/plain"},
        {".xht", "application/xhtml+xml"},
        {".xhtml", "application/xhtml+xml"},
        {".xla", "application/vnd.ms-excel"},
        {".xlam", "application/vnd.ms-excel.addin.macroEnabled.12"},
        {".xlc", "application/vnd.ms-excel"},
        {".xld", "application/vnd.ms-excel"},
        {".xlk", "application/vnd.ms-excel"},
        {".xll", "application/vnd.ms-excel"},
        {".xlm", "application/vnd.ms-excel"},
        {".xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
        {".xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12"},
        {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
        {".xlt", "application/vnd.ms-excel"},
        {".xltm", "application/vnd.ms-excel.template.macroEnabled.12"},
        {".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
        {".xlw", "application/vnd.ms-excel"},
        {".xml", "text/xml"},
        {".xmta", "application/xml"},
        {".xof", "x-world/x-vrml"},
        {".XOML", "text/plain"},
        {".xpm", "image/x-xpixmap"},
        {".xps", "application/vnd.ms-xpsdocument"},
        {".xrm-ms", "text/xml"},
        {".xsc", "application/xml"},
        {".xsd", "text/xml"},
        {".xsf", "text/xml"},
        {".xsl", "text/xml"},
        {".xslt", "text/xml"},
        {".xsn", "application/octet-stream"},
        {".xss", "application/xml"},
        {".xtp", "application/octet-stream"},
        {".xwd", "image/x-xwindowdump"},
        {".z", "application/x-compress"},
        {".zip", "application/x-zip-compressed"},
        #endregion

        };

        public static void AddMimeType (string extension, string mimetype)
        {
            if (extension == null)
            {
                throw new ArgumentNullException("extension");
            }
 
            extension = extension.ToLower();
            mimetype = mimetype.ToLower();
            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            if (!_mappings.ContainsKey(extension))
                _mappings.Add(extension, mimetype);

        }

        public static string GetMimeType(string extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException("extension");
            }

            extension = extension.ToLower();
            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            string mime;

            return _mappings.TryGetValue(extension, out mime) ? mime : "application/octet-stream";
        }

        public static string GetExtensionFromMime(string MimeType)
        {
            if (MimeType == null)
            {
                throw new ArgumentNullException("No MimeType");
            }

            MimeType = MimeType.ToLower();
            return  _mappings.Where(p => p.Value == MimeType).Select(p => p.Key).FirstOrDefault();

        }
        
    }
    #endregion
}
