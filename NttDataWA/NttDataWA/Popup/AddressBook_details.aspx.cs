using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
    public partial class AddressBook_details : System.Web.UI.Page
    {

        #region Properties

        private bool ActiveCodeDescriptionAdministrationSender
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["ActiveCodeDescriptionAdministrationSender"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["ActiveCodeDescriptionAdministrationSender"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["ActiveCodeDescriptionAdministrationSender"] = value;
            }
        }

        private bool EnableDistributionLists
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["enableDistributionLists"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["enableDistributionLists"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["enableDistributionLists"] = value;
            }
        }

        private bool IsModified
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["IsModified"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["IsModified"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["IsModified"] = value;
            }
        }

        private bool IsModifiedChannel
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["IsModifiedChannel"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["IsModifiedChannel"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["IsModifiedChannel"] = value;
            }
        }

        private bool IsModifiedCaselle
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["IsModifiedCaselle"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["IsModifiedCaselle"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["IsModifiedCaselle"] = value;
            }
        }

        private bool IsEmailReadOnly
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["IsEmailReadOnly"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["IsEmailReadOnly"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["IsEmailReadOnly"] = value;
            }
        }

        public DocsPaWR.Corrispondente corr
        {
            get
            {
                return Session["AddressBook_details_corr"] as DocsPaWR.Corrispondente;
            }
            set
            {
                Session["AddressBook_details_corr"] = value;
            }
        }

        public List<DocsPaWR.MailCorrispondente> CaselleList
        {
            get
            {
                return Session["AddressBook_details_gvCaselle"] as List<DocsPaWR.MailCorrispondente>;
            }
            set
            {
                Session["AddressBook_details_gvCaselle"] = value;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.LoadKeys();
                    this.InitLanguage();
                    this.InitPage();
                    this.drawDettagliCorr();
                }
                else
                {
                    // detect action from popup confirm
                    if (this.proceed_delete.Value == "true") { this.DeleteProceed(); return; }
                }

                this.RefreshScripts();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void LoadKeys()
        {
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ATTIVA_CODICE_DESCRIZIONE_AMMINISTRAZIONE_MITTENTE.ToString()]) && bool.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ATTIVA_CODICE_DESCRIZIONE_AMMINISTRAZIONE_MITTENTE.ToString()]))
            {
                this.ActiveCodeDescriptionAdministrationSender = true;
            }
        }

        private void InitPage()
        {
            if (this.corr != null)
            {
                if (this.corr.tipoIE == "E" && string.IsNullOrEmpty(this.corr.dta_fine))
                {
                    this.BtnModify.Visible = true;
                    this.BtnModify.Enabled = !corr.inRubricaComune;
                    this.BtnDelete.Visible = true;
                    this.BtnDelete.Enabled = !corr.inRubricaComune;

                    if (corr.inRubricaComune)
                        this.setCampiReadOnly();
                    else
                        this.txt_CodRubrica.ReadOnly = true;
                }
                else
                {
                    this.setCampiReadOnly();
                }
            }
        }

        private void InitLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();

            if (this.corr != null)
            {
                switch (this.corr.tipoCorrispondente)
                {
                    case "L":
                        this.lbl_nomeCorr.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsTitleList", language);
                        break;
                    case "F":
                        this.lbl_nomeCorr.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsTitleRF", language);
                        break;
                    default:
                        this.lbl_nomeCorr.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsTitle", language);
                        break;
                }
            }

            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("GenericBtnClose", language);
            this.BtnModify.Text = Utils.Languages.GetLabelFromCode("GenericBtnModify", language);
            this.BtnDelete.Text = Utils.Languages.GetLabelFromCode("GenericBtnDelete", language);
            this.lbl_registro.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsRegistry", language);
            this.lbl_indirizzo.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsAddress", language);
            this.lbl_cap.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsZipCode", language);
            this.lbl_citta.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsCity", language);
            this.lbl_provincia.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsDistrict", language);
            this.lbl_local.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsPlace", language);
            this.lbl_nazione.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsCountry", language);
            this.lbl_telefono.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsPhone", language);
            this.lbl_telefono2.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsPhone2", language);
            this.lbl_fax.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsFax", language);
            this.lbl_codfisc.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsTaxId", language);
            this.lbl_partita_iva.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsCommercialId", language);
            //this.lbl_codice_ipa.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsIpaCode", language);
            this.lbl_email.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsEmail", language);
            this.lbl_codAOO.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsCodAOO", language);
            this.lbl_codAmm.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsCodAdmin", language);
            this.lblNote.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsNoteEmail", language);
            this.lbl_note.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsNote", language);
            this.lbl_preferredChannel.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsPreferredChannel", language);
            this.lbl_tipocorr.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsType", language);
            this.lbl_CodRubrica.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsAddressBookCode", language);
            this.lbl_descrizione.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsAddressBookDescription", language);
            this.lbl_CodR.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsAddressBookCode", language);
            this.lbl_DescR.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsAddressBookDescription", language);
            this.lbl_titolo.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsTitolo", language);
            this.lbl_nome.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsName", language);
            this.lbl_cognome.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsSurname", language);
            this.lbl_luogonascita.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsBirthplace", language);
            this.lbl_dataNascita.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsBirthday", language);
            this.lbl_Ruoli.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsRoles", language);
            this.dg_listCorr.Columns[0].HeaderText = Utils.Languages.GetLabelFromCode("CorrespondentDetailsCode", language);
            this.dg_listCorr.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("CorrespondentDetailsDescription", language);
            this.ddl_tipoCorr.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectOptionNeutral", language));
            this.ddl_titolo.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectOptionNeutral", language));
            this.dd_canpref.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectOptionNeutral", language));
            this.imgAggiungiCasella.AlternateText = Utils.Languages.GetLabelFromCode("imgAggiungiCasella", language);
            this.imgAggiungiCasella.ToolTip = Utils.Languages.GetLabelFromCode("imgAggiungiCasella", language);

            this.cbInteroperanteRGS.Text = Utils.Languages.GetLabelFromCode("CorrispondentInteroperanteRGS", language);
            this.BtnInsert.Text = Utils.Languages.GetLabelFromCode("GenericBtnInsert", language);
        }

        protected string GetLabelDelete()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode("imgCasellaDelete", language);
        }

        private string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            try {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                this.CloseMask(string.Empty);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void ClearSessionData()
        {
            this.corr = null;
            this.CaselleList = null;
            this.IsModified = false;
            this.IsModifiedChannel = false;
            this.IsModifiedCaselle = false;
            this.IsEmailReadOnly = false;
        }

        protected void CloseMask(string returnValue)
        {
            this.ClearSessionData();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "parent.closeAjaxModal('AddressBook_Details', '" + returnValue + "', parent);", true);
        }

        private void setCampiReadOnly()
        {
            this.txt_codAmm.ReadOnly = true;
            this.txt_codAOO.ReadOnly = true;
            this.txt_email.ReadOnly = true;
            this.txt_CodR.ReadOnly = true;
            this.txt_DescR.ReadOnly = true;
            this.txt_CodRubrica.ReadOnly = true;
            this.txt_descrizione.ReadOnly = true;
            this.txt_indirizzo.ReadOnly = true;
            this.txt_cap.ReadOnly = true;
            this.txt_citta.ReadOnly = true;
            this.txt_local.ReadOnly = true;
            this.txt_provincia.ReadOnly = true;
            this.txt_nazione.ReadOnly = true;
            this.txt_telefono.ReadOnly = true;
            this.txt_telefono2.ReadOnly = true;
            this.txt_fax.ReadOnly = true;
            this.txt_codfisc.ReadOnly = true;
            this.txt_partita_iva.ReadOnly = true;
            //this.txt_codice_ipa.ReadOnly = true;
            this.txt_nome.ReadOnly = true;
            this.txt_cognome.ReadOnly = true;
            this.txt_note.ReadOnly = true;
            this.dd_canpref.Enabled = false;
            this.txt_luogoNascita.ReadOnly = true;
            this.txt_dataNascita.ReadOnly = true;
            this.ddl_titolo.Enabled = false;
            this.txtNote.ReadOnly = true;
            this.txtCasella.ReadOnly = true;
            this.imgAggiungiCasella.Enabled = false;
        }

        private void drawDettagliCorr()
        {
            if (this.corr != null)
            {
                if (corr.tipoCorrispondente == "L" || (corr.tipoCorrispondente == "F" && !corr.inRubricaComune))
                {
                    this.pnl_registro.Visible = false;
                    this.pnl_email.Visible = false;
                    this.pnlStandard.Visible = false;
                    this.pnlRuolo.Visible = false;
                    this.pnlRuoliUtente.Visible = false;
                    this.PanelListaCorrispondenti.Visible = true;

                    ArrayList listaCorrispondenti = new ArrayList();

                    if (corr.tipoCorrispondente == "L")
                    {
                        this.lbl_nomeLista.Text = UserManager.getNomeLista(this, this.corr.codiceRubrica, UserManager.GetInfoUser().idAmministrazione) + " (" + this.corr.codiceRubrica + ")";
                        listaCorrispondenti = UserManager.getCorrispondentiByCodLista(this, this.corr.codiceRubrica, UserManager.GetInfoUser().idAmministrazione);
                    }
                    else
                    {
                        this.lbl_nomeLista.Text = UserManager.getNomeRF(this, this.corr.codiceRubrica);
                        listaCorrispondenti = UserManager.getCorrispondentiByCodRF(this, this.corr.codiceRubrica);
                    }

                    this.dg_listCorr.DataSource = this.creaDataTable(listaCorrispondenti);
                    this.dg_listCorr.DataBind();
                }
                else
                {
                    // registro è popolato solo per i corrisp esterni
                    if (this.corr.tipoIE != null && this.corr.tipoIE.Equals("E"))
                    {
                        this.pnl_registro.Visible = true;
                        string codiceRegistroCorr = string.Empty;
                        if (this.corr.idRegistro == null || (this.corr.idRegistro != null && this.corr.idRegistro.Trim() == ""))
                        {
                            this.lit_registro.Text = "TUTTI [RC]";
                        }
                        else
                        {
                            DocsPaWR.Registro regCorr = UserManager.getRegistroBySistemId(this, this.corr.idRegistro);
                            if (regCorr != null)
                            {
                                if (regCorr.chaRF == "0")
                                    this.lbl_registro.Text = this.GetLabel("CorrespondentDetailsRegistry");
                                else
                                    this.lbl_registro.Text = this.GetLabel("CorrespondentDetailsRF");
                                codiceRegistroCorr = regCorr.codRegistro;
                            }
                        }
                        LoadRegistries(codiceRegistroCorr);
                        this.txt_email.Visible = false;
                        this.plcNoteMail.Visible = true;
                        this.IsEmailReadOnly = false;
                        this.EnableInsertMail();
                        this.CaselleList = null;
                        this.BindGridViewCaselle(this.corr);
                    }
                    else
                    {
                        this.txt_email.Visible = true;
                        this.txtCasella.Visible = false;
                        this.plcNoteMail.Visible = false;
                        this.imgAggiungiCasella.Visible = false;
                        this.updPanelMail.Visible = false;
                    }

                    // tipo corrispondente
                    this.LoadCorrespondentTypes();

                    // titoli
                    this.LoadTitles();

                    // canale preferenziale
                    // Se esterno e non di rubrica comune, non faccio inserire IS
                    if (this.corr.tipoIE == "E" && !this.corr.inRubricaComune)
                        this.LoadPreferredChannels(false);

                    else
                        this.LoadPreferredChannels(true);

                    if (this.corr.tipoIE == "E")
                    {
                        this.plcPreferredChannel.Visible = true;
                        if (this.corr.canalePref != null)
                        {
                            this.dd_canpref.SelectedIndex = this.dd_canpref.Items.IndexOf(this.dd_canpref.Items.FindByValue(this.corr.canalePref.systemId));
                            this.setVisibilityPanelStar();
                        }
                    }
                    else
                    {
                        this.plcPreferredChannel.Visible = false;
                    }


                    // campi rubrica
                    if (!(this.corr is DocsPaWR.Ruolo))
                    {
                        this.pnlStandard.Visible = true;
                        this.pnlRuolo.Visible = false;
                        this.pnl_indirizzo.Visible = true;

                        if (this.corr is DocsPaWR.Utente)
                        {
                            this.pnl_nome_cogn.Visible = true;

                            // campi visibili solo nel caso di utente esterno
                            if (this.corr.tipoIE.Equals("E"))
                            {
                                this.pnl_titolo.Visible = true;
                                this.pnl_infonascita.Visible = true;
                            }
                            DocsPaWR.Utente u = (DocsPaWR.Utente)this.corr;

                            // dati utente
                            this.txt_cognome.Text = u.cognome;
                            this.txt_nome.Text = u.nome;

                            string id_amm = UserManager.GetInfoUser().idAmministrazione;
                            DocsPaWR.ElementoRubrica[] ers = UserManager.GetRuoliUtente(this, id_amm, u.codiceRubrica);
                            this.lblRuoli.Text = "";

                            if (ers.Length > 0)
                            {
                                foreach (DocsPaWR.ElementoRubrica er in ers)
                                {
                                    this.lblRuoli.Text += (er.descrizione + "<br />");
                                }
                                this.lblRuoli.Visible = true;
                                this.lbl_Ruoli.Visible = true;
                            }

                            if (!this.corr.tipoIE.Equals("E"))
                                this.pnlRuoliUtente.Visible = true;

                            this.pnl_descrizione.Visible = false;
                            this.pnl_titolo.Visible = true;
                            this.pnl_nome_cogn.Visible = true;
                            this.pnl_infonascita.Visible = true;
                        }
                        else
                        {
                            this.pnl_descrizione.Visible = true;
                            this.pnl_titolo.Visible = false;
                            this.pnl_nome_cogn.Visible = false;
                            this.pnl_infonascita.Visible = false;
                        }
                    }
                    else
                    {
                        this.pnlStandard.Visible = false;
                        this.pnlRuolo.Visible = true;
                        this.pnl_descrizione.Visible = true;
                        this.pnl_titolo.Visible = false;
                        this.pnl_nome_cogn.Visible = false;
                        this.pnl_infonascita.Visible = false;
                        this.pnl_indirizzo.Visible = false;
                        DocsPaWR.Ruolo u = (DocsPaWR.Ruolo)this.corr;

                        this.txt_CodR.Text = u.codiceRubrica;
                        this.txt_DescR.Text = u.descrizione;
                    }


                    DocsPaWR.CorrespondentDetails corrDetails = AddressBookManager.GetCorrespondentDetails(corr.systemId);
                    if (corrDetails != null)
                    {
                        this.ddl_titolo.SelectedIndex = this.ddl_titolo.Items.IndexOf(this.ddl_titolo.Items.FindByText(corrDetails.Title));
                        this.txt_indirizzo.Text = corrDetails.Address;
                        this.txt_cap.Text = corrDetails.ZipCode;
                        this.txt_citta.Text = corrDetails.City;
                        this.txt_provincia.Text = corrDetails.District;
                        this.txt_local.Text = corrDetails.Place;
                        this.txt_nazione.Text = corrDetails.Country;
                        this.txt_telefono.Text = corrDetails.Phone;
                        this.txt_telefono2.Text = corrDetails.Phone2;
                        this.txt_fax.Text = corrDetails.Fax;
                        this.txt_codfisc.Text = corrDetails.TaxId;
                        this.txt_partita_iva.Text = corrDetails.CommercialId;
                        //this.txt_codice_ipa.Text = corrDetails.IpaCode;
                        this.txt_note.Text = corrDetails.Note;
                        this.txt_dataNascita.Text = corrDetails.BirthDay;
                        this.txt_luogoNascita.Text = corrDetails.BirthPlace;
                    }

                    this.txt_CodRubrica.Text = corr.codiceRubrica;

                    if (this.ActiveCodeDescriptionAdministrationSender && !string.IsNullOrEmpty(corr.codDescAmministrizazione))
                        this.txt_descrizione.Text = corr.codDescAmministrizazione + corr.descrizione;
                    else
                        this.txt_descrizione.Text = corr.descrizione;

                    if(corr.email != null)
                        this.txt_email.Text = corr.email;

                    if (!string.IsNullOrEmpty(corr.email) && corr.Emails.Length == 0)
                        this.txtCasella.Text = corr.email;

                    this.txt_codAmm.Text = corr.codiceAmm;
                    this.txt_codAOO.Text = corr.codiceAOO;
                    this.txt_nome.Text = corr.nome;
                    this.txt_cognome.Text = corr.cognome;

                    this.cbInteroperanteRGS.Checked = corr.interoperanteRGS;
                }
            }
        }

        private DocsPaWR.Registro[] getListaRegistri()
        {
            //prendo i registri/RF visibili al ruolo dell'utente
            this.ddl_registri.Enabled = true;
            //DocsPaWR.Registro[] userRegistri = UserManager.getListaRegistriWithRF(this, "", "");
            DocsPaWR.Registro[] userRegistri = RegistryManager.GetRegAndRFListInSession();
            return userRegistri;
        }


        protected void LoadRegistries(string codiceRegistroSelezionato)
        {
            DocsPaWR.Registro[] userRegistri = this.getListaRegistri();
            ddl_registri.Items.Clear();
            if (userRegistri != null && userRegistri.Length > 0)
            {
                //Spaziatura per RF
                string strText = "";
                //int contatoreRegRF = 0;
                for (short iff = 0; iff < 3; iff++)
                {
                    strText += " ";
                }

                //Aggiunta registri o rf
                for (int i = 0; i < userRegistri.Length; i++)
                {
                    if (!userRegistri[i].Sospeso)
                    {
                        if (userRegistri[i].chaRF == "1")
                        {
                            //RF
                            if (UserManager.IsAuthorizedFunctions("DO_INS_CORR_RF"))
                            {
                                string testo = strText + userRegistri[i].codRegistro;
                                ListItem item = new ListItem();
                                item.Text = testo;
                                item.Value = userRegistri[i].systemId + "_" + userRegistri[i].rfDisabled + "_" + userRegistri[i].idAOOCollegata;
                                this.ddl_registri.Items.Add(item);
                                if (codiceRegistroSelezionato.Equals(userRegistri[i].codRegistro))
                                    this.ddl_registri.SelectedValue = item.Value;
                            }
                        }
                        else
                        {
                            //Registro
                            if ((UserManager.IsAuthorizedFunctions("DO_INS_CORR_REG") && !userRegistri[i].flag_pregresso))
                            {
                                string testo = userRegistri[i].codRegistro;
                                ListItem item = new ListItem();
                                item.Text = testo;
                                item.Value = userRegistri[i].systemId + "_" + userRegistri[i].rfDisabled + "_" + userRegistri[i].idAOOCollegata;
                                this.ddl_registri.Items.Add(item);
                                if (codiceRegistroSelezionato.Equals(userRegistri[i].codRegistro))
                                    this.ddl_registri.SelectedValue = item.Value;
                            }

                        }
                    }
                }

                //Aggiunta voce su tutti i registri o rf
                if (UserManager.IsAuthorizedFunctions("DO_INS_CORR_TUTTI"))
                {
                    ListItem item = new ListItem();
                    item.Text = "TUTTI";
                    item.Value = "";
                    this.ddl_registri.Items.Add(item);
                    if(string.IsNullOrEmpty(codiceRegistroSelezionato))
                        this.ddl_registri.SelectedIndex = this.ddl_registri.Items.IndexOf(this.ddl_registri.Items.FindByText("TUTTI"));
                }
            }
        }

        protected void ddl_registri_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                bool rfPresente = false;

                for (int li = 0; li < this.ddl_registri.Items.Count; li++)
                {
                    string valore = this.ddl_registri.Items[li].Value;
                    string[] arrayValori = valore.Split('_');
                    if (arrayValori != null && arrayValori.Length > 1)
                    {
                        if (!string.IsNullOrEmpty(arrayValori[1]))
                        {
                            rfPresente = true;
                            break;
                        }
                    }
                }

                if (rfPresente)
                {
                    this.lbl_registro.Text = this.GetLabel("CorrespondentDetailsRegistryRF");
                }
                else
                {
                    this.lbl_registro.Text = this.GetLabel("CorrespondentDetailsRegistry");
                }

                int indice_selezionato = this.ddl_registri.SelectedIndex;

                ListItem item = null;
                string valore2 = null;
                string[] arrayValori2 = null;

                if (indice_selezionato != -1)
                {
                    item = this.ddl_registri.Items[indice_selezionato];
                    valore2 = item.Value;
                    arrayValori2 = valore2.Split('_');
                }
                if (string.IsNullOrEmpty(valore2))
                {
                    if (string.IsNullOrEmpty(corr.idRegistro))
                    {
                        this.BtnInsert.Visible = false;
                        this.BtnModify.Enabled = true;
                    }
                    else
                    {
                        this.BtnInsert.Visible = true;
                        this.BtnModify.Enabled = false;
                    }
                }
                if (arrayValori2 != null && arrayValori2.Length > 1)
                {
                    if (!string.IsNullOrEmpty(arrayValori2[1]))
                    {
                        if(arrayValori2[0].Equals(corr.idRegistro))
                        {
                            this.BtnInsert.Visible = false;
                            this.BtnModify.Enabled = true;
                        }
                        else
                        {
                            this.BtnInsert.Visible = true;
                            this.BtnModify.Enabled = false;
                        }
                        if (arrayValori2[1] == "1")
                        {
                            this.BtnInsert.Enabled = false;
                            this.BtnInsert.ToolTip = this.GetLabel("AddressBookInsertDisabled");
                        }
                        else
                        {
                            this.BtnInsert.Enabled = true;
                            this.BtnInsert.ToolTip = this.GetLabel("GenericBtnInsert");
                        }

                    }
                    else
                    {
                        this.BtnInsert.Enabled = true;
                        this.BtnInsert.ToolTip = this.GetLabel("GenericBtnInsert");
                    }
                }
                this.UpPanelButton.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void BtnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
                string corr_type = ddl_tipoCorr.SelectedValue;

                int indxMail = this.dd_canpref.Items.IndexOf(this.dd_canpref.Items.FindByText("MAIL"));
                int indxInterop = this.dd_canpref.Items.IndexOf(this.dd_canpref.Items.FindByText("INTEROPERABILITA"));

                if (this.txt_CodRubrica.Text == "" ||
                    (corr_type == "U" && this.txt_descrizione.Text.Trim() == "") ||
                    (corr_type == "R" && this.txt_DescR.Text.Trim() == "") ||
                    (corr_type == "P" && (this.txt_cognome.Text.Trim() == "" || this.txt_nome.Text.Trim() == "")) ||
                    (dd_canpref.SelectedIndex == indxInterop && (this.txt_codAmm.Text.Equals(String.Empty) ||
                    this.txt_codAOO.Text.Equals(String.Empty))))
                {
                    string msg = "WarningAddressBookInsertObligatory";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                    return;
                }
                else
                {
                    if (CaselleList == null || (CaselleList as List<DocsPaWR.MailCorrispondente>).Count < 1)
                    {
                        //verifico che l'indirizzo non sia vuoto
                        if (string.IsNullOrEmpty(txtCasella.Text))
                        {
                            if (dd_canpref.SelectedItem.Text.Equals("MAIL") || dd_canpref.SelectedItem.Text.Equals("INTEROPERABILITA"))
                            {
                                string msg = "WarningAddressBookEmailNotInserted";
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                                return;
                            }
                        }

                        //verifico il formato dell'indirizzo mail
                        string pattern = "^(([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+([;](([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+)*$";
                        if (!string.IsNullOrEmpty(txtCasella.Text) && !System.Text.RegularExpressions.Regex.Match(txtCasella.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), pattern).Success)
                        {
                            string msg = "WarningAddressBookEmailInvalid";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                            return;
                        }
                        if (!string.IsNullOrEmpty(txtCasella.Text))
                        {
                            (CaselleList as List<DocsPaWR.MailCorrispondente>).Add(new DocsPaWR.MailCorrispondente()
                            {
                                Email = txtCasella.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                                Note = txtNote.Text,
                                Principale = "1"
                            });
                        }
                    }

                    //verifica del formato della data di nascita
                    if (this.txt_dataNascita.Text != string.Empty && !utils.isDate(this.txt_dataNascita.Text))
                    {
                        string msg = "WarningAddressBookBirthdayInvalid";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                        return;
                    }

                    if ((this.txt_cap != null && !this.txt_cap.Text.Equals("") && !utils.isNumeric(txt_cap.Text))
                        || (this.txt_cap != null && !this.txt_cap.Text.Equals("") && !utils.isNumeric(txt_cap.Text)))
                    {
                        string msg = "WarningAddressBookZipcodeInvalid";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                        return;
                    }

                    //Verifica della correttezza del codice fiscale
                    if (corr_type.Equals("U"))
                    {
                        if ((this.txt_codfisc != null && !this.txt_codfisc.Text.Trim().Equals("")) && ((this.txt_codfisc.Text.Trim().Length == 11 && utils.CheckVatNumber(this.txt_codfisc.Text.Trim()) != 0) || (this.txt_codfisc.Text.Trim().Length == 16 && utils.CheckTaxCode(this.txt_codfisc.Text.Trim()) != 0) || (this.txt_codfisc.Text.Trim().Length != 11 && this.txt_codfisc.Text.Trim().Length != 16)))
                        {
                            string msg = "WarningAddressBookTaxIdInvalid";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                            return;
                        }

                        //Verifica della correttezza della partita iva
                        if (this.txt_partita_iva != null && !this.txt_partita_iva.Text.Equals("") && utils.CheckVatNumber(txt_partita_iva.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray())) != 0)
                        {
                            string msg = "WarningAddressBookVatInvalid";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                            return;
                        }
                    }
                    else
                    {
                        if (this.txt_codfisc != null && !this.txt_codfisc.Text.Equals("") && utils.CheckTaxCode(this.txt_codfisc.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray())) != 0)
                        {
                            string msg = "WarningAddressBookTaxIdInvalid";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                            return;
                        }


                        if (this.txt_partita_iva != null && !this.txt_partita_iva.Text.Equals("") && utils.CheckVatNumber(this.txt_partita_iva.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray())) != 0)
                        {
                            string msg = "WarningAddressBookVatInvalid";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                            return;
                        }
                    }



                    if ((this.txt_telefono == null || this.txt_telefono.Text.Equals(""))
                        && !(this.txt_telefono2 == null || this.txt_telefono2.Text.Equals(""))
                        || (this.txt_telefono == null || this.txt_telefono.Text.Equals(""))
                        && !(this.txt_telefono2 == null || this.txt_telefono2.Text.Equals("")))
                    {
                        string msg = "WarningAddressBookPhoneNotInserted";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                        return;
                    }

                    if (this.ddl_registri != null && this.ddl_registri.Items.Count >= 0)
                    {
                        if (!(this.ddl_registri.SelectedValue.Equals("")))
                        {
                            string idRegistroCorrente = "";

                            if (ddl_registri.SelectedValue != string.Empty)
                            {
                                char[] sep = { '_' };
                                string[] datiSelezione = ddl_registri.SelectedValue.Split(sep);

                                idRegistroCorrente = datiSelezione[0];
                                DocsPaWR.Registro registro_corrente = UserManager.getRegistroBySistemId(this, idRegistroCorrente);
                                if (registro_corrente.Sospeso)
                                {
                                    string msg = "WarningAddressBookRegistrySuspended";
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '', '" + utils.FormatJs(registro_corrente.descrizione) + "');", true);
                                    return;
                                }
                            }
                        }
                    }
                }

                DocsPaWR.InfoUtente iu = UserManager.GetInfoUser();

                string idRegistro = string.Empty;
                if (ddl_registri != null && ddl_registri.SelectedIndex >= 0)
                {
                    if (ddl_registri.SelectedValue != string.Empty)
                    {
                        char[] sep = { '_' };
                        string[] datiSelezione = ddl_registri.SelectedValue.Split(sep);

                        idRegistro = datiSelezione[0];
                    }
                }

                DocsPaWR.Corrispondente corr = new DocsPaWR.Corrispondente();
                DocsPaWR.Corrispondente res = null;


                //creo l'oggetto canale
                DocsPaWR.Canale canale = new DocsPaWR.Canale();
                canale.systemId = this.dd_canpref.SelectedItem.Value;

                switch (corr_type)
                {
                    case "U":
                        DocsPaWR.UnitaOrganizzativa uo = new DocsPaWR.UnitaOrganizzativa();
                        uo.tipoCorrispondente = "U";
                        uo.codiceCorrispondente = this.txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                        uo.codiceRubrica = this.txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                        uo.codiceAmm = this.txt_codAmm.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                        uo.codiceAOO = this.txt_codAOO.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                        uo.descrizione = this.txt_descrizione.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                        uo.idAmministrazione = iu.idAmministrazione;
                        uo.localita = this.txt_local.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                        uo.idRegistro = idRegistro;
                        uo.interoperanteRGS = this.cbInteroperanteRGS.Checked;

                        uo = (DocsPaWR.UnitaOrganizzativa)AddressBookManager.ValorizeInfoCorr(uo, txt_indirizzo.Text.Trim(), txt_citta.Text.Trim(), txt_cap.Text.Trim(), txt_provincia.Text.Trim(), txt_nazione.Text.Trim(), txt_telefono.Text.Trim(), txt_telefono2.Text.Trim(), txt_fax.Text.Trim()
                            , txt_codfisc.Text.Trim(), txt_note.Text.Trim(), txt_local.Text.Trim(), string.Empty, string.Empty, string.Empty, txt_partita_iva.Text.Trim(), string.Empty);

                        uo.dettagli = true;
                        uo.canalePref = canale;
                        //mail
                        if (CaselleList != null && (CaselleList as List<MailCorrispondente>).Count > 0)
                        {
                            foreach (MailCorrispondente c in (CaselleList as List<MailCorrispondente>))
                            {
                                if (c.Principale.Equals("1"))
                                {
                                    uo.email = c.Email.Trim();
                                    break;
                                }
                            }
                        }
                        else
                        {
                            uo.email = string.Empty;
                        }
                        res = UserManager.addressbookInsertCorrispondente(this, uo, null);
                        if (res != null && (!string.IsNullOrEmpty(res.systemId)))
                        {
                            if (!MultiCasellaManager.InsertMailCorrispondenteEsterno((CaselleList as List<MailCorrispondente>), res.systemId))
                            {
                                string msg = "ErrorAddressBookMultiEmailInsert";
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                                return;
                            }
                        }
                        break;

                    case "R":
                        res = new DocsPaWR.Corrispondente();
                        DocsPaWR.Ruolo ruolo = new DocsPaWR.Ruolo();
                        ruolo.tipoCorrispondente = "R";
                        ruolo.codiceCorrispondente = this.txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                        ruolo.codiceRubrica = this.txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                        ruolo.descrizione = this.txt_DescR.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                        ruolo.idRegistro = idRegistro;
                        ruolo.codiceAmm = this.txt_codAmm.Text;
                        ruolo.codiceAOO = this.txt_codAOO.Text;
                        ruolo.idAmministrazione = UserManager.GetInfoUser().idAmministrazione;
                        ruolo.interoperanteRGS = this.cbInteroperanteRGS.Checked;
                        DocsPaWR.UnitaOrganizzativa parent_uo = new UnitaOrganizzativa();
                        parent_uo.descrizione = "";
                        parent_uo.systemId = "0";
                        ruolo.canalePref = canale;
                        //mail
                        if (CaselleList != null && (CaselleList as List<MailCorrispondente>).Count > 0)
                        {
                            foreach (MailCorrispondente c in (CaselleList as List<MailCorrispondente>))
                            {
                                if (c.Principale.Equals("1"))
                                {
                                    ruolo.email = c.Email.Trim();
                                    break;
                                }
                            }
                        }
                        else
                        {
                            ruolo.email = string.Empty;
                        }
                        res = UserManager.addressbookInsertCorrispondente(this, ruolo, parent_uo);
                        if (res != null && (!string.IsNullOrEmpty(res.systemId)))
                        {
                            if (!MultiCasellaManager.InsertMailCorrispondenteEsterno((CaselleList as List<MailCorrispondente>), res.systemId))
                            {
                                string msg = "ErrorAddressBookMultiEmailInsert";
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                                return;
                            }
                        }
                        break;

                    case "P":
                        res = new DocsPaWR.Corrispondente();
                        DocsPaWR.Utente utente = new DocsPaWR.Utente();
                        utente.codiceCorrispondente = this.txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                        utente.codiceRubrica = this.txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                        utente.cognome = this.txt_cognome.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                        utente.nome = this.txt_nome.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                        utente.codiceAmm = this.txt_codAmm.Text;
                        utente.codiceAOO = this.txt_codAOO.Text;
                        utente.descrizione = this.ddl_titolo.Text + this.txt_cognome.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()) + this.txt_nome.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                        utente.luogoDINascita = this.txt_luogoNascita.Text;
                        utente.dataNascita = this.txt_dataNascita.Text;
                        utente.idAmministrazione = UserManager.GetInfoUser().idAmministrazione;
                        utente.titolo = this.ddl_titolo.Text;
                        utente.idRegistro = idRegistro;
                        utente.tipoCorrispondente = this.ddl_tipoCorr.SelectedItem.Value;
                        utente.canalePref = canale;
                        utente.interoperanteRGS = this.cbInteroperanteRGS.Checked;
                        //mail
                        if (CaselleList != null && (CaselleList as List<MailCorrispondente>).Count > 0)
                        {
                            foreach (MailCorrispondente c in (CaselleList as List<MailCorrispondente>))
                            {
                                if (c.Principale.Equals("1"))
                                {
                                    utente.email = c.Email.Trim();
                                    break;
                                }
                            }
                        }
                        else
                        {
                            utente.email = string.Empty;
                        }
                        if ((txt_indirizzo.Text != null && !txt_indirizzo.Equals("")) ||
                                (txt_cap.Text != null && !txt_cap.Equals("")) ||
                                (txt_citta.Text != null && !txt_citta.Equals("")) ||
                                (txt_provincia.Text != null && !txt_provincia.Equals("")) ||
                                (txt_nazione.Text != null && !txt_nazione.Equals("")) ||
                                (txt_telefono.Text != null && !txt_telefono.Equals("")) ||
                                (txt_telefono2 != null && !txt_telefono2.Equals("")) ||
                                (txt_fax.Text != null && !txt_fax.Equals("")) ||
                                (txt_codfisc.Text != null && !txt_codfisc.Equals("")) ||
                                (txt_note.Text != null && !txt_note.Equals("")) ||
                                 txt_partita_iva.Text != null && !txt_partita_iva.Equals(""))
                        {
                            utente.dettagli = true;

                            utente = (DocsPaWR.Utente)AddressBookManager.ValorizeInfoCorr(utente, txt_indirizzo.Text.Trim(), txt_citta.Text.Trim(), txt_cap.Text.Trim(), txt_provincia.Text.Trim(), txt_nazione.Text.Trim(), txt_telefono.Text.Trim(), txt_telefono2.Text.Trim(), txt_fax.Text.Trim()
                                , txt_codfisc.Text.Trim(), txt_note.Text.Trim(), txt_local.Text.Trim(), ddl_titolo.SelectedValue, txt_luogoNascita.Text, txt_dataNascita.Text, txt_partita_iva.Text.Trim(), string.Empty);
                        }
                        res = UserManager.addressbookInsertCorrispondente(this, utente, null);
                        if (res != null && (!string.IsNullOrEmpty(res.systemId)))
                        {
                            if (!MultiCasellaManager.InsertMailCorrispondenteEsterno((CaselleList as List<MailCorrispondente>), res.systemId))
                            {
                                string msg = "ErrorAddressBookMultiEmailInsert";
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                                return;
                            }
                        }
                        break;

                }

                if (res != null && res.errore == null)
                {                   
                    this.CloseMask(res.systemId);
                }
                else
                {
                    string msg = "WarningAddressBookInsertError";
                    string err = string.Empty;
                    if (res != null && res.errore != null) err = res.errore;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '', '" + utils.FormatJs(err) + "');", true);
                    return;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private DataTable creaDataTable(ArrayList listaCorrispondenti)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("CODICE");
            dt.Columns.Add("DESCRIZIONE");
            
            for (int i = 0; i < listaCorrispondenti.Count; i++)
            {
                if (listaCorrispondenti[i] != null)
                {
                    DocsPaWR.Corrispondente c = (DocsPaWR.Corrispondente)listaCorrispondenti[i];
                    DataRow dr = dt.NewRow();
                    if (c.disabledTrasm)
                    {
                        dr[0] = "<span style=\"color:red\">" + c.codiceRubrica + "</span>";
                        dr[1] = "<span style=\"color:red\">" + c.descrizione + "</span>";
                    }
                    else
                    {
                        dr[0] = c.codiceRubrica;
                        dr[1] = c.descrizione;
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }

        private void setVisibilityPanelStar()
        {
            this.starCodAOO.Visible = false;
            this.starEmail.Visible = false;
            this.starCodAmm.Visible = false;
            this.cbInteroperanteRGS.Visible = false;
            this.cbInteroperanteRGS.Checked = false;

            switch (this.dd_canpref.SelectedItem.Text)
            {
                case "MAIL":
                    this.starEmail.Visible = true;
                    break;

                case "INTEROPERABILITA":
                    this.starEmail.Visible = true;
                    this.starCodAOO.Visible = true;
                    this.starCodAmm.Visible = true;
                    if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_ENABLE_FLUSSO_AUTOMATICO.ToString())) &&
                         Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_ENABLE_FLUSSO_AUTOMATICO.ToString()).Equals("1"))
                        this.cbInteroperanteRGS.Visible = true;
                    break;
            }
        }

        private void LoadCorrespondentTypes()
        {
            this.ddl_tipoCorr.Items.Add(new ListItem("UO", "U"));
            this.ddl_tipoCorr.Items.Add(new ListItem("RUOLO", "R"));
            this.ddl_tipoCorr.Items.Add(new ListItem("PERSONA", "P"));

            if (this.corr != null)
            {
                if (this.corr.tipoCorrispondente == "U")
                    this.ddl_tipoCorr.SelectedIndex = 0;
                else if (this.corr.tipoCorrispondente == "P")
                    this.ddl_tipoCorr.SelectedIndex = 2;
                else if (this.corr.tipoCorrispondente == "F")
                {
                    this.ddl_tipoCorr.Items.Add(new ListItem("RAGGRUPPAMENTO FUNZIONALE", "F"));
                    this.ddl_tipoCorr.SelectedIndex = 3;
                }
                else
                    this.ddl_tipoCorr.SelectedIndex = 1;
            }
        }

        private void LoadPreferredChannels(bool showIs)
        {
            this.dd_canpref.Items.Add("");
            string idAmm = UserManager.GetInfoUser().idAmministrazione;
            DocsPaWR.MezzoSpedizione[] m_sped = AddressBookManager.GetAmmListaMezzoSpedizione(idAmm, false);
            if (m_sped != null && m_sped.Length > 0)
            {
                foreach (DocsPaWR.MezzoSpedizione m_spediz in m_sped)
                {
                    if (!showIs)
                    {
                        if (!m_spediz.chaTipoCanale.ToUpper().Equals("S"))
                        {
                            ListItem item = new ListItem(m_spediz.Descrizione, m_spediz.IDSystem);
                            this.dd_canpref.Items.Add(item);
                        }
                    }
                    else
                    {
                        ListItem item = new ListItem(m_spediz.Descrizione, m_spediz.IDSystem);
                        this.dd_canpref.Items.Add(item);
                    }
                }
            }
        }

        private void LoadTitles()
        {
            ddl_titolo.Items.Add("");

            string[] listaTitoli = AddressBookManager.GetListaTitoli();
            foreach (string tit in listaTitoli)
            {
                ddl_titolo.Items.Add(tit);
            }
        }

        protected void ddl_tipoCorr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try {
                switch (((DropDownList)sender).SelectedValue)
                {
                    case "P":
                        pnl_infonascita.Visible = true;
                        pnl_nome_cogn.Visible = true;
                        txt_nome.ReadOnly = false;
                        txt_cognome.ReadOnly = false;
                        pnl_descrizione.Visible = false;
                        pnlRuolo.Visible = false;
                        pnl_titolo.Visible = true;
                        pnl_indirizzo.Visible = true;
                        break;
                    case "U":
                        pnl_infonascita.Visible = false;
                        pnl_nome_cogn.Visible = false;
                        pnl_descrizione.Visible = true;
                        pnlRuolo.Visible = false;
                        pnl_titolo.Visible = false;
                        pnl_indirizzo.Visible = true;
                        break;
                    case "R":
                        pnl_infonascita.Visible = false;
                        pnl_nome_cogn.Visible = false;
                        pnl_descrizione.Visible = true;
                        pnl_titolo.Visible = false;
                        pnl_indirizzo.Visible = false;
                        break;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void dd_canpref_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try {
                this.setVisibilityPanelStar();

                //SE L'UTENTE STA MODIFICANDO IL CANALE PREFERENZIALE DEL CORRISPONDENTE
                if (this.corr.canalePref.systemId != dd_canpref.SelectedItem.Value)
                {
                    this.IsModifiedChannel = true;
                }
                else
                {
                    this.IsModifiedChannel = false;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void RefreshScripts()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "chosen_deselect", "$('.chzn-select-deselect').chosen({ allow_single_deselect: true, no_results_text: '" + utils.FormatJs(this.GetLabel("GenericChosenSelectNone")) + "' });", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "chosen", "$('.chzn-select').chosen({ no_results_text: '" + utils.FormatJs(this.GetLabel("GenericChosenSelectNone")) + "' });", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        protected void BtnDelete_Click(object sender, EventArgs e)
        {
            try {
                string msg = "";
                Registro r = null;

                if (this.corr.idRegistro != "")
                {
                    r = RegistryManager.getRegistroBySistemId(this.corr.idRegistro);
                    if ((r.chaRF == "0") && !UserManager.IsAuthorizedFunctions("DO_MOD_CORR_REG"))
                    {
                        msg = "WarningAddressBookDeleteNotAllowed";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                        return;
                    }
                    else if ((r.chaRF == "1") && !UserManager.IsAuthorizedFunctions("DO_MOD_CORR_RF"))
                    {
                        msg = "WarningAddressBookDeleteNotAllowed";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                        return;
                    }
                }
                else if (!UserManager.IsAuthorizedFunctions("DO_MOD_CORR_TUTTI"))
                {
                    msg = "WarningAddressBookDeleteNotAllowed";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                    return;
                }

                msg = "ConfirmAddressBookDeleteCorrespondent";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "ajaxConfirmModal('" + utils.FormatJs(msg) + "', 'proceed_delete', '" + utils.FormatJs(this.GetLabel("AddressBookDetailsConfirmDelete")) + "');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void DeleteProceed()
        {
            string msg = string.Empty;
            string message = string.Empty;
            DatiModificaCorr datiModifica = new DatiModificaCorr();

            try
            {
                if (!string.IsNullOrEmpty(this.proceed_delete.Value))
                {
                    // prendo la system_id del corrispondente da eliminare
                    string idCorrGlobali = this.corr.systemId;

                    // popolo l'oggetto DatiModificaCorr necessario all'esecuzione della procedura 
                    datiModifica.idCorrGlobali = idCorrGlobali;

                    int flagListe = 0;
                    if (this.EnableDistributionLists)
                        flagListe = 1;

                    // operazione andata a buon fine
                    if (UserManager.DeleteModifyCorrispondenteEsterno(this, datiModifica, flagListe, "D", out message))
                    {
                        switch (message)
                        {
                            case "OK":
                                msg = "WarningAddressBookDeleteOk";
                                break;
                            default:
                                msg = "WarningAddressBookDeleteKo";
                                break;
                        }
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxDialogModal", "parent.parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'info', '');", true);
                        if (message == "OK") this.CloseMask("del|" + this.corr.systemId);
                    }
                    else
                    {
                        switch (message)
                        {
                            case "ERROR":
                                msg = "WarningAddressBookDeleteKo";
                                break;
                            case "NOTOK":
                                DataSet dsListe = AddressBookManager.isCorrInListaDistr(idCorrGlobali);
                                msg = "WarningAddressBookDeleteKoLists";
                                if (dsListe != null)
                                {
                                    if (dsListe.Tables.Count > 0)
                                    {
                                        DataTable tab = dsListe.Tables[0];
                                        if (tab.Rows.Count > 0)
                                        {
                                            message = "Attenzione, utente presente nelle seguenti liste di distribuzione<br />";
                                            for (int i = 0; i < tab.Rows.Count; i++)
                                            {
                                                message += tab.Rows[i]["var_desc_corr"].ToString();
                                                if (!string.IsNullOrEmpty(tab.Rows[i]["prop"].ToString()))
                                                    message += " creata da " + tab.Rows[i]["prop"].ToString();
                                                else
                                                    if (!string.IsNullOrEmpty(tab.Rows[i]["ruolo"].ToString()))
                                                        message += " creata per il ruolo " + tab.Rows[i]["ruolo"].ToString();
                                                message += "<br />";
                                            }
                                        }
                                    }
                                }
                                break;
                            default:
                                msg = "WarningAddressBookDeleteKo";
                                break;
                        }
                        if (string.IsNullOrEmpty(message))
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxDialogModal", "parent.parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'info', '');", true);
                        else
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxDialogModal", "parent.parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'info', '', '" + utils.FormatJs(message) + "');", true);
                    }
                }
                else
                {
                    this.BtnModify.Enabled = false;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        protected void DataChangedHandler(object sender, System.EventArgs e)
        {
            this.IsModified = true;
        }

        protected void txt_cap_TextChanged(object sender, System.EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            this.IsModified = true;
            try
            {
                //this.txt_citta.Text = string.Empty;
                //this.txt_provincia.Text = string.Empty;
                if (!string.IsNullOrEmpty(this.txt_cap.Text))
                {
                    string comune = string.Empty;
                    string[] infoComune = this.txt_cap.Text.Split('-');
                    string cap = infoComune[0].Trim();
                    if (infoComune.Count() > 1)
                    {
                        comune = infoComune[1].Trim();
                        InfoComune info = AddressBookManager.GetCapComuni(cap, comune);
                        if (info != null && !string.IsNullOrEmpty(info.COMUNE))
                        {
                            this.txt_cap.Text = info.CAP;
                            this.txt_citta.Text = info.COMUNE;
                            this.txt_provincia.Text = info.PROVINCIA;
                            this.RapidTxtCap.ContextKey = info.COMUNE;
                            this.UpPnlInfoComune.Update();
                        }
                    }
                }
                this.txt_citta.Focus();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void txt_citta_TextChanged(object sender, System.EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            this.IsModified = true;
            try
            {
                string comune = this.txt_citta.Text;
                this.RapidTxtCap.ContextKey = comune;
                this.txt_local.Focus();
                if (!string.IsNullOrEmpty(this.txt_citta.Text))
                {
                    InfoComune c = AddressBookManager.GetProvinciaComune(comune);
                    if (c != null && !string.IsNullOrEmpty(c.COMUNE))
                    {
                        this.txt_citta.Text = c.COMUNE;
                        this.txt_provincia.Text = c.PROVINCIA;
                        this.RapidTxtCap.MinimumPrefixLength = 0;
                        this.txt_cap.Text = string.Empty;
                        this.txt_cap.Focus();
                    }
                    this.UpPnlInfoComune.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void BtnModify_Click(object sender, EventArgs e)
        {
            try {
                string msg = string.Empty;
                string message = string.Empty;
                Registro r = null;
                DatiModificaCorr datiModifica = new DatiModificaCorr();

                if (!this.IsModified && !this.IsModifiedChannel &&
                    !this.IsModifiedCaselle && string.IsNullOrEmpty(txtCasella.Text))
                {
                    msg = "WarningAddressBookNoneModify";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                    return;
                }


                if (this.corr.idRegistro != "")
                {
                    r = RegistryManager.getRegistroBySistemId(this.corr.idRegistro);
                    if ((r.chaRF == "0") && !UserManager.IsAuthorizedFunctions("DO_MOD_CORR_REG"))
                    {
                        msg = "WarningAddressBookModifyNotAllowed";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                        return;
                    }
                    else if ((r.chaRF == "1") && !UserManager.IsAuthorizedFunctions("DO_MOD_CORR_RF"))
                    {
                        msg = "WarningAddressBookModifyNotAllowed";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                        return;
                    }
                }
                else if (!UserManager.IsAuthorizedFunctions("DO_MOD_CORR_TUTTI"))
                {
                    msg = "WarningAddressBookModifyNotAllowed";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                    return;
                }

                if (this.verificaSelezione(ref msg))
                {
                    string tipoURP = "U";
                    if (this.corr is Utente)
                    {
                        tipoURP = "P";
                    }
                    else
                    {
                        if (this.corr is Ruolo)
                        {
                            tipoURP = "R";
                        }
                    }

                    if (!string.IsNullOrEmpty(tipoURP))
                    {
                        switch (tipoURP)
                        {
                            case "U":
                                this.modifyUO(ref datiModifica);
                                break;
                            case "R":
                                this.modifyRuolo(ref datiModifica);
                                break;
                            case "P":
                                this.modifyUtente(ref datiModifica);
                                break;
                        }
                        if (this.dd_canpref.SelectedItem.Value == "")
                        {
                            for (int i = 0; i > this.dd_canpref.Items.Count; i++)
                            {
                                if (this.dd_canpref.Items[i].Text.ToUpper().Equals("LETTERA"))
                                    datiModifica.idCanalePref = this.dd_canpref.Items[i].Value;
                            }
                        }
                    }

                    // operazione andata a buon fine
                    int flagListe = 0;
                    if (this.EnableDistributionLists)
                        flagListe = 1;

                    string idNewCorr = string.Empty;
                    if (UserManager.DeleteModifyCorrispondenteEsterno(this, datiModifica, flagListe, "M", out idNewCorr, out message))
                    {
                        switch (message)
                        {
                            case "OK":
                                msg = "InfoAddressBookModifyOk";
                                //modifico eventualmente la lista delle caselle associate al corrispondente esterno
                                if (!string.IsNullOrEmpty(idNewCorr) && !idNewCorr.Equals("0"))
                                    this.InsertComboMailsCorr(idNewCorr, ref msg);
                                else
                                    this.InsertComboMailsCorr(datiModifica.idCorrGlobali, ref msg);
                                break;
                            default:
                                msg = "WarningAddressBookModifyKo";
                                break;
                        }

                        if (message == "OK")
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'info', '');", true);
                            this.CloseMask("mod");
                        }
                        else
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(message))
                        {
                            if (message.Equals("KO"))
                                msg = "WarningAddressBookModifyKo";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(msg))
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private bool verificaSelezione(ref string msg)
        {
            string corr_type = this.ddl_tipoCorr.SelectedValue;
            bool resultCheck = true;

            int indxMail = this.dd_canpref.Items.IndexOf(this.dd_canpref.Items.FindByText("MAIL"));
            int indxInterop = this.dd_canpref.Items.IndexOf(this.dd_canpref.Items.FindByText("INTEROPERABILITA"));

            if ((corr_type == "U" && this.txt_descrizione.Text.Trim() == "") ||
                (corr_type == "R" && this.txt_descrizione.Text.Trim() == "") ||
                (corr_type == "P" && (this.txt_cognome.Text.Trim() == "" || this.txt_nome.Text.Trim() == "")) ||
                (this.dd_canpref.SelectedIndex == indxInterop && (this.txt_codAmm.Text.Equals(String.Empty) ||
                    this.txt_codAOO.Text.Equals(String.Empty))))
            {
                msg = "WarningAddressBookModifyObligatory";
                resultCheck = false;
            }

            //controlli mail
            if (updPanelMail.Visible)
            {
                string pattern = "^(([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+([;](([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+)*$";
                //se attivo il multicasella
                if (gvCaselle.Rows.Count < 1 && (dd_canpref.SelectedIndex == indxMail || dd_canpref.SelectedIndex == indxInterop))
                {
                    //verifico che l'indirizzo non sia vuoto
                    if (string.IsNullOrEmpty(txtCasella.Text))
                    {
                        msg = "WarningAddressBookEmailNotInserted";
                        resultCheck = false;
                    }

                    //verifico il formato dell'indirizzo mail

                    if (!System.Text.RegularExpressions.Regex.Match(txtCasella.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), pattern).Success)
                    {
                        msg = "WarningAddressBookEmailInvalid";
                        resultCheck = false;
                    }
                    if (resultCheck)
                    {
                        CaselleList.Add(new DocsPaWR.MailCorrispondente()
                        {
                            Email = txtCasella.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                            Note = txtNote.Text,
                            Principale = "1"
                        });
                        txt_email.Text = txtCasella.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                    }
                }
                else
                {
                    foreach (GridViewRow row in gvCaselle.Rows)
                    {
                        //verifico che l'indirizzo non sia vuoto
                        if (string.IsNullOrEmpty((row.FindControl("txtEmailCorr") as System.Web.UI.WebControls.TextBox).Text))
                        {
                            msg = "WarningAddressBookEmailNotInserted";
                            resultCheck = false;
                            row.Cells[1].Focus();
                            break;
                        }
                        //verifico il formato dell'indirizzo mail
                        if (!System.Text.RegularExpressions.Regex.Match((row.FindControl("txtEmailCorr") as System.Web.UI.WebControls.TextBox).Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray())
                            , pattern).Success)
                        {
                            msg = "WarningAddressBookEmailInvalid";
                            resultCheck = false;
                            row.Cells[1].Focus();
                            break;
                        }
                    }
                    if (resultCheck)
                    {
                        bool princ = false;
                        //scrivo in txt_email la casella di posta principale
                        foreach (DocsPaWR.MailCorrispondente c in CaselleList)
                        {
                            if (c.Principale.Equals("1"))
                            {
                                princ = true;
                                this.txt_email.Text = c.Email.Trim();
                                break;
                            }
                        }
                        if (!princ) // Nel caso vengano eliminate tutte le caselle per un corrispondente con canale no interoperante/no mail, allora svuoto anche txt_email
                            txt_email.Text = string.Empty;
                    }

                }
            }
            else
            {
                // no multicasella(corrispondenti non esterni
                if ((dd_canpref.SelectedIndex == indxMail && this.txt_email.Text.Equals(String.Empty))
                || (dd_canpref.SelectedIndex == indxInterop && this.txt_email.Text.Equals(String.Empty)))
                {
                    msg = "WarningAddressBookEmailNotInserted";
                    resultCheck = false;
                }
            }

            if (pnlStandard.Visible)//caso utenti e Uo
            {
                if ((this.txt_telefono == null || this.txt_telefono.Text.Equals(""))
                    && !(this.txt_telefono2 == null || this.txt_telefono2.Text.Equals("")))
                {
                    msg = "WarningAddressBookPhoneNotInserted";
                    resultCheck = false;
                }

                //verifica del corretto formato della data di nascita nel caso in cui non sia stata cancellata
                if (this.txt_dataNascita.Text != string.Empty && !utils.isDate(this.txt_dataNascita.Text))
                {
                    msg = "WarningAddressBookBirthdayInvalid";
                    resultCheck = false;
                }

                if (this.txt_cap != null && !this.txt_cap.Text.Equals("") && !utils.isNumeric(this.txt_cap.Text))
                {
                    msg = "WarningAddressBookZipcodeInvalid";
                    resultCheck = false;
                }

                if (this.txt_provincia != null && !this.txt_provincia.Text.Equals("") && !utils.isCorrectProv(this.txt_provincia.Text))
                {
                    msg = "WarningAddressBookDistrictInvalid";
                    resultCheck = false;
                }

                if (corr_type.Equals("P"))
                {
                    if (this.txt_codfisc != null && !this.txt_codfisc.Text.Equals("") && (utils.CheckTaxCode(this.txt_codfisc.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray())) != 0))
                    {
                        msg = "WarningAddressBookTaxIdInvalid";
                        resultCheck = false;
                    }
                }
                else
                    if (corr_type.Equals("U"))
                    {
                        if ((this.txt_codfisc != null && !this.txt_codfisc.Text.Trim().Equals("")) && ((this.txt_codfisc.Text.Trim().Length == 11 && utils.CheckVatNumber(this.txt_codfisc.Text.Trim()) != 0) || (this.txt_codfisc.Text.Trim().Length == 16 && utils.CheckTaxCode(this.txt_codfisc.Text.Trim()) != 0) || (this.txt_codfisc.Text.Trim().Length != 11 && this.txt_codfisc.Text.Trim().Length != 16)))
                        {
                            msg = "WarningAddressBookTaxIdInvalid";
                            resultCheck = false;
                        }
                    }

                if (this.txt_partita_iva != null && !this.txt_partita_iva.Text.Equals("") && (utils.CheckVatNumber(this.txt_partita_iva.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray())) != 0))
                {
                    msg = "WarningAddressBookVatInvalid";
                    resultCheck = false;
                }

                if (!updPanelMail.Visible) // per il multicasella non ripeto i controlli sulla mail
                {
                    if (dd_canpref.SelectedIndex == indxMail || dd_canpref.SelectedIndex == indxInterop)
                    {
                        if (string.IsNullOrEmpty(this.txt_email.Text) || txt_email.Text.Trim().Equals(string.Empty) || !utils.IsValidEmail(this.txt_email.Text.Trim()))
                        {
                            msg = "WarningAddressBookEmailNotInserted";
                            resultCheck = false;
                        }
                    }
                }
            }

            return resultCheck;
        }

        /// <summary>
        /// reperimento dei dati per la modifica di una uo
        /// </summary>
        /// <param name="datiModifica"></param>
        private void modifyUO(ref DatiModificaCorr datiModifica)
        {
            datiModifica.idCorrGlobali = this.corr.systemId;
            datiModifica.descCorr = this.txt_descrizione.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.codiceAoo = this.txt_codAOO.Text;
            datiModifica.codiceAmm = this.txt_codAmm.Text;
            datiModifica.email = this.txt_email.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.codice = this.txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.codRubrica = this.txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.nome = "";
            datiModifica.cognome = "";
            datiModifica.indirizzo = this.txt_indirizzo.Text;
            datiModifica.cap = this.txt_cap.Text;
            datiModifica.provincia = this.txt_provincia.Text;
            datiModifica.nazione = this.txt_nazione.Text;
            datiModifica.citta = this.txt_citta.Text;
            datiModifica.codFiscale = this.txt_codfisc.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.partitaIva = this.txt_partita_iva.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            //datiModifica.codiceIpa = this.txt_codice_ipa.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.telefono = this.txt_telefono.Text;
            datiModifica.telefono2 = this.txt_telefono2.Text;
            datiModifica.note = this.txt_note.Text;
            datiModifica.fax = this.txt_fax.Text;
            datiModifica.localita = this.txt_local.Text;
            datiModifica.idCanalePref = this.dd_canpref.SelectedItem.Value;
            datiModifica.luogoNascita = string.Empty;
            datiModifica.dataNascita = string.Empty;
            datiModifica.titolo = string.Empty;
            datiModifica.interoperanteRGS = this.cbInteroperanteRGS.Checked;
        }

        /// <summary>
        /// reperimento dei dati per la modifica di un ruolo
        /// </summary>
        /// <param name="datiModifica"></param>
        private void modifyRuolo(ref DatiModificaCorr datiModifica)
        {
            datiModifica.idCorrGlobali = this.corr.systemId;
            datiModifica.descCorr = this.txt_DescR.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.codiceAoo = this.txt_codAOO.Text;
            datiModifica.codiceAmm = this.txt_codAmm.Text;
            datiModifica.email = this.txt_email.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.codice = this.txt_CodR.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.codRubrica = this.txt_CodR.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.nome = String.Empty;
            datiModifica.cognome = String.Empty;
            datiModifica.indirizzo = String.Empty;
            datiModifica.cap = String.Empty;
            datiModifica.provincia = String.Empty;
            datiModifica.nazione = String.Empty;
            datiModifica.citta = String.Empty;
            datiModifica.codFiscale = String.Empty;
            datiModifica.partitaIva = String.Empty;
            datiModifica.telefono = String.Empty;
            datiModifica.telefono2 = String.Empty;
            datiModifica.note = String.Empty;
            datiModifica.fax = String.Empty;
            datiModifica.idCanalePref = dd_canpref.SelectedItem.Value;
            datiModifica.dataNascita = String.Empty;
            datiModifica.luogoNascita = String.Empty;
            datiModifica.titolo = String.Empty;
            datiModifica.interoperanteRGS = this.cbInteroperanteRGS.Checked;
        }

        /// <summary>
        /// reperimento dei dati per la modifica di un utente
        /// </summary>
        /// <param name="datiModifica"></param>
        private void modifyUtente(ref DatiModificaCorr datiModifica)
        {
            datiModifica.idCorrGlobali = this.corr.systemId;
            datiModifica.codiceAoo = this.txt_codAOO.Text;
            datiModifica.codiceAmm = this.txt_codAmm.Text;
            datiModifica.email = this.txt_email.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.codice = this.txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.codRubrica = this.txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.nome = this.txt_nome.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.cognome = this.txt_cognome.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.titolo = this.ddl_titolo.SelectedItem.Value;
            if (!string.IsNullOrEmpty(datiModifica.titolo))
                datiModifica.descCorr = datiModifica.titolo + " " + datiModifica.cognome + " " + datiModifica.nome;
            else
                datiModifica.descCorr = datiModifica.cognome + " " + datiModifica.nome;
            datiModifica.indirizzo = this.txt_indirizzo.Text;
            datiModifica.cap = this.txt_cap.Text;
            datiModifica.provincia = this.txt_provincia.Text;
            datiModifica.nazione = this.txt_nazione.Text;
            datiModifica.citta = this.txt_citta.Text;
            datiModifica.codFiscale = this.txt_codfisc.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.partitaIva = this.txt_partita_iva.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            //datiModifica.codiceIpa = this.txt_codice_ipa.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.telefono = this.txt_telefono.Text;
            datiModifica.telefono2 = this.txt_telefono2.Text;
            datiModifica.note = this.txt_note.Text;
            datiModifica.fax = this.txt_fax.Text;
            datiModifica.localita = this.txt_local.Text;
            datiModifica.idCanalePref = dd_canpref.SelectedItem.Value;
            datiModifica.luogoNascita = this.txt_luogoNascita.Text;
            datiModifica.dataNascita = this.txt_dataNascita.Text;
            datiModifica.tipoCorrispondente = "P";
            datiModifica.interoperanteRGS = this.cbInteroperanteRGS.Checked;
        }

        protected bool InsertComboMailsCorr(string idCorrispondente, ref string msg)
        {
            bool res = true;

            if (!string.IsNullOrEmpty(idCorrispondente))
            {
                // modifico eventualmente la lista delle caselle associate al corrispondente esterno
                if (CaselleList.Count > 0)
                {
                    res = MultiCasellaManager.InsertMailCorrispondenteEsterno(CaselleList, idCorrispondente);
                    if (!res)
                    {
                        msg = "ErrorAddressBookMultiEmailModify";
                    }
                }
                if (CaselleList.Count == 0)
                {
                    res = MultiCasellaManager.DeleteMailCorrispondenteEsterno(idCorrispondente);
                    if (!res)
                    {
                        msg = "ErrorAddressBookMultiEmailModify";
                    }
                }
            }

            return res;
        }

        protected bool TypeMailCorrEsterno(string typeMail)
        {
            return (typeMail.Equals("1")) ? true : false;
        }

        protected void BindGridViewCaselle(Corrispondente corr)
        {
            if (CaselleList == null)
                CaselleList = MultiCasellaManager.GetMailCorrispondenteEsterno(corr.systemId);
            gvCaselle.DataSource = CaselleList;
            gvCaselle.DataBind();

            //se è disabilitato il multicasella, dopo l'immissione di una casella disabilito il pulsante aggiungi.
            if (this.CaselleList.Count > 0 && !MultiCasellaManager.IsEnabledMultiMail(RoleManager.GetRoleInSession().idAmministrazione))
            {
                txtCasella.Enabled = false;
                txtNote.Enabled = false;
                imgAggiungiCasella.Enabled = false;
            }
        }

        protected void EnableInsertMail()
        {
            if (this.IsEmailReadOnly)
            {
                this.txtCasella.Enabled = false;
                this.txtNote.Enabled = false;
                this.imgAggiungiCasella.Enabled = false;
            }
            else
            {
                if (this.corr != null && !this.corr.inRubricaComune)
                {
                    this.txtCasella.Enabled = true;
                    this.txtNote.Enabled = true;
                    this.imgAggiungiCasella.Enabled = true;
                }
            }
        }

        protected void gvCaselle_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try {
                if (e.Row.RowType == DataControlRowType.DataRow && this.IsEmailReadOnly)
                {
                    (e.Row.FindControl("txtEmailCorr") as System.Web.UI.WebControls.TextBox).ReadOnly = true;
                    (e.Row.FindControl("txtNoteMailCorr") as System.Web.UI.WebControls.TextBox).ReadOnly = true;
                    (e.Row.FindControl("rdbPrincipale") as System.Web.UI.WebControls.RadioButton).Enabled = false;
                    (e.Row.FindControl("imgEliminaCasella") as System.Web.UI.WebControls.ImageButton).Enabled = false;
                }
                else if (e.Row.RowType == DataControlRowType.DataRow && (!this.IsEmailReadOnly))
                {
                    if (this.corr != null && this.corr.inRubricaComune)
                    {
                        (e.Row.FindControl("txtEmailCorr") as System.Web.UI.WebControls.TextBox).ReadOnly = true;
                        (e.Row.FindControl("txtNoteMailCorr") as System.Web.UI.WebControls.TextBox).ReadOnly = true;
                        (e.Row.FindControl("rdbPrincipale") as System.Web.UI.WebControls.RadioButton).Enabled = false;
                        (e.Row.FindControl("imgEliminaCasella") as System.Web.UI.WebControls.ImageButton).Enabled = false;
                    }
                    else
                    {
                        (e.Row.FindControl("txtEmailCorr") as System.Web.UI.WebControls.TextBox).ReadOnly = false;
                        (e.Row.FindControl("txtNoteMailCorr") as System.Web.UI.WebControls.TextBox).ReadOnly = false;
                        (e.Row.FindControl("rdbPrincipale") as System.Web.UI.WebControls.RadioButton).Enabled = true;
                        (e.Row.FindControl("imgEliminaCasella") as System.Web.UI.WebControls.ImageButton).Enabled = true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void txtEmailCorr_TextChanged(object sender, EventArgs e)
        {
            try {
                this.IsModifiedCaselle = true;
                string newMail = (sender as System.Web.UI.WebControls.TextBox).Text;
                int rowModify = ((sender as System.Web.UI.WebControls.TextBox).Parent.Parent as System.Web.UI.WebControls.GridViewRow).RowIndex;
                CaselleList[rowModify].Email = newMail;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void txtNoteMailCorr_TextChanged(object sender, EventArgs e)
        {
            try {
                this.IsModifiedCaselle = true;
                string newNote = (sender as System.Web.UI.WebControls.TextBox).Text;
                int rowModify = ((sender as System.Web.UI.WebControls.TextBox).Parent.Parent as System.Web.UI.WebControls.GridViewRow).RowIndex;
                CaselleList[rowModify].Note = newNote;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void rdbPrincipale_ChecekdChanged(object sender, EventArgs e)
        {
            try {
                string mailSelect = (((sender as RadioButton).Parent.Parent as System.Web.UI.WebControls.GridViewRow).FindControl("txtEmailCorr") as TextBox).Text;
                List<DocsPaWR.MailCorrispondente> listCaselle = CaselleList;
                foreach (DocsPaWR.MailCorrispondente c in listCaselle)
                {
                    if (c.Email.Trim().Equals(mailSelect.Trim()))
                        c.Principale = "1";
                    else
                        c.Principale = "0";
                }
                CaselleList = listCaselle as List<DocsPaWR.MailCorrispondente>;
                gvCaselle.DataSource = CaselleList;
                gvCaselle.DataBind();
                this.IsModifiedCaselle = true;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void imgEliminaCasella_Click(object sender, ImageClickEventArgs e)
        {
            try {
                bool isComboMain = (((sender as System.Web.UI.WebControls.ImageButton).Parent.Parent as System.Web.UI.WebControls.GridViewRow).
                                        FindControl("rdbPrincipale") as RadioButton).Checked;
                //se presenti più caselle e si tenta di eliminare una casella settata come principale il sistema avvisa l'utente
                if (isComboMain && CaselleList.Count > 1)
                {
                    string msg = "WarningAddressBookDeletingPrimaryEmail";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                    return;
                }
                int indexRowDelete = ((sender as System.Web.UI.WebControls.ImageButton).Parent.Parent as System.Web.UI.WebControls.GridViewRow).RowIndex;
                CaselleList.RemoveAt(indexRowDelete);
                gvCaselle.DataSource = CaselleList;
                gvCaselle.DataBind();
                if (CaselleList.Count < 1 && !MultiCasellaManager.IsEnabledMultiMail(RoleManager.GetRoleInSession().idAmministrazione))
                {
                    txtCasella.Enabled = true;
                    txtNote.Enabled = true;
                    imgAggiungiCasella.Enabled = true;
                }
                this.IsModifiedCaselle = true;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void imgAggiungiCasella_Click(object sender, ImageClickEventArgs e)
        {
            try {
                //verifico che l'indirizzo non sia vuoto
                if (string.IsNullOrEmpty(this.txtCasella.Text))
                {
                    string msg = "WarningAddressBookEmailNotInserted";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                    return;
                }

                //verifico il formato dell'indirizzo mail
                string pattern = "^(([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+([;](([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+)*$";
                if (!System.Text.RegularExpressions.Regex.Match(this.txtCasella.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), pattern).Success)
                {
                    string msg = "WarningAddressBookEmailNotInserted";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                    return;
                }

                //verifico che la casella non sia già stata associata al corrispondente       
                if (CaselleList != null)
                {
                    foreach (DocsPaWR.MailCorrispondente c in CaselleList)
                    {
                        if (c.Email.Trim().Equals(this.txtCasella.Text.Trim()))
                        {
                            string msg = "WarningAddressBookEmailAlreadyInserted";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                            return;
                        }
                    }
                }
                CaselleList.Add(new DocsPaWR.MailCorrispondente()
                {
                    Email = this.txtCasella.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                    Note = txtNote.Text,
                    Principale = gvCaselle.Rows.Count < 1 ? "1" : "0"
                });
                if (CaselleList.Count > 0)
                {
                    //this.txtCasella.Enabled = false;
                    //txtNote.Enabled = false;
                    //imgAggiungiCasella.Enabled = false;
                }
                this.gvCaselle.DataSource = CaselleList;
                this.gvCaselle.DataBind();
                this.IsModifiedCaselle = true;

                this.txtCasella.Text = string.Empty;
                this.txtNote.Text = string.Empty;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

    }
}