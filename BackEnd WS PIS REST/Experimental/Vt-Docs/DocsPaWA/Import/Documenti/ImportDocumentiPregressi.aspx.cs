using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.SiteNavigation;
using System.Drawing;
using DocsPAWA.DocsPaWR;
using System.IO;
using System.Web.UI.WebControls;

namespace DocsPAWA.Import.Documenti
{
    public partial class ImportDocumentiPregressi : CssPage
    {
        protected DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();

        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager.GetCurrent(this).RegisterPostBackControl(this.btnImporta);
            Utils.startUp(this);
            Page.Response.Expires = -1;
            if (!IsPostBack)
            {
                GetTheme();
                GetInitData();
                //string result = string.Empty;
                //this.hid_tab_est.Value = result;
                this.nuovo_imp.Attributes.Remove("class");
                this.nuovo_imp.Attributes.Add("class", "Click");
                this.stato_imp.Attributes.Remove("class");
                this.stato_imp.Attributes.Add("class", "noClick");
            }
        }


        protected void btn_Nuovo_Click(object sender, EventArgs e)
        {
            this.pnlReport.Visible = false;
            this.btnNuovo.Enabled = false;
            this.btnImporta.Enabled = true;
            this.btnEsporta.Visible = true;
            this.btnEsporta.Enabled = false;
            this.btnContinua.Enabled = false;
            this.pnlAvviso.Visible = false;
            this.pnlReport.Visible = false;
            this.uploadFile.Disabled = false;
            this.uploadFile = null;
            this.lbl_alert.Text = string.Empty;
            this.grvItems.DataSource = null;
            this.grvItems.DataBind();
            this.box_upload.Update();
            this.upPnlReport.Update();
            this.UpdatePanel1.Update();
           
        }


        protected void GetInitData()
        {

        }

        protected void GetTheme()
        {
            string Tema = string.Empty;
            string idAmm = string.Empty;
            if ((string)Session["AMMDATASET"] != null)
                idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
            else
            {
                idAmm = UserManager.getInfoUtente().idAmministrazione;
            }

            UserManager userM = new UserManager();
            Tema = userM.getCssAmministrazione(idAmm);

            if (!string.IsNullOrEmpty(Tema))
            {
                string[] colorsplit = Tema.Split('^');
                System.Drawing.ColorConverter colConvert = new ColorConverter();
                this.titlePage.ForeColor = System.Drawing.Color.White;
                this.titlePage.BackColor = (System.Drawing.Color)colConvert.ConvertFromString("#" + colorsplit[2]);
                this.lblAvviso.ForeColor = System.Drawing.Color.White;
                this.lblAvviso.BackColor = (System.Drawing.Color)colConvert.ConvertFromString("#" + colorsplit[2]);
                this.titleReport.ForeColor = System.Drawing.Color.White;
                this.titleReport.BackColor = (System.Drawing.Color)colConvert.ConvertFromString("#" + colorsplit[2]);
            }
            else
            {
                System.Drawing.ColorConverter colConvert = new ColorConverter();
                this.titlePage.ForeColor = System.Drawing.Color.White;
                this.titlePage.BackColor = (System.Drawing.Color)colConvert.ConvertFromString("#810d06");
                this.lblAvviso.ForeColor = System.Drawing.Color.White;
                this.lblAvviso.BackColor = (System.Drawing.Color)colConvert.ConvertFromString("#810d06");
                this.titleReport.ForeColor = System.Drawing.Color.White;
                this.titleReport.BackColor = (System.Drawing.Color)colConvert.ConvertFromString("#810d06");
            }
        }

