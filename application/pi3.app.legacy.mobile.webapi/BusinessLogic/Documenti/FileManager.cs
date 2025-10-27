// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using BusinessLogic.Documenti.DigitalSignature;
using DocsPaVO.documento;
using DocsPaVO.utente;
using Microsoft.AspNetCore.Http;
using Pi3.Infrastructure.Tibco.Services.File.FirmaRemota;
using Pi3.Infrastructure.Tibco.Services.File.FirmaRemota2;
using Serilog;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace BusinessLogic.Documenti;

public class FileManager
{
	class PdfInfo
	{
		public string version;
		public bool IsSigned;
		public bool IsPdfA;
		public bool HasJava;
		public string conformance;
		public bool HasBiometricData;

	}

    private static ILogger logger = Log.ForContext(typeof(FileManager));

    private static bool GestioneTSAttacced = true;
	private static bool GestionePades = true;


	public static DocsPaVO.documento.FileRequest putFile(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento fileDoc, DocsPaVO.utente.InfoUtente objSicurezza, string repositoryRootPath = "", string temporaryRootPath = "")
	{
		return putFile(fileRequest, fileDoc, objSicurezza, true, repositoryRootPath, temporaryRootPath);
	}

	public static DocsPaVO.documento.FileRequest putFile(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento fileDoc, DocsPaVO.utente.InfoUtente objSicurezza, bool verifyFileFormat, string repositoryRootPath = "", string temporaryRootPath = "")
	{
		DocsPaVO.documento.FileRequest retValue = fileRequest;

		string errorMessage;

		if (!putFile(ref retValue, fileDoc, objSicurezza, verifyFileFormat, out errorMessage, repositoryRootPath, temporaryRootPath))
			throw new ApplicationException(errorMessage);

		return retValue;
	}

	public static bool putFile(ref DocsPaVO.documento.FileRequest fileRequest,
								DocsPaVO.documento.FileDocumento fileDoc,
								DocsPaVO.utente.InfoUtente objSicurezza,
								bool verifyFileFormat,
								out string errorMessage, string repositoryRootPath = "", string temporaryRootPath = "", bool processFileInfo = true)
	{
		logger.Information("BEGIN");
		bool retValue = true;
		errorMessage = string.Empty;
		String tipoFirma = DocsPaVO.documento.TipoFirma.NESSUNA_FIRMA;
		bool signTypeChecked = false;

		if (fileRequest.repositoryContext == null)
		{
			// Verifica stato di consolidamento del documento, solamente se non si sta acquisendo il file nel repository context
			retValue = DocumentConsolidation.CanExecuteAction(objSicurezza, fileRequest.docNumber, DocumentConsolidation.ConsolidationActionsDeniedEnum.ModifyVersions);
		}

		if (!retValue)
		{
			errorMessage = "Il documento risulta in stato consolidato, impossibile acquisire il file";
		}
		else
		{
			//controllo se doc in cestino
			string incestino = string.Empty;
			if (fileRequest != null && !string.IsNullOrEmpty(fileRequest.docNumber))
				incestino = BusinessLogic.Documenti.DocManager.checkdocInCestino(fileRequest.docNumber);

			//Verifico se non posso acquisire perchè è attivo un processo di firma per il documento principale o allegato e non sono il titolare del passo in esecuzione
			if (!fileRequest.conSegnaturaPermanente && !LibroFirma.LibroFirmaManager.CanExecuteAction(fileRequest, objSicurezza))
			{
				errorMessage = "il documento principale o l'allegato è in libro firma.";
				throw new Exception("Non è possibile acquisire poichè il documento principale o l'allegato è in libro firma");
			}

			if (!string.IsNullOrEmpty(incestino) && incestino == "1")
				throw new Exception("Il documento è stato rimosso, non è più possibile modificarlo");

			if (retValue)
			{
				//tolgo nel nome del file i caratteri accentati
				string nameWithoutAccentedString = ConvertAccentedString(fileDoc.name);
				fileDoc.name = nameWithoutAccentedString;
				fileRequest.fileName = nameWithoutAccentedString;

				//fix per il salvataggio nella components
				if (String.IsNullOrEmpty(fileDoc.fullName))
					fileDoc.fullName = fileDoc.name;


				//Gestione base64
				if (fileDoc.name.ToUpper().EndsWith("P7M"))
				{
					byte[] deb64Content = readBase64(fileDoc.content);
					if (deb64Content != null)
					{
						fileDoc.content = deb64Content;
						fileDoc.length = fileDoc.content.Length;
					}
				}

				// Verifica se il file è acquisito nell'ambito di un repository di sessione
				if (fileRequest.repositoryContext != null)
				{
					bool isProtocollo = false;

					// Determina la tipologia del documento 
					if (fileRequest is DocsPaVO.documento.Documento)
						isProtocollo = (!fileRequest.repositoryContext.IsDocumentoGrigio);
					else if (fileRequest is DocsPaVO.documento.Allegato)
						// L'allegato è sempre un documento grigio
						isProtocollo = false;

					// Verifica se il formato documento è tra quelli accettati dall'amministrazione
					if (verifyFileFormat)
						retValue = IsFileAccepted(objSicurezza, isProtocollo, fileDoc, out errorMessage, processFileInfo);

					if (retValue)
					{
						SessionRepositoryFileManager fileManager = SessionRepositoryFileManager.GetFileManager(fileRequest.repositoryContext);

						// Inserimento di un file in un documento quando ancora non è stato salvato,
						// pertanto è disponibile un repository temporaneo valido solamente nell'ambito dell'inserimento
						fileManager.SetFile(fileRequest, fileDoc);

						// Aggiornamento oggetto FileRequest
						fileRequest.fileName = fileDoc.name;
						fileRequest.fileSize = fileDoc.content.Length.ToString();
						fileRequest.subVersion = "A";
						fileRequest.path = fileDoc.path;
					}
				}
				else
				{
					bool scanned = isScannedDocument(fileDoc);
					if (scanned)
						fileDoc.name = setFileNameForScanneddocuments(objSicurezza, fileRequest, fileDoc);

					// Verifica se il formato documento è tra quelli accettati dall'amministrazione
					if (verifyFileFormat)
						retValue = IsFileAccepted(objSicurezza, fileRequest.docNumber, fileDoc, out errorMessage, processFileInfo);

					if (retValue)
					{
						// Creazione contesto transazionale
						using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
						{
							//controllo se il file è già stato acquistito per gestire la concorrenza
							DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

							//la stringa isFilePresent vale:
							//"1" se il file è stato già acquisito;
							//"0" se il file ancora non è stato acquisito
							bool isFilePresent = doc.CheckAcquisizioneFile(fileRequest.docNumber, fileRequest.versionId);

							if (isFilePresent)
							{
								errorMessage = "Impossibile acquisire il file perchè risulta già acquisito";
								retValue = false;
							}
							else if (fileDoc.content.Length == 0)
							{
								errorMessage = "Impossibile acquisire il file perchè è di 0 byte";
								retValue = false;
							}
							else
							{
								DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(objSicurezza);

								if (GestioneTSAttacced)
								{
									//Gestione timestamped file
									if ((fileDoc.name.ToUpper().EndsWith("M7M")) ||
										(fileDoc.name.ToUpper().EndsWith("TSD")))
									{
										string extension = fileDoc.name.ToUpper();
										if (extension.EndsWith("M7M"))
										{
											//GestioneM7M(fileRequest, fileDoc, objSicurezza);
											DigitalSignature.PKCS_Utils.m7m m7mhandler = new DigitalSignature.PKCS_Utils.m7m();
											m7mhandler.explode(fileDoc.content);
											AggiuntaEVerificaMarca(fileRequest, objSicurezza, m7mhandler.Data.Content, m7mhandler.TSR);
										}

										if (extension.EndsWith("TSD"))
										{
											//GestioneTSD(fileRequest, fileDoc, objSicurezza);

											DigitalSignature.PKCS_Utils.tsd tsdhandler = new DigitalSignature.PKCS_Utils.tsd();
											tsdhandler.explode(fileDoc.content);
											AggiuntaEVerificaMarca(fileRequest, objSicurezza, tsdhandler.Data.Content, tsdhandler.TSR);


										}

									}
								}


								if (string.IsNullOrEmpty(fileRequest.fileName))
									fileRequest.fileName = fileDoc.name;

								//string estensione = fileDoc.estensioneFile;
								string estensione = "";
								if (string.IsNullOrEmpty(estensione))
								{
									if (
										fileDoc.name.ToUpper().EndsWith("P7M") ||
										fileDoc.name.ToUpper().EndsWith("TSD") ||
										fileDoc.name.ToUpper().EndsWith("M7M")
										)
									{
										estensione = fileDoc.name.Substring(fileDoc.name.IndexOf(".") + 1);
									}
									else
									{
										estensione = Path.GetExtension(fileDoc.name);
										if (estensione.StartsWith("."))
											estensione = estensione.Substring(1);
									}
								}

								if (
									fileDoc.name.ToUpper().EndsWith("P7M") ||
									fileDoc.name.ToUpper().EndsWith("TSD") ||
									fileDoc.name.ToUpper().EndsWith("M7M") ||
									fileRequest.firmato == "1"
									)
								{
									while (
										fileDoc.name.ToUpper().EndsWith("P7M") ||
										fileDoc.name.ToUpper().EndsWith("TSD") ||
										fileDoc.name.ToUpper().EndsWith("M7M")
										)
									{
										if (estensione.LastIndexOf(".") > -1)
										{
											// Mod. Lembo per ticket: while breaker
											if (!estensione.ToUpper().EndsWith("P7M") &&
												!estensione.ToUpper().EndsWith("M7M") &&
												!estensione.ToUpper().EndsWith("TSD"))
											{
												if (estensione.EndsWith(")") && estensione.LastIndexOf("(") != -1)
												{
													string estensioneReplace = estensione;
													estensione = estensione.Remove(estensione.LastIndexOf("("));
													//al nome del file vado a rimuovere il (1) aggiunto dai browser
													fileDoc.name = fileDoc.name.Replace(estensioneReplace, estensione);
													fileDoc.fullName = fileDoc.fullName.Replace(estensioneReplace, estensione);
													fileRequest.fileName = fileRequest.fileName.Replace(estensioneReplace, estensione);
												}
												break;
											}

											estensione = estensione.Remove(estensione.LastIndexOf("."));
											if (estensione.EndsWith(")") && estensione.LastIndexOf("(") != -1)
											{
												string estensioneReplace = estensione;
												estensione = estensione.Remove(estensione.LastIndexOf("("));
												//al nome del file vado a rimuovere il (1) aggiunto dai browser
												fileDoc.name = fileDoc.name.Replace(estensioneReplace, estensione);
												fileDoc.fullName = fileDoc.fullName.Replace(estensioneReplace, estensione);
												fileRequest.fileName = fileRequest.fileName.Replace(estensioneReplace, estensione);
											}
											if (estensione.ToUpper().EndsWith("P7M"))
											{
												fileRequest.firmato = "1";
												tipoFirma = DocsPaVO.documento.TipoFirma.CADES;
												signTypeChecked = true;
											}
											if (estensione.ToUpper().EndsWith("TSD"))
											{
												tipoFirma = DocsPaVO.documento.TipoFirma.TSD;
												signTypeChecked = true;
											}
										}
										else
										{
											if (estensione.EndsWith(")") && estensione.LastIndexOf("(") != -1)
											{
												string estensioneReplace = estensione;
												estensione = estensione.Remove(estensione.LastIndexOf("("));
												//al nome del file vado a rimuovere il (1) aggiunto dai browser
												fileDoc.name = fileDoc.name.Replace(estensioneReplace, estensione);
												fileDoc.fullName = fileDoc.fullName.Replace(estensioneReplace, estensione);
												fileRequest.fileName = fileRequest.fileName.Replace(estensioneReplace, estensione);
											}
											break;
										}
									}
									//LULUCIANI: PUò ACCADERE CHE IL NOME DEL FILE CONTENGA "." QUESTO FA Sì CHE L'ESTENSIONE 
									//RISULTI SPORCA, PER EVITARE CIò ALLA FINE DEL PRECDENTE whilE CHIAMO COMUNQUE IL METODO

									string estensione2 = Path.GetExtension(estensione);
									if (!string.IsNullOrEmpty(estensione2))
										estensione = estensione2;


									if (estensione.StartsWith("."))
										estensione = estensione.Substring(1);

									//PER ESTENSIONI CHE CONTENGONO ... (ESEMPRIO PDF...P7M)
									while (estensione.EndsWith("."))
										estensione = estensione.Remove(estensione.LastIndexOf("."));

									if (fileDoc.name.ToUpper().EndsWith("P7M"))
									{
										fileRequest.firmato = "1";
										tipoFirma = DocsPaVO.documento.TipoFirma.CADES;
										signTypeChecked = true;
									}
								}
								//In caso di XML, verifico se è firmato XADES
								else if (estensione.ToUpper().EndsWith("XML") && IsSignedXades(fileDoc))
								{
									fileRequest.firmato = "1";
									tipoFirma = DocsPaVO.documento.TipoFirma.XADES.ToString();
									signTypeChecked = true;
								}
								else
								{
									fileRequest.firmato = "0";
									tipoFirma = DocsPaVO.documento.TipoFirma.NESSUNA_FIRMA;
								}


								//test se pades
								if (GestionePades)
								{
									if (estensione.ToUpper().EndsWith("PDF"))
									{
										if (BusinessLogic.Documenti.DigitalSignature.Pades_Utils.Pades.IsPdfPades(fileDoc))
										{
											fileRequest.firmato = "1";
											if (!signTypeChecked)
												tipoFirma = DocsPaVO.documento.TipoFirma.PADES;
										}
										else //Verifico la firma pades con ASPOSE
										{
											if (IsSignedPades(fileDoc))
											{
												fileRequest.firmato = "1";
												if (!signTypeChecked)
													tipoFirma = DocsPaVO.documento.TipoFirma.PADES;
											}
										}
									}
								}

								if (fileRequest.tipoFirma == DocsPaVO.documento.TipoFirma.ELETTORNICA)
								{
									switch (tipoFirma)
									{
										case (DocsPaVO.documento.TipoFirma.CADES):
											tipoFirma = DocsPaVO.documento.TipoFirma.CADES_ELETTORNICA;
											break;
										case (DocsPaVO.documento.TipoFirma.XADES):
											tipoFirma = DocsPaVO.documento.TipoFirma.PADES_ELETTORNICA;
											break;
										case (DocsPaVO.documento.TipoFirma.TSD):
											tipoFirma = DocsPaVO.documento.TipoFirma.PADES_ELETTORNICA;
											break;
										case (DocsPaVO.documento.TipoFirma.NESSUNA_FIRMA):
											tipoFirma = DocsPaVO.documento.TipoFirma.ELETTORNICA;
											break;
									}
								}
								fileRequest.tipoFirma = tipoFirma;

								if (estensione.IndexOf(".") != -1)
								{
									string[] extSplit = estensione.Split('.');
									estensione = extSplit[extSplit.Length - 1];
								}

								//se nomeOriginale non è stato impostato lo metto io riprendendolo dal nome
								if (String.IsNullOrEmpty(fileDoc.nomeOriginale))
								{
									fileDoc.nomeOriginale = fileDoc.name;
								}

								if (retValue)
								{
									bool cacheAttivo = CacheFileManager.isActiveCaching(objSicurezza.idAmministrazione);
									if (cacheAttivo)
									{
										logger.Debug("eseguo il putfile della cache");
										if (!CacheFileManager.PutFile(objSicurezza, fileRequest, fileDoc, estensione, out errorMessage))
										{
											if (errorMessage == string.Empty)
												errorMessage = "Non è stato possibile acquisire il documento. Er su CM <BR><BR>Ripetere l'operazione di acquisizione.";

											retValue = false;
										}
									}
									else
									{
										if (!documentManager.PutFile(fileRequest, fileDoc, estensione, repositoryRootPath))
										{
											retValue = false;
											errorMessage = "Non è stato possibile acquisire il documento. Er su PF <BR><BR>Ripetere l'operazione di acquisizione.";
											logger.Error(errorMessage.Replace("<BR", ""));
										}
										//Non è più utilizzata
										//else
										//{
										//	DocsPaDocumentale.Documentale.FullTextSearchManager fullTextManager = new DocsPaDocumentale.Documentale.FullTextSearchManager(objSicurezza);
										//	retValue = fullTextManager.SetDocumentAsIndexed(fileRequest.docNumber);

										//	if (!retValue)
										//	{
										//		errorMessage = "Non è stato possibile acquisire il documento. Er su Idx <BR><BR>Ripetere l'operazione di acquisizione.";
										//		logger.Error(errorMessage.Replace("<BR", ""));
										//	}
										//}
									}
									documentManager = null;

									if (retValue)
										transactionContext.Complete();

									if (!cacheAttivo)
									{
										// Ulteriore verifica tramite impronta della corretta acq
										if (!doc.CheckAcquisizioneFile(fileRequest.docNumber, fileRequest.versionId))
										{
											errorMessage = "Non è stato possibile acquisire il documento Er su CAF. <BR><BR>Ripetere l'operazione di acquisizione.";
											logger.Error(errorMessage.Replace("<BR", ""));
											//08-02-2016: Commentato il cestinaDocumento perchè se falisce l'acquisizione cestinava il protocollo
											//doc.CestinaDocumento(objSicurezza.idPeople, fileRequest.docNumber, fileRequest.versionId);
											retValue = false;
										}
									}
								}
							}
						}
					}
				}
			}
		}
		logger.Information("END");

		//Funzioni Accessorie da effetuare una volta inserito il file.
		//se retvalue è false non vengono effettuate in quanto è fallito l'inserimento
		if (retValue)
		{
			//Crea il fileInfo per la carta di identità del documento.
			if (processFileInfo)
				processFileInformation(fileRequest, objSicurezza, temporaryRootPath);

			//estrae dalla fattura PA gli eventuali allegati contenuti nell'XML e li inserisce come allegati del documento
			addAllegatiFatturaPA(fileRequest, fileDoc, objSicurezza);

			//Inserisce i file nell'aera di spool per l'indicizzazione
			sendFileToIndexer(fileRequest, fileDoc, objSicurezza);
		}
		return retValue;
	}

