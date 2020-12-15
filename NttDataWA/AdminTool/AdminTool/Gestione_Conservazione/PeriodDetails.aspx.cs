using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAAdminTool.DocsPaWR;
using SAAdminTool.SiteNavigation;
using System.Data;

namespace SAAdminTool.AdminTool.Gestione_Conservazione
{
    public partial class PeriodDetails : System.Web.UI.Page
    {
        private DocsPaWR.DocsPaWebService _wsInstance = new DocsPaWebService();

        protected void Page_Load(object sender, EventArgs e)
        {
            Utils.startUp(this);
            if (!IsPostBack)
            {
                Initialize();
                FetchTipiConservazione();
                PopulatePolicyPeriod();
                
            }

            Corrispondente tempCorr = UserManager.getCorrispondenteSelezionatoRuoloAmministrazione(this.Page);
            if (tempCorr != null)
            {
                txtCodRuolo.Text = tempCorr.codiceRubrica;
                txtDescRuolo.Text = tempCorr.descrizione;
                id_corr.Value = tempCorr.systemId;
                Utente[] roleUsers = _wsInstance.getUserInRoleByIdCorrGlobali(tempCorr.systemId);
                if (roleUsers != null && roleUsers.Length > 0)
                {
                    this.ddl_role_users.Enabled = true;
                    this.ddl_role_users.Items.Clear();
                    for (int i = 0; i < roleUsers.Length; i++)
                    {
                        ddl_role_users.Items.Add(roleUsers[i].descrizione);
                        ddl_role_users.Items[i].Value = (roleUsers[i].systemId).ToString();
                    }
                }
                else
                {
                    this.ddl_role_users.Enabled = false;
                    this.ddl_role_users.Items.Clear();
                }
                UserManager.removeCorrispondentiSelezionati(this.Page);
            }
        }

        protected void Initialize()
        {
            this.btnApriRubrica.Attributes.Add("onmouseover", "this.src='../../images/proto/rubrica_hover.gif'");
            this.btnApriRubrica.Attributes.Add("onmouseout", "this.src='../../images/proto/rubrica.gif'");
            this.btnApriRubrica.OnClientClick = String.Format("openTransmissionAddressBook();");

            DocsPaWR.Utente ut = new SAAdminTool.DocsPaWR.Utente();
            string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            string codiceAmministrazione = amministrazione[0];
            string idAmministrazione = IdAmministrazione.ToString();

            ut.codiceAmm = codiceAmministrazione;
            ut.idAmministrazione = idAmministrazione;
            ut.tipoIE = "I";
            Session.Add("userData", ut);

            DocsPaWR.Ruolo rl = new SAAdminTool.DocsPaWR.Ruolo();
            rl.codiceAmm = codiceAmministrazione;
            rl.idAmministrazione = idAmministrazione;
            rl.tipoIE = "I";

            rl.systemId = idAmministrazione;
            rl.uo = new SAAdminTool.DocsPaWR.UnitaOrganizzativa();
            rl.uo.codiceRubrica = codiceAmministrazione;

            Session.Add("userRuolo", rl);

            DocsPaWR.Registro reg = new SAAdminTool.DocsPaWR.Registro();
            reg.codAmministrazione = codiceAmministrazione;
            reg.idAmministrazione = idAmministrazione;
            Session.Add("userRegistro", reg);

            if (Request.QueryString["id"] != null)
            {
                string idPolicy = Request.QueryString["id"].ToString();
                this.Policy = _wsInstance.GetPolicyById(idPolicy);
            }
        }

