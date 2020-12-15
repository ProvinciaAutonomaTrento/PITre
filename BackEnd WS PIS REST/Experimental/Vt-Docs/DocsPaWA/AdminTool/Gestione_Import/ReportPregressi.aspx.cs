using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.SiteNavigation;

namespace DocsPAWA.AdminTool.Gestione_Import
{
    public partial class ReportPregressi : System.Web.UI.Page
    {
        private DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GestioneGrafica();
                PopolaInfoUtente();
                DocsPaWR.ReportPregressi[] result = SendRequest();
                FetchData(result);
            }
        }

        protected DocsPaWR.ReportPregressi[] SendRequest()
        {
            ws.Timeout = System.Threading.Timeout.Infinite;
            //Parametro true: fa il get degli item del report; false: non fa il get degli item 
            DocsPaWR.ReportPregressi[] result = ws.GetReports(true, InfoUtente,true);
            return result;
        }

        protected void GestioneGrafica()
        {
            if (!IsPostBack)
            {
                lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "1");
            }
        }

        protected void FetchData(DocsPaWR.ReportPregressi[] result)
        {
            this.grvItems.DataSource = result;
            this.grvItems.CurrentPageIndex = 0;
            this.grvItems.DataBind();
        }

        //Andrea - campo descrizione report
        protected string GetDescrizione(DocsPaWR.ReportPregressi item)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(item.descrizione))
            {
                result = item.descrizione;
            }
            return result;
        }

        protected string GetDataInizio(DocsPaWR.ReportPregressi item)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(item.dataEsecuzione))
            {
                result = item.dataEsecuzione;
            }
            return result;
        }

        protected string GetDataFine(DocsPaWR.ReportPregressi item)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(item.dataFine))
            {
                result = item.dataFine;
            }
            return result;
        }

        protected string GetNumeroDocumenti(DocsPaWR.ReportPregressi item)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(item.numDoc))
            {
                result = item.numDoc;
            }
            return result;
        }

        protected string GetReportID(DocsPaWR.ReportPregressi item)
        {
            string result = item.systemId;
            return result;
        }

        protected void DataGrid_ItemCreated(object sender, DataGridItemEventArgs e)
        {

          
        }

        protected void ImageCreatedRender(Object sender, DataGridItemEventArgs e)
        {
            ImageButton imgDelete = (ImageButton)e.Item.FindControl("btn_Rimuovi");
            ImageButton imgDettaglio = (ImageButton)e.Item.FindControl("btn_dettagli");

            Image imgErrore = (Image)e.Item.FindControl("img_errore");

            if (imgDettaglio != null)
            {
                TableCell cell = (TableCell)imgDettaglio.Parent;
                DataGridItem dgItem = (DataGridItem)cell.Parent;
                Label a = (Label)dgItem.FindControl("PERC");
                if (a != null && !string.IsNullOrEmpty(a.Text) && a.Text.Equals("100%"))
                {
                    imgDettaglio.Visible = true;
                    imgDettaglio.Attributes.Add("onmouseover", "this.src='../../images/proto/dett_lente_doc_up.gif'");
                    imgDettaglio.Attributes.Add("onmouseout", "this.src='../../images/proto/dett_lente_doc.gif'");

                    if (imgDelete != null)
                    {
                        imgDelete.Visible = true;
                        imgDelete.Attributes.Add("onmouseover", "this.src='../../images/ricerca/cancella_griglia_hover.gif'");
                        imgDelete.Attributes.Add("onmouseout", "this.src='../../images/ricerca/cancella_griglia.gif'");
                    }

                    //Andrea
                    if (imgErrore != null)
                    {
                        imgErrore.Visible = true;
                    }
                }
            }

            if (e.Item.ItemType == ListItemType.Pager)
            {
                if (e.Item.Cells.Count > 0)
                {
                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());
                }
            }
        }

        protected void DeleteReport(object sender, EventArgs e)
        {
            ImageButton image = (ImageButton)sender;
            TableCell cell = (TableCell)image.Parent;
            DataGridItem dgItem = (DataGridItem)cell.Parent;
            Label a = (Label)dgItem.FindControl("SYSTEM_ID");

            //Cancella Report
            ws.Timeout = System.Threading.Timeout.Infinite;
            this.ws.DeleteReportById(a.Text);

            DocsPaWR.ReportPregressi[] result = SendRequest();
            FetchData(result);
            this.box_upload.Update();
        }

        protected void ViewDetails(object sender, EventArgs e)
        {
            ImageButton image = (ImageButton)sender;
            TableCell cell = (TableCell)image.Parent;
            DataGridItem dgItem = (DataGridItem)cell.Parent;
            Label a = (Label)dgItem.FindControl("SYSTEM_ID");

            //Apri export excel dei dati dell'imports
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "OpenImpotDetails", "OpenImpotDetails('" + a.Text + "');", true);
        }

        protected void Refresh(object sender, EventArgs e)
        {
            DocsPaWR.ReportPregressi[] result = SendRequest();
            FetchData(result);
            this.box_upload.Update();
        }

        protected string GetPercentuale(DocsPaWR.ReportPregressi item)
        {
            string result = "0";
            int numDoc = Int32.Parse(item.numDoc);
            int elaborati = 0;
            double percentuale = 0;
            if (item.itemPregressi != null && item.itemPregressi.Length > 0)
            {
                //elaborati = item.itemPregressi.Length;
                elaborati = Int32.Parse(item.numeroElaborati);
                percentuale = (100 * elaborati) / numDoc;
                result = percentuale.ToString();
            }
            return result + "%";
        }

        protected string NumPercentuale(DocsPaWR.ReportPregressi item)
        {
            string result = "0";
            int numDoc = Int32.Parse(item.numDoc);
            int elaborati = 0;
            double percentuale = 0;
            if (item.itemPregressi != null && item.itemPregressi.Length > 0)
            {
                elaborati = item.itemPregressi.Length;
                percentuale = (100 * elaborati) / numDoc;
                result = percentuale.ToString();
            }
            return result;
        }

        protected string GetImmaginePercentuale(DocsPaWR.ReportPregressi item)
        {
            string result = "../../images/progressBar/00_perc.jpg";
            int numDoc = Int32.Parse(item.numDoc);
            int elaborati = 0;
            double percentuale = 0;
            if (item.itemPregressi != null && item.itemPregressi.Length > 0)
            {
                elaborati = item.itemPregressi.Length;
                percentuale = (100 * elaborati) / numDoc;
                result = percentuale.ToString();
            }
            if (percentuale >= 0 && percentuale < 10)
            {
                result = "../../images/progressBar/00_perc.jpg";
            }
            if (percentuale >= 10 && percentuale < 20)
            {
                result = "../../images/progressBar/10_perc.jpg";
            }
            if (percentuale >= 20 && percentuale < 30)
            {
                result = "../../images/progressBar/20_perc.jpg";
            }
            if (percentuale >= 30 && percentuale < 40)
            {
                result = "../../images/progressBar/30_perc.jpg";
            }
            if (percentuale >= 40 && percentuale < 50)
            {
                result = "../../images/progressBar/40_perc.jpg";
            }
            if (percentuale >= 50 && percentuale < 60)
            {
                result = "../../images/progressBar/50_perc.jpg";
            }
            if (percentuale >= 60 && percentuale < 70)
            {
                result = "../../images/progressBar/60_perc.jpg";
            }
            if (percentuale >= 70 && percentuale < 80)
            {
                result = "../../images/progressBar/70_perc.jpg";
            }
            if (percentuale >= 80 && percentuale < 90)
            {
                result = "../../images/progressBar/80_perc.jpg";
            }
            if (percentuale >= 90 && percentuale < 100)
            {
                result = "../../images/progressBar/90_perc.jpg";
            }
            if (percentuale == 100)
            {
                result = "../../images/progressBar/100_perc.jpg";
            }

            return result;
        }

        protected void PopolaInfoUtente()
        {
            DocsPaWR.Utente ut = new DocsPAWA.DocsPaWR.Utente();
            string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            string codiceAmministrazione = amministrazione[0];
            string idAmministrazione = IdAmministrazione.ToString();

            ut.codiceAmm = codiceAmministrazione;
            ut.idAmministrazione = idAmministrazione;
            ut.tipoIE = "I";
            UserManager.setUtente(this, ut);

            DocsPaWR.Ruolo rl = new DocsPAWA.DocsPaWR.Ruolo();
            rl.codiceAmm = codiceAmministrazione;
            rl.idAmministrazione = idAmministrazione;
            rl.tipoIE = "I";
            rl.systemId = idAmministrazione;
            rl.uo = new DocsPAWA.DocsPaWR.UnitaOrganizzativa();
            rl.uo.codiceRubrica = codiceAmministrazione;
            UserManager.setRuolo(this, rl);

            this.InfoUtente = UserManager.getInfoUtente(this);
        }

        protected String GetImageErrore(DocsPaWR.ReportPregressi item)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(item.inError))
            {
                if (!item.inError.Equals("0"))
                {
                    result = "../../images/ico_warning.png";
                }
                else
                    result = "../../images/check_policy.gif";
            }
            return result;
        }

        protected string GetNumeroDiErrori(DocsPaWR.ReportPregressi item)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(item.inError))
            {
                if (item.inError.Equals("0"))
                {
                    result = "Nessun errore presente";
                }
                else
                {
                    if (item.inError.Equals("1"))
                    {
                        result = "Presente 1 errore";
                    }
                    else
                    {
                        result = "Sono presenti " + item.inError + " errori";
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Reperimento idamministrazione corrente
        /// </summary>
        protected int IdAmministrazione
        {
            get
            {
                string idAmministrazione = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");

                return Convert.ToInt32(idAmministrazione);
            }
        }

        /// <summary>
        /// Link della pagina del dettaglio dell import
        /// </summary>
        protected string UrlViewImport
        {
            get
            {
                return "ImportPregressiDetails.aspx";
            }
        }

        /// <summary>
        /// InfoUtente
        /// </summary>
        protected DocsPaWR.InfoUtente InfoUtente
        {
            get
            {
                if (CallContextStack.CurrentContext != null && CallContextStack.CurrentContext.ContextState["infoUtente"] != null)
                {
                    if (CallContextStack.CurrentContext == null)
                    {
                        CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                    }
                    return CallContextStack.CurrentContext.ContextState["infoUtente"] as DocsPaWR.InfoUtente;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (CallContextStack.CurrentContext == null)
                {
                    CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                }
                CallContextStack.CurrentContext.ContextState["infoUtente"] = value;
            }
        }

        /// <summary>
        /// Reports
        /// </summary>
        public DocsPaWR.ReportPregressi[] Reports
        {
            get
            {
                if (CallContextStack.CurrentContext != null && CallContextStack.CurrentContext.ContextState["reports"] != null)
                {
                    if (CallContextStack.CurrentContext == null)
                    {
                        CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                    }
                    return CallContextStack.CurrentContext.ContextState["reports"] as DocsPaWR.ReportPregressi[];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (CallContextStack.CurrentContext == null)
                {
                    CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                }
                CallContextStack.CurrentContext.ContextState["reports"] = value;
            }
        }
    }
}