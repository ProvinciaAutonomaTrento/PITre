using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.SiteNavigation;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.AdminTool.Gestione_Conservazione
{
    public partial class RespConservazione : System.Web.UI.Page
    {

        private DocsPaWR.DocsPaWebService _wsInstance = new DocsPAWA.DocsPaWR.DocsPaWebService();

        private enum FunctionsToAuthorize
        {
            DO_CONSOLIDAMENTO,
            DO_CONSOLIDAMENTO_METADATI,
            DO_SACER_VERSAMENTO,
            DO_SACER_RECUPERO
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Initialize();
                this.SaveUserInSession();

                this.FetchData();
                
            }
        }

        private void Initialize()
        {
            this.btnApriRubrica.Attributes.Add("onmouseover", "this.src='../../images/proto/rubrica_hover.gif'");
            this.btnApriRubrica.Attributes.Add("onmouseout", "this.src='../../images/proto/rubrica.gif'");
            this.btnApriRubrica.OnClientClick = String.Format("OpenAddressBook();");
            this.UsersInRole = null;
        }

        private void SaveUserInSession()
        {
            DocsPaWebService ws = new DocsPaWebService();
            ws.Timeout = System.Threading.Timeout.Infinite;
            string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            string codiceAmministrazione = amministrazione[0];
            string idAmministrazione = ws.getIdAmmByCod(codiceAmministrazione);

            DocsPAWA.DocsPaWR.Utente ut = new DocsPAWA.DocsPaWR.Utente();
            ut.codiceAmm = codiceAmministrazione;
            ut.idAmministrazione = idAmministrazione;
            ut.tipoIE = "I";

            //ut.idRegistro = idRegistro;

            Session.Add("userData", ut);

            DocsPAWA.DocsPaWR.Ruolo rl = new DocsPAWA.DocsPaWR.Ruolo();
            rl.codiceAmm = codiceAmministrazione;
            rl.idAmministrazione = idAmministrazione;
            rl.tipoIE = "I";

            //rl.idRegistro = idRegistro;

            rl.systemId = idAmministrazione;
            rl.uo = new DocsPAWA.DocsPaWR.UnitaOrganizzativa();
            rl.uo.codiceRubrica = codiceAmministrazione;
            Session.Add("userRuolo", rl);

            DocsPAWA.DocsPaWR.Registro reg = new DocsPAWA.DocsPaWR.Registro();

            //reg = ws.GetRegistroBySistemId(idRegistro);

            Session.Add("userRegistro", reg);
        }



        private void FetchData()
        {
            string idGruppo = this._wsInstance.GetIdRuoloRespConservazione(this.IdAmministrazione.ToString());

            if (!string.IsNullOrEmpty(idGruppo))
            {
                Ruolo r = UserManager.getRuoloByIdGruppo(idGruppo, this.Page);
                if (r != null)
                {
                    this.txtCodRuolo.Text = r.codiceRubrica;
                    this.txtDescRuolo.Text = r.descrizione;
                    this.id_corr.Value = r.idGruppo;
                    this.UsersInRole = UserManager.GetUsersInRoleMinimalInfo(this.id_corr.Value);
                    this.PopolaDdlUtenti();
                    this.ddl_user.Enabled = true;
                    string idUtenteResp = this._wsInstance.GetIdUtenteRespConservazione(this.IdAmministrazione.ToString());
                    if (!string.IsNullOrEmpty(idUtenteResp))
                        this.ddl_user.SelectedValue = idUtenteResp;
                }
                else
                {
                    this.txtCodRuolo.Text = string.Empty;
                    this.txtDescRuolo.Text = string.Empty;
                    this.id_corr.Value = string.Empty;
                    this.ddl_user.Items.Clear();
                    this.ddl_user.Enabled = false;
                }
            }
            else
            {
                this.txtCodRuolo.Text = string.Empty;
                this.txtDescRuolo.Text = string.Empty;
                this.id_corr.Value = string.Empty;
                this.ddl_user.Items.Clear();
                this.ddl_user.Enabled = false;
            }
        }

        protected void btnSalvaRespCons_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(id_corr.Value))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_prole", "alert('Selezionare un ruolo');", true);
            }
            else
            {
                // controllo che il ruolo abbia almeno un utente attivo
                //UserMinimalInfo[] users = UserManager.GetUsersInRoleMinimalInfo(id_corr.Value);
                if (this.UsersInRole != null && this.UsersInRole.Length > 0)
                {
                    if (this._wsInstance.SaveRuoloRespConservazione(id_corr.Value, this.ddl_user.SelectedValue, IdAmministrazione.ToString()))
                    {
                        string msg = "Salvataggio ruolo responsabile avvenuto correttamente. \\n"; 

                        // controllo che il ruolo scelto sia abilitato alle seguenti operazioni:
                        // consolidamento documenti (file e metadati)
                        // operazioni di versamento e recupero
                        ArrayList functions = this.GetUnauthorizedFunctions(id_corr.Value);
                        if (functions != null && functions.Count > 0)
                        {
                            msg += "ATTENZIONE: per il corretto funzionamento del processo è necessario abilitare il ruolo all\\'utilizzo delle seguenti funzioni: \\n";
                            foreach (string f in functions)
                            {
                                msg += f + " \\n";
                            }
                        }

                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "role_saved", "alert('" + msg + "');", true);              
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "save_error", "alert('Si è verificato un errore nel salvataggio del ruolo');", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_prole", "alert('Il ruolo selezionato non ha utenti associati');", true);
                }

            }

            this.pnlRuoloResp.Update();
        }

        protected void btnRubricaRuoloResp_Click(object sender, EventArgs e)
        {
            // Se è stato selezionato un corrispondente, ne vengono visualizzate le informazioni
            if (Session["RuoloResponsabile"] != null && Session["RuoloResponsabile"] is ElementoRubrica[])
            {
                ElementoRubrica el = ((ElementoRubrica[])Session["RuoloResponsabile"])[0];

                // Recupero del dettaglio del corrispondente
                Ruolo corr = UserManager.getCorrispondenteByCodRubricaIE(
                    this.Page,
                    el.codice,
                    DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO) as Ruolo;

                this.txtCodRuolo.Text = corr.codiceRubrica;
                this.txtDescRuolo.Text = corr.descrizione;
                this.id_corr.Value = corr.idGruppo;

                this.UsersInRole = UserManager.GetUsersInRoleMinimalInfo(this.id_corr.Value);
                this.PopolaDdlUtenti();
                this.ddl_user.Enabled = true;

                Session.Remove("RuoloResponsabile");

                this.pnlRuoloResp.Update();
            }
        }

        protected void txtCodRuolo_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCodRuolo.Text))
            {
                this.SetDescCorr(txtCodRuolo.Text);
            }
            else
            {
                txtCodRuolo.Text = string.Empty;
                txtDescRuolo.Text = string.Empty;
                id_corr.Value = string.Empty;
                this.ddl_user.Items.Clear();
                this.ddl_user.Enabled = false;

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_prole", "alert('Inserire un codice da cercare in rubrica');", true);
            }

            this.pnlRuoloResp.Update();
        }

        private void SetDescCorr(string codRubrica)
        {
            //DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteByCodRubricaIE(this.Page, codRubrica, AddressbookTipoUtente.INTERNO);

            DocsPaWR.Corrispondente corr = null;

            ArrayList listaRuoli = ProfilazioneDocManager.getRuoliByAmm(IdAmministrazione.ToString(), codRubrica, "COD_RUOLO", this.Page);
            id_corr.Value = string.Empty;

            if(listaRuoli != null && listaRuoli.Count > 0)
            {
                if (listaRuoli.Count == 1)
                {
                    Ruolo r = (Ruolo)listaRuoli[0];

                    txtCodRuolo.Text = r.codice;
                    txtDescRuolo.Text = r.descrizione;
                    id_corr.Value = r.idGruppo;

                    this.UsersInRole = UserManager.GetUsersInRoleMinimalInfo(id_corr.Value);
                    this.PopolaDdlUtenti();
                    this.ddl_user.Enabled = true;
                }
                else
                {
                    foreach (Ruolo r in listaRuoli)
                    {
                        if (r.codice.ToUpper().Equals(codRubrica.ToUpper()))
                        {
                            txtCodRuolo.Text = r.codice;
                            txtDescRuolo.Text = r.descrizione;
                            id_corr.Value = r.idGruppo;

                            this.UsersInRole = UserManager.GetUsersInRoleMinimalInfo(id_corr.Value);
                            this.PopolaDdlUtenti();
                            this.ddl_user.Enabled = true;
                        }
                    }
                    if (string.IsNullOrEmpty(id_corr.Value))
                    {
                        txtCodRuolo.Text = string.Empty;
                        txtDescRuolo.Text = string.Empty;
                        id_corr.Value = string.Empty;
                        this.ddl_user.Items.Clear();
                        this.ddl_user.Enabled = false;
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_found", "alert('Corrispondente non trovato');", true);
                    }
                }
            }
            else
            {
                txtCodRuolo.Text = string.Empty;
                txtDescRuolo.Text = string.Empty;
                id_corr.Value = string.Empty;
                this.ddl_user.Items.Clear();
                this.ddl_user.Enabled = false;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_found", "alert('Corrispondente non trovato');", true);
            }

        }

        private ArrayList GetUnauthorizedFunctions(string idGroup)
        {

            ArrayList result = new ArrayList();

            // inserisco il ruolo in sessione
            Ruolo r = UserManager.getRuoloByIdGruppo(idGroup, this.Page);
            Session.Add("userRuolo", r);

            // verifico quali funzioni sono attive
            foreach (FunctionsToAuthorize f in Enum.GetValues(typeof(FunctionsToAuthorize)))
            {
                if (!UserManager.ruoloIsAutorized(this.Page, f.ToString()))
                    result.Add(f.ToString());
            }
            
            //if(UserManager.ruoloIsAutorized(this.Page, "DO_SACER_VERSAMENTO"))

            // pulisco la sessione
            Session.Remove("userRuolo");
            this.SaveUserInSession();

            return result;
        }

        protected void PopolaDdlUtenti()
        {
            if (this.UsersInRole != null)
            {
                this.ddl_user.Items.Clear();
                foreach (UserMinimalInfo user in this.UsersInRole)
                {
                    ListItem item = new ListItem();
                    item.Text = user.Description;
                    item.Value = user.SystemId;
                    this.ddl_user.Items.Add(item);
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

        protected UserMinimalInfo[] UsersInRole
        {
            get
            {
                if (CallContextStack.CurrentContext != null && CallContextStack.CurrentContext.ContextState["UsersInRoleRespCons"] != null)
                {
                    if (CallContextStack.CurrentContext == null)
                    {
                        CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                    }
                    return CallContextStack.CurrentContext.ContextState["UsersInRoleRespCons"] as UserMinimalInfo[];
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
                CallContextStack.CurrentContext.ContextState["UsersInRoleRespCons"] = value;
            }
        }
    }
}