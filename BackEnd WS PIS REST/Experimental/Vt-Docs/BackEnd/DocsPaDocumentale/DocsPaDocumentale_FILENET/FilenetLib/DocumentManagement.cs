using System;
using System.Configuration;
using DocsPaUtils.Functions;
using DocsPaVO.utente;
using System.Collections;
using System.Data;
using log4net;

namespace DocsPaDocumentale_FILENET.FilenetLib
{
	/// <summary>
	/// Questa classe implemanta la gestione dei documenti nel documentale FileNet
	/// </summary>
	public class DocumentManagement
	{
        private static ILog logger = LogManager.GetLogger(typeof(DocumentManagement));
		private const int PAGE_SIZE=20;

		#region Metodi statici

		public static bool RemoveDocument(string docnumber, string idamministrazione)
		{
			try
			{
				IDMError.ErrorManager idmErrorManager = new IDMError.ErrorManager();
                DocsPaDocumentale_FILENET.Documentale.UserManager userManager = new DocsPaDocumentale_FILENET.Documentale.UserManager();
				IDMObjects.Library oLibrary = userManager.getFileNETLib(idamministrazione);
				IDMObjects.IFnDocumentDual docFNET = (IDMObjects.IFnDocumentDual) oLibrary.GetObject(IDMObjects.idmObjectType.idmObjTypeDocument, docnumber, oLibrary, null, null);
				if (docFNET != null)
					docFNET.Delete();
				return true;
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la rimozione del documento.", exception);
				return false;
			}

		}

		
		public static string CreateDocument(DocsPaVO.documento.SchedaDocumento schedaDoc, int idamministrazione, 
			string dst) 
		{
			string docNumber = null;

			
			try
			{
				ArrayList app = new ArrayList();
				logger.Debug("creazione Doc FileNET");
				IDMObjects.Library oLibrary;
				IDMObjects.IFnFolderDual folder;
				IDMObjects.ClassDescription cDescr;
				IDMObjects.IFnDocumentDual doc;


                DocsPaDocumentale_FILENET.Documentale.UserManager userManager = new DocsPaDocumentale_FILENET.Documentale.UserManager();
				oLibrary = userManager.getFileNETLib(idamministrazione.ToString());
				oLibrary.LogonId=dst;
				folder = getRootFolder(oLibrary);

				logger.Debug ("permesso folder ="+folder.GetState(IDMObjects.idmFolderState.idmFolderCanFileIn).ToString());
				string folderName = folder.ID.ToString() + " - " + folder.Name.ToString();
				logger.Debug("Folder: " + folderName);
				cDescr = getClassDocument(oLibrary);
				doc =(IDMObjects.IFnDocumentDual) oLibrary.CreateObject(IDMObjects.idmObjectType.idmObjTypeDocument,cDescr,folder,0,"");

				// set doc id
				IDMObjects.IFnPropertyDual propDocName=doc.Properties[1];
                propDocName.FormatValue();
                propDocName.Value = doc.ID.ToString();

                // set num. max versioni (online limit) custom prop. 
                IDMObjects.IFnPropertyDual propMaxVersion = doc.Properties[3];
                propMaxVersion.FormatValue();
                propMaxVersion.Value = "99";
				

				//non serve retaggio della 2.0
				logger.Debug ("prima di eliminare i permessi");
				//rimuovo i permessi che FN mette di default tipicamente security ad singolo utente
//				int numP = doc.Permissions.Count;
//				for (int i=1; i<= numP; i++) 
//				{
//					doc.Permissions.Remove(doc.Permissions[1].ID);
//				}
				//non c'è bisogno di farlo basta impostare la proprietà:
				//Default Item Access List del gruppo come inserisci security per Gruppo/Admin.
				//doc.Permissions.Add(ConfigurationManager.AppSettings["FNET_userGroup"].ToString(),4);

				logger.Debug ("crea prima versione");
				schedaDoc.systemId = doc.ID.ToString();
				schedaDoc.docNumber = schedaDoc.systemId;
				
				schedaDoc = Versione.addFirstVersione(schedaDoc,doc);

				docNumber=schedaDoc.systemId;
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
		
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la creazione di un documento.", exception);
				docNumber = null;
			}

			return docNumber; 

			#region Codice Originale
			////			string idAmministrazione = objSicurezza.idAmministrazione;
			////			string library = DocsPaDB.Utils.Personalization.getInstance(objSicurezza.idAmministrazione).getLibrary();
			//			
			//			documento = new DocsPaDocumentale.HummingbirdLib.Documento(this.userInfo.dst, library, DocsPaDocumentale.HummingbirdLib.Tipi.ObjectType.Cyd_CmnProjects);
			//
			//			/*
			//			docObj.SetProperty("DOCNAME", "");	
			//			docObj.SetProperty("AUTHOR", schedaDoc.idPeople);	
			//			docObj.SetProperty("AUTHOR_ID", schedaDoc.userId);
			//			docObj.SetProperty("TYPE_ID", schedaDoc.typeId);				
			//			docObj.SetProperty( "TYPIST_ID", schedaDoc.userId);					
			//			docObj.SetProperty("APP_ID", schedaDoc.appId);
			//			docObj.SetProperty( "%VERIFY_ONLY", "%NO" );
			//			*/
			//			documento.DocumentName = "";
			//			documento.Author = schedaDoc.idPeople;
			//			documento.AuthorId = schedaDoc.userId;
			//			documento.TypeId = schedaDoc.typeId;
			//			documento.TypistId = schedaDoc.userId;
			//			documento.AppId = schedaDoc.appId;
			//			documento.VerifyOnly = DocsPaDocumentale.HummingbirdLib.Tipi.VerifyOnlyType.No;
			//
			//			// è necessario inserire anche i permessi per l'utente TYPIST_ID
			//			documento.SetTrustee(schedaDoc.userId, 2, 0);
			//
			//			//docObj.SetTrustee("PRO_DIP1_DIRII_UFF",1,255);
			//			documento.Create();	
			//	
			//			/*
			//			DocsPaWS.Utils.ErrorHandler.checkPCDOperation(docObj,"Errore nella creazione del documento");
			//			*/
			//			if(documento.GetErrorCode() != 0)
			//			{
			//				throw new Exception("Errore nella creazione del documento");
			//			}
			//
			//			
			//			/*
			//			string 	docNumber = docObj.GetReturnProperty("%OBJECT_IDENTIFIER").ToString();
			//			*/
			//			string docNumber = documento.ObjectIdentifier;
			//
			//			//string versionId = docObj.GetReturnProperty("%VERSION_ID").ToString();
			//	
			//			// unlock del documento			
			//			//docObj.SetProperty("%OBJECT_IDENTIFIER", docNumber);		
			//			
			//			/*
			//			docObj.SetProperty("%STATUS", "%UNLOCK");
			//			*/
			//			documento.Status = DocsPaDocumentale.HummingbirdLib.Tipi.StatusType.Unlock;
			//
			//			documento.Update();	
			//	
			//			/*
			//			DocsPaWS.Utils.ErrorHandler.checkPCDOperation(docObj,"Errore nell'update del documento");
			//			*/
			//			if(documento.GetErrorCode() != 0)
			//			{
			//				throw new Exception("Errore nell'update del documento");
			//			}
			//			
			//			
			//
			//			//aggiorna flag daInviare
			//			//string updateStr = "UPDATE VERSIONS SET CHA_DA_INVIARE = '1'";
			//			string firstParam = "CHA_DA_INVIARE = '1'";
			//			
			//			if(schedaDoc.documenti!=null && schedaDoc.documenti.Count>0)
			//			{
			//				
			//				int lastDocNum=schedaDoc.documenti.Count-1;
			//				
			//				
			//				if(((DocsPaVO.documento.Documento)schedaDoc.documenti[lastDocNum]).dataArrivo!=null && !((DocsPaVO.documento.Documento)schedaDoc.documenti[lastDocNum]).dataArrivo.Equals(""))
			//				{
			//					firstParam += ", DTA_ARRIVO =" + DocsPaDbManagement.Functions.Functions.ToDate(((DocsPaVO.documento.Documento)schedaDoc.documenti[lastDocNum]).dataArrivo,false);
			//				}
			//			}
			//			//updateStr +=" WHERE DOCNUMBER=" + docNumber;
			//			//
			//			//db.executeNonQuery(updateStr);
			//			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			//			doc.UpdateVersions(firstParam, docNumber);
			//		
			//			return docNumber; 
			#endregion
		}