        protected void btn_Importa_Click(object sender, EventArgs e)
        {

            bool controllo = true;


            if (uploadFile != null && !string.IsNullOrEmpty(uploadFile.Value))
            {
                //Nel CallContext inserisco nome del file(con estensione) a partire dal path del file di import
                importFileName = Path.GetFileName(uploadFile.Value);

                //Inizio importazione
                HttpPostedFile p = uploadFile.PostedFile;
                Stream fs = p.InputStream;
                byte[] dati = new byte[fs.Length];
                fs.Read(dati, 0, (int)fs.Length);
                fs.Close();

                DocsPaWR.EsitoImportPregressi result = SendRequest(dati);
                //Se mi torna null il foglio excel non è valido
                if (result == null)
                {
                    utils.AlertPostLoad.GenericMessage(this, "Impossibile contatare il server. Riprovare più tardi.");
                    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Impossibile contatare il server. Riprovare più tardi.');", true);
                    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Inserire un foglio excel valido');", true);
                }
                else
                {
                    if (result.itemPregressi != null && result.itemPregressi.Length > 0)
                    {
                        List<DocsPaWR.ItemReportPregressi> itemInErrorOrWarning = result.itemPregressi.Where(x => x.esito != "S").ToList();

                        FetchData(itemInErrorOrWarning);

                        if (result.esito)
                        {
                            this.btnContinua.Enabled = true;
                            EsitoImport = result;

                            this.lbl_alert.Text = "Nessun problema da segnalare durante la prima analisi del file excel";
                            this.btnImporta.Enabled = false;
                            this.uploadFile.Disabled = true;
                            this.btnContinua.Enabled = true;

                            //Bottone esporta disabilitato
                            this.btnEsporta.Visible = true;
                            this.btnEsporta.Enabled = false;
                        }
                        else
                        {
                            this.btnContinua.Enabled = false;
                            EsitoImport = new DocsPaWR.EsitoImportPregressi();

                            this.btnImporta.Enabled = false;
                            this.uploadFile.Disabled = true;
                            if (itemInErrorOrWarning.Count > 0)
                            {
                                //Andrea - lista itemInErrorOrWarning messa nel CallContext per poterla passare come parametro dell'export excel
                                ReportInError = itemInErrorOrWarning;

                                if (itemInErrorOrWarning.Count == 1)
                                {
                                    this.lbl_alert.Text = "Attenzione, c\' è " + itemInErrorOrWarning.Count + " avviso durante la prima analisi del file excel";
                                }
                                else
                                {
                                    this.lbl_alert.Text = "Attenzione, ci sono " + itemInErrorOrWarning.Count + " avvisi durante la prima analisi del file excel";
                                }

                                //Andrea - Abilito il pulsante esporta solo se ho elementi in errore
                                this.btnEsporta.Visible = true;
                                this.btnEsporta.Enabled = true;
                            }
                            else
                            {
                                this.lbl_alert.Text = "File excel non compatibile o corrotto.";
                                this.btnEsporta.Enabled = false;
                            }
                        }
                    }
                    else
                    {
                        this.btnContinua.Enabled = false;
                        EsitoImport = new DocsPaWR.EsitoImportPregressi();
                        this.btnImporta.Enabled = false;
                        this.uploadFile.Disabled = true;
                        this.lbl_alert.Text = "Nessuna riga presente nel foglio excel.";
                        //this.lbl_alert.Text = "Impossibile contatare il server. Riprovare più tardi.";
                    }
                    this.pnlReport.Visible = true;
                    this.btnNuovo.Enabled = true;
                    this.pnlAvviso.Visible = true;
                    this.pnlReport.Visible = true;
                    this.box_upload.Update();
                    this.upPnlReport.Update();
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Nessun file excel valido selezionato.');", true);
            }
        }

        protected void FetchData(List<DocsPaWR.ItemReportPregressi> result)
        {
            this.grvItems.DataSource = result;
            this.grvItems.CurrentPageIndex = 0;
            this.grvItems.DataBind();
        }

        protected DocsPaWR.EsitoImportPregressi SendRequest(byte[] fileToImport)
        {
            wws.Timeout = System.Threading.Timeout.Infinite;
            DocsPaWR.EsitoImportPregressi retValue = wws.importPregresso(UserManager.getInfoUtente(this), fileToImport, false);

            return retValue;
        }

        private bool importa(DocsPaWR.InfoUtente infoUtente, DocsPaWR.EsitoImportPregressi esitoImport, string descrizione)
        {
            wws.Timeout = System.Threading.Timeout.Infinite;
            //Aggiunta parametro descrizione
            wws.asyncImportPregresso(infoUtente, esitoImport, descrizione);

            return true;
        }

        //Aggiunta parametro descrizione
        private delegate bool asyncDelegate(DocsPaWR.InfoUtente infoUtente, DocsPaWR.EsitoImportPregressi esitoImpor, string descrizione);

        asyncDelegate mDelegate;

        protected void btn_Continua_Click(object sender, EventArgs e)
        {
            //Andrea
            //Prelevo la descrizione e la passo come parametro del metodo importa
            string descrizione = this.descrizione.Text.ToString();

            mDelegate = new asyncDelegate(importa);
            AsyncCallback callback = new AsyncCallback(asyncImport);

            //Andrea
            //Aggiunta parametro descrizione
            mDelegate.BeginInvoke(UserManager.getInfoUtente(this), EsitoImport, descrizione, callback, null);

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Import inviato con successo!');", true);
            this.pnlReport.Visible = false;
            this.btnNuovo.Enabled = false;
            this.btnImporta.Enabled = true;
            this.pnlAvviso.Visible = false;
            this.pnlReport.Visible = false;
            this.uploadFile.Disabled = false;
            this.uploadFile = null;
            this.btnContinua.Enabled = false;
            this.box_upload.Update();
            this.upPnlReport.Update();

            this.descrizione.Text = string.Empty;

        }

        //Andrea - Export ReportInErrore
        protected void btn_Esporta_Click(object sender, EventArgs e)
        {
            this.Export(ReportInError);
        }

        protected void Export(List<DocsPaWR.ItemReportPregressi> reportInErr)
        {
            DocsPaWR.FileDocumento file = new DocsPAWA.DocsPaWR.FileDocumento();
            try
            {
                file = GetFile(reportInErr);
                //Andrea De Marco - Aggiunto importFileName al Nome del File: EsitoControllo_nomefile.
                if (file != null)
                    file.name = file.name + importFileName;

                if (file == null || file.content == null || file.content.Length == 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "noRisultatiRicerca", "alert('Nessun documento trovato.');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "openFile", "OpenFile('" + file.name.ToString() + "');", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "errore", "alert('Errore di sistema: " + ex.Message.Replace("'", "\\'") + "');", true);
            }
        }

        public DocsPAWA.DocsPaWR.FileDocumento GetFile(List<DocsPaWR.ItemReportPregressi> repInErr)
        {
            this.FileDocumento = wws.ExportReportExcel(repInErr.ToArray());
            return this.FileDocumento;
        }

        private void asyncImport(IAsyncResult ar)
        {
            bool bResult = mDelegate.EndInvoke(ar);

            string mess = string.Empty;

            if (ar.IsCompleted)
                mess = "Importazione eseguita correttamente.";
            else
                mess = "Errori nell'importazione.";

            lbl_alert.Text = mess;
        }

        protected void btn_chiudi_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "close", "window.close();", true);
        }

