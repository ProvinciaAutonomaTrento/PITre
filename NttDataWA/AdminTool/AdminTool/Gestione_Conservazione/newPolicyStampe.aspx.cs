using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using SAAdminTool.SiteNavigation;
using SAAdminTool.DocsPaWR;
using System.Text;
using System.Collections;

namespace SAAdminTool.AdminTool.Gestione_Conservazione
{
    public partial class newPolicyStampe : System.Web.UI.Page
    {
        private DocsPaWR.DocsPaWebService _wsInstance = null;


        protected void Page_Load(object sender, EventArgs e)
        {
            Utils.startUp(this);
            if (!IsPostBack)
            {
                Repertori = GetRegisteredRegistries();
                GetTypeRegistries();
                this.lbl_dataCreazioneA.Visible = false;
                this.lblA.Visible = false;
                if (Request.QueryString["id"] != null && !string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    ViewDetailsPolicy();
                    this.titlePage.Text = "Dettaglio della Policy";
                }

                if (rbl_tipo.SelectedValue.Equals("C"))
                {
                    this.ddl_rf_aoo.Enabled = true;
                    this.ddl_type_repertorio.Enabled = true;
                }
                else
                {
                    this.ddl_rf_aoo.Enabled = false;
                    this.ddl_type_repertorio.Enabled = false;
                }
            }
        }

        protected RegistroRepertorio[] GetRegisteredRegistries()
        {
            RegisteredRegistriRepertorioRequest req = new RegisteredRegistriRepertorioRequest();
            req.AdministrationId = IdAmministrazione.ToString();
            RegisteredRegistriRepertorioResponse resp = WsInstance.GetRegisteredRegistries(req);
            return resp.RegistriRepertorio;
        }

        protected void GetTypeRegistries()
        {
            if (Repertori != null && Repertori.Length > 0)
            {
                ddl_type_repertorio.Items.Clear();
                ddl_type_repertorio.Items.Add("");
                for (int i = 0; i < Repertori.Length; i++)
                {
                    ddl_type_repertorio.Items.Add(Repertori[i].TipologyDescription + " - " + Repertori[i].CounterDescription);
                    ddl_type_repertorio.Items[i + 1].Value = Repertori[i].CounterId;
                }
            }
            this.upTipoRepertori.Update();
        }

        protected void RepertoriChange_Click(object sender, EventArgs e)
        {
            if (Repertori != null && Repertori.Length != 0 && !String.IsNullOrEmpty(ddl_type_repertorio.SelectedValue))
            {
                RegistroRepertorio repertorio = Repertori.Where(rep => rep.CounterId.Equals(ddl_type_repertorio.SelectedValue)).FirstOrDefault();


                if (repertorio != null)
                {
                    RegistroRepertorioSettingsRequest request = new RegistroRepertorioSettingsRequest();
                    request.CounterId = repertorio.CounterId;
                    request.TipologyKind = TipologyKind.D;

                    RegistroRepertorioSettingsResponse resp = new RegistroRepertorioSettingsResponse();
                    resp = WsInstance.GetRegisterSettings(request);

                    RegistroRepertorioSingleSettings r = resp.RegistroRepertorioSingleSettings;

                    ddl_rf_aoo.Items.Clear();
                    ddl_rf_aoo.Items.Add(new ListItem("Tutti", "TUTTI"));

                    int num = 0;
                    Registro[] reg = WsInstance.GetRfByIdAmm(IdAmministrazione, "0");
                    if (reg != null && reg.Length > 0)
                    {
                        num = reg.Length;

                        for (int i = 0; i < reg.Length; i++)
                        {
                            string descrizione = "[" + reg[i].codice + "]" + " - " + reg[i].descrizione;
                            ddl_rf_aoo.Items.Add(descrizione);
                            ddl_rf_aoo.Items[i + 1].Value = reg[i].systemId;
                        }
                    }

                    Registro[] rf = WsInstance.GetRfByIdAmm(IdAmministrazione, "1");
                    if (rf != null && rf.Length > 0)
                    {
                        num = rf.Length + num;
                        int z = 0;
                        for (int y = reg.Length; y < num; y++, z++)
                        {
                            string descrizione = "[" + rf[z].codice + "]" + " - " + rf[z].descrizione;
                            ddl_rf_aoo.Items.Add(descrizione);
                            ddl_rf_aoo.Items[y + 1].Value = rf[z].systemId;
                        }
                    }


                }
            }
            else
            {
                ddl_rf_aoo.Items.Clear();
            }
            this.upRfAOO.Update();
        }

