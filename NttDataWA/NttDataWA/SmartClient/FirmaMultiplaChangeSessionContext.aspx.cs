using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.Utils;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;

namespace NttDataWA.SmartClient
{
    /// <summary>
    /// La pagina ha la responsabilità di modificare il contesto di sessione
    /// necessario per poter riutilizzare l'infrastruttura di firma digitale
    /// </summary>
    /// <remarks>
    /// La funzione di firma digitale finora è stata utilizzata solo direttamente
    /// nel profilo di un singolo documento. Pertanto utilizza le variabili
    /// di sessione dei metodi come "GetDocumentoSelezionato", "GetSelectedFile", ecc.
    /// Per uniformarsi a tale infrastruttura la pagina, ogni qualvolta
    /// invocata con l'id di un documento da query string, effettua il reperimento
    /// dei metadati necessari ed aggiorna il relativo stato in sessoine
    /// </remarks>
    public partial class FirmaMultiplaChangeSessionContext : Page
    {
        //private ILog logger = LogManager.GetLogger(typeof(FirmaMultiplaChangeSessionContext));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //try {
                //this.Response.Expires = -1;

                // Aggiornamento stato di sessione
                this.RefreshState();
            //}
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return;
            //}
        }

        /// <summary>
        /// Reperimento id del documento da query string
        /// </summary>
        protected string IdDocumento
        {
            get
            {
                return this.Request.QueryString["idDocumento"];
            }
        }

