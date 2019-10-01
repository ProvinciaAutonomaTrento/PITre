using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;

namespace NttDataWA.Popup
{
    public partial class StateInstanceProcessSignature : System.Web.UI.Page
    {
        #region Properties

        private string IdInstanceProcess
        {
            get
            {
                return (String)HttpContext.Current.Session["IdInstanceProcess"];
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
                this.InitializaPage();
            }
        }

        private void InitializaPage()
        {
            IstanzaProcessoDiFirma istanza = UIManager.SignatureProcessesManager.GetIstanzaProcessoDiFirmaByIdIstanzaProcesso(this.IdInstanceProcess);
            if (istanza != null)
            {
                PopolaDettalioIstanza(istanza);
                BindTreeViewStateSignatureProcess(istanza);
            }
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.StateInstanceProcessSignatureClose.Text = Utils.Languages.GetLabelFromCode("StateInstanceProcessSignatureClose", language);
            this.LtlNameSignatureProcess.Text = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeLtlNameSignatureProcess", language);
            this.LtlProponente.Text = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeLtlProponente", language);
            this.LtlAvviatoIl.Text = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeLtlAvviatoIl", language);
            this.LtlNoteAvvio.Text = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeLtlNoteAvvio", language);
            this.LtlMotivoInterruzione.Text = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeLtlMotivoInterruzione", language);
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
                            bool isInterrupted = istanzaProcessoDiFirma.statoProcesso == TipoStatoProcesso.STOPPED;
                            this.AddChildrenElements(passo, ref root, true, isInterrupted, istanzaProcessoDiFirma.ChaInterroDa);
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
                    bool isInterrupted = false;
                    bool isConcluted = false;
                    foreach (IstanzaPassoDiFirma passo in istanzaProcessoDiFirma.istanzePassoDiFirma)
                    {
                        isInterrupted = istanzaProcessoDiFirma.statoProcesso == TipoStatoProcesso.STOPPED;
                        isConcluted = istanzaProcessoDiFirma.statoProcesso == TipoStatoProcesso.CLOSED;
                        this.AddChildrenElements(passo, ref root, isConcluted, isInterrupted, istanzaProcessoDiFirma.ChaInterroDa);
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


        private TreeNode AddChildrenElements(IstanzaPassoDiFirma p, ref TreeNode root, bool isConcluted, bool isInterrupted, char interrottoDa)
        {
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
            if (!string.IsNullOrEmpty(p.Errore))
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
            if (p.statoPasso.Equals(TipoStatoPasso.LOOK) && isInterrupted && interrottoDa != '0')
            {
                TreeNode nodeChildDateExecution = new TreeNode();
                switch (interrottoDa)
                {
                    case PROPONENTE:
                        nodeChildDateExecution.Text = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeInterruptedByProponente", UserManager.GetUserLanguage());
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

        #region Event Button

        protected void StateInstanceProcessSignatureClose_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('StateInstanceProcessSignature','');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #endregion

    }
}