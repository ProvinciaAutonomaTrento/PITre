using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.DocsPaWR;
using DocsPAWA.SiteNavigation;

namespace DocsPAWA.AdminTool.UserControl
{
    public partial class RuoloResponsabile : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Salvataggio delle informazioni sul ruolo
            this.SaveUserInSession();

        }

        /// <summary>
        /// Metodo per la visualizzazione delle informazioni su uno specifico ruolo e per il caricamento del ruolo
        /// e degli utenti del ruolo
        /// </summary>
        /// <param name="roleId">Id gruppo del ruolo da analizzare</param>
        private void AnalyzeResult(String roleId)
        {
            bool showRoleDescription = true;
            try
            {
                // Se non si può scegliere l'utente, non vengono caricati gli utenti del ruolo
                if (this.ShowUserSelection)
                {
                    UserMinimalInfo[] users = UserManager.GetUsersInRoleMinimalInfo(roleId);

                    // Se il ruolo non ha utenti, non è possibile utilizzarlo come responsabile
                    if (users.Length == 0)
                    {
                        showRoleDescription = false;
                        //this.AjaxMessageBox1.ShowMessage("Il ruolo responsabile delle stampe repertori, deve contenere almeno un utente!");
                        this.AjaxMessageBox1.ShowMessage("Il ruolo responsabile, deve contenere almeno un utente!");
                        this.ddlUsers.Visible = false;
                    }
                    else
                    {
                        this.UserSystemId = users[0].SystemId;
                        this.ddlUsers.Visible = true;
                        this.ddlUsers.DataSource = users;
                        this.ddlUsers.DataBind();

                        // Selezione del primo utente disponibile
                        this.UserSystemId = this.ddlUsers.SelectedValue;
                    }

                }

                // Visualizzazione della descrizione del ruolo e memorizzazione dell'id gruppo del ruolo
                if(showRoleDescription)
                    this.txtRoleDescription.Text = UserManager.GetRoleDescriptionByIdGroup(roleId);
                

            }
            catch (Exception ex)
            {
                this.AjaxMessageBox1.ShowMessage("Si è verificato un errore durante il recupero delle informazioni sugli utenti del ruolo. ");

            }

        }

        /// <summary>
        /// Apertura della rubrica per la selezione di un ruolo interno all'amministrazione
        /// </summary>
        protected void btnRubricaRuoloResp_Click(object sender, ImageClickEventArgs e)
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

                this.RoleSystemId = corr.idGruppo;
                Session.Remove("RuoloResponsabile");
            }

        }

        /// <summary>
        /// Reset dei dati
        /// </summary>
        protected void imgDelRuoloResp_Click(object sender, ImageClickEventArgs e)
        {
            this.RoleSystemId = null;
            this.UserSystemId = null;
            this.txtRoleDescription.Text = String.Empty;
            this.ddlUsers.Items.Clear();
            this.ddlUsers.Visible = false;
        }

        /// <summary>
        /// Salvataggio dell'id del ruolo selezionato
        /// </summary>
        public void ddlUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.UserSystemId = this.ddlUsers.SelectedValue;

        }

        protected void ddlUsers_PreRender(object sender, EventArgs e)
        {
            
            try
            {
                if(!String.IsNullOrEmpty(this.UserSystemId))
                    this.ddlUsers.SelectedValue = this.UserSystemId;
                else
                {
                    this.UserSystemId = this.ddlUsers.SelectedValue;
                }
            }
            catch (Exception ex)
            {

            }

        }

        /// <summary>
        /// System id del ruolo
        /// </summary>
        public String RoleSystemId 
        {
            get
            {
                String roleId = CallContextStack.CurrentContext.ContextState["RoleSystemId" + this.ClientID] as String;
                return String.IsNullOrEmpty(roleId) ? String.Empty : roleId;
            }
            set
            {
                this.txtRoleDescription.Text = String.Empty;
                CallContextStack.CurrentContext.ContextState["RoleSystemId" + this.ClientID] = value;
                if (!String.IsNullOrEmpty(value) && value != "0")
                    this.AnalyzeResult(value);
            }

        }

       

        /// <summary>
        /// System di dell'utente
        /// </summary>
        public String UserSystemId 
        {
            get
            {
                String userId = CallContextStack.CurrentContext.ContextState["UserSystemId" + this.ClientID] as String;
                return String.IsNullOrEmpty(userId) ? String.Empty : userId;
            }
            set
            {
                CallContextStack.CurrentContext.ContextState["UserSystemId" + this.ClientID] = value;
            }
        }

        /// <summary>
        /// Proprietà per decidere se consentire la scelta dell'utente di un ruolo
        /// </summary>
        public bool ShowUserSelection { get; set; }

        /// <summary>
        /// Metodo per la pulizia del corrispondente trovato
        /// </summary>
        public void CleanSelectedCorr()
        {
            Session.Remove("RuoloResponsabile");
        }

        /// <summary>
        /// Metodo per il salvataggio delle informazioni sull'utente loggato DA CONTROLLARE
        /// </summary>
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

        


    }
}