	private static string ConvertAccentedString(string text)
	{
		var normalizedString = text.Normalize(NormalizationForm.FormD);
		var stringBuilder = new StringBuilder();

		foreach (var c in normalizedString)
		{
			var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
			if (unicodeCategory != UnicodeCategory.NonSpacingMark)
			{
				stringBuilder.Append(c);
			}
		}

		return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
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

	private static bool IsFileAccepted(DocsPaVO.utente.InfoUtente infoUtente, bool isProtocollo, DocsPaVO.documento.FileDocumento fileDocument, out string errorMessage, bool checkSign = true, string basePath = "")
	{
		string fileName = string.Empty;
		if (!string.IsNullOrEmpty(fileDocument.fullName))
			fileName = fileDocument.fullName;
		else
			fileName = fileDocument.name;
		FileInfo fileInfo;
		try
		{
			fileInfo = new FileInfo(fileName);
		}
		catch (PathTooLongException)
		{
			string name = Path.GetFileName(fileName);
			fileInfo = new FileInfo(name);
		}
		string extension = fileInfo.Extension.Replace(".", string.Empty);
		//è un p7m? lo dovrei sbustare...
		//se trovate bachi, corregeteli senza troppa esitazione...
		//30/11/2012 afaillace.. aggiunto blocco
		byte[] content;
		if (extension.ToUpper().EndsWith("P7M".ToUpper()))
		{
			//devo duplicare il filedocumento perchè non voglio modificare quello di input.
			DocsPaVO.documento.FileDocumento verFileDocument = new DocsPaVO.documento.FileDocumento();
			verFileDocument.content = fileDocument.content;
			verFileDocument.fullName = fileDocument.fullName;
			verFileDocument.name = fileDocument.name;
			try
			{
				if (checkSign)
				{
					VerifyFileSignature(verFileDocument, null);
					if (verFileDocument.signatureResult != null)
					{

						if (
							(verFileDocument.signatureResult.ErrorMessages != null) &&
							(verFileDocument.signatureResult.ErrorMessages.Length > 0)
							)
						{
							string errRetval = "\r\n";
							foreach (string s in verFileDocument.signatureResult.ErrorMessages)
								errRetval += s + " \r\n";

							//throw new Exception("Il documento firmato ha i seguenti problemi:" + errRetval +"\r\n non è possobile verificarlo");
						}
					}
				}
			}
			catch (Exception ex)
			{
				logger.Debug("Il documento firmato è danneggiato, non è più possibile verificarlo: " + ex.Message);
			}
			fileName = verFileDocument.fullName;
			content = verFileDocument.content;
		}
		else
		{
			content = fileDocument.content;
		}
		//FineBlocco


		return IsFileAccepted(infoUtente, isProtocollo, fileName, out errorMessage, content);
	}

	private static bool isScannedDocument(DocsPaVO.documento.FileDocumento fileDocumento)
	{
		//prima di tutto deve essere cartaceo
		if (!fileDocumento.cartaceo)
			return false;

		string fileName = fileDocumento.name;
		fileName = fileName.Replace(getExts(fileName), string.Empty);
		bool isScannedFileName = false;
		//poi il nome deve almeno avere un valore
		if (!string.IsNullOrEmpty(fileName))
		{
			//Testo il nome con il guid.tiff (nuova versione degli smart client)
			Regex guidRegEx = new Regex(@"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$");
			isScannedFileName = guidRegEx.IsMatch(fileName);
			if (isScannedFileName)
				return true;

			//nel caso testo il pattern con la vecchia nomenclatura Activex dpascanXXXX.tif
			isScannedFileName = false;
			guidRegEx = new Regex(@"^(\{{0,1}dpascan([0-9]){4})$");
			isScannedFileName = guidRegEx.IsMatch(fileName);
			if (isScannedFileName)
				return true;

			//Per i file provenienti da ACQ MASSIVA
			isScannedFileName = false;
			guidRegEx = new Regex(@"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})_ACQMASSIVA$");
			isScannedFileName = guidRegEx.IsMatch(fileName);
			if (isScannedFileName)
				return true;


		}
		return false;
	}

	private static string setFileNameForScanneddocuments(DocsPaVO.utente.InfoUtente objSicurezza, DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento fileDocumento)
	{
		string FileName = string.Empty;
		string ext = System.IO.Path.GetExtension(fileDocumento.name);
		DocsPaVO.documento.SchedaDocumento sd = BusinessLogic.Documenti.DocManager.getDettaglio(objSicurezza, fileRequest.docNumber, fileRequest.docNumber);
		DocsPaVO.documento.Allegato all = null;
		bool isallegato = isDocAllegato(sd, ref all);

		if (!isallegato)
		{
			FileName = "Documento_Principale_";
			if (sd.protocollo != null)
				FileName += removeIllegalChars(sd.protocollo.segnatura, true) + ext;
			else
				FileName += fileRequest.docNumber + ext;
		}
		else
		{  //è un allegato
			string descAllegato = Truncate(fileRequest.descrizione, 115);// tronco a 115
			// L'allegato è sempre un documento grigio
			FileName += removeIllegalChars(descAllegato, true) + "_" + fileRequest.docNumber + ext;
		}
		return FileName;

	}

	private static bool IsFileAccepted(DocsPaVO.utente.InfoUtente infoUtente, string docNumber, DocsPaVO.documento.FileDocumento fileDocument, out string errorMessage, bool checkSign = true)
	{
		bool isProtocollo = (Documenti.DocManager.GetTipoDocumento(docNumber) != "G");

		return IsFileAccepted(infoUtente, isProtocollo, fileDocument, out errorMessage, checkSign);
	}

	private static void AggiuntaEVerificaMarca(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente objSicurezza, byte[] Content, List<DigitalSignature.PKCS_Utils.CryptoFile> TSRLst)
	{
		foreach (DigitalSignature.PKCS_Utils.CryptoFile cf in TSRLst)
		{
			DigitalSignature.VerifyTimeStamp checkMarca = new DigitalSignature.VerifyTimeStamp();
			DocsPaVO.areaConservazione.OutputResponseMarca resultMarca = checkMarca.Verify(Content, cf.Content);

			if (resultMarca.esito == "OK")
			{
				DocsPaDB.Query_DocsPAWS.TimestampDoc timestampDoc = new DocsPaDB.Query_DocsPAWS.TimestampDoc();
				timestampDoc.saveTSR(objSicurezza, resultMarca, fileRequest);
			}
		}
	}

	private static bool IsSignedXades(FileDocumento fileDoc)
	{
		bool result = false;
		XmlDocument Xmlfile = new XmlDocument();
		XmlTextReader tr = new XmlTextReader(new System.IO.MemoryStream(fileDoc.content));
		tr.XmlResolver = null;
		try
		{
			Xmlfile.Load(tr);
			XmlNodeList signature = Xmlfile.DocumentElement.GetElementsByTagName("ds:Signature");
			if (signature != null && signature.Count > 0)
			{
				result = true;
			}
		}
		catch (Exception e)
		{
			logger.Error("Errore nel metodo IsSignedXades " + e.Message);
			return false;
		}
		finally
		{
			tr.Close();
		}

		return result;
	}

	private static bool IsSignedPades(FileDocumento fileDoc)
	{
		bool result = false;
		try
		{
			logger.Debug("Verifico se firmato Pades");

#if false  // usa webservice per la conversione
			ConvertEngineWS converter = new ConvertEngineWS();
			string converterEngineUrl = System.Configuration.ConfigurationManager.AppSettings["INLINE_CONVERTER_URL"];
			if (!string.IsNullOrEmpty(converterEngineUrl))
			{
				// Impostazione dell'url del convertitore
				converter.Url = converterEngineUrl;
				try
				{
					#region BigFile 

					if (fileDoc.GetType() == typeof(BigFileDocumento))
					{
						logger.Debug("Invoco verifica Pades per BigFile");
						BigFileDocumento bigFile = (BigFileDocumento)fileDoc;
						result = converter.IsPdfPadesBigFile(bigFile.BigFilePath);
					}
					#endregion
					else
					{
						logger.Debug("Invoco verifica Pades di default");
						result = converter.IsPdfPades(fileDoc.content);
					}

				}
				catch (Exception e)
				{
					// Recupero dell'eccezione originale e sua scrittura nel log
					ApplicationException originalException = DocsPaUtils.Exceptions.SoapExceptionParser.GetOriginalException(e);

					logger.Debug("Eccezione durante la conversione con il motore esterno.", originalException);

				}
			}
#endif            
		}
		catch (Exception e)
		{
			logger.Error("Errore in IsSignedPadesAspose: " + e.Message);
			result = false;
		}

		return result;
	}

	public static void processFileInformation(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente objSicurezza, string repositoryRootPath = "")
	{
		logger.Debug("processFileInformation");
		bool disabled = true;
		string config = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_PROCESS_FILEINFO");
		if (!string.IsNullOrEmpty(config))
			if ((config == "1") || (config.ToLower() == "true"))
				disabled = false;
		if (disabled)
			return;

		if (fileRequest.repositoryContext != null)
			return;

		if (String.IsNullOrEmpty(fileRequest.docNumber))
			return;
		logger.Debug("processFileInformation2");
		if (objSicurezza != null && string.IsNullOrEmpty(objSicurezza.dst))
		{
			logger.Debug("Ricavo Token");
			objSicurezza.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();
			logger.Debug("Token ricavato");
		}
		logger.Debug("processFileInformation3");
		string fileNameBack = fileRequest.fileName;
		DocsPaDB.Query_DocsPAWS.Documenti docs = new DocsPaDB.Query_DocsPAWS.Documenti();
		string fileInfoMask = docs.GetFileInfoMask(fileRequest.versionId, fileRequest.docNumber);
		logger.Debug("GET FileinfoMASK for doc: {0} ver: {1}  mask: {2}", fileRequest.docNumber, fileRequest.versionId, fileInfoMask);
		DocsPaVO.documento.FileInformation fileInfo = DocsPaVO.documento.FileInformation.decodeMask(fileInfoMask);

		/*
		// Setto tutto a untested (vecchio disabled)
		if (disabled)
		{
			fileInfo.Status = DocsPaVO.documento.FileInformation.VerifyStatus.Untested;
			docs.UpdateComponentsFileInfo(DocsPaVO.documento.FileInformation.encodeMask(fileInfo), fileRequest.versionId, fileRequest.docNumber);
			return;
		}
		*/
		logger.Debug("processFileInformation4");
		// Modifica per evitare errori col documentale Documentum.
		DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
		fileRequest.version = documentale.GetVersionFromVersionId(fileRequest.versionId);
		fileRequest.versionLabel = fileRequest.version;

        //1) prelevo il file
        logger.Debug($"fileRequest.fileName : {fileRequest.fileName}");
        logger.Debug($"fileRequest.path : {fileRequest.path}");
        logger.Debug($"repositoryRootPath : {repositoryRootPath}");
        DocsPaVO.documento.FileDocumento filedoc = getFile(fileRequest, objSicurezza, false, false, repositoryRootPath);
        logger.Debug($"filedoc.name : {filedoc.name}");
        logger.Debug($"filedoc.path : {filedoc.path}");
		if(filedoc.content == null)
            logger.Debug($"filedoc.content null");

        if (!String.IsNullOrEmpty(filedoc.name))
		{
			if (Path.GetExtension(filedoc.name).ToLowerInvariant().Contains("pdf"))
			{
				fileInfo.PdfVer = Encoding.UTF8.GetString(filedoc.content, 5, 3);
				logger.Debug("Estenzione PDF, versione ricavata {0}", fileInfo.PdfVer);
			}
		}
		DocsPaVO.documento.FileDocumento filedocfirmato = getFileFirmato(fileRequest, objSicurezza, false, repositoryRootPath);
#if false    // usa Sa_Utils                  
		Sa_Utils.FileTypeFinder ff = new Sa_Utils.FileTypeFinder(); 
		string fileExtension = ff.FileType(filedoc.content);

		logger.DebugFormat("processFileInformation5");
		if ((fileExtension.ToUpperInvariant().Contains("PDF/")) && (!String.IsNullOrEmpty(fileInfo.PdfVer)))
			fileInfo.PdfVer += "§" + fileExtension;

		logger.DebugFormat("SAUTILS fileExtension for file is {0}", fileExtension);

		if (ff.isExecutable(fileExtension))
		{
			fileInfo.NoMacroOrExe = DocsPaVO.documento.FileInformation.VerifyStatus.Invalid;
			logger.DebugFormat("SAUTILS Says file has macros");
		}
		else
		{
			fileInfo.NoMacroOrExe = DocsPaVO.documento.FileInformation.VerifyStatus.Valid;
			logger.DebugFormat("SAUTILS Says file has NO macros");
		}
#endif
            string fileExtension = null;

		bool fileformatOK = IsValidFileContent(filedoc);
		if (fileformatOK)
		{
			fileInfo.FileFormatOK = DocsPaVO.documento.FileInformation.VerifyStatus.Valid;
			logger.Debug("SAUTILS Says file has good Extension");
		}
		else
		{
			fileInfo.FileFormatOK = DocsPaVO.documento.FileInformation.VerifyStatus.Invalid;
			logger.Debug("SAUTILS Says file has BAD Extension");
		}
		ArrayList tsAl = TimestampManager.getTimestampsDoc(objSicurezza, fileRequest);
		string extOFN = string.Empty;
		try
		{
			extOFN = System.IO.Path.GetExtension(filedocfirmato.nomeOriginale);
			logger.Debug("OFN ext: {0}", extOFN);
		}
		catch { }

		//Test sull' Impronta
		string impronta;

		logger.Debug("processFileInformation");

		docs.GetImpronta(out impronta, fileRequest.versionId, fileRequest.docNumber);
		if (string.IsNullOrEmpty(impronta))
		{
			//Sul DB non è stato salvata l'impronta/ metto untested perchè non posso testare
			fileInfo.FileHashOK = DocsPaVO.documento.FileInformation.VerifyStatus.Untested;
		}
		else
		{

			//ho un impronta, testo sia SHA1 che SHA256 non si sa mai
			if (
				impronta.Equals(DocsPaUtils.Security.CryptographyManager.CalcolaImpronta(filedocfirmato.content)) ||
				impronta.Equals(DocsPaUtils.Security.CryptographyManager.CalcolaImpronta256(filedocfirmato.content))
				)
			{
				//impronta valida, metto a valid il flag
				fileInfo.FileHashOK = DocsPaVO.documento.FileInformation.VerifyStatus.Valid;
			}
			else
			{
				//impronta nonvalida, metto a invalid il flag
				fileInfo.FileHashOK = DocsPaVO.documento.FileInformation.VerifyStatus.Invalid;
			}
		}

		//controllo TS
		if (tsAl.Count == 0)     //TS non presenti
			fileInfo.TimeStampStatus = DocsPaVO.documento.FileInformation.VerifyStatus.NotApplicable;

        TimestampDoc tsInfo = tsAl[0] as TimestampDoc;

        if (tsInfo != null)
        {

			BusinessLogic.Documenti.DigitalSignature.VerifyTimeStamp vts = new Documenti.DigitalSignature.VerifyTimeStamp();
			DocsPaVO.areaConservazione.OutputResponseMarca resultMarca = vts.Verify(filedocfirmato.content, Convert.FromBase64String(tsInfo.TSR_FILE));
			logger.Debug("Found TS: result  {0}", resultMarca.esito);
			if (resultMarca.esito == "OK")
			{
				fileInfo.TimeStampStatus = DocsPaVO.documento.FileInformation.VerifyStatus.Valid;
			}
			else
			{
				//gestire se è TSD o M7M, il controllo non va fatto dato che sarà errato 
				if ((extOFN.ToUpperInvariant().Contains("TSD") || extOFN.ToUpperInvariant().Contains("M7M")))
				{

					resultMarca = vts.Verify(BusinessLogic.Documenti.DigitalSignature.Helpers.sbustaFileTimstamped(filedocfirmato.content), Convert.FromBase64String(tsInfo.TSR_FILE));
					logger.Debug("Found [TSD] TS: result  {0}", resultMarca.esito);
					if (resultMarca.esito == "OK")
					{
						fileInfo.TimeStampStatus = DocsPaVO.documento.FileInformation.VerifyStatus.Valid;
						if (Convert.ToDateTime(tsInfo.DTA_SCADENZA) < System.DateTime.Now)
						{
							fileInfo.TimeStampStatus = DocsPaVO.documento.FileInformation.VerifyStatus.Expired;
							logger.Debug("Found [TSD] TS: result  Expired");
						}
					}
					else
					{
						fileInfo.TimeStampStatus = DocsPaVO.documento.FileInformation.VerifyStatus.NotApplicable;
						logger.Debug("Found [TSD] TS: result  NA: bad TS");
					}
				}
				fileInfo.TimeStampStatus = DocsPaVO.documento.FileInformation.VerifyStatus.Invalid;
				logger.Debug("Found  TS: result  Invalid bad TS");
			}

			if (Convert.ToDateTime(tsInfo.DTA_SCADENZA) < System.DateTime.Now)
			{
				fileInfo.TimeStampStatus = DocsPaVO.documento.FileInformation.VerifyStatus.Expired;
				logger.Debug("Found TS: result  Expired");
			}

		}

		if (fileInfo.CheckRefDate == DateTime.MinValue)
			fileInfo.CheckRefDate = DateTime.Now;

		// già stato controllato... non rifaccio il controllo
		if ((fileInfo.Signature != DocsPaVO.documento.FileInformation.VerifyStatus.Valid) &&
		   (fileInfo.CrlStatus != DocsPaVO.documento.FileInformation.VerifyStatus.Valid))
		{
			logger.Debug("Firma non valida o non verificata");
			if (fileRequest.firmato == "1")
			{
				logger.Debug("Firmato, verifica schedulata");
				fileInfo.CrlStatus = DocsPaVO.documento.FileInformation.VerifyStatus.InProgress;
				fileInfo.Signature = DocsPaVO.documento.FileInformation.VerifyStatus.InProgress;
				fileInfo.CheckRefDate = DateTime.MinValue; //rimetto il minvalue, dato che la data la metto alla fine della verifica crl
			}
			else
			{
				logger.Debug("Non Firmato");
				fileInfo.CrlStatus = DocsPaVO.documento.FileInformation.VerifyStatus.NotApplicable;
				fileInfo.Signature = DocsPaVO.documento.FileInformation.VerifyStatus.NotApplicable;
			}
		}

		//foto formato in amm
		try
		{
			string estensione = Path.GetExtension(filedoc.name).ToUpperInvariant();

			//tolgo il punto davanti all'aestensione nel caso esista.
			if (estensione.StartsWith(".")) estensione = estensione.Substring(1);

			DocsPaVO.FormatiDocumento.SupportedFileType[] fileTypes = FormatiDocumento.SupportedFormatsManager.GetFileTypes(Convert.ToInt32(objSicurezza.idAmministrazione));
			DocsPaVO.FormatiDocumento.SupportedFileType FileType = (from fileType in fileTypes where fileType.FileExtension.ToUpper().Equals(estensione) select fileType).FirstOrDefault();
			if (FileType.FileTypePreservation)
				fileInfo.Preservable = DocsPaVO.documento.FileInformation.VerifyStatus.Valid;
			else
				fileInfo.Preservable = DocsPaVO.documento.FileInformation.VerifyStatus.Invalid;

			if (FileType.FileTypeSignature)
				fileInfo.Signable = DocsPaVO.documento.FileInformation.VerifyStatus.Valid;
			else
				fileInfo.Signable = DocsPaVO.documento.FileInformation.VerifyStatus.Invalid;

			if (fileInfo.AdminRefDate == DateTime.MinValue)
				fileInfo.AdminRefDate = DateTime.Now;
		}
		catch (Exception e)
		{
			logger.Debug("Errore reperendo il formato dall'amministrazione {0} stack {1}", e.Message, e.StackTrace);
		}

		fileInfo.setGlobalStatus();
		string fileInfoStr = DocsPaVO.documento.FileInformation.encodeMask(fileInfo);
		logger.Debug("SET FileinfoMASK for doc: {0} ver: {1}  mask: {2}", fileRequest.docNumber, fileRequest.versionId, fileInfoMask);
		docs.UpdateComponentsFileInfo(fileInfoStr, fileRequest.versionId, fileRequest.docNumber);

		fileRequest.fileName = fileNameBack;

		//Se sto acquisendo l'ultima versione, aggiorno l'informazione sul file acquisito in DPA_INFO_FILE
		bool isUltimaVersione = fileRequest.versionId.Equals(VersioniManager.getLatestVersionID(fileRequest.docNumber, objSicurezza));
		if (isUltimaVersione)
		{
			fileRequest.dataAcquisizione = docs.GetDataAcquisizioneFile(fileRequest.versionId);
			UpdateInfoFileAcquisito(fileInfo, fileRequest, filedoc.nomeOriginale, fileExtension, objSicurezza);
		}

	}

	private static void addAllegatiFatturaPA(FileRequest fileRequest, FileDocumento fileDoc, DocsPaVO.utente.InfoUtente objSicurezza)
	{
		logger.Debug("Start");

		bool disabled = true;
		string config = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_PROCESS_FATTURAPA");
		if (!string.IsNullOrEmpty(config))
			if ((config == "1") || (config.ToLower() == "true"))
				disabled = false;
		if (disabled)
			return;


		XmlParsing.FatturaPA.FatturaPAManager pam = new XmlParsing.FatturaPA.FatturaPAManager();
		try
		{

			byte[] content = fileDoc.content;
			if (fileDoc.name.ToUpper().EndsWith("P7M"))
			{ //gestire il file p7m  .. nel caso la fattura fosse in un container P7m , sbusto all'infinito fino a che non arrivo al content vero e proprio
				do
				{
					try
					{
						content = Documenti.DigitalSignature.PKCS_Utils.Pkcs.extractSignedContent(content);
					}
					catch  //mi sta dando ecccezione, vuol dire che sono arrivato al file.. esco dal ciclo..
					{
						break;
					}
				} while (true);

			}

			//prima di tutto controlliamo se è una fattura PA
			if (pam.isFatturaPA(content))
			{
				logger.Debug("L'allegato risulta essere una fattura PA");
				string docnumPrinc = string.Empty;
				//reperisco una scheda documento dal documumber del filerequest
				SchedaDocumento sdFile = BusinessLogic.Documenti.DocManager.getDettaglioPerNotificaAllegati(objSicurezza, fileRequest.docNumber, fileRequest.docNumber);

				//Verifico se la scheda documento si riferisce al documento principale o ad un suo eventuale allegato..
				// se fosse il documento principale reperisco dalle l'informazione ricavata il docnumber che in ogni modo dovrà essere lo stesso del filerequest
				if (sdFile.documentoPrincipale == null) //doc principale
				{
					docnumPrinc = sdFile.docNumber;
				}
				else //Allegato
				{
					//nel caso fosse un allegato prendo il documber dalla struttura docummento principale e da li reperisco la scheda documento del documento principale.
					docnumPrinc = sdFile.documentoPrincipale.docNumber;
					sdFile = BusinessLogic.Documenti.DocManager.getDettaglioPerNotificaAllegati(objSicurezza, docnumPrinc, docnumPrinc);
				}


				//Questa chiamata estrae alla fattura PA gli eventuali allegati..
				XmlParsing.FatturaPA.FatturaPAManager.allegati[] allegatiFatturaPA = pam.getAllegatiFromFatturaPA(content);

				//nel caso non ci fossero allegati o la chiamata fosse fallita il risultato sarà null quindi non è necessario proseguire oltre.
				if (allegatiFatturaPA != null)
				{

					List<String> hashAllegati = new List<string>();
					//Mi ciclo gli allegati della scheda doumento per salvarmi gli hash, ma lo faccio solo se ho realmente degli allegati in SD
					ArrayList allArray = BusinessLogic.Documenti.AllegatiManager.getAllegati(docnumPrinc, string.Empty);
					if (allArray.Count > 0)
					{
						using (DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti())
						{
							foreach (DocsPaVO.documento.Allegato all in allArray)
							{
								string imp = "";
								doc.GetImpronta(out imp, all.versionId, all.docNumber);
								hashAllegati.Add(imp.ToLower());
							}
						}
					}

					//Per ogni allegato in fattura, controllo se quell'HASH non sia già presente nella mia lista.. se è presente annullo il contenutoAttachment facendo 
					//saltare l'aquisizione e l'aggiunta dell'allegato.
					if (allArray.Count > 0)
					{
						foreach (XmlParsing.FatturaPA.FatturaPAManager.allegati allPA in allegatiFatturaPA)
						{
							string hashAllegato = DocsPaUtils.Security.CryptographyManager.CalcolaImpronta256(allPA.contenutoAttachment);
							if (hashAllegati.Contains(hashAllegato.ToLower()))
								allPA.contenutoAttachment = null;
						}
					}

					int contatore = 0;
					string nomeFattura = fileDoc.nomeOriginale;
					if (!string.IsNullOrEmpty(nomeFattura))
						nomeFattura = Path.GetFileNameWithoutExtension(nomeFattura);
					else
						nomeFattura = "FatturaPA";

					//per ogni allegato in fattura pa....
					foreach (XmlParsing.FatturaPA.FatturaPAManager.allegati a in allegatiFatturaPA)
					{
						//controllo se il contenuto non fosse null... 
						//magari è stato posto a null perchè l'allegato è già presente, 
						//magari è posto a null perchè all'allegato realmente non esiste o l'xml è corrotto
						//se è null salto a quello successvio
						if (a.contenutoAttachment == null)
							continue;

						//creo il nome dal nome attachment e dal suo formato (estensione)

						//Gestione delle varie casitiche.
						string nomeAll = "Attachment.bin";
						if (string.IsNullOrEmpty(a.formatoAttachment))
						{
							if (String.IsNullOrEmpty(Path.GetExtension(a.nomeAttachment)))
							{
								a.formatoAttachment = "bin";
							}
							else
							{
								a.formatoAttachment = Path.GetExtension(a.nomeAttachment);
								a.nomeAttachment = Path.GetFileNameWithoutExtension(a.nomeAttachment);
							}
							a.formatoAttachment = a.formatoAttachment.Replace(".", "");
							nomeAll = String.Format("{0}.{1}", a.nomeAttachment, a.formatoAttachment);
						}
						else
						{
							nomeAll = String.Format("{0}.{1}", a.nomeAttachment, a.formatoAttachment);
						}

						DocsPaVO.documento.Allegato all = new DocsPaVO.documento.Allegato();
						try
						{
							string descBreveAtt = string.Format("{0}_All {1}", nomeFattura, contatore++);
							if (string.IsNullOrEmpty(a.descrizioneAttachment))
								all.descrizione = descBreveAtt;
							else
								all.descrizione = String.Format("{0}: {1}", descBreveAtt, a.descrizioneAttachment);

							all.docNumber = docnumPrinc;
							all.fileName = nomeAll;
							all.version = "0";
							all.numeroPagine = 1;

							DocsPaVO.documento.Allegato allIns = null;
							//aggiungo un allegato in DPA
							allIns = AllegatiManager.aggiungiAllegato(objSicurezza, all);
							BusinessLogic.Documenti.AllegatiManager.setFlagAllegati_PEC_IS_EXT(all.versionId, all.docNumber, "D");
						}
						catch (Exception e)
						{
							logger.Debug("Errore creando l'allegato per la fattura PA  ", e);
							return;
						}

						try
						{
							string err = null;

							DocsPaVO.documento.FileDocumento fdAll = new DocsPaVO.documento.FileDocumento();
							fdAll.content = a.contenutoAttachment;
							fdAll.length = a.contenutoAttachment.Length;

							fdAll.name = all.fileName;
							fdAll.bypassFileContentValidation = true;
							fdAll.fullName = nomeAll;
							fdAll.name = nomeAll;
							fdAll.nomeOriginale = nomeAll;
							fdAll.estensioneFile = Path.GetExtension(nomeAll);
							fdAll.contentType = getContentType(nomeAll);

							DocsPaVO.documento.SchedaDocumento sd = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(objSicurezza, docnumPrinc);
							DocsPaVO.documento.FileRequest fRAll = (DocsPaVO.documento.FileRequest)sd.documenti[0];
							fRAll = (DocsPaVO.documento.FileRequest)all;

							if (fdAll.content.Length > 0)
							{
								if (!BusinessLogic.Documenti.FileManager.putFile(ref fRAll, fdAll, objSicurezza, out err))
								{
									logger.Debug("Errore durante la putfile aggiungendo l'allegato per la fattura PA");
									AllegatiManager.rimuoviAllegato(all, objSicurezza);
									//  BusinessLogic.interoperabilita.InteroperabilitaManager.deleteNotifica(daticert.docnumber);
									throw new Exception(err);
								}
							}
						}
						catch (Exception e)
						{
							logger.Debug("Errore aggiungendo il contenuto all'allegato per la fattura PA  ", e);
							return;
						}
					}
				}
			}
		}
		catch (Exception e)
		{
			logger.Debug("Errore rilvenado se il file è una FatturaPA, ok non fa nulla", e);
			return;
		}
	}

	private static void sendFileToIndexer(FileRequest fileRequest, FileDocumento fileDoc, DocsPaVO.utente.InfoUtente objSicurezza)
	{
		logger.Debug("Start");

		string folder = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_INDEXER_SUBMIT_FOLDER");
		if (string.IsNullOrEmpty(folder))
			return;


		//scrivo su file solo se esiste il content
		if (fileDoc.content != null)
		{
			try
			{
				Uri u = new Uri(folder);

				if (u.Scheme.ToLower() == "file") //nel caso fosse una cartella locale
				{
					if (!string.IsNullOrEmpty(u.LocalPath))
					{
						string lpath = u.LocalPath;
						if (!Directory.Exists(lpath))
							Directory.CreateDirectory(lpath);

						// Creazione nome file
						string fileName = string.Empty;
						string estensione = Path.GetExtension(fileDoc.nomeOriginale);

						if (!string.IsNullOrEmpty(fileRequest.fileName))
						{
							fileName = fileRequest.fileName;

							string extensions = string.Empty;

							while (!string.IsNullOrEmpty(Path.GetExtension(fileName)))
							{
								extensions = Path.GetExtension(fileName) + extensions;

								fileName = Path.GetFileNameWithoutExtension(fileName);
							}

							fileName = string.Concat(fileRequest.versionId, extensions);
						}
						else
							fileName = string.Format("{0}.{1}", fileRequest.versionId, estensione);


						if (!string.IsNullOrEmpty(fileRequest.versionId))
							File.WriteAllBytes(Path.Combine(lpath, fileName), fileDoc.content);

					}
				}
				else if (u.Scheme.ToLower() == "svc")   //nel caso fosse un servizio svc
				{
					throw new NotImplementedException("Funzionalità di invio a WS non implmentata");
				}
			}
			catch (Exception e)
			{
				logger.Error("Errore, copiando il file per l'indicizzazione ", e);
			}
		}
	}

	/// <summary>
	/// Verifica firma digitale del file
	/// </summary>
	/// <param name="fileDoc"></param>
	/// <param name="dataDiRiferimento"> data di riferimento , se null non controlla la CLR esterna</param>
	public static bool VerifyFileSignature(DocsPaVO.documento.FileDocumento fileDoc, DateTime? dataDiRiferimento, string basePath = "")
	{
		bool retval = false;
		VerifySignature verifySignature = new VerifySignature();

		string inputDirectory = verifySignature.GetPKCS7InputDirectory(basePath);

		// Creazione cartella di appoggio nel caso non esista
		if (!System.IO.Directory.Exists(inputDirectory))
			System.IO.Directory.CreateDirectory(inputDirectory);

		logger.Debug("PKCS7InputDirectory: " + inputDirectory);

		string inputFile = string.Concat(inputDirectory, fileDoc.name);

		// Copia del file firmato dalla cartella del documentale
		// alla cartella di input utilizzata dal ws della verifica
		CopySignedFileToInputFolder(fileDoc, inputFile);

		if (dataDiRiferimento == null)
		{
			fileDoc.signatureResult = verifySignature.Verify(fileDoc.name, basePath);
		}
		else if (dataDiRiferimento == DateTime.MinValue)  //La MARCA
		{
			fileDoc.signatureResult = verifySignature.VerifyM7M(fileDoc.name, basePath);
			try
			{
				// Rimozione del file firmato dalla cartella di input
				File.Delete(inputFile);
			}
			catch { }
			if (fileDoc.signatureResult.StatusCode == 0) //Valido
				retval = true;

			return retval;
		}
		else
		{
			if (fileDoc.name.ToUpper().EndsWith(".XML"))
				VerifyFileSignatureXADES(fileDoc, dataDiRiferimento.Value, basePath);
			else
			{
				fileDoc.estensioneFile = getEstensioneIntoSignedFile(fileDoc.nomeOriginale);
				fileDoc.signatureResult = verifySignature.Verify_External(fileDoc, dataDiRiferimento.Value, basePath);
				//pezza verrastro
				fileDoc.name = Path.GetFileNameWithoutExtension(fileDoc.name);
			}

		}

		try
		{
			// Rimozione del file firmato dalla cartella di input
			File.Delete(inputFile);
		}
		catch
		{
		}
		string outputDirectory = verifySignature.GetPKCS7OutputDirectory(basePath);
		if (fileDoc.signatureResult.StatusCode == 0) //Valido
			retval = true;

		// Creazione cartella di appoggio nel caso non esista
		if (!System.IO.Directory.Exists(outputDirectory))
			System.IO.Directory.CreateDirectory(outputDirectory);

		logger.Debug("PKCS7OutputDirectory: " + outputDirectory);

		// Il valore di ritorno è 0 solamente se la firma del file è stata verificata
		string outputFileName = string.Empty;

		//INIZIO ABBATANGELI 
		//fileDoc.signatureResult.PKCS7Documents == null
		if (fileDoc.signatureResult.PKCS7Documents != null)
		{

			for (int i = 0; i < fileDoc.signatureResult.PKCS7Documents.Length; i++)
			{
				// Ricerca nel file firmato del documento originale,
				// che ha estensione != da ".P7M"
				DocsPaVO.documento.PKCS7Document innerDocument = fileDoc.signatureResult.PKCS7Documents[i];

				if (!string.IsNullOrEmpty(innerDocument.DocumentFileName) && !innerDocument.DocumentFileName.ToUpper().EndsWith(".P7M"))
				{
					fileDoc.name = innerDocument.DocumentFileName;
					outputFileName = string.Concat(outputDirectory, fileDoc.name);
                    logger.Debug($"VerifyFileSignature outputFileName {outputFileName}");
                    fileDoc.content = null;
					// Lettura del contenuto del file originale
					fileDoc.content = GetOutputFileContent(outputFileName);
					if(fileDoc.content == null)
						logger.Debug($"VerifyFileSignature fileDoc.content null");
                    fileDoc.length = fileDoc.content.Length;
                    logger.Debug($"VerifyFileSignature fileDoc.content.Length {fileDoc.content.Length}");
                    fileDoc.contentType = getContentType(fileDoc.name);
					fileDoc.fullName = string.Concat(fileDoc.path, '\u005C'.ToString(), fileDoc.name);

					try
					{
						// Rimozione del file creato nella cartella di output
						File.Delete(outputFileName);
					}
					catch
					{
					}

					break;
				}
				innerDocument = null;
			}

			//FINE ABBATANGELI
		}

		verifySignature = null;

		if (dataDiRiferimento == null)
		{
			if (BusinessLogic.Documenti.DigitalSignature.Pades_Utils.Pades.IsPdfPades(fileDoc))
				BusinessLogic.Documenti.DigitalSignature.Pades_Utils.Pades.VerifyPadesSignature(fileDoc);
		}

		return retval; 
	}

	private static bool IsFileAccepted(DocsPaVO.utente.InfoUtente infoUtente, bool isProtocollo, string fileName, out string errorMessage, byte[] fileContent)
	{
		errorMessage = string.Empty;
		bool accepted = false;

		if (FormatiDocumento.Configurations.SupportedFileTypesEnabled)
		{
			FileInfo fileInfo;
			try
			{
				fileInfo = new FileInfo(fileName);
			}
			catch (PathTooLongException)
			{
				string name = Path.GetFileName(fileName);
				fileInfo = new FileInfo(name);
			}
			string extension = fileInfo.Extension.Replace(".", string.Empty);
			if (extension.Contains(" "))
				extension = extension.Replace(" ", "");
			bool Validation = false;
			if (!string.IsNullOrEmpty(extension))
			{
				// Reperimento di tutti i formati di file accettati dall'amministrazione
				DocsPaVO.FormatiDocumento.SupportedFileType[] acceptedFormats = FormatiDocumento.SupportedFormatsManager.GetFileTypes(Convert.ToInt32(infoUtente.idAmministrazione));

				DocsPaVO.FormatiDocumento.SupportedFileType selectedFormat = null;

				foreach (DocsPaVO.FormatiDocumento.SupportedFileType format in acceptedFormats)
				{
					accepted = (format.FileTypeUsed && format.FileExtension.ToUpper().Equals(extension.ToUpper()));

					if (accepted)
					{
						Validation = format.FileTypeValidation;
						selectedFormat = format;
						break;
					}
				}

				if (accepted)
				{
					//con la carta di identità del documento il controllo non è piu bloccante.
					/*
					if (Validation)
					{
						if (fileContent != null)
						{
							logger.Debug(String.Format("Validazione Start {0}, len {1}", fileName, fileContent.Length));
							Sa_Utils.FileTypeFinder ff = new Sa_Utils.FileTypeFinder();
							string fileExtension = ff.FileType(fileContent);
							logger.Debug(String.Format("Validazione End {0}, Declared :{1}  Found:{2}", fileName, extension, fileExtension));
							//if (!extension.ToLower().Contains(fileExtension.ToLower()))
							if (!fileExtension.ToLower().Contains(extension.ToLower()))
							{
								errorMessage = string.Format(string.Format("Il formato file dichiarato '{0}' Non è quello riscontrato '{1}'", extension.ToLower(), fileExtension.ToLower()));
								logger.Debug(errorMessage);
								accepted = false;
							}

						}
					}
					*/
					// Verifica se il formato è valido rispetto al tipo documento 
					if ((selectedFormat.DocumentType == DocsPaVO.FormatiDocumento.DocumentTypeEnum.Grigio && isProtocollo)
						|| (selectedFormat.DocumentType == DocsPaVO.FormatiDocumento.DocumentTypeEnum.Protocollo && !isProtocollo))
					{
						accepted = false;

						errorMessage = string.Format("Il formato file '{0}' non è valido per un {1}", extension, (isProtocollo ? "protocollo" : "documento grigio"));
					}
				}
				else
				{
					errorMessage = string.Format("Il formato file '{0}' non è tra quelli accettati in amministrazione", extension);
				}
			}
		}
		else
		{
			accepted = true;
		}

		return accepted;
	}

	static string getExts(string infile)
	{
		string fname = System.IO.Path.GetFileNameWithoutExtension(infile);

		if (fname.Contains('.'))
			fname = fname.Remove(fname.IndexOf('.'));

		string extname = infile.Replace(fname, string.Empty);
		return extname;
	}

	private static bool isDocAllegato(DocsPaVO.documento.SchedaDocumento sch, ref DocsPaVO.documento.Allegato retDocAllegato)
	{
		if (sch.documentoPrincipale == null) return false;
		ArrayList allegati = AllegatiManager.getAllegati(sch.documentoPrincipale.docNumber, string.Empty);
		foreach (DocsPaVO.documento.Allegato allegato in allegati.ToArray())
			if (sch.docNumber == allegato.docNumber) { retDocAllegato = allegato; return true; }
		return false;
	}

	public static string removeIllegalChars(string filename, bool normalizeDotsAndSpacesToo)
	{

		if (string.IsNullOrEmpty(filename))
			return filename;

		string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

		foreach (char c in invalid)
			filename = filename.Replace(c.ToString(), "_");

		if (normalizeDotsAndSpacesToo)
		{
			filename = filename.Replace(".", "_");
			filename = filename.Replace(" ", "_");
		}
		return filename;
	}

	public static string Truncate(string value, int maxLength)
	{
		return value.Length <= maxLength ? value : value.Substring(0, maxLength);
	}

	public static DocsPaVO.documento.FileDocumento getFile(
			DocsPaVO.documento.FileRequest objFileRequest,
			DocsPaVO.utente.InfoUtente objSicurezza, string basePath = "")
	{

		return getFile(objFileRequest, objSicurezza, true, false, basePath);
	}

	public static DocsPaVO.documento.FileDocumento getFileFirmato(
			DocsPaVO.documento.FileRequest objFileRequest,
			DocsPaVO.utente.InfoUtente objSicurezza, bool conservazione, string basePath = "")
	{
		return getFile(objFileRequest, objSicurezza, false, conservazione, basePath);
	}

	private static DocsPaVO.documento.FileDocumento getFile(
	  DocsPaVO.documento.FileRequest objFileRequest,
	  DocsPaVO.utente.InfoUtente objSicurezza,
	  bool verificaFileFirmato, bool conservazione, string basePath = "")
	{
		logger.Information("BEGIN");
		//logger.DebugFormat("Parametri verificaFileFirmato {0}  | conservazione {1}", verificaFileFirmato, conservazione);
		bool isContainer = false;
		// Verifica se il file è acquisito nell'ambito di un repository di sessione
		if (objFileRequest.repositoryContext != null)
        {
            logger.Debug($"objFileRequest.repositoryContext : {objFileRequest.repositoryContext}");
            DocsPaVO.documento.FileDocumento fileDocument = null;
			// Reperimento del file in un documento quando ancora non è stato salvato,
			//// pertanto è disponibile un repository temporaneo valido solamente nell'ambito dell'inserimento
			if (CacheFileManager.isActiveCaching(objSicurezza.idAmministrazione))
			{
				if (CacheFileManager.isFileInCache(objFileRequest.docNumber))
				{
					string path = CacheFileManager.ricercaPathCaching(objFileRequest.docNumber, objFileRequest.versionId, objSicurezza.idAmministrazione);
					if (!string.IsNullOrEmpty(path))
						objFileRequest.path = path;
				}

			}

			SessionRepositoryFileManager fileManager = SessionRepositoryFileManager.GetFileManager(objFileRequest.repositoryContext);

			fileDocument = fileManager.GetFile(objFileRequest);

			if (fileDocument != null && fileDocument.name.ToUpper().EndsWith(".P7M") && verificaFileFirmato)
            {
                logger.Information("VerifyFileSignature");
                VerifyFileSignature(fileDocument, null, basePath);
			}
			logger.Information("END");
			return fileDocument;
		}

		DocsPaVO.documento.FileDocumento fileDoc = new DocsPaVO.documento.FileDocumento();
		fileDoc.path = objFileRequest.docServerLoc + objFileRequest.path;
		fileDoc.name = objFileRequest.fileName;

		//
		if (DocsPaVO.Settings.AppSettings.Instance.documentale.ToUpper() == "FILENET")
		{
			DocsPaDocumentale.Documentale.DocumentManager FNdoc = new DocsPaDocumentale.Documentale.DocumentManager(objSicurezza);
			fileDoc.name = FNdoc.GetOriginalFileName(objFileRequest.docNumber, objFileRequest.versionId);
		}
		fileDoc.nomeOriginale = getOriginalFileName(objSicurezza, objFileRequest);

		populateNameAndContent(fileDoc);
		logger.Debug("Full name: " + fileDoc.fullName);
		logger.Debug("idAmministrazione: " + objSicurezza.idAmministrazione);
		logger.Debug("Library: " + DocsPaDB.Utils.Personalization.getInstance(objSicurezza.idAmministrazione).getLibrary());

		string docNumber = objFileRequest.docNumber;
		// string versionId = objFileRequest.versionId;	
		string version_label = objFileRequest.versionLabel;

		// Acquisizione del file 
		//// modalita caching
		string messageError = string.Empty;

		if (CacheFileManager.isActiveCaching(objSicurezza.idAmministrazione))
		{
			if (!CacheFileManager.getFile(ref objSicurezza, ref objFileRequest, fileDoc, out messageError))
			{
				logger.Debug("Errore nella gestione del File (getFile)");
				throw new Exception(messageError);
			}
		}
		else// modalità classica
		{
			DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(objSicurezza);

			if (!documentManager.GetFile(ref fileDoc, ref objFileRequest))
			{
				logger.Debug("Errore nella gestione del File (getFile)");
				throw new Exception();
			}
		}

        if (fileDoc != null && fileDoc.name.ToUpper().EndsWith(".TSR") && !conservazione)
		{
			//se il file è .TSR viene verificata la marca temporale
			if (verificaFileFirmato)
				VerifyFileTimeStamp(fileDoc);
			logger.Information("END");
			return fileDoc;
		}


		if (GestioneTSAttacced)
		{
			//logger.Debug("Gestione sbusto time stamp attiva");
			//logger.DebugFormat("Parametri EXT {0},  VFF {1}  path{2}", fileDoc.name.ToUpper(), verificaFileFirmato,fileDoc.path);
			// Gestione file timestampati

			//Faillace 10/07
			//documentum , in fase di putfile , non da la giusta estensione per i file TSD nella components (sotto investigazione)
			//prendo quindi il nome originale e lo processo
			//fileDoc.name = fileDoc.nomeOriginale;

			if (fileDoc != null && fileDoc.name.ToUpper().EndsWith(".TSD") && verificaFileFirmato)
			{
				EstrazioneTSD(objFileRequest, fileDoc, objSicurezza);
				//logger.DebugFormat("Sbusto TSD nuova len{0}", fileDoc.length);
				populateNameAndContent(fileDoc);
				isContainer = true;
			}
			if (fileDoc != null && fileDoc.name.ToUpper().EndsWith(".M7M") && verificaFileFirmato)
			{
				EstrazioneM7M(objFileRequest, fileDoc, objSicurezza);
				//logger.DebugFormat("Sbusto M7M nuova len{0}", fileDoc.length);
				populateNameAndContent(fileDoc);
				isContainer = true;
			}
		}

		if (fileDoc != null && fileDoc.name.ToUpper().EndsWith(".P7M") && verificaFileFirmato)
		{
			// Se file .P7M, viene fatta la verifica della firma digitale del file
			try
			{
				VerifyFileSignature(fileDoc, null, basePath);
			}
			catch (Exception ex)
			{
				logger.Debug("Eroore in verifica Firma " + ex.Message);
			}
			//logger.DebugFormat("Sbusto CADES nuova len{0}", fileDoc.length);
			if (fileDoc.signatureResult != null)
			{
				ArrayList tsAl = TimestampManager.getTimestampsDoc(objSicurezza, objFileRequest);
				List<DocsPaVO.documento.TSInfo> tsLst = new List<DocsPaVO.documento.TSInfo>();

				foreach (DocsPaVO.documento.TimestampDoc tsdoc in tsAl)
				{
					if (!string.IsNullOrEmpty(tsdoc.TSR_FILE))
					{
						DocsPaVO.documento.TSInfo info = new VerifyTimeStamp().getTSCertInfo(tsdoc.TSR_FILE);
						tsLst.Add(info);
					}
					else
					{
						logger.Error("La marca è nulla!!! errore nel DB");
					}
				}

				if (tsLst.Count > 0)
					fileDoc.signatureResult.DocumentTimeStampInfo = tsLst.ToArray();
			}
			logger.Information("END");
			return fileDoc;
		}
		else
		{
			if (fileDoc.name.ToUpper().EndsWith(".PDF") && GestionePades)
			{
				if (BusinessLogic.Documenti.DigitalSignature.Pades_Utils.Pades.IsPdfPades(fileDoc) || IsSignedPades(fileDoc))
				{
					try
					{
						bool verifyFileSignature_External = !string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_VERIFY_FILESIGNATURE_EXTERNAL")) && DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_VERIFY_FILESIGNATURE_EXTERNAL").Equals("1");
						if (verifyFileSignature_External && verificaFileFirmato)
						{
							//VerifyFileSignature(fileDoc, DateTime.Now); //Verifica esterna, se fallisce faccio la verifica interna
							string name = fileDoc.name;
							byte[] content = fileDoc.content;

							VerifySignatureResult resultTemp = VerifyFileSignaturePades_External(fileDoc, DateTime.Now, basePath);
							if (resultTemp == null || resultTemp.StatusCode == -100 || resultTemp.PKCS7Documents == null || resultTemp.PKCS7Documents.Length == 0)
							{
								fileDoc.name = name;
								fileDoc.content = content;
								BusinessLogic.Documenti.DigitalSignature.Pades_Utils.Pades.VerifyPadesSignature(fileDoc);
								// VerifyFileSignature(fileDoc, null);
							}
							else
							{
								fileDoc.content = content;
								fileDoc.signatureResult = resultTemp;
							}
						}
						else
						{
							BusinessLogic.Documenti.DigitalSignature.Pades_Utils.Pades.VerifyPadesSignature(fileDoc);
						}
						//E' una firma pades elaboriamo una VSR per il pades

						/* MEV 2020: Se la chiave è attiva controllo sempre con il servizio esterno, per risolvere i problemi sia di pades non visibili e sia di cades con estensione p7m mancante
						//ItextSharp in alcuni casi non restituisce tutte le firme PADES, in questo caso chiamo il servizio esterno
						if(verificaFileFirmato)
						{
							VerifySignatureResult resultTemp = VerifyFileSignaturePades_External(fileDoc, DateTime.Now);
							if (resultTemp != null && resultTemp.PKCS7Documents != null)
							{
								if (resultTemp.PKCS7Documents.Count() > fileDoc.signatureResult.PKCS7Documents.Count())
								{
									fileDoc.signatureResult = resultTemp;
								}
								else if (resultTemp.PKCS7Documents.Count() == fileDoc.signatureResult.PKCS7Documents.Count())
								{
									for(int i =0; i < resultTemp.PKCS7Documents.Count(); i++)
									{
										if(resultTemp.PKCS7Documents[i].SignersInfo.Count() > fileDoc.signatureResult.PKCS7Documents[i].SignersInfo.Count())
										{
											fileDoc.signatureResult = resultTemp;
											break;
										}
									}
								}
							}
						}
							*/
					}
					catch (Exception ex)
					{
						logger.Debug("Errore nella verifica della firma PADES: {0} \r\n{1}", ex.Message, ex.StackTrace);
					}
				}

			}
			else
			{
				if (fileDoc.name.ToUpper().EndsWith(".XML") && verificaFileFirmato)
				{
					VerifyFileSignatureXADES(fileDoc, DateTime.MinValue, basePath);
				}
			}
			// verifico l'impronta, solamente se non si è in repository di sessione
			string versionId = objFileRequest.versionId;
			string impronta = "";

			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

			if (CacheFileManager.isActiveCaching(objSicurezza.idAmministrazione))
			{
				DocsPaDB.Query_DocsPAWS.Caching cache = new DocsPaDB.Query_DocsPAWS.Caching();
				cache.GetImpronta(out impronta, versionId, objFileRequest.docNumber, objSicurezza.idAmministrazione);
				if (string.IsNullOrEmpty(impronta))
				{
					if (verificaPath(objFileRequest.docServerLoc))
					{
						logger.Information("END");
						return fileDoc;
					}
				}
			}
			else
			{
				// verifico l'impronta, solamente se non si è in repository di sessione


				doc = new DocsPaDB.Query_DocsPAWS.Documenti();

				doc.GetImpronta(out impronta, versionId, objFileRequest.docNumber);

				//verifico i path dei file per vedere se bisogna fare il controllo sull'impronta !!! ATTENZIONE
				if (string.IsNullOrEmpty(impronta))
				{
					if (verificaPath(objFileRequest.docServerLoc))
					{
						logger.Information("END");
						return fileDoc;
					} 
				}
			}
			if (
				impronta.Equals(DocsPaUtils.Security.CryptographyManager.CalcolaImpronta(fileDoc.content)) ||
				impronta.Equals(DocsPaUtils.Security.CryptographyManager.CalcolaImpronta256(fileDoc.content)) ||
				isContainer)
			{
				logger.Information("END");
				return fileDoc;
			}
			else
			{
				logger.Information("END");
				return null;
			}
		}
	}

