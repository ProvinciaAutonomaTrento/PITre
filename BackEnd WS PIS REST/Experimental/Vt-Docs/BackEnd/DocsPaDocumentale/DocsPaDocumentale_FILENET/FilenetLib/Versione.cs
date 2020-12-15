using System;
using System.Collections;
using System.Configuration;
using DocsPaDocumentale_FILENET.FilenetLib;
using log4net;


namespace DocsPaDocumentale_FILENET.FilenetLib
{
	/// <summary>
	/// Summary description for Versione.
	/// </summary>
	public class Versione
	{
        private static ILog logger = LogManager.GetLogger(typeof(Versione));
		internal static DocsPaVO.documento.SchedaDocumento addFirstVersione(DocsPaVO.documento.SchedaDocumento schedaDocumento, IDMObjects.IFnDocumentDual IDMdoc) 
		{
			DocsPaVO.documento.Documento versione = null;
			logger.Debug ("sono in add first versione");
			if (schedaDocumento.documenti == null) 
				schedaDocumento.documenti = new ArrayList();

			if (schedaDocumento.documenti.Count == 0) 
				versione = new DocsPaVO.documento.Documento();
			else 
				versione = (DocsPaVO.documento.Documento)schedaDocumento.documenti[0];

			versione.docNumber = schedaDocumento.docNumber;

			// gestione dei firmatari
			setFirmatari( versione);

			versione.fileSize = "0";
			if (versione.daInviare != null  && versione.daInviare.Length > 0)
				versione.daInviare = "1";
			
			//string nomeFile = ConfigurationManager.AppSettings["FNET_pathServer"] + ConfigurationManager.AppSettings["FNET_nomeFileTemp"];
           

            logger.Debug("prima di SaveNew ");
            // logger.Debug("chiamata saveNew con file in path: " + nomeFile);

            //file tmp dinamico costruito on line su percorso standard
            string nomeFile = ConfigurationManager.AppSettings["FNET_pathServer"] + schedaDocumento.idPeople + "_void.tmp";

            try
            {
                // verifico preesistenza
                if (System.IO.File.Exists(nomeFile))
                    System.IO.File.Delete(nomeFile);

                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(nomeFile))
                {
                    writer.WriteLine("  ");
                    writer.Flush();
                    writer.Close();
                }

                IDMdoc.SaveNew(nomeFile,
                    IDMObjects.idmSaveNewOptions.idmDocSaveNewKeep | IDMObjects.idmSaveNewOptions.idmDocSaveNewIndexContent);


                logger.Debug("chiamata saveNew con file in path: " + nomeFile + " esisto OK");
                bool canCheckout = IDMdoc.GetState(IDMObjects.idmDocState.idmDocCanCheckout);
                logger.Debug("dopo di GetState canCheckout su doc di saveNew id:" + IDMdoc.ID);
                bool canCheckin = IDMdoc.GetState(IDMObjects.idmDocState.idmDocCanCheckin);
                logger.Debug("dopo di GetState canCheckin su doc di saveNew id:" + IDMdoc.ID);
                versione.versionId = getFNETnumVers("1") + schedaDocumento.docNumber;
                logger.Debug("dopo di versionID creata: " + versione.versionId);
                if (schedaDocumento.documenti.Count == 0)
                    schedaDocumento.documenti.Add(versione);
                else
                    schedaDocumento.documenti[0] = versione;

                updateVers(versione);

                return schedaDocumento;
            }
            catch (Exception ex)
            {

                IDMError.ErrorManager idmErrorManager = new IDMError.ErrorManager();
                logger.Debug(ex.Message);
                string msg = ex.Message;
                for (int i = 0; i < idmErrorManager.Errors.Count; i++)
                    msg += " " + idmErrorManager.Errors[i].Description;
                logger.Debug(msg);
                return null;
            }
            finally
            {
                // verifico preesistenza
                if (System.IO.File.Exists(nomeFile))
                    System.IO.File.Delete(nomeFile);
            }
		}

		internal static void setFirmatari(DocsPaVO.documento.FileRequest fileRequest) 
		{
			if(fileRequest.firmatari != null && fileRequest.firmatari.Count>0)
			{
				DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
				doc.UpdateFirmatari(fileRequest);
			}
		}

