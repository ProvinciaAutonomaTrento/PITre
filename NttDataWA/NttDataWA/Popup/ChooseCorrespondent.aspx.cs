using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;

namespace NttDataWA.Popup
{
    public partial class ChooseCorrespondent : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.InitializePage();
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void InitializePage()
        {
            this.InitializeLanguage();
            this.ListCorr = null;
            this.ElemTemp = null;
            this.SelectedCustomCorrespondentIndex = string.Empty;
            this.GetInitData();
        }

        protected void SetSelectedRow(object sender, EventArgs e)
        {
            try {
                int indexCorr = Convert.ToInt32(this.grid_rowindex.Text);
                this.ElemTemp = this.ListCorr[indexCorr];
                if (!string.IsNullOrEmpty(this.TypeRecord) && (this.TypeRecord.Equals("A") || this.TypeRecord.Equals("P") || this.TypeRecord.Equals("I")))
                {
                    this.SelectedCustomCorrespondentIndex = string.Empty;
                }
                else
                {
                    this.SelectedCustomCorrespondentIndex = indexCorr.ToString();
                }
                this.HighlightSelectedRow(indexCorr);
                this.UpGridCorr.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void HighlightSelectedRow(int sel)
        {
            if (this.GridCorr.Rows.Count > 0)
            {
                foreach (GridViewRow GVR in this.GridCorr.Rows)
                {
                    if (GVR.RowIndex == sel)
                    {
                        GVR.CssClass += " selectedrow";
                    }
                    else
                    {
                        GVR.CssClass = GVR.CssClass.Replace(" selectedrow", "");
                    }
                }
            }
        }

        protected void GridCorr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Attributes["onclick"] = "disallowOp('Content2');$('#grid_rowindex').val('" + e.Row.RowIndex.ToString() + "'); __doPostBack('UpGridCorr', ''); return false;";
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.ChooseCorrespondentBtnOk.Text = Utils.Languages.GetLabelFromCode("ChooseCorrespondentBtnOk", language);
            this.ChooseCorrespondentBtnClose.Text = Utils.Languages.GetLabelFromCode("ChooseCorrespondentBtnClose", language);
            this.ChooseCorrespondentLabelTitle.Text = Utils.Languages.GetLabelFromCode("ChooseCorrespondentLabelTitle", language);
        }

        protected String GetCorrID(DocsPaWR.ElementoRubrica elem)
        {
            if (!string.IsNullOrEmpty(elem.systemId))
            {
                return elem.systemId;
            }
            else
            {
                return elem.codice;
            }

        }

        protected String GetCorrName(DocsPaWR.ElementoRubrica elem)
        {
            return elem.descrizione;
        }

        protected String GetCorrCodice(DocsPaWR.ElementoRubrica elem)
        {
            return elem.codice;
        }

        protected String GetCorrTipo(DocsPaWR.ElementoRubrica elem)
        {
            string result = string.Empty;
            string codRegTemp = elem.codiceRegistro;
            if (elem.isRubricaComune == true)
            {
                codRegTemp = "RC";
            }
            else
            {
                if (codRegTemp == null || string.IsNullOrEmpty(codRegTemp))
                {
                    if (elem.interno == true)
                    {
                        codRegTemp = string.Empty;
                    }
                    else
                    {
                        codRegTemp = "TUTTI";
                    }
                }
                else
                {
                    codRegTemp = elem.codiceRegistro;
                }
            }

            return codRegTemp;
        }

        protected void GetInitData()
        {
            this.ListCorr = this.FoundCorr;
            string language = UIManager.UserManager.GetUserLanguage();


            this.GridCorr.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("GridCorrDescription", language);
            this.GridCorr.Columns[2].HeaderText = Utils.Languages.GetLabelFromCode("GridCorrCode", language);
            this.GridCorr.Columns[3].HeaderText = Utils.Languages.GetLabelFromCode("GridCorrVisibility", language);
            this.GridCorr.Columns[4].HeaderText = Utils.Languages.GetLabelFromCode("GridCorrStored", language);

            this.GridCorr.DataSource = this.ListCorr;
            //if (!string.IsNullOrEmpty(Request.QueryString["page"]) && Request.QueryString["page"].Equals("ricerca"))
            //{
            //    foreach (DataGridColumn col in GridCorr.Columns)
            //    {
            //        if (col.HeaderText.ToUpper().Equals("STORICIZZATO"))
            //        {
            //            col.Visible = true;
            //            break;
            //        }
            //    }
            //}
            this.GridCorr.DataBind();
        }

        protected string GetStoricizzato(DocsPaWR.ElementoRubrica elem)
        {
            DocsPaWR.Corrispondente corr = UIManager.AddressBookManager.GetCorrespondentBySystemId(elem.systemId);
            if (corr != null && (!string.IsNullOrEmpty(corr.dta_fine)))
                return "SI";
            else
                return "NO";
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

            if (this.ElemTemp!=null)
            {
                string retValue = string.Empty;
                DocsPaWR.Corrispondente tempCorr = null;
                if (!string.IsNullOrEmpty(this.ElemTemp.systemId))
                {
                    tempCorr = UIManager.AddressBookManager.GetCorrespondentBySystemId(ElemTemp.systemId);
                }
                if (tempCorr == null)
                {
                    tempCorr = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(ElemTemp.codice);
                }

                if (!string.IsNullOrEmpty(this.TypeRecord) && (this.TypeRecord.Equals("A") || this.TypeRecord.Equals("P") || this.TypeRecord.Equals("I")))
                {
                    //if (tempCorr == null)
                    //{
                    //    tempCorr = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(ElemTemp.codice);
                    //}


                    if (this.TypeRecord.Equals("A"))
                    {
                        if (this.MultipleSenders != null && this.MultipleSenders.Count > 0 && UIManager.AddressBookManager.esisteCorrispondente(this.MultipleSenders.ToArray<Corrispondente>(), tempCorr))
                        {
                            string msg = "WarningChooseCorrespondentMultipleSender";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');}", true);
                            return;
                        }
                        else
                        {
                            retValue = "sender";
                            this.ChooseMultipleCorrespondent = tempCorr;
                        }
                    }

                    if (this.TypeRecord.Equals("P"))
                    {
                        if (this.TypeChooseCorrespondent.Equals("Sender"))
                        {
                            retValue = "sender";
                            this.ChooseMultipleCorrespondent = tempCorr;
                        }
                        else
                        {
                            if (this.ListRecipients != null && this.ListRecipients.Count > 0 && UIManager.AddressBookManager.esisteCorrispondente(this.ListRecipients.ToArray<Corrispondente>(), tempCorr))
                            {
                                string msg = "WarningChooseCorrespondentRecipients";
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');}", true);
                                return;
                            }
                            else
                            {
                                if (this.ListRecipientsCC != null && this.ListRecipientsCC.Count > 0 && UIManager.AddressBookManager.esisteCorrispondente(this.ListRecipientsCC.ToArray<Corrispondente>(), tempCorr))
                                {
                                    string msg = "WarningChooseCorrespondentRecipientsCC";
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');}", true);
                                    return;
                                }
                                else
                                {
                                    retValue = "recipients";
                                    this.ChooseMultipleCorrespondent = tempCorr;
                                }
                            }
                        }
                    }

                    if (this.TypeRecord.Equals("I"))
                    {
                        if (this.TypeChooseCorrespondent.Equals("Sender"))
                        {
                            retValue = "sender";
                            this.ChooseMultipleCorrespondent = tempCorr;
                        }
                        else
                        {
                            if (this.ListRecipients != null && this.ListRecipients.Count > 0 && UIManager.AddressBookManager.esisteCorrispondente(this.ListRecipients.ToArray(), tempCorr))
                            {
                                string msg = "WarningChooseCorrespondentRecipients";
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');}", true);
                                return;
                            }
                            else
                            {
                                if (this.ListRecipientsCC != null && this.ListRecipientsCC.Count > 0 && UIManager.AddressBookManager.esisteCorrispondente(this.ListRecipientsCC.ToArray(), tempCorr))
                                {
                                    string msg = "WarningChooseCorrespondentRecipientsCC";
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');}", true);
                                    return;
                                }
                                else
                                {
                                    retValue = "recipients";
                                    this.ChooseMultipleCorrespondent = tempCorr;
                                }
                            }
                        }
                    }
                }
                else
                {
                    //Profiler document
                    retValue = this.IdCustomObjectCustomCorrespondent;
                    this.ChooseMultipleCorrespondent = tempCorr;
                }

                Response.Write("<html><body><script type=\"text/javascript\">parent.closeAjaxModal('ChooseCorrespondent', '" + retValue + "');</script></body></html>");
                Response.End();
            }
            else
            {
                string msg = "WarningChooseCorrespondentNoSelected";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');}", true);
            }
        }

        /// <summary>
        /// Delete Property in session
        /// </summary>
        private void DeleteProperty()
        {

        }

        protected void ObjectBtnChiudi_Click(object sender, EventArgs e)
        {
            Response.Write("<html><body><script type=\"text/javascript\">parent.closeAjaxModal('ChooseCorrespondent', '');</script></body></html>");
            Response.End();
        }

        private DocsPaWR.ElementoRubrica[] ListCorr
        {
            get
            {
                DocsPaWR.ElementoRubrica[] result = null;
                if (HttpContext.Current.Session["listCorr"] != null)
                {
                    result = HttpContext.Current.Session["listCorr"] as DocsPaWR.ElementoRubrica[];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listCorr"] = value;
            }
        }

        public string TypeChooseCorrespondent
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["typeChooseCorrespondent"] != null)
                {
                    result = HttpContext.Current.Session["typeChooseCorrespondent"] as string;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["typeChooseCorrespondent"] = value;
            }
        }