		internal static IDMObjects.ClassDescription getClassDocument(IDMObjects.Library oLibrary) 
		{
			return (IDMObjects.ClassDescription)oLibrary.GetObject(IDMObjects.idmObjectType.idmObjTypeClassDesc,"General",IDMObjects.idmObjectType.idmObjTypeDocument,null, null);
		}

		internal static IDMObjects.ClassDescription getClassFolder(IDMObjects.Library oLibrary) 
		{
			return (IDMObjects.ClassDescription)oLibrary.GetObject(IDMObjects.idmObjectType.idmObjTypeClassDesc,"",IDMObjects.idmObjectType.idmObjTypeFolder,"", "");
		}

		internal static IDMObjects.IFnFolderDual getRootFolder(IDMObjects.Library oLibrary) 
		{
			//string pathName = ConfigurationManager.AppSettings["FNET_pathRootFolder"];
			string folderKey = checkFolderCompliance(oLibrary);
			//return(IDMObjects.IFnFolderDual) oLibrary.GetObject(IDMObjects.idmObjectType.idmObjTypeFolder,"/"+folderCompliance[3]+"/"+folderCompliance[1],"",null,null);
			//devi passare la co_key corretta di anno/mese
			return(IDMObjects.IFnFolderDual) oLibrary.GetObject(IDMObjects.idmObjectType.idmObjTypeFolder,folderKey,"",null,null);
		}