        public void Page_Prerender(object sender, EventArgs e)
        {

        }

        protected void ViewDetailsPolicy()
        {
            string idPolicy = Request.QueryString["id"].ToString();
            this.Policy = WsInstance.GetPolicyById(idPolicy);
            this.txt_nome.Text = this.Policy.nome;

            this.lbl_dataCreazioneDa.txt_Data.Text = this.Policy.dataCreazioneDa;

            if (!string.IsNullOrEmpty(this.Policy.tipoDataCreazione))
            {
                this.ddl_dataCreazione_E.SelectedValue = this.Policy.tipoDataCreazione;

                if (this.Policy.tipoDataCreazione.Equals("2") || this.Policy.tipoDataCreazione.Equals("3") || this.Policy.tipoDataCreazione.Equals("4") || this.Policy.tipoDataCreazione.Equals("5"))
                {
                    this.lblDa.Visible = false;
                    this.lbl_dataCreazioneDa.Visible = false;
                    this.lblA.Visible = false;
                    this.lbl_dataCreazioneA.Visible = false;
                }
                else
                {
                    if (this.Policy.tipoDataCreazione.Equals("0"))
                    {
                        this.lblDa.Visible = true;
                        this.lbl_dataCreazioneDa.Visible = true;
                        this.lbl_dataCreazioneDa.txt_Data.Text = this.Policy.dataCreazioneDa;

                        this.lblA.Visible = false;
                        this.lbl_dataCreazioneA.Visible = false;
                        this.lbl_dataCreazioneA.txt_Data.Text = string.Empty;
                    }
                    if (this.Policy.tipoDataCreazione.Equals("1"))
                    {
                        this.lblDa.Visible = true;
                        this.lbl_dataCreazioneDa.Visible = true;
                        this.lbl_dataCreazioneDa.txt_Data.Text = this.Policy.dataCreazioneDa;
                        if (!string.IsNullOrEmpty(this.Policy.dataCreazioneA))
                        {
                            this.lblA.Visible = true;
                            this.lbl_dataCreazioneA.Visible = true;
                            this.lbl_dataCreazioneA.txt_Data.Text = this.Policy.dataCreazioneA;
                        }
                    }
                }
            }

            this.rbl_tipo.SelectedValue = this.Policy.tipo;

            if (this.Policy.tipo.Equals("C"))
            {
                GetTypeRegistries();
                this.ddl_rf_aoo.Enabled = true;
                this.ddl_type_repertorio.Enabled = true;

                if (!string.IsNullOrEmpty(this.Policy.idTemplate))
                {
                    this.ddl_type_repertorio.SelectedValue = this.Policy.idTemplate;

                    if (Repertori != null && Repertori.Length != 0 && !String.IsNullOrEmpty(ddl_type_repertorio.SelectedValue))
                    {
                        RegistroRepertorio repertorio = Repertori.Where(rep => rep.CounterId.Equals(ddl_type_repertorio.SelectedValue)).FirstOrDefault();


                        if (repertorio != null)
                        {
                            RegistroRepertorioSettingsRequest request = new RegistroRepertorioSettingsRequest();
                            request.CounterId = repertorio.CounterId;
                            request.TipologyKind = TipologyKind.D;

                            RegistroRepertorioSettingsResponse resp = new RegistroRepertorioSettingsResponse();
                            resp = WsInstance.GetRegisterSettings(request);

                            RegistroRepertorioSingleSettings r = resp.RegistroRepertorioSingleSettings;

                            ddl_rf_aoo.Items.Clear();
                            ddl_rf_aoo.Items.Add(new ListItem("Tutti", "TUTTI"));

                            int num = 0;
                            Registro[] reg = WsInstance.GetRfByIdAmm(IdAmministrazione, "0");
                            if (reg != null && reg.Length > 0)
                            {
                                num = reg.Length;

                                for (int i = 0; i < reg.Length; i++)
                                {
                                    string descrizione = "[" + reg[i].codice + "]" + " - " + reg[i].descrizione;
                                    ddl_rf_aoo.Items.Add(descrizione);
                                    ddl_rf_aoo.Items[i + 1].Value = reg[i].systemId;
                                }
                            }

                            Registro[] rf = WsInstance.GetRfByIdAmm(IdAmministrazione, "1");
                            if (rf != null && rf.Length > 0)
                            {
                                num = rf.Length + num;
                                int z = 0;
                                for (int y = reg.Length; y < num; y++, z++)
                                {
                                    string descrizione = "[" + rf[z].codice + "]" + " - " + rf[z].descrizione;
                                    ddl_rf_aoo.Items.Add(descrizione);
                                    ddl_rf_aoo.Items[y + 1].Value = rf[z].systemId;
                                }
                            }


                        }
                    }
                    else
                    {
                        ddl_rf_aoo.Items.Clear();
                    }


                    if (!string.IsNullOrEmpty(this.Policy.idRf))
                    {
                        this.ddl_rf_aoo.SelectedValue = this.Policy.idRf;
                    }
                }
            }
            else
            {
                this.ddl_rf_aoo.Enabled = false;
                this.ddl_type_repertorio.Enabled = false;
            }
        }

