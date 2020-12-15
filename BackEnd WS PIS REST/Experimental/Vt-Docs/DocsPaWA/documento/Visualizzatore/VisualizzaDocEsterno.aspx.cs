using DocsPAWA.DocsPaWR;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Services;
using System.IO;

namespace DocsPAWA.documento.Visualizzatore
{
    public partial class VisualizzaDocEsterno : System.Web.UI.Page
    {
        /// <summary>
        /// Url della pagina di visualizzazione del documento (nel frame)
        /// </summary>
        protected StringBuilder _frameSrc = new StringBuilder(String.Empty);

        #region Recupero parametri di configurazione

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

        #endregion

        #region Recupero dati e binding

        protected DocsPaWR.BaseInfoDoc[] GetData(string docNumber, string idProfile, string versionNumber)
        {
            // La lista con le informazioni di base su un documento.
            // Questa lista conterrà le informazioni sul documento e sui suoi allegati se
            // la è attiva la visualizzazione unificata altrimenti conterrà solo le informazioni
            // sul documento attualmente selezionato
            DocsPaWR.BaseInfoDoc[] temporary = null;

            // L'array da restituire
            DocsPaWR.BaseInfoDoc[] toReturn = null;

            // Caricamento della lista di documenti
            try
            {
                temporary = DocumentManager.GetBaseInfoForDocument(idProfile, docNumber, versionNumber);
            }
            catch (Exception e)
            {
                // In caso di errore, redirezionamento alla pagina di errore
                ErrorManager.redirect(this, e);
            }

            // Se non è attiva la visualizzazione unificata
            if (this.VisualizzazioneUnificata)
                // Bisogna restituire tutta la lista
                toReturn = temporary;
            else
                // Altrimenti viene prelevato il documento specificato
                if (!String.IsNullOrEmpty(docNumber))
                    // Altrimenti bisogna restituire solo l'elemento selezionato
                    toReturn = temporary.Where(e => e.DocNumber == docNumber).ToArray();
                else
                    toReturn = temporary.Where(e => e.IdProfile == idProfile).ToArray();


            // Restituzione della lista di informazioni
            return toReturn;

        }

        /// <summary>
        /// Associazione dati
        /// </summary>
        protected void Bind(BaseInfoDoc[] infoDocs)
        {
            this.grdButtons.DataSource = infoDocs;
            this.grdButtons.DataBind();

        }

        #endregion

        #region Proprietà per il binding
        /// <summary>
        /// Verifica se il file è stato acquisito per la versione
        /// correntemente visualizzata
        /// </summary>
        /// <param name="baseInfoDoc">Le informazioni di base sul documento visualizzato</param>
        /// <returns>True se il documento ha l'immagine acquisita</returns>
        protected bool IsAcquired(DocsPaWR.BaseInfoDoc baseInfoDoc)
        {
            return baseInfoDoc.HaveFile;
        }

        /// <summary>
        /// Reperimento della descrizione della versione
        /// </summary>
        /// <param name="baseInfoDoc">L'oggetto con le informazioni di base sul documento visualizzato</param>
        /// <returns>La descrizione da visualizzare per il documento</returns>
        protected string GetLabel(DocsPaWR.BaseInfoDoc baseInfoDoc)
        {
            if (baseInfoDoc.IsAttachment)
                return String.Format("{0}", baseInfoDoc.VersionLabel);
            else
                return string.Format("V.{0}", baseInfoDoc.VersionNumber.ToString());
        }

        /// <summary>
        /// Reperimento del tooltip della versione
        /// </summary>
        /// <param name="baseInfoDoc"></param>
        /// <returns></returns>
        protected string GetLabelTooltip(DocsPaWR.BaseInfoDoc baseInfoDoc)
        {
            string tooltip = baseInfoDoc.Description;

            if (tooltip.Length > 128)
                return tooltip.Substring(0, 125) + "...";
            else
                return tooltip.ToString();
        }

        /// <summary>
        /// Reperimento dell'url dell'immagine per la versione
        /// </summary>
        /// <param name="baseInfoDoc"></param>
        /// <returns></returns>
        //protected string GetVersionImage(DocsPaWR.BaseInfoDoc baseInfoDoc)
        //{
        //    bool isAcquired = this.IsAcquired(baseInfoDoc);

