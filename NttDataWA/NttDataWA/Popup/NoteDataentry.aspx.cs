using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using NttDatalLibrary;

namespace NttDataWA.Popup
{
    public partial class NoteDataentry : System.Web.UI.Page
    {

        #region Properties

        private bool IsNew
        {
            get
            {
                return Request.QueryString["isnew"] != null;
            }
        }

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

            this.RefreshScript();
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            this.CloseMask(true);
        }

        protected void BtnConfirm_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

            if (string.IsNullOrEmpty(this.ddlRf.SelectedValue))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "dialog", "ajaxDialogModal('WarningNoteDataentrySelectRF', 'warning', '');", true);
                return;
            }
            else if (string.IsNullOrEmpty(this.txt_desc.Text))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "dialog", "ajaxDialogModal('WarningNoteDataentrySelectDesc', 'warning', '');", true);
                return;
            }
            else
            {
                DocsPaWR.NotaElenco nota = new DocsPaWR.NotaElenco();
                DocsPaWR.InfoUtente infoUt = UserManager.GetInfoUser();
                string message = "";
                bool result = false;

                if (this.IsNew)
                {
                    nota.codRegRf = this.ddlRf.SelectedItem.Text.Substring(0, this.ddlRf.SelectedItem.Text.IndexOf("-") - 1);
                    nota.idRegRf = this.ddlRf.SelectedItem.Value;
                    nota.descNota = this.txt_desc.Text;

                    result = NoteManager.InsertNotaInElenco(nota, out message);
                }
                else
                {
                    nota = (DocsPaWR.NotaElenco)this.ListaNote[this.verificaSelezioneNota()];
                    nota.codRegRf = this.ddlRf.SelectedItem.Text.Substring(0, this.ddlRf.SelectedItem.Text.IndexOf("-") - 1);
                    nota.idRegRf = this.ddlRf.SelectedItem.Value;
                    nota.descNota = this.txt_desc.Text;

                    result = NoteManager.ModNotaInElenco(nota, out message);
                }

                if (result)
                {
                    if (!string.IsNullOrEmpty(message))
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "dialog", "ajaxDialogModal('ErrCustom', 'error', '', '"+utils.FormatJs(message)+"');", true);
                    else
                        this.CloseMask(true);
                }
                else
                {
                    if (!string.IsNullOrEmpty(message))
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "dialog", "ajaxDialogModal('ErrCustom', 'error', '', '" + utils.FormatJs(message) + "');", true);
                    else
                    {
                        if (this.IsNew)
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "dialog", "ajaxDialogModal('ErrorNoteDataentryInserting', 'error', '');", true);
                        else
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "dialog", "ajaxDialogModal('ErrorNoteDataentryModifying', 'error', '');", true);
                    }
                    return;
                }
            }
        }

        #endregion

        #region Methods

        protected void InitializePage()
        {
            this.InitializeLanguage();
            this.CaricaRfVisibili();

            if (!this.IsNew)
            {
                int posizione = this.verificaSelezioneNota();
                if (posizione >= 0)
                {
                    DocsPaWR.NotaElenco nota = (DocsPaWR.NotaElenco)this.ListaNote[posizione];
                    this.ddlRf.Items.FindByValue(nota.idRegRf).Selected = true;
                    this.txt_desc.Text = nota.descNota;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "dialog", "ajaxDialogModal('ErrorNoteDataentryModifying', 'error', '');", true);
                }
            }
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnConfirm.Text = Utils.Languages.GetLabelFromCode("GenericBtnOk", language);
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("GenericBtnClose", language);
            this.lblRf.Text = Utils.Languages.GetLabelFromCode("ManageNotesRF", language);
            this.lblDesc.Text = Utils.Languages.GetLabelFromCode("ManageNotesDesc", language);
            this.ddlRf.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("ManageNotesRFSelect", language));
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
        }

        private void CloseMask(bool withReturnValue)
        {
            string retValue = withReturnValue ? "true" : "false";

            if (this.IsNew)
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('NoteDataentryNew', '" + retValue + "');", true);
            else
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('NoteDataentryModify', '" + retValue + "');", true);
        }

        private void CaricaRfVisibili()
        {
            int truncate = 60;
            this.ddlRf.Items.Clear();
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
                    this.ddlRf.Items.Add(li);
                }
                else
                {
                    //ListItem lit2 = new ListItem();
                    //lit2.Value = "T";
                    //lit2.Text = "Tutti";
                    //this.ddlRf.Items.Add(lit2);
                    foreach (DocsPaWR.Registro regis in listaRF)
                    {
                        ListItem li = new ListItem();
                        li.Value = regis.systemId;
                        if (regis.descrizione.Length > truncate)
                            li.Text = regis.codRegistro + " - " + regis.descrizione.Substring(0, truncate) + "...";
                        else
                            li.Text = regis.codRegistro + " - " + regis.descrizione;
                        this.ddlRf.Items.Add(li);
                    }
                }
            }
            else
            {
                this.ddlRf.Enabled = false;
            }
        }

        private int verificaSelezioneNota()
        {
            int posizione = -1;

            foreach (string id in this.CheckList)
            {
                for (int i=0; i<this.ListaNote.Length; i++)
                    if (this.ListaNote[i].idNota == id)
                        posizione = i;
            }

            return posizione;
        }

        #endregion
    }
}