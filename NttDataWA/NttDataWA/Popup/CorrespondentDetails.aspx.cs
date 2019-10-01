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
    public partial class CorrespondentDetails : System.Web.UI.Page
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

        public DocsPaWR.Corrispondente TempSender
        {
            get
            {
                DocsPaWR.Corrispondente result = null;
                if (HttpContext.Current.Session["tempSender"] != null)
                {
                    result = HttpContext.Current.Session["tempSender"] as DocsPaWR.Corrispondente;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["tempSender"] = value;
            }
        }

        public DocsPaWR.Corrispondente Richiedente
        {
            get
            {
                DocsPaWR.Corrispondente result = null;
                if (HttpContext.Current.Session["richiedente"] != null)
                {
                    result = HttpContext.Current.Session["richiedente"] as DocsPaWR.Corrispondente;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["richiedente"] = value;
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

        public string tipoCor
        {
            get
            {
                if (Session["CorrespondentDetails_Type"] != null) return Session["CorrespondentDetails_Type"].ToString();
                return null;
            }
            set
            {
                Session["CorrespondentDetails_Type"] = value;
            }
        }

        public string indexCor
        {
            get
            {
                if (Session["CorrespondentDetails_Index"] != null) return Session["CorrespondentDetails_Index"].ToString();
                return null;
            }
            set
            {
                Session["CorrespondentDetails_Index"] = value;
            }
        }

        public bool readOnly
        {
            get
            {
                if (Session["CorrespondentDetails_readOnly"] == null) return false;
                return (bool)Session["CorrespondentDetails_readOnly"];
            }
            set
            {
                Session["CorrespondentDetails_readOnly"] = value;
            }
        }

        public DocsPaWR.Corrispondente corr
        {
            get
            {
                return Session["CorrespondentDetails_corr"] as DocsPaWR.Corrispondente;
            }
            set
            {
                Session["CorrespondentDetails_corr"] = value;
            }
        }

        #endregion


        private void Page_Load(object sender, System.EventArgs e)
        {
            try {
                if (!this.Page.IsPostBack)
                {
                    if (!string.IsNullOrEmpty(this.tipoCor) && !string.IsNullOrEmpty(this.indexCor))
                    {
                        this.InitLanguage();


                        if (this.indexCor.Contains("________"))
                        {
                            this.indexCor = this.indexCor.Replace("________", "&");
                        }

                        if ((this.tipoCor == null) || (this.tipoCor == ""))
                            return;
                        if (this.DocumentInWorking == null)
                            return;

                        if (this.tipoCor.Equals("M"))
                        {
                            if (this.DocumentInWorking.tipoProto == "A")
                            {
                                this.btn_CreaCorDoc.Visible = true;

                                //INC000000833221 -PITRE - Comune di Trento
                                //corr = this.TempSender;
                                if (!string.IsNullOrEmpty(this.TempSender.systemId))
                                    corr = UIManager.AddressBookManager.GetCorrespondentBySystemId(this.TempSender.systemId);
                                else
                                    corr = this.TempSender;

                                if (corr == null)
                                //da protocollo inserendo solo la descrizione e aprendo subito dopo la maschera di dettagli,
                                //la pagina non ha ancora creato il mitt, perchè non c'è stato alcun post back, quindi lo creo io.
                                {
                                    corr = new DocsPaWR.Corrispondente();
                                    corr.tipoCorrispondente = "O";
                                    corr.idAmministrazione = UserManager.GetUserInSession().idAmministrazione;
                                    corr.descrizione = this.indexCor;
                                }
                            }
                            if (this.DocumentInWorking.tipoProto == "P")
                            {
                                this.btn_CreaCorDoc.Visible = false;
                                corr = this.TempSender;
                                readOnly = true;
                            }
                            if (this.DocumentInWorking.tipoProto == "I")
                            {
                                this.btn_CreaCorDoc.Visible = false;
                                corr = this.TempSender;
                                readOnly = true;
                            }

                        }
                        else if (this.tipoCor.Equals("I"))
                        {
                            this.btn_CreaCorDoc.Visible = true;
                            corr = this.SenderIntermediate;
                        }
                        else if (this.tipoCor.Equals("D"))
                        {
                            if (this.indexCor == null || this.indexCor.Equals("") || Int32.Parse(this.indexCor) < 0)
                                return;

                            corr = this.ListRecipients[Int32.Parse(this.indexCor)];
                            if (this.DocumentInWorking.tipoProto == "P")
                                this.btn_CreaCorDoc.Visible = true;
                            else
                            {
                                this.btn_CreaCorDoc.Visible = false;
                                readOnly = true;
                            }
                        }
                        else if (this.tipoCor.Equals("C"))
                        {
                            if (this.indexCor == null || this.indexCor.Equals("") || Int32.Parse(this.indexCor) < 0)
                                return;
                            corr = this.ListRecipientsCC[Int32.Parse(this.indexCor)];
                            if (this.DocumentInWorking.tipoProto == "P")
                                this.btn_CreaCorDoc.Visible = true;
                            else
                            {
                                this.btn_CreaCorDoc.Visible = false;
                                readOnly = true;
                            }
                        }
                        else if (this.tipoCor.Equals("MD"))
                        {
                            this.btn_CreaCorDoc.Visible = false;
                            readOnly = true;

                            if (this.DocumentInWorking.tipoProto == "A")
                            {
                                corr = this.MultipleSenders[Int32.Parse(this.indexCor)];

                                if (corr == null)
                                //da protocollo inserendo solo la descrizione e aprendo subito dopo la maschera di dettagli,
                                //la pagina non ha ancora creato il mitt, perchè non c'è stato alcun post back, quindi lo creo io.
                                {
                                    corr = new DocsPaWR.Corrispondente();
                                    corr.tipoCorrispondente = "O";
                                    corr.idAmministrazione = UserManager.GetUserInSession().idAmministrazione;
                                    corr.descrizione = this.indexCor;
                                }
                            }
                        }

                        if (corr == null)
                            return;
                        if (!string.IsNullOrEmpty(corr.tipoCorrispondente) && corr.tipoCorrispondente.Equals("C"))
                        {
                            corr = UIManager.AddressBookManager.GetCorrespondentBySystemId(corr.systemId);
                        }
                        if (!string.IsNullOrEmpty(corr.tipoCorrispondente) && ((corr.tipoCorrispondente.Equals("F") && !corr.inRubricaComune) || corr.tipoCorrispondente.Equals("L")))
                        {
                            this.PanelListaCorrispondenti.Visible = true;
                            this.pnl_notcommon.Visible = false;
                            this.pnl_email.Visible = false;
                            this.btn_CreaCorDoc.Visible = false;
                            readOnly = true;

                            ArrayList listaCorrispondenti = new ArrayList();
                            if (corr.tipoCorrispondente.Equals("F"))
                            {
                                listaCorrispondenti = UserManager.getCorrispondentiByCodRF(this, corr.codiceRubrica);
                                this.lbl_nomeCorr.Text = UserManager.getNomeRF(this, corr.codiceRubrica);
                            }
                            else
                            {
                                listaCorrispondenti = UserManager.getCorrispondentiByCodLista(this, corr.codiceRubrica, UserManager.GetInfoUser().idAmministrazione);
                                this.lbl_nomeCorr.Text = UserManager.getNomeLista(this, corr.codiceRubrica, UserManager.GetInfoUser().idAmministrazione);
                            }

                            this.dg_listCorr.Columns[0].HeaderText = this.GetLabel("CorrespondentDetailsCode");
                            this.dg_listCorr.Columns[1].HeaderText = this.GetLabel("CorrespondentDetailsDescription");

                            this.dg_listCorr.DataSource = this.creaDataTable(listaCorrispondenti);
                            this.dg_listCorr.DataBind();
                        }


                        if ((this.DocumentInWorking.systemId != null) && (!this.DocumentInWorking.predisponiProtocollazione))
                        {
                            if (!string.IsNullOrEmpty(DocumentInWorking.tipoProto) && (DocumentInWorking.tipoProto.Equals("A") || DocumentInWorking.tipoProto.Equals("P")))
                            {
                                readOnly = false;

                                if (DocumentInWorking.tipoProto.Equals("P") && corr != null && !string.IsNullOrEmpty(corr.systemId))
                                    readOnly = true;
                            }
                            else
                                readOnly = true;

                            //if (Convert.ToInt32(this.DocumentInWorking.accessRights) < Convert.ToInt32(HMdiritti.HMdiritti_Write))
                            //{
                            //    readOnly = true;
                            //}

                            //if (!UIManager.UserManager.IsAuthorizedFunctions("DO_PROT_DEST_MODIFICA"))
                            //{
                            //    readOnly = true;
                            //}

                            // se si tratta di un destinatario visualizzo la mail principale
                            if (!string.IsNullOrEmpty(corr.tipoIE) && corr.tipoIE.Equals("E"))
                            {
                                this.lbl_email.Text = this.GetLabel("CorrespondentDetailsMainEmail");
                                corr.email = (from c in MultiCasellaManager.GetMailCorrispondenteEsterno(corr.systemId) where c.Principale.Equals("1") select c).Count() > 0 ?
                                    "* " + (from c in MultiCasellaManager.GetMailCorrispondenteEsterno(corr.systemId) where c.Principale.Equals("1") select c.Email).First() : string.Empty;
                            }
                        }

                        this.getInfoCor(corr);
                        if (corr != null && !string.IsNullOrEmpty(corr.email))
                            corr.email = corr.email.Replace("* ", "");


                        // La creazione del corrispondente occasionale è consentita solo 
                        // per i corrispondenti che non provengono da rubrica comune
                        if (!readOnly) readOnly = !(this.corr != null && !this.corr.inRubricaComune);

                        this.setFieldsProperty();

                        this.BindGridViewCaselle(corr);
                    }
                    else if (Request.QueryString["instanceDetails"] != null && Request.QueryString["instanceDetails"].Equals("r"))//CASO IN CUI LA POPUP è CHIAMATA DALLA MASCHERA INSTANCED DETAILS
                    {
                        this.InitLanguage();
                        if (this.Richiedente.tipoCorrispondente.Equals("O") || string.IsNullOrEmpty(Richiedente.tipoCorrispondente))
                        {                         
                            corr = this.Richiedente;
                            corr.tipoCorrispondente = "O";
                            corr.idAmministrazione = UserManager.GetUserInSession().idAmministrazione;
                            if (string.IsNullOrEmpty(corr.systemId))
                            {
                                this.btn_CreaCorDoc.Visible = true;
                                readOnly = false;
                            }
                        }
                        else
                        {
                            this.btn_CreaCorDoc.Visible = false;
                            corr = this.Richiedente;
                            readOnly = true;
                        }
                        if (corr == null)
                            return;
                        if (corr.tipoCorrispondente.Equals("C"))
                        {
                            corr = UIManager.AddressBookManager.GetCorrespondentBySystemId(corr.systemId);
                        }
                        if (!string.IsNullOrEmpty(corr.tipoCorrispondente) && ((corr.tipoCorrispondente.Equals("F") && !corr.inRubricaComune) || corr.tipoCorrispondente.Equals("L")))
                        {
                            this.PanelListaCorrispondenti.Visible = true;
                            this.pnl_notcommon.Visible = false;
                            this.pnl_email.Visible = false;
                            this.btn_CreaCorDoc.Visible = false;
                            readOnly = true;

                            ArrayList listaCorrispondenti = new ArrayList();
                            if (corr.tipoCorrispondente.Equals("F"))
                            {
                                listaCorrispondenti = UserManager.getCorrispondentiByCodRF(this, corr.codiceRubrica);
                                this.lbl_nomeCorr.Text = UserManager.getNomeRF(this, corr.codiceRubrica);
                            }
                            else
                            {
                                listaCorrispondenti = UserManager.getCorrispondentiByCodLista(this, corr.codiceRubrica, UserManager.GetInfoUser().idAmministrazione);
                                this.lbl_nomeCorr.Text = UserManager.getNomeLista(this, corr.codiceRubrica, UserManager.GetInfoUser().idAmministrazione);
                            }

                            this.dg_listCorr.Columns[0].HeaderText = this.GetLabel("CorrespondentDetailsCode");
                            this.dg_listCorr.Columns[1].HeaderText = this.GetLabel("CorrespondentDetailsDescription");

                            this.dg_listCorr.DataSource = this.creaDataTable(listaCorrispondenti);
                            this.dg_listCorr.DataBind();
                        }



                            
                        if (!string.IsNullOrEmpty(corr.tipoIE) && corr.tipoIE.Equals("E"))
                        {
                            this.lbl_email.Text = this.GetLabel("CorrespondentDetailsMainEmail");
                            corr.email = (from c in MultiCasellaManager.GetMailCorrispondenteEsterno(corr.systemId) where c.Principale.Equals("1") select c).Count() > 0 ?
                                "* " + (from c in MultiCasellaManager.GetMailCorrispondenteEsterno(corr.systemId) where c.Principale.Equals("1") select c.Email).First() : string.Empty;
                        }


                        this.getInfoCor(corr);
                        if (corr != null && !string.IsNullOrEmpty(corr.email))
                            corr.email = corr.email.Replace("* ", "");


                        // La creazione del corrispondente occasionale è consentita solo 
                        // per i corrispondenti che non provengono da rubrica comune
                        if (!readOnly) readOnly = !(this.corr != null && !this.corr.inRubricaComune);

                        this.setFieldsProperty();

                        this.BindGridViewCaselle(corr);
                    }
                }
                RefreshScript();
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
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("GenericBtnClose", language);
            this.btn_CreaCorDoc.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsBtnCreateCorrDoc", language);
            this.lbl_indirizzo.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsAddress", language);
            this.lbl_cap.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsZipCode", language);
            this.lbl_citta.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsCity", language);
            this.lbl_provincia.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsDistrict", language);
            this.lbl_local.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsPlace", language);
            this.lbl_nazione.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsCountry", language);
            this.lbl_telefono.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsPhone", language);
            this.lbl_telefono2.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsPhone2", language);
            this.lbl_fax.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsFax", language);
            this.lbl_dtnasc.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsBirthday", language);
            this.lbl_codfisc.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsTaxId", language);
            this.lbl_partita_iva.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsCommercialId", language);
            //this.lbl_codice_ipa.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsIpaCode", language);
            this.lbl_email.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsEmail", language);
            this.lbl_codAOO.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsCodAOO", language);
            this.lbl_codAmm.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsCodAdmin", language);
            this.lbl_note.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsNote", language);
            this.lbl_nome.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsName", language);
            this.lbl_cognome.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsSurname", language);
            this.lbl_canalePreferenziale.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsPreferredChannel", language);
            this.lbl_tipocorr.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsType", language);
            this.lbl_registro.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsRegistry", language);
            this.lbl_luogoNasc.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsBirthplace", language);
            this.lbl_titolo.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsTitolo", language);
            this.ddl_titolo.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectOptionNeutral", language));
            this.gvCaselle.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("CorrespondentDetailsEmail", language);
            this.gvCaselle.Columns[2].HeaderText = Utils.Languages.GetLabelFromCode("CorrespondentDetailsEmailNote", language);
            this.gvCaselle.Columns[3].HeaderText = "*";
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        private void setFieldsProperty()
        {
            this.txt_cap.ReadOnly = readOnly;
            this.txt_citta.ReadOnly = readOnly;
            this.txt_codfisc.ReadOnly = readOnly;
            this.txt_partita_iva.ReadOnly = readOnly;
            //this.txt_codice_ipa.ReadOnly = readOnly;
            this.txt_email.ReadOnly = readOnly;
            this.txt_fax.ReadOnly = readOnly;
            this.txt_luogoNasc.ReadOnly = readOnly;
            this.txt_dtnasc.ReadOnly = readOnly;
            this.txt_indirizzo.ReadOnly = readOnly;
            this.txt_nazione.ReadOnly = readOnly;
            this.txt_note.ReadOnly = readOnly;
            this.txt_provincia.ReadOnly = readOnly;
            this.txt_telefono.ReadOnly = readOnly;
            this.txt_telefono2.ReadOnly = readOnly;
            this.txt_local.ReadOnly = readOnly;
            this.txt_codAOO.ReadOnly = readOnly;
            this.txt_codAmm.ReadOnly = readOnly;
            this.btn_CreaCorDoc.Enabled = !readOnly;
            this.txt_cognome.ReadOnly = readOnly;
            this.txt_nome.ReadOnly = readOnly;
            this.ddl_titolo.Enabled = !readOnly;
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            try {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                this.CloseMask(false);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void btn_CreaCorDoc_Click(object sender, EventArgs e)
        {
            try {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                if (!this.HasChanged())
                {
                    string msg = "InfoCorrespondentDetailsNotModified";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');}", true);
                    return;
                }
                if (!this.checkFields())
                    return;

                this.creaCorr();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private bool HasChanged()
        {
            if (corr.info == null)
            {
                DocsPaWR.CorrespondentDetails corrDetails = AddressBookManager.GetCorrespondentDetails(corr.systemId);
                if (corrDetails != null)
                {
                    if (this.txt_indirizzo.Text != corrDetails.Address) return true;
                    if (this.txt_cap.Text != corrDetails.ZipCode) return true;
                    if (this.txt_citta.Text != corrDetails.City) return true;
                    if (this.txt_provincia.Text != corrDetails.District) return true;
                    if (this.txt_local.Text != corrDetails.Place) return true;
                    if (this.txt_nazione.Text != corrDetails.Country) return true;
                    if (this.txt_telefono.Text != corrDetails.Phone) return true;
                    if (this.txt_telefono2.Text != corrDetails.Phone2) return true;
                    if (this.txt_fax.Text != corrDetails.Fax) return true;
                    if (this.txt_dtnasc.Text != corrDetails.BirthDay) return true;
                    if (this.txt_codfisc.Text != corrDetails.TaxId) return true;
                    if (this.txt_partita_iva.Text != corrDetails.CommercialId) return true;
                    //if (this.txt_codice_ipa.Text != corrDetails.IpaCode) return true;
                    if (this.txt_luogoNasc.Text != corrDetails.BirthPlace) return true;
                    if (this.txt_dtnasc.Text != corrDetails.BirthDay) return true;
                    if (!this.ddl_titolo.SelectedValue.Equals(corrDetails.Title)) return true;
                }
            }
            else if (corr.info.Tables["Corrispondente"].Rows.Count > 0)
            {
                if (this.txt_indirizzo.Text != corr.info.Tables["Corrispondente"].Rows[0]["indirizzo"].ToString()) return true;
                if (this.txt_cap.Text != corr.info.Tables["Corrispondente"].Rows[0]["cap"].ToString()) return true;
                if (this.txt_citta.Text != corr.info.Tables["Corrispondente"].Rows[0]["citta"].ToString()) return true;
                if (this.txt_provincia.Text != corr.info.Tables["Corrispondente"].Rows[0]["provincia"].ToString()) return true;
                if (this.txt_local.Text != corr.info.Tables["Corrispondente"].Rows[0]["localita"].ToString()) return true;
                if (this.txt_nazione.Text != corr.info.Tables["Corrispondente"].Rows[0]["nazione"].ToString()) return true;
                if (this.txt_telefono.Text != corr.info.Tables["Corrispondente"].Rows[0]["telefono"].ToString()) return true;
                if (this.txt_telefono2.Text != corr.info.Tables["Corrispondente"].Rows[0]["telefono2"].ToString()) return true;
                if (this.txt_fax.Text != corr.info.Tables["Corrispondente"].Rows[0]["fax"].ToString()) return true;
                if (this.txt_codfisc.Text != corr.info.Tables["Corrispondente"].Rows[0]["codiceFiscale"].ToString()) return true;
                if (this.txt_partita_iva.Text != corr.info.Tables["Corrispondente"].Rows[0]["partitaIva"].ToString()) return true;
                //if (this.txt_codice_ipa.Text != corr.info.Tables["Corrispondente"].Rows[0]["codiceIpa"].ToString()) return true;
                if (this.txt_luogoNasc.Text != corr.info.Tables["Corrispondente"].Rows[0]["luogoNascita"].ToString()) return true;
                if (this.txt_dtnasc.Text != corr.info.Tables["Corrispondente"].Rows[0]["dataNascita"].ToString()) return true;
                if (!this.ddl_titolo.SelectedValue.Equals(corr.info.Tables["Corrispondente"].Rows[0]["titolo"].ToString())) return true;
            }

            if (corr.email != this.txt_email.Text) return true;
            if (corr.codiceAOO != this.txt_codAOO.Text) return true;
            if (corr.codiceAmm != this.txt_codAmm.Text) return true;

            return false;
        }

        private void ClearSessionData()
        {
            this.tipoCor = null;
            this.indexCor = null;
            this.readOnly = false;
            this.corr = null;
        }

        protected void CloseMask(bool withReturnValue)
        {


            string returnValue = string.Empty;
            if (Request.QueryString["instanceDetails"] == null || !Request.QueryString["instanceDetails"].Equals("r"))
            {
                if (withReturnValue) returnValue = this.tipoCor;
            }
            else
                returnValue = "up"; 
            this.ClearSessionData();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('CorrespondentDetails', '" + returnValue + "');} else {parent.closeAjaxModal('CorrespondentDetails', '" + returnValue + "');};", true);
        }

        private string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
        }

        protected void BindGridViewCaselle(DocsPaWR.Corrispondente corr)
        {
            List<DocsPaWR.MailCorrispondente> listaMail = MultiCasellaManager.GetMailCorrispondenteEsterno(corr.systemId);
            if (listaMail != null && listaMail.Count > 0)
            {
                if (listaMail.Count == 1)
                {
                    this.lbl_email.Visible = true;
                    this.txt_email.Visible = true;
                    this.plc_gvCaselle.Visible = false;
                }
                else
                {
                    this.plcSingleMail.Visible = false;
                    this.plc_gvCaselle.Visible = true;
                    this.gvCaselle.DataSource = listaMail;
                    this.gvCaselle.DataBind();
                }
            }
        }

        private void getInfoCor(DocsPaWR.Corrispondente corr)
        {
            if (corr != null)
            {
                // con codice IPA
                //Literal[] dettLabel = { this.lbl_indirizzo, this.lbl_cap, this.lbl_citta, this.lbl_provincia, this.lbl_local, this.lbl_nazione, this.lbl_telefono, this.lbl_telefono2, this.lbl_fax, this.lbl_dtnasc, this.lbl_codfisc, this.lbl_partita_iva, this.lbl_codice_ipa, this.lbl_note };
                //CustomTextArea[] dettText = { this.txt_indirizzo, this.txt_cap, this.txt_citta, this.txt_provincia, this.txt_local, this.txt_nazione, this.txt_telefono, this.txt_telefono2, this.txt_fax, this.txt_dtnasc, this.txt_codfisc, this.txt_partita_iva, this.txt_codice_ipa, this.txt_note };

                // senza codice IPA
                Literal[] dettLabel = { this.lbl_indirizzo, this.lbl_cap, this.lbl_citta, this.lbl_provincia, this.lbl_local, this.lbl_nazione, this.lbl_telefono, this.lbl_telefono2, this.lbl_fax, this.lbl_dtnasc, this.lbl_codfisc, this.lbl_partita_iva,  this.lbl_note };
                CustomTextArea[] dettText = { this.txt_indirizzo, this.txt_cap, this.txt_citta, this.txt_provincia, this.txt_local, this.txt_nazione, this.txt_telefono, this.txt_telefono2, this.txt_fax, this.txt_dtnasc, this.txt_codfisc, this.txt_partita_iva,  this.txt_note };
                
                if (corr.GetType().Equals(typeof(DocsPaWR.Ruolo)))
                {
                    pnl_notcommon.Visible = false;
                    pnl_user.Visible = false;
                    txt_note.Visible = false;
                    lbl_note.Visible = false;
                }
                if (!corr.GetType().Equals(typeof(DocsPaWR.Utente)) && corr.tipoCorrispondente != "O")
                {
                    pnl_user.Visible = false;
                }
                else
                {
                    LoadTitles();
                }

                if (string.IsNullOrEmpty(corr.tipoCorrispondente) || (!(corr.tipoCorrispondente.Equals("F") && !corr.inRubricaComune) && !corr.tipoCorrispondente.Equals("L")))
                {
                    this.drawInfoCor(corr);
                    this.drawDettagliCorr(corr);
                }
                if (Request.QueryString["instanceDetails"] == null || !Request.QueryString["instanceDetails"].Equals("r"))
                {
                    if (this.DocumentInWorking.systemId == null)
                    {
                        /*
                           in attesa di future implementazioni
                           per la gestione di questa casisitica
                        */
                    }
                    else
                    {
                        this.drawDescrIntOp(corr);
                    }
                }
            }
        }

        private void drawInfoCor(DocsPaWR.Corrispondente myCorr)
        {
            string desc = "";
            if (myCorr.GetType() == typeof(DocsPaWR.Ruolo))
            {
                DocsPaWR.Ruolo ruo;
                if (corr.tipoCorrispondente != null && corr.tipoCorrispondente.Equals("O"))
                {
                    ruo = (DocsPaWR.Ruolo)myCorr;
                    desc = myCorr.descrizione;
                }
                else
                {
                    //	INIZIO MODIFICA PER ALLEGGERIRE LA RISOLUZIONE DEL DESTINATARIO
                    DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPaWR.AddressbookQueryCorrispondente();
                    qco.codiceRubrica = myCorr.codiceRubrica;
                    qco.idAmministrazione = myCorr.idAmministrazione;
                    //GLOBALE: perchè se vengo dalla ricerca non ho l'informazione 
                    //se il mitt/dest è Interno o Esterno
                    qco.tipoUtente = DocsPaWR.AddressbookTipoUtente.GLOBALE;
                    qco.fineValidita = true;

                    DocsPaWR.UnitaOrganizzativa corrUO;
                    string descrUO = "";
                    Corrispondente[] corrs = UserManager.getListaCorrispondenti(this, qco);
                    if (corrs != null && corrs.Count() > 0)
                        ruo = (DocsPaWR.Ruolo)corrs[0];
                    else
                        ruo = (DocsPaWR.Ruolo)myCorr;

                    if (ruo != null)
                    {
                        DocsPaWR.Ruolo corrRuolo = ruo;
                        corrUO = corrRuolo.uo;
                        while (corrUO != null)
                        {
                            descrUO = descrUO + " - " + corrUO.descrizione;
                            corrUO = corrUO.parent;
                        }

                        desc = corrRuolo.descrizione + descrUO;
                    }
                    else
                    {
                        desc = myCorr.descrizione;
                    }
                }

                this.lbl_nomeCorr.Text = desc;
                if (ruo.codiceRubrica != null && ruo.codiceRubrica != "")
                {
                    this.lbl_codRubr_corr.Text = "(" + ruo.codiceRubrica + ")";
                }
                // FINE MODIFICA

            }
            else
                if (myCorr.GetType() == typeof(DocsPaWR.Utente))
                {
                    DocsPaWR.Utente corrUtente = (DocsPaWR.Utente)myCorr;
                    DocsPaWR.Ruolo corrRuolo;
                    if (corrUtente.ruoli != null && corrUtente.ruoli.Length > 0)
                    {
                        corrRuolo = (DocsPaWR.Ruolo)corrUtente.ruoli[0];
                    }
                    lbl_nomeCorr.Text = corrUtente.descrizione;
                    if (corrUtente.codiceRubrica != null && corrUtente.codiceRubrica != "")
                    {
                        this.lbl_codRubr_corr.Text = "(" + corrUtente.codiceRubrica + ")";
                    }
                }
                else
                    if (myCorr.GetType() == typeof(DocsPaWR.UnitaOrganizzativa))
                    {
                        DocsPaWR.UnitaOrganizzativa corrUnitOrg = (DocsPaWR.UnitaOrganizzativa)myCorr;
                        string descrUO = "";

                        DocsPaWR.UnitaOrganizzativa corrUO;
                        corrUO = corrUnitOrg.parent;
                        if (corrUnitOrg.parent != null && corrUnitOrg.parent.systemId != null && corrUnitOrg.parent.systemId != "")
                            while (corrUO != null)
                            {
                                if (corrUO.descrizione == null || (corrUO.descrizione != null && corrUO.descrizione.Equals(String.Empty)))
                                {
                                    corrUO.descrizione = UserManager.getCorrispondentBySystemID(corrUO.systemId).descrizione;
                                }
                                descrUO = descrUO + "&nbsp;-&nbsp;" + corrUO.descrizione;
                                corrUO = corrUO.parent;
                            }

                        this.lbl_nomeCorr.Text = corrUnitOrg.descrizione + descrUO;
                        if (corrUnitOrg.codiceRubrica != null && corrUnitOrg.codiceRubrica != "")
                        {
                            this.lbl_codRubr_corr.Text = "(" + corrUnitOrg.codiceRubrica + ")";
                        }
                    }
                    else
                    {
                        this.lbl_nomeCorr.Text = myCorr.descrizione;
                        if (myCorr.codiceRubrica != null && !myCorr.codiceRubrica.Equals(""))
                            this.lbl_codRubr_corr.Text = "(" + myCorr.codiceRubrica + ")";
                    }

        }

        private void drawDettagliCorr(DocsPaWR.Corrispondente corr)
        {
            if (corr == null) return;

            if (corr.info == null)
            {
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
                    this.txt_luogoNasc.Text = corrDetails.BirthPlace;
                    this.txt_dtnasc.Text = corrDetails.BirthDay;
                    this.txt_codfisc.Text = corrDetails.TaxId;
                    this.txt_partita_iva.Text = corrDetails.CommercialId;
                    this.txt_note.Text = corrDetails.Note;
                    //this.txt_codice_ipa.Text = corrDetails.IpaCode;
                }
            }
            else if (corr.info.Tables["Corrispondente"].Rows.Count > 0)
            {
                this.ddl_titolo.SelectedIndex = this.ddl_titolo.Items.IndexOf(this.ddl_titolo.Items.FindByText(corr.info.Tables["Corrispondente"].Rows[0]["titolo"].ToString()));
                this.txt_indirizzo.Text = corr.info.Tables["Corrispondente"].Rows[0]["indirizzo"].ToString();
                this.txt_cap.Text = corr.info.Tables["Corrispondente"].Rows[0]["cap"].ToString();
                this.txt_citta.Text = corr.info.Tables["Corrispondente"].Rows[0]["citta"].ToString();
                this.txt_provincia.Text = corr.info.Tables["Corrispondente"].Rows[0]["provincia"].ToString();
                this.txt_local.Text = corr.info.Tables["Corrispondente"].Rows[0]["localita"].ToString();
                this.txt_nazione.Text = corr.info.Tables["Corrispondente"].Rows[0]["nazione"].ToString();
                this.txt_telefono.Text = corr.info.Tables["Corrispondente"].Rows[0]["telefono"].ToString();
                this.txt_telefono2.Text = corr.info.Tables["Corrispondente"].Rows[0]["telefono2"].ToString();
                this.txt_fax.Text = corr.info.Tables["Corrispondente"].Rows[0]["fax"].ToString();
                this.txt_codfisc.Text = corr.info.Tables["Corrispondente"].Rows[0]["codiceFiscale"].ToString();
                this.txt_partita_iva.Text = corr.info.Tables["Corrispondente"].Rows[0]["partitaIva"].ToString();
                //this.txt_codice_ipa.Text = corr.info.Tables["Corrispondente"].Rows[0]["codiceIpa"].ToString();
                this.txt_luogoNasc.Text = corr.info.Tables["Corrispondente"].Rows[0]["luogoNascita"].ToString();
                this.txt_dtnasc.Text = corr.info.Tables["Corrispondente"].Rows[0]["dataNascita"].ToString();
            }
            this.txt_nome.Text = corr.nome;
            this.txt_cognome.Text = corr.cognome;
            this.txt_email.Text = corr.email;
            this.txt_codAmm.Text = corr.codiceAmm;
            this.txt_codAOO.Text = corr.codiceAOO;

            if (this.corr.tipoCorrispondente == "U")
                this.ddl_tipoCorr.SelectedIndex = 0;
            else if (this.corr.tipoCorrispondente == "P")
                this.ddl_tipoCorr.SelectedIndex = 2;
            else if (this.corr.tipoCorrispondente == "F")
                this.ddl_tipoCorr.SelectedIndex = 3;
            else if (string.IsNullOrEmpty(this.corr.tipoCorrispondente) || this.corr.tipoCorrispondente == "O")
                this.ddl_tipoCorr.SelectedIndex = 4;
            else
                this.ddl_tipoCorr.SelectedIndex = 1;

            // canale preferenziale
            this.LoadPreferredChannels(true);
            if (this.corr.tipoIE == "E")
            {
                this.dd_canpref.Visible = true;
                if (this.corr.canalePref != null)
                {
                    this.dd_canpref.SelectedIndex = this.dd_canpref.Items.IndexOf(this.dd_canpref.Items.FindByValue(this.corr.canalePref.systemId));
                }

                // registro è popolato solo per i corrisp esterni
                this.pnl_registro.Visible = true;

                if (this.corr.idRegistro == null || (this.corr.idRegistro != null && this.corr.idRegistro.Trim() == ""))
                    this.lit_registro.Text = "TUTTI [RC]";
                else
                {
                    DocsPaWR.Registro regCorr = UserManager.getRegistroBySistemId(this, this.corr.idRegistro);
                    if (regCorr != null)
                    {
                        this.lit_registro.Text = regCorr.codRegistro;
                        if (regCorr.chaRF == "0")
                            this.lbl_registro.Text = this.GetLabel("CorrespondentDetailsRegistry");
                        else
                            this.lbl_registro.Text = this.GetLabel("CorrespondentDetailsRF");
                    }
                }
            }
            else
            {
                this.pnl_registro.Visible = false;
                this.pnl_ddCanPref.Visible = false;
            }
        }

        private void LoadPreferredChannels(bool showIs)
        {
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

        protected void drawDescrIntOp(DocsPaWR.Corrispondente corr)
        {
            string dataSpedizione = "";

            Table tbl = new Table();
            tbl.CssClass = "tbl";
            tbl.Attributes["style"] = "width: 600px; border-collapse:collapse;";

            Literal litHeader = new Literal();
            litHeader.Text = this.GetLabel("CorrespondentDetailsShipments");

            TableHeaderCell cell1 = new TableHeaderCell();
            cell1.ColumnSpan = 2;

            TableRow row1 = new TableRow();

            cell1.Controls.Add(litHeader);
            row1.Controls.Add(cell1);
            tbl.Controls.Add(row1);

            try
            {
                DocsPaWR.InfoDocumento infoDoc = DocumentManager.getInfoDocumento(this.DocumentInWorking);
                DocsPaWR.ProtocolloDestinatario[] dest = null;
                if (corr.systemId != null)
                    dest = DocumentManager.getDestinatariInteropAggConferma(infoDoc.idProfile, corr);
                if (dest != null)
                {
                    if (dest.Length > 0)
                    {

                        for (int i = 0; i < dest.Length; i++)
                        {
                            dataSpedizione = dest[i].dta_spedizione;
                            this.AddRow(ref tbl, this.GetLabel("CorrespondentDetailsShippingDate"), dataSpedizione);
                        }
                        if ((dataSpedizione == null) || (dataSpedizione != null && dataSpedizione.Equals(String.Empty)))
                        {
                            plc_DescIntOp.Visible = false;
                            return;
                        }
                        else
                        {
                            plc_DescIntOp.Visible = true;
                            this.plc_DescIntOp.Controls.Add(tbl);
                        }
                    }
                    else
                    {
                        plc_DescIntOp.Visible = false;
                        return;
                    }
                }

                else
                    return;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        private void AddRow(ref Table tbl, string label, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                Literal lit1 = new Literal();
                lit1.Text = label;

                TableCell cell1 = new TableCell();
                cell1.Width = new Unit(35, UnitType.Percentage);
                cell1.Controls.Add(lit1);


                Literal lit2 = new Literal();
                lit2.Text = value;

                TableCell cell2 = new TableCell();
                cell2.Width = new Unit(65, UnitType.Percentage);
                cell2.Controls.Add(lit2);


                TableRow row = new TableRow();
                row.Controls.Add(cell1);
                row.Controls.Add(cell2);
                tbl.Controls.Add(row);
            }
        }

        protected bool TypeMailCorrEsterno(string typeMail)
        {
            return (typeMail.Equals("1")) ? true : false;
        }

        private DataTable creaDataTable(ArrayList listaCorrispondenti)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("CODICE");
            dt.Columns.Add("DESCRIZIONE");

            for (int i = 0; i < listaCorrispondenti.Count; i++)
            {
                DocsPaWR.Corrispondente c = (DocsPaWR.Corrispondente)listaCorrispondenti[i];
                DataRow dr = dt.NewRow();
                dr[0] = c.codiceRubrica;
                dr[1] = c.descrizione;
                dt.Rows.Add(dr);
            }
            return dt;
        }

        private bool checkFields()
        {
            if ((this.txt_telefono == null || this.txt_telefono.Text.Equals("")) && !(this.txt_telefono2 == null || this.txt_telefono2.Text.Equals("")))
            {
                string msg = "InfoCorrespondentDetailsPhoneObligatory";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');}", true);
                return false;
            }
            else if (this.txt_cap != null && !this.txt_cap.Text.Equals("") && this.txt_cap.Text.Length != 5)
            {
                string msg = "InfoCorrespondentDetailsZipCodeObligatory";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');}", true);
                return false;
            }
            else if (this.txt_cap != null && !this.txt_cap.Text.Equals("") && !utils.isNumeric(this.txt_cap.Text))
            {
                string msg = "InfoCorrespondentDetailsZipCodeFormat";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');}", true);
                return false;
            }
            else if (this.txt_provincia != null && !this.txt_provincia.Text.Equals("") && this.txt_provincia.Text.Length != 2)
            {
                string msg = "InfoCorrespondentDetailsDistrictObligatory";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');}", true);
                return false;
            }
            else if (this.txt_provincia != null && !this.txt_provincia.Text.Equals("") && !utils.isCorrectProv(this.txt_provincia.Text))
            {
                string msg = "InfoCorrespondentDetailsDistrictFormat";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');}", true);
                return false;
            }

            //Controllo campo codice fiscale: se la lunghezza del campo è di 11 applico il controllo della partita iva supponendo che si tratti di UO, altrimenti quello
            // di codice fiscale 
            else if ((this.txt_codfisc != null && !this.txt_codfisc.Text.Trim().Equals("")) && ((this.txt_codfisc.Text.Trim().Length == 11 && utils.CheckVatNumber(this.txt_codfisc.Text.Trim()) != 0) || (this.txt_codfisc.Text.Trim().Length == 16 && utils.CheckTaxCode(this.txt_codfisc.Text.Trim()) != 0) || (this.txt_codfisc.Text.Trim().Length != 11 && this.txt_codfisc.Text.Trim().Length != 16)))
            {
                string msg = "InfoCorrespondentDetailsTaxIdFormat";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');}", true);
                return false;
            }

            else if (this.txt_partita_iva != null && !this.txt_partita_iva.Text.Trim().Equals("") && utils.CheckVatNumber(this.txt_partita_iva.Text.Trim()) != 0)
            {
                string msg = "InfoCorrespondentDetailsCommercialIdFormat";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');}", true);
                return false;
            }

            return true;
        }

        private void creaCorr()
        {
            try
            {
                corr.nome = string.Empty;
                corr.cognome = string.Empty;
                corr.codiceRubrica = null;
                corr.idOld = "0";
                corr.systemId = null;
                corr.tipoCorrispondente = "O";
                corr.tipoIE = null;
                corr.email = this.txt_email.Text;
                if (Request.QueryString["instanceDetails"] == null || !Request.QueryString["instanceDetails"].Equals("r"))
                {
                    if (this.DocumentInWorking.registro == null)
                        corr.idRegistro = UIManager.RegistryManager.GetRegistryInSession().systemId;
                    else
                        corr.idRegistro = this.DocumentInWorking.registro.systemId;
                }
                corr.codiceAmm = this.txt_codAmm.Text;
                corr.codiceAOO = this.txt_codAOO.Text;

                corr = AddressBookManager.ValorizeInfoCorr(corr
                    , this.txt_indirizzo.Text
                    , this.txt_citta.Text
                    , this.txt_cap.Text
                    , this.txt_provincia.Text
                    , this.txt_nazione.Text
                    , this.txt_telefono.Text
                    , this.txt_telefono2.Text
                    , this.txt_fax.Text
                    , this.txt_codfisc.Text
                    , this.txt_note.Text
                    , this.txt_local.Text
                    , this.ddl_titolo.SelectedValue
                    , this.txt_luogoNasc.Text
                    , this.txt_dtnasc.Text
                    , this.txt_partita_iva.Text,null
                    
                );
                // Codice IPA, inserire nel metodo precedente
                // , this.txt_codice_ipa.Text
                if (Request.QueryString["instanceDetails"] == null || !Request.QueryString["instanceDetails"].Equals("r"))
                    this.aggiornaSchedaDocumento(corr);
                else
                {
                    Richiedente = corr;
                }
                this.CloseMask(true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
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

        private void aggiornaSchedaDocumento(DocsPaWR.Corrispondente newCorr)
        {
            if ((this.tipoCor == null) || (this.tipoCor == ""))
                return;
            if (this.DocumentInWorking == null)
                return;

            if (this.tipoCor.Equals("M"))
            {
                this.Sender = newCorr;
            }
            else if (this.tipoCor.Equals("I"))
            {
                this.SenderIntermediate = newCorr;
            }
            else if (this.tipoCor.Equals("D"))
            {
                this.ListRecipients[Int32.Parse(this.indexCor)] = newCorr;
            }
            else if (this.tipoCor.Equals("C"))
            {
                this.ListRecipientsCC[Int32.Parse(this.indexCor)] = newCorr;
            }
        }

    }
}