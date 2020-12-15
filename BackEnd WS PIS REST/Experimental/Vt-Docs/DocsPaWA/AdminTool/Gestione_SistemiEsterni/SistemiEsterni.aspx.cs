using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using DocsPAWA.DocsPaWR;
using System.Collections;
using Amministrazione.Manager;


namespace DocsPAWA.AdminTool.Gestione_SistemiEsterni
{
    public partial class SistemiEsterni : System.Web.UI.Page
    {
        protected string idAmministrazione;
        protected DataSet dsRegistri;
        protected DataSet dsRF;
        protected DataSet dsPIS;
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["AdminBookmark"] = "SistemiEsterni";
            //----- CONTROLLO DELL'UTENTE AMMINISTRATORE CONNESSO -----------
            if (Session.IsNewSession)
            {
                Response.Redirect("../Exit.aspx?FROM=EXPIRED");
            }

            AmmUtils.WebServiceLink wws = new AmmUtils.WebServiceLink();
            InitializeComponent();
            if (!wws.CheckSession(Session.SessionID))
            {
                Response.Redirect("../Exit.aspx?FROM=ABORT");
            }
            // ---------------------------------------------------------------
            if (!Page.IsPostBack)
            {
                // prelievo di tutti i sistemi esterni
                //this.pnl_dett_sist_esterno.Visible = false;
                string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
                string codiceAmministrazione = amministrazione[0];
                string idAmministrazione = wws.AmmGetIDAmm(codiceAmministrazione);
                this.idAmministrazione = idAmministrazione;
                DocsPaWR.SistemaEsterno[] sysExts = Manager.SistemiEsterniManager.getSistemiEsterni(idAmministrazione);
                dg_SistemiEsterni.DataSource = this.ConvertToDataSet(sysExts);
                dg_SistemiEsterni.DataBind();
                //UpdatePanelSistemiEsterni.Update();
               
                this.pnl_dett_sist_esterno.Visible = false;
                this.pnl_metodi_pis.Visible = false;
                this.pnl_reg_sist_esterno.Visible = false;
                this.pnl_altre_conf.Visible = false;

                if (string.IsNullOrEmpty(DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "BE_AUTH_SYS_EXT_TRANSITION")) || DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "BE_AUTH_SYS_EXT_TRANSITION") == "0")
                {
                    this.msg_Transition.Visible = false;
                }
            }
        }

        private void RegisterClientScript(string scriptKey, string scriptValue)
        {
            if (!this.Page.IsStartupScriptRegistered(scriptKey))
            {
                string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), scriptKey, scriptString, false);
            }
        }

        private DataSet ConvertToDataSet(DocsPaWR.SistemaEsterno[] sysExt)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable("SYS_EXT");

            dt.Columns.Add(new DataColumn("ID"));
            dt.Columns.Add(new DataColumn("Codice"));
            dt.Columns.Add(new DataColumn("UserID_UtSys"));
            dt.Columns.Add(new DataColumn("Desc_Estesa"));
            dt.Columns.Add(new DataColumn("TknTime"));
            dt.Columns.Add(new DataColumn("Diritti"));
            dt.Columns.Add(new DataColumn("IdRole"));
            ds.Tables.Add(dt);

            dt = ds.Tables["SYS_EXT"];
            foreach (DocsPaWR.SistemaEsterno s1 in sysExt)
            {
                DataRow r = dt.NewRow();
                r["ID"] = s1.IdSistemaEsterno;
                r["Codice"] = s1.CodiceApplicazione;
                r["UserID_UtSys"] = s1.UserIdAssociato;
                r["Desc_Estesa"] = s1.DescEstesa;
                r["TknTime"] = s1.TokenPeriod;
                r["Diritti"] = s1.Diritti;
                r["IdRole"] = s1.idRuoloAssociato;
                dt.Rows.Add(r);
            }
            return ds;
        }

        private void dg_SistemiEsterni_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            string commandname = e.CommandName;

            if (commandname == "Select")
            {
                pnl_reg_sist_esterno.Visible = true;
                pnl_metodi_pis.Visible = true;
                pnl_dett_sist_esterno.Visible = true;
                pnl_altre_conf.Visible = true;
                DettagliRegistri(e.Item.Cells[6].Text);
                DettagliRF(e.Item.Cells[6].Text);
                DettagliPIS(e.Item.Cells[5].Text);
                this.txt_Sys_CodeApp.Text = e.Item.Cells[1].Text;
                this.txt_Sys_CodeApp.Enabled = false;
                this.hd_codeAPP.Value = e.Item.Cells[1].Text;
                this.txt_Sys_Ut_Sys.Text = e.Item.Cells[2].Text;
                this.txt_Sys_Ut_Sys.Enabled = false;
                this.hd_codeUser.Value = e.Item.Cells[2].Text;
                this.txt_Sys_extDesc.Text = e.Item.Cells[3].Text;
                this.txt_Sys_tknTime.Text = e.Item.Cells[4].Text;
                this.btn_mod_Sys.Text = "Modifica";
                this.tr_Sys_tknTime.Visible = true;
                this.hd_idSysExt.Value = e.Item.Cells[0].Text;
                this.hd_idSysRole.Value = e.Item.Cells[6].Text;
                this.lbl_err_sys_codeApp.Visible = false;
                this.lbl_err_sys_Ut_Sys.Visible = false;
                //this.btn_del_Sys.Visible = true;
                //updPanelRegistri.Update();
                //updPanelRF.Update();
            }
            //else if (commandname == "Delete")
            //{
            //    DocsPaWR.SistemaEsterno delse = new SistemaEsterno();
            //    delse.IdSistemaEsterno = e.Item.Cells[0].Text;
            //    delse.CodiceApplicazione = e.Item.Cells[1].Text;
            //    delse.UserIdAssociato = e.Item.Cells[2].Text;
            //    delse.DescEstesa = e.Item.Cells[3].Text;
            //    delse.TokenPeriod = Int32.Parse(e.Item.Cells[4].Text);
            //    delse.Diritti = e.Item.Cells[5].Text;
            //    delse.idRuoloAssociato = e.Item.Cells[6].Text;
            //    delse.idAmministrazione = ((string)Session["AMMDATASET"]).Split('@')[3];
            //    DocsPAWA.AdminTool.Manager.SessionManager session = new DocsPAWA.AdminTool.Manager.SessionManager();
            //    DocsPAWA.DocsPaWR.InfoUtenteAmministratore datiAmministratore = session.getUserAmmSession();
            //    Manager.SistemiEsterniManager.delExtSys(delse, datiAmministratore);
            //}
        }

        private void btn_nuovo_Click(object sender, System.EventArgs e)
        {
            this.txt_Sys_CodeApp.Text = string.Empty;
            this.txt_Sys_CodeApp.Enabled = true;
            this.txt_Sys_Ut_Sys.Text = string.Empty;
            this.txt_Sys_Ut_Sys.Enabled = true;
            this.txt_Sys_extDesc.Text = string.Empty;
            this.txt_Sys_tknTime.Text = string.Empty;
            this.tr_Sys_tknTime.Visible = false;
            this.pnl_dett_sist_esterno.Visible = true;
            this.pnl_reg_sist_esterno.Visible = false;
            this.pnl_metodi_pis.Visible = false;
            this.pnl_altre_conf.Visible = false;
            this.btn_mod_Sys.Text = "Inserisci";
            this.lbl_err_sys_codeApp.Visible = false;
            this.lbl_err_sys_Ut_Sys.Visible = false;
            //this.btn_del_Sys.Visible = false;
        }

        private void btn_mod_Sys_Click(object sender, System.EventArgs e)
        {
            //Amministrazione.Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
            //DocsPAWA.DocsPaWR.OrgRuolo ruolo = new DocsPAWA.DocsPaWR.OrgRuolo();
            if (btn_mod_Sys.Text == "Inserisci")
            {
                if (VerificaCampiInseriti())
                {
                    // Inserimento Utente.
                    string idAmm = ((string)Session["AMMDATASET"]).Split('@')[3];

                    string ctrlSysExt = Manager.SistemiEsterniManager.ctrlInserimentoSistemaEsterno(idAmm, this.txt_Sys_Ut_Sys.Text, this.txt_Sys_CodeApp.Text);
                    bool ctrlInsUO = true;
                    if (ctrlSysExt == "OK")
                    {
                        Amministrazione.Manager.OrganigrammaManager theManager2 = new Amministrazione.Manager.OrganigrammaManager();

                        // Ricerca dell'id della UO dei sistemi esterni
                        string idUO = "";
                        UnitaOrganizzativa hubSysExt = Manager.SistemiEsterniManager.getHubSistemiEsterni("HUB_SYST_EXT", idAmm);
                        if (hubSysExt != null)
                        {
                            idUO = hubSysExt.systemId;
                        }
                        else
                        {
                            // Se non presente, devo creare l'UO.
                            theManager2.ListaUOLivelloZero(idAmm);
                            DocsPAWA.DocsPaWR.OrgUO newUO = new DocsPAWA.DocsPaWR.OrgUO();

                            newUO.IDParent = ((DocsPAWA.DocsPaWR.OrgUO)theManager2.getListaUO()[0]).IDCorrGlobale;
                            newUO.Codice = "HUB_SYST_EXT";
                            newUO.CodiceRubrica = "HUB_SYST_EXT";
                            newUO.Descrizione = "Sistema Esterno";
                            newUO.Livello = (Int32.Parse(((DocsPAWA.DocsPaWR.OrgUO)theManager2.getListaUO()[0]).Livello) + 1).ToString();
                            newUO.IDAmministrazione = idAmm;
                            DocsPAWA.DocsPaWR.OrgDettagliGlobali dettagli = new DocsPAWA.DocsPaWR.OrgDettagliGlobali();
                            newUO.DettagliUo = dettagli;
                            newUO.DettagliUo.Indirizzo = string.Empty;
                            newUO.DettagliUo.Citta = string.Empty;
                            newUO.DettagliUo.Cap = string.Empty;
                            newUO.DettagliUo.Provincia = string.Empty;
                            newUO.DettagliUo.Nazione = string.Empty;
                            newUO.DettagliUo.Telefono1 = string.Empty;
                            newUO.DettagliUo.Telefono2 = string.Empty;
                            newUO.DettagliUo.Fax = string.Empty;
                            newUO.DettagliUo.Note = string.Empty;
                            newUO.DettagliUo.CodiceFiscale = string.Empty;
                            newUO.DettagliUo.PartitaIva = string.Empty;
                            theManager2.InsNuovaUO(newUO);
                            DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
                            esito = theManager2.getEsitoOperazione();

                            if (esito.Codice.Equals(0))
                            {
                                hubSysExt = Manager.SistemiEsterniManager.getHubSistemiEsterni("HUB_SYST_EXT", idAmm);
                                idUO = hubSysExt.systemId;
                                Manager.SistemiEsterniManager.setVisibilityHubSysExt(idUO);
                            }
                            else
                            {
                                ctrlInsUO = false;
                                string scriptString = "<SCRIPT>alert('Attenzione, errore nella creazione dell''hub per i sistemi esterni.');</SCRIPT>";
                                this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                            }
                        }
                        if (ctrlInsUO)
                        {
                            Amministrazione.Manager.UtentiManager theManager = new Amministrazione.Manager.UtentiManager();
                            DocsPAWA.DocsPaWR.OrgUtente utente = new DocsPAWA.DocsPaWR.OrgUtente();
                            utente.Codice = this.txt_Sys_Ut_Sys.Text.ToUpper();
                            utente.Nome = this.txt_Sys_CodeApp.Text;
                            utente.Cognome = "Sistema esterno";
                            utente.UserId = this.txt_Sys_Ut_Sys.Text;
                            utente.CodiceRubrica = this.txt_Sys_Ut_Sys.Text.ToUpper();
                            utente.IDCorrGlobale = null;
                            utente.IDPeople = null;
                            utente.Password = "pass_SE_Alca7Ra2_" + this.txt_Sys_CodeApp.Text;
                            utente.Amministratore = "0";
                            utente.Email = "";
                            utente.FromEmail = "";
                            utente.Abilitato = "1";
                            utente.AbilitatoCentroServizi = false;
                            utente.AbilitatoEsibizione = false;
                            utente.Dominio = "";
                            utente.Sede = "";
                            utente.NotificaTrasm = null;
                            utente.IDAmministrazione = idAmm;
                            utente.NessunaScadenzaPassword = true;
                            utente.SincronizzaLdap = false;
                            utente.AutenticatoInLdap = false;
                            utente.IdSincronizzazioneLdap = "";
                            utente.IdClientSideModelProcessor = 0;
                            utente.SmartClientConfigurations = new DocsPAWA.DocsPaWR.SmartClientConfigurations();
                            utente.SmartClientConfigurations.ComponentsType = "3";
                            utente.SmartClientConfigurations.ApplyPdfConvertionOnScan = false;
                            utente.DispositivoStampa = null;
                            theManager.InsUtente(utente, idAmm);
                            // Fine inserimento utente

                            // Inserimento ruolo
                            DocsPAWA.DocsPaWR.OrgRuolo ruolo = new DocsPAWA.DocsPaWR.OrgRuolo();
                            // Ho bisogno dell'IDCorrGlob dell'id dell'uo di sistema.



                            // Per i test lo cablo. Dopo vedo come prelevarlo.

                            ruolo.IDUo = idUO;
                            ruolo.Codice = this.txt_Sys_CodeApp.Text.ToUpper();
                            ruolo.CodiceRubrica = this.txt_Sys_CodeApp.Text.ToUpper();
                            ruolo.Descrizione = this.txt_Sys_extDesc.Text.Replace("'", "''");
                            ruolo.DiRiferimento = "0";
                            ruolo.Responsabile = "0";
                            ruolo.Segretario = "0";
                            ruolo.DisabledTrasm = "1";
                            ruolo.IDAmministrazione = idAmm;
                            ruolo.RuoloDiSistema = "1";
                            TipoRuolo tipoRuolo = Manager.SistemiEsterniManager.getTipoRuoloByCode(idAmm, "R_Sist");
                            if (tipoRuolo == null)
                            {
                                DocsPAWA.AdminTool.Manager.SessionManager session = new DocsPAWA.AdminTool.Manager.SessionManager();

                                DocsPAWA.DocsPaWR.InfoUtenteAmministratore datiAmministratore = session.getUserAmmSession();

                                DocsPAWA.DocsPaWR.OrgTipoRuolo tipoRuolo2 = new OrgTipoRuolo();
                                tipoRuolo2.Codice = "R_Sist";
                                tipoRuolo2.Descrizione = "Integratore";
                                tipoRuolo2.Livello = "200";
                                tipoRuolo2.IDAmministrazione = idAmm;

                                DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                                ws.AmmInsertTipoRuolo(datiAmministratore, ref tipoRuolo2, idAmm);
                                tipoRuolo = Manager.SistemiEsterniManager.getTipoRuoloByCode(idAmm, "R_Sist");
                                if (tipoRuolo != null)
                                    ruolo.IDTipoRuolo = tipoRuolo.systemId;

                            }
                            else
                            {
                                ruolo.IDTipoRuolo = tipoRuolo.systemId;
                            }
                            theManager2.InsNuovoRuolo(ruolo, false);
                            // fine inserimento ruolo

                            // Questo metodo inserisce l'utente nel ruolo e la riga nella DPA_EXTERNAL_SYSTEMS
                            bool inserito = Manager.SistemiEsterniManager.insSysExtAfterAssoc(idAmm, this.txt_Sys_Ut_Sys.Text, this.txt_Sys_CodeApp.Text, this.txt_Sys_extDesc.Text);


                            Response.Redirect(Request.RawUrl);
                        }
                    }
                    else
                    {
                        if (ctrlSysExt.Contains("USERID_NOT_VALID"))
                        {
                            lbl_err_sys_Ut_Sys.Visible = true;
                        }
                        if (ctrlSysExt.Contains("CODEROLE_NOT_VALID"))
                        {
                            lbl_err_sys_codeApp.Visible = true;
                        }
                    }

                }

            }
            else if (btn_mod_Sys.Text == "Modifica")
            {
                if (VerificaCampiInseriti())
                {
                    string idSysExt = this.hd_idSysExt.Value;
                    int tknTime = 20;
                    if (!Int32.TryParse(this.txt_Sys_tknTime.Text, out tknTime))
                    {
                        //string scriptString = "<SCRIPT>alert('Attenzione, valore inserito per periodo token non valido. Inserire un intero.');</SCRIPT>";
                        //this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                        RegisterClientScript("alert1", "alert('Attenzione, valore inserito per periodo token non valido. Inserire un intero.');");

                    }
                    else
                    {
                        if (tknTime >= 0 || tknTime < 1439)
                        {
                            string descrizione = this.txt_Sys_extDesc.Text.Replace("'", "''");

                            Manager.SistemiEsterniManager.modDescTknPIS(descrizione, tknTime.ToString(), idSysExt);
                            Response.Redirect(Request.RawUrl);
                        }
                        else
                        {
                            //string scriptString = "<SCRIPT>alert('Attenzione, valore inserito per periodo token non valido');</SCRIPT>";
                            //this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                            RegisterClientScript("alert1", "alert('Attenzione, valore inserito per periodo token non valido');");
                        }
                    }
                }
            }

        }

        private void btn_del_Sys_Click(object sender, System.EventArgs e)
        {
            RegisterClientScript("alert1", "alert('Andato.');");
            if (false)
            {
                DocsPaWR.SistemaEsterno delse = new SistemaEsterno();
                delse.IdSistemaEsterno = this.hd_idSysExt.Value;
                delse.CodiceApplicazione = this.hd_codeAPP.Value;
                delse.UserIdAssociato = this.hd_codeUser.Value;
                delse.DescEstesa = "";
                delse.TokenPeriod = 0;
                delse.Diritti = "";
                delse.idRuoloAssociato = this.hd_idSysRole.Value;
                delse.idAmministrazione = ((string)Session["AMMDATASET"]).Split('@')[3];
                DocsPAWA.AdminTool.Manager.SessionManager session = new DocsPAWA.AdminTool.Manager.SessionManager();
                DocsPAWA.DocsPaWR.InfoUtenteAmministratore datiAmministratore = session.getUserAmmSession();
                Manager.SistemiEsterniManager.delExtSys(delse, datiAmministratore);
            }
        }

        private bool VerificaCampiInseriti()
        {
            if (this.txt_Sys_CodeApp.Text == null || this.txt_Sys_CodeApp.Text.Trim() == "" || !AmmUtils.UtilsXml.IsAlphaNumeric(this.txt_Sys_CodeApp.Text))
            {
                RegisterClientScript("alertCodeApp", "alert('Attenzione, valore inserito per il codice applicazione non valido. Ammessi numeri, lettere, underscore e punto.');");
                return false;   
            }
            if (this.txt_Sys_Ut_Sys.Text == null || this.txt_Sys_Ut_Sys.Text.Trim() == "" || !AmmUtils.UtilsXml.IsAlphaNumeric(this.txt_Sys_Ut_Sys.Text))
            {
                RegisterClientScript("alertCodeApp", "alert('Attenzione, valore inserito per il nome utente non valido. Ammessi numeri, lettere, underscore e punto.');");
                return false;
            }
            return true;
        }

        private void dg_SistemiEsterni_DeleteCommand(object sender, DataGridCommandEventArgs e)
        {
            
                DocsPaWR.SistemaEsterno delse = new SistemaEsterno();
                delse.IdSistemaEsterno = e.Item.Cells[0].Text;
                delse.CodiceApplicazione = e.Item.Cells[1].Text;
                delse.UserIdAssociato = e.Item.Cells[2].Text;
                delse.DescEstesa = "";
                delse.TokenPeriod = 0;
                delse.Diritti = "";
                delse.idRuoloAssociato = e.Item.Cells[6].Text;
                delse.idAmministrazione = ((string)Session["AMMDATASET"]).Split('@')[3];
                DocsPAWA.AdminTool.Manager.SessionManager session = new DocsPAWA.AdminTool.Manager.SessionManager();
                DocsPAWA.DocsPaWR.InfoUtenteAmministratore datiAmministratore = session.getUserAmmSession();
                Manager.SistemiEsterniManager.delExtSys(delse, datiAmministratore);
                Response.Redirect(Request.RawUrl);
            
        }

        private void btn_mod_PIS_Click(object sender, System.EventArgs e)
        {
            CheckBox spunta;
            string metodiabilitati = "";
            string idSysExt = this.hd_idSysExt.Value;
            foreach (DataGridItem item in dg_pis.Items)
            {
                spunta = (CheckBox)item.Cells[4].FindControl("cbx_Sel_pis");
                if (spunta.Checked)
                {
                    metodiabilitati += item.Cells[1].Text.ToUpper() + ";";
                }
            }

            Manager.SistemiEsterniManager mgr = new Manager.SistemiEsterniManager();

            bool retval = Manager.SistemiEsterniManager.modPIS(metodiabilitati, idSysExt);
            Response.Redirect(Request.RawUrl);
        }

        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            dg_SistemiEsterni.ItemCreated += new DataGridItemEventHandler(dg_SistemiEsterni_ItemCreated);
            dg_SistemiEsterni.ItemCommand += new DataGridCommandEventHandler(dg_SistemiEsterni_ItemCommand);
            dg_SistemiEsterni.DeleteCommand += new DataGridCommandEventHandler(dg_SistemiEsterni_DeleteCommand);
            btn_nuovo.Click += new EventHandler(btn_nuovo_Click);
            btn_mod_pis.Click += new EventHandler(btn_mod_PIS_Click);
            btn_mod_registri.Click += new EventHandler(btn_mod_registri_Click);
            btn_mod_Sys.Click += new EventHandler(btn_mod_Sys_Click);
            //btn_del_Sys.Click += new EventHandler(btn_del_Sys_Click);
        }

        void btn_mod_registri_Click(object sender, EventArgs e)
        {
            InserimentoRegistri();
        }

        private void dg_SistemiEsterni_ItemCreated(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            e.Item.Cells[8].Attributes.Add("onclick", "if (!window.confirm('Sei sicuro di voler eliminare questo sistema esterno?')) {return false};");

        }

        private void InitializeDataSetRegistri()
        {
            dsRegistri = new DataSet();
            DataColumn[] keys = new DataColumn[2];
            DataColumn dc;
            dsRegistri.Tables.Add("REGISTRI");
            dc = new DataColumn("IDRegistro", System.Type.GetType("System.String"));
            dsRegistri.Tables["REGISTRI"].Columns.Add(dc);
            keys[0] = dc;
            dc = new DataColumn("Codice");
            dsRegistri.Tables["REGISTRI"].Columns.Add(dc);
            dc = new DataColumn("Descrizione");
            dsRegistri.Tables["REGISTRI"].Columns.Add(dc);
            dc = new DataColumn("EmailRegistro", System.Type.GetType("System.String"));
            dsRegistri.Tables["REGISTRI"].Columns.Add(dc);
            keys[1] = dc;
            dc = new DataColumn("IDAmministrazione");
            dsRegistri.Tables["REGISTRI"].Columns.Add(dc);
            dc = new DataColumn("Sel");
            dsRegistri.Tables["REGISTRI"].Columns.Add(dc);
            dc = new DataColumn("consulta");
            dsRegistri.Tables["REGISTRI"].Columns.Add(dc);
            dc = new DataColumn("notifica");
            dsRegistri.Tables["REGISTRI"].Columns.Add(dc);
            dc = new DataColumn("spedisci");
            dsRegistri.Tables["REGISTRI"].Columns.Add(dc);
            dc = new DataColumn("Sospeso");
            dsRegistri.Tables["REGISTRI"].Columns.Add(dc);
            dsRegistri.Tables["REGISTRI"].PrimaryKey = keys;
        }

        private void InitializeDataSetRF()
        {
            dsRF = new DataSet();
            DataColumn[] keys = new DataColumn[2];
            DataColumn dc;
            dsRF.Tables.Add("RF");

            dc = new DataColumn("IDRegistro", System.Type.GetType("System.String"));
            dsRF.Tables["RF"].Columns.Add(dc);
            keys[0] = dc;

            dc = new DataColumn("Codice");
            dsRF.Tables["RF"].Columns.Add(dc);

            dc = new DataColumn("Descrizione");
            dsRF.Tables["RF"].Columns.Add(dc);

            dc = new DataColumn("IDAmministrazione");
            dsRF.Tables["RF"].Columns.Add(dc);

            dc = new DataColumn("Sel");
            dsRF.Tables["RF"].Columns.Add(dc);

            dc = new DataColumn("Disabled");
            dsRF.Tables["RF"].Columns.Add(dc);

            dc = new DataColumn("AooCollegata");
            dsRF.Tables["RF"].Columns.Add(dc);

            dc = new DataColumn("EmailRegistro", System.Type.GetType("System.String"));

            dsRF.Tables["RF"].Columns.Add(dc);
            keys[1] = dc;

            dc = new DataColumn("consulta");
            dsRF.Tables["RF"].Columns.Add(dc);

            dc = new DataColumn("notifica");
            dsRF.Tables["RF"].Columns.Add(dc);

            dc = new DataColumn("spedisci");
            dsRF.Tables["RF"].Columns.Add(dc);

            dsRF.Tables["RF"].PrimaryKey = keys;
        }

        private void InitializeDSPIS()
        {
            dsPIS = new DataSet();
            DataColumn[] keys = new DataColumn[2];
            DataColumn dc;
            dsPIS.Tables.Add("PIS");

            dc = new DataColumn("IDPIS", System.Type.GetType("System.String"));
            dsPIS.Tables["PIS"].Columns.Add(dc);
            keys[0] = dc;

            dc = new DataColumn("nomePIS");
            dsPIS.Tables["PIS"].Columns.Add(dc);

            dc = new DataColumn("Descrizione");
            dsPIS.Tables["PIS"].Columns.Add(dc);
            dc = new DataColumn("FILE_SVC");
            dsPIS.Tables["PIS"].Columns.Add(dc);

            dc = new DataColumn("Sel");
            dsPIS.Tables["PIS"].Columns.Add(dc);
        }

        private void DettagliRegistri(string idCorr)
        {
            DocsPaWebService ws = new DocsPaWebService();
            Amministrazione.Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
            //AmmUtils.WebServiceLink wws = new AmmUtils.WebServiceLink();
            string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            //string codiceAmministrazione = amministrazione[0];
            //string idAmministrazione = wws.AmmGetIDAmm(codiceAmministrazione);                
            theManager.ListaRegistriRF(amministrazione[3], idCorr, "0");
            if (theManager.getListaRegistri().Count > 0)
            {
                InitializeDataSetRegistri();
                DataRow row;
                foreach (DocsPAWA.DocsPaWR.OrgRegistro registro in theManager.getListaRegistri())
                {
                    row = dsRegistri.Tables[0].NewRow();
                    row["IDRegistro"] = registro.IDRegistro;
                    row["Codice"] = registro.Codice;
                    row["Descrizione"] = registro.Descrizione;
                    row["IDAmministrazione"] = registro.IDAmministrazione;
                    if (registro.Associato != null && registro.Associato != String.Empty)
                    {
                        row["Sel"] = "true";
                    }
                    else
                    {
                        row["Sel"] = "false";
                    }
                    row["Sospeso"] = registro.Sospeso;
                    string emails = "";
                    //caso ruolo associato ad un Registro
                    if (row["Sel"].ToString().Equals("true"))
                    {
                        DataSet ds = DocsPAWA.utils.MultiCasellaManager.GetRightMailRegistro(registro.IDRegistro, idCorr);
                        //se è un Registro con una o più caselle imposto i diritti del ruolo sulle singole caselle
                        if (ds.Tables.Count > 0 && ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows.Count > 0)
                        {
                            if (ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows.Count > 0)
                            {
                                //string casellaPrincipale = DocsPAWA.utils.MultiCasellaManager.GetMailPrincipaleRegistro(registro.IDRegistro);
                                foreach (DataRow r in ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
                                {
                                    if (row.RowState.ToString().ToLower().Equals("added"))
                                    {
                                        row = dsRegistri.Tables[0].NewRow();
                                        row["IDRegistro"] = registro.IDRegistro;
                                        row["Codice"] = registro.Codice;
                                        row["Descrizione"] = registro.Descrizione;
                                        row["IDAmministrazione"] = registro.IDAmministrazione;
                                        if (registro.Associato != null && registro.Associato != String.Empty)
                                        {
                                            row["Sel"] = "true";
                                        }
                                        else
                                        {
                                            row["Sel"] = "false";
                                        }
                                        row["Sospeso"] = registro.Sospeso;
                                    }
                                    //if (casellaPrincipale != null && casellaPrincipale.Equals(r["EMAIL_REGISTRO"].ToString()))
                                    //{
                                    //    row["EmailRegistro"] = "* " + r["EMAIL_REGISTRO"].ToString();
                                    //}
                                    //else
                                    //{
                                    emails += (r["EMAIL_REGISTRO"].ToString() + ";");
                                    row["EmailRegistro"] = emails;

                                    //}
                                    //row["consulta"] = (r["CONSULTA"].ToString().Equals("1") ? "true" : "false");
                                    //row["spedisci"] = (r["SPEDISCI"].ToString().Equals("1") ? "true" : "false");
                                    //row["notifica"] = (r["NOTIFICA"].ToString().Equals("1") ? "true" : "false");
                                    row["consulta"] = "false";
                                    row["spedisci"] = "false";
                                    row["notifica"] = "false";
                                    //dsRegistri.Tables["REGISTRI"].Rows.Add(row);
                                }
                            }
                        }
                        //se Registro senza caselle di posta
                        else
                        {
                            row["EmailRegistro"] = string.Empty;
                            row["consulta"] = "false";
                            row["spedisci"] = "false";
                            row["notifica"] = "false";
                            //dsRegistri.Tables["REGISTRI"].Rows.Add(row);
                        }
                    }
                    //se il ruolo non è associato al Registro
                    else
                    {
                        CasellaRegistro[] caselle = DocsPAWA.utils.MultiCasellaManager.GetMailRegistro(registro.IDRegistro);
                        //se registro con una o più caselle imposto i diritti del ruolo sulle singole caselle
                        if (caselle != null && caselle.Length > 0)
                        {
                            //string casellaPrincipale = DocsPAWA.utils.MultiCasellaManager.GetMailPrincipaleRegistro(registro.IDRegistro);
                            foreach (CasellaRegistro c in caselle)
                            {
                                if (row.RowState.ToString().ToLower().Equals("added"))
                                {
                                    row = dsRegistri.Tables[0].NewRow();
                                    row["IDRegistro"] = registro.IDRegistro;
                                    row["Codice"] = registro.Codice;
                                    row["Descrizione"] = registro.Descrizione;
                                    row["IDAmministrazione"] = registro.IDAmministrazione;
                                    if (registro.Associato != null && registro.Associato != String.Empty)
                                    {
                                        row["Sel"] = "true";
                                    }
                                    else
                                    {
                                        row["Sel"] = "false";
                                    }
                                    row["Sospeso"] = registro.Sospeso;
                                }
                                //if (casellaPrincipale != null && casellaPrincipale.Equals(c.EmailRegistro))
                                //{
                                //    row["EmailRegistro"] = "* " + c.EmailRegistro;
                                //}
                                //else
                                //{
                                emails += (c.EmailRegistro + ";");
                                row["EmailRegistro"] = emails;
                                //}
                                row["consulta"] = "false";
                                row["spedisci"] = "false";
                                row["notifica"] = "false";
                                //dsRegistri.Tables["REGISTRI"].Rows.Add(row);
                            }
                            if (ws.IsEnabledInteropInterna())
                            {
                                if (row.RowState.ToString().ToLower().Equals("added"))
                                {
                                    row = dsRegistri.Tables[0].NewRow();
                                    row["IDRegistro"] = registro.IDRegistro;
                                    row["Codice"] = registro.Codice;
                                    row["Descrizione"] = registro.Descrizione;
                                    row["IDAmministrazione"] = registro.IDAmministrazione;
                                    if (registro.Associato != null && registro.Associato != String.Empty)
                                    {
                                        row["Sel"] = "true";
                                    }
                                    else
                                    {
                                        row["Sel"] = "false";
                                    }
                                    row["Sospeso"] = registro.Sospeso;
                                }
                                row["EmailRegistro"] = string.Empty;
                                row["consulta"] = "false";
                                row["spedisci"] = "false";
                                row["notifica"] = "false";
                                //dsRegistri.Tables["REGISTRI"].Rows.Add(row);
                            }
                        }

                        //se registro senza caselle di posta 
                        else
                        {
                            row["EmailRegistro"] = string.Empty;
                            row["consulta"] = "false";
                            row["spedisci"] = "false";
                            row["notifica"] = "false";
                            //dsRegistri.Tables["REGISTRI"].Rows.Add(row);
                        }
                    }
                    dsRegistri.Tables["REGISTRI"].Rows.Add(row);
                }

                DataView dv = dsRegistri.Tables["REGISTRI"].DefaultView;
                dv = OrdinaGrid(dv, "Descrizione");
                this.dg_registri.DataSource = dv;
                this.dg_registri.DataBind();
            }
        }

        private void DettagliRF(string idCorr)
        {

            DocsPaWebService ws = new DocsPaWebService();
            Amministrazione.Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
            //AmmUtils.WebServiceLink wws = new AmmUtils.WebServiceLink();
            string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            //string codiceAmministrazione = amministrazione[0];
            //string idAmministrazione = wws.AmmGetIDAmm(codiceAmministrazione);                
            theManager.ListaRegistriRF(amministrazione[3], idCorr, "1");
            DataRow row;
            if (theManager.getListaRegistri() != null && theManager.getListaRegistri().Count > 0)
            {
                this.pnlRF.Visible = true;
                InitializeDataSetRF();
                foreach (DocsPAWA.DocsPaWR.OrgRegistro registro in theManager.getListaRegistri())
                {
                    row = dsRF.Tables[0].NewRow();
                    row["IDRegistro"] = registro.IDRegistro;

                    row["Codice"] = registro.Codice;

                    row["Descrizione"] = registro.Descrizione;
                    row["IDAmministrazione"] = registro.IDAmministrazione;
                    if (registro.Associato != null && registro.Associato != String.Empty)
                    {
                        row["Sel"] = "true";
                    }
                    else
                    {
                        row["Sel"] = "false";
                    }

                    row["Disabled"] = registro.rfDisabled;
                    row["AooCollegata"] = registro.idAOOCollegata;
                    string emails = "";
                    //caso ruolo associato ad un RF
                    if (row["Sel"].ToString().Equals("true"))
                    {
                        DataSet ds = DocsPAWA.utils.MultiCasellaManager.GetRightMailRegistro(registro.IDRegistro, idCorr);
                        //se RF con una o più caselle imposto i diritti del ruolo sulle singole caselle
                        if (ds.Tables.Count > 0 && ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows.Count > 0)
                        {
                            if (ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows.Count > 0)
                            {
                                string casellaPrincipale = DocsPAWA.utils.MultiCasellaManager.GetMailPrincipaleRegistro(registro.IDRegistro);
                                foreach (DataRow r in ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
                                {
                                    if (row.RowState.ToString().ToLower().Equals("added"))
                                    {
                                        row = dsRF.Tables[0].NewRow();
                                        row["IDRegistro"] = registro.IDRegistro;
                                        row["Codice"] = registro.Codice;
                                        row["Descrizione"] = registro.Descrizione;
                                        row["IDAmministrazione"] = registro.IDAmministrazione;
                                        if (registro.Associato != null && registro.Associato != String.Empty)
                                        {
                                            row["Sel"] = "true";
                                        }
                                        else
                                        {
                                            row["Sel"] = "false";
                                        }
                                        row["Disabled"] = registro.rfDisabled;
                                        row["AooCollegata"] = registro.idAOOCollegata;
                                    }
                                    //if (casellaPrincipale != null && casellaPrincipale.Equals(r["EMAIL_REGISTRO"].ToString()))
                                    //{
                                    //    row["EmailRegistro"] = "* " + r["EMAIL_REGISTRO"].ToString();
                                    //}
                                    //else
                                    //{
                                    emails += (r["EMAIL_REGISTRO"].ToString() + ";");
                                    row["EmailRegistro"] = emails;
                                    //}
                                    //row["consulta"] = (r["CONSULTA"].ToString().Equals("1") ? "true" : "false");
                                    //row["spedisci"] = (r["SPEDISCI"].ToString().Equals("1") ? "true" : "false");
                                    //row["notifica"] = (r["NOTIFICA"].ToString().Equals("1") ? "true" : "false");
                                    row["consulta"] = "false";
                                    row["spedisci"] = "false";
                                    row["notifica"] = "false";
                                    //dsRF.Tables["RF"].Rows.Add(row);
                                }
                            }
                        }

                        //se rf senza caselle di posta
                        else
                        {
                            row["EmailRegistro"] = string.Empty;
                            row["consulta"] = "false";
                            row["spedisci"] = "false";
                            row["notifica"] = "false";
                            //dsRF.Tables["RF"].Rows.Add(row);
                        }
                    }
                    //se il ruolo non è associato all'RF
                    else
                    {
                        CasellaRegistro[] caselle = DocsPAWA.utils.MultiCasellaManager.GetMailRegistro(registro.IDRegistro);
                        //se rf con una o più caselle imposto i diritti del ruolo sulle singole caselle
                        if (caselle != null && caselle.Length > 0)
                        {
                            //string casellaPrincipale = DocsPAWA.utils.MultiCasellaManager.GetMailPrincipaleRegistro(registro.IDRegistro);
                            foreach (CasellaRegistro c in caselle)
                            {
                                if (row.RowState.ToString().ToLower().Equals("added"))
                                {
                                    row = dsRF.Tables[0].NewRow();
                                    row["IDRegistro"] = registro.IDRegistro;
                                    row["Codice"] = registro.Codice;
                                    row["Descrizione"] = registro.Descrizione;
                                    row["IDAmministrazione"] = registro.IDAmministrazione;
                                    row["Sel"] = "false";
                                    row["Disabled"] = registro.rfDisabled;
                                    row["AooCollegata"] = registro.idAOOCollegata;
                                }
                                //if (casellaPrincipale != null && casellaPrincipale.Equals(c.EmailRegistro))
                                //{
                                //    row["EmailRegistro"] = "* " + c.EmailRegistro;
                                //}
                                //else
                                //{
                                emails += (c.EmailRegistro + ";");
                                row["EmailRegistro"] = emails;
                                //}
                                row["consulta"] = "false";
                                row["spedisci"] = "false";
                                row["notifica"] = "false";
                                //dsRF.Tables["RF"].Rows.Add(row);
                            }
                            if (ws.IsEnabledInteropInterna())
                            {
                                if (row.RowState.ToString().ToLower().Equals("added"))
                                {
                                    row = dsRF.Tables[0].NewRow();
                                    row["IDRegistro"] = registro.IDRegistro;
                                    row["Codice"] = registro.Codice;
                                    row["Descrizione"] = registro.Descrizione;
                                    row["IDAmministrazione"] = registro.IDAmministrazione;
                                    row["Sel"] = "false";
                                    row["Disabled"] = registro.rfDisabled;
                                    row["AooCollegata"] = registro.idAOOCollegata;
                                }
                                row["EmailRegistro"] = string.Empty;
                                row["consulta"] = "false";
                                row["spedisci"] = "false";
                                row["notifica"] = "false";
                                //dsRF.Tables["RF"].Rows.Add(row);
                            }
                        }

                        //se rf senza caselle di posta 
                        else
                        {
                            row["EmailRegistro"] = string.Empty;
                            row["consulta"] = "false";
                            row["spedisci"] = "false";
                            row["notifica"] = "false";
                            //dsRF.Tables["RF"].Rows.Add(row);
                        }
                    }
                    dsRF.Tables["RF"].Rows.Add(row);
                }

                DataView dv = dsRF.Tables["RF"].DefaultView;
                dv = OrdinaGrid(dv, "Descrizione");
                this.dg_RF.DataSource = dv;
                this.dg_RF.DataBind();
            }

        }

        private void AlertJS(string msg)
        {
            if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
            {
                string scriptString = "<SCRIPT>alert('" + msg.Replace("'", "\\'") + "');</SCRIPT>";
                this.Page.RegisterStartupScript("alertJavaScript", scriptString);
            }
        }

        private void DettagliPIS(string diritti)
        {
            DocsPaWR.MetodoPIS[] meths = Manager.SistemiEsterniManager.getPISMethods();
            InitializeDSPIS();
            DataRow row;
            foreach (DocsPaWR.MetodoPIS pis in meths)
            {
                row = dsPIS.Tables[0].NewRow();
                row["IDPIS"] = pis.IdMetodo;
                row["nomePIS"] = pis.MethodName;
                row["Descrizione"] = pis.Description;
                row["FILE_SVC"] = pis.FileSVC;
                if (!string.IsNullOrEmpty(diritti))
                {
                    string[] metodiabilitati = diritti.Split(';');
                    if (metodiabilitati.Contains(pis.MethodName.ToUpper()))
                    {
                        row["Sel"] = "true";

                    }
                    else
                    {
                        row["Sel"] = "false";
                    }
                }
                else
                {
                    row["Sel"] = "false";
                }
                dsPIS.Tables[0].Rows.Add(row);
            }
            DataView dv = dsPIS.Tables["PIS"].DefaultView;
            //dv = OrdinaGrid(dv, "FILE_SVC nomePIS");
            this.dg_pis.DataSource = dv;
            this.dg_pis.DataBind();


        }

        private DataView OrdinaGrid(DataView dv, string sortColumn)
        {
            string[] words = dv.Sort.Split(' ');
            string sortMode;
            if (words.Length < 2)
            {
                sortMode = " ASC";
            }
            else
            {
                if (words[1].Equals("ASC"))
                {
                    sortMode = " DESC";
                }
                else
                {
                    sortMode = " ASC";
                }
            }
            dv.Sort = sortColumn + sortMode;
            return dv;
        }

        private void InserimentoRegistri()
        {
            try
            {
                if (this.dg_registri.Items.Count > 0)
                {
                    string idSysRole = this.hd_idSysRole.Value;
                    CheckBox spunta;
                    string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
                    DocsPAWA.DocsPaWR.OrgRegistro registro = null;

                    ArrayList listaRegistriSelezionati = new ArrayList();

                    for (int i = 0; i < this.dg_registri.Items.Count; i++)
                    {
                        spunta = (CheckBox)dg_registri.Items[i].Cells[5].FindControl("cbx_Sel");

                        if (spunta.Checked)
                        {
                            registro = new DocsPAWA.DocsPaWR.OrgRegistro();

                            registro.IDRegistro = dg_registri.Items[i].Cells[0].Text;
                            registro.Codice = dg_registri.Items[i].Cells[1].Text;
                            registro.Descrizione = dg_registri.Items[i].Cells[2].Text;
                            registro.IDAmministrazione = amministrazione[3];
                            registro.Associato = idSysRole;

                            listaRegistriSelezionati.Add(registro);

                            registro = null;
                        }
                    }

                    for (int i = 0; i < this.dg_RF.Items.Count; i++)
                    {
                        spunta = (CheckBox)dg_RF.Items[i].Cells[5].FindControl("cbx_Sel");

                        if (spunta.Checked)
                        {
                            registro = new DocsPAWA.DocsPaWR.OrgRegistro();

                            registro.IDRegistro = dg_RF.Items[i].Cells[0].Text;
                            registro.Codice = dg_RF.Items[i].Cells[1].Text;
                            registro.Descrizione = dg_RF.Items[i].Cells[2].Text;
                            registro.IDAmministrazione = amministrazione[3];
                            registro.Associato = idSysRole;
                            registro.rfDisabled = dg_RF.Items[i].Cells[9].Text;
                            registro.idAOOCollegata = dg_RF.Items[i].Cells[10].Text;

                            if (!(registro.rfDisabled == "1" && (!listaRegistriSelezionati.Contains(registro.idAOOCollegata))))
                            {
                                listaRegistriSelezionati.Add(registro);

                            }
                            else
                            {
                                spunta.Checked = false;
                            }
                            registro = null;
                        }
                    }

                    if (listaRegistriSelezionati != null && listaRegistriSelezionati.Count > 0)
                    {
                        DocsPAWA.DocsPaWR.OrgRegistro[] registri = new DocsPAWA.DocsPaWR.OrgRegistro[listaRegistriSelezionati.Count];
                        listaRegistriSelezionati.CopyTo(registri);
                        listaRegistriSelezionati = null;

                        // prende la system_id della UO padre
                        // Ho bisogno dell'IDCorrGlob dell'id dell'uo di sistema.

                        // Per i test lo cablo. Dopo vedo come prelevarlo.
                        string idUO = "14088633";

                        Amministrazione.Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                        theManager.getListaRegistri();
                        theManager.InsRegistri(registri, idUO, idSysRole);

                        DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
                        esito = theManager.getEsitoOperazione();
                        //aggiorna la visibilità del ruolo sulle caselle di posta
                        if (esito.Codice == 0)
                        {
                            DocsPAWA.DocsPaWR.ValidationResultInfo res = SetVisCaselleRegistri();
                            if (!res.Value)
                            {
                                string scriptString = "<SCRIPT>alert('Attenzione, errore aggiornamento visibilità caselle di posta');</SCRIPT>";
                                this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                            }
                        }
                        if (!esito.Codice.Equals(0))
                        {
                            if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
                            {
                                string scriptString = "<SCRIPT>alert('Attenzione, " + esito.Descrizione.Replace("'", "''") + "');</SCRIPT>";
                                this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                            }
                        }

                        esito = null;
                    }
                    else
                    {
                        if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
                        {
                            string scriptString = "<SCRIPT>alert('Attenzione, nessun registro selezionato');</SCRIPT>";
                            this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                        }
                    }
                }

                //Serve per far ricaricare alla rubrica le modifiche apportate
                DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                ws.resetHashTable();
            }
            catch (Exception ex)
            {

            }
        }

        private DocsPAWA.DocsPaWR.ValidationResultInfo SetVisCaselleRegistri()
        {
            DocsPaWebService ws = new DocsPaWebService();
            string idSysRole = this.hd_idSysRole.Value;

            //elimino tutte le voci associate al ruolo in DPA_VIS_MAIL_REGISTRO
            DocsPAWA.DocsPaWR.ValidationResultInfo result = DocsPAWA.utils.MultiCasellaManager.DeletelRightMailRegistro(string.Empty, idSysRole, string.Empty);
            if (result.Value)
            {
                System.Collections.Generic.List<RightRuoloMailRegistro> rightRuoloMailReg = new System.Collections.Generic.List<RightRuoloMailRegistro>();
                //inserisco le coppie 1 a n (ruolo/casella di posta registro)
                if (this.dg_registri.Items.Count > 0)
                {
                    string idRuolo = idSysRole;
                    foreach (DataGridItem item in dg_registri.Items)
                    {
                        CheckBox cbx_Sel = item.Cells[5].FindControl("cbx_Sel") as CheckBox;
                        // se il registro/rf ha caselle di posta ed è associato al ruolo
                        if (cbx_Sel.Checked && (!item.Cells[3].Text.Equals("&nbsp;") && (!string.IsNullOrEmpty(item.Cells[3].Text))))
                        {
                            if (!string.IsNullOrEmpty(item.Cells[3].Text) && item.Cells[3].Text.Equals("&nbsp;"))
                            {
                                string[] mails = item.Cells[3].Text.Split(';');
                                foreach (string mail in mails)
                                {
                                    if (!string.IsNullOrEmpty(mail))
                                    {
                                        string idRegistro = item.Cells[0].Text;
                                        string consulta = (item.Cells[6].FindControl("cbx_Consulta") as CheckBox).Checked.ToString();
                                        string notifica = (item.Cells[7].FindControl("cbx_Notifica") as CheckBox).Checked.ToString();
                                        string spedisci = (item.Cells[8].FindControl("cbx_Spedisci") as CheckBox).Checked.ToString();
                                        rightRuoloMailReg.Add(new RightRuoloMailRegistro
                                        {
                                            IdRegistro = idRegistro,
                                            idRuolo = idRuolo,
                                            EmailRegistro = mail,
                                            cha_consulta = consulta,
                                            cha_spedisci = spedisci,
                                            cha_notifica = notifica
                                        });
                                    }
                                }
                            }
                        }
                        if (cbx_Sel.Checked && (string.IsNullOrEmpty(item.Cells[3].Text) || (item.Cells[3].Text.Equals("&nbsp;")) || string.IsNullOrEmpty(item.Cells[3].Text)) && ws.IsEnabledInteropInterna())
                        {
                            string idRegistro = item.Cells[0].Text;
                            string consulta = (item.Cells[6].FindControl("cbx_Consulta") as CheckBox).Checked.ToString();
                            string notifica = (item.Cells[7].FindControl("cbx_Notifica") as CheckBox).Checked.ToString();
                            string spedisci = (item.Cells[8].FindControl("cbx_Spedisci") as CheckBox).Checked.ToString();
                            rightRuoloMailReg.Add(new RightRuoloMailRegistro
                            {
                                IdRegistro = idRegistro,
                                idRuolo = idRuolo,
                                EmailRegistro = "",
                                cha_consulta = consulta,
                                cha_spedisci = spedisci,
                                cha_notifica = notifica
                            });
                        }
                    }
                }
                //inserisco le coppie 1 a n (ruolo/casella di posta rf)
                if (this.dg_RF.Items.Count > 0)
                {
                    string idRuolo = idSysRole;
                    foreach (DataGridItem item in dg_RF.Items)
                    {
                        CheckBox cbx_Sel = item.Cells[5].FindControl("cbx_Sel") as CheckBox;
                        // se il registro/rf ha caselle di posta ed è associato al ruolo
                        if (cbx_Sel.Checked && (!item.Cells[3].Text.Equals("&nbsp;") && (!string.IsNullOrEmpty(item.Cells[3].Text))))
                        {
                            if (!string.IsNullOrEmpty(item.Cells[3].Text) && item.Cells[3].Text.Equals("&nbsp;"))
                            {
                                string[] mails = item.Cells[3].Text.Split(';');
                                foreach (string mail in mails)
                                {
                                    if (!string.IsNullOrEmpty(mail))
                                    {
                                        string idRegistro = item.Cells[0].Text;
                                        string consulta = (item.Cells[6].FindControl("cbx_Consulta") as CheckBox).Checked.ToString();
                                        string notifica = (item.Cells[7].FindControl("cbx_Notifica") as CheckBox).Checked.ToString();
                                        string spedisci = (item.Cells[8].FindControl("cbx_Spedisci") as CheckBox).Checked.ToString();
                                        rightRuoloMailReg.Add(new RightRuoloMailRegistro
                                        {
                                            IdRegistro = idRegistro,
                                            idRuolo = idRuolo,
                                            EmailRegistro = mail,
                                            cha_consulta = consulta,
                                            cha_spedisci = spedisci,
                                            cha_notifica = notifica
                                        });
                                    }
                                }
                            }
                        }
                        if (cbx_Sel.Checked && (string.IsNullOrEmpty(item.Cells[3].Text) || (item.Cells[3].Text.Equals("&nbsp;")) || string.IsNullOrEmpty(item.Cells[3].Text)) && ws.IsEnabledInteropInterna())
                        {
                            string idRegistro = item.Cells[0].Text;
                            string consulta = (item.Cells[6].FindControl("cbx_Consulta") as CheckBox).Checked.ToString();
                            string notifica = (item.Cells[7].FindControl("cbx_Notifica") as CheckBox).Checked.ToString();
                            string spedisci = (item.Cells[8].FindControl("cbx_Spedisci") as CheckBox).Checked.ToString();
                            rightRuoloMailReg.Add(new RightRuoloMailRegistro
                            {
                                IdRegistro = idRegistro,
                                idRuolo = idRuolo,
                                EmailRegistro = "",
                                cha_consulta = consulta,
                                cha_spedisci = spedisci,
                                cha_notifica = notifica
                            });
                        }
                    }
                }
                if (rightRuoloMailReg.Count > 0)
                    result = DocsPAWA.utils.MultiCasellaManager.InsertRightMailRegistro(rightRuoloMailReg);
            }
            return result;
        }

                
    }
}