        public DocsPaWR.ElementoRubrica[] FoundCorr
        {
            get
            {
                DocsPaWR.ElementoRubrica[] result = null;
                if (HttpContext.Current.Session["foundCorr"] != null)
                {
                    result = HttpContext.Current.Session["foundCorr"] as DocsPaWR.ElementoRubrica[];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["foundCorr"] = value;
            }
        }

        public string TypeRecord
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["typeRecord"] != null)
                {
                    result = HttpContext.Current.Session["typeRecord"] as string;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["typeRecord"] = value;
            }
        }

        public List<Corrispondente> ListRecipientsCC
        {
            get
            {
                List<Corrispondente> result = new List<Corrispondente>();
                if (HttpContext.Current.Session["listRecipientsCC"] != null)
                {
                    result = HttpContext.Current.Session["listRecipientsCC"] as List<Corrispondente>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listRecipientsCC"] = value;
            }
        }

        public List<Corrispondente> ListRecipients
        {
            get
            {
                List<Corrispondente> result = new List<Corrispondente>();
                if (HttpContext.Current.Session["listRecipients"] != null)
                {
                    result = HttpContext.Current.Session["listRecipients"] as List<Corrispondente>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listRecipients"] = value;
            }
        }

        public DocsPaWR.Corrispondente ChooseMultipleCorrespondent
        {
            get
            {
                DocsPaWR.Corrispondente result = null;
                if (HttpContext.Current.Session["chooseMultipleCorrespondent"] != null)
                {
                    result = HttpContext.Current.Session["chooseMultipleCorrespondent"] as DocsPaWR.Corrispondente;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["chooseMultipleCorrespondent"] = value;
            }
        }

        public List<Corrispondente> MultipleSenders
        {
            get
            {
                List<Corrispondente> result = new List<Corrispondente>();
                if (HttpContext.Current.Session["multipleSenders"] != null)
                {
                    result = HttpContext.Current.Session["multipleSenders"] as List<Corrispondente>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["multipleSenders"] = value;
            }
        }

        public ElementoRubrica ElemTemp
        {
            get
            {
                ElementoRubrica result = new ElementoRubrica();
                if (HttpContext.Current.Session["elemTemp"] != null)
                {
                    result = HttpContext.Current.Session["elemTemp"] as ElementoRubrica;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["elemTemp"] = value;
            }
        }

        public string IdCustomObjectCustomCorrespondent
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["idCustomObjectCustomCorrespondent"] != null)
                {
                    result = HttpContext.Current.Session["idCustomObjectCustomCorrespondent"] as string;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["idCustomObjectCustomCorrespondent"] = value;
            }
        }

        public string SelectedCustomCorrespondentIndex
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["selectedCustomCorrespondentIndex"] != null)
                {
                    result = HttpContext.Current.Session["selectedCustomCorrespondentIndex"] as string;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["selectedCustomCorrespondentIndex"] = value;
            }
        }



    }
}