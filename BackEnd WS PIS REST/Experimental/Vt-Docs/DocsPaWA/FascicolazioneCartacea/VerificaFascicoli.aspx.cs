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

namespace DocsPAWA.FascicolazioneCartacea
{
    /// <summary>
    /// 
    /// </summary>
    public partial class VerificaFascicoli : DocsPAWA.CssPage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Response.Expires = -1;

            if (!this.IsPostBack)
            {
                this.RegisterClientScriptHandler();

                // Caricamento dati fascicoli
                this.Fetch();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void RegisterClientScriptHandler()
        {
            this.btnClose.Attributes.Add("onClick", "window.close();");
        }

        /// <summary>
        /// Caricamento dati
        /// </summary>
        protected void Fetch()
        {
            DocsPaWR.FascicoloArchivio[] fascicoli = this.GetFascicoli();

            if (fascicoli == null)
                throw new ApplicationException("Si è verificato un errore nel reperimento dei fascicoli cartacei");

            string message = string.Empty;

            if (fascicoli.Length==0)
            {
                message = "Il documento non risulta fascicolato in alcun fascicolo cartaceo";
            }
            else
            {
                message = "Il documento risulta già presente nei seguenti fascicoli cartacei:";

                this.grdFascicoliCartacei.DataSource = this.FascicoliToDataset(this.GetFascicoli());
                this.grdFascicoliCartacei.DataBind();
            }

            this.lblTitle.Text = message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fascicoli"></param>
        /// <returns></returns>
        protected virtual DataSet FascicoliToDataset(DocsPaWR.FascicoloArchivio[] fascicoli)
        {
            DataSet ds = new DataSet();

            DataTable dt = new DataTable();
            dt.Columns.Add("IdFascicolo", typeof(int));
            dt.Columns.Add("CodiceFascicolo", typeof(string));
            dt.Columns.Add("DescrizioneFascicolo", typeof(string));
            ds.Tables.Add(dt);

            foreach (DocsPaWR.FascicoloArchivio item in fascicoli)
            {
                DataRow row = dt.NewRow();
                row["IdFascicolo"] = item.IdFascicolo;
                row["CodiceFascicolo"] = item.CodiceFascicolo;
                row["DescrizioneFascicolo"] = item.DescrizioneFascicolo;
                dt.Rows.Add(row);
            }

            return ds;
        }

        /// <summary>
        /// Reperimento fascicoli cartacei contenenti il documento
        /// </summary>
        /// <returns></returns>
        protected virtual DocsPaWR.FascicoloArchivio[] GetFascicoli()
        {
            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            return ws.FascCartaceaVerificaDocumentoFascicolato(UserManager.getInfoUtente(), this.VersionId);
        }

        /// <summary>
        /// Reperimento VersionId del documento per cui è richiesto il controllo
        /// </summary>
        protected virtual int VersionId
        {
            get
            {
                int retValue;
                if (Int32.TryParse(Request.QueryString["versionId"], out retValue))
                    return retValue;
                else 
                    throw new ApplicationException("Parametro 'versionId' non valido");
            }
        }
    }
}
