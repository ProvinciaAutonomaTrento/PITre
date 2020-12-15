using System;
using System.Collections;
using System.Xml;
using System.IO;
using System.Web.UI;
using DocsPAWA;
using ProtocollazioneIngresso.Log;
using DocsPAWA.DocsPaWR;

namespace ProtocollazioneIngresso.Scansione
{
	/// <summary>
	/// Upload dei documenti acquisiti mediante la protocollazione in ingresso
	/// </summary>
	public class DocumentUploader
	{
		private Page _page=null;

		public DocumentUploader(Page page)
		{ 
			this._page=page;
		}

        /// <summary>
        /// Upload del documento
        /// </summary>
        /// <param name="fileDoc"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool Upload(DocsPAWA.DocsPaWR.FileDocumento fileDoc, bool conversionePDFServer, out string errorMessage)
        {
            bool retValue = false;
            errorMessage = string.Empty;

            try
            {
                ProtocollazioneIngressoLog.WriteLogEntry(
                       string.Format("UploadDocumento (FileName: {0} - Dim: {1}", fileDoc.name, fileDoc.content.Length.ToString()));
            }
            catch
            {
            }

            DocsPAWA.DocsPaWR.FileRequest fileReq = this.GetFileRequest(fileDoc.name);

            // Booleano utilizzato per indicare se è necessario convertire in PDF in modalità Asincrona
            bool convertAsync = false;

            if (fileReq != null)
            {
                fileReq.cartaceo = fileDoc.cartaceo;

                ProtocollazioneIngresso.Login.LoginMng loginMng = new ProtocollazioneIngresso.Login.LoginMng(this._page);

                DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                ws.Timeout = System.Threading.Timeout.Infinite;

                // Se è attiva la conversione PDF sincrona, si prova a convertire il file facendo vincere lei
                // se la conversione va a buon fine, viene associato il file pdf al documento altrimenti viene
                // acquisito il TIF e si prova ad eseguire la conversione asincrona
                if (Utils.IsEbabledConversionePdfLatoServerSincrona().ToLower() == "true")
                {
                    FileDocumento convertedDoc = ws.GeneratePDFInSyncMod(fileDoc);

                    if (convertedDoc == null)
                        convertAsync = true;

                    if (convertedDoc != null && convertedDoc.content.Length > 0)
                        fileDoc = convertedDoc;

                }


                retValue = ws.DocumentoPutFileNoException(ref fileReq, fileDoc, loginMng.GetInfoUtente(), out errorMessage);
            }

            //Se abilitata la conversione lato server ed è necessario effettuare la conversione asincrona, 
            // chiamo il webmethod che mette in coda il file da convertire
            if (retValue && conversionePDFServer && convertAsync)
            {
                DocsPAWA.DocsPaWR.SchedaDocumento documento = null;

                if (fileReq.GetType() == typeof(DocsPAWA.DocsPaWR.Allegato))
                {
                    // Il documento acquisito è un allegato: reperimento della scheda documento
                    documento = DocumentManager.getDettaglioDocumento(this._page, fileReq.docNumber, fileReq.docNumber);
                }
                else
                {
                    ProtocollazioneIngresso.Protocollo.ProtocolloMng protocolloMng = new ProtocollazioneIngresso.Protocollo.ProtocolloMng(this._page);

                    documento = protocolloMng.GetDocumentoCorrente();
                }

                FileManager.EnqueueServerPdfConversion(this._page,
                                        UserManager.getInfoUtente(this._page),
                                        fileDoc.content,
                                        fileDoc.name,
                                        documento);
            }

            return retValue;
        }

		/// <summary>
		/// Reperimento oggetto "FileRequest" relativamente a:
		/// - Documento principale;
		/// - Nuova versione documento;
		/// - Allegato;
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		private DocsPAWA.DocsPaWR.FileRequest GetFileRequest(string fileName)
		{
			if (fileName.ToUpper().StartsWith("DOC"))
				return this.GetFileRequestDocumentoPrincipale();
			else if (fileName.ToUpper().StartsWith("VER"))
				return this.GetFileRequestVersione();
			else if (fileName.ToUpper().StartsWith("ALL"))
				return this.GetFileRequestAllegato();
			else
				return null;
		}

