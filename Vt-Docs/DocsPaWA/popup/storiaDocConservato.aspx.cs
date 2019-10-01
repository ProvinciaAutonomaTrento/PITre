using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using DocsPAWA.dataSet;
using System.Collections.Generic;

namespace DocsPAWA.popup
{
    public partial class storiaDocConservato : DocsPAWA.CssPage
    {
        protected DocsPAWA.dataSet.DataSetRDettItemsCons dataSetItemsCons = new DataSetRDettItemsCons();
        protected DocsPAWA.DocsPaWR.DettItemsConservazione[] dettItemsCons;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;

            if (!Page.IsPostBack)
            {
                if (Request.QueryString["idProfile"] != null && Request.QueryString["idProfile"] != string.Empty)
                {
                    string idProfile = Request.QueryString["idProfile"].ToString();
                    Session["idProfile"] = idProfile;
                    dettItemsCons = DocumentManager.getStoriaConsDoc(this, idProfile);
                    this.caricaGridView(dettItemsCons, this.gv_dettItemsCons);
                }
                if (Request.QueryString["idProject"] != null && Request.QueryString["idProject"] != string.Empty)
                {
                    string idProject = Request.QueryString["idProject"].ToString();
                    Session["idProject"] = idProject;
                    dettItemsCons = DocumentManager.getStoriaConsFasc(this, idProject);
                    this.caricaGridView(dettItemsCons, this.gv_dettItemsCons);
                    this.lbl_intestazione.Text = "Storia conservazione fascicolo";
                }
            }
        }

        protected void gv_dettItemsCons_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.gv_dettItemsCons.PageIndex = e.NewPageIndex;
            dettItemsCons = DocumentManager.getStoriaConsDoc(this, (string)Session["idProfile"]);
            this.caricaGridView(dettItemsCons, this.gv_dettItemsCons);
        }

        protected void caricaGridView(DocsPAWA.DocsPaWR.DettItemsConservazione[] dettItemsCons, GridView gv)
        {
            this.dataSetItemsCons = new DocsPAWA.dataSet.DataSetRDettItemsCons();
            for (int i = 0; i < dettItemsCons.Length; i++)
            {
                this.dataSetItemsCons.DataTable1.AddDataTable1Row(dettItemsCons[i].IdConservazione, dettItemsCons[i].Descrizione, dettItemsCons[i].Data_riversamento, dettItemsCons[i].UserId, dettItemsCons[i].CollocazioneFisica, dettItemsCons[i].tipo_cons, dettItemsCons[i].num_docInFasc, dettItemsCons[i].id_profile_trasm);
            }
            gv.DataSource = this.dataSetItemsCons.Tables[0];
            gv.DataBind();
        }

        protected void gv_dettItemsCons_PreRender(object sender, EventArgs e)
        {
            if (Request.QueryString["idProfile"] != null && Request.QueryString["idProfile"] != string.Empty)
            {
                this.gv_dettItemsCons.Columns[6].Visible = false;
            }

            DocsPAWA.DocsPaWR.TipoIstanzaConservazione[] cons = ProxyManager.getWS().GetTipologieIstanzeConservazione();

            for (int i = 0; i < this.gv_dettItemsCons.Rows.Count; i++)
            {
                for (int t = 0; t < cons.Length; t++)
                {
                    string tipo_cons = ((Label)this.gv_dettItemsCons.Rows[i].FindControl("tipo_cons")).Text;
                    if (tipo_cons.Equals(cons[t].Codice))
                    {
                        ((Label)this.gv_dettItemsCons.Rows[i].FindControl("tipo_cons")).Text = cons[t].Descrizione;
                    }
                }
            }
        }

        protected void gv_dettItemsCons_SelectedIndexChanged(object sender, EventArgs e)
        {
            string errorMessage = string.Empty;
            int documentIndex = this.gv_dettItemsCons.SelectedIndex;
            string idProfile_trasm = ((Label)this.gv_dettItemsCons.Rows[documentIndex].FindControl("lbl_idProfileTrasm")).Text;
            // Impostazione documento correntemente selezionato nel contesto
            this.SetCurrentDocumentIndexOnContext(documentIndex);
            if (!string.IsNullOrEmpty(idProfile_trasm))
            {
                DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDettaglioDocumento(this, idProfile_trasm, "");
                DocsPaWR.InfoDocumento doc = DocumentManager.getInfoDocumento(schedaDoc);

                if (doc.tipoProto.ToUpper().Equals("R"))
                {
                    Response.Write("<script>alert('Tipologia documento non visualizzabile')</script>");
                }
                else
                {

                    // Verifica se l'utente ha i diritti per accedere al documento
                    int retValue = DocumentManager.verificaACL("D", idProfile_trasm, UserManager.getInfoUtente(), out errorMessage);
                    if (retValue == 0 || retValue == 1)
                    {
                        string script = ("<script>alert('" + errorMessage + "');</script>");
                        Response.Write(script);
                    }
                    else
                    {
                        DocumentManager.setRisultatoRicerca(this, doc);

                        DocumentManager.removeListaDocProt(this);

                        Response.Write("<script>window.open('../documento/gestionedoc.aspx?tab=protocollo','principale'); window.close();</script>");

                    }
                }
            }
            else
            {
                Response.Write("<script>alert('Non esiste alcuna trasmissione associata al documento');</script>");
            }
        }

        private void SetCurrentDocumentIndexOnContext(int docIndex)
        {
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;

            if (currentContext != null)
                currentContext.QueryStringParameters["docIndex"] = docIndex.ToString();
        }


    }
}
