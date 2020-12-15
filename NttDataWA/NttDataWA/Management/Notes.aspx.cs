using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using NttDatalLibrary;

namespace NttDataWA.Management
{
    public partial class Notes : System.Web.UI.Page
    {

        #region Fields

        protected static int numNote;

        #endregion

        #region Properties

        protected DocsPaWR.NotaElenco[] ListaNote
        {
            get
            {
                if (Session["ListaNote"] != null)
                    return (DocsPaWR.NotaElenco[])Session["ListaNote"];
                else
                    return new DocsPaWR.NotaElenco[0];
            }
            set
            {
                Session["ListaNote"] = value;
            }
        }

        private List<string> CheckList
        {
            get
            {
                return HttpContext.Current.Session["CheckList"] as List<string>;
            }
            set
            {
                HttpContext.Current.Session["CheckList"] = value;
            }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InitializePage();
            }
            else
            {
                this.ReadRetValueFromPopup();
            }

            this.RefreshScript();
        }

        protected void dgNote_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            CheckBox chkSelection = e.Row.Cells[0].FindControl("cbSel") as CheckBox;
            if (chkSelection != null)
            {
                chkSelection.CssClass = "chk" + e.Row.Cells[1].Text;

                if (this.CheckList.Contains(e.Row.Cells[1].Text))
                    chkSelection.Checked = true;
            }
        }

        protected void cbSel_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
                this.CheckList.Add(((CheckBox)sender).CssClass.Replace("chk", ""));
            else
                this.CheckList.Remove(((CheckBox)sender).CssClass.Replace("chk", ""));


            this.BtnNew.Enabled = true;
            this.BtnModify.Enabled = false;
            this.BtnDelete.Enabled = true;

            int elemSelezionati = 0;
            for (int i = 0; i < dgNote.Rows.Count; i++)
            {
                GridViewRow item = dgNote.Rows[i];
                CheckBox chkSelection = item.Cells[0].FindControl("cbSel") as CheckBox;
                if (chkSelection != null && chkSelection.Checked)
                    elemSelezionati++;
            }

            if (elemSelezionati == 0)
                this.BtnDelete.Enabled = false;
            if (elemSelezionati == 1)
                this.BtnModify.Enabled = true;

            this.UpPnlButtons.Update();
        }

        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

            if (this.ddlFiltroRf.Enabled)
                this.ListaNote = NoteManager.GetListaNote(this.ddlFiltroRf.SelectedItem.Value, this.txt_desc.Text, out numNote);
            this.BindGridAndSelect();
            this.visib_pulsanti(true);
        }

        protected void BtnDelete_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

            ArrayList lista = new ArrayList();
            //Recupero la lista delle note da rimuovere
            foreach (string id in this.CheckList)
            {
                foreach (NotaElenco n in this.ListaNote)
                    if (n.idNota == id)
                        lista.Add(n);
            }

            if (lista.Count > 0)
            {
                if (NoteManager.DeleteNoteInElenco((DocsPaWR.NotaElenco[])lista.ToArray(typeof(DocsPaWR.NotaElenco))))
                {
                    this.ListaNote = NoteManager.GetListaNote(ddlFiltroRf.SelectedItem.Value, txt_desc.Text, out numNote);
                    this.BindGridAndSelect();
                    this.visib_pulsanti(true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "dialog", "ajaxDialogModal('ErrorManageNotesImpossibleToDelete', 'error', '');", true);
                    return;
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "dialog", "ajaxDialogModal('WarningManageNotesNoneSelected', 'warning', '');", true);
                return;
            }
        }

        #endregion

        #region Methods

        protected void InitializePage()
        {
            this.InitializeLanguage();
            this.CheckList = new List<string>();
            this.CaricaRfVisibili();
            if (this.ListaNote.Length>0)
                this.BindGridAndSelect();
            this.visib_pulsanti(true);
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnSearch.Text = Utils.Languages.GetLabelFromCode("ManageNotesBtnSearch", language);
            this.BtnNew.Text = Utils.Languages.GetLabelFromCode("ManageNotesBtnNew", language);
            this.BtnModify.Text = Utils.Languages.GetLabelFromCode("ManageNotesBtnModify", language);
            this.BtnDelete.Text = Utils.Languages.GetLabelFromCode("ManageNotesBtnDelete", language);
            this.BtnImport.Text = Utils.Languages.GetLabelFromCode("ManageNotesBtnImport", language);
            this.pageTitle.Text = Utils.Languages.GetLabelFromCode("ManageNotesTitle", language);
            this.dgNote.Columns[0].HeaderText = Utils.Languages.GetLabelFromCode("ManageNotesGridSel", language);
            this.dgNote.Columns[2].HeaderText = Utils.Languages.GetLabelFromCode("ManageNotesGridCode", language);
            this.dgNote.Columns[3].HeaderText = Utils.Languages.GetLabelFromCode("ManageNotesGridDescription", language);
            this.lbl_messaggio.Text = Utils.Languages.GetLabelFromCode("ManageNotesResult", language);
            this.lblRf.Text = Utils.Languages.GetLabelFromCode("ManageNotesRF", language);
            this.lblDesc.Text = Utils.Languages.GetLabelFromCode("ManageNotesDesc", language);
            this.NoteDataentryNew.Title = Utils.Languages.GetLabelFromCode("ManageNotesDataentryPopupNewTitle", language);
            this.NoteDataentryModify.Title = Utils.Languages.GetLabelFromCode("ManageNotesDataentryPopupModifyTitle", language);
            this.NoteImport.Title = Utils.Languages.GetLabelFromCode("ManageNotesImportPopupTitle", language);
        }

        private string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
        }

        private void ReadRetValueFromPopup()
        {
            if (!string.IsNullOrEmpty(this.NoteDataentryNew.ReturnValue))
            {
                this.txt_desc.Text = string.Empty;
                this.ddlFiltroRf.SelectedIndex = -1;
                
                this.ListaNote = NoteManager.GetListaNote(this.ddlFiltroRf.SelectedValue, this.txt_desc.Text, out numNote);
                this.BindGridAndSelect();
                this.visib_pulsanti(true);

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('NoteDataentryNew','');", true);
            }

            if (!string.IsNullOrEmpty(this.NoteDataentryModify.ReturnValue))
            {
                this.txt_desc.Text = string.Empty;
                this.ddlFiltroRf.SelectedIndex = -1;

                this.ListaNote = NoteManager.GetListaNote(this.ddlFiltroRf.SelectedValue, this.txt_desc.Text, out numNote);
                this.BindGridAndSelect();
                this.visib_pulsanti(true);

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('NoteDataentryModify','');", true);
            }
        }

        private void CaricaRfVisibili()
        {
            int truncate = 60;
            this.ddlFiltroRf.Items.Clear();
            //DocsPaWR.Registro[] listaRF = UserManager.getListaRegistriWithRF(Page, "1", "");
            DocsPaWR.Registro[] listaRF = RegistryManager.GetRFListInSession();
            if (listaRF != null && listaRF.Length > 0)
            {
                if (listaRF.Length == 1)
                {
                    ListItem li = new ListItem();
                    li.Value = listaRF[0].systemId;
                    if (listaRF[0].descrizione.Length > truncate)
                        li.Text = listaRF[0].codRegistro + " - " + listaRF[0].descrizione.Substring(0, truncate) + "...";
                    else
                        li.Text = listaRF[0].codRegistro + " - " + listaRF[0].descrizione;
                    this.ddlFiltroRf.Items.Add(li);
                }
                else
                {
                    ListItem lit = new ListItem();
                    lit.Value = "";
                    lit.Text = "Seleziona rf";
                    this.ddlFiltroRf.Items.Add(lit);
                    ListItem lit2 = new ListItem();
                    lit2.Value = "T";
                    lit2.Text = "Tutti";
                    this.ddlFiltroRf.Items.Add(lit2);
                    foreach (DocsPaWR.Registro regis in listaRF)
                    {
                        ListItem li = new ListItem();
                        li.Value = regis.systemId;
                        if (regis.descrizione.Length > truncate)
                            li.Text = regis.codRegistro + " - " + regis.descrizione.Substring(0, truncate) + "...";
                        else
                            li.Text = regis.codRegistro + " - " + regis.descrizione;
                        this.ddlFiltroRf.Items.Add(li);
                    }
                }
            }
            else
            {
                ddlFiltroRf.Enabled = false;
            }
        }

        private void visib_pulsanti(bool vis)
        {
            if (vis)
            {
                this.BtnNew.Enabled = true;
                if (this.ListaNote.Length == 0)
                {
                    this.BtnModify.Enabled = false;
                    this.BtnDelete.Enabled = false;
                }
                if (this.ListaNote.Length == 1)
                {
                    this.BtnModify.Enabled = true;
                    this.BtnDelete.Enabled = true;
                    CheckBox chkSelection = dgNote.Rows[0].Cells[0].FindControl("cbSel") as CheckBox;
                    chkSelection.Checked = vis;

                    if (vis)
                        this.CheckList.Add(chkSelection.CssClass.Replace("chk", ""));
                }
                else
                {
                    if (this.CheckList.Count == 1)
                    {
                        this.BtnModify.Enabled = true;
                        this.BtnDelete.Enabled = true;
                    }
                    else
                    {
                        this.BtnModify.Enabled = false;
                        this.BtnDelete.Enabled = false;
                    }
                }
            }
            else
            {
                this.BtnModify.Enabled = false;
                this.BtnDelete.Enabled = false;
                this.BtnNew.Enabled = false;
            }
            if (!ddlFiltroRf.Enabled)
                this.BtnNew.Enabled = false;

            this.UpPnlButtons.Update();
        }

        protected void BindGridAndSelect()
        {
            if (this.ListaNote != null)
            {
                DocsPaWR.NotaElenco[] note = this.ListaNote;
                this.dgNote.DataSource = note;
                this.dgNote.DataBind();
                if (this.ListaNote.Length > 0)
                {
                    this.dgNote.Visible = true;
                    this.lbl_messaggio.Text = this.GetLabel("ManageNotesResult").Replace("0", numNote.ToString());
                }
                else
                {
                    this.dgNote.Visible = false;
                    this.visib_pulsanti(true);
                    this.lbl_messaggio.Text = this.GetLabel("ManageNotesResult");
                }
            }
            else
            {
                this.dgNote.Visible = false;
                this.visib_pulsanti(true);
            }
            this.BtnNew.Enabled = true;

            this.UpPnlButtons.Update();
        }

        #endregion

    }
}