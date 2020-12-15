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
using System.Linq;

namespace SAAdminTool.AdminTool.Gestione_ProfDinamica
{
    public partial class DirittiRuoloOggettoCustomDoc : System.Web.UI.Page
    {
        private string idTemplate = string.Empty;
        private string idOggettoCustom = string.Empty;
        private DocsPaWR.Templates template;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.idTemplate = this.Request.QueryString["idTemplate"].ToString();
                this.idOggettoCustom = this.Request.QueryString["idOggettoCustom"].ToString();
                template = (SAAdminTool.DocsPaWR.Templates)Session["templateSelPerVisibilita"];

                if (template != null && template.ELENCO_OGGETTI != null && !string.IsNullOrEmpty(idTemplate) && !string.IsNullOrEmpty(idOggettoCustom))
                {
                    impostaTitolo();
                    caricaDgRuoli();
                }
            }
        }

        private void caricaDgRuoli()
        {
            ArrayList listaDirittiRuoliOggettoCustom = ProfilazioneDocManager.getRuoliFromOggettoCustomDoc(idTemplate, idOggettoCustom, this);

            DataTable dt = new DataTable();
            dt.Columns.Add("DESCRIZIONE RUOLO");
            dt.Columns.Add("MODIFICA");
            dt.Columns.Add("VISIBILITA");
            dt.Columns.Add("ANNULLA REP");
            
            foreach(DocsPaWR.AssDocFascRuoli assDocFascRuoli in listaDirittiRuoliOggettoCustom)
            {
                DataRow rw = dt.NewRow();
                DocsPaWR.Ruolo ruolo = UserManager.getRuoloByIdGruppo(assDocFascRuoli.ID_GRUPPO,this);
                if (ruolo != null)
                {
                    rw[0] = ruolo.descrizione;
                    if (assDocFascRuoli.INS_MOD_OGG_CUSTOM == "1")
                        rw[1] = "SI";
                    else
                        rw[1] = "NO";
                    if (assDocFascRuoli.VIS_OGG_CUSTOM == "1")
                        rw[2] = "SI";
                    else
                        rw[2] = "NO";
                    if (assDocFascRuoli.ANNULLA_REPERTORIO == "1")
                        rw[3] = "SI";
                    else
                        rw[3] = "NO";
                    dt.Rows.Add(rw);
                }
            }
            dt.AcceptChanges();
            dg_Ruoli.DataSource = dt;
            dg_Ruoli.DataBind();            
        }

        private void impostaTitolo()
        {
            foreach (DocsPaWR.OggettoCustom oggettoCustom in template.ELENCO_OGGETTI)
            {
                if (oggettoCustom.SYSTEM_ID.ToString() == idOggettoCustom)
                    lbl_titolo.Text = "TIPOLOGIA : - "+ template.DESCRIZIONE + " - CAMPO : " + oggettoCustom.DESCRIZIONE;
            }
        }
    }
}
