using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DocsPAWA.utils;

namespace DocsPAWA.fascicolo
{
    /// <summary>
    /// Summary description for gestioneFasc.
    /// </summary>
    public class gestioneFasc : System.Web.UI.Page
    {
        protected DocsPaWebCtrlLibrary.IFrameWebControl iFrame_sx;
        protected DocsPaWebCtrlLibrary.IFrameWebControl iFrame_dx;
        public DocsPAWA.DocsPaWR.Fascicolo Fasc;

        private void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                Utils.startUp(this);
                string tab = "";
                string back = "";
                Response.Expires = 0;

                // Impostazione contesto corrente
                this.SetContext();

                // Se si proviene dalla nuova pagina di ricerca fascicoli...
                if (!String.IsNullOrEmpty(Request["from"]) && Request["from"].Equals("newRicFasc"))
                {
                    // viene prelevato l'id del fascicolo da caricare
                    string projectId = Request["idProject"];

                    // Salvataggio dell'id del fascicolo
                    SearchUtils.SetObjectId(projectId);

                    // Caricamento del fascicolo con id pari a idProject
                    FascicoliManager.setFascicoloSelezionato(
                        this, 
                        FascicoliManager.getFascicoloById(this, projectId));

                    // Redirezionamento del frame di destra alla pagina per la gestione del fascicolo
                    ClientScript.RegisterStartupScript(
                        this.GetType(), "PR", 
                        "top.principale.document.location ='" + Utils.getHttpFullPath() +
                            "/fascicolo/gestionefasc.aspx?tab=documenti';", true);

                    return;

                }
                    
                Fasc = FascicoliManager.getFascicoloSelezionato(this);

                // Inizializzazione controllo verifica acl
                if ((Fasc != null) && (Fasc.systemID != null))
                    this.InitializeControlAclFascicolo();


                if (Request.QueryString["tab"] != null)
                {
                    tab = Request.QueryString["tab"];
                }
                if (Request.QueryString["back"] != null)
                {
                    back = Request.QueryString["back"];
                }

                if (back != null && back != "")
                {
                    this.iFrame_sx.NavigateTo = "tabGestioneFasc.aspx?tab=" + tab + "&back=Y&docIndex=" + Request.QueryString["docIndex"];
                }
                else
                {
                    this.iFrame_sx.NavigateTo = "tabGestioneFasc.aspx?tab=" + tab;
                }

