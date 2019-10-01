using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDatalLibrary;
using System.Collections;
using NttDataWA.UIManager;
using System.Threading;
using System.Data;

namespace NttDataWA.Popup
{
    public partial class ReportFormatiIstanzaConservazione : System.Web.UI.Page
    {


        protected List<ReportFormatiConservazione> listaDoc
        {
            get
            {
                List<ReportFormatiConservazione> result = null;
                if (HttpContext.Current.Session["ReportConservazioneListaDoc"] != null)
                {
                    result = HttpContext.Current.Session["ReportConservazioneListaDoc"] as List<ReportFormatiConservazione>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["ReportConservazioneListaDoc"] = value;
            }
        }

        private InfoConservazione InstanceConservation
        {
            get
            {
                if (HttpContext.Current.Session["InstanceConservazione"] != null)
                    return (InfoConservazione)HttpContext.Current.Session["InstanceConservazione"];
                else
                    return null;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.InitializeLenguage();
                this.InitializeData();

            }
            this.RefreshScript();

        }

        private void RefreshScript()
        {
            ScriptManager.GetCurrent(this.Page).RegisterPostBackControl(this.btnGeneraReport);
        }

        private void InitializeData()
        {
            if (this.listaDoc == null)
            {
                this.listaDoc = DocumentManager.getDettaglioReportConservazioneByIdIstanzaCons(this.InstanceConservation.SystemID);

                this.litNumDocValue.Text = this.listaDoc.Where(x=>x.TipoFile.Equals("P")).Count().ToString();
                this.litNumFileValue.Text = this.listaDoc.Count.ToString();
            }
        }

        private void InitializeLenguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();

            this.litNumDoc.Text = Utils.Languages.GetLabelFromCode("litNumDoc", language);
            this.litNumFile.Text = Utils.Languages.GetLabelFromCode("litNumFile", language);
            this.btnGeneraReport.Text = Utils.Languages.GetLabelFromCode("btnGeneraReport", language);
            this.btnChiudi.Text = Utils.Languages.GetLabelFromCode("AddDocInProjectBtnClose", language);
            this.litAmmessi.Text = Utils.Languages.GetLabelFromCode("litAmmessi", language);
            this.litNonAmmessi.Text = Utils.Languages.GetLabelFromCode("litNonAmmessi", language);
            this.litValido.Text = Utils.Languages.GetLabelFromCode("litValido", language);
            this.litNonValido.Text = Utils.Languages.GetLabelFromCode("litNonValido", language);
            this.litNonDaValidare.Text = Utils.Languages.GetLabelFromCode("litNonDaValidare", language);
            this.litConv.Text = Utils.Languages.GetLabelFromCode("litConv", language);
            this.litNonConv.Text = Utils.Languages.GetLabelFromCode("litNonConv", language);
            this.litNonDaConv.Text = Utils.Languages.GetLabelFromCode("litNonDaConv", language);
            this.litError.Text = Utils.Languages.GetLabelFromCode("litError", language);
            this.litFiltriAvanzati.Text = Utils.Languages.GetLabelFromCode("litFiltriAvanzati", language);
            this.litFiltriBase.Text = Utils.Languages.GetLabelFromCode("litFiltriBase", language);
            this.litNonConvertibili.Text = Utils.Languages.GetLabelFromCode("litNonConvertibili", language);
        }

        private void exportGridToExcel(GridView gv, string filename)
        {

            //HttpResponse Response = HttpContext.Current.Response;
            if (gv.Rows.Count > 0)
            {

                System.IO.StringWriter tw = new System.IO.StringWriter();
                System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

                //Get the HTML for the control.
                gv.RenderControl(hw);

                //Write the HTML back to the browser.
                //Response.ContentType = application/vnd.ms-excel;
                Response.ContentType = "application/vnd.ms-excel";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");
                //Response.Write("<style>.text { mso-number-format:\\@; } </style>");
                //style to format numbers to string
                string style = @"<style> .textmode { mso-text-format:\@; } </style>";
                Response.Write(style);
                Response.Output.Write(tw.ToString());
                Response.Flush();
                Response.End();
                //Response.Write(tw.ToString());
                //Response.End();
            }
            else
            {
                string msgErrorExportExcel = "msgErrorExportExcel";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + msgErrorExportExcel.Replace("'", @"\'") + "', 'warning', '');", true);
                return;
            }


        }

