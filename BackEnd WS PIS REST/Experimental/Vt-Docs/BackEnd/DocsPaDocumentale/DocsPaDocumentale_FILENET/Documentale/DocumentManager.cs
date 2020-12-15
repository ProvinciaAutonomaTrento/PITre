using System;
using System.IO;
using System.Collections;
using System.Configuration;
using DocsPaVO.utente;
using DocsPaDocumentale_FILENET.FilenetLib;
using DocsPaDocumentale.Interfaces;
using DocsPaUtils.Functions;
using log4net;

namespace DocsPaDocumentale_FILENET.Documentale
{
	/// <summary>
	/// Classe per la gestione di un documento tramite il documentale ETDoc
	/// </summary>
	public class DocumentManager : IDocumentManager
	{
        private ILog logger = LogManager.GetLogger(typeof(DocumentManager));
		protected DocsPaVO.utente.InfoUtente userInfo;
		protected string v_e_name;
		
		#region Costruttori
		/// <summary>
		/// </summary>
		protected DocumentManager()
		{
		}

		/// <summary>
		/// Inizializza l'istanza della classe acquisendo i dati relativi all'utente 
		/// ed alla libreria per la connessione al documentale.
		/// </summary>
		/// <param name="infoUtente">Dati relativi all'utente</param>
		public DocumentManager(DocsPaVO.utente.InfoUtente infoUtente)
		{
			userInfo = infoUtente;
		}
        
		#endregion

		#region Metodi

        /// <summary>
        /// Creazione di un nuovo documento per la stampa registro.
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public bool CreateDocumentoStampaRegistro(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo)
        {
            DocsPaVO.utente.Ruolo[] ruoliSuperiori;
            return this.CreateDocumentoStampaRegistro(schedaDocumento, ruolo, out ruoliSuperiori);
        }

        /// <summary>
        /// Creazione di un nuovo documento per la stampa registro.
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <param name="ruoliSuperiori"></param>
        /// <returns>ID del documento o 'null' se si è verificato un errore</returns>
        public bool CreateDocumentoStampaRegistro(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.utente.Ruolo[] ruoliSuperiori)
        {
            bool retValue = false;
            ruoliSuperiori = null;

			string dst=userInfo.dst;
		
			try
			{
				schedaDocumento.docNumber = FilenetLib.DocumentManagement.CreateDocument(schedaDocumento, Int32.Parse(this.userInfo.idAmministrazione), this.userInfo.dst);
				
                if (string.IsNullOrEmpty(schedaDocumento.docNumber)) 
                    throw new Exception("Errore nella creazione di un documento in Filenet");

                schedaDocumento.docNumber = this.CreateDocument(schedaDocumento);
                schedaDocumento.systemId = schedaDocumento.docNumber;

                retValue = (!string.IsNullOrEmpty(schedaDocumento.systemId));

                if (retValue)
                {
                    using (DocsPaDB.Query_DocsPAWS.Report dbReport = new DocsPaDB.Query_DocsPAWS.Report())
                        dbReport.UpdProf(schedaDocumento, schedaDocumento.registro.systemId, schedaDocumento.docNumber);

                    using (DocsPaDB.Query_DocsPAWS.Documenti dbDocument = new DocsPaDB.Query_DocsPAWS.Documenti())
                        dbDocument.GetProfile(this.userInfo, ref schedaDocumento);

                    // Impostazione visibilità documento ai ruoli superiori al ruolo corrente
                    using (DocsPaDB.Query_DocsPAWS.Documenti dbDocumenti = new DocsPaDB.Query_DocsPAWS.Documenti())
                        schedaDocumento = dbDocumenti.SetDocTrustees(schedaDocumento, ruolo, out ruoliSuperiori, null);
                }
			}
			catch(Exception exception)
			{
                retValue = false;

				logger.Debug("Errore nella creazione di un documento.", exception);
			}

            return retValue;
		}

        ///// <summary>
        ///// Distrugge il documento dell'istanza della classe precedentemente creato.
        ///// </summary>
        ///// <returns></returns>
		
        //public bool DestroyNewDocument()
        //{
        //    return true;
        //}

        //public bool DestroyNewDocument(string docnumber, string idamministrazione)
        //{
        //    bool result;
        //    try
        //    {
        //        UserManager userManager = new UserManager();
        //        string dst=userInfo.dst;
        //        result = FilenetLib.DocumentManagement.RemoveDocument(docnumber, idamministrazione);
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Debug("Errore nella rimozione del documento in Filenet", e);
        //        return false;
        //    }
        //}

		/// <summary>
		/// Acquisisce un documento
		/// </summary>
		/// <param name="docNumber"></param>
		/// <param name="version"></param>
		/// <param name="versionId"></param>
		/// <param name="versionLabel"></param>
		/// <returns>
		/// Documento in formato binario o 'null' se si è verificato un errore.
		/// </returns>



        public byte[] GetFile(string docNumber, string version, string versionId, string versionLabel)
			
		{		
			byte[] fileCont = null;
			
			try
			{
				DocsPaVO.documento.FileRequest fileRequest = new DocsPaVO.documento.FileRequest();
				DocsPaVO.documento.FileDocumento fileDocumento = new DocsPaVO.documento.FileDocumento();

				fileRequest.versionId = versionId;
				this.GetFile(ref fileDocumento, ref fileRequest);

				fileCont = fileDocumento.content;

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'acquisizione di un documento.", exception);

				fileCont = null;
			}

			return fileCont;

			
		}

