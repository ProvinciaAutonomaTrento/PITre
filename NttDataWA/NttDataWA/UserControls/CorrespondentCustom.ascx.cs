using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDatalLibrary;
using System.ComponentModel;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using System.Text;

namespace NttDataWA.UserControls
{
    public partial class CorrespondentCustom : System.Web.UI.UserControl
    {

        string typeChooseCorrespondent = string.Empty;
        RubricaCallType calltypeCorrespondentCustom = RubricaCallType.CALLTYPE_CORR_INT_EST;
        string pageCaller = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                this.InitializePage();
                this.InitializeLanguage();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void InitializePage()
        {
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_NEW_RUBRICA_VELOCE.ToString())) && (Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_NEW_RUBRICA_VELOCE.ToString())).Equals("1"))
            {
                this.EnableAjaxAddressBook = true;
            }
            else
            {
                this.EnableAjaxAddressBook = false;
            }

            if (!string.IsNullOrEmpty(this.PageCaller) && this.PageCaller.Equals("Popup"))
            {
                this.TxtCodeCorrespondentCustomValue.Attributes.Add("onchange", "disallowOp('Content2');");
                this.DocumentImgCustomCorrespondentAddressBookCustom.Attributes.Add("onclick", "disallowOp('Content2');");
            }

            if (this.SearchCorrespondentIntExtWithDisabled)
            {
                this.ChkStoryCustomCorrespondent.Visible = true;
            }
            else
            {
                this.ChkStoryCustomCorrespondent.Visible = false;
            }

            if (CODICE_READ_ONLY)
            {
                this.TxtCodeCorrespondentCustomValue.ReadOnly = true;
                this.TxtDescriptionCorrespondentCustomValue.ReadOnly = true;
                this.RapidCorrespondentCustom.Enabled = false;
                this.DocumentImgCustomCorrespondentAddressBookCustom.Enabled = false;
            }
            else
            {
                this.TxtCodeCorrespondentCustomValue.ReadOnly = false;
                this.TxtDescriptionCorrespondentCustomValue.ReadOnly = false;
                this.RapidCorrespondentCustom.Enabled = true;

                this.DocumentImgCustomCorrespondentAddressBookCustom.Enabled = true;

                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.AUTOCOMPLETE_MINIMUMPREFIXLENGTH.ToString()]))
                {
                    this.AjaxAddressBookMinPrefixLenght = int.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.AUTOCOMPLETE_MINIMUMPREFIXLENGTH.ToString()]);
                }

             //   this.RapidCorrespondentCustom.MinimumPrefixLength = 3;

                switch (typeChooseCorrespondent)
                {

                    case "INTERNI":
                        if (SearchCorrespondentIntExtWithDisabled)
                        {
                            this.CalltypeCorrespondentCustom = RubricaCallType.CALLTYPE_CORR_INT_CON_DISABILITATI;
                        }
                        else
                        {
                            this.CalltypeCorrespondentCustom = RubricaCallType.CALLTYPE_CORR_INT;
                        }
                        if (this.EnableAjaxAddressBook)
                        {
                            this.SetAjaxAddressBook(this.CalltypeCorrespondentCustom);
                        }
                        else
                        {
                            this.RapidCorrespondentCustom.Enabled = false;
                        }
                        break;
                    case "ESTERNI":
                        if (SearchCorrespondentIntExtWithDisabled)
                        {
                            this.CalltypeCorrespondentCustom = RubricaCallType.CALLTYPE_CORR_EST_CON_DISABILITATI;
                        }
                        else
                        {
                            this.CalltypeCorrespondentCustom = RubricaCallType.CALLTYPE_CORR_EST;
                        }
                        if (this.EnableAjaxAddressBook)
                        {
                            this.SetAjaxAddressBook(this.CalltypeCorrespondentCustom);
                        }
                        else
                        {
                            this.RapidCorrespondentCustom.Enabled = false;
                        }
                        break;
                    case "INTERNI/ESTERNI":
                        if (SearchCorrespondentIntExtWithDisabled)
                        {
                            this.CalltypeCorrespondentCustom = RubricaCallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI;
                        }
                        else
                        {
                            this.CalltypeCorrespondentCustom = RubricaCallType.CALLTYPE_CORR_INT_EST;
                        }
                        if (this.EnableAjaxAddressBook)
                        {
                            this.SetAjaxAddressBook(this.CalltypeCorrespondentCustom);
                        }
                        else
                        {
                            this.RapidCorrespondentCustom.Enabled = false;
                        }
                        break;
                    case "0":
                        if (SearchCorrespondentIntExtWithDisabled)
                        {
                            this.CalltypeCorrespondentCustom = RubricaCallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI;
                        }
                        else
                        {
                            this.CalltypeCorrespondentCustom = RubricaCallType.CALLTYPE_CORR_INT_EST;
                        }
                        break;
                    case "MISSING_ROLES":
                        this.CalltypeCorrespondentCustom = RubricaCallType.CALLTYPE_CORR_INT_NO_UO;
                        if (this.EnableAjaxAddressBook)
                        {
                            this.SetAjaxAddressBook(this.CalltypeCorrespondentCustom);
                        }
                        else
                        {
                            this.RapidCorrespondentCustom.Enabled = false;
                        }
                        break;
                }
            }
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.DocumentImgCustomCorrespondentAddressBookCustom.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgCustomCorrespondentAddressBookCustom", language);
            this.DocumentImgCustomCorrespondentAddressBookCustom.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgCustomCorrespondentAddressBookCustom", language);
            this.ChkStoryCustomCorrespondent.Text = Utils.Languages.GetLabelFromCode("chk_mitt_dest_storicizzati", language);
        }

        protected void SetAjaxAddressBook(RubricaCallType callType)
        {
            this.RapidCorrespondentCustom.MinimumPrefixLength = this.AjaxAddressBookMinPrefixLenght;
            this.RapidCorrespondentCustom.Enabled = true;
            this.TxtDescriptionCorrespondentCustomValue.Enabled = true;
            string dataUser = UIManager.RoleManager.GetRoleInSession().systemId;
            dataUser = dataUser + "-" + UIManager.RegistryManager.GetRegistryInSession().systemId;
            this.RapidCorrespondentCustom.ContextKey = dataUser + "-" + UIManager.UserManager.GetInfoUser().idAmministrazione + "-" + callType.ToString();
            RapidCorrespondentCustom.BehaviorID = "behavior_" + this.ClientID;
            string b = "behavior_" + this.ClientID;
            RapidCorrespondentCustom.OnClientPopulated = "acePopulated" + this.ClientID;
            RapidCorrespondentCustom.OnClientItemSelected = "aceSelected" + this.ClientID;
            string nomeFunzionePopulated = "acePopulated" + this.ClientID;
            string nomeFunzioneSelected = "aceSelected" + this.ClientID;
            string unique = this.UniqueID;
            builderJS(b, nomeFunzionePopulated, nomeFunzioneSelected, unique);
        }

        protected void TxtCodeCorrespondentCustom_OnTextChanged(object sender, EventArgs e)
        {
            try {
                if (!string.IsNullOrEmpty(this.PageCaller) && this.PageCaller.Equals("Popup"))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                }

                ElementoRubrica[] listaCorr = null;
                Corrispondente corr = null;

                if (!string.IsNullOrEmpty(this.TxtCodeCorrespondentCustomValue.Text))
                {
                    switch (typeChooseCorrespondent)
                    {
                        case "INTERNI":
                            if (SearchCorrespondentIntExtWithDisabled && this.ChkStoryCustomCorrespondent.Checked)
                            {
                                this.CalltypeCorrespondentCustom = RubricaCallType.CALLTYPE_CORR_INT_CON_DISABILITATI;
                            }
                            else
                            {
                                this.CalltypeCorrespondentCustom = RubricaCallType.CALLTYPE_CORR_INT;
                            }
                            break;
                        case "ESTERNI":
                            this.CalltypeCorrespondentCustom = RubricaCallType.CALLTYPE_CORR_EST;
                            break;
                        case "INTERNI/ESTERNI":
                            if (SearchCorrespondentIntExtWithDisabled && this.ChkStoryCustomCorrespondent.Checked)
                            {
                                this.CalltypeCorrespondentCustom = RubricaCallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI;
                            }
                            else
                            {
                                this.CalltypeCorrespondentCustom = RubricaCallType.CALLTYPE_CORR_INT_EST;
                            }
                            break;
                        case "0":
                            if (SearchCorrespondentIntExtWithDisabled && this.ChkStoryCustomCorrespondent.Checked)
                            {
                                this.CalltypeCorrespondentCustom = RubricaCallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI;
                            }
                            else
                            {
                                this.CalltypeCorrespondentCustom = RubricaCallType.CALLTYPE_CORR_INT_EST;
                            }
                            break;
                        case "MISSING_ROLES":
                            this.CalltypeCorrespondentCustom = RubricaCallType.CALLTYPE_CORR_INT_NO_UO;
                            break;
                    }

                    if (this.ChooseMultipleCorrespondent == null)
                    {
                        listaCorr = UIManager.AddressBookManager.getElementiRubricaMultipli(this.TxtCodeCorrespondentCustomValue.Text, this.CalltypeCorrespondentCustom, true);

                        if (listaCorr == null || (listaCorr != null && listaCorr.Length == 0))
                        {
                            corr = null;
                            this.IdCorrespondentCustomHidden.Value = string.Empty;
                            this.TxtCodeCorrespondentCustomValue.Text = string.Empty;
                            this.TxtDescriptionCorrespondentCustomValue.Text = string.Empty;
                            string msgDesc = "WarningDocumentCorrNotFound";
                            //this.UpPnlCorrespondentCustom.Update();

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                        }
                        else
                        {
                            if (listaCorr != null && listaCorr.Length > 1)
                            {
                                if (!string.IsNullOrEmpty(SelectedCustomCorrespondentIndex))
                                {
                                    int index = Convert.ToInt32(SelectedCustomCorrespondentIndex);
                                    ElementoRubrica el = listaCorr[index];
                                    if (!string.IsNullOrEmpty(el.systemId))
                                    {
                                        corr = UIManager.AddressBookManager.GetCorrespondentBySystemId(el.systemId);
                                    }
                                    else
                                    {
                                        corr = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(el.codice);
                                    }
                                    if (corr != null)
                                    {
                                        this.IdCorrespondentCustomHidden.Value = corr.systemId;
                                        this.TxtCodeCorrespondentCustomValue.Text = corr.codiceRubrica;
                                        this.TxtDescriptionCorrespondentCustomValue.Text = corr.descrizione;
                                    }
                                    else
                                    {
                                        corr = null;
                                        this.IdCorrespondentCustomHidden.Value = string.Empty;
                                        this.TxtCodeCorrespondentCustomValue.Text = string.Empty;
                                        this.TxtDescriptionCorrespondentCustomValue.Text = string.Empty;
                                    }

                                    this.SelectedCustomCorrespondentIndex = string.Empty;
                                }
                                else
                                {
                                    corr = null;
                                    this.FoundCorr = listaCorr;
                                    this.TypeChooseCorrespondent = this.TypeCorrespondentCustom.ToString();
                                    this.TypeRecord = "custom";
                                    this.IdCustomObjectCustomCorrespondent = this.ID;
                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chooseCorrespondent", "ajaxModalPopupChooseCorrespondent();", true);
                                    return;
                                }
                            }
                            else
                            {
                                if (listaCorr != null && listaCorr.Length == 1) // && !this.cbx_storicizzato.Checked)
                                {
                                    DocsPaWR.ElementoRubrica er = listaCorr[0];

                                    if (!string.IsNullOrEmpty(er.systemId))
                                    {
                                        corr = UIManager.AddressBookManager.GetCorrespondentBySystemId(er.systemId);
                                    }
                                    else
                                    {
                                        corr = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(er.codice);
                                    }

                                    if (corr != null)
                                    {
                                        this.IdCorrespondentCustomHidden.Value = corr.systemId;
                                        this.TxtCodeCorrespondentCustomValue.Text = corr.codiceRubrica;
                                        this.TxtDescriptionCorrespondentCustomValue.Text = corr.descrizione;
                                    }
                                    else
                                    {
                                        corr = null;
                                        this.IdCorrespondentCustomHidden.Value = string.Empty;
                                        this.TxtCodeCorrespondentCustomValue.Text = string.Empty;
                                        this.TxtDescriptionCorrespondentCustomValue.Text = string.Empty;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (this.IdCustomObjectCustomCorrespondent == this.ID)
                        {
                            this.IdCorrespondentCustomHidden.Value = this.ChooseMultipleCorrespondent.systemId;
                            this.TxtCodeCorrespondentCustomValue.Text = this.ChooseMultipleCorrespondent.codiceRubrica;
                            this.TxtDescriptionCorrespondentCustomValue.Text = this.ChooseMultipleCorrespondent.descrizione;
                            this.ChooseMultipleCorrespondent = null;
                        }
                    }
                }
                else
                {
                    this.IdCorrespondentCustomHidden.Value = string.Empty;
                    this.TxtCodeCorrespondentCustomValue.Text = string.Empty;
                    this.TxtDescriptionCorrespondentCustomValue.Text = string.Empty;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void builderJS(string b, string nomeFunzionePopulated, string nomeFunzioneSelected, string uniqueID)
        {
            //Populated
            StringBuilder sbPopulated = new StringBuilder();
            sbPopulated.AppendLine("function " + nomeFunzionePopulated + "(sender, e) {");
            sbPopulated.AppendLine("var behavior = $find('" + b + "');");
            sbPopulated.AppendLine("var target = behavior.get_completionList();");
            sbPopulated.AppendLine("if (behavior._currentPrefix != null) {");
            sbPopulated.AppendLine("var prefix = behavior._currentPrefix.toLowerCase();");
            sbPopulated.AppendLine("var i;");
            sbPopulated.AppendLine("for (i = 0; i < target.childNodes.length; i++) {");
            sbPopulated.AppendLine("var sValue = target.childNodes[i].innerHTML.toLowerCase();");
            sbPopulated.AppendLine("if (sValue.indexOf(prefix) != -1) {");
            sbPopulated.AppendLine("var fstr = target.childNodes[i].innerHTML.substring(0, sValue.indexOf(prefix));");
            sbPopulated.AppendLine(
                "var pstr = target.childNodes[i].innerHTML.substring(fstr.length, fstr.length + prefix.length);");
            sbPopulated.AppendLine(
                "var estr = target.childNodes[i].innerHTML.substring(fstr.length + prefix.length, target.childNodes[i].innerHTML.length);");
            sbPopulated.AppendLine("target.childNodes[i].innerHTML = fstr + '<span class=\"selectedWord\">' + pstr + '</span>' + estr;");
            sbPopulated.AppendLine("try");
            sbPopulated.AppendLine("{");
            sbPopulated.AppendLine("target.childNodes[i].attributes[\"_value\"].value = fstr + pstr + estr;");
            sbPopulated.AppendLine("}");
            sbPopulated.AppendLine("catch (ex)");
            sbPopulated.AppendLine("{");
            sbPopulated.AppendLine("target.childNodes[i].attributes[\"_value\"] = fstr + pstr + estr;");
            sbPopulated.AppendLine("}");
            sbPopulated.AppendLine("}");
            sbPopulated.AppendLine("}");
            sbPopulated.AppendLine("}");
            sbPopulated.AppendLine("}");

            //Response.Write(sbPopulated.ToString());
            ScriptManager.RegisterStartupScript(this, this.GetType(), nomeFunzioneSelected + "1", sbPopulated.ToString(), true);

            StringBuilder sbSelected = new StringBuilder();
            sbSelected.AppendLine("function " + nomeFunzioneSelected + "(sender, e) {");
            sbSelected.AppendLine("var value = e.get_value();");
            sbSelected.AppendLine("if (!value)");
            sbSelected.AppendLine("{");
            sbSelected.AppendLine("if (e._item.parentElement && e._item.parentElement.tagName == \"LI\")");
            sbSelected.AppendLine("{");
            sbSelected.AppendLine("try");
            sbSelected.AppendLine("{");
            sbSelected.AppendLine("value = e._item.parentElement.attributes[\"_value\"].value;");
            sbSelected.AppendLine("}");
            sbSelected.AppendLine("catch (ex1)");
            sbSelected.AppendLine("{");
            sbSelected.AppendLine("value = e._item.parentElement.attributes[\"_value\"];");
            sbSelected.AppendLine("}");
            sbSelected.AppendLine("if (value == undefined || value == null)");
            sbSelected.AppendLine("value = e._item.parentElement.attributes[\"_value\"];");
            sbSelected.AppendLine("}");
            sbSelected.AppendLine("else if (e._item.parentElement && e._item.parentElement.parentElement.tagName == \"LI\")");
            sbSelected.AppendLine("{");
            sbSelected.AppendLine("try");
            sbSelected.AppendLine("{");
            sbSelected.AppendLine("value = e._item.parentElement.attributes[\"_value\"].value;");
            sbSelected.AppendLine("}");
            sbSelected.AppendLine("catch (ex1)");
            sbSelected.AppendLine("{");
            sbSelected.AppendLine("value = e._item.parentElement.attributes[\"_value\"];");
            sbSelected.AppendLine("}");
            sbSelected.AppendLine("if (value == undefined || value == null)");
            sbSelected.AppendLine("value = e._item.parentElement.attributes[\"_value\"];");
            sbSelected.AppendLine("}");
            sbSelected.AppendLine("else if (e._item.parentNode && e._item.parentNode.tagName == \"LI\")");
            sbSelected.AppendLine("{");
            sbSelected.AppendLine("value = e._item.parentNode._value;");
            sbSelected.AppendLine("}");
            sbSelected.AppendLine("else if (e._item.parentNode && e._item.parentNode.parentNode.tagName == \"LI\")");
            sbSelected.AppendLine("{");
            sbSelected.AppendLine("value = e._item.parentNode.parentNode._value;");
            sbSelected.AppendLine("}");
            sbSelected.AppendLine("else value = \"\";");
            sbSelected.AppendLine("}");
            sbSelected.AppendLine("var searchText = $get('" + this.TxtDescriptionCorrespondentCustomValue.ClientID + "').value;");
            sbSelected.AppendLine("searchText = searchText.replace('null', '');");
            sbSelected.AppendLine("var testo = value;");
            sbSelected.AppendLine("var indiceFineCodice = testo.lastIndexOf(')');");
            sbSelected.AppendLine("document.getElementById('" + this.TxtDescriptionCorrespondentCustomValue.ClientID + "').focus();");
            sbSelected.AppendLine("document.getElementById('" + this.TxtDescriptionCorrespondentCustomValue.ClientID + "').value = \"\";");
            sbSelected.AppendLine("var indiceDescrizione = testo.lastIndexOf('(');");
            sbSelected.AppendLine("var descrizione = testo.substr(0, indiceDescrizione - 1);");
            sbSelected.AppendLine("var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);");
            sbSelected.AppendLine("document.getElementById('" + this.TxtCodeCorrespondentCustomValue.ClientID + "').value = codice;");
            sbSelected.AppendLine("document.getElementById('" + this.TxtDescriptionCorrespondentCustomValue.ClientID + "').value = descrizione;");
            //sbSelected.AppendLine("setTimeout(\"__doPostBack('txt_Codice',''), 0\");");
            sbSelected.AppendLine("}");



            ScriptManager.RegisterStartupScript(this, this.GetType(), nomeFunzioneSelected + "2", sbSelected.ToString(), true);

        }

        protected void DocumentImgCustomAddressBook_Click(object sender, EventArgs e)
        {
            try {
                HttpContext.Current.Session["AddressBook.from"] = "CUSTOM";

                if (!string.IsNullOrEmpty(this.PageCaller) && this.PageCaller.Equals("Popup"))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                }

                switch (typeChooseCorrespondent)
                {

                    case "INTERNI":
                        if (this.SearchCorrespondentIntExtWithDisabled && this.ChkStoryCustomCorrespondent.Checked)
                        {
                            this.CallType = RubricaCallType.CALLTYPE_CORR_INT_CON_DISABILITATI;
                        }
                        else
                        {
                            this.CallType = RubricaCallType.CALLTYPE_CORR_INT;
                        }
                        break;
                    case "ESTERNI":
                        if (this.SearchCorrespondentIntExtWithDisabled && this.ChkStoryCustomCorrespondent.Checked)
                        {
                            this.CallType = RubricaCallType.CALLTYPE_CORR_EST_CON_DISABILITATI;
                        }
                        else
                        {
                            this.CallType = RubricaCallType.CALLTYPE_CORR_EST;
                        }
                        break;
                    case "INTERNI/ESTERNI":
                        if (this.SearchCorrespondentIntExtWithDisabled && this.ChkStoryCustomCorrespondent.Checked)
                        {
                            this.CallType = RubricaCallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI;
                        }
                        else
                        {
                            this.CallType = RubricaCallType.CALLTYPE_CORR_INT_EST;
                        }
                        break;
                    case "0":
                        if (this.SearchCorrespondentIntExtWithDisabled && this.ChkStoryCustomCorrespondent.Checked)
                        {
                            this.CallType = RubricaCallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI;
                        }
                        else
                        {
                            this.CallType = RubricaCallType.CALLTYPE_CORR_INT_EST;
                        }
                        break;
                    case "MISSING_ROLES":
                        this.CallType = RubricaCallType.CALLTYPE_CORR_INT_NO_UO;
                        break;
                }

                this.RemovePropertySearchCorrespondentIntExtWithDisabled();
                this.IdCustomObjectCustomCorrespondent = this.ID;

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "UpPnlCustomCorrespondent", "ajaxModalPopupAddressBook();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        public bool CODICE_READ_ONLY
        {
            get
            {
                return sCodiceReadOnly;
            }
            set
            {
                sCodiceReadOnly = value;
            }
        }

        protected string GetIdUserControlCorrespondet()
        {
            return this.ClientID;
        }

        public string TypeCorrespondentCustom
        {
            get
            {
                return TypeChooseCorrespondent;
            }
            set
            {
                TypeChooseCorrespondent = value;
            }
        }

        [Browsable(true)]
        public string IdCorrespondentCustom
        {
            get
            {
                return this.IdCorrespondentCustomHidden.Value;
            }
            set
            {
                this.IdCorrespondentCustomHidden.Value = value;
            }
        }

        [Browsable(true)]
        public string TxtCodeCorrespondentCustom
        {
            get
            {
                return this.TxtCodeCorrespondentCustomValue.Text;
            }
            set
            {
                this.TxtCodeCorrespondentCustomValue.Text = value;
            }
        }


        [Browsable(true)]
        public string TxtDescriptionCorrespondentCustom
        {
            get
            {
                return this.TxtDescriptionCorrespondentCustomValue.Text;
            }
            set
            {
                this.TxtDescriptionCorrespondentCustomValue.Text = value;
            }
        }

        [Browsable(true)]
        public bool DisabledCorrespondentCustom
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["disabledCorrespondentCustom"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["disabledCorrespondentCustom"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["disabledCorrespondentCustom"] = value;
            }
        }

        [Browsable(true)]
        public string TxtEtiCustomCorrespondent
        {
            get
            {
                return this.TxtEtiCustomCorrespondentValue.Text;
            }
            set
            {
                this.TxtEtiCustomCorrespondentValue.Text = value;
            }
        }

        public RubricaCallType CallType
        {
            get
            {
                if (HttpContext.Current.Session["callType"] != null)
                    return (RubricaCallType)HttpContext.Current.Session["callType"];
                else return RubricaCallType.CALLTYPE_PROTO_IN;
            }
            set
            {
                HttpContext.Current.Session["callType"] = value;
            }

        }

        private RubricaCallType CalltypeCorrespondentCustom
        {
            get
            {
                return calltypeCorrespondentCustom;
            }
            set
            {
                calltypeCorrespondentCustom = value;
            }
        }

        public string TypeChooseCorrespondent
        {
            get
            {
                return typeChooseCorrespondent;
            }
            set
            {
                typeChooseCorrespondent = value;
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

        private bool EnableAjaxAddressBook
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["enableAjaxAddressBook"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["enableAjaxAddressBook"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["enableAjaxAddressBook"] = value;
            }
        }

        private int AjaxAddressBookMinPrefixLenght
        {
            get
            {
                int result = 3;
                if (HttpContext.Current.Session["ajaxAddressBookMinPrefixLenght"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["ajaxAddressBookMinPrefixLenght"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["ajaxAddressBookMinPrefixLenght"] = value;
            }
        }

        [Browsable(true)]
        public bool sCodiceReadOnly
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["sCodiceReadOnly"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["sCodiceReadOnly"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["sCodiceReadOnly"] = value;
            }
        }

        [Browsable(true)]
        public bool ChkStoryCustomCorrespondentCustom
        {
            get
            {
                return this.ChkStoryCustomCorrespondent.Checked;
            }
            set
            {
                this.ChkStoryCustomCorrespondent.Checked = value;
            }
        }

        [Browsable(true)]
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

        [Browsable(true)]
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

        [Browsable(true)]
        public string SelectedCustomCorrespondentIndex
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["selectedCustomCorrespondentIndex"] != null)
                {
                    result = HttpContext.Current.Session["selectedCustomCorrespondentIndex"].ToString();
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["selectedCustomCorrespondentIndex"] = value;
            }
        }

        [Browsable(true)]
        public string PageCaller
        {
            get
            {
                return pageCaller;
            }
            set
            {
                pageCaller = value;
            }
        }

        public Corrispondente ChooseMultipleCorrespondent
        {
            get
            {
                Corrispondente result = null;
                if (HttpContext.Current.Session["chooseMultipleCorrespondent"] != null)
                {
                    result = HttpContext.Current.Session["chooseMultipleCorrespondent"] as Corrispondente;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["chooseMultipleCorrespondent"] = value;
            }
        }

        private bool SearchCorrespondentIntExtWithDisabled
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["searchCorrespondentIntExtWithDisabled"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["searchCorrespondentIntExtWithDisabled"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["searchCorrespondentIntExtWithDisabled"] = value;
            }
        }

        private void RemovePropertySearchCorrespondentIntExtWithDisabled()
        {
            HttpContext.Current.Session.Remove("searchCorrespondentIntExtWithDisabled");
        }


    }
}