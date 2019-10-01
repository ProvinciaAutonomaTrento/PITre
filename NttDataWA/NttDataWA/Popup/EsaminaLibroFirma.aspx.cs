using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;

using NttDatalLibrary;
using NttDataWA.Utils;

namespace NttDataWA.Popup
{
    public partial class EsaminaLibroFirma : System.Web.UI.Page
    {
        #region Properties

        private List<ElementoInLibroFirma> ListaElementiSelezionati
        {
            get
            {
                List<ElementoInLibroFirma> result = null;
                if (HttpContext.Current.Session["ListaElementiSelezionati"] != null)
                {
                    result = HttpContext.Current.Session["ListaElementiSelezionati"] as List<ElementoInLibroFirma>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["ListaElementiSelezionati"] = value;
            }
        }

        private List<ElementoInLibroFirma> ListaElementiLibroFirma
        {
            get
            {
                if (HttpContext.Current.Session["ListaElementiLibroFirma"] != null)
                    return (List<ElementoInLibroFirma>)HttpContext.Current.Session["ListaElementiLibroFirma"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["ListaElementiLibroFirma"] = value;
            }
        }

        private List<ElementoInLibroFirma> ListaElementiFiltrati
        {
            get
            {
                List<ElementoInLibroFirma> result = null;
                if (HttpContext.Current.Session["ListaElementiFiltrati"] != null)
                {
                    result = HttpContext.Current.Session["ListaElementiFiltrati"] as List<ElementoInLibroFirma>;
                }
                return result;
            }

            set
            {
                HttpContext.Current.Session["ListaElementiFiltrati"] = value;
            }
        }

        private int ElementoSelezionato
        {
            get
            {
                int result = 0;
                if (HttpContext.Current.Session["ElementoSelezionato"] != null)
                {
                    result = Convert.ToInt32(HttpContext.Current.Session["ElementoSelezionato"]);
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["ElementoSelezionato"] = value;
            }
        }

        private bool IsZoom
        {
            get
            {
                return (bool)HttpContext.Current.Session["isZoom"];
            }

            set
            {
                HttpContext.Current.Session["isZoom"] = value;
            }
        }

        private bool EsaminaSingolo
        {
            get
            {
                if (!string.IsNullOrEmpty(Request.QueryString["caller"]) && Request.QueryString["caller"].Equals("s"))
                    return true;
                else
                    return false;
            }
        }

        private int MaxSelLibroFirma
        {
            get
            {
                int result = 100;
                if (HttpContext.Current.Session["MaxSelLibroFirma"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["MaxSelLibroFirma"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["MaxSelLibroFirma"] = value;
            }
        }

        private long maxFileSizeSelectable
        {
            get
            {
                if (HttpContext.Current.Session["MaxFileSizeSelectable"] != null)
                    return (long)HttpContext.Current.Session["MaxFileSizeSelectable"];
                else
                    return 0;
            }
            set
            {
                HttpContext.Current.Session["MaxFileSizeSelectable"] = value;
            }
        }


        private long totalFileSize
        {
            get
            {
                if (HttpContext.Current.Session["TotalFileSize"] != null)
                    return (long)HttpContext.Current.Session["TotalFileSize"];
                else
                    return 0;
            }
            set
            {
                HttpContext.Current.Session["TotalFileSize"] = value;
            }
        }
        #endregion

        #region Standard method

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.InitializeLanguage();
                this.InitializePage();
            }
            else
            {
                ReadRetValueFromPopup();
            }

            RefreshScript();
        }

        private void ReadRetValueFromPopup()
        {
            if (!string.IsNullOrEmpty(this.InterruptionSignatureProcess.ReturnValue))
            {
                this.txtMotivoRespingimento.Text = HttpContext.Current.Session["MotivoRespingimento"].ToString();
                HttpContext.Current.Session.Remove("MotivoRespingimento");
                this.pnlMotivoRespingimento.Attributes.Add("style", "display:block");
                this.UpdateStateElement(TipoStatoElemento.DA_RESPINGERE);
                if (this.EsaminaLFNonDiCompetenza.Visible)
                {
                    this.EsaminaLFNonDiCompetenza.Visible = false;
                    this.UpPnlButtons.Update();
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('InterruptionSignatureProcess','');", true);
            }
        }

        private void InitializeLanguage()
        {
            string language = UserManager.GetUserLanguage();
            this.EsaminaLFSelezionaPerFirma.Text = Utils.Languages.GetLabelFromCode("EsaminaLFSelezionaPerFirma", language);
            this.EsaminaLFSelezionaPerRespingimento.Text = Utils.Languages.GetLabelFromCode("EsaminaLFSelezionaPerRespingimento", language);
            this.EsaminaLFDeseleziona.Text = Utils.Languages.GetLabelFromCode("EsaminaLFDeseleziona", language);
            this.EsaminaLFNonDiCompetenza.Text = Utils.Languages.GetLabelFromCode("EsaminaLFNonDiCompetenza", language);
            this.EsaminaLFChiudi.Text = Utils.Languages.GetLabelFromCode("EsaminaLFChiudi", language);
            this.LtlDataCreazione.Text = Utils.Languages.GetLabelFromCode("EsaminaLFLtlDataCreazione", language);
            this.ltlOggetto.Text = Utils.Languages.GetLabelFromCode("EsaminaLFltlOggetto", language);
            this.ltlTipologia.Text = Utils.Languages.GetLabelFromCode("EsaminaLFltlTipologia", language);
            this.ltlDestinatario.Text = Utils.Languages.GetLabelFromCode("EsaminaLFlblDestinatario", language);
            this.ltlTipoFirma.Text = Utils.Languages.GetLabelFromCode("EsaminaLFltlTipoFirma", language);
            this.ltlProponente.Text = Utils.Languages.GetLabelFromCode("EsaminaLFltlProponente", language);
            this.ltlProvieneDa.Text = Utils.Languages.GetLabelFromCode("EsaminaLFltlProvieneDa", language);
            this.ltlFile.Text = Utils.Languages.GetLabelFromCode("EsaminaLFlblFile", language);
            this.ltlDataInserimento.Text = Utils.Languages.GetLabelFromCode("EsaminaLFlblDataInserimento", language);
            this.ltlStato.Text = Utils.Languages.GetLabelFromCode("EsaminaLFlblStato", language);
            this.ltlNote.Text = Utils.Languages.GetLabelFromCode("EsaminaLFlblNote", language);
            this.btn_first.AlternateText = Utils.Languages.GetLabelFromCode("btn_firstSmistaDocumenti", language);
            this.btn_first.ToolTip = Utils.Languages.GetLabelFromCode("btn_firstSmistaDocumenti", language);
            this.btn_previous.AlternateText = Utils.Languages.GetLabelFromCode("btn_previousSmistaDocumenti", language);
            this.btn_previous.ToolTip = Utils.Languages.GetLabelFromCode("btn_previousSmistaDocumenti", language);
            this.btn_next.AlternateText = Utils.Languages.GetLabelFromCode("btn_nextSmistaDocumenti", language);
            this.btn_next.ToolTip = Utils.Languages.GetLabelFromCode("btn_nextSmistaDocumenti", language);
            this.btn_last.AlternateText = Utils.Languages.GetLabelFromCode("btn_lastSmistaDocumenti", language);
            this.btn_last.ToolTip = Utils.Languages.GetLabelFromCode("btn_lastSmistaDocumenti", language);
            this.ltlIdDocumento.Text = Utils.Languages.GetLabelFromCode("EsaminaLtlIdDocumento", language);
            this.ltlSegnatura.Text = Utils.Languages.GetLabelFromCode("EsaminaLtlSegnatura", language);
            this.ltlMotivoRespingimento.Text = Utils.Languages.GetLabelFromCode("EsaminaltlMotivoRespingimento", language);
            this.chk_showDoc.Text = Utils.Languages.GetLabelFromCode("EsaminaLFChkShowDoc", language);
            this.chk_showDoc.ToolTip = Utils.Languages.GetLabelFromCode("EsaminaLFChkShowDocTooltip", language);
            this.InterruptionSignatureProcess.Title = Utils.Languages.GetLabelFromCode("EsaminaltlMotivoRespingimento", language);
            this.ltlOggettoAllegato.Text = Utils.Languages.GetLabelFromCode("EsaminaLFltlOggettoAllegato", language);
        }

        private string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
        }

        private void InitializePage()
        {
            ClearSession();
            PopolaCampi();

            if (this.EsaminaSingolo)
                this.pnlNavigationButtons.Visible = false;
        }

        private void ClearSession()
        {
            HttpContext.Current.Session.Remove("ElementoSelezionato");
        }

        private void PopolaCampi()
        {
            ElementoInLibroFirma elemento = this.ListaElementiSelezionati[this.ElementoSelezionato];
            InfoDocLibroFirma doc = elemento.InfoDocumento;
            DocsPaWR.SchedaDocumento schedaDocumentoSelezionato = DocumentManager.getDocumentDetails(this, elemento.InfoDocumento.Docnumber, elemento.InfoDocumento.Docnumber);
            FileManager.setSelectedFile(schedaDocumentoSelezionato.documenti[0]);
            if (!string.IsNullOrEmpty(elemento.InfoDocumento.IdDocumentoPrincipale))
            {
                DocumentManager.setSelectedAttachId(schedaDocumentoSelezionato.documenti[0].versionId);
                DocumentManager.setSelectedRecord(DocumentManager.getDocumentDetails(this, elemento.InfoDocumento.IdDocumentoPrincipale, elemento.InfoDocumento.IdDocumentoPrincipale));
            }
            else
            {
                DocumentManager.RemoveSelectedAttachId();
                DocumentManager.setSelectedRecord(schedaDocumentoSelezionato);
            }

            this.lblIdDocumento.Text = doc.Docnumber;

            if (string.IsNullOrEmpty(doc.NumProto))
            {
                this.pnlSegnatura.Attributes.Add("style", "display:none");
            }
            else
            {
                this.pnlSegnatura.Attributes.Add("style", "display:block");
                this.lblSegnatura.CssClass = "redWeight";
                this.lblSegnatura.Text = schedaDocumentoSelezionato.protocollo.segnatura;
            }

            this.lblDataCreazione.Text = doc.DataCreazione;
            if (string.IsNullOrEmpty(elemento.InfoDocumento.IdDocumentoPrincipale))
            {
                this.pnlOggettoAllegato.Visible = false;
                this.lblOggetto.Text = elemento.InfoDocumento.Oggetto;
                this.lblOggetto.ToolTip = elemento.InfoDocumento.Oggetto;
                this.lblOggettoAllegato.Text = string.Empty;
                this.ltlOggetto.Text = Utils.Languages.GetLabelFromCode("EsaminaLFltlOggetto", UserManager.GetUserLanguage());
            }
            else
            {
                this.lblOggetto.Text = elemento.InfoDocumento.OggettoDocumentoPrincipale;
                this.lblOggetto.ToolTip = elemento.InfoDocumento.OggettoDocumentoPrincipale;
                this.lblOggettoAllegato.Text = elemento.InfoDocumento.Oggetto;
                this.lblOggettoAllegato.ToolTip = elemento.InfoDocumento.Oggetto;
                this.pnlOggettoAllegato.Visible = true;
                this.ltlOggetto.Text = Utils.Languages.GetLabelFromCode("EsaminaLFltlOggettoDocumento", UserManager.GetUserLanguage());
            }
            if (!string.IsNullOrEmpty(doc.TipologiaDocumento))
            {
                this.lblTipologia.Text = doc.TipologiaDocumento;
                this.pnlTipologia.Attributes.Add("style", "display:block");
            }
            else
            {
                this.pnlTipologia.Attributes.Add("style", "display:none");
            }
            if (!string.IsNullOrEmpty(doc.Destinatario))
            {
                this.lblDestinatario.Text = doc.Destinatario;
                this.pnlDestinatario.Attributes.Add("style", "display:block");
            }
            else
            {
                this.pnlDestinatario.Attributes.Add("style", "display:none");
                //this.pnlDestinatario.Visible = false;
            }

            this.imgTypeSignature.ImageUrl = LibroFirmaManager.GetIconTypeSignature(elemento);
            this.imgTypeSignature.ToolTip = LibroFirmaManager.GetLabelTypeSignature(elemento.TipoFirma);

            if (elemento.UtenteProponente != null && !string.IsNullOrEmpty(elemento.UtenteProponente.idPeople))
            {
                if (!string.IsNullOrEmpty(elemento.DescProponenteDelegato))
                {
                    string language = UIManager.UserManager.GetUserLanguage();
                    string del = Utils.Languages.GetLabelFromCode("TransmissionDelegatedBy", language).ToUpper();

                    this.lblProvieneDa.Text = elemento.DescProponenteDelegato + " (" + elemento.RuoloProponente.descrizione + ") " + del + " " + elemento.UtenteProponente.descrizione;
                    this.lblProvieneDa.ToolTip = elemento.DescProponenteDelegato + " (" + elemento.RuoloProponente.descrizione + ") " + del + " " + elemento.UtenteProponente.descrizione;
                }
                else
                {
                    this.lblProvieneDa.Text = elemento.UtenteProponente.descrizione + " (" + elemento.RuoloProponente.descrizione + ")";
                    this.lblProvieneDa.ToolTip = elemento.UtenteProponente.descrizione + " (" + elemento.RuoloProponente.descrizione + ")";
                }
            }
            else
            {
                this.lblProvieneDa.Text = elemento.UtenteProponente + " (" + elemento.RuoloProponente + ")";
                this.lblProvieneDa.ToolTip = elemento.UtenteProponente + " (" + elemento.RuoloProponente + ")";
            }

            if (elemento.StatoFirma.Equals(TipoStatoElemento.DA_RESPINGERE))
            {
                this.txtMotivoRespingimento.Text = elemento.MotivoRespingimento;
                this.pnlMotivoRespingimento.Attributes.Add("style", "display:block");
            }
            else
            {
                this.txtMotivoRespingimento.Text = string.Empty;
                this.pnlMotivoRespingimento.Attributes.Add("style", "display:none");
            }


            this.lblFile.Text = "V" + doc.NumVersione.ToString();
            if (string.IsNullOrEmpty(doc.IdDocumentoPrincipale))
            {
                this.lblFile.Text += " DP";
            }
            else
            {
                this.lblFile.Text += " A" + doc.NumAllegato.ToString();
            }

            this.lblDataInserimento.Text = elemento.DataInserimento;

            this.UpdateImageState(elemento);

            if (!string.IsNullOrEmpty(elemento.Note))
            {
                this.lblNote.Text = elemento.Note;
                this.lblNote.ToolTip = elemento.Note;
                this.pnlNote.Attributes.Add("style", "display:block");
            }
            else
            {
                this.pnlNote.Attributes.Add("style", "display:none");
            }

            this.trvDettagliFirma.Nodes.Clear();

            BuildTreeView(schedaDocumentoSelezionato);

            this.TreeSignatureProcess.Nodes.Clear();
            if (elemento.Modalita.Equals(LibroFirmaManager.Modalita.AUTOMATICA))
            {
                List<IstanzaProcessoDiFirma> istanzaProcesso = LibroFirmaManager.GetIstanzaProcessoDiFirma(elemento.InfoDocumento.Docnumber);
                //Estraggo dalla lista il processo in esecuzione
                IstanzaProcessoDiFirma istanza = (from i in istanzaProcesso
                                                  where string.IsNullOrEmpty(i.dataChiusura)
                                                  select i).FirstOrDefault();
                if (istanza != null)
                {
                    this.pnlProponente.Visible = true;
                    this.lblProponente.Text = istanza.UtenteProponente.descrizione + " (" + istanza.RuoloProponente.descrizione + ")";
                    if (!string.IsNullOrEmpty(istanza.DescUtenteDelegato))
                    {
                        string language = UIManager.UserManager.GetUserLanguage();
                        string del = Utils.Languages.GetLabelFromCode("TransmissionDelegatedBy", language).ToUpper();

                        this.lblProponente.Text = istanza.DescUtenteDelegato + " (" + istanza.RuoloProponente.descrizione + ")";
                        this.lblProponente.Text += " " + del + " " + istanza.UtenteProponente.descrizione;
                    }
                    else
                    {
                        this.lblProponente.Text = istanza.UtenteProponente.descrizione + " (" + istanza.RuoloProponente.descrizione + ")";
                    }
                    this.lblProponente.ToolTip = this.lblProponente.Text;
                    BindTreeViewStateSignatureProcess(istanza);
                }

                this.pnlDetailsSignatureProcess.Attributes.Add("style", "display:block");
            }
            else
            {
                this.pnlProponente.Visible = false;
                this.pnlDetailsSignatureProcess.Attributes.Add("style", "display:none");
            }
            if (elemento.TipoFirma.Equals(LibroFirmaManager.TypeEvent.ADVANCEMENT_PROCESS))
            {
                this.EsaminaLFSelezionaPerFirma.Text = Utils.Languages.GetLabelFromCode("EsaminaLFSelezionaPerProcedi", UserManager.GetUserLanguage());
            }
            else
            {
                this.EsaminaLFSelezionaPerFirma.Text = Utils.Languages.GetLabelFromCode("EsaminaLFSelezionaPerFirma", UserManager.GetUserLanguage());
            }
            this.ShowDocumentFile(this.chk_showDoc.Checked);


            RefreshElementCounter();
            EnabledNavigationButton();
            EnableButton();
            EnabledButtonEsamina(true);

            this.upPnlInfoDoc.Update();
            this.upPnlInfoElementoLF.Update();
            this.upPnlNavigationButtons.Update();
            this.UpPnlDettagliFirma.Update();
            this.UpPnlDetailsSignatureProcess.Update();
        }

        private void BindTreeViewStateSignatureProcess(IstanzaProcessoDiFirma istanzaProcessoDiFirma)
        {
            try
            {
                if (istanzaProcessoDiFirma != null)
                {
                    TreeNode root = new TreeNode();
                    root.Text = istanzaProcessoDiFirma.Descrizione;
                    root.Value = istanzaProcessoDiFirma.idIstanzaProcesso;
                    root.ToolTip = istanzaProcessoDiFirma.Descrizione;
                    root.SelectAction = TreeNodeSelectAction.None;
                    foreach (IstanzaPassoDiFirma passo in istanzaProcessoDiFirma.istanzePassoDiFirma)
                    {
                        this.AddChildrenElements(passo, ref root, false);
                    }

                    this.TreeSignatureProcess.Nodes.Add(root);
                    this.TreeSignatureProcess.DataBind();
                }
            }
            catch (Exception e)
            {
                string msg = "ErrorDetailsAutomaticMode";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                return;
            }
        }

        private TreeNode AddChildrenElements(IstanzaPassoDiFirma p, ref TreeNode root, bool isConcluted)
        {
            TreeNode nodeChild = new TreeNode();

            nodeChild.ImageUrl = LibroFirmaManager.GetIconEventType(p);
            nodeChild.Value = p.idPasso;
            nodeChild.Text = LibroFirmaManager.GetHolder(p);
            nodeChild.ToolTip = LibroFirmaManager.GetHolder(p);
            nodeChild.SelectAction = TreeNodeSelectAction.None;

            if (!string.IsNullOrEmpty(p.Note))
            {
                TreeNode nodeChildNote = new TreeNode();
                nodeChildNote.Text = p.Note;
                nodeChildNote.ToolTip = p.Note;
                nodeChildNote.SelectAction = TreeNodeSelectAction.None;
                nodeChild.ChildNodes.Add(nodeChildNote);
            }

            if (!string.IsNullOrEmpty(p.dataEsecuzione))
            {
                TreeNode nodeChildDateExecution = new TreeNode();
                string action = p.statoPasso.Equals(TipoStatoPasso.CLOSE) ? Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeEseguitoIl", UserManager.GetUserLanguage()) :
                    Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeInterrottoIl", UserManager.GetUserLanguage());
                nodeChildDateExecution.Text = action + " " + p.dataEsecuzione;
                nodeChildDateExecution.SelectAction = TreeNodeSelectAction.None;
                nodeChild.ChildNodes.Add(nodeChildDateExecution);
            }

            root.ChildNodes.Add(nodeChild);
            if (p.statoPasso.Equals(TipoStatoPasso.LOOK) && !isConcluted)
            {
                nodeChild.Select();
            }

            return nodeChild;
        }

        protected void TreeSignatureProcess_Collapsed(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

        }

        public void EnableButton()
        {
            ElementoInLibroFirma elemento = this.ListaElementiSelezionati[this.ElementoSelezionato];
            if (elemento.Modalita.Equals(LibroFirmaManager.Modalita.AUTOMATICA))
            {
                this.EsaminaLFNonDiCompetenza.Visible = true;

                if (!string.IsNullOrEmpty(elemento.DataAccettazione))//LA TRASMISSIONE è STATA GIà ACCETTATA
                {
                    this.EsaminaLFNonDiCompetenza.Visible = false;
                }
            }
            else if (elemento.Modalita.Equals(LibroFirmaManager.Modalita.MANUALE))
            {
                this.EsaminaLFNonDiCompetenza.Visible = false;
            }
            this.UpPnlButtons.Update();
        }

        /// <summary>
        /// Seleziona l'url dell'icona in base al tipo di firma(digitale/elettronica)
        /// </summary>
        /// <param name="passo"></param>
        /// <returns></returns>
        protected string GetIconTypeSignature(ElementoInLibroFirma elemento)
        {
            string url = string.Empty;

            url = !string.IsNullOrEmpty(elemento.IdUtenteTitolare) ? "../Images/Icons/LibroFirma/" + elemento.TipoFirma.ToLower() + "_user.png" : "../Images/Icons/LibroFirma/" + elemento.TipoFirma.ToLower() + "_role.png";

            return url;
        }

        protected void chk_showDoc_CheckedChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            this.IsZoom = false;

            this.ShowDocumentFile(this.chk_showDoc.Checked);

            this.UpPnlViewer.Update();
        }


        /// <summary>
        /// Visualizzazione file
        /// </summary>
        private void ShowDocumentFile(bool autoPreview)
        {
            this.IsZoom = false;
            this.ViewDocument.ShowDocumentAcquired(autoPreview);

            this.UpPnlViewer.Update();
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        #endregion

        #region Event

        protected void EsaminaLFSelezionaPerFirma_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                ElementoInLibroFirma elemento = this.ListaElementiSelezionati[ElementoSelezionato];

                //Controllo che non venga superato il numero massimo di elementi selezionabili
                if (elemento.StatoFirma == TipoStatoElemento.PROPOSTO &&
                    (from i in this.ListaElementiLibroFirma where i.StatoFirma != TipoStatoElemento.PROPOSTO select i).ToList().Count >= this.MaxSelLibroFirma)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningMaxSelectedItemInLF', 'warning', '', '" + this.MaxSelLibroFirma + "');} else {parent.ajaxDialogModal('WarningMaxSelectedItemInLF', 'warning', '', '" + this.MaxSelLibroFirma + "');}", true);
                    return;
                }

                //Controllo che non sia stata superata la dimensione massima elaborabile
                if ((elemento.StatoFirma.Equals(TipoStatoElemento.PROPOSTO) || elemento.StatoFirma == TipoStatoElemento.DA_RESPINGERE) && (this.totalFileSize + elemento.FileSize) > this.maxFileSizeSelectable)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningMaxFileSizeSelectedItemInLF', 'warning', '', '" + (this.maxFileSizeSelectable / 1000000) + "');} else {parent.ajaxDialogModal('WarningMaxFileSizeSelectedItemInLF', 'warning', '', '" + (this.maxFileSizeSelectable / 1000000) + "');}", true);
                    return;
                }
                else
                {
                    this.totalFileSize += elemento.FileSize;
                }

                this.txtMotivoRespingimento.Text = string.Empty;
                this.pnlMotivoRespingimento.Attributes.Add("style", "display:none");
                UpdateStateElement(TipoStatoElemento.DA_FIRMARE);
                if (this.EsaminaLFNonDiCompetenza.Visible)
                {
                    this.EsaminaLFNonDiCompetenza.Visible = false;
                    this.UpPnlButtons.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void EsaminaLFSelezionaPerRespingimento_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                ElementoInLibroFirma elemento = this.ListaElementiSelezionati[ElementoSelezionato];
                if (elemento.StatoFirma == TipoStatoElemento.PROPOSTO &&
                    (from i in this.ListaElementiLibroFirma where i.StatoFirma != TipoStatoElemento.PROPOSTO select i).ToList().Count >= this.MaxSelLibroFirma)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningMaxSelectedItemInLF', 'warning', '', '" + this.MaxSelLibroFirma + "');} else {parent.ajaxDialogModal('WarningMaxSelectedItemInLF', 'warning', '', '" + this.MaxSelLibroFirma + "');}", true);
                    return;
                }
                if (elemento.StatoFirma == TipoStatoElemento.DA_FIRMARE)
                {
                    this.totalFileSize += elemento.FileSize;
                }

                HttpContext.Current.Session["MotivoRespingimento"] = this.txtMotivoRespingimento.Text;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "InterruptionSignatureProcess", "ajaxModalPopupInterruptionSignatureProcess();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void EsaminaLFDeseleziona_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                this.txtMotivoRespingimento.Text = string.Empty;
                this.pnlMotivoRespingimento.Attributes.Add("style", "display:none");
                UpdateStateElement(TipoStatoElemento.PROPOSTO);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        /// <summary>
        /// Aggiorna lo stato dell'elemento in libro firma
        /// </summary>
        /// <param name="newState"></param>
        private void UpdateStateElement(TipoStatoElemento newState)
        {
            ElementoInLibroFirma elemento = this.ListaElementiSelezionati[ElementoSelezionato];

            //Controllo che non sia stato superato il numero massimo di elementi selezionabili
            if ((newState == TipoStatoElemento.DA_FIRMARE || newState == TipoStatoElemento.DA_RESPINGERE)
                && elemento.StatoFirma.Equals(TipoStatoElemento.PROPOSTO)
                && (from i in this.ListaElementiLibroFirma where i.StatoFirma != TipoStatoElemento.PROPOSTO select i).ToList().Count >= this.MaxSelLibroFirma)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningMaxSelectedItemInLF', 'warning', '', '" + this.MaxSelLibroFirma + "');} else {parent.ajaxDialogModal('WarningMaxSelectedItemInLF', 'warning', '', '" + this.MaxSelLibroFirma + "');}", true);
                return;
            }

            //Controllo che non sia stata superata la dimensione massima elaborabile
            if ((newState == TipoStatoElemento.DA_FIRMARE) && (elemento.StatoFirma.Equals(TipoStatoElemento.PROPOSTO) || elemento.StatoFirma == TipoStatoElemento.DA_RESPINGERE)
                && (this.totalFileSize + elemento.FileSize) > this.maxFileSizeSelectable)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningMaxFileSizeSelectedItemInLF', 'warning', '', '" + (this.maxFileSizeSelectable / 1000000) + "');} else {parent.ajaxDialogModal('WarningMaxFileSizeSelectedItemInLF', 'warning', '', '" + (this.maxFileSizeSelectable / 1000000) + "');}", true);
                return;
            }
            else
            {
                //Aggiorno la dimensione totale dei file
                this.totalFileSize += elemento.FileSize;
            }

            //Se stò passando dallo stato "da firmare" a "proposto" o "da respingere" aggiorno la dimensione totale di file decrementandola
            if (elemento.StatoFirma == TipoStatoElemento.DA_FIRMARE && (newState == TipoStatoElemento.DA_RESPINGERE || newState == TipoStatoElemento.PROPOSTO))
            {
                this.totalFileSize -= elemento.FileSize;
            }

            string message = string.Empty;
            string msg = string.Empty;
            elemento.MotivoRespingimento = this.txtMotivoRespingimento.Text;
            if (LibroFirmaManager.AggiornaStatoElemento(elemento, newState.ToString(), out message))
            {

                if (string.IsNullOrEmpty(message))
                {
                    elemento.StatoFirma = newState;

                    if (string.IsNullOrEmpty(elemento.DataAccettazione) && newState.Equals(TipoStatoElemento.DA_FIRMARE))
                    {
                        elemento.DataAccettazione = Utils.dateformat.getDataOdiernaDocspa();
                        ListaElementiLibroFirma.Where(i => i.IdTrasmSingola.Equals(elemento.IdTrasmSingola)).ToList().ForEach(f => f.DataAccettazione = elemento.DataAccettazione);
                        ListaElementiLibroFirma.Where(i => i.IdTrasmSingola.Equals(elemento.IdTrasmSingola)).ToList().ForEach(f => f.StatoFirma = elemento.StatoFirma);
                    }

                    this.UpdateImageState(elemento);

                    this.upPnlImgState.Update();
                }
                else
                {
                    msg = RemoveElementsLockedAnotherUser(message);
                    if (this.ListaElementiSelezionati.Count > 0)
                    {
                        PopolaCampi();
                    }
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RemoveElementsLockedAnotherUser', 'warning', '', '" + msg + "');} else {parent.ajaxDialogModal('RemoveElementsLockedAnotherUser', 'warning', '', '" + msg + "');}", true);
                }

                this.upPnlInfoElementoLF.Update();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorUpdateStateElementLF', 'error', '', '" + msg + "');} else {parent.ajaxDialogModal('ErrorUpdateStateElementLF', 'error', '', '" + msg + "');}", true);
            }
        }

