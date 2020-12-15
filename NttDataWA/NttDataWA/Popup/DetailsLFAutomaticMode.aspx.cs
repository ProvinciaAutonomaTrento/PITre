using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;
using NttDataWA.UserControls;

namespace NttDataWA.Popup
{
    public partial class DetailsLFAutomaticMode : System.Web.UI.Page
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

        private List<IstanzaProcessoDiFirma> ListaIstanzaProcessoDiFirma
        {
            get
            {
                if (HttpContext.Current.Session["ListaIstanzaProcessoDiFirma"] != null)
                    return (List<IstanzaProcessoDiFirma>)HttpContext.Current.Session["ListaIstanzaProcessoDiFirma"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["ListaIstanzaProcessoDiFirma"] = value;
            }
        }

        private IstanzaProcessoDiFirma IstanzaProcessoDiFirma
        {
            get
            {
                if (HttpContext.Current.Session["IstanzaProcessoDiFirma"] != null)
                    return (IstanzaProcessoDiFirma)HttpContext.Current.Session["IstanzaProcessoDiFirma"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["IstanzaProcessoDiFirma"] = value;
            }
        }

        #endregion

        #region Constant

        private const char PROPONENTE = 'P';
        private const char TITOLARE = 'T';
        private const char AMMINISTRATORE = 'A';

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

        protected void ReadRetValueFromPopup()
        {
            if (!string.IsNullOrEmpty(this.InterruptionSignatureProcess.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('InterruptionSignatureProcess','');", true);
                GoToTabConclutedLnk();
                if (Request.QueryString["caller"] == null)
                {
                    if (UIManager.DocumentManager.getSelectedAttachId() != null)
                    {
                        FileManager.GetFileRequest(UIManager.DocumentManager.getSelectedAttachId()).inLibroFirma = false;
                    }
                    else
                    {
                        FileManager.GetFileRequest().inLibroFirma = false;
                    }
                }
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('SuccessInterruptionSignatureProcess', 'check', '');} else {parent.ajaxDialogModal('SuccessInterruptionSignatureProcess', 'check', '');}", true);
            }
        }
        private void InitializeLanguage()
        {
            string language = UserManager.GetUserLanguage();
            this.DetailsLFAutomaticModeClose.Text = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeClose", language);
            this.LtlNameSignatureProcess.Text = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeLtlNameSignatureProcess", language);
            this.LtlProponente.Text = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeLtlProponente", language);
            this.LtlAvviatoIl.Text = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeLtlAvviatoIl", language);
            this.DetailsLFAutomaticModeInterruption.Text = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeInterruption", language);
            this.LtlNoteAvvio.Text = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeLtlNoteAvvio", language);
            this.ltlNotificationOption.Text = Utils.Languages.GetLabelFromCode("StartProcessSignatureltlNotificationOption", language);
            this.DetailsLFAutomaticModeModify.Text = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeModify", language);
            this.InExecuteLinkList.Text = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeInExecuteLinkList", language);
            this.InConcludedLinkList.Text = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeInConcludedLinkList", language);
            this.InterruptionSignatureProcess.Title = Utils.Languages.GetLabelFromCode("InterruptionSignatureProcessTitle", language);
            this.LtlMotivoInterruzione.Text = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeLtlMotivoInterruzione", language);
        }

        private void EnableContentProponent(IstanzaProcessoDiFirma istanzaProcessoDiFirma)
        {
            if (istanzaProcessoDiFirma.RuoloProponente.idGruppo.Equals(RoleManager.GetRoleInSession().idGruppo))
            {
                this.DetailsLFAutomaticModeInterruption.Visible = true;
                this.DetailsLFAutomaticModeModify.Visible = true;
                this.pnlProponente.Attributes.Add("style", "display:block");
                this.BuildOpzioniNotifiche(istanzaProcessoDiFirma);
            }
            else
            {
                this.DetailsLFAutomaticModeInterruption.Visible = false;
                this.DetailsLFAutomaticModeModify.Visible = false;
                this.pnlProponente.Attributes.Add("style", "display:none");
            }

            this.UpPnlButtons.Update();
        }

        private void InitializePage()
        {
            string docnumber = string.Empty;
            if (Request.QueryString["caller"] != null)
            {
                docnumber = ListaElementiSelezionati[0].InfoDocumento.Docnumber;
            }
            else
            {
                DocsPaWR.FileRequest fileReq = null;

                if (FileManager.GetSelectedAttachment() == null)
                    fileReq = UIManager.FileManager.getSelectedFile();
                else
                {
                    fileReq = FileManager.GetSelectedAttachment();
                }

                docnumber = fileReq.docNumber;
            }
            this.ListaIstanzaProcessoDiFirma = LibroFirmaManager.GetIstanzaProcessoDiFirma(docnumber);

            //Estraggo dalla lista il processo in esecuzione
            IstanzaProcessoDiFirma istanza = (from i in ListaIstanzaProcessoDiFirma
                                              where string.IsNullOrEmpty(i.dataChiusura)
                                              select i).FirstOrDefault();
            if (istanza != null)
            {
                PopolaDettalioIstanza(istanza);
                BindTreeViewStateSignatureProcess(istanza);
                EnableContentProponent(istanza);
            }
            else
            {
                this.pnlDetailsSignatureProcess.Visible = false;
                this.DetailsLFAutomaticModeInterruption.Visible = false;
                this.DetailsLFAutomaticModeModify.Visible = false;
            }
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
        }

        private void PopolaDettalioIstanza(IstanzaProcessoDiFirma istanzaProcessoDiFirma)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            string del = Utils.Languages.GetLabelFromCode("TransmissionDelegatedBy", language).ToUpper();

            this.lblNameSignatureProcess.Text = istanzaProcessoDiFirma.Descrizione;
            if (!string.IsNullOrEmpty(istanzaProcessoDiFirma.DescUtenteDelegato))
            {
                this.lblProponente.Text = istanzaProcessoDiFirma.DescUtenteDelegato + " (" + istanzaProcessoDiFirma.RuoloProponente.descrizione + ")";
                this.lblProponente.Text += " " + del + " " + istanzaProcessoDiFirma.UtenteProponente.descrizione;
            }
            else
            {
                this.lblProponente.Text = istanzaProcessoDiFirma.UtenteProponente.descrizione + " (" + istanzaProcessoDiFirma.RuoloProponente.descrizione + ")";
            }
            this.lblProponente.ToolTip = this.lblProponente.Text;
            this.LblAvviatoIl.Text = istanzaProcessoDiFirma.dataAttivazione;

            if (!string.IsNullOrEmpty(istanzaProcessoDiFirma.dataChiusura))
            {
                this.lblConclusoIl.Text = istanzaProcessoDiFirma.dataChiusura;
                this.pnlConclusoIl.Attributes.Add("style", "display:block");
                if (istanzaProcessoDiFirma.statoProcesso.Equals(TipoStatoProcesso.STOPPED))
                {
                    switch (istanzaProcessoDiFirma.ChaInterroDa)
                    { 
                        case PROPONENTE:
                            this.LtlConclusoIl.Text = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeInterrottoDalProponenteIl", UserManager.GetUserLanguage());
                            break;
                        case TITOLARE:
                            this.LtlConclusoIl.Text = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeInterrottoDalTitolareIl", UserManager.GetUserLanguage());
                            break;
                        case AMMINISTRATORE:
                            this.LtlConclusoIl.Text = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeInterrottoDaAmministratoreIl", UserManager.GetUserLanguage());
                            break;
                        default:
                            this.LtlConclusoIl.Text = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeInterrottoIl", UserManager.GetUserLanguage());
                            break;
                    }
                }
                else
                {
                    if ((from i in istanzaProcessoDiFirma.istanzePassoDiFirma where i.statoPasso.Equals(TipoStatoPasso.CUT) select i).FirstOrDefault() != null)
                    {
                        this.LtlConclusoIl.Text = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeConclusoConTroncamentoIl", UserManager.GetUserLanguage());
                    }
                    else
                    {
                        this.LtlConclusoIl.Text = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeConclusoIl", UserManager.GetUserLanguage());
                    }
                }
            }
            else
            {
                this.pnlConclusoIl.Attributes.Add("style", "display:none");
            }

            if (!string.IsNullOrEmpty(istanzaProcessoDiFirma.NoteDiAvvio))
            {
                this.lblNoteAvvio.Text = istanzaProcessoDiFirma.NoteDiAvvio;
                this.pnlNoteAvvio.Attributes.Add("style", "display:block");
            }
            else
            {
                this.pnlNoteAvvio.Attributes.Add("style", "display:none");
            }

            if (!string.IsNullOrEmpty(istanzaProcessoDiFirma.MotivoRespingimento) && !string.IsNullOrEmpty(istanzaProcessoDiFirma.dataChiusura))
            {
                lblMotivoInterruzione.Text = istanzaProcessoDiFirma.MotivoRespingimento;
                this.pnlMotivoInterruzione.Attributes.Add("style", "display:block");
            }
            else
            {
                this.pnlMotivoInterruzione.Attributes.Add("style", "display:none");
            }
        }


        #endregion

        #region TreeView

        private void BindTreeViewStateSignatureProcessConcluted(List<IstanzaProcessoDiFirma> istanzeProcessoDiFirma)
        {
            try
            {
                if (istanzeProcessoDiFirma != null && istanzeProcessoDiFirma.Count > 0)
                {
                    foreach (IstanzaProcessoDiFirma istanzaProcessoDiFirma in istanzeProcessoDiFirma)
                    {
                        TreeNode root = new TreeNode();
                        root.Text = istanzaProcessoDiFirma.Descrizione;
                        root.Value = istanzaProcessoDiFirma.idIstanzaProcesso;
                        root.ToolTip = istanzaProcessoDiFirma.Descrizione;
                        foreach (IstanzaPassoDiFirma passo in istanzaProcessoDiFirma.istanzePassoDiFirma)
                        {
                            this.AddChildrenElements(passo, ref root, istanzaProcessoDiFirma);
                        }
                        root.Collapse();
                        this.TreeSignatureProcess.Nodes.Add(root);
                    }
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
                    //if(!string.IsNullOrEmpty(istanzaProcessoDiFirma.Errore))
                    //    root.ImageUrl = "../Images/Icons/task_in_corso.png";
                    foreach (IstanzaPassoDiFirma passo in istanzaProcessoDiFirma.istanzePassoDiFirma)
                    {
                        this.AddChildrenElements(passo, ref root, istanzaProcessoDiFirma);
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


        private TreeNode AddChildrenElements(IstanzaPassoDiFirma p, ref TreeNode root, IstanzaProcessoDiFirma istanzaProcessoDiFirma)
        {
            bool isInterrupted = istanzaProcessoDiFirma.statoProcesso == TipoStatoProcesso.STOPPED;
            char interrottoDa = istanzaProcessoDiFirma.ChaInterroDa;
            bool isConcluted = !string.IsNullOrEmpty(istanzaProcessoDiFirma.dataChiusura);

            TreeNode nodeChild = new TreeNode();
            string text;
            nodeChild.ImageUrl = p.statoPasso.Equals(TipoStatoPasso.CUT) ? LibroFirmaManager.GetIconEventTypeDisabled(p) : LibroFirmaManager.GetIconEventType(p);
            nodeChild.Value = p.idPasso;
            text = p.statoPasso.Equals(TipoStatoPasso.CUT) ? "<div class ='disabled'>" + LibroFirmaManager.GetHolder(p) + "</div>" : LibroFirmaManager.GetHolder(p);
            nodeChild.Text = text;
            nodeChild.ToolTip = LibroFirmaManager.GetHolder(p);
            nodeChild.SelectAction = TreeNodeSelectAction.None;

            if (!string.IsNullOrEmpty(p.Note))
            {
                TreeNode nodeChildNote = new TreeNode();
                text = p.statoPasso.Equals(TipoStatoPasso.CUT) ? "<div class ='disabled'>" + p.Note + "</div>" : p.Note;
                nodeChildNote.Text = text;
                nodeChildNote.ToolTip = p.Note;
                nodeChildNote.SelectAction = TreeNodeSelectAction.None;
                nodeChild.ChildNodes.Add(nodeChildNote);
            }

            if (!string.IsNullOrEmpty(p.dataEsecuzione))
            {
                TreeNode nodeChildDateExecution = new TreeNode();
                string user = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeUserLocker", UserManager.GetUserLanguage()) + "  " + p.DescrizioneUtenteLocker;
                string action = p.statoPasso.Equals(TipoStatoPasso.CLOSE) ? Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeEseguitoIl", UserManager.GetUserLanguage()) :
                    Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeInterrottoIl", UserManager.GetUserLanguage());
                text = p.statoPasso.Equals(TipoStatoPasso.CUT) ? "<div class ='disabled'>" + action + " " + p.dataEsecuzione + " " + user + "</div>" : action + " " + p.dataEsecuzione + " " + user;
                nodeChildDateExecution.Text = text;
                nodeChildDateExecution.SelectAction = TreeNodeSelectAction.None;
                nodeChild.ChildNodes.Add(nodeChildDateExecution);
            }
            if(!string.IsNullOrEmpty(p.Errore))
            {
                TreeNode nodeChildNote = new TreeNode();
                text = FormatError(p.Errore);
                nodeChildNote.Text = text;
                nodeChildNote.ToolTip = text;
                nodeChildNote.SelectAction = TreeNodeSelectAction.None;
                nodeChildNote.ImageUrl = "../Images/Icons/task_in_corso.png";
                nodeChild.ChildNodes.Add(nodeChildNote);
            }

            root.ChildNodes.Add(nodeChild);
            if (p.statoPasso.Equals(TipoStatoPasso.LOOK) && !isConcluted)
            {
                nodeChild.Select();
            }
            
            if (p.statoPasso.Equals(TipoStatoPasso.LOOK)  && isInterrupted && interrottoDa != '0')
            {
                TreeNode nodeChildDateExecution = new TreeNode();
                switch(interrottoDa)
                {
                    case PROPONENTE:
                        string descUtenteInterruzione;
                        if (string.IsNullOrEmpty(istanzaProcessoDiFirma.DescUtenteDelegatoInterruzione))
                        {
                            descUtenteInterruzione = istanzaProcessoDiFirma.DescUtenteInterruzione;
                        }
                        else
                        {
                            descUtenteInterruzione = istanzaProcessoDiFirma.DescUtenteDelegatoInterruzione + " (" + Utils.Languages.GetLabelFromCode("TransmissionDelegatedBy", UserManager.GetUserLanguage()).ToUpper() + " " + istanzaProcessoDiFirma.DescUtenteInterruzione + ")";
                        }
                        //nodeChildDateExecution.Text = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeInterruptedByProponente", UserManager.GetUserLanguage());
                        nodeChildDateExecution.Text = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeInterruptedByProponenteId", UserManager.GetUserLanguage()).Replace("@@", descUtenteInterruzione);
                        break;
                    case AMMINISTRATORE:
                        nodeChildDateExecution.Text = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeInterruptedByAmministrazione", UserManager.GetUserLanguage());
                        break;
                    default:
                        nodeChildDateExecution.Text = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeInterrupted", UserManager.GetUserLanguage());
                        break;
                }
                nodeChildDateExecution.SelectAction = TreeNodeSelectAction.None;
                nodeChild.ChildNodes.Add(nodeChildDateExecution);
            }
            return nodeChild;
        }

        private string FormatError(string error)
        {
            string retValue = string.Empty;

            retValue = error.Replace("#DESTINATARIO#", "<br /><b>").Replace("#DESCRIZIONE#", "</b>");

            return retValue;
        }

        protected void TreeSignatureProcess_SelectedNodeChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            try
            {
                TreeNode node = this.TreeSignatureProcess.SelectedNode;
                IstanzaProcessoDiFirma istanza = (from i in this.ListaIstanzaProcessoDiFirma where i.idIstanzaProcesso.Equals(node.Value) select i).FirstOrDefault();
                if (istanza != null)
                {
                    PopolaDettalioIstanza(istanza);
                    this.pnlDetailsSignatureProcess.Visible = true;
                    this.pnlProponente.Visible = false;
                    this.upPnlDetailsSignatureProcess.Update();
                }
            }
            catch (Exception ex)
            {

                return;
            }
        }

        protected void TreeSignatureProcess_Collapsed(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

        }

        protected void TreeSignatureProcess_Expanded(object sender, TreeNodeEventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            //e.Node.Select();
        }

        #endregion

        #region Event button

        protected void InExecuteLinkList_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

            this.TreeSignatureProcess.Nodes.Clear();

            this.liInExecuteList.Attributes.Remove("class");
            this.liInExecuteList.Attributes.Add("class", "addressTab");
            this.liConcludedList.Attributes.Remove("class");
            this.liConcludedList.Attributes.Add("class", "otherAddressTab");

            //Estraggo dalla lista il processo in esecuzione
            IstanzaProcessoDiFirma istanza = (from i in ListaIstanzaProcessoDiFirma
                                              where string.IsNullOrEmpty(i.dataChiusura)
                                              select i).FirstOrDefault();
            if (istanza != null)
            {
                PopolaDettalioIstanza(istanza);
                this.pnlDetailsSignatureProcess.Visible = true;
                BindTreeViewStateSignatureProcess(istanza);
                EnableContentProponent(istanza);
                this.pnlProponente.Visible = true;
            }
            else
            {
                this.pnlDetailsSignatureProcess.Visible = false;
            }

            this.upPnlTreeSignatureProcess.Update();
            this.upPnlDetailsSignatureProcess.Update();
            this.UpTypeResult.Update();
        }

        private void GoToTabConclutedLnk()
        {
            this.TreeSignatureProcess.Nodes.Clear();

            this.liConcludedList.Attributes.Remove("class");
            this.liConcludedList.Attributes.Add("class", "addressTab");
            this.liInExecuteList.Attributes.Remove("class");
            this.liInExecuteList.Attributes.Add("class", "otherAddressTab");

            List<IstanzaProcessoDiFirma> istanzeConcluse = (from i in ListaIstanzaProcessoDiFirma
                                                            where !string.IsNullOrEmpty(i.dataChiusura)
                                                            orderby Utils.dateformat.ConvertToDate(i.dataChiusura) descending
                                                            select i).ToList();

            BindTreeViewStateSignatureProcessConcluted(istanzeConcluse);
            PopolaDettalioIstanza(IstanzaProcessoDiFirma);
            this.TreeSignatureProcess.Nodes[0].Expand();
            this.TreeSignatureProcess.Nodes[0].Select();
            this.pnlDetailsSignatureProcess.Visible = true;
            this.DetailsLFAutomaticModeInterruption.Visible = false;
            this.DetailsLFAutomaticModeModify.Visible = false;
            this.pnlProponente.Attributes.Add("style", "display:none");

            this.UpPnlButtons.Update();
            this.upPnlTreeSignatureProcess.Update();
            this.upPnlDetailsSignatureProcess.Update();
            this.UpTypeResult.Update();
        }

        protected void InConcludedLinkList_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            this.TreeSignatureProcess.Nodes.Clear();

            this.liConcludedList.Attributes.Remove("class");
            this.liConcludedList.Attributes.Add("class", "addressTab");
            this.liInExecuteList.Attributes.Remove("class");
            this.liInExecuteList.Attributes.Add("class", "otherAddressTab");

            List<IstanzaProcessoDiFirma> istanzeConcluse = (from i in ListaIstanzaProcessoDiFirma
                                                            where !string.IsNullOrEmpty(i.dataChiusura)
                                                            select i).ToList();

            BindTreeViewStateSignatureProcessConcluted(istanzeConcluse);
            this.pnlDetailsSignatureProcess.Visible = false;
            this.DetailsLFAutomaticModeInterruption.Visible = false;
            this.DetailsLFAutomaticModeModify.Visible = false;
            this.pnlProponente.Attributes.Add("style", "display:none");

            this.UpPnlButtons.Update();
            this.upPnlTreeSignatureProcess.Update();
            this.upPnlDetailsSignatureProcess.Update();
            this.UpTypeResult.Update();
        }


        protected void DetailsLFAutomaticModeInterruption_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

            this.IstanzaProcessoDiFirma = (from i in ListaIstanzaProcessoDiFirma
                                           where string.IsNullOrEmpty(i.dataChiusura)
                                           select i).FirstOrDefault();

            string idDocPrinc = string.Empty;
            if (Request.QueryString["caller"] != null)
            {
                idDocPrinc = ListaElementiSelezionati[0].InfoDocumento.IdDocumentoPrincipale;
            }
            else
            {
                idDocPrinc = (DocumentManager.getSelectedRecord().documentoPrincipale!=null ? DocumentManager.getSelectedRecord().documentoPrincipale.docNumber : DocumentManager.getSelectedRecord().docNumber);
            }

            HttpContext.Current.Session["idDocPrinc"] = idDocPrinc;

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "InterruptionSignatureProcess", "ajaxModalPopupInterruptionSignatureProcess();", true);
        }