		internal static string writeTempFile(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento fileDocumento) 
		{
			string path = ConfigurationManager.AppSettings["FNET_pathServer"];
			string fileName = fileRequest.versionId + "." + Functions.getExt(fileDocumento.fullName);

			string tmpFile = path + "\\" + fileName;
			DocsPaUtils.Functions.Functions.write(tmpFile, fileDocumento);
			return fileName;
		}

		internal static IDMObjects.IFnDocumentDual getDocument(string id, string idamministrazione, string dst) 
		{
			IDMError.ErrorManager idmErrorManager = new IDMError.ErrorManager();
            DocsPaDocumentale_FILENET.Documentale.UserManager userManager = new DocsPaDocumentale_FILENET.Documentale.UserManager();
			IDMObjects.Library oLibrary = userManager.getFileNETLib(idamministrazione);
			oLibrary.LogonId=dst;

			IDMObjects.IFnDocumentDual docFNET = (IDMObjects.IFnDocumentDual) oLibrary.GetObject(IDMObjects.idmObjectType.idmObjTypeDocument, id, oLibrary, null, null);
			return docFNET;
		}


		internal static ArrayList RicercaFT(string testo, string idReg, InfoUtente infoUtente, int numPage, string dst, 
			out int numTotPage, out int nRec)
		{
			logger.Debug("RicercaFT");

			numTotPage=0;
			nRec=0;
						
			ArrayList resultSet = new ArrayList();
            DocsPaDocumentale_FILENET.Documentale.UserManager userManager = new DocsPaDocumentale_FILENET.Documentale.UserManager();
			IDMObjects.Library oLibrary = userManager.getFileNETLib(infoUtente.idAmministrazione);
			// oLibrary.LogonId = dst;

			DocsPaWrapperFilenet.FilenetFullTextClass fullTextClass  = new DocsPaWrapperFilenet.FilenetFullTextClass();
			
			Array docNumbers = fullTextClass.RicercaFullText(testo, oLibrary.Name, "Admin", "");
			if (docNumbers==null || docNumbers.Length == 0)
				return null;
			else
			{
				for (int i=0; i<=docNumbers.Length-1; i++)
				{
					resultSet.Add(Int32.Parse(docNumbers.GetValue(i).ToString()));				
				}
			}
			
			int numDocs = resultSet.Count;
			string[] resultKey= GetResultKeys(resultSet, numPage, numDocs);

			resultSet= GetListaDocumenti(resultKey, idReg, infoUtente);
			
			numTotPage=(numDocs / PAGE_SIZE);

			if(numTotPage * PAGE_SIZE < numDocs)
				numTotPage++;

			nRec=numDocs;

			return resultSet;
		}

		private static ArrayList GetListaDocumenti(string[] resultKeys,
			string idRegistro,
			InfoUtente infoUtente)
		{
			DocsPaDB.Query_DocsPAWS.RicercaFullText ricercaFT=new DocsPaDB.Query_DocsPAWS.RicercaFullText();
			return ricercaFT.GetDocumentiFilenet(resultKeys,infoUtente);
		}

		private static string[] GetResultKeys(ArrayList docNumbers,int requestedPage,int numDocs)
		{
			ArrayList arrayList=new ArrayList();

			if (numDocs>0)
			{
				int initPosition=((requestedPage * PAGE_SIZE) - PAGE_SIZE);

				for(int i=initPosition; i<(initPosition + PAGE_SIZE); i++)
				{
					if (i <= docNumbers.Count-1) 
						arrayList.Add(docNumbers[i]);
					else
						break;
				}
			} 

			string[] retValue=new string[arrayList.Count];
			for(int i=0; i<=arrayList.Count-1; i++)
				retValue[i]=arrayList[i].ToString();

			return retValue;
		}

		#endregion

		#region nuovi metodi filenet per la gestione dei folder
		
