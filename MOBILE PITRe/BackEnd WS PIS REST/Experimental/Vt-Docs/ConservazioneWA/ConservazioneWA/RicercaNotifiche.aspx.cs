using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ConservazioneWA
{
    /// <summary>
    /// 
    /// </summary>
    public partial class RicercaNotifiche : System.Web.UI.Page
    {

        protected WSConservazioneLocale.InfoAmministrazione amm;
        protected WSConservazioneLocale.InfoUtente infoUtente;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;
            this.infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
            if (!this.IsPostBack)
            {
                this.Fetch();
                GestioneGrafica();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdNotifiche_ItemCommand(object sender, DataGridCommandEventArgs e)
        {
            if (e.CommandName == "GO_TO_ISTANZA")
            {
                Label lblIdIstanza = (Label)e.Item.FindControl("lblIdIstanza");

                this.Response.Redirect(string.Format("~/RicercaIstanze.aspx?id={0}", lblIdIstanza.Text));
            }
            else if (e.CommandName == "GO_TO_SUPPORTO")
            {
                Label lblIdSupporto = (Label)e.Item.FindControl("lblIdSupporto");

                this.Response.Redirect(string.Format("~/RicercaSupportiIstanze.aspx?id={0}", lblIdSupporto.Text));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdNotifiche_PageIndexChanged(object sender, DataGridPageChangedEventArgs e)
        {
            this.grdNotifiche.CurrentPageIndex = e.NewPageIndex;

            this.Fetch();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdNotifiche_PreRender(object sender, EventArgs e)
        {
            foreach (DataGridItem itm in this.grdNotifiche.Items)
            {
                Label lblGiorniScadenza = (Label)itm.FindControl("lblGiorniScadenza");

                if (lblGiorniScadenza != null)
                {
                    int giorniScadenza = Convert.ToInt32(lblGiorniScadenza.Text);

                    if (giorniScadenza > 0)
                        itm.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void Fetch()
        {
            this.grdNotifiche.DataSource = Utils.ConservazioneManager.GetNotifiche((Utils.ConservazioneManager.GetInfoAmmCorrente(infoUtente.idAmministrazione)).IDAmm);
            this.grdNotifiche.DataBind();
        }

        protected void GestioneGrafica()
        {

            amm = Utils.ConservazioneManager.GetInfoAmmCorrente(infoUtente.idAmministrazione);
            this.lbl_amm.Text = this.amm.Codice + " - " + this.amm.Descrizione;
        }
    }
}