		#region Metodo Commentato
		/// <summary>
		/// Ritorna un oggetto di ricerca.
		/// </summary>
		/// <param name="docNumber"></param>
		/// <returns>
		/// Oggetto di ricerca o 'null' se si è verificato un errore.
		/// </returns>
		//		private DocsPaDocumentale.HummingbirdLib.Ricerca GetSearcObject(string docNumber) 
		//		{
		//			DocsPaDocumentale.HummingbirdLib.Ricerca ricerca = new DocsPaDocumentale.HummingbirdLib.Ricerca(this.userInfo.dst, this.library, DocsPaDocumentale.HummingbirdLib.Tipi.ObjectType.Cyd_CmnVersions);
		//
		//			// Return the version id.
		//			ricerca.AddReturnProperty(DocsPaDocumentale.HummingbirdLib.Tipi.SearchReturnPropertyType.VersionId);
		//			ricerca.AddReturnProperty(DocsPaDocumentale.HummingbirdLib.Tipi.SearchReturnPropertyType.Version);
		//			ricerca.AddReturnProperty(DocsPaDocumentale.HummingbirdLib.Tipi.SearchReturnPropertyType.SubVersion);
		//			ricerca.AddReturnProperty(DocsPaDocumentale.HummingbirdLib.Tipi.SearchReturnPropertyType.VersionLabel);
		//
		//			// Constrain the search to the docnumber that was passed in.
		//			ricerca.AddSearchCriteria(DocsPaDocumentale.HummingbirdLib.Tipi.SearchCriteriaType.DocumentNumber, docNumber);
		//
		//			// Sort by VERSION and SUBVERSION.
		//			ricerca.AddOrderByProperty(DocsPaDocumentale.HummingbirdLib.Tipi.OrderByPropertyType.Version, false);
		//			ricerca.AddOrderByProperty(DocsPaDocumentale.HummingbirdLib.Tipi.OrderByPropertyType.SubVersion, false);
		//
		//			// Execute the search.
		//			ricerca.Execute();
		//
		//			if(ricerca.GetErrorCode() != 0)
		//			{
		//				logger.Debug("ERROR_CHECKINDOCACT_LATESTVERSIONEXECUTE_FORLOCK");
		//				ricerca = null;
		//			}
		//
		//			// Go to the first row of the results, and save the version id.
		//			ricerca.NextRow();
		//
		//			if(ricerca.GetErrorCode() != 0)
		//			{
		//				logger.Debug("ERROR_CHECKINDOCACT_LATESTVERSIONNEXTROW_FORLOCK");
		//				ricerca = null;
		//			}
		//
		//			return ricerca;
		//
		//			#region Codice Originale
		//			//			//string library = DocsPaDB.Utils.Personalization.getInstance(objSicurezza.idAmministrazione).getLibrary();
		//			//				
		//			//			/*
		//			//			
		//			//			
		//			//			// Create a search object.
		//			//			PCDCLIENTLib.PCDSearch pLatestVersion = new PCDCLIENTLib.PCDSearch();
		//			//
		//			//			// Set the Security Token.
		//			//			pLatestVersion.SetDST(objSicurezza.dst);
		//			//
		//			//			// Add the library that was passed in.
		//			//			pLatestVersion.AddSearchLib(library);
		//			//
		//			//			// Use the versions form to access properties.
		//			//			pLatestVersion.SetSearchObject("cyd_cmnversions");
		//			//			*/
		//			//			DocsPaDocumentale.HummingbirdLib.Ricerca ricerca = new DocsPaDocumentale.HummingbirdLib.Ricerca(this.userInfo.dst, library, DocsPaDocumentale.HummingbirdLib.Tipi.ObjectType.Cyd_CmnVersions);
		//			//
		//			//			// Return the version id.
		//			//			ricerca.AddReturnProperty(DocsPaDocumentale.HummingbirdLib.Tipi.SearchReturnPropertyType.VersionId);
		//			//			ricerca.AddReturnProperty(DocsPaDocumentale.HummingbirdLib.Tipi.SearchReturnPropertyType.Version);
		//			//			ricerca.AddReturnProperty(DocsPaDocumentale.HummingbirdLib.Tipi.SearchReturnPropertyType.SubVersion);
		//			//			ricerca.AddReturnProperty(DocsPaDocumentale.HummingbirdLib.Tipi.SearchReturnPropertyType.VersionLabel);
		//			//
		//			//			// Constrain the search to the docnumber that was passed in.
		//			//			ricerca.AddSearchCriteria(DocsPaDocumentale.HummingbirdLib.Tipi.SearchCriteriaType.DocumentNumber, docNumber);
		//			//
		//			//			// Sort by VERSION and SUBVERSION.
		//			//			ricerca.AddOrderByProperty(DocsPaDocumentale.HummingbirdLib.Tipi.OrderByPropertyType.Version, false);
		//			//			ricerca.AddOrderByProperty(DocsPaDocumentale.HummingbirdLib.Tipi.OrderByPropertyType.SubVersion, false);
		//			//
		//			//			// Execute the search.
		//			//			ricerca.Execute();
		//			//
		//			//			/*
		//			//			DocsPaWS.Utils.ErrorHandler.checkPCDOperation(pLatestVersion, "ERROR_CHECKINDOCACT_LATESTVERSIONEXECUTE_FORLOCK");
		//			//			*/
		//			//			if(ricerca.GetErrorCode() != 0)
		//			//			{
		//			//				//throw new Exception("ERROR_CHECKINDOCACT_LATESTVERSIONEXECUTE_FORLOCK");
		//			//				
		//			//				ricerca = null;
		//			//			}
		//			//
		//			//			// Go to the first row of the results, and save the version id.
		//			//			ricerca.NextRow();
		//			//
		//			//			/*
		//			//			DocsPaWS.Utils.ErrorHandler.checkPCDOperation(pLatestVersion, "ERROR_CHECKINDOCACT_LATESTVERSIONNEXTROW_FORLOCK");
		//			//			*/
		//			//			if(ricerca.GetErrorCode() != 0)
		//			//			{
		//			//				//throw new Exception("ERROR_CHECKINDOCACT_LATESTVERSIONNEXTROW_FORLOCK");
		//			//				
		//			//				ricerca = null;
		//			//			}
		//			//
		//			//			return ricerca;
		//			#endregion
		//		}
		#endregion

