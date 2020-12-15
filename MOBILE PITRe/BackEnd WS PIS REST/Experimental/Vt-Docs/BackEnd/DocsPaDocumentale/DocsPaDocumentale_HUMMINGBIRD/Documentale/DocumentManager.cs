using System;
using System.IO;
using System.Collections;
using DocsPaVO.utente;
using DocsPaDocumentale.Interfaces;
using System.Threading;
using log4net;
using DocsPaVO.documento;

namespace DocsPaDocumentale_HUMMINGBIRD.Documentale
{
	/// <summary>
	/// Classe per la gestione di un documento tramite il documentale Hummingbird
	/// </summary>
	public class DocumentManager : IDocumentManager
	{
        private ILog logger = LogManager.GetLogger(typeof(DocumentManager));
		protected DocsPaVO.utente.InfoUtente userInfo;
		protected string library;
		public static Mutex semProtNuovo = new Mutex();
		public static Mutex semAddAllegati = new Mutex();
		
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
            library = DocsPaDB.Utils.Personalization.getInstance(infoUtente.idAmministrazione).getLibrary();
		}
		#endregion

		#region Metodi
        public string GetFileExtension(string docnumber, string versionid)
		{
			return null;
		}

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
        /// <returns>ID del documento o 'null' se si è verificato un errore</returns>
        public bool CreateDocumentoStampaRegistro(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.utente.Ruolo[] ruoliSuperiori)
        {
            DocsPaDocumentale.Interfaces.IDocumentManager documentManager = new DocsPaDocumentale_ETDOCS.Documentale.DocumentManager(this.userInfo);

            // Delega la creazione del documento stampa registro all'implementazione ETDOCS
            return documentManager.CreateDocumentoStampaRegistro(schedaDocumento, ruolo, out ruoliSuperiori);

            //bool retValue = false;
            //ruoliSuperiori = null;

            //DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Documento documentoHummingird = null;

            //try
            //{
            //    schedaDocumento.systemId = this.CreateDocument(schedaDocumento, out documentoHummingird);
            //    schedaDocumento.docNumber = schedaDocumento.systemId;

            //    retValue = (!string.IsNullOrEmpty(schedaDocumento.systemId));

            //    if (retValue)
            //    {
            //        // Impostazione visibilità documento ai ruoli superiori al ruolo corrente
            //        using (DocsPaDB.Query_DocsPAWS.Documenti dbDocumenti = new DocsPaDB.Query_DocsPAWS.Documenti())
            //            schedaDocumento = dbDocumenti.SetDocTrustees(schedaDocumento, ruolo, out ruoliSuperiori);
            //    }
            //}
            //catch(Exception ex)
            //{
            //    retValue = false;

            //    logger.Debug("Errore nella creazione di un documento.", ex);
            //}

            //return retValue;
		}

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
			byte[] fileCont;

			DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Documento documento = new DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Documento(this.userInfo.dst, this.library, DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.ObjectType.Def_Prof);
			
			try
			{
				documento.ObjectIdentifier = docNumber;
				documento.Fetch();
				documento.Status = DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.StatusType.Lock;
				documento.Update();

				if(documento.GetErrorCode() != 0)
				{
					logger.Debug("Errore nel locking");
					throw new Exception("Errore nel locking");
				}

				logger.Debug("documento trovato e lockato");

				DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.AcquisizioneDocumento acquisizioneDocumento = new DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.AcquisizioneDocumento(this.userInfo.dst, this.library);

                acquisizioneDocumento.AddSearchCriteria(DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.AcquisizioneSearchCriteriaType.DocumentNumber, docNumber);
                acquisizioneDocumento.AddSearchCriteria(DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.AcquisizioneSearchCriteriaType.Version, version);
                acquisizioneDocumento.AddSearchCriteria(DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.AcquisizioneSearchCriteriaType.VersionId, versionId);
                acquisizioneDocumento.AddSearchCriteria(DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.AcquisizioneSearchCriteriaType.VersionLabel, versionLabel);

				acquisizioneDocumento.Execute();

				if(acquisizioneDocumento.GetErrorCode() != 0)
				{
					logger.Debug("Errore nell'execute");
					throw new Exception("Errore nell'execute");
				}

				acquisizioneDocumento.SetRow(1);
				
				logger.Debug("Execute eseguito");

				if(acquisizioneDocumento.GetErrorCode() != 0)
				{
					logger.Debug("Errore nel getDoc");
					throw new Exception("Errore nel getDoc");
				}

				//logger.Debug("prova" + acquisizioneDocumento.Author + "\nStream..");

				DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.AcquisizioneStream acquisizioneStream = acquisizioneDocumento.Stream;

				if(acquisizioneStream == null)
				{
					logger.Debug("Stream vuoto");
				}

				int btread   = 1; // Numero di bytes che legge
				int fileSize = 0; 

				System.Collections.ArrayList fileContentArray = new System.Collections.ArrayList();

				while(btread>0)
				{
					logger.Debug("lettura");

					byte[] cont = acquisizioneStream.Read(128000, out btread);
					
					logger.Debug("fatta");
					fileSize=fileSize+btread;
					logger.Debug(cont.ToString());
					fileContentArray.Add(cont);
				}

				fileCont = new byte[fileSize];
				
				for(int i=0;i<fileContentArray.Count;i++)
				{
					logger.Debug(i+" "+((byte[])fileContentArray[i]).Length);
					
					for(int k=0;k<((byte[])fileContentArray[i]).Length;k++)
					{
						fileCont[128000*i+k]=((byte[])fileContentArray[i])[k];
					}
				}

				// unlock del documento
                documento = new DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Documento(this.userInfo.dst, this.library, DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.ObjectType.Def_Prof);

				documento.ObjectIdentifier = docNumber;
				documento.Fetch();

                documento.Status = DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.StatusType.Unlock;
				documento.Update();
			}
			catch(Exception e)
			{
				logger.Debug(e.ToString());

				documento.Status = DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.StatusType.Unlock;
				documento.Update();

				fileCont = null;
			}

			return fileCont;