        protected void PopulatePolicyPeriod()
        {
            if (!string.IsNullOrEmpty(this.Policy.tipoPeriodo))
            {
                if (this.Policy.tipoPeriodo.Equals(this.type1.Value))
                {
                    this.type1.Checked = true;
                }
                if (this.Policy.tipoPeriodo.Equals(this.type2.Value))
                {
                    this.type2.Checked = true;
                }
                if (this.Policy.tipoPeriodo.Equals(this.type3.Value))
                {
                    this.type3.Checked = true;
                }
                if (this.Policy.tipoPeriodo.Equals(this.type4.Value))
                {
                    this.type4.Checked = true;
                }
            }

            this.txtNumGiorni.Text = this.Policy.periodoGiornalieroNGiorni;
            this.txtOreGiorni.Text = this.Policy.periodoGiornalieroOre;
            this.txtMinutiGiorni.Text = this.Policy.periodoGiornalieroMinuti;

            this.txtOreSettimana.Text = this.Policy.periodoSettimanaleOre;
            this.txtMinutiSettimana.Text = this.Policy.periodoSettimanaleMinuti;

            if (this.Policy.periodoSettimanaleLunedi)
            {
                this.chK_lunedi.Selected = true;
            }
            if (this.Policy.periodoSettimanaleMartedi)
            {
                this.chK_martedi.Selected = true;
            }
            if (this.Policy.periodoSettimanaleMercoledi)
            {
                this.chK_mercoledi.Selected = true;
            }
            if (this.Policy.periodoSettimanaleGiovedi)
            {
                this.chK_giovedi.Selected = true;
            }
            if (this.Policy.periodoSettimanaleVenerdi)
            {
                this.chK_venerdi.Selected = true;
            }
            if (this.Policy.periodoSettimanaleSabato)
            {
                this.chK_sabato.Selected = true;
            }
            if (this.Policy.periodoSettimanaleDomenica)
            {
                this.chK_domenica.Selected = true;
            }

            this.txt_day.Text = this.Policy.periodoMensileGiorni;
            this.txtOreMesi.Text = this.Policy.periodoMensileOre;
            this.txtMinutiMese.Text = this.Policy.periodoMensileMinuti;

            if (!string.IsNullOrEmpty(this.Policy.idRuolo))
            {
                Corrispondente tempCorr = UserManager.getCorrispondenteBySystemIDDisabled(this.Page, this.Policy.idRuolo);
                if (tempCorr != null)
                {
                    this.txtCodRuolo.Text = tempCorr.codiceRubrica;
                    this.txtDescRuolo.Text = tempCorr.descrizione;
                    this.id_corr.Value = tempCorr.systemId;
                    Utente[] roleUsers = _wsInstance.getUserInRoleByIdCorrGlobali(tempCorr.systemId);
                    if (roleUsers != null && roleUsers.Length > 0)
                    {
                        this.ddl_role_users.Enabled = true;
                        this.ddl_role_users.Items.Clear();
                        for (int i = 0; i < roleUsers.Length; i++)
                        {
                            ddl_role_users.Items.Add(roleUsers[i].descrizione);
                            ddl_role_users.Items[i].Value = (roleUsers[i].systemId).ToString();
                        }
                    }
                    else
                    {
                        this.ddl_role_users.Enabled = false;
                    }
                }
                else
                {
                    this.txtCodRuolo.Text = string.Empty;
                    this.txtDescRuolo.Text = string.Empty;
                    this.id_corr.Value = string.Empty;
                    this.ddl_role_users.Enabled = false;
                    this.ddl_role_users.Items.Clear();
                }
            }

            if (!string.IsNullOrEmpty(this.Policy.idUtenteRuolo))
            {
                Corrispondente tempCorr = UserManager.getCorrispondenteBySystemIDDisabled(this.Page, this.Policy.idUtenteRuolo);
                if (tempCorr != null)
                {
                    this.ddl_role_users.SelectedValue = this.Policy.idUtenteRuolo;
                }
                else
                {
                    this.ddl_role_users.Enabled = false;
                    this.ddl_role_users.Items.Clear();
                }
            }
            else
            {
                this.ddl_role_users.Enabled = false;
                this.ddl_role_users.Items.Clear();
            }

            if (this.Policy.consolidazione)
            {
                this.chk_consolidamento.Checked = true;
            }

            this.txtAvvisoMesi.Text = this.Policy.avvisoMesi;
            // MEV CS 1.5
            this.txtAvvisoMesiLegg.Text = this.Policy.avvisoMesiLegg;
            // fine MEV CS 1.5

            if (this.Policy.periodoAttivo)
            {
                this.chk_attiva.Checked = true;
            }
            this.chk_invio_automatico.Checked = this.Policy.statoInviato;

            //MEV CS 1.5 F02_01 conversione automatica documenti
            if (this.chk_invio_automatico.Checked)
            {
                this.chk_conversione_automatica.Enabled = true;
                //this.chk_conversione_automatica.Checked = false;
            }
            else
            {
                this.chk_conversione_automatica.Enabled = false;
                this.chk_conversione_automatica.Checked = false;
            }
            this.chk_conversione_automatica.Checked = this.Policy.statoConversione;
            //fine MEV CS 1.5 F02_01 conversione automatica documenti

            this.txt_a_ore.Text = this.Policy.periodoAnnualeOre;
            this.txt_a_mese.Text = this.Policy.periodoAnnualeMese;
            this.txt_a_day.Text = this.Policy.periodoAnnualeGiorno;
            this.txt_a_mm.Text = this.Policy.periodoAnnualeMinuti;
            if (this.ddl_tipoCons.Items.FindByValue(this.Policy.tipoConservazione) != null)
            {
                this.ddl_tipoCons.SelectedValue = this.Policy.tipoConservazione;
            }

            if (this.ddl_tipoCons.SelectedValue == "CONSERVAZIONE_CONSOLIDATA")
            {
                this.chk_consolidamento.Checked = true;
                this.chk_consolidamento.Enabled = false;
            }
            if (this.ddl_tipoCons.SelectedValue == "CONSERVAZIONE_NON_CONSOLIDATA")
            {
                this.chk_consolidamento.Checked = false;
                this.chk_consolidamento.Enabled = false;
            }
            if (this.ddl_tipoCons.SelectedValue == "ESIBIZIONE")
            {
                this.chk_consolidamento.Checked = false;
                this.chk_consolidamento.Enabled = false;
            }
            if (this.ddl_tipoCons.SelectedValue == "CONSERVAZIONE_INTERNA")
            {
                this.chk_consolidamento.Checked = false;
                this.chk_consolidamento.Enabled = true;
            }
        }

