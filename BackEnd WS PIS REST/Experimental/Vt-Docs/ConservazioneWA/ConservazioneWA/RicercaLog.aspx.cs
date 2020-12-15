using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace ConservazioneWA
{
    /// <summary>
    /// 
    /// </summary>
    public partial class RicercaLog : System.Web.UI.Page
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

            //GM 29-8-2013
            //se è presente in sessione la variabile valoreContent va rimossa
            //per evitare problemi nell'estrazione dei report excel
            if(Session["valoreContent"] != null)
                Session.Remove("valoreContent");


            if (!this.IsPostBack)
            {
                GestioneGrafica();
                PopolaDdlAzioni();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnFind_Click(object sender, EventArgs e)
        {
            this.grdLogApplicativi.CurrentPageIndex = 0;

            this.Fetch();

            this.upDettaglio.Update();

            this.btnExportPdf.Visible = true;
            this.btnExportXls.Visible = true;
            //this.btnExport.Attributes.Add("onClick", "exportLog();");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdLogApplicativi_ItemCommand(object sender, DataGridCommandEventArgs e)
        {
            if (e.CommandName == "GO_TO_ISTANZA")
            {
                LinkButton btnIstanza = (LinkButton)e.Item.FindControl("btnIstanza");

                this.Response.Redirect(string.Format("~/RicercaIstanze.aspx?id={0}", btnIstanza.Text));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdLogApplicativi_PageIndexChanged(object sender, DataGridPageChangedEventArgs e)
        {
            this.grdLogApplicativi.CurrentPageIndex = e.NewPageIndex;

            this.Fetch();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdLogApplicativi_PreRender(object sender, EventArgs e)
        {
            foreach (DataGridItem itm in this.grdLogApplicativi.Items)
            {
                Label lblEsito = (Label)itm.FindControl("lblEsito");

                if (lblEsito != null)
                {
                    if (lblEsito.Text != "OK")
                        itm.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void Fetch()
        {
            WSConservazioneLocale.InfoUtente infoUtente = (WSConservazioneLocale.InfoUtente)Session["infoutCons"];

            //GM 31-7-2013
            //filtro su esito
            string esito = this.ddl_esito.SelectedItem.Value;
            if (esito == "-1")
                esito = string.Empty;

            //filtro su azione
            string azione = this.ddl_azione.SelectedItem.Value;
            if (azione == "0")
                azione = string.Empty;

            this.grdLogApplicativi.DataSource = Utils.ConservazioneManager.GetLogs(
                                infoUtente, 
                                this.txtIdIstanza.Text, 
                                this.txtDataLogFrom.Text,
                                this.txtDataLogTo.Text,
                                this.txtUtente.Text,
                                azione,
                                esito);

            this.grdLogApplicativi.DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logItem"></param>
        /// <returns></returns>
        protected string GetEsito(WSConservazioneLocale.LogConservazione logItem)
        {
            return (logItem.Esito ? "OK" : "KO");
        }

        protected void GestioneGrafica()
        {
            amm = Utils.ConservazioneManager.GetInfoAmmCorrente(infoUtente.idAmministrazione);
            this.lbl_amm.Text = this.amm.Codice + " - " + this.amm.Descrizione;
            this.btnFind.Attributes.Add("onmouseover", "this.className='cbtnHover';");
            this.btnFind.Attributes.Add("onmouseout", "this.className='cbtn';");
            this.btnExportPdf.Attributes.Add("onmouseover", "this.className='cbtnHover';");
            this.btnExportPdf.Attributes.Add("onmouseout", "this.className='cbtn';");
            this.btnExportXls.Attributes.Add("onmouseover", "this.className='cbtnHover';");
            this.btnExportXls.Attributes.Add("onmouseout", "this.className='cbtn';");

        }

        protected void PopolaDdlAzioni()
        {
            //recupero la lista delle azioni da inserire nella ddl
            WSConservazioneLocale.LogConservazione[] listAzioni = ConservazioneWA.Utils.ConservazioneManager.GetListAzioniLog();

            //aggiungo gli elementi alla ddl
            foreach (WSConservazioneLocale.LogConservazione azione in listAzioni)
            {
                ListItem l = new ListItem(azione.Azione, azione.CodiceAzione);
                this.ddl_azione.Items.Add(l);
            }
      
        }

        protected void ExportPdf(object sender, EventArgs e)
        {
            this.GeneraReport("PDF");
        }

        protected void ExportExcel(object sender, EventArgs e)
        {
            this.GeneraReport("EXL");
        }

        protected void GeneraReport(string reportType)
        {
            //recupero i parametri da inserire in una lista di filtri ricerca
            List<WSConservazioneLocale.FiltroRicerca> filters = new List<WSConservazioneLocale.FiltroRicerca>();

            string esito = this.ddl_esito.SelectedItem.Value;
            if (esito == "-1")
                esito = string.Empty;

            string azione = this.ddl_azione.SelectedItem.Value;
            if (azione == "0")
                azione = string.Empty;

            filters.Add(new WSConservazioneLocale.FiltroRicerca { argomento = "idIstanza", valore = this.txtIdIstanza.Text });
            filters.Add(new WSConservazioneLocale.FiltroRicerca { argomento = "dataFrom", valore = this.txtDataLogFrom.Text });
            filters.Add(new WSConservazioneLocale.FiltroRicerca { argomento = "dataTo", valore = this.txtDataLogTo.Text });
            filters.Add(new WSConservazioneLocale.FiltroRicerca { argomento = "utente", valore = this.txtUtente.Text });
            filters.Add(new WSConservazioneLocale.FiltroRicerca { argomento = "azione", valore = azione });
            filters.Add(new WSConservazioneLocale.FiltroRicerca { argomento = "esito", valore = esito });

            string tipoReport = reportType;
            string reportKey = "LogConservazione";

            string titoloReport = string.Format("Log Conservazione");

            WSConservazioneLocale.FileDocumento fileDoc = Utils.ConservazioneManager.getFileReport(
                filters.ToArray(),
                tipoReport,
                tipoReport,
                titoloReport,
                reportKey,
                reportKey,
                infoUtente);

            //se il report è in pdf modifico il content-type per
            //permettere la visualizzazione nel browser
            if (reportType == "PDF")
            {
                fileDoc.contentType = "application/pdf";
                fileDoc.name = string.Format("Export_Log_{0}.pdf", DateTime.Now.ToString("dd-MM-yyyy"));
            }
            else
            {
                fileDoc.contentType = "application/vnd.ms-excel";
                fileDoc.name = string.Format("Export_Log_{0}.xls", DateTime.Now.ToString("dd-MM-yyyy"));
            }

            if (fileDoc != null)
            {
                Session.Add("fileReport", fileDoc);
                //ClientScript.RegisterStartupScript(this.GetType(), "popupExport", "visualizzaReport();", true);
                string script = string.Format("visualizzaReport('{0}');", reportType);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "popupReport", script, true);
                //string filePath = "c:/sviluppo/report1.pdf";
                //FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                //fileStream.Write(fileDoc.content, 0, fileDoc.content.Length);  
                //fileStream.Close();

            }
                
        }

    }
}
