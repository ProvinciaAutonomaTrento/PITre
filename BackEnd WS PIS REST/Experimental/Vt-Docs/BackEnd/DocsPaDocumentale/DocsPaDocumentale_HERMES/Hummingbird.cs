using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaDocumentale_HERMES.DMFileOperation;

namespace DocsPaDocumentale_HERMES_HB
{
    /// <summary>
    /// Class factory per la crezione di un oggetto Service di Hummingbird
    /// </summary>
    public class DocumentManagerHermesHB : IDocumentManager
    {
        public DocumentManagerHermesHB() { }

        #region Metodi da implementare

        public bool GetFile(ref DocsPaVO.documento.FileDocumento fileDocumento, ref DocsPaVO.documento.FileRequest fileRequest)
        {
            
            
            throw new NotImplementedException();
        }

        public byte[] GetFile(string docNumber, string version, string versionId, string versionLabel)
        {
            throw new NotImplementedException();
        }

        public bool PutFile(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento fileDocumento, string estensione)
        {
            String Libreria = "";
            String NomeForm = System.Configuration.ConfigurationManager.AppSettings["Hummingbird_form"];
            String Titolo_Doc = fileRequest.fileName;
            String Appl_ID = fileRequest.docNumber;
            String Autore = fileRequest.autore;
            String Note = fileRequest.descrizione;
            Byte[] pDoc = fileDocumento.content;
            String Trustee = "";
            String DMUser = System.Configuration.ConfigurationManager.AppSettings["Hummingbird_user"];
            String DMPassword = System.Configuration.ConfigurationManager.AppSettings["Hummingbird_password"];
            String esito = "";

            DMFileOperationSoapClient ws = new DMFileOperationSoapClient();
            esito = ws.AggiungiDocumentoSec(Libreria, NomeForm, Titolo_Doc, Appl_ID, Autore, Note, pDoc, Trustee, DMUser, DMPassword);
            if (esito == "0")
                return false;
            else
                return true;
        }
        #endregion

        #region Metodi da non implementare
        public bool AddAttachment(DocsPaVO.documento.Allegato allegato, string putfile)
        {
            throw new NotImplementedException();
        }

        public bool AddDocumentoInCestino(DocsPaVO.documento.InfoDocumento infoDocumento)
        {
            throw new NotImplementedException();
        }

        public bool AddPermission(DocsPaVO.documento.DirittoOggetto infoDiritto)
        {
            throw new NotImplementedException();
        }

        public bool AddPermissionToRole(DocsPaVO.documento.DirittoOggetto rights)
        {
            throw new NotImplementedException();
        }

        public bool AddVersion(DocsPaVO.documento.FileRequest fileRequest, bool daInviare)
        {
            throw new NotImplementedException();
        }

        public bool AnnullaProtocollo(ref DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.documento.ProtocolloAnnullato protocolloAnnullato)
        {
            throw new NotImplementedException();
        }

        public bool CreateDocumentoGrigio(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo)
        {
            throw new NotImplementedException();
        }

        public bool CreateDocumentoGrigio(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.utente.Ruolo[] ruoliSuperiori)
        {
            throw new NotImplementedException();
        }

        public bool CreateDocumentoStampaRegistro(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.utente.Ruolo[] ruoliSuperiori)
        {
            throw new NotImplementedException();
        }

        public bool CreateDocumentoStampaRegistro(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo)
        {
            throw new NotImplementedException();
        }

        public bool CreateProtocollo(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione)
        {
            throw new NotImplementedException();
        }

        public bool CreateProtocollo(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione, out DocsPaVO.utente.Ruolo[] ruoliSuperiori)
        {
            throw new NotImplementedException();
        }

        public string GetFileExtension(string docnumber, string versionid)
        {
            throw new NotImplementedException();
        }

        public string GetLatestVersionId(string docNumber)
        {
            throw new NotImplementedException();
        }

        public string GetOriginalFileName(string docnumber, string versionid)
        {
            throw new NotImplementedException();
        }

        public bool IsVersionWithSegnature(string versionId)
        {
            throw new NotImplementedException();
        }

        public void ModifyAttatchment(DocsPaVO.documento.Allegato allegato)
        {
            throw new NotImplementedException();
        }

        public bool ModifyExtension(ref DocsPaVO.documento.FileRequest fileRequest, string docNumber, string version_id, string version, string subVersion, string versionLabel)
        {
            throw new NotImplementedException();
        }

        public void ModifyVersion(DocsPaVO.documento.FileRequest fileRequest)
        {
            throw new NotImplementedException();
        }

        public bool ModifyVersionSegnatura(string versionId)
        {
            throw new NotImplementedException();
        }

        public bool PredisponiProtocollazione(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            throw new NotImplementedException();
        }

        public bool ProtocollaDocumentoPredisposto(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione)
        {
            throw new NotImplementedException();
        }

        public void RefreshAclDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            throw new NotImplementedException();
        }

        public bool Remove(params DocsPaVO.documento.InfoDocumento[] items)
        {
            throw new NotImplementedException();
        }

        public bool RemoveAttatchment(DocsPaVO.documento.Allegato allegato)
        {
            throw new NotImplementedException();
        }

        public bool RemovePermission(DocsPaVO.documento.DirittoOggetto infoDiritto)
        {
            throw new NotImplementedException();
        }

        public bool RemoveVersion(DocsPaVO.documento.FileRequest fileRequest)
        {
            throw new NotImplementedException();
        }

        public bool RestoreDocumentoDaCestino(DocsPaVO.documento.InfoDocumento infoDocumento)
        {
            throw new NotImplementedException();
        }

        public bool SalvaDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento, bool ufficioReferenteEnabled, out bool ufficioReferenteSaved)
        {
            throw new NotImplementedException();
        }

        public bool ScambiaAllegatoDocumento(DocsPaVO.documento.Allegato allegato, DocsPaVO.documento.Documento documento)
        {
            throw new NotImplementedException();
        }
        #endregion

        
    }
}