	public static bool IsValidFileContent(DocsPaVO.documento.FileDocumento fileDocument)
	{
		bool isValid = false;

		string fileName = string.Empty;

		if (!string.IsNullOrEmpty(fileDocument.fullName))
			fileName = fileDocument.fullName;
		else
			fileName = fileDocument.name;

		logger.Debug("Validazione Start {0}", fileName);
#if false  // usa Sa_Utils
		Sa_Utils.FileTypeFinder ff = new Sa_Utils.FileTypeFinder();
		string fileExtension = ff.FileType(fileDocument.content);

		string extension = System.IO.Path.GetExtension(fileName).Replace(".", string.Empty);

		//normalizzo l'estensine per file strani html /htm tif/tiff etc etc
		extension = normalizeDblExtensions(extension);

		logger.Debug(String.Format("Validazione End {0}, Declared :[{1}]  Found:[{2}]", fileName, extension, fileExtension));
		isValid = fileExtension.ToLower().Contains(extension.ToLower()); 
#endif

		return isValid;
	}

	public static void UpdateInfoFileAcquisito(DocsPaVO.documento.FileInformation fileInfo, FileRequest fileRequest, string varNomeOriginale, string fileExtFileTypeFinder, DocsPaVO.utente.InfoUtente infoUtente, bool noNotifica = false)
	{
		try
		{
			bool notifica = false;
			InfoFile infoFile = new InfoFile();
			infoFile.IdProfile = fileRequest.docNumber;
			infoFile.VersionId = fileRequest.versionId;
			infoFile.NomeFile = varNomeOriginale;
			infoFile.Estensione = getEstensioneIntoSignedFile(fileRequest.fileName); 
			if (fileInfo.FileFormatOK.Equals(FileInformation.VerifyStatus.Invalid))
				infoFile.EstensioneConforme = false;

			if (fileInfo.NoMacroOrExe.Equals(FileInformation.VerifyStatus.Invalid))
			{
				infoFile.Conforme = false;
				if (fileExtFileTypeFinder.ToLower().Contains("+macro") || fileExtFileTypeFinder.ToLower().Contains("docm") ||
					fileExtFileTypeFinder.ToLower().Contains("xlsm") || fileExtFileTypeFinder.ToLower().Contains("pptm"))
				{
					infoFile.ContieneMacro = true;
				}
				infoFile.ContieneForms = fileExtFileTypeFinder.ToLower().Contains("+forms");
				infoFile.ContieneJavascript = fileExtFileTypeFinder.ToLower().Contains("+javascript");

				if (infoFile.ContieneMacro)
					infoFile.DescrizioneInfoFile += "MACRO";

				if (infoFile.ContieneForms)
					infoFile.DescrizioneInfoFile += string.IsNullOrEmpty(infoFile.DescrizioneInfoFile) ? "FormPDF" : ",FormPDF";

				if (infoFile.ContieneJavascript)
					infoFile.DescrizioneInfoFile += string.IsNullOrEmpty(infoFile.DescrizioneInfoFile) ? "JAVASCRIPT" : ",JAVASCRIPT";
			}

			if (!infoFile.EstensioneConforme)
				infoFile.DescrizioneInfoFile += string.IsNullOrEmpty(infoFile.DescrizioneInfoFile) ? "NON_CONFORME" : ",NON_CONFORME";
			//infoFile.DescrizioneInfoFile += string.IsNullOrEmpty(infoFile.DescrizioneInfoFile) ? "Estensione non conforme al formato di file" : ",Estensione non conforme al formato di file";

			infoFile.DataAcquisizione = fileRequest.dataAcquisizione;

			DocsPaDB.Query_DocsPAWS.Documenti docs = new DocsPaDB.Query_DocsPAWS.Documenti();
			if (!infoFile.Conforme)
			{
				infoFile.IdDocumentoPrincipale = docs.GetIdDocumentoPrincipale(infoFile.IdProfile);
				notifica = true && !noNotifica;
			}

			//Aggiorno le informazione del file acquisito
			docs.UpdateInfoFile(infoFile, notifica);
		}
		catch (Exception e)
		{
			logger.Error("Errore in UpdateInfoFileAcquisito: " + e.Message);
		}
	}

	public static DocsPaVO.documento.Applicazione getApplicazione(string estensione)
	{
		logger.Debug("getApplicazione");

		DocsPaVO.documento.Applicazione res = new DocsPaVO.documento.Applicazione();
		DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
		doc.GetExt(estensione, ref res);

		return res;

		#region Codice Commentato
		/*try
		{
			db.openConnection();
			string labelString="SELECT SYSTEM_ID, DEFAULT_EXTENSION FROM APPS ORDER BY SYSTEM_ID DESC";
			logger.Debug(labelString);
			db.fillTable(labelString,dataSet,"APPS");
			string lastSysId=dataSet.Tables["APPS"].Rows[0]["SYSTEM_ID"].ToString();
			System.Data.DataRow[] extRows = dataSet.Tables["APPS"].Select("DEFAULT_EXTENSION='"+estensione.ToUpper()+"'");
			
			if(extRows.Length==0)
			{
				int sysId=Int32.Parse(lastSysId)+1;
				string insertString="INSERT INTO APPS (SYSTEM_ID,";
				insertString=insertString+"APPLICATION,DESCRIPTION,FILING_SCHEME,DEFAULT_EXTENSION) VALUES (";
				insertString=insertString+sysId+",";
				insertString=insertString+"'GEN_"+estensione.ToUpper()+"','GEN_"+estensione.ToUpper()+"',2,'"+estensione.ToUpper()+"')";
				string insertString = obj.insertApp(sysId, estensione.ToUpper());
				logger.Debug(insertString);
				db.insertLocked(insertString,"APPS");
				res.systemId=sysId.ToString();
				res.estensione=estensione;

			}
			else 
			{
				res.estensione = estensione;
				res.systemId = extRows[0]["SYSTEM_ID"].ToString();
			}

			db.closeConnection();
			logger.Debug("Fine getApplicazione");
			
			return res;
		}
		catch(Exception e)
		{
			logger.Debug(e.Message);
			db.closeConnection();
			
			throw new Exception("F_System");
		}*/
		#endregion
	}