		/// <summary>
		/// Crea un nuovo documento
		/// </summary>
		/// <param name="schedaDoc"></param>
		/// <returns></returns>
		private string CreateDocument(DocsPaVO.documento.SchedaDocumento schedaDoc) 
		{
			string docNumber = null;

			try
			{
				//INSERT su Components, Profile, Security e Versions
				DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
				docNumber = documentale.CreateDocument(schedaDoc);

				logger.Debug("documento creato");	
				
				#region Codice Commentato
				//			if(documento.GetErrorCode() != 0)
				//			{
				//				throw new Exception("Errore nell'update del documento");
				//			}
				//
				//			documento.Status = DocsPaDocumentale.HummingbirdLib.Tipi.StatusType.Lock;
				//			documento.Update();	
				//	
				//			if(documento.GetErrorCode() != 0)
				//			{
				//				throw new Exception("Errore nell'update del documento");
				//			}
				#endregion
		
				if(docNumber != null)
				{
					//aggiorna flag daInviare
					string firstParam = "CHA_DA_INVIARE = '1'";
				
					if(schedaDoc.documenti != null && schedaDoc.documenti.Count > 0)
					{
						logger.Debug("Documenti presenti");
						int lastDocNum = schedaDoc.documenti.Count - 1;
					
						if(((DocsPaVO.documento.Documento)schedaDoc.documenti[lastDocNum]).dataArrivo!=null && !((DocsPaVO.documento.Documento)schedaDoc.documenti[lastDocNum]).dataArrivo.Equals(""))
						{
							firstParam += ", DTA_ARRIVO =" + DocsPaDbManagement.Functions.Functions.ToDate(((DocsPaVO.documento.Documento)schedaDoc.documenti[lastDocNum]).dataArrivo);
						}
					}
				
					DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
					doc.UpdateVersions(firstParam, docNumber);
					doc.F_UpdateVersions(this.v_e_name, schedaDoc.docNumber, "1");
					
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la creazione di un documento.", exception);

				docNumber = null;
			}

			return docNumber; 
		}

        /// <summary>
        /// Reperimento versionlabel per l'allegato
        /// </summary>
        /// <param name="docnumber"></param>
        /// <returns></returns>
        protected virtual string GetAllegatoVersionLabel(string docnumber)
        {
            DocsPaDB.Query_DocsPAWS.Documenti obj = new DocsPaDB.Query_DocsPAWS.Documenti();
            return obj.GetVersionLabelAllegato(docnumber, false);
        }

		/// <summary>
		/// </summary>
		/// <param name="allegato"></param>
		/// <returns></returns>
		/// <summary>
		/// </summary>
		/// <param name="allegato"></param>
		/// <returns></returns>
        public bool AddAttachment(DocsPaVO.documento.Allegato allegato, string putfile)
		{
			bool result = true; // Presume successo
			string old_verLabel=allegato.versionLabel;
			System.Data.DataSet ds;
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

			try 
			{
                if (putfile != null && putfile == "Y")
                {
                    if (allegato.versionLabel != null && !allegato.versionLabel.Equals(""))
                    {
                        doc.UpdateVersion(allegato.versionId);
                    }
                }
                else
                {
                    // Reperimento VersionLabel per l'allegato
                    allegato.versionLabel = this.GetAllegatoVersionLabel(allegato.docNumber);
                }

				DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
				documentale.AddAttachment(ref allegato, this.userInfo.idPeople, this.userInfo.userId);

				// Ricerca filename
				doc.GetPath(out ds, allegato.versionId, allegato.docNumber);

				allegato.fileName = ds.Tables["PATH"].Rows[0]["PATH"].ToString();
				logger.Debug("fn="+allegato.fileName);
				
				doc.UpdateNumPageDescription(allegato.numeroPagine, allegato.versionId, allegato.descrizione);					
			} 
			catch(Exception exception) 
			{
				logger.Debug("Errore durante l'aggiunta di un allegato.", exception);
				if(putfile!=null && putfile=="Y")
				{
					if(allegato!=null && allegato.versionLabel != null && ! allegato.versionLabel.Equals(""))	
					{
						doc.RollBackUpdateVersion(allegato.versionId,old_verLabel);

					}
				}
				result = false;
			}

            return result;
		}

        /// <summary>
        /// Modifica di un allegato
        /// </summary>
        /// <param name="allegato"></param>
        public void ModifyAttatchment(DocsPaVO.documento.Allegato allegato)
        {
            DocsPaDB.Query_DocsPAWS.Documenti obj = new DocsPaDB.Query_DocsPAWS.Documenti();
            obj.ModificaAllegato(allegato, this.userInfo, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public bool CreateDocumentoGrigio(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo)
        {
            DocsPaVO.utente.Ruolo[] ruoliSuperiori;
            return this.CreateDocumentoGrigio(schedaDocumento, ruolo, out ruoliSuperiori);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <param name="sede"></param>
        /// <param name="risultatoProtocollazione"></param>
        /// <returns></returns>
        public bool CreateProtocollo(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione)
        {
            DocsPaVO.utente.Ruolo[] ruoliSuperiori;
            return this.CreateProtocollo(schedaDocumento, ruolo, out risultatoProtocollazione, out ruoliSuperiori);
        }

		/// <summary>
		/// </summary>
		/// <param name="schedaDocumento"></param>
		/// <param name="ruolo"></param>
		/// <returns></returns>
        public bool CreateDocumentoGrigio(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.utente.Ruolo[] ruoliSuperiori, string conCopia = null) 
		{
			bool result = true; // Presume successo
			string docNumber = null;
            ruoliSuperiori = null;
			
			try
			{
				schedaDocumento.docNumber = FilenetLib.DocumentManagement.CreateDocument(schedaDocumento, Int32.Parse(this.userInfo.idAmministrazione),this.userInfo.dst);	
				if (schedaDocumento.docNumber==null) throw new Exception("Errore nella creazione di un documento in Filenet");

				// creo il nuovo documento
				this.v_e_name= "0001 00";
				docNumber = this.CreateDocument(schedaDocumento);	

				DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();

                if (!documenti.AddDocGrigia(this.userInfo.idAmministrazione, ref schedaDocumento, this.userInfo, ruolo, out ruoliSuperiori))
				{
					DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
					documentale.DeleteDocument(schedaDocumento.docNumber);

					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la creazione di un documento grigio.", exception);
				result = false;
			}
			
			return result;
		}

        /// <summary>
        /// Predisposizione di un documento alla protocollazione
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public bool PredisponiProtocollazione(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.PredisponiAllaProtocollazione(userInfo,
                                                    ref schedaDocumento,
                                                    this.userInfo.sede);
        }

        /// <summary>
        /// Protocollazione di un documento predisposto
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <param name="risultatoProtocollazione"></param>
        /// <returns></returns>
        public bool ProtocollaDocumentoPredisposto(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione)
        {
            risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.OK;
            bool retValue = this.PredisponiProtocollazione(schedaDocumento);

            // Impostazione utente protocollatore
            schedaDocumento.protocollatore = new DocsPaVO.documento.Protocollatore(this.userInfo, ruolo);

            try
            {
                DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                doc.ProtocollaDocProntoProtocollazione_SEM(this.userInfo, ruolo, ref schedaDocumento);

                if (schedaDocumento.documenti != null && schedaDocumento.documenti.Count > 0)
                {
                    string firstParam = "";

                    logger.Debug("Documenti presenti");
                    int lastDocNum = schedaDocumento.documenti.Count - 1;

                    if (((DocsPaVO.documento.Documento)schedaDocumento.documenti[lastDocNum]).dataArrivo != null && !((DocsPaVO.documento.Documento)schedaDocumento.documenti[lastDocNum]).dataArrivo.Equals(""))
                    {
                        firstParam += "DTA_ARRIVO =" + DocsPaDbManagement.Functions.Functions.ToDate(((DocsPaVO.documento.Documento)schedaDocumento.documenti[lastDocNum]).dataArrivo);
                        doc.UpdateVersions(firstParam, schedaDocumento.docNumber);
                    }
                }
            }
            catch (Exception ex)
            {
                risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.APPLICATION_ERROR;

                string msg = string.Format("Errore nella protocollazione del documento: {0}", ex.Message);
                logger.Debug(msg);
                throw new ApplicationException(msg, ex);
            }

            return retValue;
        }

        /// <summary>
        /// Save delle modifiche apportate al documento
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ufficioReferenteEnabled"></param>
        /// <param name="ufficioReferenteSaved"></param>
        /// <returns></returns>
        public bool SalvaDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento, bool ufficioReferenteEnabled, out bool ufficioReferenteSaved)
        {
            ufficioReferenteSaved = false;
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.SalvaModifiche(this.userInfo, ufficioReferenteEnabled, out ufficioReferenteEnabled, ref schedaDocumento);
        }

        /// <summary>
        /// Rimozione di un allegato
        /// </summary>
        /// <param name="allegato"></param>
        /// <returns></returns>
        public bool RemoveAttatchment(DocsPaVO.documento.Allegato allegato)
        {
            return this.RemoveVersion(allegato);
        }

		/// <summary>
		/// </summary>
		/// <param name="fileRequest"></param>
		/// <returns></returns>
		public bool RemoveVersion(DocsPaVO.documento.FileRequest fileRequest)
		{
			bool result=false;
			string dst=userInfo.dst;
			
			try
			{
				FilenetLib.Versione versFilenet=new Versione();
				if (versFilenet.VersioneIsAcquisito(fileRequest.docNumber, fileRequest.versionId))
					versFilenet.remove(fileRequest, this.userInfo.idAmministrazione,fileRequest.fNversionId, dst);
			}
			catch(Exception e)
			{
				logger.Debug("Errore durante la rimozione della versione", e);
				throw e;
			}
		

			try
			{
				DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
				documentale.RemoveVersion(fileRequest.docNumber, fileRequest.versionId);
				result=true;
			}
			catch(Exception exception) 
			{		
				logger.Debug("Errore durante la cancellazione di una versione.", exception);
				result = false;
			}

			return result;

		}

		/// <summary>
		/// </summary>
		/// <param name="fileDocumento"></param>
		/// <param name="fileRequest"></param>
		/// <returns></returns>
		public bool GetFile(ref DocsPaVO.documento.FileDocumento fileDoc, ref DocsPaVO.documento.FileRequest fileRequest)
		{
			bool result=false;
			string copyPath = "";
			string dst=userInfo.dst;
				
			try 
			{
				string versionID="";
				versionID=GetFileNetVersionID(fileRequest.docNumber, fileRequest.versionId);

				string docnumber = Versione.setDocnumber2Filenet(fileRequest.docNumber, versionID);

				IDMObjects.IFnDocumentDual verFNET = FilenetLib.DocumentManagement.getDocument(docnumber, 
					this.userInfo.idAmministrazione, dst);
			
				string path = ConfigurationManager.AppSettings["FNET_pathServer"];
				string fileName = "";
				verFNET.Version.Copy( out copyPath, path, fileName, IDMObjects.idmCopy.idmCopyOverwrite);
				//perchè fare indicizazzione di un file in visualizzazione, basta farla in memorizzazione 16_06_2006
				//				try
				//				{
				//					bool canIndex = verFNET.GetState(IDMObjects.idmDocState.idmDocCanIndex);
				//					if (canIndex)
				//					verFNET.Version.IndexContent(IDMObjects.idmIndex.idmIndexContent);
				//			
				//				}
				//				catch
				//				{ DocsPaUtils.LogsManagement.logger.Debug("Errore nell'indicizzazione del file.");}				
				string fileExt = Functions.getExt(copyPath);
				fileDoc = Functions.read(copyPath);
				DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
				ArrayList apps = documenti.GetApplicazioni(fileExt, new ArrayList());
				if (apps != null && apps.Count > 0)
				{
					fileDoc.contentType=((DocsPaVO.documento.Applicazione) apps[0]).mimeType;
					fileDoc.estensioneFile=fileExt;
					File.Delete(copyPath);
				}
                //le info gia ci sono  (A.B.)
                //fileRequest.versionId=GetVersionID(fileRequest.docNumber, fileRequest.version);
				result=true;
			} 
			catch(Exception exception)
			{				
				//cancello il file temporaneo
				if(copyPath != null && copyPath.Length > 0)
					File.Delete(copyPath);
				logger.Debug("Errore durante la lettura di un documento.", exception);
				throw exception;
			}

			return result;
		}

		/// <summary>
		/// Metodo che restituisce la directory dove salvare il documento, esclusa la doc root. Verifica che la Doc Root
		/// esista, nel caso contrario restituisce null
		/// </summary>
		/// <param name="fileRequest"></param>
		/// <returns></returns>
		string GetDocPath(DocsPaVO.documento.FileRequest fileRequest,DocsPaVO.utente.InfoUtente objSicurezza)
		{
			string result=null;
			try
			{
				if(fileRequest!=null)
				{
					string filePath="";
					
					//legge il record da DPA_CORR_GLOBALI in JOIN con DPA_AMMINISTRA
					DocsPaDB.Query_DocsPAWS.Documentale documentale=new DocsPaDB.Query_DocsPAWS.Documentale();
					System.Data.DataSet corrispondente;
					if(!documentale.DOC_GetCorrByIdPeople(fileRequest.idPeople,out corrispondente))
					{
						logger.Debug ("Errore nella lettura del corrispondente relativo al documento");
						throw new Exception();
					}
					//legge l'amministrazione
					string amministrazione=corrispondente.Tables[0].Rows[0]["VAR_CODICE_AMM"].ToString();
					//legge l'id della uo di appartenenza del gruppo
					string id=documentale.DOC_GetIdUoBySystemId(objSicurezza.idGruppo);
					if(id==null)
					{
						logger.Debug ("Errore nella lettura del gruppo relativo al documento");
						throw new Exception();
					}
					//recupera il nome della UO
					string codiceUO=documentale.DOC_GetUoById(id);
					//legge la tabella profile
					System.Data.DataSet documento;
					if(!documentale.DOC_GetDocByDocNumber(fileRequest.docNumber,out documento))
					{
						logger.Debug ("Errore nella lettura del documento: " + fileRequest.docNumber);
						throw new Exception();
					}
					//legge l'anno di creazione del documento
					string anno=System.DateTime.Parse(documento.Tables[0].Rows[0]["CREATION_DATE"].ToString()).Year.ToString();
					//verifica se il documento è protocollato
					string tipoProtocollo;
					tipoProtocollo=documento.Tables[0].Rows[0]["CHA_TIPO_PROTO"].ToString().ToUpper();
					//se A -> protocollo in arrivo; se P -> protocollo in partenza ; tutto il resto -> non protocollato
					string registro="";
					if(tipoProtocollo=="A" || tipoProtocollo=="P" || tipoProtocollo=="I")
					{
						//crea il path nel caso di documento protocollato -> AMMINISTRAZIONE + REGISTRO + ANNO + [COD_UO] + [ARRIVO|PARTENZA]

						//legge il registro della protocollazione
						registro=documentale.DOC_GetRegistroById(documento.Tables[0].Rows[0]["ID_REGISTRO"].ToString());
						if(registro==null)
						{
							logger.Debug ("Errore nella lettura del registro");
							throw new Exception();
						}
						/*
							<add key="DOC_CODICE_UFFICIO" value="1"/>  
							<add key="DOC_ARRIVO_PARTENZA" value="1"/>  
						*/
						filePath += amministrazione + "\\" + anno + "\\" + registro;
						if(System.Configuration.ConfigurationManager.AppSettings["DOC_CODICE_UFFICIO"]!=null)
						{
							if(System.Configuration.ConfigurationManager.AppSettings["DOC_CODICE_UFFICIO"]=="1")
							{
								filePath += "\\" + codiceUO;
							}
						}
						if(System.Configuration.ConfigurationManager.AppSettings["DOC_ARRIVO_PARTENZA"]!=null)
						{
							if(System.Configuration.ConfigurationManager.AppSettings["DOC_ARRIVO_PARTENZA"]=="1")
							{
								if(tipoProtocollo=="A")
								{
									filePath += "\\Arrivo";
								}
								if(tipoProtocollo=="P")
								{
									filePath += "\\Partenza";
								}
								if(tipoProtocollo=="I")
								{
									filePath += "\\Interno";
								}
							}
						}
					}
					else
					{
						//crea il path nel caso di documento non protocollato -> AMMINISTRAZIONE + ANNO + [COD_UO]
						filePath += amministrazione + "\\" + anno;
						if(System.Configuration.ConfigurationManager.AppSettings["DOC_CODICE_UFFICIO"]!=null)
						{
							if(System.Configuration.ConfigurationManager.AppSettings["DOC_CODICE_UFFICIO"]=="1")
							{
								filePath += "\\" + codiceUO;
							}
						}
					}

					//verifica se la directory esiste
					DirectoryInfo docFullPath= new DirectoryInfo(System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"] + "\\" + filePath);
					if(!docFullPath.Exists)
					{
						//crea la directory
						docFullPath.Create();
					}
					
					//restituisce la directory
					result=filePath;

				}
			}
			catch(Exception e)
			{
				logger.Debug("Errore creazione path documentale per documento: " + fileRequest.docNumber ,e);
				result=null;
			}
			return result;
		}

		string GetDocPathAdvanced(DocsPaVO.documento.FileRequest fileRequest,DocsPaVO.utente.InfoUtente objSicurezza)
		{
			string result=null;
			try
			{
				if(fileRequest!=null)
				{
					string filePath="";
					
					
					//legge il record da DPA_CORR_GLOBALI in JOIN con DPA_AMMINISTRA
					DocsPaDB.Query_DocsPAWS.Documentale documentale=new DocsPaDB.Query_DocsPAWS.Documentale();
					System.Data.DataSet corrispondente;
					if(documentale.DOC_GetCorrByIdPeople(objSicurezza.idPeople,out corrispondente))
					{
						//logger.Debug ("Errore nella lettura del corrispondente relativo al documento");
						//throw new Exception();
					}
					//legge l'amministrazione
					string amministrazione=corrispondente.Tables[0].Rows[0]["VAR_CODICE_AMM"].ToString();
					
					//legge l'id della uo di appartenenza del gruppo
					string id=documentale.DOC_GetIdUoBySystemId(objSicurezza.idGruppo);
					if(id==null)
					{
						logger.Debug ("Errore nella lettura del gruppo relativo al documento");
						throw new Exception();
					}
					//recupera il nome della UO
					string codiceUO=documentale.DOC_GetUoById(id);
					//legge la tabella profile
					System.Data.DataSet documento;
					if(!documentale.DOC_GetDocByDocNumber(fileRequest.docNumber,out documento))
					{
						logger.Debug ("Errore nella lettura del documento: " + fileRequest.docNumber);
						throw new Exception();
					}
					//legge l'anno di creazione del documento
					string anno=System.DateTime.Parse(documento.Tables[0].Rows[0]["CREATION_DATE"].ToString()).Year.ToString();
					//verifica se il documento è protocollato
					string tipoProtocollo;
					tipoProtocollo=documento.Tables[0].Rows[0]["CHA_TIPO_PROTO"].ToString().ToUpper();
					//se A -> protocollo in arrivo; se P -> protocollo in partenza ; tutto il resto -> non protocollato
					string registro="";
					string arrivoPartenza="";
					if(tipoProtocollo=="A" || tipoProtocollo=="P"  || tipoProtocollo=="I")
					{
						//crea il path nel caso di documento protocollato -> AMMINISTRAZIONE + REGISTRO + ANNO + [COD_UO] + [ARRIVO|PARTENZA]

						//legge il registro della protocollazione
						registro=documentale.DOC_GetRegistroById(documento.Tables[0].Rows[0]["ID_REGISTRO"].ToString());
						if(registro==null)
						{
							logger.Debug ("Errore nella lettura del registro");
							registro="";
						}

						if(tipoProtocollo=="A")
						{
							arrivoPartenza = "Arrivo";
						}
						if(tipoProtocollo=="P")
						{
							arrivoPartenza = "Partenza";
						}
						if(tipoProtocollo=="I")
						{
							arrivoPartenza = "Interno";
						}
					}

					filePath=System.Configuration.ConfigurationManager.AppSettings["DOC_PATH"];
					if(filePath==null) filePath="";

					filePath=filePath.Replace("AMMINISTRAZIONE",amministrazione);
					filePath=filePath.Replace("REGISTRO",registro);
					filePath=filePath.Replace("ANNO",anno);
					filePath=filePath.Replace("ARRIVO_PARTENZA",arrivoPartenza);
					filePath=filePath.Replace("UFFICIO",codiceUO);
					filePath=filePath.Replace("UTENTE",objSicurezza.userId);

					filePath=filePath.Replace(@"\\",@"\");
					if(filePath.EndsWith(@"\"))
					{
						filePath=filePath.Remove(filePath.Length-1,1);
					}

					//verifica se la directory esiste
					string appo=@System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"] + "\\" + filePath;
					DirectoryInfo docFullPath= new DirectoryInfo(appo);
					

					if(!docFullPath.Exists)
					{
						//crea la directory
						docFullPath.Create();
					}
					
					//restituisce la directory
					result=filePath;

				}
			}
			catch(Exception e)
			{
				logger.Debug("Errore creazione path documentale per documento: " + fileRequest.docNumber ,e);
				result=null;
			}
			return result;
		}

		/// <summary>
		/// </summary>
		/// <param name="fileRequest"></param>
		/// <param name="fileDoc"></param>
		/// <param name="objSicurezza"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		public bool PutFile(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento fileDocumento, string estensione) 
		{
			bool result;

            string versionID=fileRequest.versionId;
			string version=fileRequest.version;
			string dst=userInfo.dst;

			DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest) fileRequest.Clone();

			// scrivi un file temporaneo
			string tmpFile = DocsPaDocumentale_FILENET.FilenetLib.DocumentManagement.writeTempFile(fileRequest, fileDocumento);
			string tmpFullName = ConfigurationManager.AppSettings["FNET_pathServer"] + "\\" + tmpFile;
			try 
			{
				// acqisisce il file temporaneo
				string numversFNET=Versione.createVersionFNET(tmpFile, ref fileRequest, this.userInfo.idAmministrazione, dst);
				File.Delete(tmpFullName);
			
				// cancella la vecchia versione
				// Maurizio Tammacco -> commentato perchè sembra non corretto
				//Versione versione = new Versione();
				//versione.remove(fr, objSicurezza.idAmministrazione, fileRequest.versionId, dst);

				// aggiorna i dati della nuova versione
				Versione.updateVers(fileRequest);
				DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
				doc.F_UpdateVersions(numversFNET, fileRequest.docNumber, version);
			} 
			catch (Exception e) 
			{
				if (File.Exists(tmpFullName)) File.Delete(tmpFullName);
				Versione versione = new Versione();
				versione.unlock(fileRequest, this.userInfo.idAmministrazione);
				logger.Debug("Errore durante la scrittura di un documento in Filenet", e);
				throw e;
			}

			try 
			{
				string nomeFile = fileRequest.fileName;
				string docNumber = fileRequest.docNumber;
				//string version_id = GetVersionID(docNumber);	
				string version_id = versionID;
				fileRequest.version=version;

				// Inserimento Stream
				//string filePath = System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"];
				//string fileName = version_id + "." + fileRequest.applicazione.estensione;
				//docPath contiene il percorso del file esclusa la docRoot

				string fileNameFilenet="";
				string docPath = GetStackFilenet(fileRequest.fNversionId, 
					out fileNameFilenet);
				if(docPath==null || fileNameFilenet.Length==0)
				{
					string msg="Impossibile costruire il percorso del documentale filenet";
					logger.Debug(msg);
					throw new Exception(msg);
				}

				string docPathChild=docPath;
				docPath=ConfigurationManager.AppSettings["DOC_ROOT"]+@"\"+docPath;

				//if (! Directory.Exists(docPath))
				//	Directory.CreateDirectory(docPath);


				//filePath è il percorso completo del file
				string filePath = docPath + @"\" + fileNameFilenet;
 
				//FileStream fileStream = new FileStream(filePath, FileMode.Create);
				//fileStream.Write(fileDocumento.content, 0, fileDocumento.content.Length);
				//fileStream.Close();
				
				string varImpronta = DocsPaUtils.Security.CryptographyManager.CalcolaImpronta256(fileDocumento.content);
				
				// aggiorno la tabella COMPONENTS				
				DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
				doc.UpdateComponents(fileDocumento.content.Length.ToString(),varImpronta,version_id,docNumber);
				
				DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
				//documentale.UpdateFileName(fileName, version_id);
				//il filename è il percorso del file esclusa la doc root
				documentale.UpdateFileName(docPathChild + @"\" + fileNameFilenet, version_id);

				//Aggiorno la tabella PROFILE
				if (Int32.Parse(fileRequest.version) > 0) 
				{
					DocsPaDB.Query_DocsPAWS.Documenti document2 = new DocsPaDB.Query_DocsPAWS.Documenti();
					doc.SetImg(fileRequest.docNumber);
				}

				// UpdateFirmatari
				if (estensione.EndsWith("P7M")) 
				{					
					DocsPaDB.Query_DocsPAWS.Documenti document3 = new DocsPaDB.Query_DocsPAWS.Documenti();
					
					if(!doc.UpdateFirmatari(fileRequest))
					{
						logger.Debug("Errore durante l'aggiornamento firmatari.");
						throw new Exception();
					}
				}

				
				fileRequest.fileName = fileNameFilenet;
				fileRequest.fileSize = fileDocumento.content.Length.ToString();	
			} 
			catch(Exception exception)
			{				
				logger.Debug("Errore durante la scrittura di un documento.", exception);
				result = false;
				return result;
			}
			
			return true;
		}

		/// <summary>
		/// </summary>
		/// <param name="schedaDoc"></param>
		/// <param name="objSicurezza"></param>
		/// <param name="objRuolo"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
        public bool CreateProtocollo(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione, out DocsPaVO.utente.Ruolo[] ruoliSuperiori, string conCopia = null) 
		{
			bool result = true; // Presume successo
            ruoliSuperiori = null;
			string dst=userInfo.dst;
			risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.OK;
			string docNumberFilenet="";
			string docNumberETDocs="";
		
			try
			{
				schedaDocumento.docNumber = FilenetLib.DocumentManagement.CreateDocument(schedaDocumento, Int32.Parse(this.userInfo.idAmministrazione), dst);	
				docNumberFilenet=schedaDocumento.docNumber;
				if (schedaDocumento.docNumber == null)
				{
					logger.Debug("Errore nella creazione del documento sul documentale Filenet");
					throw new Exception("Errore nella creazione del documento sul documentale Filenet");
				}
			}
			catch(Exception exception)
			{
				string msg = exception.Message;
				logger.Debug("Errore durante l'aggiunta di un protocollo: " + msg);
				risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.APPLICATION_ERROR;
				return false;
			}
			

			try
			{
				// verifico i dati di ingresso
				DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
				risultatoProtocollazione = doc.CheckInputData(this.userInfo.idAmministrazione, schedaDocumento);

				logger.Debug("nomeUtente="+schedaDocumento.userId);	
			
				// creo il nuovo documento
				schedaDocumento.docNumber = this.CreateDocument(schedaDocumento);
				docNumberETDocs=schedaDocumento.docNumber;
				schedaDocumento.docNumber=docNumberFilenet;
				if(schedaDocumento.docNumber!=null && schedaDocumento.docNumber!="")
				{
					doc = new DocsPaDB.Query_DocsPAWS.Documenti();

                    if (!doc.ProtocollaDocNuovo_SEM(ref schedaDocumento, this.userInfo, ruolo, out ruoliSuperiori))
					{
						DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
						documentale.DeleteDocument(schedaDocumento.docNumber);

						throw new Exception();
					}
					// schedaDocumento.docNumber=docNumberETDocs;
				}
				else
				{
					logger.Debug("Errore nella creazione del documento sul documentale");
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				string msg = exception.Message;
				logger.Debug("Errore durante l'aggiunta di un protocollo: " + msg);
				risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.APPLICATION_ERROR;
				result = false;
			}

			return result;

		}

		/// <summary>
		/// </summary>
		/// <param name="fileRequest"></param>
		/// <param name="daInviare"></param>
		/// <returns></returns>
		public bool AddVersion(DocsPaVO.documento.FileRequest fileRequest, bool daInviare) 
		{
			bool result = true; // Presume successo

			bool update = false;
			string oldApp = null;	

			System.Data.DataSet ds;
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

			try 
			{
				if(fileRequest.applicazione!=null)
				{
					if(fileRequest.applicazione.systemId==null)
					{
						logger.Debug("sysid vuoto");

						DocsPaVO.documento.Applicazione res=new DocsPaVO.documento.Applicazione();
						doc.GetExt(fileRequest.applicazione.estensione, ref res);

						fileRequest.applicazione = res;
					}
					logger.Debug("Update della tabella profile");
					string param = "(APPLICATION is NULL OR APPLICATION != " + fileRequest.applicazione.systemId + ") AND DOCNUMBER=" + fileRequest.docNumber;
					doc.GetApplication(out oldApp, fileRequest.docNumber, fileRequest.applicazione.systemId,param);
		
					update=true;
				}

				DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
				documentale.AddVersion(ref fileRequest, this.userInfo.idPeople, this.userInfo.userId);

				//ESTRAZIONE DEL FILENAME, VERSION, LASTEDITTIME
				doc.SetCompVersions(fileRequest.versionId, fileRequest.docNumber, out ds);

				fileRequest.fileName=ds.Tables["VERS"].Rows[0]["PATH"].ToString();
				fileRequest.version=ds.Tables["VERS"].Rows[0]["VERSION"].ToString();
				fileRequest.subVersion=ds.Tables["VERS"].Rows[0]["SUBVERSION"].ToString();
				fileRequest.versionLabel=ds.Tables["VERS"].Rows[0]["VERSION_LABEL"].ToString();
				fileRequest.dataInserimento=ds.Tables["VERS"].Rows[0]["DTA_CREAZIONE"].ToString();
				
				//EMosca 29/11/2004
				/*Aggiunto && oldApp!="" nell'if.
				 * oldApp risulta vuoto per tutte le versioni 
				 * (tranne Hummingbird che inserisce di default un pdf di size=0 alla creazione del doc.)
				 */ 
				if(update && oldApp!="") 
				{
					DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
					documenti.UpdateApplication(oldApp, fileRequest.docNumber);
				}

				DocsPaDB.Query_DocsPAWS.Documenti documenti2 = new DocsPaDB.Query_DocsPAWS.Documenti();
				documenti2.UpdateVersionManager(fileRequest, daInviare);
				
				logger.Debug("Fine addVersion");								
			} 
			catch(Exception exception) 
			{
				logger.Debug("Errore durante l'aggiunta di una versione.", exception);

				if(update) 
				{
					DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
					documenti.UpdateApplication(oldApp, fileRequest.docNumber);
				}

				result = false;
			}

			return result;

			#region Codice Commentato
			//			
			//			string library = DocsPaDB.Utils.Personalization.getInstance(infoUtente.idAmministrazione).getLibrary();
			//			/*
			//			PCDCLIENTLib.PCDDocObjectClass pDocObj = new PCDCLIENTLib.PCDDocObjectClass();
			//			*/
			//			DocsPaDocumentale.HummingbirdLib.ClasseDocumento documento = new DocsPaDocumentale.HummingbirdLib.ClasseDocumento(infoUtente.dst, library, DocsPaDocumentale.HummingbirdLib.Tipi.ObjectType.Def_Prof);
			//			/*
			//			PCDCLIENTLib.PCDDocObjectClass pDocObj1 = new PCDCLIENTLib.PCDDocObjectClass();
			//			*/
			//			DocsPaDocumentale.HummingbirdLib.ClasseDocumento documento1 = new DocsPaDocumentale.HummingbirdLib.ClasseDocumento(infoUtente.dst, library, DocsPaDocumentale.HummingbirdLib.Tipi.ObjectType.Def_Prof);
			//			bool locked=false;
			//			bool update=false;
			//			string oldApp=null;
			//			//DocsPaWS.Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();
			//			//db.openConnection();
			//			System.Data.DataSet ds;
			//			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			//			try 
			//			{
			//				if(fileRequest.applicazione!=null)
			//				{
			//					if(fileRequest.applicazione.systemId==null)
			//					{
			//						
			//						fileRequest.applicazione=DocsPaWS.FileManager.getApplicazione(fileRequest.applicazione.estensione);
			//					}
			//					
			//					string param = "APPLICATION != " + fileRequest.applicazione.systemId + " AND DOCNUMBER=" + fileRequest.docNumber;
			//					doc.GetApplication(out oldApp, fileRequest.docNumber, fileRequest.applicazione.systemId,param);
			//
			//					/*string queryString="SELECT APPLICATION FROM PROFILE WHERE DOCNUMBER="+fileRequest.docNumber;
			//					
			//					oldApp=db.executeScalar(queryString).ToString();
			//					
			//					updateApplication(db, fileRequest.applicazione.systemId, fileRequest.docNumber);
			//					*/			
			//					update=true;
			//				}
			//
			//				/*
			//				
			//				pDocObj1.SetDST(infoUtente.dst);
			//				pDocObj1.SetObjectType("DEF_PROF");
			//				pDocObj1.SetProperty("%TARGET_LIBRARY", library);
			//				*/
			//
			//				/*
			//				pDocObj1.SetProperty("%OBJECT_IDENTIFIER", fileRequest.docNumber);
			//				pDocObj1.SetProperty("%STATUS","%LOCK");
			//				*/
			//				documento1.ObjectIdentifier = fileRequest.docNumber;
			//				documento1.Status = DocsPaDocumentale.HummingbirdLib.Tipi.StatusType.Lock;
			//				documento1.Update();
			//
			//				//DocsPaWS.Utils.ErrorHandler.checkPCDOperation(pDocObj1,"Errore nel locking");
			//				locked=true;
			//				
			//				if(fileRequest.versionId != null && !fileRequest.versionId.Equals("")) 
			//				{
			//					/*
			//					pDocObj.SetProperty("%VERSION_ID",fileRequest.versionId);
			//					pDocObj.SetProperty("%VERSION_DIRECTIVE","%PCD_NEWSUBVERSION");
			//					*/
			//					documento.VersionId = fileRequest.versionId;
			//					documento.SetVersionDirective(DocsPaDocumentale.HummingbirdLib.Tipi.VersionDirectiveType.NewSubversion);
			//				} 
			//				else
			//				{
			//					/*
			//					pDocObj.SetProperty("%VERSION_DIRECTIVE","%PCD_NEW_VERSION");
			//					*/
			//					documento.SetVersionDirective(DocsPaDocumentale.HummingbirdLib.Tipi.VersionDirectiveType.NewVersion);
			//				}
			//
			//				/*
			//				
			//				pDocObj.SetDST(infoUtente.dst);
			//				
			//				pDocObj.SetObjectType("DEF_PROF");
			//				
			//				pDocObj.SetProperty("%TARGET_LIBRARY", library);
			//				*/
			//
			//				/*
			//				pDocObj.SetProperty("%OBJECT_IDENTIFIER", fileRequest.docNumber);
			//				pDocObj.SetProperty("DOCNAME", "");	
			//				pDocObj.SetProperty("AUTHOR", infoUtente.idPeople);	
			//				pDocObj.SetProperty("AUTHOR_ID", infoUtente.userId);
			//				pDocObj.SetProperty("TYPE_ID", "MAIL");				
			//				pDocObj.SetProperty( "TYPIST_ID", infoUtente.idPeople);					
			//				pDocObj.SetProperty( "%VERIFY_ONLY", "%NO" );
			//				pDocObj.SetProperty("%VERSION_COMMENT", fileRequest.descrizione);	
			//				*/
			//				documento.ObjectIdentifier = fileRequest.docNumber;
			//				documento.DocumentName = "";
			//				documento.Author = infoUtente.idPeople;
			//				documento.AuthorId = infoUtente.userId;
			//				documento.TypeId = "MAIL";
			//				documento.TypistId = infoUtente.idPeople;
			//				documento.VerifyOnly = DocsPaDocumentale.HummingbirdLib.Tipi.VerifyOnlyType.No;
			//				documento.VersionComment = fileRequest.descrizione;
			//
			//				//DATA CREAZIONE DA INSERIRE
			//				/*
			//				DocsPaWS.Utils.ErrorHandler.checkPCDOperation(pDocObj,"Errore nell'assegnazione delle proprieta'");
			//				*/
			//				if(documento.GetErrorCode() != 0)
			//				{
			//					throw new Exception("Errore nell'assegnazione delle proprieta'");
			//				}
			//				
			//				
			//				documento.Update();
			//
			//				/*
			//				DocsPaWS.Utils.ErrorHandler.checkPCDOperation(pDocObj,"Errore nella creazione dell'fileRequest");
			//				*/
			//				if(documento.GetErrorCode() != 0)
			//				{
			//					throw new Exception("Errore nella creazione dell'fileRequest");
			//				}
			//
			//				/*
			//				fileRequest.versionId = pDocObj.GetReturnProperty("%VERSION_ID").ToString();
			//				pDocObj.SetProperty("%STATUS","%UNLOCK");
			//				*/
			//				fileRequest.versionId = documento.VersionId;
			//				documento.Status = DocsPaDocumentale.HummingbirdLib.Tipi.StatusType.Unlock;
			//
			//				documento.Update();
			//
			//				locked=false;
			//				//fileRequest.versionId = getLatestVersionID(fileRequest.docNumber,infoUtente);
			//				
			//				
			//				/*
			//				
			//				*/
			//				
			//
			//				//ESTRAZIONE DEL FILENAME, VERSION, LASTEDITTIME
			//				/*string fileQueryString =
			//					"SELECT A.PATH, B.VERSION, B.LASTEDITDATE, B.SUBVERSION, B.VERSION_LABEL, " +
			//					DocsPaWS.Utils.dbControl.toChar("B.DTA_CREAZIONE",false) + " AS DTA_CREAZIONE " +
			//					"FROM COMPONENTS A, VERSIONS B " +
			//					"WHERE B.VERSION_ID=A.VERSION_ID AND A.VERSION_ID="+fileRequest.versionId+" AND A.DOCNUMBER="+fileRequest.docNumber;
			//				
			//				db.fillTable(fileQueryString,ds,"VERS");*/
			//				doc.SetCompVersions(fileRequest.versionId, fileRequest.docNumber, out ds);
			//
			//				fileRequest.fileName=ds.Tables["VERS"].Rows[0]["PATH"].ToString();
			//				fileRequest.version=ds.Tables["VERS"].Rows[0]["VERSION"].ToString();
			//				fileRequest.subVersion=ds.Tables["VERS"].Rows[0]["SUBVERSION"].ToString();
			//				fileRequest.versionLabel=ds.Tables["VERS"].Rows[0]["VERSION_LABEL"].ToString();
			//				fileRequest.dataInserimento=ds.Tables["VERS"].Rows[0]["DTA_CREAZIONE"].ToString();
			//				
			//				if(update) 
			//				{
			//					updateApplication(oldApp, fileRequest.docNumber);
			//				}
			//
			//				//updateDaInviare(db, fileRequest.versionId, daInviare);
			//				updateVersion(fileRequest, daInviare);
			//				
			//				//db.closeConnection();
			//				
			//				return fileRequest;
			//			} 
			//			catch(Exception e) 
			//			{
			//				
			//				if(locked) 
			//				{
			//					/*
			//					PCDCLIENTLib.PCDDocObjectClass pDocObj2 = new PCDCLIENTLib.PCDDocObjectClass();
			//					*/
			//					DocsPaDocumentale.HummingbirdLib.ClasseDocumento documento2 = new DocsPaDocumentale.HummingbirdLib.ClasseDocumento(infoUtente.dst, library, DocsPaDocumentale.HummingbirdLib.Tipi.ObjectType.Def_Prof);
			//					
			//					/*
			//					pDocObj2.SetDST(infoUtente.dst);
			//					pDocObj2.SetObjectType("DEF_PROF");
			//					pDocObj2.SetProperty("%TARGET_LIBRARY", DocsPaWS.Utils.Personalization.getInstance(infoUtente.idAmministrazione).getLibrary());
			//					*/
			//
			//					/*
			//					pDocObj2.SetProperty("%OBJECT_IDENTIFIER", fileRequest.docNumber);
			//					pDocObj2.SetProperty("%STATUS","%UNLOCK");
			//					*/
			//					documento2.ObjectIdentifier = fileRequest.docNumber;
			//					documento2.Status = DocsPaDocumentale.HummingbirdLib.Tipi.StatusType.Unlock;
			//
			//					documento2.Update();
			//				}
			//				if(update) 
			//				{
			//					updateApplication(oldApp, fileRequest.docNumber);
			//				}
			//				//db.closeConnection();
			//				throw new Exception("F_System");
			//			}
			#endregion
		}

		/// <summary>
		/// </summary>
		/// <param name="docNumber"></param>
		/// <returns></returns>
		public string GetLatestVersionId(string docNumber) 
		{
			string versionId = null;
			
			DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
			versionId = documentale.GetLatestVersionId(docNumber);

			return versionId;
		}

		/// <summary>
		/// </summary>
		/// <param name="fileRequest"></param>
		/// <param name="estensione"></param>
		/// <param name="objSicurezza"></param>
		public bool ModifyExtension(ref DocsPaVO.documento.FileRequest fileRequest, 
			string docNumber, 
			string version_id, 
			string version, 
			string subVersion, 
			string versionLabel) 
		{
			bool result = true; // Presume successo

			try
			{
				bool allegato = fileRequest.GetType().Equals(typeof(DocsPaVO.documento.Allegato));
				string dataInserimento = fileRequest.dataInserimento;

				if(allegato)
				{
					DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
					documentale.RemoveVersion(docNumber, version_id);
				}

				version_id = fileRequest.versionId;
				
				DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
				doc.ModificaEstensione(allegato, ref fileRequest, version, versionLabel, version_id, subVersion, dataInserimento);
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la modifica dell'estensione del file.", exception);

				result = false;
			}

			return result;
		}

        /// <summary>
        /// Annullamento di un protocollo
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="protocolloAnnullato"></param>
        /// <returns></returns>
        public bool AnnullaProtocollo(ref DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.documento.ProtocolloAnnullato protocolloAnnullato)
        {
            bool retValue = false;

            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            retValue = doc.AnnullaProtocollo(this.userInfo.idPeople, schedaDocumento.systemId, ref protocolloAnnullato);

            if (retValue)
                schedaDocumento.protocollo.protocolloAnnullato = protocolloAnnullato;

            return retValue;
        }

        /// <summary>
        /// Modifica dei metadati di una versione
        /// </summary>
        /// <param name="fileRequest"></param>
        public void ModifyVersion(DocsPaVO.documento.FileRequest fileRequest)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            doc.ModificaVersione(fileRequest);
        }

        /// <summary>
        /// Setta a 1 cha_segnatura se la versione contiene un documento in formato pdf, con segnatura impressa
        /// </summary>
        /// <param name="versionId"></param>
        /// <returns>
        /// bool che indica l'esito dell'operazione di update
        /// </returns>
        public bool ModifyVersionSegnatura(string versionId)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.ModifyVersionSegnatura(versionId);
        }

        /// <summary>
        /// Informa se la versione ha associato un file con impressa la segnatura
        /// </summary>
        /// <param name="versionId"></param>
        /// <returns>
        /// bool che indica se la versione ha associato un file con impressa segnatura o meno
        /// </returns>
        public bool IsVersionWithSegnature(string versionId)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.IsVersionWithSegnature(versionId);
        }

        /// <summary>
        /// Rimozione di un documento grigio
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        public string RemoveDocumentoGrigio(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.ExecRimuoviSchedaMethod(this.userInfo.idPeople, schedaDocumento);
        }

        /// <summary>
        /// Inserimento di un documento nel cestino
        /// </summary>
        /// <param name="infoDocumento"></param>
        /// <returns></returns>
        public bool AddDocumentoInCestino(DocsPaVO.documento.InfoDocumento infoDocumento)
        {
            IDocumentManager documentManager = new DocsPaDocumentale_ETDOCS.Documentale.DocumentManager(this.userInfo);

            return documentManager.AddDocumentoInCestino(infoDocumento);
        }

        /// <summary>
        /// Ripristino del documento dal cestino
        /// </summary>
        /// <param name="infoDocumento"></param>
        /// <returns></returns>
        public bool RestoreDocumentoDaCestino(DocsPaVO.documento.InfoDocumento infoDocumento)
        {
            IDocumentManager documentManager = new DocsPaDocumentale_ETDOCS.Documentale.DocumentManager(this.userInfo);

            return documentManager.RestoreDocumentoDaCestino(infoDocumento);
        }

        /// <summary>
        /// Scambia il file associato ad un allegato con il file associato ad un documento
        /// </summary>
        /// <param name="allegato"></param>
        /// <param name="documento"></param>
        /// <returns></returns>
        public bool ScambiaAllegatoDocumento(DocsPaVO.documento.Allegato allegato, DocsPaVO.documento.Documento documento)
        {
            DocsPaDB.Query_DocsPAWS.Documenti documentiDb = new DocsPaDB.Query_DocsPAWS.Documenti();

            documentiDb.ScambiaVersioni(documento, allegato);

            return true;
        }

        /// <summary>
        /// Rimozione documenti
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public bool Remove(params DocsPaVO.documento.InfoDocumento[] items)
        {
            IDocumentManager documentManager = new DocsPaDocumentale_ETDOCS.Documentale.DocumentManager(this.userInfo);

            return documentManager.Remove(items);
        }

		private string GetVersionID(string docnumber, string version)
		{
			DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
			return documenti.F_GetNumVersione(docnumber, version, true);
		}

		private string GetFileNetVersionID(string docnumber, string vers)
		{
			DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
			// string docnumberF = Versione.getFNETename(docnumber);
			string s = documenti.F_GetFilenetNumVersione(docnumber, vers);
			string s2 = s.Substring(0,4);
			int version=int.Parse(s2);
			string zero= new string(Convert.ToChar("0"), 4-version.ToString().Length);
			string versionF=string.Format("{0}{1}",zero,version.ToString());
			return string.Format("{0}  {1}", versionF,docnumber);
		}

		public bool SetDocPath2Filenet(string versionid)
		{
			bool result=false;
			string version=versionid.Substring(0,7);
			string docnumber=versionid.Substring(7);

			DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
			System.Data.DataSet ds=documenti.F_GetVersionData(docnumber, version);
			if (ds.Tables.Count==0) return false;
			if (ds.Tables[0].Rows.Count==0) return false;
			string stackid=ds.Tables["ELEMENT"].Rows[0]["V_STACK_ID"].ToString();
			string stack_file_id=ds.Tables["ELEMENT"].Rows[0]["V_STACK_FILE_ID"].ToString();
			string stack_path=documenti.F_GetStack(stackid);
	        string version_id=documenti.F_GetVersionID(docnumber, version);
			string path_doc=stack_path.Substring(3)+@"\"+stack_file_id;
			result=documenti.F_UpdateComponents(docnumber, version_id, path_doc);
			return result;
		}

		public string GetStackFilenet(string versionid, out string filenamefilenet)
		{
			filenamefilenet="";
			string version=versionid.Substring(0,7);
			string docnumber=versionid.Substring(7);

			DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
			System.Data.DataSet ds=documenti.F_GetVersionData(docnumber, version);
			string stackid=ds.Tables["ELEMENT"].Rows[0]["V_STACK_ID"].ToString();
			string stack_file_id=ds.Tables["ELEMENT"].Rows[0]["V_STACK_FILE_ID"].ToString();
			string stack_path=documenti.F_GetStack(stackid);
			filenamefilenet=stack_file_id;
			stack_path=stack_path.Replace(":","");
			return "\\"+stack_path;
		}

        public string GetFileExtension(string docnumber, string versionid)
		{
			DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
			string v_name_fn=documenti.F_GetFilenetNumVersione(docnumber, versionid);
			string docNumb=DocsPaDocumentale_FILENET.FilenetLib.Versione.getFNETename(docnumber);
			string fileName=documenti.F_GetOriginalFileName( docNumb, v_name_fn);
			return fileName.Substring(fileName.LastIndexOf(".")+1);
		}

		public string GetOriginalFileName(string docnumber, string versionid)
		{
			DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
			string v_name_fn=documenti.F_GetFilenetNumVersione(docnumber, versionid);
			string docNumb=DocsPaDocumentale_FILENET.FilenetLib.Versione.getFNETename(docnumber);
			return documenti.F_GetOriginalFileName( docNumb, v_name_fn);
		}

        /// <summary>
        /// Impostazione di un permesso su un documento
        /// </summary>
        /// <param name="infoDiritto"></param>
        /// <returns></returns>
        public bool AddPermission(DocsPaVO.documento.DirittoOggetto infoDiritto)
        {
            IDocumentManager documentManager = new DocsPaDocumentale_ETDOCS.Documentale.DocumentManager(this.userInfo);

            return documentManager.AddPermission(infoDiritto);
        }

        /// <summary>
        /// Revoca di un permesso su un documento
        /// </summary>
        /// <param name="documentInfo"></param>
        /// <returns></returns>
        public bool RemovePermission(DocsPaVO.documento.DirittoOggetto infoDiritto)
        {
            IDocumentManager documentManager = new DocsPaDocumentale_ETDOCS.Documentale.DocumentManager(this.userInfo);

            return documentManager.AddPermission(infoDiritto);
        }

		#endregion

        /// <summary>
        /// Metodo per l'assegnazione di un diritto di tipo A ad un ruolo
        /// </summary>
        /// <param name="rights">Informazioni sul diritto da assegnare</param>
        /// <returns>True se è andato bene</returns>
        public bool AddPermissionToRole(DocsPaVO.documento.DirittoOggetto rights)
        {
            IDocumentManager documentManager = new DocsPaDocumentale_ETDOCS.Documentale.DocumentManager(this.userInfo);

            return documentManager.AddPermission(rights);
        }

        /// <summary>
        /// Aggiorna le ACL del documento
        /// </summary>
        /// <param name="schedaDocumento"></param>
        public virtual void RefreshAclDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            // Do nothing
        }
    }
}
