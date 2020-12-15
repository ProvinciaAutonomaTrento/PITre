using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace SAAdminTool.AdminTool.Gestione_VociMenu
{
    public partial class Testata : System.Web.UI.UserControl
    {
        SAAdminTool.DocsPaWR.InfoUtenteAmministratore _datiAmministratore = null;
        const string tastoAbilitato = "../Images/tasto_a.gif";
        const string tastoDefault = "../Images/tasto.jpg";
        const string tastoDisabilitato = "../Images/tasto_d.gif";
        protected string fromAdmin;
        private string idAmm;
        //protected System.Web.UI.WebControls.Image logoAmm;
        // protected System.Web.UI.WebControls.Label lbl_help;



        protected void Page_Load(object sender, EventArgs e)
        {
            idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
            if (!IsPostBack)
            {
                Inizialize();
            }

            if (fileExist("logoAmm.gif", "LoginFE"))
            {
                this.logoAmm.ImageUrl = "~/images/loghiAmministrazioni/logoAmm.gif";
            }

            this.fromAdmin = (string)Session["AdminBookmark"];
            if (this.fromAdmin == "" || this.fromAdmin == null)
                this.fromAdmin = "Home";

            this.lbl_help.Attributes.Add("onClick", "OpenHelp('" + this.fromAdmin + "')");
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            this.RefreshControlsEnabled();

            if (System.Configuration.ConfigurationManager.AppSettings["CONSERVAZIONE"] != null && !System.Configuration.ConfigurationManager.AppSettings["CONSERVAZIONE"].ToString().Equals("1"))
                this.td_conservazione.Visible = false;

            // INTEGRAZIONE PITRE-PARER
            // se è attiva la conservazione PARER il menu conservazione deve essere nascosto
            this.td_conservazione.Visible = this.DisplayMenuConservazione();
        }

        private void Inizialize()
        {
            this.setUserSession();
            string tipoAmministratore = this.getTipoAmministratore();

            if (tipoAmministratore.Equals("3"))
            {
                setGUIMenuIniziale();
                setGUIMenuUserAdmin();
            }
            else
                setGUIMenu();


        }

        private bool fileExist(string fileName, string type)
        {
            return FileManager.fileExist(fileName, type);
        }

        private void setUserSession()
        {
            this._datiAmministratore = new SAAdminTool.DocsPaWR.InfoUtenteAmministratore();

            SAAdminTool.AdminTool.Manager.SessionManager sessionMng = new SAAdminTool.AdminTool.Manager.SessionManager();
            this._datiAmministratore = sessionMng.getUserAmmSession();
        }

        private string getTipoAmministratore()
        {
            string retValue = string.Empty;
            retValue = this._datiAmministratore.tipoAmministratore;
            return retValue;
        }

        private SAAdminTool.DocsPaWR.Menu[] getVociMenuUserAdmin()
        {
            SAAdminTool.DocsPaWR.Menu[] lista = this._datiAmministratore.VociMenu;
            return lista;
        }

        private void setGUIMenuUserAdmin()
        {
            SAAdminTool.DocsPaWR.Menu[] vociMenu = this.getVociMenuUserAdmin();

            foreach (SAAdminTool.DocsPaWR.Menu item in vociMenu)
            {
                switch (item.Codice)
                {
                    case "Home": //---------------------------------------------------------------------------
                        if (item.Associato.Equals(string.Empty))
                        {
                            this.td_home.Attributes.Add("background", tastoDisabilitato);
                            this.Hyperlink_home.Enabled = false;
                        }
                        else
                        {
                            if (Request.QueryString["from"] == "HP" || Request.QueryString["from"] == null)
                            {
                                this.td_home.Attributes.Add("background", tastoAbilitato);
                                this.Hyperlink_home.Enabled = false;
                            }
                            else
                            {
                                this.td_home.Attributes.Add("background", tastoDefault);
                                this.Hyperlink_home.Enabled = true;
                            }
                        }
                        break;

                    case "Tipi ruolo": //---------------------------------------------------------------------------
                        if (item.Associato.Equals(string.Empty))
                        {
                            this.td_ruoli.Attributes.Add("background", tastoDisabilitato);
                            this.Hyperlink_ruoli.Enabled = false;
                        }
                        else
                        {
                            if (Request.QueryString["from"] == "RU")
                            {
                                this.td_ruoli.Attributes.Add("background", tastoAbilitato);
                                this.Hyperlink_ruoli.Enabled = false;
                            }
                            else
                            {
                                this.td_ruoli.Attributes.Add("background", tastoDefault);
                                this.Hyperlink_ruoli.Enabled = true;
                            }
                        }
                        break;

                    case "Utenti": //---------------------------------------------------------------------------
                        if (item.Associato.Equals(string.Empty))
                        {
                            this.td_utenti.Attributes.Add("background", tastoDisabilitato);
                            this.Hyperlink_utenti.Enabled = false;
                        }
                        else
                        {
                            if (Request.QueryString["from"] == "UT")
                            {
                                this.td_utenti.Attributes.Add("background", tastoAbilitato);
                                this.Hyperlink_utenti.Enabled = false;
                            }
                            else
                            {
                                this.td_utenti.Attributes.Add("background", tastoDefault);
                                this.Hyperlink_utenti.Enabled = true;
                            }
                        }
                        break;

                    case "Registri": //---------------------------------------------------------------------------
                        if (item.Associato.Equals(string.Empty))
                        {
                            this.td_registri.Attributes.Add("background", tastoDisabilitato);
                            this.Hyperlink_registri.Enabled = false;
                        }
                        else
                        {
                            if (Request.QueryString["from"] == "RG")
                            {
                                this.td_registri.Attributes.Add("background", tastoAbilitato);
                                this.Hyperlink_registri.Enabled = false;
                            }
                            else
                            {
                                this.td_registri.Attributes.Add("background", tastoDefault);
                                this.Hyperlink_registri.Enabled = true;
                            }
                        }
                        break;

                    case "Funzioni": //---------------------------------------------------------------------------
                        if (item.Associato.Equals(string.Empty))
                        {
                            this.td_funzioni.Attributes.Add("background", tastoDisabilitato);
                            this.Hyperlink_funzioni.Enabled = false;
                        }
                        else
                        {
                            if (Request.QueryString["from"] == "FU")
                            {
                                this.td_funzioni.Attributes.Add("background", tastoAbilitato);
                                this.Hyperlink_funzioni.Enabled = false;
                            }
                            else
                            {
                                this.td_funzioni.Attributes.Add("background", tastoDefault);
                                this.Hyperlink_funzioni.Enabled = true;
                            }
                        }
                        break;

                    case "Rag.Trasm.": //---------------------------------------------------------------------------
                        if (item.Associato.Equals(string.Empty))
                        {
                            this.td_ragioni.Attributes.Add("background", tastoDisabilitato);
                            this.Hyperlink_ragTrasm.Enabled = false;
                        }
                        else
                        {
                            if (Request.QueryString["from"] == "RT")
                            {
                                this.td_ragioni.Attributes.Add("background", tastoAbilitato);
                                this.Hyperlink_ragTrasm.Enabled = false;
                            }
                            else
                            {
                                this.td_ragioni.Attributes.Add("background", tastoDefault);
                                this.Hyperlink_ragTrasm.Enabled = true;
                            }
                        }
                        break;

                    case "Organigramma": //---------------------------------------------------------------------------
                        if (item.Associato.Equals(string.Empty))
                        {
                            this.td_organigramma.Attributes.Add("background", tastoDisabilitato);
                            this.Hyperlink_organigramma.Enabled = false;
                        }
                        else
                        {
                            if (Request.QueryString["from"] == "OR")
                            {
                                this.td_organigramma.Attributes.Add("background", tastoAbilitato);
                                this.Hyperlink_organigramma.Enabled = false;
                            }
                            else
                            {
                                this.td_organigramma.Attributes.Add("background", tastoDefault);
                                this.Hyperlink_organigramma.Enabled = true;
                            }
                        }
                        break;

                    case "Titolario": //---------------------------------------------------------------------------
                        if (item.Associato.Equals(string.Empty))
                        {
                            this.td_titolario.Attributes.Add("background", tastoDisabilitato);
                            this.Hyperlink_titolario.Enabled = false;
                        }
                        else
                        {
                            if (Request.QueryString["from"] == "TI")
                            {
                                this.td_titolario.Attributes.Add("background", tastoAbilitato);
                                this.Hyperlink_titolario.Enabled = false;
                            }
                            else
                            {
                                this.td_titolario.Attributes.Add("background", tastoDefault);
                                this.Hyperlink_titolario.Enabled = true;
                            }
                        }
                        break;
                    case "FE_ABILITA_POLICY_CONSERVAZIONE": //---------------------------------------------------------------------------
                        if (item.Associato.Equals(string.Empty))
                        {
                            this.td_conservazione.Attributes.Add("background", tastoDisabilitato);
                            this.Hyperlink_conservazione.Enabled = false;
                        }
                        else
                        {
                            if (Request.QueryString["from"] == "CON")
                            {
                                this.td_conservazione.Attributes.Add("background", tastoAbilitato);
                                this.Hyperlink_conservazione.Enabled = false;
                            }
                            else
                            {
                                this.td_conservazione.Attributes.Add("background", tastoDefault);
                                this.Hyperlink_conservazione.Enabled = true;
                            }
                        }
                        break;
                    case "Pubblicazioni": //---------------------------------------------------------------------------
                        if (item.Associato.Equals(string.Empty))
                        {
                            this.td_pubblicazioni.Attributes.Add("background", tastoDisabilitato);
                            this.Hyperlink_pubblicazioni.Enabled = false;
                        }
                        else
                        {
                            if (Request.QueryString["from"] == "TI")
                            {
                                this.td_pubblicazioni.Attributes.Add("background", tastoAbilitato);
                                this.Hyperlink_pubblicazioni.Enabled = false;
                            }
                            else
                            {
                                this.td_pubblicazioni.Attributes.Add("background", tastoDefault);
                                this.Hyperlink_pubblicazioni.Enabled = true;
                            }
                        }
                        break;
                }
            }
        }

        private void setGUIMenu()
        {
            //- case "Home": //---------------------------------------------------------------------------
            if (Request.QueryString["from"] == "HP" || Request.QueryString["from"] == null)
            {
                this.td_home.Attributes.Add("background", tastoAbilitato);
                this.td_home.Attributes.Add("class", "testo_bianco");
                this.Hyperlink_home.Enabled = false;
            }
            else
            {
                this.td_home.Attributes.Add("background", tastoDefault);
                if (Session["AMMDATASET"] != null)
                    this.Hyperlink_home.Enabled = true;
                else
                    this.Hyperlink_home.Enabled = false;
            }

            //- case "Tipi ruolo": //---------------------------------------------------------------------------
            if (Request.QueryString["from"] == "RU")
            {
                this.td_ruoli.Attributes.Add("background", tastoAbilitato);
                this.Hyperlink_ruoli.Enabled = false;
            }
            else
            {
                this.td_ruoli.Attributes.Add("background", tastoDefault);
                if (Session["AMMDATASET"] != null)
                    this.Hyperlink_ruoli.Enabled = true;
                else
                    this.Hyperlink_ruoli.Enabled = false;
            }

            //- case "Utenti": //---------------------------------------------------------------------------
            if (Request.QueryString["from"] == "UT")
            {
                this.td_utenti.Attributes.Add("background", tastoAbilitato);
                this.Hyperlink_utenti.Enabled = false;
            }
            else
            {
                this.td_utenti.Attributes.Add("background", tastoDefault);
                if (Session["AMMDATASET"] != null)
                    this.Hyperlink_utenti.Enabled = true;
                else
                    this.Hyperlink_utenti.Enabled = false;
            }

            //- case "Registri": //---------------------------------------------------------------------------
            if (Request.QueryString["from"] == "RG")
            {
                this.td_registri.Attributes.Add("background", tastoAbilitato);
                this.Hyperlink_registri.Enabled = false;
            }
            else
            {
                this.td_registri.Attributes.Add("background", tastoDefault);
                if (Session["AMMDATASET"] != null)
                    this.Hyperlink_registri.Enabled = true;
                else
                    this.Hyperlink_registri.Enabled = false;
            }

            //- case "Funzioni": //---------------------------------------------------------------------------
            if (Request.QueryString["from"] == "FU")
            {
                this.td_funzioni.Attributes.Add("background", tastoAbilitato);
                this.Hyperlink_funzioni.Enabled = false;
            }
            else
            {
                this.td_funzioni.Attributes.Add("background", tastoDefault);
                if (Session["AMMDATASET"] != null)
                    this.Hyperlink_funzioni.Enabled = true;
                else
                    this.Hyperlink_funzioni.Enabled = false;
            }


            //- case "Rag.Trasm.": //---------------------------------------------------------------------------
            if (Request.QueryString["from"] == "RT")
            {
                this.td_ragioni.Attributes.Add("background", tastoAbilitato);
                this.Hyperlink_ragTrasm.Enabled = false;
            }
            else
            {
                this.td_ragioni.Attributes.Add("background", tastoDefault);
                if (Session["AMMDATASET"] != null)
                    this.Hyperlink_ragTrasm.Enabled = true;
                else
                    this.Hyperlink_ragTrasm.Enabled = false;
            }


            //- case "Organigramma": //---------------------------------------------------------------------------

            if (Request.QueryString["from"] == "OR")
            {
                this.td_organigramma.Attributes.Add("background", tastoAbilitato);
                this.Hyperlink_organigramma.Enabled = false;
            }
            else
            {
                this.td_organigramma.Attributes.Add("background", tastoDefault);
                if (Session["AMMDATASET"] != null)
                    this.Hyperlink_organigramma.Enabled = true;
                else
                    this.Hyperlink_organigramma.Enabled = false;
            }

            //- case "Titolario": //---------------------------------------------------------------------------

            if (Request.QueryString["from"] == "TI")
            {
                this.td_titolario.Attributes.Add("background", tastoAbilitato);
                this.Hyperlink_titolario.Enabled = false;
            }
            else
            {
                this.td_titolario.Attributes.Add("background", tastoDefault);
                if (Session["AMMDATASET"] != null)
                    this.Hyperlink_titolario.Enabled = true;
                else
                    this.Hyperlink_titolario.Enabled = false;
            }

            //- case "Conservazione": //---------------------------------------------------------------------------

            if (Request.QueryString["from"] == "CON")
            {
                this.td_conservazione.Attributes.Add("background", tastoAbilitato);
                this.Hyperlink_conservazione.Enabled = false;
            }
            else
            {
                this.td_organigramma.Attributes.Add("background", tastoDefault);
                if (Session["AMMDATASET"] != null)
                    this.Hyperlink_conservazione.Enabled = true;
                else
                    this.Hyperlink_conservazione.Enabled = false;
            }

            //- case "Pubblicazioni": //---------------------------------------------------------------------------

            if (Request.QueryString["from"] == "PU")
            {
                this.td_pubblicazioni.Attributes.Add("background", tastoAbilitato);
                this.Hyperlink_pubblicazioni.Enabled = false;
            }
            else
            {
                this.td_pubblicazioni.Attributes.Add("background", tastoDefault);
                if (Session["AMMDATASET"] != null)
                    this.Hyperlink_pubblicazioni.Enabled = true;
                else
                    this.Hyperlink_pubblicazioni.Enabled = false;
            }
        }

        private void setGUIMenuIniziale()
        {
            //--case "Home": //---------------------------------------------------------------------------
            this.td_home.Attributes.Add("background", tastoDisabilitato);
            this.Hyperlink_home.Enabled = false;
            //--case "Tipi ruolo": //---------------------------------------------------------------------------
            this.td_ruoli.Attributes.Add("background", tastoDisabilitato);
            this.Hyperlink_ruoli.Enabled = false;

            //--case "Utenti": //---------------------------------------------------------------------------
            this.td_utenti.Attributes.Add("background", tastoDisabilitato);
            this.Hyperlink_utenti.Enabled = false;

            //--case "Registri": //---------------------------------------------------------------------------
            this.td_registri.Attributes.Add("background", tastoDisabilitato);
            this.Hyperlink_registri.Enabled = false;
            //--case "Funzioni": //---------------------------------------------------------------------------
            this.td_funzioni.Attributes.Add("background", tastoDisabilitato);
            this.Hyperlink_funzioni.Enabled = false;

            //--case "Rag.Trasm.": //---------------------------------------------------------------------------
            this.td_ragioni.Attributes.Add("background", tastoDisabilitato);
            this.Hyperlink_ragTrasm.Enabled = false;

            //--case "Organigramma": //---------------------------------------------------------------------------
            this.td_organigramma.Attributes.Add("background", tastoDisabilitato);
            this.Hyperlink_organigramma.Enabled = false;

            //--case "Titolario": //---------------------------------------------------------------------------
            this.td_titolario.Attributes.Add("background", tastoDisabilitato);
            this.Hyperlink_titolario.Enabled = false;

            //--case "Conservazione": //---------------------------------------------------------------------------
            this.td_conservazione.Attributes.Add("background", tastoDisabilitato);
            this.Hyperlink_conservazione.Enabled = false;

            //--case "Pubblicazioni": //---------------------------------------------------------------------------
            this.td_pubblicazioni.Attributes.Add("background", tastoDisabilitato);
            this.Hyperlink_pubblicazioni.Enabled = false;

        }

        /// <summary>
        /// 
        /// </summary>
        private void RefreshControlsEnabled()
        {
            this.td_pubblicazioni.Visible = Gestione_Pubblicazioni.Configurations.PublisherEnabled;
        }

        private bool DisplayMenuConservazione()
        {
            bool result = true;

            string FE_WA_CONSERVAZIONE = string.Empty;
            FE_WA_CONSERVAZIONE = SAAdminTool.utils.InitConfigurationKeys.GetValue("0", "FE_WA_CONSERVAZIONE");
            if (!string.IsNullOrEmpty(FE_WA_CONSERVAZIONE) && FE_WA_CONSERVAZIONE.Equals("1"))
                result = false;
            else
                result = true;

            return result;


        }
    }
}
