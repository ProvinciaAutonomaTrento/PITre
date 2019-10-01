using System;
using log4net;
using System.Collections.Generic;

namespace BusinessLogic.Documenti
{
	/// <summary>
	/// Classe per la gestione dei documenti firmati
	/// </summary>
	public class SignedFileManager
	{
        private static ILog logger = LogManager.GetLogger(typeof(SignedFileManager));
		/// <summary>
		/// Aggiunta di un file firmato ad un documento esistente
		/// </summary>
		/// <param name="base64content"></param>
		/// <param name="cofirma"></param>
		/// <param name="fileRequest"></param>
		/// <param name="infoUtente"></param>
		/// <returns></returns>
		public static bool AppendDocumentoFirmato(
								string base64content,
								bool cofirma,
								ref DocsPaVO.documento.FileRequest fileRequest, 
								DocsPaVO.utente.InfoUtente infoUtente)
		{
           return  AppendDocumentoFirmato(Convert.FromBase64String(base64content),cofirma, ref fileRequest, infoUtente,false);
		}


        
        /// <summary>
        /// Aggiunta di un file firmato ad un documento esistente
        /// </summary>
        /// <param name="base64content"></param>
        /// <param name="cofirma"></param>
        /// <param name="fileRequest"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static bool AppendDocumentoFirmato(
                                byte[] signedContent,
                                bool cofirma,
                                ref DocsPaVO.documento.FileRequest fileRequest,
                                DocsPaVO.utente.InfoUtente infoUtente)
        {
            return AppendDocumentoFirmato(signedContent , cofirma, ref fileRequest, infoUtente, false);
        }


        public static bool AppendDocumentoFirmatoPades(
                            byte[] signedContent,
                            bool cofirma,
                            ref DocsPaVO.documento.FileRequest fileRequest,
                            DocsPaVO.utente.InfoUtente infoUtente, bool isConvertedToPdf = false)
        {
            return AppendDocumentoFirmato(signedContent, cofirma, ref fileRequest, infoUtente, true, isConvertedToPdf);
        }