	public static string getContentType(string fileName)
	{
		string[] extArr = fileName.Split('.');
		string ext = extArr[extArr.Length - 1].ToLower();
		//string contentType = DocsPaUtils.Configuration.DocumentTypeManager.GetValue(ext);
		string contentType = null;
		DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
		ArrayList contents = new ArrayList();
		doc.GetApplicazioni(ext, contents);
		if (contents != null && contents.Count > 0)
		{
			DocsPaVO.documento.Applicazione appl = (DocsPaVO.documento.Applicazione)contents[0];
			if (appl != null)
			{
				contentType = appl.mimeType;
			}
		}

		if (contentType == null)
		{
			contentType = "application/x-" + ext;
		}

		return contentType;
	}

	public static bool putFile(ref DocsPaVO.documento.FileRequest fileRequest,
								DocsPaVO.documento.FileDocumento fileDoc,
								DocsPaVO.utente.InfoUtente objSicurezza,
								out string errorMessage, bool processFileInfo = true, string repositoryRootPath = "", string temporaryRootPath = "")
	{
		return putFile(ref fileRequest, fileDoc, objSicurezza, true, out errorMessage, repositoryRootPath, temporaryRootPath, processFileInfo);
	}

	private static void CopySignedFileToInputFolder(DocsPaVO.documento.FileDocumento fileDoc, string inputFile)
	{
		FileStream stream = new FileStream(inputFile, FileMode.Create, FileAccess.Write);
		stream.Write(fileDoc.content, 0, fileDoc.content.Length);
		stream.Flush();
		stream.Close();
		stream = null;
	}

	public static string getEstensioneIntoSignedFile(string fullname)
	{
		string retValue = string.Empty;

		// Reperimento del nome del file con estensione
		string fileName = new System.IO.FileInfo(fullname).Name;

		string[] items = fileName.Split('.');

		for (int i = (items.Length - 1); i >= 0; i--)
		{
			if (!(items[i].ToUpper().EndsWith("P7M") ||
				items[i].ToUpper().EndsWith("TSD") ||
				items[i].ToUpper().EndsWith("M7M"))
				)
			{
				retValue = items[i];
				break;
			}
		}
		return retValue;
	}

	public static DocsPaVO.documento.FileDocumento getVoidFileConSegnatura(DocsPaVO.documento.FileRequest objFileRequest, DocsPaVO.documento.SchedaDocumento sch, DocsPaVO.utente.InfoUtente infoutente, string xmlPath, DocsPaVO.documento.labelPdf position)
	{
		logger.Information("BEGIN");
		DocsPaVO.documento.FileDocumento fileDoc = new DocsPaVO.documento.FileDocumento();
		fileDoc = addVoidEtic(ref fileDoc, sch, infoutente, position);
		logger.Information("END");
		return fileDoc;
	}

	private static DocsPaVO.documento.FileDocumento addVoidEtic(ref DocsPaVO.documento.FileDocumento file, DocsPaVO.documento.SchedaDocumento sch, DocsPaVO.utente.InfoUtente utente, DocsPaVO.documento.labelPdf labelPdf)
	{
		int posX = 0;
		int posY = 0;
		PdfInfo info = getPdfInfo(file);
		bool isPades = info.IsSigned;

		// se è pdfa ed è pure firmato non posso fare append perchè la conformità pdfa sarebbe corrotta
		if (info.IsSigned && info.IsPdfA)
			isPades = false;

		// se sono presenti i dati biometrici ritorno il fileinfo senza modificare il file
		if (info.HasBiometricData)
		{
			file.LabelPdf = labelPdf;
			file.LabelPdf.default_position = "";
			return file;
		}

		//isPades = false;
		//^^^^^^^^^^^^^^ Interruttore , commentare per attivare lo stamping su firma

		byte[] rtn = null;
		//verranno scritti i file temporanei in un subfolder di REPORTS_PATH
		string basePathFiles = DocsPaVO.Settings.AppSettings.Instance.REPORTS_PATH;
		//string fullPath = basePathFiles + @"\EtichettaPdf";
		string fullPath = basePathFiles.Replace("%DATA", "EtichettaPdf");
		//standard naming per il file temporaneo
		string fileName = @"\" + sch.docNumber + "_" + utente.userId + ".pdf";
		//combinazione path & filename
		string fullPathFileName = fullPath + fileName;
		System.IO.FileStream fileStream = null;
		System.IO.FileStream fs = null;
		//estraggo la posizione dall'oggetto Label per mantenere inalterato il funzionamento del metodo
		string position = null;

		if (labelPdf != null)
		{
			position = labelPdf.position;
			//aggiungo il carattere ed il colore selezionati all'oggetto label del fileDoc
			if (!string.IsNullOrEmpty(labelPdf.sel_font))
			{
				file.LabelPdf.sel_font = labelPdf.sel_font;
			}
			if (!string.IsNullOrEmpty(labelPdf.sel_color))
			{
				file.LabelPdf.sel_color = labelPdf.sel_color;
			}
		}
		DocsPaVO.amministrazione.InfoAmministrazione currAmm = Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(utente.idAmministrazione);

		try
		{
			//verifico esistenza directory
			if (!System.IO.Directory.Exists(fullPath))
			{
				System.IO.Directory.CreateDirectory(fullPath);
			}
			//verifico esistenza file
			if (System.IO.File.Exists(fullPathFileName))
			{
				System.IO.File.Delete(fullPathFileName);
			}

			//verifico la posizione prescelta
			if ((position == null) || (position == ""))
			{
				//caricamento preferenze di default di LabelPDF
				loadXmlLabelProperties(file, null, currAmm);
			}
			else
			{
				//caricamento preferenze utente per LabelPDF
				loadXmlLabelProperties(file, position, currAmm);
			}

			if (file.content == null)
			{
#if false   // usa iTextSharp
				iTextSharp.text.Document document = new iTextSharp.text.Document();
				FileStream newStream = new FileStream(fullPathFileName, FileMode.Create);
				PdfWriter.GetInstance(document, newStream);
				document.Open();
				document.Add(new iTextSharp.text.Paragraph(" "));
				document.Close();

				newStream = new FileStream(fullPathFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
				file.content = new byte[newStream.Length];
				newStream.Read(file.content, 0, file.content.Length);
				newStream.Flush();
				newStream.Close();
				File.Delete(fullPathFileName);

#endif                
			}

			System.IO.MemoryStream ms = new System.IO.MemoryStream(file.content, true);

			//Stringa del timbro o della segnatura
			int maxT = 0;
			string maxTimbro = "";
			string escape = "\n";
			//In base all'informazione nel frontEnd decidere se devo caricare la segnatura oppure il Timbro!!!
			//...mettendo a true l'ultimo parametro carico il timbro!!!
			string dati = GetDatiEtichetta(utente, currAmm, labelPdf, sch, file.signatureResult);

			//se l'etichetta è vuota significa che il documento non è stato ancora creato, metto quindi una string fittizia
			if (string.IsNullOrEmpty(dati))
				dati = System.DateTime.Now.ToString();
			//rimuovo l'ultimo separatore!!!
			dati = dati.TrimEnd(Convert.ToChar(escape));
			string[] rTimbro = dati.Split(Convert.ToChar(escape));
			for (int t = 0; t < rTimbro.Length; t++)
			{
				if (rTimbro[t].Length > maxT)
				{
					maxTimbro = rTimbro[t];
					maxT = rTimbro[t].Length;
				}
				//Rimuovo gli spazi bianchi dai dati del timbro che in caso di orientamento
				//verticale creano un disallineamento!
				rTimbro[t] = rTimbro[t].Trim();
			}
			//se dal frontEnd ho valorizzato la rotazione allora la sostituisco a quella configurata
			if (labelPdf.label_rotation != String.Empty)
			{
				currAmm.Timbro_rotazione = labelPdf.label_rotation;
			}
			//valorizzo i dati da passare al frontEnd
			file.LabelPdf.label_rotation = currAmm.Timbro_rotazione;
			file.LabelPdf.orientamento = labelPdf.orientamento;
			file.LabelPdf.tipoLabel = labelPdf.tipoLabel;

			// posizione 0 del memory stream
			ms.Position = 0;
#if false   // usa iTextSharp
			iTextSharp.text.pdf.PdfReader prd = new PdfReader(ms);

			// dimensioni della prima pagina del documento
			iTextSharp.text.Rectangle rect = new iTextSharp.text.Rectangle(prd.GetPageSize(1));

			//font di stampa
			BaseFont bf = castFontTypePreferences(ref file);

			// verifico le dimensioni massime consentite per la sovrastampa				

			// calcola l'occupazione in pixel della segnatura
			int segnaPix = Convert.ToInt32(bf.GetWidthPoint(maxTimbro, Convert.ToInt32(file.LabelPdf.font_size)));

			//viene allineata la dimensione massima utilizzabile per la sovrastampa
			// sottraendo la massima occupazione della segnatura con un sfrido di 5 px
			int maxWidth = (Convert.ToInt32(rect.Width) - segnaPix) - 5;

			//inserisco le info della pagina PDF sul VO
			file.LabelPdf.pdfHeight = rect.Height.ToString();
			file.LabelPdf.pdfWidth = Convert.ToString(maxWidth);


			#region posizionamento con valori in Pixel
			// posizionamento in Pixel
			posX = 0;
			posY = 0;

			// verifica dimensioni coerenti
			DocsPaVO.documento.position default_pos = null;

			for (int i = 0; i < file.LabelPdf.positions.Count; i++)
			{
				default_pos = (DocsPaVO.documento.position)file.LabelPdf.positions[i];

				if (default_pos.posName == file.LabelPdf.default_position)
				{
					posX = Convert.ToInt32(default_pos.PosX);
					posY = Convert.ToInt32(rect.Top) - Convert.ToInt32(default_pos.PosY);

					// verifico se la prescelta X rientra nel dim del foglio
					if (posX >= Convert.ToInt32(file.LabelPdf.pdfWidth))
					{
						posX = Convert.ToInt32(file.LabelPdf.pdfWidth);
					}
					// verifico se la prescelta Y rientra nel dim del foglio
					if (posY <= 0)
					{
						posY = Convert.ToInt32(file.LabelPdf.font_size);
					}
					break;
				}

			}

			fs = new System.IO.FileStream(fullPathFileName, System.IO.FileMode.OpenOrCreate);
			iTextSharp.text.pdf.PdfStamper stamp = new PdfStamper(prd, fs);

			stamp.Writer.Open();
			PdfContentByte cb = null;

			if (!isPades)
				cb = stamp.GetOverContent(1);
			else
				cb = new iTextSharp.text.pdf.PdfContentByte(stamp.Writer);

			iTextSharp.text.Color colore = castFontColorPreferences(ref file);
			//colore
			cb.SetColorFill(colore);

			if (!isPades)
			{
				cb.BeginText();
				//font
				cb.SetFontAndSize(bf, Convert.ToInt32(file.LabelPdf.font_size));


				//dimensioni pagina
				iTextSharp.text.Rectangle size = cb.PdfDocument.PageSize;

				cb.SetTextMatrix(posX, posY);

				//Come già fatto per la posizione gestire la rotazione in base all'informazione passata dal FrontEnd!
				writeLabelOnNonSignedPDF(file, ref posX, ref posY, currAmm, rTimbro, segnaPix, default_pos, cb);

				//cb.ShowText(sch.protocollo.segnatura);
				cb.EndText();
			}
			else
			{
				int offset = 0;
				int AppoX = posX;
				int AppoY = posY;
				int k = 0;
				float rotaz = 0;
				string Timbro_rotazione = currAmm.Timbro_rotazione;
				string posName = default_pos.posName;
				int timbroLength = rTimbro.Length;
				string font_size = file.LabelPdf.font_size;

				calcolaRettangoloEtichetta(ref posX, ref posY, segnaPix, ref offset, AppoX, AppoY, k, ref rotaz, Timbro_rotazione, posName, timbroLength, font_size);

				string segnatura = "";
				int maxLen = 0;
				foreach (string lineaTimbro in rTimbro)
				{
					if (lineaTimbro.Length > maxLen)
						maxLen = lineaTimbro.Length;
					segnatura += lineaTimbro + "\r";
				}

				maxLen *= 8;

				iTextSharp.text.Rectangle annoRect = new iTextSharp.text.Rectangle(posX, posY, posX + 195, posY - 80);

				PdfAnnotation annot = new PdfAnnotation(stamp.Writer, annoRect);
				annot.Put(iTextSharp.text.pdf.PdfName.SUBTYPE, iTextSharp.text.pdf.PdfName.FREETEXT);
				annot.BorderStyle = new iTextSharp.text.pdf.PdfBorderDictionary(0, 0);

				annot.Put(iTextSharp.text.pdf.PdfName.CONTENTS, new iTextSharp.text.pdf.PdfString(segnatura, iTextSharp.text.pdf.PdfObject.TEXT_UNICODE));
				annot.DefaultAppearanceString = cb;
				annot.Flags = iTextSharp.text.pdf.PdfAnnotation.FLAGS_PRINT | iTextSharp.text.pdf.PdfAnnotation.FLAGS_READONLY | iTextSharp.text.pdf.PdfAnnotation.FLAGS_NOZOOM | iTextSharp.text.pdf.PdfAnnotation.FLAGS_LOCKED;
				stamp.AddAnnotation(annot, 1);
			}
			stamp.Close();
			#endregion

			fileStream = new System.IO.FileStream(fullPathFileName, System.IO.FileMode.Open);
			int fileLength = (int)fileStream.Length;
			rtn = new byte[fileLength];
			fileStream.Read(rtn, 0, fileLength);
			fileStream.Flush();
			fileStream.Close();
			file.length = fileLength;
			file.fullName = fileName;
			file.name = fileName;
			file.estensioneFile = "PDF";
			file.path = fullPath;
			file.content = rtn;
			file.contentType = "application/pdf"; 
#endif
			return file;
		}
		catch (Exception ex)
		{
			if (System.IO.File.Exists(fullPathFileName))
			{
				System.IO.File.Delete(fullPathFileName);
			}
			logger.Debug(ex, "Errore addVoidEtic");
			throw ex;
		}
		finally
		{
			if (System.IO.File.Exists(fullPathFileName))
			{

				System.IO.File.Delete(fullPathFileName);
			}
			if (fileStream != null)
			{
				fileStream.Close();
				fileStream = null;
			}

			if (fs != null)
			{
				fs.Close();
				fs = null;
			}
		}
	}

	private static PdfInfo getPdfInfo(DocsPaVO.documento.FileDocumento fileDoc)
	{
		return getPdfInfo(fileDoc.content);
	}

	private static PdfInfo getPdfInfo(byte[] content)
	{
		PdfInfo retval = new PdfInfo();
		try
		{
#if false   // usa iTextSharp
			iTextSharp.text.pdf.PdfReader r = new iTextSharp.text.pdf.PdfReader(content);
			retval.version = "1." + r.PdfVersion;
			iTextSharp.text.pdf.AcroFields af = r.AcroFields;
			if (!String.IsNullOrEmpty(r.JavaScript))
				retval.HasJava = true;

			//get metadata per controllo conformità e PDFa


			if (r.Metadata != null)
			{


				try
				{

					string part = StrongXmlReadElementText("pdfaid:part", r.Metadata);
					if (!string.IsNullOrEmpty(part))
					{
						retval.IsPdfA = true;
						string conf = StrongXmlReadElementText("pdfaid:conformance", r.Metadata);
						if (!string.IsNullOrEmpty(conf))
							retval.conformance = String.Format("PDF/A {0}{1}", part, conf);

					}
					string PdfE = StrongXmlReadElementText("pdfe:ISO_PDFEVersion", r.Metadata);
					if (!string.IsNullOrEmpty(PdfE))
						retval.conformance = PdfE;


					string pdfX = StrongXmlReadElementText("pdfxid:GTS_PDFXVersion", r.Metadata);
					if (!string.IsNullOrEmpty(pdfX))
					{
						retval.conformance = pdfX;
						string pdfVT = StrongXmlReadElementText("pdfvtid:GTS_PDFVTVersion", r.Metadata);
						retval.conformance += "|" + pdfVT;
					}
				}
				catch
				{

				}
			}
			if (af != null)
			{
				System.Collections.ArrayList sigs = af.GetSignatureNames();
				//controllo se firmato
				if (sigs.Count > 0)
					retval.IsSigned = true;

				//controllo dati biometrici e grafometrici
				foreach (string name in sigs)
				{
					bool hasBio = af.GetSignatureDictionary(name).Contains(new iTextSharp.text.pdf.PdfName("Prop_BiometricData"));
					if (hasBio)
					{
						retval.HasBiometricData = true;
						break;
					}
				}
			}
#endif

			return retval;
		}
		catch
		{
			return retval;
		}
	}

	/// <summary>
	/// Metodo che sfrutta la lettura da DB invece che da file XML: è il nuovo metodo implementato per la versione
	/// che prevede anche il timbro su pdf oltre che la segnatura.
	/// </summary>
	/// <param name="file"></param>
	/// <param name="position"></param>
	/// <param name="Amm"></param>
	public static void loadXmlLabelProperties(DocsPaVO.documento.FileDocumento file, string position, DocsPaVO.amministrazione.InfoAmministrazione Amm)
	{
		string delimitatore = "-";
		try
		{
			//carico info Font
			DocsPaVO.amministrazione.carattere carat = new DocsPaVO.amministrazione.carattere();
			for (int i = 0; i < Amm.Timbro.carattere.Count; i++)
			{
				carat = (DocsPaVO.amministrazione.carattere)Amm.Timbro.carattere[i];
				//Se da front-end ho selezionato un tipo di font lo utilizzo altrimenti uso quello
				//configurato in amministrazione...
				if (string.IsNullOrEmpty(file.LabelPdf.sel_font))
				{
					file.LabelPdf.sel_font = Amm.Timbro_carattere;
				}
				//if (carat.id == Amm.Timbro_carattere)
				if (carat.id == file.LabelPdf.sel_font)
				{
					file.LabelPdf.font_type = carat.caratName;
					file.LabelPdf.font_size = carat.dimensione;
				}
			}
			DocsPaVO.amministrazione.color colore = new DocsPaVO.amministrazione.color();
			for (int j = 0; j < Amm.Timbro.color.Count; j++)
			{
				colore = (DocsPaVO.amministrazione.color)Amm.Timbro.color[j];
				//Se selezionato uso il colore scelto da front-end
				if (string.IsNullOrEmpty(file.LabelPdf.sel_color))
				{
					file.LabelPdf.sel_color = Amm.Timbro_colore;
				}
				//if (colore.id == Amm.Timbro_colore)
				if (colore.id == file.LabelPdf.sel_color)
				{
					file.LabelPdf.font_color = colore.colName;
				}
			}
			file.LabelPdf.label_rotation = Amm.Timbro_rotazione;

			#region LoadDefaulPosition
			// carico le 4 posizioni
			string default_pos = string.Empty;
			DocsPaVO.documento.position pos_upSx = new DocsPaVO.documento.position();
			DocsPaVO.documento.position pos_upDx = new DocsPaVO.documento.position();
			DocsPaVO.documento.position pos_downSx = new DocsPaVO.documento.position();
			DocsPaVO.documento.position pos_downDx = new DocsPaVO.documento.position();
			DocsPaVO.amministrazione.posizione pos = new DocsPaVO.amministrazione.posizione();
			for (int k = 0; k < Amm.Timbro.positions.Count; k++)
			{
				pos = (DocsPaVO.amministrazione.posizione)Amm.Timbro.positions[k];
				// posizione Alto Sinistra
				if (pos.posName == "pos_upSx")
				{
					pos_upSx.posName = pos.posName;
					pos_upSx.PosX = pos.PosX;
					pos_upSx.PosY = pos.PosY;
					file.LabelPdf.positions.Add(pos_upSx);
				}
				// posizione Alto Destra
				if (pos.posName == "pos_upDx")
				{
					pos_upDx.posName = pos.posName;
					pos_upDx.PosX = pos.PosX;
					pos_upDx.PosY = pos.PosY;
					file.LabelPdf.positions.Add(pos_upDx);
				}
				// posizione basso Sinistra
				if (pos.posName == "pos_downSx")
				{
					pos_downSx.posName = pos.posName;
					pos_downSx.PosX = pos.PosX;
					pos_downSx.PosY = pos.PosY;
					file.LabelPdf.positions.Add(pos_downSx);
				}
				// posizione basso Destra
				if (pos.posName == "pos_downDx")
				{
					pos_downDx.posName = pos.posName;
					pos_downDx.PosX = pos.PosX;
					pos_downDx.PosY = pos.PosY;
					file.LabelPdf.positions.Add(pos_downDx);
				}
				// posizione di default
				if (pos.id == Amm.Timbro_posizione)
				{
					default_pos = pos.posName;
				}
			}
			#endregion

			if ((position == null) || (position == ""))
			{
				//prendo la default su XML
				file.LabelPdf.default_position = default_pos;
			}
			else
			{
				//verifico le scelte utente
				string[] posPers = position.Split(Convert.ToChar(delimitatore));
				// è stata scelta una posizione standard
				if (posPers.Length == 1)
				{
					//forzo la scelta utente con default
					file.LabelPdf.default_position = position;
				}
				else
				{
					//prima di passare alle coordinate personalizzate verifico se la x e la y corrispondono ad
					//una delle coordinate di default...
					if (posPers[0].ToString() == pos_upSx.PosX)
					{
						if (posPers[1].ToString() == pos_upSx.PosY)
						{
							file.LabelPdf.default_position = pos_upSx.posName;
						}
						else
						{
							if (posPers[1].ToString() == pos_downSx.PosY)
							{
								file.LabelPdf.default_position = pos_downSx.posName;
							}
							else
							{
								file.LabelPdf.default_position = "pos_pers";
								DocsPaVO.documento.position pos_pers = new DocsPaVO.documento.position();
								pos_pers.posName = "pos_pers";
								pos_pers.PosX = posPers[0].ToString();
								pos_pers.PosY = posPers[1].ToString();
								file.LabelPdf.positions.Add(pos_pers);
							}
						}
					}
					else
					{
						if (posPers[0].ToString() == pos_upDx.PosX)
						{
							if (posPers[1].ToString() == pos_upDx.PosY)
							{
								file.LabelPdf.default_position = pos_upDx.posName;
							}
							else
							{
								if (posPers[1].ToString() == pos_downDx.PosY)
								{
									file.LabelPdf.default_position = pos_downDx.posName;
								}
								else
								{
									file.LabelPdf.default_position = "pos_pers";
									DocsPaVO.documento.position pos_pers = new DocsPaVO.documento.position();
									pos_pers.posName = "pos_pers";
									pos_pers.PosX = posPers[0].ToString();
									pos_pers.PosY = posPers[1].ToString();
									file.LabelPdf.positions.Add(pos_pers);
								}
							}
						}
						else
						{
							file.LabelPdf.default_position = "pos_pers";
							DocsPaVO.documento.position pos_pers = new DocsPaVO.documento.position();
							pos_pers.posName = "pos_pers";
							pos_pers.PosX = posPers[0].ToString();
							pos_pers.PosY = posPers[1].ToString();
							file.LabelPdf.positions.Add(pos_pers);
						}
					}

				}
			}

		}

		catch (Exception ex)
		{
			logger.Debug(ex, "Errore loadXmlLabelProperties");
			throw ex;
		}

	}

	public static string GetDatiEtichetta(
					DocsPaVO.utente.InfoUtente utente,
					DocsPaVO.amministrazione.InfoAmministrazione Amm,
					DocsPaVO.documento.labelPdf labelInfo,
					DocsPaVO.documento.SchedaDocumento sch,
					DocsPaVO.documento.VerifySignatureResult signatureResult,
					string version_label_allegato = ""
						)
	{
		string retValue = string.Empty;

		//separatore fra un valore e l'altro del timbro
		string separatore = " ";
		//valore di fine riga per determinare se andare a capo oppure no
		string escape = String.Empty;
		//valore che verrà restituito come output alla richiesta di dati da stampare su pdf!!!
		string Timbro = String.Empty;
		//oggetto all'interno del quale leggere tutti i dati del timbro!!!
		DocsPaVO.amministrazione.InfoAmministrazione currAmm = Amm;
		//parametri per recuperare la/le classifica
		string profile = sch.systemId;
		string people = utente.idPeople;
		string gruppo = utente.idGruppo;
		//è l'ultimo valore sostituito nella fase di creazione del timbro
		string[] lastVal = { "COD_AMM", "COD_REG", "NUM_PROTO", "DATA_COMP", "ORA", "NUM_ALLEG", "CLASSIFICA", "IN_OUT", "COD_UO_PROT", "COD_UO_VIS", "COD_RF_PROT", "COD_RF_VIS" };

		//se da amministrazione ho selezionato il timbro devo abilitare la stampa del medesimo!!!
		//leggendo l'informazione direttamente da amministrazione, solo nel caso in cui non venga
		//modificata la selezione da front-end
		//if (orientamento == String.Empty || orientamento == null)
		//{
		//    if (currAmm.Timbro_orientamento != null && currAmm.Timbro_orientamento != string.Empty && currAmm.Timbro_orientamento != "false")
		//    {
		//        isTimbro = true;
		//    }
		//}
		if (labelInfo.tipoLabel)
		{

			//orientamento del timbro non applicabile alla segnatura
			if (labelInfo.orientamento == String.Empty || labelInfo.orientamento == null)
			{
				labelInfo.orientamento = currAmm.Timbro_orientamento;
			}
			//nel caso che anche il valore letto in amministrazione sia null di default stampo il timbro in verticale
			if (labelInfo.orientamento.ToLower() == "orizzontale")
			{
				escape = "";
			}
			else
			{
				escape = "\n";
			}

			string TimbroIniziale = currAmm.Timbro_pdf;
			string datiTimbro = currAmm.Timbro_pdf;
			string sep = DocsPaDB.Utils.Personalization.getInstance(currAmm.IDAmm).getSeparator();


			//nome amministrazione
			if (datiTimbro.Contains("COD_AMM"))
			{
				//string codAmm = DocsPaDB.Utils.Personalization.getInstance(sch.registro.idAmministrazione).getCodiceAmministrazione();
				string codAmm = DocsPaDB.Utils.Personalization.getInstance(currAmm.IDAmm).getCodiceAmministrazione();
				if (codAmm != String.Empty)
				{
					datiTimbro = datiTimbro.Replace("COD_AMM", (codAmm + escape));
					lastVal[0] = codAmm + escape;
				}
				else
				{
					datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_AMM", lastVal);
					//Metodo alternativo al RemoveDesc che lascia le etichette ma gestisce la nuova linea
					//datiTimbro = datiTimbro.Remove((datiTimbro.IndexOf("COD_AMM") - 1), 1);
					//datiTimbro = datiTimbro.Replace("COD_AMM", escape);
					datiTimbro = datiTimbro.Replace("COD_AMM", "");
				}
			}

			//questo è il codice AOO ed è valorizzato solo sui documenti protocollati
			if (datiTimbro.Contains("COD_REG"))
			{

				// se si tratta di un allegato ad un protocollo lo recupero dal COD_REG del protocollo
				if ((sch.documentoPrincipale != null) && (sch.documentoPrincipale.codRegistro != null))
				{
					lastVal[1] = sch.documentoPrincipale.codRegistro + escape;
					datiTimbro = datiTimbro.Replace("COD_REG", (sch.documentoPrincipale.codRegistro + escape));
				}
				//bisogna effettuare un controllo sul fatto che sia valorizzato oppure no!
				else if (sch.registro != null)
				{
					lastVal[1] = sch.registro.codRegistro + escape;
					datiTimbro = datiTimbro.Replace("COD_REG", (sch.registro.codRegistro + escape));
				}
				else
				{
					datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_REG", lastVal);
					//Metodo alternativo al RemoveDesc che lascia le etichette ma gestisce la nuova linea
					//datiTimbro = datiTimbro.Remove((datiTimbro.IndexOf("COD_REG") - 1), 1);
					//datiTimbro = datiTimbro.Replace("COD_REG", escape);
					datiTimbro = datiTimbro.Replace("COD_REG", "");
				}
			}

			//numero protocollo
			if (datiTimbro.Contains("NUM_PROTO"))
			{
				//Normalizzo il numero di protocollo secondo lo standard a 7 cifre
				int MAX_LENGTH = 7;
				string zeroes = "";
				string numProto = "";

				// se si tratta di un allegato ad un protocollo lo recupero dal NUM_PROTO del protocollo
				if ((sch.documentoPrincipale != null) && (sch.documentoPrincipale.numProt != null))
				{
					numProto = sch.documentoPrincipale.numProt;
					for (int ind = 1; ind <= MAX_LENGTH - numProto.Length; ind++)
					{
						zeroes = zeroes + "0";
					}
					numProto = zeroes + numProto;

					datiTimbro = datiTimbro.Replace("NUM_PROTO", (numProto + escape));
					lastVal[2] = numProto + escape;

				}
				// se si tratta di un allegato ad un grigio lo recupero dal docnumber del documento principale 
				else if ((sch.documentoPrincipale != null) && (sch.documentoPrincipale.numProt == null) && sch.documentoPrincipale.docNumber != null)
				{
					numProto = sch.documentoPrincipale.docNumber;
					for (int ind = 1; ind <= MAX_LENGTH - numProto.Length; ind++)
					{
						zeroes = zeroes + "0";
					}
					numProto = zeroes + numProto;

					datiTimbro = datiTimbro.Replace("NUM_PROTO", ("ID: " + numProto + escape));
					lastVal[2] = numProto + escape;

				}
				else if (sch.protocollo != null)
				{
					numProto = sch.protocollo.numero != null ? sch.protocollo.numero : string.Empty;
					for (int ind = 1; ind <= MAX_LENGTH - numProto.Length; ind++)
					{
						zeroes = zeroes + "0";
					}
					numProto = zeroes + numProto;

					//***********
					datiTimbro = datiTimbro.Replace("NUM_PROTO", (numProto + escape));
					lastVal[2] = numProto + escape;
				}
				// Per far funzionare il timbro anche con i documenti grigi
				else if ((sch.documentoPrincipale == null) && (sch.tipoProto == "G"))
				{
					numProto = sch.docNumber;
					for (int ind = 1; ind <= MAX_LENGTH - numProto.Length; ind++)
					{
						zeroes = zeroes + "0";
					}
					numProto = zeroes + numProto;
					datiTimbro = datiTimbro.Replace("NUM_PROTO", ("ID: " + numProto + escape));
					lastVal[2] = "ID: " + numProto + escape;
				}
				else
				{
					datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "NUM_PROTO", lastVal);
					//Metodo alternativo al RemoveDesc che lascia le etichette ma gestisce la nuova linea
					//datiTimbro = datiTimbro.Remove((datiTimbro.IndexOf("NUM_PROTO") - 1), 1);
					//datiTimbro = datiTimbro.Replace("NUM_PROTO", escape);
					datiTimbro = datiTimbro.Replace("NUM_PROTO", "");
				}
			}

			//data protocollazione
			if (datiTimbro.Contains("DATA_COMP"))
			{
				// se si tratta di un allegato ad un protocollo lo recupero dal DATA_COMP del protocollo
				if (sch.documentoPrincipale != null)
				{
					//datiTimbro = datiTimbro.Replace("DATA_COMP", (sch.dataCreazione + escape));
					//protocollo.dataProtocollazione + escape));

					//PALUMBO: nel caso di documento Protocollato il metodo GetProtoData valorizza la dataApertura con la DataProtocollo
					if (sch.documentoPrincipale.dataApertura != null)
						datiTimbro = datiTimbro.Replace("DATA_COMP", (sch.documentoPrincipale.dataApertura + escape));
					else
						datiTimbro = datiTimbro.Replace("DATA_COMP", (sch.dataCreazione + escape));
					lastVal[3] = sch.dataCreazione + escape;

				}
				// Per far funzionare il timbro anche con i documenti grigi
				else if ((sch.documentoPrincipale == null) && (sch.tipoProto == "G"))
				{
					datiTimbro = datiTimbro.Replace("DATA_COMP", (sch.dataCreazione + escape));

					lastVal[3] = sch.dataCreazione + escape;
				}

				else if (sch.protocollo != null)
				{
					datiTimbro = datiTimbro.Replace("DATA_COMP", (sch.protocollo.dataProtocollazione + escape));
					lastVal[3] = sch.protocollo.dataProtocollazione + escape;
				}
				else
				{
					datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "DATA_COMP", lastVal);
					//Metodo alternativo al RemoveDesc che lascia le etichette ma gestisce la nuova linea
					//datiTimbro = datiTimbro.Remove((datiTimbro.IndexOf("DATA_COMP") - 1), 1);
					//datiTimbro = datiTimbro.Replace("DATA_COMP", escape);
					datiTimbro = datiTimbro.Replace("DATA_COMP", "");
				}
			}

			//ora di protocollazione
			if (datiTimbro.Contains("ORA"))
			{
				//aggiunta dell'ora di protocollazione nel timbro
				string ora = sch.oraCreazione;
				if ((ora != null) && (ora != ""))
				{
					//se l'ora è nel formato comprensivo dei secondi devo rimuovere i secondi prima di inserire l'ora!
					if (ora.Length > 5)
					{
						ora = ora.Remove((ora.Length - 3), 3);
					}
					datiTimbro = datiTimbro.Replace("ORA", (ora + escape));
					lastVal[4] = ora + escape;
				}
				else
				{
					datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "ORA", lastVal);
					//Metodo alternativo al RemoveDesc che lascia le etichette ma gestisce la nuova linea
					//datiTimbro = datiTimbro.Remove((datiTimbro.IndexOf("ORA") - 1), 1);
					//datiTimbro = datiTimbro.Replace("ORA", escape);
					datiTimbro = datiTimbro.Replace("ORA", "");
				}
			}

