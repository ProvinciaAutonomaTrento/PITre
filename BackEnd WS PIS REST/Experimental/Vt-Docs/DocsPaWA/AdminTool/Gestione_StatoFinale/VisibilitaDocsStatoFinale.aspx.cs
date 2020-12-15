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
using System.Configuration;
using System.Linq;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.popup
{
    public partial class VisibilitaDocsStatoFinale : System.Web.UI.Page
    {
        protected DocsPAWA.DocsPaWR.DocumentoDiritto[] list;
        
        protected DocsPAWA.dataSet.DS_Visibilit dS_Visibilit1;



        protected void Page_Load(object sender, EventArgs e)
        {
            
            try
            {
                if(!IsPostBack)
                {
                    ViewState["lista"] = null;
                    if (Request.QueryString["IdDoc"] != null)
                    {
                        string IdDoc = Request.QueryString["IdDoc"].ToString();

                        DocsPaWR.SchedaDocumento sd = DocumentManager.getDettaglioDocumento(this, IdDoc, null);
                        

                        bindGrid(IdDoc);
                        lblmodDoc.Text = "Selezionare i Ruoli per cui si desidera sbloccare il documento " + IdDoc + " e cliccare su Conferma";
                    }
                 }
            }

            catch (Exception ex)
            {
                ErrorManager.redirectToErrorPage(this, ex);


            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="diritto"></param>
        /// <returns></returns>
        protected bool IsUnLocked(DocsPaWR.DocumentoDiritto diritto)
        {


            return (diritto.accessRights == 0 || diritto.accessRights == 255 || diritto.accessRights > 45);

        }


        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            this.PreRender += new EventHandler(VisibilitaDocsStatoFinale_PreRender);

            base.OnInit(e);
        }

        protected void VisibilitaDocsStatoFinale_PreRender(object sender,EventArgs e)
        {
            if (Request.QueryString["IdDoc"] != null)
            {
                string IdDoc = Request.QueryString["IdDoc"].ToString();
                //26-02-2016: è stato deciso di rimuovere questo controllo
                /*
                DocsPaWR.SchedaDocumento sd = this.getDettaglioDocumento( IdDoc, null);
                if(sd.ConsolidationState.State!=DocumentConsolidationStateEnum.None)
                //if (sd.ConsolidationState.State.Equals("1") || sd.ConsolidationState.State.Equals("2"))
                {
                    foreach (DataGridItem item in dg_Visibilita.Items)
                    {

                        if (item.ItemType == ListItemType.Item ||
                            item.ItemType == ListItemType.AlternatingItem)
                        {



                            CheckBox cb_Abilita = (CheckBox)item.FindControl("cb_Abilita");
                            if (cb_Abilita.Checked)
                            {
                                cb_Abilita.Enabled = true;
                            }
                            else
                            {
                                cb_Abilita.Enabled = false;
                            }
                        }
                    }

                }
                 * */
            }
        }

        protected SchedaDocumento getDettaglioDocumento( string idProfile, string docNumber)
        {
            DocsPaWR.SchedaDocumento sd = new DocsPAWA.DocsPaWR.SchedaDocumento();

            try
            {
                if (idProfile == null && docNumber == null)
                {
                    sd = null;
                }
                else
                {
                    AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();
                    sd = ProxyManager.getWS().DocumentoGetDettaglioDocumentoNoSecurity(sessionManager.getUserAmmSession(), idProfile, docNumber);
                    if ((sd == null))// || (sd.inCestino != null && sd.inCestino == "1"))
                    {
                        string errorMessage = string.Empty;

                        if (sd == null)
                        {
                           
                                errorMessage = "Attenzione, non è stato possibile recuperare i dati del documento richiesto.\\nConsultare il log per maggiori dettagli.";
                                Page.Response.Write("<script>alert('" + errorMessage + "');</script>");
                                if (Page.Session["protocolloEsistente"] != null && (bool)Page.Session["protocolloEsistente"])
                                {
                                    Page.Session.Remove("protocolloEsistente");
                                }
                                else
                                {
                                    // Redirect alla homepage di docspa
                                    SiteNavigation.CallContextStack.Clear();
                                    SiteNavigation.NavigationContext.RefreshNavigation();
                                    string script = "<script>window.open('../GestioneRuolo.aspx','principale','width=410,height=300,scrollbars=NO');</script>";
                                    Page.Response.Write(script);
                                }
                        }
                      
                        
                       
                    }
                }
            }
            catch (Exception es)
            {
                return null;
            }

            return sd;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        public DocsPaWR.DocumentoDiritto[] GetVisibilitaDocumento(string idDocument)
        {
            AdminTool.Manager.SessionManager sessionManager = new AdminTool.Manager.SessionManager();

            list = ProxyManager.getWS().DocumentoGetVisibilita(sessionManager.getUserAmmSession(), idDocument, false);

            return list.Where(e => e.soggetto.GetType() == typeof(DocsPaWR.Ruolo)).OrderBy(e => e.soggetto.codiceRubrica).ToArray();
        }

        private void bindGrid(string IdDoc)
        {
            try
            {
                
                DocsPaWR.DocumentoDiritto[] docDir = this.GetVisibilitaDocumento(IdDoc);
                ViewState["lista"] = docDir;
                this.dg_Visibilita.VirtualItemCount = docDir.Length;
                this.dg_Visibilita.DataSource = docDir;
                this.dg_Visibilita.DataBind();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string GetTipoCorr(DocsPAWA.DocsPaWR.DocumentoDiritto docDirit)
        {
            try
            {
                string rtn = "";
                if (docDirit.soggetto.GetType() == typeof(DocsPAWA.DocsPaWR.Utente))
                    rtn = "UTENTE";
                else if (docDirit.soggetto.GetType() == typeof(DocsPAWA.DocsPaWR.Ruolo))
                    rtn = "RUOLO";
                else if (docDirit.soggetto.GetType() == typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa))
                    rtn = "U.O.";
                return rtn;
            }
            catch (Exception ex)
            {

                throw ex;

            }
        }

        protected void btn_conferma_Click(object sender, EventArgs e)
        {

           this.SaveChanges();



        }

        private void SaveChanges()
        {
            ArrayList arListModifiche = new ArrayList();
            //scorro gli items del datagrid 
            try
            {
                foreach (DataGridItem item in dg_Visibilita.Items)
                {

                    if (item.ItemType == ListItemType.Item ||
                        item.ItemType == ListItemType.AlternatingItem)
                    {

                        if (list == null)
                            list = (DocsPaWR.DocumentoDiritto[])ViewState["lista"];

                        Label lblIdRuolo = (Label)item.FindControl("lblIdRuolo");
                        CheckBox cb_Abilita = (CheckBox)item.FindControl("cb_Abilita");

                        //ricavo l'elemento corrispondente all'item corrente del datagrid dal datasource a cui avevo bindato il dataGrid
                        DocsPaWR.DocumentoDiritto[] docDirPrev = list.Where(d => d.personorgroup == lblIdRuolo.Text).ToArray();

                        // da questo elemento ricavo i diritti di accesso del ruolo corrente al documento al momento del binding
                        int previousRight = docDirPrev[0].accessRights;

                        // se il valore è stato cambiato...
                        if (cb_Abilita.Checked != (previousRight > 45))
                        {
                            //...inserisco l'elemento da aggiornare nella lista da dare in pasto all'update
                            DocsPaWR.ModificaAclDocumentoStatoFinale infoModifica = new DocsPaWR.ModificaAclDocumentoStatoFinale();
                            infoModifica.IdDocumento = Request.QueryString["IdDoc"].ToString();
                            infoModifica.IdRuolo = lblIdRuolo.Text;
                            if (cb_Abilita.Checked)
                                infoModifica.Azione = "SBLOCCA";
                            else infoModifica.Azione = "BLOCCA";



                            arListModifiche.Add(infoModifica);

                        }
                    }
                }




                if (arListModifiche.Count > 0)
                {
                    DocsPaWR.ModificaAclDocumentoStatoFinale[] infoModifiche = new DocsPaWR.ModificaAclDocumentoStatoFinale[arListModifiche.Count];
                    arListModifiche.ToArray().CopyTo(infoModifiche, 0);
                    this.UpdateRights(infoModifiche);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        private void UpdateRights(DocsPaWR.ModificaAclDocumentoStatoFinale[] infoModifiche)
        {
            try
            {
                bool success = false;
                if (infoModifiche.Length > 0)
                {
                    AdminTool.Manager.SessionManager sessionManager = new AdminTool.Manager.SessionManager();

                    success = ProxyManager.getWS().ModificaDocumentoStatoFinale(sessionManager.getUserAmmSession(), infoModifiche);
                }
                
                if (success)
                { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ok", "javascript:alert('Operazione effettuata con successo');Closewindow('true');", true); }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void dg_Visibilita_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {

            dg_Visibilita.CurrentPageIndex = e.NewPageIndex;

            if (Request.QueryString["IdDoc"] != null)
            {
                string IdDoc = Request.QueryString["IdDoc"].ToString();

                bindGrid(IdDoc);
            }

        }
    }
}