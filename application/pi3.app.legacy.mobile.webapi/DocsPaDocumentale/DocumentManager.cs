// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaDocumentale.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DocsPaVO.Pubblicazione.FiltroDocumenti;

namespace DocsPaDocumentale.Documentale;

public class DocumentManager : IDocumentManager
{
    private static Type _type = null;

    /// <summary>
    /// Reperimento del tipo relativo al documentale corrente
    /// </summary>
    static DocumentManager()
    {
        //if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["documentale"]))
        //{
        //    string documentale = ConfigurationManager.AppSettings["documentale"].ToLower();


        //    //CONTROLLO SULLO STATO PROTOCOLLAZIONE LIBERA: SE TRUE CONTROLLO IL DOCUMENTALE???
        //    //string protLibera = ConfigurationManager.AppSettings["PROTOCOLLAZIONE LIBERA"].ToLower();
        //    //if (protLibera == "true")
        //    //{

        //    if (documentale.Equals(TipiDocumentaliEnum.Etnoteam.ToString().ToLower()))
        //        _type = typeof(DocsPaDocumentale_ETDOCS.Documentale.DocumentManager);
        //    else if (documentale.Equals(TipiDocumentaliEnum.Hummingbird.ToString().ToLower()))
        //        _type = typeof(DocsPaDocumentale_HUMMINGBIRD.Documentale.DocumentManager);
        //    else if (documentale.Equals(TipiDocumentaliEnum.Filenet.ToString().ToLower()))
        //        _type = typeof(DocsPaDocumentale_FILENET.Documentale.DocumentManager);
        //    else if (documentale.Equals(TipiDocumentaliEnum.Pitre.ToString().ToLower()))
        //        _type = typeof(DocsPaDocumentale_PITRE.Documentale.DocumentManager);
        //    else if (documentale.Equals(TipiDocumentaliEnum.CDC.ToString().ToLower()))
        //        _type = typeof(DocsPaDocumentale_CDC.Documentale.DocumentManager);
        //    else if (documentale.Equals(TipiDocumentaliEnum.GFD.ToString().ToLower()))
        //        _type = typeof(DocsPaDocumentale_GFD.Documentale.DocumentManager);

        //    //Giordano Iacozzilli  08/10/2012 Aggiunta strato SharePoint
        //    else if (documentale.Equals(TipiDocumentaliEnum.SharePoint.ToString().ToLower()))
        //        _type = typeof(DocsPaDocumentale_CDC_SP.Documentale.DocumentManager);
        //    //Fine
        //    //}
        //    //else
        //    //    _type = typeof(DocsPaDocumentale_ETDOCS.Documentale.DocumentManager);
        //}
        _type = typeof(DocsPaDocumentale_ETDOCS.Documentale.DocumentManager);
    }

    /// <summary>
    /// Oggetto documentale corrente
    /// </summary>
    private IDocumentManager _instance = null;

    protected IDocumentManager Instance
    {
        get
        {
            return this._instance;
        }
    }

    public DocumentManager(DocsPaVO.utente.InfoUtente infoUtente, string wspia)
    {
        bool protocollazioneLibera = false;
        if (!string.IsNullOrEmpty(DocsPaVO.Settings.AppSettings.Instance.PROTOCOLLAZIONE_LIBERA) &&
                DocsPaVO.Settings.AppSettings.Instance.PROTOCOLLAZIONE_LIBERA.ToUpper().Equals("TRUE"))
            protocollazioneLibera = bool.Parse(DocsPaVO.Settings.AppSettings.Instance.PROTOCOLLAZIONE_LIBERA.ToLower());

        /* MEV 3765 Gestione selettiva integrazione WSPIA
         * Modifica MCaropreso:
         * Scegli se utilizzare GFD o ETDOCS
         */
        if (!string.IsNullOrEmpty(DocsPaVO.Settings.AppSettings.Instance.documentale))
        {
            string documentale = DocsPaVO.Settings.AppSettings.Instance.documentale.ToLower();
            if (protocollazioneLibera && documentale.Equals(TipiDocumentaliEnum.GFD.ToString().ToLower()))
            {
#if false   // chiama le implementazioni del documentale
                if (wspia == "1")
                    _type = typeof(DocsPaDocumentale_GFD.Documentale.DocumentManager);
                else
                {
                    _type = typeof(DocsPaDocumentale_ETDOCS.Documentale.DocumentManager);
                }
#endif
            }
        }

        //else
        //{
        //    if (wspia == "0" || wspia == null)
        //        _type = typeof(DocsPaDocumentale_ETDOCS.Documentale.DocumentManager);
        //}

        this._instance = (IDocumentManager)Activator.CreateInstance(_type, infoUtente);
    }

    public DocumentManager(DocsPaVO.utente.InfoUtente infoUtente)
    {
        this._instance = (IDocumentManager)Activator.CreateInstance(_type, infoUtente);
    }

    public bool CreateDocumentoStampaRegistro(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo)
    {
        return this.Instance.CreateDocumentoStampaRegistro(schedaDocumento, ruolo);
    }

    public bool CreateDocumentoStampaRegistro(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.utente.Ruolo[] ruoliSuperiori)
    {
        return this.Instance.CreateDocumentoStampaRegistro(schedaDocumento, ruolo, out ruoliSuperiori);
    }

    public bool CreateDocumentoGrigio(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.utente.Ruolo[] ruoliSuperiori, string conCopia = null)
    {
        return this.Instance.CreateDocumentoGrigio(schedaDocumento, ruolo, out ruoliSuperiori);
    }

    public bool CreateDocumentoGrigio(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo)
    {
        return this.Instance.CreateDocumentoGrigio(schedaDocumento, ruolo);
    }

