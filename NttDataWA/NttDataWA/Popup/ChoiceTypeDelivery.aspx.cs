using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using NttDataWA.UIManager;
using NttDatalLibrary;

namespace NttDataWA.Popup
{
    public partial class ChoiceTypeDelivery : System.Web.UI.Page
    {

        #region Properties

        private SchedaDocumento DocumentInWorking
        {
            get
            {
                SchedaDocumento result = null;
                if (HttpContext.Current.Session["document"] != null)
                {
                    result = HttpContext.Current.Session["document"] as SchedaDocumento;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["document"] = value;
            }
        }

        public DocsPaWR.Corrispondente Sender
        {
            get
            {
                DocsPaWR.Corrispondente result = null;
                if (HttpContext.Current.Session["sender"] != null)
                {
                    result = HttpContext.Current.Session["sender"] as DocsPaWR.Corrispondente;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["sender"] = value;
            }
        }

        public DocsPaWR.Corrispondente SenderIntermediate
        {
            get
            {
                DocsPaWR.Corrispondente result = null;
                if (HttpContext.Current.Session["SenderIntermediate"] != null)
                {
                    result = HttpContext.Current.Session["SenderIntermediate"] as DocsPaWR.Corrispondente;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["SenderIntermediate"] = value;
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

        public bool editMode
        {
            get
            {
                if (Session["ChoiceTypeDelivery_editMode"] == null) return false;
                return (bool)Session["ChoiceTypeDelivery_editMode"];
            }
            set
            {
                Session["ChoiceTypeDelivery_editMode"] = value;
            }
        }

        public List<Mezzi> listaMezzoSpedizione
        {
            get
            {

                if (HttpContext.Current.Session["listaMezzoSpedizione"] != null)
                {
                    return HttpContext.Current.Session["listaMezzoSpedizione"] as List<Mezzi>;
                }
                else
                {
                    return new List<Mezzi>();
                }
            }
            set
            {
                HttpContext.Current.Session["listaMezzoSpedizione"] = value;
            }
        }

        /// <summary>
        /// Per rendere possibile la modifica del mezzo di spedizione.
        /// </summary>
        public string bypassaControlloRicevute
        {
            get
            {
                if (HttpContext.Current.Session["bypassaControlloRicevute2032"] != null)
                {
                    return HttpContext.Current.Session["bypassaControlloRicevute2032"].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (value != null)
                    HttpContext.Current.Session["bypassaControlloRicevute2032"] = value;
                else
                    HttpContext.Current.Session["bypassaControlloRicevute2032"] = string.Empty;
            }

        }

        public bool modificaDestinatari
        {
            get
            {
                return (Boolean)HttpContext.Current.Session["modDestinatariPerCambioCanale20140221"];
            }

            set
            {

                HttpContext.Current.Session["modDestinatariPerCambioCanale20140221"] = value;

            }
        }

        public Dictionary<string, string> TypeDelivery
        {
            get
            {
                Dictionary<String, String> result = null;
                if (HttpContext.Current.Session["TypeDelivery"] != null)
                {
                    result = HttpContext.Current.Session["TypeDelivery"] as Dictionary<String, String>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TypeDelivery"] = value;
            }
        }

        private int SelectedPage
        {
            get
            {
                int toReturn = 1;
                if (HttpContext.Current.Session["selectedPageTypeDelivery"] != null) Int32.TryParse(HttpContext.Current.Session["selectedPageTypeDelivery"].ToString(), out toReturn);
                if (toReturn < 1) toReturn = 1;

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["selectedPageTypeDelivery"] = value;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.Page.IsPostBack)
                {
                    this.InitLanguage();
                    HttpContext.Current.Session.Remove("selectedPageTypeDelivery");
                    TypeDelivery = new Dictionary<string, string>();
                    //preparo il datasource con i mezzi trasmissione
                    DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
                    string idAmm = UserManager.GetInfoUser().idAmministrazione;
                    DocsPaWR.MezzoSpedizione[] m_sped = ws.AmmListaMezzoSpedizione(idAmm, false);

                    List<Mezzi> listaMS = new List<Mezzi>();
                    foreach (DocsPaWR.MezzoSpedizione m in m_sped)
                    {
                        // Il mezzo per l'interoperabilità semplificata può essere inserito solo se è attiva
                        if (m.chaTipoCanale == "S")
                        {
                            if (SimplifiedInteroperabilityManager.IsEnabledSimpInterop)
                                listaMS.Add(new Mezzi(m.Descrizione, m.IDSystem));
                        }
                        else
                            listaMS.Add(new Mezzi(m.Descrizione, m.IDSystem));
                    }
                    this.listaMezzoSpedizione = listaMS;

                    //setto il mezzo di spedizione per tutti
                    this.ddlAll.Items.Add(new ListItem("", "0"));
                    if (listaMS != null)
                    {
                        foreach (Mezzi m in listaMS)
                            this.ddlAll.Items.Add(new ListItem(m.Descrizione, m.Valore));

                        if (!editMode)
                            this.ddlAll.Enabled = false;
                    }

                    this.BindGrid();

                    if (!editMode)
                        this.BtnOk.Enabled = false;
                }
                else
                {
                    SetTypeDelivery();
                    if (!string.IsNullOrEmpty(this.grid_pageindex.Value) && this.SelectedPage != int.Parse(this.grid_pageindex.Value))
                    {
                        this.SelectedPage = int.Parse(this.grid_pageindex.Value);
                        this.BindGrid();
                    }

                    // riposiziono lo scroll del div in cima
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ScrollGridViewOnTop", "setFocusOnTop() ", true);
                }

                this.ReApplyScripts();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void InitLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnOk.Text = Utils.Languages.GetLabelFromCode("GenericBtnOk", language);
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("GenericBtnClose", language);
            this.lblAll.Text = Utils.Languages.GetLabelFromCode("ChoiceTypeDeliveryAll", language);
            this.ddlAll.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("ChoiceTypeDeliverySelect", language));

            this.gvList.Columns[0].HeaderText = Utils.Languages.GetLabelFromCode("ChoiceTypeDeliveryRecipient", language);
            this.gvList.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("ChoiceTypeDeliveryId", language);
            this.gvList.Columns[2].HeaderText = Utils.Languages.GetLabelFromCode("ChoiceTypeDeliveryMeanOfTransport", language);
        }

        private void ReApplyScripts()
        {
            this.ReApplyChosenScript();
        }

        private void ReApplyChosenScript()
        {
            string label = this.GetLabel("GenericNoResults");
            ScriptManager.RegisterStartupScript(this, this.GetType(), "chosen_deselect", "$('.chzn-select-deselect').chosen({ allow_single_deselect: true, no_results_text: '" + utils.FormatJs(label) + "' });", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "chosen", "$('.chzn-select').chosen({ no_results_text: '" + utils.FormatJs(label) + "' });", true);
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            try
            {
                this.CloseMask(false);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void ClearSessionData()
        {
            this.editMode = false;
            this.listaMezzoSpedizione = null;
        }

        protected void CloseMask(bool withReturnValue)
        {
            this.ClearSessionData();

            string returnValue = "";
            if (withReturnValue) returnValue = "true";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('ChoiceTypeDelivery', '" + returnValue + "');} else {parent.closeAjaxModal('ChoiceTypeDelivery', '" + returnValue + "');};", true);
        }

        private string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
        }

        public void BindGrid()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            //preparo i destinatari/destinatari cc
            List<Cols> columns = new List<Cols>();
            DocsPaWR.Corrispondente[] listaCorrispondenti;
            DocsPaWR.Corrispondente[] listaCorrispondentiCc;
            listaCorrispondenti = this.ListRecipients.ToArray();
            listaCorrispondentiCc = this.ListRecipientsCC.ToArray();
            int i = (this.gvList.PageSize * (this.SelectedPage - 1));
            if (listaCorrispondenti != null)
            {
                while (i < ((this.gvList.PageSize * (SelectedPage - 1)) + this.gvList.PageSize) && i < listaCorrispondenti.Count())
                {
                    Corrispondente c = listaCorrispondenti[i];
                    if (!string.IsNullOrEmpty(c.systemId))
                    {
                        //Canale canalePreferenz = UserManager.getCorrispondentBySystemID(c.systemId).canalePref;
                        Canale canalePreferenz = UserManager.GetCanalePreferenzialeByIdCorr(c.systemId);
                        if (canalePreferenz != null && (!string.IsNullOrEmpty(canalePreferenz.descrizione)))
                            columns.Add(new Cols("(" + canalePreferenz.descrizione + ") " + c.descrizione, c.systemId));
                        else
                            columns.Add(new Cols(c.descrizione, c.systemId));
                    }
                    i++;
                }
            }
            if (listaCorrispondentiCc != null)
            {
                int j = listaCorrispondenti != null ? (i - listaCorrispondenti.Count()) : i;
                while (i < ((this.gvList.PageSize * (SelectedPage - 1)) + this.gvList.PageSize) && j < listaCorrispondentiCc.Count())
                {
                    Corrispondente c = listaCorrispondentiCc[j];
                    if (!string.IsNullOrEmpty(c.systemId))
                    {
                        //Canale canalePreferenz = UserManager.getCorrispondentBySystemID(c.systemId).canalePref;
                        Canale canalePreferenz = UserManager.GetCanalePreferenzialeByIdCorr(c.systemId);
                        if (canalePreferenz != null && (!string.IsNullOrEmpty(canalePreferenz.descrizione)))

                            columns.Add(new Cols("(Cc)(" + c.canalePref.descrizione + ") " + c.descrizione, c.systemId));
                        else
                            columns.Add(new Cols("(Cc)  " + c.descrizione, c.systemId));
                    }
                    i++;
                    j++;
                }

            }

            /* PagedDataSource dsP = new PagedDataSource();
             dsP.DataSource = columns;
             dsP.AllowPaging = true;
             if ((this.ListRecipients.Count + ListRecipientsCC.Count) > 0 && ((float)(this.ListRecipients.Count + ListRecipientsCC.Count) / this.gvList.PageSize) <= (this.SelectedPage - 1))
                 SelectedPage = ((this.ListRecipients.Count + ListRecipientsCC.Count) % this.gvList.PageSize) > 0 ?
                     ((this.ListRecipients.Count + ListRecipientsCC.Count) / this.gvList.PageSize) + 1 : ((this.ListRecipients.Count + ListRecipientsCC.Count) / this.gvList.PageSize);
             else if (string.IsNullOrEmpty(this.grid_pageindex.Value) || (this.ListRecipients.Count + ListRecipientsCC.Count) == 0)
                 SelectedPage = 1;
             //SelectedPage -= 1;
             dsP.CurrentPageIndex = SelectedPage - 1;
             dsP.PageSize = this.gvList.PageSize;
             * */
            this.gvList.DataSource = columns;
            this.gvList.DataBind();
            this.BuildGridNavigator();
            this.UpGrid.Update();
            this.upPnlGridIndexes.Update();
        }


        protected void gridViewResult_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                this.gvList.PageIndex = e.NewPageIndex;
                BindGrid();
                this.UpPnlContainer.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void BuildGridNavigator()
        {
            try
            {
                this.plcNavigator.Controls.Clear();

                int countPage = (int)Math.Round(((double)((this.ListRecipients.Count + ListRecipientsCC.Count)) / (double)this.gvList.PageSize) + 0.49); //(this.ListRecipients.Count + ListRecipientsCC.Count) % this.gvList.PageSize;

                int numPage = SelectedPage;
                if (countPage > 1)
                {
                    Panel panel = new Panel();
                    panel.EnableViewState = true;
                    panel.CssClass = "recordNavigator2";

                    int startFrom = 1;
                    if (numPage > 6) startFrom = numPage - 5;

                    int endTo = 10;
                    if (numPage > 6) endTo = numPage + 5;
                    if (endTo > countPage) endTo = countPage;

                    if (startFrom > 1)
                    {
                        LinkButton btn = new LinkButton();
                        btn.EnableViewState = true;
                        btn.Text = "...";
                        btn.Attributes["onclick"] = " $('#grid_pageindex').val(" + (startFrom - 1) + ");disallowOp('Content2'); __doPostBack('upPnlGridIndexes', ''); return false;";
                        panel.Controls.Add(btn);
                    }

                    for (int i = startFrom; i <= endTo; i++)
                    {
                        if (i == numPage)
                        {
                            Literal lit = new Literal();
                            lit.Text = "<span>" + i.ToString() + "</span>";
                            panel.Controls.Add(lit);
                        }
                        else
                        {
                            LinkButton btn = new LinkButton();
                            btn.EnableViewState = true;
                            btn.Text = i.ToString();
                            btn.Attributes["onclick"] = " $('#grid_pageindex').val($(this).text());disallowOp('Content2'); __doPostBack('upPnlGridIndexes', ''); return false;";
                            panel.Controls.Add(btn);
                        }
                    }

                    if (endTo < countPage)
                    {
                        LinkButton btn = new LinkButton();
                        btn.EnableViewState = true;
                        btn.Text = "...";
                        btn.Attributes["onclick"] = " $('#grid_pageindex').val(" + endTo + ");disallowOp('Content2'); __doPostBack('upPnlGridIndexes', ''); return false;";
                        panel.Controls.Add(btn);
                    }

                    this.plcNavigator.Controls.Add(panel);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void gvList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                DocsPaWR.Corrispondente[] dest = this.ListRecipients.ToArray();
                DocsPaWR.Corrispondente[] destCc = this.ListRecipientsCC.ToArray();
                //id del corrispondente
                Label idCorr = (Label)e.Row.FindControl("lblId");
                //popola la dropddownlist  
                DropDownList ddl1 = (DropDownList)e.Row.FindControl("ddlM");
                Canale canale = null, canaleOrig = null;
                string tipoDest = string.Empty;
                Corrispondente corr = null;
                if (ddl1 != null)
                {
                    //leggo il canale in sessione associato al corrispondente
                    if (dest != null && dest.Length > 0)
                    {
                        /*
                        foreach (Corrispondente d in dest)
                        {
                            if (!string.IsNullOrEmpty(d.systemId) && d.systemId.Equals(idCorr.Text))
                            {
                                canale = d.canalePref;
                                tipoDest = "d";
                                break;
                            }
                        }
                         * */
                        corr = (from d in dest where !string.IsNullOrEmpty(d.systemId) && d.systemId.Equals(idCorr.Text) select d).FirstOrDefault();
                        if (corr != null)
                        {
                            canale = corr.canalePref;
                            tipoDest = "d";
                        }
                    }
                    if (destCc != null && destCc.Length > 0 && (!tipoDest.Equals("d")))
                    {
                        /*
                        foreach (Corrispondente d in destCc)
                        {
                            if (!string.IsNullOrEmpty(d.systemId) && d.systemId.Equals(idCorr.Text))
                            {
                                canale = d.canalePref;
                                break;
                            }
                        }
                         * */
                        corr = (from d in destCc where !string.IsNullOrEmpty(d.systemId) && d.systemId.Equals(idCorr.Text) select d).FirstOrDefault();
                        if (corr != null)
                        {
                            canale = corr.canalePref;
                        }
                    }

                    ddl1.Items.Add(new ListItem("", "0"));
                    if (listaMezzoSpedizione != null)
                    {
                        if (corr.tipoIE == null ||
                            corr.tipoIE.Equals("I"))
                            ddl1.Enabled = false;
                        else
                        {
                            canaleOrig = UserManager.GetCanalePreferenzialeByIdCorr(idCorr.Text);
                            List<Mezzi> filteredMeans = this.GetMeansDeliveryFiltered(canaleOrig, corr);
                            if (filteredMeans != null && filteredMeans.Count > 0)
                            {
                                foreach (Mezzi m in filteredMeans)
                                {
                                    ddl1.Items.Add(new ListItem(m.Descrizione, m.Valore));
                                    if (canale != null && canale.systemId != null)
                                    {
                                        if (canaleOrig == null || (canaleOrig != null && canaleOrig.systemId != null && canaleOrig.systemId != canale.systemId &&
                                            (!TypeDelivery.ContainsKey(corr.systemId) || !string.IsNullOrEmpty(TypeDelivery[corr.systemId]))))
                                        {
                                            if (m.Valore.Equals(canale.systemId))
                                            {
                                                ddl1.SelectedValue = canale.systemId;
                                            }
                                        }
                                        if (TypeDelivery.ContainsKey(corr.systemId) && !string.IsNullOrEmpty(TypeDelivery[corr.systemId]))
                                        {
                                            if (m.Valore.Equals(TypeDelivery[corr.systemId]))
                                            {
                                                ddl1.SelectedValue = TypeDelivery[corr.systemId];
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (!editMode)
                            ddl1.Enabled = false;
                    }

                    ddl1.Attributes.Add("data-placeholder", this.GetLabel("ChoiceTypeDeliverySelect"));
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private List<Mezzi> GetMeansDeliveryFiltered(Canale canaleOrig, Corrispondente corr)
        {
            DocsPaWR.DocsPaWebService wws = new DocsPaWebService();
            DocsPaWR.MezzoSpedizione[] m_sped = wws.AmmListaMezzoSpedizione(UserManager.GetInfoUser().idAmministrazione, false);
            List<Mezzi> filteredMeans = new List<Mezzi>();
            //Corrispondente corr = UserManager.getCorrispondentBySystemID(idDest);
            if (canaleOrig != null)
            {
                switch (canaleOrig.typeId.ToUpper())
                {

                    case "INTEROPERABILITA":
                        foreach (MezzoSpedizione m in m_sped)
                        {
                            // Il canale interoperabilità semplificata non può essere inserito se il corrispondente non
                            // ha URL mentre il canale mail non deve essere inserito
                            if ((m.chaTipoCanale == "S" && SimplifiedInteroperabilityManager.IsEnabledSimpInterop &&
                                corr.Url != null && corr.Url.Length > 0 &&
                                Uri.IsWellFormedUriString(corr.Url[0].Url, UriKind.Absolute)) ||
                                (m.chaTipoCanale != "S" && !m.Descrizione.ToUpper().Equals("MAIL")))
                                filteredMeans.Add(new Mezzi(m.Descrizione, m.IDSystem));
                        }
                        break;

                    case "MAIL":
                        foreach (MezzoSpedizione m in m_sped)
                        {
                            if (!m.Descrizione.ToUpper().Equals("INTEROPERABILITA") && m.chaTipoCanale != "S")
                            {
                                filteredMeans.Add(new Mezzi(m.Descrizione, m.IDSystem));
                            }
                            else
                            {
                                if (m.chaTipoCanale == "S")
                                {
                                    if (SimplifiedInteroperabilityManager.IsEnabledSimpInterop &&
                                        corr.Url != null && corr.Url.Length > 0 &&
                                        Uri.IsWellFormedUriString(corr.Url[0].Url, UriKind.Absolute))
                                        filteredMeans.Add(new Mezzi(m.Descrizione, m.IDSystem));

                                }
                                else
                                    //Verifico che siano presenti i campi obbligatori per l'utente con canale preferito INTEROPERABILITA
                                    if (!string.IsNullOrEmpty(corr.codiceAmm) && (!string.IsNullOrEmpty(corr.codiceAOO)) && (!string.IsNullOrEmpty(corr.email)))
                                    {
                                        filteredMeans.Add(new Mezzi(m.Descrizione, m.IDSystem));
                                    }
                            }
                        }
                        break;

                    default:
                        this.FilterInteroperability(m_sped, corr, ref filteredMeans);

                        break;
                }
            }
            return filteredMeans;
        }

        /// <summary>
        /// Metodo per il filtraggio dei mezzi di spedizione relativi all'interoperabilità. Questo metodo
        /// non contente di inserire nella combo dei mezzi di spedizione, Interoperabilità e 
        /// Interoperabilità Semplificata se non sono soddisfatti i requisiti
        /// </summary>
        /// <param name="m_sped">Lista dei mezzi di spedizione</param>
        /// <param name="corr">Corrispondente selezionato</param>
        private void FilterInteroperability(MezzoSpedizione[] m_sped, Corrispondente corr, ref List<Mezzi> filteredMeans)
        {

            // Prelevamento mezzi di interoperabilità e di tutti quelli che non riguardano l'interoperabilità
            var interop = m_sped.Where(m => m.Descrizione == "INTEROPERABILITA" || m.Descrizione == "MAIL" || m.chaTipoCanale == "S");
            var other = m_sped.Except(interop);

            // Tutti gli altri elementi vengono aggiunti alla lista dei mezzi filtrati
            foreach (var mezzo in other)
                filteredMeans.Add(new Mezzi(mezzo.Descrizione, mezzo.IDSystem));

            // L'interoperabilità classica può essere utilizzata solo se sono valorizzati tutti i campi obbligatori
            var classic = interop.Where(m => m.Descrizione == "INTEROPERABILITA").FirstOrDefault();
            if (classic != null && !string.IsNullOrEmpty(corr.codiceAmm) &&
                (!string.IsNullOrEmpty(corr.codiceAOO)) && (!string.IsNullOrEmpty(corr.email)))
                filteredMeans.Add(new Mezzi(classic.Descrizione, classic.IDSystem));

            var mail = interop.Where(m => m.Descrizione == "MAIL").FirstOrDefault();
            if (mail != null && !String.IsNullOrEmpty(corr.email))
                filteredMeans.Add(new Mezzi(mail.Descrizione, mail.IDSystem));

            var simpInterop = interop.Where(m => m.chaTipoCanale == "S").FirstOrDefault();
            if (simpInterop != null && SimplifiedInteroperabilityManager.IsEnabledSimpInterop &&
                corr.Url != null && corr.Url.Length > 0 &&
                Uri.IsWellFormedUriString(corr.Url[0].Url, UriKind.Absolute))
                filteredMeans.Add(new Mezzi(simpInterop.Descrizione, simpInterop.IDSystem));

        }

        private void SetTypeDelivery()
        {
            string idCorr = string.Empty;
            string idCanale = string.Empty;
            DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
            DocsPaWR.Corrispondente[] dest = this.ListRecipients.ToArray();
            DocsPaWR.Corrispondente[] destCc = this.ListRecipientsCC.ToArray();
            // Per rendere possibile la modifica del mezzo di spedizione.
            string bypassa = "";
            DocsPaWR.Canale canale = new DocsPaWR.Canale();
            if (this.gvList != null)
            {
                Canale canaleTutti = null;
                if (!this.ddlAll.SelectedValue.Equals("0"))
                    canaleTutti = ws.getCanaleBySystemId(this.ddlAll.SelectedValue);

                if (this.gvList.Rows.Count > 0)
                {

                    //itero sui singoli item della griglia
                    foreach (GridViewRow item in this.gvList.Rows)
                    {
                        string tipoDest = string.Empty;
                        TableCellCollection cells = item.Cells;
                        foreach (TableCell cell in cells)
                        {
                            ControlCollection controls = cell.Controls;
                            foreach (Control control in controls)
                            {
                                if (control.GetType() == typeof(Label))
                                {
                                    idCorr = ((Label)control).Text;
                                    break;
                                }

                                if (control.GetType() == typeof(DropDownList))
                                {
                                    idCanale = ((DropDownList)control).SelectedValue;
                                    break;

                                }
                            }
                        }

                        if (dest != null && dest.Length > 0)
                        {
                            foreach (DocsPaWR.Corrispondente corr in dest)
                            {


                                //l'utente a selezionata blank quindi reimposto il canale di default
                                if (!string.IsNullOrEmpty(corr.systemId) && corr.systemId.Equals(idCorr) && (idCanale.Equals("0")))
                                {
                                    if (corr.canalePref != null && corr.canalePref.systemId != null)
                                    {
                                        canale = UserManager.getCorrispondentBySystemID(idCorr).canalePref;
                                        //corr.canalePref = canale;
                                        if (!corr.canalePref.systemId.Equals(canale.systemId))
                                        {
                                            if (TypeDelivery.ContainsKey(corr.systemId))
                                                TypeDelivery[corr.systemId] = string.Empty;
                                            else
                                                TypeDelivery.Add(corr.systemId, string.Empty);
                                        }
                                    }

                                    else if (TypeDelivery.ContainsKey(corr.systemId))
                                    {
                                        TypeDelivery.Remove(corr.systemId);
                                    }
                                    tipoDest = "d";
                                    break;
                                }
                                //imposto il canale selezionato dall'utente
                                else if (!string.IsNullOrEmpty(corr.systemId) && corr.systemId.Equals(idCorr))
                                {
                                    //canale = ws.getCanaleBySystemId(idCanale);
                                    //corr.canalePref = canale;
                                    if (TypeDelivery.ContainsKey(corr.systemId))
                                        TypeDelivery[corr.systemId] = idCanale;
                                    else
                                        TypeDelivery.Add(corr.systemId, idCanale);
                                    tipoDest = "d";
                                    break;
                                }
                            }
                        }
                        if (destCc != null && destCc.Length > 0 && (!tipoDest.Equals("d")))
                        {
                            foreach (DocsPaWR.Corrispondente corr in destCc)
                            {
                                //l'utente ha selezionata blank quindi reimposto il canale di default
                                if (!string.IsNullOrEmpty(corr.systemId) && corr.systemId.Equals(idCorr) && (idCanale.Equals("0")))
                                {
                                    if (corr.canalePref != null && corr.canalePref.systemId != null)
                                    {
                                        canale = UserManager.getCorrispondentBySystemID(idCorr).canalePref;
                                        //corr.canalePref = canale;
                                        if (!corr.canalePref.systemId.Equals(canale.systemId))
                                        {
                                            if (TypeDelivery.ContainsKey(corr.systemId))
                                                TypeDelivery[corr.systemId] = string.Empty;
                                            else
                                                TypeDelivery.Add(corr.systemId, string.Empty);
                                        }
                                        else if (TypeDelivery.ContainsKey(corr.systemId))
                                        {
                                            TypeDelivery.Remove(corr.systemId);
                                        }
                                        break;
                                    }
                                }
                                //imposto il canale selezionato dall'utente
                                else if (!string.IsNullOrEmpty(corr.systemId) && corr.systemId.Equals(idCorr))
                                {
                                    //canale = ws.getCanaleBySystemId(idCanale);
                                    //corr.canalePref = canale;
                                    if (TypeDelivery.ContainsKey(corr.systemId))
                                        TypeDelivery[corr.systemId] = idCanale;
                                    else
                                        TypeDelivery.Add(corr.systemId, idCanale);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        protected void BtnOk_Click(object sender, EventArgs e)
        {
            try
            {
                bool acceptCanaleTutti;
                // Per rendere possibile la modifica del mezzo di spedizione.
                string bypassa = "";
                DocsPaWR.Canale canale = new DocsPaWR.Canale();
                DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
                DocsPaWR.Corrispondente[] dest = this.ListRecipients.ToArray();
                DocsPaWR.Corrispondente[] destCc = this.ListRecipientsCC.ToArray();


                Canale canaleTutti = null;
                if (!this.ddlAll.SelectedValue.Equals("0"))
                    canaleTutti = ws.getCanaleBySystemId(this.ddlAll.SelectedValue);

                string tipoDest = string.Empty;
                if (dest != null && dest.Length > 0)
                {
                    acceptCanaleTutti = false;
                    foreach (DocsPaWR.Corrispondente corr in dest)
                    {
                        if (!string.IsNullOrEmpty(corr.systemId))
                        {
                            // se è impostato il canale tutti ed il corrispondente corrente ha visibilità del mezzo di spedizione tutti allora lo imposto
                            if (canaleTutti != null)
                            {
                                List<Mezzi> listMezziVisCorr = GetMeansDeliveryFiltered(UserManager.getCorrispondentBySystemID(corr.systemId).canalePref, corr);
                                foreach (Mezzi m in listMezziVisCorr)
                                {
                                    if (m.Descrizione.Equals(canaleTutti.descrizione) && m.Valore.Equals(canaleTutti.systemId))
                                    {
                                        acceptCanaleTutti = true;
                                        corr.canalePref = canaleTutti;
                                        bypassa += corr.systemId + "§";
                                        break;
                                    }
                                }
                            }
                            if (!acceptCanaleTutti && TypeDelivery.ContainsKey(corr.systemId))
                            {

                                if (!string.IsNullOrEmpty(TypeDelivery[corr.systemId]))
                                {
                                    canale = ws.getCanaleBySystemId(TypeDelivery[corr.systemId]);
                                    corr.canalePref = canale;
                                    bypassa += corr.systemId + "§";
                                }
                                else if (corr.canalePref != null && corr.canalePref.systemId != null)
                                {
                                    canale = UserManager.getCorrispondentBySystemID(corr.systemId).canalePref;
                                    corr.canalePref = canale;
                                }
                            }
                            else if (!acceptCanaleTutti && corr.canalePref != null && corr.canalePref.systemId != null)
                            {
                                canale = ws.getCanaleBySystemId(corr.canalePref.systemId);
                                corr.canalePref = canale;
                            }
                        }
                    }
                }
                if (destCc != null && destCc.Length > 0)
                {
                    acceptCanaleTutti = false;
                    foreach (DocsPaWR.Corrispondente corr in destCc)
                    {
                        if (!string.IsNullOrEmpty(corr.systemId))
                        {
                            // se è impostato il canale tutti ed il corrispondente corrente ha visibilità del mezzo di spedizione tutti allora lo imposto
                            if (canaleTutti != null)
                            {
                                List<Mezzi> listMezziVisCorr = GetMeansDeliveryFiltered(UserManager.getCorrispondentBySystemID(corr.systemId).canalePref, corr);
                                foreach (Mezzi m in listMezziVisCorr)
                                {
                                    if (m.Descrizione.Equals(canaleTutti.descrizione) && m.Valore.Equals(canaleTutti.systemId))
                                    {
                                        acceptCanaleTutti = true;
                                        corr.canalePref = canaleTutti;
                                        bypassa += corr.systemId + "§";
                                        break;
                                    }
                                }
                            }
                            if (!acceptCanaleTutti && TypeDelivery.ContainsKey(corr.systemId))
                            {
                                if (!string.IsNullOrEmpty(TypeDelivery[corr.systemId]))
                                {
                                    canale = ws.getCanaleBySystemId(TypeDelivery[corr.systemId]);
                                    corr.canalePref = canale;
                                    bypassa += corr.systemId + "§";
                                }
                                else if (corr.canalePref != null && corr.canalePref.systemId != null)
                                {
                                    canale = UserManager.getCorrispondentBySystemID(corr.systemId).canalePref;
                                    corr.canalePref = canale;
                                }
                            }
                            else if (!acceptCanaleTutti && corr.canalePref != null && corr.canalePref.systemId != null)
                            {
                                canale = ws.getCanaleBySystemId(corr.canalePref.systemId);
                                corr.canalePref = canale;
                            }
                        }
                    }
                }
                //Salvo le informazioni aggiornate nella sessione
                this.ListRecipients = dest.OfType<Corrispondente>().ToList();
                this.ListRecipientsCC = destCc.OfType<Corrispondente>().ToList();
                // Per rendere possibile la modifica del mezzo di spedizione.
                this.bypassaControlloRicevute = bypassa;
                this.modificaDestinatari = true;
                this.CloseMask(true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        //protected void BtnOk_Click(object sender, EventArgs e)
        //{
        //    try {
        //        string idCorr = string.Empty;
        //        string idCanale = string.Empty;
        //        bool acceptCanaleTutti;
        //        // Per rendere possibile la modifica del mezzo di spedizione.
        //        string bypassa = "";
        //        DocsPaWR.Canale canale = new DocsPaWR.Canale();
        //        DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
        //        DocsPaWR.Corrispondente[] dest = this.ListRecipients.ToArray();
        //        DocsPaWR.Corrispondente[] destCc = this.ListRecipientsCC.ToArray();

        //        if (this.gvList != null)
        //        {
        //            Canale canaleTutti = null;
        //            if (!this.ddlAll.SelectedValue.Equals("0"))
        //                canaleTutti = ws.getCanaleBySystemId(this.ddlAll.SelectedValue);

        //            if (this.gvList.Rows.Count>0)
        //            {
        //                //itero sui singoli item della griglia
        //                foreach (GridViewRow item in this.gvList.Rows)
        //                {
        //                    string tipoDest = string.Empty;
        //                    TableCellCollection cells = item.Cells;
        //                    foreach (TableCell cell in cells)
        //                    {
        //                        ControlCollection controls = cell.Controls;
        //                        foreach (Control control in controls)
        //                        {
        //                            if (control.GetType() == typeof(Label))
        //                            {
        //                                idCorr = ((Label)control).Text;
        //                                break;
        //                            }

        //                            if (control.GetType() == typeof(DropDownList))
        //                            {
        //                                idCanale = ((DropDownList)control).SelectedValue;
        //                                break;

        //                            }
        //                        }
        //                    }

        //                    if (dest != null && dest.Length > 0)
        //                    {
        //                        acceptCanaleTutti = false;
        //                        foreach (DocsPaWR.Corrispondente corr in dest)
        //                        {
        //                            // se è impostato il canale tutti ed il corrispondente corrente ha visibilità del mezzo di spedizione tutti allora lo imposto
        //                            if (corr.systemId.Equals(idCorr) && canaleTutti != null)
        //                            {
        //                                List<Mezzi> listMezziVisCorr = GetMeansDeliveryFiltered(UserManager.getCorrispondentBySystemID(idCorr).canalePref, corr);
        //                                foreach (Mezzi m in listMezziVisCorr)
        //                                {
        //                                    if (m.Descrizione.Equals(canaleTutti.descrizione) && m.Valore.Equals(canaleTutti.systemId))
        //                                    {
        //                                        acceptCanaleTutti = true;
        //                                        corr.canalePref = canaleTutti;
        //                                        tipoDest = "d";
        //                                        bypassa += idCorr+"§";
        //                                        break;
        //                                    }
        //                                }
        //                            }

        //                            //l'utente a selezionata blank quindi reimposto il canale di default
        //                            if (!acceptCanaleTutti && corr.systemId.Equals(idCorr) && (idCanale.Equals("0")))
        //                            {
        //                                if (corr.canalePref != null && corr.canalePref.systemId != null)
        //                                {
        //                                    canale = UserManager.getCorrispondentBySystemID(idCorr).canalePref;
        //                                    corr.canalePref = canale;
        //                                    tipoDest = "d";
        //                                    break;
        //                                }
        //                            }
        //                            //imposto il canale selezionato dall'utente
        //                            else if (!acceptCanaleTutti && corr.systemId.Equals(idCorr))
        //                            {
        //                                canale = ws.getCanaleBySystemId(idCanale);
        //                                corr.canalePref = canale;
        //                                tipoDest = "d";
        //                                bypassa += idCorr + "§";
        //                                break;
        //                            }
        //                        }
        //                    }
        //                    if (destCc != null && destCc.Length > 0 && (!tipoDest.Equals("d")))
        //                    {
        //                        acceptCanaleTutti = false;
        //                        foreach (DocsPaWR.Corrispondente corr in destCc)
        //                        {
        //                            // se è impostato il canale tutti ed il corrispondente corrente ha visibilità del mezzo di spedizione tutti allora lo imposto
        //                            if (corr.systemId.Equals(idCorr) && canaleTutti != null)
        //                            {
        //                                List<Mezzi> listMezziVisCorr = GetMeansDeliveryFiltered(UserManager.getCorrispondentBySystemID(idCorr).canalePref, corr);
        //                                foreach (Mezzi m in listMezziVisCorr)
        //                                {
        //                                    if (m.Descrizione.Equals(canaleTutti.descrizione) && m.Valore.Equals(canaleTutti.systemId))
        //                                    {
        //                                        acceptCanaleTutti = true;
        //                                        corr.canalePref = canaleTutti;
        //                                        tipoDest = "d";
        //                                        bypassa += idCorr + "§";
        //                                        break;
        //                                    }
        //                                }
        //                            }

        //                            //l'utente a selezionata blank quindi reimposto il canale di default
        //                            if (!acceptCanaleTutti && corr.systemId.Equals(idCorr) && (idCanale.Equals("0")))
        //                            {
        //                                if (corr.canalePref != null && corr.canalePref.systemId != null)
        //                                {
        //                                    canale = UserManager.getCorrispondentBySystemID(idCorr).canalePref;
        //                                    corr.canalePref = canale;
        //                                    break;
        //                                }
        //                            }
        //                            //imposto il canale selezionato dall'utente
        //                            else if (!acceptCanaleTutti && corr.systemId.Equals(idCorr))
        //                            {
        //                                canale = ws.getCanaleBySystemId(idCanale);
        //                                corr.canalePref = canale;
        //                                bypassa += idCorr + "§";
        //                                break;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        //Salvo le informazioni aggiornate nella sessione
        //        this.ListRecipients = dest.OfType<Corrispondente>().ToList();
        //        this.ListRecipientsCC = destCc.OfType<Corrispondente>().ToList();
        //        // Per rendere possibile la modifica del mezzo di spedizione.
        //        this.bypassaControlloRicevute = bypassa;
        //        this.modificaDestinatari = true;
        //        this.CloseMask(true);
        //    }
        //    catch (System.Exception ex)
        //    {
        //        UIManager.AdministrationManager.DiagnosticError(ex);
        //        return;
        //    }
        //}

    }

    public class Mezzi
    {
        private string descrizione;
        private string valore;
        public Mezzi(string descrizione, string valore)
        {
            this.descrizione = descrizione;
            this.valore = valore;
        }
        public string Descrizione { get { return descrizione; } }
        public string Valore { get { return valore; } }
    }

    public class Cols
    {
        private string descrizione;
        private string corrId;
        public Cols(string descrizione, string corrId)
        {
            this.descrizione = descrizione;
            this.corrId = corrId;
        }
        public string Descrizione { get { return descrizione; } }
        public string Id { get { return corrId; } }
    }

}