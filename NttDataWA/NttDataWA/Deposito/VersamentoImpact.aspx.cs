using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;
using NttDatalLibrary;
using System.Collections;
using System.Drawing;
using System.Web.UI.DataVisualization.Charting;
using System.Globalization;

namespace NttDataWA.Deposito
{
    public partial class VersamentoImpact : System.Web.UI.Page
    {

        #region Fields

        #region listaProfileTransfer
        protected List<ARCHIVE_Profile_TransferPolicy> listaProfileTransfer
        {
            get
            {
                List<ARCHIVE_Profile_TransferPolicy> result = null;
                if (HttpContext.Current.Session["listaProfileTransfer"] != null)
                {
                    result = HttpContext.Current.Session["listaProfileTransfer"] as List<ARCHIVE_Profile_TransferPolicy>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listaProfileTransfer"] = value;
            }
        }

        #endregion

        #region Oggetto Transfer in Context

        protected List<ARCHIVE_Transfer> TransferInContext
        {
            get
            {
                List<ARCHIVE_Transfer> result = null;
                if (HttpContext.Current.Session["TransferInContext"] != null)
                {
                    result = HttpContext.Current.Session["TransferInContext"] as List<ARCHIVE_Transfer>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TransferInContext"] = value;
            }
        }
        #endregion

        #region Oggetto TransferPolicy in Context

        protected List<ARCHIVE_TransferPolicy> TransferPolicyInContext
        {
            get
            {
                List<ARCHIVE_TransferPolicy> result = null;
                if (HttpContext.Current.Session["TransferPolicyInContext"] != null)
                {
                    result = HttpContext.Current.Session["TransferPolicyInContext"] as List<ARCHIVE_TransferPolicy>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TransferPolicyInContext"] = value;
            }
        }

        public List<ARCHIVE_Profile_TransferPolicy> grdDocImpactSource
        {
            get
            {
                List<ARCHIVE_Profile_TransferPolicy> result = null;
                if (HttpContext.Current.Session["grdDocImpactSource"] != null)
                {
                    result = HttpContext.Current.Session["grdDocImpactSource"] as List<ARCHIVE_Profile_TransferPolicy>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["grdDocImpactSource"] = value;

            }
        }
        #endregion

        protected List<Int32> selectedProfile
        {
            get
            {
                List<Int32> result = new List<Int32>();
                if (HttpContext.Current.Session["selectedProfile"] != null)
                {
                    result = HttpContext.Current.Session["selectedProfile"] as List<Int32>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["selectedProfile"] = value;
            }
        }

        protected Boolean? selectedAll
        {
            get
            {
                Boolean? result = false;
                if (HttpContext.Current.Session["selectedAll"] != null)
                {
                    result = HttpContext.Current.Session["selectedAll"] as Boolean?;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["selectedAll"] = value;
            }
        }

        protected List<ARCHIVE_TransferState> TransferStateInContext
        {
            get
            {
                List<ARCHIVE_TransferState> result = null;
                if (HttpContext.Current.Session["TransferStateInContext"] != null)
                {
                    result = HttpContext.Current.Session["TransferStateInContext"] as List<ARCHIVE_TransferState>;
                }
                return result;
            }

        }

        protected List<ARCHIVE_JOB_Transfer> TransferJobInContext
        {
            get
            {
                List<ARCHIVE_JOB_Transfer> result = null;
                if (HttpContext.Current.Session["TransferJobInContext"] != null)
                    result = HttpContext.Current.Session["TransferJobInContext"] as List<ARCHIVE_JOB_Transfer>;

                return result;
            }
            set
            {
                HttpContext.Current.Session["TransferJobInContext"] = value;
            }
        }


        #endregion

        #region struct

        public struct VincoloCopia
        {
            private string Vincolo;

            public string vincolo
            {
                get { return Vincolo; }
                set { Vincolo = value; }
            }
            private double Percentuale;

            public double percentuale
            {
                get { return Percentuale; }
                set { Percentuale = value; }
            }

            public VincoloCopia(string vincolo, double percent)
            {
                Vincolo = vincolo;
                Percentuale = percent;
            }


        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    this.InitializePage();

                }
                this.refreshScript();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void InitializePage()
        {
            this.InitializeLanguage();
            this.getDataPage();
            this.LoadDataInControl();
        }

        private void LoadDataInControl()
        {
            try
            {
                this.LoadDataInRblTransferImpact();

                this.LoadDataInPieChart();
                this.loadDataInGrdVincolo();
                this.upPnlChartPieGrid.Update();

                this.loadDataInBarChartAnni();
                this.loadDataInBarChartMesi();

                this.visibilityGrdDoc();
                this.loadDataInGrdDoc();
                this.upPnlGrdDoc12mesi.Update();
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void visibilityGrdDoc()
        {

            if (this.TransferStateInContext.Count > 0 && this.TransferJobInContext.Count > 0)
            {
                //se lo stato è compreso tra analisi completata e la schedulazione dell'esecuzione, e se
                //la lista dei documenti acceduti nell'ultimo anno non sia vuota
                if (this.TransferStateInContext.Max(p => p.TransferStateType_ID) >
                                UIManager.ArchiveManager._dictionaryTransferState.Where(x => x.Value == "ANALISI COMPLETATA").
                                                                        Select(x => x.Key).FirstOrDefault()
                && this.TransferJobInContext.Where(j => j.Transfer_ID == this.TransferInContext.FirstOrDefault().System_ID)
                                                                          .Max(j => j.JobType_ID) == 3
                && this.grdDocImpactSource.Count > 0)
                {
                    this.btnConferma.Enabled = true;
                    this.chkSelectAll.Enabled = true;
                    this.GrdDoc.Columns[6].Visible = false;

                }
                else
                {
                    this.btnConferma.Enabled = false;
                    this.chkSelectAll.Enabled = false;
                    this.GrdDoc.Columns[6].Visible = true;
                }
            }
        }

        private void LoadDataInRblTransferImpact()
        {

            // creazione dinamica della rbl
            //this.rblTransferImpact.Items.Add(new ListItem("", "1"));
            //this.rblTransferImpact.Items.Add(new ListItem("", "2"));
            //this.rblTransferImpact.Items.Add(new ListItem("", "3"));
            //this.rblTransferImpact.Items.Add(new ListItem("", "4"));
        }

        private void getDataPage()
        {
            if (this.TransferInContext != null && this.TransferInContext.Count > 0)
            {
                this.TxtIdVersamento.Text = this.TransferInContext.FirstOrDefault().System_ID.ToString();
                this.TxtDescrizioneVersamento.Text = this.TransferInContext.FirstOrDefault().Description.ToString();
                this.TxtNoteVersamento.Text = this.TransferInContext.FirstOrDefault().Note.ToString();
                
                //insieme di documenti relativi al versamento ossia tutti i documenti inclusi dalle policy abilitate
                this.listaProfileTransfer = UIManager.ArchiveManager.GetARCHIVE_Profile_TransferPolicyByTransferPolicyList(
                                                         UIManager.ArchiveManager.GetSQLinStringFromListIdPolicy(
                                                                 this.TransferPolicyInContext.Where(x => x.Enabled == 1).
                                                                                        Select(x => x.System_ID).ToList()));

                DateTime startDate = new DateTime(DateTime.Now.AddYears(-1).Year, DateTime.Now.Month, 1);
                //lista di dati della griglia grdDoc documenti 
                this.grdDocImpactSource = this.listaProfileTransfer.Where(x => x.TipoTransferimento_Versamento == "TRASFERIMENTO"
                                                            && x.DataUltimoAccesso > startDate).Select(x => x).ToList();

                //job assoiciati al versamento utili per effettuare verifiche sui controlli della pagina
                this.TransferJobInContext = UIManager.ArchiveManager.GetARCHIVE_JOB_TransferByTransfer_ID(this.TransferInContext.FirstOrDefault().System_ID);

                this.selectedProfile = new List<Int32>();
                this.selectedAll = false;

            }
        }

        private void updateDataInGridDoc()
        {
            DateTime startDate = new DateTime(DateTime.Now.AddYears(-1).Year, DateTime.Now.Month, 1);
            this.grdDocImpactSource = this.listaProfileTransfer.Where(x => x.TipoTransferimento_Versamento == "TRASFERIMENTO"
                                                        && x.DataUltimoAccesso > startDate).ToList();

            if (this.grdDocImpactSource == null || this.grdDocImpactSource.Count == 0)
                this.grdDocImpactSource = this.getGrdDocDummy();


            this.GrdDoc.DataSource = this.grdDocImpactSource;
            this.GrdDoc.DataBind();
        }

        private void refreshScript()
        {

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);

        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.LitVersamentoImpProject.Text = Utils.Languages.GetLabelFromCode("VersamentoTitle", language);
            this.LitVersamentoId.Text = Utils.Languages.GetLabelFromCode("LitVersamentoId", language);
            this.LitVersamentoDescrizione.Text = Utils.Languages.GetLabelFromCode("LitVersamentoDescrizione", language);
            this.LitVersamentoNote.Text = Utils.Languages.GetLabelFromCode("LitVersamentoNote", language);
            this.ChartTransfer.Titles[0].Text = Utils.Languages.GetLabelFromCode("ChartTransferTitle", language);

            this.LitTipoAnalisi.Text = Utils.Languages.GetLabelFromCode("LitTipoAnalisi", language);
            this.btnVersamentoNuovo.Text = Utils.Languages.GetLabelFromCode("VersamentoNuovo", language);
            this.btnVersamentoAnalizza.Text = Utils.Languages.GetLabelFromCode("VersamentoAnalizza", language);
            this.btnVersamentoProponi.Text = Utils.Languages.GetLabelFromCode("VersamentoProponi", language);
            this.btnVersamentoApprova.Text = Utils.Languages.GetLabelFromCode("VersamentoApprova", language);
            this.btnVersamentoEsegui.Text = Utils.Languages.GetLabelFromCode("VersamentoEsegui", language);
            this.btnVersamentoModifica.Text = Utils.Languages.GetLabelFromCode("VersamentoModifica", language);
            this.btnVersamentoElimina.Text = Utils.Languages.GetLabelFromCode("VersamentoElimina", language);
            this.btnConferma.Text = Utils.Languages.GetLabelFromCode("msgTitleCnfirm", language);
            this.lblSelectAll.Text = Utils.Languages.GetLabelFromCode("ExportDatiSelectAll", language);
            this.lblTitoloGrdDoc.Text = Utils.Languages.GetLabelFromCode("impactTitoloGrdDoc", language);
            //sezione grafici
            ChartAcc5Anni.Titles[0].Text = Utils.Languages.GetLabelFromCode("ChartAcc5AnniTitle", language);
            ChartAcc12Mesi.Titles[0].Text = Utils.Languages.GetLabelFromCode("ChartAcc12MesiTitle", language);
            
        }

        protected void rblTransferImpact_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.showSelectedPanel(this.rblTransferImpact.SelectedValue);
        }

        protected void GrdDoc_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GrdDoc.DataSource = this.grdDocImpactSource;
            this.GrdDoc.PageIndex = e.NewPageIndex;
            this.GrdDoc.DataBind();
            this.upPnlGrdDoc12mesi.Update();

        }

        protected void GrdDoc_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string idProfile = this.GrdDoc.DataKeys[e.Row.RowIndex].Value.ToString();
                if (!idProfile.Equals("0"))
                {
                    if (this.listaProfileTransfer.Select(x => x.Profile_ID.ToString()).Equals(idProfile))
                    {
                        bool value = (this.listaProfileTransfer.Where(x => x.Profile_ID.Equals(idProfile)).Select(x => x.MantieniCopia).FirstOrDefault()) > 0;
                        ((CheckBox)e.Row.FindControl("chkCopia")).Checked = value;

                    }
                }
                else
                {
                    ((CheckBox)e.Row.FindControl("chkCopia")).Visible = false;
                    ((Label)e.Row.FindControl("LblUtentiValue")).Visible = false;
                    ((Label)e.Row.FindControl("LblAOOValue")).Visible = false;
                    ((Label)e.Row.FindControl("LblIdValue")).Visible = false;
                    ((Label)e.Row.FindControl("LblOggettoValue")).Visible = false;
                    ((Label)e.Row.FindControl("LbltipoValue")).Visible = false;
                    ((Label)e.Row.FindControl("LblTipoDocValue")).Visible = false;
                    ((Label)e.Row.FindControl("LblDataCreazioneValue")).Visible = false;
                    ((Label)e.Row.FindControl("LblAccValue")).Visible = false;
                    
                }
            }
        }

        protected void btnConferma_Click(object sender, EventArgs e)
        {
            //confermo il mantenimento come copia
            UIManager.ArchiveManager.UpdateARCHIVE_Profile_TransferPolicyByProfileList(UIManager.ArchiveManager.GetSQLinStringFromListIdPolicy(this.selectedProfile), this.TransferInContext.FirstOrDefault().System_ID);
            this.updateDataInGridDoc();
            upPnlGrdDoc12mesi.Update();
        }

        //metodo che fa il databinding dei dati nella griglia GrdDoc
        private void loadDataInGrdDoc()
        {
            if (this.grdDocImpactSource == null || this.grdDocImpactSource.Count == 0)
                this.grdDocImpactSource = this.getGrdDocDummy();

            this.GrdDoc.DataSource = this.grdDocImpactSource;
            this.GrdDoc.DataBind();
        }

        private List<ARCHIVE_Profile_TransferPolicy> getGrdDocDummy()
        {
            List<ARCHIVE_Profile_TransferPolicy> lista = new List<ARCHIVE_Profile_TransferPolicy>();
            ARCHIVE_Profile_TransferPolicy item = new ARCHIVE_Profile_TransferPolicy();
            item.Profile_ID = 0;
            item.TransferPolicy_ID = 0;
            item.Registro = string.Empty;
            item.DataCreazione = null;
            item.CopiaPerCatenaDoc_Versamento = 0;
            lista.Add(item);

            return lista;
        }



        private void loadDataInBarChartMesi()
        {
            if (this.listaProfileTransfer != null && this.listaProfileTransfer.Count > 0)
            {
                var list = from x in listaProfileTransfer
                           where x.DataUltimoAccesso != null && x.DataUltimoAccesso > DateTime.Now.AddMonths(-12)
                           select x;
                //risultati raggruppati per anno
                var groupResult = from p in list
                                  group p by p.DataUltimoAccesso.Value.Month into counts
                                  select new { Numero = counts.Count(), Mese = counts.Key };

                if (groupResult.Count() > 0)
                {
                    this.pnlChart12mesi.Visible = true;
                    this.pnlErrore12mesi.Visible = false;


                    List<int> Mese = groupResult.Select(x => x.Mese).Distinct().ToList();
                    List<int> numAccessi = groupResult.Select(x => x.Numero).ToList();

                    //se i dati sono inferiori di quelli sufficenti per creare il report
                    //aggiungo valori nulli per rendere il grafico leggibile
                    if (Mese.Count < 12)
                    {

                        List<int> templateMesi = Enumerable.Range(DateTime.Now.AddMonths(-1).Month, 12).Select(x => (x % 12) + 1)
                                                       .ToList();

                        List<int> mesiRes = new List<int>();
                        List<int> numAccessiRes = new List<int>();

                        int index = 0;

                        for (int i = 0; i < templateMesi.Count; i++)
                        {
                            if (Mese.Contains(templateMesi[i]))
                            {
                                index = Mese.IndexOf(templateMesi[i]);
                                mesiRes.Add(templateMesi[i]);
                                numAccessiRes.Add(numAccessi[index]);
                            }
                            else
                            {
                                mesiRes.Add(templateMesi[i]);
                                numAccessiRes.Add(0);
                            }
                        }

                        Mese = mesiRes;
                        numAccessi = numAccessiRes;
                    }

                    Color[] colorSet = { Color.LightSeaGreen ,
                                                Color.SteelBlue,
                                                Color.LightSlateGray,
                                                Color.MediumSlateBlue,
                                                Color.MediumSeaGreen,
                                                Color.LightGoldenrodYellow,
                                                Color.MediumPurple,
                                                Color.MediumTurquoise,
                                                Color.MidnightBlue,
                                                Color.LightSkyBlue,
                                                Color.MediumOrchid,
                                                Color.MediumVioletRed};

                    //gestione delle etichette dell'asse x
                    ChartAcc12Mesi.ChartAreas["ChartArea3"].AxisX.Interval = 1;
                    ChartAcc12Mesi.ChartAreas["ChartArea3"].AxisX.LabelStyle.Angle = -45;
                    ChartAcc12Mesi.ChartAreas["ChartArea3"].AxisX.LabelStyle.Font = new Font("Verdana", 8);
                    //ChartAcc12Mesi.Series["SerieAcc12Mesi"]["DrawingStyle"] = "Cylinder";

                    string nomeMese;
                    DataPoint point;
                    Legend item = new Legend("Legend");

                    
                    for (int i = 0; i < Mese.Count(); i++)
                    {
                       
                        nomeMese = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Mese[i]);
                        point = new DataPoint();
                        point.SetValueXY(nomeMese, numAccessi[i]);


                        point.Color = colorSet[i];
                        item.CustomItems.Add(point.Color, nomeMese + " = " + point.YValues[0].ToString() + " accessi");
                        ChartAcc12Mesi.Series["SerieAcc12Mesi"].Points.Add(point);
                    }


                    //ChartAcc12Mesi.ApplyPaletteColors();

                    //asscio il valore di ogni grafico come tooltip
                    ChartAcc12Mesi.Series["SerieAcc12Mesi"].ToolTip = "#VALY";

                    ChartAcc12Mesi.Series["SerieAcc12Mesi"].IsVisibleInLegend = false;

                    //gestione della legenda
                    ChartAcc12Mesi.Legends.Add(item);
                    
                    ChartAcc12Mesi.Legends[0].Enabled = true;
                    //ChartAcc12Mesi.Titles[0].Text = "Numero accessi negli ultimi 12 mesi";
                    ChartAcc12Mesi.Titles[0].Font = new Font("Verdana", 11, FontStyle.Bold);
                    ChartAcc12Mesi.Legends[0].Title = "Mesi";


                    ChartAcc12Mesi.Legends[0].Docking = Docking.Right;
                    ChartAcc12Mesi.Legends[0].Alignment = System.Drawing.StringAlignment.Center;
                    ChartAcc12Mesi.Legends[0].Font = new Font("Verdana", 8);
                    ChartAcc12Mesi.Legends[0].TitleFont = new Font("Verdana", 10, FontStyle.Bold);

                }
                else
                {
                    //non ci sono dati per questo report mostro il panel di errore
                    this.pnlChart12mesi.Visible = false;
                    this.pnlErrore12mesi.Visible = true;

                }


            }
        }

        private void loadDataInBarChartAnni()
        {
          
                if (this.listaProfileTransfer != null && this.listaProfileTransfer.Count > 0)
                {

                    int anno = DateTime.Now.Year;
                    var list = from p in listaProfileTransfer
                               where p.DataUltimoAccesso != null && p.DataUltimoAccesso.Value.Year > (anno - 5)
                               select p;
                    
                    //risultati raggruppati per anno
                    var groupResult = from p in list
                                      group p by p.DataUltimoAccesso.Value.Year into counts
                                      select new { Numero = counts.Count(), Anno = counts.Key };


                    if (groupResult.Count() > 0)
                    {
                        this.pnlChart5Anni.Visible = true;
                        this.pnlErrore5Anni.Visible = false;

                        List<int> anni = groupResult.Select(x => x.Anno).Distinct().ToList();
                        List<int> numAccessi = groupResult.Select(x => x.Numero).ToList();

                        //se non ci sono abbastanza dati per creare il report ne aggiungo di nulli
                        //per renderlo più leggibile
                        if (anni.Count < 5)
                        {

                            List<int> templateAnni = Enumerable.Range(anno - 4, 5)
                                                               .ToList();
                            List<int> anniRes = new List<int>();
                            List<int> numAccessiRes = new List<int>();

                            int index = 0;

                            for (int i = 0; i < templateAnni.Count; i++)
                            {
                                if (anni.Contains(templateAnni[i]))
                                {
                                    index = anni.IndexOf(templateAnni[i]);
                                    anniRes.Add(templateAnni[i]);
                                    numAccessiRes.Add(numAccessi[index]);
                                }
                                else
                                {
                                    anniRes.Add(templateAnni[i]);
                                    numAccessiRes.Add(0);
                                }
                            }

                            anni = anniRes;
                            numAccessi = numAccessiRes;
                        }

                        Color[] colorSet = {Color.LightSeaGreen,
                                         Color.SteelBlue,
                                         Color.LightSlateGray ,
                                         Color.MediumSlateBlue,
                                         Color.MediumSeaGreen};

                       
                        Legend item = new Legend("Legend");
                        ChartAcc5Anni.Series["SerieAcc5Anni"].IsVisibleInLegend = false;

                        DataPoint point;
                        
                        for (int i = 0; i < anni.Count(); i++)
                        {
                            point = new DataPoint();

                            point.SetValueXY(anni[i], numAccessi[i]);
                            point.Color = colorSet[i];
                            ChartAcc5Anni.Series["SerieAcc5Anni"].Points.Add(point);

                            item.CustomItems.Add(point.Color, point.XValue.ToString() + " = " + point.YValues[0].ToString() + " documenti");

                        }

                        ChartAcc5Anni.Series["SerieAcc5Anni"].ToolTip = "#VALY";



                        
                        ChartAcc5Anni.Titles[0].Font = new Font("Verdana", 11, FontStyle.Bold);

                        ChartAcc5Anni.Legends.Add(item);
                        ChartAcc5Anni.Legends[0].Enabled = true;
                        ChartAcc5Anni.Legends[0].Title = "Anni";
                        ChartAcc5Anni.Legends[0].TitleFont = new Font("Verdana", 11, FontStyle.Bold);
                       
                        ChartAcc5Anni.Legends[0].Alignment = System.Drawing.StringAlignment.Center;
                        ChartAcc5Anni.Legends[0].Font = new Font("Verdana", 9);
                        ChartAcc5Anni.Legends[0].TitleFont = new Font("Verdana", 11, FontStyle.Bold);

                    }
                    else
                    {
                        //non ci sono risultati per lo specifico report
                        this.pnlChart5Anni.Visible = false;
                        this.pnlErrore5Anni.Visible = true;

                    }
                }
            
        }



        private void LoadDataInPieChart()
        {
            if (this.listaProfileTransfer != null && this.listaProfileTransfer.Count > 0)
            {
                double totpProfile = this.listaProfileTransfer.Count;
                double nTrans = this.listaProfileTransfer.Where(x => x.TipoTransferimento_Versamento.Equals("TRASFERIMENTO")).Count();
                double nCopia = this.listaProfileTransfer.Where(x => x.TipoTransferimento_Versamento.Equals("COPIA")).Count();

                double percentCopie = nCopia / totpProfile;
                double percentTransfer = nTrans / totpProfile;


                double[] yValues = { percentTransfer, percentCopie };
                string[] xValues = { "Documenti trasferiti in modo effetivo ", "Documenti trasferiti come copia " };

                ChartTransfer.Series["SerieTransfer"].Points.DataBindXY(xValues, yValues);

                ChartTransfer.Series["SerieTransfer"].Points[0].Color = Color.MediumSeaGreen;
                ChartTransfer.Series["SerieTransfer"].Points[1].Color = Color.FromArgb(200, Color.Blue);

                ChartTransfer.Series["SerieTransfer"].Points[1]["Exploded"] = (nCopia > 0).ToString(); ;


                ChartTransfer.Series["SerieTransfer"].ChartType = SeriesChartType.Pie;

                ChartTransfer.Series["SerieTransfer"].Points[0].ToolTip = (Math.Round(yValues[0], 2) * 100).ToString() + "%";
                ChartTransfer.Series["SerieTransfer"].Points[1].ToolTip = (Math.Round(yValues[1], 2) * 100).ToString() + "%";
                ChartTransfer.Series["SerieTransfer"].Points[0].Label = xValues[0] + "(" + (Math.Round(yValues[0], 2) * 100).ToString() + "%)";
                ChartTransfer.Series["SerieTransfer"].Points[1].Label = xValues[1] + "(" + (Math.Round(yValues[1], 2) * 100).ToString() + "%)";

                ChartTransfer.Series["SerieTransfer"]["PieLabelStyle"] = "Disabled";
                //-----------------------------------------------
                //ChartTransfer.Series["SerieTransfer"]["PieDrawingStyle"] = "SoftEdge";

                ChartTransfer.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
                ChartTransfer.ChartAreas["ChartArea1"].Area3DStyle.LightStyle =
                                            System.Web.UI.DataVisualization.Charting.LightStyle.Realistic;


                ChartTransfer.Legends[0].Enabled = true;
                ChartTransfer.Legends[0].Title = "Tipo trasferimento";
                ChartTransfer.Legends[0].Font = new Font("Verdana", 9);



            }
            else
            {

            }
        }

        private void loadDataInGrdVincolo()
        {
            if (this.listaProfileTransfer != null && this.listaProfileTransfer.Count > 0)
            {
                List<VincoloCopia> grdSource = new List<VincoloCopia>();

                double nCopia = this.listaProfileTransfer.Where(x => x.TipoTransferimento_Versamento.Equals("COPIA")).Count();
                double percFascDoc = 0;
                double percCatDoc = 0;
                double percCons = 0;
                if (nCopia > 0)
                {
                    double nFascProc = this.listaProfileTransfer.Where(x => x.CopiaPerFascicolo_Versamento == 1).Count();
                    double nCatDoc = this.listaProfileTransfer.Where(x => x.CopiaPerCatenaDoc_Versamento == 1).Count();
                    double nCons = this.listaProfileTransfer.Where(x => x.CopiaPerConservazione_Versamento == 1).Count();


                    percFascDoc = Math.Round(nFascProc / nCopia, 2) * 100;
                    percCatDoc = Math.Round(nCatDoc / nCopia, 2) * 100;
                    percCons = Math.Round(nCons / nCopia, 2) * 100;
                }
                grdSource.Add(new VincoloCopia("Documenti inclusi in altri fascicoli procidementali in archivio corrente", percFascDoc));
                grdSource.Add(new VincoloCopia("Documenti concatenati ad altri documenti in archivio corrente", percCatDoc));
                grdSource.Add(new VincoloCopia("Documenti in conservazione in archivio corrente", percCons));

                this.GrdVincoli.DataSource = grdSource;
                this.GrdVincoli.DataBind();

            }

        }

        protected void chkCopia_CheckedChange(object sender, EventArgs e)
        {
            Int32 _idProfile = 0;
            CheckBox _box = (CheckBox)sender;
            Boolean _checked = _box.Checked;

            GridViewRow gridviewRow = (GridViewRow)_box.NamingContainer;

            //Get the rowindex
            int rowindex = gridviewRow.RowIndex;

            _idProfile = Convert.ToInt32(GrdDoc.DataKeys[rowindex].Value);
            if (_checked)
                this.selectedProfile.Add(_idProfile);
            else
            {
                this.selectedProfile.Remove(_idProfile);
                this.selectedAll = false;
            }
        }

        protected void addAll_Click(object sender, EventArgs e)
        {

            bool value = ((CheckBox)sender).Checked;
            this.selectedProfile = new List<Int32>();
            foreach (GridViewRow dgItem in GrdDoc.Rows)
            {
                CheckBox checkBox = dgItem.FindControl("chkCopia") as CheckBox;
                checkBox.Checked = value;
            }
            if (value)
            {
                this.selectedProfile.AddRange(this.grdDocImpactSource.Select(x => x.Profile_ID).ToList());
            }
            this.selectedAll = value;
            this.upPnlGrdDoc12mesi.Update();

        }


        private void RefreshScript()
        {
            ScriptManager.GetCurrent(this.Page).RegisterPostBackControl(this.ChartTransfer);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);

        }


        private void showSelectedPanel(string value)
        {
            switch (value)
            {
                case "1":
                    this.pnlChartPieGrid.Visible = true;

                    this.upPnlChartPieGrid.Update();

                    this.pnlNDocAcc5anni.Visible = false;
                    this.upPnlNDocAcc5anni.Update();

                    this.pnlNDocAcc12mesi.Visible = false;
                    this.upPnlNDocAcc12mesi.Update();

                    this.pnlGrdDoc12mesi.Visible = false;
                    this.upPnlGrdDoc12mesi.Update();
                    break;

                case "2":
                    this.pnlChartPieGrid.Visible = false;
                    this.upPnlChartPieGrid.Update();

                    this.pnlNDocAcc5anni.Visible = true;
                    this.upPnlNDocAcc5anni.Update();

                    this.pnlNDocAcc12mesi.Visible = false;
                    this.upPnlNDocAcc12mesi.Update();

                    this.pnlGrdDoc12mesi.Visible = false;
                    this.upPnlGrdDoc12mesi.Update();
                    break;

                case "3":
                    this.pnlChartPieGrid.Visible = false;
                    this.upPnlChartPieGrid.Update();

                    this.pnlNDocAcc5anni.Visible = false;
                    this.upPnlNDocAcc5anni.Update();

                    this.pnlNDocAcc12mesi.Visible = true;
                    this.upPnlNDocAcc12mesi.Update();

                    this.pnlGrdDoc12mesi.Visible = false;
                    this.upPnlGrdDoc12mesi.Update();
                    break;

                case "4":
                    this.pnlChartPieGrid.Visible = false;
                    this.upPnlChartPieGrid.Update();

                    this.pnlNDocAcc5anni.Visible = false;
                    this.upPnlNDocAcc5anni.Update();

                    this.pnlNDocAcc12mesi.Visible = false;
                    this.upPnlNDocAcc12mesi.Update();

                    this.pnlGrdDoc12mesi.Visible = true;
                    this.upPnlGrdDoc12mesi.Update();
                    break;
            }
        }


    }
}