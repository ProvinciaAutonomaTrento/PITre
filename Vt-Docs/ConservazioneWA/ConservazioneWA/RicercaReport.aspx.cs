using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ConservazioneWA.Utils;
using ConservazioneWA.DocsPaWR;
using System.Collections;
using System.Globalization;
using System.Text;
using System.IO;

namespace ConservazioneWA
{
    public partial class RicercaReport : System.Web.UI.Page
    {
        
        protected WSConservazioneLocale.InfoUtente infoUtente;
        protected DocsPaWR.InfoUtente infoUser;
        protected WSConservazioneLocale.InfoAmministrazione amm;

        

        

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;
            this.infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
            if (!IsPostBack)
            {
                GetLettereProtocolli();
                GestioneGrafica();

                GetTypeDocument();
                caricaComboTipoFileAcquisiti();
                this.RecordCount = 0;
                this.SelectedPage = 1;
                this.SearchFilters = null;
                this.Result = null;
                this.TemplateProf = null;
            }
        }

        public void Page_Prerender(object sender, EventArgs e)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session["fascicoli"] != null)
            {
                WSConservazioneLocale.Fascicolo[] projectList = HttpContext.Current.Session["fascicoli"] as WSConservazioneLocale.Fascicolo[];

                projectList = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void dgResult_ItemCommand(object sender, DataGridCommandEventArgs e)
        {
            Label lblSystemId = (Label)e.Item.FindControl("SYSTEM_ID");

            if (e.CommandName == "VISUALIZZA_REPORT_PDF")
            {
                ScriptManager.RegisterStartupScript(this,
                            this.GetType(),
                            "VISUALIZZA_REPORT_PDF",
                            string.Format("showFile('{0}', '0');", lblSystemId.Text),
                            true);
            }
            //else if (e.CommandName == "VISUALIZZA_FILE_CHIUSURA_XML")
            //{
            //    ScriptManager.RegisterStartupScript(this,
            //                this.GetType(),
            //                "VISUALIZZA_REPORT_PDF",
            //                string.Format("showFile('{0}', '1');", lblSystemId.Text),
            //                true);
            //}
            //else if (e.CommandName == "VISUALIZZA_FILE_CHIUSURA_P7M")
            //{
            //    ScriptManager.RegisterStartupScript(this,
            //this.GetType(),
            //"VISUALIZZA_REPORT_PDF",
            //string.Format("showFile('{0}', '2');", lblSystemId.Text),
            //true);
            //}
            //else if (e.CommandName == "VISUALIZZA_FILE_MARCATO")
            //{
            //    ScriptManager.RegisterStartupScript(this,
            //this.GetType(),
            //"VISUALIZZA_REPORT_PDF",
            //string.Format("showFile('{0}', '3');", lblSystemId.Text),
            //true);
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool IsRicercaNotifiche()
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void dgResult_PreRender(object sender, EventArgs e)
        {
            //foreach (DataGridItem item in this.dgResult.Items)
            //{
            //    Button btnVisualizzaReportPdf = (Button)item.FindControl("btnVisualizzaReportPdf");

            //    if (btnVisualizzaReportPdf != null)
            //    {
            //        Label lblSystemId = (Label)item.FindControl("SYSTEM_ID");

            //        btnVisualizzaReportPdf.OnClientClick = string.Format("showFile('{0}', '0');", lblSystemId.Text);
            //    }
            //}
        }

        protected void btnStampa_Click(object sender, EventArgs e)
        {
            
            WSConservazioneLocale.FiltroRicerca[][] qV = new WSConservazioneLocale.FiltroRicerca[1][];
            WSConservazioneLocale.FileDocumento fd = null;
            WSConservazioneLocale.InfoUtente infoUt = new WSConservazioneLocale.InfoUtente();
            if (this.RadioButtonReport.SelectedIndex.Equals(0))
            {
                string titolo = "Report istanze di conservazione";
                string rKey = "IstanzeConservazione";
                string cName = "ReportConservazione";
                
                if (GetFiltriRicerca1(ref qV))
                {
                    if (this.RadioButtonStampaReport.SelectedItem.Value.ToUpper().Equals("XLS"))
                    {
                        fd = ConservazioneManager.getFileReport(qV[0], "EXL", "EXL",titolo,rKey,cName, this.infoUtente);
                    }
                    else if (this.RadioButtonStampaReport.SelectedItem.Value.ToUpper().Equals("PDF"))
                    { 
                        fd = ConservazioneManager.getFileReport(qV[0], "PDF", "PDF", titolo,rKey,cName,this.infoUtente);
                    }
                    
                    
                }
               
            
            }

            else if (this.RadioButtonReport.SelectedIndex.Equals(1))
            {
                string titolo = "Report sulle verifiche in fase di presa in carico delle istanze";
                string rKey = "ReportVerificheFaseInPresa";
                string cName = "ReportConservazione";

                if (GetFiltriRicerca2(ref qV))
                {
                    if (this.RadioButtonStampaReport.SelectedItem.Value.ToUpper().Equals("XLS"))
                    {
                        fd = ConservazioneManager.getFileReport(qV[0], "EXL", "EXL", titolo, rKey, cName, this.infoUtente);
                    }
                    else if (this.RadioButtonStampaReport.SelectedItem.Value.ToUpper().Equals("PDF"))
                    {
                        fd = ConservazioneManager.getFileReport(qV[0], "PDF", "PDF", titolo, rKey, cName, this.infoUtente);
                    }


                }
            
            
            }

            else if (this.RadioButtonReport.SelectedIndex.Equals(2))
            {
                string titolo = "Verifiche di integrità e leggibilità delle istanze";
                string rKey = "VerifichePeriodiche";
                string cName = "ReportConservazione";

                if (GetFiltriRicerca3(ref qV))
                {
                    if (this.RadioButtonStampaReport.SelectedItem.Value.ToUpper().Equals("XLS"))
                    {
                        fd = ConservazioneManager.getFileReport(qV[0], "EXL", "EXL", titolo, rKey, cName, this.infoUtente);
                    }
                    else if (this.RadioButtonStampaReport.SelectedItem.Value.ToUpper().Equals("PDF"))
                    {
                        fd = ConservazioneManager.getFileReport(qV[0], "PDF", "PDF", titolo, rKey, cName, this.infoUtente);
                    }


                }

            }

            else if (this.RadioButtonReport.SelectedIndex.Equals(3))
            {
                string titolo = "Report sulla storia delle istanze";
                string rKey = "ReportStoriaIstanze";
                string cName = "ReportConservazione";

                if (GetFiltriRicerca4(ref qV))
                {
                    if (this.RadioButtonStampaReport.SelectedItem.Value.ToUpper().Equals("XLS"))
                    {
                        fd = ConservazioneManager.getFileReport(qV[0], "EXL", "EXL", titolo, rKey, cName, this.infoUtente);
                    }
                    else if (this.RadioButtonStampaReport.SelectedItem.Value.ToUpper().Equals("PDF"))
                    {
                        fd = ConservazioneManager.getFileReport(qV[0], "PDF", "PDF", titolo, rKey, cName, this.infoUtente);
                    }


                }

            }

            else if (this.RadioButtonReport.SelectedIndex.Equals(4))
            {
                string titolo = "Report sulla storia dei documenti";
                string rKey = "ReportStoriaDocumenti";
                string cName = "ReportConservazione";

                if (GetFiltriRicerca5(ref qV))
                {
                    if (this.RadioButtonStampaReport.SelectedItem.Value.ToUpper().Equals("XLS"))
                    {
                        fd = ConservazioneManager.getFileReport(qV[0], "EXL", "EXL", titolo, rKey, cName, this.infoUtente);
                    }
                    else if (this.RadioButtonStampaReport.SelectedItem.Value.ToUpper().Equals("PDF"))
                    {
                        fd = ConservazioneManager.getFileReport(qV[0], "PDF", "PDF", titolo, rKey, cName, this.infoUtente);
                    }


                }

            }



            //se fileDocumento esiste != null  -- richiama metodo/pagina per visualizzare il file
            if(fd!=null)
            {
            Session.Add("valoreContent", this.RadioButtonStampaReport.SelectedItem.Value.ToUpper());
            Session.Add("fileReport", fd);
            ClientScript.RegisterStartupScript(this.Page.GetType(), "", "window.open('PopUp/docVisualizzaReport.aspx','Graph','height=400,width=500,resizable=1');", true);
            
                
            }
           
        }

        private void GetLettereProtocolli()
        {

            //DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            //DocsPAWA.DocsPaWR.InfoUtente user = new InfoUtente();
            //this.etichette = wws.getEtichetteDocumenti(user, IdAmministrazione.ToString());
            //this.chk_Arr.Text = etichette[0].Etichetta; //Valore A
            //this.chk_Part.Text = etichette[1].Etichetta; //Valore P
            //this.chk_Int.Text = etichette[2].Etichetta;
            //this.chk_Grigio.Text = etichette[3].Etichetta;
            //if (!wws.IsInternalProtocolEnabled(IdAmministrazione.ToString()))
            //    this.chkList.Items.Remove(this.chkList.Items[2]);
          

        }






        protected void GestioneGrafica()
        {
            this.lblANotifiche.Visible = false;
            this.lbl_dCreazioneA.Visible = false;
            this.Label7.Visible = false;
            this.lbl_dCreazioneA1.Visible = false;
            this.Label8.Visible = false;
            this.lbl_dCreazioneA2.Visible = false;

            amm = Utils.ConservazioneManager.GetInfoAmmCorrente(infoUtente.idAmministrazione);
            this.lbl_amm.Text = this.amm.Codice + " - " + this.amm.Descrizione;
            //this.btnApriRubrica.Attributes.Add("onmouseover", "this.src='Img/rubrica_hover.gif'");
            // this.btnApriRubrica.Attributes.Add("onmouseout", "this.src='Img/rubrica.gif'");
            // this.btnApriRubrica.OnClientClick = String.Format("_ApriRubricaRicercaProprietario();");

        }


        /// Evento generato al cambio del testo nella casella del codice rubrica
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        

        protected void setDescCorr(string codRubrica)
        {

        }

        /// <summary>
        /// Evento generato al cambio del testo nella casella del codice rubrica
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       

        protected void setDescCorrMittDest(string codRubrica)
        {


        }

        /// <summary>
        /// Evento generato al cambio del testo nella casella del codice fascicolo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtCodFascicolo_TextChanged(object sender, EventArgs e)
        {

        }

        protected void SearchProject()
        {

        }

        protected void GetTypeDocument()
        {

            

        }

        protected void caricaComboTipoFileAcquisiti()
        {

        }

        protected bool GetFiltriRicerca1(ref WSConservazioneLocale.FiltroRicerca[][] qV )
        {
            //qV = new WSConservazioneLocale.FiltroRicerca[1][];
            qV[0] = new WSConservazioneLocale.FiltroRicerca[1];
            WSConservazioneLocale.FiltroRicerca[] fVList = new WSConservazioneLocale.FiltroRicerca[0];
            WSConservazioneLocale.FiltroRicerca fV1 = null;

            #region ddl data invio 

            if (this.ddl_invio1.SelectedIndex == 0)
            {//valore singolo carico DATA_INVIO_IL
                if (!this.GetCalendarControl("lbl_dCreazioneDa").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneDa").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_INVIO_IL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneDa").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            if (this.ddl_invio1.SelectedIndex == 1)
            {//intervallo DATA_INVIO_DAL,DATA_INVIO_AL
                if (!this.GetCalendarControl("lbl_dCreazioneDa").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneDa").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_INVIO_DAL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneDa").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }

                if (!this.GetCalendarControl("lbl_dCreazioneA").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneA").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_INVIO_AL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneA").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            if (this.ddl_invio1.SelectedIndex == 2)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_INVIO_TODAY.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.ddl_invio1.SelectedIndex == 3)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_INVIO_SC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.ddl_invio1.SelectedIndex == 4)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_INVIO_MC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion

            #region ddl data chiusura

            if (this.ddl_chiusura1.SelectedIndex == 0)
            {//valore singolo carico DATA_CHIUSURA_IL
                if (!this.GetCalendarControl("lbl_dCreazioneDa1").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneDa1").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_CHIUSURA_IL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneDa1").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            if (this.ddl_chiusura1.SelectedIndex == 1)
            {//intervallo DATA_CHIUSURA_DAL,DATA_CHIUSURA_AL
                if (!this.GetCalendarControl("lbl_dCreazioneDa1").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneDa1").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_CHIUSURA_DAL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneDa1").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }

                if (!this.GetCalendarControl("lbl_dCreazioneA1").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneA1").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_CHIUSURA_AL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneA1").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            if (this.ddl_chiusura1.SelectedIndex == 2)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_CHIUSURA_TODAY.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.ddl_chiusura1.SelectedIndex == 3)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_CHIUSURA_SC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.ddl_chiusura1.SelectedIndex == 4)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_CHIUSURA_MC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }
            
            #endregion

            #region ddl data rifiuto

            if (this.ddl_rifiuto1.SelectedIndex == 0)
            {//valore singolo carico DATA_RIFIUTO_IL
                if (!this.GetCalendarControl("lbl_dCreazioneDa2").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneDa2").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_RIFIUTO_IL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneDa2").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            if (this.ddl_rifiuto1.SelectedIndex == 1)
            {//intervallo DATA_RIFIUTO_DAL,DATA_RIFIUTO_AL
                if (!this.GetCalendarControl("lbl_dCreazioneDa2").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneDa2").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_RIFIUTO_DAL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneDa2").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }

                if (!this.GetCalendarControl("lbl_dCreazioneA2").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneA2").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_RIFIUTO_AL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneA2").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            if (this.ddl_rifiuto1.SelectedIndex == 2)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_RIFIUTO_TODAY.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.ddl_rifiuto1.SelectedIndex == 3)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_RIFIUTO_SC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.ddl_rifiuto1.SelectedIndex == 4)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_RIFIUTO_SC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            #endregion
            qV[0] = fVList;
            

            return true;
        }

        protected bool GetFiltriRicerca2(ref WSConservazioneLocale.FiltroRicerca[][] qV)
        {
           
            qV[0] = new WSConservazioneLocale.FiltroRicerca[1];
            WSConservazioneLocale.FiltroRicerca[] fVList = new WSConservazioneLocale.FiltroRicerca[0];
            WSConservazioneLocale.FiltroRicerca fV1 = null;

            #region ddl id istanza
            if (ddl_idIstanza.SelectedIndex == 0)
            {
                if (this.txt_initIdIst.Text != null && !this.txt_initIdIst.Text.Equals(""))
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.ID_ISTANZA.ToString();
                    fV1.valore = this.txt_initIdIst.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            else if(ddl_idIstanza.SelectedIndex == 1)
            {
                //valore singolo carico NUM_PROTOCOLLO_DAL - NUM_PROTOCOLLO_AL
                    if (!this.txt_initIdIst.Text.Equals(""))
                    {
                        fV1 = new WSConservazioneLocale.FiltroRicerca();
                        fV1.argomento = WSConservazioneLocale.FiltriConservazione.ID_ISTANZA_DAL.ToString();
                        fV1.valore = this.txt_initIdIst.Text;
                        fVList = addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.txt_fineIdIst.Text.Equals(""))
                    {
                        fV1 = new WSConservazioneLocale.FiltroRicerca();
                        fV1.argomento = WSConservazioneLocale.FiltriConservazione.ID_ISTANZA_AL.ToString();
                        fV1.valore = this.txt_fineIdIst.Text;
                        fVList = addToArrayFiltroRicerca(fVList, fV1);
                    }
                
            }
            #endregion

            #region ddl data invio conservazione
            if (this.ddl_invioc.SelectedIndex == 0)
            {//valore singolo carico DATA_INVIO_IL
                if (!this.GetCalendarControl("lbl_dCreazioneDa4").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneDa4").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_INVIO_IL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneDa4").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            if (this.ddl_invioc.SelectedIndex == 1)
            {//intervallo DATA_INVIO_DAL,DATA_INVIO_AL
                if (!this.GetCalendarControl("lbl_dCreazioneDa4").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneDa4").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_INVIO_DAL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneDa4").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }

                if (!this.GetCalendarControl("lbl_dCreazioneA4").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneA4").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_INVIO_AL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneA4").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            if (this.ddl_invioc.SelectedIndex == 2)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_INVIO_TODAY.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.ddl_invioc.SelectedIndex == 3)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_INVIO_SC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.ddl_invioc.SelectedIndex == 4)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_INVIO_MC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            #endregion
            #region ddl data chiusura 2

            if (this.ddl_chiusura2.SelectedIndex == 0)
            {//valore singolo carico DATA_CHIUSURA_IL
                if (!this.GetCalendarControl("lbl_dCreazioneDa5").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneDa5").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_CHIUSURA_IL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneDa5").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            if (this.ddl_chiusura2.SelectedIndex == 1)
            {//intervallo DATA_CHIUSURA_DAL,DATA_CHIUSURA_AL
                if (!this.GetCalendarControl("lbl_dCreazioneDa5").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneDa5").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_CHIUSURA_DAL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneDa5").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }

                if (!this.GetCalendarControl("lbl_dCreazioneA5").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneA5").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_CHIUSURA_AL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneA5").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            if (this.ddl_chiusura2.SelectedIndex == 2)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_CHIUSURA_TODAY.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.ddl_chiusura2.SelectedIndex == 3)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_CHIUSURA_SC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.ddl_chiusura2.SelectedIndex == 4)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_CHIUSURA_MC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            #endregion

            #region ddl data rifiuto 2

            if (this.ddl_rifiuto2.SelectedIndex == 0)
            {//valore singolo carico DATA_RIFIUTO_IL
                if (!this.GetCalendarControl("lbl_dCreazioneDa6").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneDa6").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_RIFIUTO_IL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneDa6").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            if (this.ddl_rifiuto2.SelectedIndex == 1)
            {//intervallo DATA_RIFIUTO_DAL,DATA_RIFIUTO_AL
                if (!this.GetCalendarControl("lbl_dCreazioneDa6").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneDa6").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_RIFIUTO_DAL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneDa6").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }

                if (!this.GetCalendarControl("lbl_dCreazioneA6").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneA6").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_RIFIUTO_AL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneA6").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            if (this.ddl_rifiuto2.SelectedIndex == 2)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_RIFIUTO_TODAY.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.ddl_rifiuto2.SelectedIndex == 3)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_RIFIUTO_SC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.ddl_rifiuto2.SelectedIndex == 4)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_RIFIUTO_MC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            #endregion
            qV[0] = fVList;


            return true;
        }

        protected bool GetFiltriRicerca3(ref WSConservazioneLocale.FiltroRicerca[][] qV)
        {
             
            qV[0] = new WSConservazioneLocale.FiltroRicerca[1];
            WSConservazioneLocale.FiltroRicerca[] fVList = new WSConservazioneLocale.FiltroRicerca[0];
            WSConservazioneLocale.FiltroRicerca fV1 = null;

            #region ddl id istanza2
            if (ddl_idIstanza2.SelectedIndex == 0) {
            if (this.TextBox1.Text != null && !this.TextBox1.Text.Equals(""))
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.ID_ISTANZA.ToString();
                fV1.valore = this.TextBox1.Text;
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            }
            else if (ddl_idIstanza2.SelectedIndex == 1) 
            {//valore singolo carico NUM_PROTOCOLLO_DAL - NUM_PROTOCOLLO_AL
                if (!this.TextBox1.Text.Equals(""))
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.ID_ISTANZA_DAL.ToString();
                    fV1.valore = this.TextBox1.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
                if (!this.TextBox2.Text.Equals(""))
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.ID_ISTANZA_AL.ToString();
                    fV1.valore = this.TextBox2.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion

            #region ddl data invio conservazione 3
            if (this.ddl_invio3.SelectedIndex == 0)
            {//valore singolo carico DATA_INVIO_IL
                if (!this.GetCalendarControl("lbl_dCreazioneDa7").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneDa7").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_INVIO_IL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneDa7").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            if (this.ddl_invio3.SelectedIndex == 1)
            {//intervallo DATA_INVIO_DAL,DATA_INVIO_AL
                if (!this.GetCalendarControl("lbl_dCreazioneDa7").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneDa7").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_INVIO_DAL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneDa7").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }

                if (!this.GetCalendarControl("lbl_dCreazioneA7").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneA7").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_INVIO_AL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneA7").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            if (this.ddl_invio3.SelectedIndex == 2)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_INVIO_TODAY.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.ddl_invio3.SelectedIndex == 3)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_INVIO_SC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.ddl_invio3.SelectedIndex == 4)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_INVIO_MC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            #endregion
            #region ddl data chiusura 3

            if (this.ddl_chiusura3.SelectedIndex == 0)
            {//valore singolo carico DATA_CHIUSURA_IL
                if (!this.GetCalendarControl("lbl_dCreazioneDa8").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneDa8").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_CHIUSURA_IL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneDa8").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            if (this.ddl_chiusura3.SelectedIndex == 1)
            {//intervallo DATA_CHIUSURA_DAL,DATA_CHIUSURA_AL
                if (!this.GetCalendarControl("lbl_dCreazioneDa8").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneDa8").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_CHIUSURA_DAL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneDa8").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }

                if (!this.GetCalendarControl("lbl_dCreazioneA8").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneA8").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_CHIUSURA_AL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneA8").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            if (this.ddl_chiusura3.SelectedIndex == 2)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_CHIUSURA_TODAY.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.ddl_chiusura3.SelectedIndex == 3)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_CHIUSURA_SC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.ddl_chiusura3.SelectedIndex == 4)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_CHIUSURA_MC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            #endregion
            #region tipo verifica
            if (cboTipiConservazione.SelectedIndex != 0 && !string.IsNullOrEmpty(cboTipiConservazione.Text))
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.TIPO_DI_VERIFICA.ToString();
                fV1.valore = this.cboTipiConservazione.SelectedItem.Value.ToString();
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            //else if (cboTipiConservazione.SelectedIndex == 2)
            //{
            //    fV1 = new WSConservazioneLocale.FiltroRicerca();
            //    fV1.argomento = WSConservazioneLocale.FiltriConservazione.TIPO_DI_VERIFICA.ToString();
            //    fV1.valore = this.cboTipiConservazione.SelectedItem.Value.ToString();
            //    fVList = addToArrayFiltroRicerca(fVList, fV1);
            //}

            //else if (cboTipiConservazione.SelectedIndex == 3)
            //{
            //    fV1 = new WSConservazioneLocale.FiltroRicerca();
            //    fV1.argomento = WSConservazioneLocale.FiltriConservazione.TIPO_DI_VERIFICA.ToString();
            //    fV1.valore = this.cboTipiConservazione.SelectedItem.Value.ToString();
            //    fVList = addToArrayFiltroRicerca(fVList, fV1);
            //}
            #endregion
            #region id supporto
            if (ddl_idSupporto.SelectedIndex == 0) {

                if (this.TextBox3.Text != null && !this.TextBox3.Text.Equals(""))
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.ID_SUPPORTO.ToString();
                    fV1.valore = this.TextBox3.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            

            else if (ddl_idSupporto.SelectedIndex == 1)
            {//intervallo id supporto
                if (!this.TextBox3.Text.Equals(""))
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.ID_SUPPORTO_DAL.ToString();
                    fV1.valore = this.TextBox3.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
                if (!this.TextBox4.Text.Equals(""))
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.ID_SUPPORTO_AL.ToString();
                    fV1.valore = this.TextBox4.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion
            #region ddl data generazione supporto
            if (this.ddl_generazioneSupporto.SelectedIndex == 0)
            {//valore singolo carico DATA_INVIO_IL
                if (!this.GetCalendarControl("lbl_dCreazioneDa10").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneDa10").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_GENERAZIONE_IL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneDa10").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            if (this.ddl_generazioneSupporto.SelectedIndex == 1)
            {//intervallo DATA_INVIO_DAL,DATA_INVIO_AL
                if (!this.GetCalendarControl("lbl_dCreazioneDa10").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneDa10").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_GENERAZIONE_DAL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneDa10").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }

                if (!this.GetCalendarControl("lbl_dCreazioneA10").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneA10").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_GENERAZIONE_AL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneA10").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            if (this.ddl_generazioneSupporto.SelectedIndex == 2)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_GENERAZIONE_TODAY.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.ddl_generazioneSupporto.SelectedIndex == 3)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_GENERAZIONE_SC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.ddl_generazioneSupporto.SelectedIndex == 4)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_GENERAZIONE_MC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            #endregion
            #region data esecuzione verifica
            if (this.ddl_esecuzioneVerifica.SelectedIndex == 0)
            {//valore singolo carico DATA_INVIO_IL
                if (!this.GetCalendarControl("lbl_dCreazioneDa11").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneDa11").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_ES_VERIFICA_IL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneDa11").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            if (this.ddl_esecuzioneVerifica.SelectedIndex == 1)
            {//intervallo DATA_INVIO_DAL,DATA_INVIO_AL
                if (!this.GetCalendarControl("lbl_dCreazioneDa11").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneDa11").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_ES_VERIFICA_DAL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneDa11").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }

                if (!this.GetCalendarControl("lbl_dCreazioneA11").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneA11").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_ES_VERIFICA_AL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneA11").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            if (this.ddl_esecuzioneVerifica.SelectedIndex == 2)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_ES_VERIFICA_TODAY.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.ddl_esecuzioneVerifica.SelectedIndex == 3)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_ES_VERIFICA_SC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.ddl_esecuzioneVerifica.SelectedIndex == 4)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_ES_VERIFICA_MC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }
            
            #endregion
            #region esito della verifica
            if (DropDownList14.SelectedIndex != 0)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.ESITO_VERIFICA.ToString();
                fV1.valore = this.DropDownList14.SelectedItem.Value.ToString();
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion  
            qV[0] = fVList;
            
            return true;
        }

        protected bool GetFiltriRicerca4(ref WSConservazioneLocale.FiltroRicerca[][] qV)
        {
             
            qV[0] = new WSConservazioneLocale.FiltroRicerca[1];
            WSConservazioneLocale.FiltroRicerca[] fVList = new WSConservazioneLocale.FiltroRicerca[0];
            WSConservazioneLocale.FiltroRicerca fV1 = null;

            #region ddl id istanza3
            if (ddl_idIstanza3.SelectedIndex == 0)
            {
                if (this.TextBox5.Text != null && !this.TextBox5.Text.Equals(""))
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.ID_ISTANZA.ToString();
                    fV1.valore = this.TextBox5.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            else 
            if (ddl_idIstanza3.SelectedIndex == 1)
            {//valore singolo carico NUM_PROTOCOLLO_DAL - NUM_PROTOCOLLO_AL
                if (!this.TextBox5.Text.Equals(""))
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.ID_ISTANZA_DAL.ToString();
                    fV1.valore = this.TextBox5.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
                if (!this.TextBox6.Text.Equals(""))
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.ID_ISTANZA_AL.ToString();
                    fV1.valore = this.TextBox6.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion

            #region ddl data invio conservazione 3
            if (this.ddl_invioc3.SelectedIndex == 0)
            {//valore singolo carico DATA_INVIO_IL
                if (!this.GetCalendarControl("lbl_dCreazioneDa12").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneDa12").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_INVIO_IL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneDa12").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            if (this.ddl_invioc3.SelectedIndex == 1)
            {//intervallo DATA_INVIO_DAL,DATA_INVIO_AL
                if (!this.GetCalendarControl("lbl_dCreazioneDa12").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneDa12").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_INVIO_DAL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneDa12").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }

                if (!this.GetCalendarControl("lbl_dCreazioneA12").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneA12").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_INVIO_AL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneA12").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            if (this.ddl_invioc3.SelectedIndex == 2)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_INVIO_TODAY.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.ddl_invioc3.SelectedIndex == 3)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_INVIO_SC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.ddl_invioc3.SelectedIndex == 4)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_INVIO_MC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            #endregion
            #region ddl data chiusura 4

            if (this.ddl_chiusura4.SelectedIndex == 0)
            {//valore singolo carico DATA_CHIUSURA_IL
                if (!this.GetCalendarControl("lbl_dCreazioneDa13").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneDa13").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_CHIUSURA_IL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneDa13").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            if (this.ddl_chiusura4.SelectedIndex == 1)
            {//intervallo DATA_CHIUSURA_DAL,DATA_CHIUSURA_AL
                if (!this.GetCalendarControl("lbl_dCreazioneDa13").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneDa13").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_CHIUSURA_DAL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneDa13").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }

                if (!this.GetCalendarControl("lbl_dCreazioneA13").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneA13").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_CHIUSURA_AL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneA13").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            if (this.ddl_chiusura4.SelectedIndex == 2)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_CHIUSURA_TODAY.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.ddl_chiusura4.SelectedIndex == 3)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_CHIUSURA_SC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.ddl_chiusura4.SelectedIndex == 4)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_CHIUSURA_MC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            #endregion
            #region ddl data rifiuto 3
            if (this.ddl_rifiuto3.SelectedIndex == 0)
            {//valore singolo carico DATA_RIFIUTO_IL
                if (!this.GetCalendarControl("lbl_dCreazioneDa14").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneDa14").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_RIFIUTO_IL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneDa14").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            if (this.ddl_rifiuto3.SelectedIndex == 1)
            {//intervallo DATA_RIFIUTO_DAL,DATA_RIFIUTO_AL
                if (!this.GetCalendarControl("lbl_dCreazioneDa14").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneDa14").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_RIFIUTO_DAL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneDa14").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }

                if (!this.GetCalendarControl("lbl_dCreazioneA14").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneA14").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_RIFIUTO_AL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneA14").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            if (this.ddl_rifiuto3.SelectedIndex == 2)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_RIFIUTO_TODAY.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.ddl_rifiuto3.SelectedIndex == 3)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_RIFIUTO_SC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.ddl_rifiuto3.SelectedIndex == 4)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_RIFIUTO_MC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion
            qV[0] = fVList;

            return true;
        }

        protected bool GetFiltriRicerca5(ref WSConservazioneLocale.FiltroRicerca[][] qV)
        {
             
            qV[0] = new WSConservazioneLocale.FiltroRicerca[1];
            WSConservazioneLocale.FiltroRicerca[] fVList = new WSConservazioneLocale.FiltroRicerca[0];
            WSConservazioneLocale.FiltroRicerca fV1 = null;

            #region ddl id istanza4
            if (ddl_idIstanza4.SelectedIndex == 0)
            {
                if (this.TextBox7.Text != null && !this.TextBox7.Text.Equals(""))
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.ID_ISTANZA.ToString();
                    fV1.valore = this.TextBox7.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            else if(ddl_idIstanza4.SelectedIndex==1)
            {//valore singolo carico NUM_PROTOCOLLO_DAL - NUM_PROTOCOLLO_AL
                if (!this.TextBox7.Text.Equals(""))
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.ID_ISTANZA_DAL.ToString();
                    fV1.valore = this.TextBox7.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
                if (!this.TextBox8.Text.Equals(""))
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.ID_ISTANZA_AL.ToString();
                    fV1.valore = this.TextBox8.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion

            #region ddl data invio conservazione 5
            if (this.ddl_invioc5.SelectedIndex == 0)
            {//valore singolo carico DATA_INVIO_IL
                if (!this.GetCalendarControl("lbl_dCreazioneDa16").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneDa16").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_INVIO_IL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneDa16").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            if (this.ddl_invioc5.SelectedIndex == 1)
            {//intervallo DATA_INVIO_DAL,DATA_INVIO_AL
                if (!this.GetCalendarControl("lbl_dCreazioneDa16").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneDa16").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_INVIO_DAL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneDa16").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }

                if (!this.GetCalendarControl("lbl_dCreazioneA16").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneA16").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_INVIO_AL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneA16").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            if (this.ddl_invioc5.SelectedIndex == 2)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_INVIO_TODAY.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.ddl_invioc5.SelectedIndex == 3)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_INVIO_SC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.ddl_invioc5.SelectedIndex == 4)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_INVIO_MC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            #endregion
            #region ddl data chiusura 5

            if (this.ddl_chiusura5.SelectedIndex == 0)
            {//valore singolo carico DATA_CHIUSURA_IL
                if (!this.GetCalendarControl("lbl_dCreazioneDa15").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneDa15").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_CHIUSURA_IL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneDa15").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            if (this.ddl_chiusura5.SelectedIndex == 1)
            {//intervallo DATA_CHIUSURA_DAL,DATA_CHIUSURA_AL
                if (!this.GetCalendarControl("lbl_dCreazioneDa15").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneDa15").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_CHIUSURA_DAL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneDa15").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }

                if (!this.GetCalendarControl("lbl_dCreazioneA15").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dCreazioneA15").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_CHIUSURA_AL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dCreazioneA15").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            if (this.ddl_chiusura5.SelectedIndex == 2)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_CHIUSURA_TODAY.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.ddl_chiusura5.SelectedIndex == 3)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_CHIUSURA_SC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.ddl_chiusura5.SelectedIndex == 4)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriConservazione.DATA_CHIUSURA_MC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            #endregion
            #region ddl id documento
            if (ddl_idDocumento.SelectedIndex == 0)
            {
                if (this.TextBox9.Text != null && !this.TextBox9.Text.Equals(""))
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DOCUMENTO.ToString();
                    fV1.valore = this.TextBox9.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            else if(ddl_idDocumento.SelectedIndex==1)
            {//valore singolo carico NUM_PROTOCOLLO_DAL - NUM_PROTOCOLLO_AL
                if (!this.TextBox9.Text.Equals(""))
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DOCUMENTO_DAL.ToString();
                    fV1.valore = this.TextBox9.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
                if (!this.TextBox8.Text.Equals(""))
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.DOCUMENTO_AL.ToString();
                    fV1.valore = this.TextBox10.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion

            #region ddl protocollo
            if (ddl_protocollo.SelectedIndex == 0)
            {
                if (this.TextBox11.Text != null && !this.TextBox11.Text.Equals(""))
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.PROTOCOLLO.ToString();
                    fV1.valore = this.TextBox11.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            else
            {//valore singolo carico NUM_PROTOCOLLO_DAL - NUM_PROTOCOLLO_AL
                if (!this.TextBox11.Text.Equals(""))
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.PROTOCOLLO_DAL.ToString();
                    fV1.valore = this.TextBox11.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
                if (!this.TextBox12.Text.Equals(""))
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriConservazione.PROTOCOLLO_AL.ToString();
                    fV1.valore = this.TextBox12.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            #endregion
            qV[0] = fVList;

            return true;
        }

        public static WSConservazioneLocale.FiltroRicerca[] addToArrayFiltroRicerca(WSConservazioneLocale.FiltroRicerca[] array, WSConservazioneLocale.FiltroRicerca nuovoElemento)
        {
            WSConservazioneLocale.FiltroRicerca[] nuovaLista;
            if (array != null)
            {
                int len = array.Length;
                nuovaLista = new WSConservazioneLocale.FiltroRicerca[len + 1];
                array.CopyTo(nuovaLista, 0);
                nuovaLista[len] = nuovoElemento;
                return nuovaLista;
            }
            else
            {
                nuovaLista = new WSConservazioneLocale.FiltroRicerca[1];
                nuovaLista[0] = nuovoElemento;
                return nuovaLista;
            }
        }

        /// 
        /// </summary>
        /// <param name="controlId"></param>
        /// <returns></returns>
        protected ConservazioneWA.UserControl.Calendar GetCalendarControl(string controlId)
        {
            return (ConservazioneWA.UserControl.Calendar)this.FindControl(controlId);
        }

        public static bool isDate(string data)
        {
            try
            {
                data = data.Trim();
                CultureInfo ci = new CultureInfo("it-IT");
                string[] formati = { "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy H:mm:ss", "dd/MM/yyyy" };
                DateTime d_ap = DateTime.ParseExact(data, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string DO_AdattaString(string valore)
        {
            valore = valore.Trim();
            valore = valore.Replace("\r", "");
            valore = valore.Replace("\n", "");
            return valore;
        }

        /// <summary>
        /// Funzione per la ricerca dei documenti
        /// </summary>
        /// <param name="recordNumber">Numero di record restituiti dalla ricerca</param>
        protected WSConservazioneLocale.SearchObject[] SearchDocument(WSConservazioneLocale.FiltroRicerca[][] searchFilters, int selectedPage, out int recordNumber)
        {
            // Documenti individuati dalla ricerca
            WSConservazioneLocale.SearchObject[] documents;

            // Numero totale di pagine
            int pageNumbers;

            // Lista dei system id dei documenti restituiti dalla ricerca
            WSConservazioneLocale.SearchResultInfo[] idProfiles = null;

            documents = ConservazioneManager.getQueryInfoDocumentoPagingCustom(infoUtente, searchFilters, this.SelectedPage, out pageNumbers, out recordNumber, false, !IsPostBack, false, 10, false, null, null, out idProfiles);

            // Memorizzazione del numero di risultati restituiti dalla ricerca, del numero di pagine e dei risultati
            this.RecordCount = recordNumber;
            this.PageCount = pageNumbers;
            this.Result = documents;

            return documents;
        }

       

        protected String GetSystemID(WSConservazioneLocale.SearchObject temp)
        {
            return temp.SearchObjectID;
        }

        protected String GetDataCreazione(WSConservazioneLocale.SearchObject temp)
        {
            return temp.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D9")).FirstOrDefault().SearchObjectFieldValue;
        }

        protected String GetTipo(WSConservazioneLocale.SearchObject temp)
        {
            string value = temp.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D3")).FirstOrDefault().SearchObjectFieldValue;

            if (value == "G")
                value = "NP";

            return value;
        }

        protected String GetIdSegnatura(WSConservazioneLocale.SearchObject doc)
        {
            string result = string.Empty;
            StringBuilder temp;
            string numeroDocumento = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D1")).FirstOrDefault().SearchObjectFieldValue;
            string numeroProtocollo = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D12")).FirstOrDefault().SearchObjectFieldValue;
            string dataProtocollo = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D9")).FirstOrDefault().SearchObjectFieldValue;
            string dataApertura = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D9")).FirstOrDefault().SearchObjectFieldValue;
            string protTit = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("PROT_TIT")).FirstOrDefault().SearchObjectFieldValue;

            temp = new StringBuilder("<span style=\"color:");
            // Se il documento è un protocollo viene colorato in rosso altrimenti
            // viene colorato in nero
            temp.Append(String.IsNullOrEmpty(numeroProtocollo) ? "#333333" : "Red");
            // Il testo deve essere grassetto
            temp.Append("; font-weight:bold;\">");

            // Creazione dell'informazione sul documento
            if (!String.IsNullOrEmpty(numeroProtocollo))
            {
                temp.Append(numeroProtocollo);
                temp.Append("<br />");
                temp.Append(dataProtocollo);
            }
            else
            {
                temp.Append(numeroDocumento);
                temp.Append("<br />");
                temp.Append(dataApertura);
            }


            if (!String.IsNullOrEmpty(protTit))
                temp.Append("<br />" + protTit);

            // Chiusura del tag span
            temp.Append("</span>");

            result = temp.ToString();

            return result;
        }

        protected String GetOggetto(WSConservazioneLocale.SearchObject temp)
        {
            return temp.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D4")).FirstOrDefault().SearchObjectFieldValue;
        }

        protected String GetRegistro(WSConservazioneLocale.SearchObject temp)
        {
            return temp.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D2")).FirstOrDefault().SearchObjectFieldValue;
        }

        protected String GetTipoFile(WSConservazioneLocale.SearchObject temp)
        {
            return temp.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D23")).FirstOrDefault().SearchObjectFieldValue;
        }

        protected String GetTipologia(WSConservazioneLocale.SearchObject temp)
        {
            return temp.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("U1")).FirstOrDefault().SearchObjectFieldValue;
        }

        protected String GetIstanze(WSConservazioneLocale.SearchObject temp)
        {
            string istanze = string.Empty;

            WSConservazioneLocale.SearchObjectField field = temp.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ISTANZECONSERVAZIONE")).FirstOrDefault();

            if (field != null)
            {
                string totaleIstanze = field.SearchObjectFieldValue;

                if (!string.IsNullOrEmpty(totaleIstanze))
                {
                    string[] singoleIstanze = totaleIstanze.Split('-');
                    if (singoleIstanze != null)
                    {
                        istanze = "<ul class=\"link_istanze\">";
                        for (int i = 0; i < singoleIstanze.Length; i++)
                        {
                            if (i != singoleIstanze.Length - 1)
                            {
                                istanze += "<li class=\"spazioLi\"><a href=\"RicercaIstanze.aspx?id=" + singoleIstanze[i] + "\" title=\"Istanza numero " + singoleIstanze[i] + "\">" + singoleIstanze[i] + "</a></li>";
                            }
                            else
                            {
                                istanze += "<li><a href=\"RicercaIstanze.aspx?id=" + singoleIstanze[i] + "\" title=\"Istanza numero " + singoleIstanze[i] + "\">" + singoleIstanze[i] + "</a></li>";
                            }
                        }
                        istanze += "</ul>";
                    }
                }
            }

            return istanze;
        }

        protected void ChangeTypeDocument(object sender, EventArgs e)
        {

        }

        protected void ViewCampiProlilati(object sender, EventArgs e)
        {

        }

        protected string UrlCampiProfilati
        {
            get
            {
                return "PopUp/CampiProfilati.aspx?type=D";
            }

        }

        protected string UrlChooseProject
        {
            get
            {
                return "ChooseProject.aspx";
            }

        }

        protected int SelectedPage
        {
            get
            {
                int toReturn = 20;
                if (HttpContext.Current.Session["selectedPage"] != null)
                    toReturn = Convert.ToInt32(HttpContext.Current.Session["selectedPage"].ToString());

                return toReturn;
            }

            set
            {
                HttpContext.Current.Session["selectedPage"] = value;
            }
        }

        protected int RecordCount
        {
            get
            {
                int toReturn = 20;
                if (HttpContext.Current.Session["recordCount"] != null)
                    toReturn = Convert.ToInt32(HttpContext.Current.Session["recordCount"].ToString());

                return toReturn;
            }

            set
            {
                HttpContext.Current.Session["recordCount"] = value;
            }
        }

        protected WSConservazioneLocale.FiltroRicerca[][] SearchFilters
        {
            get
            {
                return HttpContext.Current.Session["searchFilters"] as WSConservazioneLocale.FiltroRicerca[][];

            }

            set
            {
                HttpContext.Current.Session["searchFilters"] = value;
            }
        }

        /// <summary>
        /// Numero di pagine restituiti dalla ricerca
        /// </summary>
        protected int PageCount
        {
            get
            {
                int toReturn = 1;

                if (HttpContext.Current.Session["PageCount"] != null &&
                    Int32.TryParse(
                        HttpContext.Current.Session["PageCount"].ToString(),
                        out toReturn)) ;

                return toReturn;
            }

            set
            {
                HttpContext.Current.Session["PageCount"] = value;
            }
        }

        /// <summary>
        /// Risultati restituiti dalla ricerca.
        /// </summary>
        public WSConservazioneLocale.SearchObject[] Result
        {
            get
            {
                return HttpContext.Current.Session["Result"] as WSConservazioneLocale.SearchObject[];
            }
            set
            {
                HttpContext.Current.Session["Result"] = value;
            }
        }

        /// <summary>
        /// Template selezionato
        /// </summary>
        protected WSConservazioneLocale.Templates TemplateProf
        {
            get
            {
                return HttpContext.Current.Session["TemplateProf"] as WSConservazioneLocale.Templates;
            }
            set
            {
                HttpContext.Current.Session["TemplateProf"] = value;
            }
        }


        //codice aggiunto da C.Fuccia per la ricerca dei report
        protected void RadioButtonReport_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.RadioButtonReport.SelectedIndex.Equals(0))
            {

                this.Fieldset5.Visible = false;
                this.Fieldset3.Visible = false;
                this.Fieldset4.Visible = false;
                this.Fieldset2.Visible = false;
                this.pnl_prova5.Visible = false;
                this.pnl_prova4.Visible = false;
                this.pnl_prova3.Visible = false;
                this.pnl_prova2.Visible = false;
                this.Fieldset1.Visible = true;
                this.pnl_prova.Visible = true;



            }
            else
                if (this.RadioButtonReport.SelectedIndex.Equals(1))
                {
                    this.Fieldset5.Visible = false;
                    this.Fieldset3.Visible = false;
                    this.Fieldset4.Visible = false;
                    this.Fieldset1.Visible = false;
                    this.Fieldset2.Visible = true;
                    this.pnl_prova5.Visible = false;
                    this.pnl_prova.Visible = false;
                    this.pnl_prova3.Visible = false;
                    this.pnl_prova4.Visible = false;
                    this.pnl_prova2.Visible = true;


                }

                else
                    if (this.RadioButtonReport.SelectedIndex.Equals(2))
                    {
                        this.Fieldset5.Visible = false;
                        this.Fieldset2.Visible = false;
                        this.Fieldset4.Visible = false;
                        this.Fieldset1.Visible = false;
                        this.Fieldset3.Visible = true;
                        this.pnl_prova.Visible = false;
                        this.pnl_prova2.Visible = false;
                        this.pnl_prova4.Visible = false;
                        this.pnl_prova5.Visible = false;
                        this.pnl_prova3.Visible = true;



                    }

                    else if (this.RadioButtonReport.SelectedIndex.Equals(3))
                    {
                        this.Fieldset5.Visible = false;
                        this.Fieldset2.Visible = false;
                        this.Fieldset4.Visible = true;
                        this.Fieldset1.Visible = false;
                        this.Fieldset3.Visible = false;
                        this.pnl_prova.Visible = false;
                        this.pnl_prova2.Visible = false;
                        this.pnl_prova3.Visible = false;
                        this.pnl_prova4.Visible = true;
                        this.pnl_prova5.Visible = false;


                    }

                    else if (this.RadioButtonReport.SelectedIndex.Equals(4))
                    {
                        this.Fieldset5.Visible = true;
                        this.Fieldset2.Visible = false;
                        this.Fieldset4.Visible = false;
                        this.Fieldset1.Visible = false;
                        this.Fieldset3.Visible = false;
                        this.pnl_prova.Visible = false;
                        this.pnl_prova2.Visible = false;
                        this.pnl_prova3.Visible = false;
                        this.pnl_prova4.Visible = false;
                        this.pnl_prova5.Visible = true;


                    }

        }

        protected void ddl_invio1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_invio1.SelectedIndex)
            {
                case 0:
                    this.lblANotifiche.Visible = false;
                    this.lbl_dCreazioneA.Visible = false;
                    this.lbl_dCreazioneA.Text = string.Empty;
                    this.lblDaNotifiche.Text = "Il";

                    this.lblDaNotifiche.Visible = true;
                    this.lbl_dCreazioneDa.Visible = true;
                    break;

                case 1:
                    this.lblDaNotifiche.Text = "Da";
                    this.lblANotifiche.Visible = true;
                    this.lblANotifiche.Text = "A";
                    this.lbl_dCreazioneA.Visible = true;
                    this.lblDaNotifiche.Visible = true;
                    this.lbl_dCreazioneDa.Visible = true;
                    break;

                case 2:
                    this.lblANotifiche.Visible = false;
                    this.lbl_dCreazioneA.Visible = false;
                    this.lbl_dCreazioneA.Text = string.Empty;

                    this.lblDaNotifiche.Visible = false;
                    this.lbl_dCreazioneDa.Visible = false;
                    this.lbl_dCreazioneDa.Text = string.Empty;
                    break;

                case 3:
                    this.lblANotifiche.Visible = false;
                    this.lbl_dCreazioneA.Visible = false;
                    this.lbl_dCreazioneA.Text = string.Empty;

                    this.lblDaNotifiche.Visible = false;
                    this.lbl_dCreazioneDa.Visible = false;
                    this.lbl_dCreazioneDa.Text = string.Empty;
                    break;

                case 4:
                    this.lblANotifiche.Visible = false;
                    this.lbl_dCreazioneA.Visible = false;
                    this.lbl_dCreazioneA.Text = string.Empty;

                    this.lblDaNotifiche.Visible = false;
                    this.lbl_dCreazioneDa.Visible = false;
                    this.lbl_dCreazioneDa.Text = string.Empty;
                    break;

            }

            //this.upFiltriRicerca.Update();
        }


        protected void ddl_chiusura1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_chiusura1.SelectedIndex)
            {
                case 0:
                    this.Label7.Visible = false;
                    this.lbl_dCreazioneA1.Visible = false;
                    this.lbl_dCreazioneA1.Text = string.Empty;
                    this.Label6.Text = "Il";

                    this.Label6.Visible = true;
                    this.lbl_dCreazioneDa1.Visible = true;
                    break;

                case 1:
                    this.Label7.Visible = true;
                    this.lbl_dCreazioneA1.Visible = true;
                    this.Label6.Text = "Da";
                    this.Label6.Visible = true;
                    this.Label7.Text = "A";
                    this.lbl_dCreazioneDa1.Visible = true;

                    break;

                case 2:
                    this.Label7.Visible = false;
                    this.lbl_dCreazioneA1.Visible = false;
                    this.lbl_dCreazioneA1.Text = string.Empty;

                    this.Label6.Visible = false;
                    this.lbl_dCreazioneDa1.Visible = false;
                    this.lbl_dCreazioneDa1.Text = string.Empty;
                    break;

                case 3:
                    this.Label7.Visible = false;
                    this.lbl_dCreazioneA1.Visible = false;
                    this.lbl_dCreazioneA1.Text = string.Empty;

                    this.Label6.Visible = false;
                    this.lbl_dCreazioneDa1.Visible = false;
                    this.lbl_dCreazioneDa1.Text = string.Empty;
                    break;

                case 4:
                    this.Label7.Visible = false;
                    this.lbl_dCreazioneA1.Visible = false;
                    this.lbl_dCreazioneA1.Text = string.Empty;

                    this.Label6.Visible = false;
                    this.lbl_dCreazioneDa1.Visible = false;
                    this.lbl_dCreazioneDa1.Text = string.Empty;
                    break;

            }

            //this.upFiltriRicerca.Update();
        }


        protected void ddl_rifiuto1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_rifiuto1.SelectedIndex)
            {
                case 0:
                    this.Label8.Visible = false;
                    this.lbl_dCreazioneA2.Visible = false;
                    this.lbl_dCreazioneA2.Text = string.Empty;
                    this.Label1.Text = "Il";

                    this.Label1.Visible = true;
                    this.lbl_dCreazioneDa2.Visible = true;
                    break;

                case 1:
                    this.Label1.Visible = true;
                    this.lbl_dCreazioneA2.Visible = true;
                    this.Label1.Text = "Da";
                    this.Label8.Visible = true;
                    this.Label8.Text = "A";
                    this.lbl_dCreazioneDa2.Visible = true;

                    break;

                case 2:
                    this.Label8.Visible = false;
                    this.lbl_dCreazioneA2.Visible = false;
                    this.lbl_dCreazioneA2.Text = string.Empty;

                    this.Label1.Visible = false;
                    this.lbl_dCreazioneDa2.Visible = false;
                    this.lbl_dCreazioneDa2.Text = string.Empty;
                    break;

                case 3:
                    this.Label8.Visible = false;
                    this.lbl_dCreazioneA2.Visible = false;
                    this.lbl_dCreazioneA2.Text = string.Empty;

                    this.Label1.Visible = false;
                    this.lbl_dCreazioneDa2.Visible = false;
                    this.lbl_dCreazioneDa2.Text = string.Empty;
                    break;

                case 4:
                    this.Label8.Visible = false;
                    this.lbl_dCreazioneA2.Visible = false;
                    this.lbl_dCreazioneA2.Text = string.Empty;

                    this.Label1.Visible = false;
                    this.lbl_dCreazioneDa2.Visible = false;
                    this.lbl_dCreazioneDa2.Text = string.Empty;
                    break;

            }

            //this.upFiltriRicerca.Update();
        }

        protected void ddl_idIstanza_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            txt_fineIdIst.Text = "";

            if (this.ddl_idIstanza.SelectedIndex == 0)
            {
                this.txt_fineIdIst.Visible = false;
                this.lblAIdIst.Visible = false;
                this.lblDAIdIst.Text = "Il";
            }
            else
            {
                this.txt_fineIdIst.Visible = true;
                this.lblAIdIst.Visible = true;
                this.lblDAIdIst.Text = "Da";
            }
            //this.upFiltriRicerca.Update();
        }

        protected void ddl_invioc_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_invioc.SelectedIndex)
            {
                case 0:
                    this.Label11.Visible = false;
                    this.lbl_dCreazioneA4.Visible = false;
                    this.lbl_dCreazioneA4.Text = string.Empty;
                    this.Label10.Text = "Il";

                    this.Label10.Visible = true;
                    this.lbl_dCreazioneDa4.Visible = true;
                    break;

                case 1:
                    this.lbl_dCreazioneDa4.Visible = true;
                    this.Label10.Visible = true;
                    this.lbl_dCreazioneA4.Visible = true;
                    this.Label10.Text = "Da";
                    this.Label11.Visible = true;


                    break;

                case 2:
                    this.Label11.Visible = false;
                    this.lbl_dCreazioneA4.Visible = false;
                    this.lbl_dCreazioneA4.Text = string.Empty;

                    this.Label10.Visible = false;
                    this.lbl_dCreazioneDa4.Visible = false;
                    this.lbl_dCreazioneDa4.Text = string.Empty;
                    break;

                case 3:
                    this.Label11.Visible = false;
                    this.lbl_dCreazioneA4.Visible = false;
                    this.lbl_dCreazioneA4.Text = string.Empty;

                    this.Label10.Visible = false;
                    this.lbl_dCreazioneDa4.Visible = false;
                    this.lbl_dCreazioneDa4.Text = string.Empty;
                    break;

                case 4:
                    this.Label11.Visible = false;
                    this.lbl_dCreazioneA4.Visible = false;
                    this.lbl_dCreazioneA4.Text = string.Empty;

                    this.Label10.Visible = false;
                    this.lbl_dCreazioneDa4.Visible = false;
                    this.lbl_dCreazioneDa4.Text = string.Empty;
                    break;

            }

            //this.upFiltriRicerca.Update();
        }

        protected void ddl_chiusura2_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_chiusura2.SelectedIndex)
            {
                case 0:
                    this.Label13.Visible = false;
                    this.lbl_dCreazioneA5.Visible = false;
                    this.lbl_dCreazioneA5.Text = string.Empty;
                    this.Label12.Text = "Il";

                    this.Label12.Visible = true;
                    this.lbl_dCreazioneDa5.Visible = true;
                    break;

                case 1:
                    this.Label12.Visible = true;
                    this.lbl_dCreazioneA5.Visible = true;
                    this.Label12.Text = "Da";
                    this.Label13.Visible = true;
                    this.lbl_dCreazioneDa5.Visible = true;

                    break;

                case 2:
                    this.Label13.Visible = false;
                    this.lbl_dCreazioneA5.Visible = false;
                    this.lbl_dCreazioneA5.Text = string.Empty;

                    this.Label12.Visible = false;
                    this.lbl_dCreazioneDa5.Visible = false;
                    this.lbl_dCreazioneDa5.Text = string.Empty;
                    break;

                case 3:
                    this.Label13.Visible = false;
                    this.lbl_dCreazioneA5.Visible = false;
                    this.lbl_dCreazioneA5.Text = string.Empty;

                    this.Label12.Visible = false;
                    this.lbl_dCreazioneDa5.Visible = false;
                    this.lbl_dCreazioneDa5.Text = string.Empty;
                    break;

                case 4:
                    this.Label13.Visible = false;
                    this.lbl_dCreazioneA5.Visible = false;
                    this.lbl_dCreazioneA5.Text = string.Empty;

                    this.Label12.Visible = false;
                    this.lbl_dCreazioneDa5.Visible = false;
                    this.lbl_dCreazioneDa5.Text = string.Empty;
                    break;

            }

            //this.upFiltriRicerca.Update();
        }

        protected void ddl_rifiuto2_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_rifiuto2.SelectedIndex)
            {
                case 0:
                    this.Label15.Visible = false;
                    this.lbl_dCreazioneA6.Visible = true;
                    this.lbl_dCreazioneA6.Text = string.Empty;
                    this.Label14.Text = "Il";

                    this.Label14.Visible = true;
                    this.lbl_dCreazioneDa6.Visible = false;
                    break;

                case 1:
                    this.Label14.Visible = true;
                    this.lbl_dCreazioneA6.Visible = true;
                    this.Label14.Text = "Da";
                    this.Label15.Visible = true;
                    this.lbl_dCreazioneDa6.Visible = true;

                    break;

                case 2:
                    this.Label15.Visible = false;
                    this.lbl_dCreazioneA6.Visible = false;
                    this.lbl_dCreazioneA6.Text = string.Empty;

                    this.Label14.Visible = false;
                    this.lbl_dCreazioneDa6.Visible = false;
                    this.lbl_dCreazioneDa6.Text = string.Empty;
                    break;

                case 3:
                    this.Label15.Visible = false;
                    this.lbl_dCreazioneA6.Visible = false;
                    this.lbl_dCreazioneA6.Text = string.Empty;

                    this.Label14.Visible = false;
                    this.lbl_dCreazioneDa6.Visible = false;
                    this.lbl_dCreazioneDa6.Text = string.Empty;
                    break;

                case 4:
                    this.Label14.Visible = false;
                    this.lbl_dCreazioneA6.Visible = false;
                    this.lbl_dCreazioneA6.Text = string.Empty;

                    this.Label14.Visible = false;
                    this.lbl_dCreazioneDa6.Visible = false;
                    this.lbl_dCreazioneDa6.Text = string.Empty;
                    break;

            }

            //this.upFiltriRicerca.Update();
        }

        protected void ddl_idIstanza2_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            TextBox2.Text = "";

            if (this.ddl_idIstanza2.SelectedIndex == 0)
            {
                this.TextBox2.Visible = false;
                this.Label3.Visible = false;
                this.Label2.Text = "Il";
            }
            else
            {
                this.TextBox2.Visible = true;
                this.Label3.Visible = true;
                this.Label2.Text = "Da";
            }
            //this.upFiltriRicerca.Update();
        }

        protected void ddl_invio3_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_invio3.SelectedIndex)
            {
                case 0:
                    this.Label16.Visible = false;
                    this.lbl_dCreazioneA7.Visible = false;
                    this.lbl_dCreazioneA7.Text = string.Empty;
                    this.Label9.Text = "Il";

                    this.Label9.Visible = true;
                    this.lbl_dCreazioneDa7.Visible = true;
                    break;

                case 1:
                    this.Label9.Visible = true;
                    this.lbl_dCreazioneA7.Visible = true;
                    this.Label9.Text = "Da";
                    this.Label16.Visible = true;
                    this.lbl_dCreazioneDa7.Visible = true;

                    break;

                case 2:
                    this.Label16.Visible = false;
                    this.lbl_dCreazioneA7.Visible = false;
                    this.lbl_dCreazioneA7.Text = string.Empty;

                    this.Label9.Visible = false;
                    this.lbl_dCreazioneDa7.Visible = false;
                    this.lbl_dCreazioneDa7.Text = string.Empty;
                    break;

                case 3:
                    this.Label16.Visible = false;
                    this.lbl_dCreazioneA7.Visible = false;
                    this.lbl_dCreazioneA7.Text = string.Empty;

                    this.Label9.Visible = false;
                    this.lbl_dCreazioneDa7.Visible = false;
                    this.lbl_dCreazioneDa7.Text = string.Empty;
                    break;

                case 4:
                    this.Label9.Visible = false;
                    this.lbl_dCreazioneA7.Visible = false;
                    this.lbl_dCreazioneA7.Text = string.Empty;

                    this.Label9.Visible = false;
                    this.lbl_dCreazioneDa7.Visible = false;
                    this.lbl_dCreazioneDa7.Text = string.Empty;
                    break;

            }

            //this.upFiltriRicerca.Update();
        }

        protected void ddl_chiusura3_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_chiusura3.SelectedIndex)
            {
                case 0:
                    this.Label18.Visible = false;
                    this.lbl_dCreazioneA8.Visible = false;
                    this.lbl_dCreazioneA8.Text = string.Empty;
                    this.Label17.Text = "Il";

                    this.Label17.Visible = true;
                    this.lbl_dCreazioneDa8.Visible = true;
                    break;

                case 1:
                    this.Label17.Visible = true;
                    this.lbl_dCreazioneA8.Visible = true;
                    this.Label17.Text = "Da";
                    this.Label18.Visible = true;
                    this.lbl_dCreazioneDa8.Visible = true;

                    break;

                case 2:
                    this.Label18.Visible = false;
                    this.lbl_dCreazioneA8.Visible = false;
                    this.lbl_dCreazioneA8.Text = string.Empty;

                    this.Label17.Visible = false;
                    this.lbl_dCreazioneDa8.Visible = false;
                    this.lbl_dCreazioneDa8.Text = string.Empty;
                    break;

                case 3:
                    this.Label18.Visible = false;
                    this.lbl_dCreazioneA8.Visible = false;
                    this.lbl_dCreazioneA8.Text = string.Empty;

                    this.Label17.Visible = false;
                    this.lbl_dCreazioneDa8.Visible = false;
                    this.lbl_dCreazioneDa8.Text = string.Empty;
                    break;

                case 4:
                    this.Label17.Visible = false;
                    this.lbl_dCreazioneA8.Visible = false;
                    this.lbl_dCreazioneA8.Text = string.Empty;

                    this.Label17.Visible = false;
                    this.lbl_dCreazioneDa8.Visible = false;
                    this.lbl_dCreazioneDa8.Text = string.Empty;
                    break;

            }

            //this.upFiltriRicerca.Update();
        }

        protected void ddl_idSupporto_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            TextBox4.Text = "";

            if (this.ddl_idSupporto.SelectedIndex == 0)
            {
                this.TextBox4.Visible = false;
                this.Label22.Visible = false;
                this.Label21.Text = "Il";
            }
            else
            {
                this.TextBox4.Visible = true;
                this.Label22.Visible = true;
                this.Label21.Text = "Da";
            }

            //this.upFiltriRicerca.Update();
        }

        protected void ddl_generazioneSupporto_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_generazioneSupporto.SelectedIndex)
            {
                case 0:
                    this.Label24.Visible = false;
                    this.lbl_dCreazioneA10.Visible = false;
                    this.lbl_dCreazioneA10.Text = string.Empty;
                    this.Label23.Text = "Il";

                    this.Label23.Visible = true;
                    this.lbl_dCreazioneDa10.Visible = true;
                    break;

                case 1:
                    this.Label23.Visible = true;
                    this.lbl_dCreazioneA10.Visible = true;
                    this.Label23.Text = "Da";
                    this.Label24.Visible = true;
                    this.lbl_dCreazioneDa10.Visible = true;

                    break;

                case 2:
                    this.Label24.Visible = false;
                    this.lbl_dCreazioneA10.Visible = false;
                    this.lbl_dCreazioneA8.Text = string.Empty;

                    this.Label23.Visible = false;
                    this.lbl_dCreazioneDa10.Visible = false;
                    this.lbl_dCreazioneDa10.Text = string.Empty;
                    break;

                case 3:
                    this.Label24.Visible = false;
                    this.lbl_dCreazioneA10.Visible = false;
                    this.lbl_dCreazioneA10.Text = string.Empty;

                    this.Label23.Visible = false;
                    this.lbl_dCreazioneDa10.Visible = false;
                    this.lbl_dCreazioneDa10.Text = string.Empty;
                    break;

                case 4:
                    this.Label23.Visible = false;
                    this.lbl_dCreazioneA10.Visible = false;
                    this.lbl_dCreazioneA10.Text = string.Empty;

                    this.Label23.Visible = false;
                    this.lbl_dCreazioneDa10.Visible = false;
                    this.lbl_dCreazioneDa10.Text = string.Empty;
                    break;

            }

            //this.upFiltriRicerca.Update();
        }

        protected void ddl_esecuzioneVerifica_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_esecuzioneVerifica.SelectedIndex)
            {
                case 0:
                    this.Label26.Visible = false;
                    this.lbl_dCreazioneA11.Visible = false;
                    this.lbl_dCreazioneA11.Text = string.Empty;
                    this.Label25.Text = "Il";

                    this.Label25.Visible = true;
                    this.lbl_dCreazioneDa11.Visible = true;
                    break;

                case 1:
                    this.Label25.Visible = true;
                    this.lbl_dCreazioneA11.Visible = true;
                    this.Label25.Text = "Da";
                    this.Label26.Visible = true;
                    this.lbl_dCreazioneDa11.Visible = true;

                    break;

                case 2:
                    this.Label26.Visible = false;
                    this.lbl_dCreazioneA11.Visible = false;
                    this.lbl_dCreazioneA11.Text = string.Empty;

                    this.Label25.Visible = false;
                    this.lbl_dCreazioneDa11.Visible = false;
                    this.lbl_dCreazioneDa11.Text = string.Empty;
                    break;

                case 3:
                    this.Label26.Visible = false;
                    this.lbl_dCreazioneA11.Visible = false;
                    this.lbl_dCreazioneA11.Text = string.Empty;

                    this.Label25.Visible = false;
                    this.lbl_dCreazioneDa11.Visible = false;
                    this.lbl_dCreazioneDa11.Text = string.Empty;
                    break;

                case 4:
                    this.Label25.Visible = false;
                    this.lbl_dCreazioneA11.Visible = false;
                    this.lbl_dCreazioneA11.Text = string.Empty;

                    this.Label25.Visible = false;
                    this.lbl_dCreazioneDa11.Visible = false;
                    this.lbl_dCreazioneDa11.Text = string.Empty;
                    break;

            }

            //this.upFiltriRicerca.Update();
        }

        protected void ddl_idIstanza3_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            TextBox4.Text = "";

            if (this.ddl_idIstanza3.SelectedIndex == 0)
            {
                this.TextBox6.Visible = false;
                this.Label19.Visible = false;
                this.Label4.Text = "Il";
            }
            else
            {
                this.TextBox6.Visible = true;
                this.Label19.Visible = true;
                this.Label4.Text = "Da";
            }
        }

        protected void ddl_invioc3_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_invioc3.SelectedIndex)
            {
                case 0:
                    this.Label27.Visible = false;
                    this.lbl_dCreazioneA12.Visible = false;
                    this.lbl_dCreazioneA12.Text = string.Empty;
                    this.Label20.Text = "Il";

                    this.Label20.Visible = true;
                    this.lbl_dCreazioneDa12.Visible = true;
                    break;

                case 1:
                    this.Label20.Visible = true;
                    this.lbl_dCreazioneA12.Visible = true;
                    this.Label20.Text = "Da";
                    this.Label27.Visible = true;
                    this.lbl_dCreazioneDa12.Visible = true;

                    break;

                case 2:
                    this.Label27.Visible = false;
                    this.lbl_dCreazioneA12.Visible = false;
                    this.lbl_dCreazioneA12.Text = string.Empty;

                    this.Label20.Visible = false;
                    this.lbl_dCreazioneDa12.Visible = false;
                    this.lbl_dCreazioneDa12.Text = string.Empty;
                    break;

                case 3:
                    this.Label27.Visible = false;
                    this.lbl_dCreazioneA12.Visible = false;
                    this.lbl_dCreazioneA12.Text = string.Empty;

                    this.Label20.Visible = false;
                    this.lbl_dCreazioneDa12.Visible = false;
                    this.lbl_dCreazioneDa12.Text = string.Empty;
                    break;

                case 4:
                    this.Label20.Visible = false;
                    this.lbl_dCreazioneA12.Visible = false;
                    this.lbl_dCreazioneA12.Text = string.Empty;

                    this.Label20.Visible = false;
                    this.lbl_dCreazioneDa12.Visible = false;
                    this.lbl_dCreazioneDa12.Text = string.Empty;
                    break;

            }

            //this.upFiltriRicerca.Update();
        }

        protected void ddl_chiusura4_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_chiusura4.SelectedIndex)
            {
                case 0:
                    this.Label29.Visible = false;
                    this.lbl_dCreazioneA13.Visible = false;
                    this.lbl_dCreazioneA13.Text = string.Empty;
                    this.Label28.Text = "Il";

                    this.Label28.Visible = true;
                    this.lbl_dCreazioneDa13.Visible = true;
                    break;

                case 1:
                    this.Label28.Visible = true;
                    this.lbl_dCreazioneA13.Visible = true;
                    this.Label28.Text = "Da";
                    this.Label29.Visible = true;
                    this.lbl_dCreazioneDa13.Visible = true;

                    break;

                case 2:
                    this.Label29.Visible = false;
                    this.lbl_dCreazioneA13.Visible = false;
                    this.lbl_dCreazioneA13.Text = string.Empty;

                    this.Label28.Visible = false;
                    this.lbl_dCreazioneDa13.Visible = false;
                    this.lbl_dCreazioneDa13.Text = string.Empty;
                    break;

                case 3:
                    this.Label29.Visible = false;
                    this.lbl_dCreazioneA13.Visible = false;
                    this.lbl_dCreazioneA13.Text = string.Empty;

                    this.Label28.Visible = false;
                    this.lbl_dCreazioneDa13.Visible = false;
                    this.lbl_dCreazioneDa13.Text = string.Empty;
                    break;

                case 4:
                    this.Label28.Visible = false;
                    this.lbl_dCreazioneA13.Visible = false;
                    this.lbl_dCreazioneA13.Text = string.Empty;

                    this.Label28.Visible = false;
                    this.lbl_dCreazioneDa13.Visible = false;
                    this.lbl_dCreazioneDa13.Text = string.Empty;
                    break;

            }

            //this.upFiltriRicerca.Update();
        }

        protected void ddl_rifiuto3_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_rifiuto3.SelectedIndex)
            {
                case 0:
                    this.Label31.Visible = false;
                    this.lbl_dCreazioneA14.Visible = false;
                    this.lbl_dCreazioneA14.Text = string.Empty;
                    this.Label30.Text = "Il";

                    this.Label30.Visible = true;
                    this.lbl_dCreazioneDa14.Visible = true;
                    break;

                case 1:
                    this.Label30.Visible = true;
                    this.lbl_dCreazioneA14.Visible = true;
                    this.Label30.Text = "Da";
                    this.Label31.Visible = true;
                    this.lbl_dCreazioneDa14.Visible = true;

                    break;

                case 2:
                    this.Label31.Visible = false;
                    this.lbl_dCreazioneA14.Visible = false;
                    this.lbl_dCreazioneA14.Text = string.Empty;

                    this.Label30.Visible = false;
                    this.lbl_dCreazioneDa14.Visible = false;
                    this.lbl_dCreazioneDa14.Text = string.Empty;
                    break;

                case 3:
                    this.Label31.Visible = false;
                    this.lbl_dCreazioneA14.Visible = false;
                    this.lbl_dCreazioneA14.Text = string.Empty;

                    this.Label30.Visible = false;
                    this.lbl_dCreazioneDa14.Visible = false;
                    this.lbl_dCreazioneDa14.Text = string.Empty;
                    break;

                case 4:
                    this.Label30.Visible = false;
                    this.lbl_dCreazioneA14.Visible = false;
                    this.lbl_dCreazioneA14.Text = string.Empty;

                    this.Label30.Visible = false;
                    this.lbl_dCreazioneDa14.Visible = false;
                    this.lbl_dCreazioneDa14.Text = string.Empty;
                    break;

            }

            //this.upFiltriRicerca.Update();
        }

        protected void ddl_idIstanza4_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            TextBox7.Text = "";

            if (this.ddl_idIstanza4.SelectedIndex == 0)
            {
                this.TextBox8.Visible = false;
                this.Label32.Visible = false;
                this.Label5.Text = "Il";
            }
            else
            {
                this.TextBox8.Visible = true;
                this.Label32.Visible = true;
                this.Label5.Text = "Da";
            }
        }

        protected void ddl_invioc5_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_invioc5.SelectedIndex)
            {
                case 0:
                    this.Label34.Visible = false;
                    this.lbl_dCreazioneA16.Visible = false;
                    this.lbl_dCreazioneA16.Text = string.Empty;
                    this.Label33.Text = "Il";

                    this.Label33.Visible = true;
                    this.lbl_dCreazioneDa16.Visible = true;
                    break;

                case 1:
                    this.Label33.Visible = true;
                    this.lbl_dCreazioneA16.Visible = true;
                    this.Label33.Text = "Da";
                    this.Label34.Visible = true;
                    this.lbl_dCreazioneDa16.Visible = true;

                    break;

                case 2:
                    this.Label34.Visible = false;
                    this.lbl_dCreazioneA16.Visible = false;
                    this.lbl_dCreazioneA16.Text = string.Empty;

                    this.Label33.Visible = false;
                    this.lbl_dCreazioneDa16.Visible = false;
                    this.lbl_dCreazioneDa16.Text = string.Empty;
                    break;

                case 3:
                    this.Label34.Visible = false;
                    this.lbl_dCreazioneA16.Visible = false;
                    this.lbl_dCreazioneA16.Text = string.Empty;

                    this.Label33.Visible = false;
                    this.lbl_dCreazioneDa16.Visible = false;
                    this.lbl_dCreazioneDa16.Text = string.Empty;
                    break;

                case 4:
                    this.Label33.Visible = false;
                    this.lbl_dCreazioneA16.Visible = false;
                    this.lbl_dCreazioneA16.Text = string.Empty;

                    this.Label33.Visible = false;
                    this.lbl_dCreazioneDa16.Visible = false;
                    this.lbl_dCreazioneDa16.Text = string.Empty;
                    break;

            }

            //this.upFiltriRicerca.Update();
        }

        protected void ddl_chiusura5_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_chiusura5.SelectedIndex)
            {
                case 0:
                    this.Label36.Visible = false;
                    this.lbl_dCreazioneA15.Visible = false;
                    this.lbl_dCreazioneA15.Text = string.Empty;
                    this.Label35.Text = "Il";

                    this.Label35.Visible = true;
                    this.lbl_dCreazioneDa15.Visible = true;
                    break;

                case 1:
                    this.Label35.Visible = true;
                    this.lbl_dCreazioneA15.Visible = true;
                    this.Label35.Text = "Da";
                    this.Label36.Visible = true;
                    this.lbl_dCreazioneDa15.Visible = true;

                    break;

                case 2:
                    this.Label36.Visible = false;
                    this.lbl_dCreazioneA15.Visible = false;
                    this.lbl_dCreazioneA15.Text = string.Empty;

                    this.Label35.Visible = false;
                    this.lbl_dCreazioneDa15.Visible = false;
                    this.lbl_dCreazioneDa15.Text = string.Empty;
                    break;

                case 3:
                    this.Label36.Visible = false;
                    this.lbl_dCreazioneA15.Visible = false;
                    this.lbl_dCreazioneA15.Text = string.Empty;

                    this.Label35.Visible = false;
                    this.lbl_dCreazioneDa15.Visible = false;
                    this.lbl_dCreazioneDa15.Text = string.Empty;
                    break;

                case 4:
                    this.Label35.Visible = false;
                    this.lbl_dCreazioneA15.Visible = false;
                    this.lbl_dCreazioneA15.Text = string.Empty;

                    this.Label35.Visible = false;
                    this.lbl_dCreazioneDa15.Visible = false;
                    this.lbl_dCreazioneDa15.Text = string.Empty;
                    break;

            }

            //this.upFiltriRicerca.Update();
        }

        protected void ddl_idDocumento_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            TextBox9.Text = "";

            if (this.ddl_idDocumento.SelectedIndex == 0)
            {
                this.TextBox10.Visible = false;
                this.Label38.Visible = false;
                this.Label37.Text = "Il";
            }
            else
            {
                this.TextBox10.Visible = true;
                this.Label38.Visible = true;
                this.Label37.Text = "Da";
            }
        }

        protected void ddl_protocollo_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            TextBox11.Text = "";

            if (this.ddl_protocollo.SelectedIndex == 0)
            {
                this.TextBox12.Visible = false;
                this.Label40.Visible = false;
                this.Label39.Text = "Il";
            }
            else
            {
                this.TextBox12.Visible = true;
                this.Label40.Visible = true;
                this.Label39.Text = "Da";
            }
        }

        protected void RadioButtonStampaReport_SelectedIndexChanged(object sender, EventArgs e)
        {
            

        }

        protected void btnReset_Click(object sender, EventArgs e) 
        {
            if (this.RadioButtonReport.SelectedIndex.Equals(0))
            {

                this.ddl_invio1.SelectedIndex = 0;
                this.GetCalendarControl("lbl_dCreazioneDa").txt_Data.Text = string.Empty;
                this.GetCalendarControl("lbl_dCreazioneDa").txt_Data.Visible = true;
                this.lbl_dCreazioneDa.Visible = true;
                this.GetCalendarControl("lbl_dCreazioneA").txt_Data.Text = string.Empty;
                this.lblDaNotifiche.Text = "Il";
                this.lblDaNotifiche.Visible = true;
                this.lblANotifiche.Text = string.Empty;
                this.lbl_dCreazioneA.Visible = false;
                this.ddl_chiusura1.SelectedIndex = 0;
                this.GetCalendarControl("lbl_dCreazioneDa1").txt_Data.Text = string.Empty;
                this.GetCalendarControl("lbl_dCreazioneA1").txt_Data.Text = string.Empty;
                this.GetCalendarControl("lbl_dCreazioneDa1").txt_Data.Visible=true;
                this.lbl_dCreazioneDa1.Visible = true;
                this.Label6.Visible = true;
                this.Label6.Text = "Il";
                this.Label7.Text = string.Empty;
                this.lbl_dCreazioneA1.Visible = false;
                this.ddl_rifiuto1.SelectedIndex = 0;
                this.GetCalendarControl("lbl_dCreazioneDa2").txt_Data.Text = string.Empty;
                this.GetCalendarControl("lbl_dCreazioneA2").txt_Data.Text = string.Empty;
                this.lbl_dCreazioneDa2.Visible = true;
                this.Label1.Text = "Il";
                this.Label1.Visible = true;
                this.Label8.Text = string.Empty;
                this.lbl_dCreazioneA2.Visible = false;

            }

            if (this.RadioButtonReport.SelectedIndex.Equals(1))
            {
                this.ddl_idIstanza.SelectedIndex = 0;
                this.lblDAIdIst.Text = "Il";
                this.txt_initIdIst.Visible = true;
                this.txt_initIdIst.Text = string.Empty;
                this.lblAIdIst.Visible = false;
                this.txt_fineIdIst.Text = string.Empty;
                this.txt_fineIdIst.Visible = false;
                this.ddl_invioc.SelectedIndex = 0;
                this.GetCalendarControl("lbl_dCreazioneDa4").txt_Data.Text = string.Empty;
                this.GetCalendarControl("lbl_dCreazioneA4").txt_Data.Text = string.Empty;
                this.lbl_dCreazioneA4.Visible = false;
                this.lbl_dCreazioneDa4.Visible = true;
                this.Label11.Visible = false;
                this.Label10.Text = "Il";
                this.Label10.Visible = true;
                this.ddl_chiusura2.SelectedIndex = 0;
                this.GetCalendarControl("lbl_dCreazioneDa5").txt_Data.Text = string.Empty;
                this.GetCalendarControl("lbl_dCreazioneA5").txt_Data.Text = string.Empty;
                this.lbl_dCreazioneA5.Visible = false;
                this.lbl_dCreazioneDa5.Visible = true;
                this.Label12.Text = "Il";
                this.Label13.Visible = false;
                this.Label12.Visible = true;
                this.ddl_rifiuto2.SelectedIndex = 0;
                this.GetCalendarControl("lbl_dCreazioneDa6").txt_Data.Text = string.Empty;
                this.GetCalendarControl("lbl_dCreazioneA6").txt_Data.Text = string.Empty;
                this.lbl_dCreazioneA6.Visible = false;
                this.lbl_dCreazioneDa6.Visible = true;
                this.Label14.Text = "Il";
                this.Label15.Visible = false;
                this.Label14.Visible = true;

            }

            if (this.RadioButtonReport.SelectedIndex.Equals(2))
            {
                this.ddl_idIstanza2.SelectedIndex = 0;
                this.TextBox1.Text = string.Empty;
                this.TextBox1.Visible = true;
                this.Label2.Text = "Il";
                this.Label3.Visible = false;
                this.TextBox2.Text = string.Empty;
                this.TextBox2.Visible = false;
                this.ddl_invio3.SelectedIndex = 0;
                this.GetCalendarControl("lbl_dCreazioneDa7").txt_Data.Text = string.Empty;
                this.GetCalendarControl("lbl_dCreazioneA7").txt_Data.Text = string.Empty;
                this.lbl_dCreazioneDa7.Visible = true;
                this.lbl_dCreazioneA7.Visible = false;
                this.Label9.Text = "Il";
                this.Label16.Visible = false;
                this.Label9.Visible = true;
                this.ddl_chiusura3.SelectedIndex = 0;
                this.GetCalendarControl("lbl_dCreazioneDa8").txt_Data.Text = string.Empty;
                this.GetCalendarControl("lbl_dCreazioneA8").txt_Data.Text = string.Empty;
                this.lbl_dCreazioneDa8.Visible = true;
                this.lbl_dCreazioneA8.Visible = false;
                this.Label17.Text = "Il";
                this.Label17.Visible = true;
                this.Label18.Visible = false;
                this.cboTipiConservazione.SelectedIndex = 0;
                this.ddl_idSupporto.SelectedIndex = 0;
                this.Label21.Visible = true;
                this.Label21.Text = "Il";
                this.Label22.Visible = false;
                this.TextBox4.Visible = false;
                this.TextBox4.Text = string.Empty;
                this.TextBox3.Text = string.Empty;
                this.ddl_generazioneSupporto.SelectedIndex = 0;
                this.GetCalendarControl("lbl_dCreazioneDa10").txt_Data.Text = string.Empty;
                this.GetCalendarControl("lbl_dCreazioneA10").txt_Data.Text = string.Empty;
                this.lbl_dCreazioneA10.Visible = false;
                this.lbl_dCreazioneDa10.Visible = true;
                this.Label23.Visible = true;
                this.Label23.Text = "Il";
                this.Label24.Visible = false;
                this.ddl_esecuzioneVerifica.SelectedIndex = 0;
                this.GetCalendarControl("lbl_dCreazioneDa11").txt_Data.Text = string.Empty;
                this.GetCalendarControl("lbl_dCreazioneA11").txt_Data.Text = string.Empty;
                this.lbl_dCreazioneA11.Visible = false;
                this.lbl_dCreazioneDa11.Visible = true;
                this.Label25.Visible = true;
                this.Label25.Text = "Il";
                this.Label26.Visible = false;
                this.DropDownList14.SelectedIndex = 0;
            }

            if (this.RadioButtonReport.SelectedIndex.Equals(3))
            {
                this.ddl_idIstanza3.SelectedIndex = 0;
                this.TextBox5.Visible = true;
                this.TextBox5.Text = string.Empty;
                this.TextBox6.Visible = false;
                this.TextBox6.Text = string.Empty;
                this.Label4.Text = "Il";
                this.Label4.Visible = true;
                this.Label19.Visible = false;
                this.ddl_invioc3.SelectedIndex = 0;
                this.GetCalendarControl("lbl_dCreazioneDa12").txt_Data.Text = string.Empty;
                this.GetCalendarControl("lbl_dCreazioneA12").txt_Data.Text = string.Empty;
                this.lbl_dCreazioneA12.Visible = false;
                this.lbl_dCreazioneDa12.Visible = true;
                this.Label20.Text = "Il";
                this.Label20.Visible = true;
                this.Label27.Visible = false;
                this.ddl_chiusura4.SelectedIndex = 0;
                this.GetCalendarControl("lbl_dCreazioneDa13").txt_Data.Text = string.Empty;
                this.GetCalendarControl("lbl_dCreazioneA13").txt_Data.Text = string.Empty;
                this.lbl_dCreazioneA13.Visible = false;
                this.lbl_dCreazioneDa13.Visible = true;
                this.Label28.Text = "Il";
                this.Label28.Visible = true;
                this.Label29.Visible = false;
                this.ddl_rifiuto3.SelectedIndex = 0;
                this.GetCalendarControl("lbl_dCreazioneDa14").txt_Data.Text = string.Empty;
                this.GetCalendarControl("lbl_dCreazioneA14").txt_Data.Text = string.Empty;
                this.lbl_dCreazioneA14.Visible = false;
                this.lbl_dCreazioneDa14.Visible = true;
                this.Label30.Text = "Il";
                this.Label30.Visible = true;
                this.Label31.Visible = false;
            }

            if (this.RadioButtonReport.SelectedIndex.Equals(4))
            {
                this.ddl_idIstanza4.SelectedIndex = 0;
                this.TextBox7.Visible = true;
                this.TextBox7.Text = string.Empty;
                this.TextBox8.Visible = false;
                this.TextBox8.Text = string.Empty;
                this.Label5.Text = "Il";
                this.Label5.Visible = true;
                this.Label32.Visible = false;
                this.ddl_invioc5.SelectedIndex = 0;
                this.GetCalendarControl("lbl_dCreazioneDa16").txt_Data.Text = string.Empty;
                this.GetCalendarControl("lbl_dCreazioneA16").txt_Data.Text = string.Empty;
                this.lbl_dCreazioneA16.Visible = false;
                this.lbl_dCreazioneDa16.Visible = true;
                this.Label33.Text = "Il";
                this.Label33.Visible = true;
                this.Label34.Visible = false;
                this.ddl_chiusura5.SelectedIndex = 0;
                this.GetCalendarControl("lbl_dCreazioneDa15").txt_Data.Text = string.Empty;
                this.GetCalendarControl("lbl_dCreazioneA15").txt_Data.Text = string.Empty;
                this.lbl_dCreazioneA15.Visible = false;
                this.lbl_dCreazioneDa15.Visible = true;
                this.Label35.Text = "Il";
                this.Label35.Visible = true;
                this.Label36.Visible = false;
                this.ddl_idDocumento.SelectedIndex = 0;
                this.TextBox9.Visible = true;
                this.TextBox9.Text = string.Empty;
                this.TextBox10.Visible = false;
                this.TextBox10.Text = string.Empty;
                this.Label37.Text = "Il";
                this.Label37.Visible = true;
                this.Label38.Visible = false;
                this.ddl_protocollo.SelectedIndex = 0;
                this.TextBox11.Text = string.Empty;
                this.TextBox11.Visible = true;
                this.TextBox12.Visible = false;
                this.TextBox12.Text = string.Empty;
                this.Label39.Text = "Il";
                this.Label39.Visible = true;
                this.Label40.Visible = false;
            }

        }

                
    }

}