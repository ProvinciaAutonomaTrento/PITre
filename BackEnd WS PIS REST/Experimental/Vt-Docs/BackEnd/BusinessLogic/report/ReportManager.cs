using System;
using System.Collections;
using System.IO;
using log4net;
using DocsPaVO.utente;
using System.Text.RegularExpressions;

namespace BusinessLogic.Report
{
	/// <summary>
	/// </summary>
	public class ReportManager
	{
        private static ILog logger = LogManager.GetLogger(typeof(ReportManager));
		/// <summary></summary>
		/// <param name="schedaDoc"></param>
		/// <param name="infoUtente"></param>
		/// <returns></returns>
		public static DocsPaVO.documento.FileDocumento getSchedaDocReport(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDoc, string mittDest_Indirizzo)
		{
			DocsPaVO.documento.FileDocumento result=null;

			if(schedaDoc.protocollo==null)
			{
				result=GetStampaProfiloRTF(infoUtente, schedaDoc);
			}
			else
			{
				result=new DocsPaVO.documento.FileDocumento();

				string path = AppDomain.CurrentDomain.BaseDirectory ;
				string report = string.Empty;	
				bool existUffRef = false;	

				try
				{
					existUffRef = ExistUfficioReferente(schedaDoc);
					
					logger.Debug("Generazione report");

					if (existUffRef)
						report = ReportUtils.stringFile(path + "\\report\\schedaDoc\\SchedaDoc_UffRef.rtf");
					else
						report = ReportUtils.stringFile(path + "\\report\\schedaDoc\\SchedaDoc.rtf");
                    if (schedaDoc.registro != null)
                    {
                        report = report.Replace("XNOME_REGISTRO", schedaDoc.registro.descrizione);
                        report = report.Replace("XCODICE_REGISTRO", schedaDoc.registro.codRegistro);
                    }
                    else
                    {
                        report = report.Replace("XNOME_REGISTRO", string.Empty);
                        report = report.Replace("XCODICE_REGISTRO", string.Empty);
                    }
					report=report.Replace("XNUM_PR",schedaDoc.protocollo.numero);
					report=report.Replace("XDTA_PR",schedaDoc.protocollo.dataProtocollazione);
					report=report.Replace("XA/P",schedaDoc.tipoProto);
					
					string mittDest="";
					string protMitt="";
					string uffRef="";	// Ufficio referente
				
                    DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();


					if(schedaDoc.protocollo.GetType().Equals(typeof(DocsPaVO.documento.ProtocolloEntrata)))
					{
                        if (mittDest_Indirizzo.ToUpper() =="FALSE")
                            mittDest = ((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).mittente.descrizione;
                        else
                            if (mittDest_Indirizzo.ToUpper() == "TRUE")
                            mittDest = doc.GetIndirizzoMittDest(schedaDoc.systemId, schedaDoc.tipoProto);
						protMitt=((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).dataProtocolloMittente+((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).descrizioneProtocolloMittente;

						// Ufficio referente
						if (existUffRef)
							uffRef = ((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).ufficioReferente.descrizione;
					}
					else
					{
						// Ufficio referente
						if (existUffRef)
							uffRef = ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).ufficioReferente.descrizione;

                        if (mittDest_Indirizzo.ToUpper()=="FALSE")
                        {
                            System.Collections.ArrayList dest = ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari;
                            for (int i = 0; i < dest.Count; i++)
                            {
                                mittDest = mittDest + ((DocsPaVO.utente.Corrispondente)dest[i]).descrizione;
                                if (i < (dest.Count - 1))
                                {
                                    mittDest = mittDest + " \\par ";
                                }
                            }
                            System.Collections.ArrayList destCC = ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza;
                            if (destCC != null && destCC.Count > 0)
                            {
                                mittDest = mittDest + " \\par ";
                                for (int i = 0; i < destCC.Count; i++)
                                {
                                    mittDest = mittDest + ((DocsPaVO.utente.Corrispondente)destCC[i]).descrizione;
                                    if (i < (destCC.Count - 1))
                                    {
                                        mittDest = mittDest + " \\par ";
                                    }
                                }
                            }
                        }
                        else
                            if (mittDest_Indirizzo.ToUpper() == "TRUE")
                            mittDest = doc.GetIndirizzoMittDest(schedaDoc.systemId, schedaDoc.tipoProto);
						
					}

					report=report.Replace("XMITT/DEST",mittDest);
					report=report.Replace("XPROT_MITT",protMitt);
					
					//classifica
					DocsPaVO.documento.InfoDocumento infoDoc=new DocsPaVO.documento.InfoDocumento(schedaDoc);
                    System.Collections.ArrayList fascicoli = BusinessLogic.Fascicoli.FascicoloManager.getFascicoliDaDoc(infoUtente, infoDoc.idProfile);
                    string classifica = "";
                    string codice = "";
                    for (int i = 0; i < fascicoli.Count; i++)
                    {
                        //questo serve in caso nel codice fascicolo sia utilizzato come separatore il back slash "\", solo così è stampato nel file rtf...
                        codice = ((DocsPaVO.fascicolazione.Fascicolo)fascicoli[i]).codice.Replace(@"\", @"{\rtlch\fcs1 \af0\afs20 \ltrch\fcs0 \fs20\lang1040\langfe1033\langnp1040\insrsid4612691 \\}");
                        classifica = classifica + " " + codice;
                        if (i < (fascicoli.Count - 1))
                        {
                            classifica = classifica + " \\par";
                        }
                    }									
					report=report.Replace("XCLASSIFICA",classifica);
					
					report=report.Replace("XRISPOSTA","");
					report=report.Replace("XOGGETTO",schedaDoc.oggetto.descrizione);
					report=report.Replace("XNOTE", BusinessLogic.Note.NoteManager.GetUltimaNotaAsString(infoUtente, new DocsPaVO.Note.AssociazioneNota(DocsPaVO.Note.AssociazioneNota.OggettiAssociazioniNotaEnum.Documento, schedaDoc.systemId)));
                    
					if (existUffRef)
						report = report.Replace("XUFFREF", uffRef);
											
					result.content=ReportUtils.toByteArray(report);					
					result.length=result.content.Length;
					result.contentType="application/rtf";
					result.name="report.rtf";
					logger.Debug("fileDocumento generato");
				}
				catch(Exception e)
				{
				
					logger.Debug("Errore nella gestione di Report (getSchedaDocReport)",e);
					throw e;
				}
			}

			return result;
		}

		private static bool ExistUfficioReferente(DocsPaVO.documento.SchedaDocumento schedaDoc)
		{
			bool retValue = false;
			
			if(schedaDoc.protocollo.GetType().Equals(typeof(DocsPaVO.documento.ProtocolloEntrata)))
			{
				if(((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).ufficioReferente!=null)
					retValue = true;
			}
			else
			{
				if (((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).ufficioReferente != null)
					retValue = true;
			}			
			
			return retValue;
		}

		private static DocsPaVO.documento.FileDocumento GetStampaProfiloRTF(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDocumento)
		{
			logger.Debug("GetStampaProfiloRTF");

			string reportData=ReportUtils.stringFile(AppDomain.CurrentDomain.BaseDirectory + @"\report\schedaDoc\schedaProfiloDoc.txt");
			reportData=reportData.Replace("XIDDOC",schedaDocumento.docNumber);
			reportData=reportData.Replace("XDTACREAZIONE",schedaDocumento.dataCreazione);
			reportData=reportData.Replace("XOGG",schedaDocumento.oggetto.descrizione);
			
			string paroleChiavi=string.Empty;
			for (int i=0;i<schedaDocumento.paroleChiave.Count;i++)
			{
				if (paroleChiavi!=string.Empty)
					paroleChiavi += "; ";

				paroleChiavi += ((DocsPaVO.documento.ParolaChiave) schedaDocumento.paroleChiave[i]).descrizione;
			}
            
			reportData=reportData.Replace("XPAROLECHIAVI", paroleChiavi);
            reportData = reportData.Replace("XNOTE", BusinessLogic.Note.NoteManager.GetUltimaNotaAsString(infoUtente, new DocsPaVO.Note.AssociazioneNota(DocsPaVO.Note.AssociazioneNota.OggettiAssociazioniNotaEnum.Documento, schedaDocumento.systemId)));

			string tipoAtto=string.Empty;
			if (schedaDocumento.tipologiaAtto!=null)
				tipoAtto=schedaDocumento.tipologiaAtto.descrizione;

			reportData=reportData.Replace("XTIPODOC",tipoAtto);

			DocsPaVO.documento.FileDocumento retValue=new DocsPaVO.documento.FileDocumento();

			retValue.content=ReportUtils.toByteArray(reportData);
			retValue.length=retValue.content.Length;
			retValue.contentType="application/rtf";
			retValue.name="report.rtf";

			return retValue;
		}

		/// <summary></summary>
		/// <param name="schedaDoc"></param>
		/// <returns></returns>
		public static DocsPaVO.documento.FileDocumento getBustaReport(DocsPaVO.documento.SchedaDocumento schedaDoc)
		{
			DocsPaVO.documento.FileDocumento result=new DocsPaVO.documento.FileDocumento();
			//string path = ReportUtils.getPathName();
			//string path = DocsPaUtils.Functions.Functions.GetReportsPath();
			string path = AppDomain.CurrentDomain.BaseDirectory + "report";
			
			try
			{
				logger.Debug("Generazione report");  
				string report=ReportUtils.stringFile(path+"\\busta\\headerBusta.txt");
				System.Collections.ArrayList dest=((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari;
				
				for(int i=0;i<((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari.Count;i++)
				{
					string body=ReportUtils.stringFile(path+"\\busta\\bodyBusta.txt");
					logger.Debug("Sostituzione campi destinatario "+i);
					body=body.Replace("XNOME_REG",schedaDoc.registro.descrizione);
					logger.Debug("Ricerca dettagli destinatario "+i);
					DocsPaVO.addressbook.QueryCorrispondente qco=new DocsPaVO.addressbook.QueryCorrispondente();
					qco.systemId=((DocsPaVO.utente.Corrispondente) dest[i]).systemId;
					DocsPaVO.addressbook.DettagliCorrispondente dettDest=BusinessLogic.Utenti.addressBookManager.dettagliCorrispondenteMethod(qco);
					body=body.Replace("XNOME_DEST",((DocsPaVO.utente.Corrispondente) dest[i]).descrizione);
					body=body.Replace("XINDIRIZZO_DEST",dettDest.Corrispondente[0].indirizzo);
					string citta="";
					if(dettDest.Corrispondente[0].cap!=null)
					{
						citta=citta+dettDest.Corrispondente[0].cap+"   ";
					};
					if(dettDest.Corrispondente[0].citta!=null)
					{
						citta=citta+dettDest.Corrispondente[0].citta+"   ";
					};
					if(dettDest.Corrispondente[0].nazione!=null)
					{
						citta=citta+"("+dettDest.Corrispondente[0].nazione+")";
					};
					body=body.Replace("XCITTA_DEST",citta);
					if(i!=0)
					{
						body=body.Replace("XNEW_PAGE","{\\b0\\lang1040\\langfe1033\\langnp1040 \\page }");
					}
					else
					{
						body=body.Replace("XNEW_PAGE","");
					}
					report=report+body;
				}

				string bottom=ReportUtils.stringFile(path+"\\busta\\bottomBusta.txt");
				report=report+bottom;
				logger.Debug("Generazione fileDocumento");
				logger.Debug("Creazione content");
				result.content=ReportUtils.toByteArray(report);
				logger.Debug("Content creato");
				result.length=result.content.Length;
				result.contentType="application/rtf";
				result.name="report.rtf";
				logger.Debug("fileDocumento generato");
			}
			catch(Exception e)
			{
				logger.Debug("Errore nella gestione di Report (getBustaReport)",e);
				throw e;
			}
			return result;
		}

        /// <summary></summary>
        /// <param name="schedaDoc"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.FileDocumento getRicevutaProtocolloRtf(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDoc)
        {
            DocsPaVO.documento.FileDocumento result = new DocsPaVO.documento.FileDocumento();
            string path = AppDomain.CurrentDomain.BaseDirectory + "report";

            try
            {
                logger.Debug("Generazione report");
                //string report = ReportUtils.stringFile(path + "\\ricevuta\\ricevuta.txt");
                string modelRootPath = System.Configuration.ConfigurationManager.AppSettings["MODELS_ROOT_PATH"];

                string report = ReportUtils.stringFile(string.Concat(modelRootPath, string.Format(@"\Modelli\{0}\Ricevute\{1}\Ricevuta.{2}", schedaDoc.registro.codAmministrazione, schedaDoc.registro.codRegistro, "rtf")));

                report = Regex.Replace(report, "#Amministrazione#", BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoUtente.idAmministrazione).Descrizione, RegexOptions.IgnoreCase);
                report = Regex.Replace(report, "#Data ora protocollo#", BusinessLogic.Documenti.ProtoManager.getDataOraProtocollo(schedaDoc.docNumber), RegexOptions.IgnoreCase);
                report = Regex.Replace(report, "#Data protocollo#", schedaDoc.protocollo.dataProtocollazione, RegexOptions.IgnoreCase);
                report = Regex.Replace(report, "#Numero protocollo#", schedaDoc.protocollo.numero, RegexOptions.IgnoreCase);
                report = Regex.Replace(report, "#Segnatura#", schedaDoc.protocollo.segnatura, RegexOptions.IgnoreCase);
                report = Regex.Replace(report, "#Oggetto#", schedaDoc.oggetto.descrizione.Replace("\n", @" \par "), RegexOptions.IgnoreCase);

                if (schedaDoc.protocollatore != null)
                {
                    Corrispondente corr = new Corrispondente();
                    DocsPaDB.Query_DocsPAWS.Utenti obj = new DocsPaDB.Query_DocsPAWS.Utenti();
                    // PROTOCOLLATORE						
                    if (schedaDoc.protocollatore.utente_idPeople != null && schedaDoc.protocollatore.utente_idPeople != string.Empty)
                    {
                        corr = obj.GetCorrispondenteBySystemID(obj.GetIDUtCorr(schedaDoc.protocollatore.utente_idPeople));
                        report = Regex.Replace(report, "#Protocollatore#", corr.descrizione, RegexOptions.IgnoreCase);
                    }

                    // RUOLO PROTOCOLLATORE					
                    if (schedaDoc.protocollatore.ruolo_idCorrGlobali != null && schedaDoc.protocollatore.ruolo_idCorrGlobali != string.Empty)
                    {
                        corr = obj.GetCorrispondenteBySystemID(schedaDoc.protocollatore.ruolo_idCorrGlobali);
                        report = Regex.Replace(report, "#Ruolo protocollatore#", corr.descrizione, RegexOptions.IgnoreCase);
                    }

                    // UO PROTOCOLLATORE					
                    if (schedaDoc.protocollatore.uo_idCorrGlobali != null && schedaDoc.protocollatore.uo_idCorrGlobali != string.Empty)
                    {
                        //descrizione
                        corr = obj.GetCorrispondenteBySystemID(schedaDoc.protocollatore.uo_idCorrGlobali);
                        report = Regex.Replace(report, "#Uo protocollatore#", corr.descrizione, RegexOptions.IgnoreCase);
                    }
                }

                // NOTE 
                // Reperimento dell'ultima nota visibile a tutti
                string testoNote = string.Empty;

                foreach (DocsPaVO.Note.InfoNota nota in BusinessLogic.Note.NoteManager.GetNote(infoUtente, new DocsPaVO.Note.AssociazioneNota(DocsPaVO.Note.AssociazioneNota.OggettiAssociazioniNotaEnum.Documento, schedaDoc.systemId), null))
                {
                    if (nota.TipoVisibilita == DocsPaVO.Note.TipiVisibilitaNotaEnum.Tutti)
                    {
                        testoNote = nota.Testo;
                        break;
                    }
                }
                          
                report = Regex.Replace(report, "#Note#", testoNote, RegexOptions.IgnoreCase);          


                //report custom pat momentaneo
                if (schedaDoc.creatoreDocumento != null && !string.IsNullOrEmpty(schedaDoc.creatoreDocumento.idCorrGlob_UO))
                {

                    Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemIDDisabled(schedaDoc.creatoreDocumento.idCorrGlob_UO);
                    report = Regex.Replace(report, "#Uo creatore#", corr.descrizione, RegexOptions.IgnoreCase);
                }
                else
                {
                    report = Regex.Replace(report, "#Uo creatore#", "", RegexOptions.IgnoreCase);
                }

                if (schedaDoc.protocollo.GetType() == typeof(DocsPaVO.documento.ProtocolloEntrata))
                {
                    report = Regex.Replace(report, "#Mittente#", ((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).mittente.descrizione, RegexOptions.IgnoreCase);
                }
                else if (schedaDoc.protocollo.GetType() == typeof(DocsPaVO.documento.ProtocolloUscita))
                {
                    report = Regex.Replace(report, "#Mittente#", ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).mittente.descrizione, RegexOptions.IgnoreCase);
                }
                else if (schedaDoc.protocollo.GetType() == typeof(DocsPaVO.documento.ProtocolloInterno))
                {
                    report = Regex.Replace(report, "#Mittente#", ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).mittente.descrizione, RegexOptions.IgnoreCase);
                }

                report = Regex.Replace(report, "#Numero Allegati#", schedaDoc.allegati.Count.ToString(), RegexOptions.IgnoreCase);

                logger.Debug("Generazione fileDocumento");
                logger.Debug("Creazione content");
                result.content = ReportUtils.toByteArray(report);
                logger.Debug("Content creato");
                result.length = result.content.Length;
                result.contentType = "application/rtf";
                result.name = "ricevuta.rtf";
                logger.Debug("fileDocumento generato");
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella gestione di Report (getRicevutaProtocolloRtf)", e);
                throw e;
            }
            return result;
        }

	}
}