		internal static DocsPaVO.documento.FileRequest updateVers(DocsPaVO.documento.FileRequest fileRequest) 
		{
			string tipoFile = "0";
				
			if(fileRequest.GetType().Equals(typeof(DocsPaVO.documento.Allegato)))
			{
				fileRequest.versionLabel = "A" + fileRequest.version;
				tipoFile = "1";
			}
			else 
			{
				fileRequest.versionLabel = fileRequest.version;
			}

			if (fileRequest.descrizione == null) fileRequest.descrizione="";
			fileRequest.version=getNumVersione(fileRequest);

			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			string docnumber= getFNETename(fileRequest.docNumber);
			// v_e_name = doc.F_GetFilenetNumVersione( docnumber, fileRequest.version);

			DocsPaDB.Query_DocsPAWS.Documenti docF = new DocsPaDB.Query_DocsPAWS.Documenti();
			docF.F_UpdateVersion(fileRequest, tipoFile);

			return fileRequest;
		}

		internal static string getNumVersione(DocsPaVO.documento.FileRequest fileRequest) 
		{
			string allegato = "0";
			if(fileRequest.GetType().Equals(typeof(DocsPaVO.documento.Allegato)))
				allegato = "1";

			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			string docnumberFNET = getFNETename(fileRequest.docNumber);
			string numVers = doc.F_GetNumVersione(docnumberFNET, allegato);

			if (numVers==null || numVers.Length==0)
				return "1";

			int numVer = Int32.Parse(numVers) + 1;
			return numVer.ToString();
		}

		internal static string  getFNETnumVers(string ver) 
		{
			int lun = ver.Length;
			for (int i=lun; i<4; i++)
				ver= "0" + ver;
			ver = ver + " 00";
			return ver;
		}


		internal static DocsPaVO.documento.FileRequest createVersionFNET(DocsPaVO.documento.FileRequest fileRequest, string idamministrazione, string dst) 
		{
			string nomeFile = getDefaultFile();
			createVersionFNET(nomeFile, ref fileRequest, idamministrazione, dst );
			return fileRequest;			
		}

		internal static string createVersionFNET(string nomeFile, ref DocsPaVO.documento.FileRequest fileRequest, string idamministrazione, string dst ) 
		{
			IDMError.ErrorManager idmErrorManager = new IDMError.ErrorManager();
			try 
			{
				string path = ConfigurationManager.AppSettings["FNET_pathServer"];

                DocsPaDocumentale_FILENET.Documentale.UserManager userManager = new DocsPaDocumentale_FILENET.Documentale.UserManager();	
				IDMObjects.Library oLibrary = userManager.getFileNETLib(idamministrazione); 

				oLibrary.LogonId=dst;
				IDMObjects.IFnFolderDual folder = DocumentManagement.getRootFolder(oLibrary);
				IDMObjects.IFnDocumentDual docFNET = (IDMObjects.IFnDocumentDual) oLibrary.GetObject(IDMObjects.idmObjectType.idmObjTypeDocument,fileRequest.docNumber,oLibrary, null, null);
				bool canCheckout = docFNET.GetState(IDMObjects.idmDocState.idmDocCanCheckout);
				
				if (canCheckout)
					docFNET.Version.CheckoutNoCopy();
				else
					throw new Exception("Il file è utilizzato da un altro processo");
				
				bool canCheckin = docFNET.GetState(IDMObjects.idmDocState.idmDocCanCheckin);

				docFNET.Version.Checkin(path, nomeFile, IDMObjects.idmCheckin.idmCheckinKeep);

				IDMObjects.Version vers = docFNET.Version;
				bool canIndex = docFNET.GetState(IDMObjects.idmDocState.idmDocCanIndex);
                if (canIndex)
                    vers.IndexContent(IDMObjects.idmIndex.idmIndexContent);



				string numVers = getFNETnumVers(docFNET.Version.Number); 
				//OLD: fileRequest.versionId =  numVers +	getFNETename(fileRequest.docNumber);
				fileRequest.fNversionId =  numVers +	getFNETename(fileRequest.docNumber);
				return numVers;
			} 
			catch(Exception e) 
			{
				string msg = e.Message;		
				for (int i=0; i < idmErrorManager.Errors.Count; i++)
					msg += " " + idmErrorManager.Errors[i].Description;
				throw new Exception(msg);
			}
		}
		internal static string getFNETename(string docnumber) 
           	{
			if (docnumber.Length >= 9) return docnumber;
			string s=new String(Convert.ToChar("0"),9-docnumber.Length);
			return s+docnumber;

		}
		internal static string getDefaultFile() 
		{
			string path = ConfigurationManager.AppSettings["FNET_pathServer"];
			string fileName = ConfigurationManager.AppSettings["FNET_nomeFileTemp"];
			string fullName = path + "\\" + fileName;
			if(!System.IO.File.Exists(fullName)) 
			{
				logger.Debug("Scrittura file vuoto: " + fullName);
				System.IO.File.Create(fullName);
			}
			return fileName;
		}

