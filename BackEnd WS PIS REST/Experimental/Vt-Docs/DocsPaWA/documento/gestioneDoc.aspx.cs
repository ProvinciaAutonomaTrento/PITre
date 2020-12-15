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

namespace DocsPAWA
{
	/// <summary>
	/// Summary description for gestioneDoc.
	/// </summary>
	public class gestioneDoc : System.Web.UI.Page
	{
		protected DocsPaWebCtrlLibrary.IFrameWebControl iFrame_sx;
		protected DocsPaWebCtrlLibrary.IFrameWebControl iFrame_dx;

        private void Page_Load(object sender, System.EventArgs e)
        {
            //controlla se session Id è not null
            Utils.startUp(this);
            try
            {
                Response.Expires = 0;
                string tab = Request.QueryString["tab"];
                string isNew = Request.QueryString["isNew"];
                string daCestino = Request.QueryString["daCestino"];
                //se arrivo dalla popup rimozione documento il javascript su librerie 
                // ApriFinestraRimuoviProfilo setta questo parametro a 'Y' che viene utilizzato nel metodo setContext
                
                if (isNew == null) isNew = "0";

                if (tab.Equals("protocollo"))
                    Session["Bookmark"] = "NuovoProt";
                if (tab.Equals("profilo"))
                    Session["Bookmark"] = "NuovoDoc";


                // Impostazione contesto corrente
                this.SetContext(ref tab, isNew);

                //inserito per schianto nuovo protocollo dopo ricerca
                //DocumentManager.removeDocumentoSelezionato(this);
                DocsPaWR.InfoDocumento infoDoc = DocumentManager.getRisultatoRicerca(this);

                // Se il query string contiene from valorizzato come newRicDoc, viene creato un
                // nuovo info documento con id profile pari al valore assunto da idProfile del querystring
                // e tipoProto con il valore assunto da protoType del querystring e si redireziona la pagina
                // di sinistra alla pagina documento/gestionedoc.aspx?tab=protocollo
                if (!String.IsNullOrEmpty(Request["from"]) &&
                    Request["from"].Equals("newRicDoc"))
                {
                    DocumentManager.setRisultatoRicerca(this, new DocsPaWR.InfoDocumento()
                    {
                        idProfile = Request["idProfile"],
                        tipoProto = Request["protoType"]
                    });

                    // Salvataggio dell'id del documento selezionato. Questa informazione viene utilizzata
                    // quando si torna sulla pagina dei risultati attraverso il pulsante "Indietro"
                    SearchUtils.SetObjectId(Request["idProfile"]);

 //                   ClientScript.RegisterStartupScript(this.GetType(), "PR", "top.principale.document.location ='" + Utils.getHttpFullPath() + "/documento/gestionedoc.aspx?tab=profilo';", true);
                    ClientScript.RegisterStartupScript(this.GetType(), "PR", "top.principale.document.location ='../documento/gestionedoc.aspx?tab=profilo';", true);
 
                    return;

                }



                if (infoDoc != null && (infoDoc.idProfile != null || infoDoc.docNumber != null))
                {
                    if (daCestino != null && daCestino.Equals("1"))
                        DocumentManager.setDocumentoSelezionato(this, DocumentManager.getDettaglioDocumentoDaCestino(this, infoDoc.idProfile, infoDoc.docNumber));
                    else
                    {
                        DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDettaglioDocumento(this, infoDoc.idProfile, infoDoc.docNumber);
                        if (infoDoc.inArchivio != "0")
                        {
                            schedaDoc.inArchivio = infoDoc.inArchivio;
                        }
                        DocumentManager.setDocumentoSelezionato(this, schedaDoc);
                        //DocumentManager.setDocumentoSelezionato(this, DocumentManager.getDettaglioDocumento(this, infoDoc.idProfile, infoDoc.docNumber));
                    }
                    // Problemi lo pulisce il tasto del menu:
                    DocumentManager.removeRisultatoRicerca(this);
					DocumentManager.removeFiltroRicDoc(this);
					if ((infoDoc.tipoProto.Equals("G"))
                        //modifica del 15/05/2009 
                        ||(infoDoc.tipoProto.Equals("R"))
                        || (infoDoc.tipoProto.Equals("C"))
                        //fine modifica del 15/05/2009
                        )

						tab = "Profilo";
					else
						tab = "Protocollo";
				}
				this.iFrame_sx.NavigateTo = "tabGestioneDoc.aspx?tab=" + tab+"&isNew="+isNew;


				if(!tab.Equals("trasmissioni"))
                {					
					this.iFrame_dx.NavigateTo = "tabDoc.aspx";				
				}
                else
                    this.iFrame_dx.NavigateTo = "tabTrasmissioniEff.aspx";

            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        /// <summary>
        /// Impostazione contesto corrente
        /// </summary>
        /// <param name="tabName"></param>
        /// <param name="isNew"></param>
        private void SetContext(ref string tabName, string isNew)
        {
            string url = DocsPAWA.Utils.getHttpFullPath() + "/documento/GestioneDoc.aspx";
            if (tabName != string.Empty)
                url += "?tab=" + tabName;
            if (isNew != string.Empty)
                url += "&isNew=" + isNew;

            // Verifica il parametro da querystring "forceNewContext" che,
            // se fornito, richiede di forzare la creazione di un nuovo callcontext
            bool forceNewContext;
            bool.TryParse(Request.QueryString["forceNewContext"], out forceNewContext);

            SiteNavigation.CallContext context = SiteNavigation.CallContextStack.CurrentContext;

            if (!forceNewContext && isNew == "0" &&
                context != null && 
                (context.ContextName == SiteNavigation.NavigationKeys.DOCUMENTO ||
                 context.ContextName == SiteNavigation.NavigationKeys.ALLEGATO) &&
                context.IsBack)
            {
                // Se il contesto corrente è il documento (ovvero in un contesto di back),
                // vengono reperiti i valori di "idProfile" e "docNumber" per
                // ripristinare il documento corrente
                if (context.ContextState["idProfile"] != null)
                {
                    string idProfile = context.ContextState["idProfile"].ToString();
                    string docNumber = context.ContextState["docNumber"].ToString();

                    // Reperimento scheda documento dal database e impostazione in sessione
                    DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDettaglioDocumento(this, idProfile, docNumber);
                    DocumentManager.setDocumentoSelezionato(schedaDoc);
                    DocumentManager.setDocumentoInLavorazione(schedaDoc);

                    if (tabName.ToLower() == "protocollo" &&
                        schedaDoc != null && schedaDoc.protocollo == null)
                    {
                        tabName = "profilo";
                        context.QueryStringParameters["tab"] = tabName;
                    }
                }
            }
            else
            {
                bool allegato;
                bool.TryParse(this.Request.QueryString["allegato"], out allegato);

                string contextName = string.Empty;

                if (allegato)
                    contextName = SiteNavigation.NavigationKeys.ALLEGATO;
                else
                    contextName = SiteNavigation.NavigationKeys.DOCUMENTO;

                // Creazione di un nuovo contesto
                context = new SiteNavigation.CallContext(contextName, url);

                context.RestoreContextState += new DocsPAWA.SiteNavigation.RestoreContextDelegate(OnRestoreContextState);
                context.ContextFrameName = "top.principale";

                if (SiteNavigation.CallContextStack.SetCurrentContext(context, forceNewContext))
                    SiteNavigation.NavigationContext.RefreshNavigation();
            }
        }

        /// <summary>
        /// Handler evento "RestoreContextState" per il ripristino
        /// manuale della scheda documento mediante il metodo "DocumentManager.setDocumentoSelezionato"
        /// </summary>
        /// <param name="e"></param>
        private static void OnRestoreContextState(SiteNavigation.RestoreContextEventArgs e)
        {
            if (e.SessionState.Contains("gestioneDoc.schedaDocumento"))
            {
                DocsPaWR.SchedaDocumento schedaDoc = (DocsPAWA.DocsPaWR.SchedaDocumento)e.SessionState["gestioneDoc.schedaDocumento"];

                DocumentManager.setDocumentoSelezionato(schedaDoc);
                DocumentManager.setDocumentoInLavorazione(schedaDoc);
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
    }
}