        private string RemoveElementsLockedAnotherUser(string message)
        {
            string infoElement;
            string msg = string.Empty;
            string[] elements = message.Split('#');
            List<string> idElements = new List<string>();
            foreach (string element in elements)
            {
                if (!string.IsNullOrEmpty(element))
                {
                    string[] info = element.Split('@');
                    idElements.Add(info[0]);
                    infoElement = info[1] + " (" + info[2] + ")";
                    msg += infoElement + "<br />";
                }
            }
            this.ListaElementiSelezionati = (from e in this.ListaElementiSelezionati where !idElements.Contains(e.IdElemento) select e).ToList();
            return msg;
        }

        protected void EsaminaLFNonDiCompetenza_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                ElementoInLibroFirma elemento = this.ListaElementiSelezionati[ElementoSelezionato];
                string error = string.Empty;
                if (LibroFirmaManager.InterruptionSignatureProcessByHolder(elemento, out error))
                {
                    //Rimuovo l'elemento
                    UpdateListElements(elemento);

                    if (this.ListaElementiSelezionati != null && this.ListaElementiSelezionati.Count > 0)
                    {
                        if ((this.ElementoSelezionato == this.ListaElementiSelezionati.Count))
                            this.ElementoSelezionato = 0;
                        PopolaCampi();
                    }
                    else
                    {
                        //Chiudo la popup di esamina in libro firma
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('EsaminaLibroFirmaNoItem', 'warning', '', '');} else {parent.ajaxDialogModal('EsaminaLibroFirmaNoItem', 'warning', '', '');}", true);
                        if (EsaminaSingolo)
                            ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('EsaminaLibroFirmaSingolo','');", true);
                        else
                            ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('EsaminaLibroFirma','');", true);
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void EsaminaLFChiudi_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                if (EsaminaSingolo)
                    ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('EsaminaLibroFirmaSingolo','');", true);
                else
                    ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('EsaminaLibroFirma','');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void BtnStato_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            ElementoInLibroFirma elemento = this.ListaElementiSelezionati[ElementoSelezionato];
            switch (elemento.StatoFirma)
            {
                case TipoStatoElemento.PROPOSTO:
                    this.txtMotivoRespingimento.Text = string.Empty;
                    this.pnlMotivoRespingimento.Attributes.Add("style", "display:none");
                    UpdateStateElement(TipoStatoElemento.DA_FIRMARE);
                    if (this.EsaminaLFNonDiCompetenza.Visible)
                    {
                        this.EsaminaLFNonDiCompetenza.Visible = false;
                        this.UpPnlButtons.Update();
                    }
                    break;
                case TipoStatoElemento.DA_FIRMARE:
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "InterruptionSignatureProcess", "ajaxModalPopupInterruptionSignatureProcess();", true);
                    break;
                case TipoStatoElemento.DA_RESPINGERE:
                    this.txtMotivoRespingimento.Text = string.Empty;
                    this.pnlMotivoRespingimento.Attributes.Add("style", "display:none");
                    UpdateStateElement(TipoStatoElemento.PROPOSTO);
                    break;
            }
        }