			//numero allegati
			if (datiTimbro.Contains("NUM_ALLEG"))
			{
				datiTimbro = datiTimbro.Replace("NUM_ALLEG", (sch.allegati.Count + escape));
				//datiTimbro = datiTimbro.Replace("NUM_ALLEG", "");
				lastVal[5] = System.Convert.ToString(sch.allegati.Count) + escape;
			}

			//classificazione o classificazioni del corrente documento protocollato
			if (datiTimbro.Contains("CLASSIFICA"))
			{
				string padding = string.Empty;
				ArrayList classifica = new ArrayList();
				ArrayList folders = new ArrayList();
				// Bisogna mettere il controllo su classifica perchè potrebbe non essere ancora assegnata!!!
				// controllo prima se sono su un allegato ad un protocollo
				string idprofile = sch.documentoPrincipale != null ? sch.documentoPrincipale.docNumber : profile;
				classifica = Fascicoli.FascicoloManager.getFascicoliDaDoc(utente, idprofile);

				string key_beprojectlevel = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_PROJECT_LEVEL");
				if (!string.IsNullOrEmpty(key_beprojectlevel) && key_beprojectlevel.Equals("1"))
					folders = Fascicoli.FolderManager.GetFoldersDocument(sch.docNumber);

				if (classifica != null)
				{
					for (int i = 0; i < classifica.Count; i++)
					{
						DocsPaVO.fascicolazione.Fascicolo fascicolo = (DocsPaVO.fascicolazione.Fascicolo)classifica[i];
						if (fascicolo.codice != String.Empty)
						{
							string temp = string.Empty;
							var folder = (from DocsPaVO.fascicolazione.Folder f in folders
										  where f.idFascicolo == fascicolo.systemID
										  select f).FirstOrDefault();

							if (folder != null)
							{
								for (int j = 1; j < folder.codicelivello.Length / 4; j++)
								{
									string val = folder.codicelivello.Substring(j * 4, 4);
									temp += string.Format(".{0}", Convert.ToInt32(val));
								}

								// Questo approccio genera un errore ma al momento 
								// non mi viene in mente niente di più semplice
								Timbro += padding + separatore + (i == 0 ? "[" : "; ") +
									GetCodiceFascicolo(currAmm.Fascicolatura, fascicolo.codice, temp) + escape;
							}
							else if (!string.IsNullOrEmpty(key_beprojectlevel) && key_beprojectlevel.Equals("1"))
							{
								Timbro += padding + separatore + (i == 0 ? "[" : "; ") + fascicolo.codice + escape;
								if (escape != "")
									padding = ReturnDesc(TimbroIniziale, datiTimbro, "CLASSIFICA", lastVal);
							}
							else
							{
								Timbro = Timbro + padding + separatore + (i == 0 ? "[" : "; ") + fascicolo.codice + escape;
								//Nel caso di orientamento verticale replico il separatore letto in amministrazione
								//altrimenti uso il seguente separatore
								padding = " -";
								if (escape != "")
									padding = ReturnDesc(TimbroIniziale, datiTimbro, "CLASSIFICA", lastVal);
							}
						}
					}
					if (Timbro == string.Empty)
					{
						datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "CLASSIFICA", lastVal);
						//Metodo alternativo al RemoveDesc che lascia le etichette ma gestisce la nuova linea
						//datiTimbro = datiTimbro.Remove((datiTimbro.IndexOf("CLASSIFICA") - 1), 1);
						//datiTimbro = datiTimbro.Replace("CLASSIFICA", escape);
						datiTimbro = datiTimbro.Replace("CLASSIFICA", "");
					}
					else
					{
						Timbro += "]";
						datiTimbro = datiTimbro.Replace("CLASSIFICA", Timbro);
						lastVal[6] = Timbro;
					}
				}
			}

			//Tipo di protocollo
			if (datiTimbro.Contains("IN_OUT"))
			{
				string arrPart = "";
				if (sch.protocollatore != null)
				{
					if (sch.protocollo.GetType() == typeof(DocsPaVO.documento.ProtocolloEntrata))
					{
						arrPart = "A";
					}
					else if (sch.protocollo.GetType() == typeof(DocsPaVO.documento.ProtocolloUscita))
					{
						arrPart = "P";
					}
					else if (sch.protocollo.GetType() == typeof(DocsPaVO.documento.ProtocolloInterno))
					{
						arrPart = "I";
					}
				}
				if (arrPart == string.Empty)
				{
					datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "IN_OUT", lastVal);
					//Metodo alternativo al RemoveDesc che lascia le etichette ma gestisce la nuova linea
					//datiTimbro = datiTimbro.Remove((datiTimbro.IndexOf("IN_OUT") - 1), 1);
					//datiTimbro = datiTimbro.Replace("IN_OUT", escape);
					datiTimbro = datiTimbro.Replace("IN_OUT", "");
				}
				else
				{
					datiTimbro = datiTimbro.Replace("IN_OUT", arrPart + escape);
					lastVal[7] = arrPart + escape;
				}
			}


			// GESTIONE UNITA' ORGANIZZATIVE ED RF ***********************************************
			if (datiTimbro.Contains("COD_UO_PROT"))
			{
				if (sch.protocollatore != null)
				{
					//string uo = getCodiceUO(sch.creatoreDocumento.idCorrGlob_Ruolo);

					//string uo = getCodiceUO(sch.protocollatore.ruolo_idCorrGlobali);
					//ABBATANGELI - Necessario per creare il timbro correttamente anche se il ruolo è stato storicizzato
					string uo = getCodiceUOEnabledAndDisabled(sch.protocollatore.ruolo_idCorrGlobali);
					//uo = sch.protocollatore.uo_codiceCorrGlobali;
					if (!string.IsNullOrEmpty(uo))
					{
						datiTimbro = datiTimbro.Replace("COD_UO_PROT", (uo + escape));
						lastVal[8] = uo + escape;
					}
					else
					{
						datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_UO_PROT", lastVal);
						datiTimbro = datiTimbro.Replace("COD_UO_PROT", "");
					}
				}
				else
				{
					datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_UO_PROT", lastVal);
					datiTimbro = datiTimbro.Replace("COD_UO_PROT", "");
				}
			}

			if (datiTimbro.Contains("COD_UO_VIS"))
			{
				if (utente.idCorrGlobali != string.Empty)
				{
					string uo = getCodiceUO(utente.idCorrGlobali);
					if (!string.IsNullOrEmpty(uo))
					{
						datiTimbro = datiTimbro.Replace("COD_UO_VIS", (uo + escape));
						lastVal[9] = uo + escape;
					}
					else
					{
						datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_UO_VIS", lastVal);
						datiTimbro = datiTimbro.Replace("COD_UO_VIS", "");
					}
				}
				else
				{
					datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_UO_VIS", lastVal);
					datiTimbro = datiTimbro.Replace("COD_UO_VIS", "");
				}
			}

			if (datiTimbro.Contains("COD_RF_PROT"))
			{
				if (sch.protocollatore != null)
				{
					string rf = getCodiceRF(sch.protocollatore.ruolo_idCorrGlobali);
					if (!string.IsNullOrEmpty(rf))
					{
						datiTimbro = datiTimbro.Replace("COD_RF_PROT", (rf + escape));
						lastVal[10] = rf + escape;
					}
					else
					{
						datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_RF_PROT", lastVal);
						datiTimbro = datiTimbro.Replace("COD_RF_PROT", "");
					}
				}
				else
				{
					datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_RF_PROT", lastVal);
					datiTimbro = datiTimbro.Replace("COD_RF_PROT", "");
				}
			}

			if (datiTimbro.Contains("COD_RF_VIS"))
			{
				if (utente.idCorrGlobali != string.Empty)
				{
					string rf = getCodiceRF(utente.idCorrGlobali);
					if (!string.IsNullOrEmpty(rf))
					{
						datiTimbro = datiTimbro.Replace("COD_RF_VIS", (rf + escape));
						lastVal[11] = rf + escape;
					}
					else
					{
						datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_RF_VIS", lastVal);
						datiTimbro = datiTimbro.Replace("COD_RF_VIS", "");
					}
				}
				else
				{
					datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_RF_VIS", lastVal);
					datiTimbro = datiTimbro.Replace("COD_RF_VIS", "");
				}
			}
			// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


			Timbro = datiTimbro;

			//nel caso di documento annullato stampo il timbro di annullamento come segue
			if (sch.protocollo != null && sch.protocollo.protocolloAnnullato != null)
				Timbro = separatore + sch.protocollo.segnatura + "\n" + separatore + "Annullato il: " + sch.protocollo.protocolloAnnullato.dataAnnullamento + "\n" + separatore + "Motivo: " + sch.protocollo.protocolloAnnullato.autorizzazione;

			retValue = string.IsNullOrEmpty(version_label_allegato) ? Timbro : Timbro + " - " + version_label_allegato;
			//retValue = Timbro;
		}
		else
		{

			//nel caso di documento annullato stampo il timbro di annullamento come segue
			if (sch.protocollo != null && sch.protocollo.protocolloAnnullato != null)
			{
				Timbro = sch.protocollo.segnatura + "\n" + "Annullato il: " + sch.protocollo.protocolloAnnullato.dataAnnullamento + "\n" + "Motivo: " + sch.protocollo.protocolloAnnullato.autorizzazione;
				retValue = Timbro;
			}
			else
			{
				//protocolli
				if (sch != null && (sch.protocollo != null
				 && sch.protocollo.segnatura != null
				 && sch.protocollo.segnatura != ""))
				{
					//segnatura in alternativa al timbro ed all'annullamento!!!
					retValue = string.IsNullOrEmpty(version_label_allegato) ? sch.protocollo.segnatura : sch.protocollo.segnatura + " - " + version_label_allegato;

					//se siamo in CC aggiungo alla segnatura il protocollo arma(per carabinieri) in stampa A4 segnatura
					string protoArma = string.Empty;
					string livTitolario = string.Empty;
					if (!string.IsNullOrEmpty(DocsPaVO.Settings.AppSettings.Instance.ENABLE_PROTOCOLLO_TIT))
					{
						protoArma = DocsPaVO.Settings.AppSettings.Instance.ENABLE_PROTOCOLLO_TIT;
						livTitolario = DocsPaVO.Settings.AppSettings.Instance.ENABLE_LIVELLI_TITOLARIO;
						ArrayList fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoliDaDoc(utente, profile);

						string classifica = string.Empty;
						string sep = ":";
						string space = " ";
						//classificazione
						for (int i = 0; i < fascicolo.Count; i++)
						{
							DocsPaVO.fascicolazione.Fascicolo fasc = (DocsPaVO.fascicolazione.Fascicolo)fascicolo[i];
							if (fasc.codice != String.Empty)
							{
								DocsPaVO.fascicolazione.Classifica[] classif = BusinessLogic.Fascicoli.TitolarioManager.getGerarchia(fasc.idClassificazione, Amm.IDAmm);
								classifica = classif[classif.Length - 1].codice;
							}
						}

						retValue += "\n" + livTitolario + sep + classifica + space + protoArma + sep + sch.protocolloTitolario;
					}
					//end check carabinieri
				}
				//predisposti o grigi
				else if (sch != null && (sch.protocollo != null
				 && string.IsNullOrEmpty(sch.protocollo.segnatura)
				 && !string.IsNullOrEmpty(sch.systemId))

					|| (sch.protocollo == null
						&& !string.IsNullOrEmpty(sch.systemId)))

					if (sch.documentoPrincipale != null)
					{
						if (sch.documentoPrincipale.segnatura != null)
							retValue = string.IsNullOrEmpty(version_label_allegato) ? sch.documentoPrincipale.segnatura : sch.documentoPrincipale.segnatura + " - " + version_label_allegato;
						//retValue = sch.documentoPrincipale.segnatura;
						else
							retValue = string.IsNullOrEmpty(version_label_allegato) ? sch.documentoPrincipale.docNumber + "  " + sch.dataCreazione.ToString() : sch.documentoPrincipale.docNumber + "  " + sch.dataCreazione.ToString() + " - " + version_label_allegato;
						//retValue = sch.documentoPrincipale.docNumber + "  " + sch.dataCreazione.ToString();
					}

					else
						retValue = string.IsNullOrEmpty(version_label_allegato) ? sch.docNumber + "  " + sch.dataCreazione.ToString() : sch.docNumber + "  " + sch.dataCreazione.ToString() + " - " + version_label_allegato;
				//retValue = sch.docNumber + "  " + sch.dataCreazione.ToString();

				//&& supportedType
				//&& Session["allegato"] == null
				//)
				///

			}
		}

		return retValue;
	}

	/// <summary>
	/// Codice ROBUSTO per Leggere porzioni di XML anche corrotte o non well formed.
	/// </summary>
	/// <param name="element"></param>
	/// <param name="metadata"></param>
	/// <returns></returns>
	private static string StrongXmlReadElementText(string element, byte[] metadata)
	{
		string xmlstring = System.Text.ASCIIEncoding.ASCII.GetString(metadata);
		int conf = xmlstring.IndexOf("<" + element);
		if (conf == -1)
			return null;
		using (MemoryStream xmlfile = new MemoryStream(metadata))
		{
			xmlfile.Position = conf;
			using (XmlTextReader xt = new XmlTextReader(xmlfile))
			{
				xt.Namespaces = false;
				try
				{
					//retry
					int count = 0;
					while (true)
					{
						count++;
						xt.Read();
						if (xt.NodeType == XmlNodeType.Text)
							return xt.Value;

						if (count > 100)
							return null;
					}
				}
				catch { return null; }
			}
		}
	}

	/// <summary>
	/// Questo metodo rimuove i separatori o la descrizione dei campi non valorizzati del timbro
	/// </summary>
	/// <returns></returns>
	private static string RemoveDesc(string timbro_iniziale, string currTimbro, string currVal, string[] dati)
	{
		int count = 0;
		int start = 0;
		int inizio = 0;
		string specialChar = "#%*@";
		//Uso un ciclo while nel caso ci sia per errore più di un'occorrenza del codice da rimuovere!
		while (currTimbro.Contains(currVal))
		{
			//devo ricalcolare l'indice dei codici precedenti
			int[] ordine = codicePrec(timbro_iniziale);

			if (currVal.Equals("COD_AMM"))
			{
				if (ordine[0] >= 0)
				{
					start = currTimbro.IndexOf(dati[ordine[0]]) + dati[ordine[0]].Length;
					count = currTimbro.IndexOf("COD_AMM") - start;
					currTimbro = currTimbro.Remove(start, count);
				}
				else
				{
					count = currTimbro.IndexOf("COD_AMM");
					currTimbro = currTimbro.Remove(0, count);
				}
				inizio = currTimbro.IndexOf(currVal);
				currTimbro = currTimbro.Remove(inizio, currVal.Length);
				currTimbro = currTimbro.Insert(inizio, specialChar);
				timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
			}
			if (currVal.Equals("COD_REG"))
			{
				if (ordine[1] >= 0)
				{
					start = currTimbro.IndexOf(dati[ordine[1]]) + dati[ordine[1]].Length;
					count = currTimbro.IndexOf("COD_REG") - start;
					currTimbro = currTimbro.Remove(start, count);
				}
				else
				{
					count = currTimbro.IndexOf("COD_REG");
					currTimbro = currTimbro.Remove(0, count);
				}
				inizio = currTimbro.IndexOf(currVal);
				currTimbro = currTimbro.Remove(inizio, currVal.Length);
				currTimbro = currTimbro.Insert(inizio, specialChar);
				timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
			}
			if (currVal.Equals("NUM_PROTO"))
			{
				if (ordine[2] >= 0)
				{
					start = currTimbro.IndexOf(dati[ordine[2]]) + dati[ordine[2]].Length;
					count = currTimbro.IndexOf("NUM_PROTO") - start;
					currTimbro = currTimbro.Remove(start, count);
				}
				else
				{
					count = currTimbro.IndexOf("NUM_PROTO");
					currTimbro = currTimbro.Remove(0, count);
				}
				inizio = currTimbro.IndexOf(currVal);
				currTimbro = currTimbro.Remove(inizio, currVal.Length);
				currTimbro = currTimbro.Insert(inizio, specialChar);
				timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
			}
			if (currVal.Equals("DATA_COMP"))
			{
				if (ordine[3] >= 0)
				{
					start = currTimbro.IndexOf(dati[ordine[3]]) + dati[ordine[3]].Length;
					count = currTimbro.IndexOf("DATA_COMP") - start;
					currTimbro = currTimbro.Remove(start, count);
				}
				else
				{
					count = currTimbro.IndexOf("DATA_COMP");
					currTimbro = currTimbro.Remove(0, count);
				}
				inizio = currTimbro.IndexOf(currVal);
				currTimbro = currTimbro.Remove(inizio, currVal.Length);
				currTimbro = currTimbro.Insert(inizio, specialChar);
				timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
			}
			if (currVal.Equals("ORA"))
			{
				if (ordine[4] >= 0)
				{
					start = currTimbro.IndexOf(dati[ordine[4]]) + dati[ordine[4]].Length;
					count = currTimbro.IndexOf("ORA") - start;
					currTimbro = currTimbro.Remove(start, count);
				}
				else
				{
					count = currTimbro.IndexOf("ORA");
					currTimbro = currTimbro.Remove(0, count);
				}
				inizio = currTimbro.IndexOf(currVal);
				currTimbro = currTimbro.Remove(inizio, currVal.Length);
				currTimbro = currTimbro.Insert(inizio, specialChar);
				timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
			}
			if (currVal.Equals("NUM_ALLEG"))
			{
				if (ordine[5] >= 0)
				{
					start = currTimbro.IndexOf(dati[ordine[5]]) + dati[ordine[5]].Length;
					count = currTimbro.IndexOf("NUM_ALLEG") - start;
					currTimbro = currTimbro.Remove(start, count);
				}
				else
				{
					count = currTimbro.IndexOf("NUM_ALLEG");
					currTimbro = currTimbro.Remove(0, count);
				}
				inizio = currTimbro.IndexOf(currVal);
				currTimbro = currTimbro.Remove(inizio, currVal.Length);
				currTimbro = currTimbro.Insert(inizio, specialChar);
				timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
			}
			if (currVal.Equals("CLASSIFICA"))
			{
				if (ordine[6] >= 0)
				{
					start = currTimbro.IndexOf(dati[ordine[6]]) + dati[ordine[6]].Length;
					count = currTimbro.IndexOf("CLASSIFICA") - start;
					currTimbro = currTimbro.Remove(start, count);
				}
				else
				{
					count = currTimbro.IndexOf("CLASSIFICA");
					currTimbro = currTimbro.Remove(0, count);
				}
				inizio = currTimbro.IndexOf(currVal);
				currTimbro = currTimbro.Remove(inizio, currVal.Length);
				currTimbro = currTimbro.Insert(inizio, specialChar);
				timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
			}
			if (currVal.Equals("IN_OUT"))
			{
				if (ordine[7] >= 0)
				{
					start = currTimbro.IndexOf(dati[ordine[7]]) + dati[ordine[7]].Length;
					count = currTimbro.IndexOf("IN_OUT") - start;
					currTimbro = currTimbro.Remove(start, count);
				}
				else
				{
					count = currTimbro.IndexOf("IN_OUT");
					currTimbro = currTimbro.Remove(0, count);
				}
				inizio = currTimbro.IndexOf(currVal);
				currTimbro = currTimbro.Remove(inizio, currVal.Length);
				currTimbro = currTimbro.Insert(inizio, specialChar);
				timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
			}

			// GESTIONE UNITA' ORGANIZZATIVE ED RF ***********************************************
			if (currVal.Equals("COD_UO_PROT"))
			{
				if (ordine[8] >= 0)
				{
					start = currTimbro.IndexOf(dati[ordine[8]]) + dati[ordine[8]].Length;
					count = currTimbro.IndexOf("COD_UO_PROT") - start;
					currTimbro = currTimbro.Remove(start, count);
				}
				else
				{
					count = currTimbro.IndexOf("COD_UO_PROT");
					currTimbro = currTimbro.Remove(0, count);
				}
				inizio = currTimbro.IndexOf(currVal);
				currTimbro = currTimbro.Remove(inizio, currVal.Length);
				currTimbro = currTimbro.Insert(inizio, specialChar);
				timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
			}
			if (currVal.Equals("COD_UO_VIS"))
			{
				if (ordine[9] >= 0)
				{
					start = currTimbro.IndexOf(dati[ordine[9]]) + dati[ordine[9]].Length;
					count = currTimbro.IndexOf("COD_UO_VIS") - start;
					currTimbro = currTimbro.Remove(start, count);
				}
				else
				{
					count = currTimbro.IndexOf("COD_UO_VIS");
					currTimbro = currTimbro.Remove(0, count);
				}
				inizio = currTimbro.IndexOf(currVal);
				currTimbro = currTimbro.Remove(inizio, currVal.Length);
				currTimbro = currTimbro.Insert(inizio, specialChar);
				timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
			}
			if (currVal.Equals("COD_RF_PROT"))
			{
				if (ordine[10] >= 0)
				{
					start = GetStartIndex(dati, ordine, currTimbro, 10);
					count = currTimbro.IndexOf("COD_RF_PROT") - start;
					currTimbro = currTimbro.Remove(start, count);
				}
				else
				{
					count = currTimbro.IndexOf("COD_RF_PROT");
					currTimbro = currTimbro.Remove(0, count);
				}
				inizio = currTimbro.IndexOf(currVal);
				currTimbro = currTimbro.Remove(inizio, currVal.Length);
				currTimbro = currTimbro.Insert(inizio, specialChar);
				timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
			}
			if (currVal.Equals("COD_RF_VIS"))
			{
				if (ordine[11] >= 0)
				{
					start = currTimbro.IndexOf(dati[ordine[11]]) + dati[ordine[11]].Length;
					count = currTimbro.IndexOf("COD_RF_VIS") - start;
					currTimbro = currTimbro.Remove(start, count);
				}
				else
				{
					count = currTimbro.IndexOf("COD_RF_VIS");
					currTimbro = currTimbro.Remove(0, count);
				}
				inizio = currTimbro.IndexOf(currVal);
				currTimbro = currTimbro.Remove(inizio, currVal.Length);
				currTimbro = currTimbro.Insert(inizio, specialChar);
				timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
			}
			// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		}
		currTimbro = currTimbro.Replace(specialChar, currVal);
		return currTimbro;
	}

	private static string GetCodiceFascicolo(string fascicolatura, string codice, string temp)
	{
		int startfasc = fascicolatura.IndexOf("NUM_PROG");
		startfasc = (startfasc == 0 ? 0 : startfasc - 1);
		char cstartfasc = startfasc == 0 ? char.MinValue : fascicolatura[startfasc];

		int index = 0, rep = 0;
		while (index <= startfasc)
		{
			if (fascicolatura[index] == fascicolatura[startfasc])
				rep++;

			index++;
		}

		int endfasc = fascicolatura.IndexOf("NUM_PROG") + "NUM_PROG".Length;
		endfasc = endfasc >= fascicolatura.Length ? fascicolatura.Length - 1 : endfasc;
		char cendfasc = endfasc == fascicolatura.Length - 1 ? char.MinValue : fascicolatura[endfasc];

		int startcod = 0;
		if (cstartfasc != char.MinValue)
		{
			if (cendfasc == char.MinValue)
				startcod = codice.LastIndexOf(cstartfasc);
			else
			{
				for (int i = 0; i < rep; i++)
				{
					startcod = codice.IndexOf(cstartfasc, startcod + 1);
				}
			}
		}

		int endcod = cendfasc == char.MinValue ? codice.Length : codice.IndexOf(cendfasc, startcod + 1);
		return codice.Insert(endcod, temp);
	}

	/// <summary>
	/// Questo metodo restituisce i separatori o la descrizione dei campi richiesti
	/// </summary>
	/// <returns></returns>
	private static string ReturnDesc(string timbro_iniziale, string currTimbro, string currVal, string[] dati)
	{
		int[] ordine = new int[12];
		int i = -1;
		while (timbro_iniziale != string.Empty)
		{
			string appo = timbro_iniziale;
			if (timbro_iniziale.StartsWith("COD_AMM"))
			{
				ordine[0] = i;
				timbro_iniziale = timbro_iniziale.Replace("COD_AMM", "");
				i = 0;
			}
			if (timbro_iniziale.StartsWith("COD_REG"))
			{
				ordine[1] = i;
				timbro_iniziale = timbro_iniziale.Replace("COD_REG", "");
				i = 1;
			}
			if (timbro_iniziale.StartsWith("NUM_PROTO"))
			{
				ordine[2] = i;
				timbro_iniziale = timbro_iniziale.Replace("NUM_PROTO", "");
				i = 2;
			}
			if (timbro_iniziale.StartsWith("DATA_COMP"))
			{
				ordine[3] = i;
				timbro_iniziale = timbro_iniziale.Replace("DATA_COMP", "");
				i = 3;
			}
			if (timbro_iniziale.StartsWith("ORA"))
			{
				ordine[4] = i;
				timbro_iniziale = timbro_iniziale.Replace("ORA", "");
				i = 4;
			}
			if (timbro_iniziale.StartsWith("NUM_ALLEG"))
			{
				ordine[5] = i;
				timbro_iniziale = timbro_iniziale.Replace("NUM_ALLEG", "");
				i = 5;
			}
			if (timbro_iniziale.StartsWith("CLASSIFICA"))
			{
				ordine[6] = i;
				timbro_iniziale = timbro_iniziale.Replace("CLASSIFICA", "");
				i = 6;
			}
			if (timbro_iniziale.StartsWith("IN_OUT"))
			{
				ordine[7] = i;
				timbro_iniziale = timbro_iniziale.Replace("IN_OUT", "");
				i = 7;
			}

			// GESTIONE UNITA' ORGANIZZATIVE ED RF ***********************************************
			if (timbro_iniziale.StartsWith("COD_UO_PROT"))
			{
				ordine[8] = i;
				timbro_iniziale = timbro_iniziale.Replace("COD_UO_PROT", "");
				i = 8;
			}
			if (timbro_iniziale.StartsWith("COD_UO_VIS"))
			{
				ordine[9] = i;
				timbro_iniziale = timbro_iniziale.Replace("COD_UO_VIS", "");
				i = 9;
			}
			if (timbro_iniziale.StartsWith("COD_RF_PROT"))
			{
				ordine[10] = i;
				timbro_iniziale = timbro_iniziale.Replace("COD_RF_PROT", "");
				i = 10;
			}
			if (timbro_iniziale.StartsWith("COD_RF_VIS"))
			{
				ordine[11] = i;
				timbro_iniziale = timbro_iniziale.Replace("COD_RF_VIS", "");
				i = 11;
			}
			// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

			//se il timbro iniziale è rimasto invariato rimuovo un carattere ed inizio di nuovo la ricerca
			if (timbro_iniziale == appo)
			{
				timbro_iniziale = timbro_iniziale.Remove(0, 1);
			}
		}

		int count = 0;
		int start = 0;
		if (currVal.Equals("COD_AMM"))
		{
			if (ordine[0] >= 0)
			{
				start = currTimbro.IndexOf(dati[ordine[0]]) + dati[ordine[0]].Length;
				count = currTimbro.IndexOf("COD_AMM") - start;
				currTimbro = currTimbro.Substring(start, count);
			}
			else
			{
				count = currTimbro.IndexOf("COD_AMM");
				currTimbro = currTimbro.Substring(0, count);
			}
		}
		if (currVal.Equals("COD_REG"))
		{
			if (ordine[1] >= 0)
			{
				start = currTimbro.IndexOf(dati[ordine[1]]) + dati[ordine[1]].Length;
				count = currTimbro.IndexOf("COD_REG") - start;
				currTimbro = currTimbro.Substring(start, count);
			}
			else
			{
				count = currTimbro.IndexOf("COD_REG");
				currTimbro = currTimbro.Substring(0, count);
			}
		}
		if (currVal.Equals("NUM_PROTO"))
		{
			if (ordine[2] >= 0)
			{
				start = currTimbro.IndexOf(dati[ordine[2]]) + dati[ordine[2]].Length;
				count = currTimbro.IndexOf("NUM_PROTO") - start;
				currTimbro = currTimbro.Substring(start, count);
			}
			else
			{
				count = currTimbro.IndexOf("NUM_PROTO");
				currTimbro = currTimbro.Substring(0, count);
			}
		}
		if (currVal.Equals("DATA_COMP"))
		{
			if (ordine[3] >= 0)
			{
				start = currTimbro.IndexOf(dati[ordine[3]]) + dati[ordine[3]].Length;
				count = currTimbro.IndexOf("DATA_COMP") - start;
				currTimbro = currTimbro.Substring(start, count);
			}
			else
			{
				count = currTimbro.IndexOf("DATA_COMP");
				currTimbro = currTimbro.Substring(0, count);
			}
		}
		if (currVal.Equals("ORA"))
		{
			if (ordine[4] >= 0)
			{
				start = currTimbro.IndexOf(dati[ordine[4]]) + dati[ordine[4]].Length;
				count = currTimbro.IndexOf("ORA") - start;
				currTimbro = currTimbro.Substring(start, count);
			}
			else
			{
				count = currTimbro.IndexOf("ORA");
				currTimbro = currTimbro.Substring(0, count);
			}
		}
		if (currVal.Equals("NUM_ALLEG"))
		{
			if (ordine[5] >= 0)
			{
				start = currTimbro.IndexOf(dati[ordine[5]]) + dati[ordine[5]].Length;
				count = currTimbro.IndexOf("NUM_ALLEG") - start;
				currTimbro = currTimbro.Substring(start, count);
			}
			else
			{
				count = currTimbro.IndexOf("NUM_ALLEG");
				currTimbro = currTimbro.Substring(0, count);
			}
		}
		if (currVal.Equals("CLASSIFICA"))
		{
			if (ordine[6] >= 0)
			{
				start = currTimbro.IndexOf(dati[ordine[6]]) + dati[ordine[6]].Length;
				count = currTimbro.IndexOf("CLASSIFICA") - start;
				currTimbro = currTimbro.Substring(start, count);
			}
			else
			{
				count = currTimbro.IndexOf("CLASSIFICA");
				currTimbro = currTimbro.Substring(0, count);
			}
		}
		if (currVal.Equals("IN_OUT"))
		{
			if (ordine[7] >= 0)
			{
				start = currTimbro.IndexOf(dati[ordine[7]]) + dati[ordine[7]].Length;
				count = currTimbro.IndexOf("IN_OUT") - start;
				currTimbro = currTimbro.Substring(start, count);
			}
			else
			{
				count = currTimbro.IndexOf("IN_OUT");
				currTimbro = currTimbro.Substring(0, count);
			}
		}
		return currTimbro;
	}

	/// <summary>
	/// Restituisce l'Unità Organizzativa relativa al'id in CorrGlob di un dato Ruolo
	/// </summary>
	/// <param name="idCorrGlob_Ruolo"></param>
	/// <returns></returns>
	private static string getCodiceUOEnabledAndDisabled(string idCorrGlob_Ruolo)
	{
		string uo = string.Empty;
		//NB: l'oggetto registri contenuto dentro l'oggetto ruolo NON prevede gli RF ma SOLO i registri
		//    associati a tale ruolo!!!
		DocsPaVO.utente.Ruolo ruolo = Utenti.UserManager.getRuoloEnabledAndDisabled(idCorrGlob_Ruolo);
		if (ruolo != null)
		{
			uo = ruolo.uo.codice;
		}
		return uo;
	}

	/// <summary>
	/// Restituisce l'Unità Organizzativa relativa al'id in CorrGlob di un dato Ruolo
	/// </summary>
	/// <param name="idCorrGlob_Ruolo"></param>
	/// <returns></returns>
	private static string getCodiceUO(string idCorrGlob_Ruolo)
	{
		string uo = string.Empty;
		//NB: l'oggetto registri contenuto dentro l'oggetto ruolo NON prevede gli RF ma SOLO i registri
		//    associati a tale ruolo!!!
		DocsPaVO.utente.Ruolo ruolo = Utenti.UserManager.getRuolo(idCorrGlob_Ruolo);
		if (ruolo != null)
		{
			uo = ruolo.uo.codice;
		}
		return uo;
	}

	/// <summary>
	/// Restituisce RF, se esiste, relativo al'id in CorrGlob di un dato Ruolo
	/// </summary>
	/// <param name="idCorrGlob_Ruolo"></param>
	/// <returns></returns>
	private static string getCodiceRF(string idCorrGlob_Ruolo)
	{
		string RF = string.Empty;
		//NB: idCorrGlob_Ruolo in questo caso è equivalente a ruolo.systemId!!!
		//Se invece che "1" passo "null" ottengo anche i registri oltre gli RF.
		ArrayList listaRF = Utenti.RegistriManager.getListaRegistriRfRuolo(idCorrGlob_Ruolo, "1", null);
		if (listaRF.Count > 0)
		{
			DocsPaVO.utente.Registro reg = (DocsPaVO.utente.Registro)listaRF[0];
			if (reg != null)
			{
				if (reg.chaRF == "1")
				{
					RF = reg.codRegistro;
				}
			}
		}
		return RF;
	}

	/// <summary>
	/// Restituisce il vettore degli indici dei codici precedenti
	/// </summary>
	/// <param name="timbro_iniziale"></param>
	/// <returns></returns>
	private static int[] codicePrec(string timbro_iniziale)
	{
		int[] ordine = new int[12];
		int i = -1;
		while (timbro_iniziale != string.Empty)
		{
			string appo = timbro_iniziale;
			if (timbro_iniziale.StartsWith("COD_AMM"))
			{
				ordine[0] = i;
				timbro_iniziale = timbro_iniziale.Replace("COD_AMM", "");
				i = 0;
			}
			if (timbro_iniziale.StartsWith("COD_REG"))
			{
				ordine[1] = i;
				timbro_iniziale = timbro_iniziale.Replace("COD_REG", "");
				i = 1;
			}
			if (timbro_iniziale.StartsWith("NUM_PROTO"))
			{
				ordine[2] = i;
				timbro_iniziale = timbro_iniziale.Replace("NUM_PROTO", "");
				i = 2;
			}
			if (timbro_iniziale.StartsWith("DATA_COMP"))
			{
				ordine[3] = i;
				timbro_iniziale = timbro_iniziale.Replace("DATA_COMP", "");
				i = 3;
			}
			if (timbro_iniziale.StartsWith("ORA"))
			{
				ordine[4] = i;
				timbro_iniziale = timbro_iniziale.Replace("ORA", "");
				i = 4;
			}
			if (timbro_iniziale.StartsWith("NUM_ALLEG"))
			{
				ordine[5] = i;
				timbro_iniziale = timbro_iniziale.Replace("NUM_ALLEG", "");
				i = 5;
			}
			if (timbro_iniziale.StartsWith("CLASSIFICA"))
			{
				ordine[6] = i;
				timbro_iniziale = timbro_iniziale.Replace("CLASSIFICA", "");
				i = 6;
			}
			if (timbro_iniziale.StartsWith("IN_OUT"))
			{
				ordine[7] = i;
				timbro_iniziale = timbro_iniziale.Replace("IN_OUT", "");
				i = 7;
			}

			// GESTIONE UNITA' ORGANIZZATIVE ED RF ***********************************************
			if (timbro_iniziale.StartsWith("COD_UO_PROT"))
			{
				ordine[8] = i;
				timbro_iniziale = timbro_iniziale.Replace("COD_UO_PROT", "");
				i = 8;
			}
			if (timbro_iniziale.StartsWith("COD_UO_VIS"))
			{
				ordine[9] = i;
				timbro_iniziale = timbro_iniziale.Replace("COD_UO_VIS", "");
				i = 9;
			}
			if (timbro_iniziale.StartsWith("COD_RF_PROT"))
			{
				ordine[10] = i;
				timbro_iniziale = timbro_iniziale.Replace("COD_RF_PROT", "");
				i = 10;
			}
			if (timbro_iniziale.StartsWith("COD_RF_VIS"))
			{
				ordine[11] = i;
				timbro_iniziale = timbro_iniziale.Replace("COD_RF_VIS", "");
				i = 11;
			}
			// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

			//se il timbro iniziale è rimasto invariato rimuovo un carattere ed inizio di nuovo la ricerca
			if (timbro_iniziale == appo)
			{
				timbro_iniziale = timbro_iniziale.Remove(0, 1);
			}
		}
		return ordine;
	}

	private static int GetStartIndex(string[] dati, int[] ordine, string currTimbro, int index)
	{
		int start = 0;
		if (ordine[index] >= 0)
		{
			if (currTimbro.IndexOf(dati[ordine[index]]) < 0)
			{
				start = GetStartIndex(dati, ordine, currTimbro, ordine[index]);
			}
			else
			{
				start = currTimbro.IndexOf(dati[ordine[index]]) + dati[ordine[index]].Length;
			}
		}
		return start;
	}

	public static DocsPaVO.documento.FileDocumento getInfoFile(
		DocsPaVO.documento.FileRequest objFileRequest,
		DocsPaVO.utente.InfoUtente objSicurezza
		)
	{
		DocsPaVO.documento.FileDocumento fileDoc = new DocsPaVO.documento.FileDocumento();
		fileDoc.path = objFileRequest.docServerLoc + objFileRequest.path;
		fileDoc.name = objFileRequest.fileName;
		int indice;
		//

		if (DocsPaVO.Settings.AppSettings.Instance.documentale.ToUpper() == "FILENET")
		{
			DocsPaDocumentale.Documentale.DocumentManager FNdoc = new DocsPaDocumentale.Documentale.DocumentManager(objSicurezza);
			fileDoc.name = FNdoc.GetOriginalFileName(objFileRequest.docNumber, objFileRequest.versionId);
		}
		fileDoc.nomeOriginale = getOriginalFileName(objSicurezza, objFileRequest);
		indice = fileDoc.name.LastIndexOf(@"\");
		if (indice < (fileDoc.name.Length - 1))
		{
			fileDoc.name = fileDoc.name.Substring(indice + 1);
		}

		//modifica
		if (string.IsNullOrEmpty(fileDoc.path))
			fileDoc.fullName = '\u005C'.ToString() + fileDoc.name;
		else
			//fine modifica
			//fileDoc.fullName = fileDoc.path + '\u005C'.ToString() + fileDoc.name;
			fileDoc.fullName = objFileRequest.fileName;
		fileDoc.contentType = getContentType(fileDoc.name);
		fileDoc.estensioneFile = getEstensioneIntoSignedFile(fileDoc.fullName);
		logger.Debug("Full name: " + fileDoc.fullName);
		logger.Debug("idAmministrazione: " + objSicurezza.idAmministrazione);
		logger.Debug("Library: " + DocsPaDB.Utils.Personalization.getInstance(objSicurezza.idAmministrazione).getLibrary());

		string docNumber = objFileRequest.docNumber;
		// string versionId = objFileRequest.versionId;	
		string version_label = objFileRequest.versionLabel;

		return fileDoc;
	}

	public static string getOriginalFileName(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.FileRequest objFileRequest)
	{
		using (DocsPaDB.Query_DocsPAWS.Documenti dbDocumenti = new DocsPaDB.Query_DocsPAWS.Documenti())
		{
			string OriginalfileName = dbDocumenti.GetNomeOriginale(objFileRequest.versionId, objFileRequest.docNumber);
			if (!string.IsNullOrEmpty(OriginalfileName))
			{
				return removeIllegalChars(OriginalfileName, false);
			}
			else
			{
				return null;
			}

		}
	}

	public static SchedaDocumento Stamp(labelPdf labelPdf, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.FileRequest fr, SchedaDocumento schedaDoc, out ResultSigilloElettronico result)
	{
		bool isConverted;
		bool signResult = true;
		result = ResultSigilloElettronico.OK;
		try
		{
			if (!(Convert.ToInt32(fr.fileSize) > 0))
			{
				result = ResultSigilloElettronico.FILE_NON_ACQUISITO;
				return schedaDoc;
			}

			//Verifico che il file è un pdf
			if (!Path.GetExtension(fr.fileName).ToUpper().Equals(".PDF"))
			{
				result = ResultSigilloElettronico.FORMATO_FILE_NON_VALIDO;
				return schedaDoc;
			}

			string segnaturaPermanente = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "FE_SEGNATURA_PERMANENTE");
			if (!string.IsNullOrEmpty(segnaturaPermanente) && segnaturaPermanente.Equals("1"))
			{
				//Estraggo l'ultima versione del documento dal DB perchè quella passata dal FE potrebbe non essere aggiornata.
				schedaDoc.documenti = new ArrayList();
				schedaDoc.documenti.AddRange(BusinessLogic.Documenti.DocManager.GetVersionsMainDocument(infoUtente, schedaDoc.docNumber));
				fr = (FileRequest)schedaDoc.documenti[0];

				DocsPaVO.amministrazione.InfoAmministrazione currAmm = Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoUtente.idAmministrazione);
				DocsPaVO.documento.FileDocumento fd = BusinessLogic.Documenti.FileManager.getFile(fr, infoUtente, false, false, out isConverted);

				string stampText = string.Empty;
				Registro reg = null;
				if (schedaDoc.protocollo != null && !string.IsNullOrEmpty(schedaDoc.protocollo.segnatura))
				{
					reg = schedaDoc.registro;
					stampText = GetDatiEtichetta(infoUtente, currAmm, labelPdf, schedaDoc, null, string.Empty);
				}
				else
				{
					//caso di documento repertoriato e non protocollato
					if (schedaDoc.template == null && schedaDoc.tipologiaAtto != null && !string.IsNullOrEmpty(schedaDoc.tipologiaAtto.systemId))
					{
						DocsPaDB.Query_DocsPAWS.Model model = new DocsPaDB.Query_DocsPAWS.Model();
						DocsPaVO.ProfilazioneDinamica.Templates template = model.getTemplateDettagli(schedaDoc.docNumber);
						schedaDoc.template = template;

					}
					if ((isDocRepertoriato(schedaDoc, currAmm.Codice)))
						stampText = GetDatiEtichettaProtocolloRepertorio(schedaDoc, currAmm.Codice);

					DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg = (from DocsPaVO.ProfilazioneDinamica.OggettoCustom o in schedaDoc.template.ELENCO_OGGETTI
																	   where o.TIPO.DESCRIZIONE_TIPO.Equals("Contatore") && o.REPERTORIO.Equals("1")
																	   select o).FirstOrDefault();
					if (ogg != null)
					{
						logger.Debug("Trovato contatore");
						switch (ogg.TIPO_CONTATORE)
						{
							case "T":
								reg = BusinessLogic.Utenti.RegistriManager.getRegistriRuolo(infoUtente.idCorrGlobali)[0] as DocsPaVO.utente.Registro;
								break;
							case "A":
								reg = BusinessLogic.Utenti.RegistriManager.getRegistro(ogg.ID_AOO_RF);
								break;
							case "R":
								reg = BusinessLogic.Utenti.RegistriManager.getRegistro(ogg.ID_AOO_RF);
								reg = BusinessLogic.Utenti.RegistriManager.getRegistro(reg.idAOOCollegata);
								break;
						}
					}
				}
				int leftX = 0; int leftY = 0; int rightX = 0; int rightY = 0;
				int h = int.Parse(labelPdf.pdfHeight);
				int w = int.Parse(labelPdf.pdfWidth);
				int areaH = Convert.ToInt32(labelPdf.font_size);  //parametriziamo in base all' alteza del font?
				int areaW = w;
                switch (labelPdf.default_position)
                {
                    /*
                    case "pos_upSx":
                        leftX = int.Parse((labelPdf.positions[0] as position).PosX); // in basso a sinistra
                        leftY = h - int.Parse((labelPdf.positions[0] as position).PosY); // in basso a sinistra zero parte dal basso del pdf
                        rightX = leftX + areaW;  //in alto a destra
                        rightY = leftY - areaH;
                        break;
                    case "pos_upDx":
                        leftX = int.Parse((labelPdf.positions[1] as position).PosX);
                        leftY = h - int.Parse((labelPdf.positions[1] as position).PosY); // in basso a sinistra zero parte dal basso del pdf
                        rightX = leftX + areaW; //dan verificare w semrba troppo piccolo w -5;
                        rightY = leftY - areaH;
                        break;
                    case "pos_downSx":
                        leftX = int.Parse((labelPdf.positions[2] as position).PosX);
                        leftY = h - int.Parse((labelPdf.positions[2] as position).PosY);
                        rightX = leftX + areaW;
                        rightY = leftY - areaH;

                        break;
                    case "pos_downDx":
                        leftX = int.Parse((labelPdf.positions[3] as position).PosX);
                        leftY = h - int.Parse((labelPdf.positions[3] as position).PosY);
                        rightX = leftX + areaW; //dan verificare w  w - 5;
                        rightY = leftY - areaH;
                        break;
                    default:
                        if ((from position x in labelPdf.positions where x.posName == "pos_pers" select x).FirstOrDefault() != null)
                        {
                            leftX = (from position x in labelPdf.positions where x.posName == "pos_pers" select int.Parse(x.PosX)).FirstOrDefault(); // in basso a sinistra
                            leftY = h - (from position x in labelPdf.positions where x.posName == "pos_pers" select int.Parse(x.PosY)).FirstOrDefault();// in basso a sinistra zero parte dal basso del pdf
                        }
                        else
                        {
                            leftX = Convert.ToInt32(labelPdf.default_position.Split('-')[0]); // in basso a sinistra
                            leftY = h - Convert.ToInt32(labelPdf.default_position.Split('-')[1]); // in basso a sinistra zero parte dal basso del pdf
                        }
                        rightX = leftX + areaW;  //in alto a destra
                        rightY = leftY - areaH;
                        break;
                    */
                    //Nuove coordinate
                    case "pos_upSx":
                        leftX = int.Parse((labelPdf.positions[0] as position).PosX); // in basso a sinistra
                        rightY = h - int.Parse((labelPdf.positions[0] as position).PosY); // in basso a sinistra zero parte dal basso del pdf
                        rightX = areaW;  //in alto a destra
                        leftY = rightY - (areaH * 3);
                        break;
                    case "pos_upDx":
                        leftX = int.Parse((labelPdf.positions[1] as position).PosX);
                        rightY = h - int.Parse((labelPdf.positions[1] as position).PosY); // in basso a sinistra zero parte dal basso del pdf
                        rightX = areaW;
                        leftY = rightY - (areaH * 3);
                        break;
                    case "pos_downSx":
                        leftX = int.Parse((labelPdf.positions[2] as position).PosX);
                        rightY = h - int.Parse((labelPdf.positions[2] as position).PosY);
                        rightX = areaW;
                        leftY = rightY - (areaH * 3);

                        break;
                    case "pos_downDx":
                        leftX = int.Parse((labelPdf.positions[3] as position).PosX);
                        rightY = h - int.Parse((labelPdf.positions[3] as position).PosY);
                        rightX = areaW;
                        leftY = rightY - (areaH * 3);
                        break;
                    default:
                        if ((from position x in labelPdf.positions where x.posName == "pos_pers" select x).FirstOrDefault() != null)
                        {
                            leftX = (from position x in labelPdf.positions where x.posName == "pos_pers" select int.Parse(x.PosX)).FirstOrDefault(); // in basso a sinistra
                            rightY = h - (from position x in labelPdf.positions where x.posName == "pos_pers" select int.Parse(x.PosY)).FirstOrDefault();// in basso a sinistra zero parte dal basso del pdf
                        }
                        else
                        {
                            leftX = Convert.ToInt32(labelPdf.default_position.Split('-')[0]); // in basso a sinistra
                            rightY = h - Convert.ToInt32(labelPdf.default_position.Split('-')[1]); // in basso a sinistra zero parte dal basso del pdf
                        }
                        rightX = areaW;   //in alto a destra
                        leftY = rightY - (areaH * 3);
                        break;
                }
                string statusCode = string.Empty;
				byte[] pdfFirmato = BusinessLogic.Documenti.DigitalSignature.RemoteSignature.Pdfsignature(reg.codiceIpa, currAmm.codiceIpa, fd.content, 1, leftX, leftY, rightX, rightY, stampText, out statusCode);
				if (pdfFirmato.Length <= 0)
				{
					switch (statusCode)
					{
						case "506":
							result = DocsPaVO.documento.ResultSigilloElettronico.SERVICE_UNAVAILABLE;
							break;
						case "504":
							result = DocsPaVO.documento.ResultSigilloElettronico.DATI_DI_FIRMA_ERRATI;
							break;
						default:
							result = DocsPaVO.documento.ResultSigilloElettronico.SYSTEM_ERROR;
							break;
					}
					return schedaDoc;
				}

				fr.conSegnaturaPermanente = true;
				signResult = BusinessLogic.Documenti.SignedFileManager.AppendDocumentoFirmatoPades(pdfFirmato, false, ref fr, infoUtente, "", "", true);

				if (signResult)
				{
					//Aggiorno la versione della scheda documento
					if (schedaDoc.documenti != null && schedaDoc.documenti.Count > 0)
					{
						List<Documento> listNewDocument = new List<Documento>();
						listNewDocument.Add(fr as Documento);
						listNewDocument.AddRange((schedaDoc.documenti.Cast<Documento>()).ToList());
						schedaDoc.documenti = new ArrayList(listNewDocument);
					}
				}
			}
		}
		catch (Exception e)
		{
			logger.Error("Errore in Stamp: " + e.Message);
			result = ResultSigilloElettronico.SYSTEM_ERROR;
		}
		return schedaDoc;
	}

	/// <summary>
	/// Determina e un documento è repertoriato 
	/// se definito per il documento
	/// </summary>
	/// <returns>ritorna una stringa rappresentante il protocollo di repertorio</returns>
	public static bool isDocRepertoriato(DocsPaVO.documento.SchedaDocumento sch, string codiceAmministrazione)
	{
		try
		{
			if (sch.template != null)
				if (sch.template.ELENCO_OGGETTI.Count > 0)
					foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom objAttrib in sch.template.ELENCO_OGGETTI)
						if (objAttrib.REPERTORIO == "1") return true;
			return false;
		}
		catch { return false; }
	}

	/// <summary>
	/// Determina e formatta una label contenente le info relative al protocollo di repertorio 
	/// se definito per il documento
	/// </summary>
	/// <returns>ritorna una stringa rappresentante il protocollo di repertorio</returns>
	public static string GetDatiEtichettaProtocolloRepertorio(DocsPaVO.documento.SchedaDocumento sch, string codiceAmministrazione)
	{
		string labelResult = string.Empty;
		try
		{
			//verifica se il documento ha associato un tipo documento 
			if (sch.template != null)
			{
				// verifica se esistono attributi associati al tipo documento
				if (sch.template.ELENCO_OGGETTI.Count > 0)
				{
					// verifica se almeno un attributo tipo funzione è di tipo repertoriato
					foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom objAttrib in sch.template.ELENCO_OGGETTI)
						if (objAttrib.REPERTORIO == "1" && objAttrib.CAMPO_COMUNE == "0")
							return string.Format("{0} - {1}", getFormattedProtocolloDiRepertorio(objAttrib, codiceAmministrazione), sch.template.DESCRIZIONE);
					return string.Empty;
				}
				else return string.Empty;
			}
			else return string.Empty;
		}
		catch
		{
			return string.Empty;
		}
	}

	/// <summary>
	/// Reperimento del file richiesto e conversione inline in pdf
	/// </summary>
	/// <remarks>
	/// La conversione in pdf è effettuata solo se il convertitore
	/// correntemente impostato supporta il formato del file originale
	/// </remarks>
	/// <param name="objFileRequest"></param>
	/// <param name="objSicurezza"></param>
	/// <param name="verificaFileFirmato"></param>
	/// <param name="convertPdfInLine">
	/// Se true, converte il file originale in formato pdf 
	/// (se il formato è supportato dal convertitore corrementente impostato)
	/// </param>
	/// <param name="isConverted">
	/// True se il file è stato convertito in pdf
	/// </param>
	/// <returns></returns>
	public static DocsPaVO.documento.FileDocumento getFile(
						DocsPaVO.documento.FileRequest objFileRequest,
						DocsPaVO.utente.InfoUtente objSicurezza,
						bool verificaFileFirmato,
						bool convertPdfInLine,
						out bool isConverted,
                        string basePath = "")
	{
		logger.Information("BEGIN");
		isConverted = false;
		DocsPaVO.documento.FileDocumento fileDocument = getFile(objFileRequest, objSicurezza, verificaFileFirmato, false, basePath);

		if (fileDocument != null && convertPdfInLine && (!string.IsNullOrEmpty(fileDocument.name)) && PdfConverter.CanConvertFile(fileDocument.name))
		{
			// Conversione in pdf del file, 
			// se la tipologia di file è tra quelle 
			// per cui è possibile effettuarla
			string outputfileName = Path.Combine(GetConvertPdfTempFolder(objSicurezza), objSicurezza.userId + "_" + objSicurezza.idPeople + "_" + fileDocument.name + ".pdf");

			isConverted = ConvertToPdf(objSicurezza, fileDocument, outputfileName);
		}
		logger.Information("END");
		return fileDocument;
	}

        /// <summary>
        /// Formatta il contatore del protocollo repertoriato 
        /// </summary>
        /// <returns></returns>
        private static string getFormattedProtocolloDiRepertorio(DocsPaVO.ProfilazioneDinamica.OggettoCustom objAtt,
                                                                 string codiceAmministrazione)
        {
            string ProtocolloFormattedResult = string.Empty;
            if (objAtt.VALORE_DATABASE != null && objAtt.VALORE_DATABASE != "")
            {
                ProtocolloFormattedResult = objAtt.FORMATO_CONTATORE;//.Replace("|", "/").Replace("-", "/");
                ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("ANNO", objAtt.ANNO);
                ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("CONTATORE", objAtt.VALORE_DATABASE);
                ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("COD_AMM", codiceAmministrazione);
                ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("COD_UO", objAtt.CODICE_DB);
                if (!string.IsNullOrEmpty(objAtt.DATA_INSERIMENTO))
                {
                    int fine = objAtt.DATA_INSERIMENTO.LastIndexOf(".");
                    if (fine == -1) fine = objAtt.DATA_INSERIMENTO.LastIndexOf(":");
                    ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("gg/mm/aaaa hh:mm", objAtt.DATA_INSERIMENTO.Substring(0, fine));
                    ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("gg/mm/aaaa", objAtt.DATA_INSERIMENTO.Substring(0, 10));
                }

                if (!string.IsNullOrEmpty(objAtt.ID_AOO_RF) && objAtt.ID_AOO_RF != "0")
                {
                    DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(objAtt.ID_AOO_RF);
                    if (reg != null)
                    {
                        //ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("RF", reg.codRegistro);
                        //ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("AOO", reg.codRegistro);
                        if (!string.IsNullOrEmpty(reg.chaRF) && reg.chaRF == "1")
                        {
                            ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("RF", reg.codRegistro);

                            if (!string.IsNullOrEmpty(reg.idAOOCollegata))
                            {
                                DocsPaVO.utente.Registro registro = new DocsPaVO.utente.Registro();
                                DocsPaDB.Query_DocsPAWS.Utenti rub = new DocsPaDB.Query_DocsPAWS.Utenti();
                                rub.GetRegistro(reg.idAOOCollegata, ref registro);
                                if (registro != null)
                                    ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("AOO", registro.codRegistro);
                            }
                        }
                        else //se contatore di AOO non ho i dati per ricavare RF perchè non mi viene passato in input. 
                        {
                            ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("RF", reg.codRegistro);
                            ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("AOO", reg.codRegistro);
                        }
                    }
                }
            }
            // codice protocollo di repertorio
            return string.Format("{0}", ProtocolloFormattedResult);
        }

        /// <summary>
        /// Reperimento cartella temporanea conversione pdf
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        private static string GetConvertPdfTempFolder(DocsPaVO.utente.InfoUtente infoUtente)
        {
            string basePath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");

            string codiceAmministrazione = string.Empty;

            using (DocsPaDB.Query_Utils.Utils dbUtils = new DocsPaDB.Query_Utils.Utils())
                codiceAmministrazione = dbUtils.getCodAmm(infoUtente.idAmministrazione);

            const string PARAM = "%DATA";

            if (basePath.Contains(PARAM))
                basePath = basePath.Replace(PARAM, string.Format(@"DPA.Convert\{0}\", codiceAmministrazione));
            else
                basePath = Path.Combine(basePath, string.Format(@"DPA.Convert\{0}\", codiceAmministrazione));

            return basePath;
        }

        private static bool ConvertToPdf(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.FileDocumento originalFileDocument, string outputPdfFile)
        {
            logger.Information("BEGIN");
            bool retValue = false;

            string temporaryFolder = GetConvertPdfTempFolder(infoUtente);

            if (!Directory.Exists(temporaryFolder))
                Directory.CreateDirectory(temporaryFolder);

            string temporaryFilePath = (Path.Combine(temporaryFolder, infoUtente.userId + "_" + Guid.NewGuid() + "_" + originalFileDocument.name));

            if (outputPdfFile.IndexOf(@"\") == -1)
                outputPdfFile = (temporaryFolder + outputPdfFile);

            try
            {
                // Il file da convertire viene salvato nella stessa cartella 
                // del file di output pdf per essere successivamente rimosso
                //if (File.Exists(temporaryFilePath))
                //    File.Delete(temporaryFilePath);


                if (!File.Exists(temporaryFilePath))
                {
                    File.WriteAllBytes(temporaryFilePath, originalFileDocument.content);
                }

                retValue = PdfConverter.Convert(temporaryFilePath, outputPdfFile);

                if (retValue)
                {

                    logger.Debug(string.Format("Documento '{0}' convertito in pdf nel file '{1}'", temporaryFilePath, outputPdfFile));

                    // Lettura del content del file pdf appena creato
                    byte[] newContent = File.ReadAllBytes(outputPdfFile);

                    // Aggiornamento degli attributi chiave relavivamente all'oggetto "FileDocumento"
                    originalFileDocument.content = newContent;
                    originalFileDocument.length = newContent.Length;
                    originalFileDocument.name += ".pdf";
                    originalFileDocument.fullName += ".pdf";
                    originalFileDocument.contentType = "application/pdf";
                    originalFileDocument.estensioneFile = "pdf";
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in ConvertToPdf: " + ex.Message);
            }

            try
            {
                // Tentativo di cancellazione del file originale temporaneo
                if (File.Exists(temporaryFilePath))
                    File.Delete(temporaryFilePath);

                // Tentativo di cancellazione del file pdf temporaneo
                if (File.Exists(outputPdfFile))
                    File.Delete(outputPdfFile);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore non bloccante in ConvertToPdf, non è stato possibile cancellare il file temporaneo: " + ex.Message);
            }
            logger.Information("END");
            return retValue;
        }

        public static bool VerifyFileSignatureXADES(DocsPaVO.documento.FileDocumento fileDoc, DateTime? dataDiRiferimento, string basePath)
        {
            bool retval = false;
            VerifySignature verifySignature = new VerifySignature();

            string inputDirectory = verifySignature.GetPKCS7InputDirectory(basePath);

            // Creazione cartella di appoggio nel caso non esista
            if (!System.IO.Directory.Exists(inputDirectory))
                System.IO.Directory.CreateDirectory(inputDirectory);

            logger.Debug("PKCS7InputDirectory: " + inputDirectory);

            string inputFile = string.Concat(inputDirectory, fileDoc.name);

            // Copia del file firmato dalla cartella del documentale
            // alla cartella di input utilizzata dal ws della verifica
            CopySignedFileToInputFolder(fileDoc, inputFile);
            fileDoc.signatureResult = verifySignature.VerifySignatureXADES_External(fileDoc, dataDiRiferimento.Value, basePath);
            try
            {
                // Rimozione del file firmato dalla cartella di input
                File.Delete(inputFile);
            }
            catch
            {
            }
            if (fileDoc.signatureResult != null && fileDoc.signatureResult.StatusCode == 0) //Valido
                retval = true;

            return retval;
        }

        /// <summary>
        /// Lettura e restituzione del contenuto del file originale
        /// estratto dal file firmato (dalla cartella di output utilizzata
        /// dal ws esterno che effettua la verififica della firma digitale)
        /// </summary>
        /// <param name="outputFileName"></param>
        /// <returns></returns>
        private static byte[] GetOutputFileContent(string outputFileName)
        {
            FileStream stream = new FileStream(outputFileName, FileMode.Open, FileAccess.Read);
            byte[] retValue = new byte[stream.Length];
            var read = stream.Read(retValue, 0, retValue.Length);
            stream.Flush();
            stream.Close();
            stream = null;
            return retValue;
        }

    private static void populateNameAndContent(DocsPaVO.documento.FileDocumento fileDoc)
    {
        int indice;

        if ((fileDoc != null) && (!String.IsNullOrEmpty(fileDoc.name)))
        {
            indice = fileDoc.name.LastIndexOf(@"\");
            if (indice < (fileDoc.name.Length - 1))
            {
                fileDoc.name = fileDoc.name.Substring(indice + 1);
            }
            //modifica
            if (string.IsNullOrEmpty(fileDoc.path))
                fileDoc.fullName = fileDoc.name;
            else
                //fineModifica
                fileDoc.fullName = fileDoc.path + '\u005C'.ToString() + fileDoc.name;

            fileDoc.contentType = getContentType(fileDoc.name);
        }
    }

    private static void VerifyFileTimeStamp(DocsPaVO.documento.FileDocumento fileDoc)
    {
        VerifyTimeStamp verifyTimestamp = new VerifyTimeStamp();
        DocsPaVO.areaConservazione.OutputResponseMarca marca = verifyTimestamp.Verify(fileDoc.content);
        fileDoc.timestampResult = marca;
        fileDoc.content = marca.DecryptedTSR.content;
        fileDoc.contentType = marca.DecryptedTSR.contentType;
        fileDoc.length = marca.DecryptedTSR.length;
    }

    public static void EstrazioneTSD(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento fileDoc, DocsPaVO.utente.InfoUtente objSicurezza)
    {
        EstrazioneTSContainer(fileRequest, fileDoc, objSicurezza, new DigitalSignature.PKCS_Utils.tsd());
    }

    public static void EstrazioneM7M(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento fileDoc, DocsPaVO.utente.InfoUtente objSicurezza)
    {
        EstrazioneTSContainer(fileRequest, fileDoc, objSicurezza, new DigitalSignature.PKCS_Utils.m7m());            //AggiuntaEVerificaMarca(fileRequest, objSicurezza, m7mhandler.Data.Content, m7mhandler.TSR);
    }

    private static void EstrazioneTSContainer(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento fileDoc, DocsPaVO.utente.InfoUtente objSicurezza, DigitalSignature.PKCS_Utils.ITimeStampedContainer TScontainer)
    {
        TScontainer.explode(fileDoc.content);
        fileDoc.content = TScontainer.Data.Content;

        if (!String.IsNullOrEmpty(fileDoc.name))
            fileDoc.name = removeLastExtension(fileDoc.name);
        if (!String.IsNullOrEmpty(fileDoc.fullName))
            fileDoc.fullName = removeLastExtension(fileDoc.fullName);
        if (!String.IsNullOrEmpty(fileDoc.nomeOriginale))
            fileDoc.nomeOriginale = removeLastExtension(fileDoc.nomeOriginale);
        if (!String.IsNullOrEmpty(fileRequest.fileName))
            fileRequest.fileName = removeLastExtension(fileRequest.fileName);
        if (!String.IsNullOrEmpty(fileRequest.path))
            fileRequest.path = removeLastExtension(fileRequest.path);

        fileRequest.fileSize = TScontainer.Data.Content.Length.ToString();
        fileDoc.estensioneFile = Path.GetExtension(fileDoc.fullName).Replace(".", "");
    }

    public static string removeLastExtension(string fileName)
    {
        return Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName));
    }

    public static VerifySignatureResult VerifyFileSignaturePades_External(DocsPaVO.documento.FileDocumento fileDoc, DateTime? dataDiRiferimento, string basePath)
    {
        bool retval = false;
        VerifySignature verifySignature = new VerifySignature();
        VerifySignatureResult signResult;


        string inputDirectory = verifySignature.GetPKCS7InputDirectory(basePath);

        // Creazione cartella di appoggio nel caso non esista
        if (!System.IO.Directory.Exists(inputDirectory))
            System.IO.Directory.CreateDirectory(inputDirectory);

        logger.Debug("PKCS7InputDirectory: " + inputDirectory);

        string inputFile = string.Concat(inputDirectory, fileDoc.name);

        // Copia del file firmato dalla cartella del documentale
        // alla cartella di input utilizzata dal ws della verifica
        CopySignedFileToInputFolder(fileDoc, inputFile);
        try
        {
            signResult = verifySignature.Verify_External(fileDoc, dataDiRiferimento.Value, basePath);
            if (signResult != null && signResult.PKCS7Documents != null && signResult.PKCS7Documents.Count() > 0)
            {
                signResult.PKCS7Documents[0].SignAlgorithm = "PADES" + signResult.PKCS7Documents[0].SignAlgorithm;
                signResult.PKCS7Documents[0].SignatureType = DocsPaVO.documento.SignType.PADES;
            }
            else
            {
                signResult = fileDoc.signatureResult;
            }
        }
        catch
        {
            // Rimozione del file firmato dalla cartella di input
            File.Delete(inputFile);
            signResult = fileDoc.signatureResult;
        }
        return signResult;
    }

    public static bool verificaPath(string path)
    {
        char[] separator = { ';' };
        string pathString = "";

        // chiedere a Gennaro...
        if (DocsPaVO.Settings.AppSettings.Instance.verifica_path_file != null)
        {
            pathString = DocsPaVO.Settings.AppSettings.Instance.verifica_path_file;

            if (pathString == null || pathString.Equals(""))
            {
                return false;
            }
        }
        else
        {
            return false;
        }
        logger.Debug(pathString);
        String[] pathFile = pathString.Split(separator);

        for (int i = 0; i < pathFile.Length; i++)
        {
            if (path.ToUpper().Equals(pathFile[i].ToUpper()))
            {
                return true;
            }
        }

        return false;
    }

    public static DateTime dataRiferimentoValitaDocumento(
       DocsPaVO.documento.FileRequest objFileRequest,
       DocsPaVO.utente.InfoUtente objSicurezza)
    {
        DateTime referenceDate = DateTime.MinValue;

        //Prendo il TimeStamp
        var ts = BusinessLogic.Documenti.TimestampManager.getTimestampsDoc(objSicurezza, objFileRequest);

        //Se presente almeno uno
        if (ts.Count > 0)
        {
            //Prendo la data temporale piu vecchia (last of)
            DocsPaVO.documento.TimestampDoc timestampDoc = ts[ts.Count - 1] as DocsPaVO.documento.TimestampDoc;
            if (!String.IsNullOrEmpty(timestampDoc.DTA_CREAZIONE))  //controlliamo se esiste al data creazione (deve esistere)
            {
                logger.Debug("Data di creazione da timestamp [{0}]", timestampDoc.DTA_CREAZIONE);
                try
                {
                    referenceDate = DocsPaUtils.Functions.Functions.ToDate(timestampDoc.DTA_CREAZIONE); //La prendiamo come buona.
                }
                catch
                {
                    referenceDate = Convert.ToDateTime(timestampDoc.DTA_CREAZIONE);
                }
            }
        }

        //Nel caso non fosse presente il timeTimestamp si fa un fallback verso la data di protocollazione
        if (referenceDate == DateTime.MinValue)
        {
            //Reperisco la scheda documento (usando sicurezza blanda)
            DocsPaVO.documento.SchedaDocumento schDoc = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(objSicurezza, objFileRequest.docNumber);

            //se scheda esiste e non è null
            if (schDoc != null)
            {
                if (schDoc.protocollo != null) // ed è presente un protocollo
                {
                    if (!String.IsNullOrEmpty(schDoc.protocollo.dataProtocollazione))  //e la data di procollazione non è nulla
                        referenceDate = DocsPaUtils.Functions.Functions.ToDate(schDoc.protocollo.dataProtocollazione); //Prendiamo quella data per buona.

                }
                if (referenceDate == DateTime.MinValue)
                {
                    string dataRepertoriazione = new DocsPaDB.Query_DocsPAWS.Documenti().GetDataCreazioneRepertorio(schDoc.docNumber);
                    if (!String.IsNullOrEmpty(dataRepertoriazione))
                    {
                        try
                        {
                            CultureInfo ci = new CultureInfo("it-IT");
                            referenceDate = DateTime.ParseExact(dataRepertoriazione, "dd/MM/yyyy HH:mm", ci.DateTimeFormat);

                        }
                        catch (Exception e)
                        {
                            logger.Error("Errore parsing la data [{0}] {1} {2}", dataRepertoriazione, e.Message, e.StackTrace);
                        }
                    }
                }
                if (referenceDate == DateTime.MinValue)
                {
                    try
                    {
                        referenceDate = DocsPaUtils.Functions.Functions.ToDate(schDoc.dataCreazione); //La prendiamo come buona.
                    }
                    catch
                    {
                        referenceDate = Convert.ToDateTime(schDoc.dataCreazione);
                    }
                }
            }
        }


        //Se tutto fallisce prendiamo la data attuale alle 00.00  
        if (referenceDate == DateTime.MinValue)
            referenceDate = DateTime.Now.Date;

        logger.Debug("referenceDate - {0}", referenceDate.ToString());

        return referenceDate;
    }

    public static string getOriginalExtension(string DocNumber, string version_id)
    {
        string result = string.Empty;

        try
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            result = doc.getOriginaExt(DocNumber, version_id);
        }
        catch (Exception e)
        {
            logger.Debug("Errore in ConservazioneManager  - metodo: getOriginalExtension", e);
            throw new Exception("Errore verifica formati");
        }

        return result;
    }

    /// <summary>
    /// Apro una sessione multisign,inserisce dentro i file
    /// </summary>
    /// <param name="infoUtente">oggetto infoutente</param>
    /// <param name="fileRequestList">array di filerequest</param>
    /// <param name="cofirma">opzione cofirma</param>
    /// <param name="timestamp">opzione timestamp</param>
    /// <param name="tipoFirma">PADES/CASES</param>
    /// <returns>token di sessione multisign</returns>
    public static string HSM_OpenMultiSignSession(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.FileRequest[] fileRequestList, bool cofirma, bool timestamp, string tipoFirma, Pi3.Core.Services.Configuration.IConfigurationService configurationService, string temporaryRootPath)
    {
        logger.Debug("inizio");
        RemoteSignature.SignType tipo = (RemoteSignature.SignType)Enum.Parse(typeof(RemoteSignature.SignType), tipoFirma);

        BusinessLogic.Documenti.DigitalSignature.RemoteSignature.MultiSign ms = new RemoteSignature.MultiSign(cofirma, timestamp, tipo, configurationService, temporaryRootPath);
        logger.Debug("Sessione HSM aperta, token: {0}", ms.SessionToken);
        List<string> tokenList = new List<string>();
        foreach (DocsPaVO.documento.FileRequest fr in fileRequestList)
        {
            if (fr != null)
            {
                DocsPaVO.documento.FileDocumento fd = null;
                try
                {
                    fd = BusinessLogic.Documenti.FileManager.getFileFirmato(fr, infoUtente, false);
                }
                catch (Exception e)
                {
                    logger.Error("Errore reperendo il file{0} {1}", e.Message, e.StackTrace);
                }

                if (fd != null)
                {
                    try
                    {
                        string hash = ms.Put(fd, fr.versionId, fr.fileName);
                        tokenList.Add(hash + "§" + fr.docNumber);
                    }
                    catch (Exception e)
                    {
                        logger.Error("Errore inserendolo in sessione il file{0} {1}", e.Message, e.StackTrace);
                    }
                }
            }
        }
        string retval = ms.SessionToken;
        foreach (string toks in tokenList)
            retval += "|" + toks;

        return retval;
    }

    public static async Task<bool> HSM_RequestOTP(String AliasCertificato, String DominioCertificato, IFirmaRemota2Service firmaRemotaService)
    {
        logger.Debug("inizio");
        try
        {
            return await BusinessLogic.Documenti.DigitalSignature.RemoteSignature.RichiediOTP(AliasCertificato, DominioCertificato, firmaRemotaService);
        }
        catch (Exception ex)
        {
            logger.Error("Errore invocando il metodo RichiediOTP {0} {1}", ex.Message, ex.StackTrace);
            return false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="infoUtente">infoutente</param>
    /// <param name="fr">filerequest</param>
    /// <param name="cofirma">richiedo cofirma</param>
    /// <param name="timestamp">richiedo timestamp</param>
    /// <param name="tipoFirma">CADES/PADES</param>
    /// <param name="AliasCertificato">Alias del certificato</param>
    /// <param name="DominioCertificato">Dominio del Certificato</param>
    /// <param name="OtpFirma">Otp della Firma</param>
    /// <param name="PinCertificato">Pin del certificato</param>
    /// <returns></returns>
    public static async Task<bool> HSM_Sign(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.FileRequest fr, bool cofirma, bool timestamp, string tipoFirma, String AliasCertificato, String DominioCertificato, String OtpFirma, String PinCertificato, bool ConvertPdf, IFirmaRemota2Service firmaRemotaService)
    {

        try
        {
            RemoteSignature.SignType tipo = (RemoteSignature.SignType)Enum.Parse(typeof(RemoteSignature.SignType), tipoFirma);
            BusinessLogic.Documenti.DigitalSignature.RemoteSignature rs = new RemoteSignature(AliasCertificato, DominioCertificato, PinCertificato, tipo, timestamp, cofirma,
                firmaRemotaService);

            DocsPaVO.documento.FileDocumento fd = BusinessLogic.Documenti.FileManager.getFileFirmato(fr, infoUtente, false);
            if (ConvertPdf)
            {
                fd = BusinessLogic.LiveCycle.LiveCycle.GeneratePDFInSyncMod(fd);
                fr.fileName = fd.name;
            }

            byte[] content = fd.content;
            // qui viene fatta la chiamata al servizio remoto Tibco
            byte[] signed = await rs.Sign(fr.fileName, content, OtpFirma);
            bool signResult = false;
            if ((tipo == RemoteSignature.SignType.PADES))
                signResult = BusinessLogic.Documenti.SignedFileManager.AppendDocumentoFirmatoPades(signed, cofirma, ref fr, infoUtente);
            else   //cades non cofirmato
                signResult = BusinessLogic.Documenti.SignedFileManager.AppendDocumentoFirmato(Convert.ToBase64String(signed), cofirma, ref fr, infoUtente);

            if (signResult)
            {
                //L'utente che firma potrebbe non essere lo stesso del certificato di firma, lo estraggo quindi dalla verfica firma 
                //string descrizioneFirmatario = BusinessLogic.Utenti.UserManager.getCorrispondenteByIdPeople(infoUtente.idPeople, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente).descrizione;
                string descrizioneFirmatario = GetSubjectNameSignature(AliasCertificato, DominioCertificato);
                if (string.IsNullOrEmpty(descrizioneFirmatario))
                    descrizioneFirmatario = BusinessLogic.Utenti.UserManager.getCorrispondenteByIdPeople(infoUtente.idPeople, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente).descrizione;
                FirmatarioDocumento firmatarioDoc = new FirmatarioDocumento() { IdProfile = fr.docNumber, IdVersion = fr.versionId, DescrizioneFirmatario = descrizioneFirmatario };
                BusinessLogic.Documenti.DocManager.InsertFirmatarioDocumento(firmatarioDoc);
            }

            return signResult;
        }
        catch (Exception ex)
        {
            logger.Error("Errore invocando il metodo RemoteSignature {0} {1}", ex.Message, ex.StackTrace);
            return false;
        }

    }

    /// <summary>
    /// Firma, reperimento file e chiusura sessione
    /// </summary>
    /// <param name="infoUtente">oggetto infoutente</param>
    /// <param name="fileRequestList">lista di filerequest</param>
    /// <param name="MultiSignToken">Token tornato dalla HSM_OpenMultiSignSession</param>
    /// <param name="AliasCertificato">alias certificato</param>
    /// <param name="DominioCertificato">dominio certificato</param>
    /// <param name="OtpFirma">Otp di firma</param>
    /// <param name="PinCertificato">Pin del certificato</param>
    /// <returns>valore firmati tutti ok, o errore</returns>
    public static async Task<FirmaResult[]> HSM_SignMultiSignSession(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.FileRequest[] fileRequestList, string MultiSignToken, String AliasCertificato, String DominioCertificato, String OtpFirma, String PinCertificato, bool cofirma, IFirmaRemota2Service firmaRemotaService, string temporaryRootPath, string repositoryRootPath = "")
    {
        logger.Debug("inizio");
        string[] tokenLst = MultiSignToken.Split('|');
        string sessionToken = tokenLst[0];
        List<FirmaResult> retval = new List<FirmaResult>();
        BusinessLogic.Documenti.DigitalSignature.RemoteSignature.MultiSign ms = new RemoteSignature.MultiSign(AliasCertificato, DominioCertificato, sessionToken, cofirma, firmaRemotaService, temporaryRootPath);
        string descrizioneFirmatario = GetSubjectNameSignature(AliasCertificato, DominioCertificato);
        if (string.IsNullOrEmpty(descrizioneFirmatario))
            descrizioneFirmatario = BusinessLogic.Utenti.UserManager.getCorrispondenteByIdPeople(infoUtente.idPeople, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente).descrizione;
        //bool cofirma = ms.Cofirma;
        bool result;
        string error = string.Empty;
        EsitoFirma esitoFirma;
        try
        {
            result = await ms.Sign(PinCertificato, OtpFirma);
        }
        catch (Exception e)
        {
            result = false;
            error = e.Message;
        }
        if (result)
        {
            foreach (string fileSession in tokenLst)
            {

                string[] hashLst = fileSession.Split('§');
                if (hashLst.Length != 2) //non ci sta la coppia , o è il primo o ci sono problemi
                    continue;

                string filehash = hashLst[0];
                string docNumber = hashLst[1];
                //seleziono l'oggetto FR dal docnumber del token
                DocsPaVO.documento.FileRequest fr = (from a in fileRequestList where a.docNumber == docNumber select a).FirstOrDefault() as DocsPaVO.documento.FileRequest;
                FirmaResult firmres = new FirmaResult { fileRequest = fr };

                // Non viene settato nel FE
                fr.inLibroFirma = BusinessLogic.LibroFirma.LibroFirmaManager.IsDocInLibroFirma(fr.docNumber);

                //non trovato
                if (fr == null)
                {
                    logger.Debug("FR è null.. male male male!");
                    firmres.errore = "false: Filerequest nullo";
                }
                else
                {
                    byte[] signed = ms.Get(filehash);
                    if (signed != null)
                    {
                        bool signResult = false;

                        //if ((ms.TipoFirma == RemoteSignature.SignType.PADES) || ms.Cofirma)  //cades cofirmato, o pades, non gradico venga cambiata l'ext
                        //    signResult = BusinessLogic.Documenti.SignedFileManager.AppendDocumentoFirmato(Convert.ToBase64String(signed), true, ref fr, infoUtente, true);
                        //else   //cades non cofirmato
                        //    signResult = BusinessLogic.Documenti.SignedFileManager.AppendDocumentoFirmato(Convert.ToBase64String(signed), false, ref fr, infoUtente, true);

                        logger.Debug($"fr.fileName: {fr.fileName},\n fr.versionId: {fr.versionId},\n Path.GetFileNameWithoutExtension: {Path.GetFileNameWithoutExtension(fr.fileName)}, estensione: {Path.GetExtension(fr.fileName)}");

						//Cambio del fileName perché sul path remoto dava problemi
						//fr.fileName = $"{fr.versionId}{Path.GetExtension(fr.fileName)}";
						//path remoto 
						var uri = new Uri(fr.fileName);
                        var filename = uri.Segments.Last();

                        fr.fileName = Uri.UnescapeDataString(filename);
                        logger.Debug($"fr.fileName nuovo: {filename}");

                        if (ms.TipoFirma == RemoteSignature.SignType.PADES)
                        {
                            signResult = BusinessLogic.Documenti.SignedFileManager.AppendDocumentoFirmatoPades(signed, ms.Cofirma, ref fr, infoUtente, repositoryRootPath, temporaryRootPath);
                        }
                        else
                        {
                            signResult = BusinessLogic.Documenti.SignedFileManager.AppendDocumentoFirmato(Convert.ToBase64String(signed), ms.Cofirma, ref fr, infoUtente, repositoryRootPath, temporaryRootPath);
                        }
                        if (!signResult)
                        {
                            firmres.errore = "false: Errore creando la nuova versione firmata";
                            if (fr.inLibroFirma)
                            {
                                string[] splitMsg = firmres.errore.Split(':');
                                BusinessLogic.LibroFirma.LibroFirmaManager.AggiornaErroreEsitoFirma(fr.docNumber, splitMsg[1].ToString());
                            }
                        }
                        else
                        {
                            //Inserisco nella tabella firmatari documento
                            //string descrizioneFirmatario = BusinessLogic.Utenti.UserManager.getCorrispondenteByIdPeople(infoUtente.idPeople, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente).descrizione;
                            FirmatarioDocumento firmatarioDoc = new FirmatarioDocumento() { IdProfile = fr.docNumber, IdVersion = fr.versionId, DescrizioneFirmatario = descrizioneFirmatario };
                            BusinessLogic.Documenti.DocManager.InsertFirmatarioDocumento(firmatarioDoc);

                            string method = "DOC_SIGNATURE";
                            string description = "Il documento è stato firmato digitalmente HSM CADES";
                            if (ms.TipoFirma == RemoteSignature.SignType.PADES)
                            {
                                method = "DOC_SIGNATURE_P";
                                description = "Il documento è stato firmato digitalmente HSM PADES";
                            }
							//Spostato a dopo l'inserimento in coda
                            //BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, method, fr.docNumber,
                                //description, DocsPaVO.Logger.CodAzione.Esito.OK, (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "0");

                            firmres.errore = "true: Versione creata con successo";

                            //MEV LIBRO FIRMA EMANUELA 12-06-2015: Se il documento è in libro firma aggiorno la data di esecuzione
                            if (fr.inLibroFirma)
                            {
                                BusinessLogic.LibroFirma.LibroFirmaManager.AggiornaDataEsecuzioneElemento(fr.docNumber, DocsPaVO.LibroFirma.TipoStatoElemento.FIRMATO.ToString());
                                BusinessLogic.LibroFirma.LibroFirmaManager.SalvaStoricoIstanzaProcessoFirmaByDocnumber(fr.docNumber, description, infoUtente);
                            }
                        }
                    }
                    else
                    {
                        firmres.errore = "false: Errore firmando remotamente il documento";
                    }
                }
                retval.Add(firmres);
            }
            ms.CloseSession();
        }
        else
        {
            //andato male.. CIAO CIAO
            logger.Debug("La firma multipla è andata male... esco e chiudo la sessione");
            ms.CloseSession();
            string id = ExtractFromString(error, "#CODE", "#MESSAGE");
            esitoFirma = GetEsitoFirma(id);
            error = ExtractFromString(error, "#MESSAGE", "#TYPE");
            retval.Add(new FirmaResult { errore = string.IsNullOrEmpty(error) ? "false: La firma multipla è fallita globalmente" : error, esito = esitoFirma });

            foreach (FileRequest fr in fileRequestList)
            {
                fr.inLibroFirma = BusinessLogic.LibroFirma.LibroFirmaManager.IsDocInLibroFirma(fr.docNumber);
                if (fr.inLibroFirma)
                {
                    string msg = string.IsNullOrEmpty(error) ? "Errore durante la procedura di firma" : error;
                    BusinessLogic.LibroFirma.LibroFirmaManager.AggiornaErroreEsitoFirma(fr.docNumber, msg);
                }
            }
        }
        return retval.ToArray();
    }

    public static bool processFileInformationCRLUpdate(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente objSicurezza, DocsPaVO.documento.FileDocumento filedocfirmato, DateTime DataDiRiferimentoCRL)
    {
        if (fileRequest.firmato == "1")
        {
            try
            {
                if (objSicurezza != null && string.IsNullOrEmpty(objSicurezza.dst))
                {
                    logger.Debug("Ricavo Token");
                    objSicurezza.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();
                    logger.Debug("Token ricavato");
                }

                DocsPaDB.Query_DocsPAWS.Documenti docs = new DocsPaDB.Query_DocsPAWS.Documenti();
                string fileInfoMask = docs.GetFileInfoMask(fileRequest.versionId, fileRequest.docNumber);
                logger.Debug("GET FileinfoMASK CRL for doc: {0} ver: {1}  mask: {2}", fileRequest.docNumber, fileRequest.versionId, fileInfoMask);
                DocsPaVO.documento.FileInformation fileInfo = DocsPaVO.documento.FileInformation.decodeMask(fileInfoMask);

                //controllo già effettuato in precedenza, non lo rifaccio..
                if ((fileInfo.Signature != DocsPaVO.documento.FileInformation.VerifyStatus.Valid) ||
                    (fileInfo.CrlStatus != DocsPaVO.documento.FileInformation.VerifyStatus.Valid))
                {
                    if (filedocfirmato.signatureResult.StatusCode == -100)
                    {
                        //server sta giu o non o funzionante
                        logger.Debug("Errore verificando la firma -100");
                        fileInfo.Signature = DocsPaVO.documento.FileInformation.VerifyStatus.InProgress;
                        fileInfo.CrlStatus = DocsPaVO.documento.FileInformation.VerifyStatus.InProgress;
                    }
                    else
                    {
                        logger.Debug("Status verificando la firma {0}", filedocfirmato.signatureResult.StatusCode);
                        bool hasErrs = false;
                        if (filedocfirmato.signatureResult.ErrorMessages != null)
                            if (filedocfirmato.signatureResult.ErrorMessages.Length != 0)
                                hasErrs = true;

                        if ((filedocfirmato.signatureResult.StatusCode != -1) && (hasErrs == false))
                        {
                            fileInfo.Signature = DocsPaVO.documento.FileInformation.VerifyStatus.Valid;
                            logger.Debug("firma OK");
                        }
                        else
                        {
                            fileInfo.Signature = DocsPaVO.documento.FileInformation.VerifyStatus.Invalid;
                            logger.Debug("firma NON OK");
                        }
                        //controllo CRL:
                        if (revokedCertArePresent(filedocfirmato))
                        {
                            logger.Debug("ATTENZIONE Sono presenti Certificati revocati");
                            fileInfo.CrlStatus = DocsPaVO.documento.FileInformation.VerifyStatus.Invalid;
                        }
                        else
                        {
                            logger.Debug("NON Sono presenti Certificati revocati");
                            fileInfo.CrlStatus = DocsPaVO.documento.FileInformation.VerifyStatus.Valid;
                        }

                    }

                    fileInfo.setGlobalStatus();

                    if (fileInfo.CheckRefDate == DateTime.MinValue)
                        fileInfo.CheckRefDate = DateTime.Now;

                    //if (fileInfo.CrlRefDate == DateTime.MinValue)
                    fileInfo.CrlRefDate = DataDiRiferimentoCRL;


                    fileInfoMask = DocsPaVO.documento.FileInformation.encodeMask(fileInfo);
                    logger.Debug("SET FileinfoMASK CRL for doc: {0} ver: {1}  mask: {2}", fileRequest.docNumber, fileRequest.versionId, fileInfoMask);
                    docs.UpdateComponentsFileInfo(fileInfoMask, fileRequest.versionId, fileRequest.docNumber);

                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore settando la CRL per la FileInformation {0} {1}", e.Message, e.StackTrace);
                return false;
            }
            return true;
        }
        return false;
    }

    public static bool revokedCertArePresent(DocsPaVO.documento.FileDocumento filedoc)
    {
        foreach (DocsPaVO.documento.PKCS7Document p7md in filedoc.signatureResult.PKCS7Documents)
        {
            foreach (DocsPaVO.documento.SignerInfo siinfo in p7md.SignersInfo)
            {
                if (siinfo.CertificateInfo.RevocationDate != DateTime.MinValue)
                    return true;
            }
        }
        return false;
    }

    private static string GetSubjectNameSignature(string aliasCertificato, string dominioCertificato)
    {
        string retValue = string.Empty;
        logger.Debug("START GetSubjectNameSignature");
        try
        {
            string certificate = BusinessLogic.Documenti.DigitalSignature.RemoteSignature.GetHSMCertificateList(aliasCertificato, dominioCertificato);
            logger.Debug("CERTIFICATO: " + certificate);
            if (!string.IsNullOrEmpty(certificate))
            {
                int startSubjectName = certificate.ToUpper().LastIndexOf("SUBJECTNAME\":");
                int endSubjectName = certificate.IndexOf("\",", startSubjectName);
                string subjectName = certificate.Substring(startSubjectName, endSubjectName - startSubjectName).Trim();

                int startCN = subjectName.ToUpper().LastIndexOf("CN=") + 3;
                int endCN = subjectName.IndexOf(",", startCN);
                if (endCN == -1)
                {
                    //caso CN=\"Giancarlo   Ruscitti\",
                    endCN = subjectName.Length;
                    string cn = subjectName.Substring(startCN, endCN - startCN).Trim();
                    int startRt = cn.ToUpper().LastIndexOf("\"") + 1;
                    int endRt = cn.IndexOf(@"\", startRt);
                    retValue = cn.Substring(startRt, endRt - startRt).Trim().Replace("   ", " ");
                }
                else
                    retValue = subjectName.Substring(startCN, endCN - startCN).Trim();
            }
        }
        catch (Exception e)
        {
            logger.Error("Errore in GetSubjectNameSignature: " + e.Message);
            retValue = string.Empty;
        }
        logger.Debug("END GetSubjectNameSignature");
        return retValue;
    }

    private static EsitoFirma GetEsitoFirma(string id)
    {
        EsitoFirma esito = new EsitoFirma();
        DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
        esito = documenti.GetMessaggioEsitoFirma(id);
        return esito;
    }

    private static string ExtractFromString(string text, string startString, string endString)
    {
        string result = text;
        int indexStart = 0, indexEnd = 0;
        indexStart = text.IndexOf(startString);
        indexEnd = text.IndexOf(endString);
        if (indexStart > 0 && indexEnd > 0)
        {
            indexStart += startString.Length;
            try
            {
                result = text.Substring(indexStart, indexEnd - indexStart);
            }
            catch (Exception e)
            {
                return result;
            }
        }
        return result;
    }

    /// <summary>
    /// Linearizza un file PDF nel caso non lo fosse già
    /// </summary>
    /// <param name="contentFile">byte[]</param>
    /// /// <returns>byte[]</returns>
    public static FileDocumento LinearizzePDFContent(FileDocumento contentFile)
    {
        FileDocumento tempFile = contentFile;

#if false
		PDFLinearizator.ConvertedPDF cpdf = PDFLinearizator.PDFLinearizator.LinearizePDFfromContent(contentFile.content, contentFile.nomeOriginale);

        if (cpdf != null && cpdf.ConvertedContent != null && cpdf.ConvertedContent.Length > 0)
        {
            tempFile.content = cpdf.ConvertedContent;
            tempFile.length = tempFile.content.Length;
        }
        else
            tempFile = contentFile;
#endif
        return tempFile;
    }


}