        protected void DetailsLFAutomaticModeModify_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

            //Estraggo dalla lista il processo in esecuzione
            IstanzaProcessoDiFirma istanza = (from i in ListaIstanzaProcessoDiFirma
                                              where string.IsNullOrEmpty(i.dataChiusura)
                                              select i).FirstOrDefault();
            OpzioniNotifica opzioniNotifiche = new OpzioniNotifica();
            opzioniNotifiche.Notifica_concluso = this.cbxNotificationOption.Items.FindByValue("cbxNotificationOptionOptCP").Selected;
            opzioniNotifiche.Notifica_interrotto = this.cbxNotificationOption.Items.FindByValue("cbxNotificationOptionOptIP").Selected;

            ListItem cbxErrore = this.cbxNotificationOption.Items.FindByValue("cbxNotificationOptionOptErrorePassoAutomaticoP");
            opzioniNotifiche.NotificaErrore = cbxErrore != null && cbxErrore.Selected;

            ListItem cbxDestNonInterop = this.cbxNotificationOption.Items.FindByValue("cbxNotificationOptionOptDestNonInterop");
            opzioniNotifiche.NotificaPresenzaDestNonInterop = cbxDestNonInterop != null && cbxDestNonInterop.Selected;
            istanza.Notifiche = opzioniNotifiche;