        protected void BtnSave_Click(object sender, EventArgs e)
        {
            GetPeriodPolicyDetails();
            if (!string.IsNullOrEmpty(this.txtCodRuolo.Text) && !string.IsNullOrEmpty(this.txtDescRuolo.Text) && !string.IsNullOrEmpty(this.id_corr.Value))
            {
                if (this.chk_attiva.Checked)
                {
                    if (!this.type1.Checked && !this.type2.Checked && !this.type3.Checked && !this.type4.Checked)
                    {
                        this.chk_attiva.Checked = false;
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_prole", "alert('Per attivare questa Policy devi selezionare almeno un periodo temporale');", true);
                    }
                    else
                    {
                        if (this.type1.Checked)
                        {
                            if (string.IsNullOrEmpty(this.txtNumGiorni.Text) || string.IsNullOrEmpty(this.txtOreGiorni.Text) || string.IsNullOrEmpty(this.txtMinutiGiorni.Text))
                            {
                                this.chk_attiva.Checked = false;
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_prole", "alert('Definire correttamente data e ora');", true);
                            }
                            else
                            {
                                bool retval = _wsInstance.SavePeriodPolicy(this.Policy);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi", "close_and_save('Y');", true);
                            }
                        }
                        if (this.type2.Checked)
                        {
                            if (string.IsNullOrEmpty(this.txtOreSettimana.Text) || string.IsNullOrEmpty(this.txtMinutiSettimana.Text))
                            {
                                this.chk_attiva.Checked = false;
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_prole", "alert('Inserire data e ora se vuoi attivare il periodo settimanale');", true);
                            }
                            else
                            {
                                bool retval = _wsInstance.SavePeriodPolicy(this.Policy);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi", "close_and_save('Y');", true);
                            }
                        }
                        if (this.type3.Checked)
                        {
                            if (string.IsNullOrEmpty(this.txt_day.Text) || string.IsNullOrEmpty(txtOreMesi.Text) || string.IsNullOrEmpty(txtMinutiMese.Text))
                            {
                                this.chk_attiva.Checked = false;
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_prole", "alert('Inserire giorno, ora e minuti della policy mensile');", true);
                            }
                            else
                            {
                                bool retval = _wsInstance.SavePeriodPolicy(this.Policy);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi", "close_and_save('Y');", true);
                            }
                        }
                        if (this.type4.Checked)
                        {
                            if (string.IsNullOrEmpty(this.txt_a_mese.Text) || string.IsNullOrEmpty(this.txt_a_day.Text) || string.IsNullOrEmpty(this.txt_a_ore.Text) || string.IsNullOrEmpty(this.txt_a_mm.Text))
                            {
                                this.chk_attiva.Checked = false;
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_prole", "alert('Inserire mese, giorno, ora e minuti per attivare la policy annuale');", true);
                            }
                            else
                            {
                                bool retval = _wsInstance.SavePeriodPolicy(this.Policy);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi", "close_and_save('Y');", true);
                            }
                        }
                    }
                }
                else
                {
                    bool retval = _wsInstance.SavePeriodPolicy(this.Policy);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi", "close_and_save('Y');", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_alert", "alert('Inserire il ruolo proprietario della conservazione');", true);
            }
           
        }