        //    if (baseInfoDoc.IsAttachment)
        //    {
        //        if (isAcquired)
        //            return "../../images/ico_allegato_enable.gif";
        //        else
        //            return "../../images/ico_allegato_disabled.gif";
        //    }
        //    else
        //        return "../../images/ico_doc_enable.gif";
        //}
        protected string GetVersionImage(DocsPaWR.BaseInfoDoc baseInfoDoc)
        {
            bool isAcquired = this.IsAcquired(baseInfoDoc);

            if (baseInfoDoc.IsAttachment)
            {
                if (DocumentManager.AllegatoIsPEC(baseInfoDoc.VersionId).Equals("1"))
                    return "../../images/allegato_pec.jpg";
                else
                {
                    if (DocumentManager.AllegatoIsIS(baseInfoDoc.VersionId).Equals("1"))
                        return "~/images/ico_allegato_PI3.jpg";
                    else
                    {
                        if (DocumentManager.AllegatoIsEsterno(baseInfoDoc.VersionId).Equals("1"))
                            return "~/images/allegato_esterno.jpg";
                        else
                        {
                            if (isAcquired)
                                return "../../images/ico_allegato_enable.gif";
                            else
                                return "../../images/ico_allegato_disabled.gif";
                        }
                    }
                }
            }
            else
                return "../../images/ico_doc_enable.gif";
        }

        /// <summary>
        /// Reperimento dell'url dell'immagine per la versione stampabile
        /// </summary>
        /// <returns></returns>
        protected string GetPrintableVersionImage()
        {
            return "../../images/proto/stampa.gif";
        }

        protected string GetName(BaseInfoDoc baseInfoDoc)
        {
            return baseInfoDoc.Name;
        }

        protected string GetDocNumber(BaseInfoDoc baseInfoDoc)
        {
            return baseInfoDoc.DocNumber;
        }

        protected string GetVersionId(BaseInfoDoc baseInfoDoc)
        {
            return baseInfoDoc.VersionId;
        }

        protected string GetVersionNumber(BaseInfoDoc baseInfoDoc)
        {
            return baseInfoDoc.VersionNumber.ToString();
        }

        protected string GetPath(BaseInfoDoc baseInfoDoc)
        {
            if (String.IsNullOrEmpty(baseInfoDoc.Path))
                return String.Empty;
            else
                return baseInfoDoc.Path;
        }

        protected string GetFileName(BaseInfoDoc baseInfoDoc)
        {
            if (String.IsNullOrEmpty(baseInfoDoc.FileName))
                return String.Empty;
            else
                return baseInfoDoc.FileName;
        }

        protected string GetIsProto(BaseInfoDoc baseInfoDoc)
        {
            return baseInfoDoc.IsProto.ToString();
        }

        #endregion

        #region Gestione colorazione lista bottoni
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
        /// Rimozione delle selezioni
        /// </summary>
        protected void ClearGridItemSelections()
        {
            foreach (DataGridItem item in this.grdButtons.Items)
                this.SelectGridItem(item, false);
        }

        #endregion

        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;

            // Validazione dei parametri
            bool validationResult = this.ValidateParams();

            // Se la validazione è andata a buon fine...
            if (validationResult)
                // ...si procede alla visualizzazione del documento
                this.LoadDocument();

            else
                // ...altrimenti, redirezionamento alla pagina di errore
                ErrorManager.redirectToErrorPage(this, new Exception(
                    String.Format("Parametri non validi o non sufficienti: {0}",
                        Request.QueryString.ToString())));
        }

        /// <summary>
        /// Caricamento delle informazioni sul documento
        /// </summary>
        private void LoadDocument()
        {
            // Il docNumber
            string docNumber = String.Empty;
            // L'idProfile
            string idProfile = String.Empty;
            // Il numero di versione
            string numVersion = String.Empty;

            // Lettura del docNumber
            if (!string.IsNullOrEmpty(Request.QueryString["docnumber"]))
                docNumber = Request["docNumber"];

            // Lettura dell'idProfile
            if (!string.IsNullOrEmpty(Request.QueryString["idprofile"]))
                idProfile = Request["idProfile"];

            // Lettura del numero di versione
            if (!String.IsNullOrEmpty(Request["numVersion"]))
                numVersion = Request["numVersion"];

            // Se non si è in postback
            if (!this.IsPostBack)
            {
                // Caricamento dei dati da agganciare al data grid
                BaseInfoDoc[] docs = this.GetData(docNumber, idProfile, numVersion);

                // Bind di docs con il data grid
                this.Bind(docs);

                // Visualizzazione del documento richiesto o in sua assenza, del primo documento
                // disponibile
                BaseInfoDoc docToShow = docs.Where(e => e.DocNumber == docNumber).FirstOrDefault();
                if (docToShow == null)
                    docToShow = docs[0];

                this.ShowDocument(docToShow.Description,
                    docToShow.DocNumber,
                    docToShow.VersionId,
                    docToShow.VersionNumber.ToString(),
                    docToShow.Path,
                    docToShow.FileName,
                    false,
                    docToShow.IsProto);

                // Sbiancamento preliminare
                this.ClearGridItemSelections();

                // Coloramento del primo elemento
                this.SelectGridItem(this.grdButtons.Items[0], true);

            }

        }

        protected void grdButtons_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            DataGridItem item = e.Item;

            if (item == null)
                return;

