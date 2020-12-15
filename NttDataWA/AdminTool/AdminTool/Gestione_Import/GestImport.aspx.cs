using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using System.IO;
using SAAdminTool.SiteNavigation;

namespace SAAdminTool.AdminTool.Gestione_Import
{
    public partial class GestImport : System.Web.UI.Page
    {

        protected SAAdminTool.DocsPaWR.DocsPaWebService wws = new SAAdminTool.DocsPaWR.DocsPaWebService();
        
        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager.GetCurrent(this).RegisterPostBackControl(this.btnImporta); 

            if (!IsPostBack)
            {
                GestioneGrafica();
                PopolaInfoUtente();
            }
        }

        protected void GestioneGrafica()
        {
            if (!IsPostBack)
            {
                lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "1");
                this.btnNuovo.Enabled = false;
                this.descrizione.Text = string.Empty;
            }
        }

        protected void btn_Importa_Click(object sender, EventArgs e)
        {
            bool controllo = true;

            if (uploadFile != null && !string.IsNullOrEmpty(uploadFile.Value))
            {
                //Nel CallContext inserisco nome del file(con estensione) a partire dal path del file di import
                importFileName=Path.GetFileName(uploadFile.Value);

                //Per togliere l'estensione:
                /*
                string ext = Path.GetExtension(importFileName);
                importFileName.Replace(ext, "");
                */

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
                    //utils.AlertPostLoad.GenericMessage(this, "Inserire un foglio excel valido");
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
                utils.AlertPostLoad.GenericMessage(this, "Nessun file excel valido selezionato.");
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
            DocsPaWR.EsitoImportPregressi retValue = wws.importPregresso(InfoUtente, fileToImport, true);

            return retValue;
        }

        private bool importa(DocsPaWR.InfoUtente infoUtente, DocsPaWR.EsitoImportPregressi esitoImport, string descrizione)
        {
            wws.Timeout = System.Threading.Timeout.Infinite;

            return wws.asyncImportPregresso(infoUtente, esitoImport, descrizione);
        }

        //private delegate bool asyncDelegate(DocsPaWR.InfoUtente infoUtente, DocsPaWR.EsitoImportPregressi esitoImport);

        //asyncDelegate mDelegate;

        protected void btn_Continua_Click(object sender, EventArgs e)
        {
            
            /*
            mDelegate = new asyncDelegate(importa);
            AsyncCallback callback = new AsyncCallback(asyncImport);

            mDelegate.BeginInvoke(InfoUtente, EsitoImport, callback, null);
            */

            //Andrea - descrizione report
            string descrizione = this.descrizione.Text.ToString();

            if (importa(InfoUtente, EsitoImport, descrizione))
            {
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
            else 
            {
                this.descrizione.Text = string.Empty;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Errori in importazione!');", true);
            }
        }

        //Andrea - Export ReportInErrore
        protected void btn_Esporta_Click(object sender, EventArgs e) 
        {
            this.Export(ReportInError);
        }

        protected void Export(List<DocsPaWR.ItemReportPregressi> reportInErr)
        {
            DocsPaWR.FileDocumento file = new SAAdminTool.DocsPaWR.FileDocumento();
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

        public SAAdminTool.DocsPaWR.FileDocumento GetFile(List<DocsPaWR.ItemReportPregressi> repInErr)
        {
            this.FileDocumento = wws.ExportReportExcel(repInErr.ToArray());
            return this.FileDocumento;
        }
        

        

        /*
        private void asyncImport(IAsyncResult ar)
        {
            bool bResult = mDelegate.EndInvoke(ar);

            string mess = string.Empty;

            if(ar.IsCompleted)
                mess = "Importazione eseguita correttamente.";
            else
                mess = "Errori nell'importazione.";

            lbl_alert.Text = mess;
        }
        */

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

        protected void ImageCreatedRender(Object sender, DataGridItemEventArgs e)
        {

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

        protected void btn_Nuovo_Click(object sender, EventArgs e)
        {
            this.pnlReport.Visible = false;
            this.btnNuovo.Enabled = false;
            this.btnImporta.Enabled = true;
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
            this.descrizione.Text = string.Empty;
        }


        protected void PopolaInfoUtente()
        {

            DocsPaWR.Utente ut = new SAAdminTool.DocsPaWR.Utente();

            string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            string codiceAmministrazione = amministrazione[0];
            string idAmministrazione = IdAmministrazione.ToString();

            ut.codiceAmm = codiceAmministrazione;
            ut.idAmministrazione = idAmministrazione;
            ut.tipoIE = "I";

            UserManager.setUtente(this, ut);

            DocsPaWR.Ruolo rl = new SAAdminTool.DocsPaWR.Ruolo();
            rl.codiceAmm = codiceAmministrazione;
            rl.idAmministrazione = idAmministrazione;
            rl.tipoIE = "I";
            rl.systemId = idAmministrazione;
            rl.uo = new SAAdminTool.DocsPaWR.UnitaOrganizzativa();
            rl.uo.codiceRubrica = codiceAmministrazione;
            UserManager.setRuolo(this, rl);

            this.InfoUtente = UserManager.getInfoUtente(this);
            this.InfoUtente.idPeople = "-1";
            this.InfoUtente.idGruppo = "-1";

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