        /// <summary>
        /// Aggiornamento stato di sessione
        /// </summary>
        protected virtual void RefreshState()
        {
            try
            {
                // NB: 
                // Importante mantenere l'impostazione a null di questo flag,
                // utilizzato poi dalla pagina "docSalva.aspx". 
                // Se lo si toglie, la firma multipla non funziona. 
                // Grazie, utilizzo sessione selvaggio!
                UIManager.UserManager.setBoolDocSalva(this, null);

                FirmaDigitaleMng mng = new FirmaDigitaleMng();
                DocsPaWR.SchedaDocumento schedaDocumento = mng.GetSchedaDocumento(this.IdDocumento);

                if (schedaDocumento != null)
                {
                    bool consolidated = false;

                    if (((Convert.ToInt32(schedaDocumento.accessRights) > 0) && (Convert.ToInt32(schedaDocumento.accessRights) < 45))) 
                    {
                        // Non si posseggono i diritti per firmare il documento (attesa accettazione, stato finale, prot. annullati)
                        throw new ApplicationException("Non si posseggono i diritti per firmare il documento");
                    }
        
                    if (schedaDocumento.checkOutStatus != null) 
                    {
                        // Documento bloccato
                        throw new ApplicationException("Il documento risulta bloccato");
                    }
                          
                       
                    if  ((schedaDocumento.protocollo !=null) && (schedaDocumento.protocollo.protocolloAnnullato != null))
                    {
                        // Prot. annullato 
                        throw new ApplicationException("Il documento risulta annullato");
                    }
                                           
                    if (schedaDocumento.ConsolidationState != null &&
                        schedaDocumento.ConsolidationState.State > DocsPaWR.DocumentConsolidationStateEnum.None)
                    {
                        // Il documento risulta consolidato, non può essere firmato digitalmente
                        consolidated = true;
                    }

                    if (consolidated)
                        throw new ApplicationException("Il documento risulta in stato consolidato");
                    
                }
                else
                    throw new ApplicationException("Errore nel reperimento dei metadati del documento");

                DocsPaWR.FileRequest fileRequest = null;

                if (schedaDocumento.documenti != null && schedaDocumento.documenti.Length > 0)
                {
                    if (Request.QueryString["fromDoc"] != null && Request.QueryString["fromDoc"].Equals("1"))
                    {
                        //Emanuela 16-04-2014: modifica per consetire la firma anche degli allegati
                        //fileRequest = UIManager.FileManager.getSelectedFile();
                        fileRequest = schedaDocumento.documenti[0];
                        if (UIManager.DocumentManager.getSelectedAttachId() != null)
                        {
                            FileRequest fileReqTemp = FileManager.GetFileRequest(UIManager.DocumentManager.getSelectedAttachId());

                            Allegato a = new Allegato();
                            a.applicazione = fileRequest.applicazione;
                            a.daAggiornareFirmatari = fileRequest.daAggiornareFirmatari;
                            a.dataInserimento = fileRequest.dataInserimento;
                            a.descrizione = fileRequest.descrizione;
                            a.docNumber = fileRequest.docNumber;
                            a.docServerLoc = fileRequest.docServerLoc;
                            a.fileName = fileRequest.fileName;
                            a.fileSize = fileRequest.fileSize;
                            a.firmatari = fileRequest.firmatari;
                            a.firmato = fileRequest.firmato;
                            a.idPeople = fileRequest.idPeople;
                            a.path = fileRequest.path;
                            a.subVersion = fileRequest.version;
                            a.version = fileRequest.version;
                            a.versionId = fileRequest.versionId;
                            a.versionLabel = fileRequest.versionLabel;
                            a.cartaceo = fileRequest.cartaceo;
                            a.repositoryContext = fileRequest.repositoryContext;
                            a.TypeAttachment = 1;
                            a.numeroPagine = (fileReqTemp as Allegato).numeroPagine;

                            if ((fileRequest.fNversionId != null) && (fileRequest.fNversionId != ""))
                                a.fNversionId = fileRequest.fNversionId;

                            fileRequest = a;
                        }
                    }
                    else
                    {
                        schedaDocumento = DocumentManager.getDocumentListVersions(this.Page, this.IdDocumento, this.IdDocumento);
                        fileRequest = schedaDocumento.documenti[0];
                       // fileRequest = schedaDocumento.documenti[0];
                    }

                    int fileSize;
                    Int32.TryParse(fileRequest.fileSize, out fileSize);
                    
                    if (fileSize == 0)
                        throw new ApplicationException("Nessun file acquisito per il documento");

                    #region VERIFICA DIMENSIONE MASSIMA FILE
                    int maxDimFileSign = 0;
                    if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, Utils.DBKeys.FE_DO_BIG_FILE_MIN.ToString())) &&
                       !Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, Utils.DBKeys.FE_DO_BIG_FILE_MIN.ToString()).Equals("0"))
                        maxDimFileSign = Convert.ToInt32(Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, Utils.DBKeys.FE_DO_BIG_FILE_MIN.ToString()));
                    if (maxDimFileSign > 0 && Convert.ToInt32(fileSize) > maxDimFileSign)
                    {
                        string maxSize = Convert.ToString(Math.Round((double)maxDimFileSign / 1048576, 3));
                        string msg = "La dimensione del file supera il limite massimo consentito per la firma. Il limite massimo consentito e' " + maxSize + " Mb";
                        throw new ApplicationException(msg);
                    }
                    #endregion

                    if (string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_REQ_CONV_PDF.ToString())) || !Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_REQ_CONV_PDF.ToString()).Equals("1"))
                    {
                        if (!this.IsFormatSupportedForSign(fileRequest))
                            throw new ApplicationException("Formato file non ammesso per la firma");
                    }
                }

                if (fileRequest != null)
                {
                    // Impostazione scheda documento in sessione
                    // NB: Ultima operazione nei controlli

                    //Emanuela 17-04-2014: commentato perchè dal tab allegati alla chiusura della popup, dopo la firma, tornando nel tab profilo, caricava il
                    //profilo dell'allegato
                    //UIManager.DocumentManager.setSelectedRecord(schedaDocumento);

                    // Impostazione metadati file selezionato
                    UIManager.FileManager.setSelectedFile(fileRequest);

                    //ABBATANGELI MASSSIGNATURE HASH

                    bool cosign = false;
                    string tipoFirma = this.Request.QueryString["tipoFirma"];
                    if (!string.IsNullOrEmpty(tipoFirma))
                        if (tipoFirma.ToLower().Equals("cosign"))
                            cosign = true;

                    bool pades = false;
                    string tipoPades = this.Request.QueryString["pades"];
                    if (!string.IsNullOrEmpty(tipoPades))
                        if (tipoPades.ToLower().Equals("true"))
                            pades = true;

                    string extFile = Path.GetExtension(fileRequest.fileName);
                    bool isPadesSign = fileRequest.tipoFirma == TipoFirma.PADES || fileRequest.tipoFirma == TipoFirma.PADES_ELETTORNICA;
                    bool firmato = (!string.IsNullOrEmpty(fileRequest.firmato) && fileRequest.firmato.Trim() == "1");

                    if(!firmato && cosign)
                        throw new ApplicationException("Impossibile apporre la firma parallela perche' il file non risulta essere firmato");

                    if (firmato && extFile.ToUpper() == ".P7M" && pades)
                    {
                        throw new ApplicationException("Impossibile firmare PADES un file firmato CADES");
                    }

                    if (firmato && isPadesSign && !pades && cosign)
                    {
                        throw new ApplicationException("Impossibile cofirmare CADES un file firmato PADES");
                    }

                    NttDataWA.UIManager.FileManager.setMassSignature(fileRequest, cosign, pades, IdDocumento);
                }
                else
                    throw new ApplicationException("Errore nel reperimento dell'ultima versione del documento");
                
            }
            catch (Exception ex)
            {
                string exMessage = !String.IsNullOrEmpty(ex.Message) && ex.Message.Length < 200 ? ex.Message : "Errore generico";
                //FirmaDigitaleResultManager.SetData(
                //            new FirmaDigitaleResultStatus
                //            {
                //                Status = false,
                //                StatusDescription = exMessage,
                //                IdDocument = this.IdDocumento
                //            }
                //    );
                if (this.Response != null)
                {
                    this.Response.StatusCode = 500;
                    this.Response.Write(exMessage);
                    this.Response.End();
                    this.Response.Flush();
                    //throw new ApplicationException(exMessage);
                }
                //this.Response.StatusCode = -1;
                //this.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                //this.Response.Status = "999";
                //this.Response.StatusDescription = ex.Message;
                //this.Response.End();
                //throw new ApplicationException(ex.Message);

                //DocsPAWA.Logger.log("statusCode = " + this.Response.StatusCode);
                //DocsPAWA.Logger.log("StatusDescription = " + this.Response.StatusDescription);

                //DocsPAWA.Logger.logException(ex);
            }
        }

        /// <summary>
        /// Verifica se il formato del file è ammesso per la firma digitale
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        protected bool IsFormatSupportedForSign(DocsPaWR.FileRequest fileRequest)
        {
            bool retValue = false;

            if (!NttDataWA.FormatiDocumento.Configurations.SupportedFileTypesEnabled)
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

                    DocsPaWR.SupportedFileType[] fileTypes = ProxyManager.GetWS().GetSupportedFileTypes(Convert.ToInt32(UIManager.UserManager.GetInfoUser().idAmministrazione));

                    retValue = fileTypes.Count(e => e.FileExtension.ToLower() == extension.ToLower() && 
                                                e.FileTypeUsed && e.FileTypeSignature) > 0;
                }
            }

            return retValue;
        }
    }
}
