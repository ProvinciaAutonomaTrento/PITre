using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using AjaxControlToolkit;
using SAAdminTool.DocsPaWR;
using System.Collections.Generic;

namespace SAAdminTool.UserControls
{
    public partial class Corrispondente : System.Web.UI.UserControl
    {
        public string TIPO_CORRISPONDENTE = string.Empty;

        private string sCodice = string.Empty;
        private string sCssCodice = string.Empty;
        private string sDescrizione = string.Empty;
        private string sCssDescrizione = string.Empty;
        private int widthCodice = 0;
        private int widthDescrizione = 0;
        private bool sCodiceReadOnly = false;
        private bool sDescrizioneReadOnly = false;
        private bool disabledCorr = false;
        private bool sRicercaAjax = true;
        private ClientScriptManager cs = null;
        public string FILTRO = string.Empty;
        public bool isRiprodotto = false;
        //private bool storicizzato = false;
        protected Dictionary<string, SAAdminTool.DocsPaWR.Corrispondente> dic_Corr;

        protected void Page_Load(object sender, EventArgs e)
        {
            cs = this.Parent.Page.ClientScript;

            if (Session["ricercaCorrispondenteStoricizzato"] != null)
                cbx_storicizzato.Visible = true;

            //ABILITA NUOVA RUBRICA VELOCE MITTENTE ARRIVO
            //MODIFICA GIORDANO 27/03/2012:
            //CODICE NUOVO:
            if (!string.IsNullOrEmpty(utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE_CAMPI_PROF")) && (utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE_CAMPI_PROF")).Equals("1"))
            {
                GetIDBehavior();
            }

            if (!IsPostBack)
            {
                //MODIFICA GIORDANO 27/03/2012:
                //CODICE VECCHIO:
                //if (!string.IsNullOrEmpty(utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")) && (utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")).Equals("1"))
                //{
                //    GetIDBehavior();
                //}
                //FINE MODIFICA GIORDANO 27/03/2012
                this.txt_Codice.CssClass = CSS_CODICE;
                this.txt_Descrizione.CssClass = CSS_DESCRIZIONE;
                this.cbx_storicizzato.CssClass = CSS_DESCRIZIONE;
            }
            //else
            //{
            //MODIFICA GIORDANO 27/03/2012
            //Commento questo codice perchè se la pagina va in postback non si può cancellare il contenuto del Jscript.

            //mittente_veloce.OnClientPopulated = "";
            //mittente_veloce.OnClientItemSelected = "";

            //FINE MODIFICA GIORDANO 27/03/2012
            //}
        }

        protected void GetIDBehavior()
        {
            if (Session["CountCorr"] != null)
            {
                string dataUser = null;
                DocsPaWR.Corrispondente cr = (SAAdminTool.DocsPaWR.Corrispondente)this.Session["userData"];
                System.Web.HttpContext ctx = System.Web.HttpContext.Current;
                if (ctx.Session["userRuolo"] != null)
                    dataUser = ((SAAdminTool.DocsPaWR.Ruolo)ctx.Session["userRuolo"]).systemId;
                if (ctx.Session["userRegistro"] != null)
                    dataUser = dataUser + "-" + ((Registro)ctx.Session["userRegistro"]).systemId;

                string idAmm = cr.idAmministrazione;
                string callType = "CALLTYPE_PROTO_IN";
                int count = 0;
                if (Session["CountCorr"] != null && !string.IsNullOrEmpty(Session["CountCorr"].ToString()))
                    count = Convert.ToInt32(Session["CountCorr"].ToString());
                int j = 0;
                if (Session["whichCorr"] != null && Convert.ToInt32(Session["whichCorr"].ToString()) == count - 1)
                    Session.Remove("whichCorr");
                if (Session["whichCorr"] == null)
                    Session.Add("whichCorr", j);
                else if (Convert.ToInt32(Session["whichCorr"]) < count)
                    Session.Add("whichCorr", Convert.ToInt32(Session["whichCorr"]) + 1);

                int currCorr = Convert.ToInt32(Session["whichCorr"]);
                mittente_veloce.ContextKey = dataUser + "-" + idAmm + "-" + callType;
                mittente_veloce.BehaviorID = "behavior_" + currCorr;
                string b = "behavior_" + currCorr;
                mittente_veloce.OnClientPopulated = "acePopulated" + currCorr;
                mittente_veloce.OnClientItemSelected = "aceSelected" + currCorr;
                string nomeFunzionePopulated = "acePopulated" + currCorr;
                string nomeFunzioneSelected = "aceSelected" + currCorr;
                string unique = this.UniqueID;
                mittente_veloce.Enabled = false;
                builderJS(b, nomeFunzionePopulated, nomeFunzioneSelected, unique);
            }
        }

