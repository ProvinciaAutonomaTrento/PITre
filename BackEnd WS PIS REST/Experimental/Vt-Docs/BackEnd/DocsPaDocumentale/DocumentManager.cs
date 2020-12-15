using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using DocsPaVO.utente;
using DocsPaDocumentale.Interfaces;

namespace DocsPaDocumentale.Documentale
{
    /// <summary>
    /// Classe per l'interazione con il motore documentale corrente.
    /// Internamente istanzia e utilizza l'oggetto del motore documentale
    /// che implementa l'interfaccia "IDocumentManager"
    /// </summary>
    public class DocumentManager : IDocumentManager
    {
        #region Ctros, variables, constants

        /// <summary>
        /// Tipo documentale corrente
        /// </summary>
        private static Type _type = null;

        /// <summary>
        /// Oggetto documentale corrente
        /// </summary>
        private IDocumentManager _instance = null;

        /// <summary>
        /// Reperimento del tipo relativo al documentale corrente
        /// </summary>
        static DocumentManager()
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["documentale"]))
            {
                string documentale = ConfigurationManager.AppSettings["documentale"].ToLower();


                //CONTROLLO SULLO STATO PROTOCOLLAZIONE LIBERA: SE TRUE CONTROLLO IL DOCUMENTALE???
                //string protLibera = ConfigurationManager.AppSettings["PROTOCOLLAZIONE LIBERA"].ToLower();
                //if (protLibera == "true")
                //{

                if (documentale.Equals(TipiDocumentaliEnum.Etnoteam.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_ETDOCS.Documentale.DocumentManager);
                else if (documentale.Equals(TipiDocumentaliEnum.Pitre.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_PITRE.Documentale.DocumentManager);
                else if (documentale.Equals(TipiDocumentaliEnum.CDC.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_CDC.Documentale.DocumentManager);
                //Giordano Iacozzilli  08/10/2012 Aggiunta strato SharePoint
                else if (documentale.Equals(TipiDocumentaliEnum.SharePoint.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_CDC_SP.Documentale.DocumentManager);
                //Fine
                //else if (documentale.Equals(TipiDocumentaliEnum.GFD.ToString().ToLower()))
                //    _type = typeof(DocsPaDocumentale_GFD.Documentale.TitolarioManager);
                //else if (documentale.Equals(TipiDocumentaliEnum.Hummingbird.ToString().ToLower()))
                //    _type = typeof(DocsPaDocumentale_HUMMINGBIRD.Documentale.TitolarioManager);
                //else if (documentale.Equals(TipiDocumentaliEnum.Filenet.ToString().ToLower()))
                //    _type = typeof(DocsPaDocumentale_FILENET.Documentale.TitolarioManager);
                //}
                //else
                //    _type = typeof(DocsPaDocumentale_ETDOCS.Documentale.DocumentManager);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected DocumentManager()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="wspia"></param>
        public DocumentManager(DocsPaVO.utente.InfoUtente infoUtente, string wspia)
        {
            bool protocollazioneLibera = false;
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["PROTOCOLLAZIONE_LIBERA"]) &&
                    ConfigurationManager.AppSettings["PROTOCOLLAZIONE_LIBERA"].ToUpper().Equals("TRUE"))
                protocollazioneLibera = bool.Parse(ConfigurationManager.AppSettings["PROTOCOLLAZIONE_LIBERA"].ToLower());

            /* MEV 3765 Gestione selettiva integrazione WSPIA
             * Modifica MCaropreso:
             * Scegli se utilizzare GFD o ETDOCS
             */
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["documentale"]))
            {
                string documentale = ConfigurationManager.AppSettings["documentale"].ToLower();
                if (protocollazioneLibera && documentale.Equals(TipiDocumentaliEnum.GFD.ToString().ToLower()))
                {
                    //if (wspia == "1")
                    //    _type = typeof(DocsPaDocumentale_GFD.Documentale.DocumentManager);
                    //else
                    //{
                        _type = typeof(DocsPaDocumentale_ETDOCS.Documentale.DocumentManager);
                    //}
                }
            }

            //else
            //{
            //    if (wspia == "0" || wspia == null)
            //        _type = typeof(DocsPaDocumentale_ETDOCS.Documentale.DocumentManager);
            //}

            this._instance = (IDocumentManager) Activator.CreateInstance(_type, infoUtente);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public DocumentManager(DocsPaVO.utente.InfoUtente infoUtente)
        {
            this._instance = (IDocumentManager) Activator.CreateInstance(_type, infoUtente);
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Reperimento istanza oggetto "IDocumentManager"
        /// relativamente al documentale correntemente configurato
        /// </summary>
        protected IDocumentManager Instance
        {
            get
            {
                return this._instance;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public bool CreateDocumentoStampaRegistro(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo)
        {
            return this.Instance.CreateDocumentoStampaRegistro(schedaDocumento, ruolo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <param name="ruoliSuperiori"></param>
        /// <returns></returns>
        public bool CreateDocumentoStampaRegistro(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.utente.Ruolo[] ruoliSuperiori)
        {
            return this.Instance.CreateDocumentoStampaRegistro(schedaDocumento, ruolo, out ruoliSuperiori);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="version"></param>
        /// <param name="versionId"></param>
        /// <param name="versionLabel"></param>
        /// <returns></returns>
        public byte[] GetFile(string docNumber, string version, string versionId, string versionLabel)
        {
            return this.Instance.GetFile(docNumber, version, versionId, versionLabel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allegato"></param>
        /// <param name="putfile"></param>
        /// <returns></returns>
        public bool AddAttachment(DocsPaVO.documento.Allegato allegato, string putfile)
        {
            return this.Instance.AddAttachment(allegato, putfile);
        }

        /// <summary>
        /// Modifica di un allegato
        /// </summary>
        /// <param name="allegato"></param>
        public void ModifyAttatchment(DocsPaVO.documento.Allegato allegato)
        {
            this.Instance.ModifyAttatchment(allegato);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <param name="sede"></param>
        /// <param name="ruoliSuperiori"></param>
        /// <returns></returns>
        public bool CreateDocumentoGrigio(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.utente.Ruolo[] ruoliSuperiori, string conCopia = null)
        {
            return this.Instance.CreateDocumentoGrigio(schedaDocumento, ruolo, out ruoliSuperiori);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <param name="sede"></param>
        /// <returns></returns>
        public bool CreateDocumentoGrigio(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo)
        {
            return this.Instance.CreateDocumentoGrigio(schedaDocumento, ruolo);
        }

        /// <summary>
        /// Predisposizione di un documento alla protocollazione
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public bool PredisponiProtocollazione(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            return this.Instance.PredisponiProtocollazione(schedaDocumento);
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
            return this.Instance.ProtocollaDocumentoPredisposto(schedaDocumento, ruolo, out risultatoProtocollazione);
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
            return this.Instance.SalvaDocumento(schedaDocumento, ufficioReferenteEnabled, out ufficioReferenteSaved);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="idamministrazione"></param>
        /// <returns></returns>
        public bool RemoveVersion(DocsPaVO.documento.FileRequest fileRequest)
        {
            return this.Instance.RemoveVersion(fileRequest);
        }

        /// <summary>
        /// Rimozione di un allegato
        /// </summary>
        /// <param name="allegato"></param>
        /// <returns></returns>
        public bool RemoveAttatchment(DocsPaVO.documento.Allegato allegato)
        {
            return this.Instance.RemoveAttatchment(allegato);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileDocumento"></param>
        /// <param name="fileRequest"></param>
        /// <param name="idamministrazione"></param>
        /// <returns></returns>
        public bool GetFile(ref DocsPaVO.documento.FileDocumento fileDocumento, ref DocsPaVO.documento.FileRequest fileRequest)
        {
            return this.Instance.GetFile(ref fileDocumento, ref fileRequest);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="fileDocumento"></param>
        /// <param name="estensione"></param>
        /// <param name="objSicurezza"></param>
        /// <returns></returns>
        public bool PutFile(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento fileDocumento, string estensione)
        {
            return this.Instance.PutFile(fileRequest, fileDocumento, estensione);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <param name="sede"></param>
        /// <param name="risultatoProtocollazione"></param>
        /// <param name="ruoliSuperiori"></param>
        /// <returns></returns>
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
            return this.Instance.CreateProtocollo(schedaDocumento, ruolo, out risultatoProtocollazione);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="daInviare"></param>
        /// <returns></returns>
        public bool AddVersion(DocsPaVO.documento.FileRequest fileRequest, bool daInviare)
        {
            return this.Instance.AddVersion(fileRequest, daInviare);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public string GetLatestVersionId(string docNumber)
        {
            return this.Instance.GetLatestVersionId(docNumber);
        }

        /// Setta a 1 cha_segnatura se la versione contiene un documento in formato pdf, con segnatura impressa
        /// </summary>
        /// <param name="versionId"></param>
        /// <returns>
        /// bool che indica l'esito dell'operazione di update
        /// </returns>
        public bool ModifyVersionSegnatura(string versionId)
        {
            return this.Instance.ModifyVersionSegnatura(versionId);
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
            return this.Instance.IsVersionWithSegnature(versionId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="docNumber"></param>
        /// <param name="version_id"></param>
        /// <param name="version"></param>
        /// <param name="subVersion"></param>
        /// <param name="versionLabel"></param>
        /// <param name="objSicurezza"></param>
        /// <returns></returns>
        public bool ModifyExtension(ref DocsPaVO.documento.FileRequest fileRequest, string docNumber, string version_id, string version,
                    string subVersion, string versionLabel)
        {
            return this.Instance.ModifyExtension(ref fileRequest, docNumber, version_id, version, subVersion, versionLabel);
        }

        /// <summary>
        /// Annullamento di un protocollo
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="protocolloAnnullato"></param>
        /// <returns></returns>
        public bool AnnullaProtocollo(ref DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.documento.ProtocolloAnnullato protocolloAnnullato)
        {
            return this.Instance.AnnullaProtocollo(ref schedaDocumento, protocolloAnnullato);
        }

        /// <summary>
        /// Modifica dei metadati di una versione
        /// </summary>
        /// <param name="fileRequest"></param>
        public void ModifyVersion(DocsPaVO.documento.FileRequest fileRequest)
        {
            this.Instance.ModifyVersion(fileRequest);
        }

        /// <summary>
        /// Inserimento di un documento nel cestino
        /// </summary>
        /// <param name="infoDocumento"></param>
        /// <returns></returns>
        public bool AddDocumentoInCestino(DocsPaVO.documento.InfoDocumento infoDocumento)
        {
            return this.Instance.AddDocumentoInCestino(infoDocumento);
        }

        /// <summary>
        /// Rimozione documenti
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public bool Remove(params DocsPaVO.documento.InfoDocumento[] items)
        {
            return this.Instance.Remove(items);
        }

        /// <summary>
        /// Ripristino del documento dal cestino
        /// </summary>
        /// <param name="infoDocumento"></param>
        /// <returns></returns>
        public bool RestoreDocumentoDaCestino(DocsPaVO.documento.InfoDocumento infoDocumento)
        {
            return this.Instance.RestoreDocumentoDaCestino(infoDocumento);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="docnumber"></param>
        /// <param name="versionid"></param>
        /// <returns></returns>
        public string GetFileExtension(string docnumber, string versionid)
        {
            return this.Instance.GetFileExtension(docnumber, versionid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="docnumber"></param>
        /// <param name="versionid"></param>
        /// <returns></returns>
        public string GetOriginalFileName(string docnumber, string versionid)
        {
            return this.Instance.GetOriginalFileName(docnumber, versionid);
        }

        /// <summary>
        /// Impostazione di un permesso su un documento
        /// </summary>
        /// <param name="infoDiritto"></param>
        /// <returns></returns>
        public bool AddPermission(DocsPaVO.documento.DirittoOggetto infoDiritto)
        {
            return this.Instance.AddPermission(infoDiritto);
        }

        /// <summary>
        /// Revoca di un permesso su un documento
        /// </summary>
        /// <param name="documentInfo"></param>
        /// <returns></returns>
        public bool RemovePermission(DocsPaVO.documento.DirittoOggetto infoDiritto)
        {
            return this.Instance.RemovePermission(infoDiritto);
        }

        /// <summary>
        /// Scambia il file associato ad un allegato con il file associato ad un documento
        /// </summary>
        /// <param name="allegato"></param>
        /// <param name="documento"></param>
        /// <returns></returns>
        public bool ScambiaAllegatoDocumento(DocsPaVO.documento.Allegato allegato, DocsPaVO.documento.Documento documento)
        {
            return this.Instance.ScambiaAllegatoDocumento(allegato, documento);
        }


        #endregion

        /// <summary>
        /// Metodo per l'assegnazione di un diritto di tipo A ad un ruolo
        /// </summary>
        /// <param name="rights">Informazioni sul diritto da assegnare</param>
        /// <returns>True se è andato bene</returns>
        public bool AddPermissionToRole(DocsPaVO.documento.DirittoOggetto rights)
        {
            return this.Instance.AddPermissionToRole(rights);
        }

        /// <summary>
        /// Aggiorna le ACL del documento
        /// </summary>
        /// <param name="schedaDocumento"></param>
        public virtual void RefreshAclDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            this.Instance.RefreshAclDocumento(schedaDocumento);
        }
    }
}