		///metodo di indagine gestione folder
		///
		public static string checkFolderCompliance(IDMObjects.Library oLibrary)
		{
			string folderKey = "";
				
			try
			{
				// verifico esistenza
				folderKey = checkFolder();
				if (folderKey!="")
				{
					return folderKey;
				}
				else
				{
					//devo creare la struttura
					string yearFolderKey = checkYearFolder(oLibrary);
					folderKey = checkMonthFolder(oLibrary,yearFolderKey);
					return folderKey;
				}
				
			}
			catch (Exception ex)
			{
				logger.Debug("Errore Verifica esistenza Folder:", ex);
				return folderKey; 
			}

		}

		public static string checkFolder()
		{
			string infoFolder = "";
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
			
			try
			{
				DocsPaUtils.Query query=DocsPaUtils.InitQuery.getInstance().getQuery("S_FN_GET_FOLDER_CUSTOM");
				string commandText=query.getSQL();
				System.Diagnostics.Debug.WriteLine("SQL - checkFolderFN - QUERY : "+commandText);
				DataSet ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);
				if (ds.Tables[0].Rows.Count != 0)
				{	
					//co_key mese
					infoFolder = ds.Tables[0].Rows[0][0].ToString();
				}
				return infoFolder;
			}
			catch (Exception ex)
			{
				logger.Debug("Errore Verifica esistenza Folder:", ex);
				return infoFolder;
			}
			finally
			{
				dbProvider.Dispose();
			}

		}

		private static string checkYearFolder(IDMObjects.Library oLibrary)
		{
			string infoFolder = "";
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
			try
			{
				DocsPaUtils.Query query=DocsPaUtils.InitQuery.getInstance().getQuery("S_FN_GET_YEAR_FOLDER_CUSTOM");
				string commandText=query.getSQL();
				System.Diagnostics.Debug.WriteLine("SQL - checkYearFolderFN - QUERY : "+commandText);
				DataSet ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);
				if (ds.Tables[0].Rows.Count != 0)
				{	
					//co_key anno
					infoFolder = ds.Tables[0].Rows[0][0].ToString();
				}
				else
				{
					// la devo creare
					string year = DateTime.Now.Year.ToString();
					IDMObjects.IFnFolderDual folder;
					folder = (IDMObjects.IFnFolderDual)oLibrary.CreateObject(IDMObjects.idmObjectType.idmObjTypeFolder,"",System.Reflection.Missing.Value,System.Reflection.Missing.Value,System.Reflection.Missing.Value);
					folder.Name = year;
					folder.SaveNew(IDMObjects.idmSaveNewOptions.idmDocSaveNewNoKeep);
					infoFolder = folder.ID.ToString();
				}
				return infoFolder;				
			}
			
			catch (Exception ex)
			{

				logger.Debug("Errore Verifica esistenza Folder Anno:", ex);
				return infoFolder;
			}
			finally
			{
				dbProvider.Dispose();
			}

		}

		private static string checkMonthFolder(IDMObjects.Library oLibrary,string yearFolderKey)
		{
			string infoFolder = "";
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
			try
			{
				DocsPaUtils.Query query=DocsPaUtils.InitQuery.getInstance().getQuery("S_FN_GET_MONTH_FOLDER_CUSTOM");
				query.setParam("yearFolderKey",yearFolderKey);
				string commandText=query.getSQL();
				System.Diagnostics.Debug.WriteLine("SQL - checkMonthFolderFN - QUERY : "+commandText);
				DataSet ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);
				if (ds.Tables[0].Rows.Count != 0)
				{	
					//co_key anno
					infoFolder = ds.Tables[0].Rows[0][0].ToString();
				}
				else
				{
					//la devo creare
					string month = DateTime.Now.Month.ToString();
					//padre
					IDMObjects.IFnFolderDual folderParent;
					//figlio
					IDMObjects.IFnFolderDual folder;
					//prelevo il folder del padre
					folderParent = (IDMObjects.IFnFolderDual) oLibrary.GetObject(IDMObjects.idmObjectType.idmObjTypeFolder,yearFolderKey,"",null,null);
					// creo il subfolder partendo dal padre
					folder=folderParent.CreateSubFolder(month,System.Reflection.Missing.Value);
					folder.SaveNew(IDMObjects.idmSaveNewOptions.idmDocSaveNewNoKeep);
					infoFolder = folder.ID.ToString();
					
				}
				return infoFolder;
			}
			
			catch (Exception ex)
			{

				logger.Debug("Errore Verifica esistenza Folder Mese:", ex);
				return infoFolder;
			}
			finally
			{
				dbProvider.Dispose();
			}


		}


		#endregion

		

	}
}