        protected void btnModifyMotivoRespingimento_Click(object sender, EventArgs e)
        {
            HttpContext.Current.Session["MotivoRespingimento"] = this.txtMotivoRespingimento.Text;
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "InterruptionSignatureProcess", "ajaxModalPopupInterruptionSignatureProcess();", true);
        }

        #region Navigation

        protected void btn_first_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                this.ElementoSelezionato = 0;
                PopolaCampi();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void btn_previous_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                this.ElementoSelezionato = this.ElementoSelezionato - 1;
                PopolaCampi();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void btn_next_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                this.ElementoSelezionato = this.ElementoSelezionato + 1;
                PopolaCampi();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void btn_last_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                this.ElementoSelezionato = this.ListaElementiSelezionati.Count() - 1;
                PopolaCampi();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #endregion

        #endregion

        #region Utils

        private void UpdateListElements(ElementoInLibroFirma elemento)
        {
            //VADO A RIMUOVERE TUTTI GLI ELEMENTI IN LIBRO FIRMA LEGATI ALLA STESSA TRASMISSIONE(CASO DI DOCUMENTI E/O SUOI ALLEGATI)
            this.ListaElementiSelezionati = (from i in this.ListaElementiSelezionati
                                             where !i.IdTrasmSingola.Equals(elemento.IdTrasmSingola)
                                             select i).ToList();

            this.ListaElementiLibroFirma = (from i in this.ListaElementiLibroFirma
                                            where !i.IdTrasmSingola.Equals(elemento.IdTrasmSingola)
                                            select i).ToList();

            this.ListaElementiFiltrati = (from i in this.ListaElementiFiltrati
                                          where !i.IdTrasmSingola.Equals(elemento.IdTrasmSingola)
                                          select i).ToList();
        }

        private void RefreshElementCounter()
        {
            lbl_contatore.Text = ElementoSelezionato + 1 + " / " + this.ListaElementiSelezionati.Count();
            this.upPnlNavigationButtons.Update();
        }

        private void EnabledNavigationButton()
        {
            this.btn_first.Enabled = true;
            this.btn_previous.Enabled = true;
            this.btn_next.Enabled = true;
            this.btn_last.Enabled = true;
            if (this.ElementoSelezionato == 0)
            {
                this.btn_first.Enabled = false;
                this.btn_previous.Enabled = false;
            }
            if (this.ElementoSelezionato == this.ListaElementiSelezionati.Count - 1)
            {
                this.btn_next.Enabled = false;
                this.btn_last.Enabled = false;
            }
        }

        private void UpdateImageState(ElementoInLibroFirma elemento)
        {
            switch (elemento.StatoFirma)
            {
                case TipoStatoElemento.PROPOSTO:
                    (this.imgStato as CustomImageButton).ImageUrl = "../Images/Icons/LibroFirma/Sel_None.png";
                    (this.imgStato as CustomImageButton).ImageUrlDisabled = "../Images/Icons/LibroFirma/Sel_None_disabled.png";
                    (this.imgStato as CustomImageButton).OnMouseOutImage = "../Images/Icons/LibroFirma/Sel_None.png";
                    (this.imgStato as CustomImageButton).OnMouseOverImage = "../Images/Icons/LibroFirma/Sel_None_hover.png";
                    (this.imgStato as CustomImageButton).ToolTip = Utils.Languages.GetLabelFromCode("LibroFirmaProposto", UserManager.GetUserLanguage());
                    break;
                case TipoStatoElemento.DA_FIRMARE:
                    (this.imgStato as CustomImageButton).ImageUrl = "../Images/Icons/LibroFirma/Sel_Firma.png";
                    (this.imgStato as CustomImageButton).ImageUrlDisabled = "../Images/Icons/LibroFirma/Sel_Firma_disabled.png";
                    (this.imgStato as CustomImageButton).OnMouseOutImage = "../Images/Icons/LibroFirma/Sel_Firma.png";
                    (this.imgStato as CustomImageButton).OnMouseOverImage = "../Images/Icons/LibroFirma/Sel_Firma_hover.png";

                    (this.imgStato as Image).ToolTip = Utils.Languages.GetLabelFromCode("LibroFirmaToSisgn", UserManager.GetUserLanguage());
                    break;
                case TipoStatoElemento.DA_RESPINGERE:
                    (this.imgStato as CustomImageButton).ImageUrl = "../Images/Icons/LibroFirma/Sel_Rifiuta.png";
                    (this.imgStato as CustomImageButton).ImageUrlDisabled = "../Images/Icons/LibroFirma/Sel_Rifiuta_disabled.png";
                    (this.imgStato as CustomImageButton).OnMouseOutImage = "../Images/Icons/LibroFirma/Sel_Rifiuta.png";
                    (this.imgStato as CustomImageButton).OnMouseOverImage = "../Images/Icons/LibroFirma/Sel_Rifiuta_hover.png";
                    (this.imgStato as CustomImageButton).ToolTip = Utils.Languages.GetLabelFromCode("LibroFirmaToReject", UserManager.GetUserLanguage());
                    break;
            }
        }

        #endregion

        #region TreeView

        private void BuildTreeView(SchedaDocumento schedaDocumento)
        {
            this.trvDettagliFirma.Nodes.Clear();
            if (schedaDocumento.documenti[0].firmato == "1")
            {
                this.pnlInfoFirma.Attributes.Add("style", "display:block");
                TreeNode nodeDigitalSignature = BuildNodesDigitalSignature();
                if (nodeDigitalSignature != null)
                    this.trvDettagliFirma.Nodes.Add(nodeDigitalSignature);


                ElementoInLibroFirma elemento = this.ListaElementiSelezionati[this.ElementoSelezionato];
                List<DocsPaWR.FirmaElettronica> listElectronicSignature = DocumentManager.GetElectronicSignatureDocument(elemento.InfoDocumento.Docnumber, elemento.InfoDocumento.VersionId);
                if (listElectronicSignature != null && listElectronicSignature.Count > 0)
                {
                    TreeNode nodeElectronicSignature = BuildNodesElectronicSignature(listElectronicSignature);
                    if (nodeElectronicSignature != null)
                        this.trvDettagliFirma.Nodes.Add(BuildNodesElectronicSignature(listElectronicSignature));
                }
            }
            else
            {
                this.pnlInfoFirma.Attributes.Add("style", "display:none");
            }

            this.trvDettagliFirma.ExpandAll();
        }

        private TreeNode BuildNodesDigitalSignature()
        {
            TreeNode nodesDigitalSignature = null;
            FileDocumento signedDocument = this.GetSignedDocument();
            if (signedDocument != null && signedDocument.signatureResult != null)
            {
                VerifySignatureResult signatureResult = signedDocument.signatureResult;
                int documentIndex = 0;
                TreeNode documentNode = null;
                // Aggiunta del nodo relativo al file originale
                nodesDigitalSignature = new TreeNode() { Text = Languages.GetLabelFromCode("EsaminaLibroFirmaNodesDigitalSignature", UserManager.GetUserLanguage()) + " (" + signatureResult.PKCS7Documents.Count().ToString() + ")" };

                foreach (PKCS7Document document in signatureResult.PKCS7Documents)
                {
                    // Aggiunta del nodo solo se il documento è p7m
                    documentNode = this.GetNodesFirmeDigitali(document, documentIndex, null);
                    nodesDigitalSignature.ChildNodes.Add(documentNode);
                    documentIndex++;
                }
            }
            return nodesDigitalSignature;
        }

        private TreeNode BuildNodesElectronicSignature(List<DocsPaWR.FirmaElettronica> listElectronicSignature)
        {
            TreeNode nodesElectronicSignature = null;
            TreeNode nodeDateAffixing = null;
            string lblDateAffixing = Utils.Languages.GetLabelFromCode("EsaminaLblDateAffixing", UserManager.GetUserLanguage());
            nodesElectronicSignature = new TreeNode() { Text = Languages.GetLabelFromCode("EsaminaLibroFirmaNodesElectronicSignature", UserManager.GetUserLanguage()) + " (" + listElectronicSignature.Count().ToString() + ")" };
            TreeNode parentNode = null;
            foreach (FirmaElettronica e in listElectronicSignature)
            {
                parentNode = new TreeNode() { Text = e.Firmatario };// + e.DateAffixing.ToString() };
                nodeDateAffixing = new TreeNode() { Text = lblDateAffixing + " " + e.DataApposizione.ToString() };
                parentNode.ChildNodes.Add(nodeDateAffixing);
                nodesElectronicSignature.ChildNodes.Add(parentNode);
            }
            return nodesElectronicSignature;
        }

        protected void trvDettagliFirma_Collapsed(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
        }

        protected void trvDettagliFirma_SelectedNodeChanged(object sender, EventArgs e)
        {
            this.UpPnlDettagliFirma.Update();
        }

        private FileDocumento GetSignedDocument()
        {
            DocsPaWR.FileRequest fileRequest = DocumentManager.getSelectedRecord().documenti[0];
            DocsPaWR.FileDocumento signedDocument = null;
            if (fileRequest != null && fileRequest.fileName != null && fileRequest.fileName != "")
            {
                signedDocument = this.DocumentoGetFileCached(fileRequest);
            }

            return signedDocument;
        }

        //meccanismo di caching per evitare di fare la getFile tutte le volte, con conseguente controllo del certificato
        private FileDocumento DocumentoGetFileCached(DocsPaWR.FileRequest fileRequest)
        {
            FileDocumento retval = null;
            if (HttpContext.Current.Session["FileRequest_Cached"] == null)
                HttpContext.Current.Session["FileDocumento_Cached"] = null;

            if (HttpContext.Current.Session["FileDocumento_Cached"] == null)
            {
                retval = DocumentManager.DocumentoGetFile(fileRequest);
                HttpContext.Current.Session["FileRequest_Cached"] = fileRequest;
                HttpContext.Current.Session["FileDocumento_Cached"] = retval;
            }
            else
            {
                if (HttpContext.Current.Session["FileRequest_Cached"] != fileRequest)
                {
                    //filerequest è cambiato
                    retval = DocumentManager.DocumentoGetFile(fileRequest);
                    HttpContext.Current.Session["FileRequest_Cached"] = fileRequest;
                    HttpContext.Current.Session["FileDocumento_Cached"] = retval;

                }
                else
                {
                    retval = (FileDocumento)HttpContext.Current.Session["FileDocumento_Cached"];
                }
            }
            return retval;
        }


        private TreeNode GetNodeDocumentoOriginale(FileDocumento originalDocument)
        {
            bool isSignedDocument = (originalDocument.signatureResult != null);

            TreeNode node = new TreeNode();
            node.Value = "originalDocument";
            node.Target = "right";
            node.Text = this.GetLabel("DigitalSignDetailsOriginalDoc");

            string documentFileName = string.Empty;
            if (isSignedDocument)
                documentFileName = originalDocument.signatureResult.FinalDocumentName;
            else
                documentFileName = originalDocument.name;

            System.IO.FileInfo info = new System.IO.FileInfo(documentFileName);
            string extFileOrignale = info.Extension;
            if (extFileOrignale != "")
                extFileOrignale = extFileOrignale.Replace(".", "").ToLower();
            info = null;

            string imageUrl = ResolveUrl(FileManager.getFileIcon(this, extFileOrignale));
            node.ImageUrl = imageUrl.Replace("icon_", "small_");

            return node;
        }

        private TreeNode GetNodesFirmeElettroniche()
        {
            TreeNode rootNode = new TreeNode();

            return rootNode;
        }

        private TreeNode GetNodesFirmeDigitali(PKCS7Document document, int documentIndex, string signerLevel)
        {
            TreeNode rootNode = new TreeNode();
            rootNode.Text = this.GetLabel("DigitalSignDetailsSigns") + " (" + document.SignersInfo.Length + ")";
            rootNode.Value = string.Empty;

            string imageUrl = ResolveUrl(FileManager.getFileIcon(this, "p7m"));
            if (document.SignatureType == SignType.PADES)
                imageUrl = ResolveUrl(FileManager.getFileIcon(this, "pdf"));
            rootNode.ImageUrl = imageUrl.Replace("icon_", "small_");

            int index = 0;

            foreach (SignerInfo info in document.SignersInfo)
            {
                TreeNode signNode = new TreeNode();
                signNode.Value = "sign&documentIndex=" + documentIndex.ToString() + "&index=" + index.ToString() + signerLevel;

                string nodeText = info.SubjectInfo.Cognome + " " + info.SubjectInfo.Nome + " " + document.SignatureType.ToString();
                if (nodeText.Trim() == string.Empty)
                    nodeText = this.GetLabel("DigitalSignDetailsNotAvailable");

                signNode.Text = nodeText;

                if (info.SignatureTimeStampInfo != null)
                {
                    foreach (TSInfo ts in info.SignatureTimeStampInfo)
                    {
                        TreeNode tsNode = new TreeNode();
                        tsNode.Value = "timestamp&documentIndex=" + documentIndex.ToString() + "&index=" + index.ToString() + signerLevel;
                        string tsText = this.GetLabel("DigitalSignDetailsTimestamp");
                        tsNode.Text = tsText;
                        tsNode.ImageUrl = "../Images/Icons/small_timestamp.png";
                        signNode.ChildNodes.Add(tsNode);
                    }
                }

                if (info.counterSignatures != null)
                {
                    int signLevels = 0;
                    foreach (SignerInfo csigner in info.counterSignatures)
                    {
                        List<SignerInfo> tmpLst = new List<SignerInfo>();
                        tmpLst.Add(csigner);
                        signerLevel = ":" + signLevels.ToString();
                        signNode.ChildNodes.Add(this.GetNodesFirmeDigitali(tmpLst.ToArray(), documentIndex, signerLevel));
                        signLevels++;
                    }
                }

                rootNode.ChildNodes.Add(signNode);
                signNode = null;

                index++;
            }

            return rootNode;
        }

        private TreeNode GetNodesFirmeDigitali(SignerInfo[] signersInfo, int documentIndex, string signerLevel)
        {
            TreeNode rootNode = new TreeNode();
            rootNode.Text = this.GetLabel("DigitalSignDetailsSigns") + " (" + signersInfo.Length + ")";
            int index = 0;

            foreach (SignerInfo info in signersInfo)
            {
                TreeNode signNode = new TreeNode();
                signNode.Value = "sign&documentIndex=" + documentIndex.ToString() + "&index=" + index.ToString() + signerLevel;

                string nodeText = info.SubjectInfo.Cognome + " " + info.SubjectInfo.Nome;
                if (nodeText.Trim() == string.Empty)
                    nodeText = this.GetLabel("DigitalSignDetailsNotAvailable");

                signNode.Text = nodeText;

                if (info.SignatureTimeStampInfo != null)
                {
                    foreach (TSInfo ts in info.SignatureTimeStampInfo)
                    {
                        TreeNode tsNode = new TreeNode();
                        tsNode.Value = "timestamp&documentIndex=" + documentIndex.ToString() + "&index=" + index.ToString() + signerLevel;
                        string tsText = this.GetLabel("DigitalSignDetailsTimestamp");
                        tsNode.Text = tsText;
                        tsNode.ImageUrl = "../Images/Icons/small_timestamp.png";
                        signNode.ChildNodes.Add(tsNode);
                    }
                }

                if (info.counterSignatures != null)
                {
                    int signLevels = 0;
                    foreach (SignerInfo csigner in info.counterSignatures)
                    {
                        List<SignerInfo> tmpLst = new List<SignerInfo>();
                        tmpLst.Add(csigner);
                        signerLevel = ":" + signLevels.ToString();
                        signNode.ChildNodes.Add(GetNodesFirmeDigitali(tmpLst.ToArray(), documentIndex, signerLevel));
                        signLevels++;
                    }
                }

                rootNode.ChildNodes.Add(signNode);
                signNode = null;

                index++;
            }

            return rootNode;
        }

        #endregion

        public void EnabledButtonEsamina(bool enable)
        {
            this.EsaminaLFSelezionaPerFirma.Enabled = enable;
            this.EsaminaLFNonDiCompetenza.Enabled = enable;
            this.EsaminaLFDeseleziona.Enabled = enable;
            this.EsaminaLFSelezionaPerRespingimento.Enabled = enable;
            this.imgStato.Enabled = enable;

            this.upPnlImgState.Update();
            this.UpPnlButtons.Update();
        }
    }
}