            if (LibroFirmaManager.UpdateIstanzaProcessoDiFirma(istanza))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('SuccessModifyIstanzaProcesso', 'check', '');} else {parent.ajaxDialogModal('SuccessModifyIstanzaProcesso', 'check', '');}", true);
                return;
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorModifyIstanzaProcesso', 'error', '');} else {parent.ajaxDialogModal('ErrorModifyIstanzaProcesso', 'error', '');}", true);
                return;
            }
        }

        protected void DetailsLFAutomaticModeClose_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('DetailsLFAutomaticMode','');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void BuildOpzioniNotifiche(IstanzaProcessoDiFirma istanza)
        {
            string language = UserManager.GetUserLanguage();
            bool isPresentePassoSpedizione = false;
            bool isPresentePassoAutomatico = false;

            this.cbxNotificationOption.Items.Clear();

            ListItem checkConclusione = new ListItem(Utils.Languages.GetLabelFromCode("StartProcessSignaturecbxNotificationOptionOptCP", language), "cbxNotificationOptionOptCP");
            checkConclusione.Selected = istanza.Notifiche.Notifica_concluso;
            this.cbxNotificationOption.Items.Add(checkConclusione);

            ListItem checkInterruzione = new ListItem(Utils.Languages.GetLabelFromCode("StartProcessSignaturecbxNotificationOptionOptIP", language), "cbxNotificationOptionOptIP");
            checkInterruzione.Selected = istanza.Notifiche.Notifica_interrotto;
            this.cbxNotificationOption.Items.Add(checkInterruzione);

            isPresentePassoAutomatico = (from p in istanza.istanzePassoDiFirma where p.IsAutomatico select p).FirstOrDefault() != null;
            if (isPresentePassoAutomatico)
            {
                ListItem checkErrore = new ListItem(Utils.Languages.GetLabelFromCode("cbxNotificationOptionOptErrorePassoAutomaticoP", language), "cbxNotificationOptionOptErrorePassoAutomaticoP");
                checkErrore.Selected = istanza.Notifiche.NotificaErrore;
                this.cbxNotificationOption.Items.Add(checkErrore);

                //Se la chiave di notifica obbligatoria di presenza di destinatari non interoperanti è obbligatoria, non mostro il check di scelta
                string attiva = Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, Utils.DBKeys.NOTIFICA_DEST_NO_INTEROP_OBB.ToString());
                if (string.IsNullOrEmpty(attiva) || !attiva.Equals("1"))
                {
                    isPresentePassoSpedizione = (from p in istanza.istanzePassoDiFirma where p.Evento.CodiceAzione.Equals(Azione.DOCUMENTOSPEDISCI.ToString()) select p).FirstOrDefault() != null;
                    if (isPresentePassoSpedizione)
                    {
                        ListItem checkDestNonInterop = new ListItem(Utils.Languages.GetLabelFromCode("cbxNotificationOptionOptDestNonInterop", language), "cbxNotificationOptionOptDestNonInterop");
                        checkDestNonInterop.Selected = istanza.Notifiche.NotificaPresenzaDestNonInterop;
                        this.cbxNotificationOption.Items.Add(checkDestNonInterop);
                    }
                }
            }
        }

        #endregion

    }
}