        protected void Chk_attivaChecked(object sender, EventArgs e)
        {
           
        }

        protected void GetPeriodPolicyDetails()
        {
            if (this.type1.Checked)
            {
                this.Policy.tipoPeriodo = this.type1.Value;
            }
            if (this.type2.Checked)
            {
                this.Policy.tipoPeriodo = this.type2.Value;
            }
            if (this.type3.Checked)
            {
                this.Policy.tipoPeriodo = this.type3.Value;
            }
            if (this.type4.Checked)
            {
                this.Policy.tipoPeriodo = this.type4.Value;
            }

            this.Policy.periodoGiornalieroNGiorni = this.txtNumGiorni.Text;
            this.Policy.periodoGiornalieroOre = this.txtOreGiorni.Text;
            this.Policy.periodoGiornalieroMinuti = this.txtMinutiGiorni.Text;
            this.Policy.periodoSettimanaleOre = this.txtOreSettimana.Text;
            this.Policy.periodoSettimanaleMinuti = this.txtMinutiSettimana.Text;
            this.Policy.periodoSettimanaleLunedi = this.chK_lunedi.Selected;
            this.Policy.periodoSettimanaleMartedi = this.chK_martedi.Selected;
            this.Policy.periodoSettimanaleMercoledi = this.chK_mercoledi.Selected;
            this.Policy.periodoSettimanaleGiovedi = this.chK_giovedi.Selected;
            this.Policy.periodoSettimanaleVenerdi = this.chK_venerdi.Selected;
            this.Policy.periodoSettimanaleSabato = this.chK_sabato.Selected;
            this.Policy.periodoSettimanaleDomenica = this.chK_domenica.Selected;
            this.Policy.periodoMensileGiorni = this.txt_day.Text;
            this.Policy.periodoMensileOre = this.txtOreMesi.Text;
            this.Policy.periodoMensileMinuti = this.txtMinutiMese.Text;
            this.Policy.idRuolo = this.id_corr.Value;
            this.Policy.consolidazione = this.chk_consolidamento.Checked;
            this.Policy.avvisoMesi = this.txtAvvisoMesi.Text;
            // MEV CS 1.5
            this.Policy.avvisoMesiLegg = this.txtAvvisoMesiLegg.Text;
            // fine MEV CS 1.5
            this.Policy.periodoAttivo = this.chk_attiva.Checked;
            this.Policy.idUtenteRuolo = this.ddl_role_users.SelectedValue;
            this.Policy.statoInviato = this.chk_invio_automatico.Checked;
            // MEV CS 1.5 F02_01 conversione automatica
            this.Policy.statoConversione = this.chk_conversione_automatica.Checked;
            // fine MEV CS 1.5 F02_01 conversione automatica
            this.Policy.periodoAnnualeMese = this.txt_a_mese.Text;
            this.Policy.periodoAnnualeGiorno = this.txt_a_day.Text;
            this.Policy.periodoAnnualeOre = this.txt_a_ore.Text;
            this.Policy.periodoAnnualeMinuti = this.txt_a_mm.Text;
            this.Policy.tipoConservazione = this.ddl_tipoCons.SelectedValue;

        }