		internal void remove(DocsPaVO.documento.FileRequest fileReq, string idamministrazione, string versFNET,string dst) 
		{
			// int numVersione = getNumVers(fileReq.versionId);
			int numVersione = getNumVers(versFNET);
            
           
			string strNumVersione=getFNETnumVers(numVersione.ToString());
			IDMError.ErrorManager idmErrorManager = new IDMError.ErrorManager();
            DocsPaDocumentale_FILENET.Documentale.UserManager userManager = new DocsPaDocumentale_FILENET.Documentale.UserManager();
			IDMObjects.Library oLibrary = userManager.getFileNETLib(idamministrazione);
			oLibrary.LogonId=dst;
			IDMObjects.IFnDocumentDual docFNET = (IDMObjects.IFnDocumentDual) oLibrary.GetObject(IDMObjects.idmObjectType.idmObjTypeDocument,fileReq.docNumber,oLibrary, null, null);
			IDMObjects.IFnDocumentDual verFNET = (IDMObjects.IFnDocumentDual) oLibrary.GetObject(IDMObjects.idmObjectType.idmObjTypeDocument,fileReq.docNumber + ":" + strNumVersione,oLibrary, null, null);
			try 
			{
                //TODO: 04/06/2007  (A.B.)
                //attualmente viene de-indicizzato il file acquisito sulla versione e/o allegato
                // non viene ne eliminata la versione Filenet ne eliminato il file vero è proprio
                //in quanto se rimosso viene messo off line la versione e nascono problemi 
                //per eventuali versioni successive.
                docFNET.Version.IndexContent(IDMObjects.idmIndex.idmIndexContentDeindex);
  
                #region testing filenet
                //docFNET.Version.Series.Remove(numVersione);
                //docFNET.Version.Archive(IDMObjects.idmArchive.idmArchiveDispose);
				//string ArchivioFile = DocsPaDocumentale_FILENET.FilenetLib.DocumentManagement.checkFolderCompliance(oLibrary);
				//verFNET.Version.ArchiveEx(String.Empty);
                //removeVers(fileReq.versionId);
                //removeVers(versFNET);
                //docFNET.Version.Archive(IDMObjects.idmArchive.idmArchiveDispose);
                #endregion

            } 
			catch(Exception e) 
			{
				string msg = e.Message;		
				for (int i=0; i < idmErrorManager.Errors.Count; i++)
					msg += " " + idmErrorManager.Errors[i].Description;
				throw new Exception(msg);
			}
		}

		internal int getNumVers(string systemId) 
		{
			int result;
			try
			{
				result = DocsPaUtils.Functions.Functions.toInt(systemId.Substring(0, systemId.IndexOf(" ")));
			}
			catch
			{
				result=Int32.Parse(systemId);
			}
			return result;
		}

		internal void removeVers(string versionId)
		{
			DocsPaDB.Query_DocsPAWS.Documenti queryVersion = new DocsPaDB.Query_DocsPAWS.Documenti();
			if ( ! queryVersion.F_DeleteVersion(versionId)) throw new Exception("Errore nella eliminazione della versione");
		}

		internal void unlock(DocsPaVO.documento.FileRequest fileReq, string idamministrazione) 
		{
			int numVersione = getNumVers(fileReq.versionId);
            DocsPaDocumentale_FILENET.Documentale.UserManager userManager = new DocsPaDocumentale_FILENET.Documentale.UserManager();
			IDMObjects.Library oLibrary = userManager.getFileNETLib(idamministrazione);
			IDMObjects.IFnDocumentDual docFNET = (IDMObjects.IFnDocumentDual) oLibrary.GetObject(IDMObjects.idmObjectType.idmObjTypeDocument,fileReq.docNumber,oLibrary, null, null);
			bool canCheckin = docFNET.GetState(IDMObjects.idmDocState.idmDocCanCheckin);
			if(canCheckin)
				docFNET.Version.CancelCheckout(IDMObjects.idmCancelCheckout.idmCancelCheckoutNoKeep);
		}

		internal static string setDocnumber2Filenet(string docnumber, string versionId)
		{
			return getFNETename(docnumber)+":"+Convert.ToInt32(versionId.Substring(0,4));
		}

		internal bool VersioneIsAcquisito(string docnumber, string versionid)
		{
			
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			return doc.F_VersioneISAcquisito(docnumber, versionid);
		}

	}
}