			#region Codice Originale
			//			byte[] fileCont;
			//
			//			/*
			//			PCDCLIENTLib.PCDDocObject DocObj=new PCDCLIENTLib.PCDDocObjectClass();
			//			*/
			//			DocsPaDocumentale.HummingbirdLib.Documento documento = new DocsPaDocumentale.HummingbirdLib.Documento(this.userInfo.dst, library, DocsPaDocumentale.HummingbirdLib.Tipi.ObjectType.Def_Prof);
			//			
			//			try
			//			{
			//				/*
			//				DocObj.SetDST(infoUtente.dst);
			//				DocObj.SetObjectType("DEF_PROF");
			//				DocObj.SetProperty("%TARGET_LIBRARY", library);
			//
			//				DocObj.SetProperty("%OBJECT_IDENTIFIER", docNumber);
			//				*/
			//				documento.ObjectIdentifier = docNumber;
			//
			//				documento.Fetch();
			//
			//				/*
			//				DocObj.SetProperty("%STATUS", "%LOCK");
			//				*/
			//				documento.Status = DocsPaDocumentale.HummingbirdLib.Tipi.StatusType.Lock;
			//
			//				documento.Update();
			//
			//				/*
			//				DocsPaWS.Utils.ErrorHandler.checkPCDOperation(DocObj,"Errore nel locking");
			//				*/
			//				if(documento.GetErrorCode() != 0)
			//				{
			//					throw new Exception("Errore nel locking");
			//				}
			//
			//				
			//
			//				/*
			//				PCDCLIENTLib.PCDGetDoc pGetDoc=new PCDCLIENTLib.PCDGetDocClass();
			//				*/
			//				DocsPaDocumentale.HummingbirdLib.AcquisizioneDocumento acquisizioneDocumento = new DocsPaDocumentale.HummingbirdLib.AcquisizioneDocumento(this.userInfo.dst, library);
			//
			//				/*
			//				pGetDoc.SetDST(infoUtente.dst);
			//				pGetDoc.AddSearchCriteria("%TARGET_LIBRARY", DocsPaWS.Utils.Personalization.getInstance(infoUtente.idAmministrazione).getLibrary());
			//	
			//				pGetDoc.AddSearchCriteria("%DOCUMENT_NUMBER", docNumber);
			//				pGetDoc.AddSearchCriteria("%VERSION", version);
			//				pGetDoc.AddSearchCriteria("%VERSION_ID", versionId);
			//				pGetDoc.AddSearchCriteria("%VERSION_LABEL", versionLabel);
			//				*/
			//				acquisizioneDocumento.AddSearchCriteria(DocsPaDocumentale.HummingbirdLib.Tipi.AcquisizioneSearchCriteriaType.DocumentNumber, docNumber);
			//				acquisizioneDocumento.AddSearchCriteria(DocsPaDocumentale.HummingbirdLib.Tipi.AcquisizioneSearchCriteriaType.Version, version);
			//				acquisizioneDocumento.AddSearchCriteria(DocsPaDocumentale.HummingbirdLib.Tipi.AcquisizioneSearchCriteriaType.VersionId, versionId);
			//				acquisizioneDocumento.AddSearchCriteria(DocsPaDocumentale.HummingbirdLib.Tipi.AcquisizioneSearchCriteriaType.VersionLabel, versionLabel);
			//
			//				acquisizioneDocumento.Execute();
			//
			//				/*
			//				DocsPaWS.Utils.ErrorHandler.checkPCDOperation(pGetDoc,"Errore nell'execute");
			//				*/
			//				if(acquisizioneDocumento.GetErrorCode() != 0)
			//				{
			//					throw new Exception("Errore nell'execute");
			//				}
			//
			//				acquisizioneDocumento.SetRow(1);
			//				
			//				
			//
			//				/*
			//				DocsPaWS.Utils.ErrorHandler.checkPCDOperation(pGetDoc,"Errore nel getDoc");
			//				*/
			//				if(acquisizioneDocumento.GetErrorCode() != 0)
			//				{
			//					throw new Exception("Errore nel getDoc");
			//				}
			//
			//				
			//				
			//
			//				/*
			//				PCDCLIENTLib.PCDGetStream pGetStream;
			//				*/
			//				DocsPaDocumentale.HummingbirdLib.AcquisizioneStream acquisizioneStream = acquisizioneDocumento.Stream;
			//
			//				if(acquisizioneStream == null)
			//				{
			//					
			//				}
			//				int btread=1;  //numero di bytes che legge
			//				int fileSize=0;
			//				System.Collections.ArrayList fileContentArray=new System.Collections.ArrayList();
			//
			//				while(btread>0)
			//				{
			//					
			//
			//					/*
			//					byte[] cont = (byte[])pGetStream.Read(128000,out btread);
			//					*/
			//					byte[] cont = acquisizioneStream.Read(28000, out btread);
			//					
			//					
			//					fileSize=fileSize+btread;
			//					
			//					fileContentArray.Add(cont);
			//				}
			//
			//				fileCont = new byte[fileSize];
			//				
			//				for(int i=0;i<fileContentArray.Count;i++)
			//				{
			//					
			//					
			//					for(int k=0;k<((byte[])fileContentArray[i]).Length;k++)
			//					{
			//						fileCont[128000*i+k]=((byte[])fileContentArray[i])[k];
			//					}
			//				}
			//
			//				// unlock del documento
			//				documento = new DocsPaDocumentale.HummingbirdLib.Documento(this.userInfo.dst, library, DocsPaDocumentale.HummingbirdLib.Tipi.ObjectType.Def_Prof);
			//
			//				/*
			//				DocObj.SetDST(infoUtente.dst);
			//				DocObj.SetObjectType("DEF_PROF");
			//				DocObj.SetProperty("%TARGET_LIBRARY", DocsPaWS.Utils.Personalization.getInstance(infoUtente.idAmministrazione).getLibrary());
			//			
			//				DocObj.SetProperty("%OBJECT_IDENTIFIER", docNumber);
			//				*/
			//				documento.ObjectIdentifier = docNumber;
			//
			//				documento.Fetch();
			//
			//				/*
			//				DocObj.SetProperty("%STATUS", "%UNLOCK");
			//				*/
			//				documento.Status = DocsPaDocumentale.HummingbirdLib.Tipi.StatusType.Unlock;
			//
			//				documento.Update();
			//			}
			//			catch(Exception e)
			//			{
			//				/*
			//				DocObj.SetProperty("%STATUS", "%UNLOCK");
			//				*/
			//				
			//
			//				documento.Status = DocsPaDocumentale.HummingbirdLib.Tipi.StatusType.Unlock;
			//				documento.Update();
			//
			//				fileCont = null;
			//			}
			//
			//			return fileCont;
			#endregion
		}

		/// <summary>
		/// Ritorna un oggetto di ricerca.
		/// </summary>
		/// <param name="docNumber"></param>
		/// <returns>
		/// Oggetto di ricerca o 'null' se si è verificato un errore.
		/// </returns>
        private DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Ricerca GetSearcObject(string docNumber) 
		{
            DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Ricerca ricerca = new DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Ricerca(this.userInfo.dst, this.library, DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.ObjectType.Cyd_CmnVersions);

			// Return the version id.
            ricerca.AddReturnProperty(DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.SearchReturnPropertyType.VersionId);
            ricerca.AddReturnProperty(DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.SearchReturnPropertyType.Version);
            ricerca.AddReturnProperty(DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.SearchReturnPropertyType.SubVersion);
            ricerca.AddReturnProperty(DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.SearchReturnPropertyType.VersionLabel);

			// Constrain the search to the docnumber that was passed in.
            ricerca.AddSearchCriteria(DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.SearchCriteriaType.DocumentNumber, docNumber);

			// Sort by VERSION and SUBVERSION.
            ricerca.AddOrderByProperty(DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.OrderByPropertyType.Version, false);
            ricerca.AddOrderByProperty(DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.OrderByPropertyType.SubVersion, false);

			// Execute the search.
			ricerca.Execute();

			if(ricerca.GetErrorCode() != 0)
			{
				logger.Debug("ERROR_CHECKINDOCACT_LATESTVERSIONEXECUTE_FORLOCK");
				ricerca = null;
			}

			// Go to the first row of the results, and save the version id.
			ricerca.NextRow();

			if(ricerca.GetErrorCode() != 0)
			{
				logger.Debug("ERROR_CHECKINDOCACT_LATESTVERSIONNEXTROW_FORLOCK");
				ricerca = null;
			}

			return ricerca;

			#region Codice Originale
			//			//string library = DocsPaDB.Utils.Personalization.getInstance(objSicurezza.idAmministrazione).getLibrary();
			//				
			//			/*
			//			
			//			
			//			// Create a search object.
			//			PCDCLIENTLib.PCDSearch pLatestVersion = new PCDCLIENTLib.PCDSearch();
			//
			//			// Set the Security Token.
			//			pLatestVersion.SetDST(objSicurezza.dst);
			//
			//			// Add the library that was passed in.
			//			pLatestVersion.AddSearchLib(library);
			//
			//			// Use the versions form to access properties.
			//			pLatestVersion.SetSearchObject("cyd_cmnversions");
			//			*/
			//			DocsPaDocumentale.HummingbirdLib.Ricerca ricerca = new DocsPaDocumentale.HummingbirdLib.Ricerca(this.userInfo.dst, library, DocsPaDocumentale.HummingbirdLib.Tipi.ObjectType.Cyd_CmnVersions);
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
			//			/*
			//			DocsPaWS.Utils.ErrorHandler.checkPCDOperation(pLatestVersion, "ERROR_CHECKINDOCACT_LATESTVERSIONEXECUTE_FORLOCK");
			//			*/
			//			if(ricerca.GetErrorCode() != 0)
			//			{
			//				//throw new Exception("ERROR_CHECKINDOCACT_LATESTVERSIONEXECUTE_FORLOCK");
			//				
			//				ricerca = null;
			//			}
			//
			//			// Go to the first row of the results, and save the version id.
			//			ricerca.NextRow();
			//
			//			/*
			//			DocsPaWS.Utils.ErrorHandler.checkPCDOperation(pLatestVersion, "ERROR_CHECKINDOCACT_LATESTVERSIONNEXTROW_FORLOCK");
			//			*/
			//			if(ricerca.GetErrorCode() != 0)
			//			{
			//				//throw new Exception("ERROR_CHECKINDOCACT_LATESTVERSIONNEXTROW_FORLOCK");
			//				
			//				ricerca = null;
			//			}
			//
			//			return ricerca;
			#endregion
		}

		/// <summary>
		/// </summary>
		/// <param name="schedaDoc"></param>
		/// <returns></returns>
        private string CreateDocument(DocsPaVO.documento.SchedaDocumento schedaDoc, out DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Documento documentoHummingird) 
		{
            documentoHummingird = new DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Documento(this.userInfo.dst, this.library, DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.ObjectType.Cyd_DefProf);

            documentoHummingird.DocumentName = "";
            documentoHummingird.Author = schedaDoc.idPeople;
            documentoHummingird.AuthorId = schedaDoc.userId;
            documentoHummingird.TypeId = schedaDoc.typeId;
            documentoHummingird.TypistId = schedaDoc.userId;
            documentoHummingird.AppId = schedaDoc.appId;
            documentoHummingird.VerifyOnly = DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.VerifyOnlyType.No;

			// è necessario inserire anche i permessi per l'utente TYPIST_ID
            documentoHummingird.SetTrustee(schedaDoc.userId, 2, 0);
            documentoHummingird.Create();

            if (documentoHummingird.GetErrorCode() != 0)
			{
				logger.Debug("Errore nella creazione del documento");
				throw new Exception("Errore nella creazione del documento");
			}

			logger.Debug("documento creato");	
			
			//ENAC:			
			//System.Threading.Thread.Sleep(300);

            string docNumber = documentoHummingird.ObjectIdentifier;
            documentoHummingird.ObjectIdentifier = docNumber;

            documentoHummingird.Status = DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.StatusType.Unlock;
            documentoHummingird.Update();

            if (documentoHummingird.GetErrorCode() != 0)
			{
                // documentoHummingird.Delete();
				throw new Exception("Errore nell'update del documento");
			}
	
			logger.Debug("unlocked");	

			//aggiorna flag daInviare
			string firstParam = "CHA_DA_INVIARE = '1'";
			try
			{
				if(schedaDoc.documenti!=null && schedaDoc.documenti.Count>0)
				{
					logger.Debug("Documenti presenti");
					int lastDocNum=schedaDoc.documenti.Count-1;
					logger.Debug(""+lastDocNum);
				
					if(((DocsPaVO.documento.Documento)schedaDoc.documenti[lastDocNum]).dataArrivo!=null && !((DocsPaVO.documento.Documento)schedaDoc.documenti[lastDocNum]).dataArrivo.Equals(""))
					{
						firstParam += ", DTA_ARRIVO =" + DocsPaDbManagement.Functions.Functions.ToDate(((DocsPaVO.documento.Documento)schedaDoc.documenti[lastDocNum]).dataArrivo);
					}
				}
			
				DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
				doc.UpdateVersions(firstParam, docNumber);
			}
			catch
			{
				logger.Debug("Errore nella creazione del documento sul documentale");
                // documentoHummingird.Delete();
				throw new Exception();
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
        /// 
		/// </summary>
		/// <param name="allegato"></param>
		/// <returns></returns>
        public bool AddAttachment(DocsPaVO.documento.Allegato allegato, string putfile)
		{
			bool result = true; // Presume successo

            DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.ClasseDocumento documento = new DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.ClasseDocumento(this.userInfo.dst, this.library, DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.ObjectType.Def_Prof);
            DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.ClasseDocumento documento1 = new DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.ClasseDocumento(this.userInfo.dst, this.library, DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.ObjectType.Def_Prof);

			bool locked=false;
			bool update=false;
			string oldApp = null;
			string oldVersion_label=allegato.versionLabel;			
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti(); 
			System.Data.DataSet ds;
			
			try
			{
				semAddAllegati.WaitOne();

                if (putfile != null && putfile != "")
                {
                    //ENAC
                    //if(doc.DeleteADT(allegato.docNumber))
                    //	logger.Debug("Cancellato ADT precedente su docnumber: "+allegato.docNumber);
                    if (allegato.versionLabel != null && !allegato.versionLabel.Equals(""))
                    {
                        doc.UpdateVersion(allegato.versionId);

                    }
                }
                else
                {
                    // Reperimento versionlabel per l'allegato
                    allegato.versionLabel = this.GetAllegatoVersionLabel(allegato.docNumber);
                }

				documento1.ObjectIdentifier = allegato.docNumber;
                documento1.Status = DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.StatusType.Lock;
				logger.Debug("Tentativo di lock del docnumber: "+allegato.docNumber);
				documento1.Update();

				locked=true;

				documento.ObjectIdentifier = allegato.docNumber;
                documento.SetVersionDirective(DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.VersionDirectiveType.AddAttachment);
				documento.AttachmentId = allegato.versionLabel;
				documento.VersionComment = allegato.descrizione;
				documento.DocumentName = "";
				documento.Author = userInfo.idPeople;	
				documento.AuthorId = userInfo.userId;
				documento.TypeId = "MAIL";
				documento.TypistId = userInfo.idPeople;
                documento.VerifyOnly = DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.VerifyOnlyType.No;
				
				// DATA CREAZIONE DA INSERIRE
				if(documento.GetErrorCode() != 0)
				{
					logger.Debug("Errore nell'assegnazione delle proprieta' su version_label "+allegato.versionLabel);
					throw new Exception("Errore nell'assegnazione delle proprieta'");
				}
				
				logger.Debug("proprietà inserite");
				logger.Debug("tentativo di update PDC su version_label: "+allegato.versionLabel);				
				documento.Update();
				
				
				
				if(documento.GetErrorCode() != 0)
				{
					
					logger.Debug("Errore nell'assegnazione delle proprieta'");
					throw new Exception("Errore nell'assegnazione delle proprieta'");
				}

				allegato.versionId = documento.VersionId;

				// UNLOCK DEL DOCUMENTO
                DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.ClasseDocumento documento2 = new DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.ClasseDocumento(this.userInfo.dst, this.library, DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.ObjectType.Def_Prof);

				documento2.ObjectIdentifier = allegato.docNumber;
                documento2.Status = DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.StatusType.Unlock;
				documento2.Update();

				locked = false;

				
				doc.GetPath(out ds, allegato.versionId, allegato.docNumber);
				
				
				// Ricerca filename
				//controlla se esiste, altrimenti in ENAC errori randomici:
				if(ds!=null && ds.Tables["PATH"]!=null && ds.Tables["PATH"].Rows.Count>0)
				{
					allegato.fileName = ds.Tables["PATH"].Rows[0]["PATH"].ToString();
				}
				else
				{
					System.Threading.Thread.Sleep(800);
					logger.Debug("fn="+allegato.fileName+" tentativo2 di creazione versionid="+allegato.versionId+" docnumber="+allegato.docNumber);
					doc.GetPath(out ds, allegato.versionId, allegato.docNumber);

					allegato.fileName = ds.Tables["PATH"].Rows[0]["PATH"].ToString();
					logger.Debug("fn="+allegato.fileName+" ritentato versionid="+allegato.versionId+" docnumber="+allegato.docNumber);
				}
				logger.Debug("fn="+allegato.fileName);
				
				if(update)
				{
					doc.UpdateAppProfile(oldApp, allegato.docNumber);
					update = false;
				}
				
				doc.UpdateNumPageDescription(allegato.numeroPagine, allegato.versionId, allegato.descrizione);
				
			}
			catch(Exception e)
			{
				logger.Debug(e);
				
				if(locked)
				{
                    DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.ClasseDocumento documento2 = new DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.ClasseDocumento(this.userInfo.dst, this.library, DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.ObjectType.Def_Prof);

					documento2.ObjectIdentifier = allegato.docNumber;
                    documento2.Status = DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.StatusType.Unlock;
					documento2.Update();
				}
				if(update)
				{
					doc.UpdateAppProfile(oldApp, allegato.docNumber);
				}
				
				result = false;
				
				if(putfile!=null  && putfile!="")
				{
					if(allegato!=null && allegato.versionLabel != null && ! allegato.versionLabel.Equals(""))	
					{
						doc.RollBackUpdateVersion(allegato.versionId,oldVersion_label);

					} 
				}
			}
			finally
			{
				semAddAllegati.ReleaseMutex();
			}
			return result;

			#region Codice Originale
			//			bool result = true; // Presume successo
			//
			//			/*
			//			PCDCLIENTLib.PCDDocObjectClass pDocObj = new PCDCLIENTLib.PCDDocObjectClass();
			//			*/
			//			DocsPaDocumentale.HummingbirdLib.ClasseDocumento documento = new DocsPaDocumentale.HummingbirdLib.ClasseDocumento(this.userInfo.dst, library, DocsPaDocumentale.HummingbirdLib.Tipi.ObjectType.Def_Prof);
			//
			//			/*
			//			PCDCLIENTLib.PCDDocObjectClass pDocObj1 = new PCDCLIENTLib.PCDDocObjectClass();
			//			*/
			//			DocsPaDocumentale.HummingbirdLib.ClasseDocumento documento1 = new DocsPaDocumentale.HummingbirdLib.ClasseDocumento(this.userInfo.dst, library, DocsPaDocumentale.HummingbirdLib.Tipi.ObjectType.Def_Prof);
			//
			//			bool locked=false;
			//			bool update=false;
			//			string oldApp = null;
			//						
			//			//DocsPaWS.Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();
			//
			//			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti(); 
			//			System.Data.DataSet ds;
			//			
			//			try
			//			{
			////				if(allegato.applicazione!=null)
			////				{
			////					if(allegato.applicazione.systemId==null)
			////					{
			////						allegato.applicazione=DocsPaWS.FileManager.getApplicazione(allegato.applicazione.estensione);
			////					}
			////
			////					
			////					
			////					//db.openConnection();
			////					/*
			////					string queryString="SELECT APPLICATION FROM PROFILE WHERE DOCNUMBER="+allegato.docNumber;
			////					oldApp=db.executeScalar(queryString).ToString();
			////					updateString="UPDATE PROFILE SET APPLICATION="+allegato.applicazione.systemId+" WHERE DOCNUMBER="+allegato.docNumber;
			////					db.executeNonQuery(updateString);
			////					*/
			////
			////					string param = "DOCNUMBER="+allegato.docNumber;
			////					doc.GetApplication(out oldApp, allegato.docNumber, allegato.applicazione.systemId, param);
			////					
			////					update=true;
			////				}
			////
			////				if(allegato.versionLabel != null && ! allegato.versionLabel.Equals(""))	
			////				{
			////					/*string sqlString =
			////						"UPDATE VERSIONS SET VERSION_LABEL='ATD' WHERE VERSION_ID=" + allegato.versionId;
			////					db.executeNonQuery(sqlString);*/
			////
			////					doc.UpdateVersion(allegato.versionId);
			////
			////				} 
			////				else
			////				{
			////					allegato.versionLabel=getVersionLabel(allegato.docNumber);
			////				}
			//
			//				/*
			//				
			//				pDocObj1.SetDST(infoUtente.dst);
			//				pDocObj1.SetObjectType("DEF_PROF");
			//				pDocObj1.SetProperty("%TARGET_LIBRARY", DocsPaWS.Utils.Personalization.getInstance(infoUtente.idAmministrazione).getLibrary());
			//				*/
			//
			//				/*
			//				pDocObj1.SetProperty("%OBJECT_IDENTIFIER", allegato.docNumber);
			//				*/
			//				documento1.ObjectIdentifier = allegato.docNumber;
			//
			//				//pDocObj1.SetProperty("%VERSION_ID",VersioniManager.getLatestVersionID(allegato.docNumber,infoUtente));
			//				
			//				/*
			//				pDocObj1.SetProperty("%STATUS","%LOCK");
			//				*/
			//				documento1.Status = DocsPaDocumentale.HummingbirdLib.Tipi.StatusType.Lock;
			//
			//				documento1.Update();
			//
			//				//DocsPaWS.Utils.ErrorHandler.checkPCDOperation(pDocObj1,"Errore nel locking");
			//				locked=true;
			//
			//				/*
			//				
			//				pDocObj.SetDST(infoUtente.dst);
			//				
			//				pDocObj.SetObjectType("DEF_PROF");
			//				
			//				pDocObj.SetProperty("%TARGET_LIBRARY", DocsPaWS.Utils.Personalization.getInstance(infoUtente.idAmministrazione).getLibrary());
			//				*/
			//
			//				/*
			//				pDocObj.SetProperty("%OBJECT_IDENTIFIER", allegato.docNumber);
			//				pDocObj.SetProperty("%VERSION_DIRECTIVE","%ADD_ATTACHMENT");
			//				pDocObj.SetProperty("%ATTACHMENT_ID",allegato.versionLabel);
			//				pDocObj.SetProperty("%VERSION_COMMENT",allegato.descrizione);
			//				pDocObj.SetProperty("DOCNAME", "");	
			//				pDocObj.SetProperty("AUTHOR", infoUtente.idPeople);	
			//				pDocObj.SetProperty("AUTHOR_ID", infoUtente.userId);
			//				pDocObj.SetProperty("TYPE_ID", "MAIL");				
			//				pDocObj.SetProperty( "TYPIST_ID", infoUtente.idPeople);					
			//				pDocObj.SetProperty( "%VERIFY_ONLY", "%NO" );
			//				*/
			//				documento.ObjectIdentifier = allegato.docNumber;
			//				documento.SetVersionDirective(DocsPaDocumentale.HummingbirdLib.Tipi.VersionDirectiveType.AddAttachment);
			//				documento.AttachmentId = allegato.versionLabel;
			//				documento.VersionComment = allegato.descrizione;
			//				documento.DocumentName = "";
			//				documento.Author = userInfo.idPeople;	
			//				documento.AuthorId = userInfo.userId;
			//				documento.TypeId = "MAIL";
			//				documento.TypistId = userInfo.idPeople;
			//				documento.VerifyOnly = DocsPaDocumentale.HummingbirdLib.Tipi.VerifyOnlyType.No;
			//
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
			//				DocsPaWS.Utils.ErrorHandler.checkPCDOperation(pDocObj,"Errore nella creazione dell'allegato");
			//				*/
			//				if(documento.GetErrorCode() != 0)
			//				{
			//					throw new Exception("Errore nell'assegnazione delle proprieta'");
			//				}
			//
			//				/*
			//				allegato.versionId = pDocObj.GetReturnProperty("%VERSION_ID").ToString();
			//				*/
			//				allegato.versionId = documento.VersionId;
			//
			//				//UNLOCK DEL DOCUMENTO
			//				/*
			//				PCDCLIENTLib.PCDDocObjectClass pDocObj2 = new PCDCLIENTLib.PCDDocObjectClass();
			//				*/
			//				DocsPaDocumentale.HummingbirdLib.ClasseDocumento documento2 = new DocsPaDocumentale.HummingbirdLib.ClasseDocumento(this.userInfo.dst, library, DocsPaDocumentale.HummingbirdLib.Tipi.ObjectType.Def_Prof);
			//
			//				/*
			//				pDocObj2.SetDST(infoUtente.dst);
			//				pDocObj2.SetObjectType("DEF_PROF");
			//				pDocObj2.SetProperty("%TARGET_LIBRARY", DocsPaWS.Utils.Personalization.getInstance(infoUtente.idAmministrazione).getLibrary());
			//				*/
			//
			//				/*
			//				pDocObj2.SetProperty("%OBJECT_IDENTIFIER", allegato.docNumber);
			//				*/
			//				documento2.ObjectIdentifier = allegato.docNumber;
			//				
			//				//pDocObj2.SetProperty("%VERSION_ID",VersioniManager.getLatestVersionID(allegato.docNumber,infoUtente));
			//				
			//				/*
			//				pDocObj2.SetProperty("%STATUS","%UNLOCK");
			//				*/
			//				documento2.Status = DocsPaDocumentale.HummingbirdLib.Tipi.StatusType.Unlock;
			//
			//				documento2.Update();
			//
			//				locked=false;
			//				//ricerca filename
			//
			//				/*string fileQueryString="SELECT PATH FROM COMPONENTS WHERE VERSION_ID="+allegato.versionId+" AND DOCNUMBER="+allegato.docNumber;
			//				db.fillTable(fileQueryString,ds,"PATH");*/
			//				doc.GetPath(out ds, allegato.versionId, allegato.docNumber);
			//
			//				allegato.fileName = ds.Tables["PATH"].Rows[0]["PATH"].ToString();
			//				
			//				
			//				if(update)
			//				{
			//					/*updateString="UPDATE PROFILE SET APPLICATION="+oldApp+" WHERE DOCNUMBER="+allegato.docNumber;
			//					db.executeNonQuery(updateString);*/
			//					doc.UpdateAppProfile(oldApp, allegato.docNumber);
			//					update=false;
			//					//db.closeConnection();
			//				}
			//
			//				//numero pagine
			//				//db.openConnection();
			//				/*updateString="UPDATE VERSIONS SET NUM_PAG_ALLEGATI="+allegato.numeroPagine+" WHERE VERSION_ID="+allegato.versionId;
			//				db.executeNonQuery(updateString);*/
			//				
			//				doc.UpdateNumPage(allegato.numeroPagine, allegato.versionId);
			//				//db.closeConnection();
			//			}
			//			catch(Exception e)
			//			{
			//				
			//				
			//				if(locked)
			//				{
			//					/*
			//					PCDCLIENTLib.PCDDocObjectClass pDocObj2 = new PCDCLIENTLib.PCDDocObjectClass();
			//					*/
			//					DocsPaDocumentale.HummingbirdLib.ClasseDocumento documento2 = new DocsPaDocumentale.HummingbirdLib.ClasseDocumento(this.userInfo.dst, library, DocsPaDocumentale.HummingbirdLib.Tipi.ObjectType.Def_Prof);
			//
			//					/*
			//					pDocObj2.SetDST(infoUtente.dst);
			//					pDocObj2.SetObjectType("DEF_PROF");
			//					pDocObj2.SetProperty("%TARGET_LIBRARY", DocsPaWS.Utils.Personalization.getInstance(infoUtente.idAmministrazione).getLibrary());
			//					*/
			//
			//					/*
			//					pDocObj2.SetProperty("%OBJECT_IDENTIFIER", allegato.docNumber);
			//					*/
			//					documento2.ObjectIdentifier = allegato.docNumber;
			//
			//					//pDocObj2.SetProperty("%VERSION_ID",VersioniManager.getLatestVersionID(allegato.docNumber,infoUtente));
			//					
			//					/*
			//					pDocObj2.SetProperty("%STATUS","%UNLOCK");
			//					*/
			//					documento2.Status = DocsPaDocumentale.HummingbirdLib.Tipi.StatusType.Unlock;
			//
			//					documento2.Update();
			//				}
			//				if(update)
			//				{
			//					/*updateString="UPDATE PROFILE SET APPLICATION="+oldApp+" WHERE DOCNUMBER="+allegato.docNumber;
			//					db.executeNonQuery(updateString);*/
			//					doc.UpdateAppProfile(oldApp, allegato.docNumber);
			//					//db.closeConnection();
			//				}
			//				
			//				//throw new Exception("F_System");
			//				
			//
			//				result = false;
			//			}
			//
			//			return result;
			#endregion
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
            ruoliSuperiori = null;

			try
			{
				// creo il nuovo documento
                DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Documento documento;
				schedaDocumento.docNumber = this.CreateDocument(schedaDocumento, out documento);	

				DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();

                if (!documenti.AddDocGrigia(this.userInfo.idAmministrazione, ref schedaDocumento, this.userInfo, ruolo, out ruoliSuperiori))
				{
					// documento.Delete();
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug(exception.ToString());

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
            return doc.PredisponiAllaProtocollazione(this.userInfo,
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
            return doc.SalvaModifiche(this.userInfo, ufficioReferenteEnabled, out ufficioReferenteSaved, ref schedaDocumento);
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
			bool result = true; // Presume successo

            DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.ClasseDocumento documento = new DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.ClasseDocumento(this.userInfo.dst, library, DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.ObjectType.Def_Prof);
            DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.ClasseDocumento documento1 = new DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.ClasseDocumento(this.userInfo.dst, library, DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.ObjectType.Def_Prof);

			bool locked=false;
			
			try
			{
				documento1.ObjectIdentifier = fileRequest.docNumber;

				if(documento.GetErrorCode() != 0)
				{
					logger.Debug("Errore nella ricerca dell'allegato");
					throw new Exception("Errore nella ricerca dell'allegato");
				}

				logger.Debug("proprietà inserite");

                documento1.Status = DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.StatusType.Lock;
				documento1.Update();

				if(documento.GetErrorCode() != 0)
				{
					logger.Debug("Errore di update");
					throw new Exception("Errore di update");
				}

				locked=true;

				// Cancellazione versione
				documento.ObjectIdentifier = fileRequest.docNumber;
				documento.VersionId = fileRequest.versionId;
                documento.SetVersionDirective(DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.VersionDirectiveType.DeleteVersion);
				documento.Update();

				if(documento.GetErrorCode() != 0)
				{
					logger.Debug("Errore nella cancellazione dell'allegato");
					throw new Exception("Errore nella cancellazione dell'allegato");
				}

				// Unlock del documento
                DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.ClasseDocumento documento2 = new DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.ClasseDocumento(this.userInfo.dst, library, DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.ObjectType.Def_Prof);

				documento2.ObjectIdentifier = fileRequest.docNumber;
                documento2.Status = DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.StatusType.Unlock;
				documento2.Update();

				locked = false;
			}
			catch (Exception e) 
			{
				if(locked)
				{
                    DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.ClasseDocumento documento2 = new DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.ClasseDocumento(this.userInfo.dst, library, DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.ObjectType.Def_Prof);

					documento2.ObjectIdentifier = fileRequest.docNumber;
                    documento2.Status = DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.StatusType.Unlock;
					documento2.Update();
				}
				logger.Debug(e);
				
				result = false;
			}

			return result;

			#region Codice Originale
			//			
			//			
			//			string library = DocsPaDB.Utils.Personalization.getInstance(objSicurezza.idAmministrazione).getLibrary();
			//
			//			/*
			//			PCDCLIENTLib.PCDDocObjectClass pDocObj = new PCDCLIENTLib.PCDDocObjectClass();
			//			*/
			//			DocsPaDocumentale.HummingbirdLib.ClasseDocumento documento = new DocsPaDocumentale.HummingbirdLib.ClasseDocumento(objSicurezza.dst, library, DocsPaDocumentale.HummingbirdLib.Tipi.ObjectType.Def_Prof);
			//
			//			/*
			//			PCDCLIENTLib.PCDDocObjectClass pDocObj1 = new PCDCLIENTLib.PCDDocObjectClass();
			//			*/
			//			DocsPaDocumentale.HummingbirdLib.ClasseDocumento documento1 = new DocsPaDocumentale.HummingbirdLib.ClasseDocumento(objSicurezza.dst, library, DocsPaDocumentale.HummingbirdLib.Tipi.ObjectType.Def_Prof);
			//
			//			bool locked=false;
			//			
			//			try
			//			{
			//				/*
			//				pDocObj1.SetDST(objSicurezza.dst);
			//				
			//				pDocObj1.SetObjectType("DEF_PROF");
			//				
			//				
			//				
			//				pDocObj1.SetProperty("%TARGET_LIBRARY", DocsPaWS.Utils.Personalization.getInstance(objSicurezza.idAmministrazione).getLibrary());
			//				*/
			//
			//				/*
			//				pDocObj1.SetProperty("%OBJECT_IDENTIFIER",fileReq.docNumber);
			//				*/
			//				documento1.ObjectIdentifier = fileReq.docNumber;
			//
			//				/*
			//				DocsPaWS.Utils.ErrorHandler.checkPCDOperation(pDocObj,"Errore nella ricerca dell'allegato");
			//				*/
			//				if(documento.GetErrorCode() != 0)
			//				{
			//					throw new Exception("Errore nella ricerca dell'allegato");
			//				}
			//
			//				
			//
			//				/*
			//				pDocObj1.SetProperty("%STATUS","%LOCK");
			//				*/
			//				documento1.Status = DocsPaDocumentale.HummingbirdLib.Tipi.StatusType.Lock;
			//
			//				documento1.Update();
			//
			//				/*
			//				DocsPaWS.Utils.ErrorHandler.checkPCDOperation(pDocObj,"Errore di update");
			//				*/
			//				if(documento.GetErrorCode() != 0)
			//				{
			//					throw new Exception("Errore di update");
			//				}
			//
			//				locked=true;
			//
			//				//cancellazione versione
			//				/*
			//				pDocObj.SetDST(objSicurezza.dst);
			//				
			//				pDocObj.SetObjectType("DEF_PROF");
			//				pDocObj.SetProperty("%TARGET_LIBRARY", DocsPaWS.Utils.Personalization.getInstance(objSicurezza.idAmministrazione).getLibrary());
			//				*/
			//
			//				/*
			//				pDocObj.SetProperty("%OBJECT_IDENTIFIER", fileReq.docNumber);
			//				*/
			//				documento.ObjectIdentifier = fileReq.docNumber;
			//
			//				/*
			//				pDocObj.SetProperty("%VERSION_ID",fileReq.versionId);
			//				*/
			//				documento.VersionId = fileReq.versionId;
			//
			//				/*
			//				pDocObj.SetProperty("%VERSION_DIRECTIVE","%PCD_DELETE_VERSION");
			//				*/
			//				documento.SetVersionDirective(DocsPaDocumentale.HummingbirdLib.Tipi.VersionDirectiveType.DeleteVersion);
			//
			//				documento.Update();
			//
			//				/*
			//				DocsPaWS.Utils.ErrorHandler.checkPCDOperation(pDocObj,"Errore nella cancellazione dell'allegato");
			//				*/
			//				if(documento.GetErrorCode() != 0)
			//				{
			//					throw new Exception("Errore nella cancellazione dell'allegato");
			//				}
			//
			//				//unlock del documento
			//				/*
			//				PCDCLIENTLib.PCDDocObjectClass pDocObj2 = new PCDCLIENTLib.PCDDocObjectClass();
			//				*/
			//				DocsPaDocumentale.HummingbirdLib.ClasseDocumento documento2 = new DocsPaDocumentale.HummingbirdLib.ClasseDocumento(objSicurezza.dst, library, DocsPaDocumentale.HummingbirdLib.Tipi.ObjectType.Def_Prof);
			//
			//				/*
			//				pDocObj2.SetDST(objSicurezza.dst);
			//				pDocObj2.SetObjectType("DEF_PROF");
			//				pDocObj2.SetProperty("%TARGET_LIBRARY", DocsPaWS.Utils.Personalization.getInstance(objSicurezza.idAmministrazione).getLibrary());
			//				*/
			//
			//				/*
			//				pDocObj2.SetProperty("%OBJECT_IDENTIFIER", fileReq.docNumber);
			//				*/
			//				documento2.ObjectIdentifier = fileReq.docNumber;
			//
			//				/*
			//				pDocObj2.SetProperty("%STATUS","%UNLOCK");
			//				*/
			//				documento2.Status = DocsPaDocumentale.HummingbirdLib.Tipi.StatusType.Unlock;
			//
			//				documento2.Update();
			//
			//				locked=false;
			//
			//				/*db.openConnection();
			//				db.beginTransaction();
			//				string filePath=allegato.docServerLoc+allegato.path+allegato.fileName;
			//				string deleteVersions="DELETE FROM VERSIONS WHERE VERSION_ID="+allegato.versionId;
			//				db.executeNonQuery(deleteVersions);
			//				string deleteComponents="DELETE FROM COMPONENTS WHERE VERSION_ID="+allegato.versionId;
			//				db.executeNonQuery(deleteComponents);
			//				System.IO.File.Delete(filePath);
			//				db.commitTransaction();*/
			//			}
			//			catch (Exception e) 
			//			{
			//				if(locked)
			//				{
			//					/*
			//					PCDCLIENTLib.PCDDocObjectClass pDocObj2 = new PCDCLIENTLib.PCDDocObjectClass();
			//					*/
			//					DocsPaDocumentale.HummingbirdLib.ClasseDocumento documento2 = new DocsPaDocumentale.HummingbirdLib.ClasseDocumento(objSicurezza.dst, library, DocsPaDocumentale.HummingbirdLib.Tipi.ObjectType.Def_Prof);
			//
			//					/*
			//					pDocObj2.SetDST(objSicurezza.dst);
			//					pDocObj2.SetObjectType("DEF_PROF");
			//					pDocObj2.SetProperty("%TARGET_LIBRARY", DocsPaWS.Utils.Personalization.getInstance(objSicurezza.idAmministrazione).getLibrary());
			//					*/
			//
			//					/*
			//					pDocObj2.SetProperty("%OBJECT_IDENTIFIER", fileReq.docNumber);
			//					*/
			//					documento2.ObjectIdentifier = fileReq.docNumber;
			//					
			//					/*
			//					pDocObj2.SetProperty("%STATUS","%UNLOCK");
			//					*/
			//					documento2.Status = DocsPaDocumentale.HummingbirdLib.Tipi.StatusType.Unlock;
			//
			//					documento2.Update();
			//				}
			//				
			//				
			//				throw new Exception("F_System");
			//			}
			#endregion
		}

        public bool GetFile(ref DocsPaVO.documento.FileDocumento fileDocumento, ref DocsPaVO.documento.FileRequest fileRequest)
        {
            bool result = true; // Presume successo

            try
            {
                DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.AcquisizioneDocumento acquisizioneDocumento = new DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.AcquisizioneDocumento(this.userInfo.dst, library);

                acquisizioneDocumento.DocumentNumber = fileRequest.docNumber;
                acquisizioneDocumento.VersionId = fileRequest.versionId;
                acquisizioneDocumento.Execute();

                if (acquisizioneDocumento.GetErrorCode() != 0)
                {
                    logger.Debug("Errore durante l'acquisizione del file.");
                    throw new Exception();
                }

                acquisizioneDocumento.NextRow();

                if (acquisizioneDocumento.GetErrorCode() != 0)
                {
                    logger.Debug("Errore durante l'acquisizione del file.");
                    throw new Exception();
                }

                DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.AcquisizioneStream acquisizioneStream = acquisizioneDocumento.Stream;

                fileDocumento.length = acquisizioneStream.StreamSize;

                logger.Debug("name = " + fileDocumento.name);
                logger.Debug("length = " + fileDocumento.length);

                int bytesRead;
                byte[] stream = acquisizioneStream.Read(fileDocumento.length, out bytesRead);

                if (fileDocumento.length != bytesRead)
                    throw new ApplicationException(string.Format("Errore nella lettura del file dal documentale. Sono stati estratti {0} bytes invece di {1} bytes", bytesRead.ToString(), fileDocumento.length.ToString()));

                fileDocumento.content = stream;

                acquisizioneStream.SetComplete();
            }
            catch (Exception exception)
            {
                logger.Debug("Errore nella ricerca del file.", exception);

                result = false;
                throw exception;
            }

            return result;

            #region Codice Originale
            //			DocsPaVO.documento.FileDocumento fileDoc = new DocsPaVO.documento.FileDocumento();
            //			fileDoc.path = objFileRequest.docServerLoc + objFileRequest.path;
            //			fileDoc.name = objFileRequest.fileName;
            //			fileDoc.fullName = fileDoc.path + '\u005C'.ToString() + fileDoc.name;	
            //			fileDoc.contentType = getContentType(fileDoc.name);
            //			
            //			
            //			
            //			
            //			string docNumber = objFileRequest.docNumber;
            //			string versionId = objFileRequest.versionId;	
            //			string version_label = objFileRequest.versionLabel;	
            //							
            //			string library = DocsPaDB.Utils.Personalization.getInstance(objSicurezza.idAmministrazione).getLibrary();
            //
            //			/*
            //			PCDCLIENTLib.PCDGetDoc pGetDoc = new PCDCLIENTLib.PCDGetDoc();
            //			*/
            //			DocsPaDocumentale.HummingbirdLib.AcquisizioneDocumento acquisizioneDocumento = new DocsPaDocumentale.HummingbirdLib.AcquisizioneDocumento(objSicurezza.dst, library);
            //
            //			/*
            //			pGetDoc.SetDST(objSicurezza.dst);
            //			pGetDoc.AddSearchCriteria("%TARGET_LIBRARY", DocsPaWS.Utils.Personalization.getInstance(objSicurezza.idAmministrazione).getLibrary());
            //			*/
            //
            //			/*
            //			pGetDoc.AddSearchCriteria("%DOCUMENT_NUMBER", docNumber);			
            //			pGetDoc.AddSearchCriteria("%VERSION_ID", versionId);
            //			*/
            //			acquisizioneDocumento.DocumentNumber = docNumber;
            //			acquisizioneDocumento.VersionId = versionId;
            //
            //			acquisizioneDocumento.Execute();
            //
            //			/*
            //			DocsPaWS.Utils.ErrorHandler.checkPCDOperation(pGetDoc,"Errore nella ricerca del file");
            //			*/
            //			if(acquisizioneDocumento.GetErrorCode() != 0)
            //			{
            //				//throw new Exception("Errore nella ricerca del file");
            //				throw new Exception("Errore nella ricerca del file");
            //			}
            //
            //			acquisizioneDocumento.NextRow();
            //
            //			/*
            //			DocsPaWS.Utils.ErrorHandler.checkPCDOperation(pGetDoc,"Errore nella ricerca del file");
            //			*/
            //			if(acquisizioneDocumento.GetErrorCode() != 0)
            //			{
            //				//throw new Exception("Errore nella ricerca del file");
            //				throw new Exception("Errore nella ricerca del file");
            //			}
            //
            //			/*
            //			PCDCLIENTLib.PCDGetStream pGetStream = (PCDCLIENTLib.PCDGetStream) acquisizioneDocumento.CurrentInstance.GetPropertyValue("%CONTENT"); 
            //			*/
            //			DocsPaDocumentale.HummingbirdLib.AcquisizioneStream acquisizioneStream = acquisizioneDocumento.Stream;
            //
            //			/*
            //			DocsPaWS.Utils.ErrorHandler.checkPCDOperation(pGetDoc,"Errore nella apertura del file");
            //			*/
            //			if(acquisizioneDocumento.GetErrorCode() != 0)
            //			{
            //				//throw new Exception("Errore nella ricerca del file");
            //				throw new Exception("Errore nella ricerca del file");
            //			}
            //
            //			/*
            //			fileDoc.length = (int) pGetStream.GetPropertyValue("%ISTREAM_STATSTG_CBSIZE_LOWPART");
            //			*/
            //			fileDoc.length = acquisizioneStream.StreamSize;
            //
            //			
            //			
            //			//throw new  Exception("stop");
            //
            //			fileDoc.content = new Byte[fileDoc.length];
            //			int bytesRead = 0;
            //			int totalBytesRead = 0;
            //			int i;
            //			int j = 0;
            //			byte[] stream;
            //			
            //			do 
            //			{ 
            //				/*
            //				stream = ((byte[]) pGetStream.Read(256000, out bytesRead));	
            //				*/
            //				stream = acquisizioneStream.Read(256000, out bytesRead);
            //
            //				for (i=0; i < stream.Length; i++)
            //					fileDoc.content[j++] = stream[i];
            //				totalBytesRead += bytesRead;
            //			} 
            //			while (bytesRead > 0);
            //
            //			acquisizioneStream.SetComplete();
            //		
            //			
            //			// verifico l'impronta
            //			string impronta = "";
            //			
            //			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            //			doc.GetImpronta(out impronta, versionId, docNumber);
            //			/*DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
            //			db.openConnection();
            //			try 
            //			{
            //				string queryString =
            //					"SELECT VAR_IMPRONTA FROM COMPONENTS " +
            //					"WHERE VERSION_ID=" + version_id + " AND DOCNUMBER=" + docNumber;
            //				
            //				impronta = db.executeScalar(queryString).ToString();
            //				
            //			} 
            //			catch(Exception) 
            //			{
            //			}
            //
            //			db.closeConnection();*/
            //
            //			//verifico i path dei file per vedere se bisogna fare il controllo sull'impronta !!! ATTENZIONE
            //			if (impronta == null || impronta.Equals(""))
            //			{
            //				if(verificaPath(objFileRequest.docServerLoc))
            //				{
            //					return fileDoc; 
            //				}
            //			}
            //			
            //			if(impronta.Equals(calcolaImpronta(fileDoc.content)))
            //			{
            //				return fileDoc;
            //			}
            //			else
            //			{
            //				return null;
            //			}
            #endregion
        }


		/// <summary>
		/// </summary>
		/// <param name="fileRequest"></param>
		/// <param name="fileDoc"></param>
		/// <param name="objSicurezza"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
        /// <summary>
        public bool PutFile(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento fileDocumento, string estensione)
        {
            bool result = true; // Presume successo

            DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Documento documento = new DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Documento(this.userInfo.dst, library, DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.ObjectType.Def_Prof);

            try
            {
                string nomeFile = fileRequest.fileName;
                string docNumber = fileRequest.docNumber;
                string version_id = fileRequest.versionId;

                //// lock del file
                documento.ObjectIdentifier = docNumber;
                documento.VersionId = version_id;
                documento.Status = DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.StatusType.Lock;
                documento.Update();

                version_id = fileRequest.versionId;

                DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.InserimentoDocumento inserimentoDocumento = new DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.InserimentoDocumento(this.userInfo.dst, library);

                inserimentoDocumento.DocumentNumber = docNumber;
                inserimentoDocumento.VersionId = version_id;
                inserimentoDocumento.Execute();

                if (inserimentoDocumento.GetErrorCode() != 0)
                {
                    logger.Debug("Errore durante l'inserimento del file.");
                    throw new Exception();
                }

                inserimentoDocumento.NextRow();

                // Inserimento Stream
                DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.InserimentoStream inserimentoStream = inserimentoDocumento.Stream;

                int bytesWritten = 0;
                int bytesToWrite = fileDocumento.content.Length;

                // Scrittura dell'intero contenuto del file
                inserimentoStream.Write(fileDocumento.content, bytesToWrite, out bytesWritten);

                logger.Debug("bytesToWrite: " + bytesToWrite.ToString());
                logger.Debug("bytesWritten: " + bytesWritten.ToString());

                // Verifica se il file è stato scritto correttamente
                if (bytesToWrite != bytesWritten)
                    throw new ApplicationException(string.Format("Errore nell'inserimento del file nel documentale, sono stati scritti {0} bytes invece di {1} bytes", bytesWritten.ToString(), bytesToWrite.ToString()));

                inserimentoStream.SetComplete();

                inserimentoStream = null;

                //controllo a posteriori per problemi in HUMMY su file system.
                bool esiste=false;
                if (System.Configuration.ConfigurationSettings.AppSettings["HM_CHECK_FILE_EXISTS"] != null
                    &&
                    System.Configuration.ConfigurationSettings.AppSettings["HM_CHECK_FILE_EXISTS"] != "0"
                    )
                {

                    string path = getPathFile(docNumber, version_id);
                    esiste=File.Exists(path);
                }
            else
                    esiste=true; //non faccio controllo e vado avanto come prima

            if(esiste)
            {
                // Calcolo dell'hash del file
                string varImpronta = DocsPaUtils.Security.CryptographyManager.CalcolaImpronta256(fileDocumento.content);

                // aggiorno la tabella COMPONENTS				
                DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                doc.UpdateComponents(bytesWritten.ToString(), varImpronta, version_id, docNumber);

                //Aggiorno la tabella PROFILE

                // Set Imgage
                if (Int32.Parse(fileRequest.version) > 0)
                {
                    DocsPaDB.Query_DocsPAWS.Documenti document2 = new DocsPaDB.Query_DocsPAWS.Documenti();
                    doc.SetImg(fileRequest.docNumber);
                }

                // UpdateFirmatari
                if (estensione.EndsWith("P7M"))
                {
                    DocsPaDB.Query_DocsPAWS.Documenti document3 = new DocsPaDB.Query_DocsPAWS.Documenti();

                    if (!doc.UpdateFirmatari(fileRequest))
                    {
                        logger.Debug("Errore durante l'aggiornamento firmatari.");
                        throw new Exception();
                    }
                }
                    }
                    else
                    {
                        logger.Debug("Errore (FILE_HM_NOT_EXISTS) il file " + fileDocumento.name + " non è stato creato sul file system dal documentale Hummingbird.");
                        throw new Exception();
                    }

                // Unlock del file
                documento.ObjectIdentifier = docNumber;
                documento.Status = DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.StatusType.Unlock;
                documento.Update();

                if (documento.GetErrorCode() != 0)
                {
                    logger.Debug("Errore durante l'unlock del file.");
                    throw new Exception();
                }

                fileRequest.fileSize = bytesWritten.ToString();

                inserimentoDocumento = null;
            }
            catch (Exception e)
            {
                // unlock del file
                documento.Status = DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.StatusType.Unlock;
                documento.Update();

                if (documento.GetErrorCode() != 0)
                {
                    logger.Debug("Errore nell'unlock del documento");
                }

                logger.Debug(e.ToString());

                result = false;
            }

            return result;
        }

        private string getPathFile(string docnumber, string versionId  )
        {
            string path=string.Empty;
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {


                string cmd = "SELECT DOCSERVER_LOC,P.PATH,C.PATH FROM PROFILE P,COMPONENTS C WHERE  C.DOCNUMBER=P.DOCNUMBER AND C.DOCNUMBER=" + docnumber + " AND C.VERSION_ID=" + versionId;
                logger.Debug(cmd);
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(cmd))
                {
                    if (reader.Read())
                    {
                        if(!reader.IsDBNull(0))
                         path=reader.GetString(0);//c'è uno \in più.

                        if(!reader.IsDBNull(1))
                         path+=reader.GetString(1);

                        if(!reader.IsDBNull(2))
                         path+=reader.GetString(2);
                    }
                }
            }  
            return path;
        
        }
        private void unPutFile(string versionId, string docnumber,string subversion)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            doc.UpdateComponents("0", "null", versionId, docnumber);
            doc.RollBackVersion(versionId,subversion);           
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
			risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.OK;

			try
			{
				semProtNuovo.WaitOne();

				// verifico i dati di ingresso
				DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                bool protocollazioneLibera = false;
                ArrayList funz = ruolo.funzioni;
                foreach (DocsPaVO.utente.Funzione f in funz)//verifico se attiva la microfunz IMP_DOC_MASSIVA_PREG
                {
                    if (f.codice.Equals("IMP_DOC_MASSIVA_PREG"))
                        protocollazioneLibera = true;
                }

                risultatoProtocollazione = doc.CheckInputData(this.userInfo.idAmministrazione, schedaDocumento, protocollazioneLibera);
				if(risultatoProtocollazione == DocsPaVO.documento.ResultProtocollazione.OK)
				{
					logger.Debug("nomeUtente="+schedaDocumento.userId);	
			
					// creo il nuovo documento
                    DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Documento documento;
					schedaDocumento.docNumber = this.CreateDocument(schedaDocumento, out documento);

					if(schedaDocumento.docNumber!=null && schedaDocumento.docNumber!="")
					{
						doc = new DocsPaDB.Query_DocsPAWS.Documenti();

                        if (!doc.ProtocollaDocNuovo_SEM(ref schedaDocumento, this.userInfo, ruolo, out ruoliSuperiori))
						{
                            // NB: Con l'introduzione del TransactionContext, 
                            // è stato necessario rimuovere la delete con le api di hummingbird
                            // in quanto mandano in lock la tabella
							// documento.Delete();
							throw new Exception();
						}
					}
					else
					{
						logger.Debug("Errore nella creazione del documento sul documentale");
						
                        // NB: Con l'introduzione del TransactionContext, 
                        // è stato necessario rimuovere la delete con le api di hummingbird
                        // in quanto mandano in lock la tabella
                        // documento.Delete();
						throw new Exception();
					}
				}
				
			}
			catch(Exception exception)
			{
				logger.Debug(exception.ToString());
//				if(exception.Message.ToLower().Equals("errore nell'update del documento"))
//					risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.HM_UPDATE_ERROR;
//				else
					risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.APPLICATION_ERROR;
				result = false;
			}
			finally
			{
				semProtNuovo.ReleaseMutex();
			}

			return result;

			#region Codice Originale
			//			//DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();			
			//			
			//			// verifico i dati di ingresso
			//			//db.openConnection();
			//			checkInputData(schedaDoc, objSicurezza);
			//			
			//			
			//			// creo il nuovo documento
			//			string library = DocsPaDB.Utils.Personalization.getInstance(objSicurezza.idAmministrazione).getLibrary();
			//			DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(objSicurezza, library);
			//
			//			DocsPaDocumentale.HummingbirdLib.Documento documento;
			//			//schedaDoc.docNumber = createPCDDoc(out documento, schedaDoc, objSicurezza);	
			//			schedaDoc.docNumber = documentManager.CreateDocument(out documento, schedaDoc);	
			//			
			//			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			//			
			//			if (!doc.ProtocollaDocNuovo(ref schedaDoc, objSicurezza, objRuolo))
			//			{
			//				documento.Delete();
			//				throw new Exception();
			//			}
			//
			//			/*try 
			//			{		
			//				// leggo le informazioni sul documento dalla profile
			//				schedaDoc = getProfile(schedaDoc);
			//				
			//				// Setto i permessi per i superiori dell'utente
			//				schedaDoc = setDocTrustees(schedaDoc, objRuolo);
			//				
			//				// oggetto
			//				schedaDoc = addOggetto(schedaDoc, objSicurezza);
			//				
			//				// Inizio transazione sul db
			//				db.beginTransaction();
			//				
			//				// leggo il numero di protocollo
			//				schedaDoc = getNumeroProtocollo(db, schedaDoc, objRuolo);
			//				
			//				// aggiorno la tabella PROFILE
			//				schedaDoc.protocollo.daProtocollare = "0";
			//				updateProfile(db, schedaDoc);
			//				
			//				//se è risposta ad un protocollo, si aggiorna la tabella dei collegamenti
			//				if(schedaDoc.protocollo.rispostaProtocollo!=null&&schedaDoc.protocollo.rispostaProtocollo.idProfile!=null)
			//				{
			//					updateCollegamenti(db,schedaDoc);
			//				}
			//				
			//				// corrispondenti
			//				addCorrispondenti(db, schedaDoc);
			//				
			//				//data ultimo protocollo
			//				schedaDoc = setDataUltimoProtocollo(db, schedaDoc);
			//				db.commitTransaction();
			//				db.closeConnection();
			//			} 
			//			catch (Exception e) 
			//			{
			//				
			//				
			//				// TODO: throw throwException(db, documento,"F_System");
			//			}*/
			//			return schedaDoc;
			#endregion
		}

		/// <summary>
		/// </summary>
		/// <param name="fileRequest"></param>
		/// <param name="daInviare"></param>
		/// <returns></returns>
		public bool AddVersion(DocsPaVO.documento.FileRequest fileRequest, bool daInviare) 
		{
			bool result = true; // Presume successo

            DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.ClasseDocumento documento = new DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.ClasseDocumento(this.userInfo.dst, library, DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.ObjectType.Def_Prof);
            DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.ClasseDocumento documento1 = new DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.ClasseDocumento(this.userInfo.dst, library, DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.ObjectType.Def_Prof);
			
			bool locked=false;
			bool update=false;
			string oldApp=null;
			
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
						DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
						documenti.GetExt(fileRequest.applicazione.estensione, ref res);

						fileRequest.applicazione = res;
					}
					logger.Debug("Update della tabella profile");
					string param = "APPLICATION != " + fileRequest.applicazione.systemId + " AND DOCNUMBER=" + fileRequest.docNumber;
					doc.GetApplication(out oldApp, fileRequest.docNumber, fileRequest.applicazione.systemId,param);
		
					update=true;
				}

                documento1.ObjectIdentifier = fileRequest.docNumber;
                documento1.Status = DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.StatusType.Lock;
                documento1.Update();

				locked=true;
				
				if(fileRequest.versionId != null && !fileRequest.versionId.Equals("")) 
				{
					documento.VersionId = fileRequest.versionId;
                    documento.SetVersionDirective(DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.VersionDirectiveType.NewSubversion);
				} 
				else
				{
                    documento.SetVersionDirective(DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.VersionDirectiveType.NewVersion);
				}
                
				documento.ObjectIdentifier = fileRequest.docNumber;
				documento.DocumentName = "";
                documento.Author = this.userInfo.idPeople;
                documento.AuthorId = this.userInfo.userId;
                documento.TypeId = "MAIL";
				documento.TypistId = this.userInfo.idPeople;
                documento.VerifyOnly = DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.VerifyOnlyType.No;
				documento.VersionComment = fileRequest.descrizione;

				//DATA CREAZIONE DA INSERIRE
				if(documento.GetErrorCode() != 0)
				{
					logger.Debug("Ritento assegnazione delle proprieta'");
					System.Threading.Thread.Sleep(400);
					if(documento.GetErrorCode() != 0)
					{
						logger.Debug("Errore nell'assegnazione delle proprieta'");
						throw new Exception("Errore nell'assegnazione delle proprieta'");
					}
				}
				
				logger.Debug("proprietà inserite");
				documento.Update();

				if(documento.GetErrorCode() != 0)
				{
					logger.Debug("Errore nella creazione dell'fileRequest");
					throw new Exception("Errore nella creazione dell'fileRequest");
				}

				fileRequest.versionId = documento.VersionId;
				
				if(documento.GetErrorCode() != 0)
				{
					logger.Debug("Errore nella creazione dell'fileRequest");
					throw new Exception("Errore nella creazione dell'fileRequest");
				}


                documento.ObjectIdentifier = fileRequest.docNumber;
                documento.Status = DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.StatusType.Unlock;
                documento.Update();


				locked=false;
				
				//ESTRAZIONE DEL FILENAME, VERSION, LASTEDITTIME
				//luluciani; quindi non è un set ma un get, ma chi caccio la chiamato set....
				doc.SetCompVersions(fileRequest.versionId, fileRequest.docNumber, out ds);

				try
				{
					fileRequest.fileName=ds.Tables["VERS"].Rows[0]["PATH"].ToString();
				
				}
				catch(Exception ex)
				{
					logger.Debug(ex.Message+ "ritento estrazione nome file cond docnumber="+fileRequest.docNumber+" versionid= "+fileRequest.versionId );

					System.Threading.Thread.Sleep(400);
					doc.SetCompVersions(fileRequest.versionId, fileRequest.docNumber, out ds);
					
					fileRequest.fileName=ds.Tables["VERS"].Rows[0]["PATH"].ToString();
				}

				fileRequest.version=ds.Tables["VERS"].Rows[0]["VERSION"].ToString();
				fileRequest.subVersion=ds.Tables["VERS"].Rows[0]["SUBVERSION"].ToString();
				fileRequest.versionLabel=ds.Tables["VERS"].Rows[0]["VERSION_LABEL"].ToString();
				fileRequest.dataInserimento=ds.Tables["VERS"].Rows[0]["DTA_CREAZIONE"].ToString();
                DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
                string full_name_utente = u.getUtenteById(this.userInfo.idPeople).descrizione;
                if (full_name_utente != null)
                    fileRequest.autore = full_name_utente;
		
				DocsPaDB.Query_DocsPAWS.Documenti documenti2 = new DocsPaDB.Query_DocsPAWS.Documenti();
				documenti2.UpdateVersionManager(fileRequest, daInviare);
				
				logger.Debug("Fine addVersion");								
			} 
			catch(Exception e) 
			{
				logger.Debug(e.ToString());

				if(locked) 
				{
                    // Unlock del file
                    documento.ObjectIdentifier = fileRequest.docNumber;
                    documento.Status = DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.StatusType.Unlock;
                    documento.Update();
                }
				if(update) 
				{
					DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
					documenti.UpdateApplication(oldApp, fileRequest.docNumber);
				}
				
				result = false;
				throw new Exception("Errore nel metodo AddVersion della classe DocumentManager.cs : "+e.Message);
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

            DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Ricerca ricerca = this.GetSearcObject(docNumber);

			versionId = ricerca.VersionId;
			ricerca.ReleaseResults();

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
				//				string docNumber = fileRequest.docNumber;
				//				string version_id = fileRequest.versionId;	
				//				string version = fileRequest.version;
				//				string subVersion = fileRequest.subVersion;
				//				string versionLabel = fileRequest.versionLabel;
				string dataInserimento = fileRequest.dataInserimento;

				//DocsPaDocumentale.HummingbirdLib.Documento delDocumento = new DocsPaDocumentale.HummingbirdLib.Documento(this.userInfo.dst, this.library, DocsPaDocumentale.HummingbirdLib.Tipi.ObjectType.Def_Prof);
                DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Documento documento = new DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Documento(this.userInfo.dst, this.library, DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi.ObjectType.Def_Prof);



				//Modifica cancellazione versione a manella
				DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
				try
				{
					dbProvider.BeginTransaction();
					
					DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DOC_DELETE_VERSION_COMPONENTS");		
					queryMng.setParam("version_id",version_id);
					queryMng.setParam("docNumber",docNumber);
					string commandText=queryMng.getSQL();
					System.Diagnostics.Debug.WriteLine("SQL - elimina Versione da COMPONENTS - QUERY :"+commandText);
					dbProvider.ExecuteNonQuery(commandText);
					
					queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DOC_DELETE_VERSION_VERSIONS");		
					queryMng.setParam("version_id",version_id);
					queryMng.setParam("docNumber",docNumber);
					commandText=queryMng.getSQL();
					System.Diagnostics.Debug.WriteLine("SQL - elimina Versione da VERSIONS - QUERY :"+commandText);
					dbProvider.ExecuteNonQuery(commandText);
					
					dbProvider.CommitTransaction();
				}
				catch(Exception ex)
				{
					System.Diagnostics.Debug.WriteLine("Errore nella cancellazione della versione "+ex.Message);
					dbProvider.RollbackTransaction();	
					throw new Exception(ex.Message);					
				}
				finally
				{
					dbProvider.CloseConnection();
				}
				//Fine Modifica cancellazione versione a manella



				//delDocumento.ObjectIdentifier = docNumber;
				//delDocumento.VersionId = version_id;
				//delDocumento.SetVersionDirective(DocsPaDocumentale.HummingbirdLib.Tipi.VersionDirectiveType.DeleteVersion);
				//logger.Debug("Tentativo di cancellazione della versione: "+version_id+" del documber: "+docNumber);
				//delDocumento.Update();
			
				//if(delDocumento.GetErrorCode() != 0)
				//{
				//	logger.Debug("Errore nel delete della versione");
				//	throw new Exception("Errore nel delete della versione");
				//}

				//logger.Debug("versione cancellata");
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
        /// Rimozione documenti
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public bool Remove(params DocsPaVO.documento.InfoDocumento[] items)
        {
            IDocumentManager documentManager = new DocsPaDocumentale_ETDOCS.Documentale.DocumentManager(this.userInfo);

            return documentManager.Remove(items);
        }


		public string GetOriginalFileName(string docnumber, string versionid)
		{
			return null;
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

            return documentManager.RemovePermission(infoDiritto);
        }

        /// <summary>
        /// Metodo per l'assegnazione di un diritto di tipo A ad un ruolo
        /// </summary>
        /// <param name="rights">Informazioni sul diritto da assegnare</param>
        /// <returns>True se è andato bene</returns>
        public bool AddPermissionToRole(DirittoOggetto rights)
        {
            IDocumentManager documentManager = new DocsPaDocumentale_ETDOCS.Documentale.DocumentManager(this.userInfo);

            return documentManager.AddPermissionToRole(rights);
        }

        /// <summary>
        /// Aggiorna le ACL del documento
        /// </summary>
        /// <param name="schedaDocumento"></param>
        public virtual void RefreshAclDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            // Do nothing
        }

		#endregion
	}
}