        protected void btnChiudi_Click(object sender, EventArgs e)
        {
            try
            {
                this.closePage();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }

        }

        protected void btnGeneraReport_Click(object sender, EventArgs e)
        {
            try
            {
                List<ReportFormatiConservazione> docFiltrati = this.generateDataForExport();

                //string filename = "ReportFormatiIstanzaConservazione_" + this.InstanceConservation.SystemID + "_InData_" + System.DateTime.Now.ToString() + ".xls";

                //GridView grdForExport = new GridView();
                //grdForExport.AllowPaging = false;
                //grdForExport.DataSource = this.listaDoc;

                var docPerExport = ((from d in docFiltrati
                                     where !d.Modifica.Equals("0")
                                     orderby d.System_ID ascending
                                     select new
                                     {
                                         Segnatura = this.getSegnaturaDocPrincipale(d.ID_DocPrincipale),
                                         idDoc = d.DocNumber,
                                         Oggetto = d.dettaglioDocumento.Oggetto,
                                         DataCrezioneOProto = string.IsNullOrEmpty(d.dettaglioDocumento.DataProto) ? d.dettaglioDocumento.DataCreazione : d.dettaglioDocumento.DataProto,
                                         TipoDoc = d.dettaglioDocumento.TipoDocumento.Replace("G", "NP"),
                                         TipoFile = d.Estensione,
                                         PrincipaleAllegato = this.getTipoFile(d.TipoFile.ToUpper()),
                                         Ammesso = this.getFlagResult(d.Ammesso),
                                         Valido = this.getFlagResult(d.Valido),
                                         Consolidato = this.getFlagResult(d.Consolidato),
                                         Firmato = this.getFlagResult(d.Firmata),
                                         ConTimestamp = this.getFlagResult(d.Marcata),
                                         Convertibile = this.getFlagResult(d.Convertibile),
                                         TipoDiritto = this.getFlagDirittiResult(d.Modifica),
                                         UtenteProprietario = d.dettaglioDocumento.DescrUtProp,
                                         RuoloProprietario = d.dettaglioDocumento.DescrRuoloProp
                                     })).ToList();

                docPerExport.AddRange(from d in docFiltrati
                                      where d.Modifica.Equals("0")
                                      orderby d.System_ID ascending
                                      select new
                                      {
                                          Segnatura = this.getSegnaturaDocPrincipale(d.ID_DocPrincipale),
                                          idDoc = d.DocNumber,
                                          Oggetto = "Non si possiedono i diritti",
                                          DataCrezioneOProto = string.IsNullOrEmpty(d.dettaglioDocumento.DataProto) ? d.dettaglioDocumento.DataCreazione : d.dettaglioDocumento.DataProto,
                                          TipoDoc = d.dettaglioDocumento.TipoDocumento.Replace("G", "NP"),
                                          TipoFile = d.Estensione,
                                          PrincipaleAllegato = this.getTipoFile(d.TipoFile.ToUpper()),
                                          Ammesso = this.getFlagResult(d.Ammesso),
                                          Valido = this.getFlagResult(d.Valido),
                                          Consolidato = this.getFlagResult(d.Consolidato),
                                          Firmato = this.getFlagResult(d.Firmata),
                                          ConTimestamp = this.getFlagResult(d.Marcata),
                                          Convertibile = this.getFlagResult(d.Convertibile),
                                          TipoDiritto = this.getFlagDirittiResult(d.Modifica),
                                          UtenteProprietario = d.dettaglioDocumento.DescrUtProp,
                                          RuoloProprietario = d.dettaglioDocumento.DescrRuoloProp
                                      });


                //IEnumerable<DataRow> report = docPerExport.AsEnumerable() as IEnumerable<DataRow>;
                // Create a table from the query.
                DataTable dt = new DataTable();
                dt.Columns.Add("Segnatura");
                dt.Columns.Add("idDoc");
                dt.Columns.Add("Oggetto");
                dt.Columns.Add("DataCrezioneOProto");
                dt.Columns.Add("TipoDoc");
                dt.Columns.Add("TipoFile");
                dt.Columns.Add("PrincipaleAllegato");
                dt.Columns.Add("Ammesso");
                dt.Columns.Add("Valido");
                dt.Columns.Add("Consolidato");
                dt.Columns.Add("Firmato");
                dt.Columns.Add("ConTimestamp");
                dt.Columns.Add("Convertibile");
                dt.Columns.Add("TipoDiritto");
                dt.Columns.Add("UtenteProprietario");
                dt.Columns.Add("RuoloProprietario");


                foreach (var doc in docPerExport)
                {
                    dt.Rows.Add(doc.Segnatura
                                , doc.idDoc,
                                doc.Oggetto,
                                doc.DataCrezioneOProto
                                , doc.TipoDoc
                                , doc.TipoFile
                                , doc.PrincipaleAllegato
                                , doc.Ammesso
                                , doc.Valido
                                , doc.Consolidato
                                , doc.Firmato
                                , doc.ConTimestamp
                                , doc.Convertibile
                                , doc.TipoDiritto
                                , doc.UtenteProprietario
                             , doc.RuoloProprietario);
                }
                DataSet ds = new DataSet();
                ds.Tables.Add(dt);


                this.exportGridToExcel(ds);


                //}

                //string filePath = "c:/sviluppo/report1.pdf";
                //FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                //fileStream.Write(fileDoc.content, 0, fileDoc.content.Length);  
                //fileStream.Close();

                //grdForExport.DataSource = docPerExport;
                //grdForExport.RowStyle.CssClass = "tbl_rounded_custom";
                //grdForExport.DataBind();


                //this.exportGridToExcel(grdForExport, filename);
            }
            catch (ThreadAbortException exT)
            {
                //Log some trace info here
                return;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }

        }