    public bool CreateProtocollo(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione, out DocsPaVO.utente.Ruolo[] ruoliSuperiori, string conCopia = null)
    {
        //for (int i = 0; i < ruolo.registri.Count; i++)
        //{
        //    DocsPaVO.utente.Registro reg = new Registro();
        //    reg = (DocsPaVO.utente.Registro) ruolo.registri[i];
        //    if (schedaDocumento.registro.codRegistro == reg.codRegistro)
        //    {
        //        if (reg.FlagWspia == "0" || reg.FlagWspia == null)
        //            _type = typeof(DocsPaDocumentale_ETDOCS.Documentale.DocumentManager);


        //        break;
        //    }
        //}
        return this.Instance.CreateProtocollo(schedaDocumento, ruolo, out risultatoProtocollazione, out ruoliSuperiori);
    }

    public bool CreateProtocollo(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione)
    {
        return this.Instance.CreateProtocollo(schedaDocumento, ruolo, out risultatoProtocollazione);
    }

    public bool SalvaDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento, bool ufficioReferenteEnabled, out bool ufficioReferenteSaved)
    {
        return this.Instance.SalvaDocumento(schedaDocumento, ufficioReferenteEnabled, out ufficioReferenteSaved);
    }

    public bool PredisponiProtocollazione(DocsPaVO.documento.SchedaDocumento schedaDocumento)
    {
        return this.Instance.PredisponiProtocollazione(schedaDocumento);
    }

    public bool ProtocollaDocumentoPredisposto(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione)
    {
        return this.Instance.ProtocollaDocumentoPredisposto(schedaDocumento, ruolo, out risultatoProtocollazione);
    }

    public bool AddVersion(DocsPaVO.documento.FileRequest fileRequest, bool daInviare)
    {
        return this.Instance.AddVersion(fileRequest, daInviare);
    }

    public bool RemoveVersion(DocsPaVO.documento.FileRequest fileRequest)
    {
        return this.Instance.RemoveVersion(fileRequest);
    }

    public bool AddAttachment(DocsPaVO.documento.Allegato allegato, string putfile)
    {
        return this.Instance.AddAttachment(allegato, putfile);
    }

    public void ModifyAttatchment(DocsPaVO.documento.Allegato allegato)
    {
        this.Instance.ModifyAttatchment(allegato);
    }

    public bool RemoveAttatchment(DocsPaVO.documento.Allegato allegato)
    {
        return this.Instance.RemoveAttatchment(allegato);
    }

    public bool AnnullaProtocollo(ref DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.documento.ProtocolloAnnullato protocolloAnnullato)
    {
        return this.Instance.AnnullaProtocollo(ref schedaDocumento, protocolloAnnullato);
    }

    public void ModifyVersion(DocsPaVO.documento.FileRequest fileRequest)
    {
        this.Instance.ModifyVersion(fileRequest);
    }

    public bool ModifyVersionSegnatura(string versionId)
    {
        return this.Instance.ModifyVersionSegnatura(versionId);
    }

    public bool IsVersionWithSegnature(string versionId)
    {
        return this.Instance.IsVersionWithSegnature(versionId);
    }

    public bool AddDocumentoInCestino(DocsPaVO.documento.InfoDocumento infoDocumento)
    {
        return this.Instance.AddDocumentoInCestino(infoDocumento);
    }

    public bool RestoreDocumentoDaCestino(DocsPaVO.documento.InfoDocumento infoDocumento)
    {
        return this.Instance.RestoreDocumentoDaCestino(infoDocumento);
    }

    public bool Remove(params DocsPaVO.documento.InfoDocumento[] items)
    {
        return this.Instance.Remove(items);
    }

    public byte[] GetFile(string docNumber, string version, string versionId, string versionLabel)
    {
        return this.Instance.GetFile(docNumber, version, versionId, versionLabel);
    }

    public bool GetFile(ref DocsPaVO.documento.FileDocumento fileDocumento, ref DocsPaVO.documento.FileRequest fileRequest)
    {
        return this.Instance.GetFile(ref fileDocumento, ref fileRequest);
    }

    public bool PutFile(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento fileDocumento, string estensione, string repositoryRootPath = "")
    {
        return this.Instance.PutFile(fileRequest, fileDocumento, estensione, repositoryRootPath);
    }

    public string GetLatestVersionId(string docNumber)
    {
        return this.Instance.GetLatestVersionId(docNumber);
    }

    public bool ModifyExtension(ref DocsPaVO.documento.FileRequest fileRequest, string docNumber, string version_id, string version,
                string subVersion, string versionLabel)
    {
        return this.Instance.ModifyExtension(ref fileRequest, docNumber, version_id, version, subVersion, versionLabel);
    }

    public string GetFileExtension(string docnumber, string versionid)
    {
        return this.Instance.GetFileExtension(docnumber, versionid);
    }

    public string GetOriginalFileName(string docnumber, string versionid)
    {
        return this.Instance.GetOriginalFileName(docnumber, versionid);
    }

    public bool AddPermission(DocsPaVO.documento.DirittoOggetto infoDiritto)
    {
        return this.Instance.AddPermission(infoDiritto);
    }

    public bool RemovePermission(DocsPaVO.documento.DirittoOggetto infoDiritto)
    {
        return this.Instance.RemovePermission(infoDiritto);
    }

    public bool ScambiaAllegatoDocumento(DocsPaVO.documento.Allegato allegato, DocsPaVO.documento.Documento documento)
    {
        return this.Instance.ScambiaAllegatoDocumento(allegato, documento);
    }

    public bool AddPermissionToRole(DocsPaVO.documento.DirittoOggetto rights)
    {
        return this.Instance.AddPermissionToRole(rights);
    }

    public virtual void RefreshAclDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento)
    {
        this.Instance.RefreshAclDocumento(schedaDocumento);
    }
}