        protected void GetIDBehaviorProsp()
        {
            if (Session["CountCorr"] != null)
            {
                string dataUser = null;
                DocsPaWR.Corrispondente cr = (SAAdminTool.DocsPaWR.Corrispondente)this.Session["userData"];
                System.Web.HttpContext ctx = System.Web.HttpContext.Current;
                if (ctx.Session["userRuolo"] != null)
                    dataUser = ((SAAdminTool.DocsPaWR.Ruolo)ctx.Session["userRuolo"]).systemId;
                if (ctx.Session["userRegistro"] != null)
                    dataUser = dataUser + "-" + ((Registro)ctx.Session["userRegistro"]).systemId;

                string idAmm = cr.idAmministrazione;
                string callType = "CALLTYPE_PROTO_IN";
                int count = Convert.ToInt32(Session["CountCorr"].ToString());
                int j = 0;
                if (Session["whichCorrProsp"] == null)
                    Session.Add("whichCorrProsp", j);
                else if (Convert.ToInt32(Session["whichCorrProsp"]) < count)
                    Session.Add("whichCorrProsp", Convert.ToInt32(Session["whichCorrProsp"]) + 1);

                int currCorr = Convert.ToInt32(Session["whichCorrProsp"]);
                //mittente_veloce.ContextKey = dataUser + "-" + idAmm + "-" + callType;
                //mittente_veloce.BehaviorID = "behavior_" + currCorr;
                string b = "behavior_" + currCorr;
                //mittente_veloce.OnClientPopulated = "acePopulated" + currCorr;
                //mittente_veloce.OnClientItemSelected = "aceSelected" + currCorr;
                string nomeFunzionePopulated = "acePopulated" + currCorr;
                string nomeFunzioneSelected = "aceSelected" + currCorr;
                string unique = this.UniqueID;
                builderJS(b, nomeFunzionePopulated, nomeFunzioneSelected, unique);
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
            sbPopulated.AppendLine("}");
            sbPopulated.AppendLine("}");
            sbPopulated.AppendLine("}");
            sbPopulated.AppendLine("}");

            //Response.Write(sbPopulated.ToString());
            ScriptManager.RegisterStartupScript(this, this.GetType(), nomeFunzioneSelected + "1", sbPopulated.ToString(), true);

            StringBuilder sbSelected = new StringBuilder();
            sbSelected.AppendLine("function " + nomeFunzioneSelected + "(sender, e) {");
            sbSelected.AppendLine("var value = e.get_value();");
            sbSelected.AppendLine("if (!value) {");
            sbSelected.AppendLine("if (e._item.parentElement && e._item.parentElement.tagName == \"LI\")");
            sbSelected.AppendLine("value = e._item.parentElement.attributes[\"_value\"].value;");
            sbSelected.AppendLine(
                "else if (e._item.parentElement && e._item.parentElement.parentElement.tagName == \"LI\")");
            sbSelected.AppendLine("value = e._item.parentElement.parentElement.attributes[\"_value\"].value;");
            sbSelected.AppendLine("else if (e._item.parentNode && e._item.parentNode.tagName == \"LI\")");
            sbSelected.AppendLine("value = e._item.parentNode._value;");
            sbSelected.AppendLine("else if (e._item.parentNode && e._item.parentNode.parentNode.tagName == \"LI\")");
            sbSelected.AppendLine("value = e._item.parentNode.parentNode._value;");
            sbSelected.AppendLine("else value = \"\";");
            sbSelected.AppendLine("}");
            //sbSelected.AppendLine("alert(2);var searchText = $get('"+uniqueID+"_txt_Descrizione').value;alert(searchText);");
            //sbSelected.AppendLine("searchText = searchText.replace('null', '');");
            sbSelected.AppendLine("var testo = value;");
            sbSelected.AppendLine("var indiceFineCodice = testo.lastIndexOf(')');");
            sbSelected.AppendLine("document.getElementById('" + uniqueID + "_txt_Descrizione').focus();");
            sbSelected.AppendLine("document.getElementById('" + uniqueID + "_txt_Descrizione').value = \"\";");
            sbSelected.AppendLine("var indiceDescrizione = testo.lastIndexOf('(');");
            sbSelected.AppendLine("var descrizione = testo.substr(0, indiceDescrizione - 1);");
            sbSelected.AppendLine("var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);");
            sbSelected.AppendLine("document.getElementById('" + uniqueID + "_txt_Codice').value = codice;");
            sbSelected.AppendLine("document.getElementById('" + uniqueID + "_txt_Descrizione').value = descrizione;");
            //sbSelected.AppendLine("setTimeout(\"__doPostBack('txt_Codice',''), 0\");");
            sbSelected.AppendLine("}");

            ScriptManager.RegisterStartupScript(this, this.GetType(), nomeFunzioneSelected + "2", sbSelected.ToString(), true);

            //Response.Write(sbSelected.ToString());


            //ScriptManager.RegisterStartupScript(((Panel)this.Parent.Page.FindControl("panel_Contenuto")), this.GetType(), "Populated" + i, sbPopulated.ToString(), true);

        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            
            if (Session["dictionaryCorrispondente"] != null)
                dic_Corr = (Dictionary<string, SAAdminTool.DocsPaWR.Corrispondente>)Session["dictionaryCorrispondente"];

            if (dic_Corr != null && dic_Corr.ContainsKey(this.ID) && dic_Corr[this.ID] != null)
            {
                if (Session["multiCorr"] == null)
                {
                    sCodice = dic_Corr[this.ID].codiceRubrica;
                    sDescrizione = dic_Corr[this.ID].descrizione;
                }
            }

            if (dic_Corr != null && dic_Corr.ContainsKey(this.ID) && dic_Corr[this.ID] == null)
            {
                txt_Codice.Text = string.Empty;
                txt_Descrizione.Text = string.Empty;
                sCodice = string.Empty;
                sDescrizione = string.Empty;
            }

            if (UserManager.getCorrispondenteSelezionato(this.Page) != null)
            {
                DocsPaWR.Corrispondente co = UserManager.getCorrispondenteSelezionato(this.Page);
                sCodice = co.codiceRubrica;
                sDescrizione = co.descrizione;
            }

            mittente_veloce.Enabled = sRicercaAjax;

            //Campo Codice
            if (sCodice != "")
                txt_Codice.Text = sCodice;
            if (sCssCodice != "")
                txt_Codice.CssClass = sCssCodice;
            if (sCodiceReadOnly)
            {
                txt_Codice.ReadOnly = sCodiceReadOnly;
                btn_Rubrica.Enabled = false;
            }
            if (widthCodice != 0)
                txt_Codice.Width = widthCodice;

            //Campo Descrizione
            if (sDescrizione != "")
                txt_Descrizione.Text = sDescrizione;
            if (sCssDescrizione != "")
                txt_Descrizione.CssClass = sCssDescrizione;

            txt_Descrizione.ReadOnly = sDescrizioneReadOnly;
            if (widthDescrizione != 0)
                txt_Descrizione.Width = widthDescrizione;

            if (disabledCorr)
            {
                //this.txt_Codice.Text = string.Empty;
                //this.txt_Descrizione.Text = string.Empty;
                this.txt_Codice.Enabled = false;
                this.txt_Descrizione.Enabled = false;
                this.btn_Rubrica.Enabled = false;
                this.mittente_veloce.Enabled = false;
            }
            else
            {
                this.txt_Codice.Enabled = true;
                this.txt_Descrizione.Enabled = true;
                this.btn_Rubrica.Enabled = true;
                this.mittente_veloce.Enabled = true;
            }

        }

        public bool DISABLED_CORR
        {
            get
            {
                return disabledCorr;
            }
            set
            {
                disabledCorr = value;
            }
        }


        public int WIDTH_CODICE
        {
            get
            {
                return widthCodice;
            }
            set
            {
                widthCodice = value;
            }
        }

        public int WIDTH_DESCRIZIONE
        {
            get
            {
                return widthDescrizione;
            }
            set
            {
                widthDescrizione = value;
            }
        }

        public string CSS_CODICE
        {
            get
            {
                return sCssCodice;
            }
            set
            {
                sCssCodice = value;
            }
        }

        public string CSS_DESCRIZIONE
        {
            get
            {
                return sCssDescrizione;
            }
            set
            {
                sCssDescrizione = value;
            }
        }

        public string TIPO
        {
            get
            {
                if (this.ViewState["TIPO"] != null)
                    return this.ViewState["TIPO"].ToString();
                else
                    return string.Empty;
            }
            protected set
            {
                this.ViewState["TIPO"] = value;
            }
        }

        public string ID_CORRISPONDENTE
        {
            get
            {
                if (this.ViewState["ID_CORRISPONDENTE"] != null)
                    return this.ViewState["ID_CORRISPONDENTE"].ToString();
                else
                    return string.Empty;
            }
            set
            {
                this.ViewState["ID_CORRISPONDENTE"] = value;
            }
        }

        public string SYSTEM_ID_CORR
        {
            get
            {
                return txt_SystemId.Value;
            }
            set
            {
                txt_SystemId.Value = value;
            }
        }

        public string CODICE_TEXT
        {
            get
            {
                return txt_Codice.Text;
            }
            set
            {
                sCodice = value;
                txt_Codice.Text = value;
            }
        }

        public bool RICERCA_AJAX
        {
            get
            {
                return sRicercaAjax;
            }
            set
            {
                sRicercaAjax = value;
            }
        }

        public string DESCRIZIONE_TEXT
        {
            get
            {
                return txt_Descrizione.Text;
            }
            set
            {
                sDescrizione = value;
                txt_Descrizione.Text = value;
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

        public bool DESCRIZIONE_READ_ONLY
        {
            get
            {
                return sDescrizioneReadOnly;
            }
            set
            {
                sDescrizioneReadOnly = value;
            }
        }

        public bool IS_RIPRODOTTO
        {
            get
            {
                return isRiprodotto;
            }
            set
            {
                isRiprodotto = value;
            }
        }

        protected void btn_Rubrica_Click(object sender, ImageClickEventArgs e)
        {
            //In questo caso il campo è readOnly.
            //Controllo solo il codice perchè la descrizione è sempre readOnly,
            //l'apertura della rubrica è quindi inutile in quanto non si può
            //modificare il valore di questo userControl
            if (sCodiceReadOnly)
                return;

            if (Session["dictionaryCorrispondente"] != null)
                dic_Corr = (Dictionary<string, SAAdminTool.DocsPaWR.Corrispondente>)Session["dictionaryCorrispondente"];

            if (dic_Corr != null && dic_Corr.ContainsKey(this.ID))
                dic_Corr[this.ID] = null;

            SAAdminTool.DocsPaWR.Ruolo ruoloSelezionato = UserManager.getRuolo();
            //Se in sessione c'è un ruolo, vuol dire che sono sul frontend e quindi la
            //rubrica puo' funzionare, in quanto preleva da sessione il registro, il ruolo e
            //l'utente che la vorrebbero utilizzando.
            //In caso contrario, vuol dire che sto chiamando l'apertura della rubrica
            //dall'amministrazione, in particolare da una delle due anteprime di profilazione.
            //In questo caso non essendoci nessun registro, nè ruolo nè utente in sessione,
            //l'apertura della rubrica viene inibita, ma non è un problema in quanto essendo
            //un antemprima, serve solo per determinare una veste grafica gradevole per tipo
            //di documento o di fascicolo che si sta creando.            
            if (ruoloSelezionato != null)
            {
                if (TIPO_CORRISPONDENTE != null)
                {
                    switch (TIPO_CORRISPONDENTE)
                    {
                        case "INTERNI":
                            if (!string.IsNullOrEmpty(FILTRO) && FILTRO.Equals("NO_UO"))
                            {
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "aperturaRubrica", "apriRubrica('CALLTYPE_CORR_INT_NO_UO');", true);
                            }
                            else
                            {
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "aperturaRubrica", "apriRubrica('CALLTYPE_CORR_INT');", true);
                            }
                            break;

                        case "ESTERNI":
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "aperturaRubrica", "apriRubrica('CALLTYPE_CORR_EST');", true);
                            break;

                        case "INTERNI/ESTERNI":
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "aperturaRubrica", "apriRubrica('CALLTYPE_CORR_INT_EST');", true);
                            break;
                        default:
                            if(this.cbx_storicizzato.Checked)
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "aperturaRubrica", "apriRubrica('CALLTYPE_CORR_NO_FILTRI');", true);
                            else
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "aperturaRubrica", "apriRubrica('CALLTYPE_RICERCA_CORR_NON_STORICIZZATO');", true);
                            break;
                    }
                }
            }

            //Aggiungo in sessione l'id del campo, perchè poichè in una popup di profilazione
            //questo tipo di campo puo' essere ripetuto, devo riuscire a distinguerlo
            Session.Add("rubrica.idCampoCorrispondente", this.ID);
        }

        protected void cbx_storicizzato_checkedChanged(object sender, EventArgs e)
        {
            //if (this.cbx_storicizzato.Checked)
            //{
            //    this.storicizzato = true;
            //    if (Session["ricercaCorrispondenteStoricizzato"] == null)
            //        Session.Add("ricercaCorrispondenteStoricizzato", true);
            //}
            //else
            //{
            //    this.storicizzato = false;
            //    if (Session["ricercaCorrispondenteStoricizzato"] != null)
            //        Session.Remove("ricercaCorrispondenteStoricizzato");
            //}
        }

        protected void txt_Codice_TextChanged(object sender, EventArgs e)
        {
            bool eseguiRicerca = true;
            if (Session["dictionaryCorrispondente"] != null)
            {
                dic_Corr = (Dictionary<string, SAAdminTool.DocsPaWR.Corrispondente>)Session["dictionaryCorrispondente"];
                if(dic_Corr.ContainsKey(this.ID))
                {
                    SAAdminTool.DocsPaWR.Corrispondente corr_saved = dic_Corr[this.ID];
                    if(dic_Corr[this.ID] != null && corr_saved.codiceRubrica.ToUpper().Equals(txt_Codice.Text.ToUpper()))
                        eseguiRicerca = false;
                    if (eseguiRicerca && Session["noRicercaCodice"] != null)
                    {
                        eseguiRicerca = false;
                        Session.Remove("noRicercaCodice");
                    }
                }
            }
            else
                dic_Corr = new Dictionary<string, DocsPaWR.Corrispondente>();

            if (Session["noDoppiaRicerca"] == null)
            {
                //caso di annullamento dei dati inseriti
                if (txt_Codice.Text.Equals("") && eseguiRicerca)
                {
                    eseguiRicerca = false;
                    dic_Corr[this.ID] = null;
                    Session["dictionaryCorrispondente"] = dic_Corr;
                    txt_Descrizione.Text = "";
                    //Session.Add("resetCorrispondente", true);
                }
                
                if (Session["rubrica.campoCorrispondente"] == null && eseguiRicerca)
                {
                    ElementoRubrica[] listaCorr = null;

                    //In questo caso il campo è readOnly.
                    //Controllo solo il codice perchè la descrizione è sempre readOnly,
                    //l'apertura della rubrica è quindi inutile in quanto non si può
                    //modificare il valore di questo userControl
                    if (sCodiceReadOnly)
                        return;

                    SAAdminTool.DocsPaWR.Ruolo ruoloSelezionato = UserManager.getRuolo();

                    DocsPaWR.Registro[] regAll = UserManager.getListaRegistriWithRF(ruoloSelezionato.systemId, "", "");
                    string condRegistri = string.Empty;
                    if (regAll != null && regAll.Length > 0)
                    {
                        condRegistri = " and (id_registro in (";
                        foreach (DocsPaWR.Registro reg in regAll)
                            condRegistri += reg.systemId + ",";
                        condRegistri = condRegistri.Substring(0, condRegistri.Length - 1);
                        condRegistri += ") OR id_registro is null)";
                    }

                    //Se in sessione c'è un ruolo, vuol dire che sono sul frontend e quindi la
                    //rubrica puo' funzionare, in quanto preleva da sessione il registro, il ruolo e
                    //l'utente che la vorrebbero utilizzando.
                    //In caso contrario, vuol dire che sto chiamando l'apertura della rubrica
                    //dall'amministrazione, in particolare da una delle due anteprime di profilazione.
                    //In questo caso non essendoci nessun registro, nè ruolo nè utente in sessione,
                    //l'apertura della rubrica viene inibita, ma non è un problema in quanto essendo
                    //un antemprima, serve solo per determinare una veste grafica gradevole per tipo
                    //di documento o di fascicolo che si sta creando.            
                    if (ruoloSelezionato != null)
                    {
                        if ((sCodice != null && sCodice != "") || (txt_Codice.Text != null && txt_Codice.Text != ""))
                        {
                            listaCorr = UserManager.getElementiRubricaMultipli(this.Page, txt_Codice.Text, RubricaCallType.CALLTYPE_RICERCA_CORRISPONDENTE, !this.cbx_storicizzato.Checked);

                            if (listaCorr == null || (listaCorr != null && listaCorr.Length == 0))
                            {
                                this.TIPO = "";
                                this.ID_CORRISPONDENTE = "";
                                sCodice = txt_Codice.Text;
                                sDescrizione = null;
                                dic_Corr[this.ID] = null;
                                Session.Add("dictionaryCorrispondente", dic_Corr);
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "noCorr", String.Format("alert('Nessun corrispondente individuato con il codice {0}');", this.txt_Codice.Text), true);
                            }
                            else
                            {
                                if (listaCorr != null && listaCorr.Length > 1)
                                {
                                    dic_Corr[this.ID] = null;
                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "multiCorr", "ApriFinestraMultiCorrispondenti('" + this.txt_Codice.ClientID + "','" + this.txt_Descrizione.ClientID + "', '" + this.ID + "');", true);
                                    Session.Add("multiCorr", listaCorr);
                                    Session["noRicercaDesc"] = true;
                                    Session["idCorrMulti"] = Convert.ToInt32(this.ID);
                                    return;
                                }
                                else
                                {
                                    if (listaCorr != null && listaCorr.Length == 1) // && !this.cbx_storicizzato.Checked)
                                    {
                                        SAAdminTool.DocsPaWR.ElementoRubrica er = listaCorr[0];
                                        if (this.cbx_storicizzato.Checked)
                                            dic_Corr[this.ID] = UserManager.getCorrispondenteBySystemIDDisabled(this.Page, er.systemId);
                                        else
                                            dic_Corr[this.ID] = UserManager.getCorrispondenteByCodRubricaIENotdisabled(this.Page, er.codice, AddressbookTipoUtente.GLOBALE);
                                        this.TIPO = er.tipo;
                                        if (dic_Corr[this.ID] != null)
                                        {
                                            this.ID_CORRISPONDENTE = er.systemId;
                                            txt_Descrizione.Text = er.descrizione;
                                            sDescrizione = er.descrizione;
                                            txt_Codice.Text = er.codice;
                                            sCodice = er.codice;
                                        }
                                        else
                                        {
                                            this.ID_CORRISPONDENTE = string.Empty;
                                            txt_Descrizione.Text = string.Empty;
                                            sDescrizione = string.Empty;
                                            txt_Codice.Text = string.Empty;
                                            sCodice = string.Empty;
                                        }
                                        Session.Add("dictionaryCorrispondente", dic_Corr);
                                        Session["noRicercaDesc"] = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
                Session.Remove("noDoppiaRicerca");

            if (Session["countCorr"] != null && Convert.ToInt32(Session["countCorr"]) > 1)
                Session.Remove("whichCorr");

            if (isRiprodotto)
            {
                Session.Remove("CorrSelezionatoDaMulti");
                Session.Remove("rubrica.campoCorrispondente");
            }
        }

        protected void txt_Descrizione_TextChanged(object sender, EventArgs e)
        {
            //In questo caso il campo è readOnly.
            //Controllo solo il codice perchè la descrizione è sempre readOnly,
            //l'apertura della rubrica è quindi inutile in quanto non si può
            //modificare il valore di questo userControl
            bool eseguiRicerca = true;
            if (Session["dictionaryCorrispondente"] != null)
            {
                dic_Corr = (Dictionary<string, SAAdminTool.DocsPaWR.Corrispondente>)Session["dictionaryCorrispondente"];
                if (dic_Corr.ContainsKey(this.ID))
                {
                    SAAdminTool.DocsPaWR.Corrispondente corr_saved = dic_Corr[this.ID];
                    if (dic_Corr[this.ID] != null && corr_saved.codiceRubrica.ToUpper().Equals(txt_Codice.Text.ToUpper()))
                        eseguiRicerca = false;
                    if (eseguiRicerca && Session["noRicercaDesc"] != null)
                    {
                        eseguiRicerca = false;
                        Session.Remove("noRicercaDesc");
                    }
                }
            }
            else
                dic_Corr = new Dictionary<string, DocsPaWR.Corrispondente>();

            if (Session["noDoppiaRicerca"] == null)
            {
                //caso di annullamento dei dati inseriti
                if (txt_Descrizione.Text.Equals("") && eseguiRicerca)
                {
                    eseguiRicerca = false;
                    dic_Corr[this.ID] = null;
                    Session.Add("dictionaryCorrispondente", dic_Corr);
                    txt_Codice.Text = "";
                }
                
                if (Session["rubrica.campoCorrispondente"] == null && eseguiRicerca)
                {
                    SAAdminTool.DocsPaWR.Ruolo ruoloSelezionato = UserManager.getRuolo();

                    DocsPaWR.Registro[] regAll = UserManager.getListaRegistriWithRF(ruoloSelezionato.systemId, "", "");
                    string condRegistri = string.Empty;
                    string condRegistri2 = string.Empty;
                    if (regAll != null && regAll.Length > 0)
                    {
                        foreach (DocsPaWR.Registro reg in regAll)
                            condRegistri2 += reg.systemId + ",";

                        condRegistri = " and (id_registro in (" + condRegistri2.Substring(0, condRegistri2.Length - 1) + ") OR id_registro is null)";
                        condRegistri2 = condRegistri2.Substring(0, condRegistri2.Length - 1);
                    }

                    //Se in sessione c'è un ruolo, vuol dire che sono sul frontend e quindi la
                    //rubrica puo' funzionare, in quanto preleva da sessione il registro, il ruolo e
                    //l'utente che la vorrebbero utilizzando.
                    //In caso contrario, vuol dire che sto chiamando l'apertura della rubrica
                    //dall'amministrazione, in particolare da una delle due anteprime di profilazione.
                    //In questo caso non essendoci nessun registro, nè ruolo nè utente in sessione,
                    //l'apertura della rubrica viene inibita, ma non è un problema in quanto essendo
                    //un antemprima, serve solo per determinare una veste grafica gradevole per tipo
                    //di documento o di fascicolo che si sta creando.            
                    if (ruoloSelezionato != null)
                    {
                        if ((txt_Descrizione.Text != null && txt_Descrizione.Text != ""))
                        {
                            ParametriRicercaRubrica qco = new ParametriRicercaRubrica();
                            qco.caller = new RubricaCallerIdentity();
                            qco.caller.IdUtente = UserManager.getInfoUtente().idPeople;
                            qco.caller.filtroRegistroPerRicerca = condRegistri2;
                            qco.descrizione = txt_Descrizione.Text;
                            qco.doUo = true;
                            qco.doRuoli = true;
                            qco.doUtenti = true;
                            qco.tipoIE = AddressbookTipoUtente.GLOBALE;
                            if (this.cbx_storicizzato.Checked)
                                qco.calltype = RubricaCallType.CALLTYPE_RICERCA_CORRISPONDENTE;
                            else
                                qco.calltype = RubricaCallType.CALLTYPE_CORR_INT_EST;

                            SAAdminTool.DocsPaWR.ElementoRubrica[] er = UserManager.getElementiRubrica(this.Page, qco); // .getElementoRubrica(this.Page, txt_Codice.Text, condRegistri);
                            if (er != null)
                            {
                                if (er.Length == 0)
                                {
                                    this.TIPO = "";
                                    this.ID_CORRISPONDENTE = "";
                                    sDescrizione = txt_Descrizione.Text;
                                    sCodice = null;
                                    dic_Corr[this.ID] = null;
                                    Session.Add("dictionaryCorrispondente", dic_Corr);
                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "noCorr", String.Format("alert('Nessun corrispondente individuato con la descrizione {0}');", this.txt_Descrizione.Text.Replace("'","''")), true);
                                }
                                else
                                {
                                    if (er.Length == 1)
                                    {
                                        this.TIPO = er[0].tipo;
                                        this.ID_CORRISPONDENTE = er[0].systemId;
                                        Session["noDoppiaRicerca"] = true;
                                        txt_Descrizione.Text = er[0].descrizione;
                                        sDescrizione = er[0].descrizione;
                                        txt_Codice.Text = er[0].codice;
                                        sCodice = er[0].codice;
                                        dic_Corr[this.ID] = UserManager.getCorrispondenteBySystemIDDisabled(this.Page, er[0].systemId);
                                        Session.Add("dictionaryCorrispondente", dic_Corr);
                                        Session["noRicercaCodice"] = true;
                                    }
                                    else
                                    {
                                        // apro popup per la scelta dei corrispondenti
                                        dic_Corr[this.ID] = null;
                                        Page.ClientScript.RegisterStartupScript(this.GetType(), "multiCorr", "ApriFinestraMultiCorrispondenti('" + this.txt_Codice.ClientID + "','" + this.txt_Descrizione.ClientID + "');", true);
                                        Session.Add("multiCorr", er);
                                        Session["noRicercaCodice"] = true;
                                        Session["idCorrMulti"] = Convert.ToInt32(this.ID);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
                Session.Remove("noDoppiaRicerca");
            
            if (Session["countCorr"] != null && Convert.ToInt32(Session["countCorr"]) > 1)
                Session.Remove("whichCorr");

            if (isRiprodotto)
            {
                Session.Remove("CorrSelezionatoDaMulti");
                Session.Remove("rubrica.campoCorrispondente");
            }
        }
    }
}