        /// <summary>
        /// Aggiunta di un file firmato ad un documento esistente
        /// </summary>
        /// <param name="base64content"></param>
        /// <param name="cofirma"></param>
        /// <param name="fileRequest"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        private static bool AppendDocumentoFirmato(
                                byte[] signedContent,
                                bool cofirma,
                                ref DocsPaVO.documento.FileRequest fileRequest,
                                DocsPaVO.utente.InfoUtente infoUtente,bool isPades, bool isConvertedToPdf = false)
        {
            bool retValue = true;

            DocsPaVO.documento.FileRequest fileRequest_old = (DocsPaVO.documento.FileRequest)fileRequest.Clone();

            try
            {
                if (fileRequest.repositoryContext == null)
                {
                    // Verifica stato di consolidamento del documento, solamente se non si sta firmando nel repository context
                    DocumentConsolidation.CanExecuteAction(infoUtente,
                            fileRequest.docNumber,
                            DocumentConsolidation.ConsolidationActionsDeniedEnum.SignDocument,
                            true);
                }
                //luca
                if (fileRequest != null && !String.IsNullOrEmpty(fileRequest.fileName) && (fileRequest.fileName.ToLower().EndsWith("pdf_convertito") || isConvertedToPdf))
                {
                    fileRequest.fileName = System.IO.Path.GetFileNameWithoutExtension(fileRequest.fileName) + ".pdf";
                    isConvertedToPdf = true;
                }
                //end luca
                // Verifica se il formato del file è accettato per la firma
                if (!IsFormatSupportedForSign(infoUtente, fileRequest))
                    throw new ApplicationException("Formato file non supportato per la firma");

                DocsPaVO.documento.Applicazione app = new DocsPaVO.documento.Applicazione();
                DocsPaVO.documento.FileDocumento fileDoc = new DocsPaVO.documento.FileDocumento();

                fileDoc.content = signedContent;
                fileDoc.length = fileDoc.content.Length;
                string nomeOriginale = BusinessLogic.Documenti.FileManager.getOriginalFileName(infoUtente, fileRequest);

                if (isPades)
                {
                    // INC000001085991 PITRE - conversione in pdf su firma pades non aggiorna l'estensione
                    // fileDoc.nomeOriginale = nomeOriginale ;
                    if (!string.IsNullOrEmpty(nomeOriginale))
                    {
                        //se il filename finisce PDF probabilmente è stato convertito.
                        //controllo inoltre se il nomeoriginale non finisce con PDF, in tal caso lo popolo.
                        if ((System.IO.Path.GetExtension(fileRequest.fileName).ToUpper() == ".PDF") && 
                            (System.IO.Path.GetExtension(nomeOriginale).ToUpper() != ".PDF") && isConvertedToPdf)
                            nomeOriginale += ".PDF";

                        fileDoc.nomeOriginale = nomeOriginale;
                    }

                    fileDoc.estensioneFile = GetAppSuffix(fileRequest.fileName);
                    fileDoc.name = fileRequest.fileName;
                    app.estensione = GetAppSuffix(fileRequest.fileName);
                }
                else
                {
                    if (cofirma && System.IO.Path.GetExtension(nomeOriginale).ToUpper().Equals(".P7M"))
                    {
                        app.estensione = GetAppSuffix(fileRequest.fileName);
                        fileDoc.name = fileRequest.fileName;



                        if (!string.IsNullOrEmpty(nomeOriginale))
                        {
                            //se il filename finisce PDF probabilmente è stato convertito.
                            //controllo inoltre se il nomeoriginale non finisce con PDF, in tal caso lo popolo.
                            if ((System.IO.Path.GetExtension(fileRequest.fileName).ToUpper() == ".PDF") &&
                                (System.IO.Path.GetExtension(nomeOriginale).ToUpper() != ".PDF"))
                                nomeOriginale += ".PDF";

                            fileDoc.nomeOriginale = nomeOriginale;
                        }
                        fileDoc.estensioneFile = GetAppSuffix(fileRequest.fileName);
                    }
                    else
                    {
                        app.estensione = GetAppSuffix(fileRequest.fileName + ".P7M");
                        fileDoc.name = fileRequest.fileName + ".P7M";

                        //luca
                        if (isConvertedToPdf)
                        {
                            //ATTENZIONE, Se un giorno attiveremo la firma pades lato client questo codice non andrà più bene
                            if (!string.IsNullOrEmpty(nomeOriginale))
                                nomeOriginale = nomeOriginale + ".PDF";
                        }
                        // end luca
                        if (!string.IsNullOrEmpty(nomeOriginale))
                        {
                            //se il filename finisce PDF probabilmente è stato convertito.
                            //controllo inoltre se il nomeoriginale non finisce con PDF, in tal caso lo popolo.
                            if ((System.IO.Path.GetExtension(fileRequest.fileName).ToUpper() == ".PDF") &&
                                (System.IO.Path.GetExtension(nomeOriginale).ToUpper() != ".PDF"))
                                nomeOriginale += ".PDF";

                            fileDoc.nomeOriginale = nomeOriginale + ".P7M";
                        }
                        fileDoc.estensioneFile = GetAppSuffix(fileRequest.fileName + ".p7m");
                    }
                }
                fileDoc.fullName = fileDoc.name;

                bool addNewAttatchment = false;
                bool isAllegato = (fileRequest.GetType().Equals(typeof(DocsPaVO.documento.Allegato)));

                if (isAllegato && fileRequest.repositoryContext == null)
                {
                    // La firma digitale per l'allegato viene fatta solamente se il documento già esiste su database

                    // Se è attiva la gestione di profilazione degli allegati,
                    // deve essere aggiunta una nuova versione del documento.
                    // Altrimenti, deve essere creato un nuovo allegato.
                    addNewAttatchment = (!BusinessLogic.Documenti.AllegatiManager.isEnabledProfilazioneAllegati());
                }

                if (addNewAttatchment)
                {
                    fileRequest.docNumber = BusinessLogic.Documenti.AllegatiManager.getIdDocumentoPrincipale((DocsPaVO.documento.Allegato)fileRequest);
                    fileRequest.descrizione = "Versione firmata";
                    fileRequest.cartaceo = false;
                    fileRequest = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, (DocsPaVO.documento.Allegato)fileRequest);

                    if (fileRequest == null)
                        throw new ApplicationException("Errore nella creazione dell'allegato firmato per il documento");
                }
                else
                {
                    fileRequest.applicazione = app;
                    fileRequest.versionId = "";
                    fileRequest.descrizione = "Versione firmata";
                    fileRequest.cartaceo = false;
                    fileRequest = BusinessLogic.Documenti.VersioniManager.addVersion(fileRequest, infoUtente, false);

                    if (fileRequest == null)
                        throw new ApplicationException("Errore nella creazione della versione firmata per il documento");

                    bool setDataFirma = BusinessLogic.Documenti.DocManager.SetDataFirmaDocumento(fileRequest.docNumber, fileRequest.versionId);

                    if (!isAllegato)
                        ((DocsPaVO.documento.Documento)fileRequest).daInviare = "1";
                }

                //PDZ devo sapere prima di fare la put se è firmato elettronicamente per la gestione tipo firma mista
                List<DocsPaVO.LibroFirma.FirmaElettronica> firmaE = LibroFirma.LibroFirmaManager.GetFirmaElettronicaDaFileRequest(fileRequest_old);
                bool isFirmatoElettonicamente = firmaE != null && firmaE.Count > 0;
                fileRequest.tipoFirma = isFirmatoElettonicamente ? DocsPaVO.documento.TipoFirma.ELETTORNICA : fileRequest.tipoFirma;

                BusinessLogic.Documenti.FileManager.putFile(fileRequest, fileDoc, infoUtente, false);

                //ABBATANGELI GIANLUIGI - FIRMA ELETTRONICA

                if (isFirmatoElettonicamente)
                {
                    string impronta = string.Empty;
                    DocsPaDB.Query_DocsPAWS.Documenti docInfoDB = new DocsPaDB.Query_DocsPAWS.Documenti();
                    docInfoDB.GetImpronta(out impronta, fileRequest.versionId, fileRequest.docNumber);

                    foreach (DocsPaVO.LibroFirma.FirmaElettronica firma in firmaE)
                    {
                        firma.UpdateXml(impronta, fileRequest.versionId, fileRequest.version);
                        LibroFirma.LibroFirmaManager.InserisciFirmaElettronica(firma);
                    }
                }
                //FINE
            }
            catch (Exception e)
            {
                logger.Debug(e.ToString());
                logger.Debug("Errore nella gestione della verifica firma (verificaFirmaMethod)", e);

                retValue = false;
            }

            return retValue;
        }

        /// <summary>
        /// Verifica se il formato del file è ammesso per la firma digitale
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        private static bool IsFormatSupportedForSign(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.FileRequest fileRequest)
        {
            bool retValue = false;

            if (!FormatiDocumento.Configurations.SupportedFileTypesEnabled)
            {
                retValue = true;
            }
            else
            {
                string extension = System.IO.Path.GetExtension(fileRequest.fileName);

                if (!string.IsNullOrEmpty(extension))
                {
                    // Rimozione del primo carattere dell'estensione (punto)
                    extension = extension.Substring(1);

                    DocsPaVO.FormatiDocumento.SupportedFileType fileType = BusinessLogic.FormatiDocumento.SupportedFormatsManager.GetFileType(Convert.ToInt32(infoUtente.idAmministrazione), extension);

                    retValue = (fileType != null && fileType.FileTypeUsed && fileType.FileTypeSignature);
                }
            }

            return retValue;
        }

		/// <summary>
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		private static string GetAppSuffix(string filename)
		{
			logger.Debug("getApp");
			char[] dot={'.'};
			string[] parts=filename.Split(dot);
			string suffix=parts[parts.Length-1];
			if(suffix.ToUpper().Equals("P7M"))
			{
				string res="";
				int index=1;
				while(suffix.ToUpper().Equals("P7M"))
				{
					index=index+1;
					res=".P7M"+res;
					suffix=parts[parts.Length-index];
				}
				res=suffix+res;
				return res;
			}
			else
			{
				return suffix;
			}
		}
	}
}