        private void exportGridToExcel(DataSet ds)
        {
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                FileDocumento fd = DocumentManager.createReport(null, ds, "Excel", "Report sulla verifica dei formati di un'istanza di conservazione",
                                                string.Format("Istanza di conservazione = {0}", this.InstanceConservation.SystemID),
                                                "VerificaFormatiConservazione", "VerificaFormatiConservazione", UserManager.GetInfoUser());

                fd.contentType = "application/vnd.ms-excel";
                //fd.name = string.Format("Export_Verifica_{0}.xls", DateTime.Now.ToString("dd-MM-yyyy"));

                Response.ContentType = fd.contentType;

                Response.AddHeader("content-disposition", "inline;filename=" + fd.name);

                Response.AddHeader("content-length", fd.content.Length.ToString());

                Response.BinaryWrite(fd.content);
                Response.Flush();
                Response.End();
            }
            else
            {
                string msgErrorExportExcel = "msgErrorExportExcel";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + msgErrorExportExcel.Replace("'", @"\'") + "', 'warning', '');", true);
                return;
            }
            

        }

        private string getSegnaturaDocPrincipale(string id_docPrincipale)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(id_docPrincipale))
            {
                var doc = this.listaDoc.Where(x => x.DocNumber.Equals(id_docPrincipale)).FirstOrDefault();
                result = string.IsNullOrEmpty(doc.dettaglioDocumento.Segnatura) ? doc.DocNumber : doc.dettaglioDocumento.Segnatura;
            }

            return result;
        }



        //metodo che filtra i documenti associati all'item a secondo dei filtri applicati
        // (è una cosa orribile è composto da una cascata di if poichè nella form ci sono molti checkbox
        // e bisogna gestire le varie combinazioni)
        private List<ReportFormatiConservazione> generateDataForExport()
        {
            List<ReportFormatiConservazione> result = this.listaDoc;

            //gestione radioButton (con diversa granularità dei risultati da esportare
            //ossia se si usano i filtri base la granularità del report è il documento composto da doc principale e allegati, 
            //mentre usando i filtri avanzati la granularita diventa il singolo file principale e\o allegato)
            if (this.rbtConformDoc.SelectedValue != "0") //tutti
            {
                List<string> docPrincipaleNonConforme = result.Where(x => !(x.Ammesso.Equals("1") && (x.Valido.Equals("1") || x.Valido.Equals("2"))))
                                    .Select(x => x.ID_DocPrincipale).Distinct().ToList();
                if (this.rbtConformDoc.SelectedValue.Equals("1"))
                {
                    result = result.Select(x => x).Where(x => !docPrincipaleNonConforme.Contains(x.ID_DocPrincipale)).ToList();
                }
                else
                {
                    result = result.Select(x => x).Where(x => docPrincipaleNonConforme.Contains(x.ID_DocPrincipale)).ToList();
                }

            }

            //ammessi
            if (result.Count > 0 && !((this.chkAmmessi.Checked && this.chkNonAmmessi.Checked) ||
                (!this.chkAmmessi.Checked && !this.chkNonAmmessi.Checked)))
            {
                if (this.chkAmmessi.Checked)
                {
                    result = result.Select(x => x).Where(x => x.Ammesso.Equals("1")).ToList();
                }
                else
                {
                    result = result.Select(x => x).Where(x => x.Ammesso.Equals("0")).ToList();
                }
            }


            //Validi
            if (result.Count > 0 && !((this.chkNonValido.Checked && this.chkValido.Checked && this.chkNonDaValidare.Checked) ||
                (!this.chkNonValido.Checked && !this.chkValido.Checked && !this.chkNonDaValidare.Checked)))
            {
                if (this.chkValido.Checked)
                {
                    if (this.chkNonValido.Checked)
                    {
                        result = result.Select(x => x).Where(x => x.Valido.Equals("1") || x.Valido.Equals("0")).ToList();
                    }
                    else
                    {
                        if (this.chkNonDaValidare.Checked)
                        {
                            result = result.Select(x => x).Where(x => x.Valido.Equals("1") || x.Valido.Equals("2")).ToList();
                        }
                        else
                        {
                            result = result.Select(x => x).Where(x => x.Valido.Equals("1")).ToList();
                        }
                    }
                }
                else
                {
                    if (this.chkNonValido.Checked)
                    {
                        if (this.chkNonDaValidare.Checked)
                        {
                            result = result.Select(x => x).Where(x => x.Valido.Equals("0") || x.Valido.Equals("2")).ToList();
                        }
                        else
                        {
                            result = result.Select(x => x).Where(x => x.Valido.Equals("0")).ToList();
                        }
                    }
                    else
                    {
                        result = result.Select(x => x).Where(x => x.Valido.Equals("2")).ToList();
                    }
                }

            }

            //convertibili
            if (result.Count > 0 && !((this.chkConv.Checked && this.chkNonConvDaMe.Checked && this.chkNonDaConv.Checked && this.chkNonConvertibili.Checked) ||
                (!this.chkConv.Checked && !this.chkNonConvDaMe.Checked && !this.chkNonDaConv.Checked && !this.chkNonConvertibili.Checked)))
            {
                if (this.chkConv.Checked)
                {
                    if (this.chkNonConvDaMe.Checked)
                    {
                        if (chkNonDaConv.Checked)
                        {
                            result = result.Select(x => x).Where(x => x.Convertibile.Equals("1") || x.Convertibile.Equals("2")).ToList();
                        }
                        else
                        {

                            if (chkNonConvertibili.Checked)
                            {

                                result = result.Select(x => x).Where(x => x.Convertibile.Equals("1") || x.Convertibile.Equals("0")).ToList();
                            }
                            else
                            {
                                result = result.Select(x => x).Where(x => x.Convertibile.Equals("1")).ToList();
                            }
                        }
                    } //chkNonConvDaMe.Checked FALSE
                    else
                    {
                        if (this.chkNonDaConv.Checked)
                        {
                            if (chkNonConvertibili.Checked)
                            {
                                result = result.Select(x => x).Where(x => x.Convertibile.Equals("1") || x.Convertibile.Equals("2") || (x.Ammesso.Equals("0") && x.Convertibile.Equals("0"))).ToList();
                            }
                            else
                            {
                                result = result.Select(x => x).Where(x => x.Convertibile.Equals("1") || x.Convertibile.Equals("2")).ToList();
                            }

                        }
                        else
                        {
                            if (chkNonConvertibili.Checked)
                            {
                                result = result.Select(x => x).Where(x => x.Convertibile.Equals("1") || (x.Ammesso.Equals("0") && x.Convertibile.Equals("0"))).ToList();
                            }
                            else
                            {
                                result = result.Select(x => x).Where(x => x.Convertibile.Equals("1")).ToList();
                            }
                        }
                    }
                }//chkConv.checked = false
                else
                {
                    if (this.chkNonConvDaMe.Checked)
                    {
                        if (this.chkNonDaConv.Checked)
                        {
                            if (chkNonConvertibili.Checked)
                            {
                                result = result.Select(x => x).Where(x => (x.Convertibile.Equals("1") && !x.Modifica.Equals("2")) || x.Convertibile.Equals("2") || (x.Convertibile.Equals("0") && x.Ammesso.Equals("0"))).ToList();
                            }
                            else
                            {
                                result = result.Select(x => x).Where(x => (x.Convertibile.Equals("1") && !x.Modifica.Equals("2")) || x.Convertibile.Equals("2")).ToList();
                            }
                        }
                        else
                        {
                            if (chkNonConvertibili.Checked)
                            {
                                result = result.Select(x => x).Where(x => (x.Convertibile.Equals("0") && x.Ammesso.Equals("0")) || (x.Convertibile.Equals("1") && !x.Modifica.Equals("2"))).ToList();

                            }
                            else
                            {
                                result = result.Select(x => x).Where(x => (x.Convertibile.Equals("1") && !x.Modifica.Equals("2"))).ToList();
                            }
                        }
                    }
                    else
                    {
                        if (this.chkNonDaConv.Checked)
                        {
                            if (chkNonConvertibili.Checked)
                            { result = result.Select(x => x).Where(x => x.Convertibile.Equals("2") || (x.Convertibile.Equals("0") && x.Ammesso.Equals("0"))).ToList(); }
                            else
                            {
                                result = result.Select(x => x).Where(x => x.Convertibile.Equals("2")).ToList();
                            }
                        }
                        else
                        {
                            result = result.Select(x => x).Where(x => x.Convertibile.Equals("0") && x.Ammesso.Equals("0")).ToList();

                        }
                    }
                }

            }

            //In errore 
            if (result.Count > 0 && this.chkError.Checked)
            {
                result = result.Select(x => x).Where(x => x.Errore.Equals("1")).ToList();
            }


            return result;
        }

        private string getTipoFile(string p)
        {
            string result = string.Empty;
            switch (p)
            {
                case "A":
                    result = "Allegato";
                    break;
                case "P":
                    result = "Principale";
                    break;
            }
            return result;
        }

        private string getFlagResult(string p)
        {
            string result = string.Empty;
            switch (p)
            {
                case "0":
                    result = "No";
                    break;
                case "1":
                    result = "Si";
                    break;
                case "2":
                    result = "Non soggetto";
                    break;
            }
            return result;
        }

        private string getFlagDirittiResult(string p)
        {
            string result = string.Empty;
            switch (p)
            {
                case "0":
                    result = "Nessuno";
                    break;
                case "1":
                    result = "Lettura";
                    break;
                case "2":
                    result = "Scrittura";
                    break;
            }
            return result;
        }

        private void closePage()
        {
            this.listaDoc = null;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "closeAJM", "parent.closeAjaxModal('ReportFormatiConservazione','',parent );", true);

        }

        public override void VerifyRenderingInServerForm(Control control)
        {

        }

    }
}