		/// <summary>
		/// Reperimento oggetto "FileRequest" relativamente al documento principale
		/// dalla scheda documento
		/// </summary>
		/// <returns></returns>
		private DocsPAWA.DocsPaWR.FileRequest GetFileRequestDocumentoPrincipale()
		{
			ProtocollazioneIngresso.Protocollo.ProtocolloMng protocolloMng=new ProtocollazioneIngresso.Protocollo.ProtocolloMng(this._page);
			return protocolloMng.GetDocumentoCorrente().documenti[0];
		}

		/// <summary>
		/// Creazione oggetto "FileRequest" relativamente ad una nuova versione del documento
		/// </summary>
		/// <returns></returns>
		private DocsPAWA.DocsPaWR.Documento GetFileRequestVersione()
		{
            DocsPAWA.DocsPaWR.Documento retValue = new DocsPAWA.DocsPaWR.Documento();
			retValue.descrizione=string.Empty;

			ProtocollazioneIngresso.Protocollo.ProtocolloMng protocolloMng=new ProtocollazioneIngresso.Protocollo.ProtocolloMng(this._page);

            DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento = protocolloMng.GetDocumentoCorrente();
			retValue.docNumber=schedaDocumento.docNumber;

            DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
			ProtocollazioneIngresso.Login.LoginMng loginMng=new ProtocollazioneIngresso.Login.LoginMng(this._page);
			retValue=(DocsPAWA.DocsPaWR.Documento) ws.DocumentoAggiungiVersione(retValue,loginMng.GetInfoUtente());
			loginMng=null;
			ws=null;

			// Inserimento del file request nella scheda documento
			this.AppendFileRequestDocumenti(schedaDocumento,retValue);
			
			return retValue;
		}

		/// <summary>
		/// Creazione oggetto "FileRequest" relativamente all'allegato del documento
		/// </summary>
		/// <returns></returns>
		private DocsPAWA.DocsPaWR.FileRequest GetFileRequestAllegato()
		{
            DocsPAWA.DocsPaWR.Allegato retValue = new DocsPAWA.DocsPaWR.Allegato();
			retValue.numeroPagine=0;
			retValue.descrizione=string.Empty;

			ProtocollazioneIngresso.Protocollo.ProtocolloMng protocolloMng=new ProtocollazioneIngresso.Protocollo.ProtocolloMng(this._page);
			retValue.docNumber=protocolloMng.GetDocumentoCorrente().docNumber;

            DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
			ProtocollazioneIngresso.Login.LoginMng loginMng=new ProtocollazioneIngresso.Login.LoginMng(this._page);
			retValue=ws.DocumentoAggiungiAllegato(loginMng.GetInfoUtente(),retValue);
			
			retValue.version="0";

			loginMng=null;
			ws=null;

			// Impostazione della descrizione dell'allegato
			retValue.descrizione="Allegato " + retValue.versionLabel;

			// Inserimento del file request nella scheda documento
			this.AppendFileRequestAllegati(protocolloMng.GetDocumentoCorrente(),retValue);

			return retValue;
		}

		/// <summary>
		/// Inserimento del file request nella scheda documento
		/// </summary>
		/// <param name="schedaDocumento"></param>
		/// <param name="fileRequest"></param>
		private void AppendFileRequestDocumenti(DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento,DocsPAWA.DocsPaWR.FileRequest fileRequest)
		{
			ArrayList list=new ArrayList(schedaDocumento.documenti);
			list.Add(fileRequest);

			schedaDocumento.documenti=null;

			schedaDocumento.documenti=new DocsPAWA.DocsPaWR.Documento[list.Count];
			list.CopyTo(schedaDocumento.documenti);
		}

		/// <summary>
		/// Inserimento del file request nella scheda documento
		/// </summary>
		/// <param name="schedaDocumento"></param>
		/// <param name="fileRequestAllegato"></param>
		private void AppendFileRequestAllegati(DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento,DocsPAWA.DocsPaWR.Allegato fileRequestAllegato)
		{
			ArrayList list=new ArrayList(schedaDocumento.allegati);
			list.Add(fileRequestAllegato);

			schedaDocumento.allegati=null;

			schedaDocumento.allegati=new DocsPAWA.DocsPaWR.Allegato[list.Count];
			list.CopyTo(schedaDocumento.allegati);
		}
	}
}