            if (e.CommandName == "ShowVersion")
                // Visualizzazione del documento in modalità standard
                this.PerformActionShowVersion(item);
            else if (e.CommandName == "ShowPrintableVersion")
                // Visualizzazione del documento in modalità attachment
                this.PerformActionShowPrintableVersion(item);

            this.ClearGridItemSelections();
            this.SelectGridItem(e.Item, true);
        }

        #endregion

        #region Visualizzazione documento

        public void ShowDocument(string docName,
            string docNumber,
            string versionId,
            string versionNumber,
            string path,
            string fileName,
            bool showAsAttachment,
            bool isProto)
        {
            // Impostazione titolo pagina
            if (isProto)
                this.appTitleProvider.PageName = "Segnatura di Protocollo: " + docName;
            else
                this.appTitleProvider.PageName = "Documento non Protocollato N° " + docName;

            #region Creazione query string

            // Preparazione della query string per la visualizzazione del documento
            this._frameSrc.AppendFormat("VisualizzaDocumento.aspx?downloadAsAttatchment={0}&docNumber={1}&versionId={2}&versionNumber={3}&path={4}&fileName={5}",
                showAsAttachment,
                docNumber,
                versionId,
                versionNumber,
                path,
                fileName);

            // Gestione della visualizzazione del file in originale
            if (!string.IsNullOrEmpty(Request.QueryString["visInOrig"]))
                this._frameSrc.AppendFormat("&visInOrig=",
                    Request.QueryString["visInOrig"].ToString());

            // Gestione visualizzazione con segnatura
            if (!string.IsNullOrEmpty(Request.QueryString["plusEtic"]))
                this._frameSrc.Append("&plusEtic=1&pos=" + Request.QueryString["pos"].ToString() +
                                    "&tipo=" + Request.QueryString["tipo"].ToString() +
                                    "&rotazione=" + Request.QueryString["rotazione"].ToString() +
                                    "&carattere=" + Request.QueryString["carattere"].ToString() +
                                    "&colore=" + Request.QueryString["colore"].ToString() +
                                    "&orientamento=" + Request.QueryString["orientamento"].ToString());

            #endregion

            //   Amministrazione.UserControl.ScrollKeeper skVisUnificata = new Amministrazione.UserControl.ScrollKeeper();
            //   skVisUnificata.WebControl = "pnlButton";
            //   this.Form.Controls.Add(skVisUnificata);
            this.pnlButton.Width = Unit.Percentage(100);

            // Attach del source al frame
            this.iFrameVisUnificata.Attributes["src"] = String.Empty;
            this.iFrameVisUnificata.Attributes["src"] = this._frameSrc.ToString();
        }

        private void ShowDocument(DataGridItem item, bool showAsAttachment)
        {
            // Prelevamento di tutti i dati dall'item corrente
            string docName = ((Literal)item.Cells[4].FindControl("ltlDocName")).Text;
            string docNumber = ((Literal)item.Cells[4].FindControl("ltlDocNumber")).Text;
            string versionId = ((Literal)item.Cells[4].FindControl("ltlVersionId")).Text;
            string versionNumber = ((Literal)item.Cells[4].FindControl("ltlVersionNumber")).Text;
            string path = ((Literal)item.Cells[4].FindControl("ltlPath")).Text;
            string fileName = ((Literal)item.Cells[4].FindControl("ltlFileName")).Text;
            bool isProto = Boolean.Parse(((Literal)item.Cells[4].FindControl("ltlIsProto")).Text);

            // Si richiama la funzione per la visualizzazione del documento
            this.ShowDocument(docName, docNumber, versionId, versionNumber, path, fileName, showAsAttachment, isProto);

        }

        #endregion

        #region Validazione parametri
        /// <summary>
        /// Funzione per la validazione dei paramtri di query string
        /// Per essere validi è necessario che siano presenti almeno uno
        /// fra docNumber e idProfile.
        /// </summary>
        /// <returns>True se la validazione passa, fase altrimenti</returns>
        private bool ValidateParams()
        {
            // Si prova a leggere il docNumber
            string docNumber = Request["docNumber"];

            // Si prova a leggere l'idProfile
            string idProfile = Request["idProfile"];

            // docNumber ed idProfile non possono essere entrambi nulli
            return !(String.IsNullOrEmpty(docNumber) && String.IsNullOrEmpty(idProfile));

        }
        #endregion

        #region Actions

        private void PerformActionShowPrintableVersion(DataGridItem item)
        {
            this.ShowDocument(item, true);
        }

        private void PerformActionShowVersion(DataGridItem item)
        {
            this.ShowDocument(item, false);
        }

        #endregion

        [WebMethod]
        public static void AbandonSession()
        {
            UserManager.logoff(HttpContext.Current.Session);
            HttpContext.Current.Session.Abandon();
            //HttpContext.Current.Session.Abandon();
        }
    }
}