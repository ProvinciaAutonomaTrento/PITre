using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using System.Collections;
using System.Linq;
using System.IO;

namespace DocsPAWA.documento
{
    public partial class docVisualizzaFrame : System.Web.UI.Page
    {
        /// <summary>
        /// Url della pagina di visualizzazione del documento (nel frame)
        /// </summary>
        protected string _frameSrc = string.Empty;

        /// <summary>
        /// Verifica se è stata attivata la  modalità di visualizzazione dei file per il documento
        /// </summary>
        protected bool VisualizzazioneUnificata
        {
            get
            {
                // Determina se attiva la visualizzazione Unificata
                return ((System.Configuration.ConfigurationManager.AppSettings["VIS_UNIFICATA"] != null) &&
                        (System.Configuration.ConfigurationManager.AppSettings["VIS_UNIFICATA"] == "1"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected DocsPaWR.FileRequest[] GetData()
        {
            List<DocsPaWR.FileRequest> list = new List<DocsPAWA.DocsPaWR.FileRequest>();

            if (this.VisualizzazioneUnificata)
            {
                string versionIdDocumentoPrincipale = string.Empty;

                if (this.OnTabAllegati)
                    versionIdDocumentoPrincipale = this.VersionIdDocumentoPrincipale;
                else
                    versionIdDocumentoPrincipale = this.VersionId;

                // Se attiva la visualizzazione unificata, vengono visualizzati tutti gli allegati e il documento principale
                list.Add(this.GetFileRequest(versionIdDocumentoPrincipale));

                foreach (DocsPaWR.Allegato allegato in this.CurrentSchedaDocumento.allegati)
                    list.Add(allegato);
            }
            else
            {
                // Visualizzazione del solo documento selezionato
                list.Add(this.SelectedFile);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Associazione dati
        /// </summary>
        protected virtual void Bind()
        {
            this.grdButtons.DataSource = this.GetData();
            this.grdButtons.DataBind();
        }

        /// <summary>
        /// Verifica se il file è stato acquisito per la versione
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        protected bool IsAcquired(DocsPaWR.FileRequest fileRequest)
        {
            int fileSize;
            Int32.TryParse(fileRequest.fileSize, out fileSize);
            return (fileSize > 0);
        }

        /// <summary>
        /// Reperimento della descrizione della versione
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        protected string GetLabel(DocsPaWR.FileRequest fileRequest)
        {
            if (fileRequest.GetType() == typeof(DocsPaWR.Allegato))
                return fileRequest.versionLabel;
            else
            {
                if (Request.QueryString["save"] != null && Request.QueryString["save"].Equals("true"))
                {
                    int newId = Convert.ToInt32(fileRequest.version) + 1;
                    return string.Format("V.{0}", newId);
                }
                else
                    return string.Format("V.{0}", fileRequest.version.ToString());
            }
        }

        /// <summary>
        /// Reperimento della descrizione della versione
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        protected string GetNomeOriginale(DocsPaWR.FileRequest fileRequest)
        {
            FileManager fileMan = new FileManager();
            string retval =fileMan.getInfoFile (Page,fileRequest).nomeOriginale;
            if (retval == null)
                retval = "";
            return retval;
        }

        /// <summary>
        /// Reperimento del tooltip della versione
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        protected string GetLabelTooltip(DocsPaWR.FileRequest fileRequest)
        {
            string tooltip = string.Empty;

            if (fileRequest.GetType() == typeof(DocsPaWR.Allegato))
                tooltip = fileRequest.descrizione;
            else
                tooltip = this.CurrentSchedaDocumento.oggetto.descrizione;

            if (tooltip != null)
            {
                if (tooltip.Length > 128)
                    return tooltip.Substring(0, 125) + "...";
                else
                    return tooltip.ToString();
            }
            else
                return string.Empty;
        }

        /// <summary>
        /// Reperimento dell'url dell'immagine per la versione
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        protected string GetVersionImage(DocsPaWR.FileRequest fileRequest)
        {
            bool isAcquired = this.IsAcquired(fileRequest);

            if (fileRequest.GetType() == typeof(DocsPaWR.Allegato))
            {
                if (DocumentManager.AllegatoIsPEC(fileRequest.versionId).Equals("1"))
                {
                    return "../images/allegato_pec.jpg";
                }
                else if (DocumentManager.AllegatoIsIS(fileRequest.versionId).Equals("1"))
                {
                    return "~/images/ico_allegato_PI3.jpg";
                }
                else if (DocumentManager.AllegatoIsEsterno(fileRequest.versionId).Equals("1"))
                {
                    return "~/images/allegato_esterno.jpg";
                }                    
                else
                {
                    if (isAcquired)
                        return "../images/ico_allegato_enable.gif";
                    else
                        return "../images/ico_allegato_disabled.gif";
                }
            }
            else
                return "../images/ico_doc_enable.gif";
        }

        /// <summary>
        /// Reperimento dell'url dell'immagine per la versione stampabile
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        protected string GetPrintableVersionImage(DocsPaWR.FileRequest fileRequest)
        {
            return "../images/proto/stampa.gif";
        }

        /// <summary>
        /// Reperimento scheda documento correntemente in lavorazione
        /// </summary>
        protected DocsPaWR.SchedaDocumento CurrentSchedaDocumento
        {
            get
            {
                return DocumentManager.getDocumentoSelezionato(this);
            }
        }

        /// <summary>
        /// Visualizzazione del documento nel frame
        /// </summary>
        protected void ShowDocumentOnFrame(bool downloadAsAttatchment, bool versioneStamp)
        {
            string frameSrc = this._frameSrc;
            if (downloadAsAttatchment)
                frameSrc += "&downloadAsAttatchment=true&versioneStampabile=" + versioneStamp.ToString();
            this.iframeVisUnificata.Attributes["src"] = frameSrc;
        }

        /// <summary>
        /// Nasconde il documento visualizzato nel frame
        /// </summary>
        protected void HideDocumentOnFrame()
        {
            this.iframeVisUnificata.Attributes["src"] = string.Empty;
        }

        /// <summary>
        /// Id della versione del documento principale visualizzato
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        protected string VersionIdDocumentoPrincipale
        {
            get
            {
                if (this.ViewState["VersionIdDocumentoPrincipale"] != null)
                    return this.ViewState["VersionIdDocumentoPrincipale"].ToString();
                else
                    return this.CurrentSchedaDocumento.documenti[0].versionId;
            }
            set
            {
                this.ViewState["VersionIdDocumentoPrincipale"] = value;
            }
        }

        /// <summary>
        /// Id della versione del documento principale da visualizzare
        /// </summary>
        protected string VersionId
        {
            get
            {
                if (this.ViewState["VersionId"] != null)
                    return this.ViewState["VersionId"].ToString();
                else
                    return this.CurrentSchedaDocumento.documenti[0].versionId;
            }
            set
            {
                this.ViewState["VersionId"] = value;
            }
        }

        /// <summary>
        /// Indica se si è in visualizzazione documenti dal tab allegati del documento principale
        /// </summary>
        protected bool OnTabAllegati
        {
            get
            {
                if (this.ViewState["OnTabAllegati"] != null)
                    return Convert.ToBoolean(this.ViewState["OnTabAllegati"]);
                else
                    return false;
            }
            set
            {
                this.ViewState["OnTabAllegati"] = value;
            }
        }

        /// <summary>
        /// Selezione di un elemento nel datagrid
        /// </summary>
        /// <param name="item"></param>
        /// <param name="selected"></param>
        /// <returns></returns>
        protected void SelectGridItem(DataGridItem item, bool selected)
        {
            Label lblDescrizione = (Label)item.Cells[2].FindControl("lblDescrizione");

            if (selected)
            {
                item.Cells[2].BackColor = System.Drawing.Color.Navy;
                lblDescrizione.ForeColor = System.Drawing.Color.White;
            }
            else
            {
                item.Cells[2].BackColor = System.Drawing.Color.White;
                lblDescrizione.ForeColor = System.Drawing.Color.Black;
            }
        }

        /// <summary>
        /// Selezione elemento correntemente in sessione nel datagrid
        /// </summary>
        /// <param name="version"></param>
        protected void ShowVersion(DocsPaWR.FileRequest version)
        {
            foreach (DataGridItem item in this.grdButtons.Items)
            {               
                if (Server.HtmlDecode(item.Cells[0].Text).Trim() == version.versionId.Trim())
                {
                    this.SelectGridItem(item, true);

                    this.PerformActionShowVersion(version.versionId);

                    break;
                }
            }
        }

        /// <summary>
        /// Rimozione delle selezioni
        /// </summary>
        protected void ClearGridItemSelections()
        {
            foreach (DataGridItem item in this.grdButtons.Items)
                this.SelectGridItem(item, false);
        }

        /// <summary>
        /// Reperimento filerequest a partire dal versionid
        /// </summary>
        /// <param name="versionId"></param>
        /// <returns></returns>
        private DocsPaWR.FileRequest GetFileRequest(string versionId)
        {
            DocsPaWR.FileRequest fileRequest = this.CurrentSchedaDocumento.documenti.Where(e => e.versionId == versionId).FirstOrDefault();

            if (fileRequest == null)
                // Ricerca la versione negli allegati al documento
                fileRequest = this.CurrentSchedaDocumento.allegati.Where(e => e.versionId == versionId).FirstOrDefault();

            return fileRequest;
        }

        /// <summary>
        /// Reperimento della versione correntemente selezionata
        /// </summary>
        protected DocsPaWR.FileRequest SelectedFile
        {
            get
            {
                return FileManager.getSelectedFile();
            }
        }

        /// <summary>
        /// Verifica se la versione correntemente selezionata si riferisce ad un allegato
        /// </summary>
        protected bool IsAllegato
        {
            get
            {
                return this.SelectedFile.GetType() == typeof(DocsPaWR.Allegato);
            }
        }

        #region Event Handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;

            //provo a prendere la scheda da sessione se è diversa da zero
            DocsPaWR.SchedaDocumento schedaCorrente = this.CurrentSchedaDocumento;

            //se vengo dalle maschere di ricerche ho in query string altri due parametri (docnumber,idrofile)
            //che mi permettono di recupare da db il dettaglio documento.
            if (schedaCorrente == null)
            {
                schedaCorrente = new DocsPAWA.DocsPaWR.SchedaDocumento();

                string docNumber = string.Empty;
                string idProfile = string.Empty;

                if (!string.IsNullOrEmpty(Request.QueryString["docnumber"]))
                    docNumber = Request.QueryString["docnumber"];

                if (!string.IsNullOrEmpty(Request.QueryString["idprofile"]))
                    idProfile = Request.QueryString["idprofile"];

                schedaCorrente = DocumentManager.getDettaglioDocumento(this, idProfile, docNumber);
            }

            if (!this.IsPostBack)
            {
                // Reperimento versionid della versione correntemente selezionata
                this.VersionId = this.SelectedFile.versionId;

                if (this.SelectedFile.GetType() == typeof(DocsPaWR.Allegato))
                {
                    this.OnTabAllegati = true;

                    // Visualizzazione allegato da tab allegati del documento principale
                    this.VersionIdDocumentoPrincipale = schedaCorrente.documenti[0].versionId;

                    //foreach (DocsPaWR.FileRequest fileRequest in schedaCorrente.documenti)
                    //{
                    //    if (this.IsAcquired(fileRequest))
                    //    {
                    //        // Reperimento versionid dell'ultima versione acquisita del documento principale
                    //        this.VersionIdDocumentoPrincipale = fileRequest.versionId;
                    //        break;
                    //    }
                    //}
                }
                else
                {
                    this.OnTabAllegati = false;

                    this.VersionIdDocumentoPrincipale = this.VersionId;
                }

                this.Bind();
            }

            // imposto titleBar
            if (schedaCorrente.protocollo == null)
                this.Title = "Documento non Protocollato N° " + schedaCorrente.docNumber;
            else
                this.Title = "Segnatura di Protocollo: " + schedaCorrente.protocollo.segnatura;

            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                //btn_visualizza
                if (!string.IsNullOrEmpty(Request.QueryString["visInOrig"]))
                    this._frameSrc = "docVisualizza.aspx?id=" + Request.QueryString["id"].ToString() + "&visInOrig=" + Request.QueryString["visInOrig"].ToString();
                else
                    this._frameSrc = "docVisualizza.aspx?id=" + Request.QueryString["id"].ToString();
                //btn_visualizza con segnatura
                if (!string.IsNullOrEmpty(Request.QueryString["plusEtic"]))
                {
                    this._frameSrc += "&plusEtic=1&pos=" + Request.QueryString["pos"].ToString() +
                                        "&tipo=" + Request.QueryString["tipo"].ToString() +
                                        "&rotazione=" + Request.QueryString["rotazione"].ToString() +
                                        "&carattere=" + Request.QueryString["carattere"].ToString() +
                                        "&colore=" + Request.QueryString["colore"].ToString() +
                                        "&orientamento=" + Request.QueryString["orientamento"].ToString();
                    //Mev Firma1 - aggiunto parametro notimbro <
                    if (Session["notimbro"] != null)
                        this._frameSrc += "&notimbro=" + Session["notimbro"].ToString();
                    else
                        this._frameSrc += "&notimbro=false";
                    //>
                    if (Request.QueryString["save"] != null && (!string.IsNullOrEmpty(Request.QueryString["save"].ToString())))
                        this._frameSrc += "&save=true";
                }
            }

            AdminTool.UserControl.ScrollKeeper skVisUnificata = new AdminTool.UserControl.ScrollKeeper();
            skVisUnificata.WebControl = "pnlButton";
            this.Form.Controls.Add(skVisUnificata);
            this.pnlButton.Width = Unit.Percentage(100);

            if (!this.IsPostBack)
            {
                // Reperimento versione correntemente visualizzata
                DocsPaWR.FileRequest currentVersion = this.SelectedFile;
                if (this.IsAcquired(currentVersion))
                {
                    // Se acquisita, visualizzazione della versione
                    this.ShowVersion(currentVersion);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void grdButtons_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            string versionId = e.Item.Cells[0].Text;

            if (e.CommandName == "ShowVersion")
                this.PerformActionShowVersion(versionId);
            else if (e.CommandName == "ShowPrintableVersion")
                this.PerformActionShowPrintableVersion(versionId);

            this.ClearGridItemSelections();
            this.SelectGridItem(e.Item, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
        }

        #endregion

        #region Actions

        /// <summary>
        /// Azione di visualizzazione del documento nel frame
        /// </summary>
        /// <param name="versionId"></param>
        protected virtual void PerformActionShowVersion(string versionId)
        {
            DocsPaWR.FileRequest fileRequest = this.GetFileRequest(versionId);

            bool downloadAsAttatchment = false; 
            downloadAsAttatchment |=Path.GetExtension(fileRequest.fileName).ToLower().Equals(".eml");
            
            this.Session["VisualizzatoreUnificato.SelectedFileRequest"] = fileRequest;

            // Visualizzazione del documento nel frame
           // this.ShowDocumentOnFrame(false);
            this.ShowDocumentOnFrame(downloadAsAttatchment, false);
        }

        /// <summary>
        /// Azione di visualizzazione del documento tramite l'applicazione proprietaria
        /// </summary>
        /// <param name="versionId"></param>
        protected virtual void PerformActionShowPrintableVersion(string versionId)
        {
            this.HideDocumentOnFrame();

            DocsPaWR.FileRequest fileRequest = this.GetFileRequest(versionId);

            this.Session["VisualizzatoreUnificato.SelectedFileRequest"] = fileRequest;

            // Visualizzazione del documento nel frame
            this.ShowDocumentOnFrame(true, true);
        }

        #endregion
    }
}