        protected void Refresh(object sender, EventArgs e)
        {
            this.n_import.Visible = false;
            this.stato_import.Visible = true;
            this.btnContinua.Visible = false;
            FetchData();
            this.tipo_import.Update();
        }

        protected string GetTipoAvviso(DocsPaWR.ItemReportPregressi item)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(item.esito))
            {
                if (item.esito.Equals("W"))
                {
                    result = "Attenzione";
                }
                else
                {
                    if (item.esito.Equals("E"))
                    {
                        result = "Errore";
                    }
                }
            }
            return result;
        }

        protected string GetErrore(DocsPaWR.ItemReportPregressi item)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(item.errore))
            {
                string[] errori = item.errore.Split('|');
                result = "<ul>";
                foreach (string err in errori)
                {
                    result += "<li>" + err + "</li>";
                }
                result += "</ul>";
            }
            return result;
        }

        protected string GetLineaExcel(DocsPaWR.ItemReportPregressi item)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(item.ordinale))
            {
                result = item.ordinale;
            }
            return result;
        }

        protected void DataGrid_ItemCreated(object sender, DataGridItemEventArgs e)
        {

            //ImageButton imgDelete = (ImageButton)e.Item.FindControl("btn_Rimuovi");
            //ImageButton imgDetails = (ImageButton)e.Item.FindControl("btn_dettagli");


            //if (imgDelete != null)
            //{
            //    imgDelete.Attributes.Add("onmouseover", "this.src='../../images/ricerca/cancella_griglia_hover.gif'");
            //    imgDelete.Attributes.Add("onmouseout", "this.src='../../images/ricerca/cancella_griglia.gif'");
            //}

            //if (imgDetails != null)
            //{
            //    imgDetails.Attributes.Add("onmouseover", "this.src='../../images/proto/dett_lente_doc_up.gif'");
            //    imgDetails.Attributes.Add("onmouseout", "this.src='../../images/proto/dett_lente_doc.gif'");
            //}



            //if (e.Item.ItemType == ListItemType.Pager)
            //{
            //    if (e.Item.Cells.Count > 0)
            //    {
            //        e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());
            //    }
            //}
        }

        protected String GetImageAvviso(DocsPaWR.ItemReportPregressi item)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(item.esito))
            {
                if (item.esito.Equals("W"))
                {
                    result = "../../images/ico_warning.png";
                }
                else
                {
                    if (item.esito.Equals("E"))
                    {
                        result = "../../images/ico_error.png";
                    }
                }
            }
            return result;
        }

        protected void NI_Click(object sender, EventArgs e)
        {
            this.nuovo_imp.Attributes.Remove("class");
            this.nuovo_imp.Attributes.Add("class", "Click");
            this.stato_imp.Attributes.Remove("class");
            this.stato_imp.Attributes.Add("class", "noClick");

            this.btn_refresh.Visible = false;
            this.label_refresh.Visible = false;
            this.n_import.Visible = true;
            this.stato_import.Visible = false;
            this.pnlReport.Visible = false;
            this.btnContinua.Enabled = false;
            this.btnImporta.Enabled = true;
            this.btnNuovo.Enabled = false;
            if (this.uploadFile != null)
            {
                this.uploadFile.Disabled = false;
            }
            this.uploadFile = null;
            this.btnContinua.Visible = true;
            this.btnEsporta.Visible = true;
            this.btnEsporta.Enabled = false;
            this.UpdatePanel1.Update();
            this.tipo_import.Update();

            this.descrizione.Text = string.Empty;
        }

        protected void SI_Click(object sender, EventArgs e)
        {
            this.nuovo_imp.Attributes.Remove("class");
            this.nuovo_imp.Attributes.Add("class", "noClick");
            this.stato_imp.Attributes.Remove("class");
            this.stato_imp.Attributes.Add("class", "Click");

            this.btn_refresh.Visible = true;
            this.label_refresh.Visible = true;
            this.n_import.Visible = false;
            this.stato_import.Visible = true;
            this.btnContinua.Visible = false;
            this.btnEsporta.Visible = false;
            FetchData();
            this.tipo_import.Update();
        }


        protected void FetchData()
        {
            wws.Timeout = System.Threading.Timeout.Infinite;
            //Parametro true: fa il get degli item del report; false: non fa il get degli item 
            DocsPaWR.ReportPregressi[] result = wws.GetReports(true, UserManager.getInfoUtente(this), false);

            this.gridReport.DataSource = result;
            this.gridReport.CurrentPageIndex = 0;
            this.gridReport.DataBind();
        }


        protected void ImageCreatedRender(Object sender, DataGridItemEventArgs e)
        {
            ImageButton imgDelete = (ImageButton)e.Item.FindControl("btn_Rimuovi");
            ImageButton imgDettaglio = (ImageButton)e.Item.FindControl("btn_dettagli");

            //Andrea
            System.Web.UI.WebControls.Image imgErrore = (System.Web.UI.WebControls.Image)e.Item.FindControl("img_errore");

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
            wws.Timeout = System.Threading.Timeout.Infinite;
            this.wws.DeleteReportById(a.Text);
            FetchData();
            this.tipo_import.Update();
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

        //Andrea
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
        //Andrea
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

        /// <summary>
        ///  esitoImport
        /// </summary>
        protected DocsPaWR.EsitoImportPregressi EsitoImport
        {
            get
            {
                if (CallContextStack.CurrentContext != null && CallContextStack.CurrentContext.ContextState["esitoImport"] != null)
                {
                    if (CallContextStack.CurrentContext == null)
                    {
                        CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                    }
                    return CallContextStack.CurrentContext.ContextState["esitoImport"] as DocsPaWR.EsitoImportPregressi;
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
                CallContextStack.CurrentContext.ContextState["esitoImport"] = value;
            }
        }

        /// <summary>
        ///  ReportInError
        /// </summary>
        protected List<DocsPaWR.ItemReportPregressi> ReportInError
        {
            get
            {
                if (CallContextStack.CurrentContext != null && CallContextStack.CurrentContext.ContextState["ReportInError"] != null)
                {
                    if (CallContextStack.CurrentContext == null)
                    {
                        CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                    }
                    return CallContextStack.CurrentContext.ContextState["ReportInError"] as List<DocsPaWR.ItemReportPregressi>;
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
                CallContextStack.CurrentContext.ContextState["ReportInError"] = value;
            }
        }

        /// <summary>
        /// File Excel
        /// </summary>
        public DocsPaWR.FileDocumento FileDocumento
        {
            get
            {
                if (CallContextStack.CurrentContext != null && CallContextStack.CurrentContext.ContextState["fileDocumento"] != null)
                {
                    if (CallContextStack.CurrentContext == null)
                    {
                        CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                    }
                    return CallContextStack.CurrentContext.ContextState["fileDocumento"] as DocsPaWR.FileDocumento;
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
                CallContextStack.CurrentContext.ContextState["fileDocumento"] = value;
            }
        }

        /// <summary>
        /// importFileName
        /// </summary>
        public string importFileName
        {
            get
            {
                if (CallContextStack.CurrentContext != null && CallContextStack.CurrentContext.ContextState["importFileName"] != null)
                {
                    if (CallContextStack.CurrentContext == null)
                    {
                        CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                    }
                    return CallContextStack.CurrentContext.ContextState["importFileName"] as string;
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
                CallContextStack.CurrentContext.ContextState["importFileName"] = value;
            }
        }
    }
}