        protected void BtnSaveDocument_Click(object sender, EventArgs e)
        {
            bool insert = CheckValues();
            if (insert)
            {
                Policy policy = PopulatePolicy();
                if (Request.QueryString["s"] != null && Request.QueryString["s"].ToString().Equals("new"))
                {
                    //Inserimento di una nuova policy
                    bool result = WsInstance.InserisciPolicyConservazione(policy);

                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi", "close_and_save('Y');", true);
                }
                else
                {
                    //Modifica policy
                    policy.system_id = Request.QueryString["id"].ToString();
                    bool result = WsInstance.ModifyPolicyConservazione(policy);

                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi", "close_and_save('Y');", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_stampa", "alert('Inserire il nome della Policy');", true);
            }
        }

        protected Policy PopulatePolicy()
        {
            Policy result = new Policy();
            result.automatico = false;
            result.idAmministrazione = IdAmministrazione.ToString();

            result.nome = this.txt_nome.Text;

            if (!string.IsNullOrEmpty(this.rbl_tipo.SelectedValue))
            {
                result.tipo = this.rbl_tipo.SelectedValue;
                if (rbl_tipo.SelectedValue.Equals("C"))
                {
                    if (!string.IsNullOrEmpty(this.ddl_type_repertorio.SelectedValue))
                    {
                        result.idTemplate = this.ddl_type_repertorio.SelectedValue;
                        if (!string.IsNullOrEmpty(this.ddl_rf_aoo.SelectedValue) && !(this.ddl_rf_aoo.SelectedValue.ToUpper().Equals("TUTTI")))
                        {
                            result.idRf = this.ddl_rf_aoo.SelectedValue;
                        }
                        else
                        {
                            result.idRf = string.Empty;
                        }
                    }
                }
            }

            result.tipoDataCreazione = this.ddl_dataCreazione_E.SelectedValue;
            result.dataCreazioneDa = this.lbl_dataCreazioneDa.Text;
            if (this.lbl_dataCreazioneA != null && !string.IsNullOrEmpty(this.lbl_dataCreazioneA.Text))
            {
                result.dataCreazioneA = this.lbl_dataCreazioneA.Text;
            }

            return result;
        }

        protected bool CheckValues()
        {
            bool result = false;
            if ((!string.IsNullOrEmpty(this.txt_nome.Text)))
            {
                result = true;
            }

            return result;
        }

