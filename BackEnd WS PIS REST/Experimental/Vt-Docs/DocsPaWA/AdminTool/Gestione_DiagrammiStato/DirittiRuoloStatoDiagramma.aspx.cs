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
using System.Collections.Generic;
namespace DocsPAWA.AdminTool.Gestione_DiagrammiStato
{
    public partial class DirittiRuoloStatoDiagramma : System.Web.UI.Page
    {
        private string idStato = string.Empty;
        private string idDiagramma = string.Empty;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.idStato = this.Request.QueryString["idStato"];
                this.idDiagramma = this.Request.QueryString["idDiagramma"];

                if (!string.IsNullOrEmpty(idStato) && (!string.IsNullOrEmpty(idDiagramma)))
                {
                    impostaTitolo();
                    caricaDgRuoli();
                }
            }
        }

        private void caricaDgRuoli()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("DESCRIZIONE RUOLO");
            dt.Columns.Add("VISIBILITA");
            ArrayList listaRuoliRicerca = (ArrayList)Session["LISTA_RUOLI"];
            List<DocsPaWR.AssRuoloStatiDiagramma> assRuoliStatoDia = DiagrammiManager.GetRuoliStatiDiagramma(Convert.ToInt32(idDiagramma));
            if (listaRuoliRicerca != null && listaRuoliRicerca.Count > 0)
            {
                foreach (DocsPaWR.Ruolo role in listaRuoliRicerca)
                {
                    DataRow rw = dt.NewRow();
                    string cha_not_vis = (from r in assRuoliStatoDia
                                          where r.ID_DIAGRAMMA.Equals(idDiagramma) && r.ID_STATO.Equals(idStato)
                                              && r.ID_GRUPPO.Equals(role.idGruppo)
                                          select r.CHA_NOT_VIS).FirstOrDefault();
                    rw[0] = role.descrizione;
                    rw[1] = string.IsNullOrEmpty(cha_not_vis) ? "SI" : "NO";
                    dt.Rows.Add(rw);
                }
                dt.AcceptChanges();
                dg_Ruoli.DataSource = dt;
                dg_Ruoli.DataBind();
            }
        }

        private void impostaTitolo()
        {
            DocsPaWR.DiagrammaStato diagramma = DiagrammiManager.getDiagrammaById(idDiagramma, this.Page);
            lbl_titolo.Text = "Diagramma : - "+ diagramma.DESCRIZIONE + " - STATO : " + 
                (from s in diagramma.STATI where s.SYSTEM_ID.ToString().Equals(idStato) select s.DESCRIZIONE).First();
        }
    }
}