        /// <summary>
        /// Evento generato al cambio del testo nella casella del codice rubrica
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtCodRuolo_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCodRuolo.Text))
            {
                setDescCorr(txtCodRuolo.Text);
            }
            else
            {
                txtCodRuolo.Text = string.Empty;
                txtDescRuolo.Text = string.Empty;
                id_corr.Value = string.Empty;
                this.ddl_role_users.Enabled = false;
                this.ddl_role_users.Items.Clear();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_prole", "alert('Inserire un codice da cercare in rubrica');", true);
            }
            this.upAddress.Update();
        }

        // MEV CS 1.5 F02_01 conversione automatica
        /// <summary>
        /// Evento generato al check del chekcbox chk_invio_automatico
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chk_invio_automatico_CheckedChanged(object sender, EventArgs e)
        {
            bool check = ((CheckBox)sender).Checked;
            if (check)
            {
                this.chk_conversione_automatica.Enabled = true;
            }
            else
            {
                this.chk_conversione_automatica.Enabled = false;
                this.chk_conversione_automatica.Checked = false;
            }
            this.upPnlConversioneAutomatica.Update();
        }
        // FINE MEV CS 1.5 F02_01 conversione automatica

        protected void setDescCorr(string codRubrica)
        {
            DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteByCodRubricaIE(this.Page, codRubrica, AddressbookTipoUtente.INTERNO);
            if (corr == null)
            {
                txtCodRuolo.Text = string.Empty;
                txtDescRuolo.Text = string.Empty;
                id_corr.Value = string.Empty;
                this.ddl_role_users.Enabled = false;
                this.ddl_role_users.Items.Clear();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_found", "alert('Corrispondente non trovato');", true);
            }
            else
            {
                if (corr != null && !corr.tipoCorrispondente.Equals("R"))
                {
                    this.ddl_role_users.Enabled = false;
                    this.ddl_role_users.Items.Clear();
                    txtCodRuolo.Text = string.Empty;
                    txtDescRuolo.Text = string.Empty;
                    id_corr.Value = string.Empty;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_found", "alert('Inserire soltanto ruoli');", true);
                }
                else
                {
                    txtCodRuolo.Text = corr.codiceRubrica;
                    txtDescRuolo.Text = corr.descrizione;
                    id_corr.Value = corr.systemId;
                    Utente[] roleUsers = _wsInstance.getUserInRoleByIdCorrGlobali(corr.systemId);
                    if (roleUsers != null && roleUsers.Length > 0)
                    {
                        this.ddl_role_users.Enabled = true;
                        this.ddl_role_users.Items.Clear();
                        for (int i = 0; i < roleUsers.Length; i++)
                        {
                            ddl_role_users.Items.Add(roleUsers[i].descrizione);
                            ddl_role_users.Items[i].Value = (roleUsers[i].systemId).ToString();
                         }
                    }
                    else
                    {
                        this.ddl_role_users.Enabled = false;
                        this.ddl_role_users.Items.Clear();
                    }
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        protected void FetchTipiConservazione()
        {
            this.ddl_tipoCons.DataSource = ProxyManager.getWS().GetTipologieIstanzeConservazione();
          
            this.ddl_tipoCons.DataBind();
            // --------------------------------------------------------------------------------
            // Patch per evitare schianti:
            // Non so cosa volevano fare, comunque se non ci sono almeno 3 elementi, questa operazione schianta.
            // Aggiungo il controllo if (ddl_tipoCons.Items.Count > 2) per evitare lo schianto.
            // --------------------------------------------------------------------------------
            if (ddl_tipoCons.Items.Count > 2)
                this.ddl_tipoCons.Items.RemoveAt(2);
        }

        protected void SelectTipoConservazione(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.ddl_tipoCons.SelectedValue))
            {
                if (this.ddl_tipoCons.SelectedValue == "CONSERVAZIONE_CONSOLIDATA")
                {
                    this.chk_consolidamento.Checked = true;
                    this.chk_consolidamento.Enabled = false;
                }
                if (this.ddl_tipoCons.SelectedValue == "CONSERVAZIONE_NON_CONSOLIDATA")
                {
                    this.chk_consolidamento.Checked = false;
                    this.chk_consolidamento.Enabled = false;
                }
                if (this.ddl_tipoCons.SelectedValue == "ESIBIZIONE")
                {
                    this.chk_consolidamento.Checked = false;
                    this.chk_consolidamento.Enabled = false;
                }
                if (this.ddl_tipoCons.SelectedValue == "CONSERVAZIONE_INTERNA")
                {
                    this.chk_consolidamento.Checked = false;
                    this.chk_consolidamento.Enabled = true;
                }
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
    }
}