        protected void ddl_dataCreazione_E_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_dataCreazione_E.SelectedIndex)
            {
                case 0:
                    this.lblA.Visible = false;
                    this.lbl_dataCreazioneA.Visible = false;
                    this.lbl_dataCreazioneA.Text = string.Empty;

                    this.lblDa.Visible = true;
                    this.lbl_dataCreazioneDa.Visible = true;
                    break;

                case 1:
                    this.lblA.Visible = true;
                    this.lbl_dataCreazioneA.Visible = true;

                    this.lblDa.Visible = true;
                    this.lbl_dataCreazioneDa.Visible = true;
                    break;

                case 2:
                    this.lblA.Visible = false;
                    this.lbl_dataCreazioneA.Visible = false;
                    this.lbl_dataCreazioneA.Text = string.Empty;

                    this.lblDa.Visible = false;
                    this.lbl_dataCreazioneDa.Visible = false;
                    this.lbl_dataCreazioneDa.Text = string.Empty;
                    break;

                case 3:
                    this.lblA.Visible = false;
                    this.lbl_dataCreazioneA.Visible = false;
                    this.lbl_dataCreazioneA.Text = string.Empty;

                    this.lblDa.Visible = false;
                    this.lbl_dataCreazioneDa.Visible = false;
                    this.lbl_dataCreazioneDa.Text = string.Empty;
                    break;

                case 4:
                    this.lblA.Visible = false;
                    this.lbl_dataCreazioneA.Visible = false;
                    this.lbl_dataCreazioneA.Text = string.Empty;

                    this.lblDa.Visible = false;
                    this.lbl_dataCreazioneDa.Visible = false;
                    this.lbl_dataCreazioneDa.Text = string.Empty;
                    break;

                case 5:
                    this.lblA.Visible = false;
                    this.lbl_dataCreazioneA.Visible = false;
                    this.lbl_dataCreazioneA.Text = string.Empty;

                    this.lblDa.Visible = false;
                    this.lbl_dataCreazioneDa.Visible = false;
                    this.lbl_dataCreazioneDa.Text = string.Empty;
                    break;
            }

            this.upCreationDate.Update();
        }

        protected void StampeChange_Click(object sender, EventArgs e)
        {
            if (rbl_tipo.SelectedValue.Equals("C"))
            {
                this.ddl_rf_aoo.Enabled = true;
                this.ddl_type_repertorio.Enabled = true;
            }
            else
            {
                this.ddl_rf_aoo.Enabled = false;
                this.ddl_type_repertorio.Enabled = false;
            }
            this.upRfAOO.Update();
            this.upTipoRepertori.Update();
        }


        protected DocsPaWR.DocsPaWebService WsInstance
        {
            get
            {
                if (this._wsInstance == null)
                {
                    this._wsInstance = new SAAdminTool.DocsPaWR.DocsPaWebService();
                    this._wsInstance.Timeout = System.Threading.Timeout.Infinite;
                }
                return this._wsInstance;
            }
        }

        /// <summary>
        /// Reperimento idamministrazione corrente
        /// </summary>
        protected int IdAmministrazione
        {
            get
            {
                string idAmministrazione = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");

                return Convert.ToInt32(idAmministrazione);
            }
        }


        /// <summary>
        /// Policy selezionata
        /// </summary>
        protected Policy Policy
        {
            get
            {
                if (CallContextStack.CurrentContext != null && CallContextStack.CurrentContext.ContextState["policy"] != null)
                {
                    if (CallContextStack.CurrentContext == null)
                    {
                        CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                    }
                    return CallContextStack.CurrentContext.ContextState["policy"] as Policy;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (CallContextStack.CurrentContext == null)
                {
                    CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                }
                CallContextStack.CurrentContext.ContextState["policy"] = value;
            }
        }

        /// <summary>
        /// Repertori
        /// </summary>
        protected RegistroRepertorio[] Repertori
        {
            get
            {
                if (CallContextStack.CurrentContext != null && CallContextStack.CurrentContext.ContextState["repertori"] != null)
                {
                    if (CallContextStack.CurrentContext == null)
                    {
                        CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                    }
                    return CallContextStack.CurrentContext.ContextState["repertori"] as RegistroRepertorio[];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (CallContextStack.CurrentContext == null)
                {
                    CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                }
                CallContextStack.CurrentContext.ContextState["repertori"] = value;
            }
        }

    }

}