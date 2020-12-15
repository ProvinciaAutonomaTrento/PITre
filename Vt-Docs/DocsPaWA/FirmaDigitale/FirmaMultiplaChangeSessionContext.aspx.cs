using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;

namespace DocsPAWA.FirmaDigitale
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
    public partial class FirmaMultiplaChangeSessionContext : System.Web.UI.Page
    {
        private ILog logger = LogManager.GetLogger(typeof(FirmaMultiplaChangeSessionContext));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //this.Response.Expires = -1;

            // Aggiornamento stato di sessione
            this.RefreshState();
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
                UserManager.setBoolDocSalva(this, null);

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
                        fileRequest = FileManager.getSelectedFile(this);
                    }
                    else
                    {
                        fileRequest = schedaDocumento.documenti[0];
                    }

                    int fileSize;
                    Int32.TryParse(fileRequest.fileSize, out fileSize);
                    
                    if (fileSize == 0)
                        throw new ApplicationException("Nessun file acquisito per il documento");

                    if (!this.IsFormatSupportedForSign(fileRequest))
                        throw new ApplicationException("Formato file non ammesso per la firma");
                }

                if (fileRequest != null)
                {
                    // Impostazione scheda documento in sessione
                    // NB: Ultima operazione nei controlli
                    DocumentManager.setDocumentoSelezionato(schedaDocumento);

                    // Impostazione metadati file selezionato
                    FileManager.setSelectedFile(this, fileRequest);
                }
                else
                    throw new ApplicationException("Errore nel reperimento dell'ultima versione del documento");
            }
            catch (Exception ex)
            {
                //this.Response.StatusCode = -1;
                
                this.Response.StatusDescription = ex.Message;
                throw new ApplicationException(ex.Message);

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

            if (!DocsPAWA.FormatiDocumento.Configurations.SupportedFileTypesEnabled)
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

                    DocsPaWR.SupportedFileType[] fileTypes = ProxyManager.getWS().GetSupportedFileTypes(Convert.ToInt32(UserManager.getInfoUtente().idAmministrazione));

                    retValue = fileTypes.Count(e => e.FileExtension.ToLower() == extension.ToLower() && 
                                                e.FileTypeUsed && e.FileTypeSignature) > 0;
                }
            }

            return retValue;
        }
    }
}