                //this.iFrame_dx.NavigateTo = "tabFascListaDoc.aspx";
                this.iFrame_dx.NavigateTo = "../waitingpage.htm";


            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }
        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);

        }
        #endregion

		/// <summary>
		/// Impostazione contesto corrente
		/// </summary>
        private void SetContext()
        {
            bool forceNewContext;
            bool.TryParse(Request.QueryString["forceNewContext"], out forceNewContext);
            SiteNavigation.CallContext context = SiteNavigation.CallContextStack.CurrentContext;

            if (!forceNewContext && context != null &&
                context.ContextName == SiteNavigation.NavigationKeys.FASCICOLO &&
                context.IsBack)
            {
                if (context.ContextState["systemId"] != null)
                {
                    string systemId = context.ContextState["systemId"].ToString();
                    DocsPaWR.Fascicolo fascicolo = FascicoliManager.getFascicolo(this, systemId);
                    FascicoliManager.setFascicoloSelezionato(fascicolo);
                }
            }
            else
            {
                string url = DocsPAWA.Utils.getHttpFullPath() + "/fascicolo/gestioneFasc.aspx";

                context = new SiteNavigation.CallContext(SiteNavigation.NavigationKeys.FASCICOLO, url);
                context.ContextFrameName = "top.principale";
                context.ContextDisposed += new EventHandler(context_ContextDisposed);

                if (SiteNavigation.CallContextStack.SetCurrentContext(context,forceNewContext))
                    SiteNavigation.NavigationContext.RefreshNavigation();
            }
        }

        #region Gestione controllo acl fascicolo


        /// <summary>
        /// Inizializzazione controllo verifica acl
        /// </summary>
        protected virtual void InitializeControlAclFascicolo()
        {
            AclFascicolo ctl = this.GetControlAclFascicolo();
            ctl.IdFascicolo = FascicoliManager.getFascicoloSelezionato().systemID;
            ctl.OnAclRevocata += new EventHandler(this.OnAclFascicoloRevocata);
        }

        /// <summary>
        /// Reperimento controllo acldocumento
        /// </summary>
        /// <returns></returns>
        protected AclFascicolo GetControlAclFascicolo()
        {
            return (AclFascicolo)this.FindControl("aclFascicolo");
        }

        /// <summary>
        /// Listener evento OnAclDocumentoRevocata
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAclFascicoloRevocata(object sender, EventArgs e)
        {
            // Redirect alla homepage di docspa
            SiteNavigation.CallContextStack.Clear();
            SiteNavigation.NavigationContext.RefreshNavigation();
            string script = "<script>window.open('../GestioneRuolo.aspx','principale','width=410,height=300,scrollbars=NO');</script>";
            Response.Write(script);
        }

        #endregion

        private static void context_ContextDisposed(object sender, EventArgs e)
        {
            FascicoliManager.removeFascicoloSelezionato();
            FascicoliManager.removeMemoriaFascicoloSelezionato();

            FascicoliManager.removeFolderSelezionato();
            FascicoliManager.removeMemoriaFolderSelezionata();
        }

        protected void btn_inserisciDoc_Click(object sender, ImageClickEventArgs e)
        {
            if (!this.GetControlAclFascicolo().AclRevocata)
            {
                //DocsPaWR.Folder selectedFolder=getSelectedFolder();

                DocsPaWR.Folder selectedFolder = FascicoliManager.getFolderSelezionato(this);
                string idFolder = "";
                string scriptName = "";
                string script = "";

                if (selectedFolder != null && selectedFolder.systemID != "")
                {
                    idFolder = selectedFolder.systemID;

                    string tipoDoc = "tipoDoc=T";
                    string action = "action=addDocToFolder";
                    string parameter = "";
                    parameter = "folderId=" + idFolder;

                    /* massimo digregorio 
                    descrizione: visualizzazione dei DOCUMENTI in ADL filtratri per Registro del FASCICOLO. 
                    new:*/
                    string paramIdReg = String.Empty;
                    if (Fasc.idRegistroNodoTit != null && !Fasc.idRegistroNodoTit.Equals(String.Empty))
                        paramIdReg = "&idReg=" + Fasc.idRegistroNodoTit;

                    string queryString = tipoDoc + "&" + action + "&" + parameter + paramIdReg;


                    script = "<script>ApriFinestraRicercaDocPerClassifica('" + queryString + "');</script>";
                    //script="<script>ApriFinestraADL('../popup/areaDiLavoro.aspx?"+queryString+"');</script>";
                    //scriptName="addFromADL";
                    scriptName = "addRicPerClass";
                }
                else
                {
                    script = "<script>alert('Selezionare un Folder');</script>";
                    scriptName = "SelectFolderAlert";
                }

                this.RegisterStartupScript(scriptName, script);
                Session["ListaDocs-CampiProf"] = "ListaDocs";
                //Page.RegisterClientScriptBlock(scriptName,script);
            }
        }

        //protected void btn_visualizzaDoc_Click(object sender, ImageClickEventArgs e)
        //{

        //    if (Session["IdFolderselezionato"] != null)
        //    {
        //        Session["ListaDocs-CampiProf"] = "ListaDocs";
        //        string newUrl = "tabFascListaDoc.aspx?idFolder=" + Session["IdFolderselezionato"].ToString();

        //        this.iFrame_dx.NavigateTo = newUrl;

        //        //Response.Write("<script>parent.parent.iFrame_dx.location='" + newUrl + "'</script>");

        //    }
        //}

        //protected void btnFilterDocs_Click(object sender, ImageClickEventArgs e)
        //{
        //    if (Session["IdFolderselezionato"] != null)
        //    {
        //       // Session["ListaDocs-CampiProf"] = "ListaDocs";
        //        string newUrl = "tabFascListaDoc.aspx?idFolder=" + Session["IdFolderselezionato"].ToString() + "&Filtra=True";

        //        this.iFrame_dx.NavigateTo = newUrl;

        //        //Response.Write("<script>parent.parent.iFrame_dx.location='" + newUrl + "'</script>");

        //    }
        //}

        //protected void btnShowAllDocs_Click(object sender, ImageClickEventArgs e)
        //{
        //    if (Session["IdFolderselezionato"] != null)
        //    {
        //        // Session["ListaDocs-CampiProf"] = "ListaDocs";
        //        string newUrl = "tabFascListaDoc.aspx?idFolder=" + Session["IdFolderselezionato"].ToString() + "&Filtra=False";

        //        this.iFrame_dx.NavigateTo = newUrl;

        //        //Response.Write("<script>parent.parent.iFrame_dx.location='" + newUrl + "'</script>");

        //    }
        //}
    }
}

